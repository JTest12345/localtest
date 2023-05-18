using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.IO;
using GEICS.Database;
using System.Drawing;
namespace GEICS
{
    public class ExcelControl : IDisposable
    {
		private static object lockObject = new object();
		private static ExcelControl singleton = null;
        private Excel.ApplicationClass xlApp = null;
        private Excel.Workbook xlWorkBook = null;
        private Excel.Worksheet xlWorkSheet = null;
        private Excel.Range xlRange = null;
        private string currentBookName = string.Empty;
        public bool nonCloseFlg = false;

        object objMissing = System.Reflection.Missing.Value;

		public const int TPL00_DATASTART_ROW = 1;
        public const int TPL01_DATASTART_ROW = 14;
        public const int TPL01_HEADER_ROW = TPL01_DATASTART_ROW - 4;
		public const int TPL02_DATASTART_ROW = 4;
		public const int TPL02_HEADER_COL = 6;
		public const int TPL02_HEADER_ROW = TPL02_DATASTART_ROW - 1;
        public const int TPL04_DATASTART_ROW = 14;

		public const int MAX_COL = 256;

		const int PARAMETER_REF_COL = 12;
        const int RESINGROUP_REF_COL = 11;


        public int ProgressRate { get; set; }

		public struct MasterLimitExcelData
		{
			public object[,] data;
			public bool[,] colorFgArray;
			public bool coloringFG;
			public string typeCDStr;
            public bool resinGroupOutputFG;
        }

        public struct MasterPlcFileConvExcelData
        {
            public object[,] data;
            public bool[] colorFgArray;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ExcelControl()
        {
        }

        ~ExcelControl()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            try
            {
				if (singleton != null)
				{
					if (singleton.xlRange != null)
					{
						while (Marshal.ReleaseComObject(singleton.xlRange) > 0) ;
						singleton.xlRange = null;
					}
					if (singleton.xlWorkSheet != null)
					{
						while (Marshal.ReleaseComObject(singleton.xlWorkSheet) > 0) ;
						singleton.xlWorkSheet = null;
					}

					if (singleton.xlWorkBook != null)
					{
						//this.xlWorkBook.Close(false, objMissing, false);
						while (Marshal.ReleaseComObject(singleton.xlWorkBook) > 0) ;
						singleton.xlWorkBook = null;
					}


					if (singleton.xlApp != null)
					{
						singleton.xlApp.DisplayAlerts = false;

						singleton.xlApp.Quit();

						singleton.xlApp.DisplayAlerts = true;

						while (Marshal.ReleaseComObject(singleton.xlApp) > 0) ;
						singleton.xlApp = null;
					}
				}
                GC.Collect();
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            {
				if (singleton != null)
				{
					singleton.xlWorkSheet = null;
					singleton.xlWorkBook = null;
					singleton.xlApp = null;
					singleton = null;
				}

                GC.Collect();
            }
        }

		public static ExcelControl GetInstance()
		{
			lock (lockObject)
			{
				if (singleton == null)
				{
					singleton = new ExcelControl();
				}

				if (singleton.xlApp == null)
				{
					singleton.xlApp = new Excel.ApplicationClass();
				}

				return singleton;
			}
		}

		public static void CloseProcess()
		{
			if (singleton != null)
			{
				singleton.Dispose();
				singleton = null;
			}
		}

        public void SaveToDir(string filenm)
        {
            /*
            if (System.IO.File.Exists(filenm))
            {
                //既にあれば削除
                System.IO.File.Delete(filenm);
            }
            */

			singleton.xlApp.DisplayAlerts = false;
            //xlWorkBook.Save();
			singleton.xlWorkBook.SaveAs(filenm, objMissing, objMissing, objMissing, objMissing, objMissing, Excel.XlSaveAsAccessMode.xlExclusive, objMissing, objMissing, objMissing, objMissing);
			singleton.xlApp.DisplayAlerts = true;

        }

        /// <summary>
        /// Excelブックを開きます。
        /// </summary>
        /// <param name="wBookName"></param>
        /// <returns></returns>
        private bool workBookOpen(string wBookName, bool readOnlyFG)
        {
            //ワークブック引数設定
            string myFileName = wBookName;		            // ファイル名
            object myUpdateLinks = 3;				        // ファイル内のリンクの更新方法 　3:外部参照、リモート参照共に更新されます
            object myReadOnly = readOnlyFG;                       // 読取り専用設定　true:読取専用
            object myFormat = 6;                            // ファイル名がテキスト形式である場合に使用する区切り文字を指定　6:Delimiterで指定した文字を使用する
            object myPassword = objMissing;                 // 読込みパスワード
            object myWriteResPassword = objMissing;         // 書込みパスワード
            object myIgnoreReadOnlyRecommended = true;      // 読取り推奨メッセージ　true:非表示
            object myOrigin = Excel.XlPlatform.xlWindows;   // テキストファイル時のコードセット指定（日本版Excelでは非対応）
            object myDelimiter = ",";                       // 区切り文字
            object myEditable = false;                      // Excel4.0アドイン用
            object myNotify = false;                        // ファイルが開けない場合に通知リストに追加する　false：しない
            object myConverter = objMissing;                // 使用コンバータID
            object myAddToMru = false;                      // 最近使用したファイルの一覧への追加　false：しない
            object myLocal = objMissing;                    // 使用コンバータID
            object myLoad = objMissing;                     // 使用コンバータID

            try
            {
				//if (currentBookName == wBookName)
				//{
				//    return true;
				//}

                //ワークブック開く
                singleton.xlWorkBook = singleton.xlApp.Workbooks.Open(myFileName, myUpdateLinks, myReadOnly, myFormat, myPassword, myWriteResPassword,
                    myIgnoreReadOnlyRecommended, myOrigin, myDelimiter, myEditable, myNotify, myConverter, myAddToMru);
                this.currentBookName = wBookName;
                return true;
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.Message);
                return false;
            }
        }

