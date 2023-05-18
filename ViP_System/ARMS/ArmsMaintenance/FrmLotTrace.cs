using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmLotTrace : Form
    {
        /// <summary>
        /// 検索データ保存テーブル
        /// </summary>
        private DataTable dt;

        private List<Material> selectedMatList = new List<Material>();

        private List<string> selectedLotList = new List<string>();

        public FrmLotTrace()
        {
            InitializeComponent();

            dt = new DataTable();
            dt.Columns.Add("colChk", typeof(bool));
            dt.Columns.Add("LotNo", typeof(string));
            dt.Columns.Add("TypeCd", typeof(string));
            dt.Columns.Add("ProcNm", typeof(string));
            dt.Columns.Add("CutBlendCd", typeof(string));
            dt.Columns.Add("MagNo", typeof(string));
            dt.Columns.Add("MacNm", typeof(string));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            if (rdoLotSearch.Checked == true)
            {
                traceFromMatLot();
            }
            else if (rdoResinSearch.Checked == true)
            {
                traceFromResin();
            }
            else
            {
                traceFromLot();
            }

            this.grdLotList.DataSource = dt;

            #region グリッド表示列変更

            foreach (DataGridViewColumn col in this.grdLotList.Columns)
            {
                switch (col.Name)
                {
                    case "colChk":
                        col.HeaderText = "選択";
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット番号";
                        col.ReadOnly = true;
                        break;

                    case "TypeCd":
                        col.HeaderText = "タイプ";
                        col.ReadOnly = true;
                        break;

                    case "ProcNm":
                        col.HeaderText = "現在完了工程";
                        col.ReadOnly = true;
                        break;

                    case "CutBlendCd":
                        col.HeaderText = "カットブレンドグループ";
                        col.ReadOnly = true;
                        break;

                    case "MagNo":
                        col.HeaderText = "マガジン";
                        col.ReadOnly = true;
                        break;

                    case "MacNm":
                        col.HeaderText = "使用設備";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

        }




        private void traceFromResin()
        {
            this.dt.Clear();
            SortedList<int, MachineInfo> maclist = new SortedList<int, MachineInfo>();

            string mixresultid = txtMixResultId.Text;
            if (string.IsNullOrEmpty(mixresultid)) return;

            Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
            foreach (Magazine mag in maglist)
            {
                Order[] orders = Order.GetOrder(mag.NascaLotNO);
                foreach (Order ord in orders)
                {
                    if (!maclist.Keys.Contains(ord.MacNo))
                    {
                        MachineInfo m = MachineInfo.GetMachine(ord.MacNo, true);
                        if (m != null)
                        {
                            maclist.Add(m.MacNo, m);
                        }
                        else 
                        {
                            //マスタに無い装置が使用されていた場合はトレース結果の表示から省く
                            continue;
                        }
                    }

                    MachineInfo mac = maclist[ord.MacNo];
                    Resin[] resins = ord.GetResins();
                    int ct = resins.Where(r => r.MixResultId == mixresultid).Count();
                    if (ct >= 1)
                    {
                        AsmLot lot = AsmLot.GetAsmLot(ord.LotNo);
                        dt.Rows.Add(new object[] { false, lot.NascaLotNo, lot.TypeCd, mag.NowCompProcessNm, lot.CutBlendCd, mag.MagazineNo, mac.LongName });
                        break;
                    }
                }
            }

            return;
        }


        private void traceFromMatLot()
        {
            this.dt.Clear();

            if (this.selectedMatList.Count == 0) return;

            Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
            foreach (Magazine mag in maglist)
            {
                Order[] orders = Order.GetOrder(mag.NascaLotNO);
                foreach (Order ord in orders)
                {
                    Material[] matlist = ord.GetMaterials();

                    bool isExists = false;

                    //2012/9/3複数指定対応
                    foreach (Material mat in this.selectedMatList)
                    {
                        int ct = matlist.Where(m => m.MaterialCd == mat.MaterialCd && m.LotNo == mat.LotNo).Count();
                        if (ct >= 1)
                        {
                            MachineInfo mac = MachineInfo.GetMachine(ord.MacNo, true);
                            //マスタに無い装置が使用されていた場合はトレース結果として扱わない
                            if (mac == null) continue;

                            AsmLot lot = AsmLot.GetAsmLot(ord.LotNo);
                            dt.Rows.Add(new object[] { false, lot.NascaLotNo, lot.TypeCd, mag.NowCompProcessNm, lot.CutBlendCd, mag.MagazineNo, mac.LongName });
                            isExists = true;
                            break;
                        }
                    }

                    if (isExists == true) break;
                }
            }

            return;
        }


        private void traceFromLot()
        {
            this.dt.Clear();

            if (string.IsNullOrEmpty(this.txtAsmLotNo.Text) == false)
                this.selectedLotList.Add(this.txtAsmLotNo.Text);

            if (this.selectedLotList.Count == 0) return;

            //1ロットずつ取得
            foreach (string asmLot in this.selectedLotList)
            {
                Magazine[] maglist = Magazine.GetMagazine(null, asmLot, true, null);
                foreach (Magazine mag in maglist)
                {
                    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

                    Order[] orders = Order.GetOrder(lot.NascaLotNo, mag.NowCompProcess);

                    foreach (Order o in orders)
                    {
                        MachineInfo mac = MachineInfo.GetMachine(o.MacNo, true);

                        //マスタに無い装置が使用されていた場合はトレース結果として扱わない
                        if (mac == null) continue;

                        dt.Rows.Add(new object[] { false, o.NascaLotNo, lot.TypeCd, mag.NowCompProcessNm, lot.CutBlendCd, mag.MagazineNo, mac.LongName });
                    }
                }
            }
        }


        private void btnSelectMat_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMaterial frm = new FrmSelectMaterial(true, this.selectedMatList);
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.selectedMatList = frm.SelectedMatList;
                }

                if (this.selectedMatList.Count >= 1)
                {
                    this.btnSelectMat.BackColor = Color.LawnGreen;
                }
                else
                {
                    this.btnSelectMat.BackColor = SystemColors.Control;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmLotTrace_Load(object sender, EventArgs e)
        {
            Process[] procs = Process.GetWorkflowProcess();
            this.cmbRestrictProc.DataSource = procs;
            this.cmbRestrictProc.DisplayMember = "InlineProNM";
        }

        private void btnAddRestrict_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtRestrictNote.Text))
                {
                    MessageBox.Show("規制理由が入力されていません");
                    return;
                }
                else if (this.txtRestrictNote.Text.Length > 200)
                {
                    MessageBox.Show("規制理由は200以下にしてください");
                    return;
                }

                int restrictEmpId = 0;

                if (string.IsNullOrEmpty(txtRestrictEmp.Text))
                {
                    MessageBox.Show("社員番号が入力されていません");
                    return;
                }
                else if (int.TryParse(txtRestrictEmp.Text, out restrictEmpId) == false)
                {
                    MessageBox.Show("社員番号は数字のみで入力して下さい");
                    return;
                }


                //選択行だけに絞り込み
                List<DataGridViewRow> selected = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in this.grdLotList.Rows)
                {
                    if ((bool)row.Cells["colChk"].Value == true)
                    {
                        selected.Add(row);
                    }
                }
                if (selected.Count == 0)
                {
                    MessageBox.Show("ロットが選択されていません");
                    return;
                }

                Process proc = (Process)this.cmbRestrictProc.SelectedItem;

                string msg = string.Format("全{0}行に{1}工程への投入規制を設定します", selected.Count, proc.InlineProNM);
                DialogResult res = MessageBox.Show(this, msg, "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
                if (res != DialogResult.OK) return;


                foreach (DataGridViewRow row in selected)
                {
                    Restrict rs = new Restrict();
                    rs.LotNo = row.Cells["LotNo"].Value.ToString();
                    rs.ProcNo = proc.ProcNo;
                    rs.Reason = txtRestrictNote.Text;
                    rs.DelFg = false;
                    rs.LastUpdDt = DateTime.Now;
                    rs.UpdUserCd = txtRestrictEmp.Text;
                    rs.RestrictReleaseFg = ckbRestrictRelease.Checked;
                    rs.Save();
                }

                MessageBox.Show("規制完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelectAsmLot_Click(object sender, EventArgs e)
        {
            try
            {
                FrmInputGrid frm = new FrmInputGrid(this.selectedLotList);
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.selectedLotList = frm.ValList;

                    if (frm.ValList.Count > 0)
                        this.btnSelectAsmLot.ImageIndex = 0;
                    else
                        this.btnSelectAsmLot.ImageIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
