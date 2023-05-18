using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using EICS.Machine;
using System.Net.Sockets;
using System.Threading;
using SLCommonLib.DataBase;
using EICS.Database;
using EICS.Structure; 


namespace EICS.Machine
{
    /// <summary>
    /// スパッタ装置処理(MAP etc.)
    /// </summary>
    public class SUPMachineInfo : MachineBase
    {
		PLC_Melsec machinePLC;

        protected virtual string QRCODE_SETTING_OK_ADDR { get { return "B000010"; } }
        protected virtual string JUDGE_RESULT_ADDR { get { return "W000010"; } }
        protected virtual string SET_JUDGE_FG { get { return "W000011"; } }
        protected virtual string TYPE_ADDR { get { return "W0000A4"; } }
        protected virtual string TYPE_SET_END_ADDR { get { return "B000011"; } }
        protected virtual string QRCODE_ADDR { get { return "W000020"; } }
        protected virtual int QRCODE_PLCDEVICE_LEN { get { return 10; } }
                
        ~SUPMachineInfo()
		{
            if (machinePLC != null)
                machinePLC.Dispose();
		}

        public override void CheckFile(LSETInfo lsetInfo)
        {
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
			List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
				= commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			if (settingInfoPerLine.IsEnablePreVerify(lsetInfo.EquipmentNO))
			{
				ConnectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
				startProcess(lsetInfo);
			}

            /// マガジン別データ処理を実行
            CollectCsvFiles(lsetInfo);

            /// 自動出力データ処理を実行
            CollectAutoCsvFiles(lsetInfo);                        
        }

		//移載機側で異なるﾌﾟﾛｸﾞﾗﾑの混載を防止するので
		private void startProcess(LSETInfo lsetInfo)
		{
			base.machineStatus = Constant.MachineStatus.Runtime;

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			int nw = int.MinValue;
			//QR書込み完了フラグ監視
			nw = Convert.ToInt32(machinePLC.GetBit(QRCODE_SETTING_OK_ADDR));

			//☆装置通信--------

			//■SUPのマガジンロード時の動作
			//マガジンNo書込み完了フラグON
			if (nw > 0)
			{
				string materialCD = getTypeFromTray(lsetInfo, ref base.errorMessageList);

				if (string.IsNullOrEmpty(materialCD))
				{
					machinePLC.SetWordAsDecimalData(JUDGE_RESULT_ADDR, 2);
					return;
				}

				machinePLC.SetWordAsDecimalData(JUDGE_RESULT_ADDR, 1);

				SendInfoForSelectRecipe(lsetInfo, materialCD);

				LSETInfo lsetInfoForWstJudge = (LSETInfo)lsetInfo.Clone();
				lsetInfoForWstJudge.TypeCD = materialCD;

				JudgeProcess(lsetInfoForWstJudge);
			}
		}

		public void SendInfoForSelectRecipe(LSETInfo lsetInfo, string typeCD)
		{
			//machinePLC.SetString(SET_PLANTCD, lsetInfo.EquipmentNO);
			//machinePLC.SetWordAsDecimalData(SET_PROCESSCD, procNoForPLA);
			machinePLC.SetString(TYPE_ADDR, typeCD);

			machinePLC.SetBit(TYPE_SET_END_ADDR, 1, "1");

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
				, string.Format("ｽﾊﾟｯﾀ条件情報送信完({0}:1) ﾀｲﾌﾟ:{1} to:{2}"
				, TYPE_SET_END_ADDR, TYPE_ADDR, typeCD));
		}

		protected virtual void JudgeProcess(LSETInfo lsetInfo)
		{
			//一定時間 SET_INFOSEND_OKが0になるのを待つ。
			DateTime latestDt = DateTime.Now;

			while (machinePLC.GetBool(TYPE_SET_END_ADDR) == true)
			{
				TimeSpan ts = DateTime.Now - latestDt;
				if (ts.TotalMilliseconds > 10000.0)
				{
					machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 1);
					throw new ApplicationException("ｽﾊﾟｯﾀ装置が応答していません。ｽﾊﾟｯﾀ前照合機能に装置が対応していないか、その他の問題が発生している可能性があります。");
				}
			}

			PLCDDBBase.PLAMachineInfo plcDDGBasedMac = new PLCDDBBase.PLAMachineInfo(lsetInfo, machinePLC);

			plcDDGBasedMac.CreateFileProcess(lsetInfo, true);

			List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
			plcDDGBasedMac.StartingProcess(lsetInfo, out errMsgList);

			if (errMsgList.Count > 0)
			{
				AlertLog alertLog = AlertLog.GetInstance();
				foreach (ErrMessageInfo errMsg in errMsgList)
				{
					alertLog.logMessageQue.Enqueue(errMsg.MessageVAL);
				}
			}

			//plcDDGBasedMac.SendResultProcess(lsetInfo, true);			

