using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissimSimulator
{
    public class Location
    {
        private Dictionary<string, CellTower> cellTowers;

        public string LocationId { get; set; }

        public ICollection<CellTower> Cells
        {
            get
            {
                return cellTowers.Values;
            }
        }

        public Location(string locationId)
        {
            LocationId = locationId;
            cellTowers = new Dictionary<string, CellTower>();
        }

        public CellTower GetCell(string cellId)
        {
            return cellTowers[cellId];
        }

        public bool ContainsCell(string cellId)
        {
            return cellTowers.ContainsKey(cellId);
        }

        public void AddCellTower(CellTower tower)
        {
            cellTowers.Add(tower.CellTowerId, tower);
        }
    }
}
