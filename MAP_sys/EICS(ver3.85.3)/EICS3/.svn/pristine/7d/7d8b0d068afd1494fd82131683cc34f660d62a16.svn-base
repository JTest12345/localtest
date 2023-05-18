using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EICS.Database;
using System.Threading;
using EICS.Structure;
using System.IO;

namespace EICS.Machine
{
	class LMMachineInfo : MachineBase
	{
		/// <summary>ﾛｯﾄﾏｰｷﾝｸﾞ装置名</summary>
		public virtual string ASSETS_NM() { return "ﾛｯﾄﾏｰｷﾝｸﾞ装置"; }

		public bool plcGetStringSwapFG;

		public const string TRG_Res_LM_START_MELSEC = "B001C0A";// "H4010";//"B1C0A";//LM QRコードリーダ読取完了　確認
		public const string Res_LM_MAGAZINE_MELSEC = "W001C28";//"D12024";//"W1C28";//LM W1C28～W1C2DにマガジンNoが入ってくる？確認
		public const string TEACH_Send_LM_MarkingNO_MELSEC = "W001020";//LM 印字文字格納先　確認
		public const string TEACH_Send_LM_Type_MELSEC = "W001030";//タイプ
		public const string TEACH_Send_Startable_MELSEC = "B001020";//印字文字、品種情報送信完了信号
		
		//public const string TRG_Res_LM_START_OMRON = "CH60.10";// "H4010";//"B1C0A";//LM QRコードリーダ読取完了　確認
		//public const string Res_LM_MAGAZINE_OMRON = "DM12440";//"D12024";//"W1C28";//LM W1C28～W1C2DにマガジンNoが入ってくる？確認
		//public const string TEACH_Send_LM_MarkingNO_OMRON = "EM0332";//LM 印字文字格納先　確認
		//public const string TEACH_Send_LM_Type_OMRON = "EM0348";//タイプ
		//public const string TEACH_Send_Startable_OMRON = "HM122.00";//印字文字、品種情報送信完了信号

		public virtual string TRG_Res_LM_START_OMRON() { return "CH60.10"; }// "H4010";//"B1C0A";//LM QRコードリーダ読取完了　確認
		public virtual string Res_LM_MAGAZINE_OMRON() { return "DM12440"; }//"D12024";//"W1C28";//LM W1C28～W1C2DにマガジンNoが入ってくる？確認
		public virtual string TEACH_Send_LM_MarkingNO_OMRON() { return "EM0332"; }//LM 印字文字格納先　確認
		public virtual string TEACH_Send_LM_Type_OMRON() { return "EM0348"; }//タイプ
		public virtual string TEACH_Send_Startable_OMRON() { return "HM122.00"; }//印字文字、品種情報送信完了信号

		public string GetPLCMemAddress(SettingInfo settingInfoPerLine, string MemAddr)
		{
			if (settingInfoPerLine.LineType == Constant.LineType.High.ToString())
			{
				switch (MemAddr)
				{
					case "TRG_Res_LM_START":
						return TRG_Res_LM_START_OMRON();
					case "Res_LM_MAGAZINE":
						return Res_LM_MAGAZINE_OMRON();
					case 	"TEACH_Send_LM_MarkingNO":
						return TEACH_Send_LM_MarkingNO_OMRON();
					case "TEACH_Send_LM_Type":
						return TEACH_Send_LM_Type_OMRON();
					case "TEACH_Send_Startable":
						return TEACH_Send_Startable_OMRON();
					default:
						throw new ApplicationException("未定義のPLCメモリアドレスが指定されていますシステム構築担当者へ連絡してください");
				}
			}
			else
			{
				switch (MemAddr)
				{
					case "TRG_Res_LM_START":
						return TRG_Res_LM_START_MELSEC;
					case "Res_LM_MAGAZINE":
						return Res_LM_MAGAZINE_MELSEC;
					case "TEACH_Send_LM_MarkingNO":
						return TEACH_Send_LM_MarkingNO_MELSEC;
					case "TEACH_Send_LM_Type":
						return TEACH_Send_LM_Type_MELSEC;
					case "TEACH_Send_Startable":
						return TEACH_Send_Startable_MELSEC;
					default:
						throw new ApplicationException("未定義のPLCメモリアドレスが指定されていますシステム構築担当者へ連絡してください");
				}
			}
		}


		/// <summary>Prefix_NM</summary>
		protected enum FileType
		{
			PLC
		}

		//PLC_Keyence plc;//本番では三菱？という話なので、量試後に修正必要!!
		//PLC_Melsec plc;
		public IPlc plc;
		//PLC_MI plcm;

