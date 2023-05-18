using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using EICS.Machine;
using EICS.Database;
using EICS.Structure;
using System.Threading;
using System.Threading.Tasks;

namespace EICS
{
    /// <summary>
    /// モールド機処理
    /// </summary>
    public class MDMachineInfo : MachineBase
    {
		/// <summary>モールド装置名</summary>
		public const string ASSETS_NM = "ﾓｰﾙﾄﾞ機";

		public const int LOTINDEX_FILENAME = 2;
		public const string UNKNOWN_LOT_FILENAME = "UNKNOWN";

		protected virtual int PR_KEIKOUFG_COL_INDEX() { return 5; }
		protected virtual int OR_KEIKOUFG_COL_INDEX() { return 15; }
		protected virtual int SM_KEIKOUFG_COL_INDEX() { return 7; }
		protected virtual int AM_KEIKOUFG_COL_INDEX() { return 14; }
		protected virtual int EM_KEIKOUFG_COL_INDEX() { return 28; }
		protected virtual int EF_KEIKOUFG_COL_INDEX() { return 9; }

		protected virtual int QC_TIMING_NO() { return 7; }

        /// <summary>ファイル種類</summary>
        private enum FileType
        {
            EF, OR, PR, SF, EM, SM, AM, SD
        }

		///// <summary>モールド動作パターン</summary>
		//private enum MoldPattern
		//{
		//    /// <summary>縦</summary>
		//    Length,
		//    /// <summary>横</summary>
		//    Side,
		//}

		///// <summary>モールド動作パターン(横)進行方向</summary>
		//private enum MoldMovement
		//{
		//    /// <summary>右</summary>
		//    Right,
		//    /// <summary>左</summary>
		//    Left,
		//}

        #region 定数

        /// <summary>ファイルS*内容(日付)</summary>
        public const int FILE_S_DAY = 1;

        /// <summary>ファイルS*内容(時間)</summary>
        public const int FILE_S_TIME = 2;

        /// <summary>ファイルS*内容列(ｼﾘﾝｼﾞNO)</summary>
        public const int FILE_S_SYRINGENO = 4;

		///// <summary>ファイルS*内容列(傾向管理フラグ)</summary>
		//public const int FILE_S_KEIKOUFG = 7;

        /// <summary>ファイルS*内容(傾向管理フラグOFF)</summary>
        public const string FILE_S_KEIKOUFG_OFF = "0";

        /// <summary>ファイル内容異常値</summary>
        public const string FILE_ERRORVAL = "9999";

        /// <summary>ファイルS*内容開始行</summary>
        public const int FILE_S_DATASTARTROW = 2;

        /// <summary>ファイルS*内容マガジンNO行</summary>
        public const int FILE_S_MAGAZINEROW = 2;

        /// <summary>ファイルS*内容マガジンNO列</summary>
        public const int FILE_S_MAGAZINECOL = 3;

        /// <summary>ファイルOR内容マガジンNO行</summary>
        public const int FILE_OR_MAGAZINEROW = 2;

        /// <summary>ファイルOR内容マガジンNO列</summary>
        public const int FILE_OR_MAGAZINECOL = 3;

        /// <summary>ファイルEF内容マガジンNO行</summary>
        public const int FILE_EF_MAGAZINEROW = 2;

        /// <summary>ファイルEF内容マガジンNO列</summary>
        public const int FILE_EF_MAGAZINECOL = 3;

        /// <summary>ファイルEM内容マガジンNO行</summary>
        public const int FILE_EM_MAGAZINEROW = 2;

        /// <summary>ファイルEM内容マガジンNO列</summary>
        public const int FILE_EM_MAGAZINECOL = 3;

        /// <summary>ファイルAM内容マガジンNO行</summary>
        public const int FILE_AM_MAGAZINEROW = 2;

        /// <summary>ファイルAM内容マガジンNO列</summary>
        public const int FILE_AM_MAGAZINECOL = 3;

        /// <summary>ファイルSD内容マガジン段数列</summary>
        public const int FILE_SD_STEP_NO = 4;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞﾓｰﾙﾄﾞNO列</summary>
        public const int FILE_SD_MOLD_NO = 5;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞ1樹脂小フラグ列</summary>
        public const int FILE_SD_SYRINGE1_FG = 11;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞ2樹脂小フラグ列</summary>
        public const int FILE_SD_SYRINGE2_FG = 12;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞ3樹脂小フラグ列</summary>
        public const int FILE_SD_SYRINGE3_FG = 13;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞ5樹脂小フラグ列</summary>
        public const int FILE_SD_SYRINGE4_FG = 14;

        /// <summary>ファイルSD内容ｼﾘﾝｼﾞ5樹脂小フラグ列</summary>
        public const int FILE_SD_SYRINGE5_FG = 15;

        /// <summary>ファイルSD内容動作パターンNO列</summary>
        public const int FILE_SD_MOLDPATTERN_NO = 16;

        /// <summary>ファイルSD内容シリンジ本数NO列</summary>
        public const int FILE_SD_SYRINGE_NO = 17;

		/// <summary>シリンジ本数 1:5本仕様 2:4本仕様 3:8本仕様</summary>
		public const int FIVE_SYRINGE_CD = 1;
		public const int FOUR_SYRINGE_CD = 2;
        public const int EIGHT_SYRINGE_CD = 3;

		public string IPAddress { get; set; }
		public int PortNO { get; set; }

		public TcpClient tcp = null;
		public NetworkStream ns = null;



        #endregion

		//#region HSMS

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		//#endregion

		protected void InitFileErrorState()
		{
			//IsErrorAM = false;
			//IsErrorEF = false;
			//IsErrorEM = false;
			IsErrorOR = false;
			IsErrorPR = false;
			//IsErrorSD = false;
			//IsErrorSF = false;
			//IsErrorSM = false;
		}

		// マッピングしない作業
		private enum NonMappingWork
		{
			PreMold
		}

        /// <summary>
        /// ファイルチェック 
        /// </summary>
        /// <param name="equipmentInfo"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
			try
			{
				InitFileErrorState();

				CheckDirectory(lsetInfo);

				base.machineStatus = Constant.MachineStatus.Runtime;

				if (IPAddress == null)
					IPAddress = lsetInfo.IPAddressNO;

				if (PortNO == null)
					PortNO = lsetInfo.PortNO;

				MDMachineInfo mdMachineInfo = new MDMachineInfo();
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
				SettingInfo settingInfoCommon = SettingInfo.GetSingleton();

#if Debug
				//lsetInfo.InputFolderNM = @"C:\qcil\data\MD\SA0063(255号機)\";
				//LRMapping中には装置停止命令等がある為、コメント解除時は要注意
				//LRMapping(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
				//ZFMapping(ref tcp, ref ns, lsetInfo, ref errorMessageList);
				//MapMapping(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);

#else
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

				if (SendHeartBeat(lsetInfo, 1, 11) == false)
				{
					return;
				}

                #region マッピングデータ提供　[LENSへ移動 ※マッピング未導入ラインだけEICSから0埋めデータを送信する]

                if (settingInfoPerLine.WBMappingFG == false)
                {
					resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Res_Mapping, Constant.ssuffix_U);
					if (resMsg == "Error")
					{
						base.machineStatus = Constant.MachineStatus.Stop;
						return;
					}
					if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
					{
						if (SendHeartBeat(lsetInfo, 0, 11) == false)
						{
							return;
						}

						resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Memory, 1, Constant.ssuffix_U);
						int nMemoryMode = Convert.ToInt32(resMsg.Trim());

						//if (settingInfoPerLine.MapFG || settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString())
						//{
						//    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "■マッピング情報提供(MAP) 開始", lsetInfo.EquipmentNO);

						//    MapMapping(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
						//}
						//else
						//{
							//resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Memory, 1, Constant.ssuffix_U);
							//int nMemoryMode = Convert.ToInt32(resMsg.Trim());
						if (nMemoryMode == 1)
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "■マッピング情報提供(LR) 開始", lsetInfo.EquipmentNO);

							//LRメモリへ書き込み
							//LRMapping(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
							SetUnMappingDataToLR(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
						}
						else
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "■マッピング情報提供(ZF) 開始", lsetInfo.EquipmentNO);

							//ZFメモリへ書き込み
							//ZFMapping(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
							SetUnMappingDataToZF(ref tcp, ref ns, lsetInfo, ref errorMessageList);
						}
							//}
						
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "□マッピング情報提供完了", lsetInfo.EquipmentNO);

						//マッピング情報提供 立ち下げ
						resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Res_Mapping, 0, 1, Constant.ssuffix_U);
						if (resMsg == "Error")
						{
							base.machineStatus = Constant.MachineStatus.Stop;
							return;
						}
					}
				}
				#endregion 

