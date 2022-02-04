using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;

namespace Samples
{
    class ArmsWebAp_functions
    {

        static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////
            //①設備名(plantcd)からマガジン情報を入手
            //  戻り値はVirtualMagからのmagnoおよびWorkStart(開始時間)
            //////////////////////////////////////////////////////////////

            //var plantcd = "TCSDR01";
            var plantcd = "TCSCT01";

            var list = ArmsWebApi.Functions.GetMagazines(plantcd);

            foreach (var item in list)
            {
                Console.WriteLine("MagNo: " + item.MagazineNo + "  [" + item.WorkStart + "]");
            }

            Console.ReadKey();
        }
    }
}
