using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
    class Log
    {
        static Log()
        {
            ConfigInfoLogAppender();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Info(string lotNo, string functionNm, string message)
        {
            LogManager.GetLogger("LENS2ApiLogger")
                .Info(string.Format("[ロットNo]{0} [処理]{1} : {2}", lotNo, functionNm, message));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Error(string lotNo, string functionNm, string message)
        {
            LogManager.GetLogger("LENS2ApiLogger")
                .Error(string.Format("[ロットNo]{0} [処理]{1} : {2}", lotNo, functionNm, message));

            //メール送信
        }

        public static void Exclamation(string lotNo, string functionNm, string message)
        {
            LogManager.GetLogger("LENS2ApiLogger")
                .Fatal(string.Format("[ロットNo]{0} [処理]{1} : {2}", lotNo, functionNm, message));
        }

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
        /// InfoLoggerのRollingFile
        /// </summary>
        private static void ConfigInfoLogAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.File = "APILOG\\LOG";
            rolling.DatePattern = "yyyyMMdd'.log'";
            rolling.StaticLogFileName = false;
            rolling.AppendToFile = true;
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            rolling.Layout = defaultLayout;
            
            //フィルタ設定
            log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter();
            filter.LoggerToMatch = "LENS2ApiLogger";
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
    }
}
