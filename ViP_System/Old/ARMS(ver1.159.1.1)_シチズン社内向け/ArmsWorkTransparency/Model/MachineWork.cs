using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using System.Data;

namespace ArmsWorkTransparency.Model
{
    public class MachineWork
    {
        /// <summary>
        /// ロットの現在工程～受け取り工程までの作業時間を取得
        /// </summary>
        /// <param name="currentProc"></param>
        /// <param name="nextProc"></param>
        /// <param name="typeGroupCd"></param>
        /// <param name="typeCd"></param>
        /// <param name="isWorkComplete"></param>
        /// <returns></returns>
        //public static int GetWorkTime(Process currentProc, Process nextProc, string typeGroupCd, string typeCd, bool isWorkComplete)
        public static int? GetWorkTime(Process currentProc, Process nextProc, List<string> typeGroupCd, string typeCd)
        {
            int retv = 0;
            
            Process[] workList = Process.GetWorkFlow(typeCd);

            Process currentWork = workList.Where(w => w.ProcNo == currentProc.ProcNo).SingleOrDefault();
            Process nextWork = workList.Where(w => w.ProcNo == nextProc.ProcNo).SingleOrDefault();
            if (nextWork == null)
                return null;

            IEnumerable<Process> targetWorkList = workList.Where(w => w.WorkOrder >= currentWork.WorkOrder && w.WorkOrder < nextWork.WorkOrder);
            if (targetWorkList.Count() == 0)
                return null;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                SqlParameter paramProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.NVarChar);
                foreach (Process targetWork in targetWorkList)
                {
                    string sql = @" SELECT Work_TM FROM TmMachineWork WITH(nolock) 
                                    WHERE Del_FG = 0 AND Proc_NO = @PROCNO AND TypeGroup_CD in ('" + string.Join("','", typeGroupCd) + "') ";

                    paramProcNo.Value = targetWork.ProcNo;

                    cmd.CommandText = sql;
                    object workTm = cmd.ExecuteScalar();
                    if (workTm == null)
                    {
                        // どの品種でも共通設定の場合、まとめ型番ではなく共通文字で検索
                        sql = @" SELECT Work_TM FROM TmMachineWork WITH(nolock) 
                                    WHERE Del_FG = 0 AND Proc_NO = @PROCNO AND TypeGroup_CD = '共通' ";

                        cmd.CommandText = sql;
                        workTm = cmd.ExecuteScalar();
                        if (workTm != null)
                        {
                            retv += Convert.ToInt32(workTm);
                        }
                    }
                    else
                    {
                        retv += Convert.ToInt32(workTm);
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// 装置の作業時間を取得
        /// </summary>
        /// <param name="TypeGroupCd"></param>
        /// <param name="ModelNm"></param>
        /// <returns></returns>
        public static int GetWorkTime(string TypeGroupCd, string ModelNm)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT Work_TM FROM TmMachineWork WITH(nolock) 
                                WHERE Del_FG = 0 AND Model_NM = @MODELNM AND TypeGroup_CD = @TYPEGROUPCD ";

                cmd.Parameters.Add("@MODELNM", SqlDbType.NVarChar).Value = ModelNm;
                cmd.Parameters.Add("@TYPEGROUPCD", SqlDbType.NVarChar).Value = TypeGroupCd;

                cmd.CommandText = sql;
                object workTm = cmd.ExecuteScalar();

                if (workTm == null)
                {
                    return 0;
                    //throw new ApplicationException($"装置作業マスタに作業時間の設定がされていません。まとめ型番:{ TypeGroupCd } 型式:{ ModelNm }");
                }
                else
                {
                    return Convert.ToInt32(workTm);
                }
            }
        }
    }
}
