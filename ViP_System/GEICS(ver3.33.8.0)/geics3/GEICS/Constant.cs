using System;
using System.Collections.Generic;

namespace GEICS
{
    /// <summary>
    /// Constant の概要の説明です
    /// </summary>
    public class Constant
    {
        public static IMessage MessageInfo;
        public static List<SettingInfo> settingInfoList;
        public static EmployeeInfo EmployeeInfo;
        public static bool fPackage = false;
        //public static bool fMap = false;
        public static TypeGroup typeGroup;
        
        public static bool fOutline = false;
        public static bool fSemi = false;
        public static bool fClient = false;
		public static int WBEquipVerParamNO;
        public static string StrQCIL;
        public static string StrARMS;
        public static string StrNASCA;
		public const string LENS_DB_NM = "LENS";

        public enum TypeGroup //整合チェック時に大・小文字の区別を無くす為、全て大文字で定義する事（チェックする際は、ToUpper()を使用する事）
        {
            MAP,
            SV,
			SIDEVIEW,
			AUTOMOTIVE,
            LAMP,
			SMD3in1,
			X19,
			MPL,
            DMC,
			X83_385_COB,
			X83,
			_385,
			COB,
			NTSV,
			SIGMA,
			VOYAGER_CSP_093_KIRAMEKI,
			VOYAGER,
			CSP,
			_093,
			KIRAMEKI,
            NLCV
        }

        public enum Function
        {
            /// <summary>
            /// 閾値変更
            /// </summary>
            F001,
            /// <summary>
            /// 定型文変更
            /// </summary>
            F002,
            /// <summary>
            /// 権限変更
            /// </summary>
            F003,
            /// <summary>
            /// 中間サーバ反映
            /// </summary>
            F004,
			/// <summary>
			/// 閾値(内規)変更
			/// </summary>
			F005,
            /// <summary>
            /// 閾値(PS)変更
            /// </summary>
            F001_PS,
        }

		/// <summary>
		/// ライン種類
		/// </summary>
		public enum LineType
		{
			// アウトライン
			OUT,
			// 自動搬送
			AUTO,
			// 高生産性
			HIGH,
			// ハイブリッド
			HYBRID
		}
        
        //データベース接続文字列-------------------------------------

        public const string StrQCIL_RADS1 = "Server=SLA-RADS1;Connect Timeout=0;Database=QCIL;User ID=sla02;password=2Gquf5d;Application Name=GEICS3";
        public const string StrQCIL_SV_AUTO = "Server=HQDB101.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;User ID=sla05;password=ZP3wnIC;Application Name=GEICS3";
        public const string StrQCIL_NSERVER = "Server=NSERVER01.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_MAP_AUTO = "Server=HQDB101.nichia.local\\INST1;Connect Timeout=0;Database=QCIL_MAP;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_MAPOUT = "Server=HQDB101.nichia.local\\INST1;Connect Timeout=0;Database=QCIL_MAP_OUTLINE;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_HQDB5_AOI = "Server=AOIDB1.aoi.local\\INST1;Connect Timeout=0;Database=QCIL_AOI;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_HQDB5_AOI_SEMI = "Server=AOIDB1.aoi.local\\INST1;Connect Timeout=0;Database=QCIL_HIGH;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_AOI_MAP_SEMI = "Server=AOIDB1.aoi.local\\INST1;Connect Timeout=0;Database=QCIL02;User ID=inline;password=R28uHta;Application Name=GEICS3";
		public const string StrQCIL_AOI_MAP_AUTO = "Server=AOIDB1.aoi.local\\INST1;Connect Timeout=0;Database=QCIL02_AUTO;User ID=inline;password=R28uHta;Application Name=GEICS3";
        public const string StrQCIL_HQDB5_KYO = "Server=TECDB1.takatsuki.local\\INST1;Connect Timeout=0;Database=QCIL_KYOTO;User ID=inline;password=R28uHta;Application Name=GEICS3";
		public const string StrQCIL_KYO_3in1 = "Server=TECDB1.takatsuki.local\\INST1;Connect Timeout=0;Database=QCIL_KYOTO;User ID=inline;password=R28uHta;Application Name=GEICS3_";
        public const string StrQCIL_HQDB5_MIK = "Server=TECDB1.takatsuki.local\\INST1;Connect Timeout=0;Database=QCIL01_HIGH_MIK;User ID=inline;password=R28uHta;Application Name=GEICS3";
		//public const string StrQCIL_CE = "Server=CEJDB1.citizen.local\INST1;Connect Timeout=0;Database=QCIL;User ID=inline;password=R28uHta;Application Name=インライン傾向管理システム";
        public const string StrQCIL_CE = @"Server=CEJDB1.citizen.local\INST1;Connect Timeout=0;Database=QCIL;User ID=inline;password=R28uHta;Application Name=GEICS3";

