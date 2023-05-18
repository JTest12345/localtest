/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC120 テストロット一時記録
 * 
 * 概略           : テストロットをレジストリに一時記録する
 * 
 * 作成           : 2016/08/23 SLA2.Uchida
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
    public partial class FVDC120 : Form
    {
        private FVDC121 FVDC121         = new FVDC121();
        bool CtrlFlg                    = false;
        public FVDC120()
        {
            InitializeComponent();
            CtrlFlg                     = false;
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC120_Load(object sender, EventArgs e)
        {

            try
            {
                this.dsFVDC120.FVDC120.Rows.Clear();

                /// レジストリより保存していた情報を取得
                string strTestLot       = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);

                /// データが無いとき処理を抜ける
                if ((strTestLot == null) || (strTestLot == "")) return;

                /// データが有るときグリッドにセットする
                string[] sptTestLot     = strTestLot.Split(',');
                for (int i = 0; i < sptTestLot.Length; i++)
                {
                    this.dsFVDC120.FVDC120.Rows.Add(sptTestLot[i]);
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
                        string strTestLot   = "@";

                        /// 表の行数分繰り返す
                        for (int i = 0; i < this.dataGridView.RowCount - 1; i++)
                        {
                            /// チェックＯＮの行のとき
                            if (Convert.ToBoolean(this.dataGridView["chkLine", i].Value))
                            {
                                strTestLot  += "," + this.dataGridView["Lot_NO", i].Value.ToString().Trim();
                            }
                        }
                        strTestLot          = strTestLot.Replace("@,", "").Replace("@", "");
                        dsFVDC120.FVDC120.Rows.Clear();
                        string[] sptTestLot = strTestLot.Split(',');
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (sptTestLot[i].Trim() != "")
                            {
                                this.dsFVDC120.FVDC120.Rows.Add(sptTestLot[i]);
                            }
                        }

                        //クリップボードの内容を取得して、行で分ける
                        string pasteText    = Clipboard.GetText();

                        if (string.IsNullOrEmpty(pasteText)) return;

                        pasteText           = pasteText.Replace("\r\n", "\n");
                        pasteText           = pasteText.Replace('\r', '\n');
                        pasteText           = pasteText.TrimEnd(new char[] { '\n' });
                        string[] lines      = pasteText.Split('\n');

                        /// 入力された内容でプロファイルを検索する
                        CommonRead objCommonRead = new CommonRead();
                        using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                        {
                            /// クリップボードよりグリッドにデータを貼り付ける
                            foreach (string line in lines)
                            {
                                //タブで分割
                                string[] vals = line.Split('\t');
                                if ((vals[0].Trim() != "")
                                    && (vals[0].Substring(vals[0].Length - 3, 2) == "99")
                                    && (!strTestLot.Contains(vals[0].Trim().ToUpper())))
                                {
                                    if (vals[0].Substring(vals[0].Length - 3, 2) == "99")
                                    {
                                        strTestLot          += "," + vals[0].Trim().ToUpper();
                                        /// データ追加
                                        dsFVDC120.FVDC120.Rows.Add(
                                                new object[] {
			                                                vals[0].Trim().ToUpper()
			                                            });
                                    }
                                    else
                                    {
                                        DsName dsName = new DsName();
                                        string WhereSql;
                                        WhereSql = " INNER JOIN TmProfile ON TnLot.profileid = TmProfile.profileid "
                                                            + "WHERE (TnLot.lotno = '" + vals[0].Trim().ToUpper() + "') ";
                                        objCommonRead.TopNameRead(sqlInfo, "", "TnLot", "TmProfile.profilenm", "TmProfile.trialno", WhereSql, false, ref dsName);
                                        if (dsName.Name.Rows.Count == 0)
                                        {
                                        }
                                        /// 検証用ロットのとき
                                        else if (dsName.Name[0].Key_CD.Contains("検証")
                                                || dsName.Name[0].Key_CD.Contains("テスト")
                                                || dsName.Name[0].Key_CD.Contains("てすと")
                                                || dsName.Name[0].Key_CD.Contains("ダミー")
                                                || dsName.Name[0].Key_CD.ToUpper().Contains("DUMMY")
                                                || dsName.Name[0].Key_CD.ToUpper().Contains("TEST")
                                                || dsName.Name[0].Key_CD.ToUpper().Contains("ＴＥＳＴ")
                                                || dsName.Name[0].Data_NM.Contains("検証")
                                                || dsName.Name[0].Data_NM.Contains("テスト")
                                                || dsName.Name[0].Data_NM.Contains("てすと")
                                                || dsName.Name[0].Data_NM.Contains("ダミー")
                                                || dsName.Name[0].Data_NM.ToUpper().Contains("DUMMY")
                                                || dsName.Name[0].Data_NM.ToUpper().Contains("TEST")
                                                || dsName.Name[0].Data_NM.ToUpper().Contains("ＴＥＳＴ"))
                                        {
                                            /// 検証用ロットのときだけ入力を受け付ける
                                            strTestLot      += "," + vals[0].Trim().ToUpper();
                                            /// データ追加
                                            dsFVDC120.FVDC120.Rows.Add(
                                                    new object[] {
			                                                 vals[0].Trim().ToUpper()
			                                             });
                                        }
                                    }

                                }
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
            /// レジストリを更新する
            string strTestLot               = "@";

            /// 表の行数分繰り返す
            for (int i = 0; i < this.dataGridView.RowCount - 1; i++)
            {
                /// チェックＯＮで空白でない行のとき
                if ((Convert.ToBoolean(this.dataGridView["chkLine", i].Value))
                    && (this.dataGridView["Lot_NO", i].Value.ToString().Trim() != ""))
                {
                    strTestLot              += "," + this.dataGridView["Lot_NO", i].Value.ToString().Trim();
                }
            }
            strTestLot                      = strTestLot.Replace("@,,", "").Replace("@,", "").Replace(",,", ",").Replace("@", "");

            /// 設定されたデータが無いとき
            if (strTestLot == "")
            {
                /// レジストリに保存している内容をクリアする
                try
                {
                    fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
                }
                catch { }
            }
            else
            {
                /// 入力された内容をレジストリに登録します。
                fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot, (object)strTestLot);
            }

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

        /// <summary>
        /// セルをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /// 空欄の場合には何もしない
            if (this.dataGridView[1, e.RowIndex].Value.ToString().Trim() == "") return;

            /// クリップボードにセルの内容をコピーする
            Clipboard.Clear();
            if (this.dataGridView.Rows.Count > 0)
            {
                string strClipboard         = this.dataGridView[1, e.RowIndex].Value.ToString();
                try
                {
                    Clipboard.SetText(strClipboard);
                }
                catch { }
            }

        }

        /// <summary>
        /// セルを離れるとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_RowLeave(object sender, DataGridViewCellEventArgs e)
        {            
            /// 空欄の場合には何もしない
            try
            {
                if (this.dataGridView[1, e.RowIndex].Value.ToString().Trim() == "") return;
            }
            catch
            {
                return;
            }

            if (this.dataGridView[1, e.RowIndex].Value.ToString().Trim().Substring(this.dataGridView[1, e.RowIndex].Value.ToString().Trim().Length - 3,2) == "99")
            {
            }
            else
            {
                /// 入力された内容でプロファイルを検索する
                CommonRead objCommonRead                        = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    DsName dsName                               = new DsName();
                    string WhereSql;
                    WhereSql                                    = " INNER JOIN TmProfile ON TnLot.profileid = TmProfile.profileid "
                                                                + "WHERE (TnLot.lotno = '" + this.dataGridView[1, e.RowIndex].Value.ToString().Trim() + "') ";
                    objCommonRead.TopNameRead(sqlInfo, "", "TnLot", "TmProfile.profilenm", "TmProfile.trialno", WhereSql, false, ref dsName);
                    if (dsName.Name.Rows.Count == 0)
                    {
                        /// データをクリアする
                        this.dataGridView[1, e.RowIndex].Value  = "";
                        /// チェックをＯＦＦにする
                        this.dataGridView["chkLine", e.RowIndex].Value = false;
                    }
                    /// 検証用ロットのとき
                    else if (dsName.Name[0].Key_CD.Contains("検証")
                        || dsName.Name[0].Key_CD.Contains("テスト")
                        || dsName.Name[0].Key_CD.Contains("てすと")
                        || dsName.Name[0].Key_CD.Contains("ダミー")
                        || dsName.Name[0].Key_CD.ToUpper().Contains("DUMMY")
                        || dsName.Name[0].Key_CD.ToUpper().Contains("TEST")
                        || dsName.Name[0].Key_CD.ToUpper().Contains("ＴＥＳＴ")
                        || dsName.Name[0].Data_NM.Contains("検証")
                        || dsName.Name[0].Data_NM.Contains("テスト")
                        || dsName.Name[0].Data_NM.Contains("てすと")
                        || dsName.Name[0].Data_NM.Contains("ダミー")
                        || dsName.Name[0].Data_NM.ToUpper().Contains("DUMMY")
                        || dsName.Name[0].Data_NM.ToUpper().Contains("TEST")
                        || dsName.Name[0].Data_NM.ToUpper().Contains("ＴＥＳＴ"))
                    {
                        /// 検証用ロットのときだけ入力を受け付ける
                    }
                    else
                    {
                        /// データをクリアする
                        this.dataGridView[1, e.RowIndex].Value          = "";
                        /// チェックをＯＦＦにする
                        this.dataGridView["chkLine", e.RowIndex].Value  = false;
                    }
                }
            }
        }

        /// <summary>
        /// 検証プロファイルよりロットを検索が押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProfile_Click(object sender, EventArgs e)
        {
            /// 設定画面を表示する
            FVDC121.ShowDialog();

            /// 画面が読み込まれたときを再実行
            FVDC120_Load(null, null);
        }
    }
}
