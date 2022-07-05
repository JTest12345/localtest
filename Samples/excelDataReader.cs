using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

namespace Samples
{
    class excelDataReader
    {
        static void _Main()
        {
            var filepath = @"C:\Oskas\procmaster\shomei\ver9\shinkishutenkai\220330@Ver.9 新機種展開 仕様一覧【5版】.xlsm";
            var sheetname = "一覧";
            AsDataSetSample(filepath, sheetname);
        }

        private static void AsDataSetSample(string filepath, string sheetname)
        {
            //ファイルの読み取り開始
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else if (filepath.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else
                {
                    Console.WriteLine("サポート対象外の拡張子です。");
                    return;
                }

                //全シート全セルを読み取り
                var dataset = reader.AsDataSet();

                //シート名を指定
                var worksheet = dataset.Tables[sheetname];

                if (worksheet is null)
                {
                    Console.WriteLine("指定されたワークシート名が存在しません。");
                }
                else
                {
                    //セルの入力文字を読み取り
                    for (var row = 0; row < worksheet.Rows.Count; row++)
                    {
                        for (var col = 0; col < worksheet.Columns.Count; col++)
                        {
                            Console.WriteLine(worksheet.Rows[row][col]);
                        }
                    }
                }


                Console.WriteLine(worksheet.Rows[24][3]);

                reader.Close();
            }
        }
    }
}
