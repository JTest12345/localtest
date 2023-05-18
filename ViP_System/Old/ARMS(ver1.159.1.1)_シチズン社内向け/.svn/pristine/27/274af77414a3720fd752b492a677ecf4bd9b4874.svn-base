using ArmsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge
{
    /// <summary>
    /// 流動規制
    /// </summary>
    public class Restrict
    {
        /// <summary>
        /// 規制解除
        /// </summary>
        public static void CancelRestriction()
        {
            try
            {
                if (Config.Settings.CancelRestrictDieShearFunctionId.HasValue)
                {
                    // ダイシェア試験対象ロットで、Pstesterの実績がNG以外になっていれば規制解除
                    List<string> lotList = ArmsApi.Model.Restrict.GetDieshearRestrictLot();
                    foreach (string lot in lotList)
                    {
                        if (ArmsApi.Model.PSTESTER.WorkResult.IsResultPassing(lot, Config.Settings.CancelRestrictDieShearFunctionId.Value))
                        { 
                            ArmsApi.Model.Restrict.Cancel(lot, ArmsApi.Model.Restrict.RESTRICT_REASON_DIESHEARSAMPLE);

                            Log.SysLog.Info(string.Format("[流動規制解除] LotNo:{0} 規制内容:{1} 解除理由:{2}",
                                lot, ArmsApi.Model.Restrict.RESTRICT_REASON_DIESHEARSAMPLE, "ダイシェア測定結果で合格判定された為"));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge] CancelRestriction Error:" + err.ToString());
            }
        }   
    }
}
