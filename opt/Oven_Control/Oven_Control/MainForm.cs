using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.Security.Permissions;
using System.IO;

using OvenCommand;
using static Oven_Control.OperateFile;
using static Oven_Control.Program;

using static Oven_Control.ArmsSystem;
using KodaClassLibrary;
using System.Linq.Expressions;

namespace Oven_Control {



    public partial class MainForm : Form {

        private Dictionary<int, IPAddress> machine_info;
        private int selected_ovenNo;

        /// <summary>
        /// "DBC"とか"SDRC"とか"SCTC"とか
        /// </summary>
        private string selected_process;

        /// <summary>
        /// "DB2"とか"SDR1"とか"SCT3"とか
        /// </summary>
        private string oven_recipe;

        /// <summary>
        /// Startボタン押した時の時間
        /// </summary>
        private DateTime startbtn_push_time;

        private Thread check_start_thread;


        //コンストラクタ
        public MainForm() {
            InitializeComponent();

            //20230215 別クラスからMainFormの値を参照するため
            this.Load += MainForm_Load;
            
            //初期化処理
            MachineInfo_Initialize();

            MachineNo_comboBox.SelectedIndex = 0;
            Process_comboBox.SelectedIndex = 0;
            RecipeFolder_label.Text = RecipeFolder;
            if (use_ftp == true) {
                UseFTP_checkBox.Checked = true;
            }
            else {
                UseFTP_checkBox.Checked = false;
            }
            if (Directory.Exists(FtpFolder) == false) {
                Directory.CreateDirectory(FtpFolder);
            }
            operator_label.Text = "";

            if (armsSystem) {
                Step2_groupBox.Text = "マガジンQRコード読み取り";
                ReadQR_textBox.BackColor = Color.SandyBrown;
            }
#if DEBUG
            ShowTestForm_btn.Visible = Enabled;
#endif
            //開始完了チェックを表示するラベルの位置サイズ調整（デザイン上邪魔になるのでここで設定）
            CheckStart_label.Location = new Point(5, 460);
            CheckStart_label.Size = new Size(840, 260);

            this.Left = 0;
            this.Top = 0;

            //pm2_label.Visible = true;
            //pm2_label.Text = "FALCON55℃ステップ専用" + "\r\n" + "NG率:0.99%\r\nNG回数:20回";
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

        //20230215
        //Form1オブジェクトを保持するためのフィールド
        private static MainForm _formInstance;

        //Form1オブジェクトを取得、設定するためのプロパティ
        public static MainForm FormInstance
        {
            get
            {
                return _formInstance;
            }
            set
            {
                _formInstance = value;
            }
        }
        //TextBox1.Textを取得、設定するためのプロパティ
        public string LabelText
        {
            get
            {
                return pm2_label.Text;
            }
            set
            {
                pm2_label.Text = value;
                //pm2_label.Visible = true;
            }
        }

        //Form1のLoadイベントハンドラ
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            MainForm.FormInstance = this;
        }

        /// <summary>
        /// ｛設備番号：IPアドレス｝のDictionaryをセットする
        /// </summary>
        private void SetMachineInfo() {         
            machine_info = Read_MachineInfo();
            if (machine_info == null) {
                Info_textBox.Text = "設備情報がありませんでした";
            }
        }

        /// <summary>
        /// コンボボックスのリストに設備番号を追加する
        /// </summary>
        private void MachineNo_add_combobox() {
            if (machine_info != null) {
                MachineNo_comboBox.Items.Clear();
                foreach (int i in machine_info.Keys) {
                    MachineNo_comboBox.Items.Add(i);
                }
            }
        }

