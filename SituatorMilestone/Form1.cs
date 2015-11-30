using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace SituatorMilestone
{
    public partial class Form1 : Form
    {
        private HttpClient _client;
        private IEnumerable<MilestoneTask> _milestoneTasks;
        private string _sessionToken;
        public Form1()
        {
            InitializeComponent();
            textBoxIncidentId.Text = ConfigurationManager.AppSettings["incidentId"];
            textBoxSitWebApi.Text = ConfigurationManager.AppSettings["situatorWebApiUrl"];
            dateTimePickerDateProg.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateProg.CustomFormat = "dd-MM-yyyy hh:mm:ss";
            dateTimePickerDateTis.Format = DateTimePickerFormat.Custom;
            dateTimePickerDateTis.CustomFormat = "dd-MM-yyyy hh:mm:ss";
        }

        private async Task<HttpClient> GetHttpClient()
        {
            var cookieContainer = new CookieContainer();
            //var token = textBoxToken.Text; //await Login();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            var result = new HttpClient(handler) { BaseAddress = new Uri(GetBaseAddress()) };
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (string.IsNullOrWhiteSpace(_sessionToken))
            {
                _sessionToken = await LogIn(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
            }
            cookieContainer.Add(result.BaseAddress, new Cookie("session-token", _sessionToken));

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            if (_client == null)
            {
                _client = await GetHttpClient();
            }

            _milestoneTasks = (await GetMilestones(_client, textBoxSitWebApi.Text, textBoxIncidentId.Text)).ToList();
            //  textBoxMilestones.Text += milestoneIds.Aggregate((a, b) => a + " " + b);

            foreach (var milestoneTask in _milestoneTasks)
            {
                //var milestoneTask = await GetTask(_client, textBoxSitWebApi.Text, t);
                textBoxMilestones.Text += Environment.NewLine;
                textBoxMilestones.Text += "task " + milestoneTask.Name + Environment.NewLine;
                foreach (var te in milestoneTask.TaskEvents)
                {
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
            var doc = XDocument.Parse(content);

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

        private static async Task<IEnumerable<MilestoneTask>> GetMilestones(HttpClient client, string urlSit, string incidentId)
        {
            var result = new List<MilestoneTask>();
            // TODO: milestone tasks on closed incidents does not work. Will be fixed in 26 nov Situator Drop.
            var url = string.Format("{0}/odata/Incidents({1})?$expand=DTasks/DTaskEvents", urlSit, incidentId);
            //var url = string.Format("{0}/odata/incidents?$filter=IncidentID eq {1} and Status eq 'Closed'&$expand=DTasks", urlSit, incidentId);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return result;

            var res = await response.Content.ReadAsStringAsync();
            var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
            var dTasks = myDyn.DTasks;
            var jValuePrognose = ((IEnumerable<dynamic>)dTasks).FirstOrDefault(dt =>
               (dt.TaskType == "MilestoneTask") && (dt.Name == "PrognoseVerloop"));
            if (jValuePrognose != null)
            {
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

        private string GetBaseAddress()
        {
            var uri = new Uri(textBoxSitWebApi.Text, UriKind.Absolute);
            //remove segments
            var noLastSegment = uri.GetComponents(UriComponents.SchemeAndServer,
                UriFormat.SafeUnescaped);
            return noLastSegment;
        }

        private Uri GetSituatorWebApiUrl()
        {
            return new Uri(textBoxSitWebApi.Text, UriKind.Absolute);
        }

        private async Task<string> LogIn(string user, string password)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(user, password);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-Version", "1");
            var sitUri = GetSituatorWebApiUrl();
            var url = sitUri + "/" + "api/Login";
            //string url = GetBaseAddress() + "/" + "api/Login";
            var response = await client.PostAsync(url, null);
            if (!response.IsSuccessStatusCode) return string.Empty;

            var responseCookies = cookies.GetCookies(sitUri).Cast<Cookie>();
            foreach (var cookie in responseCookies)
            {
                if (cookie.Name == "session-token")
                {
                    return cookie.Value;
                }
            }
            return string.Empty;
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
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", GetSituatorWebApiUrl(), tisTask.Id);
            var umt = new UpdateMilestoneTask { Comment = textBoxCommentTis.Text, PlannedEndTime = ticks.ToString() };
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await _client.PostAsync(url, content);
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
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", GetSituatorWebApiUrl(), progTask.Id);
            var umt = new UpdateMilestoneTask { Comment = textBoxCommentProg.Text, PlannedEndTime = ticks.ToString() };
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await _client.PostAsync(url, content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result);
            }
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

