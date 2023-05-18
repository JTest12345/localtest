/*************************************************************************************
 * システム名     : 保全システム
 *  
 * 処理名         : ExcelReport　エクセル出力
 * 
 * 概略           : エクセル出力関連の処理を行う
 * 
 * 作成           : 2006/11/14 SLA.Uchida
 * 
 * 修正履歴       : 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
 *                  2018/04/17 SLA2内田   macgroupとstockerは無条件に文字型とする。
 *                  2018/11/08 SLA2内田   マッピング解析機能追加
 ************************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Reflection;
using Excel;

namespace FVDC
{
	/// <summary>
	/// Excelを用いる帳票の基底クラス
	/// </summary>
	///
	public class ExcelReport : IDisposable
	{
        public ExcelAdapter excel;

		public ExcelReport()
		{
		}

		~ExcelReport()
		{
			this.Dispose();
		}
		public void Dispose()
		{
			if(this.excel != null)
			{
				excel.Dispose();
			}			
		}
		
		/// <summary>
		/// 指定されたエクセルレポートを開く
		/// </summary>
		/// <param name="prmfileName"></param>
		/// <returns>成功時0</returns>
		public bool OpenReport(string prmfileName)
		{
			try
			{
				if ( excel == null )
				{
					excel				= new ExcelAdapter();
				}

				object objMissing		= System.Reflection.Missing.Value;
				excel.Book				= excel.App.Workbooks.Open(
					prmfileName, objMissing, objMissing, objMissing, objMissing,
					objMissing, objMissing, objMissing, objMissing, objMissing,
					objMissing, objMissing, objMissing);

				excel.App.Visible		= true;
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 与えられたDataTableをそのままExcelに吐き出す
		/// </summary>
		/// <param name="dt">Excelに出力するDataTable</param>
		/// <returns>成功時0</returns>
		public int MakePlaneReport(System.Data.DataTable dt)
		{
			if(excel == null)
			{
				excel = new ExcelAdapter();
			}

		
			excel.Book = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet = (Worksheet)excel.Book.Worksheets[1];

			object missing = System.Reflection.Missing.Value;

			if(dt == null)
			{
				throw new ApplicationException("Data object not found.", null);
			}

			for(int i = 0; i <= dt.Columns.Count - 1; i++)
			{
				excel.Cell(1, i + 1).Value2 = dt.Columns[i].ColumnName;
			}

			for(int i = 0; i <= dt.Rows.Count - 1; i++)
			{
				for(int j = 0; j <= dt.Columns.Count - 1; j++)
				{
					excel.Cell(i + 2 , j + 1).Value2 = dt.Rows[i][j].ToString();
				}
			}

			excel.App.Visible = true;
			return 0;
		}
        
		/// <summary>
		/// 与えられたdataGridViewコレクションのキャプションを漢字に設定している列のみExcelに吐き出す
		/// </summary>
		/// <param name="dt">Excelに出力するUltraGridコレクション</param>
		/// <returns>成功時0</returns>
		public int MakeReport(DataGridView dataGridView, NotifyStatusHandler notifyHandler)
		{
			bool canNotify							= false;
			if ( notifyHandler != null ) canNotify	= true;
			if ( canNotify ) notifyHandler(0, "　エ ク セ ル 出 力 を 開 始 し ま し た 。");

			if(dataGridView.Rows.Count == 0)
			{
				return 0;
			}

			if(excel == null)
			{
				excel       = new ExcelAdapter();
			}
		
			excel.Book      = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet     = (Worksheet)excel.Book.Worksheets[1];

			object missing  = System.Reflection.Missing.Value;

            /// 領域確保
			object[,] objItem						= new object[dataGridView.Rows.Count + 1, dataGridView.Columns.Count];
            
            /// 見出し設定
			int		SetIx							= 0;
			for(int i = 0; i < dataGridView.Columns.Count; i++)
			{
                if (dataGridView.Columns[i].Visible)
				{
					objItem[0, SetIx]				= dataGridView.Columns[i].HeaderText;
					SetIx++;
				}
			}
            
			if ( canNotify ) notifyHandler(0, "　デ　ー　タ　編　集　中");

            /// 詳細データ設定
			long ix			= 1;
			for(int i = 0; i < dataGridView.Rows.Count; i++)
			{
				if ((dataGridView.Rows[i].Visible)
                    && (dataGridView[0, i].Value != null))
				{
					SetIx							= 0;
					for(int j = 0; j < dataGridView.Columns.Count; j++)
					{
                        if ( dataGridView.Columns [j].DataPropertyName == "Status_NM" )
                        {
							objItem[ix, SetIx - 1]			= dataGridView [j, i].Value;
                        }
                        else if (dataGridView.Columns[j].Visible)
						{
							try
							{
								switch ( dataGridView.Columns [j].ValueType.Name )
								{
									case "Boolean":
										if ( (bool)dataGridView [j, i].Value )
										{
											objItem [ix, SetIx]     = "○";
										}
										break;
									case "DateTime":
										objItem [ix, SetIx]         = "'" + Convert.ToDateTime(dataGridView [j, i].Value).ToString("yyyy/MM/dd HH:mm:ss");
										break;
									default:
                                        if (dataGridView.Columns[j].DataPropertyName.Contains("_DT"))
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;
                                        }
                                        else if (dataGridView.Columns[j].DataPropertyName.Contains("シリアル№"))   /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;               /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        }
                                        else
                                        {
										    objItem [ix, SetIx]     = dataGridView [j, i].FormattedValue;
                                            string[] sptItem        = objItem [ix, SetIx].ToString().Split('\n');
                                            if (sptItem.Length > 30)
                                            {
                                                objItem[ix, SetIx]  = objItem [ix, SetIx].ToString().Replace("\r\n"," , ");
                                            }
                                            if (objItem[ix, SetIx].ToString().Length > 900)
                                            {
                                                objItem[ix, SetIx]  = objItem[ix, SetIx].ToString().Substring(0, 900);
                                            }
                                        }
										break;
								}
							}
							catch
							{
							}
							SetIx++;
						}
					}
					ix++;
				}
				if ( canNotify ) notifyHandler(100 * i / dataGridView.Rows.Count, null);
			}
			Range			range;
			range									= excel.Sheet.get_Range("A1", Missing.Value);
			range									= range.get_Resize(ix, SetIx);
			range.Value2							= objItem;
			/// 自動サイズ調整
			range.Columns.EntireColumn.AutoFit();
			range.Rows.EntireRow.AutoFit();
			/// フィルター設定
			range.AutoFilter(1, Missing.Value, XlAutoFilterOperator.xlAnd, Missing.Value, Missing.Value);
			/// ウインドウ枠の固定
			range									= excel.Sheet.get_Range("A2", Missing.Value);
			range.Activate();
			excel.App.ActiveWindow.FreezePanes		= true;

			excel.App.Visible = true;
			return 0;
		}

		/// <summary>
		/// 与えられたdataGridViewコレクションのキャプションを漢字に設定している列のみExcelに吐き出す
		/// </summary>
		/// <param name="dt">Excelに出力するUltraGridコレクション</param>
		/// <returns>成功時0</returns>
		public int MakeCaptionReport(DataGridView dataGridView, NotifyStatusHandler notifyHandler)
		{
			bool canNotify							= false;
			if ( notifyHandler != null ) canNotify	= true;
			if ( canNotify ) notifyHandler(0, "　エ ク セ ル 出 力 を 開 始 し ま し た 。");

			if(dataGridView.Rows.Count == 0)
			{
				return 0;
			}

			if(excel == null)
			{
				excel = new ExcelAdapter();
			}
		
			excel.Book = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet = (Worksheet)excel.Book.Worksheets[1];

			object missing = System.Reflection.Missing.Value;

            /// 領域確保
			object[,] objItem						= new object[dataGridView.Rows.Count + 1, dataGridView.Columns.Count];
            
            /// 見出し設定
			int		SetIx							= 0;
			for(int i = 0; i < dataGridView.Columns.Count; i++)
			{
                if ((dataGridView.Columns[i].DataPropertyName != dataGridView.Columns[i].HeaderText)
                    && (dataGridView.Columns[i].HeaderText.Trim() != "")
                    && (dataGridView.Columns[i].Visible))
				{
					objItem[0, SetIx]				= dataGridView.Columns[i].HeaderText;
					SetIx++;
				}
			}
            
			if ( canNotify ) notifyHandler(0, "　デ　ー　タ　編　集　中");

            /// 詳細データ設定
			long ix			= 1;
			for(int i = 0; i < dataGridView.Rows.Count; i++)
			{
				if (dataGridView.Rows[i].Visible)
				{
					SetIx							= 0;
					for(int j = 0; j < dataGridView.Columns.Count; j++)
					{
                        if ( dataGridView.Columns [j].DataPropertyName == "Status_NM" )
                        {
							objItem[ix, SetIx - 1]			= dataGridView [j, i].Value;
                        }
                        else if ((dataGridView.Columns[j].DataPropertyName != dataGridView.Columns[j].HeaderText)
                            && (dataGridView.Columns[j].HeaderText.Trim() != "")
                            && (dataGridView.Columns[j].Visible))
						{
							try
							{
								switch ( dataGridView.Columns [j].ValueType.Name )
								{
									case "Boolean":
										if ( (bool)dataGridView [j, i].Value )
										{
											objItem [ix, SetIx]     = "○";
										}
										break;
									case "DateTime":
										objItem [ix, SetIx]         = "'" + Convert.ToDateTime(dataGridView [j, i].Value).ToString("yyyy/MM/dd HH:mm:ss");
										break;
									default:
                                        if (dataGridView.Columns[j].DataPropertyName.Contains("_DT"))
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;
                                        }
                                        else if (dataGridView.Columns[j].DataPropertyName.Contains("シリアル№"))   /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;               /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        }
                                        else
                                        {
										    objItem [ix, SetIx]     = dataGridView [j, i].FormattedValue;
                                            string[] sptItem        = objItem [ix, SetIx].ToString().Split('\n');
                                            if (sptItem.Length > 30)
                                            {
                                                objItem[ix, SetIx]  = objItem [ix, SetIx].ToString().Replace("\r\n"," , ");
                                            }
                                            if (objItem[ix, SetIx].ToString().Length > 900)
                                            {
                                                objItem[ix, SetIx]  = objItem[ix, SetIx].ToString().Substring(0, 900);
                                            }
                                        }
										break;
								}
							}
							catch
							{
							}
							SetIx++;
						}
					}
					ix++;
				}
				if ( canNotify ) notifyHandler(100 * i / dataGridView.Rows.Count, null);
			}
			Range			range;
			range									= excel.Sheet.get_Range("A1", Missing.Value);
			range									= range.get_Resize(ix, SetIx);
			range.Value2							= objItem;
			/// 自動サイズ調整
			range.Columns.EntireColumn.AutoFit();
			range.Rows.EntireRow.AutoFit();
			/// フィルター設定
			range.AutoFilter(1, Missing.Value, XlAutoFilterOperator.xlAnd, Missing.Value, Missing.Value);
			/// ウインドウ枠の固定
			range									= excel.Sheet.get_Range("A2", Missing.Value);
			range.Activate();
			excel.App.ActiveWindow.FreezePanes		= true;

			excel.App.Visible = true;
			return 0;
		}

		/// <summary>
		/// 与えられたDataTableコレクションのキャプションを漢字に設定している列のみExcelに吐き出す
		/// </summary>
		/// <param name="dt">Excelに出力するDataTableコレクション</param>
		/// <returns>成功時0</returns>
		public int MakeCaptionReport(System.Data.DataTable dt)
		{
			if(dt.Rows.Count == 0)
			{
				return 0;
			}
			if(excel == null)
			{
				excel                               = new ExcelAdapter();
			}
		
			excel.Book                              = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet                             = (Worksheet)excel.Book.Worksheets[1];

			object missing                          = System.Reflection.Missing.Value;
            
            /// 領域確保
			object[,] objItem						= new object[dt.Rows.Count + 1, dt.Columns.Count];
            
            /// 見出し設定
			int		SetIx							= 0;
			for(int i = 0; i <= dt.Columns.Count - 1; i++)
			{
				if ((dt.Columns[i].ColumnName != dt.Columns[i].Caption)
					&& (dt.Columns[i].Caption.Trim() != ""))
				{
					objItem[0, SetIx]				= dt.Columns[i].Caption;
					SetIx++;
				}
			}
            
            /// 詳細データ設定
			long ix			= 1;
            try
            {
			    for(int i = 0; i <= dt.Rows.Count - 1; i++)
			    {
				    SetIx								= 0;
				    for(int j = 0; j <= dt.Columns.Count - 1; j++)
				    {
					    if ( ( dt.Columns [j].ColumnName != dt.Columns [j].Caption )
						    && ( dt.Columns [j].Caption.Trim() != "" ) )
					    {
						    try
						    {
							    switch ( dt.Columns [j].DataType.Name )
							    {
								    case "Boolean":
									    if ( Convert.ToBoolean(dt.Rows [i] [j]) )
									    {
										    objItem [ix, SetIx]         = "○";
									    }
									    break;
								    case "DateTime":
									    objItem [ix, SetIx]             = "'" + Convert.ToDateTime(dt.Rows [i] [j]).ToString("yyyy/MM/dd HH:mm:ss");
									    break;
								    default:
                                        if ( dt.Columns [j].ColumnName.Contains("_DT"))
                                        {
									        objItem [ix, SetIx]         = "'" + dt.Rows [i] [j].ToString();
                                        }
                                        else if ( dt.Columns[j].ColumnName.Contains("シリアル№"))               /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        {
                                            objItem[ix, SetIx]          = "'" + dt.Rows [i] [j].ToString();     /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        }
                                        else
                                        {
                                            string[] sptItem = dt.Rows[i][j].ToString().Split('\n');
                                            if (sptItem.Length > 30)
                                            {
                                                objItem[ix, SetIx] = dt.Rows[i][j].ToString().Replace("\r\n", " , ");
                                            }
                                            else
                                            {
                                                objItem[ix, SetIx] = dt.Rows[i][j].ToString();
                                            }
                                            if (objItem[ix, SetIx].ToString().Length > 900)
                                            {
                                                objItem[ix, SetIx] = objItem[ix, SetIx].ToString().Substring(0, 900);
                                            }
                                        }
									    break;
							    }
						    }
						    catch
						    {
						    }
						    SetIx++;
					    }
				    }
				    ix++;
			    }
			    Range			range;
			    range									= excel.Sheet.get_Range("A1", Missing.Value);
			    range									= range.get_Resize(ix, SetIx);
			    range.Value2							= objItem;

			    /// 自動サイズ調整
			    range.Columns.EntireColumn.AutoFit();
			    range.Rows.EntireRow.AutoFit();
			    /// フィルター設定
			    range.AutoFilter(1, Missing.Value, XlAutoFilterOperator.xlAnd, Missing.Value, Missing.Value);
			    /// ウインドウ枠の固定
			    range									= excel.Sheet.get_Range("A2", Missing.Value);
			    range.Activate();
			    excel.App.ActiveWindow.FreezePanes		= true;

			    excel.App.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("対象データが出力上限を変えましたので\n\n検索条件を追加して再度出力願います。", "検索エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                excel.App.DisplayAlerts                 = false;
            }
			return 0;
		}

        /// <summary>
        /// dataGridViewコレクションを連続してExcelに吐き出す
        /// </summary>
        /// <param name="dt">Excelに出力するUltraGridコレクション</param>
        /// <returns>成功時0</returns>
        public long MakeNextReport(DataGridView dataGridView, string Title, long SetRow)
        {
            if (dataGridView.Rows.Count == 0)
            {
                return SetRow - 1;
            }
                        
            object missing      = System.Reflection.Missing.Value;

            /// 領域確保
            object[,] objItem   = new object[dataGridView.Rows.Count + 1, dataGridView.Columns.Count];

            /// 見出し設定
            int SetIx = 0;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if ((dataGridView.Columns[i].HeaderText.Trim() != "")
                    && (dataGridView.Columns[i].Visible))
                {
                    objItem[0, SetIx] = dataGridView.Columns[i].HeaderText;
                    SetIx++;
                }
            }
            
            /// 詳細データ設定
            long ix                                                 = 1;
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].Visible)
                {
                    SetIx = 0;
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        if (dataGridView.Columns[j].DataPropertyName == "Status_NM")
                        {
                            objItem[ix, SetIx - 1]                  = dataGridView[j, i].Value;
                        }
                        else if (dataGridView[j, i].Value == null)
                        {
                            objItem[ix, SetIx - 1]                  = "NULL";
                        }
                        else if ((dataGridView.Columns[j].HeaderText.Trim() != "")
                                && (dataGridView.Columns[j].Visible))
                        {
                            try
                            {
                                switch (dataGridView.Columns[j].ValueType.Name)
                                {
                                    case "Boolean":
                                        if ((bool)dataGridView[j, i].Value)
                                        {
                                            objItem[ix, SetIx]      = "TRUE";
                                        }
                                        else
                                        {
                                            objItem[ix, SetIx]      = "FALSE";
                                        }
                                        break;
                                    case "DateTime":
                                        objItem[ix, SetIx]          = "'" + Convert.ToDateTime(dataGridView[j, i].Value).ToString("yyyy/MM/dd HH:mm:ss");
                                        break;
                                    default:
                                        if (dataGridView[j, i].Value.ToString() == "NULL")
                                        {
                                            objItem[ix, SetIx]      = dataGridView[j, i].Value;
                                        }
                                        else if (dataGridView.Columns[j].DataPropertyName.Contains("_DT"))
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;
                                        }
                                        else if (dataGridView.Columns[j].DataPropertyName.Contains("シリアル№"))   /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;               /// 2016/02/16 SLA2内田   シリアル№は無条件に文字型とする。
                                        }
                                        else if ((dataGridView.Columns[j].DataPropertyName.Contains("macgroup"))    /// 2018/04/17 SLA2内田   macgroupとstockerは無条件に文字型とする。
                                                || (dataGridView.Columns[j].DataPropertyName.Contains("stocker")))  /// 2018/04/17 SLA2内田   macgroupとstockerは無条件に文字型とする。
                                        {
                                            objItem[ix, SetIx]      = "'" + dataGridView[j, i].Value;               /// 2018/04/17 SLA2内田   macgroupとstockerは無条件に文字型とする。
                                        }
                                        else
                                        {
                                            objItem[ix, SetIx]      = dataGridView[j, i].FormattedValue;
                                            string[] sptItem        = objItem[ix, SetIx].ToString().Split('\n');
                                            if (sptItem.Length > 30)
                                            {
                                                objItem[ix, SetIx]  = objItem[ix, SetIx].ToString().Replace("\r\n", " , ");
                                            }
                                            if (objItem[ix, SetIx].ToString().Length > 900)
                                            {
                                                objItem[ix, SetIx]  = objItem[ix, SetIx].ToString().Substring(0, 900);
                                            }
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                            }
                            SetIx++;
                        }
                    }
                    ix++;
                }
            }
            Range range;
            range               = excel.Sheet.get_Range("A" + SetRow.ToString(), Missing.Value);
            range               = range.get_Resize(1, 1);
            range.Value2        = Title;
            SetRow              = SetRow + 1;
            range               = excel.Sheet.get_Range("A" + SetRow.ToString(), Missing.Value);
            range               = range.get_Resize(ix, SetIx);
            range.Value2        = objItem;

            /// 罫線設定
            range.Borders[Excel.XlBordersIndex.xlDiagonalDown].LineStyle        = Excel.XlLineStyle.xlLineStyleNone;
            range.Borders[Excel.XlBordersIndex.xlDiagonalUp].LineStyle          = Excel.XlLineStyle.xlLineStyleNone;
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle            = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle             = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle          = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle           = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle      = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle    = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight               = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight                = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight             = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight              = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Weight       = Excel.XlBorderWeight.xlThin;

            /// 色設定
            range                       = excel.Sheet.get_Range(excel.Sheet.Cells[SetRow, 1], excel.Sheet.Cells[SetRow, dataGridView.Columns.Count]);
            range.Interior.ColorIndex   = 34;

            /// 自動サイズ調整
            //range.Columns.EntireColumn.AutoFit();
            //range.Rows.EntireRow.AutoFit();
            //range.Activate();

            return SetRow + ix;
        }
        /// <summary>
        /// マッピングマッピング
        /// </summary>
        /// <param name="dt">Excelに出力するUltraGridコレクション</param>
        /// <returns>成功時0</returns>
        public long MapingDataReport(DataGridView dataGridView, string Title, long SetRow) /// 2018/11/08 SLA2内田　マッピング解析機能追加
        {
            object missing      = System.Reflection.Missing.Value;

            /// 領域確保
            object[,] objItem   = new object[dataGridView.Rows.Count, dataGridView.Columns.Count];
            int SpaceCT         = 0;
            int AvgCT           = 0;
            int AverageTotal    = 0;
            int AvgCol          = dataGridView.Columns.Count - 1;

            /// 詳細データ設定
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].Visible)
                {
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        objItem[i, j]               = dataGridView[j, i].Value;
                    }
                    if (dataGridView[AvgCol, i].Value.ToString() == "") SpaceCT++;

                    if (AvgCT < 20)
                    {
                        try
                        {
                            int intData             = Convert.ToInt32(dataGridView[AvgCol, i].Value);
                            if (intData > 10)
                            {
                                AverageTotal        += intData;
                                AvgCT++;
                            }
                        }
                        catch { }
                        try
                        {
                            int intData             = Convert.ToInt32(dataGridView[AvgCol - 1, i].Value);
                            if (intData > 10)
                            {
                                AverageTotal        += intData;
                                AvgCT++;
                            }
                        }
                        catch { }
                    }
                }
            }
            /// タイトル
            Range range;
            range               = excel.Sheet.get_Range("A" + SetRow.ToString(), Missing.Value);
            range               = range.get_Resize(1, 1);
            range.Value2        = Title;
            /// データ
            SetRow              = SetRow + 1;
            range               = excel.Sheet.get_Range("A" + SetRow.ToString(), Missing.Value);
            range               = range.get_Resize(dataGridView.Rows.Count, dataGridView.Columns.Count);
            range.Value2        = objItem;

            /// 罫線設定
            range.Borders[Excel.XlBordersIndex.xlDiagonalDown].LineStyle        = Excel.XlLineStyle.xlLineStyleNone;
            range.Borders[Excel.XlBordersIndex.xlDiagonalUp].LineStyle          = Excel.XlLineStyle.xlLineStyleNone;
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle            = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle             = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle          = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle           = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle      = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle    = Excel.XlLineStyle.xlContinuous;
            range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight               = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight                = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight             = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight              = Excel.XlBorderWeight.xlThin;
            range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Weight       = Excel.XlBorderWeight.xlThin;


            /// 色調査
            //try
            //{
            //    if (SetRow >= 100) return 100;
            //    for (int i = 0; i < 100; i++)
            //    {
            //        range = excel.Sheet.get_Range(excel.Sheet.Cells[SetRow + i, 1], excel.Sheet.Cells[SetRow + i, dataGridView.Columns.Count]);
            //        range.Interior.ColorIndex = SetRow + i;
            //    }
            //    return 100;
            //}
            //catch { }

            /// サイズ調整
            if (AvgCT == 0)
            {
                range.ColumnWidth                                       = 2;
                range.Rows.EntireRow.AutoFit();
            }
            /// 背景色設定
            range.Activate();
            range.Select();
            /// 空白セルのとき
            if (SpaceCT > dataGridView.Rows.Count / 2)
            {
                range.FormatConditions.Add(XlFormatConditionType.xlCellValue, Excel.XlFormatConditionOperator.xlNotEqual, @"=""""");
            }
            /// 樹脂量セルのとき
            else if (AvgCT > 1)
            {
                int Average                                             = AverageTotal / AvgCT;
                range.FormatConditions.Add(XlFormatConditionType.xlCellValue, Excel.XlFormatConditionOperator.xlLess, "=" + Average.ToString());
                if (Average < 100)
                {
                    range.ColumnWidth                                   = 2;
                    range.Rows.EntireRow.AutoFit();
                }
                else
                {
                    range.ColumnWidth                                   = 3;
                    range.Rows.EntireRow.AutoFit();
                }
            }
            else
            {
                range.FormatConditions.Add(XlFormatConditionType.xlCellValue, Excel.XlFormatConditionOperator.xlGreater, "=0");
            }
            range.FormatConditions[1].Interior.PatternColorIndex        = Excel.XlPattern.xlPatternAutomatic;
            range.FormatConditions[1].Interior.ColorIndex               = 44;
            return SetRow + dataGridView.Rows.Count;
        }
	}
}
