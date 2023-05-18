using EICS.Database;
using EICS.Machine.Base;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;

namespace EICS.Machine
{
	/// <summary>
	/// AuSn(金スズのバンプボンダー)
	/// </summary>
	class BBSQMachineInfo : PLCDDGBasedMachine
	{
		private const string START_PROCESS_TARGET_FILE_NM = "Parameters.xml";
        private const int RESULT2_COLUMNS_NUM = 5;
        private const int RESULT2_DOTNUM_COLUMN = 4;
        private const string PREFIX_RESULT2 = "Result2";
        private const string PREFIX_RESULT3 = "Result3";


        //SQ装置は完了時のファイルが基板毎と年月日時分の2種あって設定ファイルでフォローしきれないので
        //基板DMの文字数は決め打ちとする。（元々NTSV基板関連は厚み取得等で位置決め打ちの箇所が他にもあるので
        //冗長性は変わらないと判断）
        private const int SUBSTRATE_IDENTITY_LENGTH = 17;

        new protected virtual string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"EM50000",
				"EM50002"
			};
		}

        protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "EM50020"; }

        //BBSQは他装置と違い傾向管理ファイル出力トリガをfin2で出力するように対応 2016/6/3 n.yoshi
        protected override string EXT_TRIG_FILE() { return EXT_FIN2_FILE; }

        //さらに追加でmpdファイルと同時に傾向管理ファイルが出力されるようになったのでそれ用の拡張子も用意。//2017/8/1 湯浅
        private const string EXT_TRIG_FILE_SUB = "fin";

        public override void CheckFile(LSETInfo lsetInfo)
        {
#if TEST
			lsetInfo.InputFolderNM = @"C:\qcil\data\test";
			lsetInfo.IPAddressNO = "172.21.56.53";
#endif
            try
            {
				base.machineStatus = Constant.MachineStatus.Runtime;

				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);
				//問題発生時は装置停止
#if TEST
#else
                
				//ハートビート Hレベル
				SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), true);

                bool xmlParamResult = isCheckOKXmlParameter(lsetInfo);
                if(xmlParamResult == false)        
                {
                    MachineStopProcessWithoutFile(lsetInfo, true);
                    return;
                }

                if (lsetInfo.MainThreadFG)
				{
					CreateFileProcess(lsetInfo, true);
					CreateFileProcess(lsetInfo, false);
				}

                StartingProcess(lsetInfo);

                MachineStopProcess(lsetInfo, true);
#endif
				int timingNo;
				if (Common.IsLEDChip(lsetInfo.ChipNM))
				{
					timingNo = QC_TIMING_NO_LED();
				}
				else
				{
					timingNo = QC_TIMING_NO_ZD();
				}

                EndingProcessOnlyFin2File(lsetInfo, timingNo);

                //基板単位の傾向管理追加　　※EndingProcessOnlyFin2Fileより後に処理するようにしないとfin2を拾ってしまうバグがある
                this.EndingProcessEachSubstrate(lsetInfo, timingNo, true);

#if TEST
#else
                MachineStopProcess(lsetInfo, false);

				ResponseOKFile(true, lsetInfo);

				//ハートビート Lレベル
				SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), false);
