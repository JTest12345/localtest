using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
	public class ChangeCarrierModel
	{
		private const int OutputMpdProcNo = 999;

		public ChangeCarrierModel(string empcd)
        {
            DataMatirxList = new List<string>();

            this.EmpCd = empcd;
        }

		public string LotNo { get; set; }
		public List<string> DataMatirxList { get; set; }
		public string OrgCarrierNo { get; set; }
		public string NewCarrierNo { get; set; }
		public bool ProcNo { get; set; }
		public string EmpCd { get; set; }
		public DateTime LastUpdDt { get; set; }
		public bool OnlyOutputMpdFg { get; set; }


		public void ChangeCarrier()
		{
			CheckRegistrableData(this.LotNo, this.NewCarrierNo);

			ArmsApi.Model.LotCarrier orgLotCarrier = LotCarrier.GetData(this.OrgCarrierNo, true, true);

			if(orgLotCarrier.LotNo != this.LotNo)
			{
				throw new ApplicationException(string.Format(
					"変更前キャリアが稼働中の状態で存在しません。ロット番号:{0} キャリア番号:{1}", this.LotNo, this.OrgCarrierNo));
			}

			ArmsApi.Model.LotCarrier newLotCarrier = new LotCarrier(this.LotNo, this.NewCarrierNo);

			ArmsApi.Model.AsmLot lot = AsmLot.GetAsmLot(this.LotNo);

			Order[] orderList = ArmsApi.Model.Order.SearchOrder(this.LotNo, null, null, false, false);

			Order order = orderList.OrderBy(o => o.WorkStartDt).Last();

			//ArmsApi.Model.Process proc = Process.GetNowProcess(lot);

			//newLotCarrier.DeleteInsert(this.OrgCarrierNo, this.NewCarrierNo, proc.ProcNo);

			LENS2_Api.ARMS.LotCarrier.ChangeCarrierNo(this.LotNo, this.OrgCarrierNo, this.NewCarrierNo, order.ProcNo, this.EmpCd, false);

		}

		public void CheckRegistrableData(string lotno, string carrierno)
		{
			ArmsApi.Model.LotCarrier.CheckRegistrableData(lotno, carrierno);

			//SLP2 年末 基板単位の実績管理機能が実装されたのちは、コメントアウトから復帰させてOK（対応者：吉本・四宮）
			//ArmsApi.Model.Magazine.CheckIdentifyData(lotno);
		}

	}
}