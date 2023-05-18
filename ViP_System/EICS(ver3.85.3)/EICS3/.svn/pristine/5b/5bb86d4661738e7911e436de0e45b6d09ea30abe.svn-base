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
	/// 最終段にくる不良集計が必要になるマウンタ装置クラス
	/// </summary>
	class LastMTMachineInfoYAMAHA : MTMachineInfoYAMAHA
	{
		protected override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			string fileSearchPatternStr = string.Format("^{0}_.*[.]{1}", targetFileIdentity, EXT_MPD_FILE);

			List<string> mpdFilePathList = Common.GetFiles(this.EndFileDir, fileSearchPatternStr, EXT_MPD_FILE);

			if (mpdFilePathList.Count == 0)
			{
				throw new ApplicationException(string.Format("ﾄﾘｶﾞを確認しましたが、mpdﾌｧｲﾙが存在しません。監視Dir:{0} ﾌｧｲﾙ識別子:{1}", this.EndFileDir, targetFileIdentity));
			}
			else if (mpdFilePathList.Count > 1)
			{
				throw new ApplicationException(string.Format("同一識別子にて複数mpdﾌｧｲﾙが存在します。監視Dir:{0} ﾌｧｲﾙ識別子:{1}", this.EndFileDir, targetFileIdentity));
			}

			//集計とnasファイル出力の実行
			CountDefect(mpdFilePathList[0], lsetInfo.InlineCD, lotNO, magNO, equipNO, procNO, targetFileIdentity, lsetInfo.EquipInfo.ErrConvWithProcNo);

			string parentDir = Path.Combine(Path.GetDirectoryName(mpdFilePathList[0])
				, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());

			string processedFileNm = string.Format("_{0}", Path.GetFileName(mpdFilePathList[0]));

			File.Move(mpdFilePathList[0], Path.Combine(parentDir, processedFileNm));
		}

		protected override List<string> GetBackupTargetFile(string targetFileIdentity)
		{
			List<string> fileListInEndDir = Common.GetFiles(EndFileDir, string.Format("{0}*", targetFileIdentity));
			fileListInEndDir = fileListInEndDir
				.Where(f => Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_OK_FILE)
					&& Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_NG_FILE)
			&& Path.GetExtension(f) != string.Format(".{0}", EXT_MPD_FILE)).ToList();

			return fileListInEndDir;
		}
	}
}
