using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Oskas;

namespace FIFDebugTools
{
    public partial class PlcTestForm : Form
    {
        //CommonFuncs common = new CommonFuncs();
        PlcCom plc = new PlcCom();
        MySQL db = new MySQL();

        // DB接続文字列
        string ConnectionString;
        // 改行コード
        static string crlf = "\r\n";


        public PlcTestForm()
        {
            InitializeComponent();

            //ConnectionString = ConnectionSt;  //DBが必要の際は引数から設定
            cmbCmd.SelectedIndex = 0;
        }


        delegate void ConsoleDelegatePlc(string text, int level);
        private void ConsoleShowPlc(string text, int level)
        {
            if (consoleBox_plc.InvokeRequired)
            {
                ConsoleDelegatePlc d = new ConsoleDelegatePlc(ConsoleShowPlc);
                BeginInvoke(d, new object[] { text, level });
            }
            else
            {
                string message = "";
                switch (level)
                {
                    case 1:
                        message = "[info] ";
                        break;
                    case 2:
                        message = "[Send] ";
                        break;
                    case 3:
                        message = "[Recieve] ";
                        break;
                    case 4:
                        message = "[Warn] ";
                        break;
                    case 5:
                        message = "[ERROR] ";
                        break;
                }
                consoleBox_plc.AppendText(message + text + crlf);
            }
        }


        private void btSendCmd_Click(object sender, EventArgs e)
        {
            bool CmdError=false;
            //string Command = "";
            string msg = "";
            int meslevel = 0;

            //if (plc.CheckSocketConnect(txtIpaddress.Text, Int32.Parse(txtPort.Text), ref msg))
            if (cmbCmd.SelectedIndex > -1 && txtDevType.Text != "" && txtDeviceNo.Text != "")
            {
                if (plc.CheckSocketConnect(txtIpaddress.Text, Int32.Parse(txtPort.Text), ref msg))
                {
                    switch (cmbCmd.SelectedIndex)
                    {
                        case 0:
                            //Command = "RD " + cmbDeviceType.Text + txtDeviceNo.Text + ".U\r";
                            plc.DeviceRead(txtIpaddress.Text, Int32.Parse(txtPort.Text), txtDevType.Text, Int32.Parse(txtDeviceNo.Text), ref msg);
                            break;
                        case 1:
                            if (txtCmdData.Text != "")
                            {
                                //Command = "WD " + cmbDeviceType.Text + txtDeviceNo.Text + ".U " + txtCmdData.Text + "\r";
                                plc.DeviceWrite(txtIpaddress.Text, Int32.Parse(txtPort.Text), txtDevType.Text, Int32.Parse(txtDeviceNo.Text), Int32.Parse(txtCmdData.Text), ref msg);
                                break;
                            }
                            else
                            {
                                CmdError = true;
                                break;
                            }
                    }
                }
                else
                    meslevel = 4;
            }
            else
                CmdError = true;

            if (!CmdError)
            {
                ConsoleShowPlc(msg, meslevel);
            }
            else
            {
                ConsoleShowPlc("コマンド条件が不正です", 4);
            }
            
        }


        private void btComTest_Click(object sender, EventArgs e)
        {
            bool CmdError = false;
            string msg = "";
            int meslevel = 0;

            if (txtIpaddress.Text != "" && txtPort.Text != "")
            {
                if (CommonFuncs.IntValueChk(txtPort.Text))
                {
                    if (plc.CheckSocketConnect(txtIpaddress.Text, Int32.Parse(txtPort.Text), ref msg))
                    {
                        plc.OpenPlcPortTest(txtIpaddress.Text, Int32.Parse(txtPort.Text), ref msg);
                    }
                    else
                        meslevel = 4;
                }
                else
                    CmdError = true;
            }
            else
                CmdError = true;

            if (!CmdError)
            {
                ConsoleShowPlc(msg, meslevel);
            }
            else
            {
                ConsoleShowPlc("コマンド条件が不正です", 4);
            } 
        }


        private void btQueryIpPt_Click(object sender, EventArgs e)
        {
            bool CmdError = false;
            string msg = "";
            var retDict = new Dictionary<string, string>();

            if (cmbPcat.Text != "" && txtMno.Text != "")
            {
                /*
                string mcat = cmbPcat.Text;
                string macno = txtMno.Text;
                string query = $"SELECT Ipaddress, Port, Devtype, Devno FROM Macconinfo WHERE Mno='{macno}' AND Pcat='{mcat}'";
                int dataLen = 4;
                if(!db.SqlTask_Read("*", ConnectionString, query, dataLen, ref retDict, ref msg))
                    CmdError = true;
                */

            }
            else
                CmdError = false;

            if (!CmdError)
            {
                txtIpaddress.Text = retDict["Ipaddress"];
                txtPort.Text = retDict["Port"];
                txtDevType.Text = retDict["Devtype"];
                txtDeviceNo.Text = retDict["Devno"];
                ConsoleShowPlc("PLC情報取得完了", 0);
            }
            else
            {
                ConsoleShowPlc("クエリ条件不正:" + msg, 4);
            }
        }

    }
}
