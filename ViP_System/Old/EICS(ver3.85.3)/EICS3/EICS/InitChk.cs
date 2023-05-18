using EICS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS
{
	class InitChk
	{
		public static void Run()
		{
			PlcFileConvGeneral();
		}

		public static void PlcFileConvGeneral()
		{
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			foreach(SettingInfo settingInfoPerLine in commonSettingInfo.SettingInfoList)
			{
				if (PlcFileConv.GetGeneralNm(settingInfoPerLine.LineCD, PlcFileConv.IDENTCD_LOTNO) == string.Empty)
				{
                    //throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTLOT", "LOTNO", settingInfoPerLine.PackagePC));
                    throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTLOT", "LOTNO", settingInfoPerLine.EicsConnectionString));
				}

				if (PlcFileConv.GetGeneralNm(settingInfoPerLine.LineCD, PlcFileConv.IDENTCD_MEASUREDT) == string.Empty)
				{
					//throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTMDT", "MEASUREDT", settingInfoPerLine.PackagePC));
                    throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTMDT", "MEASUREDT", settingInfoPerLine.EicsConnectionString));
                }

                if (PlcFileConv.GetGeneralNm(settingInfoPerLine.LineCD, PlcFileConv.IDENTCD_MAGNO) == string.Empty)
				{
					//throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTMAG", "MAGNO", settingInfoPerLine.PackagePC));
                    throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTMAG", "MAGNO", settingInfoPerLine.EicsConnectionString));
                }

                if (PlcFileConv.GetGeneralNm(settingInfoPerLine.LineCD, PlcFileConv.IDENTCD_TYPE) == string.Empty)
				{
					//throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTTYP", "TYPE", settingInfoPerLine.PackagePC));
                    throw new ApplicationException(string.Format("TmGENERALにGeneralGrp_CD:{0} General_CD:{1} General_NM:{2}が必要です。Server:{3}", "PLCFILE", "IDENTTYP", "TYPE", settingInfoPerLine.EicsConnectionString));
                }
            }
		}
	}
}
