using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace GEICS
{
    public partial class frmLotInfo : Form
    {
        F03_TrendChart _parent = null;
        List<string> _lotList = new List<string>();
        private bool _IsNascaConnectFG = false;
        /// <summary>ロット・設備グリッド用テーブル</summary>
        DataTable _TblLotStb = new DataTable();
        Dictionary<string, string> _dicStb = new Dictionary<string, string>(); //設備ディクショナリ

        public frmLotInfo(F03_TrendChart parent)
        {
            _parent = parent;
            TopLevel = true;
            InitializeComponent();
        }

        public frmLotInfo(List<string> lotList)
        {
            _lotList = lotList;
            TopLevel = true;
            InitializeComponent();
        }

        private void frmLotInfo_Load(object sender, EventArgs e)
        {
            //NASCAへの接続状態を確認
            _IsNascaConnectFG = ConnectNASCA.CheckConnect();

            if (_parent != null)
            {
                splitContainer1.Panel1.Controls.Add(this._parent.DtLotStb);
                splitContainer1.Panel2.Controls.Add(this._parent.DtNascaList);
            }
            else 
            {
                grdLotStb.Visible = true;
                dgvNascaList.Visible = true;

                SETdTblMaterial_ARMS();
                SetTblLotStb_ARMS();
                ViewGrdLotStb();
            }
        }

        /// <summary>
        /// 資材グリッド表示用テーブルの取得(ARMS)
        /// </summary>
        /// <param name="sWhereSql"></param>
        private void SETdTblMaterial_ARMS()
        {
            dsMaterial.dTblMaterial.Clear();

            foreach (string lotNO in _lotList)
            {
                List<MaterialStbInfo> matStbList = new List<MaterialStbInfo>();

                //資材取得-----------------------------------------
                List<MaterialStbInfo> matStbMainList = ConnectARMS.GetTblMaterialStb_ARMS(lotNO);
                foreach (MaterialStbInfo matStbStockInfo in matStbMainList)
                {
                    matStbList.Add(matStbStockInfo);
                }

                //資材(搭載機Stocker1)の取得-----------------------
                List<MaterialStbInfo> matStbStock1List = ConnectARMS.GetTblMaterialStb_ARMS(lotNO, "stocker1");
                foreach (MaterialStbInfo matStbStockInfo in matStbStock1List)
                {
                    matStbList.Add(matStbStockInfo);
                }

                //資材(搭載機Stocker2)の取得-----------------------
                List<MaterialStbInfo> matStbStock2List = ConnectARMS.GetTblMaterialStb_ARMS(lotNO, "stocker2");
                foreach (MaterialStbInfo matStbStockInfo in matStbStock2List)
                {
                    matStbList.Add(matStbStockInfo);
                }

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

                //資材(樹脂)の取得---------------------------------
                List<MaterialStbInfo> matStbStock3List = ConnectARMS.GetTblMaterialResinStb_ARMS(lotNO);
                if (_IsNascaConnectFG)
                {
                    //NASCAに接続できる場合、調合材料を取得する
                    matStbStock3List = GetResinData(matStbStock3List);
                }
                foreach (MaterialStbInfo matStbStockInfo in matStbStock3List)
                {
                    matStbList.Add(matStbStockInfo);
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

                //交換日時を取得
                GetChangeTiming_ARMS(ref matStbList);

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
                }
            }
        }

        /// <summary>
        /// ロット・設備グリッド表示用テーブルの取得(ARMS)
        /// </summary>
        private void SetTblLotStb_ARMS()
        {
            ArrayList arlData = new ArrayList();

            foreach (string lotNO in _lotList)
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

        /// <summary>
        /// 交換日時を取得(ARMS)
        /// </summary>
        /// <param name="matStbList"></param>
        private void GetChangeTiming_ARMS(ref List<MaterialStbInfo> matStbList)
        {
            for (int i = 0; i < matStbList.Count; i++)
            {
                matStbList[i].ChangeDT = ConnectARMS.GetChangeTimingDT(matStbList[i]);
            }
        }

        /// <summary>
        /// 品目グループ
        /// </summary>
        private void GetMateGroup(ref List<MaterialStbInfo> matStbList)
        {
            for (int i = 0; i < matStbList.Count; i++)
            {
                MateGroupInfo mateGroupInfo = ConnectNASCA.GetMateGroup(matStbList[i].MaterialCD);
                matStbList[i].MateGroupCD = mateGroupInfo.MateGroupCD;
                matStbList[i].MateGroupNM = mateGroupInfo.MateGroupNM;
            }
        }

        /// <summary>
        /// ロット・設備情報を表示
        /// </summary>
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

        private void frmLotInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
