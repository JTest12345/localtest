using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using EICS.Machine;
using System.Threading;
using EICS.Database;
using EICS.Structure;

namespace EICS.Machine
{
    /// <summary>
    /// プラズマ機処理
    /// </summary>
    public class PLAMachineInfo : MachineBase
    {
		/// <summary>プラズマ装置名</summary>
		public const string ASSETS_NM = "ﾌﾟﾗｽﾞﾏ";

		protected const string GEN_PLA_PROC = "PLA_PROC";


        /// <summary>ファイル内容マガジンNO行</summary>
        protected const int FILE_MAGAZINEROW = 3;

        /// <summary>ファイル内容マガジンNO列</summary>
        protected const int FILE_MAGAZINECOL = 3;

		private const int FILE_DATECOL = 1;

		private const int FILE_TIMECOL = 2;

		/// <summary>各ファイルの左端先頭に追加される列数（参照列を右方向にオフセットする量）</summary>
		private const int COL_OFFSET_BY_ADDED_COL_TO_TOP = 1;

		/// 各ファイルの列数定義はオフセットして無い状態のもの
		/// <summary>LEファイルで参照する列</summary>
		private const int LE_COL_LEN = 10;
		private const int LE_COL_TARGET_FG = 9;

		/// <summary>EEファイルで参照する列</summary>
		private const int EE_COL_LEN = 10;
		private const int EE_COL_TARGET_FG = 9;

		/// <summary>JEファイルで参照する列</summary>
		private const int JE_COL_LEN = 14;
		private const int JE_COL_TARGET_FG = 13;

		protected virtual string GET_MAGAZINE_ADDR() { return "W000300"; }
		protected virtual int GET_MAGAZINENO_WDEVICE_LEN() { return 10; }
		protected virtual string GET_MAGAZINE_SETTING_OK_ADDR() { return "B000300"; }
		protected virtual string SET_JUDGE_ADDR() { return "W000210"; }
		protected virtual string SET_INFOSEND_OK_ADDR() { return "B000230"; }
		protected virtual string SET_JUDGENG_FG_ADDR() { return "W000250"; }
		protected virtual string SET_PLANTCD_ADDR() { return "W000260"; }
		protected virtual string SET_PROCESSCD_ADDR() { return "W000263"; }
		protected virtual string SET_TYPECD_ADDR() { return "W000264"; }

		//PLC PLCInst;
		protected PLC_Melsec machinePLC;
		//public void ConnectPLC(string ipAddress, int portNO)
		//{
		//	if (PLCInst == null)
		//	{
		//		PLCInst = new PLC(ipAddress, portNO);
		//	}
		//	else
		//	{
		//		if (PLCInst.ConnectedPLC() == false)
		//		{
		//			PLCInst.ConnectPLC();
		//		}
		//	}
		//}
		protected void InitAndCheck(int lineCD)
		{
			GetPlaWorkCDProcNoDict(lineCD);
		}

		protected void ConnectPLC(string ipAddress, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
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

        /// <summary>
        /// ファイルチェック 
        /// </summary>
        /// <param name="mFile"></param>
        /// <param name="lsetInfo"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
				= commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

			ConnectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
			//ConnectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO);

			CheckDirectory(lsetInfo);

            base.machineStatus = Constant.MachineStatus.Runtime;

            System.Net.Sockets.TcpClient tcp = null;
            System.Net.Sockets.NetworkStream ns = null;


			StartingProcess(lsetInfo);
            //------------------

            try
            {
                //■PLAのマガジンアンロード時の動作(出力順)
                ClearPLAFolderPath(lsetInfo);
                SortedList<int, DateTime> sortedlistLogFile = KLinkInfo.CntLogFile2(lsetInfo.InputFolderNM + "LE\\", "LE");
                //マガジンファイルを見つけた時の処理(1マガジン単位ファイルが1つ以上あった場合)
                if (sortedlistLogFile.Count > 0)
                {
                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[2]LEファイル処理前 " + lsetInfo.ChipNM); 

                    FileProcessingPLA(ref tcp, ref ns, lsetInfo, "LE", ref base.errorMessageList);

                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[3]LEファイル処理後 " + lsetInfo.ChipNM); 
                }
                ClearPLAFolderPath(lsetInfo);
                sortedlistLogFile = KLinkInfo.CntLogFile2(lsetInfo.InputFolderNM + "EE\\", "EE");
                if (sortedlistLogFile.Count > 0)
                {
                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[4]EEファイル処理前 " + lsetInfo.ChipNM); 

                    FileProcessingPLA(ref tcp, ref ns, lsetInfo, "EE", ref base.errorMessageList);

                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[5]EEファイル処理後 " + lsetInfo.ChipNM); 
                }
                ClearPLAFolderPath(lsetInfo);
                sortedlistLogFile = KLinkInfo.CntLogFile2(lsetInfo.InputFolderNM + "JE\\", "JE");
                if (sortedlistLogFile.Count > 0)
                {
                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[6]JEファイル処理前 " + lsetInfo.ChipNM); 

                    FileProcessingPLA(ref tcp, ref ns, lsetInfo, "JE", ref base.errorMessageList);

                    //☆
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[7]JEファイル処理後 " + lsetInfo.ChipNM); 
                }
            }
            catch (Exception err)
            {
                throw;
            }

        }

