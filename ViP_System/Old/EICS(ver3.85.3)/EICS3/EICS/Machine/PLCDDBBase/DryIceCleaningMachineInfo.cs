using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
    class DryIceCleaningMachineInfo : PLCDDGBasedMachine
    {
        private const string REQUEST_PROGRAMNAME_ADDRESS = "B000500";

        private const string RESPONSE_PROGRAMNAME_ADDRESS = "W000400";

        private const string RESPONSE_COMPLETE_PROGRAMNAME_ADDRESS = "B000400";

        private const string REQUEST_PARAMERTERCHECK_ADDRESS = "B000520";

        private const string RESPONSE_RESULT_PARAMERTERCHECK_PROGRAMNO_ADDRESS = "W000420";

        private const string RESPONSE_COMPLETE_PARAMERTERCHECK_PROGRAMNO_ADDRESS = "B000420";

        private const string REQUEST_WORKCOMPLETE_ADDRESS = "B000530";

        private const string REQUEST_COMPLETE_WORKCOMPLETE_ADDRESS = "B000430";

        private const string LOADERMAGAZINENO_ADDRESS = "W000500";

        private const int MAGAZINENO_LENGTH = 10;

        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_DRYICECN;
        }

        public LSETInfo LsetInfo { get; set; }

        public DryIceCleaningMachineInfo(LSETInfo lsetInfo)
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

            if (requestProgramName())
            {
                string programName = getProgramName(lsetInfo.InlineCD, lsetInfo.ModelNM);

                responseProgramName(programName);
            }

            if (requestParameterCheck())
            {
                if (checkParameter())
                {
                    responseParameterCheckOk();
                }
                else
                {
                    responseParameterCheckNg();
                }
            }

            if (requestWorkComplete())
            {
                workComplete();
            }

            int timingNo = GetTimingNo(this.LsetInfo.ChipNM);
            EndingProcess(this.LsetInfo, timingNo);
        }

        private bool requestProgramName()
        {
            if (plc.GetBit(REQUEST_PROGRAMNAME_ADDRESS) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 品種要求確認 [アドレス:{REQUEST_PROGRAMNAME_ADDRESS}]");
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
        private void responseProgramName(string programName)
        {
            plc.SetString(RESPONSE_PROGRAMNAME_ADDRESS, programName);
            plc.SetBit(RESPONSE_COMPLETE_PROGRAMNAME_ADDRESS, 1, "1");

            outputLog($"装置 << システム プログラム名:{programName}送信 [アドレス:{RESPONSE_PROGRAMNAME_ADDRESS}]");
        }

        private bool requestParameterCheck()
        {
            if (plc.GetBit(REQUEST_PARAMERTERCHECK_ADDRESS) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム パラメータチェック要求確認 [アドレス:{REQUEST_PARAMERTERCHECK_ADDRESS}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseParameterCheckOk()
        {
            plc.SetString(RESPONSE_RESULT_PARAMERTERCHECK_PROGRAMNO_ADDRESS, "1");
            plc.SetBit(RESPONSE_COMPLETE_PARAMERTERCHECK_PROGRAMNO_ADDRESS, 1, "1");

            outputLog($"装置 << システム パラメータ照合OK送信 [アドレス:{RESPONSE_RESULT_PARAMERTERCHECK_PROGRAMNO_ADDRESS}]");
        }

        private void responseParameterCheckNg()
        {
            plc.SetString(RESPONSE_RESULT_PARAMERTERCHECK_PROGRAMNO_ADDRESS, "2");
            plc.SetBit(RESPONSE_COMPLETE_PARAMERTERCHECK_PROGRAMNO_ADDRESS, 1, "1");

            outputLog($"装置 << システム パラメータ照合NG送信 [アドレス:{RESPONSE_RESULT_PARAMERTERCHECK_PROGRAMNO_ADDRESS}]");
        }

        private bool requestWorkComplete()
        {
            if (plc.GetBit(REQUEST_WORKCOMPLETE_ADDRESS) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 完了処理要求確認 [アドレス:{REQUEST_WORKCOMPLETE_ADDRESS}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseWorkComplete()
        {
            plc.SetBit(REQUEST_COMPLETE_WORKCOMPLETE_ADDRESS, 1, "1");
            outputLog($"装置 << システム 完了処理完了 [アドレス:{REQUEST_COMPLETE_WORKCOMPLETE_ADDRESS}]");
        }

        private string getProgramName(int lineCd, string modelNm)
        {
            string magazineNo = plc.GetMagazineNo(LOADERMAGAZINENO_ADDRESS, MAGAZINENO_LENGTH);
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
        private bool checkParameter()
        {
            CreateFileProcess(this.LsetInfo, true);

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
        private void workComplete()
        {
            CreateFileProcess(this.LsetInfo, false);
            responseWorkComplete();
        }

        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, 
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }
    }
}
