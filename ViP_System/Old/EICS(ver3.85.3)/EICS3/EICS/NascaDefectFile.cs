using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EICS
{
    public class NascaDefectFile
    {
		public class DefectAddrInfo
		{
			public int Addr { get; set; }
			public string ErrCd { get; set; }
			public string CauseCd { get; set; }
			public string ClassCd { get; set; }
		}

		public class DefectQtyInfo
		{
			public string ErrCd { get; set; }
			public string CauseCd { get; set; }
			public string ClassCd { get; set; }
			public int Qty { get; set; }
		}

		public static List<DefectQtyInfo> GetDefectQtyList(List<DefectAddrInfo> allDefAddrInfoList)
		{
			List<DefectQtyInfo> defQtyList = new List<DefectQtyInfo>();

			List<DefectAddrInfo> defAddrInfoList = allDefAddrInfoList.FindAll(d => d.Addr > 0);

			foreach (DefectAddrInfo defAddr in defAddrInfoList)
			{
				int index = defQtyList.FindIndex(dq => dq.ErrCd == defAddr.ErrCd && dq.CauseCd == defAddr.CauseCd && dq.ClassCd == defAddr.ClassCd);

				if (index == -1)
				{
					DefectQtyInfo defQty = new DefectQtyInfo();
					defQty.ErrCd = defAddr.ErrCd;
					defQty.CauseCd = defAddr.CauseCd;
					defQty.ClassCd = defAddr.ClassCd;
					defQty.Qty = 1;

					defQtyList.Add(defQty);
				}
				else
				{
					defQtyList[index].Qty++;
				}
			}

			return defQtyList;
		}


		const string NASCA_DEFECT_FILE_EXT = ".nas";

		public static void Create(string lotNo, string magazineNo, string plantCd, int lineNo, string data)
		{
			Create(lotNo, magazineNo, plantCd, lineNo, data, null);
		}

		public static string Create(string lotNo, string magazineNo, string plantCd, int lineNo, List<DefectQtyInfo> defQtyList)
		{
			string data = "";
			foreach (DefectQtyInfo defQty in defQtyList)
			{
				data += string.Format("{0},{1},{2},{3}\r\n", defQty.ErrCd, defQty.CauseCd, defQty.ClassCd, defQty.Qty);
			}
			return Create(lotNo, magazineNo, plantCd, lineNo, data, null, null);
		}

		public static string Create(string lotNo, string magazineNo, string plantCd, int lineNo, long? procNo, List<DefectQtyInfo> defQtyList, string targetFileIdentity)
		{
			string data = "";
			foreach (DefectQtyInfo defQty in defQtyList)
			{
				data += string.Format("{0},{1},{2},{3}\r\n", defQty.ErrCd, defQty.CauseCd, defQty.ClassCd, defQty.Qty);
			}
			return Create(lotNo, magazineNo, plantCd, lineNo, data, procNo, targetFileIdentity);
		}

		public static string Create(string lotNo, string magazineNo, string plantCd, int lineNo, string data, long? procNO)
		{
			return Create(lotNo, magazineNo, plantCd, lineNo, data, null, null);
		}

		public static string Create(string lotNo, string magazineNo, string plantCd, int lineNo, string data, long? procNO, string targetFileIdentity)
		{
            SettingInfo setting = SettingInfo.GetSettingInfoPerLine(lineNo);

            if (!Directory.Exists(setting.DirNASCAFolder))
            {
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("NasFile Create() :{0}が無い為、作成", setting.DirNASCAFolder));
				Directory.CreateDirectory(setting.DirNASCAFolder);
            }

            string lotText = lotNo;
            if (string.IsNullOrEmpty(lotNo))
            {
                lotText = "unknown";
            }

			string filePath;

			//if (string.IsNullOrEmpty(targetFileIdentity))
			//{
			//	filePath = Path.Combine(setting.DirNASCAFolder,
			//		string.Format("{0}_{1}_{2}", lotText, magazineNo, plantCd));
			//}
			//else
			//{
			//	filePath = Path.Combine(setting.DirNASCAFolder,
			//		string.Format("{0}_{1}_{2}_{3}", lotText, magazineNo, plantCd, targetFileIdentity));
			//}

			
			
			filePath = Path.Combine(setting.DirNASCAFolder,
				string.Format("{0}_{1}_{2}", lotText, magazineNo, plantCd));

			if (string.IsNullOrEmpty(targetFileIdentity) == false && procNO.HasValue == false)
			{
				filePath += string.Format("_{0}", targetFileIdentity);
			}
			else if (string.IsNullOrEmpty(targetFileIdentity) == false && procNO.HasValue == true)
			{
				filePath += string.Format("_{0}_{1}", targetFileIdentity, procNO);
			}

			string completeNasFilePath = Create(filePath, data);

			if (procNO.HasValue)
			{
				if (ConnectDB.IsDefectEnd(lineNo, lotNo, procNO.Value))
				{
					ConnectDB.UpdateIsDefectEnd(lineNo, lotNo, procNO.Value, false);
				}
			}

			return completeNasFilePath;
        }

        private static string Create(string path, string data)
        {
			//既に同一のファイルが存在する場合は削除
			if (File.Exists(path))
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("NasFile Create() 既に作成中ファイルがある為、削除 path={0}", path));
				File.Delete(path);
			}

			string completeNasFilePath = path + NASCA_DEFECT_FILE_EXT;
			if (File.Exists(completeNasFilePath))
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("NasFile Create() 既に.nasファイルがある為、削除 path={0}", completeNasFilePath));
				File.Delete(path + NASCA_DEFECT_FILE_EXT);
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("NasFile Create() 作成開始 : path={0}, data={1}", path, data));
			StreamWriter textFiler = File.CreateText(path);
			textFiler.Write(data);
			textFiler.Close();

			FileInfo createdFileInfo = new FileInfo(path);
			createdFileInfo.MoveTo(completeNasFilePath);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("NasFile Create() 作成完了 : path={0}", completeNasFilePath));

			return completeNasFilePath;
        }

        public static bool Exists(string magazineNo, string plantCd, int lineNo) 
        {
            SettingInfo setting = SettingInfo.GetSettingInfoPerLine(lineNo);

            string[] files = Directory.GetFiles(setting.DirNASCAFolder, string.Format("*_{0}_{1}.nas", magazineNo, plantCd));
            if (files.Count() == 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

		public static List<NascaDefectFile.DefectQtyInfo> ConvertLENSDefectData(LENS2_Api.EICS.Defect[] defectArray)
		{
			List<NascaDefectFile.DefectQtyInfo> defectList = new List<NascaDefectFile.DefectQtyInfo>();

			foreach (LENS2_Api.EICS.Defect defect in defectArray)
			{
				//if(defect.QcParam_NO
				NascaDefectFile.DefectQtyInfo defectQtyInfo = new NascaDefectFile.DefectQtyInfo();

				defectQtyInfo.ClassCd = defect.DefClass_CD;
				defectQtyInfo.CauseCd = defect.DefCause_CD;
				defectQtyInfo.ErrCd = defect.NascaErr_NO;
				defectQtyInfo.Qty = defect.Qty;

				defectList.Add(defectQtyInfo);
			}

			return defectList;
		}
    }
}
