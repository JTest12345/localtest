/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC130 タイプ切替
 * 
 * 概略           : テストロットのタイプを検証中にテストタイプ⇔通常タイプ切替を可能とする
 * 
 * 作成           : 2017/06/06 SLA2.Uchida
 * 
 * 修正履歴       : 2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった
 *                  2020/02/14 SLA2.Uchida 複数タイプ切替対応
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC130 : Form
    {
        private int SelRow                  = -1;       /// 2020/02/14 SLA2.Uchida 複数タイプ切替対応

        public FVDC130()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC130_Load(object sender, EventArgs e)
        {            
            try
            {
                SelRow                      = -1;       /// 2020/02/14 SLA2.Uchida 複数タイプ切替対応

                this.dsFVDC130              = new dsFVDC130();

                /// レジストリより保存していた情報を取得
                string strTestLot           = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);

                /// データが無いとき処理を抜ける
                if ((strTestLot == null) || (strTestLot == "")) return;

                /// ロットに割当たっているタイプを検索する
                string WhereSql             = " WHERE lotno IN('" + strTestLot.Replace(",", "','") + "')";
                DsName dsLotType            = new DsName();
                CommonRead objCommonRead    = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    objCommonRead.NameRead(sqlInfo, " TnLot", "lotno", "typecd", WhereSql, false, ref dsLotType);
                }

                /// タイプ選択リストの作成
                ///↓↓↓↓↓↓　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↓↓↓↓↓↓
                //this.dsType.Name.Rows.Clear();
                //string TipeList             = "";
                //for (int i = 0; i < dsLotType.Name.Rows.Count; i++)
                //{
                //    string TypeNM           = dsLotType.Name[i].Data_NM.Replace("_TEST", "");
                //    if (!TipeList.Contains(TypeNM))
                //    {
                //        TipeList            += "," + TypeNM;
                //        this.dsType.Name.Rows.Add(TypeNM);
                //        this.dsType.Name.Rows.Add(TypeNM + "_TEST");
                //    }
                //}
                //string TypeNM               = dsLotType.Name[0].Data_NM.Replace("_TEST", "");
                //this.dsType.Name.Rows.Clear();
                //this.dsType.Name.Rows.Add(TypeNM);
                //this.dsType.Name.Rows.Add(TypeNM + "_TEST");
                ///↑↑↑↑↑↑　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↑↑↑↑↑↑

                /// グリッドデータに連携する
                for (int i = 0; i < dsLotType.Name.Rows.Count; i++)
                {
                    this.dsFVDC130.LotType.Rows.Add(
                        dsLotType.Name[i].Key_CD
                        , dsLotType.Name[i].Data_NM
                        , dsLotType.Name[i].Data_NM
                        );
                }
                this.dgvLotType.DataSource  = this.dsFVDC130;
            }
            catch { }
        }

        ///↓↓↓↓↓↓　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↓↓↓↓↓↓
        /// <summary>
        /// 行の変更が有ったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dgvLotType_RowValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    /// タイプが最初に表示された内容と違うか新旧比較する
        //    int ix                                      = e.RowIndex;
        //    if (this.dgvLotType["Type_New", ix].Value.ToString() == this.dgvLotType["Type_Old", ix].Value.ToString())
        //    {
        //        /// 同じとき操作をクリアする
        //        this.dgvLotType["TexAction", ix].Value  = "";
        //    }
        //    else
        //    {
        //        /// 違うとき操作に『切替』をセットする
        //        this.dgvLotType["TexAction", ix].Value  = "切替";
        //    }
        //}
        ///↑↑↑↑↑↑　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↑↑↑↑↑↑
       
        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            /// 画面を閉じる
            this.Close();
        }

        /// <summary>
        /// ＯＫボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.dgvLotType.RowCount < 1) return;

            /// メッセージを表示する
            DialogResult OkNgButton = MessageBox.Show("　行タイトルが『切替』となっているロットのタイプを切り替えます。"
                            + "\n\n　テストタイプのままではNASCA連携が出来ませんので"
                            + "\n　NASCA連携が必要なロットは元に戻して下さい。"
                            + "\n\n　そのまま切替を続行する場合にはＯＫを押して下さい。", "タイプ切替確認",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            /// キャンセルが押されたとき、処理を終了する
            if (OkNgButton == System.Windows.Forms.DialogResult.Cancel) return;

            CommonIO objCommonIO        = new CommonIO();
            /// 表の行数分繰り返す
            for (int i = 0; i < this.dgvLotType.RowCount; i++)
            {
                /// 切替で空白でない行のとき
                if ((this.dgvLotType["TexAction", i].Value != null)
                    && (this.dgvLotType["TexAction", i].Value.ToString().Contains("切替")))
                {
                    /// ロットテーブルの対象ロットのタイプを変更する
                    string UpdateChar   = " typecd = '" + this.dgvLotType["Type_New", i].Value.ToString() + "' ";
                    string WhereChar    = " WHERE lotno = '" + this.dgvLotType["Lot_No", i].Value.ToString() + "' ";
                    objCommonIO.Update(Constant.StrLENSDB, "TnLot", UpdateChar.Replace("typecd", "TypeCD"), WhereChar.Replace("lotno", "LotNO"));  /// 2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった
                    if (objCommonIO.Update(Constant.StrARMSDB, "TnLot", UpdateChar, WhereChar))
                    {
                        /// グリッドの情報を最新に変更する
                        this.dgvLotType["TexAction", i].Value   = "";
                        this.dgvLotType["Type_Old", i].Value    = this.dgvLotType["Type_New", i].Value.ToString();
                        /// 画面を閉じる
                        this.Close();
                    }
                }
            }
        }

        ///↓↓↓↓↓↓　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↓↓↓↓↓↓
        /// <summary>
        /// グリッドの右クリックメニューを表示する直前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsDataGridView_Opening(object sender, CancelEventArgs e)
        {
            if (SelRow == -1) return;

            string TypeName                         = this.dgvLotType["Type_Old", SelRow].Value.ToString().Replace("_TEST","");
            string TestTypeName                     = TypeName + "_TEST";

            this.cmsDataGridView.Items.Clear();

            /// 選択リストに追加
            ToolStripMenuItem tsmItem1              = new ToolStripMenuItem();
            tsmItem1.Name                           = TypeName;
            tsmItem1.Text                           = TypeName;
            this.cmsDataGridView.Items.AddRange(new ToolStripItem[] { tsmItem1 });

            ToolStripMenuItem tsmItem2              = new ToolStripMenuItem();
            tsmItem2.Name                           = TestTypeName;
            tsmItem2.Text                           = TestTypeName;
            this.cmsDataGridView.Items.AddRange(new ToolStripItem[] { tsmItem2 });

            this.cmsDataGridView.Size               = new System.Drawing.Size(150, 48);
        }

        /// <summary>
        /// グリッドの右クリックメニューを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsDataGridView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            if (SelRow == -1) return;

            this.dgvLotType["Type_New", SelRow].Value           = e.ClickedItem.Name;

            /// タイプが最初に表示された内容と違うか新旧比較する
            if (this.dgvLotType["Type_Old", SelRow].Value.ToString() == e.ClickedItem.Name)
            {
                this.dgvLotType["TexAction", SelRow].Value      = "";
            }
            else
            {
                this.dgvLotType["TexAction", SelRow].Value      = "切替";
            }
            this.dgvLotType.UpdateCellValue(0, SelRow);
            this.dgvLotType.UpdateCellValue(2, SelRow);
            this.dgvLotType.RefreshEdit();
            this.dgvLotType.EndEdit();
            this.dgvLotType.Refresh();
            this.dgvLotType.Update();
        }

        /// <summary>
        /// グリッド位置の取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelRow                      = e.RowIndex;
        }

        private void dataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            SelRow                      = e.RowIndex;
        }

        ///↑↑↑↑↑↑　2020/02/14 SLA2.Uchida 複数タイプ切替対応　↑↑↑↑↑↑
    }
}
