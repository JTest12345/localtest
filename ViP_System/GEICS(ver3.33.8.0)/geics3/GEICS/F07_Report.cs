using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.Specialized;

namespace GEICS
{
    public partial class F07_Report : Form
    {
        frmDrawGraphCatalog frmDrawGraphCatalog;
        Common Com = new Common();

        int _nLineCD = 0;
        public bool fTypeAll = false;
        public bool fMachineALL = false;

        public F07_Report()
        {
            InitializeComponent();

            MyInitializeComponent();
        }
        private void MyInitializeComponent()
        {
            //nLineCD
            _nLineCD = Common.nLineCD;
            SetCmbbLineNo();
            cmbbLineNo.Text = Convert.ToString(Common.nLineCD);

            //sType
			SetCmbbTypeNM(false);
            
            //開始・完了時間設定
            SetInitTimeDaily();

            //sAssetsNM
            SetCmbbAssetsNM();

			cmbbAssetsNM.SelectedIndex = 0;

			//cmbbAssetsNM.Text = "ﾀﾞｲﾎﾞﾝﾀﾞｰ";

            //List<string> ListEqui = GetSameAssets(nLineCD, sAssetsNM);

            /*
            clbMachineList.Items.Clear();
            for (int i = 0; i < ListEqui.Count; i++)
            {
                clbMachineList.Items.Add(ListEqui[i].Trim(), CheckState.Checked);//全てCheckON
            }
            */
        }

        //public void BatchReport_TUKIBETUDEBUG(string sbatch)
        //{
        //    bool flg = false;//false→日次,true→月次
        //    string sWhereSQL = "";
        //    DateTime dtStart = DateTime.MinValue;
        //    DateTime dtEnd = DateTime.MinValue;
        //    int nSheetNo = 1;

        //    SetInitTimeDaily();//レポート出力期間をコントロールに記入
        //    SetCmbbAssetsNM();

        //    SetInitTimeMonthly();
        //    flg = true;//月次

        //    //0=日次レポート,1=月次レポート
        //    for (int k = 1; k < 2; k++)
        //    {
        //        dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
        //        dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text + ":00");

        //        List<string> ListType = SetCmbbTypeNMForBatch(_nLineCD, dtStart, dtEnd);//ライン番号,集計日時セット

        //        for (int i = 0; i < clbType.CheckedItems.Count; i++)
        //        {
        //            if (Convert.ToString(clbType.CheckedItems[i]) == "") { continue; }
        //            for (int nTimmingNO = 1; nTimmingNO <= 4; nTimmingNO++)
        //            {
        //                switch (nTimmingNO)
        //                {
        //                    case 1: cmbbAssetsNM.Text = "ﾀﾞｲﾎﾞﾝﾀﾞｰ"; break;
        //                    case 2: cmbbAssetsNM.Text = "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ"; break;
        //                    case 3: cmbbAssetsNM.Text = "外観検査機"; break;
        //                    //case 1: case 2: case 3:
        //                    //    continue;
        //                    case 4: cmbbAssetsNM.Text = "ﾓｰﾙﾄﾞ機"; break;
        //                }

        //                sWhereSQL = GetWhereSQLEqui();
        //                frmDrawGraphCatalog = new frmDrawGraphCatalog(_nLineCD, nTimmingNO, sWhereSQL, clbType.CheckedItems[i].ToString().Trim(), dtStart, dtEnd, nSheetNo, flg);
        //                //if (frmDrawGraphCatalog._TargetLotList.Count > 0)
        //                //{
        //                    //ファイルに画像を保存して指定場所へ
        //                    frmDrawGraphCatalog.Show(this);// → frmDrawGraphCatalog_Loadに実装
        //                //}
        //            }
        //            nSheetNo = nSheetNo + 1;//Type毎にSheet追加
        //        }

        //        nSheetNo = 1;
        //    }
        //}

