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
using System.Collections;
using GEICS.Database;

namespace GEICS
{
	public partial class F03_TrendChart : Form
	{
		//初期データ
		bool _IsOnlyEqui_FG = false;            //単設備モード
		int _OnlyEqui_BeforAfterLotCT = 0;      //前後ロット数
		string _OnlyEqui_LotNO = "";            //装置番号
		string _OnlyEqui_EquiNO = "";           //装置番号
		int _nLineCD = 0;
		int _nDefectNO = 0;
		string _sDefectNM = "";
		string _sLotNO = "";
		string _sType = "";
		string _sResult = "";
		string _sInspectionNM = "";
		string _sInspectionNMAdd = "";
		DateTime _dtMeasure = DateTime.MinValue;
		string _sEquiNO = "";
		string _sEquiNOAdd = "";
		string _sAssetsNM = "";
		int _nBackNum = 0;
		int _nMultiNO = 0;
		int _nProcessNO = 0;
		bool _IsNascaConnectFG = true;
		string _TimingNM = string.Empty;

		//グラフ描画[＜戻る][進む>]用
		int nTotalGraphNum = 0;
		int nWatchGraphNum = 0;
		int nRelationGraphNum = 0;
		int nCurrentGraphNum = 0;

		//一発目に表示した監視項目
		int nArrartProcessNO = 0;

		Common Com = new Common();
		Painter p = new Painter();

		List<SortedList<int, QCLogData>> cndDataItem_One = new List<SortedList<int, QCLogData>>();
		List<SortedList<int, QCLogData>> cndDataItem_Watch = new List<SortedList<int, QCLogData>>();
		List<SortedList<int, QCLogData>> cndDataItem_Relation = new List<SortedList<int, QCLogData>>();

		SortedList<int, GraphData> ListGraphData_One = new SortedList<int, GraphData>();
		SortedList<int, GraphData> ListGraphData_Watch = new SortedList<int, GraphData>();
		SortedList<int, GraphData> ListGraphData_Relation = new SortedList<int, GraphData>();

		//グラフに描画されているLotのリスト
		List<string> ListLot = new List<string>();

		//描画対象Lot
		List<string> _TargetLotList = new List<string>();

		//DataGridViewでクリックした内容を有しているLotリスト
		List<string> ListSameLot = new List<string>();

		/// <summary>ロット・設備グリッド用テーブル</summary>
		DataTable _TblLotStb = new DataTable();
		Dictionary<string, string> _dicStb = new Dictionary<string, string>(); //設備ディクショナリ

		frmDrawGraph frmDrawGraph;
		frmDrawGraphCatalog frmDrawGraphCatalog;