        /// <summary>セル結合 縦</summary>
		//private void cellMerge(int maxRowIndex)
		//{
		//    int startRowIndex = 6;

		//    Excel.Range startCell = (Excel.Range)singleton.xlWorkSheet.Cells[6, "C"];
		//    Excel.Range endCell = null;

		//    for (int i = 0; i <= maxRowIndex; i++)
		//    {
		//        endCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i, "C"];
		//        if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
		//        {
		//            endCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "C"];

		//            //作業
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(startCell, endCell);
		//            singleton.xlRange.Merge(false);

		//            startCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i, "C"];


		//            //レベル
		//            Excel.Range tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "B"];
		//            Excel.Range tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "B"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            //NASCA作業名
		//            tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "D"];
		//            tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "D"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            //教育予定
		//            tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "J"];
		//            tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "J"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            //見直し周期
		//            tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "K"];
		//            tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "K"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            //認定日
		//            tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "M"];
		//            tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "M"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            //認定者
		//            tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "N"];
		//            tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[6 + i - 1, "N"];
		//            singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
		//            singleton.xlRange.Merge(false);

		//            startRowIndex = 6 + i;
		//        }
		//    }
		//}

        /// <summary>セル結合 縦</summary>
        private void cellMergeEdu(int maxRowIndex, int startRowIndex)
        {
            int fixStartRowIndex = startRowIndex;

			Excel.Range startCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex, "D"];
            Excel.Range endCell = null;
			Excel.Range levelStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "E"];
            Excel.Range levelEndCell = null;

