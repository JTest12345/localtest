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

using static Dage_Collection.OperateFile;
using static Dage_Collection.Statistics;


namespace Dage_Collection {
    public partial class DataChangeForm : Form {

        private MainForm fm;

        //コンストラクター
        public DataChangeForm(MainForm form) {
            InitializeComponent();
            fm = form;
        }

        //フォームロードイベント
        private void DataChangeForm_Load(object sender, EventArgs e) {
            //「その他」の理由を書くテキストボックス
            TextBox1.Enabled = false;
            TextBox1.Text = "";

            //何行目のデータか入力するテキストボックス
            Row_Num.Text = "";

            //破壊モード入力テキストボックス
            Destruction_Mode.Enabled = false;
            Destruction_Mode.Text = "";

            //修正後のコメントテキストボックス
            TextBox2.Enabled = false;
            TextBox2.Text = "";

            //初期選択させる
            Reason_ListBox.SelectedIndex = 0;
            Change_ListBox.SelectedIndex = 0;
        }

        //フォームの×ボタンなどで閉じられなくする
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
        /// 戻るボタン処理
        /// </summary>
        private void Back_Button_Click(object sender, EventArgs e) {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// // 修正理由選択時処理
        /// </summary>
        private void Reason_ListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Reason_ListBox.Text == "その他") {
                TextBox1.Enabled = true;
            }
            else {
                TextBox1.Enabled = false;
                TextBox1.Text = "";
            }
        }

        /// <summary>
        /// 修正内容選択時処理
        /// </summary>
        private void Change_ListBox_SelectedValueChanged(object sender, EventArgs e) {

            switch (Change_ListBox.Text) {

                case "データを消したい": {
                        Destruction_Mode.Enabled = false;
                        Destruction_Mode.Text = "";
                        TextBox2.Enabled = false;
                        TextBox2.Text = "";
                        break;
                    }

                case "破壊モードを修正したい": {
                        Destruction_Mode.Enabled = true;
                        TextBox2.Enabled = false;
                        TextBox2.Text = "";
                        break;
                    }

                case "コメントを修正したい": {
                        Destruction_Mode.Enabled = false;
                        Destruction_Mode.Text = "";
                        TextBox2.Enabled = true;
                        break;
                    }
            }
        }


        /// <summary>
        /// 変更実施ボタン処理
        /// </summary>
        private void Change_Go_Click(object sender, EventArgs e) {

            string reason;

            //修正理由を取得する
            if (Reason_ListBox.Text == "その他") {
                reason = TextBox1.Text;
                if (reason == "") {
                    MessageBox.Show("理由を入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else {
                reason = Reason_ListBox.Text;
            }

            //修正する行を取得する
            int change_row;
            try {
                change_row = int.Parse(Row_Num.Text) - 1; // 1～30　⇒　0～29

                if (change_row > 29 || change_row < 0) {
                    MessageBox.Show("行数は1～30で入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            catch {
                MessageBox.Show("行数は数字で入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //修正する行にはデータがあるか調べる
            if (fm.DataGridView1.Rows[change_row].Cells[0].Value == null) {
                MessageBox.Show("指定の行にはデータがありません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //修正前に元データ取得
            string old_data = Get_Data(change_row);

            int log_mode = 0;

            //修正実施処理
            switch (Change_ListBox.Text) {

                case "データを消したい": {
                        log_mode = 0;

                        //データクリア
                        for (int n = 0; n < 3; n++)
                            fm.DataGridView1.Rows[change_row].Cells[n].Value = null;

                        //平均とか再計算+グラフ再表示
                        Recalculation();
                        break;
                    }

                case "破壊モードを修正したい": {
                        log_mode = 1;
                        int mode;

                        //破壊モードの数字チェック
                        try {
                            mode = int.Parse(Destruction_Mode.Text);
                            if (mode > 9 || mode < 0) {
                                MessageBox.Show("破壊モードは0～9の数字で入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                        catch {
                            MessageBox.Show("破壊モードは数字で入力して下さい。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        //破壊モード修正
                        fm.DataGridView1.Rows[change_row].Cells[1].Value = mode;
                        break;
                    }

                case "コメントを修正したい": {
                        log_mode = 2;

                        //コメント修正
                        fm.DataGridView1.Rows[change_row].Cells[2].Value = TextBox2.Text;
                        break;
                    }
            }

            // Logファイルに書き込み
            Change_Log_Write(change_row, log_mode, old_data, reason);

            // このフォームを閉じる＋破棄
            this.Close();
            this.Dispose();
        }



        /// <summary>
        /// DataGridViewデータ取得して書き込みテキストを返す関数
        /// </summary>
        private string Get_Data(int row) {
            string text = "";
            //MainFormのデータ取得
            for (int n = 0; n < 3; n++) {
                text += fm.DataGridView1.Rows[row].Cells[n].Value.ToString();
                text += ",";
            }
            return text;
        }

        /// <summary>
        /// 修正内容Log書き込み
        /// </summary>
        private void Change_Log_Write(int row, int mode, string old_data, string reason) {

            //日時
            string textline = DateTime.Now.ToString() + ",";
            //機種,LotNo,測定箇所
            textline += $"{fm.Product_label.Text},{fm.LotNo_label.Text},{fm.Chip_Place_label.Text},";
            //修正前データ
            textline += old_data;

            textline += "→,";

            switch (mode) {
                case 0: {
                        textline += " , , ,データ消去,";
                        break;
                    }

                case 1: {
                        textline += Get_Data(row);
                        textline += "破壊モード変更,";
                        break;
                    }

                case 2: {
                        textline += Get_Data(row);
                        textline += "コメント変更,";
                        break;
                    }
            }

            textline += reason;
            textline += "\n";

            //データ修正履歴ファイルに書き込み
            ChangeLogSave($"{fm.Result_Folder}/Change_Log.csv", textline);
        }

        /// <summary>
        /// 平均とか再計算+グラフ再表示処理
        /// </summary>
        private void Recalculation() {

            List<string> data_list = fm.Get_DGVdata();

            //平均値再計算
            double ave = GetAverage(data_list);
            fm.Average_label.Text = ave.ToString("0.00");

            //最大値再計算
            double max = GetMax(data_list);
            fm.Max_label.Text = max.ToString("0.00");

            //最小値再計算
            double min = GetMin(data_list);
            fm.Min_label.Text = min.ToString("0.00");

            //標準偏差再計算
            fm.Sigma_label.Text = GetSigma(data_list).ToString("0.00");

            //レンジ再計算
            double range = (max - min);
            fm.Range_label.Text = range.ToString("0.00");

            //グラフ更新
            if (fm.Chart1.Series[3].Points.Count > 0) {

                //グラフの最後のポイントを再計算後の値にする
                fm.Chart1.Series[3].Points[fm.Chart1.Series[3].Points.Count - 1].YValues[0] = ave;
                fm.Chart1.ChartAreas[0].RecalculateAxesScale();
                fm.Chart1.Invalidate(); // グラフ再描画
            }

            if (fm.Chart2.Series[3].Points.Count > 0) {

                //グラフの最後のポイントを再計算後の値にする
                fm.Chart2.Series[3].Points[fm.Chart2.Series[3].Points.Count - 1].YValues[0] = range;
                fm.Chart1.ChartAreas[0].RecalculateAxesScale();
                fm.Chart2.Invalidate(); // グラフ再描画
            };

            //管理値データ更新させる
            fm.RadioButton_ini();
            fm.Check_Standard();
        }
    }
}
