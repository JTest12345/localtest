using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace EICS.Arms
{
	class LotChar
	{
		//private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		public string LotCharCd { get; set; }
		public string LotCharVal { get; set; }
		public string ListVal { get; set; }

		#region Delete

		public static void Delete(int lineCD, string lotno, string lotcharcd)
		{
			Delete(lotno, lotcharcd, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
		}

		public static void Delete(string lotno, string lotcharcd, string constr)
		{
			//ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";
				cmd.Parameters.Add("@CD", SqlDbType.NVarChar).Value = lotcharcd ?? "";

				try
				{
					con.Open();
					cmd.CommandText = "DELETE FROM TnLotCond WHERE lotno=@LOTNO AND lotcharcd=@CD";
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new Exception("LotCond更新エラー", ex);
				}
			}
		}
		#endregion

		#region DeleteInsert

		public void DeleteInsert(int lineCD, string lotno)
		{
			DeleteInsert(lotno, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
		}

		public void DeleteInsert(string lotno, string constr)
		{
			//ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{

				if (this.LotCharVal == null)
				{
					this.LotCharVal = this.ListVal;
				}

				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@CD", SqlDbType.NVarChar).Value = this.LotCharCd ?? "";
				cmd.Parameters.Add("@VAL", SqlDbType.NVarChar).Value = this.LotCharVal ?? "";
				cmd.Parameters.Add("@LISTVAL", SqlDbType.NVarChar).Value = this.ListVal ?? "";
				cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

				try
				{
					con.Open();
					cmd.Transaction = con.BeginTransaction();

					//前履歴は削除
					cmd.CommandText = "DELETE FROM TnLotCond WHERE lotno=@LOTNO AND lotcharcd=@CD";
					cmd.ExecuteNonQuery();

					//新規Insert
					cmd.CommandText = @"
                        INSERT
                         INTO TnLotCond(lotno
                            , lotcharcd
                            , lotcharval
                            , listval
                            , lastupddt)
                        values(@LOTNO
                            , @CD
                            , @VAL
                            , @LISTVAl
                            , @UPDDT)";
					cmd.ExecuteNonQuery();

					cmd.Transaction.Commit();
					//log.Info("UPDATE LOTCHAR" + lotno + ":" + this.LotCharCd + ":" + this.LotCharVal + ":" + this.ListVal);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "UPDATE LOTCHAR" + lotno + ":" + this.LotCharCd + ":" + this.LotCharVal + ":" + this.ListVal);
				}
				catch (Exception ex)
				{
					cmd.Transaction.Rollback();
					throw new Exception("LotCond更新エラー", ex);
				}
			}
		}

		#endregion
	}
}