            for (int i = 0; i <= maxRowIndex; i++)
            {
				endCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
				levelEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
                {
                    //レベル
					levelEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i - 1, "E"];
					singleton.xlRange = singleton.xlWorkSheet.get_Range(levelStartCell, levelEndCell);
					singleton.xlRange.Merge(false);

					startCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
					levelStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                }
                else
                {
                    if ((levelEndCell.Value == null) || (levelStartCell.Value.ToString() != levelEndCell.Value.ToString()))
                    {
                        //レベル
						levelEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i - 1, "E"];
						singleton.xlRange = singleton.xlWorkSheet.get_Range(levelStartCell, levelEndCell);
						singleton.xlRange.Merge(false);

						levelStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                    }
                }
            }


			startCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex, "D"];
            endCell = null;

            for (int i = 0; i <= maxRowIndex; i++)
            {
				endCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
                if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
                {
					endCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i - 1, "D"];

                    //スキルマップ名
					singleton.xlRange = singleton.xlWorkSheet.get_Range(startCell, endCell);
					singleton.xlRange.Merge(false);

					startCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i, "D"];


                    //係
					Excel.Range tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "B"];
					Excel.Range tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i - 1, "B"];
					singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
					singleton.xlRange.Merge(false);

                    //様式番号
					tmpStartCell = (Excel.Range)singleton.xlWorkSheet.Cells[startRowIndex, "C"];
					tmpEndCell = (Excel.Range)singleton.xlWorkSheet.Cells[fixStartRowIndex + i - 1, "C"];
					singleton.xlRange = singleton.xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
					singleton.xlRange.Merge(false);

                    startRowIndex = fixStartRowIndex + i;
                }
            }
        }

        /// <summary>ExcelSheetのデータ取込</summary>
        public object[,] DataRead(string wBookName)
        {
			if (!singleton.workBookOpen(wBookName, true))
            {
                System.Windows.Forms.MessageBox.Show(wBookName + Constant.MessageInfo.Message_18);
                return null;
            }

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

            //保護解除
			singleton.xlWorkSheet.Unprotect(objMissing);

            //データ取得
			Excel.Range endCell = singleton.xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, objMissing);
			object[,] xlData = (object[,])(((Excel.Range)singleton.xlWorkSheet.get_Range("A1", endCell)).Value);

            //破棄
            while (Marshal.ReleaseComObject(endCell) > 0) ;
            endCell = null;

            return xlData;
        }

        /// <summary>ExcelSheetのデータ取込</summary>
        //public object[,] DataRead(string wBookName,int nSheetNo)
        public void DataRead(string wBookName, int nSheetNo, bool readOnlyFG)
        {
			if (!singleton.workBookOpen(wBookName, readOnlyFG))
            {
                System.Windows.Forms.MessageBox.Show(wBookName + Constant.MessageInfo.Message_18);
                return;
            }

            //if (xlWorkBook.Worksheets.Count < nSheetNo)
            //{
            //    xlWorkBook.Worksheets.Add;
            //}
			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[nSheetNo];


            //保護解除
			singleton.xlWorkSheet.Unprotect(objMissing);

            //データ取得
			Excel.Range endCell = singleton.xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, objMissing);
            //object[,] xlData = (object[,])(((Excel.Range)xlWorkSheet.get_Range("A1", endCell)).Value);
            //xlWorkBook.Close(false, wBookName, false);

            //破棄
            while (Marshal.ReleaseComObject(endCell) > 0) ;
            endCell = null;
        }

		/// <summary>
		/// ExcelSheetに引数のアドレス直下に保存されている画像(連番.png)をExcelに貼り付けていく
		/// </summary>
		/// <param name="Address"></param>
		/// <returns></returns>
		public bool OutputGraph()
		{
			string tempDirPath = Path.Combine(Path.GetTempPath(), "GEICS_GRAPH");
			string imgDirPath = Path.Combine(Path.GetTempPath(), @"GEICS_GRAPH\Img");
			string tempFilePath = Path.GetTempFileName();

			string filePath = Path.Combine(Path.GetDirectoryName(tempFilePath), string.Format("{0}.xls", Path.GetFileNameWithoutExtension(tempFilePath)));
	
			File.Move(tempFilePath, filePath);

			if (!workBookOpen(filePath, true))
			{
				throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
			}

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.ActiveSheet;
			singleton.xlWorkBook.SaveCopyAs(filePath);

			int x_shift = 0;

			//次書き足すTypeの列を探索
			for (int i = 0; ; i++)
			{
				if (((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value == null)
				{
					//((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value = sType;
					//((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value = "";
					x_shift = i * 720;
					break;
				}
			}

			int nNum = 0, x = 0, y = 0;

			string[] sortedFpath = new string[] { };

			//0.png,1.png...の順に貼り付けていく
			sortedFpath = System.IO.Directory.GetFiles(imgDirPath);
			//Array.Sort(sortedFpath);
			int nFileNameIndex = sortedFpath.Length;

			for (int i = 0; i < nFileNameIndex; i++)
			{
				System.IO.FileInfo fileinfo = new System.IO.FileInfo(imgDirPath);
				string swfullpath = fileinfo.FullName + "\\" + i + ".png";
				if (!File.Exists(swfullpath))
				{
					continue;
				}

				Image bmp = Bitmap.FromFile(swfullpath);

				if (nNum % 2 == 0)//左側
				{
					x = 0;
				}
				else//右側
				{
					x = bmp.Width + 5;
				}
				x = x + x_shift;//既に登録されているType分xはシフトする。

				y = (bmp.Height * (nNum / 2) + 15);

				singleton.xlWorkSheet.Shapes.AddPicture(swfullpath, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, x, y, bmp.Width, bmp.Height);

				nNum++;
			}

			singleton.xlApp.Visible = true;

			return true;
		}

        /// <summary>
        /// ExcelSheetに引数のアドレス直下に保存されている画像(連番.png)をExcelに貼り付けていく
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ImageOutput(string Address1,string Address2, string sAssetsNM,string sType, string sFileNM,int nSheetNo)
        {
            string sFileFullName = "";
            bool fnew = false;//初めてのExcel出力の場合

            foreach (string sfullpath in System.IO.Directory.GetFiles(Address2))
            {
                string swfilenm = sfullpath.Substring(Address2.Length + 1, sfullpath.Length - Address2.Length - 1);//ファイル名取得
                //同ライン/同工程/同日のExcelが出力されている場合
                if (swfilenm.Contains(sFileNM) == true)
                {
                    fnew = true;//既に同条件で出力されている。
                    break;
                }
            }

            if (fnew == true)
            {
                sFileFullName = Address2 + "\\" + sFileNM;//既に同条件で出力されたファイルに追記する。
            }
            else
            {
                sFileFullName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\" + SLCommonLib.Commons.Configuration.GetAppConfigString("ReportTemplateName");//新規の場合は、こちら
            }

            //object[,] xlData = DataRead(sFileFullName, nSheetNo);
            DataRead(sFileFullName, nSheetNo, false);

            //int rowMaxIndex = xlData.GetLength(0);
			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[nSheetNo];
			singleton.xlWorkSheet.Name = sType;

            int x_shift = 0;

            //次書き足すTypeの列を探索
            for (int i = 0; ; i++)
            {
                if (((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value == null)
                {
                    //((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value = sType;
                    ((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value = sAssetsNM;
                    x_shift=i*720;
                    break;
                }
            }
            
            int nNum = 0, x = 0, y = 0;
            //string sImageAddress = Address1 + "\\" + sType;
            string sImageAddress = Address1;

            string[] sortedFpath = new string[] { };

            //0.png,1.png...の順に貼り付けていく
            sortedFpath = System.IO.Directory.GetFiles(sImageAddress);
            //Array.Sort(sortedFpath);
            int nFileNameIndex = sortedFpath.Length;

            for (int i = 0; i < nFileNameIndex; i++) {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(sImageAddress);
                string swfullpath = fileinfo.FullName +"\\"+ i + ".png";
                if (!File.Exists(swfullpath)) 
                {
                    continue;
                }

                if (nNum % 2 == 0)//左側
                {
                    x = 0;
                }
                else//右側
                {
                    x = 355;
                }
                x = x + x_shift;//既に登録されているType分xはシフトする。

                y = (145 * (nNum / 2) + 15);
				singleton.xlWorkSheet.Shapes.AddPicture(swfullpath, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, x, y, 350, 140);
                nNum++;
            }
            /*
            //画像をExcelへ貼り付けしていく
            foreach (string sfullpath in System.IO.Directory.GetFiles(sImageAddress))
            {
                string swfilenm = sfullpath.Substring(sImageAddress.Length + 1, sfullpath.Length - sImageAddress.Length - 1);      //ファイル名取得
                //画像ファイルなら
                if (swfilenm.Contains(".png") == true)
                {
                    if (nNum % 2 == 0)//左側
                    {
                        x = 0;
                    }
                    else//右側
                    {
                        x = 355;
                    }
                    x = x + x_shift;//既に登録されているType分xはシフトする。

                    y = (145 * (nNum / 2) + 15);
                    xlWorkSheet.Shapes.AddPicture(sfullpath, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, x, y, 350, 140);
                    nNum++;
                }
            }
            */

            return true;
        }

		[Obsolete("LimitDataOutput()を使用して下さい。", true)]
        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutput(DateTime dtOutput, string typeNM, string diceCount, string lineNM, dsTmPLMEx dsTmPLMEx,List<string> ListModelNM)
        {
            string sfileName = string.Empty;

			if ((Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG) && Constant.fOutline)
            {
                sfileName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Master_MAP_OUT.xls";//MAPアウトライン用テンプレート   
            }
			else if ((Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG) && !Constant.fOutline)
            {
                sfileName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Master_MAP.xls";
            }
            else 
            {
                sfileName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Master.xls";
            }

            object[,] xlData = DataRead(sfileName);

            int rowMaxIndex = xlData.GetLength(0);

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			singleton.xlWorkSheet.Cells[4, "D"] = dtOutput;
			singleton.xlWorkSheet.Cells[5, "D"] = typeNM;
            
            //2012.3.8 HIshiguchi ダイス------------
			singleton.xlWorkSheet.Cells[4, "F"] = diceCount;

#if NMC
            if (Constant.fOutline)
            {
                xlWorkSheet.Cells[6, "D"] = "Outline";
            }
            else 
            {
                xlWorkSheet.Cells[6, "D"] = "";
            }
#else
            if(Constant.fOutline)
            {
				singleton.xlWorkSheet.Cells[6, "D"] = "アウトライン";
            }
            else if (Constant.fSemi)
            {
				singleton.xlWorkSheet.Cells[6, "D"] = "高生産性ライン";
            }
            else
            {
				singleton.xlWorkSheet.Cells[6, "D"] = "自動搬送ライン";
            }
#endif
            int nFix=14;
            int nQcParamNo;
            string sModelNM="";

            for (int k = nFix; k <= rowMaxIndex; k++)
            {

                try
                {
                    nQcParamNo = Convert.ToInt32(xlData[k, 2]);
                }
                catch
                {
                    nQcParamNo = 0;
                }
                sModelNM = Convert.ToString(xlData[k, 3]);

                if (nQcParamNo != 0)
                {

                    for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
                    {
                        if ((dsTmPLMEx.TvPLMEx[i].QcParam_NO == nQcParamNo) &&
                            (dsTmPLMEx.TvPLMEx[i].Model_NM == sModelNM))
                        {
                            try
                            {
								singleton.xlWorkSheet.Cells[k, "A"] = dsTmPLMEx.TvPLMEx[i].Rev;  //Rev
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "A"] = "";  //Rev
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "H"] = dsTmPLMEx.TvPLMEx[i].Parameter_MAX;  //管理値上限
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "H"] = "";  //管理値上限
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "J"] = dsTmPLMEx.TvPLMEx[i].Parameter_MIN;  //管理値下限
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "J"] = "";  //管理値下限
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "L"] = dsTmPLMEx.TvPLMEx[i].Parameter_VAL.Trim();  //文字列
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "L"] = "";  //文字列
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "N"] = dsTmPLMEx.TvPLMEx[i].QcLine_PNT;     //打点数
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "N"] = "";     //打点数
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "P"] = dsTmPLMEx.TvPLMEx[i].QcLine_MAX;     //管理値上限
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "P"] = "";     //管理値上限
                            }

                            try
                            {
								singleton.xlWorkSheet.Cells[k, "R"] = dsTmPLMEx.TvPLMEx[i].QcLine_MIN;     //管理値下限
                            }
                            catch
                            {
								singleton.xlWorkSheet.Cells[k, "R"] = "";     //管理値下限
                            }
                        }
                    }
                }
            }

            //保護
			singleton.xlWorkSheet.Protect(objMissing, objMissing, objMissing, objMissing, objMissing);

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutput(object[,] data, string startDT, string endDT)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Result", true))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Result" + Constant.MessageInfo.Message_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			singleton.xlWorkSheet.Cells[9, "F"] = startDT + "～" + endDT + "までの登録データを取得";

			singleton.xlRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[11, "B"], singleton.xlWorkSheet.Cells[11 + rowMaxIndex - 1, "G"]);
			singleton.xlRange.Value2 = data;

            //セル幅を広げる
			singleton.xlRange.EntireColumn.AutoFit();

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputTemp(string sectionNM, string mtgProNM, string proNM, NameValueCollection nvcWork)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\SkillMapTemp", true))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\SkillMapTemp" + Constant.MessageInfo.Message_18);
                return false;
            }

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[2];

			singleton.xlWorkSheet.Cells[2, "C"] = sectionNM;
			singleton.xlWorkSheet.Cells[2, "D"] = mtgProNM;
			singleton.xlWorkSheet.Cells[2, "E"] = proNM;

            for (int i = 0; i < nvcWork.AllKeys.Length; i++)
            {
				singleton.xlWorkSheet.Cells[i + 1, "A"] = nvcWork.Get(i);
            }

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputEdu(object[,] data)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Educationalist", true))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Educationalist" + Constant.MessageInfo.Message_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			singleton.xlRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[5, "B"], singleton.xlWorkSheet.Cells[5 + rowMaxIndex - 1, "G"]);
			singleton.xlRange.Value2 = data;

            //セル幅を広げる
			singleton.xlRange.EntireColumn.AutoFit();

			singleton.xlApp.DisplayAlerts = false;

            //セル結合
            cellMergeEdu(rowMaxIndex, 5);

			singleton.xlApp.DisplayAlerts = true;

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputEduChoice(object[,] data)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\EducationalistChoice", true))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\EducationalistChoice" + Constant.MessageInfo.Message_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			singleton.xlRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[6, "B"], singleton.xlWorkSheet.Cells[6 + rowMaxIndex - 1, "I"]);
			singleton.xlRange.Value2 = data;

            //セル幅を広げる
			singleton.xlRange.EntireColumn.AutoFit();

			singleton.xlApp.DisplayAlerts = false;

            //セル結合
            cellMergeEdu(rowMaxIndex, 6);

			singleton.xlApp.DisplayAlerts = true;

            return true;
        }

        /// <summary>
        /// 閾値マスタをExcelに出力する
        /// </summary>
        /// <param name="data"></param>
        /// <param name="protectFG"></param>
        public void LimitDataOutput(object[,] data, bool[,] colorFgArray, bool coloringFG, string typeCDStr, bool resinGroupOutputFG)
        {
            try
            {
				singleton.ProgressRate = 0;

				DateTime stDT = DateTime.Now;

                string filePath = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\TPL01_MasterLimit.xls";
                if (!workBookOpen(filePath, true))
                {                    
                    throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
                }

				singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

				singleton.xlWorkSheet.Cells[4, "D"] = System.DateTime.Now;
				singleton.xlWorkSheet.Cells[5, "D"] = Constant.EmployeeInfo.LoginServerNM;
				singleton.xlWorkSheet.Cells[6, "D"] = typeCDStr;

                int rowStartIndex = TPL01_DATASTART_ROW;
                int colStartIndex = 2;

                int rowMaxIndex = data.GetLength(0) - 1;
                int colMaxIndex = data.GetLength(1) - 1;

                // 通常の閾値/内規出力の場合は、『樹脂Gr』列を削除
                if (resinGroupOutputFG == false)
                {
                    Excel.Range myRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[10, "L"], singleton.xlWorkSheet.Cells[13, "L"]);
                    myRange.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                }

                //データ出力
                Excel.Range range = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex + colMaxIndex]);
                range.Value = data;

                //罫線を引く
				range = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex + colMaxIndex + 2]);

				if (rowMaxIndex > 0)
				{
                    WriteLine(range, true);
                }
				else
				{
                    WriteLine(range, false);
                }

				if (coloringFG)
				{
                    ChangeStyleHavingValCell(data, colorFgArray, range, rowStartIndex, colStartIndex, resinGroupOutputFG);
                }
	
				singleton.ProgressRate = 100;
				singleton.xlApp.Visible = true;

				System.Diagnostics.Debug.Print("エクセル出力タイム：" + (DateTime.Now - stDT).ToString());
            }
            catch(ApplicationException err)
            {
                throw err;
            }
        }

		private void ChangeStyleHavingValCell(object[,] data, bool[,] colorFgArray, Excel.Range range, int rowStartIndex, int colStartIndex, bool resinGroupOutputFG)
		{
			//背景色の変更とロック設定
			int colorPinkIndex = 38; int colorYellowIndex = 36;

            int paramRefCol = PARAMETER_REF_COL;
            if (resinGroupOutputFG) paramRefCol++;


            for (int row = range.Row; row < range.Row + range.Rows.Count; row++)
			{
				for (int targetCol = paramRefCol; targetCol <= paramRefCol + 14; targetCol += 2)
				{
					if (data.GetValue(row - rowStartIndex, targetCol - colStartIndex) != null)
					{
						Excel.Range judgeRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, targetCol]);
						judgeRange.Interior.ColorIndex = colorYellowIndex;

						Excel.Range targetRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, targetCol + 1]);
						targetRange.Interior.ColorIndex = colorPinkIndex;
					}
				}
				for(int targetCol = 0; targetCol < colorFgArray.GetLength(1); targetCol++)
				{
					if (colorFgArray[row - rowStartIndex, targetCol])
					{
						Excel.Range targetRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, targetCol + colStartIndex]);
						targetRange.Interior.ColorIndex = colorPinkIndex;
					}
				}
				
				Excel.Range editRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, 2]);
				editRange.Interior.ColorIndex = colorPinkIndex;

				editRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, 8]);
				editRange.Interior.ColorIndex = colorPinkIndex;

				editRange = ((Excel.Range)singleton.xlWorkSheet.Cells[row, paramRefCol + 16]);
				editRange.Interior.ColorIndex = colorPinkIndex;

				singleton.ProgressRate = (int)(((double)row / (double)(range.Row + range.Rows.Count)) * 100);
			}

			Excel.Range typeCellRange = ((Excel.Range)singleton.xlWorkSheet.Cells[6, 4]);
			//typeCellRange.Locked = true;

			//保護
			//singleton.xlWorkSheet.Protect(objMissing, objMissing, objMissing, objMissing, objMissing);
		}

		public void OutputParameterReportData(Dictionary<int, KeyValuePair<string, bool>> header, object[,] data, List<string> serverNMList, List<string> targetMachineList, List<int> targetParamList, string targetDT)
		{
			try
			{
				string filePath = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\TPL02_ParameterPerLotReport.xls";
				if (!workBookOpen(filePath, true))
				{
					throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
				}

				singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[2];

				int row = TPL02_DATASTART_ROW;

				foreach (string serverNM in serverNMList)
				{
					singleton.xlWorkSheet.Cells[row++, "A"] = serverNM;
				}

				row = TPL02_DATASTART_ROW;
				foreach (string targetMachine in targetMachineList)
				{
					singleton.xlWorkSheet.Cells[row++, "C"] = targetMachine;
				}

				row = TPL02_DATASTART_ROW;
				foreach (int targetParam in targetParamList)
				{
					singleton.xlWorkSheet.Cells[row++, "E"] = targetParam.ToString();
				}

				row = TPL02_DATASTART_ROW;
				singleton.xlWorkSheet.Cells[row, "G"] = targetDT;

				int rowStartIndex = TPL02_DATASTART_ROW;
				int colStartIndex = 1;

				int rowMaxIndex = data.GetLength(0) + TPL02_DATASTART_ROW -1;
				int colMaxIndex = data.GetLength(1);

				singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

				//ヘッダ出力
				int headerStartCol = TPL02_HEADER_COL;
				int headerMaxCol = TPL02_HEADER_COL + header.Count - 1;

				if (colMaxIndex > MAX_COL)
				{
					colMaxIndex = MAX_COL;
					headerMaxCol = MAX_COL;
					F100_MsgBox.Show(string.Format("Excelへの出力列数が{0}以上となる為、超過分は出力されません。\r\n別途条件を変更して出力して下さい。", MAX_COL));
				}

				Excel.Range paramHeaderRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[TPL02_HEADER_ROW, headerStartCol], singleton.xlWorkSheet.Cells[TPL02_HEADER_ROW, headerMaxCol]);

				string[] headerNameArray = new string[header.Count];

				for(int i = 0; i < header.Count; i++)
				{
					headerNameArray[i] = header[i].Key;
				}

				//header.Keys.ToSortableBindingList().CopyTo(headerNameArray, 0);
				paramHeaderRange.Value = headerNameArray;

				//for (int index = 0; index < header.Count; index++)
				//{
				//    Excel.Range dataColRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowStartIndex, headerStartCol + index], xlWorkSheet.Cells[rowMaxIndex, headerStartCol + index]);

				//    if (header.ToSortableBindingList()[index].Value == true) //indexで指定したヘッダ列が数値型なら
				//    {//カンマ区切り、小数点以下第2位まで
				//        dataColRange.NumberFormat = "#,##0.00_ ";
				//    }
				//    else
				//    {
				//        dataColRange.NumberFormat = "@";
				//    }

				//    dataColRange.Calculate();
					
				//    Marshal.ReleaseComObject(dataColRange);
					
				//}

				//データ出力
				Excel.Range dataRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowMaxIndex, colMaxIndex]);
				dataRange.Value = data;
				dataRange.EntireColumn.AutoFit();
				//dataRange.NumberFormatLocal = "G/標準";
				
				Marshal.ReleaseComObject(dataRange);


				singleton.xlApp.Visible = true;
			}
			catch (ApplicationException err)
			{
				throw err;
			}
		}

		public void OutputWBVerReportData(object[,] data, List<string> serverNMList, List<string> targetMachineList, List<int> targetParamList, string targetDT)
		{
			try
			{
				string filePath = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\TPL03_WBEquipVerReport.xls";
				if (!workBookOpen(filePath, true))
				{
					throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
				}

				singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[2];

				int row = TPL02_DATASTART_ROW;

				foreach (string serverNM in serverNMList)
				{
					singleton.xlWorkSheet.Cells[row++, "A"] = serverNM;
				}

				row = TPL02_DATASTART_ROW;
				foreach (string targetMachine in targetMachineList)
				{
					singleton.xlWorkSheet.Cells[row++, "C"] = targetMachine;
				}

				row = TPL02_DATASTART_ROW;
				foreach (int targetParam in targetParamList)
				{
					singleton.xlWorkSheet.Cells[row++, "E"] = targetParam.ToString();
				}

				row = TPL02_DATASTART_ROW;
				singleton.xlWorkSheet.Cells[row, "G"] = targetDT;

				int rowStartIndex = TPL02_DATASTART_ROW;
				int colStartIndex = 1;

				int rowMaxIndex = data.GetLength(0)+1;
				int colMaxIndex = data.GetLength(1);

				singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

				//データ出力
				Excel.Range dataRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowMaxIndex, colMaxIndex]);
				dataRange.Value = data;
				dataRange.EntireColumn.AutoFit();

				singleton.xlApp.Visible = true;
			}
			catch (ApplicationException err)
			{
				throw err;
			}
		}

        public List<PlmInfo> GetLimitData(string filePath)
        {
            List<PlmInfo> plmList = new List<PlmInfo>();

            if (!workBookOpen(filePath, true))
            {
                throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
            }

            singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

            int lastRowIndex = 0;
            int i = TPL01_DATASTART_ROW;
			while (((Excel.Range)singleton.xlWorkSheet.Cells[i, 2]).Text.ToString() != "")
            {
                lastRowIndex = i; 
                i++;
            }

            bool resinGroupInputFG = false;
            if(((Excel.Range)singleton.xlWorkSheet.Cells[TPL01_HEADER_ROW, "L"]).Text.ToString() == "樹脂Gr")
            {
                resinGroupInputFG = true;
            }
            int readColCt = 28;
            int paramRedCol = PARAMETER_REF_COL;
            if (resinGroupInputFG)
            {
                readColCt++;
                paramRedCol++;
            }
            

			object[,] dataList = (object[,])singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[TPL01_DATASTART_ROW, 2], singleton.xlWorkSheet.Cells[lastRowIndex, readColCt]).Value;

            for (int rowIndex = 1; rowIndex <= dataList.GetLength(0); rowIndex++)
            {
				if (dataList[rowIndex, paramRedCol + 15] == null) 
                {
                    continue;
                }
                PlmInfo plmInfo = new PlmInfo();
				plmInfo.MaterialCD = dataList[rowIndex, 1].ToString();
				plmInfo.QcParamNO = Convert.ToInt32(dataList[rowIndex,2]);
                plmInfo.ModelNM = dataList[rowIndex, 3].ToString();
                plmInfo.ChangeFG = true;
                
                object diceCT = dataList[rowIndex, 7];
                if (diceCT != null)
                {
                    plmInfo.DiceCT = diceCT.ToString();
                }

				object equipmentNo = dataList[rowIndex, 10];
				if (equipmentNo != null)
				{
					plmInfo.EquipmentNO = equipmentNo.ToString();
				}
				else
				{
					plmInfo.EquipmentNO = string.Empty;
				}

                if (resinGroupInputFG == true)
                {
                    object resinGroupCD = dataList[rowIndex, RESINGROUP_REF_COL];
                    if (resinGroupCD != null)
                    {
                        plmInfo.ResinGroupCD = resinGroupCD.ToString();
                    }
                    else
                    {
                        plmInfo.ResinGroupCD = string.Empty;
                    }
                }
                else
                {
                    plmInfo.ResinGroupCD = string.Empty;
                }

				object paramMAX = dataList[rowIndex, paramRedCol];
                if (paramMAX != null)
                {
                    plmInfo.ParameterMAX = Convert.ToDecimal(paramMAX);
                }
				object paramMIN = dataList[rowIndex, paramRedCol + 2];
                if (paramMIN != null)
                {
                    plmInfo.ParameterMIN = Convert.ToDecimal(paramMIN);
                }
				object paramVAL = dataList[rowIndex, paramRedCol + 4];
                if (paramVAL != null)
                {
                    plmInfo.ParameterVAL = paramVAL.ToString();
                }
				object qcLinePNT = dataList[rowIndex, paramRedCol + 6];
                if (qcLinePNT != null)
                {
                    plmInfo.QcLinePNT = Convert.ToInt32(qcLinePNT);
                }
				object qcLineMAX = dataList[rowIndex, paramRedCol + 8];
                if (qcLineMAX != null)
                {
                    plmInfo.QcLineMAX = Convert.ToDecimal(qcLineMAX);
                }
				object qcLineMIN = dataList[rowIndex, paramRedCol + 10];
                if (qcLineMIN != null)
                {
                    plmInfo.QcLineMIN = Convert.ToDecimal(qcLineMIN);
                }
                object paramGetUpperCond = dataList[rowIndex, paramRedCol + 12];
                if (paramGetUpperCond != null)
                {
                    plmInfo.ParamGetUpperCond = Convert.ToSingle(paramGetUpperCond);
                }
                object paramGetLowerCond = dataList[rowIndex, paramRedCol + 14];
                if (paramGetLowerCond != null)
                {
                    plmInfo.ParamGetLowerCond = Convert.ToSingle(paramGetLowerCond);
                }
                plmInfo.QCnumNO = 9;    //修正する必要有り
                plmInfo.ReasonVAL = dataList[rowIndex, paramRedCol + 15].ToString();

                plmList.Add(plmInfo);
            }
            return plmList;
        }


        /// <summary>
        /// ダイス個数を取得
        /// </summary>
        /// <returns></returns>
        public int GetDiceCount() 
        {
			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

			Excel.Range diceCTRange = (Excel.Range)singleton.xlWorkSheet.Cells[4, 6];
            return Convert.ToInt32(diceCTRange.Value);
        }

        /// <summary>
        /// 指定範囲に罫線を引く
        /// </summary>
        /// <param name="range"></param>
        private void WriteLine(Excel.Range range, bool isMultiRow) 
        {
            try
            {
                //罫線
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

                range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

				if (isMultiRow)
				{
					range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
					range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Weight = Excel.XlBorderWeight.xlThin;
					range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
				}

                range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
                range.Borders[Excel.XlBordersIndex.xlInsideVertical].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;

            }
            catch (Exception err)
            {
                throw err;
            }
        }

		public List<string> GetTypeList(string filePath)
		{
			List<string> typeList = new List<string>();

			if (!workBookOpen(filePath, true))
            {
                throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
            }

			singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];



            int lastRowIndex = 0;
			int i = TPL00_DATASTART_ROW;
			while (!string.IsNullOrEmpty(((Excel.Range)singleton.xlWorkSheet.Cells[i, 1]).Text.ToString()))
            {
                lastRowIndex = i; 
                i++;
            }

			object[,] dataList = (object[,])singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[TPL00_DATASTART_ROW, 1], singleton.xlWorkSheet.Cells[lastRowIndex, 1]).Value;

			for (int rowIndex = 1; rowIndex <= dataList.GetLength(0); rowIndex++)
			{
				object type = dataList[rowIndex, 1];
				
				typeList.Add(Convert.ToString(type).Trim());
			}

			return typeList;
		}

		public int GetProgressRate()
		{
			return singleton.ProgressRate;
		}

        /// <summary>
        /// PLCデバイスマスタをExcelに出力する
        /// </summary>
        /// <param name="data"></param>
        /// <param name="protectFG"></param>
        public void PlcFileConvDataOutput(object[,] data, bool[] colorFgArray)
        {
            try
            {
                singleton.ProgressRate = 0;

                DateTime stDT = DateTime.Now;

                string filePath = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\TPL04_MasterPlcFileConv.xls";
                if (!workBookOpen(filePath, true))
                {
                    throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
                }

                singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

                int rowStartIndex = TPL04_DATASTART_ROW;
                int colStartIndex = 2;

                int rowMaxIndex = data.GetLength(0) - 1;
                int colMaxIndex = data.GetLength(1) - 1;



                //データ出力
                Excel.Range range = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex + colMaxIndex]);
                range.Value = data;

                //罫線を引く
                range = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex + colMaxIndex]);

                if (rowMaxIndex > 0)
                {
                    WriteLine(range, true);
                }
                else
                {
                    WriteLine(range, false);
                }

                // セルの色変更
                int colorPinkIndex = 38;
                for (int targetCol = 0; targetCol < colorFgArray.Length; targetCol++)
                {
                    if (colorFgArray[targetCol])
                    {
                        Excel.Range targetRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex+targetCol], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex+targetCol]);
                        targetRange.Interior.ColorIndex = colorPinkIndex;
                    }
                    if (targetCol == 7)
                    {
                        Excel.Range targetRange = singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[rowStartIndex, colStartIndex + targetCol], singleton.xlWorkSheet.Cells[rowStartIndex + rowMaxIndex, colStartIndex + targetCol]);
                    }
                }

                singleton.ProgressRate = 100;
                singleton.xlApp.Visible = true;

                //System.Diagnostics.Debug.Print("エクセル出力タイム：" + (DateTime.Now - stDT).ToString());
            }
            catch (ApplicationException err)
            {
                throw err;
            }
        }

        public object[,] GetPlcFileConvData(string filePath)
        {
            List<PlcFileConv> pfcList = new List<PlcFileConv>();

            if (!workBookOpen(filePath, true))
            {
                throw new ApplicationException(filePath + Constant.MessageInfo.Message_18);
            }

            singleton.xlWorkSheet = (Excel.Worksheet)singleton.xlWorkBook.Worksheets[1];

            int lastRowIndex = 0;
            int i = TPL04_DATASTART_ROW;
            while (((Excel.Range)singleton.xlWorkSheet.Cells[i, 2]).Text.ToString() != "")
            {
                lastRowIndex = i;
                i++;
            }

            object[,] dataList = (object[,])singleton.xlWorkSheet.get_Range(singleton.xlWorkSheet.Cells[TPL04_DATASTART_ROW, 2], singleton.xlWorkSheet.Cells[lastRowIndex, 2 + 8]).Value;

            
            return dataList;
        }

    }
}
