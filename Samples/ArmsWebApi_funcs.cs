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

        static void _Main(string[] args)
        {
            //////////////////////////////////////////////////////////////
            //①設備名(plantcd)からマガジン情報を入手
            //  戻り値はVirtualMagからのmagnoおよびWorkStart(開始時間)
            //////////////////////////////////////////////////////////////

            //var plantcd = "TCSDR01";
            var plantcd = "TCSCT01";

            //var list = ArmsWebApi.Functions.GetMagazines(plantcd);

            //foreach (var item in list)
            //{
            //    Console.WriteLine("MagNo: " + item.MagazineNo + "  [" + item.WorkStart + "]");
            //}


            /////////////////////////////////
            ///ARMS不良関連
            /////////////////////////////////
            var def = GetDefItems("720220308140000006", "AU0309-1210E1W11-00E-6012", 4);
            Console.WriteLine(def);
            Console.ReadLine();

            Console.ReadKey();
        }


        //Arms基板不良項目リスト抽出
        public static DefItem[] GetDefItems(string lotno, string typecd, int pocno)
        {
            DefItem[] defs = Defect.GetAllDefectSubSt(lotno, typecd, pocno);
            return defs;
        }
    }
}
