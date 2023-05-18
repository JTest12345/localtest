using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using EICS.Machine;
using System.Net.Sockets;
using EICS.Database;
using EICS.Structure;

namespace EICS
{

	/// <summary>
	/// ダイボンダー処理
	/// </summary>
	public class DBMachineInfo : MachineBase
	{
		protected const string FILE_SCAN_PREFIX = "STARTPARAM";
		protected const string START_PARAM_FILE_EXT = "txt";

		public bool DoIntervalLogOutFG { get; set; }
		public DateTime LastIntervalLogOutDT { get; set; }

		/// <summary>ダイボンダー装置名</summary>
		public const string ASSETS_NM = "ﾀﾞｲﾎﾞﾝﾀﾞｰ";

		private const double TARGET_BM_RECORD_START_OFFSET_MINUTE = -180;

		private const int MACHINE_SEQNO_START_INDEX_ON_FILENAME = 1;
		private const int MACHINE_SEQNO_LEN = 4;

		public int LotCompWaitCt { get; set; }

		/// <summary>装置型式</summary>
		public enum ModelType
		{
			AD8930V,
			AD830,
			AD8930,
			AD838L,
		}
		/// <summary>ログファイル出力タイミング</summary>
		public enum MachineTiming
		{
			Start,
			MagazineComplete
		}

		#region 定数

		public const string DATA_OUTPUT_PATH = @"C:\QCIL\data\DB";

		public const int LOT_COMP_WAIT_ALERT_CT = 150;

		/// <summary>最大滞留ファイル数</summary>
		public const int FILE_COUNT_MAX = 20;

		/// <summary>実行フォルダ名</summary>
		public const string FOLDER_RUN_NM = "Run";

		#region Oファイル必要・要素数
		/// <summary>Oファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_O_AD830 = 20;

		/// <summary>Oファイル必要文字長さ(2)</summary>
		public const int FILE_NEED_LENGTH_O_AD830_2 = 21;

		/// <summary>Oファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_O_AD8930V = 16;

		/// <summary>Oファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_O_AD8930 = 15;

		/// <summary>Oファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_O_AD838L = 27; //LEDは33 ZDは27

		#endregion

		/// <summary>Pファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_P = 3;

		#region Mファイル必要・要素数
		/// <summary>Mファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_M_AD8930V = 59;

		/// <summary>Mファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_M_AD830 = 30;

		/// <summary>Mファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_M_AD8930 = 29;

		/// <summary>Mファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_M_AD838L = 77;

		#endregion

		#region Lファイル必要・要素数
		/// <summary>Lファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_L = 7;

		/// <summary>Lファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_L_AD8930 = 6;

		#endregion

		#region Hファイル必要・要素数
		/// <summary>Hファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_H = 16;

		/// <summary>Hファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_H_AD8930 = 15;

        protected const int DICE_NO_INDEX_H = 5;

        #endregion

        #region Iファイル必要・要素数
        /// <summary>Iファイル必要文字長さ</summary>
        public const int FILE_NEED_LENGTH_I = 15;

		/// <summary>Iファイル必要文字長さ</summary>
		public const int FILE_NEED_LENGTH_I_AD8930 = 14;

        protected const int DICE_NO_INDEX_I = 5;

        /// <summary>Iファイル：エラーコード列</summary>
        public const int FILE_ERROR_COL_I = 14;

        #endregion

        #region Sファイル必要・要素数
        /// <summary>Sファイル必要文字長さ</summary>
        public const int FILE_NEED_LENGTH_S = 16;

        protected const int DICE_NO_INDEX_S = 5;

        /// <summary>Sファイル：エラーコード列</summary>
        public const int FILE_ERROR_COL_S = 15;

        #endregion

        /// <summary>ファイル内容(測定日時日付)</summary>
        public const int FILE_MEASUREDATE_DATE = 0;

		/// <summary>ファイル内容(測定日時時間)</summary>
		public const int FILE_MEASUREDATE_TIME = 1;

		/// <summary>ファイル内容(マガジン段数)</summary>
		public const int FILE_MAGAZINESTEP_NO = 2;

		/// <summary>ファイル内容(ダイスNO)</summary>
		public const int FILE_DICENO = 5;

		/// <summary>ファイル内容(X位置)</summary>
		public const int FILE_X = 6;

		/// <summary>ファイル内容(X位置_Skipped)</summary>
		public const string FILE_X_SKIPPED = "Skipped";

		/// <summary>ファイル内容(X閾値)</summary>
		public const int FILE_XLIMIT = 8;

		/// <summary>ファイル内容(Y位置)</summary>
		public const int FILE_Y = 7;

		/// <summary>ファイル内容(Y閾値)</summary>
		public const int FILE_YLIMIT = 9;

		///// <summary>Iファイル：エラーコード列</summary>
		//public const int FILE_ERROR_COL_I = 14;

		/// <summary>ファイル内容(右スタンプ区分)</summary>
		public const string FILE_STAMP_RIGHT_KB = "右";

		/// <summary>ファイル内容(左スタンプ区分)</summary>
		public const string FILE_STAMP_LEFT_KB = "左";

        /// <summary>ファイルIパラメータ名(Post x)</summary>
        public const string I_PARAM_X_NM = "Post Inspection(Placement-x)ｴﾗｰ回数";

		/// <summary>ファイルIパラメータ名(Post y)</summary>
		public const string I_PARAM_Y_NM = "Post Inspection(Placement-y)ｴﾗｰ回数";

		/// <summary>ファイルIパラメータ名(Post θ)</summary>
		public const string I_PARAM_SIGMA_NM = "Post Inspection(Placement-θ)ｴﾗｰ回数";

		/// <summary>ファイルHパラメータ名(Pre x)</summary>
		public const string H_PARAM_X_NM = "Pre Inspection(Placement-x)ｴﾗｰ回数";

		/// <summary>ファイルHパラメータ名(Pre y)</summary>
		public const string H_PARAM_Y_NM = "Pre Inspection(Placement-y)ｴﾗｰ回数";

		/// <summary>ファイルHパラメータ名(Pre Area)</summary>
		public const string H_PARAM_AREA_NM = "Pre Inspection(Area)ｴﾗｰ回数";

        /// <summary>ファイルSパラメータ名(Pre x)</summary>
        public const string S_PARAM_X_NM = "Substrate Inspection(Placement-x)ｴﾗｰ回数";

        /// <summary>ファイルSパラメータ名(Pre y)</summary>
        public const string S_PARAM_Y_NM = "Substrate Inspection(Placement-y)ｴﾗｰ回数";

        /// <summary>ファイルSパラメータ名(Pre Area)</summary>
        public const string S_PARAM_AREA_NM = "Substrate Inspection(Area)ｴﾗｰ回数";

        private const int AD830_WITH_DEFAULT_COL_GAP = 0;

		private const int AD8930V_WITH_DEFAULT_COL_GAP = 0;

		private const int AD8930_WITH_DEFAULT_COL_GAP = -1;

		private const int AD838L_WITH_DEFAULT_COL_GAP = 0;

        #endregion

        public DBMachineInfo()
		{
			LotCompWaitCt = 0;
		}

		public override void InitFirstLoop(LSETInfo lsetInfo)
		{
			CheckFileFmtFromParamWhenInit(lsetInfo, false);
		}

		/// <summary>
		/// ファイルチェック
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <returns>装置処理状態ステータス</returns>
		public override void CheckFile(LSETInfo lsetInfo)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			bool isWaitForRenameByArms = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);
			bool isFullParamFG = settingInfoPerLine.GetFullParameterFG(lsetInfo.EquipmentNO);

			DoIntervalLogOutFG = IsNeedIntervalLogOut();
#if Debug
            //開発者が確認する場合、自分のPCにネットワークドライブが無いので、保存先を変える
			lsetInfo.InputFolderNM = @"C:\QCIL\data\DB\Share" + @"\";
            //lsetInfo.InputFolderNM = @"C:\qcil\data\DB";
#else
#endif
			//lsetInfo.InputFolderNM = @"C:\qcil\data\DB\Share\" + lsetInfo.EquipmentNO + @"\";
#if DEBUG
			lsetInfo.InputFolderNM = @"C:\qcil\data\DB\Share\";
#else
#endif

			//ログファイル保存先フォルダの存在確認はMachineBaseへ移動。バッチも使用しないようにバッチの中の機能をEICS3に移植
			//機能は実装済みなのでCheckDirectory内の処理を復活させれば良い

			//ログファイル保存先フォルダの存在確認
			if (!Directory.Exists(lsetInfo.InputFolderNM))
			{
				CheckDirectory(lsetInfo);

				string message = string.Format(Constant.MessageInfo.Message_39, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO) + lsetInfo.InputFolderNM;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);

				//1分毎に接続確認
				System.Threading.Thread.Sleep(60000);

