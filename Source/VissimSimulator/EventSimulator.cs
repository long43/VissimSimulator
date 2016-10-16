using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using VISSIMLIB;
using Oracle.DataAccess.Client;

namespace VissimSimulator
{
    public class EventSimulator
    {
        #region private fields
        private const string CellLinkRelationFilePath = @".\Input\Taicang_Major_Cell_Link_Related.csv";
        private const string VissimSimulatorFilePath = @"C:\Users\Public\Documents\PTV Vision\PTV Vissim 6\Taicang.inpx";
        private const char Delimiter = ',';
        private const long SimulationTicks = 3600;

        //the cellular network
        private CellularNetwork cellularNetwork;

        //Dictionary that holds all vehicle events in the network
        private Dictionary<string, VehicleEvent> vehicleEvents;

        //BlockingCollection that holds all CellularTower Events
        private BlockingCollection<CellularTowerEvent> cellularTowerEvents;

        //Vissim simulator
        private Vissim vissim;
        #endregion //end private fields*

        #region public methods
        public EventSimulator()
        {
            CellularNetwork cellularNetwork = new CellularNetwork();
            Dictionary<string, VehicleEvent> VehicleEvents = new Dictionary<string, VehicleEvent>();
            BlockingCollection<CellularTowerEvent> CellularTowerEvents = new BlockingCollection<CellularTowerEvent>();
        }
       
        /// <summary>
        /// This method attempts to create the OUTPUT QRACLE table.
        /// If will do nothing but print an error if the table already exists.
        /// </summary>
        public void TryCreateTbale()
        {

            using (OracleConnection con = new OracleConnection())
            {
                con.ConnectionString = "host = serverName;databse = myDatabse; uid = userName; pwd = password";
                con.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand(
                        "CREATE TBALE OUTPUT1(LocationId INT, CellularTowerId INT, EventType TEXT, EventTimeSpan TEXT)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Table not created.");
                }
            }
        }




        public void Run()
        {
            vissim = new Vissim();
            ///Load Vissim net work
            vissim.LoadNet();

            //initialize the cellular network
            cellularNetwork.LoadFromFile(CellLinkRelationFilePath, Delimiter);
            ///initialize the table
            TryCreateTbale();
            //set up the collector threads. For now, only need one thread on this


            //for now, we only need 1 worker to collect the event
            CollectorWorker worker = new CollectorWorker();
            Task collectorTask = Task.Factory.StartNew( () =>
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
                        //get the vehicle id
                        string vehicleId = vehicle.AttValue["No"];

                        //first check if this vehicle has event
                        if (vehicleEvents.ContainsKey(vehicleId))
                        {
                            CellularTowerEvent cEvent = DetectEvent(vehicleId, currentTick);
                            cellularTowerEvents.Add(cEvent);
                        }
                        else //if no vehicle event, that means this is new vehicle entering the vissim network
                        {
                            GenerateEvent(currentTick);
                        }
                    }
                    //you need to make the Vissim simulation move forward one tick. Find the corresponding Vissim doc on how the COM-API calls look like.
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

        /// <summary>
        /// Exit the vissim
        /// </summary>
        public void Exit()
        {
            vissim.Exit();
        }

        #endregion //end public methods

        #region private methods
        private CellularTowerEvent DetectEvent(string vehicleId, long currentTick)
        {
            VehicleEvent vEvent = vehicleEvents[vehicleId];

            //find out the active event on this vehicle
            Event evt = vEvent.GetActiveEvent(currentTick);
            //check if the event is happenning
            if (evt != null)
            {
                //if the event is happening, see if any cellTower/Location capture it
                string linkId = vEvent.VehicleLink;
                CellularTowerEvent cEvent = null;
                ///current time.
                switch (evt.EventType)
                {
                    case EventType.PowerOn: //this is a LU event
                        Location location = cellularNetwork.FindLocationByLinkId(linkId);
                        cEvent = new CellularTowerEvent(location.LocationId, evt, currentTick);
                        break;
                    case EventType.OnCall: //this is a hand-off event
                        CellTower cell = cellularNetwork.FindCellTowerByLinkId(linkId);
                        cEvent = new CellularTowerEvent(cell.CellTowerId, evt, currentTick);
                        break;
                    default:
                        break;
                }
                return cEvent;
            }
            return null;
        }

        private void GenerateEvent(int currentTick)
        {
            Random rnd = new Random();
            //get a random number
            int vehiclePossible = rnd.Next(0, 10);

            //let's say 80% of vehicles will have PowerOn event
            if (vehiclePossible <= 8)
            {
                //no vehicle event on this vehicle yet. Means this is a new vehicle in the vissim network
                VehicleEvent vEvent = new VehicleEvent();
                vEvent.AddPowerOnEvent();

                int nextPossibleOnCall = rnd.Next(0, 10);

                //let's say 10% of vehicles will have OnCall event
                if (nextPossibleOnCall < 1)
                {
                    vEvent.AddOnCallEvent(currentTick);
                }

                vehicleEvents.Add(vEvent.Vehicleid, vEvent);
            }
        }
        #endregion private methods
    }
}
