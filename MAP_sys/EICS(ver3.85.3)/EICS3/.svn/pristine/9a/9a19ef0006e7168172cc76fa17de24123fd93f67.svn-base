using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
    //LM(内製) DMC/G11
    class LM2MachineInfo : PLCDDGBasedMachine
    {
        private const string PLC_READY_ADDR = "EM50050";         //開始要求信号(OFF:0 or ON:1)  PLC(1)→上位(0)
        private const string PLC_START_JUDGE_ADDR = "EM50020";   //開始許可信号( NG:0 or OK:2)  PLC(0)←上位(2)

        private const int PLC_READY_OFF = 0;

        private const int PLC_START_JUDGE_NG = 1;
        private const int PLC_START_JUDGE_OK = 0;
        protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "EM50020"; }

        public LM2MachineInfo(LSETInfo lsetInfo)
        {
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {

            base.machineStatus = Constant.MachineStatus.Runtime;
            InitPropAtLoop(lsetInfo);
            InitPLC(lsetInfo);

            //装置スタート時の要求信号あれば
            if (isRequestStartCheck())
            {
                try
                {
                    //開始時パラメータを装置メモリから取得し、wst/finファイル作成
                    CreateFileProcess(lsetInfo, true);

                    //trgファイル確認後、wstファイル判定。判定結果はerrMsgListへ。
                    List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
                    StartingProcess(lsetInfo, out errMsgList);

                    MachineStopProcess(lsetInfo, true);
                    ResponseOKFile(true, lsetInfo);
                }
                catch (Exception)
                {
                    plc.SetWordAsDecimalData(PLC_START_JUDGE_ADDR, PLC_START_JUDGE_NG);
                    plc.SetWordAsDecimalData(PLC_READY_ADDR, PLC_READY_OFF);
                    throw;
                }
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
