using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EICS.Database;
using System.IO;
using System.Threading;

namespace EICS.Machine
{
	class PLAMachineInfoDirectlyCom : PLAMachineInfo
	{
        //PLC_Melsec machinePLC;


		//private Dictionary<string, int> plaWorkCDProcNoDict = new Dictionary<string, int>();

		#region ADDRESS
		//■PC→プラズマ/////////////////////////////////////////
		/// <summary>
		/// パラメータ書込み完了フラグ
		/// </summary>
		public const string SET_PARAM_SETTING_OK = "B000200";
		/// <summary>
		/// 工程確認(ON＝DB後, OFF＝DBｵｰﾌﾞﾝ後) 
		/// </summary>
		public const string SET_PROCESS_TEACH = "B000201";
		/// <summary>
		/// 判定結果待ち受けフラグEE
		/// </summary>
		public const string SET_CHECK_EE_FLG = "B000211";
		/// <summary>
		/// 判定結果待ち受けフラグJE
		/// </summary>
		public const string SET_CHECK_JE_FLG = "B000212";
		/// <summary>
		/// 判定結果待ち受けフラグLE
		/// </summary>
		public const string SET_CHECK_LE_FLG = "B000213";
		/// <summary>
		/// RF入射
		/// </summary>
		public const string SET_PARAM_RF = "W000200";
		/// <summary>
		/// 処理時間
		/// </summary>
		public const string SET_PARAM_TIME = "W000202";
		/// <summary>
		/// Arガス流量
		/// </summary>
		public const string SET_PARAM_AR = "W000204";
		/// <summary>
		/// CF4ガス流量
		/// </summary>
		public const string SET_PARAM_CF = "W000205";
		/// <summary>
		/// 真空度設定
		/// </summary>
		public const string SET_PARAM_VACUUM = "W000206";
		/// <summary>
		/// 放電開始圧力設定
		/// </summary>
		public const string SET_PARAM_DISCHARGE = "W000208";

		/// <summary>
		/// 判定結果(OK = 0, NG = 1)
		/// </summary>
		//public const string SET_JUDGE = "W000210";

		/// <summary>
		/// EEファイル判定結果(OK = 1, NG = 0)
		/// </summary>
		public const string SET_EE_JUDGE = "W000211";

		/// <summary>
		/// JEファイル判定結果(OK = 1, NG = 0)
		/// </summary>
		public const string SET_JE_JUDGE = "W000212";

		/// <summary>
		/// LEファイル判定結果(OK = 1, NG = 0)
		/// </summary>
		public const string SET_LE_JUDGE = "W000212";

		//public const string SET_PLANTCD = "W000260";
		//public const string SET_PROCESSCD = "W000263";
		//public const string SET_TYPECD = "W000264";
		//public const string SET_INFOSEND_OK = "B000230";
		//public const string SET_JUDGENG_FG = "W000250";

		//■プラズマ→PC/////////////////////////////////////////
		/// <summary>
		/// マガジンＮｏ書込み完了フラグ
		/// </summary>
		//public const string GET_MAGAZINE_SETTING_OK = "B000300";

		/// <summary>
		/// 装置運転中(ON = 有効, OFF = 無効) (未使用 2013/8/2確認 nyoshimoto）
		/// </summary>
		public const string GET_PLA_READY = "B00030E";

		/// <summary>
		/// 傾向管理有効無効(ON = 有効. OFF = 無効) (未使用 2013/8/2確認 nyoshimoto）
		/// </summary>
		public const string GET_PLA_TRENDSWITCH = "B00030F";

		/// <summary>
		/// マガジンＮｏ(W000300～000305) ASCII12文字分
		/// </summary>
		//public const string GET_MAGAZINE = "W000300";
		public const string Get_UNLOAD_MAGAZINE = "";

		public const string GET_INSPECT_MAGAZINENO = "D4512";

		public const string GET_START_YEAR = "D4524";
		public const string GET_START_MONTH = "D4525";
		public const string GET_START_DAY = "D4526";
		public const string GET_START_HOUR = "D4527";
		public const string GET_START_MINUTE = "D4528";
		public const string GET_START_SECOND = "D4529";

		public const string GET_LE_VACUUM = "D4550";
		public const string GET_LE_MBOX = "D4551";
		public const string GET_LE_RF = "D4552";
		public const string GET_LE_SHIELD = "D4553";
		public const string GET_LE_CERAMIC = "D4554";
		public const string GET_LE_TREND = "D4555";

