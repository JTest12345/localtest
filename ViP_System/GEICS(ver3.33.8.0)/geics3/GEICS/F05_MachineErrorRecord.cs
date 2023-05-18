using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Xml;

namespace GEICS
{
    public partial class F05_MachineErrorRecord : Form
    {
        Common Com = new Common();
        Painter p = new Painter();

        static public bool fDrawComplete;
        static public bool fClient;
        static string _sPlantNM;
        static string _sLineCategory;

        //static List<string> ListLine = new List<string>();        //ラインリスト(文字列)
        static List<string> ListEqui = new List<string>();        //設備リスト(文字列)

        static List<int> ListLineSQL=new List<int>();               //ラインリスト(コード)SQL用
        static List<string> ListAssetsSQL = new List<string>();     //設備名リスト(文字列)
        static List<string> ListEquiSQL = new List<string>();       //設備番号リスト(文字列)
        static List<string> ListTypeSQL = new List<string>();       //Typeリスト

        static bool fLineAll=false;
        static bool fAssetsAll = false;
        static bool fEquiAll = false;
        static bool fTypeAll = false;

		static TmLINEInfo tmLineInfo;

        //設備情報
        public struct EquiInfo
        {
            public string sEquipmentNO;     //設備番号('SLC-'は付けない)
            public string sAssetsNM;        //設備名
            public string sMachinSeqNO;     //号機番号
            public string sModelNM;         //装置型式
            public int nDispNO;             //表示順
            public string sIPAddressNO;     //IPアドレス
            public string sInputFolderNM;   //装置出力ファイルの取得先
        }

		public F05_MachineErrorRecord(TmLINEInfo selectedLineInfo, string sPlantNM, string sLineCategory)
        {
            _sPlantNM=sPlantNM;
            _sLineCategory = sLineCategory;

            InitializeComponent();
			MyInitializeComponent(selectedLineInfo);

            //<--Start 2010.03.09 応答なし回避
            fDrawComplete = true;
            //-->End 2010.03.09 応答なし回避

            rbAllData.Checked = true;
            rbUnConfirmedData.Checked = false;
            rbNoneData.Checked = false;
        }

        //初期設定
        private void MyInitializeComponent(TmLINEInfo selectedLineInfo)
        {
			tmLineInfo = selectedLineInfo;
			SetControll_Line(tmLineInfo, false);
            fLineAll = false;
        }

        //タイマーイベント
        private void timer_Tick(object sender, EventArgs e)
        {
            /*
            //中間サーバーであれば
            if (fClient == false)
            {
                SetEndDate();//終了のみ更新 1分おき
            }
            */
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
            //中間サーバーであれば
            if (fClient == false)
            {
                //開始のみ更新 60分おき
                SetStartDate();
            }
            */
        }


        //選択されたTreeNode取得
        private void trvGIL_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string sDBInfo="";
            string sEquiNM,sEquiNO,sClass,sParam,sTiming;
            string sQcParamNo, sManageNm;
            DateTime dtStart, dtEnd;

            string[] path = e.Node.FullPath.Split('\\');
            string[] DBInfo;
                        
            //最下部Node以外無反応
            if (e.Node.Nodes.Count != 0)
            {
                return;
            }

            btnRedraw.Enabled = true;//必要情報が表示された為、描画可能

            //最下部4層以上のNodeが選択された場合
            if (path.Length == 4)
            {
                sEquiNM = path[0];  //装置名
                sEquiNO = path[1].Substring(0, 6);  //設備番号
                sClass  = path[2];  //データ分類:装置設定ﾊﾟﾗﾒｰﾀ...etc
                sParam  = path[3];  //パラメータ名:プログラム名...etc
                sTiming = "";       //タイミング:1マガジン終
            }
            else if (path.Length == 5)
            {
                sEquiNM = path[0];  //装置名
                sEquiNO = path[1].Substring(0, 6);  //設備番号
                sClass  = path[2];  //データ分類:装置設定ﾊﾟﾗﾒｰﾀ...etc
                sParam  = path[3];  //パラメータ名:プログラム名...etc
                sTiming = path[4];  //タイミング:1マガジン終
            }
            //<--SAC-AI00235(傾向確認データ追加) 2010/04/15 Y.Matsushima Start
            else if (path.Length == 6)
            {
                sEquiNM = path[0];  //装置名
                sEquiNO = path[1].Substring(0, 6);  //設備番号
                sClass = path[2];  //データ分類:装置設定ﾊﾟﾗﾒｰﾀ...etc
                if (path[5] == "ALL")
                {
                    sParam = path[3];
                }else{
                    sParam = path[3] + path[5];  //パラメータ名:プログラム名...etc
                }
                sTiming = path[4];  //タイミング:1マガジン終
            }
            //-->SAC-AI00235(傾向確認データ追加) 2010/04/15 Y.Matsushima End
            else//最下部4層目以外のNodeが選択された場合
            {
                return;
            }

