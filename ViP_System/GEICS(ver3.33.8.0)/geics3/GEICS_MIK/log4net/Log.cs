using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace log4net
{

    /*
     * 2007.4.26 furukawa
     * ���O���c���ɂ�
     *      Log.Logger��DEBUG�`FATAL���\�b�h���g�p����B
     * 
     * ���O�����[�����M����ɂ�
     *      Log()������SMTP�̃R�����g���O���B
     *      ConfigSmtpAppender���\�b�h������ݒ肷��B
     *      �p�ɂɐݒ�̕ύX���\�z�����ꍇ�́A�ݒ荀�ڂ�K�X�O����Config�t�@�C���֓������B
     * 
     * 
     * Log�̏o�͌`����ύX����ɂ�
     *      private static log4net.Layout.ILayout defaultLayout �̓��e��ύX����B
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
        /// �o�͌`����`
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
        /// �ÓI�R���X�g���N�^
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

        #region Appender������
        /// <summary>
        /// SmtpAppender�̏�����
        /// </summary>
        private static void ConfigSmtpAppender()
        {
            log4net.Appender.SmtpAppender smtp = new log4net.Appender.SmtpAppender();
            smtp.SmtpHost = "pws4";
            smtp.Subject = "N�H��C�����C�����";
            smtp.To = "yosuke.matsushima@nichia.co.jp";
            smtp.From = "yosuke.matsushima@nichia.co.jp";
            smtp.Layout = defaultLayout;
            smtp.Priority = System.Net.Mail.MailPriority.High;


            //�t�B���^�ݒ�
            smtp.AddFilter(GetRangeFilter(log4net.Core.Level.Error, log4net.Core.Level.Fatal));
            smtp.Evaluator = new log4net.Core.LevelEvaluator(log4net.Core.Level.Error);

            smtp.ActivateOptions(); //�K�{
            log4net.Config.BasicConfigurator.Configure(smtp);
        }



        /// <summary>
        /// RollingFileAppender�̏�����
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

            //�t�B���^�ݒ�
            rolling.AddFilter(GetRangeFilter(log4net.Core.Level.Info, log4net.Core.Level.Fatal));

            rolling.ActivateOptions();�@//�K�{
            log4net.Config.BasicConfigurator.Configure(rolling);
        }


        /// <summary>
        /// TraceAppender�̏�����
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
        /// �t�B���^�𐶐�
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


