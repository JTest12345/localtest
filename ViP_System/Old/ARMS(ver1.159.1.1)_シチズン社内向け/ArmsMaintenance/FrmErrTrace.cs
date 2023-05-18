using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmErrTrace : Form
    {
        public FrmErrTrace()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("ロット");
            dt.Columns.Add("裏面マーキング");
            dt.Columns.Add("側面マーキング");
            dt.Columns.Add("X");
            dt.Columns.Add("Y");
            dt.Columns.Add("エラー内容");
            dt.Columns.Add("工程");
        }

        DataTable dt;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtLotNo.Text) &&
                string.IsNullOrEmpty(this.txtSideMarking.Text) &&
                string.IsNullOrEmpty(this.txtBackSideMarking.Text))
            {
                MessageBox.Show("検索条件のいずれかを入力してください。");
                return;
            }

            ErrTrace[] errtraces = ErrTrace.GetDatas(this.txtLotNo.Text.Trim(), this.txtSideMarking.Text.Trim(), this.txtBackSideMarking.Text.Trim());

            dt.Clear();
            int i = 1;
            foreach (ErrTrace err in errtraces)
            {
                this.dt.Rows.Add(
                    new object[] { err.LotNo, err.BackSideMarking, err.SideMarking, err.XAddress, err.YAddress, err.ErrDetail, err.ErrInlineProNM });
        
                i++;
                if (i >= 1000)
                {
                    MessageBox.Show("検索結果が1000件を超えた為中断");
                    break;
                }
            }
        }
    }
}
