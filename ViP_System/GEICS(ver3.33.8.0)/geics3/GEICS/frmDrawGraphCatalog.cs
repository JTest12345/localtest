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
using SLCommonLib.Commons;
using System.IO;
using GEICS.Database;

namespace GEICS
{
    public partial class frmDrawGraphCatalog : Form
    {
		//グラフ描画開始Y位置
		const int CHART_AREA_START_YPos = 60;

		const int CHART_WIDTH_DEFALT = 500;
		const int CHART_HEIGHT_DEFAULT = 200;
		

        object objMissing = System.Reflection.Missing.Value;

        Common Com = new Common();
        Painter p = new Painter();
        EventHandler eventH;

        int _nLineCD = 0;
        string _sType = "";
        DateTime _dtMeasure = DateTime.MinValue;
        int _nSheetNo = 1;
        bool _flg = false;

        /// <summary> メッセージ </summary>
        //private IMessage systemMessage = null;

        string _sLine = "";//ライン名取得 例：「N-3(SV) 高生産性」
        string _syyyyMM = "";//年月取得
        string _syyyyMMdd = "";//年月日取得
        string _sReport = "";
        string _sAssetsNM = "";

        //仕様追加分
        int _nTimmingNO = 0;
        string _sWhereSQLEqui = "";
        DateTime _dtStart = DateTime.MinValue;
        DateTime _dtEnd = DateTime.MinValue;

        Chart[] chart2 = null;//監視項目用
        Chart[] chart1 = null;//関連項目

        //グラフに描画されているLotのリスト
        List<string> ListLot = new List<string>();

        //描画対象Lot
        public List<string> _TargetLotList = new List<string>();

        List<SortedList<int, QCLogData>> cndDataItem_Watch = new List<SortedList<int, QCLogData>>();
        List<SortedList<int, QCLogData>> cndDataItem_Relation = new List<SortedList<int, QCLogData>>();

        F03_TrendChart _parent = null;
        SubDrawGraph _SubDrawGraph;

		ExcelControl xls;

        /// <summary>
        /// frmDrawGraphAndList.csの関連グラフ(btnAllGraph)押下時使用
        /// </summary>
        /// <param name="nLineCD"></param>
        /// <param name="nDefectNO">不具合No</param>
        /// <param name="sLotNO">異常のあったLotNO</param>
        /// <param name="sType">異常のあったType</param>
        /// <param name="sResult"></param>
        /// <param name="sInspectionNM"></param>
        /// <param name="dtMeasure"></param>
        /// <param name="sEquiNO"></param>
        /// <param name="sAssetsNM"></param>
        /// <param name="nBackNum">異常のあったLotNOから遡るデータ数</param>
        /// <param name="nMultiNO"></param>
        /// <param name="nProcessNO"></param>
        /// <param name="parent"></param>
        public frmDrawGraphCatalog(int nLineCD, int nTimmingNO, string sWhereSQLEqui, string sType, DateTime dtStart, DateTime dtEnd, F03_TrendChart parent)
        {
            InitializeComponent();
            eventH = new System.EventHandler(this.chart_Click);

            _nLineCD = nLineCD;
            _nTimmingNO = nTimmingNO;
            _sWhereSQLEqui = sWhereSQLEqui;
            _sType = sType;
            _dtStart = dtStart;
            _dtEnd = dtEnd;
            _parent = parent;

			ViewChart();

        }

		public void ViewChart()
		{
			SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();
			SortedList<int, GraphData> ListGraphData_Relation = new SortedList<int, GraphData>();

			//監視・関連項目関係無し
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				ListGraphData_Watch = GETGraphInfo_Map(_nTimmingNO);//_nTimmingNOはダイボンダー:1...
			}
			else
			{
				ListGraphData_Watch = GETGraphInfo(_nTimmingNO);//_nTimmingNOはダイボンダー:1...
			}
			if (this._parent.Closed_FG) return;


			Dictionary<int, bool> showTargetDict = GetShowTargetGraph(ListGraphData_Watch, chkbResultLogOnly.Checked);

			//監視項目コントロール追加
			CreateGraphDesign_Watch(ListGraphData_Watch, 2, showTargetDict, CHART_WIDTH_DEFALT + 2 * sbChartWidth.Value, CHART_HEIGHT_DEFAULT + 2 * sbChartHeight.Value);
			if (this._parent.Closed_FG) return;

			//監視項目グラフにデータ流し込み
			DrawGraph(ListGraphData_Watch, showTargetDict, ListLot, 2);
		}


