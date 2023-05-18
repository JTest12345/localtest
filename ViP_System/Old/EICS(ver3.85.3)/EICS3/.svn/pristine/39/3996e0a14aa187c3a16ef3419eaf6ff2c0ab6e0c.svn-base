using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
    public class WaterCleaning : PLCDDGBasedMachine
    {
        #region PLCアドレス
        private string[] RequestProgramnameAddressList
        {
            get { return new string[] { "B000AA0", "B000BA0", "B000CA0", "B000DA0", "B000EA0" }; }
        }

        private string[] ResponseProgramnameAddressList
        {
            get { return new string[] { "W000AA0", "W000BA0", "W000CA0", "W000DA0", "W000EA0" }; }
        }

        private string[] ResponseCompleteProgramnameAddressList
        {
            get { return new string[] { "B000A10", "B000B10", "B000C10", "B000D10", "B000E10" }; }
        }

        private string[] RequestParameterCheckAddressList
        {
            get { return new string[] { "B000AB0", "B000BB0", "B000CB0", "B000DB0", "B000EB0" }; }
        }

        private string[] ResponseResultParamerterCheckAddressList
        {
            get { return new string[] { "W000AB0", "W000BB0", "W000CB0", "W000DB0", "W000EB0" }; }
        }

        private string[] ResponseCompleteParamerterCheckAddressList
        {
            get { return new string[] { "B000A20", "B000B20", "B000C20", "B000D20", "B000E20" }; }
        }

        private string[] LoaderMagazineNoAddressList
        {
            get { return new string[] { "W000A20", "W000B20", "W000C20", "W000D20", "W000E20" }; }
        }

        private string[] RequestWorkCompleteAddressList
        {
            get { return new string[] { "B000AD0", "B000BD0", "B000CD0", "B000DD0", "B000ED0" }; }
        }

        private string[] ResponseCompleteWorkCompleteAddressList
        {
            get { return new string[] { "B000A40", "B000B40", "B000C40", "B000D40", "B000E40" }; }
        }

        #endregion

        private const int MAGAZINENO_LENGTH = 10;

        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_WATERCLEANING;
        }

        public LSETInfo LsetInfo { get; set; }

        public WaterCleaning(LSETInfo lsetInfo)
        {
            this.LsetInfo = lsetInfo;

            // 設定ファイルPLC設定を基ににPLCインスタンス初期化
            InitPLC(lsetInfo);
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {
            // 設定ファイル読み出し
            InitPropAtLoop(lsetInfo);
            this.LsetInfo = lsetInfo;

            base.machineStatus = Constant.MachineStatus.Runtime;

            int tankNo = 0;
            if (int.TryParse(this.LsetInfo.EquipPartID, out tankNo) == false)
            {
                throw new ApplicationException("槽番号の設定がデータベースマスタで設定されていない為、監視できません。設定場所:QCIL.TmLSET.EquipPart_ID");
            }
            // TODO 6(アドレスの配列数)槽以上が取得できた時のエラー処理を追加する
            
            // TODO 0の場合のエラー処理追加


            if (requestProgramName(tankNo))
            {
                string programName = getProgramName(lsetInfo.InlineCD, lsetInfo.ModelNM, tankNo);

                responseProgramName(programName, tankNo);
            }

            if (requestParameterCheck(tankNo))
            {
                if (checkParameter(tankNo))
                {
                    responseParameterCheckOk(tankNo);
                }
                else
                {
                    responseParameterCheckNg(tankNo);
                }
            }

            if (requestWorkComplete(tankNo))
            {
                workComplete(tankNo);
            }

            int timingNo = GetTimingNo(this.LsetInfo.ChipNM);
            EndingProcess(this.LsetInfo, timingNo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool requestProgramName(int tankNo)
        {

            if (plc.GetBit(RequestProgramnameAddressList[tankNo-1]) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 品種要求確認 [アドレス:{RequestProgramnameAddressList[tankNo-1]}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// プログラム名送信
        /// </summary>
        /// <param name="programName"></param>
        private void responseProgramName(string programName, int tankNo)
        {
            plc.SetString(ResponseProgramnameAddressList[tankNo-1], programName);
            plc.SetBit(ResponseCompleteProgramnameAddressList[tankNo-1], 1, "1");

            outputLog($"装置 << システム プログラム名:{programName}送信 [アドレス:{ResponseProgramnameAddressList[tankNo-1]}]");
        }

        private bool requestParameterCheck(int tankNo)
        {
            if (plc.GetBit(RequestParameterCheckAddressList[tankNo-1]) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム パラメータチェック要求確認 [アドレス:{RequestParameterCheckAddressList[tankNo-1]}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseParameterCheckOk(int tankNo)
        {
            plc.SetString(ResponseResultParamerterCheckAddressList[tankNo-1], "1");
            plc.SetBit(ResponseCompleteParamerterCheckAddressList[tankNo-1], 1, "1");

            outputLog($"装置 << システム パラメータ照合OK送信 [アドレス:{ResponseResultParamerterCheckAddressList[tankNo-1]}]");
        }

        private void responseParameterCheckNg(int tankNo)
        {
            plc.SetString(ResponseResultParamerterCheckAddressList[tankNo-1], "2");
            plc.SetBit(ResponseCompleteParamerterCheckAddressList[tankNo-1], 1, "1");

            outputLog($"装置 << システム パラメータ照合NG送信 [アドレス:{ResponseResultParamerterCheckAddressList[tankNo-1]}]");
        }

        private bool requestWorkComplete(int tankNo)
        {
            if (plc.GetBit(RequestWorkCompleteAddressList[tankNo-1]) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 完了処理要求確認 [アドレス:{RequestWorkCompleteAddressList[tankNo-1]}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseWorkComplete(int tankNo)
        {
            plc.SetBit(ResponseCompleteWorkCompleteAddressList[tankNo-1], 1, "1");
            outputLog($"装置 << システム 完了処理完了 [アドレス:{ResponseCompleteWorkCompleteAddressList[tankNo-1]}]");
        }

        private string getProgramName(int lineCd, string modelNm, int tankNo)
        {
            string magazineNo = plc.GetMagazineNo(LoaderMagazineNoAddressList[tankNo-1], MAGAZINENO_LENGTH);
            if (string.IsNullOrEmpty(magazineNo))
            {
                throw new ApplicationException("ローダーマガジンのQR読み取りに失敗しました。再度読み直して下さい。");
            }

            Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(lineCd, magazineNo);

            string programName = Database.Plm.GetProgramName(lot.TypeCd, lineCd, modelNm);

            return programName;
        }

        /// <summary>
        /// パラメータチェック
        /// </summary>
        /// <returns></returns>
        private bool checkParameter(int tankNo)
        {
            CreateFileProcess(this.LsetInfo, true, null, tankNo.ToString());

            List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
            StartingProcess(this.LsetInfo, out errMessageList);

            if (errMessageList.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 完了処理
        /// </summary>
        private void workComplete(int tankNo)
        {
            CreateFileProcess(this.LsetInfo, false, null, tankNo.ToString());
            //responseWorkComplete(tankNo);
        }

        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }
    }
}
