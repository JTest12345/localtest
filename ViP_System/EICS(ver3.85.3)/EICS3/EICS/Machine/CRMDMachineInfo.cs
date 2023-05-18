using EICS.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	/// <summary>
	/// 色調補正(CR:Color Revision MolD [SL47略称参照])
	/// x19ライン導入済み
	/// </summary>
	class CRMDMachineInfo : MDMachineInfo
	{


		protected override int OR_KEIKOUFG_COL_INDEX() { return 10;}

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
						//「受付準備OKをOFF」
						resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
						if (resMsg == "Error")
						{
							base.machineStatus = Constant.MachineStatus.Stop;
							return;
						}

						resMsg = kLinkInfo.KLINK_GetKV_RDS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.MAPINFO_MD_Memory, 1, Constant.ssuffix_U);
						int nMemoryMode = Convert.ToInt32(resMsg.Trim());

						if (nMemoryMode == 1)
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "■マッピング情報提供(LR) 開始", lsetInfo.EquipmentNO);

							//LRメモリへ書き込み
							SetUnMappingDataToLR(ref tcp, ref ns, lsetInfo, ref base.errorMessageList);
						}
						else
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "■マッピング情報提供(ZF) 開始", lsetInfo.EquipmentNO);

							//ZFメモリへ書き込み
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

		//[SM:製品出来栄え/1ﾏｶﾞｼﾞﾝ終了]
		protected override void DbInput_MD_SMFile(LSETInfo lsetInfo, MagInfo magInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList)
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
					FileValueInfo fileValueInfo = GetFileValue(fileLineValueList, filefmtInfo.ColumnNO, plmInfo.TotalKB);
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

		//未実装 2015/8/1
		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}

		private static FileValueInfo GetFileValue(string[] fileLineValueList, int valueColumnNO, string totalKB)
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


					if (fileValue.Length < (valueColumnNO + 1))
					{
						//指定列がファイル内容に存在しない場合、次へ
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

	}
}
