using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

using ResinClassLibrary;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
//using System.Windows.Forms;

namespace Create_Recipe {

    /// <summary>
    /// 使用するPCにはExcelがインストールされていることが必要
    /// </summary>
    public static class OperateExcel {

        /// <summary>
        /// Excel先行ログの条件列検索する時に検索文字列を変換するために使う
        /// </summary>
        public static readonly Dictionary<string, string> conditions_dic = new Dictionary<string, string> {
            {"First_1","先行評価(配合比①)" },
            {"First_2","先行評価(配合比②)" },
            {"First_3","先行評価(配合比③)" },
            {"First_4","先行評価(配合比④)" },
            {"Trial","本番仮" },
            {"MP","本番確定" }
        };

        /// <summary>
        /// 先行ログから配合情報を取得する
        /// </summary>
        /// <param name="excel_path">参照するExcelファイルの絶対パス</param>
        public static MixInfo Get_SenkoLogMixInfo(string excel_path, InputInfo input_info) {

            //Excel.Applicationの新しいインスタンスを生成する
            Application xlApp = new Application();

            //Excelを表示するか
            xlApp.Visible = false;

            //ブックのオブジェクト
            Workbook xlBook = null;

            //シートのオブジェクト
            Worksheet xlSheet = null;

            int find_row = 0;

            var mi = new MixInfo();

            try {
                // 既存の Excel ブックを開く
                xlBook = xlApp.Workbooks.Open(excel_path, ReadOnly: true);

                xlSheet = xlBook.Sheets["Log"];

                int last_row = (int)xlSheet.Cells[xlSheet.Rows.Count, 1].End(XlDirection.xlUp).Row;

                //下の行から探していく
                for (int i = last_row; i >= 4; i--) {

                    //機種名の一致を確認
                    if (xlSheet.Cells[i, 1].Text == input_info.ProductName) {

                        //種類の一致を確認
                        if (xlSheet.Cells[i, 2].Text == input_info.MoldType) {

                            //波長ランクの一致を確認
                            if (xlSheet.Cells[i, 3].Text == input_info.WavelengthRank) {

                                //特殊指定の一致を確認
                                if (xlSheet.Cells[i, 4].Text == input_info.SpecialDesignation) {

                                    //条件(本番仮、本番確定、先行評価)の一致を確認
                                    string cond = conditions_dic[input_info.Conditions];

                                    if (cond.Contains("先行評価")) {
                                        cond = "先行評価";
                                    }

                                    if (xlSheet.Cells[i, 5].Text == cond) {

                                        find_row = i;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }

                if (find_row == 0) {
                    //見つからなかった場合
                    return null;
                }
                else {
                    //部材名取得
                    mi.ResinA.FormatName = (string)xlSheet.Cells[find_row, 47].Value;
                    mi.ResinB.FormatName = (string)xlSheet.Cells[find_row, 50].Value;
                    mi.Y1.FormatName = (string)xlSheet.Cells[find_row, 62].Value;
                    mi.Y2.FormatName = (string)xlSheet.Cells[find_row, 67].Value;
                    mi.Y3.FormatName = (string)xlSheet.Cells[find_row, 72].Value;
                    mi.R1.FormatName = (string)xlSheet.Cells[find_row, 77].Value;
                    mi.R2.FormatName = (string)xlSheet.Cells[find_row, 82].Value;
                    mi.R3.FormatName = (string)xlSheet.Cells[find_row, 87].Value;

                    //LotNo取得
                    mi.ResinA.LotNo = (string)xlSheet.Cells[find_row, 48].Value;
                    mi.ResinB.LotNo = (string)xlSheet.Cells[find_row, 51].Value;
                    mi.Filler.LotNo = (string)xlSheet.Cells[find_row, 57].Value;
                    mi.Toro.LotNo = (string)xlSheet.Cells[find_row, 60].Value;
                    mi.Y1.LotNo = (string)xlSheet.Cells[find_row, 63].Value;
                    mi.Y2.LotNo = (string)xlSheet.Cells[find_row, 68].Value;
                    mi.Y3.LotNo = (string)xlSheet.Cells[find_row, 73].Value;
                    mi.R1.LotNo = (string)xlSheet.Cells[find_row, 78].Value;
                    mi.R2.LotNo = (string)xlSheet.Cells[find_row, 83].Value;
                    mi.R3.LotNo = (string)xlSheet.Cells[find_row, 88].Value;

                    //配合量(100gベース)取得
                    var val = xlSheet.Cells[find_row, 49].Value;
                    if (val != null) {
                        mi.ResinA.BaseAmount = (decimal)val;
                    }

                    val = xlSheet.Cells[find_row, 52].Value;
                    if (val != null) {
                        mi.ResinB.BaseAmount = (decimal)val;
                    }

                    val = xlSheet.Cells[find_row, 58].Value;
                    if (val != null) {
                        mi.Filler.BaseAmount = (decimal)val;
                        mi.Filler.FormatName = (string)xlSheet.Cells[find_row, 56].Value;
                    }

                    val = xlSheet.Cells[find_row, 61].Value;
                    if (val != null) {
                        mi.Toro.BaseAmount = (decimal)val;
                        mi.Toro.FormatName = (string)xlSheet.Cells[find_row, 59].Value;
                    }


                    //先行評価(配合比②)の場合に配合２のデータを持ってくる
                    //照明では先行評価は配合比②までしかない
                    if (conditions_dic[input_info.Conditions] == "先行評価(配合比②)") {
                        //先行評価(配合比②)の場合
                        val = xlSheet.Cells[find_row, 65].Value;
                        if (val != null) {
                            mi.Y1.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 70].Value;
                        if (val != null) {
                            mi.Y2.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 75].Value;
                        if (val != null) {
                            mi.Y3.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 80].Value;
                        if (val != null) {
                            mi.R1.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 85].Value;
                        if (val != null) {
                            mi.R2.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 90].Value;
                        if (val != null) {
                            mi.R3.BaseAmount = (decimal)val;
                        }
                    }
                    else {
                        val = xlSheet.Cells[find_row, 64].Value;
                        if (val != null) {
                            mi.Y1.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 69].Value;
                        if (val != null) {
                            mi.Y2.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 74].Value;
                        if (val != null) {
                            mi.Y3.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 79].Value;
                        if (val != null) {
                            mi.R1.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 84].Value;
                        if (val != null) {
                            mi.R2.BaseAmount = (decimal)val;
                        }

                        val = xlSheet.Cells[find_row, 89].Value;
                        if (val != null) {
                            mi.R3.BaseAmount = (decimal)val;
                        }
                    }

                    //日時、条件など取得
                    mi.Conditions = (string)xlSheet.Cells[find_row, 5].Value;
                    mi.ResultOrInput = (string)xlSheet.Cells[find_row, 6].Value;
                    mi.EvaluationMethod = (string)xlSheet.Cells[find_row, 7].Value;
                    mi.Time = (DateTime)xlSheet.Cells[find_row, 8].Value;
                    mi.SenkoLogDataRow = find_row;
                }
            }
            catch (Exception ex) {
                throw;
            }
            finally {
                if (xlBook != null) {
                    //ブック閉じる
                    xlBook.Close(SaveChanges: false);

                    //Excelを終了する
                    xlApp.Quit();

                    //オブジェクトを解放する
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                }
            }

            return mi;
        }

        /// <summary>
        /// 機種ごとの使用A剤量を取得する
        /// </summary>
        /// <param name="excel_path"></param>
        /// <param name="input_info"></param>
        /// <returns></returns>
        public static int Get_ResinA_Amount(string excel_path, InputInfo input_info) {

            //Excel.Applicationの新しいインスタンスを生成する
            Application xlApp = new Application();
            //Excelを表示するか
            xlApp.Visible = false;

            //xlApplicationからWorkBooksを取得する
            Workbooks xlBooks = xlApp.Workbooks;

            Workbook xlBook = null;

            //Excelファイルのファイル名(filename.xlsm)
            string excel_filename = Path.GetFileName(excel_path);

            int amount = 0;
            try {
                // 既存の Excel ブックを開く
                xlBook = xlBooks.Open(excel_path);

                // マクロを実行する
                // 標準モジュール内のGet_ResinA_Amountメソッドに引数を渡して実行
                string macro_name = "Get_ResinA_Amount";
                amount = xlApp.Run($"{excel_filename}!{macro_name}", input_info.ProductName, input_info.TotalLFNum, input_info.SpecialDesignation);
            }
            catch (Exception ex) {
                //LogError(ex.Message);
            }
            finally {
                if (xlBook != null)
                    xlBook.Close();
            }

            //Excelを終了する
            xlApp.Quit();

            //オブジェクトを解放する
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBooks);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);

            return amount;
        }






        /// <summary>
        /// 照明HW用Excel先行ログをデータベースに移すためだけに使う(これが終わったら消していい）
        /// </summary>
        /// <param name="excel_path"></param>
        /// <param name="buzai_master_path"></param>
        /// <param name="start_row"></param>
        [Obsolete]
        public static void Write_ExcelSenkoLog_to_Databsase(string excel_path, string buzai_master_path, int start_row = 4, int end_row = 5000, System.Windows.Forms.Button btn = null) {

            //Excel.Applicationの新しいインスタンスを生成する
            Application xlApp = new Application();

            //Excelを表示するか
            xlApp.Visible = false;

            //ブックのオブジェクト
            Workbook xlBook = null;

            //シートのオブジェクト
            Worksheet xlSheet = null;


            int i;
            try {
                // 既存の Excel ブックを開く
                xlBook = xlApp.Workbooks.Open(excel_path, ReadOnly: true);

                xlSheet = xlBook.Sheets["Log"];

                //int end_row = 5000;//データ取得終了行
                for (i = start_row; i <= end_row; i++) {

                    var data = new ResinSQL.ResinSenkoLog_Data();

                    //日時、条件など取得
                    data.ExcelSenkoLogRow = i;
                    data.ProductName = xlSheet.Cells[i, 1].Text;//機種名

                    //機種名無ければ終わり
                    if (string.IsNullOrEmpty(data.ProductName)) { break; }

                    data.MoldType = xlSheet.Cells[i, 2].Text;//種類
                    data.WavelengthRank = xlSheet.Cells[i, 3].Text;//波長ランク
                    data.SpecialDesignation = xlSheet.Cells[i, 4].Text;//特殊指定
                    data.Conditions = (string)xlSheet.Cells[i, 5].Value;//条件
                    if (data.Conditions.Contains("本番確定")) {
                        data.Conditions = "MP";
                    }
                    else if (data.Conditions.Contains("本番仮")) {
                        data.Conditions = "Trial";
                    }
                    else {
                        data.Conditions = "First_1";
                    }

                    data.ResultOrInput = (string)xlSheet.Cells[i, 6].Value;//結果/投入指示
                    if (data.ResultOrInput.Contains("結果")) {
                        data.ResultOrInput = "Result";
                    }
                    else {
                        data.ResultOrInput = "Input";
                    }

                    data.EvaluationMethod = (string)xlSheet.Cells[i, 7].Value;//評価法
                    var dt = (DateTime)xlSheet.Cells[i, 8].Value;
                    data.InsertAt = new DateTimeOffset(dt, new TimeSpan(9, 0, 0));
                    data.InsertBy = (string)xlSheet.Cells[i, 9].Value;
                    data.Place = "TC";//CET照明HW
                    data.BaseAmount = 100;//照明は100g基準
                    data.MixTypeCode = "101";

                    string second = data.ProductName.Substring(1, 1);//機種名2文字目取得
                    if (second == "H") {
                        data.MixTypeCode = "102";
                    }

                    //部材情報・・本番確定、本番仮、先行評価①
                    var mi1 = new MixInfo();

                    //A剤取得
                    mi1.ResinA.FormatName = (string)xlSheet.Cells[i, 47].Value;
                    mi1.ResinA.LotNo = (string)xlSheet.Cells[i, 48].Value;
                    //配合量(100gベース)取得
                    var val = xlSheet.Cells[i, 49].Value;
                    if (val != null) {
                        mi1.ResinA.BaseAmount = (decimal)val;
                    }

                    //B剤取得
                    mi1.ResinB.FormatName = (string)xlSheet.Cells[i, 50].Value;
                    mi1.ResinB.LotNo = (string)xlSheet.Cells[i, 51].Value;
                    val = xlSheet.Cells[i, 52].Value;
                    if (val != null) {
                        mi1.ResinB.BaseAmount = (decimal)val;
                    }
                    //フィラー
                    mi1.Filler.FormatName = (string)xlSheet.Cells[i, 56].Value;
                    mi1.Filler.LotNo = (string)xlSheet.Cells[i, 57].Value;
                    val = xlSheet.Cells[i, 58].Value;
                    if (val != null) {
                        mi1.Filler.BaseAmount = (decimal)val;
                    }

                    //トロ
                    mi1.Toro.FormatName = (string)xlSheet.Cells[i, 59].Value;
                    mi1.Toro.LotNo = (string)xlSheet.Cells[i, 60].Value;
                    val = xlSheet.Cells[i, 61].Value;
                    if (val != null) {
                        mi1.Toro.BaseAmount = (decimal)val;
                    }

                    //黄色1
                    mi1.Y1.FormatName = (string)xlSheet.Cells[i, 62].Value;
                    mi1.Y1.LotNo = (string)xlSheet.Cells[i, 63].Value;
                    val = xlSheet.Cells[i, 64].Value;//先行評価①
                    if (val != null) {
                        mi1.Y1.BaseAmount = (decimal)val;
                    }

                    //黄色2
                    mi1.Y2.FormatName = (string)xlSheet.Cells[i, 67].Value;
                    mi1.Y2.LotNo = (string)xlSheet.Cells[i, 68].Value;
                    val = xlSheet.Cells[i, 69].Value;//先行評価①
                    if (val != null) {
                        mi1.Y2.BaseAmount = (decimal)val;
                    }

                    //黄色3
                    mi1.Y3.FormatName = (string)xlSheet.Cells[i, 72].Value;
                    mi1.Y3.LotNo = (string)xlSheet.Cells[i, 73].Value;
                    val = xlSheet.Cells[i, 74].Value;//先行評価①
                    if (val != null) {
                        mi1.Y3.BaseAmount = (decimal)val;
                    }

                    //赤1
                    mi1.R1.FormatName = (string)xlSheet.Cells[i, 77].Value;
                    mi1.R1.LotNo = (string)xlSheet.Cells[i, 78].Value;
                    val = xlSheet.Cells[i, 79].Value;//先行評価①
                    if (val != null) {
                        mi1.R1.BaseAmount = (decimal)val;
                    }

                    //赤2
                    mi1.R2.FormatName = (string)xlSheet.Cells[i, 82].Value;
                    mi1.R2.LotNo = (string)xlSheet.Cells[i, 83].Value;
                    val = xlSheet.Cells[i, 84].Value;//先行評価①
                    if (val != null) {
                        mi1.R2.BaseAmount = (decimal)val;
                    }

                    //赤3
                    mi1.R3.FormatName = (string)xlSheet.Cells[i, 87].Value;
                    mi1.R3.LotNo = (string)xlSheet.Cells[i, 88].Value;
                    val = xlSheet.Cells[i, 89].Value;//先行評価①
                    if (val != null) {
                        mi1.R3.BaseAmount = (decimal)val;
                    }


                    //部材情報の辞書作成
                    var dic = Buzai.Get_BuzaiInfo(buzai_master_path, 1);
                    //Format名称から正式部材情報取得
                    foreach (var mb in new List<MixBuzai> { mi1.ResinA, mi1.ResinB, mi1.Filler, mi1.Toro, mi1.TiO2, mi1.Y1, mi1.Y2, mi1.Y3, mi1.R1, mi1.R2, mi1.R3 }) {
                        if (mb != null && string.IsNullOrEmpty(mb.FormatName) == false && mb.BaseAmount >= 0 && mb.FormatName != "-") {
                            mb.Name = dic[mb.FormatName].Name;
                            mb.Fcode = dic[mb.FormatName].Fcode;
                            mb.NeedGroveBox = dic[mb.FormatName].NeedGroveBox;
                        }
                    }


                    //複製のためシリアライズ
                    string mi1_json = JsonConvert.SerializeObject(mi1);

                    //部材情報・・先行評価②
                    var mi2 = JsonConvert.DeserializeObject<MixInfo>(mi1_json);


                    //先行評価(配合比②)のデータを取得する
                    //照明では先行評価は配合比②までしかない
                    bool senko2 = false;
                    //黄色1
                    val = xlSheet.Cells[i, 65].Value;
                    if (val != null) {
                        mi2.Y1.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.Y1.BaseAmount = null;
                    }

                    //黄色2
                    val = xlSheet.Cells[i, 70].Value;
                    if (val != null) {
                        mi2.Y2.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.Y2.BaseAmount = null;
                    }

                    //黄色3
                    val = xlSheet.Cells[i, 75].Value;
                    if (val != null) {
                        mi2.Y3.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.Y3.BaseAmount = null;
                    }

                    //赤1
                    val = xlSheet.Cells[i, 80].Value;
                    if (val != null) {
                        mi2.R1.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.R1.BaseAmount = null;
                    }

                    //赤2
                    val = xlSheet.Cells[i, 85].Value;
                    if (val != null) {
                        mi2.R2.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.R2.BaseAmount = null;
                    }

                    //赤3
                    val = xlSheet.Cells[i, 90].Value;
                    if (val != null) {
                        mi2.R3.BaseAmount = (decimal)val;
                        senko2 = true;
                    }
                    else {
                        mi2.R3.BaseAmount = null;
                    }

                    //Name(NP-2063-1005とか)が無い or 配合比が無い場合は部材をnullにする
                    if (string.IsNullOrEmpty(mi1.ResinA.Name) || mi1.ResinA.BaseAmount == null) { mi1.ResinA = null; }
                    if (string.IsNullOrEmpty(mi1.ResinB.Name) || mi1.ResinB.BaseAmount == null) { mi1.ResinB = null; }
                    if (string.IsNullOrEmpty(mi1.Filler.Name) || mi1.Filler.BaseAmount == null) { mi1.Filler = null; }
                    if (string.IsNullOrEmpty(mi1.TiO2.Name) || mi1.TiO2.BaseAmount == null) { mi1.TiO2 = null; }
                    if (string.IsNullOrEmpty(mi1.Toro.Name) || mi1.Toro.BaseAmount == null) { mi1.Toro = null; }
                    if (string.IsNullOrEmpty(mi1.Y1.Name) || mi1.Y1.BaseAmount == null) { mi1.Y1 = null; }
                    if (string.IsNullOrEmpty(mi1.Y2.Name) || mi1.Y2.BaseAmount == null) { mi1.Y2 = null; }
                    if (string.IsNullOrEmpty(mi1.Y3.Name) || mi1.Y3.BaseAmount == null) { mi1.Y3 = null; }
                    if (string.IsNullOrEmpty(mi1.R1.Name) || mi1.R1.BaseAmount == null) { mi1.R1 = null; }
                    if (string.IsNullOrEmpty(mi1.R2.Name) || mi1.R2.BaseAmount == null) { mi1.R2 = null; }
                    if (string.IsNullOrEmpty(mi1.R3.Name) || mi1.R3.BaseAmount == null) { mi1.R3 = null; }

                    if (senko2) {
                        //Name(NP-2063-1005とか)が無い or 配合比が無い場合は部材をnullにする
                        if (string.IsNullOrEmpty(mi2.ResinA.Name) || mi2.ResinA.BaseAmount == null) { mi2.ResinA = null; }
                        if (string.IsNullOrEmpty(mi2.ResinB.Name) || mi2.ResinB.BaseAmount == null) { mi2.ResinB = null; }
                        if (string.IsNullOrEmpty(mi2.Filler.Name) || mi2.Filler.BaseAmount == null) { mi2.Filler = null; }
                        if (string.IsNullOrEmpty(mi2.TiO2.Name) || mi2.TiO2.BaseAmount == null) { mi2.TiO2 = null; }
                        if (string.IsNullOrEmpty(mi2.Toro.Name) || mi2.Toro.BaseAmount == null) { mi2.Toro = null; }
                        if (string.IsNullOrEmpty(mi2.Y1.Name) || mi2.Y1.BaseAmount == null) { mi2.Y1 = null; }
                        if (string.IsNullOrEmpty(mi2.Y2.Name) || mi2.Y2.BaseAmount == null) { mi2.Y2 = null; }
                        if (string.IsNullOrEmpty(mi2.Y3.Name) || mi2.Y3.BaseAmount == null) { mi2.Y3 = null; }
                        if (string.IsNullOrEmpty(mi2.R1.Name) || mi2.R1.BaseAmount == null) { mi2.R1 = null; }
                        if (string.IsNullOrEmpty(mi2.R2.Name) || mi2.R2.BaseAmount == null) { mi2.R2 = null; }
                        if (string.IsNullOrEmpty(mi2.R3.Name) || mi2.R3.BaseAmount == null) { mi2.R3 = null; }
                    }

                    //↓ここからデータベース書き込み処理
                    var sql = new ResinSQL(@"Data Source=vautom4\SQLEXpress; Initial Catalog=KODA;Connect Timeout=5; User ID=inline; Password=R28uHta");

                    //部材情報JSON作成
                    string buzai_json = "";
                    foreach (var mb in new List<MixBuzai> { mi1.ResinA, mi1.ResinB, mi1.Filler, mi1.TiO2, mi1.Toro, mi1.Y1, mi1.Y2, mi1.Y3, mi1.R1, mi1.R2, mi1.R3 }) {
                        if (mb != null) {
                            buzai_json += $@"{{""base-amount"": {mb.BaseAmount},""f-code"": ""{mb.Fcode}"",""name"": ""{mb.Name}"",""type"": ""{mb.Type}"",""lotno"": ""{mb.LotNo}"",""need-grovebox"": {mb.NeedGroveBox.ToString().ToLower()}}}";
                            buzai_json += ",";
                        }
                    }
                    buzai_json = buzai_json.Trim(',');//最後のカンマ除去
                    buzai_json = "[" + buzai_json + "]";//JSONのリストにする
                    data.Buzai = buzai_json;

                    //データベース書き込み
                    sql.Insert_SenkoLogData(data);

                    //先行評価②がある場合
                    if (senko2) {
                        buzai_json = "";
                        foreach (var mb in new List<MixBuzai> { mi2.ResinA, mi2.ResinB, mi2.Filler, mi2.TiO2, mi2.Toro, mi2.Y1, mi2.Y2, mi2.Y3, mi2.R1, mi2.R2, mi2.R3 }) {
                            if (mb != null) {
                                buzai_json += $@"{{""base-amount"": {mb.BaseAmount},""f-code"": ""{mb.Fcode}"",""name"": ""{mb.Name}"",""type"": ""{mb.Type}"",""lotno"": ""{mb.LotNo}"",""need-grovebox"": {mb.NeedGroveBox.ToString().ToLower()}}}";
                                buzai_json += ",";
                            }
                        }
                        buzai_json = buzai_json.Trim(',');//最後のカンマ除去
                        buzai_json = "[" + buzai_json + "]";//JSONのリストにする
                        data.Buzai = buzai_json;

                        data.Conditions = "First_2";
                        sql.Insert_SenkoLogData(data);
                    }

                    //ただ進捗を見たいだけ
                    if (btn != null) {
                        btn.Text = i.ToString();
                        btn.Update();
                    }
                }
            }
            catch (Exception ex) {
                throw;
            }
            finally {
                if (xlBook != null) {
                    //ブック閉じる
                    xlBook.Close(SaveChanges: false);

                    //Excelを終了する
                    xlApp.Quit();

                    //オブジェクトを解放する
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                }
            }

        }

    }
}
