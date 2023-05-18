using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    public partial class FrmVirtualMagMainte : Form
    {
        public FrmVirtualMagMainte()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            updateGrid();
        }

        private void updateGrid()
        {
            List<VirtualMag> virtualMagList
                = VirtualMag.GetVirtualMag(this.txtMacNo.Text, string.Empty, this.getMagazineNoSearchChar()).ToList();

            List<VirtualMag> vMagList = new List<VirtualMag>();

            if (string.IsNullOrWhiteSpace(txtLineNo.Text) == false)
            {
                string[] lineArray = txtLineNo.Text.Split(',');
                List<MachineInfo> macList = new List<MachineInfo>();
                foreach(string line in lineArray.Distinct())
                {
                    MachineInfo[] macArray = MachineInfo.SearchMachine(null, null, true, false, false, null, null, line);
                    macList.AddRange(macArray.ToList());
                }
                vMagList = virtualMagList.Where(v => macList.Any(m => m.MacNo == v.MacNo)).ToList();
            }
            else
            {
                vMagList = virtualMagList;
            }

            bsItems.DataSource = vMagList;
            //bsItems.DataSource = virtualMagList;

            List<VirtualMag> pMags = priorityMagazine();
            foreach (DataGridViewRow row in grdVirtualMag.Rows)
            {
                if (pMags.Exists(m => m.MagazineNo == row.Cells["MagazineNo"].Value.ToString() 
                    && m.MacNo == Convert.ToInt32(row.Cells["Macno"].Value)
                    && m.LocationId == Convert.ToInt32(row.Cells["locationid"].Value)))
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;            
                }
            }
            

            //this.grdVirtualMag.DataSource = virtualMagList;

            //this.grdVirtualMag.Columns["CurrentLocation"].Visible = false;
            //this.grdVirtualMag.Columns["Macno"].Visible = false;
            //this.grdVirtualMag.Columns["LocationId"].Visible = false;
            //for (int i = 0; i < this.grdVirtualMag.Columns.Count; i++)
            //{
            //    this.grdVirtualMag.Columns[i].ReadOnly = true;
            //}

            //this.grdVirtualMag.Columns["Check"].HeaderText = "チェック";
            //this.grdVirtualMag.Columns["Check"].ReadOnly = false;
            //this.grdVirtualMag.Columns["ClassNm"].HeaderText = "装置";
            //this.grdVirtualMag.Columns["PlantNm"].HeaderText = "号機";
            //this.grdVirtualMag.Columns["LocationNm"].HeaderText = "位置";
            //this.grdVirtualMag.Columns["orderid"].HeaderText = "優先順序";
            //this.grdVirtualMag.Columns["MagazineNo"].HeaderText = "マガジンNO";
            //this.grdVirtualMag.Columns["LastMagazineNo"].HeaderText = "元マガジンNO";
            //this.grdVirtualMag.Columns["ProcNo"].HeaderText = "工程NO";
            //this.grdVirtualMag.Columns["WorkStart"].HeaderText = "作業開始日時";
            //this.grdVirtualMag.Columns["WorkComplete"].HeaderText = "作業完了日時";
            //this.grdVirtualMag.Columns["NextMachinesString"].HeaderText = "次装置";
            //this.grdVirtualMag.Columns["RelatedMaterialsString"].HeaderText = "使用原材料";
            //this.grdVirtualMag.Columns["FrameMatCd"].HeaderText = "フレーム品目CD(MAP)";
            //this.grdVirtualMag.Columns["FrameLotNo"].HeaderText = "フレームロットNO(MAP)";
            //this.grdVirtualMag.Columns["CurrentFrameCt"].HeaderText = "現在フレーム格納数(MAP)";
            //this.grdVirtualMag.Columns["MaxFrameCt"].HeaderText = "最大フレーム格納数(MAP)";
            //this.grdVirtualMag.Columns["StockerStartCol"].HeaderText = "ストッカー開始段数";
            //this.grdVirtualMag.Columns["StockerEndCol"].HeaderText = "ストッカー終了段数";
            //this.grdVirtualMag.Columns["StockerChangeCt"].HeaderText = "ストッカー交換回数";
            //this.grdVirtualMag.Columns["LastStockerChange"].HeaderText = "ストッカー最終更新日時";
            //this.grdVirtualMag.Columns["Stocker1"].HeaderText = "ストッカー1使用段数";
            //this.grdVirtualMag.Columns["Stocker2"].HeaderText = "ストッカー2使用段数";
            //this.grdVirtualMag.Columns["StartWafer"].HeaderText = "ウェハー開始段数";
            //this.grdVirtualMag.Columns["EndWafer"].HeaderText = "ウェハー終了段数";
            //this.grdVirtualMag.Columns["WaferChangerChangeCount"].HeaderText = "ウェハー交換回数";
            //this.grdVirtualMag.Columns["WaferChangerChangeTime"].HeaderText = "ウェハー交換日時";
            //this.grdVirtualMag.Columns["MapAoiMagazineLotNo"].HeaderText = "マガジンロット番号(AOIMAP)";
            //this.grdVirtualMag.Columns["PurgeReason"].HeaderText = "排出理由";
            //this.grdVirtualMag.Columns["Origin"].HeaderText = "製造ライン";
            //this.grdVirtualMag.Columns["ProgramTotalMinutes"].HeaderText = "プログラム運転時間";
            //this.grdVirtualMag.Columns["LastUpdDt"].HeaderText = "更新日時";
        }

        /// <summary>
        /// 装置選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMachine_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMachine frmmac = new FrmSelectMachine();
                DialogResult res = frmmac.ShowDialog();

                if (res == DialogResult.OK)
                {
                    MachineInfo m = frmmac.SelectedMachine;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            grdVirtualMag.EndEdit();

            try
            {
                List<VirtualMag> deleteList = new List<VirtualMag>();

                foreach (DataGridViewRow row in this.grdVirtualMag.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value == true)
                    {
                        VirtualMag v = (VirtualMag)row.DataBoundItem;

                        FrmPasswordDialog frm = new FrmPasswordDialog(
                            v.MagazineNo,
                            v.MagazineNo + " の仮想マガジンデータを削除します。\r\n本当に削除する場合は入力欄に" + v.MagazineNo + "と入力してください。");
                        DialogResult res = frm.ShowDialog();

                        if (res == DialogResult.OK)
                        {
                            v.Delete();

                            if (deleteList.Exists(d => d.MacNo == v.MacNo && d.LocationId == v.LocationId) == false)
                            {
                                deleteList.Add(v);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                //投入順番を再割り当て
                foreach (VirtualMag d in deleteList)
                {
                    d.updateOrder();
                }

                updateGrid();
                MessageBox.Show("削除完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// マガジン排出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMagazineDischarge_Click(object sender, EventArgs e)
        {
            grdVirtualMag.EndEdit();
                        
            try
            {
                List<VirtualMag> mags = selectMagazine();
                if (mags.Count == 0) 
                {
                    return;
                }
                
                if (DialogResult.OK != MessageBox.Show("選択マガジンを排出CV搬送します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }
                
                foreach (VirtualMag mag in mags)
                {
                    mag.NextMachines.Clear();
                    mag.NextMachines.Add(Route.GetDischargeConveyor(mag.MacNo));

                    VirtualMag.UpdateNextMachines(mag);
                }

                MessageBox.Show("設定完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                updateGrid();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\r\n" + err.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void grdVirtualMag_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) { return; }

            DataGridViewCell selectCell = grdVirtualMag.Rows[e.RowIndex].Cells["SelectFG"];
            if (e.ColumnIndex == selectCell.ColumnIndex)
            {
                return;
            }

            if (Convert.ToBoolean(selectCell.Value))
            {
                selectCell.Value = false;
            }
            else 
            {
                selectCell.Value = true;
            }
        }

        /// <summary>
        /// 優先搬送設定を保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrioritySave_Click(object sender, EventArgs e)
        {
            grdVirtualMag.EndEdit();

            if (grdVirtualMag.SelectedRows.Count == 0)
            {
                return;
            }

            if (DialogResult.OK != MessageBox.Show("選択マガジンに優先搬送設定をします。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)) 
            {
                return;      
            }

            try
            {
                List<VirtualMag> mags = selectMagazine();
                foreach (VirtualMag mag in mags)
                {
                    mag.PriorityFg = true;

                    VirtualMag.UpdatePriority(mag);
                }

                MessageBox.Show("設定完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                updateGrid();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\r\n" + err.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 選択中マガジン取得
        /// </summary>
        private List<VirtualMag> selectMagazine()
        {
            List<VirtualMag> mags = new List<VirtualMag>();
            foreach (DataGridViewRow row in grdVirtualMag.Rows)
            {
                if (Convert.ToBoolean(row.Cells["SelectFG"].Value))
                {
                    mags.Add((VirtualMag)bsItems.List[row.Index]);
                }
            }
            return mags;
        }
        
        /// <summary>
        /// 優先搬送マガジン取得
        /// </summary>
        /// <returns></returns>
        private List<VirtualMag> priorityMagazine() 
        {
            List<VirtualMag> mags = (List<VirtualMag>)bsItems.DataSource;
            return mags.FindAll(m => m.PriorityFg).ToList();
        }

        /// <summary>
        /// 検索条件のマガジンNO取得
        /// </summary>
        /// <returns></returns>
        private string getMagazineNoSearchChar()
        {
            string magNo = string.Empty;
            if (!string.IsNullOrEmpty(txtMagNo.Text.Trim()))
            {
                string[] magChar = txtMagNo.Text.Trim().Split(' ');
                if (magChar.Count() >= 2)
                {
                    magNo = magChar[1];
                }
                else
                {
                    magNo = magChar[0];
                }
            }
            return magNo;
        }

        // 改善：次装置の情報の表示
        private void grdVirtualMag_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (grdVirtualMag.Columns[e.ColumnIndex].Name == "NextMachinesInfoOpenButton")
                {
                    VirtualMag mag = (VirtualMag)bsItems.List[e.RowIndex];
                    if (mag.NextMachines.Any() == true)
                    {
                        FrmMacMaster frm = new FrmMacMaster(false, mag.NextMachines);
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("次装置が存在しないマガジンが押されました。マガジンNo=" + mag.MagazineNo);
                    }
                }
            }
            catch { }
        }
    }
}
