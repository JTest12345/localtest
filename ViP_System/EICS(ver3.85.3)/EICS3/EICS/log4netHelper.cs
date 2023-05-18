using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using log4net.Repository;
using log4net;
using log4net.Config;
using log4net.Appender;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]

namespace log4netHelper
{

    #region log4net関連定義　enum
    /// <summary>
    /// ログの種類（＝設定ファイルのlogger名）
    /// </summary>
    public enum LogType
    {
        ROOT,
        //DEFAULT,
        //LTFILENOTFOUNDLOG,


        //*2007.05.18 emori
        //* 標準では、rootを使用。
        //* 付属のlog4net.configに記述されているroot以外のloggerを使用する場合は
        //* 以下のコメントを外す。　
        //*　　注　log4net.config利用の際は、Properties\AssemblyInfo.csの最終行に
        //* 　　　　　[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]
        //*　　　　を必ず追加すること。しなければファイルが読み込まれません。
        //*    　　また、Logクラス内のコメントも合わせて外すこと。
        //FILELOG,
        //CONSOLELOG,
        //SMTPLOG,
        //MESSAGEBOXLOG,
    }

    /// <summary>
    /// ログレベル（数字が小さいほどレベルが高い）
    /// </summary>
    public enum LogLevel
    {
        /// <summary>1:Fatal(致命的障害)</summary>
        FATAL = 1,
        /// <summary>2:Error(障害)</summary>
        ERROR = 2,
        /// <summary>3:Warn(警告)</summary>
        WARN = 3,
        /// <summary>4:Info(情報)</summary>
        INFO = 4,
        /// <summary>5:Degug(デバッグ・トレース用)</summary>
        DEBUG = 5,
    }
    #endregion


    #region ログ制御クラス

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
    /// <summary>
    /// ログ制御クラス
    /// </summary>
    public abstract class LogControl
    {
		private static bool isBatchMode = false;

		public static void SetBatchMode()
		{
			isBatchMode = true;
			ConfigRollingFileAppender();
			ConfigSmtpAppender();
		}

        /// <summary>総合ログ</summary>
        private static readonly log4net.ILog logger_ROOT = log4net.LogManager.GetLogger(LogType.ROOT.ToString());

        ///// <summary>標準ログ</summary>
        //private static readonly log4net.ILog logger_DEFAULT = log4net.LogManager.GetLogger(LogType.DEFAULT.ToString());

        ///// <summary>選別ファイルが見つからなかった時用ログ</summary>
        //private static readonly log4net.ILog logger_LTFILENOTFOUNDLOG = log4net.LogManager.GetLogger(LogType.LTFILENOTFOUNDLOG.ToString());

        //*2007.05.18 emori
        //* 標準では、標準ログを使用。
        //* 付属のlog4net.configに記述されているroot以外のloggerを使用する場合は
        //* 以下のコメントを外す。　
        //*　　注　log4net.config利用の際は、Properties\AssemblyInfo.csの最終行に
        //* 　　　　　[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]
        //*　　　　を必ず追加すること。しなければファイルが読み込まれません。
        //*    　　また、LogType列挙型内のコメントも合わせて外し、
        //*        GetLoggerメソッドに追加
        ///// <summary>ファイル出力のみ行いたい場合</summary>
        //private static readonly log4net.ILog logger_FILE = log4net.LogManager.GetLogger(LogType.FILELOG.ToString());
        ///// <summary>コンソール出力のみ行いたい場合</summary>
        //private static readonly log4net.ILog logger_CONSOLE = log4net.LogManager.GetLogger(LogType.CONSOLELOG.ToString());
        ///// <summary>メール送信のみ行いたい場合</summary>
        //private static readonly log4net.ILog logger_SMTP = log4net.LogManager.GetLogger(LogType.SMTPLOG.ToString());
        ///// <summary>メッセージボックス表示のみ行いたい場合</summary>
        //private static readonly log4net.ILog logger_MESSAGE = log4net.LogManager.GetLogger(LogType.MESSAGEBOXLOG.ToString());


