using MultiQueueModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiQueueSimulation
{
    public partial class DataView : Form
    {
        public DataView(SimulationSystem system)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            dt.Columns.Add("Customer No.", typeof(int));
            dt.Columns.Add("Random Digit For Arrivals", typeof(int));
            dt.Columns.Add("Time Between Arrivals", typeof(int));
            dt.Columns.Add("Clock Time of Arrivals", typeof(int));
            dt.Columns.Add("Random Digit For Service", typeof(int));
            for(int i = 0; i < system.NumberOfServers; i++)
            {
                dt.Columns.Add("Server " + (i + 1) + ": Time Service Begins", typeof(int));
                dt.Columns.Add("Server " + (i + 1) + ": Service Time", typeof(int));
                dt.Columns.Add("Server " + (i + 1) + ": Time Service Ends", typeof(int));
            }
            dt.Columns.Add("Time in Queue", typeof(int));

            for (int i = 0; i < system.SimulationTable.Count; i++)
            {
                SimulationCase simulationCase = system.SimulationTable[i];
                object[] obj = new object[dt.Columns.Count];
                int index = 0;
                obj[index++] = simulationCase.CustomerNumber;
                obj[index++] = (i == 0) ? 0 : simulationCase.RandomInterArrival;
                obj[index++] = simulationCase.InterArrival;
                obj[index++] = simulationCase.ArrivalTime;
                obj[index++] = simulationCase.RandomService;
                for (int serverId = 1; serverId <= system.NumberOfServers; ++serverId)
                {
                    if (serverId == simulationCase.AssignedServer.ID)
                    {
                        obj[index++] = simulationCase.StartTime;
                        obj[index++] = simulationCase.ServiceTime;
                        obj[index++] = simulationCase.EndTime;
                    }
                    else
                    {
                        obj[index++] = -1;
                        obj[index++] = -1;
                        obj[index++] = -1;
                    }
                }
                obj[index] = simulationCase.TimeInQueue;
                dt.Rows.Add(obj);
            }

            dataGridView1.DataSource = dt;

            dt = new DataTable();
            dt.Columns.Add("Average Waiting Time", typeof(decimal));
            dt.Columns.Add("Max Queue Length", typeof(int));
            dt.Columns.Add("Waiting Probability", typeof(decimal));
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                dt.Columns.Add("Server " + (i + 1) + ": Idle Probability", typeof(decimal));
                dt.Columns.Add("Server " + (i + 1) + ": Average Service Time", typeof(decimal));
                dt.Columns.Add("Server " + (i + 1) + ": Utilization", typeof(decimal));
            }
            object[] data = new object[dt.Columns.Count];
            int idx = 0;
            data[idx++] = system.PerformanceMeasures.AverageWaitingTime;
            data[idx++] = system.PerformanceMeasures.MaxQueueLength;
            data[idx++] = system.PerformanceMeasures.WaitingProbability;
            for (int i = 0; i < system.NumberOfServers; i++)
            {
                data[idx++] = system.Servers[i].IdleProbability;
                data[idx++] = system.Servers[i].AverageServiceTime;
                data[idx++] = system.Servers[i].Utilization;
            }
            dt.Rows.Add(data);
            dataGridView2.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataView_Load(object sender, EventArgs e)
        {

        }
    }
}
