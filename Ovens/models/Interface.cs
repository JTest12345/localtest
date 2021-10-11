using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovens.models
{
    //
    // OVEN Interfaces
    //

    interface IOVENPLC
    {
        string IPAddress { get; set; }
        int Port { get; set; }
        bool Ping();
    }

    interface INOVEN
    {
        string SP_ADDRESS { get; set; }
        string PV_ADDRESS { get; set; }
        string NG1_ADDRESS { get; set; }
        string NG2_ADDRESS { get; set; }
        string MACNO_ADDRESS { get; set; }
        string STDEV_PLUS_ADDRESS { get; set; }
        string STDEV_MINUS_ADDRESS { get; set; }
    }
}
