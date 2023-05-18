using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace GEICS
{
    public partial class frmMasterSetting : Form
    {
        Common Com = new Common();
        static public bool fDrawComplete;
        bool fnewtype = false;//新規Typeの場合true
        //static public string StrQCIL;
        static bool _fWrite;//true(書き込み権限)・false(読み取り権限)
        public string _sLineNM;

        public frmMasterSetting(bool fWrite)
        {
            InitializeComponent();
            _fWrite = fWrite;
        }

        public frmMasterSetting(bool fWrite, string lineNM)
        {
            InitializeComponent();
            _fWrite = fWrite;
            _sLineNM = lineNM;
        }
        private void MyInitializeComponent()
        {
            if (Constant.fClient)
            {
                toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
                toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;
            }
            else 
            {
                statusStrip.Visible = false;
            }

            txtbOutputDate.Text = Convert.ToString(DateTime.Now);
            
            //if (cmbbLineNo.Text == "高効率ライン")
            //{
            //    //高効率ライン用
            //    StrQCIL = "Server=NSERVER01.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;User ID=sla02;password=2Gquf5d;Application Name=QCIL_40692";
            //}
            //else
            //{
            //    //自動化ライン用
            //    StrQCIL = "Server=HQDB5.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;UID=sla02;PWD=2Gquf5d;Application Name=QCIL_40692";
            //}
            cmbbLineNo.Text = _sLineNM;
            SetType();

            //読み込み権限(TmADMINに登録なし)の場合
            if (_fWrite == false)
            {
                this.Text = this.Text + Constant.MessageInfo.Message_77;
                btnOutputForm.Enabled = true;
                btnOutputExcel.Enabled = true;
                btnReadExcel.Enabled = false;
                btnEntry.Enabled = false;
            }
            else
            {
                this.Text = this.Text + Constant.MessageInfo.Message_76;
                btnOutputForm.Enabled = true;

                btnOutputExcel.Enabled = true;
                btnReadExcel.Enabled = true;
                btnEntry.Enabled = true;
            }
        }

        private void frmMasterSetting_Load(object sender, EventArgs e)
        {
            MyInitializeComponent();
        }

        //[画面出力]ボタン
        private void btnOutputForm_Click(object sender, EventArgs e)
        {
            try
            {
                SetListData(cmbbType.Text.Trim());

                //2012.3.8 HIshiguchi ダイス数量表示-------------
                int diceCount = ConnectQCIL.GetDiceCount(cmbbType.Text.Trim());
                if (diceCount == int.MinValue)
                {
                    txtDiceCount.Text = Constant.MessageInfo.Message_80;
                    txtDiceCount.ForeColor = Color.Red;
                }
                else
                {
                    txtDiceCount.Text = Convert.ToString(diceCount);
                }
                //-----------------------------------------------

            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetType()
        {
            //閾値マスタにあるTypeのみ表示
            this.cmbbType.Items.Clear();
            string sqlCmdTxt = "SELECT DISTINCT [Material_CD] FROM [TmPLM] WITH(NOLOCK) WHERE Del_FG <> '1' ORDER BY [Material_CD]  ASC";
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
                        this.cmbbType.Items.Insert(i, Convert.ToString(reader["Material_CD"]).Trim());
                        i = i + 1;
                    }
                    //this.cmbbType.Items.Insert(i, "新規タイプ");
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        //[Excel出力]ボタン
        private void btnOutputExcel_Click(object sender, EventArgs e)
        {
            if (cmbbType.Text == "") 
            {
                MessageBox.Show(Constant.MessageInfo.Message_44, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                SetListData(cmbbType.Text.Trim());

                //2012.3.8 HIshiguchi ダイス数量表示-------------
                int diceCount = ConnectQCIL.GetDiceCount(cmbbType.Text.Trim());
                if (diceCount == int.MinValue)
                {
                    txtDiceCount.Text = Constant.MessageInfo.Message_80;
                    txtDiceCount.ForeColor = Color.Red;
                }
                else
                {
                    txtDiceCount.Text = Convert.ToString(diceCount);
                }
                //-----------------------------------------------

				ExcelControl excelControl = ExcelControl.GetInstance();
                
                List<string> ListModelNM = new List<string>();

                ListModelNM = GetModelList();

                if (!excelControl.DataOutput(Convert.ToDateTime(txtbOutputDate.Text), cmbbType.Text.Trim(), txtDiceCount.Text.Trim(), cmbbLineNo.Text.Trim(), dsTmPLMEx, ListModelNM))
                {
                    //元のカーソルに戻す
                    Cursor.Current = Cursors.Default;

                    MessageBox.Show(Constant.MessageInfo.Message_19, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //excelControl.nonCloseFlg = true;
                excelControl.xlApp.Visible = true;
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //[Excel読み込み]ボタン
        private void btnReadExcel_Click(object sender, EventArgs e)
        {
            //Excelから読み込んで登録
            string fileName = getFilePath();

            if (fileName != "")
            {
                using (ExcelControl excelControl = ExcelControl.GetInstance())
                {
                    object[,] xlData = excelControl.DataRead(fileName);
                    SetExceldata(xlData);
                }
            }

            //ライン種別(=データベース)を変更不可
            cmbbLineNo.Enabled = false;
        }

        private List<int> GetChangeRowsList(object[,] xlData)
        {
            string sRev="",sNo = "", sReason = "", sUpdCD = "";

            int rowMaxIndex = xlData.GetLength(0);//行数。xlData.GetLength(1)は列数
            List<int> wList = new List<int>();//変更された行番号(Rev,管理番号,変更理由,変更者全て入力ある場合、変更行とする)
            //変更行探索
            for (int i = 14; i <= rowMaxIndex; i++)
            {
                try
                {
                    sRev = xlData[i, (int)Constant.ENUM_EXCEL_COL.Rev].ToString().Trim();       //RevisionNo
                }
                catch//セル空白の場合null
                {
                    sRev = "";
                }

                try
                {
                    sNo = xlData[i, (int)Constant.ENUM_EXCEL_COL.Reason].ToString().Trim();     //管理番号(QcParam_NO)
                }
                catch//セル空白の場合null
                {
                    sNo = "";
                }
                try
                {
                    sReason = xlData[i, (int)Constant.ENUM_EXCEL_COL.Reason].ToString().Trim(); //変更理由
                }
                catch
                {
                    sReason = ""; //変更理由

                }
                try
                {
                    sUpdCD = xlData[i, (int)Constant.ENUM_EXCEL_COL.Updcd].ToString().Trim();   //変更者
                }
                catch
                {
                    sUpdCD = "";   //変更者

                }

                //No/変更理由/変更者(社員番号)に全て文字列が入力されている場合、その行を変更行とみなす
                //新規の場合は全行が変更行
                if ((sNo != "" && sReason != "" && sUpdCD != "")) //sRev != "" &&    || (fnewtype==true)) //HIshiguchi 2012/01/12 新規登録でも入れない管理項目がある為、除外する。
                {
                    wList.Add(i);
                }
            }

            return wList;
        }

        private void SetDataSet(object[,] xlData)
        {
            string sRev = "", sNo = "", sModel="";
            string sMax = "", sMin = "", sVal = "", sPnt = "", sQcMax = "", sQcMin = "";
            string sReason = "", sUpdCD = "";

            int rowMaxIndex = xlData.GetLength(0);      //行数。xlData.GetLength(1)は列数
            List<int> ListChangeRows = new List<int>(); //変更された行番号
            ListChangeRows = GetChangeRowsList(xlData); //変更行取得

            try
            {
                //変更行のみループ
                foreach (int i in ListChangeRows)
                {
                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Rev] == null)
                    {
                        sRev = "1";
                    }
                    else
                    {
                        sRev = xlData[i, (int)Constant.ENUM_EXCEL_COL.Rev].ToString().Trim();               //RevisionNo
                    }
                    sNo = xlData[i, (int)Constant.ENUM_EXCEL_COL.No].ToString().Trim();                 //QcParam_NO
                    sModel = xlData[i, (int)Constant.ENUM_EXCEL_COL.Model].ToString().Trim();           //Model_NM

                   
                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamMax_After] == null)
                    {
                        sMax = "";
                    }
                    else
                    {
                        sMax = xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamMax_After].ToString().Trim();    //管理値上限(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamMin_After] == null)
                    {
                        sMin = "";
                    }
                    else
                    {
                        sMin = xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamMin_After].ToString().Trim();    //管理値下限(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamVal_After] == null)
                    {
                        sVal = "";
                    }
                    else
                    {
                        sVal = xlData[i, (int)Constant.ENUM_EXCEL_COL.ParamVal_After].ToString().Trim();    //文字列(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinepnt_After] == null)
                    {
                        sPnt = "";
                    }
                    else
                    {
                        sPnt = xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinepnt_After].ToString().Trim();   //打点数(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinemax_After] == null)
                    {
                        sQcMax = "";
                    }
                    else
                    {
                        sQcMax = xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinemax_After].ToString().Trim(); //管理値上限(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinemin_After] == null)
                    {
                        sQcMin = "";
                    }
                    else
                    {
                        sQcMin = xlData[i, (int)Constant.ENUM_EXCEL_COL.Qclinemin_After].ToString().Trim(); //管理値下限(変更値)
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Reason] == null)
                    {
                        sReason = "";
                    }
                    else
                    {
                        sReason = xlData[i, (int)Constant.ENUM_EXCEL_COL.Reason].ToString().Trim();         //変更理由
                    }

                    if (xlData[i, (int)Constant.ENUM_EXCEL_COL.Updcd] == null)
                    {
                        sUpdCD = "";
                    }
                    else
                    {
                        sUpdCD = xlData[i, (int)Constant.ENUM_EXCEL_COL.Updcd].ToString().Trim();           //変更者
                    }

                    for (int j = 0; j < dsTmPLMEx.TvPLMEx.Count; j++)
                    {
                        if ( (dsTmPLMEx.TvPLMEx[j].QcParam_NO == Convert.ToInt32(sNo)) &&
                             (dsTmPLMEx.TvPLMEx[j].Model_NM == sModel))
                        {
                            if (fnewtype == true)
                            {
                                dsTmPLMEx.TvPLMEx[j].Rev = 1;//新規TypeのRevisionは1で固定
                            }
                            else
                            {
                                dsTmPLMEx.TvPLMEx[j].Rev = Convert.ToInt32(sRev) + 1;//Revisionを1つあげる
                            }

                            dsTmPLMEx.TvPLMEx[j].Material_CD=Convert.ToString(xlData[5, 4]);//Typeは上書き

                            if (sMax != "-" && sMax != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].Parameter_MAX = Convert.ToDouble(sMax);
                            }
                            if (sMin != "-" && sMin != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].Parameter_MIN = Convert.ToDouble(sMin);
                            }
                            if (sVal != "-" && sVal != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].Parameter_VAL = sVal;
                            }
                            if (sPnt != "-" && sPnt != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].QcLine_PNT = Convert.ToInt32(sPnt);
                            }
                            if (sQcMax != "-" && sQcMax != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].QcLine_MAX = Convert.ToDouble(sQcMax);
                            }
                            if (sQcMin != "-" && sQcMin != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].QcLine_MIN = Convert.ToDouble(sQcMin);
                            }
                            if (sReason != "-" && sReason != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].Reason_NM = sReason;
                            }
                            if (sUpdCD != "-" && sUpdCD != "")
                            {
                                dsTmPLMEx.TvPLMEx[j].UpdUser_CD = sUpdCD;
                            }
                            dsTmPLMEx.TvPLMEx[j].LastUpd_DT = DateTime.Now;
                            dsTmPLMEx.TvPLMEx[j].Edit_FG = true;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Constant.MessageInfo.Message_55);
            }
        }
        private bool CheckNewType(string sType)
        {
            bool flg = false;

            List<string> wList = new List<string>();

            DateTime dtTime = Convert.ToDateTime(txtbOutputDate.Text);


            string BaseSql = "SELECT TOP 1 Material_CD  FROM [TmPLM] WITH(NOLOCK) Where Material_CD='{0}'";

            string sqlCmdTxt = string.Format(BaseSql, sType);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        wList.Add(Convert.ToString(reader["Material_CD"]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //MessageBox.Show("QcParamNoの取得に失敗しました。\r\n\r\n [詳細]=" + ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            if (wList.Count == 0)
            {
                flg = true;
            }
            return flg;
        }
        /// <summary>
        /// データベース登録
        /// </summary>
        /// <param name="xlData"></param>
        private void SetExceldata(object[,] xlData)
        {
            dsTmPLMEx.Clear();

            if (CheckNewType(Convert.ToString(xlData[5, 4])) == true)
            {
                fnewtype = true;
            }
            else
            {
                fnewtype = false;
            }
            ////ライン選択を設定し、変更出来ないようにする
            //cmbbLineNo.Text = Convert.ToString(xlData[6, 4]);
            //cmbbLineNo.Enabled = false;

            cmbbType.Enabled = false;

            //Excelファイルから取得したTypeを元にデータベースへ問い合わせ
            //変更前のマスタ(データセット)作成
            if (fnewtype == false)
            {
                SetListData(Convert.ToString(xlData[5, 4]));
            }
            else
            {
                SetListData(Common.GetPRMFirstMaterial());
            }

            //変更後のマスタ(データセット)作成
            SetDataSet(xlData);

            //型番を出力(登録用)
            txtType.Text = Convert.ToString(xlData[5, 4]).Trim();

            //2012.3.8 HIshiguchi ダイス数量出力--------------------------

            //ダイス数量を出力
            int diceCount = int.MinValue;
            if (int.TryParse(Convert.ToString(xlData[4, 6]), out diceCount)
                && Convert.ToInt32(xlData[4, 6]) != 0)
            {
                txtDiceCount.Text = Convert.ToString(diceCount);
                btnEntry.Enabled = true;
            }
            else
            {
                //数値でない場合(小数点を含んだ数値、0、文字列)、登録ボタンを押せなくする
                MessageBox.Show(Constant.MessageInfo.Message_65, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnEntry.Enabled = false;
            }
            //------------------------------------------------------------
        }

        //[登録]ボタン
        private void btnEntry_Click(object sender, EventArgs e)
        {
            if(CheckDataSet()==false){
                MessageBox.Show(Constant.MessageInfo.Message_56);
                return;
            }

            try
            {
                SetDatabase();
                MessageBox.Show(Constant.MessageInfo.Message_8);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //変更行があるか?チェック
        private bool CheckDataSet()
        {
            bool flg = false;

            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true)
                {
                    flg = true;
                    break;
                }
            }
            return flg;
        }

        private void SetDatabase()
        {
            try
            {
                //新規Typeでない場合
                if (fnewtype == false)
                {
                    InsertTmPLMHist();  //TmPLMHistはInsert
                    UpdateTmPLM();      //TmPLMはUpdate
                    UpdateTmQCST();     //TmQCSTはUpdate

                    //2012.3.8 HIshiguchi ダイス数量更新(TmDIECT)--------------------------
                    ConnectQCIL.UpdateTmDIECT(txtType.Text, Convert.ToInt32(txtDiceCount.Text));
                }
                else//新規Typeの場合
                {
                    InsertTmPLMHist();  //TmPLMHistはInsert
                    InsertTmPLM();      //TmPLMはInsert
                    InsertTmQCST();     //TmQCSTはInsert&Update
                    UpdateTmQCST();     //TmQCSTはInsert&Update

                    //2012.3.8 HIshiguchi ダイス数量追加(TmDIECT)--------------------------
                    ConnectQCIL.InsertTmDIECT(txtType.Text, Convert.ToInt32(txtDiceCount.Text));
                }
            }
            catch (Exception err) 
            {
                throw err;
            }
        }

        private void InsertTmPLMHist()
        {
            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true)
                {
                    string BaseSql = "", sqlCmdTxt = "";

                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            SqlParameter pModel = connect.Command.Parameters.Add("@MODELNM", SqlDbType.Char);         
                            SqlParameter pQcparam = connect.Command.Parameters.Add("@QCPARAMNO", SqlDbType.Int);      
                            SqlParameter pMate = connect.Command.Parameters.Add("@MATERIALCD", SqlDbType.Char);       
                            SqlParameter pRev = connect.Command.Parameters.Add("@REVNO", SqlDbType.Int);              
                            SqlParameter pPrmMax = connect.Command.Parameters.Add("@PARAMETERMAX", SqlDbType.Decimal);
                            SqlParameter pPrmMin = connect.Command.Parameters.Add("@PARAMETERMIN", SqlDbType.Decimal);
                            SqlParameter pParamVal = connect.Command.Parameters.Add("@PARAMETERVAL", SqlDbType.Char); 
                            SqlParameter pQcpnt = connect.Command.Parameters.Add("@QCLINEPNT", SqlDbType.Int);        
                            SqlParameter pQcMax = connect.Command.Parameters.Add("@QCLINEMAX", SqlDbType.Decimal);    
                            SqlParameter pQcMin = connect.Command.Parameters.Add("@QCLINEMIN", SqlDbType.Decimal); 
                            SqlParameter pAimLVal = connect.Command.Parameters.Add("@AIMLINEVAL", SqlDbType.Decimal);    
                            SqlParameter pAimRVal = connect.Command.Parameters.Add("@AIMRATEVAL", SqlDbType.Decimal); 
                            SqlParameter pReason = connect.Command.Parameters.Add("@REASONNM", SqlDbType.Char);       
                            SqlParameter pDsfg = connect.Command.Parameters.Add("@DSFG", SqlDbType.Bit);              
                            SqlParameter pDelfg = connect.Command.Parameters.Add("@DELFG", SqlDbType.Bit);
                            SqlParameter pUpdcd = connect.Command.Parameters.Add("@UPDUSERCD", SqlDbType.Char);
                            SqlParameter pUpddt = connect.Command.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                            pModel.Value = dsTmPLMEx.TvPLMEx[i].Model_NM;
                            pQcparam.Value = dsTmPLMEx.TvPLMEx[i].QcParam_NO;
                            pMate.Value =dsTmPLMEx.TvPLMEx[i].Material_CD;
                            pRev.Value = dsTmPLMEx.TvPLMEx[i].Rev;
                            try
                            {
                                pPrmMax.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MAX;
                            }
                            catch
                            {
                                pPrmMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pPrmMin.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MIN;
                            }
                            catch
                            {
                                pPrmMin.Value = DBNull.Value;
                            }
                            try
                            {
                                pParamVal.Value = dsTmPLMEx.TvPLMEx[i].Parameter_VAL;
                            }
                            catch
                            {
                                pParamVal.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcpnt.Value = dsTmPLMEx.TvPLMEx[i].QcLine_PNT;
                            }
                            catch
                            {
                                pQcpnt.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMax.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MAX;
                            }
                            catch
                            {
                                pQcMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMin.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MIN;
                            }
                            catch
                            {
                                pQcMin.Value = DBNull.Value;
                            }
                            pAimLVal.Value = DBNull.Value;
                            pAimRVal.Value = DBNull.Value;
                            try
                            {
                                pReason.Value = dsTmPLMEx.TvPLMEx[i].Reason_NM;
                            }
                            catch
                            {
                                pReason.Value = DBNull.Value;
                            }
                            pDsfg.Value = dsTmPLMEx.TvPLMEx[i].DS_FG;
                            pDelfg.Value = 0;
                            pUpdcd.Value = dsTmPLMEx.TvPLMEx[i].UpdUser_CD;
                            pUpddt.Value = dsTmPLMEx.TvPLMEx[i].LastUpd_DT;

                            BaseSql = @"INSERT INTO [TmPLMHist]
                                        ([Model_NM]
                                        ,[QcParam_NO]
                                        ,[Material_CD]
                                        ,[Rev_NO]
                                        ,[Parameter_MAX]
                                        ,[Parameter_MIN]        
                                        ,[Parameter_VAL]        
                                        ,[QcLine_PNT]           
                                        ,[QcLine_MAX]           
                                        ,[QcLine_MIN]           
                                        ,[AimLine_VAL]          
                                        ,[AimRate_VAL]          
                                        ,[Reason_NM]            
                                        ,[DS_FG]                
                                        ,[Del_FG]               
                                        ,[UpdUser_CD]           
                                        ,[LastUpd_DT])          
                                 VALUES(@MODELNM, 
                                        @QCPARAMNO,
                                        @MATERIALCD,
                                        @REVNO,     
                                        @PARAMETERMAX,
                                        @PARAMETERMIN,
                                        @PARAMETERVAL,
                                        @QCLINEPNT, 
                                        @QCLINEMAX, 
                                        @QCLINEMIN, 
                                        @AIMLINEVAL,
                                        @AIMRATEVAL,
                                        @REASONNM,
                                        @DSFG,
                                        @DELFG,
                                        @UPDUSERCD,
                                        @LASTUPDDT)";
                            sqlCmdTxt = BaseSql;
                            connect.Command.CommandText = sqlCmdTxt;
                            connect.Command.ExecuteNonQuery();
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                            connect.Close();
                        }
                    }
                }
            }
        }

        private void UpdateTmPLM()
        {
            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true)
                {
                    string BaseSql = "", sqlCmdTxt = "";

                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            SqlParameter pPrmMax = connect.Command.Parameters.Add("@PARAMETERMAX", SqlDbType.Decimal);
                            SqlParameter pPrmMin = connect.Command.Parameters.Add("@PARAMETERMIN", SqlDbType.Decimal);
                            SqlParameter pPrmVal = connect.Command.Parameters.Add("@PARAMETERVAL", SqlDbType.Char);
                            SqlParameter pQcMax = connect.Command.Parameters.Add("@QCLINEMAX", SqlDbType.Decimal);
                            SqlParameter pQcMin = connect.Command.Parameters.Add("@QCLINEMIN", SqlDbType.Decimal);
                            SqlParameter pUpdUsrCd = connect.Command.Parameters.Add("@UPDUSERCD", SqlDbType.Char);
                            SqlParameter pLastUpdDt = connect.Command.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                            SqlParameter pQcPrmNo = connect.Command.Parameters.Add("@QCPARAMNO", SqlDbType.Int);
                            SqlParameter pMate = connect.Command.Parameters.Add("@MATERIALCD", SqlDbType.Char);
                            SqlParameter pModelNM = connect.Command.Parameters.Add("@MODELNM", SqlDbType.Char);

                            try
                            {
                                pPrmMax.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MAX;
                            }
                            catch
                            {
                                pPrmMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pPrmMin.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MIN;
                            }
                            catch
                            {
                                pPrmMin.Value = DBNull.Value;
                            }
                            try
                            {
                                pPrmVal.Value = dsTmPLMEx.TvPLMEx[i].Parameter_VAL;
                            }
                            catch
                            {
                                pPrmVal.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMax.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MAX;
                            }
                            catch
                            {
                                pQcMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMin.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MIN;
                            }
                            catch
                            {
                                pQcMin.Value = DBNull.Value;
                            }
                            pUpdUsrCd.Value = dsTmPLMEx.TvPLMEx[i].UpdUser_CD;
                            pLastUpdDt.Value = DateTime.Now;

                            pQcPrmNo.Value = dsTmPLMEx.TvPLMEx[i].QcParam_NO;
                            pMate.Value = dsTmPLMEx.TvPLMEx[i].Material_CD;
                            pModelNM.Value = dsTmPLMEx.TvPLMEx[i].Model_NM;

                            BaseSql = @"UPDATE [TmPLM] SET
                                [Parameter_MAX]  = @PARAMETERMAX,
                                [Parameter_MIN]  = @PARAMETERMIN,
                                [Parameter_VAL]  = @PARAMETERVAL,
                                [QcLine_MAX]     = @QCLINEMAX,
                                [QcLine_MIN]     = @QCLINEMIN,
                                [UpdUser_CD]     = @UPDUSERCD,
                                [LastUpd_DT]     = @LASTUPDDT
                                Where QcParam_NO=@QCPARAMNO AND Material_CD=@MATERIALCD AND Model_NM=@MODELNM";

                            sqlCmdTxt = BaseSql;
                            connect.Command.CommandText = sqlCmdTxt;
                            connect.Command.ExecuteNonQuery();
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                            connect.Close();
                        }
                    }
                }
            }
        }
        /*
        //既存Typeのみ
        private void UpdateTmPLM()
        {
            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true)
                {
                    string BaseSql = "", sqlCmdTxt = "";

                    BaseSql = "UPDATE [TmPLM] SET "
                                + "[Parameter_MAX]  = {0} "
                                + "[Parameter_MIN]  = {1} "
                                + "[Parameter_VAL]  ='{2}'"
                                + "[QcLine_MAX]     = {3} "
                                + "[QcLine_MIN]     = {4} "
                                + "[UpdUser_CD]     ='{5}'"
                                + "[LastUpd_DT]     ='{6}'"
                                +"Where QcParam_NO={7} AND Material_CD='{8}'";
                    sqlCmdTxt = string.Format(BaseSql,
                                    dsTmPLMEx.TvPLMEx[i].Parameter_MAX, // {0}
                                    dsTmPLMEx.TvPLMEx[i].Parameter_MIN, // {1}
                                    dsTmPLMEx.TvPLMEx[i].Parameter_VAL, //'{2}'
                                    dsTmPLMEx.TvPLMEx[i].QcLine_MAX,    // {3}
                                    dsTmPLMEx.TvPLMEx[i].QcLine_MIN,    // {4}
                                    dsTmPLMEx.TvPLMEx[i].UpdUser_CD,    //'{5}'
                                    DateTime.Now,                       //'{6}'
                                    dsTmPLMEx.TvPLMEx[i].QcParam_NO,    // {7}
                                    dsTmPLMEx.TvPLMEx[i].Material_CD    //'{8}'
                                    );

                    using (IConnection connect = NascaConnection.CreateInstance(frmMasterSetting.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {

                            connect.Command.CommandText = sqlCmdTxt;
                            connect.Command.ExecuteNonQuery();
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                            connect.Close();
                        }
                    }
                }
            }
        }
        */


        //既存Typeのみ
        private void UpdateTmQCST()
        {
            int nInspectionNo = 0;
            int nQcPnt = 0;
            bool flg = false;   //*点連続がない場合は、USE_FGはFALSE
                                //*点連続がある場合は、USE_FGはTRUE

            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true )
                {
                    try
                    {//値が有る場合TmQCSTへUPDATEが必要、値が無い場合TmQCSTへUPDATEが不要(continueへ)
                        nQcPnt = dsTmPLMEx.TvPLMEx[i].QcLine_PNT;
                        flg = true;
                    }
                    catch
                    {
                        flg = false;//dsTmPLMEx.TvPLMEx[i].QcLine_PNTがnull(値無し)の場合
                    }

                    //*点連続がある場合は、USE_FGはTRUE
                    if (flg == true)
                    {
                        //InspectionNO取得
                        nInspectionNo = GetInspectionNo(dsTmPLMEx.TvPLMEx[i].QcParam_NO);
                        if (nInspectionNo == 0)//エラーチェック
                        {   //flg == trueでここに来たらNG
                            MessageBox.Show(string.Format(Constant.MessageInfo.Message_57, dsTmPLMEx.TvPLMEx[i].QcParam_NO));
                        }

                        string BaseSql = "", sqlCmdTxt = "";

                        BaseSql = "UPDATE [TmQCST] SET "
                                    + "[QCnum_VAL]  = {0}, "
                                    + "[USE_FG]     = 1, "
                                    + "[UpdUser_CD] = '{1}', "
                                    + "[LastUpd_DT] = '{2}' "
                                    + "Where Material_CD='{3}' AND Inspection_NO={4} AND QCnum_NO={5}";
                        sqlCmdTxt = string.Format(BaseSql,
                                        dsTmPLMEx.TvPLMEx[i].QcLine_PNT, // {0}
                                        dsTmPLMEx.TvPLMEx[i].UpdUser_CD,
                                        dsTmPLMEx.TvPLMEx[i].LastUpd_DT,
                                        dsTmPLMEx.TvPLMEx[i].Material_CD,
                                        nInspectionNo,
                                        Constant.TMQCST_UPDATE_QCnum_NO
                                        );

                        using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                        {
                            SqlDataReader reader = null;
                            try
                            {
                                connect.Command.CommandText = sqlCmdTxt;
                                connect.Command.ExecuteNonQuery();
                            }
                            finally
                            {
                                if (reader != null) reader.Close();
                                connect.Close();
                            }
                        }
                    }//*点連続がない場合は、USE_FGはFALSE
                    else
                    {
                        //InspectionNO取得
                        nInspectionNo = GetInspectionNo(dsTmPLMEx.TvPLMEx[i].QcParam_NO);
                        if (nInspectionNo == 0)//エラーチェック
                        {
                            //MessageBox.Show("TmQsubから[QcParam_NO]=" + dsTmPLMEx.TvPLMEx[i].QcParam_NO + "でInspection_NOを取得出来ませんでした。");
                            continue;//Inspection_NOとQcParam_NOの関連付けなしの場合は、continue
                        }

                        string BaseSql = "", sqlCmdTxt = "";

                        BaseSql = "UPDATE [TmQCST] SET "
                                    + "[USE_FG]     = 0 "
                                    + "Where Material_CD='{0}' AND Inspection_NO={1} AND QCnum_NO={2}";
                        sqlCmdTxt = string.Format(BaseSql,
                                        dsTmPLMEx.TvPLMEx[i].Material_CD,
                                        nInspectionNo,
                                        Constant.TMQCST_UPDATE_QCnum_NO
                                        );

                        using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                        {
                            SqlDataReader reader = null;
                            try
                            {
                                connect.Command.CommandText = sqlCmdTxt;
                                connect.Command.ExecuteNonQuery();
                            }
                            finally
                            {
                                if (reader != null) reader.Close();
                                connect.Close();
                            }
                        }
                    }
                }
            }
        }

        //[TmQsub]Process_NO(QcParam_NO)から[Inspection_NO]を取得する。
        private int GetInspectionNo(int nQcParamNO)
        {
            int n = 0;

            string BaseSql = "SELECT [Inspection_NO]  FROM [TmQsub] With(NOLOCK) "
                                + "Where Process_NO LIKE '%,{0},%' And Del_FG=0";

            string sqlCmdTxt = string.Format(BaseSql, nQcParamNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        n = Convert.ToInt32(reader["Inspection_NO"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Constant.MessageInfo.Message_58 + ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            return n;
        }

        //新規タイプのみ
        private void InsertTmPLM()
        {
            for (int i = 0; i < dsTmPLMEx.TvPLMEx.Count; i++)
            {
                if (dsTmPLMEx.TvPLMEx[i].Edit_FG == true)
                {
                    string BaseSql = "", sqlCmdTxt = "";

                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            SqlParameter pModel = connect.Command.Parameters.Add("@MODELNM", SqlDbType.Char);
                            SqlParameter pQcparam = connect.Command.Parameters.Add("@QCPARAMNO", SqlDbType.Int);
                            SqlParameter pMate = connect.Command.Parameters.Add("@MATERIALCD", SqlDbType.Char);
                            SqlParameter pPrmMax = connect.Command.Parameters.Add("@PARAMETERMAX", SqlDbType.Decimal);
                            SqlParameter pPrmMin = connect.Command.Parameters.Add("@PARAMETERMIN", SqlDbType.Decimal);
                            SqlParameter pParamVal = connect.Command.Parameters.Add("@PARAMETERVAL", SqlDbType.Char);
                            SqlParameter pQcpnt = connect.Command.Parameters.Add("@QCLINEPNT", SqlDbType.Int);
                            SqlParameter pQcMax = connect.Command.Parameters.Add("@QCLINEMAX", SqlDbType.Decimal);
                            SqlParameter pQcMin = connect.Command.Parameters.Add("@QCLINEMIN", SqlDbType.Decimal);
                            SqlParameter pAimLVal = connect.Command.Parameters.Add("@AIMLINEVAL", SqlDbType.Decimal);
                            SqlParameter pAimRVal = connect.Command.Parameters.Add("@AIMRATEVAL", SqlDbType.Decimal);
                            SqlParameter pDsfg = connect.Command.Parameters.Add("@DSFG", SqlDbType.Bit);
                            SqlParameter pDelfg = connect.Command.Parameters.Add("@DELFG", SqlDbType.Bit);
                            SqlParameter pUpdcd = connect.Command.Parameters.Add("@UPDUSERCD", SqlDbType.Char);
                            SqlParameter pUpddt = connect.Command.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                            pModel.Value = dsTmPLMEx.TvPLMEx[i].Model_NM;
                            pQcparam.Value = dsTmPLMEx.TvPLMEx[i].QcParam_NO;
                            pMate.Value = dsTmPLMEx.TvPLMEx[i].Material_CD;
                            try
                            {
                                pPrmMax.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MAX;
                            }
                            catch
                            {
                                pPrmMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pPrmMin.Value = dsTmPLMEx.TvPLMEx[i].Parameter_MIN;
                            }
                            catch
                            {
                                pPrmMin.Value = DBNull.Value;
                            }
                            try
                            {
                                pParamVal.Value = dsTmPLMEx.TvPLMEx[i].Parameter_VAL;
                            }
                            catch
                            {
                                pParamVal.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcpnt.Value = dsTmPLMEx.TvPLMEx[i].QcLine_PNT;
                            }
                            catch
                            {
                                pQcpnt.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMax.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MAX;
                            }
                            catch
                            {
                                pQcMax.Value = DBNull.Value;
                            }
                            try
                            {
                                pQcMin.Value = dsTmPLMEx.TvPLMEx[i].QcLine_MIN;
                            }
                            catch
                            {
                                pQcMin.Value = DBNull.Value;
                            }
                            pAimLVal.Value = DBNull.Value;
                            pAimRVal.Value = DBNull.Value;

                            pDsfg.Value = dsTmPLMEx.TvPLMEx[i].DS_FG;
                            pDelfg.Value = 0;
                            pUpdcd.Value = dsTmPLMEx.TvPLMEx[i].UpdUser_CD;
                            pUpddt.Value = dsTmPLMEx.TvPLMEx[i].LastUpd_DT;

                            BaseSql = @"INSERT INTO [TmPLM]
                                 ([Model_NM]
                                ,[QcParam_NO]
                                ,[Material_CD]
                                ,[Parameter_MAX]
                                ,[Parameter_MIN]
                                ,[Parameter_VAL]
                                ,[QcLine_MAX]
                                ,[QcLine_MIN]
                                ,[AimLine_VAL]
                                ,[AimRate_VAL]
                                ,[DS_FG]
                                ,[Del_FG]
                                ,[UpdUser_CD]
                                ,[LastUpd_DT]) 
                            VALUES(@MODELNM, 
                                   @QCPARAMNO,
                                   @MATERIALCD,
                                   @PARAMETERMAX,
                                   @PARAMETERMIN,
                                   @PARAMETERVAL,
                                   @QCLINEMAX, 
                                   @QCLINEMIN, 
                                   @AIMLINEVAL,
                                   @AIMRATEVAL,
                                   @DSFG,
                                   @DELFG,
                                   @UPDUSERCD,
                                   @LASTUPDDT)";

                            sqlCmdTxt = BaseSql;

                            connect.Command.CommandText = sqlCmdTxt;
                            connect.Command.ExecuteNonQuery();
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                            connect.Close();
                        }
                    }
                }
            }
        }
        //新規Typeのみ
        private void InsertTmQCST()
        {
            //NNSW208のコピーでTmQCSTをInsertする。
            SetdsTmQCST(Common.GetPRMFirstMaterial(), txtType.Text.Trim());

            for (int i = 0; i < dsTmQCST.dTblTmQCST.Count; i++)
            {
                string BaseSql = "", sqlCmdTxt = "";

                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        SqlParameter pMate = connect.Command.Parameters.Add("@MATERIALCD", SqlDbType.Char);
                        SqlParameter pInsp = connect.Command.Parameters.Add("@INSPECTIONNO", SqlDbType.Decimal);
                        SqlParameter pQcno = connect.Command.Parameters.Add("@QCNUMNO", SqlDbType.Decimal);
                        SqlParameter pQcval = connect.Command.Parameters.Add("@QCNUMVAL", SqlDbType.Char);
                        SqlParameter pUsefg = connect.Command.Parameters.Add("@USEFG", SqlDbType.Bit);
                        SqlParameter pDelfg = connect.Command.Parameters.Add("@DELFG", SqlDbType.Bit);
                        SqlParameter pUpdcd = connect.Command.Parameters.Add("@UPDUSERCD", SqlDbType.Char);
                        SqlParameter pUpddt = connect.Command.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                        pMate.Value = dsTmQCST.dTblTmQCST[i].Material_CD.Trim();
                        pInsp.Value = dsTmQCST.dTblTmQCST[i].Inspection_NO;
                        pQcno.Value = dsTmQCST.dTblTmQCST[i].QCnum_NO;
                        try
                        {
                            pQcval.Value = dsTmQCST.dTblTmQCST[i].QCnum_VAL.Trim();
                        }
                        catch
                        {
                            pQcval.Value = DBNull.Value;
                        }
                        pUsefg.Value = dsTmQCST.dTblTmQCST[i].USE_FG;
                        pDelfg.Value = dsTmQCST.dTblTmQCST[i].Del_FG;
                        //pUpdcd.Value = dsTmQCST.dTblTmQCST[i].UpdUser_CD.Trim();
                        //pUpddt.Value = dsTmQCST.dTblTmQCST[i].LastUpd_DT;
                        pUpdcd.Value = Constant.EmployeeInfo.EmployeeCD;
                        pUpddt.Value = DateTime.Now;

                        BaseSql = @"INSERT INTO [TmQCST]
                                    ([Material_CD]
                                    ,[Inspection_NO]
                                    ,[QCnum_NO]
                                    ,[QCnum_VAL]
                                    ,[USE_FG]
                                    ,[Del_FG]
                                    ,[UpdUser_CD]
                                    ,[LastUpd_DT]) 
                                    VALUES(
                                        @MATERIALCD,
                                        @INSPECTIONNO,
                                        @QCNUMNO,
                                        @QCNUMVAL,
                                        @USEFG,
                                        @DELFG,
                                        @UPDUSERCD,
                                        @LASTUPDDT)";
                        sqlCmdTxt = BaseSql;
                        connect.Command.CommandText = sqlCmdTxt;
                        connect.Command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                    }
                }
            }
        }

        private void SetdsTmQCST(string sFromType,string sToType)
        {
            dsTmQCST.Clear();

            string BaseSql = "SELECT [Material_CD],[Inspection_NO],[QCnum_NO],[QCnum_VAL],[USE_FG],[Del_FG],[UpdUser_CD],[LastUpd_DT] "
                            + "FROM TmQCST WITH(NOLOCK) Where Material_CD='{0}'";

            string sqlCmdTxt = string.Format(BaseSql, sFromType);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow wDataRow = dsTmQCST.dTblTmQCST.NewRow();

                        wDataRow["Material_CD"] = sToType;
                        wDataRow["Inspection_NO"] = reader["Inspection_NO"];
                        wDataRow["QCnum_NO"] = reader["QCnum_NO"];
                        wDataRow["QCnum_VAL"] = reader["QCnum_VAL"];
                        wDataRow["USE_FG"] = reader["USE_FG"];
                        wDataRow["Del_FG"] = reader["Del_FG"];
                        wDataRow["UpdUser_CD"] = reader["UpdUser_CD"];
                        wDataRow["LastUpd_DT"] = reader["LastUpd_DT"];

                        dsTmQCST.dTblTmQCST.Rows.Add(wDataRow);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Constant.MessageInfo.Message_59 + ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        /// <summary>
        /// ファイル選択
        /// </summary>
        /// <returns></returns>
        private string getFilePath()
        {
            string fileName = "";

            //レジストリからフォルダを取得
            string folderPath = (string)SLCommonLib.Commons.RegistryControl.GetRegistry("FolderPath");
            if (folderPath == "")
            {
                folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            openFileDialog1.InitialDirectory = folderPath;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;

                //レジストリにフォルダを記録
                SLCommonLib.Commons.RegistryControl.SetRegistry("FolderPath", Path.GetDirectoryName(fileName));
            }

            return fileName;
        }

        private List<string> GetModelList() 
        {
            List<string> wList = new List<string>();
            string BaseSql = "SELECT Distinct [Model_NM] FROM TmPLM WITH(NOLOCK) ORDER BY [Model_NM] ASC";

            string sqlCmdTxt = string.Format(BaseSql);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        wList.Add(Convert.ToString(reader["Model_NM"]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Constant.MessageInfo.Message_60 + ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wList;
        }

        private List<int> GetQcParamList()
        {
            List<int> wList=new List<int>();

            string BaseSql = "SELECT Distinct QcParam_NO FROM TmPLM WITH(NOLOCK) ORDER BY QcParam_NO ASC";

            string sqlCmdTxt = string.Format(BaseSql);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        wList.Add(Convert.ToInt32(reader["QcParam_NO"]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Constant.MessageInfo.Message_59 + ex.ToString());
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wList;
        }

        private void SetListData(string sType)
        {
            dsTmPLMEx.Clear();

            //List<int> ListQcParamNo=new List<int>();
            //List<string> ListModelNM = new List<string>();

            //ListModelNM = GetModelList();
            //ListQcParamNo = GetQcParamList();

            DateTime dtTime = Convert.ToDateTime(txtbOutputDate.Text);

            //foreach (string tmpModelNM in ListModelNM){
                //foreach (int tmpQcParamNo in ListQcParamNo)
                //{
            //string BaseSql = "SELECT max([Rev_NO]) AS Rev_NO,[QcParam_NO],[Model_NM],[Material_CD],[Class_NM],[Parameter_NM],[Manage_NM], "
            //    //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
            //    /*+ "[Timing_NM],[Parameter_MAX],[Parameter_MIN],[Parameter_VAL],[QcLine_PNT],[QcLine_MAX],[QcLine_MIN], "*/
            //                + "[Info1_NM],[Info2_NM],[Info3_NM],[Timing_NM],[Parameter_MAX],[Parameter_MIN],[Parameter_VAL],[QcLine_PNT],[QcLine_MAX],[QcLine_MIN], "
            //    //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
            //                + "[AimLine_VAL],[AimRate_VAL],[Reason_NM],[DS_FG],[Del_FG],[UpdUser_CD],[LastUpd_DT] "
            //                + "FROM [TvPLMHist] WITH(NOLOCK) "
            //                + "Where Material_CD='{0}' AND LastUpd_DT <= '{1}' ";//AND QcParam_NO={2} " AND Model_NM='{3}' ORDER BY [Rev_NO] DESC ";
                            
                    //string sqlCmdTxt = string.Format(BaseSql, sType, dtTime);//, tmpQcParamNo, tmpModelNM);

            string sql = @" SELECT Rev_NO, QcParam_NO, Model_NM, Material_CD, Class_NM, Parameter_NM, Manage_NM, Info1_NM, Info2_NM, Info3_NM, Timing_NM,
                               Parameter_MAX, Parameter_MIN, Parameter_VAL, QcLine_PNT, QcLine_MAX, QcLine_MIN, AimLine_VAL, AimRate_VAL, Reason_NM, 
                               DS_FG, Del_FG, UpdUser_CD, LastUpd_DT
                            FROM TvPLMHist WITH (NOLOCK)
                            WHERE                 
	                            exists(
	                            select max(Rev_NO) AS Rev_NO, QcParam_NO, Material_CD, Model_NM 
	                            from TvPLMHist AS TvPLMHist2 WITH (NOLOCK)
	                            where (Material_CD = @TypeCD) AND (Del_FG = 0)
	                            and (TvPLMHist.QcParam_NO = TvPLMHist2.QcParam_NO)
	                            and (TvPLMHist.Material_CD = TvPLMHist2.Material_CD)
	                            and (TvPLMHist.Model_NM = TvPLMHist2.Model_NM)
	                            group by QcParam_NO, Material_CD, Model_NM
	                            having (TvPLMHist.Rev_NO = max(rev_no))
	                            ) ";
            
                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlParameter param = new SqlParameter("@TypeCD", SqlDbType.Char);
                        param.Value = sType;
                        connect.Command.Parameters.Add(param);

                        SqlDataReader reader = null;
                        try
                        {
                            connect.Command.CommandText = sql;
                            using (reader = connect.Command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    DataRow wDataRow = dsTmPLMEx.TvPLMEx.NewRow();

                                    wDataRow["Rev"] = reader["Rev_NO"];//wDataRow[dsTmPLMEx.TvPLMEx.RevColumn] = reader["Rev_NO"];
                                    wDataRow["QcParam_NO"] = (reader["QcParam_NO"] == null ? DBNull.Value : reader["QcParam_NO"]);
                                    wDataRow["Model_NM"] = reader["Model_NM"];
                                    wDataRow["Material_CD"] = reader["Material_CD"];
                                    wDataRow["Class_NM"] = reader["Class_NM"];
                                    wDataRow["Parameter_NM"] = reader["Parameter_NM"];
                                    string sInspectionNM = Convert.ToString(wDataRow["Parameter_NM"]);
                                    if (sInspectionNM.Substring(0, 1) == "F")
                                    {
                                        sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
                                        wDataRow["Parameter_NM"] = sInspectionNM;
                                    }
                                    wDataRow["Manage_NM"] = reader["Manage_NM"];
                                    //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                                    wDataRow["Info1_NM"] = reader["Info1_NM"];
                                    wDataRow["Info2_NM"] = reader["Info2_NM"];
                                    wDataRow["Info3_NM"] = reader["Info3_NM"];
                                    //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                                    wDataRow["Timing_NM"] = reader["Timing_NM"];
                                    wDataRow["Parameter_MAX"] = (reader["Parameter_MAX"] == null ? DBNull.Value : reader["Parameter_MAX"]);
                                    wDataRow["Parameter_MIN"] = (reader["Parameter_MIN"] == null ? DBNull.Value : reader["Parameter_MIN"]);
                                    wDataRow["Parameter_VAL"] = (reader["Parameter_VAL"] == null ? DBNull.Value : reader["Parameter_VAL"]);
                                    wDataRow["QcLine_PNT"] = (reader["QcLine_PNT"] == null ? DBNull.Value : reader["QcLine_PNT"]);
                                    wDataRow["QcLine_MAX"] = (reader["QcLine_MAX"] == null ? DBNull.Value : reader["QcLine_MAX"]);
                                    wDataRow["QcLine_MIN"] = (reader["QcLine_MIN"] == null ? DBNull.Value : reader["QcLine_MIN"]);
                                    wDataRow["Reason_NM"] = (reader["Reason_NM"] == null ? DBNull.Value : reader["Reason_NM"]);
                                    wDataRow["DS_FG"] = (reader["DS_FG"] == null ? DBNull.Value : reader["DS_FG"]);
                                    wDataRow["UpdUser_CD"] = (reader["UpdUser_CD"] == null ? DBNull.Value : reader["UpdUser_CD"]);
                                    wDataRow["LastUpd_DT"] = reader["LastUpd_DT"];
                                    wDataRow["Edit_FG"] = 0;

                                    dsTmPLMEx.TvPLMEx.Rows.Add(wDataRow);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;                            
                        }
                    }
                //}
            //}
        }



        private void cmbbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbbType.Text == "新規タイプ")
            //{
            //    btnOutputForm.Enabled = false;
            //    btnReadExcel.Enabled = false;
            //    btnEntry.Enabled = false;
            //}
            //else
            //{
            //    btnOutputForm.Enabled = true;
            //    btnReadExcel.Enabled = true;
            //    btnEntry.Enabled = true;
            //}
        }

        private void cmbbLineNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbbLineNo.Text == "高効率ライン")
            //{
            //    //高効率ライン用
            //    StrQCIL = "Server=NSERVER01.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;User ID=sla02;password=2Gquf5d;Application Name=QCIL_40692";

            //}
            //else if (cmbbLineNo.Text == "自動化ライン")
            //{
            //    //自動化ライン用
            //    StrQCIL = "Server=HQDB5.nichia.local\\INST1;Connect Timeout=0;Database=QCIL;UID=sla02;PWD=2Gquf5d;Application Name=QCIL_40692";
            //}
            //else
            //{
            //    MessageBox.Show("読み込んだExcelファイルに自動化ライン・高効率ライン以外が設定されています。Excelファイルを修正下さい。");
            //    return;
            //}
            //SetType();
        }

        private void gvMasterSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            //[履歴]列の[確認]ボタンがクリックされた
            if (dgv.Columns[e.ColumnIndex].Name == "Revision")
            {
                int nRev = Convert.ToInt32(gvMasterSetting[2, e.RowIndex].Value);//Rev_NO
                int nQcParamNO = Convert.ToInt32(gvMasterSetting[3, e.RowIndex].Value);//QcParam_NO
                string sModelNM = Convert.ToString(gvMasterSetting[4, e.RowIndex].Value);//Model_NM
                string sType = Convert.ToString(gvMasterSetting[5, e.RowIndex].Value);//Material_CD

                frmMasterSettingRev frmMasterSettingRev = new frmMasterSettingRev(nRev, nQcParamNO,sModelNM, sType);
                frmMasterSettingRev.ShowDialog();
            }
        }



    }
}
