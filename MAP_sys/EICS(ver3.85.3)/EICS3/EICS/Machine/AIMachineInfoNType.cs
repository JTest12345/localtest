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
	class AIMachineInfoNType : AIMachineInfo
	{
		/// <summary>ファイルOA内容列(傾向管理フラグ)</summary>
		public const int COL_OA_KEIKOUFG = 5;

		/// <summary>ファイルOA内容列(変化点フラグ)</summary>
		public const int COL_OA_CHANGEFG = 6;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM_KEIKOUFG = 5;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM_CHANGEFG = 6;

		public AIMachineInfoNType(LSETInfo lset) : base(lset)
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

		protected override void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string prefixNM, ref List<ErrMessageInfo> errMessageList)
		{
			try
			{
				string sfname = "";
				string sWork = "";
				string sFileType = "";
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
						sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
						sortedCreateTime[i] = sfname.Substring(9, 10);//ﾌｧｲﾙ名に付加されている日付文字列取得
						i = i + 1;
					}
					Array.Sort(sortedCreateTime);

					for (i = 0; i < nSameTargetFileNum - 1; i++)
					{
						foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, ".*" + prefixNM + sortedCreateTime[i] + ".*"))
						{
							//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
							sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
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
								flg = GetFileInfo(swfname, lsetInfo.InputFolderNM, ref sfname, ref sFileType, ref sWork);
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
					FileInfo fileInfo = new FileInfo(swfname);

					//<--人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima
					bool flg;
					for (int j = 0; j < 5; j++)
					{
						flg = GetFileInfo(fileInfo.FullName, lsetInfo.InputFolderNM, ref sfname, ref sFileType, ref sWork);
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

					SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

					bool getLotFromNascaFg = false;
					if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString())
					{
						getLotFromNascaFg = true;
					}

					MagInfo magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL, getLotFromNascaFg, FILE_DATECOL, FILE_TIMECOL);

					//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
					switch (prefixNM)
					{
						case "MM":/*3*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:MM File"));
							MMFile mmFile = new MMFile(this);
							mmFile.Run(lsetInfo, magInfo, textArray, ref errMessageList, COL_KEIKOUFG, COL_ADDRESS, COL_CHANGEFG, COL_TRANCD, TimingNO, 0);
							isMMFileNothing = false;
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:MM File"));
							break;
						case "OA":/*2*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:OA File"));
							DbInput_AI_OAFile(lsetInfo, magInfo, textArray, ref errMessageList, COL_OA_KEIKOUFG, COL_OA_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:OA File"));
							break;
						//case "SM":/*1*/
						//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM File"));
						//	DbInput_AI_SMFile(lsetInfo, magInfo, textArray, FileType.SM.ToString(), ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
						//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM File"));
						//	break;
						case "SM1":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM1 File"));
							DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM1", ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM1 File"));

							MpdFileProcess(lsetInfo, magInfo);

							break;
						case "SM2":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM2 File"));
							DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM2", ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM2 File"));
							break;
					}

					//処理済みファイルを保管フォルダへ移動
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, "");

				}
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}
}
