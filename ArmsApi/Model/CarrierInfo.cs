using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class CarrierInfo
    {
        public int CarNo { get; set; }

        public CarrierInfo()
        {
        
        }
        public CarrierInfo(int carNo)
        {
            this.CarNo = carNo;
        }

        public static CarrierInfo[] GetCarriers()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetNetworkSettings()
        {
            throw new NotImplementedException();
        }
    }
}
