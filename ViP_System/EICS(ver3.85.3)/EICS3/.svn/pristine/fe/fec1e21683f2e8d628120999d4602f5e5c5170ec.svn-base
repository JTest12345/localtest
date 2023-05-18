using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using EICS.Database;
using System.Threading;
using System.Text.RegularExpressions;

namespace EICS
{
    public partial class SettingInfo
    {
		//変数やプロパティは量が増えてきたので宣言をSettinInfoParam.csへ記載
		private static Mutex mutex = new Mutex(false, "SettingMutex");

		private SettingInfo()
		{
		}

		public static SettingInfo GetSingleton()
		{
			try
			{
				mutex.WaitOne();

				if (settingInfo == null)
				{
					settingInfo = new SettingInfo();
				}

				mutex.ReleaseMutex();
			}
			catch (Exception err)
			{
				throw;
			}

			return settingInfo;
		}

		private void GetPLAParamNoSetting(XmlNode doc)
		{
			PLA_RfParamNOList = GetPLAParamNoList(doc, "PLA_RfParamNo");
			PLA_TimeParamNOList = GetPLAParamNoList(doc, "PLA_TimeParamNo");
			PLA_ArParamNOList = GetPLAParamNoList(doc, "PLA_ArParamNo");
			PLA_CfParamNOList = GetPLAParamNoList(doc, "PLA_CfParamNo");
			PLA_VacuumParamNOList = GetPLAParamNoList(doc, "PLA_VacuumParamNo");
			PLA_PressParamNOList = GetPLAParamNoList(doc, "PLA_PressParamNo");
		}

        //スパッタ関連の必要情報を設定ファイルから読み込み
        private void GetSUPNoSetting(XmlNode doc)
        {
            SUP_AutoFileRf1ParamNo = GetIntVal(doc, "SUP_AutoFileRfParam", "RF1QcParam_no");
            SUP_AutoFileRf2ParamNo = GetIntVal(doc, "SUP_AutoFileRfParam", "RF2QcParam_no");
            SUP_AutoFileRfExclusionTemp = GetIntVal(doc, "SUP_AutoFileRfParam", "exclusionTemp");

            SUP_AutoFileReflect1ParamNo = GetIntVal(doc, "SUP_AutoFileReflectParam", "RF1QcParam_no");
            SUP_AutoFileReflect2ParamNo = GetIntVal(doc, "SUP_AutoFileReflectParam", "RF2QcParam_no");
            SUP_AutoFileReflectCheckCount = GetIntVal(doc, "SUP_AutoFileReflectParam", "continuallyTime");

            SUP_AutoFileGusTentionParamNo = GetIntVal(doc, "SUP_AutoFileGusTentionParam", "QcParam_no");
            SUP_AutoFileGusTentionCheckCount = GetIntVal(doc, "SUP_AutoFileGusTentionParam", "continuallyTime");

            SUP_AutoFileGusFlowParamNo = GetIntVal(doc, "SUP_AutoFileGusFlowParamNo", "QcParam_no");

            SUP_OnceVdcChangeRf1ParamNo = GetIntVal(doc, "SUP_OnceVdcChangeParam", "RF1QcParam_no");
            SUP_OnceVdcChangeRf2ParamNo = GetIntVal(doc, "SUP_OnceVdcChangeParam", "RF2QcParam_no");
            SUP_OnceVdcChengeRate = GetIntVal(doc, "SUP_OnceVdcChangeParam", "rate");

            SUP_MultiVdcChangeRf1ParamNo = GetIntVal(doc, "SUP_MultiVdcChangeParam", "RF1QcParam_no");
            SUP_MultiVdcChangeRf2ParamNo = GetIntVal(doc, "SUP_MultiVdcChangeParam", "RF2QcParam_no");
            SUP_MultiVdcChangeRate = GetIntVal(doc, "SUP_MultiVdcChangeParam", "rate");
            SUP_MultiVdcChangeCount = GetIntVal(doc, "SUP_MultiVdcChangeParam", "count");

            SUP_VdcChangeExclusionTime = GetIntVal(doc, "SUP_VdcChangeExclusionTime", "value");

            SUP_ProgramParamNo = GetIntVal(doc, "SUP_ProgramParamNo", "value");
        }
        

		private List<int> GetPLAParamNoList(XmlNode doc, string nodeNm)
		{
			List<int> paramNoList = new List<int>();
			string paramsStr = MustGetStrVal(doc, nodeNm);

			string[] paramNoArray = paramsStr.Replace(" ", "").Split(',');

			foreach (string paramNoStr in paramNoArray)
			{
				int paramNo;
				if (int.TryParse(paramNoStr, out paramNo) == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, nodeNm, paramNoStr));
				}
				paramNoList.Add(paramNo);
			}

