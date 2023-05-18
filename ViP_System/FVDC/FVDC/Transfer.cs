using System;

namespace FVDC
{
	/// <summary>
	/// 画面間の共通受け渡し領域
	/// </summary>
	public class Transfer
	{
		/// <summary>
		/// ログインした社員の基本情報
		/// </summary>
		public static string	EmpCd;			/// 社員コード
		public static string	EmpName;		/// 社員名
		public static string	PostName;		/// 役職名
		public static string	CompanyName;	/// 所属会社名
		public static string	CompanyNo;		/// 所属会社番号
		public static string	RegularKBN;		/// 正社員区分
		public static string	Sjname;			/// 所属部門名
		public static string	Scode;			/// 所属部門CD
		public static string	SectionName;	/// 所属部署名（複数所属有り）
		public static string	DivisionGP;		/// 所属部門グループ
		public static string	PlaceKB;		/// 場所区分		
		public static string	PlaceNM;		/// 場所名		
		public static string	Language;		/// 言語
		public static string	LogInMode;		/// 画面表示用
		public static int		LevelCd;		/// ログインレベル（役職レベルと同じ）
		public static bool		ReferenceMode;  /// 参照モード
        public static string    ServerName;		/// サーバー名
        public static string    LogInName;		/// ログイン情報
		public static string	LogInServer;	/// ログインサーバー
		public static string	UserName;		/// ユーザー名
		public static string	UserDomainName;	/// ドメイン名
		public static string	MachineName;	/// マシン名
		public static string	Version;		/// バージョン
												/// 
		/// <summary>
		/// 下位画面への引継ぎ情報
		/// </summary>
		public static int		RequestKB;		/// 依頼区分
		public static int		RequestNO;	    /// 依頼受付番号
		public static string	DeviceID;		/// 機器管理番号
		public static string	WorkerID;       /// 作業者ID
		public static string	WorkerNM;       /// 作業者ID
		public static string	WkhID;		    /// 勤務シフトID
		public static DateTime	WorkDate;		/// 出勤日
		public static string	WhereSql;		/// 検索条件
		public static DateTime	ThisDate;		/// 対象日
		public static string	SelectWork;		/// 選択作業
        public static bool      FomeClose;      /// 画面表示モード
        public static string    MacNo;		    /// マシン№
        public static string    ServerNM;	    /// サーバー情報
        public static string    LogInID;		/// ログイン情報
        public static string    ConnectionNM;	/// 接続情報
        public static string    MacNoList;		/// マシン№リスト
        public static string    TypeNM;         /// タイプ名

        /// <summary>
        /// アプリケーション起動パス
        /// </summary>
        public static string		StartupPath;

		/// <summary>
		/// デスクトップパス
		/// </summary>
		public static string		DeskTopPath;
		
		/// <summary>
		/// ファイルアップロードパス
		/// </summary>
		public static string		FileUpLoadPath;

        /// <summary>
        /// 置換文字列
        /// </summary>
        public static string        SearchChar;
        public static string        ReplaceChar;

        /// <summary>
        /// タイプ
        /// </summary>
        public static DsName        dsType;

        /// <summary>
        /// 工程
        /// </summary>
        public static DsName        dsProcess;

		/// <summary>
        /// 作業CD
		/// </summary>
        public static DsName        dsWorkcd;

        /// <summary>
        /// 設備番号
        /// </summary>
        public static DsFree        dsDeviceID;

    }
}
