/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC310 設備番号複数選択
 * 
 * 概略           : 設備番号をクリップボードより受取り複数選択可能とする
 * 
 * 作成           : 2020/01/28 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC310 : Form
    {
        bool CtrlFlg                        = false;
        public FVDC310()
        {
            InitializeComponent();
            CtrlFlg                         = false;
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC310_Load(object sender, EventArgs e)
        {
            try
            {
                if (Transfer.dsDeviceID.List.Rows.Count == 0) return;

                this.dsFVDC310.List.Rows.Clear();

                for (int i = 0; i < Transfer.dsDeviceID.List.Rows.Count; i++)
                {
                    this.dsFVDC310.List.Rows.Add(Transfer.dsDeviceID.List.Rows[i].ItemArray);
                }
                this.dataGridView.Update();
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
            for (int i = 0; i < this.dataGridView.RowCount - 1; i++)
            {
                /// チェックをＯＮにする
                this.dataGridView["chkLine", i].Value      = true;
            }
        }
        /// <summary>
        /// 全解除を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFullOFF_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView.RowCount; i++)
            {
                /// チェックをＯＦＦにする
                this.dataGridView["chkLine", i].Value      = false;
            }
        }
        
        /// <summary>
        /// [Ctrl] + [V] を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 17:    /// Ｃｔｒｌキー
                    CtrlFlg                 = true;
				    break;
                case 86:    /// Ｖキー
                    if (CtrlFlg)
                    {
                        this.dsFVDC310.List.Rows.Clear();

                        //クリップボードの内容を取得して、行で分ける
                        string pasteText    = Clipboard.GetText();

                        if (string.IsNullOrEmpty(pasteText)) return;

                        pasteText           = pasteText.Replace("\r\n", "\n");
                        pasteText           = pasteText.Replace('\r', '\n');
                        pasteText           = pasteText.TrimEnd(new char[] { '\n' });
                        string[] lines      = pasteText.Split('\n');
                        
                        /// クリップボードよりグリッドにデータを貼り付ける
                        foreach (string line in lines)
                        {
                            //タブで分割
                            string[] vals   = line.Split('\t');
                            if (vals[0].Trim() != "")
                            {
                                /// データ追加
                                this.dsFVDC310.List.Rows.Add(
                                        new object[] {
			                                             vals[0].Trim().ToUpper().Replace("SLC-","").Replace("Ｓ","S").Replace("ｓ","S").Replace("０","0").Replace("SO","S0").Replace("\"","").Replace(",","")
			                                         });
                            }
                        }
                        this.dataGridView.Update();
                    }
                    CtrlFlg                 = false;
				    break;
            }

        }
        /// <summary>
        /// ＯＫボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            
            Transfer.dsDeviceID             = new DsFree();
            /// 表の行数分繰り返す
            for (int i = 0; i < this.dataGridView.RowCount - 1; i++)
            {
                /// チェックＯＮの行のとき
                if(Convert.ToBoolean(this.dataGridView["chkLine", i].Value))
                {
                    Transfer.dsDeviceID.List.Rows.Add(
                            new object[] {
			                            this.dataGridView["DeviceID", i].Value.ToString().Trim()
			                            });
                }
            }
            this.dsFVDC310.List.Rows.Clear();
            this.Close();
        }
        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.dsFVDC310.List.Rows.Clear();
            this.Close();
        }
        /// <summary>
        /// 行にデータが追加されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            /// 全てチェックONにする
            btnFullON_Click(sender, e);
        }

    }
}
