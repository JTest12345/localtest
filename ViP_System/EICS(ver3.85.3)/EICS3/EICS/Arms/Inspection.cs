using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace EICS.Arms
{
	class Inspection
	{
		private string _lotno;
		/// <summary>
		/// ロット番号
		/// </summary>
		public string LotNo { get { return _lotno; } set { _lotno = Order.MagLotToNascaLot(value); } }

		/// <summary>
		/// 工程
		/// </summary>
		public int ProcNo { get; set; }

		//private string procnm;
		//public string ProcNm
		//{
		//    get
		//    {
		//        if (string.IsNullOrEmpty(procnm))
		//        {
		//            Process p = Process.GetProcess(ProcNo);
		//            if (p != null)
		//            {
		//                this.procnm = p.InlineProNM;
		//            }
		//        }
		//        return procnm;
		//    }
		//}

		/// <summary>
		/// 検査済みフラグ
		/// </summary>
		public bool IsInspected { get; set; }


		#region GetInspection

		/// <summary>
		/// 検査必要工程情報取得
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static Inspection GetInspection(int lineCD, string lotno, int procno)
		{
			Inspection[] list = GetInspections(lineCD, lotno);

			foreach (Inspection isp in list)
			{
				if (isp.ProcNo == procno)
				{
					return isp;
				}
			}

			return null;
		}
		#endregion

		#region GetInspections


		public static Inspection[] GetInspections(int lineCD, string lotno)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			List<Inspection> retv = new List<Inspection>();

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{


				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

				try
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT 
                          lotno , 
                          procno , 
                          sgyfg 
                        FROM 
                          TnInspection
                        WHERE 
                          lotno = @LOTNO";

					cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{

						while (reader.Read())
						{
							Inspection i = new Inspection();
							i.LotNo = SQLite.ParseString(reader["lotno"]);
							i.ProcNo = SQLite.ParseInt(reader["procno"]);
							i.IsInspected = SQLite.ParseBool(reader["sgyfg"]);

							retv.Add(i);
						}
					}
				}
				catch (Exception ex)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
					throw ex;
				}
			}
			return retv.ToArray();
		}
		#endregion

		#region 原材料抜き取り設定


		//private class SamplingSetting
		//{
		//    public string HMGP { get; set; }
		//    public int ProcNo { get; set; }
		//    /// <summary>
		//    /// ロット単位の抜き取りフラグ
		//    /// Falseの場合は同一ロットでも交換時・装置単位で抜き取り
		//    /// </summary>
		//    public bool IsEveryLot { get; set; }
		//}

		/// <summary>
		/// 抜き取り検査フラグ操作
		/// </summary>
		/// <param name="ord"></param>
		/// <param name="lotno"></param>
		//public static void Sampling(Order ord, string lotno)
		//{
		//    if (!ord.IsComplete) return;

		//    Machine mac = Machine.GetMachine(ord.MacNo);
		//    if (mac == null)
		//    {
		//        return;
		//    }
		//    Material[] matlist = mac.GetMaterials(ord.WorkStartDt, ord.WorkEndDt);

		//    SamplingSetting[] ss = getSettings();
		//    foreach (Material mat in matlist)
		//    {
		//        IEnumerable<SamplingSetting> found = ss.Where(s => s.HMGP == mat.HMGroup);

		//        if (found != null && found.Count() >= 1)
		//        {
		//            if (mat.IsFrame)
		//            {
		//                #region フレームのストッカー判定

		//                //フレームの場合、混載は抜き取り対象にしないため、片方のストッカーの指図のみ対象
		//                if (ord.ScNo1 == "1" && ord.ScNo2 == "1")
		//                {
		//                    continue;
		//                }
		//                if (mat.StockerNo == 1)
		//                {
		//                    if (ord.ScNo1 == "0") continue;
		//                }
		//                if (mat.StockerNo == 2)
		//                {
		//                    if (ord.ScNo2 == "0") continue;
		//                }
		//                #endregion
		//            }

		//            if (mat.IsWafer)
		//            {
		//                throw new ArmsException("ウェハー抜き取り未対応");
		//            }


		//            bool isTimeSampled = false;
		//            bool isLotSampled = false;

		//            foreach (SamplingSetting setting in found)
		//            {
		//                if (setting.IsEveryLot)
		//                {
		//                    if (mat.IsLotSampled == true) continue;
		//                    Inspection isp = new Inspection();
		//                    isp.LotNo = lotno;
		//                    isp.ProcNo = setting.ProcNo;
		//                    isp.DeleteInsert();

		//                    isLotSampled = true;

		//                    AsmLot lot = AsmLot.GetAsmLot(lotno);
		//                    lot.IsChangePointLot = true;
		//                    lot.Update();

		//                    log.Info("[原材料検査抜き取り]" + lotno + ":" + mat.MaterialCd + ":" + mat.LotNo);
		//                }
		//                else
		//                {
		//                    if (mat.IsTimeSampled == true) continue;
		//                    Inspection isp = new Inspection();
		//                    isp.LotNo = lotno;
		//                    isp.ProcNo = setting.ProcNo;
		//                    isp.DeleteInsert();

		//                    isTimeSampled = true;

		//                    AsmLot lot = AsmLot.GetAsmLot(lotno);
		//                    lot.IsChangePointLot = true;
		//                    lot.Update();

		//                    log.Info("[原材料検査抜き取り]" + lotno + ":" + mat.MaterialCd + ":" + mat.LotNo + ":" + mac.MacNo + ":" + mat.InputDt);
		//                }
		//            }

		//            if (isTimeSampled)
		//            {
		//                mat.IsTimeSampled = true;
		//                mac.DeleteInsertMacMat(mat);
		//            }
		//            if (isLotSampled)
		//            {
		//                UpdateLotSampled(mat.MaterialCd, mat.LotNo);
		//            }

		//        }
		//    }
		//}


		/// <summary>
		/// 抜き取り設定取得
		/// </summary>
		/// <returns></returns>
		//private static SamplingSetting[] getSettings()
		//{
		//    List<SamplingSetting> retv = new List<SamplingSetting>();

		//    using (SqlConnection con = new SqlConnection(SQLite.ConStr))
		//    using (SqlCommand cmd = con.CreateCommand())
		//    {
		//        con.Open();
		//        cmd.CommandText = "SELECT hmgroup, procno, everylotfg FROM TmMatSampling";

		//        using (SqlDataReader rd = cmd.ExecuteReader())
		//        {
		//            while (rd.Read())
		//            {
		//                SamplingSetting s = new SamplingSetting();
		//                s.HMGP = SQLite.ParseString(rd["hmgroup"]);
		//                s.ProcNo = SQLite.ParseInt(rd["procno"]);
		//                s.IsEveryLot = SQLite.ParseBool(rd["everylotfg"]);

		//                retv.Add(s);
		//            }
		//        }
		//    }

		//    return retv.ToArray();
		//}

		//public static void UpdateLotSampled(string matcd, string lotno)
		//{
		//    using (SqlConnection con = new SqlConnection(SQLite.ConStr))
		//    using (SqlCommand cmd = con.CreateCommand())
		//    {
		//        try
		//        {
		//            con.Open();

		//            cmd.Parameters.Add("@MATCD", System.Data.SqlDbType.NVarChar).Value = matcd;
		//            cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotno;

		//            cmd.CommandText = "UPDATE TnMaterials SET issampled=1 WHERE materialcd=@MATCD AND lotno=@LOTNO";
		//            cmd.ExecuteNonQuery();
		//        }
		//        catch (Exception ex)
		//        {
		//            throw new ArmsException("抜き取り検査フラグ更新失敗:" + matcd + ":" + lotno, ex);
		//        }
		//    }
		//}
		#endregion


		public void DeleteInsert(int lineCD)
		{
			DeleteInsert(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
		}

		public void DeleteInsert(string constr)
		{
			//ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				try
				{
					con.Open();
					cmd.Transaction = con.BeginTransaction();

					cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
					cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
					cmd.Parameters.Add("@SGYFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsInspected);
					cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;


					//前履歴は削除
					cmd.CommandText = "DELETE FROM TnInspection WHERE lotno=@LOTNO AND procno=@PROC";
					cmd.ExecuteNonQuery();

					//新規Insert
					cmd.CommandText = @"
                            INSERT
                             INTO TnInspection(lotno
	                            , procno
	                            , sgyfg
	                            , lastupddt)
                            values(@LOTNO
	                            , @PROC
                                , @SGYFG
	                            , @UPDDT )";

					cmd.ExecuteNonQuery();

					cmd.Transaction.Commit();
				}
				catch (Exception ex)
				{
					cmd.Transaction.Rollback();
					throw new Exception("抜き取り検査設定エラー:" + this.LotNo, ex);
				}
			}
		}


		/// <summary>
		/// 抜取り検査設定自体を削除
		/// 通常は完了フラグONで更新を行うので、
		/// このメソッドは検査機の投入フラグ自体を消す必要がある場合のみ。
		/// </summary>
		//public void Delete()
		//{
		//    using (SqlConnection con = new SqlConnection(SQLite.ConStr))
		//    using (SqlCommand cmd = con.CreateCommand())
		//    {
		//        try
		//        {
		//            con.Open();

		//            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
		//            cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;

		//            cmd.CommandText = "DELETE FROM TnInspection WHERE lotno=@LOTNO AND procno=@PROC";
		//            cmd.ExecuteNonQuery();

		//            cmd.ExecuteNonQuery();
		//        }
		//        catch (Exception ex)
		//        {
		//            throw new Exception("抜き取り検査設定エラー:" + this.LotNo, ex);
		//        }
		//    }
		//}
	}
}