        public void BatchReport(string sbatch)
        {
            bool flg = false;//false→日次,true→月次
            string sWhereSQL = "";
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;
            int nSheetNo = 1;

            SetInitTimeDaily();//レポート出力期間をコントロールに記入
            SetCmbbAssetsNM();

            int outputKB = 0;
            if (sbatch.Contains("MONTH"))
            {
                //ﾊﾞｯﾁ引数にMONTHの場合、月別のみ出力
                outputKB = 1;
                SetInitTimeMonthly();
                flg = true;//月次
            }

            //0=日次レポート,1=月次レポート
            for (int k = outputKB; k < 2; k++)
            {
                dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
                dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text + ":00");

                List<string> ListType=SetCmbbTypeNMForBatch(_nLineCD, dtStart, dtEnd);//ライン番号,集計日時セット
                
                for (int i = 0; i < clbType.CheckedItems.Count; i++)
                {
                    if (Convert.ToString(clbType.CheckedItems[i]) == "") { continue; }
                    for (int nTimmingNO = 1; nTimmingNO <= 7; nTimmingNO++)
                    {
                        switch (nTimmingNO)
                        {
							case 1:
							case 2:
									cmbbAssetsNM.Text = "ﾀﾞｲﾎﾞﾝﾀﾞｰ"; break;
							case 5: cmbbAssetsNM.Text = "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ"; break;
							case 6: cmbbAssetsNM.Text = "外観検査機"; break;
							case 7: cmbbAssetsNM.Text = "ﾓｰﾙﾄﾞ機"; break;
                        }

                        sWhereSQL = GetWhereSQLEqui();
                        frmDrawGraphCatalog = new frmDrawGraphCatalog(_nLineCD, nTimmingNO.ToString(), sWhereSQL, clbType.CheckedItems[i].ToString().Trim(), dtStart, dtEnd, nSheetNo, flg, sbatch);

                        //ファイルに画像を保存して指定場所へ
                        frmDrawGraphCatalog.frmDrawGraphCatalog_Load(null, null);
                        //frmDrawGraphCatalog.Show(this);// → frmDrawGraphCatalog_Loadに実装
                    }
                    nSheetNo = nSheetNo + 1;//Type毎にSheet追加
                }

                //本日が月初め(=1日)でなければ、月次レポート出力を行わない。
                if (DateTime.Now.Day != 1)
                {
                    break;
                }

                SetInitTimeMonthly();
                flg = true;//月次
                nSheetNo = 1;

            }
        }