        #region property
        //* 2007.5.18 emori
        //* 仕様変更に伴うコメントアウト。
        //-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->-->
        //private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("log4net");
        ///// <summary>
        ///// log4net.ILog
        ///// </summary>
        //public static log4net.ILog Logger
        //{
        //    get { return logger; }
        //}
        //<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--<--

        /// <summary>
        /// 出力形式定義
        /// </summary>
        private static log4net.Layout.ILayout defaultLayout
        {
            get
            {
                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%d,%-5p,-,%m%n");
                layout.Header = "[START " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]" + Environment.NewLine;
                layout.Footer = Environment.NewLine + "\r\n\r\n";//Environment.NewLine + "[END " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]\r\n\r\n";
                return layout;
            }
        }
        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static LogControl()
        {
            //設定ファイルが読み込まれていなければ初期化する
            if (!GetFlgLog4NetConfigReaded())
            {
                //  Console
                ConfigConsoleAndTraceAppender();

                //  RollingFile
                ConfigRollingFileAppender();

                //  SMTP
                //ConfigSmtpAppender();
            }

            //fileNMChange();
        }



        /// <summary>
        /// 設定ファイルが読み込まれているかどうかを取得（TRUE:取得済　FALSE:未取得）
        /// </summary>
        /// <returns></returns>
        private static bool GetFlgLog4NetConfigReaded()
        {
            bool retV = false;

            Assembly entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != null)
            {
                foreach (ILoggerRepository repository in LogManager.GetAllRepositories())
                {
                    foreach (IAppender appender in repository.GetAppenders())
                    {
                        retV = true;
                    }
                }
            }

