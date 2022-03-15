using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// 富士情報　淳一さんのロジックを参照して作成
namespace ArmsApi.Model.FORMS
{
	public class ProccessForms
	{
		public class forminfo
		{
			public int ProcNo { get; set; }
			public string FormNo { get; set; }
			public int? FormRev { get; set; }
			public string Workcd { get; set; }
		}

		public enum WorkOrder
		{
			Current,
			Next,
			Previous
		}

		//帳票システムで前の工程がクローズしていること
		public static bool isClosedPrevProcess(string typecd, string lotno, int procno, out string Msg)
		{
			bool retv = false;
			Msg = "";

			//帳票対象の前の工程取得
			forminfo pf = GetWorkFlow(typecd, procno, WorkOrder.Previous);
			if (string.IsNullOrEmpty(pf.FormNo))
			{
				//先頭工程の場合は前の工程がないのでTrue
				//前の工程が帳票対象でない場合もTrue
				return true;
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT isclosed FROM TnFormTran 
								WHERE typecd = @TypeCd 
								AND lotno = @LotNo
								AND procno = @ProcNo ";

				cmd.Parameters.Add("@TypeCd", System.Data.SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@ProcNo", System.Data.SqlDbType.Int).Value = pf.ProcNo;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					if (rd.Read())
					{
						if (SQLite.ParseBool(rd["isclosed"]))
						{
							retv = true;
						}
						else
						{
							Msg = string.Format("帳票がクローズしていません 工程ID:{0}", pf.ProcNo);
							retv = false;
						}
					}
					else
					{
						Msg = string.Format("帳票データが存在しません 工程ID:{0}", pf.ProcNo);
						retv = false;
					}
				}
			}
			return retv;
		}

		public static bool MergeFormTrn(string typecd, string lotno, int procno, string plantcd, long macno, string empcd)
        {
			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
            {
				con.Open();
				return MergeFormTrn(typecd, lotno, procno, plantcd, macno, empcd, cmd);
			}
		}
		public static bool MergeFormTrn(string typecd, string lotno, int procno, string plantcd, long macno, string empcd, SqlCommand cmd)
		{
			//対象工程の帳票情報取得
			forminfo pf = GetWorkFlow(typecd, procno, WorkOrder.Current);
			if (pf.FormNo == "")
			//対象工程の帳票情報が無ければ処理を抜ける
			{
				return true;
            }

			cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
			cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
			cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = pf.ProcNo;
			cmd.Parameters.Add("@MANUBASE", SqlDbType.VarChar).Value = "CET";
			cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = pf.Workcd;
			cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;
			cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
			cmd.Parameters.Add("@FORMNO", SqlDbType.NVarChar).Value = pf.FormNo;
			if (pf.FormRev != null)
            {
				cmd.Parameters.Add("@FORMREV", SqlDbType.Int).Value = pf.FormRev;
			}
			cmd.Parameters.Add("@ISCLOSED", SqlDbType.Int).Value = SQLite.SerializeBool(false);
			cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = empcd;
			cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

			cmd.CommandText = @"
                    MERGE INTO TnFormTran as t USINg (
                    SELECT @TYPECD as typecd
	                    , @LOTNO as lotno
	                    , @PROCNO as procno
	                    , @MANUBASE as manubase
	                    , @WORKCD as workcd
	                    , @PLANTCD as plantcd
	                    , @MACNO as macno
                        , formno
                        , formrev
                        , coj
	                    , @ISCLOSED isclosed
	                    , @EMPCD empcd 
	                    , @LASTUPDDT lastupddt
                    FROM TmFormMaster WITH (NOLOCK)
                    WHERE formno = @FORMNO";

			if (pf.FormRev == null)
			{
				cmd.CommandText += " AND iscurrversion = 1";
			}
			else
			{
				cmd.CommandText += " AND formrev = @FORMREV";
			}

			cmd.CommandText += @"
	                ) as m ON ( t.typecd = m.typecd AND t.lotno = m.lotno AND t.procno = m.procno)
                    WHEN MATCHED THEN
                    UPDATE SET plantcd = m.plantcd, macno = m.macno, updateat = m.lastupddt, updateby = m.empcd
                    WHEN NOT MATCHED THEN
                    INSERT (typecd, lotno, procno, manubase, Workcd, plantcd, macno, formno, formrev, coj, isclosed, insertat, insertby)
                    VALUES (typecd, lotno, procno, manubase, Workcd, plantcd, macno, formno, formrev, coj,  isclosed, lastupddt, empcd);";

			int rcnt = cmd.ExecuteNonQuery();

			if (rcnt == 0)
            {
				throw new ArmsException($"帳票マスタが存在しません　formno：{pf.FormNo} formrev:{pf.FormRev}");
			}
			if (rcnt > 1)
			{
				throw new ArmsException($"帳票マスタが複数存在します　formno：{pf.FormNo} formrev:{pf.FormRev}");
			}
			return true;
		}

