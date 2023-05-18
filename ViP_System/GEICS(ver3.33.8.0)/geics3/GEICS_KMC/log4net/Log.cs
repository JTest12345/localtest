using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace log4net
{

    /*
     * 2007.4.26 furukawa
     * ログを残すには
     *      Log.LoggerのDEBUG～FATALメソッドを使用する。
     * 
     * ログをメール送信するには
     *      Log()内部のSMTPのコメントを外す。
     *      ConfigSmtpAppenderメソッド内部を設定する。
     *      頻繁に設定の変更が予想される場合は、設定項目を適宜外部のConfigファイルへ逃がす。
     * 
     * 
     * Logの出力形式を変更するには
     *      private static log4net.Layout.ILayout defaultLayout の内容を変更する。
     *  
     * */
    public abstract class Log
    {

        #region property
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("log4net");
        /// <summary>
        /// log4net.ILog
        /// </summary>
        public static log4net.ILog Logger
        {
            get { return logger; }
        }


        /// <summary>
        /// 出力形式定義
        /// </summary>
        private static log4net.Layout.ILayout defaultLayout
        {
            get
            {
                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%d,%-5p,-,%m%n");
                layout.Header = "[START]" + Environment.NewLine;
                layout.Footer = Environment.NewLine + "[END]";
                return layout;
            }
        }
        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Log()
        {
            //  Console
            ConfigConsoleAndTraceAppender();

            //  RollingFile
            ConfigRollingFileAppender();

            //  SMTP
            ConfigSmtpAppender();

        }

        #region Appender初期化
        /// <summary>
        /// SmtpAppenderの初期化
        /// </summary>
        private static void ConfigSmtpAppender()
        {
            log4net.Appender.SmtpAppender smtp = new log4net.Appender.SmtpAppender();
            smtp.SmtpHost = "pws4";
            smtp.Subject = "N工場インライン情報";
            smtp.To = "yosuke.matsushima@nichia.co.jp";
            smtp.From = "yosuke.matsushima@nichia.co.jp";
            smtp.Layout = defaultLayout;
            smtp.Priority = System.Net.Mail.MailPriority.High;


            //フィルタ設定
            smtp.AddFilter(GetRangeFilter(log4net.Core.Level.Error, log4net.Core.Level.Fatal));
            smtp.Evaluator = new log4net.Core.LevelEvaluator(log4net.Core.Level.Error);

            smtp.ActivateOptions(); //必須
            log4net.Config.BasicConfigurator.Configure(smtp);
        }



        /// <summary>
        /// RollingFileAppenderの初期化
        /// </summary>
        private static void ConfigRollingFileAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "Log.txt";
            rolling.StaticLogFileName = true;
            rolling.AppendToFile = true;
            rolling.MaxSizeRollBackups = 5;
            rolling.MaximumFileSize = "10000KB";
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            rolling.AddFilter(GetRangeFilter(log4net.Core.Level.Info, log4net.Core.Level.Fatal));

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

        #region private method
        /// <summary>
        /// フィルタを生成
        /// </summary>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        private static log4net.Filter.IFilter GetRangeFilter(log4net.Core.Level minLevel, log4net.Core.Level maxLevel)
        {
            log4net.Filter.LevelRangeFilter rangeFilter = new log4net.Filter.LevelRangeFilter();
            rangeFilter.LevelMin = minLevel;
            rangeFilter.LevelMax = maxLevel;

            return rangeFilter;
        }

        #endregion


    }

}


