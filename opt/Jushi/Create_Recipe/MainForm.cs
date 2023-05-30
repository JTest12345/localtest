using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using ResinPrg;//Junki Nakamura DLL

using static Create_Recipe.Program;
using static Create_Recipe.OperateExcel;
using ResinClassLibrary;
using ResinVBClassLibrary;



namespace Create_Recipe {

    public partial class MainForm : Form {

        private const string _rcp_txt = "_rcp.txt";
        private const string _rcp_json = "_rcp.json";
        private const string _rcp_filt_json = "_rcp_filt.json";
        private const string print_1_recipelabel = "1Recipe.lbx";
        private const string print_2_recipelabel = "2Recipe.lbx";
        private const string resin_cupno_label = "ResinCupLabel_BlackRed.lbx";

        /// <summary>
        /// 入力情報
        /// </summary>
        private InputInfo input_info;

        /// <summary>
        /// 配合情報
        /// </summary>
        private MixInfo mix_info;

        /// <summary>
        /// ろ過後配合情報
        /// </summary>
        private MixInfo after_filt_mix_info;

        /// <summary>
        /// 配合手段
        /// </summary>
        private string flow_mode;

        /// <summary>
        /// 製造拠点辞書
        /// </summary>
        public static readonly Dictionary<string, string> place_dic = new Dictionary<string, string> {
            {"TC","CET 3-3F" },
            {"TS","境川" },
            {"DG","デバッグ" }
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm() {
            InitializeComponent();

            this.Text = $"樹脂カップレシピ作成(C#) {AppVersion}";

            //初期化
            clear_button_Click(null, null);

            place_label.Text = $"({Program.Place}) {place_dic[Place]}";
            if (Place == "TC") {
            }
            else if (Place == "TS") {
                exist_autoMachine_checkBox.Checked = false;
                exist_autoMachine_checkBox.Enabled = false;
            }

            //自分自身のフォームを最大化
            this.WindowState = FormWindowState.Maximized;
            productName_textBox.Select();

            show_senkolog_database();

#if DEBUG
            button1.Visible = true;
#endif
        }

        /// <summary>
        /// 設定メニュー処理
        /// </summary>
        private void setting_ToolStripMenuItem_Click(object sender, EventArgs e) {
            var f = new SettingForm();
            f.ShowDialog();
            f.Dispose();
            place_label.Text = $"({Program.Place}) {place_dic[Place]}";
            if (Place == "TC" || Place == "DG") {
                exist_autoMachine_checkBox.Checked = true;
                exist_autoMachine_checkBox.Enabled = true;
            }
            else if (Place == "TS") {
                exist_autoMachine_checkBox.Checked = false;
                exist_autoMachine_checkBox.Enabled = false;
            }

            show_senkolog_database();
        }

        /// <summary>
        /// 先行評価ログがデータベースかどうかを色分け
        /// </summary>
        private void show_senkolog_database() {

            if (AppUsePath.SenkologDatabase) {
                place_label.BackColor = Color.Yellow;
            }
            else {
                place_label.BackColor = Color.CornflowerBlue;
            }
        }

        /// <summary>
        /// 印刷テストメニュー処理
        /// </summary>
        private void print_test_ToolStripMenuItem_Click(object sender, EventArgs e) {
            string printer_name = "";
            printer_name = BrotherLabelPrint.CheckPrinterOnline();
            if (string.IsNullOrEmpty(printer_name)) {
                MessageBox.Show("使用可能なラベルプリンターがありません。\n電源が入っていないか、接続異常の可能性があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string label_path = $@"{AppFolder}\label\{print_1_recipelabel}";
            var recipe = new Recipe { MoldType = "MD", MixTypeCode = "101", FlowMode = Recipe.MANUAL };
            recipe.LotNoList = new List<string> { "test-lot" };
            var list = new List<string> { "TC221201001-1" };
            BrotherLabelPrint.PrintRecipeNoLabel(label_path, recipe, list, printer_name);
        }

        /// <summary>
        /// データグリッドビュー初期化処理
        /// </summary>
        private void DataGridView_Initialize() {

            //各データグリッドビュー初期化
            lotinfo_dataGridView.Rows.Clear();
            autoMachine_setBuzai_dataGridView.Rows.Clear();


            //ロット番号/基板枚数入力データグリッドビューに5行追加
            for (int i = 0; i < 5; i++) {
                lotinfo_dataGridView.Rows.Add();
            }

            //自動配合機セット部材データグリッドビューに16行追加
            for (int i = 0; i < 16; i++) {
                autoMachine_setBuzai_dataGridView.Rows.Add();
            }

            foreach (var dgv in new List<DataGridView> { mixInfo_dataGridView, afterfilt_mixInfo_dataGridView }) {

                //初期化
                dgv.Rows.Clear();

                //配合情報データグリッドビューに7行追加
                for (int i = 0; i < 8; i++) {
                    dgv.Rows.Add();
                }

                dgv.Rows[0].HeaderCell.Value = "フォーマット名称";
                dgv.Rows[1].HeaderCell.Value = "部材名";
                dgv.Rows[2].HeaderCell.Value = "LotNo";
                dgv.Rows[3].HeaderCell.Value = "配合比(ベース)";
                dgv.Rows[4].HeaderCell.Value = "配合量[g]";
                dgv.Rows[5].HeaderCell.Value = "誤差量[g]";
                dgv.Rows[6].HeaderCell.Value = "配合順番";
                dgv.Rows[7].HeaderCell.Value = "自動機残量[g]";

                //配合情報の列の色初期化
                for (int i = 0; i < dgv.Columns.Count; i++) {
                    dgv.Columns[i].DefaultCellStyle.BackColor = SystemColors.Window;
                }

            }
        }

        /// <summary>
        /// 最初からやり直すボタン処理
        /// </summary>
        private void clear_button_Click(object sender, EventArgs e) {

            input_info = null;
            mix_info = null;
            after_filt_mix_info = null;
            flow_mode = "";
            senkolog_panel.Visible = false;
            flow_mode_label.Text = "";
            make_amount_numericUpDown.Value = 0;
            create_cupno_label.Text = "";
            productName_textBox.Text = "";
            input_info_label.Text = "";
            mixtypecd_label.Text = "";

            //各コントロール初期化
            input_productName_panel.Enabled = true;
            input_productName_panel.BackColor = Color.PaleTurquoise;
            input_lot_panel.Enabled = false;
            input_lot_panel.BackColor = SystemColors.Control;
            condition1_panel.Enabled = false;
            condition1_panel.BackColor = SystemColors.Control;
            condition2_panel.Enabled = false;
            condition2_panel.BackColor = SystemColors.Control;
            create_recipe_panel.Enabled = false;
            create_recipe_panel.BackColor = SystemColors.Control;
            setting_ToolStripMenuItem.Enabled = true;
            clear_condition2_button.Enabled = false;
            clear_lot_info_button.Enabled = false;

            DataGridView_Initialize();

            product_type_comboBox.SelectedIndex = 1;
            product_type_comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 機種名入力ボックスイベント処理
        /// </summary>
        private void productName_textBox_KeyPress(object sender, KeyPressEventArgs e) {

            //Enterが押された場合
            if ((int)e.KeyChar == 13) {
                string pn = productName_textBox.Text;
                if (string.IsNullOrEmpty(pn)) {
                    MessageBox.Show("入力されていません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    productName_textBox.Text = "";
                    return;
                }
                try {
                    Program.CheckAppVersion();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Stop!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                //オブジェクト作成
                input_info = new InputInfo() {
                    Place = Program.Place,
                    ProductName = pn,
                    ProductType = product_type_comboBox.SelectedItem.ToString()
                };

                WriteInputInfoLabel();
                input_productName_panel.Enabled = false;
                input_productName_panel.BackColor = SystemColors.Control;
                //樹脂種類選択
                type_comboBox.SelectedIndex = 1;
                type_comboBox.SelectedIndex = 0;
                //波長選択
                wavelength_rank_comboBox.SelectedIndex = 1;
                wavelength_rank_comboBox.SelectedIndex = 0;
                //特殊指定選択
                special_designation_comboBox.SelectedIndex = 1;
                special_designation_comboBox.SelectedIndex = 0;

                condition1_panel.Enabled = true;
                condition1_panel.BackColor = Color.PaleTurquoise;

                setting_ToolStripMenuItem.Enabled = false;

                //設定ファイルが更新されていたら再設定する
                if (UsePath.CheckUpdateSetting()) {
                    AppUsePath = UsePath.ReadSetting();
                }

                show_senkolog_database();
            }
        }

        /// <summary>
        /// 波長ランク選択時処理
        /// </summary>
        private void wavelength_rank_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            input_info.WavelengthRank = cb.SelectedItem.ToString();
        }

        /// <summary>
        /// 樹脂種類選択時処理
        /// </summary>
        private void type_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            var type = cb.SelectedItem.ToString();

            var sp = type.Split(')')[0].Replace("(", "").Split(',');

            input_info.MoldType = sp[0];
            input_info.MixTypeCode = sp[1];

            if (input_info.MoldType.Contains("MD")) {
                wavelength_rank_comboBox.Enabled = true;
                special_designation_comboBox.Enabled = true;
            }
            else {
                wavelength_rank_comboBox.SelectedIndex = 1;
                wavelength_rank_comboBox.SelectedIndex = 0;
                wavelength_rank_comboBox.Enabled = false;
                special_designation_comboBox.SelectedIndex = 1;
                special_designation_comboBox.SelectedIndex = 0;
                special_designation_comboBox.Enabled = false;
            }

        }

        /// <summary>
        /// Warm指定選択時処理
        /// </summary>
        private void special_designation_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            input_info.SpecialDesignation = cb.SelectedItem.ToString().Split(')')[0];
        }

        /// <summary>
        /// 設定1ボタン処理
        /// </summary>
        private void set_condition1_button_Click(object sender, EventArgs e) {

            condition1_panel.Enabled = false;
            condition1_panel.BackColor = SystemColors.Control;
            WriteInputInfoLabel();

            //条件選択
            jouken_comboBox.SelectedIndex = 1;
            jouken_comboBox.SelectedIndex = 0;
            condition2_panel.Enabled = true;
            condition2_panel.BackColor = Color.PaleTurquoise;
        }

        /// <summary>
        /// 条件選択時処理
        /// </summary>
        private void jouken_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            input_info.Conditions = cb.SelectedItem.ToString().Split(')')[0];
        }

        /// <summary>
        /// 使用部材を設定する
        /// <para>データベースにある部材情報をMixInfoの各部材プロパティに結び付ける</para>
        /// </summary>
        /// <param name="mi">配合情報</param>
        /// <param name="list"></param>
        private void Set_UseBuzai(MixInfo mi, List<MixBuzai> list) {

            foreach (var mb in list.Where(x => x.Type.Contains("RESIN") == true)) {

                if (mb.Type == Buzai.RESIN_A) {
                    mi.ResinA = mb;
                }
                else if (mb.Type == Buzai.RESIN_B) {
                    mi.ResinB = mb;
                }
            }

            foreach (var mb in list.Where(x => x.Type == Buzai.FILLER)) {
                //現在1個しかない
                mi.Filler = mb;
            }

            foreach (var mb in list.Where(x => x.Type == Buzai.TIO2)) {
                //現在1個しかない
                mi.TiO2 = mb;
            }

            foreach (var mb in list.Where(x => x.Type == Buzai.TORO)) {
                //現在1個しかない
                mi.Toro = mb;
            }

            int i = 0;
            foreach (var mb in list.Where(x => x.Type == Buzai.YELLOW)) {
                //現在2個しかないが3個目も赤と同様にしておく
                if (i == 0) {
                    mi.Y1 = mb;
                }
                else if (i == 1) {
                    mi.Y2 = mb;
                }
                else {
                    mi.Y3 = mb;
                }
                i += 1;
            }

            i = 0;
            foreach (var mb in list.Where(x => x.Type == Buzai.RED)) {
                //現在3個の可能性あり
                if (i == 0) {
                    mi.R1 = mb;
                }
                else if (i == 1) {
                    mi.R2 = mb;
                }
                else {
                    mi.R3 = mb;
                }
                i += 1;
            }

            //使用しない部材はnullにする
            mi.NotUseBuzai_to_Null();
        }

        /// <summary>
        ///  配合する各部材の誤差量情報を設定する
        ///  <para>"buzai_master.csv"から情報を取得する</para>
        /// </summary>
        /// <param name="mi">配合情報</param>
        public bool Set_AllowableError(MixInfo mi) {

            string path = $@"{SystemFileFolderPath}\{input_info.ProductType}_buzai_master.csv";

            bool is_ok = true;

            //部材情報の辞書作成
            var dic = Buzai.Get_BuzaiInfo(path, 0);

            //使用する各部材について情報を設定
            foreach (var mb in mi.UseBuzaiList) {
                if (dic.ContainsKey(mb.Fcode)) {
                    var info = dic[mb.Fcode];
                    mb.AllowableErrorGram = info.AllowableErrorGram;
                    mb.AllowableErrorPercent = info.AllowableErrorPercent;
                }
                else {
                    MessageBox.Show($"{mb.Fcode} の部材情報マスタが登録されていません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    is_ok = false;
                }
            }

            return is_ok;
        }

        /// <summary>
        ///  Format名称から各部材情報を設定する
        /// </summary>
        /// <param name="mi"></param>
        public bool Set_BuzaiInfo(string path, MixInfo mi) {

            bool is_ok = true;

            //部材情報の辞書作成
            var dic = Buzai.Get_BuzaiInfo(path, 1);

            //使用する各部材について情報を設定
            foreach (var mb in mi.UseBuzaiList) {
                if (dic.ContainsKey(mb.FormatName)) {
                    var info = dic[mb.FormatName];
                    mb.Fcode = info.Fcode;
                    mb.Name = info.Name;
                    mb.NeedGroveBox = info.NeedGroveBox;
                    mb.AllowableErrorGram = info.AllowableErrorGram;
                    mb.AllowableErrorPercent = info.AllowableErrorPercent;
                }
                else {
                    MessageBox.Show($"{mb.FormatName} の部材情報マスタが登録されていません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    is_ok = false;
                }
            }

            return is_ok;
        }

        /// <summary>
        /// 設定2ボタン処理
        /// </summary>
        private void set_condition2_button_Click(object sender, EventArgs e) {

            //表示カーソル変更
            this.Cursor = Cursors.WaitCursor;

            try {
                //(TS)境川、(TC)CET 3-3Fの設定ファイルに先行評価ログがデータベースかどうか書いてある
                if (AppUsePath.SenkologDatabase) {

                    //過去ログから配合情報取得
                    var koda_resin_sql = new ResinSQL(KodaConStr);
                    var data = koda_resin_sql.Get_LatestSenkoLogData<ResinSQL.ResinSenkoLog_Data>(input_info.ProductName,
                                                                                        input_info.WavelengthRank,
                                                                                        input_info.MoldType,
                                                                                        input_info.SpecialDesignation,
                                                                                        input_info.Conditions,
                                                                                        input_info.Place);

                    //配合情報オブジェクト作成
                    mix_info = new MixInfo {
                        Time = data.InsertAt.DateTime,
                        Conditions = data.Conditions,
                        ResultOrInput = data.ResultOrInput,
                        EvaluationMethod = data.EvaluationMethod,
                        BaseAmountForCalculation = data.BaseAmount,
                        SenkoLogDataRow = data.ExcelSenkoLogRow
                    };

                    //配合情報に部材情報をセットする
                    var mix_buzai_list = JsonConvert.DeserializeObject<List<MixBuzai>>(data.Buzai);
                    Set_UseBuzai(mix_info, mix_buzai_list);
                    Set_AllowableError(mix_info);

                    //ろ過後用配合情報オブジェクト作成
                    if (string.IsNullOrEmpty(data.AfterFiltBuzai) == false) {
                        after_filt_mix_info = new MixInfo();
                        var after_filt_mix_buzai_list = JsonConvert.DeserializeObject<List<MixBuzai>>(data.AfterFiltBuzai);
                        Set_UseBuzai(after_filt_mix_info, after_filt_mix_buzai_list);
                        Set_AllowableError(after_filt_mix_info);
                    }

                    //MixTypecdをデータベースの値で上書き
                    input_info.MixTypeCode = data.MixTypeCode;
                }
                else {//先行評価ログがExcelの場合

                    //過去ログを探す
                    if (File.Exists(AppUsePath.SenkoLog_FilePath) == false) {
                        MessageBox.Show("先行ログファイルが見つかりません。\nネットワーク異常がないか確認して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //過去ログから配合情報取得
                    mix_info = OperateExcel.Get_SenkoLogMixInfo(AppUsePath.SenkoLog_FilePath, input_info);

                    if (mix_info == null) {
                        MessageBox.Show("先行ログにデータがありませんでした。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    //使用しない部材はnullにする
                    mix_info.NotUseBuzai_to_Null();

                    //Excel先行評価ログを使うフローは(TC)CET 3-3Fの照明のみなので
                    mix_info.BaseAmountForCalculation = 100;

                    //部材のフォーマット名称から各情報取得
                    string buzai_master_path = $@"{SystemFileFolderPath}\{input_info.ProductType}_buzai_master.csv";
                    if (Set_BuzaiInfo(buzai_master_path, mix_info) == false) {
                        return;
                    }


                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally {
                this.Cursor = Cursors.Default;
            }

            //先行ログ配合情報をどこから取得したか表示
            senkolog_time_label.Text = "日付時間：" + mix_info.Time.ToString("yyyy/MM/dd HH:mm:ss");
            senkolog_condition_label.Text = "条件：" + mix_info.Conditions;
            senkolog_result_label.Text = "結果/投入指示：" + mix_info.ResultOrInput;
            senkolog_method_label.Text = "評価方法：" + mix_info.EvaluationMethod;
            senkolog_base_amount_label.Text = "ベース量：" + mix_info.BaseAmountForCalculation.ToString("f3");
            senkolog_row_label.Text = mix_info.SenkoLogDataRow == 0 ? "" : "取得行：" + mix_info.SenkoLogDataRow.ToString();
            senkolog_panel.Visible = true;

            //先行ログ情報表示
            Show_MixInfo(mixInfo_dataGridView, mix_info, null, true);
            if (after_filt_mix_info != null) {
                Show_MixInfo(afterfilt_mixInfo_dataGridView, after_filt_mix_info, null, true);
            }

            //各コントロール設定
            condition2_panel.Enabled = false;
            condition2_panel.BackColor = SystemColors.Control;
            input_lot_panel.Enabled = true;
            input_lot_panel.BackColor = Color.PaleTurquoise;
            clear_condition2_button.Enabled = true;

            WriteInputInfoLabel();
        }

        /// <summary>
        /// 条件設定2やり直すボタン処理
        /// </summary>
        private void clear_condition2_button_Click(object sender, EventArgs e) {
            clear_lot_info_button_Click(null, null);

            jouken_comboBox.SelectedIndex = 1;
            jouken_comboBox.SelectedIndex = 0;
            condition2_panel.Enabled = true;
            condition2_panel.BackColor = Color.PaleTurquoise;
            input_lot_panel.Enabled = false;
            input_lot_panel.BackColor = SystemColors.Control;
            senkolog_panel.Visible = false;
            DataGridView_Initialize();

            clear_condition2_button.Enabled = false;
            clear_lot_info_button.Enabled = false;
        }


        /// <summary>
        /// ロット情報確認
        /// </summary>
        private bool Check_LotInfo() {
            input_info.Lots = new List<Lot>();

            for (int i = 0; i < lotinfo_dataGridView.Rows.Count; i++) {
                var val1 = lotinfo_dataGridView.Rows[i].Cells[0].Value;
                var val2 = lotinfo_dataGridView.Rows[i].Cells[1].Value;

                if (val1 != null) {
                    string lotno10 = val1.ToString();

                    string str_num = "aaa";
                    if (val2 != null) {
                        str_num = val2.ToString();
                    }

                    try {
                        int num = int.Parse(str_num);

                        if (num == 0) {
                            MessageBox.Show("基板枚数が0のロットは入力しないで下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            input_info.Lots = null;
                            return false;
                        }

                        input_info.Lots.Add(new Lot {
                            LotNo10 = lotno10,
                            LF_Num = num
                        });
                    }
                    catch (Exception ex) {
                        MessageBox.Show("基板枚数を数字で入力して下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        input_info.Lots = null;
                        return false;
                    }
                }
            }

            if (input_info.Lots.Count == 0) {
                MessageBox.Show("ロット番号/基板枚数を入力して下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                input_info.Lots = null;
                return false;
            }
            else {
                input_lot_panel.Enabled = false;
                input_lot_panel.BackColor = SystemColors.Control;
                return true;
            }
        }

        /// <summary>
        /// 配合比取得ボタン処理
        /// </summary>
        private void get_mix_info_button_Click(object sender, EventArgs e) {

            if (Check_LotInfo() == false) { return; }

            //マスタファイルからカップ作業規制時間を取得する
            input_info.WorkRegulationDatas = Get_CupWorkRegulation($@"{SystemFileFolderPath}\PMMSTimeSetting.txt", input_info.ProductName, input_info.MixTypeCode, "temp-cupno");
            if (input_info.WorkRegulationDatas.Count == 0) {
                MessageBox.Show("作業規制時間を取得できませんでした。\nマスタファイルに登録がされていません。", "マスタ情報無し", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            try {
                decimal make_amount;//樹脂作製量

                //(TS)境川の場合は現在resinA取得ファイルが無い
                if (input_info.Place == "TS") {
                    make_amount = make_amount_numericUpDown.Value;

                    if (make_amount <= 0) {
                        MessageBox.Show("作製樹脂量を0より大きい値を入力して下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                else {

                    if (input_info.Conditions.Contains("First")) {
                        make_amount = 20;
                    }
                    else {
                        //樹脂量一覧表からA剤量取得
                        string get_resinA_amount_path = $@"{SystemFileFolderPath}\get_resinA_amount.xlsm";
                        make_amount = Get_ResinA_Amount(get_resinA_amount_path, input_info);

                        if (make_amount == -1) {
                            MessageBox.Show($"作製樹脂量が取得出来ませんでした。\n樹脂量取得ファイルに当てはまる機種名がありません。\n\n樹脂量取得ファイル\n{get_resinA_amount_path}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        else if (make_amount == -2) {
                            MessageBox.Show("作製樹脂量が取得出来ませんでした。\nこの基板枚数は樹脂量取得出来る上限を超えています。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        else if (make_amount == -3) {
                            MessageBox.Show("作製樹脂量が取得出来ませんでした。\nこの基板枚数での樹脂量は樹脂量取得ファイルに記載がありませんので出来ません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    //作製樹脂量表示
                    make_amount_numericUpDown.Value = make_amount;
                }

                //作製樹脂量から実際に配合する量を計算
                mix_info.Calculate_MixAmount(mix_info.BaseAmountForCalculation, make_amount);

                //配合の順番をセットする
                if (input_info.MoldType.Contains("MD")) {
                    mix_info.Set_MD_MixOrder();
                }
                else {
                    mix_info.Set_SDR_MixOrder();
                    if (after_filt_mix_info != null) {
                        after_filt_mix_info.Set_AfterFilt_SDR_MixOrder();
                    }
                }

                //自動機情報を取得する
                AutoMachine auto_machine = null;
                if (exist_autoMachine_checkBox.Checked) {
                    auto_machine = AutoMachine.Get_SetBuzai(AppUsePath.AutoMachine_RemainingWeight_FilePath);
                }

                //各部材が自動配合かどうか確認する
                Check_UseAutoMachine(mix_info, auto_machine);

                //樹脂カップ配合手段を決定する
                flow_mode = mix_info.Get_FlowMode();

                //配合手段がMANUALの場合、全ての部材を手配合にする
                //一部の部材は自動機で出来ても、条件によってMANUALである場合があるため
                if (flow_mode == Recipe.MANUAL) {
                    foreach (var mb in mix_info.UseBuzaiList) {
                        mb.UseAutoMachine = false;
                    }
                    if (after_filt_mix_info != null) {
                        foreach (var mb in after_filt_mix_info.UseBuzaiList) {
                            mb.UseAutoMachine = false;
                        }
                    }
                }

                //各部材の誤差量計算
                mix_info.Calculate_AllowableErrorGram();
                if (after_filt_mix_info != null) {
                    after_filt_mix_info.Calculate_AllowableErrorGram();
                }

                //データグリッドビューとかに表示
                Show_MixInfo(mixInfo_dataGridView, mix_info, auto_machine);
                if (after_filt_mix_info != null) {
                    Show_MixInfo(afterfilt_mixInfo_dataGridView, after_filt_mix_info, auto_machine);
                }

                //レシピファイル作成ボタン押せるようにする
                create_recipe_panel.Enabled = true;
                create_recipe_panel.BackColor = Color.PaleTurquoise;
                clear_lot_info_button.Enabled = true;

                //AUTOの場合にADDに変更できるチェックボックスを表示する
                change_to_ADD_checkBox.Checked = false;
                if (flow_mode == Recipe.AUTO) {
                    change_to_ADD_checkBox.Visible = true;
                }
                else {
                    change_to_ADD_checkBox.Visible = false;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                if (create_recipe_panel.Enabled == false) {
                    input_lot_panel.Enabled = true;
                    input_lot_panel.BackColor = Color.PaleTurquoise;
                }
            }
        }

        /// <summary>
        /// 部材毎に自動配合機を使用するかどうか確認する
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="am"></param>
        public void Check_UseAutoMachine(MixInfo mi, AutoMachine am) {

            foreach (var mb in mi.UseBuzaiList) {

                if (am == null) {
                    mb.UseAutoMachine = false;
                    continue;
                }

                //配合使用部材と品番とロット番号が同じである自動配合機にセットされている部材を取得
                //マルコム自動配合機では部材名のスラッシュを抜いた名前で登録されているので、検索名をセット
                var malcom_name = mb.Name.Replace("/", "");
                var sb_list = am.SetBuzaiList.Where(x => x.Name == malcom_name && x.LotNo == mb.LotNo).ToList();

                if (sb_list.Count > 0) {
                    mb.UseAutoMachine = true;
                }
                else {
                    mb.UseAutoMachine = false;
                    continue;
                }

                //自動配合機で出来るフィラー量は6gまで
                if (mb.Name == "FUSELEX E-2") {
                    if (mb.Amount > 6) {
                        mb.UseAutoMachine = false;
                        continue;
                    }
                }

                //自動機に残量が足りない場合は手配合にする
                foreach (var sb in sb_list) {
                    if ((decimal)sb.RemainingWeight < mb.Amount * 3) {
                        sb.Shortage = true;
                        mb.UseAutoMachine = false;
                    }
                }
            }
        }

        /// <summary>
        /// 配合情報を表示
        /// </summary>
        public void Show_MixInfo(DataGridView dgv, MixInfo mi, AutoMachine am, bool pre_show = false) {

            if (mi.ResinA != null) { write_dataGridView(dgv, mi.ResinA, 0, am, pre_show); }
            if (mi.ResinB != null) { write_dataGridView(dgv, mi.ResinB, 1, am, pre_show); }
            if (mi.Filler != null) { write_dataGridView(dgv, mi.Filler, 2, am, pre_show); }
            if (mi.Toro != null) { write_dataGridView(dgv, mi.Toro, 3, am, pre_show); }
            if (mi.TiO2 != null) { write_dataGridView(dgv, mi.TiO2, 4, am, pre_show); }
            if (mi.Y1 != null) { write_dataGridView(dgv, mi.Y1, 5, am, pre_show); }
            if (mi.Y2 != null) { write_dataGridView(dgv, mi.Y2, 6, am, pre_show); }
            if (mi.Y3 != null) { write_dataGridView(dgv, mi.Y3, 7, am, pre_show); }
            if (mi.R1 != null) { write_dataGridView(dgv, mi.R1, 8, am, pre_show); }
            if (mi.R2 != null) { write_dataGridView(dgv, mi.R2, 9, am, pre_show); }
            if (mi.R3 != null) { write_dataGridView(dgv, mi.R3, 10, am, pre_show); }

            //配合比書いてある行の色を変える
            if (pre_show == false) {
                dgv.Rows[4].DefaultCellStyle.BackColor = Color.Yellow;
            }

            //データグリッドビューのサイズ調整
            int width = 0;
            for (int i = 0; i < dgv.Columns.Count; i++) {
                width += dgv.Columns[i].Width;
            }
            dgv.Width = dgv.RowHeadersWidth + width + 5;

            int height = 0;
            for (int i = 0; i < dgv.Rows.Count; i++) {
                height += dgv.Rows[i].Height;
            }
            dgv.Height = dgv.ColumnHeadersHeight + height + 2;

            //自動配合機セット部材表示
            if (am != null) {
                var sb_list = am.SetBuzaiList.OrderBy(x => x.PumpNo).ToList();
                for (int i = 0; i < sb_list.Count; i++) {
                    var sb = sb_list[i];
                    autoMachine_setBuzai_dataGridView.Rows[i].Cells[0].Value = sb.PumpNo;
                    autoMachine_setBuzai_dataGridView.Rows[i].Cells[1].Value = sb.Name;
                    autoMachine_setBuzai_dataGridView.Rows[i].Cells[2].Value = sb.LotNo;
                    autoMachine_setBuzai_dataGridView.Rows[i].Cells[3].Value = sb.RemainingWeight;
                }
            }

            //配合手段表示
            flow_mode_label.Text = flow_mode;
        }

        /// <summary>
        /// データグリッドビューに配合情報を書く
        /// </summary>
        /// <param name="buzai"></param>
        /// <param name="colno">対象部材の列番号</param>
        /// <param name="am"></param>
        private void write_dataGridView(DataGridView dgv, MixBuzai buzai, int colindex, AutoMachine am, bool pre_show) {

            dgv.Rows[0].Cells[colindex].Value = buzai.FormatName;
            dgv.Rows[1].Cells[colindex].Value = buzai.Name;
            dgv.Rows[2].Cells[colindex].Value = buzai.LotNo;

            object val;
            if (buzai.BaseAmount == null) {
                val = $"{buzai.PercentOfTotalWeight}%";
            }
            else {
                val = ((double)buzai.BaseAmount).ToString("f3");
            }

            dgv.Rows[3].Cells[colindex].Value = val;
            if (pre_show == false) {
                dgv.Rows[4].Cells[colindex].Value = buzai.Amount.ToString("f3");
                dgv.Rows[5].Cells[colindex].Value = buzai.AllowableErrorGram;
                dgv.Rows[6].Cells[colindex].Value = buzai.MixOrder.ToString();
            }

            //同じ品番、LotNoの部材が自動配合機にセットされているなら、部材残量表示
            //マルコム自動配合機では部材名のスラッシュを抜いた名前で登録されているので、検索名をセット
            var malcom_name = buzai.Name.Replace("/", "");
            if (am != null) {
                var a = am.SetBuzaiList.Where(x => x.Name == malcom_name && x.LotNo == buzai.LotNo);
                if (a.Any()) { dgv.Rows[7].Cells[colindex].Value = a.First().RemainingWeight; }
            }

            //自動配合機使う部材列の色を変える
            if (buzai.UseAutoMachine) {
                dgv.Columns[colindex].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            }

            //GroveBox使う部材列の色を変える
            if (buzai.NeedGroveBox) {
                dgv.Columns[colindex].DefaultCellStyle.BackColor = Color.Tomato;
            }

        }

        /// <summary>
        /// ロット情報入力やり直すボタン処理
        /// </summary>
        private void clear_lot_info_button_Click(object sender, EventArgs e) {
            create_recipe_panel.Enabled = false;
            create_recipe_panel.BackColor = SystemColors.Control;
            input_lot_panel.Enabled = true;
            input_lot_panel.BackColor = Color.PaleTurquoise;

            DataGridView_Initialize();
        }



        /// <summary>
        /// レシピファイル作成ボタン処理
        /// </summary>
        private void create_recipe_button_Click(object sender, EventArgs e) {

            string printer_name = "";
            if (print_label_checkBox.Checked) {
                printer_name = BrotherLabelPrint.CheckPrinterOnline();
                if (string.IsNullOrEmpty(printer_name)) {
                    MessageBox.Show("使用可能なラベルプリンターがありません。\n電源が入っていないか、接続異常の可能性があります。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            //レシピ用オブジェクト作成
            var recipe = new Recipe {
                ProductName = input_info.ProductName,
                WavelengthRank = input_info.WavelengthRank,
                LotNoList = input_info.Lots.Select(x => x.LotNo10).ToList(),
                FlowMode = flow_mode,
                MoldType = input_info.MoldType,
                MixTypeCode = input_info.MixTypeCode,
                ManualMode_MixTypeCode = input_info.MixTypeCode,
                LF_Num = input_info.TotalLFNum,
                SpecialDesignation = input_info.SpecialDesignation,
                CupWorkRegulations = input_info.WorkRegulationDatas,
                Henkaten = "",
            };

            //Recipeに必要なものを選択して再作成(通常部材)
            var normal_buzai = new List<RecipeBuzai>();
            foreach (var mb in mix_info.UseBuzaiList.Where(x => x.NeedGroveBox == false)) {
                var rb = new RecipeBuzai {
                    Fcode = mb.Fcode,
                    Name = mb.Name,
                    Type = mb.Type,
                    LotNo = mb.LotNo,
                    NeedGroveBox = mb.NeedGroveBox,
                    UpperAllowableErrorGram = mb.UpperAllowableErrorGram,
                    LowerAllowableErrorGram = mb.LowerAllowableErrorGram,
                    Amount = mb.Amount,
                    UseAutoMachine = mb.UseAutoMachine,
                    MixOrder = mb.MixOrder
                };
                normal_buzai.Add(rb);
            }

            //Recipeに必要なものを選択して再作成(グローブボックス必要部材)
            var ksf_buzai = new List<RecipeBuzai>();
            foreach (var mb in mix_info.UseBuzaiList.Where(x => x.NeedGroveBox == true)) {
                var rb = new RecipeBuzai {
                    Fcode = mb.Fcode,
                    Name = mb.Name,
                    Type = mb.Type,
                    LotNo = mb.LotNo,
                    NeedGroveBox = mb.NeedGroveBox,
                    UpperAllowableErrorGram = mb.UpperAllowableErrorGram,
                    LowerAllowableErrorGram = mb.LowerAllowableErrorGram,
                    Amount = mb.Amount,
                    UseAutoMachine = mb.UseAutoMachine,
                    MixOrder = mb.MixOrder
                };
                ksf_buzai.Add(rb);
            }
            //KSF使う場合はMANUAL以外は手配合用レシピファイルを2個作成するため、変数を用意しておく
            var is_2file = false;
            if (ksf_buzai.Count > 0) { is_2file = true; }



            //Recipeに必要なものを選択して再作成(ろ過後用部材)
            var afterfilt_usebuzai = new List<RecipeBuzai>();
            if (after_filt_mix_info != null) {
                foreach (var mb in after_filt_mix_info.UseBuzaiList) {
                    var rb = new RecipeBuzai {
                        Fcode = mb.Fcode,
                        Name = mb.Name,
                        Type = mb.Type,
                        LotNo = mb.LotNo,
                        NeedGroveBox = mb.NeedGroveBox,
                        UpperAllowableErrorGram = mb.UpperAllowableErrorGram,
                        LowerAllowableErrorGram = mb.LowerAllowableErrorGram,
                        Amount = mb.Amount,
                        UseAutoMachine = mb.UseAutoMachine,
                        MixOrder = mb.MixOrder,
                        PercentOfTotalWeight = mb.PercentOfTotalWeight
                    };
                    afterfilt_usebuzai.Add(rb);
                }
            }

            //配合手段によってMixTypeCode 書き換え(101→103,104,105,106)
            Set_MixTypeCode(recipe, is_2file);
            mixtypecd_label.Text = recipe.MixTypeCode;
            mixtypecd_label.Update();




            var koda_resin_sql = new ResinSQL(KodaConStr);

            //次のカップNo採番する
            try {
                recipe.CupNo = koda_resin_sql.Insert_CupNo(input_info.Place, recipe);
            }
            catch (Exception ex) {
                var msg = $"{ex.Message}\nカップ番号の採番に失敗しました。\nネットワーク異常がないか確認して下さい。";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //カップ番号表示
            create_cupno_label.Text = $"{recipe.CupNo.Substring(0, 2)} {recipe.CupNo.Substring(2, 6)} {recipe.CupNo.Substring(8)}";
            create_cupno_label.Update();

            //作業規制時間取得の際に仮のカップ番号にしといたので書き換え
            foreach (var cwr in recipe.CupWorkRegulations) {
                cwr.cupno = recipe.CupNo;
            }

            //PMMS TmWorkRegulationデータベースに書き込む
            string err_msg = "";
            while (true) {
                bool success = PMMS.Insert_CupTimeInfo_to_TmWorkRegulation(recipe.CupWorkRegulations, ref err_msg);
                if (success) {
                    break;
                }
                else {
                    var msg = $"{err_msg}\n\nPMMS 作業規制時間書き込みに失敗しました。\nネットワーク異常がないか確認して下さい。" +
                            $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                    var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (ret == DialogResult.No) {
                        MessageBox.Show("差立てを中断します。\nこの樹脂カップ番号は使用しないで下さい。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            //PMMS CupInfoデータベースに書き込む
            err_msg = "";
            var cupinfo_list = new List<TnCupInfo> {
                new TnCupInfo { cupno = recipe.CupNo, mixtypecd = recipe.MixTypeCode }
            };
            while (true) {
                bool success = PMMS.Insertmixtypecd(cupinfo_list, ref err_msg);
                if (success) {
                    break;
                }
                else {
                    var msg = $"{err_msg}\n\nPMMS カップ情報書き込みに失敗しました。\nネットワーク異常がないか確認して下さい。" +
                            $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                    var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (ret == DialogResult.No) {
                        MessageBox.Show("差立てを中断します。\nこの樹脂カップ番号は使用しないで下さい。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }



            string all_recipe_json = "";
            var print_recipe_cupno_list = new List<string>();

            //手配合ソフト用にレシピファイル作成＋保存
            string manual_recipe_path;
            if (recipe.FlowMode == Recipe.AUTO) {

                if (is_2file) {
                    //配合済みフォルダ(already-made)に入れておく
                    manual_recipe_path = $@"{AppUsePath.AlreadyMadeRecipeFolderPath}\{recipe.CupNo}-1{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = normal_buzai;
                    var s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += s;
                    }

                    manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}-2{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = ksf_buzai;
                    s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += $",{s}";
                        print_recipe_cupno_list.Add($"{recipe.CupNo}-2");
                    }
                }
                else {
                    //配合済みフォルダ(already-made)に入れておく
                    manual_recipe_path = $@"{AppUsePath.AlreadyMadeRecipeFolderPath}\{recipe.CupNo}{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = normal_buzai;
                    var s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += s;
                        print_recipe_cupno_list.Add($"{recipe.CupNo}");
                    }
                }
            }
            else if (recipe.FlowMode == Recipe.MANUAL) {
                manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}{_rcp_json}";
                //レシピ部材セット
                recipe.UseBuzai = normal_buzai;
                recipe.UseBuzai.AddRange(ksf_buzai);
                var s = Save_ManualRecipe(recipe, manual_recipe_path);
                if (s == null) {
                    return;
                }
                else {
                    all_recipe_json += s;
                    print_recipe_cupno_list.Add($"{recipe.CupNo}");
                }
            }
            //ADD,CUT_INの場合
            else {

                if (is_2file) {
                    manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}-1{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = normal_buzai;
                    var s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += s;
                        print_recipe_cupno_list.Add($"{recipe.CupNo}-1");
                    }

                    manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}-2{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = ksf_buzai;
                    s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += $",{s}";
                        print_recipe_cupno_list.Add($"{recipe.CupNo}-2");
                    }
                }
                else {
                    manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}{_rcp_json}";
                    //レシピ部材セット
                    recipe.UseBuzai = normal_buzai;
                    var s = Save_ManualRecipe(recipe, manual_recipe_path);
                    if (s == null) {
                        return;
                    }
                    else {
                        all_recipe_json += s;
                        print_recipe_cupno_list.Add($"{recipe.CupNo}");
                    }
                }
            }



            //配合自動機用にレシピファイル作成＋保存
            if (recipe.FlowMode != Recipe.MANUAL) {

                while (true) {
                    try {
                        string auto_recipe_path;
                        if (recipe.FlowMode == Recipe.AUTO) {
                            auto_recipe_path = $@"{AppUsePath.AutoMachine_SaveFolderPath.Auto_SaveFolderPath}\{recipe.ProductName}";
                            //無かったらフォルダを作る
                            if (Directory.Exists(auto_recipe_path) == false) {
                                Directory.CreateDirectory(auto_recipe_path);
                            }
                            auto_recipe_path = $@"{auto_recipe_path}\F{recipe.CupNo}{_rcp_txt}";
                        }
                        else {
                            auto_recipe_path = $@"{AppUsePath.AutoMachine_SaveFolderPath.Add_SaveFolderPath}\F{recipe.CupNo}{_rcp_txt}";
                        }

                        //レシピ部材セット
                        recipe.UseBuzai = new List<RecipeBuzai>(normal_buzai);
                        recipe.UseBuzai.AddRange(ksf_buzai);

                        Recipe.CreateRecipe_for_AutoMachine(recipe, auto_recipe_path, change_to_ADD_checkBox.Checked);
                        break;
                    }
                    catch (Exception ex) {
                        var msg = $"{ex.Message}\n配合自動機用レシピ保存に失敗しました。\nネットワーク異常がないか確認して下さい。" +
                            $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                        var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (ret == DialogResult.No) {
                            MessageBox.Show("差立てを中断します。\nこの樹脂カップ番号は使用しないで下さい。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                }
            }


            //ろ過後配合用ファイル作成＋保存
            if (after_filt_mix_info != null && recipe.MoldType == "SDRF") {
                manual_recipe_path = $@"{AppUsePath.ManualRecipe_SaveFolderPath}\{recipe.CupNo}{_rcp_filt_json}";
                //レシピ部材セット
                recipe.UseBuzai = afterfilt_usebuzai;
                var s = Save_ManualRecipe(recipe, manual_recipe_path);
                if (s == null) {
                    return;
                }
                else {
                    all_recipe_json += $",{s}";
                }
            }


            //KODAデータベースにレシピ内容を書き込む
            while (true) {
                try {
                    all_recipe_json = "[" + all_recipe_json + "]";//JSONのListにするため
                    koda_resin_sql.Update_CupnoRecipe(recipe.CupNo, all_recipe_json);
                    break;
                }
                catch (Exception ex) {
                    var msg = $"{ex.Message}\nKODAデータベースへのレシピ書き込みに失敗しました。\nネットワーク異常がないか確認して下さい。" +
                        $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                    var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (ret == DialogResult.No) {
                        MessageBox.Show("差立てを中断します。\nこの樹脂カップ番号は使用しないで下さい。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            //ラベル印刷
            if (print_label_checkBox.Checked) {
                string labelfile_path = $@"{AppFolder}\label\{print_1_recipelabel}";
                //条件によってラベル印刷QR数が違う
                if (is_2file) {
                    if (recipe.FlowMode == Recipe.ADD || recipe.FlowMode == Recipe.CUT_IN) {
                        labelfile_path = $@"{AppFolder}\label\{print_2_recipelabel}";
                    }
                }
                while (true) {
                    try {
                        BrotherLabelPrint.PrintRecipeNoLabel(labelfile_path, recipe, print_recipe_cupno_list, printer_name);
                        break;
                    }
                    catch (Exception ex) {
                        var msg = $"{ex.Message}\nラベル印刷に失敗しました。\nラベルプリンターと接続異常がないか確認して下さい。" +
                            $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\n[いいえ]を押した場合は手書きで樹脂レシピ番号を記載して下さい。";
                        var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (ret == DialogResult.No) {
                            string recipe_number = "";
                            foreach (var s in print_recipe_cupno_list) {
                                recipe_number += s + "\n";
                            }
                            MessageBox.Show($"樹脂レシピ番号を記載して下さい。\n\n{recipe_number}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                }

                //ADDに変更するチェックボックスがONの時
                if (change_to_ADD_checkBox.Checked) {
                    while (true) {
                        try {
                            //自動配合機に投入する空カップ用のラベル印刷する
                            BrotherLabelPrint.PrintResinCupLabel($@"{AppFolder}\label\{resin_cupno_label}", recipe, printer_name);
                            break;
                        }
                        catch (Exception ex) {
                            var msg = $"{ex.Message}\nラベル印刷に失敗しました。\nラベルプリンターと接続異常がないか確認して下さい。" +
                                $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\n[いいえ]を押した場合は配合ソフトからラベル印刷して下さい。";
                            var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (ret == DialogResult.No) {
                                MessageBox.Show($"配合ソフトで下記の樹脂レシピ番号でラベル印刷して下さい。\n\n{print_recipe_cupno_list[0]}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                    }
                }
            }

            create_recipe_panel.Enabled = false;
            MessageBox.Show($"{recipe.CupNo}で差立て完了しました。", "complete!");
        }

        /// <summary>
        /// 配合手段に応じてMixTypeCodeを設定する
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="is_ksf"></param>
        private void Set_MixTypeCode(Recipe recipe, bool is_ksf) {

            if (recipe.MixTypeCode == "101") {

                if (recipe.FlowMode == Recipe.MANUAL) { return; }

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
        }

        /// <summary>
        /// 手配合レシピを保存する
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="path"></param>
        /// <returns>レシピファイルの中身の文字列</returns>
        private string Save_ManualRecipe(Recipe recipe, string path) {
            string json;
            while (true) {
                try {
                    json = Recipe.CreateRecipe_for_HaigoSoft(recipe, path);
                    return json;
                }
                catch (Exception ex) {
                    var msg = $"{ex.Message}\n手配合用レシピ保存に失敗しました。\nネットワーク異常がないか確認して下さい。" +
                        $"\n\nリトライしますか？\n基本的に[はい]を押して下さい。\nリトライしてもだめな場合はこの状態のまま管理者に連絡して下さい。";
                    var ret = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (ret == DialogResult.No) {
                        MessageBox.Show("差立てを中断します。\nこの樹脂カップ番号は使用しないで下さい。", "Stop", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return null;
                    }
                }
            }
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
        /// 入力情報をラベルに書く
        /// </summary>
        public void WriteInputInfoLabel() {
            input_info_label.Text = input_info.ProductName;
            input_info_label.Text += $" <{input_info.WavelengthRank}> {input_info.MoldType}/{input_info.MixTypeCode}/{input_info.SpecialDesignation}";
            if (!string.IsNullOrEmpty(input_info.Conditions)) {
                input_info_label.Text += $" {OperateExcel.conditions_dic[input_info.Conditions]}";
            }
        }



        private void button1_Click(object sender, EventArgs e) {
            /*デバッグ用ボタン*/

            string senkolog_path = $@"D:\Documents and Settings\jnk-nkmr\デスクトップ\EMS製品部\異動_引継ぎ資料\平田氏\Jushi\Create_Recipe\bin\Debug\system_file\SenkoLog4_CET.xlsm";
            string buzai_master_path = $@"{SystemFileFolderPath}\HW_buzai_master.csv";


            ////Excelログをデータベースへ
            OperateExcel.Write_ExcelSenkoLog_to_Databsase(senkolog_path, buzai_master_path, start_row: 4239, end_row: 5000, btn: button1);

            MessageBox.Show("終了");

            /*
            input_info = new InputInfo { ProductName = "CS0809H1201E1W302H7X5", Lots = new List<Lot> { new Lot { LF_Num = 101 } }, SpecialDesignation = "" };

            string get_resinA_amount_path = $@"{SystemFileFolderPath}\1get_resinA_amount.xlsm";
            var make_amount = Get_ResinA_Amount(get_resinA_amount_path, input_info);
            */

        }
    }


    /// <summary>
    /// このレシピ作成アプリ(差立てExcelの代わり)の入力情報クラス
    /// </summary>
    public class InputInfo {

        /// <summary>
        /// 製造場所
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 機種名
        /// </summary>
        public string ProductName { get; set; }

        public int TotalLFNum {
            get {
                int num = Lots.Sum(x => x.LF_Num);
                return num;
            }
        }

        /// <summary>
        /// 製品種類
        /// <para>HW,LW,...</para>
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 製品Lot情報
        /// </summary>
        public List<Lot> Lots { get; set; }

        /// <summary>
        /// 波長ランク
        /// </summary>
        public string WavelengthRank { get; set; }

        /// <summary>
        /// 樹脂種類タイプ
        /// <para>樹脂(MD) or ダム(SDR)</para>
        /// </summary>
        public string MoldType { get; set; }

        /// <summary>
        /// 樹脂作業手順コード
        /// <para>1,2,3,...101,102,...</para>
        /// </summary>
        public string MixTypeCode { get; set; }

        /// <summary>
        /// 条件
        /// <para>本番確定 or 本番仮 or 先行評価条件(配合比①) or 先行評価条件(配合比②) or 先行評価条件(配合比③) or 先行評価条件(配合比④)</para>
        /// </summary>
        public string Conditions { get; set; }


        /// <summary>
        /// 特殊指定
        /// <para>"" or DIF or Warm or Cool</para>
        /// </summary>
        public string SpecialDesignation { get; set; }

        /// <summary>
        /// 作業規制時間
        /// </summary>
        public List<TmWorkRegulationData> WorkRegulationDatas { get; set; }

    }

    public class Lot {

        /// <summary>
        /// 10桁ロット番号
        /// </summary>
        public string LotNo10 { get; set; }

        /// <summary>
        /// 18桁ロット番号
        /// </summary>
        public string LotNo18 { get; set; }

        /// <summary>
        /// 基板枚数
        /// </summary>
        public int LF_Num { get; set; }
    }
}
