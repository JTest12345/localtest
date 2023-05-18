using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace GEICS
{
    public partial class F02_Menu : Form
    {
        /// <summary>ログオフフラグ</summary>
        public bool LogoffFG = false;

		private bool MasterDownloadDisable;

        public F02_Menu(string serverNm)
        {
            InitializeComponent();

			MasterDownloadDisable = false;

        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMenu_Load(object sender, EventArgs e)
        {
            try
            {
				if (MasterDownloadDisable)
				{
					toolbtnMiddleServer.Visible = false;
				}

                toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
                toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;

                tvMenu.ExpandAll();

                if (!Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F004)))
                {
                    //ライン反映を保持していない場合、[中間サーバ反映]非表示
                    toolbtnMiddleServer.Visible = false;
                }
                if (!Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F003)))
                {
                    //権限変更を保持していない場合、[権限管理]項目削除
                    tvMenu.Nodes["ndMaterMainte"].Nodes["ndCompetence"].Remove();
                }
                if (!Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F002)))
                {
                    //定型文変更を保持していない場合、[定型文管理]項目削除
                    tvMenu.Nodes["ndMaterMainte"].Nodes["ndStandardText"].Remove();
                }
#if AOI || CITIZEN || CEJ || MIK || KYO || KMC
				tvMenu.Nodes["ndReport"].Nodes["ndWBMachineVer"].Remove();
#endif
				if (!Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F005)))
				{
					tvMenu.Nodes["ndMaterMainte"].Nodes["ndMasterInnerLimit"].Remove();
				}

                //tvMenu.Nodes["ndMaterMainte"].Nodes["ndMasterFilefmtType"].Remove();
                //tvMenu.Nodes["ndMaterMainte"].Nodes["ndMasterFilefmtWirebond"].Remove();

            }
            catch(Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// メニュー選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvMenu_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (!this.tvMenu.SelectedNode.IsSelected)
                {
                    return;
                }
				F08_ParameterReport formParameterReport;

                switch (e.Node.Name)
                {
                    case "ndMachineTrend":
                        F04_ErrorRecord formMachineTrend = new F04_ErrorRecord();
                        formMachineTrend.ShowDialog();
                        formMachineTrend.Dispose();
                        break;

                    case "ndStandardText":
                        M05_StandardText formStandardText = new M05_StandardText();
                        formStandardText.ShowDialog();
                        break;

                    case "ndCompetence":
                        M04_UserFunction formUserFunction = new M04_UserFunction();
                        formUserFunction.ShowDialog();
                        break;

                    case "ndMasterParameter":
                        M03_Parameter formMasterParameter = new M03_Parameter(true);
                        formMasterParameter.ShowDialog();
                        break;

                    case "ndMasterParameterSummary":
                        M16_ParameterSummary formMasterParameterSummary = new M16_ParameterSummary();
                        formMasterParameterSummary.ShowDialog();
                        break;

                    case "ndMasterLimit":
						//閾値変更を保持している場合、書き込みモードで開く
                        if (Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F001)))
                        {
                            M01_Limit formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.Write);
                            formMasterLimit.ShowDialog();
                        }
                        else 
                        {
                            M01_Limit formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.Read);
                            formMasterLimit.ShowDialog();
                        }
                        break;
                    case "ndMasterInnerLimit":
                        M01_Limit formMasterInnerLimit = new M01_Limit(M01_Limit.FunctionStyle.InnerLimitEdit);
                        formMasterInnerLimit.ShowDialog();

                        break;
                    case "ndMasterPhosphorSheetLimit":
                        //閾値変更を保持している場合、書き込みモードで開く
                        if (Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F001_PS)))
                        {
                            M01_Limit formMasterPhosphorSheetLimit = new M01_Limit(M01_Limit.FunctionStyle.PhosphorSheetWrite);
                            formMasterPhosphorSheetLimit.ShowDialog();
                        }
                        else
                        {
                            M01_Limit formMasterPhosphorSheetLimit = new M01_Limit(M01_Limit.FunctionStyle.PhosphorSheetRead);
                            formMasterPhosphorSheetLimit.ShowDialog();
                        }
                        break;
                    case "ndMasterFilefmtTypeWirebonder":
						M12_FileFmtTypeWirebonder form12 = new M12_FileFmtTypeWirebonder();
						form12.ShowDialog();
						break;
					case "ndMasterFilefmtWirebonder":
						M13_FileFmtWirebonder form13 = new M13_FileFmtWirebonder();
						form13.ShowDialog();
						break;
					case "ndMasterMsgfmtTypeWirebonder":
						M14_MsgFmtTypeWirebonder form14 = new M14_MsgFmtTypeWirebonder();
						form14.ShowDialog();
						break;
					case "ndMasterMsgfmtWirebonder":
						M15_MsgFmtWirebonder form15 = new M15_MsgFmtWirebonder();
						form15.ShowDialog();
						break;
                    case "ndMasterPlcFileConv":
                        M17_PlcFileConv form17 = new M17_PlcFileConv();
                        form17.ShowDialog();
                        break;
