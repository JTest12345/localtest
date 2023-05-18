using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrendGraph.Models
{
    public class WorkRegulation
    {
        public string magno { get; set; }
        public int procfrom { get; set; }
        public int procto { get; set; }
        public int fromwaittime { get; set; }
        public int fromtoendtime { get; set; }
    }
}