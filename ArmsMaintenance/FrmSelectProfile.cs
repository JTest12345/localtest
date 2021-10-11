using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using System.Threading.Tasks;

namespace ArmsMaintenance
{
    public partial class FrmSelectProfile : Form
    {
        /// <summary>
        /// 他画面からのプロファイル選択用の呼び出し
        /// </summary>
        protected bool selectionDialogMode;


        /// <summary>
        /// 選択モード時の選択結果
        /// </summary>
        public Profile Selected;

        public FrmSelectProfile()
        {
            InitializeComponent();
        }

        public FrmSelectProfile(bool selectionMode)
        {
            this.selectionDialogMode = selectionMode;
            InitializeComponent();
            this.btnSetActive.Text = "選択";
        }


        private void FrmSelectProfile_Load(object sender, EventArgs e)
        {
            // <!--【改修1.133.0】
            
            // 『ライン』コンボボックスの要素 (プロファイルマスタから取得。空白, "高生産性"を先頭にする。他は昇順に並び替える)
            Profile[] profList = Profile.SearchProfiles(null, null, false, false);
            string[] lineArray = profList.Where(l => string.IsNullOrWhiteSpace(l.LineNo) == false)
                                        .Where(l => l.LineNo != ArmsApi.Config.LINENAME_HIGHLINE)
                                        .Select(p => p.LineNo)
                                        .Distinct().OrderBy(p => p).ToArray();
            this.cmbLineNoCondition.Items.Add(string.Empty);
            this.cmbLineNoCondition.Items.Add(ArmsApi.Config.LINENAME_HIGHLINE);
            this.cmbLineNoCondition.Items.AddRange(lineArray);

            // 【改修1.133.0】 -->
        }

