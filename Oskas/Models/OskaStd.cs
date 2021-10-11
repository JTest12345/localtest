using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oskas
{
    public class Cnslcnf
    {
        //Console message Lebel(INT)
        public const int msg_debug = 1;
        public const int msg_info = 2;
        public const int msg_warn = 3;
        public const int msg_error = 4;
        public const int msg_fatal = 5;
        public const int msg_task = 6;
        public const int msg_alarm = 7;
        public const int msg_detect = 8;

        
        
        //Message Max ROW
        public const int mesMaxLen = 1000;
        public int mesLen { get; set; }
    }
}