		//表示するグラフをデータが存在するものに絞る為の情報取得
		public Dictionary<int, bool> GetShowTargetGraph(SortedList<int, GraphData> ListGraphData_Watch, bool resultLogOnlyFg)
		{
			Dictionary<int, bool> showTargetDict = new Dictionary<int, bool>();

			for (int i = 0; i < ListGraphData_Watch.Count; i++)
			{
				List<int> qcParamList = ListGraphData_Watch[i].Process;

				SortedList<int, QCLogData> qcItemList = GetQCItem(qcParamList);

				if (qcItemList.Count > 0)
				{
					if (resultLogOnlyFg)
					{
						bool addTargetFg = false;

						foreach (int qcParamNo in qcParamList)
						{
							List<Prm> prmList = Prm.GetData(null, null, qcParamNo, "製品出来栄え", null);

							if (prmList.Count > 0)
							{

								if (chkbUpLim.Checked || chkbLowLim.Checked)
								{
									List<Plm> plmList = Plm.GetData(qcItemList[0].TypeCD, qcItemList[0].EquiNO, qcParamNo);

									if (plmList.Count > 0)
									{
										if (chkbUpLim.Checked)
										{
											if (plmList[0].ParameterMAX >= int.Parse(tbUpLim.Text))
											{
												continue;
											}
										}

										if (chkbLowLim.Checked)
										{
											if (plmList[0].ParameterMIN <= int.Parse(tbLowLim.Text))
											{
												continue;
											}
										}
									}
								}
								addTargetFg = true;
							}
						}

						if (addTargetFg)
						{
							showTargetDict.Add(i, true);
						}
					}
					else
					{
						showTargetDict.Add(i, true);
					}
				}
				else
				{
					showTargetDict.Add(i, false);
				}
			}

			return showTargetDict;
		}

        //新機能：レポート用
        public frmDrawGraphCatalog(int nLineCD, string nTimmingNO, string sWhereSQLEqui, string sType, DateTime dtStart, DateTime dtEnd,int nSheetNo,bool flg)
        {
            InitializeComponent();
            eventH = new System.EventHandler(this.chart_Click);

            _nLineCD = nLineCD;
            //_nTimmingNO = nTimmingNO;
            _sWhereSQLEqui = sWhereSQLEqui;
            _sType = sType;
            _dtStart = dtStart;
            _dtEnd = dtEnd;
            _flg = flg;//false→日次,true→月次
            _nSheetNo = nSheetNo;

            _sLine = Com.GetInlineString(_nLineCD);//ライン名取得 例：「N-3(SV) 高生産性」
            _syyyyMM = _dtStart.ToString("yyyyMM");//年月取得
            _syyyyMMdd = _dtStart.ToString("yyyyMMdd");//年月日取得
            _sReport = "";
            _sAssetsNM = "";

            if (_flg == true)
            {
                _sReport = "月次";
            }
            else
            {
                _sReport = "日次";
            }
            //switch (_nTimmingNO)
            //{
            //    case 1: _sAssetsNM = "ダイボンダー"; break;
            //    case 2: _sAssetsNM = "ワイヤーボンダー"; break;
            //    case 3: _sAssetsNM = "外観検査機"; break;
            //    case 4: _sAssetsNM = "モールド機"; break;
            //}
            SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();

            //他から呼ばれる為、以下の処理はコンストラクタへ
            //監視・関連項目関係無し
            ListGraphData_Watch = GETGraphInfo(nTimmingNO, _flg);

            //項目なしの場合
            if (ListGraphData_Watch.Count == 0)
            {
                return;
            }

			Dictionary<int, bool> showTargetDict = GetShowTargetGraph(ListGraphData_Watch, false);

            //監視項目コントロール追加
            CreateGraphDesign_Watch(ListGraphData_Watch, 2, showTargetDict);

            //監視項目グラフにデータ流し込み
            DrawGraph(ListGraphData_Watch, showTargetDict, ListLot, 2);
        }

        //新機能：レポート用(バッチ)
        public frmDrawGraphCatalog(int nLineCD, string nTimmingNO, string sWhereSQLEqui, string sType, DateTime dtStart, DateTime dtEnd, int nSheetNo, bool flg, string sbatch)
        {
            InitializeComponent();
            eventH = new System.EventHandler(this.chart_Click);

            _nLineCD = nLineCD;
            //_nTimmingNO = nTimmingNO;
            _sWhereSQLEqui = sWhereSQLEqui;
            _sType = sType;
            _dtStart = dtStart;
            _dtEnd = dtEnd;
            _flg = flg;//false→日次,true→月次
            _nSheetNo = nSheetNo;

            _sLine = Com.GetInlineString(_nLineCD);//ライン名取得 例：「N-3(SV) 高生産性」
            _syyyyMM = _dtStart.ToString("yyyyMM");//年月取得
            _syyyyMMdd = _dtStart.ToString("yyyyMMdd");//年月日取得
            _sReport = "";
            _sAssetsNM = "";

            if (_flg == true)
            {
                _sReport = "月次";
            }
            else
            {
                _sReport = "日次";
            }
			switch (nTimmingNO)
            {
				case "1": _sAssetsNM = "ﾀﾞｲﾎﾞﾝﾀﾞｰZD"; break;
				case "2": _sAssetsNM = "ﾀﾞｲﾎﾞﾝﾀﾞｰLED"; break;
				//case "3": _sAssetsNM = "ﾌﾟﾗｽﾞﾏDB"; break;
				//case "4": _sAssetsNM = "ﾌﾟﾗｽﾞﾏWB"; break;
				case "5": _sAssetsNM = "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ"; break;
				case "6": _sAssetsNM = "外観検査機"; break;
				case "7": _sAssetsNM = "ﾓｰﾙﾄﾞ機"; break;
				//case "8": _sAssetsNM = "遠心沈降機"; break;
				//case "9": _sAssetsNM = "ﾀﾞｲｻｰ"; break;


				//case "1": _sAssetsNM = "ダイボンダー"; break;
				//case "2": _sAssetsNM = "ワイヤーボンダー"; break;
				//case "3": _sAssetsNM = "外観検査機"; break;
				//case "4": _sAssetsNM = "モールド機"; break;
            }
            SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();

            //他から呼ばれる為、以下の処理はコンストラクタへ
            //監視・関連項目関係無し
            ListGraphData_Watch = GETGraphInfo(nTimmingNO, _flg);

            //項目なしの場合
            if (ListGraphData_Watch.Count == 0)
            {
                return;
            }

            //監視項目コントロール追加
            CreateGraphDesign_WatchReport(ListGraphData_Watch, 2);
        }

