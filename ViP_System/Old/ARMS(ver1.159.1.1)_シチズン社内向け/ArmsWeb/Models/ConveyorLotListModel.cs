using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class ConveyorLotListModel
	{
        public ConveyorLotListModel(string lineno, int num)
        {
            try
            {
				bool errorfg = false;
				if(num == 1)
				{
					errorfg = true;
				}
				//コンベア情報取得
				List<AsmLotLog> lots = AsmLot.GetLotLog(lineno, errorfg, true);
				//逆順ソート
				lots.Reverse();

				
                this.cList = new List<ConveyorList>();
                foreach (AsmLotLog lot in lots)
				{
					ConveyorList conv = new ConveyorList();

					conv.LotLog = lot;
					conv.Lot = AsmLot.GetAsmLot(lot.LotNo);                         
					conv.Proc = Process.GetNextProcess(Convert.ToInt32(lot.ProcNo), conv.Lot);
                    this.cList.Add(conv);
                }
            }
            catch (Exception ex)
            {
                this.cList = null;
                this.ErrMsg = ex.ToString();
            }
        }

        /// <summary>
        /// 完了待ち状態の仮想マガジンレコード一覧
        /// </summary>
        public List<ConveyorList> cList { get; set; }

        public string ErrMsg { get; set; }
    }


	public class ConveyorList
	{
		public AsmLotLog LotLog { get; set; }
		public AsmLot Lot { get; set; }

		public Process Proc { get; set; }
	}

}