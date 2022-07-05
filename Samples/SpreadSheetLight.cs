using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SpreadSheetLight関連に使用します
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace Samples
{
    class Spreadsheetlight
    {
        static void Main(string[] args)
        {
            ///////////////////////////////////////
            //書込み
            ///////////////////////////////////////
            ///
            //SLDocument sl = new SLDocument();

            ////
            //// Bool 文字列・数値の入力 例です
            ////

            //// A1に Bool型のTrueを設定します  (Excel:TRUE、LibreOffice:1)
            //sl.SetCellValue("A1", true);

            //// B3に 数値(円周率)を入力します
            //sl.SetCellValue("B3", 3.14159);

            //// 4行目の2列目(B4)に、数値を文字列として入力します
            //sl.SetCellValueNumeric(4, 2, "3.14159");

            //// C6に 文字列を入力します
            //sl.SetCellValue("C6", "This is at C6!");

            //// I6に & や <> など記号付の文字列を入力します
            //sl.SetCellValue("I6", "Dinner & Dance costs < $10");

            ////
            //// 数式の入力 例です
            ////
            //sl.SetCellValue(7, 3, "=SUM(A2:T2)");

            //// 「'==」のように頭に'シングルクォートをつけると文字列として認識されます

            ////
            //// ループを使ったセルへの入力例です
            ////

            //// 2行目の1列目(A1)から、2行目の20列目(T1) に1～20の値を入力します。
            //for (int i = 1; i <= 20; ++i) sl.SetCellValue(2, i, i);

            ////
            //// 参照・範囲指定方法です
            ////

            //// セルの参照やセルの範囲は、SLConvertクラスを使用します
            //sl.SetCellValue(SLConvert.ToCellReference(7, 4), string.Format("=SUM({0})", SLConvert.ToCellRange(2, 1, 2, 20)));

            ////
            //// 日付の入力とスタイルの設定方法です
            ////

            //// 日付にはフォーマットコードが必要です。
            //// 指定しない場合、浮動小数点数のように表示されてしまいます

            //// C8セルに日付を入力します。(日時はデタラメ)
            //sl.SetCellValue("C8", new DateTime(3141, 5, 9));

            //// スタイルを作成して、FormatCodeを設定します。
            //SLStyle style = sl.CreateStyle();
            //style.FormatCode = "d-mmm-yyyy";

            //// C8セルにスタイルを設定します。
            //sl.SetCellStyle("C8", style);

            ////
            //// 数値へのスタイルの設定方法です
            ////

            //// 9行4列目(D4セル)に数値を入力します(数値はデタラメ)
            //sl.SetCellValue(9, 4, 456.123789);

            //// 上記セルにスタイルを設定します。
            //// FormatCodeプロパティを使用するだけなので、新しいSLStyleを作成する必要はありません。
            //style.FormatCode = "0.000%";
            //sl.SetCellStyle(9, 4, style);

            //// ファイル名を指定して保存します。
            //sl.SaveAs("HelloWorld.xlsx");

            //// 終了待ちです。
            //Console.WriteLine("End of program");
            //Console.ReadLine();

            ///////////////////////////////////////
            //読込
            ///////////////////////////////////////
            ///

            //SLDocument sl = new SLDocument("HelloWorld.xlsx");

            //var str = sl.GetCellValueAsString("A1");

            //Console.WriteLine(str + "\r\n");

            SLDocument sl = new SLDocument("C:\\temp\\ninjosystem\\system\\resources\\custmRpt\\ENERGY.xlsx");

            sl.SaveAs("C:\\temp\\spreadsheetlight.xlsx");

            Console.WriteLine("End of program");
            Console.ReadLine();

        }
    }
}