				base.machineStatus = Constant.MachineStatus.Runtime;
				return;
			}

			bool isOutputCommonPath = settingInfoPerLine.IsOutputCommonPath(lsetInfo.EquipmentNO);

			MoveOwnFileFromCommonPath(settingInfoPerLine.DirCommonOutput, lsetInfo.InputFolderNM
				, lsetInfo.MachineSeqNO, isOutputCommonPath, MACHINE_SEQNO_START_INDEX_ON_FILENAME, MACHINE_SEQNO_LEN);

			BackupMpd(lsetInfo);

			if (DoIntervalLogOutFG)
			{
				string[] checkFileList = Directory.GetFiles(lsetInfo.InputFolderNM);
				if (checkFileList.Length >= FILE_COUNT_MAX)
				{
					//ファイルが20以上存在する場合、ログ出力
					string sMessage = string.Format(Constant.MessageInfo.Message_25, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				}
			}
			try
			{
				//Runフォルダ内の滞留ファイルを処理
				string runDirPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_RUN_NM);
				if (Directory.Exists(runDirPath))
				{
					if (Directory.GetFiles(runDirPath).Length != 0)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Run内ファイル取得");
						//1Set揃っているか確認
						GetFilesWithCheckMagazineFileExist(runDirPath, "", lsetInfo, isOutputCommonPath, settingInfoPerLine.DirCommonOutput);

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "1セット確認");

						//マガジンタイミングの処理
						if (Constant.MachineStatus.Wait == CheckMagazineTiming(lsetInfo, ref base.errorMessageList))
						{
							base.machineStatus = Constant.MachineStatus.Runtime;
							return;
						}
					}
				}

				//Shearフォルダ内のマガジン連番リストを生成
				List<string> magazineIDList = GetMagazineIDData(settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO)
					, lsetInfo.InputFolderNM, MachineTiming.MagazineComplete, settingInfoPerLine.GetSFileExists(lsetInfo.EquipmentNO));
				if (magazineIDList.Count != 0)
				{
					foreach (string magazineID in magazineIDList)
					{
                        if (string.IsNullOrWhiteSpace(magazineID) == true)
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"処理フォルダ(Run)に移動する処理でmagazineIDが空白のため処理対象外");
                            continue;
                        }

						//1Set揃っているか確認
						List<string> moveTargetFileList = GetFilesWithCheckMagazineFileExist(lsetInfo.InputFolderNM, magazineID
							, lsetInfo, isOutputCommonPath, settingInfoPerLine.DirCommonOutput);

                        //moveTargetFileListのログ追加
                        int filecount = 0;
                        foreach (var moveTargetFilePath in moveTargetFileList)
                        {
                            filecount++;
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"処理フォルダ(Run)に移動する対象のファイル：{filecount}、{moveTargetFilePath}");
                        }

                        //処理フォルダ(Run)に1Setを移動
                        MoveRunFile(lsetInfo.InputFolderNM, magazineID, moveTargetFileList);

						//マガジンタイミングの処理
						if (Constant.MachineStatus.Wait == CheckMagazineTiming(lsetInfo, ref base.errorMessageList))
						{
							base.machineStatus = Constant.MachineStatus.Runtime;
							return;
						}
					}
				}
				else if (isFullParamFG == false)
				{
					magazineIDList = GetMagazineIDData(false, lsetInfo.InputFolderNM, MachineTiming.Start, settingInfoPerLine.GetSFileExists(lsetInfo.EquipmentNO));
					foreach (string magazineID in magazineIDList)
					{
						//1Set揃っているか確認
						CheckStartFileExist(lsetInfo.InputFolderNM, magazineID, lsetInfo, isOutputCommonPath, settingInfoPerLine.DirCommonOutput);

						//スタートタイミングの処理
						CheckStartTiming(lsetInfo, magazineID, ref base.errorMessageList);
					}
				}
				else
				{
					//開始・全パラメタチェック処理

					//●●●_YYYYMMDDHHmmSS.txtのファイルがあるかチェック
					//●●●：PKGファイル名(品種プログラム名)
					//YYYYMMDDHHmmSS：西暦表記の年月日時分秒の数字
					List<string> filePathList = Common.GetFiles(lsetInfo.InputFolderNM, ".+[.]txt");

					if (filePathList.Count > 0)
					{
						List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

						FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false);

						FileScan fileScan = FileScan.GetSingle(lsetInfo.InlineCD, lsetInfo.ModelNM, FILE_SCAN_PREFIX);

						List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, true);

						List<FileFmtWithPlm> fileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);

						//WBでも同じことしてるのになぜかDBではオブジェクト参照が…のエラーが発生する
						//fileFmtWithPlmList = fileFmtWithPlmList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.Plm.QcParamNO));

						List<FileFmtWithPlm> fileFmtWithPlmTempList = new List<FileFmtWithPlm>();

						foreach (FileFmtWithPlm fileFmtWithPlm in fileFmtWithPlmList)
						{
							if (plmPerTypeModelChipList.Exists(p => p.QcParamNO == fileFmtWithPlm.Plm.QcParamNO))
							{
								fileFmtWithPlmTempList.Add(fileFmtWithPlm);
							}
						}

						fileFmtWithPlmList = fileFmtWithPlmTempList;

						List<Log> logList = new List<Log>();
						foreach (string filePath in filePathList)
						{
							FileInfo fi = new FileInfo(filePath);

							if (fi.Length == 0)
							{
								continue;
							}

							long fileLastSize = fi.Length;
							Thread.Sleep(300);

							fi.Refresh();
							if (fileLastSize != fi.Length)
							{
								continue;
							}

							DateTime fileCreationDt = File.GetCreationTime(filePath);

							//CIFSの処理に放り込めるか確認必要（ファイルの中身を放り込める形になってれば、改修少ないと思うが
							//ファイルのパスを渡す形の場合、ファイル名が既存仕様と乖離してるので合わせこむような関数が間に必要）
							MachineFile macFile = new MachineFile();
							List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

							macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, CIFSBasedMachine.LF, ':', lsetInfo.TypeCD, string.Empty
								, string.Empty, fileCreationDt, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, null, ref logList, true);

							foreach (Log log in logList)
							{
								ConnectDB.InsertTnLOG(log, lsetInfo);
							}

							Regex regex = new Regex("[.]" + START_PARAM_FILE_EXT + "$", RegexOptions.IgnoreCase);

							string fileNmWithoutExt = regex.Replace(Path.GetFileName(filePath), string.Empty);

							CIFSBasedMachine.OutputResultFile(lsetInfo, errMsgList, lsetInfo.InputFolderNM, fileNmWithoutExt, true);

							List<string> backupTarget = new List<string>();
							backupTarget.Add(filePath);

							CIFS.BackupDoneFiles(backupTarget, lsetInfo.InputFolderNM, string.Empty, fileCreationDt);

							base.errorMessageList.AddRange(errMsgList);
						}
					}
				}
				base.machineStatus = Constant.MachineStatus.Runtime;

				return;
			}
			finally
			{
				if (DoIntervalLogOutFG)
				{
					LastIntervalLogOutDT = DateTime.Now;
					DoIntervalLogOutFG = false;
				}
			}
		}

		public bool IsWorkEndLot(int lineCD, List<string> moveTargetFileList)
		{
			string lFilePath = moveTargetFileList.Find(m => Path.GetFileName(m).StartsWith("L"));

			if (MachineFile.IsThereLotNoInFileName(lFilePath) == false)
			{
				return false;
			}

			string lotNo = MachineFile.GetLotFromFileName(lFilePath);
			int procNo = MachineFile.GetProcFromFileName(lFilePath);

			ArmsLotInfo lotInfo = ConnectDB.GetARMSLotData(lineCD, null, null, null, false, false, false, lotNo, procNo)[0];

			if (string.IsNullOrEmpty(lotInfo.EndDT))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool IsNeedIntervalLogOut()
		{
			if (LastIntervalLogOutDT == null)
			{
				return true;
			}
			else
			{
				TimeSpan ts = DateTime.Now - LastIntervalLogOutDT;

				if (ts.TotalMinutes >= 60.0)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// マガジンタイミング処理
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		public Constant.MachineStatus CheckMagazineTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			AlertLog alertLog = AlertLog.GetInstance();

			//[E:errorlog]   / [H:Pre bond全データ]   / [I:Post bond全データ] / [J:Pre bondのXyθエラー] / [K:Post bondのXyθエラー]
			//[L:life Count] / [O:装置設定パラメータ] / [P:Package data name] / [W:Wafer Log data]

			//処理するフォルダ
			string runFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_RUN_NM);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			//チップ
			//string chipNM = settingInfoPerLine.GetChipNM(lsetInfo.EquipmentNO);
			int timingNO = 0;

			//if (Common.IsLEDChip(chipNM))
			if (Common.IsLEDChip(lsetInfo.ChipNM))
			{
				timingNO = 2;
			}
			else
			{
				timingNO = 1;
			}

			bool completeBFileProcessFG = false;

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "WFileProcess()前");

			WFileProcess(lsetInfo, runFolderPath, ref errMessageList);

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "WFileProcess()完了");

			MagInfo magInfo;
			string dtFileStamp, sFileStamp;

			// ARMSによるファイルリネームを待つ
			if (settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO))
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ARMS Rename待ち完了");

				dtFileStamp = KLinkInfo.GetFileStampDT(runFolderPath, Convert.ToString(FileType.L));
				dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//装置の時間として使用(ファイルスタンプ-5分)
				sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");


				List<string> lFilePathList = MachineFile.GetPathList(runFolderPath, string.Format("^{0}", Convert.ToString(FileType.L)), true);

				if (lFilePathList != null)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Lファイル取得数:{0}", lFilePathList.Count));
				}
				else
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Lファイル取得数:null"));
				}

				if (lFilePathList.Count > 1)
				{
					throw new ApplicationException(string.Format(
						"{0}にてLﾌｧｲﾙが複数確認されました。Runﾌｫﾙﾀﾞ内はﾏｶﾞｼﾞﾝ単位で出力される1組のﾌｧｲﾙのみとして下さい。", runFolderPath));
				}
				else if (lFilePathList.Count == 0)
				{
					throw new ApplicationException(string.Format("{0}からﾌｧｲﾙが取得出来ませんでした。", runFolderPath));
				}

				string lotNo = MachineFile.GetLotFromFileName(lFilePathList[0]);
				int procNo = MachineFile.GetProcFromFileName(lFilePathList[0]);
				string magNo = MachineFile.GetMagazineNoFromFileName(lFilePathList[0]);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("ファイルからロットNo,工程No取得"));

				List<ArmsLotInfo> lotInfoList = ConnectDB.GetARMSLotData(lsetInfo.InlineCD, null, null, null, false, false, false, lotNo, procNo);

				if (lotInfoList == null || lotInfoList.Count == 0)
				{
					Thread.Sleep(300000);
					lotInfoList = ConnectDB.GetARMSLotData(lsetInfo.InlineCD, null, null, null, false, false, false, lotNo, procNo);

					if (lotInfoList == null || lotInfoList.Count == 0)
					{
						throw new ApplicationException(string.Format("ARMSから実績取得出来ませんでした。lotNo:{0} 工程No:{1}", lotNo, procNo));
					}
				}

				ArmsLotInfo lotInfo = lotInfoList[0];

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("GetARMSLotData()完了"));

				magInfo = new MagInfo();
				magInfo.StartDT = Convert.ToDateTime(lotInfo.StartDT);

				magInfo.sMagazineNO = magNo;
				magInfo.sNascaLotNO = lotInfo.LotNO;  //最初のLot
				magInfo.sMaterialCD = lotInfo.TypeCD; //最初のLot
				magInfo.ProcNO = lotInfo.ProcNO;

				if (string.IsNullOrEmpty(dtFileStamp) == false)
				{
					magInfo.EndDT = Convert.ToDateTime(dtFileStamp);
				}
				else
				{
					return Constant.MachineStatus.Wait;
				}
			}
			else
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ARMS Rename待たない");

				//1マガジン単位ファイルのファイルスタンプ取得
				dtFileStamp = KLinkInfo.GetFileStampDT(runFolderPath, Convert.ToString(FileType.L));

				dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//装置の時間として使用(ファイルスタンプ-5分)
				sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");

				//ARMSからロット情報取得
				magInfo = GetMagInfo(lsetInfo, dtFileStamp, true);
				if (magInfo == null)
				{
					throw new ApplicationException(string.Format("【紐付けエラー】紐付け可能なロット情報が存在しません。{0}Fileのファイルスタンプ[{1}]", Convert.ToString(FileType.L), dtFileStamp));

				}
				else if (magInfo.EndDT == null || magInfo.EndDT == DateTime.MinValue)
				{
					if (IsLegacyFile(lsetInfo, dtFileStamp))
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("ファイルスタンプ({0})より新しい開始時間を持つ完了実績が発生した為、Unchainへ移動", dtFileStamp));
						UnchainFileProcess(lsetInfo, runFolderPath, FileType.L);

						return Constant.MachineStatus.Wait;
					}

					return Constant.MachineStatus.Wait;
				}
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Magazine.GetData()前");

			Database.LENS.Magazine magConfig = EICS.Database.LENS.Magazine.GetData(magInfo.sMaterialCD, this.LineCD);

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Magazine.GetData()完了");

			if (magConfig == null)
			{
				throw new ApplicationException(string.Format("マガジン構成マスタが存在しません。 型番:{0}", magInfo.sMaterialCD));
			}

			magInfo.FrameXPackageCT = magConfig.PackageQtyX;
			magInfo.FrameYPackageCT = magConfig.PackageQtyY;
			magInfo.FramePackageCT = magConfig.TotalFramePackage;
			magInfo.MagazineMaxStepCT = magConfig.StepNum;
			magInfo.MagazineStepCT = magConfig.StepNum;
			magInfo.MagazinePackageCT = magConfig.TotalMagazinePackage;

			//トレース用の履歴を登録する　(GEICSで装置単位のグラフ表示に使用)
			if (magInfo.sNascaLotNO == null)
			{
				Lott.SetTnLOTT(lsetInfo, "不明");
			}
			else
			{
				Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Bファイル GetMachineFile()前");

			List<string> sBFileList = MachineFile.GetPathList(runFolderPath, "^B");

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Bファイル GetMachineFile()完了 MoveExceptLatestFile()前");

			MoveExceptLatestFile(magInfo.sNascaLotNO, sBFileList, dtFileStamp, lsetInfo);

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "Bファイル取得後 MoveExceptLatestFile()完了");

			//処理フォルダ内のファイルを取得
			List<string> mFileList = MachineFile.GetPathList(runFolderPath, "");
			foreach (string mFilePath in mFileList)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("mFileList取得 mFilePath:{0}", mFilePath));

				FileInfo fileInfo = new FileInfo(mFilePath);

				//登録済みか確認する
				if (CheckEndFile(fileInfo.FullName, lsetInfo.InputFolderNM, sFileStamp, magInfo.sNascaLotNO))
				{
					//登録済みの場合、対象ファイルを削除して次ファイル処理へ
					fileInfo.Delete();
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_46, fileInfo.FullName));
					continue;
				}

				string sFileType = GetMachineFileChar(fileInfo.FullName, lsetInfo.AssetsNM);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] {0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
                int fileLen;

                switch (sFileType)
				{
					case "H":
                        if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930))
                        {
                            fileLen = FILE_NEED_LENGTH_H_AD8930;
                        }
                        else
                        {
                            fileLen = FILE_NEED_LENGTH_H;
                        }

                        DbInput_DB_HFile(lsetInfo, magInfo, mFilePath, ref errMessageList, fileLen, null, FileType.H);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
					case "I":
                        if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930))
                        {
                            fileLen = FILE_NEED_LENGTH_I_AD8930;
                        }
                        else
                        {
                            fileLen = FILE_NEED_LENGTH_I;
                        }
                        DbInput_DB_IFile(lsetInfo, magInfo, mFilePath, ref errMessageList, fileLen, GetCol(lsetInfo.ModelNM, FILE_ERROR_COL_I), FileType.I);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
                    case "S":
                        fileLen = FILE_NEED_LENGTH_S;
                        DbInput_DB_SFile(lsetInfo, magInfo, mFilePath, ref errMessageList, fileLen, null, FileType.S);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
                        SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
                        break;
                    case "L":
						DbInput_DB_LFile(lsetInfo, magInfo, mFilePath, ref errMessageList);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
					case "O":
						DbInput_DB_OFile(lsetInfo, magInfo, mFilePath, Constant.nMagazineTimming, ref errMessageList);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
					case "P":
						DbInput_DB_PFile(lsetInfo, magInfo, mFilePath, Constant.nMagazineTimming, ref errMessageList);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
					case "W":
						DbInput_DB_WFile(lsetInfo, mFilePath);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
					case "B":
						if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
						{
							alertLog.logMessageQue.Enqueue(string.Format(Constant.MessageInfo.Message_140, lsetInfo.EquipmentNO, sFileType, fileInfo.Name));
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_140, lsetInfo.EquipmentNO, sFileType, fileInfo.Name));
							SingleMoveCompFile(lsetInfo, fileInfo, string.Empty, sFileType, sFileStamp);
							return Constant.MachineStatus.Runtime;
						}
						if (settingInfoPerLine.GetBMCount(lsetInfo.EquipmentNO))
						{
							DbInput_DB_BFile(lsetInfo, magInfo, mFilePath);
							completeBFileProcessFG = true;
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
						}
						else
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File BMカウント OFFの為、処理無し", lsetInfo.EquipmentNO, sFileType));
						}

						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);

						break;
                    case "E":
                        DbInput_DB_EFile(lsetInfo, magInfo, mFilePath, ref errMessageList);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
                        SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
                        break;

                    default:
						SingleMoveCompFile(lsetInfo, fileInfo, magInfo.sNascaLotNO, sFileType, sFileStamp);
						break;
				}

			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "mFileList foreach後");

			if (completeBFileProcessFG == false)
			{
				//全ファイル処理後にBファイルの処理が実行されなかった場合
				NascaDefectFile
					.Create(magInfo.sNascaLotNO, magInfo.sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, string.Empty, magInfo.ProcNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("全ファイル処理後、nasファイルを確認出来なかった為、空のnasファイル作成", lsetInfo.EquipmentNO));

			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB CheckQC", lsetInfo.EquipmentNO));
			CheckQC(lsetInfo, timingNO, magInfo.sMaterialCD);//1はDBZD,2はDBLEDの意味
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB:CheckQC", lsetInfo.EquipmentNO));

			return Constant.MachineStatus.Runtime;
		}

		private void WFileProcess(LSETInfo lsetInfo, string runDir, ref List<ErrMessageInfo> errMessageList)
		{
			List<string> targetFileList = Common.GetFiles(runDir, "^W.*");

			targetFileList = targetFileList.Where(f => Path.GetFileNameWithoutExtension(f).Substring(0, 1) != "_").ToList();

			foreach (string targetFile in targetFileList)
			{
				DbInput_DB_WFile(lsetInfo, targetFile);
				ChangeCompleteMachineFile(targetFile, ref errMessageList);
			}
		}

		private void SingleMoveCompFile(LSETInfo lsetInfo, FileInfo fileInfo, string sNascaLotNO, string sFileType, string sFileStamp)
		{
			//保管場所へ移動
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
			MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, sNascaLotNO, sFileStamp);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
		}

		/// <summary>
		/// スタートタイミング処理
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		public void CheckStartTiming(LSETInfo lsetInfo, string magazineID, ref List<ErrMessageInfo> errMessageList)
		{
			//[E:errorlog]   / [H:Pre bond全データ]   / [I:Post bond全データ] / [J:Pre bondのXyθエラー] / [K:Post bondのXyθエラー]
			//[L:life Count] / [O:装置設定パラメータ] / [P:Package data name] / [W:Wafer Log data]
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			MagInfo magInfo = new MagInfo();
			magInfo.sMagazineNO = "";
			magInfo.sNascaLotNO = null;
			magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

			List<string> sFileList = GetMachineStartFile(lsetInfo.InputFolderNM, magazineID);
			foreach (string sFile in sFileList)
			{
				//ファイル内処理(スタートタイミングでは異常判定だけ行い、管理項目内容はDB登録しない)
				string sFileType = GetMachineFileChar(sFile, lsetInfo.AssetsNM);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB {1}File ", lsetInfo.EquipmentNO, sFileType));
				switch (sFileType)
				{
					case "O":
						DbInput_DB_OFile(lsetInfo, magInfo, sFile, Constant.nStartTimming, ref errMessageList);
						break;
					case "P":
						DbInput_DB_PFile(lsetInfo, magInfo, sFile, Constant.nStartTimming, ref errMessageList);
						break;
				}
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));

				//処理済みのファイル名を変更する
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
				ChangeCompleteMachineFile(sFile, ref errMessageList);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
			}
		}

        /// <summary>
        /// 引数であるファイル内容、対象列インデクスからダイス数を取得。取得できない場合、nullを返す
        /// </summary>
        /// <param name="fileLineValue"></param>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        protected int? GetDiceCountFromFileContents(string[] fileLineValue, int targetIndex)
        {
            //ファイル内容からダイス数取得
            List<string> fileContents = new List<string>();
            foreach (string lineValue in fileLineValue)
            {
                string[] lineItem = lineValue.Split(',');
                fileContents.Add(lineItem[targetIndex]);
            }

            int maxDiceNo = int.MinValue;
            foreach (string item in fileContents.Distinct())
            {
                int temp;
                if (int.TryParse(item, out temp) == false)
                {
                    string msg = string.Format("数値変換出来ない内容がファイルに書き込まれました。変換対象:{0}", item);
                    throw new ApplicationException(msg);
                }

                if (maxDiceNo < temp)
                {
                    maxDiceNo = temp;
                }
            }

            if (maxDiceNo == int.MinValue)
            {
                return null;
            }
            else
            {
                //ファイル中のダイスNoは0始まりの値である為、ダイス数を表すため、1を加算する
                maxDiceNo = maxDiceNo + 1;
            }

            return maxDiceNo;
        }

        /// <summary>
        /// マガジンタイミングファイルの存在確認
        /// </summary>
        /// <param name="targetDirPath">対象ディレクトリ</param>
        /// <param name="magazineID">マガジン連番</param>
        protected List<string> GetFilesWithCheckMagazineFileExist(string targetDirPath, string magazineID, LSETInfo lsetInfo, bool isOutputCommonPath, string dirCommonOutput)
		{
			return GetFilesWithCheckMagazineFileExist(targetDirPath, magazineID, System.DateTime.Now, null, lsetInfo, isOutputCommonPath, dirCommonOutput);
		}

		private List<string> GetFilesWithCheckMagazineFileExist(string targetDirPath, string magazineID, DateTime startDT, StringBuilder noFileNM, LSETInfo lsetInfo, bool isOutputCommonPath, string dirCommonOutput)
		{
			List<string> targetFileList = new List<string>();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			bool waitForRenameByArms = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);

			if (startDT.AddSeconds((commonSettingInfo.DBMachineLogOutWaitmSec / 1000) + 5) <= System.DateTime.Now)
			{
				//5秒の制限時間を超えた場合、エラー
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_52, magazineID, Constant.MessageInfo.Message_53, targetDirPath, noFileNM.ToString())
                    + $"監視開始時間：{startDT}");
			}

			if (waitForRenameByArms)
			{
				targetFileList = Common.GetFiles(targetDirPath, string.Format(".*{0}_.*_.*_.*_.*", magazineID), ".*");
				targetFileList.AddRange(Common.GetFiles(targetDirPath, ".*" + magazineID, ".*"));
			}
			else
			{
				targetFileList = Common.GetFiles(targetDirPath, ".*" + magazineID, ".*");
			}

			//存在確認の対象外リスト
			//List<string> excludeTargetFileList

			if (targetFileList.Count == 0)
			{
				Thread.Sleep(commonSettingInfo.DBMachineLogOutWaitmSec);
				MoveOwnFileFromCommonPath(dirCommonOutput, lsetInfo.InputFolderNM, lsetInfo.MachineSeqNO, isOutputCommonPath, MACHINE_SEQNO_START_INDEX_ON_FILENAME, MACHINE_SEQNO_LEN);
				targetFileList = GetFilesWithCheckMagazineFileExist(targetDirPath, magazineID, startDT, null, lsetInfo, isOutputCommonPath, dirCommonOutput);
			}

			StringBuilder noFileText = new StringBuilder();

			string hFileSearchCond, iFileSearchCond, lFileSearchCond, sFileSearchCond;

			if (waitForRenameByArms)
			{
				hFileSearchCond = "^H.*_.*_.*_.*_.*$";
				iFileSearchCond = "^I.*_.*_.*_.*_.*$";
				lFileSearchCond = "^L.*_.*_.*_.*_.*$";
                sFileSearchCond = "^S.*_.*_.*_.*_.*$";
            }
            else
			{
				hFileSearchCond = "^H.*$";
				iFileSearchCond = "^I.*$";
				lFileSearchCond = "^L.*$";
                sFileSearchCond = "^S.*$";
            }

			//Hファイル存在確認
			List<string> searchList
				= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), hFileSearchCond) == true).ToList();
			if (searchList.Count == 0)
			{
				if (noFileText.Length > 0)
					noFileText.Append("/");
				noFileText.Append(FileType.H.ToString());
			}

			//Iファイル存在確認
			searchList
				= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), iFileSearchCond) == true).ToList();
			if (searchList.Count == 0)
			{
				if (noFileText.Length > 0)
					noFileText.Append("/");
				noFileText.Append(FileType.I.ToString());
			}

			//Lファイル存在確認
			searchList
				= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), lFileSearchCond) == true).ToList();
			if (searchList.Count == 0)
			{
				if (noFileText.Length > 0)
					noFileText.Append("/");
				noFileText.Append(FileType.L.ToString());
			}

            bool sFileExists = settingInfoPerLine.GetSFileExists(lsetInfo.EquipmentNO);
            if (sFileExists)
            {
                //Sファイル存在確認
                searchList
                    = targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), sFileSearchCond) == true).ToList();
                if (searchList.Count == 0)
                {
                    if (noFileText.Length > 0)
                        noFileText.Append("/");
                    noFileText.Append(FileType.S.ToString());
                }
            }

            if (noFileText.Length != 0)
            {
                Thread.Sleep(commonSettingInfo.DBMachineLogOutWaitmSec);
                MoveOwnFileFromCommonPath(dirCommonOutput, lsetInfo.InputFolderNM, lsetInfo.MachineSeqNO, isOutputCommonPath, MACHINE_SEQNO_START_INDEX_ON_FILENAME, MACHINE_SEQNO_LEN);
                targetFileList = GetFilesWithCheckMagazineFileExist(targetDirPath, magazineID, startDT, noFileText, lsetInfo, isOutputCommonPath, dirCommonOutput);
            }

            return targetFileList;
		}

		/// <summary>
		/// スタートタイミングファイルの存在確認
		/// </summary>
		/// <param name="targetDirPath">対象ディレクトリ</param>
		/// <param name="magazineID">マガジン連番</param>
		protected void CheckStartFileExist(string targetDirPath, string startID, LSETInfo lsetInfo, bool isOutputCommonPath, string dirCommonOutput)
		{
			CheckStartFileExist(targetDirPath, startID, System.DateTime.Now, "", lsetInfo, isOutputCommonPath, dirCommonOutput);
		}

		private void CheckStartFileExist(string targetDirPath, string startID, DateTime startDT, string noFileNM, LSETInfo lsetInfo, bool isOutputCommonPath, string dirCommonOutput)
		{

			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			List<string> targetFileList = Common.GetFiles(targetDirPath, ".*", startID);

			string fileNM = Path.GetFileNameWithoutExtension(targetFileList[0]);
			string magazineID = fileNM.Substring(fileNM.Length - 3, 3);

			if (startDT.AddSeconds((commonSettingInfo.DBMachineLogOutWaitmSec / 1000) + 10) <= System.DateTime.Now)
			{
				//5秒の制限時間を超えた場合、エラー
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_52, magazineID, Constant.MessageInfo.Message_53, targetDirPath, noFileNM)
                     + $">>監視開始時間：{startDT}");
			}

			StringBuilder noFileText = new StringBuilder();

			//Oファイル存在確認
			List<string> searchList
				= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), "^O.*$") == true).ToList();
			if (searchList.Count == 0)
			{
				if (noFileText.Length > 0)
					noFileText.Append("/");
				noFileText.Append(FileType.O.ToString());
			}

			//Pファイル存在確認
			searchList
				= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), "^P.*$") == true).ToList();
			if (searchList.Count == 0)
			{
				if (noFileText.Length > 0)
					noFileText.Append("/");
				noFileText.Append(FileType.P.ToString());
			}

			//2012/06/27追加↓ T.Sasaki-------------------------------------------[start]

			//NMCの場合、Mファイル存在確認

			if (Constant.fNmc || lsetInfo.EquipInfo.MFileExists == true)
			{
				//Mファイル存在確認
				searchList
					= targetFileList.Where(t => Regex.IsMatch(Path.GetFileNameWithoutExtension(t), "^M.*$") == true).ToList();
				if (searchList.Count == 0)
				{
					if (noFileText.Length > 0)
						noFileText.Append("/");
					noFileText.Append(FileType.M.ToString());
				}
			}
            
			//2012/06/27追加↑ T.Sasaki---------------------------------------------[end]

			if (noFileText.Length != 0)
			{
				Thread.Sleep(commonSettingInfo.DBMachineLogOutWaitmSec);
				if (isOutputCommonPath == true)
				{
					MoveOwnFileFromCommonPath(dirCommonOutput, lsetInfo.InputFolderNM, lsetInfo.MachineSeqNO, isOutputCommonPath, MACHINE_SEQNO_START_INDEX_ON_FILENAME, MACHINE_SEQNO_LEN);
				}
				CheckStartFileExist(targetDirPath, startID, startDT, noFileText.ToString(), lsetInfo, isOutputCommonPath, dirCommonOutput);
			}
		}

		/// <summary>
		/// 対象フォルダのファイルからマガジン連番リストを取得
		/// </summary>
		/// <param name="targetDirPath">対象フォルダ</param>
		/// <param name="timing">装置タイミング</param>
		/// <returns>マガジン連番リスト</returns>
		public List<string> GetMagazineIDData(bool waitForRenameByArmsFg, string targetDirPath, MachineTiming timing, bool outputSFileFg)
		{
			List<string> rMagazineIDList = new List<string>();
			List<string> fileList = new List<string>();

			if (waitForRenameByArmsFg)
			{
				fileList = Common.GetFiles(targetDirPath, ".*_.*_.*_.*_.*"); //ARMSのリネーム待ち
			}
			else
			{
				fileList = new List<string>(Directory.GetFiles(targetDirPath)); //
			}
			foreach (string file in fileList)
			{
				string magazineID = string.Empty;
				if (timing == MachineTiming.Start)
				{
					string fileNM = Path.GetFileNameWithoutExtension(file);
					//2012/06/27 Mファイル追加 T.Sasaki
					if (!Regex.IsMatch(fileNM, "^O.*...$") && !Regex.IsMatch(fileNM, "^P.*...$") && !Regex.IsMatch(fileNM, "^M.*...$"))
					{
						continue;
					}

					//スタートファイルは拡張子がスタート連番
					magazineID = Path.GetExtension(file).Replace(".", "");
				}
				else
				{
					string fileNM = Path.GetFileNameWithoutExtension(file);

                    if (outputSFileFg)
                    {
                        if (!Regex.IsMatch(fileNM, @"^H.*...$")
                            && !Regex.IsMatch(fileNM, "^I.*...$") && !Regex.IsMatch(fileNM, "^L.*...$") && !Regex.IsMatch(fileNM, "^S.*...$"))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!Regex.IsMatch(fileNM, @"^H.*...$")
                            && !Regex.IsMatch(fileNM, "^I.*...$") && !Regex.IsMatch(fileNM, "^L.*...$"))
                        {
                            continue;
                        }
                    }

                    if (waitForRenameByArmsFg)
					{
						magazineID = fileNM.Substring(fileNM.IndexOf("_", 1) - 3, 3);
					}
					else
					{
						//マガジンファイルは後部3文字がマガジン連番
						magazineID = fileNM.Substring(fileNM.Length - 3, 3);
					}
				}

				if (!rMagazineIDList.Exists(f => f == magazineID))
				{
					rMagazineIDList.Add(magazineID);
				}
			}

			return rMagazineIDList.OrderBy(s => s).ToList();
		}

		public static int GetCol(string modelNm, int defaultCol)
		{
			// 『+ "-2"』をしている箇所、"DB-2"の箇所は閾値上位への切り替えの為の一時的なコードの為、閾値上位の切替な完了後に削除して問題なし
			if (modelNm.ToUpper() == Convert.ToString(ModelType.AD830) || modelNm.ToUpper() == Convert.ToString(ModelType.AD830) + "-2" || modelNm.ToUpper() == "DB" || modelNm.ToUpper() == "DB-2")
			{
				return defaultCol + AD830_WITH_DEFAULT_COL_GAP;
			}
			else if (modelNm.ToUpper() == Convert.ToString(ModelType.AD8930V) || modelNm.ToUpper() == Convert.ToString(ModelType.AD8930V) + "-2")
			{
				return defaultCol + AD8930V_WITH_DEFAULT_COL_GAP;
			}
			else if (modelNm.ToUpper() == Convert.ToString(ModelType.AD8930) || modelNm.ToUpper() == Convert.ToString(ModelType.AD8930) + "-2")
			{
				return defaultCol + AD8930_WITH_DEFAULT_COL_GAP;
			}
			else if (modelNm.ToUpper() == Convert.ToString(ModelType.AD838L) || modelNm.ToUpper() == Convert.ToString(ModelType.AD838L) + "-2")
			{
				return defaultCol + AD838L_WITH_DEFAULT_COL_GAP;
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_157, "列取得", modelNm));
			}
		}

		#region 各ファイル処理

		/// <summary>
		/// Oファイル   [O:装置設定パラメータ]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="MagInfoDB">マガジン情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		/// <param name="nTimmingMode">ログファイル作成タイミング</param>
		/// <returns>装置処理状態ステータス</returns>
		public void DbInput_DB_OFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, int nTimmingMode, ref List<ErrMessageInfo> errMessageList)
		{
			//全ファイル内容取得(1行)
			string[] fileLineValue = GetMachineFileLineValue(sFilePath);

			//Oファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.O), lsetInfo, magInfo.sMaterialCD);

			List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.O));
			//	throw new Exception(message);
			//}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要な文字長が型式で違う為。
				int needLength = 0;
				if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930V))
				{
					needLength = FILE_NEED_LENGTH_O_AD8930V;
				}
				else if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930))
				{
					needLength = FILE_NEED_LENGTH_O_AD8930;
				}
				else if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD838L))
				{
					needLength = FILE_NEED_LENGTH_O_AD838L;
				}
				else
				{
					needLength = FILE_NEED_LENGTH_O_AD830;
				}

				//必要ファイル内容取得
				FileValueInfo sFileValueInfo = null;
				if (plmInfo.TotalKB != Convert.ToString(Constant.CalcType.SP))
				{
					sFileValueInfo = GetFileValue(fileLineValue, filefmtInfo.ColumnNO, needLength, sFilePath);
				}
				else
				{
					//特殊な取得処理
					//if (plmInfo.ParameterNM.Contains(O_PARAM_PICKLEVELLIMIT) 
					//                            || plmInfo.ParameterNM.Contains(O_PARAM_BONDLEVELLIMIT)) 
					if (plmInfo.RefQcParamNO == null || plmInfo.RefQcParamNO == 0)  //plmInfoの参照QcParamNoが存在するかどうかで、ifの中に入る。入った後は、参照QcParamNoで前回値の項目を取得 (2013/9/19改修 n.yoshimoto)
					{                                           //(BTS.2099 2013/9/19 n.yoshimoto BTS.2099では差分データが登録されているが、DB仕様では差分データが登録されてない為、DBにはBTS.2099機能は適用しない）
						throw new Exception(string.Format("TmPRMのTotalKBが設定されていますが、パラメタ名が前回取得値の差異集計の対象ではありません。QcParamNO = {0}", plmInfo.QcParamNO));
					}
					else
					{
						//前回取得値差異
						//sFileValueInfo = GetFileValueGap(fileLineValue, filefmtInfo.ColumnNO, needLength, lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, sFilePath);
						sFileValueInfo = GetFileValueGap(fileLineValue, filefmtInfo.ColumnNO, needLength, lsetInfo.EquipmentNO, plmInfo.RefQcParamNO.Value, sFilePath);
					}
				}

				switch (nTimmingMode)
				{
					case Constant.nStartTimming:
						//異常判定
						OutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL, sFileValueInfo.MeasureDT, ref errMessageList);
						break;

					case Constant.nMagazineTimming:
						//DB登録
						ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL, sFileValueInfo.MeasureDT);
						break;

					case Constant.nStartTimmingNMC:
						//異常判定+DB登録
						ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL, sFileValueInfo.MeasureDT, ref errMessageList);
						break;

					default:
						throw new Exception(string.Format(Constant.MessageInfo.Message_47, sFilePath));
				}
			}
		}

		/// <summary>
		/// Pファイル   [P:Package data name]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="MagInfoDB">マガジン情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		/// <param name="nTimmingMode">ログファイル作成タイミング</param>
		/// <returns>装置処理状態ステータス</returns>
		public void DbInput_DB_PFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, int nTimmingMode, ref List<ErrMessageInfo> errMessageList)
		{
			//全ファイル内容取得(1行)
			string[] fileLineValue = GetMachineFileLineValue(sFilePath);

			//Pファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.P), lsetInfo, magInfo.sMaterialCD);

			List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.P));
			//	throw new Exception(message);
			//}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);
				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要ファイル内容取得
				FileValueInfo sFileValueInfo = GetFileValue(fileLineValue, filefmtInfo.ColumnNO, FILE_NEED_LENGTH_P, sFilePath);

				switch (nTimmingMode)
				{
					case Constant.nStartTimming:
						//異常判定
						OutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT, ref errMessageList);
						break;

					case Constant.nMagazineTimming:
						//DB登録
						ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT);
						break;

					case Constant.nStartTimmingNMC:
						//異常判定+DB登録
						ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT, ref errMessageList);
						break;
					default:
						throw new Exception(string.Format(Constant.MessageInfo.Message_47, sFilePath));
				}
			}
		}

		/// <summary>
		/// Mファイル   [M:装置パラメータ拡張]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="magInfo">マガジン情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		/// <param name="nTimmingMode">ログファイル作成タイミング</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		public void DbInput_DB_MFile(LSETInfo lsetInfo, MagInfo magInfo, string sFilePath, int nTimmingMode, ref List<ErrMessageInfo> errMessageList)
		{
			//全ファイル内容取得(1行)
			string[] fileLineValue = GetMachineFileLineValue(sFilePath);

			//Mファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.M), lsetInfo, magInfo.sMaterialCD);

			List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.M));
			//	throw new Exception(message);
			//}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要な文字長が型式で違う為。
				int needLength = 0;
				if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930V))
				{
					needLength = FILE_NEED_LENGTH_M_AD8930V;
				}
				else if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD8930))
				{
					needLength = FILE_NEED_LENGTH_M_AD8930;
				}
				else if (lsetInfo.ModelNM == Convert.ToString(ModelType.AD838L))
				{
					needLength = FILE_NEED_LENGTH_M_AD838L;
				}
				else
				{
					needLength = FILE_NEED_LENGTH_M_AD830;
				}

				//ファイル内容取得
				FileValueInfo sFileValueInfo = GetFileValue(fileLineValue, filefmtInfo.ColumnNO, needLength, sFilePath);

				switch (nTimmingMode)
				{
					case Constant.nStartTimming:
						//異常判定
						OutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT, ref errMessageList);
						break;

					case Constant.nMagazineTimming:
						//DB登録
						ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT);
						break;

					case Constant.nStartTimmingNMC:
						//異常判定+DB登録
						ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, sFileValueInfo.TargetStrVAL.ToUpper(), sFileValueInfo.MeasureDT, ref errMessageList);
						break;
					default:
						throw new Exception(string.Format(Constant.MessageInfo.Message_47, sFilePath));
				}
			}
		}

        /// <summary>
        /// Sファイル   [S:Substrate Inspection]
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <param name="mFilePath">ログファイル保存先</param>
        /// <param name="nLEDNum">ダイス搭載数</param>
        /// <param name="errMessageList">異常内容リスト</param>
        /// <returns>装置処理状態ステータス</returns>
        public void DbInput_DB_SFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList, int fileLen, int? errCdCol, FileType fileType)
        {
            //全ファイル内容取得(複数行)
            string[] fileLineValue = GetMachineFileLineValue(mFilePath);

            int? diceCt = GetDiceCountFromFileContents(fileLineValue, DICE_NO_INDEX_S);
            if (diceCt.HasValue == false)
            {
                string errMsg = string.Format("ﾀﾞｲｽ数が出力ﾌｧｲﾙから取得できませんでした。対象ﾌｧｲﾙ:{0}", mFilePath);
                throw new ApplicationException(errMsg);
            }
            using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD)))
            {
                var list = eicsDB.TmBonding.Where(b => b.Type_CD == magInfo.sMaterialCD && b.Proc_NO == magInfo.ProcNO);
                if (list.Count() == 0)
                {
                    DataContext.TmBonding b = new DataContext.TmBonding { Type_CD = magInfo.sMaterialCD, Proc_NO = magInfo.ProcNO, ChipBond_CT = diceCt.Value, LastUpd_DT = System.DateTime.Now };
                    eicsDB.TmBonding.InsertOnSubmit(b);
                    eicsDB.SubmitChanges();
                }
            }

            //Sファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.S), lsetInfo, magInfo.sMaterialCD);

            int? maxDiceKB = Prm.GetMaxDieKB(lsetInfo.InlineCD, filefmtList.Select(f => f.QCParamNO).ToList(), lsetInfo.ChipNM);
            if (maxDiceKB.HasValue)
            {
                if (diceCt.Value > maxDiceKB)
                {
                    string errMsg = string.Format(
                        "出力ﾌｧｲﾙから取得したﾀﾞｲｽ最大数がﾏｽﾀから取得したﾀﾞｲｽ最大数より多い為処理できません。ﾌｧｲﾙ側:{0} / ﾏｽﾀ側:{1} / 対象ﾌｧｲﾙ:{2}",
                        diceCt, maxDiceKB, mFilePath);
                    throw new ApplicationException(errMsg);
                }
            }

            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

            FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

            filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
