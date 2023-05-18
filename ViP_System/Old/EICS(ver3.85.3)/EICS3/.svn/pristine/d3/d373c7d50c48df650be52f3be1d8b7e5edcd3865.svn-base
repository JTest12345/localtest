using EICS.Structure;
using LENS2_Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	/// <summary>
	/// 検査機(RAIM)・PLCでマッピングデータのやり取りをする仕様
	/// </summary>
	class AIMachineInfoRAIM2 : AIMachineInfoRAIM
	{
		#region 定数定義
		private const string trigFileExt = "fin";

		private Dictionary<string, string> cameraGr1ColSetList = new Dictionary<string, string>();
		private Dictionary<string, string> cameraGr2ColSetList = new Dictionary<string, string>();

		/// <summary>低倍率カメラの列名</summary>
		private const string cameraGr1Addr1 = "St1ﾏｯﾋﾟﾝｸﾞｱﾄﾞﾚｽ C1";
		private const string cameraGr1Result1 = "検査St1検査結果 C1";
		private const string cameraGr1Addr2 = "St1ﾏｯﾋﾟﾝｸﾞｱﾄﾞﾚｽ C2";
		private const string cameraGr1Result2 = "検査St1検査結果 C2";
		private const string cameraGr1Addr3 = "St2ﾏｯﾋﾟﾝｸﾞｱﾄﾞﾚｽ C3";
		private const string cameraGr1Result3 = "検査St2検査結果 C3";
		private const string cameraGr1Addr4 = "St2ﾏｯﾋﾟﾝｸﾞｱﾄﾞﾚｽ C4";
		private const string cameraGr1Result4 = "検査St2検査結果 C4";

		//protected override string CARRIER_NO_PLC_ADDRESS() { return "EM30000"; }
		//protected override int CARRIER_NO_PLC_ADDRESS_LENGTH() { return 1000; }


		/// <summary>高倍率カメラの列名</summary>
		private const string cameraGr2Addr1 = "";
		private const string cameraGr2Result1 = "";

		private void initCameraGr1ColumnSet()
		{
			if (cameraGr1ColSetList.Count == 0)
			{
				cameraGr1ColSetList.Add(cameraGr1Addr1, cameraGr1Result1);
				cameraGr1ColSetList.Add(cameraGr1Addr2, cameraGr1Result2);
				cameraGr1ColSetList.Add(cameraGr1Addr3, cameraGr1Result3);
				cameraGr1ColSetList.Add(cameraGr1Addr4, cameraGr1Result4);
			}
		}

		private void initCameraGr2ColumnSet()
		{
			if (cameraGr2ColSetList.Count == 0)
			{
				cameraGr2ColSetList.Add(cameraGr2Addr1, cameraGr2Result1);
			}
		}

		private const int COL_KEIKOUFG = 4;
		private const int COL_CHANGEFG = -1;
		/// <summary>ファイル内容(アドレスNO)</summary>
		private const int COL_ADDRESS = 7;
		/// <summary>ファイル内容(処理CD)</summary>
		private const int COL_TRANCD = 8;
		private const int COL_OPENNINGCHKFG = 5;

		private const int DATA_START_ROW = 2;
		private const int HEADER_ROW = 1;
		private const int MAPINFO_COL_NUM_AFTER_UNION = 2;

		public char FileNmSplitter { get; set; }
		public int FileKindIndexInFileNm { get; set; }
		public int DateIndex { get; set; }
		public int DateLen { get; set; }
		public int EndFileIdLen { get; set; }
		public int IdentityIndexInFileNm { get; set; }
		public int TypeCdIndexInFileNm { get; set; }
		public int LotNoIndexInFileNm { get; set; }
		public int MagNoIndexInFileNm { get; set; }
		public int ProcNoIndexInFileNm { get; set; }
		public string EndFileDir { get; set; }

		#endregion

		public AIMachineInfoRAIM2(LSETInfo lset) : base(lset)
		{
			initCameraGr1ColumnSet();
			initCameraGr2ColumnSet();

			lsetInfo = lset;
			this.LineCD = lset.InlineCD;

			if (lset.EnableResultPriorityJudge_FG)
			{
				RunningLog runLog = RunningLog.GetInstance();
				runLog.logMessageQue.Enqueue("【チェック】装置の検査NG優先度設定 取得テスト 開始");
				GetResultPriority(lset.IPAddressNO);
				runLog.logMessageQue.Enqueue("【チェック】装置の検査NG優先度設定 取得テスト 終了");
			}

			FileNmSplitter = CIFS.FILE_NM_SPLITTER;
			FileKindIndexInFileNm = CIFSBasedMachine.FILEKIND_INDEX_IN_FILENM;

			IdentityIndexInFileNm = CIFSBasedMachine.IDENTITY_INDEX_IN_FILENM;
			TypeCdIndexInFileNm = CIFSBasedMachine.TYPECD_INDEX_IN_FILENM;
			LotNoIndexInFileNm = CIFSBasedMachine.LOTNO_INDEX_IN_FILENM;
			MagNoIndexInFileNm = CIFSBasedMachine.MAGNO_INDEX_IN_FILENM;
			ProcNoIndexInFileNm = CIFSBasedMachine.PROC_INDEX_IN_FILENM;
		}

		protected override void FileProcessSelector(string sFileType, LSETInfo lsetInfo, MagInfo magInfo, string[] textArray, string fileFullPath, ref List<ErrMessageInfo> errMessageList)
		{
			string lotNo;
			SMFile9Cam smFile;

			if (magInfo != null && string.IsNullOrEmpty(magInfo.sNascaLotNO) == false)
			{
				lotNo = magInfo.sNascaLotNO;
			}
			else
			{
				lotNo = string.Empty;
			}

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			bool enableOpeningChk = settingInfoPerLine.IsEnableOpeningChk(lsetInfo.EquipmentNO);

			//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
			switch (sFileType)
			{
				case "OA":/*2*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:OA File"));
					OAFile9Cam oaFile = new OAFile9Cam(lsetInfo.InlineCD);

					oaFile.Run(lsetInfo, textArray, ref errMessageList, COL_KEIKOUFG, COL_OPENNINGCHKFG);

					lotNo = oaFile.LotNo;

					if (enableOpeningChk)
					{
						OpeningFileProcess(oaFile.OpeningCheckFileType, fileFullPath);
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:OA File"));
					break;
				case "SM1":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM1 File"));
					smFile = new SMFile9Cam(lsetInfo.InlineCD);
					smFile.Run(lsetInfo, textArray, ref errMessageList, "SM1", COL_KEIKOUFG, COL_OPENNINGCHKFG, null);
					lotNo = smFile.LotNo;

					if (enableOpeningChk)
					{
						OpeningFileProcess(smFile.OpeningCheckFileType, fileFullPath);
					}

					MpdFileProcess(lsetInfo, magInfo);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM1 File"));
					break;
				case "SM2":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM2 File"));
					smFile = new SMFile9Cam(lsetInfo.InlineCD);
					smFile.Run(lsetInfo, textArray, ref errMessageList, "SM2", COL_KEIKOUFG, COL_OPENNINGCHKFG, null);
					lotNo = smFile.LotNo;

					if (enableOpeningChk)
					{
						OpeningFileProcess(smFile.OpeningCheckFileType, fileFullPath);
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM2 File"));
					break;
				case "SM3":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM3 File"));
					smFile = new SMFile9Cam(lsetInfo.InlineCD);
					smFile.Run(lsetInfo, textArray, ref errMessageList, "SM3", COL_KEIKOUFG, COL_OPENNINGCHKFG, null);
					lotNo = smFile.LotNo;

					if (enableOpeningChk)
					{
						OpeningFileProcess(smFile.OpeningCheckFileType, fileFullPath);
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM3 File"));
					break;
			}

			//処理済みファイルを保管フォルダへ移動
			MoveCompleteMachineFile(fileFullPath, lsetInfo, lotNo, "");
		}
	}
}
