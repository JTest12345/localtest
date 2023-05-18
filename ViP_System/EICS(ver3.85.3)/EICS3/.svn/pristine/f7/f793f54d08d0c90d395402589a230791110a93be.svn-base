using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EICS.Structure;

namespace EICS.Machine
{
	public class WBMachineInfoNMC : WBMachineInfo
	{
		public const string ASSETS_NM = "Wire Bonder";

		private Constant.FrameSupplyStatus FrameSupplyStatus { get; set; }

		/// <summary>
		/// ファイルチェック(NMC)
		/// </summary>
		/// <returns></returns>
		public new void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				string sFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_START_NM);
				if (!Directory.Exists(sFolderPath))
				{
					//処理フォルダが無い場合、作成
					Directory.CreateDirectory(sFolderPath);
				}

				List<string> sFileList = MachineFile.GetPathList(sFolderPath, "^" + Convert.ToString(FileType.SP));
				if (sFileList.Count == 0)
				{
					//処理ファイルが無い場合、待機
					return;
				}

				foreach (string sFile in sFileList)
				{
					//スタートタイミング処理
					CheckStartTiming(sFile, lsetInfo, ref base.errorMessageList);

					if (base.errorMessageList.Count != 0)
					{
						//処理ファイルに異常がある場合、ローダーを閉める
						this.FrameSupplyStatus = Constant.FrameSupplyStatus.NG;
					}
					else
					{
						//処理ファイルが正常な場合、ローダーを開く
						this.FrameSupplyStatus = Constant.FrameSupplyStatus.OK;
					}
				}
			}
			catch (Exception err)
			{
				this.FrameSupplyStatus = Constant.FrameSupplyStatus.NG;
				throw;
			}
			finally
			{
#if Debug
#else
                //システム起動時から初回ファイル処理をするまではPLCに命令を送らない。
                if (FrameSupplyStatus != Constant.FrameSupplyStatus.Wait)
                {
					SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

                    using (PLC_Omron plc = new PLC_Omron(commonSettingInfo.LocalHostIP, lsetInfo.LoaderAddressNO,
                        Convert.ToByte(PLC_Omron.GetNodeAddress(lsetInfo.IPAddressNO)), lsetInfo.LoaderPlcNodeNO, commonSettingInfo.PLCReceiveTimeout))
                    {
                        //フレーム供給状態の命令を送る

                        //D900(フレーム供給前)
                        plc.ChangeFrameSupplyStatus(Constant.PlcMemory.D900, FrameSupplyStatus, ref base.errorMessageList);
                        //D910(フレーム供給後)
                        plc.ChangeFrameSupplyStatus(Constant.PlcMemory.D910, FrameSupplyStatus, ref base.errorMessageList);
                    }
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END] WB Check:{0}", FrameSupplyStatus), lsetInfo.EquipmentNO);
                }
#endif
			}
		}

		/// <summary>
		/// スタートタイミング処理(NMC)
		/// </summary>
		/// <param name="sFilePath"></param>
		/// <param name="lsetInfo"></param>
		/// <param name="errMessageList"></param>
		public void CheckStartTiming(string sFilePath, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();
			magInfo.sMagazineNO = "";
			magInfo.sNascaLotNO = null;

			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

			try
			{
				FileInfo sFileInfo = new FileInfo(sFilePath);

				//測定日時を取得
				string dtMeasureDT = Convert.ToString(GetFileName_MeasureDT(sFileInfo.Name));

				//SPファイル処理
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] WB SPFile"), lsetInfo.EquipmentNO);
				
				errMessageList.AddRange(DbInput_WB_SPFile_OLD(lsetInfo, magInfo, sFileInfo.FullName, dtMeasureDT, Constant.nStartTimmingNMC));
				
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END] WB SPFile"), lsetInfo.EquipmentNO);

				//移動先のパスを生成
				DirectoryInfo fromDir = new DirectoryInfo(sFileInfo.Directory.FullName);
				string equipDirPath = fromDir.Parent.FullName;
				string dateDirNM = sFileInfo.LastWriteTime.ToString("yyyy/MM").Replace("/", "");
				string moveToFilePath = Path.Combine(Path.Combine(equipDirPath, dateDirNM), sFileInfo.Name);

				//処理済みファイルを移動(中間サーバ→中間サーバ年月フォルダ)
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] WB SPFile Move"), lsetInfo.EquipmentNO);
				MoveMachineFileNMC(sFileInfo.FullName, moveToFilePath);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END] WB SPFile Move"), lsetInfo.EquipmentNO);
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public Constant.FrameSupplyStatus GetFrameSupplyStatus()
		{
			return FrameSupplyStatus;
		}

	}
}
