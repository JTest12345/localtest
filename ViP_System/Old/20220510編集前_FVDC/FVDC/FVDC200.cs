/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC200 マッピングファイル解析
 * 
 * 概略           : 各種マッピングファイルを解析して実際のフレーム配置に編集表示する。
 *                  外観検査機の検査結果ファイルについては別のマッピングデータとの照合が可能。
 *                  テキスト形式のファイルについてはグリッド上で編集して上書き更新が可能。
 * 
 * 作成           : 2018/11/08 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Excel;

namespace FVDC
{
    public partial class FVDC200 : Form
    {
        internal NotifyStatusHandler notifyHander;
        private string DragName                 = "";
        private DsFree dsCSV                    = new DsFree();
        private bool   DifferenceFG             = false;
        private Form   OwnerForm;
        public FVDC200()
        {
            InitializeComponent();

            /// ステータスバー設定
            this.tsslProgressBar.Visible        = false;
            this.tsslMessage.Size               = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);

            //string strTitle                     = Properties.Resources.CSV_SD + ",ﾃﾞｨｽﾍﾟﾝｽﾀｲﾑ計,判定 ﾃﾞｨｽﾍﾟﾝｻ計";
            //string[] sptTitle                   = strTitle.Split(',');
            //sptTitle[0]                         = "";
            //this.cmbTitle.Items.AddRange(sptTitle);
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC200_Load(object sender, EventArgs e)
        {
            OwnerForm                           = this.Owner;
            this.tsslServer.Text                = Transfer.ServerName;

            /// タイプ情報を自動設定
            this.dsType                         = (DsName)Transfer.dsType.Copy();
            this.cmbType.DataSource             = this.dsType;
            if (Transfer.TypeNM != "")
            {
                this.cmbType.SelectedText       = Transfer.TypeNM;
                this.cmbType.SelectedValue      = Transfer.TypeNM;
                this.cmbType.DroppedDown        = false;
            }
            /// リセットを押したとき
            tsbReset_Click(null, null);
        }
        /// <summary>
        /// ドラッグしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flowLayoutPanel_DragOver(object sender, DragEventArgs e)
        {            
            /// ドラッグしたファイルの情報を取得する。
            try
            {
                object FileNasme    = e.Data.GetData("FileNameW");
                string[] strFile    = (string[])FileNasme;
                DragName            = strFile[0];
                e.Effect            = DragDropEffects.Link;
            }
            catch { }
        }
        /// <summary>
        /// ドラッグドロップしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flowLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect                                    = DragDropEffects.None;
            this.btnFileWrite.Visible                   = false;
            this.cmbTitle.Visible                       = false;
            DifferenceFG                                = false;
            try
            {
                if (DragName != "")
                {
                    this.lblTitle.Visible               = false;
                    this.cmbTitle.Visible               = false;
                    bool NoticingFG                     = false;
                    this.TopMost                        = true;
                    /// マウスカーソルの変更
                    Cursor.Current                      = Cursors.WaitCursor;
                    notifyHander                        = new NotifyStatusHandler(updateStatus);
                    notifyHander(0, "　マ　ッ　ピ　ン　グ　デ　ー　タ　解　析　中　・　・　・");
                    this.TopMost                        = false;

                    this.flowLayoutPanel.AutoScroll     = false;
                    this.flowLayoutPanel.SuspendLayout();

                    if ((this.cmbType.SelectedValue == null)
                        || (this.cmbType.SelectedValue.ToString() == ""))
                    {
                        MessageBox.Show("　最初にタイプを選択して下さい。", "タイプ選択",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.cmbType.DroppedDown        = true;
                        return;
                    }

                    FileInfo fileInfo                   = new FileInfo(DragName);

                    /// 縦横列データ等取得
                    int ixX                             = Convert.ToInt32(this.lblXPackage.Text.Replace("X：", ""));
                    int ixY                             = Convert.ToInt32(this.lblYPackage.Text.Replace("Y：", ""));
                    int MaxCT                           = ixX * ixY;

                    /// テキストファイルのとき
                    if ((fileInfo.Name.ToLower().Contains(".wbm"))
                        || (fileInfo.Name.ToLower().Contains(".mpd")))
                    {
                        this.flowLayoutPanel.Controls.Clear();
                        /// ストリームファイルとして読込む
                        StreamReader fs                 = new StreamReader(DragName);
                        string strData                  = fs.ReadToEnd();
                        fs.Close();
                        if (strData.Contains(","))
                        {
                            string[] sptData            = strData.Split(',');

                            int FlmCT                   = sptData.Length / MaxCT;

                            /// 雛形データセットの作成
                            DsFree dsMapping            = new DsFree();
                            dsMapping.List.Columns.Clear();
                            /// 横列
                            for (int i = 0; i < ixX; i++)
                            {
                                dsMapping.List.Columns.Add(i.ToString(), Type.GetType("System.String"));
                                dsMapping.List.Columns[i.ToString()].Caption        = i.ToString();
                                dsMapping.List.Columns[i.ToString()].DefaultValue   = "";
                            }
                            /// 縦列
                            for (int i = 0; i < ixY; i++)
                            {
                                dsMapping.List.Rows.Add(new object[] { "" });
                            }

                            /// 初期化
                            int SetCT                   = 0;
                            int FLMCT                   = 0;
                            int SetX                    = ixX - 1;
                            int SetY                    = -1;
                            bool ChangeFG               = false;

                            /// 雛形から設定用データセットにコピー
                            DsFree dsMappingData        = (DsFree)dsMapping.Copy();

                            /// 反転設定するとき
                            if (this.chkInvert.Checked)
                            {
                                /// 先に全フレームを作成する
                                for (int i = 0; i < FlmCT; i++)
                                {
                                    /// データグリッドをフローレイアウトに追加する
                                    System.Windows.Forms.Label lblGridTitle         = new System.Windows.Forms.Label();
                                    DataGridView dataGridView                       = new DataGridView();
                                    try
                                    {
                                        FLMCT++;
                                        ///グリッドラベル定義
                                        lblGridTitle.AutoSize                       = true;
                                        lblGridTitle.Font                           = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                                        lblGridTitle.Name                           = "lbl_" + FLMCT.ToString();
                                        lblGridTitle.Text                           = "【" + FLMCT.ToString() + "】";
                                        lblGridTitle.Margin                         = new System.Windows.Forms.Padding(3, 6, 0, 0);
                                        /// フローレイアウトに追加する
                                        this.flowLayoutPanel.Controls.Add(lblGridTitle);
                                        ///グリッド定義
                                        dataGridView.AllowUserToAddRows             = false;
                                        dataGridView.AllowUserToDeleteRows          = false;
                                        dataGridView.ColumnHeadersHeightSizeMode    = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                                        dataGridView.ColumnHeadersVisible           = false;
                                        dataGridView.RowHeadersVisible              = false;
                                        dataGridView.ImeMode                        = ImeMode.Off;
                                        dataGridView.Name                           = "FLM_" + FLMCT.ToString();
                                        dataGridView.ReadOnly                       = false;
                                        dataGridView.RowTemplate.Height             = 16;
                                        dataGridView.DataSource                     = dsMapping.List.Copy();
                                        /// フローレイアウトに追加する
                                        this.flowLayoutPanel.Controls.Add(dataGridView);
                                        this.flowLayoutPanel.SetFlowBreak(dataGridView, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        /// フローレイアウトに追加出来なくなったとき
                                        MessageBox.Show(ex.Message, "表示エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        dataGridView?.Dispose();
                                        return;
                                    }
                                }
                            }

                            /// データ数分繰り返す
                            for (int i = 0; i < sptData.Length; i++)
                            {
                                /// カウント
                                SetCT++;
                                SetY++;
                                /// データを設定用データセットにセット
                                try
                                {
                                    dsMappingData.List[SetY][SetX]                  = sptData[i];
                                }
                                catch (Exception ex)
                                {
                                    string Message                                  = ex.Message;
                                }
                                /// 縦１列が終了したとき
                                if (SetCT % ixY == 0)
                                {
                                    SetX                                            = SetX - 1;
                                    SetY                                            = -1;
                                }

                                /// １フレーム終了時
                                if (SetCT == MaxCT)
                                {
                                        
                                    /// カウンター類をリセット
                                    SetCT                                               = 0;
                                    SetX                                                = ixX - 1;
                                    SetY                                                = -1;
                                    ChangeFG                                            = false;
                                    /// 反転設定するとき
                                    if (this.chkInvert.Checked)
                                    {
                                        /// フレームグリッドを割付け
                                        string strFLM_NM                                = "FLM_" + FLMCT.ToString();
                                        DataGridView dataGridView                       = (DataGridView)this.flowLayoutPanel.Controls[strFLM_NM];
                                        FLMCT--;

                                        for (int j = 0; j < ixX; j++)
                                        {
                                            dataGridView.Columns[j].Width               = 14;
                                            dataGridView.Columns[j].Resizable           = DataGridViewTriState.False;
                                            for (int k = 0; k < ixY; k++)
                                            {
                                                dataGridView[j, k].Value                = dsMappingData.List[k][j].ToString();

                                                switch (dataGridView[j, k].Value.ToString())
                                                {
                                                    case "": /// 未設定セルのとき
                                                        dataGridView[j, k].Style.BackColor  = Color.Blue;
                                                        NoticingFG                          = true;
                                                        break;
                                                    case "0":
                                                        dataGridView[j, k].Style.BackColor  = Color.White;
                                                        break;
                                                    default:
                                                        dataGridView[j, k].Style.BackColor  = Color.Red;
                                                        DifferenceFG                        = true;
                                                        break;
                                                }
                                            }
                                        }
                                        /// ステータスバーの更新
                                        notifyHander((FlmCT - FLMCT) * 100 / FlmCT, null);
                                        this.btnFileWrite.Visible                       = true;
                                    }
                                    /// 反転しないとき
                                    else
                                    {
                                        /// データグリッドをフローレイアウトに追加する
                                        System.Windows.Forms.Label lblGridTitle         = new System.Windows.Forms.Label();
                                        DataGridView dataGridView                       = new DataGridView();
                                        try
                                        {
                                            FLMCT++;
                                            ///グリッドラベル定義
                                            lblGridTitle.AutoSize                       = true;
                                            lblGridTitle.Font                           = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                                            lblGridTitle.Name                           = "lbl_" + FLMCT.ToString();
                                            lblGridTitle.Text                           = "【" + FLMCT.ToString() + "】";
                                            lblGridTitle.Margin                         = new System.Windows.Forms.Padding(3, 6, 0, 0);
                                            /// フローレイアウトに追加する
                                            this.flowLayoutPanel.Controls.Add(lblGridTitle);
                                            ///グリッド定義
                                            dataGridView.AllowUserToAddRows             = false;
                                            dataGridView.AllowUserToDeleteRows          = false;
                                            dataGridView.ColumnHeadersHeightSizeMode    = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                                            dataGridView.ColumnHeadersVisible           = false;
                                            dataGridView.RowHeadersVisible              = false;
                                            dataGridView.ImeMode                        = ImeMode.Off;
                                            dataGridView.Name                           = "FLM_" + FLMCT.ToString();
                                            dataGridView.ReadOnly                       = false;
                                            dataGridView.RowTemplate.Height             = 16;
                                            dataGridView.DataSource                     = dsMapping.List.Copy();
                                            /// フローレイアウトに追加する
                                            this.flowLayoutPanel.Controls.Add(dataGridView);
                                            this.flowLayoutPanel.SetFlowBreak(dataGridView, true);
                                            for (int j = 0; j < ixX; j++)
                                            {
                                                dataGridView.Columns[j].Width           = 14;
                                                dataGridView.Columns[j].Resizable       = DataGridViewTriState.False;
                                                for (int k = 0; k < ixY; k++)
                                                {
                                                    dataGridView[j, k].Value            = dsMappingData.List[k][j].ToString();

                                                    switch (dataGridView[j, k].Value.ToString())
                                                    {
                                                        case "": /// 未設定セルのとき
                                                            dataGridView[j, k].Style.BackColor  = Color.Blue;
                                                            NoticingFG                          = true;
                                                            break;
                                                        case "0":
                                                            dataGridView[j, k].Style.BackColor  = Color.White;
                                                            break;
                                                        default:
                                                            dataGridView[j, k].Style.BackColor  = Color.Red;
                                                            DifferenceFG                        = true;
                                                            break;
                                                    }
                                                }
                                            }
                                            /// ステータスバーの更新
                                            notifyHander(FLMCT * 100 / FlmCT, null);
                                            this.btnFileWrite.Visible                   = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            /// フローレイアウトに追加出来なくなったとき
                                            MessageBox.Show(ex.Message, "表示エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            dataGridView?.Dispose();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    /// CSVファイルのとき
                    else if (fileInfo.Name.ToLower().Contains(".csv"))
                    {
                        int SiftCT                                      = 1;
                        /// 対象ファイルのとき
                        if ((fileInfo.Name.Substring(0, 2) == "MM")
                            || (fileInfo.Name.Substring(0, 2) == "ME")
                            || (fileInfo.Name.Contains("_MM"))
                            || (fileInfo.Name.Contains("_SD")))
                        {
                            /// エクセルファイルとして読込む
                            dsCSV                               = new DsFree();
                            using (ExcelIO exRead = new ExcelIO())
                            {
                                if (fileInfo.Name.Substring(0, 2) == "MM")
                                {
                                    this.flowLayoutPanel.Controls.Clear();
                                    /// CSVファイルデータを取得しデータセットに書き込む
                                    exRead.CSV_Read(DragName, Properties.Resources.CSVMM, ref SiftCT, ref dsCSV, notifyHander);

                                    /// CSVファイル内容をグリッドにセットする
                                    CSV_GridSet(Properties.Resources.CSVMM, SiftCT);
                                }
                                else if (fileInfo.Name.Substring(0, 2) == "ME")
                                {
                                    this.flowLayoutPanel.Controls.Clear();
                                    /// CSVファイルデータを取得しデータセットに書き込む
                                    exRead.CSV_Read(DragName, Properties.Resources.CSVME, ref SiftCT, ref dsCSV, notifyHander);

                                    /// CSVファイル内容をグリッドにセットする
                                    CSV_GridSet(Properties.Resources.CSVME, SiftCT);
                                }
                                else if (fileInfo.Name.Contains("_MM"))
                                {
                                    /// CSVファイルデータを取得しデータセットに書き込む
                                    exRead.CSV_Read(DragName, Properties.Resources.CSV_MM, ref SiftCT, ref dsCSV, notifyHander);
                                    /// 既存グリッドが無いとき
                                    if (this.flowLayoutPanel.Controls.Count == 0)
                                    {
                                        /// CSVファイル内容をグリッドにセットする
                                        CSV_GridSet(Properties.Resources.CSV_MM, SiftCT);
                                    }
                                    else
                                    {
                                        /// CSVファイル内容を既存グリッドに上書きする
                                        CSV_GridOverwrite();
                                    }
                                }
                                else if (fileInfo.Name.Contains("_SD"))
                                {
                                    this.flowLayoutPanel.Controls.Clear();
                                    /// CSVファイルデータを取得しデータセットに書き込む
                                    exRead.CSV_Read(DragName, Properties.Resources.CSV_SD, ref SiftCT, ref dsCSV, notifyHander);

                                    /// CSVファイル内容をグリッドにセットする
                                    CSV_GridSet(Properties.Resources.CSV_SD, SiftCT);

                                    //this.lblTitle.Visible       = true;
                                    //this.cmbTitle.Visible       = true;
                                    ///// メッセージを表示する
                                    //MessageBox.Show("　表示項目を選択して下さい。", "表示項目選択要求",
                                    //                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //this.cmbTitle.DroppedDown   = true;
                                    //return;
                                }
                            }
                            /// グリッドに設定する
                        }   
                        else
                        {
                            MessageBox.Show("　マッピングファイルで無いため読込出来ません。\n\n" +
                                "　内容を確認して再度読込願います。", "読込対象外", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    if (NoticingFG)
                    {
                        MessageBox.Show("　フレーム情報とデータ数が一致していません。\n\n" +
                            "　選択タイプが違いますので御確認願います。", "不一致警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                /// 差異が無いとき
                if (!DifferenceFG)
                {
                    MessageBox.Show("　差異が有るデータが存在しません。\n\n" +
                        "　全て同じデータとなります。", "差異無し", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string Message                              = ex.Message;
            }
            finally
            {
                /// ステータスバーの更新
                notifyHander(100, null);

                /// 画面サイズが変更されたときを実行し再描画する
                FVDC200_Resize(null, null);

                /// マウスカーソルを元に戻します。
                notifyHander(0, "");
                Cursor.Current                              = Cursors.Default;
            }
        }
        /// <summary>
        /// タイプを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((this.cmbType.SelectedValue == null)
                || (this.cmbType.SelectedValue.ToString() == ""))
            {
                this.cmbType.DroppedDown        = true;
                return;
            }
            /// フレームサイズを検索し表示する

            CommonRead objCommonRead            = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrLENSDB, false))
            {
                string WhereSql                 = " INNER JOIN TmMag ON TmType.MagazineID = TmMag.MagazineID"
                                                + " WHERE (TmType.TypeCD = '" + this.cmbType.SelectedValue.ToString() + "')";
                DsName dsName                   = new DsName();
                if (objCommonRead.NameRead(sqlInfo, "TmType", "TmMag.FrameXPackage", "TmMag.FrameYPackage", WhereSql, false, ref dsName))
                {
                    if (dsName.Name.Rows.Count > 0)
                    {
                        this.lblXPackage.Text   = "X：" + dsName.Name[0].Key_CD;
                        this.lblYPackage.Text   = "Y：" + dsName.Name[0].Data_NM;
                    }
                    else
                    {
                        WhereSql                = " INNER JOIN TmMag ON TmType.MagazineID = TmMag.MagazineID"
                                                + " WHERE (TmType.TypeCD LIKE '" + this.cmbType.SelectedValue.ToString().Substring(0,7) + "%')";
                        if (objCommonRead.NameRead(sqlInfo, "TmType", "TmMag.FrameXPackage", "TmMag.FrameYPackage", WhereSql, false, ref dsName))
                        {
                            if (dsName.Name.Rows.Count > 0)
                            {
                                this.lblXPackage.Text   = "X：" + dsName.Name[0].Key_CD;
                                this.lblYPackage.Text   = "Y：" + dsName.Name[0].Data_NM;
                            }
                        }
                    }
                }

            }

        }
        /// <summary>
        /// エクセル出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExcelOut_Click(object sender, EventArgs e)
        {
            /// マウスカーソルの変更
            Cursor.Current              = Cursors.WaitCursor;
            notifyHander                = new NotifyStatusHandler(updateStatus);
            try
            {
                notifyHander(0, "　エ　ク　セ　ル　出　力　中　・　・　・");
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
                    long SetFlm = 1;
                    string Title = "【】";

                    for (int i = 1; i < this.flowLayoutPanel.Controls.Count; i = i + 2)
                    {
                        /// ステータスバーの更新
                        notifyHander(i * 100 / this.flowLayoutPanel.Controls.Count, null);

                        /// 各グリッドの情報を出力する
                        Title = "【" + SetFlm.ToString() + "】";
                        try
                        {
                            SetRow = exRep.MapingDataReport((DataGridView)this.flowLayoutPanel.Controls[i], Title, SetRow) + 1;
                            SetFlm++;
                        }
                        catch { }
                    }

                    /// 出力内容を表示する
                    exRep.excel.App.Visible = true;

                }
            }
            catch
            {
                /// エクセルが無いときCSV出力に切り替える
                notifyHander(0, "　Ｃ　Ｓ　Ｖ　出　力　中　・　・　・");
                string[] sptDragName        = DragName.Split('\\');
                long SetFlm                 = 1;
                string[] sptFileName        = sptDragName[sptDragName.Length - 1].Split('.');
                string Title                = "【】";
                string OutFileName          = Transfer.DeskTopPath + "出力結果【" + sptFileName[0] + "】.csv";
                try
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(OutFileName, FileMode.CreateNew), Encoding.Default))
                    {
                        /// グリッドよりマッピングデータを抽出する
                        for (int i = 1; i < this.flowLayoutPanel.Controls.Count; i = i + 2)
                        {
                            /// 各グリッドの情報を出力する
                            Title           = "【" + SetFlm.ToString() + "】";
                            sw.WriteLine(Title);
                            try
                            {
                                CsvDataReport((DataGridView)this.flowLayoutPanel.Controls[i], Title, sw);
                                SetFlm++;
                                sw.WriteLine("");
                            }
                            catch { }
                            notifyHander(i * 100 / this.flowLayoutPanel.Controls.Count, null);
                        }
                    }
                    System.Diagnostics.Process.Start(OutFileName);
                }               
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ファイル出力エラー", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            /// マウスカーソルを元に戻します。
            notifyHander(0, "");
            Cursor.Current                  = Cursors.Default;
        }
        /// <summary>
        /// CSV形式でマッピングデータを編集する
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="Title"></param>
        /// <param name="sw"></param>
        private void CsvDataReport(DataGridView dataGridView, string Title, StreamWriter sw)
        {
            /// 詳細データ設定
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].Visible)
                {
                    string MapData          = "@";
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        MapData             += "," + dataGridView[j, i].Value;
                    }
                    sw.WriteLine(MapData.Replace("@,",""));
                }
            }

        }

        /// <summary>
        /// 画面サイズが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC200_Resize(object sender, EventArgs e)
        {

            /// スクロール表示を切らないと正確な計算が出来ない
            this.flowLayoutPanel.AutoScroll         = false;
            int prmWidth                            = 0;
            int prmHeight                           = 0;
            notifyHander                            = new NotifyStatusHandler(updateStatus);

            /// データグリッドビュー数分繰り返す
            for (int i = i = 1; i < this.flowLayoutPanel.Controls.Count; i = i + 2)
            {
                /// ステータスバーの更新
                notifyHander(i * 100 / this.flowLayoutPanel.Controls.Count, null);

                /// データグリッドビューサイズ調整
                try
                {
                    DataGridReSize((DataGridView)this.flowLayoutPanel.Controls[i], ref prmWidth, ref prmHeight);
                }
                catch { }
            }

            /// 画面を再描画してスクロールも自動表示する。
            this.flowLayoutPanel.ResumeLayout();
            this.flowLayoutPanel.AutoScroll         = true;

            /// マウスカーソルを元に戻します。
            notifyHander(0, "");
            Cursor.Current = Cursors.Default;
        }
        /// <summary>
        /// データグリッドビューサイズ調整
        /// </summary>
        /// <param name="dataGridView"></param>
        private void DataGridReSize(DataGridView dataGridView, ref int prmWidth, ref int prmHeight)
        {
            /// 最初のグリッドだけサイズを取得する
            if (prmWidth == 0)
            {
                /// 規定値取得
                int MaxWidth                            = this.flowLayoutPanel.Size.Width - 22;

                /// 一時的に自動サイズ調整を有効にして自動的に割り当てられたグリッド幅を取得する。
                this.flowLayoutPanel.ResumeLayout();
                this.flowLayoutPanel.AutoScroll         = true;
                dataGridView.ScrollBars                 = System.Windows.Forms.ScrollBars.None;
                dataGridView.AutoSize                   = true;
                prmWidth                                = dataGridView.Size.Width + 1;
                prmHeight                               = dataGridView.Size.Height;
                dataGridView.AutoSize                   = false;
                dataGridView.ScrollBars                 = System.Windows.Forms.ScrollBars.Both;
                this.flowLayoutPanel.AutoScroll         = false;

                /// 横幅が最大表示サイズを超えていたら最大表示サイズに変更する
                if (prmWidth > MaxWidth)
                {
                    prmWidth = MaxWidth;
                }
            }

            ///データグリッドビューサイズ設定
            dataGridView.Size                       = new System.Drawing.Size(prmWidth, prmHeight);
                       
            dataGridView.Refresh();
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
				this.tsslProgressBar.Visible		= false;
				this.tsslProgressBar.Value			= 0;
            }
			else
			{
				this.tsslProgressBar.Visible		= true;
				this.tsslProgressBar.Value			= progress;
            }
            //	メッセージの設定
            if (Message != null)
			{
				this.tsslMessage.Text				= Message;
                this.tsslMessage.Visible            = true;
            }
            this.tsslMessage.Size                   = new Size(StatusStrip.SizeChanged(this.Size.Width, this.tsslProgressBar.Size.Width, this.tsslProgressBar.Visible), 17);
            this.statusStrip.Refresh();
        }
        /// <summary>
        /// リセットを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbReset_Click(object sender, EventArgs e)
        {
            /// 表示ﾃﾞｰﾀをクリア
            this.flowLayoutPanel.Controls.Clear();
            this.lblTitle.Visible                   = false;
            this.cmbTitle.Visible                   = false;
            this.btnFileWrite.Visible               = false;
        }

        /// <summary>
        /// CSVファイル内容をグリッドにセットする
        /// </summary>
        /// <param name="DataList"></param>
        private void CSV_GridSet(string DataList, int SiftCT)
        {
            ///
            this.chkInvert.Checked                  = false;
            string TitleNM                          = "";
            string[] sptTitle                       = Properties.Resources.CSV_SD.Split(',');
            try
            {
                TitleNM                             = this.cmbTitle.SelectedItem.ToString();
            }
            catch { }
            string LogingNM                         = "";
            if ((DataList.Contains("ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ"))
                || (DataList.Contains("位置")))
            {
                LogingNM                            = dsCSV.List.Columns[1].ColumnName;
                TitleNM                             = dsCSV.List.Columns[dsCSV.List.Columns.Count - 1].ColumnName;
            }
            else if (TitleNM == "")
            {
                return;
            }

            /// シフト設定するとき
            if (SiftCT > 1)
            {
                this.chkInvert.Checked              = false;
                if (DragName.Contains("log000_SD"))
                {
                    try
                    {
                        /// ロット番号、タイプ名の分離
                        string[] sptDragName        = DragName.Split('\\');
                        string[] sptFileNM          = sptDragName[sptDragName.Length - 1].Split('_');
                        string LotNo                = sptFileNM[2];
                        string ProcNO               = "@";
                        /// TmMPGORDERの取得
                        CommonRead objCommonRead    = new CommonRead();
                        using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrQCILDB, true))
                        {
                            string WhereSql         = " WHERE (Model_NM = 'MD') AND (Del_FG = 0)";
                            DsName dsName           = new DsName();
                            if (objCommonRead.NameRead(sqlInfo, "TmMPGORDER", "Model_NM", "Proc_NO", WhereSql, false, ref dsName))
                            {
                                for (int i = 0; i < dsName.Name.Rows.Count; i++)
                                {
                                    ProcNO          += "," + dsName.Name[i].Data_NM;
                                }
                            }

                        }
                        /// 反転情報の取得
                        ProcNO                      = ProcNO.Replace("@,", "");
                        /// 京都はマスタ設定が無いので固定値を設定
                        if (ProcNO == "@")
                        {
                            ProcNO                  = "10,27,28,206,243,268,289,351,392,398,400,404,405,406";
                        }
                        using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                        {
                            /// ファイル作成日時の取得
                            FileInfo fileInfo       = new FileInfo(DragName);
                            string CreateTM         = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
                            /// MD工程のロット情報取得
                            string WhereSql         = " WHERE (lotno = N'" + LotNo + "') AND (CAST('" + CreateTM + "' AS DATETIME) BETWEEN startdt AND enddt)";
                            DsName dsName           = new DsName();
                            if (objCommonRead.NameRead(sqlInfo, "TnTran", "macno", "startdt", WhereSql, false, ref dsName))
                            {
                                if (dsName.Name.Rows.Count > 0)
                                {
                                    /// 反転対象設備情報読込
                                    string StartDT  = dsName.Name[0].Data_NM;
                                    WhereSql        = " INNER JOIN TmMachine ON TnTran.macno = TmMachine.macno"
                                                    + " WHERE (lotno = N'" + LotNo + "') AND (procno IN("+ ProcNO + ")) AND (startdt < '" + StartDT + "')";
                                    dsName          = new DsName();
                                    if (objCommonRead.NameRead(sqlInfo, "TnTran", "TnTran.macno", "TmMachine.revdeployfg", WhereSql, false, ref dsName))
                                    {
                                        for (int i = 0; i < dsName.Name.Rows.Count; i++)
                                        {
                                            /// 反転フラグONのとき
                                            if (dsName.Name[i].Data_NM == "1")
                                            {
                                                /// 反転チェックフラグを逆に変更する
                                                if (this.chkInvert.Checked)
                                                {
                                                    this.chkInvert.Checked  = false;
                                                }
                                                else
                                                {
                                                    this.chkInvert.Checked  = true;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    catch { }
                }
            }

            /// 縦横列データ等取得
            int ixX                                 = Convert.ToInt32(this.lblXPackage.Text.Replace("X：", ""));
            int ixY                                 = Convert.ToInt32(this.lblYPackage.Text.Replace("Y：", ""));
            int MaxCT                               = ixX * ixY;
            int FlmCT                               = dsCSV.List.Count / MaxCT * SiftCT;
            int SiftX                               = ixX / SiftCT;

            if (dsCSV.List.Count * SiftCT <= MaxCT)
            {
                int LogMax                          = (int)dsCSV.List[dsCSV.List.Count - 1][LogingNM];
                FlmCT                               = ((LogMax / MaxCT) + 1) * SiftCT;
            }

            if (((LogingNM == "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ")
                && (dsCSV.List.Count % MaxCT != 0))
                || (ixX % SiftCT != 0))
            {
                /// 選択タイプとチップ数が合わなかったとき
                DialogResult OkNgButton = MessageBox.Show("　フレーム情報とデータ数が一致していません。\n\n" 
                             + "　選択タイプが違いますので御確認願います。\n\n"
                             + "　そのまま処理を続行する場合にはＯＫを押して下さい。", "選択タイプ不一致警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                /// キャンセルが押されたとき、処理を終了する
                if (OkNgButton == System.Windows.Forms.DialogResult.Cancel) return;
            }

            /// 雛形データセットの作成
            DsFree dsMapping                        = new DsFree();
            dsMapping.List.Columns.Clear();
            /// 横列
            for (int i = 0; i < ixX; i++)
            {
                dsMapping.List.Columns.Add(i.ToString(), Type.GetType("System.String"));
                dsMapping.List.Columns[i.ToString()].Caption        = i.ToString();
                dsMapping.List.Columns[i.ToString()].DefaultValue   = "";
            }
            /// 縦列
            for (int i = 0; i < ixY; i++)
            {
                dsMapping.List.Rows.Add(new object[] { "" });
            }

            /// 初期化
            int SetCT                               = 0;
            int LogCT                               = 0;
            int ColCT                               = 1;
            int FLMCT                               = 0;
            int SetX                                = ixX - 1;
            int SetY                                = 0;
            bool ChangeFG                           = false;
            int AvgCT                               = 0;
            int AverageTotal                        = 0;
            notifyHander                            = new NotifyStatusHandler(updateStatus);

            if (SiftCT > 1)
            {
                SetX                                = SiftCT - 1;
            }

            /// 雛形から設定用データセットにコピー
            DsFree dsMappingData                    = (DsFree)dsMapping.Copy();

            
            /// 反転設定するとき
            if (this.chkInvert.Checked)
            {
                /// 先に全フレームを作成する
                for (int i = 0; i < FlmCT; i++)
                {
                    /// データグリッドをフローレイアウトに追加する
                    System.Windows.Forms.Label lblGridTitle         = new System.Windows.Forms.Label();
                    DataGridView dataGridView                       = new DataGridView();
                    try
                    {
                        FLMCT++;
                        ///グリッドラベル定義
                        lblGridTitle.AutoSize                       = true;
                        lblGridTitle.Font                           = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                        lblGridTitle.Name                           = "lbl_" + FLMCT.ToString();
                        lblGridTitle.Text                           = "【" + FLMCT.ToString() + "】";
                        lblGridTitle.Margin                         = new System.Windows.Forms.Padding(3, 6, 0, 0);
                        /// フローレイアウトに追加する
                        this.flowLayoutPanel.Controls.Add(lblGridTitle);
                        ///グリッド定義
                        dataGridView.AllowUserToAddRows             = false;
                        dataGridView.AllowUserToDeleteRows          = false;
                        dataGridView.ColumnHeadersHeightSizeMode    = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                        dataGridView.ColumnHeadersVisible           = false;
                        dataGridView.RowHeadersVisible              = false;
                        dataGridView.ImeMode                        = ImeMode.Off;
                        dataGridView.Name                           = "FLM_" + FLMCT.ToString();
                        dataGridView.ReadOnly                       = false;
                        dataGridView.RowTemplate.Height             = 16;
                        dataGridView.DataSource                     = dsMapping.List.Copy();
                        /// フローレイアウトに追加する
                        this.flowLayoutPanel.Controls.Add(dataGridView);
                        this.flowLayoutPanel.SetFlowBreak(dataGridView, true);
                    }
                    catch (Exception ex)
                    {
                        /// フローレイアウトに追加出来なくなったとき
                        MessageBox.Show(ex.Message, "表示エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dataGridView?.Dispose();
                        return;
                    }
                }
            }


            for (int i = 0; i < dsCSV.List.Count; i++)
            {
                /// カウント
                SetCT                                               = SetCT + SiftCT;
                LogCT++;

                if (((!LogingNM.Contains("ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（"))
                        && (!LogingNM.Contains("ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ")))
                    || (LogingNM.Contains("位置")))
                {
                    if (dsCSV.List[i][LogingNM].ToString() != LogCT.ToString())
                    {
                        /// ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽと一致しないとき
                        MessageBox.Show("　ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽと一致しません。\n\n　選択タイプが違いますので御確認願います。\n\n" +
                            "　CSVﾌｧｲﾙｱﾄﾞﾚｽ：" + dsCSV.List[i][LogingNM].ToString() + " 設定ｱﾄﾞﾚｽ：" + LogCT.ToString()
                            , "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ不一致エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                /// データを設定用データセットにセット
                try
                {
                    if ((LogingNM.Contains("ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ"))
                        || (LogingNM.Contains("位置")))
                    {
                        /// 同じロギングアドレスのレコードが有るとき
                        if (Convert.ToInt32(dsCSV.List[i][LogingNM].ToString()) < LogCT)
                        {
                            /// 次のレコードにする
                            i++;
                        }
                        else if (Convert.ToInt32(dsCSV.List[i][LogingNM].ToString()) == LogCT)
                        {
                            dsMappingData.List[SetY][SetX]  = dsCSV.List[i][TitleNM];
                        }
                        else
                        {
                            dsMappingData.List[SetY][SetX]  = "";
                            i--;
                        }
                        SetY++;
                    }
                    else
                    {
                        /// シフト設定するとき
                        if (SiftCT > 1)
                        {
                            /// 1レコードで全シリンジのデータを設定
                            for (int j = 1; j <= SiftCT; j++)
                            {
                                dsMappingData.List[SetY][SiftX * j - ColCT]     = dsCSV.List[i][j];
                            }
                            /// 設定位置カウント
                            if (ChangeFG)
                            {
                                ColCT--;
                            }
                            else
                            {
                                ColCT++;
                            }
                            /// １行終了時１行下げてＵターンする
                            if ((SiftX < ColCT)
                                || (ColCT == 0))
                            {
                                if (ChangeFG)
                                {
                                    ColCT                   = 1;
                                    ChangeFG                = false;
                                }
                                else
                                {
                                    ColCT                   = SiftX;
                                    ChangeFG                = true;
                                }
                                SetY++;
                            }
                        }
                        /// シフト設定しないとき
                        else
                        {
                            dsMappingData.List[SetY][SetX]  = dsCSV.List[i][TitleNM];
                            SetY++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string Message                          = ex.Message;
                }
                /// 縦１列が終了したとき
                if ((SiftCT == 1)  
                    && (SetCT % ixY == 0))
                {
                    SetX--;
                    SetY                                    = 0;
                }
            

                /// １フレーム終了時
                if ((SetCT == MaxCT) 
                    || (i == dsCSV.List.Count - 1))
                {

                    /// カウンター類をリセット
                    SetCT                                   = 0;
                    SetX                                    = ixX - 1;
                    SetY                                    = 0;
                    ChangeFG                                = false;
                    ColCT                                   = 1;

                    /// 反転設定するとき
                    if (this.chkInvert.Checked)
                    {
                        /// フレームグリッドを割付け
                        string strFLM_NM                            = "FLM_" + FLMCT.ToString();
                        DataGridView dataGridView                   = (DataGridView)this.flowLayoutPanel.Controls[strFLM_NM];
                        FLMCT--;
                        int Average                                 = 1;
                        try
                        {
                            Average                                 = AverageTotal / AvgCT;
                        }
                        catch { }

                        for (int j = 0; j < ixX; j++)
                        {
                            dataGridView.Columns[j].Resizable       = DataGridViewTriState.False;
                            if (LogingNM == "")
                            {
                                if (Average < 100)
                                {
                                    dataGridView.Columns[j].Width   = 18;
                                }
                                else
                                {
                                    dataGridView.Columns[j].Width   = 26;
                                }
                            }
                            else
                            {
                                dataGridView.Columns[j].Width       = 14;
                            }
                            for (int k = 0; k < ixY; k++)
                            {
                                dataGridView[j, k].Value            = dsMappingData.List[k][j].ToString();

                                switch (dataGridView[j, k].Value.ToString())
                                {
                                    case "": /// 未設定セルのとき
                                        if (LogingNM == "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（")
                                        {
                                            dataGridView[j, k].Style.BackColor          = Color.White;
                                        }
                                        else
                                        {
                                            dataGridView[j, k].Style.BackColor          = Color.Blue;
                                        }
                                        break;
                                    case "0":
                                        if (LogingNM == "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（")
                                        {
                                            dataGridView[j, k].Style.BackColor          = Color.Red;
                                            DifferenceFG                                = true;
                                        }
                                        else
                                        {
                                            dataGridView[j, k].Style.BackColor          = Color.White;
                                        }
                                        break;
                                    default:
                                        /// 樹脂量のとき
                                        if (AvgCT > 1)
                                        {
                                            try
                                            {
                                                /// 平均以下のとき
                                                if (Average > Convert.ToInt32(dataGridView[j, k].Value))
                                                {
                                                    dataGridView[j, k].Style.BackColor  = Color.Red;
                                                    DifferenceFG                        = true;
                                                }
                                                else
                                                {
                                                    dataGridView[j, k].Style.BackColor  = Color.White;
                                                }
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            dataGridView[j, k].Style.BackColor          = Color.Red;
                                            DifferenceFG                                = true;
                                        }
                                        break;
                                }
                            }
                        }
                        /// ステータスバーの更新
                        notifyHander((FlmCT - FLMCT) * 100 / FlmCT, null);
                        this.btnFileWrite.Visible                       = true;
                    }
                    else
                    {


                        /// データグリッドをフローレイアウトに追加する
                        System.Windows.Forms.Label lblGridTitle = new System.Windows.Forms.Label();
                        DataGridView dataGridView               = new DataGridView();
                        try
                        {
                            FLMCT++;
                            ///グリッドラベル定義
                            lblGridTitle.AutoSize                       = true;
                            lblGridTitle.Font                           = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
                            lblGridTitle.Name                           = "lbl_" + FLMCT.ToString();
                            lblGridTitle.Text                           = "【" + FLMCT.ToString() + "】";
                            lblGridTitle.Margin                         = new System.Windows.Forms.Padding(3, 6, 0, 0);
                            /// フローレイアウトに追加する
                            this.flowLayoutPanel.Controls.Add(lblGridTitle);
                            ///グリッド定義
                            dataGridView.AllowUserToAddRows             = false;
                            dataGridView.AllowUserToDeleteRows          = false;
                            dataGridView.ColumnHeadersHeightSizeMode    = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                            dataGridView.ColumnHeadersVisible           = false;
                            dataGridView.RowHeadersVisible              = false;
                            dataGridView.Name                           = "FLM_" + FLMCT.ToString();
                            dataGridView.ReadOnly                       = false;
                            dataGridView.RowTemplate.Height             = 16;
                            dataGridView.DataSource                     = dsMapping.List.Copy();
                            int Average                                 = 1;
                            try
                            {
                                Average                                 = AverageTotal / AvgCT;
                            }
                            catch { }
                            /// フローレイアウトに追加する
                            this.flowLayoutPanel.Controls.Add(dataGridView);
                            this.flowLayoutPanel.SetFlowBreak(dataGridView, true);
                            for (int j = 0; j < ixX; j++)
                            {
                                dataGridView.Columns[j].Resizable       = DataGridViewTriState.False;
                                if (LogingNM == "")
                                {
                                    if (Average < 100)
                                    {
                                        dataGridView.Columns[j].Width   = 18;
                                    }
                                    else
                                    {
                                        dataGridView.Columns[j].Width   = 26;
                                    }
                                }
                                else
                                {
                                    dataGridView.Columns[j].Width       = 14;
                                }
                                for (int k = 0; k < ixY; k++)
                                {
                                    dataGridView[j, k].Value            = dsMappingData.List[k][j].ToString();

                                    switch (dataGridView[j, k].Value.ToString())
                                    {
                                        case "": /// 未設定セルのとき
                                            if (LogingNM == "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（")
                                            {
                                                dataGridView[j, k].Style.BackColor          = Color.White;
                                            }
                                            else
                                            {
                                                dataGridView[j, k].Style.BackColor          = Color.Blue;
                                            }
                                            break;
                                        case "0":
                                            if (LogingNM == "ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（")
                                            {
                                                dataGridView[j, k].Style.BackColor          = Color.Red;
                                                DifferenceFG                                = true;
                                            }
                                            else
                                            {
                                                dataGridView[j, k].Style.BackColor          = Color.White;
                                            }
                                            break;
                                        default:
                                            /// 樹脂量のとき
                                            if (AvgCT > 1)
                                            {
                                                try
                                                {
                                                    /// 平均以下のとき
                                                    if (Average > Convert.ToInt32(dataGridView[j, k].Value))
                                                    {
                                                        dataGridView[j, k].Style.BackColor  = Color.Red;
                                                        DifferenceFG                        = true;
                                                    }
                                                    else
                                                    {
                                                        dataGridView[j, k].Style.BackColor  = Color.White;
                                                    }
                                                }
                                                catch { }
                                            }
                                            else
                                            {
                                                dataGridView[j, k].Style.BackColor          = Color.Red;
                                                DifferenceFG                                = true;
                                            }
                                            break;
                                    }
                                }
                            }
                            /// ステータスバーの更新
                            notifyHander(FLMCT * 100 / FlmCT, null);
                            //notifyHander.BeginInvoke(FLMCT * 100 / FlmCT, null, new AsyncCallback(CallMethod), null);
                            dsMappingData                                                   = (DsFree)dsMapping.Copy();
                        }
                        catch (Exception ex)
                        {
                            /// フローレイアウトに追加出来なくなったとき
                            MessageBox.Show(ex.Message, "表示エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dataGridView?.Dispose();
                            return;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// CSVファイル内容を既存グリッドに上書きする
        /// </summary>
        private void CSV_GridOverwrite()
        {
            string[] sptMM                          = Properties.Resources.CSV_MM.Split(',');
            string LogingNM                         = sptMM[1];
            string TitleNM                          = sptMM[2];

            /// 縦横列データ等取得
            int ixX                                 = Convert.ToInt32(this.lblXPackage.Text.Replace("X：", ""));
            int ixY                                 = Convert.ToInt32(this.lblYPackage.Text.Replace("Y：", ""));
            int MaxCT                               = ixX * ixY;
            int FlmCT                               = dsCSV.List.Count / MaxCT;

            /// 初期化
            int SetCT                               = 0;
            int LogCT                               = 0;
            int PanelCT                             = 1;
            int SetX                                = ixX - 1;
            int SetY                                = -1;
            //bool ChangeFG                           = false;

            /// 最初のフレームグリッドを割付け
            DataGridView dataGridView               = (DataGridView)this.flowLayoutPanel.Controls[PanelCT];
            
            for (int i = 0; i < dsCSV.List.Count; i++)
            {
                /// カウント
                SetCT++;
                LogCT++;
                SetY++;

                /// データを既存グリッドに上書きする
                try
                {
                    if (dsCSV.List[i][LogingNM].ToString() == LogCT.ToString())
                    {
                        if ((dataGridView[SetX, SetY].Value.ToString() == "0")
                            || (dataGridView[SetX, SetY].Value.ToString() == "S")
                            || (dataGridView[SetX, SetY].Value.ToString() == ""))
                        {
                            dataGridView[SetX, SetY].Value              = dsCSV.List[i][TitleNM].ToString().Replace("0", "W").Replace("1", "A");
                            dataGridView[SetX, SetY].Style.BackColor    = Color.GreenYellow;
                            DifferenceFG                                = true;
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
                catch (Exception ex)
                {
                    string Message                  = ex.Message;
                }
                /// 縦１列が終了したとき
                if (SetCT % ixY == 0)
                {
                    SetX                            = SetX - 1;
                    SetY                            = -1;
                }

                /// １フレーム終了時
                if (SetCT == MaxCT)
                {
                    /// カウンター類をリセット
                    SetCT                           = 0;
                    SetX                            = ixX - 1;
                    SetY                            = -1;
                    /// 次のフレームグリッドを割付け
                    PanelCT                         = PanelCT + 2;
                    try
                    {
                        dataGridView                = (DataGridView)this.flowLayoutPanel.Controls[PanelCT];
                    }
                    catch
                    {
                        PanelCT++;
                        dataGridView                = (DataGridView)this.flowLayoutPanel.Controls[PanelCT];
                    }
                }
            }
        }
        /// <summary>
        /// 表示項目が選択されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTitle_SelectionChangeCommitted(object sender, EventArgs e)
        {
            /// マウスカーソルの変更
            Cursor.Current                          = Cursors.WaitCursor;
            notifyHander                            = new NotifyStatusHandler(updateStatus);
            notifyHander(0, "　表　示　項　目　編　集　中　・　・　・");

            /// CSVファイル内容をグリッドにセットする
            this.flowLayoutPanel.Controls.Clear();
            this.flowLayoutPanel.AutoScroll         = false;
            this.flowLayoutPanel.SuspendLayout();
            this.cmbTitle.DroppedDown               = false;
            this.cmbTitle.Refresh();
            int SiftCT                              = 5;

            CSV_GridSet(Properties.Resources.CSV_SD, SiftCT);

            /// 画面サイズが変更されたときを実行し再描画する
            FVDC200_Resize(null, null);

            /// 差異が無いとき
            if (!DifferenceFG)
            {
                MessageBox.Show("　差異が有るデータが存在しません。\n\n" +
                    "　全て同じデータとなります。", "差異無し", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 常駐を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbResident_Click(object sender, EventArgs e)
        {
            this.Visible                            = false;
            this.notifyIcon.Visible                 = true;
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
                this.WindowState                    = FormWindowState.Normal;
            }
            this.Visible                            = true;
            this.notifyIcon.Visible                 = false;
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
                this.WindowState                    = FormWindowState.Normal;
            }
            this.Visible                            = true;
            this.notifyIcon.Visible                 = false;
        }

        
        /// <summary>
        /// タイプ検索ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            string TypeNM = this.cmbType.Text.Trim();
            if (TypeNM == "") return;
            if (TypeNM.Length < 3) return;
            Cursor.Current                  = Cursors.WaitCursor;
            /// 入力された内容で曖昧検索する
            CommonRead objCommonRead        = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                /// 対象タイプドロップダウン作成
                this.dsType                 = new DsName();
                this.dsType.Name.Rows.Add(new object[] { "", "" });
                string WhereSql;
                WhereSql                    = " INNER JOIN TnTran ON TnLot.lotno = TnTran.lotno "
                                            + "INNER JOIN TmWorkFlow ON TnLot.typecd = TmWorkFlow.typecd "
                                            + "AND TnTran.procno = TmWorkFlow.procno "
                                            + "WHERE (TmWorkFlow.delfg = 0) AND (TnTran.delfg = 0) "
                                            + "AND (TmWorkFlow.typecd LIKE '%" + TypeNM + "%')"
                                            + " GROUP BY TnLot.typecd ORDER BY TnLot.typecd";
                objCommonRead.TopNameRead(sqlInfo, " TOP (100) ", "TnLot", "TnLot.typecd", "TnLot.typecd", WhereSql, false, ref this.dsType);
                this.cmbType.Text           = "";
                this.cmbType.DataSource     = this.dsType;
                this.cmbType.DroppedDown    = true;
            }
            /// 画面をリセット
            tsbReset_Click(null, null);
            Cursor.Current                  = Cursors.Default;
        }
        /// <summary>
        /// 画面を閉じるとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC200_FormClosed(object sender, FormClosedEventArgs e)
        {
            /// オーナー画面を表示する
            try
            {
                this.Owner.Visible          = true;
            }
            catch
            {
                OwnerForm.Visible           = true;
            }
        }
        /// <summary>
        /// 非同期処理の終了
        /// </summary>
        /// <param name="ar"></param>
        internal void CallMethod(IAsyncResult ar)
        {
            NotifyStatusHandler notifyHander = (NotifyStatusHandler)ar.AsyncState;
            notifyHander.EndInvoke(ar);
        }

        /// <summary>
        /// ファイル更新ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileWrite_Click(object sender, EventArgs e)
        {
            if (DragName != "")
            {
                /// マウスカーソルの変更
                Cursor.Current      = Cursors.WaitCursor;
                notifyHander        = new NotifyStatusHandler(updateStatus);
                notifyHander(0, "　フ　ァ　イ　ル　出　力　編　集　中　・　・　・");

                StreamWriter fs     = null;
                string MapData      = "@";

                /// グリッドよりマッピングデータを抽出する
                for (int i = 1; i < this.flowLayoutPanel.Controls.Count; i = i + 2)
                {
                    try
                    {
                        MapDataCreate((DataGridView)this.flowLayoutPanel.Controls[i], ref MapData);
                    }
                    catch { }
                    notifyHander(i * 100 / this.flowLayoutPanel.Controls.Count, null);
                }

                try
                {
                    fs              = new StreamWriter(new FileStream(DragName, FileMode.Create));
                    fs.Write(MapData.Replace("@,", ""));
                }
                catch (Exception ex)
                {
                    string ErrMessage       = ex.Message;
                }
                finally
                {
                    fs.Close();
                    /// マウスカーソルを元に戻します。
                    notifyHander(0, "");
                    Cursor.Current          = Cursors.Default;

                    /// リセットを押したとき
                    tsbReset_Click(null, null);

                    MessageBox.Show("　ファイルへの上書き更新が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }
        /// <summary>
        /// グリッドよりマッピングデータを抽出する
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="MapData"></param>
        private void MapDataCreate(DataGridView dataGridView, ref string MapData)
        {
            /// 縦横列データ等取得
            int ixX                         = dataGridView.ColumnCount;
            int ixY                         = dataGridView.RowCount;
            int MaxCT                       = ixX * ixY;

            /// 初期化
            int SetX                        = ixX - 1;
            int SetY                        = -1;

            /// 詳細データ設定
            for (int SetCT = 1; SetCT <= MaxCT; SetCT++)
            {
                SetY++;

                /// データを既存グリッドに上書きする
                MapData                     += "," + dataGridView[SetX, SetY].Value.ToString();

                /// 縦１列が終了したとき
                if (SetCT % ixY == 0)
                {
                    SetX                    = SetX - 1;
                    SetY                    = -1;
                }

            }

        }
        /// <summary>
        /// 置換をクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbReplace_Click(object sender, EventArgs e)
        {
            FVDC210 FVDC210                 = new FVDC210();
            Transfer.SearchChar             = "";
            Transfer.ReplaceChar            = "";
            FVDC210.ShowDialog(this);

            if ((Transfer.SearchChar.Trim() == "") 
                && (Transfer.ReplaceChar.Trim() == "")) return;

            /// グリッドデータ置換
            
            /// マウスカーソルの変更
            Cursor.Current                  = Cursors.WaitCursor;
            notifyHander                    = new NotifyStatusHandler(updateStatus);
            notifyHander(0, "　グ　リ　ッ　ド　デ　ー　タ　置　換　中　・　・　・");


            /// グリッド定義
            DataGridView dataGridView;

            /// グリッド数分繰り返す
            for (int i = 1; i < this.flowLayoutPanel.Controls.Count; i = i + 2)
            {
                try
                {
                    dataGridView            = (DataGridView)this.flowLayoutPanel.Controls[i];

                    /// グリッド内の一致するデータを置き換える
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        for (int k = 0; k < dataGridView.Columns.Count; k++)
                        {
                            if (dataGridView[k, j].Value.ToString() == Transfer.SearchChar)
                            {
                                dataGridView[k, j].Value    = Transfer.ReplaceChar;
                            }
                        }
                    }
                    dataGridView.Refresh();
                }
                catch { }
                notifyHander(i * 100 / this.flowLayoutPanel.Controls.Count, null);
            }


            /// マウスカーソルを元に戻します。
            notifyHander(0, "");
            Cursor.Current                  = Cursors.Default;
        }
        /// <summary>
        /// 対象タイプに入力されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_TextChanged(object sender, EventArgs e)
        {
            /// テキストが変更されたとき
            string TypeNM                           = this.cmbType.Text.Trim();
            if (TypeNM == "") return;
            if (this.cmbType.SelectedIndex > -1) return;
            if (TypeNM.Length < 8)
            {
                this.lblXPackage.Text               = "";
                this.lblYPackage.Text               = "";
                return;
            }

            for (int i = 0; i < this.dsType.Name.Rows.Count; i++)
            {
                if (TypeNM == dsType.Name[i].Data_NM)
                {
                    try
                    {
                        this.cmbType.SelectedValue  = TypeNM;
                        this.cmbType.DroppedDown    = false;
                        /// リセットを押したとき
                        tsbReset_Click(null, null);
                        return;
                    }
                    catch { }
                }
            }

            /// タイプ情報を自動設定
            this.dsType.Name.Rows.Add(new object[] { TypeNM, TypeNM });
            this.cmbType.DataSource                 = this.dsType;
            try
            {
                this.cmbType.SelectedValue          = Transfer.TypeNM;
                this.cmbType.DroppedDown            = false;
            }
            catch { }
            /// リセットを押したとき
            tsbReset_Click(null, null);
        }
    }
}
