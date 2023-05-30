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
using System.Windows.Forms.DataVisualization.Charting;
using System.Security.Permissions;

using static Dage_Collection.Program;
using static Dage_Collection.OperateFile;
using static Dage_Collection.Statistics;
using static Dage_Collection.TrendConstant;




namespace Dage_Collection {
    public partial class MainForm : Form {

        /// <summary>
        /// 測定データフォルダ、結果保存先フォルダが書いてあるファイル
        /// </summary>
        public string fldNameFilePath { get; private set; } = $"{SettingsFolder}/FolderPass.txt";

        /// <summary>
        /// 結果保存先フォルダ
        /// </summary>
        public string Result_Folder { get; private set; }

        /// <summary>
        /// 測定データフォルダ
        /// </summary>
        public string Measurement_Folder { get; private set; }


        //コンストラクター
        public MainForm() {
            InitializeComponent();
        }

        // フォームの×ボタンなどで閉じられなくする
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                const int CS_NOCLOSE = 0x200;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;

                return cp;
            }
        }

        //フォームロードイベント
        private void MainForm_Load(object sender, EventArgs e) {
            // Dage測定データ保存フォルダ設定
            Measurement_Folder_ini(sender, e);
            // 結果保存先フォルダ設定
            Result_Folder_ini(sender, e);
            //データグリッドビュー初期化
            DataGridview_ini();
            //ラベルテキスト初期化
            Label_Text_Clear();
            //グラフ初期化
            Graph_Clear();
            //ボタン初期化
            Button_ini();
        }



        /// <summary>
        /// 測定データフォルダ初期読み込み
        /// </summary>
        private void Measurement_Folder_ini(object sender, EventArgs e) {
            try {
                Measurement_Folder = Read_Line(fldNameFilePath, 2); // 2行目を読み込む
            }
            catch {
                MessageBox.Show($"測定データフォルダの読み込みに失敗しました。\n\n読み込みファイル:{fldNameFilePath}\nがおかしくないか調べて下さい。",
                    "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Application.Exit(); // フォルダパスが読み込めない場合
            }

            if (Directory.Exists(Measurement_Folder)) {
                FolderBrowserDialog1.SelectedPath = Measurement_Folder;
                MeasurementFolder_label.Text = Measurement_Folder;
            }
            else {
                MessageBox.Show("測定データが保存してあるフォルダが見つかりません。\n再設定が必要です。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SelectPath_Measure_Click(sender, e);
            }
        }

        /// <summary>
        /// 測定データ保存フォルダ選択ボタン処理
        /// </summary>
        private void SelectPath_Measure_Click(object sender, EventArgs e) {
            FolderBrowserDialog1.Description = "測定データが保存してあるフォルダを選択してください。";

            if (Directory.Exists(Measurement_Folder))
                FolderBrowserDialog1.SelectedPath = Measurement_Folder;    // 初期選択するパスの設定
            else
                FolderBrowserDialog1.SelectedPath = @"C:\";// 初期選択するパスの設定

            if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                Measurement_Folder = FolderBrowserDialog1.SelectedPath;
                MeasurementFolder_label.Text = Measurement_Folder;

                //選択したフォルダパスをファイルに保存
                try {
                    Write_Line(fldNameFilePath, 2, Measurement_Folder);
                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをファイルに保存するのを失敗しました。\n\nファイル:{fldNameFilePath}\nがおかしくないか調べて下さい。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (e.ToString() == "System.EventArgs") {
                MessageBox.Show("終了します。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
        }

        /// <summary>
        /// 結果保存先フォルダ初期読み込み
        /// </summary>
        private void Result_Folder_ini(object sender, EventArgs e) {
            try {
                Result_Folder = Read_Line(fldNameFilePath, 4); // 4行目を読み込む
            }
            catch {
                MessageBox.Show($"結果保存先フォルダの読み込みに失敗しました。\n\n読み込みファイル:{fldNameFilePath}\nがおかしくないか調べて下さい。",
                    "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Application.Exit(); // フォルダパスが読み込めない場合
            }

            if (Directory.Exists(Result_Folder)) {
                FolderBrowserDialog2.SelectedPath = Result_Folder;
                ResultFolder_label.Text = Result_Folder;
            }
            else {
                MessageBox.Show("結果保存先フォルダが見つかりません。\n再設定が必要です。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SelectPass_Result_Click(sender, e);
            }
        }

        /// <summary>
        /// 結果保存先フォルダ選択ボタン処理
        /// </summary>
        private void SelectPass_Result_Click(object sender, EventArgs e) {
            FolderBrowserDialog2.Description = "結果保存先にするフォルダを選択してください。";

            if (Directory.Exists(Result_Folder))
                FolderBrowserDialog2.SelectedPath = Result_Folder;    // 初期選択するパスの設定
            else
                FolderBrowserDialog2.SelectedPath = @"C:\";// 初期選択するパスの設定

            if (FolderBrowserDialog2.ShowDialog() == DialogResult.OK) {
                Result_Folder = FolderBrowserDialog2.SelectedPath;
                ResultFolder_label.Text = Result_Folder;

                //選択したフォルダパスをファイルに保存
                try {
                    Write_Line(fldNameFilePath, 4, Result_Folder);
                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをファイルに保存するのを失敗しました。\n\nファイル:{fldNameFilePath}\nがおかしくないか調べて下さい。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (e.ToString() == "System.EventArgs") {
                MessageBox.Show("終了します。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
        }

        /// <summary>
        /// データグリッドビュー初期化(30行追加)
        /// </summary>
        private void DataGridview_ini() {
            for (int i = 0; i < 30; i++) {
                // データ行追加
                DataGridView1.Rows.Add();
                // 行ヘッダー追加
                DataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        /// <summary>
        /// ラベルテキスト初期化
        /// </summary>
        private void Label_Text_Clear() {
            Serial_label.Text = "-";
            Date_label.Text = "-";
            Time_label.Text = "-";
            TestGroup_label.Text = "-";
            Chip_Place_label.Text = "-";
            Bonder_label.Text = "-";
            Operator_label.Text = "-";
            Average_label.Text = "NaN";
            Max_label.Text = "NaN";
            Min_label.Text = "NaN";
            Sigma_label.Text = "NaN";
            Range_label.Text = "NaN";
            Management_value_label.Text = "";
            Judge_label.Text = "-";
            X_UCL_label.Text = "-";
            X_CL_label.Text = "-";
            X_LCL_label.Text = "-";
            R_UCL_label.Text = "-";
            R_CL_label.Text = "-";
            R_LCL_label.Text = "-";
        }

        /// <summary>
        /// グラフ初期化
        /// </summary>
        private void Graph_Clear() {
            for (int i = 0; i < 4; i++) {
                Chart1.Series[i].Points.Clear(); // 初期準備としてChart1に系列を4個作成しておく
                Chart1.Invalidate(); // グラフ再描画
                Chart2.Series[i].Points.Clear(); // 初期準備としてChart1に系列を4個作成しておく
                Chart2.Invalidate(); // グラフ再描画
            }
        }

        /// <summary>
        /// ボタン初期化
        /// </summary>
        private void Button_ini() {
            Button_Off(DataCheck_Button);//データ確認ボタン
            Button_Off(DataChange_Button);//データ修正ボタン
            Button_Off(DieSahreSave_Button);//DBデータ保存ボタン
            Button_Off(WB_Data_Save_Button);//WBデータ保存ボタン
            Button_Off(Save_as_Trial_Button);//試打ちとして保存ボタン
            Button_Off(Done_Button);//完了ボタン
            Button_On(Close_Button);//ソフト終了ボタン
        }

        /// <summary>
        /// ボタンをONにする処理
        /// </summary>
        private void Button_On(Control button) {
            button.Enabled = true;
            button.BackColor = Color.Yellow;
        }

        /// <summary>
        /// ボタンをOFFにする処理
        /// </summary>
        private void Button_Off(Control button) {
            button.Enabled = false;
            button.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// ラジオボタン初期化
        /// </summary>
        public void RadioButton_ini() {
            foreach (RadioButton r in DS_Low_Limit_GBox.Controls) {
                r.Checked = false;
            }
            foreach (RadioButton r in BS_Low_Limit_GBox.Controls) {
                r.Checked = false;
            }
            foreach (RadioButton r in PC_Low_Limit_GBox.Controls) {
                r.Checked = false;
            }
        }

        /// <summary>
        /// グループボックスON処理
        /// </summary>
        private void GroupBox_On(Control groupbox) {
            groupbox.Enabled = true;
            groupbox.BackColor = Color.Yellow;
        }

        /// <summary>
        /// グループボックスOFF処理
        /// </summary>
        private void GroupBox_Off(Control groupbox) {
            groupbox.Enabled = false;
            groupbox.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// QRコードで読み込んだ機種名/LotNo(スペースも含めた35文字)
        /// <para>ファイル名検索で使用</para>
        /// </summary>
        public string QR_Read_Product { get; private set; } = "";

        /// <summary>
        /// Control_Character_Visualization '制御文字可視化
        /// </summary>
        private string CCV(int decicode) {
            string str = "";
            string[] code = new string[] {"NUL","SOH","STX","ETX","EOT","ENQ","ACK","BEL","BS","HT",
                                          "LF","VT","FF","CR","SO","SI","DLE","DC1","DC2","DC3",
                                          "DC4","NAK","SYN","ETB","CAN","EM","SUB","ESC","FS","GS",
                                          "RS","US"};
            if (decicode < 32) {
                str = "<" + code[decicode] + ">";
            }
            else if (decicode == 32) {
                str = "<SP>";
            }
            else if (decicode == 127) {
                str = "<DEL>";
            }
            else {
                str = ((char)decicode).ToString();
            }
            return str;
        }

        /// <summary>
        /// QRコード読み込み時処理
        /// </summary>
        private void ReadQR_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            QRcode_label.Text += CCV((int)e.KeyChar);

            //QRコードリーダーの終端文字を<CR>にしておく必要がある（<CR><LF>ではだめ）
            if ((int)e.KeyChar == 13) {
                if (ReadQR_textBox.Text.Length > 10) {
                    QR_Read_Product = ReadQR_textBox.Text;
                    string lotno = QR_Read_Product.Substring(QR_Read_Product.Length - 10);
                    string product = QR_Read_Product.Replace(lotno, "");
                    product = product.Replace(" ", "");
                    LotNo_label.Text = lotno;
                    Product_label.Text = product;

                    ReadQR_textBox.Enabled = false;
                    ReadQR_textBox.BackColor = SystemColors.Control;

                    Button_On(DataCheck_Button);
                    Button_On(Done_Button);
                }
                else {
                    MessageBox.Show("機種名が10文字以下のことはありません", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ReadQR_textBox.Text = "";
                QRcode_label.Text = "";
            }
        }

        /// <summary>
        /// 測定データ保存フォルダ、結果保存先フォルダがあるか（アクセスできるか）調べる
        /// </summary>
        private bool Folder_Access_Check() {
            if (Directory.Exists(Result_Folder) == false) {
                MessageBox.Show("「結果保存先フォルダ」にアクセス出来ません。\nフォルダにアクセス出来るか確認後、再実行して下さい。",
                    "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (Directory.Exists(Measurement_Folder) == false) {
                MessageBox.Show("「測定データフォルダ」にアクセス出来ません。\nフォルダにアクセス出来るか確認後、再実行して下さい。",
                    "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Dage測定データ読み取り
        /// </summary>
        private bool Measurement_DataRead(string filepath) {

            string line;                          // 1行分データ読み込みしたものを一時的に格納する変数
            string[] strArray;                    // 1行分データを分割して一時的に代入するための配列
            List<string> data = new List<string>();//測定結果を格納していくリスト

            int DGV_nowRowCount = 0;

            try {

                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    while (true) {

                        line = sr.ReadLine();

                        //読み取りが無ければループ抜ける
                        if (line == null) { break; }

                        if (line != "") {

                            // ダブルクォート除去
                            line = line.Replace("\"", "");

                            // 1行分データを分割して配列へ格納
                            strArray = line.Split(',');

                            switch (strArray[0]) {

                                case "TESTER":
                                    Serial_label.Text = $"{strArray[1]}_{strArray[2]}";
                                    break;

                                case "DATE":
                                    Date_label.Text = strArray[1];
                                    break;

                                case "TIME":
                                    Time_label.Text = strArray[1];
                                    break;

                                case "TESTGROUP":
                                    TestGroup_label.Text = strArray[1];
                                    break;

                                case "Operator":
                                    Operator_label.Text = strArray[1];
                                    break;

                                case "Bonder":
                                    Bonder_label.Text = strArray[1];
                                    break;

                                case "Chip-Place":
                                    Chip_Place_label.Text = strArray[1];
                                    break;

                                case "TEST":
                                    DataGridView1.Rows[DGV_nowRowCount].Cells[0].Value = strArray[1];
                                    DataGridView1.Rows[DGV_nowRowCount].Cells[1].Value = strArray[2];
                                    DataGridView1.Rows[DGV_nowRowCount].Cells[2].Value = strArray[3];
                                    DGV_nowRowCount += 1;
                                    data.Add(strArray[1]);
                                    break;
                            }
                        }
                    }
                }

                // 平均値をラベルに書き込み
                Average_label.Text = GetAverage(data).ToString("0.00");

                // 最大値をラベルに書き込み
                double max = GetMax(data);
                Max_label.Text = max.ToString("0.00");

                // 最小値をラベルに書き込み
                double min = GetMin(data);
                Min_label.Text = min.ToString("0.00");

                // 標準偏差をラベルに書き込み
                Sigma_label.Text = GetSigma(data).ToString("0.00");

                // レンジ値をラベルに書き込み
                Range_label.Text = (max - min).ToString("0.00");

                return true;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message + "\n\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 傾向値管理線更新
        /// </summary>
        private void TrendVal_Update(double X_ave, double R_ave, double A2, double D3, double D4) {

            // X_UCLラベルに書き込み
            X_UCL_label.Text = (X_ave + (A2 * R_ave)).ToString("0.00");

            // X_CLラベルに書き込み
            X_CL_label.Text = (X_ave).ToString("0.00");

            // X_LCLラベルに書き込み
            X_LCL_label.Text = (X_ave - (A2 * R_ave)).ToString("0.00");


            // R_UCLラベルに書き込み
            R_UCL_label.Text = (D4 * R_ave).ToString("0.00");

            // R_CLラベルに書き込み
            R_CL_label.Text = (R_ave).ToString("0.00");

            // R_LCLラベルに書き込み
            R_LCL_label.Text = (D3 * R_ave).ToString("0.00");
        }

        /// <summary>
        /// X管理図にプロット
        /// </summary>
        private void Chart_X_Add(List<string> x_list) {
            for (int i = 0; i < 4; i++)
                Chart1.Series[i].Points.Clear();// 初期準備としてChart1に系列を4個作成しておく

            //過去の傾向値データプロット
            for (int i = 0; i < x_list.Count; i++) {
                Chart1.Series[0].Points.Add(double.Parse(X_UCL_label.Text));
                Chart1.Series[1].Points.Add(double.Parse(X_CL_label.Text));
                Chart1.Series[2].Points.Add(double.Parse(X_LCL_label.Text));
                Chart1.Series[3].Points.Add(double.Parse(x_list[i]));
            }

            //今回の測定結果に対するプロット＋設定
            Chart1.Series[0].Points.Add(double.Parse(X_UCL_label.Text));
            Chart1.Series[1].Points.Add(double.Parse(X_CL_label.Text));
            Chart1.Series[2].Points.Add(double.Parse(X_LCL_label.Text));
            Chart1.Series[3].Points.Add(double.Parse(Average_label.Text));
            Chart1.Series[3].Points[Chart1.Series[3].Points.Count - 1].MarkerColor = Color.Yellow;
            Chart1.Series[3].Points[Chart1.Series[3].Points.Count - 1].BorderColor = Color.Black;
            Chart1.Series[3].Points[Chart1.Series[3].Points.Count - 1].MarkerStyle = MarkerStyle.Star5;
            Chart1.Series[3].Points[Chart1.Series[3].Points.Count - 1].MarkerSize = 15;
        }

        /// <summary>
        /// R管理図にプロット
        /// </summary>
        private void Chart_R_Add(List<string> r_list) {
            for (int i = 0; i < 4; i++)
                Chart2.Series[i].Points.Clear();// 初期準備としてChart1に系列を4個作成しておく

            //過去の傾向値データプロット
            for (int i = 0; i < r_list.Count; i++) {
                Chart2.Series[0].Points.Add(double.Parse(R_UCL_label.Text));
                Chart2.Series[1].Points.Add(double.Parse(R_CL_label.Text));
                Chart2.Series[2].Points.Add(double.Parse(R_LCL_label.Text));
                Chart2.Series[3].Points.Add(double.Parse(r_list[i]));
            }

            //今回の測定結果に対するプロット＋設定
            Chart2.Series[0].Points.Add(double.Parse(R_UCL_label.Text));
            Chart2.Series[1].Points.Add(double.Parse(R_CL_label.Text));
            Chart2.Series[2].Points.Add(double.Parse(R_LCL_label.Text));
            Chart2.Series[3].Points.Add(double.Parse(Range_label.Text));
            Chart2.Series[3].Points[Chart2.Series[3].Points.Count - 1].MarkerColor = Color.Yellow;
            Chart2.Series[3].Points[Chart2.Series[3].Points.Count - 1].BorderColor = Color.Black;
            Chart2.Series[3].Points[Chart2.Series[3].Points.Count - 1].MarkerStyle = MarkerStyle.Star5;
            Chart2.Series[3].Points[Chart2.Series[3].Points.Count - 1].MarkerSize = 15;
        }

        /// <summary>
        /// DS、BS、PCの下限値のラジオボタンをチェックする
        /// </summary>
        public void Check_Standard() {
            string chip_place = Chip_Place_label.Text;
            Dictionary<string, string> dic = Read_StandardData($"{Product_label.Text}.csv");

            if (chip_place.Contains("DS-")) {

                //データが無い or 読み取りたい項目が登録されていなければ終了
                if (dic == null || dic.ContainsKey(chip_place) == false) {
                    RadioButton_DS100gf.Checked = true;
                    return;
                }

                //DS下限値チェック
                switch (dic[chip_place]) {

                    case "30gf": {
                            RadioButton_DS30gf.Checked = true;
                            break;
                        }
                    case "100gf": {
                            RadioButton_DS100gf.Checked = true;
                            break;
                        }
                    default: {
                            RadioButton_DS100gf.Checked = true;
                            break;
                        }
                }
            }
            else if (chip_place.Contains("BS-")) {

                //データが無い or 読み取りたい項目が登録されていなければ終了
                if (dic == null || dic.ContainsKey(chip_place) == false) {
                    RadioButton_BS50gf.Checked = true;
                    return;
                }

                //BS下限値チェック
                switch (dic[chip_place]) {

                    case "25gf": {
                            RadioButton_BS25gf.Checked = true;
                            break;
                        }
                    case "40gf": {
                            RadioButton_BS40gf.Checked = true;
                            break;
                        }
                    case "50gf": {
                            RadioButton_BS50gf.Checked = true;
                            break;
                        }
                    default: {
                            RadioButton_BS50gf.Checked = true;
                            break;
                        }
                }
            }
            else if (chip_place.Contains("PC-")) {

                //データが無い or 読み取りたい項目が登録されていなければ終了
                if (dic == null || dic.ContainsKey(chip_place) == false) {
                    RadioButton_PC7gf.Checked = true;
                    return;
                }

                //PC下限値チェック
                switch (dic[chip_place]) {

                    case "4gf": {
                            RadioButton_PC4gf.Checked = true;
                            break;
                        }
                    case "7gf": {
                            RadioButton_PC7gf.Checked = true;
                            break;
                        }
                    default: {
                            RadioButton_PC7gf.Checked = true;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// データ確認ボタン処理
        /// </summary>
        private void DataCheck_Button_Click(object sender, EventArgs e) {
            //使用するフォルダがあるか調べる
            if (Folder_Access_Check() == false) { return; }

            //結果保存先フォルダにrawフォルダが無かったら作る
            string raw_folder = Result_Folder + "/raw";
            if (Directory.Exists(raw_folder) == false) {
                Directory.CreateDirectory(raw_folder);
            }

            //"機種名　LotNo"記載のファイル検索して見つける
            string filepath = FileExist(Measurement_Folder, "*" + QR_Read_Product + "*.csv");

            //ファイルが見つからなければ終了
            if (filepath == null) {
                MessageBox.Show("対象の測定データファイルが見つかりません。",
                    "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //グラフ初期化
            Graph_Clear();
            //ラジオボタン初期化
            RadioButton_ini();

            //測定データ読み込みに失敗した場合は終了
            if (Measurement_DataRead(filepath) == false) {
                MessageBox.Show("データ読み込み処理に失敗しましたので終了します。",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //ボタンON/OFF
            Button_Off(DataCheck_Button); // データ確認ボタン
            Button_On(DataChange_Button); // データ修正ボタン
            Button_Off(Done_Button); // 完了ボタンOFF
            Button_Off(Close_Button); // ソフト終了ボタンOFF

            //管理値有無によってボタンON/OFF
            if (Chip_Place_label.Text.Contains("DS-")) {
                //DBデータ保存ボタンON
                Button_On(DieSahreSave_Button);

                //DS下限値グループボックスON
                GroupBox_On(DS_Low_Limit_GBox);

                //DSの下限値をチェックする
                Check_Standard();
            }
            else if (Chip_Place_label.Text.Contains("BS-")) {
                //BS下限値グループボックスON
                GroupBox_On(BS_Low_Limit_GBox);

                //BS、PCの下限値をチェックする
                Check_Standard();
            }
            else if (Chip_Place_label.Text.Contains("PC-")) {
                //PC下限値グループボックスON
                GroupBox_On(PC_Low_Limit_GBox);

                //BS、PCの下限値をチェックする
                Check_Standard();
            }
            else {
                MessageBox.Show("測定箇所がおかしいかもしれません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // 完了ボタンON
                Button_On(Done_Button);
                return;
            }

            //傾向値管理データ確認
            Trend trend = new Trend(Result_Folder + "/Trend", Product_label.Text, Bonder_label.Text);

            if (trend.Check_OldTrend(Chip_Place_label.Text)) {

                double a2 = Get_A2(Chip_Place_label.Text);
                double d3 = Get_D3(Chip_Place_label.Text);
                double d4 = Get_D4(Chip_Place_label.Text);

                // 傾向値管理線など更新
                if (a2 >= 0 && d3 >= 0 && d4 >= 0) {
                    //傾向値データラベル書き込み
                    TrendVal_Update(GetAverage(trend.OldTrend_Xlist), GetAverage(trend.OldTrend_Rlist), a2, d3, d4);
                    //chart1にX管理図書き込み
                    Chart_X_Add(trend.OldTrend_Xlist);
                    //chart2にR管理図書き込み
                    Chart_R_Add(trend.OldTrend_Rlist);
                }
            }

            // 共有フォルダに生データコピー
            string str = "";
            do {
                try {
                    File.Copy(filepath, raw_folder + "/" + Path.GetFileName(filepath) + str);
                    break;
                }
                catch {
                    str += ".csv";
                }
            }
            while (true);

            // 元々の場所にある生データ削除
            File.Delete(filepath);
        }



        /// <summary>
        /// データ修正ボタン処理
        /// </summary>
        private void DataChange_Button_Click(object sender, EventArgs e) {
            DataChangeForm fm = new DataChangeForm(this);
            fm.Left = this.Left + 620;
            fm.Top = this.Top + 120;
            fm.StartPosition = FormStartPosition.Manual;
            fm.ShowDialog(this); // フォームを表示する
            fm.BringToFront(); // 最前面へ移動
        }

        /// <summary>
        /// データグリッドビューのデータクリア
        /// </summary>
        private void DataGridView_ClearData() {
            for (int m = 0; m < DataGridView1.RowCount; m++) {
                for (int n = 0; n < 3; n++)
                    DataGridView1.Rows[m].Cells[n].Value = null;
            }
        }



        /// <summary>
        /// 傾向値管理用書き込みテキスト作成
        /// </summary>
        private string CreateTrendSaveData() {
            string textline = Product_label.Text + ","; //機種名
            textline += LotNo_label.Text + ",";  //ロット番号
            textline += Bonder_label.Text + ","; //設備
            textline += Date_label.Text + " " + Time_label.Text + ","; //日時
            textline += Average_label.Text + ","; //平均
            textline += Range_label.Text + ","; //レンジ
            textline += Get_DGVdata().Count; //データ個数
            textline += "\n";

            return textline;
        }



        /// <summary>
        /// DBデータ保存ボタン処理
        /// </summary>
        private void DieSahreSave_Button_Click(object sender, EventArgs e) {

            // フォルダにアクセス出来るか調べる
            if (Folder_Access_Check() == false) { return; }

            if (MessageBox.Show("DBデータで保存しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                return;
            }

            string textline;
            string folderpath = Result_Folder + "/DB";
            string filename = $"{DateTime.Now.Year}_{DateTime.Now.Month}.csv";

            // csvファイルへデータ書き込み
            for (int i = 0; i < DataGridView1.RowCount; i++) {
                if (DataGridView1.Rows[i].Cells[0].Value != null) {

                    textline = Product_label.Text + ","; //機種名
                    textline += LotNo_label.Text + ",";  //ロット番号
                    textline += Chip_Place_label.Text + ","; //測定箇所
                    textline += DataGridView1.Rows[i].Cells[0].Value.ToString() + ","; //測定結果
                    textline += DataGridView1.Rows[i].Cells[1].Value.ToString() + ","; //破壊モード
                    textline += DataGridView1.Rows[i].Cells[2].Value.ToString() + ","; //コメント
                    textline += Bonder_label.Text + ","; //設備
                    textline += Operator_label.Text + ","; //作業者
                    textline += Date_label.Text + " " + Time_label.Text + ","; //日時
                    textline += Serial_label.Text + ","; //テスト装置
                    textline += TestGroup_label.Text; //テストグループ
                    textline += "\n";

                    DataSave($"{folderpath}/{filename}", textline);
                }
            }

            //傾向値管理データ保存
            TrendDataSave($"{Result_Folder}/Trend/{Chip_Place_label.Text}_trend.csv", CreateTrendSaveData());

            //どのラジオボタンがTrueか調べて、機種ごとの下限値を保存
            foreach (RadioButton r in DS_Low_Limit_GBox.Controls) {
                if (r.Checked) {
                    Save_StandardData($"{Product_label.Text}.csv", Chip_Place_label.Text, r.Text);
                    break;
                }
            }

            //ボタンON/OFとか
            DataGridView_ClearData();
            Label_Text_Clear();
            Button_On(DataCheck_Button);
            Button_On(Done_Button);
            Button_Off(DataChange_Button);
            Button_Off(DieSahreSave_Button);
            GroupBox_Off(DS_Low_Limit_GBox);
        }

        /// <summary>
        /// WBデータ保存ボタン処理
        /// </summary>
        private void WB_Data_Save_Button_Click(object sender, EventArgs e) {

            // フォルダにアクセス出来るか調べる
            if (Folder_Access_Check() == false) { return; }

            if (MessageBox.Show("WBデータで保存しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                return;
            }

            string textline;
            string folderpath = Result_Folder + "/WB";
            string filename = $"{DateTime.Now.Year}_{DateTime.Now.Month}.csv";

            // csvファイルへデータ書き込み
            for (int i = 0; i < DataGridView1.RowCount; i++) {
                if (DataGridView1.Rows[i].Cells[0].Value != null) {
                    textline = Product_label.Text + ","; //機種名
                    textline += LotNo_label.Text + ",";  //ロット番号
                    textline += Chip_Place_label.Text + ","; //測定箇所
                    textline += DataGridView1.Rows[i].Cells[0].Value.ToString() + ","; //測定結果
                    textline += DataGridView1.Rows[i].Cells[1].Value.ToString() + ","; //破壊モード
                    textline += DataGridView1.Rows[i].Cells[2].Value.ToString() + ","; //コメント
                    textline += Bonder_label.Text + ","; //設備
                    textline += Operator_label.Text + ","; //作業者
                    textline += Date_label.Text + " " + Time_label.Text + ","; //日時
                    textline += Serial_label.Text + ","; //テスト装置
                    textline += TestGroup_label.Text; //テストグループ
                    textline += "\n";

                    DataSave($"{folderpath}/{filename}", textline);
                }
            }

            //傾向値管理データ保存
            TrendDataSave($"{Result_Folder}/Trend/{Chip_Place_label.Text}_trend.csv", CreateTrendSaveData());

            //どのラジオボタンがTrueか調べて、機種ごとの下限値を保存
            if (Chip_Place_label.Text.Contains("BS-")) {
                //BS下限値グループ
                foreach (RadioButton r in BS_Low_Limit_GBox.Controls) {
                    if (r.Checked) {
                        Save_StandardData($"{Product_label.Text}.csv", Chip_Place_label.Text, r.Text);
                        break;
                    }
                }
            }
            else if (Chip_Place_label.Text.Contains("PC-")) {
                //PC下限値グループ
                foreach (RadioButton r in PC_Low_Limit_GBox.Controls) {
                    if (r.Checked) {
                        Save_StandardData($"{Product_label.Text}.csv", Chip_Place_label.Text, r.Text);
                        break;
                    }
                }
            }

            //ボタンON/OFとか
            DataGridView_ClearData();
            Label_Text_Clear();
            Button_On(DataCheck_Button);
            Button_On(Done_Button);
            Button_Off(DataChange_Button);
            Button_Off(WB_Data_Save_Button);
            Button_Off(Save_as_Trial_Button);
            GroupBox_Off(BS_Low_Limit_GBox);
            GroupBox_Off(PC_Low_Limit_GBox);
        }

        /// <summary>
        /// 試打ちとして保存ボタン処理
        /// </summary>
        private void Save_as_Trial_Button_Click(object sender, EventArgs e) {

            // フォルダにアクセス出来るか調べる
            if (Folder_Access_Check() == false) { return; }


            if (MessageBox.Show("試打ちとして保存しますか？\n傾向値データには保存されません。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                return;
            }

            string textline;
            string folderpath = Result_Folder + "/WB/Trial";
            string filename = $"{DateTime.Now.Year}_{DateTime.Now.Month}.csv";

            // csvファイルへデータ書き込み
            for (int i = 0; i < DataGridView1.RowCount; i++) {
                if (DataGridView1.Rows[i].Cells[0].Value != null) {
                    textline = Product_label.Text + ","; //機種名
                    textline += LotNo_label.Text + ",";  //ロット番号
                    textline += Chip_Place_label.Text + ","; //測定箇所
                    textline += DataGridView1.Rows[i].Cells[0].Value.ToString() + ","; //測定結果
                    textline += DataGridView1.Rows[i].Cells[1].Value.ToString() + ","; //破壊モード
                    textline += DataGridView1.Rows[i].Cells[2].Value.ToString() + ","; //コメント
                    textline += Bonder_label.Text + ","; //設備
                    textline += Operator_label.Text + ","; //作業者
                    textline += Date_label.Text + " " + Time_label.Text + ","; //日時
                    textline += Serial_label.Text + ","; //テスト装置
                    textline += TestGroup_label.Text; //テストグループ
                    textline += "\n";

                    DataSave($"{folderpath}/{filename}", textline);
                }
            }

            //ボタンON/OFとか
            DataGridView_ClearData();
            Label_Text_Clear();
            Button_On(DataCheck_Button);
            Button_On(Done_Button);
            Button_Off(DataChange_Button);
            Button_Off(WB_Data_Save_Button);
            Button_Off(Save_as_Trial_Button);
            GroupBox_Off(BS_Low_Limit_GBox);
            GroupBox_Off(PC_Low_Limit_GBox);
        }

        /// <summary>
        /// 完了ボタン処理
        /// </summary>
        private void Done_Button_Click(object sender, EventArgs e) {
            QR_Read_Product = "";
            QRcode_label.Text = "";
            Product_label.Text = "-";
            LotNo_label.Text = "-";

            ReadQR_textBox.Enabled = true;
            ReadQR_textBox.BackColor = Color.White;
            Button_ini();
            StatisticsAlert_label.Visible = false;
        }

        /// <summary>
        /// ソフト終了ボタン処理
        /// </summary>
        private void Close_Button_Click(object sender, EventArgs e) {
            Application.Exit();
        }


        /// <summary>
        /// データグリッドビュー内の強度データリストを取得する
        /// </summary>
        public List<string> Get_DGVdata() {
            //再計算用のデータをデータグリッドビューから取得する
            List<string> data_list = new List<string>();
            for (int i = 0; i < DataGridView1.RowCount; i++) {
                if (DataGridView1.Rows[i].Cells[0].Value != null) {
                    data_list.Add(DataGridView1.Rows[i].Cells[0].Value.ToString());
                }
            }
            return data_list;
        }


        private bool Check_NAN() {
            //変化しないとその後のイベントが発動しないのでここで変えておく
            Management_value_label.Text = "";

            if (Average_label.Text == "NaN") {
                StatisticsAlert_label.Visible = true;
                Judge_label.Text = "-"; // OK/NGのラベル
                Button_On(Done_Button);           //完了ボタンON
                Button_Off(WB_Data_Save_Button);  //WBデータ保存ボタンOFF
                Button_Off(Save_as_Trial_Button); //試打ちとして保存ボタンOFF
                Button_Off(DataChange_Button);    //データ修正ボタンOFF
                GroupBox_Off(BS_Low_Limit_GBox);  //BS下限値グループボックスOFF
                GroupBox_Off(PC_Low_Limit_GBox);  //PC下限値グループボックスOFF
                return false;
            }
            else if (Sigma_label.Text == "NaN") {
                //データが1個の時
                if (Get_DGVdata().Count == 1) {
                    Management_value_label.Text = "下限値判定";
                    return false;
                }
                else {
                    StatisticsAlert_label.Visible = true;
                    Judge_label.Text = "-"; // OK/NGのラベル
                    Button_On(Done_Button);          //完了ボタンON
                    Button_Off(WB_Data_Save_Button); //WBデータ保存ボタンOFF
                    Button_Off(Save_as_Trial_Button);//試打ちとして保存ボタンOFF
                    Button_Off(DataChange_Button);   //データ修正ボタンOFF
                    GroupBox_Off(BS_Low_Limit_GBox); //BS下限値グループボックスOFF
                    GroupBox_Off(PC_Low_Limit_GBox); //PC下限値グループボックスOFF
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// プルカット下限値選択変更時処理
        /// </summary>
        public void PC_Low_Limit_CheckedChanged(object sender, EventArgs e) {

            //操作されたラジオボタン取得
            RadioButton rbtn = (RadioButton)sender;

            //ラジオボタンをOFFにするイベントでこのメソッドが呼ばれた場合は何もしない
            if (rbtn.Checked == false) { return; }

            //平均値、標準偏差が計算出来ない時
            if (Check_NAN() == false) { return; }

            double ave = GetAverage(Get_DGVdata());
            double sigma = GetSigma(Get_DGVdata());

            //全データが同じ場合などでσ=0の場合は下限値判定にする
            if (sigma == 0) {
                Management_value_label.Text = "下限値判定";
                return;
            }

            //管理値計算
            if (rbtn.Text == "4gf") {
                Management_value_label.Text = ((ave - 4) / sigma).ToString("0.00");
            }
            else if (rbtn.Text == "7gf") {
                Management_value_label.Text = ((ave - 7) / sigma).ToString("0.00");
            }
        }

        /// <summary>
        /// ボールシェア下限値選択変更時処理
        /// </summary>
        public void BS_Low_Limit_CheckedChanged(object sender, EventArgs e) {

            //操作されたラジオボタン取得
            RadioButton rbtn = (RadioButton)sender;

            //ラジオボタンをOFFにするイベントでこのメソッドが呼ばれた場合は何もしない
            if (rbtn.Checked == false) { return; }

            //平均値、標準偏差が計算出来ない時
            if (Check_NAN() == false) { return; }

            double ave = GetAverage(Get_DGVdata());
            double sigma = GetSigma(Get_DGVdata());

            //全データが同じ場合などでσ=0の場合は下限値判定にする
            if (sigma == 0) {
                Management_value_label.Text = "下限値判定";
                return;
            }

            //管理値計算
            if (rbtn.Text == "25gf") {
                Management_value_label.Text = ((ave - 25) / sigma).ToString("0.00");
            }
            else if (rbtn.Text == "40gf") {
                Management_value_label.Text = ((ave - 40) / sigma).ToString("0.00");
            }
            else if (rbtn.Text == "50gf") {
                Management_value_label.Text = ((ave - 50) / sigma).ToString("0.00");
            }
        }

        /// <summary>
        /// ダイシェア下限値選択変更時処理
        /// </summary>
        public void DS_Low_Limit_CheckedChanged(object sender, EventArgs e) {
            //変化しないとその後のイベントが発動しないのでここで変えておく
            Management_value_label.Text = "";

            //操作されたラジオボタン取得
            RadioButton rbtn = (RadioButton)sender;

            //ラジオボタンをOFFにするイベントでこのメソッドが呼ばれた場合は何もしない
            if (rbtn.Checked == false) { return; }

            //平均値が計算出来ない時
            if (Average_label.Text == "NaN") {
                StatisticsAlert_label.Visible = true;
                Judge_label.Text = "-"; // OK/NGのラベル
                Button_On(Done_Button);           //完了ボタンON
                Button_Off(DieSahreSave_Button);  //DBデータ保存ボタンOFF
                Button_Off(DataChange_Button);    //データ修正ボタンOFF
                GroupBox_Off(DS_Low_Limit_GBox);  //DS下限値グループボックスOFF
                return;
            }

            Management_value_label.Text = "下限値判定";
        }




        /// <summary>
        /// 下限値判定する
        /// </summary>
        private void Low_limit_judge() {
            string chip_place = Chip_Place_label.Text;
            double min = double.Parse(Min_label.Text);
            bool okng = false;

            //DBデータ下限値判定
            if (chip_place.Contains("DS-")) {

                if (RadioButton_DS30gf.Checked == true) {
                    if (min >= 30) {
                        okng = true;
                    }
                }
                else if (RadioButton_DS100gf.Checked == true) {
                    if (min >= 100) {
                        okng = true;
                    }
                }

                //データが下限値以上だったら
                if (okng) {
                    Judge_label.Text = "OK";
                }
                else {
                    Judge_label.Text = "NG";
                }

                return;
            }

            //WBデータ下限値判定
            if (chip_place.Contains("BS-")) {

                if (RadioButton_BS25gf.Checked == true) {
                    if (min >= 25) {
                        okng = true;
                    }
                }
                else if (RadioButton_BS40gf.Checked == true) {
                    if (min >= 40) {
                        okng = true;
                    }
                }
                else if (RadioButton_BS50gf.Checked == true) {
                    if (min >= 50) {
                        okng = true;
                    }
                }
            }
            else if (chip_place.Contains("PC-")) {

                if (RadioButton_PC4gf.Checked == true) {
                    if (min >= 4) {
                        okng = true;
                    }
                }
                else if (RadioButton_PC7gf.Checked == true) {
                    if (min >= 7) {
                        okng = true;
                    }
                }
            }

            //データが下限値以上だったら
            if (okng) {
                Judge_label.Text = "OK";
                Button_On(WB_Data_Save_Button); // WBデータ保存ボタンON
                Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
            }
            else {
                Judge_label.Text = "NG";
                Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
                Button_Off(WB_Data_Save_Button); // WBデータ保存ボタンOFF
            }

            return;
        }




        /// <summary>
        /// 管理値からOK/NG判定する
        /// </summary>
        private void Management_value_label_TextChanged(object sender, EventArgs e) {
            //一旦初期化する処理で発生したイベントでは何もしない
            if (Management_value_label.Text == "") { return; }

            //管理値ではなく、下限値で判定する場合
            if (Management_value_label.Text == "下限値判定") {
                Low_limit_judge();
                return;
            }

            string chip_place = Chip_Place_label.Text;
            //bool judge = false;
            //管理値判定をするかどうか
            if (chip_place.Contains("BS-LED-")) {
                //judge = true;
            }
            else if (chip_place.Contains("BS-Lead-")) {
                //judge = true;
            }
            else if (chip_place.Contains("PC-LED-")) {
                //judge = true;
            }
            else {
                //管理値判定しない場合は下限値判定
                Management_value_label.Text = "下限値判定";
                return;
            }

            //管理値取得
            double value;
            try {
                value = double.Parse(Management_value_label.Text);
            }
            catch {
                value = -99999;
            }

            // 管理値から判定する
            //if (judge) {

                if (value >= 4) {
                    Judge_label.Text = "OK";
                    Button_On(WB_Data_Save_Button); // WBデータ保存ボタンON
                    Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
                }
                else if (value < 4 && value > -1000) {
                    Judge_label.Text = "NG";
                    Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
                    Button_Off(WB_Data_Save_Button); // WBデータ保存ボタンOFF
                }
                else {//どんな場合か分からない
                    Judge_label.Text = "????";
                    Button_On(WB_Data_Save_Button); // WBデータ保存ボタンON
                    Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
                }
            //}
            //else {
            //    Judge_label.Text = "判定無し";
            //    Button_On(WB_Data_Save_Button); // WBデータ保存ボタンON
            //    Button_On(Save_as_Trial_Button); // 試打ちとして保存ボタンON
            //}
        }

        private void Judge_label_TextChanged(object sender, EventArgs e) {
            //操作されたラベル取得
            Label label = (Label)sender;

            if (label.Text == "OK") {
                Judge_label.BackColor = Color.Yellow;
            }
            else if (label.Text == "NG") {
                Judge_label.BackColor = Color.Red;
            }
            else {
                Judge_label.BackColor = SystemColors.Control;
            }
        }
    }
}
