using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EICS
{
    public partial class F06_ErrCheck : Form
    {
        private bool fOK = false;
        public bool FOK
        {
            get { return fOK; }
        }

        public F06_ErrCheck(string msg)
        {
            InitializeComponent();
            txtbMessage.Text = string.Format(Constant.MessageInfo.Message_30, msg);

            string slot = Getlot(msg);
            int linecd = Getlineno(msg);

            //ロット番号・linecdが取得出来ていれば、グリッドに情報表示。
            if (slot != "" && linecd != -1)
            {
                List<LotPrevInfo> listlotprevinfo = ConnectDB.GetLotPrevInfo(slot, linecd);
                gvLotInfo.DataSource=listlotprevinfo;
            }
        }
        /// <summary>
        /// エラーメッセージ内に含まれるロットの文字列を抽出。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private int Getlineno(string msg)
        {
            int wlinecd = -1;
            //private const string MESSAGE_22 = "[{0}/{1}号機][{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},Lot={6},Linecd={7}";
            //private const string MESSAGE_23 = "[{0}/{1}号機][{2}]の設定値に誤りがあります。取得値={3},閾値={4},Lot={5},Linecd={6}";
            List<string> msglist = msg.Split(',').ToList();

            for (int i = 0; i < msglist.Count; i++)
            {
                if (msglist[i].Contains("Linecd="))
                {
                    wlinecd = Convert.ToInt32(msglist[i].Substring(7, msglist[i].Length - 7));//"Linecd="を省いた部分がLinecd
                }
            }
            //空白かLotか?どちらか
            return wlinecd;
        }
        /// <summary>
        /// エラーメッセージ内に含まれるロットの文字列を抽出。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string Getlot(string msg)
        {
            string wlot = "";
            //private const string MESSAGE_22 = "[{0}/{1}号機][{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},Lot={6},Linecd={7}";
            //private const string MESSAGE_23 = "[{0}/{1}号機][{2}]の設定値に誤りがあります。取得値={3},閾値={4},Lot={5},Linecd={6}";
            List<string> msglist = msg.Split(',').ToList();

            for (int i = 0; i < msglist.Count; i++)
            {
                if (msglist[i].Contains("Lot=")) {
                    wlot = msglist[i].Substring(4, msglist[i].Length - 4);//"Lot="を省いた部分がlotno
                }
            }
            //空白かLotか?どちらか
            return wlot;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            fOK = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
