using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Data.Linq;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model;

using ARMS3.Model.Machines;
using ARMS3.Model.Carriers;
using System.Threading;
using ARMS3.Model.PLC;

namespace ARMS3
{
    public partial class FrmSignalDisplay : Form
    {
        /// <summary>
        /// イメージリスト用列挙体
        /// 「initImageList」関数のImages.Addの順番を変更する場合は、メンバーの順番を変更する
        /// </summary>
        private enum ButtonColorEnum
        {
            Black = 0,
            Error,
            Blue,
            Green,
            Yellow,
            Red,
        }

        /// <summary>
        /// 装置用TreeNodeクラス
        /// </summary>
        private class MachineTreeNode : TreeNode
        {
            public string TabName { get; set; }
            public int MacNo { get; set; }
            public int UniqNo { get; set; }
            public string LineNo { get; set; }
            public List<SignalTreeNode> ChildNodes { get; set; }

            public void updateSignalStatus()
            {
                if (this.ChildNodes == null || this.ChildNodes.Any() == false) return;
                int imageIndex = (int)ButtonColorEnum.Black;
                int idx = this.ChildNodes.FindIndex(l => l.IsMachineReadyBitSignal == true);
                if (idx != -1)
                {
                    if (this.ChildNodes[idx].BitOn == true)
                    {
                        imageIndex = (int)ButtonColorEnum.Green;
                    }
                }
                else
                {
                    if (this.ChildNodes.Any(l => l.BitOn == true) == true)
                    {
                        imageIndex = (int)ButtonColorEnum.Green;
                    }
                }
                this.ImageIndex = imageIndex;
                this.SelectedImageIndex = imageIndex;
            }
        }

