using System;
using System.Linq;
using NLog;

namespace Oskas
{
    public static class OskNLog
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        //
        // Action for console
        //
        static Action<string, int> Console;
        public static void InitConsoleAction(Action<string, int> act)
        {
            Console = act;
        }

        public static void setFolderName(string fldname)
        {
            logger = LogManager.GetLogger("Log");
            logger.Factory.Configuration.Variables.Add("runtime", fldname);
            logger.Factory.ReconfigExistingLoggers();
        }

        public static void Log(string msg, int level)
        {
            try
            {   if (level == Cnslcnf.msg_debug)
                {
                    logger.Debug(msg);
                }
                else if (level == Cnslcnf.msg_info)
                {
                    logger.Info(msg);
                }
                else if (level == Cnslcnf.msg_warn)
                {
                    logger.Warn(msg);
                }
                else if (level == Cnslcnf.msg_alarm)
                {
                    logger.Warn(msg);
                }
                else if (level == Cnslcnf.msg_error)
                {
                    logger.Error(msg);
                }
                else if (level == Cnslcnf.msg_fatal)
                {
                    logger.Fatal(msg);
                }

                Console(msg, level);
            }
            catch (Exception e)
            {
                Console(e.ToString(), 0);
            }
            
        }
    }
}