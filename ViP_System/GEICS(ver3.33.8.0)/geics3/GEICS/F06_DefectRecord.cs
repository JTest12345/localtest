using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SLCommonLib.Commons;
using GEICS.Function;

namespace GEICS
{
    public partial class F06_DefectRecord : Form
    {
        public F06_DefectRecord()
        {
            InitializeComponent();
        }

        /// <summary>ファイルMM内容開始行</summary>
        private const int FILE_MM_DATASTARTROW = 2;

        /// <summary>マッピング周辺検査数</summary>
        private const int MAPPING_SINSP_CT = 5;

        /// <summary>マッピング周辺検査識別文字</summary>
        private const string MAPPING_SINSP_KB = "S";

        /// <summary>ファイルMM内容列(エラーCD)不良マーク検出</summary>
        private const string FILE_MM_ERRORCD_BADMARK = "0x0018";

        /// <summary>ファイルMM内容列(エラーCD)不良マーク検出スキップ</summary>
        private const string FILE_MM_ERRORCD_BADMARKSKIP = "0x001E";

        /// <summary>ファイルMM内容列(ロギングアドレス)</summary>
        private const int FILE_MM_ADDRESSNO = 4;

        /// <summary>ファイルMM内容列(エラーCD)</summary>
        private const int FILE_MM_ERRORCD = 5;

        /// <summary>ファイルMM内容列(ユニットNO)</summary>
        private const int FILE_MM_UNITNO = 6;

        /// <summary>ファイルMM内容列(処理CD)</summary>
        private const int FILE_MM_TRANCD = 7;

        /// <summary>ログファイル保管場所(WB)</summary>
        private const string FILE_DEPOSITORY_WB = @"\\{0}\QCIL\data\WB\{1}\{2}\Bind\{3}";

        /// <summary>マッピングファイル保管場所(AI)</summary>
        private const string MAPPINGFILE_DEPOSITORY_AI = @"\\{0}\QCIL\data\AI\Mapping";

        /// <summary>
        /// MMファイル処理CD
        /// </summary>
        public enum MappingBaseTranCD : int
        {
            /// <summary>処理無し</summary>
            None = 0,
            /// <summary>外観検査</summary>
            Inspection = 1,
            /// <summary>周辺強度</summary>
            Strength = 2,
            /// <summary>目視検査</summary>
            Seeing = 3,
            /// <summary>検査無し</summary>
            NotInspection = 4
        }

        /// <summary>
        /// マッピングファイル検査優先度
        /// </summary>
        private enum MappingDataTranCD : int
        {
            /// <summary>処理無し</summary>
            None = 0,
            /// <summary>周辺検査</summary>
            Around = 1,
            /// <summary>検査無し</summary>
            NotInspection = 2,
            /// <summary>外観検査</summary>
            Inspection = 3,
            /// <summary>周辺強度</summary>
            Strength = 4,
            /// <summary>目視検査</summary>
            Seeing = 5
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDefectData_Load(object sender, EventArgs e)
        {
            //不良一覧グリッド処理CD列のコンボボックスリスト生成
            List<ServInfo> servList = ConnectQCIL.GetServData();
            cmbServer.DataSource = servList;
            cmbServer.DisplayMember = "ServerNM";
            cmbServer.ValueMember = "ServerCD";

			bool packageFG = false;

			if (Constant.settingInfoList != null)
			{
				foreach (SettingInfo settingInfo in Constant.settingInfoList)
				{
					if (settingInfo.InlineCD == Common.nLineCD)
					{
						packageFG = settingInfo.PackageFG;
					}
				}
			}
            //中間サーバの場合、ライン固定
			if (packageFG)
            {
                txtLineCD.Text = Common.nLineCD.ToString();
                txtLineCD.ReadOnly = true;

				cmbServer.SelectedValue = Common.nLineCD.ToString();
                cmbServer.Enabled = false;
            }          
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                List<GeneralInfo> generalList = ConnectQCIL.GetGeneralData(Constant.GENERALGRP_CD_WBTRAN);

                //グリッド列[処理CD]コンボボックスのアイテム作成
                DataGridViewComboBoxColumn tranCombobox = (DataGridViewComboBoxColumn)dgvDefect.Columns["TransactionCD"];
                tranCombobox.DataSource = generalList;
                tranCombobox.DisplayMember = "GeneralNM";
                tranCombobox.ValueMember = "GeneralCD";

                //グリッド列[処理CD(変更)]コンボボックスのアイテム作成
                DataGridViewComboBoxColumn updTranCombobox = (DataGridViewComboBoxColumn)dgvDefect.Columns["UpdateTransactionCD"];
                updTranCombobox.DataSource = generalList;
                updTranCombobox.DisplayMember = "GeneralNM";
                updTranCombobox.ValueMember = "GeneralCD";

                //不良一覧出力
                outputDefectData();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// グリッドセル値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDefect_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDefect.SelectedRows.Count == 0)
            {
                return;
            }
			
            ((SortableBindingList<DefectInfo>)bsDefect.DataSource)[e.RowIndex].ChangeFG = true;
            dgvDefect.Rows[e.RowIndex].Cells["ChangeFG"].Value = true;

			string strUpdateDefAddressNO = ((SortableBindingList<DefectInfo>)bsDefect.DataSource)[e.RowIndex].UpdateDefAddressNO;

			if (string.IsNullOrEmpty(strUpdateDefAddressNO) == false)
			{
				EICSFunction.CheckIntParsable(strUpdateDefAddressNO);
			}
        }



