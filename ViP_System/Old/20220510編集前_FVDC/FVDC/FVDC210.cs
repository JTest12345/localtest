/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC210 置換
 * 
 * 概略           : 置換する文字の入力を行う。
 * 
 * 作成           : 2018/11/08 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC210 : Form
    {
        public FVDC210()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 置換ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Transfer.SearchChar     = this.txtSearch.Text;
            Transfer.ReplaceChar    = this.txtReplace.Text;
            this.Close();
        }
        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Transfer.SearchChar     = "";
            Transfer.ReplaceChar    = "";
            this.Close();
        }
    }
}
