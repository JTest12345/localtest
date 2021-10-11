using System.Collections.Generic;
using System.Windows.Forms;
using Ovens.models;
using Oskas;

namespace Ovens
{
    public partial class FmOvnView : Form
    {
        private List<VipLinePlc> PlcList;

        public FmOvnView(List<OvenClient> ovnclient, List<VipLinePlc> Plc)
        {
            InitializeComponent();
            UpdateOvenTree(ovnclient, Plc);

            //var plcNum = ovnclient.Count;
            //var treeNode = new TreeNode[plcNum];

            //for (int i = 0; i < plcNum; i++)
            //{
            //    var ovnNum = ovnclient[i].oven.Count;
            //    var treeNode_ovn = new TreeNode[ovnNum];
            //    for (int l = 0; l < ovnNum; l++)
            //    {
            //        treeNode_ovn[l] = new TreeNode(ovnclient[i].oven[l].oven_id);
            //        //treeNode_ovn[l].ForeColor = System.Drawing.Color.Gray;
            //    }

            //    treeNode[i] = new TreeNode(ovnclient[i].header_ovn.macno, treeNode_ovn);
            //    treeNode[i].ForeColor = System.Drawing.Color.LightGray;
            //}
            //treeView1.Nodes.Clear();
            //treeView1.Nodes.AddRange(treeNode);
        }

        public void UpdateOvenTree(List<OvenClient> ovnclient, List<VipLinePlc> Plc)
        {
            var plcNum = ovnclient.Count;
            var treeNode = new TreeNode[plcNum];

            PlcList = new List<VipLinePlc>();

            for (int i = 0; i < plcNum; i++)
            {
                var ovnNum = ovnclient[i].oven.Count;
                var treeNode_ovn = new TreeNode[ovnNum];

                PlcList.Add(Plc[i]);
                
                for (int l = 0; l < ovnNum; l++)
                {
                    treeNode_ovn[l] = new TreeNode(ovnclient[i].oven[l].oven_id);
                    //treeNode_ovn[l].ForeColor = System.Drawing.Color.Gray;
                }

                treeNode[i] = new TreeNode(ovnclient[i].header_ovn.macno, treeNode_ovn);
                if (!Plc[i].taskEnabled)
                {
                    treeNode[i].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    treeNode[i].ForeColor = System.Drawing.Color.Black;
                }
            }
            treeView1.Nodes.Clear();
            treeView1.Nodes.AddRange(treeNode);
        }

        bool _bDoubleClicked;

        private void treeView1_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            PlcList[treeView1.SelectedNode.Index].taskEnabled = false;
            MessageBox.Show(PlcList[treeView1.SelectedNode.Index].taskEnabled.ToString());
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