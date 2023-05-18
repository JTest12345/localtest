using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using SLCommonLib.DataBase;

namespace EICS
{
    #region QCIL

    /// <summary>
    /// 装置情報
    /// </summary>
    public class EquipmentInfo : ICloneable
    {
        public int LineNO { get; set; }
        public string AssetsNM { get; set; }
        public string ModelNM { get; set; }
        public string EquipmentNO { get; set; }
        public string MachineNM { get; set; }
        public string TypeCD { get; set; }
        public string ChipNM { get; set; }
		public bool BMCountFG { get; set; }
        public string DirWBMagazine { get; set; }
        public string IPAddressNO { get; set; }
        public string PortNO { get; set; }
        public string InputFolderNM { get; set; }
        public Constant.ReportType ReportType { get; set; }
		public bool AIInspectPointCheckFG { get; set; }
		public bool IsOutputMagOnHSMS { get; set; }
		public bool IsOutputCommonPath { get; set; }
		public bool IsNotSendZeroMapping { get; set; }
		/// <summary>タイプ選択用コンボボックスの無効フラグ</summary>
		public bool UnSelectableTypeFG { get; set; }
		public bool WaitForRenameByArmsFG { get; set; }
		public bool IsOutputCIFSResultFG { get; set; }
		public bool IsOutputPLCResultFG { get; set; }
		public int DateStrIndex { get; set; }
		public int DateStrLen { get; set; }
		public int EndIdLen { get; set; }
		public string StartFileDirNm { get; set; }
		public string EndFileDirNm { get; set; }
		public string PLCType { get; set; }
		public string PLCProtocol { get; set; }

		public string EncodeStr { get; set; }
		public string AvailableAddress { get; set; }
		public bool EnablePreVerifyFG { get; set; }
		public bool EnableOpeningChkFg { get; set; }
        public string PLCEncode { get; set; }
        public bool FullParameterFG { get; set; }
        public bool SubtractWBMMErrorFg { get; set; }
        public bool LMArmsHandshakeFG { get; set; }
        public bool UnSelectableWorkFG { get; set; }

		public bool ForciblyEnableSequencialFileProcessFg { get; set; }

		//KAIJO製WBのSPファイルが3つ出てくるというので、3つのSPファイルを結合して処理する機能をオフにするか
		//装置改造が一斉に入る事は、まれなので号機個別に切り替えが出来るようにするためのフラグ
		public bool Disable3SPFilesSupportFunc  { get; set; }

		//NASCA不良登録用ファイルの出力を行うかどうかのフラグ
		public bool IsOutputNasFile { get; set; }
		//ローダにQRが付いており、かつファイル内にマガジンNOを出力する仕組みがある
		public bool HasLoaderQRReader { get; set; }
        //ASMダイボンダーで開始時にO/Pファイルに加えMファイルが出力されるか
        public bool MFileExists { get; set; }

        public string TypeGroupCD { get; set; }

        //<--後工程合理化/エラー集計
        public int PostProcessMonitoringCycleSec { get; set; }
        //-->後工程合理化/エラー集計

        //ErrConvのProcNo列を参照するかどうかのフラグ。※201807時点ではmpdファイルにしか適用なし(MMファイルは無効)
        public bool ErrConvWithProcNo { get; set; }

        public string DisplayTypeCD
        {
            get
            {
                if (string.IsNullOrEmpty(TypeGroupCD))
                {
                    return TypeCD;
                }
                else
                {
                    return TypeGroupCD;
                }
            }
        }

        public object Clone()
		{
			return MemberwiseClone();
		}

        /// <summary>
        /// ASMダイボンダーで完了時にSファイルが出力されるか
        /// </summary>
        public bool SFileExists { get; set; }
    }

