using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using ProcMasterIF.Coj.mst000002;
using Newtonsoft.Json;
using Oskas;


namespace Samples
{
    class excelDataReader
    {
        static void _Main()
        {
            //var filepath = @"C:\Oskas\procmaster\shomei\ver9\shinkishutenkai\220330@Ver.9 新機種展開 仕様一覧【5版】.xlsm";
            //var sheetname = "一覧";
            var filepath = @"C:\Users\jn-wtnb\Desktop\その他\対象機種の部材使用数.xlsx";
            //var sheetname = "完成品";
            var sheetname = "半完成品";
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
                    for (var row = 1; row < worksheet.Rows.Count; row++)
                    {
                        for (var col = 0; col < worksheet.Columns.Count; col++)
                        {
                            Console.WriteLine(worksheet.Rows[row][col]);
                        }

                        var kishu = worksheet.Rows[row][0].ToString();
                        //var FilePath = @"C:\Users\jn-wtnb\Desktop\その他\4m\kansei\" + kishu.Substring(0, kishu.Length - 2) + "_m.json";
                        var FilePath = @"C:\Users\jn-wtnb\Desktop\その他\4m\hankan\" + kishu.Substring(0, kishu.Length - 2) + "_m.json";
                        var json = JsonConvert.DeserializeObject<Root>(JsonFileReader(FilePath));
                        var newdatalist = new List<KouteisagyouPropData>();
                        foreach (var item in json.cejObject.coList[4].props.propdata)
                        {
                            var buzaiitem = JsonConvert.DeserializeObject<KouteisagyouPropData>(item.ToString());
                            if (buzaiitem.buzai.Count > 0)
                            {
                                foreach (var buzaikodo in buzaiitem.buzai)
                                {
                                    Console.WriteLine(buzaikodo.buzaikodo);
                                    if (worksheet.Rows[row][3].ToString() == buzaikodo.buzaikodo)
                                    {
                                        var suryo = (double)worksheet.Rows[row][4];
                                        buzaikodo.siyouryou = suryo.ToString("F10");
                                    }
                                }
                            }
                            newdatalist.Add(buzaiitem);
                        }

                        json.cejObject.coList[4].props.propdata = new List<object>(newdatalist);
                        var msg = "";
                        CommonFuncs.JsonFileWriter(FilePath, json, ref msg);
                    }
                    
                }

                reader.Close();
            }

            Console.ReadKey();
        }

        /////////////////////////////
        // json file 読み込み用
        /////////////////////////////
        static public string JsonFileReader(string FilePath)
        {
            StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("utf-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
    }
    
}
