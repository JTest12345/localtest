using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;

namespace ArmsApi
{
    public class Log
    {
        static Log()
        {
            try
            {
                // Console
                ConfigConsoleAndTraceAppender();

                // SysLog
                ConfigSysLogAppender();

                // RBLog
                ConfigRBLogAppender();

                // ApiLog
                ConfigApiLogAppender();

                // DelLog
                ConfigDelLogAppender();

                // AGVLog
                ConfigAGVLogAppender();


                // SMTP
                ConfigSmtpAppender();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Api呼び出しログ
        /// </summary>
        public static ILog ApiLog = LogManager.GetLogger("ApiLogger");

        /// <summary>
        /// ロボット搬送動作、装置センサー監視動作用
        /// </summary>
        public static ILog RBLog = LogManager.GetLogger("RobotLogger");

        /// <summary>
        /// その他のシステムログ
        /// </summary>
        public static ILog SysLog = LogManager.GetLogger("SystemLogger");

        /// <summary>
        /// データ削除ログ
        /// </summary>
        public static ILog DelLog = LogManager.GetLogger("DeleteLogger");

        /// <summary>
        /// AGVログ
        /// </summary>
        public static ILog AGVLog = LogManager.GetLogger("AGVLogger");


        /// <summary>
        /// 出力形式定義
        /// </summary>
        private static log4net.Layout.ILayout defaultLayout
        {
            get
            {
                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%d,%-5p,%m%n");
                layout.Header = "[START]" + Environment.NewLine;
                layout.Footer = Environment.NewLine + "[END]";
                return layout;
            }
        }

        /// <summary>
        /// 出力形式定義(Mail)
        /// </summary>
        private static log4net.Layout.ILayout mailLayout
        {
            get
            {
                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%d,%-5p,%m%n");
                layout.Header = "[START]" + Environment.NewLine + Config.Settings.ErrorMailBodyHeader + Environment.NewLine;
                layout.Footer = Environment.NewLine + "[END]";
                return layout;
            }
        }

        #region Appender初期化

        /// <summary>
        /// SmtpAppenderの初期化
        /// </summary>
        private static void ConfigSmtpAppender()
        {
            log4net.Appender.SmtpAppender smtp = new log4net.Appender.SmtpAppender();
            smtp.SmtpHost = "HQSMTP1.nichia.local";
            smtp.Subject = Config.Settings.ErroMailTitle;

            foreach (string address in Config.Settings.ErrorMailTo)
            {
                if (!string.IsNullOrEmpty(smtp.To)) smtp.To += ",";
                smtp.To += ("<" + address + ">");
            }

            smtp.From = Config.Settings.ErrorMailFrom;
            smtp.Layout = mailLayout;
            smtp.Priority = System.Net.Mail.MailPriority.High;
            
            //フィルタ設定
            log4net.Filter.LevelRangeFilter rangeFilter = new log4net.Filter.LevelRangeFilter();
            rangeFilter.LevelMin = log4net.Core.Level.Fatal;
            rangeFilter.LevelMax = log4net.Core.Level.Fatal;
            smtp.AddFilter(rangeFilter);
			smtp.BufferSize = 100;

            //Evaluator
			//log4net.Core.LevelEvaluator eval = new log4net.Core.LevelEvaluator();
			//eval.Threshold = log4net.Core.Level.Error;
			//smtp.Evaluator = eval;

            smtp.ActivateOptions(); //必須
            log4net.Config.BasicConfigurator.Configure(smtp);
        }

        /// <summary>
        /// SysLogのRollingFile
        /// </summary>
        private static void ConfigSysLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "C:\\ARMS_VSP\\LOG\\LOG";
            rolling.DatePattern = "yyyyMMdd";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;
            
            //フィルタ設定
            log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter();
            filter.LoggerToMatch = "SystemLogger";
            filter.AcceptOnMatch = true;

            log4net.Filter.LevelRangeFilter rangeFilter = new log4net.Filter.LevelRangeFilter();
            rangeFilter.LevelMin = log4net.Core.Level.Info;
            rangeFilter.LevelMax = log4net.Core.Level.Fatal;
            filter.Next = rangeFilter;

            rolling.AddFilter(filter);
            rolling.AddFilter(new log4net.Filter.DenyAllFilter());

            rolling.ActivateOptions();　//必須
            log4net.Config.BasicConfigurator.Configure(rolling);
        }
        
        /// <summary>
        /// RBログのRollingFile
        /// </summary>
        private static void ConfigRBLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "C:\\ARMS_VSP\\LOG\\MOVELOG";
            rolling.DatePattern = "yyyyMMdd";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            log4net.Filter.LoggerMatchFilter loggerFilter = new log4net.Filter.LoggerMatchFilter();
            loggerFilter.LoggerToMatch = "RobotLogger";
            loggerFilter.AcceptOnMatch = true;
            rolling.AddFilter(loggerFilter);
            rolling.AddFilter(new log4net.Filter.DenyAllFilter());

            rolling.ActivateOptions();　//必須
            log4net.Config.BasicConfigurator.Configure(rolling);
        }