        private void SetCmbbLineNo()
        {
            //閾値マスタにあるInline_CDのみ表示
            this.cmbbLineNo.Items.Clear();
            //sqlCmdTxt = "SELECT DISTINCT [Inline_CD] FROM [TmLSET] WITH(NOLOCK) WHERE Del_FG <> '1' ORDER BY [Inline_CD]  ASC";
            string sqlCmdTxt = "SELECT DISTINCT [Inline_CD] FROM [TmLINE] WITH(NOLOCK) WHERE Del_FG <> '1' ORDER BY [Inline_CD]  ASC";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        this.cmbbLineNo.Items.Insert(i, Convert.ToString(reader["Inline_CD"]).Trim());
                        i = i + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        private void SetCmbbAssetsNM()
        {
            //閾値マスタにあるInline_CDのみ表示
            string sqlCmdTxt = "Select DISTINCT Assets_NM From TmEQUI WITH(NOLOCK) Where Assets_NM<>'ｶｯﾄﾌｫｰﾐﾝｸﾞ機' ORDER BY Assets_NM  DESC";
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        this.cmbbAssetsNM.Items.Insert(i, Convert.ToString(reader["Assets_NM"]).Trim());
                        i = i + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        List<string> GetSameAssets(int nLineCD, string sAssetsNM)
        {
            List<string> wList = new List<string>();
            string sEquipmentNO = "";

            string BaseSql = "SELECT Equipment_NO FROM TvLSET WITH(NOLOCK) Where Inline_CD={0} AND Assets_NM='{1}'";

            string sqlCmdTxt = string.Format(BaseSql, nLineCD, sAssetsNM);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        sEquipmentNO = Convert.ToString(reader["Equipment_NO"]).Trim();
                        sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);
                        wList.Add(sEquipmentNO);  //設備番号
                        //wList.Add(Convert.ToString(reader["Equipment_NO"]).Trim());  //設備番号
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wList;
        }

        private void cmbbAssetsNM_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCmbbMachine(true);
            /*
            List<string> ListEqui = GetSameAssets(Convert.ToInt32(cmbbLineNo.Text), cmbbAssetsNM.Text);
            string sEquipmentNO = "";
            clbMachineList.Items.Clear();
            for (int i = 0; i < ListEqui.Count; i++)
            {
                sEquipmentNO = ListEqui[i].Trim();
                sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);

                clbMachineList.Items.Add(ListEqui[i].Trim(), CheckState.Checked);//全てCheckON
            }
            */
        }
        /// <summary>
        /// flg=trueの場合、チェックを全て付ける
        /// flg=falseの場合、チェックを全て外す
        /// </summary>
        /// <param name="flg"></param>
        private void SetCmbbMachine(bool flg)
        {
            List<string> ListEqui = GetSameAssets(Convert.ToInt32(cmbbLineNo.Text), cmbbAssetsNM.Text);
            string sEquipmentNO = "";
            clbMachineList.Items.Clear();

            for (int i = 0; i < ListEqui.Count; i++)
            {
                sEquipmentNO = ListEqui[i].Trim();
                sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);

                if (flg == false)
                {
                    clbMachineList.Items.Add(ListEqui[i].Trim(), CheckState.Unchecked);
                }
                else
                {
                    clbMachineList.Items.Add(ListEqui[i].Trim(), CheckState.Checked);//全てCheckON
                }
                
            }
        }

        /// <summary>
        /// flg=trueの場合、チェックを全て付ける
        /// flg=falseの場合、チェックを全て外す
        /// </summary>
        /// <param name="flg"></param>
        private void SetCmbbTypeNM(bool flg)
        {
            this.clbType.Items.Clear();

            //閾値マスタにあるTypeのみ表示
            string sqlCmdTxt = "SELECT DISTINCT [Material_CD] FROM [TmPLM] WITH(NOLOCK) ORDER BY [Material_CD] ASC";
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        if (flg == false)
                        {
                            this.clbType.Items.Add(Convert.ToString(reader["Material_CD"]).Trim(), CheckState.Unchecked);
                        }
                        else
                        {
                            this.clbType.Items.Add(Convert.ToString(reader["Material_CD"]).Trim(), CheckState.Checked);
                        }
                        i = i + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        private List<string> SetCmbbTypeNMForBatch(int nLineNO, DateTime dtFrom, DateTime dtTo)
        {
            int nCnt = int.MinValue;
            //From～Toに何日あるか?確認
            if (dtTo.Year - dtFrom.Year > 0)
            {
                nCnt = (dtTo.DayOfYear + 365) - dtFrom.DayOfYear;
            }
            else
            {
                nCnt = dtTo.DayOfYear - dtFrom.DayOfYear;
            }
            List<string> wListType = new List<string>();
            this.clbType.Items.Clear();

            //一日刻みでSQL実行
            dtTo = dtFrom.AddDays(1);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlParameter pInlinecd = connect.Command.Parameters.Add("@INLINECD", SqlDbType.Int);
                SqlParameter pdtfrom = connect.Command.Parameters.Add("@DTFROM", SqlDbType.DateTime);
                SqlParameter pdtto = connect.Command.Parameters.Add("@DTTO", SqlDbType.DateTime);

                //日数分ループ
                for (int i = 0; i < nCnt; i++)
                {
                    //閾値マスタにあるTypeのみ表示
                    string sqlCmdTxt = @"SELECT Material_CD
                                FROM TnLOG With(NOLOCK) 
                                WHERE Inline_CD = @INLINECD AND
                                (Measure_DT >= @DTFROM) AND (Measure_DT < @DTTO) ";
                    //Group BY Material_CD";
                    //option (MAXDOP 1)";

                    SqlDataReader reader = null;

                    pInlinecd.Value = nLineNO;
                    pdtfrom.Value = dtFrom;
                    pdtto.Value = dtTo;

                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        reader = connect.Command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (wListType.Contains(Convert.ToString(reader["Material_CD"]).Trim()) == false)
                            {
                                wListType.Add(Convert.ToString(reader["Material_CD"]).Trim());
                                this.clbType.Items.Add(Convert.ToString(reader["Material_CD"]).Trim(), CheckState.Checked);
                            }
                        }
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                    }

                    dtFrom = dtFrom.AddDays(1);
                    dtTo = dtTo.AddDays(1);
                }
            }

            //wListType.Add("NESW455B31");
            //this.clbType.Items.Add("NESW455B31", CheckState.Checked);

            //wListType.Add("NSSW206B");
            //this.clbType.Items.Add("NSSW206B", CheckState.Checked);

            //wListType.Add("NSSW206A1");
            //this.clbType.Items.Add("NSSW206A1", CheckState.Checked);

            //wListType.Add("NESW455B01");
            //this.clbType.Items.Add("NESW455B01", CheckState.Checked);

            //wListType.Add("NSSW206B001");
            //this.clbType.Items.Add("NSSW206B001", CheckState.Checked);

            //wListType.Add("NESW15501");
            //this.clbType.Items.Add("NESW15501", CheckState.Checked);

            //wListType.Add("NESW455B3");
            //this.clbType.Items.Add("NESW455B3", CheckState.Checked);

            return wListType;
        }

        //抽出ボタン押下
        private void btnOutput_Click(object sender, EventArgs e)
        {
            bool flg=true;//false→日次,true→月次
            //int nTimmingNO = 0;
            string sWhereSQL = "";
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);
            TimeSpan ts = dtEnd.Subtract(dtStart);

            if (ts.Days>31)
            {
                MessageBox.Show(Constant.MessageInfo.Message_64);
                return;
            }

            string timingNO = Common.GetTimingNO(cmbbAssetsNM.Text.Trim());

            sWhereSQL = GetWhereSQLEqui();

            //日次にチェックあり
            if(rbDayly.Checked==true){
                flg=false;
            }
            //Environment.SpecialFolder.MyDocuments//Mydocumentアドレス

            for (int i = 0; i < clbType.CheckedItems.Count; i++)
            {
                //if (clbType.CheckedItems[i].ToString().Trim() == "NSSW206C")
                //{
                //    continue;
                //}

                frmDrawGraphCatalog = new frmDrawGraphCatalog(_nLineCD, timingNO, sWhereSQL, clbType.CheckedItems[i].ToString().Trim(), dtStart, dtEnd, 1, flg);
                if (frmDrawGraphCatalog._TargetLotList.Count > 0)
                {
                    frmDrawGraphCatalog.Show(this);
                }
            }
            MessageBox.Show(Constant.MessageInfo.Message_48);
        }

        private string GetWhereSQLEqui()
        {
            string sWhereSQL = "@";
            string sEquiNO = "";
            int npos = 0;

            if (clbMachineList.CheckedItems.Count == 0)
            {
                sWhereSQL = "";
                return sWhereSQL;
            }
            for (int i = 0; i < clbMachineList.CheckedItems.Count; i++)
            {
                sEquiNO = clbMachineList.CheckedItems[i].ToString().Trim();
                npos = sEquiNO.IndexOf("(");
                sEquiNO = sEquiNO.Substring(npos + 1, sEquiNO.Length - npos - 2);//「S*****」表記
                sWhereSQL = sWhereSQL + "Equipment_NO='" + sEquiNO + "' OR ";
                //sWhereSQL = sWhereSQL + "Equipment_NO='" + clbMachineList.CheckedItems[i].ToString().Trim() + "' OR ";
            }
            sWhereSQL = sWhereSQL.Replace("@", " AND (");
            sWhereSQL = sWhereSQL + ")";
            sWhereSQL = sWhereSQL.Replace("OR )", ")");

            return sWhereSQL;
        }

        private string GetWhereSQLEqui(int nTimming)
        {
            string sWhereSQL = "@";
            string sEquiNO = "";
            int npos = 0;

            if (clbMachineList.CheckedItems.Count == 0)
            {
                sWhereSQL = "";
                return sWhereSQL;
            }
            for (int i = 0; i < clbMachineList.CheckedItems.Count; i++)
            {
                sEquiNO = clbMachineList.CheckedItems[i].ToString().Trim();
                npos = sEquiNO.IndexOf("(");
                sEquiNO = sEquiNO.Substring(npos + 1, sEquiNO.Length - npos - 2);//「S*****」表記
                sWhereSQL = sWhereSQL + "Equipment_NO='" + sEquiNO + "' OR ";
                //sWhereSQL = sWhereSQL + "Equipment_NO='" + clbMachineList.CheckedItems[i].ToString().Trim() + "' OR ";
            }
            sWhereSQL = sWhereSQL.Replace("@", " AND (");
            sWhereSQL = sWhereSQL + ")";
            sWhereSQL = sWhereSQL.Replace("OR )", ")");

            return sWhereSQL;
        }

        //日次チェックボックス
        private void rbDayly_CheckedChanged(object sender, EventArgs e)
        {
            SetInitTimeDaily();
        }

        //月次チェックボックス
        private void rbMonthly_CheckedChanged(object sender, EventArgs e)
        {
            SetInitTimeMonthly();
        }

        private void SetInitTimeMonthly()
        {
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(DateTime.Today.AddMonths(-1).Year.ToString() + "/" + DateTime.Today.AddMonths(-1).Month.ToString() + "/1" + " 0:00");
            dtEnd = Convert.ToDateTime(dtStart.AddMonths(1).Year.ToString() + "/" + dtStart.AddMonths(1).Month.ToString() + "/1" + " 0:00");
            //dtEnd = dtEnd.AddDays(-1);

            txtbStartYear.Text = dtStart.Year.ToString();
            txtbStartMonth.Text = dtStart.Month.ToString();
            txtbStartDay.Text = dtStart.Day.ToString();
            txtbStartHour.Text = dtStart.Hour.ToString();
            txtbStartMinute.Text = dtStart.Minute.ToString();

            txtbEndYear.Text = dtEnd.Year.ToString();
            txtbEndMonth.Text = dtEnd.Month.ToString();
            txtbEndDay.Text = dtEnd.Day.ToString();
            txtbEndHour.Text = dtEnd.Hour.ToString();
            txtbEndMinute.Text = dtEnd.Minute.ToString();
        }

        private void SetInitTimeDaily()
        {
            //TEST用------------------------------------------
            //txtbStartYear.Text = DateTime.Today.AddDays(-2).Year.ToString();
            //txtbStartMonth.Text = DateTime.Today.AddDays(-2).Month.ToString();
            //txtbStartDay.Text = DateTime.Today.AddDays(-2).Day.ToString();

            //txtbEndYear.Text = DateTime.Today.AddDays(-1).Year.ToString();
            //txtbEndMonth.Text = DateTime.Today.AddDays(-1).Month.ToString();
            //txtbEndDay.Text = DateTime.Today.AddDays(-1).Day.ToString();
            //------------------------------------------------

            txtbStartYear.Text = DateTime.Today.AddDays(-1).Year.ToString();
            txtbStartMonth.Text = DateTime.Today.AddDays(-1).Month.ToString();
            txtbStartDay.Text = DateTime.Today.AddDays(-1).Day.ToString();
            txtbStartHour.Text = "7";
            txtbStartMinute.Text = "30";

            txtbEndYear.Text = DateTime.Today.Year.ToString();
            txtbEndMonth.Text = DateTime.Today.Month.ToString();
            txtbEndDay.Text = DateTime.Today.Day.ToString();
            txtbEndHour.Text = "7";
            txtbEndMinute.Text = "30";
        }
        //日インクリメント
        private void btnDayInc_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            txtbStartYear.Text = dtStart.AddDays(1).Year.ToString();
            txtbStartMonth.Text = dtStart.AddDays(1).Month.ToString();
            txtbStartDay.Text = dtStart.AddDays(1).Day.ToString();
            txtbStartHour.Text = dtStart.AddDays(1).Hour.ToString();
            txtbStartMinute.Text = dtStart.AddDays(1).Minute.ToString();

            txtbEndYear.Text = dtEnd.AddDays(1).Year.ToString();
            txtbEndMonth.Text = dtEnd.AddDays(1).Month.ToString();
            txtbEndDay.Text = dtEnd.AddDays(1).Day.ToString();
            txtbEndHour.Text = dtEnd.AddDays(1).Hour.ToString();
            txtbEndMinute.Text = dtEnd.AddDays(1).Minute.ToString();

        }
        //日デクリメント
        private void btnDayDec_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            txtbStartYear.Text = dtStart.AddDays(-1).Year.ToString();
            txtbStartMonth.Text = dtStart.AddDays(-1).Month.ToString();
            txtbStartDay.Text = dtStart.AddDays(-1).Day.ToString();
            txtbStartHour.Text = dtStart.AddDays(-1).Hour.ToString();
            txtbStartMinute.Text = dtStart.AddDays(-1).Minute.ToString();

            txtbEndYear.Text = dtEnd.AddDays(-1).Year.ToString();
            txtbEndMonth.Text = dtEnd.AddDays(-1).Month.ToString();
            txtbEndDay.Text = dtEnd.AddDays(-1).Day.ToString();
            txtbEndHour.Text = dtEnd.AddDays(-1).Hour.ToString();
            txtbEndMinute.Text = dtEnd.AddDays(-1).Minute.ToString();
        }
        //月インクリメント
        private void btnMonthInc_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            txtbStartYear.Text = dtStart.AddMonths(1).Year.ToString();
            txtbStartMonth.Text = dtStart.AddMonths(1).Month.ToString();
            txtbStartDay.Text = dtStart.AddMonths(1).Day.ToString();
            txtbStartHour.Text = dtStart.AddMonths(1).Hour.ToString();
            txtbStartMinute.Text = dtStart.AddMonths(1).Minute.ToString();

            txtbEndYear.Text = dtEnd.AddMonths(1).Year.ToString();
            txtbEndMonth.Text = dtEnd.AddMonths(1).Month.ToString();
            txtbEndDay.Text = dtEnd.AddMonths(1).Day.ToString();
            txtbEndHour.Text = dtEnd.AddMonths(1).Hour.ToString();
            txtbEndMinute.Text = dtEnd.AddMonths(1).Minute.ToString();
        }
        //月デクリメント
        private void btnMonthDec_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;

            dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
            dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

            txtbStartYear.Text = dtStart.AddMonths(-1).Year.ToString();
            txtbStartMonth.Text = dtStart.AddMonths(-1).Month.ToString();
            txtbStartDay.Text = dtStart.AddMonths(-1).Day.ToString();
            txtbStartHour.Text = dtStart.AddMonths(-1).Hour.ToString();
            txtbStartMinute.Text = dtStart.AddMonths(-1).Minute.ToString();

            txtbEndYear.Text = dtEnd.AddMonths(-1).Year.ToString();
            txtbEndMonth.Text = dtEnd.AddMonths(-1).Month.ToString();
            txtbEndDay.Text = dtEnd.AddMonths(-1).Day.ToString();
            txtbEndHour.Text = dtEnd.AddMonths(-1).Hour.ToString();
            txtbEndMinute.Text = dtEnd.AddMonths(-1).Minute.ToString();
        }

        //Type全てチェック
        private void chkbTypeAll_CheckedChanged(object sender, EventArgs e)
        {
            if (fTypeAll == false)
            {
                SetCmbbTypeNM(true);
                fTypeAll = true;
            }
            else
            {
                SetCmbbTypeNM(false);
                fTypeAll = false;
            }
        }

        private void btnTypeAll_Click(object sender, EventArgs e)
        {
            if (fTypeAll == false)
            {
                SetCmbbTypeNM(true);
                fTypeAll = true;
            }
            else
            {
                SetCmbbTypeNM(false);
                fTypeAll = false;
            }
        }

        private void btnMachineAll_Click(object sender, EventArgs e)
        {
            if (fMachineALL == false)
            {
                SetCmbbMachine(true);
                fMachineALL = true;
            }
            else
            {
                SetCmbbMachine(false);
                fMachineALL = false;
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void F07_Report_Load(object sender, EventArgs e)
        {

        }


   }
}