    /// <summary>
    /// ライン装置情報
    /// </summary>
    public class LSETInfo : ICloneable
    {
        public int InlineCD { get; set; }
        public int DispCD { get; set; }
        /// <summary>設備CD:Sxxxx</summary>
        public string EquipmentNO { get; set; }
        public string EquipmentCD { get; set; }
        public int ProcessCD { get; set; }
        public string SeqCD { get; set; }
        public string IPAddressNO { get; set; }
        public int PortNO { get; set; }
        public string InputFolderNM { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public string LastUpdDT { get; set; }

        public string AssetsNM { get; set; }
        public string TypeCD { get; set; }
        public string ChipNM { get; set; }
        public string DirWBMagazine { get; set; }
        public Constant.ReportType ReportType { get; set; }
        public string ModelNM { get; set; }
        public string MachineNM { get; set; }
        public string MachineSeqNO { get; set; }
        public string LoaderAddressNO { get; set; }
        public byte LoaderPlcNodeNO { get; set; }
		public string ThreadGrpCD { get; set; }
		public bool MainThreadFG { get; set; }
		public string EquipPartID { get; set; }
        public bool ReferMultiServerFG { get; set; }
		public bool EnableResultPriorityJudge_FG { get; set; }
		public EquipmentInfo EquipInfo { get; set; }
        public string  MachineFolderNM{ get; set; }

        public object Clone()
		{
			LSETInfo temp = (LSETInfo)MemberwiseClone();
			((LSETInfo)temp).EquipInfo = (EquipmentInfo)((LSETInfo)temp).EquipInfo.Clone();

			return temp;
		}
    }

    /// <summary>
    /// 処理ファイル情報
    /// </summary>
    public class FILEFMTInfo 
    {
        public int QCParamNO { get; set; }
        public string ParameterNM { get; set; }
        public int ColumnNO { get; set; }
        public string SearchNM { get; set; }
        public string MachinePrefixNM { get; set; }
        public string PrefixNM { get; set; }
		public string ChipNM { get; set; }
		public string HeaderNM { get; set; }
		public bool StartUpFG { get; set; }
		public string EquipPartID { get; set; }
        public string XPath { get; set; }
        public int XPath_SearchNO { get; set; }


        /// <summary>
        /// ログファイル紐付けマスタ[TmFILEFMT]取得
        /// </summary>
        /// <param name="prefixNM">ファイル種類</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けマスタ</returns>
        public static List<FILEFMTInfo> GetData(string prefixNM, LSETInfo lsetInfo, string materialCD)
        {
            System.Data.Common.DbDataReader rd = null;
            List<FILEFMTInfo> filefmtList = new List<FILEFMTInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmFILEFMT.QcParam_NO, TmPRM.Parameter_NM, TmFILEFMT.Column_NO, TmFILEFMT.Header_NM, TmFILEFMT.Search_NM, TmFILEFMT.MachinePrefix_NM, TmFILEFMT.Prefix_NM, TmPRM.Chip_NM, TmFILEFMT.XPath, TmFILEFMT.XPath_SearchNO
                            FROM TmFILEFMT WITH(nolock)              
                            INNER JOIN TmPRM WITH(nolock) ON TmPRM.QcParam_NO = TmFILEFMT.QcParam_NO
                            WHERE (TmFILEFMT.Model_NM = @ModelNM) AND (TmFILEFMT.Del_FG = 0) ";

                if (prefixNM != "")
                {
                    sql += " AND (TmFILEFMT.Prefix_NM = @PrefixNM) ";
                    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNM);
                }
                if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
                {
                    sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
                    conn.SetParameter("@ChipNM", SqlDbType.VarChar, lsetInfo.ChipNM);
                }

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);