        /// <summary>
        /// 装置内の各信号項目用TreeNodeクラス
        /// </summary>
        private class SignalTreeNode : TreeNode
        {
            public ButtonColorEnum ButtonColor { get; set; }
            public IPLC Plc { get; set; }
            public string PlcAddress { get; set; }
            public string PlcPort { get; set; }
            public string PlcMaker { get; set; }
            public string PlcSocket { get; set; }
            public string HostIpAddress { get; set; }
            public string BitAddress { get; set; }
            public bool IsMachineReadyBitSignal { get; set; }
            public bool BitOn
            {
                get
                {
                    if (this.ImageIndex != (int)ButtonColorEnum.Black && this.ImageIndex != (int)ButtonColorEnum.Error)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        #region 定数

        private const string MACHINE_READY_BIT_ADDRESS_UPPER_NAME = "MACHINEREADYBITADDRESS";

        #endregion

        #region プロパティ

        /// <summary>
        /// 装置TreeNodeリスト
        /// </summary>
        private List<MachineTreeNode> macNodeList { get; set; }

        /// <summary>
        /// 信号のアイコンリスト
        /// </summary>
        private ImageList imageList { get; set; }

        #endregion

        #region コンストラクタ

        public FrmSignalDisplay()
        {
            InitializeComponent();

            //FormRedraw(true);
        }

        #endregion

        /// <summary>
        /// 画面ロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSignalDisplay_Load(object sender, EventArgs e)
        {
            this.initMachineTreeNodeList();

            if (this.macNodeList.Any() == false)
            {
                MessageBox.Show("信号確認の対象装置が存在しません。");
                this.Close();
            }

            this.initTabControl();

            this.updateAllMachineSignalStatus(false);
        }

        /// <summary>
        /// 画面で使用するイメージリストを初期化
        /// </summary>
        private void initImageList()
        {
            // ※Image.Addの順番を変える場合は、「ButtonColorEnum」列挙体のメンバーの順番を変える
            imageList = new ImageList();
            imageList.Images.Add(Properties.Resources.button_black);
            imageList.Images.Add(Properties.Resources.error);
            imageList.Images.Add(Properties.Resources.button_blue);
            imageList.Images.Add(Properties.Resources.button_green);
            imageList.Images.Add(Properties.Resources.button_yellow);
            imageList.Images.Add(Properties.Resources.button_red);
        }

        /// <summary>
        /// マスタに指定した文字に対応したButtonColor列挙体を返す
        /// </summary>
        /// <param name="buttoncolor"></param>
        /// <returns></returns>
        private ButtonColorEnum getButtonColor(string buttoncolor)
        {
            switch(buttoncolor.ToUpper().Trim())
            {
                case "BLUE":
                    return ButtonColorEnum.Blue;

                case "GREEN":
                    return ButtonColorEnum.Green;

                case "YELLOW":
                    return ButtonColorEnum.Yellow;

                case "RED":
                    return ButtonColorEnum.Red;

                default:
                    return ButtonColorEnum.Black;
            }
        }

        /// <summary>
        /// 画面上の装置 ( + 信号項目) のリストを初期化
        /// </summary>
        private void initMachineTreeNodeList()
        {
            this.initImageList();

            XDocument doc = XDocument.Load(LineConfig.LineConfigFilePath);
            IEnumerable<XElement> machineElms = LineConfig.LoadMachineLineConfig(doc);

            var flownms = LineKeeper.Machines
                .Where(l => string.IsNullOrWhiteSpace(l.SignalDisplayFlowName) == false)
                .Select(x => x.SignalDisplayFlowName)
                .Distinct();
            var signalMasters = LineconfigSignalDisplay.GetLineconfigSignalDisplay(flownms);
            
            this.macNodeList = new List<MachineTreeNode>();
            string[] dispLineNoList = Config.Settings.SignalDisplayLineNoList.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

            foreach (var macElm in machineElms)
            {
                int macNo = int.Parse(macElm.Element("machineNo").Value);

                IMachine imac = LineKeeper.Machines.FirstOrDefault(l => l.MacNo == macNo);
                if (imac == null) continue;

                // 設定ファイルで指定していないラインの装置は表示しない
                if (dispLineNoList.Contains(imac.LineNo) == false) continue;

                var node = new MachineTreeNode();
                node.TabName = imac.SignalDisplayFlowName;
                node.MacNo = imac.MacNo;
                node.UniqNo = imac.MacNo % 1000;
                node.Text = $"{imac.Name} 装置番号=[{imac.MacNo}]";
                node.LineNo = imac.LineNo;                
                node.ChildNodes = new List<SignalTreeNode>();

                #region 装置1台あたりの各信号項目の設定

                var flowMaster = signalMasters.Where(l => l.flownm == imac.SignalDisplayFlowName);
                if (flowMaster.Any() == false) continue;
                
                foreach (var master in flowMaster.OrderBy(l => l.vieworder))
                {
                    var sigNode = new SignalTreeNode();
                    sigNode.Text = master.dispname;
                    sigNode.ButtonColor = this.getButtonColor(master.buttoncolor);
                    sigNode.IsMachineReadyBitSignal = (master.tagname.ToUpper() == MACHINE_READY_BIT_ADDRESS_UPPER_NAME);

                    #region PLC情報の取得

                    string addressTagName = "plcAddress";
                    string portTagName = "plcPort";
                    string makerTagName = "plcMaker";
                    string haddressTagName = "hostIpAddress";

                    if (string.IsNullOrWhiteSpace(master.plcaddresstagname) == false)
                    {
                        addressTagName = master.plcaddresstagname;
                    }
                    if (string.IsNullOrWhiteSpace(master.plcporttagname) == false)
                    {
                        portTagName = master.plcporttagname;
                    }
                    if (string.IsNullOrWhiteSpace(master.plcmakertagname) == false)
                    {
                        makerTagName = master.plcmakertagname;
                    }
                    if (string.IsNullOrWhiteSpace(master.hostipadresstagname) == false)
                    {
                        haddressTagName = master.hostipadresstagname;
                    }

                    XElement plcAddressElm = macElm.Element(addressTagName);
                    XElement plcPortElm = macElm.Element(portTagName);
                    XElement plcMakerElm = macElm.Element(makerTagName);
                    XElement hostIpAddressElm = macElm.Element(haddressTagName);

                    sigNode.PlcAddress = plcAddressElm?.Value;
                    sigNode.PlcPort = plcPortElm?.Value;
                    sigNode.HostIpAddress = hostIpAddressElm?.Value;
                    sigNode.PlcMaker = plcMakerElm?.Value;

                    XElement plcSocketElm = macElm.Element("plcSocket");
                    sigNode.PlcSocket = plcSocketElm?.Value ?? Socket.Udp.ToString();

                    #region CarrierPLC or ConveyorPlc 指定の場合の特殊処理 (ARMS3.Model.LineConfig.csの内容を移植)
                    if ( (addressTagName == "carrierPlcAddress" && portTagName == "carrierPlcPort") ||
                         (addressTagName == "conveyorPlcAddress" && portTagName == "conveyorPlcPort") )
                    {
                        // PLCメーカーの指定がない場合は、三菱製を指定
                        if (string.IsNullOrWhiteSpace(master.plcmakertagname) || 
                            macElm.Element(master.plcmakertagname) == null ||
                            string.IsNullOrWhiteSpace(macElm.Element(master.plcmakertagname).Value))
                        {
                            sigNode.PlcMaker = Maker.Mitsubishi.ToString();
                        }

                        sigNode.PlcSocket = Socket.Udp.ToString();
                        if (sigNode.PlcMaker == Maker.Keyence.ToString())
                        {
                            sigNode.PlcSocket = Socket.Tcp.ToString();
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(sigNode.PlcMaker) == true || string.IsNullOrEmpty(sigNode.PlcAddress) == true ||
                        string.IsNullOrEmpty(sigNode.PlcPort) == true)
                    {
                        continue;
                    }
                    #endregion

                    #region BitAddress情報の取得

                    string bitAddress = string.Empty;
                    switch(master.xmlkb)
                    {
                        case 1:
                            bitAddress = macElm.Element(master.tagname)?.Value;
                            break;

                        case 2:
                            IEnumerable<XElement> pElms = macElm.Elements(master.tagname);
                            IEnumerable<XElement> cElms = pElms?.Elements(master.childtagname);
                            bitAddress = cElms?.ElementAtOrDefault(master.childno)?.Value;                            
                            break;

                        case 3:
                        case 4:
                            var subElms = macElm.Element(master.tagname);
                            if (subElms != null)
                            {
                                foreach(var elm in subElms.Elements(master.childtagname))
                                {
                                    int key;
                                    string sKey = elm.Attribute("key")?.Value;
                                    if (int.TryParse(sKey, out key) == false) continue;
                                    if (key != master.childno) continue;

                                    if (master.xmlkb == 3)
                                    {
                                        bitAddress = elm.Value;
                                    }
                                    else /* if (master.xmlkb == 4) */
                                    {
                                        bitAddress = elm.Attribute("reqBit")?.Value;
                                    }
                                    break;
                                }
                            }
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(bitAddress) == true) continue;
                    sigNode.BitAddress = bitAddress;

                    #endregion

                    node.ChildNodes.Add(sigNode);
                }

                // 同じIPアドレス・ポート・メーカーのPLCはクラス変数の実体をまとめておく
                foreach(var group in node.ChildNodes.GroupBy(g => new { g.PlcAddress, g.PlcPort, g.PlcMaker, g.PlcSocket, g.HostIpAddress }))
                {
                    IPLC plc = Common.GetInstance(group.Key.PlcMaker, group.Key.PlcAddress, int.Parse(group.Key.PlcPort), 
                                                     group.Key.HostIpAddress, group.Key.PlcSocket);
                    
                    // 新規生成したPLCクラス変数への参照を割り当てる
                    foreach (SignalTreeNode sigNode in group)
                    {
                        sigNode.Plc = plc;
                    }
                }

                #endregion

                //// 装置の信号色 (緑 or 黒)
                //int imageIndex = (int)ButtonColorEnum.Green;
                //if (node.ChildNodes.Any() == false)
                //{
                //    imageIndex = (int)ButtonColorEnum.Error;
                //}
                //node.ImageIndex = imageIndex;
                //node.SelectedImageIndex = imageIndex;

                this.macNodeList.Add(node);
            }

            // 並び替え ①TmMachine.[lineno] -> ②TmMachine.macno の下3桁 -> ③TmMachine.macno
            this.macNodeList = this.macNodeList.OrderBy(l => l.TabName).ThenBy(l => l.LineNo).ThenBy(l => l.UniqNo).ThenBy(l => l.MacNo).ToList();
        }

        /// <summary>
        /// タブページ + 装置のツリーノードを新規作成
        /// </summary>
        private void initTabControl()
        {
            // 画面上で右クリックを押したときのメニュー
            var menu = new ContextMenuStrip();
            menu.Opening += new System.ComponentModel.CancelEventHandler(this.menu_Opening);

            // メニューに項目追加
            var itemSignalUpdate = new ToolStripMenuItem();
            itemSignalUpdate.Text = "信号取得";
            itemSignalUpdate.Image = Properties.Resources.button_green;
            itemSignalUpdate.Click += new System.EventHandler(this.itemSignalUpdate_Click);
            menu.Items.Add(itemSignalUpdate);

            // 各タブページの作成
            foreach (var group in this.macNodeList.GroupBy(g => g.TabName))
            {
                var tv = new TreeView();
                tv.ImageList = this.imageList;
                tv.Width = 400; tv.Height = 450;
                tv.ContextMenuStrip = menu;

                foreach (var macNode in group)
                {
                    tv.Nodes.Add(macNode);                    
                    foreach (var childNode in macNode.ChildNodes)
                    {
                        macNode.Nodes.Add(childNode);
                    }
                }

                var tp = new TabPage(group.Key);
                tp.Controls.Add(tv);
                this.tabControl.TabPages.Add(tp);
            }
        }

        /// <summary>
        /// 指定タブページの全装置信号色を更新
        /// </summary>
        /// <param name="onlyselecedtabpage">選択中のタブページのみを更新するかどうか</param>
        private void updateAllMachineSignalStatus(bool onlyselecedtabpage)
        {
            foreach(TabPage tab in this.tabControl.TabPages)
            {
                // 選択中のタブページのツリーのみ更新
                if (onlyselecedtabpage == true &&
                    this.tabControl.TabPages.IndexOf(tab) != this.tabControl.SelectedIndex)
                {
                    continue;
                }

                foreach (var ctrl in tab.Controls)
                {
                    if ((ctrl is TreeView) == false) continue;

                    var tv = (TreeView)ctrl;
                    foreach (var node in tv.Nodes)
                    {
                        if ((node is MachineTreeNode) == false) continue;

                        var macNode = (MachineTreeNode)node;
                        this.updateSignalStatus(macNode.ChildNodes);
                        macNode.updateSignalStatus();
                    }
                }
            }
        }

        /// <summary>
        /// 指定ノードの信号色を更新する (単体TreeNode指定)
        /// </summary>
        /// <param name="node">対象ノード</param>
        private void updateSignalStatus(SignalTreeNode node)
        {
            var list = new List<SignalTreeNode>() { node };
            this.updateSignalStatus(list);
        }

        /// <summary>
        /// 指定ノードの信号色を更新する (複数TreeNode指定)
        /// </summary>
        /// <param name="nodes">対象ノードリスト</param>
        private void updateSignalStatus(List<SignalTreeNode> nodes)
        {
            // PLCへのping不通時の時短処理   (ノード単位でPingチェックせずに同じアドレスのPLCは一回でPingチェックを終わらせる)
            foreach (var group in nodes.GroupBy(g => g.Plc))
            {
                IPLC iPlc = group.Key;

                bool pingOK = true;
                try
                {
                    pingOK = iPlc.Ping(iPlc.IPAddress, 50, 1);
                }
                catch (Exception)
                {
                    pingOK = false;
                }

                // PING通信がつながらない場合は、対象PLCを指定している項目を全てエラー扱いにする
                if (pingOK == false)
                {
                    int imageIndex = (int)ButtonColorEnum.Error;
                    foreach (SignalTreeNode sigNode in group)
                    {
                        sigNode.ImageIndex = imageIndex;
                        sigNode.SelectedImageIndex = imageIndex;
                    }
                    continue;
                }

                // 各信号の取得
                foreach (SignalTreeNode sigNode in group)
                {
                    try
                    {
                        if (iPlc.GetBit(sigNode.BitAddress) == Common.BIT_ON)
                        {
                            sigNode.ImageIndex = (int)sigNode.ButtonColor;
                        }
                        else
                        {
                            sigNode.ImageIndex = (int)ButtonColorEnum.Black;
                        }
                    }
                    catch (Exception)
                    {
                        sigNode.ImageIndex = (int)ButtonColorEnum.Error;
                    }
                    sigNode.SelectedImageIndex = sigNode.ImageIndex;
                }
            }
        }

        /// <summary>
        /// 信号取得ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.updateAllMachineSignalStatus(true);

            MessageBox.Show("信号取得完了");
        }

        /// <summary>
        /// ツリー描画枠内で右クリックを押したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_Opening(object sender, CancelEventArgs e)
        {
            TabPage tab = this.tabControl.TabPages[this.tabControl.SelectedIndex];
            int tabIndex = -1;
            foreach (var ctrl in tab.Controls)
            {
                if ((ctrl is TreeView) == false) continue;

                tabIndex = tab.Controls.IndexOf((Control)ctrl);
                break;
            }

            var tv = (TreeView)tab.Controls[tabIndex];
            if (tv.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// 信号取得ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemSignalUpdate_Click(object sender, EventArgs e)
        {
            TabPage tab = this.tabControl.TabPages[this.tabControl.SelectedIndex];
            int tabIndex = -1;
            foreach (var ctrl in tab.Controls)
            {
                if ((ctrl is TreeView) == false) continue;

                tabIndex = tab.Controls.IndexOf((Control)ctrl);
                break;      
            }
            if (tabIndex == -1) return;

            var tv = (TreeView)tab.Controls[tabIndex];            
            if (tv.SelectedNode.Level == 0)
            {
                //装置選択時
                MachineTreeNode selectedNode = (MachineTreeNode)tv.SelectedNode;
                this.updateSignalStatus(selectedNode.ChildNodes);
                selectedNode.updateSignalStatus();
            }
            else
            {
                //信号項目選択時
                var node = (SignalTreeNode)tv.SelectedNode;
                this.updateSignalStatus(node);
                MachineTreeNode macNode = (MachineTreeNode)node.Parent;
                macNode.updateSignalStatus(); 
            }

            MessageBox.Show("信号取得完了");
        }        
    }
}
