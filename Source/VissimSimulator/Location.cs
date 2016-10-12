using System.Collections.Generic;

namespace VissimSimulator
{
    public class Location
    {
        #region private fields
        //Dictionary to hold all cell towers
        private Dictionary<string, CellTower> cellTowers;
        #endregion //private fields

        #region public properties
        ///<summary>Id of the location</summary>
        public string LocationId { get; set; }

        ///<summary>Collection of cells</summary>
        public ICollection<CellTower> Cells
        {
            get
            {
                return cellTowers.Values;
            }
        }
        #endregion //public properties

        #region public methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationId">LocationId</param>
        public Location(string locationId)
        {
            LocationId = locationId;
            cellTowers = new Dictionary<string, CellTower>();
        }

        /// <summary>
        /// Get cell from the location with a given cell id
        /// </summary>
        /// <param name="cellId">cell id</param>
        /// <returns>CellTower</returns>
        public CellTower GetCell(string cellId)
        {
            return cellTowers[cellId];
        }

        /// <summary>
        /// Does the location contains a given cell?
        /// </summary>
        /// <param name="cellId">Cell Id</param>
        /// <returns>True if it contains, otherwise false</returns>
        public bool ContainsCell(string cellId)
        {
            return cellTowers.ContainsKey(cellId);
        }

        /// <summary>
        /// Add a cell tower to the location
        /// </summary>
        /// <param name="tower">CellTower</param>
        public void AddCellTower(CellTower tower)
        {
            cellTowers.Add(tower.CellTowerId, tower);
        }
        #endregion //public methods
    }
}
