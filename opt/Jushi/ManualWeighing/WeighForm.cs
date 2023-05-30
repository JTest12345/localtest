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
using System.Security.Permissions;

using ResinPrg;//Junki Nakamura DLL

using static ManualWeighing.Program;
using ResinClassLibrary;
using System.Diagnostics.Contracts;

namespace ManualWeighing {

    public partial class WeighForm : Form {

        private const string CUP = "Cup";
        private const string GROSS_WEIGHT = "Gross Weight";

        /// <summary>
        /// 作業者
        /// </summary>
        private string employee_code;

        /// <summary>
        /// ドア開閉チェックに使用するDIOクラスオブジェクト
        /// </summary>
        private ContecDio dio;

        /// <summary>
        /// グローブボックスのドアが開いているか
        /// <para>true = 開いている</para>
        /// </summary>
        private bool door_open = false;

        /// <summary>
        /// 計量するレシピ
        /// </summary>
        private Recipe recipe;

        /// <summary>
        /// 現在計量している部材順番
        /// <para>カップ計量を最初に実施するので初期値は0とする</para>
        /// </summary>
        private int now_weigh_order = 0;

        /// <summary>
        /// 手配合する部材辞書
        /// <para>int : 順番　RecipeBuzai : 部材情報</para>
        /// <para>0 : カップ</para>
        /// <para>1～ : 部材</para>
        /// </summary>
        private Dictionary<int, RecipeBuzai> mix_order_dic;

        /// <summary>
        /// 現在計量している部材
        /// </summary>
        private RecipeBuzai now_weigh_buzai {
            get {
                if (mix_order_dic.Keys.Contains(now_weigh_order)) {
                    return mix_order_dic[now_weigh_order];
                }
                else {
                    return null;
                }
            }
        }

        private RecipeBuzai gross_weight;

        /// <summary>
        /// 計量値データ保持用キュー
        /// </summary>
        private Queue<WeighingData> stable_datas = new Queue<WeighingData>();

        /// <summary>
        /// 計量OKかどうか
        /// <para>true = OK</para>
        /// </summary>
        private bool weigh_ok = false;

        /// <summary>
        /// 今回のレシピでグローブボックスが必須かどうか
        /// <para>true = 必須</para>
        /// </summary>
        private bool need_groveBox = false;

        /// <summary>
        /// 作製している場所がグローブボックスかどうか
        /// <para>true = グローブボックス</para>
        /// </summary>
        private bool is_groveBox = false;

        /// <summary>
        /// 現在計量中かどうか
        /// <para>true = 現在計量中</para>
        /// </summary>
        private bool now_weighing = false;

        private static MainForm MFinstance;

