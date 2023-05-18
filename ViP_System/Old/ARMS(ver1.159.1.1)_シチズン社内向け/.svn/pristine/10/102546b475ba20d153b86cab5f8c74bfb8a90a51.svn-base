using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmSelectMachineButtons : Form
    {
        #region 定数

        /// <summary> ボタンのサイズ(横) </summary>
        private const int BUTTON_SIZE_X = 110;
        /// <summary> ボタンのサイズ(縦) </summary>
        private const int BUTTON_SIZE_Y = 85;

        /// <summary> ボタンの初期位置(横) </summary>
        private const int BUTTON_LOCATION_INIT_X = 10;
        /// <summary> ボタンの初期位置(縦) </summary>
        private const int BUTTON_LOCATION_INIT_Y = 10;

        /// <summary> ボタンの余白(横) </summary>
        private const int BUTTON_MARGIN_X = 10;
        /// <summary> ボタンの余白(縦) </summary>
        private const int BUTTON_MARGIN_Y = 10;

        /// <summary> ボタンの個数 (1タブページ当たり) </summary>
        private const int BUTTON_COUNT = 14;
        /// <summary> ボタンの個数 (横) </summary>
        private const int BUTTON_COUNT_X = 7;
        /// <summary> ボタンの個数 (縦) </summary>
        private const int BUTTON_COUNT_Y = 2;

        #endregion

        #region プロパティ

        /// <summary> 返値(装置情報) </summary>
        public MachineInfo SelectedMacInfo;

        /// <summary> 前回選択したボタンのMacNo </summary>
        public int InitMacNo;

        /// <summary> 画面に表示する装置リスト </summary>
        public List<MachineInfoGroup> ViewMacInfoList;

        #endregion


        public FrmSelectMachineButtons() : this(0, new List<MachineInfoGroup>())
        {
        }

        public FrmSelectMachineButtons(int initMacNo, List<MachineInfoGroup> macInfoList)
        {
            InitializeComponent();
            this.ViewMacInfoList = macInfoList;
            this.InitMacNo = initMacNo;
        }

        private void FrmSelectMachineButtons_Load(object sender, EventArgs e)
        {
            foreach(MachineInfoGroup data in this.ViewMacInfoList)
            {
                createTabPage(data);
            }

            // 先頭のページ(デザインで作成済み)のページを削除
            this.tabCtrl.TabPages.RemoveAt(0);

            // 表示タブの選択
            bool selectedFg = false;
            for (int i = 0; i < tabCtrl.TabPages.Count; i++)
            {
                TabPage tp = tabCtrl.TabPages[i];
                foreach(Control ctrl in tp.Controls)
                {
                    if (ctrl.Tag == null) continue;

                    MachineInfo tagData = (MachineInfo)ctrl.Tag;
                    if (tagData.MacNo == InitMacNo)
                    {
                        tabCtrl.SelectedIndex = i;
                        ctrl.Select();

                        selectedFg = true;
                        break;
                    }
                }
                if (selectedFg == true) break;
            }
        }

        private void createTabPage(MachineInfoGroup dataClass)
        {
            TabPage tp;
            // 装置リスト分のループ (1ページ当たりのボタンずつカウントアップ)
            for (int i = 0; i < dataClass.MacList.Count(); i += BUTTON_COUNT)
            {
                // 新規タブページの作成
                tp = new TabPage();
                tp.Text = $"{dataClass.ClassNm}({dataClass.LineNo})";

                // ボタンを個数分、作成する
                Button[] buttons = new Button[BUTTON_COUNT];
                int x = 0;
                int y = 0;
                for (int j = 0; j < BUTTON_COUNT; j++)
                {
                    // 配置場所の右端に来たら、更新する (左端 + 一つ下)
                    if (x >= BUTTON_COUNT_X)
                    {
                        x = 0;
                        y++;
                    }

                    // ボタンの新規作成 (ここではサイズと位置だけ定義。中身はこの関数内の次のfor文で定義する)
                    buttons[j] = new Button();
                    buttons[j].Size = new Size(BUTTON_SIZE_X, BUTTON_SIZE_Y);
                    int pointX = BUTTON_LOCATION_INIT_X + (BUTTON_SIZE_X + BUTTON_MARGIN_X) * x;
                    int pointY = BUTTON_LOCATION_INIT_Y + (BUTTON_SIZE_Y + BUTTON_MARGIN_Y) * y;
                    buttons[j].Location = new Point(pointX, pointY);
                    tp.Controls.Add(buttons[j]);

                    // ボタン配置位置の更新
                    x++;
                }

                // 設置対象の装置情報配列を変数にコピー (リスト終わりの端数は切り取る)
                int destArrayLength = BUTTON_COUNT;
                if (dataClass.MacList.Count - i < destArrayLength)
                {
                    destArrayLength = dataClass.MacList.Count - i;
                }
                MachineInfo[] arrayData = new MachineInfo[destArrayLength];
                Array.Copy(dataClass.MacList.ToArray(), i, arrayData, 0, destArrayLength);

                // データ設置対象のボタンのループ
                for (int j = 0; j < arrayData.Length; j++)
                {
                    // ボタンに表記・内容・イベントを追加
                    buttons[j].Text = $"{arrayData[j].NascaPlantCd}\r\n{arrayData[j].MachineName}";
                    buttons[j].Font = new Font(buttons[j].Font.OriginalFontName, 14.25F);
                    buttons[j].Tag = arrayData[j];
                    buttons[j].Click += new EventHandler(btnClick);
                }

                this.tabCtrl.TabPages.Add(tp);
            }
        }

        private void btnClick(object sender, EventArgs e)
        {
            this.SelectedMacInfo = ((Button)sender).Tag as MachineInfo;
            this.Close();
        }
    }
}
