using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace LENS2
{
	public class Log
	{
        public const int CellStrLenPerRow = 80;

		static Log() 
		{
			ConfigInfoLogAppender();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Info(string classNm, string machineNm, string message, bool isScreenLog)
		{
			if (isScreenLog)
			{
				F01_Watch.WriteLog(Properties.Resources.Infomation, classNm, machineNm, message);
			}

			LogManager.GetLogger("InfoLogger")
				.Info(string.Format("[装置種類]{0} [号機]{1} : {2}", classNm, machineNm, message));
            
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Error(string classNm, string machineNm, string message, bool isScreenLog)
		{
			if (isScreenLog)
			{
				F01_Watch.errorSound.PlayLooping();
				F01_Watch.WriteLog(Properties.Resources.Error, classNm, machineNm, message);
			}

			LogManager.GetLogger("InfoLogger")
				.Error(string.Format("[装置種類]{0} [号機]{1} : {2}", classNm, machineNm, message));

			//メール送信
		}

		public static void Exclamation(string classNm, string machineNm, string message, bool isScreenLog) 
		{
			if (isScreenLog)
			{
				F01_Watch.WriteLog(Properties.Resources.Exclamation, classNm, machineNm, message);
			}

			LogManager.GetLogger("InfoLogger")
				.Fatal(string.Format("[装置種類]{0} [号機]{1} : {2}", classNm, machineNm, message));
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
			rolling.File = "LOG\\LOG";
            rolling.DatePattern = "yyyyMMdd'.log'";
			rolling.StaticLogFileName = false;
			rolling.AppendToFile = true;
			rolling.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
			rolling.Layout = defaultLayout;

			//フィルタ設定
			log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter();
			filter.LoggerToMatch = "InfoLogger";
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
