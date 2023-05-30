using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualBasic;

using static V_Data_Collection.Program;

namespace V_Data_Collection {
    public partial class MainForm : Form {

        /// <summary>
        /// 投入枚数
        /// </summary>
        int input_num;

        /// <summary>
        /// 回収枚数
        /// </summary>
        int output_num;

        /// <summary>
        /// 上刃ブレードカット距離
        /// </summary>
        double upper_brade_cut;

        /// <summary>
        /// 下刃ブレードカット距離
        /// </summary>
        double lower_brade_cut;

        /// <summary>
        /// 切削位置オフセット
        /// </summary>
        double cutpos_offset;

        /// <summary>
        /// 残厚補正(上刃)
        /// </summary>
        double depth_upper_offset;

        /// <summary>
        /// 残厚補正(下刃)
        /// </summary>
        double depth_lower_offset;

        /// <summary>
        /// 残厚センター補正
        /// </summary>
        double depth_center_offset;

        /// <summary>
        /// V溝加工ステーション(残厚測定用)
        /// <para>例：zan_st="ST1"</para>
        /// </summary>
        private string zan_st;

        /// <summary>
        /// 残厚測定箇所
        /// <para>例：zan_posi="残厚左上"</para>
        /// </summary>
        private string zan_posi;

        /// <summary>
        /// 外観状態確認結果
        /// <para>"OK" or "NG"</para>
        /// </summary>
        private string gaikan;

        /// <summary>
        /// 測定項目名一覧
        /// <para>例：[ロット番号,シリアルカウンタ,判定,・・・]</para>
        /// </summary>
        private List<string> header_name;

        /// <summary>
        /// 測定項目が何行目か分かるようにするために使用する
        /// <para>"残厚左上":22(行目)</para>
        /// </summary>
        private Dictionary<string, int> header_index_dic;

        /// <summary>
        /// 記録開始したか
        /// <para>true = 記録開始した</para>
        /// </summary>
        private bool rec_start = false;

        /// <summary>
        /// 量産か点検か
        /// <para>量産の場合はtrue</para>
        /// </summary>
        private bool mp_or_check = true;

        /// <summary>
        /// 各STデータがどのファイルのものか記憶しておく
        /// </summary>
        private Dictionary<int, string> use_filename_dic = new Dictionary<int, string>() { { 0, "" }, { 1, "" }, { 2, "" } };


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm() {
            InitializeComponent();
        }