        /// <summary>
        /// コンボボックスのリストに工程名を追加する
        /// </summary>
        private void Process_add_combobox() {
            string[] name = ConfigurationManager.AppSettings["Process"].Split(',');
            foreach (string item in name) {
                Process_comboBox.Items.Add(item);
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void MachineInfo_Initialize() {
            SetMachineInfo();
            MachineNo_add_combobox();
            Process_add_combobox();
            Step1_groupBox.Enabled = true;
        }

        /// <summary>
        /// 設備番号選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNo_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            MachineInfo_label.Text = "Machine：";
            int num = int.Parse(MachineNo_comboBox.SelectedItem.ToString());
            MachineInfo_label.Text += num + "\n";
            MachineInfo_label.Text += "IP：";
            MachineInfo_label.Text += machine_info[num].ToString();
            selected_ovenNo = num;
            Step2_groupBox.Enabled = true;
        }

        /// <summary>
        /// 工程名選択された時のイベント
        /// </summary>
        private void Process_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            selected_process = Process_comboBox.SelectedItem.ToString();
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
                        product_info_dgv.Rows.Add(dic["product"], dic["lotno"]);
                    }

                    //ここから追加　20230123

                    //20230123 Armsロットを取得後の処理を追加しないといけない
                    //キュア炉の工程設定
                    string process_code = selected_process;

                    //キュア炉の工程設定はKODA APIを使用して、完了工程から新規

