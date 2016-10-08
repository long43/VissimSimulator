using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissimSimulator
{
    public class CellTower
    {
        public string CellTowerId { get; set; }

        public HashSet<string> Links { get; private set; }

        public CellTower()
        {
            Links = new HashSet<string>();
        }

        public CellTower(string cellId) : this()
        {
            CellTowerId = cellId;
        }

        public void AddLink(string linkId)
        {
            Links.Add(linkId);
        }
    }
}
