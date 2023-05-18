using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS
{
	public class RunningLog
	{
		public const int LOG_MAX_LINE = 50;

		private static RunningLog logInstance;
		public Queue<string> logMessageQue;
		static public System.Object lockThis = new System.Object();

		private RunningLog()
		{
			logMessageQue = new Queue<string>();
		}

		public static RunningLog GetInstance()
		{
			if (logInstance == null)
			{
				logInstance = new RunningLog();
			}

			return logInstance;
		}
	}

	public class AlertLog
	{
		public const int LOG_MAX_LINE = 50;

		private static AlertLog logInstance;
		public Queue<string> logMessageQue;
		static public System.Object lockThis = new System.Object();

		private AlertLog()
		{
			logMessageQue = new Queue<string>();
		}

		public static AlertLog GetInstance()
		{
			if (logInstance == null)
			{
				logInstance = new AlertLog();
			}

			return logInstance;
		}
	}
}
