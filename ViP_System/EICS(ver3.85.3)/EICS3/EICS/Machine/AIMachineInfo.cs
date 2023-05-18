using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using EICS.Machine;
using EICS.Database;
using EICS.Database.LENS;
using EICS.Structure;
using System.Threading;
using System.Text.RegularExpressions;

namespace EICS
{
    /// <summary>
    /// 外観検査機処理
    /// </summary>
    public class AIMachineInfo : MachineBase
    {
		const int SEND_MAPPING_CT_AT_ONCE = 1000;
		TcpClient tcp = null;
		NetworkStream ns = null;
		/// <summary>ファイルMM内容列(傾向管理フラグ)</summary>
		public const int COL_KEIKOUFG = 6;
		public const int TimingNO = 6;

		/// <summary>ファイルMM内容列(変化点フラグ)</summary>
		public const int COL_CHANGEFG = 7;

		/// <summary>ファイル内容(アドレスNO)</summary>
		public const int COL_ADDRESS = 4;

		/// <summary>ファイル内容(処理CD)</summary>
		public const int COL_TRANCD = 5;

        protected bool isMMFileNothing;
		protected string sLot = string.Empty;
		protected string sMagazineNO = string.Empty;
		protected bool sendedChangePointFG;

		protected const int FULLSIZE_MAPPING_DATA_COUNT = 160000;

		/// <summary>外観検査装置名</summary>
		public const string ASSETS_NM = "外観検査機";

        /// <summary>ファイル種類</summary>
        public enum FileType
        {
            OA, SM, MM
        }

        #region 定数

        /// <summary>PLC連続書き込み最大数(800)</summary>
        private const int PLC_MEMORY_WRITE_MAX = 800;

        /// <summary>PLC1アドレスビット数</summary>
        private const int PLC_MEMORY_EM_BIT = 16;

        /// <summary>PLCアドレス(WBマッピング)</summary>
        public const int PLC_ADDRESS_MAPPING = 00000;        //00000～09999

        /// <summary>PLCアドレス(装置マッピング)</summary>
        public const int PLC_ADDRESS_MACHINEMAP = 10000;   //10000～フレーム段数分(1フレーム8アドレス)

        /// <summary>装置マッピング1フレームアドレス個数</summary>
        public const int MACHINEMAP_ONEFRAMECT = 8;

        /// <summary>ファイル内容(日付)</summary>
        public const int FILE_DAY = 1;

        /// <summary>ファイル内容(時間)</summary>
        public const int FILE_TIME = 2;

        /// <summary>ファイル内容マガジンNO行</summary>
        public const int FILE_MAGAZINEROW = 2;

        /// <summary>ファイル内容マガジンNO列</summary>
        public const int FILE_MAGAZINECOL = 3;

		/// <summary>ファイル内容DATE列</summary>
		public const int FILE_DATECOL = 1;

		/// <summary>ファイル内容TIME列</summary>
		public const int FILE_TIMECOL = 2;

        /// <summary>ファイル内容開始行</summary>
        public const int FILE_DATASTARTROW = 2;

        /// <summary>ファイルOA内容列(傾向管理フラグ)</summary>
        public const int COL_OA_KEIKOUFG = 7;

        /// <summary>ファイルOA内容列(変化点フラグ)</summary>
		public const int COL_OA_CHANGEFG = 8;
    
        /// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM_KEIKOUFG = 16;

        /// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM_CHANGEFG = 55;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM1_KEIKOUFG = 4;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM1_CHANGEFG = 5;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM2_KEIKOUFG = 4;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM2_CHANGEFG = 5;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int COL_SM3KEIKOUFG = 4;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		public const int COL_SM3_CHANGEFG = 5;

        /// <summary>ファイル内容(傾向管理フラグOFF)</summary>
        public const string KEIKOUFG_OFF = "0";

		public const string OPENINGCHECKFG_ON = "1";

        /// <summary>ファイル内容(変化点フラグOFF)</summary>
        public const string CHANGEFG_OFF = "0";
		public const string CHANGEFG_ON = "1";


        /// <summary>ファイル内容異常値</summary>
        public const string FILE_ERRORSTR = "-999000";

        #endregion

		public AIMachineInfo()
		{
		}

		public AIMachineInfo(LSETInfo lset)
		{
			lsetInfo = lset;

			this.LineCD = lset.InlineCD;
		}

        //public AIMachineInfo(int lineCD)
        //{
        //	this.LineCD = lineCD;
        //}

        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
            if (lsetInfo.EnableResultPriorityJudge_FG)
            {
                RunningLog runLog = RunningLog.GetInstance();
                runLog.logMessageQue.Enqueue(string.Format("{0}:【チェック】装置の検査NG優先度設定 取得テスト 開始", lsetInfo.EquipmentNO));
                GetResultPriority(lsetInfo.IPAddressNO);
                runLog.logMessageQue.Enqueue(string.Format("{0}:【チェック】装置の検査NG優先度設定 取得テスト 終了", lsetInfo.EquipmentNO));
            }
        }

        /// <summary>
        /// ファイルチェック
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="errMessageList"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
			AlertLog alertLog = AlertLog.GetInstance();

			try
			{
				//KLinkInfo.Open(ref tcp, lsetInfo.IPAddressNO, lsetInfo.PortNO);

				CheckDirectory(lsetInfo);

				base.machineStatus = Constant.MachineStatus.Runtime;

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
				SettingInfo settingInfoCommon = SettingInfo.GetSingleton();

				//AI,MD,PLA,ECKはBlackJumbDogの起動確認
				if (settingInfoPerLine.BlackJumboDogFG == "ON")
				{
					if (!KLinkInfo.CheckBlackJumboDog())
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_14, this.LineCD));
					}
				}

				//マッピング有効ラインの場合、LENS2起動確認
				if (settingInfoCommon.LensFG)
				{
					if (!KLinkInfo.CheckLENS())
					{
						throw new ApplicationException(string.Format("LENS2を起動して下さい。", this.LineCD));
					}
				}

				string resMsg = string.Empty;
				KLinkInfo kLinkInfo = new KLinkInfo();

				if (settingInfoCommon.IsMappingMode == false)
				{
					//「受付準備OKをON」
					resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 1, 11, Constant.ssuffix_U);
					if (resMsg == "Error")
					{
						base.machineStatus = Constant.MachineStatus.Stop;
						return;
					}
					System.Threading.Thread.Sleep(100);//0.1秒ready信号をON
				}

				if (settingInfoCommon.IsMappingMode == false && settingInfoPerLine.WBMappingFG == false)
				{
					string sType = GetType(ref tcp, ref ns, lsetInfo);

					if (string.IsNullOrEmpty(sType) == false)
					{
						if (settingInfoPerLine.IsNotSendZeroMapping(lsetInfo.EquipmentNO))
						{
						}
						else
						{
							ForcedZeroMappingProcess(ref tcp, ref ns, lsetInfo, sType);
						}

						SendTypeProcess(ref tcp, ref ns, lsetInfo.IPAddressNO, sType);
					}
				}

				//対象ファイル種類を取得
				//Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, true);
				//foreach (KeyValuePair<string, string> prefix in prefixList)
				//{
				//	FileProcess(lsetInfo, prefix.Key.ToUpper(), prefix.Value, true);
				//}

				//prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, false);
				//foreach (KeyValuePair<string, string> prefix in prefixList)
				//{
				//	FileProcess(lsetInfo, prefix.Key.ToUpper(), prefix.Value, false);
				//}

				Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0);
				foreach (KeyValuePair<string, string> prefix in prefixList)
				{
					FileProcess(lsetInfo, prefix.Key, prefix.Value);
				}
#if Debug
#else
				if (settingInfoCommon.IsMappingMode == false)
				{
					//「受付準備OKをOFF」
					resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
					if (resMsg == "Error")
					{
						base.machineStatus = Constant.MachineStatus.Stop;
						return;
					}
				}

				if (IsMustStopMachine())
				{
					SetMustStopMachineFG(false);
					throw new ApplicationException("装置監視停止します。");
				}
