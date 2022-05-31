using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FileIf;
using Oskas;

namespace FIFDebugTools
{
    public class MagCupDebug
    {
        string globalmsg;
        string query;
        MySQL mysql = new MySQL();
        static string file = @"C:\Oskas\magcupdir\SCT\M01001\in\xxxx.csv";
        static string iniDebugDbfile = @"C:\Oskas\debug\iniDebugDb.csv";

        public bool InitMagDebugDbArea(Mcfilesys fs, ref string sqlscr)
        {

            //*************************************************************************
            // ■ デバッグ用領域イニシャル処理
            //　　DBのデバッグ領域をイニシャル処理する為、MySQLにSQLを投げられるようにする
            //*************************************************************************
  
            ///////////////////////////////////////////////////////////
            //// ◆ MagCupのfilesystemクラスの実体化
            ///////////////////////////////////////////////////////////
            //var fs = new mcfilesys();
            //initMagCupFileSystem(fs);


            /////////////////////////////////////////////////////////
            // ◆ current_magテーブルのシミュレーション領域を初期化
            /////////////////////////////////////////////////////////

            List<string> lists = new List<string>();

            StreamReader sr = new StreamReader(iniDebugDbfile);
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    lists.Add(line);
                }
            }


            var maglist = lists[0].Split(',');
            var productList = lists[1].Split(',');
            var lotnoList = lists[2].Split(',');
            var lpcodeList = lists[3].Split(',');
            var macnoList = lists[4].Split(',');
            var ioList = lists[5].Split(',');
            var valbsList = lists[6].Split(',');
            sqlscr = "";

            for (int i = 0; i < maglist.Length; i++)
            {
                var cm = new Current_mag
                {
                    Magno = maglist[i],
                    Product = productList[i],
                    Lotno = lotnoList[i],
                    Lpcode = lpcodeList[i],
                    Cstmproduct = "NA",
                    Cstmlotno = "NA",
                    Macno = macnoList[i],
                    Io = ioList[i],
                    Valbs = int.Parse(valbsList[i]),
                    Henkaten = ""
                };

                query = $"UPDATE current_mag SET last_pcode='{cm.Lpcode}', product='{cm.Product}', lotno='{cm.Lotno}', macno='{cm.Macno}', in_out='{cm.Io}', val_bs='{cm.Valbs}' WHERE magno='{cm.Magno}'";
                //sqlscr += query + "\r\n";

                if (!mysql.SqlTask_Write(i.ToString(), fs.mci.ConnectionStrings[0], query, ref globalmsg))
                {
                    sqlscr += "[SQL: Fail] " + query + "\r\n" + globalmsg + "\r\n";
                    return false;
                }
                else
                {
                    //sqlscr += "[SQL: Pass] " + query + "\r\n";
                }
                Thread.Sleep(100);
            }
            sqlscr += "[SQL: Pass] マガジンテーブル初期化しました" + "\r\n";

            /////////////////////////////////////////////////////////
            // ◆ Process_resultsテーブルのデバッグ領域を初期化
            /////////////////////////////////////////////////////////
            ///
            // ◇製品をテーブルからすべて削除
            List<string> delProductList = new List<string>();

            foreach (string product in productList)
            {
                query = $"DELETE FROM `Process_results` WHERE product='{product}'";
                if (!delProductList.Contains(product))
                {
                    if (!mysql.SqlTask_Write("0", fs.mci.ConnectionStrings[0], query, ref globalmsg))
                    {
                        sqlscr += $"[SQL: Fail]デバッグ製品{product}を実績テーブルから削除できませんでした" + globalmsg + "\r\n";
                        return false;
                    }
                    else
                    {
                        sqlscr += $"[SQL: Pass] デバッグ製品{product}を実績テーブルから削除しました" + "\r\n";
                        delProductList.Add(product);
                    }

                    Thread.Sleep(100);
                }
            }

            return true;
        }


        public void initMagCupFileSystem(Mcfilesys fs)
        {
            // MagCupのイニシャルファイルを読込
            var mci = new Magcupini();
            string msg = "";
            if (!mci.GetMugCupYamlValues(ref msg))
            {
                Console.WriteLine(msg);
            }

            // MagCupDBのデバック領域をイニシャル
            //【デバック用製品・ロット】
            // Product：CLUxxxxxxxxxx-xxx
            // Lotno：2XXXXXXXX1-2XXXXXXXX9 
            //【デバック領域】
            // currmag：Magno: 99990-99999
            //
            //【デバック用製品工程テーブル】
            // procmaster：Pno(1), Pcat(DB), Pcode(DB001)
            // procmaster：Pno(2), Pcat(WB), Pcode(WB001)
            // procmaster：Pno(3), Pcat(SDR), Pcode(SDR001)
            // procmaster：Pno(4), Pcat(SCT), Pcode(SCT001)
            // macmaster：Macno(M00001, M00002, M00003), Mparam(CLUxxx_DB)
            // macmaster：Macno(M00010, M00020, M00030), Mparam(CLUxxx_WB)
            // macmaster：Macno(M00100, M00200, M00300), Mparam(CLUxxx_SDR)
            // macmaster：Macno(M01000, M02000, M03000), Mparam(CLUxxx_SCT)
            //

            // MagCupのfilesystemクラスの実体化
            // mcfilesys fs = new mcfilesys();

            fs.filepath = file; //設備タスク対象ファイル名
            fs.lowerfilepath = file.ToLower(); //対象ファイル小文字
            fs.Upperfilepath = file.ToUpper(); //対象ファイル大文字
            fs.tmpfilepath = fs.filepath.Replace(@"\in\", @"\temp\"); //TMPフォルダパス
            fs.key = Tasks_Common.FindKey(mci, fs.lowerfilepath); //ファイル名から抽出したタスクキー(_min1.csvなど)
            fs.ff = fs.filepath.Split(System.IO.Path.DirectorySeparatorChar); //ファイル名のディレクトリ分割
            int indexofmagcup = Array.IndexOf(fs.ff, "magcupdir", 0);
            fs.Pcat = fs.ff[indexofmagcup + 1]; //設備カテゴリ
            fs.Macno = fs.ff[indexofmagcup + 2]; //設備No
            fs.FindFold = fs.ff[indexofmagcup + 3]; //ファイルが検出されたワークフォルダ
            fs.MagCupNo = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), ""); //ファイル名（拡張子なし）
            fs.RecipeFile = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), "");  //レシピ名（拡張子なし）
            fs.fpath = $"{mci.MCDir}\\{fs.Pcat}\\{fs.Macno}"; //設備フォルダパス（ルート）
            fs.keylbl = Tasks_Common.FindKeyAlt(mci, fs.lowerfilepath); // タスクキーラベル(min1など)
            fs.mclbl = ""; //タスク種別のラベル（タスク内で代入）
            string[] extstr = fs.lowerfilepath.Split('.');
            fs.ext = extstr[1];

            //fs.ConnectionStrings = mci.ConnectionStrings; //DB接続文字配列
            //fs.WipDir = mci.WipDir; // WIPひな形フォルダ（現在未使用）
            //fs.RecipUpLoadDir = mci.RecipUpLoadDir; //レシピバックアップをアップロード先
            //fs.UsePlcTrig = mci.UsePlcTrig; //【デバック用設定】PLCを使用しない
            fs.mci = mci; //iniクラスの取り込み
        }

    }
}
