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

namespace GEICS
{
    public partial class frmMasterSettingRev : Form
    {
        static int _nRev;
        static int _nQcParamNO;
        static string _sModelNM;
        static string _sType;
        Common Com = new Common();

        public frmMasterSettingRev(int __nRev, int __nQcParamNO,string __sModelNM, string __sType)
        {
            InitializeComponent();

            _nRev = __nRev;
            _nQcParamNO = __nQcParamNO;
            _sModelNM = __sModelNM;
            _sType = __sType;
        }

        private void SetListData(int nrev, int nQcParamNo,string sModelNM, string sType)
        {
            //dsTmPLMEx1.Clear();

            List<int> ListQcParamNo = new List<int>();
            List<string> ListModelNM = new List<string>();

            ListQcParamNo.Add(nQcParamNo);
            ListModelNM.Add(sModelNM);
            //DateTime dtTime = Convert.ToDateTime(txtbOutputDate.Text);

            foreach (string tmpModelNM in ListModelNM)
            {
                foreach (int tmpQcParamNo in ListQcParamNo)
                {
                    string BaseSql = "SELECT [Rev_NO],[QcParam_NO],[Model_NM],[Material_CD],[Class_NM],[Parameter_NM],[Manage_NM], "
                        //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                        /*+ "[Timing_NM],[Parameter_MAX],[Parameter_MIN],[Parameter_VAL],[QcLine_PNT],[QcLine_MAX],[QcLine_MIN], "*/
                                    + "[Info1_NM],[Info2_NM],[Info3_NM],[Timing_NM],[Parameter_MAX],[Parameter_MIN],[Parameter_VAL],[QcLine_PNT],[QcLine_MAX],[QcLine_MIN], "
                        //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                                    + "[AimLine_VAL],[AimRate_VAL],[Reason_NM],[DS_FG],[Del_FG],[UpdUser_CD],[LastUpd_DT] "
                                    + "FROM [TvPLMHist] WITH(NOLOCK) "
                                    + "Where Material_CD='{0}' AND  QcParam_NO={1} AND Model_NM='{2}' AND Rev_NO<={3} ORDER BY [Rev_NO] DESC ";

                    string sqlCmdTxt = string.Format(BaseSql, sType, tmpQcParamNo, tmpModelNM,nrev);

                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            connect.Command.CommandText = sqlCmdTxt;
                            reader = connect.Command.ExecuteReader();
                            while (reader.Read())
                            {
                                DataRow wDataRow = dsTmPLMEx1.TvPLMEx.NewRow();

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

                                dsTmPLMEx1.TvPLMEx.Rows.Add(wDataRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            //MessageBox.Show("エラーリストの取得に失敗しました。もう一度取得して下さい。\r\n\r\n [詳細]=" + ex.ToString());
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

        private void frmMasterSettingRev_Load(object sender, EventArgs e)
        {
            SetListData(_nRev, _nQcParamNO,_sModelNM, _sType);
            gvMasterSetting.Refresh();
        }
    }
}
