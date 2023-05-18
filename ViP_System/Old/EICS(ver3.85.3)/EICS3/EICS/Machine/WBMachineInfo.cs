
namespace EICS
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Drawing;
    using NascaAPI;
    using System.Text.RegularExpressions;
	using EICS.Machine;
	using System.Net.Sockets;
	using System.Threading;
	using SLCommonLib.DataBase;
	using EICS.Database;
	using EICS.Structure; 

    /// <summary>
    /// ワイヤーボンダー処理
    /// </summary>
    public class WBMachineInfo : MachineBase
    {
		/// <summary>ワイヤーボンダー装置名</summary>
		public const string ASSETS_NM = "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ";
		public const int DATETIME_CHAR_COUNT_IN_FILENM = 12;

        #region 定数

        public const string FOLDER_START_NM = "Start";
      
        public const string FOLDER_MAGAZINE_NM = "Magazine";

        /// <summary>実行フォルダ名</summary>
        public const string FOLDER_RUN_NM = "Run";

        /// <summary>ファイルMM内容開始行</summary>
        public const int FILE_MM_DATASTARTROW = 2;

		/// <summary>ファイルME内容開始行</summary>
		public const int FILE_ME_DATASTARTROW = 2;

		public const int FILE_ME_DATANOCOL = 0;

		public const int FILE_ME_ADDRESSCOL = 4;

		/// <summary>
		/// MEファイル内のNascaの起因CD、不良CD、分類CDデータが入る列(index値)
		/// </summary>
		public const int FILE_ME_NASCACAUSECOL = 12;
		public const int FILE_ME_NASCAERRORCOL = 13;
		public const int FILE_ME_NASCACLASSCOL = 14;

        /// <summary>ファイルMM内容列(ロギングアドレス)</summary>
        private const int FILE_MM_ADDRESSNO = 4;

        /// <summary>ファイルMM内容列(エラーCD)</summary>
        private const int FILE_MM_ERRORCD = 5;

        /// <summary>ファイルMM内容列(ユニットNO)</summary>
        private const int FILE_MM_UNITNO = 6;

        /// <summary>ファイルMM内容列(処理CD)</summary>
        private const int FILE_MM_TRANCD = 7;

        /// <summary>マッピング周辺検査識別文字</summary>
        public const string MAPPING_SINSP_KB = "S";

        /// <summary>マッピング部材交換免責識別文字</summary>
        public const string MAPPING_PARTSCHANGE_KB = "M";

        /// <summary>マッピング周辺検査数</summary>
        private const int MAPPING_SINSP_CT = 5;

		/// <summary>ファイル名にロットNoが付いている場合にファイル名を'_'で区切った際に必要な要素数</summary>
		/// ファイル名からロットNo付与済みかどうかを判断する為に使用
		private const int NEED_FILE_NAME_ELEMENT_TO_FIX_LOT = 4;

        /// <summary>ファイルMM内容列(エラーCD)不良マーク検出</summary>
        private const string FILE_MM_ERRORCD_BADMARK = "0x0018";

        /// <summary>ファイルMM内容列(エラーCD)不良マーク検出スキップ</summary>
        private const string FILE_MM_ERRORCD_BADMARKSKIP = "0x001E";

        /// <summary>
        /// MMファイル処理CD
        /// </summary>
        public enum MappingBaseTranCD : int 
        {
            /// <summary>処理無し</summary>
            None = 0,
            /// <summary>外観検査</summary>
            Inspection = 1, 
            /// <summary>周辺強度</summary>
            Strength = 2,
            /// <summary>目視検査</summary>
            Seeing = 3,
            /// <summary>検査無し</summary>
            NotInspection = 4
        }

        /// <summary>
        /// マッピングファイル検査優先度
        /// </summary>
        private enum MappingDataTranCD: int
        {
            /// <summary>処理無し</summary>
            None = 0,
            /// <summary>周辺検査</summary>
            Around = 1,
            /// <summary>部材交換免責</summary>
            PartsChange = 2,
            /// <summary>検査無し</summary>
            NotInspection = 3,
            /// <summary>外観検査</summary>
            Inspection = 4,
            /// <summary>周辺強度</summary>
            Strength = 5,
            /// <summary>目視検査</summary>
            Seeing = 6
        }

        /// <summary>
        /// パッケージ進行方向
        /// </summary>
        public enum Direction 
        {
            /// <summary>前</summary>
            Before,
            /// <summary>後</summary>
            After,
        }

        #endregion

		public WBMachineInfo()
		{
		}

		public WBMachineInfo(int lineCD, string equipNO)
		{
			this.LineCD = lineCD;
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			WaitForRenameByArmsFG = settingInfoPerLine.GetWaitForRenameByArmsFG(equipNO);
		}

		public override void InitFirstLoop(LSETInfo lsetInfo)
		{
			CheckFileFmtFromParamWhenInit(lsetInfo, true);
		}

        /// <summary>
        /// ファイルチェック
        /// </summary>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
			CheckDirectory(lsetInfo);

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			bool isOutputCIFSResult = settingInfoPerLine.IsOutputCIFSResult(lsetInfo.EquipmentNO);
#if Debug
        //開発者が確認する場合、保存先を変える
		lsetInfo.InputFolderNM = @"C:\QCIL\data\WB\" + lsetInfo.EquipmentNO + @"\";
#else
			if (settingInfo.KissFG == "ON")
			{
				if (!KLinkInfo.CheckKISS())
				{
					F01_MachineWatch.sp.PlayLooping();
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format(Constant.MessageInfo.Message_6, lsetInfo.InlineCD), Color.Red);
					base.errorMessageList.Add(errMessageInfo);
					base.machineStatus = Constant.MachineStatus.Stop;
					return;
				}
			}
