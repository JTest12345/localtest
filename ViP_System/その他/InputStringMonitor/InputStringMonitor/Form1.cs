using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InputStringMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
        }

        // WindowsAPIのインポート
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern short GetKeyState(int nVirtKey);

        //============
        // フォームがロード時に起動するイベントプロシージャ
        private void Form1Load(object sender, EventArgs e)
        {
            // コントロールの初期化            
            timer1.Interval = 10;
            timer1.Enabled = true;
            label1.Text = "";
            
        }


    //============
    // タイマーで定期的に起動するイベントプロシージャ
        private void timer1Tick(object sender, EventArgs e)
        {
            // 指定キー（Aキー）が押下されているのか確認
            bool KeyState = IsKeyLocked(System.Windows.Forms.Keys.D4);
            

            //label1.Text = label1.Text;

            // Aキーが押下の場合
            if (KeyState == true)
            {
                // Aキーが押下されていることをラベルに表示
                //label1.Text = "Enter押下状態";
                label1.Text = label1.Text + "4";
            }
            // Aキーが押下されていない場合
            else
            {
                //label1.Text = "---";
            }

            KeyState = IsKeyLocked(System.Windows.Forms.Keys.D9);
            if (KeyState == true)
            {
                // Aキーが押下されていることをラベルに表示
                //label1.Text = "Enter押下状態";
                label1.Text = label1.Text + "9";
            }

            KeyState = IsKeyLocked(System.Windows.Forms.Keys.A);
            if (KeyState == true)
            {
                // Aキーが押下されていることをラベルに表示
                //label1.Text = "Enter押下状態";
                label1.Text = label1.Text + "A";
            }
            KeyState = IsKeyLocked(System.Windows.Forms.Keys.S);
            if (KeyState == true)
            {
                // Aキーが押下されていることをラベルに表示
                //label1.Text = "Enter押下状態";
                label1.Text = label1.Text + "S";
            }

        }


        //============
        // 指定キー押下状態調査メソッド
        // 指定のキーが押下状態か調査するメソッドです。
        // 第１引数: 調査対象のキーを示すKeys列挙体
        // 戻り値: 判定結果 true:押下 / false:非押下
        public bool IsKeyLocked(System.Windows.Forms.Keys KeyValue)

        {
            // WindowsAPIで押下判定
            bool KeyState = (GetKeyState((int)KeyValue) & 0x80) != 0;
            return KeyState;
        }

        
    }
}