        void SaveControlImage(Control c, string file)
        {
            int w = c.DisplayRectangle.Size.Width; // コントロールの幅
            int h = c.DisplayRectangle.Size.Height; // コントロールの高さ

            using (Bitmap bmp = new Bitmap(w, h))
            {
                c.DrawToBitmap(bmp, new Rectangle(0, 0, w, h));
                bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void OutputImage()
        {
            //string spath1 = Constant.StrDirReport + _sReport + "\\" + _sLine + "\\" + _syyyyMMdd + "\\" + _sAssetsNM + "\\" + _sType;
            //string spath1 = Constant.StrDirReport + _sReport + "\\" + _syyyyMMdd + "\\" + _sLine + "\\" + _sType + "\\" + _sAssetsNM;
            string spath1 = Configuration.GetAppConfigString("ReportOutputPath") + _sReport + "\\" + _syyyyMMdd + "\\" + _sLine + "\\" + _sType + "\\" + _sAssetsNM;
            string spath2 = "";

            //ディレクトリ作成
            if (System.IO.Directory.Exists(spath1) == false)
            {
                System.IO.Directory.CreateDirectory(spath1);
            }

            for (int i = 0; i < this.chart2.Length; i++)
            {
                spath2=spath1 + "\\" + i + ".png";

                if (System.IO.Directory.Exists(spath1) == false)
                {
                    System.IO.Directory.CreateDirectory(spath1);
                }
                else
                {
                    if (System.IO.File.Exists(spath2))
                    {
                        //登録済みﾌｧｲﾙは削除して次へ
                        System.IO.File.Delete(spath2);
                    }
                }

                SaveControlImage(this.chart2[i], spath2); // フォーム全体
            }
        }

        public void frmDrawGraphCatalog_Load(object sender, EventArgs e)
        {
            if (Program.sBatch != "")
            {
                //OutputImage();      //画像をサーバーへ保存

                InsertExcelFile();//↑の画像をExcelへ出力

                this.Close();       //フォームを閉じる
                this.Dispose();     //フォームを閉じる

                //this.chart1 = null;
                //this.chart2 = null;
                //GC.Collect();
            }
        }

        private void InsertExcelFile()
        {
            //string sfileNM = "【" + _sReport + "レポート】" + _sLine + "_" + _sAssetsNM + "_" + _syyyyMMdd + ".xls";
            //string spath1 = Constant.StrDirReport + _sReport + "\\" + _sLine + "\\" + _syyyyMMdd + "\\" + _sAssetsNM;
            //string spath2 = Constant.StrDirReport + _sReport + "\\" + _sLine + "\\" + _syyyyMMdd;

            string spath1 = Configuration.GetAppConfigString("ReportOutputPath") + _sReport + "\\" + _syyyyMMdd + "\\" + _sLine + "\\" + _sType + "\\" + _sAssetsNM;
            string spath2 = Configuration.GetAppConfigString("ReportOutputPath") + _sReport + "\\" + _syyyyMMdd;
            string sfileNM = "【" + _sReport + "レポート】" + _sLine + "_" + _syyyyMMdd + ".xls";
            //string sfileName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\" + SLCommonLib.Commons.Configuration.GetAppConfigString("ReportTemplateName");

			if (!Directory.Exists(spath1))
			{
				Directory.CreateDirectory(spath1);
			}

            using (ExcelControl excelControl = ExcelControl.GetInstance())
            {
                if (!excelControl.ImageOutput(spath1, spath2, _sAssetsNM, _sType, sfileNM, _nSheetNo))
                {
                    MessageBox.Show(Constant.MessageInfo.Message_19, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                excelControl.nonCloseFlg = false;
                excelControl.SaveToDir(spath2 + "\\" + sfileNM);

                //excelControl.xlApp.Save(spath1 + "\\" + sfileNM);
            }
        }

		private void CreateGraphDesign_Watch(SortedList<int, GraphData> ListGraphData, int nMode, Dictionary<int, bool> showTargetDict)
		{
			CreateGraphDesign_Watch(ListGraphData, nMode, showTargetDict, CHART_WIDTH_DEFALT, CHART_HEIGHT_DEFAULT);
		}

		private void CreateGraphDesign_Watch(SortedList<int, GraphData> ListGraphData, int nMode, Dictionary<int, bool> showTargetDict, int chartWidth, int chartHeight)
        {
            int nNum = ListGraphData.Count;

            Label[] objLabel = new Label[nNum];

            chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart[nNum];//監視項目用
            ChartArea[] chartArea2 = new ChartArea[nNum];
            Legend[] legend2 = new Legend[nNum];

			List<int> chartControlIndexList = new List<int>();
			for (int controlIndex = 0; controlIndex < this.Controls.Count; controlIndex++)
			{
				while (controlIndex < this.Controls.Count && Controls[controlIndex] is Chart)
				{
					Controls.RemoveAt(controlIndex);
				}
			}

            int x = 0, y = 0;

            this.Text = "[不具合内容] : " + ListGraphData[0].Defect;

			int chartIndex = 0;

            for (int i = 0; i < nNum; i++)
            {
				if (showTargetDict.ContainsKey(i) == false || showTargetDict[i] == false)
				{
					continue;
				}

                ///
                ///Label
                /// 
                objLabel[chartIndex] = new System.Windows.Forms.Label();
                objLabel[chartIndex].AutoSize = true;

				if (chartIndex % 2 == 0)//左側
                {
                    x = 0;
                }
                else//右側
                {
                    x = chartWidth + 5;
                }
				y = ((chartHeight + 5) * (chartIndex / 2) + CHART_AREA_START_YPos);
                objLabel[chartIndex].Location = new System.Drawing.Point(x, y);

				objLabel[chartIndex].Name = "label" + (chartIndex + 1);
                objLabel[chartIndex].Size = new System.Drawing.Size(62, 12);
                objLabel[chartIndex].TabIndex = 28;
                objLabel[chartIndex].Text = "[■グラフ番号" + ListGraphData[i].InspectionNO + "]" + ListGraphData[i].Timing + " : " + ListGraphData[i].InspectionNM;

                //this.Controls.Add(objLabel[i]);

                // 
                // chart
                // 
                chart2[chartIndex] = new Chart();
                chartArea2[chartIndex] = new ChartArea();
                legend2[chartIndex] = new Legend();

                ((System.ComponentModel.ISupportInitialize)(chart2[chartIndex])).BeginInit();

                chart2[chartIndex].Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top) | System.Windows.Forms.AnchorStyles.Left)));
                chart2[chartIndex].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
                chart2[chartIndex].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chart2[chartIndex].BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
                chart2[chartIndex].BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chart2[chartIndex].BorderlineWidth = 2;
                chart2[chartIndex].BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chartArea2[chartIndex].Area3DStyle.Inclination = 40;
                chartArea2[chartIndex].Area3DStyle.IsClustered = true;
                chartArea2[chartIndex].Area3DStyle.IsRightAngleAxes = false;
                chartArea2[chartIndex].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
                chartArea2[chartIndex].Area3DStyle.Perspective = 9;
                chartArea2[chartIndex].Area3DStyle.Rotation = 25;
                chartArea2[chartIndex].Area3DStyle.WallWidth = 3;
                chartArea2[chartIndex].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea2[chartIndex].AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[chartIndex].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[chartIndex].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea2[chartIndex].AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[chartIndex].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[chartIndex].BackColor = System.Drawing.Color.OldLace;
                chartArea2[chartIndex].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chartArea2[chartIndex].BackSecondaryColor = System.Drawing.Color.White;
                chartArea2[chartIndex].BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea2[chartIndex].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chartArea2[chartIndex].CursorX.IsUserEnabled = true;
                chartArea2[chartIndex].CursorX.IsUserSelectionEnabled = true;
                chartArea2[chartIndex].CursorY.IsUserEnabled = true;
                chartArea2[chartIndex].CursorY.IsUserSelectionEnabled = true;
                chartArea2[chartIndex].Name = "GRAPH";
                chartArea2[chartIndex].ShadowColor = System.Drawing.Color.Transparent;

                legend2[chartIndex].Alignment = System.Drawing.StringAlignment.Far;
                legend2[chartIndex].BackColor = System.Drawing.Color.Transparent;
                legend2[chartIndex].Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                legend2[chartIndex].IsTextAutoFit = false;
                legend2[chartIndex].Name = "Default";

                chart2[chartIndex].ChartAreas.Add(chartArea2[chartIndex]);

                chart2[chartIndex].Legends.Add(legend2[chartIndex]);
                chart2[chartIndex].Location = new System.Drawing.Point(x, y);
                chart2[chartIndex].Name = "chart1";
                chart2[chartIndex].Size = new System.Drawing.Size(chartWidth, chartHeight);
                chart2[chartIndex].TabIndex = 1;

                this.chart2[chartIndex].Click += eventH;

                this.Controls.Add(chart2[chartIndex]);
				
				chartIndex++;
            }
        }