        /// <summary>
        /// PCの使用可能なCOMポートを取得してコンボボックスに追加する
        /// </summary>
        private void update_comport() {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0) {
                foreach (string port in ports) {
                    comport_comboBox.Items.Add(port);
                }
                comport_comboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// LM1000からの受信したcsvの読み込む列名を取得＋残厚項目も入れてheader listを作る
        /// <para>さらに項目名と行番号の辞書作る</para>
        /// </summary>
        private bool Set_HeaderName() {

            //行ヘッダーリスト作成
            header_name = new List<string>();
            string path = $@"{AppFolder}/LM-1000_csv_columnName.txt";

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {

                while (true) {

                    string line = sr.ReadLine();

                    if (string.IsNullOrEmpty(line)) { break; }

                    header_name.Add(line);
                }
            }

            //残厚はLM-1000からのデータでは無いので、ここで追加している
            header_name.Add("残厚右上");
            header_name.Add("残厚右下");
            header_name.Add("残厚左上");
            header_name.Add("残厚左下");

            //メジャリング再測定したかも追加
            header_name.Add("手動再測定確認");

            try {
                //行ヘッダー名と行番号の辞書作成
                header_index_dic = new Dictionary<string, int>();
                for (int i = 0; i < header_name.Count; i++) {
                    header_index_dic.Add(header_name[i], i);
                }
                return true;
            }
            catch (Exception ex) {
                MessageBox.Show($"測定項目の設定に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// MainFormロードイベント 
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e) {

            //COMポートセッティング
            update_comport();

            //生データ移動先フォルダが無ければ作成する
            if (Directory.Exists(RawFolder) == false) {
                Directory.CreateDirectory(RawFolder);
            }

            csv_Fld_label.Text = $"csvデータ受信フォルダ\n{DataFolder}";
            save_fld_label.Text = $"データ保存先フォルダ\n{SaveFolder}";
            st1_radioButton.Checked = true;
            UR_radioButton.Checked = true;
            empty_check_timer.Enabled = true;
        }

        /// <summary>
        /// LM-1000からのcsvが保存されるフォルダにcsvファイルが既に存在しているか確認
        /// </summary>
        private void empty_check_timer_Tick(object sender, EventArgs e) {
            //記録開始中は何もしない
            if (rec_start) { return; }

            string[] filepaths;
            try {
                filepaths = Directory.GetFiles(DataFolder, "*.csv");
            }
            catch (Exception ex) {
                empty_check_timer.Enabled = false;
                MessageBox.Show("csvデータ受信フォルダが見つかりません。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                csv_fld_select_Click(null, null);
                filepaths = Directory.GetFiles(DataFolder, "*.csv");
                empty_check_timer.Enabled = true;
            }

            if (filepaths.Length == 0) {
                rec_start_button.Enabled = true;
                check_start_button.Enabled = true;
                not_empty_label.Visible = false;
            }
            else {
                rec_start_button.Enabled = false;
                check_start_button.Enabled = false;
                not_empty_label.Visible = true;
            }

        }


        /// <summary>
        /// 記録開始ボタン処理
        /// <para>点検開始ボタンにも同じ処理を使用する</para>
        /// </summary>
        private void rec_start_button_Click(object sender, EventArgs e) {

            //測定項目セット
            if (Set_HeaderName() == false) { return; }

            //データグリッドビューの行を作成し、行ヘッダー書き込む
            if (dataGridView1.Rows.Count == 0) {
                header_name.ForEach(name =>
                 {
                     dataGridView1.Rows.Add();
                     var new_row = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
                     new_row.HeaderCell.Value = name;
                 });
            }

            rec_start = true;
            timer1.Enabled = true;
            csvfld_select_ToolStripMenuItem.Enabled = false;
            rec_start_button.Enabled = false;
            check_start_button.Enabled = false;
            now_rec_label.Visible = true;

            var bt = (Button)sender;
            if (bt.Text == "点検開始") {
                mp_or_check = false;
            }
            else {
                mp_or_check = true;
            }
        }

        /// <summary>
        /// 記録中止ボタン処理
        /// <para>保存完了処理にも使う</para>
        /// </summary>
        private void rec_cancel_button_Click(object sender, EventArgs e) {

            if (rec_start == false) { return; }

            DialogResult result;
            if (sender == null) {
                result = DialogResult.Yes;
            }
            else {
                result = MessageBox.Show("記録中止しますか？\nデータは消えてしまいます。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes) {
                //データグリッドビューを消す前に行っておく
                ST1_remeasurement_checkBox.Checked = false;
                ST2_remeasurement_checkBox.Checked = false;
                ST3_remeasurement_checkBox.Checked = false;
                ST1_remeasurement_checkBox.BackColor = Color.Empty;
                ST2_remeasurement_checkBox.BackColor = Color.Empty;
                ST3_remeasurement_checkBox.BackColor = Color.Empty;
                //データグリッドビュー消去
                dataGridView1.Rows.Clear();
                rec_start = false;
                timer1.Enabled = false;
                csvfld_select_ToolStripMenuItem.Enabled = true;
                rec_start_button.Enabled = true;
                check_start_button.Enabled = true;
                now_rec_label.Visible = false;
            }
        }

        /// <summary>
        /// COMポート選択時処理
        /// </summary>
        private void comport_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox cb = (ComboBox)sender;
            serialPort1.PortName = cb.SelectedItem.ToString();
        }

        /// <summary>
        /// 接続ボタン処理
        /// </summary>
        private void connect_button_Click(object sender, EventArgs e) {
            Button btn = (Button)sender;
            if (btn.Text == "接続") {
                try {
                    serialPort1.Open();
                    btn.Text = "切断";
                    connect_label.Text = "接続中";
                    connect_label.BackColor = Color.LightGreen;
                    comport_comboBox.Enabled = false;
                }
                catch (Exception ex) {
                    MessageBox.Show("接続できませんでした。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                serialPort1.Close();
                btn.Text = "接続";
                connect_label.Text = "切断中";
                connect_label.BackColor = Color.Orange;
                comport_comboBox.Enabled = true;
            }
        }

        /// <summary>
        /// ST選択ラジオボタン選択時処理(残厚測定用)
        /// </summary>
        private void st_radioButton_CheckedChanged(object sender, EventArgs e) {
            var rb = (RadioButton)sender;
            if (rb.Checked) {
                zan_st = rb.Text;
            }
        }

        /// <summary>
        /// 残厚測定箇所選択ラジオボタン選択時処理
        /// </summary>
        private void zan_posi_radioButton_CheckedChanged(object sender, EventArgs e) {
            var rb = (RadioButton)sender;
            if (rb.Checked) {
                zan_posi = "残厚" + rb.Text;
            }
        }

        /// <summary>
        /// 指定箇所の残厚データ削除ボタン
        /// </summary>
        private void zan_delete_button_Click(object sender, EventArgs e) {
            dataGridView1[zan_st, header_index_dic[zan_posi]].Value = null;
        }

        /// <summary>
        /// データグリッドビューに書く処理
        /// </summary>
        /// <param name="text"></param>
        private void write_data(string text) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => write_data(text)), null);
                return;
            }
            else {
                dataGridView1[zan_st, header_index_dic[zan_posi]].Value = text;

                //残厚測定値が見えるように表示位置調整
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;

                //マザーボードのスピーカーから音を出す
                Console.Beep();
            }
        }

        /// <summary>
        /// シリアル通信データ受信時処理
        /// </summary>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {

            serialPort1.NewLine = "\r";

            string r_msg = serialPort1.ReadLine();

            if (r_msg.Substring(0, 2) == "91") {
                MessageBox.Show($"測定器からエラー信号が出力されました。\n\nError Code:{r_msg.Substring(2)}");
                return;
            }

            if (r_msg.Substring(0, 3) != "01A") {
                MessageBox.Show($"データフォーマットにない出力を受信しました。");
                serialPort1.DiscardInBuffer();
                return;
            }

            //記録開始していないなら終了
            if (rec_start == false) { return; }

            double data = double.Parse(r_msg.Substring(3));

            write_data(data.ToString());
        }

        /// <summary>
        /// タイマー1処理・・・csv受信フォルダ内のファイルチェックする
        /// </summary>
        private void timer1_tick_method(object sender, EventArgs e) {
            string[] filepaths;

            try {
                filepaths = Directory.GetFiles(DataFolder, "*.csv");
            }
            catch (Exception ex) {
                timer1.Enabled = false;
                MessageBox.Show("csvデータ受信フォルダが見つかりません。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                csv_fld_select_Click(null, null);
                filepaths = Directory.GetFiles(DataFolder, "*.csv");
                timer1.Enabled = true;
            }

            if (filepaths.Length == 0) {
                return;
            }

            filepaths.ToList().ForEach(f =>
            {
                //csvファイルのデータ読み取り
                read_csv(f);

                ////csvファイル移動
                int cnt = 0;
                while (true) {
                    try {
                        string destFileName = $@"{RawFolder}\{Path.GetFileName(f)}";

                        if (File.Exists(destFileName)) {
                            destFileName = $@"{RawFolder}\copy_{Path.GetFileName(f)}";
                        }

                        File.Move(f, destFileName);
                        break;
                    }
                    catch (IOException) {
                        Thread.Sleep(500);//早すぎると失敗する時があるので
                        cnt += 1;
                        if (cnt > 10) { break; }
                    }
                }

            });

            //点検の場合は自動ですぐに保存
            if (mp_or_check == false) {
                save_button_Click(null, null);
            }
        }

        /// <summary>
        /// LM-1000から受信したcsvファイルを読み込む処理
        /// </summary>
        /// <param name="path"></param>
        private void read_csv(string path) {

            string[] csv_colname_array;
            string[] data;
            string stNo = "";

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("shift_jis"))) {


                    //1行目読み込む
                    string line = sr.ReadLine();

                    //line="" or null なら終了
                    if (string.IsNullOrEmpty(line)) {
                        return;
                    }

                    //列名配列取得
                    csv_colname_array = line.Split(',');


                    for (int i = 0; i < 3; i++) {
                        //2,3,4行目読み込む(使わない)
                        line = sr.ReadLine();
                        //line="" or null なら終了
                        if (string.IsNullOrEmpty(line)) {
                            return;
                        }
                    }

                    //5行目読み込む
                    line = sr.ReadLine();

                    //line="" or null なら終了
                    if (string.IsNullOrEmpty(line)) {
                        //break;
                        return;
                    }

                    data = line.Split(',');
                }


                //data[3]はシリアルカウンタ = ST番号と考える
                stNo = data[3];
                //stNo = int.Parse(data[3]).ToString();
                //string stNo = data[3].Replace("0", "");
                string col_name = "ST" + int.Parse(data[3]).ToString();

                //データグリッドビューに書く
                for (int j = 0; j < data.Length; j++) {
                    if (header_index_dic.ContainsKey(csv_colname_array[j])) {
                        dataGridView1[col_name, header_index_dic[csv_colname_array[j]]].Value = data[j];
                    }
                }

                if (col_name == "ST1") {
                    use_filename_dic[0] = Path.GetFileName(path);
                    if (data[4] == "OK") {
                        ST1_remeasurement_checkBox.BackColor = Color.Empty;
                    }
                    else {
                        ST1_remeasurement_checkBox.BackColor = Color.Yellow;
                    }
                }
                else if (col_name == "ST2") {
                    use_filename_dic[1] = Path.GetFileName(path);
                    if (data[4] == "OK") {
                        ST2_remeasurement_checkBox.BackColor = Color.Empty;
                    }
                    else {
                        ST2_remeasurement_checkBox.BackColor = Color.Yellow;
                    }
                }
                else if (col_name == "ST3") {
                    use_filename_dic[2] = Path.GetFileName(path);
                    if (data[4] == "OK") {
                        ST3_remeasurement_checkBox.BackColor = Color.Empty;
                    }
                    else {
                        ST3_remeasurement_checkBox.BackColor = Color.Yellow;
                    }
                }

            }
            catch (Exception ex) {
                timer1.Enabled = false;
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                using (StreamWriter w = new StreamWriter($"{AppFolder}\\errorlog.txt", true, Encoding.UTF8)) {
                    w.WriteLine($"{dt}\t{Path.GetFileName(path)}\t\t{ex.Message.Replace("\r\n", "")}");
                }
                string msg = $"csvファイルの読み込みに失敗しました。\n" +
                    $"errorlog.txtに読み込み失敗したファイル名を書き込みました。\n\n" +
                    $"シリアルカウンタの値が正しいか確認して下さい。\n" +
                    $"半角数字の1 , 2 , 3のみOKです。\n4 , 2.00などが入力されている場合はデータ取得出来ません。\n\n" +
                    $"入力されていたシリアルカウンタ値：{stNo}\n\n" +
                    $"{ex.Message}";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                timer1.Enabled = true;
            }
        }


