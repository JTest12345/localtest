using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace GEICS
{
    public class ExcelControl : IDisposable
    {
        public Excel.ApplicationClass xlApp = null;
        public Excel.Workbook xlWorkBook = null;
        private Excel.Worksheet xlWorkSheet = null;
        private Excel.Range xlRange = null;
        private string currentBookName = string.Empty;
        public bool nonCloseFlg = false;

        /*}
        private static ExcelControl ec = null;
        /// <summary>
        /// インスタンス生成
        /// </summary>
        /// <returns>ExcelControlのインスタンス</returns>
        public static ExcelControl CreateInstance()
        {
            //			if(ec==null)
            //			{
            ec = new ExcelControl();
            //エクセル起動
            ec.xlApp = new Excel.ApplicationClass();
            //			}
            return ec;
        }

        /// <summary>
        /// ワークブックを開く
        /// </summary>
        /// <param name="wBookName">ワークブック名</param>
        /// <param name="isReadOnly">読取専用フラグ</param>
        /// <returns>結果</returns>
        public bool ConnectWorkBook(string wBookName, bool isReadOnly)
        {
            //ワークブック引数設定
            string myFileName = wBookName;		            // ファイル名
            object myUpdateLinks = 3;				        // ファイル内のリンクの更新方法 　3:外部参照、リモート参照共に更新されます
            object myReadOnly = true;                       // 読取り専用設定　true:読取専用
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

            try
            {
                //ワークブック開く
                xlWorkBook = xlApp.Workbooks.Open(myFileName, myUpdateLinks, myReadOnly, myFormat, myPassword, myWriteResPassword,
                    myIgnoreReadOnlyRecommended, myOrigin, myDelimiter, myEditable, myNotify, myConverter, myAddToMru);
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }

        }
        /// <summary>
        /// デストラクタもどき
        /// M$は、必ず通る事を保証してませんが、念のため。
        /// </summary>
        ~ExcelControl()
        {
            this.Dispose();
        }
        */

        #region IDisposable メンバ

        public void Dispose()
        {

            try
            {
                if (xlApp != null)
                {
                    if (this.xlRange != null)
                    {
                        while (Marshal.ReleaseComObject(this.xlRange) > 0) ;
                        this.xlRange = null;
                    }

                    if (this.xlWorkSheet != null)
                    {
                        while (Marshal.ReleaseComObject(this.xlWorkSheet) > 0) ;
                        this.xlWorkSheet = null;
                    }

                    if (this.xlApp != null)
                    {
                        foreach (Excel.Workbook exBook in this.xlApp.Workbooks)
                        {
                            if (exBook.Name.ToUpper().Substring(0, 4) != "BOOK")
                            {
                                exBook.Close(false, objMissing, false);

                                while (Marshal.ReleaseComObject(exBook) > 0) ;
                            }
                        }
                    }


                    if (this.xlWorkBook != null)
                    {
                        if (!this.nonCloseFlg)
                        {
                            this.xlWorkBook.Close(false, objMissing, false);
                        }
                        while (Marshal.ReleaseComObject(this.xlWorkBook) > 0) ;
                        this.xlWorkBook = null;
                    }



                    if (this.xlApp != null)
                    {

                        if (!this.nonCloseFlg)
                        {
                            this.xlApp.Quit();
                        }
                        else
                        {
                            this.xlApp.Visible = true;
                        }

                        while (Marshal.ReleaseComObject(this.xlApp) > 0) ;
                        this.xlApp = null;
                    }
                }
                GC.Collect();
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            {
                xlApp = null;
                GC.Collect();
            }
        }

        #endregion

        object objMissing = System.Reflection.Missing.Value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExcelControl()
        {
            xlApp = new Excel.ApplicationClass();
        }

        /// <summary>
        /// Excelブックを開きます。
        /// </summary>
        /// <param name="wBookName"></param>
        /// <returns></returns>
        private bool workBookOpen(string wBookName)
        {
            //ワークブック引数設定
            string myFileName = wBookName;		            // ファイル名
            object myUpdateLinks = 3;				        // ファイル内のリンクの更新方法 　3:外部参照、リモート参照共に更新されます
            object myReadOnly = true;                       // 読取り専用設定　true:読取専用
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
                if (currentBookName == wBookName)
                {
                    return true;
                }
                //ワークブック開く
                this.xlWorkBook = xlApp.Workbooks.Open(myFileName, myUpdateLinks, myReadOnly, myFormat, myPassword, myWriteResPassword,
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
        private void cellMerge(int maxRowIndex)
        {
            int startRowIndex = 6;

            Excel.Range startCell = (Excel.Range)xlWorkSheet.Cells[6, "C"];
            Excel.Range endCell = null;

            for (int i = 0; i <= maxRowIndex; i++)
            {
                endCell = (Excel.Range)xlWorkSheet.Cells[6 + i, "C"];
                if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
                {
                    endCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "C"];

                    //作業
                    xlRange = xlWorkSheet.get_Range(startCell, endCell);
                    xlRange.Merge(false);

                    startCell = (Excel.Range)xlWorkSheet.Cells[6 + i, "C"];


                    //レベル
                    Excel.Range tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "B"];
                    Excel.Range tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "B"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //NASCA作業名
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "D"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "D"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //教育予定
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "J"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "J"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //見直し周期
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "K"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "K"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //認定日
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "M"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "M"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //認定者
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "N"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[6 + i - 1, "N"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    startRowIndex = 6 + i;
                }
            }
        }

        /// <summary>セル結合 縦</summary>
        private void cellMergeEdu(int maxRowIndex, int startRowIndex)
        {
            int fixStartRowIndex = startRowIndex;

            Excel.Range startCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex, "D"];
            Excel.Range endCell = null;
            Excel.Range levelStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "E"];
            Excel.Range levelEndCell = null;

            for (int i = 0; i <= maxRowIndex; i++)
            {
                endCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
                levelEndCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
                {
                    //レベル
                    levelEndCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i - 1, "E"];
                    xlRange = xlWorkSheet.get_Range(levelStartCell, levelEndCell);
                    xlRange.Merge(false);

                    startCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
                    levelStartCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                }
                else
                {
                    if ((levelEndCell.Value == null) || (levelStartCell.Value.ToString() != levelEndCell.Value.ToString()))
                    {
                        //レベル
                        levelEndCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i - 1, "E"];
                        xlRange = xlWorkSheet.get_Range(levelStartCell, levelEndCell);
                        xlRange.Merge(false);

                        levelStartCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "E"];
                    }
                }
            }


            startCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex, "D"];
            endCell = null;

            for (int i = 0; i <= maxRowIndex; i++)
            {
                endCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "D"];
                if ((endCell.Value == null) || (startCell.Value.ToString() != endCell.Value.ToString()))
                {
                    endCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i - 1, "D"];

                    //スキルマップ名
                    xlRange = xlWorkSheet.get_Range(startCell, endCell);
                    xlRange.Merge(false);

                    startCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i, "D"];


                    //係
                    Excel.Range tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "B"];
                    Excel.Range tmpEndCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i - 1, "B"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    //様式番号
                    tmpStartCell = (Excel.Range)xlWorkSheet.Cells[startRowIndex, "C"];
                    tmpEndCell = (Excel.Range)xlWorkSheet.Cells[fixStartRowIndex + i - 1, "C"];
                    xlRange = xlWorkSheet.get_Range(tmpStartCell, tmpEndCell);
                    xlRange.Merge(false);

                    startRowIndex = fixStartRowIndex + i;
                }
            }
        }

        /// <summary>ExcelSheetのデータ取込</summary>
        public object[,] DataRead(string wBookName)
        {
            if (!workBookOpen(wBookName))
            {
                System.Windows.Forms.MessageBox.Show(wBookName + SystemMessage.MESSAGE_18);
                return null;
            }

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            //保護解除
            xlWorkSheet.Unprotect(objMissing);

            //データ取得
            Excel.Range endCell = xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, objMissing);
            object[,] xlData = (object[,])(((Excel.Range)xlWorkSheet.get_Range("A1", endCell)).Value);

            //破棄
            while (Marshal.ReleaseComObject(endCell) > 0) ;
            endCell = null;

            return xlData;
        }

        /// <summary>
        /// ExcelSheetに引数のアドレス直下に保存されている画像(連番.png)をExcelに貼り付けていく
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ImageOutput(string Address,string sType,string sFileNM)
        {
            string sFileFullName = "";
            bool fnew=false;//初めてのExcel出力の場合

            foreach (string sfullpath in System.IO.Directory.GetFiles(Address))
            {
                string swfilenm = sfullpath.Substring(Address.Length+1, sfullpath.Length - Address.Length-1);      //ファイル名取得
                //同ライン/同工程/同日のExcelが出力されている場合
                if (swfilenm.Contains(sFileNM) == true)
                {
                    fnew = true;//既に同条件で出力されている。
                    break;
                }
            }

            if (fnew == true)
            {
                sFileFullName = Address + "\\" + sFileNM;
            }
            else
            {
                sFileFullName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\TPL_GRAPH.xls";
            }

            object[,] xlData = DataRead(sFileFullName);

            int rowMaxIndex = xlData.GetLength(0);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            for(int i=0;;i++){
                if (((Excel.Range)xlWorkSheet.Cells[1, (i*14)+1]).Value == null)
                {
                    ((Excel.Range)xlWorkSheet.Cells[1, (i * 14) + 1]).Value = sType;
                    break;
                }
            }

            int nNum = 0, x = 0, y = 0;
            foreach (string sfullpath in System.IO.Directory.GetFiles(Address))
            {
                string swfilenm = sfullpath.Substring(Address.Length+1, sfullpath.Length - Address.Length-1);      //ファイル名取得
                //画像ファイルなら
                if (swfilenm.Contains(".png") == true)
                {
                    if (nNum % 2 == 0)//左側
                    {
                        x = 0;
                    }
                    else//右側
                    {
                        x = 505;
                    }
                    y = (205 * (nNum / 2) + 5);

                    xlWorkSheet.Shapes.AddPicture(sfullpath, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, x, y, 500, 200);
                    nNum++;
                }
            }
            
            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutput(DateTime dtOutput, string typeNM, string lineNM, dsTmPLMEx dsTmPLMEx)
        {
            string sfileName = SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Master.xls";
            object[,] xlData = DataRead(sfileName);

            int rowMaxIndex = xlData.GetLength(0);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            xlWorkSheet.Cells[4, "D"] = dtOutput;
            xlWorkSheet.Cells[5, "D"] = typeNM;

            if (lineNM =="高効率ライン")
            {
                xlWorkSheet.Cells[6, "D"] = "高効率ライン";
            }
            else
            {
                xlWorkSheet.Cells[6, "D"] = "自動化ライン";
            }

            int nFix=14;
            int nQcParamNo;
            for (int k = nFix; k <= rowMaxIndex; k++)
            {

                try{
                    nQcParamNo = Convert.ToInt32(xlData[k, 2]);
                }catch{
                    nQcParamNo=0;
                }

                if (nQcParamNo != 0)
                {
                    for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
                    {
                        if (dsTmPLMEx.TvPLMEx[i].QcParam_NO == nQcParamNo)
                        {
                            try
                            {
                                xlWorkSheet.Cells[k, "A"] = dsTmPLMEx.TvPLMEx[i].Rev;  //Rev
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "A"] = "";  //Rev
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "H"] = dsTmPLMEx.TvPLMEx[i].Parameter_MAX;  //管理値上限
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "H"] = "";  //管理値上限
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "J"] = dsTmPLMEx.TvPLMEx[i].Parameter_MIN;  //管理値下限
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "J"] = "";  //管理値下限
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "L"] = dsTmPLMEx.TvPLMEx[i].Parameter_VAL.Trim();  //文字列
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "L"] = "";  //文字列
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "N"] = dsTmPLMEx.TvPLMEx[i].QcLine_PNT;     //打点数
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "N"] = "";     //打点数
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "P"] = dsTmPLMEx.TvPLMEx[i].QcLine_MAX;     //管理値上限
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "P"] = "";     //管理値上限
                            }

                            try
                            {
                                xlWorkSheet.Cells[k, "R"] = dsTmPLMEx.TvPLMEx[i].QcLine_MIN;     //管理値下限
                            }
                            catch
                            {
                                xlWorkSheet.Cells[k, "R"] = "";     //管理値下限
                            }
                        }
                    }
                }
            }
            //保護
            xlWorkSheet.Protect(objMissing, objMissing, objMissing, objMissing, objMissing);

            return true;
        }


        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutput(object[,] data, string startDT, string endDT)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Result"))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Result" + SystemMessage.MESSAGE_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            xlWorkSheet.Cells[9, "F"] = startDT + "～" + endDT + "までの登録データを取得";

            xlRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[11, "B"], xlWorkSheet.Cells[11 + rowMaxIndex - 1, "G"]);
            xlRange.Value2 = data;

            //セル幅を広げる
            xlRange.EntireColumn.AutoFit();

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputTemp(string sectionNM, string mtgProNM, string proNM, NameValueCollection nvcWork)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\SkillMapTemp"))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\SkillMapTemp" + SystemMessage.MESSAGE_18);
                return false;
            }


            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[2];

            xlWorkSheet.Cells[2, "C"] = sectionNM;
            xlWorkSheet.Cells[2, "D"] = mtgProNM;
            xlWorkSheet.Cells[2, "E"] = proNM;

            for (int i = 0; i < nvcWork.AllKeys.Length; i++)
            {
                xlWorkSheet.Cells[i + 1, "A"] = nvcWork.Get(i);
            }


            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputEdu(object[,] data)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Educationalist"))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\Educationalist" + SystemMessage.MESSAGE_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            xlRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[5, "B"], xlWorkSheet.Cells[5 + rowMaxIndex - 1, "G"]);
            xlRange.Value2 = data;

            //セル幅を広げる
            xlRange.EntireColumn.AutoFit();

            xlApp.DisplayAlerts = false;

            //セル結合
            cellMergeEdu(rowMaxIndex, 5);

            xlApp.DisplayAlerts = true;

            return true;
        }

        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputEduChoice(object[,] data)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\EducationalistChoice"))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\EducationalistChoice" + SystemMessage.MESSAGE_18);
                return false;
            }

            int rowMaxIndex = data.GetLength(0);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            xlRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[6, "B"], xlWorkSheet.Cells[6 + rowMaxIndex - 1, "I"]);
            xlRange.Value2 = data;

            //セル幅を広げる
            xlRange.EntireColumn.AutoFit();

            xlApp.DisplayAlerts = false;

            //セル結合
            cellMergeEdu(rowMaxIndex, 6);

            xlApp.DisplayAlerts = true;

            return true;
        }

        /*
        /// <summary>ExcelSheetにデータ出力</summary>
        public bool DataOutputWork(string sectionNM, string processNM, string mateGrpNM, List<ExcelInfo> excelDataList)
        {
            if (!workBookOpen(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\WorkCheck"))
            {
                System.Windows.Forms.MessageBox.Show(SLCommonLib.Commons.Common.GetAppFolderPath() + @"\template\WorkCheck" + SystemMessage.MESSAGE_18);
                return false;
            }

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];

            //課
            xlWorkSheet.Cells[5, "B"] = sectionNM;
            //品種
            xlWorkSheet.Cells[5, "C"] = mateGrpNM;
            //工程
            xlWorkSheet.Cells[5, "D"] = processNM;

            for (int i = 0; i < excelDataList.Count - 1; i++)
            {
                Excel.Worksheet formatSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];
                Excel.Worksheet copyToSheet = (Excel.Worksheet)xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];
                formatSheet.Copy(objMissing, copyToSheet);
            }

            for (int i = 0; i < excelDataList.Count; i++)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1 + i];

                int rowMaxIndex = excelDataList[i].ExcelData.GetLength(0);
                int columnMaxIndex = excelDataList[i].ExcelData.GetLength(1);

                xlRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[8, 2], xlWorkSheet.Cells[8 + rowMaxIndex - 1, 2 + columnMaxIndex - 1]);
                xlRange.Value2 = excelDataList[i].ExcelData;

                //セル幅を広げる
                xlRange.EntireColumn.AutoFit();
            }

            return true;
        }
        */
    }
}
