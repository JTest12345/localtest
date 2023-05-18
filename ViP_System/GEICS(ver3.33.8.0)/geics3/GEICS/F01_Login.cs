using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Collections.Specialized;
using GEICS.Database;

namespace GEICS
{
    public partial class F01_Login : Form
    {

        public F01_Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {

                SetLineSetting();

//#if TEST
//				Constant.EmployeeInfo = EmployeeInfo.GetLoginEmpInfo(txtEmployeeID.Text.Trim(), ConnectQCIL.CONNECT_SERVER_DEBUG + ":" + cmbServer.Text);
//#else 
                //ユーザ情報取得
                Constant.EmployeeInfo = EmployeeInfo.GetLoginEmpInfo(txtEmployeeID.Text.Trim(), cmbServer.Text);
//#endif

                this.Hide();
                this.UpdateStyles();

				F02_Menu formMenu = new F02_Menu(cmbServer.Text);
                formMenu.ShowDialog();

                if (formMenu.LogoffFG)
                {
                    this.Show();
                }
                else
                {
                    Application.Exit();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {

                //ログインサーバー設定
                NameValueCollection nvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ServerList");
                cmbServer.SourceNVC = nvc;

                //cmbServer.Items.Add("試用サーバ");

                //foreach (KeyValuePair<string, string> keyValPair in nvc)
                //{
                //    cmbServerList.Items.Add(keyValPair.Key);
                //}
				cmbServerList.Visible = false;
				label5.Visible = false;

//#if DEBUG
//                label2.Visible = false;
//                cmbServer.Visible = false;

//                cmbServerList.Visible = true;
//                label5.Visible = true;

//                cmbServerList.Items.Add("試用サーバ");
//                this.Text = string.Format("{0}:Debug版", this.Text);
//#endif
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetLineSetting() 
        {
            NameValueCollection ArmsServerNvc;
//#if DEBUG
//switch (cmbServerList.Text)
//{
//#else
            switch (cmbServer.Text)
            {
                //#endif
#if NICHIA
     //           case "MAP自動搬送":
     //               Constant.fSemi = false;
     //               Constant.typeGroup = Constant.TypeGroup.MAP;
     //               Constant.fOutline = false;
     //               Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					//Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_AUTO, ConnectQCIL.CONNECT_PASS_MAP_AUTO);
     //               break;
     //           case "MAP高生産性":
     //               Constant.fSemi = true;
     //               Constant.typeGroup = Constant.TypeGroup.MAP;
     //               Constant.fOutline = false;
     //               Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					//Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_HIGH, ConnectQCIL.CONNECT_PASS_MAP_HIGH);
     //               break;
                case "MAP合理化":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.MAP;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_HIGH, ConnectQCIL.CONNECT_PASS_MAP_HIGH);
                    ArmsServerNvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ArmsServerList");
                    Constant.StrARMS = string.Format(ArmsServerNvc[cmbServer.Text], ConnectQCIL.CONNECT_ID_MAP_AUTO, ConnectQCIL.CONNECT_PASS_MAP_HIGH);
                    break;
                case "3in1高生産性":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SMD3in1;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
				case "19/SHP高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.X19;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    Common.notUseTmQdiwFG = true;
					break;
				case "MPL合理化":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.MPL;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    Common.notUseTmQdiwFG = true;
					break;
				case "83/385/48/COB高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.X83_385_COB;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    Common.notUseTmQdiwFG = true;
					break;
                case "DMC":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.DMC;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    Common.notUseTmQdiwFG = true;
                    break;
#endif
#if KYO || NICHIA
				case "SIDEVIEW合理化(京都)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_KYO, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_HIGH_KYO, ConnectQCIL.CONNECT_PASS_SV_HIGH_KYO);
                    break;
				case "3in1合理化(京都)":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.SMD3in1;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_KYO, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
                case "3in1高生産性(京都A棟)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SMD3in1;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_KYO, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
                case "3in1高生産性(京都B棟)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SMD3in1;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_KYO, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;

#endif
#if MIK || NICHIA
                //            case "SIDEVIEW高生産性(三方)":
                //                Constant.fSemi = true;
                //                Constant.typeGroup = Constant.TypeGroup.SV;
                //                Constant.fOutline = false;
                //                Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_MIK, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                //	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_HIGH_MIK, ConnectQCIL.CONNECT_PASS_SV_HIGH_MIK);
                //                break;
                //case "SIDEVIEW自動搬送(三方)":
                //	Constant.fSemi = false;
                //	Constant.typeGroup = Constant.TypeGroup.SV;
                //	Constant.fOutline = false;
                //	Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_MIK, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                //	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_HIGH_MIK, ConnectQCIL.CONNECT_PASS_SV_HIGH_MIK);
                //	break;

                case "SIDEVIEW合理化(三方)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_MIK, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_HIGH_MIK, ConnectQCIL.CONNECT_PASS_SV_HIGH_MIK);
                    ArmsServerNvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ArmsServerList");
                    Constant.StrARMS = string.Format(ArmsServerNvc[cmbServer.Text], ConnectQCIL.CONNECT_ID_SV_HIGH_MIK, ConnectQCIL.CONNECT_PASS_SV_HIGH_MIK);
                    break;
#endif
#if AOI || NICHIA
                case "SIDEVIEW自動搬送(アオイ)":
                    Constant.fSemi = false;
                    Constant.typeGroup = Constant.TypeGroup.SV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_AUTO_AOI, ConnectQCIL.CONNECT_PASS_SV_AUTO_AOI);
                    break;
                case "SIDEVIEW高生産性(アオイ)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.SV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_SV_HIGH_AOI, ConnectQCIL.CONNECT_PASS_SV_HIGH_AOI);
                    break;
				//case "MAP高生産性(アオイ)":
				//	Constant.fSemi = true;
    //                Constant.typeGroup = Constant.TypeGroup.MAP;
    //                Constant.fOutline = false;
				//	Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
				//	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_HIGH_AOI, ConnectQCIL.CONNECT_PASS_MAP_HIGH_AOI);
				//	break;
				//case "MAP自動搬送(アオイ)":
				//	Constant.fSemi = false;
				//	Constant.typeGroup = Constant.TypeGroup.MAP;
				//	Constant.fOutline = false;
				//	Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
				//	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_AUTO_AOI, ConnectQCIL.CONNECT_PASS_MAP_AUTO_AOI);
				//	break;
                case "MAP合理化(アオイ)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.MAP;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_MAP_HIGH_AOI, ConnectQCIL.CONNECT_PASS_MAP_HIGH_AOI);
                    ArmsServerNvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ArmsServerList");
                    Constant.StrARMS = string.Format(ArmsServerNvc[cmbServer.Text], ConnectQCIL.CONNECT_ID_MAP_AUTO_AOI, ConnectQCIL.CONNECT_PASS_MAP_HIGH_AOI);
                    break;
                case "NTSV合理化(アオイ)":
                    Constant.fSemi = false;
                    Constant.typeGroup = Constant.TypeGroup.NTSV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_AOI, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_NTSV_AOI, ConnectQCIL.CONNECT_PASS_NTSV_AOI);
                    break;
#endif
#if NICHIA
                case "LampOut":
                    Constant.fSemi = false;
                    Constant.typeGroup = Constant.TypeGroup.LAMP;
                    Constant.fOutline = true;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_NMC, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_LAMP_OUT_NMC, ConnectQCIL.CONNECT_PASS_LAMP_OUT_NMC);
                    break;
				//case "GA自動搬送":
				//	Constant.fSemi = false;
				//	Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
    //                Constant.fOutline = false;
				//	Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
				//	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
				//	break;
				//case "GA高生産性":
				//	Constant.fSemi = true;
				//	Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
				//	Constant.fOutline = false;
				//	Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
				//	Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
				//	break;
                case "GA合理化":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    ArmsServerNvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ArmsServerList");
                    Constant.StrARMS = string.Format(ArmsServerNvc[cmbServer.Text], ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
                case "NTSV/DMC合理化":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.NTSV;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
				case "VOYAGER/CSP/R-PKG/N-KIRAMEKI/A-MAP高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.VOYAGER_CSP_093_KIRAMEKI;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
				case "CSP高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.CSP;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
				case "093高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup._093;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
				case "KIRAMEKI高生産性":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.KIRAMEKI;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
					break;
                //case "NLCV-DMC":
                //    Constant.fSemi = true;
                //    Constant.typeGroup = Constant.TypeGroup.NLCV;
                //    Constant.fOutline = false;
                //    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                //    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                //    break;
                case "NLCV-導光板":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.NLCV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
                case "NLCV-セグメント":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.NLCV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
                case "NLCV-二次実装":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.NLCV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
                    break;
#endif
#if CITIZEN || CEJ || NICHIA
     //           case "MAP自動搬送（シチズン境川）":
     //               Constant.fSemi = true;
     //               Constant.typeGroup = Constant.TypeGroup.MAP;
     //               Constant.fOutline = false;
     //               Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_CE, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
     //               Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_CE, ConnectQCIL.CONNECT_PASS_CE);
     //               break;

     //           case "MAP高生産性(シチズン境川)":
					//Constant.fSemi = true;
     //               Constant.typeGroup = Constant.TypeGroup.MAP;
     //               Constant.fOutline = false;
					//Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_CE, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					//Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_CE, ConnectQCIL.CONNECT_PASS_CE);
					//break;

                case "MAP合理化(シチズン境川)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.MAP;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_CE, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_CE, ConnectQCIL.CONNECT_PASS_CE);
                    break;

                case "COB高生産性(シチズン)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.COB;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_CE, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_CE, ConnectQCIL.CONNECT_PASS_CE);
                    break;

#endif
#if KMC || NICHIA
                case "KIRAMEKI高生産性(小糸製作所)":
                    Constant.fSemi = true;
                    Constant.typeGroup = Constant.TypeGroup.KIRAMEKI;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA_KMC, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_ID_KMC, ConnectQCIL.CONNECT_PASS_KMC);
                    break;
#endif
#if DEBUG
                case "試用サーバ":
					Constant.fSemi = false;
					Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                    Constant.StrQCIL = string.Format(@"Server={0};Connect Timeout=0;Database=QCIL;User ID={1};password={2};Application Name=インライン傾向管理システム", ConnectQCIL.CONNECT_SERVER_DEBUG, ConnectQCIL.CONNECT_ID_DEBUG, ConnectQCIL.CONNECT_PASS_DEBUG);
                    break;
#endif

                //case "試用サーバ":
                //    Constant.fSemi = false;
                //    Constant.typeGroup = Constant.TypeGroup.AUTOMOTIVE;
                //    Constant.fOutline = false;
                //    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
                //    Constant.StrQCIL = string.Format(@"Server={0};Connect Timeout=0;Database=QCIL;User ID={1};password={2};Application Name=インライン傾向管理システム", @"SLA-0040-2\SQLEXPRESS", "inline", "R28uHta");
                //    break;
//#if SIGMA
				case "SIGMA":
					Constant.fSemi = true;
					Constant.typeGroup = Constant.TypeGroup.SIGMA;
					Constant.fOutline = false;
					Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
					break;
//#endif
#if NICHIA
				default:
                    Constant.fSemi = false;
                    Constant.typeGroup = Constant.TypeGroup.SV;
                    Constant.fOutline = false;
                    Constant.StrNASCA = string.Format(GEICS.Properties.Settings.Default.ConnectionString_NASCA, ConnectNASCA.CONNECT_USER_ID, ConnectNASCA.CONNECT_USER_PASS);
					Constant.StrQCIL = string.Format(Convert.ToString(cmbServer.SelectedValue), ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
                    break;
#endif
            }
#if TEST
			//Constant.StrQCIL = string.Format(@"Server={0};Connect Timeout=0;Database=QCIL;User ID={1};password={2};Application Name=インライン傾向管理システム", ConnectQCIL.CONNECT_SERVER_DEBUG, ConnectQCIL.CONNECT_ID_DEBUG, ConnectQCIL.CONNECT_PASS_DEBUG);
            //Constant.StrQCIL = string.Format(@"Server=sla-0040-2\SQLEXPRESS;Connect Timeout=0;Database=QCIL_SV11;User ID={0};password={1};Application Name=インライン傾向管理システム", ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
#else
#endif

        }

        /// <summary>
        /// 終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