			return paramNoList;
		}

		/// <summary>
		/// ライン毎のSettingInfoインスタンスの取得
		/// </summary>
		/// <param name="lineCD"></param>
		/// <returns></returns>
		public static SettingInfo GetSettingInfoPerLine(int lineCD)
		{
			SettingInfo lineSetting;

			try
			{
				mutex.WaitOne();

				if (settingInfo == null)
				{
					settingInfo = new SettingInfo();
				}

				lineSetting = settingInfo.SettingInfoList.Find(s => s.LineCD == lineCD);

				mutex.ReleaseMutex();

			}
			catch (Exception err)
			{
				throw;
			}

			return lineSetting;
		}

        /// <summary>
        /// 型番を取得
        /// </summary>
        /// <param name="plantCD"></param>
        /// <returns></returns>
        public string GetMaterialCD(string plantCD) 
        {
			try
			{
				EquipmentInfo equipmentInfo = this.EquipmentList.Find(e => e.EquipmentNO == plantCD);
				if (equipmentInfo == null)
				{
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_84, plantCD));
                }

				return equipmentInfo.TypeCD;
			}
			catch (Exception err)
			{
				throw;
			}
        }

		/// <summary>
		/// まとめ型番を取得
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetTypeGroupCD(string plantCD)
		{
			try
			{
				EquipmentInfo equipmentInfo = this.EquipmentList.Find(e => e.EquipmentNO == plantCD);
				if (equipmentInfo == null)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_84, plantCD));
				}

				return equipmentInfo.TypeGroupCD;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// 対象設備の型番を変更する
		/// </summary>
		/// <param name="plantCD">設定・変更しない場合は空白</param>
		/// <param name="mateialCD">設定・変更しない場合は空白</param>
		/// <param name="bmCountFG">設定・変更しない場合はnull値</param>
		/// <returns></returns>
		public static bool SetMachineSetting(string plantCD, string mateialCD, string chipNM, string bmCountFG, string isOutputNasFile)
        {
			mutex.WaitOne();

            FileInfo fileInfo = new FileInfo(Constant.SETTING_FILE_PATH);
            if (!fileInfo.Exists)
            {
                MessageBox.Show(Constant.SETTING_FILE_PATH + Constant.MessageInfo.Message_10, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (fileInfo.IsReadOnly)
            {
                MessageBox.Show(Constant.MessageInfo.Message_11 + Constant.MessageInfo.Message_12, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return false;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fileInfo.FullName);

            XmlNode targetNode = null;
            XmlNodeList nodes = doc.SelectNodes(LINEINFO_NODE + "/EquipmentList/Equipment");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["no"].Value == plantCD)
                {
                    targetNode = node;
                }
            }
            if (targetNode == null)
            {
                MessageBox.Show(string.Format(Constant.MessageInfo.Message_13, plantCD), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (mateialCD != "")
            {
                targetNode.Attributes["value"].Value = mateialCD;
            }
            if (chipNM != "")
            {
                targetNode.Attributes["chip"].Value = chipNM;
            }
			if (bmCountFG != "")
			{
				targetNode.Attributes["BMCountFG"].Value = bmCountFG;
			}
			if (isOutputNasFile != "")
			{
				targetNode.Attributes["IsOutputNasFile"].Value = isOutputNasFile;
			}

            doc.Save(fileInfo.FullName);

			mutex.ReleaseMutex();

            return true;
        }


        /// <summary>
        /// 設定ファイル(QCIL.xml)の全データを取得する
        /// </summary>
        /// <param name="path"></param>
        public void GetSettingData(string path)
        {
			mutex.WaitOne();

			FileEncode = Encoding.GetEncoding("Shift_jis");	

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

			SettingInfoList = new List<SettingInfo>();

			GetLotNoConvSetting(doc.SelectSingleNode("//qcil_info"));

			Language = Convert.ToString(doc.SelectSingleNode("//Language").Attributes["value"].Value);
			NetExePath = Convert.ToString(doc.SelectSingleNode("//NetExePath").Attributes["value"].Value);
				GetResultPriorityJudge(doc);
				GetResultPriorityJudge(doc);

			if (doc.SelectSingleNode("//SectionCD") != null)
			{
				SectionCD = Convert.ToString(doc.SelectSingleNode("//SectionCD").Attributes["value"].Value);
			}
			else
			{
				throw new ApplicationException("//SectionCDが設定されていません。");
			}


			GetPlcExtractExclusion(doc);

			GetPLAParamNoSetting(doc.SelectSingleNode("//qcil_info"));

            GetSUPNoSetting(doc.SelectSingleNode("//qcil_info"));
                
            GetLotNoConvSetting(doc.SelectSingleNode("//qcil_info"));

			GetHonbanDBInfo(doc);

			if (doc.SelectSingleNode("//MachineLogOutWaitmSec") != null)
			{
				int waitmSec;
				string sWaitmSec = doc.SelectSingleNode("//MachineLogOutWaitmSec").Attributes["value"].Value;
				if (int.TryParse(sWaitmSec, out waitmSec) == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "//MachineLogOutWaitmSec", sWaitmSec));
				}
				MachineLogOutWaitmSec = waitmSec;
			}
			else
			{
				MachineLogOutWaitmSec = DEFAULT_MACHINE_LOT_WAIT_SEC;
			}
				
			if (doc.SelectSingleNode("//DBMachineLogOutWaitmSec") != null)
			{
				int waitmSec;
				string sWaitmSec = doc.SelectSingleNode("//DBMachineLogOutWaitmSec").Attributes["value"].Value;
				if (int.TryParse(sWaitmSec, out waitmSec) == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "//DBMachineLogOutWaitmSec", sWaitmSec));
				}
				DBMachineLogOutWaitmSec = waitmSec;
			}
			else
			{
				DBMachineLogOutWaitmSec = DEFAULT_DB_MACHINE_LOT_WAIT_SEC;
			}

            //<--後工程合理化/エラー集計
            if (doc.SelectSingleNode("//PostProcessMonitoringCycleSec") != null)
            {
                int waitmSec;
                string sWaitmSec = doc.SelectSingleNode("//PostProcessMonitoringCycleSec").Attributes["value"].Value;
                if (int.TryParse(sWaitmSec, out waitmSec) == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "//PostProcessMonitoringCycleSec", sWaitmSec));
                }
                PostProcessMonitoringCycleSec = waitmSec;
            }
            else
            {
                PostProcessMonitoringCycleSec = DEFAULT_ST_WAIT_SEC;//1800秒(30分)
            }
            //-->後工程合理化/エラー集計

            if (doc.SelectSingleNode("//ErrorCheckIntervalSec") != null)
			{
				int intervalSec;
				string sIntervalSec = doc.SelectSingleNode("//ErrorCheckIntervalSec").Attributes["value"].Value;
				if (int.TryParse(sIntervalSec, out intervalSec) == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "//ErrorCheckIntervalSec", sIntervalSec));
				}
				ErrorCheckIntervalSec = intervalSec;
			}
			else
			{
				ErrorCheckIntervalSec = DEFAULT_ERR_CHK_INTERVAL_SEC;
			}

            if (doc.SelectSingleNode("//DeleteSysLogIntervalDay") != null)
            {
                int intervalDay;
                string sIntervalDay = doc.SelectSingleNode("//DeleteSysLogIntervalDay").Attributes["value"].Value;
                if (int.TryParse(sIntervalDay, out intervalDay) == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "//DeleteSysLogIntervalDay", sIntervalDay));
                }
                DeleteSysLogIntervalDay = intervalDay;
            }
            else
            {
                DeleteSysLogIntervalDay = 0;
            }

			if (doc.SelectSingleNode("//PLCReceiveTimeout") != null)
			{
				PLCReceiveTimeout = Convert.ToInt32(doc.SelectSingleNode("//PLCReceiveTimeout").Attributes["value"].Value);
			}

			if (doc.SelectSingleNode("//MasterMissDisplayCount") != null)
			{
				MasterMissDisplayCount = GetIntVal(doc, "//MasterMissDisplayCount", "value");
			}
			else
			{
				MasterMissDisplayCount = Constant.DEFAULT_MASTER_MISS_DISPLAY_COUNT;
			}

			LocalHostIP = GetStrVal(doc, "//LocalHostIP");


			if (doc.SelectSingleNode("//IsMappingMode") == null)
			{
				throw new ApplicationException(string.Format("設定ファイルにIsMappingModeタグを追加して下さい。"));
			}
			string mappingModeStr = Convert.ToString(doc.SelectSingleNode("//IsMappingMode").Attributes["value"].Value);

			if (mappingModeStr.ToUpper() == "ON")
			{
				IsMappingMode = true;
			}
			else if (mappingModeStr.ToUpper() == "OFF")
			{
				IsMappingMode = false;
			}
			else
			{
				throw new ApplicationException(string.Format("設定ファイル：IsMappingModeの値が不正です。(ONもしくはOFFを設定）取得値：{0}", mappingModeStr));
			}

			if (doc.SelectSingleNode("//LENS") == null)
            {
                throw new ApplicationException(string.Format("設定ファイルの「configuration」タグの直下に「LENS」タグを追加して下さい。"));
            }
            string lensFgStr = Convert.ToString(doc.SelectSingleNode("//LENS").Attributes["value"].Value);

            if (lensFgStr.ToUpper() == "ON")
            {
                LensFG = true;
            }
            else if (lensFgStr.ToUpper() == "OFF")
            {
                LensFG = false;
            }
            else
            {
                throw new ApplicationException($"設定ファイル：「LENS」タグの値が不正です。(ONもしくはOFFを設定）取得値：{lensFgStr}");
            }

			//MDマッピングデータ照合機能追加で追加
			if (doc.SelectSingleNode("//DummyAIModelNM") == null)
			{
				throw new ApplicationException(string.Format("設定ファイルにDummyAIModelNMタグを追加して下さい。"));
			}
			DummyAIModelNM = Convert.ToString(doc.SelectSingleNode("//DummyAIModelNM").Attributes["value"].Value);

			LoadSettingPerLine(doc);

			string processCDPath = "//arms_info/AIProcessCD";
			XmlNode nodeAIProcessNO = doc.SelectSingleNode(processCDPath);
			if (nodeAIProcessNO == null)
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, processCDPath));
			}
			ArmsAIProcessNO = Convert.ToInt32(nodeAIProcessNO.Attributes["value"].Value);
            ArmsServerList = GetArmsServerList(doc);

			if (HasDebugSetting())
			{
				XmlDocument debugDoc = new XmlDocument();
				debugDoc.Load(DEBUG_SETTING_FILE_PATH);

				DebugEICSServer = GetDebugEICSServerSpecifiedLineCT(debugDoc);
				DebugEICSDatabase = GetDebugEICSDatabaseSpecifiedLineCT(debugDoc);

				DebugARMSServer = GetDebugARMSServer(debugDoc);
				DebugARMSDatabase = GetDebugARMSDatabase(debugDoc);
			}

            if (doc.SelectSingleNode("//OvenTemparatureControlConnectionString") != null)
            {
                OvenTemparatureControlConnectionString = GetStrVal(doc, "//OvenTemparatureControlConnectionString");
            }

            CheckDebugSetting();

			mutex.ReleaseMutex();
        }

		private void GetPlcExtractExclusion(XmlDocument doc)
		{
			settingInfo.PlcExtractExclusionList = new List<Machine.PLCDDGBasedMachine.ExtractExclusion>();
			XmlNodeList nodeList = doc.SelectNodes(PLC_EXTRACT_EXCLUSION_NODE);

			foreach (XmlNode node in nodeList)
			{
				EICS.Machine.PLCDDGBasedMachine.ExtractExclusion extractExclusion = new Machine.PLCDDGBasedMachine.ExtractExclusion();

				extractExclusion.ModelNm = node.Attributes["ModelNm"].Value;
				extractExclusion.DataType = node.Attributes["DataType"].Value;
				extractExclusion.ExceptionValue = node.Attributes["ExceptionVal"].Value;

				settingInfo.PlcExtractExclusionList.Add(extractExclusion);
			}
		}

		private void LoadSettingPerLine(XmlDocument doc)
		{
			for (int lineCount = 0; lineCount < doc.SelectNodes(LINEINFO_NODE).Count; lineCount++)
			{
				SettingInfo settingInfoPerLine = new SettingInfo();

				settingInfoPerLine.EquipmentList = GetEquipSetting(doc, lineCount);

				settingInfoPerLine.LineCD = Convert.ToInt32(doc.SelectNodes(LINEINFO_NODE)[lineCount].Attributes["inline_cd"].Value);

				settingInfoPerLine.LineType = GetLineTypeSettingSpecifiedLineCT(doc, lineCount);

				settingInfoPerLine.PackagePC = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("PackagePC").Attributes["value"].Value);

                settingInfoPerLine.ArmsConnectionString = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("ArmsConnectionString").Attributes["value"].Value);

                settingInfoPerLine.EicsConnectionString = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("EicsConnectionString").Attributes["value"].Value);

                settingInfoPerLine.LensConnectionString = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("LensConnectionString").Attributes["value"].Value);

                if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("ARMSDBPC") != null)
				{
					settingInfoPerLine.ArmsDBPC = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("ARMSDBPC").Attributes["value"].Value);
				}

				settingInfoPerLine.PcNO = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("pc_no").Attributes["value"].Value);

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Map") != null)
				{
					MapKB = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Map").Attributes["value"].Value);
					if (MapKB == "ON")
					{
						settingInfoPerLine.MapFG = true;
					}
				}

				settingInfoPerLine.TypeGroup = GetTypeGroupSettingSpecifiedLineCT(doc, lineCount);

				if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString())
				{
					settingInfoPerLine.MapFG = true;
				}

				//if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Outline") != null)
				//{
				//    OutlineKB = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Outline").Attributes["value"].Value);
				//    if (OutlineKB == "ON")
				//    {
				//        //Constant.fOutline = true;
				//        settingInfo.OutlineFG = true;
				//    }
				//}

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Nmc") != null)
				{
					NmcKB = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("Nmc").Attributes["value"].Value);
					if (NmcKB == "ON")
					{
						Constant.fNmc = true;
						settingInfoPerLine.NmcFG = true;
					}
				}

				settingInfoPerLine.KissFG = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("KISS").Attributes["value"].Value);
				settingInfoPerLine.BlackJumboDogFG = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("BlackJumboDog").Attributes["value"].Value);

				settingInfoPerLine.WBMappingFG = GetBoolVal(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("WBMappingKB"), "value");

				//settingInfoPerLine.LENSLinkEnable = MustGetBoolVal(doc.SelectNodes(LINEINFO_NODE)[lineCount], "LENSLinkEnable", "value");				

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("MappingFolder") != null)
				{
					settingInfoPerLine.DirMDMapping = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("MappingFolder").Attributes["value"].Value;
				}
				//☆
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirMDMapping") != null)
				{
					settingInfoPerLine.DirMDMapping = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirMDMapping").Attributes["value"].Value;
				}
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirAIMapping") != null)
				{
					settingInfoPerLine.DirAIMapping = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirAIMapping").Attributes["value"].Value;
				}
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("ForWBCompareMachineLogDirPath") != null)
				{
					settingInfoPerLine.ForWBCompareMachineLogDirPath = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("ForWBCompareMachineLogDirPath").Attributes["value"].Value;
				}
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("MappingBKFolder") != null)
				{
					settingInfoPerLine.DirMappingBKFolder = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("MappingBKFolder").Attributes["value"].Value;
				}
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("NASCAFolder") != null)
				{
					settingInfoPerLine.DirNASCAFolder = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("NASCAFolder").Attributes["value"].Value;
				}
				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirCommonOutput") != null)
				{
					settingInfoPerLine.DirCommonOutput = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("DirCommonOutput").Attributes["value"].Value;
				}

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("NotUseTmQDIW") != null)
				{
					settingInfoPerLine.NotUseTmQDIWFG = GetBoolVal(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("NotUseTmQDIW"), "value");
				}

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("BadMarkingInfo") != null)
				{
					settingInfoPerLine.BMCauseCD = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("BadMarkingInfo").Attributes["BMCauseCD"].Value;
					settingInfoPerLine.BMClassCD = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("BadMarkingInfo").Attributes["BMClassCD"].Value;
					settingInfoPerLine.BMDefectCD = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("BadMarkingInfo").Attributes["BMDefectCD"].Value;

					if (string.IsNullOrEmpty(settingInfoPerLine.BMCauseCD) || string.IsNullOrEmpty(settingInfoPerLine.BMClassCD) || string.IsNullOrEmpty(settingInfoPerLine.BMDefectCD))
					{
						throw new Exception(Constant.MessageInfo.Message_139);
					}
				}

				settingInfoPerLine.IsSendJudgeOKAlwaysOnHSMS = false;

				if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("SendJudgeOKAlwaysOnHSMS") != null)
				{
					string strIsSendJudgeOKAlwaysOnHSMS = Convert.ToString(doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("SendJudgeOKAlwaysOnHSMS").Attributes["value"].Value).ToUpper();

					if (strIsSendJudgeOKAlwaysOnHSMS == "ON")
					{
						settingInfoPerLine.IsSendJudgeOKAlwaysOnHSMS = true;
					}
				}

				SettingInfoList.Add(settingInfoPerLine);
			}

            foreach (SettingInfo si in SettingInfoList)
            {
                foreach (EquipmentInfo ei in si.EquipmentList)
                {
                    using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, si.LineCD)))
                    {
                        DataContext.TmEQUI equiRec = eicsDB.TmEQUI.Where(t => t.Equipment_NO == ei.EquipmentNO && t.Del_FG == false).FirstOrDefault();

                        if (equiRec == null)
                        {
                            throw new ApplicationException($"設備CD『{ei.EquipmentNO}』でTmEQUIからマスタが取得できません。");
                        }

                        ei.ModelNM = equiRec.Model_NM;
                        ei.MachineNM = equiRec.MachinSeq_NO;
                        ei.AssetsNM = equiRec.Assets_NM;
                    }
                }
            }
        }

        private void GetLotNoConvSetting(XmlNode node)
		{
			bool needLotNoConvertFG;
            string lotNoConvertIDStr, lotNoConvStartIndexStr, lotNoConvLenStr, FullSerialNoMarkingDigitStr;

			needLotNoConvertFG = MustGetBoolVal(node, "LotNoConvert", "NeedFG");

			if (needLotNoConvertFG)
			{

                //2016.3.14 湯浅　完全連番モードを追加。これがtrueの場合は後の項目は使用しないが引数自体は必須。
                FullSerialNoModeFG = MustGetBoolVal(node, "LotNoConvert", "FullSerialNoModeFG");
                FullSerialNoMarkingDigitStr = MustGetStrVal(node, "LotNoConvert", "Digit");

                int fullSerialNoMarkingDigit;
                if (int.TryParse(FullSerialNoMarkingDigitStr, out fullSerialNoMarkingDigit) == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "/qcil_info/LotNoConvert Digit=", FullSerialNoMarkingDigitStr));
                }
                FullSerialNoMarkingDigit = fullSerialNoMarkingDigit;
                
                lotNoConvertIDStr = MustGetStrVal(node, "LotNoConvert", "ID");
                lotNoConvStartIndexStr = MustGetStrVal(node, "LotNoConvert", "StartIndex");
                lotNoConvLenStr = MustGetStrVal(node, "LotNoConvert", "Len");

                LotNoConvertID = lotNoConvertIDStr.Trim();

                int lotNoConvLen;
                if (int.TryParse(lotNoConvLenStr, out lotNoConvLen) == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "/qcil_info/LotNoConvert Len=", lotNoConvLenStr));
                }
                LotNoConvLen = lotNoConvLen;

                int lotNoConvStartIndex;
                if (int.TryParse(lotNoConvStartIndexStr, out lotNoConvStartIndex) == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_135, "/qcil_info/LotNoConvert StartIndex=", lotNoConvStartIndexStr));
                }
                LotNoConvStartIndex = lotNoConvStartIndex;
			}
		}

		private string GetLineTypeSettingSpecifiedLineCT(XmlDocument doc, int lineCount)
		{
			bool lineTypeMatchFG = false;

			string lineTypeStr = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("LineType").Attributes["value"].Value;

			string[] LineTypeArray = Enum.GetNames(typeof(Constant.LineType));
			foreach (string lineTypeVal in LineTypeArray)
			{
				if (lineTypeStr.ToUpper() == lineTypeVal.ToUpper())
				{
					lineTypeMatchFG = true;
					lineTypeStr = lineTypeVal;
					break;
				}
			}
			if (lineTypeMatchFG == false)
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_98, lineTypeStr));
			}

			return lineTypeStr;
		}

		private string GetTypeGroupSettingSpecifiedLineCT(XmlDocument doc, int lineCount)
		{
			bool typeGroupMatchFG = false;

			if (doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("TypeGroup") == null)
			{
				throw new ApplicationException(string.Format("qcil.xmlにTypeGroupがありません。設定を追加して下さい。"));
			}

			string typeGroupStr = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectSingleNode("TypeGroup").Attributes["value"].Value;

			string[] typeGroupArray = Enum.GetNames(typeof(Constant.TypeGroup));
			foreach (string typeGroupVal in typeGroupArray)
			{
				if (typeGroupStr.ToUpper() == typeGroupVal.ToUpper())
				{
					typeGroupMatchFG = true;
					typeGroupStr = typeGroupVal;
					break;
				}
			}
			if (typeGroupMatchFG == false)
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_98, typeGroupStr));
			}

			return typeGroupStr;
		}

		/// <summary>
		/// 装置リストの取得
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="lineCount"></param>
		/// <returns></returns>
		private List<EquipmentInfo> GetEquipSetting(XmlDocument doc, int lineCount)
		{
			List<EquipmentInfo> equipmentList = new List<EquipmentInfo>();

			XmlNodeList nodes = doc.SelectNodes(LINEINFO_NODE)[lineCount].SelectNodes("//EquipmentList/Equipment");
			foreach (XmlNode node in nodes)
			{
				EquipmentInfo equipmentInfo = new EquipmentInfo();

				if (node.Attributes["no"] == null)
				{
					string path = string.Format("{0}", node.BaseURI);
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, path));
				}
				else
				{
					equipmentInfo.EquipmentNO = node.Attributes["no"].Value;
				}
				//equipmentInfo.TypeCD = node.Attributes["value"].Value; //2015.11.5 タイプ取込をDBに変えるので削除。（定期取得のルーチンで上書き）

				//if (node.Attributes["chip"] != null)
				//{
				//	equipmentInfo.ChipNM = node.Attributes["chip"].Value;
				//}

				equipmentInfo.IsOutputCommonPath = GetBoolVal(node, "IsOutputCommonPath");

				equipmentInfo.BMCountFG = GetBoolVal(node, "BMCountFG");

				equipmentInfo.IsOutputNasFile = GetBoolVal(node, "IsOutputNasFile");

				equipmentInfo.IsNotSendZeroMapping = GetBoolVal(node, "IsNotSendZeroMapping");

				equipmentInfo.UnSelectableTypeFG = GetBoolVal(node, "UnSelectableType");

				equipmentInfo.WaitForRenameByArmsFG = GetBoolVal(node, "WaitForRenameByArmsFg");

				equipmentInfo.HasLoaderQRReader = MustGetBoolVal(node, "HasLoaderQRReader");

				equipmentInfo.IsOutputCIFSResultFG = GetBoolVal(node, "IsOutputCIFSResultFG");

				equipmentInfo.IsOutputPLCResultFG = GetBoolVal(node, "IsOutputPLCResultFG");

				equipmentInfo.EnablePreVerifyFG = GetBoolVal(node, "EnablePreVerifyFG");

				equipmentInfo.EnableOpeningChkFg = GetBoolVal(node, "EnableOpeningCheckFG");

                equipmentInfo.UnSelectableWorkFG = GetBoolVal(node, "UnSelectableWorkFG");

				equipmentInfo.ForciblyEnableSequencialFileProcessFg = GetBoolVal(node, "ForciblyEnableSequencialEndFileProcessFG");


				//<--後工程合理化/エラー集計
				equipmentInfo.PostProcessMonitoringCycleSec = GetIntVal(node, "PostProcessMonitoringCycleSec");
                //-->後工程合理化/エラー集計

                //equipmentInfo.Disable3SPFilesSupportFunc = MustGetBoolVal(node, "Disable3SPFilesSupportFunc");

                //山本LM装置(フレームタイプ)のARMSとのハンドシェイクをOFFにするモードを追加。2016.5.28
                equipmentInfo.LMArmsHandshakeFG = GetBoolVal(node, "LMArmsHandshakeFG");

				if (node.Attributes["dirWBMagazine"] != null)
				{
					equipmentInfo.DirWBMagazine = node.Attributes["dirWBMagazine"].Value;
				}
				else
				{
					equipmentInfo.DirWBMagazine = WBMachineInfo.FOLDER_MAGAZINE_NM;
				}

				if (node.Attributes["startFileDir"] != null)
				{
					equipmentInfo.StartFileDirNm = node.Attributes["startFileDir"].Value;
				}
				else
				{
					equipmentInfo.StartFileDirNm = string.Empty;
				}

				if (node.Attributes["endFileDir"] != null)
				{
					equipmentInfo.EndFileDirNm = node.Attributes["endFileDir"].Value;
				}
				else
				{
					equipmentInfo.EndFileDirNm = string.Empty;
				}

				if (node.Attributes["dateStrIndex"] != null)
				{
					string dateIndexStr = node.Attributes["dateStrIndex"].Value;					
					int dateStrIndex;

					if (int.TryParse(dateIndexStr, out dateStrIndex))
					{
						equipmentInfo.DateStrIndex = dateStrIndex;
					}
					else
					{
						throw new ApplicationException(string.Format("設備毎の設定[dateStrIndex]が数値変換出来ません。 変換対象：{0}", dateIndexStr));
					}
				}
				else
				{
					equipmentInfo.DateStrIndex = 0;
				}

				if (node.Attributes["dateStrLen"] != null)
				{
					string dateLenStr = node.Attributes["dateStrLen"].Value;
					int dateStrLen;

					if (int.TryParse(dateLenStr, out dateStrLen))
					{
						equipmentInfo.DateStrLen = dateStrLen;
					}
					else
					{
						throw new ApplicationException(string.Format("設備毎の設定[dateStrLen]が数値変換出来ません。 変換対象：{0}", dateLenStr));
					}
				}
				else
				{
					equipmentInfo.DateStrLen = 0;
				}

				equipmentInfo.EndIdLen = GetIntVal(node, "endIdLen");

				if (equipmentInfo.EndIdLen == 0)
				{
					equipmentInfo.EndIdLen = equipmentInfo.DateStrLen;
				}

				if (node.Attributes["plcType"] != null)
				{
					equipmentInfo.PLCType = node.Attributes["plcType"].Value;
				}
				else
				{
					equipmentInfo.PLCType = string.Empty;
				}

				equipmentInfo.PLCProtocol = GetStrVal(node, string.Empty, "plcProtocol");

				if (node.Attributes["plcEnc"] != null)
				{
					equipmentInfo.PLCEncode = node.Attributes["plcEnc"].Value;
				}
				else
				{
					equipmentInfo.PLCEncode = "ASCII";
				}

				if (node.Attributes["encodeStr"] != null)
				{
					equipmentInfo.EncodeStr = node.Attributes["encodeStr"].Value;
				}
				else
				{
					equipmentInfo.EncodeStr = string.Empty;
				}

				if (node.Attributes["availableAddress"] != null)
				{
					equipmentInfo.AvailableAddress = node.Attributes["availableAddress"].Value;
				}
				else 
				{
					equipmentInfo.AvailableAddress = string.Empty;
				}

				equipmentInfo.AIInspectPointCheckFG = GetBoolVal(node, "AIInspectPointCheckFG");

				XmlAttribute atReportType = node.Attributes["reportType"];
				if (atReportType != null)
				{
					equipmentInfo.ReportType = (Constant.ReportType)Enum.Parse(typeof(Constant.ReportType), atReportType.Value);
				}

                equipmentInfo.IsOutputMagOnHSMS = GetBoolVal(node, "IsOutputMagOnHSMS");

                equipmentInfo.FullParameterFG = GetBoolVal(node, "FullParameterFG");

                if (node.Attributes["MFileExists"] != null)
                {
                    equipmentInfo.MFileExists = GetBoolVal(node, "MFileExists");
                }
                else
                {
                    equipmentInfo.MFileExists = false;
                }

                equipmentInfo.SubtractWBMMErrorFg = GetBoolVal(node, "SubtractWBMMErrorFG");

                equipmentInfo.SFileExists = GetBoolVal(node, "SFileExists");

                equipmentInfo.ErrConvWithProcNo = GetBoolVal(node, "ErrConvWithProcNo");

                equipmentList.Add(equipmentInfo);
			}

			return equipmentList;
		}



        private static List<string> GetArmsServerList(XmlDocument doc)
        {
            List<string> serverList = new List<string>();
            XmlNodeList nodes = doc.SelectNodes("//arms_info/DBServerList/Server");

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["name"] == null)
                {
                    string path = string.Format("{0} name値", node.BaseURI);
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, path));
                }
                else
                {
                    serverList.Add(node.Attributes["name"].Value);
                }
            }

            return serverList;
        }

        public static string GetLanguage(string path) 
        {
			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			string languageValPath = "//Language";
			XmlNode nodeLanguage = doc.SelectSingleNode(languageValPath);
			if (nodeLanguage == null)
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_69, languageValPath));
			}
			return nodeLanguage.Attributes["value"].Value;
        }

		public static string GetNetExePath()
		{
			return SettingInfo.settingInfo.NetExePath;
		}

		public static int GetArmsProcessNO(string path)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			string processCDPath = "//arms_info/AIProcessCD";
			XmlNode nodeAIProcessNO = doc.SelectSingleNode(processCDPath);
			if (nodeAIProcessNO == null)
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_69, processCDPath));
			}
			return Convert.ToInt32(nodeAIProcessNO.Attributes["value"].Value);

		}

		public static Constant.DBConnectGroup GetDBGroup(int lineCD)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);
			if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString() && settingInfoPerLine.LineType == Constant.LineType.High.ToString())
			{
				return Constant.DBConnectGroup.HONBAN_MAP_HIGH;
			}
			else if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString() && settingInfoPerLine.LineType == Constant.LineType.Auto.ToString())
			{
				return Constant.DBConnectGroup.HONBAN_MAP_AUTO;
			}
			//else if (settingInfo.TypeGroup == Constant.TypeGroup.SideView.ToString() && settingInfo.LineType == Constant.LineType.High.ToString())
			//{
			//    return Constant.DBConnectGroup.HONBAN_SIDE_HIGH;
			//}
			else if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.SideView.ToString() && settingInfoPerLine.LineType == Constant.LineType.Auto.ToString())
			{
				return Constant.DBConnectGroup.HONBAN_SIDE;
			}
			else if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.SideView.ToString() && settingInfoPerLine.LineType == Constant.LineType.Hybrid.ToString())
			{
				return Constant.DBConnectGroup.HONBAN_SIDE;
			}
			else if (settingInfoPerLine.TypeGroup == Constant.TypeGroup.AutoMotive.ToString() && settingInfoPerLine.LineType == Constant.LineType.Auto.ToString())
			{
				return Constant.DBConnectGroup.HONBAN_GA_AUTO;
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_152, settingInfoPerLine.TypeGroup, settingInfoPerLine.LineType));
			}
		}

		private void GetResultPriorityJudge(XmlDocument doc)
		{
			XmlNode node = doc.SelectSingleNode("//qcil_info/ResultPriorityJudge");

			ResultPriorityJudgeInst = new ResultPriorityJudge();

			if (node != null)
			{
				ResultPriorityJudgeInst.StartNgNo = MustGetIntVal(node, "startNgNo");
				ResultPriorityJudgeInst.StartPlcAddr = MustGetStrVal2(node, "startPlcAddr");
				ResultPriorityJudgeInst.Length = MustGetIntVal(node, "len");		

				Regex reg = new Regex(@"[^0-9]");

				string addrNumPartStr = reg.Replace(ResultPriorityJudgeInst.StartPlcAddr, "");

				int addrNumPart;
				if(int.TryParse(addrNumPartStr, out addrNumPart) == false)
				{
					throw new ApplicationException(
						string.Format("ResultPriorityJudgeのStartPlcAddrについて数字以外の文字列除去後、数値変換できない文字が残っています。設定値を確認下さい。変換対象:{0}", addrNumPartStr));
				}

			}
		}

		private void GetHonbanDBInfo(XmlDocument doc)
		{
			string serverItemPath = "//qcil_info/HonbanServer";
			string dbItemPath = "//qcil_info/HonbanDB";

			if (doc.SelectSingleNode(serverItemPath) != null)
			{
				HonbanServer = Convert.ToString(doc.SelectSingleNode(serverItemPath).Attributes["value"].Value);
			}
			else
			{
				throw new ApplicationException(string.Format("{0}が設定されていません。", serverItemPath));
			}

			if (doc.SelectSingleNode(dbItemPath) != null)
			{
				HonbanDB = Convert.ToString(doc.SelectSingleNode(dbItemPath).Attributes["value"].Value);
			}
			else
			{
				throw new ApplicationException(string.Format("{0}が設定されていません。", dbItemPath));
			}
		}

		private string GetDebugEICSServerSpecifiedLineCT(XmlDocument doc)
		{
			string debugEICSServer = null;
			if (doc.SelectSingleNode("//qcil_info/DebugEICSServer") != null)
			{
				debugEICSServer = doc.SelectSingleNode("//qcil_info/DebugEICSServer").Attributes["value"].Value;
			}

			if (string.IsNullOrEmpty(debugEICSServer))
			{
				throw new ApplicationException("DebugEICSServerを未設定か設定に誤りがあります。");
			}

			return debugEICSServer;
		}

		private string GetDebugEICSDatabaseSpecifiedLineCT(XmlDocument doc)
		{
			string debugEICSDatabase = null;
			if (doc.SelectSingleNode("//qcil_info/DebugEICSDatabase") != null)
			{
				debugEICSDatabase = doc.SelectSingleNode("//qcil_info/DebugEICSDatabase").Attributes["value"].Value;
			}

			if (string.IsNullOrEmpty(debugEICSDatabase))
			{
				throw new ApplicationException("DebugEICSDatabaseを未設定か設定に誤りがあります。");
			}

			return debugEICSDatabase;
		}

		private string GetDebugARMSServer(XmlDocument doc)
		{
			string debugARMSServer = null;
			if (doc.SelectSingleNode("//arms_info/DebugARMSServer") != null)
			{
				debugARMSServer = doc.SelectSingleNode("//arms_info/DebugARMSServer").Attributes["value"].Value;
			}

			if (string.IsNullOrEmpty(debugARMSServer))
			{
				throw new ApplicationException("DebugARMSServerを未設定か設定に誤りがあります。");
			}

			return debugARMSServer;
		}

		private string GetDebugARMSDatabase(XmlDocument doc)
		{
			string debugARMSDatabase = null;
			if (doc.SelectSingleNode("//arms_info/DebugARMSDatabase") != null)
			{
				debugARMSDatabase = doc.SelectSingleNode("//arms_info/DebugARMSDatabase").Attributes["value"].Value;
			}

			if (string.IsNullOrEmpty(debugARMSDatabase))
			{
				throw new ApplicationException("DebugARMSDatabaseを未設定か設定に誤りがあります。");
			}

			return debugARMSDatabase;
		}

		public static void CheckDebugSetting()
		{
			SettingInfo settingInfo = SettingInfo.GetSingleton();

			if (File.Exists(DEBUG_SETTING_FILE_PATH))
			{
				MustConfirmMsgBox.Show("製造で使用する際に、このメッセージが表示された場合はシステム担当に連絡してください。");

				string msg = string.Empty;

				msg += "EICS Server:\r\n\t" + settingInfo.DebugEICSServer + "\r\n";
				msg += "EICS DB:\r\n\t" + settingInfo.DebugEICSDatabase + "\r\n\r\n";

				msg += "ARMS Server:\r\n\t" + settingInfo.DebugARMSServer + "\r\n";
				msg += "ARMS DB:\r\n\t" + settingInfo.DebugARMSDatabase;

				MessageBox.Show(msg);
			}
		}

		public static bool HasDebugSetting()
		{
			return File.Exists(DEBUG_SETTING_FILE_PATH);
		}

		public static Encoding GetEncode()
		{
			return SettingInfo.settingInfo.FileEncode;
		}

		public static string GetSectionCD()
		{
			return settingInfo.SectionCD;
		}


		//設定ファイルとLSETを比較して設定ファイルを更新する。(2015.11.04追加)
		//起動初回のみこの関数を実行し、以後は定期実行オブジェクトを使用する。
		public static void SettingFileTypeChanger()
		{
			//設定データ
			SettingInfo settingInfo = SettingInfo.GetSingleton();
			List<int> lineList = SettingInfo.GetLineCDList();

			try
			{
				foreach (int lineCd in lineList)
				{
					List<Lset> lsetDataList = Lset.GetLsetData(lineCd);

					foreach (Lset lsetData in lsetDataList)
					{

						for (int i = 0; settingInfo.SettingInfoList.Count() > i; i++)
						{
							EquipmentInfo equipmentInfo = settingInfo.SettingInfoList[i].EquipmentList.Find(l => l.EquipmentNO == lsetData.EquipmentNO);
                            if (equipmentInfo == null)
                            {
                                continue;
                            }
                            if (equipmentInfo.TypeGroupCD != lsetData.WorkingTypeGroup_CD)
                            {
                                equipmentInfo.TypeGroupCD = lsetData.WorkingTypeGroup_CD;
                            }
                            if (equipmentInfo.TypeCD != lsetData.WorkingType_CD)
                            {
                                equipmentInfo.TypeCD = lsetData.WorkingType_CD;
                            }
                            if (equipmentInfo.ChipNM != lsetData.ChipNM)
                            {
                                equipmentInfo.ChipNM = lsetData.ChipNM;
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        // 設定ファイルから参照するサーバを全て取得する。
        public static List<string> GetReferServerNm(LSETInfo lsetInfo)
        {
            List<string> refServerList = new List<string>();

            SettingInfo commonSetting = SettingInfo.GetSingleton();
            SettingInfo lineSetting = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
            refServerList.Add(lineSetting.PackagePC);
            if (lsetInfo.ReferMultiServerFG)
            {
                refServerList.AddRange(commonSetting.ArmsServerList);
            }

            return refServerList;
        }
    }
}
