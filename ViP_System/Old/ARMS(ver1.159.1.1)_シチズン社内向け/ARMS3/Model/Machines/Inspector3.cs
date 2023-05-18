using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
    /// 外観検査機 RAIM (WB後検査/塗布後検査 SIGMA用も使用)
	/// </summary>
	public class Inspector3 : Inspector2
	{
		private const int DATA_NO_COL = 0;
		private const int MAG_NO_COL = 3;

		protected override void concreteThreadWork()
		{
			if (this.IsRequireOutput() == true)
			{
				workComplete();
			}

			if (this.IsWorkStartAutoComplete)
			{
				if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
				{
					workStart();
				}
			}

			CheckMachineLogFile();

			Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);

            //基板作業処理ありの場合
            if (this.IsSubstrateComplete)
            {
                //基板作業開始
                base.SubstrateWorkStart();

                //基板作業完了
                base.SubstrateWorkComplete();
            }
        }

		public void CheckMachineLogFile()
		{
			List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
			foreach (string logFile in logFiles)
			{
				if (MachineLog.IsLotFromFileName(logFile)) continue;

				if (IsStartLogFile(logFile) == false)
				{
					MachineLog mLog = parseMachineLog(logFile);
					if (mLog.IsUnknownData)
					{
						MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
						return;
					}

					Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
					if (svrMag == null)
						throw new ApplicationException(
							string.Format("装置ログ内のマガジンNo:{0}の稼働中マガジンが存在しません。", mLog.MagazineNo));

					AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
					int procNo = Process.GetNowProcess(lot).ProcNo;

					string changeFileName = string.Empty;

					string logFileName = Path.GetFileNameWithoutExtension(logFile);
					if (Regex.IsMatch(logFileName, "^log..._SM4.*$"))
					{
						changeFileName = logFileName.Replace("_SM4", "_SM1");
					}
					else if (Regex.IsMatch(logFileName, "^log..._SM5.*$"))
					{
						changeFileName = logFileName.Replace("_SM5", "_SM2");
					}
					else if (Regex.IsMatch(logFileName, "^log..._SM6.*$"))
					{
						changeFileName = logFileName.Replace("_SM6", "_SM3");
					}

					MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo, changeFileName);
				}
			}
		}

		private MachineLog parseMachineLog(string filepath, int retryCt)
		{
			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
			{
				throw new ApplicationException("ファイル解析中にエラーが発生しました：" + filepath);
			}

			MachineLog mLog = new MachineLog();
			string magNo = null;

			try
			{
				using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
				{
					int lineno = 0;
					while (sr.Peek() > 0)
					{
						string line = sr.ReadLine();

						line = line.Replace("\"", "");

						if (string.IsNullOrEmpty(line)) continue;

						//CSV各要素分解
						string[] data = line.Split(',');

						//DATA_NOが数字変換できない行は飛ばす
						int datano;
						if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

						string[] magStr = data[MAG_NO_COL].Split(' ');
						if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
						{
							//自動化用の30_を取り除き
							magNo = magStr[1];
						}
						else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
						{
							//高効率用の11_を取り除き
							magNo = magStr[1];
						}
						else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
						{
							//高効率用の13_を取り除き
							magNo = magStr[1];
						}
						else
						{
							magNo = data[MAG_NO_COL];
						}

						lineno = lineno + 1;
					}

					if (lineno == 0)
					{
						// データがヘッダのみを想定
						mLog.IsUnknownData = true;
						return mLog;
					}

					if (magNo == null) throw new ApplicationException("装置ログにマガジン番号が存在しません:" + filepath);
				}

				mLog.MagazineNo = magNo;
				return mLog;
			}
			catch (IOException)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return parseMachineLog(filepath, retryCt);
			}
		}
		private MachineLog parseMachineLog(string filepath)
		{
			return parseMachineLog(filepath, 0);
		}

		public bool IsStartLogFile(string fullName)
		{
			string fileName = Path.GetFileNameWithoutExtension(fullName);
			if (Regex.IsMatch(fileName, "^log..._OA.*$"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
