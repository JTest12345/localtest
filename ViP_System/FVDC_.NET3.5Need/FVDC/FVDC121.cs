/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC121 プロファイル選択
 * 
 * 概略           : 検証用プロファイルを表示して選択したプロファイルよりロットを選択可能とする
 * 
 * 作成           : 2017/04/04 SLA2.Uchida
 * 
 * 修正履歴       : 
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
    public partial class FVDC121 : Form
    {
        public FVDC121()
        {
            InitializeComponent();
        }

        private void FVDC121_Load(object sender, EventArgs e)
        {
            
            /// 検証用プロファイルを検索する
            CommonRead objCommonRead                        = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                dsProfile                                   = new DsName();
                string WhereSql;
                WhereSql                                    = " WHERE (profilenm LIKE '%検証%') OR  (profilenm LIKE N'%禁止%')"
                                                            + " OR (profilenm LIKE '%テスト%')  OR  (profilenm LIKE N'%てすと%')"
                                                            + " OR (profilenm LIKE '%ダミー%')  OR  (profilenm LIKE N'%DUMMY%')"
                                                            + " OR (profilenm LIKE '%TEST%')  OR  (profilenm LIKE N'%ＴＥＳＴ%')"
                                                            + " OR (profilenm LIKE '%Jobショップライン%') ORDER BY profileid DESC ";
                if (objCommonRead.TopNameRead(sqlInfo, "", "TmProfile", "profileid", "profilenm", WhereSql, false, ref dsProfile))
                {
                    this.dgvProfile.DataSource              = this.dsProfile;
                }
            }
            
        }
        /// <summary>
        /// セルのボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            System.Windows.Forms.DataGridView dataGridView  = new DataGridView();
            dataGridView                                    = (DataGridView)sender;
            try
            {
                string profileid                            = dataGridView[1, e.RowIndex].Value.ToString();
                string WhereSql;
                DsName dsLot                                = new DsName();
                /// 検証用ロットを検索する
                CommonRead objCommonRead                    = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    WhereSql                                = " WHERE (profileid = " + profileid + ") ";
                    if (objCommonRead.TopNameRead(sqlInfo, "", "TnLot", "lotno", "lotno", WhereSql, false, ref dsLot))
                    {
                        this.dsFVDC120.FVDC120.Rows.Clear();
                        for (int i = 0; i < dsLot.Name.Rows.Count; i++)
                        {
                            this.dsFVDC120.FVDC120.Rows.Add(dsLot.Name[i].Data_NM);
                        }
                        this.dgvLot.Update();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 全選択を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFullON_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dgvLot.RowCount - 1; i++)
            {
                /// チェックをＯＮにする
                this.dgvLot["chkLine", i].Value     = true;
            }
        }
        /// <summary>
        /// 全解除を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFullOFF_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dgvLot.RowCount; i++)
            {
                /// チェックをＯＦＦにする
                this.dgvLot["chkLine", i].Value     = false;
            }
        }

        /// <summary>
        /// ＯＫボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            /// レジストリより保存していた情報を取得
            string strTestLot       = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);

            /// データが無いとき処理を抜ける
            if ((strTestLot == null) || (strTestLot == ""))
            {
                strTestLot          = "@";
            }
            
            /// 表の行数分繰り返す
            for (int i = 0; i < this.dgvLot.RowCount - 1; i++)
            {
                /// チェックＯＮで空白でなく存在しない行のとき
                if ((Convert.ToBoolean(this.dgvLot["chkLine", i].Value))
                    && (this.dgvLot["Lot_NO", i].Value.ToString().Trim() != "")
                    && (!strTestLot.Contains(this.dgvLot["Lot_NO", i].Value.ToString().Trim())))
                {
                    /// レジストリの情報に追加する
                    strTestLot      += "," + this.dgvLot["Lot_NO", i].Value.ToString().Trim();
                }
            }
            strTestLot = strTestLot.Replace("@,,", "").Replace("@,", "").Replace(",,", ",").Replace("@", "");

            /// 入力された内容をレジストリに登録します。
            fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot, (object)strTestLot);

            dsFVDC120.FVDC120.Rows.Clear();
            this.Close();
        }
        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
