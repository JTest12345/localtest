using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EICS.Arms
{
	class Process
	{
		/// <summary>
		/// 工程のマガジン分割状態
		/// </summary>
		public enum MagazineDevideStatus : int
		{
			Single = 0,
			Double = 2,
			SingleToDouble = 1,
			DoubleToSingle = 3,
			Unknown = 99,
		}

		public override string ToString()
		{
			return this.InlineProNM;
		}

		private const int DISCHARGE_PROC_NO = -1;

		#region プロパティ

		/// <summary>
		/// 工程グループCd
		/// </summary>
		public string ProcGroupCd { get; set; }

		/// <summary>
		/// インライン工程No
		/// </summary>
		public int ProcNo { get; set; }

		/// <summary>
		/// インライン工程名
		/// </summary>
		public string InlineProNM { get; set; }

		/// <summary>
		/// NASCA作業CD
		/// </summary>
		public string WorkCd { get; set; }

		/// <summary>
		/// 最終ステータス
		/// </summary>
		public bool FinalSt { get; set; }

		/// <summary>
		/// 開始作業
		/// </summary>
		public bool FirstSt { get; set; }

		/// <summary>
		/// 抜き取り検査工程
		/// </summary>
		public bool IsSamplingInspection { get; set; }

		/// <summary>
		/// マガジン変更
		/// </summary>
		public bool MagazineChgFg { get; set; }

		/// <summary>
		/// ダミー登録用設備CD
		/// </summary>
		public int? AutoUpdMachineNo { get; set; }

		/// <summary>
		/// マガジン分割状態
		/// </summary>
		public int MagDevideStatus { get; set; }

		/// <summary>
		/// 作業順
		/// </summary>
		public int WorkOrder { get; set; }

		#endregion

		#region インライン工程マスタ情報取得


		public static Process GetProcess(int lineCD, string workcd)
		{
			return GetProcess(lineCD, null, workcd);
		}


		public static Process GetProcess(int lineCD, long procNo)
		{
			return GetProcess(lineCD, procNo, null);
		}



		/// <summary>
		/// procnoまたはworkcdをキーにProcessを取得
		/// 条件一致無しの場合はNull、複数結果の場合は先頭を返す
		/// </summary>
		/// <param name="procNo"></param>
		/// <param name="workcd"></param>
		/// <returns></returns>
		private static Process GetProcess(int lineCD, long? procNo, string workcd)
		{
			Process[] proclist = SearchProcess(lineCD, procNo, workcd);

			if (proclist.Length == 0)
			{
				return null;
			}
			else
			{
				return proclist[0];
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="procno"></param>
		/// <param name="workcd">Null許可</param>
		/// <returns></returns>
		public static Process[] SearchProcess(int lineCD, long? procNo, string workcd)
		{
			List<Process> retv = new List<Process>();

			try
			{
				using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					string sql = @"
                       SELECT
                        procgroupcd
                        , procno
                        , procnm
                        , workcd
                        , finalst
                        , firstst
                        , samplingfg
                        , magazinechgfg
                        , autoupdmachineno
                        FROM TmProcess
                        WHERE delfg=0 ";

					if (procNo.HasValue)
					{
						sql += " AND procno = @PROCNO";
						cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;
					}

					if (workcd != null)
					{
						sql += " AND workcd = @WORKCD";
						cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = workcd;
					}


					cmd.CommandText = sql.Replace("\r\n", "");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							Process p = new Process();

							p.ProcGroupCd = SQLite.ParseString(reader["procgroupcd"]);
							p.ProcNo = SQLite.ParseInt(reader["procno"]);
							p.InlineProNM = SQLite.ParseString(reader["procnm"]);
							p.WorkCd = SQLite.ParseString(reader["workcd"]);
							p.FinalSt = SQLite.ParseBool(reader["finalst"]);
							p.FirstSt = SQLite.ParseBool(reader["firstst"]);
							p.IsSamplingInspection = SQLite.ParseBool(reader["samplingfg"]);
							p.MagazineChgFg = SQLite.ParseBool(reader["magazinechgfg"]);
							p.AutoUpdMachineNo = (reader["AutoUpdMachineNo"] != DBNull.Value) ? (int?)SQLite.ParseInt(reader["AutoUpdMachineNo"]) : null;

							retv.Add(p);
						}
					}
				}
			}
			catch (Exception ex)
			{
				//LogManager.GetCurrentClassLogger().Error(ex);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
				throw ex;
			}

			return retv.ToArray();
		}


		#endregion

		#region ワークフロー取得 GetWorkFlow
		public static Process[] GetWorkFlow(int lineCD, string typecd, string procGroupcd)
		{
			List<Process> retlist = new List<Process>();

			try
			{
				using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					string sql = @"
                        SELECT
                            procno, workorder
                        FROM
                            TmWorkFlow WITH(nolock)
                        WHERE
                            typecd = @TYPECD AND delfg=0 ";

					sql += " ORDER BY workorder";

					cmd.CommandText = sql.Replace("\r\n", "");
					cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = typecd ?? "";

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							int procno = SQLite.ParseInt(reader["procno"]);
							Process retv = Process.GetProcess(lineCD, procno);

							if (procGroupcd != string.Empty && procGroupcd != retv.ProcGroupCd)
							{
								//同工程のみ取得
								continue;
							}
							retv.WorkOrder = SQLite.ParseInt(reader["workorder"]);

							retlist.Add(retv);
						}
					}
				}
			}
			catch (Exception ex)
			{
				//LogManager.GetCurrentClassLogger().Error(ex);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
				throw ex;
			}

			return retlist.ToArray();


		}

		public static Process[] GetWorkFlow(int lineCD, string typecd)
		{
			return GetWorkFlow(lineCD, typecd, string.Empty);
		}
		#endregion

		#region ワークフロー　指図発行必要値取得 GetOrderMove
		public static int GetOrderMove(int lineCD, string typecd, int procno)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					string sql = @"
                        SELECT
                            ordermove
                        FROM
                            TmWorkFlow
                        WHERE
                            typecd =  @TYPECD AND procno=@PROCNO AND delfg=0
                        ORDER BY workorder";

					cmd.CommandText = sql.Replace("\r\n", "");
					cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
					cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;


					return SQLite.ParseInt(cmd.ExecuteScalar());
				}
			}
			catch (Exception ex)
			{
				//LogManager.GetCurrentClassLogger().Error(ex);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, ex.ToString());
				throw ex;
			}
		}
		#endregion

		#region 次作業取得GetNextProcess
		/// <summary>
		/// 次作業取得する
		/// 次Orderを取得したい場合は使用禁止　Order.GetNextMagazineを使用すること
		/// </summary>
		/// <param name="lot"></param>
		/// <returns></returns>
		public static Process GetNextProcess(int lineCD, int currentProcessNo, AsmLot lot)
		{
			// 次Orderを取得したい場合は使用禁止　Order.GetNextMagazineを使用すること
			Process[] procs = Process.GetWorkFlow(lineCD, lot.TypeCd);

			Process next = null;
			bool found = false;

			foreach (Process p in procs)
			{
				if (found == true)
				{
					next = p;
					break;
				}

				if (p.ProcNo == currentProcessNo)
				{
					found = true;
				}
			}

			if (found == false)
			{
				throw new Exception("インライン工程マスタに情報がありません:" + lot.TypeCd + ":" + currentProcessNo.ToString()); ;
			}

			//次工程が無い場合はnullを返して終了
			if (next == null) return null;
			return next;
		}
		#endregion

		#region GetPrevProcess

		/// <summary>
		/// 前工程を取得する
		/// 前Orderを取得する場合には仕様禁止 Order.GetPrevMagazineOrderを使用すること
		/// </summary>
		/// <param name="currentProcessNo"></param>
		/// <param name="typecd"></param>
		/// <returns></returns>
		public static Process GetPrevProcess(int lineCD, int currentProcessNo, string typecd)
		{
			Process[] procs = Process.GetWorkFlow(lineCD, typecd);

			Process prev = null;
			foreach (Process p in procs)
			{
				if (p.ProcNo == currentProcessNo)
				{
					return prev;
				}
				else
				{
					prev = p;
				}
			}

			throw new Exception("インライン工程マスタに情報がありません:" + typecd + ":" + currentProcessNo.ToString()); ;
		}

		public static Process[] GetPrevProcess(int lineCD, string typecd, string procGroupcd, int currentOrder)
		{
			Process[] workflow = GetWorkFlow(lineCD, typecd, procGroupcd);
			return workflow.Where(w => w.WorkOrder < currentOrder).ToArray();
		}

        #endregion

        public static Process GetNowProcess(int lineCD, AsmLot lot, int? macno)
        {
            using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT procno FROM TnTran WITH(nolock) WHERE delfg = 0 AND lotno like @LOTNO ";
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lot.NascaLotNo + "%";

                if (macno.HasValue)
                {
                    sql += " AND macno = @MACNO ";
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno.Value;
                }
                else
                {
                    sql += " AND iscomplt = 0 ";
                }
                cmd.CommandText = sql;
                object procno = cmd.ExecuteScalar();
                if (procno == null)
                {
                    throw new Exception(string.Format("現在作業中の実績が見つかりません。 ロットNO:{0}", lot.NascaLotNo));
                }
                else
                {
                    return Process.GetProcess(lineCD, Convert.ToInt32(procno));
                }
            }
        }

        public static Process GetNowProcess(int lineCD, AsmLot lot)
        {
            return GetNowProcess(lineCD, lot, null);
        }

        public static Process GetNowProcess(int lineCD, string lotNo)
        {
            using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT procno FROM TnTran WITH(nolock) WHERE delfg = 0 AND lotno = @LOTNO AND iscomplt = 0";
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotNo;

                cmd.CommandText = sql;
                object procno = cmd.ExecuteScalar();
                if (procno == null)
                {
                    return Process.GetProcess(lineCD, Order.GetLastProcNoFromLotNo(lineCD, lotNo));
                }
                else
                {
                    return Process.GetProcess(lineCD, Convert.ToInt32(procno));
                }
            }
        }

        public static MagazineDevideStatus GetMagazineDevideStatus(int lineCD, string magazineNo, string procno)
		{
			int iprocno;

			if (int.TryParse(procno, out iprocno) == false)
			{
				return MagazineDevideStatus.Unknown;
			}
			else
			{
				return GetMagazineDevideStatus(lineCD, magazineNo, iprocno);
			}
		}

		public static MagazineDevideStatus GetMagazineDevideStatus(int lineCD, string magazineNo, int procno)
		{
			Magazine mag = Magazine.GetCurrent(lineCD, magazineNo);
			if (mag == null) return MagazineDevideStatus.Unknown;

			AsmLot lot = AsmLot.GetAsmLot(lineCD, mag.NascaLotNO);
			if (lot == null) return MagazineDevideStatus.Unknown;

			return GetMagazineDevideStatus(lineCD, lot, procno);
		}

		public static MagazineDevideStatus GetMagazineDevideStatus(int lineCD, AsmLot lot, int procno)
		{
			if (lot == null) return MagazineDevideStatus.Single;
			try
			{
				using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT
                            magdevidestatus
                        FROM
                            TmWorkFlow
                        WHERE
                            typecd =  @TYPECD AND delfg=0 AND procno=@PROCNO
                        ORDER BY workorder";

					cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = lot.TypeCd ?? "";
					cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;

					object o = cmd.ExecuteScalar();
					int? st = SQLite.ParseNullableInt(o);
					if (st == null)
					{
						return MagazineDevideStatus.Unknown;
					}
					else
					{
						try
						{
							return (MagazineDevideStatus)st;
						}
						catch (Exception)
						{
							return MagazineDevideStatus.Unknown;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//LogManager.GetCurrentClassLogger().Error(ex);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, ex.ToString());
				throw ex;
			}

		}
	}
}