#endif
			}
			finally
			{
				if (ns != null) { ns.Close(); }
				if (tcp != null) { tcp.Close(); }
			}
        }

		protected virtual void FileProcess(LSETInfo lsetInfo, string prefixNm, string prefixAddr)
		{
			SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
			AlertLog alertLog = AlertLog.GetInstance();
			string resMsg = string.Empty;
			KLinkInfo kLinkInfo = new KLinkInfo();
#if Debug
        //ファイルの情報登録・異常あれば通知
        //FileDistribution(ref tcp, ref ns, lsetInfo, prefix.Key, ref base.errorMessageList);
#else
			//ファイル取得要求
			resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return;
			}
			if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
			{
				//if (prefixNm == MMFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
				if(prefixNm != "OA")
				{
					FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
					if (file == null)
					{
						return;
					}

					if (!MachineFile.IsThereLotNoInFileName(file.FullName))
					{
						//MMファイル名にロット情報が付与されていない間は待機
						return;
					}
                    if (prefixNm == MMFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
					{
						if (MachineFile.IsUnknownFile(file) == false)
						{
							MMFile mmFile = new MMFile(file);

							sLot = mmFile.LotNo;

							if (Database.LENS.WorkResult.IsComplete(mmFile.LotNo, mmFile.ProcNo, this.LineCD) == false)
							{
								//LENSの処理が完了していない間は待機
								return;
							}
						}
					}
				}

				//「受付準備OKをOFF」
				resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
				if (resMsg == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return;
				}

				//ファイルの情報登録・異常あれば通知
				FileDistribution(ref tcp, ref ns, prefixAddr, lsetInfo, prefixNm, ref base.errorMessageList);

				//ヘッダだけのデータ空ファイルが出るが削除されるなどして、MMファイルが無い場合
				//検査位置が決定していて、検査結果が出てきてないので照合NG
				if (prefixNm == "MM")
				{
					if (isMMFileNothing)
					{
						if (sLot != "UNKNOWN")
						{
							if (Lot.IsExecuteInspectionResultJudgment(sLot, lsetInfo.InlineCD) == false)
							{
								SendNoConformanceSignal(lsetInfo);

								alertLog.logMessageQue.Enqueue(Constant.MessageInfo.Message_143);
								SetMustStopMachineFG(true);
							}
						}

						//Nasca不良ファイル作成
						MagInfo magInfo = new MagInfo();
						magInfo.sMagazineNO = "UNKNOWN";
						magInfo.sNascaLotNO = sLot;

						//NascaDefectFile.csのCreate()へ統合
						//CreateNasMappingFile(magInfo, lsetInfo.EquipmentNO, string.Empty);
						NascaDefectFile.Create(sLot, sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, string.Empty);

					}
				}

				//ファイル取得要求 立ち下げ
				resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, 0, 1, Constant.ssuffix_U);
				if (resMsg == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return;
				}
			}
