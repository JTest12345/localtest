using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResinClassLibrary {

    public static class BrotherLabelPrint {

        /// <summary>
        /// 印刷可能なラベルプリンターがあるか確認する
        /// </summary>
        /// <returns>プリンターの名前(無いときはnull)</returns>
        public static string CheckPrinterOnline() {

            var printer = new bpac.Printer();

            var installed_printers = printer.GetInstalledPrinters();

            foreach (var name in installed_printers) {
                if (printer.IsPrinterOnline((string)name)) {

                    return (string)name;
                }
            }
            return null;
        }

        /// <summary>
        /// 作製した樹脂カップに貼り付けるラベルを印刷する
        /// </summary>
        /// <param name="labelfile_path">ラベルファイルパス</param>
        /// <param name="recipe">印刷するレシピ</param>
        /// <exception cref="Exception"></exception>
        public static void PrintResinCupLabel(string labelfile_path, Recipe recipe, string printer_name) {

            var doc = new bpac.Document();

            //ラベルフォーマットファイルを開く
            var is_open = doc.Open(labelfile_path);

            if (is_open == false) {
                throw new Exception("ラベルフォーマットファイルが開けませんでした。");
            }

            /*↓ラベルフォーマット内の文字を書き換える*/
            bpac.Object obj;

            //カップ番号
            doc.GetObject("CupNo_TXT").Text = recipe.CupNo;

            //終了時間
            doc.GetObject("ChogoTime_TXT").Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //波長ランク
            doc.GetObject("SoshiRank_TXT").Text = $"< {recipe.WavelengthRank} >";

            //機種名
            obj = doc.GetObject("KshCd_TXT");
            if (obj != null) { obj.Text = recipe.ProductName; }

            //ロット番号
            if (recipe.LotNoList.Count == 1) {
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = recipe.LotNoList[0]; }
            }
            else if (recipe.LotNoList.Count == 2) {
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = $"{recipe.LotNoList[0]}\n{recipe.LotNoList[1]}"; }
            }
            else {
                string text = "";
                for (int i = 0; i < recipe.LotNoList.Count; i++) {
                    text += recipe.LotNoList[i];
                    if (i % 2 == 0) {
                        text += "/";
                    }
                    else {
                        text += "\n";
                    }
                }
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = text.Substring(0, text.Length - 1); }
            }

            //調合種類
            obj = doc.GetObject("ChogoType_TXT");
            if (obj != null) { obj.Text = $"{recipe.MoldType}/{recipe.MixTypeCode}/{recipe.FlowMode}"; }

            //変化点
            obj = doc.GetObject("Henkaten_TXT");
            if (obj != null) { obj.Text = recipe.Henkaten; }

            //QRコード
            obj = doc.GetObject("CupNoQR");
            if (obj != null) { obj.Text = recipe.CupNo; }

            /*↓印刷実行*/
            if (doc.SetPrinter(printer_name, false) == false) {
                throw new Exception("プリンター設定に失敗しました。");
            }

            if (doc.StartPrint(labelfile_path, bpac.PrintOptionConstants.bpoAutoCut) == false) {
                throw new Exception("印刷ジョブ設定開始に失敗しました。");
            }

            if (doc.PrintOut(1, bpac.PrintOptionConstants.bpoAutoCut) == false) {
                throw new Exception("印刷ジョブ追加に失敗しました。");
            }

            if (doc.EndPrint() == false) {
                throw new Exception("印刷ジョブ設定終了に失敗しました。");
            }

            doc.Close();
        }

        /// <summary>
        /// 作成したレシピ番号QRを印刷する(差立て時に配合情報紙に貼り付ける用)
        /// </summary>
        /// <param name="labelfile_path"></param>
        /// <param name="recipe"></param>
        /// <exception cref="Exception"></exception>
        public static void PrintRecipeNoLabel(string labelfile_path, Recipe recipe, List<string> recipe_cupno_list, string printer_name) {

            var doc = new bpac.Document();

            //ラベルフォーマットファイルを開く
            var is_open = doc.Open(labelfile_path);

            if (is_open == false) {
                throw new Exception("ラベルフォーマットファイルが開けませんでした。");
            }

            /*↓ラベルフォーマット内の文字を書き換える*/
            bpac.Object obj;

            //レシピ1
            var rc1 = recipe_cupno_list[0];
            doc.GetObject("RecipeNo1QR").Text = rc1;
            doc.GetObject("RecipeNo1_TXT").Text = $"{rc1.Substring(0, 2)} {rc1.Substring(2, 6)} {rc1.Substring(8)}";

            //レシピ2
            if (recipe_cupno_list.Count > 1) {
                var rc2 = recipe_cupno_list[1];
                obj = doc.GetObject("RecipeNo2QR");
                if (obj != null) { obj.Text = rc2; }
                obj = doc.GetObject("RecipeNo2_TXT");
                if (obj != null) { obj.Text = $"{rc2.Substring(0, 2)} {rc2.Substring(2, 6)} {rc2.Substring(8)}"; }
            }

            //ロット番号
            if (recipe.LotNoList.Count == 1) {
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = recipe.LotNoList[0]; }
            }
            else if (recipe.LotNoList.Count == 2) {
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = $"{recipe.LotNoList[0]}\n{recipe.LotNoList[1]}"; }
            }
            else {
                string text = "";
                for (int i = 0; i < recipe.LotNoList.Count; i++) {
                    text += recipe.LotNoList[i];
                    if (i % 2 == 0) {
                        text += "/";
                    }
                    else {
                        text += "\n";
                    }
                }
                obj = doc.GetObject("LotNo_TXT");
                if (obj != null) { obj.Text = text.Substring(0, text.Length - 1); }
            }


            //調合種類
            obj = doc.GetObject("TypeFlow_TXT");
            if (obj != null) { obj.Text = $"{recipe.MoldType}/{recipe.MixTypeCode}/{recipe.FlowMode}"; }


            /*↓印刷実行*/
            if (doc.SetPrinter(printer_name, false) == false) {
                throw new Exception("プリンター設定に失敗しました。");
            }

            if (doc.StartPrint(labelfile_path, bpac.PrintOptionConstants.bpoAutoCut) == false) {
                throw new Exception("印刷ジョブ設定開始に失敗しました。");
            }

            if (doc.PrintOut(1, bpac.PrintOptionConstants.bpoAutoCut) == false) {
                throw new Exception("印刷ジョブ追加に失敗しました。");
            }

            if (doc.EndPrint() == false) {
                throw new Exception("印刷ジョブ設定終了に失敗しました。");
            }

            doc.Close();

        }

    }
}