            return retV;
        }


        #region logger取得メソッド

        /// <summary>
        /// 標準loggerを取得
        /// </summary>
        /// <returns></returns>
        public static log4net.ILog GetLogger()
        {
            return GetLogger(LogType.ROOT);
        }

        /// <summary>
        /// 指定したログタイプのloggerを取得
        /// </summary>
        /// <param name="aLogType"></param>
        /// <returns></returns>
        public static log4net.ILog GetLogger(LogType aLogType)
        {
            log4net.ILog retV = null;
            switch (aLogType)
            {
                case LogType.ROOT:
                    retV = logger_ROOT;
                    break;
                //case LogType.LTFILENOTFOUNDLOG:
                //    retV = logger_LTFILENOTFOUNDLOG;
                //    break;
                //case LogType.DEFAULT:
                //    retV = logger_DEFAULT;
                //    break;
                default:
                    throw new SLException("想定されていないロガータイプが指定されました。\r\nlog4netHelperクラスのGetLoggerメソッドまたは変数logger_****を確認してください",
                        LogLevel.DEBUG,LogType.ROOT);
            }

            return retV;
        }
        #endregion


        #region Appender初期化
        /// <summary>
        /// SmtpAppenderの初期化
        /// </summary>
        private static void ConfigSmtpAppender()
        {
			log4net.Appender.SmtpAppender smtp = new log4net.Appender.SmtpAppender();
			smtp.Name = "SMTPAppender";
			if(isBatchMode)
			{	
				smtp.Subject = "EICS3 BatchMode Error Mail";
				smtp.From = "LOG4NET[automail@sl13.nichia.co.jp]";
				smtp.To = "naohito.yoshimoto@nichia.co.jp";
				//smtp.To = "haruhisa.ishiguchi@nichia.co.jp,naohito.yoshimoto@nichia.co.jp,motoaki.yuasa@nichia.co.jp,hiroki.shinomiya@nichia.co.jp";
			}
			else
			{
				smtp.Subject = "EICS ERROR LOG";
				smtp.To = "haruhisa.ishiguchi@nichia.co.jp";
				smtp.From = "haruhisa.ishiguchi@nichia.co.jp,naohito.yoshimoto@nichia.co.jp,motoaki.yuasa@nichia.co.jp,hiroki.shinomiya@nichia.co.jp,kei.nagao@nichia.co.jp";
			}

			smtp.SmtpHost = "HQSMTP1.nichia.local";
			smtp.Layout = defaultLayout;
			smtp.Priority = System.Net.Mail.MailPriority.High;

			//フィルタ設定
			smtp.AddFilter(GetRangeFilter(log4net.Core.Level.Error, log4net.Core.Level.Fatal));

            smtp.ActivateOptions(); //必須
            log4net.Config.BasicConfigurator.Configure(smtp);
        }

        /// <summary>
        /// RollingFileAppenderの初期化
        /// </summary>
        private static void ConfigRollingFileAppender()
        {
            log4net.Appender.RollingFileAppender rolling = new log4net.Appender.RollingFileAppender();
            rolling.Name = "RollingFileAppender";

			if (isBatchMode)
			{
				rolling.File = "C:\\qcil\\BatchModeLog\\MyApp." + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			}
            //rolling.File = "Log\\MyApp." + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			//rolling.File = "Log\\MyApp.txt"; //2013.10.29 log4net.configの記載と被っている為、コメントアウト n.yoshimoto
            rolling.StaticLogFileName = true;
            rolling.AppendToFile = true;
            rolling.MaxSizeRollBackups = 50;
            rolling.MaximumFileSize = "1000KB";
            rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            rolling.Layout = defaultLayout;

            //フィルタ設定
            rolling.AddFilter(GetRangeFilter(log4net.Core.Level.Debug, log4net.Core.Level.Fatal));

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


        /// <summary>
        /// MessageboxAppenderの初期化
        /// </summary>
        public static void ConfigMessageboxAppender()
        {
            ConfigMessageboxAppender("");
        }

        /// <summary>
        /// MessageboxAppenderの設定
        /// </summary>
        public static void ConfigMessageboxAppender(string title)
        {
            //MessageBoxAppenderの取得
            IAppender tmpAppender = GetIAppender(typeof(MessageBoxAppender).ToString());

            //タイトルが空白の場合は、アプリケーション名を表示
            Assembly entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != null)
            {
                title = (title == "") ? entryAsm.GetName().Name : title;
            }
            MessageBoxAppender mbox = (MessageBoxAppender)tmpAppender; //new MessageBoxAppender();
            mbox.MessageTitle = title;
            //フィルタ設定
            mbox.AddFilter(new log4net.Filter.DenyAllFilter());    //このアペンダを使用しない
            //mbox.AddFilter(GetRangeFilter(log4net.Core.Level.Debug, log4net.Core.Level.Fatal));
            log4net.Config.BasicConfigurator.Configure(mbox);
        }


        /// <summary>
        /// 指定したアペンダのインスタンスを取得（インスタンスが存在しない場合は新規作成して取得）
        /// </summary>
        /// <param name="appenderTypeName">アペンダのタイプ名</param>
        /// <returns>アペンダのインスタンス</returns>
        private static IAppender GetIAppender(string appenderTypeName)
        {
            IAppender retV = null;
            try
            {
                Assembly entryAsm = Assembly.GetEntryAssembly();
                if (entryAsm != null)
                {
                    foreach (ILoggerRepository repository in LogManager.GetAllRepositories())
                    {
                        foreach (IAppender appender in repository.GetAppenders())
                        {
                            try
                            {
                                if (appender.GetType().ToString() == appenderTypeName)
                                {
                                    retV = appender;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            if (retV == null)
            {
                Type t = Type.GetType(appenderTypeName);

                object tmp = System.Activator.CreateInstance(t, null);
                retV = (IAppender)tmp;
            }

            return retV;
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


        #region ファイルログ名変更メソッド
        //HACK log4netの設定ファイルの値が変わると挙動不審になる可能性大。マジックナンバーもつこうてます。 
        /// <summary>
        /// ログファイルの名称をアプリケーション名に変更
        /// ものすごく適当。とりあえず放置。
        /// </summary>
        private static void fileNMChange(string changeVAL)
        {
            try
            {
                Assembly entryAsm = Assembly.GetEntryAssembly();
                if (entryAsm != null)
                {
                    foreach (ILoggerRepository repository in LogManager.GetAllRepositories())
                    {
                        foreach (IAppender appender in repository.GetAppenders())
                        {
                            try
                            {
                                if (appender.Name.Equals("RollingFileAppender"))
                                {
                                    FileAppender fileAppender = appender as FileAppender;
                                    if (fileAppender != null)
                                    {
                                        string file = fileAppender.File;
                                        if (!string.IsNullOrEmpty(file))
                                        {
                                            if (file.Contains("MyApp"))
                                            {
                                                //fileAppender.File = fileAppender.File.Replace("MyApp", entryAsm.GetName().Name);
                                                fileAppender.File = fileAppender.File.Replace("MyApp", changeVAL);
                                                fileAppender.ActivateOptions();
                                                //変更前ファイルの削除
                                                System.IO.File.Delete(file);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region ログ出力

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        public static void OutputLog(LogLevel level, string message)
		{
#if Debug
			string msgFrom = string.Empty;
			for (int depth = 1; depth <= 2; depth++)
			{
				StackFrame callerFrame = new StackFrame(depth);
				//メソッド名
				string methodName = callerFrame.GetMethod().Name;
				//クラス名
				string className = callerFrame.GetMethod().ReflectedType.FullName;

				msgFrom += string.Format("クラス名{0}:関数名{1} ⇒ ", className, methodName);
			}
			message = msgFrom + "\r\n" + message;
#endif

			OutputLog(LogType.ROOT, level, message, "");
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        public static void OutputLog(LogLevel level, string message, string title)
        {
            //fileNMChange(title);

            OutputLog(LogType.ROOT, level, message, "");
        }

		public static void OutputLogMail(LogLevel level, string message, string title)
		{
			OutputLog(LogType.ROOT, level, message, title);
		}

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="aLogType">ログ出力タイプ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        public static void OutputLog(LogType aLogType, LogLevel level, string message)
        {
            OutputLog(aLogType, level, message, "");
        }


        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="aLogType">ログ出力タイプ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        public static void OutputLog(LogType aLogType, LogLevel level, string message, string title)
        {
            string outputMsg = "";

            log4net.ILog logger = GetLogger(aLogType);
            ConfigMessageboxAppender(title);
            
            outputMsg = message;
            
            switch (level)
            {
                case LogLevel.DEBUG:
                    logger.Debug(outputMsg);
                    break;
                case LogLevel.INFO:
                    logger.Info(outputMsg);
                    break;
                case LogLevel.WARN:
                    logger.Warn(outputMsg);
                    break;
                case LogLevel.ERROR:
                    logger.Error(outputMsg);
                    break;
                case LogLevel.FATAL:
                    logger.Fatal(outputMsg);
                    break;
            }
        }

        /// <summary>
        /// Exceptionログの出力
        /// </summary>
        /// <param name="aLogType">ログ出力タイプ</param>
        public static void OutputExceptionLog(LogType aLogType, SLException ex)
        {
            string outputMsg = "";

            log4net.ILog logger = GetLogger(aLogType);

            outputMsg = ex.ExceptionMessage;
            ConfigMessageboxAppender(ex.Title);

            switch (ex.ExceptionLogLevel)
            {
                case LogLevel.DEBUG:
                    logger.Debug(outputMsg, ex.ExceptionInnerException);
                    break;
                case LogLevel.INFO:
                    logger.Info(outputMsg, ex.ExceptionInnerException);
                    break;
                case LogLevel.WARN:
                    logger.Warn(outputMsg, ex.ExceptionInnerException);
                    break;
                case LogLevel.ERROR:
                    logger.Error(outputMsg, ex.ExceptionInnerException);
                    break;
                case LogLevel.FATAL:
                    logger.Fatal(outputMsg, ex.ExceptionInnerException);
                    break;
            }
        }

        /// <summary>
        /// Exceptionログの出力
        /// </summary>
        public static void OutputExceptionLog(SLException ex)
        {
            OutputExceptionLog(ex.ExceptionLogType,ex);
        }
        #endregion



    }

    #endregion

    #region メッセージボックス表示アペンダクラス

    /// <summary>
    /// メッセージボックス表示用アペンダ
    /// </summary>
    public class MessageBoxAppender : log4net.Appender.AppenderSkeleton
    {
        /// <summary>メッセージボックスに表示するタイトル</summary>
        private string _messageTitle = "";

        /// <summary>メッセージボックスに表示するタイトル</summary>
        public string MessageTitle
        {
            get { return this._messageTitle; }
            set { this._messageTitle = value; }
        }

        /// <summary>ログイベント発生時の処理</summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            string message = loggingEvent.MessageObject.ToString();

            Icon icon = null;

            //ログレベルに応じて、メッセージボックスのアイコンを変更する
            switch ((LogLevel)Enum.Parse(typeof(LogLevel), loggingEvent.Level.ToString().ToUpper()))
            {

                case LogLevel.DEBUG:
                    icon = null;
                    break;
                case LogLevel.INFO:
                    icon = SystemIcons.Information;
                    break;
                case LogLevel.WARN:
                    icon = SystemIcons.Warning;
                    break;
                case LogLevel.ERROR:
                    icon = SystemIcons.Error;
                    break;
                case LogLevel.FATAL:
                    icon = SystemIcons.Hand;
                    break;
                default:
                    icon = null;
                    break;
            }

            //Exception経由でのメッセージの表示の場合、エラー発生箇所の表示をメッセージの後ろに追加する。
            if (loggingEvent.ExceptionObject != null)
            {
                message += "\r\n"
                    + loggingEvent.ExceptionObject.StackTrace;
            }

            if (message != "")
            {
                //メッセージボックスの表示
                ErrDialog.Show(message, icon, this._messageTitle);
            }
        }
    }

    #endregion

    #region オリジナルメッセージボックス

    //TODO 必要なら、ＯＫボタンの対応化や、テキストボックスの内容に応じてダイアログの大きさを可変にするよう変更すべき
    /// <summary>
    /// オリジナルメッセージボックス（※現時点でボタンは、ＯＫのみ）
    /// </summary>
    public class ErrDialog : Form
    {
        //フォームのコントロール変数
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pctIcon;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnClipBoard;

        //システムアイコン
        private System.Drawing.Icon _sytemIcon = null;
        //メッセージ
        private string _message = "";
        //タイトル
        private string _title = "";


        #region コンストラクタ関連
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ErrDialog()
        {
            InitializeComponent();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        private ErrDialog(string message) : this(message, null, "") { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        private ErrDialog(string message, string title) : this(message, null, title) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="icon">システムアイコン</param>
        private ErrDialog(string message, System.Drawing.Icon icon) : this(message, icon, "") { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="icon">システムアイコン</param>
        /// <param name="title">タイトル</param>
        private ErrDialog(string message, System.Drawing.Icon icon, string title)
            : this()
        {
            this._sytemIcon = icon;
            this._message = message;
            this._title = title;
        }

        /// <summary>
        /// ダイアログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <returns>DialogResult</returns>
        public static DialogResult Show(string message)
        {
            ErrDialog tmp = new ErrDialog(message);
            return tmp.ShowDialog();
        }
        /// <summary>
        /// ダイアログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>DialogResult</returns>
        public static DialogResult Show(string message, string title)
        {
            ErrDialog tmp = new ErrDialog(message, title);
            return tmp.ShowDialog();
        }
        /// <summary>
        /// ダイアログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="icon">システムアイコン</param>
        /// <returns>DialogResult</returns>
        public static DialogResult Show(string message, System.Drawing.Icon icon)
        {
            ErrDialog tmp = new ErrDialog(message, icon);
            return tmp.ShowDialog();
        }
        /// <summary>
        /// ダイアログの表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="icon">システムアイコン</param>
        /// <param name="title">タイトル</param>
        /// <returns>DialogResult</returns>
        public static DialogResult Show(string message, System.Drawing.Icon icon, string title)
        {
            ErrDialog tmp = new ErrDialog(message, icon, title);
            return tmp.ShowDialog();
        }


        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region 初期設定

        private void InitializeComponent()
        {
            this.pctIcon = new System.Windows.Forms.PictureBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnClipBoard = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pctIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pctIcon
            // 
            this.pctIcon.Location = new System.Drawing.Point(12, 12);
            this.pctIcon.Name = "pctIcon";
            this.pctIcon.Size = new System.Drawing.Size(32, 32);
            this.pctIcon.TabIndex = 0;
            this.pctIcon.TabStop = false;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Location = new System.Drawing.Point(55, 12);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(565, 154);
            this.txtMessage.TabIndex = 3;
            this.txtMessage.Text = "";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(510, 172);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(110, 31);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnClipBoard
            // 
            this.btnClipBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClipBoard.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClipBoard.Location = new System.Drawing.Point(394, 172);
            this.btnClipBoard.Name = "btnClipBoard";
            this.btnClipBoard.Size = new System.Drawing.Size(110, 31);
            this.btnClipBoard.TabIndex = 2;
            this.btnClipBoard.Text = "メッセージのコピー";
            this.btnClipBoard.UseVisualStyleBackColor = true;
            this.btnClipBoard.Click += new System.EventHandler(this.btnClipBoard_Click);
            // 
            // ErrDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 213);
            this.Controls.Add(this.btnClipBoard);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.pctIcon);
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.Text = "System Message";
            this.Load += new System.EventHandler(this.ErrDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pctIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.StartPosition = FormStartPosition.CenterParent;

        }

        #endregion

        #region イベント

        /// <summary>
        /// フォームロード時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ErrDialog_Load(object sender, EventArgs e)
        {
            if (this._sytemIcon == null)
            {
                //システムアイコンが設定されていない場合、テキストボックスを左に寄せ、幅を大きくする
                this.txtMessage.Left = 12;
                this.txtMessage.Width += 43;
            }
            else
            {
                //システムアイコンを表示
                this.pctIcon.Image = this._sytemIcon.ToBitmap();
            }

            //メッセージを表示
            this.txtMessage.Text = this._message;
            //タイトルを表示
            this.Text = this._title;
        }

        /// <summary>
        /// クリップボードへのコピーボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClipBoard_Click(object sender, EventArgs e)
        {
            //テキストボックスに表示されている項目をクリップボードにコピー
            Clipboard.SetDataObject(txtMessage.Text, true);
        }

        #endregion
    }
    #endregion


    #region SL汎用Exception

    /// <summary>
    /// 汎用Exception
    /// </summary>
    public class SLException : Exception
    {
        private string _message = "";
        private string _title = "";
        private LogType _logType;
        private LogLevel _logLevel;
        private Exception _innerException = null;

        #region Property

        /// <summary>ログ</summary>
        public log4net.ILog _logger = null;

        /// <summary>
        /// ログ用メッセージ
        /// </summary>
        public string ExceptionMessage
        {
            get { return this._message; }
        }

        /// <summary>
        /// ログタイトル
        /// </summary>
        public string Title
        {
            get { return this._title; }
        }

        /// <summary>
        /// 内包するSystemException
        /// </summary>
        public Exception ExceptionInnerException
        {
            get { return this._innerException; }
        }

        /// <summary>
        /// ログレベル
        /// </summary>
        public LogLevel ExceptionLogLevel
        {
            get { return this._logLevel; }
        }

        /// <summary>
        /// ログタイプ
        /// </summary>
        public LogType ExceptionLogType
        {
            get { return this._logType; }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SLException() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public SLException(string message) : this(message, null) { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        public SLException(string message, LogLevel level) : this(message, null, level) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        public SLException(string message, LogLevel level, string title) : this(message, null, level, title) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, bool doOutputLog) : this(message, null, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, LogLevel level, bool doOutputLog) : this(message, null, level, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, LogLevel level, string title, bool doOutputLog) : this(message, new Exception(), level, title, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        public SLException(Exception ex) : this(ex.Message, ex) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        public SLException(Exception ex, LogLevel level) : this(ex.Message, ex, level) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        public SLException(string message, Exception ex)
            : this(message, ex, LogLevel.ERROR) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        public SLException(string message, Exception ex, LogLevel level)
            : this(message, ex, level, "") { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(Exception ex, bool doOutputLog) : this(ex.Message, ex, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(Exception ex, LogLevel level, bool doOutputLog) : this(ex.Message, ex, level, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, bool doOutputLog)
            : this(message, ex, LogLevel.ERROR, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, LogLevel level, bool doOutputLog)
            : this(message, ex, level, "", doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        public SLException(string message, Exception ex, LogLevel level, string title)
            : this(message, ex, level, title, false) { }
                /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, LogLevel level, string title, bool doOutputLog)
            : this(message, ex, level,LogType.ROOT, title, false) { }

        #region LogType対応
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(string message,LogType logType) : this(message, null,logType) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(string message, LogLevel level, LogType logType) : this(message, null, level, logType) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        public SLException(string message, LogLevel level,LogType logType, string title) : this(message, null, level, logType, title) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力</param>
        public SLException(string message, LogType logType, bool doOutputLog) : this(message, null,logType, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力</param>
        public SLException(string message, LogLevel level,LogType logType, bool doOutputLog) : this(message, null, level, logType, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>        
        /// <param name="doOutputLog">エラーログの即時出力</param>
        public SLException(string message, LogLevel level,LogType logType, string title, bool doOutputLog) : this(message, new Exception(), level, logType, title, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(Exception ex, LogType logType) : this(ex.Message, ex,logType) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(Exception ex, LogLevel level,LogType logType) : this(ex.Message, ex, level,logType) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(string message, Exception ex,LogType logType)
            : this(message, ex, LogLevel.ERROR,logType) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        public SLException(string message, Exception ex, LogLevel level,LogType logType)
            : this(message, ex, level, logType, "") { }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>        
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(Exception ex, LogType logType, bool doOutputLog) : this(ex.Message, ex,logType, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(Exception ex, LogLevel level,LogType logType, bool doOutputLog) : this(ex.Message, ex, level, logType, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>        
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, LogType logType, bool doOutputLog)
            : this(message, ex, LogLevel.ERROR,logType, doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>        
        /// <param name="logType">ログタイプ</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, LogLevel level,LogType logType, bool doOutputLog)
            : this(message, ex, level, logType, "", doOutputLog) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        public SLException(string message, Exception ex, LogLevel level,LogType logType, string title)
            : this(message, ex, level, logType, title, false) { }


        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="ex">元Exception</param>
        /// <param name="level">ログレベル</param>
        /// <param name="logType">ログタイプ</param>
        /// <param name="title">メッセージボックス表示用タイトル</param>
        /// <param name="doOutputLog">エラーログの即時出力(LogTypeはROOT)</param>
        public SLException(string message, Exception ex, LogLevel level,LogType logType, string title, bool doOutputLog)
            : base(message, ex)
        {
            this._message = message;
            this._title = title;
            this._logLevel = level;
            this._logType = logType;
            this._innerException = ex;

            if (doOutputLog) this.OutputLog();
        }

        #endregion

        #region エラーログの出力

        /// <summary>
        /// エラーログの出力
        /// </summary>
        /// <param name="aLogType">ログ出力タイプ</param>
        public void OutputLog(LogType aLogType)
        {
            LogControl.OutputExceptionLog(aLogType, this);
        }

        /// <summary>
        /// エラーログの出力
        /// </summary>
        /// <param name="aLogType">ログ出力タイプ</param>
        public void OutputLog()
        {
            this.OutputLog(this._logType);
        }

        #endregion

    }

    #endregion
}
