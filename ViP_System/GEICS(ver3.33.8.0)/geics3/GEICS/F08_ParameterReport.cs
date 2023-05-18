using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using GEICS.Report;

namespace GEICS
{
	public partial class F08_ParameterReport : Form
	{
		private List<F80_MultiSelectPlant.MultiSelectPlant> selectPlantList { get; set; }
		private List<F81_MultiSelectParam.MultiSelectParam> selectParamList { get; set; }

		private ReportType ReportTypeInfo { get; set; }
		ExcelControl excel;

		public enum ReportType
		{
			/// <summary>ロット毎パラメータレポート</summary>
			ParameterPerLot,
			/// <summary>装置バージョンレポート</summary>
			WBEquipmentVersion	
		}

		private string[] WBMachineKindList = new string[3]
		{		
			"FB-700",
			"FB-880",
			"FB-900"
		};

		public F08_ParameterReport(ReportType reportType)
		{
			try
			{
				InitializeComponent();

				selectPlantList = new List<F80_MultiSelectPlant.MultiSelectPlant>();
				selectParamList = new List<F81_MultiSelectParam.MultiSelectParam>();



				switch (reportType)
				{
					case ReportType.ParameterPerLot:
						//"ロット毎パラメータレポート";
						this.Text = "ロット毎パラメータレポート";
						InitParameterPerLotReport();
						break;
					case ReportType.WBEquipmentVersion:
						this.Text = "WB装置バージョン変更履歴";
						InitWBEquipmentVersionReport();
						break;
				}

				ReportTypeInfo = reportType;
			}
			catch (Exception err)
			{
				if (excel != null)
				{
					excel.Dispose();
				}

				throw new Exception(err.Message);
			}
		}

		~F08_ParameterReport()
		{
			if (excel != null)
			{
				excel.Dispose();
			}
		}

		/// <summary>ロット毎パラメータレポートモードで初期化</summary>
		private void InitParameterPerLotReport()
		{
			
		}

		/// <summary>WB装置バージョン変更履歴モードで初期化</summary>
		private void InitWBEquipmentVersionReport()
		{
			this.tbParamNO.Text = Constant.WBEquipVerParamNO.ToString();
			this.tbParamNO.Enabled = false;
			this.btnParamMultiSelect.Enabled = false;
		}

		private int? GetAssignAssetsCD()
		{
			//"ロット毎パラメータレポート";
			if(ReportTypeInfo == ReportType.ParameterPerLot)
			{
				return null;
			}
			else if (ReportTypeInfo == ReportType.WBEquipmentVersion)
			{

				return (int)Constant.ASEETS_CD.WB;
			}
			else
			{
				return null;
			}
		}

		private void F08_ParameterReport_Load(object sender, EventArgs e)
		{
			try
			{
				//サーバの読込み
				NameValueCollection nvc = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ServerList");

				for(int index = 0; index < nvc.Count; index++)
				{
					clbServer.Items.Add(nvc.GetValues(index)[0]);
				}

				dtsTargetDT.StartDate = DateTime.Today.Date.ToString();
				dtsTargetDT.EndDate = DateTime.Today.AddDays(1).Date.ToString();

			}
			catch (Exception err)
			{
				throw new Exception(err.Message);
			}
		}

		/// <summary>
		/// サーバ全選択ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSelect_Click(object sender, EventArgs e)
		{
			for (int index = 0; index < clbServer.Items.Count; index++)
			{
				clbServer.SetItemCheckState(index, CheckState.Checked);
			}
		}

		/// <summary>
		/// サーバ全解除ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCansel_Click(object sender, EventArgs e)
		{
			for (int index = 0; index < clbServer.Items.Count; index++)
			{
				clbServer.SetItemCheckState(index, CheckState.Unchecked);
			}
		}