#if Debug
                if (filefmtInfo.QCParamNO == 110)
                {
                    Console.WriteLine("");
                }
#endif
                //閾値マスタ情報(TmPLM)取得
                //PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
                Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                //必要ファイル内容取得
                FileValueInfo mFileValueInfo = null;
                if (plmInfo.TotalKB != Convert.ToString(Constant.CalcType.SP))
                {
                    //右、左スタンプ取得
                    mFileValueInfo
                        = GetFileValueMulti(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.TotalKB, diceCt.Value, plmInfo.DieKB, plmInfo.ParameterNM, mFilePath, lsetInfo, magInfo, ref errMessageList);
                }
                else
                {
                    //X,Y,AREA取得
                    mFileValueInfo
                        = GetFileValueXYA(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.ParameterNM, mFilePath, lsetInfo, ref errMessageList, filefmtInfo.SearchNM);
                }

                //異常判定+DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(mFileValueInfo.TargetStrVAL), mFileValueInfo.MeasureDT, ref errMessageList);
            }
        }

        /// <summary>
        /// Hファイル   [H:Pre bond全データ]
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <param name="mFilePath">ログファイル保存先</param>
        /// <param name="nLEDNum">ダイス搭載数</param>
        /// <param name="errMessageList">異常内容リスト</param>
        /// <returns>装置処理状態ステータス</returns>
        public void DbInput_DB_HFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList, int fileLen, int? errCdCol, FileType fileType)
		{
			//全ファイル内容取得(複数行)
			string[] fileLineValue = GetMachineFileLineValue(mFilePath);

            int? diceCt = GetDiceCountFromFileContents(fileLineValue, GetCol(lsetInfo.ModelNM, DICE_NO_INDEX_H));
            if (diceCt.HasValue == false)
            {
                string errMsg = string.Format("ﾁｯﾌﾟ数が出力ﾌｧｲﾙから取得できませんでした。対象ﾌｧｲﾙ:{0}", mFilePath);
                throw new ApplicationException(errMsg);
            }
            using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD)))
            {
                var list = eicsDB.TmBonding.Where(b => b.Type_CD == magInfo.sMaterialCD && b.Proc_NO == magInfo.ProcNO);
                if (list.Count() == 0)
                {
                    DataContext.TmBonding b = new DataContext.TmBonding { Type_CD = magInfo.sMaterialCD, Proc_NO = magInfo.ProcNO, ChipBond_CT = diceCt.Value, LastUpd_DT = System.DateTime.Now };
                    eicsDB.TmBonding.InsertOnSubmit(b);
                    eicsDB.SubmitChanges();
                }
            }

            //Hファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.H), lsetInfo, magInfo.sMaterialCD);

            int? maxDiceKB = Prm.GetMaxDieKB(lsetInfo.InlineCD, filefmtList.Select(f => f.QCParamNO).ToList(), lsetInfo.ChipNM);
            if (maxDiceKB.HasValue)
            {
                if (diceCt.Value > maxDiceKB)
                {
                    string errMsg = string.Format(
                        "出力ﾌｧｲﾙから取得したﾁｯﾌﾟ最大数がﾏｽﾀから取得したﾁｯﾌﾟ最大数より多い為処理できません。ﾌｧｲﾙ側:{0} / ﾏｽﾀ側:{1} / 対象ﾌｧｲﾙ:{2}",
                        diceCt, maxDiceKB, mFilePath);
                    throw new ApplicationException(errMsg);
                }
            }

            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
