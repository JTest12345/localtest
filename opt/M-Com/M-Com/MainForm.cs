using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;
using System.IO.Ports;
using System.IO;
using System.Configuration;

using static M_Com.Program;
using static M_Com.ArmsSystem;

using MathNet.Numerics.Statistics;
/*
 * 統計値求めるのに
 * NuGetで"MathNet.Numerics"をインストールして使う
 */

namespace M_Com {

    public partial class MainForm : Form {

        /// <summary>
        /// 使用する工程用のパネル
        /// </summary>
        Panel process_panel;

        /// <summary>
        /// 測定素子（RGB用）
        /// </summary>
        public string measure_chip;

        /// <summary>
        /// 測定箇所
        /// </summary>
        public string measure_place;

        /// <summary>
        /// 測定項目
        /// </summary>
        public string measure_item;

        /// <summary>
        /// シリアル通信でコマンド送信後にfalseにして、受信完了したらtrueにする
        /// </summary>
        public bool serial_rec_end = true;

        /// <summary>
        /// データ保存したかどうか
        /// <para>true = データ保存した</para>
        /// </summary>
        public bool save_done = true;

        /// <summary>
        /// 測定箇所_測定項目ごとのデータを持っている
        /// <para>例：{Blue_Anode_BS : SampleData , Red_Anode_PC : SampleData}</para>
        /// </summary>
        public Dictionary<string, SampleData> item_stat_dic;

        /// <summary>
        /// 機種の上下限値規格
        /// <para>例⇒{Anode_ボール径 X:StandardValue,Anode_ボール径 Y:StandardValue,・・・}</para>
        /// </summary>
        public Dictionary<string, Standard.StandardValue> standard_dic;

        /// <summary>
        /// 測定数をチェックするか
        /// </summary>
        public bool measure_num_check = true;