#endif
            }
            catch (Exception err)
			{
				//装置停止処理
				SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
				throw;
			}
		}

		public bool isCheckOKXmlParameter(LSETInfo lsetInfo)
		{
			bool xmlFileJudgeResult = true;
			string dtStr = string.Empty;
			string createFileNm = string.Empty;
			string ext = string.Empty;

			List<string> subGrpOutputDirList = new List<string>();
			AlertLog alertLog = AlertLog.GetInstance();
            
            try
			{
                //全パラチェック
                List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);
                FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false);

                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

				if (Directory.Exists(StartFileDir) == false)
				{
					Directory.CreateDirectory(StartFileDir);
				}

				Dictionary<string, string> prefixList = FileFmt.GetMachineFilePrefix(lsetInfo, 0, true, true);

				//取得要求の出ているPrefixに関連するPLCメモリアドレスリストを取得
				foreach (KeyValuePair<string, string> prefixAddr in prefixList)
				{
					//データ取得要求チェック
					if (IsGetableData(plc, prefixAddr.Value) == false)
					{
						continue;
					}

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} XMLパラメータチェック開始");

                    //TmFILEFMTからPrefix_NMでレコード取得する
                    //List<FileFmt> fileFmtList = FileFmt.GetData(lsetInfo, true, prefixAddr.Key, null);

                    List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(prefixAddr.Key, lsetInfo, true);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} getStartingProcTargetFilePath開始");

                    //最新の処理対象ファイルを取得する
                    string targetFilePath = getStartingProcTargetFilePath(prefixAddr.Key);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} getStartingProcTargetFilePath完了:{targetFilePath}");

                    //①ファイルを取得したらFileFMTとPlmとXmlDocを渡す

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} XML読み込み開始:{targetFilePath}");

                    XElement xmlDoc;
                    try
                    {
                        xmlDoc = XElement.Load(targetFilePath);
                    }
                    catch (Exception)
                    {
                        // ファイルロックが発生したため、リトライするように変更
                        Thread.Sleep(3000);
                        xmlDoc = XElement.Load(targetFilePath);
                    }

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} XML読み込み完了:{targetFilePath}");

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} 閾値取得開始");

                    List<Plm> plmList = Plm.GetData(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, lsetInfo.ChipNM, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD));
                    List<FileFmtWithPlm> fileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} 閾値取得完了");

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} 閾値チェック開始:{fileFmtWithPlmList.Count}");

                    List<FileData> fileDataList = new List<FileData>();

					foreach (FileFmtWithPlm fileFmt in fileFmtWithPlmList)
					{
						Plm plm = plmList.Find(p => p.QcParamNO == fileFmt.Plm.QcParamNO);
                        
						FileData fileData = FileFmt.GetFileData(xmlDoc, fileFmt.FileFmt, plm, targetFilePath);

						if (fileData.GetResult() == false)
						{
							xmlFileJudgeResult = false;

							string fileDataStr = string.IsNullOrEmpty(fileData.StrValue)? fileData.DecValue.ToString() : fileData.StrValue;

							string limitValStr;
								
							if(string.IsNullOrEmpty(fileData.RuleValue))
							{
								limitValStr = fileData.LowerLimit.ToString() + "～" + fileData.UpperLimit.ToString();
							}
							else	
							{
								limitValStr = fileData.RuleValue;
							}

							string errMsg = string.Format(
								"[{0}/{1}号機][管理番号:{7}/{2}]の設定値に誤りがあります。取得値={3},閾値={4},Lot={5},Linecd={6}",
								lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plm.ParameterNM, fileDataStr, limitValStr, "開始時判定の為、ﾛｯﾄ不明", lsetInfo.InlineCD, plm.QcParamNO);
							alertLog.logMessageQue.Enqueue(errMsg);
						}
					}

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} 閾値チェック完了");

                    //設定ファイルをコピー退避
                    string destPath = Path.Combine(StartFileDir, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
                    if (Directory.Exists(destPath) == false)
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    string destfileNm = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(targetFilePath);
                    File.Copy(targetFilePath, Path.Combine(destPath, destfileNm));

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号:{lsetInfo.MachineSeqNO} XMLパラメータチェック完了");
                }

            }
			catch (Exception err)
			{
				alertLog.logMessageQue.Enqueue(err.ToString());
				return false;
			}

			return xmlFileJudgeResult;
		}

		private string getStartingProcTargetFilePath(string targetIdentityNm)
		{
			//string[] targetDirs = Directory.GetDirectories(StartFileDir, targetIdentityNm, SearchOption.AllDirectories);

            //List<string> targetFilePathList = new List<string>();
            //foreach (string dirPath in targetDirs)
            //{
            //targetFilePathList.AddRange(Directory.GetFiles(StartFileDir, START_PROCESS_TARGET_FILE_NM, SearchOption.AllDirectories).ToList());
            //}
            KeyValuePair<DateTime, string> targetFileInfo = new KeyValuePair<DateTime, string>(DateTime.MinValue, string.Empty);

            string[] targetFilePathList = Directory.GetFiles(Path.Combine(StartFileDir, "レシピ"), START_PROCESS_TARGET_FILE_NM, SearchOption.AllDirectories);
            foreach (string targetFilePath in targetFilePathList)
			{
				FileInfo fileInfo = new FileInfo(targetFilePath);

				if (fileInfo.LastWriteTime >= targetFileInfo.Key)
				{
					targetFileInfo = new KeyValuePair<DateTime, string>(fileInfo.LastWriteTime, fileInfo.FullName);
				}
			}

			if (string.IsNullOrEmpty(targetFileInfo.Value))
			{
				string errMsg = string.Format("{0}の信号が1となった事を確認しましたが、ﾌｧｲﾙが見つかりませんでした。ﾌｧｲﾙ探索対象のﾙｰﾄﾃﾞｨﾚｸﾄﾘ:{1}", targetIdentityNm, StartFileDir);
				throw new ApplicationException(errMsg);
			}

			return targetFileInfo.Value;
		}

        /// <summary>
        /// mpdファイルの不良データ登録
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="targetFileIdentity"></param>
        /// <param name="lotNO"></param>
        /// <param name="magNO"></param>
        /// <param name="procNO"></param>
        /// <param name="equipNO"></param>
		protected void RegistrationMapDef(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{

			List<string> targetDMList = Common.GetFiles(EndFileDir, targetFileIdentity + ".mpd");

            if(File.Exists(Path.Combine(EndFileDir, targetFileIdentity + ".mpd")) == true)
            {
                CIFSBasedMachineAddMPD.MpdProcess(lsetInfo, targetFileIdentity, this.EndFileDir, lotNO, magNO, procNO, equipNO);
            }
            
		}

		protected override bool IsFinishedFile(string fileDir)
		{
			List<string> filePathList = new List<string>();
		
			filePathList.AddRange(Common.GetFiles(fileDir, ".*\\." + CIFS.EXT_FIN2_FILE));

			if (filePathList.Count() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        private void EndingProcessEachSubstrate(LSETInfo lsetInfo, int? timingNo, bool isAvailable)
        {

            List<Log> logList = new List<Log>();
            List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
            CheckResult chkResult = new CheckResult();

            bool doneFG = false;
            string typeCD = string.Empty;

            // ロット、タイプ、工程Noがファイル名に付与される。付与された事がEICSで処理していいというトリガ。
            // 出力ファイルの存在チェック関数呼び出し
            string targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(EndFileDir, EXT_TRIG_FILE_SUB, DateIndex, SUBSTRATE_IDENTITY_LENGTH, true);
            FileNameAdditionInfo fileNmAddInfo = new FileNameAdditionInfo();

            //処理可能な出力ファイルが有れば、ファイル毎に処理する関数
            if (string.IsNullOrEmpty(targetFileIdentity) == false)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾄﾘｶﾞ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                List<string> trigFilePathList = Common.GetFiles(EndFileDir, string.Format("{0}.*\\.{1}", targetFileIdentity, EXT_TRIG_FILE_SUB));
                if (trigFilePathList.Count > 1)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.FATAL,
                        string.Format("同一のﾌｧｲﾙ種類、出力ﾀｲﾐﾝｸﾞ識別文字列(日付orDM)でﾌｧｲﾙが複数抽出されました。 監視Dir:{0}　出力ﾀｲﾐﾝｸﾞ識別文字列:{1}",
                        EndFileDir, targetFileIdentity));
                }
                string trigFilePath = trigFilePathList.Single();
                //string trigFileNm = Path.GetFileNameWithoutExtension(trigFilePath);

                DateTime dt = File.GetLastWriteTime(trigFilePath);


                fileNmAddInfo = GetInfoFromFileNm(trigFilePath, FileNmSplitter
                    , this.IdentityIndexInFileNm, this.TypeCdIndexInFileNm, this.LotNoIndexInFileNm, this.MagNoIndexInFileNm, this.ProcNoIndexInFileNm);


                //LENS処理が終わってない場合はスルー。
                if (Database.LENS.WorkResult.IsComplete(
                        fileNmAddInfo.LotNo, fileNmAddInfo.ProcNo, lsetInfo.InlineCD, targetFileIdentity, lsetInfo.EquipmentNO) == false)
                {
                    return;
                }

                //Result2とResult3とmpdが揃っていることを確認。
                CheckSubstarteFileExist(EndFileDir, targetFileIdentity);



                //処理対象ファイルの種類毎の情報を取得
                List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);

                foreach(FileScan fileScan in fileScanList)
                {
                    //Result2とResult3以外は基板毎には対象外なのでスルー
                    if(fileScan.PrefixNM != PREFIX_RESULT2 && fileScan.PrefixNM != PREFIX_RESULT3)
                    {
                        continue;
                    }

                    string fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + fileScan.PrefixNM + @".*[.]" + EXT_END_FILE();

                    List<string> fileNmList = Common.GetFiles(EndFileDir, fileSearchPattern);

                    if (fileNmList.Count == 0)
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                            "ﾌｧｲﾙが存在しない為ｽｷｯﾌﾟ {0}/{1}/{2} 監視ﾊﾟｽ:{3} ﾌｧｲﾙ種:{4} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{5}"
                            , lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, EndFileDir, fileScan.PrefixNM, fileSearchPattern));

                        continue;
                    }
                    else if (fileNmList.Count > 1)
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN,
                            string.Format("同一のﾌｧｲﾙ種類、識別文字列(日付orDM)でﾌｧｲﾙが複数抽出されました。 検索Dir:{0} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{1}",
                            EndFileDir, fileSearchPattern));
                    }
                            
                    string fileNm = fileNmList.Single();
                    string filePath = Path.Combine(EndFileDir, fileNm);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));

                    if(fileScan.PrefixNM == PREFIX_RESULT3)
                    {
                        //標準のCIFS処理
                        if (MachineFile.CheckMachineFile(filePath, LF(), fileNmAddInfo.TypeCd, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, dt, fileScan, lsetInfo, lsetInfo.ChipNM, ref chkResult.ErrorInfo, EncodeStr, ref chkResult.RegistrationData) == false)
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理 ﾌｧｲﾙ内容判定処理が未完了のまま終了 {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));
                            continue;
                        }
                        OutputResult(lsetInfo, targetFileIdentity, EndFileDir, chkResult, false);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));
                    }
                    else if(fileScan.PrefixNM == PREFIX_RESULT2)
                    {
                        //SQ検査結果集計(SQ独自関数)
                        MagInfo magInfo = new MagInfo();
                        magInfo.sMaterialCD = fileNmAddInfo.TypeCd;
                        magInfo.sNascaLotNO = fileNmAddInfo.LotNo;

                        List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();

                        Result2_FileCheck(lsetInfo, magInfo, filePath, ref errMessageList);
                    }

                }

                //chkResult = CheckMachineEndFile(lsetInfo, fileNmAddInfo.TypeCd, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, dt, targetFileIdentity);

                RegistrationMapDef(lsetInfo, targetFileIdentity, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, fileNmAddInfo.ProcNo, lsetInfo.EquipmentNO);

                // 完了フォルダ内のファイル群の取得して該当の基板DMのファイルを全移動。
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                List<string> backupFileList = GetBackupTargetFile(targetFileIdentity);
                BackupDoneEndFiles(backupFileList, EndFileDir, fileNmAddInfo.LotNo, DateTime.Now);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                doneFG = true;
            }

            if (timingNo.HasValue && doneFG && string.IsNullOrEmpty(fileNmAddInfo.TypeCd) == false)
            {
                CheckQC(lsetInfo, timingNo.Value, fileNmAddInfo.TypeCd);
            }

        }

        public void Result2_FileCheck(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList)
        {
            //全パラチェック
            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);
            FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false);

            //全ファイル内容取得(複数行)
            string[] fileLineValue = GetMachineFileLineValue(mFilePath);

            //Resultファイルの紐付けマスタ情報(TmFILEFMT)を取得。Prefixは決め打ち。
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("Result2", lsetInfo, magInfo.sMaterialCD);

            //閾値絞り込みがlsetinfoのtypece固定なのでファイルの型番に変換。
            LSETInfo checkLsetInfo = (LSETInfo)lsetInfo.Clone();
            checkLsetInfo.TypeCD = magInfo.sMaterialCD;
            List<FileFmtWithPlm> fileFmtWithPlmList = FileFmtWithPlm.GetData(checkLsetInfo, false, plmPerTypeModelChipList, filefmtList, null);

            //20171026コードレビュー指摘により不要となった為、削除。（閾値上位のみでカバー可と判断）
            //ファイルから最大ドット数を取得
            //int maxDotNo = GetMaxDotNo(fileLineValue, mFilePath);

            foreach (FileFmtWithPlm filefmtInfo in fileFmtWithPlmList)
            {
                Prm prmData = Prm.GetData(LineCD, filefmtInfo.Plm.QcParamNO, lsetInfo.ModelNM, lsetInfo.ChipNM);

                //20171026コードレビュー指摘により不要となった為、削除。（閾値上位のみでカバー可と判断）
                ////TmPRMにdieKBが指定されていて、且つそれがファイル内の最大ドット数を越えている場合は処理除外。
                //if (string.IsNullOrWhiteSpace(prmData.DieKB) == false && Convert.ToInt32(prmData.DieKB) > maxDotNo) continue;

                //閾値マスタ情報(TmPLM)取得
                //PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
                Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.Plm.QcParamNO, false);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.Plm.QcParamNO, filefmtInfo.Plm.ParameterNM);
                    throw new Exception(message);
                }


                //必要ファイル内容取得
                FileValueInfo mFileValueInfo = null;

                mFileValueInfo
                    = GetFileValueMulti(fileLineValue, filefmtInfo.FileFmt.ColumnNO, plmInfo, mFilePath, lsetInfo, ref errMessageList);


                //異常判定+DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(mFileValueInfo.TargetStrVAL), mFileValueInfo.MeasureDT, ref errMessageList);
            }
        }

        private FileValueInfo GetFileValueMulti
    (string[] fileValueList, int needColumnNO, Plm plmInfo, string mFilePath, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
            FileValueInfo fileValueInfo = new FileValueInfo();
            List<double> targetValueList = new List<double>();

            List<General> extractSkipInfoList = new List<General>();


            for (int i = 0; i < fileValueList.Length; i++)  
            {
                int rowCount = i + 1; //行数
                string[] fileValue = fileValueList[i].Split(',');


                if (fileValue.Length < RESULT2_COLUMNS_NUM)
                {
                    //必要な文字長に満たない場合、メッセージ表示
                    string message = string.Format(Constant.MessageInfo.Message_45, mFilePath, rowCount);
                    ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
                    errMessageList.Add(errMessageInfo);

                    continue;
                }

                if (String.IsNullOrWhiteSpace(plmInfo.DieKB) == false && fileValue[RESULT2_DOTNUM_COLUMN] != plmInfo.DieKB)
                {
                    //ドット順が違うデータの場合、次に進める。ドット順の指定の無いパラメータなら全行取得する。
                    continue;
                }
                

                fileValueInfo.MeasureDT = File.GetLastWriteTime(mFilePath).ToString();
                targetValueList.Add(Convert.ToDouble(fileValue[needColumnNO]));
            }

            if(fileValueInfo.MeasureDT == null)
            {
                throw new ApplicationException($"該当ドットNoのデータが存在しません。ファイルを確認してください。ドットNo『{plmInfo.DieKB}』、ファイルパス『{mFilePath}』");
            }

            //計算区分(Total_KBに沿って計算。指定なしは不可。
            double fileVAL = double.MinValue;
            if (plmInfo.TotalKB == Constant.CalcType.AVE.ToString())
            {
                fileVAL = calcAvg(targetValueList);
            }
            else if (plmInfo.TotalKB == Constant.CalcType.SIGMA.ToString())
            {
                double fileAveVAL = calcAvg(targetValueList);
                fileVAL = calcSigma(targetValueList, fileAveVAL);
            }
            else if (plmInfo.TotalKB == Constant.CalcType.MAX.ToString())
            {
                fileVAL = CalcMax(targetValueList);
            }
            else if (plmInfo.TotalKB == Constant.CalcType.MIN.ToString())
            {
                fileVAL = CalcMin(targetValueList);
            }
            else if (plmInfo.TotalKB == Constant.CalcType.MODE.ToString())
            {
                fileVAL = CalcMode(targetValueList);
            }
            else if (plmInfo.TotalKB == Constant.CalcType.SUM.ToString())
            {
                fileVAL = CalcSum(targetValueList);
            }
            else
            {
                throw new ApplicationException($"計算区分が定義されていません。管理番号：『{plmInfo.QcParamNO}』,計算区分：{plmInfo.TotalKB}");
            }

            fileValueInfo.TargetStrVAL = Convert.ToString(fileVAL);

            return fileValueInfo;
        }

        /// <summary>
        /// 基板完了時にwed2、wed3、mpdファイルが揃っていることをチェック。（揃ってなければ例外エラー）
        /// </summary>
        /// <param name="endDirPath"></param>
        /// <param name="fileIdentity"></param>
        private void CheckSubstarteFileExist(string endFileDir, string targetFileIdentity)
        {
            //基板DMだけで前方一致検索するので誤取得の危険が無いとは言えないがそもそも基板DMの文字長は
            //決め打ち（長さが違うものがあったらそこでNGになる)なので問題ないと判断。
            List<string> targetFileList = Common.GetFiles(endFileDir, "^" + targetFileIdentity + ".*");

            List<string> searchList
                = targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), ".*Result2") == true).ToList();
            if (searchList.Count == 0)
            {
                throw new ApplicationException($"finファイルが出力されていますがResult2ファイルが存在しません。識別ID『{targetFileIdentity}』、パス『{endFileDir}』");
            }

            searchList = targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), ".*Result3") == true).ToList();
            if (searchList.Count == 0)
            {
                throw new ApplicationException($"finファイルが出力されていますがResult3ファイルが存在しません。識別ID『{targetFileIdentity}』、パス『{endFileDir}』");
            }

            searchList = targetFileList.Where(t => Regex.IsMatch(Path.GetFileName(t), ".*[.]mpd$") == true).ToList();
            if (searchList.Count == 0)
            {
                throw new ApplicationException($"finファイルが出力されていますがmpdファイルが存在しません。識別ID『{targetFileIdentity}』、パス『{endFileDir}』");
            }
        }

        /// <summary>
        /// SQ装置でResultとResult2を共存させるためのラップ関数。
        /// fin2ファイルが存在するときのみendingProcessとresponceOKFile(装置の信号を落としてOKファイルを移動する)を
        /// 実行する。
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="timingNo"></param>
        private void EndingProcessOnlyFin2File(LSETInfo lsetInfo, int timingNo)
        {
            string targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(EndFileDir, EXT_TRIG_FILE(), DateIndex, EndFileIdLen, true);

            //処理可能な出力ファイルが有れば、ファイル毎に処理する関数
            if (string.IsNullOrEmpty(targetFileIdentity) == false)
            {
                EndingProcess(lsetInfo, timingNo);
                ResponseOKFile(false, lsetInfo);
            }
        }

        //20171026コードレビュー指摘により不要となった為、削除。
        ///// <summary>
        ///// ファイルの中身をチェックし、最大のドット数を取得
        ///// </summary>
        ///// <param name="fileValueList"></param>
        ///// <returns></returns>
        //private int GetMaxDotNo(string[] fileValueList, string mFilePath)
        //{
        //    int maxDotNo = 0;
        //    for (int i = 1; i < fileValueList.Length; i++)
        //    {
        //        string[] fileValue = fileValueList[i].Split(',');

        //        if (fileValue.Length < RESULT2_COLUMNS_NUM)
        //        {
        //            string errMsg = $"ファイルからドットNo列が取得できません。{mFilePath}";
        //            throw new ApplicationException(errMsg);
        //        }

        //        if (Convert.ToInt32(fileValue[RESULT2_DOTNUM_COLUMN]) > maxDotNo)
        //        {
        //            maxDotNo = Convert.ToInt32(fileValue[RESULT2_DOTNUM_COLUMN]);
        //        }
        //    }

        //    if(maxDotNo == 0)
        //    {
        //        string errMsg = $"ファイルからドットNoが集計できません。{mFilePath}";
        //        throw new ApplicationException(errMsg);
        //    }

        //    return maxDotNo;
        //}
    }
}
