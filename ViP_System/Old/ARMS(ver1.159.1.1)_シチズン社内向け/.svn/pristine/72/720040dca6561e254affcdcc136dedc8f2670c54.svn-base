using ArmsApi.Model.NASCA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class Process
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

        /// <summary>
        /// 工程のダミー登録
        /// </summary>
        public enum AutoDummyTran : int
        {
            TimeLimitChkWorkEnd = 1,
            DummyTran = 2,
            TimeLimitTgtWorkStart = 3,
            TimeLimitTgtWorkEnd = 4,
            Unknown = 99,
        }

        public override string ToString()
        {
            return this.InlineProNM;
        }

        private const int DISCHARGE_PROC_NO = -1;

		//public const string ECK_WORK_CD = "MD0081";

        #region プロパティ

        /// <summary>
        /// 工程グループCd(追加)
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
        /// ダミー登録用設備CD
        /// </summary>
        public int? AutoUpdMachineNo { get; set; }

        /// <summary>
        /// 作業順(追加)
        /// </summary>
        public int WorkOrder { get; set; }

		/// <summary>
		/// Nasca不良の登録が必要な工程
		/// </summary>
		public bool IsNascaDefect { get; set; }

		/// <summary>
		/// チップ特定CD
		/// </summary>
		public string DiceClassCd { get; set; }

        /// <summary>
        /// 調合タイプ
        /// </summary>
        public List<string> MixTypeCd { get; set; } = new List<string>();

        /// <summary>
		/// ダミー実績登録区分
		/// </summary>
		public AutoDummyTran AutoDummyTranKb { get; set; }

        /// <summary>
        /// カットブレンド
        /// </summary>
        public bool IsCutBlend { get; set; }

        /// <summary>
        /// ロットマーキング
        /// </summary>
        public bool IsLotMarking { get; set; }

        #endregion

        #region インライン工程マスタ情報取得

        /// <summary>
        /// ダイボンド以外の工程で使用可能
        /// </summary>
        /// <param name="workcd"></param>
        /// <returns></returns>
        public static Process GetProcess(string workcd)
        {
            return GetProcess(null, workcd, null);
        }

		public static Process[] GetProcessFromWorkCd(string workcd)
		{
			return  SearchProcess(null, workcd, null, false);
		}

        public static Process GetProcess(int procNo)
        {
            return GetProcess(procNo, null, null);
        }

        public static Process GetProcess(string workcd, string diceclasscd)
        {
            return GetProcess(null, workcd, diceclasscd);
        }

        /// <summary>
        /// procnoまたはworkcdをキーにProcessを取得
        /// 条件一致無しの場合はNull、複数結果の場合は先頭を返す
        /// </summary>
        /// <param name="procNo"></param>
        /// <param name="workcd"></param>
        /// <returns></returns>
        private static Process GetProcess(int? procNo, string workcd, string diceclasscd)
        {
            Process[] proclist = SearchProcess(procNo, workcd, diceclasscd, false);

            if (proclist.Length == 0)
            {
                return null;
            }
            else
            {
                return proclist[0];
            }
        }

        public static Process[] GetProcessList(string workcd)
        {
            return GetProcessList(null, workcd, null);
        }

        public static Process[] GetProcessList(int? procNo, string workcd, string diceclasscd)
        {
            return SearchProcess(null, workcd, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="workcd">Null許可</param>
        /// <returns></returns>
        public static Process[] SearchProcess(int? procNo, string workcd, string diceclasscd, bool isWorkflow)
        {
            List<Process> retv = new List<Process>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @"
                       SELECT
                        procgroupcd
                        , TmProcess.procno
                        , procnm
                        , workcd
                        , finalst
                        , firstst
                        , samplingfg
                        , autoupdmachineno
						, diceclasscd
						, nascadefectfg
						, mixtypecd
                        , cutblendfg
                        , lotmarkingfg
                        FROM TmProcess
                        WHERE TmProcess.delfg=0 ";

                    if (procNo.HasValue)
                    {
                        sql += " AND TmProcess.procno = @PROCNO";
                        cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;
                    }

                    if (workcd != null)
                    {
                        sql += " AND TmProcess.workcd = @WORKCD";
                        cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = workcd;
                    }

                    if (!string.IsNullOrEmpty(diceclasscd))
                    {
                        sql += " AND TmProcess.diceclasscd = @DICECLASSCD";
                        cmd.Parameters.Add("@DICECLASSCD", SqlDbType.NVarChar).Value = diceclasscd;
                    }

                    if (isWorkflow)
                    {
                        sql += " and exists(select * from TmWorkFlow where delfg = 0 and TmWorkFlow.procno = TmProcess.procno)";
                    }

                    sql += " ORDER BY procno";

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
                            p.AutoUpdMachineNo = (reader["AutoUpdMachineNo"] != DBNull.Value) ? (int?)SQLite.ParseInt(reader["AutoUpdMachineNo"]) : null;
							p.DiceClassCd = SQLite.ParseString(reader["diceclasscd"]);
							p.IsNascaDefect = SQLite.ParseBool(reader["nascadefectfg"]);

                            string mixTypeCd = reader["mixtypecd"].ToString().Trim();
                            if (string.IsNullOrEmpty(mixTypeCd) == false)
                            { 
                                p.MixTypeCd.AddRange(mixTypeCd.Split(','));
                            }

                            p.IsCutBlend = SQLite.ParseBool(reader["cutblendfg"]);
                            p.IsLotMarking = SQLite.ParseBool(reader["lotmarkingfg"]);

                            retv.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //LogManager.GetCurrentClassLogger().Error(ex);
                Log.SysLog.Error(ex);
                throw ex;
            }

            return retv.ToArray();
        }

        /// <summary>
        /// ワークフローに登録されているリスト取得
        /// </summary>
        /// <returns></returns>
        public static Process[] GetWorkflowProcess()
        {
            return SearchProcess(null, null, null, true);
        }

        #endregion

        #region ワークフロー取得 GetWorkFlow
        public static Process[] GetWorkFlow(string typecd, string procGroupcd, bool includeDelete)
        {
            List<Process> retlist = new List<Process>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

					string sql = @"
                        SELECT
                            procno, workorder, autodummytran
                        FROM
                            TmWorkFlow WITH(nolock)
                        WHERE 1=1 AND typecd = @TYPECD ";

					if (includeDelete == false) 
					{
						sql += " AND delfg = 0 ";
					}

					sql += " ORDER BY workorder";

                    cmd.CommandText = sql.Replace("\r\n", "");
                    cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = typecd ?? "";

                    Process[] wfProcArray = Process.GetWorkflowProcess();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int procno = SQLite.ParseInt(reader["procno"]);
                            Process retv = wfProcArray.Where(wf => wf.ProcNo == procno).FirstOrDefault();
                            if (retv == null)
                            {
                                retv = Process.GetProcess(procno);
                            }

                            if (procGroupcd != string.Empty && procGroupcd != retv.ProcGroupCd)
                            {
                                //同工程のみ取得
                                continue;
                            }
                            retv.WorkOrder = SQLite.ParseInt(reader["workorder"]);

                            int? kb = SQLite.ParseNullableInt(reader["autodummytran"]);
                            if (kb == null)
                            {
                                retv.AutoDummyTranKb = AutoDummyTran.Unknown;
                            }
                            else
                            {
                                try
                                {
                                    retv.AutoDummyTranKb = (AutoDummyTran)kb;
                                }
                                catch (Exception)
                                {
                                    retv.AutoDummyTranKb = AutoDummyTran.Unknown;
                                }
                            }

                            retlist.Add(retv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                //LogManager.GetCurrentClassLogger().Error(ex);
                throw ex;
            }

            return retlist.ToArray();


        }
        public static Process[] GetWorkFlow(string typecd)
        {
            return GetWorkFlow(typecd, string.Empty, false);
        }
        #endregion

        #region ワークフロー　指図発行必要値取得 GetOrderMove
        public static int GetOrderMove(string typecd, int procno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
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
                Log.SysLog.Error(ex);
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
        public static Process GetNextProcess(int currentProcessNo, AsmLot lot)
        {
            // 次Orderを取得したい場合は使用禁止　Order.GetNextMagazineを使用すること
            Process[] procs = Process.GetWorkFlow(lot.TypeCd);

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
                throw new ArmsException("インライン工程マスタに情報がありません:" + lot.TypeCd + ":" + currentProcessNo.ToString()); ;
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
        public static Process GetPrevProcess(int currentProcessNo, string typecd)
        {
            Process[] procs = Process.GetWorkFlow(typecd);

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

            throw new ArmsException("インライン工程マスタに情報がありません:" + typecd + ":" + currentProcessNo.ToString()); ;
        }

        public static Process[] GetPrevProcess(string typecd, string procGroupcd, int currentOrder)
        {
            Process[] workflow = GetWorkFlow(typecd, procGroupcd, false);
            return workflow.Where(w => w.WorkOrder < currentOrder).ToArray();
        }

        #endregion

        public static Process GetNowProcess(AsmLot lot, int? macno) 
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
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
                    return Process.GetProcess(Convert.ToInt32(procno));
                }
            }
        }

        public static Process GetNowProcess(AsmLot lot)
        {
            return GetNowProcess(lot, null);
        }

        public static Process GetNowProcess(string lotNo)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT procno FROM TnTran WITH(nolock) WHERE delfg = 0 AND lotno = @LOTNO AND iscomplt = 0";
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotNo;

                cmd.CommandText = sql;
                object procno = cmd.ExecuteScalar();
                if (procno == null)
                {
                    return Process.GetProcess(Order.GetLastProcNoFromLotNo(lotNo));
                }
                else
                {
                    return Process.GetProcess(Convert.ToInt32(procno));
                }
            }
        }


        public static Process GetFirstProcess(string typecd) 
		{
			Process[] workflow = GetWorkFlow(typecd);
			if (workflow.Count() == 0)
			{
				throw new Exception(
					string.Format("作業順マスタが存在しない為、第一作業の取得に失敗しました。型番:{0}", typecd));
			}
			return workflow.OrderBy(w => w.WorkOrder).First();
		}

        /// <summary>
        /// 作業順の最終作業か調べる
        /// </summary>
        /// <param name="procNo"></param>
        /// <param name="typecd"></param>
        /// <returns></returns>
        public static bool IsLastProcess(Process proc, string typecd) 
        {
            Process[] workflows = GetWorkFlow(typecd);
            if (workflows.Max(w => w.WorkOrder) == proc.WorkOrder)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

		/// <summary>
		/// 作業順の最終作業を取得
		/// </summary>
		/// <returns></returns>
		public static Process GetLastProcess(string procgroupcd, string typecd, bool includeDelete)
		{
			Process[] wfs = GetWorkFlow(typecd, procgroupcd, includeDelete);
			if (wfs.Count() == 0)
			{
				throw new Exception(
					string.Format("[GetLastProcess] 作業順マスタが見つかりません。TypeCd:{0} ProcGroupCd:{1}", typecd, procgroupcd));
			}

			int lastOrder = wfs.Max(w => w.WorkOrder);
			wfs = wfs.Where(w => w.WorkOrder == lastOrder).ToArray();

			return wfs.Single();
		}
		public static Process GetLastProcess(string typecd) 
		{
			return GetLastProcess(string.Empty, typecd, false); 
		}

        /// <summary>
        /// 型番毎の最終作業番号を取得
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetFinalProcNoDictionary()
        {
            Dictionary<string, int> retv = new Dictionary<string, int>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT typecd, procno FROM TmWORKFLOW TEMP WITH(NOLOCK)
                    WHERE delfg = 0
                    AND workorder = (SELECT MAX(workorder) FROM TmWORKFLOW WHERE typecd = TEMP.typecd AND delfg = 0) ";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(SQLite.ParseString(rd["typecd"]), SQLite.ParseInt(rd["procno"]));
                    }
                }
            }

            return retv;
        }

        public static MagazineDevideStatus GetMagazineDevideStatus(string magazineNo, string procno)
        {
            int iprocno;

            if (int.TryParse(procno, out iprocno) == false)
            {
                return MagazineDevideStatus.Unknown;
            }
            else
            {
                return GetMagazineDevideStatus(magazineNo, iprocno);
            }
        }

        public static MagazineDevideStatus GetMagazineDevideStatus(string magazineNo, int procno)
        {
            Magazine mag = Magazine.GetCurrent(magazineNo);
            if (mag == null) return MagazineDevideStatus.Unknown;

            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null) return MagazineDevideStatus.Unknown;

            return GetMagazineDevideStatus(lot, procno);
        }

        public static MagazineDevideStatus GetMagazineDevideStatus(AsmLot lot, int procno)
        {
            if (lot == null) return MagazineDevideStatus.Single;
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
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
                Log.SysLog.Error(ex);
                throw ex;
            }

        }

        public static AutoDummyTran GetAutoDummyTran(string typecd, int procno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                            autodummytran
                        FROM
                            TmWorkFlow
                        WHERE
                            typecd =  @TYPECD AND delfg=0 AND procno=@PROCNO
                        ORDER BY workorder";

                    cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = typecd ?? "";
                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;

                    object o = cmd.ExecuteScalar();
                    int? st = SQLite.ParseNullableInt(o);
                    if (st == null)
                    {
                        return AutoDummyTran.Unknown;
                    }
                    else
                    {
                        try
                        {
                            return (AutoDummyTran)st;
                        }
                        catch (Exception)
                        {
                            return AutoDummyTran.Unknown;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //LogManager.GetCurrentClassLogger().Error(ex);
                Log.SysLog.Error(ex);
                throw ex;
            }

        }

        public static string[] GetWorkFlowTypeCd() 
		{
			List<string> retv = new List<string>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.CommandText = @"
                    SELECT DISTINCT typecd FROM TmWorkFlow WITH(nolock) ";

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv.Add(SQLite.ParseString(rd["typecd"]));
					}
				}
			}

			return retv.ToArray();
		}

		public static int[] GetWorkFlowProcNo()
		{
			List<int> retv = new List<int>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.CommandText = @"
                    SELECT DISTINCT procno FROM TmWorkFlow WITH(nolock) WHERE delfg = 0";

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv.Add(SQLite.ParseInt(rd["procno"]));
					}
				}
			}

			return retv.ToArray();
		}
        
		/// <summary>
		/// マガジン交換作業か確認
		/// </summary>
		/// <param name="workCd"></param>
		/// <returns></returns>
		public static bool HasMagazineChangeWork(string workCd) 
		{
			if (workCd == "MD0103")
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// マガジン交換前待機作業か確認
		/// </summary>
		/// <param name="workCd"></param>
		/// <returns></returns>
		public static bool HasMagazineChangeBufferWork(string workCd)
		{
			if (workCd == "BUFFER")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
        
        /// <summary>
        /// 作業順に指定した作業が含まれているか確認
        /// </summary>
        /// <param name="workCd"></param>
        /// <param name="typeCd"></param>
        /// <returns></returns>
        public static bool HasWorkflowExists(string workCd, string typeCd)
		{
			List<Process> wfList = Process.GetWorkFlow(typeCd).ToList();
			if (wfList.Count == 0) 
			{
				return false;
			}

			if (wfList.Exists(w => w.WorkCd == workCd)) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// 作業順に指定した作業が含まれているか確認
        /// </summary>
        /// <param name="workCd"></param>
        /// <param name="typeCd"></param>
        /// <returns></returns>
        public static bool HasWorkflowExists(int procNo, string typeCd)
        {
            List<Process> wfList = Process.GetWorkFlow(typeCd).ToList();
            if (wfList.Count == 0)
            {
                return false;
            }

            if (wfList.Exists(w => w.ProcNo == procNo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 指定工程間でフレームの搭載位置をマガジン内で上下に逆転させるかのチェック
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="targetProcNo">先工程</param>
        /// <param name="fromProcNo">元工程</param>
        /// <returns></returns>
        public static bool IsReverseFramePlacement(string lotNo, int toProcNo, int currentProcNo)
        {
            var orders = Order.GetOrder(lotNo).OrderBy(o => o.WorkStartDt);

            //<int,int> = <proc,反転フラグ>
            Dictionary<int, int> frameDeployStates = new Dictionary<int, int>();

            //反転フラグ 1=正方向 -1=逆方向
            int deployState = 1;
            foreach (var o in orders)
            {
                if (frameDeployStates.Keys.Where(f => f == o.ProcNo).Count() != 0)
                    continue;

                MachineInfo m = MachineInfo.GetMachine(o.MacNo);

                if (m != null && m.RevDeployFg)
                {
                    deployState = -deployState;// 上下状態反転
                }
                
                frameDeployStates.Add(o.ProcNo, deployState);
            }

            if (frameDeployStates.Count == 0)
            {
                //作業が存在しない
                return false;
            }

            if (frameDeployStates.ContainsKey(toProcNo) == false)
            {
                throw new ApplicationException("作業実績が存在しません 工程No:" + toProcNo);
            }
            
            if (frameDeployStates.ContainsKey(currentProcNo) == false)
            {
                throw new ApplicationException("作業実績が存在しません 工程No:" + currentProcNo);
            }

            // 確認先の工程の反転状態(-1or1)と自工程のマップデータ参照時(実際は自工程の一つ手前）での反転状態(-1or1)を比較
            // それぞれの反転状態を足して0になるなら、状態が食い違っているので反転の必要有の判断
            if (frameDeployStates[toProcNo] + frameDeployStates[currentProcNo] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public static int? GetFirstMappingProc(string typecd)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(SQLite.ConStr))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT
                            procno
                        FROM
                            TmWorkFlow
                        WHERE
                            typecd =  @TYPECD AND delfg=0 AND mappingfg=1
                        ORDER BY workorder";

					cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = typecd;

					object o = cmd.ExecuteScalar();
					if (o != null)
					{
                        return Convert.ToInt32(o);
                    }
                    else
                    {
                        return null;
                    }
				}
			}
			catch (Exception ex)
			{
				Log.SysLog.Error(ex);
				//LogManager.GetCurrentClassLogger().Error(ex);
				throw ex;
			}
		}

        /// <summary>
        /// その作業で樹脂Gが投入禁止か否かを判定
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="resingroupcd"></param>
        /// <returns></returns>
        public static bool IsDenyProcResin(int procno, string resingroupcd)
        {
            List<string> retv = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT resingpcd
									FROM TmDenyProcResin WITH(nolock) 
									WHERE delfg = 0 AND procno = @PROCNO AND resingpcd = @RESINGROUPCD";

                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;
                    cmd.Parameters.Add("@RESINGROUPCD", SqlDbType.NVarChar).Value = resingroupcd;

                    cmd.CommandText = sql;
                    object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 次作業(MD樹脂を使用する)取得GetNextProcessUsingResin
        /// <summary>
        /// 次作業取得する
        /// 次Orderを取得したい場合は使用禁止　Order.GetNextMagazineを使用すること
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public static Process GetNextResinProcess(int currentProcessNo, AsmLot lot)
        {
            // 次Orderを取得したい場合は使用禁止　Order.GetNextMagazineを使用すること
            Process[] procs = Process.GetWorkFlow(lot.TypeCd);

            Process next = null;
            bool found = false;

            foreach (Process p in procs)
            {
                if (found == true)
                {
                    if (p.MixTypeCd.Any() == true)
                    //if (string.IsNullOrWhiteSpace(p.MixTypeCd) == false)
                    {
                        next = p;
                        break;
                    }
                }

                if (p.ProcNo == currentProcessNo)
                {
                    found = true;
                }
            }

            if (found == false)
            {
                throw new ArmsException("ARMS作業順マスタに情報がありません:" + lot.TypeCd + ":" + currentProcessNo.ToString()); ;
            }

            //次工程が無い場合はnullを返して終了
            if (next == null) return null;
            return next;
        }
        #endregion

        public static string[] GetWorkFlowTypeList()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT DISTINCT typecd FROM TmWorkFlow WITH(nolock) WHERE delfg = 0 ";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(SQLite.ParseString(rd["typecd"]));
                    }
                }
            }

            return retv.ToArray();
        }
        
        /// <summary>
        /// 最終ステータス後の工程か否かを判定
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="typecd"></param>
        /// <returns></returns>
        public static bool IsFinalStAfterProcess(Process proc, string typecd)
        {
            Process[] workflows = GetWorkFlow(typecd);
            Process final = workflows.Where(p => p.FinalSt == true).FirstOrDefault();

            if (final.WorkOrder < proc.WorkOrder)
                return true;
            else
                return false;
        }

        public static Process GetNowProcessFromNasca(string lotno, List<string> targetWorkList)
        {
            string workcd = null;
            List<NascaTranData> tranList = Importer.GetLotTranData(lotno);
            foreach (NascaTranData tran in tranList)
            {
                if (targetWorkList.Contains(tran.WorkCd) == false)
                {
                    continue;
                }
                if (tran.StartDt.HasValue == false)
                {
                    workcd = tran.WorkCd;
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(workcd))
            {
                throw new ApplicationException($"NASCA実績から自工程の作業CDの取得に失敗しました。[ロットNo:『{lotno}』]");
            }
            Process p = Process.GetProcess(workcd);
            if (p == null)
            {
                throw new ApplicationException($"自工程の作業CDがTmProcessに登録されていません。[作業CD：『{workcd}』]");
            }

            return p;
        }
    }
}
