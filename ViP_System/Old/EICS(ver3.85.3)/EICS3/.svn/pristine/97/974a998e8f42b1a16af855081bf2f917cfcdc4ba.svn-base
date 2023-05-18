using System.Linq;
using EICS.Database;
using System.Collections.Generic;
using System.Text;

namespace EICS
{
	public partial class SettingInfo
	{
		const string LINEINFO_NODE = "//qcil_info/line_info";
		const string PLC_EXTRACT_EXCLUSION_NODE = "//qcil_info/PlcExtractExclusionList/PlcExtractExclusion";
		const string DEBUG_SETTING_FILE_PATH = @"C:\QCIL\SettingFiles\DebugQCIL.xml";

		#region qcil_info

		public List<int> PLA_RfParamNOList { get; set; }
		public List<int> PLA_TimeParamNOList { get; set; }
		public List<int> PLA_ArParamNOList { get; set; }
		public List<int> PLA_CfParamNOList { get; set; }
		public List<int> PLA_VacuumParamNOList { get; set; }
		public List<int> PLA_PressParamNOList { get; set; }


        #region スパッタ管理条件

        /// <summary>自動出力ファイルのRF出力判定項目及び判定下限温度</summary>
        public int SUP_AutoFileRf1ParamNo { get; set; }
        public int SUP_AutoFileRf2ParamNo { get; set; }
        public int SUP_AutoFileRfExclusionTemp { get; set; }

        /// <summary>自動出力ファイルの反射波判定項目及び連続判定レコード数（秒）</summary>
        public int SUP_AutoFileReflect1ParamNo { get; set; }
        public int SUP_AutoFileReflect2ParamNo { get; set; }
        public int SUP_AutoFileReflectCheckCount { get; set; }

        /// <summary>自動出力ファイルのガス圧判定項目及び連続判定レコード数（秒）</summary>
        public int SUP_AutoFileGusTentionParamNo { get; set; }
        public int SUP_AutoFileGusTentionCheckCount { get; set; }

        /// <summary>自動出力ファイルのガス流量判定項目</summary>
        public int SUP_AutoFileGusFlowParamNo { get; set; }
        
        /// <summary>バッチ毎ファイルの単発変動率チェック項目及び変動率閾値(%)</summary>
        public int SUP_OnceVdcChangeRf1ParamNo { get; set; }
        public int SUP_OnceVdcChangeRf2ParamNo { get; set; }
        public int SUP_OnceVdcChengeRate { get; set; }

        /// <summary>バッチ毎ファイルの連続変動率チェック項目、変動率閾値(%)、連続ロット数</summary>
        public int SUP_MultiVdcChangeRf1ParamNo { get; set; }
        public int SUP_MultiVdcChangeRf2ParamNo { get; set; }
        public int SUP_MultiVdcChangeRate { get; set; }
        public int SUP_MultiVdcChangeCount { get; set; }

        /// <summary>バッチ毎ファイルの変動率判定リセット期間(分)</summary>
        public int SUP_VdcChangeExclusionTime { get; set; }
        
        /// <summary>タイプに紐つくプログラムを管理するQcParamNo</summary>
        public int SUP_ProgramParamNo { get; set; }

        
        #endregion

        public string DebugEICSServer { get; set; }
		public string DebugEICSDatabase { get; set; }

		/// <summary>シングルトンのSettingInfoインスタンス</summary>
		private static SettingInfo settingInfo;

		public List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> PlcExtractExclusionList { get; set; }

		/// <summary>バッチモードでの実行かどうか</summary>
		public bool IsBatchMode { get; set; }

		/// <summary>設定情報リスト</summary>
		public List<SettingInfo> SettingInfoList { get; set; }

		/// <summary>装置リスト</summary>
		public List<EquipmentInfo> EquipmentList { get; set; }

		/// <summary>部署CD</summary>
		public string SectionCD { get; set; }

		public string HonbanServer { get; set; }

		public string HonbanDB { get; set; }

		/// <summary>Omron製PLC使用時にPLCと通信するPCのIPが必要な為</summary>
		public string LocalHostIP { get; set; }

		// 内製装置出力ログが取得要求立てた後にファイルが出てないことへの対策（設定ファイル未設定時のデフォルト値）単位はミリ秒
		const int DEFAULT_MACHINE_LOT_WAIT_SEC = 300;

