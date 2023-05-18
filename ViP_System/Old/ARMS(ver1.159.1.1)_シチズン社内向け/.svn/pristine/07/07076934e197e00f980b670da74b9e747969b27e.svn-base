using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Models
{
	public class TransferCarrierModel
	{
		private const int OutputMpdProcNo = 999;

		public TransferCarrierModel()
		{
			ProcNM = "";
		}

		public string LotNo { get; set; }
		public class RingCmb
		{
			public string RingNo { get; set; }
			public string NextRingNo { get; set; }
		}

		public string ringno { get; set; }
		public string nextringno { get; set; }
		public List<RingCmb> RingList { get; set; }
		public string CarrierNo { get; set; }
		public string NewCarrierNo { get; set; }
		public int ProcNo { get; set; }
		public string ProcNM { get; set; }
		public string EmpCd { get; set; }
		public DateTime LastUpdDt { get; set; }
		public bool OnlyOutputMpdFg { get; set; }
		public string MagazineNo { get; set; }

		//転写処理
		public void InputCheck(string nowRingID, string nextRingID)
		{
			string[] detcst = nowRingID.Split(' ');
			string[] atacst = nextRingID.Split(' ');

			TransferCarrierModel.RingCmb ringcmb = new TransferCarrierModel.RingCmb();

			//転写前リング登録
			if (detcst.Length >= 3)
			{
				ringcmb.RingNo = nowRingID;
			}
			//else if (detcst.Length == 4)
			//{
			//	ringcmb.RingNo = nowRingID;
			//}
			else
			{
				throw new ApplicationException(string.Format("フォーマットエラー：リングIDを読み込んでください。"));
			}

			if (this.LotNo == null)
			{
				this.LotNo = LENS2_Api.ARMS.LotCarrier.GetLotNo(ringcmb.RingNo, true, false);
				if (this.LotNo == null)
				{
					throw new ApplicationException(string.Format("紐付けられたロットが存在しないか、稼働中ではありません。リング : {0}",ringcmb.RingNo));
				}
				this.ProcNo = ArmsApi.Model.LotCarrier.GetProc(this.LotNo);
				if (this.ProcNo == 0)
				{
					throw new ApplicationException(string.Format("現在工程が不正です。"));
				}
				ArmsApi.Model.Process proc = ArmsApi.Model.Process.GetProcess(this.ProcNo);
				this.ProcNM = proc.InlineProNM;
			}
			else
			{
				if (this.LotNo != LENS2_Api.ARMS.LotCarrier.GetLotNo(ringcmb.RingNo, true))
				{
					throw new ApplicationException(string.Format("現在選択中のロットに紐づいていません。ロット : {0} ,リング ： {1}", this.LotNo,ringcmb.RingNo));
				}
			}

			//転写後リング登録
			if (atacst.Length >= 3)
			{
				ringcmb.NextRingNo = nextRingID;
			}
			//else if (atacst.Length == 4)
			//{
			//	ringcmb.NextRingNo = nextRingID;
			//}
			else
			{
				throw new ApplicationException(string.Format("フォーマットエラー：リングIDを読み込んでください。"));
			}
			if (LENS2_Api.ARMS.LotCarrier.GetLotNo(ringcmb.NextRingNo, true, false) != null)
			{
				throw new ApplicationException(string.Format("既に他のロットに紐づいています。リング : {0}", ringcmb.NextRingNo));
			}
			this.RingList.Add(ringcmb);
		}
			


		//転写処理
		//public void RingTransfer()
		//{
		//	using (SqlConnection conLens = new SqlConnection(LENS2_Api.Config.Settings.LensConnectionString))
		//	using (SqlConnection conArms = new SqlConnection(SQLite.ConStr))
		//	using (SqlCommand cmdArms = conArms.CreateCommand())
		//	{
		//		SqlCommand cmdLens = conLens.CreateCommand();
		//		conArms.Open();
		//		conLens.Open();

		//		using (SqlTransaction tranArms = conArms.BeginTransaction())
		//		using (SqlTransaction tranLens = conLens.BeginTransaction())
		//			try
		//			{
		//				cmdArms.Transaction = tranArms;
		//				cmdLens.Transaction = tranLens;
		//				foreach (TransferCarrierModel.RingCmb ring in this.RingList)
		//				{
		//					//転写対応（TnLotCarrierデータ移行）
		//					ArmsApi.Model.LotCarrier.Transfer(cmdArms, this.LotNo, ring.RingNo, ring.NextRingNo, this.ProcNo, "660");
							
		//					//TnMapResult更新
		//					//MAPデータの有無確認
		//					string mappingData = LENS2_Api.MapResult.GetMappingData(this.LotNo, this.ProcNo, ring.RingNo, null, true);
		//					//MAPデータが存在する時
		//					if (mappingData != "")
		//					{
		//						//登録済みデータの確認
								
		//							if (!LENS2_Api.MapResult.SaveData(ref cmdLens, this.LotNo, ring.NextRingNo, ring.NextRingNo, this.ProcNo, mappingData, "660", false, 0, 0, 0, 0))
		//							{
		//								//アップデート
		//								cmdLens.Transaction.Rollback();
		//								throw new ApplicationException(string.Format("マッピングデータの登録に失敗しました。データ名：{0}", ring.NextRingNo));
		//							}
		//					}
		//				}
		//				tranArms.Commit();
		//				tranLens.Commit();
		//			}

		//			catch (Exception err)
		//			{
		//				cmdLens.Transaction.Rollback();
		//				cmdLens.Transaction.Rollback();
		//			}
		//	}
		//}
	}
}