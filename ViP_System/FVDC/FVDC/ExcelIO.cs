/*************************************************************************************
 * システム名     : 投入計画取込システム
 *  
 * 処理名         : ExcelIO　エクセル入出力
 * 
 * 概略           : エクセル入出力関連の処理を行う
 * 
 * 作成           : 2009/03/09 SAC.Uchida
 * 
 * 修正履歴       : 2018/11/08 SLA2.Uchida マッピング解析機能追加
 ************************************************************************************/

using System;
using System.IO;
using System.Data;
using System.Reflection;
using Excel;

namespace FVDC
{
	/// <summary>
	/// Excelを用いる帳票の基底クラス
	/// </summary>
	///
	public class ExcelIO : IDisposable
	{
		/// 変数宣言
		protected	ExcelAdapter	excel;

		public ExcelIO()
		{
		}

		~ExcelIO()
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
				excel = new ExcelAdapter();
			}
		
			excel.Book = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet = (Worksheet)excel.Book.Worksheets[1];

			object missing = System.Reflection.Missing.Value;
            
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
			for(int i = 0; i <= dt.Rows.Count - 1; i++)
			{
				SetIx								= 0;
				for(int j = 1; j < dt.Columns.Count; j++)
				{
					if ( ( dt.Columns [j - 1].ColumnName != dt.Columns [j - 1].Caption )
						&& ( dt.Columns [j - 1].Caption.Trim() != "" ) )
					{
						try
						{
							switch ( dt.Columns [j - 1].DataType.Name )
							{
								case "Boolean":
									if ( Convert.ToBoolean(dt.Rows [i] [j - 1]) )
									{
										objItem [ix, SetIx] = "○";
									}
									break;
								case "DateTime":
									objItem [ix, SetIx] = Convert.ToDateTime(dt.Rows [i] [j - 1]).ToString("yyyy/MM/dd HH:mm:ss");
									break;
								default:
									objItem [ix, SetIx] = dt.Rows [i] [j - 1].ToString();
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
        /// CSVファイルデータを取得しデータセットに書き込む
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="KeyChar"></param>
        /// <param name="dsCSV"></param>
        /// <returns></returns>
        public bool CSV_Read(string FileName, string DataList, ref int SiftCT, ref DsFree dsCSV, NotifyStatusHandler notifyHander)　/// 2018/11/08 SLA2.Uchida マッピング解析機能追加
        {

            try
            {
                string[] sptTitle           = DataList.Split(',');
                string KeyChar              = sptTitle[0];
                object objValue;

                if (excel == null)
                {
                    excel                   = new ExcelAdapter();
                }
                
                /// Excel初期設定
                excel.App.DisplayAlerts     = false;

                object objMissing           = System.Reflection.Missing.Value;
                excel.Book                  = excel.App.Workbooks.Open(
                                            FileName, objMissing, objMissing, objMissing, objMissing,
                                            objMissing, objMissing, objMissing, objMissing, objMissing,
                                            objMissing, objMissing, objMissing);

                excel.Sheet                 = (Excel.Worksheet)excel.Book.Worksheets[1];

                /// セルの内容をアレイリストに一括でコピーする
                Excel.Range range           = excel.Sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, objMissing);
                int MaxRow                  = range.Row;
                int MaxCol                  = range.Column;

                Range FindRange　           = range.Find(KeyChar);

                range                       = excel.Sheet.get_Range("A" + FindRange.Row, objMissing);
                range                       = range.get_Resize(MaxRow - FindRange.Row + 1, MaxCol);
                objValue                    = range.Value;
                MaxRow                      = MaxRow - FindRange.Row + 1;
                object[,] objItem           = new object[MaxRow, MaxCol];
                Array.Copy((System.Array)objValue, objItem, objItem.Length);
                int CntRow                  = MaxRow / 100;
                int OutTimeCT               = 0;
                int TotalCT                 = 0;
                int SumCT                   = 0;

                /// タイトルデータをデータセットに追加する

                /// データカラム生成
                dsCSV.List.Columns.Clear();

                /// 位置情報が含まれるか検索
                bool LogingFG                                           = false;
                if (DataList.Contains("位置"))
                {
                    for (int k = 0; k < MaxCol; k++)
                    {
                        if (objItem[0, k].ToString().Contains("位置"))
                        {
                            LogingFG                                    = true;
                            break;
                        }
                    }
                    /// 位置情報が有るとき
                    if (LogingFG)
                    {
                        DataList                                        = sptTitle[0] + "," + sptTitle[1] + "," + sptTitle[2];
                    }
                    /// 位置情報が無いとき
                    else
                    {
                        /// 存在するタイトルだけにする
                        DataList                                        = sptTitle[0];
                        for (int k = 1; k < MaxCol; k++)
                        {
                            for (int j = 1; j < sptTitle.Length; j++)
                            {
                                try
                                {
                                    if (objItem[0, k].ToString().Contains(sptTitle[j]))
                                    {
                                        DataList                        = DataList + "," + sptTitle[j];
                                        if (sptTitle[j].Contains("吐出時間"))
                                        {
                                            OutTimeCT++;
                                        }
                                        break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    sptTitle                                            = DataList.Split(',');
                }
                else
                {
                    LogingFG                                            = true;
                }

                string CheckTitle                                       = "";
                for (int i = 0; i < sptTitle.Length; i++)
                {
                    if ((sptTitle[i].Contains("ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ"))
                        || (sptTitle[i].Contains("位置")))
                    {
                        CheckTitle                                      = sptTitle[i];
                        dsCSV.List.Columns.Add(sptTitle[i], Type.GetType("System.Int32"));
                        dsCSV.List.Columns[sptTitle[i]].DefaultValue    = 0;
                    }
                    else
                    {
                        dsCSV.List.Columns.Add(sptTitle[i], Type.GetType("System.String"));
                        dsCSV.List.Columns[sptTitle[i]].DefaultValue    = "";
                    }
                    dsCSV.List.Columns[sptTitle[i]].Caption             = sptTitle[i];
                }
                DsFree dsSort                                           = (DsFree)dsCSV.Clone();
                /// データ取得
                int IX                                                  = -1;
                for (int i = 1; i < MaxRow; i++)
                {
                    if (objItem[i, 0].ToString() != "")
                    {
                        IX++;
                        dsCSV.List.Rows.Add(new object[] { "0" });

                        for (int k = 0; k < MaxCol; k++)
                        {
                            for (int j = 0; j < sptTitle.Length; j++)
                            {
                                try
                                {
                                    ///↓↓↓↓↓↓　2019/02/25 SLA2.Uchida アドレスが０のデータを読み飛ばす　↓↓↓↓↓↓
                                    if ((objItem[0, k].ToString().Contains(CheckTitle))
                                        && (objItem[i, k].ToString() == "0"))
                                    {
                                        for (i = i + 1; i < MaxRow; i++)
                                        {
                                            if (objItem[i, k].ToString() != "0")
                                            {
                                                break;
                                            }
                                        }
                                        if (i == MaxRow)
                                        {
                                            dsCSV.List.Rows.RemoveAt(dsCSV.List.Rows.Count - 1);
                                            k                           = MaxCol;
                                            break;
                                        }
                                    }
                                    ///↑↑↑↑↑↑　2019/02/25 SLA2.Uchida アドレスが０のデータを読み飛ばす　↑↑↑↑↑↑
                                    if (objItem[0, k].ToString().Contains(sptTitle[j]))
                                    {
                                        try
                                        {
                                            dsCSV.List[IX][sptTitle[j]] = objItem[i, k].ToString();
                                        }
                                        catch
                                        {
                                            dsCSV.List[IX][sptTitle[j]] = Convert.ToInt32(objItem[i, k]);
                                        }
                                        /// 排出時間を２０件だけ合計する
                                        if ((SumCT < 20)
                                            && (sptTitle[j].Contains("吐出時間")))
                                        {
                                            try
                                            {
                                                TotalCT                 += Convert.ToInt32(objItem[i, k]);
                                                SumCT++;
                                            }
                                            catch { }
                                        }

                                        break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    /// ステータスバーの更新    
                    if ((CntRow > 0) && (i % CntRow == 0))
                    {
                        notifyHander(i / CntRow, null);
                    }
                }

                if (LogingFG)
                {
                    /// 並べ替え
                    DataView dtView             = new DataView(dsCSV.List);
                    dtView.Sort                 = sptTitle[1];
                    for (int i = 0; i < dsCSV.List.Rows.Count; i++)
                    {
                        if (dtView[i].Row.ItemArray[0].ToString() != "")
                        {
                            dsSort.List.Rows.Add(dtView[i].Row.ItemArray);
                        }
                    }
                    dsCSV                       = (DsFree)dsSort.Copy();
                }
                /// 排出時間が有るとき
                if (OutTimeCT > 0)
                {
                    /// 合計の平均を求める
                    int AvgCT                   = TotalCT / SumCT / 2;
                    int SubCT                   = 0;

                    /// 無効排出時間列の検出＆不要カラムの削除
                    for (int i = dsCSV.List.Columns.Count - 1; i > 0; i--)
                    {
                        if (dsCSV.List.Columns[i].ColumnName.Contains("吐出時間"))
                        {
                            /// 平均の５０％以下もしくは数字で無いとき無効列としてカウントする
                            try
                            {
                                if (Convert.ToInt32(dsCSV.List[1][i].ToString()) < AvgCT)
                                {
                                    SubCT++;
                                }
                            }
                            catch
                            {
                                SubCT++;
                            }
                            /// 列を削除する
                            dsCSV.List.Columns.Remove(dsCSV.List.Columns[i].ColumnName);
                        }
                    }
                    SiftCT                      = OutTimeCT - SubCT;
                }

                return true;
            }
            catch (Exception ex)
            {
                ///TODO: エラー処理
                excel.App.DisplayAlerts			= true;
                return false;
            }
            finally
            {
                /// ブックを閉じる
                excel.Book.Close(false, FileName, System.Reflection.Missing.Value);
                excel.App.DisplayAlerts			= true;
            }
        }
    }
}
