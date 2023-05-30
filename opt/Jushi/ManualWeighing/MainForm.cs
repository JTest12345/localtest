using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

using Newtonsoft.Json;
using ResinPrg;//Junki Nakamura DLL

using ResinClassLibrary;

using static ManualWeighing.Program;
using ResinVBClassLibrary;
using System.CodeDom;
//using static ResinClassLibrary.PmmsSQL;
//using static ResinClassLibrary.CupWorkRegulation;

namespace ManualWeighing {

    public partial class MainForm : Form {

        private const string _Filt = "_filt.";
        private const string RCP_TXT = "_rcp.txt";
        private const string RCP_JSON = "_rcp.json";
        private const string RCP_FILT_JSON = "_rcp_filt.json";

        /// <summary>
        /// 全レシピリスト
        /// </summary>
        private List<RecipeFile> all_recipe_list;

        /// <summary>
        /// 普通の配合レシピリスト
        /// </summary>
        private List<RecipeFile> Normal_RecipeList {
            get {
                //TODO:.txt認識しないように変更する（試作の.txtは認識する）
                //var a= all_recipe_list.Where(x => x.FilePath.Contains(_Filt) == false).ToList();
                //var b = a.Where(x => x.FilePath.Contains(RCP_JSON) == true || x.FilePath.Contains("CDB00") == true).ToList();
                //return b;
                return all_recipe_list.Where(x => x.FilePath.Contains(_Filt) == false).ToList();
            }
        }

        /// <summary>
        /// ろ過後の配合レシピリスト
        /// </summary>
        private List<RecipeFile> Filt_RecipeList {
            get {
                return all_recipe_list.Where(x => x.FilePath.Contains(_Filt) == true).ToList();
            }
        }

        /// <summary>
        /// 選択したレシピのファイルパス
        /// </summary>
        private string selected_recipe_filepath;

        /// <summary>
        /// 選択したろ過後レシピのファイルパス
        /// </summary>
        private string selected_filtration_recipe_filepath;

        /// <summary>
        /// グローブボックスかどうか
        /// <para>true = グローブボックス</para>
        /// </summary>
        private bool is_groveBox = false;

