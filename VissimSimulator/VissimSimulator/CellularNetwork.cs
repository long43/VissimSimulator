using System;
using System.Linq;
using System.Collections.Generic;

namespace VissimSimulator
{
    public class CellularNetwork
    {
        private Dictionary<string, Location> network;

        public ICollection<Location> Locations
        {
            get 
            {
                return network.Values;
            }
        }

        public CellularNetwork()
        {
            network = new Dictionary<string, Location>();
        }


        public void AddLocation(Location location)
        {
            network.Add(location.LocationId, location);
        }

        public bool ContainsLocation(string locationId)
        {
            return network.ContainsKey(locationId);
        }

        public IEnumerable<string> FindLinksByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   from link in cell.Links
                   select link;
        }

        public IEnumerable<CellTower> FindCellTowersByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   select cell;
        }

        public IEnumerable<string> FindLinksByCellId(string cellId)
        {
            return from location in network.Values
                   from cell in location.Cells
                   from link in cell.Links
                   where cell.CellTowerId == cellId
                   select link;
        }
             
        public Location FindLocationByLinkId(string linkId)
        {
            return (from location in network.Values
                    from cell in location.Cells
                   where cell.Links.Contains(linkId)
                   select location).FirstOrDefault();
        }

        public CellTower FindCellIdByLinkId(string linkId)
        { 
            return (from location in network.Values
                    from cell in location.Cells
                    where cell.Links.Contains(linkId)
                    select cell).FirstOrDefault();
        }

        public Location FindLocationByCellId(string cellId)
        {
            return (from location in network.Values
                    where location.ContainsCell(cellId)
                    select location).FirstOrDefault();
        }
    }

    public class CellTower
    {
        public string CellTowerId { get; set; }

        public HashSet<string> Links { get; private set; }

        public CellTower()
        {
            Links = new HashSet<string>();
        }

        public void AddLink(string linkId)
        {
            Links.Add(linkId);
        }
    }

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
