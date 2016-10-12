
namespace VissimSimulator
{
    public class CellularTowerEvent
    {
        #region public properties
        public string LocationId { set; get; }
        public string CellularTowerId { set; get; }
        public Event Event { get; private set; }
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

        #endregion //public methods
    }
}
