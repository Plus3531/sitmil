using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SituatorMilestone
{
    public partial class FormDB : Form
    {
        public FormDB()
        {
            InitializeComponent();
        }
        //TODO: get incidentType from database
        private const string ClosedIncidents =
            "SELECT IncidentID FROM dbo.Incidents where Status = 3 and (ClosureTime > {0} and ClosureTime < {1}) and IncidentType = 15444";

        private const string Ieps =
            "SELECT [Name], [Value] FROM [IncidentExtendedProperties] Where IncidentID = {0} and Category = 'IEP'";

        private async void button1_Click(object sender, EventArgs e)
        {
            var incidentsDictionary = await GetIncidentTree();
            //get the content of the (sub)incidents
            foreach (var item in incidentsDictionary)
            {
                var intakeId = item.Key;
                var ieps = GetIeps(intakeId);
                foreach (var iep in ieps) Console.WriteLine(iep.Name + ": " + iep.Value);
            }
        }

        private async Task<Dictionary<int, IEnumerable<int>>> GetIncidentTree()
        {
            //intake incidentId=key, subincidentIds=value
            var incidentsDictionary = new Dictionary<int, IEnumerable<int>>();
            var incidentIds = GetClosedIncidents().ToList();
            if (!incidentIds.Any())
            {
                //MessageBox.Show("no closed incidents found in time range");
                return incidentsDictionary;
            }
            foreach (var id in incidentIds)
            {
                incidentsDictionary.Add(id, null);
                var httpClient = await PreparedHttpClient.GetInstance();
                const string query = "{0}/odata/Incidents({1})/ChildsId";
                var url = string.Format(query, PreparedHttpClient.GetSituatorWebApiUrl(), id);

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    continue;
                }

                var res = await response.Content.ReadAsStringAsync();
                var myDyn = JsonConvert.DeserializeObject<dynamic>(res);
                if (myDyn.value != null)
                {
                    var iids = new List<int>();
                    foreach (var v in myDyn.value)
                    {
                        iids.Add((int)v.Id);
                    }
                    incidentsDictionary[id] = iids;
                }

            }
            return incidentsDictionary;
        }

        private IEnumerable<int> GetClosedIncidents()
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                var toTicks = DateTime.Now.Ticks;
                var fromTicks = DateTime.Now.AddDays(-100).Ticks;
                cmd.CommandText = string.Format(ClosedIncidents, fromTicks, toTicks);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
        }
        private IEnumerable<Iep> GetIeps(int incidentId)
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
            
                cmd.CommandText = string.Format(Ieps, incidentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new Iep {Name = reader.GetString(0), Value = reader.GetString(1)};
                    }
                    reader.Close();
                }
            }
        }
    }

    public class Iep
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
