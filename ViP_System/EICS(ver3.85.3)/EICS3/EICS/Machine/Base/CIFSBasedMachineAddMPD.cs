using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.Base
{
	class CIFSBasedMachineAddMPD : CIFSBasedMachine, IMachineAddMPD
	{
		public new string EXT_MPDTRIG_FILE() { return EXT_FIN2_FILE; }

		protected override string GetStartableFileIdentity()
		{
			string targetFileIdentity = string.Empty;

			if (IsStartableProcess())
			{
				targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);
			}

			return targetFileIdentity;
		}

		//mpdﾌｧｲﾙを処理する
		public static void MpdProcess(LSETInfo lsetInfo, string targetFileIdentity, string targetDir, string lotNO, string magNO, int procNO, string equipNO)
		{
			//現状はmpdﾌｧｲﾙから不良登録用のﾌｧｲﾙ作成しかしてないが、機能追加するようになれば関数細分化する 2016/5/17 n.yoshi

			//不良集計関数を呼び出してnasファイルを作成する
			string fileSearchPatternStr = string.Format("{0}", targetFileIdentity);

			List<string> mpdFilePathList = Common.GetFiles(targetDir, fileSearchPatternStr, EXT_MPD_FILE);

			if (mpdFilePathList.Count == 0)
			{
				throw new ApplicationException(string.Format("ﾄﾘｶﾞを確認しましたが、mpdﾌｧｲﾙが存在しません。監視Dir:{0} ﾌｧｲﾙ識別子:{1}", targetDir, targetFileIdentity));
			}
			else if (mpdFilePathList.Count > 1)
			{
				throw new ApplicationException(string.Format("同一識別子にて複数mpdﾌｧｲﾙが存在します。監視Dir:{0} ﾌｧｲﾙ識別子:{1}", targetDir, targetFileIdentity));
			}

			//集計とnasファイル出力の実行
			CountDefect(mpdFilePathList[0], lsetInfo.InlineCD, lotNO, magNO, equipNO, procNO, targetFileIdentity, lsetInfo.EquipInfo.ErrConvWithProcNo);
		}

		public virtual void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
				
		}

		public void BackupDoneStartFiles(List<string> chkTargetFileList, string targetFileDir, string lotNO, DateTime doneDt)
		{
			foreach (string chkTargetFile in chkTargetFileList)
			{
				if (chkTargetFile.Contains(EXT_MPD_FILE))
				{
					chkTargetFileList.Remove(chkTargetFile);
				}
			}

			// 処理したファイルの移動
			CIFS.BackupDoneFiles(chkTargetFileList, targetFileDir, lotNO, doneDt);
		}

		public void BackupDoneEndFiles(List<string> fileListInEndDir, string distDir, string lotNO, DateTime doneDt)
		{
			CIFS.BackupDoneFiles(fileListInEndDir, distDir, lotNO, doneDt);
		}

		public bool IsFinishedLENSProcess(string lotNo, int procNo, int lineCD, string carrierNo, string plantCd)
		{
			return Database.LENS.WorkResult.IsComplete(lotNo, procNo, lineCD, carrierNo, plantCd);
		}

		//
		//protected override void OutputResult(string targetFileIdentity, CheckResult chkResult, bool isStartTiming)
		//{
		//	if (isStartTiming)
		//	{
		//		base.OutputResult(targetFileIdentity, chkResult, isStartTiming);
		//	}
		//}
	}
}
