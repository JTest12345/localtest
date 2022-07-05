using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class ArmsApiPlayGround
    {
        static void _Main(string[] args)
        {
            var lotno = "FJ0000001";
            //var order = ArmsApi.Model.Order.GetOrder(lotno);
            //if (order.Length != 0)
            //{
            //    var currproc = ArmsApi.Model.Process.GetNowProcess(lotno);
            //    var curorder = ArmsApi.Model.Order.GetOrder(lotno, currproc.ProcNo);

            //    if (curorder.Length == 0)
            //    {
            //        Console.WriteLine("TnTranが不正な状態です" + "\r\n");
            //        return;
            //    }

            //    if (curorder[0].WorkEndDt != null)
            //    {
            //        Console.WriteLine("完了済み" + "\r\n");
            //    }
            //    else
            //    {
            //        Console.WriteLine("開始済み" + "\r\n");
            //    }
            //}
            //else
            //{
            //    var mag = ArmsApi.Model.Magazine.GetCurrent(lotno);
            //    if (mag != null)
            //    {
            //        Console.WriteLine("初工程開始前" + "\r\n");
            //    }
            //    else
            //    {
            //        Console.WriteLine("TnTranが不正な状態です" + "\r\n");
            //        return;
            //    }
            //}

            var workcd = "BTC";
            var prclist = ArmsApi.Model.Process.GetProcessList(workcd);
            var lot = ArmsApi.Model.AsmLot.GetAsmLot(lotno);
            var nextmagproc = ArmsApi.Model.Process.GetNextProcess(prclist[0].ProcNo, lot);
            var nextorder = ArmsApi.Model.Order.GetNextMagazineOrder(lotno, prclist[0].ProcNo);


            Console.ReadLine();
            Console.ReadKey();
        }

    }
}
