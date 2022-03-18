using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueTesting;
using MultiQueueModels;

namespace MultiQueueSimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FileNameForum());
            if (SimulationSystem.PATH.Length > 0)
            {
                SimulationSystem system = new SimulationSystem();
                system.ReadInput();
                system.Simulate();
                system.CalculatePerformanceMeasures();
                if (SimulationSystem.PATH[SimulationSystem.PATH.Length - 5] == '1')
                {
                    MessageBox.Show("Test Case 1");
                    string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
                    MessageBox.Show(result);
                }
                else if(SimulationSystem.PATH[SimulationSystem.PATH.Length - 5] == '2')
                {
                    MessageBox.Show("Test Case 2");
                    string result = TestingManager.Test(system, Constants.FileNames.TestCase2);
                    MessageBox.Show(result);
                }
                else
                {
                    MessageBox.Show("Test Case 3");
                    string result = TestingManager.Test(system, Constants.FileNames.TestCase3);
                    MessageBox.Show(result);
                }
                Application.Run(new DataView(system));
            }
           
        }
    }
}