		int getDataCompCt = 0;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="nLineCD"></param>
		/// <param name="nDefectNO">不具合No</param>
		/// <param name="sLotNO">異常のあったLotNO</param>
		/// <param name="sType">異常のあったType</param>
		/// <param name="sResult"></param>
		/// <param name="sInspectionNM"></param>
		/// <param name="dtMeasure"></param>
		/// <param name="sEquiNO"></param>
		/// <param name="sAssetsNM"></param>
		/// <param name="nBackNum">異常のあったLotNOから遡るデータ数</param>
		/// <param name="nMultiNO"></param>
		/// <param name="nProcessNO"></param>
		public F03_TrendChart(int nLineCD, int nDefectNO, string sDefectNM, string sLotNO, string sType, string sResult, string sInspectionNM, DateTime dtMeasure, string sEquiNO, string sAssetsNM, int nBackNum, int nMultiNO, int nProcessNO, string timingNM)
		{
			InitializeComponent();

			_nLineCD = nLineCD;
			_nDefectNO = nDefectNO;
			_sDefectNM = sDefectNM;
			_sLotNO = sLotNO;
			_sType = sType;
			_sResult = sResult;
			_sInspectionNMAdd = sInspectionNM;
			_sInspectionNM = _sInspectionNMAdd;

			_dtMeasure = dtMeasure;

			_sEquiNOAdd = sEquiNO;//「***号機(S*****)」表記
			int npos = sEquiNO.IndexOf("(");
			_sEquiNO = _sEquiNOAdd.Substring(npos + 1, _sEquiNOAdd.Length - npos - 2);//「S*****」表記

			_sAssetsNM = sAssetsNM;
			_nBackNum = nBackNum;
			_nMultiNO = nMultiNO;
			_nProcessNO = nProcessNO;
			_TimingNM = timingNM;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="nLineCD"></param>
		/// <param name="nDefectNO">不具合No</param>
		/// <param name="sLotNO">異常のあったLotNO</param>
		/// <param name="sType">異常のあったType</param>
		/// <param name="sResult"></param>
		/// <param name="sInspectionNM"></param>
		/// <param name="dtMeasure"></param>
		/// <param name="sEquiNO"></param>
		/// <param name="sAssetsNM"></param>
		/// <param name="nBackNum">異常のあったLotNOから遡るデータ数</param>
		/// <param name="nMultiNO"></param>
		/// <param name="nProcessNO"></param>
		/// <param name="IsOnlyEqui">装置限定フラグ</param>
		/// <param name="OnlyEqui_BeforAfterLotCT">異常ロットの前後ロット数（片側分）</param>
		/// <param name="OnlyEqui_EquiNO">装置限定時のロット番号</param>
		/// <param name="OnlyEqui_EquiNO">装置限定時の装置番号</param>
		public F03_TrendChart(int nLineCD, int nDefectNO, string sDefectNM, string sLotNO, string sType, string sResult, string sInspectionNM, DateTime dtMeasure, string sEquiNO, string sAssetsNM,
			int nBackNum, int nMultiNO, int nProcessNO, bool IsOnlyEqui, int OnlyEqui_BeforAfterLotCT, string OnlyEqui_LotNO, string OnlyEqui_EquiNO, string timingNM)
			: this(nLineCD, nDefectNO, sDefectNM, sLotNO, sType, sResult, sInspectionNM, dtMeasure, sEquiNO, sAssetsNM, nBackNum, nMultiNO, nProcessNO, timingNM)
		{
			this._IsOnlyEqui_FG = IsOnlyEqui;
			this._OnlyEqui_BeforAfterLotCT = OnlyEqui_BeforAfterLotCT;
			this._OnlyEqui_LotNO = OnlyEqui_LotNO;
			this._OnlyEqui_EquiNO = OnlyEqui_EquiNO;
		}

		BackgroundWorker bwGetMaterialTbl, bwGetPlantTbl;


		private void GetMaterialFromARMS(object sender, DoWorkEventArgs e)
		{
			SETdTblMaterial_ARMS();
		}

		private void GetPlantFromARMS(object sender, DoWorkEventArgs e)
		{
			SetTblLotStb_ARMS();
		}

		private void DrawList()
		{
			getDataCompCt = 0;

			//高生産性ラインだけは、パッケージ化されていないのでNASCA本番サーバーを見に行く
			//if (Form1.fPackage == true)
			//{
#if MEASURE_TIME
			Console.WriteLine("1:" + DateTime.Now.ToLongTimeString());
#endif
			//資材グリッド用データ取得
			SETdTblMaterial_ARMS();
			//bwGetMaterialTbl.RunWorkerAsync();

#if MEASURE_TIME
				Console.WriteLine("2:" + DateTime.Now.ToLongTimeString());
#endif
			//ロット・設備グリッド用データ取得
			SetTblLotStb_ARMS();
			//bwGetPlantTbl.RunWorkerAsync();
			//}
			//else
			//{
			//    if (ListLot.Count > 0)
			//    {
			//        //資材グリッド用データ取得
			//        SETdTblMaterial();

			//        //ロット・設備グリッド用データ取得
			//        SetTblLotStb();
			//    }
			//}
#if MEASURE_TIME
				Console.WriteLine("3:" + DateTime.Now.ToLongTimeString());
#endif
			while (getDataCompCt < 2)
			{
				Thread.Sleep(100);
			}

			//ロット・設備グリッド表示
			ViewGrdLotStb();
#if MEASURE_TIME
			Console.WriteLine("4:" + DateTime.Now.ToLongTimeString());
#endif
			ListLot.Clear();
		}

		/// <summary>
		/// 資材グリッド表示用テーブルの取得(ARMS)
		/// </summary>
		/// <param name="sWhereSql"></param>
		private void SETdTblMaterial_ARMS()
		{
			dsMaterial.dTblMaterial.Clear();

			List<MaterialStbInfo> matStbList = new List<MaterialStbInfo>();

			foreach (string lotNO in ListLot)
			{

#if MEASURE_TIME
				DateTime baseTime = DateTime.Now;
#endif
				//資材取得-----------------------------------------
				List<MaterialStbInfo> matStbMainList = ConnectARMS.GetTblMaterialStb_ARMS(lotNO);
				matStbList.AddRange(matStbMainList);
#if MEASURE_TIME
				Console.WriteLine("資材取得完(ms) / " + (DateTime.Now-baseTime).TotalMilliseconds);
				baseTime = DateTime.Now;
#endif
				//資材(搭載機Stocker1)の取得-----------------------
				List<MaterialStbInfo> matStbStock1List = ConnectARMS.GetTblMaterialStb_ARMS(lotNO, "stocker1");
				matStbList.AddRange(matStbStock1List);
#if MEASURE_TIME
				Console.WriteLine("搭載機Stocker1完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
				baseTime = DateTime.Now;
#endif
				//資材(搭載機Stocker2)の取得-----------------------
				List<MaterialStbInfo> matStbStock2List = ConnectARMS.GetTblMaterialStb_ARMS(lotNO, "stocker2");
				matStbList.AddRange(matStbStock2List);
#if MEASURE_TIME
				Console.WriteLine("搭載機Stocker2完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
				baseTime = DateTime.Now;
#endif
				//資材(DBウェハー)の取得---------------------------
				int stockerStartID = 0; int stockerEndID = 0;
				//ストッカーの使用開始と終了番号を取得
				ConnectARMS.GetMaterialStockerStartEnd(lotNO, ref stockerStartID, ref stockerEndID);
				if (stockerStartID != -1 && stockerEndID != -1)
				{
					//ストッカーを2つ以上使用している場合、始めのストッカーはMAXNOまで資材を足しこむ
					if (ConnectARMS.GetMaterialStockerMultiFG(lotNO))
					{
						stockerEndID = ConnectARMS.GetMaterialStockerMax(lotNO);
					}

					for (int i = stockerStartID; i <= stockerEndID; i++)
					{
						MaterialStbInfo matStbInfo = ConnectARMS.GetTblMaterialStb_ARMS(lotNO, i);
						if (matStbInfo.LotNO != null && matStbInfo.MaterialCD != null)
						{
							matStbList.Add(matStbInfo);
						}
					}
				}
#if MEASURE_TIME
				Console.WriteLine("DBウェハ完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
				baseTime = DateTime.Now;
#endif
				//資材(樹脂)の取得---------------------------------
				List<MaterialStbInfo> matStbStock3List = ConnectARMS.GetTblMaterialResinStb_ARMS(lotNO);
				if (_IsNascaConnectFG)
				{
					//NASCAに接続できる場合、調合材料を取得する
					matStbStock3List = GetResinData(matStbStock3List);
				}
				matStbList.AddRange(matStbStock3List);
#if MEASURE_TIME
				Console.WriteLine("樹脂完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
				baseTime = DateTime.Now;
#endif

			}


			//NASCAに接続できる場合、品目グループを取得する
			if (_IsNascaConnectFG)
			{
				GetMateGroup(ref matStbList);
			}
			else
			{
				dgvNascaList.Columns["MateGroup_JA"].Visible = false;
			}
			//#if MEASURE_TIME
			//                Console.WriteLine("品目Grp完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			//                baseTime = DateTime.Now;
			//#endif
			//交換日時を取得
			GetChangeTiming_ARMS(ref matStbList);

			//#if MEASURE_TIME
			//                Console.WriteLine("交換日時完(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			//                baseTime = DateTime.Now;
			//                int loopct = 0;
			//#endif
			foreach (MaterialStbInfo matStbInfo in matStbList)
			{
				DataRow dr = dsMaterial.dTblMaterial.NewRow();
				dr["Lot_NO"] = matStbInfo.CLotNO;
				dr["MateGroup_JA"] = matStbInfo.MateGroupNM;
				dr["mtralitem_cd"] = matStbInfo.MaterialCD;
				dr["material_ja"] = matStbInfo.MaterialNM;
				dr["MateLot_NO"] = matStbInfo.LotNO;
				dr["MixResult_ID"] = matStbInfo.MixResultID;
				//dr["Material_CD"] = matStbInfo.CMaterialCD;
				dr["material_cd2"] = matStbInfo.CMaterialCD;
				dr["complt_dt"] = matStbInfo.CompleteDT;
				dr["MateChange_DT"] = matStbInfo.ChangeDT;
				dr["Plant_CD"] = matStbInfo.PlantCD;
				dsMaterial.dTblMaterial.Rows.Add(dr);
				//#if MEASURE_TIME
				//                    loopct++;
				//#endif
			}
			//#if MEASURE_TIME
			//                Console.WriteLine("最後のforeach完loop数:" + loopct + "(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			//                baseTime = DateTime.Now;
			//#endif
			getDataCompCt++;
		}

		/// <summary>
		/// ロット・設備グリッド表示用テーブルの取得(ARMS)
		/// </summary>
		private void SetTblLotStb_ARMS()
		{
			ArrayList arlData = new ArrayList();



			foreach (string lotNO in ListLot)
			{

				//ロットから設備情報を取得
				List<LotStbInfo> dicStbList = ConnectARMS.SetTblLotStb_ARMS(lotNO);


				if (dicStbList.Count == 0)
				{
					continue;
				}

				//列情報[設備分類]のリストを作成
				foreach (LotStbInfo lotStdInfo in dicStbList)
				{
					if (!this._dicStb.ContainsKey(lotStdInfo.PlantClasNM))
					{
						this._dicStb.Add(lotStdInfo.PlantClasNM, lotStdInfo.PlantClasNM);
					}

				}

				arlData.Add(dicStbList);
			}


			//列情報を作成する
			this._TblLotStb = new DataTable();
			this._TblLotStb.Columns.Add(new DataColumn("Lot_NO", typeof(String)));
			foreach (string dicStbKey in _dicStb.Keys)
			{
				this._TblLotStb.Columns.Add(new DataColumn(dicStbKey, typeof(String)));
				this._TblLotStb.Columns[dicStbKey].Caption = _dicStb[dicStbKey];
				this._TblLotStb.Columns.Add(new DataColumn(dicStbKey + "_CD", typeof(String)));
			}

			//一致する列に設備情報を出力する
			foreach (List<LotStbInfo> lotStdList in arlData)
			{
				DataRow dr = this._TblLotStb.NewRow();
				foreach (LotStbInfo lotStdInfo in lotStdList)
				{
					dr["Lot_NO"] = lotStdInfo.LotNO;
					dr[lotStdInfo.PlantClasNM] = lotStdInfo.PlantNM;
					dr[lotStdInfo.PlantClasNM + "_CD"] = lotStdInfo.PlantCD;
				}
				this._TblLotStb.Rows.Add(dr);
			}

			getDataCompCt++;
		}

		private void ViewGrdLotStb()
		{
			this.grdLotStb.DataSource = null;
			this.grdLotStb.DataSource = this._TblLotStb;

			this.grdLotStb.Columns["Lot_NO"].HeaderText = "ロット番号";
			this.grdLotStb.Columns["Lot_NO"].ReadOnly = true;
			this.grdLotStb.Columns["Lot_NO"].Frozen = true;

			foreach (string aKey in this._dicStb.Keys)
			{
				this.grdLotStb.Columns[aKey].HeaderText = this._dicStb[aKey];
				this.grdLotStb.Columns[aKey].ReadOnly = true;
				this.grdLotStb.Columns[aKey + "_CD"].HeaderText = this._dicStb[aKey] + "_CD";
				this.grdLotStb.Columns[aKey + "_CD"].ReadOnly = true;
			}
		}

		/// <summary>
		/// 交換日時を取得(ARMS)
		/// </summary>
		/// <param name="matStbList"></param>
		private void GetChangeTiming_ARMS(ref List<MaterialStbInfo> matStbList)
		{
			List<MaterialStbInfo> registeredInfo = new List<MaterialStbInfo>();

			for (int i = 0; i < matStbList.Count; i++)
			{
				if (matStbList[i].ChangeDT == null)
				{

					matStbList[i].ChangeDT = ConnectARMS.GetChangeTimingDT(matStbList[i]);
					registeredInfo.Add(matStbList[i]);
				}

			}
		}

		/// <summary>
		/// 品目グループ
		/// </summary>
		private void GetMateGroup(ref List<MaterialStbInfo> matStbList)
		{
			for (int i = 0; i < matStbList.Count; i++)
			{
				//MaterialGroupCDかMateGroupNMの何れか1つでも空白の時だけ情報取得する
				if (string.IsNullOrEmpty(matStbList[i].MateGroupCD) || string.IsNullOrEmpty(matStbList[i].MateGroupNM))
				{
					MateGroupInfo mateGroupInfo = ConnectNASCA.GetMateGroup(matStbList[i].MaterialCD);
					matStbList[i].MateGroupCD = mateGroupInfo.MateGroupCD;
					matStbList[i].MateGroupNM = mateGroupInfo.MateGroupNM;
				}
#if MEASURE_TIME
				DateTime baseTime = DateTime.Now;
#endif
				for (int j = i + 1; j < matStbList.Count; j++)
				{
					if ((matStbList[i].MaterialCD == matStbList[j].MaterialCD) && (matStbList[i].MaterialNM == matStbList[j].MaterialNM))
					{
						matStbList[j].MateGroupCD = matStbList[i].MateGroupCD;
						matStbList[j].MateGroupNM = matStbList[i].MateGroupNM;
					}
				}
#if MEASURE_TIME
				Console.WriteLine("品目グループコピーループ(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
#endif

			}
		}

		/// <summary>
		/// 樹脂情報の取得
		/// </summary>
		/// <param name="matStbStockList"></param>
		/// <returns></returns>
		private List<MaterialStbInfo> GetResinData(List<MaterialStbInfo> matStbStockList)
		{
			List<MaterialStbInfo> materialStbList = new List<MaterialStbInfo>();

			foreach (MaterialStbInfo materialStbInfo in matStbStockList)
			{
				List<MaterialStbInfo> resultList = ConnectNASCA.GetResinData(materialStbInfo.MixResultID);
				foreach (MaterialStbInfo resultInfo in resultList)
				{
					resultInfo.CMaterialCD = materialStbInfo.CMaterialCD;
					resultInfo.CLotNO = materialStbInfo.CLotNO;
					resultInfo.CompleteDT = materialStbInfo.CompleteDT;
					resultInfo.PlantCD = materialStbInfo.PlantCD;

					materialStbList.Add(resultInfo);
				}
			}

			return materialStbList;
		}


		private void MyInitializeComponent(int nLineCD, int nDefectNO, string sLotNO, string sType, string sResult, string sInspectionNM, DateTime dtMeasure, string sEquiNO, string sAssetsNM, int nBackNum, int nMultiNO, int nProcessNO, string timingNM)
		{
			if (Constant.typeGroup != Constant.TypeGroup.MAP)
			{
				this.Text += "[監視項目] : " + ListGraphData_One[0].Defect;
			}

			//nLineCD
			SetCmbbLineNo();
			cmbbLineNo.Text = Convert.ToString(nLineCD).Trim();

			//nDefectNO
			lblDefectNO.Text = Convert.ToString(nDefectNO).Trim();

			//sLotNO
			lblLotNO.Text = sLotNO;

			//sType
			SetCmbbTypeNM();
			cmbbType.Text = sType;

			//sResult
			lblresult.Text = sResult;

			//dtMeasure
			txtbEndYear.Text = dtMeasure.AddMinutes(1).Year.ToString();
			txtbEndMonth.Text = dtMeasure.AddMinutes(1).Month.ToString();
			txtbEndDay.Text = dtMeasure.AddMinutes(1).Day.ToString();
			txtbEndHour.Text = dtMeasure.AddMinutes(1).Hour.ToString();
			txtbEndMinute.Text = dtMeasure.AddMinutes(1).Minute.ToString();

			txtbStartYear.Text = dtMeasure.AddDays(-1).Year.ToString();
			txtbStartMonth.Text = dtMeasure.AddDays(-1).Month.ToString();
			txtbStartDay.Text = dtMeasure.AddDays(-1).Day.ToString();
			txtbStartHour.Text = dtMeasure.AddDays(-1).Hour.ToString();
			txtbStartMinute.Text = dtMeasure.AddDays(-1).Minute.ToString();


			//sAssetsNM
			//if (Constant.fMap)
			//{
			SetCmbbAssetsNM_Map();
			//}
			//else
			//{
			//    SetCmbbAssetsNM();
			//}
			cmbbAssetsNM.Text = timingNM;
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				SetCmbbQcParamNM_Map(sAssetsNM);
			}
			else
			{
				SetCmbbQcParamNM(sAssetsNM);
			}

			cmbbQcParamNM.Text = sInspectionNM.Trim();

			List<string> ListEqui = GetSameAssets(nLineCD, sAssetsNM);
			string sEquipmentNO = "";

			if (nMultiNO == 0)
			{
				clbMachineList.Items.Clear();
				for (int i = 0; i < ListEqui.Count; i++)
				{
					sEquipmentNO = ListEqui[i].Trim();
					sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);

					if (ListEqui[i].Trim() == sEquiNO)
					{
						clbMachineList.Items.Add(sEquipmentNO, CheckState.Checked);//CheckON
					}
					else
					{
						clbMachineList.Items.Add(sEquipmentNO, CheckState.Unchecked);//CheckOFF
					}
				}
			}
			else if (nMultiNO == 1)
			{
				clbMachineList.Items.Clear();
				for (int i = 0; i < ListEqui.Count; i++)
				{
					sEquipmentNO = ListEqui[i].Trim();
					sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);
					clbMachineList.Items.Add(sEquipmentNO, CheckState.Checked);//全てCheckON
				}
			}

			//監視不具合入力
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				cmbbDefectNM.Visible = false;//MAPでは監視項目という概念なし
				btnNext.Enabled = false;
				btnBack.Enabled = false;
			}
			else
			{
				SetCmbbDefectNM();
				cmbbDefectNM.Text = _sDefectNM;
			}

			txtbInspectionNO.Text = Convert.ToString(ListGraphData_One[0].InspectionNO);

			txtbProcessNO.Text = Convert.ToString(GetProcessNO());

			nArrartProcessNO = ListGraphData_One[0].InspectionNO;

			btnBack.Enabled = false;

			if (this._IsNascaConnectFG)
			{
				if (Constant.typeGroup != Constant.TypeGroup.MAP)
				{
					//マルチスレッド処理
					//Action act = new Action(長い処理);
					//act.BeginInvoke(new AsyncCallback(Callback), null);
				}
			}
		}

		public void SetText(string txt)
		{
			if (this.InvokeRequired == true)
			{
				this.Invoke(new Action<string>(s => SetText(s)), txt);
			}
			else
			{
				btnAllGraph.Text = txt;
			}
		}

		/// <summary>
		/// 終了時に呼ばれるメソッド/* フォルダ毎に用意されているフラグをOFFにする*/
		/// </summary>
		/// <param name="ar"></param>
		private void Callback(IAsyncResult ar)
		{
			SetText("関連グラフ");

			if (this.InvokeRequired == true)
			{
				this.Invoke(new Action(() => this.btnAllGraph.Enabled = true));
			}
			else
			{
				this.btnAllGraph.Enabled = true;
			}
		}

		private void btnAllGraph_Click(object sender, EventArgs e)
		{
			frmDrawGraph.Visible = true;
			frmDrawGraph.Show();
		}

		private string GetProcessNO()
		{
			string sProcessNO = "";
			int nInspectionNO = -1;

			if (int.TryParse(txtbInspectionNO.Text, out nInspectionNO) == false)
			{
				return sProcessNO;
			}

			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT Process_NO FROM [TmQsub] WITH(NOLOCK) Where Inspection_NO={0}";

			string sqlCmdTxt = string.Format(BaseSql, nInspectionNO);

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
						sProcessNO = Convert.ToString(reader["Process_NO"]);
						i = i + 1;
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			return sProcessNO;
		}

		private int GetQcParamNO(string sInspectionNM)
		{
			int nQcParamNO = 0;

			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT Inspection_NO FROM [TmQins] WITH(NOLOCK) Where Inspection_NM='{0}'";

			string sqlCmdTxt = string.Format(BaseSql, sInspectionNM.Replace("'", "''"));

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
						nQcParamNO = Convert.ToInt32(reader["Inspection_NO"]);
						i = i + 1;
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			return nQcParamNO;
		}

		private string SettxtbQcParamNM()
		{
			string sQcParamNM = "該当なし";
			int nQcParamNO = 0;

			if (int.TryParse(txtbInspectionNO.Text, out nQcParamNO) == false)
			{
				return sQcParamNM;
			}

			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT Inspection_NM FROM [TmQsub] WITH(NOLOCK) Where Inspection_NO={0}";

			string sqlCmdTxt = string.Format(BaseSql, nQcParamNO);

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
						string sInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
						if (sInspectionNM.Substring(0, 1) == "F")
						{
							sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「F*****(不具合A)」の表記に変更
						}
						sQcParamNM = sInspectionNM;
						i = i + 1;
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			return sQcParamNM;
		}

		#region コンボボックスの設定

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

		private void SetCmbbAssetsNM_Map()
		{
			//閾値マスタにあるInline_CDのみ表示
			//string sqlCmdTxt = "Select DISTINCT Timing_NM From TmQtim WITH(NOLOCK) ORDER BY Timing_NM  ASC";
			string sqlCmdTxt = "Select DISTINCT Timing_NM From TmQtim WITH(NOLOCK)";
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
						this.cmbbAssetsNM.Items.Insert(i, Convert.ToString(reader["Timing_NM"]).Trim());
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

		private void SetCmbbQcParamNM_Map(string sAssetsNM)
		{
			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT DISTINCT Inspection_NM FROM [TvQDIW_Map]  WITH(NOLOCK) WHERE Timing_NM Like '%{0}%'";
			string sqlCmdTxt = string.Format(BaseSql, sAssetsNM);
			string sInspectionNM = "";
			this.cmbbQcParamNM.Items.Clear();
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
						sInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
						if (sInspectionNM.Substring(0, 1) == "F")
						{
							sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM).Trim();//「F*****」→「不具合A(F*****)」の表記に変更
						}
						this.cmbbQcParamNM.Items.Insert(i, sInspectionNM);
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

		private void SetCmbbQcParamNM(string sAssetsNM)
		{
			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT DISTINCT Inspection_NM FROM [TvQDIW]  WITH(NOLOCK) WHERE Timing_NM='{0}'";
			string sqlCmdTxt = string.Format(BaseSql, sAssetsNM);
			string sInspectionNM = "";
			this.cmbbQcParamNM.Items.Clear();
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
						sInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
						if (sInspectionNM.Substring(0, 1) == "F")
						{
							sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM).Trim();//「F*****」→「不具合A(F*****)」の表記に変更
						}
						this.cmbbQcParamNM.Items.Insert(i, sInspectionNM);
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

		private void SetCmbbLineNo()
		{
			//閾値マスタにあるInline_CDのみ表示
			this.cmbbLineNo.Items.Clear();
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

		private void SetCmbbDefectNM()
		{
			//閾値マスタにあるTypeのみ表示
			string sqlCmdTxt = "SELECT  [Defect_NM] FROM [TmQdfc] WITH(NOLOCK) ORDER BY [Defect_NO]  ASC";
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
						this.cmbbDefectNM.Items.Insert(i, Convert.ToString(reader["Defect_NM"]).Trim());
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

		private void SetCmbbTypeNM()
		{
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
						this.cmbbType.Items.Insert(i, Convert.ToString(reader["Material_CD"]).Trim());
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

		#endregion

		List<string> GetSameAssets(int nLineCD, string sAssetsNM)
		{
			List<string> wList = new List<string>();

			//if (sAssetsNM.Contains("ﾀﾞｲﾎﾞﾝﾀﾞｰKE205"))
			//{
			//}
			if (sAssetsNM.Contains("ﾀﾞｲﾎﾞﾝﾀﾞｰ"))
			{
				sAssetsNM = "ﾀﾞｲﾎﾞﾝﾀﾞｰ";
			}
			else if (sAssetsNM.Contains("ﾌﾟﾗｽﾞﾏ"))
			{
				sAssetsNM = "ﾌﾟﾗｽﾞﾏ";
			}
			// ﾊｰﾄﾞｺｰﾃﾞｨﾝｸﾞはﾏｽﾞｲが解決の時間が無いので、急場しのぎの為、やむを得ずﾊｰﾄﾞｺｰﾃﾞｨﾝｸﾞ追記(2015/11/18 nyoshimoto)
			else if (sAssetsNM.Contains("ﾌﾘｯﾌﾟﾁｯﾌﾟ"))
			{
				sAssetsNM = "ﾌﾘｯﾌﾟﾁｯﾌﾟﾎﾞﾝﾀﾞｰ";
			}
			else if (sAssetsNM.Contains("外観検査機(ﾀﾞｲｽｸﾗｯｸ)"))
			{
				sAssetsNM = "外観検査機JType";
			}
			else if (sAssetsNM.Contains("電着"))
			{
				sAssetsNM = "電着";
			}

			string BaseSql = "SELECT Equipment_NO FROM TvLSET WITH(NOLOCK) Where Inline_CD={0} ";

			if (sAssetsNM.Contains("ﾓｰﾙﾄﾞ機") || sAssetsNM.Contains("ｲﾝﾅｰMD") || sAssetsNM.Contains("樹脂枠"))
			{
				BaseSql += " AND ( Assets_NM='ﾓｰﾙﾄﾞ機' OR Assets_NM='ｲﾝﾅｰﾓｰﾙﾄﾞ機MDM20' OR Assets_NM='ｲﾝﾅｰﾓｰﾙﾄﾞ機MDM50' OR Assets_NM = 'ﾓｰﾙﾄﾞ外製' OR Assets_NM='ﾓｰﾙﾄﾞ機_Y' ) ";
			}
			else if (sAssetsNM.Contains("圧縮成型"))
			{
				BaseSql += " AND ( Assets_NM='圧縮成形（セミオート）' OR Assets_NM='圧縮成形（フルオート）' )";
			}
			else if (sAssetsNM.Contains("外観検査機"))
			{
				BaseSql += " AND ( Assets_NM='外観検査機' OR Assets_NM='外観検査機JType' OR Assets_NM='外観検査機NType' OR Assets_NM='外観検査機RAIM' )";
			}
			else if (sAssetsNM.Contains("反射材ﾎﾟｯﾃｨﾝｸﾞ"))
			{
				BaseSql += " AND ( Assets_NM='反射材ﾎﾟｯﾃｨﾝｸﾞQUSPA' OR Assets_NM='反射材ﾎﾟｯﾃｨﾝｸﾞ内製' OR Assets_NM='外観検査機NType' OR Assets_NM='外観検査機RAIM' )";
			}
			else if (sAssetsNM.Contains("ﾊﾞﾝﾌﾟﾎﾞﾝﾄﾞ"))
			{
				BaseSql += " AND ( Assets_NM='ﾊﾞﾝﾌﾟﾎﾞﾝﾀﾞｰ' )";
			}
			else
			{
				BaseSql += " AND Assets_NM like '{1}%' ";
			}

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
						wList.Add(Convert.ToString(reader["Equipment_NO"]).Trim());  //設備番号
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

		private SortedList<int, GraphData> GETGraphInfo_Map(string sInspectionNM)
		{
			int nCnt = 0;
			string[] textArray = new string[] { };
			string sWork = "";
			if (sInspectionNM.Substring(0, 1) == "F")
			{
				sInspectionNM = sInspectionNM.Substring(0, 5);
			}
			SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

			string BaseSql = "Select Inspection_NO,Timing_NM,Inspection_NM,Process_NO From TvQDIW_Map WITH(NOLOCK) Where Inspection_NM='{0}'";
			string sqlCmdTxt = string.Format(BaseSql, sInspectionNM.Replace("'", "''"));

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();
					while (reader.Read())
					{
						GraphData wGraphData = new GraphData();                               //こっちが正解
						wGraphData.Defect = "";
						wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
						wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));
						wGraphData.InspectionNM = _sInspectionNMAdd;
						sWork = Convert.ToString(reader["Process_NO"]);
						textArray = sWork.Split(',');
						for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
						{
							wGraphData.Process.Add(Convert.ToInt32(textArray[i]));
						}

						wList.Add(nCnt, wGraphData);
						nCnt = nCnt + 1;
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

		private SortedList<int, GraphData> GETGraphInfo(int nDefectNO, string sInspectionNM)
		{
			int nCnt = 0;
			string[] textArray = new string[] { };
			string sWork = "";
			if (sInspectionNM.Substring(0, 1) == "F")
			{
				sInspectionNM = sInspectionNM.Substring(0, 10);
			}
			SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

			string BaseSql = "Select Defect_NM,Inspection_NO,Timing_NM,Inspection_NM,Process_NO From TvQDIW WITH(NOLOCK) Where Defect_NO={0} AND Inspection_NM='{1}'";
			string sqlCmdTxt = string.Format(BaseSql, nDefectNO, sInspectionNM.Replace("'", "''"));

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();
					while (reader.Read())
					{
						GraphData wGraphData = new GraphData();                               //こっちが正解
						wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
						wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
						wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));
						wGraphData.InspectionNM = _sInspectionNMAdd;
						sWork = Convert.ToString(reader["Process_NO"]);
						textArray = sWork.Split(',');
						for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
						{
							wGraphData.Process.Add(Convert.ToInt32(textArray[i]));
						}

						wList.Add(nCnt, wGraphData);
						nCnt = nCnt + 1;
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

		private SortedList<int, GraphData> GETGraphInfo(int nDefectNO, int nWatchNO)
		{
			int nCnt = 0;
			string[] textArray = new string[] { };
			string sWork = "";

			SortedList<int, GraphData> wList = new SortedList<int, GraphData>();

			string BaseSql = "Select Defect_NM,Timing_NM,Inspection_NO,Inspection_NM,Process_NO From TvQDIW WITH(NOLOCK) Where Watch_NO={0} AND Defect_NO={1} ORDER BY Process_NO";
			string sqlCmdTxt = string.Format(BaseSql, nWatchNO, nDefectNO);

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();
					while (reader.Read())
					{
						GraphData wGraphData = new GraphData();                               //こっちが正解
						wGraphData.Defect = (Convert.ToString(reader["Defect_NM"]));
						wGraphData.Timing = (Convert.ToString(reader["Timing_NM"]));
						wGraphData.InspectionNO = (Convert.ToInt32(reader["Inspection_NO"]));
						wGraphData.InspectionNM = (Convert.ToString(reader["Inspection_NM"]));
						sWork = Convert.ToString(reader["Process_NO"]);
						textArray = sWork.Split(',');
						for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
						{
							int processNo;
							if (int.TryParse(textArray[i], out processNo) == false)
							{
								throw new ApplicationException(string.Format(
									"TvQDIWから取得したProcess_NOが数値変換できません。 InspectionNO:{0} 変換対象:{1}", wGraphData.InspectionNO, processNo));
							}

							wGraphData.Process.Add(processNo);
						}

						wList.Add(nCnt, wGraphData);
						nCnt = nCnt + 1;
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

		private void DrawGraph(List<SortedList<int, QCLogData>> cndDataItem, List<string> ListLot, int nMode)
		{
#if MEASURE_TIME
			Console.WriteLine("DrawGraph()開始 / " + DateTime.Now.ToLongTimeString());
#endif
			int nLineNo = Convert.ToInt32(cmbbLineNo.Text);


			string[] textArray = new string[] { };
			string sWork = "";

			List<int> Process = new List<int>();
			sWork = txtbProcessNO.Text;//暫定動作確認用
			textArray = sWork.Split(',');//暫定動作確認用
			for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
			{
				Process.Add(Convert.ToInt32(textArray[i]));
			}

			int nProcessNO = Convert.ToInt32(textArray[1]);

			if (this._IsOnlyEqui_FG)        //装置限定モードのとき
			{
				cndDataItem.Add(GetQCItem_OnlyEqui());
			}
			else                            //装置限定モード以外のとき
			{
				cndDataItem.Add(GetQCItem());
			}

			if (cndDataItem.Count > 0)
			{
				if (cndDataItem[0].Count > 0)
				{
					p.DrawGraph(chart1, cndDataItem[0], ListLot, nMode);
					//描画されているLot一覧取得
					ListLot.Clear();
					for (int i = 0; i < cndDataItem[0].Count; i++)
					{
						string tmpStr = cndDataItem[0].Values[i].LotNO;
						if (ListLot.Contains(tmpStr) == false)
						{
							ListLot.Add(tmpStr);
						}
					}
				}
				else
				{
					chart1.Series.Clear();
					chart1.Titles.Clear();
					chart1.Titles.Add("No Image.");

					ListLot.Clear();
				}
			}
			else
			{
				chart1.Series.Clear();
				chart1.Titles.Clear();
				chart1.Titles.Add("No Image.");

				ListLot.Clear();
			}
			cndDataItem.Clear();
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
			}
			sWhereSQL = sWhereSQL.Replace("@", " AND (");
			sWhereSQL = sWhereSQL + ")";
			sWhereSQL = sWhereSQL.Replace("OR )", ")");

			return sWhereSQL;
		}

		private SortedList<int, QCLogData> GetQCItem()
		{
			int nCnt = 0;

			string[] textArray = new string[] { };
			string sWork = "";
			string WhereSQL = "@";
			string sPrmNM = "";
			SortedList<int, string> Qcprmdt = new SortedList<int, string>();//例：<105,ｼﾘﾝｼﾞ1ﾊﾞｷｭｰﾑ圧>

			sWork = txtbProcessNO.Text;
			textArray = sWork.Split(',');

			for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
			{
				WhereSQL = WhereSQL + "QcParam_NO=" + textArray[i] + " OR ";
				sPrmNM = Com.GetTmPRM(Convert.ToInt32(textArray[i]));
				Qcprmdt.Add(Convert.ToInt32(textArray[i]), sPrmNM);
			}

			WhereSQL = WhereSQL.Replace("@", " AND (") + ")";
			WhereSQL = WhereSQL.Replace("OR )", ")");

			DateTime dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
			DateTime dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

			SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
			string sWhereSQLEqui = GetWhereSQLEqui();

			string BaseSql = " SELECT NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT ,QcParam_NO FROM [TnLOG] WITH(NOLOCK) " +
							 " WHERE Inline_CD ={0} AND (Measure_DT>='{1}' AND Measure_DT<='{2}') AND Material_CD='{3}' AND NascaLot_No <> ''" + sWhereSQLEqui + WhereSQL +
							 " ORDER BY Measure_DT";
			string sqlCmdTxt = string.Format(BaseSql, cmbbLineNo.Text, dtStart, dtEnd, cmbbType.Text.Trim());

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						QCLogData wQCLogData = new QCLogData();
						wQCLogData.EquiNO = Convert.ToString(reader["Equipment_NO"]).Trim();  //設備番号
						wQCLogData.LotNO = Convert.ToString(reader["NascaLot_NO"]).Trim();    //Lot
						wQCLogData.TypeCD = cmbbType.Text.Trim();                             //Type
						wQCLogData.MeasureDT = Convert.ToDateTime(reader["Measure_DT"]);      //計測日時
						wQCLogData.Data = Convert.ToDouble(reader["DParameter_VAL"]);         //data
						wQCLogData.Defect = Convert.ToInt32(lblDefectNO.Text);                //監視項目No
						wQCLogData.QcprmNO = Convert.ToInt32(reader["QcParam_NO"]);           //管理番号
						for (int i = 0; i < Qcprmdt.Count; i++)
						{
							if (wQCLogData.QcprmNO == Qcprmdt.Keys[i])
							{
								wQCLogData.QcprmNM = Qcprmdt.Values[i];
							}
						}
						cndDataItem.Add(nCnt, wQCLogData);
						nCnt = nCnt + 1;

					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			return cndDataItem;
		}

		private SortedList<int, QCLogData> GetQCItem_OnlyEqui()
		{

			SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
			List<QCLogData> tmpQCLogDataList = new List<QCLogData>();

			string sqlCmdTxt = ""
			+ " SELECT \r\n"
			+ "     TOP " + Convert.ToString(this._OnlyEqui_BeforAfterLotCT + 1) + " NascaLot_NO, \r\n"
			+ "     DParameter_VAL, \r\n"
			+ "     Equipment_NO, \r\n"
			+ "     Measure_DT, \r\n"
			+ "     QcParam_NO \r\n"
			+ " FROM \r\n"
			+ "     TnLOG \r\n"
			+ " WHERE \r\n"
			+ "     NascaLot_NO <> '' AND \r\n"
			+ "     EXISTS \r\n"
			+ "         (SELECT \r\n"
			+ "             Equipment_NO, \r\n"
			+ "             Measure_DT, \r\n"
			+ "             QcParam_NO, \r\n"
			+ "             Inline_CD, \r\n"
			+ "             Material_CD \r\n"
			+ "         FROM \r\n"
			+ "             TnLOG TnLog2 \r\n"
			+ "         WHERE \r\n"
			+ "             Inline_CD = @INLINECD AND \r\n"
			+ "             QcParam_NO = @QCPARAMNO AND \r\n"
			+ "             Material_CD = @MATERIALCD AND \r\n"
			+ "             NascaLot_NO = @NASCALOTNO AND \r\n"
			+ "             Equipment_NO = @EQUIPMENTNO AND \r\n"
			+ "             TnLOG.Inline_CD = Inline_CD AND \r\n"
			+ "             TnLOG.QcParam_NO = QcParam_NO AND \r\n"
			+ "             TnLOG.Material_CD = Material_CD AND \r\n"
			+ "             TnLOG.Equipment_NO = Equipment_NO AND \r\n"
			+ "             TnLOG.Measure_DT <= Measure_DT \r\n"
			+ "         ) \r\n"
			+ " ORDER BY \r\n"
			+ "     Measure_DT \r\n";

			string sqlCmdTxt2 = ""
			+ " SELECT \r\n"
			+ "     TOP " + Convert.ToString(this._OnlyEqui_BeforAfterLotCT + 1) + " NascaLot_NO, \r\n"
			+ "     DParameter_VAL, \r\n"
			+ "     Equipment_NO, \r\n"
			+ "     Measure_DT, \r\n"
			+ "     QcParam_NO \r\n"
			+ " FROM \r\n"
			+ "     TnLOG \r\n"
			+ " WHERE \r\n"
			+ "     NascaLot_NO <> '' AND \r\n"
			+ "     EXISTS \r\n"
			+ "         (SELECT \r\n"
			+ "             Equipment_NO, \r\n"
			+ "             Measure_DT, \r\n"
			+ "             QcParam_NO, \r\n"
			+ "             Inline_CD, \r\n"
			+ "             Material_CD \r\n"
			+ "         FROM \r\n"
			+ "             TnLOG TnLog2 \r\n"
			+ "         WHERE \r\n"
			+ "             Inline_CD = @INLINECD AND \r\n"
			+ "             QcParam_NO = @QCPARAMNO AND \r\n"
			+ "             Material_CD = @MATERIALCD AND \r\n"
			+ "             NascaLot_NO = @NASCALOTNO AND \r\n"
			+ "             Equipment_NO = @EQUIPMENTNO AND \r\n"
			+ "             TnLOG.Inline_CD = Inline_CD AND \r\n"
			+ "             TnLOG.QcParam_NO = QcParam_NO AND \r\n"
			+ "             TnLOG.Material_CD = Material_CD AND \r\n"
			+ "             TnLOG.Equipment_NO = Equipment_NO AND \r\n"
			+ "             TnLOG.Measure_DT >= Measure_DT \r\n"
			+ "         ) \r\n"
			+ " ORDER BY \r\n"
			+ "     Measure_DT \r\n";


			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;

				string[] tmpProcessNO = txtbProcessNO.Text.Split(',');
				//2個以上ある場合、@INLINECDを重複して使用しているエラー発生
				for (int i = 1; i < tmpProcessNO.Length - 1; i++)
				{
					try
					{
						connect.Command.Parameters.Add("@INLINECD", SqlDbType.Int).Value = Convert.ToInt32(cmbbLineNo.Text);
						connect.Command.Parameters.Add("@QCPARAMNO", SqlDbType.Int).Value = Convert.ToInt32(tmpProcessNO[i]);
						connect.Command.Parameters.Add("@MATERIALCD", SqlDbType.Char).Value = cmbbType.Text.Trim();
						connect.Command.Parameters.Add("@NASCALOTNO", SqlDbType.VarChar).Value = this._OnlyEqui_LotNO;
						connect.Command.Parameters.Add("@EQUIPMENTNO", SqlDbType.Char).Value = this._OnlyEqui_EquiNO;
						connect.Command.CommandText = sqlCmdTxt;
						reader = connect.Command.ExecuteReader();

						while (reader.Read())
						{
							QCLogData wQCLogData = new QCLogData();                               //こっちが正解
							wQCLogData.EquiNO = Convert.ToString(reader["Equipment_NO"]).Trim();  //設備番号
							wQCLogData.LotNO = Convert.ToString(reader["NascaLot_NO"]).Trim();    //Lot
							wQCLogData.TypeCD = cmbbType.Text.Trim();                             //Type
							wQCLogData.MeasureDT = Convert.ToDateTime(reader["Measure_DT"]);      //計測日時
							wQCLogData.Data = Convert.ToDouble(reader["DParameter_VAL"]);         //data
							wQCLogData.Defect = Convert.ToInt32(lblDefectNO.Text);                //監視項目No
							wQCLogData.QcprmNO = Convert.ToInt32(reader["QcParam_NO"]);              //管理番号
							List<QCLogData> foundList = tmpQCLogDataList.FindAll((x) => { return x.LotNO == wQCLogData.LotNO; });
							if (foundList == null)
								tmpQCLogDataList.Add(wQCLogData);
							else if (foundList.Count == 0)
								tmpQCLogDataList.Add(wQCLogData);
						}
					}
					finally
					{
						if (reader != null) reader.Close();
					}
					try
					{
						connect.Command.CommandText = sqlCmdTxt2;
						reader = connect.Command.ExecuteReader();

						while (reader.Read())
						{
							QCLogData wQCLogData = new QCLogData();                               //こっちが正解
							wQCLogData.EquiNO = Convert.ToString(reader["Equipment_NO"]).Trim();  //設備番号
							wQCLogData.LotNO = Convert.ToString(reader["NascaLot_NO"]).Trim();    //Lot
							wQCLogData.TypeCD = cmbbType.Text.Trim();                             //Type
							wQCLogData.MeasureDT = Convert.ToDateTime(reader["Measure_DT"]);      //計測日時
							wQCLogData.Data = Convert.ToDouble(reader["DParameter_VAL"]);         //data
							wQCLogData.Defect = Convert.ToInt32(lblDefectNO.Text);                //監視項目No
							wQCLogData.QcprmNO = Convert.ToInt32(reader["QcParam_NO"]);              //管理番号
							List<QCLogData> foundList = tmpQCLogDataList.FindAll((x) => { return x.LotNO == wQCLogData.LotNO; });
							if (foundList == null)
								tmpQCLogDataList.Add(wQCLogData);
							else if (foundList.Count == 0)
								tmpQCLogDataList.Add(wQCLogData);
						}
					}
					finally
					{
						if (reader != null) reader.Close();
					}
				}
				tmpQCLogDataList.Sort((x, y) => { return (int)((TimeSpan)(x.MeasureDT - y.MeasureDT)).TotalSeconds; });
				for (int i = 0; i < tmpQCLogDataList.Count; i++)
				{
					cndDataItem.Add(i, tmpQCLogDataList[i].Clone());
				}
			}

			return cndDataItem;
		}

		//[再抽出]ボタン
		private void btnRedraw_Click(object sender, EventArgs e)
		{
			DrawGraph(cndDataItem_One, ListLot, 1);
			DrawList();
			this.Refresh();

		}

		//[戻る]ボタン
		private void btnBack_Click(object sender, EventArgs e)
		{
			if (nCurrentGraphNum != 1)
			{
				nCurrentGraphNum -= 1;
				btnNext.Enabled = true;
				btnBack.Enabled = true;
				if (nCurrentGraphNum == 1)
				{
					btnBack.Enabled = false;
				}
			}
			else
			{
				btnBack.Enabled = false;
			}
			//監視項目
			if (nCurrentGraphNum <= nWatchGraphNum)
			{
				SetDisp(ListGraphData_Watch, nCurrentGraphNum - 1);
			}
			else//関連項目
			{
				SetDisp(ListGraphData_Relation, nCurrentGraphNum - nWatchGraphNum - 1);
			}
			DrawGraph(cndDataItem_One, ListLot, 1);
			DrawList();
			this.Refresh();
		}

		//[進む]ボタン
		private void btnNext_Click(object sender, EventArgs e)
		{
			//if (nTotalGraphNum - nWatchGraphNum != nCurrentGraphNum)
			if (nTotalGraphNum != nCurrentGraphNum)
			{
				nCurrentGraphNum += 1;
				btnBack.Enabled = true;
				btnNext.Enabled = true;
				if (nTotalGraphNum == nCurrentGraphNum + 1)
				{
					btnNext.Enabled = false;
				}
			}
			else
			{
				btnNext.Enabled = false;
			}
			//監視項目
			if (nCurrentGraphNum <= nWatchGraphNum)
			{
				SetDisp(ListGraphData_Watch, nCurrentGraphNum - 1);
			}
			else//関連項目
			{
				SetDisp(ListGraphData_Relation, nCurrentGraphNum - nWatchGraphNum - 1);
			}
			DrawGraph(cndDataItem_One, ListLot, 1);
			DrawList();
			this.Refresh();

		}
		private void SetDisp(SortedList<int, GraphData> ListGraphData, int nNum)
		{
			int nInspectionNO;

			cmbbDefectNM.Text = ListGraphData[nNum].Defect;

			nInspectionNO = ListGraphData[nNum].InspectionNO;
			txtbInspectionNO.Text = Convert.ToString(nInspectionNO);

			cmbbAssetsNM.Text = ListGraphData[nNum].Timing;
		}

		private void cmbbAssetsNM_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<string> ListEqui = GetSameAssets(Convert.ToInt32(cmbbLineNo.Text), cmbbAssetsNM.Text);
			string sEquipmentNO = "";
			string viewParamNm;

			clbMachineList.Items.Clear();
			for (int i = 0; i < ListEqui.Count; i++)
			{
				sEquipmentNO = ListEqui[i].Trim();
				sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);

				clbMachineList.Items.Add(sEquipmentNO, CheckState.Checked);//全てCheckON
			}
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				SetCmbbQcParamNM_Map(cmbbAssetsNM.Text);
			}
			else
			{
				SetCmbbQcParamNM(cmbbAssetsNM.Text);
			}

			

			if(cmbbQcParamNM.Items.Count > 0)
			{
				viewParamNm = cmbbQcParamNM.Items[0].ToString();
			}
			else
			{
				viewParamNm = "未管理";
			}
			cmbbQcParamNM.Text = viewParamNm;
		}

		private void txtbInspectionNO_TextChanged(object sender, EventArgs e)
		{
			//txtbQcParamNM.Text = SettxtbQcParamNM();
			txtbProcessNO.Text = Convert.ToString(GetProcessNO()).Trim();
		}

		private void dgvNascaList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.ColumnIndex == -1 || e.RowIndex == -1)
			{
				return;
			}
			if (dgvNascaList.Columns[e.ColumnIndex].Name == "plantCDDataGridViewTextBoxColumn")
			{
				if (!this._IsOnlyEqui_FG)
				{
					F03_TrendChart aFrm = new F03_TrendChart(_nLineCD, _nDefectNO, _sDefectNM, _sLotNO, _sType, _sResult, _sInspectionNM, _dtMeasure, _sEquiNO, _sAssetsNM, _nBackNum, _nMultiNO, _nProcessNO,
						true, 20, Convert.ToString(dgvNascaList[0, e.RowIndex].Value).Trim(), Convert.ToString(dgvNascaList[e.ColumnIndex, e.RowIndex].Value).Trim(), _TimingNM);
					aFrm.ShowDialog();
				}
			}
			else
			{
				if (dgvNascaList.Columns[e.ColumnIndex].Name == "mateChangeDTDataGridViewTextBoxColumn")
				{
					string sChangeDT = Convert.ToString(dgvNascaList[e.ColumnIndex, e.RowIndex].Value).Trim();
					string sMateLot = Convert.ToString(dgvNascaList["mateLotNODataGridViewTextBoxColumn", e.RowIndex].Value).Trim();
					string sSearchDT = "";
					string sSearchLot = "";
					string sLotNO = "";
					ListSameLot.Clear();
					if (sChangeDT.Trim() != "")
					{
						for (int i = 0; i < dsMaterial.dTblMaterial.Count; i++)
						{
							sSearchDT = Convert.ToString(dgvNascaList[e.ColumnIndex, i].Value).Trim();
							if (sSearchDT == sChangeDT)
							{
								sSearchLot = Convert.ToString(dgvNascaList["mateLotNODataGridViewTextBoxColumn", i].Value).Trim();
								if (sSearchLot == sMateLot)
								{
									sLotNO = Convert.ToString(dgvNascaList[0, i].Value).Trim();
									//重複なき場合は、Listへ追加
									if (!ListSameLot.Contains(sLotNO))
									{
										ListSameLot.Add(sLotNO);
									}
								}
							}
						}
					}
				}
				else
				{
					string sClickNM = Convert.ToString(dgvNascaList[e.ColumnIndex, e.RowIndex].Value).Trim();
					string sSearchNM = "";
					string sLotNO = "";
					ListSameLot.Clear();

					for (int i = 0; i < dsMaterial.dTblMaterial.Count; i++)
					{
						sSearchNM = Convert.ToString(dgvNascaList[e.ColumnIndex, i].Value).Trim();
						if (sSearchNM == sClickNM)
						{
							sLotNO = Convert.ToString(dgvNascaList[0, i].Value).Trim();
							//重複なき場合は、Listへ追加
							if (ListSameLot.Contains(sLotNO) == false)
							{
								ListSameLot.Add(sLotNO);
							}
						}
					}
				}
				//該当Lotのグラフに色付けを行う
				DrawGraph(cndDataItem_One, ListSameLot, 1);

			}
		}

		//分解ボタン押下
		private void cbAnalysis_CheckedChanged(object sender, EventArgs e)
		{
			if (!_IsOnlyEqui_FG)
			{
				DrawGraph(cndDataItem_One, ListLot, (this.cbAnalysis.Checked) ? 1 : 0);
				DrawList();
				this.Refresh();
			}
		}

		/// <summary>
		/// フォームロード時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmDrawGraphAndList_Load(object sender, EventArgs e)
		{
			try
			{
				bwGetMaterialTbl = new BackgroundWorker();
				bwGetMaterialTbl.DoWork += new DoWorkEventHandler(GetMaterialFromARMS);

				bwGetPlantTbl = new BackgroundWorker();
				bwGetPlantTbl.DoWork += new DoWorkEventHandler(GetPlantFromARMS);

				//NASCAへの接続状態を確認
				_IsNascaConnectFG = ConnectNASCA.CheckConnect();

				Closed_FG = false;

				//異常項目単体取得
				if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
				{
					ListGraphData_One = GETGraphInfo_Map(_sInspectionNM);
				}
				else
				{
					ListGraphData_One = GETGraphInfo(_nDefectNO, _sInspectionNM);
				}
				MyInitializeComponent(_nLineCD, _nDefectNO, _sLotNO, _sType, _sResult, _sInspectionNMAdd, _dtMeasure, _sEquiNO, _sAssetsNM, _nBackNum, _nMultiNO, _nProcessNO, _TimingNM);

				//監視項目リスト取得
				if (Constant.typeGroup != Constant.TypeGroup.MAP)
				{
					ListGraphData_Watch = GETGraphInfo(_nDefectNO, 2);//2が監視項目の意味

					//関連項目リスト取得
					ListGraphData_Relation = GETGraphInfo(_nDefectNO, 1);//1が関連項目の意味

					nWatchGraphNum = ListGraphData_Watch.Count;
					nRelationGraphNum = ListGraphData_Relation.Count;
					nTotalGraphNum = nWatchGraphNum + nRelationGraphNum;
				}

				if (this._IsOnlyEqui_FG)
				{
					this.ListLot.Add(this._OnlyEqui_LotNO);
					this.gbEquipment.Enabled = false;
					this.gbCompornent.Enabled = false;
					this.gbGraph.Enabled = false;
					this.gbQc.Enabled = false;
					this.btnBack.Enabled = false;
					this.btnRedraw.Enabled = false;
					this.btnNext.Enabled = false;
					this.cbAnalysis.Checked = true;
					DrawGraph(cndDataItem_One, ListLot, 1);
					DrawList();
				}
				else
				{
					DrawGraph(cndDataItem_One, ListLot, 1);
					this._TargetLotList = new List<string>();
					foreach (string tmpLot in this.ListLot)
					{
						this._TargetLotList.Add(tmpLot);
					}
					DrawList();
				}
				this.Text += "    Ver." + Program.SystemVer;
				//行番号表示
				this.dgvNascaList.RowPostPaint += new DataGridViewRowPostPaintEventHandler(Common.Grd_RowPostPaint);
				this.grdLotStb.RowPostPaint += new DataGridViewRowPostPaintEventHandler(Common.Grd_RowPostPaint);
			}
			catch (ApplicationException err)
			{
				MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			catch (Exception err)
			{
				MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void grdLotStb_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.ColumnIndex == -1 || e.RowIndex == -1)
			{
				return;
			}
			DataGridView aGrd = (DataGridView)sender;
			string sClickNM = Convert.ToString(aGrd[e.ColumnIndex, e.RowIndex].Value).Trim();
			string sSearchNM = "";
			string sLotNO = "";
			ListSameLot.Clear();

			for (int i = 0; i < aGrd.Rows.Count; i++)
			{
				sSearchNM = Convert.ToString(aGrd[e.ColumnIndex, i].Value).Trim();
				if (sSearchNM == sClickNM)
				{
					sLotNO = Convert.ToString(aGrd["Lot_NO", i].Value).Trim();
					//重複なき場合は、Listへ追加
					if (ListSameLot.Contains(sLotNO) == false)
					{
						ListSameLot.Add(sLotNO);
					}
				}
			}

			DrawGraph(cndDataItem_One, ListSameLot, 1);
		}

		public bool Closed_FG { get; set; }
		private void frmDrawGraphAndList_FormClosed(object sender, FormClosedEventArgs e)
		{
			Closed_FG = true;
		}

		/// <summary>
		/// [工程一覧]ボタン押下時の入力チェック関数
		/// </summary>
		/// <returns></returns>
		bool InputCheckCatalog()
		{
			bool fCheck = false;
			DateTime wdt;

			//設備が未設定
			if (clbMachineList.CheckedItems.Count == 0)
			{
				fCheck = true;
			}

			try
			{
				wdt = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
				wdt = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);
			}
			catch
			{
				fCheck = true;
			}

			return fCheck;
		}

		//[工程一覧]ボタン
		private void btnCatalog_Click(object sender, EventArgs e)
		{
			try
			{
				int nTimmingNO = 0;
				string sWhereSQL = "";
				DateTime dtStart = DateTime.MinValue;
				DateTime dtEnd = DateTime.MinValue;

				if (InputCheckCatalog())
				{
					MessageBox.Show(Constant.MessageInfo.Message_54);
					return;
				}

				List<string> ListEqui = new List<string>();

				Dictionary<string, int> qtimList = Qtim.GetQtimList();

				qtimList.TryGetValue(cmbbAssetsNM.Text.Trim(), out nTimmingNO);

				#region コメントアウト
				//装置名をnTimmingNOへ変換
				//if (Constant.fMap)
				//{
				//TmQtimのTiming_NMの文字列
				//switch (cmbbAssetsNM.Text.Trim())
				//{
				//    case "ﾀﾞｲﾎﾞﾝﾀﾞｰZD":
				//        nTimmingNO = 1;
				//        break;
				//    case "ﾀﾞｲﾎﾞﾝﾀﾞｰLED":
				//        nTimmingNO = 2;
				//        break;
				//    case "ﾌﾟﾗｽﾞﾏDB":
				//        nTimmingNO = 3;
				//        break;
				//    case "ﾌﾟﾗｽﾞﾏWB":
				//        nTimmingNO = 4;
				//        break;
				//    case "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ":
				//        nTimmingNO = 5;
				//        break;
				//    case "外観検査機":
				//        nTimmingNO = 6;
				//        break;
				//    case "ﾓｰﾙﾄﾞ機":
				//        nTimmingNO = 7;
				//        break;
				//    case "遠心沈降機":
				//        nTimmingNO = 8;
				//        break;
				//    case "ﾀﾞｲｻｰ":
				//        nTimmingNO = 9;
				//        break;
				//    case "ﾛｯﾄﾏｰｷﾝｸﾞ":
				//        nTimmingNO = 10;
				//        break;
				//    case "ﾌﾟﾗｽﾞﾏMD":
				//        nTimmingNO = 11;
				//        break;
				//    case "ZDﾌﾘｯﾌﾟﾁｯﾌﾟ":
				//        nTimmingNO = 13;
				//        break;
				//    case "LEDﾌﾘｯﾌﾟﾁｯﾌﾟ":
				//        nTimmingNO = 14;
				//        break;
				//    case "ﾊﾞﾝﾌﾟﾎﾞﾝﾄﾞ":
				//        nTimmingNO = 15;
				//        break;
				//    case "圧縮成型":
				//        nTimmingNO = 16;
				//        break;
				//    case "ﾌﾞﾚｲｸ":
				//        nTimmingNO = 17;
				//        break;
				//    case "裏面洗浄":
				//        nTimmingNO = 18;
				//        break;
				//    case "ﾚｰｻﾞｰｽｸﾗｲﾌﾞ":
				//        nTimmingNO = 19;
				//        break;
				//    case "基板重量測定":
				//        nTimmingNO = 20;
				//        break;
				//    case "色調補正":
				//        nTimmingNO = 21;
				//        break;
				//    case "電着":
				//        nTimmingNO = 22;
				//        break;
				//    case "白電着":
				//        nTimmingNO = 23;
				//        break;
				//    case "外観検査機(ﾀﾞｲｽｸﾗｯｸ)":
				//        nTimmingNO = 24;
				//        break;

				//}
				//}
				//else
				//{
				//    switch (cmbbAssetsNM.Text.Trim())
				//    {
				//        case "ﾀﾞｲﾎﾞﾝﾀﾞｰ":
				//            nTimmingNO = 1;
				//            break;
				//        case "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ":
				//            nTimmingNO = 2;
				//            break;
				//        case "外観検査機":
				//            nTimmingNO = 3;
				//            break;
				//        case "ﾓｰﾙﾄﾞ機":
				//            nTimmingNO = 4;
				//            break;
				//    }
				//}

				#endregion

				sWhereSQL = GetWhereSQLEqui();

				dtStart = Convert.ToDateTime(txtbStartYear.Text + "/" + txtbStartMonth.Text + "/" + txtbStartDay.Text + " " + txtbStartHour.Text + ":" + txtbStartMinute.Text);
				dtEnd = Convert.ToDateTime(txtbEndYear.Text + "/" + txtbEndMonth.Text + "/" + txtbEndDay.Text + " " + txtbEndHour.Text + ":" + txtbEndMinute.Text);

				frmDrawGraphCatalog = new frmDrawGraphCatalog(_nLineCD, nTimmingNO, sWhereSQL, cmbbType.Text.Trim(), dtStart, dtEnd, this);

				frmDrawGraphCatalog.ShowDialog(this);

			}
			catch (Exception)
			{
				throw;
			}
			//消えない対策
			//this.Controls.Add((DataGridView)dgvNascaList);
			//this.Controls.Add((DataGridView)grdLotStb);
			//dgvNascaList.Visible = false;
			//grdLotStb.Visible = false;

		}

		private void cmbbQcParamNM_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				SettxtbInspectionNO_Map(cmbbQcParamNM.Items[cmbbQcParamNM.SelectedIndex].ToString().Trim());
			}
			else
			{
				SettxtbInspectionNO(cmbbQcParamNM.Items[cmbbQcParamNM.SelectedIndex].ToString().Trim());
			}
		}

