using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api.ARMS
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
        /// ダイス特定CD
        /// </summary>
        public string DiceClassCd { get; set; }

        /// <summary>
        /// 調合タイプ
        /// </summary>
        public string MixTypeCd { get; set; }

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
            return SearchProcess(null, workcd, null, false);
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

            using (SqlConnection con = new SqlConnection(SQLite.ArmsConStr))
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
                    sql += "AND EXISTS(SELECT * FROM TmWorkFlow WHERE delfg = 0 AND TmWorkFlow.procno = TmProcess.procno) ";
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
                        p.AutoUpdMachineNo = (reader["AutoUpdMachineNo"] != DBNull.Value) ? (int?)SQLite.ParseInt(reader["AutoUpdMachineNo"]) : null;
                        p.DiceClassCd = SQLite.ParseString(reader["diceclasscd"]);
                        p.IsNascaDefect = SQLite.ParseBool(reader["nascadefectfg"]);

                        p.MixTypeCd = reader["mixtypecd"].ToString().Trim();

                        retv.Add(p);
                    }
                }
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

            using (SqlConnection con = new SqlConnection(SQLite.ArmsConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"
                    SELECT
                        procno, workorder
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

                        retlist.Add(retv);
                    }
                }
            }

            return retlist.ToArray();


        }
        public static Process[] GetWorkFlow(string typecd)
        {
            return GetWorkFlow(typecd, string.Empty, false);
        }

        #endregion

    }
}
