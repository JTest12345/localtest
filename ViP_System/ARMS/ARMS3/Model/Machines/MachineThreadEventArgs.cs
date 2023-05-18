using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{

    public class MachineThreadEventArgs : EventArgs
    {
        public MachineThreadEventArgs(ARMSThreadObject sender, ExitStatus status, Exception ex)
        {
            this.Sender = sender;
            this.Exception = ex;
            this.Status = status;
        }

        public enum ExitStatus
        {
            //通常終了
            NormalExit,
            //強制終了
            UserAbort,
            //スレッド内例外による終了
            Exception,
            //他スレッド動作中
            ThreadAlreadyRun,
        }

        public ARMSThreadObject Sender { get; set; }
        public Exception Exception { get; set; }
        public ExitStatus Status { get; set; }
    }
}
