using EICS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Machine
{
	class LMMachineInfoChipType : LMMachineInfo
	{
		/// <summary>ﾛｯﾄﾏｰｷﾝｸﾞ装置名</summary>
		public virtual string ASSETS_NM() { return "ﾁｯﾌﾟﾀｲﾌﾟﾛｯﾄﾏｰｷﾝｸﾞ装置"; }

		public override string TRG_Res_LM_START_OMRON() { return "HM402.00"; }
		public override string Res_LM_MAGAZINE_OMRON() { return "EM310000"; }
		public override string TEACH_Send_LM_MarkingNO_OMRON() { return "EM311000"; }
		public override string TEACH_Send_LM_Type_OMRON() { return "EM311032"; }
		public override string TEACH_Send_Startable_OMRON() { return "HM453.00"; }
		public string TEACH_Send_StartableNG = "HM453.01";

		/// <summary>
		/// ファイルチェック
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <returns>装置処理状態ステータス</returns>
		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

				List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
					= commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

				if (plc == null)
				{
					//if (settingInfoPerLine.LineType == Constant.LineType.High.ToString())
					//{
					//オムロンPLCを使用する場合
					plc = new PLC_Omron(commonSettingInfo.LocalHostIP, lsetInfo.IPAddressNO, Convert.ToByte(PLC_Omron.GetNodeAddress(lsetInfo.IPAddressNO)), lsetInfo.LoaderPlcNodeNO, commonSettingInfo.PLCReceiveTimeout);
					plcGetStringSwapFG = true;
					//}
					//else
					//{
					//	//三菱PLCを使用する場合
					//	plc = new PLC_Melsec(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
					//	plcGetStringSwapFG = false;
					//}
				}

				// 開始時処理

				MagInfo magInfo = new MagInfo();
				string markingStr = string.Empty;
				string lotMarkingStr = string.Empty;
				string printOutTime1 = string.Empty;
				string printOutTime2 = string.Empty;

				RunningLog runLog = RunningLog.GetInstance();

				// マガジン開始トリガ待ち
				if (plc.GetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TRG_Res_LM_START)) == "1")
				{
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
					magInfo = GetMagazineInfo(lsetInfo, sMagazineNO);

					//ロットが取れなかったNULLの場合、
					if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_129, lsetInfo.EquipmentNO, updateDT.ToString()));
					}

					//// ロットNoからロットマーキングNoを取得 2016.3.22 完全連番フラグを追加。湯浅
                    LotMark lotMarkData = Common.GetLotMarkingNO(lsetInfo.InlineCD, magInfo.sNascaLotNO
						, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup)
						, (Constant.LineType)Enum.Parse(typeof(Constant.LineType), settingInfoPerLine.LineType
                        , commonSettingInfo.FullSerialNoModeFG)); //車載の処理をハードコーディング（今後の方針についても考えておく） //この行のコメントアウトは元に戻す必要あり(2013/9/12 n.yoshimoto

                    string logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[印字文字取得]印字文字：{4} ロットNo：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkData.MarkNo, magInfo.sNascaLotNO);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
					runLog.logMessageQue.Enqueue(logMsg);

                    bool isInsertEnd = LotMark.CancelableInsert(lsetInfo.InlineCD, lotMarkData, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup), commonSettingInfo.FullSerialNoModeFG);

					if (isInsertEnd == false)
					{
						throw new ApplicationException("ロットNo、印字文字のデータベース登録時に異常が確認され、作業者にて処理が取り消されました。");
					}

					bool verifyLotMarkingStrResult = false;
					string recvLotMarkingStr;
					int tryCt = 1;

					do
					{
                        plc.SetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), lotMarkData.MarkNo);

                        recvLotMarkingStr = plc.GetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), lotMarkData.MarkNo.Length, plcGetStringSwapFG, false);

                        logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[送信印字文字と装置受信文字の照合] 送信文字：{4} 受信文字：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkData.MarkNo, recvLotMarkingStr);
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

					if (typeCondList.Count != 1)
					{
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_124,
							typeCondList.Count, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, typeCD, condCD));
					}

					int programNO;
					if (!int.TryParse(typeCondList.First().CondVAL, out programNO))
					{
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_125, typeCondList.First().CondVAL));
					}

					plc.SetWordAsDecimalData(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_Type), programNO);
					//*************  *************

					logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[プログラム番号送信完了]プログラム番号:{4}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, programNO);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
					runLog.logMessageQue.Enqueue(logMsg);
					
					//// 装置へ送信完了フラグを立てる
					plc.SetBit(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_Startable), 1, "1");

				}
			}
			catch (Exception err) 
			{
				plc.SetBit(TEACH_Send_StartableNG, 1, "1");

				throw;
			}
		}
	}
}
