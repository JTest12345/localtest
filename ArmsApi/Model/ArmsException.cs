using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class ArmsException : ApplicationException
    {
        public string ClientMessage { get; set; }

        public ArmsException(string msg, Exception innerException)
            : base(msg, innerException)
        {
            Log.SysLog.Error(msg + innerException.ToString());
            this.ClientMessage = msg + innerException.ToString();
        }

        public ArmsException(string msg)
            : base(msg)
        {
            Log.SysLog.Error(msg);
            this.ClientMessage = msg;
        }
    }
}