#endif
				#region [ファイル処理]

				//対象ファイル種類を取得
                Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, true);
                foreach (KeyValuePair<string, string> prefix in prefixList)
                {
					FileProcess(lsetInfo, prefix.Key, prefix.Value, true);
				}

				prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, false);
				foreach (KeyValuePair<string, string> prefix in prefixList)
				{
					FileProcess(lsetInfo, prefix.Key, prefix.Value, false);
				}

                #endregion//--> [ファイル処理]

                #region 装置運転開始時のパラメータ管理

                // QCIL.xml内の設備毎のタグ「PlcParamCheckFG」がTrueの時のみ、処理を行う。
                if (settingInfoPerLine.GetFullParameterFG(lsetInfo.EquipmentNO))
                {
                    CheckPlcParameter(lsetInfo, ref errorMessageList);
                }

                #endregion

                if (SendHeartBeat(lsetInfo, 0, 11) == false)
				{
					return;
				}
            }
            catch (Exception err)
            {
                throw;
            }
            finally
            {
                if (ns != null) { ns.Close(); }
                if (tcp != null) { tcp.Close(); }
            }
        }

		public virtual bool SendHeartBeat(LSETInfo lsetInfo, int heartState, int len)
		{
#if Debug
#else
			SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
			if (settingInfoCommon.IsMappingMode == false)
			{
				KLinkInfo kLinkInfo = new KLinkInfo();
				//「受付準備OKをOFF」
				string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, heartState, len, Constant.ssuffix_U);
				if (resMsg == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return false;
				}
			}
