using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using EICS.Database;
using EICS.Structure;
using System.Transactions;
using System.Threading;
using System.Diagnostics;

namespace EICS.Machine
{
    class ColorAutoMeasurerMachineInfo : PLCDDGBasedMachine
    {
        #region PLCアドレス定義

        private const string PLC_MEMORY_ADDR_PROGRAMNAME_REQ = "EM50054";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_SEND = "EM50027";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_RESULT_SEND = "EM50026";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE = "EM50023";

        private const string PLC_MEMORY_ADDR_PARAMCHECK_REQ = "EM50050";
        private const string PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND = "EM50020";

        private const string PLC_MEMORY_ADDR_COMPLETE_REQ = "EM50053";
        private const string PLC_MEMORY_ADDR_COMPLETE_RESULT_SEND = "EM50024";

        private string[] PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()
        { return new string[] { "EM51000", "EM51010", "EM51020", "EM51030", "EM51040", "EM51050", "EM51060", "EM51070" }; }

        private const int MAGAZINENO_LENGTH = 10;

        private const string PLC_MEMORY_ADDR_ENTEREDTRAYNO = "EM60020";
        private const string PLC_MEMORY_ADDR_POSITIONSHEETNO = "EM60030";
        private const string PLC_MEMORY_ADDR_MEASUREDT = "EM60090";

        private List<string> PLC_MEMORY_ADDR_SHEETDATASTART { get {
                return new List<string> { "EM60000", "EM60200", "EM60400", "EM60600", "EM60800", "EM61000", "EM61200", "EM61400", "EM61600", "EM61800", "EM62000", "EM62200" }; }}

        #endregion

        public enum WorkCompleteType
        {
            Sheet, 
            Tray
        }

        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }

        private string getTrayNoAddress(string sheetDataStartAddress)
        {
            return sheetDataStartAddress.Substring(0, 5) + "20";
        }

        private string getSheetNoAddress(string sheetDataStartAddress)
        {
            return sheetDataStartAddress.Substring(0, 5) + "30";
        }

        private string getMeasureDateAddress(string sheetDataStartAddress)
        {
            return sheetDataStartAddress.Substring(0, 5) + "90";
        }