        /// <summary>
        /// DeleteログのRollingFile
        /// </summary>
        private static void ConfigDelLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "C:\\ARMS_VSP\\LOG\\DELETELOG";
            rolling.DatePattern = "yyyyMMdd";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter();
            filter.LoggerToMatch = "DeleteLogger";
            filter.AcceptOnMatch = true;

            log4net.Filter.LevelRangeFilter rangeFilter = new log4net.Filter.LevelRangeFilter();
            rangeFilter.LevelMin = log4net.Core.Level.Info;
            rangeFilter.LevelMax = log4net.Core.Level.Fatal;
            filter.Next = rangeFilter;

            rolling.AddFilter(filter);
            rolling.AddFilter(new log4net.Filter.DenyAllFilter());

            rolling.ActivateOptions();　//必須
            log4net.Config.BasicConfigurator.Configure(rolling);
        }

        /// <summary>
        /// NASCAログのAppender
        /// </summary>
        private static void ConfigApiLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "C:\\ARMS_VSP\\LOG\\APILOG";
            rolling.DatePattern = "yyyyMMdd";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            log4net.Filter.LoggerMatchFilter loggerFilter = new log4net.Filter.LoggerMatchFilter();
            loggerFilter.LoggerToMatch = "ApiLogger";
            loggerFilter.AcceptOnMatch = true;
            rolling.AddFilter(loggerFilter);
            rolling.AddFilter(new log4net.Filter.DenyAllFilter());


            rolling.ActivateOptions();　//必須
            log4net.Config.BasicConfigurator.Configure(rolling);
        }


        /// <summary>
        /// AGVログのAppender
        /// </summary>
        private static void ConfigAGVLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "C:\\ARMS_VSP\\LOG\\AGVLOG";
            rolling.DatePattern = "yyyyMMdd";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            log4net.Filter.LoggerMatchFilter loggerFilter = new log4net.Filter.LoggerMatchFilter();
            loggerFilter.LoggerToMatch = "AGVLogger";
            loggerFilter.AcceptOnMatch = true;
            rolling.AddFilter(loggerFilter);
            rolling.AddFilter(new log4net.Filter.DenyAllFilter());

            rolling.ActivateOptions();　//必須
            log4net.Config.BasicConfigurator.Configure(rolling);
        }

        /// <summary>
        /// TraceAppenderの初期化
        /// </summary>
        private static void ConfigConsoleAndTraceAppender()
        {
            //  Console
            log4net.Config.BasicConfigurator.Configure();

            //  Trace
            log4net.Appender.TraceAppender trace = new log4net.Appender.TraceAppender();
            trace.Layout = defaultLayout;

            trace.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(trace);
        }
        
        #endregion

        #region テキストボックスへのログ出力
        
        /// <summary>
        /// テキストボックス吐き出し用のMemoryAppenders
        /// </summary>
        private static List<MemoryAppender> MemoryAppenders = new List<MemoryAppender>();

        /// <summary>
        /// 500ms毎にILog内のメモリーアペンダーのイベントを指定LogActionに吐き出し
        /// スレッドプール使用
        /// </summary>
        /// <param name="log"></param>
        /// <param name="logAction"></param>
        public static async void SetupMemoryLogToAction(ILog log, string patternLayoutFormat, Action<string> logAction)
        {
            //TODO　セットアップ重複時の動作確認

            if (logAction == null) throw new ApplicationException("LogAction Not Found");

            string memoryAppenderName = log.Logger.Name + "MemoryAppender";

            log4net.Appender.MemoryAppender ma = null;
            if (MemoryAppenders.Exists(a => a.Name == memoryAppenderName) == false)
            {
                //MemoryAppender作成
                ma = new MemoryAppender();
                ma.Layout = new log4net.Layout.PatternLayout(patternLayoutFormat);
                ma.Fix = log4net.Core.FixFlags.All;
                ma.Name = memoryAppenderName;

                //フィルタ設定
                log4net.Filter.LoggerMatchFilter loggerFilter = new log4net.Filter.LoggerMatchFilter();
                loggerFilter.LoggerToMatch = log.Logger.Name;
                loggerFilter.AcceptOnMatch = true;
                ma.AddFilter(loggerFilter);
                ma.AddFilter(new log4net.Filter.DenyAllFilter());
                ma.ActivateOptions();
                log4net.Config.BasicConfigurator.Configure(ma);
                MemoryAppenders.Add(ma);
            }
            else
            {
                ma = MemoryAppenders.Where(a => a.Name == memoryAppenderName).Single();
            }

            try
            {
                await (Task.Run(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(500);
                            log4net.Core.LoggingEvent[] evs = ma.GetEvents();
                            foreach (log4net.Core.LoggingEvent ev in evs)
                            {
                                if (ma.Layout != null)
                                {
                                    ma.Layout.Format(sw, ev);
                                    logAction(sw.ToString());
                                    sb.Clear();
                                }
                                else
                                {
                                    logAction(ev.RenderedMessage);
                                }
                            }
                            ma.Clear();
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                log.Info("MemoryLogToAction再起動: " + ex.ToString());
                SetupMemoryLogToAction(log, patternLayoutFormat, logAction);
            }
        }
        #endregion
    }
}
