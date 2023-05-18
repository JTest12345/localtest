using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace GEICS
{
    public partial class frmDrawGraph : Form
    {
        Common Com = new Common();
        Painter p = new Painter();

        int _nLineCD = 0;
        int _nDefectNO = 0;
        string _sLotNO = "";
        string _sType = "";
        string _sResult = "";
        string _sInspectionNM = "";
        string _sInspectionNMAdd = "";
        DateTime _dtMeasure = DateTime.MinValue;
        string _sEquiNO = "";
        string _sAssetsNM = "";
        int _nBackNum = 0;
        int _nMultiNO = 0;
        int _nProcessNO = 0;

        Chart[] chart2 = null;//監視項目用
        Chart[] chart1 = null;//関連項目

        //グラフに描画されているLotのリスト
        List<string> ListLot = new List<string>();

        //描画対象Lot
        List<string> _TargetLotList = new List<string>();

        List<SortedList<int, QCLogData>> cndDataItem_Watch = new List<SortedList<int, QCLogData>>();
        List<SortedList<int, QCLogData>> cndDataItem_Relation = new List<SortedList<int, QCLogData>>();

        F03_TrendChart _parent = null;


        ///// <summary>
        ///// frmDrawGraphAndList.csの関連グラフ(btnAllGraph)押下時使用
        ///// </summary>
        ///// <param name="nLineCD"></param>
        ///// <param name="nDefectNO">不具合No</param>
        ///// <param name="sLotNO">異常のあったLotNO</param>
        ///// <param name="sType">異常のあったType</param>
        ///// <param name="sResult"></param>
        ///// <param name="sInspectionNM"></param>
        ///// <param name="dtMeasure"></param>
        ///// <param name="sEquiNO"></param>
        ///// <param name="sAssetsNM"></param>
        ///// <param name="nBackNum">異常のあったLotNOから遡るデータ数</param>
        ///// <param name="nMultiNO"></param>
        ///// <param name="nProcessNO"></param>
        ///// <param name="parent"></param>
        ////public frmDrawGraph(int nLineCD, int nDefectNO, string sLotNO, string sType, string sResult, string sInspectionNM, DateTime dtMeasure, string sEquiNO, string sAssetsNM, int nBackNum, int nMultiNO, int nProcessNO,frmDrawGraphAndList parent)
        //public frmDrawGraph(int nLineCD, int nDefectNO, string sLotNO, string sType, string sResult, string sInspectionNM, DateTime dtMeasure, string sEquiNO, string sAssetsNM, int nBackNum, int nMultiNO, int nProcessNO,List<string> TargetLotList,F03_TrendChart parent)
        //{
        //    InitializeComponent();

        //    _nLineCD = nLineCD;
        //    _nDefectNO = nDefectNO;
        //    _sLotNO = sLotNO;
        //    _sType = sType;
        //    _sResult = sResult;
        //    _sInspectionNMAdd = sInspectionNM;
        //    int npos=sInspectionNM.IndexOf("(F");
        //    if (npos>0)
        //    {
        //        _sInspectionNM = _sInspectionNMAdd.Substring(npos, _sInspectionNMAdd.Length - npos);
        //    }
        //    else
        //    {
        //        _sInspectionNM = _sInspectionNMAdd;
        //    }
        //    _dtMeasure = dtMeasure;
        //    _sEquiNO = sEquiNO;
        //    _sAssetsNM = sAssetsNM;
        //    _nBackNum = nBackNum;
        //    _nMultiNO = nMultiNO;
        //    _nProcessNO = nProcessNO;
        //    _parent = parent;
        //    _TargetLotList = TargetLotList;


        //    SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();
        //    SortedList<int, GraphData> ListGraphData_Relation = new SortedList<int, GraphData>();

        //    //他から呼ばれる為、以下の処理はコンストラクタへ
        //    //監視項目リスト取得
        //    ListGraphData_Watch = GETGraphInfo(_nDefectNO, 2);//2が監視項目の意味
        //    if (this._parent.Closed_FG) return;

        //    //関連項目リスト取得
        //    ListGraphData_Relation = GETGraphInfo(_nDefectNO, 1);//1が関連項目の意味
        //    if (this._parent.Closed_FG) return;

        //    //監視項目コントロール追加
        //    CreateGraphDesign_Watch(ListGraphData_Watch, 2);
        //    if (this._parent.Closed_FG) return;

        //    //関連項目コントロール追加
        //    CreateGraphDesign_Relation(ListGraphData_Watch.Count, ListGraphData_Relation, 1);
        //    if (this._parent.Closed_FG) return;

        //    //監視項目グラフにデータ流し込み
        //    DrawGraph(ListGraphData_Watch, ListLot, 2);
        //    if (this._parent.Closed_FG) return;

        //    //関連項目グラフにデータ流し込み
        //    DrawGraph(ListGraphData_Relation, ListLot, 1);
        //}

        private void CreateGraphDesign_Watch(SortedList<int, GraphData> ListGraphData, int nMode)
        {
            this.SuspendLayout();
            int nNum = ListGraphData.Count;

            Label[] objLabel = new Label[nNum];

            chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart[nNum];//監視項目用
            ChartArea[] chartArea2 = new ChartArea[nNum];
            Legend[] legend2 = new Legend[nNum];

            int x = 0, y = 0;
            this.Text = "[不具合内容] : " + ListGraphData[0].Defect;

            for (int i = 0; i < nNum; i++)
            {
                ///
                ///Label
                /// 
                objLabel[i] = new System.Windows.Forms.Label();
                objLabel[i].AutoSize = true;
                if (i % 2 == 0)//左側
                {
                    x = 0;
                }
                else//右側
                {
                    x = 505;
                }
                y = (205 * (i / 2) + 5);
                objLabel[i].Location = new System.Drawing.Point(x, y);

                objLabel[i].Name = "label" + (i + 1);
                objLabel[i].Size = new System.Drawing.Size(62, 12);
                objLabel[i].TabIndex = 28;
                objLabel[i].Text = "[■グラフ番号" + ListGraphData[i].InspectionNO + "]" + ListGraphData[i].Timing+" : "+ ListGraphData[i].InspectionNM;

                this.Controls.Add(objLabel[i]);

                // 
                // chart
                // 
                chart2[i] = new System.Windows.Forms.DataVisualization.Charting.Chart();
                chartArea2[i] = new ChartArea();
                legend2[i] = new Legend();

                ((System.ComponentModel.ISupportInitialize)(chart2[i])).BeginInit();

                chart2[i].Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top ) | System.Windows.Forms.AnchorStyles.Left)));
                chart2[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
                chart2[i].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chart2[i].BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
                chart2[i].BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chart2[i].BorderlineWidth = 2;
                chart2[i].BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chartArea2[i].Area3DStyle.Inclination = 40;
                chartArea2[i].Area3DStyle.IsClustered = true;
                chartArea2[i].Area3DStyle.IsRightAngleAxes = false;
                chartArea2[i].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
                chartArea2[i].Area3DStyle.Perspective = 9;
                chartArea2[i].Area3DStyle.Rotation = 25;
                chartArea2[i].Area3DStyle.WallWidth = 3;
                chartArea2[i].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea2[i].AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[i].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[i].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea2[i].AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[i].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[i].BackColor = System.Drawing.Color.OldLace;
                chartArea2[i].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chartArea2[i].BackSecondaryColor = System.Drawing.Color.White;
                chartArea2[i].BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chartArea2[i].CursorX.IsUserEnabled = true;
                chartArea2[i].CursorX.IsUserSelectionEnabled = true;
                chartArea2[i].CursorY.IsUserEnabled = true;
                chartArea2[i].CursorY.IsUserSelectionEnabled = true;
                chartArea2[i].Name = "GRAPH";
                chartArea2[i].ShadowColor = System.Drawing.Color.Transparent;
                chart2[i].ChartAreas.Add(chartArea2[i]);
                legend2[i].Alignment = System.Drawing.StringAlignment.Far;
                legend2[i].BackColor = System.Drawing.Color.Transparent;
                legend2[i].Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                legend2[i].IsTextAutoFit = false;
                legend2[i].Name = "Default";
                chart2[i].Legends.Add(legend2[i]);
                chart2[i].Location = new System.Drawing.Point(x, y);
                chart2[i].Name = "chart1";
                chart2[i].Size = new System.Drawing.Size(500, 200);
                chart2[i].TabIndex = 1;
                this.Controls.Add(chart2[i]);
            }
            this.ResumeLayout(false);
        }

        private void CreateGraphDesign_Relation(int nNum1, SortedList<int, GraphData> ListGraphData, int nMode)
        {
            this.SuspendLayout();
            int nNum2=ListGraphData.Count;
            //objPlotSurface2D1 = new NPlot.Windows.PlotSurface2D[nNum2];
            Label[] objLabel = new Label[nNum2];

            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart[nNum2];//監視項目用
            ChartArea[] chartArea1 = new ChartArea[nNum2];
            Legend[] legend1 = new Legend[nNum2];

            int x = 0, y = 0, p = 0;
            if (nNum1 % 2 == 0)
            {
                if (nNum1 == 2)
                {
                    p = 205 * (nNum1 / 2);
                }
                else
                {
                    p = (205 * ((nNum1 / 2) - 1)) ;
                }
            }
            else
            {
                p = (205 * ((nNum1 / 2)+1));
            }

            for (int i = 0; i < nNum2; i++)
            {
                ///
                ///Label
                /// 
                objLabel[i] = new System.Windows.Forms.Label();
                objLabel[i].AutoSize = true;
                if (i % 2 == 0)//左側
                {
                    x = 0;
                }
                else//右側
                {
                    x = 505;
                }
                y = p+(205 * (i / 2) + 5);

                objLabel[i].Location = new System.Drawing.Point(x, y);

                objLabel[i].Name = "label" + (nNum1+i + 1);
                objLabel[i].Size = new System.Drawing.Size(62, 12);
                objLabel[i].TabIndex = 28;
                objLabel[i].Text = "[グラフ番号" + ListGraphData[i].InspectionNO + "]" + ListGraphData[i].Timing + " : " + ListGraphData[i].InspectionNM;

                this.Controls.Add(objLabel[i]);
                // 
                // chart
                // 
                chart1[i] = new System.Windows.Forms.DataVisualization.Charting.Chart();
                chartArea1[i] = new ChartArea();
                legend1[i] = new Legend();

                ((System.ComponentModel.ISupportInitialize)(chart1[i])).BeginInit();

                chart1[i].Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top) | System.Windows.Forms.AnchorStyles.Left)));
                chart1[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
                chart1[i].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chart1[i].BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
                chart1[i].BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chart1[i].BorderlineWidth = 2;
                chart1[i].BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chartArea1[i].Area3DStyle.Inclination = 40;
                chartArea1[i].Area3DStyle.IsClustered = true;
                chartArea1[i].Area3DStyle.IsRightAngleAxes = false;
                chartArea1[i].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
                chartArea1[i].Area3DStyle.Perspective = 9;
                chartArea1[i].Area3DStyle.Rotation = 25;
                chartArea1[i].Area3DStyle.WallWidth = 3;
                chartArea1[i].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea1[i].AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea1[i].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea1[i].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea1[i].AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea1[i].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea1[i].BackColor = System.Drawing.Color.OldLace;
                chartArea1[i].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chartArea1[i].BackSecondaryColor = System.Drawing.Color.White;
                chartArea1[i].BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea1[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chartArea1[i].CursorX.IsUserEnabled = true;
                chartArea1[i].CursorX.IsUserSelectionEnabled = true;
                chartArea1[i].CursorY.IsUserEnabled = true;
                chartArea1[i].CursorY.IsUserSelectionEnabled = true;
                chartArea1[i].Name = "GRAPH";
                chartArea1[i].ShadowColor = System.Drawing.Color.Transparent;
                chart1[i].ChartAreas.Add(chartArea1[i]);
                legend1[i].Alignment = System.Drawing.StringAlignment.Far;
                legend1[i].BackColor = System.Drawing.Color.Transparent;
                legend1[i].Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                legend1[i].IsTextAutoFit = false;
                legend1[i].Name = "Default";
                chart1[i].Legends.Add(legend1[i]);
                chart1[i].Location = new System.Drawing.Point(x, y);
                chart1[i].Name = "chart";
                chart1[i].Size = new System.Drawing.Size(500, 200);
                chart1[i].TabIndex = 1;
                this.Controls.Add(chart1[i]);
            }
            this.ResumeLayout(false);
        }

        private SortedList<int, GraphData> GETGraphInfo2(int nTimming, int nWatchNO)
        {
            int nCnt = 0;
            string[] textArray = new string[] { };
            string sWork = "";
            string sInspectionNM = "";

            SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

            string BaseSql = "Select Defect_NM,Timing_NM,Inspection_NO,Inspection_NM,Process_NO From TvQDIW WITH(NOLOCK) Where Watch_NO={0} AND Timing_NO={1} ORDER BY Process_NO";
            string sqlCmdTxt = string.Format(BaseSql, nWatchNO, nTimming);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        GraphData wGraphData = new GraphData();                               //こっちが正解
                        wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
                        wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
                        wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));

                        sInspectionNM = (Convert.ToString(reader["Inspection_NM"]));
                        if (sInspectionNM.Substring(0, 1) == "F")
                        {
                            sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
                        }
                        //wGraphData.InspectionNM = (Convert.ToString(reader["Inspection_NM"]));
                        wGraphData.InspectionNM = sInspectionNM;

                        sWork = Convert.ToString(reader["Process_NO"]);
                        textArray = sWork.Split(',');
                        for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
                        {
                            wGraphData.Process.Add(Convert.ToInt32(textArray[i]));
                        }

                        wList.Add(nCnt, wGraphData);
                        nCnt = nCnt + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wList;
        }

        private SortedList<int, GraphData> GETGraphInfo(int nDefectNO, int nWatchNO)
        {
            int nCnt = 0;
            string[] textArray = new string[] { };
            string sWork = "";
            string sInspectionNM = "";

            SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

            string BaseSql = "Select Defect_NM,Timing_NM,Inspection_NO,Inspection_NM,Process_NO From TvQDIW WITH(NOLOCK) Where Watch_NO={0} AND Defect_NO={1} ORDER BY Process_NO";
            string sqlCmdTxt = string.Format(BaseSql, nWatchNO, nDefectNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        GraphData wGraphData = new GraphData();                               //こっちが正解
                        wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
                        wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
                        wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));

                        sInspectionNM = (Convert.ToString(reader["Inspection_NM"]));
                        if (sInspectionNM.Substring(0, 1) == "F")
                        {
                            sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
                        }

                        //wGraphData.InspectionNM = (Convert.ToString(reader["Inspection_NM"]));
                        wGraphData.InspectionNM = sInspectionNM;

                        sWork = Convert.ToString(reader["Process_NO"]);
                        textArray = sWork.Split(',');
                        for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
                        {
                            wGraphData.Process.Add(Convert.ToInt32(textArray[i]));
                        }

                        wList.Add(nCnt, wGraphData);
                        nCnt = nCnt + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wList;
        }

        private void frmDrawGraph_Load(object sender, EventArgs e)
        {

        }

        private string GetModel(string sEquiNo)
        {
            string sModel = "";
            string BaseSql = "SELECT Model_NM FROM TmEQUI WITH(NOLOCK) Where Equipment_NO='{0}'";

            string sqlCmdTxt = string.Format(BaseSql, sEquiNo);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        sModel = Convert.ToString(reader["Model_NM"]).Trim();  //設備番号
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return sModel;
        }

        //ﾀﾞｲﾎﾞﾝﾀﾞｰ装置
        private string GETWhereEquiDB(int nInlineCD)
        {
            string WhereSql = "@";
            string BaseSql = "SELECT Equipment_NO FROM TvLSET WITH(NOLOCK) WHERE Inline_CD ={0} AND Assets_NM='ﾀﾞｲﾎﾞﾝﾀﾞｰ'";
            string sqlCmdTxt = string.Format(BaseSql, nInlineCD);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        WhereSql = WhereSql + " OR NttSJSB.Plant_CD='" + Convert.ToString(reader["Equipment_NO"]).Trim() + "'";  //設備番号
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            
            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";
            return WhereSql;
        }

        //ﾀﾞｲﾎﾞﾝﾀﾞｰとﾜｲﾔｰﾎﾞﾝﾀﾞｰ装置
        private string GETWhereEquiWB(int nInlineCD)
        {
            string WhereSql = "@";
            string BaseSql = "SELECT Equipment_NO FROM TvLSET WITH(NOLOCK) WHERE Inline_CD ={0} AND (Assets_NM='ﾀﾞｲﾎﾞﾝﾀﾞｰ' OR Assets_NM='ﾜｲﾔｰﾎﾞﾝﾀﾞｰ')";
            string sqlCmdTxt = string.Format(BaseSql, nInlineCD);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        WhereSql = WhereSql + " OR NttSJSB.Plant_CD='" + Convert.ToString(reader["Equipment_NO"]).Trim() + "'";  //設備番号
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";
            return WhereSql;
        }
        
        //ﾓｰﾙﾄﾞ機
        private string GETWhereEquiMD(int nInlineCD)
        {
            string WhereSql = "@";
            string BaseSql = "SELECT Equipment_NO FROM TvLSET WITH(NOLOCK) WHERE Inline_CD ={0} AND Assets_NM='ﾓｰﾙﾄﾞ機'";
            string sqlCmdTxt = string.Format(BaseSql, nInlineCD);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        WhereSql = WhereSql + " OR NttSJSB.Plant_CD='" + Convert.ToString(reader["Equipment_NO"]).Trim() + "'";  //設備番号
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";
            return WhereSql;
        }

        private void frmDrawGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible=false;
            e.Cancel = true;
        }
    }
}