        public LSETInfo LsetInfo { get; set; }

        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_COLORAUTOMEASIRER;
        }

        public ColorAutoMeasurerMachineInfo(LSETInfo lsetInfo)
        {
            this.LsetInfo = lsetInfo;

            // 設定ファイルPLC設定を基ににPLCインスタンス初期化
            InitPLC(lsetInfo);
        }

        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {
            // 設定ファイル読み出し
            InitPropAtLoop(lsetInfo);
            this.LsetInfo = lsetInfo;

            string errMsg;

            base.machineStatus = Constant.MachineStatus.Runtime;

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

                //このルーチンだとOK、NGファイルは不要だがStartProcessの処理上作成してしまうので後で消す。
                List<string> filePathList = Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);
                filePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE));
                foreach (string filePath in filePathList)
                {
                    File.Delete(filePath);
                }
            }

            if (requestWorkComplete())
            {
                // 1シートを色調測定した後の完了処理
                workComplete(this.LsetInfo);
            }

            int timingNo = GetTimingNo(this.LsetInfo.ChipNM);
            EndingProcess(this.LsetInfo, timingNo, true, true, WorkCompleteType.Tray);

            ResponseOKFile(true, lsetInfo);

            //このルーチンだとOK、NGファイルは不要だがEndingProcessの処理上作成してしまうので後で消す。
            List<string> endFilePathList = Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);
            endFilePathList.AddRange(Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE));
            foreach (string filePath in endFilePathList)
            {
                File.Delete(filePath);
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
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE, 0);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_REQ, 0);

            outputLog($"装置 << システム プログラム名:{programName}送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND}]");
            outputLog($"装置 << システム 装置停止要求(0：継続)送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE}]");
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

        private bool requestWorkComplete()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_COMPLETE_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 完了処理要求確認 [アドレス:{PLC_MEMORY_ADDR_COMPLETE_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 色調自動測定では判定が終わったらファイルをRunからOutputに移す。　※トレイ完了時にまとめて登録処理する。
        /// </summary>
        private void responseWorkComplete(string orgDir)
        {
            List<string> ngFilePathList = Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE);
            bool existJudgeFile = false;

            if (ngFilePathList.Count() > 0)
            {
                plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_COMPLETE_RESULT_SEND, 2);
                outputLog($"装置 << 色調判定NG返答 [アドレス:{PLC_MEMORY_ADDR_COMPLETE_RESULT_SEND}]");
                existJudgeFile = true;
            }

            List<string> okFilePathList = Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);

            if (okFilePathList.Count() > 0)
            {
                plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_COMPLETE_RESULT_SEND, 1);
                outputLog($"装置 << 色調判定OK返答 [アドレス:{PLC_MEMORY_ADDR_COMPLETE_RESULT_SEND}]");
                existJudgeFile = true;
            }

            if (existJudgeFile)
            {
                plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_COMPLETE_REQ, 0);
                outputLog($"装置 << システム 完了処理完了 [アドレス:{PLC_MEMORY_ADDR_COMPLETE_REQ}]");

                foreach (string filepath in ngFilePathList)
                {
                    File.Delete(filepath);
                    outputLog($"装置 NGファイル判定完了の為削除 [ファイルパス:{filepath}]");
                }

                foreach (string filepath in okFilePathList)
                {
                    File.Delete(filepath);
                    outputLog($"装置 OKファイル判定完了の為削除 [ファイルパス:{filepath}]");
                }

                List<string> filePathList = Common.GetFiles(EndFileDir, ".*\\.");
                foreach (string filePath in filePathList)
                {
                    string destPath = Path.Combine(orgDir, Path.GetFileName(filePath));
                    File.Move(filePath, destPath);
                }
            }
        }

        private bool checkTypeBlend(int lineCd, out string typeCd, out string errMsg)
        {
            typeCd = "";
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

                string lotTypeCd = getLotTypeCd(lotNo);
                if (string.IsNullOrEmpty(lotTypeCd))
                {
                    errMsg = $"読み取み込んだロットのNASCA指図から型番情報が取得できません。[段数：[{i + 1}段],アドレス：{address},ロットNo：{lotNo}]";
                    return false;
                }

                // -------- 型番混在チェック
                if (string.IsNullOrWhiteSpace(typeCd) == true)
                {
                    // 1ロット目
                    typeCd = lotTypeCd;
                }
                else
                {
                    // 2ロット目：型番混在チェック
                    if (typeCd != lotTypeCd)
                    {
                        errMsg = $"読込ロットの型番が他のロットと違います。[段数：[{i + 1}段],ロットNo：{lotNo},型番：{lotTypeCd}],[型番(他ロット)：{typeCd}]";
                        return false;
                    }
                }
            }
            if (string.IsNullOrEmpty(typeCd))
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

        /// <summary>
        /// 完了処理
        /// </summary>
        private void workComplete(LSETInfo lsetInfo)
        {
            //シートのパラメータチェックはRunフォルダで行う。
            string endFileDirOrg = EndFileDir;
            try
            {
                EndFileDir = Path.Combine(EndFileDir, "RUN");
                CreateFileProcess(lsetInfo, false, "Result");
                int timingNo = GetTimingNo(lsetInfo.ChipNM);
                EndingProcess(lsetInfo, timingNo, true, false, WorkCompleteType.Sheet);

                responseWorkComplete(endFileDirOrg);
            }
            finally
            {
                EndFileDir = endFileDirOrg;
            }
        }

        protected void EndingProcess(LSETInfo lsetInfo, int? timingNo, bool isAvailable, bool isNeedAddedInfo, WorkCompleteType compType)
        {
            List<Log> logList = new List<Log>();
            List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

            //bool doneFG = false;
            string typeCD = string.Empty;

            if (isAvailable == false)
            {
                if (string.IsNullOrEmpty(CIFS.GetLatestProcessableFileIdentity(EndFileDir, CIFS.EXT_OK_FILE, DateIndex, EndFileIdLen, true)) == false)
                {
                    //既に.OKファイルが存在する場合は作成しない
                    return;
                }

                Thread.Sleep(3000);
                string dateStr = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                CIFS.OutputResultFile(EndFileDir, dateStr, string.Empty, true);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, dateStr));
                return;
            }

            // ロット、タイプ、工程Noがファイル名に付与される。付与された事がEICSで処理していいというトリガ。
            // 出力ファイルの存在チェック関数呼び出し。
            // ファイルのリネーム情報を無視する場合はファイル名リネーム自体を無視してファイル取得。
            string targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(EndFileDir, EXT_TRIG_FILE(), DateIndex, EndFileIdLen, isNeedAddedInfo);
            FileNameAdditionInfo fileNmAddInfo = new FileNameAdditionInfo();

            //処理可能な出力ファイルが有れば、ファイル毎に処理する関数
            if (string.IsNullOrEmpty(targetFileIdentity) == false)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾄﾘｶﾞ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                List<string> trigFilePathList = Common.GetFiles(EndFileDir, string.Format("{0}.*\\.{1}", targetFileIdentity, EXT_TRIG_FILE()));
                if (trigFilePathList.Count > 1)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.FATAL,
                        string.Format("同一のﾌｧｲﾙ種類、出力ﾀｲﾐﾝｸﾞ識別文字列(日付orDM)でﾌｧｲﾙが複数抽出されました。 監視Dir:{0}　出力ﾀｲﾐﾝｸﾞ識別文字列:{1}",
                        EndFileDir, targetFileIdentity));
                }

                string trigFilePath = trigFilePathList.Single();

                string trigFileNm = Path.GetFileNameWithoutExtension(trigFilePath);

                DateTime dt = File.GetLastWriteTime(trigFilePath);

                if (isNeedAddedInfo)
                {
                    fileNmAddInfo = GetInfoFromFileNm(trigFilePath, FileNmSplitter
                           , this.IdentityIndexInFileNm, this.TypeCdIndexInFileNm, this.LotNoIndexInFileNm, this.MagNoIndexInFileNm, this.ProcNoIndexInFileNm);
                }
                else
                {
                    fileNmAddInfo = getInfoFromOther(trigFilePath);
                }

                if (IsFinishedLENSProcess(fileNmAddInfo.LotNo, fileNmAddInfo.ProcNo, lsetInfo.InlineCD, targetFileIdentity, lsetInfo.EquipmentNO) == false)
                {
                    return;
                }

                AdditionEndProcess(lsetInfo, targetFileIdentity, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, fileNmAddInfo.ProcNo, lsetInfo.EquipmentNO);

                string prefixNm = "Result";
                bool isErrorSound = true;
                if (compType == WorkCompleteType.Sheet)
                {
                    // CheckMachineEndFileの中で警告音を出す処理があるので鳴らさないように
                    // （シート毎の測定値は異常判定はするが、特に作業者にフォローしてもらう運用はないのでエラー表示や警告音は出さない）
                    isErrorSound = false;
                }

                CheckResult chkResult = CheckMachineEndFile(lsetInfo, fileNmAddInfo.TypeCd, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, dt, targetFileIdentity, prefixNm, isErrorSound);

                //処理対象ファイルの種類毎の情報を取得
                List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);

                //通常は完了時の判定結果ファイル出力は不要。
                //20160428現在、例外的に完了時の判定結果ファイル出力が必要なEDPMachineInfo、BWMMachineInfo以外は
                //OutputResultの中でファイル出力しないようにしている 2016/4/28 n.yoshimoto

                OutputResult(lsetInfo, targetFileIdentity, EndFileDir, chkResult, false);

                if (compType == WorkCompleteType.Sheet)
                {
                    // シート毎の測定値は異常判定はするが、特に作業者にフォローしてもらう運用はないのでエラー表示や警告音は出さない
                    base.errorMessageList.Clear();
                }

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                BackupFileEndTiming(targetFileIdentity, fileNmAddInfo.LotNo, dt);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                //doneFG = true;

            }

            // トレイ処理完了時(トレイの最終シート処理で実行する)
            if (isTrayFileEnd(fileNmAddInfo.LotNo, EndFileDir))
            {
                CheckQC(lsetInfo, timingNo.Value, fileNmAddInfo.TypeCd);
            }
        }
        
        /// <summary>
        /// EndingProcess関数内での閾値判定 閾00
        /// 値を樹脂グループで絞って取得する為、オーバーライド
        /// </summary>
        protected override CheckResult CheckMachineEndFile(LSETInfo lsetInfo, string typeCD, string lotNO, string magNO, DateTime dt, string targetFileIdentity, string prefixNM, bool isErrorSound)
        {
            //初期判定時はロットがDUMMYになってるので樹脂Grを最初のロットで取得する。
            string resinLot = lotNO;
            if (resinLot == "DUMMY")
            {
                for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
                {
                    string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                    string tLotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                    if (string.IsNullOrEmpty(tLotNo)) continue;

                    string[] lotNoArray = tLotNo.Split(' ');

                    if (lotNoArray.Length >= 2)
                    {
                        resinLot = lotNoArray[1];
                    }
                }
            }
            EICS.NascaLotCharInfo resingroupinfo = ConnectDB.GetNascaLotCharInfo(lsetInfo.InlineCD, resinLot, typeCD, Constant.RESIN_GROUP_LOTCHAR_CD);
            if (string.IsNullOrEmpty(resingroupinfo.LotCharVal))
                throw new ApplicationException($"投入中ロット:{lotNO}の製品型番:{typeCD}は閾値マスタの樹脂Gr管理が対象になりますが、NASCAロット特性で樹脂グループの特性値が無い為、出来栄えチェックをする事ができません。");

            return CheckMachineEndFile(lsetInfo, typeCD, lotNO, magNO, dt, targetFileIdentity, resingroupinfo.LotCharVal, prefixNM, isErrorSound);
        }

        /// <summary>
        /// EndingProcess関数内での閾値判定 閾値を樹脂グループで絞って取得する為、オーバーライド
        /// </summary>
        protected override FileNameAdditionInfo getInfoFromOther(string filePath)
        {
            FileNameAdditionInfo fileNmAddInfo = new FileNameAdditionInfo();

            string lottypecd = GetTypeFromPlcLotInfo();

            fileNmAddInfo.Identity = Path.GetFileNameWithoutExtension(filePath).Split('_')[0];
            fileNmAddInfo.LotNo = "DUMMY";
            fileNmAddInfo.TypeCd = lottypecd;
            fileNmAddInfo.ProcNo = 0;
            fileNmAddInfo.MagNo = "DUMMY";

            return fileNmAddInfo;
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
                return getLotTypeCd(lotNo);
            }
            return null;
        }

        /// <summary>
        /// EndingProcess関数内での閾値判定 閾値を樹脂グループで絞って取得する為、オーバーライド
        /// </summary>
        public override void BackupFileEndTiming(string targetFileIdentity, string lotNo, DateTime dt)
        {
            //RUNフォルダ内のチェックは一時チェックなのでデータを退避しない
            if (EndFileDir.EndsWith("\\RUN") == true) return;

            List<string> backupFileList = GetBackupTargetFile(targetFileIdentity);

            BackupDoneEndFiles(backupFileList, EndFileDir, lotNo, dt);
        }

        protected override void OutputResult(LSETInfo lsetInfo, string targetFileIdentity, string outputPath, CheckResult chkResult, bool isStartTiming)
        {
            if (chkResult.RegistrationData[0].NascaLotNO == "DUMMY")
            {
                for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
                {
                    string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                    string tLotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                    if (string.IsNullOrEmpty(tLotNo)) continue;

                    string[] lotNoArray = tLotNo.Split(' ');

                    if (lotNoArray.Length >= 2)
                    {
                        chkResult.RegistrationData[0].NascaLotNO = lotNoArray[1];
                    }
                    else
                    {
                        chkResult.RegistrationData[0].NascaLotNO = tLotNo;
                    }
                    break;
                }
            }

            EICS.NascaLotCharInfo resingroupinfo = ConnectDB.GetNascaLotCharInfo(lsetInfo.InlineCD, chkResult.RegistrationData[0].NascaLotNO, chkResult.RegistrationData[0].MaterialCD, Constant.RESIN_GROUP_LOTCHAR_CD);

            if (resingroupinfo == null || string.IsNullOrEmpty(resingroupinfo.LotCharVal))
            {
                throw new ApplicationException($"投入ロットから樹脂グループ特性の取得に失敗しました。ロット：『{chkResult.RegistrationData[0].NascaLotNO}』");
            }
            string resinGroup = resingroupinfo.LotCharVal;



            string timingMsg = string.Empty;

            if (isStartTiming)
            {
                timingMsg = "開始";
            }
            else
            {
                timingMsg = "完了";
            }

            //判定結果出力

            //BBSQの検証時に完了時OKファイルが出力されていない事が原因で、装置側トリガが落ちていないことが判明
            //19ラインのレーザスクライブではOKファイルが完了時に出ており、下記の完了時にEDP、BWM以外は出力しないという根拠不明
            //BBSQの事もあるのでひとまず、下記はコメントアウト
            //if (isStartTiming == false)
            //{
            //	//完了時はEDPMachineInfo、BWMMachineInfoを除き判定結果ファイルの出力は行わない 2016/4/28 n.yoshimoto
            //}
            //else if (chkResult.ErrorInfo.Count > 0)

            if (chkResult.ErrorInfo.Count > 0)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0}時処理 判定結果NG出力 {1}/{2}/{3} ﾄﾘｶﾞﾌｧｲﾙ識別:{4} ", timingMsg, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                base.errorMessageList.AddRange(chkResult.ErrorInfo);

                string errMsg = string.Empty;

                foreach (ErrMessageInfo errMsgInfo in chkResult.ErrorInfo)
                {
                    errMsg += errMsgInfo.MessageVAL + "\r\n";
                }

                //BBSQ等のPLC連動ソフトで問題があったので巻き戻し
                ////完了時処理では装置へのOK/NG応答は不要なためファイル出力無し
                //if(isStartTiming == true)
                CIFS.OutputResultFile(outputPath, targetFileIdentity, errMsg, false);
            }
            else
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0}時処理 判定結果OK出力 {1}/{2}/{3} ﾄﾘｶﾞﾌｧｲﾙ識別:{4} ", timingMsg, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                ////BBSQ等のPLC連動ソフトで問題があったので巻き戻し
                ////完了時処理では装置へのOK/NG応答は不要なためファイル出力無し
                //if (isStartTiming == true)
                CIFS.OutputResultFile(outputPath, targetFileIdentity, string.Empty, true);
            }

            if (isStartTiming == false)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
                    , string.Format("完了時処理 閾値判定結果のDB登録開始 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} "
                    , lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                foreach (Log log in chkResult.RegistrationData)
                {
                    ConnectDB.InsertTnLOG(log, lsetInfo, resinGroup);
                }

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
                    , string.Format("完了時処理 閾値判定結果のDB登録完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} "
                    , lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
            }
        }

        protected override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
        {
            if (isCompletedTray(lotNO) == false)
                // シートの完了時は処理しない
                return;

            // 色調測定結果の保存 (トレイ毎)
            saveMeasureResult(lotNO, lsetInfo.ModelNM);

            // 色調測定結果の傾向監視(トレイ毎)
            colorTrendMonitoring(lotNO);
        }

        /// <summary>
        /// 色調測定結果をデータベースに保存
        /// </summary>
        private void saveMeasureResult(string lotNo, string modelNm)
        {
            try
            {
                using (DataContext.EICSDataContext db = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, this.LineCD)))
                {
                    if (db.TnPsMeasureResult.Where(p => p.Lot_NO == lotNo && p.Plant_CD == this.Code && p.New_FG == true).Any())
                        // 同装置、同ロットで記録があれば処理なし
                        return;

                    outputLog($"[トレイ完了時処理] 色調測定結果保存 開始 ロットNo:{lotNo}");

                    string typeCd = getLotTypeCd(lotNo);

                    // 過去トレイデータの紐付けを外す
                    string firstTrayNo = getEnteredTrayNo(getTrayNoAddress(PLC_MEMORY_ADDR_SHEETDATASTART[1]));
                    var previousList = db.TnPsMeasureResult.Where(p => p.Tray_NO == firstTrayNo && p.New_FG == true);
                    foreach (var p in previousList)
                    {
                        p.New_FG = false;
                    }

                    // 保存対象の測定値に絞る (条件：TmFILEFMTのAdditionEndProcess_FG=true)  
                    var paramList = db.TmFILEFMT.Where(p => p.Model_NM == modelNm && p.AdditionEndProcess_FG == true && p.Del_FG == false)
                        .Select(p => new { p.QcParam_NO });
                    foreach (var p in paramList)
                    {
                        string changeUnitVal = db.TmPRM.Where(m => m.QcParam_NO == p.QcParam_NO && m.Del_FG == false).Select(m => m.ChangeUnit_VAL).SingleOrDefault();

                        var plcFileConvList = db.TmPlcFileConv.Where(c => c.Model_NM == modelNm && c.QcParam_NO == p.QcParam_NO
                                && c.Del_FG == false && c.Prefix_NM == "End");
                        if (plcFileConvList.Any() == false)
                        {
                            throw new ApplicationException($"マスタ(TmPlcFileConv)が未登録のため、色調測定結果の保存ができませんでした。マスタに登録後、再開して下さい。 装置型式:{modelNm} 管理番号:{p.QcParam_NO}");
                        }

                        // シート毎にPLCからデータを取得
                        for (int i = 0; i < PLC_MEMORY_ADDR_SHEETDATASTART.Count; i++)
                        {
                            decimal measureAve = 0;

                            plcFileConvList = plcFileConvList.Where(c => c.Identify_CD == (i + 1).ToString());
                            foreach (var address in plcFileConvList)
                            {
                                string plcValue = plc.GetDataAsString(address.Plc_ADDR, address.Data_LEN, address.DataType_CD);
                                double measureVal = CalcChangeUnit(changeUnitVal, double.Parse(plcValue));
                                measureAve += Convert.ToDecimal(measureVal);
                            }
                            measureAve = measureAve / plcFileConvList.Count();

                            DataContext.TnPsMeasureResult d = new DataContext.TnPsMeasureResult();

                            d.Tray_NO = getEnteredTrayNo(getTrayNoAddress(PLC_MEMORY_ADDR_SHEETDATASTART[i]));
                            d.Sheet_NO = getEnteredSheetNo(getSheetNoAddress(PLC_MEMORY_ADDR_SHEETDATASTART[i]));
                            d.QcParam_NO = p.QcParam_NO;
                            d.Measure_DT = System.DateTime.Now;
                            d.Plant_CD = this.Code;
                            d.Lot_NO = lotNo;
                            d.Type_CD = typeCd;
                            d.New_FG = true;
                            d.MeasureAve_VAL = measureAve;
                            d.LastUpd_DT = System.DateTime.Now;
                            db.TnPsMeasureResult.InsertOnSubmit(d);
                        }
                    }
                    db.SubmitChanges();
                    outputLog($"[トレイ完了時処理] 色調測定結果保存 完了 ロットNo:{lotNo}");
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException($"測定結果の保存に失敗しました。 LotNo:{lotNo} エラー内容:{err.Message} トレース:{err.StackTrace}");
            }
        }

        /// <summary>
        /// 1シート毎の対象色調測定結果を1ロット(1トレイ)毎に平均値を取り、閾値超えチェックを行う
        /// </summary>
        /// <param name="lotNo"></param>
        private void colorTrendMonitoring(string lotNo)
        {
            string typeCd = getLotTypeCd(lotNo);
            string[] resinGroup = getResinGroup(lotNo, typeCd);
            if (resinGroup.Any() == false)
                throw new ApplicationException($"投入中ロット:{lotNo}の製品型番:{typeCd}は閾値マスタの樹脂Gr管理が対象になりますが、NASCAロット特性で樹脂グループの特性値が無い為、出来栄えチェックをする事ができません。");
            
            List<Plm> plmList = Plm.GetDatas(this.LsetInfo.InlineCD, typeCd, this.LsetInfo.ModelNM, false);
            plmList = Plm.GetParameterFromResinGroup(plmList, resinGroup);

            outputLog("[トレイ完了時処理] 色調測定値傾向監視 開始");

            foreach (Plm plm in plmList)
            {
                if (plm.EquipManageFG == 1 && plm.EquipmentNO != this.Code)
                {
                    // 閾値が装置毎管理の設定で装置番号が違う閾値は除外
                    continue;
                }

                using (DataContext.EICSDataContext db = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, this.LineCD)))
                {
                    // TmFILEFMのTPrefix_NMが"End"の管理項目が対象
                    if (db.TmFILEFMT
                        .Where(f => f.QcParam_NO == plm.QcParamNO && f.Model_NM == plm.ModelNM && f.Del_FG == false && f.Prefix_NM == "End").Any() == false)
                        continue;

                    // 重複登録チェック　既に登録済みは何もしない
                    if (db.TnLOG.Where(l => l.Inline_CD == this.LineCD && l.Equipment_NO == LsetInfo.EquipmentNO && l.NascaLot_NO == lotNo && l.QcParam_NO == plm.QcParamNO).Any())
                        continue;

                    var result = db.TnPsMeasureResult.Where(m => m.Lot_NO == lotNo && m.New_FG == true && m.QcParam_NO == plm.QcParamNO);
                    if (result.Any() == false)
                    {
                        // 色調測定データがない場合は傾向管理しない
                        continue;
                    }

                    DateTime measureDt = result.FirstOrDefault().Measure_DT;
                    decimal ave = result.Average(r => r.MeasureAve_VAL);

                    string errMsg = ParameterInfo.CheckParameter(plm, ave.ToString(), this.LsetInfo, lotNo);
                    if (string.IsNullOrEmpty(errMsg) == false)
                    {
                        ErrMessageInfo errMsgInfo = new ErrMessageInfo(errMsg, System.Drawing.Color.Red);
                        base.errorMessageList.Add(errMsgInfo);
                    }

                    Log log = new Log();

                    log.QcParamNO = plm.QcParamNO;
                    log.NascaLotNO = lotNo;
                    log.MagazineNO = lotNo;
                    log.DParameterVAL = ave;
                    log.MaterialCD = plm.MaterialCD;
                    log.MeasureDT = measureDt;
                    log.MessageNM = errMsg;
                    log.ErrorFG = true;

                    this.LsetInfo.TypeCD = plm.MaterialCD;
                    ConnectDB.InsertTnLOG(log, this.LsetInfo, string.Join(",", resinGroup));
                }
            }

            outputLog("[トレイ完了時処理] 色調測定値傾向監視 完了");
        }

        /// <summary>
        /// 切断後、入ったトレイのQRコード内容をPLCから取得
        /// </summary>
        /// <returns></returns>
        private string getEnteredTrayNo(string addess)
        {
            return this.plc.GetString(addess, MAGAZINENO_LENGTH, false, true);
        }

        /// <summary>
        /// 切断後、トレイに置いた位置番号をPLCから取得
        /// </summary>
        /// <returns></returns>
        private int getEnteredSheetNo(string address)
        {
            return this.plc.GetWordAsDecimalData(address, 1);
        }

        /// <summary>
        /// 測定日時をPLCから取得
        /// </summary>
        /// <returns></returns>
        private DateTime getMesureDate(string address)
        {
            return this.plc.GetWordsAsDateTime(address);
        }

        private string getLotTypeCd(string lotNo)
        {
            LotInfoReferring reference = GetLotInfoReferring(this.LsetInfo.EquipmentNO, this.LsetInfo.InlineCD);
            if (reference == LotInfoReferring.ARMS)
            {
                Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(this.LsetInfo.InlineCD, lotNo);
                return lot.TypeCd;
            }
            else
            {
                return ConnectDB.GetTypeCD(this.LsetInfo.InlineCD, lotNo);
            }
        }

        private string[] getResinGroup(string lotNo, string typeCd)
        {
            LotInfoReferring reference = GetLotInfoReferring(this.LsetInfo.EquipmentNO, this.LsetInfo.InlineCD);
            if (reference == LotInfoReferring.ARMS)
            {
                Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(this.LsetInfo.InlineCD, lotNo);
                return lot.ResinGpCd.Split(',');
            }
            else
            {
                EICS.NascaLotCharInfo resingroup = ConnectDB.GetNascaLotCharInfo(LsetInfo.InlineCD, lotNo, typeCd, Constant.RESIN_GROUP_LOTCHAR_CD);
                return resingroup.LotCharVal.Split(',');
            }
        }

        /// <summary>
        /// トレイ完了時か確認
        /// </summary>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        private bool isCompletedTray(string lotNo)
        {
            if (lotNo == "DUMMY" || string.IsNullOrEmpty(lotNo))
            {
                // シート完了時はARMSがファイル名にロット番号の代わりにダミー文字を付け足すので
                // そうじゃない場合にトレイ完了時と判断する
                // (装置の完了信号はARMSが落としてしまうので使えない）
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// トレイ処理完了時
        /// (トレイ全てのファイル処理を終えた後、CheckQCを実行させたい為のトリガ)
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private bool isTrayFileEnd(string lotNo, string directoryPath)
        {
            // ARMSがファイル名にロット番号を付与している(トレイ完了時)、
            // ロット番号で出力ファイルを検索したけど無いが条件
            
            if (isCompletedTray(lotNo) == false)
                return false;

            string[] files = Directory.GetFiles(directoryPath, $"*{lotNo}*");
            if (files.Any() == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
