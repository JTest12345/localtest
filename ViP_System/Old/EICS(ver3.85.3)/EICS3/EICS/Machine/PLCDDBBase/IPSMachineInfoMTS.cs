using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class IPSMachineInfoMTS : PLCDDGBasedMachine
	{
        private const int IPS_PRM_TIMING_NO = 44;

        //IPS(外製)
        private const string PLC_READY_ADDR = "DM2900";         //開始要求信号(OFF:0 or ON:1)  PLC(1)→上位(0)
        private const string PLC_START_JUDGE_ADDR = "DM2901";   //開始許可信号( NG:0 or OK:2)  PLC(0)←上位(2)

        private const int PLC_READY_OFF = 0;

        private const int PLC_START_JUDGE_NG = 1;
        private const int PLC_START_JUDGE_OK = 2;

        public IPSMachineInfoMTS(LSETInfo lsetInfo)
		{
			//InitPropAtLoop(lsetInfo);
			//InitPLC(lsetInfo);
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				base.machineStatus = Constant.MachineStatus.Runtime;

				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);

                //装置スタート時の要求信号あれば
                if (isRequestStartCheck())
				{
                    CreateFileProcess(lsetInfo, true);

                    List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
                    StartingProcess(lsetInfo, out errMsgList);

                    //装置パラメータが判定NGの場合
                    if (errMsgList.Count > 0)
                    {
                        plc.SetWordAsDecimalData(PLC_START_JUDGE_ADDR, PLC_START_JUDGE_NG); //NG=1
                    }
                    //装置パラメータが判定OKの場合
                    else
                    {
                        plc.SetWordAsDecimalData(PLC_START_JUDGE_ADDR, PLC_START_JUDGE_OK); //OK=2
                    }
                    //判定結果に関わらず、データ確認要求を落とす。
                    plc.SetWordAsDecimalData(PLC_READY_ADDR, PLC_READY_OFF);//OFF=0 要求信号立ち下げ
                }

            }
			catch (Exception err)
			{
				throw;
			}
		}

		private bool isRequestStartCheck()
		{
			if (plc.GetDataAsString(PLC_READY_ADDR, 1, PLC.DT_DEC_16BIT) == PLC.BIT_ON)
			{
				return true;
			}
			return false;
		}
    }
}
