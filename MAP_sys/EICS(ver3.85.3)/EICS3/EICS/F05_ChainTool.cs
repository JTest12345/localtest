using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EICS.Machine;

namespace EICS
{
    public partial class F05_ChainTool : Form
    {
        //Common Com = new Common();
        //GQCD gqcd = new GQCD();
        //PackageArms armsinfo = new PackageArms();

        string selectedAddress;
        /// <summary>
        /// 
        /// </summary>
        public F05_ChainTool(int lineCD)
        {
            InitializeComponent();
            MyInitializeComponent(lineCD);
        }

        /// <summary>
        /// [検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSerch_Click(object sender, EventArgs e)
        {
            MonRefresh();
        }

        /// <summary>
        /// [データ削除]ボタン
        /// 実際は削除しない。フォルダ名に"_"を付けるだけ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string sDir = gvChainList.SelectedRows[0].Cells[1].Value.ToString().Trim();
                FileInfo wFileInfo = new FileInfo(sDir);
                string swDir = wFileInfo.DirectoryName + "\\_" + wFileInfo.Name;
                Directory.Move(sDir, swDir);
            }
            catch 
            {
                MessageBox.Show(Constant.MessageInfo.Message_31, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            MonRefresh();
        }

        /// <summary>
        /// [紐付け実行]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChainExe_Click(object sender, EventArgs e)
        {
            if (ChecktxtbInput() == true)
            {
                MessageBox.Show(Constant.MessageInfo.Message_32, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //string sSETSUZOKU = Constant.StrQCIL;   //接続先を保管
            //GQCD.StrQCIL = @"Server=HQDB5\INST1;Connect Timeout=60;Database=QCIL;User ID=inline;password=R28uHta;Application Name=EICS2";//本番サーバーへ書き換え
            
            ChainExe();
            MessageBox.Show(Constant.MessageInfo.Message_33, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MonRefresh();
            //GQCD.StrQCIL = sSETSUZOKU;          //接続先を元に戻す
        }

        private void ChainExe() 
        {
            List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
            DBMachineInfo dbMachineInfo = new DBMachineInfo();
            //WBMachineInfo wbMachineInfo = new WBMachineInfo();

            //string sfpath = selectedAddress+"\\";
            string sfpath = selectedAddress;
            string sfname, sFileType, sFileType2, sFileType3, sWork;
            string[] textArray = new string[] { };
            //string sFolder;
            //string sToFolder;
            int nline = Convert.ToInt32(txtbLineNo.Text);   //ライン番号
            string sLot = txtbLotNo.Text;                   //Lot番号
            string sType = txtbType.Text;                   //Type
            string sEqui = txtbEqui.Text;                   //設備番号
            //string sUpdUsr = txtUpdUser.Text;               //更新者
			int nTimmingMode = Constant.nMagazineTimming;
            LSETInfo lsetInfo = ConnectDB.GetLSETData_Chain(nline, sEqui)[0];
            
            MagInfo MagInfo = new MagInfo();
            MagInfo.sMagazineNO = "";
            MagInfo.sNascaLotNO = sLot;  //最初のLot
            MagInfo.sMaterialCD = sType; //最初のLot
            
            try
            {
				List<EquipmentInfo> equipList = ConnectDB.GetEquipmentList(nline);
				EquipmentInfo equipInfo = equipList.Single(e => e.EquipmentNO == sEqui);

				MachineBase machineInfo = MachineBase.GetMachineInfo(equipInfo);


                FileInfo wwFileInfo = new FileInfo(sfpath);
                string sToDir = wwFileInfo.DirectoryName + "\\_" + wwFileInfo.Name + "_"+ sLot + "(" + sType + ")";
                foreach (string swfname in System.IO.Directory.GetFiles(sfpath))
                {
                    FileInfo wFileInfo = new FileInfo(swfname);
                    //sfname = swfname.Substring(sfpath.Length, swfname.Length - sfpath.Length);      //ファイル名取得
                    sfname = wFileInfo.Name;
                    sFileType = sfname.Substring(0, 1);                                             //ファイルタイプ取得
                    sFileType2 = sfname.Substring(1, 1);                                            //ファイル2タイプ取得
                    sFileType3 = sfname.Substring(0, 2);                                            //ファイル3タイプ取得
                    //0KBのﾌｧｲﾙは削除して次へ。
                    if (swfname.Length == 0)
                    {
                        System.IO.File.Delete(swfname);
                        continue;
                    }

                    using (System.IO.StreamReader textFile = new System.IO.StreamReader(swfname, System.Text.Encoding.Default))
                    {
                        sWork = textFile.ReadToEnd();
                        textFile.Close();
                    }

                    textArray = sWork.Split('\n');

                    //string spath1 = "", spath2 = "", spath3 = "", sCreateFileYM = "";
                    string sCreateFileYM = "";
                    string dtMeasureDT = "";
                    //ﾌｧｲﾙ更新日時を文字列
                    if (lsetInfo.AssetsNM == Constant.ASSETS_DB_NM || lsetInfo.AssetsNM == Constant.NMC_ASSETS_DB_NM)
                    {
                        dtMeasureDT = File.GetLastWriteTime(swfname).ToString();
                    }
                    else
                    {
                        sCreateFileYM = Convert.ToString(System.IO.File.GetLastWriteTime(swfname));
                        sCreateFileYM = sCreateFileYM.Substring(0, 4) + sCreateFileYM.Substring(5, 2);//yyyymm
						dtMeasureDT = WBMachineInfo.GetFileName_MeasureDT(sfname).ToString();
                    }

                    int fileLen;
                    switch (sFileType)
                    {
                        case "B"://DB
                            continue;//Armsが処理するのでこのまま
                        case "W"://DB
							
						    ((DBMachineInfo)machineInfo).DbInput_DB_WFile(lsetInfo, wFileInfo.FullName);

							break;
						case "H"://DB    
                            if (lsetInfo.ModelNM == Convert.ToString(DBMachineInfo.ModelType.AD8930))
                            {
                                fileLen = DBMachineInfo.FILE_NEED_LENGTH_H_AD8930;
                            }
                            else
                            {
                                fileLen = DBMachineInfo.FILE_NEED_LENGTH_H;
                            }

                            ((DBMachineInfo)machineInfo).DbInput_DB_HFile(lsetInfo, MagInfo, wFileInfo.FullName, ref errMessageList, fileLen,
                                null, DBMachineInfo.FileType.H);

                            break;
						case "I"://DB
                            if (lsetInfo.ModelNM == Convert.ToString(DBMachineInfo.ModelType.AD8930))
                            {
                                fileLen = DBMachineInfo.FILE_NEED_LENGTH_I_AD8930;
                            }
                            else
                            {
                                fileLen = DBMachineInfo.FILE_NEED_LENGTH_I;
                            }

                            ((DBMachineInfo)machineInfo).DbInput_DB_IFile(lsetInfo, MagInfo, wFileInfo.FullName, ref errMessageList, fileLen,
                                DBMachineInfo.GetCol(lsetInfo.ModelNM, DBMachineInfo.FILE_ERROR_COL_I), DBMachineInfo.FileType.I);

                            break;
						case "L"://DB
							((DBMachineInfo)machineInfo).DbInput_DB_LFile(lsetInfo, MagInfo, wFileInfo.FullName, ref errMessageList);
							((DBMachineInfo)machineInfo).CheckQC(lsetInfo, 1, MagInfo.sMaterialCD);//1はDBの意味

							break;
						case "O"://DB
							((DBMachineInfo)machineInfo).DbInput_DB_OFile(lsetInfo, MagInfo, wFileInfo.FullName, nTimmingMode, ref errMessageList);//エラー出力あり・データベース登録なし

							break;
						case "P"://DB
							((DBMachineInfo)machineInfo).DbInput_DB_PFile(lsetInfo, MagInfo, wFileInfo.FullName, nTimmingMode, ref errMessageList);

							break;
						case "_"://スタートタイミングでエラー出力済ファイル
							if (sFileType2 == "O")//DB
							{
								((DBMachineInfo)machineInfo).DbInput_DB_OFile(lsetInfo, MagInfo, wFileInfo.FullName, nTimmingMode, ref errMessageList);//エラー出力なし・データベース登録あり

							}
							else if (sFileType2 == "P")//DB
							{
								((DBMachineInfo)machineInfo).DbInput_DB_PFile(lsetInfo, MagInfo, wFileInfo.FullName, nTimmingMode, ref errMessageList);//エラー出力なし・データベース登録あり

							}
							else if (sFileType2 == "S")//WB SP
							{
								errMessageList.AddRange(((WBMachineInfo)machineInfo).DbInput_WB_SPFile(lsetInfo, MagInfo, wFileInfo.FullName, dtMeasureDT, nTimmingMode));//エラー出力なし・データベース登録あり

							}
							break;
						case "S"://WB SP
							errMessageList.AddRange(((WBMachineInfo)machineInfo).DbInput_WB_SPFile(lsetInfo, MagInfo, wFileInfo.FullName, dtMeasureDT, nTimmingMode));//エラー出力なし・データベース登録あり

							break;
						case "M"://WB MP,ML
							if (sFileType3 == "MP")
							{
								((WBMachineInfo)machineInfo).DbInput_WB_MPFile(lsetInfo, MagInfo, wFileInfo.FullName, dtMeasureDT, ref errMessageList);//エラー出力なし・データベース登録あり
							}
							else
							{
								((WBMachineInfo)machineInfo).DbInput_WB_MLFile(lsetInfo, MagInfo, wFileInfo.FullName, dtMeasureDT, ref errMessageList);//エラー出力なし・データベース登録あり
							}
							((WBMachineInfo)machineInfo).CheckQC(lsetInfo, 5, MagInfo.sMaterialCD);//2はWBの意味    

                            break;

                    }
                    FileInfo fileInfo = new FileInfo(swfname);
                    DirectoryInfo DirInfo = new DirectoryInfo(fileInfo.FullName);
                    
                }
                System.IO.Directory.Move(sfpath, sToDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool ChecktxtbInput()
		{
			int lineCD;    
			bool flg = false;

            //Lotが入力されている
            if (txtbLotNo.Text.Trim() == "")
            {
                flg = true;
                return flg;
            }

            //LotからTypeを取得可能
            if (txtbType.Text.Trim() == "")
            {
				if (!int.TryParse(txtbLineNo.Text, out lineCD))
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_96, txtbLineNo.Text));
				}

                txtbType.Text = ConnectDB.GetARMSLotType(lineCD, txtbLotNo.Text);
                if (txtbType.Text.Trim() == "")
                {
                    MessageBox.Show(Constant.MessageInfo.Message_34, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtbType.Enabled = true;

                    flg = true;
                    return flg;
                }
            }

            //設備が入力されている
            if (txtbEqui.Text.Trim() == "")
            {
                flg = true;
                return flg;
            }

            //登録者が入力されている
            //if (txtUpdUser.Text.Trim() == "")
            //{
            //    flg = true;
            //    return flg;
            //}

            //ARMS
            //設備Noからmacno取得
            //int nmacno = armsinfo.GetMacNo(txtbEqui.Text.Trim());
            //DateTime dtStartDT = Convert.ToDateTime(txtbStartYDT.Text.Trim() + "/" + txtbStartMDT.Text.Trim() + "/01 0:00:00");
            //DateTime dtEndDT = dtStartDT.AddMonths(1);
            //flg = armsinfo.CheckLotUSE(nmacno, dtStartDT,dtEndDT, txtbLotNo.Text.Trim());
            return flg;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void MyInitializeComponent(int lineCD)
        {
            txtbLineNo.Text = Convert.ToString(lineCD);//ライン番号
            //txtbStartDT.Text= DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd 0:00:00");//開始日時
            //txtbEndDT.Text = DateTime.Now.ToString("yyyy/MM/dd 0:00:00");//完了日時
            txtbStartYDT.Text = DateTime.Now.AddDays(-1).ToString("yyyy");
            txtbStartMDT.Text = DateTime.Now.AddDays(-1).ToString("MM");

            txtUpdUser.Text="";   //登録者

            MonRefresh();
        }

        /// <summary>
        /// グリッドビューのリフレッシュ
        /// </summary>
        private void MonRefresh()
        {
			int lineCD;

            dsF05.DtItems.Clear();

            txtbLotNo.Text = "";  //ロット番号
            txtbType.Text = ""; //Type
            txtbEqui.Text = "";   //設備番号

            txtbType.Enabled = false;//Type Disable

            string sYYYYMM = string.Empty;
            //int nStartyyyymmdd = int.MinValue;
            //int nEndyyyymmdd = int.MinValue;

            //設備フォルダ直下の保管フォルダ探索用
            //sYYYYMM = txtbEndDT.Text.Substring(0, 4) + txtbStartDT.Text.Substring(5, 2);
            DateTime wdt = Convert.ToDateTime(txtbStartYDT.Text.Trim() +"/"+ txtbStartMDT.Text.Trim()+ "/01 0:00:00");

            //sYYYYMM = txtbStartYDT.Text.Trim() + txtbStartMDT.Text.Trim();
            sYYYYMM = wdt.ToString("yyyyMM");
            /*
            //unchainフォルダ直下の保管フォルダ探索用
            nStartyyyymmdd = Convert.ToInt32(txtbStartDT.Text.Substring(0, 4) +
                                                txtbStartDT.Text.Substring(5, 2) +
                                                txtbStartDT.Text.Substring(8, 2));
            nEndyyyymmdd = Convert.ToInt32(txtbEndDT.Text.Substring(0, 4) +
                                                txtbStartDT.Text.Substring(5, 2) +
                                                txtbStartDT.Text.Substring(8, 2));
            */
            //探索リスト取得
            List<UnchainInfo> ListUnchainInfo = new List<UnchainInfo>();        //探索ディレクトリ一式
            List<UnchainInfo> OutputListUnchainInfo = new List<UnchainInfo>();  //探索ディレクトリで発見されたUnchainフォルダ一式→グリッドビューに表示

			if (!int.TryParse(txtbLineNo.Text, out lineCD))
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_96, txtbLineNo.Text));
			}
            ListUnchainInfo = ConnectDB.GetListUnchainInfo(lineCD);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

            //探索ディレクトリ内にUnchainフォルダがあるか探索しList化
            foreach (UnchainInfo unchaininfo in ListUnchainInfo)
            {
                if (settingInfoPerLine.EquipmentList.FindAll(e => e.EquipmentNO == unchaininfo.Equi).Count == 0)
                {
                    continue;
                }

                //C:\QCIL\data\WB\S07005\201111\unchain
                unchaininfo.Dir = unchaininfo.Dir.Trim() + sYYYYMM + "\\unchain";//探索ディレクトリ
                FileInfo wFileInfo = new FileInfo(unchaininfo.Dir);
                DirectoryInfo wDirectoryInfo = new DirectoryInfo(wFileInfo.FullName);
                if (!wDirectoryInfo.Exists)
                {
                    continue;
                }

                //本日分だけしか探索出来ければ、夜勤者に確認出来ないので不採用。
                //DirectoryInfo[] wwDirectoryInfo = wDirectoryInfo.GetDirectories(Convert.ToString(nStartyyyymmdd) + "*");
                //
                DirectoryInfo[] wwDirectoryInfo = wDirectoryInfo.GetDirectories(sYYYYMM + "*");
                for (int i = 0; i < wwDirectoryInfo.Length; i++)
                {
                    UnchainInfo wunchaininfo = new UnchainInfo();
                    wunchaininfo.Equi = unchaininfo.Equi;
                    wunchaininfo.Dir = wwDirectoryInfo[i].FullName;
                    OutputListUnchainInfo.Add(wunchaininfo);
                }
            }

            foreach (UnchainInfo unchainInfo in OutputListUnchainInfo) 
            {
                DataRow dr = dsF05.DtItems.NewRow();
                dr[dsF05.DtItems.Plant_CDColumn] = unchainInfo.Equi;
                dr[dsF05.DtItems.Address_VALColumn] = unchainInfo.Dir;
                dsF05.DtItems.Rows.Add(dr);
            }

            //グリッドビューに挿入
            //gvChainList.DataSource = OutputListUnchainInfo;

            //グリッドビュー表示設定
            //gvChainList.Columns[0].HeaderText = "設備番号";
            //gvChainList.Columns[1].HeaderText = "アドレス";
            //gvChainList.Columns[1].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        }

        //選択行変更    
        private void gvChainList_SelectionChanged(object sender, EventArgs e)
        {
            string sEqui = "";
            txtbEqui.Text=sEqui;
            if (gvChainList.SelectedRows.Count > 0)
            {
                txtbEqui.Text = gvChainList.SelectedRows[0].Cells[0].Value.ToString();  //設備番号
                selectedAddress = gvChainList.SelectedRows[0].Cells[1].Value.ToString();//アドレス
                btnDelete.Enabled = true;
                btnChainExe.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
                btnChainExe.Enabled = false;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void F05_ChainTool_Load(object sender, EventArgs e)
        {

        }
    }
}
    