        private void CreateGraphDesign_WatchReport(SortedList<int, GraphData> ListGraphData, int nMode) 
        {
            int nNum = ListGraphData.Count;

            //Label[] objLabel = new Label[nNum];
            //chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart[nNum];//監視項目用
            //ChartArea[] chartArea2 = new ChartArea[nNum];
            //Legend[] legend2 = new Legend[nNum];

            int x = 0, y = 0;

            this.Text = "[不具合内容] : " + ListGraphData[0].Defect;

            //-----HIshiguchi
            string spath1 = Configuration.GetAppConfigString("ReportOutputPath") + _sReport + "\\" + _syyyyMMdd + "\\" + _sLine + "\\" + _sType + "\\" + _sAssetsNM;
            string spath2 = "";

            //ディレクトリ作成
            if (System.IO.Directory.Exists(spath1) == false)
            {
                System.IO.Directory.CreateDirectory(spath1);
            }
            //--------

            string sWhereEqui;

            string[] textArray = new string[] { };
            sWhereEqui = ChangeFormat(_sWhereSQLEqui);
            bool flg = false;

            for (int i = 0; i < nNum; i++)
            {
                List<string> lotList = new List<string>();
                List<string> targetLotList = new List<string>();

                Painter painter = new Painter();

                Label objLabel = new Label();
                Chart chart = new Chart();
                ChartArea chartArea = new ChartArea();
                Legend legend = new Legend();

                ///
                ///Label
                /// 
                objLabel = new System.Windows.Forms.Label();
                objLabel.AutoSize = true;

                if (i % 2 == 0)//左側
                {
                    x = 0;
                }
                else//右側
                {
                    x = 505;
                }
                y = (205 * (i / 2) + 5);
                objLabel.Location = new System.Drawing.Point(x, y);

                objLabel.Name = "label" + (i + 1);
                objLabel.Size = new System.Drawing.Size(62, 12);
                objLabel.TabIndex = 28;
                objLabel.Text = "[■グラフ番号" + ListGraphData[i].InspectionNO + "]" + ListGraphData[i].Timing + " : " + ListGraphData[i].InspectionNM;

                //this.Controls.Add(objLabel[i]);

                // 
                // chart
                // 
                //chart2[i] = new Chart();
                //chartArea2[i] = new ChartArea();
                //legend2[i] = new Legend();

                ((System.ComponentModel.ISupportInitialize)(chart)).BeginInit();

                chart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top) | System.Windows.Forms.AnchorStyles.Left)));
                chart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
                chart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chart.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
                chart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chart.BorderlineWidth = 2;
                chart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
                chartArea.Area3DStyle.Inclination = 40;
                chartArea.Area3DStyle.IsClustered = true;
                chartArea.Area3DStyle.IsRightAngleAxes = false;
                chartArea.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
                chartArea.Area3DStyle.Perspective = 9;
                chartArea.Area3DStyle.Rotation = 25;
                chartArea.Area3DStyle.WallWidth = 3;
                chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                chartArea.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea.BackColor = System.Drawing.Color.OldLace;
                chartArea.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
                chartArea.BackSecondaryColor = System.Drawing.Color.White;
                chartArea.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                chartArea.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                chartArea.CursorX.IsUserEnabled = true;
                chartArea.CursorX.IsUserSelectionEnabled = true;
                chartArea.CursorY.IsUserEnabled = true;
                chartArea.CursorY.IsUserSelectionEnabled = true;
                chartArea.Name = "GRAPH";
                chartArea.ShadowColor = System.Drawing.Color.Transparent;

                legend.Alignment = System.Drawing.StringAlignment.Far;
                legend.BackColor = System.Drawing.Color.Transparent;
                legend.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                legend.IsTextAutoFit = false;
                legend.Name = "Default";

                chart.ChartAreas.Add(chartArea);

                chart.Legends.Add(legend);
                chart.Location = new System.Drawing.Point(x, y);
                chart.Name = "chart1";
                chart.Size = new System.Drawing.Size(500, 200);
                chart.TabIndex = 1;

                //this.chart2[i].Click += eventH;

                //this.Controls.Add(chart2[i]);

                SortedList<int, QCLogData> logDataList = GetQCItem(ListGraphData[i].Process);
                //cndDataItem_Watch.Add(GetQCItem(ListGraphData[i].Process));

                string tmpStr = "";
                if (flg == false)
                {
                    //if (cndDataItem_Watch[0].Count > 0)
                    if (logDataList.Count > 0)
                    {
                        //ListLot.Clear();
                        lotList.Clear();

                        //for (int j = 0; j < cndDataItem_Watch[0].Count; j++)
                        for (int j = 0; j < logDataList.Count; j++)
                        {
                            //tmpStr = cndDataItem_Watch[0].Values[j].LotNO;
                            tmpStr = logDataList.Values[j].LotNO;

                            //if (ListLot.Contains(tmpStr) == false)
                            if (!lotList.Contains(tmpStr))
                            {
                                //ListLot.Add(tmpStr);
                                lotList.Add(tmpStr);
                            }
                        }

                        //foreach (string tmpLot in this.ListLot)
                        foreach (string tmpLot in lotList)
                        {
                            //this._TargetLotList.Add(tmpLot);
                            targetLotList.Add(tmpLot);
                        }
                        //ListLot.Clear();
                        lotList.Clear();

                        flg = true;
                    }
                }

                //if (cndDataItem_Watch[i].Count > 0)
                if (logDataList.Count > 0)
                {
                    //p.DrawGraph(chart, cndDataItem_Watch[i], ListLot, 1);
                    painter.DrawGraph(chart, logDataList, lotList, 1);
                }

                //-----------

                spath2 = spath1 + "\\" + i + ".png";

                if (System.IO.Directory.Exists(spath1) == false)
                {
                    System.IO.Directory.CreateDirectory(spath1);
                }
                else
                {
                    if (System.IO.File.Exists(spath2))
                    {
                        //登録済みﾌｧｲﾙは削除して次へ
                        System.IO.File.Delete(spath2);
                    }
                }

                logDataList = null;
                painter = null;
                targetLotList = null;
                lotList = null;
                GC.Collect();

                chart.SaveImage(spath2, ChartImageFormat.Png);

                chart.Dispose();
                chart = null;
                chartArea = null;
                legend = null;
                objLabel = null;
                GC.Collect();

                //-----------

            }
        }

        void chart_Click(object sender, EventArgs e)
        {
            ((Chart)sender).Click -= eventH;
            int nScrollX = this.AutoScrollPosition.X;
            int nScrollY = this.AutoScrollPosition.Y;
            int nTop = ((Chart)sender).Top;
            int nLeft = ((Chart)sender).Left;
            int nWidth = ((Chart)sender).Width;
            int nHeight = ((Chart)sender).Height;

            _SubDrawGraph = new SubDrawGraph((Chart)sender, this._parent);
            _SubDrawGraph.ShowDialog();

			if (nScrollY < 0)
			{
				nScrollY += CHART_AREA_START_YPos;
			}

			((Chart)sender).Top = nTop - (nScrollY);
            ((Chart)sender).Left= nLeft;
            ((Chart)sender).Width = nWidth;
            ((Chart)sender).Height = nHeight;
            ((Chart)sender).Dock = System.Windows.Forms.DockStyle.None;

            this.Controls.Add(((Chart)sender));

            ((Chart)sender).Click += eventH;
            ((Chart)sender).Legends[0].MaximumAutoSize = 50;

            //<---000115対応「SLC・SGA要望対応」 Y.Matsushima 2011/02/10
            this.AutoScrollPosition =new Point(-nScrollX,-nScrollY);//取得時は負の値。設定時は正の値。
            //--->000115対応「SLC・SGA要望対応」 Y.Matsushima 2011/02/10
        }
        
        private void CreateGraphDesign_Relation(int nNum1, SortedList<int, GraphData> ListGraphData, int nMode)
        {
            this.SuspendLayout();
            int nNum2 = ListGraphData.Count;
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
                    p = (205 * ((nNum1 / 2) - 1));
                }
            }
            else
            {
                p = (205 * ((nNum1 / 2) + 1));
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
                y = p + (205 * (i / 2) + 5);

                objLabel[i].Location = new System.Drawing.Point(x, y);

                objLabel[i].Name = "label" + (nNum1 + i + 1);
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

        private SortedList<int, GraphData> GETGraphInfo(int nTimming)
        {
            int nCnt = 0;
            string[] textArray = new string[] { };
            string sWork = "";
            string sInspectionNM="";
            SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

            string BaseSql = "Select Distinct Inspection_NO " + 
                            "Defect_NM,Timing_NM,Inspection_NO,Inspection_NM,Process_NO " +
                            "From [TvQDIW] WITH(NOLOCK) " +
                            "Where Timing_NO={0} " +
                            "ORDER BY Inspection_NO ";

            string sqlCmdTxt = string.Format(BaseSql, nTimming);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        GraphData wGraphData = new GraphData();
                        wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
                        wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
                        wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));
                        
                        sInspectionNM=(Convert.ToString(reader["Inspection_NM"]));
                        if (sInspectionNM.Substring(0, 1) == "F")
                        {
                            sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
                        }
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

        private SortedList<int, GraphData> GETGraphInfo_Map(int nTimming)
        {
            int nCnt = 0;
            string[] textArray = new string[] { };
            string sWork = "";
            string sInspectionNM = "";
            SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

            string BaseSql = "Select Distinct Inspection_NO " +
                            "Defect_NM,Timing_NM,Inspection_NO,Inspection_NM,Process_NO " +
                            "From [TvQDIW_Map] WITH(NOLOCK) " +
                            "Where Timing_NO={0} " +
                            "ORDER BY Inspection_NO ";

            string sqlCmdTxt = string.Format(BaseSql, nTimming);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        GraphData wGraphData = new GraphData();
                        wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
                        wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
                        wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));

                        sInspectionNM = (Convert.ToString(reader["Inspection_NM"]));
                        if (sInspectionNM.Substring(0, 1) == "F")
                        {
                            sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
                        }
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

        private SortedList<int, GraphData> GETGraphInfo(string nTimming, bool flg)
        {
            int nCnt = 0;
            string[] textArray = new string[] { };
            string sWork = "";
            string sWhere = "";

            SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

            //flg=false→日次
            //    true →月次
            if (flg == false)
            {
                sWhere="Dayly_FG=1 ";
            }
            else
            {
                sWhere="Monthly_FG=1 ";
            }

            string BaseSql = "Select Timing_NM,Inspection_NO,Inspection_NM,Process_NO " +
                            "From [TvSlcReportSetting] WITH(NOLOCK) " +
                            "Where Timing_NO in({0}) AND " + sWhere +
                            "ORDER BY Inspection_NO ";

            string sqlCmdTxt = string.Format(BaseSql, nTimming);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        GraphData wGraphData = new GraphData();
                        wGraphData.Defect = _sType;
                        wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
                        wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));
                        string sInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
                        if (sInspectionNM.Substring(0, 1) == "F")
                        {
                            sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
                        }
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

        string ChangeFormat(string sWhere)
        {
            string sWhereSQL = "";

            sWhereSQL = sWhere.Replace("AND", "");//ANDはなくて良い
            sWhereSQL = sWhereSQL.Replace("Equipment_NO", "NttSJSB.Plant_CD");//Plant_CDへ

            return sWhereSQL;
        }

        private void DrawGraph(SortedList<int, GraphData> ListGraphData, Dictionary<int, bool> showTargetDict, List<string> ListLot, int Mode)
        {
            string sWhereEqui;

            string[] textArray = new string[] { };
            sWhereEqui = ChangeFormat(_sWhereSQLEqui);

            bool flg = false;
			cndDataItem_Watch = new List<SortedList<int, QCLogData>>();

            for (int i = 0; i < ListGraphData.Count; i++)
            {
				SortedList<int, QCLogData> qcItemList = GetQCItem(ListGraphData[i].Process);

				//if (qcItemList.Count > 0)
				//{
				//    cndDataItem_Watch.Add(qcItemList);
				//}
				//else
				//{
				//    continue;
				//}
				cndDataItem_Watch.Add(qcItemList);

                int endIndex = cndDataItem_Watch.Count - 1;

                string tmpStr = "";
                if (flg == false)
                {
                    if (cndDataItem_Watch[endIndex].Count > 0)
                    {
                        ListLot.Clear();

						//cndDataItem_Watch[endIndex] = cndDataItem_Watch[endIndex].OrderBy(c => c.Value.MeasureDT)

                        for (int j = 0; j < cndDataItem_Watch[endIndex].Count; j++)
                        {
                            tmpStr = cndDataItem_Watch[endIndex].Values[j].LotNO;
                            if (ListLot.Contains(tmpStr) == false)
                            {
                                ListLot.Add(tmpStr);
                            }
                        }

                        foreach (string tmpLot in this.ListLot)
                        {
                            this._TargetLotList.Add(tmpLot);
                        }
                        ListLot.Clear();
                        flg = true;
                    }
                }
            }

			string imgDirPath = Path.Combine(Path.GetTempPath(), @"GEICS_GRAPH\Img");

			if (Directory.Exists(imgDirPath))
			{
				Directory.Delete(imgDirPath, true);
				Directory.CreateDirectory(imgDirPath);
			}
			else
			{
				Directory.CreateDirectory(imgDirPath);
			}

			//for (int i = 0; i < cndDataItem_Watch.Count; i++)
			//{
			//    if (cndDataItem_Watch[i].Count > 0)
			//    {
			//        p.DrawGraph(chart2[i], cndDataItem_Watch[i], ListLot, 1);

			//        SaveControlImage(chart2[i], Path.Combine(imgDirPath, string.Format("{0}.png", i)));
			//    }
			//}

			int chartIndex = 0, index = 0;
			foreach (SortedList<int, QCLogData> cndDataItem in cndDataItem_Watch)
			{
				if (showTargetDict.ContainsKey(index) && showTargetDict[index])
				{
					//if (cndDataItem.Count > 0)
					//{
					//if (showTargetDict.ContainsKey(index++) == false || showTargetDict[index] == false)
					//{
					//    continue;
					//}

					//if (chart2[chartIndex] == null)
					//{
					//    continue;
					//}

					p.DrawGraph(chart2[chartIndex], cndDataItem, ListLot, 1);
					SaveControlImage(chart2[chartIndex], Path.Combine(imgDirPath, string.Format("{0}.png", chartIndex)));
					index++;
					chartIndex++;
				}
				else
				{
					index++;
				}
			}

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
                        WhereSql = WhereSql + " OR dbo.NttSJSB.Plant_CD='" + Convert.ToString(reader["Equipment_NO"]).Trim() + "'";  //設備番号
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

        private string GETWhereSQLProcess(List<int> wProcess)
        {
            string sWhereSql = "@";

            foreach (int nProcess in wProcess)
            {
                sWhereSql = sWhereSql + " OR QcParam_NO = " + nProcess;
            }

            sWhereSql = sWhereSql.Replace("@ OR", "AND (");
            sWhereSql = sWhereSql + ")";

            if (sWhereSql == "@)")
            {
                sWhereSql = "";
            }

            return sWhereSql;
        }

        private SortedList<int, QCLogData> GetQCItem(List<int> Process)
        {
            int nCnt = 0;

            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
            string sWhereSQL = GETWhereSQLProcess(Process);

            string sPrmNM = "";
            SortedList<int, string> Qcprmdt = new SortedList<int, string>();//例：<105,ｼﾘﾝｼﾞ1ﾊﾞｷｭｰﾑ圧>

            for (int i =0 ; i < Process.Count; i++)
            {
                sPrmNM = Com.GetTmPRM(Process[i]);
                Qcprmdt.Add(Process[i], sPrmNM);
            }
            string BaseSql = "SELECT NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT,QcParam_NO  FROM [TnLOG] WITH(NOLOCK)" +
                             "WHERE Inline_CD ={0} AND (Measure_DT>='{1}' AND Measure_DT<='{2}') AND NascaLot_No <> '' AND Seq_NO=1 AND Material_CD='{3}' " + _sWhereSQLEqui;
            string sqlCmdTxt = string.Format(BaseSql, _nLineCD, _dtStart, _dtEnd, _sType);

            //sqlCmdTxt = sqlCmdTxt + sWhereSQL + " ORDER BY Measure_DT ASC";
            sqlCmdTxt = sqlCmdTxt + sWhereSQL;

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    List<QCLogData> QCLogDataOrder = new List<QCLogData>();
                    while (reader.Read())
                    {
                        QCLogData wQCLogData = new QCLogData();                                //こっちが正解
                        wQCLogData.EquiNO = Convert.ToString(reader["Equipment_NO"]).Trim();   //設備番号
                        wQCLogData.LotNO = Convert.ToString(reader["NascaLot_NO"]).Trim();     //Lot
                        wQCLogData.TypeCD = _sType.Trim();                                     //Type
                        wQCLogData.MeasureDT = Convert.ToDateTime(reader["Measure_DT"]);       //計測日時
                        wQCLogData.Data = Convert.ToDouble(reader["DParameter_VAL"]);          //data
                        wQCLogData.Defect = 0;                                                 //監視項目No
                        wQCLogData.QcprmNO = Convert.ToInt32(reader["QcParam_NO"]);            //監視項目No

                        for (int i = 0; i < Qcprmdt.Count; i++)
                        {
                            if (wQCLogData.QcprmNO == Qcprmdt.Keys[i])
                            {
                                wQCLogData.QcprmNM = Qcprmdt.Values[i];
                            }
                        }
                        
                        QCLogDataOrder.Add(wQCLogData);
                    }

                    //ORDER BYを使うとタイムアウトになる為

					//IEnumerable<QCLogData> IEQCLogDataOrder = QCLogDataOrder.OrderBy(s => s.LotNO).ThenBy(s => s.MeasureDT);
					IEnumerable<QCLogData> IEQCLogDataOrder = QCLogDataOrder.OrderBy(s => s.MeasureDT);
                    foreach (QCLogData wQCLogDataOrder in IEQCLogDataOrder)
                    {
                        cndDataItem.Add(nCnt, wQCLogDataOrder);
                        nCnt = nCnt + 1;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }

            }
            return cndDataItem;
        }

		private void btnViewGraph_Click(object sender, EventArgs e)
		{
			ViewChart();

			//SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();
			//SortedList<int, GraphData> ListGraphData_Relation = new SortedList<int, GraphData>();

			////監視・関連項目関係無し
			//if (Constant.typeGroup == Constant.TypeGroup.MAP || Constant.notUseTmQdiwFG)
			//{
			//    ListGraphData_Watch = GETGraphInfo_Map(_nTimmingNO);//_nTimmingNOはダイボンダー:1...
			//}
			//else
			//{
			//    ListGraphData_Watch = GETGraphInfo(_nTimmingNO);//_nTimmingNOはダイボンダー:1...
			//}
			//if (this._parent.Closed_FG) return;

			////for (int i = 0; i < ListGraphData_Watch.Count; i++)
			////{
			////    SortedList<int, QCLogData> cndDataItem = GetQCItem(ListGraphData_Watch[i].Process);


			////    cndDataItem_Watch.Add();
			////}


			//Dictionary<int, bool> showTargetDict = GetShowTargetGraph(ListGraphData_Watch);

			////監視項目コントロール追加
			//CreateGraphDesign_Watch(ListGraphData_Watch, 2, showTargetDict);
			//if (this._parent.Closed_FG) return;

			////監視項目グラフにデータ流し込み
			//DrawGraph(ListGraphData_Watch, ListLot, 2);
		}

		private void sbChartWidth_ValueChanged(object sender, EventArgs e)
		{
			tbChartWidth.Text = (CHART_WIDTH_DEFALT + 2 * sbChartWidth.Value).ToString();
		}

		private void sbChartHeight_ValueChanged(object sender, EventArgs e)
		{
			tbChartHeight.Text = (CHART_HEIGHT_DEFAULT + 2 * sbChartHeight.Value).ToString();
		}

		private void btnOutputXls_Click(object sender, EventArgs e)
		{
			xls = ExcelControl.GetInstance();
			xls.OutputGraph();
		}

		private void frmDrawGraphCatalog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (xls != null)
			{
				xls.Dispose();
			}
		}
    }
}


