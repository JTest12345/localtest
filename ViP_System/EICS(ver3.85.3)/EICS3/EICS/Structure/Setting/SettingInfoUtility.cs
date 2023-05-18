using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EICS
{
	public partial class SettingInfo
	{
		public static bool GetBatchModeFG()
		{
			return settingInfo.IsBatchMode;
		}

		public static string GetPackagePC(int lineCD)
		{
			return settingInfo.SettingInfoList.Find(s => s.LineCD == lineCD).PackagePC;
		}

		public static string GetArmsDBPC(int lineCD)
		{
			return settingInfo.SettingInfoList.Find(s => s.LineCD == lineCD).ArmsDBPC;
		}

		public static int GetErrChkIntervalSec()
		{
			return settingInfo.ErrorCheckIntervalSec;
		}

		public static int GetDelSysLogIntervalDay()
		{
			return settingInfo.DeleteSysLogIntervalDay;
		}

		public static List<int> GetLineCDList()
		{
			return settingInfo.SettingInfoList.Select(s => s.LineCD).ToList();
		}

		public EquipmentInfo GetEquipInfo(string plantCD)
		{
			return EquipmentList.Find(e => e.EquipmentNO == plantCD);
		}

		/// <summary>
		/// チップを取得
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetChipNM(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			string chipNM = equipmentInfo.ChipNM;

			return chipNM;
		}

		public bool GetBMCount(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.BMCountFG;
		}

		public bool GetUnSelectableTypeFG(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.UnSelectableTypeFG;
		}

		public bool GetWaitForRenameByArmsFG(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.WaitForRenameByArmsFG;
		}

		public string GetPLCEncode(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.PLCEncode;
		}

		public bool GetHasLoaderQRReaderFG(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.HasLoaderQRReader;
		}

		public bool IsOutputMagazineOnHSMS(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.IsOutputMagOnHSMS;
		}

		public bool IsOutputNasFile(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.IsOutputNasFile;
		}

		public bool IsNotSendZeroMapping(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.IsNotSendZeroMapping;
		}

		public bool IsOutputCIFSResult(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.IsOutputCIFSResultFG;
		}

		//public bool IsOutputPLCResult(string plantCD)
		//{
		//	EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

		//	return equipmentInfo.IsOutputPLCResultFG;
		//}

		public bool IsEnablePreVerify(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.EnablePreVerifyFG;
		}

		public bool IsEnableOpeningChk(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.EnableOpeningChkFg;
		}

		public bool Disable3SPFilesSupportFunc(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.Disable3SPFilesSupportFunc;
		}

		/// <summary>
		/// WBマガジンディレクトリを取得(重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetWBMagazineDir(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			string wbMagazineDir = equipmentInfo.DirWBMagazine;

			return wbMagazineDir;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public bool IsAIInspectCheck(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.AIInspectPointCheckFG;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetStartFileDirNm(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.StartFileDirNm;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetPLCType(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.PLCType.ToUpper();
		}

		public string GetPLCProtocol(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.PLCProtocol.ToUpper();
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetEndFileDirNm(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.EndFileDirNm;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public int GetDateStrIndex(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.DateStrIndex;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public int GetDateStrLen(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.DateStrLen;
		}

		public int GetEndIdLen(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.EndIdLen;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetEncodeStr(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.EncodeStr;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public bool GetFullParameterFG(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.FullParameterFG;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public string GetAvailableAddress(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.AvailableAddress;
		}

		/// <summary>
		/// 山本LM装置(フレームタイプ)のARMSとのハンドシェイクをするかどうかのフラグ。2016.5.28
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public bool GetLMArmsHandShakeFG(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.LMArmsHandshakeFG;
		}

		/// <summary>
		/// レポートタイプを取得
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public Constant.ReportType GetReportType(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.ReportType;
		}

		public bool IsOutputCommonPath(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.IsOutputCommonPath;
		}

		/// <summary>
		/// (重要：ライン毎のSettingInfoインスタンスから呼び出しする必要有)
		/// </summary>
		/// <param name="plantCD"></param>
		/// <returns></returns>
		public bool GetForciblyEnableSequencialFileProcessFg(string plantCD)
		{
			EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

			return equipmentInfo.ForciblyEnableSequencialFileProcessFg;
		}


		private static bool GetBoolVal(XmlNode node, string nodeNm)
		{
			if (node.Attributes[nodeNm] != null)
			{
				if (node.Attributes[nodeNm].Value.ToUpper() == "TRUE" || node.Attributes[nodeNm].Value.ToUpper() == "ON")
					return true;
				else if (node.Attributes[nodeNm].Value.ToUpper() == "FALSE" || node.Attributes[nodeNm].Value.ToUpper() == "OFF")
					return false;
				else
					throw new Exception(string.Format(Constant.MessageInfo.Message_137, nodeNm, node.Attributes[nodeNm].Value, node.BaseURI));
			}
			else
			{
				return false;
			}
		}

		private static string GetStrVal(XmlNode node, string nodeNm)
		{
			if (node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes["value"] != null)
				{
					return node.SelectSingleNode(nodeNm).Attributes["value"].Value;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + " value"));
				}
			}
			else
			{
				return string.Empty;
			}
		}

		private static string GetStrVal(XmlNode node, string nodeNm, string attrNm)
		{
			if (string.IsNullOrEmpty(nodeNm) == false && node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes[attrNm] != null)
				{
					return node.SelectSingleNode(nodeNm).Attributes[attrNm].Value;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + attrNm));
				}
			}
			else if (node.Attributes[attrNm] != null)
			{
				return node.Attributes[attrNm].Value;
			}
			else
			{
				return string.Empty;
			}
		}

		//設定ファイルからINT型のデータを読み込む。タグが無い場合は0を返す。
		private static int GetIntVal(XmlNode node, string nodeNm, string attrNm)
		{
			int retv;
			string sretv;

			if (node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes[attrNm] != null)
				{
					sretv = node.SelectSingleNode(nodeNm).Attributes[attrNm].Value;
					if (int.TryParse(sretv, out retv) == false)
					{
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_158, nodeNm + attrNm));
					}
					else
					{
						return retv;
					}
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + attrNm));
				}
			}
			else
			{
				retv = 0;
				return retv;
			}
		}

		private static int GetIntVal(XmlNode node, string attrNm)
		{
			int retv;
			string sretv;

			if (node.Attributes[attrNm] != null)
			{
				sretv = node.Attributes[attrNm].Value;
				if (int.TryParse(sretv, out retv) == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_158, node.Name + attrNm));
				}
			}
			else
			{
				retv = 0;
			}

			return retv;
		}

		private static int MustGetIntVal(XmlNode node, string attrNm)
		{
			int retv;

			if (node != null)
			{
				if (node.Attributes[attrNm] != null)
				{
					string strVal = node.Attributes[attrNm].Value;
					if (int.TryParse(strVal, out retv) == false)
					{
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_158, node.Name + attrNm));
					}
					else
					{
						return retv;
					}
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, node.OuterXml + "内に不足(" + attrNm + ")"));
				}
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, node.Name + "内に不足(" + attrNm + ")"));
			}
		}

		private static string MustGetStrVal(XmlNode node, string nodeNm)
		{
			if (node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes["value"] != null)
				{
					return node.SelectSingleNode(nodeNm).Attributes["value"].Value;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + " value"));
				}
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm));
			}
		}

		private static string MustGetStrVal2(XmlNode node, string attrNm)
		{
			if (node != null)
			{
				if (node.Attributes[attrNm] != null)
				{
					return node.Attributes[attrNm].Value;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, node.OuterXml + "内に不足(" + attrNm + ")"));
				}
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, node.Name + "内に不足(" + attrNm + ")"));
			}
		}

		private static bool MustGetBoolVal(XmlNode node, string nodeNm, string attrNm)
		{
			if (node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes[attrNm] != null)
				{
					string getStr = node.SelectSingleNode(nodeNm).Attributes[attrNm].Value.ToUpper();
					if (getStr == "FALSE" || getStr == "OFF")
					{
						return false;
					}
					return true;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + attrNm));
				}
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm));
			}
		}

		private static bool MustGetBoolVal(XmlNode node, string attrNm)
		{
			if (node.Attributes[attrNm] != null)
			{
				string getStr = node.Attributes[attrNm].Value.ToUpper();
				if (getStr == "FALSE" || getStr == "OFF")
				{
					return false;
				}
				return true;
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, node.OuterXml + "内に不足(" + attrNm + ")"));
			}

		}

		private static string MustGetStrVal(XmlNode node, string nodeNm, string attrNm)
		{
			if (node.SelectSingleNode(nodeNm) != null)
			{
				if (node.SelectSingleNode(nodeNm).Attributes[attrNm] != null)
				{
					return node.SelectSingleNode(nodeNm).Attributes[attrNm].Value;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm + attrNm));
				}
			}
			else
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_69, nodeNm));
			}
		}

        public bool GetSFileExists(string plantCD)
        {
            EquipmentInfo equipmentInfo = EquipmentList.Find(e => e.EquipmentNO == plantCD);

            return equipmentInfo.SFileExists;
        }
    }
}