		/// <summary>設備複数選択ボタン押下</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPlantMultiSelect_Click(object sender, EventArgs e)
		{

			if (clbServer.CheckedItems.Count == 0)
			{
				MessageBox.Show("サーバを選択して下さい。");
				return;
			}

			List<string> selectServerNMList = GetSelectServerNMList();

			F80_MultiSelectPlant formMultiSelect = new F80_MultiSelectPlant(selectServerNMList, selectPlantList, GetAssignAssetsCD());
			formMultiSelect.ShowDialog();

			selectPlantList = formMultiSelect.GetMultiSelectPlant();

			if (selectPlantList.Count > 0)
			{
				btnPlantMultiSelect.ImageIndex = 1;
			}
			else
			{
				btnPlantMultiSelect.ImageIndex = 0;
			}

		}

		/// <summary>
		/// 選択したサーバ名を取得
		/// </summary>
		/// <returns></returns>
		private List<string> GetSelectServerNMList()
		{
			string[] selectServerNM = new string[clbServer.CheckedItems.Count];
			clbServer.CheckedItems.CopyTo(selectServerNM, 0);

			return selectServerNM.ToList();
		}

		/// <summary>パラメータ複数選択ボタン押下</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnParamMultiSelect_Click(object sender, EventArgs e)
		{
			if (selectPlantList.Count() == 0)
			{
				MessageBox.Show("設備を選択して下さい。");
				return;
			}

			List<string> selectServerNMList = GetSelectServerNMList();

			// 選択された設備に関係のあるパラメータに絞り込む為に、装置種類を取得
			List<string> modelNMList = selectPlantList.Select(s => s.ModelNM).Distinct().ToList();

			//for (int index = 0; index < modelNMList.Count; index++)
			//{
			//	foreach (string wbMachineKind in WBMachineKindList)
			//	{
			//		if (modelNMList[index] == wbMachineKind)
			//		{
			//			modelNMList[index] = "WB";
			//		}
			//	}
			//}

			modelNMList = modelNMList.Distinct().ToList();

			// 選択された設備のあるサーバに接続先サーバを限定する為の処理
			if (selectPlantList.Count != 0)
			{
				selectServerNMList = selectPlantList.Select(s => s.ServerNM).Distinct().ToList();
			}

			F81_MultiSelectParam formMultiSelect = new F81_MultiSelectParam(selectServerNMList, selectParamList, modelNMList);
			formMultiSelect.ShowDialog();

			selectParamList = formMultiSelect.GetMultiSelectParam();

			if (selectParamList.Count > 0)
			{
				btnParamMultiSelect.ImageIndex = 1;
			}
			else
			{
				btnParamMultiSelect.ImageIndex = 0;
			}
		}

		/// <summary>レポート出力ボタン押下</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOutputReport_Click(object sender, EventArgs e)
		{
			try
			{

				if (selectParamList.Count() == 0 && string.IsNullOrEmpty(tbParamNO.Text))
				{
					MessageBox.Show("パラメータを選択して下さい。");
					return;
				}

				excel = ExcelControl.GetInstance();

				switch (ReportTypeInfo)
				{
					case ReportType.ParameterPerLot:
						OutputParameterReport();
						break;
					case ReportType.WBEquipmentVersion:
						OutputWBEquipVerReport();
						break;
				}
			}
			catch (Exception err)
			{
				if (excel != null)
				{
					excel.Dispose();
				}

				throw new Exception(err.Message);
			}
		}

		private void OutputWBEquipVerReport()
		{
			List<WBEquipVerReport> reportData = GetWBVerReportDataSelectServer();

			if (IsOutputableRowCountToExcel(reportData.Count()) == false)
			{
				return;
			}

			object[,] arrayForXls = WBEquipVerReport.ConvertToExcelData(reportData);
			
			List<string> selectServerNMList = GetSelectServerNMList();
			List<string> selectPlantCDList = selectPlantList.Select(s => s.EquipmentNO).ToList();
			List<int> selectParamNOList = new List<int>();
			selectParamNOList.Add(Constant.WBEquipVerParamNO);
			string sTargetDT = string.Format("{0} ～ {1}", dtsTargetDT.StartDate, dtsTargetDT.EndDate);

			excel.OutputWBVerReportData(arrayForXls, selectServerNMList, selectPlantCDList, selectParamNOList, sTargetDT);
		}

