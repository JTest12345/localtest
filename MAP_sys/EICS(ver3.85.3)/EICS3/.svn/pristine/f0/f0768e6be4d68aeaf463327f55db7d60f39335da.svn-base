using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using EICS.Machine;
using System.IO;
using System.Net.Sockets;
using System.ComponentModel;

namespace EICS
{
    static class Program
    {
		

        /// <summary> 
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
		static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			SettingInfo settingInfo = SettingInfo.GetSingleton();
			RunningLog runLog = RunningLog.GetInstance();

            try
            {
#if DEBUG

#endif
				
				settingInfo.IsBatchMode = false;
                //設定ファイルの値を取得
				//Constant.SettingInfo = SettingInfo.GetInstance();// new SettingInfo();

                //設定ファイルから言語を取得
                //Constant.SettingInfo.GetLanguage(Constant.SETTING_FILE_PATH);

				string language = SettingInfo.GetLanguage(Constant.SETTING_FILE_PATH);

                //メッセージ言語の設定
				Constant.MessageInfo = (IMessage)System.Activator.CreateInstance(Type.GetType("EICS.Message" + language + "Info"));

				if (args.Length != 0)
				{
					if (args[0].Contains("/batch:"))
					{
						string batchNM = args[0].Replace("/batch:", "");

						BLCS_Library.LogHelper.EntryLog(batchNM, System.DateTime.Now.ToString(), null, null);

						settingInfo.IsBatchMode = true;

						if (File.Exists(args[1]))
						{
							settingInfo.GetSettingData(args[1]);
							log4netHelper.LogControl.SetBatchMode();
							BatchModeExecute(args);
						}
						else
						{
							BLCS_Library.LogHelper.EntryErrorLog(batchNM, BLCS_Library.ErrorLevel.MailInHouse, string.Format(Constant.MessageInfo.Message_146, args[1]), string.Empty);
							throw new ApplicationException(string.Format(Constant.MessageInfo.Message_146, args[1]));
						}
						BLCS_Library.LogHelper.EntryLog(batchNM, null, System.DateTime.Now.ToString(), null);
						return;
					}
					else if (args[0].Contains("/test"))
					{
					}
					else
					{
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_151, args[0]));
					}
				}

				if (args.Length > 0 && args[0].Contains("/test"))
				{
					settingInfo.GetSettingData(Constant.TEST_SETTING_FILE_PATH);
				}
				else
				{
					settingInfo.GetSettingData(Constant.SETTING_FILE_PATH);
				}

				SettingInfo.SettingFileTypeChanger();

				//InitChk.Run();

				//視覚言語の設定
				Thread.CurrentThread.CurrentUICulture =
					new System.Globalization.CultureInfo(language);
				Application.EnableVisualStyles();

				//多重起動確認
				if (KLinkInfo.CheckEICS())
				{
					MessageBox.Show(Constant.MessageInfo.Message_17, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

                if (settingInfo.LensFG == KLinkInfo.CheckLENS())
                {
                }
				else
				{
                    if (settingInfo.LensFG == true)
                    {
                        MessageBox.Show("LENS2を起動して下さい。");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("LENS2起動中にマッピングモードが有効なEICSは起動できません。");
                        return;
                    }
                }

				LENS2_Api.Config.LoadSetting();

				ArmsApi.Config.LoadSetting();

                Application.Run(new F01_MachineWatch());
            }
            catch (Exception err)
            {
				if (settingInfo.IsBatchMode)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, err.Message);
				}
				else
				{
					MessageBox.Show(err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
            }
        }

		private static void BatchModeExecute(string[] exeParamArray)
		{
			//多重起動確認
			if (KLinkInfo.CheckEICS())
			{
				throw new ApplicationException(Constant.MessageInfo.Message_17);
			}

			SettingInfo settingInfo = SettingInfo.GetSingleton();



			foreach (SettingInfo settingInfoPerLine in settingInfo.SettingInfoList)
			{
				List<EquipmentInfo> equipList = ConnectDB.GetEquipmentList(settingInfoPerLine.LineCD);

				foreach (EquipmentInfo equipInfo in equipList)
				{
					List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();

					if (equipInfo.ModelNM != "MD")
					{
						continue;
					}

					//設備情報を取得
					LSETInfo lsetInfo = ConnectDB.GetLSETInfo(equipInfo.LineNO, equipInfo.EquipmentNO);
					//lsetInfo.ChipNM = settingInfoPerLine.GetChipNM(equipInfo.EquipmentNO);
					lsetInfo.DirWBMagazine = settingInfoPerLine.GetWBMagazineDir(equipInfo.EquipmentNO);
					lsetInfo.ReportType = settingInfoPerLine.GetReportType(equipInfo.EquipmentNO);

					DateTime endDT = new DateTime();
					DateTime startDT = new DateTime();
					bool isSetedEndDT = false;

					for (int index = 1; index < exeParamArray.Length; index++ )
					{
						if (exeParamArray[index].StartsWith("/s:"))
						{
							string startDate = exeParamArray[index].Replace("/s:", "");
							string startTime = exeParamArray[index+1];

							if (DateTime.TryParse(string.Format("{0} {1}", startDate, startTime) , out startDT) == false)
							{
								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "/s:パラメタ異常");
							}
						}

						if (exeParamArray[index].StartsWith("/e:"))
						{
							isSetedEndDT = true;
							string endDate = exeParamArray[index].Replace("/e:", "");
							string endTime = exeParamArray[index+1];

							if (DateTime.TryParse(string.Format("{0} {1}", endDate, endTime), out endDT) == false)
							{
								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "/e:パラメタ異常");
							}
						}
					}

					if (isSetedEndDT == false)
					{
						endDT = DateTime.Now;
					}

					MDMachineInfo mdMachineInfo = (MDMachineInfo)MachineBase.GetMachineInfo(equipInfo);


					mdMachineInfo.ManualFileProcessing(lsetInfo, ref errMessageList, startDT, endDT);

					foreach (ErrMessageInfo errMessage in errMessageList)
					{
						string equipmentMessage = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
						if (!errMessage.MessageVAL.Contains(equipmentMessage))
						{
							//設備の情報が無い場合付け足す
							errMessage.MessageVAL = equipmentMessage + errMessage.MessageVAL;
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, errMessage.MessageVAL);
					}
				}
			}			
		}

    }
}
