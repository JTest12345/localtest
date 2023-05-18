using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EICS.Database;
using EICS.Structure;

namespace EICS.Machine.CIFSBase
{
    public class CO2MachineInfo : CIFSBasedMachine
    {
        List<FileFmtWithPlm> fileFmtWithPlmListLocal = new List<FileFmtWithPlm>();
        DateTime lastGetLimitMaster = new DateTime();

        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_CO;
        }
		public override void InitFirstLoop(LSETInfo lsetInfo)
		{
			CheckFileFmtFromParamWhenInit(lsetInfo, false);
		}

		public override void CheckFile(LSETInfo lsetInfo)
        {

            base.machineStatus = Constant.MachineStatus.Runtime;

            InitPropAtLoop(lsetInfo);

            // 開始時処理関数
            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);
            List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, true);

            List<ErrMessageInfo> errMsgInfoList = new List<ErrMessageInfo>();
            List<FileFmtWithPlm> fileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);
            StartingProcess(lsetInfo, fileFmtWithPlmList);
  
            ResultFileLowerExtention();

            int timingNo = GetTimingNo(lsetInfo.ChipNM);

            // 完了時処理関数
            EndingProcess(lsetInfo, timingNo);

            //AdditionProcess(lsetInfo);
        }
         

        //wstファイルをトリガとするようにオーバーライド
        public override void StartingProcess(LSETInfo lsetInfo, List<FileFmtWithPlm> fileFmtWithPlmList)
        {
            List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

            string targetFileIdentity = string.Empty;
            try
            {
                List<Log> logList = new List<Log>();
                DateTime doneDt = DateTime.Now;
                CheckResult chkResult = new CheckResult();

                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

                //処理開始して良いかトリガを確認
                targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false); ;

                //処理可能な出力ファイルが有れば、ファイル毎に処理する関数
                if (string.IsNullOrEmpty(targetFileIdentity) == false)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 トリガ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                    // .wstファイル群の取得
                    List<string> chkTargetFileList = Common.GetFiles(StartFileDir, EXT_START_FILE);

                    // ファイル内の装置パラメタチェック
                    foreach (string chkTargetFilePath in chkTargetFileList)
                    {
                        string fileNm = Path.GetFileName(chkTargetFilePath);
                        string[] fileNmElement = Path.GetFileNameWithoutExtension(fileNm).Split(FileNmSplitter);

                        string fileKind = string.Empty;

                        if (FileKindIndexInFileNm < 0)
                        {
                        }
                        else if (fileNmElement.Count() <= FileKindIndexInFileNm)
                        {
                            fileNmElement = new string[] { Path.GetFileNameWithoutExtension(fileNm), string.Empty };
                            fileKind = fileNmElement[FileKindIndexInFileNm];
                        }
                        else
                        {
                            fileKind = fileNmElement[FileKindIndexInFileNm];
                        }

                        List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, fileKind, true);
                        FileScan fileScan;

                        List<FileFmt> fileFmtList = FileFmt.GetData(lsetInfo, true, fileKind, null);

                        if (fileScanList.Count == 0 && fileFmtList.Count == 0)
                        {
                            continue;
                        }
                        else if (fileScanList.Count > 0 && fileFmtList.Count == 0)
                        {
                            throw new ApplicationException(
                                string.Format("TmFileScanﾏｽﾀ設定済みですがTmFILEFMT未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
                                lsetInfo.ModelNM, fileKind));
                        }
                        else if (fileScanList.Count == 0 && fileFmtList.Count > 0)
                        {
                            throw new ApplicationException(
                                string.Format("TmFILEFMTﾏｽﾀ設定済みですがTmFileScan未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
                                lsetInfo.ModelNM, fileKind));
                        }
                        else if (fileScanList.Count == 1)
                        {
                            fileScan = fileScanList.Single();
                        }
                        else
                        {
                            throw new ApplicationException(
                                string.Format("TmFileScanから複数のﾏｽﾀが取得されました。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}", lsetInfo.ModelNM, fileKind));
                        }

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));

                        string filePath = Path.Combine(StartFileDir, fileNm);

                        MachineFile macFile = new MachineFile();
                        if (macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, LF, lsetInfo.TypeCD, string.Empty, string.Empty, doneDt, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, EncodeStr, ref logList) == false)
                        {
                            continue;
                        }
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));
                    }

                    if (doneDt != DateTime.MinValue)
                    {
                        chkTargetFileList.AddRange(GetBackupTargetStartFiles(targetFileIdentity));

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                        // 処理したファイルの移動
                        BackupDoneStartFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                        //判定結果出力
                        if (errMsgList.Count > 0)
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果NG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                            base.errorMessageList.AddRange(errMsgList);

                            string errMsg = string.Empty;

                            foreach (ErrMessageInfo errMsgInfo in errMsgList)
                            {
                                errMsg += errMsgInfo.MessageVAL + "\r\n";
                            }

                            CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
                        }
                        else
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                            CIFS.OutputResultFile(StartFileDir, targetFileIdentity, string.Empty, true);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (string.IsNullOrEmpty(targetFileIdentity))
                {
                    string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるSTOP出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\n StackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
                    CIFS.OutputStopFile(StartFileDir, DateTime.Now.ToString("yyyyMMddHHmmss"), errMsg, false);
                }
                else
                {
                    string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるNG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\n StackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
                    CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
                }

                throw;
            }
        }

        /// <summary>
        /// 装置側が小文字のok/ngにしか対応してないのでその対応。
        /// 以前の検証時に要望は上げているはずなのでメーカーに再依頼を掛けて不要になれば除外する。
        /// </summary>
        private void ResultFileLowerExtention()
        {
            List<string> okFileList = Common.GetFiles(StartFileDir, CIFS.EXT_OK_FILE);
            foreach(string file in okFileList)
            {
                string lowerFileNm = file.Replace(".OK", ".ok");
                File.Move(file, lowerFileNm);
            }
            
            List<string> ngFileList = Common.GetFiles(StartFileDir, CIFS.EXT_NG_FILE);
            foreach (string file in ngFileList)
            {
                string lowerFileNm = file.Replace(".NG", ".ng");
                File.Move(file, lowerFileNm);
            }
        }
    }
}