        public const string StrQCIL_NMC = "Server=NMCDB1.nmc.local\\INST1;Connect Timeout=0;Database=QCIL;User ID=inline;password=R28uHta;Application Name=GEICS3";

        //NASCAサーバー
        public const string StrNASCA_AOI = "Server=HQDBREF4.nichia.local\\INST4;Connect Timeout=0;Database=SC02_NADB01;UID=sla02;PWD=2Gquf5d;Application Name=GEICS3";
        public const string StrNASCA_KYO = "Server=HQDBREF4.nichia.local\\INST4;Connect Timeout=0;Database=SC03_NADB01;UID=sla02;PWD=2Gquf5d;Application Name=GEICS3";
        public const string StrNASCA_MIK = "Server=HQDBREF4.nichia.local\\INST4;Connect Timeout=0;Database=SC06_NADB01;UID=sla02;PWD=2Gquf5d;Application Name=GEICS3";

//#if TEST
//        public const string SQLite_ARMS = @"Server=sla-0040-2\SQLEXPRESS;Connect Timeout=0;Database=ARMS_MAP;UID=inline;PWD=R28uHta;Application Name=インライン傾向管理システム";
//        public const string StrPACKAGE = @"Server=sla-0040-2\SQLEXPRESS;Connect Timeout=0;Database=QCIL_MAP;UID=inline;PWD=R28uHta;Application Name=インライン傾向管理システム";
//#else
        public const string SQLite_ARMS = @"Server={0}\SQLEXPRESS;Connect Timeout=0;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=GEICS3";
        public const string StrPACKAGE = @"Server={0}\SQLEXPRESS;Connect Timeout=0;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=GEICS3";

        
//#endif
        //-------------------------------------------------------------

        //管理方法
        public const string sOKNG = "OK/NG";
        public const string sOther = "OTHER";
        public const string sMax = "MAX";
        public const string sMaxMin = "MAX-MIN";
        public const string sXbar = "X-bar";

        #region グラフ描画関連
        /// <summary>チャートエリア名</summary>
        public const string CHART_CHARTAREANM = "GRAPH";
        /// <summary>Series:X軸項目名</summary>
        public const string CHART_X = "X";
        /// <summary>Series:全装置用</summary>
        public const string CHART_EQUINO_ALL = "ALL";
        /// <summary>Series:旗付け用</summary>
        public const string CHART_FLAG_LOT = "FlagLot";
        /// <summary>
        /// ライン種別
        /// </summary>
        public enum ENUM_CHART_LineKind
        {
            CL = 1,
            UCL = 2,
            LCL = 3,
            QUCL = 4,
            QLCL = 5,
        }

        public const string CHART_CL_NM = "平均値";

        public const string CHART_KIKAKU_NM = "規格";
        public const string CHART_UCL_NM = "上限値";
        public const string CHART_LCL_NM = "下限値";
        public const string CHART_UCL_NM_KIKAKU = "規格上限値";
        public const string CHART_LCL_NM_KIKAKU = "規格下限値";
		public const string CHART_INNER_UPPER_LIMIT_NM = "工程狙い上限値";
		public const string CHART_INNER_LOWER_LIMIT_NM = "工程狙い下限値";

        /// <summary>
        /// 空のデータポイントの表示位置
        /// </summary>
        public enum ENUM_CHART_EmptyPointStyleCustom
        {
            Zero,
            Average,
        }
        #endregion

		public enum ASEETS_CD
		{
			DB = 1,
			WB = 2,
			AI = 3,
			MD = 4,
			CF = 5
		}

        public const string GENERALGRP_CD_WBTRAN = "1";


        public const string VOID_STRING = "";

    }
}