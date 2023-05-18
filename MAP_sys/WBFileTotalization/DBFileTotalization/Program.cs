using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFileTotalization
{
    class Program
    {
        static void Main(string[] args)
        {
            Totalization TL = new Totalization();
            TL.ReadData();
        }
    }
}
