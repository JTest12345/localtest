using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacMnt
{
    public class MntData
    {
        public int MacNo { get; set; }
        public string Plantcd { get; set; }
        public DateTime MntDt { get; set; }
        public bool DelFg { get; set; }
    }

    public class MacFailData
    {
        public int macno { get; set; }

        public string cause { get; set; }

        public string worker { get; set; }

        public DateTime faildate { get; set; }

        public DateTime inputdate { get; set; }
    }
}