        /// <summary>
        /// 風速計の排気が60秒間完了しているかどうか
        /// <para>true = 完了している 20230125</para>
        /// </summary>
        public static bool fusoku_flg = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm() {
            //test 2049年期限まで認識 20230412
            //string label_limit = "230411";
            //string format = "yyMMdd";
            //DateTime limit = DateTime.ParseExact(label_limit, format, null);
            //if (limit < DateTime.Now)
            //{
            //    return;
            //}

            InitializeComponent();

            this.Text = $"電子天秤樹脂秤量(C#) {AppVersion}";

            //資格情報の関係でアクセスできない時にここで例外が発生しない
            //フォームが開かないで終わる
            using (var fs = new FileStream($@"{SystemFileFolderPath}\ManualWeighing_latest-version.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.UTF8)) {
                string line = sr.ReadLine();
                latest_version_label.Text = $"現在のソフト最新版は[{line}]です。";

                if (line != AppVersion) {
                    latest_version_label.Text += "\nこのソフトは最新ではありません。";
                    latest_version_label.BackColor = Color.Khaki;
                }
            }

            //自分自身のフォームを最大化
            this.WindowState = FormWindowState.Maximized;

            //fusoku_realtime_value.Text = "DG";
            //風速計タイマー開始 20230213
            //fusoku_value_timer.Start();

            //非同期処理開始 20230125
            //fusoku_call();

        }

        /// <summary>
        /// 初期化(メニュー)押した時の処理
        /// </summary>
        private void initialize_ToolStripMenuItem_Click(object sender, EventArgs e) {

            //グローブボックス扉開閉チェック通信可能か確認
            var dio = new ContecDio(AppSettings.DioDeviceName);
            try {
                dio.Initialize();
                var b = dio.Get_InputBit(AppSettings.DoorCheckBitNo);
                if (b == 0) { throw new Exception(""); }
                //KSF使用可能
                is_groveBox = true;
                ksf_ng_label.Visible = false;
            }
            catch (Exception ex) {
                //KSF使用不可
                is_groveBox = false;
                ksf_ng_label.Visible = true;
            }
            finally {
                dio.End();
            }

            //レシピリスト更新
            try {
                update_recipeliist_button_Click(null, null);
            }
            catch (Exception ex) {
                MessageBox.Show($"レシピが取得出来ませんでした。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //カップ番号入力ボックス使用可能にする
            input_cupno_textBox.ReadOnly = false;
            input_cupno_textBox.Select();
        }

        /// <summary>
        /// 設定(メニュー)押した時の処理
        /// </summary>
        private void setting_ToolStripMenuItem_Click(object sender, EventArgs e) {
            var f = new SettingForm();
            f.ShowDialog();
            f.Dispose();
            input_cupno_textBox.ReadOnly = true;
        }

        /// <summary>
        /// レシピリストデータグリッドビュー更新
        /// </summary>
        /// <param name="list"></param>
        private void Update_recipe_list_dataGridView(List<RecipeFile> list, DataGridView dgv) {

            //データグリッドビュー初期化
            dgv.Columns.Clear();
            //以下のプロパティを設定しないと描画が遅くなる
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            var table = new DataTable("table");
            table.Columns.Add("カップ番号");
            table.Columns.Add("ファイルパス");

            for (int i = 0; i < list.Count; i++) {
                //新しく1行データ作成
                DataRow dr = table.NewRow();

                dr["カップ番号"] = list[i].CupNo;
                dr["ファイルパス"] = list[i].FilePath;

                //テーブルに1行分データ追加
                table.Rows.Add(dr);
            }

            //データグリッドビューにセット
            dgv.DataSource = table;

            //プロパティを戻す
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
        }

        /// <summary>
        /// 更新ボタン処理
        /// </summary>
        private void update_recipeliist_button_Click(object sender, EventArgs e) {
            all_recipe_list = Get_RecipeList();
            Update_recipe_list_dataGridView(Normal_RecipeList, recipe_list_dataGridView);
            Update_recipe_list_dataGridView(Filt_RecipeList, filtration_recipe_list_dataGridView);
            normal_recipe_num_label.Text = Normal_RecipeList.Count.ToString();
            filt_recipe_num_label.Text = Filt_RecipeList.Count.ToString();
        }

        /// <summary>
        /// レシピファイルが保存されているフォルダからレシピリストを取得する
        /// </summary>
        /// <returns></returns>
        private List<RecipeFile> Get_RecipeList() {

            var rf = new List<RecipeFile>();

            var txt_files = Directory.GetFiles(AppSettings.RecipeFolderPath, $"*{RCP_TXT}");

            foreach (var tf in txt_files) {
                rf.Add(new RecipeFile {
                    CupNo = Path.GetFileName(tf).Replace(RCP_TXT, ""),
                    FilePath = tf
                });
            }

            var json_files = Directory.GetFiles(AppSettings.RecipeFolderPath, $"*{RCP_JSON}");

            foreach (var jf in json_files) {
                rf.Add(new RecipeFile {
                    CupNo = Path.GetFileName(jf).Replace(RCP_JSON, ""),
                    FilePath = jf
                });
            }

            json_files = Directory.GetFiles(AppSettings.RecipeFolderPath, $"*{RCP_FILT_JSON}");

            foreach (var jf in json_files) {
                rf.Add(new RecipeFile {
                    CupNo = Path.GetFileName(jf).Replace(RCP_FILT_JSON, ""),
                    FilePath = jf
                });
            }

            return rf;
        }

        /// <summary>
        /// カップ番号入力した時の処理
        /// </summary>
        private void input_cupno_textBox_TextChanged(object sender, EventArgs e) {

            string cupno = input_cupno_textBox.Text.ToUpper();

            var list = Normal_RecipeList.Where(x => x.CupNo.Contains(cupno)).ToList();
            var filt_list = Filt_RecipeList.Where(x => x.CupNo.Contains(cupno)).ToList();

            //レシピリストデータグリッドビュー更新
            Update_recipe_list_dataGridView(list, recipe_list_dataGridView);
            Update_recipe_list_dataGridView(filt_list, filtration_recipe_list_dataGridView);

            //データが1行の場合のみ秤量開始出来る(普通の配合)
            if (recipe_list_dataGridView.Rows.Count == 1) {
                start_weigh_button.Visible = true;
                start_manual_weigh_button.Visible = true;
                selected_recipe_filepath = recipe_list_dataGridView.Rows[0].Cells[1].Value.ToString();
                if (recipe_list_tabControl.SelectedIndex == 0) {
                    start_weigh_button.Select();
                }
            }
            else {
                start_weigh_button.Visible = false;
                start_manual_weigh_button.Visible = false;
                selected_recipe_filepath = "";
            }

            //データが1行の場合のみ秤量開始出来る(ろ過後配合)
            if (filtration_recipe_list_dataGridView.Rows.Count == 1) {
                filtration_weigh_start_button.Visible = true;
                selected_filtration_recipe_filepath = filtration_recipe_list_dataGridView.Rows[0].Cells[1].Value.ToString();
                if (recipe_list_tabControl.SelectedIndex == 1) {
                    filtration_weigh_start_button.Select();
                }
            }
            else {
                filtration_weigh_start_button.Visible = false;
                selected_filtration_recipe_filepath = "";
            }
        }

        /// <summary>
        /// 秤量開始ボタン処理
        /// </summary>
        private void start_weigh_button_Click(object sender, EventArgs e) {
            //20230125 デバッグ時コメントアウト プリンタチェック

            LabelPrinterName = BrotherLabelPrint.CheckPrinterOnline();

            if (string.IsNullOrEmpty(LabelPrinterName))
            {
                MessageBox.Show("使用可能なラベルプリンターがありません。\n電源が入っていないか、接続異常の可能性があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }


            if (selected_recipe_filepath.Contains($"-2{RCP_JSON}")) {
                string cupno_1_filename = Path.GetFileName(selected_recipe_filepath).Replace($"-2{RCP_JSON}", $"-1{RCP_JSON}");
                string cupno_1_already_made_path = $@"{AppSettings.AlreadyMadeRecipeFolderPath}\{cupno_1_filename}";

                if (File.Exists(cupno_1_already_made_path) == false) {
                    MessageBox.Show("最初に1番目のレシピで樹脂配合を行って下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            //レシピファイルからレシピ取得
            Recipe recipe;
            if (selected_recipe_filepath.Contains("_rcp.txt")) {
                recipe = Recipe.Get_Recipe_from_txtFile(selected_recipe_filepath);
                if (recipe == null) {
                    MessageBox.Show("レシピの取得に失敗しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try {
                    /*--↓仕方ないからここでFコード設定している(レシピファイルにFコード無いから)--*/
                    /*--あとグローブボックスを使う部材かどうかも--*/

                    //このフローは一時的であり　3-3F HWのみとする

                    //部材情報の辞書作成
                    string path = $@"{SystemFileFolderPath}\HW_buzai_master.csv";
                    var dic = Buzai.Get_BuzaiInfo(path, 2);

                    //使用する各部材について情報を設定
                    foreach (var ub in recipe.UseBuzai) {
                        foreach (var key in dic.Keys) {
                            //keyはスラッシュありの部材名だが、txtファイルから読み取る部材名にはスラッシュが無い
                            if (key == ub.Name || key.Replace("/", "") == ub.Name) {
                                ub.Fcode = dic[key].Fcode;
                                ub.NeedGroveBox = dic[key].NeedGroveBox;
                                break;
                            }
                        }
                    }


                    int fcode_cnt = recipe.UseBuzai.Count(x => string.IsNullOrEmpty(x.Fcode));
                    if (fcode_cnt > 0) {
                        MessageBox.Show("Fコードが設定されていない部材があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    /*--↑仕方ないからここでFコード設定している(レシピファイルにFコード無いから)--*/

                    /*--↓仕方ないからここでMixTypecd変換している(レシピファイルは101のみだから)--*/
                    bool is_ksf = false;
                    if (recipe.UseBuzai.Count(x => x.NeedGroveBox == true) > 0) { is_ksf = true; }
                    if (recipe.MixTypeCode == "101") {

                        if (recipe.FlowMode == Recipe.AUTO) {

                            if (is_ksf) {
                                recipe.MixTypeCode = "105";
                            }
                            else {
                                recipe.MixTypeCode = "103";
                            }
                        }
                        else {
                            if (is_ksf) {
                                recipe.MixTypeCode = "106";
                            }
                            else {
                                recipe.MixTypeCode = "104";
                            }
                        }
                    }


                    /*--↓仕方ないからここでカップ作業規制時間を取得している(レシピファイルに無いから)--*/
                    var mix_typecd = recipe.MixTypeCode;
                    var list = new List<string> { "103", "104", "105", "106" };
                    if (list.Contains(mix_typecd)) {
                        mix_typecd = "101";
                    }
                    recipe.CupWorkRegulations = Get_CupWorkRegulation($@"{SystemFileFolderPath}\PMMSTimeSetting.txt", recipe.ProductName, mix_typecd, recipe.CupNo);

                    /*--↓仕方ないからここでPMMS TmWorkRegulationへ書き込む*/
                    string err_msg = "";
                    bool success = PMMS.Insert_CupTimeInfo_to_TmWorkRegulation(recipe.CupWorkRegulations, ref err_msg);
                    if (!success) {
                        var msg = $"{err_msg}\n\nPMMS 作業規制時間書き込みに失敗しました。\nネットワーク異常がないか確認して下さい。";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    /*--↓仕方ないからここでPMMS CupInfoへ書き込む*/
                    //書き込まないとtranが書き込めない
                    err_msg = "";
                    var cupinfo_list = new List<TnCupInfo> {
                        new TnCupInfo { cupno = recipe.CupNo, mixtypecd = recipe.MixTypeCode }
                    };
                    success = PMMS.Insertmixtypecd(cupinfo_list, ref err_msg);
                    if (!success) {
                        var msg = $"{err_msg}\n\nPMMS カップ情報書き込みに失敗しました。\nネットワーク異常がないか確認して下さい。";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"樹脂マスタ処理でエラーが発生しました。\n{ex.Message}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else {
                //JSON形式のレシピは読み込むだけ
                recipe = Recipe.Get_Recipe_from_jsonFile<Recipe>(selected_recipe_filepath);
                recipe.FileName = Path.GetFileName(selected_recipe_filepath);
                if (recipe == null) {
                    MessageBox.Show("レシピの取得に失敗しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            //KSFを使うか確認
            int ksf_cnt = recipe.UseBuzai.Count(x => x.NeedGroveBox == true);
            if (ksf_cnt > 0 && is_groveBox == false) {
                MessageBox.Show("危険！\nKSF蛍光体を使用する配合はグローブボックス内で配合しなければなりません。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool need_grovebox = false;
            if (ksf_cnt > 0) {
                need_grovebox = true;
            }


            var empcd = Interaction.InputBox("社員コードを入力して下さい。", "Input Employee Code");
            empcd = empcd.Replace("01 ", "");
            if (string.IsNullOrEmpty(empcd)) {
                MessageBox.Show("社員コードを入力して下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //MANUAL秤量開始ボタンの場合は配合手段をMANUALに変える
            var btn = (Button)sender;
            if (btn.Tag != null && btn.Tag.ToString() == Recipe.MANUAL) {

                if (recipe.FileName.Contains($"-1{RCP_JSON}")) {
                    var rec2 = Recipe.Get_Recipe_from_jsonFile<Recipe>($@"{AppSettings.RecipeFolderPath}\{recipe.FileName.Replace("-1", "-2")}");
                    foreach (var ub2 in rec2.UseBuzai) {
                        recipe.UseBuzai.Add(ub2);
                    }
                    recipe.FileName += $@",{recipe.FileName.Replace("-1", "-2")}";
                }


                recipe.FlowMode = Recipe.MANUAL;
                //recipe.ProcessNo = 1;
                foreach (var b in recipe.UseBuzai) {
                    b.UseAutoMachine = false;
                }

                //PMMS CupInfoデータベースを上書きする
                if (recipe.ManualMode_MixTypeCode == null) {
                    var list = new List<string> { "103", "104", "105", "106" };
                    if (list.Contains(recipe.MixTypeCode)) {
                        recipe.MixTypeCode = "101";
                    }
                }
                else {
                    //jsonファイルには書いてある
                    recipe.MixTypeCode = recipe.ManualMode_MixTypeCode;
                }

                string err_msg = "";
                var cupinfo_list = new List<TnCupInfo> {
                    new TnCupInfo { cupno = recipe.CupNo, mixtypecd = recipe.MixTypeCode }
                };
                while (true) {
                    bool success = PMMS.Insertmixtypecd(cupinfo_list, ref err_msg);
                    if (success) {
                        break;
                    }
                    else {
                        var msg = $"{err_msg}\n\nPMMS カップ情報上書きに失敗しました。\nネットワーク異常がないか確認して下さい。" +
                                $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                        var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (ret == DialogResult.No) {
                            MessageBox.Show("中断します。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                }
            }


            //前の工程までが完了しているかチェック
            //但し、最初の配合時はtrueになり通る
            //手配合が2回ある場合用
            //MANUALの時はチェックしない
            if (recipe.FlowMode != Recipe.MANUAL) {
                string errmsg = "";
                bool finish = ResinPrg.PMMS.BeforeMixCheck(recipe.CupNo, ref errmsg);

                if (!finish) {
                    MessageBox.Show($"PMMSシステムで前の工程が完了していません。\n\n{errmsg}", "Stop!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            if (is_groveBox) {
                MessageBox.Show("排気をOFFにして下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //秤量開始
            input_cupno_textBox.Text = "";
            var f = new WeighForm(recipe, empcd, is_groveBox, need_grovebox);
            f.ShowDialog();
            f.Dispose();

            //レシピリスト更新
            update_recipeliist_button_Click(null, null);
        }

        /// <summary>
        /// ろ過前後秤量開始ボタン処理
        /// </summary>
        private void filtration_weigh_start_button_Click(object sender, EventArgs e) {
            //20230220 デバッグ時コメントアウト
            
            LabelPrinterName = BrotherLabelPrint.CheckPrinterOnline();

            if (string.IsNullOrEmpty(LabelPrinterName)) {
                MessageBox.Show("使用可能なラベルプリンターがありません。\n電源が入っていないか、接続異常の可能性があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            
            //JSON形式のレシピは読み込むだけ
            var recipe = Recipe.Get_Recipe_from_jsonFile<AfterFiltrationRecipe>(selected_filtration_recipe_filepath);
            recipe.FileName = Path.GetFileName(selected_filtration_recipe_filepath);

            
            //レシピ作成後、どのタイミングで空カップを測定しても良い仕様　20230220
            //

            string empcd = "";
            //ろ過前に空カップの重量を測定しておく　※ろ過で受け側のカップを事前に測定
            if (recipe.BeforeCupWeight == null) {
                //ラベル印刷
                BrotherLabelPrint.PrintResinCupLabel(AppSettings.LabelFormatFilePath, recipe, LabelPrinterName);
                MessageBox.Show("カップにラベルを貼り付けてください。", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                //ろ過前までがPMMSで完了しているかチェック
                //但し、最初の配合もしていない時はtrueになり通ってしまう。
                string errmsg = "";
                bool finish = ResinPrg.PMMS.BeforeMixCheck(recipe.CupNo, ref errmsg);

                if (!finish)
                {
                    MessageBox.Show($"PMMSシステムで前の工程が完了していません。\n\n{errmsg}", "Stop!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                empcd = Interaction.InputBox("社員コードを入力して下さい。", "Input Employee Code");

                empcd = empcd.Replace("01 ", "");

                if (string.IsNullOrEmpty(empcd)) {
                    MessageBox.Show("社員コードを入力して下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (is_groveBox) {
                MessageBox.Show("排気をOFFにして下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //秤量開始
            var f = new WeighFiltrationForm(recipe, empcd, is_groveBox);
            f.ShowDialog();
            f.Dispose();

            //レシピリスト更新
            update_recipeliist_button_Click(null, null);
        }



        /// <summary>
        /// ラベル再印刷ボタン処理
        /// </summary>
        private void print_button_Click(object sender, EventArgs e) {

            LabelPrinterName = BrotherLabelPrint.CheckPrinterOnline();

            if (string.IsNullOrEmpty(LabelPrinterName)) {
                MessageBox.Show("使用可能なラベルプリンターがありません。\n電源が入っていないか、接続異常の可能性があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var cupno = Interaction.InputBox("樹脂カップ番号を入力して下さい。", "Input CupNo");

            if (string.IsNullOrEmpty(cupno)) { return; }

            string path = Get_AlreadyCupPath(cupno);

            if (string.IsNullOrEmpty(path)) {
                MessageBox.Show("レシピファイルがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //レシピファイルからレシピ取得
            Recipe recipe;
            if (path.Contains("_rcp.txt")) {
                recipe = Recipe.Get_Recipe_from_txtFile(path);
            }
            else {
                recipe = Recipe.Get_Recipe_from_jsonFile<Recipe>(path);
            }

            if (recipe == null) {
                MessageBox.Show("レシピ取得出来ませんでした。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //印刷
            BrotherLabelPrint.PrintResinCupLabel(AppSettings.LabelFormatFilePath, recipe, LabelPrinterName);
        }

        /// <summary>
        /// レシピ復活ボタン処理
        /// </summary>
        private void revival_button_Click(object sender, EventArgs e) {
            var cupno = Interaction.InputBox("樹脂カップ番号を入力して下さい。", "Input CupNo");

            if (string.IsNullOrEmpty(cupno)) { return; }

            string old_path = Get_AlreadyCupPath(cupno);

            if (string.IsNullOrEmpty(old_path)) {
                MessageBox.Show("レシピファイルがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //レシピファイルからレシピ取得
            Recipe recipe;
            if (old_path.Contains("_rcp.txt")) {
                recipe = Recipe.Get_Recipe_from_txtFile(old_path);
            }
            else {
                recipe = Recipe.Get_Recipe_from_jsonFile<Recipe>(old_path);
            }

            if (recipe.FlowMode != Recipe.MANUAL) {
                MessageBox.Show("配合手段がMANUALのレシピしか復活できません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string dest_path = $@"{AppSettings.RecipeFolderPath}\{Path.GetFileName(old_path)}";
            try {
                File.Move(old_path, dest_path);
                //レシピリスト更新
                update_recipeliist_button_Click(null, null);
            }
            catch {
                MessageBox.Show("復活出来ませんでした。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 作製済みカップのレシピファイルパスを取得する
        /// </summary>
        private string Get_AlreadyCupPath(string cupno) {

            cupno = cupno.ToUpper();

            string txt_path = $@"{AppSettings.AlreadyMadeRecipeFolderPath}/{cupno}_rcp.txt";
            string json_path = $@"{AppSettings.AlreadyMadeRecipeFolderPath}/{cupno}_rcp.json";

            string path = "";
            if (File.Exists(txt_path)) {
                path = txt_path;
            }
            else {
                if (File.Exists(json_path)) {
                    path = json_path;
                }
            }

            return path;
        }

        /// <summary>
        /// 樹脂カップ作業制限時間リストを取得する
        /// <para>"PMMSTimeSetting.txt"から取得</para>
        /// </summary>
        /// <param name="path">制限時間マスタファイルパス</param>
        /// <param name="p_name">機種名</param>
        /// <param name="mixtype_code">樹脂カップ作業工程コード</param>
        /// <param name="cupno">カップ番号</param>
        /// <returns></returns>
        public List<TmWorkRegulationData> Get_CupWorkRegulation(string path, string p_name, string mixtype_code, string cupno) {

            var list = new List<TmWorkRegulationData>();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                //1行目を読み込む（使わない）
                sr.ReadLine();

                //2行目以降
                string str;

                while (true) {
                    str = sr.ReadLine();

                    if (str == "" | str == null) {
                        break;
                    }

                    string[] array = str.Split(',');

                    var name_ok = VBFuncs.CheckProductName(p_name, array[0]);

                    if (name_ok) {
                        if (mixtype_code == array[1]) {

                            int procfrom = int.Parse(array[2]);
                            int procto = int.Parse(array[3]);

                            //同じ基準工程～次工程の情報が既にあるか数える
                            int cnt = list.Count(x => x.procfrom == procfrom && x.procto == procto);

                            if (cnt == 0) {
                                list.Add(new TmWorkRegulationData {
                                    cupno = cupno,
                                    procfrom = procfrom,                 //基準工程
                                    procto = procto,                     //次工程
                                    fromwaittime = int.Parse(array[4]),  //基準工程実施してから次工程開始が出来るようになるまでの時間(分)
                                    fromtoendtime = int.Parse(array[5]), //基準工程実施してから次工程完了までの時間(分)
                                });
                            }
                        }
                    }
                }

            }

            return list;
        }


        /// <summary>
        /// フォルダ内のレシピファイルのリストを作るために使う
        /// </summary>
        private class RecipeFile {

            public string CupNo { get; set; }


            public string FilePath { get; set; }
        }

        /// <summary>
        /// 風速計の値を取得 //20230125
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fusoku_value_timer_Tick(object sender, EventArgs e)
        {

            if (!kanomax_serialPort.IsOpen)
            {
                //シリアルポートオープン
                kanomax_serialPort.PortName = AppSettings.kanomaxSerialPortName;
                kanomax_serialPort.NewLine = "\r";
                kanomax_serialPort.Open();
                kanomax_serialPort.DiscardInBuffer();
            }



            kanomax_serialPort.DiscardInBuffer();
            kanomax_serialPort.NewLine = "\r";
            kanomax_serialPort.Write("DG,01" + "\r");


            string r_msg = kanomax_serialPort.ReadLine();
            //kanomax_serialPort.ReadLine();

            string[] arr = r_msg.Split(',');
            string kanomax_value = arr[2].Substring(0, 2) + "." + arr[2].Substring(2, 2);
            //頭が0なら、小数点調整で頭1文字削除
            if(kanomax_value.Substring(0,1) == "0")
            {
                kanomax_value = kanomax_value.Substring(1);
            }

            fusoku_realtime_value.Text = kanomax_value;

            //kanomax_serialPort.Write("PG,01" + "\r");
            //r_msg = kanomax_serialPort.ReadLine();

            //kanomax_serialPort.Close();



            //double test = 0;

            //Random r1 = new System.Random();

            //test = r1.Next(0, 100);
            ////test = 1;

            //fusoku_realtime_value.Text = test.ToString();


        }

        //20230125 風速計メインフォームテスト用　実環境未使用
        public async Task Do_fusoku()
        {
            if (fusoku_flg == false)
            {
                //風速計NG表示
                fusokuNG_label.Size = new Size(1000, 400);
                //fusokuNG_label.Location = new Point(this.Width, this.Height / 2 - fusokuNG_label.Height / 2);

                //風速計値表示
                fusoku_realtime_label.Size = new Size(600, 200);
                //fusoku_realtime_label.Location = new Point(this.Width / 2 - fusoku_realtime_label.Width / 2, this.Height / 2 - fusoku_realtime_label.Height / 2);
                fusoku_realtime_label.Visible = true;

                int remain_time = AppSettings.ManualMixSecond;

                //MessageBox.Show("排気ONボタンを押して排気を開始して下さい。\n\nその後、OKボタンを押して下さい。");
                //status_label.Text = "排気中...";

                //局所排気を指定時間実施しなければならない
                remain_time = 20;//AppSettings.ExhaustSecond;
                while (remain_time > 0)
                {
                    fusoku_realtime_label.Text = $"排気中...\n\n残り時間  {remain_time.ToString()} 秒";
                    await Task.Delay(1000);

                    //ドアが閉まっている場合のみ残り時間を減らす
                    //かつ風速計の値が一定以上であれば時間を減らす 20230125
                    if (Double.Parse(fusoku_realtime_value.Text) > 0.02)
                    {
                        remain_time -= 1;
                        fusokuNG_label.Visible = false;
                    }
                    else
                        fusokuNG_label.Visible = true;
                }

                fusoku_realtime_label.Visible = false;

                fusoku_flg = true;
                //風速計の値取得のタイマーを止める
                fusoku_value_timer.Stop();
                
            }
        }

        //20230125 風速計メインフォームテスト用　実環境未使用
        public async void fusoku_call()
        {
            await Do_fusoku();
        }

        private void kanomax_serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //string r_msg;
            //try
            //{
            //    r_msg = kanomax_serialPort.ReadLine();
            //    kanomax_serialPort.Close();
            //    //計量状態を取得
            //    fusoku_realtime_value.Text = r_msg;
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show($"データの受信に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //serial_rec_end = true;
            //    return;
            //}
        }
    }
}
