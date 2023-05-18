using System;
using System.Collections.Generic;
using System.Text;

namespace FVDC
{
	/// <summary>
	/// 共通に使用する固定値
	/// </summary>
	public class Constant
	{
		/// <summary>
		/// 接続するＤＢ名
        /// </summary>
        public const string StrCheckDB      = "Server={0};Connect Timeout=10;Database=ARMS;User ID=inline;password=R28uHta";
        public const string StrARMSDB       = "Server={0};Connect Timeout=0;Database=ARMS;User ID=inline;password=R28uHta";
        public const string StrLENSDB       = "Server={0};Connect Timeout=0;Database=LENS;User ID=inline;password=R28uHta";
        public const string StrQCILDB       = "Server={0};Connect Timeout=0;Database=QCIL;User ID=inline;password=R28uHta";
        public const string StrSQLDB        = "Server={0};Connect Timeout=0;Database={1};User ID=inline;password=R28uHta";
        public const string StrLoginDB      = "Server={0};Connect Timeout=0;Database={1};User ID={2};password={3}";


	}
}
