using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class DivideCarrierModel
    {
		public DivideCarrierModel(string empcd)
        {
            this.EmpCD = empcd;
            this.ConfirmedMagazineFg = false;
        }

		public string LotNo { get; set; }
		public string MagazineNo { get; set; }
		public string OrgCarrierNo { get; set; }
		public string NewCarrierNo { get; set; }
		public Magazine OperationMagazine { get; set; }
		public Magazine CurrentMagazine { get; set; }
		public bool ConfirmedMagazineFg { get; set; }
		public string EmpCD { get; set; }

        public bool DivideCarrier(out List<string> errMsg)
        {
            try
            {
                errMsg = new List<string>();

                LotCarrier.DivideCarrier(this.LotNo, this.OrgCarrierNo, this.NewCarrierNo, this.EmpCD);

                return true;
            }
            catch (ApplicationException ex)
            {
                throw ex;
            }
        }
    }
}