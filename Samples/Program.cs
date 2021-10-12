using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using ArmsApi.Model;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            /////////////////////
            // ArmsApiサンプル //
            /////////////////////

            // マガジンQRコードの読取事例を設定
            string magqr = "A 10000";
            var magcode = magqr.Split(' ');


            /////////////////////////////////////////////////////////////////
            // マガジンコードからマガジン情報を読み込む関数
            ArmsApi.Model.Magazine mag = Magazine.GetCurrent(magcode[1]);
            
            if (mag != null)
            {
                // Magazineクラスの◆ロットNo(NascaLotNO)を呼び出す
                Console.WriteLine(mag.NascaLotNO);

                /////////////////////////////////////////////////////////////////
                // ロットNoからロット情報を読み込む関数
                ArmsApi.Model.AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

                if (lot != null)
                {
                    // AsmLotクラスの◆機種名を呼び出す
                    Console.WriteLine(lot.TypeCd);
                }
                else
                {
                    Console.WriteLine("対象のロット情報はありません");
                }
            }
            else
            {
                Console.WriteLine("対象のマガジン情報はありません");
            }

            Console.ReadKey();

        }
    }
}
