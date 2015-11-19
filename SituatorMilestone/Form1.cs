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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SituatorMilestone
{
    public partial class Form1 : Form
    {
        private HttpClient client;
        private IEnumerable<MilestoneTask> milestoneTasks;
        private string sessionToken;
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

        private HttpClient GetHttpClient()
        {
            var cookieContainer = new CookieContainer();
            //var token = textBoxToken.Text; //await Login();
            var handler = new HttpClientHandler {CookieContainer = cookieContainer};
            var result = new HttpClient(handler) {BaseAddress = new Uri(GetBaseAddress())};
            result.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                sessionToken = LogIn(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["password"]);
            }
            cookieContainer.Add(result.BaseAddress, new Cookie("session-token", sessionToken));

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                client = GetHttpClient();
            }
            
            milestoneTasks = (await GetMilestoneIds(client, textBoxSitWebApi.Text, textBoxIncidentId.Text)).ToList();
          //  textBoxMilestones.Text += milestoneIds.Aggregate((a, b) => a + " " + b);
            textBoxMilestones.Text += Environment.NewLine;
            foreach (var t in milestoneTasks)
            {
                var milestoneTask = await GetTask(client, textBoxSitWebApi.Text, t);
                textBoxMilestones.Text += "task " + Environment.NewLine;
                foreach (var te in milestoneTask.TaskEvents)
                {
                    textBoxMilestones.Text += "Task event " + Environment.NewLine;
                    var taskEvent = ProcessContent(te);
                    textBoxMilestones.Text += string.Format("{0}, Reporttime: {1}, Comment: {2}, PlannedEndTime: {3}", taskEvent.UserJobTitleName,
                        taskEvent.ReportTimeDateTime, taskEvent.Comment, taskEvent.PlannedEndTime);
                }
            }
        }

        private TaskEvent ProcessContent(TaskEvent taskEvent)
        {
            var doc = XDocument.Parse(taskEvent.Content);
            foreach (var el in doc.Root.Elements())
            {
                Console.WriteLine("{0} {1}", el.Name, el.Value);
                if (el.Name == "PlannedEndTime")
                {
                    var ticks = long.Parse(el.Value);
                    if (ticks > 0)
                    {
                        taskEvent.PlannedEndTime = new DateTime(ticks);
                    }

                }
                else if (el.Name == "UserJobTitleName")
                {
                    taskEvent.UserJobTitleName = el.Value;
                }
                else if (el.Name == "Comment")
                {
                    taskEvent.Comment = el.Value;
                }
            }
            return taskEvent;
        }
    
        private static async Task<MilestoneTask> GetTask(HttpClient client, string urlSit, MilestoneTask milestoneTask)
        {
            var url = string.Format("{0}/odata/DTasks({1})?$expand=DTaskEvents", urlSit, milestoneTask.Id);
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
                var dTaskEvents = myDyn.DTaskEvents;
                var taskEvents = ((IEnumerable<dynamic>) dTaskEvents).Select(dt => 
                    new TaskEvent
                    {
                        Content = dt.Content,
                        ReportTimeDateTime = DateTime.Parse(dt.ReportTimeDateTime.ToString(), CultureInfo.InvariantCulture),                       
                    });
                milestoneTask.TaskEvents = HandleTaskContent(taskEvents);
            }
            return milestoneTask;
        }

        private static IEnumerable<TaskEvent> HandleTaskContent(IEnumerable<TaskEvent> taskEvents)
        {
            var localTaskEvents = taskEvents.ToList();
            foreach (var te in localTaskEvents)
            {
                if (string.IsNullOrWhiteSpace(te.Content))
                {
                    var content = Regex.Unescape(te.Content);
                    content = content.Replace("\r\n", "").Replace("\n", "");
                    var doc = XDocument.Parse(content);

                    foreach (var el in doc.Root.Elements())
                    {
                        if (el.Name == "PlannedEndTime")
                        {
                            var ticks = long.Parse(el.Value);
                            te.PlannedEndTime = new DateTime(ticks);
                        }
                        else if (el.Name == "Comment")
                        {
                            te.Comment = el.Value;
                        }
                        else if (el.Name == "UserJobTitleName")
                        {
                            te.UserJobTitleName = el.Value;
                        }
                    }
                }
            }
            return localTaskEvents;
        }
    

    private static async Task<IEnumerable<MilestoneTask>> GetMilestoneIds(HttpClient client, string urlSit, string incidentId)
        {
            var result = new List<MilestoneTask>();
            // TODO: milestone tasks on closed incidents does not work. Will be fixed in 26 nov Situator Drop.
            var url = string.Format("{0}/odata/Incidents({1})?$expand=DTasks", urlSit, incidentId);
            //var url = string.Format("{0}/odata/incidents?$filter=IncidentID eq {1} and Status eq 'Closed'&$expand=DTasks", urlSit, incidentId);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return result;

            var res = await response.Content.ReadAsStringAsync();
            var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
            var dTasks = myDyn.DTasks;
            var jValuePrognose = ((IEnumerable<dynamic>) dTasks).FirstOrDefault(dt => 
                (dt.TaskType == "MilestoneTask") && (dt.Name == "PrognoseVerloop"));
            result.Add(new MilestoneTask {Id = jValuePrognose.TaskID, Name = jValuePrognose.Name});
            var jValueScenario = ((IEnumerable<dynamic>)dTasks).FirstOrDefault(dt =>
                (dt.TaskType == "MilestoneTask") && (dt.Name == "ScenarioVerloop"));
            result.Add(new MilestoneTask { Id = jValueScenario.TaskID, Name = jValueScenario.Name });
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

        public string LogIn(string user, string password)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };
            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(user, password);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-Version", "1");
            var sitUri = GetSituatorWebApiUrl();
            var url = sitUri + "/"  + "api/Login";
            //string url = GetBaseAddress() + "/" + "api/Login";
            HttpResponseMessage response = client.PostAsync(url, null).Result;

            var uri = new Uri(GetBaseAddress() + "/");
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
            if (milestoneTasks == null)
            {
                button1_Click(sender, e);
            }
            if (milestoneTasks == null) return;
            var tisTask = milestoneTasks.FirstOrDefault(m => m.Name.ToLower().Contains("scenario"));
            if (tisTask == null) return;
            
            var ticks = dateTimePickerDateTis.Value.Ticks;
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", GetSituatorWebApiUrl(), tisTask.Id);
            var umt = new UpdateMilestoneTask {Comment = textBoxCommentTis.Text, PlannedEndTime = ticks.ToString()};
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await client.PostAsync(url, content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result);
            }
        }

        private async void buttonUpdateProg_Click(object sender, EventArgs e)
        {
            if (milestoneTasks == null)
            {
                button1_Click(sender, e);
            }
            if (milestoneTasks == null) return;
            var progTask = milestoneTasks.FirstOrDefault(m => m.Name.ToLower().Contains("prog"));
            if (progTask == null) return;

            var ticks = dateTimePickerDateProg.Value.Ticks;
            var url = string.Format("{0}/odata/DTasks({1})/UpdateEndTime", GetSituatorWebApiUrl(), progTask.Id);
            var umt = new UpdateMilestoneTask { Comment = textBoxCommentTis.Text, PlannedEndTime = ticks.ToString() };
            var formatter = new JsonMediaTypeFormatter { SerializerSettings = { NullValueHandling = NullValueHandling.Ignore } };
            var content = new ObjectContent<UpdateMilestoneTask>(umt, formatter);
            var response = await client.PostAsync(url, content);
            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}

public class UpdateMilestoneTask
{
    public string PlannedEndTime { get; set; }
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
            base("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)))) { }
    }

