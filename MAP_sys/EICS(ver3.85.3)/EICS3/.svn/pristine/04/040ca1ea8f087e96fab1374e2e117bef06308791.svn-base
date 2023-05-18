using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EICS.Machine;
using System.Net.Sockets;
using EICS.Database;
using EICS.Structure;

namespace EICS
{
    /// <summary>
    /// ダイサー処理
    /// </summary>
    public class DCMachineInfo : MachineBase
    {
		/// <summary>ダイサー装置名</summary>
		public const string ASSETS_NM = "ﾀﾞｲｻｰ";
		private const int FILE_MEASUREDATE_DATE = 1;
		private const int FILE_MEASUREDATE_TIME = 2;


        /// <summary>
        /// ファイルチェック 
        /// </summary>
        /// <param name="mFile"></param>
        /// <param name="lsetInfo"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
			CheckDirectory(lsetInfo);

			base.machineStatus = Constant.MachineStatus.Runtime;

#if Debug
            lsetInfo.InputFolderNM = @"C:\QCIL\data\DC\";
#endif

            if (Directory.Exists(lsetInfo.InputFolderNM) == false)
            {
                string sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "/" + lsetInfo.InputFolderNM + "が見つかりません。";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                //<--Start 1分毎にﾀﾞｲﾎﾞﾝﾀﾞｰ装置接続 2010/10/13 Y.Matsushima 
                KLinkInfo.Kickbatch(lsetInfo);
                //-->End 1分毎にﾀﾞｲﾎﾞﾝﾀﾞｰ装置接続 2010/10/13 Y.Matsushima 

                System.Threading.Thread.Sleep(60000);//1分毎に接続確認 
				base.machineStatus = Constant.MachineStatus.Runtime;
                return;
            }