		public const string GET_EE_RF = "D4560";
		public const string GET_EE_PRESS = "D4561";
		public const string GET_EE_FLUX = "D4562";
		public const string GET_EE_VDC = "D4563";
		public const string GET_EE_CARRY = "D4564";
		public const string GET_EE_TREND = "D4565";


		//■プラズマ⇔PC/////////////////////////////////////////

		public const string PLA_DATAOUTCOMP = "B000220";

		#endregion

		#region ファイル内列位置

		public const int JEFILE_DATE_COL = 1;
		public const int JEFILE_TIME_COL = 2;
		public const int JEFILE_MAGAZINE_COL = 3;
		/// <summary>
		/// JE傾向管理まとめフラグ
		/// </summary>
		public const int JEFILE_ENABLE_COL = 14;
		/// <summary>
		/// JEファイルの全列数
		/// </summary>
		public const int JEFILE_TOTAL_COL_LEN = 15;

		/// <summary>
		/// EE傾向管理まとめフラグ
		/// </summary>
		public const int EEFILE_ENABLE_COL = 9;
		/// <summary>
		/// EEファイルの全列数
		/// </summary>
		public const int EEFILE_TOTAL_COL_LEN = 10;

		/// <summary>
		/// LE傾向管理まとめフラグ
		/// </summary>
		public const int LEFILE_ENABLE_COL = 9;
		/// <summary>
		/// LEファイルの全列数
		/// </summary>
		public const int LEFILE_TOTAL_COL_LEN = 10;

		#endregion


		#region PLA

		//ARMSのprocno情報 PLA用
		public const int ARMS_DBAFTER = 5;        //5	硬化前プラズマ洗浄	DB0027
		public const int ARMS_DBOVENAFTER = 8;    //8	プラズマ洗浄	WB0002
		public const string sPLA_Model = "：DB硬化前";//新たに追加されたDB硬化前の場合、WBは""

		//□装置設定ﾊﾟﾗﾒｰﾀｰ(FileIndex:-)----------------------------------------------------------------------
		public const string sPLA_SetRf = "RF入射(w)";              //装置にはそのまま
		public const string sPLA_SetTime = "処理時間(s)";            //装置には×10
		public const string sPLA_SetArGas = "Arガス流量(sccm)";       //装置には×10
		public const string sPLA_SetCfGas = "Cfガス流量";
		public const string sPLA_SetVacuum = "真空度設定(pa)";         //装置には×100
		public const string sPLA_SetPress = "放射開始圧力設定(pa)";   //装置には×100
		#endregion

		//public const int MAGAZINENO_WDEVICE_LEN = 10;
		public const int MAGAZINENO_DDEVICE_LEN = 10;

		/// <summary>プリフィックス種類</summary>
		private enum PrefixType
		{
			JE, LE, EE
		}

        public PLAMachineInfoDirectlyCom(int lineCD, string equipmentNo)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);
			//SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
			bool isEnablePreVerify = settingInfoPerLine.IsEnablePreVerify(equipmentNo);

