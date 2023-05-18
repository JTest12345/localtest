using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model.NASCA;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMaintenance
{
    public partial class FrmImportLot : Form
    {
        private static FrmImportLot instance;

        public static FrmImportLot GetInstance()
        {
            if (instance == null)
            {
                instance = new FrmImportLot();
            }
            else if (instance.Disposing)
            {
                instance = new FrmImportLot();
            }

            return instance;
        }
        private FrmImportLot()
        {
            InitializeComponent();
        }

        #region AppendLog

        public static void AppendLog(string log)
        {
            if (instance == null)
            {
                Log.SysLog.Info(log);
                return;
            }

            if (instance.InvokeRequired == true)
            {
                instance.Invoke(new Action<string>(s => FrmImportLot.AppendLog(s)), log);
            }
            else
            {
                if (instance.txtLog.Lines.Length >= 5000)
                {
                    instance.txtLog.Text = instance.txtLog.Text.Remove(0, instance.txtLog.Lines[0].Length + 2);
                }

                Log.SysLog.Info(log);
                instance.txtLog.AppendText(log + "\r\n");
            }
        }
        #endregion

        private void 取り込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!rdoAllInfo.Checked && !rdoLot.Checked)
            {
                MessageBox.Show(this, "取り込み方法を選択してください");
                return;
            }

            bool isAllInfo = rdoAllInfo.Checked;

            DialogResult res;
            if (isAllInfo)
            {
                res = MessageBox.Show(this, "作業実績とマガジン情報をNASCAで上書きします", "警告",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
            }
            else
            {
                res = MessageBox.Show(this, "ロット情報と検査抜き取り情報をNASCAで上書きします", "警告",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
            }
            if (res != DialogResult.OK) return;

            try
            {
                if (string.IsNullOrEmpty(this.txtLotno.Text)) return;
                Action act = new Action(() =>
                {
                    string[] lotlist = this.txtLotno.Lines;
                    foreach (string lot in lotlist)
                    {
                        try
                        {
                            if (isAllInfo)
                            {
                                Importer.ImportAsmLot(lot, Config.Settings.SectionCd, new Action<string>(AppendLog));
                            }
                            else
                            {
                                Importer.ImportAsmLot(lot, new Action<string>(AppendLog));
                            }
                        }
                        catch
                        {
                        }
                    }
                });

                this.menuStrip1.Enabled = false;
                act.BeginInvoke(new AsyncCallback(complete), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void complete(IAsyncResult res)
        {
            Action act = new Action(() => { this.menuStrip1.Enabled = true; MessageBox.Show("完了"); });
            this.Invoke(act);
        }
    }
}