		private void SettxtbInspectionNO_Map(string sInspectionNM)
		{
			if (sInspectionNM.Substring(0, 1) == "F")
			{
				sInspectionNM = sInspectionNM.Substring(0, 10);
			}
			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT Distinct Inspection_NO  FROM [TvQDIW_Map] With(NOLOCK) WHERE Inspection_NM='{0}'";
			string sqlCmdTxt = string.Format(BaseSql, sInspectionNM.Replace("'", "''"));
			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						txtbInspectionNO.Text = Convert.ToString(reader["Inspection_NO"]);
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
		}

		private void SettxtbInspectionNO(string sInspectionNM)
		{
			if (sInspectionNM.Substring(0, 1) == "F")
			{
				sInspectionNM = sInspectionNM.Substring(0, 10);
			}
			//閾値マスタにあるInline_CDのみ表示
			string BaseSql = "SELECT Distinct Inspection_NO  FROM [TvQDIW] With(NOLOCK) WHERE Inspection_NM='{0}'";
			string sqlCmdTxt = string.Format(BaseSql, sInspectionNM.Replace("'", "''"));
			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						txtbInspectionNO.Text = Convert.ToString(reader["Inspection_NO"]);
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
		}

		public Control DtLotStb
		{
			get { return (Control)grdLotStb; }
		}
		public Control DtNascaList
		{
			get { return (Control)dgvNascaList; }
		}

		private void label10_Click(object sender, EventArgs e)
		{

		}

	}

	public class StructureDrawList
	{
		public string P_LotNO { get; set; }
		public string C_MateGroupCD { get; set; }
		public string C_MateGroupNM { get; set; }
		public string C_MaterialCD { get; set; }
		public string C_MaterialNM { get; set; }
		public string C_LotNO { get; set; }
		public string P_MaterialCD { get; set; }
		public string P_PlantCD { get; set; }
		public string complt_dt { get; set; }
		public string MateChangeDT { get; set; }

	}
}