#if Debug
                    if (filefmtInfo.QCParamNO == 110)
                    {
                        Console.WriteLine("");
                    }
#endif
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要ファイル内容取得
				FileValueInfo mFileValueInfo = null;
				if (plmInfo.TotalKB != Convert.ToString(Constant.CalcType.SP))
				{
					//右、左スタンプ取得
					mFileValueInfo
						= GetFileValueMulti(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.TotalKB, diceCt.Value, plmInfo.DieKB, plmInfo.ParameterNM, mFilePath, lsetInfo, magInfo, ref errMessageList);
				}
				else
				{
					//X,Y,AREA取得
					mFileValueInfo
						= GetFileValueXYA(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.ParameterNM, mFilePath, lsetInfo, ref errMessageList, filefmtInfo.SearchNM);
				}

                //異常判定+DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(mFileValueInfo.TargetStrVAL), mFileValueInfo.MeasureDT, ref errMessageList);
            }
		}

		/// <summary>
		/// Iファイル   [I:Post bond全データ]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="magInfo">マガジン情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		/// <param name="nLEDNum">ダイス搭載数</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		public void DbInput_DB_IFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList, int fileLen, int? errCdCol, FileType fileType)
		{
			//全ファイル内容取得(複数行)
			string[] fileLineValue = GetMachineFileLineValue(mFilePath);

            int? diceCt = GetDiceCountFromFileContents(fileLineValue, GetCol(lsetInfo.ModelNM, DICE_NO_INDEX_I));
            if (diceCt.HasValue == false)
            {
                string errMsg = string.Format("ﾁｯﾌﾟ数が出力ﾌｧｲﾙから取得できませんでした。対象ﾌｧｲﾙ:{0}", mFilePath);
                throw new ApplicationException(errMsg);
            }

            //Iファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.I), lsetInfo, magInfo.sMaterialCD);

            int? maxDiceKB = Prm.GetMaxDieKB(lsetInfo.InlineCD, filefmtList.Select(f => f.QCParamNO).ToList(), lsetInfo.ChipNM);
            if (maxDiceKB.HasValue)
            {
                if (diceCt > maxDiceKB)
                {
                    string errMsg = string.Format(
                        "出力ﾌｧｲﾙから取得したﾁｯﾌﾟ最大数がﾏｽﾀから取得したﾁｯﾌﾟ最大数より多い為処理できません。ﾌｧｲﾙ側:{0} / ﾏｽﾀ側:{1} / 対象ﾌｧｲﾙ:{2}",
                        diceCt, maxDiceKB, mFilePath);
                    throw new ApplicationException(errMsg);
                }
            }

            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.I));
			//	throw new Exception(message);
			//}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{