            //4層目は、タイミングを入れなくてもレコード特定可能
            if (sTiming == "")
            {
                sDBInfo = Com.GetTvPRM_Record(sClass, sParam);
            }
            else//5層目は、タイミングを入れないとレコード特定NG
            {
                sDBInfo = Com.GetTvPRM_Record(sClass, sTiming, sParam);
            }

            DBInfo = sDBInfo.Split(',');
            sQcParamNo = DBInfo[0];
            sTiming = DBInfo[2];
            sManageNm = DBInfo[4];

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            //<--Start 2010.03.09 応答なし回避
            // フォームを無効にしてボタンを押せないようにする
            btnRedraw.Enabled = false;  //[再抽出]ボタン
            //-->End 2010.03.09 応答なし回避

            //<--Start 2010.03.09 応答なし回避
            // フォームを有効に戻す
            btnRedraw.Enabled = true;  //[再抽出]ボタン
            //-->End 2010.03.09 応答なし回避
            this.Refresh();
        }

        //[グラフ再描画]ボタン押下時
        private void btnRedraw_Click(object sender, EventArgs e)
        {
            //<--Start 2010.03.09 応答なし回避 NG
            // フォームを無効にしてボタンを押せないようにする
            btnRedraw.Enabled = false;  //[再抽出]ボタン
            //-->End 2010.03.09 応答なし回避

            DateTime dtStart, dtEnd;
            if (btnRedraw_ErrorChk())
            {
                return;
            }
            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            ResetForm();

            //<--Start 2010.03.09 応答なし回避 NG
            // フォームを有効に戻す
            btnRedraw.Enabled = true;  //[再抽出]ボタン
            //-->End 2010.03.09 応答なし回避

            this.Refresh();

            MessageBox.Show(Constant.MessageInfo.Message_48);
        }

