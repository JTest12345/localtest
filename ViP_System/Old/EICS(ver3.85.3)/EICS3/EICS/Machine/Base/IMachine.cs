using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS.Machine
{
	interface IMachine
	{
		string Name { get; set; }
		string Code { get; set; }
		string AssetsNM { get; set; }
		int LineCD { get; set; }

		void CheckMachineFile(LSETInfo lsetInfo);
		void InitFirstLoop(LSETInfo lsetInfo);
		void InitHSMS(LSETInfo lsetInfo);
		void CommunicationHSMS(ref bool firstContactFG);
		List<ErrMessageInfo> GetErrorMessageList();
		void InitErrorMessageList();
		void CheckFile(LSETInfo lsetInfo);

        //<--NASCA不良のエラー判定実施:TnLogWaitingQueue(未判定)⇒判定⇒TnLogへ
        void CheckNascaError(LSETInfo lsetInfo);
        //-->NASCA不良のエラー判定実施
        Constant.MachineStatus GetMachineStatus();
		void SendStopSignalToMachine(LSETInfo lsetInfo, string plcMemAddr);
		//void StopMachineByHSMS();
	}
}