            try
            {
                //マガジンファイルを発見した時(1マガジン単位ファイルが1つ以上あった場合)
				List<string> lFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + "LM");
                if (lFileList.Count != 0)
                {
                    foreach (string lFile in lFileList)
                    {
                        //処理フォルダに最新ファイルを移動
                        MoveRunFile(lFile);

                        //マガジンタイミング処理
                        CheckDCMagazineTiming(lsetInfo, ref base.errorMessageList);
                    }
                }
                //スタートファイルを発見した時
                else
                {
                    //スタートタイミング処理
                    CheckDCStartTiming(lsetInfo, ref base.errorMessageList);
                }
            }
            catch (Exception ex)
            {
                string sMsg = lsetInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/★Mode_DC1停止<--Start";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
            }
        }

        /// <summary>
        /// マガジンタイミング処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        public void CheckDCMagazineTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList) 
        {
            //[E:errorlog]   / [H:Pre bond全データ]   / [I:Post bond全データ] / [J:Pre bondのXyθエラー] / [K:Post bondのXyθエラー]
            //[L:life Count] / [O:装置設定パラメータ] / [P:Package data name] / [W:Wafer Log data]
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            string runFolderPath = Path.Combine(lsetInfo.InputFolderNM, "Run");

            string dtFileStamp = KLinkInfo.GetFileStampDT2(runFolderPath, "LM");//1マガジン単位ファイルのファイルスタンプ取得
            dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//公開API(装置・時間)の時間として使用(ファイルスタンプ-5分)
            string sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");

            //<--Package 古川さん待ち 
            //公開APIでLot情報取得
            MagInfo magInfo = new MagInfo();
            string sMagazineNo = KLinkInfo.GetDCMagazineNo(runFolderPath, "LM");

            //<--Package 古川さん待ち
            //公開APIでLot情報取得
            ArmsLotInfo rtnArmsLotInfo = GetDCLotNo(lsetInfo.EquipmentNO, sMagazineNo);
            if (rtnArmsLotInfo == null)
            {
                magInfo.sMagazineNO = "";
                magInfo.sNascaLotNO = null;
                magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
            }
            else//Lotが複数Hitした場合
            {

                magInfo.sMagazineNO = "";
                magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;  //最初のLot
                magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD; //最初のLot
            }

            if (magInfo.sNascaLotNO == null)
            {
                Lott.SetTnLOTT(lsetInfo, "不明");
            }
            else
            {
				Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
            }
            //-->Package 古川さん待ち

            string[] textArray = new string[] { };

			List<string> fileList = MachineFile.GetPathList(runFolderPath, ".*");
            foreach (string file in fileList)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);

                    //#if Debug   //立ち上げ用:立ち上げ時はLogファイルは貴重なデータとなる為、削除せずに保存しておく。
                    string spath1 = "", spath2 = "", sCreateFileYM = "";
                    //ﾌｧｲﾙ更新日時を文字列
                    sCreateFileYM = Convert.ToString(File.GetLastWriteTime(fileInfo.FullName));
                    sCreateFileYM = sCreateFileYM.Substring(0, 4) + sCreateFileYM.Substring(5, 2);//yyyymm

                    if (magInfo.sNascaLotNO == null)
                    {
                        spath1 = lsetInfo.InputFolderNM + sCreateFileYM + "\\unchain\\" + sFileStamp;
                    }
                    else
                    {
                        spath1 = lsetInfo.InputFolderNM + sCreateFileYM + "\\Bind\\" + magInfo.sNascaLotNO;
                    }

                    spath2 = spath1 + "\\" + fileInfo.Name;

                    //登録済みﾌｧｲﾙは削除して次へ
                    if (File.Exists(spath2))
                    {
                        File.Delete(fileInfo.FullName);
                        continue;
                    }

                    using (StreamReader textFile = new StreamReader(fileInfo.FullName, System.Text.Encoding.Default))
                    {
                        textArray = textFile.ReadToEnd().Split('\n');
                        textFile.Close();
                    }

                    string sFileType = fileInfo.Name.Substring(0, 1);
                    string sFileType2 = GetMachineFileChar(fileInfo.FullName, lsetInfo.AssetsNM);
                    switch (sFileType2)
                    {
                        case "LM":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]DC:LM File");
                            this.DbInput_DC_LMFile(lsetInfo, magInfo, textArray);
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]DC:LM File");
                            break;
                        case "OR"://スタートタイミングでエラー出力済ファイル
                            if (sFileType == "_")
                            {
                                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]DC:_OR File");
                                this.DbInput_DC_ORFile(lsetInfo, magInfo, textArray, Constant.nMagazineTimming, ref errMessageList);//エラー出力なし・データベース登録あり
                                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]DC:_OR File");
                            }
                            break;
                        //E,Kﾌｧｲﾙ→不要
                        //Wﾌｧｲﾙ→InlineでｼｰｹﾝｻからNASCA登録する為、何もせずファイル削除する
                    }

                    //保管場所へ移動
                    MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, sFileStamp);

                }
                catch (Exception ex)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
					
					string sMessage;

					if (Directory.Exists(lsetInfo.InputFolderNM) == false)
					{
						sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "/" + lsetInfo.InputFolderNM + "が見つかりません。";
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					}
                    

                    //ファイルが20個以上溜まった場合
                    if (fileList.Count > 20)
                    {
                        sMessage = string.Format(Constant.MessageInfo.Message_25, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
                        //sMessage = "ファイルが20個以上溜まっています。状況を確認下さい。\r\n\r\n"
                        //        + "[インライン番号]" + Constant.inlineCD + "\r\n"
                        //        + "[設備番号]" + lsetInfo.EquipmentNO + " [設備名]" + lsetInfo.AssetsNM;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    }

                    //バッチファイル(ネットワークドライブの割り当て)実行
                    KLinkInfo.Kickbatch(lsetInfo);

                    return;
                }
            }
        }

        /// <summary>
        /// スタートタイミング処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        public void CheckDCStartTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList) 
        {
            string[] textArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            List<string> sFileList = GetMachineStartFile(lsetInfo.InputFolderNM, lsetInfo, ref errMessageList);
            if (sFileList.Count == 0) 
            {
                return;
            }

            MagInfo magInfo = new MagInfo();
            magInfo.sMagazineNO = "";
            magInfo.sNascaLotNO = null;
            magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

            foreach (string sFile in sFileList)
            {
                try
                {
                    FileInfo sFileInfo = new FileInfo(sFile);
                    using (StreamReader textFile = new StreamReader(sFileInfo.FullName, System.Text.Encoding.Default))
                    {
                        textArray = textFile.ReadToEnd().Split('\n');
                        textFile.Close();
                    }

                    //ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
                    
                    string sFileType = GetMachineFileChar(sFileInfo.FullName, lsetInfo.AssetsNM);
                    switch (sFileType)
                    {
                        case "OR":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]DC:O File");
                            DbInput_DC_ORFile(lsetInfo, magInfo, textArray, Constant.nStartTimming, ref errMessageList);//エラー出力あり・データベース登録なし
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]DC:O File");
                            break;
                    }

                    string renameFilePath = lsetInfo.InputFolderNM + "_" + sFileInfo.Name;//スタートタイミングで処理済(エラー出力あり・DB登録なし)となったファイル
                    if (File.Exists(renameFilePath) == false)
                    {
                        File.Move(sFileInfo.FullName, renameFilePath);//リネーム例：スタートタイミングで未処理「O*****.***」→スタートタイミングで処理済「_O*****.***」
                    }
                    else//既にある場合、delete
                    {
                        File.Delete(sFileInfo.FullName);
                    }
                }
                catch (Exception ex)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);

					string sMessage;

					if (Directory.Exists(lsetInfo.InputFolderNM) == false)
					{
						sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "/" + lsetInfo.InputFolderNM + "が見つかりません。";
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					}

                    //ファイルが20個以上溜まった場合
                    if (sFileList.Count > 20)
                    {
                        sMessage = string.Format(Constant.MessageInfo.Message_25, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
                        //sMessage = "ファイルが20個以上溜まっています。状況を確認下さい。\r\n\r\n"
                        //        + "[インライン番号]" + Constant.inlineCD + "\r\n"
                        //        + "[設備番号]" + lsetInfo.EquipmentNO + " [設備名]" + lsetInfo.AssetsNM;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    }

                    //バッチファイル(ネットワークドライブの割り当て)実行
                    KLinkInfo.Kickbatch(lsetInfo);

                    return;
                }
            }
        }

        #region 各ファイル処理

        /// <summary>
        /// ORファイル
        /// </summary>
        /// <param name="EquiInfo"></param>
        /// <param name="MagInfoDB"></param>
        /// <param name="textArray"></param>
        /// <param name="nTimmingMode"></param>
        public void DbInput_DC_ORFile(LSETInfo lsetInfo, MagInfo MagInfo, string[] textArray, int nTimmingMode, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();

            string[] recordArray = new string[] { };

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("OR", lsetInfo, MagInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                FileValueInfo fileValueInfo = new FileValueInfo();
                int nRowCnt = 0;
                foreach (string srecord in textArray)
                {
                    nRowCnt += 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                    {
                        continue;
                    }

                    if (recordArray[0] != "")
                    {
                        fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));
                    }
                    else
                    {
                        break;
                    }
                    fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim().ToUpper();   //Program

                }

				paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.sStartBefore);
                
				//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
				plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                if (plmInfo != null)
                {
                    if (nTimmingMode == Constant.nStartTimming)
                    {
                        OutputErr(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, ref errMessageList);
                    }
                    else
                    {
                        ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT);
                    }
                }
                else
                {
                    if (nTimmingMode == Constant.nMagazineTimming)
                    {
                        ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");//ｴﾗｰﾒｯｾｰｼﾞなし。ﾃﾞｰﾀ保管のみ
                    }
                }
            }
        }

        /// <summary>
        /// LMファイル
        /// </summary>
        /// <param name="EquiInfo"></param>
        /// <param name="MagInfoDB"></param>
        /// <param name="textArray"></param>
        /// <param name="nTimmingMode"></param>
        public void DbInput_DC_LMFile(LSETInfo lsetInfo, MagInfo MagInfo, string[] textArray)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            //MagInfo MagInfoDB = new MagInfo();

            //string sWearZ1 = "";
            //string sWearZ2 = "";
            //string sDamageZ1 = "";
            //string sDamageZ2 = "";

            
            //DateTime dtMeasureDT = Convert.ToDateTime("9999/01/01");

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("LM", lsetInfo, MagInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
				paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
				
				//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
				plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                FileValueInfo fileValueInfo = new FileValueInfo();

				if (plmInfo.TotalKB != Convert.ToString(Constant.CalcType.SP))
				{
					if (plmInfo.RefQcParamNO.HasValue) // Total_KBがSPじゃ無いなのにRefQcParam_NOが設定されている場合にエラー
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_136, plmInfo.QcParamNO));
					}

					fileValueInfo = GetFileValue(filefmtInfo.ColumnNO, textArray);
				}
				else
				{
					if (plmInfo.RefQcParamNO.HasValue)
					{
						//前回取得値差異
						fileValueInfo = GetFileValueGap(filefmtInfo.ColumnNO, lsetInfo.EquipmentNO, plmInfo.RefQcParamNO.Value, textArray);
					}
					else //Total_KBがSPなのに、RefQcParam_NOが設定されて無い場合(== int.MinValue)にエラー
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_129, plmInfo.QcParamNO));
					}
				}

                if (plmInfo != null)
                {
                    ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT);
                }
                else
                {
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, fileValueInfo.TargetStrVAL, fileValueInfo.MeasureDT, "判定なし保管のみ");//ｴﾗｰﾒｯｾｰｼﾞなし。ﾃﾞｰﾀ保管のみ
                }
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]DC:CheckQC");
            CheckQC(lsetInfo, 9, MagInfo.sMaterialCD);//6はAIの意味 SMファイル SM→OA→MMの順
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]DC:CheckQC");
        }

		private FileValueInfo GetFileValue(int columnNO, string[] textArray)
        {
            try
            {
				FileValueInfo fileValueInfo = new FileValueInfo();
				int nRowCnt = 0;
				string[] recordArray = new string[] { };

				//最終基板のみ記録。最後に記録されたsWearZ1...が最終基板。
				foreach (string srecord in textArray)
				{
					nRowCnt += 1;
					recordArray = srecord.Split(',');
					if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
					{
						continue;
					}

					if (recordArray[0] != "")
					{
						fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[FILE_MEASUREDATE_DATE] + " " + recordArray[FILE_MEASUREDATE_TIME]));
					}
					else
					{
						break;
					}
					
					fileValueInfo.TargetStrVAL = recordArray[columnNO].Trim();
				}

				return fileValueInfo;
			}
			catch (Exception err)
			{
				throw;
			}
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
        private FileValueInfo GetFileValueGap(int columnNO, string equipmentNO, int refQcParamNO, string[] textArray)
        {
            try
            {
				string[] fileValue = textArray[2].Split(',');

				if (fileValue[FILE_MEASUREDATE_DATE] == "" || fileValue[FILE_MEASUREDATE_TIME] == "")
				{
					//測定日付が含まれていない場合、ファイル処理しない
					throw new Exception(string.Format(Constant.MessageInfo.Message_134));
				}

				FileValueInfo fileValueInfo = new FileValueInfo();

				fileValueInfo = GetFileValue(columnNO, textArray);

				fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(fileValue[FILE_MEASUREDATE_DATE] + " " + fileValue[FILE_MEASUREDATE_TIME]));

				string targetVAL = fileValue[columnNO];

				decimal LotVAL = decimal.MinValue;
				if (!decimal.TryParse(targetVAL, out LotVAL))
				{
					//取得値に問題が有る場合、ファイル処理しない
					throw new Exception(string.Format(Constant.MessageInfo.Message_135, "DC:LMファイルの取得値", targetVAL));
				}

				//前回取得値(同装置での前ロット取得値)を取得
                decimal backLotVAL = ConnectDB.GetTnLOG_DParam(this.LineCD, equipmentNO, fileValueInfo.MeasureDT, refQcParamNO, 1);
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
            catch (Exception err) 
            {
                throw;
            }
		}

        #endregion

		#region HSMS

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		#endregion

        private static bool fCheckDCContinueFile(string sFileType, int nTimmingMode)
        {
            bool fCheck = true;
            if (nTimmingMode == Constant.nStartTimming)
            {
                //マガジンタイミングのファイルの場合はTrue(continue:無視する)
                switch (sFileType)
                {
                    case "OR":
                        fCheck = false;
                        break;
                }
            }
            else
            {
                fCheck = false;
            }
            return fCheck;
        }

        public ArmsLotInfo GetDCLotNo(string plantCD, string magazineNO)
        {
            string sMessage = "";
            ArmsLotInfo rtnArmsLotInfo = null;

            //API1 確認OK
            try
            {
                rtnArmsLotInfo = ConnectDB.GetInlineMagazineLotDC(LineCD, plantCD, magazineNO);//Lot,startdt,enddt取得
                //rtnArmsLotInfo.Type = armsinfo.GetLotType(rtnArmsLotInfo.Lot);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + this.LineCD + "\r\n" +
                            ex.Message + "☆マガジンNo=[" + magazineNO + "]でGetInlineMagazineLotが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                rtnArmsLotInfo = null;
            }

            if (rtnArmsLotInfo == null)
            {
                sMessage = "流動中でないかロット情報がありません。" + magazineNO;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            // マガジンロット情報取得
            return rtnArmsLotInfo;
        }

    }
}
