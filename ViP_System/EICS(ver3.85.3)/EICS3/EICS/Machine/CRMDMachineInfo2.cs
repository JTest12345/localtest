using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Machine
{
    //COB高生産性に導入されている色調補正MD
    class CRMDMachineInfo2 : MDMachineInfo
    {
        protected override int PR_KEIKOUFG_COL_INDEX() { return 5; }
        protected override int OR_KEIKOUFG_COL_INDEX() { return 4; }
        protected override int SM_KEIKOUFG_COL_INDEX() { return 5; }
        protected override int AM_KEIKOUFG_COL_INDEX() { return 14; }
        protected override int EM_KEIKOUFG_COL_INDEX() { return 4; }
        protected override int EF_KEIKOUFG_COL_INDEX() { return 9; }

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

                #region 装置運転開始時のパラメータ管理

                // QCIL.xml内の設備毎のタグ「PlcParamCheckFG」がTrueの時のみ、処理を行う。
                if (settingInfoPerLine.GetFullParameterFG(lsetInfo.EquipmentNO))
                {
                    CheckPlcParameter(lsetInfo, ref errorMessageList);
                }

                #endregion

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

                #region [ファイル処理]

                // 開始時パラメータファイルの傾向監視
                Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, true);
                foreach (KeyValuePair<string, string> prefix in prefixList)
                {
                    FileProcess(lsetInfo, prefix.Key, prefix.Value, true, true);
                }

				prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, false);

                // 完了時出来栄えファイルの傾向監視
                if (settingInfoPerLine.GetForciblyEnableSequencialFileProcessFg(lsetInfo.EquipmentNO) == false && isCompleteEndFile(lsetInfo))
                {
                    // 先に全てのファイル取得要求をOFF (色調バラツキにつながる為、装置の動作を極力止めない)
                    
                    foreach (KeyValuePair<string, string> prefix in prefixList)
                    {
                        resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, 0, 1, Constant.ssuffix_U);
                        if (resMsg == "Error")
                        {
                            base.machineStatus = Constant.MachineStatus.Stop;
                            return;
                        }
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} □MD欠損調査 {1}:{2}取得要求立ち下げ", lsetInfo.EquipmentNO, prefix.Key, prefix.Value), lsetInfo.EquipmentNO);
                    }

                    foreach (KeyValuePair<string, string> prefix in prefixList)
                    {
                        FileProcess(lsetInfo, prefix.Key, prefix.Value, false, false);
                    }
                }
				else if(settingInfoPerLine.GetForciblyEnableSequencialFileProcessFg(lsetInfo.EquipmentNO) == true)
				{
					foreach (KeyValuePair<string, string> prefix in prefixList)
					{
						FileProcess(lsetInfo, prefix.Key, prefix.Value, false, true);
					}
				}

                #endregion//--> [ファイル処理]

                if (SendHeartBeat(lsetInfo, 0, 11) == false)
                {
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ns != null) { ns.Close(); }
                if (tcp != null) { tcp.Close(); }
            }
        }

        protected void FileProcess(LSETInfo lsetInfo, string prefixNm, string prefixAddr, bool startUpFG, bool isUseMacRequest)
        {
            AlertLog alertLog = AlertLog.GetInstance();

            string resMsg = string.Empty; string logMsg = string.Empty;
            KLinkInfo kLinkInfo = new KLinkInfo();
            SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            MagInfo magInfo = new MagInfo();

            bool waitForRenameByArmsFg = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);

            if (isUseMacRequest || settingInfoPerLine.GetForciblyEnableSequencialFileProcessFg(lsetInfo.EquipmentNO))
            {
                //ファイル取得要求
                resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    logMsg = string.Format("装置からのﾌｧｲﾙ取得要求の確認時に異常発生 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
                    alertLog.logMessageQue.Enqueue(logMsg);

                    base.machineStatus = Constant.MachineStatus.Stop;
                    return;
                }
                if (Convert.ToInt16(resMsg.ToString().Trim()) != 1)//ONの場合
                {
                    return;
                }

                logMsg = string.Format("ﾌｧｲﾙ取得要求確認 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg, lsetInfo.EquipmentNO);
            }

            if (startUpFG == false && lsetInfo.EquipInfo.WaitForRenameByArmsFG)
            {
                FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
                int loopCt = 0;

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
            else
            {
                magInfo.sMaterialCD = lsetInfo.TypeCD;
                magInfo.sMagazineNO = string.Empty;
                magInfo.sNascaLotNO = string.Empty;
            }

            if ((prefixNm == SDFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
                || (waitForRenameByArmsFg && startUpFG == false))
            {
                FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
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

            if (isUseMacRequest || settingInfoPerLine.GetForciblyEnableSequencialFileProcessFg(lsetInfo.EquipmentNO))
            {
                //ファイル取得要求 立ち下げ
                resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, 0, 1, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    base.machineStatus = Constant.MachineStatus.Stop;
                    return;
                }
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} □MD欠損調査 {1}:{2}取得要求立ち下げ", lsetInfo.EquipmentNO, prefixNm, prefixAddr), lsetInfo.EquipmentNO);
            }
            
        }

        /// <summary>
        /// 全ての出来栄えファイル(SM, AM, EM)の出力が完了しているか確認
        /// </summary>
        /// <returns></returns>
        private bool isCompleteEndFile(LSETInfo lset)
        {
            KLinkInfo kLinkInfo = new KLinkInfo();

            Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lset, 0, false);
            foreach (KeyValuePair<string, string> prefix in prefixList)
            {
                string flag = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lset.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, Constant.ssuffix_U);
                if (int.Parse(flag) != KLinkInfo.BIT_ON)
                {
                    return false;
                }
            }

            return true;
        }

		protected override string getSyringeParamSearchNmCondition(string searchNm, int syringe)
		{
			string retv = string.Empty;
			try
			{
				retv = string.Format(searchNm, syringe);
			}
			catch(Exception ex)
			{
				throw new ApplicationException($"searchNm:[{searchNm}] syringe:[{syringe}] で string.Format(searchNm, syringe)での文字生成が行えませんでした。マスタ担当者に連絡して下さい。");
			}
			return retv;
		}
	}
}
