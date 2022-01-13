using System.Collections.Generic;
using System.Windows.Forms;
using Ovens.models;
using Oskas;

namespace Ovens
{
    public partial class FmOvnView : Form
    {

        public FmOvnView(List<OvenClient> ovnclient, List<VipLinePlc> Plc)
        {
            InitializeComponent();

            var plcNum = ovnclient.Count;
            var treeNode = new TreeNode[plcNum];

            for (int i = 0; i < plcNum; i++)
            {
                var ovnNum = ovnclient[i].oven.Count;
                var treeNode_ovn = new TreeNode[ovnNum];
                for (int l = 0; l < ovnNum; l++)
                {
                    treeNode_ovn[l] = new TreeNode(ovnclient[i].oven[l].oven_id);
                    //treeNode_ovn[l].ForeColor = System.Drawing.Color.Gray;
                }

                treeNode[i] = new TreeNode(ovnclient[i].header_ovn.macno, treeNode_ovn);
                treeNode[i].ForeColor = System.Drawing.Color.LightGray;
            }
            treeView1.Nodes.Clear();
            treeView1.Nodes.AddRange(treeNode);
        }

        public void UpdateOvenTree(List<OvenClient> ovnclient, List<VipLinePlc> Plc)
        {
            var plcNum = ovnclient.Count;

            for (int i = 0; i < plcNum; i++)
            {
                var ovnNum = ovnclient[i].oven.Count;
                
                for (int l = 0; l < ovnNum; l++)
                {
                    //treeView1.Nodes[i].Nodes[l].ForeColor = System.Drawing.Color.Red;
                }

                if (!Plc[i].taskEnabled)
                {
                    treeView1.Nodes[i].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    treeView1.Nodes[i].ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        bool _bDoubleClicked;

        private void treeView1_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 必要であれば下記をフォームに変更して情報表示する
            MessageBox.Show(treeView1.SelectedNode.Text);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                _bDoubleClicked = true;
            }
            else
            {
                _bDoubleClicked = false;
            }
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (_bDoubleClicked == true)
            {
                e.Cancel = true;
                _bDoubleClicked = false;
            }
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (_bDoubleClicked == true)
            {
                e.Cancel = true;
                _bDoubleClicked = false;
            }
        }
    }

    public class OvenTreeNode
    {
        public TreeNode treeNode_Cnt { get; set; }
        public TreeNode[] treeNode_Ovn { get; set; }
    }

}