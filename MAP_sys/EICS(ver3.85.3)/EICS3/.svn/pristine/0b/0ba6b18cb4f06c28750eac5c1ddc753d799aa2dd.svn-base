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

namespace EICS
{
    /// <summary>
    /// 遠心沈降機処理
    /// </summary>
    public class ECKMachineInfo : MachineBase
    {
		/// <summary>遠心沈降機装置名</summary>
		public const string ASSETS_NM = "遠心沈降機";
        private TcpClient tcp = null;
        private NetworkStream ns = null;
        /// <summary>
        /// ファイルチェック 
        /// </summary>
        /// <param name="mFile"></param>
        /// <param name="lsetInfo"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {

			KLinkInfo kLinkInfo = new KLinkInfo();

			try
			{
				CheckDirectory(lsetInfo);

				base.machineStatus = Constant.MachineStatus.Runtime;

#if Debug
            lsetInfo.InputFolderNM = @"C:\QCIL\data\ECK\";
            //TcpClient tcp = null;
            //NetworkStream ns = null;
            ////対象ファイル種類を取得
            //Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0);
            //foreach (KeyValuePair<string, string> prefix in prefixList)
            //{
            //    FileDistribution(ref tcp, ref ns, lsetInfo, prefix.Key, ref base.errorMessageList);
            //}
#else
				

                //「受付準備OKをON」
                string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 1, 11, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    F01_MachineWatch.spMachine.Play();
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ECK上位リンク不可能");
                    base.machineStatus = Constant.MachineStatus.Stop;
                }
                System.Threading.Thread.Sleep(50);//0.1秒ready信号をON

                #region [ファイル処理]
                ////サーバーが落ちている時は、ファイル処理せずに取得要求を立ち下げる=ファイルは来るが処理せず、装置は動かす。
                //if (fServerNG == true)
                //{
                //PRファイル取得要求 立ち下げ
                //resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_PR, 0, 1, Constant.ssuffix_U);
                ////ORファイル取得要求 立ち下げ
                //resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_OR, 0, 1, Constant.ssuffix_U);
                ////AMファイル取得要求 立ち下げ
                //resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_AM, 0, 1, Constant.ssuffix_U);
                ////EMファイル取得要求 立ち下げ
                //resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_EM, 0, 1, Constant.ssuffix_U);
                ////「受付準備OKをOFF」
                //resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                //return;
                //}

                //対象ファイル種類を取得
                Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0);
                foreach (KeyValuePair<string, string> prefix in prefixList)
                {
                    //ファイル取得要求
                    resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, Constant.ssuffix_U);
                    if (resMsg == "Error")
                    {
                        F01_MachineWatch.spMachine.Play();
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ECK上位リンク不可能");
						base.machineStatus = Constant.MachineStatus.Stop;
                        return;
                    }
                    if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
                    {

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, string.Format("ファイル取得要求確認、受付準備OK立ち下げ信号送信：設備：{0} / アドレス：{1} / {2} ファイル"
								, lsetInfo.EquipmentCD, Constant.TRG_Send_Restarting, prefix.Key.ToString()));

                        //「受付準備OKをOFF」
                        resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                        if (resMsg == "Error")
                        {
                            F01_MachineWatch.spMachine.Play();
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ECK上位リンク不可能");
							base.machineStatus = Constant.MachineStatus.Stop;
                            return;
                        }

                        //多品種流動対応 装置設定画面で型番を選ばず開始する仕様
                        if (string.IsNullOrEmpty(lsetInfo.TypeCD))
                        {
                            Arms.Order order = Arms.Order.GetLastOrder(lsetInfo.InlineCD, Arms.Machine.GetMachine(lsetInfo.InlineCD, lsetInfo.EquipmentNO).MacNo);
                            if (order == null)
                            {
                                throw new ApplicationException(string.Format("対象設備の製造実績が存在しなかった為、製品型番が取得できませんでした。設備番号:{0}", lsetInfo.EquipmentNO));
                            }
                            Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(lsetInfo.InlineCD, order.LotNo);
                            lsetInfo.TypeCD = lot.TypeCd;
                        }

                        //ファイルの情報を取得する
						FileDistribution(ref tcp, ref ns, prefix.Value, lsetInfo, prefix.Key, ref base.errorMessageList);

