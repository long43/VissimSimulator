using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissimSimulator
{
    public class Link
    {
        public string LinkId { get; set; }

        public string CellId { get; set; }

        public Link(string linkId, string cellId)
        {
            LinkId = linkId;
            CellId = cellId;
        }
    }
}
