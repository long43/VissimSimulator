using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

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


        /// <summary>
        /// Read the csv files to initialize the cellular network
        /// The format of the CellLinkRelation file is as follows:
        /// LINK_ID,CELLID,LAC
        /// </summary>
        private void LoadFromFile(string networkFilePath)
        {
            //read the cell-location relation file
            using (StreamReader cellLinkReader = new StreamReader(File.OpenRead(networkFilePath)))
            {
                //skip the header line
                string line = cellLinkReader.ReadLine();

                //read the rest of the file
                while ((line = cellLinkReader.ReadLine()) != null)
                {
                    string[] values = line.Split(delimiter);

                    string locationId = values[2];
                    Location location;

                    if (!this.ContainsLocation(locationId))
                    {
                        location = new Location(locationId);
                        //if the Location is new location, then the cell must be new cell
                        CellTower cell = new CellTower();
                        cell.CellTowerId = values[1];
                        //if the cell is a new cell, then the link must be a new link
                        cell.Links.Add(values[3]);

                        location.AddCellTower(cell);
                    }
                    else
                    { 
                        //if this location pre exists
                        location = this.GetLocation(locationId);
                        //check if the cell also exists

                    }
                    
                }
            }
        }

        public Location GetLocation(string locationId)
        {
            return network[locationId];
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
