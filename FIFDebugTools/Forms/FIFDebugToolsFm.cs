using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using FileIf;

namespace FIFDebugTools
{
    
    public partial class FIFDebugToolsFm : Oskas.fmMain
    {
        //PlcTestForm
        PlcTestForm _plcTestForm;
        //ArmsTranController
        ArmsTranController _armsTranController;

        // 改行コード
        static string crlf = "\r\n";
        // MagCupFSインスタンス
        Mcfilesys fs = new Mcfilesys();
        // MagCupDebugクラスインスタンス
        MagCupDebug MCDbg = new MagCupDebug();
        // mqttインスタンス
        MqttClient mqttClient1;
        MqttClient mqttClient2;

        public FIFDebugToolsFm()
        {
            _plcTestForm = new PlcTestForm();
            _armsTranController = new ArmsTranController();

            InitializeComponent();
            toolStripStatusLabel1.Text = string.Format("");
            toolStripStatusLabel2.Text = string.Format("");

            MCDbg.initMagCupFileSystem(fs);

            mqttClient1 = new MqttClient(fs.mci.mosquittoHost);
            mqttClient2 = new MqttClient(fs.mci.mosquittoHost);

            Task task = Task.Run(() =>
            {
                mqtttest();
                mqttmagcup();
            });
        }

        private void mqtttest()
        {
            mqttClient1.MqttMsgPublishReceived += (sender, eventArgs) =>
            {
                var msg = Encoding.UTF8.GetString(eventArgs.Message);
                var topic = eventArgs.Topic;
                ConsoleShow(msg);
            };

            var ret = mqttClient1.Connect(Guid.NewGuid().ToString());
            ConsoleShow($"Connected with result code {ret}" + crlf);
            mqttClient1.Subscribe(new[] { "test" }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //while (mqttClient.IsConnected)
            //{
            //}
        }

        private void mqttmagcup()
        {
            mqttClient2.MqttMsgPublishReceived += (sender, eventArgs) =>
            {
                var msg = Encoding.UTF8.GetString(eventArgs.Message);
                var topic = eventArgs.Topic;
                toolStripStatusLabel1.Text = string.Format("MagCupServer Status: " + msg);
            };

            var ret = mqttClient2.Connect(Guid.NewGuid().ToString());
            mqttClient2.Subscribe(new[] { "magcup" }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //while (mqttClient.IsConnected)
            //{
            //}
        }


        public delegate void _ConsoleDelegate(string text);
        public void ConsoleShow(string text)
        {
            if (consoleBox.InvokeRequired)
            {
                _ConsoleDelegate d = new _ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] { text });
            }
            else
            {
                consoleBox.AppendText(text);
            }
        }

        private void btServerStart_Click(object sender, EventArgs e)
        {
            var mes = "";
            if (MCDbg.InitMagDebugDbArea(fs, ref mes))
            {
                ConsoleShow(mes);
                ConsoleShow("*********************************************" + crlf);
                ConsoleShow("MagCupDBのデバック領域初期化が成功しました" + crlf);
                ConsoleShow("*********************************************" + crlf);
                btServerStart.Enabled = true;
            }
            else
            {
                ConsoleShow(mes);
                ConsoleShow("*********************************************" + crlf);
                ConsoleShow("MagCupDBのデバック領域初期化が失敗しました" + crlf);
                ConsoleShow("*********************************************" + crlf);
                btServerStart.Enabled = false;
            }
        }

        private void FIFDebugToolsFm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mqttClient1.IsConnected) mqttClient1.Disconnect();
            if (mqttClient2.IsConnected) mqttClient2.Disconnect();
        }

        private void OpenPlcTestForm(object sender, EventArgs e)
        {
            if (!_plcTestForm.Visible)
            {
                if (_plcTestForm.IsDisposed)
                {
                    _plcTestForm = new PlcTestForm();
                }
                _plcTestForm.Show(this);
            }
            else
            {
                ConsoleShow("表示されてますよ！");
            }
        }

        private void OpenArmsTranController(object sender, EventArgs e)
        {
            if (!_armsTranController.Visible)
            {
                if (_armsTranController.IsDisposed)
                {
                    _armsTranController = new ArmsTranController();
                }
                _armsTranController.Show(this);
            }
            else
            {
                ConsoleShow("表示されてますよ！");
            }
        }
    }
}
