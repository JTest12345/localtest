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

        public Magazine mag;

        public List<ArmsApi.Model.Magazine> MagList { get; set; }

        public AsmLot lot;

        public WorkStartAltModel wsm;

        public WorkStart(string plantcd, string magno)
        {
            this.plantcd = plantcd;
            this.magno = magno;
            this.mag = Magazine.GetCurrent(magno);
            this.lotno = mag.NascaLotNO;
            this.lot = AsmLot.GetAsmLot(lotno);
            this.wsm = new WorkStartAltModel(plantcd);
            this.MagList = new List<Magazine>();
            MagList.Add(mag);
        }

        public bool CheckBeforeStart(out string msg)
        {
            var jcm = Magazine.GetCurrent(magno);
            
            return wsm.CheckBeforeStart(jcm, out msg);
        }

        public bool Start(out string msg)
        {
            return wsm.WorkStart(out msg);
        }
    }
}
