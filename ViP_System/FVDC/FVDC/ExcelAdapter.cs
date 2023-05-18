using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel;

namespace FVDC
{
	/// <summary>
	/// EXCELを起動・操作するクラス
	/// </summary>
	public class ExcelAdapter : IDisposable
	{   
		public Excel.Application   App;
		public Excel.Workbook      Book;
		public Excel.Workbook      TemplateBook;
		public Excel.Worksheet     Sheet;
		public Excel.Worksheet     TemplateSheet;
		public Excel.Range         RangeObj;

		//コンストラクタ
		public ExcelAdapter()
		{
			this.App = new Excel.Application();//ApplicationClass();
		}



		~ExcelAdapter()
		{
			this.Dispose();
		}

		/// <summary>
		/// 空のワークブック作成
		/// </summary>
		public void CreateNewWorkBook()
		{
			Object objMissing   = System.Reflection.Missing.Value;
	
			this.Book           = this.App.Workbooks.Add(objMissing);
			this.Sheet          = (Excel.Worksheet)Book.Worksheets[1];
		}
		/// <summary>
		/// 空のワークシート作成
		/// </summary>
		public void CreateNewWorkSheet()
		{
			Object objMissing   = System.Reflection.Missing.Value;
			this.Sheet          = (Excel.Worksheet)Book.Sheets.Add(objMissing,this.Sheet,objMissing,objMissing);
		}


		/// <summary>
		/// 指定パスからブックを開く
		/// </summary>
		/// <param name="fileName"></param>
		public void LoadFromFile(string fileName)
		{
			Object objMissing   = System.Reflection.Missing.Value;

			this.Book           = this.App.Workbooks.Open(
				                    fileName, objMissing, objMissing, objMissing, objMissing,
				                    objMissing, objMissing, objMissing, objMissing, objMissing,
				                    objMissing, objMissing, objMissing);


			this.Sheet          = (Excel.Worksheet)Book.Worksheets[1];
		}

		/// <summary>
		/// 指定パスからテンプレートを開く
		/// </summary>
		/// <param name="fileName"></param>
		public void LoadTemplateFile(string TemplateName)
		{
			Object objMissing   = System.Reflection.Missing.Value;


			this.TemplateBook   = this.App.Workbooks.Open(
				                    TemplateName, objMissing, objMissing, objMissing, objMissing,
				                    objMissing, objMissing, objMissing, objMissing, objMissing,
				                    objMissing, objMissing, objMissing);


			this.TemplateSheet  = (Excel.Worksheet)TemplateBook.Worksheets[1];
		}



		/// <summary>
		/// Application.Cells[][]のラッパー
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public Excel.Range Cell(object row, object col)
		{
			return (Excel.Range)App.Cells[row, col];
		}


		/// <summary>
		/// Application.Rows[]のラッパー
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Excel.Range Row(object row)
		{
			return (Excel.Range)App.Rows[row, System.Reflection.Missing.Value];
		}


		/// <summary>
		/// Application.get_Rangeのラッパー
		/// </summary>
		/// <param name="cell1"></param>
		/// <param name="cell2"></param>
		/// <returns></returns>
		public Excel.Range Range(object cell1, object cell2)
		{
			return App.get_Range(cell1, cell2);
		}



		/// <summary>
		/// Bookの参照先を変更
		/// Activateは行わない。
		/// </summary>
		/// <param name="index"></param>
		public void ChangeBook(object index)
		{
			this.Book           = this.App.Workbooks[index];
		}


		/// <summary>
		/// Sheetの参照先を変更してActivate
		/// </summary>
		/// <param name="index"></param>
		public void ChangeSheet(object index)
		{
			this.Sheet          = (Excel.Worksheet)this.Book.Worksheets[index];
			//this.Sheet.Activate();
		}
		/// <summary>
		/// Sheetの参照先を変更してActivate
		/// </summary>
		/// <param name="index"></param>
		public void ChangeSheet(string name)
		{
			this.Sheet          = (Excel.Worksheet)this.Book.Worksheets[name];
			//this.Sheet.Activate();
		}
		/// <summary>
		/// Sheetの参照先を変更してActivate
		/// </summary>
		/// <param name="index"></param>
		public void ChangeTemplateSheet(object index)
		{
			this.TemplateSheet  = (Excel.Worksheet)this.TemplateBook.Worksheets[index];
			//this.Sheet.Activate();
		}
		/// <summary>
		/// Sheetの参照先を変更してActivate
		/// </summary>
		/// <param name="index"></param>
		public void ChangeTemplateSheet(string name)
		{
			this.TemplateSheet  = (Excel.Worksheet)this.TemplateBook.Worksheets[name];
			//this.Sheet.Activate();
		}
		
