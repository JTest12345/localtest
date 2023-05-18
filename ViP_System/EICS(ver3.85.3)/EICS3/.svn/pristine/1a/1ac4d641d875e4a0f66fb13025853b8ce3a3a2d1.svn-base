using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
    public class SheetRingLoader : PLCDDGBasedMachine
    {
        private const string PLC_MEMORY_ADDR_MACHINE_READY = "EM50070";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_REQ = "EM50054";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_SEND = "EM50027";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_RESULT_SEND = "EM50026";
        private const string PLC_MEMORY_ADDR_PARAMCHECK_REQ = "EM50050";
        private const string PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND = "EM50020";

        private string[] PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()
        { return new string[] { "EM51400", "EM51410", "EM51420", "EM51430", "EM51440", "EM51450", "EM51460", "EM51470" }; }

        private const int MAGAZINENO_LENGTH = 10;


        public LSETInfo LsetInfo { get; set; }

        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
        }

        public SheetRingLoader(LSETInfo lsetInfo)
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

            string errMsg;

            base.machineStatus = Constant.MachineStatus.Runtime;

            if (readyMachine() == true)
            {
                if (requestProgramName())
                {
                    string typeCd;
                    if (checkTypeBlend(lsetInfo.InlineCD, out typeCd, out errMsg))
                    {
                        string programName = getProgramName(lsetInfo.InlineCD, lsetInfo.ModelNM, typeCd);
                        responseTypeBlendCheckOKAndProgramName(programName);
                    }
                    else
                    {
                        responseTypeBlendCheckNG();
                        throw new ApplicationException(errMsg);
                    }
                }

                if (requestParameterCheck())
                {
                    this.LsetInfo.TypeCD = GetTypeFromPlcLotInfo();
                    if (checkParameter())
                    {
                        responseParameterCheckOk();
                    }
                    else
                    {
                        responseParameterCheckNg();
                    }

                    // OK, NGファイルを削除
                    List<string> startFilePathList = Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);
                    startFilePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE));
                    foreach (string filePath in startFilePathList)
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }

        private bool readyMachine()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_MACHINE_READY) == PLC.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool requestProgramName()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_PROGRAMNAME_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 品種要求確認 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// プログラム名送信 (型番混在チェックOK)
        /// </summary>
        /// <param name="programName"></param>
        private void responseTypeBlendCheckOKAndProgramName(string programName)
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_SEND, Convert.ToInt32(programName));
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_REQ, 0);

            outputLog($"装置 << システム プログラム名:{programName}送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND}]");
        }

        /// <summary>
        /// プログラム名送信 (型番混在チェックNG)
        /// </summary>
        /// <param name="programName"></param>
        private void responseTypeBlendCheckNG()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_RESULT_SEND, 1);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_REQ, 0);

            outputLog($"装置 << システム 装置停止要求(1：停止)送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND}]");
        }

        private bool requestParameterCheck()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_PARAMCHECK_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム パラメータチェック要求確認 [アドレス:{PLC_MEMORY_ADDR_PARAMCHECK_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseParameterCheckOk()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND, 0);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMCHECK_REQ, 0);

            outputLog($"装置 << システム パラメータ照合結果(0：OK)送信 [アドレス:{PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND}]");
        }

        private void responseParameterCheckNg()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND, 1);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMCHECK_REQ, 0);

            outputLog($"装置 << システム パラメータ照合(1：NG)送信 [アドレス:{PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND}]");
        }

        private bool checkTypeBlend(int lineCd, out string typecd, out string errMsg)
        {
            typecd = "";
            errMsg = "";

            // -------- マガジンNo 取得 -> アッセンロット情報取得
            for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
            {
                string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                string lotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                if (string.IsNullOrEmpty(lotNo)) continue;

                string[] lotNoArray = lotNo.Split(' ');

                if (lotNoArray.Length >= 2)
                {
                    lotNo = lotNoArray[1];
                }

                string lottypecd = ConnectDB.GetTypeCD(lineCd, lotNo);
                if (string.IsNullOrEmpty(lottypecd))
                {
                    errMsg = $"読み取み込んだロットのNASCA指図から型番情報が取得できません。[段数：[{i + 1}段],アドレス：{address},ロットNo：{lotNo}]";
                    return false;
                }

                // -------- 型番混在チェック
                if (string.IsNullOrWhiteSpace(typecd) == true)
                {
                    // 1ロット目
                    typecd = lottypecd;
                }
                else
                {
                    // 2ロット目：型番混在チェック
                    if (typecd != lottypecd)
                    {
                        errMsg = $"読込ロットの型番が他のロットと違います。[段数：[{i + 1}段],ロットNo：{lotNo},型番：{lottypecd}],[型番(他ロット)：{typecd}]";
                        return false;
                    }
                }
            }
            if (string.IsNullOrEmpty(typecd))
            {
                errMsg = "全てのトラベルシート情報からロット情報を取得できませんでした。QR読み取りに失敗した思われます。再度読み直して下さい。";
                return false;
            }

            return true;
        }

        private string getProgramName(int lineCd, string modelNm, string typecd)
        {
            string programName = Database.Plm.GetProgramName(typecd, lineCd, modelNm);

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

        private string GetTypeFromPlcLotInfo()
        {
            for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
            {
                string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                string lotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                if (string.IsNullOrEmpty(lotNo)) continue;

                string[] lotNoArray = lotNo.Split(' ');

                if (lotNoArray.Length >= 2)
                {
                    lotNo = lotNoArray[1];
                }

                return ConnectDB.GetTypeCD(this.LsetInfo.InlineCD, lotNo);
            }
            return null;
        }
        
        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }
    }
}