                using (rd = conn.GetReader(sql))
                {
                    if (!rd.HasRows)
                    {
                        //設定されていない場合、装置処理停止
                        string message = string.Format(Constant.MessageInfo.Message_27, materialCD, prefixNM);
                        throw new Exception(message);
                    }

                    int ordXPath = rd.GetOrdinal("XPath");
                    int ordXPath_SearchNO = rd.GetOrdinal("XPath_SearchNO");

                    while (rd.Read())
                    {
                        FILEFMTInfo filefmtInfo = new FILEFMTInfo();
                        filefmtInfo.QCParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                        filefmtInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                        filefmtInfo.ColumnNO = Convert.ToInt32(rd["Column_NO"]);
                        filefmtInfo.HeaderNM = Convert.ToString(rd["Header_NM"]).Trim();
                        filefmtInfo.SearchNM = Convert.ToString(rd["Search_NM"]).Trim();
                        filefmtInfo.MachinePrefixNM = Convert.ToString(rd["MachinePrefix_NM"]).Trim();
                        filefmtInfo.PrefixNM = Convert.ToString(rd["Prefix_NM"]).Trim();

                        if (rd.IsDBNull(ordXPath) == false)
                        {
                            filefmtInfo.XPath = rd.GetString(ordXPath).Trim();
                        }
                        else
                        {
                            filefmtInfo.XPath = string.Empty;
                        }

                        if (rd.IsDBNull(ordXPath_SearchNO) == false)
                        {
                            filefmtInfo.XPath_SearchNO = rd.GetInt32(ordXPath_SearchNO);
                        }
                        else
                        {
                            filefmtInfo.XPath_SearchNO = 0;
                        }
                        filefmtList.Add(filefmtInfo);
                    }
                }
            }