        #region グリッドツールバーイベント

        /// <summary>
        /// 保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void toolPreserve_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_25, "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
			{
				return;
			}

			try
			{
				dgvDefect.EndEdit();

				//変更行を取得
				List<DefectInfo> targetList = (((SortableBindingList<DefectInfo>)bsDefect.DataSource).Where(d => d.ChangeFG).ToList());
				if (targetList.Count == 0)
				{
					return;
				}

				F70_InputUserCD frmInputUserCD = new F70_InputUserCD();
				frmInputUserCD.ShowDialog();

				string updUserCD = frmInputUserCD.GetInputUserCD();
				if (string.IsNullOrEmpty(updUserCD))
				{
					throw new ApplicationException("入力してください。");
				}

				EICSFunction.UpdateDefect(targetList, updUserCD);

				MessageBox.Show(Constant.MessageInfo.Message_75, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<MappingDataInfo> MMFileProcess(ref List<DefectInfo> targetList)
		{
			string lotNO = targetList[0].LotNO;
			string equipmentNO = targetList[0].PlantCD;
			string dateNO = targetList[0].TargetDT.ToString("yyyyMM");

			//MMファイル保管場所パスを生成
			string lotDir = string.Format(FILE_DEPOSITORY_WB
				, ((ServInfo)cmbServer.SelectedItem).ServerCD, equipmentNO, dateNO, lotNO);
			if (!Directory.Exists(lotDir))
			{
				//MMファイル保管場所が存在しない場合、処理終了
				throw new Exception(string.Format(Constant.MessageInfo.Message_81, lotDir));
			}

			//MMファイル内容取得
			string[] fileList = Directory.GetFiles(lotDir, "MM*");
			if (fileList.Length == 0)
			{
				//MMファイルが存在しない場合、処理終了
				throw new Exception(string.Format(Constant.MessageInfo.Message_82, lotDir));
			}
			if (fileList.Length > 1)
			{
				//MMファイルが複数存在する場合、処理終了
				throw new Exception(string.Format(Constant.MessageInfo.Message_83, lotDir));
			}
			string[] fileLineValue = MachineFileInfo.GetMachineFileLineValue(fileList[0]);

			//フレーム情報を取得
			string typeCD = ConnectARMS.GetTypeCD(lotNO);
			FRAMEInfo frameInfo = ConnectQCIL.GetFRAMEData(typeCD);

			//MMファイル内容修正リスト取得(不良修正箇所だけ書き換えたリスト)
			List<MappingBaseInfo> mappingBaseList = getReWriteMachineData(fileLineValue, ref targetList);

			//MMファイル内容修正リストからマッピングリスト取得
			List<MappingDataInfo> mappingDataList = getMappingData(frameInfo.MagazinPackageMAXCT, mappingBaseList);

			return mappingDataList;
		}

        /// <summary>
        /// 選択ロット検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolLotSearch_Click(object sender, EventArgs e)
        {
            if (dgvDefect.SelectedRows.Count == 0)
            {
                return;
            }

            txtLotNO.Text = dgvDefect.CurrentRow.Cells["LotNO"].Value.ToString();

            //不良一覧出力
            outputDefectData();
        }

        #endregion

        /// <summary>
        /// 不良一覧出力
        /// </summary>
        private void outputDefectData()
        {
            try
            {
                //検索条件を取得
                DefectSearchInfo searchInfo = getDefectSearchInfo();

                List<DefectInfo> defectList = ConnectQCIL.GetDefectData(searchInfo);


                SortableBindingList<DefectInfo> sortDefectList = new SortableBindingList<DefectInfo>();
                foreach (DefectInfo defectInfo in defectList)
                {
                    DefectInfo sortDefectInfo = defectInfo;
                    sortDefectList.Add(sortDefectInfo);
                }
                
                //不良履歴を取得
                bsDefect.DataSource = sortDefectList;


                foreach(DataGridViewRow gridRow in dgvDefect.Rows)
                {
                    if (!Convert.ToBoolean(gridRow.Cells["AddressCompareFG"].Value)) 
                    {
                        //アドレス照合NGは背景色変更
                        gridRow.DefaultCellStyle.BackColor = Color.MistyRose;
                    }
                }

                //保存ボタン使用不可へ(ロット単位(マッピングファイル単位)で保存処理を行う為。)
                if (txtLotNO.Text == string.Empty)
                {
                    toolPreserve.Enabled = false;
                }
                else
                {
                    toolPreserve.Enabled = true;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 選択している検索条件を取得
        /// </summary>
        /// <returns>検索条件</returns>
        private DefectSearchInfo getDefectSearchInfo()
        {
            DefectSearchInfo searchInfo = new DefectSearchInfo();

            int LineCD = int.MinValue;
            if (int.TryParse(txtLineCD.Text, out LineCD))
            {
                searchInfo.LineCD = LineCD;
            }

            searchInfo.PlantCD = txtPlantCD.Text;
            searchInfo.LotNO = txtLotNO.Text;
            searchInfo.DefItemNM = txtDefItemNM.Text;
			searchInfo.IsTargetDelRecord = chkIsTargetDelRecord.Checked;

            if (dtsTargetDT.Checked)
            {
                searchInfo.TargetFromDT = Convert.ToDateTime(dtsTargetDT.StartDate);
                searchInfo.TargetToDT = Convert.ToDateTime(dtsTargetDT.EndDate);
            }
            else
            {
                searchInfo.TargetFromDT = null;
                searchInfo.TargetToDT = null;
            }

            return searchInfo;
        }

        /// <summary>
        /// MMファイル内容修正リスト取得(不良修正箇所だけ書き換えたリスト)
        /// </summary>
        /// <param name="targetList">MMファイル内容</param>
        /// <param name="defectList">不良修正箇所リスト</param>
        /// <returns>MMファイル内容修正リスト</returns>
        private List<MappingBaseInfo> getReWriteMachineData(string[] targetList, ref List<DefectInfo> defectList) 
        {
            try
            {
                //MMファイル内容からマッピング元リスト領域を生成する
                List<MappingBaseInfo> mappingBaseList = new List<MappingBaseInfo>();
                for (int i = FILE_MM_DATASTARTROW; i < targetList.Length; i++)
                {
                    string[] targetValue = targetList[i].Split(',');

                    int addressNO = Convert.ToInt32(targetValue[FILE_MM_ADDRESSNO].Trim());
                    if (addressNO == 0) { continue; }

                    int unitNO = Convert.ToInt32(targetValue[FILE_MM_UNITNO].Trim());
                    string errorCD = targetValue[FILE_MM_ERRORCD].Trim();
                    string tranCD = targetValue[FILE_MM_TRANCD].Trim();

                    MappingBaseInfo mappingBaseInfo = new MappingBaseInfo(unitNO, addressNO, tranCD, errorCD);
                    mappingBaseList.Add(mappingBaseInfo);
                }

                foreach(DefectInfo defectInfo in defectList)
                {
                    int targetIndex = mappingBaseList.FindIndex(m => m.AddressNO.ToString() == defectInfo.DefAddressNO && m.UnitNO.ToString() == defectInfo.DefUnitNO);

                    //MMファイル処理CDを取得
                    defectInfo.TranCD = mappingBaseList[targetIndex].TranNO;
                    //変更処理CDに書き換える
                    mappingBaseList[targetIndex].TranNO = defectInfo.UpdateTranCD; 
                }

                return mappingBaseList;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        /// </summary>
        /// MMファイル内容修正リストからマッピングリスト取得
        /// </summary>
        /// <param name="targetList">元ファイル(MMファイル)内容</param>
        /// <returns>マッピング内容</returns>
        private List<MappingDataInfo> getMappingData(int mappingRange, List<MappingBaseInfo> mappingBaseList) 
        {
            List<MappingDataInfo> rMappingDataList = new List<MappingDataInfo>();

            try
            {
                //周辺検査(S)を代入
                for (int i = 0; i < mappingBaseList.Count; i++)
                {
                    if (mappingBaseList[i].TranNO != ((int)MappingBaseTranCD.Inspection).ToString())
                    {
                        continue;
                    }

                    //検査(1)の前5つに"S"を代入  Inspection,Eject,Reserve優先
                    int sInspCount = 0;
                    for (int beforeIndex = i - 1; sInspCount < MAPPING_SINSP_CT; beforeIndex--)
                    {
                        if (beforeIndex < 0) { break; }

                        string errorCD = mappingBaseList[beforeIndex].ErrorCD;
                        if (errorCD == FILE_MM_ERRORCD_BADMARK || errorCD == FILE_MM_ERRORCD_BADMARKSKIP)
                        {
                            //不良CDがバッドマークの場合、1つ飛ばす
                            beforeIndex--;
                            if (beforeIndex < 0) { break; }
                        }

                        mappingBaseList[beforeIndex].TranNO
                            = CompareMappingTranCD(MAPPING_SINSP_KB, mappingBaseList[beforeIndex].TranNO);
                        sInspCount++;
                    }

                    //検査(1)の後5つに"S"を代入  Inspection,Eject,Reserve優先
                    sInspCount = 0;
                    for (int afterIndex = i + 1; sInspCount < MAPPING_SINSP_CT; afterIndex++)
                    {
                        if (afterIndex + 1 > mappingBaseList.Count) { break; }

                        string errorCD = mappingBaseList[afterIndex].ErrorCD;
                        if (errorCD == FILE_MM_ERRORCD_BADMARK || errorCD == FILE_MM_ERRORCD_BADMARKSKIP)
                        {
                            //不良CDがバッドマークの場合、1つ飛ばす
                            afterIndex++;
                            if (afterIndex + 1 > mappingBaseList.Count) { break; }
                        }

                        mappingBaseList[afterIndex].TranNO
                            = CompareMappingTranCD(MAPPING_SINSP_KB, mappingBaseList[afterIndex].TranNO);
                        sInspCount++;
                    }
                }

#if TEST
                ////MMファイル検査状態を表示する
                //OutputMappingResult(@"C:\QCIL\MappingBaseData.txt", mappingBaseList);
#endif

                //ユニットを合わせたマッピング元データを生成する
                foreach (MappingBaseInfo mappingBaseInfo in mappingBaseList)
                {
                    if (!rMappingDataList.Exists(m => m.AddressNO == mappingBaseInfo.AddressNO))
                    {
                        MappingDataInfo mappingDataInfo = new MappingDataInfo(mappingBaseInfo.AddressNO, mappingBaseInfo.TranNO);
                        rMappingDataList.Add(mappingDataInfo);
                    }
                    else
                    {
                        //ユニットとユニットを比較して優先順位が高い方の処理CDをアドレスへ代入する
                        int targetIndex = rMappingDataList.FindIndex(m => m.AddressNO == mappingBaseInfo.AddressNO);
                        rMappingDataList[targetIndex].InspectionNO
                            = CompareMappingTranCD(mappingBaseInfo.TranNO, rMappingDataList[targetIndex].InspectionNO);
                    }
                }

                //不足アドレスがないか確認
                for (int addressNO = 1; addressNO <= mappingRange; addressNO++)
                {
                    if (!rMappingDataList.Exists(m => m.AddressNO == addressNO))
                    {
                        //不足アドレスが存在する場合、そのアドレスに周辺検査(S)を設定する。
                        MappingDataInfo mappingInspInfo = new MappingDataInfo(addressNO, MAPPING_SINSP_KB);
                        rMappingDataList.Add(mappingInspInfo);

                    }
                }

                return rMappingDataList.OrderBy(insp => insp.AddressNO).ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// MMファイル処理CDを比較して優先度が高い方を返す
        /// </summary>
        /// <param name="tranCD1">比較対象1</param>
        /// <param name="tranCD2">比較対象2</param>
        /// <returns></returns>
        private static string CompareMappingTranCD(string tranCD1, string tranCD2)
        {
            //比較対象1の優先度を取得
            string baseName = string.Empty;
            if (tranCD1 == MAPPING_SINSP_KB)
            {
                baseName = MappingDataTranCD.Around.ToString();
            }
            else
            {
                baseName = Enum.GetName(typeof(MappingBaseTranCD), Convert.ToInt32(tranCD1));
            }
            int dataOrderNO1 = Convert.ToInt32(((MappingDataTranCD)Enum.Parse(typeof(MappingDataTranCD), baseName)));

            //比較対象2の優先度を取得
            if (tranCD2 == MAPPING_SINSP_KB)
            {
                baseName = MappingDataTranCD.Around.ToString();
            }
            else
            {
                baseName = Enum.GetName(typeof(MappingBaseTranCD), Convert.ToInt32(tranCD2));
            }
            int dataOrderNO2 = Convert.ToInt32(((MappingDataTranCD)Enum.Parse(typeof(MappingDataTranCD), baseName)));

            //比較して優先度が高い方を返す
            if (dataOrderNO1 > dataOrderNO2)
            {
                return tranCD1;
            }
            else
            {
                return tranCD2;
            }
        }


    }
}