#if AOI || CITIZEN || CEJ || MIK || KYO || KMC
#else
					case "ndWBMachineVer":
						formParameterReport = new F08_ParameterReport(F08_ParameterReport.ReportType.WBEquipmentVersion);
						formParameterReport.ShowDialog();
						break;
#endif
					case "ndParameter":
						formParameterReport = new F08_ParameterReport(F08_ParameterReport.ReportType.ParameterPerLot);
						formParameterReport.ShowDialog();
						break;

				}
            }
            catch (Exception err) 
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ログオフボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbtnLogoff_Click(object sender, EventArgs e)
        {
            this.LogoffFG = true;
            this.Close();
            this.Dispose();

            Common.nLineCD = int.MinValue;
        }

        /// <summary>
        /// 中間サーバへ反映ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbtnMiddleServer_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show(Constant.MessageInfo.Message_62, "Exclamation", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
            {
                return;
            }

            try
            {
                string parameterVAL = string.Empty; string batchPath = string.Empty;
                switch (Constant.StrQCIL)
                {
      //              case Constant.StrQCIL_MAP_AUTO:
      //                  parameterVAL = "/GEICSMasterDownload";
						//batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatch(LENS2Ver)Path"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
      //                  break;
                    case Constant.StrQCIL_MAPOUT:
                        parameterVAL = "/GEICSMasterDownload_MAPOUT";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
                    case Constant.StrQCIL_SV_AUTO:
                        parameterVAL = "/GEICSMasterDownload_SIDEAUTO";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatch(LENS2Ver)Path"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
                    case Constant.StrQCIL_HQDB5_KYO:
                        parameterVAL = "/GEICSMasterDownload_KYOSEMI";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
					case Constant.StrQCIL_KYO_3in1:
						parameterVAL = "/GEICSMasterDownload_KYOSEMI";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
      //              case Constant.StrQCIL_HQDB5_MIK:
      //                  parameterVAL = "/GEICSMasterDownload_MIKSEMI";
						//batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatch(LENS2Ver)Path"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						//break;
                    case Constant.StrQCIL_HQDB5_AOI:
                        parameterVAL = "/GEICSMasterDownload_AOIAUTO";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatch(LENS2Ver)Path"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
                    case Constant.StrQCIL_HQDB5_AOI_SEMI:
                        parameterVAL = "/GEICSMasterDownload_AOISEMI";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatch(LENS2Ver)Path"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
					case Constant.StrQCIL_AOI_MAP_AUTO:
						parameterVAL = "/GEICSMasterDownload_AOIMAPAUTO";
						//batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), "AOI_MAP_AUTO");
						//batchPath = Path.Combine(batchPath, SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
                    case Constant.StrQCIL_NMC:
                        parameterVAL = "/GEICSMasterDownload_NMCOUT";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
					case Constant.StrQCIL_AOI_MAP_SEMI:
						parameterVAL = "/GEICSMasterDownload_AOIMAPHIGH";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
					case Constant.StrQCIL_CE:
						parameterVAL = "/GEICSMasterDownload_CEJMAPHIGH";
						batchPath = Path.Combine(SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath"), SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchExeName"));
						break;
                    default:
                        throw new Exception(Constant.MessageInfo.Message_68);
                }

				//batchPath = SLCommonLib.Commons.Configuration.GetAppConfigString("MasterDownloadBatchPath");

                if (Process.GetProcessesByName(batchPath.Replace(".exe", "")).Length >= 1)
                {
                    //バッチが実行中の場合、処理を抜ける
                    MessageBox.Show(Constant.MessageInfo.Message_61, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //バッチを実行した後、完了するまで応答を待つ
                System.Diagnostics.Process process = Process.Start(batchPath, parameterVAL);
                process.WaitForExit();
                MessageBox.Show(Constant.MessageInfo.Message_63, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err) 
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

		private void toolStripButton1_Click(object sender, EventArgs e)
		{

			int nLineCD = 0;
			int QcnrNO = 0;
			int DefectNO = 0;
			string DefectNM = "";
			string LotNO = "";
			int BackNum = 0;
			string Type = "";
			string InspectionNM = "";
			string Result = "";
			int MultiNO = 0;
			string sEquiNO = "";
			string sAssetsNM = "";
			DateTime dtMeasure = Convert.ToDateTime("9999/01/01");
			int InspectionNO = 0;

			//nLineCD = Convert.ToInt32(cmbbLineNo.Text.Trim());
			nLineCD = Common.nLineCD;

			DefectNO = 0;
			//int nCnfmNO = Com.GetTnQCNRCnfm_CnfmNO(nLineCD, QcnrNO);        //Confirm_NO
			//int nCheckNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["Check_NO"].Value);           //int nCheckNO = Convert.ToInt32(gvQCErrList[17, e.RowIndex].Value);//Check_NO
			string timingNM = "";

			F03_TrendChart frmDrawGraphAndList = new F03_TrendChart(nLineCD, DefectNO, DefectNM, LotNO, Type, Result, InspectionNM, dtMeasure.AddMinutes(10), sEquiNO, sAssetsNM, BackNum, MultiNO, InspectionNO, timingNM);

			frmDrawGraphAndList.ShowDialog();
			GC.Collect();
		}
    }
}
