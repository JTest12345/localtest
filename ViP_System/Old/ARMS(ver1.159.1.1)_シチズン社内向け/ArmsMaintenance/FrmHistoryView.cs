using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMaintenance
{
    public partial class FrmHistoryView : Form
    {
        public FrmHistoryView()
        {
            InitializeComponent();
        }

        public FrmHistoryView(string lotno)
            : this()
        {
            this.txtLotNo.Text = lotno;
            this.rdoLotNo.Checked = true;
            search();
        }

        History[] hislist;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search();
        }


        private void search()
        {
            string lotno = null;
            DateTime? from = null;
            DateTime? to = null;

            if (rdoLotNo.Checked)
            {
                lotno = txtLotNo.Text;
                //ロット単位で検索した場合のみ確認ボタン有効
                this.btnCheckOk.Enabled = true;
            }

            if (rdoEditDt.Checked)
            {
                from = dtpEditFrom.Value;
                to = dtpEditTo.Value;
                //時間で検索した場合は確認ボタン無効
                this.btnCheckOk.Enabled = false;
            }

            hislist = History.Search(lotno, from, to);
            this.grdHistory.DataSource = hislist;

            #region ヘッダー

            foreach (DataGridViewColumn col in grdHistory.Columns)
            {
                switch (col.Name)
                {
                    case "LotNo":
                        col.HeaderText = "ロット";
                        col.DisplayIndex = 1;
                        break;

                    case "OldStartDt":
                        col.HeaderText = "開始日[前]";
                        col.DisplayIndex = 10;
                        break;

                    case "NewStartDt":
                        col.HeaderText = "開始日[後]";
                        col.DisplayIndex = 11;
                        break;

                    case "OldEndDt":
                        col.HeaderText = "終了日[前]";
                        col.DisplayIndex = 12;
                        break;

                    case "NewEndDt":
                        col.HeaderText = "終了日[後]";
                        col.DisplayIndex = 13;
                        break;

                    case "OldProcNm":
                        col.HeaderText = "工程[前]";
                        col.DisplayIndex = 6;
                        break;

                    case "NewProcNm":
                        col.HeaderText = "工程[後]";
                        col.DisplayIndex = 7;
                        break;

                    case "OldMacNm":
                        col.HeaderText = "設備[前]";
                        col.DisplayIndex = 8;
                        break;

                    case "NewMacNm":
                        col.HeaderText = "設備[後]";
                        col.DisplayIndex = 9;
                        break;

                    case "OldMagNo":
                        col.HeaderText = "マガジン[前]";
                        col.DisplayIndex = 4;
                        break;

                    case "NewMagNo":
                        col.HeaderText = "マガジン[後]";
                        col.DisplayIndex = 5;
                        break;

                    case "EmpCd":
                        col.HeaderText = "更新者";
                        col.DisplayIndex = 14;
                        break;

                    case "EditDt":
                        col.HeaderText = "更新日";
                        col.DisplayIndex = 2;
                        break;

                    case "Note":
                        col.HeaderText = "更新理由";
                        col.DisplayIndex = 15;
                        break;

                    case "Checked":
                        col.HeaderText = "確認済";
                        col.DisplayIndex = 3;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

            if (hislist.Length == 0)
            {
                this.btnCheckOk.Enabled = false;
            }
        }


        /// <summary>
        /// 確認ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckOk_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "履歴チェックの更新を行います", "確認",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            if (res != DialogResult.OK)
            {
                MessageBox.Show(this, "更新を中断しました");
                return;
            }

            try
            {
                foreach (History h in hislist)
                {
                    h.UpdateChecked(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
                return;
            }

            MessageBox.Show(this, "更新完了");
            search();
        }

        private void FrmHistoryView_Load(object sender, EventArgs e)
        {

        }
    }
}
