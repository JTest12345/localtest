using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrendGraph.Models
{
    public class MultiList
    {
        public IEnumerable<GraphData> GraphData { get; set; }
        public IEnumerable<MachineList> MachineList { get; set; }
        public IEnumerable<CurrentMacno> CurrentMacno { get; set; }
    }
}