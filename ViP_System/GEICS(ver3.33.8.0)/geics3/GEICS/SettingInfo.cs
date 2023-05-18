using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GEICS
{
    public class SettingInfo
    {
		public int InlineCD { get; set; }
		public string LineType { get; set; }
		public bool PackageFG { get; set; }
		public string PackagePC { get; set; }
        public string EicsConnectionString { get; set; }
        public string ArmsConnectionString { get; set; }
        public string LensConnectionString { get; set; }

        public bool ClientFG { get; set; }
		public bool MapFG { get; set; }

		public string StrQCIL { get; set; }
		public string StrARMS { get; set; }
		

        public const string SETTING_FILE_PATH = "C:\\QCIL\\SettingFiles\\QCIL.xml";

        public static List<SettingInfo> GetSetting()
        {
            string sInlinecd = "";;
            string sPackage = "";
            
            //string sfName = "C:\\QCIL\\SettingFiles\\QCIL.xml";

             //クライアント=sfNameのファイルなし→開始/終了がタイマーで変化なし
            if (File.Exists(SETTING_FILE_PATH))
            {
                Constant.fClient = false;//中間サーバー
            }
            else
            {
                Constant.fClient = true;//クライアント
				//中間サーバー使用の場合
				return null;
            }

			List<SettingInfo> settingInfoList = new List<SettingInfo>();

            XDocument doc = XDocument.Load(SETTING_FILE_PATH);

			GetGeicsInfo(doc);


			if (doc.Root.Element("qcil_info") == null)
			{
				Constant.fClient = true;
			}

            //sInlinecd = doc.SelectSingleNode("//configuration/qcil_info/inline_cd").Attributes["value"].Value;

			foreach (XElement lineInfo in doc.Root.Elements("qcil_info").Elements("line_info"))
			{
				SettingInfo settingInfo = new SettingInfo();

				sInlinecd = lineInfo.Attribute("inline_cd").Value;

				int inlineCD;
				if (int.TryParse(sInlinecd, out inlineCD))
				{
					settingInfo.InlineCD = inlineCD;
				}
				else
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_99, sInlinecd));
				}

				//<--Package
				try
				{
					sPackage = lineInfo.Element("Package").Attribute("value").Value;
					//sPackage = doc.SelectSingleNode("//configuration/qcil_info/Package").Attributes["value"].Value;
				}
				catch
				{
					//設定がなければ、OFF
					sPackage = "OFF";
				}

				if (sPackage == "ON")
				{
					settingInfo.PackageFG = true;
				}
				else
				{
					settingInfo.PackageFG = false;
				}
				//-->Package

				try
				{
					settingInfo.PackagePC = lineInfo.Element("PackagePC").Attribute("value").Value;

                    //Constant.sPackagePC = doc.SelectSingleNode("//configuration/qcil_info/PackagePC").Attributes["value"].Value;
                }
                catch
				{//設定がなければ、OFF
					settingInfo.PackagePC = "localhost";
                }

                try
                {
                    settingInfo.EicsConnectionString = lineInfo.Element("EicsConnectionString").Attribute("value").Value;
                }
                catch
                {//設定がなければ、OFF
                    settingInfo.EicsConnectionString = null;
                }
                try
                {
                    settingInfo.ArmsConnectionString = lineInfo.Element("ArmsConnectionString").Attribute("value").Value;
                }
                catch
                {//設定がなければ、OFF
                    settingInfo.ArmsConnectionString = null;
                }
                try
                {
                    settingInfo.LensConnectionString = lineInfo.Element("LensConnectionString").Attribute("value").Value;
                }
                catch
                {//設定がなければ、OFF
                    settingInfo.LensConnectionString = null;
                }


                Common com = new Common();
				
                //settingInfo.MapFG = com.GetMapSetting();   //QCIL.xml > 川幅変更済み ON/OFF
                string typeGroup = lineInfo.Element("TypeGroup").Attribute("value").Value.ToUpper();
                if (typeGroup == Constant.TypeGroup.MAP.ToString().ToUpper()) 
                {
                    Constant.typeGroup = Constant.TypeGroup.MAP;
                }
				else if ((typeGroup == Constant.TypeGroup.SV.ToString().ToUpper()) || (typeGroup == Constant.TypeGroup.SIDEVIEW.ToString().ToUpper()))
                {
                    Constant.typeGroup = Constant.TypeGroup.SV;
                }
				else if (typeGroup == Constant.TypeGroup.AUTOMOTIVE.ToString().ToUpper())
                {
					Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
                }
				else if (typeGroup == Constant.TypeGroup.X19.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.X19;
				}
				else if (typeGroup == Constant.TypeGroup.MPL.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.MPL;
				}
				else if (typeGroup == Constant.TypeGroup.X83_385_COB.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.X83_385_COB;
				}
				else if (typeGroup == Constant.TypeGroup.X83.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.X83;
				}
				else if (typeGroup == Constant.TypeGroup.COB.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.COB;
				}
				else if (typeGroup == Constant.TypeGroup._385.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup._385;
				}
				else if (typeGroup == Constant.TypeGroup.VOYAGER.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.VOYAGER;
				}
				else if (typeGroup == Constant.TypeGroup.CSP.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.CSP;
				}
				else if (typeGroup == Constant.TypeGroup._093.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup._093;
				}
				else if (typeGroup == Constant.TypeGroup.KIRAMEKI.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.KIRAMEKI;
				}
				else if (typeGroup == Constant.TypeGroup.VOYAGER_CSP_093_KIRAMEKI.ToString().ToUpper())
				{
					Constant.typeGroup = Constant.TypeGroup.VOYAGER_CSP_093_KIRAMEKI;
				}
                else if (typeGroup == Constant.TypeGroup.NLCV.ToString().ToUpper())
                {
                    Constant.typeGroup = Constant.TypeGroup.NLCV;
                }
                else 
				{
					throw new ApplicationException("想定していないTypeGroupが設定されています。");
				}

				bool lineTypeMatchFG = false;
				string sLineType = lineInfo.Element("LineType").Attribute("value").Value;
				string[] LineTypeArray = Enum.GetNames(typeof(Constant.LineType));

				foreach(string lineTypeVal in LineTypeArray)
				{
					if(sLineType.ToUpper() == lineTypeVal.ToUpper())
					{
						lineTypeMatchFG = true;
						settingInfo.LineType = lineTypeVal.ToUpper();
						break;
					}
				}
				if (lineTypeMatchFG == false)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_98, sLineType));
				}


				//string OutlineKB = Convert.ToString(doc.Root.Element("qcil_info").Element("Outline").Attribute("value").Value);
				//if (OutlineKB == "ON")
				//{
				//    Constant.fOutline = true;
				//}

				//if (Constant.sPackagePC != "localhost")
				//{
				//    Constant.fSemi = true;
				//}

				settingInfoList.Add(settingInfo);

			}
            return settingInfoList;
        }

		private static void GetGeicsInfo(XDocument doc)
		{
			if (doc.Root.Element("geics_info") != null)
			{
				string sWBEquipVerParamNO = doc.Root.Element("geics_info").Element("WBEquipVer").Attribute("paramNo").Value;
				int wbEquipVerParamNO;

				if (!int.TryParse(sWBEquipVerParamNO, out wbEquipVerParamNO))
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_99, sWBEquipVerParamNO));
				}

				Constant.WBEquipVerParamNO = wbEquipVerParamNO;
			}
		}
    }
}