#endif
		}

		protected string GetType(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();
			//TYPE教えて信号 確認
			string resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Type, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return string.Empty;
			}
			if (Convert.ToInt16(resMsg) == 1)//ONの場合
			{
				//Lot
				//"0000 0000 0000 0000 0000 0000 0000 0000 0000 0000\r\n"が入っていた
				string strQr = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TEACH_Res_AI_Lot, 10, Constant.ssuffix_H);
				if (strQr == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return string.Empty;
				}
				strQr = kLinkInfo.BytesStringToAscii(strQr);
				strQr = strQr.Trim('\0', '\n', '\r', ' ');

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "AI/QR: " + strQr);

				string sType = ""; string sLot = "";

				string[] strQrArray = strQr.Split(' ');
				if (strQrArray[0] == Constant.DISCRIMINATION_LOT)
				{
					sLot = strQrArray[1];
					sType = ConnectDB.GetARMSLotType(this.LineCD, sLot);
				}
				else if (strQrArray[0] == Constant.DISCRIMINATION_MAGAZINE)
				{
                    Arms.Magazine svrMag = Arms.Magazine.GetCurrent(this.LineCD, strQrArray[1]);
					sType = ConnectDB.GetARMSLotType(this.LineCD, svrMag.NascaLotNO);
				}
				lsetInfo.TypeCD = sType;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "AI/Type: " + sType);

				return sType;
			}
			return string.Empty;
		}

		protected void ForcedZeroMappingProcess(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string sType)
        {
            SendChangepointFlag(ref tcp, ref ns, lsetInfo.IPAddressNO, false);

			Database.LENS.Magazine magConfig = EICS.Database.LENS.Magazine.GetData(sType, this.LineCD);

            List<string> mappingList = MappingFile.GetFullSizeMappingData(magConfig.TotalMagazinePackage, "1");

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("【AI:全0マッピングデータ送信】ロットNO：{0}", sLot ));
            //PLCにマッピングデータを送信
            SendMappingData(ref tcp, ref ns, lsetInfo.IPAddressNO, mappingList);
        }

		protected void SendTypeProcess(ref TcpClient tcp, ref NetworkStream ns, string ipAddr, string sType)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();

			//製品型番をPLCに送信
			SendTypeData(ref tcp, ref ns, ipAddr, sType);

			//トリガ立ち下げ
			string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, ipAddr, Constant.MACHINE_PORT, Constant.TRG_Send_Type, 0, 1, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return;
			}
		}

		protected override void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string sTargetFile, ref List<ErrMessageInfo> errMessageList)
        {
			try
			{
				string sfname = "";
				string sWork = "";
				string sFileType = "";
				string[] textArray = new string[] { };
				string spath1 = "", spath2 = "";
				string sMessage = "";

				List<string> fileList = GetMachineLogPathList(lsetInfo, ref tcp, ref ns, fileGetReqFgAddr, lsetInfo.InputFolderNM, sTargetFile + ".*");


				int nSameTargetFileNum = fileList.Count;
				string[] sortedCreateTime = new string[nSameTargetFileNum];
				int i = 0;
				isMMFileNothing = true;

				//ターゲットファイルが複数あった場合
				if (nSameTargetFileNum > 1)
				{
					i = 0;
					sMessage = lsetInfo.AssetsNM + "/TargetFile=" + sTargetFile + "/[" + nSameTargetFileNum + "]ありました。";
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
						foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, sTargetFile + sortedCreateTime[i] + ".*"))
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
							for (int j = 0; j < 200; j++)
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
				List<string> machineFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + sTargetFile);
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

					MagInfo magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL);

					FileProcessSelector(sTargetFile, lsetInfo, magInfo, textArray, fileInfo.FullName, ref errMessageList);

					//処理済みファイルを保管フォルダへ移動
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, "");

				}
			}
			catch (Exception err)
			{
				throw;
			}
        }

		protected virtual void FileProcessSelector(string sFileType, LSETInfo lsetInfo, MagInfo magInfo, string[] textArray, string fileFullPath, ref List<ErrMessageInfo> errMessageList)
		{
			//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
			switch (sFileType)
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
				case "SM":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM File"));
					DbInput_AI_SMFile(lsetInfo, magInfo, textArray, FileType.SM.ToString(), ref errMessageList, COL_SM_KEIKOUFG, COL_SM_CHANGEFG);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM File"));
					break;
				case "SM1":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM1 File"));
					DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM1", ref errMessageList, COL_SM1_KEIKOUFG, COL_SM1_CHANGEFG);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM1 File"));
					break;
				case "SM2":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM2 File"));
					DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM2", ref errMessageList, COL_SM2_KEIKOUFG, COL_SM2_CHANGEFG);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM2 File"));
					break;
				case "SM3":/*1*/
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[START]AI:SM3 File"));
					DbInput_AI_SMFile(lsetInfo, magInfo, textArray, "SM3", ref errMessageList, COL_SM2_KEIKOUFG, COL_SM2_CHANGEFG);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}\t/{2}", lsetInfo.EquipmentNO, magInfo.sNascaLotNO, "[END]AI:SM3 File"));
					break;
			}
		}

        #region 各ファイル処理

        //[OA:装置実行ﾊﾟﾗﾒｰﾀ]
		protected void DbInput_AI_OAFile(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList, int keikouFgColIndex, int changeFgColIndex)
        {
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			string qrValue = string.Empty;

			//マガジン情報取得

#if Debug
			//magInfo.sMaterialCD = lsetInfo.TypeCD;
#endif

            //OAファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.OA), lsetInfo, magInfo.sMaterialCD);

			List<FileFmt> noChkFileFmt = new List<FileFmt>();

			if (filefmtList.Count == 0)
			{
				noChkFileFmt = FileFmt.GetData(lsetInfo, null, Convert.ToString(FileType.OA), Constant.NO_CHK_QCPARAM_NO);
			}

			if (noChkFileFmt.Count >= 1)
			{
				return;
			}

            if (filefmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.OA));
                throw new Exception(message);
            }

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
				if (filefmtInfo.QCParamNO == Constant.NO_CHK_QCPARAM_NO)
				{
					continue;
				}

                //閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, this.LineCD);
				Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false, lsetInfo.ChipNM);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                FileValueInfo fileValueInfo = new FileValueInfo();
                for (int i = FILE_DATASTARTROW; i < fileLineValueList.Length; i++)
                {
                    if (fileLineValueList[i] == "")
                    {
                        //空白行の場合、次へ
                        continue;
                    }

                    string[] fileValue = fileLineValueList[i].Split(',');
					if (fileValue[keikouFgColIndex].Trim() == KEIKOUFG_OFF)
                    {
                        //傾向管理しないデータの場合、次へ
                        continue;
                    }
                    if (settingInfo.WBMappingFG)
                    {
						if (fileValue[changeFgColIndex].Trim() == CHANGEFG_OFF)
                        {
                            //変化点フラグOFFの場合、次へ
                            continue;
                        }
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_DAY] + " " + fileValue[FILE_TIME]));
					fileValueInfo.TargetVAL = CalcChangeUnit(filefmtInfo.QCParamNO, Convert.ToDouble(fileValue[filefmtInfo.ColumnNO]));
                    fileValueInfo.SubmitFG = true;
                }

                if (!fileValueInfo.SubmitFG) 
                {
                    continue;
                }

                //異常判定+DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(fileValueInfo.TargetVAL), fileValueInfo.MeasureDT, ref errMessageList);
            }
        }
        
        //[SM:製品出来栄え]
        protected void DbInput_AI_SMFile(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, string prefix, ref List<ErrMessageInfo> errMessageList, int keikouFgColIndex, int changeFgColIndex)
        {
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			string strMeasureDt = string.Empty;

#if Debug
			//magInfo.sMaterialCD = lsetInfo.TypeCD;
#endif

            //SMファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(prefix), lsetInfo, magInfo.sMaterialCD);

			List<FileFmt> noChkFileFmt = new List<FileFmt>();

			if (filefmtList.Count == 0)
			{
				noChkFileFmt = FileFmt.GetData(lsetInfo, null, prefix, Constant.NO_CHK_QCPARAM_NO);
			}

			if (noChkFileFmt.Count >= 1)
			{
				return;
			}
			
            if (filefmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(prefix));
                throw new Exception(message);
            }

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
				if (filefmtInfo.QCParamNO == Constant.NO_CHK_QCPARAM_NO)
				{
					continue;
				}

                //閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, this.LineCD);
				Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false, lsetInfo.ChipNM);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

				List<string> dataList = MachineFile.GetDataListFromStrArray(fileLineValueList, FILE_DATASTARTROW, keikouFgColIndex, KEIKOUFG_OFF
					, settingInfo.WBMappingFG, changeFgColIndex, CHANGEFG_OFF, filefmtInfo.ColumnNO, FILE_ERRORSTR, FILE_DAY, FILE_TIME, out strMeasureDt);

				List<string> refDataList = new List<string>();
				string[] refSampleData = null;
				#region 下記コメント群は関数化【MachineFile.GetDataListFromStrArray()】
				//FileValueInfo fileValueInfo = new FileValueInfo();
				//List<double> valueList = new List<double>();
				//List<string> dataList = new List<string>();

				//for (int i = FILE_DATASTARTROW; i < fileLineValueList.Length; i++)
				//{
				//	if (fileLineValueList[i] == "")
				//	{
				//		//空白行の場合、次へ
				//		continue;
				//	}

				//	string[] fileValue = fileLineValueList[i].Split(',');
				//	if (fileValue[keikouFgColIndex].Trim() == KEIKOUFG_OFF)
				//	{
				//		//傾向管理しないデータの場合、次へ
				//		continue;
				//	}
				//	if (settingInfo.WBMappingFG)
				//	{
				//		if (changeFgColIndex >= 0)
				//		{
				//			if (fileValue[changeFgColIndex].Trim() == CHANGEFG_OFF)
				//			{
				//				//変化点フラグOFFの場合、次へ
				//				continue;
				//			}
				//		}
				//	}
				//	if (Convert.ToDouble(fileValue[filefmtInfo.ColumnNO]) == FILE_ERRORVAL) 
				//	{
				//		continue;
				//	}

				//	double measureVAL = 0;
				//	string txtVal = fileValue[filefmtInfo.ColumnNO].Trim();
                    
				//	if (!double.TryParse(txtVal, out measureVAL))
				//	{
				//		//測定値が数値以外の場合、次へ
				//		continue;
				//	}

				//	fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_DAY] + " " + fileValue[FILE_TIME]));
				//	//valueList.Add(measureVAL);
				//	dataList.Add(txtVal);
				//}
				#endregion

				if (plmInfo.RefQcParamNO.HasValue && plmInfo.TotalKB.Trim().ToUpper() == Constant.CalcType.SIGMA2USIGMA.ToString())
				{
					List<FileFmt> refFileFmtList = FileFmt.GetData(lsetInfo, null, prefix, plmInfo.RefQcParamNO);
					
					if (refFileFmtList.Count != 1)
					{
						throw new ApplicationException(string.Format(
							"参照ﾊﾟﾗﾒﾀ番号から取得したTmFileFmtﾚｺｰﾄﾞが1件に限定出来ませんでした。ﾏｽﾀ担当者へ連絡して下さい。ModelNm:{0} Prefix:{1} QcParamNo:{2} RefQcParamNo:{3}"
							, lsetInfo.ModelNM, prefix, plmInfo.QcParamNO, plmInfo.RefQcParamNO));
					}

					FileFmt refFileFmt = refFileFmtList.Single();
					string dummyStr = string.Empty;

					refDataList = MachineFile.GetDataListFromStrArray(fileLineValueList, FILE_DATASTARTROW, keikouFgColIndex, KEIKOUFG_OFF
					, settingInfo.WBMappingFG, changeFgColIndex, CHANGEFG_OFF, refFileFmt.ColumnNO.Value, FILE_ERRORSTR, FILE_DAY, FILE_TIME, out dummyStr);

					refSampleData = PrmSummary.ConvertToSummaryData(refDataList.ToArray(), plmInfo.RefQcParamNO.Value, plmInfo.MaterialCD, lsetInfo.InlineCD);
				}

				if (dataList.Count == 0) 
                {
                    continue;
                }

                double rValue = 0;

				//データサンプリング(指定位置・数量のデータに限定)
				string[] sampledData = PrmSummary.ConvertToSummaryData(dataList.ToArray(), plmInfo.QcParamNO, plmInfo.MaterialCD, lsetInfo.InlineCD);

				//データ集計・計算処理
				string calculatedValStr;
				try
				{
					calculatedValStr = MachineFile.GetCalculatedResult(plmInfo, sampledData, refSampleData, lsetInfo.InlineCD);
				}
				catch (Exception err)
				{
					throw new Exception(string.Format("{0} ﾊﾟﾗﾒｰﾀ名(ﾌｧｲﾙ):{1}", err.Message, plmInfo.ParameterNM), err);
				}

				if (!double.TryParse(calculatedValStr, out rValue))
				{
					throw new ApplicationException(string.Format("SMﾌｧｲﾙ・集計結果が数値変換出来ませんでした。 変換対象:{0} ﾊﾟﾗﾒﾀ番号:{1} ﾊﾟﾗﾒﾀ名:{2}", calculatedValStr, plmInfo.QcParamNO, plmInfo.ParameterNM));
				}

				//resultValue = CalcChangeUnit(filefmtInfo.QCParamNO, rValue);

                //異常判定+DB登録
				ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(rValue), strMeasureDt, ref errMessageList);
            }
        }
        #endregion

		protected void MpdFileProcess(LSETInfo lsetInfo, MagInfo magInfo)
		{
			string typeCd = LENS2_Api.Lot.GetData(magInfo.sNascaLotNO).TypeCd;
			
			LENS2_Api.Magazine.Config magConfig = LENS2_Api.Magazine.Config.GetData(typeCd);

			//ロットに紐つく全MapResultをリスト化して取得(LENS2_Apiに機能作成）
			LENS2_Api.MapResult mapResult = new LENS2_Api.MapResult(magInfo.sNascaLotNO, true, lsetInfo.EquipmentNO, true);

			//ロットに紐つくMapResult毎に不良を集計して（LENS2_Apiに機能作成）
			//不良毎の数をnasファイル化して出力
			LENS2_Api.EICS.Defect[] lotDefect = mapResult.GetTotalDefect();

			List<NascaDefectFile.DefectQtyInfo> defectList = NascaDefectFile.ConvertLENSDefectData(lotDefect);

			NascaDefectFile.Create(magInfo.sNascaLotNO, magInfo.sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, defectList);
		}

        #region Nasca不良

        /// <summary>
        /// Nasca不良データに含めるか確認する
        /// </summary>
        /// <param name="addressNO">アドレスNO</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <returns>ステータス</returns>
        public bool IsNascaDefect(int addressNO, MagInfo magInfo) 
        {
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString())
			{
				return true;
			}