#if Debug
					//if (filefmtInfo.QCParamNO == 134)
					//{
					//    Console.WriteLine("");
					//}
#endif

				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要ファイル内容取得
				FileValueInfo mFileValueInfo = null;
				if (plmInfo.TotalKB != Convert.ToString(Constant.CalcType.SP))
				{
					mFileValueInfo
						= GetFileValueMulti(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.TotalKB, diceCt.Value, plmInfo.DieKB, plmInfo.ParameterNM, mFilePath, lsetInfo, magInfo, ref errMessageList, GetCol(lsetInfo.ModelNM, FILE_ERROR_COL_I));
				}
				else
				{
					//X,Y,SIGMA取得
					mFileValueInfo = GetFileValueXYA(fileLineValue, filefmtInfo.ColumnNO, fileLen, plmInfo.ParameterNM, mFilePath, lsetInfo, ref errMessageList, GetCol(lsetInfo.ModelNM, FILE_ERROR_COL_I), filefmtInfo.SearchNM);
				}

				//異常判定+DB登録
				ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(mFileValueInfo.TargetStrVAL), mFileValueInfo.MeasureDT, ref errMessageList);
			}
		}

		/// <summary>
		/// Lファイル   [L:life Count]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="magInfo">マガジン情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		public void DbInput_DB_LFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList)
		{
			//全ファイル内容取得(1行)
			string[] fileLineValue = GetMachineFileLineValue(mFilePath);

			//Lファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.L), lsetInfo, magInfo.sMaterialCD);

			List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

			FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

			filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.L));
			//	throw new Exception(message);
			//}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, lsetInfo.InlineCD);
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);
				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//必要ファイル内容取得
				FileValueInfo mFileValueInfo = GetFileValue(fileLineValue, filefmtInfo.ColumnNO, FILE_NEED_LENGTH_L, mFilePath);

				//異常判定+DB登録                
				ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, mFileValueInfo.TargetStrVAL, mFileValueInfo.MeasureDT, ref errMessageList);
			}
		}

		/// <summary>
		/// Wファイル　 [W:Wafer Log]
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="mFilePath">ログファイル保存先</param>
		public void DbInput_DB_WFile(LSETInfo lsetInfo, string mFilePath)
		{
			using (ConnectDB conn = new ConnectDB(true, Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD))
			{
				try
				{
					//全ファイル内容取得(複数行)
					string[] fileValue = GetMachineFileLineValue(mFilePath);
					foreach (string fileLineValue in fileValue)
					{
						if (string.IsNullOrEmpty(fileLineValue))
						{
							continue;
						}

						string[] elm = fileLineValue.Split(',');

						//日付、時間、区分に足りない行は飛ばす
						if (elm.Length < 3)
						{
							continue;
						}

						//日付変換に失敗した行は飛ばす
						DateTime dt;
						if (DateTime.TryParse(elm[0] + " " + elm[1], out dt) == false)
						{
							continue;
						}
#if DEBUG
                        conn.InsertDBWF(LineCD, lsetInfo.EquipmentNO, dt, string.Join(",", elm, 2, elm.Length - 2));
#else
						conn.InsertDBWF(LineCD, lsetInfo.EquipmentNO, dt, string.Join(",", elm, 2, elm.Length - 2));
#endif
					}
#if DEBUG
#else
					conn.Connection.Commit();
#endif

				}
				catch (Exception err)
				{
					conn.Connection.Rollback();
					throw new ApplicationException(err.ToString());
				}
			}
		}

		#region DbInput_DB_BFile

		public void TEST_DbInput_DB_BFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath)
		{
			DbInput_DB_BFile(lsetInfo, magInfo, mFilePath);
		}

		/// <summary>
		/// バッドマーク不良データを読み込んで不良登録実施
		/// </summary>
		/// <param name="mag"></param>
		/// <returns></returns>
		private void DbInput_DB_BFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath)
		{
			//string[] files = System.IO.Directory.GetFiles(DefectFileBasePath, Convert.ToString(FileType.B) + "*");

			SettingInfo settingInfo = SettingInfo.GetSingleton();
			AlertLog alertLog = AlertLog.GetInstance();

			//if (magInfo.StartDT > magInfo.EndDT)
			//{
			//    alertLog.logMessageQue.Enqueue(string.Format("マガジンの終了時間が取得出来なかったか、値が不正です。Bファイル処理をパスします。マガジン:{0}/開始時間:{1}/終了時間:{2}", magInfo.sMagazineNO, magInfo.StartDT, magInfo.EndDT));
			//    return false;
			//}

			//string path = null;
			//最新の更新日付のデータのみ対象
			//DateTime lastWrite = DateTime.MinValue;
			//foreach (string file in files)
			//{
			//    FileInfo fi = new FileInfo(file);
			//    if (lastWrite >= fi.LastWriteTime)
			//    {
			//        continue;
			//    }
			//    path = file;
			//    lastWrite = fi.LastWriteTime;
			//}

			if (File.Exists(mFilePath) == false)
			{
				alertLog.logMessageQue.Enqueue("BMカウントがONですが、Bファイルがありません。ファイルパス：" + mFilePath);
				return;
			}

			int ct = 0;
			using (StreamReader sr = new StreamReader(mFilePath))
			{
				while (sr.Peek() > 0)
				{
					string line = sr.ReadLine();

					if (!string.IsNullOrEmpty(line))
					{
						string[] field = line.Split(',');
						DateTime recordTime = Convert.ToDateTime(field[FILE_MEASUREDATE_DATE] + " " + field[FILE_MEASUREDATE_TIME]);
						if (recordTime >= magInfo.StartDT.AddMinutes(TARGET_BM_RECORD_START_OFFSET_MINUTE))
						{
							ct++;
						}
					}
				}
			}

			//Nasca用不良ファイル作成
			//※不良が無くてもARMSが不良ファイルを待ち続ける為、必ず不良ファイルを作成

			string defectdata = string.Empty;

			if (ct != 0)
			{
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				defectdata = string.Format("{0},{1},{2},{3}", settingInfoPerLine.BMDefectCD, settingInfoPerLine.BMCauseCD, settingInfoPerLine.BMClassCD, ct) + Environment.NewLine;
			}
			NascaDefectFile
				.Create(magInfo.sNascaLotNO, magInfo.sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, defectdata, magInfo.ProcNO);

		}

        #endregion

        #region DbInput_DB_EFile
        /// <summary>
        /// Eファイル   [E:Error Log]
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <param name="mFilePath">ログファイル保存先</param>
        /// <param name="errMessageList">異常内容リスト</param>
        /// <returns>装置処理状態ステータス</returns>
        public void DbInput_DB_EFile(LSETInfo lsetInfo, MagInfo magInfo, string mFilePath, ref List<ErrMessageInfo> errMessageList)
        {
            //全ファイル内容取得(複数行)
            string[] fileLineValue = GetMachineFileLineValue(mFilePath);

            //Eファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.E), lsetInfo, magInfo.sMaterialCD);

            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

            FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, true);

            filefmtList = filefmtList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.QCParamNO));

            //検索文字列に一部のメタ文字が入っている場合、エスケープ文字を追加。$や^は条件に使うので対象外。
            for(int i = 0; i < filefmtList.Count(); i++)
            {
                filefmtList[i].SearchNM = filefmtList[i].SearchNM.Replace(@"(", @"\(")
                                                                 .Replace(@")", @"\)")
                                                                 .Replace(@".", @"\.")
                                                                 .Replace(@"*", @"\*")
                                                                 .Replace(@"?", @"\?");
            }

            //その他エラー項目を取得　※管理項目名に「その他エラー」が含まれるものが対象。(複数ある事は想定しない）
            //そもそもその他エラー項目が無い場合はその他エラーは集計しない。
            FILEFMTInfo otherErrFmt = filefmtList.Where(f => f.ParameterNM.Contains("その他エラー")).FirstOrDefault();


            //エラー数集計
            Dictionary<int,int> errorList = new Dictionary<int, int>();
            if (otherErrFmt != null)
            {
                errorList.Add(otherErrFmt.QCParamNO, 0);
            }

            foreach(string lineStr in fileLineValue)
            {
                bool isHit = false;
                foreach (FILEFMTInfo filefmtInfo in filefmtList)
                {
                    if (string.IsNullOrWhiteSpace(filefmtInfo.SearchNM) == true) continue;

                    if (Regex.IsMatch(lineStr.Trim(), filefmtInfo.SearchNM) == true)
                    {
                        isHit = true;
                        if(errorList.ContainsKey(filefmtInfo.QCParamNO) == true)
                        {
                            errorList[filefmtInfo.QCParamNO] += 1;
                        }
                        else
                        {
                            errorList.Add(filefmtInfo.QCParamNO, 1);
                        }
                        continue;
                    }
                }
                if(isHit == false && otherErrFmt != null)
                {
                    errorList[otherErrFmt.QCParamNO] += 1;
                }
            }

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                if(errorList.ContainsKey(filefmtInfo.QCParamNO) == true)
                {
                    //閾値マスタ情報(TmPLM)取得
                    Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                    string errorCt = Convert.ToString(errorList[filefmtInfo.QCParamNO]);

                    //異常判定+DB登録
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, errorCt, Convert.ToString(File.GetLastWriteTime(mFilePath)), ref errMessageList);
                }

            }
        }

        #endregion

        /// <summary>
        /// バッドマーク不良追加登録
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <param name="procno"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private bool appendDefect(int lineCD, string magNO, string lotNO, long procno, int ct)
		{
			//Arms.Magazine svrmag = Arms.Magazine.GetCurrent(lineCD, magno);
			//if (svrmag == null)
			//{
			//    throw new ApplicationException("マガジン情報が存在しません:" + magno);
			//}

			if (lotNO == null)
			{
				AlertLog alertLog = AlertLog.GetInstance();
				alertLog.logMessageQue.Enqueue(string.Format(Constant.MessageInfo.Message_138, magNO, ct));
				return false;
			}

			Arms.Defect defdata = Arms.Defect.GetDefect(lineCD, lotNO, procno);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			Arms.Defect.DefItem item = new Arms.Defect.DefItem();
			item.CauseCd = settingInfoPerLine.BMCauseCD;
			item.ClassCd = settingInfoPerLine.BMClassCD;
			item.DefectCd = settingInfoPerLine.BMDefectCD;
			item.DefectCt = ct;

			bool exists = false;
			foreach (Arms.Defect.DefItem exd in defdata.DefItems)
			{
				if (exd.CauseCd == item.CauseCd && exd.ClassCd == item.ClassCd && exd.DefectCd == item.DefectCd)
				{
					exd.DefectCt += item.DefectCt;
					exists = true;
				}
			}
			if (!exists)
			{
				defdata.DefItems.Add(item);
			}

			//Log.Write("不良登録:" + svrmag.NascaLotNO + ":" + procno.ToString());
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "不良登録:" + lotNO + ":" + procno.ToString());
			defdata.DeleteInsert(this.LineCD);

			return true;
		}

		/// <summary>
		/// ファイルから必要データを取得
		/// </summary>
		/// <param name="fileLineValue">行データ</param>
		/// <param name="needColumnNO">必要な列</param>
		/// <param name="needLength">必要な文字長</param>
		/// <returns></returns>
		private FileValueInfo GetFileValue(string[] fileLineValue, int needColumnNO, int needLength, string sFilePath)
		{
			string[] fileValue = fileLineValue[0].Split(',');

			if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
			{
				//測定日付が含まれていない場合、ファイル処理しない
				throw new Exception(string.Format(Constant.MessageInfo.Message_45, sFilePath, 1));
			}

			FileValueInfo fileValueInfo = new FileValueInfo();
			fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));
			fileValueInfo.TargetStrVAL =
				fileValue[needColumnNO].Substring(fileValue[needColumnNO].IndexOf("=") + 1,
				fileValue[needColumnNO].Length - fileValue[needColumnNO].IndexOf("=") - 1);

			return fileValueInfo;
		}

		private FileValueInfo GetFileValueMulti
			(string[] fileValueList, int needColumnNO, int needLength, string totalKB, int diceCount, string parameterDiceKB, string parameterNM, string mFilePath, LSETInfo lsetInfo, MagInfo magInfo, ref List<ErrMessageInfo> errMessageList)
		{
			return GetFileValueMulti(fileValueList, needColumnNO, needLength, totalKB, diceCount, parameterDiceKB, parameterNM, mFilePath, lsetInfo, magInfo, ref errMessageList, null);
		}

		/// <summary>
		/// ファイルから必要集計データを取得
		/// </summary>
		/// <param name="fileValueList">行データリスト</param>
		/// <param name="needColumnNO">必要な列</param>
		/// <param name="needLength">必要な文字長</param>
		/// <param name="totalKB">集計区分</param>
		/// <param name="diceCount">ダイス搭載数</param>
		/// <param name="parameterNM">パラメータ名</param>
		/// <param name="mFilePath">ファイルパス(メッセージ表示用)</param>
		/// <param name="lsetInfo">装置情報(メッセージ表示用)</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns></returns>
		private FileValueInfo GetFileValueMulti
			(string[] fileValueList, int needColumnNO, int needLength, string totalKB, int diceCount, string parameterDiceKB, string parameterNM, string mFilePath, LSETInfo lsetInfo, MagInfo magInfo, ref List<ErrMessageInfo> errMessageList, int? extractSkipInfoCol)
		{
			FileValueInfo fileValueInfo = new FileValueInfo();
			List<double> targetValueList = new List<double>();

			List<General> extractSkipInfoList = new List<General>();

			if (extractSkipInfoCol.HasValue)
			{
				extractSkipInfoList = General.GetGeneralData((int)General.GeneralGrp.DBExtractSkipInfo, lsetInfo.InlineCD);
			}

			//int magazineStepMaxNO = fileValueList.Max(f => Convert.ToInt32(f[FILE_MAGAZINESTEP_NO])); 

			if (fileValueList.Length > 0)
			{
				string[] fileValue = fileValueList[fileValueList.Length - 1].Split(',');
				if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
				{
				}
				fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));
			}

			for (int i = 0; i < fileValueList.Length; i++)
			{
				int rowCount = i + 1; //行数
				string[] fileValue = fileValueList[i].Split(',');

				//if (rowCount <= fileValueList.Length - (Constant.SettingInfo.FramePackageNUM * diceCount))
				if (rowCount <= fileValueList.Length - (magInfo.FramePackageCT * diceCount))
				{
					//最後の1フレームまで進める
					continue;
				}

				if (fileValue[GetCol(lsetInfo.ModelNM, FILE_X)].StartsWith(FILE_X_SKIPPED))
				{
					//スキップ行は次へ
					continue;
				}

				if (fileValue.Length != needLength)
				{
					//必要な文字長に満たない場合、メッセージ表示
					string message = string.Format(Constant.MessageInfo.Message_45, mFilePath, rowCount);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
					errMessageList.Add(errMessageInfo);

					continue;
				}

				//ファイルから取得した値の集計をスキップする条件を持つ列(extractSkipInfoCol)が指定されている場合
				if (extractSkipInfoCol.HasValue)
				{
					//Generalの集計スキップ情報に設定された値のリストを取得し、該当するものが無いか照合する。一致するものがある場合、その行は集計に含まない（一行スキップ）
					if (extractSkipInfoList.Exists(e => e.GeneralNM == fileValue[extractSkipInfoCol.Value]))
					{
						continue;
					}
				}

				if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
				{
					//測定日付が含まれていない場合、メッセージ表示
					string message = string.Format(Constant.MessageInfo.Message_45, mFilePath, rowCount);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
					errMessageList.Add(errMessageInfo);

					continue;
				}

				if (string.IsNullOrEmpty(parameterDiceKB))
				{
					//ダイス区分が設定されていない場合、処理停止
					string message = string.Format(Constant.MessageInfo.Message_64, parameterNM);
					throw new Exception(message);
				}

				int parameterDiceCT = 0;
				if (int.TryParse(parameterDiceKB, out parameterDiceCT))
				{
                    if (parameterDiceCT == 0)
                    {
                    }
                    else
                    {
                        parameterDiceCT -= 1;   //※ファイル内ダイスNO列の定義がLED1=0, LED2=1, ...の為、ダイス数から-1する
                        if (Convert.ToInt32(fileValue[GetCol(lsetInfo.ModelNM, FILE_DICENO)]) != parameterDiceCT)
                        {
                            //ダイス搭載数が違うデータの場合、次に進める
                            continue;
                        }
                    }
                }

				if (parameterNM.Contains(FILE_STAMP_RIGHT_KB)
					//&& rowCount > fileValueList.Length - (Constant.SettingInfo.FramePackageNUM * diceCount / 2))
					&& rowCount > fileValueList.Length - (magInfo.FramePackageCT * diceCount / 2))
				{
					//右スタンプは先頭半分のデータを扱う為、違う場合は次に進める
					continue;
				}

				else if (parameterNM.Contains(FILE_STAMP_LEFT_KB)
					//&& rowCount <= fileValueList.Length - (Constant.SettingInfo.FramePackageNUM * diceCount / 2))
					&& rowCount <= fileValueList.Length - (magInfo.FramePackageCT * diceCount / 2))
				{
					//左スタンプは後尾半分のデータを扱う為、違う場合は次に進める
					continue;
				}

				fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));
				targetValueList.Add(Convert.ToDouble(fileValue[needColumnNO]));
			}