		private List<WBEquipVerReport> GetWBVerReportDataSelectServer()
		{
			List<WBEquipVerReport> reportData = new List<WBEquipVerReport>();

			DateTime startDT = DateTime.Parse(dtsTargetDT.StartDate);
			DateTime endDT = DateTime.Parse(dtsTargetDT.EndDate);

			// 選択された装置のリストから関係するサーバを取得
			List<string> serverNMList = selectPlantList.Select(s => s.ServerNM).Distinct().ToList();

			foreach (string serverNM in serverNMList)
			{
				string connStr = ConnectQCIL.GetQCILConnStrFromServerNM(serverNM);
				List<string> selectPlantCDList = selectPlantList.Select(s => s.EquipmentNO).ToList();
				//サーバ単位でレポートデータの取得
				List<WBEquipVerReport> reportDataListPerServer = WBEquipVerReport.GetReportDataFromServer(connStr, selectPlantCDList, startDT, endDT);

				reportData.AddRange(reportDataListPerServer);
			}
			return reportData;
		}

		private void OutputParameterReport()
		{
			try
			{
				ParameterReport reportData = GetParamReportDataSelectServer();

				if (IsOutputableRowCountToExcel(reportData.LotInfoList.Count()) == false)
				{
					return;
				}

				object[,] arrayForXls = ParameterPerLotReport.ConvertToExcelData(reportData);
				Dictionary<int, KeyValuePair<string, bool>> paramHeader = ParameterPerLotReport.GetParamHeaderData();
				List<string> selectServerNMList = GetSelectServerNMList();
				List<string> selectPlantCDList = selectPlantList.Select(s => s.EquipmentNO).ToList();
				List<int> selectParamNOList = selectParamList.Select(s => s.QcParamNO).ToList();
				string sTargetDT = string.Format("{0} ～ {1}", dtsTargetDT.StartDate, dtsTargetDT.EndDate);

				excel.OutputParameterReportData(paramHeader, arrayForXls, selectServerNMList, selectPlantCDList, selectParamNOList, sTargetDT);
			}
			catch (Exception err)
			{
				throw new Exception(err.Message, err);
			}
		}

		private bool IsOutputableRowCountToExcel(int rowCount)
		{
			if (rowCount == 0)
			{
				MessageBox.Show("抽出対象が存在しません。検索条件を見直して下さい。");
				return false;
			}
			if (rowCount > 65535)
			{
				MessageBox.Show("Excelの出力行の限界を超えています。検索条件を見直して下さい。");
				return false;
			}

			return true;
		}

		private ParameterReport GetParamReportDataSelectServer()
		{
			try
			{
				ParameterReport reportData = new ParameterReport();
				reportData.LotInfoList = new List<ParameterPerLotReport>();
				reportData.ParameterList = new List<Parameter>();

				DateTime startDT = DateTime.Parse(dtsTargetDT.StartDate);
				DateTime endDT = DateTime.Parse(dtsTargetDT.EndDate);

				// 選択されたパラメタのリストから関係するサーバを取得
				List<string> serverNMList = selectParamList.Select(s => s.ServerNM).Distinct().ToList();

				foreach (string serverNM in serverNMList)
				{
					//サーバに関係する設備に限定して取得
					List<string> targetPlantList = selectPlantList.Where(s => s.ServerNM == serverNM).Select(s => s.EquipmentNO).ToList();
					//サーバに関係するパラメタに限定して取得
					List<int> targetParamList = selectParamList.Where(s => s.ServerNM == serverNM).Select(s => s.QcParamNO).ToList();

					string connStr = ConnectQCIL.GetQCILConnStrFromServerNM(serverNM);

					//サーバ単位でレポートデータの取得
					ParameterReport reportDataPerServer = ParameterPerLotReport.GetReportDataFromServer(connStr, targetPlantList, targetParamList, startDT, endDT, chkSingleMagazineData.Checked);

					reportData.LotInfoList.AddRange(reportDataPerServer.LotInfoList);
					reportData.ParameterList.AddRange(reportDataPerServer.ParameterList);
				}
				return reportData;
			}
			catch (Exception err)
			{
				throw new Exception(err.Message, err);
			}
		}

		private void dtsTargetDT_Leave(object sender, EventArgs e)
		{
		}
	}
}