			if (isEnablePreVerify)
			{
				InitAndCheck(lineCD);
			}
		}

		//private void InitAndCheck(int lineCD)
		//{
		//	GetPlaWorkCDProcNoDict(lineCD);
		//}

		//private Dictionary<string, int> GetPlaWorkCDProcNoDict(int lineCD)
		//{
		//	Dictionary<string, int> plaWorkCDProcNoDict = new Dictionary<string, int>();
		//	List<General> plaProcList = General.GetGeneralData(GEN_PLA_PROC, lineCD);

		//	if (plaProcList.Count == 0)
		//	{
		//		throw new ApplicationException(string.Format("TmGeneralのGeneralGrp_CD:{0}にGeneralCD:プラズマ装置作業CD⇔GeneralNM:装置送信Noの紐付け設定が必要です。", GEN_PLA_PROC));
		//	}

		//	foreach (General plaWorkProc in plaProcList)
		//	{
		//		int plaProcNo;
		//		if (int.TryParse(plaWorkProc.GeneralNM, out plaProcNo) == false)
		//		{
		//			throw new ApplicationException(string.Format("TmGeneralから取得したNM値が数値変換出来ませんでした。変換対象:{0}", plaWorkProc.GeneralNM));
		//		}

		//		plaWorkCDProcNoDict.Add(plaWorkProc.GeneralCD, plaProcNo);
		//	}

		//	return plaWorkCDProcNoDict;
		//}

        //public void ConnectPLC(string ipAddress, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
        //{
        //    if (machinePLC == null)
        //    {
        //        machinePLC = new PLC_Melsec(ipAddress, portNO, exclusionList);
        //    }
        //    else
        //    {
        //        if (machinePLC.ConnectedPLC() == false)
        //        {
        //            machinePLC.ConnectPLC();
        //        }
        //    }
        //}

        ~PLAMachineInfoDirectlyCom()
		{
            if (machinePLC != null)
                machinePLC.Dispose();
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
			StartingProcess(lsetInfo);
			EndingProcess(lsetInfo);
		}





		public override void EndingProcess(LSETInfo lsetInfo)
		{
            if (machinePLC.GetBitState(PLA_DATAOUTCOMP))
			{
				string ulMagazineNO = machinePLC.GetMagazineNo(GET_INSPECT_MAGAZINENO, MAGAZINENO_DDEVICE_LEN);

				JEProcess(lsetInfo, ulMagazineNO);

				EEProcess(lsetInfo, ulMagazineNO);

				LEProcess(lsetInfo, ulMagazineNO);

                machinePLC.SetBit(PLA_DATAOUTCOMP, 1, "0");
			}
		}

		public void JEProcess(LSETInfo lsetInfo, string ulMagazineNO)
		{
			//ファイルパス生成
			string filePath = Path.Combine(lsetInfo.InputFolderNM, string.Format("log_JE{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
			
			//JEファイル作成
			string[] jeDataArray = CreateJEData(lsetInfo.InlineCD, filePath, ulMagazineNO);

			//JEファイル処理
			JEFileProcess(lsetInfo, jeDataArray, filePath, ref base.errorMessageList);

            //処理済みファイルを保管フォルダへ移動
            MoveCompleteMachineFile(filePath, lsetInfo, "", "");
		}

		public void EEProcess(LSETInfo lsetInfo, string ulMagazineNO)
		{
			//ファイルパス生成
			string filePath = Path.Combine(lsetInfo.InputFolderNM, string.Format("log_EE{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));

			//EEファイル作成
			string[] eeDataArray = CreateEEData(lsetInfo.InlineCD, filePath, ulMagazineNO);

			//EEファイル処理
			EEFileProcess(lsetInfo, eeDataArray, filePath, ref base.errorMessageList);

            //処理済みファイルを保管フォルダへ移動
            MoveCompleteMachineFile(filePath, lsetInfo, "", "");
		}

		public void LEProcess(LSETInfo lsetInfo, string ulMagazineNO)
		{
			//ファイルパス生成
			string filePath = Path.Combine(lsetInfo.InputFolderNM, string.Format("log_LE{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));

			//LEファイル作成
			string[] leDataArray = CreateLEData(lsetInfo.InlineCD, filePath, ulMagazineNO);

			//LEファイル処理
			LEFileProcess(lsetInfo, leDataArray, filePath, ref base.errorMessageList);

            //処理済みファイルを保管フォルダへ移動
            MoveCompleteMachineFile(filePath, lsetInfo, "", "");
		}

		public string[] CreateJEData(int lineCD, string filePath, string ulMagazineNO)
		{
			//装置内のデータ格納アドレス情報を取得
            List<PLAJEAddr> plaJEAddrList = PLAJEAddr.GetPLAJEAddr(lineCD);

			List<string> jeDataList = new List<string>();

			int index = 1;

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.Default))
            {
                
			    jeDataList.Add("[LOGGING]");
                sw.WriteLine("[LOGGING]");

			    jeDataList.Add("");
                sw.WriteLine();

                string headerStr = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
				    "INDEX", "DATE", "TIME", "開始ﾏｶﾞｼﾞﾝNo", "ﾘｰﾄﾞﾌﾚｰﾑNo", "Ar流量(sccm)", "CF4流量", "圧力(Pa)", "入射電力 Pf(W)",
				    "反射波 Pr(W)", "Vdc(-V)", "Vpp(V)", "LOAD(%)", "PHASE(%)", "傾向管理まとめ");

			    jeDataList.Add(headerStr);
                sw.WriteLine(headerStr);

			    foreach (PLAJEAddr jeAddr in plaJEAddrList)
			    {
				    int year = machinePLC.GetWordAsDecimalData(jeAddr.Year);
				    int month = machinePLC.GetWordAsDecimalData(jeAddr.Month);
				    int day = machinePLC.GetWordAsDecimalData(jeAddr.Day);
				    int hour = machinePLC.GetWordAsDecimalData(jeAddr.Hour);
				    int minute = machinePLC.GetWordAsDecimalData(jeAddr.Minute);
				    int second = machinePLC.GetWordAsDecimalData(jeAddr.Second);

				    int frameNO = machinePLC.GetWordAsDecimalData(jeAddr.FrameNO);
				    int arVal = machinePLC.GetWordAsDecimalData(jeAddr.Ar);
				    int cf4Val = machinePLC.GetWordAsDecimalData(jeAddr.CF4);
#if SHASAI_UNITTEST
                    int exhaustVal = machinePLC.GetDoubleWordAsDecimalData(jeAddr.Exhaust);
#else
				    int exhaustVal = machinePLC.GetDoubleWordAsDecimalData(jeAddr.Exhaust);
#endif
                    int pfVal = machinePLC.GetWordAsDecimalData(jeAddr.Pf);
				    int prVal = machinePLC.GetWordAsDecimalData(jeAddr.Pr);
				    int vdcVal = machinePLC.GetWordAsDecimalData(jeAddr.Vdc);
				    int vppVal = machinePLC.GetWordAsDecimalData(jeAddr.Vpp);
				    int loadVal = machinePLC.GetWordAsDecimalData(jeAddr.Load);
				    int phaseVal = machinePLC.GetWordAsDecimalData(jeAddr.Phase);
				    int trend = machinePLC.GetWordAsDecimalData(jeAddr.TrendSummary);

                    if (frameNO == 0)
                    {
                        continue;
                    }

 #if SHASAI_UNITTEST
                    string lineData = string.Format("{0},{1}/{2}/{3},{4}:{5}:{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                        index, year, month, day, hour, minute, second, magInfo.sMagazineNO, frameNO, arVal, cf4Val, exhaustVal, pfVal, prVal, vdcVal, vppVal, loadVal, phaseVal, trend);
 #else
				    string lineData = string.Format("{0},{1}/{2}/{3},{4}:{5}:{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", 
					    index, year, month, day, hour, minute, second, ulMagazineNO, frameNO, arVal, cf4Val, exhaustVal, pfVal, prVal, vdcVal, vppVal, loadVal, phaseVal, trend);
 #endif

				    index++;

				    jeDataList.Add(lineData);

                    sw.WriteLine(lineData);
                }
			}

			return jeDataList.ToArray();
		}

		public string[] CreateEEData(int lineCD, string filePath, string ulMagazineNO)
		{
			List<string> eeDataList = new List<string>();

			int index = 1;

			eeDataList.Add("[LOGGING]");
			eeDataList.Add("");
			eeDataList.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
				"INDEX", "DATE", "TIME", "開始ﾏｶﾞｼﾞﾝNo", "RF出力異常", "圧力異常", "流量異常", "Vdc異常", "搬送異常", "傾向管理まとめ"));
			
			int year = machinePLC.GetWordAsDecimalData(GET_START_YEAR);
			int month = machinePLC.GetWordAsDecimalData(GET_START_MONTH);
			int day = machinePLC.GetWordAsDecimalData(GET_START_DAY);
			int hour = machinePLC.GetWordAsDecimalData(GET_START_HOUR);
			int minute = machinePLC.GetWordAsDecimalData(GET_START_MINUTE);
			int second = machinePLC.GetWordAsDecimalData(GET_START_SECOND);

			int rfErr = machinePLC.GetWordAsDecimalData(GET_EE_RF);
			int pressErr = machinePLC.GetWordAsDecimalData(GET_EE_PRESS);
			int fluxErr = machinePLC.GetWordAsDecimalData(GET_EE_FLUX);
			int vdcErr = machinePLC.GetWordAsDecimalData(GET_EE_VDC);
			int carryErr = machinePLC.GetWordAsDecimalData(GET_EE_CARRY);
			int trend = machinePLC.GetWordAsDecimalData(GET_EE_TREND);

			string lineData = string.Format("{0},{1}/{2}/{3},{4}:{5}:{6},{7},{8},{9},{10},{11},{12},{13}",
				index, year, month, day, hour, minute, second, ulMagazineNO, rfErr, pressErr, fluxErr, vdcErr, carryErr, trend);

			eeDataList.Add(lineData);

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.Default))
            {
                sw.WriteLine(lineData);
            }

			return eeDataList.ToArray();
		}

		public string[] CreateLEData(int lineCD, string filePath, string ulMagazineNO)
		{
			List<string> leDataList = new List<string>();

			int index = 1;

			leDataList.Add("[LOGGING]");
			leDataList.Add("");
			leDataList.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
				"INDEX", "DATE", "TIME", "開始ﾏｶﾞｼﾞﾝNo", "真空ポンプ", "M.BOX", "RF電源", "ｼｰﾙﾄﾞ板", "ｾﾗﾐｯｸ電極", "傾向管理まとめ"));

			int year = machinePLC.GetWordAsDecimalData(GET_START_YEAR);
			int month = machinePLC.GetWordAsDecimalData(GET_START_MONTH);
			int day = machinePLC.GetWordAsDecimalData(GET_START_DAY);
			int hour = machinePLC.GetWordAsDecimalData(GET_START_HOUR);
			int minute = machinePLC.GetWordAsDecimalData(GET_START_MINUTE);
			int second = machinePLC.GetWordAsDecimalData(GET_START_SECOND);

			int vacuumPump = machinePLC.GetWordAsDecimalData(GET_LE_VACUUM);
			int mBox = machinePLC.GetWordAsDecimalData(GET_LE_MBOX);
			int rf = machinePLC.GetWordAsDecimalData(GET_LE_RF);
			int shield = machinePLC.GetWordAsDecimalData(GET_LE_SHIELD);
			int ceramic = machinePLC.GetWordAsDecimalData(GET_LE_CERAMIC);
			int trend = machinePLC.GetWordAsDecimalData(GET_LE_TREND);

			string lineData = string.Format("{0},{1}/{2}/{3},{4}:{5}:{6},{7},{8},{9},{10},{11},{12},{13}",
				index, year, month, day, hour, minute, second, ulMagazineNO, vacuumPump, mBox, rf, shield, ceramic, trend);

			leDataList.Add(lineData);

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.Default))
            {
                sw.WriteLine(lineData);
            }

			return leDataList.ToArray();
		}

		//[JE:製品出来栄え/1マガジン終了]
		private void JEFileProcess(LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
		{
			Plm plmInfo = new Plm();
			ParamInfo paramInfo = new ParamInfo();
			MagInfo MagInfo = new MagInfo();

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			string[] recordArray = new string[] { };

			MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL);
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
            //GetProcessKB(MagInfo.sNascaLotNO);
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

                    if (recordArray.Length == JEFILE_TOTAL_COL_LEN)
                    {
                        int nflg = Convert.ToInt32(recordArray[JEFILE_ENABLE_COL]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
                        if (nflg == 0)
                        {
                            continue;//何もせずに次へ
                        }
                    }

					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[JEFILE_DATE_COL] + " " + recordArray[JEFILE_TIME_COL]));

					string magStr = recordArray[JEFILE_MAGAZINE_COL].Replace("\r", "").Replace("\"", "").Trim();//ﾏｶﾞｼﾞﾝNo取得
					string magIdentifier = magStr.Split(' ')[0];			

					//自動化 or ハイブリッドラインの場合
					//if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
					if (magIdentifier == Constant.DISCRIMINATION_MAGAZINE)
					{
						//fileValueInfo.MagazineNO = recordArray[JEFILE_MAGAZINE_COL].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = magStr;
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
					else if (magIdentifier == Constant.DISCRIMINATION_LOT)//人搬送の場合
					{
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
						fileValueInfo.LotNO = magStr;
#if SHASAI_UNITTEST
                        fileValueInfo.LotNO = "testtesttes";

#else
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
#endif
						if (fGetMagNo == false)//1回だけ実行
						{
#if SHASAI_UNITTEST
                            MagInfo = GetMagazineInfo(lsetInfo.EquipmentNO, DateTime.Now.ToString());
#else
							MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
#endif
							fGetMagNo = true;//初回のみSet
						}
					}
					else
					{
						throw new ApplicationException(string.Format(
							"ﾌｧｲﾙから取得されたﾏｶﾞｼﾞﾝ文字列のﾗﾍﾞﾙ識別部分が未定義の値です。ﾏｶﾞｼﾞﾝ文字列:{0} / 識別部:{1}", magStr, magIdentifier));
					}
					fileValueList.Add(Convert.ToDouble(recordArray[filefmtInfo.ColumnNO]));    //流量(MAX),(MIN)用         →5列目
				}

				paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
				
				//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, lsetInfo.EquipmentNO, MagInfo.sMaterialCD, this.LineCD);
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
				else if (lsetInfo.ChipNM == "WB0002")
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:CheckQC");
					CheckQC(lsetInfo, 4, MagInfo.sMaterialCD);//4はPLAWBの意味 JEファイル LE→EE→JEの順
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:CheckQC");
				}
				else if (lsetInfo.ChipNM == "MD0002")
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:CheckQC");
					CheckQC(lsetInfo, 11, MagInfo.sMaterialCD);//11はPLAMDの意味 JEファイル LE→EE→JEの順
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:CheckQC");
                }
                else if (lsetInfo.ChipNM == "MD0157")
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]PLA:CheckQC");
                    CheckQC(lsetInfo, Constant.TIM_PLAMDOV, MagInfo.sMaterialCD);//11はPLAMDの意味 JEファイル LE→EE→JEの順
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]PLA:CheckQC");
                }
			}
		}

		//[EE:ｴﾗｰﾛｸﾞ/1マガジン終了]
		private void EEFileProcess(LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
		{
			Plm plmInfo = new Plm();
			ParamInfo paramInfo = new ParamInfo();
			MagInfo MagInfo = new MagInfo();

			string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL);
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

					if (recordArray.Length == EEFILE_TOTAL_COL_LEN)
					{
						int nflg = Convert.ToInt32(recordArray[EEFILE_ENABLE_COL]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
						if (nflg == 0)
						{
							continue;//何もせずに次へ
						}
					}

					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));
					
					string magStr = recordArray[JEFILE_MAGAZINE_COL].Replace("\r", "").Replace("\"", "").Trim();//ﾏｶﾞｼﾞﾝNo取得
					string magIdentifier = magStr.Split(' ')[0];

					//自動化 or ハイブリッドラインの場合
					//if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
					if (magIdentifier == Constant.DISCRIMINATION_MAGAZINE)
					{
						//fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = magStr;
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
					//else//人搬送の場合
					else if (magIdentifier == Constant.DISCRIMINATION_LOT)//人搬送の場合
					{
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
						fileValueInfo.LotNO = magStr;
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
						if (fGetMagNo == false)//1回だけ実行
						{
							MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
							fGetMagNo = true;//初回のみSet
						}
					}
					else
					{
						throw new ApplicationException(string.Format(
							"ﾌｧｲﾙから取得されたﾏｶﾞｼﾞﾝ文字列のﾗﾍﾞﾙ識別部分が未定義の値です。ﾏｶﾞｼﾞﾝ文字列:{0} / 識別部:{1}", magStr, magIdentifier));
					}

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, lsetInfo.EquipmentNO, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

					fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

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
		private void LEFileProcess(LSETInfo lsetInfo, string[] textArray, string filePath, ref List<ErrMessageInfo> errMessageList)
		{
			Plm plmInfo = new Plm();
			ParamInfo paramInfo = new ParamInfo();
			MagInfo MagInfo = new MagInfo();

			string[] recordArray = new string[] { };

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			MagInfo = GetMagazineInfo(lsetInfo, textArray, FILE_MAGAZINEROW, FILE_MAGAZINECOL);
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

					if (recordArray.Length == LEFILE_TOTAL_COL_LEN)
					{
						int nflg = Convert.ToInt32(recordArray[LEFILE_ENABLE_COL]);//傾向管理無効なのでﾃﾞｰﾀﾍﾞｰｽには保存しない
						if (nflg == 0)
						{
							continue;//何もせずに次へ
						}
					}

					fileValueInfo.MeasureDT = Convert.ToString(Convert.ToDateTime(recordArray[1] + " " + recordArray[2]));

					string magStr = recordArray[JEFILE_MAGAZINE_COL].Replace("\r", "").Replace("\"", "").Trim();//ﾏｶﾞｼﾞﾝNo取得
					string magIdentifier = magStr.Split(' ')[0];

					//自動化 or ハイブリッドラインの場合
					//if ((settingInfo.LineType == Constant.LineType.Auto.ToString()) || (settingInfo.LineType == Constant.LineType.Hybrid.ToString()))
					if (magIdentifier == Constant.DISCRIMINATION_MAGAZINE)
					{
						//fileValueInfo.MagazineNO = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
						fileValueInfo.MagazineNO = magStr;
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
					//else//人搬送の場合
					else if (magIdentifier == Constant.DISCRIMINATION_LOT)//人搬送の場合
					{
						//fileValueInfo.LotNO = recordArray[3].Trim();//LotNo取得
						fileValueInfo.LotNO = magStr;
						fileValueInfo.LotNO = CheckLotNo(lsetInfo, fileValueInfo.LotNO);
						if (fGetMagNo == false)//1回だけ実行
						{
							MagInfo = SetMagInfo(MagInfo, lsetInfo.EquipmentNO, fileValueInfo.LotNO);
							fGetMagNo = true;//初回のみSet
						}
					}
					else
					{
						throw new ApplicationException(string.Format(
							"ﾌｧｲﾙから取得されたﾏｶﾞｼﾞﾝ文字列のﾗﾍﾞﾙ識別部分が未定義の値です。ﾏｶﾞｼﾞﾝ文字列:{0} / 識別部:{1}", magStr, magIdentifier));
					}

					paramInfo = ConnectDB.GetTvPRM_QcParamNO(this.LineCD, filefmtInfo.QCParamNO, Constant.s1Magazine);
					
					//plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, lsetInfo.EquipmentNO, MagInfo.sMaterialCD, this.LineCD);
					plmInfo = Plm.GetData(this.LineCD, MagInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

					fileValueInfo.TargetStrVAL = recordArray[filefmtInfo.ColumnNO].Trim();

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

		public void PLAInitSetting(string sModel, string sIPAddressNO, ref LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();

			SettingInfo commonSetting = SettingInfo.GetSingleton();

			string sMessage = "";

			//■マガジンNo取得"30 M000001"→"M000001"
			string sMagazineNo = machinePLC.GetMagazineNo(GET_MAGAZINE_ADDR(), GET_MAGAZINENO_WDEVICE_LEN());

#if SHASAI_UNITTEST
            magInfo = this.GetMagazineInfo(lsetInfo.EquipmentNO, DateTime.Now.ToString());
            magInfo.sMagazineNO = sMagazineNo;
#else
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
				magInfo.sMagazineNO = sMagazineNo;
				magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
				magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
			}
#endif

			//■設定値取得
            //2015.2.12 車載3次 DB前, WB前の識別方法を名前のContains検索とChip_NM(ARMS作業CD)検索方法に変更
#if SHASAI_UNITTEST
            int nprocno = Constant.ARMS_DBOVENAFTER;
#else
            string workCd = ConnectDB.GetCurrentWork(this.LineCD, magInfo.sNascaLotNO);//現在の作業工程取得
            lsetInfo.ChipNM = workCd;
#endif
            //string sExtension = GetPLAMode(nprocno);


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
			int nArGas = Convert.ToInt32(arPlm.ParameterMAX) * 10;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)


			//Cfガス流量
			Prm CfPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_CfParamNOList, workCd);
			Plm CfPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, CfPrm.ParamNO, lsetInfo.EquipmentNO, false);

			Plm.HasProblem(lsetInfo.InlineCD, CfPlm, ref paramErrMsg);
			int nCfGas = Convert.ToInt32(CfPlm.ParameterMAX) * 10;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)


			//真空度
			Prm vacuumPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_VacuumParamNOList, workCd);
			Plm vacuumPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, vacuumPrm.ParamNO, lsetInfo.EquipmentNO, false);

			Plm.HasProblem(lsetInfo.InlineCD, vacuumPlm, ref paramErrMsg);
			int nVacuum = Convert.ToInt32(vacuumPlm.ParameterMAX) * 100;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)


			//放射開始圧力
			Prm pressPrm = Prm.GetSendPrm(lsetInfo.InlineCD, commonSetting.PLA_PressParamNOList, workCd);
			Plm pressPlm = Plm.GetData(lsetInfo.InlineCD, rtnArmsLotInfo.TypeCD, sModel, pressPrm.ParamNO, lsetInfo.EquipmentNO, false);

			Plm.HasProblem(lsetInfo.InlineCD, pressPlm, ref paramErrMsg);
			int nPress = Convert.ToInt32(pressPlm.ParameterMAX) * 100;//将来的にTmPRMのChangeUnit_VALを使えるようにマスタ整備が完了したら変更(2015/6/16 nyoshimoto)

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

			sMessage = string.Format("ﾌﾟﾗｽﾞﾏ装置へのパラメタ送信完了。  /[設備号機]={0}  /[設備CD]={1}  /[IP]={2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, sIPAddressNO);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);


			//int nRF = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetRf, workCd, lsetInfo.InlineCD);

			//int nTime = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetTime, workCd, lsetInfo.InlineCD);
			//nTime = nTime * 10;

			//int nARGas = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetArGas, workCd, lsetInfo.InlineCD);
			//nARGas = nARGas * 10;

			//int nCFGas = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetCfGas, workCd, lsetInfo.InlineCD);
			//nCFGas = nCFGas * 10;

			//int nVacuum = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetVacuum, workCd, lsetInfo.InlineCD);
			//nVacuum = nVacuum * 100;

			//int nPress = ConnectDB.GetSettingParam(sModel, magInfo.sMaterialCD, sPLA_SetPress, workCd, lsetInfo.InlineCD);
			//nPress = nPress * 100;

			//■設定値書込み
			bool fNG = false;//falseは正常,trueの場合、書き込んだ値が書き込まれていない時。
			//RF入射
			if (SendData(SET_PARAM_RF, nRF) == true)
			{
				fNG = true;
			}

			// 処理時間
			if (SendData(SET_PARAM_TIME, nTime) == true)
			{
				fNG = true;
			}
			//Arガス流量
			if (SendData(SET_PARAM_AR, nArGas) == true)
			{
				fNG = true;
			}
			//
            if (SendData(SET_PARAM_CF, nCfGas) == true)
			{
				fNG = true;
			} 
			//真空度設定
			if (SendData(SET_PARAM_VACUUM, nVacuum) == true)
			{
				fNG = true;
			}
			//放電開始圧力設定
			if (SendData(SET_PARAM_DISCHARGE, nPress) == true)
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
//2015.2.12 車載では未使用の為、廃止
//            //■工程書込み
//            if (nprocno == Constant.ARMS_DBAFTER)
//            {//硬化前プラズマ洗浄 5
//                machinePLC.SetBit(SET_PROCESS_TEACH, 1, "1");//ON=DB後
//                //lsetInfo.ChipNM = "DB";
//            }
//            else if (nprocno == Constant.ARMS_DBOVENAFTER)
//            {//プラズマ洗浄 8
//                machinePLC.SetBit(SET_PROCESS_TEACH, 1, "0");//OFF=DBオーブン後
//                //lsetInfo.ChipNM = "WB";
//            }
//            else
//            {
//                sMessage = "ﾌﾟﾗｽﾞﾏ装置に停止命令を出しました。\r\nプラズマ装置の工程が不明です。\r\n/[IP]=" + sIPAddressNO + "[procno]=" + nprocno;
//                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
//                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
//                errMessageList.Add(errMessageInfo);
//#if Debug
//#else
//                PLASTOP(sIPAddressNO);
//#endif
//            }

			//☆
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "[1]設定 " + magInfo.sNascaLotNO + " " + lsetInfo.ChipNM);

			//■パラメータ書込み完了フラグON
			machinePLC.SetBit(SET_PARAM_SETTING_OK, 1, "1");
			//■マガジンNo書き込み完了フラグOFF
			machinePLC.SetBit(GET_MAGAZINE_SETTING_OK_ADDR(), 1, "0");
		}

		public bool SendData(string sAddress, int nValue)
		{
			bool isNG = false;//false=問題なし

			//値セット
			machinePLC.SetWordAsDecimalData(sAddress, nValue);
			//セットした値を確認
			if (machinePLC.GetWordAsDecimalData(sAddress) != nValue)
			{
				isNG = true;//セットした値と違う
			}
			return isNG;
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

			machinePLC.SetWordAsDecimalData(SET_JUDGE_ADDR(), 1);//OK=0,NG=1

			//■マガジンNo書き込み完了フラグOFF
			machinePLC.SetBit(GET_MAGAZINE_SETTING_OK_ADDR(), 1, "0");
		}
	}
}
