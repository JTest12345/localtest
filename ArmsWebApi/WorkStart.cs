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

        public Magazine jcm;

        public AsmLot lot;

        public WorkStart(string plantcd, string magno)
        {
            this.plantcd = plantcd;
            this.magno = magno;
            this.jcm = Magazine.GetCurrent(magno);
            this.lotno = jcm.NascaLotNO;
            this.lot = AsmLot.GetAsmLot(lotno);
        }

        public bool CheckBeforeStart(out string msg)
        {
            var wsm = new WorkStartAltModel(plantcd);
            var jcm = Magazine.GetCurrent(magno);
            
            return wsm.CheckBeforeStart(jcm, out msg);
        }
    }
}
