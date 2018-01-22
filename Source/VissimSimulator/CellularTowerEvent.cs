using System;

namespace VissimSimulator
{
    public class CellularTowerEvent
    {
        #region public properties
        public string IMSI { private set; get; }
        public string CurLocationId { private set; get; }
        public string CurCellularTowerId { private set; get; }
        public string PreLocationId { private set; get; }
        public string PreCellularTowerId { private set; get; }
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
            return string.IsNullOrEmpty(CurCellularTowerId);
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="evt"></param>
        /// <param name="tick"></param>
        public CellularTowerEvent(string imsi, 
                                  string curLocationId, 
                                  string curCellularTowerId,
                                  string preLocationId,
                                  string preCellularTowerId,
                                  Event evt, 
                                  long tick)
        {
            IMSI = imsi;
            CurCellularTowerId = curCellularTowerId;
            CurLocationId = curLocationId;
            PreCellularTowerId = preCellularTowerId;
            PreLocationId = preLocationId;
            Event = evt;
            CurrentTick = tick;
        }

        #endregion //public methods
    }
}