#if Debug
                foreach(double value in targetValueList)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, value.ToString());
                }
#endif

			//マスタ設定している計算方法(平均 or σ)で算出
			double fileVAL = double.MinValue;
			if (totalKB == Constant.CalcType.AVE.ToString())
			{
				fileVAL = calcAvg(targetValueList);
			}
			else if (totalKB == Constant.CalcType.SIGMA.ToString())
			{
				double fileAveVAL = calcAvg(targetValueList);
				fileVAL = calcSigma(targetValueList, fileAveVAL);
			}
			fileValueInfo.TargetStrVAL = Convert.ToString(fileVAL);

			return fileValueInfo;
		}

		private FileValueInfo GetFileValueXYA
			(string[] fileValueList, int needColumnNO, int needLength, string parameterNM, string mFilePath, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList, string searchValue)
		{
			return GetFileValueXYA(fileValueList, needColumnNO, needLength, parameterNM, mFilePath, lsetInfo, ref errMessageList, null, searchValue);
		}

		/// <summary>
		/// ファイルから必要集計データ(X,Y,AREA)を取得
		/// </summary>
		/// <param name="fileValueList">行データリスト</param>
		/// <param name="needColumnNO">必要な列</param>
		/// <param name="needLength">必要な文字長さ</param>
		/// <param name="parameterNM">パラメータ名</param>
		/// <param name="mFilePath">ファイルパス(メッセージ表示用)</param>
		/// <param name="lsetInfo">装置情報(メッセージ表示用)</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns></returns>
		private FileValueInfo GetFileValueXYA
			(string[] fileValueList, int needColumnNO, int needLength, string parameterNM, string mFilePath, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList, int? extractSkipInfoCol, string searchValue)
		{
			FileValueInfo fileValueInfo = new FileValueInfo();
			int errorCount = 0;

			if (fileValueList.Length > 0)
			{
				string[] fileValue = fileValueList[fileValueList.Length - 1].Split(',');
				if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
				{
				}
				fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));
			}

			for (int i = 0; i < fileValueList.Length; i++)
			{
				int rowCount = i + 1; //行数
				string[] fileValue = fileValueList[i].Split(',');

				if (fileValue[GetCol(lsetInfo.ModelNM, FILE_X)].StartsWith(FILE_X_SKIPPED))
				{
					//スキップ行は次へ
					continue;
				}

				if (fileValue.Length != needLength)
				{
					//必要な文字長さに満たない場合、メッセージ表示
					string message = string.Format(Constant.MessageInfo.Message_45, mFilePath, rowCount);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
					errMessageList.Add(errMessageInfo);

					continue;
				}
				if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
				{
					//測定日付が含まれていない場合、メッセージ表示
					string message = string.Format(Constant.MessageInfo.Message_45, mFilePath, rowCount);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
					errMessageList.Add(errMessageInfo);

					continue;
				}

                string[] search = searchValue.Split(',');
                if (search.Where(s => s == fileValue[needColumnNO]).Count() != 0)
                {
                    if (parameterNM.Contains(I_PARAM_X_NM) || parameterNM.Contains(H_PARAM_X_NM) || parameterNM.Contains(S_PARAM_X_NM))
                    {
                        if (Convert.ToDecimal(fileValue[GetCol(lsetInfo.ModelNM, FILE_XLIMIT)]) < Math.Abs(Convert.ToDecimal(fileValue[GetCol(lsetInfo.ModelNM, FILE_X)])))
                        {
                            errorCount += 1;
                        }
                    }
                    else if (parameterNM.Contains(I_PARAM_Y_NM) || parameterNM.Contains(H_PARAM_Y_NM) || parameterNM.Contains(S_PARAM_Y_NM))
                    {
                        if (Convert.ToDecimal(fileValue[GetCol(lsetInfo.ModelNM, FILE_YLIMIT)]) < Math.Abs(Convert.ToDecimal(fileValue[GetCol(lsetInfo.ModelNM, FILE_Y)])))
                        {
                            errorCount += 1;
                        }
                    }
                    else if (parameterNM.Contains(I_PARAM_SIGMA_NM) || parameterNM.Contains(H_PARAM_AREA_NM) || parameterNM.Contains(S_PARAM_AREA_NM))
                    {
                        errorCount += 1;
                    }
                    else
                    {
                        // 従来通り、想定されていない管理項目の場合0で登録する仕様
                    }
                }
                
                fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));
			}

			fileValueInfo.TargetStrVAL = Convert.ToString(errorCount);

			return fileValueInfo;
		}

		/// <summary>
		/// ファイルから必要データ(前回取得値との差異)を取得
		/// </summary>
		/// <param name="fileLineValue">行データ</param>
		/// <param name="needColumnNO">必要な列</param>
		/// <param name="needLength">必要な文字長</param>
		/// <param name="equipmentNO">装置NO</param>
		/// <param name="qcParamNO">パラメータNO</param>
		/// <returns></returns>
		private FileValueInfo GetFileValueGap
			(string[] fileLineValue, int needColumnNO, int needLength, string equipmentNO, int refQcParamNO, string sFilePath)
		//private FileValueInfo GetFileValueGap
		//    (string[] fileLineValue, int needColumnNO, int needLength, string equipmentNO, int qcParamNO, string parameterNM, string sFilePath)
		{

			#region BTS2099検証OKで削除可能箇所
			//SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			#endregion

			string[] fileValue = fileLineValue[0].Split(',');
			if (fileValue.Length < needLength)
			{
				//必要な文字長に満たない場合、ファイル処理しない
				throw new Exception(string.Format(Constant.MessageInfo.Message_45, sFilePath, 1));
			}
			if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
			{
				//測定日付が含まれていない場合、ファイル処理しない
				throw new Exception(string.Format(Constant.MessageInfo.Message_45, sFilePath, 1));
			}

			FileValueInfo fileValueInfo = new FileValueInfo();
			fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));

			string targetVAL = fileValue[needColumnNO].Substring(fileValue[needColumnNO].IndexOf("=") + 1,
					fileValue[needColumnNO].Length - fileValue[needColumnNO].IndexOf("=") - 1);

			decimal LotVAL = decimal.MinValue;
			if (!decimal.TryParse(targetVAL, out LotVAL))
			{
				//取得値に問題が有る場合、ファイル処理しない
				throw new Exception(string.Format(Constant.MessageInfo.Message_45, sFilePath, 1));
			}

			#region BTS2099検証OKで削除可能箇所
			//コメント有効範囲・直下★まで PLMInfoでRefQcParamNOを持ち、QcParamNOと旧backQcParamNO(=RefQcParamNO)との紐付けをマスタ持ち出来るようになったので
			//下記の様な、変換は不要。また、タイプによる切換えも不要となる。(BTS.2099 2013/9/19 n.yoshimoto 検証後削除可能)
			//int backQcParamNO = 0;
			//////TODO 後日修正する(MAPしか有効じゃない)
			//if (settingInfo.MapFG || settingInfo.TypeGroup == Constant.TypeGroup.Map.ToString() || settingInfo.TypeGroup == Constant.TypeGroup.AutoMotive.ToString())
			//{
			//    switch (qcParamNO)
			//    {
			//        case 5:
			//            backQcParamNO = 2;
			//            break;
			//        case 6:
			//            backQcParamNO = 3;
			//            break;
			//        case 105:
			//            backQcParamNO = 102;
			//            break;
			//        case 106:
			//            backQcParamNO = 103;
			//            break;
			//        case 1563:					// Pick Level差異限界：Red
			//            backQcParamNO = 1560;	// Pick Level：Red
			//            break;
			//        case 1564:					// Bond Level差異限界：Red
			//            backQcParamNO = 1561;	// Bond Level：Red
			//            break;
			//        case 1599:					// Pick Level差異限界：Blue
			//            backQcParamNO = 1596;	// Pick Level：Blue
			//            break;
			//        case 1600:					// Bond Level差異限界：Blue 
			//            backQcParamNO = 1597;	// Bond Level：Blue
			//            break;
			//        case 1635:					// Pick Level差異限界：Green
			//            backQcParamNO = 1632;	// Pick Level：Green
			//            break;
			//        case 1636:					// Bond Level差異限界：Green
			//            backQcParamNO = 1633;	// Bond Level：Green
			//            break;
			//        default:
			//            throw new Exception("差異限界の判定で想定外のQcParamNoしていない異常");
			//    }
			//}
			////-----------------------------------------
			////TODO 後日修正する(SIDEVIEWしか有効じゃない)
			//else
			//{
			//    switch (parameterNM)
			//    {
			//        case O_PARAM_PICKLEVELLIMIT:
			//            backQcParamNO = 2;
			//            break;
			//        case O_PARAM_BONDLEVELLIMIT:
			//            backQcParamNO = 3;
			//            break;
			//        default:
			//            throw new Exception("異常");
			//    }

			//}
			//---------------------------------------
			//コメント有効範囲終了・★
			#endregion

			//前回取得値(同装置での前ロット取得値)を取得
			decimal backLotVAL = ConnectDB.GetTnLOG_DParam(this.LineCD, equipmentNO, fileValueInfo.MeasureDT, refQcParamNO, 1);
			//decimal backLotVAL = ConnectDB.GetTnLOG_DParam(this.LineCD, equipmentNO, fileValueInfo.MeasureDT, backQcParamNO, 1);
			if (backLotVAL == decimal.MinValue)
			{
				//前回取得値が存在しなかった場合(初期ロット)、異常判定はしないので0を格納
				fileValueInfo.TargetStrVAL = "0";
			}
			else
			{
				//前回取得値との差異を格納
				fileValueInfo.TargetStrVAL = Convert.ToString(Math.Abs(backLotVAL - LotVAL));
			}
			return fileValueInfo;
		}

		#endregion
	}
}
