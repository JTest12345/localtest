using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	class DBMachineInfoKOSAKA_KE205 : CIFSBasedMachine
	{
		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_ZDDB; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_LEDDB; }

		private string BADMARK_HEADER_NM = "ﾊﾞｯﾄﾞﾏｰｸｶｳﾝﾄ";

		protected override string GetStartableFileIdentity()
		{
			if (IsStartableProcess())
			{
				return CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);
			}
			else
			{
				return null;
			}
		}

		//追加処理
		protected override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			if (settingInfoPerLine.GetBMCount(lsetInfo.EquipmentNO))
			{
				//不良数集計の処理
				OutputNasFileProcess(lsetInfo.InlineCD, lotNO, magNO, equipNO, targetFileIdentity);
			}
		}

		private void OutputNasFileProcess(int lineCD, string lotNO, string magNO, string equipNO, string targetFileIdentity)
		{
			int defectCt = CalcBadmarkCt(targetFileIdentity);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			List<NascaDefectFile.DefectQtyInfo> defectInfoList = new List<NascaDefectFile.DefectQtyInfo>();

			NascaDefectFile.DefectQtyInfo defectInfo = new NascaDefectFile.DefectQtyInfo();

			defectInfo.CauseCd = settingInfoPerLine.BMCauseCD;
			defectInfo.ClassCd = settingInfoPerLine.BMClassCD;
			defectInfo.ErrCd = settingInfoPerLine.BMDefectCD;
			defectInfo.Qty = defectCt;

			if (defectCt > 0)
			{
				defectInfoList.Add(defectInfo);
			}

			//不良情報を受け取ってファイル出力
			NascaDefectFile.Create(lotNO, magNO, equipNO, lineCD, defectInfoList);
		}

		private int CalcBadmarkCt(string targetFileIdentity)
		{
			string[] txtArray;

			string fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + @"\." + EXT_END_FILE();

			List<string> fileNmList = Common.GetFiles(EndFileDir, fileSearchPattern);

			if (fileNmList.Count == 0)
			{
				string errMsg = string.Format(
					"ﾌｧｲﾙが存在しません。 {0}/{1}/{2} 監視ﾊﾟｽ:{3} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{4}"
					, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, EndFileDir, fileSearchPattern);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, errMsg);

				throw new ApplicationException(errMsg);
			}
			else if (fileNmList.Count > 1)
			{
				string errMsg = string.Format("同一のファイル種類、年月日時分秒文字列でファイルが複数抽出されました。 検索Dir:{0} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{1}",
					EndFileDir, fileSearchPattern);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, errMsg);

				throw new ApplicationException(errMsg);

			}
			//日付文字列とファイル種類が同じで複数ファイルが取れたらどうする？

			string fileNm = fileNmList.Single();

			string filePath = Path.Combine(EndFileDir, fileNm);
			if (MachineFile.TryGetTxt(filePath, LF, out txtArray, EncodeStr, 0) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ内容取得失敗 ﾌｧｲﾙﾊﾟｽ:{0} 行区切り文字{1}", filePath, @LF));
			}

			int badmarkColIndex = MachineFile.GetHeaderIndex(txtArray, BADMARK_HEADER_NM, true);
			if (badmarkColIndex == -1) 
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ内容取得失敗 {0}列が見つかりません。ﾌｧｲﾙﾊﾟｽ:{1}", BADMARK_HEADER_NM, filePath));
			}

			//行が進むにつれ、バッドマーク数はカウントアップするため最大値を取得
			int badmarkCt = 0; int badmarkMaxCt = 0;
			foreach (string rowData in txtArray)
			{
				if (string.IsNullOrWhiteSpace(rowData)) continue;

				string[] rowArray = rowData.Split(',');

				if (int.TryParse(rowArray[badmarkColIndex], out badmarkCt) == false)
				{
					continue;	
				}

				if (badmarkMaxCt < badmarkCt)
					badmarkMaxCt = badmarkCt;
			}

			return badmarkMaxCt;
		}
	}
}
