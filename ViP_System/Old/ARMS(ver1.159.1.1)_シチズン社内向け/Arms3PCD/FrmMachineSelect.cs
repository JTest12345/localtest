using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Diagnostics;
using DNWA.BHTCL;
using System.Threading;

namespace Arms3PCD
{
    public partial class FrmMachineSelect : Form
    {
        public FrmMachineSelect()
        {
            InitializeComponent();
        }

        private void FrmMachineSelect_Load(object sender, EventArgs e)
        {
            WebResponse res = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.txtUrl.Text = Config.Url;
                this.lstMachine.Items.Clear();
                string xml = null;

                //装置リストの取得
                string url = Config.Url + "Home/MachineList";

                WebRequest req = HttpWebRequest.Create(url);

                res = req.GetResponse();

                System.IO.Stream stm = res.GetResponseStream();

                using (StreamReader sr = new StreamReader(stm, System.Text.Encoding.UTF8))
                {
                    xml = sr.ReadToEnd();
                    res.Close();
                }

                //装置リストの表示
                if (!string.IsNullOrEmpty(xml))
                {
                    //xmlファイルの整形
                    MachineData[] machineList = MachineData.ParseXML(xml);
                    //前回設定ファイルの内容を取得
                    MachineData[] machineListSetting = Config.Machines;

                    foreach (MachineData m in machineList)
                    {
                        ListViewItem itm = this.lstMachine.Items
                        .Add(new ListViewItem(new string[] { m.Number, m.Name, m.Unit }));
                        //前回設定値を反映
                        MachineData found = machineListSetting.Where(r => r.Number == m.Number).FirstOrDefault();

                        if (found != null)
                        {
                            if (found.Enabled == true)
                            {
                                itm.Checked = true;
                            }
                            else
                            {
                                itm.Checked = false;
                            }
                        }

                    }
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// 設定保存用リストの取得
        /// </summary>
        /// <returns></returns>

        private MachineData[] getMachineDataFromListView()
        {
            List<MachineData> retv = new List<MachineData>();

            foreach (ListViewItem item in this.lstMachine.Items)                                                                                        
            {
                MachineData m = new MachineData();
                m.Enabled = item.Checked;
                m.Number = item.SubItems[0].Text;
                m.Name = item.SubItems[1].Text;
                m.Unit = item.SubItems[2].Text;
                retv.Add(m);
            }

            return retv.ToArray();
        }


        private void menuSave_Click(object sender, EventArgs e)
        {
            //設定ファイルへの書き込み
            Config.UpdateMachines(getMachineDataFromListView());
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}