using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.Base
{
	interface IMachineAddMPD
	{
		string EXT_MPDTRIG_FILE();

		////mpdﾌｧｲﾙを処理する
		//void MpdProcess(LSETInfo lsetInfo, string targetFileIdentity, string targetDir, string lotNO, string magNO, int procNO, string equipNO);

		void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO);

		void BackupDoneStartFiles(List<string> chkTargetFileList, string targetFileDir, string lotNO, DateTime doneDt);

		void BackupDoneEndFiles(List<string> fileListInEndDir, string distDir, string lotNO, DateTime doneDt);

		bool IsFinishedLENSProcess(string lotNo, int procNo, int lineCD, string carrierNo, string plantCd);
	}
}
