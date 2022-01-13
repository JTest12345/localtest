using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class ISODatetime
    {
        static void _Main(string[] args)
        {
            var strData = "2021-12-03T14:05:21.000Z";
            var dtData = DateTime.Parse(strData, null, System.Globalization.DateTimeStyles.AssumeUniversal);

            Console.WriteLine(dtData);

            Console.ReadKey();
        }
    }
}
