using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
//using VISSIMLIB;

namespace VissimSimulator
{
    public class EventSimulator
    {
#region private fields
        private const string CellLinkRelationFilePath = @".\Input\Taicang_Major_Cell_Link_Related.csv";
        private const string CellLocationRelationFilePath  = @".\Input\Taicang_Major_Cells.csv";
        private const char Delimiter = ',';

        private CellularNetwork cellularNetwork = new CellularNetwork();
        private Dictionary<string, VehicleEvent> VehicleEvents = new Dictionary<string, VehicleEvent>();
        private ConcurrentQueue<CellularTowerEvent> CellularTowerEvents = new ConcurrentQueue<CellularTowerEvent>();
        private List<CollectorWorker> CollectorWorkers;
#endregion //end private fields

#region public methods
        public void Run()
        {
            //Vissim vissim = new Vissim();
            ///Load Vissim net work
            //VissimSimulator.LoadNet(@"C:\Users\Public\Documents\PTV Vision\PTV Vissim 6\Examples Demo\Urban Intersection Beijing.CN\Intersection Beijing.inpx");
            ///Read table contains Cellular tower information and correspoing link information

            ///Generate the random event, when vehicle passing a fixed location and the Timespan is satisfied.
            CellularTowerEvent cte = new CellularTowerEvent();
            foreach (IVehicle vehicle in vissim.Net.Vehicles)
            {
                ///Select Random Vehicle
                int vehiclePossible = rnd.Next(0, 10);
                //Only Selected Vehicle can generate the Event.
                if (vehiclePossible == 0)
                {
                    Event evet = new Event();
                    ///Create random event type.
                    int i = rnd.Next(0, 1);
                    evet.TimeSpan = EventFactory.CreateTimeSpan();
                    evet.EventType = EventFactory.CreateEventType;
                    cte.Event = evet;
                }
                ///record the event information 
                CollectorWorker collect = new CollectorWorker();
                if (vehicle.Location == cte.CellTowerId & cte.Event.TimeSpan == simulationTime)
                {
                    collect.ProcessEvent(cte);
                }
                ///Make the program check the all vehicle in Vissim network every 1 sec.
                for (int i = 0; i < simulationTime; i++)
                {
                    vissim.Simulation.RunSingleStep();
                    foreach (CollectorWorker worker in CollectorWorkers)
                    {
                        Task workerTask = Task.Run(() =>
                        {
                            worker.ProcessEvent(CellularTowerEvents);
                        });

                        workerTask.Wait();
                    }
                }
            }
        }

        public void Exit()
        {
            //vissim.Exit();
        }

#endregion //end public methods


#region private methods

        /// <summary>
        /// Read the csv files to initialize the cellular network
        /// The format of the CellLinkRelation file is as follows:
        /// LINK_ID,CELLID,LAC
        /// the format of the CellLocationRelation file is as follows:
        /// LAC,CELLID
        /// </summary>
        private void LoadCellularNetwork()
        {
            //read the cell-location relation file
            using (StreamReader cellLinkReader = new StreamReader(File.OpenRead(CellLinkRelationFilePath)))
            {
                //skip the header line
                string line = cellLinkReader.ReadLine();

                //read the rest of the file
                while ((line = cellLinkReader.ReadLine()) != null)
                {
                    string[] values = line.Split(Delimiter);

                    string locationId = values[2];
                    Location location;

                    if (!cellularNetwork.ContainsLocation(locationId))
                    {
                        location = new Location(locationId);
                        location.CellTowers.Add()
                    }
                    
                }
            }

            //read the cell-link relation file
            using (StreamReader cellLinkReader = new StreamReader(File.OpenRead(CellLinkRelationFilePath)))
            {
                //skip the header line
                string line = cellLinkReader.ReadLine();

                //read the rest of the file
                while ((line = cellLinkReader.ReadLine()) != null)
                {
                    string[] values = line.Split(Delimiter);

                    CellularNetwork.
                }
            }
        }

#endregion 
    
    
    }

   
    public class CollectorWorker
    {
        public void ProcessEvent(ConcurrentQueue<CellularTowerEvent> cellularTowerEvents)
        {
            CellularTowerEvent evt = null;
            if (cellularTowerEvents.TryDequeue(out evt))
            {
                process(evt);
            }
        }

        private void process(CellularTowerEvent evt)
        {


        }
    }
}
