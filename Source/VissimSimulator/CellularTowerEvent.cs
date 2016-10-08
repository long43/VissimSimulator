using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissimSimulator
{
    public class CellularTowerEvent
    {
        public string LocationId { set; get; }
        public string CellularTowerId { set; get; }
        public Event Event;

        public bool IsLocationUpdate()
        {
            return string.IsNullOrEmpty(CellularTowerId);
        }

        public bool IsHandOff()
        {
            return !IsLocationUpdate();
        }

        public CellularTowerEvent(string id, Event evt)
        {
            if (evt.EventType == EventType.OnCall)
            {
                CellularTowerId = id;
            }
            else
            {
                LocationId = id;
            }
            Event = evt;
        }
    }
}