		public int MachineLogOutWaitmSec { get; set; }

		// AD8930VのIファイル出力後のLファイル出力ラグの為のウェイト(デフォルト)
		const int DEFAULT_DB_MACHINE_LOT_WAIT_SEC = 500;

		public int DBMachineLogOutWaitmSec { get; set; }

		/// <summary>ラインCD</summary>
		public int LineCD { get; set; }

		/// <summary>言語</summary>
		public string Language { get; set; }

		/// <summary>LENS2運用ラインかどうか</summary>
		public bool IsMappingMode { get; set; }

		/// <summary>net.exeへのフルパス</summary>
		public string NetExePath { get; set; }

		public string DummyAIModelNM { get; set; }

		public string DirCommonOutput { get; set; }

		/// <summary>マッピングファイル場所(MD)</summary>
		public string DirMDMapping { get; set; }

		/// <summary>マッピングファイル場所(AI)</summary>
		public string DirAIMapping { get; set; }

		/// <summary>マッピングファイルバックアップ場所(AI)</summary>
		public string DirMappingBKFolder { get; set; }

		/// <summary>NASCA不良ファイル出力場所</summary>
		public string DirNASCAFolder { get; set; }

		/// <summary>KISS使用フラグ</summary>
		public string KissFG { get; set; }

		/// <summary>BlackJumboDog使用フラグ</summary>
		public string BlackJumboDogFG { get; set; }

		public bool NotUseTmQDIWFG { get; set; }

		/// <summary>MAP仕様区分</summary>
		private string MapKB { get; set; }

		public bool MapFG;

		public string BMCauseCD { get; set; }
		public string BMClassCD { get; set; }
		public string BMDefectCD { get; set; }

		/// <summary>アウトライン仕様区分</summary>
		//private string OutlineKB { get; set; }
		//public bool OutlineFG;

		/// <summary>NMC仕様区分</summary>
		public string NmcKB { get; set; }

		public bool NmcFG;

		//public string NmcFG;

		/// <summary>ラインの種類</summary>
		public string LineType { get; set; }

		/// <summary>品種グループ</summary>
		public string TypeGroup { get; set; }

		/// <summary>PLC受信タイムアウト時間</summary>
		public int PLCReceiveTimeout { get; set; }

		/// <summary>WBマッピング仕様区分</summary>
		private string WBMappingKB { get; set; }

		public bool WBMappingFG;

		//public bool LENSLinkEnable { get; set; }

		/// <summary>パッケージ仕様区分</summary>
		public string PackagePC { get; set; }

		public string ArmsDBPC { get; set; }

		/// <summary>SqlServer参照先PCアドレス</summary>
		public string PcNO { get; set; }

		///// <summary>AI、MDのアドレスが16段仕様</summary>
		//public bool MapMagazineStepFG { get; set; }

		/// <summary>発生した異常を通知するパソコンのアドレスリスト</summary>
		public List<CommunicateLine> ErrorContactList { get; set; }

		//public int ReceivePort { get; set; }
		//public int SendPort { get; set; }

		public int ErrorCheckIntervalSec { get; set; }

		const int DEFAULT_ERR_CHK_INTERVAL_SEC = 15;

		public int DeleteSysLogIntervalDay { get; set; }

		public Encoding FileEncode { get; set; }

		public bool IsSendJudgeOKAlwaysOnHSMS { get; set; }

		public string LotNoConvertID { get; set; }
		public int LotNoConvStartIndex { get; set; }
        public int LotNoConvLen { get; set; }
        public bool FullSerialNoModeFG { get; set; }
        public int FullSerialNoMarkingDigit { get; set; }

		public class ResultPriorityJudge
		{
			public int StartNgNo { get; set; }
			public string StartPlcAddr { get; set; }
			public int Length { get; set; }
		}

		public ResultPriorityJudge ResultPriorityJudgeInst { get; set; }

		public int MasterMissDisplayCount { get; set; }

		#endregion



		#region arms_info

		public string DebugARMSServer { get; set; }
		public string DebugARMSDatabase { get; set; }

		/// <summary>
		/// 工程CD
		/// </summary>
		public int ArmsAIProcessNO { get; set; }
		public int ExistBMProcessNO { get; set; }

        public List<string> ArmsServerList { get; set; }

		#endregion
	}
}