		/// <summary>
		/// 小判子描画
		/// </summary>
		/// <param name="prmRow"></param>
		/// <param name="prmColum"></param>
		/// <param name="prmText"></param>
		public void SmallStamp(int prmRow, int prmColum, string prmText)
		{
			/// 文字分割
			char []		sptchar	= { '　', ' ' };
			string []	sptText	= prmText.Split(sptchar);
			if ( sptText [0] == "" )
				return;
			/// 表示位置計算
			RangeObj			= Sheet.get_Range(Sheet.Cells [prmRow, prmColum], Sheet.Cells [prmRow, prmColum]);
			float	CircleTop	= (float)(Convert.ToDouble(RangeObj.Top)	- 17);
			float	CircleLeft	= (float)(Convert.ToDouble(RangeObj.Left)	+ 3);
			float	TextTop		= (float)(Convert.ToDouble(RangeObj.Top)	- 13);
			float	TextLeft	= (float)(Convert.ToDouble(RangeObj.Left)	+ 21);
			/// 判子
			Excel.Shape shMaru	= Sheet.Shapes.AddShape(Office.MsoAutoShapeType.msoShapeOval, CircleLeft, CircleTop, 35, 35);
			shMaru.Fill.Visible								= Office.MsoTriState.msoFalse;
			shMaru.Line.DashStyle							= Office.MsoLineDashStyle.msoLineSolid;
			shMaru.Line.ForeColor.SchemeColor				= 10;	
			Excel.Shape shText	= Sheet.Shapes.AddTextEffect(Office.MsoPresetTextEffect.msoTextEffect1, sptText[0], "ＭＳ Ｐ明朝", 13, Office.MsoTriState.msoFalse, Office.MsoTriState.msoFalse, TextLeft, TextTop);
			shText.TextEffect.ToggleVerticalText();
			shText.Line.ForeColor.SchemeColor				= 10;	
			shText.Fill.ForeColor.SchemeColor				= 10;	
			shText.Height									= 27;
			shText.Width									= 27;
		}

		/// <summary>
		/// 大判子描画
		/// </summary>
		/// <param name="prmRow"></param>
		/// <param name="prmColum"></param>
		/// <param name="prmText"></param>
		public void LargeStamp(int prmRow, int prmColum, string prmText)
		{
			/// 文字分割
			char []		sptchar	= { '　', ' ' };
			string []	sptText	= prmText.Split(sptchar);
			if ( sptText [0] == "" )
				return;
			/// 表示位置計算
			RangeObj			= Sheet.get_Range(Sheet.Cells [prmRow, prmColum], Sheet.Cells [prmRow, prmColum]);
			float	CircleTop	= (float)(Convert.ToDouble(RangeObj.Top)	+ 3);
			float	CircleLeft	= (float)(Convert.ToDouble(RangeObj.Left)	+ 5);
			float	TextTop		= (float)(Convert.ToDouble(RangeObj.Top)	+ 13);
			float	TextLeft	= (float)(Convert.ToDouble(RangeObj.Left)	+ 65);
			/// 判子
			Excel.Shape shMaru	= Sheet.Shapes.AddShape(Office.MsoAutoShapeType.msoShapeOval, CircleLeft, CircleTop, 85, 85);
			shMaru.Fill.Visible								= Office.MsoTriState.msoFalse;
			shMaru.Line.DashStyle							= Office.MsoLineDashStyle.msoLineSolid;
			//shMaru.Line.ForeColor.SchemeColor				= 10;	/// 2009/02/25 削除 SAC.Uchida 経費削減のため出力結果をモノクロにする
			shMaru.Line.ForeColor.SchemeColor				= 63;	/// 2009/02/25 追加 SAC.Uchida 経費削減のため出力結果をモノクロにする
			Excel.Shape shText	= Sheet.Shapes.AddTextEffect(Office.MsoPresetTextEffect.msoTextEffect1, sptText[0], "ＭＳ Ｐ明朝", 13, Office.MsoTriState.msoFalse, Office.MsoTriState.msoFalse, TextLeft, TextTop);
			shText.TextEffect.ToggleVerticalText();
			//shText.Line.ForeColor.SchemeColor				= 10;	/// 2009/02/25 削除 SAC.Uchida 経費削減のため出力結果をモノクロにする
			shText.Line.ForeColor.SchemeColor				= 63;	/// 2009/02/25 追加 SAC.Uchida 経費削減のため出力結果をモノクロにする
			//shText.Fill.ForeColor.SchemeColor				= 10;	/// 2009/02/25 削除 SAC.Uchida 経費削減のため出力結果をモノクロにする
			shText.Fill.ForeColor.SchemeColor				= 63;	/// 2009/02/25 追加 SAC.Uchida 経費削減のため出力結果をモノクロにする
			shText.Height									= 60;
			shText.Width									= 60;
		}
        