                    bool only_jisseki = true;
                    //キュア条件を取得する
                    string recipe;
                    try
                    {
                        recipe = Get_Recipe_from_ProductRecipeCSV(dic["product"], process_code);
                    }
                    catch (Exception ex)
                    {
                        string msg = $"キュア条件が取得できませんでした。\n投入工程が合っているか確認して下さい。" +
                            $"\n条件取得工程：{process_code}\n新システム対応ロット：";
                        if (only_jisseki)
                        {
                            msg += "No";
                        }
                        else
                        {
                            msg += "Yes";
                        };

                        msg += $"\n\n{ex.Message}";

                        MessageBox.Show(msg, "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ReadQR_textBox.Text = "";
                        return;
                    }

                    //キュア条件レシピが無いなら
                    if (recipe == null)
                    {
                        MessageBox.Show($"マスタにキュア条件がありませんでした。\n機種：{dic["product"]}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ReadQR_textBox.Text = "";
                        return;
                    }
                    else
                    {

                        //キュア条件が全部同じか確認
                        for (int i = 0; i < product_info_dgv.Rows.Count; i++)
                        {

                            string val = product_info_dgv.Rows[i].Cells[2].Value.ToString();//3列目

                            if (recipe != val)
                            {
                                MessageBox.Show($"キュア条件が違います。\n機種：{dic["product"]}\n条件:{recipe}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ReadQR_textBox.Text = "";
                                return;
                            }
                        }


                        //キュア条件が全部同じだった場合のみ追加
                        product_info_dgv.Rows.Add(dic["product"], dic["lotno"], recipe);
                        //表示位置調整
                        product_info_dgv.FirstDisplayedScrollingRowIndex = product_info_dgv.Rows.Count - 1;
                        if (only_jisseki)
                        {
                            product_info_dgv.Rows[product_info_dgv.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Tan;
                        }

                    }//ここまで追加　20230123


                }
                else {
                    //CEJのLot管理票のQRコード読み取り時の機種/Lotの35文字情報
                    //KODA APIを使用
                    if (ReadQR_textBox.Text.Length > 10) {
                        //機種名、ロット番号取得
                        string lotno = ReadQR_textBox.Text.Substring(ReadQR_textBox.Text.Length - 10);
                        string product = ReadQR_textBox.Text.Replace(lotno, "");
                        product = product.Replace(" ", "");

                        //キュア炉の工程設定
                        string process_code = selected_process;

                        //KODA APIでロット情報を取得
                        bool only_jisseki = true;
                        try {
                            var lotinfo = LotInfo.GetLotInfo_from_LotNo10(product, lotno);
                            if (lotinfo.NextProcess == null) {
                                string msg = $"このロットはもう完成しているので、投入工程はありません。\n";
                                msg += $"完了工程：{lotinfo.PreProcess.ProcessName}({lotinfo.PreProcess.ProcessCode})\n";
                                msg += $"新システム対応ロット：Yes";

                                MessageBox.Show(msg, "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ReadQR_textBox.Text = "";
                                return;
                            }
                            else {
                                //新システム対応している場合は書き換え
                                only_jisseki = false;
                                if (lotinfo.NowProcess == null) {
                                    //ARMS WEBで作業開始していない場合はNextProcessを読み取る
                                    process_code = lotinfo.NextProcess.ProcessCode;
                                }
                                else {
                                    //ARMS WEBで作業開始している場合はNowProcessを読み取る
                                    process_code = lotinfo.NowProcess.ProcessCode;
                                }
                            }
                        }
                        catch (NotNewSystemException) {
                            //新システムに対応していないだけの場合は何もしない
                        }
                        catch (Exception ex) {
                            MessageBox.Show(ex.Message, "確認", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ReadQR_textBox.Text = "";
                            return;

                        }


                        //キュア条件を取得する
                        string recipe;
                        try {
                            recipe = Get_Recipe_from_ProductRecipeCSV(product, process_code);
                        }
                        catch (Exception ex) {
                            string msg = $"キュア条件が取得できませんでした。\n投入工程が合っているか確認して下さい。" +
                                $"\n条件取得工程：{process_code}\n新システム対応ロット：";
                            if (only_jisseki) {
                                msg += "No";
                            }
                            else {
                                msg += "Yes";
                            };

                            msg += $"\n\n{ex.Message}";

                            MessageBox.Show(msg, "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ReadQR_textBox.Text = "";
                            return;
                        }

                        //キュア条件レシピが無いなら
                        if (recipe == null) {
                            MessageBox.Show($"マスタにキュア条件がありませんでした。\n機種：{product}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ReadQR_textBox.Text = "";
                            return;
                        }
                        else {

                            //キュア条件が全部同じか確認
                            for (int i = 0; i < product_info_dgv.Rows.Count; i++) {

                                string val = product_info_dgv.Rows[i].Cells[2].Value.ToString();//3列目

                                if (recipe != val) {
                                    MessageBox.Show($"キュア条件が違います。\n機種：{product}\n条件:{recipe}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    ReadQR_textBox.Text = "";
                                    return;
                                }
                            }


                            //キュア条件が全部同じだった場合のみ追加
                            product_info_dgv.Rows.Add(product, lotno, recipe);
                            //表示位置調整
                            product_info_dgv.FirstDisplayedScrollingRowIndex = product_info_dgv.Rows.Count - 1;
                            if (only_jisseki) {
                                product_info_dgv.Rows[product_info_dgv.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Tan;
                            }

                        }

                    }
                    else {
                        MessageBox.Show("機種名が10文字以下のことはありません", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ReadQR_textBox.Text = "";
            }
        }

        /// <summary>
        /// 作業者コード読み込み時処理
        /// </summary>
        private void operator_textBox_KeyPress(object sender, KeyPressEventArgs e) {

            //QRコードリーダーの終端文字を<CR>にしておく必要がある（<CR><LF>ではだめ）
            if ((int)e.KeyChar == 13) {
                operator_label.Text = operator_textBox.Text;
                operator_textBox.Text = "";
            }
        }

        /// <summary>
        /// 読み取り終了ボタン処理
        /// <para>キュア条件の確認する</para>
        /// </summary>
        private void ReadQREnd_btn_Click(object sender, EventArgs e) {
            ReadQR_textBox.Enabled = false;

            ////key:value　= 機種名:条件
            //var dic = new Dictionary<string, string>();

            ////dicにDataGridViewの機種名keyを追加 valueは"None"
            //for (int i = 0; i < product_info_dgv.Rows.Count; i++) {
            //    string new_key = product_info_dgv.Rows[i].Cells[0].Value.ToString();
            //    if (dic.ContainsKey(new_key)) {
            //        continue;
            //    }
            //    dic.Add(new_key, "None");
            //}

            ////dicのvalueをNoneからキュア条件に更新
            //dic = Read_ProductRecipeCSV(dic, selected_process + "_");

            ////dicにerrorがあれば終了
            //if (dic.ContainsKey("error")) {
            //    MessageBox.Show(dic["error"], "Recipe Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //DataGridViewに書き込みながらキュア条件が全部同じか確認
            //string recipe = "";
            //for (int i = 0; i < product_info_dgv.Rows.Count; i++) {
            //    string key = product_info_dgv.Rows[i].Cells[0].Value.ToString();
            //    product_info_dgv.Rows[i].Cells[2].Value = dic[key];
            //    product_info_dgv.Update();

            //    if (i == 0) {
            //        recipe = dic[key];
            //    }
            //    else {
            //        if (recipe != dic[key]) {
            //            MessageBox.Show($"キュア条件が違う機種があります\n対象機種：{key}");
            //            ReadQREnd_btn.Enabled = false;
            //            return;
            //        }
            //    }
            //}

            //oven_recipe = recipe;
            oven_recipe = product_info_dgv.Rows[0].Cells[2].Value.ToString();//1行3列目
            Start_btn.Enabled = true;
            ReadQREnd_btn.Enabled = false;
        }

        /// <summary>
        /// DataGridViewに行が追加された時のイベント
        /// </summary>
        private void product_info_dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            ReadQREnd_btn.Enabled = true;
            Step1_groupBox.Enabled = false;
        }

        /// <summary>
        /// やり直すボタン処理
        /// </summary>
        private void Retry_btn_Click(object sender, EventArgs e) {
            //条件説明ラベルのテキスト消去
            Explanation_label.Text = "-";
            //作業者コードラベルのテキスト消去
            operator_label.Text = "";
            //データグリッドビュークリア
            product_info_dgv.Rows.Clear();
            //QRコード読み取りテキストボックスON
            ReadQR_textBox.Enabled = true;
            //読み取り終了ボタンOFF
            ReadQREnd_btn.Enabled = false;
            //StartボタンOFF
            Start_btn.Enabled = false;
            //工程とか選択ボックスON
            Step1_groupBox.Enabled = true;
        }

        /// <summary>
        /// Startボタン処理
        /// </summary>
        private void Start_btn_Click(object sender, EventArgs e) {

            double ng;
            double count = 12;
            double time =520;
            string s;

            ng = count / (time / 10) * 100;
            s = ng.ToString("0.00");

            //別スレッドにて運転開始完了したかチェック
            startbtn_push_time = DateTime.Now;
            if (check_start_thread != null) {
                //既にスレッドが動いている場合は破棄
                if (check_start_thread.IsAlive) {
                    check_start_thread.Abort();
                }
            }
            check_start_thread = new Thread(check_start);
            check_start_thread.IsBackground = true;
            check_start_thread.Start();

            //作業者チェック
            if (operator_label.Text == "") {
                MessageBox.Show("作業者コードを入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //製品配置チェック
            DialogResult ret = MessageBox.Show("製品の投入配置は正しいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ret != DialogResult.Yes) {
                return;
            }

            //温度上下限異常ラベル見えなくする
            CheckTemp_label.Visible = false;
            //FALCONラベルを見えなくする
            falcon_ng_label.Visible = false;
            //FALCON　±2度監視結果ラベルを非表示にする
            pm2_label.Visible = false;

            //グラフのプロット消去
            Temp_chart.Series[0].Points.Clear();
            Temp_chart.Series[1].Points.Clear();
            Temp_chart.Series[2].Points.Clear();

            //投入製品情報配列作成(機種名,LotNo)
            string[,] input_product = new string[product_info_dgv.RowCount, 2];

            for (int i = 0; i < product_info_dgv.RowCount; i++) {
                for (int j = 0; j < 2; j++) {
                    input_product[i, j] = product_info_dgv.Rows[i].Cells[j].Value.ToString();
                }
            }

            //20230125
            //return;

            //ovenオブジェクト作成
            IPEndPoint local_ep = new IPEndPoint(machine_info[selected_ovenNo], 57732);
            Oven oven = new Oven(selected_ovenNo, local_ep, oven_recipe, operator_label.Text, input_product);
            //ovenインスタンスにイベント追加
            oven.RunFinish += RunFinish_event;
            oven.ChartPlot += Plot_event;
            oven.TempOver += TempOver_event;
            oven.TempOver_falcon += TempOver_falcon_event;

            //プログラム運転開始
            try {
                Info_textBox.Text = "";
                oven.StartPrgm1(Info_textBox);

                //ファイル記録関連前準備
                oven.WriteStartInfo();

                //ラベルアップデート
                Explanation_label.Text = oven.explanation;
                Info_textBox.Text += $"Oven{oven.MyNo} 運転開始\r\n";
                Info_textBox.Update();
            }
            catch (Exception ex) {
                Info_textBox.Text += ex.Message + "\r\n";
                Info_textBox.Text += "運転開始処理に失敗したので運転停止します。\r\n";
                Info_textBox.Update();

                Thread.Sleep(1500);
                //プログラム運転終了
                try {
                    oven.Set_KeyProtect_OFF();
                    oven.Set_Mode_Standby();
                    string msg = "運転停止させました。\r\n再度スタートをやり直して下さい。";
                    Info_textBox.Text += msg + "\r\n";
                    Info_textBox.Update();
                    MessageBox.Show(msg, "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch {
                    string msg = "運転停止に失敗しました。\r\n運転開始していた場合は手動で運転停止してからやり直して下さい。";
                    Info_textBox.Text += msg + "\r\n";
                    Info_textBox.Update();
                    MessageBox.Show(msg, "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            

            RunStatus_label.Text = "運転中";
            RunStatus_label.Update();
            

            //温度記録開始（非同期処理）
            Task logtemp_task = Task.Run(() => { oven.LogTemp(); });

        }

        /// <summary>
        /// プログラム運転終了した時のイベント用メソッド
        /// </summary>
        public void RunFinish_event(object sender, EventArgs e) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => RunFinish_event(this, EventArgs.Empty)), null);
            }
            else {
                RunStatus_label.Text = "停止中";
                Info_textBox.Text += $"Oven{selected_ovenNo} 運転終了\r\n";
                if(oven_recipe == "JUS3")
                {
                    pm2_label.Visible = true;
                }
            }
        }

        /// <summary>
        /// RunStatus_labelの文字が変化した時の処理
        /// </summary>
        private void RunStatus_label_TextChanged(object sender, EventArgs e) {
            RunStatus_label.Font = new Font(RunStatus_label.Font.FontFamily, 24);

            if (RunStatus_label.Text == "運転中") {
                Step1_groupBox.Enabled = false;
                Step2_groupBox.Enabled = false;
                RunStatus_label.BackColor = Color.Lime;
                Start_btn.Enabled = false;
                SoftEnd_btn.Enabled = false;
            }
            else if (RunStatus_label.Text == "定値運転中") {
                Step1_groupBox.Enabled = false;
                Step2_groupBox.Enabled = false;
                RunStatus_label.BackColor = Color.Yellow;
                RunStatus_label.Font = new Font(RunStatus_label.Font.FontFamily, 18);
                Start_btn.Enabled = false;
                SoftEnd_btn.Enabled = false;
            }
            else {
                Step1_groupBox.Enabled = true;
                Step2_groupBox.Enabled = true;
                RunStatus_label.BackColor = Color.FromArgb(255, 128, 0);
                SoftEnd_btn.Enabled = true;
            }
        }

        /// <summary>
        /// STOPボタン処理(待機状態にする処理)
        /// </summary>
        private void Standby_btn_Click(object sender, EventArgs e) {

            DialogResult dr = MessageBox.Show("製品が入っていませんか？\n\n運転停止させてもいいですか？", "確認", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No) {
                return;
            }

            IPEndPoint local_ep = new IPEndPoint(machine_info[selected_ovenNo], 57732);
            Oven oven = new Oven(selected_ovenNo, local_ep, oven_recipe, "someone");
            try {
                oven.Set_KeyProtect_OFF();
                Info_textBox.Text += $"Oven{oven.MyNo} キープロテクトOFF完了\r\n";
                Info_textBox.Update();

                oven.Set_Mode_Standby();
                Info_textBox.Text += $"Oven{oven.MyNo} 運転停止完了\r\n";
                Info_textBox.Update();

                RunStatus_label.Text = "停止中";
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("停止動作に失敗しました。\r\n手動で停止してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RunStatus_label.Text = "手動停止";
            }
        }


        /// <summary>
        /// 温度をチャートにプロットするイベントで使用するメソッド
        /// </summary>
        public void Plot_event(object sender, ChartPlotEventArgs e) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => Plot_event(this, e)), null);
            }
            else {
                Temp_chart.Series[0].Points.AddXY(e.date_time, e.temperature);
                if (e.upper != null) { Temp_chart.Series[1].Points.AddXY(e.date_time, e.upper); }
                if (e.lower != null) { Temp_chart.Series[2].Points.AddXY(e.date_time, e.lower); }
            }
        }

        /// <summary>
        /// 温度が上下限超えた時のイベント用メソッド
        /// </summary>
        public void TempOver_event(object sender, EventArgs e) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => TempOver_event(this, EventArgs.Empty)), null);
            }
            else {
                if (CheckTemp_label.Visible == false) {

                    //画面に異常発生表示
                    CheckTemp_label.Visible = true;

                    //メール送信
                   　mail_send();
                }
            }
        }

        /// <summary>
        /// FALCON専用 温度が±1度を超えた時のイベント用メソッド
        /// </summary>
        public void TempOver_falcon_event(object sender, EventArgs e)
        {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => TempOver_falcon_event(this, EventArgs.Empty)), null);
            }
            else
            {
                if (falcon_ng_label.Visible == false)
                {
                    //画面にFALCON温度監視異常発生表示
                    falcon_ng_label.Visible = true;

                    //メール送信
                   　mail_send_falcon();
                }
            }
        }

        /// <summary>
        /// 上下限値異常アラームメール送信
        /// </summary>
        private void mail_send() {
            
            if (use_mail) {

                try {
                    Dictionary<string, string> mailTo_dic = ReadMailInfo();

                    Mail mail = new Mail($"oven{selected_ovenNo.ToString()}", FromMail, mailTo_dic, "キュア炉温度上下限値異常");

                    mail.mailText = "監視ステップにて温度が±5℃から外れました。";

                    mail.send(SmtpServer, SmtpPort);
                }
                catch (Exception ex) {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"上下限値異常メール送信処理失敗  System Message：{ex.Message}");
                    MessageBox.Show("上下限値異常メール送信失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        /// <summary>
        /// FALCON専用　上下限値異常アラームメール送信
        /// </summary>
        private void mail_send_falcon()
        {

            if (use_mail)
            {
                //falcon 20230220
                //List<string> lot_list = new List<string>();
                ////投入ロット一覧を取得
                //for (int i = 0; i < product_info_dgv.Rows.Count; i++)
                //{
                //    lot_list.Add(product_info_dgv.Rows[i].Cells[1].Value.ToString());//2列目lotno
                //}

                try
                {
                    Dictionary<string, string> mailTo_dic = ReadMailInfo();

                    Mail mail = new Mail($"oven{selected_ovenNo.ToString()}", FromMail, mailTo_dic, "キュア炉温度上下限値異常");

                    mail.mailText = "Falcon_55度の監視ステップにて温度が±1℃から外れました。";

                    /*
                    //lot名をメッセージへ追加
                    foreach (string lot in lot_list)
                    {
                        mail.mailText += "\r\n" + lot;
                    }
                    */

                    mail.send(SmtpServer, SmtpPort);
                }
                catch (Exception ex)
                {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"上下限値異常メール送信処理失敗  System Message：{ex.Message}");
                    MessageBox.Show("上下限値異常メール送信失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        /// <summary>
        /// FALCON専用　上下限値異常アラームメール送信
        /// </summary>
        public void mail_send_NG1persent(string persent_str)
        {

            if (use_mail)
            {             
                try
                {
                    Dictionary<string, string> mailTo_dic = ReadMailInfo();

                    Mail mail = new Mail($"oven{selected_ovenNo.ToString()}", FromMail, mailTo_dic, "キュア炉1%管理外異常");

                    mail.mailText = "Falcon_55度の監視ステップにおいて、±1℃以上2℃以下の割合が1%以上でした。" + "\r\n" + "パーセント：" + persent_str + "%";

                    mail.send(SmtpServer, SmtpPort);
                }
                catch (Exception ex)
                {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"上下限値異常メール送信処理失敗  System Message：{ex.Message}");
                    MessageBox.Show("上下限値異常メール送信失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        /// <summary>
        /// テスト用フォーム表示
        /// </summary>
        private void ShowTestForm_btn_Click(object sender, EventArgs e) {

            //var a = KodaWebApi.GetLotInfo_from_LotNo18("920221227140000728");
            //var b = KodaWebApi.GetLotInfo_from_LotNo10("CU0209-1204E1W11-00B-6262", "22Z23DD003");

            var form = new TestForm();
            form.ShowDialog();
        }

        /// <summary>
        /// ソフト終了ボタン処理
        /// </summary>
        private void SoftEnd_btn_Click(object sender, EventArgs e) {

            if (RunStatus_label.Text == "運転中") {
                MessageBox.Show("運転中はこのボタンを押してはいけません", "確認", MessageBoxButtons.YesNo);
                return;
            }

            DialogResult dr = MessageBox.Show("ソフト終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.No) { return; }

            this.Close();
        }

        /// <summary>
        /// 条件フォルダ選択ボタン処理
        /// </summary>
        private void SelectRecipeFolder_btn_Click(object sender, EventArgs e) {
            if (RunStatus_label.Text == "運転中") {
                MessageBox.Show("運転中はこのボタンを押してはいけません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SelectRecipeFolder_fBD.Description = "キュア条件ファイルが保存してあるフォルダを選択してください。";
            SelectRecipeFolder_fBD.SelectedPath = RecipeFolder;

            if (SelectRecipeFolder_fBD.ShowDialog() == DialogResult.OK) {
                string fld = SelectRecipeFolder_fBD.SelectedPath;
                //Vドライブは"V:"ではアクセスできないので書き換える
                if (fld.Substring(0, 2) == "V:") {
                    fld = fld.Replace("V:", "\\\\svfile2\\fileserver");
                }
                RecipeFolder = fld;
                RecipeFolder_label.Text = RecipeFolder;

                //選択したフォルダパスをconfigファイルに保存
                try {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["RecipeFolder"].Value = RecipeFolder;
                    config.Save();

                    MessageBox.Show("フォルダパス変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch {
                    MessageBox.Show($"フォルダパスをコンフィグファイルに保存するのを失敗しました。",
                        "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// FTPアップデートを使用するかのチェックボックス操作時処理
        /// </summary>
        private void UseFTP_checkBox_CheckedChanged(object sender, EventArgs e) {
            CheckBox cb = (CheckBox)sender;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (cb.Checked == true) {
                use_ftp = true;
                config.AppSettings.Settings["UseFTP"].Value = "true";
            }
            else {
                use_ftp = false;
                config.AppSettings.Settings["UseFTP"].Value = "false";
            }
            config.Save();
        }

        /// <summary>
        /// 異常時にメール送信するかのチェックボックス操作時処理
        /// </summary>

        private void UseMail_checkBox_CheckedChanged(object sender, EventArgs e) {
            CheckBox cb = (CheckBox)sender;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (cb.Checked == true) {
                use_mail = true;
                config.AppSettings.Settings["UseMail"].Value = "true";
            }
            else {
                use_mail = false;
                config.AppSettings.Settings["UseMail"].Value = "false";
            }
            config.Save();
        }


        /// <summary>
        /// 拡大ボタン処理（チャートの拡大/縮小処理）
        /// </summary>
        private void Zoom_btn_Click(object sender, EventArgs e) {
            Button bt = (Button)sender;
            if (bt.Text == "拡大") {
                int cnt = Temp_chart.Series[0].Points.Count;
                if (cnt == 0) { return; }
                Temp_chart.ChartAreas[0].AxisY.Minimum = Temp_chart.Series[0].Points[cnt - 1].YValues[0] - 10;
                Temp_chart.ChartAreas[0].AxisY.Maximum = Temp_chart.Series[0].Points[cnt - 1].YValues[0] + 10;
                bt.Text = "縮小";
            }
            else {
                Temp_chart.ChartAreas[0].AxisY.Minimum = double.NaN;
                Temp_chart.ChartAreas[0].AxisY.Maximum = double.NaN;
                bt.Text = "拡大";

            }

        }

        /// <summary>
        /// 定値運転開始ボタン処理
        /// </summary>
        private void const_start_button_Click(object sender, EventArgs e) {

            int set_temp = 1000;

            foreach (RadioButton rb in const_rb_gB.Controls.OfType<RadioButton>()) {

                if ((bool)rb.Checked) {

                    set_temp = int.Parse(rb.Text.Replace("℃", ""));
                    break;
                }
            }

            //設定温度が更新されていない場合は終了
            if (set_temp == 1000) { return; }

            IPEndPoint local_ep = new IPEndPoint(machine_info[selected_ovenNo], 57732);
            Oven oven = new Oven(selected_ovenNo, local_ep, oven_recipe, "someone");
            try {
                oven.Set_Mode_Constant(set_temp, Info_textBox);

                RunStatus_label.Text = "定値運転中";
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("定値運転開始に失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        /// <summary>
        /// 制限時間内に運転開始完了したか確認する処理（マルチスレッドで動かす）
        /// </summary>
        private void check_start() {

            CheckStart_label.Invoke(new Action(() => CheckStart_label.Visible = true));

            TimeSpan delta;
            string text;
            bool send_mail = false;

            while (true) {

                delta = DateTime.Now - startbtn_push_time;

                text = "まだ運転開始完了していません。\n";
                text += "5分以内に開始完了しない場合は異常としてメール送信されます。\n";
                text += "\n";
                text += $"Startボタン押した時刻：{startbtn_push_time:yyyy/MM/dd HH:mm:ss}\n";
                text += $"{delta.Minutes}分 {delta.Seconds} 秒経過";


                if (delta.Minutes > 4 && send_mail == false) {

                    send_mail = true;

                    //メール送信
                    Task send_alarm_mail = Task.Run(() =>
                    {
                        try {
                            Dictionary<string, string> mailTo_dic = ReadMailInfo();

                            Mail mail = new Mail($"oven{selected_ovenNo.ToString()}", FromMail, mailTo_dic, "キュア炉開始異常");

                            mail.mailText = "キュア炉が5分以内に運転開始されませんでした。";

                            mail.send(SmtpServer, SmtpPort);
                        }
                        catch (Exception ex) {
                            string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            WriteAppend(file_errorlog, dt + "\t" + $"キュア炉開始異常メール送信処理失敗  System Message：{ex.Message}");
                            MessageBox.Show("キュア炉開始異常メール送信失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            send_mail = false;
                        }
                    });
                }

                if (send_mail) {
                    text += "\nキュア炉開始異常発生メールが送信されました。";
                }

                //テキスト書き込み
                CheckStart_label.Invoke(new Action(() => CheckStart_label.Text = text));

                //運転開始したらループ抜ける
                if (RunStatus_label.Text == "運転中") {
                    break;
                }

                Thread.Sleep(1000);
            }

            /*
             * ここからループ抜けた後の処理
             */
            CheckStart_label.Invoke(new Action(() => CheckStart_label.Visible = false));

            if (send_mail) {

                //メール送信
                Task send_start_mail = Task.Run(() =>
                {
                    try {
                        Dictionary<string, string> mailTo_dic = ReadMailInfo();

                        Mail mail = new Mail($"oven{selected_ovenNo.ToString()}", FromMail, mailTo_dic, "キュア炉5分経過後運転開始");

                        string mt = "キュア炉が5分経過後に運転開始されました。\n\n";
                        mt += $"Startボタン押した時刻：{startbtn_push_time:yyyy/MM/dd HH:mm:ss}\n";
                        mt += $"運転開始した時刻：{DateTime.Now:yyyy/MM/dd HH:mm:ss}";

                        mail.mailText = mt;

                        mail.send(SmtpServer, SmtpPort);
                    }
                    catch (Exception ex) {
                        string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        WriteAppend(file_errorlog, dt + "\t" + $"キュア炉5分経過後運転開始メール送信処理失敗  System Message：{ex.Message}");
                        MessageBox.Show("キュア炉5分経過後運転開始メール送信失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }

        }


    }
}