						if (IsStopTargetFileKind(prefix.Key))
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, string.Format("傾向管理異常の為、装置停止要求送信：設備：{0} / アドレス：{1} / {2} ファイル"
								, lsetInfo.EquipmentCD, Constant.TRG_Send_Runnnin, prefix.Key.ToString()));

							//ファイル処理で異常判定があれば装置停止要求
							kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, string.Format("ファイル取得要求 立ち下げ信号送信：設備：{0} / アドレス：{1} / {2} ファイル"
								, lsetInfo.EquipmentCD, prefix.Value, prefix.Key.ToString()));

                        //ファイル取得要求 立ち下げ
                        resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, 0, 1, Constant.ssuffix_U);

                        if (resMsg == "Error")
                        {
                            F01_MachineWatch.spMachine.Play();
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ECK上位リンク不可能");
							base.machineStatus = Constant.MachineStatus.Stop;
                            return;
                        }
                    }
                }

                #region ループにしたから没

                ////ORファイル取得要求
                //resMsg = Com.KLINK_GetKV_RD(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_OR, Constant.ssuffix_U);
                //if (resMsg == "Error")
                //{
                //    xSP.Play();
                //    Log.Logger.Info("ECK上位リンク不可能");
                //    return;//終了
                //}
                //if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
                //{
                //    //「受付準備OKをOFF」
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //    //ORファイルの情報を取得する
                //    Com.FileDistribution(ref tcp, ref ns, EquiInfo, "OR");

                //    //ORファイル取得要求 立ち下げ
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_OR, 0, 1, Constant.ssuffix_U);

                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //}
                ////AMファイル取得要求
                //resMsg = Com.KLINK_GetKV_RD(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_AM, Constant.ssuffix_U);
                //if (resMsg == "Error")
                //{
                //    xSP.Play();
                //    Log.Logger.Info("ECK上位リンク不可能");
                //    return;//終了
                //}
                //if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
                //{
                //    //「受付準備OKをOFF」
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //    //AMファイルの情報を取得する
                //    Com.FileDistribution(ref tcp, ref ns, EquiInfo, "AM");

                //    //AMファイル取得要求 立ち下げ
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_AM, 0, 1, Constant.ssuffix_U);

                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //}
                ////EMファイル取得要求
                //resMsg = Com.KLINK_GetKV_RD(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_EM, Constant.ssuffix_U);
                //if (resMsg == "Error")
                //{
                //    xSP.Play();
                //    Log.Logger.Info("ECK上位リンク不可能");
                //    return;//終了
                //}
                //if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
                //{
                //    //「受付準備OKをOFF」
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //    //EMファイルの情報を取得する
                //    Com.FileDistribution(ref tcp, ref ns, EquiInfo, "EM");

                //    //EMファイル取得要求 立ち下げ
                //    resMsg = Com.KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfo.sIPAddressNO, Constant.Common_Port, Constant.TRG_Res_ECK_EM, 0, 1, Constant.ssuffix_U);
                //    if (resMsg == "Error")
                //    {
                //        xSP.Play();
                //        Log.Logger.Info("ECK上位リンク不可能");
                //        return;//終了
                //    }
                //}

                #endregion

                #endregion//--> [ファイル処理]

                //「受付準備OKをOFF」
                resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    F01_MachineWatch.spMachine.Play();
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ECK上位リンク不可能");
					base.machineStatus = Constant.MachineStatus.Stop;
                }

