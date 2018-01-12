using System.Collections.Generic;

namespace VissimSimulator
{
    public class CellTower
    {
        public string LocationId { get; set; }
        public string CellTowerId { get; set; }

        public Dictionary<string, Link> Links { get; private set; }

        public CellTower()
        {
            Links = new Dictionary<string, Link>();
        }

        public CellTower(string cellId, string locationId) : this()
        {
            LocationId = locationId;
            CellTowerId = cellId;
        }

        public void AddLink(string linkId)
        {
            Link link = new Link(linkId, this.CellTowerId);
            Links.Add(linkId, link);
        }

        public override bool Equals(object obj)
        {
            CellTower cell = obj as CellTower;
            return this.CellTowerId.Equals(cell.CellTowerId);
        }
    }
}