#endif

            string mFolderPath = Path.Combine(lsetInfo.InputFolderNM, lsetInfo.DirWBMagazine);
            if (!Directory.Exists(mFolderPath)) 
            {
                //処理フォルダ(Magazine or Done)が無い場合、作成
                Directory.CreateDirectory(mFolderPath);
            }

            string sFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_START_NM);
            if (!Directory.Exists(sFolderPath))
            {
                //処理フォルダ(Start)が無い場合、作成
                Directory.CreateDirectory(sFolderPath);
            }

            string runDirPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_RUN_NM);
			if (!Directory.Exists(runDirPath))
			{
				//処理フォルダ(Run)が無い場合、作成
				Directory.CreateDirectory(runDirPath);
			}

			//Runフォルダ内の滞留ファイルを処理
			if (Directory.GetFiles(runDirPath).Length != 0)
			{
				CheckMagazineTiming(lsetInfo, ref base.errorMessageList);
			}

            //マガジン完了タイミングに出力される"MP"ファイルの存在確認をする
			List<string> mpFileList = MachineFile.GetPathList(mFolderPath, "^" + Convert.ToString(FileType.MP));
            if (mpFileList.Count != 0)
            {
                foreach (string mpFile in mpFileList)
                {
					if (WaitForRenameByArmsFG)
					{
						if (MPFile.IsLotFromFileName(mpFile) == false) continue;
					}
					UnchainFileProcess(lsetInfo, runDirPath, FileType.MP);

                    //処理フォルダ(Run)に最新ロットファイルを移動

					MoveRunFile(mpFile, mFolderPath, sFolderPath, settingInfo.WBMappingFG, isOutputCIFSResult, lsetInfo);

                    //最新ロットファイルの処理
                    CheckMagazineTiming(lsetInfo, ref base.errorMessageList);
                }
            }
            //スタートファイルを見つけた時の処理
            else
            {
                CheckStartTiming(lsetInfo, ref base.errorMessageList);
            }

			if (isOutputCIFSResult)
			{
				CheckStartTiming(lsetInfo, ref base.errorMessageList);
			}

			base.machineStatus = Constant.MachineStatus.Runtime;
        }


        /// <summary>
        /// レポートメッセージチェック
        /// </summary>
        /// <param name="lsetInfo">ライン設備情報</param>
        /// <param name="msgTypeCD">メッセージ種類番号(開始、完了)</param>
        /// <param name="reportList">レポートリスト</param>
        /// <param name="errMessageList">異常内容リスト</param>
        public bool CheckReportMessage(LSETInfo lsetInfo, MagInfo magInfo, string msgTypeCD, List<ReportMessageInfo> reportList, ref List<ErrMessageInfo> errMessageList) 
        {
			if (magInfo.sNascaLotNO == null)
			{
				Lott.SetTnLOTT(lsetInfo, "不明");
			}
			else
			{
				Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
			}

            //紐付けマスタ情報(TmMSGFMT)を取得
			List<MSGFMTInfo> fmtList = ConnectDB.GetMSGFMTData(msgTypeCD, lsetInfo.TypeCD, lsetInfo.ModelNM, lsetInfo.InlineCD);
            if (fmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, lsetInfo.TypeCD, msgTypeCD);
                throw new Exception(message);
            }

            AlertLog alertLog = AlertLog.GetInstance();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", true))
            {
                foreach (MSGFMTInfo fmtInfo in fmtList)
                {
                    //閾値マスタ情報(TmPLM)取得
					Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, fmtInfo.QcParamNO, lsetInfo.EquipmentNO, false);
                    if (plmInfo == null)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_28, lsetInfo.TypeCD, fmtInfo.QcParamNO, fmtInfo.ParameterNM);
                        throw new Exception(message);
                    }

                    //パラメータ値取得
                    string parameterVAL = getParameterValue(fmtInfo, reportList);

                    //パラメータ値判定
                    string errMessageVAL = ParameterInfo.CheckParameter(plmInfo, parameterVAL, lsetInfo, magInfo.sNascaLotNO);
                    if (!string.IsNullOrEmpty(errMessageVAL))
                    {
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(errMessageVAL, Color.Red);
                        errMessageInfo.MessageVAL = string.Format("[{0}/{1}号機]{2}", lsetInfo.AssetsNM, lsetInfo.MachineNM, errMessageInfo.MessageVAL);
                        errMessageList.Add(errMessageInfo);
                        alertLog.logMessageQue.Enqueue(errMessageInfo.MessageVAL);
                    }
                    else
                    {
                        //パラメータ値(内規)判定
                        string innerLimitErrMessage = ParameterInfo.CheckInnerLimit(plmInfo, parameterVAL, lsetInfo, magInfo.sNascaLotNO);
                        if (!string.IsNullOrEmpty(innerLimitErrMessage))
                        {
							F01_MachineWatch.sp.PlayLooping();

                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(innerLimitErrMessage, Color.Blue);
                            errMessageInfo.MessageVAL = string.Format("[{0}/{1}号機]{2}", lsetInfo.AssetsNM, lsetInfo.MachineNM, errMessageInfo.MessageVAL);
                            errMessageList.Add(errMessageInfo);
                            alertLog.logMessageQue.Enqueue(errMessageInfo.MessageVAL);
                        }
                    }

                    //履歴保存
                    Database.Log log = new Database.Log(conn);
                    log.GetInsertData(lsetInfo, magInfo, plmInfo, parameterVAL, errMessageVAL, System.DateTime.Now);
                    log.Insert();
                }
                conn.Commit();
            }

            //判定結果
            bool isJudgeResult = true;
            if (errMessageList.Count != 0)
            {
                isJudgeResult = false;
            }
            return isJudgeResult;
        }

        /// <summary>
        /// マガジンタイミング処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="errMessageList"></param>
        public virtual void CheckMagazineTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
            MagInfo magInfo = new MagInfo();
            
			bool isNotFoundMEFile = true;

            string runFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_RUN_NM);
			List<string> fileList = MachineFile.GetPathList(runFolderPath);
			if (fileList.Count == 0) return;

            //MPファイルの更新日時を取得
            string dtFileStamp = KLinkInfo.GetFileStampDT(runFolderPath, Convert.ToString(FileType.MP));//1マガジン単位ファイルのファイルスタンプ取得
            dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//公開API(装置・時間)の時間として使用(ファイルスタンプ-5分)

            List<string> mmFileList = fileList.Where(f => Regex.IsMatch(Path.GetFileNameWithoutExtension(f), @"^MM.*_.*_.*$")).ToList();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			SettingInfo commonSetting = SettingInfo.GetSingleton();

            if (commonSetting.IsMappingMode)
            {
                //MMファイルからマガジン情報取得
                magInfo = GetMagazineInfo(mmFileList[0]);
            }
            else
            {
				//ARMSからマガジン情報取得
				magInfo = GetMagInfo(lsetInfo, dtFileStamp, false);

				if (magInfo == null)
				{
					return;
				}
			}

            if (magInfo.sNascaLotNO == null)
            {
				Lott.SetTnLOTT(lsetInfo, "不明");
            }
            else
            {
				Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
            }

			//ファイル処理
            foreach (string file in fileList)
            {
                FileInfo fileInfo = new FileInfo(file);
				string dtMeasureDT = Convert.ToString(GetFileName_MeasureDT(fileInfo.Name));

                string fileChar = GetMachineFileChar(fileInfo.FullName, lsetInfo.AssetsNM);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] {0} WB {1}File", lsetInfo.EquipmentNO, fileChar));
                switch (fileChar) 
                {
					case "SP":
						List<ErrMessageInfo> errMsgList = DbInput_WB_SPFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, Constant.nMagazineTimming);
						errMessageList.AddRange(errMsgList);
						break;
                    case "MP":
                        DbInput_WB_MPFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, ref errMessageList);
                        break;
                    case "ML":
                        DbInput_WB_MLFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, ref errMessageList);
                        break;
                    case "MM":
                        break;
					case "ME":
						isNotFoundMEFile = false;
						if (settingInfoPerLine.IsOutputNasFile(lsetInfo.EquipmentNO))
						{
							DbInput_WB_MEFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, ref errMessageList);
						}
						else
						{
							ErrMessageInfo errMsg = new ErrMessageInfo(
								string.Format("ロット:{0}のMEファイルが出力されていますがエラー情報集計設定がされていません。不良を確認して下さい。ファイル:{1}"
								, magInfo.sNascaLotNO, fileInfo.FullName), Color.Red);
							errMessageList.Add(errMsg);
						}
						break;
                }
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] {0} WB {1}File", lsetInfo.EquipmentNO, fileChar));

                //処理済みファイルを保管フォルダへ移動
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
                string sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");
                MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, sFileStamp);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
            }

			if (settingInfoPerLine.IsOutputNasFile(lsetInfo.EquipmentNO) && isNotFoundMEFile)
			{
				ErrMessageInfo errMsg = new ErrMessageInfo(string.Format("ロット：{0} エラー情報集計ONですが、MEファイルが存在しません。不良を確認・登録して下さい。", magInfo.sNascaLotNO), Color.Red);
				errMessageList.Add(errMsg);
			}

            //QCNRに異常履歴を挿入
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]WB:CheckQC");
            CheckQC(lsetInfo, 5, magInfo.sMaterialCD);//5はWBの意味
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]WB:CheckQC");
        }

        /// <summary>
        /// スタートタイミング処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="errMessageList"></param>
        public void CheckStartTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
            MagInfo magInfo = new MagInfo();
            magInfo.sMagazineNO = "";
            magInfo.sNascaLotNO = null;

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
            magInfo.sMaterialCD = settingInfoPerLine.GetMaterialCD(lsetInfo.EquipmentNO);
			bool isOutputCIFSResult = settingInfoPerLine.IsOutputCIFSResult(lsetInfo.EquipmentNO);

            string sFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_START_NM);
			List<string> fileList = MachineFile.GetPathList(sFolderPath, string.Format("^{0}", "^" + Convert.ToString(FileType.SP)));

			DateTime? latestMeasureDT = null;
			string latestWstFile = string.Empty;

			List<DateTime> measureDtList = new List<DateTime>();

			if (isOutputCIFSResult)
			{
				List<string> wstFileList = Common.GetFiles(sFolderPath, string.Format(".*{0}$", CIFSBasedMachine.EXT_START_FILE));

				if (wstFileList.Count == 0)
				{
					return;
				}

				latestWstFile = wstFileList.OrderByDescending(w => Path.GetFileName(w)).ToList()[0];

				foreach (string wstFile in wstFileList)
				{
					FileInfo fileInfo = new FileInfo(wstFile);

					DateTime dt;

					string fileNm = Path.GetFileNameWithoutExtension(fileInfo.Name);
					if (DateTime.TryParseExact(fileNm, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.AssumeLocal, out dt) == false)
					{
						throw new ApplicationException(string.Format(
							"wstファイル名をyyyyMMddHHmmssの形式で日時変換出来ませんでした。変換対象:{0}", fileNm));
					}

					measureDtList.Add(dt);
				}

				latestMeasureDT = measureDtList.OrderByDescending(m => m).ToList()[0];
			}

            foreach (string sFile in fileList)
            {
                FileInfo fileInfo = new FileInfo(sFile);

				DateTime dtMeasureDT = GetFileName_MeasureDT(fileInfo.Name);

				if (isOutputCIFSResult && latestMeasureDT.HasValue && latestMeasureDT.Value != dtMeasureDT)
				{
					continue;
				}

                //SPファイル処理
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}File", lsetInfo.EquipmentNO, "SP"));
				List<ErrMessageInfo> errMsgList = DbInput_WB_SPFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT.ToString("yyyy/MM/dd HH:mm:ss"), Constant.nStartTimming);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}File", lsetInfo.EquipmentNO, "SP"));

				errMessageList.AddRange(errMsgList);

				if (isOutputCIFSResult)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}", lsetInfo.EquipmentNO, "Output CIFS Result"));
					OutputCIFSResult(latestWstFile, errMsgList, 256);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}", lsetInfo.EquipmentNO, "Output CIFS Result"));
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove file:{2}", lsetInfo.EquipmentNO, "SP", sFile));
					MoveCompleteMachineFile(fileInfo, dtMeasureDT);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove file:{2}", lsetInfo.EquipmentNO, "SP", sFile));
						
					continue;
					//CIFS仕様の場合はファイルリネームはせず、処理後に即ファイルを移動する。
				}

                //処理済みファイル名を変更する
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove", lsetInfo.EquipmentNO, "SP"));
				MoveCompleteMachineFile(fileInfo, dtMeasureDT);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove", lsetInfo.EquipmentNO, "SP"));
            }

			if (isOutputCIFSResult)
			{
				fileList = MachineFile.GetPathList(sFolderPath, string.Format("^{0}", "^" + Convert.ToString(FileType.SP)));

				if (fileList.Count() > 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("警告 {0} WB 全wstﾌｧｲﾙ処理後もﾌｧｲﾙ名の日時がwstに対応しないSPﾌｧｲﾙが残っています。", lsetInfo.EquipmentNO));

					foreach (string sFile in fileList)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0} WB 未処理ﾌｧｲﾙ{1}", lsetInfo.EquipmentNO, sFile));
					}
				}
			}
        }

		protected void OutputCIFSResult(string wstFilePath, List<ErrMessageInfo> errMsgList, int? msgMaxLen)
		{
			FileInfo fi;
			string resultFilePath;

			if (errMsgList.Count > 0)
			{
				resultFilePath = wstFilePath.Replace(string.Format(".{0}", CIFSBasedMachine.EXT_START_FILE), string.Format(".{0}", CIFS.EXT_NG_FILE.ToLower()));
				string errMessage = string.Join("\r\n", errMsgList.Select(e => e.MessageVAL));

				if (msgMaxLen.HasValue)
				{
					if(errMessage.Count() > msgMaxLen)

					errMessage = errMessage.Substring(0, msgMaxLen.Value);
				}

				fi = new FileInfo(wstFilePath);
				using (StreamWriter sw = fi.AppendText())
				{
					sw.Write(errMessage);
				}
				fi.MoveTo(resultFilePath);
			}
			else
			{
				resultFilePath = wstFilePath.Replace(string.Format(".{0}", CIFSBasedMachine.EXT_START_FILE), string.Format(".{0}", CIFS.EXT_OK_FILE));
				if (File.Exists(wstFilePath))
				{
					fi = new FileInfo(wstFilePath);
					
					fi.MoveTo(resultFilePath);
				}
				else
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
						string.Format("OK判定済みですがOKﾌｧｲﾙ出力時にwstﾌｧｲﾙ名が見つかりませんでした。認識済wstﾌｧｲﾙ名でOKﾌｧｲﾙを作成します。{0}", resultFilePath));

					fi = new FileInfo(resultFilePath);
					using (FileStream fs = fi.Create())
					{
					}
				}				
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("wstﾌｧｲﾙ名を変更:{0}⇒{1}", wstFilePath, resultFilePath));
		}

        #region 各ファイル処理

        /// <summary>
        /// SPファイル処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="MagInfoWB"></param>
        /// <param name="sWork"></param>
        /// <param name="dtMeasureDT"></param>
        /// <param name="nTimmingMode"></param>
        public List<ErrMessageInfo> DbInput_WB_SPFile_OLD(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, string dtMeasureDT, int nTimmingMode)
        {
			DateTime lastDt = new DateTime();
			string fileVAL = string.Empty;
			List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

            try
            {
				List<string> kissParamAllErrMsgList = new List<string>();
				
				lastDt = DateTime.Now;

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

				//3つのSPファイルを結合する・しない
				if (settingInfoPerLine.Disable3SPFilesSupportFunc(lsetInfo.EquipmentNO))
				{
					int exclusionTargetDigit = 2;//ファイル名から取得した日時文字列には"20"をシステムが先頭に付与しておりファイル名には20は存在せず西暦下二桁から始まる
					string fileSearchStr = dtMeasureDT.Substring(exclusionTargetDigit, dtMeasureDT.Length - exclusionTargetDigit);

					List<string> fileList = Common.GetFiles(Path.GetDirectoryName(sFilePath), fileSearchStr);

					fileVAL = string.Join("\r\n", GetJoinedMachineFiles(Path.GetDirectoryName(sFilePath), @".*\d{4}" + fileSearchStr + @".*[.].*"));
				}
				else
				{
					//全ファイル内容取得
					fileVAL = GetMachineFileValue(sFilePath);
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("GetMachineFileValue() {0}秒", (DateTime.Now - lastDt).TotalSeconds));

				lastDt = DateTime.Now;

				//新規処理　全パラ対応
				


				//旧の処理（関数化して外に出す）
                //SPファイルの紐付けマスタ情報(TmFILEFMT)を取得
				List<FILEFMTWBInfo> filefmtList = ConnectDB.GetFILEFMTWBData(Convert.ToString(FileType.SP), magInfo.sMaterialCD, lsetInfo.ModelNM, lsetInfo.InlineCD);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("GetFILEFMTWBData() {0}秒", (DateTime.Now - lastDt).TotalSeconds));

                if (filefmtList.Count == 0)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.SP));
                    throw new Exception(message);
                }
                foreach (FILEFMTWBInfo filefmtInfo in filefmtList)
                {
					List<string> kissParamErrMsgList = new List<string>();

					lastDt = DateTime.Now;

					Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Plm.GetData() {0}秒", (DateTime.Now - lastDt).TotalSeconds));

                    if (plmInfo == null)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                        throw new Exception(message);
                    }

					lastDt = DateTime.Now;

                    //値を取得
                    string fileItemVAL = GetKISSParam(filefmtInfo, fileVAL, out kissParamErrMsgList);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("GetKISSParam() {0}秒", (DateTime.Now - lastDt).TotalSeconds));

					if (kissParamErrMsgList.Count > 0)
					{
						kissParamAllErrMsgList.Add(string.Format("QcParamNo:{0} ModelNm:{1} \r\n{2}"
							, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, string.Join("\r\n", kissParamErrMsgList)));

						//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
						//	string.Format("ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} QcParamNo:{3} ModelNm:{4} FilePath:{5}\r\n{6}"
						//	, lsetInfo.InlineCD, lsetInfo.EquipmentNO, filefmtInfo.PrefixNM, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, sFilePath
						//	, string.Join("\r\n", kissParamErrMsgList)));
					}

					lastDt = DateTime.Now;

                    switch (nTimmingMode)
                    {
                        //異常判定
                        case Constant.nStartTimming:

							bool isOutputCIFSResult = settingInfoPerLine.IsOutputCIFSResult(lsetInfo.EquipmentNO);

							if (isOutputCIFSResult)
							{
								magInfo.sNascaLotNO = string.Empty;
								ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);

								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("nStartTiming:InsertTnLOG() {0}秒", (DateTime.Now - lastDt).TotalSeconds));
							}
							else
							{
								OutputErr(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);
								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("nStartTiming:OutputErr() {0}秒", (DateTime.Now - lastDt).TotalSeconds));
							}

                            break;
						//DB登録
						case Constant.nMagazineTimming:
							ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT);

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("nMagazineTimming:InsertTnLOG_NotOutputErr() {0}秒", (DateTime.Now - lastDt).TotalSeconds));
							break;
                        //異常判定+DB登録
                        case Constant.nStartTimmingNMC:
							ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("nStartTimmingNMC:InsertTnLOG() {0}秒", (DateTime.Now - lastDt).TotalSeconds));
                            break;
                        default:
                            throw new Exception(string.Format(Constant.MessageInfo.Message_47, sFilePath));
                    }
                }

				if (kissParamAllErrMsgList.Count > 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format(
						"ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} FilePath:{3}\r\n{4}", lsetInfo.InlineCD, lsetInfo.EquipmentNO, "ML", sFilePath
						, string.Join("\r\n", kissParamAllErrMsgList)));
				}
            }
            catch (Exception err)
            {
				ErrMessageInfo errMsgInfo = new ErrMessageInfo(string.Format("例外:{0}/ｽﾀｯｸﾄﾚｰｽ:{1}", err.Message, err.StackTrace), Color.Red);
				errMsgList.Add(errMsgInfo);
            }

			return errMsgList;
        }

		public string GetKissParamHeader(List<string> fileTxtList, string searchNm)
		{
			string targetRecord = string.Empty;

			targetRecord = fileTxtList.Find(f => f.Contains(searchNm));

			if (string.IsNullOrEmpty(targetRecord))
			{
				throw new ApplicationException(string.Format("ﾌｧｲﾙ中に検索文字が見つかりませんでした。検索文字:{0}", searchNm));
			}
			int firstCommaIndex = targetRecord.IndexOf(',');

			if (firstCommaIndex > 0)
			{
				return targetRecord.Substring(0, firstCommaIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		public string GetKissParamOnlyRecord(string kissParamHeader)
		{
			return string.Format("{0},,", kissParamHeader);
		}

		/// <summary>
		/// SPファイル処理
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="MagInfoWB"></param>
		/// <param name="sWork"></param>
		/// <param name="dtMeasureDT"></param>
		/// <param name="nTimmingMode"></param>
		public List<ErrMessageInfo> DbInput_WB_SPFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, string dtMeasureDT, int nTimmingMode)
		{
			List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
			try
			{
				List<string> kissParamErrMsgList = new List<string>();

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

				List<string> fileTxtList = GetMachineFileTxtList(sFilePath, DateTime.Now);

				if (settingInfoPerLine.GetFullParameterFG(lsetInfo.EquipmentNO))
				{
					List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

					FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

					//dtMeasureDT(西暦4桁）からファイル検索文字列を生成するが、ファイル名は西暦の下2桁から始まるので
					//dtMeasureDTの西暦の上2桁を省く為の変数
					//int exclusionTargetDigit = 2;

					//string fileSearchStr = dtMeasureDT.Substring(exclusionTargetDigit, dtMeasureDT.Length - exclusionTargetDigit);

					//fileTxtList = GetJoinedMachineFiles(Path.GetDirectoryName(sFilePath), @"^SP.*\d{4}" + fileSearchStr + @".*[.].*").ToList();

					//fileTxtList = GetMachineFileTxtList(sFilePath, DateTime.Now);

					DateTime fileCreationDt = File.GetCreationTime(sFilePath);
					List<Log> logList = new List<Log>();
					MachineFile macFile = new MachineFile();

					List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, true);

					List<FileFmtWithPlm> fileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);

					fileFmtWithPlmList = fileFmtWithPlmList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.Plm.QcParamNO));

					FileScan fileScan = FileScan.GetSingle(lsetInfo.InlineCD, lsetInfo.ModelNM, Convert.ToString(FileType.SP));

					macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, fileTxtList, MachineFile.TEXT_SPLITTER, magInfo.sMaterialCD, magInfo.sNascaLotNO, magInfo.sMagazineNO
						, fileCreationDt, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, null, null, ref logList, true);

					return errMsgList;
				}
				else
				{

					//全ファイル内容取得
					fileTxtList = GetMachineFileTxtList(sFilePath, DateTime.Now);

					//SPファイルの紐付けマスタ情報(TmFILEFMT)を取得
					List<FILEFMTWBInfo> filefmtList = ConnectDB.GetFILEFMTWBData(Convert.ToString(FileType.SP), magInfo.sMaterialCD, lsetInfo.ModelNM, lsetInfo.InlineCD);

					//List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM);
					List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

					FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

					filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

					List<string> searchNmList = filefmtList.Select(f => f.SearchNM).Distinct().ToList();

					Dictionary<string, List<string>> fileTxtPerSearchNmDict = new Dictionary<string, List<string>>();

					foreach (string searchNm in searchNmList)
					{
						try
						{
							List<string> fileTxtPerSearchNm = new List<string>();

							string kissParamHeader = GetKissParamHeader(fileTxtList, searchNm);

							if (string.IsNullOrEmpty(kissParamHeader))
							{
								fileTxtPerSearchNm = fileTxtList.FindAll(f => f.Contains(searchNm));
							}
							else
							{
								fileTxtPerSearchNm = fileTxtList.FindAll(f => f.Contains(searchNm) || f.Contains(kissParamHeader));
							}

							fileTxtPerSearchNmDict.Add(searchNm, fileTxtPerSearchNm);
						}
						catch (Exception err)
						{
							throw new Exception(string.Format("検索文字:{0} 関連行の抽出処理時にｴﾗｰ発生 InnerErrMsg:{1} StackTrace", searchNm, err.Message, err.StackTrace));
						}
					}

					if (filefmtList.Count == 0 && plmPerTypeModelChipList.Count != 0)
					{
						//設定されていない場合、装置処理停止
						string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.SP));
						throw new Exception(message);
					}

					//List<Plm> plmList = Plm.GetDatas(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, false);

					foreach (FILEFMTWBInfo filefmtInfo in filefmtList)
					{
						try
						{
							Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, plmPerTypeModelChipList, magInfo.sMaterialCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, filefmtInfo.QCParamNO);

							if (plmInfo == null)
							{
								//設定されていない場合、装置処理停止
								string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
								throw new Exception(message);
							}

							//値を取得
							string fileItemVAL = GetKISSParam(filefmtInfo, string.Join("\r\n", fileTxtPerSearchNmDict[filefmtInfo.SearchNM]), out kissParamErrMsgList);

							if (kissParamErrMsgList.Count > 0)
							{
								//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
								//	string.Format("ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} QcParamNo:{3} ModelNm:{4} FilePath:{5}\r\n{6}"
								//	, lsetInfo.InlineCD, lsetInfo.EquipmentNO, filefmtInfo.PrefixNM, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, sFilePath
								//	, string.Join("\r\n", kissParamErrMsgList)));

								AlertLog alertLog = AlertLog.GetInstance();

								foreach (string kissParamErrMsg in kissParamErrMsgList)
								{
									alertLog.logMessageQue.Enqueue(kissParamErrMsg);

									ErrMessageInfo kissErrMsgInfo = new ErrMessageInfo(kissParamErrMsg, Color.Red);

									errMsgList.Add(kissErrMsgInfo);
								}
							}

							switch (nTimmingMode)
							{
								//異常判定
								case Constant.nStartTimming:
									bool isOutputCIFSResult = settingInfoPerLine.IsOutputCIFSResult(lsetInfo.EquipmentNO);

									if (isOutputCIFSResult)
									{
										magInfo.sNascaLotNO = string.Empty;
										OutputErr(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);
									}
									else
									{
										OutputErr(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);
									}
									break;
								//DB登録
								case Constant.nMagazineTimming:
									ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT);
									break;
								//異常判定+DB登録
								case Constant.nStartTimmingNMC:
									ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileItemVAL.ToUpper(), dtMeasureDT, ref errMsgList);
									break;
								default:
									throw new Exception(string.Format(Constant.MessageInfo.Message_47, sFilePath));
							}
						}
						catch (Exception err)
						{
							throw new Exception(string.Format("WBﾌｧｲﾙ内紐付けﾏｽﾀに基づく順次判定処理においてｴﾗｰ発生 ﾏｽﾀ情報 装置型式:{0} 管理No:{1} ﾀｲﾌﾟ:{2} InnerErrMsg:{3} StackTrace:{4}", lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.TypeCD, err.Message, err.StackTrace));
						}
					}
				}
			}
			catch (Exception err)
			{
				ErrMessageInfo errMsgInfo = new ErrMessageInfo(string.Format("例外:{0}/ｽﾀｯｸﾄﾚｰｽ:{1}", err.Message, err.StackTrace), Color.Red);
				errMsgList.Add(errMsgInfo);
			}

			return errMsgList;
		}

        /// <summary>
        /// MPファイル処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="MagInfoWB"></param>
        /// <param name="sWork"></param>
        /// <param name="dtMeasureDT"></param>
        public void DbInput_WB_MPFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        {
			List<string> kissParamAllErrMsgList = new List<string>();
			string currentStr = string.Empty;

            try
            {
                //全ファイル内容取得
                string fileVAL = GetMachineFileValue(sFilePath);

                //MPファイルの紐付けマスタ情報(TmFILEFMT)を取得
                List<FILEFMTWBInfo> filefmtList = ConnectDB.GetFILEFMTWBData(Convert.ToString(FileType.MP), magInfo.sMaterialCD, lsetInfo.ModelNM, lsetInfo.InlineCD);

				List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

				FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

				filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

				if (filefmtList.Count == 0 && plmPerTypeModelChipList.Count != 0)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.MP));
                    throw new Exception(message);
                }
                
                foreach (FILEFMTWBInfo filefmtInfo in filefmtList)
                {
					List<string> kissParamErrMsgList = new List<string>();
					currentStr = filefmtInfo.ParameterNM;

                    //閾値マスタ情報(TmPLM)取得
					Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

                    if (plmInfo == null)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                        throw new Exception(message);
                    }

                    //標準的な処理
                    if (plmInfo.TotalKB != Constant.CalcType.SP.ToString())
                    {
                        string fileValue = GetKISSParam(filefmtInfo, fileVAL, out kissParamErrMsgList);

						if (kissParamErrMsgList.Count > 0)
						{
							kissParamAllErrMsgList.Add(string.Format("QcParamNo:{0} ModelNm:{1} \r\n{2}"
								, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, string.Join("\r\n", kissParamErrMsgList)));
							//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
							//	string.Format("ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} QcParamNo:{3} ModelNm:{4} FilePath:{5}\r\n{6}"
							//	, lsetInfo.InlineCD, lsetInfo.EquipmentNO, filefmtInfo.PrefixNM, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, sFilePath
							//	, string.Join("\r\n", kissParamErrMsgList)));
						}

                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValue, dtMeasureDT, ref errMessageList);
                    }

                    //特殊な処理
                    else
                    {
                        switch (filefmtInfo.ParameterNM) 
                        {
                            //ﾜｲﾔｶｯﾄｴﾗｰ、ｽﾊﾟｰｸﾐｽの場合、検索値を集計
                            case Constant.sWB_WireCutError:
                            case Constant.sWB_SparkMiss:

                                int paramSumVAL = 0;

                                int paramCount = GetKISSParam_Cnt(fileVAL, filefmtInfo.SearchNM);
                                for (int i = 0; i < paramCount; i++)
                                {
									string kissParam = GetKISSParam_Detail(fileVAL, filefmtInfo.SearchNM, filefmtInfo.SearchNO + i, filefmtInfo.Comma_NO, out kissParamErrMsgList);
									int kissParamNumVal;

									if (int.TryParse(kissParam, out kissParamNumVal) == false)
									{
										throw new ApplicationException(
											string.Format("設備:{0} 取得した値が数字では無く、数値へ変換できない文字です。ﾏｽﾀ設定かﾌｧｲﾙの内容に問題があります。SearchNM:{1} SearchNO:{2} CommaNO:{3} 取得文字:{4}"
											, lsetInfo.EquipmentNO, filefmtInfo.SearchNM, filefmtInfo.SearchNO, filefmtInfo.Comma_NO, kissParam));
									}

									paramSumVAL += kissParamNumVal;

									if (kissParamErrMsgList.Count > 0)
									{
										kissParamAllErrMsgList.Add(string.Format("QcParamNo:{0} ModelNm:{1} \r\n{2}"
											, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, string.Join("\r\n", kissParamErrMsgList)));

										//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
										//	string.Format("ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} QcParamNo:{3} ModelNm:{4} FilePath:{5}\r\n{6}"
										//	, lsetInfo.InlineCD, lsetInfo.EquipmentNO, filefmtInfo.PrefixNM, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, sFilePath
										//	, string.Join("\r\n", kissParamErrMsgList)));
									}
                                }
                                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, paramSumVAL.ToString(), dtMeasureDT, ref errMessageList);

                                break;
                            default:
                                throw new Exception(string.Format(Constant.MessageInfo.Message_48, sFilePath));
                        }
                    }
                }

				if (kissParamAllErrMsgList.Count > 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format(
						"ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} FilePath:{3}\r\n{4}", lsetInfo.InlineCD, lsetInfo.EquipmentNO, "ML", sFilePath
						, string.Join("\r\n", kissParamAllErrMsgList)));
				}
            }
            catch (Exception err) 
            {
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format("{0} MP処理 {1}でException発生 例外:{2}", lsetInfo.EquipmentNO, currentStr, err.ToString()));
                throw;
            }
        }

        /// <summary>
        /// MLファイル処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="MagInfoWB"></param>
        /// <param name="sWork"></param>
        /// <param name="dtMeasureDT"></param>
        public void DbInput_WB_MLFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        {
			List<string> kissParamAllErrMsgList = new List<string>();
            //全ファイル内容取得
            string fileVAL = GetMachineFileValue(sFilePath);

            //MLファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTWBInfo> filefmtList = ConnectDB.GetFILEFMTWBData(Convert.ToString(FileType.ML), magInfo.sMaterialCD, lsetInfo.ModelNM, lsetInfo.InlineCD);

			List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			if (filefmtList.Count == 0 && plmPerTypeModelChipList.Count != 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.ML));
                throw new Exception(message);
            }

            foreach (FILEFMTWBInfo filefmtInfo in filefmtList)
            {
				List<string> kissParamErrMsgList = new List<string>();
                //閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, lsetInfo.EquipmentNO, magInfo.sMaterialCD, this.LineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                //標準的な処理
                if (plmInfo.TotalKB != Constant.CalcType.SP.ToString())
                {
                    string fileValue = GetKISSParam(filefmtInfo, fileVAL, out kissParamErrMsgList);

					if (kissParamErrMsgList.Count > 0)
					{
						kissParamAllErrMsgList.Add(string.Format("QcParamNo:{0} ModelNm:{1} \r\n{2}"
								, filefmtInfo.QCParamNO, filefmtInfo.Model_NM, string.Join("\r\n", kissParamErrMsgList)));
					}

                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValue, dtMeasureDT, ref errMessageList);
                }

                //前ｼｽﾃﾑからの特殊な処理
                else
                {
                    if (filefmtInfo.ParameterNM == Constant.sWB_WorkVacuumvAcuum)
                    {
                        List<FileValueInfo> fileValueList = GetWBWorkVacuumvAcuumValueList(fileVAL, plmInfo.ParameterVAL);
                        foreach (FileValueInfo fileValueInfo in fileValueList)
                        {
                            ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                        }
                    }
                }
            }

			if (kissParamAllErrMsgList.Count > 0)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format(
					"ﾗｲﾝNo:{0} 設備:{1} Prefix:{2} FilePath:{3}\r\n{4}", lsetInfo.InlineCD, lsetInfo.EquipmentNO, "ML", sFilePath
					, string.Join("\r\n", kissParamAllErrMsgList)));
			}
        }

		public bool DbInput_WB_MEFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
		{
			bool isCompleteNasFile = false;
			string rowStr = "";
			int machineTroubleDataCount = 0;
			int rowIndex = 0;
			List<NascaDefectFile.DefectAddrInfo> defectAddrList = new List<NascaDefectFile.DefectAddrInfo>();
			int fileLineNum = -1;
			try
			{
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

				//全ファイル内容取得(複数行)
				string[] fileLineValue = GetMachineFileLineValue(sFilePath);

				fileLineNum = fileLineValue.Length;

				for (rowIndex = FILE_ME_DATASTARTROW; rowIndex < fileLineNum; rowIndex++)
				{
					try
					{
						int dataNo, addr;

						rowStr = fileLineValue[rowIndex];
						string[] colData = rowStr.Split(',');

						if (colData.Length <= FILE_ME_NASCACLASSCOL)
						{
							machineTroubleDataCount++;

							if (machineTroubleDataCount <= 10)
							{
								MEFileProcessExceptionalLogOut("【装置出力異常】ファイルの列数が不足しています"
									, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, rowStr, sFilePath, ref errMessageList);
							}
							continue;
						}

						bool parseDataNoFg = int.TryParse(colData[FILE_ME_DATANOCOL].Trim(), out dataNo);
						bool parseAddrFg = int.TryParse(colData[FILE_ME_ADDRESSCOL].Trim(), out addr);

						NascaDefectFile.DefectAddrInfo defectAddrInfo = new NascaDefectFile.DefectAddrInfo();

						defectAddrInfo.CauseCd = colData[FILE_ME_NASCACAUSECOL].Trim();
						defectAddrInfo.ErrCd = colData[FILE_ME_NASCAERRORCOL].Trim();
						defectAddrInfo.ClassCd = colData[FILE_ME_NASCACLASSCOL].Trim();

						if (parseDataNoFg == false || parseAddrFg == false)
						{
							MEFileProcessLogOut("【装置出力異常】DATA No,ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽは数字の必要があります"
								, colData[FILE_ME_DATANOCOL], colData[FILE_ME_ADDRESSCOL], defectAddrInfo, lsetInfo.EquipmentNO, magInfo.sNascaLotNO
								, magInfo.sMagazineNO, sFilePath, ref errMessageList);
						}

						if (parseAddrFg)
						{
							defectAddrInfo.Addr = addr;
						}
						else
						{
							//アドレス0の不良は集計しない
							defectAddrInfo.Addr = 0;
						}

						//不良CD、起因CD、分類CDのどれか一つでも値が無ければログ出力
						if (string.IsNullOrEmpty(defectAddrInfo.ErrCd) || string.IsNullOrEmpty(defectAddrInfo.CauseCd) || string.IsNullOrEmpty(defectAddrInfo.ClassCd))
						{
							if (string.IsNullOrEmpty(defectAddrInfo.ErrCd) && string.IsNullOrEmpty(defectAddrInfo.CauseCd) && string.IsNullOrEmpty(defectAddrInfo.ClassCd))
							{
								//不良CD、起因CD、分類CDすべてが空白で有れば単にエラー無しのレコードで有る為、スルー
							}
							else
							{
								MEFileProcessLogOut("【装置出力異常】ファイル内のデータが欠損しています。不良を確認して下さい。"
									, colData[FILE_ME_DATANOCOL].Trim(), colData[FILE_ME_ADDRESSCOL].Trim(), defectAddrInfo, lsetInfo.EquipmentNO
									, magInfo.sNascaLotNO, magInfo.sMagazineNO, sFilePath, ref errMessageList);
							}
							continue;
						}

						//defectAddrList.Add(defectAddrInfo);

						int targetIndex = defectAddrList.FindIndex(d => d.Addr == defectAddrInfo.Addr);
						if (targetIndex == -1)
						{
							defectAddrList.Add(defectAddrInfo);
						}
						else
						{
							//同一アドレスデータがある場合、出力が後のエラーを優先する
							if (string.IsNullOrEmpty(defectAddrInfo.ErrCd) == false)
							{
								defectAddrList[targetIndex] = defectAddrInfo;
							}
						}
					}
					catch (Exception err)
					{
						MEFileProcessExceptionalLogOut("不良数集計にて想定外エラー発生（エラー発生行をスキップし、処理継続）"
							, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, rowStr, sFilePath, ref errMessageList);

						continue;
					}
				}
			}
			catch (Exception err)
			{
				MEFileProcessExceptionalLogOut("不良数集計にて想定外エラー発生（マッピングファイルを出力する為、処理継続）"
					, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, rowStr, sFilePath, ref errMessageList);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format("Msg:{0}\r\nStackTrace:{1}", err.Message, err.StackTrace));
			}
			finally
			{
				if (machineTroubleDataCount > 10)
				{
					MEFileProcessExceptionalLogOut(string.Format("【装置出力異常】列数が不足していたデータ数:{0}行", machineTroubleDataCount)
						, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, rowStr, sFilePath, ref errMessageList);
				}
			}

			//アドレス毎の不良情報を不良情報毎の数量情報にまとめて、nasファイル出力する
			try
			{
				MEFileRunningLogOut(string.Format("データ抽出 {0}行中 {1}行完了", rowIndex, fileLineNum)
					, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, "-", sFilePath);

				List<NascaDefectFile.DefectQtyInfo> defQtyList = NascaDefectFile.GetDefectQtyList(defectAddrList);

				NascaDefectFile.Create(magInfo.sNascaLotNO, magInfo.sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, defQtyList);

				isCompleteNasFile = true;
			}
			catch (Exception err)
			{
				MEFileProcessExceptionalLogOut("WB不良自動入力用ファイルの作成時に想定外エラー発生"
					, lsetInfo.EquipmentNO, magInfo.sNascaLotNO, magInfo.sMagazineNO, "-", sFilePath, ref errMessageList);
			}

			return isCompleteNasFile;
		}

		private void MEFileRunningLogOut(string msg, string plantCd, string lotNo, string magNo, string rowData, string filePath)
		{
			RunningLog runLog = RunningLog.GetInstance();

			string errStr = string.Format("設備:{0}/ロット:{1}/マガジン:{2}/MEFile:{3} 行内容:{4} ファイル:{5}"
				, plantCd, lotNo, magNo, msg, rowData, filePath);

			runLog.logMessageQue.Enqueue(errStr);
		}

		private void MEFileProcessExceptionalLogOut(string msg, string plantCd, string lotNo, string magNo, string rowData, string filePath, ref List<ErrMessageInfo> errMessageList)
		{
			string errStr = string.Format("設備:{0}/ロット:{1}/マガジン:{2}/MEFile:{3} 行内容:{4} ファイル:{5}"
				, plantCd, lotNo, magNo, msg, rowData, filePath);

			errMessageList.Add(new ErrMessageInfo(errStr, Color.Red));
		}

		private void MEFileProcessLogOut(string msg, string dataNo, string addr, NascaDefectFile.DefectAddrInfo defect, string plantCd, string lotNo, string magNo, string filePath, ref List<ErrMessageInfo> errMessageList)
		{
			string errStr = string.Format("設備:{0}/ロット:{1}/マガジン:{2}/MEFile:{3} dataNo:{4}/addr:{5}/不良CD:{6}/起因CD:{7}/分類CD:{8} ファイル:{9}"
				, plantCd, lotNo, magNo, dataNo, msg, addr, defect.ErrCd, defect.CauseCd, defect.ClassCd, filePath);

			errMessageList.Add(new ErrMessageInfo(errStr, Color.Red));
		}

        /// <summary>
        /// WBワーク真空吸着設定値を取得する
        /// </summary>
        /// <param name="sWork"></param>
        /// <param name="tSearchNM"></param>
        /// <returns></returns>
        public static List<FileValueInfo> GetWBWorkVacuumvAcuumValueList(string sWork, string tSearchNM)
        {
            List<FileValueInfo> rFileValueList = new List<FileValueInfo>();
            
            string[] rowList = sWork.Split('\n');
            foreach (string row in rowList)
            {
                if (row.Length == 0)
                {
                    continue;
                }

				if (row == "\r")
				{
					continue;
				}

                string fSearchNM = Convert.ToString(row.Split(',')[3]);
                if (fSearchNM == tSearchNM)
                {
                    string fValue = Convert.ToString(row.Split(',')[4]);
                    long fMeasureDT = Convert.ToInt64("20" + row.Split(',')[0] + row.Split(',')[1]);

                    int startCharIndex = fValue.LastIndexOf('[') + 1;
                    int endCharIndex = fValue.LastIndexOf(']');

                    FileValueInfo fileValueInfo = new FileValueInfo();
                    fileValueInfo.MeasureDT = Convert.ToString(KLinkInfo.ParseDate(fMeasureDT));
                    fileValueInfo.TargetStrVAL = fValue.Substring(startCharIndex, endCharIndex - startCharIndex).Trim();
                    rFileValueList.Add(fileValueInfo);
                }
            }

            return rFileValueList;
        }

        #endregion

        /// <summary>
        /// レポートから紐付け位置のパラメータ値を取得    
        /// </summary>
        /// <param name="fmtInfo">紐付け情報</param>
        /// <param name="reportList">レポート</param>
        /// <returns>パラメータ値</returns>
        private static string getParameterValue(MSGFMTInfo fmtInfo, List<ReportMessageInfo> reportList) 
        {
            string parameterVAL = string.Empty;

            if (fmtInfo.SearchGrpParamNO != 0)//KAIJOが新川の出力フォーマットから1層増えた為の追加処理
            {
                reportList = GetGrpParameterValue(fmtInfo.SearchGrpParamNO, reportList);//指定したパラメータグループのリストを取得する。
            }

            if (reportList.Count < fmtInfo.SearchParamNO)
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_78, fmtInfo.QcParamNO));
            }
            int searchIndex = fmtInfo.SearchParamNO - 1;
            ReportMessageInfo reportInfo = reportList[searchIndex];

            if (string.IsNullOrEmpty(fmtInfo.SearchParamNM) && fmtInfo.SearchValueNO == 0)
            {
                //SearchParamNO単体の検索
                parameterVAL = reportInfo.ParameterVAL.ToString();
            }
            else
            {
                //SearchParamNMの検索
                int targetIndex = reportInfo.MultiValueList.FindIndex(m => m.ParameterVAL.ToString() == fmtInfo.SearchParamNM);
                if (targetIndex == -1)
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_78, fmtInfo.QcParamNO));
                }

                //検索文字項目の次項目に値項目が存在する為 + 1 する
                parameterVAL = reportInfo.MultiValueList[targetIndex + 1].ParameterVAL.ToString();

                if (fmtInfo.SearchValueNO != 0)
                {
                    //SearchValueNOの検索
                    List<string> parameterList = parameterVAL.Split(',').ToList();
                    if (parameterList.Count < fmtInfo.SearchValueNO)
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_78, fmtInfo.QcParamNO));
                    }

                    int searchValueIndex = fmtInfo.SearchValueNO - 1;
                    parameterVAL = parameterList[searchValueIndex];
                }
            }

            return parameterVAL;

        }

		private static List<ReportMessageInfo> GetGrpParameterValue(int searchGrpParamNO, List<ReportMessageInfo> reportList)
		{
			return reportList[searchGrpParamNO-1].MultiValueList;
		}

		#region KISS
		//KISSが出力するﾌｧｲﾙ名から測定日時を返す
		public static DateTime GetFileName_MeasureDT(string sfname)
		{
			string sYear, sMonth, sDay, sHour, sMinute, sSecond;
			DateTime dtMeasureDT = Convert.ToDateTime("9999/01/01");

			//途中ファイルが来るケースがあり、その場合、_○○○○くらいのファイル名になる為、未定義エラーが発生する。
			//文字列長を調べて、ApplicationExceptionを投げるようにする。(2013/12/26 n.yoshimoto)
			//他のクラスで似たような処理が無いかもチェックする
			sSecond = "00";

			//時間はﾌｧｲﾙ名にしか書かれていないので、ﾌｧｲﾙ名から取得する。
			if (sfname.Substring(0, 1) == "_")
			{
				sYear = "20" + sfname.Substring(7, 2);
				sMonth = sfname.Substring(9, 2);
				sDay = sfname.Substring(11, 2);
				sHour = sfname.Substring(13, 2);
				sMinute = sfname.Substring(15, 2);
			}
			else
			{
				sYear = "20" + sfname.Substring(6, 2);
				sMonth = sfname.Substring(8, 2);
				sDay = sfname.Substring(10, 2);
				sHour = sfname.Substring(12, 2);
				sMinute = sfname.Substring(14, 2);
				if (sfname.Count() >= 22)
				{
					sSecond = sfname.Substring(16, 2);
				}
			}
			dtMeasureDT = Convert.ToDateTime(sYear + "/" + sMonth + "/" + sDay + " " + sHour + ":" + sMinute + ":" + sSecond);

			return dtMeasureDT;
		}

		/// <summary>
		/// KISS出力ファイルと項目名を入れると、項目の値を返す
		/// </summary>
		/// <param name="sParamNm"></param>
		/// <returns></returns>
		public static string GetKISSParam_PrgNM(string sKissOutput, string sParamNm)
        {
            string sParam = "";

            int npos = -1;
            int nStartPos, nEndPos;
            while (true)
            {
				if (npos >= sKissOutput.Length)
				{
					break;
				}

                //全角・半角が違えば、検索出来ないので注意。
                npos = sKissOutput.IndexOf(sParamNm, npos + 1);//見つからなかった場合-1を返す
                if (npos == -1)
                {
                    break;
                }
                else//項目が見つかった場合、[]の中身を取得
                {
                    nStartPos = npos + sParamNm.Length;
                    npos = sKissOutput.IndexOf("\r\n", npos + 1);

					if (npos < 0)
					{
						npos = sKissOutput.Length;
					}

                    nEndPos = npos;

                    sParam = sKissOutput.Substring(nStartPos, nEndPos - nStartPos).Trim();
                }
            }
            return sParam;
        }

        /// <summary>
        ///「KISS出力ファイル」、「項目名」、「何番目に発見された項目の値を取得するか」を入れると、項目の値を返す
        /// </summary>
        /// <param name="sParamNm"></param>
        /// <returns></returns>
        public static string GetKISSParam_Select(string sKissOutput, string sParamNm, int nNum)
        {
            string sParam = "";

            int npos = -1;
            int nStartPos, nEndPos;
            int nw = 0;

            while (true)
            {
                nw = nw + 1;
                //全角・半角が違えば、検索出来ないので注意。

				if (npos >= sKissOutput.Length)
				{
					break;
				}
                npos = sKissOutput.IndexOf(sParamNm, npos + 1);//見つからなかった場合-1を返す
                if (npos == -1)
                {
                    break;
                }
                else//項目が見つかった場合、[]の中身を取得
                {
                    if (nw == nNum)
                    {
                        npos = sKissOutput.IndexOf(",", npos + 1);
                        nStartPos = npos + 1;
                        npos = sKissOutput.IndexOf(",", npos + 1);

						if (npos < 0)
						{
							npos = sKissOutput.Length;
						}

                        nEndPos = npos;

                        sParam = sKissOutput.Substring(nStartPos, nEndPos - nStartPos).Trim();
                    }
                }
            }
            return sParam;
        }

        /// <summary>
        ///「KISS出力ファイル」、「項目名」、「何番目に発見された項目の値を取得するか」、「発見された項目から何カンマ後の値を取得するか」を入れると、項目の値を返す
        /// </summary>
        /// <param name="sParamNm"></param>
        /// <returns></returns>
        public static string GetKISSParam_Detail(string sKissOutput, string sParamNm, int nNum1, int nNum2, out List<string> errMsgList)
        {
			errMsgList = new List<string>();
            string sParam = "";

            int npos = -1;
            int nStartPos = 0, nEndPos = 0;
            int nw1 = 0, nw2 = 0;

            while (true)
            {
                nw1 = nw1 + 1;

				if (npos >= sKissOutput.Length)
				{
					break;
				}
                //全角・半角が違えば、検索出来ないので注意。
                npos = sKissOutput.IndexOf(sParamNm, npos + 1);//見つからなかった場合-1を返す
                if (npos == -1)
                {
                    break;
                }
                else//項目が見つかった場合、[]の中身を取得
                {
                    if (nw1 == nNum1)
                    {
                        for (nw2 = 0; nw2 < nNum2; nw2++)
                        {
                            npos = npos + 1;
                            npos = sKissOutput.IndexOf(",", npos);
                            nStartPos = npos + 1;
                        }
                        npos = sKissOutput.IndexOf(",", npos + 1);

						if (npos < 0)
						{
							npos = sKissOutput.Length;
						}

                        nEndPos = npos;

                        sParam = sKissOutput.Substring(nStartPos, nEndPos - nStartPos).Trim();

                        //改行があれば、改行までの文字列を返す
                        npos = -1;
                        npos = sParam.IndexOf("\r\n", npos + 1);
                        if (npos > 0)
                        {
                            sParam = sParam.Substring(0, npos).Trim();
                        }
                        break;
                    }
                }
            }

			if (sParam == string.Empty && npos < 0)
			{
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, string.Format("KISSパラメタ取得処理 パラメタが見つかりません。 検索対象：{0} SearchNo：{1} CommaNo：{2}", sParamNm, nNum1, nNum2));
				errMsgList.Add(string.Format("KISSﾊﾟﾗﾒﾀ取得処理 ﾊﾟﾗﾒﾀが見つかりません。 検索対象：{0} SearchNo(検索対象ﾋｯﾄ回数)：{1} CommaNo：{2}", sParamNm, nNum1, nNum2));
			}

            return sParam;
        }

        /// <summary>
        ///「KISS出力ファイル」、「項目名」を入れると、「項目名」が発見された数を返す
        /// </summary>
        /// <param name="sParamNm"></param>
        /// <returns></returns>
        public static int GetKISSParam_Cnt(string sKissOutput, string sParamNm)
        {
            int npos = -1;
            int nCnt = 0;

            while (true)
            {
                //全角・半角が違えば、検索出来ないので注意。
                npos = sKissOutput.IndexOf(sParamNm, npos + 1);//見つからなかった場合-1を返す
                if (npos == -1)
                {
                    break;
                }
                else//項目が見つかった場合、[]の中身を取得
                {
                    nCnt = nCnt + 1;
                }
            }
            return nCnt;
        }

        /// <summary>
        /// 「KISS出力ファイル」
        /// </summary>
        /// <param name="filefmtInfo"></param>
        /// <param name="sWork"></param>
        /// <returns></returns>
        public static string GetKISSParam(FILEFMTWBInfo filefmtInfo, string sWork, out List<string> errMsgList)
        {
			errMsgList = new List<string>();
            string sParam = "";

            //値の取得関数
            if (filefmtInfo.FunctionNO == (int)Constant.FunctionNO.Value)
            {
                sParam = GetKISSParam_PrgNM(sWork, filefmtInfo.SearchNM);                   //<文字> ﾜｲﾔｰﾎﾞﾝﾄﾞﾌﾟﾛｸﾞﾗﾑ番号(本体)→"品種名      ："

				if (string.IsNullOrEmpty(sParam))
				{
					errMsgList.Add(string.Format("KISSﾊﾟﾗﾒﾀ取得処理 ﾊﾟﾗﾒﾀが見つかりません。 FunctionNo:{0} 検索対象:{1} ", filefmtInfo.FunctionNO, filefmtInfo.SearchNM));
				}
            }
            else if (filefmtInfo.FunctionNO == (int)Constant.FunctionNO.Select)
            {
                sParam = GetKISSParam_Select(sWork, filefmtInfo.SearchNM, filefmtInfo.SearchNO);

				if (string.IsNullOrEmpty(sParam))
				{
					errMsgList.Add(string.Format("KISSﾊﾟﾗﾒﾀ取得処理 ﾊﾟﾗﾒﾀが見つかりません。 FunctionNo:{0} 検索対象:{1} SearchNo(検索対象ﾋｯﾄ回数):{2}", filefmtInfo.FunctionNO, filefmtInfo.SearchNM, filefmtInfo.SearchNO));
				}
            }
            else if (filefmtInfo.FunctionNO == (int)Constant.FunctionNO.Detail)
            {
                sParam = GetKISSParam_Detail(sWork, filefmtInfo.SearchNM, filefmtInfo.SearchNO, filefmtInfo.Comma_NO, out errMsgList);
            }

            return sParam;
        }

        #endregion

		/// <summary>
		/// 処理をするファイルを処理中フォルダに移動する(WB)
		/// </summary>
		public void MoveRunFile(string mpFilePath, string mpFolderPath, string spFolderPath, bool isMapping, bool isOutputCIFSResult, LSETInfo lsetInfo)
		{
			FileInfo mpFileInfo = new FileInfo(mpFilePath);

			string runFolderPath = Path.Combine(mpFileInfo.Directory.Parent.FullName, FOLDER_RUN_NM);
			if (!Directory.Exists(runFolderPath))
			{
				Directory.CreateDirectory(runFolderPath);
			}

			DirectoryInfo mpFolder = new DirectoryInfo(mpFolderPath);
			FileInfo[] fileList = mpFolder.GetFiles("*");

			//Magazineフォルダ内にある1マガジン分のファイル(指定のMPファイルより更新日時が古いファイル)をRunフォルダへ移動
			FileInfo[] magazineFiles = fileList.Where(f => mpFileInfo.LastWriteTime >= f.LastWriteTime).ToArray();
			if (isMapping)
			{
				FileInfo[] mmFiles = magazineFiles.Where(m => Regex.IsMatch(m.Name, "^MM.*$")).ToArray();
				if (mmFiles.Count() == 0)
					throw new ApplicationException(string.Format("マッピング機能が有効でMMファイルの未出力が発生しました。出力フォルダを確認して下さい。{0}", mpFolderPath));
				if (mmFiles.Count() > 1)
					throw new ApplicationException(string.Format("複数のMMファイルが出力されています。出力フォルダを確認して下さい。{0}", mpFolderPath));
				//if (mmFiles.Single().LastWriteTime < System.DateTime.Now.AddHours(-1))
				//    throw new ApplicationException(string.Format("マガジン完了後、MMファイル名にロット情報が付与されず1時間が経過しました。出力フォルダを確認して下さい。{0}", mpFolderPath));

				if (MMFile.IsLotFromFileName(mmFiles.Single()) == false)
				{
					//MMファイル名にロット情報が付与されていない間は待機
					return;
				}

				MMFile mmFile = new MMFile(mmFiles.Single());
				if (Database.LENS.WorkResult.IsComplete(mmFile.LotNo, mmFile.ProcNo, lsetInfo.InlineCD) == false)
				{
					//LENSの処理が完了していない間は待機
					return;
				}
			}
			foreach (FileInfo file in magazineFiles)
			{
				try
				{
					if (file.Name.StartsWith("_"))
					{
						if (file.Name.StartsWith("_SP") == false)
						{
							continue;
						}
					}

					file.MoveTo(Path.Combine(runFolderPath, file.Name));
				}
				catch(Exception err)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
						"Runﾌｫﾙﾀﾞへの移動に失敗した為、2秒後再移動します。対象ﾌｧｲﾙ:{0} 例外詳細:{1}", file.FullName, err.Message));
					//読み取り専用になる為、2秒後もう一度試す。
					Thread.Sleep(2000);
					file.MoveTo(Path.Combine(runFolderPath, file.Name));
				}
			}

			if (isOutputCIFSResult)
			{
			}
			else
			{
				//Startフォルダ内にある1マガジン分のファイル(指定のMPファイルより更新日時が古いファイル)をRunフォルダへ移動
				DirectoryInfo spFolder = new DirectoryInfo(spFolderPath);
				fileList = spFolder.GetFiles();

				magazineFiles = fileList.Where(f => mpFileInfo.LastWriteTime >= f.LastWriteTime).ToArray();
				foreach (FileInfo file in fileList)
				{
					try
					{
						file.MoveTo(Path.Combine(runFolderPath, file.Name));
					}
					catch
					{
						//読み取り専用になる為、2秒後もう一度試す。
						Thread.Sleep(2000);
						file.MoveTo(Path.Combine(runFolderPath, file.Name));
					}
				}
			}
		}

		private void ReportListOutputLog(List<ReportMessageInfo> reportList, string readMagazineNO)
		{
			foreach (ReportMessageInfo reportMessageInfo in reportList)
			{
				if (reportMessageInfo.ParameterVAL != null)
				{
					if (string.IsNullOrEmpty(reportMessageInfo.ParameterVAL.ToString().Trim()) == false)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備CD:{0} / マガジンNO:{1} / 受信：{2}", lsetInfo.EquipmentNO, readMagazineNO, reportMessageInfo.ParameterVAL), "SECS通信ログ");
					}
				}

				if (reportMessageInfo.MultiValueList != null)
				{
					foreach (ReportMessageInfo reportMessage in reportMessageInfo.MultiValueList)
					{
						if (reportMessage.ParameterVAL != null)
						{
							if (string.IsNullOrEmpty(reportMessage.ParameterVAL.ToString().Trim()) == false)
							{
								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("　　　設備CD:{0} / マガジンNO:{1} / {2}", lsetInfo.EquipmentNO, readMagazineNO, reportMessage.ParameterVAL), "SECS通信ログ");
							}
						}
					}
				}
			}
		}

		public class FileBase 
		{
			public static bool IsLotFromFileName(FileInfo file)
			{
				// MMファイル名定義(MM***_Lot_Type_Proc_MagazineNo.CSV)を想定して要素数で付与済か判断する 
				string[] nameChar = Path.GetFileNameWithoutExtension(file.FullName).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}

			public static bool IsLotFromFileName(string filePath) 
			{
				if (File.Exists(filePath) == false) 
				{
					throw new ApplicationException(string.Format("指定されたファイルが存在しません。FilePath:{0}", filePath));
				}

				FileInfo file = new FileInfo(filePath);
				return IsLotFromFileName(file);
			}
		}

		public class MMFile : FileBase
		{
			public const string FINISHEDFILE_IDENTITYNAME = "MM";

			public FileInfo Info { get; set; }
			public string TypeCd { get; set; }
			public string LotNo { get; set; }
			public int ProcNo { get; set; }

            private const int COL_ADDRESS_NO = 4;
            private const int COL_ERROR_CD = 5;
            private const string GOOD_PCS_CD = "0x0000";
            private const int WBMM_DATESTARTINDEX_FROM_MM = 6;

            public MMFile(FileInfo mmFile)
			{
				this.Info = mmFile;

				getLotFromFileName();
			}

			private void getLotFromFileName()
			{
				if (MMFile.IsLotFromFileName(this.Info) == false)
				{
					throw new ApplicationException(
						string.Format("MMファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
				}

				string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
				this.LotNo = nameChar[1].Trim();
				this.TypeCd = nameChar[2].Trim();
				this.ProcNo = int.Parse(nameChar[3].Trim());
            }
            public static Dictionary<int, string> getErrorCdList(string[] fileLineValueList)
            {
                Dictionary<int, string> retV = new Dictionary<int, string>();

                foreach (string fileLineValue in fileLineValueList)
                {
                    string[] lineData = fileLineValue.Split(',');
                    if (lineData.Length < COL_ERROR_CD) continue;

                    int address;
                    if (int.TryParse(lineData[COL_ADDRESS_NO], out address) == false) continue;

                    if (lineData[5] == GOOD_PCS_CD) continue;

                    if (retV.ContainsKey(address) == true)
                    {
                        retV[address] = lineData[5];
                    }
                    else
                    {
                        retV.Add(address, lineData[5]);
                    }
                }

                return retV;
            }

            #region WB-MMファイルデータ取得
            
            public static Dictionary<int, string> GetWBMMErrorCdData(int lineCD, string lotNO)
            {
                Dictionary<int, string> mappingFileList = new Dictionary<int, string>();

                string filePath = GetWBMMFilePath(lineCD, lotNO);

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    //MMファイルのデータを返す
                    string[] fileLineValueList = GetMachineFileLineValue(filePath, 0);

                    //WB形式のMMデータから、MappingFile形式のデータ取得
                    mappingFileList = WBMachineInfo.MMFile.getErrorCdList(fileLineValueList);
                }

                return mappingFileList;
            }

            private static string GetWBMMFilePath(int lineCD, string lotNO)
            {
                string filePath = null;
                string parentFolder = null;

                // 最初にLENSが作成したWB-MMファイル(コピー)を探す
                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

                //MMファイルのフルパス取得
                parentFolder = string.Format("{0}\\{1}", settingInfoPerLine.ForWBCompareMachineLogDirPath, lotNO);
                //ロットに紐付いている更新日時が最も遅いMMファイルを取得。無い場合はnull
                filePath = GetLatestMMFilePath(parentFolder);

                // LENSが作成したWB-MMファイルが無い → 通ってきたWB取得
                if (string.IsNullOrWhiteSpace(filePath) == true)
                {
                    Tran tranWBInfo = Tran.GetWBData(lineCD, lotNO);
                    if (tranWBInfo != null)
                    {
                        string machineFolderPath = GetMachineFolderPath(lineCD, tranWBInfo.PlantCD);

                        //MMファイルのフルパス取得
                        parentFolder = string.Format("{0}\\{1}\\Bind\\{2}", machineFolderPath, tranWBInfo.EndDT.Value.ToString("yyyyMM"), lotNO);

                        //ロットに紐付いている更新日時が最も遅いMMファイルを取得。無い場合はnull
                        filePath = GetLatestMMFilePath(parentFolder);
                    }
                }

                return filePath;
            }
            
            private static string GetMachineFolderPath(int lineCD, string plantCD)
            {
                //装置の情報取得
                List<LSETInfo> machineList = ConnectDB.GetLSETDataFromMultipleServer(lineCD, null, plantCD);

                List<string> folderPathList = new List<string>();

                if (machineList.Count > 1)
                {
                    throw new Exception(string.Format("装置のマスタ情報が複数取得されました。 ライン:{0} / 設備CD:{1}", lineCD, plantCD));
                }
                else if (machineList.Count == 0)
                {
                    return null;
                }

                //MMファイルのあるフォルダパスを取得
                string folderPath = machineList.Single().InputFolderNM;

                return folderPath;
            }
            
            /// <summary>
            /// 指定フォルダのファイル中で最も更新時間（ファイル名中に記載の日時）が新しいファイルパスを取得
            /// </summary>
            /// <param name="folderPath"></param>
            /// <param name="startIndexFromMM">MM文字のインデックスから日時文字列までの文字数</param>
            /// <returns></returns>
            private static string GetLatestMMFilePath(string folderPath)
            {
                if (!Directory.Exists(folderPath))
                {
                    return null;
                }

                List<string> fileList = Common.GetFiles(folderPath, ".*MM.*");
                int nSameTargetFileNum = fileList.Count;

                if (nSameTargetFileNum <= 0)
                {
                    return null;
                }

                DateTime[] sortedCreateTime = new DateTime[nSameTargetFileNum];

                List<FileInfo> fileInfoList = new List<FileInfo>();

                //ファイル名から日付文字列取得
                foreach (string swfname in fileList)
                {
                    fileInfoList.Add(new FileInfo(swfname));
                }

                //ファイル名からMMを検索して、最初に見つかったインデックスからの10文字(年月日日時)で配列並び替え
                fileInfoList = fileInfoList.OrderBy(f => DateTime.ParseExact(f.Name.Substring(f.Name.IndexOf("MM") + WBMM_DATESTARTINDEX_FROM_MM, 10), "yyMMddHHmm", null)).ToList();

                if (fileInfoList.Last().Exists == true)
                {
                    //ファイル名中の時間が最も遅いファイルパスを取得
                    return fileInfoList.Last().FullName;
                }
                else
                {
                    return null;
                }
            }

            #endregion
        }

        public class MPFile : FileBase
		{
			public const string FINISHEDFILE_IDENTITYNAME = "MP";

			public FileInfo Info { get; set; }
			public string TypeCd { get; set; }
			public string LotNo { get; set; }
			public int ProcNo { get; set; }
		}
    }
}