            return filefmtList;
        }
    }

    /// <summary>
    /// 処理ファイル情報（WB）
    /// </summary>
    public class FILEFMTWBInfo
    {
        public string TypeCD { get; set; }
        public string Model_NM { get; set; }

        public string PrefixNM { get; set; }
        public string ParameterNM { get; set; }
        public int QCParamNO { get; set; }
        public int FunctionNO { get; set; }
        public string SearchNM { get; set; }
        public int SearchNO { get; set; }
        public int Comma_NO { get; set; }
    }

    /// <summary>
    /// 処理メッセージ情報
    /// </summary>
    public class MSGFMTInfo
    {
		public string MsgTypeCD { get; set; }
        public string TypeCD { get; set; }

        public int QcParamNO { get; set; }
        public string ParameterNM { get; set; }

        public int SearchParamNO { get; set; }
		public int SearchGrpParamNO { get; set; }
        public string SearchParamNM { get; set; }
        public int SearchValueNO { get; set; }
    }

    /// <summary>
    /// 装置エラー変換情報
    /// </summary>
    public class ErrConvInfo 
    {
        public string EquipmentNO { get; set; }
        public int QcParamNO { get; set; }
        public string EquiErrNO { get; set; }
        public string NascaErrNO { get; set; }
    }



    /// <summary>
    /// フレーム情報
    /// </summary>
    public class FRAMEInfo 
    {
        public long FrameNO { get; set; }
        public string TypeCD { get; set; }
        public int XPackageCT { get; set; }
        public int YPackageCT { get; set; }
        public int MagazineStepCT { get; set; }
        public int MagazineStepMAXCT { get; set; }

        public int FramePackageCT { get; set; }
        public int MagazinPackageCT { get; set; }
        public int MagazinPackageMAXCT { get; set; }
        public int AroundColumnCT { get; set; }
        public int LoadStepCD { get; set; }
        public int DirectionCD { get; set; }
		public int SamplingModeCD { get; set; }
		public int SamplingCT { get; set; }

        public int AroundPackageCT { get; set; }
        public AroundInspectionType AroundInspectType { get; set; }
        public enum AroundInspectionType
        {
            Package,
            Column,
        }

        public WirebonderFrameOutOrder WBFrameOutOrder { get; set; }
        public enum WirebonderFrameOutOrder 
        {
            Asc,
            Desc
        }

        /// <summary>
        /// 全フレームのY(列)数
        /// </summary>
        public int ColumnCT 
        {
            get 
            {
                return MagazinPackageMAXCT / YPackageCT;
            }
        }

        /// <summary>
        /// ロギングアドレスから列番号を取得する
        /// </summary>
        /// <param name="addressNo"></param>
        /// <returns></returns>
        public int GetColumnNO(int addressNo)
        {
            double remainder = Math.IEEERemainder(addressNo, YPackageCT);
            if (remainder == 0)
            {
                return Convert.ToInt32(addressNo / YPackageCT);
            }
            else 
            {
                return Convert.ToInt32(Math.Ceiling((double)addressNo / (double)YPackageCT));
            }
        }
    }

    /// <summary>
    /// マッピング順序並び替え対象
    /// </summary>
    public class MPGORDERInfo 
    {
        public long ProcNO { get; set; }
    }

    /// <summary>
    /// 樹脂少対象情報
    /// </summary>
    public class DEFECTRESINInfo 
    {
        public int AddressNO { get; set; }
    }

    /// <summary>
    /// NASCA不良情報
    /// </summary>
    public struct NascaDefectInfo
    {
        public string DefItemCD { get; set; }
        public string DefCauseCD { get; set; }
        public string DefClassCD { get; set; }
        public int DefectCT { get; set; }
    }

    /// <summary>
    /// ファイル値情報
    /// </summary>
    public class FileValueInfo 
    {
        public string MeasureDT { get; set; }
        public string MagazineNO { get; set; }
        public string LotNO { get; set; }
        public double TargetVAL { get; set; }
        public double TargetAveVAL { get; set; }
        //public double TargetMaxVAL { get; set; }
        //public double TargetMinVAL { get; set; }
        //public double TargetSumVAL { get; set; }
        public double TargetSigmaVAL { get; set; }
        public string TargetStrVAL { get; set; }
        public int NascaDefectVAL { get; set; }
        public bool SubmitFG { get; set; }
    }

    /// <summary>
    /// 処理ファイル並替後情報
    /// </summary>
    public class MachineFileSortInfo
    {
        public string MachineFilePath { get; set; }
        public DateTime LastWriteDT { get; set; }
    }

    /// <summary>
    /// マガジン情報
    /// </summary>
    public class MagInfo
    {
        public string sMagazineNO;      //マガジン番号
        public string sNascaLotNO;      //NascaLot番号
        public string sMaterialCD;      //社内型番 
        public int MagazineMaxStepCT { get; set; }
        public int MagazineStepCT { get; set; }
        public int FrameXPackageCT { get; set; }
        public int FrameYPackageCT { get; set; }
        public int FramePackageCT { get; set; }
        public int MagazinePackageCT { get; set; }
        public DateTime? MeasureDT { get; set; }
		public long ProcNO { get; set; }
		public DateTime StartDT { get; set; }
		public DateTime EndDT { get; set; }

		string ConnectionString;

        /// <summary>
        /// マガジンNOか確認
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>結果</returns>
        public static bool IsMagazineNO(string value)
        {
            if (value.Substring(0, value.IndexOf(" ")) == "30")
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

		public static string GetMagLotNOFromQR(string qrData)
		{
			return qrData.Split(' ')[1];
		}

		public MagInfo(string serverNO)
		{
#if Debug
			this.ConnectionString = string.Format("Server={2}\\SQLEXPRESS;Database=ARMS_SV;Connect Timeout=60;UID={0};PWD={1};Application Name=EICS[傾向管理システム]", "inline", "R28uHta", serverNO);
#else
			this.ConnectionString = string.Format(EICS.Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", serverNO);
#endif
		}

		public MagInfo()
		{
			// 使用している気配が無い為コメントアウト 2015/1/11 n.yoshimoto
			// this.ConnectionString = ConnectDB.getConnString(Constant.DBConnectGroup.ARMS);
		}
    }

    /// <summary>
    /// エラー内容
    /// </summary>
    public class ErrMessageInfo 
    {
        public ErrMessageInfo(string messageVAL, Color showColor) 
        {
            MessageVAL = messageVAL;
            ShowColor = showColor;
        }
        public string MessageVAL { get; set; }
        public Color ShowColor { get; set; }

		public static List<ErrMessageInfo> AddHeaderInfo(string headerStr, List<ErrMessageInfo> targetList)
		{
			List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();

			foreach (ErrMessageInfo target in targetList)
			{
				target.MessageVAL = string.Format("{0} {1}", headerStr, target.MessageVAL);
				errMessageList.Add(target);
			}

			return errMessageList;
		}

    }

    /// <summary>
    /// マッピング基ファイル内容
    /// </summary>
    public class MappingBaseInfo 
    {
        public MappingBaseInfo (int unitNO, int addressNO, string tranNO, string errorCD)
        {
            UnitNO = unitNO;
            AddressNO = addressNO;
            TranNO = tranNO;
            ErrorCD = errorCD;
        }

        public int UnitNO { get; set; }
        public int AddressNO { get; set; }
        public string TranNO { get; set; }
        public string ErrorCD { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MappingMachineInfo 
    {
        public MappingMachineInfo(int stepNO, string addressRange) 
        {
            StepNO = stepNO;
            AddressRange = addressRange;
        }

        public int StepNO { get; set; }
        public string AddressRange { get; set; }
    }

    /// <summary>
    /// TnLot(ARMS)
    /// </summary>
    public class ARMSLotInfo
    {
        /// <summary>製品型番</summary>
        public string TypeCD { get; set; }

        /// <summary>ロット番号</summary>
        public string LotNO { get; set; }

        /// <summary>全数検査フラグ</summary>
        public bool FullSizeInspFG { get; set; }

        /// <summary>マッピング検査フラグ</summary>
        public bool MappingInspFG { get; set; }

        /// <summary>フラグ</summary>
        public bool ChangepointlotFG { get; set; }

    }

    /// <summary>
    /// MDディスペンスタイム情報
    /// </summary>
    public class MdDispenseInfo 
    {
        /// <summary>シリンジNO</summary>
        public int SyringeNO { get; set; }

        /// <summary>段数</summary>
        public int StepNO { get; set; }

        /// <summary>モールド打順</summary>
        public int MoldNO { get; set; }
    }

    #endregion

	public class MachineFileInfo
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string Content { get; set; }
	}

    public class LotPrevInfo
    {
        public string lotno { get; set; }
        public string procnm { get; set; }
        public string clasnm { get; set; }
        public string plantnm { get; set; }
    }

    #region ARMS

    public class WorkFrowInfo
    {
        public string TypeCD { get; set; }
        public long WorkOrder { get; set; }
        public long ProcNO { get; set; }
        public long OrderMove { get; set; }
        public int MagDevideStatus { get; set; }
    }

    #endregion

    //<--後工程合理化/エラー集計
    #region Nasca
    public class InputScheduleInfo
    {
        public string lotno { get; set; }
        public int potno { get; set; }
    }

    public class NascaTranInfo
    {
        public string lotno { get; set; }
        public DateTime startdt { get; set; }
        public DateTime compltdt { get; set; }
    }

    public class NascaLotCharInfo
    {
        public string LotNo { get; set; }
        public string LotCharCd { get; set; }
        public string LotCharVal { get; set; }
        public string ListVal { get; set; }
    }
    #endregion
    //-->後工程合理化/エラー集計

    #region 前システムから転用

    /// <summary>
    /// 紐付け不良の設備とディレクトリ情報
    /// </summary>
    public class UnchainInfo
    {
        private string _equi = "";
        private string _dir = "";

        public string Equi
        {
            get
            {
                return _equi;
            }
            set
            {
                _equi = value;
            }
        }
        public string Dir
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }
    }

    public class ArmsLotInfo
    {
        public string StartDT { get; set; }
        public string EndDT { get; set; }
        public string TypeCD { get; set; }
        public string LotNO { get; set; }
        public string InMag { get; set; }
		public string OutMag { get; set; }
		public long ProcNO { get; set; }
    }

    //設備情報
    public struct EquiInfo
    {
        public string sEquipmentNO;     //設備番号('SLC-'は付けない)
        public string sAssetsNM;        //設備名
        public string sMachinSeqNO;     //号機番号
        public string sModelNM;         //装置型式
        public int nDispNO;             //表示順
        public string sIPAddressNO;     //IPアドレス
        public string sInputFolderNM;   //装置出力ファイルの取得先
    }

    //パラメータ情報
    public struct ParamInfo
    {
        public int nQcParamNO;          //パラメータ管理番号
        public string sManageNM;        //パラメータ管理方法
        public string sParamNM;         //パラメータ名
        public string sTotalKB;
        public string ChangeUnitVAL;
    }

    //許容値
    public struct ParamLimit
    {
        public decimal dParamMAX;       //許容最大値(bflg=False 数値の場合こちらを参照)
        public decimal dParamMIN;       //許容最小値(bflg=False 数値の場合こちらを参照)
        public string sParamVAL;        //設定値(bflg=True  文字列の場合こちらを参照)
        public int nflg;                //1:文字列,0:数値
    }

    //<--SAC-AI00235(傾向確認データ追加) 2010/04/15 Y.Matsushima Start
    ///
    ///樹脂量測定値データ シリンジ1～5の各max,min,ave,mode,σの集計用
    ///     modeは1μm単位での最頻値。最頻値が複数存在する場合、その複数の平均値を表示。
    ///     例：-80が5件,-70が5件,-60が5件の場合、((-80)+(-70)+(-60))/3=-70がmode値
    public class PrmInfo
    {
        /// <summary>
        /// カウント情報を表現します
        /// </summary>
        private PrmTotal[] _prmTotal;

        /// <summary>コンストラクタ</summary>
        public PrmInfo()
        {
            _prmTotal = new PrmTotal[0];
        }

        public PrmTotal[] PrmTotal
        {
            get
            {
                return _prmTotal;
            }
            set
            {
                _prmTotal = value;
            }
        }
    }

    public class PrmTotal
    {
        /// <summary>
        /// 一時保管用
        /// </summary>
        private double _tmp;
        /// <summary>
        /// 一時保管用
        /// </summary>
        private SortedList<int, PrmMode> _prmlist;

        /// <summary>
        /// Max値
        /// </summary>
        private double _max;
        /// <summary>
        /// Min値
        /// </summary>
        private double _min;
        /// <summary>
        /// 平均値
        /// </summary>
        private double _ave;
        /// <summary>
        /// σ値
        /// </summary>
        private double _sigma;
        /// <summary>
        /// mode値
        /// </summary>
        private double _mode;

        /// <summary>コンストラクタ</summary>
        public PrmTotal()
        {
            _tmp = 0;
            _max = 0;
            _min = 0;
            _ave = 0;
            _sigma = 0;
            _mode = 0;
            _prmlist = new SortedList<int, PrmMode>();
        }
        public double Tmp
        {
            get
            {
                return _tmp;
            }
            set
            {
                _tmp = value;
            }
        }
        public double Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
        public double Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }
        public double Ave
        {
            get
            {
                return _ave;
            }
            set
            {
                _ave = value;
            }
        }
        public double Sigma
        {
            get
            {
                return _sigma;
            }
            set
            {
                _sigma = value;
            }
        }
        public double Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }
        public SortedList<int, PrmMode> Prmlist
        {
            get
            {
                return _prmlist;
            }
            set
            {
                _prmlist = value;
            }
        }
    }

    ///
    ///樹脂量測定値データ シリンジ1～5の各max,min,ave,mode,σの集計用
    ///
    public class PrmMode
    {
        ///
        /// 値
        ///
        private double _param;
        ///
        /// 個数
        ///
        private int _number;

        public PrmMode()
        {
            _param = 0;
            _number = 0;
        }

        public double Param
        {
            get
            {
                return _param;
            }
            set
            {
                _param = value;
            }
        }
        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
    }
    //-->SAC-AI00235(傾向確認データ追加) 2010/04/15 Y.Matsushima End

    ///
    ///監視項目
    ///
    public class InspData
    {
		public class QcParamInfo
		{
			public int No { get; set; }
			public bool UnManageTrendFG { get; set; }
			public bool WithoutFileFmtFG { get; set; }

			public QcParamInfo(int no, bool unManageFg, bool withoutFg)
			{
				this.No = no;
				this.UnManageTrendFG = unManageFg;
				this.WithoutFileFmtFG = withoutFg;
			}
		}
        ///
        /// Defect_NO
        /// 1:ダイス欠け,2:ダイス割れ
        private int _defect;

        private int _inspectionno;

        ///
        /// SQL番号
        /// ",190,"や",147,152,157,162,167,"のようなカンマ区切りで数字が入っている
        /// 
        /// 0～10000:装置情報連携データ取得用SQL
        /// 10001～20000:NASCA異常項目取得用SQLを使用する(DB工程)
        /// 20001～30000:NASCA異常項目取得用SQLを使用する(WB工程)
        /// 30001～40000:強度試験用SQLを使用する
        /// 40001～50000:NASCA異常項目取得用SQLを使用する(MD工程)
        //private int _number;
        //private int[] _number;
        private List<QcParamInfo> _paramInfo;

        ///
        /// 傾向管理項目名
        ///
        private string _param;

        ///
        /// 単/複数号機監視
        /// 0:単数号機監視 1:複数号機監視
        private int _multi;

        public InspData()
        {
            _defect = 0;
            _inspectionno = 0;
			//_number = "";
			_paramInfo = new List<QcParamInfo>();
            _param = "";
            _multi = 0;
        }

        public int Defect
        {
            get
            {
                return _defect;
            }
            set
            {
                _defect = value;
            }
        }
        public int InspectionNO
        {
            get
            {
                return _inspectionno;
            }
            set
            {
                _inspectionno = value;
            }
        }
        public string Param
        {
            get
            {
                return _param;
            }
            set
            {
                _param = value;
            }
        }
        public List<QcParamInfo> ParamInfo
        {
            get
            {
                return _paramInfo;
            }
            set
            {
				_paramInfo = value;
            }
        }

        public int Multi
        {
            get
            {
                return _multi;
            }
            set
            {
                _multi = value;
            }
        }
    }

    ///
    ///監視生データ
    ///
    public class QCLogData
    {
        ///
        /// 設備番号
        /// 
        private string _equino;

        ///
        /// LotNo
        ///
        private string _lotno;

        ///
        /// Type名
        ///
        private string _typecd;

        ///
        /// Log値
        ///
        private double _data;
        ///
        /// Defect_NO
        /// 1:ダイス欠け,2:ダイス割れ
        private int _defect;

        ///
        /// Inspection_NO
        /// 1:Post Inspection(Placement-x)ｽﾞﾚ平均値
        /// 2:Post Inspection(Placement-x)ｽﾞﾚ標準偏差...
        private int _inspectionno;

        ///
        /// QCParamNo
        ///
        private int _qcprmno;

        ///
        /// 計測日時
        ///
        private DateTime _measuredt;

        ///
        /// Message
        ///
        private string _messagenm;

        public QCLogData()
        {
            _equino = "";
            _lotno = "";
            _typecd = "";
            _defect = 0;
            _inspectionno = 0;
            _qcprmno = 0;
            _data = 0;
            _measuredt = Convert.ToDateTime("9999/01/01");
            _messagenm = "";
        }
        public string EquiNO
        {
            get
            {
                return _equino;
            }
            set
            {
                _equino = value;
            }
        }
        public string LotNO
        {
            get
            {
                return _lotno;
            }
            set
            {
                _lotno = value;
            }
        }
        public string TypeCD
        {
            get
            {
                return _typecd;
            }
            set
            {
                _typecd = value;
            }
        }
        public int Defect
        {
            get
            {
                return _defect;
            }
            set
            {
                _defect = value;
            }
        }
        public int InspectionNO
        {
            get
            {
                return _inspectionno;
            }
            set
            {
                _inspectionno = value;
            }
        }
        public int QcprmNO
        {
            get
            {
                return _qcprmno;
            }
            set
            {
                _qcprmno = value;
            }
        }
        public double Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public DateTime MeasureDT
        {
            get
            {
                return _measuredt;
            }
            set
            {
                _measuredt = value;
            }
        }

        public string MessageNM
        {
            get
            {
                return _messagenm;
            }
            set
            {
                _messagenm = value;
            }
        }
    }

    #endregion

}
