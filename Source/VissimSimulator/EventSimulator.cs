using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using VS = VissimSimulator;
//using VISSIMLIB;

namespace VissimSimulator
{
    public class EventSimulator
    {
        #region private fields
        private const string CellLinkRelationFilePath = @".\Input\Taicang_Major_Cell_Link_Related.csv";
        private const char Delimiter = ',';
        private const long SimulationTicks = 3600;

        private CellularNetwork cellularNetwork = new CellularNetwork();
        private Dictionary<string, VehicleEvent> VehicleEvents = new Dictionary<string, VehicleEvent>();
        private BlockingCollection<CellularTowerEvent> CellularTowerEvents = new BlockingCollection<CellularTowerEvent>();
        private List<CollectorWorker> CollectorWorkers;
        #endregion //end private fields

        #region public methods
        public void Run()
        {
            Vissim vissim = new Vissim();
            ///Load Vissim net work
            vissim.LoadNet(@"C:\Users\Public\Documents\PTV Vision\PTV Vissim 6\Examples Demo\Urban Intersection Beijing.CN\Intersection Beijing.inpx");

            //initialize the cellular network
            cellularNetwork.LoadFromFile(CellLinkRelationFilePath, Delimiter);
            //set up the collector threads. For now, only need one thread on this

            Task collector = Task.Factory.StartNew(() =>
            {
                CollectorWorker worker = new CollectorWorker();
                foreach (CellularTowerEvent cEvent in CellularTowerEvents.GetConsumingEnumerable())
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
                        string vehicleId = vehicle.Id;

                        //first check if this vehicle has event
                        if (VehicleEvents.ContainsKey(vehicleId))
                        {
                            CellularTowerEvent cEvent = DetectEvent(vehicleId, currentTick);
                            CellularTowerEvents.Add(cEvent);
                        }
                        else //if no vehicle event, that means this is new vehicle entering the vissim network
                        {
                            GenerateEvent(currentTick);
                        }
                    }
                }
            });
        }


        public void Exit()
        {
            //vissim.Exit();
        }

        #endregion //end public methods

        #region private methods
        private CellularTowerEvent DetectEvent(string vehicleId, long currentTick)
        {
            VehicleEvent vEvent = VehicleEvents[vehicleId];

            //find out the active event on this vehicle
            Event evt = vEvent.GetActiveEvent(currentTick);
            //check if the event is happenning
            if (evt != null)
            {
                //if the event is happening, see if any cellTower/Location capture it
                string linkId = vEvent.VehicleLink;
                CellularTowerEvent cEvent = null;
                switch (evt.EventType)
                {
                    case EventType.PowerOn: //this is a LU event
                        Location location = cellularNetwork.FindLocationByLinkId(linkId);
                        cEvent = new CellularTowerEvent(location.LocationId, evt);
                        break;
                    case EventType.OnCall: //this is a hand-off event
                        CellTower cell = cellularNetwork.FindCellTowerByLinkId(linkId);
                        cEvent = new CellularTowerEvent(cell.CellTowerId, evt);
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

                VehicleEvents.Add(vEvent.Vehicleid, vEvent);
            }
        }
        #endregion private methods
    }
}
