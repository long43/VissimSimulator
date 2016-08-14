using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace VissimSimulator
{
    public class EventSimulator
    {
        public Dictionary<int, Location> CellularNetwork = new Dictionary<int,Location>();

        public Dictionary<int, VehicleEvent> VehicleEvents = new Dictionary<int,VehicleEvent>();

        public ConcurrentQueue<CellularTowerEvent> CellularTowerEvents = new ConcurrentQueue<CellularTowerEvent>();

        public List<CollectorWorker> CollectorWorkers;

        public void Run()
        {
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

    public class VehicleEvent
    {

    }

    public class CellularTowerEvent
    {

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
