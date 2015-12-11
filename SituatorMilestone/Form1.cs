using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SituatorMilestone
{
    public partial class Form1 : Form
    {
        private IEnumerable<MilestoneTask> _milestoneTasks;
        public Form1()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            InitializeComponent();
            textBoxIncidentId.Text = ConfigurationManager.AppSettings["incidentId"];
            textBoxSitWebApi.Text = ConfigurationManager.AppSettings["situatorWebApiUrl"];
            dateTimePickerDateProg.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateProg.CustomFormat = "dd-MM-yyyy hh:mm:ss";
            dateTimePickerDateTis.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateTis.CustomFormat = "dd-MM-yyyy hh:mm:ss";
            comboBoxQuery.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            var httpClient = await PreparedHttpClient.GetInstance();

            var query = comboBoxQuery.SelectedItem.ToString();
            _milestoneTasks = (await GetMilestones(httpClient, textBoxSitWebApi.Text, textBoxIncidentId.Text, query)).ToList();
            //  textBoxMilestones.Text += milestoneIds.Aggregate((a, b) => a + " " + b);

            foreach (var milestoneTask in _milestoneTasks)
            {
                //var milestoneTask = await GetTask(_client, textBoxSitWebApi.Text, t);
                textBoxMilestones.Text += Environment.NewLine;
                textBoxMilestones.Text += "task " + milestoneTask.Name + Environment.NewLine;
                foreach (var te in milestoneTask.TaskEvents)
                {
                    if (te == null) continue;
                    var taskEvent = te;
                    textBoxMilestones.Text += string.Format("{0}, Reporttime: {1}, Comment: {2}, PlannedEndTime: {3}", taskEvent.UserJobTitleName,
                        taskEvent.ReportTimeDateTime, taskEvent.Comment, taskEvent.PlannedEndTime);
                    textBoxMilestones.Text += Environment.NewLine;
                }
            }
        }
        private static IEnumerable<TaskEvent> GetTaskEvents(dynamic dTaskEvents)
        {
            return (from dyn in (IEnumerable<dynamic>)dTaskEvents select GetTaskEvent(dyn)).Cast<TaskEvent>();
        }

        private static TaskEvent GetTaskEvent(dynamic dTaskEvent)
        {
            string content = dTaskEvent.Content;
            if (string.IsNullOrWhiteSpace(content)) return null;
            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(content);
            }
            if (doc == null) return null; 

            if (doc.Root == null) return null;
            var result = new TaskEvent
            {
                ReportTimeDateTime = DateTime.Parse(dTaskEvent.ReportTimeDateTime.ToString(),
                    CultureInfo.InvariantCulture)
            };
            foreach (var el in doc.Root.Elements())
            {
                if (el.Name == "PlannedEndTime")
                {
                    var ticks = long.Parse(el.Value);
                    if (ticks > 0)
                    {
                        result.PlannedEndTime = new DateTime(ticks);
                    }
                }
                else if (el.Name == "Comment")
                {
                    result.Comment = el.Value;
                }
                else if (el.Name == "UserJobTitleName")
                {
                    result.UserJobTitleName = el.Value;
                }
            }
            return result;
        }

        private static async Task<IEnumerable<MilestoneTask>> GetMilestones(HttpClient client, string urlSit, string incidentId, string query)
        {
            var result = new List<MilestoneTask>();
            // TODO: milestone tasks on closed incidents does not work. Will be fixed in 26 nov Situator Drop.
            
            var url = string.Format(query, urlSit, incidentId);
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("not found: " + url);
                }
                return result;
                
            }

            var res = await response.Content.ReadAsStringAsync();
            var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
            var dTasks = myDyn.DTasks;
            var jValuePrognose = ((IEnumerable<dynamic>)dTasks).FirstOrDefault(dt =>
               (dt.TaskType == "MilestoneTask") && (dt.Name == "PrognoseVerloop"));
            if (jValuePrognose != null)
            {
                var mmm = GetTaskEvents(jValuePrognose.DTaskEvents);
                foreach (var tev in mmm)
                {
                    try
                    {
                        var progg = JsonConvert.DeserializeObject<PrognoseDTaskEventComment>(tev.Comment);
                        if (progg.DatumTijd > DateTime.MinValue) Console.WriteLine("huu: " + progg.DatumTijd.ToLocalTime());
                        Console.WriteLine(progg);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }                    
                }
                result.Add(new MilestoneTask { TaskEvents = GetTaskEvents(jValuePrognose.DTaskEvents), Id = jValuePrognose.TaskID, Name = jValuePrognose.Name });
            }
            var jValueScenario = ((IEnumerable<dynamic>)dTasks).FirstOrDefault(dt =>
                    (dt.TaskType == "MilestoneTask") && (dt.Name == "ScenarioVerloop"));
            if (jValueScenario != null)
            {
                result.Add(new MilestoneTask() { TaskEvents = GetTaskEvents(jValueScenario.DTaskEvents), Id = jValueScenario.TaskID, Name = jValueScenario.Name });
            }
            return result;
        }

       

       

      

        private async void buttonUpdateTis_Click(object sender, EventArgs e)
        {
            if (_milestoneTasks == null)
            {
                button1_Click(sender, e);
            }
            if (_milestoneTasks == null) return;
            var tisTask = _milestoneTasks.FirstOrDefault(m => m.Name.ToLower().Contains("scenario"));
            if (tisTask == null) return;

            var ticks = dateTimePickerDateTis.Value.Ticks;
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", PreparedHttpClient.GetSituatorWebApiUrl(), tisTask.Id);

            var commentScenario = new ScenarioDTaskEventComment
            {
                Tis = comboBoxTis.SelectedItem.ToString(),
                WijzigingsType = comboBoxTisWijzigingsType.SelectedItem.ToString()
            };
            var httpClient = await PreparedHttpClient.GetInstance();
            var json = JsonConvert.SerializeObject(commentScenario);
            var umt = new UpdateMilestoneTask { Comment = json, PlannedEndTime = ticks.ToString() };
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await httpClient.PostAsync(url, content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result);
            }
        }

        private async void buttonUpdateProg_Click(object sender, EventArgs e)
        {
            if (_milestoneTasks == null)
            {
                button1_Click(sender, e);
            }
            if (_milestoneTasks == null) return;
            var progTask = _milestoneTasks.FirstOrDefault(m => m.Name.ToLower().Contains("prog"));
            if (progTask == null) return;

            var ticks = dateTimePickerDateProg.Value.Ticks;
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", PreparedHttpClient.GetSituatorWebApiUrl(), progTask.Id);
            var progComment = new PrognoseDTaskEventComment
            {
                Duur = Convert.ToInt32(comboBoxProgDuur.SelectedItem),
                HandmatigGezetIndicatie = comboBoxProgHGI.SelectedItem.ToString(),
                Type = comboBoxProgType.SelectedItem.ToString(),
                RedenWijziging = textBoxProgRedenWijziging.Text,
                DatumTijd = DateTime.Now.ToUniversalTime()
            };
            var httpClient = await PreparedHttpClient.GetInstance();
            var json = JsonConvert.SerializeObject(progComment, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ" });
            var umt = new UpdateMilestoneTask { Comment = json, PlannedEndTime = ticks.ToString() };
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await httpClient.PostAsync(url, content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result);
            }
        }

        private void queryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FormDB();
            f.Show(this);
        }
        //private static async Task<MilestoneTask> GetTask(HttpClient client, string urlSit, MilestoneTask milestoneTask)
        //{
        //    var url = string.Format("{0}/odata/DTasks({1})?$expand=DTaskEvents", urlSit, milestoneTask.Id);
        //    var response = await client.GetAsync(url);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var res = await response.Content.ReadAsStringAsync();
        //        var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
        //        var dTaskEvents = myDyn.DTaskEvents;
        //        var taskEvents = ((IEnumerable<dynamic>)dTaskEvents).Select(dt =>
        //           new TaskEvent
        //           {
        //               Content = dt.Content,
        //               ReportTimeDateTime = DateTime.Parse(dt.ReportTimeDateTime.ToString(), CultureInfo.InvariantCulture),
        //           });
        //        milestoneTask.TaskEvents = HandleTaskContent2(taskEvents);
        //    }
        //    return milestoneTask;
        //}

        //private static IEnumerable<TaskEvent> HandleTaskContent2(IEnumerable<TaskEvent> taskEvents)
        //{
        //    var localTaskEvents = taskEvents.ToList();
        //    foreach (var te in localTaskEvents)
        //    {
        //        if (string.IsNullOrWhiteSpace(te.Content)) continue;
        //        var content = te.Content;
        //        var doc = XDocument.Parse(content);

        //        if (doc.Root == null) continue;
        //        foreach (var el in doc.Root.Elements())
        //        {
        //            if (el.Name == "PlannedEndTime")
        //            {
        //                var ticks = long.Parse(el.Value);
        //                if (ticks > 0)
        //                {
        //                    te.PlannedEndTime = new DateTime(ticks);
        //                }
        //            }
        //            else if (el.Name == "Comment")
        //            {
        //                te.Comment = el.Value;
        //            }
        //            else if (el.Name == "UserJobTitleName")
        //            {
        //                te.UserJobTitleName = el.Value;
        //            }
        //        }
        //    }
        //    return localTaskEvents;
        //}
    }
}

public class ScenarioDTaskEventComment
{
    public string Tis { get; set; }
    public string WijzigingsType { get; set; }
}

public class PrognoseDTaskEventComment
{
    public string Type { get; set; }
    public int Duur { get; set; }
    public string HandmatigGezetIndicatie { get; set; }
    public string RedenWijziging { get; set; }
    public DateTime DatumTijd { get; set; }
}


public class UpdateMilestoneTask
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global    
    public string PlannedEndTime { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Comment { get; set; }
}

public class MilestoneTask
{
    public string Name { get; set; }
    public string Id { get; set; }
    public IEnumerable<TaskEvent> TaskEvents { get; set; }
}

public class TaskEvent
{
    public DateTime ReportTimeDateTime { get; set; }
    public string Content { get; set; }
    public string Comment { get; set; }
    public string UserJobTitleName { get; set; }
    public DateTime PlannedEndTime { get; set; }
}

public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    public BasicAuthenticationHeaderValue(string username, string password) :
        base("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))))
    { }
}