#endif
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

        //「装置情報」と「どのログファイルを処理すれば良いか」を入れると、装置ログをデータベース登録してくれる。
        //異常があれば、true。問題なければ、falseを返す。
		private void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string sTargetFile, ref List<ErrMessageInfo> errMessageList)
        {
			IsErrorOR = false;
			IsErrorPR = false;

            string sfname = "";
            string sWork = "";
            string sFileType = "";
            string[] textArray = new string[] { };

            //string[] sortedCreateTime = new string[] { };
            string sMessage = "";
			//List<string> fileList = Common.GetFiles(lsetInfo.InputFolderNM, sTargetFile + ".*");
			List<string> fileList = GetMachineLogPathList(lsetInfo, ref tcp, ref ns, fileGetReqFgAddr, lsetInfo.InputFolderNM, sTargetFile + ".*");

            int nSameTargetFileNum = fileList.Count;
            string[] sortedCreateTime = new string[nSameTargetFileNum];

			////<--SDカード壊れ対策 Y.Matsushima 2011/02/14
			////装置からファイル取得指示が来たのにそのターゲットファイルが無い場合、エラー出力
			//if ((nSameTargetFileNum < 1))
			//{
			//    sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "号機" + "/出力ファイル=" + sTargetFile + "ファイルが装置側から中間サーバーに転送されませんでした\r\n\r\n"
			//        + "BlackJumboDogが起動していない場合→直ちにBlackJumboDogを起動して下さい。\r\n"
			//        + "BlackJumboDogが起動している場合  →傾向管理ファイル転送ミス\r\n"
			//        + " 装置のSDカードを交換してください。\r\n\r\n"
			//        + " 設備保全担当者へ連絡してください。";

			//    ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
			//    errMessageList.Add(errMessageInfo);
			//    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
			//    return;
			//}
            //-->SDカード壊れ対策 Y.Matsushima 2011/02/14

            //「LogファイルデータをDB登録
			List<string> mFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + sTargetFile);
            //foreach (string swfname in System.IO.Directory.GetFiles(sInputFolder))
            foreach (string file in mFileList)
            {
                FileInfo fileInfo = new FileInfo(file);

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

                //ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
                switch (sFileType)
                {
                    case "PR":
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]ECK:PR File");
                        DbInput_ECK_PRFile(ref tcp, ref ns, lsetInfo, textArray, ref errMessageList);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]ECK:PR File");
                        break;
                    case "OR":
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]ECK:OR File");
                        DbInput_ECK_ORFile(ref tcp, ref ns, lsetInfo, textArray, ref errMessageList);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]ECK:OR File");
                        break;
                    case "AM":
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]ECK:AM File");
                        DbInput_ECK_AMFile(ref tcp, ref ns, lsetInfo, textArray, ref errMessageList);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]ECK:AM File");
                        break;
                    case "EM":
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]ECK:EM File");
                        DbInput_ECK_EMFile(ref tcp, ref ns, lsetInfo, textArray, ref errMessageList);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]ECK:EM File");
                        break;
                }

                //保管場所へ移動
                MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, "", "");
            }
        }

        #region 各ファイル処理
        //[PR:装置設定ﾊﾟﾗﾒｰﾀｰ/ｽﾀｰﾄ直前] -->ﾌﾟﾛｸﾞﾗﾑ名のみ
        private void DbInput_ECK_PRFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            string[] recordArray = new string[] { };

            //管理項目事にLOGに登録
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("PR", lsetInfo, lsetInfo.TypeCD);
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

                    int nflg = Convert.ToInt32(recordArray[5]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					string magLotStr = recordArray[3].Trim().ToUpper();//ﾏｶﾞｼﾞﾝNo/LotNo取得

					if (magLotStr.Contains(Constant.DUMMY_MAG_CONTAIN_STR))
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/ECK:PR File ダミーマガジンの為、処理なし");
						return;
					}

                    //高効率以外
					if((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = magLotStr;
                        fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち
							//ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
							ArmsLotInfo rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
								lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, true);

                            if (rtnArmsLotInfo == null)
                            {
								//自動機は遠心沈降中はマガジンNOが「*****_E」になる為、再度E付きでARMSに問い合わせ
								rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
									lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}_E", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, true);
							}

							if (rtnArmsLotInfo == null)
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = null;
								MagInfo.sMaterialCD = lsetInfo.TypeCD;
							}
							else
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
								MagInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
							}
                            
                            //-->Package 古川さん待ち   

                            fGetMagNo = true;//初回のみ実行
                        }
                    }
                    else//人搬送の場合
                    {
						fileValueInfo.LotNO = magLotStr;
                        fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

                    fileValueInfo.TargetStrVAL = Convert.ToString(recordArray[filefmtInfo.ColumnNO]).Trim();
                    fileValueInfo.TargetStrVAL = fileValueInfo.TargetStrVAL.Replace("\"", "").ToUpper();//余計な文字削除

                    //管理項目事にDB登録
					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.sStartBefore);
                    
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);


					int errCount = errMessageList.Count;

					ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);

					if (errMessageList.Count > errCount)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, string.Format("傾向管理判定：異常 設備：{0}", lsetInfo.EquipmentCD));

						IsErrorPR = true;
					}

					//if (plmInfo != null)
					//{
					//    if (ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList))
					//    {
					//        //問題有の為、装置停止
					//        //KLINK_SetKV_WRS(ref tcp, ref ns, EquiInfoMD.sIPAddressNO, Constant.Common_Port, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
					//    }
					//}
					//else
					//{
					//    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "");
					//}
                }
            }
        }
        //[OR:装置設定ﾊﾟﾗﾒｰﾀｰ/直前]
        private void DbInput_ECK_ORFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            //管理項目事にLOGに登録
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("OR", lsetInfo, lsetInfo.TypeCD);
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

                    int nflg = Convert.ToInt32(recordArray[5]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					string magLotStr = recordArray[3].Trim().ToUpper();//ﾏｶﾞｼﾞﾝNo/LotNo取得

					if (magLotStr.Contains(Constant.DUMMY_MAG_CONTAIN_STR))
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/ECK:OR File ダミーマガジンの為、処理なし");
						return;
					}

                    //自動化 or ハイブリッドラインの場合
                    if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = magLotStr;
						//fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                        fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							//ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
							ArmsLotInfo rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
								lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, true);

							if (rtnArmsLotInfo == null)
							{
								//自動機は遠心沈降中はマガジンNOが「*****_E」になる為、再度E付きでARMSに問い合わせ
								//rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, string.Format("{0}_E", fileValueInfo.MagazineNO));
								rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
									lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}_E", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, true);
							}

							if (rtnArmsLotInfo == null)
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = null;
								MagInfo.sMaterialCD = lsetInfo.TypeCD;
							}
							else
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
								MagInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
							}
                            
                            //-->Package 古川さん待ち   

                            fGetMagNo = true;//初回のみ実行
                        }
                    }
                    else//アウト or 人搬送ラインの場合
                    {
						fileValueInfo.LotNO = magLotStr;
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
                        fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

                    //管理項目事にDB登録
					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.sStartBefore);
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);


					int errCount = errMessageList.Count;

					ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);

					if (errMessageList.Count > errCount)
					{
						IsErrorOR = true;
					}
					
					//if (plmInfo != null)
					//{
					//    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
					//}
					//else
					//{
					//    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");
					//}
                }
            }
        }
        //[AM:装置実行ﾊﾟﾗﾒｰﾀｰ/ﾏｶﾞｼﾞﾝ終了]
        private void DbInput_ECK_AMFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            //管理項目事にLOGに登録
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("AM", lsetInfo, lsetInfo.TypeCD);
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

                    int nflg = Convert.ToInt32(recordArray[5]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					string magLotStr = recordArray[3].Trim().ToUpper();//ﾏｶﾞｼﾞﾝNo/LotNo取得

					if (magLotStr.Contains(Constant.DUMMY_MAG_CONTAIN_STR))
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/ECK:AM File ダミーマガジンの為、処理なし");
						return;
					}

					//自動化 or ハイブリッドラインの場合
					if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
                        fileValueInfo.MagazineNO = magLotStr;
						//fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                        fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							//ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
							ArmsLotInfo rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
								lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, false);

							if (rtnArmsLotInfo == null)
							{
								//自動機は遠心沈降中はマガジンNOが「*****_E」になる為、再度E付きでARMSに問い合わせ
								//rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, string.Format("{0}_E", fileValueInfo.MagazineNO));
								rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
									lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}_E", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, false);
							}

							if (rtnArmsLotInfo == null)
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = null;
								MagInfo.sMaterialCD = lsetInfo.TypeCD;
							}
							else
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
								MagInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
							}
   
                            //-->Package 古川さん待ち   

                            fGetMagNo = true;//初回のみ実行
                        }
                    }
					else//アウト or 人搬送ラインの場合
                    {
						fileValueInfo.LotNO = magLotStr;
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
                        fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

                    //DB登録

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                    if (plmInfo != null)
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                    }
                    else
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");
                    }
                }
            }
        }

        //[EM:ｴﾗｰﾛｸﾞ/ﾏｶﾞｼﾞﾝ終了]
        private void DbInput_ECK_EMFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            //管理項目事にLOGに登録
            List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("EM", lsetInfo, lsetInfo.TypeCD);
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

                    int nflg = Convert.ToInt32(recordArray[5]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                    if (nflg == 0)
                    {
                        continue;//何もせずに次へ
                    }

                    fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					string magLotStr = recordArray[3].Trim().ToUpper();//ﾏｶﾞｼﾞﾝNo/LotNo取得

					if (magLotStr.Contains(Constant.DUMMY_MAG_CONTAIN_STR))
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/ECK:EM File ダミーマガジンの為、処理なし");
						return;
					}

					//自動化 or ハイブリッドラインの場合
					if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = magLotStr;
						//fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                        fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							//ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
							ArmsLotInfo rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
								lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, false);

							if (rtnArmsLotInfo == null)
							{
								//自動機は遠心沈降中はマガジンNOが「*****_E」になる為、再度E付きでARMSに問い合わせ
								//rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, string.Format("{0}_E", fileValueInfo.MagazineNO));
								rtnArmsLotInfo = ConnectDB.GetLotNo_EckMag(
									lsetInfo.InlineCD, lsetInfo.EquipmentNO, string.Format("{0}_E", fileValueInfo.MagazineNO), fileValueInfo.MeasureDT, false);
							}

							if (rtnArmsLotInfo == null)
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = null;
								MagInfo.sMaterialCD = lsetInfo.TypeCD;
							}
							else
							{
								MagInfo.sMagazineNO = fileValueInfo.MagazineNO;
								MagInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
								MagInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
							}
                            
                            //-->Package 古川さん待ち   

                            fGetMagNo = true;//初回のみ実行
                        }
                    }
                    else//人搬送の場合
                    {
						fileValueInfo.LotNO = magLotStr;
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
                        fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

                    //DB登録

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                    if (plmInfo != null)
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                    }
                    else
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");
                    }
                }
            }
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]ECK:CheckQC");
            CheckQC(lsetInfo, 8, MagInfo.sMaterialCD);//8はECKの意味 EMファイル OR→PR→AM→EMの順
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]ECK:CheckQC");
        }
        #endregion

		#region HSMS

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		#endregion

    }
}
