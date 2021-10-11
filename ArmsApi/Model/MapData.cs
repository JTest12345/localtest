using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    class MapData
    {

        public enum ConversionPattern
        {
            Normal = 0,
            Reverse = 1,
            Left90 = 2,
        }


        public string LotNo { get; set; }
        public string CassetteNo { get; set; }
        public int ProcNo { get; set; }
        public ConversionPattern Pattern { get; set; }

        public static MapData Create(Cassette cas, string mapdata, int procno, ConversionPattern pattern)
        {
            throw new NotImplementedException();
        }


        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
