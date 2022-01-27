using Oskas;
using Oskas.Functions.Plcs;
using Ovens.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ovens
{

    public partial class FmOvens : fmMain
    {
        // Tree View Form
        FmOvnView _fmOvnView;

        // Hab Start/Stop
        public static bool habEnable { get; set; } = false;
        public Image blue = Oskas.Properties.Resources.button_blue;
        public Image yellow = Oskas.Properties.Resources.button_yellow;
        public Image red = Oskas.Properties.Resources.button_red;

        List<OvenClient> ovnclient = new List<OvenClient>();
        List<VipLinePlc> Plc = new List<VipLinePlc>();

        // Config Json
        OvenConfig Config;


        // 連続Alarm発生回数
        int cntAlarm = 0;

        // 異常メール発行回数
        int alarmMailPub = 0;

        public FmOvens()
        {
            InitializeComponent();
            ConsoleShow("Oven Data Hub", Cnslcnf.msg_info);
            toolStripStatusLabel1.Text = "オーブンデータハブは停止中";
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_red;

            //Nlog
            OskNLog.setFolderName("OVENS");

            InitOvens();

        }

        public void InitOvens()
        {
            //
            // Read Config Json
            // ovenclientリスト作成
            // PLC objects 作成
            // Console用Action挿入
            //
            var i = 0;
            Config = OvenConfigFuncs.ovnSrvConfig();
            foreach (var plcconf in Config.ceObject.oven_client)
            {
                ovnclient.Add(plcconf);
                Plc.Add(new VipLinePlc(plcconf));
                var consoleAction = new Action<string, int>(Console);
                Plc[i].InitConsoleAction(consoleAction);
                i++;
            }

            //
            // Tree View Form Object
            //
            _fmOvnView = new FmOvnView(ovnclient, Plc);

        }


        // Action for Console
        public void Console(string text, int level)
        {
            //ConsoleShow(text, level);
            OskNLog.Log(text, level);
        }

        //private void RunTask(VipLinePlc plc, bool dotask)
        //{
        //    plc.RunTask(dotask);
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            if (txt_dev.Text.Length == 7)
            {
                var Plc = new Mitsubishi("192.168.1.99", 1026);
                var plcMemAddr = txt_dev.Text;
                if (!Mitsubishi.isHexFormAddress(plcMemAddr))
                {
                    var hexaddress = string.Empty;
                    if (Mitsubishi.ExchangeDecAddressDevtoHex(plcMemAddr, ref hexaddress))
                    {
                        plcMemAddr = hexaddress;
                    }
                    else
                    {
                        return;
                    }
                }
                var Ret = Plc.GetWordAsDecimalData(plcMemAddr, 1);
                //var Ret = Plc.GetWordsAsDateTime(plcMemAddr);
                ConsoleShow(txt_dev.Text + ": " + Ret.ToString(), Cnslcnf.msg_info);
            }
            else
            {
                ConsoleShow("7桁で入力してください", Cnslcnf.msg_error);
            }
        }

        private void MnHyoujiSetubi_Click(object sender, EventArgs e)
        {
            if (!_fmOvnView.Visible)
            {
                if (_fmOvnView.IsDisposed)
                {
                    _fmOvnView = new FmOvnView(ovnclient, Plc);
                }
                _fmOvnView.Show(this);
            }
            else
            {
                ConsoleShow("ひょうじされとる券", Cnslcnf.msg_error);
            }
        }

        private void bt_StopHub_Click(object sender, EventArgs e)
        {
            StopHub();
        }

        private void StopHub()
        {
            habEnable = false;

            TaskTimer.Enabled = false;  // toolStripStatusLabel2表示用

            //foreach (var plc in Plc)
            //{
            //    if (!plc.isDisposed())
            //    {
            //        RunTask(plc, false); 
            //    }
            //    else
            //    {
            //        ConsoleShow(plc.macno + " is disposed", Cnslcnf.msg_debug);
            //    }
            //}

            toolStripStatusLabel1.Text = "オーブンデータハブは停止中";
            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_red;
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel2.Image = null;
        }

        private void btServerStart_Click(object sender, EventArgs e)
        {
            habEnable = true;
            ConsoleShow("オーブンデータハブは稼働開始しました", Cnslcnf.msg_info);

            //foreach (var plc in Plc)
            //{
            //    if (!plc.isDisposed())
            //    {
            //        RunTask(plc, true);
            //    }
            //    else
            //    {
            //        ConsoleShow(plc.macno + " is disposed", Cnslcnf.msg_debug);
            //    }
            //}

            toolStripStatusLabel1.Text = "オーブンデータハブは稼働中";
            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_blue;
            TaskTimer.Enabled = true;　// toolStripStatusLabel2表示用
            //_fmOvnView.UpdateOvenTree(ovnclient, Plc);
        }

        private void TaskTimer_Tick(object sender, EventArgs e)
        {
            _fmOvnView.UpdateOvenTree(ovnclient, Plc);

            var activePlcCnt = 0;
            var errorPlcCnt = 0;
            foreach (var plc in Plc)
            {
                if (!plc.taskEnabled) activePlcCnt++;
                if (plc.FetchStatus != 0) errorPlcCnt++;
            }

            if (activePlcCnt > 0)
            {
                toolStripStatusLabel2.Text = "停止中のオーブンが発生";
                toolStripStatusLabel2.Image = null;
                toolStripStatusLabel2.Image = yellow;
            }
            else if (errorPlcCnt > 0)
            {
                toolStripStatusLabel2.Text = "オーブンにアラーム／エラーが発生";
                toolStripStatusLabel2.Image = null;
                toolStripStatusLabel2.Image = red;
                cntAlarm += 1;
            }
            else
            {
                toolStripStatusLabel2.Text = "オーブンは正常に稼働中";
                toolStripStatusLabel2.Image = null;
                toolStripStatusLabel2.Image = blue;
                cntAlarm = 0;
                alarmMailPub = 0;
            }

            // 連続アラーム発生時間(minute)の算出
            var alarmTime = cntAlarm * TaskTimer.Interval / (60 * 1000);

            // 連続アラーム規定値を超えているか？
            if (alarmTime >= Config.ceObject.mailConf.ovenAlarmTime_Min)
            {
                // メール発行数設定が0以上であればメール発行
                if (Config.ceObject.mailConf.pubMailTimes > 0)
                    // メール発行数が規定値以下であるか
                    if (alarmMailPub < Config.ceObject.mailConf.pubMailTimes)
                    {
                        // 連続アラームが初回メール発行後を含め規定値以上であればメール発行
                        if (alarmTime >= alarmMailPub * Config.ceObject.mailConf.pubMailTerm_Min + Config.ceObject.mailConf.ovenAlarmTime_Min)
                        {
                            alarmMailPub += 1;
                            //MessageBox.Show($"メール発行{alarmMailPub}回目");
                            ConsoleShow("連続アラーム異常通知メールを発行しました", Cnslcnf.msg_error);
                            foreach (var someone in Config.ceObject.mailConf.mailto)
                            {
                                Oskas.Mailkit.SendMail(someone, Config.ceObject.mailConf.mail_title, Config.ceObject.mailConf.mail_contents);
                            }
                        }
                    }
            }

            GC.Collect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OskNLog.Log("Hello World!", 1);
        }

        private void FmOvens_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (habEnable)
            {
                MessageBox.Show("オーブンハブを停止してからクローズしてください");
                e.Cancel = true;
            }
        }

        private void MnSettei_OpenFile_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", Config.ceObject.header.path);
        }
    }

}
