using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS
{

    public class TmLINEInfo 
    { 
        /// <summary>インラインコード　     [例]1003</summary>
        public int Inline_CD { get; set; }
        /// <summary>インライン名           [例]#03 N(SV) 自動搬送</summary>
        public string Inline_NM { get; set; }
        /// <summary>工場名                 [例]N</summary>
        public string Plant_NM { get; set; }
        /// <summary>ラインカテゴリネーム   [例]SV・tV自動搬送ライン</summary>
        public string LineCate_NM { get; set; }

		public bool NotUseTmQDIW { get; set; }

        public TmLINEInfo()
        {
            Inline_CD=0;
            Inline_NM="";
            Plant_NM="";
            LineCate_NM = "";
        }
    }

    //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
    /// <summary>
    /// 管理項目の補足情報
    /// </summary>
    public class PrmAddInfo {
        private string _info1;
        private string _info2;
        private string _info3;

        public PrmAddInfo() {
            _info1 = string.Empty;
            _info2 = string.Empty;
            _info3 = string.Empty;
        }
        public string Info1
        {
            get
            {
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }
        public string Info2
        {
            get
            {
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }
        public string Info3
        {
            get
            {
                return _info3;
            }
            set
            {
                _info3 = value;
            }
        }
    }
    //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima

    //<--SAC-AI00235(傾向確認データ追加) 2010/04/15 Y.Matsushima Start
    ///
    ///樹脂量測定値データ シリンジ1～5の各max,min,ave,mode,σの集計用
    ///     modeは1μm単位での最頻値。複数の最頻値が存在する場合、その複数の平均値を表示。
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
        private SortedList<int,PrmMode> _prmlist;

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
            _prmlist = new SortedList<int,PrmMode>();
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
        ///
        /// Defect_NO
        /// 1:ダイス欠け,2:ダイス割れ
        private int _defect;

        ///
        /// SQL番号
        /// 0～1000:装置情報連携データ取得用SQL
        /// 9000:NASCA異常項目取得用SQLを使用する
        /// 9001:強度試験用SQLを使用する
        private int _number;

        ///
        /// 傾向管理項目名
        ///
        private string _param;

        public InspData()
        {
            _defect = 0;
            _number = 0;
            _param = "";
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

    ///
    ///グラフ描画用
    ///
    public class GraphData
    {
        ///
        /// 不具合内容
        /// 例:ダイス欠け...
        private string _defect;
        ///
        /// 関連装置
        /// 例:DB,WB...
        private string _timing;

        ///
        /// 傾向管理項目名
        /// 例:Post Inspection(Placement-x)ｽﾞﾚ平均値
        private int _inspectionno;

        ///
        /// 傾向管理項目名
        /// 例:Post Inspection(Placement-x)ｽﾞﾚ平均値
        private string _inspectionnm;

        ///
        /// 処理番号
        /// 1000以下は装置情報連携システム
        /// 9000はNASCA,9001は強度試験
        //private int _process;
        private List<int> _process;

        public GraphData()
        {
            _defect = "";
            _timing = "";
            _inspectionno = 0;
            _inspectionnm = "";
            _process = new List<int>();
        }
        public string Defect
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
        public string Timing
        {
            get
            {
                return _timing;
            }
            set
            {
                _timing = value;
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
        public string InspectionNM
        {
            get
            {
                return _inspectionnm;
            }
            set
            {
                _inspectionnm = value;
            }
        }
        public List<int> Process
        {
            get
            {
                return _process;
            }
            set
            {
                _process = value;
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
        /// QCParamNo
        ///
        private int _qcprmno;
        //private List<int> _qcprmno;
        ///
        /// QCParamNM
        ///
        private string _qcprmnm;
        ///
        /// 計測日時
        ///
        private DateTime _measuredt;

        public QCLogData()
        {
            _equino = "";
            _lotno = "";
            _typecd = "";
            _defect = 0;
            _qcprmno = 0;
            //_qcprmno = new List<int>();
            _qcprmnm = "";
            _data = 0;
            _measuredt = Convert.ToDateTime("9999/01/01");
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
        //public List<int> QcprmNO
        //{
        //    get
        //    {
        //        return _qcprmno;
        //    }
        //    set
        //    {
        //        _qcprmno = value;
        //    }
        //}
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
        public string QcprmNM
        {
            get
            {
                return _qcprmnm;
            }
            set
            {
                _qcprmnm = value;
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

        public QCLogData Clone()
        {
            QCLogData retV = new QCLogData();
            retV.Data = this.Data;
            retV.Defect = this.Defect;
            retV.EquiNO = this.EquiNO;
            retV.LotNO = this.LotNO;
            retV.MeasureDT = this.MeasureDT;
            retV.QcprmNO = this.QcprmNO;
            retV.TypeCD = this.TypeCD;

            return retV;
        }
    }
}
