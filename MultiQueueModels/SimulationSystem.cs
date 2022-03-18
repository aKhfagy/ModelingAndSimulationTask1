using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }

        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }
        public static string PATH = "";
        public int stoppingCriteriaIndex { get; set; }
        public int selectionMethodIndex { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public void ReadInput()
        {
            string[] lines = File.ReadAllLines(PATH);

            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line == "NumberOfServers")
                {
                    ++i;
                    this.NumberOfServers = int.Parse(lines[i]);
                    ++i;
                    ++i;
                }
                else if (line == "StoppingNumber")
                {
                    ++i;
                    this.StoppingNumber = int.Parse(lines[i]);
                    ++i;
                    ++i;
                }
                else if (line == "StoppingCriteria")
                {
                    ++i;
                    this.stoppingCriteriaIndex = int.Parse(lines[i]);
                    ++i;
                    ++i;
                }
                else if (line == "SelectionMethod")
                {
                    ++i;
                    this.selectionMethodIndex = int.Parse(lines[i]);
                    ++i;
                    ++i;
                }
                else if (line == "InterarrivalDistribution")
                {
                    ++i;
                    int index = 0;
                    TimeDistribution timeDistributionPrev = new TimeDistribution();

                    while (i < lines.Length && lines[i] != "")
                    {
                        string[] numbers = lines[i].Split(',');
                        int time = int.Parse(numbers[0]);
                        decimal destribution = decimal.Parse(numbers[1]);
                        TimeDistribution timeDistribution = new TimeDistribution();
                        timeDistribution.Time = time;
                        timeDistribution.Probability = destribution;
                        if (index > 0)
                        {
                            timeDistribution.CummProbability = timeDistributionPrev.CummProbability + destribution;
                            timeDistribution.MinRange = (int)(timeDistributionPrev.CummProbability * 100) + 1;
                            timeDistribution.MaxRange = (int)(timeDistribution.CummProbability * 100);
                        }
                        else
                        {
                            timeDistribution.CummProbability = destribution;
                            timeDistribution.MinRange = 1;
                            timeDistribution.MaxRange = (int)(timeDistribution.CummProbability * 100);
                        }
                        this.InterarrivalDistribution.Add(timeDistribution);
                        timeDistributionPrev = timeDistribution;
                        ++index;
                        ++i;
                    }
                    ++i;
                }
                else
                {
                    ++i;
                    int index = 0;
                    TimeDistribution timeDistributionPrev = new TimeDistribution();
                    Server server = new Server();
                    server.ID = this.Servers.Count+1;
                    while (i < lines.Length && lines[i] != "")
                    {
                        string[] numbers = lines[i].Split(',');
                        int time = int.Parse(numbers[0]);
                        decimal destribution = decimal.Parse(numbers[1]);
                        TimeDistribution timeDistribution = new TimeDistribution();
                        timeDistribution.Time = time;
                        timeDistribution.Probability = destribution;
                        if (index > 0)
                        {
                            timeDistribution.CummProbability = timeDistributionPrev.CummProbability + destribution;
                            timeDistribution.MinRange = (int)(timeDistributionPrev.CummProbability * 100) + 1;
                            timeDistribution.MaxRange = (int)(timeDistribution.CummProbability * 100);
                        }
                        else
                        {
                            timeDistribution.CummProbability = destribution;
                            timeDistribution.MinRange = 1;
                            timeDistribution.MaxRange = (int)(timeDistribution.CummProbability * 100);
                        }
                        server.TimeDistribution.Add(timeDistribution);
                        timeDistributionPrev = timeDistribution;
                        ++index;
                        ++i;
                    }
                    ++i;
                    this.Servers.Add(server);
                }
            }
        }

        public int GetInterArrivalTime(int randomArrival)
        {
            int interArrivalTime = 0;
            foreach (TimeDistribution timeDistribution in this.InterarrivalDistribution)
            {
                if (randomArrival >= timeDistribution.MinRange && randomArrival <= timeDistribution.MaxRange)
                {
                    interArrivalTime = timeDistribution.Time;
                }
            }
            return interArrivalTime;
        }

        public int[] GetServerWaitingTimes(int randomService)
        {
            int[] serverServiceTime = new int[NumberOfServers];
            int serverNumber = 0;
            foreach (Server server in this.Servers)
            {
                foreach (TimeDistribution timeDistribution in server.TimeDistribution)
                {
                    if (randomService >= timeDistribution.MinRange && randomService <= timeDistribution.MaxRange)
                    {
                        serverServiceTime[serverNumber] = timeDistribution.Time;
                    }
                }
                serverNumber++;
            }
            return serverServiceTime;
        }

        public void InitializeServerFinishTimes()
        {
            for (int i = 0; i < this.NumberOfServers; ++i)
            {
                Servers[i].FinishTime = -1;
            }
        }

        public void FreeServers(int currTime)
        {
            for (int i = 0; i < this.NumberOfServers; ++i)
            {
                if(currTime >= Servers[i].FinishTime)
                    Servers[i].FinishTime = -1;
            }
        }

        public void CalculatePerformanceMeasures()
        {
            decimal peopleWhoWaited = 0;
            Queue<int> startTimes = new Queue<int>();
            PerformanceMeasures.MaxQueueLength = 0;
            foreach(SimulationCase simulationCase in this.SimulationTable)
            {
                while(startTimes.Count > 0 && startTimes.Peek() <= simulationCase.ArrivalTime)
                {
                    startTimes.Dequeue();
                }
                if(simulationCase.TimeInQueue != 0)
                {
                    peopleWhoWaited++;
                    startTimes.Enqueue(simulationCase.StartTime);
                    PerformanceMeasures.MaxQueueLength = Math.Max(PerformanceMeasures.MaxQueueLength, startTimes.Count);
                    PerformanceMeasures.AverageWaitingTime += simulationCase.TimeInQueue;
                }
            }
            PerformanceMeasures.AverageWaitingTime = PerformanceMeasures.AverageWaitingTime / 100;
            PerformanceMeasures.WaitingProbability = peopleWhoWaited / 100;
        }
        
        public void CalculateServerPerformance()
        {
            
        }

        public void Simulate()
        {
            Random random = new Random();
            SimulationCase simulationCasePrev = new SimulationCase();
            for(int i = 1; i <= 100; ++i)
            {
                int randomArrival = random.Next(1, 100);
                int randomService = random.Next(1, 100);
                int interArrivalTime = this.GetInterArrivalTime(randomArrival);
                int[] serverServiceTime = this.GetServerWaitingTimes(randomService);
                int arrivalTime = (i == 1) ? 0 : simulationCasePrev.ArrivalTime + interArrivalTime;
                this.FreeServers(arrivalTime);
                SimulationCase simulationCaseNext = new SimulationCase();
                simulationCaseNext.ArrivalTime = arrivalTime;
                simulationCaseNext.InterArrival = (i == 1) ? -1 : interArrivalTime;
                simulationCaseNext.RandomInterArrival = randomArrival;
                simulationCaseNext.CustomerNumber = i;
                simulationCaseNext.RandomService = randomService;
                int serverIdx = -1;
                int minFinishTime = 1000000;
                bool oneServerIsEmpty = false;
                
                for (int j = 0; j < this.NumberOfServers; ++j)
                {
                    if(Servers[j].FinishTime == -1)
                    {
                        serverIdx = j;
                        oneServerIsEmpty = true;
                        break;
                    }
                    else
                    {
                        if(Servers[j].FinishTime < minFinishTime)
                        {
                            serverIdx = j;
                            minFinishTime = Servers[j].FinishTime;
                        }
                    }
                }
                if(oneServerIsEmpty == false)
                {
                    simulationCaseNext.TimeInQueue = minFinishTime - arrivalTime;
                    simulationCaseNext.StartTime = minFinishTime;
                    simulationCaseNext.EndTime = minFinishTime + serverServiceTime[serverIdx];
                    simulationCaseNext.ServiceTime = serverServiceTime[serverIdx];
                    Servers[serverIdx].FinishTime = simulationCaseNext.EndTime;
                    simulationCaseNext.AssignedServer = Servers[serverIdx];
                }
                else
                {
                    simulationCaseNext.TimeInQueue = 0;
                    simulationCaseNext.StartTime = arrivalTime;
                    simulationCaseNext.EndTime = arrivalTime + serverServiceTime[serverIdx];
                    simulationCaseNext.ServiceTime = serverServiceTime[serverIdx];
                    Servers[serverIdx].FinishTime = simulationCaseNext.EndTime;
                    simulationCaseNext.AssignedServer = Servers[serverIdx];
                }


                this.SimulationTable.Add(simulationCaseNext);

                simulationCasePrev = simulationCaseNext;
            }
        }

    }
}
