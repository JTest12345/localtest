using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsWeb.Models;

namespace ArmsWebApi
{
    public class WorkStart
    {
        public string plantcd;

        public string magno;

        public string lotno;

        public string empcd;

        public Magazine mag;

        public AsmLot lot;

        public WorkStartAltModel wsm;

        public WorkStart(string plantcd, string empcd, string magno)
        {
            this.plantcd = plantcd;
            this.empcd = empcd;
            this.magno = magno;
        }

        public bool CheckBeforeStart(out string msg)
        {
            this.mag = Magazine.GetCurrent(magno);
            
            if (mag == null)
            {
                msg = "対象の実マガジンは存在しません";
                return false;
            }

            this.lotno = mag.NascaLotNO;
            this.lot = AsmLot.GetAsmLot(lotno);

            if (lot == null)
            {
                msg = "対象のロットは存在しません";
                return false;
            }

            if (plantcd == "")
            {
                msg = "設備コード(plantcd)が必要です";
                return false;
            }

            wsm = new WorkStartAltModel(plantcd);

            return wsm.CheckBeforeStart(mag, out msg);
        }

        public bool Start(out string msg)
        {
            if (wsm == null)
            {
                msg = "CheckBeforeStartを実行・完了してください";
                return false;
            }

            try
            {
                wsm.MagList.Add(mag);
                return wsm.WorkStart(out msg);
            }
            catch (Exception e)
            {
                msg = e.ToString();
                return false;
            }
        }
    }
}
