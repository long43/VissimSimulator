using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VissimSimulator.Mock;
//using VISSIMLIB;

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
        
        //task cancellation source
        private CancellationTokenSource tokenSource;
        //task cancellation token
        private CancellationToken token;

        private int currentTick;

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
            currentTick = 0;
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            cellularNetwork = new CellularNetwork();
            vehicleEvents = new Dictionary<string, VehicleEvent>();
            cellularTowerEvents = new BlockingCollection<CellularTowerEvent>();
        }

        public void Run()
        {
            vissim = new Vissim();
            ///Load Vissim net work
            vissim.LoadNet(VissimSimulatorFilePath, false);

            //initialize the cellular network
            cellularNetwork.LoadFromFile(CellLinkRelationFilePath, Delimiter);

            CollectorWorker worker = new CollectorWorker(VissimEventsFilePath, cellularTowerEvents);
            //collector task: collecting the data from cellular events
            Task collectorTask = Task.Factory.StartNew(() => worker.Run(), token);

            //simulation thread: including vissim simulation, events generation and detection
            Task simulator = Task.Factory.StartNew(() => Execute(), token);

            Task randomEventsGenerator = Task.Factory.StartNew(() => GenerateCellularStaticEvents(), token);

            try
            {
                Task.WaitAll(simulator, collectorTask, randomEventsGenerator);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("there are some exceptions happened: {0}", ex.Message));
                throw ex;
            }
        }

        public void Execute()
        {
            while (currentTick < SimulationTicks)
            {
                foreach (IVehicle vehicle in vissim.Net.Vehicles)
                {
                    //get the vehicle id+
                    int vehicleId = (int)vehicle.AttValue["No"];
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

                Interlocked.Increment(ref currentTick);
            }

            //set the cancellation token to stop all tasks
            tokenSource.Cancel();

            //let the blockingqueue to know that we stopped adding new events so it will gracefully exit
            cellularTowerEvents.CompleteAdding();
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

        private void GenerateCellularStaticEvents()
        {
            while (!token.IsCancellationRequested)
            {
                foreach (Location lo in cellularNetwork.Locations)
                {
                    foreach (CellTower cl in lo.Cells)
                    {
                        //20% of the cells will have random non-vehicular events
                        //always assume each cell has 5000 cell phone users
                        for (int i = 0; i < 5000; i++)
                        {
                            //generate the events on cl level
                            Random rnd = new Random();
                            //get a random number
                            int cellRandN = rnd.Next(0, 10);
                            if (cellRandN <= 2)
                            {
                                //let's say 50% of them are power on events
                                Event evt = null;
                                if (rnd.Next(0, 10) <= 5)
                                {
                                    evt = new Event(EventType.PowerOn);
                                }
                                else
                                {
                                    evt = new Event(EventType.OnCall);
                                }
                                CellularTowerEvent cte = new CellularTowerEvent("-" + i.ToString(), lo.LocationId, cl.CellTowerId, evt, currentTick);
                                cellularTowerEvents.Add(cte);
                            }
                        }
                    }
                }
            }
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