        public static MainForm MainFormInstance
        {
            get
            {
                return MFinstance;
            }
            set 
            {
                MFinstance = value;
            }

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WeighForm(Recipe rec, string emp_code, bool isGroveBox, bool needGroveBox) {
            InitializeComponent();

            //ラベル位置設定
            door_check_label.Location = weigh_panel.Location;
            door_check_error_label.Location = new Point(door_check_label.Location.X, door_check_label.Location.Y + door_check_label.Height);

            //初期設定
            electronicBalance_serialPort.PortName = AppSettings.BalanceSerialPortName;
            dio = new ContecDio(AppSettings.DioDeviceName);
            recipe = rec;
            employee_code = emp_code;
            need_groveBox = needGroveBox;
            is_groveBox = isGroveBox;

            if(rec.MoldType.Contains("SDR"))
            {
                //配合順番辞書設定 ※ダム専用のカップ重量を設定 20230220
                mix_order_dic = new Dictionary<int, RecipeBuzai> {
                { 0, new RecipeBuzai { Name = CUP, Amount = 56, LowerAllowableErrorGram = 5, UpperAllowableErrorGram = 5 } }
                };
            }

            else 
            {             
                //配合順番辞書設定 ※通常樹脂剤用カップ
                mix_order_dic = new Dictionary<int, RecipeBuzai> {
                { 0, new RecipeBuzai { Name = CUP, Amount = 12, LowerAllowableErrorGram = 5, UpperAllowableErrorGram = 5 } }
                };
            }

            int i = 1;
            foreach (var b in recipe.UseBuzai.Where(x => x.UseAutoMachine == false).OrderBy(x => x.MixOrder)) {
                mix_order_dic.Add(i, b);
                i += 1;
            }

            //総重量判定有りの場合は最後に総重量測定を追加
            if (AppSettings.CheckGrossWeight) {
                int last = mix_order_dic.Keys.Max();
                gross_weight = new RecipeBuzai {
                    Name = GROSS_WEIGHT,
                    LowerAllowableErrorGram = AppSettings.GrossWeightAllowableErrorGram,
                    UpperAllowableErrorGram = AppSettings.GrossWeightAllowableErrorGram,
                    Amount = 0
                };
                if (need_groveBox) {
                    gross_weight.NeedGroveBox = true;
                }
                mix_order_dic.Add(last + 1, gross_weight);
            }
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
        /// フォームロードイベント
        /// </summary>
        private void WeighForm_Load(object sender, EventArgs e) {
            status_label.Text = "";

            //MainForm M = new MainForm();
            bool flgtest = MainForm.fusoku_flg;

            //自分自身のフォームを最大化
            this.WindowState = FormWindowState.Maximized;

            try {
                var lotno = $"LotNo:{recipe.LotNoList[0]}\n";
                if (recipe.LotNoList.Count > 1) {
                    for (int i = 1; i < recipe.LotNoList.Count; i++) {
                        lotno += $"          {recipe.LotNoList[i]}\n";
                    }
                }
                lotno = lotno.Substring(0, lotno.Length - 1);//最後の\を取り除く
                recipe_info_label.Text = $"Cup No:{recipe.CupNo}\n機種:{recipe.ProductName}\n" +
                    $"波長ランク:{recipe.WavelengthRank}\n{lotno}\n" +
                    $"樹脂種類:{recipe.MoldType}/{recipe.FlowMode}/{recipe.MixTypeCode}\n作業者:{employee_code}";

                Initialize_buzaiDGV();

                //シリアルポートオープン
                electronicBalance_serialPort.NewLine = "\r\n";
                electronicBalance_serialPort.Open();
                electronicBalance_serialPort.DiscardInBuffer();
                //ゼロリセットコマンド送信
                electronicBalance_serialPort.WriteLine("Z");
              
                //グローブボックス使用が必須なら
                if (need_groveBox) {
                    //CONTEC IOボードの初期化処理
                    dio.Initialize();

                    //グローブボックスドア開閉チェックタイマー開始
                    door_check_timer.Start();                 
                }

                //最初にカップを計量する
                target_value_textBox.Text = now_weigh_buzai.Amount.ToString();
                plus_allow_label.Text = $"＋{now_weigh_buzai.UpperAllowableErrorGram.ToString("f3")}";
                minus_allow_label.Text = $"－{now_weigh_buzai.LowerAllowableErrorGram.ToString("f3")}";
                status_label.Text = "カップを置いてください。\n安定したらNextボタンを押して下さい。";
                now_weigh_buzai.StartTime = DateTime.Now;
                now_weighing = true;

            }
            catch (TimeoutException tex) {
                MessageBox.Show("電子天秤のゼロリセットが出来ませんでした。\n通信が出来ているか確認して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FormClose();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FormClose();
            }
        }

        /// <summary>
        /// フォームクローズ処理
        /// </summary>
        private void FormClose() {

            now_weighing = false;

            //statusラベルの点滅タイマー停止
            status_label_timer.Stop();

            //グローブボックスドア開閉チェックタイマー停止
            status_label.Text = "グローブボックスドア開閉チェックタイマー停止";
            status_label.Update();
            door_check_timer.Stop();

            //風速計
            kanomax_realtime_value.Stop();

            //DIOデバイスの終了処理
            dio.End();
            status_label.Text = "DIOデバイスの終了";
            status_label.Update();

            //電子天秤シリアルポートを終了する
            //シリアルポート終了処理難しいので最後にする（最悪フリーズ）
            status_label.Text = "シリアルポートクローズ";
            status_label.Update();

            //シリアルポートは連続受信中の為、別スレッドからcloseしないとデッドロックする
            Task.Run(() =>
            {
                while (electronicBalance_serialPort.IsOpen) {
                    electronicBalance_serialPort.ReadTimeout = 0;
                    electronicBalance_serialPort.Close();
                    Thread.Sleep(500);
                }

            });

            Thread.Sleep(1000);
            while (electronicBalance_serialPort.IsOpen) {
                Thread.Sleep(500);
            }

            //風速計シリアルポート閉じる
            Task.Run(() =>
            {               
                while (kanomax_fusoku_serialPort.IsOpen)
                {
                    kanomax_fusoku_serialPort.ReadTimeout = 0;
                    kanomax_fusoku_serialPort.Close();
                    Thread.Sleep(500);
                }
            });

            Thread.Sleep(1000);           
            while (kanomax_fusoku_serialPort.IsOpen)
            {
                Thread.Sleep(500);
            }

            this.Close();
        }

        /// <summary>
        /// 使用部材データグリッドビュー初期化
        /// </summary>
        private void Initialize_buzaiDGV() {
            foreach (var rec in recipe.UseBuzai.OrderBy(x => x.MixOrder)) {
                use_buzai_dataGridView.Rows.Add();
                int i = use_buzai_dataGridView.Rows.Count - 1;

                use_buzai_dataGridView.Rows[i].Cells[1].Value = rec.Name;
                use_buzai_dataGridView.Rows[i].Cells[2].Value = rec.Fcode;
                use_buzai_dataGridView.Rows[i].Cells[3].Value = rec.LotNo;
                use_buzai_dataGridView.Rows[i].Cells[4].Value = rec.Amount;
                use_buzai_dataGridView.Rows[i].Cells[5].Value = $"+{((decimal)rec.UpperAllowableErrorGram).ToString("f3")} -{((decimal)rec.LowerAllowableErrorGram).ToString("f3")}";

                if (rec.UseAutoMachine) {
                    use_buzai_dataGridView.Rows[i].DefaultCellStyle.Font = new Font(use_buzai_dataGridView.DefaultCellStyle.Font, FontStyle.Strikeout);
                    use_buzai_dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            //データグリッドビューサイズ調整
            int width = 0;
            int height = 0;
            for (int i = 0; i < use_buzai_dataGridView.Columns.Count; i++) {
                width += use_buzai_dataGridView.Columns[i].Width;
            }
            for (int i = 0; i < use_buzai_dataGridView.Rows.Count; i++) {
                height += use_buzai_dataGridView.Rows[i].Height;
            }

            use_buzai_dataGridView.Width = use_buzai_dataGridView.RowHeadersWidth + width;
            use_buzai_dataGridView.Height = use_buzai_dataGridView.ColumnHeadersHeight + height;
        }

        /// <summary>
        /// 部材QRコード読み取り時処理
        /// </summary>
        private void input_buzaiQR_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            //QRコードリーダーの終端文字を<CR>にしておく必要がある
            if ((int)e.KeyChar == 13) {
                string str = input_buzaiQR_textBox.Text;
                var array = str.Split(',');

                string fcode, lotno;

                //string format;
                //string label_limit;
                //DateTime limit;
                //読み込んでいるQRコードが合っているか
                if (array.Length == 10) {
                    //4Mシステムの部材ラベル
                    fcode = array[0];
                    lotno = array[3];
                    //label_limit = array[7];
                    //format = "yyMMdd";
                    //limit = DateTime.ParseExact(label_limit, format, null);
                    ////20230412 有効期限チェック
                    //if (limit < DateTime.Now)
                    //{
                    //    status_label.Text = "有効期限がきれている部材です。";
                    //    return;
                    //}
                }
                else if (array.Length == 4) {
                    //CETの部材管理QR
                    fcode = array[0];
                    lotno = array[2];
                }
                else {
                    status_label.Text = "読み込んでいるQRが違います。";
                    input_buzaiQR_textBox.Text = "";
                    return;
                }

                //部材のFコードが一致しているか
                if (fcode == now_weigh_buzai.Fcode) {
                    buzai_fcode_textBox.BackColor = Color.LimeGreen;
                }
                else {
                    status_label.Text = "部材が違います。";
                    buzai_fcode_textBox.BackColor = Color.LightPink;
                    input_buzaiQR_textBox.Text = "";
                    return;
                }

                //部材のLotNoが一致しているか
                if (string.IsNullOrEmpty(now_weigh_buzai.LotNo)) {
                    //LotNoの指定がない場合はここでセットする
                    now_weigh_buzai.LotNo = lotno;
                }
                //指定のLotNoがある時のみチェックする
                else {
                    if (lotno == now_weigh_buzai.LotNo) {
                        buzai_lotno_textBox.BackColor = Color.LimeGreen;
                    }
                    else {
                        status_label.Text = "ロットが違います。";
                        buzai_lotno_textBox.BackColor = Color.LightPink;
                        input_buzaiQR_textBox.Text = "";
                        return;
                    }
                }

                status_label.Text = "計量開始して下さい。";
                input_buzaiQR_textBox.Enabled = false;
                input_buzaiQR_textBox.Text = "";

                //try catch追加 20230321
                try 
                {
                    electronicBalance_serialPort.WriteLine("Z");
                }
                catch (TimeoutException tex)
                {
                    //送受信バッファクリア
                    electronicBalance_serialPort.DiscardOutBuffer();
                    electronicBalance_serialPort.DiscardInBuffer();
                    //タイムアウトした場合、再度ゼロリセットコマンドを送る
                    electronicBalance_serialPort.WriteLine("Z");
                }

                //単純にコマンドを送るコードから上記へ修正　これで良ければ、ろ過後(WeighFiltrationForm.cs)も同様に修正
                //electronicBalance_serialPort.WriteLine("Z");

                now_weighing = true;
                now_weigh_buzai.StartTime = DateTime.Now;
                target_value_textBox.BackColor = SystemColors.Control;
                result_value_textBox.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// グローブボックスドア開閉チェックタイマー処理
        /// </summary>
        private void door_check_timer_Tick(object sender, EventArgs e) {

            //風速計の値を取得
            

            //グローブボックス必要ない部材の時はドアが閉じていることにする
            if (now_weigh_buzai != null && now_weigh_buzai.NeedGroveBox == false) {
                door_check_label.Visible = false;
                door_open = false;
                return;
            }

            byte sensor_on_off = 0;

            //入力ビットの読み取り
            try {
                sensor_on_off = dio.Get_InputBit(AppSettings.DoorCheckBitNo);
                door_check_error_label.Visible = false;
            }
            catch (Exception ex) {
                //ドアセンサーデータ取得失敗の場合はドアが開いていることにする
                sensor_on_off = 0;
                door_check_error_label.Text = ex.Message;
                door_check_error_label.Visible = true;
            }

            if (sensor_on_off == 1) {
                //ドアが閉じている
                door_check_label.Visible = false;
                result_value_textBox.BackColor = SystemColors.Control;
                door_open = false;
            }
            else {
                //ドアが開いている
                door_check_label.Visible = true;
                result_value_textBox.BackColor = Color.Black;
                door_open = true;
            }
        }

        /// <summary>
        /// 電子天秤からの受信時処理
        /// </summary>
        private void electronicBalance_serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {

            /*連続受信中にCloseさせるので処理は全部tryの中に入れておく*/
            try {
                //計量中でない場合は終了
                if (now_weighing == false) {
                    this.Invoke(new Action(() => next_button.Visible = false));
                    electronicBalance_serialPort.DiscardInBuffer();
                    return;
                }

                string r_msg = electronicBalance_serialPort.ReadLine();

                //計量状態を取得
                string stable = r_msg.Substring(0, 2);

                //計量データを取得
                decimal data = decimal.Parse(r_msg.Substring(3, 9));
                DisplayWeighingValue(data);

                //キューにデータを溜める
                stable_datas.Enqueue(new WeighingData { Time = DateTime.Now, Value = data });

                //一定個数以上データが溜まっていたら、先頭の古いデータを取り除く
                if (stable_datas.Count > 200) {
                    stable_datas.Dequeue();
                }

                //ドア空いていたら溜めていたデータを消す(KSFだけ)
                if (door_open) {
                    WeighReset();
                    return;
                }

                //天秤データが安定値か判定
                if (stable == "ST") {
                    //STは安定を示している
                    stable_mark_label.ForeColor = Color.Orange;
                }
                else {
                    stable_mark_label.ForeColor = Color.LightGray;
                    //不安定だったら溜めていたデータを消す
                    WeighReset();
                    return;
                }

                decimal lower = now_weigh_buzai.LowerLimit;
                decimal upper = now_weigh_buzai.UpperLimit;

                //計量値が範囲内か判定
                if (lower <= data && data <= upper) {
                    //範囲内の場合
                    within_label.ForeColor = Color.SteelBlue;
                }
                else {
                    within_label.ForeColor = Color.LightGray;
                    //範囲外だったら溜めていたデータを消す
                    WeighReset();
                    return;
                }

                //安定時間の判定
                var delta = stable_datas.Last().Time - stable_datas.First().Time;
                this.Invoke(new Action(() => stable_time_label.Text = $"{delta.TotalSeconds.ToString("f3")} 秒"));
                if (delta.TotalSeconds > AppSettings.StableSecond) {
                    weigh_ok = true;
                }
                else {
                    weigh_ok = false;
                }

                //計量OKならNextボタン押せるようにする
                if (weigh_ok) {
                    this.Invoke(new Action(() =>
                    {
                        next_button.Visible = true;
                        next_button.Select();
                    }));

                }
                else {
                    this.Invoke(new Action(() =>
                    {
                        next_button.Visible = false;
                    }));
                }

            }
            catch (ObjectDisposedException ode) {
                //終了処理時にformクローズした後にシリアル通信のデータレシーブイベント内のthis.Invokeで例外発生する
                return;
            }
            //受信バッファからの読み取りに失敗 or 変なデータを受信した場合は終了
            catch (Exception ex) {
                WeighReset();
                this.Invoke(new Action(() => result_value_textBox.Text = ""));
                return;
            }
        }

        /// <summary>
        /// 計量値を表示する
        /// </summary>
        private void DisplayWeighingValue(decimal value) {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => DisplayWeighingValue(value)), null);
                return;
            }
            else {
                result_value_textBox.Text = value.ToString("f3");
            }
        }

        /// <summary>
        /// 計量リセット処理
        /// </summary>
        private void WeighReset() {

            //別スレッドから呼び出されたとき Invokeして呼びなおす
            if (this.InvokeRequired) {
                this.Invoke(new Action(() => WeighReset()), null);
                return;
            }
            else {
                weigh_ok = false;
                next_button.Visible = false;
                stable_time_label.Text = "";
                stable_datas.Clear();
            }
        }

        /// <summary>
        /// Nextボタン処理
        /// </summary>
        private async void next_button_Click(object sender, EventArgs e) {

            //ペダルで高速で連打された時の対策
            if (next_button.Visible == false) {
                return;
            }

            //初期化
            next_button.Visible = false;
            now_weighing = false;
            weigh_ok = false;

            //計量結果取得
            now_weigh_buzai.ResultAmount = decimal.Parse(result_value_textBox.Text);
            now_weigh_buzai.EndTime = DateTime.Now;

            //総重量に追加
            if (AppSettings.CheckGrossWeight) {
                if (now_weigh_buzai.Name != GROSS_WEIGHT) {
                    gross_weight.Amount += now_weigh_buzai.ResultAmount;
                    gross_weight_label.Text = gross_weight.Amount.ToString();
                }
            }

            //計量結果表示
            if (now_weigh_order == 0) {
                //カップ計量の場合
                cup_weight_label.Text = now_weigh_buzai.ResultAmount.ToString();
            }
            else {
                //計量結果記載
                for (int i = 0; i < use_buzai_dataGridView.Rows.Count; i++) {
                    if (use_buzai_dataGridView.Rows[i].Cells[1].Value.ToString() == now_weigh_buzai.Name) {
                        use_buzai_dataGridView.Rows[i].Cells[0].Value = "✔";
                        use_buzai_dataGridView.Rows[i].Cells[6].Value = now_weigh_buzai.ResultAmount;
                        use_buzai_dataGridView.Rows[i].DefaultCellStyle.BackColor = SystemColors.Window;
                        use_buzai_dataGridView.Update();
                    }
                }
            }


            //次の部材へ
            now_weigh_order += 1;

            //まだ部材を全て計量していない場合
            if (now_weigh_buzai != null) {
                //次の部材を表示
                var buzai = now_weigh_buzai;

                if (buzai.Name != GROSS_WEIGHT) {
                    buzai_name_textBox.Text = buzai.Name;
                    buzai_fcode_textBox.Text = buzai.Fcode;
                    buzai_lotno_textBox.Text = buzai.LotNo;
                    buzai_fcode_textBox.BackColor = SystemColors.Control;
                    buzai_lotno_textBox.BackColor = SystemColors.Control;
                    for (int i = 0; i < use_buzai_dataGridView.Rows.Count; i++) {
                        if (use_buzai_dataGridView.Rows[i].Cells[1].Value.ToString() == buzai.Name) {
                            use_buzai_dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }

                    //目標値、許容誤差表示
                    target_value_textBox.Text = buzai.Amount.ToString("f3");
                    plus_allow_label.Text = $"＋{buzai.UpperAllowableErrorGram}";
                    minus_allow_label.Text = $"－{buzai.LowerAllowableErrorGram}";

                    status_label.Text = "次の部材の部材ラベルQRコードを読み込んでください。";
                    input_buzaiQR_textBox.Enabled = true;
                    input_buzaiQR_textBox.Select();
                    target_value_textBox.BackColor = Color.Black;
                    result_value_textBox.BackColor = Color.Black;
                    return;
                }
                else {
                    //総重量計量
                    status_label.Text = "樹脂カップを持ち上げて総重量を計量開始して下さい。";
                    buzai.Amount *= -1;
                    target_value_textBox.Text = buzai.Amount.ToString("f3");
                    plus_allow_label.Text = $"＋{buzai.UpperAllowableErrorGram}";
                    minus_allow_label.Text = $"－{buzai.LowerAllowableErrorGram}";
                    target_value_textBox.BackColor = SystemColors.Control;
                    electronicBalance_serialPort.WriteLine("Z");
                    now_weigh_buzai.StartTime = DateTime.Now;
                    now_weighing = true;
                    return;
                }
            }

            //総重量をプラスにする
            if (AppSettings.CheckGrossWeight) {
                gross_weight.Amount *= -1;
                gross_weight.ResultAmount *= -1;
            }



            //手配合結果をセット
            recipe.ManualMixResult = mix_order_dic;

            //KSF使用の場合、グローブボックスの中でそのまま手攪拌する
            if (need_groveBox) {
                //20230405 風速計の値をモニター開始
                //20230322
                //KSFなら風速計モニタースタート
                //風速計タイマー開始 20230322
                kanomax_realtime_value.Start();
                fusoku_label.Visible = true;
                fusoku_value_textBox.Visible = true;

                //非同期処理開始 20230125
                //fusoku_call();

                await Do_HandMix();
            }

            try {
                //ログファイルに記載
                status_label.Text = "ログファイルに書き込み";
                status_label.Update();
                SaveLog();

                //PMMS連携の為、データベース処理
                status_label.Text = "データベースに書き込み";
                status_label.Update();
                Save_PMMS_DataBase();

                //カップラベル印刷
                status_label.Text = "カップラベル印刷";
                status_label.Update();
                while (true) {
                    try {
                        BrotherLabelPrint.PrintResinCupLabel(AppSettings.LabelFormatFilePath, recipe, LabelPrinterName);
                        break;
                    }
                    catch (Exception ex) {
                        string msg = $"{ex.Message}\n";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        msg = "リトライしますか？\n\n基本的には[はい]を選択して下さい。\n";
                        msg += "[いいえ]を選択した場合は、ラベル再印刷ボタンで再印刷して下さい。";
                        var res = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.No) {
                            MessageBox.Show($"ラベル再印刷して樹脂カップに貼り付けて下さい。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }

                    }
                }
                status_label.Text = "樹脂配合完了";
                status_label.Update();

                //グローブボックスの場合は排気をONにしておくように指示する
                if (is_groveBox) {
                    string msg = "樹脂カップを取り出して下さい。\n\nその後、ドアを閉めて下さい。\n\nその後、排気をONにして下さい。";
                    if (need_groveBox) {
                        msg = "樹脂カップを取り出して下さい。\n\nその後、ドアを閉めて下さい。\n\n排気はONのままにしておいて下さい。";
                    }
                    MessageBox.Show(msg, "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //レシピを移動
                status_label.Text = "レシピ移動";
                status_label.Update();
                while (true) {
                    try {
                        //フォルダがなければ作る
                        if (!Directory.Exists(AppSettings.AlreadyMadeRecipeFolderPath)) {
                            Directory.CreateDirectory(AppSettings.AlreadyMadeRecipeFolderPath);
                        }

                        string dest = $@"{AppSettings.AlreadyMadeRecipeFolderPath}\{recipe.FileName}";

                        File.Move($@"{AppSettings.RecipeFolderPath}\{recipe.FileName}", dest);
                        break;
                    }
                    catch (Exception ex) {
                        string msg = $"{ex.Message}\nこのままの状態でネットワークに異常がないか確認して下さい。\n" +
                                   $"ネットワーク異常ではない場合は管理者に連絡して下さい。";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        msg = "リトライしますか？\n\n基本的には[はい]を選択して下さい。\n";
                        msg += "[いいえ]を選択すると使用したレシピが移動されないため、問題があります。";
                        var res = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.No) {
                            MessageBox.Show($"管理者に手動でレシピを移動してもらって下さい。\n下記ファイル名を伝えて下さい\n\nファイル名：{recipe.FileName}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }
                    }
                }

                status_label.Text = "complete!";
                status_label.Update();
                MessageBox.Show("complete!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Exit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally {
                FormClose();
            }
        }

        /// <summary>
        /// PMMS連携の為、データベースに書き込む
        /// </summary>
        public void Save_PMMS_DataBase() {

            var now = DateTime.Now;

            var cup_tran = new ResinPrg.TnPMMSTran_CEJ() {
                cupno = recipe.CupNo,
                productnm = recipe.ProductName,
                ledrank = recipe.WavelengthRank,
                kubun = recipe.FlowMode,
                result = "0",
                mixtypecd = recipe.MixTypeCode,
                seikeiki = "",
                macno = AppSettings.BalancePlantcd,
                henkaten = recipe.Henkaten,
                recipefilenm = recipe.FileName,
                bdqty = recipe.LF_Num,
                dtend = now,
                Employee = employee_code,
                initialdtend = now,
                resingroupcd = "NA",
                lotno = string.Join("/", recipe.LotNoList)
            };

            //樹脂カップ履歴を書き込む
            string errmsg = "";
            while (true) {
                try {
                    bool success = ResinPrg.PMMS.PMMSInsert(cup_tran, ref errmsg);

                    if (success) {
                        break;
                    }
                    else {
                        string msg = $"データベースに樹脂カップ履歴が保存できませんでした。\nこのままの状態でネットワークに異常がないか確認して下さい。\n" +
                                    $"ネットワーク異常ではない場合は管理者に連絡して下さい。\n\nPMMS : {errmsg}";
                        throw new Exception(msg);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    string msg = "リトライしますか？\n\n基本的には[はい]を選択して下さい。\n";
                    msg += "[いいえ]を選択すると履歴が残らないため、この樹脂カップは捨てることになります。";
                    var res = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No) {
                        throw new Exception("終了します。樹脂カップは捨てて下さい。");
                    }
                }
            }
        }

        /// <summary>
        /// 配合結果ログファイルに書き込む
        /// </summary>
        public void SaveLog() {
            while (true) {
                try {
                    WriteResult(recipe, employee_code);
                    break;
                }
                catch (Exception ex) {
                    string msg = $"{ex.Message}\nこのままの状態でネットワークに異常がないか確認して下さい。\n" +
                                    $"ネットワーク異常ではない場合は管理者に連絡して下さい。";
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    msg = "リトライしますか？\n\n基本的には[はい]を選択して下さい。\n";
                    msg += "[いいえ]を選択すると履歴が残らないため、この樹脂カップは捨てることになります。";
                    var res = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No) {
                        throw new Exception("終了します。樹脂カップは捨てて下さい。");
                    }
                }
            }
        }

        /// <summary>
        /// グローブボックスの中で手攪拌する
        /// <para>その後、排気時間待つ</para>
        /// </summary>
        public async Task Do_HandMix() {
            MessageBox.Show("KFS蛍光体使用しているので、安全のためグローブボックスの中で手攪拌する必要があります。\nOKボタンを押してから開始して下さい。");
            status_label.Text = "手攪拌中...";

            //手攪拌時間表示
            hand_mix_time_label.Size = new Size(600, 200);
            hand_mix_time_label.Location = new Point(this.Width / 2 - hand_mix_time_label.Width / 2, this.Height / 2 - hand_mix_time_label.Height / 2);
            hand_mix_time_label.Visible = true;

            int remain_time = AppSettings.ManualMixSecond;

            while (remain_time > 0) {
                hand_mix_time_label.Text = $"ドアを開けないで手攪拌して下さい。\n\n手攪拌残り時間  {remain_time.ToString()} 秒";
                await Task.Delay(1000);

                //ドアが閉まっている場合のみ残り時間を減らす
                if (door_open == false) {
                    remain_time -= 1;
                }
            }


            MessageBox.Show("排気ONボタンを押して排気を開始して下さい。\n\nその後、OKボタンを押して下さい。");
            status_label.Text = "排気中...";

            //局所排気を指定時間実施しなければならない
            remain_time = AppSettings.ExhaustSecond;
            while (remain_time > 0) {
                hand_mix_time_label.Text = $"排気中...\n\n残り時間  {remain_time.ToString()} 秒";
                await Task.Delay(1000);

                //ドアが閉まっている場合のみ残り時間を減らす
                //かつ風速計の値が3.0m/s以上であれば時間を減らす 20230125
                if (door_open == false && Double.Parse(fusoku_value_textBox.Text) >= 3) {
                    remain_time -= 1;
                    kanomaxNG_label.Visible = false;
                }
                else if(Double.Parse(fusoku_value_textBox.Text) < 3)
                    kanomaxNG_label.Visible = true;
            }

            //局所排気終了後
            hand_mix_time_label.Visible = false;

            //20230322
            fusoku_label.Visible = false;
            fusoku_value_textBox.Visible = false;
            //風速計の値取得のタイマーを止める
            kanomax_realtime_value.Stop();

        }

        /// <summary>
        /// 非同期処理テスト用
        /// <para></para>
        /// </summary>
        public async Task Do_HandMixTest()
        {
            MessageBox.Show("KFS蛍光体使用しているので、安全のためグローブボックスの中で手攪拌する必要があります。\nOKボタンを押してから開始して下さい。");
            status_label.Text = "手攪拌中...";

            //手攪拌時間表示
            hand_mix_time_label.Size = new Size(600, 200);
            hand_mix_time_label.Location = new Point(this.Width / 2 - hand_mix_time_label.Width / 2, this.Height / 2 - hand_mix_time_label.Height / 2);
            hand_mix_time_label.Visible = true;

            int remain_time = 5;//AppSettings.ManualMixSecond;

            while (remain_time > 0)
            {
                hand_mix_time_label.Text = $"ドアを開けないで手攪拌して下さい。\n\n手攪拌残り時間  {remain_time.ToString()} 秒";
                await Task.Delay(1000);

                //ドアが閉まっている場合のみ残り時間を減らす

                    remain_time -= 1;
                
            }


            MessageBox.Show("排気ONボタンを押して排気を開始して下さい。\n\nその後、OKボタンを押して下さい。");
            status_label.Text = "排気中...";

            //局所排気を指定時間実施しなければならない
            remain_time = AppSettings.ExhaustSecond;
            while (remain_time > 0)
            {
                hand_mix_time_label.Text = $"排気中...\n\n残り時間  {remain_time.ToString()} 秒";
                await Task.Delay(1000);

                //ドアが閉まっている場合のみ残り時間を減らす
                //かつ風速計の値が1.0m/s以上であれば時間を減らす 20230125
                if (Double.Parse(fusoku_value_textBox.Text) >= 1)
                {
                    remain_time -= 1;
                    kanomaxNG_label.Visible = false;
                }
                else
                    kanomaxNG_label.Visible = true;
            }

            //局所排気終了後
            hand_mix_time_label.Visible = false;

            //20230322
            fusoku_label.Visible = false;
            fusoku_value_textBox.Visible = false;
            //風速計の値取得のタイマーを止める
            kanomax_realtime_value.Stop();

        }

        /// <summary>
        /// Exitボタン処理
        /// </summary>
        private void end_button_MouseDown(object sender, MouseEventArgs e) {
            var result = MessageBox.Show("調合を終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) {
                return;
            }
            else {
                FormClose();
            }
        }

        /// <summary>
        /// status labelの点滅処理タイマー(おまけ機能)
        /// </summary>
        private void status_label_timer_Tick(object sender, EventArgs e) {
            if (now_weighing == false) {

                if (status_label.BackColor == SystemColors.Control) {
                    status_label.BackColor = Color.LightGreen;
                }
                else {
                    status_label.BackColor = SystemColors.Control;
                }
            }
        }

        /// <summary>
        /// 計量データクラス
        /// </summary>
        public class WeighingData {

            /// <summary>
            /// 計量時間
            /// </summary>
            public DateTime Time { get; set; }

            /// <summary>
            /// 計量値
            /// </summary>
            public decimal Value { get; set; }

        }
        /// <summary>
        /// 風速計の値を定期的に取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kanomax_realtime_value_Tick(object sender, EventArgs e)
        {
            if (!kanomax_fusoku_serialPort.IsOpen)
            {
                try
                {
                    //シリアルポートオープン
                    kanomax_fusoku_serialPort.PortName = AppSettings.kanomaxSerialPortName;
                    kanomax_fusoku_serialPort.NewLine = "\r";
                    kanomax_fusoku_serialPort.Open();
                    kanomax_fusoku_serialPort.DiscardInBuffer();
                    kanomax_fusoku_serialPort.ReadTimeout = 500;
                    kanomax_fusoku_serialPort.WriteTimeout = 500;
                }
                catch(TimeoutException)
                {
                    //MessageBox.Show("風速計のCOMポートを確認してください.");
                    return;
                }
                
            }
            string r_msg;

            try 
            {
                kanomax_fusoku_serialPort.DiscardInBuffer();
                kanomax_fusoku_serialPort.NewLine = "\r";
                kanomax_fusoku_serialPort.Write("DG,01" + "\r");
                r_msg = kanomax_fusoku_serialPort.ReadLine();
            }
            catch(TimeoutException)
            {
                //MessageBox.Show("風速計のCOMポートを確認してください.");
                return;
            }


            
            //kanomax_serialPort.ReadLine();

            string[] arr = r_msg.Split(',');
            string kanomax_value = arr[2].Substring(0, 2) + "." + arr[2].Substring(2, 2);
            //頭が0なら、小数点調整で頭1文字削除
            if (kanomax_value.Substring(0, 1) == "0")
            {
                kanomax_value = kanomax_value.Substring(1);
            }

            fusoku_value_textBox.Text = kanomax_value;
        }

        /// <summary>
        /// 風速計データ受信時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kanomax_fusoku_serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

        public async void fusoku_call()
        {
            await Do_HandMix();
        }
    }
}