#endif
			return true;
		}

		protected virtual void FileProcess(LSETInfo lsetInfo, string prefixNm, string prefixAddr, bool startUpFG)
		{
            AlertLog alertLog = AlertLog.GetInstance();

            string resMsg = string.Empty;
            KLinkInfo kLinkInfo = new KLinkInfo();
            SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            MagInfo magInfo = new MagInfo();

            bool waitForRenameByArmsFg = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);

            // ファイル取得要求
            resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, Constant.ssuffix_U);
            if (resMsg == "Error")
            {
                string logMsg = string.Format("装置からのﾌｧｲﾙ取得要求の確認時に異常発生 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
                alertLog.logMessageQue.Enqueue(logMsg);

                base.machineStatus = Constant.MachineStatus.Stop;
                return;
            }

            //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
            FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
            int loopCt = 0;
            //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出

            if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
            {
                string logMsg = string.Format("ﾌｧｲﾙ取得要求確認 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg, lsetInfo.EquipmentNO);

                if (startUpFG == false && lsetInfo.EquipInfo.WaitForRenameByArmsFG)
                {
                    //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                    //FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
                    //int loopCt = 0;
                    //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出


                    while (file == null && loopCt < 100)
                    {
                        Thread.Sleep(settingInfoCommon.MachineLogOutWaitmSec);
                        file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
                        loopCt++;
                    }

                    if (file == null)
                    {
                        throw new ApplicationException(string.Format(
                            "ARMSによるﾌｧｲﾙﾘﾈｰﾑを{0}ms {1}回、待機しましたが完了しませんでした。ARMSでﾄﾗﾌﾞﾙが無いか(監視状態の確認含む)確認して下さい。" +
                            "ARMSが問題無い場合 ﾘﾈｰﾑ待機設定が不要か、ﾘﾈｰﾑ完了している場合 EICSの問題の可能性が有ります。ｼｽﾃﾑ担当者へ連絡して下さい。監視ﾊﾟｽ:{2} ﾌｧｲﾙ識別:{3}"
                            , settingInfoCommon.MachineLogOutWaitmSec, loopCt, lsetInfo.InputFolderNM, prefixNm));
                    }

                    magInfo.sNascaLotNO = MachineFile.GetLotFromFileName(file.FullName, 1);
                    magInfo.sMagazineNO = MachineFile.GetMagazineNoFromFileName(file.FullName, 1);
                    magInfo.sMaterialCD = MachineFile.GetTypeFromFileName(file.FullName, 1);
                }
                //else if(startUpFG && lsetInfo.EquipInfo.WaitForRenameByArmsFG)  //2015/11/16 
                else
                {
                    magInfo.sMaterialCD = lsetInfo.TypeCD;
                    magInfo.sMagazineNO = string.Empty;
                    magInfo.sNascaLotNO = string.Empty;
                }








                //EICS.Structure.MachineFile.IsThereLotNoInFileName

                if ((prefixNm == SDFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
                    || (waitForRenameByArmsFg && startUpFG == false))
                {
                    //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                    //FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
                    //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出
                    if (file == null)
                    {
                        return;
                    }

                    if (!MachineFile.IsThereLotNoInFileName(file.FullName))
                    {
                        //SDファイル名にロット情報が付与されていない間は待機
                        return;
                    }

                    if (prefixNm == SDFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
                    {
                        //log000_454254_UNKNOWN_UNKNOWN_0.csvファイル名の時はヘッダのみのﾌｧｲﾙなのでLENS処理は完了させたことにして後の処理で削除等させる
                        if (SDFile.IsUnknownFile(file) == false)
                        {
                            SDFile sdFile = new SDFile(file);
                            if (Database.LENS.WorkResult.IsComplete(sdFile.LotNo, sdFile.ProcNo, this.LineCD) == false)
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

                


                //if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString() && prefix.Key != FileType.PR.ToString())  //BTS.2126により改修
                if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString() && IsStopTargetFileKind(prefixNm))
                {
                    //アウトラインはPRファイルとORファイルのみ異常判定する為、他ファイルはReserveへ移動 //BTS.2126により改修(ORファイル追加)
                    MoveReserveFolderFiles(lsetInfo.InputFolderNM);
                }
                else
                {
                    //ログファイル処理
                    FileDistribution(ref tcp, ref ns, prefixAddr, lsetInfo, prefixNm, magInfo, ref base.errorMessageList);
                }

                if ((settingInfoPerLine.LineType == Constant.LineType.Out.ToString() && base.errorMessageList.Count != 0) || IsStopTargetFileKind(prefixNm))
                {
                    //ファイル処理で異常判定があれば装置停止要求
                    kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
                }                

                //ファイル取得要求 立ち下げ
                resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, 0, 1, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    base.machineStatus = Constant.MachineStatus.Stop;
                    return;
                }

                //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                //ファイル名にUNKNOWNが付いていたらreserveへ
                if (file == null)
                {
                    return;
                }
                if (file.Name.Contains(UNKNOWN_LOT_FILENAME) == true && System.IO.File.Exists(file.FullName) == true)
                {
                    MoveReserveFile(file.FullName, lsetInfo.AssetsNM);
                }
                //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} □MD欠損調査 {1}:{2}取得要求立ち下げ", lsetInfo.EquipmentNO, prefixNm, prefixAddr), lsetInfo.EquipmentNO);
            }
		}



		/// <summary>
		/// ファイル取得要求の取得
		/// </summary>
		/// <param name="machineAddr"></param>
		/// <returns></returns>
		private string GetFileRequestMachineFG(string machineAddr)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();
			return kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, machineAddr, Constant.ssuffix_U);
		}

		/// <summary>
		/// 受付準備OKフラグのゼロリセット
		/// </summary>
		/// <returns></returns>
		private string ReSetReadyMachineFG()
		{
#if Debug
			return string.Empty;
#else
			KLinkInfo kLinkInfo = new KLinkInfo();
			return kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
#endif
		}

		/// <summary>
		/// ファイル取得要求のゼロリセット
		/// </summary>
		/// <param name="machineAddr"></param>
		/// <returns></returns>
		private string ReSetFileRequestMachineFG(string machineAddr)
		{
#if Debug
			return string.Empty;
#else
			KLinkInfo kLinkInfo = new KLinkInfo();

			//ファイル取得要求立下げ
			return kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, IPAddress, Constant.MACHINE_PORT, machineAddr, 0, 1, Constant.ssuffix_U);
#endif
		}

		/// <summary>
		/// MD装置の装置停止信号を送信する。
		/// </summary>
		/// <param name="tcp"></param>
		/// <param name="ns"></param>
		/// <param name="ipAddressNO"></param>
		/// <param name="portNO"></param>
		private void SendStopSignalToMachine()
		{
			
#if Debug
			if (SettingInfo.GetBatchModeFG())
			{
				int test = 0;
			}
			else
			{
				int test1 = 1;
			}
#else
			if(SettingInfo.GetBatchModeFG())
			{
			}
			else//バッチモードで無ければ装置へ停止信号送信
			{
				KLinkInfo kLinkInfo = new KLinkInfo();
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, IPAddress, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
			}
#endif
		}


		public void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string sTargetFile, MagInfo magInfo, ref List<ErrMessageInfo> errMessageList)
        {
			try
			{
				IsErrorOR = false;
				IsErrorPR = false;

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

				SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

				//<--マッピング不一致対策 発生していないが可能性がある為
				//外観検査機とﾓｰﾙﾄﾞ機は、ターゲットファイル(MM,SF)が複数あった場合、最新ファイルしか処理しない。
				//理由=NASCA公開API(マガジン)で実行すると、間違ったLotと紐付き、間違ったマッピングファイル作成・モールドへ提供を行う危険性がある為。
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

					i = 0;
					for (i = 0; i < nSameTargetFileNum - 1; i++)
					{
						foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, sTargetFile + sortedCreateTime[i] + ".*"))
						{
							//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
							sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
							spath1 = lsetInfo.InputFolderNM + "reserve";
							spath2 = spath1 + "\\" + sfname;

							if (Directory.Exists(spath1) == false)
							{
								Directory.CreateDirectory(spath1);
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
				//}
				//-->マッピング不一致対策 発生していないが可能性がある為

				//「LogファイルデータをDB登録
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
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} 項目のみの空ファイルの為、削除 ファイル：{1}", lsetInfo.EquipmentNO, swfname), lsetInfo.EquipmentNO);
						File.Delete(swfname);
						continue;
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} □MD欠損調査 {1}処理開始", lsetInfo.EquipmentNO, sFileType), lsetInfo.EquipmentNO);

					//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
					switch (sFileType)
					{
						case "AM":
                            DbInput_MD_AMFile(lsetInfo, magInfo, textArray, ref errMessageList);
                            break;
						case "EF":
							DbInput_MD_EFFile(lsetInfo, textArray, ref errMessageList);
							break;
						case "EM":
							DbInput_MD_EMFile(lsetInfo, textArray, ref errMessageList);
							break;
						case "OR":
							DbInput_MD_ORFile(lsetInfo, magInfo, textArray, ref errMessageList);
							break;
						case "PR":
							DbInput_MD_PRFile(lsetInfo, textArray, ref errMessageList);
							break;
						case "SF":
							DbInput_MD_SFFile(lsetInfo, textArray, ref errMessageList);
							break;
						case "SM":
							DbInput_MD_SMFile(lsetInfo, magInfo, textArray, ref errMessageList);
							break;
						case "SD":
							//if (Enum.GetNames(typeof(NonMappingWork)).Contains(lsetInfo.ChipNM))
							//{
							//    //ChipNMがNonMappingWorkに含まれる値の場合はSDファイル処理しない(2014/12/19 n.yoshimoto ZD白樹脂塗布時の対応)
							//}
							//else
							//{
							//    DbInput_MD_SDFile(lsetInfo, textArray, ref errMessageList);
							//}
							break;
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} ■MD欠損調査 {1}処理終了", lsetInfo.EquipmentNO, sFileType), lsetInfo.EquipmentNO);

					//処理済みファイルを保管フォルダへ移動
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, "", "");
				}
			}
			catch (Exception err)
			{
				throw;
			}
        }

		/// <summary>MD装置からシリンジ本数のCDを取得</summary>
		/// <returns>1:5本 2:4本</returns>
		private int getSyringeNumberCD(LSETInfo lsetInfo)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();
			//使用シリンジ数取得    1：5本      2：4本     3: 8本
			string resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_SyringeNum, 1, Constant.ssuffix_U);
			int nUseSyringeNum = Convert.ToInt32(resMsg.Trim());

			return nUseSyringeNum;
		}

        private int GetSyringeCount(LSETInfo lsetInfo)
        {
            int syringeNumCD = getSyringeNumberCD(lsetInfo);
            if (syringeNumCD == 1)
            {
                // 5本
                return 5;
            }
            else if (syringeNumCD == 2)
            {
                // 4本
                return 4;
            }
            else if (syringeNumCD == 3)
            {
                // 8本
                return 8;
            }
            else if (syringeNumCD == 4)
            {
                // 2本
                return 2;
            }
            else
            {
                throw new ApplicationException(string.Format("装置から未知のシリンジ本数CDを取得しました。シリンジ本数CD:{0}", syringeNumCD));
            }
        }


		protected virtual string getSyringeParamSearchNmCondition(string searchNm, int syringe)
		{
			return searchNm + syringe;
			//return string.Format(searchNm, syringe);
		}


        public FileValueInfo GetAllSyringeFileValue(string[] fileLineValueList, string searchNM, string totalKB, int syringeCt)
        {
            FileValueInfo fileValueInfo = new FileValueInfo();
            List<double> valueList = new List<double>();

            for (int syringe = 1; syringe <= syringeCt; syringe++)
            {
                //値を取得する列Noをヘッダから取得(検索文字+シリンジNo)
                int valueColumnNO = fileLineValueList[1].Split(',').ToList()
                    .FindIndex(f => f.Contains(getSyringeParamSearchNmCondition(searchNM, syringe)));
                if (valueColumnNO == -1)
                {
                    throw new Exception(string.Format(
                        "全シリンジの集計中、集計対象の検索列(検索文字:{0})が見つからなかった為、異常判定ができませんでした。傾向管理ファイルの内容が正しいか確認して下さい",
                        searchNM + syringeCt));
                }

                for (int i = FILE_S_DATASTARTROW; i < fileLineValueList.LongLength; i++)
                {
                    if (fileLineValueList[i] == "")
                    {
                        //空白行の場合、次へ
                        continue;
                    }

                    string[] fileValue = fileLineValueList[i].Split(',');
                    if (fileValue[AM_KEIKOUFG_COL_INDEX()].Trim() == FILE_S_KEIKOUFG_OFF)
                    {
                        //傾向管理しないデータの場合、次へ
                        continue;
                    }

                    if (fileValue[valueColumnNO].Trim() == FILE_ERRORVAL)
                    {
                        //測定値が異常値の場合、次へ
                        continue;
                    }

                    double measureVAL = 0;
                    if (!double.TryParse(fileValue[valueColumnNO].Trim(), out measureVAL))
                    {
                        //測定値が数値以外の場合、次へ
                        continue;
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_S_DAY] + " " + fileValue[FILE_S_TIME]));
                    valueList.Add(measureVAL);
                }
            }

            if (valueList.Count == 0)
            {
                //対象が存在しない場合、処理を抜ける
                return fileValueInfo;
            }

            double rValue = 0;
            switch (totalKB)
            {
                case "AVE":
                    rValue = calcAvg(valueList);
                    break;
                case "SIGMA":
                    double avgVAL = calcAvg(valueList);
                    rValue = calcSigma(valueList, avgVAL);
                    break;
                case "MODE":
                    rValue = CalcMode(valueList);
                    break;
                case "MAX":
                    rValue = CalcMax(valueList);
                    break;
                case "MIN":
                    rValue = CalcMin(valueList);
                    break;
                default:
                    rValue = valueList[0];
                    break;
            }
            fileValueInfo.TargetStrVAL = Convert.ToString(rValue);

            return fileValueInfo;

        }




        /// <summary>
        /// バッチ取り込み処理用関数
        /// </summary>
        public void ManualFileProcessing(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList, DateTime startDT, DateTime endDT)
		{
			//対象ファイル種類を取得
			Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0);

			foreach (KeyValuePair<string, string> prefix in prefixList)
			{
				FileCheck(lsetInfo, prefix.Key, startDT, endDT, false);
				ProcessingAllTargetFile(lsetInfo, prefix.Key, ref errMessageList);
			}
		}


		private void FileCheck(LSETInfo lsetInfo, string prefix, DateTime? startDT, DateTime? endDT, bool isMoveOlderFile)
		{
			MachineFileInfo machineFileInfo = new MachineFileInfo();

			string sMessage = "";
			string spath1 = "", spath2 = "";

			//string sWork = "";
			string sfname = "";
			//string sFileType = "";
			List<string> fileList = Common.GetFiles(lsetInfo.InputFolderNM, ".*" + prefix + ".*");

			int nSameTargetFileNum = fileList.Count;
			string[] sortedCreateTime = new string[nSameTargetFileNum];

			//if (nSameTargetFileNum > 1)
			//{
			int i = 0;
			sMessage = lsetInfo.AssetsNM + "/TargetFile=" + prefix + "/[" + nSameTargetFileNum + "]ありました。";
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

			//reserveフォルダがなければ、作成
			spath1 = lsetInfo.InputFolderNM + "reserve";
			if (Directory.Exists(spath1) == false)
			{
				Directory.CreateDirectory(spath1);
			}

			//ファイル名から日付文字列取得
			foreach (string swfname in fileList)
			{
				sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
				spath2 = spath1 + "\\" + sfname;

				if (startDT.HasValue && endDT.HasValue)
				{
					DateTime fileDT = GetDTFromLogFile(swfname, FILE_S_DAY, FILE_S_TIME);
					if (fileDT < startDT.Value || endDT.Value < fileDT)//指定日時範囲内にファイル内の日時が入って無い場合、移動
					{
						//移動先に同名ファイルが無い場合、移動
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
				
				sortedCreateTime[i] = sfname.Substring(9, 10);//ﾌｧｲﾙ名に付加されている日付文字列取得
				i = i + 1;
			}

			Array.Sort(sortedCreateTime);

			for (i = 0; i < nSameTargetFileNum - 1; i++) //nSameTargetFileNum - 1とする事で最新ファイル以外を処理する
			{
				foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, ".*" + prefix + sortedCreateTime[i] + ".*"))
				{
					//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
					sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
					spath2 = spath1 + "\\" + sfname;

					//-->人搬送量試時にモールド機が夜間に止まった対応2 2010/09/03 Y.Matsushima

					if (File.Exists(spath2) == false)
					{
						if (isMoveOlderFile)
						{
							File.Move(swfname, spath2);
						}
					}
					else//既にある場合、delete
					{
						File.Delete(swfname);
					}
				}
			}
			//}
		}

		public void ProcessingAllTargetFile(LSETInfo lsetInfo, string prefix, ref List<ErrMessageInfo> errMessageList)
		{

			MagInfo magInfo = null;
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			//「LogファイルデータをDB登録
			List<string> machineFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, prefix);
			foreach (string swfname in machineFileList)
			{
				string[] textArray;

				MachineFileInfo machineFileInfo = new MachineFileInfo();
				FileInfo fileInfo = new FileInfo(swfname);

				//<--人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima
				machineFileInfo = GetFileInfo(fileInfo.FullName, lsetInfo.InputFolderNM);

				//-->人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima

				textArray = machineFileInfo.Content.Split('\n');
				//項目のみの空ﾌｧｲﾙの場合削除
				if (textArray[2] == "")
				{
					File.Delete(swfname);
					continue;
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("□MD欠損調査 {0}処理開始", machineFileInfo.Type), lsetInfo.EquipmentNO);

				//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
				switch (prefix)
				{
					case "AM":
                        DbInput_MD_AMFile(lsetInfo, magInfo, textArray, ref errMessageList);
                        break;
					case "EF":
						DbInput_MD_EFFile(lsetInfo, textArray, ref errMessageList);
						break;
					case "EM":
						DbInput_MD_EMFile(lsetInfo, textArray, ref errMessageList);
						break;
					case "OR":
						DbInput_MD_ORFile(lsetInfo, magInfo, textArray, ref errMessageList);
						break;
					case "PR":
						DbInput_MD_PRFile(lsetInfo, textArray, ref errMessageList);
						break;
					case "SF":
						DbInput_MD_SFFile(lsetInfo, textArray, ref errMessageList);
						break;
					case "SM":
						DbInput_MD_SMFile(lsetInfo, magInfo, textArray, ref errMessageList);
						break;
					case "SD":
						//DbInput_MD_SDFile(lsetInfo, textArray, ref errMessageList);
						break;
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■MD欠損調査 {0}処理終了", machineFileInfo.Type), lsetInfo.EquipmentNO);

				//処理済みファイルを保管フォルダへ移動
				MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, "", "");
			}
		}

		public void CheckPlcParameter(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			CheckPlcParameter(lsetInfo, ref errMessageList, true);
		}

		/// <summary>
		/// 装置のPLCから取得したデータと閾値を判定する
		/// </summary>
		/// <param name="moldPatternID"></param>
		/// <returns</returns>
		public virtual void CheckPlcParameter(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList, bool precedingJudgeResult)
        {
            const string Message_158 = "[{0}/{1}号機][管理番号:{8}/{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},プログラム名={6},Linecd={7}";
            const string Message_159 = "[{0}/{1}号機][管理番号:{7}/{2}]の設定値に誤りがあります。取得値={3},閾値={4},プログラム名={5},Linecd={6}";
            const string Message_160 = "[{0}/{1}号機][管理番号:{5}/{2}]の閾値が設定されていません。プログラム名={3},Linecd={4}";
            const string Message_161 = "[{0}/{1}号機]パラメータ取得用PLCアドレスマスタが設定されていません。";

            bool IsOKPrm = true;

            // PLCをインスタンス
            PLC_Keyence plc = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);
            if (plc == null)
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCへの接続に失敗しました。 PLC = {2}, Port = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, lsetInfo.IPAddressNO, lsetInfo.PortNO);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLC接続失敗, PLC = {1}, Port = {2}, ", lsetInfo.EquipmentNO, lsetInfo.IPAddressNO, lsetInfo.PortNO));

                return;
            }

            //対象ファイル種類のフラグ用PLCアドレス情報を取得
            PlcAddrInfo plcAddrInfo = PlcAddrInfo.GetData(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcAddrInfo == null)
            {
                return;
            }

            #region 各マスタの列名の有無を確認

            // 各マスタの列名の有無を確認 
            if (string.IsNullOrEmpty(plcAddrInfo.StartONADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "運転開始要求フラグ取得アドレス", plcAddrInfo.StartONADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.StartONADDR));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrgNmADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。", lsetInfo.AssetsNM, lsetInfo.MachineNM, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmADDR));
            }
            if (plcAddrInfo.PrgNmLEN <= 0)
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が不正です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "プログラム名取得文字数", plcAddrInfo.PrgNmLEN);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」不正。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmLEN));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrmOKADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "OK信号-送信用アドレス", plcAddrInfo.PrmOKADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "OK信号-送信用アドレス", plcAddrInfo.PrmOKADDR));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrmNGADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "NG信号-送信用アドレス", plcAddrInfo.PrmNGADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "NG信号-送信用アドレス", plcAddrInfo.PrmNGADDR));
            }
            #endregion

            // 運転開始フラグ取得要求
            try
            {
                if (plc.GetDataAsString(plcAddrInfo.StartONADDR, 1, PLC.DT_UDEC_16BIT) == PLC.BIT_ON)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-運転開始フラグONを確認", lsetInfo.EquipmentNO));
                }
                else
                {
                    // 運転開始ではない為、処理中断
                    return;
                }
            }
            catch (Exception err)
            {
                // 運転開始フラグ取得失敗  ⇒  メッセージ出力
                throw;
            }

            // 装置のパラメータ管理リスト(取得PLCアドレス + 閾値)
            List<PlcFileConv> plcfileconvList = Database.PlcFileConv.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcfileconvList == null || plcfileconvList.Count == 0)
            {
                string sMessage = string.Format(Message_161, lsetInfo.AssetsNM, lsetInfo.MachineNM);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                //errMessageList.Add(errMessageInfo); 
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-パラメータ取得用PLCアドレスマスタ無, ", lsetInfo.EquipmentNO));

                // 装置へNG信号を返す
                SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                throw new Exception(errMessageInfo.MessageVAL);

                //return;
            }

            List<int> qcparamNOList = new List<int>();
            foreach (PlcFileConv pfc in plcfileconvList)
            {
				if (pfc.QcParamNO.HasValue)
				{
					qcparamNOList.Add(pfc.QcParamNO.Value);
				}
				else
				{
					throw new ApplicationException(string.Format("■設備CD={0}：MD装置パラメータチェック-パラメータ取得用PLCアドレスマスタ異常 QcParamNoがNULLです。ﾒﾓﾘAddr:{1} ", lsetInfo.EquipmentNO, pfc.PlcADDR));
				}
            }

            // プログラム名取得
            string ProgramNM = plc.GetString_AS_ShiftJIS(plcAddrInfo.PrgNmADDR, plcAddrInfo.PrgNmLEN);
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-プログラム名 = {1}", lsetInfo.EquipmentNO, ProgramNM));

            // プログラム名と取得したパラメータ管理リストの管理Noをキーに閾値リストを取得
            List<Plm> plmList = ConnectDB.GetPLMListData(qcparamNOList, lsetInfo.ModelNM, lsetInfo.EquipmentNO, ProgramNM, lsetInfo.InlineCD, null);

            string rMessage = "";
            //string lMessage = "";

            // 取得データの確認
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                string sMessage = "";

                // 閾値の取得
                Plm plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).Where(p => p.EquipmentNO == lsetInfo.EquipmentNO).FirstOrDefault();
                if (plmInfo == null)
                {
                    plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).FirstOrDefault();

                    // 装置CDが空欄の閾値レコードもない場合、エラーと判断して、処理中断
                    if (plmInfo == null)
                    {
                        string pNM;
                        try
                        {
							if (pfcInfo.QcParamNO.HasValue)
							{
								pNM = ConnectDB.GetPRMElement("Parameter_NM", pfcInfo.QcParamNO.Value, lsetInfo.InlineCD).ToString();
							}
							else
							{
								throw new ApplicationException(
									string.Format("■設備CD={0}：MD装置パラメータチェック-パラメータ取得用PLCアドレスマスタ異常 QcParamNoがNULLです。ﾒﾓﾘAddr:{1} ", lsetInfo.EquipmentNO, pfcInfo.PlcADDR));
							}
                        }
                        catch (Exception err)
                        {
                            sMessage = string.Format("[{0}/{1}号機] 閾値名マスタ(TmPRM)に管理No[{2}]のマスタが登録されていません。", lsetInfo.AssetsNM, lsetInfo.MachineNM, pfcInfo.QcParamNO);
                            rMessage += sMessage + "\n";

                            //IsOKPrm = false;

                            //continue;

                            // 装置へNG信号を返す
                            SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                            throw new Exception(sMessage);

                        }
                        sMessage = string.Format(Message_160, lsetInfo.AssetsNM, lsetInfo.MachineNM, pNM, ProgramNM, lsetInfo.InlineCD, pfcInfo.QcParamNO);
                        //tMessage += sMessage + "\r\n";
                        //ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        //errMessageList.Add(errMessageInfo); 
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                        string.Format("■設備CD={0}：MD装置パラメータチェック-閾値マスタ無, 管理NO = {1}, 管理名 = {2}, ", lsetInfo.EquipmentNO, pfcInfo.QcParamNO, pNM));

                        //IsOKPrm = false;

                        //continule;

                        // 装置へNG信号を返す
                        SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                        throw new Exception(sMessage);

                    }
                }

                pfcInfo.Plm = plmInfo;
            }
            if (!IsOKPrm)
            {
                // 装置へNG信号を返す
                SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                throw new Exception(rMessage);

                //return;
            }



            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-各PLCアドレスからデータを取得 = {1}", lsetInfo.EquipmentNO, ProgramNM));

            // パラメータ管理リストから各アドレスの値を格納
            List<PLC_Address> tmpAdrList = new List<PLC_Address>();
            foreach (PlcFileConv tmpData in plcfileconvList)
            {
                tmpAdrList.Add(new PLC_Address(tmpData.PlcADDR, tmpData.DataTypeCD, tmpData.DataLen));
            }

            // アドレス種類毎の先頭・最後尾アドレスを算出 (リストを作成)
            Dictionary<string, PLC_Device> plcdevList = PLC_Device.GetPLCDeviceList(lsetInfo, tmpAdrList);

            // 各アドレスの値をDirectoryで取得する
            Dictionary<string, decimal> AddressValue = PLC_Device.GetAdrValList(plc, plcdevList);


            #region NG判定

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-NG判定開始", lsetInfo.EquipmentNO));

            // 取得データ ⇔ 閾値の判定
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                string sValue = "";
                string sMessage = "";

                //// 閾値の取得
                //Plm plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).Where(p => p.EquipmentNO == lsetInfo.EquipmentCD).FirstOrDefault();
                //if (plmInfo == null)
                //{
                //    plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).FirstOrDefault();

                //    // 装置CDが空欄の閾値レコードもない場合、エラーと判断
                //    if (plmInfo == null)
                //    {
                //        sMessage = string.Format(Message_159, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                //        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                //        string.Format("■設備CD={0}：MD装置パラメータチェック-閾値マスタ無, 管理NO = {1}, 管理名 = {2}, ", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM));

                //        IsOKPrm = false;
                //    }
                //}

                Plm plmInfo = pfcInfo.Plm;

                if (pfcInfo.DataTypeCD == PLC.DT_STR)
                {
                    // データが文字の場合は、このタイミングでPLCからデータを取得する。
                    sValue = plc.GetString_AS_ShiftJIS(pfcInfo.PlcADDR, pfcInfo.DataLen);
                    // NG判定を行う
                    if (ConnectDB.NGJudge(plmInfo, sValue))
                    {
                        sMessage = string.Format(Message_159, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        errMessageList.Add(errMessageInfo);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                        string.Format("■設備CD={0}：MD装置パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, 閾値 = {4}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL));

                        IsOKPrm = false;
                    }
                }
                else
                {
                    // NG判定を行う
                    decimal ChangeUnitValue = Convert.ToDecimal(CalcChangeUnit(pfcInfo.Plm.ChangeUnitVal, Convert.ToDouble(AddressValue[pfcInfo.PlcADDR])));
                    if (ConnectDB.NGJudge(plmInfo, ChangeUnitValue))
                    {
                        sValue = ChangeUnitValue.ToString();

                        if (plmInfo.ManageNM == Constant.sMAXMIN)
                        {
                            sMessage = string.Format(Message_158, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, "MAX-MIN", AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX + "-" + plmInfo.ParameterMIN, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                                string.Format("■設備CD={0}：MD装置パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, {4} <= 閾値 <= {5}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMIN, plmInfo.ParameterMAX));
                        }
                        else if (plmInfo.ManageNM == Constant.sMAX)
                        {
                            sMessage = string.Format(Message_158, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, "MAX", AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                                string.Format("■設備CD={0}：MD装置パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, 閾値 <= {4}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX));
                        }

                        IsOKPrm = false;
                    }
                }
            }

			#endregion


			bool chkResult = precedingJudgeResult & IsOKPrm;

			SendOKNG(chkResult, plc, plcAddrInfo, lsetInfo);
        }

        ///<summary>
        /// OK信号またはNG信号を返す。
        ///</summary>
        protected void SendOKNG(bool OKFg, PLC_Keyence plc, PlcAddrInfo plcAddrInfo, LSETInfo lsetInfo)
        {
            if (OKFg)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-装置へOK信号を送信", lsetInfo.EquipmentNO));

                // 装置へOK信号を返す
                plc.SetBit2(plcAddrInfo.PrmOKADDR, PLC.BIT_ON);

            }
            else
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：MD装置パラメータチェック-装置へNG信号を送信", lsetInfo.EquipmentNO));

                // 装置へNG信号を返す
                plc.SetBit2(plcAddrInfo.PrmNGADDR, PLC.BIT_ON);
            }


            // 運転開始フラグ取得要求
            plc.SetBit2(plcAddrInfo.StartONADDR, PLC.BIT_OFF);
        }



        #region 各ファイル処理

        //[EF:ｴﾗｰﾛｸﾞ/ｽﾀｰﾄ直後]
        private void DbInput_MD_EFFile(LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo magInfo = null;

            string[] recordArray = new string[] { };

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("EF", lsetInfo, lsetInfo.TypeCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                int nRowCnt = 0;                //行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();

				SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                    {
                        continue;
                    }

                    int nflg = Convert.ToInt32(recordArray[9]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

                    if (magInfo == null)
                    {
                        magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_EF_MAGAZINEROW, FILE_EF_MAGAZINECOL);
                    }
                    ////自動化 or ハイブリッドラインの場合
                    //if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    //{
                    //    fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                    //    fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                    //    if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                    //    {
                    //        //<--Package 古川さん待ち             
                    //        ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(fileValueInfo.MagazineNO);
                    //        if (rtnArmsLotInfo == null)
                    //        {
                    //            MagInfoMD.sMagazineNO = fileValueInfo.MagazineNO;
                    //            MagInfoMD.sNascaLotNO = null;
                    //            MagInfoMD.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

                    //            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
                    //            //    string.Format("※MD欠損調査 {0} ライン:{1} 設備番号:{2} マガジンNO:{3} ロット紐付けNG", FileType.EF.ToString(), lsetInfo.InlineCD, lsetInfo.EquipmentNO, MagInfoMD.sMagazineNO), lsetInfo.EquipmentNO);
                    //        }
                    //        else
                    //        {
                    //            MagInfoMD.sMagazineNO = fileValueInfo.MagazineNO;
                    //            MagInfoMD.sNascaLotNO = rtnArmsLotInfo.LotNO;
                    //            MagInfoMD.sMaterialCD = rtnArmsLotInfo.TypeCD;
                    //        }
                    //        //-->Package 古川さん待ち   

                    //        fGetMagNo = true;//初回のみ実行
                    //    }
                    //}
                    ////人搬送の場合
                    //else if(settingInfo.LineType == Constant.LineType.High.ToString())
                    //{
                    //    fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
                    //    fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                    //    if (fGetMagNo == false)//1回だけ実行
                    //    {
                    //        MagInfoMD = SetMagInfo(MagInfoMD, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                    //        fGetMagNo = true;//初回のみSet
                    //    }
                    //}

                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

                    //ｼﾘﾝｼﾞ1ﾜｲﾔｰﾀｯﾁ異常(MD)
					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.sStartAfter1F);
					plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                    if (plmInfo != null)
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                    }
                    else
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");
					}
				}
            }
        }

        //[OR:装置設定ﾊﾟﾗﾒｰﾀｰ/直前]
        private void DbInput_MD_ORFile(LSETInfo lsetInfo, MagInfo magInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
 
            ParamInfo paramInfo = new ParamInfo();

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			if (lsetInfo.EquipInfo.HasLoaderQRReader)
			{
				//マガジン情報取得
				magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_OR_MAGAZINEROW, FILE_OR_MAGAZINECOL);
			}

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("OR", lsetInfo, magInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                int nRowCnt = 0;                //行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                    {
                        continue;
                    }

					int nflg = Convert.ToInt32(recordArray[OR_KEIKOUFG_COL_INDEX()]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					//if (magInfo == null || string.IsNullOrEmpty(magInfo.sMaterialCD))
					//{
					//	magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_OR_MAGAZINEROW, FILE_OR_MAGAZINECOL);
					//}

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.sStartBefore);
					plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
					
                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();
                    if (paramInfo.ChangeUnitVAL != "0") 
                    {
                        fileValueInfo.TargetStrVAL = Convert.ToString(CalcChangeUnit(filefmtInfo.QCParamNO, Convert.ToDouble(fileValueInfo.TargetStrVAL)));
                    }

					int errCount = errMessageList.Count;

					ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);

					if (errMessageList.Count > errCount)
					{
						IsErrorOR = true;
					}
				}
            }
        }

        //[PR:装置設定ﾊﾟﾗﾒｰﾀｰ/ｽﾀｰﾄ直前] -->ﾌﾟﾛｸﾞﾗﾑ名のみ
        private void DbInput_MD_PRFile(LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            string[] recordArray = new string[] { };

            //マガジン情報取得
            MagInfo magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_S_MAGAZINEROW, FILE_S_MAGAZINECOL);
            if (magInfo.sNascaLotNO == null)
            {
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
				//    string.Format("※MD欠損調査 {0} ライン:{1} 設備番号:{2} マガジンNO:{3} ロット紐付けNG", FileType.PR.ToString(), lsetInfo.InlineCD, lsetInfo.EquipmentNO, MagInfoMD.sMagazineNO), lsetInfo.EquipmentNO);            
            }

            //PRファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.PR), lsetInfo, magInfo.sMaterialCD);

			List<FileFmt> noChkFileFmt = new List<FileFmt>();

			if (filefmtList.Count == 0)
			{
				noChkFileFmt = FileFmt.GetData(lsetInfo, null, Convert.ToString(FileType.PR), Constant.NO_CHK_QCPARAM_NO);
			}

			if (noChkFileFmt.Count >= 1)
			{
				return;
			}

			if (filefmtList.Count == 0)
			{
				//設定されていない場合、装置処理停止
				string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(Convert.ToString(FileType.PR)));
				throw new Exception(message);
			}

			// CRMDでファイルが出るが取得要求を落とすだけの処理の為、下記はコメントアウトで上に処理追加
			//if (filefmtList.Count == 0)
			//{
			//	//設定されていない場合、装置処理停止
			//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.PR));
			//	throw new Exception(message);
			//}

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                //閾値マスタ情報(TmPLM)取得
				Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                int nRowCnt = 0;                //行ｶｳﾝﾄ

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                    {
                        continue;
                    }

                    int nflg = Convert.ToInt32(recordArray[5]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));
                    fileValueInfo.TargetStrVAL = Convert.ToString(recordArray[filefmtInfo.ColumnNO]).Trim();
                    fileValueInfo.TargetStrVAL = fileValueInfo.TargetStrVAL.Replace("\"", "");//余計な文字削除

                    //"("があれば、それ以降は削除
                    int npos = -1;
                    npos = fileValueInfo.TargetStrVAL.IndexOf("(");
                    //"("があった場合
                    if (npos != -1)
                    {
                        fileValueInfo.TargetStrVAL = fileValueInfo.TargetStrVAL.Substring(0, npos);//例："204(超硬T)"→"204"
                    }

					int errCount = errMessageList.Count;

                    //異常判定+DB登録
					ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL.ToUpper(), fileValueInfo.MeasureDT, ref errMessageList);

					if (errMessageList.Count > errCount)
					{
						IsErrorPR = true;
					}
                }
            }
        }

        //[SF:製品出来栄え/ｽﾀｰﾄ直後]
        private void DbInput_MD_SFFile(LSETInfo lsetInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList)
        {
            try
            {
                //マガジン情報取得
                MagInfo magInfo = GetMagazineInfo(lsetInfo, fileLineValueList, FILE_S_MAGAZINEROW, FILE_S_MAGAZINECOL);
                if (magInfo.sNascaLotNO == null) 
                {
					//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
					//    string.Format("※MD欠損調査 {0} ライン:{1} 設備番号:{2} マガジンNO:{3} ロット紐付けNG", FileType.SF.ToString(), lsetInfo.InlineCD, lsetInfo.EquipmentNO, magInfo.sMagazineNO), lsetInfo.EquipmentNO);
                }

                //SFファイルの紐付けマスタ情報(TmFILEFMT)を取得
				List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.SF), lsetInfo, magInfo.sMaterialCD);
                if (filefmtList.Count == 0)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.SF));
                    throw new Exception(message);
                }
                foreach (FILEFMTInfo filefmtInfo in filefmtList)
                {
                    //閾値マスタ情報(TmPLM)取得
					Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                    if (plmInfo == null)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                        throw new Exception(message);
                    }

                    //必要ファイル内容取得
                    FileValueInfo fileValueInfo = GetSyringeFileValue(fileLineValueList, filefmtInfo.SearchNM, filefmtInfo.ColumnNO, plmInfo.TotalKB);
                    if (fileValueInfo.TargetStrVAL == null)
                    {
                        continue;
                    }

                    //異常判定+DB登録
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                }
            }
            catch (Exception err) 
            {
                throw;
            }
        }

        //[EM:エラーログ/1マガジン毎]
        private void DbInput_MD_EMFile(LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo magInfo = null;

            string[] recordArray = new string[] { };
			//DateTime dtMeasureDT = Convert.ToDateTime("9999/01/01");

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			//SpecificFileFmt specificFileFmt = SpecificFileFmt.GetData(lsetInfo.InlineCD, lsetInfo.ModelNM, "EM");

			int keikouFgColIndex = EM_KEIKOUFG_COL_INDEX();

			//if (specificFileFmt != null && specificFileFmt.TrendMngModeColNO.HasValue)
			//{
			//	keikouFgColIndex = specificFileFmt.TrendMngModeColNO.Value;
			//}

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("EM", lsetInfo, lsetInfo.TypeCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                int nRowCnt = 0;              //行ｶｳﾝﾄ
                int nflgCnt = 0;              //傾向管理無効行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                    {
                        nflgCnt = nflgCnt + 1;
                        continue;
                    }

                    int nflg = Convert.ToInt32(recordArray[keikouFgColIndex]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        nflgCnt = nflgCnt + 1;
                        continue;//何もせずに次へ
                    }

                    string dtMeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));
                    fileValueInfo.MeasureDT = dtMeasureDT;

                    if (magInfo == null)
                    {
                        magInfo = GetMagazineInfo(lsetInfo, textArray, FILE_EM_MAGAZINEROW, FILE_EM_MAGAZINECOL);
                    }
                    ////自動化 or ハイブリッドラインの場合
                    //if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    //{
                    //    fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                    //    fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                    //    if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                    //    {
                    //        //<--Package 古川さん待ち             
                    //        ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(fileValueInfo.MagazineNO);
                    //        if (rtnArmsLotInfo == null)
                    //        {
                    //            MagInfoMD.sMagazineNO = fileValueInfo.MagazineNO;
                    //            MagInfoMD.sNascaLotNO = null;
                    //            MagInfoMD.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

                    //            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR,
                    //            //    string.Format("※MD欠損調査 {0} ライン:{1} 設備番号:{2} マガジンNO:{3} ロット紐付けNG", FileType.EM.ToString(), lsetInfo.InlineCD, lsetInfo.EquipmentNO, MagInfoMD.sMagazineNO), lsetInfo.EquipmentNO);
                    //        }
                    //        else
                    //        {
                    //            MagInfoMD.sMagazineNO = fileValueInfo.MagazineNO;
                    //            MagInfoMD.sNascaLotNO = rtnArmsLotInfo.LotNO;
                    //            MagInfoMD.sMaterialCD = rtnArmsLotInfo.TypeCD;
                    //        }
                    //        //-->Package 古川さん待ち   

                    //        fGetMagNo = true;//初回のみ実行
                    //    }
                    //}
                    ////アウト or 人搬送ラインの場合
                    //else
                    //{
                    //    fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
                    //    fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                    //    if (fGetMagNo == false)//1回だけ実行
                    //    {
                    //        MagInfoMD = SetMagInfo(MagInfoMD, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                    //        fGetMagNo = true;//初回のみSet
                    //    }
                    //}

                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

				}
                //全行傾向管理無効の場合、データベース登録なし
                if (nflgCnt == nRowCnt)
                {
                    return;
                }

                //DB登録 -> 集計したエラー回数をエラー毎に登録
				paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);//"ｼﾘﾝｼﾞ1ﾜｲﾔｰﾀｯﾁ異常"
				plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                if (plmInfo != null)
                {
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                }
                else
                {
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");
                }
            }

			CheckQC(lsetInfo, QC_TIMING_NO(), magInfo.sMaterialCD);//4はMDの意味 SM→EMファイル
        }

        //[SM:製品出来栄え/1ﾏｶﾞｼﾞﾝ終了]
        protected virtual void DbInput_MD_SMFile(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList) 
        {
            try
            {
				if (lsetInfo.EquipInfo.HasLoaderQRReader)
				{
					//マガジン情報取得
					magInfo = GetMagazineInfo(lsetInfo, fileLineValueList, FILE_S_MAGAZINEROW, FILE_S_MAGAZINECOL);
				}

                //SMファイルの紐付けマスタ情報(TmFILEFMT)を取得
                List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.SM), lsetInfo, magInfo.sMaterialCD);

				List<FileFmt> noChkFileFmt = new List<FileFmt>();

				if (filefmtList.Count == 0)
				{
					noChkFileFmt = FileFmt.GetData(lsetInfo, null, Convert.ToString(FileType.SM), Constant.NO_CHK_QCPARAM_NO);
				}

				if (noChkFileFmt.Count >= 1)
				{
					return;
				}

				if (filefmtList.Count == 0)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(Convert.ToString(FileType.SM)));
					throw new Exception(message);
				}

				// CRMDでファイルが出るが取得要求を落とすだけの処理の為、下記はコメントアウトで上に処理追加
				//if (filefmtList.Count == 0)
				//{
				//	//設定されていない場合、装置処理停止
				//	string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(FileType.SM));
				//	throw new Exception(message);
				//}
                foreach (FILEFMTInfo filefmtInfo in filefmtList)
                {

                    //閾値マスタ情報(TmPLM)取得
					Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                    if (plmInfo == null)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                        throw new Exception(message);
                    }

                    //必要ファイル内容取得
                    FileValueInfo fileValueInfo = GetSyringeFileValue(fileLineValueList, filefmtInfo.SearchNM, filefmtInfo.ColumnNO, plmInfo.TotalKB);
                    if (fileValueInfo.TargetStrVAL == null)
                    {
                        continue;
                    }

                    //DB登録
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                }
            }
            catch (Exception err) 
            {
                throw;
            }
        }

        //[AM:装置実行パラメータ/1マガジン毎]
        protected virtual void DbInput_MD_AMFile(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList)
        {
            if (lsetInfo.EquipInfo.HasLoaderQRReader)
            {
                //マガジン情報取得
                magInfo = GetMagazineInfo(lsetInfo, fileLineValueList, FILE_S_MAGAZINEROW, FILE_S_MAGAZINECOL);
            }

            //AMファイルの紐付けマスタ情報(TmFILEFMT)を取得
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.AM), lsetInfo, magInfo.sMaterialCD);

            List<FileFmt> noChkFileFmt = new List<FileFmt>();

            if (filefmtList.Count == 0)
            {
                noChkFileFmt = FileFmt.GetData(lsetInfo, null, Convert.ToString(FileType.AM), Constant.NO_CHK_QCPARAM_NO);
            }

            if (noChkFileFmt.Count >= 1)
            {
                return;
            }

            if (filefmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(Convert.ToString(FileType.AM)));
                throw new Exception(message);
            }

            int syringeCt = GetSyringeCount(lsetInfo);

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                //閾値マスタ情報(TmPLM)取得
                Plm plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                FileValueInfo fileValueInfo;
                if (string.IsNullOrEmpty(filefmtInfo.SearchNM) == false)
                {
                    //全シリンジ集計項目と判断し、シリンジ本数分検索文字の列の値を合体させる
                    fileValueInfo = GetAllSyringeFileValue(fileLineValueList, filefmtInfo.SearchNM, plmInfo.TotalKB, syringeCt);

                }
                else
                {
                    //必要ファイル内容取得
                    fileValueInfo = GetSyringeFileValue(fileLineValueList, filefmtInfo.ColumnNO, plmInfo.TotalKB);
                    if (fileValueInfo.TargetStrVAL == null)
                    {
                        continue;
                    }
                }

				ParamInfo paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);

				if (paramInfo.ChangeUnitVAL != "0")
				{
					fileValueInfo.TargetStrVAL = Convert.ToString(CalcChangeUnit(filefmtInfo.QCParamNO, Convert.ToDouble(fileValueInfo.TargetStrVAL)));
				}

				//DB登録
				ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
            }
        }


        private string GetMachineFolderPath(int lineCD, string plantCD)
		{
			//装置の情報取得
			List<LSETInfo> machineList = ConnectDB.GetLSETData(lineCD, null, plantCD);

			if (machineList.Count > 1)
			{
				throw new Exception(string.Format("装置のマスタ情報が複数取得されました。 ライン:{0} / 設備CD:{1}", lineCD, plantCD));
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
		private string GetLatestFilePath(string folderPath, int startIndexFromMM, ref List<ErrMessageInfo> errMessageList)
		{
			int i = 0;

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
			fileInfoList = fileInfoList.OrderBy(f => DateTime.ParseExact(f.Name.Substring(f.Name.IndexOf("MM") + startIndexFromMM, 10), "yyMMddHHmm", null)).ToList();

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

        /// <summary>
        /// SDファイル内容樹脂小フラグ列番号を取得
        /// </summary>
        /// <param name="syringeNO">シリンジNO</param>
        /// <returns>樹脂小フラグ列番号</returns>
        private static int GetSDFileSyringeColumn(int syringeNO)
        {
            int targetColumnIndex = int.MinValue;

            try
            {
                switch (syringeNO)
                {
                    case 1:
                        targetColumnIndex = FILE_SD_SYRINGE1_FG;
                        break;
                    case 2:
                        targetColumnIndex = FILE_SD_SYRINGE2_FG;
                        break;
                    case 3:
                        targetColumnIndex = FILE_SD_SYRINGE3_FG;
                        break;
                    case 4:
                        targetColumnIndex = FILE_SD_SYRINGE4_FG;
                        break;
                    case 5:
                        targetColumnIndex = FILE_SD_SYRINGE5_FG;
                        break;
                    default:
                        throw new Exception(string.Format(Constant.MessageInfo.Message_74, syringeNO));
                }

                return targetColumnIndex;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public FileValueInfo GetSyringeFileValue(string[] fileLineValueList, int valueColumnNO, string totalKB)
        {
            return GetSyringeFileValue(fileLineValueList, null, valueColumnNO, totalKB);
        }

        /// <summary>
        /// ファイルから必要データ(ｼﾘﾝｼﾞ)を取得
        /// </summary>
        /// <param name="fileLineValueList">ファイル内容</param>
        /// <param name="searchNM">検索文字列(ｼﾘﾝｼﾞNO)</param>
        /// <param name="valueColumnNO">取得列</param>
        /// <param name="totalKB">集計区分</param>
        /// <returns>必要データ情報</returns>
        public FileValueInfo GetSyringeFileValue(string[] fileLineValueList, string searchNM, int valueColumnNO, string totalKB)
        {
            FileValueInfo fileValueInfo = new FileValueInfo();
            List<double> valueList = new List<double>();

            try
            {
                for (int i = FILE_S_DATASTARTROW; i < fileLineValueList.LongLength; i++)
                {
                    if (fileLineValueList[i] == "")
                    {
                        //空白行の場合、次へ
                        continue;
                    }

                    string[] fileValue = fileLineValueList[i].Split(',');

					if (fileValue[SM_KEIKOUFG_COL_INDEX()].Trim() == FILE_S_KEIKOUFG_OFF)
                    {
                        //傾向管理しないデータの場合、次へ
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(searchNM) == false)
                    {
                        if (fileValue[FILE_S_SYRINGENO].Trim() != searchNM && searchNM != "")
                        {
                            //検索ｼﾘﾝｼﾞNO以外、全ｼﾘﾝｼﾞでない場合、次へ
                            continue;
                        }

                        if (fileValue.Length < (valueColumnNO + 1))
                        {
                            //測定値2がファイル内容に存在しない場合、次へ
                            continue;
                        }
                    }

                    if (fileValue[valueColumnNO].Trim() == FILE_ERRORVAL)
                    {
                        //測定値が異常値の場合、次へ
                        continue;
                    }

                    double measureVAL = 0;
                    if (!double.TryParse(fileValue[valueColumnNO].Trim(), out measureVAL))
                    {
                        //測定値が数値以外の場合、次へ
                        continue;
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_S_DAY] + " " + fileValue[FILE_S_TIME]));
                    valueList.Add(measureVAL);
                }

                if (valueList.Count == 0)
                {
                    //対象が存在しない場合、処理を抜ける
                    return fileValueInfo;
                }

                double rValue = 0;
                switch (totalKB)
                {
                    case "AVE":
                        rValue = calcAvg(valueList);
                        break;
                    case "SIGMA":
                        double avgVAL = calcAvg(valueList);
                        rValue = calcSigma(valueList, avgVAL);
                        break;
                    case "MODE":
                        rValue = CalcMode(valueList);
                        break;
                    case "MAX":
                        rValue = CalcMax(valueList);
                        break;
                    case "MIN":
                        rValue = CalcMin(valueList);
                        break;
                    default:
                        rValue = valueList[0];
                        break;
                }
                fileValueInfo.TargetStrVAL = Convert.ToString(rValue);

                return fileValueInfo;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        #endregion//-->モールド装置用

		#region マッピング　[LENSへ移動 ※マッピング未導入ラインだけEICSから0埋めデータを送信する]

        public void SetUnMappingDataToZF(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList) 
        {
            KLinkInfo kLinkInfo = new KLinkInfo();
            string resMsg = string.Empty;

            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
            SettingInfo settingInfoCommon = SettingInfo.GetSingleton();

            string[] sSyringe1Address = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス
            string[] sSyringe2Address = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス
            string[] sSyringe3Address = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス
            string[] sSyringe4Address = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス
            string[] sSyringe5Address = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス

            string[] sSyringe1Mapping = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス に書き込むSP区切りの文字列
            string[] sSyringe2Mapping = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス に書き込むSP区切りの文字列
            string[] sSyringe3Mapping = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス に書き込むSP区切りの文字列
            string[] sSyringe4Mapping = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス に書き込むSP区切りの文字列
            string[] sSyringe5Mapping = new string[2];//[0]前半1000アドレス,[1]後半1000アドレス に書き込むSP区切りの文字列

            try
            {
				//各シリンジの先頭アドレス取得
				resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Syringe1Address, 5, Constant.ssuffix_H);
				sSyringe1Address[0] = kLinkInfo.BytesStringToAscii(resMsg.Trim()).Replace("\0", "");
				int nWork = Convert.ToInt32(sSyringe1Address[0].Substring(2, 5)) + 1000;
				sSyringe1Address[1] = sSyringe1Address[0].Substring(0, 2) + Convert.ToString(nWork);

				resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Syringe2Address, 5, Constant.ssuffix_H);
				sSyringe2Address[0] = kLinkInfo.BytesStringToAscii(resMsg.Trim()).Replace("\0", "");
				nWork = Convert.ToInt32(sSyringe2Address[0].Substring(2, 5)) + 1000;
				sSyringe2Address[1] = sSyringe2Address[0].Substring(0, 2) + Convert.ToString(nWork);

				resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Syringe3Address, 5, Constant.ssuffix_H);
				sSyringe3Address[0] = kLinkInfo.BytesStringToAscii(resMsg.Trim()).Replace("\0", "");
				nWork = Convert.ToInt32(sSyringe3Address[0].Substring(2, 5)) + 1000;
				sSyringe3Address[1] = sSyringe3Address[0].Substring(0, 2) + Convert.ToString(nWork);

				resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Syringe4Address, 5, Constant.ssuffix_H);
				sSyringe4Address[0] = kLinkInfo.BytesStringToAscii(resMsg.Trim()).Replace("\0", "");
				nWork = Convert.ToInt32(sSyringe4Address[0].Substring(2, 5)) + 1000;
				sSyringe4Address[1] = sSyringe4Address[0].Substring(0, 2) + Convert.ToString(nWork);

				resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Syringe5Address, 5, Constant.ssuffix_H);
				sSyringe5Address[0] = kLinkInfo.BytesStringToAscii(resMsg.Trim()).Replace("\0", "");
				nWork = Convert.ToInt32(sSyringe5Address[0].Substring(2, 5)) + 1000;
				sSyringe5Address[1] = sSyringe5Address[0].Substring(0, 2) + Convert.ToString(nWork);

				//マッピング用メモリ ZF80000～87999全て0 →全て樹脂少なしで生産
				for (int i = 0; i < 2; i++)
				{
#if Debug
#else
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, sSyringe1Address[i], 0, 1000, "");
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, sSyringe2Address[i], 0, 1000, "");
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, sSyringe3Address[i], 0, 1000, "");
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, sSyringe4Address[i], 0, 1000, "");
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, sSyringe5Address[i], 0, 1000, "");
#endif
				}
			}
			catch (Exception err)
			{
#if Debug
#else
				//停止要求
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
				//マッピング不一致
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_NoMapping, 1, 1, Constant.ssuffix_U);
#endif
				throw;
			}
		}

		#endregion
		#region マッピング



		public void SetUnMappingDataToLR(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			KLinkInfo kLinkInfo = new KLinkInfo();

			try
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供処理開始(LR)"), lsetInfo.EquipmentNO);

								//外観検査機を通す必要なし(Top Viewはここに入る)→メモリクリア
				//マッピング用メモリ LR10000全て0 →全て樹脂少なしで生産
				//KLINK_SetKV_WRS(EquiInfoMD.sIPAddressNO, Constant.Common_Port, Constant.Syringe1StartAddress_F, 0, 563, Constant.ssuffix_U);
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe1StartAddress_F, 0, 960, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe1StartAddress_L, 0, 840, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe3StartAddress_F, 0, 960, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe2StartAddress_F, 0, 960, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe2StartAddress_L, 0, 840, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe3StartAddress_L, 0, 840, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe4StartAddress_F, 0, 960, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe4StartAddress_L, 0, 840, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe5StartAddress_F, 0, 960, "");
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.Syringe5StartAddress_L, 0, 840, "");

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ1書き込み アドレス:{0} 内容:全0マッピングデータ(960pcs分) ", Constant.Syringe1StartAddress_F), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ1書き込み アドレス:{0} 内容:全0マッピングデータ(840pcs分) ", Constant.Syringe1StartAddress_L), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ2書き込み アドレス:{0} 内容:全0マッピングデータ(960pcs分) ", Constant.Syringe2StartAddress_F), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ2書き込み アドレス:{0} 内容:全0マッピングデータ(840pcs分) ", Constant.Syringe2StartAddress_L), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ3書き込み アドレス:{0} 内容:全0マッピングデータ(960pcs分) ", Constant.Syringe3StartAddress_F), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ3書き込み アドレス:{0} 内容:全0マッピングデータ(840pcs分) ", Constant.Syringe3StartAddress_L), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ4書き込み アドレス:{0} 内容:全0マッピングデータ(960pcs分) ", Constant.Syringe4StartAddress_F), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ4書き込み アドレス:{0} 内容:全0マッピングデータ(840pcs分) ", Constant.Syringe4StartAddress_L), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ5書き込み アドレス:{0} 内容:全0マッピングデータ(960pcs分) ", Constant.Syringe5StartAddress_F), lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(LR) シリンジ5書き込み アドレス:{0} 内容:全0マッピングデータ(840pcs分) ", Constant.Syringe5StartAddress_L), lsetInfo.EquipmentNO);
			}
			catch (Exception err)
			{
#if Debug
#else
				//停止要求
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
				//マッピング不一致
				kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_NoMapping, 1, 1, Constant.ssuffix_U);

				#endif
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.FATAL, string.Format("LRマッピング照合", mappingFileInfo.Name), lsetInfo.EquipmentNO);
				throw;
			}
		}

        #endregion


		public class SDFile
		{
			public const string FILE_PREFIX = "SD";

			public FileInfo Info { get; set; }
			public string TypeCd { get; set; }
			public string LotNo { get; set; }
			public int ProcNo { get; set; }

			public SDFile(FileInfo sdFile)
			{
				this.Info = sdFile;
				getLotFromFileName();
			}

			private void getLotFromFileName()
			{
				if (SDFile.IsLotFromFileName(this.Info) == false)
				{
					throw new ApplicationException(
						string.Format("SDファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
				}

				string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
				this.LotNo = nameChar[2].Trim();
				this.TypeCd = nameChar[3].Trim();
				this.ProcNo = int.Parse(nameChar[4].Trim());
			}

			public static bool IsLotFromFileName(FileInfo file)
			{
				// ファイル名定義(log000_SD***_Lot_Type_Proc.csv)を想定して要素数で付与済か判断する 
				string[] nameChar = Path.GetFileNameWithoutExtension(file.FullName).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}

			public static bool IsUnknownFile(FileInfo sdFile)
			{
				string[] nameChar = Path.GetFileNameWithoutExtension(sdFile.FullName).Split('_');

				if (nameChar[LOTINDEX_FILENAME].ToUpper() == UNKNOWN_LOT_FILENAME)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
    }
}
