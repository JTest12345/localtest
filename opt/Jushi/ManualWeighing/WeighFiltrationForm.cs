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

using static ManualWeighing.Program;
using ResinClassLibrary;
using ResinPrg;//Junki Nakamura DLL

namespace ManualWeighing {

    /// <summary>
    /// ろ過後のダム剤に対しての配合に使用する
    /// <para>ろ過後の重量に対して配合量が決まる</para>
    /// </summary>
    public partial class WeighFiltrationForm : Form {

        public const string BeforeFiltrationCupWeight = "BeforeFiltration_CupWeight";

        /// <summary>
        /// 作業者
        /// </summary>
        private string employee_code;

        /// <summary>
        /// ろ過後かどうか
        /// <para>true = ろ過後</para>
        /// </summary>
        private bool after_filtration;

        /// <summary>
        /// 計量するレシピ
        /// </summary>
        private AfterFiltrationRecipe recipe;

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
        /// 現在計量中かどうか
        /// <para>true = 現在計量中</para>
        /// </summary>
        private bool now_weighing = false;

        /// <summary>
        /// ろ過後のダム剤重量
        /// </summary>
        private decimal after_filtration_resin_weight;

        /// <summary>
        /// 作製している場所がグローブボックスかどうか
        /// <para>true = グローブボックス</para>
        /// </summary>
        private bool is_groveBox = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WeighFiltrationForm(AfterFiltrationRecipe rec, string empcd, bool isGroveBox) {
            InitializeComponent();

            recipe = rec;
            employee_code = empcd;
            is_groveBox = isGroveBox;

            electronicBalance_serialPort.PortName = AppSettings.BalanceSerialPortName;

            if (recipe.BeforeCupWeight == null) {

                after_filtration = false;

                //20230220 ダムの場合、場合分けしたが、2回目調合時はこの中に入らない
                var before = new RecipeBuzai
                {
                    Name = BeforeFiltrationCupWeight,
                    Amount = 12,
                    LowerAllowableErrorGram = 5,
                    UpperAllowableErrorGram = 5
                };

                //ダム剤は専用カップで56g
                if (rec.MoldType.Contains("SDR"))
                {
                    before = new RecipeBuzai
                    {
                        Name = BeforeFiltrationCupWeight,
                        Amount = 56,
                        LowerAllowableErrorGram = 5,
                        UpperAllowableErrorGram = 5
                    };
                }
                else
                {
                    before = new RecipeBuzai
                    {
                        Name = BeforeFiltrationCupWeight,
                        Amount = 12,
                        LowerAllowableErrorGram = 5,
                        UpperAllowableErrorGram = 5
                    };
                }
                //ここまで追加

                mix_order_dic = new Dictionary<int, RecipeBuzai> { { 0, before } };
            }
            else {
                
                after_filtration = true;

                var after = new RecipeBuzai {
                    Name = "AfterFiltration_CupWeight",
                    Amount = 100,
                    LowerAllowableErrorGram = 100,
                    UpperAllowableErrorGram = 1000
                };

                //配合順番辞書設定
                mix_order_dic = new Dictionary<int, RecipeBuzai> { { 0, after } };
                int i = 1;
                foreach (var b in recipe.UseBuzai.Where(x => x.UseAutoMachine == false).OrderBy(x => x.MixOrder)) {
                    mix_order_dic.Add(i, b);
                    i += 1;
                }

                before_cup_weight_label.Text = recipe.BeforeCupWeight.ResultAmount.ToString();
                target_value_textBox.BackColor = Color.Black;
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
        private void WeighFiltrationForm_Load(object sender, EventArgs e) {

            //自分自身のフォームを最大化
            this.WindowState = FormWindowState.Maximized;

            try {
                recipe_info_label.Text = $"Cup No：{recipe.CupNo}\n機種：{recipe.ProductName}\n" +
                    $"LotNo：{string.Join("/", recipe.LotNoList)}\n作業者：{employee_code}";

                Initialize_buzaiDGV();

                //シリアルポートオープン
                electronicBalance_serialPort.NewLine = "\r\n";
                electronicBalance_serialPort.Open();
                electronicBalance_serialPort.DiscardInBuffer();
                //ゼロリセットコマンド送信
                electronicBalance_serialPort.WriteLine("Z");


                //最初にカップを計量する
                target_value_textBox.Text = now_weigh_buzai.Amount.ToString();
                plus_allow_label.Text = $"＋{now_weigh_buzai.UpperAllowableErrorGram.ToString("f3")}";
                minus_allow_label.Text = $"－{now_weigh_buzai.LowerAllowableErrorGram.ToString("f3")}";

                if (after_filtration == false) {
                    status_label.Text = "ラベルを貼ってから、空カップを置いてください。\n安定したらNextボタンを押して下さい。";
                }
                else {
                    status_label.Text = "ろ過後樹脂が入ったカップを置いてください。\n安定したらNextボタンを押して下さい。";

                }
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

            this.Close();
        }

        /// <summary>
        /// 使用部材データグリッドビュー初期化
        /// </summary>
        private void Initialize_buzaiDGV() {
            foreach (var rec in recipe.UseBuzai) {
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
                if (array.Length == 10) {
                    //4Mシステムの部材ラベル
                    fcode = array[0];
                    lotno = array[3];
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

                //部材のFコードが一致していたら計量開始可能
                if (fcode == now_weigh_buzai.Fcode) {
                    status_label.Text = "計量開始して下さい。";
                    input_buzaiQR_textBox.Enabled = false;
                    buzai_fcode_textBox.BackColor = Color.LimeGreen;
                    electronicBalance_serialPort.WriteLine("Z");
                    now_weighing = true;
                    now_weigh_buzai.StartTime = DateTime.Now;
                    now_weigh_buzai.LotNo = lotno;
                    target_value_textBox.BackColor = SystemColors.Control;
                    result_value_textBox.BackColor = SystemColors.Control;
                }
                else {
                    status_label.Text = "部材が違います。";
                    buzai_fcode_textBox.BackColor = Color.LightPink;
                }

                input_buzaiQR_textBox.Text = "";
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

            //初期化
            now_weighing = false;
            weigh_ok = false;
            next_button.Visible = false;

            //計量結果取得
            now_weigh_buzai.ResultAmount = decimal.Parse(result_value_textBox.Text);
            now_weigh_buzai.EndTime = DateTime.Now;

            //空カップ計量の場合(ろ過前)
            if (after_filtration == false) {
                recipe.BeforeCupWeight = new AfterFiltrationRecipe.BeforeCupWeighing {
                    ResultAmount = now_weigh_buzai.ResultAmount,
                    StartTime = now_weigh_buzai.StartTime,
                    EndTime = now_weigh_buzai.EndTime
                };

                //空カップ計量結果を加えてレシピ上書き
                Recipe.CreateRecipe_for_HaigoSoft(recipe, $@"{AppSettings.RecipeFolderPath}\{recipe.FileName}");

                //終了する
                MessageBox.Show("complete!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormClose();
                return;
            }

            //計量結果記載(ろ過後)
            if (now_weigh_order == 0) {
                var after_cup_weight = now_weigh_buzai.ResultAmount;
                after_cup_weight_label.Text = after_cup_weight.ToString();
                after_filtration_resin_weight = (decimal)after_cup_weight - (decimal)recipe.BeforeCupWeight.ResultAmount;
            }
            else {
                //ろ過後にはDM-30しか配合は無いはずなので、基本1個
                for (int i = 0; i < use_buzai_dataGridView.Rows.Count; i++) {
                    if (use_buzai_dataGridView.Rows[i].Cells[1].Value.ToString() == now_weigh_buzai.Name) {
                        use_buzai_dataGridView.Rows[i].Cells[0].Value = "✔";
                        use_buzai_dataGridView.Rows[i].Cells[3].Value = now_weigh_buzai.LotNo;
                        use_buzai_dataGridView.Rows[i].Cells[4].Value = now_weigh_buzai.Amount;
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

                buzai_name_textBox.Text = buzai.Name;
                buzai_fcode_textBox.Text = buzai.Fcode;
                buzai_lotno_textBox.Text = buzai.LotNo;
                buzai_fcode_textBox.BackColor = SystemColors.Control;
                buzai_lotno_textBox.BackColor = SystemColors.Control;

                
                //配合量を計算(ろ過後樹脂量の4.33%)⇒四捨五入で小数点3桁にする
                //現在はDM-30 1種類のパターンしかない
                buzai.Amount = after_filtration_resin_weight * (decimal)now_weigh_buzai.PercentOfTotalWeight / 100.0m;
                buzai.Amount = Math.Round(buzai.Amount, 3, MidpointRounding.AwayFromZero);

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


            //手配合結果をセット
            recipe.ManualMixResult = mix_order_dic;

            //ログ用でろ過前カップ重量を追加しておく
            recipe.ManualMixResult.Add(-1, new RecipeBuzai {
                Name = BeforeFiltrationCupWeight,
                ResultAmount = (decimal)recipe.BeforeCupWeight.ResultAmount,
                StartTime = recipe.BeforeCupWeight.StartTime,
                EndTime = recipe.BeforeCupWeight.EndTime
            });

            try {
                //ログファイルに記載
                status_label.Text = "ログファイルに書き込み";
                status_label.Update();
                SaveLog();

                //PMMS連携の為、データベース処理
                status_label.Text = "データベースに書き込み";
                status_label.Update();              
                Save_PMMS_DataBase();

                //レシピを移動
                status_label.Text = "レシピ移動";
                status_label.Update();
                File.Move($@"{AppSettings.RecipeFolderPath}\{recipe.FileName}", $@"{AppSettings.AlreadyMadeRecipeFolderPath}\{recipe.FileName}");

                status_label.Text = "樹脂配合完了";
                status_label.Update();

                if (is_groveBox) {
                    string msg = "樹脂カップを取り出して下さい。\n\nその後、ドアを閉めて下さい。\n\nその後、排気をONにして下さい。";
                    MessageBox.Show(msg, "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

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
        /// PMMS連携の為、データベースに書き込む
        /// </summary>
        public void Save_PMMS_DataBase()
        {

            var now = DateTime.Now;

            var cup_tran = new ResinPrg.TnPMMSTran_CEJ()
            {
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
            while (true)
            {
                try
                {
                    bool success = ResinPrg.PMMS.PMMSInsert(cup_tran, ref errmsg);

                    if (success)
                    {
                        break;
                    }
                    else
                    {
                        string msg = $"データベースに樹脂カップ履歴が保存できませんでした。\nこのままの状態でネットワークに異常がないか確認して下さい。\n" +
                                    $"ネットワーク異常ではない場合は管理者に連絡して下さい。\n\nPMMS : {errmsg}";
                        throw new Exception(msg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    string msg = "リトライしますか？\n\n基本的には[はい]を選択して下さい。\n";
                    msg += "[いいえ]を選択すると履歴が残らないため、この樹脂カップは捨てることになります。";
                    var res = MessageBox.Show(msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No)
                    {
                        throw new Exception("終了します。樹脂カップは捨てて下さい。");
                    }
                }
            }
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


    }
}
