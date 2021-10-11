using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace Oskas
{
    public partial class fmMain : Form
    {
        oskaini oskaini = new oskaini();
        // mqttインスタンス
        MqttClient mqttClient;
        OskasMqttClient mq = new OskasMqttClient();

        // 改行コード
        static string crlf = "\r\n";
        // サーバー稼働許可
        bool serverOn = false;
        // 参照渡し用メッセージ変数
        string msg;
        string globalmsg;
        // Static参照用
        static fmMain fmm;
        // コンソールクリアフラグ
        bool clearConsoleMessNext = false;
        bool clearDebugConsoleMessNext = false;

        public fmMain()
        {
            InitializeComponent();
            fmm = this;
            TaskTimer.Enabled = false;
            toolStripStatusLabel1.Text = string.Format("サーバーは停止中です");

            // oska.iniを読み込み
            if (!oskaini.GetOskasIniValues(ref globalmsg))
            {
                btServerStart.Enabled = false;
                ConsoleShow(globalmsg, Cnslcnf.msg_error);
            }

            if (oskaini.isUseMqtt)
            {
                mqttClient = new MqttClient(oskaini.mosquittoHost);
                var ret = mqttClient.Connect(Guid.NewGuid().ToString());
            }

            // NlogにConsoleActionを渡す
            OskNLog.InitConsoleAction(consoleAction);
            // NLogのパス設定
            // OskNLog.setFolderName(); ⇒ 継承先で設定する
        }


        int mesLen = new int(); //コンソールメッセージ数初期化
        public delegate void ConsoleDelegate(string text, int level);
        public virtual void ConsoleShow(string text, int level)
        {
            if (consoleBox.InvokeRequired)
            {
                ConsoleDelegate d = new ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] { text, level });
            }
            else
            {
                if (mesLen == 0)
                {
                    consoleBox.Clear();
                    mesLen = 0;
                }
                mesLen += 1;
                string message = "";
                DateTime today = DateTime.Today;
                DateTime now = DateTime.Now;
                string dt = now.ToString("yyyy-MM-dd HH:mm:ss] ");
                switch (level)
                {
                    case Cnslcnf.msg_info:
                        message = "[INFO: " + dt;
                        break;
                    case Cnslcnf.msg_detect:
                        message = "[DETECT: " + dt;
                        break;
                    case Cnslcnf.msg_task:
                        message = "[TASK: " + dt;
                        break;
                    case Cnslcnf.msg_alarm:
                        message = "[ALARM: " + dt;
                        ErrorLogComsole.AppendText(message + text + crlf);
                        break;
                    case Cnslcnf.msg_error:
                        message = "[ERROR: " + dt;
                        ErrorLogComsole.AppendText(message + text + crlf);
                        break;
                    case Cnslcnf.msg_debug:
                        if (oskaini.DebugMode)
                        {
                            message = "[DEBUG: " + dt + crlf;
                            consoleBox.AppendText(message + text + crlf);
                            if (oskaini.isUseMqtt)
                            {
                                mq.Mqtt_Publog(mqttClient, "test", message + text + crlf);
                            }
                        }
                        break;
                }
                if (level != Cnslcnf.msg_debug)
                {
                    consoleBox.AppendText(message + text + crlf);

                    if (oskaini.isUseMqtt)
                    {
                        mq.Mqtt_Publog(mqttClient, "test", message + text + crlf);
                    }
                }


                ///////////////////////////////////////////////////////////////////////////////////////
                // メッセージ件数が規定値を超えた場合、TextBox(consoleBox)をクリアする
                ///////////////////////////////////////////////////////////////////////////////////////
                if (mesLen > Cnslcnf.mesMaxLen)
                {
                    consoleBox.AppendText($"■■■メッセージ件数が{Cnslcnf.mesMaxLen}件を超えました。次のメッセージで表示ログはクリアされます。■■■" + crlf);
                    mesLen = 0;
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // 日付が変わった場合、ログファイル名を現在時刻に再設定する
                ///////////////////////////////////////////////////////////////////////////////////////
                if (now.Date > oskaini.SrvStatDT.Date)
                {
                    oskaini.MsglogPath = oskaini.MsglogDir + @"\" + now.ToString("yyyyMMddHHmmss") + ".txt";
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // ログ書き出し
                ///////////////////////////////////////////////////////////////////////////////////////
                if (serverOn && !AppendMsgLog(oskaini.MsglogPath, message + dt + text))
                {
                    // ここの処理は要検討！
                    MessageBox.Show("ログの書き出しができません。");
                }
            }
        }

        // Action for Nlog Console
        static Action<string, int> consoleAction = new Action<string, int>(_ConsoleShow);
        public static void _ConsoleShow(string text, int level)
        {
            if (fmm == null) return;

            if (fmm.consoleBox.InvokeRequired)
            {
                fmm.Invoke(consoleAction, new object[] { text, level });
            }
            else
            {
                if (fmm.clearConsoleMessNext)
                {
                    fmm.consoleBox.Clear();
                    fmm.clearConsoleMessNext = false;
                }
                if (fmm.clearDebugConsoleMessNext)
                {
                    fmm.ErrorLogComsole.Clear();
                    fmm.clearDebugConsoleMessNext = false;
                }

                string message = "";
                DateTime today = DateTime.Today;
                DateTime now = DateTime.Now;
                string dt = now.ToString("yyyy-MM-dd HH:mm:ss] ");
                switch (level)
                {
                    case Cnslcnf.msg_debug:
                        if (fmm.oskaini.DebugMode)
                        {
                            message = "[DEBUG: " + dt + crlf;
                            fmm.consoleBox.AppendText(message + text + crlf);
                            if (fmm.oskaini.isUseMqtt)
                            {
                                fmm.mq.Mqtt_Publog(fmm.mqttClient, "test", message + text + crlf);
                            }
                        }
                        break;
                    case Cnslcnf.msg_info:
                        message = "[INFO: " + dt;
                        break;
                    case Cnslcnf.msg_warn:
                        message = "[WARN: " + dt;
                        fmm.ErrorLogComsole.AppendText(message + text + crlf);
                        break;
                    case Cnslcnf.msg_error:
                        message = "[ERROR: " + dt;
                        fmm.ErrorLogComsole.AppendText(message + text + crlf);
                        break;
                    case Cnslcnf.msg_fatal:
                        message = "[FATAL: " + dt;
                        fmm.ErrorLogComsole.AppendText(message + text + crlf);
                        break;
                }
                if (level != Cnslcnf.msg_debug)
                {
                    fmm.consoleBox.AppendText(message + text + crlf);

                    if (fmm.oskaini.isUseMqtt)
                    {
                        fmm.mq.Mqtt_Publog(fmm.mqttClient, "test", message + text + crlf);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // メッセージ件数が規定値を超えた場合、TextBox(consoleBox)をクリアする
                ///////////////////////////////////////////////////////////////////////////////////////
                if (fmm.consoleBox.Lines.Length > Cnslcnf.mesMaxLen)
                {
                    fmm.consoleBox.AppendText($"■■■メッセージ件数が{Cnslcnf.mesMaxLen}件を超えました。次のメッセージで表示ログはクリアされます。■■■" + crlf);
                    fmm.clearConsoleMessNext = true;
                }
                if (fmm.ErrorLogComsole.Lines.Length > Cnslcnf.mesMaxLen)
                {
                    fmm.ErrorLogComsole.AppendText($"■■■メッセージ件数が{Cnslcnf.mesMaxLen}件を超えました。次のメッセージで表示ログはクリアされます。■■■" + crlf);
                    fmm.clearDebugConsoleMessNext = true;
                }

            }
        }

        public bool MakeMsgLog(string FilePath)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            try
            {
                StreamWriter writer = new StreamWriter(FilePath, false, enc);
                writer.Close();
            }
            catch
            {
                return false;
            }

            if (!File.Exists(FilePath))
            {
                return false;
            }

            return true;
        }


        public bool AppendMsgLog(string FilePath, string msg)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            File.AppendAllText(FilePath, msg + crlf);

            if (!File.Exists(FilePath))
            {
                return false;
            }

            return true;
        }

        private void fmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oskaini.isUseMqtt && mqttClient.IsConnected)
            {
                mqttClient.Disconnect();
            }
        }

        private void bt_ClearErrLogs_Click(object sender, EventArgs e)
        {
            ErrorLogComsole.Clear();
        }

    }

}
