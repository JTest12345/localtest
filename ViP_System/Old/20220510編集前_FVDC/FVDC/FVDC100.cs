/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC100 流動検証データ編集
 * 
 * 概略           : 流動検証用ダミーデータを生成/編集し、検証終了後、データの削除も行う。
 * 
 * 作成           : 2016/08/08 SLA2.Uchida
 * 
 * 修正履歴       : 2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった
 *                  2018/04/17 SLA2.Uchida ロット生成時Jobロットなら下３桁を番号とする
 *                  2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
 *                  2019/12/09 SLA2.Uchida カット工程についてもロット生成を可能とする
 *                  2020/01/28 SLA2.Uchida 設備切替機能追加
 *                  2020/01/28 SLA2.Uchida 設備ロット検索機能追加
 ************************************************************************************/

using Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC100 : Form
    {
        private FVDC110 FVDC110 = new FVDC110();
        private FVDC120 FVDC120 = new FVDC120();
        private FVDC130 FVDC130 = new FVDC130();    /// 2020/01/28 SLA2.Uchida 設備ロット検索機能追加
        private FVDC140 FVDC140 = new FVDC140();
        private FVDC200 FVDC200 = new FVDC200();
        private FVDC300 FVDC300 = new FVDC300();    /// 2020/01/28 SLA2.Uchida 設備切替機能追加
        private DsSqlData dsGridView1 = new DsSqlData();
        private DsSqlData dsGridView2 = new DsSqlData();
        private DsSqlData dsGridView3 = new DsSqlData();
        private DsSqlData dsGridView4 = new DsSqlData();
        private DsSqlData dsGridView5 = new DsSqlData();
        private DsSqlData dsGridView6 = new DsSqlData();
        private DsSqlData dsGridView7 = new DsSqlData();
        private DsSqlData dsGridView8 = new DsSqlData();
        private DsSqlData dsGridView9 = new DsSqlData();
        private DsSqlData dsGridView10 = new DsSqlData();
        private DsSqlData dsGridView11 = new DsSqlData();
        private DsSqlData dsGridView12 = new DsSqlData();
        private DsSqlData dsGridView13 = new DsSqlData();
        private DsSqlData dsGridView14 = new DsSqlData();
        private DsSqlData dsGridView15 = new DsSqlData();
        private DsSqlData dsGridView16 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
        private DsSqlData dsGridView17 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）

        private int UpdateTableCount = 0;
        private int updateStatusCount = 0;
        private int DeleteTableCount = 0;
        private int InsertTableCount = 0;

        /// データグリッドの最小高
        private int intMinHeight = 56;
        /// １行あたりのグリッドの高さ
        private int intAddLineSize = 16;
        /// グリッドの高さ調整する最大行数
        private int intAddLineMaxSize = 20;

        private bool TypeFound = false;
        private bool LoadFlg = false;

        private bool AutoSetFlg = false;

        public FVDC100()
        {
            InitializeComponent();

            /// バージョン
            try
            {
                Transfer.Version = "Ver." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                Transfer.Version = "Ver." + System.Windows.Forms.Application.ProductVersion;
            }
            this.Text = "FVDC100 流動検証サポート　" + Transfer.Version;

            /// 環境パス
            Transfer.DeskTopPath = Environment.GetEnvironmentVariable("USERPROFILE").Trim() + "\\デスクトップ\\";

            /// デスクトップが存在しないとき
            DirectoryInfo FolderInfo = new DirectoryInfo(Transfer.DeskTopPath);
            if (!FolderInfo.Exists)
            {
                /// フォルダ名を変更する
                Transfer.DeskTopPath = Transfer.DeskTopPath.Replace("デスクトップ", "Desktop");
            }


            Transfer.StartupPath = System.Windows.Forms.Application.StartupPath;
            /// 言語設定
            Transfer.Language = System.Globalization.RegionInfo.CurrentRegion.Name;

            /// ステータスバー設定
            this.tsslProgressBar.Visible = false;
            this.tsslMessage.Size = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);

        }

        /// <summary>
        /// ステータスバー更新
        /// </summary>
        /// <param name="progress"></param>
        private void updateStatus(int progress, string Message)
        {
            //	プログレスバーの更新
            if (progress > 100 || progress == 0)
            {
                this.tsslProgressBar.Visible = false;
                this.tsslProgressBar.Value = 0;
            }
            else
            {
                this.tsslProgressBar.Visible = true;
                this.tsslProgressBar.Value = progress;
            }
            //	メッセージの設定
            if (Message != null)
            {
                this.tsslMessage.Text = Message;
                this.tsslMessage.Visible = true;
                this.tsslMessage.Size = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, true), 17);
            }
            this.statusStrip.Refresh();
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC100_Load(object sender, EventArgs e)
        {
            /// レジストリより保存していた情報を取得
            try
            {
                //デフォルト値の設定
                Transfer.ServerName = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultServer);
                if ((Transfer.ServerName == null) || (Transfer.ServerName == ""))
                {
                    FVDC110.ShowDialog();
                }

                /// サーバーに接続可能か確認する
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrCheckDB, true))
                {
                    /// サーバーに接続出来なかったとき
                    if (sqlInfo.Transaction == null)
                    {
                        /// レジストリに保存している内容をクリアする
                        try
                        {
                            fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultServer);
                        }
                        catch { }
                        /// メインサーバーIP情報を表示する
                        Transfer.ServerName = "";
                        FVDC110.ShowDialog();
                    }
                }
            }
            catch
            {
                try
                {
                    FVDC110.ShowDialog();
                }
                catch
                {
                    this.Dispose();
                    System.Windows.Forms.Application.Exit();
                    return;
                }
            }
            this.tsslServer.Text = Transfer.ServerName;

            CommonRead objCommonRead = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                /// 対象ラインドロップダウン作成
                if (this.dsLine.Name.Count == 0)
                {
                    this.dsLine = new DsName();
                    this.dsLine.Name.Rows.Add(new object[] { "", "" });
                    objCommonRead.NameRead(sqlInfo, " TnLot", "macgroup", "macgroup", "", true, ref this.dsLine);
                    this.cmbLine.DataSource = this.dsLine;
                }
            }
            /// 選択ラインの設定
            this.dsType = new DsName();
            /// レジストリより保存していた情報を取得
            try
            {
                //デフォルト値の設定
                string strLine = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultLine);
                this.cmbLine.SelectedValue = (object)strLine;
                if ((this.cmbLine.SelectedValue == null)
                    || (this.cmbLine.SelectedValue.ToString() == ""))
                {
                    this.cmbLine.DroppedDown = true;
                }
            }
            catch { }


            /// 選択タイプの設定
            if (this.dsType.Name.Rows.Count <= 1)
            {
                /// タイプ・ロットドロップダウン作成
                cmbType_cmbLot_Create(true);
            }
            /// レジストリより保存していた情報を取得
            try
            {
                //デフォルト値の設定
                string strType = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultType);
                this.cmbType.Text = strType;
                if ((this.cmbType.SelectedValue == null)
                    || (this.cmbType.SelectedValue.ToString() == ""))
                {
                    this.cmbType.DroppedDown = true;
                }
            }
            catch { }

            /// 参照ロットの設定
            /// レジストリより保存していた情報を取得
            try
            {
                //デフォルト値の設定
                string strLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultLot);
                this.cmbLot.SelectedValue = (object)strLot;
                if ((this.cmbLot.SelectedValue == null)
                    || (this.cmbLot.SelectedValue.ToString() == ""))
                {
                    /// 一致しなかったとき強制的にレコードを追加する
                    this.dsLot.Name.Rows.Add(new object[] { strLot, strLot });
                    this.cmbLot.DataSource = this.dsLot;
                    this.cmbLot.SelectedIndex = this.dsLot.Name.Rows.Count - 1;
                }
            }
            catch { }


            /// レジストリより保存していたテストロット情報を取得
            string strTestLot = "";
            try
            {
                strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
            }
            catch { }

            string[] sptTestLot = null;
            try
            {
                sptTestLot = strTestLot.Split(',');
            }
            catch { }

            LoadFlg = true;
            string WhereSql;
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                /// 工程マスタ読込
                if ((Transfer.dsProcess == null)
                    || (Transfer.dsProcess.Name.Count == 0))
                {
                    Transfer.dsProcess = new DsName();
                    Transfer.dsProcess.Name.Rows.Add(new object[] { "", "" });
                    objCommonRead.NameRead(sqlInfo, " TmProcess", "procno", "procnm", " WHERE (procno > -1) AND (delfg  = 0) ", false, ref Transfer.dsProcess);
                }

                /// 作業CD読込
                if ((Transfer.dsWorkcd == null)
                    || (Transfer.dsWorkcd.Name.Count == 0))
                {
                    Transfer.dsWorkcd = new DsName();
                    objCommonRead.NameRead(sqlInfo, " TmProcess", "workcd", "procnm", " WHERE (procno > -1) AND (delfg  = 0) ", false, ref Transfer.dsWorkcd);
                }

                /// データが有るとき
                if ((strTestLot != null) && (strTestLot != ""))
                {
                    dsGridView10 = new DsSqlData();
                    dsGridView11 = new DsSqlData();
                    dsGridView12 = new DsSqlData();

                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLot", ref this.chkTableNM10, ref dsGridView10))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (this.cmbLot.Text != sptTestLot[i])
                            {
                                if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnLot", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView10))
                                {
                                }
                            }
                        }
                    }
                    /// ダミーロットのマガジンデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnMag", ref this.chkTableNM11, ref dsGridView11))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (this.cmbLot.Text != sptTestLot[i])
                            {
                                if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnMag", "lotno", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView11))
                                {
                                    this.chkTableNM11.Checked = true;
                                }
                            }
                        }
                    }
                    /// ダミーロットのトランザクションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnTran", ref this.chkTableNM12, ref dsGridView12))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (this.cmbLot.Text != sptTestLot[i])
                            {
                                WhereSql = " INNER JOIN TmWorkFlow ON dbo.TnTran.procno = dbo.TmWorkFlow.procno "
                                                + " INNER JOIN TnLot ON LEFT(TnTran.lotno, 11) = TnLot.lotno AND TmWorkFlow.typecd = REPLACE(dbo.TnLot.typecd, '_TEST', '') "
                                                + " WHERE (TnTran.lotno LIKE '" + sptTestLot[i] + "%')"
                                                + " ORDER BY TmWorkFlow.workorder , TnTran.lotno";
                                if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnTran", "", WhereSql, ref dsGridView12))
                                {
                                    this.chkTableNM12.Checked = true;
                                }
                            }
                        }
                    }
                }
            }
            ///↓↓↓↓↓↓　2016/08/08 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↓↓↓↓↓↓
            /// データが有るとき
            if ((strTestLot != null) && (strTestLot != ""))
            {
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrLENSDB, false))
                {
                    /// ダミーロットのLENSロットデータセット生成
                    dsGridView13 = new DsSqlData();
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "LENS", "TnLot", ref this.chkTableNM13, ref dsGridView13))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (this.cmbLot.Text != sptTestLot[i])
                            {
                                if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnLot", "LotNO", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView13))
                                {
                                    this.chkTableNM11.Checked = true;
                                }
                            }
                        }
                    }
                }
            }
            ///↑↑↑↑↑↑　2016/08/08 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↑↑↑↑↑↑



            /// データグリッドビューサイズ調整
            this.FVDC100_Resize(null, null);

            Cursor.Current = Cursors.Default;
            LoadFlg = false;
        }

        /// <summary>
        /// 設定を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbConfig_Click(object sender, EventArgs e)
        {
            /// 設定画面を表示する
            FVDC110.ShowDialog();

            /// サーバー情報がクリアされたとき
            if (Transfer.ServerName == "")
            {
                this.Dispose();
                System.Windows.Forms.Application.Exit();
                return;
            }

            /// サーバー情報が違うとき
            if (this.tsslServer.Text != Transfer.ServerName)
            {
                /// サーバー情報を書き換える
                this.tsslServer.Text = Transfer.ServerName;

                /// 画面をリセットする
                tsbReset_Click(null, null);
            }
        }

        /// <summary>
        /// 対象タイプを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (this.cmbType.SelectedIndex > 0)
            {
                try
                {
                    //選択された内容をレジストリに登録します。
                    fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultType, this.cmbType.Text);

                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                    {
                        if (!AutoSetFlg)
                        {
                            /// 参照ロットドロップダウン作成
                            this.dsLot = new DsName();
                            this.dsLot.Name.Rows.Add(new object[] { "", "" });
                            string WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                                        + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                                        + "AND TnTran.procno = TmWorkFlow.procno "
                                                        + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                                        + "AND (TnLot.typecd = N'" + this.cmbType.SelectedValue.ToString() + "') ";
                            if (this.cmbLine.SelectedIndex > 0)
                            {
                                WhereSql += "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                            }
                            else
                            {
                                WhereSql += "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                            }
                            objCommonRead.TopNameRead(sqlInfo, " TOP (20) ", "TnLot", "TnLot.lotno", "TnLot.lotno", WhereSql, false, ref this.dsLot);

                            this.cmbLot.DataSource = this.dsLot;
                        }
                        /// 下位のデータをクリアする
                        this.dsProcess = new DsName();
                        this.dsProcess.Name.Rows.Add(new object[] { "", "" });
                        this.cmbProcess.DataSource = this.dsProcess;

                        /// タイプがテストタイプのとき
                        string strAction = "参考";
                        if (this.cmbType.SelectedValue.ToString().Contains("_TEST"))
                        {
                            strAction = "削除";
                        }

                        /// タイプの作業フローを検索
                        dsGridView1 = new DsSqlData();
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TmWorkFlow", ref this.chkTableNM1, ref dsGridView1))
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, strAction, "TmWorkFlow", "typecd", " = '" + this.cmbType.SelectedValue.ToString() + "'", ref dsGridView1))
                            {
                            }
                        }
                        /// タイプのオーブンプロファイルを検索
                        dsGridView2 = new DsSqlData();
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TmOvenProf", ref this.chkTableNM2, ref dsGridView2))
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, strAction, "TmOvenProf", "typecd", " = '" + this.cmbType.SelectedValue.ToString() + "'", ref dsGridView2))
                            {
                                /// データグリッドビューサイズ調整
                                this.FVDC100_Resize(null, null);
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            else if (!LoadFlg)
            {
                /// 対象タイプのレジストリ情報を削除します
                try
                {
                    //fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultType);
                }
                catch { }
            }
        }

        /// <summary>
        /// ラインを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLine_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (AutoSetFlg) return;

            if (this.cmbLine.SelectedIndex > 0)
            {
                /// タイプを取得する
                string seltype;
                try
                {
                    seltype = this.cmbType.SelectedValue.ToString();
                }
                catch
                {
                    seltype = this.cmbType.Text;
                }

                try
                {
                    //選択された内容をレジストリに登録します。
                    fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultLine, this.cmbLine.SelectedValue);

                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                    {
                        if (seltype == "")
                        {
                            /// 参照タイドロップダウン作成
                            this.dsType = new DsName();
                            this.dsType.Name.Rows.Add(new object[] { "", "" });
                            string WhereSql;
                            if (this.lblType.Text == "対象タイプ")
                            {
                                WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                                        + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                                        + "AND TnTran.procno = TmWorkFlow.procno "
                                                        + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                                        + "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                                objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TnLot", "TnLot.typecd", "TnLot.typecd", WhereSql, false, ref this.dsType);
                            }
                            else
                            {
                                WhereSql = " INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                                        + "INNER JOIN TnLot ON TmWorkFlow.typecd = TnLot.typecd "
                                                        + "WHERE (TmWorkFlow_1.typecd LIKE N'%_TEST')"
                                                        + " AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + " GROUP BY TmWorkFlow.typecd ORDER BY TmWorkFlow.typecd";
                                objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TmWorkFlow", "TmWorkFlow.typecd", "TmWorkFlow.typecd", WhereSql, false, ref this.dsType);
                            }
                            this.cmbType.DataSource = this.dsType;
                            Transfer.dsType = (DsName)this.dsType.Copy();

                            /// 参照ロットドロップダウン作成
                            this.dsLot = new DsName();
                            this.dsLot.Name.Rows.Add(new object[] { "", "" });
                            if (this.lblType.Text == "対象タイプ")
                            {
                                WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                                        + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                                        + "AND TnTran.procno = TmWorkFlow.procno "
                                                        + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                                        + "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                            }
                            else
                            {
                                WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                                        + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                                        + "INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                                        + "AND TnTran.procno = TmWorkFlow.procno "
                                                        + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                                        + "AND (TmWorkFlow_1.typecd LIKE N'%_TEST') "
                                                        + "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                            }
                            objCommonRead.TopNameRead(sqlInfo, " TOP (20) ", "TnLot", "TnLot.lotno", "TnLot.lotno", WhereSql, false, ref this.dsLot);
                            this.cmbLot.DataSource = this.dsLot;
                        }
                        else
                        {
                            /// 参照ロットドロップダウン作成
                            this.dsLot = new DsName();
                            this.dsLot.Name.Rows.Add(new object[] { "", "" });
                            string WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                                        + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                                        + "AND TnTran.procno = TmWorkFlow.procno "
                                                        + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                                        + "AND (TnLot.typecd = N'" + seltype.Replace("_TEST", "") + "') "
                                                        + "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                                        + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                                        + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                            objCommonRead.TopNameRead(sqlInfo, " TOP (20) ", "TnLot", "TnLot.lotno", "TnLot.lotno", WhereSql, false, ref this.dsLot);
                            this.cmbLot.DataSource = this.dsLot;

                        }
                    }
                    /// 下位のデータをクリアする
                    this.dsProcess = new DsName();
                    this.dsProcess.Name.Rows.Add(new object[] { "", "" });
                    this.cmbProcess.DataSource = this.dsProcess;
                }
                catch
                {
                    //throw;
                }
            }
            else if (!LoadFlg)
            {
                /// 対象ラインのレジストリ情報を削除します
                try
                {
                    //fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultLine);
                }
                catch { }
            }
        }

        /// <summary>
        /// 参照ロットを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLot_SelectedIndexChanged(object sender, EventArgs e)
        {
            string SelectLotNo = "";
            if (this.cmbLot.SelectedIndex > 0)
            {
                SelectLotNo = this.cmbLot.SelectedValue.ToString();
                try
                {
                    //選択された内容をレジストリに登録します。
                    fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultLot, this.cmbLot.SelectedValue);

                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                    {
                        /// 参照ロットのロットデータを検索
                        dsGridView3 = new DsSqlData();
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLot", ref this.chkTableNM3, ref dsGridView3))
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "参照", "TnLot", "lotno", " = '" + SelectLotNo + "'", ref dsGridView3))
                            {
                                /// ラインを取得する
                                string selLine;
                                try
                                {
                                    selLine = this.cmbLine.SelectedValue.ToString();
                                }
                                catch
                                {
                                    selLine = this.cmbLine.Text;
                                }
                                /// ラインが選択されていないとき
                                if (selLine == "")
                                {
                                    /// 自動で設定を行う
                                    this.cmbLine.SelectedValue = dsGridView3.LIST[0]["macgroup"].ToString();
                                }

                                /// タイプを取得する
                                string seltype;
                                try
                                {
                                    seltype = this.cmbType.SelectedValue.ToString();
                                }
                                catch
                                {
                                    seltype = this.cmbType.Text;
                                }
                                /// タイプが選択されていないとき
                                if (seltype == "")
                                {
                                    /// 自動で設定を行う
                                    this.cmbType.SelectedValue = dsGridView3.LIST[0]["typecd"].ToString();
                                }
                            }
                        }
                        /// 参照ロットのトランザクションデータを検索
                        dsGridView4 = new DsSqlData();
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnTran", ref this.chkTableNM4, ref dsGridView4))
                        {
                            string WhereSql = " INNER JOIN TmWorkFlow ON dbo.TnTran.procno = dbo.TmWorkFlow.procno "
                                                    + " INNER JOIN TnLot ON LEFT(TnTran.lotno, 11) = TnLot.lotno AND TmWorkFlow.typecd = TnLot.typecd "
                                                    + " WHERE (TnTran.lotno LIKE '" + SelectLotNo + "%')"
                                                    + " ORDER BY TmWorkFlow.workorder , TnTran.lotno";
                            if (objCommonRead.SqlDataFind(sqlInfo, "参照", "TnTran", "", WhereSql, ref dsGridView4))
                            {
                                /// データグリッドビューサイズ調整
                                this.FVDC100_Resize(null, null);
                            }
                        }

                        /// 対象工程ドロップダウン作成
                        this.dsProcess = new DsName();
                        this.dsProcess.Name.Rows.Add(new object[] { "", "" });
                        string SvProcno = "";
                        int intProcno;
                        for (int i = 0; i < dsGridView4.LIST.Rows.Count; i++)
                        {
                            for (int j = 1; j < Transfer.dsProcess.Name.Rows.Count; j++)
                            {
                                if ((dsGridView4.LIST[i]["procno"].ToString() != SvProcno)
                                    && (dsGridView4.LIST[i]["procno"].ToString() == Transfer.dsProcess.Name[j].Key_CD))
                                {
                                    SvProcno = dsGridView4.LIST[i]["procno"].ToString();
                                    intProcno = Convert.ToInt32(SvProcno);
                                    this.dsProcess.Name.Rows.Add(
                                        new object[] {
                                             Transfer.dsProcess.Name[j].Key_CD,
                                             intProcno.ToString("00 ") + Transfer.dsProcess.Name[j].Data_NM
                                         });
                                }
                            }
                            ///↓↓↓↓↓↓　2019/12/09 SLA2.Uchida カット工程についてもロット生成を可能とする　↓↓↓↓↓↓
                            if (dsGridView4.LIST.Rows.Count == dsGridView1.LIST.Rows.Count - 1
                                && SvProcno == dsGridView1.LIST[dsGridView1.LIST.Rows.Count - 2]["procno"].ToString())
                            {
                                SvProcno = dsGridView1.LIST[dsGridView1.LIST.Rows.Count - 1]["procno"].ToString();
                                for (int j = 1; j < Transfer.dsProcess.Name.Rows.Count; j++)
                                {
                                    if (Transfer.dsProcess.Name[j].Key_CD == SvProcno)
                                    {
                                        intProcno = Convert.ToInt32(SvProcno);
                                        this.dsProcess.Name.Rows.Add(
                                            new object[] {
                                             Transfer.dsProcess.Name[j].Key_CD,
                                             intProcno.ToString("00 ") + Transfer.dsProcess.Name[j].Data_NM
                                             });
                                    }
                                }
                            }
                            ///↑↑↑↑↑↑　2019/12/09 SLA2.Uchida カット工程についてもロット生成を可能とする　↑↑↑↑↑↑
                        }
                        this.cmbProcess.DataSource = this.dsProcess;
                    }
                    this.cmbLot.SelectedValue = SelectLotNo;
                }
                catch
                {
                    throw;
                }
            }
            else if (!LoadFlg)
            {
                /// 参照ロットのレジストリ情報を削除します
                try
                {
                    fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultLot);
                }
                catch { }
            }
        }

        /// <summary>
        /// リセットを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbReset_Click(object sender, EventArgs e)
        {
            this.lblType.Text = "対象タイプ";
            TypeFound = false;
            LoadFlg = true;

            /// 画面をリセットする
            FVDC100_Reset();
        }

        /// <summary>
        /// 画面をリセットする
        /// </summary>
        private void FVDC100_Reset()
        {
            /// ロット選択データをクリアする
            try
            {
                this.cmbLot.SelectedIndex = 0;
            }
            catch { }

            /// ダミーロット関連データ検索を行っていないとき
            if (this.chkTableNM1.Text == "ARMS【TmWorkFlow】")
            {
                /// ロット選択レジストリをクリアする
                try
                {
                    fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultLot);
                }
                catch { }
                if ((!TypeFound)
                    && (this.cmbType.SelectedIndex == 0))
                {
                    /// タイプ選択レジストリをクリアする
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultType);
                    }
                    catch { }
                }
            }

            this.dsProcess = new DsName();
            this.dsProcess.Name.Rows.Add(new object[] { "", "" });
            this.cmbProcess.DataSource = this.dsProcess;

            /// チェック項目をクリアする
            this.chkTestData.Checked = false;
            this.chkTestType.Checked = false;
            this.chkProfile.Checked = false;
            this.chkEckPrm.Checked = false;

            this.nudLotCount.Value = 1;

            this.flowLayoutPanel.SuspendLayout();

            /// データクリア
            dsGridView1 = new DsSqlData();
            dsGridView2 = new DsSqlData();
            dsGridView3 = new DsSqlData();
            dsGridView4 = new DsSqlData();
            dsGridView5 = new DsSqlData();
            dsGridView6 = new DsSqlData();
            dsGridView7 = new DsSqlData();
            dsGridView8 = new DsSqlData();
            dsGridView9 = new DsSqlData();
            dsGridView10 = new DsSqlData();
            dsGridView11 = new DsSqlData();
            dsGridView12 = new DsSqlData();
            dsGridView13 = new DsSqlData();
            dsGridView14 = new DsSqlData();
            dsGridView15 = new DsSqlData();
            dsGridView16 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
            dsGridView17 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）

            /// タイプ検索したときはレジストリ情報の設定は行わない
            if (!TypeFound)
            {
                /// 画面を初期状態に戻す
                FVDC100_Load(null, null);
            }
            else
            {
                /// データグリッドビューサイズ調整
                this.FVDC100_Resize(null, null);
            }

            TypeFound = false;
        }

        /// <summary>
        /// 画面サイズが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC100_Resize(object sender, EventArgs e)
        {
            /// スクロール表示を切らないと正確な計算が出来ない
            this.flowLayoutPanel.AutoScroll = false;

            /// データグリッドビューサイズ調整
            DataGridReSize(this.chkTableNM1, dsGridView1, ref dataGridView1);
            DataGridReSize(this.chkTableNM2, dsGridView2, ref dataGridView2);
            DataGridReSize(this.chkTableNM3, dsGridView3, ref dataGridView3);
            DataGridReSize(this.chkTableNM4, dsGridView4, ref dataGridView4);
            DataGridReSize(this.chkTableNM5, dsGridView5, ref dataGridView5);
            DataGridReSize(this.chkTableNM6, dsGridView6, ref dataGridView6);
            DataGridReSize(this.chkTableNM7, dsGridView7, ref dataGridView7);
            DataGridReSize(this.chkTableNM8, dsGridView8, ref dataGridView8);
            DataGridReSize(this.chkTableNM9, dsGridView9, ref dataGridView9);
            DataGridReSize(this.chkTableNM10, dsGridView10, ref dataGridView10);
            DataGridReSize(this.chkTableNM11, dsGridView11, ref dataGridView11);
            DataGridReSize(this.chkTableNM12, dsGridView12, ref dataGridView12);
            DataGridReSize(this.chkTableNM13, dsGridView13, ref dataGridView13);
            DataGridReSize(this.chkTableNM14, dsGridView14, ref dataGridView14);
            DataGridReSize(this.chkTableNM15, dsGridView15, ref dataGridView15);
            DataGridReSize(this.chkTableNM16, dsGridView16, ref dataGridView16);  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
            DataGridReSize(this.chkTableNM17, dsGridView17, ref dataGridView17);  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）

            /// 画面を再描画してスクロールも自動表示する。
            this.flowLayoutPanel.ResumeLayout();
            this.flowLayoutPanel.AutoScroll = true;
        }

        /// <summary>
        /// データグリッドビューサイズ調整
        /// </summary>
        /// <param name="progress"></param>
        private void DataGridReSize(System.Windows.Forms.CheckBox chkTableNM, DsSqlData dsGridView, ref DataGridView dataGridView)
        {
            /// データ割付け
            dataGridView.DataSource = dsGridView.LIST;

            /// データが無いグリッド表示はしない
            if (dsGridView.LIST.Count > 0)
            {
                chkTableNM.Visible = true;
                dataGridView.Visible = true;
            }
            else
            {
                chkTableNM.Checked = false;
                chkTableNM.Text = "";
                chkTableNM.Visible = false;
                dataGridView.Visible = false;
                return;
            }

            /// 規定値取得
            int MaxWidth = this.flowLayoutPanel.Size.Width - 22;
            int MaxHeight = intMinHeight + (intAddLineSize * intAddLineMaxSize);

            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView.Columns[0].Width = 40;
            dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            dataGridView.Columns[0].ReadOnly = true;

            /// 検索した削除テーブルのとき
            if (dataGridView["操作", 0].Value.ToString() == "削除")
            {
                /// 全ての項目を変更不可にする
                for (int i = 1; i < dsGridView.LIST.Columns.Count; i++)
                {
                    dataGridView.Columns[i].ReadOnly = true;
                }
            }

            /// 実データを変更する可能性があるのでロックする。
            try
            {
                dataGridView.Columns["typecd"].ReadOnly = true;
            }
            catch { }
            try
            {
                dataGridView.Columns["lotno"].ReadOnly = true;
            }
            catch { }
            try
            {
                dataGridView.Columns["Material_CD"].ReadOnly = true;
            }
            catch { }

            /// 一時的に自動サイズ調整を有効にして自動的に割り当てられたグリッド幅を取得する。
            this.flowLayoutPanel.ResumeLayout();
            this.flowLayoutPanel.AutoScroll = true;
            dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            dataGridView.AutoSize = true;
            int SetWidth = dataGridView.Size.Width + 1;
            dataGridView.AutoSize = false;
            dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.flowLayoutPanel.AutoScroll = false;

            /// 横幅が最大表示サイズを超えていたら最大表示サイズに変更する
            if (SetWidth > MaxWidth)
            {
                SetWidth = MaxWidth;
            }
            /// 縦幅については計算して求める
            int SetHeight = intMinHeight + ((dsGridView.LIST.Count - 1) * intAddLineSize);
            if (SetHeight > MaxHeight)
            {
                SetHeight = MaxHeight;
            }

            ///データグリッドビューサイズ設定
            dataGridView.Size = new System.Drawing.Size(SetWidth, SetHeight);


            /// 行タイトルの色を変更する
            for (int i = 0; i < dsGridView.LIST.Rows.Count; i++)
            {
                if ((dataGridView["操作", i].Value.ToString() == "追加")
                    || (dataGridView["操作", i].Value.ToString() == "削除"))
                {
                    dataGridView["操作", i].Style.ForeColor = Color.Red;
                    dataGridView["操作", i].Style.SelectionForeColor = Color.Red;
                }
                else
                {
                    dataGridView["操作", i].Style.ForeColor = Color.Blue;
                    dataGridView["操作", i].Style.SelectionForeColor = Color.Blue;
                }
            }


            dataGridView.Refresh();
        }

        /// <summary>
        /// テストタイプでロット生成のチェックを変更したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTestData_CheckedChanged(object sender, EventArgs e)
        {

            if (this.chkTableNM1.Text != "ARMS【TmWorkFlow】")
            {
                this.chkTestData.Checked = false;
                return;
            }
        }

        /// <summary>
        /// テストタイプ生成のチェックを変更したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTestType_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTableNM1.Text != "ARMS【TmWorkFlow】")
            {
                this.chkTestType.Checked = false;
                return;
            }

            this.chkTableNM1.Checked = this.chkTestType.Checked;
            if (this.chkTestType.Checked)
            {
                for (int i = 0; i < dsGridView1.LIST.Rows.Count; i++)
                {
                    if (dsGridView1.LIST[i]["操作"].ToString() == "参考")
                    {
                        dsGridView1.LIST[i]["操作"] = "追加";
                        dsGridView1.LIST[i]["typecd"] = dsGridView1.LIST[i]["typecd"] + "_TEST";
                    }
                }
            }
            else
            {
                for (int i = 0; i < dsGridView1.LIST.Rows.Count; i++)
                {
                    if (dsGridView1.LIST[i]["操作"].ToString() == "追加")
                    {
                        dsGridView1.LIST[i]["操作"] = "参考";
                        dsGridView1.LIST[i]["typecd"] = dsGridView1.LIST[i]["typecd"].ToString().Replace("_TEST", "");
                    }
                }
            }
            /// データグリッドビューサイズ調整
            this.FVDC100_Resize(null, null);
        }

        /// <summary>
        /// オーブン用テストプロファイル生成のチェックを変更したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkProfile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTableNM2.Text != "ARMS【TmOvenProf】")
            {
                this.chkProfile.Checked = false;
                return;
            }

            this.chkTableNM2.Checked = this.chkProfile.Checked;

            if (this.chkProfile.Checked)
            {
                for (int i = 0; i < dsGridView2.LIST.Rows.Count; i++)
                {
                    if (dsGridView2.LIST[i]["操作"].ToString() == "参考")
                    {
                        dsGridView2.LIST[i]["操作"] = "追加";
                        dsGridView2.LIST[i]["typecd"] = dsGridView2.LIST[i]["typecd"] + "_TEST";
                    }
                }
            }
            else
            {
                for (int i = 0; i < dsGridView2.LIST.Rows.Count; i++)
                {
                    if (dsGridView2.LIST[i]["操作"].ToString() == "追加")
                    {
                        dsGridView2.LIST[i]["操作"] = "参考";
                        dsGridView2.LIST[i]["typecd"] = dsGridView2.LIST[i]["typecd"].ToString().Replace("_TEST", "");
                    }
                }
            }
            /// データグリッドビューサイズ調整
            this.FVDC100_Resize(null, null);
        }

        /// <summary>
        /// 必要資材検索を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMatFind_Click(object sender, EventArgs e)
        {
            /// 参照ロットが選択されているとき
            if (this.cmbLot.SelectedIndex > 0)
            {
                try
                {
                    /// プロファイルIDを取得する
                    string ProfileID = dsGridView3.LIST[0]["profileid"].ToString();
                    //string BlendCD              = dsGridView3.LIST[0]["blendcd"].ToString();
                    if (ProfileID.Trim() == "") return;

                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                    {
                        /// 参照ロットのロットデータを検索
                        dsGridView7 = new DsSqlData();
                        if (objCommonRead.SqlDataSetCreate(ref this.chkTableNM7, ref dsGridView7))
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, ProfileID, ref dsGridView7))
                            {
                                /// データグリッドビューサイズ調整
                                this.FVDC100_Resize(null, null);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// タイプ検索ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            /// 入力された内容で曖昧検索する
            CommonRead objCommonRead = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                /// 対象タイプドロップダウン作成
                this.dsType = new DsName();
                this.dsType.Name.Rows.Add(new object[] { "", "" });
                string WhereSql;
                if (this.cmbLine.SelectedIndex > 0)
                {
                    if (this.lblType.Text == "対象タイプ")
                    {
                        WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow.typecd LIKE '%" + this.cmbType.Text.Trim() + "%')"
                                            + "AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                            + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                            + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TnLot", "TnLot.typecd", "TnLot.typecd", WhereSql, false, ref this.dsType);
                    }
                    else
                    {
                        WhereSql = " INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                            + "INNER JOIN TnLot ON TmWorkFlow.typecd = TnLot.typecd "
                                            + "WHERE (TmWorkFlow_1.typecd LIKE N'%" + this.cmbType.Text.Trim() + "%_TEST')"
                                            + " AND (TnLot.macgroup = N'" + this.cmbLine.SelectedValue.ToString() + "') "
                                            + " GROUP BY TmWorkFlow.typecd ORDER BY TmWorkFlow.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TmWorkFlow", "TmWorkFlow.typecd", "TmWorkFlow.typecd", WhereSql, false, ref this.dsType);
                    }
                }
                else
                {
                    if (this.lblType.Text == "対象タイプ")
                    {
                        WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow.typecd LIKE '%" + this.cmbType.Text.Trim() + "%')"
                                            + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                            + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TnLot", "TnLot.typecd", "TnLot.typecd", WhereSql, false, ref this.dsType);
                    }
                    else
                    {
                        WhereSql = " INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                            + "WHERE (TmWorkFlow_1.typecd LIKE N'%" + this.cmbType.Text.Trim() + "%_TEST')"
                                            + " GROUP BY TmWorkFlow.typecd ORDER BY TmWorkFlow.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TmWorkFlow", "TmWorkFlow.typecd", "TmWorkFlow.typecd", WhereSql, false, ref this.dsType);
                    }
                }
                this.cmbType.Text = "";
                this.cmbType.DataSource = this.dsType;
                Transfer.dsType = (DsName)this.dsType.Copy();
                this.cmbType.DroppedDown = true;
            }
            /// 画面をリセット
            TypeFound = true;
            FVDC100_Reset();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// ロット生成ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLotCreate_Click(object sender, EventArgs e)
        {
            if (this.chkTableNM3.Text != "ARMS【TnLot】") return;

            if (dsGridView3.LIST.Rows.Count > 0)
            {
                DateTime AddDateTime = Convert.ToDateTime(this.dtpTestDateTime.Text.Trim());
                string LotNo = dsGridView3.LIST[0]["lotno"].ToString();
                string LotNoTop7 = LotNo.Substring(0, 7);
                string LotNoMid1 = LotNo.Substring(7, 1);
                int intLotNo = 9990;
                /// ↓↓↓↓↓↓　2018/04/17 SLA2.Uchida ロット生成時Jobロットなら下３桁を番号とする　↓↓↓↓↓↓
                DataCast objDataCast = new DataCast();
                if (objDataCast.NumericChange(LotNoMid1).ToString() != LotNoMid1)
                {
                    LotNoTop7 = LotNo.Substring(0, 8);
                    intLotNo = 990;
                }
                /// ↑↑↑↑↑↑  2018/04/17 SLA2.Uchida ロット生成時Jobロットなら下３桁を番号とする  ↑↑↑↑↑↑
                int AddIX = dsGridView3.LIST.Rows.Count;
                string AddLotNo = LotNoTop7 + intLotNo.ToString();
                /// ↓↓↓↓↓↓　2018/04/17 SLA2.Uchida ロット生成時Jobロットなら下３桁を番号とする　↓↓↓↓↓↓
                /// コピー元と同じになったとき番号を繰り上げる
                if (LotNo == AddLotNo)
                {
                    intLotNo++;
                    AddLotNo = LotNoTop7 + intLotNo.ToString();
                }
                /// ↑↑↑↑↑↑  2018/04/17 SLA2.Uchida ロット生成時Jobロットなら下３桁を番号とする  ↑↑↑↑↑↑
                /// 既に使用されている番号の後の番号を割り当てる
                for (int i = 1; i < dsGridView3.LIST.Rows.Count; i++)
                {
                    if (dsGridView3.LIST[i]["lotno"].ToString() == AddLotNo)
                    {
                        intLotNo++;
                        AddLotNo = LotNoTop7 + intLotNo.ToString();
                    }
                }
                try
                {
                    /// レジストリより保存していた情報を取得
                    string strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
                    for (int i = intLotNo; i <= 10000; i++)
                    {
                        /// レジストリに登録済みのロットのとき
                        if (strTestLot.Contains(AddLotNo))
                        {
                            intLotNo++;
                            AddLotNo = LotNoTop7 + intLotNo.ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch { }

                if (intLotNo > 9999) return;
                if (dsGridView5.LIST.Rows.Count == 0)
                {
                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                    {
                        /// マガジンデータセット生成
                        dsGridView5 = new DsSqlData();
                        objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnMag", ref this.chkTableNM5, ref dsGridView5);
                    }
                }
                ///↓↓↓↓↓↓　2016/08/08 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↓↓↓↓↓↓
                if (dsGridView6.LIST.Rows.Count == 0)
                {
                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrLENSDB, false))
                    {
                        /// マガジンデータセット生成
                        dsGridView6 = new DsSqlData();
                        objCommonRead.SqlDataSetCreate(sqlInfo, "LENS", "TnLot", ref this.chkTableNM6, ref dsGridView6);
                    }
                }
                ///↑↑↑↑↑↑　2016/08/08 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↑↑↑↑↑↑

                /// ロット数分生成する（MAX１０個まで）
                for (int i = 0; i < this.nudLotCount.Value; i++)
                {

                    /// ロットデータ生成
                    dsGridView3.LIST.Rows.Add(dsGridView3.LIST[0].ItemArray);
                    dsGridView3.LIST[AddIX]["操作"] = "追加";
                    dsGridView3.LIST[AddIX]["lotno"] = AddLotNo;

                    ///↓↓↓↓↓↓　2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↓↓↓↓↓↓
                    dsGridView6.LIST.Rows.Add(new object[] { "追加" });
                    dsGridView6.LIST[i]["LotNO"] = AddLotNo;
                    dsGridView6.LIST[i]["TypeCD"] = dsGridView3.LIST[0]["typecd"].ToString();
                    for (int j = 3; j < dsGridView6.LIST.Columns.Count; j++)
                    {
                        try
                        {
                            dsGridView6.LIST[i][j] = 0;
                        }
                        catch { }
                    }
                    dsGridView6.LIST[i]["LastupdDT"] = AddDateTime;
                    ///↑↑↑↑↑↑　2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった　↑↑↑↑↑↑

                    /// テストタイプで生成するとき
                    if (this.chkTestData.Checked)
                    {
                        dsGridView3.LIST[AddIX]["typecd"] = dsGridView3.LIST[0]["typecd"].ToString() + "_TEST";
                        dsGridView6.LIST[i]["TypeCD"] = dsGridView3.LIST[0]["typecd"].ToString() + "_TEST";       /// 2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった
                    }

                    /// トランザクションデータ生成
                    string DummyInMag = "";
                    string DummyOutMag = "";
                    string SetProcNo = "";
                    string SetLotNo = "";
                    int IX = 0;
                    for (int j = 0; j < dsGridView4.LIST.Rows.Count; j++)
                    {
                        IX = dsGridView4.LIST.Rows.Count;
                        if (dsGridView4.LIST[j]["操作"].ToString() == "参照")
                        {
                            /// テスト工程が指定されているとき、指定工程が来たらコピーを中断する
                            if ((!this.cmbProcess.SelectedValue.Equals(null))
                                && (this.cmbProcess.SelectedValue.ToString() != "")
                                && (dsGridView4.LIST[j]["procno"].ToString() == this.cmbProcess.SelectedValue.ToString()))
                            {
                                break;
                            }

                            /// データをコピーしダミーデータを生成する
                            SetProcNo = dsGridView4.LIST[j]["procno"].ToString();
                            try
                            {
                                DummyInMag = dsGridView4.LIST[j]["inmag"].ToString().Substring(0, 2) + "00XXX";
                            }
                            catch
                            {
                                DummyInMag = "";
                            }
                            try
                            {
                                DummyOutMag = dsGridView4.LIST[j]["outmag"].ToString().Substring(0, 2) + "00XXX";
                            }
                            catch
                            {
                                DummyOutMag = "";
                            }
                            /// 分割ロットのとき
                            bool splitFlg = true;
                            if (dsGridView4.LIST[j]["lotno"].ToString().Contains("_#"))
                            {
                                /// テスト用タイプの設定が分割になっているか
                                for (int k = 0; k < dsGridView1.LIST.Rows.Count; k++)
                                {
                                    if ((dsGridView3.LIST[AddIX]["typecd"].ToString() == dsGridView1.LIST[k]["typecd"].ToString())
                                        && (SetProcNo == dsGridView1.LIST[k]["procno"].ToString()))
                                    {
                                        if (dsGridView1.LIST[k]["magdevidestatus"].ToString() == "0")
                                        {
                                            splitFlg = false;
                                        }
                                        break;
                                    }

                                }
                                /// テスト用タイプの設定が分割でなく最初のロットのとき
                                if ((!splitFlg) && (dsGridView4.LIST[j]["lotno"].ToString().Contains("_#1")))
                                {
                                    /// _#1を消してデータを追加する（_#2のデータはPassする）
                                    dsGridView4.LIST.Rows.Add(dsGridView4.LIST[j].ItemArray);
                                    dsGridView4.LIST[IX]["操作"] = "追加";
                                    SetLotNo = dsGridView4.LIST[j]["lotno"].ToString().Replace(LotNo, AddLotNo).Replace("_#1", "");
                                    dsGridView4.LIST[IX]["lotno"] = SetLotNo;
                                    dsGridView4.LIST[IX]["inmag"] = DummyInMag;
                                    dsGridView4.LIST[IX]["outmag"] = DummyOutMag;
                                    dsGridView4.LIST[IX]["startdt"] = AddDateTime;
                                    dsGridView4.LIST[IX]["enddt"] = AddDateTime;
                                    dsGridView4.LIST[IX]["lastupddt"] = AddDateTime;
                                    dsGridView4.LIST[IX]["macno"] = "999999";
                                }
                            }
                            /// 分割解除処理を行わないとき
                            if (splitFlg)
                            {
                                dsGridView4.LIST.Rows.Add(dsGridView4.LIST[j].ItemArray);
                                dsGridView4.LIST[IX]["操作"] = "追加";
                                SetLotNo = dsGridView4.LIST[j]["lotno"].ToString().Replace(LotNo, AddLotNo);
                                dsGridView4.LIST[IX]["lotno"] = SetLotNo;
                                dsGridView4.LIST[IX]["inmag"] = DummyInMag;
                                dsGridView4.LIST[IX]["outmag"] = DummyOutMag;
                                dsGridView4.LIST[IX]["startdt"] = AddDateTime;
                                dsGridView4.LIST[IX]["enddt"] = AddDateTime;
                                dsGridView4.LIST[IX]["lastupddt"] = AddDateTime;
                                dsGridView4.LIST[IX]["macno"] = "999999";
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (SetLotNo != "")
                    {
                        /// マガジンデータを作成する
                        int IY = dsGridView5.LIST.Rows.Count;
                        if (DummyOutMag == "")
                        {
                            DummyOutMag = "M000XXX";
                        }
                        if (SetLotNo != AddLotNo)
                        {
                            dsGridView5.LIST.Rows.Add(new object[] { "追加" });
                            dsGridView5.LIST[IY]["lotno"] = dsGridView4.LIST[IX - 2]["lotno"];
                            dsGridView5.LIST[IY]["magno"] = DummyOutMag;
                            dsGridView5.LIST[IY]["inlineprocno"] = SetProcNo;
                            dsGridView5.LIST[IY]["newfg"] = 1;
                            dsGridView5.LIST[IY]["lastupddt"] = AddDateTime;
                            IY++;
                        }
                        dsGridView5.LIST.Rows.Add(new object[] { "追加" });
                        dsGridView5.LIST[IY]["lotno"] = SetLotNo;
                        dsGridView5.LIST[IY]["magno"] = DummyOutMag;
                        dsGridView5.LIST[IY]["inlineprocno"] = SetProcNo;
                        dsGridView5.LIST[IY]["newfg"] = 1;
                        dsGridView5.LIST[IY]["lastupddt"] = AddDateTime;
                    }

                    AddIX++;
                    intLotNo++;
                    if (intLotNo > 9999) break;
                    AddLotNo = LotNoTop7 + intLotNo.ToString();
                }
                /// テーブル選択チェックONにする
                this.chkTableNM3.Checked = true;
                this.chkTableNM4.Checked = true;
                this.chkTableNM5.Checked = true;
                this.chkTableNM6.Checked = true;  /// 2017/11/28 SLA2.Uchida ロット生成時にLENS【TnLot】も追加必須となった
                                                  /// データグリッドビューサイズ調整
                this.FVDC100_Resize(null, null);
            }
        }

        /// <summary>
        /// 遠心沈降機用テストパラメータ生成のチェックを変更したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkEckPrm_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTableNM1.Text != "ARMS【TmWorkFlow】")
            {
                this.chkEckPrm.Checked = false;
                return;
            }

            /// タイプを取得する
            string SelectType;
            try
            {
                SelectType = this.cmbType.SelectedValue.ToString();
            }
            catch
            {
                SelectType = this.cmbType.Text;
            }

            /// タイプが選択されていないとき処理しない
            if (SelectType == "") return;


            DateTime AddDateTime = Convert.ToDateTime(this.dtpTestDateTime.Text.Trim());

            /// チェックされたとき
            if (this.chkEckPrm.Checked)
            {
                try
                {
                    /// プロファイルIDを取得する
                    //string ProfileID            = dsGridView3.LIST[0]["profileid"].ToString();
                    //if (ProfileID.Trim() == "") return;

                    CommonRead objCommonRead = new CommonRead();
                    using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrQCILDB, false))
                    {
                        /// 参照ロットのロットデータを検索
                        dsGridView8 = new DsSqlData();

                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TmPLM", ref this.chkTableNM8, ref dsGridView8))
                        {
                            string WhereSql = " WHERE (Model_NM = 'ECK') AND (Del_FG = 0) AND (Material_CD = '" + SelectType + "')";
                            if (objCommonRead.SqlDataFind(sqlInfo, "追加", "TmPLM", "", WhereSql, ref dsGridView8))
                            {
                                /// テストタイプに変更
                                for (int i = 0; i < dsGridView8.LIST.Rows.Count; i++)
                                {
                                    dsGridView8.LIST[i]["Material_CD"] = SelectType + "_TEST";
                                    if (dsGridView8.LIST[i]["Parameter_MIN"].ToString() != "")
                                    {
                                        dsGridView8.LIST[i]["Parameter_MIN"] = 0;
                                    }

                                    dsGridView8.LIST[i]["LastUpd_DT"] = AddDateTime;
                                }
                                /// データグリッドビューサイズ調整
                                this.FVDC100_Resize(null, null);
                            }
                        }

                    }
                }
                catch { }
            }
            else
            {
                dsGridView8 = new DsSqlData();
                /// データグリッドビューサイズ調整
                this.FVDC100_Resize(null, null);
            }
            this.chkTableNM8.Checked = this.chkEckPrm.Checked;
        }

        /// <summary>
        /// 常駐を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbResident_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.notifyIcon.Visible = true;
        }

        /// <summary>
        /// notifyIconをダブルクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            /// 最小化されているときは解除する
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            this.Visible = true;
            this.notifyIcon.Visible = false;
        }

        /// <summary>
        /// notifyIconをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            /// 最小化されているときは解除する
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            this.Visible = true;
            this.notifyIcon.Visible = false;
        }

        /// <summary>
        /// エクセル出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExcelOut_Click(object sender, EventArgs e)
        {

            using (ExcelReport exRep = new ExcelReport())
            {
                /// 新規にエクセルブックをＯＰＥＮする
                if (exRep.excel == null)
                {
                    exRep.excel = new ExcelAdapter();
                }
                exRep.excel.Book = exRep.excel.App.Workbooks.Add(System.Reflection.Missing.Value);
                exRep.excel.Sheet = (Worksheet)exRep.excel.Book.Worksheets[1];

                long SetRow = 1;
                string Title = "";

                /// 各グリッドの情報を出力する
                if (this.chkTableNM1.Visible)
                {
                    Title = this.chkTableNM1.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView1, Title, SetRow) + 1;
                }
                if (this.chkTableNM2.Visible)
                {
                    Title = this.chkTableNM2.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView2, Title, SetRow) + 1;
                }
                if (this.chkTableNM3.Visible)
                {
                    Title = this.chkTableNM3.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView3, Title, SetRow) + 1;
                }
                if (this.chkTableNM4.Visible)
                {
                    Title = this.chkTableNM4.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView4, Title, SetRow) + 1;
                }
                if (this.chkTableNM5.Visible)
                {
                    Title = this.chkTableNM5.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView5, Title, SetRow) + 1;
                }
                if (this.chkTableNM6.Visible)
                {
                    Title = this.chkTableNM6.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView6, Title, SetRow) + 1;
                }
                if (this.chkTableNM7.Visible)
                {
                    Title = this.chkTableNM7.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView7, Title, SetRow) + 1;
                }
                if (this.chkTableNM8.Visible)
                {
                    Title = this.chkTableNM8.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView8, Title, SetRow) + 1;
                }
                if (this.chkTableNM9.Visible)
                {
                    Title = this.chkTableNM9.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView9, Title, SetRow) + 1;
                }
                if (this.chkTableNM10.Visible)
                {
                    Title = this.chkTableNM10.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView10, Title, SetRow) + 1;
                }
                if (this.chkTableNM11.Visible)
                {
                    Title = this.chkTableNM11.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView11, Title, SetRow) + 1;
                }
                if (this.chkTableNM12.Visible)
                {
                    Title = this.chkTableNM12.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView12, Title, SetRow) + 1;
                }
                if (this.chkTableNM13.Visible)
                {
                    Title = this.chkTableNM13.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView13, Title, SetRow) + 1;
                }
                if (this.chkTableNM14.Visible)
                {
                    Title = this.chkTableNM14.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView14, Title, SetRow) + 1;
                }
                if (this.chkTableNM15.Visible)
                {
                    Title = this.chkTableNM15.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView15, Title, SetRow) + 1;
                }
                /// ↓↓↓↓↓↓　2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↓↓↓↓↓↓
                if (this.chkTableNM16.Visible)
                {
                    Title = this.chkTableNM16.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView16, Title, SetRow) + 1;
                }
                if (this.chkTableNM17.Visible)
                {
                    Title = this.chkTableNM17.Text;
                    SetRow = exRep.MakeNextReport(this.dataGridView17, Title, SetRow) + 1;
                }
                /// ↑↑↑↑↑↑  2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↑↑↑↑↑↑
                /// 出力内容を表示する
                exRep.excel.App.Visible = true;
            }
        }

        /// <summary>
        /// 登録を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAdd_Click(object sender, EventArgs e)
        {

            /// メッセージを表示する
            DialogResult OkNgButton = MessageBox.Show("　チェックが入っているテーブルで操作が赤色で\n\n　追加となっているデータの登録を行います。"
                            + "\n\n　そのまま登録を続行する場合にはＯＫを押して下さい。", "登録開始確認",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            /// キャンセルが押されたとき、処理を終了する
            if (OkNgButton == System.Windows.Forms.DialogResult.Cancel) return;

            /// マウスカーソルの変更
            Cursor.Current = Cursors.WaitCursor;

            updateStatus(0, "　ダ　ミ　ー　デ　ー　タ　登　録　中　・　・　・");

            DeleteTableCount = 0;
            InsertTableCount = 0;
            try
            {
                /// グリッド毎に更新処理を行う
                if (this.chkTableNM1.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM1, ref dsGridView1)) return;
                }
                if (this.chkTableNM2.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM2, ref dsGridView2)) return;
                }
                if (this.chkTableNM3.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM3, ref dsGridView3)) return;
                }
                if (this.chkTableNM4.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM4, ref dsGridView4)) return;
                }
                if (this.chkTableNM5.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM5, ref dsGridView5)) return;
                }
                if (this.chkTableNM6.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM6, ref dsGridView6)) return;
                }
                if (this.chkTableNM7.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM7, ref dsGridView7)) return;
                }
                if (this.chkTableNM8.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM8, ref dsGridView8)) return;
                }
                if (this.chkTableNM9.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM9, ref dsGridView9)) return;
                }
                if (this.chkTableNM10.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM10, ref dsGridView10)) return;
                }
                if (this.chkTableNM11.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM11, ref dsGridView11)) return;
                }
                if (this.chkTableNM12.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM12, ref dsGridView12)) return;
                }
                if (this.chkTableNM13.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM13, ref dsGridView13)) return;
                }
                if (this.chkTableNM14.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM14, ref dsGridView14)) return;
                }
                if (this.chkTableNM15.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM15, ref dsGridView15)) return;
                }
                /// ↓↓↓↓↓↓　2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↓↓↓↓↓↓
                if (this.chkTableNM16.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM16, ref dsGridView16)) return;
                }
                if (this.chkTableNM17.Checked)
                {
                    if (GridData_Update(ref this.chkTableNM17, ref dsGridView17)) return;
                }
                /// ↑↑↑↑↑↑  2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↑↑↑↑↑↑
            }
            catch (Exception ex)
            { }
            finally
            {
                /// マウスカーソルを元に戻します。
                updateStatus(0, "");
                this.tsslProgressBar.Visible = false;
                this.tsslMessage.Size = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);
                this.statusStrip.Refresh();
                Cursor.Current = Cursors.Default;
                updateStatusCount = 0;
            }
            /// 画面をリセットする
            FVDC100_Reset();

            /// 画面を初期状態に戻す
            FVDC100_Load(null, null);

            /// 更新内容に応じてメッセージを表示する
            if (DeleteTableCount > 0 || InsertTableCount > 0)
            {
                MessageBox.Show("　正常に登録処理が終了しました。\n\n　削除件数：" + DeleteTableCount.ToString()
                                + " 件、追加件数：" + InsertTableCount.ToString() + " 件", "登録終了",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("　正常に終了しましたが登録されたデータが有りませんでした。", "登録終了",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// グリッドデータ更新処理
        /// </summary>
        /// <param name="TitleNM"></param>
        /// <param name="dsGridView"></param>
        private bool GridData_Update(ref System.Windows.Forms.CheckBox chkTableNM, ref DsSqlData dsGridView)
        {
            updateStatusCount += 100;
            /// ステータスバーの更新
            updateStatus(updateStatusCount / UpdateTableCount, null);

            string TitleNM = chkTableNM.Text;

            /// サーバー情報を設定する
            DataCast objDataCast = new DataCast();
            string TableName = "";


            /// サーバー接続情報の生成
            string ConnectServer = objDataCast.ConnectServer(TitleNM, ref TableName);

            /// 必要資材のとき直ぐに処理を抜ける
            if (TableName == "必要資材") return false;

            /// トランザクション更新を行う
            DateTime AddDateTime = Convert.ToDateTime(this.dtpTestDateTime.Text.Trim());
            bool FirstInsert = true;
            string IntoSql = "INSERT INTO " + TableName + "(";
            string ValuesSql = " VALUES";
            string breakLotNo = "@";
            bool LotNoBreakFG = false;
            int UpdateCount = 0;
            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                try
                {
                    for (int i = 0; i < dsGridView.LIST.Rows.Count; i++)
                    {
                        switch (dsGridView.LIST[i].操作)
                        {
                            case "削除":
                                break;
                            case "追加":
                                /// ロットがブレークしたとき
                                LotNoBreakFG = false;
                                if ((dsGridView.LIST.Columns[1].ColumnName == "lotno")
                                    && (!dsGridView.LIST[i][1].ToString().StartsWith(breakLotNo)))
                                {
                                    LotNoBreakFG = true;
                                    breakLotNo = dsGridView.LIST[i][1].ToString().Replace("_#1", "").Replace("_#2", "");
                                }
                                /// 最初の追加またはロットがブレークしたとき
                                if (FirstInsert || LotNoBreakFG)
                                {
                                    /// 最初の項目がタイプかロットのとき
                                    if ((dsGridView.LIST.Columns[1].ColumnName == "typecd")
                                        || (dsGridView.LIST.Columns[1].ColumnName == "lotno")
                                        || (TitleNM == "QCIL【TmPLM】"))
                                    {
                                        /// 追加前に対象のタイプ／ロットデータを全て削除しておく
                                        /// 削除SQL作成
                                        string DelWhereSql = " WHERE  (" + dsGridView.LIST.Columns[1].ColumnName + " LIKE '{0}%') ";
                                        string Delsql = "";
                                        if (dsGridView.LIST.Columns[1].ColumnName == "lotno")
                                        {
                                            Delsql = "DELETE FROM " + TableName + " " + string.Format(DelWhereSql, breakLotNo);
                                        }
                                        else
                                        {
                                            Delsql = "DELETE FROM " + TableName + " " + string.Format(DelWhereSql, dsGridView.LIST[i][1]);
                                        }

                                        /// パラメータマスタのとき
                                        if (TitleNM == "QCIL【TmPLM】")
                                        {
                                            DelWhereSql = " WHERE (Model_NM = 'ECK') AND (Material_CD = '{0}') ";
                                            Delsql = "DELETE FROM " + TableName + " " + string.Format(DelWhereSql, dsGridView.LIST[i][3]);
                                        }

                                        try
                                        {
                                            /// 削除
                                            connect.Command.CommandText = Delsql;
                                            int ExecuteCount = connect.Command.ExecuteNonQuery();
                                            DeleteTableCount += ExecuteCount;
                                            UpdateCount += ExecuteCount;
                                        }
                                        catch (SqlException ex)
                                        {
                                            /// ネットワークの一般エラー
                                            if (ex.Number == 11)
                                            {
                                                i--;
                                                break;
                                            }
                                            else
                                            {
                                                ///TODO: エラー処理
                                                MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n　" + connect.Command.CommandText + "\n　" + ConnectServer
                                                                + "\n\n　上記エラーにより更新処理を中断しました。", "GridData_Update（追加前削除）" + TitleNM,
                                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                return true;
                                            }
                                        }
                                    }
                                    /// テストロットを追加するときレジストリに追加する
                                    if (TitleNM == "ARMS【TnLot】")
                                    {
                                        /// レジストリより保存していた情報を取得
                                        string strTestLot = "";
                                        try
                                        {
                                            strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
                                        }
                                        catch { }
                                        /// データが無いとき
                                        if ((strTestLot == null) || (strTestLot == ""))
                                        {
                                            strTestLot = "@";
                                        }
                                        /// 既に登録されていないとき
                                        if (!strTestLot.Contains(dsGridView.LIST[i][1].ToString()))
                                        {
                                            strTestLot += "," + dsGridView.LIST[i][1];
                                            strTestLot = strTestLot.Replace("@,", "");

                                            /// テストロットをレジストリに登録します。
                                            fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot, (object)strTestLot);
                                        }

                                    }
                                    /// 最初の追加のとき
                                    if (FirstInsert)
                                    {
                                        /// 追加項目名リストを作成する
                                        FirstInsert = false;
                                        /// 追加対象カラム設定
                                        for (int j = 1; j < dsGridView.LIST.Columns.Count; j++)
                                        {
                                            IntoSql = IntoSql + "," + dsGridView.LIST.Columns[j].ColumnName;
                                        }
                                        IntoSql = IntoSql.Replace("(,", "(") + ")";
                                    }
                                }
                                /// 更新日が有るときテスト日時に変更する
                                try
                                {
                                    dsGridView.LIST[i]["lastupddt"] = AddDateTime;
                                }
                                catch { }

                                /// データを更新し易い文字タイプに成型する
                                string[] stgItem = new string[dsGridView.LIST.Columns.Count];
                                DataRow dsRow = dsGridView.LIST.Rows[i];
                                objDataCast.dsRowCast(dsRow, ref stgItem);
                                /// 追加対象カラム設定
                                string InsData = "(";
                                for (int j = 1; j < dsGridView.LIST.Columns.Count; j++)
                                {
                                    InsData = InsData + "," + stgItem[j];
                                }
                                InsData = InsData.Replace("(,", "(") + ")";

                                /// 追加
                                try
                                {
                                    connect.Command.CommandText = IntoSql + ValuesSql + InsData;
                                    int ExecuteCount = connect.Command.ExecuteNonQuery();
                                    InsertTableCount += ExecuteCount;
                                    UpdateCount += ExecuteCount;
                                }
                                catch (SqlException ex)
                                {
                                    switch (ex.Number)
                                    {
                                        case 11:		/// ネットワークの一般エラー
                                            i--;		/// 処理を繰り返す
                                            break;
                                        case 2627:      /// 重複エラー
                                                        /// メッセージを表示する
                                            //DialogResult OkNgButton = MessageBox.Show("　" + ex.Message.Replace("PRIMARY KEY 違反。オブジェクト", "キーが重複しています。\n　テーブル").Replace("。重複", "。\n　重複")
                                            //                + "\n\n　" + TitleNM
                                            //                + "\n\n　更新を続行するには『はい(Y)』を 中断するには『いいえ(N)』を押して下さい。", "データ重複",
                                            //                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                            ///// いいえ(N)が押されたとき、処理を中断する
                                            //if (OkNgButton == System.Windows.Forms.DialogResult.No) return true;

                                            /// カラムデータを加工する
                                            string[] strColumnName = new string[2];
                                            for (int j = 1; j < 3; j++)
                                            {
                                                /// 日付型のときマイクロ秒を検索条件から除外する
                                                if (dsGridView.LIST.Columns[j].DataType.ToString() == "System.DateTime")
                                                {
                                                    strColumnName[j - 1] = "CAST(CONVERT(VARCHAR, " + dsGridView.LIST.Columns[j].ColumnName + ", 20) AS DATETIME)";
                                                }
                                                else
                                                {
                                                    strColumnName[j - 1] = dsGridView.LIST.Columns[j].ColumnName;
                                                }
                                            }
                                            /// 削除SQL作成
                                            string WhereSql = " WHERE  (" + strColumnName[0] + " = '{0}') "
                                                                + " AND (" + strColumnName[1] + " = '{1}') ";
                                            string sql = "DELETE FROM " + TableName + " " + string.Format(WhereSql, dsGridView.LIST[i][1], dsGridView.LIST[i][2]);
                                            try
                                            {
                                                /// 削除
                                                connect.Command.CommandText = sql;
                                                int ExecuteCount = connect.Command.ExecuteNonQuery();
                                                DeleteTableCount += ExecuteCount;
                                                UpdateCount += ExecuteCount;
                                                i--;
                                            }
                                            catch (SqlException ex2)
                                            {
                                                /// ネットワークの一般エラー
                                                if (ex2.Number == 11)
                                                {
                                                    i--;
                                                }
                                                else
                                                {
                                                    ///TODO: エラー処理
                                                    MessageBox.Show("　" + ex2.Number + ":" + ex2.Message + "\n　" + connect.Command.CommandText + "\n　" + ConnectServer
                                                                    + "\n\n　上記エラーにより更新処理を中断しました。", "GridData_Update（重複削除）" + TitleNM,
                                                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    return true;
                                                }
                                            }
                                            break;
                                        default:
                                            ///TODO: エラー処理
                                            MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n　" + ConnectServer
                                                            + "\n\n　上記エラーにより更新処理を中断しました。", "GridData_Update（追加）" + TitleNM,
                                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return true;
                                    }
                                }
                                break;
                        }
                    }
                    /// 更新データが有るとき
                    if (UpdateCount > 0)
                    {
                        /// 最終的に更新を反映する
                        connect.Commit();
                    }
                    /// 更新が終了したテーブルのチェックを外す
                    chkTableNM.Checked = false;
                    return false;
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n　" + connect.Command.CommandText + "\n　" + ConnectServer
                                    + "\n\n　更新処理を中断しました。", "GridData_Update " + TitleNM,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        /// <summary>
        /// 検索テストデータ削除を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbTestDataDelete_Click(object sender, EventArgs e)
        {

            /// メッセージを表示する
            DialogResult OkNgButton = MessageBox.Show("　チェックが入っているテーブルで操作が赤色で\n　削除となっているデータの削除を行います。"
                            + "\n\n　削除しますと元に戻せませんのでｴｸｾﾙ出力等で\n　バックアップを取っておく事をお勧めします。"
                            + "\n\n　そのまま削除を続行する場合にはＯＫを押して下さい。", "削除開始確認",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            /// キャンセルが押されたとき、処理を終了する
            if (OkNgButton == System.Windows.Forms.DialogResult.Cancel) return;

            /// マウスカーソルの変更
            Cursor.Current = Cursors.WaitCursor;

            updateStatus(0, "　検　索　テ　ス　ト　デ　ー　タ　削　除　中　・　・　・");

            DeleteTableCount = 0;
            try
            {
                /// グリッド毎に更新処理を行う
                if (this.chkTableNM1.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM1, ref dsGridView1)) return;
                }
                if (this.chkTableNM2.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM2, ref dsGridView2)) return;
                }
                if (this.chkTableNM3.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM3, ref dsGridView3)) return;
                }
                if (this.chkTableNM4.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM4, ref dsGridView4)) return;
                }
                if (this.chkTableNM5.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM5, ref dsGridView5)) return;
                }
                if (this.chkTableNM6.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM6, ref dsGridView6)) return;
                }
                if (this.chkTableNM7.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM7, ref dsGridView7)) return;
                }
                if (this.chkTableNM8.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM8, ref dsGridView8)) return;
                }
                if (this.chkTableNM9.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM9, ref dsGridView9)) return;
                }
                if (this.chkTableNM10.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM10, ref dsGridView10)) return;
                }
                if (this.chkTableNM11.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM11, ref dsGridView11)) return;
                }
                if (this.chkTableNM12.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM12, ref dsGridView12)) return;
                }
                if (this.chkTableNM13.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM13, ref dsGridView13)) return;
                }
                if (this.chkTableNM14.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM14, ref dsGridView14)) return;
                }
                if (this.chkTableNM15.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM15, ref dsGridView15)) return;
                }
                /// ↓↓↓↓↓↓　2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↓↓↓↓↓↓
                if (this.chkTableNM16.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM16, ref dsGridView16)) return;
                }
                if (this.chkTableNM17.Checked)
                {
                    if (GridData_Delete(ref this.chkTableNM17, ref dsGridView17)) return;
                }
                /// ↑↑↑↑↑↑  2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↑↑↑↑↑↑
            }
            catch { }
            finally
            {
                /// マウスカーソルを元に戻します。
                updateStatus(0, "");
                this.tsslProgressBar.Visible = false;
                this.tsslMessage.Size = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);
                this.statusStrip.Refresh();
                Cursor.Current = Cursors.Default;
                updateStatusCount = 0;
            }
            /// 画面をリセットする
            FVDC100_Reset();

            /// 画面を初期状態に戻す
            FVDC100_Load(null, null);

            /// 更新内容に応じてメッセージを表示する
            if (DeleteTableCount > 0)
            {
                MessageBox.Show("　正常に削除処理が終了しました。\n\n　削除件数：" + DeleteTableCount.ToString()
                                + " 件", "削除終了",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("　正常に終了しましたが削除されたデータが有りませんでした。", "削除終了",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// グリッドデータ削除処理
        /// </summary>
        /// <param name="TitleNM"></param>
        /// <param name="dsGridView"></param>
        private bool GridData_Delete(ref System.Windows.Forms.CheckBox chkTableNM, ref DsSqlData dsGridView)
        {
            if ((!chkTableNM.Text.Contains("TnAGVPSTester"))
                && (!chkTableNM.Text.Contains("TnRestrict")))
            {
                updateStatusCount += 100;
                /// ステータスバーの更新
                updateStatus(updateStatusCount / UpdateTableCount, null);
            }

            string TitleNM = chkTableNM.Text;

            /// サーバー情報を設定する
            DataCast objDataCast = new DataCast();
            string TableName = "";

            /// サーバー接続情報の生成
            string ConnectServer = objDataCast.ConnectServer(TitleNM, ref TableName);

            /// 必要資材のとき直ぐに処理を抜ける
            if (TableName == "必要資材") return false;

            if (TitleNM == "ARMS【TnLot】")
            {
                /// テストロットのレジストリ情報を削除する
                try
                {
                    fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
                }
                catch { }
            }

            /// トランザクション更新を行う
            int UpdateCount = 0;
            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                try
                {
                    for (int i = 0; i < dsGridView.LIST.Rows.Count; i++)
                    {
                        switch (dsGridView.LIST[i].操作)
                        {
                            case "削除":

                                /// カラムデータを加工する
                                string[] strColumnName = new string[3];
                                for (int j = 1; j < 4; j++)
                                {
                                    /// 日付型のときマイクロ秒を検索条件から除外する
                                    if (dsGridView.LIST.Columns[j].DataType.ToString() == "System.DateTime")
                                    {
                                        strColumnName[j - 1] = "CAST(CONVERT(VARCHAR, " + dsGridView.LIST.Columns[j].ColumnName + ", 20) AS DATETIME)";
                                    }
                                    else
                                    {
                                        strColumnName[j - 1] = dsGridView.LIST.Columns[j].ColumnName;
                                    }
                                }

                                /// 削除SQL作成
                                string WhereSql = " WHERE  (" + strColumnName[0] + " = '{0}') "
                                                    + " AND (" + strColumnName[1] + " = '{1}') "
                                                    + " AND (" + strColumnName[2] + " = '{2}') ";
                                string sql = "DELETE FROM " + TableName + " " + string.Format(WhereSql, dsGridView.LIST[i][1], dsGridView.LIST[i][2], dsGridView.LIST[i][3]);
                                try
                                {
                                    /// 削除
                                    connect.Command.CommandText = sql;
                                    int ExecuteCount = connect.Command.ExecuteNonQuery();
                                    DeleteTableCount += ExecuteCount;
                                    UpdateCount += ExecuteCount;
                                }
                                catch (SqlException ex)
                                {
                                    /// ネットワークの一般エラー
                                    if (ex.Number == 11)
                                    {
                                        i--;
                                    }
                                    else
                                    {
                                        ///TODO: エラー処理
                                        MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n　" + connect.Command.CommandText + "\n　" + ConnectServer
                                                        + "\n\n　上記エラーにより更新処理を中断しました。", "GridData_DELETE" + TitleNM,
                                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return true;
                                    }
                                }
                                break;
                            case "追加":
                                break;
                        }
                    }
                    /// 更新データが有るとき
                    if (UpdateCount > 0)
                    {
                        /// 最終的に更新を反映する
                        connect.Commit();
                    }
                    /// 更新が終了したテーブルのチェックを外す
                    chkTableNM.Checked = false;
                    return false;
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n　" + connect.Command.CommandText + "\n　" + ConnectServer
                                    + "\n\n　削除処理を中断しました。", "GridData_DELETE " + TitleNM,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        /// <summary>
        /// ダミーロット関連データ検索を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbLotFind_Click(object sender, EventArgs e)
        {
            string strTestLot = "";
            /// レジストリより保存していたテストロット情報を取得
            try
            {
                strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
            }
            catch { }

            if ((dsGridView3.LIST.Rows.Count > 0) || ((strTestLot != null) && (strTestLot != "")))
            {
                updateStatus(0, "　検 証 ロ ッ ト 関 連 デ ー タ 検 索 中　・　・　・");

                /// パネルのレイアウトを停止する
                this.flowLayoutPanel.SuspendLayout();

                /// レジストリのデータが無いとき
                if ((strTestLot == null) || (strTestLot == ""))
                {
                    string LotNo = dsGridView3.LIST[0]["lotno"].ToString();
                    string LotNoTop7 = LotNo.Substring(0, 7);
                    string AddLotNo = "";
                    /// グリッドの情報から生成する
                    strTestLot = "@";
                    for (int intLotNo = 9990; intLotNo <= 9999; intLotNo++)
                    {
                        AddLotNo = LotNoTop7 + intLotNo.ToString();
                        strTestLot += "," + AddLotNo;
                    }
                    strTestLot = strTestLot.Replace("@,", "");
                }
                string[] sptTestLot = strTestLot.Split(',');

                /// チェック項目をクリアする
                this.chkTestData.Checked = false;
                this.chkTestType.Checked = false;
                this.chkProfile.Checked = false;
                this.chkEckPrm.Checked = false;

                /// データクリア
                dsGridView1 = new DsSqlData();
                dsGridView2 = new DsSqlData();
                dsGridView3 = new DsSqlData();
                dsGridView4 = new DsSqlData();
                dsGridView5 = new DsSqlData();
                dsGridView6 = new DsSqlData();
                dsGridView7 = new DsSqlData();
                dsGridView8 = new DsSqlData();
                dsGridView9 = new DsSqlData();
                dsGridView10 = new DsSqlData();
                dsGridView11 = new DsSqlData();
                dsGridView12 = new DsSqlData();
                dsGridView13 = new DsSqlData();
                dsGridView14 = new DsSqlData();
                dsGridView15 = new DsSqlData();
                dsGridView16 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
                dsGridView17 = new DsSqlData();  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）

                /// カラムの並びをリセットするためクリアしたリストの割付けを行う
                dataGridView1.DataSource = dsGridView1.LIST;
                dataGridView2.DataSource = dsGridView2.LIST;
                dataGridView3.DataSource = dsGridView3.LIST;
                dataGridView4.DataSource = dsGridView4.LIST;
                dataGridView5.DataSource = dsGridView5.LIST;
                dataGridView6.DataSource = dsGridView6.LIST;
                dataGridView7.DataSource = dsGridView7.LIST;
                dataGridView8.DataSource = dsGridView8.LIST;
                dataGridView9.DataSource = dsGridView9.LIST;
                dataGridView10.DataSource = dsGridView10.LIST;
                dataGridView11.DataSource = dsGridView11.LIST;
                dataGridView12.DataSource = dsGridView12.LIST;
                dataGridView13.DataSource = dsGridView13.LIST;
                dataGridView14.DataSource = dsGridView14.LIST;
                dataGridView15.DataSource = dsGridView15.LIST;
                dataGridView16.DataSource = dsGridView16.LIST;  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
                dataGridView17.DataSource = dsGridView17.LIST;  /// 2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）


                CommonRead objCommonRead = new CommonRead();
                /// ARMSデータベースを検索
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    /// ステータスバーの更新
                    updateStatus(7, null);

                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLot", ref this.chkTableNM1, ref dsGridView1))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLot", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView1))
                            {
                                this.chkTableNM1.Checked = true;
                            }
                        }
                        /// レジストリ情報を検索結果で書き換える
                        string strRegLot = "@";
                        for (int i = 0; i < dsGridView1.LIST.Rows.Count; i++)
                        {
                            strRegLot += "," + dsGridView1.LIST[i]["lotno"];
                        }
                        strRegLot = strRegLot.Replace("@,", "").Replace("@", "");
                        /// テストロットをレジストリに登録します。
                        fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot, (object)strRegLot);
                    }
                    /// ステータスバーの更新
                    updateStatus(13, null);
                    /// ダミーロットのマガジンデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnMag", ref this.chkTableNM2, ref dsGridView2))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnMag", "lotno", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView2))
                            {
                                this.chkTableNM2.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(20, null);
                    /// ダミーロットのトランザクションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnTran", ref this.chkTableNM3, ref dsGridView3))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            string WhereSql = " WHERE (lotno LIKE '" + sptTestLot[i] + "%')";
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnTran", "", WhereSql, ref dsGridView3))
                            {
                                this.chkTableNM3.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(27, null);
                    /// ダミーロットのコンディションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLotCond", ref this.chkTableNM4, ref dsGridView4))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLotCond", "lotno", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView4))
                            {
                                this.chkTableNM4.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(33, null);
                    /// ダミーロットのロットログデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLotLog", ref this.chkTableNM5, ref dsGridView5))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLotLog", "lotno", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView5))
                            {
                                this.chkTableNM5.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(40, null);
                    /// ダミーロットの•検査データを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnInspection", ref this.chkTableNM6, ref dsGridView6))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnInspection", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView6))
                            {
                                this.chkTableNM6.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(47, null);
                    /// ダミーロットの履歴データを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnHistory", ref this.chkTableNM7, ref dsGridView7))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnHistory", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView7))
                            {
                                this.chkTableNM7.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(53, null);
                    /// ダミーロットの不良データを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnDefect", ref this.chkTableNM8, ref dsGridView8))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnDefect", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView8))
                            {
                                this.chkTableNM8.Checked = true;
                            }
                        }
                    }
                }
                /// LENSデータベースを検索
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrLENSDB, false))
                {
                    /// ステータスバーの更新
                    updateStatus(60, null);
                    /// ダミーロットの監視ロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "LENS", "TnLot", ref this.chkTableNM9, ref dsGridView9))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLot", "LotNO", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView9))
                            {
                                this.chkTableNM9.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(65, null);
                    /// ダミーロットの監視トランザクションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "LENS", "TnTran", ref this.chkTableNM10, ref dsGridView10))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnTran", "LotNO", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView10))
                            {
                                this.chkTableNM10.Checked = true;
                            }
                        }
                    }
                }
                /// QCILデータベースを検索
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrQCILDB, false))
                {
                    /// ステータスバーの更新
                    updateStatus(70, null);
                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TnLOTT", ref this.chkTableNM11, ref dsGridView11))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLOTT", "NascaLot_NO", " = '" + sptTestLot[i] + "'", ref dsGridView11))
                            {
                                this.chkTableNM11.Checked = true;
                            }
                        }
                    }
                    if (dsGridView11.LIST.Rows.Count > 0)
                    {
                        string EquipmentNO = "@";
                        for (int j = 0; j < dsGridView11.LIST.Rows.Count; j++)
                        {
                            if (!EquipmentNO.Contains(dsGridView11.LIST[j]["Equipment_NO"].ToString()))
                            {
                                EquipmentNO += ",'" + dsGridView11.LIST[j]["Equipment_NO"].ToString() + "'";
                            }
                        }
                        EquipmentNO = EquipmentNO.Replace("@,", "");

                        /// ステータスバーの更新
                        updateStatus(75, null);
                        /// ダミーロットの装置異常ログデータを検索
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TnLOG", ref this.chkTableNM12, ref dsGridView12))
                        {
                            string WhereSql = " WHERE (Inline_CD = " + dsGridView11.LIST[0]["Inline_CD"].ToString() + ") "
                                            + " AND (Equipment_NO IN(" + EquipmentNO + ")) ";
                            for (int i = 0; i < sptTestLot.Length; i++)
                            {
                                if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLOG", "", WhereSql + "AND (NascaLot_NO = '" + sptTestLot[i] + "')", ref dsGridView12))
                                {
                                    this.chkTableNM12.Checked = true;
                                }
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(80, null);
                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TnLOGOutSide", ref this.chkTableNM13, ref dsGridView13))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnLOGOutSide", "NascaLot_NO", " = '" + sptTestLot[i] + "'", ref dsGridView13))
                            {
                                this.chkTableNM13.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(85, null);
                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TnQCNR", ref this.chkTableNM14, ref dsGridView14))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnQCNR", "NascaLot_NO", " = '" + sptTestLot[i] + "'", ref dsGridView14))
                            {
                                this.chkTableNM14.Checked = true;
                            }
                        }
                    }
                    if (dsGridView14.LIST.Rows.Count > 0)
                    {
                        string QCNRNO = "@";
                        for (int j = 0; j < dsGridView14.LIST.Rows.Count; j++)
                        {
                            if (!QCNRNO.Contains(dsGridView14.LIST[j]["QCNR_NO"].ToString()))
                            {
                                QCNRNO += "," + dsGridView14.LIST[j]["QCNR_NO"].ToString();
                            }
                        }
                        QCNRNO = QCNRNO.Replace("@,", "");

                        /// ステータスバーの更新
                        updateStatus(90, null);

                        /// ダミーロットの装置異常ログデータを検索
                        if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TnQCNRCnfm", ref this.chkTableNM15, ref dsGridView15))
                        {
                            string WhereSql = " WHERE QCNR_NO IN(" + QCNRNO + ") ";
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnQCNRCnfm", "", WhereSql, ref dsGridView15))
                            {
                                this.chkTableNM15.Checked = true;
                            }
                        }
                    }
                }
                /// ↓↓↓↓↓↓　2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↓↓↓↓↓↓
                /// ARMSデータベースを検索
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    /// ステータスバーの更新
                    updateStatus(95, null);
                    /// ダミーロットの監視ロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnAGVPSTester", ref this.chkTableNM16, ref dsGridView16))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnAGVPSTester", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView16))
                            {
                                this.chkTableNM16.Checked = true;
                            }
                        }
                    }
                    /// ステータスバーの更新
                    updateStatus(100, null);
                    /// ダミーロットの監視トランザクションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnRestrict", ref this.chkTableNM17, ref dsGridView17))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TnRestrict", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView17))
                            {
                                this.chkTableNM17.Checked = true;
                            }
                        }
                    }
                }
                /// ↑↑↑↑↑↑  2019 / 02 / 15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）　↑↑↑↑↑↑


                /// ステータスバー設定
                updateStatus(0, "");
                this.tsslProgressBar.Visible = false;
                this.tsslMessage.Size = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);
                this.statusStrip.Refresh();

                /// データグリッドビューサイズ調整
                this.FVDC100_Resize(null, null);
            }
        }
        /// <summary>
        /// タイプのテキストが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_TextChanged(object sender, EventArgs e)
        {
            bool MatchFlg = false;
            string strType = this.cmbType.Text.Trim();
            if (strType.Length < 6) return;

            /// テキストと一致する最初の位置にインデックスを変更する
            for (int i = 1; i < this.dsType.Name.Rows.Count; i++)
            {
                if (this.dsType.Name[i].Key_CD.Contains(strType))
                {
                    this.cmbType.SelectedIndex = i;
                    MatchFlg = true;
                    break;
                }
            }
            if (AutoSetFlg)
            {
                if (!MatchFlg)
                {
                    /// リストに追加する
                    this.dsType.Name.Rows.Add(new object[] { strType, strType });
                    /// テキストと完全に一致する位置にインデックスを変更する
                    for (int i = 1; i < this.dsType.Name.Rows.Count; i++)
                    {
                        if (this.dsType.Name[i].Key_CD == strType)
                        {
                            this.cmbType.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            /// タイプが一致しなかったとき
            else if (!MatchFlg)
            {
                /// タイプを全て検索し直す
                CommonRead objCommonRead = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    /// 対象タイプドロップダウン作成
                    this.dsType = new DsName();
                    this.dsType.Name.Rows.Add(new object[] { "", "" });
                    objCommonRead.NameRead(sqlInfo, " TmWorkFlow", "TmWorkFlow.typecd", "TmWorkFlow.typecd", " INNER JOIN TnLot ON TmWorkFlow.typecd = TnLot.typecd WHERE TmWorkFlow.delfg  = 0 ", true, ref this.dsType);
                    this.cmbType.DataSource = this.dsType;
                    Transfer.dsType = (DsName)this.dsType.Copy();
                }
                /// テキストと完全に一致する位置にインデックスを変更する
                for (int i = 1; i < this.dsType.Name.Rows.Count; i++)
                {
                    if (this.dsType.Name[i].Key_CD == strType)
                    {
                        this.cmbType.SelectedIndex = i;
                        break;
                    }
                }
            }
            //選択された内容をレジストリに登録します。
            fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultType, strType);
        }

        /// <summary>
        /// ラインのテキストが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLine_TextUpdate(object sender, EventArgs e)
        {
            /// テキストと一致する最初の位置にインデックスを変更する
            for (int i = 1; i < this.dsLine.Name.Rows.Count; i++)
            {
                try
                {
                    if (this.dsLine.Name[i].Key_CD.Contains(this.cmbLine.Text.Trim()))
                    {
                        this.cmbLine.SelectedIndex = i;
                        break;
                    }
                }
                catch { }
            }
            if (AutoSetFlg)
            {
                if (this.cmbLine.SelectedIndex < 0)
                {
                    this.dsLine.Name.Rows.Add(new object[] { this.cmbLine.Text, this.cmbLine.Text });
                }
                //選択された内容をレジストリに登録します。
                fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultLine, this.cmbLine.Text);
            }
        }

        /// <summary>
        /// ロットのテキストが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLot_TextUpdate(object sender, EventArgs e)
        {
            bool MatchFlg = false;
            /// テキストと一致する最初の位置にインデックスを変更する
            for (int i = 1; i < this.dsLot.Name.Rows.Count; i++)
            {
                if (this.dsLot.Name[i].Key_CD.Contains(this.cmbLot.Text.Trim()))
                {
                    this.cmbLot.SelectedIndex = i;
                    MatchFlg = true;
                    break;
                }
            }

            /// ロットが一致しなかったとき
            if (!MatchFlg)
            {
                CommonRead objCommonRead = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    string FindLot = this.cmbLot.Text.Trim();
                    /// ロットで該当タイプとラインを検索する
                    DsName dsFondLot = new DsName();
                    if (objCommonRead.NameRead(sqlInfo, " TnLot", "typecd", "macgroup", " WHERE lotno = '" + FindLot + "'", false, ref dsFondLot))
                    {
                        /// 読めたとき
                        if (dsFondLot.Name.Rows.Count == 1)
                        {
                            /// タイプ、ライン、ロットを再設定する
                            AutoSetFlg = true;
                            this.cmbType.Text = dsFondLot.Name[0].Key_CD;
                            this.cmbLine.Text = dsFondLot.Name[0].Data_NM;
                            AutoSetFlg = false;

                            /// 参照ロットを前７桁で再検索しドロップダウン作成
                            this.dsLot = new DsName();
                            this.dsLot.Name.Rows.Add(new object[] { "", "" });
                            string WhereSql = " WHERE (typecd = '" + dsFondLot.Name[0].Key_CD + "') AND (macgroup = '" + dsFondLot.Name[0].Data_NM
                                                        + "') AND (lotno LIKE '" + FindLot.Substring(0, 7) + "%') ORDER BY lastupddt DESC";
                            objCommonRead.TopNameRead(sqlInfo, " TOP (50) ", "TnLot", "lotno", "lotno", WhereSql, false, ref this.dsLot);
                            this.cmbLot.DataSource = this.dsLot;

                            this.cmbLot.Text = FindLot;
                            MatchFlg = false;
                            /// テキストと一致する最初の位置にインデックスを変更する
                            for (int i = 1; i < this.dsLot.Name.Rows.Count; i++)
                            {
                                if (this.dsLot.Name[i].Key_CD == FindLot)
                                {
                                    this.cmbLot.SelectedIndex = i;
                                    MatchFlg = true;
                                    break;
                                }
                            }
                            /// 一致しなかったとき強制的にレコードを追加する
                            if (!MatchFlg)
                            {
                                this.dsLot.Name.Rows.Add(new object[] { FindLot, FindLot });
                                this.cmbLot.DataSource = this.dsLot;
                                this.cmbLot.SelectedIndex = this.dsLot.Name.Rows.Count - 1;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// テストタイプ/プロファイル等検索が押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbTestTypeProfFind_Click(object sender, EventArgs e)
        {

            /// タイプを取得する
            string seltype;
            try
            {
                seltype = this.cmbType.SelectedValue.ToString();
            }
            catch
            {
                seltype = this.cmbType.Text;
            }
            if (seltype.Trim() == "") return;

            seltype = seltype + "_TEST";

            /// チェック項目をクリアする
            this.chkTestData.Checked = false;
            this.chkTestType.Checked = false;
            this.chkProfile.Checked = false;
            this.chkEckPrm.Checked = false;

            /// データクリア
            dsGridView13 = new DsSqlData();
            dsGridView14 = new DsSqlData();
            dsGridView15 = new DsSqlData();

            /// カラムの並びをリセットするためクリアしたリストの割付けを行う
            dataGridView13.DataSource = dsGridView13.LIST;
            dataGridView14.DataSource = dsGridView14.LIST;
            dataGridView15.DataSource = dsGridView15.LIST;

            CommonRead objCommonRead = new CommonRead();
            /// ARMSデータベースを検索
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TmWorkFlow", ref this.chkTableNM13, ref dsGridView13))
                {
                    if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TmWorkFlow", "typecd", " = '" + seltype + "'", ref dsGridView13))
                    {
                        //this.chkTableNM13.Checked   = true;
                    }
                }
                /// タイプのオーブンプロファイルを検索
                if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TmOvenProf", ref this.chkTableNM14, ref dsGridView14))
                {
                    if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TmOvenProf", "typecd", " = '" + seltype + "'", ref dsGridView14))
                    {
                        //this.chkTableNM14.Checked   = true;
                    }
                }
            }

            /// QCILデータベースを検索
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrQCILDB, false))
            {
                if (objCommonRead.SqlDataSetCreate(sqlInfo, "QCIL", "TmPLM", ref this.chkTableNM15, ref dsGridView15))
                {
                    string WhereSql = " WHERE (Model_NM = 'ECK') AND (Del_FG = 0) AND (Material_CD = '" + seltype + "')";
                    if (objCommonRead.SqlDataFind(sqlInfo, "削除", "TmPLM", "", WhereSql, ref dsGridView15))
                    {
                        //this.chkTableNM15.Checked   = true;
                    }
                }

            }

            /// 何か読めたとき
            if ((this.dsGridView13.LIST.Rows.Count > 0)
                || (this.dsGridView14.LIST.Rows.Count > 0)
                || (this.dsGridView15.LIST.Rows.Count > 0))
            {
                /// データグリッドビューサイズ調整
                this.FVDC100_Resize(null, null);
            }
        }

        /// <summary>
        /// テーブルのチェックを変更したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTableNM_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox checkSend = (System.Windows.Forms.CheckBox)sender;
            if (checkSend.Checked)
            {
                UpdateTableCount++;
            }
            else
            {
                UpdateTableCount--;
            }
        }


        /// <summary>
        /// セルの上にマウスを持って行ったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            System.Windows.Forms.DataGridView dataGridView = new DataGridView();
            dataGridView = (DataGridView)sender;
            try
            {
                string gridColumn = dataGridView.Columns[e.ColumnIndex].Name;
                string gridData = dataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
                /// マウスの位置により内容を表示する
                switch (gridColumn)
                {

                    /// 工程コードのとき
                    case "procno":
                    case "inlineprocno":
                    case "Process_CD":
                        for (int i = 0; i < Transfer.dsProcess.Name.Rows.Count; i++)
                        {
                            if (gridData == Transfer.dsProcess.Name[i].Key_CD)
                            {
                                this.toolTip.Show(Transfer.dsProcess.Name[i].Key_CD + ":" + Transfer.dsProcess.Name[i].Data_NM, (IWin32Window)sender, 1000);
                                break;
                            }
                        }
                        break;
                    /// 工程コードのとき
                    case "workcd":
                    case "Work_CD":
                        for (int i = 0; i < Transfer.dsWorkcd.Name.Rows.Count; i++)
                        {
                            if (gridData == Transfer.dsWorkcd.Name[i].Key_CD)
                            {
                                string[] sptName = Transfer.dsWorkcd.Name[i].Data_NM.Split('(');
                                this.toolTip.Show(sptName[0], (IWin32Window)sender, 1000);
                                break;
                            }
                        }
                        break;

                }
            }
            catch { }
        }

        /// <summary>
        /// サーバー表示上にカーソルを持って行ったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsslServer_MouseEnter(object sender, EventArgs e)
        {
            /// ツールヒントを表示する
            this.toolTip.Show(this.tsslServer.ToolTipText, (IWin32Window)this.statusStrip, 1000);
        }

        /// <summary>
        /// テストロット一時記録をクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbLotReg_Click(object sender, EventArgs e)
        {
            /// 設定画面を表示する
            FVDC120.ShowDialog();

            /// レジストリより保存していたテストロット情報を取得
            string strTestLot = "";
            try
            {
                strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
            }
            catch { }
            dsGridView10 = new DsSqlData();
            dsGridView11 = new DsSqlData();
            dsGridView12 = new DsSqlData();

            /// データが有るとき
            if ((strTestLot != null) && (strTestLot != ""))
            {
                CommonRead objCommonRead = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    string[] sptTestLot = strTestLot.Split(',');

                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLot", ref this.chkTableNM10, ref dsGridView10))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnLot", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView10))
                            {
                                //this.chkTableNM10.Checked       = true;
                            }
                        }
                    }
                    /// ダミーロットのマガジンデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnMag", ref this.chkTableNM11, ref dsGridView11))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnMag", "lotno", " LIKE '" + sptTestLot[i] + "%'", ref dsGridView11))
                            {
                                //this.chkTableNM11.Checked       = true;
                            }
                        }
                    }
                    /// ダミーロットのトランザクションデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnTran", ref this.chkTableNM12, ref dsGridView12))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            string WhereSql = " INNER JOIN TmWorkFlow ON dbo.TnTran.procno = dbo.TmWorkFlow.procno "
                                            + " INNER JOIN TnLot ON LEFT(TnTran.lotno, 11) = TnLot.lotno AND TmWorkFlow.typecd = REPLACE(dbo.TnLot.typecd, '_TEST', '') "
                                            + " WHERE (TnTran.lotno LIKE '" + sptTestLot[i] + "%')"
                                            + " ORDER BY TmWorkFlow.workorder , TnTran.lotno";
                            if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnTran", "", WhereSql, ref dsGridView12))
                            {
                                //this.chkTableNM12.Checked       = true;
                            }
                        }
                    }
                }
            }
            /// データグリッドビューサイズ調整
            this.FVDC100_Resize(null, null);
        }

        /// <summary>
        /// 対象タイプラベルをダブルクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblType_DoubleClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (this.lblType.Text == "対象タイプ")
            {
                this.lblType.Text = "テストタイプ";
            }
            else
            {
                this.lblType.Text = "対象タイプ";
            }
            /// タイプ・ロットドロップダウン作成
            cmbType_cmbLot_Create(false);


            /// 画面をリセットする
            FVDC100_Reset();
        }

        /// <summary>
        /// タイプ・ロットドロップダウン作成
        /// </summary>
        /// <param name="TypeSet"></param>
        private void cmbType_cmbLot_Create(bool TypeSet)
        {
            Cursor.Current = Cursors.WaitCursor;
            CommonRead objCommonRead = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                string WhereSql;
                if (TypeSet)
                {
                    /// 参照タイプドロップダウン作成
                    this.dsType = new DsName();
                    this.dsType.Name.Rows.Add(new object[] { "", "" });
                    if (this.lblType.Text == "対象タイプ")
                    {
                        //WhereSql            = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                        //                    + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                        //                    + "AND TnTran.procno = TmWorkFlow.procno "
                        //                    + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                        //                    + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                        //                    + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                        WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow.workorder > 1) GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TnLot", "TnLot.typecd", "TnLot.typecd", WhereSql, false, ref this.dsType);
                    }
                    else
                    {
                        WhereSql = " INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                            + "WHERE (TmWorkFlow_1.typecd LIKE N'%_TEST')"
                                            + " GROUP BY TmWorkFlow.typecd ORDER BY TmWorkFlow.typecd";
                        objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TmWorkFlow", "TmWorkFlow.typecd", "TmWorkFlow.typecd", WhereSql, false, ref this.dsType);
                    }

                    this.cmbType.DataSource = this.dsType;
                    Transfer.dsType = (DsName)this.dsType.Copy();
                }
                else
                {

                    /// 参照ロットドロップダウン作成
                    this.dsLot = new DsName();
                    this.dsLot.Name.Rows.Add(new object[] { "", "" });
                    if (this.lblType.Text == "対象タイプ")
                    {
                        WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                            + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                    }
                    else
                    {
                        WhereSql = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "INNER JOIN TmWorkFlow AS TmWorkFlow_1 ON TmWorkFlow.typecd = REPLACE(TmWorkFlow_1.typecd, '_TEST', '') "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow_1.typecd LIKE N'%_TEST') "
                                            + "AND (TmWorkFlow.workorder = (SELECT MAX(workorder) AS Expr1 FROM TmWorkFlow AS TmWorkFlow_1 "
                                            + "WHERE (delfg = 0) AND (typecd = TmWorkFlow.typecd))) ORDER BY TnLot.lastupddt DESC";
                    }
                    objCommonRead.TopNameRead(sqlInfo, " TOP (20) ", "TnLot", "TnLot.lotno", "TnLot.lotno", WhereSql, false, ref this.dsLot);
                    this.cmbLot.DataSource = this.dsLot;
                }
            }
            /// 下位のデータをクリアする
            this.dsProcess = new DsName();
            this.dsProcess.Name.Rows.Add(new object[] { "", "" });
            this.cmbProcess.DataSource = this.dsProcess;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// タイプ切替をクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbTypeReplace_Click(object sender, EventArgs e)
        {

            /// 設定画面を表示する
            FVDC130.ShowDialog();

            /// レジストリより保存していたテストロット情報を取得
            string strTestLot = "";
            try
            {
                strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
            }
            catch { }
            dsGridView10 = new DsSqlData();

            /// データが有るとき
            if ((strTestLot != null) && (strTestLot != ""))
            {
                CommonRead objCommonRead = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    string[] sptTestLot = strTestLot.Split(',');

                    /// ダミーロットのロットデータを検索
                    if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TnLot", ref this.chkTableNM10, ref dsGridView10))
                    {
                        for (int i = 0; i < sptTestLot.Length; i++)
                        {
                            if (objCommonRead.SqlDataFind(sqlInfo, "照会", "TnLot", "lotno", " = '" + sptTestLot[i] + "'", ref dsGridView10))
                            {
                                //this.chkTableNM10.Checked       = true;
                            }
                        }
                    }
                }
            }
            /// データグリッドビューサイズ調整
            this.FVDC100_Resize(null, null);
        }
        /// <summary>
        /// マッピング解析を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMapping_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            /// マッピング解析画面を表示する
            Transfer.TypeNM = this.cmbType.SelectedText;
            if (Transfer.TypeNM == "")
            {
                try
                {
                    Transfer.TypeNM = this.cmbType.SelectedValue.ToString();
                }
                catch { }
            }
            try
            {
                FVDC200.ShowDialog(this);
            }
            catch
            {
                FVDC200 = new FVDC200();
                FVDC200.ShowDialog(this);
            }

        }
        /// <summary>
        /// 流動規制解除を押したとき　2019/02/15 SLA2.Uchida AGV関連対応修正（TnAGVPSTester、TnRestrict）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDischarge_Click(object sender, EventArgs e)
        {
            /// メッセージを表示する
            DialogResult OkNgButton = MessageBox.Show("　テストロットに掛かっている流動規制を全て解除します。"
                            + "\n\n　そのまま解除を続行する場合にはＯＫを押して下さい。", "流動規制解除開始確認",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            /// キャンセルが押されたとき、処理を終了する
            if (OkNgButton == System.Windows.Forms.DialogResult.Cancel) return;

            string strTestLot = "";
            /// レジストリより保存していたテストロット情報を取得
            try
            {
                strTestLot = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
            }
            catch { }

            if ((dsGridView3.LIST.Rows.Count > 0) || ((strTestLot != null) && (strTestLot != "")))
            {
                /// レジストリのデータが無いとき
                if ((strTestLot == null) || (strTestLot == ""))
                {
                    string LotNo = dsGridView3.LIST[0]["lotno"].ToString();
                    string LotNoTop7 = LotNo.Substring(0, 7);
                    string AddLotNo = "";
                    /// グリッドの情報から生成する
                    strTestLot = "@";
                    for (int intLotNo = 9990; intLotNo <= 9999; intLotNo++)
                    {
                        AddLotNo = LotNoTop7 + intLotNo.ToString();
                        strTestLot += "," + AddLotNo;
                    }
                    strTestLot = strTestLot.Replace("@,", "");
                }
                string[] sptTestLot = strTestLot.Split(',');
                string strLotList = "@";
                for (int i = 0; i < sptTestLot.Length; i++)
                {
                    strLotList = ",'" + sptTestLot[i] + "'";
                }
                strLotList = strLotList.Replace("@,", "");


                /// トランザクション更新を行う
                using (IConnection connect = ServerConnection.CreateInstance(Constant.StrARMSDB, true))
                {
                    try
                    {
                        /// 論理削除SQL作成
                        string sql = "UPDATE TnRestrict SET delfg = 1 WHERE lotno IN(" + strLotList + ") AND delfg = 0 ";
                        /// 削除
                        connect.Command.CommandText = sql;
                        int ExecuteCount = connect.Command.ExecuteNonQuery();
                        /// 更新データが有るとき
                        if (ExecuteCount > 0)
                        {
                            /// 最終的に更新を反映する
                            connect.Commit();
                        }
                        else
                        {
                            MessageBox.Show("　流動規制解除対象となるデータは存在しませんでした。", "tsbDischarge_Click ",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ///TODO: エラー処理
                        MessageBox.Show("　" + ex.Number + ":" + ex.Message + "\n　" + connect.Command.CommandText
                                        + "\n\n　流動規制解除処理は行われませんでした。", "tsbDischarge_Click ",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        connect.Close();
                    }

                }
            }
        }
        /// <summary>
        /// 設備切替をクリックしたとき       2020/01/28 SLA2.Uchida 設備切替機能追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMacChange_Click(object sender, EventArgs e)
        {
            /// 設備切替画面を表示する
            FVDC300.ShowDialog();
        }
        /// <summary>
        /// 設備ロット検索をクリックしたとき　2020/01/28 SLA2.Uchida 設備ロット検索機能追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMacLot_Click(object sender, EventArgs e)
        {
            Transfer.DeviceID = "";
            /// 設備入力画面を表示する
            FVDC140.ShowDialog();
            if (Transfer.DeviceID.Length != 6) return;

            try
            {
                CommonRead objCommonRead = new CommonRead();
                using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                {
                    string EmpList = "@";
                    DsName dsEmpList = new DsName();
                    if (objCommonRead.NameRead(sqlInfo, "TmMachine", "macno", "macno", "WHERE plantcd = '" + Transfer.DeviceID + "'", true, ref dsEmpList))
                    {
                        EmpList = "@";
                        for (int i = 0; i < dsEmpList.Name.Rows.Count; i++)
                        {
                            EmpList += "," + dsEmpList.Name[i].Data_NM;
                        }
                        EmpList = EmpList.Replace("@,", "").Replace("@", "");
                    }

                    /// 参照ロットドロップダウン作成
                    this.dsLot = new DsName();
                    this.dsLot.Name.Rows.Add(new object[] { "", "" });
                    string WhereSql = " WHERE (delfg = 0) "
                                                + " AND (macno IN(" + EmpList + ")) "
                                                + " ORDER BY enddt DESC";

                    objCommonRead.TopNameRead(sqlInfo, " TOP (50) ", "TnTran", "lotno", "lotno", WhereSql, false, ref this.dsLot);

                    this.cmbLot.DataSource = this.dsLot;
                    this.cmbLot.SelectAll();
                    this.cmbLot.DroppedDown = true;
                    this.cmbLot.Refresh();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