        //コンストラクタ
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
        /// MainFormロードイベント 
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e) {

            //COMポートセッティング
            update_comport();

            //測定モードをWBにする
            WB_mode_radioButton.Checked = true;

            SaveFolder_label.Text = SaveFolder;
            product_label.Text = "";
            lotno_label.Text = "";
            operator_label.Text = "";
            machine_label.Text = "";
            item_cnt_textBox.Text = "";
            standard_label.Text = "";
            MeasureSerial_label.Text += $"\n{MeasureSerial}";
            if (armsSystem) {
                lotQR_label.Text = "マガジンQRコード";
                ReadQR_textBox.BackColor = Color.SandyBrown;
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
                    FormClose_button.Enabled = false;
                    comport_comboBox.Enabled = false;
                }
                catch {
                    MessageBox.Show("接続できませんでした。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                serialPort1.Close();
                btn.Text = "接続";
                connect_label.Text = "切断中";
                connect_label.BackColor = Color.Orange;
                FormClose_button.Enabled = true;
                comport_comboBox.Enabled = true;
            }
            check_input_info();
        }

        /// <summary>
        /// 測定モード選択時処理
        /// </summary>
        private void mode_radioButton_CheckedChanged(object sender, EventArgs e) {
            RadioButton rb = (RadioButton)sender;

            //checkが変化したら反応してしまうので、trueの時のみ処理
            if (rb.Checked == false) { return; }

            Process = rb.Text; ;
            panel_set();
        }

        /// <summary>
        /// 使用する測定パネルをセットする
        /// </summary>
        private void panel_set() {

            //1回全部のラジオボタンをoffにしてからonにしないとcheckedイベントが発生しないことがあるため
            foreach (RadioButton rb in WB_place_groupBox.Controls) {
                rb.Checked = false;
            }
            foreach (RadioButton rb in WB_chip_groupBox.Controls) {
                rb.Checked = false;
            }
            foreach (RadioButton rb in SDR_place_groupBox.Controls) {
                rb.Checked = false;
            }

            if (Process == "WB") {
                process_panel = WB_measure_panel;
                process_panel.Visible = true;
                process_panel.Location = new Point(5, 165);
                measure_place = null;
                blue_chip_radioButton.Checked = true;
                anode_radioButton.Checked = true;
                dataGridView1.Columns["data"].HeaderText = "測定値[μm]";
            }
            else if (Process == "SDR") {
                process_panel = SDR_measure_panel;
                process_panel.Visible = true;
                process_panel.Location = new Point(5, 165);
                plus_radioButton.Checked = true;
                dataGridView1.Columns["data"].HeaderText = "測定値[mm]";
            }

            //使用しないパネルは非表示にする
            List<Panel> panel_list = new List<Panel>() { WB_measure_panel, SDR_measure_panel };
            panel_list.ForEach(p =>
            {
                if (p != process_panel) {
                    p.Visible = false;
                }
            });
        }

        /// <summary>
        /// 測定素子ラジオボタンをチェックした時の処理
        /// </summary>
        private void chip_radioButton_CheckedChanged(object sender, EventArgs e) {
            RadioButton rb = (RadioButton)sender;
            //checkが変化したら反応してしまうので、trueの時のみ処理
            if (rb.Checked == false) { return; }

            measure_chip = rb.Text;

            if (measure_place == null) { return; }

            if (measure_place.Substring(0, 2) != "ZD" || measure_place.Substring(0, 4) != "Lead") {

                measure_place = $"{measure_chip}-{measure_place.Split('-')[1]}";
            }

        }


        /// <summary>
        /// 測定箇所ラジオボタンをチェックした時の処理
        /// </summary>
        private void place_radioButton_CheckedChanged(object sender, EventArgs e) {
            RadioButton rb = (RadioButton)sender;
            //checkが変化したら反応してしまうので、trueの時のみ処理
            if (rb.Checked == false) { return; }

            string tag = null;
            if (rb.Tag != null) { tag = rb.Tag.ToString(); }

            if (Process == "WB") {
                //ZD or Leadかどうかで処理分ける
                if (tag == "NotRGB") {
                    WB_chip_groupBox.Enabled = false;
                    measure_place = rb.Text;
                }
                else {
                    WB_chip_groupBox.Enabled = true;
                    measure_place = $"{measure_chip}-{rb.Text}";
                }
            }
            else if (Process == "SDR") {
                measure_place = rb.Text;
            }

        }

        /// <summary>
        /// 基準値を表示する
        /// </summary>
        private void view_satndard() {
            if (standard_dic == null) { return; }

            string key = $"{measure_place}_{measure_item}";

            if (standard_dic.ContainsKey(key)) {
                string lower = standard_dic[key].Lower.ToString();
                string upper = standard_dic[key].Upper.ToString();
                standard_label.Text = $"{key}　下限値：{lower}　上限値：{upper}";
            }
            else {
                standard_label.Text = "";
            }
        }

        /// <summary>
        /// 測定項目のボタン押した時の処理
        /// </summary>
        private void GetData(object sender, EventArgs e) {

            //既に実行しているシリアル通信で処理完了していない場合は終了
            if (serial_rec_end == false) { return; }

            serialPort1.DiscardInBuffer();
            save_done = false;

            Button btn = (Button)sender;
            measure_item = btn.Text;
            string qxyz = btn.Tag.ToString();

            string s_msg = null;

            if (qxyz == "QX") {
                s_msg = "QX";
            }
            else if (qxyz == "QY") {
                s_msg = "QY";
            }
            else if (qxyz == "QZ") {
                s_msg = "QZ";
            }

            //どれにも当てはまらない時は終了
            if (s_msg == null) { return; }

            view_satndard();

            //シリアルポートが開いてない時は終了
            if (serialPort1.IsOpen) {
                try {
                    serialPort1.Write(s_msg + "\r\n");
                    serial_rec_end = false;
                }
                catch (Exception ex) {
                    MessageBox.Show($"コマンド送信失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else {
                MessageBox.Show("シリアルポートが開いていないため通信できません。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// データグリッドビューに入力してあるものから測定箇所_測定項目ごとの統計値算出
        /// <para>測定数、平均値、最大値、最小値、標準偏差</para>
        /// </summary>
        private Dictionary<string, SampleData> update_item_statistics() {

            string key;
            double value;
            string ok_ng;
            bool is_ng = false;
            //データグリッドビューから読み取って測定箇所_測定項目ごとの値リスト作成
            //valuelist_dic={Blue_Anode_BS:[54,53.3,45・・・],Red_Cathode_BS:[45,35,55・・],}
            Dictionary<string, List<double>> valuelist_dic = new Dictionary<string, List<double>>();
            for (int i = 0; i < dataGridView1.RowCount; i++) {
                key = $"{dataGridView1.Rows[i].Cells[0].Value.ToString()}_{dataGridView1.Rows[i].Cells[1].Value.ToString()}";

                value = double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());

                if (valuelist_dic.ContainsKey(key)) {
                    valuelist_dic[key].Add(value);
                }
                else {
                    List<double> list = new List<double>() { value };
                    valuelist_dic.Add(key, list);
                }

                if (!is_ng) {
                    ok_ng = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    if (ok_ng == "NG") { is_ng = true; }
                }
            }
            //NGが無い場合は"NGがあります。"ラベルを見えなくする
            if (!is_ng) { ok_ng_label.Visible = false; }

            //測定箇所_測定項目ごとの統計値計算して辞書に保存
            var sample_data_dic = new Dictionary<string, SampleData>();
            valuelist_dic.Keys.ToList().ForEach(k =>
            {
                int cnt = valuelist_dic[k].Count;
                double ave = valuelist_dic[k].Average();
                double max = valuelist_dic[k].Max();
                double min = valuelist_dic[k].Min();

                double sigma = valuelist_dic[k].StandardDeviation();
                if (sigma <= 0) { sigma = double.NaN; }

                if (standard_dic != null && standard_dic.ContainsKey(k)) {
                    //メジャーリング測定値には上下限値ある
                    sample_data_dic.Add(k, new SampleData(cnt, ave, max, min, sigma, standard_dic[k].Lower, standard_dic[k].Upper));
                }
                else {
                    sample_data_dic.Add(k, new SampleData(cnt, ave, max, min, sigma));
                }

            });

            return sample_data_dic;
        }

        /// <summary>
        /// 測定項目ごとの測定数記録
        /// </summary>
        private void update_item_cnt() {

            //測定項目ごとの統計値計算
            item_stat_dic = update_item_statistics();

            //ラベルの測定数表示を更新
            List<string> item_keys = item_stat_dic.Keys.ToList();
            item_keys.Sort();
            item_cnt_textBox.Text = "";
            foreach (string key in item_keys) {

                int cnt = item_stat_dic[key].Count;
                string ave = "-";
                if (Process == "WB") {
                    ave = item_stat_dic[key].Ave.ToString("F2");
                }
                else if (Process == "SDR") {
                    ave = item_stat_dic[key].Ave.ToString("F4");
                }

                item_cnt_textBox.Text += $"{key} : {cnt.ToString()}  Ave : {ave}\r\n";
            }


            //測定数チェック
            num_ng_textBox.Text = "";
            measure_num_check_label.Visible = false;
            if (measure_num_check && standard_dic != null) {
                List<string> standard_keys = standard_dic.Keys.ToList();
                standard_keys.Sort();

                foreach (string sk in standard_keys) {

                    //基準測定数が0より大きいならcheckする
                    int sc = standard_dic[sk].Count;
                    if (sc > 0) {


                        int ic = 0;
                        if (item_stat_dic.ContainsKey(sk)) {
                            //測定データに測定箇所_測定項目があれば更新
                            ic = item_stat_dic[sk].Count;
                        }

                        //測定データ数 < 基準測定数なら
                        if (ic < sc) {
                            measure_num_check_label.Visible = true;
                            num_ng_textBox.Text += $"{sk} : 残り{(sc - ic).ToString()}個\r\n";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// データグリッドビューに書く処理
        /// </summary>
        private void measure_data_write_to_dgv(string text, bool? ok_ng) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => measure_data_write_to_dgv(text, ok_ng)), null);
                return;
            }
            else {
                string judge = "";
                if (ok_ng == true) {
                    judge = "OK";
                }
                else if (ok_ng == false) {
                    judge = "NG";
                }

                //データグリッドビューに書き込み
                dataGridView1.Rows.Add(measure_place, measure_item, text, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), judge);
                //表示位置調整
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                //行番号書き込み
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].HeaderCell.Value = dataGridView1.Rows.Count.ToString();
                //測定値NGがあれば測定値セルを赤にする
                if (ok_ng == false) {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.Red;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.ForeColor = Color.White;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Style.BackColor = Color.Red;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Style.ForeColor = Color.White;
                    ok_ng_label.Visible = true;
                }
                else if (ok_ng == true) {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.Yellow;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Style.BackColor = Color.Yellow;
                }

                //RGB素子に合わせてセルの色を変更する
                if (measure_place.Contains("Blue")) {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.BackColor = Color.Blue;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.ForeColor = Color.White;
                }
                else if (measure_place.Contains("Red")) {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.BackColor = Color.Red;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.ForeColor = Color.White;
                }
                else if (measure_place.Contains("Green")) {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.BackColor = Color.Green;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.ForeColor = Color.White;
                }

                //測定数記録更新
                update_item_cnt();

                dataGridView1.CurrentCell = null;
            }
        }

        /// <summary>
        /// 受信時イベント
        /// </summary>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {

            serialPort1.NewLine = "\r\n";

            double data;
            //受信バッファからの読み取りに失敗 or 変なデータを受信した場合は終了
            try {
                string r_msg = serialPort1.ReadLine();
                data = Double.Parse(r_msg);
            }
            catch (Exception ex) {
                MessageBox.Show($"データの受信に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                serial_rec_end = true;
                return;
            }

            if (Process == "WB") {

                //mm⇒μmへ変換
                data *= 1000;
            }
            else if (Process == "SDR") {
                //3桁の小数に変換
                data = Math.Round(data, 3, MidpointRounding.AwayFromZero);
            }

            //データが負の時は正にする
            if (data < 0) {
                data *= (-1);
            }

            bool? ok_ng = null;
            //規格上下限値と比較する
            if (standard_dic != null) {

                string key = $"{measure_place}_{measure_item}";

                if (standard_dic.ContainsKey(key)) {

                    double l = standard_dic[key].Lower;
                    double u = standard_dic[key].Upper;

                    if (data < l || u < data) {
                        //数値が上下限値から外れている
                        ok_ng = false;
                    }
                    else {
                        ok_ng = true;
                    }
                }
            }

            //データグリッドビューに書き込む
            measure_data_write_to_dgv(data.ToString(), ok_ng);
            serial_rec_end = true;
        }

        /// <summary>
        /// 保存ボタン処理
        /// </summary>
        private void save_button_Click(object sender, EventArgs e) {

            if (dataGridView1.Rows.Count < 1) {
                MessageBox.Show("データが0個です。\n保存は出来ません。", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string product = product_label.Text;
            string lotno = lotno_label.Text;

            string operator_no = operator_label.Text;
            if (operator_no == "") {
                MessageBox.Show("測定者コードが読み込まれていません。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string machine_no = machine_label.Text;
            if (machine_no == "") {
                MessageBox.Show("設備Noが読み込まれていません。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //保存先フォルダにアクセス出来るか調べる
            if (Directory.Exists(SaveFolder) == false) {
                MessageBox.Show("データ保存先フォルダが見つかりません。\n\nネットワーク接続出来ていない可能性があります。\nデータ保存先フォルダにアクセスできるか調べてください。",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //今の日時
            DateTime now = DateTime.Now;

            //記録するファイルパス
            string path = $"{SaveFolder}/{Process}_{now.ToString("yyyy_M")}.csv";

            //書き込むファイルが無い場合は作成する
            if (File.Exists(path) == false) {
                try {
                    //FileMode.Appendはファイルが無かったら新規作成する
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {

                        if (Process == "WB") {
                            string header = "機種,LotNo,測定者,設備No,日時,測定箇所,項目,測定値[μm],判定,メジャリング";
                            sw.WriteLine(header);
                        }
                        else if (Process == "SDR") {
                            string header = "機種,LotNo,測定者,設備No,日時,測定箇所,項目,測定値[mm],判定,メジャリング";
                            sw.WriteLine(header);
                        }
                    }
                }
                catch {
                    MessageBox.Show("データ保存ファイル作成に失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            //書き込むテキスト作成
            string text = "";
            var add_sql_data_list = new List<SQLite.AddSqlData>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                text += $"{product},{lotno},{operator_no},{machine_no},";
                text += dataGridView1.Rows[i].Cells[3].Value.ToString() + ",";    //測定日時
                text += dataGridView1.Rows[i].Cells[0].Value.ToString() + ",";    //測定箇所
                text += dataGridView1.Rows[i].Cells[1].Value.ToString() + ",";    //測定項目
                text += dataGridView1.Rows[i].Cells[2].Value.ToString() + ",";    //測定値
                text += dataGridView1.Rows[i].Cells[4].Value.ToString() + ",";    //判定
                text += MeasureSerial + "\n";                                     //メジャリングの型番_シリアル番号

                //データベース用のデータリスト作成
                add_sql_data_list.Add(new SQLite.AddSqlData(product, lotno, operator_no, machine_no,
                    dataGridView1.Rows[i].Cells[3].Value.ToString(),    //測定日時
                    dataGridView1.Rows[i].Cells[0].Value.ToString(),    //測定箇所
                    dataGridView1.Rows[i].Cells[1].Value.ToString(),    //測定項目
                    dataGridView1.Rows[i].Cells[2].Value.ToString(),    //測定値
                    dataGridView1.Rows[i].Cells[4].Value.ToString(),    //判定
                    MeasureSerial)                                      //メジャリングの型番_シリアル番号
                    );
            }

            //書き込み処理
            try {
                //FileMode.Appendはファイルが無かったら新規作成する
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(text);
                }
            }
            catch (Exception ex) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                File.AppendAllText($"{AppFolder}/save_error.txt", $"{dt}\tデータ保存に失敗しました。\t{ex.Message}");
                MessageBox.Show($"データ保存に失敗しました。\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //一応バックアップ
            var assd = new SQLite.AddSqlStatData(product, lotno, operator_no, machine_no, now.ToString("yyyy-MM-dd HH:mm:ss"), item_stat_dic);
            backup_mydatabase(add_sql_data_list, assd);

            product_label.Text = "";
            lotno_label.Text = "";
            operator_label.Text = "";
            machine_label.Text = "";
            dataGridView1.Rows.Clear();
            check_input_info();
            save_done = true;
            ReadQR_textBox.Enabled = true;
            ok_ng_label.Visible = false;
            measure_mode_groupBox.Enabled = true;
        }


        /// <summary>
        /// バックアップでSQLiteデータベースに書き込む
        /// </summary>
        /// <param name="asd_list"></param>
        /// <param name="assd"></param>
        private void backup_mydatabase(List<SQLite.AddSqlData> asd_list, SQLite.AddSqlStatData assd) {

            string filepath = $@"{AppFolder}/{Process}_database.sqlite3";

            create_table(filepath);

            var db = new SQLite(filepath);

            try {
                db.insert_data(asd_list);
                db.insert_data(assd);
            }
            catch (Exception ex) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                File.AppendAllText($"{AppFolder}/save_error.txt", $"{dt}\tバックアップデータベースへの書き込みに失敗しました。\t{ex.Message}");
                MessageBox.Show($"database insert error\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// SQLiteファイルが無い時に、データベースファイルとテーブルを作る
        /// </summary>
        private void create_table(string filepath) {

            if (File.Exists(filepath)) {
                return;
            }

            var db = new SQLite(filepath);

            //テーブル1作成
            string tablename = "DATA";
            Dictionary<string, string> dic = new Dictionary<string, string>() {
                { "Product", "text" },
                { "LotNo","text" },
                { "Operator", "text" },
                { "Machine", "int" },
                { "DateTime", "text" },
                { "Place", "text" },
                { "Item", "text" },
                { "Data", "real" },
                { "Judge", "text" },
                { "MeasureSerial", "text" },
            };
            db.create_table(tablename, dic);

            //テーブル2作成
            tablename = "STATDATA";
            dic = new Dictionary<string, string>() {
                { "Product", "text" },
                { "LotNo","text" },
                { "Operator", "text" },
                { "Machine", "int" },
                { "DateTime", "text" },
                { "Place", "text" },
                { "Item", "text" },
                { "Num", "int" },
                { "Max", "real" },
                { "Ave", "real" },
                { "Min", "real" },
                { "Range", "real" },
                { "Sigma", "real" },
                { "Cpk", "real" },
            };
            db.create_table(tablename, dic);
        }


        /// <summary>
        /// 保存先フォルダ選択ボタン処理
        /// </summary>
        private void SelectSaveFolder_button_Click(object sender, EventArgs e) {

            SelectSaveFolder_fBD.SelectedPath = SaveFolder;

            if (SelectSaveFolder_fBD.ShowDialog() == DialogResult.OK) {
                string fld = SelectSaveFolder_fBD.SelectedPath;
                //Vドライブは"V:"ではアクセスできないので書き換える
                if (fld.Substring(0, 2) == "V:") {
                    fld = fld.Replace("V:", "\\\\svfile2\\fileserver");
                }
                SaveFolder = fld;

                //選択したフォルダパスをconfigファイルに保存
                try {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["SaveFolder"].Value = SaveFolder;
                    config.Save();

                    SaveFolder_label.Text = SaveFolder;
                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをコンフィグファイルに保存するのを失敗しました。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// QRコード読み込み時処理
        /// </summary>
        private void ReadQR_textBox_KeyPress(object sender, KeyPressEventArgs e) {

            //QRコードリーダーの終端文字を<CR>にしておく必要がある
            if ((int)e.KeyChar == 13) {


                if (armsSystem) {
                    //ArmsSystemで機種/Lot情報取得
                    var dic = ArmsSystem.GetProductInfo(ReadQR_textBox.Text);

                    if (dic.ContainsKey("Error")) {
                        MessageBox.Show($"{dic["Error"]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else {
                        product_label.Text = dic["product"];
                        lotno_label.Text = dic["lotno"];

                        //マスタを読み込む為、この時点で測定モードパネルは変更出来ないようにする
                        measure_mode_groupBox.Enabled = false;
                    }
                }
                else {
                    //CEJのLot管理票のQRコード読み取り時の機種/Lotの35文字情報
                    if (ReadQR_textBox.Text.Length > 10) {
                        string lotno = ReadQR_textBox.Text.Substring(ReadQR_textBox.Text.Length - 10);
                        string product = ReadQR_textBox.Text.Replace(lotno, "");
                        product = product.Replace(" ", "");
                        product_label.Text = product;
                        lotno_label.Text = lotno;

                        //マスタを読み込む為、この時点で測定モードパネルは変更出来ないようにする
                        measure_mode_groupBox.Enabled = false;

                    }
                    else {
                        MessageBox.Show("機種名が10文字以下のことはありません", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


                if (product_label.Text != "") {
                    //機種上下限マスタを読み込む
                    var s = new Standard();
                    standard_dic = s.get_product_standard(product_label.Text);
                    if (standard_dic != null) {
                        check_standard_label.Text = "この機種は\n上下限値を判定します。";
                        check_standard_label.ForeColor = Color.Black;
                        check_standard_label.BackColor = Color.Yellow;
                    }
                    else {
                        check_standard_label.Text = "この機種には\n上下限値マスタがありません。\n規格値内かしっかり確認して下さい。";
                        check_standard_label.ForeColor = Color.White;
                        check_standard_label.BackColor = Color.Red;
                    }
                }

                ReadQR_textBox.Text = "";
                check_input_info();
            }
        }

        /// <summary>
        /// 作業者コード読み込み時処理
        /// </summary>
        private void operator_input_textBox_KeyPress(object sender, KeyPressEventArgs e) {

            //QRコードリーダーの終端文字を<CR>にしておく必要がある
            if ((int)e.KeyChar == 13) {
                operator_label.Text = operator_input_textBox.Text;
                operator_input_textBox.Text = "";
            }
        }

        /// <summary>
        /// 設備No読み込み時処理
        /// </summary>

        private void machine_input_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            //QRコードリーダーの終端文字を<CR>にしておく必要がある
            if ((int)e.KeyChar == 13) {

                try {
                    int _ = int.Parse(machine_input_textBox.Text);
                    machine_label.Text = machine_input_textBox.Text;
                }
                catch {
                    MessageBox.Show("設備番号は数字を入力して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                machine_input_textBox.Text = "";
            }

        }

        /// <summary>
        /// 通信接続中、機種名が入力されている時のみ測定できるようにする
        /// </summary>
        private void check_input_info() {
            if (connect_label.Text == "接続中" && product_label.Text != "") {
                process_panel.Enabled = true;
                ReadQR_textBox.Enabled = false;
            }
            else {
                process_panel.Enabled = false;
            }
        }

        /// <summary>
        /// ソフト終了ボタン処理
        /// </summary>
        private void FormClose_button_Click(object sender, EventArgs e) {

            DialogResult r;

            if (!save_done) {
                r = MessageBox.Show("データ保存をしていません。\n測定データは消えてしまいます。\n\n本当にソフト終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else {
                r = MessageBox.Show("本当にソフト終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (r == DialogResult.No) {
                return;
            }

            this.Close();
        }

        /// <summary>
        /// データグリッドビューの行削除ボタン処理
        /// </summary>
        private void delete_row_button_Click(object sender, EventArgs e) {

            DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;

            if (rows.Count != 1) {
                MessageBox.Show("1行を選択して下さい。", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int delete_row = rows[0].Index;

            if (MessageBox.Show($"{delete_row + 1}行目を削除しますか？",
                "削除の確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {

                //行削除
                dataGridView1.Rows.Remove(rows[0]);

                //測定数記録更新
                update_item_cnt();

                //データグリッドビューの行番号更新
                for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                    dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                }
            }
            dataGridView1.CurrentCell = null;
        }

        /// <summary>
        /// データグリッドビューの全データ消去
        /// </summary>
        private void allclear_button_Click(object sender, EventArgs e) {
            if (dataGridView1.Rows.Count == 0) {
                MessageBox.Show("データがありません。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult r = MessageBox.Show("測定データを全消去しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes) {
                dataGridView1.Rows.Clear();
                ok_ng_label.Visible = false;
                measure_num_check_label.Visible = false;
                num_ng_textBox.Text = "";
            }
        }

        /// <summary>
        /// ロット情報消去ボタン処理
        /// </summary>
        private void lotInfo_clear_button_Click(object sender, EventArgs e) {
            if (!save_done) {
                DialogResult r = MessageBox.Show("データ保存をしていません。\n測定データは消えてしまいます。\n\n本当にロット情報を消去しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No) {
                    return;
                }
            }
            product_label.Text = "";
            lotno_label.Text = "";
            dataGridView1.Rows.Clear();
            check_input_info();
            ReadQR_textBox.Enabled = true;
            save_done = true;
            ok_ng_label.Visible = false;
            measure_mode_groupBox.Enabled = true;
            measure_num_check_label.Visible = false;
            num_ng_textBox.Text = "";
        }

        /// <summary>
        /// フォーム上をクリックした時にデータグリッドビューを未選択にする処理
        /// </summary>
        private void MainForm_Click(object sender, EventArgs e) {
            dataGridView1.CurrentCell = null;
        }

        /// <summary>
        /// 測定数チェックするcheckboxの処理
        /// </summary>
        private void measure_num_checkBox_CheckedChanged(object sender, EventArgs e) {
            var cb = (CheckBox)sender;
            measure_num_check = cb.Checked;
        }
    }

    /// <summary>
    /// 測定データの統計値を保存するためのクラス
    /// </summary>
    public class SampleData {

        public int Count { get; private set; }
        public double Ave { get; private set; }
        public double Max { get; private set; }
        public double Min { get; private set; }

        /// <summary>
        /// 0より大きい値 or double.NaN
        /// </summary>
        public double Sigma { get; private set; }

        public double Range { get; }

        public double Upper { get; private set; }

        public double Lower { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="ave"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="sigma">0より大きい値 or double.NaNをセットする</param>
        public SampleData(int cnt, double ave, double max, double min, double sigma, double lower = double.NaN, double upper = double.NaN) {
            Count = cnt;
            Ave = ave;
            Max = max;
            Min = min;
            Sigma = sigma;
            Range = Max - Min;
            Lower = lower;
            Upper = upper;
        }

        /// <summary>
        /// Cpk取得
        /// <para>σが出せない時はdouble.NaNを返す</para>
        /// <para>上下限値が無い時はdouble.NaNを返す</para>
        /// <para>下限値しかない場合、上限値しかない場合は片側Cpkを返す</para>
        /// <para>上下限値がある時は、上限下限Cpkの小さい方を返す</para>
        /// </summary>
        public double Get_Cpk() {
            //σが出せない時
            if (double.IsNaN(Sigma)) {
                return double.NaN;
            }

            //上下限値が無い時
            if (double.IsNaN(Lower) && double.IsNaN(Upper)) {
                return double.NaN;
            }

            double low_cpk;
            double up_cpk;
            //下限値しかない場合
            if (!double.IsNaN(Lower) && double.IsNaN(Upper)) {
                low_cpk = (Ave - Lower) / (3 * Sigma);
                return low_cpk;
            }

            //上限値しかない場合
            if (!double.IsNaN(Upper) && double.IsNaN(Lower)) {
                up_cpk = (Upper - Ave) / (3 * Sigma);
                return up_cpk;
            }

            //上下限値ある場合
            low_cpk = (Ave - Lower) / (3 * Sigma);
            up_cpk = (Upper - Ave) / (3 * Sigma);

            //小さい値を返す
            if (low_cpk <= up_cpk) {
                return low_cpk;
            }
            else {
                return up_cpk;
            }
        }


    }










}

