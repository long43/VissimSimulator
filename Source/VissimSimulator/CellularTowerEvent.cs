using System;

namespace VissimSimulator
{
    public class CellularTowerEvent
    {
        #region public properties
        public string LocationId { private set; get; }
        public string CellularTowerId { private set; get; }
        public Event Event { private set; get; }
        public long CurrentTick { private set; get; }
        #endregion //public properties 

        #region public methods
        /// <summary>
        /// Is this event a LU event?
        /// </summary>
        /// <returns>True if it's LU event, otherwise return false</returns>
        public bool IsLocationUpdate()
        {
            return string.IsNullOrEmpty(CellularTowerId);
        }

        /// <summary>
        /// Is this event a HO event?
        /// </summary>
        /// <returns>True if it's HO event, otherwise return false</returns>
        public bool IsHandOff()
        {
            return !IsLocationUpdate();
        }
     
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">id, either cell id or location id</param>
        /// <param name="evt">Event</param>
        /// <param name="currentTicks">current time tick</param>
        public CellularTowerEvent(string id, Event evt, long tick)
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
            CurrentTick = tick;
        }


        #endregion //public methods
    }
}