        /// <summary>
        /// csv受信フォルダ選択処理
        /// </summary>
        private void csv_fld_select_Click(object sender, EventArgs e) {

            folderBrowserDialog1.Description = "LM-1000からのcsvファイルを受信するフォルダを選択してください。";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                string fld = folderBrowserDialog1.SelectedPath;
                //Vドライブは"V:"ではアクセスできないので書き換える
                if (fld.Substring(0, 2) == "V:") {
                    fld = fld.Replace("V:", "\\\\svfile2\\fileserver");
                }
                DataFolder = fld;


                //選択したフォルダパスをconfigファイルに保存
                try {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["datafolder"].Value = DataFolder;
                    config.Save();

                    csv_Fld_label.Text = $"csvデータ受信フォルダ\n{DataFolder}";
                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをコンフィグファイルに保存するのを失敗しました。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// データ保存先フォルダ選択処理
        /// </summary>
        private void save_fld_select_Click(object sender, EventArgs e) {
            folderBrowserDialog1.Description = "データを保存するフォルダを選択してください。";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                string fld = folderBrowserDialog1.SelectedPath;
                //Vドライブは"V:"ではアクセスできないので書き換える
                if (fld.Substring(0, 2) == "V:") {
                    fld = fld.Replace("V:", "\\\\svfile2\\fileserver");
                }
                SaveFolder = fld;


                //選択したフォルダパスをconfigファイルに保存
                try {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["savefolder"].Value = SaveFolder;
                    config.Save();

                    save_fld_label.Text = $"データ保存先フォルダ\n{SaveFolder}";
                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをコンフィグファイルに保存するのを失敗しました。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// 外観状態確認ラジオボタン選択時処理
        /// </summary>
        private void gaikan_radioButton_CheckedChanged(object sender, EventArgs e) {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked) {
                gaikan = rb.Text;
                save_button.Enabled = true;
            }
        }

        /// <summary>
        /// メジャリング再測定チェックボックスチェック時処理
        /// </summary>
        private void remeasurement_checkBox_CheckedChanged(object sender, EventArgs e) {
            CheckBox cb = (CheckBox)sender;
            string col_name = cb.Tag.ToString();

            if (cb.Checked) {
                dataGridView1[col_name, header_index_dic["手動再測定確認"]].Value = "OK";
            }
            else {
                dataGridView1[col_name, header_index_dic["手動再測定確認"]].Value = "";
            }

            //残厚測定値が見えるように表示位置調整
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;

        }

        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        private void save_button_Click(object sender, EventArgs e) {

            DialogResult result;

            if (mp_or_check) {

                //画像測定がNGの場合は再測定をしたか確認する
                var required = remeasurement_check_panel.Controls.GetEnumerator();
                foreach (var cont in remeasurement_check_panel.Controls) {

                    var cb = (CheckBox)cont;
                    string col_name = cb.Tag.ToString();

                    if (cb.BackColor == Color.Yellow && cb.Checked == false) {
                        var ques = MessageBox.Show($"{col_name}の画像検査NG項目について手動再測定しましたか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ques == DialogResult.No) {
                            return;
                        }
                        ques = MessageBox.Show("再測定がNGのデータを保存しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ques == DialogResult.No) {
                            return;
                        }
                        else {
                            dataGridView1[col_name, header_index_dic["手動再測定確認"]].Value = "NG";
                            //残厚測定値が見えるように表示位置調整
                            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                        }

                    }
                }

                input_num = (int)input_numericUpDown.Value;
                output_num = (int)output_numericUpDown.Value;
                upper_brade_cut = (double)upper_bradecut_numericUpDown.Value;
                lower_brade_cut = (double)lower_bradecut_numericUpDown.Value;

                cutpos_offset = (double)cutpos_offset_numericUpDown.Value;
                depth_lower_offset = (double)depth_lower_offset_numericUpDown.Value;
                depth_upper_offset = (double)depth_upper_offset_numericUpDown.Value;
                depth_center_offset = (double)depth_center_offset_numericUpDown.Value;

                string msg = $"投入枚数：{input_num.ToString()}\n";
                msg += $"完了枚数：{output_num.ToString()}\n";
                msg += $"外観状態確認結果：{gaikan}\n";
                msg += $"上刃ブレードカット距離：{upper_brade_cut.ToString()}m\n";
                msg += $"下刃ブレードカット距離：{lower_brade_cut.ToString()}m\n\n";
                msg += $"切削位置オフセット：{cutpos_offset.ToString()}\n";
                msg += $"残厚補正(上刃)：{depth_lower_offset.ToString()}\n";
                msg += $"残厚補正(下刃)：{depth_upper_offset.ToString()}\n";
                msg += $"残厚センター補正：{depth_center_offset.ToString()}\n\n";

                msg += "この内容で保存しますか？";
                result = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else {
                //点検の場合
                result = DialogResult.Yes;
            }

            if (result == DialogResult.Yes) {

                if (save_data()) {
                    //外観状態確認結果ラジオボタンをfalseにする
                    save_panel.Controls.OfType<RadioButton>().ToList().ForEach(rb => { rb.Checked = false; });
                    save_button.Enabled = false;
                    rec_cancel_button_Click(null, null);
                    //一応初期化
                    use_filename_dic = new Dictionary<int, string>() { { 0, "" }, { 1, "" }, { 2, "" } };

                    if (mp_or_check) {
                        MessageBox.Show("量産データを保存しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else {
                        MessageBox.Show("点検結果を保存しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        /// <summary>
        /// ST1-ST3のどの列にデータが存在しているのか調べる
        /// <para>データが無い列はデータ保存しなくていい為</para>
        /// <list type="table">
        /// <item>0:ST1</item>
        /// <item>1:ST2</item>
        /// <item>2:ST3</item>
        /// </list>
        /// </summary>
        private List<int> check_use_col() {

            var use_col_list = new List<int>();

            for (int j = 0; j < 3; j++) {
                //残厚測定4個分+手動再測定確認を除いた数だけループする
                //⇒LM-1000のデータが無い列は保存しない（何のデータか分からないため）
                for (int i = 0; i < dataGridView1.Rows.Count - 5; i++) {
                    if (dataGridView1.Rows[i].Cells[j].Value != null) {
                        use_col_list.Add(j);
                        break;
                    }
                }
            }
            return use_col_list;
        }

        /// <summary>
        /// csvにデータを保存する
        /// </summary>
        private bool save_data() {

            if (Directory.Exists(SaveFolder) == false) {
                MessageBox.Show("データ保存フォルダが見つかりません。\nデータ保存フォルダにアクセス出来るか確認して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string path = $@"{SaveFolder}\{DateTime.Now.ToString("yyyy_M")}.csv";
            if (mp_or_check == false) {
                path = $@"{SaveFolder}\{DateTime.Now.ToString("yyyy_M")}_check.csv";
            }

            //データ保存ファイルが無ければ作成する
            if (File.Exists(path) == false) {
                try {

                    string header = "";
                    header_name.ForEach(name =>
                    {
                        header += name + ",";
                    });

                    header += "投入枚数,回収枚数,外観状態確認結果,上刃ブレードカット距離,下刃ブレードカット距離,切削位置オフセット,残厚補正(上刃),残厚補正(下刃),残厚センター補正,元データファイル名";

                    header += "\n";

                    //FileMode.Appendはファイルが無かったら新規作成する
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                        sw.Write(header);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"データ保存ファイル作成に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }


            List<int> use_col = check_use_col();
            //書き込むデータが無い場合
            if (use_col.Count == 0) {
                MessageBox.Show($"保存するデータがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }


            //書き込むテキスト作成
            string text = "";
            use_col.ForEach(j =>
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++) {

                    if (dataGridView1.Rows[i].Cells[j].Value == null) {
                        text += "";
                    }
                    else {
                        text += dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }

                    text += ",";
                }

                if (mp_or_check) {
                    text += $"{input_num.ToString()},{output_num.ToString()},{gaikan},{upper_brade_cut.ToString()},{lower_brade_cut.ToString()},{cutpos_offset.ToString()},{depth_upper_offset.ToString()},{depth_lower_offset.ToString()},{depth_center_offset.ToString()},{use_filename_dic[j]}";
                }
                else {
                    //点検の場合は空欄にしておく
                    text += $",,,,,,,,,{use_filename_dic[j]}";
                }

                text += "\n";

            });

            try {
                //FileMode.Appendはファイルが無かったら新規作成する
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(text);
                }
                return true;
            }
            catch (Exception ex) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                using (StreamWriter w = new StreamWriter($"{AppFolder}\\errorlog.txt", true, Encoding.UTF8)) {
                    w.WriteLine($"{dt}\t{ex.Message}");
                }
                MessageBox.Show($"データ保存に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        /// <summary>
        /// ソフト終了処理
        /// </summary>
        private void FormClose_Click(object sender, EventArgs e) {

            if (rec_start) {
                DialogResult r = MessageBox.Show("記録中です。\n測定データがある場合は消えてしまいます。\n\n本当にソフト終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.No) {
                    return;
                }
            }

            if (serialPort1.IsOpen) {
                serialPort1.Close();
            }

            this.Close();
        }

    }
}
