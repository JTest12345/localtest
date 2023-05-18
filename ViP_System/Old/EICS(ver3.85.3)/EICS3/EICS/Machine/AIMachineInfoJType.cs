using EICS.Database;
using EICS.Database.LENS;
using EICS.Structure;
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
	/// SLS1 19品種合理化システム立ち上げ時に新規追加（SMﾌｧｲﾙが項目数多により3ﾌｧｲﾙ出力される仕様）
	/// </summary>
	class AIMachineInfoJType : AIMachineInfo
	{
		/// <summary>ファイルMM内容列(傾向管理フラグ)</summary>
		public const int COL_KEIKOUFG = 6;

		/// <summary>ファイルMM内容列(変化点フラグ)</summary>
		public const int COL_CHANGEFG = 7;

		/// <summary>ファイル内容(アドレスNO)</summary>
		public const int COL_ADDRESS = 4;

		/// <summary>ファイル内容(処理CD)</summary>
		public const int COL_TRANCD = 5;

		/// <summary>ファイルOA内容列(傾向管理フラグ)</summary>
		public const int COL_OA_KEIKOUFG = 4;

		/// <summary>ファイルOA内容列(変化点フラグ)</summary>
		public const int COL_OA_CHANGEFG = 46;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM_KEIKOUFG = 4;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM_CHANGEFG = 56;

		public const string CHIP_NM_CO = "CO後";
		public const string CHIP_NM_FC = "FC(LED)後";


		public AIMachineInfoJType(LSETInfo lset) : base(lset)
		{
			lsetInfo = lset;
			this.LineCD = lset.InlineCD;

			if (lset.EnableResultPriorityJudge_FG)
			{
				RunningLog runLog = RunningLog.GetInstance();
				runLog.logMessageQue.Enqueue("【チェック】装置の検査NG優先度設定 取得テスト 開始");
				GetResultPriority(lset.IPAddressNO);
				runLog.logMessageQue.Enqueue("【チェック】装置の検査NG優先度設定 取得テスト 終了");
			}
		}

		//public AIMachineInfoJType(int lineCD) : base(lineCD)
		//{
		//	this.LineCD = lineCD;
		//}

		public int GetTimingNO(LSETInfo lsetInfo)
		{
			//検証中timingNoの切り分けの必要を迫られたが、良い対処方法が思いつかない… 2015/8/3 n.yoshimoto
			if (lsetInfo.ChipNM == null) return 6;

			if (lsetInfo.ChipNM.ToUpper() == CHIP_NM_FC)
			{
				return 6;
			}
			else if (lsetInfo.ChipNM.ToUpper() == CHIP_NM_CO)
			{
				return 24;
			}
			else
			{
				throw new ApplicationException("ChipNMからTimingNOの取得が出来ません。ｼｽﾃﾑ担当者へ連絡して下さい。");
			}
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
#if MANUAL_TABULATION

			List<string> prefixList = new List<string>();

			prefixList.Add("SM1");

			Tabulation(lsetInfo, @"C:\qcil\data\AI\ManualTabulation", @"C:\qcil\data\AI", prefixList);

			return;
#endif
			//timingNOの取得テスト
			GetTimingNO(lsetInfo);

			base.CheckFile(lsetInfo);
		}

		protected override void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string prefixNM, ref List<ErrMessageInfo> errMessageList)
		{
			string fileNmForErrOutput = "ﾌｧｲﾙ名未特定状態";

			try
			{
				string sfname = "";
				string sWork = "";
				//string sFileType = "";
				string[] textArray = new string[] { };
				string spath1 = "", spath2 = "";
				string sMessage = "";

				List<string> fileList = GetMachineLogPathList(lsetInfo, ref tcp, ref ns, fileGetReqFgAddr, lsetInfo.InputFolderNM, ".*" + prefixNM + ".*");

				int nSameTargetFileNum = fileList.Count;
				string[] sortedCreateTime = new string[nSameTargetFileNum];
				int i = 0;
				isMMFileNothing = true;

				//ターゲットファイルが複数あった場合
				if (nSameTargetFileNum > 1)
				{
					i = 0;
					sMessage = lsetInfo.AssetsNM + "/TargetFile=" + prefixNM + "/[" + nSameTargetFileNum + "]ありました。";
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

					//ファイル名から日付文字列取得
					foreach (string swfname in fileList)
					{
						fileNmForErrOutput = swfname;
						sfname = Path.GetFileName(swfname);      //ファイル名取得
						sortedCreateTime[i] = sfname.Substring(9, 10);//ﾌｧｲﾙ名に付加されている日付文字列取得
						i = i + 1;
					}
					Array.Sort(sortedCreateTime);

					for (i = 0; i < nSameTargetFileNum - 1; i++)
					{
						foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, ".*" + prefixNM + sortedCreateTime[i] + ".*"))
						{
							fileNmForErrOutput = swfname;
							//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
							sfname = Path.GetFileName(swfname);	//ファイル名取得
							spath1 = lsetInfo.InputFolderNM + "reserve";
							spath2 = spath1 + "\\" + sfname;

							if (!System.IO.Directory.Exists(spath1))
							{
								System.IO.Directory.CreateDirectory(spath1);
							}
							else
							{
								if (File.Exists(spath2))
								{
									//登録済みﾌｧｲﾙは削除して次へ
									File.Delete(swfname);
									continue;
								}
							}
							//<--人搬送量試時にモールド機が夜間に止まった対応2 2010/09/03 Y.Matsushima
							bool flg;
							for (int j = 0; j < 5; j++)
							{
								flg = GetFileInfo(swfname, lsetInfo.InputFolderNM, ref sfname, ref sWork);
								//成功したら出る
								if (flg == true)
								{
									break;
								}
								//失敗経験あればLogへ出力
								if (j > 1)
								{
									sMessage = Convert.ToString(j) + "回 GetFileInfoで失敗がありました。";
									log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
								}
							}
							//-->人搬送量試時にモールド機が夜間に止まった対応2 2010/09/03 Y.Matsushima

							if (File.Exists(spath2) == false)
							{
								File.Move(swfname, spath2);
							}
							else//既にある場合、delete
							{
								File.Delete(swfname);
							}
						}
					}
				}

				//-->マッピング不一致対策 発生していないが可能性がある為

				//LogファイルデータをDB登録
				List<string> machineFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + prefixNM);
				foreach (string swfname in machineFileList)
				{
					fileNmForErrOutput = swfname;
					FileInfo fileInfo = new FileInfo(swfname);

					//<--人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima
					bool flg;
					for (int j = 0; j < 5; j++)
					{
						flg = GetFileInfo(fileInfo.FullName, lsetInfo.InputFolderNM, ref sfname, ref sWork);
						//成功したら出る
						if (flg == true)
						{
							break;
						}
						//失敗経験あればLogへ出力
						if (j > 1)
						{
							sMessage = Convert.ToString(j) + "回 GetFileInfoで失敗がありました。";
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

						}
					}
					//-->人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima

					textArray = sWork.Split('\n');
					//項目のみの空ﾌｧｲﾙの場合削除
					if (textArray[2] == "")
					{
						File.Delete(fileInfo.FullName);
						continue;
					}

					MagInfo magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL);

					int timingNO = GetTimingNO(lsetInfo);

					//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
					switch (prefixNM)
					{
						case "MM":/*3*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:MM File"));
							MMFile mmFile = new MMFile(this);
							mmFile.Run(lsetInfo, magInfo, textArray, ref errMessageList, COL_KEIKOUFG, COL_ADDRESS, COL_CHANGEFG, COL_TRANCD,timingNO, 0);
							isMMFileNothing = false;
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:MM File"));
							break;
						case "OA":/*2*/

							int errCt = errMessageList.Count();
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:OA File"));
							DbInput_AI_OAFile(lsetInfo, magInfo, textArray, ref errMessageList, COL_OA_KEIKOUFG, COL_OA_CHANGEFG);

							if (errCt < errMessageList.Count())
							{
								SendStopSignalToMachine(ref tcp, ref ns, lsetInfo.IPAddressNO);
							}

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:OA File"));
							break;
						case "SM1":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM1 File"));
							DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM1", ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM1 File"));
							break;
						case "SM2":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM2 File"));
							DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM2", ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM2 File"));
							break;
						case "SM3":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM3 File"));
							DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM3", ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM3 File"));
							break;
					}

					//処理済みファイルを保管フォルダへ移動
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, "");

				}
			}
			catch (Exception err)
			{
				throw new Exception(string.Format("ﾌｧｲﾙ名:{0}に関する処理時に例外発生 ｽﾀｯｸﾄﾚｰｽ:{1} ｴﾗｰ詳細:{2}", fileNmForErrOutput, err.StackTrace, err.Message), err);
			}
		}

		public void SendStopSignalToMachine(ref TcpClient tcp, ref NetworkStream ns, string host)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();

			//「受付準備OKをOFF」
			string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, host, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return;
			}	
		}

		/// <summary>
		/// 集計処理(再集計の為に作成)
		/// </summary>
		/// <param name="dataStorageRootDir">集計対象のファイルがロット毎にまとまったフォルダのあるディレクトリ</param>
		/// <param name="inputDirNm">集計の為に1ロット分のファイルを置く場所</param>
		/// <param name="prefixList">集計対象にするPrefixのリスト</param>
		public void Tabulation(LSETInfo lsetInfo, string dataStorageRootDir, string inputDirNm, List<string> prefixList)
		{
			TcpClient tcp = new TcpClient();
			NetworkStream ns = null;
			string fileGetReqFgAddr = string.Empty;

			
			lsetInfo.IPAddressNO = string.Empty;

			List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();

			List<string> dirList = Directory.GetDirectories(dataStorageRootDir).ToList();
			
			foreach(string dir in dirList)
			{
				//List<string> fileList = Directory.GetFiles(dir).ToList();

				inputDirNm = Path.Combine(inputDirNm, dir.Split('\\').Last());

				lsetInfo.InputFolderNM = inputDirNm;

				Directory.Move(dir, inputDirNm);

				foreach (string prefixNM in prefixList)
				{
					FileDistribution(ref tcp, ref ns, fileGetReqFgAddr, lsetInfo, prefixNM, ref errMessageList);
				}
			}
		}
	}
}
