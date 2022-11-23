using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIf
{
    ////////////////////////////////////
    ///
    /// VipFetchMac クラス
    ///
    ////////////////////////////////////


    public class VipFetchMac
    {
        public string MacCat { get; set; }
        public List<MacInfo> Macinfo { get; set; }
    }

    public class MacInfo
    {
        public string plantcd { get; set; }
        public bool ftpenable { get; set; }
    }

}
