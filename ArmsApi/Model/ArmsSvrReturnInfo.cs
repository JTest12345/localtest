using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{

    #region 回答ST

    public enum ResStatus
    {
        /// <summary>0:正常</summary>
        OK = 0,
        /// <summary>1:異常</summary>
        NG = 1,
        /// <summary>2:異常(リトライ)
        RETRY = 2,
    }
    #endregion
    
    public class ArmsSvrReturnInfo
    {
        #region プロパティ


        /// <summary>
        /// 回答ステータス
        /// </summary>
        public int StatusKB { get; set; }

        /// <summary>
        /// 工程ID
        /// </summary>
        public int ProcNo { get; set; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 次投入可能設備
        /// </summary>
        public MachineInfo[] NextMachines { get; set; }

        /// <summary>
        /// NASCAロット
        /// </summary>
        public string NascaLotNo { get; set; }

        #endregion


        #region GetReturnData
        /// <summary>
        /// (共通)戻り
        /// </summary>
        public static ArmsSvrReturnInfo GetReturnData(ResStatus status, string msg, int proId)
        {
            return GetReturnData(status, msg, "", proId);
        }

        public static ArmsSvrReturnInfo GetReturnData(ResStatus status, string msg, string nascalotNo, int proId)
        {
            ArmsSvrReturnInfo retInfo = new ArmsSvrReturnInfo();

            retInfo.StatusKB = (int)status;
            retInfo.NascaLotNo = nascalotNo;
            retInfo.Message = msg;
            retInfo.ProcNo = proId;

            return retInfo;
        }
        #endregion

        #region setNgReturnData
        public static ArmsSvrReturnInfo GetNgReturnData(string msg, string nascaLotNo, int processId)
        {
            if (!string.IsNullOrEmpty(nascaLotNo))
            {
                //NGログ保存
                AsmLot.InsertLotLog(nascaLotNo, DateTime.Now, msg, 0, string.Empty, false, "", string.Empty);
            }

            ArmsSvrReturnInfo retInfo = new ArmsSvrReturnInfo();

            retInfo.StatusKB = (int)ResStatus.NG;
            retInfo.Message = msg;
            retInfo.ProcNo = processId;
            retInfo.NascaLotNo = nascaLotNo;

            return retInfo;
        }
        #endregion
    }
}
