using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.FORMS
{
	public class ProccessForms
	{
		public class forminfo
		{
			public int ProcNo { get; set; }
			public string FormNo { get; set; }
			public int FormRev { get; set; }
			public string Workcd { get; set; }
		}

		public static string InsertProcForms(string typecd, string lotno)
		{
			List<forminfo> ListProcForms = new List<forminfo>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL
				string sql = @"
                        SELECT TmWorkFlow.procno as procno, TmWorkFlow.formno as formno, TmWorkFlow.formrev as formrev, TmProcess.workcd as workcd
                        FROM TmWorkFlow WITH (NOLOCK)
						INNER JOIN TmProcess WITH(nolock) ON TmWorkFlow.procno = TmProcess.procno
                        WHERE (TmWorkFlow.typecd = @TYPECD)
                        ORDER BY TmWorkFlow.workorder";
				#endregion

				cmd.CommandText = sql;
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						forminfo forminfo = new forminfo();

						forminfo.ProcNo = Convert.ToInt32(rd["procno"]);
						forminfo.FormNo = rd["formno"].ToString().Trim();
						forminfo.FormRev = Convert.ToInt32(rd["formrev"]);
						forminfo.Workcd = rd["workcd"].ToString().Trim();

						ListProcForms.Add(forminfo);
					}
				}
			}

			if (ListProcForms.Count() == 0)
			{
				return "製品コードがTnWorkFlowに登録されていません";
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.FORMSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				foreach (var lpf in ListProcForms)
				{
					cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
					cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
					cmd.Parameters.Add("@ISCLOSED", SqlDbType.Int).Value = 0;
					cmd.Parameters.Add("@INSERTAT", SqlDbType.DateTime).Value = DateTime.Now;
					cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = lpf.ProcNo;
					cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = lpf.Workcd;
					cmd.Parameters.Add("@FORMNO", SqlDbType.NVarChar).Value = lpf.FormNo;
					cmd.Parameters.Add("@FORMREV", SqlDbType.Int).Value = lpf.FormRev;
					cmd.Parameters.Add("@MANUBASE", SqlDbType.NVarChar).Value = "CET";

					List<string> queryformdata = new List<string>();

					#region SQL
					string sql = @"
                        SELECT formdata
                        FROM dbo.TmFormMaster WITH (NOLOCK)
                        WHERE (formno = @FORMNO) AND (revision = @FORMREV)";
					#endregion

					cmd.CommandText = sql;
					con.Open();

					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							var formdata = rd["formdata"].ToString().Trim();

							queryformdata.Add(formdata);
						}
					}
					con.Close();


					if (queryformdata.Count() == 0)
					{
						return "帳票マスタ読込異常：マスタがヒットしませんでした";
					}
					else if (queryformdata.Count() > 1)
					{
						return "帳票マスタ読込異常：複数のマスタを読み込んでいます";
					}

					cmd.Parameters.Add("@FORMDATA", SqlDbType.NVarChar).Value = queryformdata[0];

					try
					{
						con.Open();

						cmd.CommandText = @"
                        INSERT INTO [TnFormTran]
                            (typecd, lotno, procno, workcd, formno, formrev, formdata, isclosed, insertat, manubase)
                        VALUES
                            (@TYPECD,@LOTNO,@PROCNO,@WORKCD, @FORMNO,@FORMREV,@FORMDATA,@ISCLOSED,@INSERTAT, @MANUBASE)";
						cmd.ExecuteNonQuery();

						con.Close();
					}
					catch (Exception ex)
					{
						throw new ArmsException("ロット<->帳票データ新規挿入エラー:" + ex.ToString());
					}

					cmd.Parameters.Clear();
				}

			}

			return "OK";
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
					throw new ArmsException("帳票開始情報更新エラー:" + ex.ToString());
				}
			}

			return true;
		}

	}
}