        private bool btnRedraw_ErrorChk()
        {
            DateTime dtStart, dtEnd;
            try
            {
                dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
                dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);
            }
            catch
            {
                MessageBox.Show("ｸﾞﾗﾌ描画期間の入力に誤りがあります。");
                return true;
            }
            return false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            InitForm();  //起動時一回のみ
            //SetEquiInfo(Convert.ToInt32(Form1.nLineCD));
        }

        //現在時間-12HへSet
        private void SetStartDate()
        {
            txtbStartYear.Text = Convert.ToString(DateTime.Now.AddHours(-24).Year);                    //開始年
            txtbStartMonth.Text = Convert.ToString(DateTime.Now.AddHours(-24).Month.ToString("00"));    //開始月
            txtbStartDay.Text = Convert.ToString(DateTime.Now.AddHours(-24).Day.ToString("00"));	    //開始日
            txtbStartHour.Text = Convert.ToString(DateTime.Now.AddHours(-24).Hour.ToString("00"));     //開始時
            txtbStartMinute.Text = Convert.ToString(DateTime.Now.AddHours(-24).Minute.ToString("00"));   //開始分
        }
        //現在時間へSet
        private void SetEndDate()
        {
            txtbEndYear.Text = Convert.ToString(DateTime.Now.Year);	                            //終了年
            txtbEndMonth.Text = Convert.ToString(DateTime.Now.Month.ToString("00"));	            //終了月
            txtbEndDay.Text = Convert.ToString(DateTime.Now.Day.ToString("00"));	            //終了日
            txtbEndHour.Text = Convert.ToString(DateTime.Now.Hour.ToString("00"));	            //終了時
            txtbEndMinute.Text = Convert.ToString(DateTime.Now.Minute.ToString("00"));	            //終了分
        }

        private void SetDate()
        {
            //ﾃﾞﾌｫﾙﾄ描画期間=現在時刻12時間前～現在
            txtbStartYear.Text = Convert.ToString(DateTime.Now.AddHours(-24).Year);                    //開始年
            txtbStartMonth.Text = Convert.ToString(DateTime.Now.AddHours(-24).Month.ToString("00"));    //開始月
            txtbStartDay.Text = Convert.ToString(DateTime.Now.AddHours(-24).Day.ToString("00"));	    //開始日
            txtbStartHour.Text = Convert.ToString(DateTime.Now.AddHours(-24).Hour.ToString("00"));     //開始時
            txtbStartMinute.Text = Convert.ToString(DateTime.Now.AddHours(-24).Minute.ToString("00"));   //開始分

            txtbEndYear.Text = Convert.ToString(DateTime.Now.Year);	                            //終了年
            txtbEndMonth.Text = Convert.ToString(DateTime.Now.Month.ToString("00"));	            //終了月
            txtbEndDay.Text = Convert.ToString(DateTime.Now.Day.ToString("00"));	            //終了日
            txtbEndHour.Text = Convert.ToString(DateTime.Now.Hour.ToString("00"));	            //終了時
            txtbEndMinute.Text = Convert.ToString(DateTime.Now.Minute.ToString("00"));	            //終了分
        }

        private void InitForm()
        {
            SetDate();
            //btnRedraw.Enabled = false;//必要情報が揃っていない為、描画不可能
        }

        private bool CheckSelectControl() {

            bool fNG = false;

            //ライン未選択
            if (chklbLine.CheckedItems.Count == 0) {
                fNG = true;
            }

            //設備名未選択
            if (chklbAssetsNM.CheckedItems.Count == 0)
            {
                fNG = true;
            }

            //Type未選択
            if (chklbType.CheckedItems.Count == 0)
            {
                fNG = true;
            }

            return fNG;
        }

        public void ResetForm()
        {
            string sqlCmdTxt = "";
            DateTime dtStart, dtEnd;
            int nCnt = 0;

            //<--Start 2010.03.09 応答なし回避
            if (!fDrawComplete)
            {
                return;
            }
            fDrawComplete = false;
            //-->End 2010.03.09 応答なし回避

            //エラーリスト表示
            dsQCIL.Clear();
            dsQCIL.Dispose();

            //InputCheck
            if (CheckSelectControl() == true) {
                fDrawComplete = true;
                MessageBox.Show(Constant.MessageInfo.Message_49);
                return;
            }

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            if (rbNoneData.Checked != true)
            {
                string BaseSql = "SELECT TvQCIL.Inline_CD,TmLINE.Inline_NM, Equipment_NO,Assets_NM,MachinSeq_NO,Model_NM,Measure_DT,Seq_NO,QcParam_NO,Material_CD,"
                    + "Class_NM,Parameter_NM,Manage_NM,Timing_NM,Parameter_MAX,Parameter_MIN,DParameter_VAL,Parameter_VAL,SParameter_VAL,"
                    + "Magazine_NO,NascaLot_NO,Message_NM,DS_FG,Check_FG,TvQCIL.Del_FG,UpdUser_CD,TvQCIL.LastUpd_DT "
                    + "From dbo.TvQCIL AS TvQCIL WITH(NOLOCK) "
                    + " INNER JOIN TmLINE ON TvQCIL.Inline_CD = TmLINE.Inline_CD Where ";
                string WhereSql_Line = GetsLineSQL();   //ライン一覧
                string WhereSql_EquiNo = GetsEquiSQL(); //設備No一覧
                string WhereSql_Type = GetsTypeSQL();   //タイプ一覧
                string WhereSql_Other = "";             //その他
                string WhereSql_dt = "(Measure_DT>='" + dtStart + "' AND Measure_DT<'" + dtEnd+"')";                //期間

                if (rbUnConfirmedData.Checked == true)//全エラーリストの内、未確認情報のみ見たい場合
                {
                    WhereSql_Other = "Message_NM <>'' AND TvQCIL.Del_FG <> '1' AND Check_FG='0'";
                }
                else//全エラーリストを見たい場合
                {
                    WhereSql_Other = "Message_NM <>'' AND TvQCIL.Del_FG <> '1'";
                }

                sqlCmdTxt = BaseSql + WhereSql_Line + " AND " + WhereSql_EquiNo + " AND " + WhereSql_Type + " AND " + WhereSql_Other + " AND " + WhereSql_dt + " Option(Maxdop 1)";
                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        connect.Command.CommandTimeout = 0;

                        reader = connect.Command.ExecuteReader();
                        while (reader.Read())
                        {
                            nCnt = nCnt + 1;

                            /*
                            if (nCnt > 500)
                            {
                                MessageBox.Show("エラーリストが500件を越えました。処理を打ち切ります。期間を短くして下さい。");
                                break;
                            }
                            */

                            dsQCIL.TvQCIL.Rows.Add(
                            new object[] {  
                                    (reader["Inline_CD"]),
                                    (reader["Inline_NM"]),
                                    (reader["Equipment_NO"]),
                                    (reader["Assets_NM"]),
                                    (reader["MachinSeq_NO"]),
                                    (reader["Model_NM"]),
                                    (reader["Measure_DT"]),
                                    (reader["Seq_NO"]),
                                    (reader["QcParam_NO"]),
                                    (reader["Material_CD"]),
                                    (reader["Class_NM"]),
                                    (reader["Parameter_NM"]),
                                    (reader["Manage_NM"]),
                                    (reader["Timing_NM"]),
                                    (reader["Parameter_MAX"]),
                                    (reader["Parameter_MIN"]),
                                    (reader["DParameter_VAL"]),
                                    (reader["Parameter_VAL"]),
                                    (reader["SParameter_VAL"]),
                                    (reader["Magazine_NO"]),
                                    (reader["NascaLot_NO"]),
                                    (reader["Message_NM"].ToString().Replace("\r\n","。")),
                                    (reader["DS_FG"]),
                                    (reader["Check_FG"]),
                                    (reader["Del_FG"]),
                                    (reader["UpdUser_CD"]),
                                    (reader["LastUpd_DT"])
                              });

                            //<--Start 2010.03.09 応答なし回避
                            // メッセージ・キューにあるWindowsメッセージをすべて処理する
                            Application.DoEvents();
                            //-->End 2010.03.09 応答なし回避
                        }

                        //2012.4.10 HIshiguchi 監視項目の対応入力内容を取得する ※将来的にこの関数自体見直す
                        for (int i = 0; i < dsQCIL.TvQCIL.Count; i++)
                        {
                            int qcParamNO = Convert.ToInt32(dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.QcParam_NOColumn]);
                            int inspNO = ConnectQCIL.GetInspectionNO(qcParamNO);
                            if (inspNO == int.MinValue)
                            {
                                //非監視項目の場合、次へ
                                continue;
                            }

                            string lotNO = Convert.ToString(dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.NascaLot_NOColumn]);
                            DateTime measureDT = Convert.ToDateTime(dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.Measure_DTColumn]);

                            //対策入力内容を取得
                            QCNRInfo qcnrInfo = ConnectQCIL.GetQCNRInfo(measureDT, lotNO, inspNO);
                            if (qcnrInfo != null)
                            {
                                dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.TargetConfirm_NMColumn] = qcnrInfo.ConfirmNM;
                                dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.Check_FGColumn] = true;
                            }
                        }
                        //未対応データのみにチェックが入っている場合、入力されている行は削除する
                        if (rbUnConfirmedData.Checked)
                        {
                            for(int i = 0; i < dsQCIL.TvQCIL.Count; i++)
                            {
                                if (!Convert.ToBoolean(dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.Check_FGColumn])) 
                                //if (Convert.ToString(dsQCIL.TvQCIL.Rows[i][dsQCIL.TvQCIL.TargetConfirm_NMColumn]) == string.Empty) 
                                {
                                    continue;
                                }
                                dsQCIL.TvQCIL.Rows[i].Delete();
                                i = i - 1;
                            }
                            //gvQCError.EndEdit();
                            //tvQCILBindingSource.EndEdit();
                        }
                        //--------------------------------------------------------------------------------
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        MessageBox.Show(Constant.MessageInfo.Message_47 + ex.ToString());
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                        
                        //<--Start 2010.03.09 応答なし回避
                        fDrawComplete = true;
                        //-->End 2010.03.09 応答なし回避
                    }
                }
            }
        }

        private int GetInlineCD()
        {
            string sInlinecd;
            int inlineCD=0;
            string sfName="C:\\QCIL\\SettingFiles\\QCIL.xml";

            //中間サーバー=sfNameのファイルあり→開始/終了がタイマーで変化あり
            //クライアント=sfNameのファイルなし→開始/終了がタイマーで変化なし
            if (System.IO.File.Exists(sfName))
            {
                fClient = false;//中間サーバー
                this.Text = "GEICS(中間サーバー)";
            }
            else
            {
                fClient = true;//クライアント
                //trvGIL.Enabled = false;
                this.Text = "GEICS(クライアント)";
            }

            if (fClient == false)
            {
                System.IO.FileStream fs = new System.IO.FileStream(sfName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                XmlDocument doc = new XmlDocument();
                doc.Load((System.IO.Stream)fs);
                sInlinecd = doc.SelectSingleNode("//configuration/qcil_info/inline_cd").Attributes["value"].Value;
                inlineCD = Convert.ToInt32(sInlinecd);

                fs.Close();
            }
            return inlineCD;
        }

        private string GetMaterialCD(string sEqui)
        {
            string sMaterialcd="-";

            System.IO.FileStream fs = new System.IO.FileStream("C:\\QCIL\\SettingFiles\\QCIL.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            XmlDocument doc = new XmlDocument();
            doc.Load((System.IO.Stream)fs);
            try
            {
                sMaterialcd = doc.SelectSingleNode("//configuration/qcil_info/" + sEqui + "_Material_cd").Attributes["value"].Value;
            }
            catch
            {
                MessageBox.Show(Constant.MessageInfo.Message_50);
            }
            finally
            {
                fs.Close();
            }
            return sMaterialcd;
        }

        private void btnNinja_Click(object sender, EventArgs e)
        {
            //FormのコントロールをON/OFF
            //this.ControlBox = !this.ControlBox;
        }
        
        //[確認]ﾁｪｯｸﾎﾞｯｸｽがｸﾘｯｸされた
        private void gvQCError_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            //ボタンがクリックされた
            if (dgv.Columns[e.ColumnIndex].Name == "ConfirmNM")
            {
                //key
                int nInlineCD = Convert.ToInt32(gvQCError["Inline_CD", e.RowIndex].Value);
                string sEquipmentNO = Convert.ToString(gvQCError["Equipment_NO", e.RowIndex].Value).Trim();
                DateTime dtMesureDT = Convert.ToDateTime(gvQCError["Measure_DT", e.RowIndex].Value);
                int nSeqNO = Convert.ToInt32(gvQCError["Seq_NO", e.RowIndex].Value);
                int nQcParamNO = Convert.ToInt32(gvQCError["QcParam_NO", e.RowIndex].Value);
                int nCnfmNO = Com.GetTnLOGCnfm_CnfmNO(nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO);
                bool fCheckFG = Convert.ToBoolean(dgv["Check_FG", e.RowIndex].Value);
                string sLotNo = Convert.ToString(gvQCError["NascaLot_NO", e.RowIndex].Value).Trim();

                DateTime dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
                DateTime dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);
                string sManage = Convert.ToString(gvQCError["Manage_NM", e.RowIndex].Value).Trim();

                DateTime dtLastUpdDT = Convert.ToDateTime(gvQCError["LastUpd_DT", e.RowIndex].Value);

                //frmBackground frmbkgd = new frmBackground(nInlineCD, sEquipmentNO.Trim(), dtMesureDT, nSeqNO, nQcParamNO, nCnfmNO, fCheckFG, txtbAssetsNm.Text, dtLastUpdDT);
                //frmbkgd.ShowDialog();

                //<--Start 2010.03.09 応答なし回避
                btnRedraw.Enabled = false;  //[再抽出]ボタン
                //trvGIL.Enabled = false;     //[TreeVeiw]
                //-->End 2010.03.09 応答なし回避

                ResetForm();

                //<--Start 2010.03.09 応答なし回避
                btnRedraw.Enabled = true;  //[再抽出]ボタン
                //trvGIL.Enabled = true;     //[TreeVeiw]

                this.Refresh();
                //-->End 2010.03.09 応答なし回避
            }
        }

        private void gvQCError_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void gvQCError_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            //セルの列を確認
            if (Convert.ToBoolean(dgv[0, e.RowIndex].Value) == false)
            {
                e.CellStyle.BackColor = Color.Salmon;//未確認行は着色
            }
        }

        /// <summary>
        /// ラインチェックボックスの変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chklbLine_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 設備名リスト変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chklbAssetsNM_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ラインのチェックボックスリスト作成
        /// </summary>
        private void SetControll_Line(TmLINEInfo tmLineInfo, bool flg)
        {
            chklbLine.Items.Clear();

			//foreach (TmLINEInfo wTmLINEInfo in tmLineInfo)
			//{
                //選択されている[工場][カテゴリ]が同じであれば
			//if (wTmLINEInfo.Plant_NM == _sPlantNM && wTmLINEInfo.LineCate_NM == _sLineCategory)
			//{
			//    if (flg == true)
			//    {
			//        chklbLine.Items.Add(Convert.ToString(wTmLINEInfo.Inline_NM), CheckState.Checked);//チェックON
			//    }
			//    else {
			//        chklbLine.Items.Add(Convert.ToString(wTmLINEInfo.Inline_NM), CheckState.Unchecked);//チェックOFF
			//    }
			//}
			//}

			if (tmLineInfo.Plant_NM == _sPlantNM && tmLineInfo.LineCate_NM == _sLineCategory)
			{
				if (flg == true)
				{
					chklbLine.Items.Add(Convert.ToString(tmLineInfo.Inline_NM), CheckState.Checked);//チェックON
				}
				else
				{
					chklbLine.Items.Add(Convert.ToString(tmLineInfo.Inline_NM), CheckState.Unchecked);//チェックOFF
				}
			}
        }

        /// <summary>
        /// 設備名のチェックボックスリスト作成
        /// </summary>
        /// <param name="flg"></param>
        private void SetControll_Assets(bool flg)
        {
            ListLineSQL.Clear();
            chklbAssetsNM.Items.Clear();

            //チェックされてなければreturn
            if (chklbLine.CheckedItems.Count == 0)
            {
                chklbLine.Enabled = true;
                return;
            }

            //チェックされていれば
            for (int i = 0; i < chklbLine.CheckedItems.Count; i++)
            {
                //再抽出ボタン押下時に使用するSQLで活用
                ListLineSQL.Add(Com.GetLineNo(chklbLine.CheckedItems[i].ToString().Trim()));
            }

            //設備名リスト作成
            List<string> ListAssetsNM = GetAssetsNM();
            foreach (string sAssetsNM in ListAssetsNM)
            {
                if (flg == true)
                {
                    chklbAssetsNM.Items.Add(sAssetsNM, CheckState.Checked);
                }
                else
                {
                    chklbAssetsNM.Items.Add(sAssetsNM, CheckState.Unchecked);
                }
                
            }
        }

        //設備番号チェックリストボックスにコントロールセット
        private void SetControll_Equi(bool flg) 
        {
            ListAssetsSQL.Clear();
            chklbEqui.Items.Clear();

            if (chklbAssetsNM.CheckedItems.Count == 0)
            {
                chklbAssetsNM.Enabled = true;
                return;
            }
            //チェックされていれば
            for (int i = 0; i < chklbAssetsNM.CheckedItems.Count; i++)
            {
                //再抽出ボタン押下時に使用するSQLで活用
                ListAssetsSQL.Add(chklbAssetsNM.CheckedItems[i].ToString().Trim());
            }

            //ライン・設備名で取得した設備番号リスト(ﾀﾞｲﾎﾞﾝﾀﾞｰ11号機(S11111))
            List<string> ListEqui = GetEquipmentNO();

            foreach (string sEqui in ListEqui)
            {
                if (flg == true)
                {
                    chklbEqui.Items.Add(sEqui, CheckState.Checked);
                }
                else
                {
                    chklbEqui.Items.Add(sEqui, CheckState.Unchecked);
                }

            }
        }
        private void SetControll_Type(bool flg)
        {
            chklbType.Items.Clear();

            //Typeは設備のリストを利用して取得する
            if (chklbAssetsNM.CheckedItems.Count == 0)
            {
                chklbAssetsNM.Enabled = true;
                return;
            }

            //ライン・設備名で取得した設備番号リスト(ﾀﾞｲﾎﾞﾝﾀﾞｰ11号機(S11111))
            List<string> ListType = GetListType();

            foreach (string sType in ListType)
            {
                if (flg == true)
                {
                    chklbType.Items.Add(sType, CheckState.Checked);
                }
                else
                {
                    chklbType.Items.Add(sType, CheckState.Unchecked);
                }

            }
        }

        /// <summary>
        /// 設備名のリスト取得
        /// </summary>
        private List<string> GetAssetsNM()
        {
            List<string> wListAssetsNM = new List<string>();

            string sqlCmdTxt = @"SELECT Distinct Assets_NM,Process_CD FROM TvLSET With(NOLOCK) Where Inline_CD=@Inline_CD ORDER BY Process_CD option (MAXDOP 1)";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                //装置名の一覧は、ライン共通の為chklbLine.CheckedItems[0]でOK
                SqlParameter pInlinecd = connect.Command.Parameters.Add("@Inline_CD", SqlDbType.Int);
                pInlinecd.Value = Convert.ToInt32(Com.GetLineNo(chklbLine.CheckedItems[0].ToString()));

                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        wListAssetsNM.Add(Convert.ToString(reader["Assets_NM"]).Trim());
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wListAssetsNM;
        }

        /// <summary>
        /// [ライン][設備名]から設備番号リスト取得
        /// Inline_CDとAssets_NM は複数選択なので、SQL文字列を作成する必要あり
        /// </summary>
        /// <returns></returns>
        private List<string> GetEquipmentNO() 
        {
            List<string> wListEquipmentNO = new List<string>();
            string sLineSQL = GetsLineSQL();
            string sAssetsSQL = GetsAssetsSQL();

            string sqlCmdTxt = @"Select Assets_NM,MachinSeq_NO,Equipment_NO
                                FROM TvLSET With(NOLOCK)
                                INNER JOIN TmLINE ON TvLSET.Inline_CD = TmLINE.Inline_CD
                                Where (" + sLineSQL +") AND (" + sAssetsSQL +")"+/*('ﾀﾞｲﾎﾞﾝﾀﾞｰ') */
                                " ORDER BY Process_CD ASC option (MAXDOP 1)";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        wListEquipmentNO.Add(Convert.ToString(reader["Assets_NM"]).Trim() +
                                             Convert.ToString(reader["MachinSeq_NO"]).Trim() + "号機" +
                                             "(" + Convert.ToString(reader["Equipment_NO"]).Trim() + ")"
                                             );
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wListEquipmentNO;
        }

        private List<string> GetListType() 
        {
            List<string> wListType = new List<string>();

            string sqlCmdTxt = @"SELECT DISTINCT TmPLM.Material_CD
                                    FROM TmEQUI WITH (NOLOCK) INNER JOIN TmPLM ON TmEQUI.Model_NM = TmPLM.Model_NM
                                    WHERE (TmEQUI.Assets_NM = @Assets_NM ) 
                                    ORDER BY TmPLM.Material_CD";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                
                SqlParameter passetsnm = connect.Command.Parameters.Add("@Assets_NM", SqlDbType.NVarChar);
                passetsnm.Value = chklbAssetsNM.CheckedItems[0].ToString();//全設備共通なので、

                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        wListType.Add(Convert.ToString(reader["Material_CD"]).Trim());
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wListType;
        }
        private string GetsLineSQL() {
            string WhereSql = "@";

            List<string> ListCheckedLine = new List<string>();

            //チェックされていれば
            for (int i = 0; i < chklbLine.CheckedItems.Count; i++)
            {
                //再抽出ボタン押下時に使用するSQLで活用
                //ListCheckedLine.Add(Convert.ToString(Com.GetLineNo(chklbLine.CheckedItems[i].ToString())));
                WhereSql = WhereSql + " OR TmLINE.Inline_CD=" + Convert.ToString(Com.GetLineNo(chklbLine.CheckedItems[i].ToString()));  //設備番号
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";

            return WhereSql;
        }

        private string GetsAssetsSQL()
        {
            string WhereSql = "@";

            //チェックされていれば
            for (int i = 0; i < chklbAssetsNM.CheckedItems.Count; i++)
            {
                //再抽出ボタン押下時に使用するSQLで活用
                WhereSql = WhereSql + " OR Assets_NM='" + chklbAssetsNM.CheckedItems[i].ToString() + "'";  //設備番号
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";

            return WhereSql;
        }
        private string GetsEquiSQL()
        {
            string WhereSql = "@";
            string swEquiNO = "";
            string sEquiNO = "";
            //チェックされていれば
            for (int i = 0; i < chklbEqui.CheckedItems.Count; i++)
            {
                //文字列から設備番号のみ抽出
                swEquiNO = chklbEqui.CheckedItems[i].ToString();
                sEquiNO = swEquiNO.Substring(swEquiNO.IndexOf("(")+1,swEquiNO.IndexOf(")") - swEquiNO.IndexOf("(")-1);

                //再抽出ボタン押下時に使用するSQLで活用
                WhereSql = WhereSql + " OR Equipment_NO='" +  sEquiNO+ "'";  //設備番号
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";

            return WhereSql;
        }
        private string GetsTypeSQL()
        {
            string WhereSql = "@";

            //チェックされていれば
            for (int i = 0; i < chklbType.CheckedItems.Count; i++)
            {
                //再抽出ボタン押下時に使用するSQLで活用
                WhereSql = WhereSql + " OR Material_CD='" + chklbType.CheckedItems[i].ToString() + "'";  //設備番号
            }

            WhereSql = WhereSql.Replace("@ OR", "(");
            WhereSql = WhereSql + ")";

            return WhereSql;
        }
        //全選択-ライン
        private void btnLineAllSelect_Click(object sender, EventArgs e)
        {
            if (fLineAll == false)
            {
                SetControll_Line(tmLineInfo, true);
                fLineAll = true;
            }
            else
            {
				SetControll_Line(tmLineInfo, false);
                fLineAll = false;
            }
        }
        //全選択-設備名
        private void btnAssetsAllSelect_Click(object sender, EventArgs e)
        {
            if (fAssetsAll == false)
            {
                SetControll_Assets(true);
                fAssetsAll = true;
            }
            else
            {
                SetControll_Assets(false);
                fAssetsAll = false;
            }

        }
        //全選択-設備番号
        private void btnEquiAllSelect_Click(object sender, EventArgs e)
        {
            if(fEquiAll == false)
            {
                SetControll_Equi(true);
                fEquiAll = true;
            }
            else
            {
                SetControll_Equi(false);
                fEquiAll = false;
            }
            //static bool fEquiAll = false;
        }
        //全選択-Type
        private void btnTypeAllSelect_Click(object sender, EventArgs e)
        {
            //static bool fTypeAll = false;
            if (fTypeAll == false)
            {
                SetControll_Type(true);
                fTypeAll = true;
            }
            else
            {
                SetControll_Type(false);
                fTypeAll = false;
            }      
        }

        //ライン→設備名を設定
        private void btnSetAssets_Click(object sender, EventArgs e)
        {
            SetControll_Assets(true);

        }

        //ライン・設備名→設備番号を設定
        private void btnSetEqui_Click(object sender, EventArgs e)
        {
            SetControll_Equi(true);

        }
        //ライン・設備番号→Typeを設定
        private void btnSetType_Click(object sender, EventArgs e)
        {
            SetControll_Type(true);
        }
    }
}

