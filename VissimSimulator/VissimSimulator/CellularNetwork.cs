using System;
using System.Collections.Generic;

namespace VissimSimulator
{
    public class CellTower
    {
        public int CellTowerId { get; set; }

        public HashSet<int> Links { get; private set; }

        public CellTower()
        {
            Links = new HashSet<int>();
        }

        public void AddLink(int linkId)
        {
            Links.Add(linkId);
        }
    }

    public class Location
    {
        public int LocationId { get; set; }

        public Dictionary<int, CellTower> CellTowers { get; private set; }

        public Location()
        {
            CellTowers = new Dictionary<int, CellTower>();
        }

        public void AddCellTower(CellTower tower)
        {
            CellTowers.Add(tower.CellTowerId, tower);
        }
    }
}
