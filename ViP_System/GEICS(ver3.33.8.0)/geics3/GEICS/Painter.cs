using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data.SqlClient;
//using Microsoft.VisualBasic;

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace GEICS
{
	class Painter
	{
		//private const int BITMAP_X = 600;
		//private const int BITMAP_Y = 400;
		Common Com = new Common();

		public Painter()
		{
		}


		/// <summary>
		/// グラフ描画関数
		/// </summary>
		/// <param name="aChart"></param>
		/// <param name="cndDataItem"></param>
		/// <param name="ListLot"></param>
		/// <param name="nMode"></param>
		public void DrawGraph(Chart aChart, SortedList<int, QCLogData> cndDataItem, List<string> ListLot, int nMode)
		{
			string sModel, sManageNM;

			sModel = Com.GetMachineModel(cndDataItem[0].EquiNO);
			sManageNM = Com.GetManageNM(cndDataItem[0].QcprmNO);

			Draw(aChart, sModel, cndDataItem[0].QcprmNO, cndDataItem, sManageNM, cndDataItem[0].TypeCD, ListLot, nMode);

		}

		public SortedList<int, QCLogData> GetEquipCodeToNoList(SortedList<int, QCLogData> cndDataItem)
		{
			bool fAI = GetAIQuestion(cndDataItem[0].EquiNO);//True=外観検査機データ

			Dictionary<string, string> equiNoList = new Dictionary<string, string>();

			for (int i = 0; i < cndDataItem.Count; i++)
			{
				//外観検査データであれば、通ってきたワイヤーボンダー装置へ変換する。
				if (fAI == true)
				{
					cndDataItem[i].EquiNO = GetWBEquiNo(cndDataItem[i].LotNO, "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ", Common.nLineCD);
				}

				//既に変換済みか?「S*****」→「***号機(S*****)」
				if (!equiNoList.Keys.Contains(cndDataItem[i].EquiNO) && !string.IsNullOrEmpty(cndDataItem[i].EquiNO))
				{//No
					string equiNo = Com.AddCommentEquipmentNO(cndDataItem[i].EquiNO);
					equiNoList.Add(cndDataItem[i].EquiNO, equiNo);
					cndDataItem[i].EquiNO = equiNo;//表記変更「S*****」→「***号機(S*****)」

				}
				else
				{//Yes
					if (equiNoList.Keys.Contains(cndDataItem[i].EquiNO))
					{
						cndDataItem[i].EquiNO = equiNoList[cndDataItem[i].EquiNO];
					}
				}
			}
			return cndDataItem;
		}

		public List<string> GetTypeList(SortedList<int, QCLogData> cndDataItem)
		{
			List<string> typeList = new List<string>();

			foreach (KeyValuePair<int, QCLogData> cndData in cndDataItem)
			{
				if (!typeList.Contains(cndData.Value.TypeCD))
				{
					typeList.Add(cndData.Value.TypeCD);
				}
			}
			return typeList;
		}

		public List<string> GetPatternList(SortedList<int, QCLogData> cndDataItem)
		{
			List<string> patternList = new List<string>();
			for (int i = 0; i < cndDataItem.Count; i++)
			{
				if (!patternList.Contains(cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim()))
					patternList.Add(cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim());
			}

			return patternList;
		}

		public List<string> GetEquipNoList(SortedList<int, QCLogData> cndDataItem)
		{
			List<string> equipList = new List<string>();
			for (int i = 0; i < cndDataItem.Count; i++)
			{
				if (!equipList.Contains(cndDataItem[i].EquiNO))
					equipList.Add(cndDataItem[i].EquiNO);
			}

			return equipList;
		}

		public List<int> GetQcParamNoList(SortedList<int, QCLogData> cndDataItem)
		{
			List<int> qcParamList = new List<int>();

			foreach (KeyValuePair<int, QCLogData> cndData in cndDataItem)
			{
				if (!qcParamList.Contains(cndData.Value.QcprmNO))
				{
					qcParamList.Add(cndData.Value.QcprmNO);
				}
			}
			return qcParamList;
		}

		public void InitChart(Chart chart, string targetItem, SeriesChartType chartType, MarkerStyle? markerStyle, int? markerSize, Color lineMainColor, Color lineBorderColor)
		{
			chart.Series.Add(new Series(targetItem));
			chart.Series[targetItem].ChartArea = Constant.CHART_CHARTAREANM;
			chart.Series[targetItem].YValueMembers = targetItem;
			chart.Series[targetItem].XValueMember = Constant.CHART_X;
			chart.Series[targetItem].ShadowColor = Color.Black;
			chart.Series[targetItem].ShadowOffset = 2;
			chart.Series[targetItem].BorderWidth = 2;
			chart.Series[targetItem].BorderColor = lineBorderColor;
			chart.Series[targetItem].Color = lineMainColor;
			chart.Series[targetItem].ChartType = chartType;

			if (markerStyle.HasValue)
			{
				chart.Series[targetItem].MarkerStyle = markerStyle.Value;
			}

			if (markerSize.HasValue)
			{
				chart.Series[targetItem].MarkerSize = markerSize.Value;
			}
		}

		public void InitChartLine(Chart chart, string targetItem, Color lineMainColor, Color lineBorderColor)
		{
			InitChart(chart, targetItem, SeriesChartType.Line, null, null, lineMainColor, lineBorderColor);
		}

		public void AddSelectLotHighLight(Chart chart, List<string> targetLotList, SortedList<int, QCLogData> cndDataItem)
		{
			if (chart.DataSource != null)
			{
				DataTable table = (DataTable)chart.DataSource;


				foreach (DataRow row in table.Rows)
				{
					string targetLot = row[Constant.CHART_X].ToString();
					if (targetLotList.Contains(targetLot))
					{
						foreach (KeyValuePair<int, QCLogData> targetLotData in cndDataItem)
						{
							if (targetLotData.Value.LotNO == targetLot)
							{
								row[Constant.CHART_FLAG_LOT] = row[targetLotData.Value.EquiNO + "/" + targetLotData.Value.QcprmNM.Trim()];
								break;
							}
							else
							{
								row[Constant.CHART_FLAG_LOT] = double.NaN;
							}
						}

					}

				}
			}
		}


		/// <summary>
		/// MSCHARTでのグラフ描画
		/// </summary>
		/// <param name="aChart">MSChartのインスタンス</param>
		/// <param name="sModelNM"></param>
		/// <param name="nQcParamNO"></param>
		/// <param name="cndDataItem"></param>
		/// <param name="sManageNm"></param>
		/// <param name="sType"></param>
		/// <param name="ListLot"></param>
		/// <param name="nMode">分割モード</param>
		public void Draw(Chart aChart, string sModelNM, int nQcParamNO, SortedList<int, QCLogData> cndDataItem, string sManageNm, string sType, List<string> ListFlagLot, int nMode)
		{
			string errMsg = string.Empty;
#if MEASURE_TIME
			Console.WriteLine("Draw()開始 / " + DateTime.Now.ToLongTimeString());
#endif
			double dUcl = double.NaN;
			double dLcl = double.NaN;
			double dQUcl = 9999;
			double dQLcl = 9999;
			double dAim = double.NaN;
			double dRate = double.NaN;

			bool innerLowerLineVisible = false;
			bool innerUpperLineVisible = false;

			//装置一覧の取得
			List<string> wEquiNOList = new List<string>();
			List<string> EquiNOList = new List<string>();
			List<int> QcParamNOList = new List<int>();
			List<string> PatternList = new List<string>();
			//List<double> ValueList = new List<double>();

			//外観検査装置データであるか判断
			bool fAI = GetAIQuestion(cndDataItem[0].EquiNO);//True=外観検査機データ

			//<--宿題:月別レポートで躓く箇所なので、処理変更
			//ここを毎回DB接続せずにList<string> wEquiNO= new List<string>();で表記変更「S*****」→「***号機(S*****)」をしたか確認
			//   していないならDB接続を行うように処理変更
			#region CommentOut
			//            for (int i = 0; i < cndDataItem.Count; i++)
			//            {
			//#if MEASURE_TIME
			//                DateTime baseTime = DateTime.Now;
			//#endif
			//                //外観検査データであれば、通ってきたワイヤーボンダー装置へ変換する。
			//                if (fAI == true)
			//                {
			//                    cndDataItem[i].EquiNO = GetWBEquiNo(cndDataItem[i].LotNO, "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ",Constant.nLineCD);
			//                }
			//#if MEASURE_TIME
			//                Console.WriteLine("WB取得(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			//                baseTime = DateTime.Now;
			//#endif
			//                //既に変換済みか?「S*****」→「***号機(S*****)」
			//                if (!wEquiNOList.Contains(cndDataItem[i].EquiNO) && !string.IsNullOrEmpty(cndDataItem[i].EquiNO))
			//                {//No
			//                    wEquiNOList.Add(cndDataItem[i].EquiNO);
			//                    cndDataItem[i].EquiNO = Com.AddCommentEquipmentNO(cndDataItem[i].EquiNO);//表記変更「S*****」→「***号機(S*****)」
			//                    EquiNOList.Add(cndDataItem[i].EquiNO);
			//                }
			//                else
			//                {//Yes
			//                    for (int j = 0; j < EquiNOList.Count; j++)
			//                    {
			//                        if (EquiNOList[j].Contains(cndDataItem[i].EquiNO))
			//                        {
			//                            cndDataItem[i].EquiNO=EquiNOList[j];
			//                            break;
			//                        }
			//                    }

			//                }
			//#if MEASURE_TIME
			//                Console.WriteLine("WB変換(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			//                baseTime = DateTime.Now;
			//#endif
			//                //QcParamNOListは下記のif()で含まれてなければAddするだけで、その他の場所から参照されてないので、ひとまずコメントアウト。問題無ければ削除 2014/12/9
			//                //if (!QcParamNOList.Contains(cndDataItem[i].QcprmNO))
			//                //    QcParamNOList.Add(cndDataItem[i].QcprmNO);

			//                if (!PatternList.Contains(cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim()))
			//                    PatternList.Add(cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim());

			//                //ValueList.Add(cndDataItem[i].Data);
			//#if MEASURE_TIME
			//                Console.WriteLine("1Lot完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);

			//#endif
			//            }
			//#if MEASURE_TIME
			//            Console.WriteLine("DrawGraph() 装置取得完 / " + DateTime.Now.ToLongTimeString());
			//#endif
			#endregion
			//-->


			cndDataItem = GetEquipCodeToNoList(cndDataItem);
			PatternList = GetPatternList(cndDataItem);
			EquiNOList = GetEquipNoList(cndDataItem);



			//装置の並び替え
			PatternList.Sort((x, y) => { return string.Compare(x, y); });
			EquiNOList.Sort((x, y) => { return string.Compare(x, y); });

			//中心線用の値算出
			//double dAvg = ValueList.Average();
			double dAvg = Calc.Average(cndDataItem);

			//上限下限線用の値算出
			string sLimit = Com.GetTmPLM(sModelNM, nQcParamNO, sType);

			Dictionary<int, PlmInfo> plmList = ConnectQCIL.GetPLMData(GetTypeList(cndDataItem), GetQcParamNoList(cndDataItem), sModelNM, false);



			string[] recordArray = sLimit.Split(',');

			double tmpDblVL = double.NaN;
			if (recordArray.Length > 0)
				if (Double.TryParse(recordArray[0], out tmpDblVL))
					dUcl = tmpDblVL;
			double tmpDblVL2 = double.NaN;
			if (recordArray.Length > 1)
				if (Double.TryParse(recordArray[1], out tmpDblVL2))
					dLcl = tmpDblVL2;
			double tmpDblVL3 = double.NaN;
			double tmpDblVL4 = double.NaN;
			if (recordArray.Length > 3)
			{
				if (recordArray[4] == "NULL" || recordArray[5] == "NULL")//閾値の方
				{
					if (Double.TryParse(recordArray[2], out tmpDblVL3))
						dQUcl = tmpDblVL3;
					if (Double.TryParse(recordArray[3], out tmpDblVL4))
						dQLcl = tmpDblVL4;
				}
				else if (recordArray[2] == "NULL" || recordArray[3] == "NULL")//狙い値の方
				{
					if (Double.TryParse(recordArray[4], out tmpDblVL3))
						dAim = tmpDblVL3;
					if (Double.TryParse(recordArray[5], out tmpDblVL4))
						dRate = tmpDblVL4;
					dQUcl = ((dUcl - dAim) * dRate / 100) + dAim;
					dQLcl = ((dLcl - dAim) * dRate / 100) + dAim;
				}
				else
				{
					dQUcl = tmpDblVL3;
					dQLcl = tmpDblVL4;
				}
			}
			//<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
			//string sTmPRMInfo1=Com.GetTmPRMInfo1(nQcParamNO);
			PrmAddInfo wPrmAddInfo = Com.GetTmPRMInfo(nQcParamNO);

			string sMAX, sMIN;

			//規格値であれば「規格上限値/規格下限値」
			//管理値であれば「上限値/下限値」
			if (wPrmAddInfo.Info1.Contains(Constant.CHART_KIKAKU_NM))
			{
				sMAX = Constant.CHART_UCL_NM_KIKAKU;//規格上限値
				sMIN = Constant.CHART_LCL_NM_KIKAKU;//規格下限値
			}
			else
			{
				sMAX = Constant.CHART_UCL_NM;//上限値
				sMIN = Constant.CHART_LCL_NM;//下限値
			}
			//-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima

			//データの容れ物作成
			DataTable table = new DataTable();
			DataColumn col0 = new DataColumn(Constant.CHART_X, typeof(string));
			table.Columns.Add(col0);
			DataColumn col1 = new DataColumn(Constant.CHART_CL_NM, typeof(double));//平均値
			table.Columns.Add(col1);
			//<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
			//DataColumn col2 = new DataColumn(Constant.CHART_UCL_NM, typeof(double));//上限値
			DataColumn col2 = new DataColumn(sMAX, typeof(double));//上限値
			table.Columns.Add(col2);
			//DataColumn col3 = new DataColumn(Constant.CHART_LCL_NM, typeof(double));//下限値
			DataColumn col3 = new DataColumn(sMIN, typeof(double));//下限値
			//-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
			table.Columns.Add(col3);
			DataColumn col4 = new DataColumn(Constant.CHART_EQUINO_ALL, typeof(double));
			table.Columns.Add(col4);
			DataColumn col5 = new DataColumn(Constant.CHART_FLAG_LOT, typeof(double));
			table.Columns.Add(col5);
			//★☆
			DataColumn col6 = new DataColumn(Constant.ENUM_CHART_LineKind.QUCL.ToString(), typeof(double));
			table.Columns.Add(col6);
			DataColumn col7 = new DataColumn(Constant.ENUM_CHART_LineKind.QLCL.ToString(), typeof(double));
			table.Columns.Add(col7);


			//<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
			DataColumn col8 = new DataColumn(wPrmAddInfo.Info2, typeof(double));
			table.Columns.Add(col8);
			//-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima

			//工程狙い値
			DataColumn col9 = new DataColumn(Constant.CHART_INNER_UPPER_LIMIT_NM, typeof(double));
			table.Columns.Add(col9);
			DataColumn col10 = new DataColumn(Constant.CHART_INNER_LOWER_LIMIT_NM, typeof(double));
			table.Columns.Add(col10);


			//for (int i = 0; i < EquiNOList.Count; i++)
			//{
			//    DataColumn colN = new DataColumn(EquiNOList[i], typeof(double));
			//    table.Columns.Add(colN);
			//}

			for (int i = 0; i < PatternList.Count; i++)
			{
				DataColumn colN = new DataColumn(PatternList[i], typeof(double));
				table.Columns.Add(colN);
			}
			//データ格納
			DataRow row = null;
			string tmpLot = "";
			int tmpLot_CT = 0;

			// ロット毎のループ
			for (int i = 0; i < cndDataItem.Count; i++)
			{
				if (row != null)
				{
					if (tmpLot != cndDataItem[i].LotNO)
					{
						table.Rows.Add(row);
						row = table.NewRow();
						tmpLot_CT = 0;
					}
				}
				else
				{
					row = table.NewRow();
				}

				tmpLot = cndDataItem[i].LotNO;
				row[Constant.CHART_X] = cndDataItem[i].LotNO;
				row[Constant.CHART_CL_NM] = dAvg;
				//<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
				//row[Constant.CHART_UCL_NM] = dUcl;
				//row[Constant.CHART_LCL_NM] = dLcl;
				row[sMAX] = dUcl;
				row[sMIN] = dLcl;
				//-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
				//★☆
				row[Constant.ENUM_CHART_LineKind.QUCL.ToString()] = dQUcl;
				row[Constant.ENUM_CHART_LineKind.QLCL.ToString()] = dQLcl;
				row[Constant.CHART_EQUINO_ALL] = cndDataItem[i].Data;
				//row[Constant.CHART_FLAG_LOT] = (ListFlagLot.Contains(cndDataItem[i].LotNO)) ? cndDataItem[i].Data : double.NaN;

				if (plmList.Count == 0)
				{
					//errMsg = string.Format("閾値が設定されていません。マスタ担当者に連絡して下さい。\r\nタイプ：{0}　管理番号:{1}　管理項目：{2}", cndDataItem[i].TypeCD, cndDataItem[i].QcprmNO, cndDataItem[i].QcprmNM);
				}
				else
				{
					//工程狙い値
					if (plmList[cndDataItem[i].QcprmNO].InnerUpperLimit.HasValue && plmList[cndDataItem[i].QcprmNO].InnerUpperLimit.Value > 0)
					{
						row[Constant.CHART_INNER_UPPER_LIMIT_NM] = plmList[cndDataItem[i].QcprmNO].InnerUpperLimit.Value;
						innerUpperLineVisible = true;
					}
					else
					{
						row[Constant.CHART_INNER_UPPER_LIMIT_NM] = double.NaN;
					}

					if (plmList[cndDataItem[i].QcprmNO].InnerLowerLimit.HasValue && plmList[cndDataItem[i].QcprmNO].InnerLowerLimit.Value > 0)
					{
						row[Constant.CHART_INNER_LOWER_LIMIT_NM] = plmList[cndDataItem[i].QcprmNO].InnerLowerLimit.Value;
						innerLowerLineVisible = true;
					}
					else
					{
						row[Constant.CHART_INNER_LOWER_LIMIT_NM] = double.NaN;
					}

					//row[Constant.CHART_INNER_UPPER_LIMIT_NM] = 

					//for (int j = 0; j < EquiNOList.Count; j++)
					//{
					//    row[EquiNOList[j]] = (cndDataItem[i].EquiNO == EquiNOList[j]) ? cndDataItem[i].Data : double.NaN;
					//}

					//設備とパラメータの組み合わせ毎のループ
					for (int j = 0; j < PatternList.Count; j++)
					{
						if (tmpLot_CT == 0)
							row[PatternList[j]] = (cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim() == PatternList[j]) ? cndDataItem[i].Data : double.NaN;
						else
							row[PatternList[j]] = (cndDataItem[i].EquiNO + "/" + cndDataItem[i].QcprmNM.Trim() == PatternList[j]) ? cndDataItem[i].Data : row[PatternList[j]];
					}
					tmpLot_CT++;

					if (tmpLot_CT == PatternList.Count)
					{
						tmpLot = string.Empty;
					}
				}

			}

			if (row != null)
			{
				table.Rows.Add(row);
			}

			//if (errMsg != string.Empty)
			//{
			//    MessageBox.Show(errMsg);
			//}

			//グラフ色取得
			List<Color> colorList = GetColorList();

			//グラフ描画
			aChart.Series.Clear();
			aChart.Titles.Clear();

			//凡例に表示する系列が多い場合の為、グラフ範囲いっぱいに表示できるように設定しておく。
			//aChart.Legends[0].MaximumAutoSize = 100;

			//aChart.Legends[0].AutoFitMinFontSize = 6;
			//aChart.Legends[0].Alignment = StringAlignment.Near;

			//aChart.Legends[0].IsTextAutoFit = true;
			//aChart.Legends[0].Position.Auto = false;
			//aChart.Legends[0].Position.X = 70;
			//aChart.Legends[0].Position.Y = 15;

			//string sInspectionNM = cndDataItem[0].QcprmNM;
			string sInspectionNM = Com.GetInspectionNM(cndDataItem[0].QcprmNO);

			if (sInspectionNM.Substring(0, 1) == "F")
			{
				sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
			}

			aChart.Titles.Add(new Title(sInspectionNM));

			//0始まりのグラフで無くなる
			aChart.ChartAreas[Constant.CHART_CHARTAREANM].AxisY.IsStartedFromZero = false;

			aChart.ChartAreas[Constant.CHART_CHARTAREANM].AxisX.IntervalType = DateTimeIntervalType.Number;
			aChart.ChartAreas[Constant.CHART_CHARTAREANM].AxisX.Interval = 1;
			aChart.ChartAreas[Constant.CHART_CHARTAREANM].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

			aChart.DataSource = table;

			AddSelectLotHighLight(aChart, ListFlagLot, cndDataItem);

			InitChart(aChart, wPrmAddInfo.Info2, SeriesChartType.Bubble, null, null, Color.Red, Color.Red);

			InitChartLine(aChart, Constant.CHART_CL_NM, Color.Green, Color.Green);

			if (sManageNm != Constant.sOther)
			{
				//★☆ QUCL,QLCLが入っていたらUCL,LCLは表示しない
				if ((dQUcl != 9999) && (dQLcl != 9999))
				{
					InitChartLine(aChart, Constant.ENUM_CHART_LineKind.QUCL.ToString(), Color.SandyBrown, Color.SandyBrown);
				}
				else
				{
					InitChartLine(aChart, sMAX, Color.Red, Color.Red);
				}

				if (sManageNm != Constant.sMax)
				{
					//★☆QUCL,QLCLが入っていたらUCL,LCLは表示しない
					if ((dQUcl != 9999) && (dQLcl != 9999))
					{
						InitChartLine(aChart, Constant.ENUM_CHART_LineKind.QLCL.ToString(), Color.SandyBrown, Color.SandyBrown);
					}
					else
					{
						InitChartLine(aChart, sMIN, Color.HotPink, Color.HotPink);
					}
				}

				//工程狙い値
				if (innerUpperLineVisible)
				{
					InitChartLine(aChart, Constant.CHART_INNER_UPPER_LIMIT_NM, Color.Blue, Color.Blue);
				}

				if (innerLowerLineVisible)
				{
					InitChartLine(aChart, Constant.CHART_INNER_LOWER_LIMIT_NM, Color.Blue, Color.Blue);
				}

			}
			if (nMode == 0)
			{
				InitChart(aChart, Constant.CHART_EQUINO_ALL, SeriesChartType.Line, MarkerStyle.Circle, null, colorList[0], colorList[0]);

				// 空のデータポイントを表示する位置を指定する
				aChart.Series[Constant.CHART_EQUINO_ALL]["EmptyPointValue"] = "Average";
				// 空のデータポイントの表示方法を指定する
				aChart.Series[Constant.CHART_EQUINO_ALL].EmptyPointStyle.Color = colorList[0];
				aChart.Series[Constant.CHART_EQUINO_ALL].EmptyPointStyle.BorderColor = colorList[0];
				aChart.Series[Constant.CHART_EQUINO_ALL].EmptyPointStyle.BorderWidth = 2;
				aChart.Series[Constant.CHART_EQUINO_ALL].EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;

				aChart.Series[Constant.CHART_EQUINO_ALL].EmptyPointStyle.MarkerStyle = MarkerStyle.None;
			}
			else
			{
				for (int k = 0; k < PatternList.Count; k++)
				{
					string nm = PatternList[k];

					InitChart(aChart, nm, SeriesChartType.Line, MarkerStyle.Circle, null, colorList[k], colorList[k]);

					// 空のデータポイントを表示する位置を指定する
					aChart.Series[nm]["EmptyPointValue"] = Constant.ENUM_CHART_EmptyPointStyleCustom.Average.ToString();

					// 空のデータポイントの表示方法を指定する
					aChart.Series[nm].EmptyPointStyle.Color = colorList[k];
					aChart.Series[nm].EmptyPointStyle.BorderColor = colorList[k];
					aChart.Series[nm].EmptyPointStyle.BorderWidth = 2;
					aChart.Series[nm].EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;

					aChart.Series[nm].EmptyPointStyle.MarkerStyle = MarkerStyle.None;
				}
			}

			InitChart(aChart, Constant.CHART_FLAG_LOT, SeriesChartType.Point, MarkerStyle.Star5, 15, Color.Red, Color.Red);

		}

		public bool GetAIQuestion(string sEquiNO)
		{
			bool flg = false;

			string sAssets = "";
			string BaseSql = "SELECT [Assets_NM] FROM [TvLSET] WITH(NOLOCK) WHERE Equipment_NO='{0}'";
			string sqlCmdTxt = string.Format(BaseSql, sEquiNO.Trim());

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						sAssets = Convert.ToString(reader["Assets_NM"]);
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}

			if (sAssets == "外観検査機")
			{
				flg = true;
			}

			return flg;
		}

		public string GetWBEquiNo(string sLotNO, string sAssetsNM, int nlinecd)
		{
			string sWBEquiNO = "";

			//閾値マスタにあるTypeのみ表示
			string BaseSql = "SELECT [Equipment_NO] FROM [TnLOTT] WITH(NOLOCK) WHERE [NascaLot_NO]='{0}' AND [Assets_NM]='{1}' ";// AND Inline_CD={2}";
			string sqlCmdTxt = string.Format(BaseSql, sLotNO.Trim(), sAssetsNM.Trim());//, nlinecd);

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						sWBEquiNO = Convert.ToString(reader["Equipment_NO"]);
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			//TnLOTTからWB装置が取得出来なかった場合、ARMSのTnTranから情報取得
			if (string.IsNullOrEmpty(sWBEquiNO))
			{
				System.Globalization.CompareInfo ci = System.Globalization.CultureInfo.CurrentCulture.CompareInfo;
				List<LotStbInfo> lotInfo = ConnectARMS.SetTblLotStb_ARMS(sLotNO, sAssetsNM);

				if (lotInfo.Count == 0)
				{
					throw new ApplicationException(
						string.Format("TnLOTT,TnTranの何れからも、ﾃﾞｰﾀ取得出来ませんでした。lot:{0} 装置種:{1}", sLotNO, sAssetsNM));
				}

				sWBEquiNO = lotInfo.Find(l => ci.Compare(l.PlantClasNM, sAssetsNM, System.Globalization.CompareOptions.IgnoreWidth) == 0).PlantCD;
			}

			return sWBEquiNO;
		}

		//中心線(CL),上方管理限界(UCL),下方管理限界(LCL)→実線を引く為のｾｯﾃｨﾝｸﾞ関数
		//引数　線を引く所(値)
		//戻り値 ﾘｽﾄ
		public List<double> SettingSolidLineDraw(double dvalue, int nCount)
		{
			List<double> ds = new List<double>();
			for (int i = 0; i < nCount + 1; i++)
			{
				ds.Add(dvalue);
			}
			return ds;
		}

		/// <summary>
		/// グラフの色のリストを取得
		/// </summary>
		/// <returns></returns>
		public List<Color> GetColorList()
		{
			List<Color> retV = new List<Color>();
			//retV.Add(Color.AliceBlue);
			//retV.Add(Color.AntiqueWhite);
			retV.Add(Color.Black);
			retV.Add(Color.Aqua);
			//retV.Add(Color.Aquamarine);
			retV.Add(Color.Azure);
			//retV.Add(Color.Beige);
			//retV.Add(Color.Bisque);
			//retV.Add(Color.Black);
			retV.Add(Color.BlanchedAlmond);
			//retV.Add(Color.Blue);
			//retV.Add(Color.BlueViolet);
			retV.Add(Color.Brown);
			//retV.Add(Color.BurlyWood);
			//retV.Add(Color.CadetBlue);
			retV.Add(Color.Chartreuse);
			//retV.Add(Color.Chocolate);
			retV.Add(Color.Coral);
			retV.Add(Color.CornflowerBlue);
			//retV.Add(Color.Cornsilk);
			retV.Add(Color.Crimson);
			retV.Add(Color.Cyan);
			retV.Add(Color.DarkBlue);
			retV.Add(Color.DarkCyan);
			retV.Add(Color.DarkGoldenrod);
			retV.Add(Color.DarkGray);
			retV.Add(Color.DarkGreen);
			retV.Add(Color.DarkKhaki);
			retV.Add(Color.DarkMagenta);
			retV.Add(Color.DarkOliveGreen);
			retV.Add(Color.DarkOrange);
			retV.Add(Color.DarkOrchid);
			retV.Add(Color.DarkRed);
			retV.Add(Color.DarkSalmon);
			retV.Add(Color.DarkSeaGreen);
			retV.Add(Color.DarkSlateBlue);
			retV.Add(Color.DarkSlateGray);
			retV.Add(Color.DarkTurquoise);
			retV.Add(Color.DarkViolet);
			retV.Add(Color.DeepPink);
			retV.Add(Color.DeepSkyBlue);
			retV.Add(Color.DimGray);
			retV.Add(Color.DodgerBlue);
			//retV.Add(Color.Empty);
			retV.Add(Color.Firebrick);
			retV.Add(Color.FloralWhite);
			retV.Add(Color.ForestGreen);
			retV.Add(Color.Fuchsia);
			retV.Add(Color.Gainsboro);
			//retV.Add(Color.GhostWhite);
			retV.Add(Color.Gold);
			retV.Add(Color.Goldenrod);
			retV.Add(Color.Gray);
			//retV.Add(Color.Green);
			retV.Add(Color.GreenYellow);
			retV.Add(Color.Honeydew);
			//retV.Add(Color.HotPink);
			retV.Add(Color.IndianRed);
			retV.Add(Color.Indigo);
			retV.Add(Color.Ivory);
			retV.Add(Color.Khaki);
			retV.Add(Color.Lavender);
			retV.Add(Color.LavenderBlush);
			retV.Add(Color.LawnGreen);
			retV.Add(Color.LemonChiffon);
			retV.Add(Color.LightBlue);
			retV.Add(Color.LightCoral);
			retV.Add(Color.LightCyan);
			retV.Add(Color.LightGoldenrodYellow);
			retV.Add(Color.LightGray);
			retV.Add(Color.LightGreen);
			retV.Add(Color.LightPink);
			retV.Add(Color.LightSalmon);
			retV.Add(Color.LightSeaGreen);
			retV.Add(Color.LightSkyBlue);
			retV.Add(Color.LightSlateGray);
			retV.Add(Color.LightSteelBlue);
			retV.Add(Color.LightYellow);
			retV.Add(Color.Lime);
			retV.Add(Color.LimeGreen);
			retV.Add(Color.Linen);
			retV.Add(Color.Magenta);
			retV.Add(Color.Maroon);
			retV.Add(Color.MediumAquamarine);
			retV.Add(Color.MediumBlue);
			retV.Add(Color.MediumOrchid);
			retV.Add(Color.MediumPurple);
			retV.Add(Color.MediumSeaGreen);
			retV.Add(Color.MediumSlateBlue);
			retV.Add(Color.MediumSpringGreen);
			retV.Add(Color.MediumTurquoise);
			retV.Add(Color.MediumVioletRed);
			retV.Add(Color.MidnightBlue);
			//retV.Add(Color.MintCream);
			retV.Add(Color.MistyRose);
			retV.Add(Color.Moccasin);
			//retV.Add(Color.NavajoWhite);
			retV.Add(Color.Navy);
			retV.Add(Color.OldLace);
			retV.Add(Color.Olive);
			retV.Add(Color.OliveDrab);
			retV.Add(Color.Orange);
			retV.Add(Color.OrangeRed);
			retV.Add(Color.Orchid);
			retV.Add(Color.PaleGoldenrod);
			retV.Add(Color.PaleGreen);
			retV.Add(Color.PaleTurquoise);
			retV.Add(Color.PaleVioletRed);
			retV.Add(Color.PapayaWhip);
			retV.Add(Color.PeachPuff);
			retV.Add(Color.Peru);
			retV.Add(Color.Pink);
			retV.Add(Color.Plum);
			retV.Add(Color.PowderBlue);
			retV.Add(Color.Purple);
			retV.Add(Color.Red);
			retV.Add(Color.RosyBrown);
			retV.Add(Color.RoyalBlue);
			retV.Add(Color.SaddleBrown);
			retV.Add(Color.Salmon);
			retV.Add(Color.SandyBrown);
			retV.Add(Color.SeaGreen);
			retV.Add(Color.SeaShell);
			retV.Add(Color.Sienna);
			retV.Add(Color.Silver);
			retV.Add(Color.SkyBlue);
			retV.Add(Color.SlateBlue);
			retV.Add(Color.SlateGray);
			//retV.Add(Color.Snow);
			retV.Add(Color.SpringGreen);
			retV.Add(Color.SteelBlue);
			retV.Add(Color.Tan);
			retV.Add(Color.Teal);
			retV.Add(Color.Thistle);
			retV.Add(Color.Tomato);
			retV.Add(Color.Transparent);
			retV.Add(Color.Turquoise);
			retV.Add(Color.Violet);
			retV.Add(Color.Wheat);
			//retV.Add(Color.White);
			//retV.Add(Color.WhiteSmoke);
			retV.Add(Color.Yellow);
			retV.Add(Color.YellowGreen);

			return retV;
		}
	}
}
