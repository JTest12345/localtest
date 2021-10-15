using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class ArmsWebApSample
    {

        static void Main(string[] args)
        {
            ////////////////////////
            // ArmsWebApiサンプル //
            ////////////////////////
            //◇開始完了サンプル
            //  動作前提
            //  DB: Vautom4
            //  lotno: TESTLOT00000010000
            //  typecd: CLUHANKAN-SIM
            //  実マガジン: A 10000
            //  工程: DBC (procno: 5)
            // 【DB登録状態】
            //   TnMag: 対象マガジンのprocnoが4でnewflg=true
            //   TnTran: procno4まで登録済
            //  設備: TCDBC01
            //  登録者: APP
            //

            string magqr = "A 10000";
            var magcode = magqr.Split(' ');

            string plantcd = "TCDBC01";

            string empcd = "APP";

            ArmsWebApi.WorkStartEnd wse;
            wse = new ArmsWebApi.WorkStartEnd(plantcd, empcd, magcode[1], magcode[1]);

            string msg;

            bool success = wse.StartEnd(out msg);

            if (success)
            {
                Console.WriteLine("登録処理完了。\r\n");
            }
            else
            {
                Console.WriteLine("登録処理できませんでした。\r\n");
                Console.WriteLine(msg);
            }

            Console.ReadKey();
        }
    }
}
