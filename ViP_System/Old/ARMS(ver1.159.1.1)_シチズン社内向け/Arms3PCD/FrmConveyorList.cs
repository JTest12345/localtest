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
    public partial class FrmConveyorList : Form
    {
        private System.Media.SoundPlayer sp;

        public FrmConveyorList()
        {
            InitializeComponent();
            conveyorList = new SortedList<string, ConveyorData[]>();
            if (Config.IsBHT700 == false)
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string dir = System.IO.Path.GetDirectoryName(asm.GetName().CodeBase);
                this.sp = new System.Media.SoundPlayer(System.IO.Path.Combine(dir, "alert.wav"));
            }
        }

        private void FrmConveyorList_Load(object sender, EventArgs e)
        {
            this.Text = "ARMS3 [作業者:" + this.EmpCd + "]";
            Cursor.Current = Cursors.WaitCursor;
            updateAllList();
            Cursor.Current = Cursors.Default;

        }

        /// <summary>
        /// 社員番号
        /// </summary>
        public string EmpCd { get; set; }

        /// <summary>
        /// リスト更新処理中フラグ
        /// </summary>
        private bool updating = false;

        /// <summary>
        /// 現在選択中のタブの工程番号
        /// </summary>
        private string currentConveyorNo = "0";

        //コンベア選択用変数
        private int FINISH = 0;
        private int ERROR = 1;
        
        /// <summary>
        /// 各工程の作業完了ロットリスト
        /// </summary>
        private SortedList<string, ConveyorData[]> conveyorList;


        /// <summary>
        /// 更新
        /// </summary>

        #region updateAllList
        
        private void updateAllList()
        {
            if (updating) return;

            updating = true;
            try
            {
                //設定ファイルより対象コンベアリストを取得
                ConveyorData[] conveyors = Config.Conveyor;
                //コンベアリストが空なら元画面に戻る
                
                if (conveyors.Count() == 0)
                {
                    MessageBox.Show("コンベアの指定がされていません。");
                    this.Close();
                    return;
                }
               
                tabControl1.TabPages.Clear();
                this.conveyorList.Clear();

                foreach (ConveyorData c in conveyors)
                {
                    //完成品ロットリスト取得
                    TabPage finish = new TabPage();
                    finish.Text = "完了#" + c.cvName;
                    finish.Tag = "完了#"+ c.cvName;
                    tabControl1.TabPages.Add(finish);
                    ConveyorData[] finishlist = getConveyorLotList(c.cvNo, FINISH);
                    this.conveyorList.Add(finish.Tag.ToString(), finishlist);
                    //NG品ロットリスト取得
                    TabPage error = new TabPage();
                    error.Text = "NG#" + c.cvName;
                    error.Tag = "NG#" + c.cvName;
                    tabControl1.TabPages.Add(error);
                    ConveyorData[] errorlist = getConveyorLotList(c.cvNo, ERROR);
                    this.conveyorList.Add(error.Tag.ToString(), errorlist);
                }

                　　
               for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if ((string)tabControl1.TabPages[i].Tag == currentConveyorNo)
                    {
                        tabControl1.SelectedIndex = i;
                    }
                }

                listCurrentConveyorData();

            }
            finally
            {
                updating = false;
            }
        }
        #endregion

        #region listCurrentConveyorData

        private void listCurrentConveyorData()
        {
            this.lst1.Items.Clear();

            //表示タブの整形

            int tab = tabControl1.SelectedIndex;
            if (tab % 2 == 0)
            {
                this.lst1.Columns[0].Width = 120;
                this.lst1.Columns[1].Width = 100;
                this.lst1.Columns[2].Width = 120;
                this.lst1.Columns[3].Width = 0;
                this.lst1.Columns[4].Width = 450;
            }
            else
            {
                this.lst1.Columns[0].Width = 90;
                this.lst1.Columns[1].Width = 0;
                this.lst1.Columns[2].Width = 0;
                this.lst1.Columns[3].Width = 450;
                this.lst1.Columns[4].Width = 0;
            }

            if (this.conveyorList.Keys.Contains(currentConveyorNo) == false)
            {
                if (tabControl1.TabPages.Count >= 1)
                {
                    currentConveyorNo = tabControl1.TabPages[0].Tag.ToString();
                    if (this.conveyorList.Keys.Contains(currentConveyorNo) == false) return;
                }
                else
                {
                    return;
                }
            }

            ConveyorData[] clist = this.conveyorList[currentConveyorNo];
            if (clist == null) return;

            //カウント用
            int i = 0;

            foreach (ConveyorData lot in clist)
            {
                this.lst1.Items.Add(new ListViewItem(new string[] { lot.LotNo, lot.TypeCD, lot.NextProcCD, lot.Reason, lot.ResReason }));
                if (lot.ResReason != "")
                {
                    this.lst1.Items[i].ForeColor = Color.Blue;
                }
                i++;
            }
        }
        #endregion

        #region getLotList

        /// <summary>
        /// 中間サーバーWEBAPIからコンベア上のロットリスト取得
        /// </summary>
        /// <returns></returns>
        private ConveyorData[] getConveyorLotList(string conveyorno,int mode)
        {
            string xml = null;

            try
            {
                string url = Config.Url + "Home/ConveyorLotList/" + conveyorno + "/" + mode;
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
                MessageBox.Show(ex.ToString());
            }
           

            if (string.IsNullOrEmpty(xml))
                return new ConveyorData[0];
            else
                return ConveyorData.ParseXML(xml,mode);
        }

      
        #endregion

        #region menu
        
        private void menuItem1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            updateAllList();
            Cursor.Current = Cursors.Default;
        }

        private void menuQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion


        //タブ切り替え時
        #region tabControl1_SelectedIndexChanged
        
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating == true) return;

            currentConveyorNo = tabControl1.TabPages[tabControl1.SelectedIndex].Tag.ToString();
            listCurrentConveyorData();
        }
        #endregion

       

        private void menuBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}