		public LMMachineInfo()
		{

		}

		//Omron製PLCでの通信に変更しUDP通信の為、下記の仕組みは廃止 2015/5/9 nyoshimoto
		//public void ConnectPLC(string ipAddress, int portNO)
		//{
		//	if (plc == null)
		//	{
		//		SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
		//		//plc = new PLC_Melsec(ipAddress, portNO);
		//		plc = new PLC_Omron("", ipAddress, Convert.ToByte(GetNodeAddress(lsetInfo.IPAddressNO)), lsetInfo.LoaderPlcNodeNO, commonSettingInfo.PLCReceiveTimeout);
		//	}
		//	else
		//	{
		//		if (plc.ConnectedPLC() == false)
		//		{
		//			plc.ConnectPLC();
		//		}
		//	}
		//}

		~LMMachineInfo()
		{
			if(plc != null)
				plc.Dispose();
		}

		/// <summary>
        /// ファイルチェック
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <returns>装置処理状態ステータス</returns>
		public override void CheckFile(LSETInfo lsetInfo)
		{
			string logMsg = string.Empty;
			int programNO;

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
				= commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

			if (plc == null)
			{
				if (settingInfoPerLine.LineType == Constant.LineType.High.ToString())
				{
					//オムロンPLCを使用する場合
					plc = new PLC_Omron(commonSettingInfo.LocalHostIP, lsetInfo.IPAddressNO, Convert.ToByte(PLC_Omron.GetNodeAddress(lsetInfo.IPAddressNO)), lsetInfo.LoaderPlcNodeNO, commonSettingInfo.PLCReceiveTimeout);
					plcGetStringSwapFG = true;
				}
				else
				{
					//三菱PLCを使用する場合
					plc = new PLC_Melsec(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
					plcGetStringSwapFG = false;
				}
			}

			// 開始時処理

            MagInfo magInfo = new MagInfo();
			//string magazineNO = string.Empty;
			//string lotNO = string.Empty;
			string markingStr = string.Empty;
			string lotMarkingStr = string.Empty;
			//string typeCD = string.Empty;
			string printOutTime1 = string.Empty;
			string printOutTime2 = string.Empty;
            bool lMArmsHandShakeFG = settingInfoPerLine.GetLMArmsHandShakeFG(lsetInfo.EquipmentNO);

			RunningLog runLog = RunningLog.GetInstance();

			// マガジン開始トリガ待ち
            if(plc.GetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TRG_Res_LM_START)) == "1")
			{
                //LM装置はファイルでの傾向管理はしていないので出力フォルダはハンドシェイクのみに使用する。
                //ファイル送信時に毎回クリアするので他の用途には使えないことを注意。
				OutputEicsTrigFile(lsetInfo.InputFolderNM);

                logMsg = string.Format("{0} ライン:{1}/{2}/{3}/EICSENDファイル出力：{4}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.InputFolderNM);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                //※OmlonPLCでQR読込完了信号後、マガジンデータ等のデータ欠けが発生した為、待機する
                Thread.Sleep(1000);

                DateTime updateDT = DateTime.Now;

				// マガジンNo取得
				string sMagazineNO = plc.GetWord(GetPLCMemAddress(settingInfoPerLine, Constant.Res_LM_MAGAZINE), Constant.MAGAZINE_NO_LM_WORD_LENGTH);
                if (string.IsNullOrEmpty(sMagazineNO))
                {
                    string msg = string.Format("マガジン番号が空白 {0}", sMagazineNO);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, msg);

                    return;
                } 
				//magInfo = GetMagazineSingleInfo(lsetInfo.EquipmentNO, sMagazineNO, null, true);

				magInfo = GetMagazineInfo(lsetInfo, sMagazineNO);

				//log.logMessageQue.Enqueue("GetMagazineSingleInfo通過");

				PLCParameterProcess(magInfo, lsetInfo);
                
				//log.logMessageQue.Enqueue("PLCParameterProcess通過");

                //ロットが取れなかったNULLの場合、
                if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_129, lsetInfo.EquipmentNO, updateDT.ToString()));
                }

                //ARMSとのハンドシェイクをしないフラグを追加 2016.5.28
                if (lMArmsHandShakeFG == true && IsArmsEnd(lsetInfo.InputFolderNM, 10, 1500) == false)
                {
                    throw new ApplicationException("ARMSの処理完了通知(ARMSENDﾌｧｲﾙ形式)を取得出来ません。ARMSの監視状態がOnになっているか、ARMSでｴﾗｰが発生していないか確認して下さい。");
                }
                				
				//// ロットNoからロットマーキングNoを取得   2016.3.22 完全連番フラグを追加。湯浅。
				LotMark lotMarkData = Common.GetLotMarkingNO(lsetInfo.InlineCD, magInfo.sNascaLotNO
					, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup)
					, (Constant.LineType)Enum.Parse(typeof(Constant.LineType), settingInfoPerLine.LineType)
                    , commonSettingInfo.FullSerialNoModeFG, commonSettingInfo.FullSerialNoMarkingDigit); //車載の処理をハードコーディング（今後の方針についても考えておく） //この行のコメントアウトは元に戻す必要あり(2013/9/12 n.yoshimoto

                logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[印字文字取得]印字文字：{4} ロットNo：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkData.MarkNo, lotMarkData.LotNo);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				runLog.logMessageQue.Enqueue(logMsg);

                bool isInsertEnd = LotMark.CancelableInsert(lsetInfo.InlineCD, lotMarkData, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup), commonSettingInfo.FullSerialNoModeFG);

				if (isInsertEnd == false)
				{
					throw new ApplicationException("ロットNo、印字文字のデータベース登録時に異常が確認され、作業者にて処理が取り消されました。");
				}

				//// 装置へロットマーキングNoを送信
				//plc.SendString(Constant.TEACH_Send_LM_MarkingNO, lotMarkingStr); //PLC処理を三菱経由からOMRON直としたので本行はコメントアウト 2015/5/9 nyoshimoto

				//※三菱、オムロンどちらでも使えるように変更する
				bool verifyLotMarkingStrResult = false;
				string recvLotMarkingStr;
				int tryCt = 1;

				do
				{
                    plc.SetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), lotMarkData.MarkNo);

                    recvLotMarkingStr = plc.GetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), lotMarkData.MarkNo.Length, plcGetStringSwapFG, false);

					logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[送信印字文字と装置受信文字の照合] 送信文字：{4} 受信文字：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkingStr, recvLotMarkingStr);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                    if (lotMarkData.MarkNo == recvLotMarkingStr)
					{
						verifyLotMarkingStrResult = true;
					}

				} while (verifyLotMarkingStrResult == false && tryCt++ < 3);

				if (verifyLotMarkingStrResult == false)
				{
					throw new ApplicationException(
                        string.Format("ロットマーキング文字送信時異常終了：送信した印字文字({0})と装置が認識した印字文字({1})が異なるため", lotMarkData.MarkNo, recvLotMarkingStr));
				}

				logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[印字文字送信完了]", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				runLog.logMessageQue.Enqueue(logMsg);

				//************* 装置へタイプNoを送信するためのコード *************
				string typeCD = magInfo.sMaterialCD;
				int condCD = (int)Constant.TypeConditionCD.LM_ProgramNo;
				List<TypeCond> typeCondList = TypeCond.GetTypeCond(lsetInfo.InlineCD, lsetInfo.ModelNM, typeCD, condCD);

				//log.logMessageQue.Enqueue("GetTypeCond通過");

				if (typeCondList.Count != 1)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_124,
						typeCondList.Count, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, typeCD, condCD));
				}

				if (!int.TryParse(typeCondList.First().CondVAL, out programNO))
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_125, typeCondList.First().CondVAL));
				}

				plc.SetWordAsDecimalData(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_Type), programNO);
				//*************  *************

				//// 装置へ送信完了フラグを立てる
				plc.SetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_Startable), 1, "1");



				logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[プログラム番号送信完了]プログラム番号:{4}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, programNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				runLog.logMessageQue.Enqueue(logMsg);

                DateTime sendedCompleteDT = DateTime.Now;

                while (plc.GetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TRG_Res_LM_START)) == "1")
                {
                    System.Threading.Thread.Sleep(100);

                    if ((DateTime.Now - sendedCompleteDT).Seconds >= 5)
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_119, lsetInfo.EquipmentNO));
                    }

                }

                //ハンドシェイク用ファイル(ARMSEND/EICSEND)の削除タイミングを、ハンドシェイク確認後から
                //装置への送信完了確定後に変更。2016.5.25
                DeleteTrigFile(lsetInfo.InputFolderNM);
                logMsg = string.Format("{0} ライン:{1}/{2}/{3}/EICS・ARMSENDファイル削除：{4}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.InputFolderNM);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                // 印字文字を空白でリセット（送信完了フラグが立ち上がったままになると、無関係な印字文字を装置が取得する恐れがあるためという提言で追加
                // 印字文字送信後のフラグ下げで、装置がランダムにマーキング開始時に空白文字を取り込んでしまう不具合が発生した為、フラグ下げ後に文字送信する順番に入れ替え(2013/9/13 n.yoshimoto)
                //// 装置へ送信完了フラグを下げる
                System.Threading.Thread.Sleep(500);
                plc.SetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_Startable), 1, "0");      //ARMS PLCクラス仮実装のため　一時的にコメントアウト 2013/9/12 n.yoshimoto

                System.Threading.Thread.Sleep(500);
				
				//plc.SendString(Constant.TEACH_Send_LM_MarkingNO, "     ");
				plc.SetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), "     ");
			}
		}

		private void OutputEicsTrigFile(string outputDir)
		{
			if (Directory.Exists(outputDir) == false)
			{
				Directory.CreateDirectory(outputDir);
			}

			string[] files = Directory.GetFiles(outputDir);

			foreach (string filePath in files)
			{
				File.Delete(filePath);
			}

			string fileName = string.Format("EICSEND{0}.CSV", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
			StreamWriter sw = new StreamWriter(Path.Combine(outputDir, fileName), true, Encoding.UTF8);

			sw.Close();
		}

        //該当フォルダのARMSEND/EICSENDファイルを全削除する。2016.5.25
        private void DeleteTrigFile(string outputDir)
        {

            string[] files = Directory.GetFiles(outputDir);

            foreach (string filePath in files)
            {
                File.Delete(filePath);
            }

        }

		private void PLCParameterProcess(MagInfo magInfo, LSETInfo lsetInfo)
		{
			AlertLog alertLog = AlertLog.GetInstance();
			string parameterVAL = string.Empty;
            string sHour = string.Empty;
            string sTime = string.Empty;
            string sMinute = string.Empty;
            string sSecond = string.Empty;

			//マスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.PLC), lsetInfo, magInfo.sMaterialCD);
			if (filefmtList.Count == 0)
			{
				//設定されていない場合、装置処理停止
				string message = string.Format(Constant.MessageInfo.Message_114, magInfo.sMaterialCD, Convert.ToString(FileType.PLC));
				throw new Exception(message);
			}

			foreach (FILEFMTInfo filefmtInfo in filefmtList)
			{
				if (string.IsNullOrEmpty(filefmtInfo.MachinePrefixNM))
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_115, magInfo.sMaterialCD, Convert.ToString(FileType.PLC), filefmtInfo.QCParamNO);
					throw new Exception(message);
				}
				// 印字時間を装置から取得 印字時間である前提の処理になっているので、他の管理項目が入ってきた場合は改修必要！！
                // 時間で管理（分と秒の単位まであるが）時間は65535時間までしか管理できない（装置がその仕様との事）
                //　時間が65535時間までしか管理できないことの懸念については、湯浅さん・片山さん・SGA2白浜さんに提言済み（2013/7/23　n.yoshimoto）
                //sTime = plc.GetData(filefmtInfo.MachinePrefixNM, 1, Constant.ssuffix_H, false);
                //sTime = plc.GetWord(filefmtInfo.MachinePrefixNM);

				//sHour = plc.GetRawData(filefmtInfo.MachinePrefixNM);
				sHour = plc.GetWordAsDecimalData(filefmtInfo.MachinePrefixNM, 1).ToString();

                //sHour = sTime.Substring(0, 4);

				parameterVAL = sHour;
				//parameterVAL = Convert.ToInt32(sHour, 16).ToString();
				
				//閾値マスタ情報(TmPLM)取得
				//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, lsetInfo.EquipmentNO, magInfo.sMaterialCD, this.LineCD);
				Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, lsetInfo.EquipmentNO, false);

				if (plmInfo == null)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
					throw new Exception(message);
				}

				//パラメータ値判定
                string errMessageVAL = ParameterInfo.CheckParameter(plmInfo, parameterVAL, lsetInfo, magInfo.sNascaLotNO);
				if (!string.IsNullOrEmpty(errMessageVAL))
				{
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(errMessageVAL, Color.Red);
					base.errorMessageList.Add(errMessageInfo);
					
					alertLog.logMessageQue.Enqueue(errMessageVAL);
				}

				//履歴保存
				Database.Log log = new Database.Log(this.LineCD);
				log.GetInsertData(lsetInfo, magInfo, plmInfo, parameterVAL, errMessageVAL, System.DateTime.Now);
				log.Insert();
			}
		}

        //protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
        //{
        //    throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
        //}

	}
}