		/// <summary>
		/// 文字描画
		/// </summary>
		/// <param name="prmRow"></param>
		/// <param name="prmColum"></param>
		/// <param name="prmText"></param>
		public void TextStamp(int prmRow, int prmColum, string prmText)
		{
			/// 表示位置計算
			RangeObj			= Sheet.get_Range(Sheet.Cells [prmRow, prmColum], Sheet.Cells [prmRow, prmColum]);
			float	TextTop		= (float)(Convert.ToDouble(RangeObj.Top)	- 3);
			float	TextLeft	= (float)(Convert.ToDouble(RangeObj.Left)   + 10);
			/// 文字描画
			Excel.Shape shText	= Sheet.Shapes.AddTextEffect(Office.MsoPresetTextEffect.msoTextEffect1, prmText, "ＭＳ Ｐ明朝", 13, Office.MsoTriState.msoFalse, Office.MsoTriState.msoFalse, TextLeft, TextTop);
			shText.Line.ForeColor.SchemeColor				= 63;	
			shText.Fill.ForeColor.SchemeColor				= 63;	
			shText.Height									= 16;
			shText.Width									= 100;
		}

		/// <summary>
		/// リソース開放
		/// </summary>
		public void Dispose()
		{
			//this.Chart.Dispose();

			if(this.RangeObj != null)
			{
				while (Marshal.ReleaseComObject(RangeObj) > 0);
				RangeObj        = null;
			}
			
			if(this.Sheet != null)
			{
				while (Marshal.ReleaseComObject(Sheet) > 0);
				Sheet           = null;
			}
			if(this.TemplateSheet != null)
			{
				while (Marshal.ReleaseComObject(TemplateSheet) > 0);
				TemplateSheet   = null;
			}


			if(this.Book != null)
			{
				while (Marshal.ReleaseComObject(Book) > 0);
				Book            = null;
			}
			if(this.TemplateBook != null)
			{
				while (Marshal.ReleaseComObject(TemplateBook) > 0);
				TemplateBook    = null;
			}

			if(this.App != null)
			{
				while (Marshal.ReleaseComObject(App) > 0);
				App             = null;
			}

			GC.Collect();
		}
	}


	/// <summary>
	/// Chart関係のクラス
	/// </summary>
	public class ExcelChartAdapter : IDisposable
	{
		private Excel.ChartObject obj;
		private Excel.Chart       chart;

		public ExcelChartAdapter()
		{
		}

		public ExcelChartAdapter(Excel.ChartObject chartObject)
		{
			ChartObject         = chartObject;
		}

		public Excel.ChartObject ChartObject
		{
			get
			{
				return this.obj;
			}
			set
			{
				this.obj        = value;
				this.chart      = value.Chart;
			}
		}

		~ExcelChartAdapter()
		{
			this.Dispose();
		}


		public Excel.Chart Chart
		{
			get
			{
				return this.chart;
			}
		}


		/// <summary>
		/// SheetからChartObjectを得る。
		/// タイトル検索時に同名タイトルのChartが複数ある場合は、最初のもののみを返す。
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="index">タイトル検索時はChartTitle.Text。それ以外はindex</param>
		/// <param name="searchByChartTitile">ChartTitleで検索する場合はtrue</param>
		/// <returns></returns>
		public Excel.ChartObject GetChartObject(Excel.Worksheet sheet, object index, bool searchByChartTitile)
		{
			if(searchByChartTitile)
			{
				for(int i = 1; i <= ((Excel.ChartObjects)sheet.ChartObjects(System.Reflection.Missing.Value)).Count; i++)
				{
					if(((Excel.ChartObject)sheet.ChartObjects(i)).Chart.ChartTitle.Text == index.ToString())
					{
						return (Excel.ChartObject)sheet.ChartObjects(i);
					}
				}
			}
			else
			{
				return (Excel.ChartObject)sheet.ChartObjects(index);
			}

			Debug.WriteLine("Chart Not Found");
			throw new ApplicationException("ChartObject not found", null);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="chart"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public Excel.Series Series(Excel.Chart chart, object index)
		{
			return (Excel.Series)chart.SeriesCollection(index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Excel.Series Series(object index)
		{
			return (Excel.Series)this.Chart.SeriesCollection(index);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Excel.SeriesCollection SeriesCollection
		{
			get
			{
				return (Excel.SeriesCollection)this.Chart.SeriesCollection(System.Reflection.Missing.Value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="rowcol"></param>
		/// <param name="labels"></param>
		/// <param name="categoryLabels"></param>
		/// <param name="replace"></param>
		public void AddSeries(object source, string label)
		{
			object missing      = System.Reflection.Missing.Value;
			
			((Excel.SeriesCollection)this.Chart.SeriesCollection(missing)).NewSeries();
			int index           = ((Excel.SeriesCollection)this.Chart.SeriesCollection(missing)).Count;

			((Excel.Series)this.Chart.SeriesCollection(index)).Values   = source;
			((Excel.Series)this.Chart.SeriesCollection(index)).Name     = label;
		}

		/// <summary>
		/// リソース解放処理
		/// </summary>
		public void Dispose()
		{
			if(this.chart != null)
			{
				while (Marshal.ReleaseComObject(chart) > 0);			
				chart           = null;
			}

			if(this.obj != null)
			{
				while (Marshal.ReleaseComObject(obj) > 0);
				obj             = null;
			}
		}
	}
}