        private async void ProfileSearch(string typeCd, string dbThrowDt, string resinGroupCd, string lineNo, string mnfctKb, bool newFg)
        {
            if (newFg)
            {
                if (DialogResult.OK != MessageBox.Show("「現在流動中のプロファイルのみ表示」は検索に時間がかかります。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }
            }

            Profile[] profList = null;

            toolStripStatusLabel.Text = "検索中です。暫くお待ちください。";
            btnSearch.Enabled = false;
            btnSetActive.Enabled = false;

            await Task.Run(() =>
            {
                // ①プロファイルリストの取得
                profList = Profile.SearchProfiles(null, null, false, false);
                // <!--【改修1.133.0】
                // ②検索条件のフィルタ
                if (string.IsNullOrWhiteSpace(typeCd) == false)
                {
                    profList = profList.Where(p => p.TypeCd.ToUpper().StartsWith(typeCd.ToUpper())).ToArray();
                }
                if (string.IsNullOrEmpty(dbThrowDt) == false)
                {
                    profList = profList.Where(p => p.DBThrowDt.ToUpper().Contains(dbThrowDt.ToUpper())).ToArray();
                }
                if (string.IsNullOrEmpty(resinGroupCd) == false)
                {
                    profList = profList.Where(p => p.ResinGpCd.Contains(resinGroupCd)).ToArray();
                }
                if (string.IsNullOrEmpty(lineNo) == false)
                {
                    profList = profList.Where(p => p.LineNo == lineNo).ToArray();
                }
                if (string.IsNullOrEmpty(mnfctKb) == false)
                {
                    profList = profList.Where(p => p.MnfctKb == mnfctKb).ToArray();
                }
                // 【改修1.133.0】 -->

                if (newFg)
                {
                    Magazine[] mags = Magazine.GetMagazine(true);
                    List<int> newProfList = mags.Select(m => AsmLot.GetAsmLot(m.NascaLotNO).ProfileId).Distinct().ToList();

                    profList = profList.Where(p => newProfList.Contains(p.ProfileId)).ToArray();
                }
            });

            this.grdProfiles.DataSource = profList;

            foreach (DataGridViewColumn col in this.grdProfiles.Columns)
            {
                switch (col.Name)
                {
                    #region 列設定

                    case "colActive":
                        break;

                    case "ProfileId":
                        col.HeaderText = "プロファイルNo";
                        col.ReadOnly = true;
                        break;

                    case "ProfileNm":
                        col.HeaderText = "プロファイル名";
                        col.ReadOnly = true;
                        break;

                    case "TypeCd":
                        col.HeaderText = "タイプ";
                        col.ReadOnly = true;
                        break;

                    case "BlendCd":
                        col.HeaderText = "ブレンドコード";
                        col.ReadOnly = true;
                        break;

                    case "ResinGpCd":
                        col.HeaderText = "樹脂Gr";
                        col.ReadOnly = true;
                        break;

                    case "CutBlendCd":
                        col.HeaderText = "カットブレンド";
                        col.ReadOnly = true;
                        break;

                    case "AimRank":
                        col.HeaderText = "狙いランク";
                        col.ReadOnly = true;
                        break;

                    case "MnfctKb":
                        col.HeaderText = "製造区分";
                        col.ReadOnly = true;
                        break;

                    case "TrialNo":
                        col.HeaderText = "指示書番号";
                        col.ReadOnly = true;
                        break;

                    case "LotSize":
                        col.HeaderText = "ロットサイズ";
                        col.ReadOnly = true;
                        break;

                    case "IsCurrent":
                        col.DisplayIndex = 0;
                        col.HeaderText = "有効";
                        col.Frozen = true;
                        break;

                    case "ScheduleSelectionStandard":
                        col.HeaderText = "予定選別規格";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                        #endregion
                }
            }

            btnSearch.Enabled = true;
            btnSetActive.Enabled = true;
            toolStripStatusLabel.Text = "";
        }

        private void btnSetActive_Click(object sender, EventArgs e)
        {
            try
            {
                int currentct = 0;
                Profile current = null;

                foreach (DataGridViewRow  row in this.grdProfiles.Rows)
                {
                    Profile p = (Profile)row.DataBoundItem;

                    if (p.IsCurrent == true)
                    {
                        currentct++;
                        current = p;
                    }
                    
                }

                if (currentct == 0)
                {
                    MessageBox.Show("有効なプロファイルが選択されていません");
                    return;
                }
                else if (currentct >= 2)
                {
                    MessageBox.Show("複数のプロファイルが選ばれています");
                    return;
                }

                if (selectionDialogMode == false)
                {
                    DialogResult res = MessageBox.Show(this, "プロファイルを変更します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    if (res != DialogResult.OK) return;

                    Profile.SetActive(current.ProfileId);

                    MessageBox.Show("更新完了");
                }
                else
                {
                    DialogResult res = MessageBox.Show(this, "このプロファイルを選択しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    if (res != DialogResult.OK) return;
                    Selected = current;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void grdProfiles_SelectionChanged(object sender, EventArgs e)
        {
            Profile p = null;
            if (grdProfiles.SelectedRows.Count >= 1)
            {
                p = (Profile)this.grdProfiles.SelectedRows[0].DataBoundItem;
            }

            if (p == null) return;

            this.txtProfileNo.Text = p.ProfileId.ToString();
            this.txtProfileNm.Text = p.ProfileNm;
            this.txtTypeCd.Text = p.TypeCd;
            this.txtResinGr.Text = string.Join(",", p.ResinGpCd);
            this.txtMnfctKb.Text = p.MnfctKb;
            this.txtLotSize.Text = p.LotSize.ToString();
            
            string s = null;
            foreach (int isp in p.InspectionProcs)
            {
                Process proc = Process.GetProcess(isp);
                s += (proc.InlineProNM + " ");
            }
            this.txtInspection.Text = s;

            this.txtBlendCd.Text = p.BlendCd;
            this.txtAimRank.Text = p.AimRank;
            this.txtLastUpdDt.Text = p.LastUpdDt.ToString();
            this.txtDbThrowDt.Text = p.DBThrowDt;
            // Ver1.99.0 予定選別規格 追加
            this.txtScheduleSelectionStandard.Text = p.ScheduleSelectionStandard;

            BOM[] bomlist = Profile.GetBOM(p.ProfileId);

            this.grdBOM.DataSource = bomlist;

            foreach (DataGridViewColumn col in this.grdBOM.Columns)
            {
                
                switch (col.Name)
                {
                    case "LotCharName":
                        col.HeaderText = "区分名称";
                        col.ReadOnly = true;
                        col.DisplayIndex = 1;
                        break;

                    case "MaterialCd":
                        col.HeaderText = "品目";
                        col.ReadOnly = true;
                        col.DisplayIndex = 2;
                        break;

                    case "MaterialName":
                        col.HeaderText = "品目名称";
                        col.DisplayIndex = 3;
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileSearch(txtTypeCdCondition.Text, txtDbThrowDtCondition.Text, txtResinGroupCondition.Text, cmbLineNoCondition.Text,
                    cmbMnfctKbCondition.Text, chkNewProfile.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
