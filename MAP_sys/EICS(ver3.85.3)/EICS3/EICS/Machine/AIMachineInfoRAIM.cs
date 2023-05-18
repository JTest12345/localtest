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
	class AIMachineInfoRAIM : AIMachineInfo
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
		private Dictionary<int, int> AddrMapColIndexList = new Dictionary<int, int>();


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

		/// <summary>
		/// 検査機(RAIM)・MMファイルでマッピングデータのやり取りをする仕様
		/// </summary>
		/// <param name="lset"></param>
		public AIMachineInfoRAIM(LSETInfo lset) : base(lset)
		{
			AddrMapColIndexList.Add(7, 8);
			AddrMapColIndexList.Add(9, 10);
			AddrMapColIndexList.Add(11, 12);
			AddrMapColIndexList.Add(13, 14);

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

		public static string[] GetFirstDataSkipEmpty(string[] textArray)
		{
			for(int i = DATA_START_ROW; i < textArray.Count(); i++)
			{
				if(string.IsNullOrEmpty(textArray[i].Trim()) == false)
				{
					string[] txtItem = textArray[i].Split(',');

					return txtItem;
				}
			}
			return null;
		}

		/// <summary>
		/// ﾍｯﾀﾞ名の入ったリストから引数指定した対象ﾍｯﾀﾞ名が存在するインデックスを取得
		/// ﾍｯﾀﾞ名が存在しなかった場合は負の値(-1)を返す
		/// </summary>
		/// <param name="headerNmList"></param>
		/// <param name="targetHeaderNm"></param>
		/// <returns></returns>
		private int identifyIndex(List<string> headerNmList, string targetHeaderNm)
		{
			if (headerNmList.Count(h => h == targetHeaderNm) == 1)
			{
				for (int i = 0; i < headerNmList.Count(); i++)
				{
					if (headerNmList[i] == targetHeaderNm)
					{
						return i;
					}
				}
			}
			else
			{
				throw new ApplicationException(string.Format("ﾍｯﾀﾞ名の重複があります"));
			}

			return -1;
		}

		protected Dictionary<int, int> getColIndexSet(string headerRowStr, Dictionary<string, string> addrResultColSetList)
		{
			Dictionary<int, int> colIndexSetList = new Dictionary<int, int>();

			List<string> headerNmList = headerRowStr.Split(',').ToList();

			foreach (KeyValuePair<string, string> colSet in addrResultColSetList)
			{
				int addrIndex, resultIndex;

				addrIndex = identifyIndex(headerNmList, colSet.Key);

				resultIndex = identifyIndex(headerNmList, colSet.Value);

				if (addrIndex >= 0 && resultIndex >= 0)
				{
					colIndexSetList.Add(addrIndex, resultIndex);
				}

				string refHeaderNm = string.Empty;

				if (addrIndex < 0)
				{
					refHeaderNm += string.Format("『{0}』,", colSet.Key);
				}

				if (resultIndex < 0)
				{
					refHeaderNm += string.Format("『{0}』,", colSet.Value);
				}

				if (string.IsNullOrEmpty(refHeaderNm))
				{
					throw new ApplicationException(string.Format("ﾌｧｲﾙからﾍｯﾀﾞ位置の特定が出来ませんでした。参照ﾍｯﾀﾞ名:{0}", refHeaderNm));
				}
			}

			if (cameraGr1ColSetList.Count == colIndexSetList.Count)
			{
				return colIndexSetList;
			}
			else
			{
				throw new ApplicationException(string.Format(
					"ﾌｧｲﾙから必要な数のﾍｯﾀﾞ位置の特定が出来ませんでした。特定できたﾍｯﾀﾞ位置:{0}"
					, string.Join(",", colIndexSetList.Keys, colIndexSetList.Values)));
			}
		}

		public static string[] UnionMMData(string[] textArray, Dictionary<int, int> addrMapColList)
		{
			List<string> unionMMData = new List<string>();

			try
			{
				string[] firstData = GetFirstDataSkipEmpty(textArray);

				if (firstData == null)
				{
					throw new ApplicationException("ﾌｧｲﾙから処理可能なデータが見つかりませんでした。");
				}

				List<int> copyTargetColList = new List<int>();
				List<int> copyBaseTargetColList = new List<int>();

				foreach (KeyValuePair<int, int> addrMapCol in addrMapColList)
				{
					copyTargetColList.Add(addrMapCol.Key);
					copyTargetColList.Add(addrMapCol.Value);
				}

				for (int i = 0; i < firstData.Count(); i++)
				{
					if (copyTargetColList.Contains(i) == false)
					{
						copyBaseTargetColList.Add(i);
					}
				}

				addrMapColList = addrMapColList.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value);

				KeyValuePair<int, int> originAddrMapCol = addrMapColList.Take(1).Single();

				Dictionary<int, int> targetAddrMapCol = addrMapColList.ToDictionary(a => a.Key, a => a.Value);

				int dataNo = 0;

				for (int i = 0; i < DATA_START_ROW; i++)
				{
					unionMMData.Add(textArray[i]);
				}

				foreach (string lineTxt in textArray.Skip(DATA_START_ROW))
				{
					if (string.IsNullOrEmpty(lineTxt))
					{
						continue;
					}

					string[] txtItem = lineTxt.Split(',');

					foreach (KeyValuePair<int, int> addrMapCol in targetAddrMapCol)
					{
						string[] unionTxtItem = new string[copyBaseTargetColList.Count + MAPINFO_COL_NUM_AFTER_UNION];

						foreach (int copyIndex in copyBaseTargetColList)
						{
							unionTxtItem[copyIndex] = txtItem[copyIndex];
						}

						unionTxtItem[originAddrMapCol.Key] = txtItem[addrMapCol.Key];
						unionTxtItem[originAddrMapCol.Value] = txtItem[addrMapCol.Value];

						unionTxtItem[0] = (dataNo).ToString();

						dataNo++;

						unionMMData.Add(string.Join(",", unionTxtItem));
					}
				}

				return unionMMData.ToArray();
			}
			catch (Exception err)
			{
				throw;
			}
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
				case "MM":/*3*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:MM File"));
					MMFile mmFile = new MMFile(this);

					string[] unionTextArray = UnionMMData(textArray, AddrMapColIndexList);

					//string[] unionTextArray = UnionMMData(textArray, getColIndexSet(textArray[HEADER_ROW], cameraGr1ColSetList));

					mmFile.Run(lsetInfo, magInfo, unionTextArray, ref errMessageList, COL_KEIKOUFG, COL_ADDRESS, COL_CHANGEFG, COL_TRANCD, TimingNO, COL_OPENNINGCHKFG);
					isMMFileNothing = false;

					if (enableOpeningChk)
					{
						OpeningFileProcess(mmFile.OpeningCheckFileType, fileFullPath);
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:MM File"));
					break;
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

		public override void SendStopSignalToMachine(LSETInfo lsetInfo, string plcMemAddr)
		{
			TcpClient tcp = new TcpClient();
			NetworkStream ns = null;

			KLinkInfo.SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, lsetInfo.PortNO, plcMemAddr, 1, 1, ".U");
		}

		protected bool IsFinishedLENSProcess(string lotNo, int procNo, int lineCD, string carrierNo, string plantCd)
		{
			return Database.LENS.WorkResult.IsComplete(lotNo, procNo, lineCD, carrierNo, plantCd);
		}
	}
}
