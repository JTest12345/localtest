/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC140 設備番号入力
 * 
 * 概略           : 設備番号を入力する
 * 
 * 作成           : 2020/01/28 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC140 : Form
    {
        public FVDC140()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画面がロードされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC140_Load(object sender, EventArgs e)
        {

            Transfer.DeviceID       = "";
            this.txtDeviceID.Text   = "";
        }

        /// <summary>
        /// 設備番号の入力が有ったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDeviceID_TextChanged(object sender, EventArgs e)
        {
            if (this.lblDeviceID.Text.Trim() == "") return;

            this.txtDeviceID.Text = this.txtDeviceID.Text.Trim().ToUpper().Replace("SLC-", "").Replace("Ｓ", "S").Replace("ｓ", "S").Replace("０", "0").Replace("SO", "S0");
        }

        /// <summary>
        /// ＯＫボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Transfer.DeviceID       = this.txtDeviceID.Text;
            this.Close();
        }

        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Transfer.DeviceID       = "";
            this.Close();
        }

    }
}
