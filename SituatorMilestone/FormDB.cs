using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SituatorMilestone
{
    public partial class FormDB : Form
    {
        public FormDB()
        {
            InitializeComponent();
        }

        private const string ClosedIncidents =
            "SELECT IncidentID FROM dbo.Incidents where Status = 3 and (ClosureTime > {0} and ClosureTime < {1})";

        private void button1_Click(object sender, EventArgs e)
        {
            var incidentIds = GetClosedIncidents();
            if (!incidentIds.Any()) MessageBox.Show("no closed incidents found in time range");
        }

        private IEnumerable<int> GetClosedIncidents()
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["connectionString"]))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                var toTicks = DateTime.Now.Ticks;
                var fromTicks = DateTime.Now.AddDays(-10).Ticks;
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
    }
}
