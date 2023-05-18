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
    public partial class FrmLotList : Form
    {
        private System.Media.SoundPlayer sp;

        private const string MAC_BARCODE_HEADER = "07";
        private const string MAP_BARCODE_HEADER = "30";
        private const string LOT_BARCODE_HEADER = "11";
        private const string DEVIDED_LOT_BARCODE_HEADER = "13";

        private const string MAC_COMP_BARCODE_HEADER = "36";

        private const string DB_PRE_OVN_PL_PROC_KEYWORD = "DBPL";

        private const string MAC_OTHERMATERIAL_BARCODE_HEADER = "18";
        private const string MAC_MOLDRESIN_BARCODE_HEADER = "16";
        private const string MAC_CONDITION_BARCODE_HEADER = "20";
        private const string MAC_DEFECT_BARCODE_HEADER = "22";
        private const string MAC_WORKSTART_BARCODE_HEADER = "37";
        private const string MAC_FRAME_BARCODE_HEADER = "38";
        private const string MAC_WORKSTARTEND_BARCODE_HEADER = "39";
        private const string MAC_WAFER_BARCODE_HEADER = "40";
        private const string MAC_EICSTYPE_BARCODE_HEADER = "41";
        private const string MAC_CUTBLENDCOMP_BARCODE_HEADER = "43";
        private const string MAC_PRECUTBLEND_BARCODE_HEADER = "44";
        private const string MAC_WAFER2_BARCODE_HEADER = "45";

        public FrmLotList()
        {
            InitializeComponent();
            lotList = new SortedList<string, LotData[]>();
            if (Config.IsBHT700 == false)
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string dir = System.IO.Path.GetDirectoryName(asm.GetName().CodeBase);
                this.sp = new System.Media.SoundPlayer(System.IO.Path.Combine(dir, "alert.wav"));
            }
        }

        private void FrmLotList_Load(object sender, EventArgs e)
        {
            txtCode.Focus();
        }

        /// <summary>
        /// リスト更新処理中フラグ
        /// </summary>
        private bool updating = false;

        /// <summary>
        /// 現在選択中のタブの工程番号
        /// </summary>
        private string currentProcNo = "0";

        /// <summary>
        /// 各工程の作業完了ロットリスト
        /// </summary>
        private SortedList<string, LotData[]> lotList;

        #region updateAllList
        
        private void updateAllList()
        {
            if (updating) return;
            updating = true;
            try
            {
                ProcData[] procs = Config.Procs;
                tabControl1.TabPages.Clear();
                this.lotList.Clear();
              
                foreach (ProcData p in procs)
                {
                    if (p.Enabled == false) continue;

                    TabPage page = new TabPage();
                    page.Text = p.Name;
                    page.Tag = p.ProcNo;
                    tabControl1.TabPages.Add(page);
                 
                    //完了リスト取得
                    LotData[] Endlist = getLotList(p.ProcNo, LotData.WorkState.End);
                    //開始リスト取得
                    LotData[] Startlist = null;
                    if (p.ProcNo != DB_PRE_OVN_PL_PROC_KEYWORD)
                    {
                        Startlist = getLotList(p.ProcNo, LotData.WorkState.Start);
                    }
                 
                    List<LotData> result = new List<LotData>();

                    result.AddRange(Endlist);
                    if (Startlist != null)
                    {
                        result.AddRange(Startlist);
                    }
                    
                    this.lotList.Add(p.ProcNo, result.ToArray());
                  
                }
              

                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if ((string)tabControl1.TabPages[i].Tag == currentProcNo)
                    {
                        tabControl1.SelectedIndex = i;
                    }
                }

                listCurrentProcData();

            }
            finally
            {
                updating = false;
            }
        }
        #endregion

        #region listCurrentProcData

        private void listCurrentProcData()
        {
            this.lst1.Items.Clear();
            if (this.lotList.Keys.Contains(currentProcNo) == false)
            {
                if (tabControl1.TabPages.Count >= 1)
                {
                    currentProcNo = tabControl1.TabPages[0].Tag.ToString();
                    if (this.lotList.Keys.Contains(currentProcNo) == false) return;
                }
                else
                {
                    return;
                }
            }

            LotData[] plist = this.lotList[currentProcNo];
            if (plist == null) return;

             //カウント用
            int i = 0;


            foreach (LotData lot in plist)
            {
                string place;
                if (lot.Place == LotData.WorkState.Start)
                {
                    place = "開始";
                }
                else
                {
                    place = "完了";
                }


                this.lst1.Items.Add(new ListViewItem(new string[] { lot.LotNo, lot.MacName, lot.TypeCd, lot.ResinGpCd, place }));

                //開始・完了で文字色変更
                if (lot.Place == LotData.WorkState.Start)
                {
                    this.lst1.Items[i].ForeColor = Color.Blue;
                }
                else
                {
                    this.lst1.ForeColor = Color.Red;
                }
                i++;
            }
        }
        #endregion

        #region getLotList

        /// <summary>
        /// 中間サーバーWEBAPIからロットリスト取得
        /// </summary>
        /// <returns></returns>
        private LotData[] getLotList(string procno, LotData.WorkState place)
        {
            //DB硬化前プラズマの時間監視分岐
            if (procno == DB_PRE_OVN_PL_PROC_KEYWORD)
            {
                return getDBPreOvnPLLotList();
            }

            string xml = null;
           
            try
            {
                xml = getWorkData(procno, place, 0);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (string.IsNullOrEmpty(xml))
                return new LotData[0];
            else
                {
                //ロット一覧読み込み
                LotData[] lotList = LotData.ParseXML(xml,place);
              
                //有効設備リスト読み込み
                MachineData[] machineListSetting = Config.Machines;
               
                //有効設備のみのロットリスト
                List<LotData> newlotList = new List<LotData>();
              
                //有効設備のロットのみ表示
                foreach (LotData lot in lotList)
                {
                   // LotData data = new LotData();
                    foreach (MachineData machine in machineListSetting)
                    {
                        //有効設備でない時はスキップ
                        if (machine.Enabled == false)
                        {
                            continue;
                        }
                        //
                        if (machine.Number == lot.MacNumber)
                        {
                            //開始フラグが立っていない・ローダー仮想マガジン
                            newlotList.Add(lot);
                            break;
                        }
                    }
                }
          
                return newlotList.ToArray();
            }
        }

        private string getWorkData(string procno, LotData.WorkState place, int retryCt)
        {
            string url = "";
            string xml = null;

            try
            {
                //取得先のURLの選択
                if (place == LotData.WorkState.End)
                {
                    url = Config.Url + "Home/MelInst/" + procno;
                }
                else
                {
                    url = Config.Url + "Home/StartMag/" + procno;
                }

                WebRequest req = HttpWebRequest.Create(url);

                WebResponse res = req.GetResponse();

                try
                {
                    System.IO.Stream stm = res.GetResponseStream();

                    using (StreamReader sr = new StreamReader(stm, System.Text.Encoding.UTF8))
                    {
                        xml = sr.ReadToEnd();
                    }
                }
                finally
                {
                    res.Close();
                }
            }
            catch (WebException ex)
            {
                if (retryCt >= 5)
                {
                    throw ex;
                }

                System.Threading.Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return getWorkData(procno, place, retryCt);
            }

            return xml;
        }

        private LotData[] getDBPreOvnPLLotList()
        {
            string xml = null;

            try
            {
                WebRequest req = HttpWebRequest.Create(Config.Url + "Home/DBPreOvnPLInst/");


                WebResponse res = req.GetResponse();
                try
                {
                    System.IO.Stream stm = res.GetResponseStream();

                    using (StreamReader sr = new StreamReader(stm, System.Text.Encoding.UTF8))
                    {
                        xml = sr.ReadToEnd();
                    }
                }
                finally
                {
                    res.Close();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.ToString());
            }


            if (string.IsNullOrEmpty(xml))
                return new LotData[0];
            else
                return LotData.ParseXML(xml,0);
        }

        #endregion

        #region timer1_Tick
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            updateAllList();
            timer1.Interval = Config.Timer;

            if (timer2.Enabled == false)
            {
                this.timer2.Enabled = true;
            }
        }
        #endregion

        #region menu

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            updateAllList();
            Cursor.Current = Cursors.Default;
        }

        private void menuSetting_Click(object sender, EventArgs e)
        {
            FrmSetting frm = new FrmSetting();
            frm.ShowDialog();
        }

        private void menuMachineSelect_Click(object sender, EventArgs e)
        {
            FrmMachineSelect frm = new FrmMachineSelect();
            frm.ShowDialog();
        }

        private void menuCV_Click(object sender, EventArgs e)
        {
            FrmConveyorList frm = new FrmConveyorList();
            frm.ShowDialog();
        }

        private void menuQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        
        #region tabControl1_SelectedIndexChanged
        
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating == true) return;

            currentProcNo = tabControl1.TabPages[tabControl1.SelectedIndex].Tag.ToString();
            listCurrentProcData();
        }
        #endregion

        #region AlermTimer

        /// <summary>
        /// ユーザー通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            bool haslot = false;
            foreach (LotData[] d in this.lotList.Values)
            {
                if (d.Length >= 1)
                {
                    haslot = true;
                    break;
                }
            }

            if (haslot)
            {
                if (Config.IsBHT700)
                {
                    Beep beep = new Beep();
                    beep[Beep.Settings.EN_DEVICE.BUZZER] = Beep.EN_CTRL.ON;
                    beep[Beep.Settings.EN_DEVICE.VIBRATOR] = Beep.EN_CTRL.ON;
                    System.Threading.Thread.Sleep(5);
                }
                else
                {
                    sp.PlayLooping();
                    Thread.Sleep(3000);
                    sp.Stop();
                }
                this.timer2.Interval = Config.AlermTimer;
            }
        }
        #endregion

        #region go
        
        private void go()
        {
            if (string.IsNullOrEmpty(this.txtCode.Text)) return;

            string url;
            string[] str = this.txtCode.Text.Split(' ');

            if (str[0] == MAC_BARCODE_HEADER)
            {
                url = Config.Url + "Home/Index/" + str[1];
            }
            else if (str[0] == MAP_BARCODE_HEADER || str[0] == LOT_BARCODE_HEADER || str[0] == DEVIDED_LOT_BARCODE_HEADER)
            {
                url = Config.Url + "LotView/Index/?magno=" + this.txtCode.Text;
            }
            else if (str[0] == MAC_COMP_BARCODE_HEADER)
            {
                url = string.Format("{0}WorkEnd/UnloaderMag?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_OTHERMATERIAL_BARCODE_HEADER)
            {
                url = string.Format("{0}Material/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_MOLDRESIN_BARCODE_HEADER)
            {
                url = string.Format("{0}Resin/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_CONDITION_BARCODE_HEADER)
            {
                url = string.Format("{0}WorkCnd/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_DEFECT_BARCODE_HEADER)
            {
                url = string.Format("{0}Defect/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_WORKSTART_BARCODE_HEADER)
            {
                url = string.Format("{0}WorkStart/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_FRAME_BARCODE_HEADER)
            {
                url = string.Format("{0}Frame/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_WORKSTARTEND_BARCODE_HEADER)
            {
                url = string.Format("{0}WorkStartEnd/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_WAFER_BARCODE_HEADER)
            {
                url = string.Format("{0}Wafer/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_EICSTYPE_BARCODE_HEADER)
            {
                url = string.Format("{0}EicsTypeChange/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_CUTBLENDCOMP_BARCODE_HEADER)
            {
                url = string.Format("{0}CutBlend/Index?plantcd={1}", Config.Url, str[1]);
            }
            else if (str[0] == MAC_PRECUTBLEND_BARCODE_HEADER)
            {
                url = string.Format("{0}PreCutBlend/Index", Config.Url);
            }
            else if (str[0] == MAC_WAFER2_BARCODE_HEADER)
            {
                url = string.Format("{0}WaferEachSheet/Index?plantcd={1}", Config.Url, str[1]);
            }
            else
            {
                MessageBox.Show("不正なコードです");
                return;
            }
            
            ProcessStartInfo psi = new ProcessStartInfo("explorer.exe", url);
            Process.Start(psi);
            this.txtCode.Text = "";
        }

        private void btnGo_Click_1(object sender, EventArgs e)
        {
            go();
        }

        #endregion

        #region FrmLotList_KeyPress
        
        private void FrmLotList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                go();                                                                  
            }
        }
        #endregion




    }
}