using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	/// <summary>
	/// マウンタ
	/// </summary>
	class MTMachineInfoYAMAHA : CIFSBasedMachine
	{
		protected override string GetStartableFileIdentity()
		{
			string targetFileIdentity = string.Empty;
			
			if (IsStartableProcess())
				targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);
			return targetFileIdentity;
		}

		protected override List<string> GetBackupTargetStartFiles(string targetFileIdentity)
		{
			return Common.GetFiles(StartFileDir, string.Format("^{0}.*", targetFileIdentity));
		}

		protected override bool IsFinishedLENSProcess(string lotNo, int procNo, int lineCD, string carrierNo, string plantCd)
		{
			return Database.LENS.WorkResult.IsComplete(lotNo, procNo, lineCD, carrierNo, plantCd);
		}

		public override void BackupFileEndTiming(string targetFileIdentity, string lotNo, DateTime dt)
		{
			// 完了フォルダ内のファイル群の取得
			List<string> backupEndFileList = GetBackupTargetFile(targetFileIdentity);

			backupEndFileList.AddRange(GetProcessedMpdFile(targetFileIdentity));

			BackupDoneEndFiles(backupEndFileList, EndFileDir, lotNo, dt);

			//In側のtrgファイルのバックアップ
			List<string> backupStartFileList = GetBackupTargetStartFiles(targetFileIdentity);
			BackupDoneStartFiles(backupStartFileList, StartFileDir, lotNo, dt);
		}

		protected override List<string> GetBackupTargetFile(string targetFileIdentity)
		{
			List<string> fileListInEndDir = Common.GetFiles(EndFileDir, string.Format("{0}*", targetFileIdentity));
			fileListInEndDir = fileListInEndDir
				.Where(f => Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_OK_FILE)
					&& Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_NG_FILE)).ToList();
					//&& Path.GetExtension(f) != string.Format(".{0}", EXT_MPD_FILE)).ToList();

			return fileListInEndDir;
		}

		protected List<string> GetProcessedMpdFile(string targetFileIdentity)
		{
			List<string> processedMpdFileList = Common.GetFiles(EndFileDir, string.Format("^_{0}.*{1}", targetFileIdentity, EXT_MPD_FILE));

			return processedMpdFileList;
		}

	}
}
