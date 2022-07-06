using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class ArmsWebAp_startends
    {

        static void Main(string[] args)
        {
            ////////////////////////
            // ArmsWebApiサンプル //
            ////////////////////////
            //◇開始・完了API
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

            string msg;

            string plantcd = "TCBTC01";
            string empcd = "APP";
            string magcode = "TESTLOT001";
            //var magcode = magqr.Split(' ');


            //////////////////////////
            ////①開始完了一括
            //////////////////////////

            //ArmsWebApi.WorkStartEnd wse;
            //wse = new ArmsWebApi.WorkStartEnd(plantcd, empcd, magcode[1], magcode[1]);

            //string msg;

            //bool success = wse.StartEnd(out msg);

            //if (success)
            //{
            //    Console.WriteLine("登録処理完了。\r\n");
            //}
            //else
            //{
            //    Console.WriteLine("登録処理できませんでした。\r\n");
            //    Console.WriteLine(msg);
            //}


            ////////////////////////
            //②開始
            ////////////////////////

            //ArmsWebApi.WorkStart ws;
            //ws = new ArmsWebApi.WorkStart(plantcd, empcd, magcode);

            //bool success_cbs = ws.CheckBeforeStart(out msg);

            //if (success_cbs)
            //{
            //    Console.WriteLine("登録前確認完了。\r\n");
            //}
            //else
            //{
            //    Console.WriteLine("登録前確認で不正がありました。\r\n");
            //    Console.WriteLine(msg);

            //    Console.ReadKey();
            //    return;
            //}

            //bool success_in = ws.Start(out msg);

            //if (success_in)
            //{
            //    Console.WriteLine("開始登録処理完了。\r\n");
            //}
            //else
            //{
            //    Console.WriteLine("開始登録処理できませんでした。\r\n");
            //    Console.WriteLine(msg);

            //    Console.ReadKey();
            //    return;
            //}


            ////////////////////////
            //③完了
            ////////////////////////
            string outmagcode = "J10013";
            //var outmagcode = outmagqr.Split(' ');
            //int newMagFrameQty = 25;
            Dictionary<string, int> Defectdict = new Dictionary<string, int> { { "00019", 1 } };

            ArmsWebApi.WorkEnd we;
            //we = new ArmsWebApi.WorkEnd(plantcd, empcd, magcode, outmagcode, "");
            we = new ArmsWebApi.WorkEnd(plantcd, empcd, "", outmagcode, magcode);

            if (we.RegisterDefects(out msg, Defectdict))
            {
                Console.WriteLine("不良登録完了。\r\n");
            }
            else
            {
                Console.WriteLine("不良登録できませんでした。\r\n");
                Console.WriteLine(msg);
            }

            if (we.End(out msg))
            {
                Console.WriteLine("完了登録処理完了。\r\n");
            }
            else
            {
                Console.WriteLine("完了登録処理できませんでした。\r\n");
                Console.WriteLine(msg);
            }

            Console.ReadKey();
        }
    }
}