		protected void StartingProcess(LSETInfo lsetInfo)
		{
			base.machineStatus = Constant.MachineStatus.Runtime;

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			int nw = int.MinValue;
			//マガジンNo書込み完了フラグ監視
			nw = Convert.ToInt32(machinePLC.GetBit(GET_MAGAZINE_SETTING_OK_ADDR()));

			//☆装置通信--------

			//■PLAのマガジンロード時の動作
			//マガジンNo書込み完了フラグON
			if (nw > 0)
			{
				if (settingInfoPerLine.IsEnablePreVerify(lsetInfo.EquipmentNO))
				{
					MagInfo magInfo = GetLotInfo(lsetInfo, ref base.errorMessageList);
					int procNoForPLA;

					if (magInfo == null)
					{
						machinePLC.SetWordAsDecimalData(SET_JUDGE_ADDR(), 1);
						return;
					}

					machinePLC.SetWordAsDecimalData(SET_JUDGE_ADDR(), 0);


					string workCd = ConnectDB.GetCurrentWork(lsetInfo.InlineCD, magInfo.sNascaLotNO);//現在の作業工程取得
					lsetInfo.ChipNM = workCd;

					int plaProcNo;

					Dictionary<string, int> plaWorkCDProcNoDict = GetPlaWorkCDProcNoDict(lsetInfo.InlineCD);

					if (plaWorkCDProcNoDict.ContainsKey(workCd) == false)
					{
						throw new ApplicationException(string.Format(
							"TmGeneralのGeneralGrp_CD:{0}に{1}の値が見つかりませんでした。ﾏｽﾀ設定を確認してください。(ﾏｽﾀ未設定、文字列不一致、改行ｺｰﾄﾞ混入など)"
							, GEN_PLA_PROC, workCd));
					}

					plaWorkCDProcNoDict.TryGetValue(workCd, out plaProcNo);

					SendInfoForSelectRecipe(lsetInfo, plaProcNo, magInfo.sMaterialCD);

					lsetInfo.TypeCD = magInfo.sMaterialCD;

					JudgeProcess(lsetInfo);
				}
				else
				{
					PLAInitSetting(lsetInfo.ModelNM, lsetInfo.IPAddressNO, ref lsetInfo, ref base.errorMessageList);//プラズマへ初期設定を行う EquiInfoは中で再設定
				}
			}
		}

		protected Dictionary<string, int> GetPlaWorkCDProcNoDict(int lineCD)
		{
			Dictionary<string, int> plaWorkCDProcNoDict = new Dictionary<string, int>();
			List<General> plaProcList = General.GetGeneralData(GEN_PLA_PROC, lineCD);

			if (plaProcList.Count == 0)
			{
				throw new ApplicationException(string.Format("TmGeneralのGeneralGrp_CD:{0}にGeneralCD:プラズマ装置作業CD⇔GeneralNM:装置送信Noの紐付け設定が必要です。", GEN_PLA_PROC));
			}

			foreach (General plaWorkProc in plaProcList)
			{
				int plaProcNo;
				if (int.TryParse(plaWorkProc.GeneralNM, out plaProcNo) == false)
				{
					throw new ApplicationException(string.Format("TmGeneralから取得したNM値が数値変換出来ませんでした。変換対象:{0}", plaWorkProc.GeneralNM));
				}

				plaWorkCDProcNoDict.Add(plaWorkProc.GeneralCD, plaProcNo);
			}

			return plaWorkCDProcNoDict;
		}

		protected void SendInfoForSelectRecipe(LSETInfo lsetInfo, int procNoForPLA, string typeCD)
		{
			machinePLC.SetString(SET_PLANTCD_ADDR(), lsetInfo.EquipmentNO);
			machinePLC.SetWordAsDecimalData(SET_PROCESSCD_ADDR(), procNoForPLA);
			machinePLC.SetString(SET_TYPECD_ADDR(), typeCD);

			machinePLC.SetBit(SET_INFOSEND_OK_ADDR(), 1, "1");

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
				, string.Format("ﾌﾟﾗｽﾞﾏ条件情報送信完({0}:1) 設備:{1} to:{2} / 工程No:{3} to:{4} / ﾀｲﾌﾟ:{5} to:{6}"
				, SET_INFOSEND_OK_ADDR(), lsetInfo.EquipmentNO, SET_PLANTCD_ADDR(), procNoForPLA, SET_PROCESSCD_ADDR(), typeCD, SET_TYPECD_ADDR()));
		}

