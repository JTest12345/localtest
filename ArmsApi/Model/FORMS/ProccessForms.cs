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
			public string ProcNm { get; set; }
			public string FormNo { get; set; }
			public int? FormRev { get; set; }
			public string Workcd { get; set; }
		}

		public enum WorkOrder
		{
			Current,
			Next,
			Previous,
			SkipPrevious,
		}

		/// <summary>
		/// 対象工程の帳票データがクローズしていること
		/// </summary>
		/// <param name="typecd"></param>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <param name="Msg"></param>
		/// <param name="wo"></param>
		/// <returns></returns>
		public static bool IsClosedProcess(string typecd, string lotno, int procno, out string Msg, WorkOrder wo)
		{
			bool retv = false;
			Msg = "";

			//指定工程または指定工程の前後工程の帳票マスタ情報取得
			forminfo pf = GetWorkFlow(typecd, procno, wo);
			if (string.IsNullOrWhiteSpace(pf.FormNo))
			{
				//工程がないor工程が帳票対象でない場合もTrue
				return true;
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				//ダイシング工程でブレンドするとブレンド前の複数のロット番号に対して
				//ブレンド後のロットがworkunitidに設定される
				//全ロットがクローズしているか判定するためclosestateでソートして1件目判定する
				string sql = @" SELECT closestate FROM TnFormTran 
								WHERE typecd = @TypeCd 
								AND (lotno = @LotNo OR workunitid LIKE @WorkUnitID)
								AND procno = @ProcNo
								ORDER BY closestate";

				cmd.Parameters.Add("@TypeCd", System.Data.SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@WorkUnitID", System.Data.SqlDbType.NVarChar).Value = lotno + '%';
				cmd.Parameters.Add("@ProcNo", System.Data.SqlDbType.Int).Value = pf.ProcNo;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					if (rd.Read())
					{
						if (SQLite.ParseInt(rd["closestate"])== 1)
						{
							retv = true;

							if (wo != WorkOrder.Previous)
							{
							retv = true;
								Msg = "帳票入力は完了しています。\r\n";
								Msg += "次工程は開始可能です。";
							}
						}
						else
						{
							if (wo == WorkOrder.Previous)
							{
								Msg = string.Format("前工程の帳票入力は完了していません。 前工程:{0}", pf.ProcNm);
							}
							else
                            {
								Msg = "帳票入力は完了していません。\r\n";
								Msg += "帳票入力が未完了の状態で次工程の開始はできません。";
							}
						}
					}
					else
					{
						Msg = string.Format("帳票データが存在しません 工程:{0}", pf.ProcNm);
					}
				}
			}
			return retv;
		}
		public static bool MergeFormTrn(string typecd, string lotno, int procno, string workunitid, string plantcd, long macno, string empcd)
        {
			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
            {
				con.Open();
				return MergeFormTrn(typecd, lotno, procno, workunitid, plantcd, macno, empcd, cmd);
			}
		}
		public static bool MergeFormTrn(string typecd, string lotno, int procno, string workunitid, string plantcd, long macno, string empcd, SqlCommand cmd)
		{
			//対象工程の帳票情報取得
			forminfo pf = GetWorkFlow(typecd, procno, WorkOrder.Current);
			if (string.IsNullOrWhiteSpace(pf.FormNo))
			//対象工程の帳票情報が無ければ処理を抜ける
			{
				return true;
            }

			cmd.Parameters.Clear();
			cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
			cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
			cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = pf.ProcNo;
			cmd.Parameters.Add("@MANUBASE", SqlDbType.VarChar).Value = "CET";
			if (workunitid == null)
				cmd.Parameters.Add("@WORKUNITID", SqlDbType.NVarChar).Value = DBNull.Value;
			else
				cmd.Parameters.Add("@WORKUNITID", SqlDbType.NVarChar).Value = workunitid;
			cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = pf.Workcd;
			cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;
			cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
			cmd.Parameters.Add("@FORMNO", SqlDbType.NVarChar).Value = pf.FormNo;
			if (pf.FormRev != null)
            {
				cmd.Parameters.Add("@FORMREV", SqlDbType.Int).Value = pf.FormRev;
			}
			cmd.Parameters.Add("@CLOSESTATE", SqlDbType.Int).Value = 0;
			cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = empcd;
			cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

			cmd.CommandText = @"
                    MERGE INTO TnFormTran as t USINg (
                    SELECT @TYPECD as typecd
	                    , @LOTNO as lotno
	                    , @PROCNO as procno
	                    , @MANUBASE as manubase
	                    , @WORKUNITID as workunitid
	                    , @WORKCD as workcd
	                    , @PLANTCD as plantcd
	                    , @MACNO as macno
                        , formno
                        , formrev
                        , coj
	                    , @CLOSESTATE closestate
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
                    UPDATE SET plantcd = isnull(m.plantcd, t.plantcd), macno = isnull(m.macno, t.macno), workunitid = isnull(m.workunitid, t.workunitid), updateat = m.lastupddt, updateby = m.empcd
                    WHEN NOT MATCHED THEN
                    INSERT (typecd, lotno, procno, manubase, workunitid, workcd, plantcd, macno, formno, formrev, coj, closestate, insertat, insertby)
                    VALUES (typecd, lotno, procno, manubase, workunitid, workcd, plantcd, macno, formno, formrev, coj, closestate, lastupddt, empcd);";

			int rcnt = cmd.ExecuteNonQuery();

			string key2 = "";
			if (pf.FormRev == null) key2 = $" iscurrversion: 1";
			else key2 = $"formrev:{pf.FormRev.ToString()}";

			if (rcnt == 0) throw new ArmsException($"帳票マスタが存在しません　formno：{pf.FormNo} " + key2);
			if (rcnt > 1) throw new ArmsException($"帳票マスタが複数存在します　formno：{pf.FormNo} " + key2);

			return true;
		}

		public static forminfo GetWorkFlow(string typecd, int procno, WorkOrder wo)
		{
			forminfo retv = new forminfo();
			forminfo svretv = new forminfo();

			try
			{
				using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
				using (SqlCommand cmd = con.CreateCommand())
				{
					string sql = @"
                        SELECT w.procno, w.formno, w.formrev, p.procnm, p.workcd
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
					else if (wo == WorkOrder.Previous || wo == WorkOrder.SkipPrevious)
					{
						sql += " ORDER BY w.workorder";
					}

					cmd.CommandText = sql;
					con.Open();
					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							if (SQLite.ParseInt(rd["procno"]) == procno && wo != WorkOrder.Current)
                            {
								break;
                            }
							svretv = retv;
							retv = new forminfo();
							retv.ProcNo = SQLite.ParseInt(rd["procno"]);
							retv.ProcNm = SQLite.ParseString(rd["procnm"]);
							retv.Workcd = SQLite.ParseString(rd["workcd"]);

							if (!string.IsNullOrWhiteSpace(SQLite.ParseString(rd["formno"])))
							{
								retv.FormNo = SQLite.ParseString(rd["formno"]);
							}
							if (rd["FormRev"] != DBNull.Value)
							{
								retv.FormRev = SQLite.ParseInt(rd["formrev"]);
							}

						}
						//前工程をスキップするロットで、前工程がスキップ対象の場合さらに前の工程を返す
						if (wo == WorkOrder.SkipPrevious && retv.ProcNo == ArmsApi.Config.Settings.AFTER_CURING_CONFIRM)
						{
							retv = svretv;
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw new ArmsException("TnWorkFlow取得エラー\r\n" + ex.ToString());
			}

			return retv;
		}

		//20220627 ADD START
		public static string GetWorkUnitID(string typecd, string lotno, int procno)
		{
			string workunitid = null;
			
			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @"SELECT ISNULL(workunitid, '') as workunitid
                                 FROM TnFormTran 
								WHERE typecd = @TypeCd 
				                  AND lotno  = @LotNo
                                  AND procno = @ProcNo
							  ";
				cmd.Parameters.Add("@TypeCd", System.Data.SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@ProcNo", System.Data.SqlDbType.Int).Value = procno;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					if (rd.Read())
					{
						workunitid = SQLite.ParseString(rd["workunitid"]);
					}
				}
			}
			return workunitid;
		}
		//20220627 ADD START
	}
}