			JudgeProcess(plcDDGBasedMac, lsetInfo, true);
		}

		protected virtual void JudgeProcess(PLCDDGBasedMachine plcDDGBasedMac, LSETInfo lsetInfo, bool isStartUp)
		{
            string sMessage;

			try
			{
				if (plcDDGBasedMac.CheckForMachineStopFile(isStartUp))
				{
                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾚｼﾋﾟ判定NG送信 {2} = 2", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, SET_JUDGE_FG);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

					base.machineStatus = Constant.MachineStatus.Stop;

					//判定結果NG送信
					machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 2);
				}
				else if (IsThereOKFile(lsetInfo))
				{
                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾚｼﾋﾟ判定OK送信 {2} = 1", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, SET_JUDGE_FG);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    
                    //判定結果OK送信
					machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 1);
				}
			}
			catch (Exception err)
			{
				//判定結果NG送信
				machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 2);
				Thread.Sleep(100);
				machinePLC.SetBit(QRCODE_SETTING_OK_ADDR, 1, "0");

				throw;
			}

			Thread.Sleep(100);

            sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾏｶﾞｼﾞﾝNo書込みOFF {2} = 0", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, QRCODE_SETTING_OK_ADDR);
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

			machinePLC.SetBit(QRCODE_SETTING_OK_ADDR, 1, "0");
        }

		protected bool IsThereOKFile(LSETInfo lset)
		{
			List<string> chkDirList = new List<string>();
			List<string> chkTargetFileList = new List<string>();
			List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string, List<string>>>();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lset.InlineCD);

			string chkDir = Path.Combine(lset.InputFolderNM, settingInfoPerLine.GetStartFileDirNm(lset.EquipmentNO));

			//OKファイルがあるかチェック
			chkTargetFileList = Common.GetFiles(chkDir, EICS.Structure.CIFS.EXT_OK_FILE);
			if (chkTargetFileList.Count == 0) return false;

			KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string, List<string>>(chkDir, chkTargetFileList);

			moveTargetFileList.Add(moveInfo);

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
				"設備:{0}/{1}/{2}号機【OKﾌｧｲﾙ確認の為、出力ﾌｧｲﾙ移動を開始】", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO));

			//OKファイルを退避(サブ装置スレッドのファイルも全て移動
			foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
			{
				EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
			}

			return true;
		}

		private string getTypeFromTray(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
            try
            {
			    //■マガジンNo取得"30 M000001"→"M000001"
			    string sTrayNo = machinePLC.GetMagazineNo(QRCODE_ADDR, QRCODE_PLCDEVICE_LEN);
                string[] sTrayNoItem = sTrayNo.Split(' ');
                if (sTrayNoItem.Count() < 2)
                {
                    throw new ApplicationException(string.Format("装置から取得したﾄﾚｲNoのﾌｫｰﾏｯﾄが正しくありません。ﾄﾚｲNo:{0}", sTrayNoItem));
                }
                string trayNo = sTrayNoItem[1];

                List<Arms.LotTray> lotTrayList = Arms.LotTray.GetCurrentFromMultipleServer(lsetInfo.InlineCD, null, trayNo, null, lsetInfo.ReferMultiServerFG);
                if (lotTrayList.Count == 0)
                {
                    throw new ApplicationException(string.Format(
                            "ｽﾊﾟｯﾀ用移載後ﾄﾚｲNoに割り当たっているﾛｯﾄがありません。 ﾄﾚｲNo: {0}, 参照したDBｻｰﾊﾞ:{1}", 
                            trayNo, string.Join(",", SettingInfo.GetReferServerNm(lsetInfo))));
                }
                lotTrayList = lotTrayList.OrderBy(l => l.OrderNo).ToList();

                List<string> typeList = EXCMachineInfo.getTypeList(lsetInfo, lotTrayList.Select(l => l.LotNo).ToList());

                List<Plm> spatterProgramList =  GetSpatterProgramList(lsetInfo, typeList);

			    if (IsUniqueProgram(spatterProgramList))
			    {
				    return typeList[0];
			    }
			    else
			    {
                    throw new ApplicationException(string.Format(
                            "ﾄﾚｲ割当てされているﾛｯﾄに異なるﾌﾟﾛｸﾞﾗﾑ閾値(qcParamNo:{0})が混ざっている為、ﾏｶﾞｼﾞﾝNo判定NG送信"
                            , SettingInfo.GetSingleton().SUP_ProgramParamNo));
			    }
            }
            catch (ApplicationException ex)
            {
                AlertLog alertLog = AlertLog.GetInstance();
                alertLog.logMessageQue.Enqueue(ex.Message);
                return null;
            }
		}

		public static bool IsUniqueProgram(List<Plm> spatterProgramList)
		{
			Plm firstProgram = spatterProgramList[0];

			for (int i = 1; i < spatterProgramList.Count; i++)
			{
				Plm otherProgram = spatterProgramList[i];
				if (firstProgram.ParameterVAL != otherProgram.ParameterVAL)
				{
					AlertLog alertLog = AlertLog.GetInstance();

					alertLog.logMessageQue.Enqueue(
						string.Format("ｽﾊﾟｯﾀのﾌﾟﾛｸﾞﾗﾑ不一致です。ﾁｪｯｸ対象1 ﾀｲﾌﾟ{0},ﾌﾟﾛｸﾞﾗﾑ:{1}/ﾁｪｯｸ対象2 ﾀｲﾌﾟ{2},ﾌﾟﾛｸﾞﾗﾑ:{3}"
						, firstProgram.MaterialCD, firstProgram.ParameterVAL, otherProgram.MaterialCD, otherProgram.ParameterVAL));

					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 装置へスパッタプログラムを送信するにはタイプ名を送信するだけで良いとの事であるが
		/// スパッタ用移載機にて事前に複数プログラム混載を防止するために
		/// スパッタ装置内で持たれるタイプ⇔プログラム紐付けマスタと同じものをTmPLM管理して
		/// EICSはトレイに存在する複数マガジンのタイプでTmPLMを参照して、プログラムが一つに限定出来るかをチェックする
		/// </summary>
		/// <param name="typeList"></param>
		/// <returns></returns>
		public static List<Plm> GetSpatterProgramList(LSETInfo lsetInfo, List<string> typeList)
		{
			int refQcParamNo = SettingInfo.GetSingleton().SUP_ProgramParamNo;
			List<Plm> plmList = new List<Plm>();

			foreach (string type in typeList)
			{
                Plm plmData = Plm.GetCurrentFromMultipleServer(lsetInfo.InlineCD, type, null, refQcParamNo, null, false, null, lsetInfo.ReferMultiServerFG);

                if(plmData == null)
                {
                    string refServerNm = string.Join(",", SettingInfo.GetReferServerNm(lsetInfo));
               
					throw new ApplicationException(string.Format(
                        "ﾀｲﾌﾟから「ｽﾊﾟｯﾀﾌﾟﾛｸﾞﾗﾑ番号」の閾値を取得できませんでした。 ﾀｲﾌﾟ:{0} 管理番号:{1} 参照したDBｻｰﾊﾞ:{2}", type, refQcParamNo, refServerNm));
                }

				plmList.Add(plmData);
			}

			return plmList;
		}

		//ｽﾊﾟｯﾀ装置停止処理
		public void MachineStop(string sIPAddressNO)
		{
			machinePLC.SetWordAsDecimalData(JUDGE_RESULT_ADDR, 1);//OK=1,NG=2

			//■マガジンNo書き込み完了フラグOFF
			machinePLC.SetBit(QRCODE_SETTING_OK_ADDR, 1, "0");
		}

		protected virtual void ConnectPLC(string ipAddress, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
		{
			if (machinePLC == null)
			{
				machinePLC = new PLC_Melsec(ipAddress, portNO, exclusionList);
			}
			else
			{
				if (machinePLC.ConnectedPLC() == false)
				{
					machinePLC.ConnectPLC();
				}
			}
		}


        #region CollectCsvFiles

        /// <summary>
        /// マガジン別データ処理
        /// </summary>
        /// <param name="mac"></param>
        public void CollectCsvFiles(LSETInfo lsetInfo)
        {
            AlertLog alertLog = AlertLog.GetInstance();
            CsvFile[] csv = CsvFile.Read(lsetInfo);
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();

            //Done及びRecentlyフォルダ以外にファイルがあった場合に処理に移行
            if (csv.Count() > 0)
            {
                SettingInfo commonSetting = SettingInfo.GetSingleton();
                foreach (CsvFile c in csv)
                {
                    if (c.HasData == false) continue;

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ/" + lsetInfo.MachineNM + "号機]SUPロット別処理開始：" + c.FileNm);

                    //Recentryフォルダのファイルを取得。(フォルダがなければ生成する）
                    string recentlyPath = Path.Combine(lsetInfo.InputFolderNM, CsvFile.VDC_FOLDER_NM, "Recently");
                    if (Directory.Exists(recentlyPath) == false)
        			{
                        Directory.CreateDirectory(recentlyPath);
        			}
                    CsvFile[] recentlyFile = CsvFile.ReadDescending(lsetInfo.EquipmentNO, Path.Combine(lsetInfo.InputFolderNM, CsvFile.VDC_FOLDER_NM, "Recently"));

                    //ソートした頭のファイルの完了時間を取得（前回完了時間）
                    DateTime lastenddt = c.GetLastFileEndDt(recentlyFile);

                    //直近の処理済みファイルの最終作業時間～現ファイルの最初の作業時間が規定時間を超えていれば判定不要
                    if ((c.FromDt - lastenddt).TotalHours >= commonSetting.SUP_VdcChangeExclusionTime)
                    {
                        c.IsReset = true;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ/" + lsetInfo.MachineNM + "号機]規定時間経過しているためターゲット交換と判定：" + c.FileNm);
                    }
                    
                    if (c.IsReset == false)
                    {
                        #region 連続3ロット5%上昇条件
                        CsvFile.RfError err = c.IsLastNLotContinueError(commonSetting.SUP_MultiVdcChangeCount, commonSetting.SUP_MultiVdcChangeRate, recentlyFile, c, commonSetting.SUP_VdcChangeExclusionTime);
                        if (err != CsvFile.RfError.None)
                        {
                            if(err == CsvFile.RfError.Rf1Error || err == CsvFile.RfError.BothError)
                            {
                                alertLog.logMessageQue.Enqueue("ｽﾊﾟｯﾀ" + lsetInfo.MachineSeqNO + " 管理番号:" + commonSetting.SUP_MultiVdcChangeRf1ParamNo + "RF1閾値異常：" + c.FileNm + " RF1:" + c.RF1.ToString("#.##") + " RF2:" + c.RF2.ToString("#.##"));
                                SetQCLogToNotifierOnly(lsetInfo, commonSetting.SUP_MultiVdcChangeRf1ParamNo, c.ToDt, "RF1出力x点連続異常：" + c.FileNm, 12); //timing=12はスパッタ(MAPの場合)
                            }
                            if(err == CsvFile.RfError.Rf2Error || err == CsvFile.RfError.BothError)
                            {
                                alertLog.logMessageQue.Enqueue("ｽﾊﾟｯﾀ" + lsetInfo.MachineSeqNO + " 管理番号:" + commonSetting.SUP_MultiVdcChangeRf2ParamNo + "RF2閾値異常：" + c.FileNm + " RF1:" + c.RF1.ToString("#.##") + " RF2:" + c.RF2.ToString("#.##"));
                                SetQCLogToNotifierOnly(lsetInfo, commonSetting.SUP_MultiVdcChangeRf2ParamNo, c.ToDt, "RF2出力x点連続異常：" + c.FileNm, 12); //timing=12はスパッタ(MAPの場合)
                            }
                            
                            for (int i = 0; i < commonSetting.SUP_MultiVdcChangeCount; i++)
                            {
                                alertLog.logMessageQue.Enqueue( (i + 1) + "回前値 RF1:" + recentlyFile[i].RF1.ToString("#.##") + " RF2:" + recentlyFile[i].RF2.ToString("#.##"));
                            }
                        }
                        #endregion

                        #region 連続1ロット10%上昇条件
                        err = c.IsLastNLotContinueError(1, commonSetting.SUP_OnceVdcChengeRate, recentlyFile, c, commonSetting.SUP_VdcChangeExclusionTime);
                        if (err != CsvFile.RfError.None)
                        {
                            if (err == CsvFile.RfError.Rf1Error || err == CsvFile.RfError.BothError)
                            {
                                alertLog.logMessageQue.Enqueue("ｽﾊﾟｯﾀ" + lsetInfo.MachineSeqNO + " 管理番号:" + commonSetting.SUP_OnceVdcChangeRf1ParamNo + " RF1閾値異常：" + c.FileNm + " RF1:" + c.RF1.ToString("#.##") + " RF2:" + c.RF2.ToString("#.##"));
                                SetQCLogToNotifierOnly(lsetInfo, commonSetting.SUP_OnceVdcChangeRf1ParamNo, c.ToDt, "RF1出力2点連続異常：" + c.FileNm, 12); //timing=12はスパッタ(MAPの場合)
                            }

                            if (err == CsvFile.RfError.Rf2Error || err == CsvFile.RfError.BothError)
                            {
                                alertLog.logMessageQue.Enqueue("ｽﾊﾟｯﾀ" + lsetInfo.MachineSeqNO + " 管理番号:" + commonSetting.SUP_OnceVdcChangeRf2ParamNo + " RF2閾値異常：" + c.FileNm + " RF1:" + c.RF1.ToString("#.##") + " RF2:" + c.RF2.ToString("#.##"));
                                SetQCLogToNotifierOnly(lsetInfo, commonSetting.SUP_OnceVdcChangeRf2ParamNo, c.ToDt, "RF2出力2点連続異常：" + c.FileNm, 12); //timing=12はスパッタ(MAPの場合)
                            }

                            alertLog.logMessageQue.Enqueue("前回値 RF1:" + recentlyFile[0].RF1.ToString("#.##") + " RF2:" + recentlyFile[0].RF2.ToString("#.##"));
                        }
                        #endregion
                    }

                    //チェックしたファイルをRecentlyに移動
                    FileInfo currentFileInfo = new FileInfo(c.FilePath);
                    MoveFile(currentFileInfo, Path.Combine(lsetInfo.InputFolderNM, CsvFile.VDC_FOLDER_NM, "Recently"));

                    //Recentlyフォルダをチェックして古いファイルをDone\年月フォルダに移動
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ/" + lsetInfo.MachineNM + "号機]SUPチェック済みファイルを移動(vdc_kanri)：" + c.FileNm);
                    string sCreateFileYM = File.GetLastWriteTime(currentFileInfo.FullName).ToString("yyyyMM");
                    CheckFileCountAndMoveFile(Path.Combine(lsetInfo.InputFolderNM, CsvFile.VDC_FOLDER_NM, "Recently"), Path.Combine(lsetInfo.InputFolderNM, CsvFile.VDC_FOLDER_NM, "Done", sCreateFileYM), commonSetting.SUP_MultiVdcChangeCount);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ" + lsetInfo.EquipmentNO + "号機]SUPロット別処理完了：" + c.FileNm);
                }
            }
        }

        #endregion

        #region CollectAutoCsvFiles

        protected void CollectAutoCsvFiles(LSETInfo lsetInfo)
        {
            AlertLog alertLog = AlertLog.GetInstance();
            AutoCsvFile[] csv = AutoCsvFile.Read(lsetInfo);

            foreach (AutoCsvFile c in csv)
            {

                if (c.Errors.Count >= 1)
                {
                    for (int i = 0; i < c.Errors.Count; i++)
                    {
                        alertLog.logMessageQue.Enqueue(c.Errors[i].Msg);
                        SetQCLogToNotifierOnly(lsetInfo, c.Errors[i].QcParamNo, c.ToDt, c.Errors[i].Msg, 12); //timing=12はスパッタ(MAPの場合)
                    }
                }

                //チェックしたファイルをDone\年月フォルダに移動
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ/" + lsetInfo.MachineNM + "号機]チェック済みファイルを移動(auto)：" + c.FileNm);
                FileInfo currentFileInfo = new FileInfo(c.FilePath);
                string sCreateFileYM = File.GetLastWriteTime(currentFileInfo.FullName).ToString("yyyyMM");
                MoveFile(currentFileInfo, Path.Combine(lsetInfo.InputFolderNM, AutoCsvFile.FOLDER_NM, "Done", sCreateFileYM));
            }
        }

        #endregion

        //TnLOGにデータ登録せずにTnQCNRにのみデータ登録するメソッド
        public void SetQCLogToNotifierOnly(LSETInfo lsetInfo, int qcParamNo, DateTime measureDt, string messageNm, int nTimingNo) //#SP改修待ち
        {
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
            QCLogData wQCLogData = new QCLogData();
            InspData inspData = new InspData();

            //inspectionNoを取得。TmQDIWは使用しないためdefectは0。
            inspData.InspectionNO = ConnectDB.GetMAPInspectionNo(nTimingNo, qcParamNo, lsetInfo.InlineCD);
            inspData.Defect = 0;

            if (inspData.InspectionNO != 0)
            {
                wQCLogData.EquiNO = lsetInfo.EquipmentNO;                                       //設備番号
                wQCLogData.LotNO = "";                                                          //Lot
                wQCLogData.TypeCD = lsetInfo.TypeCD;                                            //Type
                wQCLogData.MeasureDT = measureDt;                                               //計測日時
                wQCLogData.Data = 9999;                         //data ※TmQCNR登録には不要なので9999固定(判定
                wQCLogData.Defect = inspData.Defect;                                //監視項目No
                wQCLogData.InspectionNO = inspData.InspectionNO;                     //監視番号
                wQCLogData.QcprmNO = qcParamNo;                                     //動作番号
                wQCLogData.MessageNM = messageNm;                                   //メッセージ
                cndDataItem.Add(0, wQCLogData); //※既存メソッド(Notifiry)を流用するため、データは1個しかないがリスト値に入れる

                Notifier n = new Notifier(cndDataItem, 0, lsetInfo.InlineCD);
                n.Notify();
            }
            
        }

		//#region HSMS

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		//#endregion

        public static StreamReader OpenfileWithRetry(FileInfo currentFile, DateTime checkStart)
        {
            StreamReader retv;
            TimeSpan checkRag;

            try
            {
                retv = new StreamReader(currentFile.FullName);
            }
            catch(IOException)
            {
                checkRag = DateTime.Now - checkStart; 
                Thread.Sleep(5000);
                if (checkRag.TotalSeconds < 30)
                {
                    retv = OpenfileWithRetry(currentFile, checkStart);
                }
                else
                {
                    throw new ApplicationException("ファイル'" + currentFile.Name + "' を開けませんでした。");
                }
            }

            return retv;
        }
    }


    /// <summary>
    /// マガジン別データ処理用クラス
    /// </summary>
    public class CsvFile
    {
        public enum RfError
        {
            None,
            Rf1Error,
            Rf2Error,
            BothError,
        }

        public const string VDC_FILE_HEADER = "13_Vdc_kanri_";
        public const string VDC_FOLDER_NM = "13_Vdc_kanri";
        private const int DATA_HEADER_LINE = 3;

        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public string FileNm { get; set; }
        public string FilePath { get; set; }
        public string MacNo { get; set; }
        public bool IsReset { get; set; }
        public bool HasData { get; set; }
        public double RF1
        {
            get
            {
                if (d1Raw.Count == 0)
                {
                    return 0;
                }
                return d1Raw.Average();
            }
        }
        public double RF2
        {
            get
            {
                if (d2Raw.Count == 0)
                {
                    return 0;
                }
                return d2Raw.Average();
            }
        }

        List<double> d1Raw = new List<double>();
        List<double> d2Raw = new List<double>();

        private const int TIME_COL = 0;
        private const int RF1_COL = 2;
        private const int RF2_COL = 3;

        
        /// <summary>
        /// 直近ロットのファイルを取得してエラー判定
        /// </summary>
        public RfError IsLastNLotContinueError(int lotCt, int percent, CsvFile[] recentlyFile, CsvFile currentLot, double resetHour)
        {
            if (recentlyFile.Count() >= lotCt)
            {
                 double rf1 = currentLot.RF1;
                 double rf2 = currentLot.RF2;
                 DateTime fromDt = currentLot.FromDt;

                 int rf1ErrCt = 0;
                 int rf2ErrCt = 0;

                 for (int i = 0; i < lotCt; i++)
                 {
                     if (recentlyFile[i].RF1 != 0)
                     {
                         if (rf1 / recentlyFile[i].RF1 >= (1 + 0.01 * percent))
                         {
                             rf1ErrCt++;
                         }
                         rf1 = recentlyFile[i].RF1;
                     }

                     if (recentlyFile[i].RF2 != 0)
                     {
                         if (rf2 / recentlyFile[i].RF2 >= (1 + 0.01 * percent))
                         {
                             rf2ErrCt++;
                         }
                         rf2 = recentlyFile[i].RF2;
                     }

                     if ((fromDt - recentlyFile[i].ToDt).TotalHours > resetHour)
                     {
                         return RfError.None;
                     }

                     fromDt = recentlyFile[i].FromDt;
                 }

                 if (rf1ErrCt >= lotCt && rf2ErrCt >= lotCt)
                 {
                     return RfError.BothError;
                 }
                 else if (rf1ErrCt >= lotCt)
                 {
                     return RfError.Rf1Error;
                 }
                 else if (rf2ErrCt >= lotCt)
                 {
                     return RfError.Rf2Error;
                 }
                 else
                 {
                     return RfError.None;
                 }
            }

            else //ファイルが規定数揃っていない場合は判定OK (NG条件(規定ロット数連続NG)を満たさないため)
            {
                return RfError.None;
            }


        }

    
        public DateTime GetLastFileEndDt(CsvFile[] recentlyFile)
        {
            if (recentlyFile.Count() > 0)
            {
                return recentlyFile.First().ToDt;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public CsvFile()
        {
            FromDt = DateTime.MaxValue;
            ToDt = DateTime.MinValue;
        }

        public static CsvFile[] Read(LSETInfo lsetInfo)
        {
            List<CsvFile> retv = new List<CsvFile>();

            DirectoryInfo di = new DirectoryInfo(lsetInfo.InputFolderNM);
            foreach (var subdir in di.GetDirectories())
            {
                if (subdir.Name != VDC_FOLDER_NM) continue;
                retv.AddRange(Read(lsetInfo.EquipmentCD, subdir.FullName));
            }

            return retv.ToArray();
        }



        public static CsvFile[] Read(string equipmentCd, string baseDir)
        {
            List<CsvFile> retv = new List<CsvFile>();


            DirectoryInfo dir = new DirectoryInfo(baseDir);
            foreach (var subdir in dir.GetDirectories())
            {
                if (subdir.Name == "Done" || subdir.Name == "Recently") continue;
                retv.AddRange(Read(equipmentCd, subdir.FullName));
            }

            var files = dir.GetFiles().OrderBy(f => f.LastWriteTime);

            foreach (var file in files)
            {
                if (!file.Name.StartsWith(VDC_FILE_HEADER) && !file.Name.StartsWith("_" + VDC_FILE_HEADER)) continue;

                //これから処理するファイルの頭に_を付与する。リネームに失敗した場合、対象が最新ファイルであれば無視。
                //最新ファイルで無い場合は警告メッセージを表示し、停止。(Keyence側のファイル解放が遅い場合の対処）2015.9.23 湯浅
                try
                {
                    if (!file.Name.StartsWith("_"))
                    {
                        file.MoveTo(Path.Combine(file.DirectoryName, "_" + file.Name));
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ]ファイルリネーム実行：" + file.FullName);
                    }
                }
                catch
                {
                    if (file.FullName != files.Last().FullName)
                    {
                        throw new Exception(string.Format("対象ファイルがロックされています。ファイルの確認を行って下さい。ファイル：" + file.FullName));
                    }
                    continue;
                }

                CsvFile c = new CsvFile();
                c.FileNm = file.Name;
                c.MacNo = equipmentCd;
                c.FilePath = file.FullName;
                int currentLine = 0;

                StreamReader sr = SUPMachineInfo.OpenfileWithRetry(file, DateTime.Now);

                using (sr)
                {
                    while (sr.Peek() > 0)
                    {
                        try
                        {
                            string[] line = sr.ReadLine().Split(',');

                            if (currentLine < CsvFile.DATA_HEADER_LINE) //ヘッダー領域を読み飛ばす
                            {
                                currentLine++;
                                continue;
                            }
                            DateTime dt = DateTime.Parse(line[TIME_COL]);
                            double d1 = double.Parse(line[RF1_COL]);
                            double d2 = double.Parse(line[RF2_COL]);


                            if (c.FromDt >= dt) c.FromDt = dt;
                            if (c.ToDt <= dt) c.ToDt = dt;

                            if (d1 > 0)
                            {
                                c.d1Raw.Add(d1);
                                c.HasData = true;
                            }
                            if (d2 > 0) c.d2Raw.Add(d2);
                        }
                        catch
                        {
                        }

                        currentLine++;
                    }
                }

                retv.Add(c);
            }

            return retv.ToArray();
        }

        //Read関数のファイル読込順を逆順にしてリセットフラグを追加するようにしただけ
        //ファイルリストを降順ソート出来れば不要だったが方法が判らなかった　201509湯浅
        public static CsvFile[] ReadDescending(string equipmentCd, string baseDir)
        {
            List<CsvFile> retv = new List<CsvFile>();

            DirectoryInfo dir = new DirectoryInfo(baseDir);
            foreach (var subdir in dir.GetDirectories())
            {
                if (subdir.Name == "Done" || subdir.Name == "Recently") continue;
                retv.AddRange(Read(equipmentCd, subdir.FullName));
            }

            var files = dir.GetFiles().OrderByDescending(f => f.LastWriteTime);

            foreach (var file in files)
            {
                if (!file.Name.StartsWith(VDC_FILE_HEADER) && !file.Name.StartsWith("_" + VDC_FILE_HEADER)) continue;

                CsvFile c = new CsvFile();
                c.FileNm = file.Name;
                c.MacNo = equipmentCd;
                c.FilePath = file.FullName;
                int currentLine = 0;

                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    while (sr.Peek() > 0)
                    {
                        try
                        {
                            string[] line = sr.ReadLine().Split(',');

                            if (currentLine < CsvFile.DATA_HEADER_LINE) //ヘッダー領域を読み飛ばす
                            {
                                currentLine++;
                                continue;
                            }
                            DateTime dt = DateTime.Parse(line[TIME_COL]);
                            double d1 = double.Parse(line[RF1_COL]);
                            double d2 = double.Parse(line[RF2_COL]);


                            if (c.FromDt >= dt) c.FromDt = dt;
                            if (c.ToDt <= dt) c.ToDt = dt;

                            if (d1 > 0)
                            {
                                c.d1Raw.Add(d1);
                                c.HasData = true;
                            }
                            if (d2 > 0) c.d2Raw.Add(d2);
                        }
                        catch
                        {
                            throw;
                        }

                        currentLine++;
                    }
                }

                retv.Add(c);
            }

            return retv.ToArray();
        }

        //public static CsvFile[] GetRecentlyFiles(LSETInfo lsetInfo, string recentlyPath)
        //{
        //    List<CsvFile> retv = new List<CsvFile>();

        //    DirectoryInfo di = new DirectoryInfo(recentlyPath);
        //    foreach (var subdir in di.GetDirectories())
        //    {
        //        if (subdir.Name != "Recently") continue;
        //        retv.AddRange(Read(lsetInfo.EquipmentCD, subdir.FullName));
        //    }

        //    return retv.ToArray();
        //}


    }

    /// <summary>
    /// 定期出力ファイル処理用クラス
    /// </summary>
    public class AutoCsvFile
    {

        public class LimitError

        {
            public DateTime Date { get; set; }
            public string Msg { get; set; }
            public int QcParamNo { get; set; }
        }

        public const string FILE_HEADER = "03_auto_";
        public const string FOLDER_NM = "03_auto";

        private const int TIME_COL = 0;
        private const int RF1_SHUTURYOKU_COL = 23;
        private const int RF2_SHUTURYOKU_COL = 30;
        private const int RF1_HANSHAHA_COL = 24;
        private const int RF2_HANSHAHA_COL = 31;
        private const int GUS_RYUURYOU_COL = 13;
        private const int GUS_ATSU_COL = 19;
        private const int DATA_HEADER_LINE = 3;


        public DateTime FromDt { get; set; }
        public DateTime ToDt { get; set; }
        public string FileNm { get; set; }
        public string FilePath { get; set; }
        public string MacNo { get; set; }


        bool SV1, SV2, Pr1, Pr2, Ar1, VO;


        public List<LimitError> Errors = new List<LimitError>();

        public static AutoCsvFile[] Read(LSETInfo lsetInfo)
        {
            List<AutoCsvFile> retv = new List<AutoCsvFile>();

            DirectoryInfo di = new DirectoryInfo(lsetInfo.InputFolderNM);
            foreach (var subdir in di.GetDirectories())
            {
                retv.AddRange(Read(lsetInfo.EquipmentCD, subdir.FullName, lsetInfo));
            }

            return retv.ToArray();
        }


        public static AutoCsvFile[] Read(string macno, string baseDir, LSETInfo lsetInfo)
        {
            List<AutoCsvFile> retv = new List<AutoCsvFile>();

            DirectoryInfo dir = new DirectoryInfo(baseDir);
            foreach (var subdir in dir.GetDirectories())
            {
                if (subdir.Name == "Done") continue;
                retv.AddRange(Read(lsetInfo.EquipmentCD, subdir.FullName, lsetInfo));
            }

            var files = dir.GetFiles().OrderBy(f => f.LastWriteTime);



            if (files.Count() > 0)
            {
                SettingInfo commonSetting = SettingInfo.GetSingleton();
                List<Plm> PlmParamList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, false);
                string message;
                int currentQcPramNo = 0;

                //各パラメータ取得
                Plm plmInfo_rf1Shuturyoku;
                Plm plmInfo_rf2Shuturyoku;
                Plm plmInfo_rf1Hansha;
                Plm plmInfo_rf2Hansha;
                Plm plmInfo_gusRyuuryou;
                Plm plmInfo_gusAtsu;
                
                try
                {
                    currentQcPramNo = commonSetting.SUP_AutoFileRf1ParamNo;
                    plmInfo_rf1Shuturyoku = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();

                    currentQcPramNo = commonSetting.SUP_AutoFileRf2ParamNo;
                    plmInfo_rf2Shuturyoku = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();

                    currentQcPramNo = commonSetting.SUP_AutoFileGusFlowParamNo;
                    plmInfo_gusRyuuryou = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();

                    currentQcPramNo = commonSetting.SUP_AutoFileReflect1ParamNo;
                    plmInfo_rf1Hansha = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();

                    currentQcPramNo = commonSetting.SUP_AutoFileReflect2ParamNo;
                    plmInfo_rf2Hansha = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();

                    currentQcPramNo = commonSetting.SUP_AutoFileGusTentionParamNo;
                    plmInfo_gusAtsu = PlmParamList.Where(p => p.QcParamNO == currentQcPramNo).Single();
                }
                catch
                {
                    message = string.Format(Constant.MessageInfo.Message_28, lsetInfo.TypeCD, currentQcPramNo, "ｽﾊﾟｯﾀ");
                    throw new Exception(message);
                }

                foreach (var file in files)
                {
                    if (!file.Name.StartsWith(FILE_HEADER) && !file.Name.StartsWith("_" + FILE_HEADER)) continue;
                    if (file.LastWriteTime <= DateTime.Now.AddMonths(-1)) continue;

                    //これから処理するファイルの頭に_を付与する。リネームに失敗した場合、対象が最新ファイルであれば無視。
                    //最新ファイルで無い場合は警告メッセージを表示し、停止。(Keyence側のファイル解放が遅い場合の対処）2015.9.23 湯浅
                    try
                    {
                        if (!file.Name.StartsWith("_"))
                        {
                            file.MoveTo(Path.Combine(file.DirectoryName, "_" + file.Name));
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[ｽﾊﾟｯﾀ]ファイルリネーム実行：" + file.FullName);
                            Thread.Sleep(500);
                        }
                    }
                    catch
                    {
                        if (file.FullName != files.Last().FullName)
                        {
                            throw new Exception(string.Format("対象ファイルがロックされています。ファイルの確認を行って下さい。ファイル：" + file.FullName));
                        }
                        continue;
                    }

                    AutoCsvFile c = new AutoCsvFile();
                    c.FileNm = file.Name;
                    c.MacNo = macno;
                    c.FilePath = file.FullName;

                    int gusAtsuNgCt = 0;
                    int rf1HanshaNgCt = 0;
                    int rf2HanshaNgCt = 0;
                    int currentLine = 0;
                    
                    StreamReader sr = SUPMachineInfo.OpenfileWithRetry(file, DateTime.Now);

                    using (sr)
                    {
                        while (sr.Peek() > 0)
                        {

                            try
                            {
                                string[] line = sr.ReadLine().Split(',');

                                if (currentLine < AutoCsvFile.DATA_HEADER_LINE) //ヘッダー領域を読み飛ばす
                                {
                                    currentLine++;
                                    continue;
                                }

                                message = null;
                                                                
                                DateTime dt = DateTime.Parse(line[TIME_COL]);
                                if (c.FromDt >= dt) c.FromDt = dt;
                                if (c.ToDt <= dt) c.ToDt = dt;

                                double rf1Shuturyoku = double.Parse(line[RF1_SHUTURYOKU_COL]);
                                double rf2Shuturyoku = double.Parse(line[RF2_SHUTURYOKU_COL]);

                                double rf1Hansha = double.Parse(line[RF1_HANSHAHA_COL]);
                                double rf2Hansha = double.Parse(line[RF2_HANSHAHA_COL]);

                                double gusRyuuryou = double.Parse(line[GUS_RYUURYOU_COL]);
                                double gusAtsu = double.Parse(line[GUS_ATSU_COL]);

                                #region RF1出力

                                if (rf1Shuturyoku > Convert.ToDouble(plmInfo_rf1Shuturyoku.ParameterMAX))
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf1Shuturyoku.ParameterNM, "MAX", rf1Shuturyoku, plmInfo_rf1Shuturyoku.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileRf1ParamNo), QcParamNo = commonSetting.SUP_AutoFileRf1ParamNo });
                                    c.SV1 = true;
                                }
                                else if (rf1Shuturyoku < Convert.ToDouble(plmInfo_rf1Shuturyoku.ParameterMIN) && rf1Shuturyoku >= commonSetting.SUP_AutoFileRfExclusionTemp)
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf1Shuturyoku.ParameterNM, "MIN", rf1Shuturyoku, plmInfo_rf1Shuturyoku.ParameterMIN, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileRf1ParamNo), QcParamNo = commonSetting.SUP_AutoFileRf1ParamNo });
                                    c.SV1 = true;
                                }

                                #endregion

                                #region RF2出力

                                if (rf2Shuturyoku > Convert.ToDouble(plmInfo_rf2Shuturyoku.ParameterMAX))
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf2Shuturyoku.ParameterNM, "MAX", rf2Shuturyoku, plmInfo_rf2Shuturyoku.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileRf2ParamNo), QcParamNo = commonSetting.SUP_AutoFileRf2ParamNo });
                                    c.SV2 = true;
                                }
                                else if (rf2Shuturyoku < Convert.ToDouble(plmInfo_rf2Shuturyoku.ParameterMIN) && rf2Shuturyoku >= commonSetting.SUP_AutoFileRfExclusionTemp)
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf2Shuturyoku.ParameterNM, "MIN", rf2Shuturyoku, plmInfo_rf2Shuturyoku.ParameterMIN, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileRf2ParamNo), QcParamNo = commonSetting.SUP_AutoFileRf2ParamNo });
                                    c.SV2 = true;
                                }
                                #endregion

                                #region ガス流量

                                if (gusRyuuryou > Convert.ToDouble(plmInfo_gusRyuuryou.ParameterMAX))
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_gusRyuuryou.ParameterNM, "MAX", gusRyuuryou, plmInfo_gusRyuuryou.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileGusFlowParamNo), QcParamNo = commonSetting.SUP_AutoFileGusFlowParamNo });
                                    c.Ar1 = true;
                                }
                                else if (gusRyuuryou < Convert.ToDouble(plmInfo_gusRyuuryou.ParameterMIN))
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_gusRyuuryou.ParameterNM, "MIN", gusRyuuryou, plmInfo_gusRyuuryou.ParameterMIN, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileGusFlowParamNo), QcParamNo = commonSetting.SUP_AutoFileGusFlowParamNo });
                                    c.Ar1 = true;
                                }

                                #endregion

                                #region 反射波RF1

                                if (rf1Hansha >= Convert.ToDouble(plmInfo_rf1Hansha.ParameterMAX))
                                {
                                    rf1HanshaNgCt++;
                                }
                                else
                                {
                                    rf1HanshaNgCt = 0;
                                }

                                if (rf1HanshaNgCt >= commonSetting.SUP_AutoFileReflectCheckCount)
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf1Hansha.ParameterNM + "(継続NG)", "MAX", rf1Hansha, plmInfo_rf1Hansha.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileReflect1ParamNo), QcParamNo = commonSetting.SUP_AutoFileReflect1ParamNo });
                                    c.Pr1 = true;
                                }         
                       
                                #endregion
                                
                                #region 反射波RF2

                                if (rf2Hansha > Convert.ToDouble(plmInfo_rf2Hansha.ParameterMAX))
                                {
                                    rf2HanshaNgCt++;
                                }
                                else
                                {
                                    rf2HanshaNgCt = 0;
                                }

                                if (rf2HanshaNgCt >= commonSetting.SUP_AutoFileReflectCheckCount)
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_rf2Hansha.ParameterNM + "(継続NG)", "MAX", rf2Hansha, plmInfo_rf2Hansha.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileReflect2ParamNo), QcParamNo = commonSetting.SUP_AutoFileReflect2ParamNo });
                                    c.Pr2 = true;
                                }

                                #endregion


                                if (gusAtsu != Convert.ToDouble(plmInfo_gusAtsu.ParameterMAX))
                                {
                                    gusAtsuNgCt++;
                                }
                                else
                                {
                                    gusAtsuNgCt = 0;
                                }

                                if (gusAtsuNgCt >= commonSetting.SUP_AutoFileGusTentionCheckCount) //#SP改修待ち　判定範囲を設定ファイルから取得に変更
                                {
                                    c.Errors.Add(new LimitError() { Date = dt, Msg = string.Format(Constant.MessageInfo.Message_22, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo_gusAtsu.ParameterNM + "(継続NG)", "MAX", gusAtsu, plmInfo_gusAtsu.ParameterMAX, "", lsetInfo.InlineCD, commonSetting.SUP_AutoFileGusTentionParamNo), QcParamNo = commonSetting.SUP_AutoFileGusTentionParamNo });
                                    c.VO = true;
                                }

                                currentLine++;

                            }
                            catch
                            {
                            }
                        }
                    }

                    retv.Add(c);

                }

                                
            }
            return retv.ToArray();
        }

    }

}
