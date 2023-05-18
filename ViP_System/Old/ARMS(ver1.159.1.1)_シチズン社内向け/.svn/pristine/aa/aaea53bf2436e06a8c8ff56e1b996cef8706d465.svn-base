//<--Y.Matsushima SGA-IM0000008218 2018/08/22
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmConfirmation : Form
    {
        private List<DefItem> DefectList;
        public bool retv=false;//OK:true,CANCEL:false

        public FrmConfirmation(List<DefItem> DefectChangeList)
        {
            InitializeComponent();
            DefectList = DefectChangeList;
        }

        static public bool ShowForm(List<DefItem> DefectChangeList)
        {
            FrmConfirmation frmconfirmation = new FrmConfirmation(DefectChangeList);
            frmconfirmation.ShowDialog();

            return frmconfirmation.retv;
        }
        private void FrmConfirmation_Load(object sender, EventArgs e)
        {
            this.grdDefect.DataSource = DefectList;

            this.grdDefect.Columns["OrderNo"].Visible = false;
            this.grdDefect.Columns["CauseCd"].Visible = false;
            this.grdDefect.Columns["CauseName"].ReadOnly = true;
            this.grdDefect.Columns["CauseName"].HeaderText = "起因";
            this.grdDefect.Columns["ClassCd"].Visible = false;
            this.grdDefect.Columns["ClassName"].ReadOnly = true;
            this.grdDefect.Columns["ClassName"].HeaderText = "分類";
            this.grdDefect.Columns["DefectCd"].ReadOnly = true;
            this.grdDefect.Columns["DefectCd"].HeaderText = "コード";
            this.grdDefect.Columns["DefectName"].ReadOnly = true;
            this.grdDefect.Columns["DefectName"].HeaderText = "名称";
            this.grdDefect.Columns["DefectCt"].DefaultCellStyle.BackColor = Color.Yellow;
            this.grdDefect.Columns["DefectCt"].HeaderText = "数量";
            this.grdDefect.Columns["DefectCt"].ReadOnly = true;
            this.grdDefect.Columns["IsDisplayedEICS"].Visible = false;
            this.grdDefect.Columns["ProcNo"].Visible = false;
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            this.retv = true;
            this.Close();
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
//-->Y.Matsushima SGA-IM0000008218 2018/08/22