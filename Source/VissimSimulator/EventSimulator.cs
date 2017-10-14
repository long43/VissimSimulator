using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
//using System.Configuration;
//using System.Data.SqlClient;
using VISSIMLIB;

namespace VissimSimulator
{
    public class EventSimulator
    {
        #region private fields
        private const string CellLinkRelationFilePath = @"C:\Users\Student\Desktop\VissimSimulator\Source\VissimSimulator\input\Taicang_Major_Cell_Link_Related.csv";
        private const string VissimEventsFilePath = @"C:\Users\Student\Desktop\VissimSimulator\Source\VissimSimulator\output\VehicleEvents.csv";
        private const string VissimSimulatorFilePath = @"C:\Users\Public\Documents\PTV Vision\PTV Vissim 6\Taicang.inpx";
        private const char Delimiter = ',';
        private const long SimulationTicks = 3600;

        //the cellular network
        private CellularNetwork cellularNetwork = null;

        //Dictionary that holds all vehicle events in the network
        private Dictionary<string, VehicleEvent> vehicleEvents = null;

        //BlockingCollection that holds all CellularTower Events
        private BlockingCollection<CellularTowerEvent> cellularTowerEvents = null;

        //Vissim simulator
        private Vissim vissim;
        #endregion //end private fields*

        #region public methods
        public EventSimulator()
        {
            cellularNetwork = new CellularNetwork();
            vehicleEvents = new Dictionary<string, VehicleEvent>();
            cellularTowerEvents = new BlockingCollection<CellularTowerEvent>();
        }

        /// <summary>
        /// This method attempts to create the OUTPUT QRACLE table.
        /// If will do nothing but print an error if the table already exists.
        /// </summary>
        //public void TryCreateTbale()
        //{

        //    using (SqlConnection con = new SqlConnection())
        //    {
        //        con.ConnectionString = ConfigurationManager.AppSettings["SqlConnectionString"];
        //        con.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(
        //                "CREATE TBALE OUTPUT1(LocationId INT, CellularTowerId INT, EventType TEXT, EventTimeSpan TEXT)", con))
        //            {
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //        catch
        //        {
        //            Console.WriteLine("Table already exists, or something wrong with the connection");
        //        }
        //    }
        //}

        public void Run()
        {
            vissim = new Vissim();
            ///Load Vissim net work
            vissim.LoadNet(VissimSimulatorFilePath, false);

            //initialize the cellular network
            cellularNetwork.LoadFromFile(CellLinkRelationFilePath, Delimiter);
            ///initialize the table
            //TryCreateTbale();
            //set up the collector threads. For now, only need one thread on this
            //for now, we only need 1 worker to collect the event

            using (StreamWriter writer = new StreamWriter(VissimEventsFilePath))
            {
                CollectorWorker worker = new CollectorWorker(writer);
                Task collectorTask = Task.Factory.StartNew(() =>
                {
                    foreach (CellularTowerEvent cEvent in cellularTowerEvents.GetConsumingEnumerable())
                    {
                        worker.Process(cEvent);
                    }
                });

                //simulation thread: including vissim simulation, events generation and detection
                Task simulator = Task.Factory.StartNew(() =>
                {
                    for (int currentTick = 0; currentTick < SimulationTicks; currentTick++)
                    {
                        foreach (IVehicle vehicle in vissim.Net.Vehicles)
                        {
                            //get the vehicle id+
                            int vehicleId= (int)vehicle.AttValue["No"];
                            //get the current vehicle link
                            ILane lane = vehicle.Lane;
                            string linkId = lane.AttValue["Link"];
                            //Console.WriteLine(string.Format("vehicle {0} at link {1}", vehicleId, linkId));
                            //first check if this vehicle has event
                            if (vehicleEvents.ContainsKey(vehicleId.ToString()))
                            {
                                foreach (CellularTowerEvent cEvent in DetectEvent(vehicleId.ToString(), linkId, currentTick))
                                {
                                    if (cEvent != null)
                                    {
                                        cellularTowerEvents.Add(cEvent);
                                    }
                                }

                            }
                            else //if no vehicle event, that means this is new vehicle entering the vissim network
                            {
                                GenerateEvent(vehicleId.ToString(), currentTick);
                            }
                        }
                        //make the Vissim simulation move forward one tick
                        vissim.Simulation.RunSingleStep();
                    }
                });
                try
                {
                    Task.WaitAll(simulator, collectorTask);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("there are some exceptions happened: {0}", ex.Message));
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Exit the vissim
        /// </summary>
        public void Exit()
        {
            vissim.Exit();
        }

        #endregion //end public methods

        #region private methods
        private IEnumerable<CellularTowerEvent> DetectEvent(string vehicleId, string linkId, long currentTick)
        {
            VehicleEvent vEvent = vehicleEvents[vehicleId];

            //find out the active event on this vehicle
            foreach (Event evt in vEvent.GetActiveEvent(currentTick))
            {
                //check if the event is happenning
                if (evt != null)
                {
                    ///current time.
                    Location curlocation = null;
                    CellTower curCell = null;
                    switch (evt.EventType)
                    {
                        case EventType.PowerOn: //this is a LU event
                            curlocation = cellularNetwork.FindLocationByLinkId(linkId);
                            curCell = cellularNetwork.FindCellTowerByLinkId(linkId);
                            if (curlocation != null && curCell != null)
                            {
                                //for now just use vechileId to represnet IMSI
                                yield return new CellularTowerEvent(vehicleId, curlocation.LocationId, curCell.CellTowerId, evt, currentTick);
                            }
                            break;
                        case EventType.OnCall: //this is a hand-off event
                            curlocation = cellularNetwork.FindLocationByLinkId(linkId);
                            curCell = cellularNetwork.FindCellTowerByLinkId(linkId);
                            if (curlocation != null && curCell != null)
                            {
                                //for now just use vechileId to represnet IMSI
                                yield return new CellularTowerEvent(vehicleId, curlocation.LocationId, curCell.CellTowerId, evt, currentTick);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            yield return null;
        }

        private void GenerateEvent(string vehicleId, int currentTick)
        {
            Random rnd = new Random();
            //get a random number
            int vehiclePossible = rnd.Next(0, 10);

            //let's say 80% of vehicles will have PowerOn event
            if (vehiclePossible <= 8)
            {
                //no vehicle event on this vehicle yet. Means this is a new vehicle in the vissim network
                VehicleEvent vEvent = new VehicleEvent(vehicleId);
                vEvent.AddPowerOnEvent();

                int nextPossibleOnCall = rnd.Next(0, 10);

                //let's say 20% of vehicles will have OnCall event
                if (nextPossibleOnCall < 2)
                {
                    vEvent.AddOnCallEvent(currentTick);
                }

                vehicleEvents.Add(vEvent.VehicleId, vEvent);
            }
        }
        #endregion private methods
    }
}