		protected void JudgeProcess(LSETInfo lsetInfo)
		{
			//一定時間 SET_INFOSEND_OKが0になるのを待つ。
			DateTime latestDt = DateTime.Now;

			while (machinePLC.GetBool(SET_INFOSEND_OK_ADDR()) == true)
			{
				TimeSpan ts = DateTime.Now - latestDt;
				if (ts.TotalMilliseconds > 10000.0)
				{
					machinePLC.SetWordAsDecimalData(SET_JUDGENG_FG_ADDR(), 1);
					throw new ApplicationException("プラズマ装置が応答していません。プラズマ前照合機能に装置が対応していないか、その他の問題が発生している可能性があります。");
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

		protected MagInfo GetLotInfo(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();

			SettingInfo commonSetting = SettingInfo.GetSingleton();
			string msgStr;

			//■マガジンNo取得"30 M000001"→"M000001"
			string sMagazineNo = machinePLC.GetMagazineNo(GET_MAGAZINE_ADDR(), GET_MAGAZINENO_WDEVICE_LEN());

			string[] sMagazineNoItem = sMagazineNo.Split(' ');

			if (sMagazineNoItem.Count() < 2)
			{
				throw new ApplicationException(string.Format("装置から取得したﾏｶﾞｼﾞﾝNoのﾌｫｰﾏｯﾄが正しくありません。ﾏｶﾞｼﾞﾝNo:{0}", sMagazineNo));
			}

			sMagazineNo = sMagazineNoItem[1];

			//■マガジンNoからLot/Type/どの工程か?取得
			ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, sMagazineNo);   //TODO　Nullが他に残る為、tnmagに変えたほうがいい？？
			if (rtnArmsLotInfo == null)
			{
				msgStr = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nマガジンNoから現在流動中のロットを取得出来ません。\r\n現在のNASCA情報が不正です。\r\n[mag]=" + sMagazineNo + "/[IP]=" + lsetInfo.IPAddressNO;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, msgStr);
				ErrMessageInfo errMessageInfo = new ErrMessageInfo(msgStr, Color.Red);
				errMessageList.Add(errMessageInfo);


				//ロットを取得出来ない場合は装置停止
				MachineStop(lsetInfo.IPAddressNO);

				return null;
			}
			else
			{
				magInfo.sMagazineNO = sMagazineNo;
				magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
				magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
			}

			return magInfo;
		}

		protected void JudgeProcess(PLCDDGBasedMachine plcDDGBasedMac, LSETInfo lsetInfo, bool isStartUp)
		{
			try
			{
				if (plcDDGBasedMac.CheckForMachineStopFile(isStartUp))
				{
					base.machineStatus = Constant.MachineStatus.Stop;

					//判定結果NG送信
					machinePLC.SetWordAsDecimalData(SET_JUDGENG_FG_ADDR(), 1);
				}
				else if (IsThereOKFile(lsetInfo))
				{
					//判定結果OK送信
					machinePLC.SetWordAsDecimalData(SET_JUDGENG_FG_ADDR(), 0);


				}
			}
			catch (Exception err)
			{
				//判定結果NG送信
				machinePLC.SetWordAsDecimalData(SET_JUDGENG_FG_ADDR(), 1);
				Thread.Sleep(100);
				machinePLC.SetBit(GET_MAGAZINE_SETTING_OK_ADDR(), 1, "0");

				throw;
			}

			Thread.Sleep(100);

			machinePLC.SetBit(GET_MAGAZINE_SETTING_OK_ADDR(), 1, "0");
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

        //「装置情報」と「どのログファイルを処理すれば良いか」を入れると、装置ログをデータベース登録してくれる。
        //異常があれば、true。問題なければ、falseを返す。
        private void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string sTargetFile, ref List<ErrMessageInfo> errMessageList)
        {
            string sfname = "";
            string sWork = "";
            string sFileType = "";
            string[] textArray = new string[] { };
            string sMessage = "";

			lsetInfo.InputFolderNM = lsetInfo.InputFolderNM + sTargetFile + "\\";

			//string sInputFolder = lsetInfo.InputFolderNM + sTargetFile + "\\";

            //対象ファイルが2つ以上ある場合、最新ファイル以外を[reserve]に移動する。　[reserve]に移動したファイルは今後も必要無し
			List<string> mFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + sTargetFile);
            if (mFileList.Count > 1) 
            {
                List<FileInfo> mFileInfoList = new List<FileInfo>();
                foreach (string mFile in mFileList)
                {
                    FileInfo fileInfo = new FileInfo(mFile);
                    mFileInfoList.Add(fileInfo);
                }
                IEnumerable<FileInfo> mSortFileList = mFileInfoList.OrderByDescending(m => m.LastWriteTime);

                //[Reserve]フォルダに移動
                int i = 0;
                foreach(FileInfo mSortFile in mSortFileList)
                {
                    if (i == 0) { i++; continue; }
                    MoveReserveFile(mSortFile.FullName, lsetInfo.AssetsNM);
                }
            }

            try
            {
                //「LogファイルデータをDB登録
				List<string> machineFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + sTargetFile);
                foreach (string mFile in machineFileList)
                {
                    FileInfo fileInfo = new FileInfo(mFile);

                    //<--人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima
                    bool flg;
                    for (int j = 0; j < 5; j++)
                    {
                        //
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
                    if (textArray[2] == "" || textArray[3] == "")
                    {
                        System.IO.File.Delete(fileInfo.FullName);
                        continue;
                    }

                    //ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
                    switch (sFileType)
                    {
                        case "LE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:LE File");
                            DbInput_PLA_LEFile(ref tcp, ref ns, lsetInfo, textArray, fileInfo.FullName, ref errMessageList);
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:LE File");
                            break;
                        case "EE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:EE File");
                            DbInput_PLA_EEFile(ref tcp, ref ns, lsetInfo, textArray, fileInfo.FullName, ref errMessageList);
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:EE File");
                            break;
                        case "JE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:JE File");
                            DbInput_PLA_JEFile(ref tcp, ref ns, lsetInfo, textArray, fileInfo.FullName, ref errMessageList);
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:JE File");
                            break;
                    }

                    //処理済みファイルを保管フォルダへ移動
                    MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, "", "");
                }
            }
            catch (Exception err) 
            {
                throw;
            }
        }

        #region 各ファイル処理

        //プラズマファイル処理
        public void FileProcessingPLA(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string sFileIndex, ref List<ErrMessageInfo> errMessageList)
        {
            string sfname = "";
            string sFileType = "";
            //string sWork = "";
            string[] textArray = new string[] { };

			//PLCInst = PLC.GetInstance(lsetInfo.IPAddressNO);

            try
            {
                string sInputFolder = lsetInfo.InputFolderNM + sFileIndex + "\\";
                foreach (string swfname in System.IO.Directory.GetFiles(sInputFolder))
                {
                    sfname = swfname.Substring(sInputFolder.Length, swfname.Length - sInputFolder.Length);      //ファイル名取得
                    sFileType = sfname.Substring(4, 2);                                             //ファイルタイプ取得 LE,JE,EE

					Thread.Sleep(1000);

                    //出力順
                    switch (sFileType)
                    {
                        case "LE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:LE File");
                            machinePLC.SetBit(PLC.SET_CHECK_LE_FLG, 1, "1");//判定結果待ちうけフラグON
                            FileDistribution(ref tcp, ref ns, lsetInfo, "LE", ref errMessageList);
                            machinePLC.SetBit(PLC.SET_CHECK_LE_FLG, 1, "0");//判定結果待ちうけフラグOFF

                            //現状出力ファイルからの判定はすべてOKとする。
                            machinePLC.SetWordAsDecimalData(PLC.SET_LE_JUDGE, 0);//判定OK=0,NG=1
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:LE File");
                            break;
                        case "EE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:EE File");
                            machinePLC.SetBit(PLC.SET_CHECK_EE_FLG, 1, "1");//判定結果待ちうけフラグON
                            FileDistribution(ref tcp, ref ns, lsetInfo, "EE", ref errMessageList);
                            machinePLC.SetBit(PLC.SET_CHECK_EE_FLG, 1, "0");//判定結果待ちうけフラグOFF

                            //現状出力ファイルからの判定はすべてOKとする。
                            machinePLC.SetWordAsDecimalData(PLC.SET_EE_JUDGE, 0);//判定OK=0,NG=1
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:EE File");
                            break;
                        case "JE":
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:JE File");
                            machinePLC.SetBit(PLC.SET_CHECK_JE_FLG, 1, "1");//判定結果待ちうけフラグON
                            FileDistribution(ref tcp, ref ns, lsetInfo, "JE", ref errMessageList);
                            machinePLC.SetBit(PLC.SET_CHECK_JE_FLG, 1, "0");//判定結果待ちうけフラグOFF

                            //現状出力ファイルからの判定はすべてOKとする。
                            machinePLC.SetWordAsDecimalData(PLC.SET_JE_JUDGE, 0);//判定OK=0,NG=1
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:JE File");
                            break;

                        default:
                            //不明ファイルは削除
                            System.IO.File.Delete(swfname);
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                throw;
                //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                //sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "/" + lsetInfo.InputFolderNM + "が見つかりません。";
                //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                //PLCInst.SetBit(PLC.SET_CHECK_JE_FLG, 1, "0");//判定結果待ちうけフラグOFF
            }
        }

        public void PLAInitSetting(string sModel, string sIPAddressNO, ref LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
			//PLCInst = PLC.GetInstance(sIPAddressNO);

			SettingInfo commonSetting = SettingInfo.GetSingleton();

            MagInfo MagInfo = new MagInfo();
            string sMessage = "";

			try
			{
				//■マガジンNo取得"30 M000001"→"M000001"
				string sMagazineNo = machinePLC.GetMagazineNo(GET_MAGAZINE_ADDR(), GET_MAGAZINENO_WDEVICE_LEN());
                if (sMagazineNo.Trim().Split(' ').Length >= 2)
                {
                    sMagazineNo = sMagazineNo.Split(' ')[1];
                }

				//■マガジンNoからLot/Type/どの工程か?取得
				ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, sMagazineNo);   //TODO　Nullが他に残る為、tnmagに変えたほうがいい？？
				if (rtnArmsLotInfo == null)
				{
					sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nマガジンNoから現在流動中のロットを取得出来ません。\r\n現在のNASCA情報が不正です。\r\n[mag]=" + sMagazineNo + "/[IP]=" + sIPAddressNO;
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
					errMessageList.Add(errMessageInfo);

#if Debug
#else
					//ロットを取得出来ない場合は装置停止
					PLASTOP(sIPAddressNO);
#endif
					return;
				}
				else
				{
					MagInfo.sMagazineNO = sMagazineNo;
					MagInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
					MagInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
				}

				//■設定値取得

				//2015.2.12 車載3次 DB前, WB前の識別方法を名前のContains検索とChip_NM(ARMS作業CD)検索方法に変更
				//int nprocno = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);//現在の作業工程取得
				//string sExtension = GetPLAMode(nprocno);
				string workCd = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);//現在の作業工程取得
				lsetInfo.ChipNM = workCd;


				//Rf
				Prm rfPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_RfParamNOList, workCd);
				Plm rfPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, rfPrm.ParamNO, lsetInfo.EquipmentNO, false);

				string paramErrMsg = string.Empty;

				Plm.HasProblem(lsetInfo.InlineCD, rfPlm, ref paramErrMsg);
				int nRF = Convert.ToInt32(rfPlm.ParameterMAX);


				//処理時間
				Prm timePrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_TimeParamNOList, workCd);
				Plm timePlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, timePrm.ParamNO, lsetInfo.EquipmentNO, false);

				Plm.HasProblem(lsetInfo.InlineCD, timePlm, ref paramErrMsg);
				int nTime = Convert.ToInt32(timePlm.ParameterMAX) * 10;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)

				//Arガス流量
				Prm arPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_ArParamNOList, workCd);
				Plm arPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, arPrm.ParamNO, lsetInfo.EquipmentNO, false);

				Plm.HasProblem(lsetInfo.InlineCD, arPlm, ref paramErrMsg);
				int nGas = Convert.ToInt32(arPlm.ParameterMAX) * 10;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)

				//真空度
				Prm vacuumPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_VacuumParamNOList, workCd);
				Plm vacuumPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, vacuumPrm.ParamNO, lsetInfo.EquipmentNO, false);

				Plm.HasProblem(lsetInfo.InlineCD, vacuumPlm, ref paramErrMsg);
				int nVacuum = Convert.ToInt32(vacuumPlm.ParameterMAX) * 100;

				//放射開始圧力
				Prm pressPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_PressParamNOList, workCd);
				Plm pressPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, pressPrm.ParamNO, lsetInfo.EquipmentNO, false);

				Plm.HasProblem(lsetInfo.InlineCD, pressPlm, ref paramErrMsg);
				int nPress = Convert.ToInt32(pressPlm.ParameterMAX) * 100;

				//int nRF = ConnectDB.GetSettingParam(sModel, rtnArmsLotInfo.TypeCD, Constant.sPLA_SetRf, workCd, lsetInfo.InlineCD);
				//int nTime = ConnectDB.GetSettingParam(sModel, rtnArmsLotInfo.TypeCD, Constant.sPLA_SetTime, workCd, lsetInfo.InlineCD);
				//nTime = nTime * 10;
				//int nGas = ConnectDB.GetSettingParam(sModel, rtnArmsLotInfo.TypeCD, Constant.sPLA_SetGas, workCd, lsetInfo.InlineCD);
				//nGas = nGas * 10;
				//int nVacuum = ConnectDB.GetSettingParam(sModel, rtnArmsLotInfo.TypeCD, Constant.sPLA_SetVacuum, workCd, lsetInfo.InlineCD);
				//nVacuum = nVacuum * 100;
				//int nPress = ConnectDB.GetSettingParam(sModel, rtnArmsLotInfo.TypeCD, Constant.sPLA_SetPress, workCd, lsetInfo.InlineCD);
				//nPress = nPress * 100;

				if (string.IsNullOrEmpty(paramErrMsg) == false)
				{
					sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\n" + paramErrMsg + "\r\n/[IP]=" + sIPAddressNO;
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
					errMessageList.Add(errMessageInfo);
#if Debug
#else
					PLASTOP(sIPAddressNO);
#endif
					return;
				}
				sMessage = string.Format("ﾌﾟﾗｽﾞﾏ装置へのパラメタ送信開始。  /[設備号機]={0}  /[設備CD]={1}  /[IP]={2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, sIPAddressNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

				//■設定値書込み
				bool fNG = false;//falseは正常,trueの場合、書き込んだ値が書き込まれていない時。
				//RF入射
				if (SetPLCData(sIPAddressNO, PLC.SET_PARAM_RF, nRF) == true)
				{
					fNG = true;
				}

				// 処理時間
				if (SetPLCData(sIPAddressNO, PLC.SET_PARAM_TIME, nTime) == true)
				{
					fNG = true;
				}
				//Arガス流量
				if (SetPLCData(sIPAddressNO, PLC.SET_PARAM_AR, nGas) == true)
				{
					fNG = true;
				}
				//真空度設定
				if (SetPLCData(sIPAddressNO, PLC.SET_PARAM_VACUUM, nVacuum) == true)
				{
					fNG = true;
				}
				//放電開始圧力設定
				if (SetPLCData(sIPAddressNO, PLC.SET_PARAM_DISCHARGE, nPress) == true)
				{
					fNG = true;
				}

				//設定値が正しく書き込めなかった場合、装置停止
				if (fNG == true)
				{
					sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nプラズマ装置に所定の設定値を書き込めませんでした。\r\n/[IP]=" + sIPAddressNO;
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
					errMessageList.Add(errMessageInfo);

#if Debug
#else
					PLASTOP(sIPAddressNO);
#endif
					return;
				}

				sMessage = string.Format("ﾌﾟﾗｽﾞﾏ装置へのパラメタ送信完了。  /[設備号機]={0}  /[設備CD]={1}  /[IP]={2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, sIPAddressNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);


				//■工程書込み
				if (workCd == "DB0027")
				{//DB硬化前プラズマ洗浄
					machinePLC.SetBit(PLC.SET_PROCESS_TEACH, 1, "1");//ON=DB後
				}
				else if (workCd == "WB0002")
				{//WB前プラズマ洗浄
					machinePLC.SetBit(PLC.SET_PROCESS_TEACH, 1, "0");//OFF=DBオーブン後
				}
				else
				{
					sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nプラズマ装置の工程が不明です。\r\n/[IP]=" + sIPAddressNO + "[workcd]=" + workCd;
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
					errMessageList.Add(errMessageInfo);

#if Debug
#else
					PLASTOP(sIPAddressNO);
#endif
				}

				//☆
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[1]設定 " + MagInfo.sNascaLotNO + " " + lsetInfo.ChipNM);

				//■パラメータ書込み完了フラグON
				machinePLC.SetBit(PLC.SET_PARAM_SETTING_OK, 1, "1");
				//■マガジンNo書き込み完了フラグOFF
				machinePLC.SetBit(PLC.GET_MAGAZINE_SETTING_OK, 1, "0");

			}
			catch (Exception err)
			{
				sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nパラメータ送信処理中に問題がありました。\r\n/[IP]=" + sIPAddressNO;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

				PLASTOP(sIPAddressNO);

				throw;
			}
        }


        //ﾌﾟﾗｽﾞﾏ装置停止処理
        public void PLASTOP(string sIPAddressNO)
        {
			//PLCInst = PLC.GetInstance(sIPAddressNO);

			machinePLC.SetWordAsDecimalData(PLC.SET_JUDGE, 1);//OK=0,NG=1

            //■パラメータ書込み完了フラグON
			machinePLC.SetBit(PLC.SET_PARAM_SETTING_OK, 1, "1");
            //■マガジンNo書き込み完了フラグOFF
			machinePLC.SetBit(PLC.GET_MAGAZINE_SETTING_OK, 1, "0");
        }

		//ﾌﾟﾗｽﾞﾏ装置停止処理
		public void MachineStop(string sIPAddressNO)
		{
			//PLCInst = PLC.GetInstance(sIPAddressNO);

			machinePLC.SetWordAsDecimalData(PLC.SET_JUDGE, 1);//OK=0,NG=1

			//■マガジンNo書き込み完了フラグOFF
			machinePLC.SetBit(PLC.GET_MAGAZINE_SETTING_OK, 1, "0");
		}

        public bool SetPLCData(string sIPAddressNO, string sAddress, int nValue)
        {

			//PLCInst = PLC.GetInstance(sIPAddressNO);

            bool flg = false;//false=問題なし

            //値セット
			machinePLC.SetWordAsDecimalData(sAddress, nValue);
            //セットした値を確認
			if (machinePLC.GetWordAsDecimalData(sAddress) != nValue)
            {
                flg = true;//セットした値と違う
            }

			string sMessage = "データ送信完了\t/[IP]=" + sIPAddressNO + "  /[MemAddr]=" + sAddress + "  /[Value]=" + nValue.ToString();
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);


            return flg;
        }

		private int GetColOffset(int colLen, int refColLen)
		{
			int offset;
			if (colLen == refColLen)
			{
				offset = 0;
			}
			else
			{
				offset = COL_OFFSET_BY_ADDED_COL_TO_TOP;
			}
			return offset;
		}

        //[JE:製品出来栄え/1マガジン終了]
        private void DbInput_PLA_JEFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
        {
			Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			//ファイルの列を参照する時にPLA装置の謎仕様でオフセットしてしまう列数を取得 古川Sと相談し単純に列数を数えてオフセット量を切り替える仕様
			int offset = GetColOffset(GetFileColLength(textArray[FILE_MAGAZINEROW], ','), JE_COL_LEN);

            string[] recordArray = new string[] { };

			MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL + offset);
			if (MagInfo.sNascaLotNO == null)
            {
                MoveReserveFile(filePath, lsetInfo.AssetsNM);
                string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.EquipmentNO, lsetInfo.MachineNM) + "ロット番号が取得できなかった為、移動しました。ファイルパス：" + filePath;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
                errMessageList.Add(errMessageInfo);

                return;
            }

            //2015.2.12 車載3次 DB前, WB前の識別方法を名前のContains検索とChip_NM(ARMS作業CD)検索方法に変更
            //lsetInfo.ChipNM = GetProcessKB(MagInfo.sNascaLotNO);
            lsetInfo.ChipNM = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("JE", lsetInfo, MagInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                List<double> fileValueList = new List<double>();  //流量(MAX),(MIN)用         →5列目
                int nRowCnt = 0;                //行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 4 || recordArray[0] == "")//「1,2,3行目」「Logの最終行」は無視
                    {
                        continue;
                    }

					if (recordArray.Length == JE_COL_LEN + offset)
					{
						int nflg = Convert.ToInt32(recordArray[JE_COL_TARGET_FG + offset]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                        if (nflg == 0)
                        {
                            continue;//何もせずに次へ
                        }
                    }
					else
					{
						throw new ApplicationException("列数が未知のJEファイルを取得しました。未知のファイルフォーマットの可能性が有ります。正常なファイルで有る場合、システム管理者へ問い合わせて下さい。");
					}

					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[FILE_DATECOL + offset] + " " + recordArray[FILE_TIMECOL + offset]));

					//自動化 or ハイブリッドラインの場合
					if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
                            if (rtnArmsLotInfo == null)
                            {
                                return;
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
						fileValueInfo.LotNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//LotNo取得
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }
                    fileValueList.Add(Convert.ToDouble(recordArray[filefmtInfo.ColumnNO + offset]));    //流量(MAX),(MIN)用         →5列目
                }

				paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);

				//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD); //下記で置き換え
				plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

                if (paramInfo.sTotalKB == Constant.CalcType.MAX.ToString())
                {
                    fileValueInfo.TargetVAL = CalcMax(fileValueList);
                }
                else
                {
                    fileValueInfo.TargetVAL = CalcMin(fileValueList);
                }

                if (plmInfo != null)
                {
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, Convert.ToString(fileValueInfo.TargetVAL), fileValueInfo.MeasureDT, ref errMessageList);
                }
                else
                {
                    ConnectDB.InsertTnLOG(lsetInfo, plmInfo, MagInfo, Convert.ToString(fileValueInfo.TargetVAL), fileValueInfo.MeasureDT, "判定なし保管のみ");
                }
            }

            if (MagInfo.sNascaLotNO != null)
            {
                //int nprocno = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);//現在の作業工程取得
                //string sExtension = GetPLAMode(nprocno);
                //if (sExtension == Constant.sPLA_Model)

                //DB硬化前プラズマ洗浄
                if (lsetInfo.ChipNM == "DB0027")
				{
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:CheckQC");
					CheckQC(lsetInfo, 3, MagInfo.sMaterialCD);//3はPLADBの意味 JEファイル LE→EE→JEの順
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:CheckQC");
                }
                //WB前プラズマ洗浄
                else
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:CheckQC");
                    CheckQC(lsetInfo, 4, MagInfo.sMaterialCD);//4はPLAWBの意味 JEファイル LE→EE→JEの順
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:CheckQC");
                }
            }
        }
        //[EE:ｴﾗｰﾛｸﾞ/1マガジン終了]
        private void DbInput_PLA_EEFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

			//ファイルの列を参照する時にPLA装置の謎仕様でオフセットしてしまう列数を取得 古川Sと相談し単純に列数を数えてオフセット量を切り替える仕様
			int offset = GetColOffset(GetFileColLength(textArray[FILE_MAGAZINEROW], ','), EE_COL_LEN);

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL + offset);
            if (MagInfo.sNascaLotNO == null)
            {
                MoveReserveFile(filePath, lsetInfo.AssetsNM);
                string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.EquipmentNO, lsetInfo.MachineNM) + "ロット番号が取得できなかった為、移動しました。ファイルパス：" + filePath;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
                errMessageList.Add(errMessageInfo);

                return;
            }

            //2015.2.12 車載3次 DB前, WB前の識別方法を名前のContains検索とChip_NM(ARMS作業CD)検索方法に変更
            //lsetInfo.ChipNM = GetProcessKB(MagInfo.sNascaLotNO);
            lsetInfo.ChipNM = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("EE", lsetInfo, MagInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                int nRowCnt = 0;                //行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 4 || recordArray[0] == "")//「1,2,3行目」「Logの最終行」は無視
                    {
                        continue;
                    }

					if (recordArray.Length == EE_COL_LEN + offset)
					{
						int nflg = Convert.ToInt32(recordArray[EE_COL_TARGET_FG + offset]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
						if (nflg == 0)
                        {
                            continue;//何もせずに次へ
                        }
                    }

					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[FILE_DATECOL + offset] + " " + recordArray[FILE_TIMECOL + offset]));

					//自動化 or ハイブリッドラインの場合
					if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
                            if (rtnArmsLotInfo == null)
                            {
                                //MagInfo.sMagazineNO = sMagazineNo;
                                //MagInfo.sNascaLotNO = null;
                                //MagInfo.sMaterialCD = GetMaterialCD(EquiInfo.sEquipmentNO);
                                return;
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
						fileValueInfo.LotNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//LotNo取得
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);

					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

					fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO + offset].Trim();

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
        //[LE:消耗品/1マガジン終了]
        private void DbInput_PLA_LEFile(ref TcpClient tcp, ref NetworkStream ns, LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
        {
            Plm plmInfo = new Plm();
            ParamInfo paramInfo = new ParamInfo();
            MagInfo MagInfo = new MagInfo();

			//ファイルの列を参照する時にPLA装置の謎仕様でオフセットしてしまう列数を取得 古川Sと相談し単純に列数を数えてオフセット量を切り替える仕様
			int offset = GetColOffset(GetFileColLength(textArray[FILE_MAGAZINEROW], ','), LE_COL_LEN);

            string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL + offset);
			if (MagInfo.sNascaLotNO == null) 
            {
                MoveReserveFile(filePath, lsetInfo.AssetsNM);
                string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.EquipmentNO, lsetInfo.MachineNM) + "ロット番号が取得できなかった為、移動しました。ファイルパス：" + filePath;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
                errMessageList.Add(errMessageInfo);

                return;
            }

            //2015.2.12 車載3次 DB前, WB前の識別方法を名前のContains検索とChip_NM(ARMS作業CD)検索方法に変更
            //lsetInfo.ChipNM = GetProcessKB(MagInfo.sNascaLotNO);
            lsetInfo.ChipNM = ConnectDB.GetCurrentWork(this.LineCD, MagInfo.sNascaLotNO);

            //管理項目事にLOGに登録
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData("LE", lsetInfo, MagInfo.sMaterialCD);
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                int nRowCnt = 0;                //行ｶｳﾝﾄ
                bool fGetMagNo = false;

                FileValueInfo fileValueInfo = new FileValueInfo();
                foreach (string srecord in textArray)
                {
                    nRowCnt = nRowCnt + 1;
                    recordArray = srecord.Split(',');
                    if (nRowCnt < 4 || recordArray[0] == "")//「1,2,3行目」「Logの最終行」は無視
                    {
                        continue;
                    }

					if (recordArray.Length == LE_COL_LEN + offset)
					{
						int nflg = Convert.ToInt32(recordArray[LE_COL_TARGET_FG + offset]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
						if (nflg == 0)
                        {
                            continue;//何もせずに次へ
                        }
                    }
					else
					{
						throw new ApplicationException("列数が未知のLEファイルを取得しました。未知のファイルフォーマットの可能性が有ります。正常なファイルで有る場合、システム管理者へ問い合わせて下さい。");
					}


					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[FILE_DATECOL + offset] + " " + recordArray[FILE_TIMECOL + offset]));

					//自動化 or ハイブリッドラインの場合
					if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
                    {
						fileValueInfo.MagazineNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = CheckMagNo(fileValueInfo.MagazineNO);

                        if (fGetMagNo == false)//NASCA公開API、1回だけ実行
                        {
                            //<--Package 古川さん待ち             
							ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(lsetInfo.InlineCD, fileValueInfo.MagazineNO);
                            if (rtnArmsLotInfo == null)
                            {
                                return;
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
						fileValueInfo.LotNO = recordArray[FILE_MAGAZINECOL + offset].Trim();//LotNo取得
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
                        if (fGetMagNo == false)//1回だけ実行
                        {
                            MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
                            fGetMagNo = true;//初回のみSet
                        }
                    }

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
                    
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

					fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO + offset].Trim();

                    //管理項目をDB登録する
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

        #endregion 

		//#region HSMS

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		//#endregion

//        protected string GetProcessKB(string lotNO)
//        {
//            try
//            {
//#if SHASAI_UNITTEST
//                int nprocno = Constant.ARMS_DBOVENAFTER;
//#else
//                int nprocno = ConnectDB.GetCurrentWork(this.LineCD, lotNO);
//#endif
//                switch (nprocno)
//                {
//                    case Constant.ARMS_DBAFTER:
//                        return "DB";
//                    case Constant.ARMS_DBOVENAFTER:
//                        return "WB";
//                    default:
//                        throw new Exception("[ロット番号 " + lotNO + "]プラズマ装置の工程名が不明です。");
//                }
//            }
//            catch (Exception err) 
//            {
//                throw;
//            }
//        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lsetInfo"></param>
        public static void ClearPLAFolderPath(LSETInfo lsetInfo)
        {
            if (lsetInfo.InputFolderNM.Contains("LE"))
            {
                lsetInfo.InputFolderNM = Directory.GetParent(Directory.GetParent(lsetInfo.InputFolderNM).FullName).FullName + @"\";
            }
            if (lsetInfo.InputFolderNM.Contains("JE"))
            {
                lsetInfo.InputFolderNM = Directory.GetParent(Directory.GetParent(lsetInfo.InputFolderNM).FullName).FullName + @"\";
            }
            if (lsetInfo.InputFolderNM.Contains("EE"))
            {
                lsetInfo.InputFolderNM = Directory.GetParent(Directory.GetParent(lsetInfo.InputFolderNM).FullName).FullName + @"\";
            }

            return;
        } 
    }
}