		public static forminfo GetWorkFlow(string typecd, int procno, WorkOrder wo)
		{
			forminfo retv = new forminfo();

			try
			{
				using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
				using (SqlCommand cmd = con.CreateCommand())
				{
					string sql = @"
                        SELECT w.procno, w.formno, w.formrev, p.workcd
                        FROM TmWorkFlow w WITH (NOLOCK)
						INNER JOIN TmProcess p WITH(nolock) ON w.procno = p.procno
                        WHERE w.typecd = @TYPECD AND w.delfg = 0 AND p.delfg = 0";

					cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

					if (wo == WorkOrder.Current)
                    {
                        cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;
                        sql += " AND w.procno = @PROCNO"; 
					}
					else if (wo == WorkOrder.Next)
					{
						sql += " ORDER BY w.workorder Desc";
					}
					else if (wo == WorkOrder.Previous)
					{
						sql += " ORDER BY w.workorder";
					}

					cmd.CommandText = sql;
					con.Open();
					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							if (SQLite.ParseInt(rd["procno"]) == procno)
                            {
								if (wo == WorkOrder.Current)
								{
									retv.ProcNo = SQLite.ParseInt(rd["procno"]);
									retv.Workcd = SQLite.ParseString(rd["workcd"]);
									if (string.IsNullOrEmpty(SQLite.ParseString(rd["formno"])))
                                    {
										retv.FormNo = "";
									}
									else
                                    {
										retv.FormNo = SQLite.ParseString(rd["formno"]);
									}
									if (rd["FormRev"] != DBNull.Value)
									{
										retv.FormRev = SQLite.ParseInt(rd["formrev"]);
									}
								}
								break;
                            }
							retv.ProcNo = SQLite.ParseInt(rd["procno"]);
							retv.Workcd = SQLite.ParseString(rd["workcd"]);

							if (string.IsNullOrEmpty(SQLite.ParseString(rd["formno"])))
							{
								retv.FormNo = "";
							}
							else
							{
								retv.FormNo = SQLite.ParseString(rd["formno"]);
							}
							if (rd["FormRev"] != DBNull.Value)
							{
								retv.FormRev = SQLite.ParseInt(rd["formrev"]);
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw new ArmsException("TnWorkFlow取得エラー/r/n" + ex.ToString());
			}

			return retv;
		}
		public static bool UpdateMacInfo(string typecd, string lotno, int procno, string plantcd, long macno, string empcd)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;
				cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;
				cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
				cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = empcd;
				cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

				try
				{
					con.Open();

					#region Updateコマンド

					cmd.CommandText = @"
                        UPDATE [TnFormTran]
                        SET 
                            plantcd = @PLANTCD, 
                            macno = @MACNO, 
							updateby = @EMPCD,
                            updateat = @LASTUPDDT 
                        WHERE 
                            typecd = @TYPECD AND lotno = @LOTNO AND procno = @PROCNO";
					#endregion

					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException("帳票開始情報更新エラー/r/n" + ex.ToString());
				}
			}

			return true;
		}
	}
}