#if SHASAI_UNITTEST
//            return true;
#endif
            
            //ARMSロット情報(マッピング検査)を取得
			//ARMSLotInfo lotInfo = ConnectDB.GetARMSLotInfo(this.LineCD, magInfo.sNascaLotNO);
			//if (!lotInfo.MappingInspFG)
			//{
			//    return true;
			//}

            //WBマッピングデータを取得
            List<MappingFile> mappingDataList
                = MappingFile.GetData(settingInfoPerLine.DirAIMapping, magInfo.sNascaLotNO, "wbm");

            //不良に含めない処理CDを取得
            List<Database.General> targetTranList = Database.General.GetGeneralData((int)Database.General.GeneralGrp.LittleResinTranCD, this.LineCD);

            //不良に含めないマッピング情報を取得
            List<MappingFile> targetWireMappingList = new List<MappingFile>();
            foreach (Database.General targetTran in targetTranList)
            {
                targetWireMappingList.AddRange(mappingDataList.FindAll(m => m.MappingCD == targetTran.GeneralCD));
            }

            if (targetWireMappingList.Exists(t => t.AddressNO == addressNO))
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        #endregion

        #region PLC

        /// <summary>
        /// WBマッピングデータをPLCに送信
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="ns"></param>
        /// <param name="ipAddressNO"></param>
        /// <param name="typeCD"></param>
        protected static void SendMappingDataOld(ref TcpClient tcp, ref NetworkStream ns, string ipAddressNO, List<string> mappingList) 
        {
            KLinkInfo kLinkInfo = new KLinkInfo();

            try
            {
                //1アドレス16Bitのマッピングリストを生成
                List<string> mappingPartList = GetMappingPartData(mappingList, PLC_MEMORY_EM_BIT);

				List<string> mappingListForLog = new List<string>();

                int writeAddressNO = PLC_ADDRESS_MAPPING;
                foreach (string mappingPart in mappingPartList)
                {
                    if (mappingPart == "") { continue; }

                    string[] bitVAL = new string[1000];
                    kLinkInfo.ExchangeZFMemory(mappingPart, ref bitVAL);

					mappingListForLog.Add(bitVAL[0]);

                    //マッピングデータ書込
                    string resMsg = kLinkInfo.KLINK_SetKV_WRSS(ref tcp, ref ns, ipAddressNO, Constant.MACHINE_PORT, "EM" + writeAddressNO.ToString("00000"), bitVAL[0], "");
                    if (resMsg == "Error")
                    {
                        throw new Exception(Constant.MessageInfo.Message_59);
                    }

                    writeAddressNO += 1;
                }

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("【AI送信マッピングデータ(16bit/addr)】{0}", string.Join("", mappingListForLog.ToArray())));
            }
            catch (Exception err)
            {
                throw;
            }
        }

		/// <summary>
		/// WBマッピングデータをPLCに送信
		/// </summary>
		/// <param name="tcp"></param>
		/// <param name="ns"></param>
		/// <param name="ipAddressNO"></param>
		/// <param name="typeCD"></param>
		public static void SendMappingData(ref TcpClient tcp, ref NetworkStream ns, string ipAddressNO, List<string> mappingList)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();

			//1アドレス16Bitのマッピングリストを生成
			List<string> mappingPartList = GetMappingPartData(mappingList, PLC_MEMORY_EM_BIT);

			List<string> mappingListForLog = new List<string>();

			//foreach (string mappingPart in mappingPartList)
			int loopCt = (int)Math.Ceiling(mappingPartList.Count / 1000M);

			//3.5
			//0: 0, 1000, 2500
			//1: 1000, 1000, 1500
			//2: 2000, 1000, 500
			//3: 3000,

			int remainingAmount = mappingPartList.Count;

			for (int sendCt = 0; sendCt < loopCt; sendCt++)
			{
				int takeCt = SEND_MAPPING_CT_AT_ONCE;

				if (remainingAmount < SEND_MAPPING_CT_AT_ONCE)
				{
					takeCt = remainingAmount;
				}
				else
				{
					remainingAmount = remainingAmount - SEND_MAPPING_CT_AT_ONCE;
				}

				List<string> sendMappingAtOnce = mappingPartList.Skip(sendCt * 1000).Take(takeCt).ToList();

				string bit16DataSequenceStr = string.Empty;

				foreach (string mappingPart in sendMappingAtOnce)
				{
					int intval = Convert.ToInt32(mappingPart, 2);
					//kLinkInfo.ExchangeZFMemory(mappingPart, ref bit16DataSequenceStr);
					bit16DataSequenceStr = string.Join(" ", bit16DataSequenceStr, intval);

					mappingListForLog.Add(Convert.ToString(intval, 16));
				}

				bit16DataSequenceStr = bit16DataSequenceStr.TrimStart();

				//16pkg × 1000ワード分のマッピングデータ書込
				string resMsg = kLinkInfo.KLINK_SetKV_WRSS(ref tcp, ref ns, ipAddressNO, Constant.MACHINE_PORT, "EM" + (sendCt * SEND_MAPPING_CT_AT_ONCE).ToString("00000"), bit16DataSequenceStr, "");
				if (resMsg == "Error")
				{
					throw new ApplicationException(Constant.MessageInfo.Message_59);
				}
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("【AI送信マッピングデータ(16bit/addr)】{0}", string.Join(" ", mappingListForLog.ToArray())));
		}

		/// <summary>
		/// 製品型番をPLCに送信
		/// </summary>
		/// <param name="tcp"></param>
		/// <param name="ns"></param>
		/// <param name="ipAddressNO"></param>
		/// <param name="typeCD"></param>
		protected static void SendTypeData(ref TcpClient tcp, ref NetworkStream ns, string ipAddressNO, string typeCD) 
        {
            KLinkInfo kLinkInfo = new KLinkInfo();
            string sendMsg = kLinkInfo.GetSendMsg_WRSS(typeCD);

            try
            {
                string resMsg = kLinkInfo.KLINK_SetKV_WRSS(ref tcp, ref ns, ipAddressNO, Constant.MACHINE_PORT, Constant.TEACH_Send_AI_Type, sendMsg, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_60, typeCD));
                }
            }
            catch(Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 変化点フラグをPLCに送信
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="ns"></param>
        /// <param name="ipAddressNO"></param>
        /// <param name="changePointFG"></param>
        protected static void SendChangepointFlag(ref TcpClient tcp, ref NetworkStream ns, string ipAddressNO, bool changePointFG) 
        {
            KLinkInfo kLinkInfo = new KLinkInfo();

            try
            {
                string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, ipAddressNO, Constant.MACHINE_PORT, Constant.TEACH_Send_AI_Change, Convert.ToInt16(changePointFG), 1, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_67, changePointFG));
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

		protected void SendNoConformanceSignal(LSETInfo lsetInfo)
		{
			PLC_Keyence plc = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);
			plc.SetBit(Constant.TRG_Send_NoMapping, true);
		}


        #endregion


        /// <summary>
        /// マッピングデータを指定した1要素分割数で割ったリストで取得
        /// </summary>
        /// <param name="mappingData">マッピングデータ</param>
        /// <param name="partCount">分割数</param>
        /// <returns>マッピングリスト(分割)</returns>
        private static List<string> GetMappingPartData(List<string> mappingData, int partCount)
        {
            List<string> mappingList = new List<string>();
            try
            {
                for (int i = 0; i < Math.Ceiling((decimal)(mappingData.Count / (decimal)partCount)); i++)
                {
                    string[] mappingPartList = mappingData.Skip(i * partCount).Take(partCount).ToArray();
                    mappingList.Add(string.Join("", mappingPartList));
                }

                return mappingList;
            }
            catch (Exception err) 
            {
                throw;
            }
        }

		public void OpeningFileProcess(Constant.OpeningCheckFileType fileType, string filePath)
		{
			if (fileType == Constant.OpeningCheckFileType.OpeningCheckFile)
			{
				MoveOpeningCheckFile(filePath);
			}
			else if (fileType == Constant.OpeningCheckFileType.NotOpeningCheckFile)
			{
				//オープニングチェックファイルで無い為、何も処理はしない
				return;
			}
			else if (fileType == Constant.OpeningCheckFileType.ErrorFile)
			{
				//始業点検データと通常データが混在している為、エラー出力
				throw new Exception(string.Format(Constant.MessageInfo.Message_126, filePath));
			}
			else if (fileType == Constant.OpeningCheckFileType.UnDefined)
			{
				//オープニングチェックファイルかどうかの確認処理が行われていない為、オープニングチェックファイルか不明
				throw new Exception(string.Format(Constant.MessageInfo.Message_127, filePath));
			}
		}

		public void MoveOpeningCheckFile(string filePath)
		{
			//移動元ファイルが無い場合
			if (!File.Exists(filePath))
			{
				return;
			}

			FileInfo fileInfo = new FileInfo(filePath);

			string openingCheckFolderNM = "OpeningCheck";
			string moveFolderPath = "";

			moveFolderPath = Path.Combine(fileInfo.Directory.FullName, openingCheckFolderNM);

			string moveFilePath = Path.Combine(moveFolderPath, fileInfo.Name);

			//保管フォルダへファイル移動
			//保管フォルダが無ければ作成
			if (Directory.Exists(moveFolderPath) == false)
			{
				Directory.CreateDirectory(moveFolderPath);
			}

			//移動先にファイルが無い場合
			if (File.Exists(moveFilePath) == false)
			{
				try
				{
					File.Move(fileInfo.FullName, moveFilePath);
				}
				catch
				{
					//読み取り専用になる為、2秒後もう一度試す。
					Thread.Sleep(2000);
					File.Move(fileInfo.FullName, moveFilePath);
				}
			}
			else//既にある場合、delete
			{
				File.Delete(fileInfo.FullName);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_128, fileInfo.FullName));
			}
		}

		/// <summary>
		/// Keyence限定処理（三菱PLCの場合は16進表記のアドレスに対応する必要あり）
		/// </summary>
		/// <param name="ipAddr"></param>
		/// <returns></returns>
		public Dictionary<int, int> GetResultPriority(string ipAddr)
		{
			List<string> errList = new List<string>();

			Dictionary<int, int> ngPriorityDict = new Dictionary<int, int>();

			SettingInfo commonSetting = SettingInfo.GetSingleton();

			Regex reg = new Regex(@"[^0-9]");
			
			string addrNumPartStr = reg.Replace(commonSetting.ResultPriorityJudgeInst.StartPlcAddr, "");

			int addrNumPart;
			
			//起動時の設定読み込み時に先行で数値変換可能か確認しているので、ここでは行わない。
			addrNumPart = int.Parse(addrNumPartStr);

			string addrKindPart = commonSetting.ResultPriorityJudgeInst.StartPlcAddr.Replace(addrNumPartStr, "");

			int startNgNo = commonSetting.ResultPriorityJudgeInst.StartNgNo;

			for (int i = 0; i < commonSetting.ResultPriorityJudgeInst.Length; i++)
			{
				int currentNgNo = startNgNo + i;
				int currentAddrNumPart = addrNumPart + i;

				string currentPlcMemAddr = string.Format("{0}{1}", addrKindPart, currentAddrNumPart);

				KLinkInfo kLinkInfo = new KLinkInfo();
				string priorityStr = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, ipAddr, Constant.MACHINE_PORT, currentPlcMemAddr, Constant.ssuffix_U);
                if (priorityStr == "Error")
                {
                    throw new ApplicationException($"検査機PLCからNG優先度情報を取得できませんでした。IPアドレス：{ipAddr}, ポート{Constant.MACHINE_PORT}, PLCアドレス：{currentPlcMemAddr}");
                }

                int priority;

				if (int.TryParse(priorityStr, out priority) == false)
				{
					errList.Add(string.Format("NG優先度として数値変換できない文字がPlcﾒﾓﾘｱﾄﾞﾚｽ:{0}から取得されました。取得値:{1}", currentPlcMemAddr, priorityStr));
                }
				else
				{
					ngPriorityDict.Add(currentNgNo, priority);
				}
			}

			if (errList.Count > 0)
			{
				throw new ApplicationException(string.Join("\r\n", errList));
			}

			return ngPriorityDict;
		}        
    }

    /// <summary>
    /// MMファイル
    /// </summary>
    public class MMFile : AIMachineInfo
    {
		public const string FILE_PREFIX = "MM";
		private const string CAMERA_NO_HEADER_NAME = "高倍/低倍検査中";

        /// <summary>測定日時</summary>
        public DateTime? MeasureDT { get; set; }

        /// <summary>アドレスNO</summary>
        public int AddressNO { get; set; }

        /// <summary>処理CD</summary>
        public string TranCD { get; set; }

		/// <summary>マガジン情報</summary>
		public MagInfo magInfo { get; set; }

        /// <summary>傾向管理フラグ</summary>
        public bool TrendManegeFG { get; set; }

		public int CameraNo { get; set; }

        /// <summary>変化点フラグ</summary>
        public bool ChangePointFG { get; set; }

        /// <summary>ファイル内容(日付)</summary>
        public const int COL_DAY = 1;

        /// <summary>ファイル内容(時間)</summary>
        public const int COL_TIME = 2;

		public const int CONTENTS_HEADER_ROW = 1;

        /// <summary>ファイル内容開始行</summary>
        public const int ROW_DATASTART = 2;
		public Constant.OpeningCheckFileType OpeningCheckFileType { get; set; }

        protected AIMachineInfo aiMachineInfo;

		public FileInfo Info { get; set; }
        public string TypeCd { get; set; }
        public string LotNo { get; set; }
        public int ProcNo { get; set; }

        public MMFile(FileInfo mmFile)
        {
            this.Info = mmFile;

            getLotFromFileName();
			OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;

        }

        public MMFile(AIMachineInfo instance)
        {
             aiMachineInfo = instance;
			 OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;

        }

		private void getLotFromFileName()
        {
			if (MachineFile.IsThereLotNoInFileName(this.Info.FullName) == false)
            {
                throw new ApplicationException(
                    string.Format("MMファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
            }

            string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
            this.LotNo = nameChar[2].Trim();
            this.TypeCd = nameChar[3].Trim();
            this.ProcNo = int.Parse(nameChar[4].Trim());
        }

        /// <summary>
        /// ログファイル処理
        /// </summary>
        /// <param name="lsetInfo">設備情報</param>
        /// <param name="fileLineValueList">ファイル内容</param>
        /// <param name="errMessageList">エラー情報</param>
		public void Run(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList, int keikouFgColIndex, int pkgAddrColIndex, int changeFgColIndex, int tranCdColIndex, int timingNo, int openingChkColIndex)
        {
			AlertLog alertLog = AlertLog.GetInstance();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			string qrValue = MachineBase.GetQRStrFromFile(fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL);
            //マガジン情報取得
#if Debug
			//magInfo = aiMachineInfo.GetMagazineInfo(lsetInfo, fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL, FILE_DATECOL, FILE_TIMECOL, ref qrValue);
#else
            
#endif
			Dictionary<int, int> ngPriorityDict = new Dictionary<int, int>();
			if (lsetInfo.EnableResultPriorityJudge_FG)
			{
				//装置からの設定取得
				ngPriorityDict = GetResultPriority(lsetInfo.IPAddressNO);
			}

            if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_85, magInfo.sMagazineNO));
            }

			bool enableOpeningChk = settingInfoPerLine.IsEnableOpeningChk(lsetInfo.EquipmentNO);

            //MMファイル情報取得
			List<MMFile> allDataList = GetData(lsetInfo.InlineCD, enableOpeningChk, fileLineValueList.ToList(), qrValue, keikouFgColIndex, pkgAddrColIndex, changeFgColIndex, tranCdColIndex, openingChkColIndex, ngPriorityDict);

			if (enableOpeningChk)
			{
				if (OpeningCheckFileType != Constant.OpeningCheckFileType.NotOpeningCheckFile)
				{
					return;
				}
			}

			if (allDataList.Count == 0)
			{
				throw new ApplicationException(string.Format("MMファイル内からデータ行を取得できませんでした。設備：{0}", lsetInfo.EquipmentCD));
			}
			else if (allDataList.Last().MeasureDT.HasValue == false)
			{
				throw new ApplicationException(string.Format("MMファイル内から測定日時を取得できませんでした。設備：{0}", lsetInfo.EquipmentCD));
			}

			DateTime lastMeasureDT = allDataList.Last().MeasureDT.Value;

			List<MMFile> dataList = GetNgInspectionData(allDataList);

            //閾値判定+NASCA不良判定
            LimitCheck(lsetInfo, magInfo, dataList, lastMeasureDT, ref errMessageList, timingNo);
        }

        /// <summary>
        /// ファイル情報を取得
        /// </summary>
        /// <param name="fileValue">ファイル内容</param>
        /// <returns>ファイル情報</returns>
        public List<MMFile> GetData(int lineCD, bool enableOpeningChk, List<string> fileValue, string qrValue, int keikouFgColIndex, int pkgAddrColIndex, int changeFgColIndex, int tranCdColIndex, int openingChkColIndex, Dictionary<int, int> ngPriorityDict)
        {
			int camNoColIndex = getCameraNoHeaderColIndex(fileValue);

			List<string> localErrMsgList = new List<string>();

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lineCD);
			int openingCheckRecordCt = 0;
			int recordCt = 0;

            List<MMFile> mmDataList = new List<MMFile>();
            for (int i = ROW_DATASTART; i < fileValue.Count; i++)
            {
                if (fileValue[i] == "")
                {
                    //空白行の場合、次へ
                    continue;
                }

				//空白行以外の行数カウント
				recordCt++;

				string[] fileLineValue = fileValue[i].Split(',');

				if (enableOpeningChk)
				{
					if (fileLineValue[openingChkColIndex].Trim() == AIMachineInfo.OPENINGCHECKFG_ON)
					{
						//オープニングチェックフラグOnの行数カウント
						openingCheckRecordCt++;
						continue;
					}
				}

				if (fileLineValue[FILE_MAGAZINECOL].Trim().Equals(qrValue) == false)
				{
					//指定したマガジンQR値と等しくない場合、次へ
					continue;
				}

				if (keikouFgColIndex >= 0)
				{
					if (fileLineValue[keikouFgColIndex].Trim() == AIMachineInfo.KEIKOUFG_OFF)
					{
						//傾向管理しないデータの場合、次へ
						continue;
					}
				}

				MMFile mmData = new MMFile(aiMachineInfo);

				mmData.MeasureDT = Convert.ToDateTime(fileLineValue[COL_DAY] + " " + fileLineValue[COL_TIME]);
				mmData.AddressNO = Convert.ToInt32(fileLineValue[pkgAddrColIndex].Trim());

				if (mmData.AddressNO == 0)
				{
					continue;
				}

				mmData.TranCD = fileLineValue[tranCdColIndex].Trim();
				mmData.ChangePointFG = false;

				if (changeFgColIndex >= 0)
				{
					if (fileLineValue[changeFgColIndex].Trim() == AIMachineInfo.CHANGEFG_ON)//settingInfo.WBMappingFG==trueもand条件で存在したが、四宮氏と相談し不要との事で削除 (2014/1/31 n.yoshimoto)
					{
						//変化点フラグONの場合
						mmData.ChangePointFG = true;
					}
				}
				else 
				{
					mmData.ChangePointFG = true;
				}

				int cameraNo = 0;
				if (camNoColIndex > -1)
				{
					string camNoStr = fileLineValue[camNoColIndex];

					if (int.TryParse(camNoStr, out cameraNo) == false)
					{
						string errMsg = string.Format("【高倍/低倍検査中】列の値を数値変換出来ません。変換対象:{0}", camNoStr);
						throw new ApplicationException(errMsg);
					}
				}

				mmData.CameraNo = cameraNo;

				int targetIndex = mmDataList.FindIndex(m => m.AddressNO == mmData.AddressNO && m.CameraNo == mmData.CameraNo);
				if (targetIndex == -1)
				{
                    mmDataList.Add(mmData);
				}
				else
				{
					//NG優先度設定が装置から取得出来ていれば、同一アドレスで発生したNGは優先度の高い方を選択する
					if (ngPriorityDict.Count > 0)
					{
						try
						{
							if (IsHighPriorityThanOriginal(mmDataList[targetIndex].TranCD, mmData.TranCD, ngPriorityDict) == false)
							{
								continue;
							}
						}
						catch (ApplicationException err)
						{
							localErrMsgList.Add(err.Message);
							continue;
						}
					}

					//同一アドレスデータがある場合、最後尾を優先する
					mmDataList[targetIndex] = mmData;
				}
            }

			if (enableOpeningChk)
			{
				if (openingCheckRecordCt == recordCt && openingCheckRecordCt > 0)
				{
					this.OpeningCheckFileType = Constant.OpeningCheckFileType.OpeningCheckFile;
				}
				else if (openingCheckRecordCt != recordCt && openingCheckRecordCt > 0)
				{
					this.OpeningCheckFileType = Constant.OpeningCheckFileType.ErrorFile;
				}
				else if (openingCheckRecordCt == 0)
				{
					this.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;
				}
			}

			if (localErrMsgList.Count > 0)
			{
				throw new ApplicationException(string.Join("\r\n", localErrMsgList));
			}

			///////
			//NG優先度設定が装置から取得出来ていれば、同一アドレスで発生したNGは優先度の高い方を選択する
			if (ngPriorityDict.Count > 0)
			{
				List<MMFile> retVList = new List<MMFile>();
				foreach (int addrNo in mmDataList.Select(m => m.AddressNO).Distinct())
				{
					try
					{
						MMFile HighData = mmDataList.Find(m => m.AddressNO == addrNo);

						foreach (MMFile mmData in mmDataList.FindAll(m => m.AddressNO == addrNo))
						{
							if (IsHighPriorityThanOriginal(HighData.TranCD, mmData.TranCD, ngPriorityDict) == false)
							{
								continue;
							}
							HighData = mmData;
						}

						// 優先度の高い方を返り値に追加
						retVList.Add(HighData);
					}
					catch (ApplicationException err)
					{
						localErrMsgList.Add(err.Message);
						continue;
					}
				}

				return retVList;
			}

			return mmDataList;
		}

		/// <summary>
		/// originalTranCdの優先度よりもtargetTranCdの優先度(値が小)が高いかどうか調べる
		/// </summary>
		/// <param name="originalTranCd"></param>
		/// <param name="targetTranCd"></param>
		/// <param name="ngPriorityDict"></param>
		/// <returns>targetTranCdの優先度設定値が小さい場合trueを返す</returns>
		protected bool IsHighPriorityThanOriginal(string originalTranCdStr, string targetTranCdStr, Dictionary<int, int> ngPriorityDict)
		{
			int originalTranCd, targetTranCd;

			string errMsg = string.Empty;

			if (int.TryParse(originalTranCdStr, out originalTranCd) == false)
			{
				errMsg += string.Format("処理コード:{0}が数値変換出来ません。", originalTranCdStr); 
			}

			if (int.TryParse(targetTranCdStr, out targetTranCd) == false)
			{
				errMsg += string.Format("処理コード:{0}が数値変換出来ません。", targetTranCdStr); 
			}

			if (string.IsNullOrEmpty(errMsg) == false)
			{
				throw new ApplicationException(errMsg);
			}

			if (ngPriorityDict.Keys.Contains(targetTranCd) == false)
			{
				return false;
			}

			if (ngPriorityDict.Keys.Contains(originalTranCd) == false)
			{
				return true;
				//throw new ApplicationException(string.Format("処理CD{0}がNG優先度設定リスト（計{1}件）内に見つかりません。", originalTranCd, ngPriorityDict.Count));
			}
			else
			{
				int origPriority = ngPriorityDict[originalTranCd];
				int targetPriority = ngPriorityDict[targetTranCd];

				//値の小さい方が優先度が高い
				if (targetPriority < origPriority)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// MMファイル中のカメラNo列のインデックスを取得
		/// </summary>
		/// <param name="fileValue"></param>
		/// <returns></returns>
		private int getCameraNoHeaderColIndex(List<string> fileValue)
		{
			int colIndex = -1;

			string[] headerArray = fileValue[CONTENTS_HEADER_ROW].Split(',');

			for (int i = 0; i < headerArray.Length; i++)
			{
				if (headerArray[i].Contains(CAMERA_NO_HEADER_NAME))
				{
					colIndex = i;
					break;
				}
			}

			return colIndex;
		}

		public List<MMFile> GetNgInspectionData(List<MMFile> mmDataList)
		{
			//検査不良箇所のみにフィルタ
			mmDataList = mmDataList.Where(m => m.TranCD != Constant.MAP_AI_OK
											&& m.TranCD != Constant.MAP_AI_OK2
											&& m.TranCD != Constant.MAP_AI_BadMark).ToList();

			return mmDataList;
		}

        /// <summary>
        /// 閾値判定
        /// </summary>
        /// <param name="lsetInfo">設備情報</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <param name="dataList">ファイル情報</param>
        /// <param name="errMessageList">エラー情報</param>
        public void LimitCheck(LSETInfo lsetInfo, MagInfo magInfo, List<MMFile> dataList, DateTime measureDT, ref List<ErrMessageInfo> errMessageList, int timingNO) 
        {
            string nascaFileData = string.Empty;

            // WB-MMファイルデータ(エラーコードのアドレス)
            Dictionary<int, string> dicWBErrorCdList = new Dictionary<int, string>();
            if (lsetInfo.EquipInfo.SubtractWBMMErrorFg == true)
            {
                dicWBErrorCdList = WBMachineInfo.MMFile.GetWBMMErrorCdData(lsetInfo.InlineCD, magInfo.sNascaLotNO);
            }

            //MMファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.MM), lsetInfo, magInfo.sMaterialCD);
            if (filefmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.MM));
                throw new Exception(message);
            }
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                //閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, aiMachineInfo.LineCD);
				Plm plmInfo = Plm.GetData(aiMachineInfo.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false, lsetInfo.ChipNM);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                //装置エラーNOを取得
				//ErrConvInfo errConvInfo = ConnectDB.GetErrConvInfo(lsetInfo, filefmtInfo.QCParamNO);
				ErrConvInfo errConvInfo = ConnectDB.GetErrConvInfo(lsetInfo.EquipmentNO, filefmtInfo.QCParamNO, aiMachineInfo.LineCD);


                //Nasca不良数を算出
                int nascaDefectCT = 0;
				//TnLogに登録する不良数を算出(BTS.2147)
				int loggingCT = 0;

				if (dataList.Count != 0)
				{
					//対象項目を取得
					List<MMFile> targetDataList = dataList.FindAll(d => d.TranCD == errConvInfo.EquiErrNO);

					SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

					Database.LENS.Lot lotInfo;
					//Lot lotInfo; //LENS2_ApiのappConfigが参照出来ないのでLENS2_Apiが使用出来ない 2015/5/5 nyoshimoto

					foreach (MMFile data in targetDataList)
					{
                        // 機能ON かつ WBのMMファイルのエラーコードのアドレスの場合、集計しない
                        if (lsetInfo.EquipInfo.SubtractWBMMErrorFg == true && dicWBErrorCdList.ContainsKey(data.AddressNO) == true)
                        {
                            continue;
                        }

                        bool isNascaDefect = true;

						SettingInfo settingInfoCommon = SettingInfo.GetSingleton();

						if (settingInfoCommon.IsMappingMode)
						{
							lotInfo = Database.LENS.Lot.GetData(magInfo.sNascaLotNO, lsetInfo.InlineCD);
							//lotInfo = Lot.GetData(magInfo.sNascaLotNO);

							if (lotInfo.IsMappingInspection)
							{
								isNascaDefect = aiMachineInfo.IsNascaDefect(data.AddressNO, magInfo);
							}
						}

						if (isNascaDefect)
						{
						//if (aiMachineInfo.IsNascaDefect(data.AddressNO, magInfo))
						//{
							//NASCAへの不良登録数は変化点フラグの如何によらない。
							nascaDefectCT++;

							if (data.ChangePointFG == true)
							{
								//変化点フラグが1のエラーだけTnLogへ登録する為にカウント(BTS.2147)
								loggingCT++;
							}
						}
					}
				}

                //DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo,
					loggingCT.ToString(), measureDT.ToString(), ref errMessageList);
				//targetDataListのCountからloggingCTに変更　BTS.2147改修K(n.yoshimoto）

                //Nasca不良データ情報を取得
                if (nascaDefectCT != 0)
                {
                    nascaFileData += ConnectDB.GetTmErrConv_NascaErrNO(lsetInfo, Convert.ToInt32(errConvInfo.EquiErrNO)) + "," + nascaDefectCT + Environment.NewLine;
                }
            }

            aiMachineInfo.CheckQC(lsetInfo, timingNO, magInfo.sMaterialCD);

			//Nasca不良ファイル作成
			NascaDefectFile.Create(magInfo.sNascaLotNO, magInfo.sMagazineNO, lsetInfo.EquipmentNO, lsetInfo.InlineCD, nascaFileData);            
			//aiMachineInfo.CreateNasMappingFile(magInfo, lsetInfo.EquipmentNO, nascaFileData);

        }
    }

	public class SMFile
	{
		/// <summary>ファイル内容(日付)</summary>
		private const int FILE_DAYCOL = 1;

		/// <summary>ファイル内容(時間)</summary>
		private const int FILE_TIMECOL = 2;

		/// <summary>ファイル内容マガジンNO行</summary>
		private const int FILE_MAGAZINEROW = 2;

		/// <summary>ファイル内容マガジンNO列</summary>
		private const int FILE_MAGAZINECOL = 3;

		/// <summary>ファイル内容開始行</summary>
		private const int FILE_DATASTARTROW = 2;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		private const int FILE_SM_KEIKOUFGCOL = 16;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		private const int FILE_SM_CHANGEFGCOL = 55;

		private const int COL_OPENINGCHECKFG = 27;

		/// <summary>ファイル内容(傾向管理フラグOFF)</summary>
		private const string KEIKOUFG_OFF = "0";

		/// <summary>ファイル内容(変化点フラグOFF)</summary>
		private const string CHANGEFG_OFF = "0";

		/// <summary>ファイル内容異常値</summary>
		private const double FILE_ERRORVAL = -999000;

		public List<string> ValueList = new List<string>();
		public List<string> DateTimeStrList = new List<string>();
		public string MeasureDT { get; set; }
		public double TargetVAL { get; set; }
		public Constant.OpeningCheckFileType OpeningCheckFileType { get; set; }

		public SMFile()
		{
			OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;
		}

		/// <summary>
		/// ファイル情報を取得
		/// </summary>
		/// <param name="fileValue">ファイル内容</param>
		/// <returns>ファイル情報</returns>
		public static SMFile GetData(List<string> fileValue, int? dataColumn, int samplingModeCD, int samplingCT, bool wbMappingFG, int? openingCheckCOL, int keikouFgCol, int? changeFgCol)
		{
			AlertLog alertLog = AlertLog.GetInstance();

			SMFile smFile = new SMFile();
			int openingCheckRecordCt = 0;
			int recordCt = 0;

			for (int i = FILE_DATASTARTROW; i < fileValue.Count; i++)
			{
				if (fileValue[i] == "")
				{
					//空白行の場合、次へ
					continue;
				}
                //空白行以外の行数カウント
                recordCt++;

				string[] fileLineValue = fileValue[i].Split(',');


				if (openingCheckCOL.HasValue)
				{
					if (fileLineValue[openingCheckCOL.Value].Trim() == AIMachineInfo.OPENINGCHECKFG_ON)
					{
						//オープニングチェックフラグOnの行数カウント
						openingCheckRecordCt++;
						continue;
					}
				}

				if (fileLineValue[keikouFgCol].Trim() == KEIKOUFG_OFF)
				{
					//傾向管理しないデータの場合、次へ
					continue;
				}
				if (wbMappingFG)
				{
					if (changeFgCol.HasValue)
					{
						if (fileLineValue[changeFgCol.Value].Trim() == CHANGEFG_OFF)
						{
							//変化点フラグOFFの場合、次へ
							continue;
						}
					}
				}

   				double measureVAL = 0;

				if (dataColumn.HasValue)
				{
					if (Convert.ToDouble(fileLineValue[dataColumn.Value]) == FILE_ERRORVAL)
					{
						continue;
					}

					if (!double.TryParse(fileLineValue[dataColumn.Value].Trim(), out measureVAL))
					{
						//測定値が数値以外の場合、次へ
						continue;
					}
				}
				else
				{
					continue;
				}

				smFile.DateTimeStrList.Add(Convert.ToString(Convert.ToDateTime(fileLineValue[FILE_DAYCOL] + " " + fileLineValue[FILE_TIMECOL])));
				smFile.ValueList.Add(fileLineValue[dataColumn.Value].Trim());
			}

			if (openingCheckRecordCt == recordCt && openingCheckRecordCt > 0)
			{
				smFile.OpeningCheckFileType = Constant.OpeningCheckFileType.OpeningCheckFile;
			}
			else if (openingCheckRecordCt != recordCt && openingCheckRecordCt > 0)
			{
				smFile.OpeningCheckFileType = Constant.OpeningCheckFileType.ErrorFile;
			}
			else if (openingCheckRecordCt == 0)
			{
				smFile.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

				if (smFile.ValueList.Count > 0)
				{
					smFile = GetSamplingData(smFile, samplingModeCD, samplingCT);
				}
			}

			return smFile;

		}
        public static SMFile GetData(List<string> fileValue,  bool wbMappingFG, int? openingCheckCOL, int keikouFgCol, int? changeFgCol) 
        {
			return GetData(fileValue, null, (int)Constant.SamplingModeCD.AllSampling, 0, wbMappingFG, openingCheckCOL.Value, keikouFgCol, changeFgCol);  
        }

		private static SMFile GetSamplingData(SMFile samplingTargetSMFile, int samplingModeCD, int samplingCT)
		{
			SMFile sampledSMFile = samplingTargetSMFile;
			AlertLog alertLog = AlertLog.GetInstance();

			List<double> samplingDataList = new List<double>();
			int maxIndex = 0;

			if (samplingModeCD != (int)Constant.SamplingModeCD.AllSampling && samplingCT <= 0)
			{
				samplingModeCD = (int)Constant.SamplingModeCD.AllSampling;
				alertLog.logMessageQue.Enqueue(string.Format("SMファイルのデータサンプリングモードとサンプリング数のマスタ設定が矛盾しています。サンプリングモード：抜き取り / サンプリング数：{0}", samplingCT));
			}

			switch (samplingModeCD)
			{
				case (int)Constant.SamplingModeCD.AllSampling:
					sampledSMFile.ValueList = samplingTargetSMFile.ValueList;
					maxIndex = samplingTargetSMFile.ValueList.Count - 1;
					sampledSMFile.MeasureDT = samplingTargetSMFile.DateTimeStrList[maxIndex];
					break;
				case (int)Constant.SamplingModeCD.FrontPartialSampling:
					if (samplingTargetSMFile.ValueList.Count < samplingCT)
					{
						samplingCT = samplingTargetSMFile.ValueList.Count;
						alertLog.logMessageQue.Enqueue(string.Format("SMファイルのデータ行がサンプリング数に満たない集計になっています。サンプリング数：{0}", samplingCT));
					}
					sampledSMFile.ValueList = samplingTargetSMFile.ValueList.Take(samplingCT).ToList();
					maxIndex = samplingCT - 1;
					sampledSMFile.MeasureDT = samplingTargetSMFile.DateTimeStrList[maxIndex];
					break;
				case (int)Constant.SamplingModeCD.RearPartialSampling:
					if (samplingTargetSMFile.ValueList.Count < samplingCT)
					{
						samplingCT = samplingTargetSMFile.ValueList.Count;
						alertLog.logMessageQue.Enqueue(string.Format("SMファイルのデータ行がサンプリング数に満たない集計になっています。サンプリング数：{0}", samplingCT));
					}
					samplingTargetSMFile.ValueList.Reverse();
					sampledSMFile.ValueList = samplingTargetSMFile.ValueList.Take(samplingCT).ToList();
					maxIndex = samplingTargetSMFile.ValueList.Count - 1;
					sampledSMFile.MeasureDT = samplingTargetSMFile.DateTimeStrList[maxIndex];
					break;
				default:
					throw new ApplicationException(string.Format("TmMagマスタのサンプリングモード設定に誤りがあります。"));
			}
			
			return sampledSMFile;
		}
	}
}
