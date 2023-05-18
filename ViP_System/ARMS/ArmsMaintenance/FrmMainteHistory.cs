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
    public partial class FrmMainteHistory : Form
    {
        private FrmMainteHistory()
        {
            InitializeComponent();
        }

        History _history;

        /// <summary>
        /// 作業履歴更新時の履歴更新処理
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="newtran"></param>
        public FrmMainteHistory(string lotno, Order newtran) : this()
        {
            Order oldtran = Order.GetMagazineOrder(lotno, newtran.ProcNo);

            History h = new History();
            h.LotNo = lotno;
            h.EditDt = DateTime.Now;

            if (oldtran != null)
            {
                h.OldProcNo = oldtran.ProcNo;
                h.OldMacNo = oldtran.MacNo;
                h.OldStartDt = oldtran.WorkStartDt;
                h.OldEndDt = oldtran.WorkEndDt;
            }

            h.NewProcNo = newtran.ProcNo;
            h.NewMacNo = newtran.MacNo;
            h.NewStartDt = newtran.WorkStartDt;
            h.NewEndDt = newtran.WorkEndDt;
            this._history = h;

            this.txtLotNo.Text = lotno;
            this.txtOldProc.Text = h.OldProcNm != null ? h.OldProcNm : string.Empty;
            this.txtOldMac.Text = h.OldMacNm != null ? h.OldMacNm : string.Empty;
            this.txtOldStartDt.Text = h.OldStartDt.HasValue ? h.OldStartDt.ToString() : string.Empty;
            this.txtOldEndDt.Text = h.OldEndDt.HasValue ? h.OldEndDt.ToString() : string.Empty;

            this.txtNewProc.Text = h.NewProcNm != null ? h.NewProcNm : string.Empty;
            this.txtNewMac.Text = h.NewMacNm != null ? h.NewMacNm : string.Empty;
            this.txtNewStartDt.Text = h.NewStartDt.HasValue ? h.NewStartDt.ToString() : string.Empty;
            this.txtNewEndDt.Text = h.NewEndDt.HasValue ? h.NewEndDt.ToString() : string.Empty;
        }



        /// <summary>
        /// 作業実績削除時のHistory
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="newtran"></param>
        public FrmMainteHistory(string lotno, int deleteProcNo) : this()
        {
            Order oldtran = Order.GetMagazineOrder(lotno, deleteProcNo);

            History h = new History();
            h.LotNo = lotno;
            h.EditDt = DateTime.Now;

            if (oldtran != null)
            {
                h.OldProcNo = oldtran.ProcNo;
                h.OldMacNo = oldtran.MacNo;
                h.OldStartDt = oldtran.WorkStartDt;
                h.OldEndDt = oldtran.WorkEndDt;
            }

            this._history = h;

            this.txtLotNo.Text = lotno;
            this.txtOldProc.Text = h.OldProcNm != null ? h.OldProcNm : string.Empty;
            this.txtOldMac.Text = h.OldMacNm != null ? h.OldMacNm : string.Empty;
            this.txtOldStartDt.Text = h.OldStartDt.HasValue ? h.OldStartDt.ToString() : string.Empty;
            this.txtOldEndDt.Text = h.OldEndDt.HasValue ? h.OldEndDt.ToString() : string.Empty;
        }


        /// <summary>
        /// マガジン交換履歴のHistory
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="oldmag"></param>
        /// <param name="newmag"></param>
        public FrmMainteHistory(string lotno, Magazine oldmag, Magazine newmag) : this()
        {
            History h = new History();
            h.LotNo = lotno;
            h.EditDt = DateTime.Now;

            if (oldmag != null)
            {
                h.OldProcNo = oldmag.NowCompProcess;
                h.OldMagNo = oldmag.MagazineNo;
            }

            if (newmag != null && newmag.NewFg == true)
            {
                h.NewProcNo = newmag.NowCompProcess;
                h.NewMagNo = newmag.MagazineNo;
            }


            this._history = h;

            this.txtLotNo.Text = lotno;
            this.txtOldProc.Text = h.OldProcNm != null ? h.OldProcNm : string.Empty;
            this.txtOldMag.Text = h.OldMagNo;

            this.txtNewProc.Text = h.NewProcNm != null ? h.NewProcNm : string.Empty;
            this.txtNewMag.Text = h.NewMagNo;

        }



        /// <summary>
        /// 保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtEmpCd.Text))
            {
                MessageBox.Show("社員番号を入力してください");
                this.DialogResult = DialogResult.None;
                return;
            }

            int num;
            if (!int.TryParse(this.txtEmpCd.Text, out num))
            {
                MessageBox.Show("社員番号は数字を入力してください。");
                this.DialogResult = DialogResult.None;
                return;
            }

            //if (string.IsNullOrEmpty(this.txtNote.Text))
            //{
            //    MessageBox.Show("メンテナンス理由を入力してください");
            //    this.DialogResult = DialogResult.None;
            //    return;
            //}


            //履歴登録
            if (_history != null)
            {
                _history.EmpCd = num.ToString();
                _history.Note = this.txtNote.Text;
                _history.Insert();
            }
        }

		private void btnFixedPhraseInput_Click(object sender, EventArgs e)
		{
			txtNote.Text += lstFixedPhrase.SelectedValue + "\r\n";
		}

		private void FrmMainteHistory_Load(object sender, EventArgs e)
		{
			lstFixedPhrase.DataSource = FixedPhrase.GetDataMaintenance(false).Select(f => f.Text).ToList();
		}
    }
}
