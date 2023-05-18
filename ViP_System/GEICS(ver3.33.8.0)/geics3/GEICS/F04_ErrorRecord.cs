using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Xml;
using System.Threading;
using System.Collections.Specialized;
using GEICS.Database;

namespace GEICS
{
    public partial class F04_ErrorRecord : Form
    {
        Common Com = new Common();
        Painter p = new Painter();

        F05_MachineErrorRecord Form2;
        F07_Report frmReport;

        static public bool fDrawComplete;
        //static public bool fClient;

        //static public string StrNASCA;
        static public string StrARMS;
        //static public string StrLineNM;
        //static public int nLineCD;
        //static public bool fPackage;//中間サーバーでパッケージ化されたラインの場合TRUE
                                    //TRUEの場合、外観検査結果(F****)データはNASCAを見ずにTnLOGを見る。
        //static public bool fMap;    //Trueの場合MAP C:\QCIL\SettingFiles\QCIL.xmlの<MAP value="ON" /> を見て設定
        //static public string sPackagePC;//IPAddress or PCNo

		//static public List<TmLINEInfo> ListTmLINEInfo;
		static List<TmLINEInfo> TmLINEInfoList;

        static string myPlant = string.Empty;
        static string myCategory = string.Empty;
        static string myLineName = string.Empty;
        
        //設備情報
        public struct EquiInfo
        {
            public string sEquipmentNO;     //設備番号('SLC-'は付けない)
            public string sAssetsNM;        //設備名
            public string sMachinSeqNO;     //号機番号
            public string sModelNM;         //装置型式
            public int nDispNO;             //表示順
            public string sIPAddressNO;     //IPアドレス
            public string sInputFolderNM;   //装置出力ファイルの取得先
        }

        public F04_ErrorRecord()
        {
            InitializeComponent();

        }

        //タイマーイベント

        //更新ボタン
        private void btnUpdate_Click(object sender, EventArgs e)
        {
			ResetForm();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SetStartDate();
            SetEndDate();

            InitForm();
            string sLineNM = "";//9001→9001(高生産性ライン)へ変更

            if (Common.nLineCD == 0 || Common.nLineCD == int.MinValue)
            {
                if (cmbbLineNo.Items.Count > 0)
                {
                    cmbbLineNo.Text = cmbbLineNo.Items[0].ToString();
                }
            }
            else
            {
                sLineNM = Com.GetInlineString(Common.nLineCD);
                if (sLineNM == "")
                {
                    MessageBox.Show(string.Format(Constant.MessageInfo.Message_46, Constant.StrQCIL));
                    return;
                }
                cmbbLineNo.Text = sLineNM;
            }
            myPlant = cmbbPlantNM.Text;
            myCategory = cmbbLineCategory.Text;
            myLineName = cmbbLineNo.Text;

			List<TmLINEInfo> lineInfoList = Line.GetTmLINEInfo(false, Common.nLineCD);
			if (lineInfoList.Count > 0)
			{
                Common.notUseTmQdiwFG = lineInfoList.Find(l => l.Inline_CD == Common.nLineCD).NotUseTmQDIW;
			}


            if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
            {
                gvQCErrList.Columns["Defect_NM"].Visible = false;
            }

            //行番号表示
            this.gvQCErrList.RowPostPaint += new DataGridViewRowPostPaintEventHandler(Common.Grd_RowPostPaint);

            txtbGraphPlotNum.Text = "30";//初期値は500

            //エラーリストの表示方法
            //rbAllData.Checked = true;
            //rbUnConfirmedData.Checked = false;

            //更新ボタンクリック
			ResetForm();
			//btnUpdate.PerformClick();
        }

        //工場の項目追加
        private void SetCmbbPlantNM(List<TmLINEInfo> tmLineInfo)
        {
            this.cmbbPlantNM.Items.Clear();

            //ListTmLINEInfo([インライン番号/工場名/カテゴリ/ライン名]リスト)から
            //重複を省いてコンボボックスのアイテムに追加
            int i = 0;
			foreach (TmLINEInfo wTmLINEInfo in tmLineInfo)
            {
                //工場コンボボックスになければ追加
                if (this.cmbbPlantNM.Items.Contains(wTmLINEInfo.Plant_NM.Trim()) == false)
                {
                    this.cmbbPlantNM.Items.Insert(i, wTmLINEInfo.Plant_NM.Trim());
                    i = i + 1;
                }
                
            }
            //初期値として先頭を選択
            if (this.cmbbPlantNM.Items.Count > 0)
            {
                cmbbPlantNM.Text = cmbbPlantNM.Items[0].ToString();
            }

            /*
            string sqlCmdTxt = "SELECT DISTINCT [Plant_NM] FROM [TmLINE] WITH(NOLOCK) WHERE Del_FG <> '1' ORDER BY [Plant_NM]  ASC";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader rd = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    rd = connect.Command.ExecuteReader();

                    int i = 0;
                    while (rd.Read())
                    {
                        this.cmbbPlantNM.Items.Insert(i, Convert.ToString(rd["Plant_NM"]).Trim());
                        i = i + 1;
                    }
                }
                finally
                {
                    if (rd != null) rd.Close();
                    connect.Close();
                }
            }
            */
        }

		//カテゴリの項目追加
		private void AddCmbbLineCategory(TmLINEInfo tmLineInfo)
		{
			if (cmbbPlantNM.SelectedIndex == -1)
			{
				return;
			}

			//ListTmLINEInfo([インライン番号/工場名/カテゴリ/ライン名]リスト)から
			//重複を省いてコンボボックスのアイテムに追加
			int i = 0;

				//選択されている工場と同じであれば
				if (tmLineInfo.Plant_NM == cmbbPlantNM.Text)
				{
					//工場コンボボックスになければ追加
					if (this.cmbbLineCategory.Items.Contains(tmLineInfo.LineCate_NM.Trim()) == false)
					{
						this.cmbbLineCategory.Items.Insert(i, tmLineInfo.LineCate_NM.Trim());
						i = i + 1;
					}
				}

			//初期値として先頭を選択
			if (this.cmbbLineCategory.Items.Count > 0)
			{
				//cmbbLineCategory.SelectedIndex = 0;
				cmbbLineCategory.Text = cmbbLineCategory.Items[0].ToString();
			}
		}

        //カテゴリの項目セット
		private void SetCmbbLineCategory(List<TmLINEInfo> tmLineInfo)
        {
            if (cmbbPlantNM.SelectedIndex == -1)
            {
                return;
            }

            this.cmbbLineCategory.Items.Clear();

            //ListTmLINEInfo([インライン番号/工場名/カテゴリ/ライン名]リスト)から
            //重複を省いてコンボボックスのアイテムに追加
            int i = 0;
            foreach (TmLINEInfo wTmLINEInfo in tmLineInfo)
            {
				////選択されている工場と同じであれば
				if (wTmLINEInfo.Plant_NM == cmbbPlantNM.Text)
				{
                    //工場コンボボックスになければ追加
                    if (this.cmbbLineCategory.Items.Contains(wTmLINEInfo.LineCate_NM.Trim()) == false)
                    {
                        this.cmbbLineCategory.Items.Add(wTmLINEInfo.LineCate_NM.Trim());

                        i = i + 1;
                    }
				}

            }
            //初期値として先頭を選択
            if (this.cmbbLineCategory.Items.Count > 0)
            {
                //cmbbLineCategory.SelectedIndex = 0;
                cmbbLineCategory.Text = cmbbLineCategory.Items[0].ToString();
            }

            /*
            string sqlCmdTxt = "SELECT DISTINCT [LineCate_NM] FROM [TmLINE] WITH(NOLOCK) WHERE Plant_NM=@Plant_NM AND Del_FG <> '1' ORDER BY [LineCate_NM]  ASC";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {

                SqlParameter param = new SqlParameter("@Plant_NM", DbType.String);
                param.Value = cmbbPlantNM.SelectedText;
                connect.Command.Parameters.Add(param);

                SqlDataReader rd = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    rd = connect.Command.ExecuteReader();

                    int i = 0;
                    while (rd.Read())
                    {
                        //nvc.Add(Convert.ToString(rd["Inline_CD"]).Trim(), Convert.ToString(rd["Inline_NM"]).Trim());
                        this.cmbbLineCategory.Items.Insert(i, Convert.ToString(rd["LineCate_NM"]).Trim());
                        i = i + 1;
                    }
                }
                finally
                {
                    if (rd != null) rd.Close();
                    connect.Close();
                }
            }
            */

        }

        /// <summary>
        /// LineNOコンボボックス(設定ファイルからラインを取得する)
        /// </summary>
        private void SetCmbbLineNo()
        {
            //カテゴリが選べれていなければ、ラインの特定は出来ないので、return
            if (cmbbLineCategory.SelectedIndex == -1)
            {
                return;
            }
            this.cmbbLineNo.Items.Clear();

            //ListTmLINEInfo([インライン番号/工場名/カテゴリ/ライン名]リスト)から
            //重複を省いてコンボボックスのアイテムに追加

			//foreach (SettingInfo settingInfo in Constant.settingInfoList)
			//{
			//    foreach (TmLINEInfo wTmLineInfo in TmLINEInfoList)
			//    {
			//        if (wTmLineInfo.Inline_CD == settingInfo.InlineCD)
			//        {
			//            this.cmbbLineNo.Items.Insert(i++, wTmLineInfo.Inline_NM);
			//        }
			//    }
			//}
			////初期値として先頭を選択
			//if (this.cmbbLineNo.Items.Count > 0)
			//{
			//    //cmbbLineNo.SelectedIndex = 0;
			//    cmbbLineNo.Text = cmbbLineNo.Items[0].ToString();
			//}

			foreach (SettingInfo settingInfo in Constant.settingInfoList)
			{

				if (TmLINEInfoList.Exists(wtm => wtm.Inline_CD == settingInfo.InlineCD))
				{
					this.cmbbLineNo.Items.Add(TmLINEInfoList.Find(wtm => wtm.Inline_CD == settingInfo.InlineCD).Inline_NM);
				}

			}

			if (this.cmbbLineNo.Items.Count == 0)
			{
				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_46));
			}
        }

		/// <summary>
        /// LineNOコンボボックス(管理者モード用)
        /// </summary>
		private void SetCmbbLineNo(List<TmLINEInfo> ListTmLINEInfo)
		{
			//カテゴリが選べれていなければ、ラインの特定は出来ないので、return
			if (cmbbLineCategory.SelectedIndex == -1)
			{
				return;
			}
			this.cmbbLineNo.Items.Clear();

			int i = 0;
			foreach (TmLINEInfo wTmLINEInfo in ListTmLINEInfo)
			{
				//選択されている[工場][カテゴリ]が同じであれば
				if (wTmLINEInfo.Plant_NM == cmbbPlantNM.Text && wTmLINEInfo.LineCate_NM == cmbbLineCategory.Text)
				{
					//コンボボックスになければ追加
					if (this.cmbbLineNo.Items.Contains(wTmLINEInfo.Inline_NM.Trim()) == false)
					{
						this.cmbbLineNo.Items.Insert(i, wTmLINEInfo.Inline_NM.Trim());
						i = i + 1;
					}
				}
			}
		}

//        private List<TmLINEInfo> GetTmLINEInfo(bool packageFG, int lineCD) 
//        {
//            List<TmLINEInfo> wListTmLINEInfo = new List<TmLINEInfo>();

//            string sqlCmdTxt = @"SELECT Inline_CD,Inline_NM,Plant_NM,LineCate_NM
//                                FROM TmLINE
//                                Where Del_FG=0";

//            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
//            {
//                if (packageFG) 
//                {
//                    sqlCmdTxt += " AND (Inline_CD = @Line_CD) ";
//                    SqlParameter param = new SqlParameter("@Line_CD", SqlDbType.Int);
//                    param.Value = lineCD;
//                    connect.Command.Parameters.Add(param);
//                }

//                SqlDataReader rd = null;
//                try
//                {
//                    connect.Command.CommandText = sqlCmdTxt;
//                    rd = connect.Command.ExecuteReader();

//                    while (rd.Read())
//                    {
//                        TmLINEInfo wTmLINEInfo=new TmLINEInfo();
//                        wTmLINEInfo.Inline_CD=Convert.ToInt32(rd["Inline_CD"]);
//                        wTmLINEInfo.Inline_NM = Convert.ToString(rd["Inline_NM"]).Trim();
//                        wTmLINEInfo.Plant_NM = Convert.ToString(rd["Plant_NM"]).Trim();
//                        wTmLINEInfo.LineCate_NM = Convert.ToString(rd["LineCate_NM"]).Trim();
//                        wListTmLINEInfo.Add(wTmLINEInfo);
//                    }
//                }
//                finally
//                {
//                    if (rd != null) rd.Close();
//                    connect.Close();
//                }
//            }
//            return wListTmLINEInfo;
//        }

        /*
        private void SetCmbbType()
        {
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
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }
        */
        private void InitForm()
        {
#if TEST
#else
            if (!Constant.fClient && Constant.settingInfoList.Count == 1)
            {
                //中間サーバモード
                toolbtnReport.Visible = false;
            }
			else if (!Constant.fClient && Constant.settingInfoList.Count > 1)
			{
				toolbtnReport.Visible = false;
			}
			else
			{
				this.Text += Constant.MessageInfo.Message_78;
				//this.Text = "GEICS2" + Constant.MessageInfo.Message_78;
			}
#endif		
			TmLINEInfoList = new List<TmLINEInfo>();

			//設定ファイルから情報取得可能な場合
			if (Constant.settingInfoList != null)
			{
				foreach (SettingInfo settingInfo in Constant.settingInfoList)
				{
					//参照するDBを確認
					SetConnectDB();

					Constant.StrQCIL = settingInfo.StrQCIL;
					StrARMS = settingInfo.StrARMS;

					//閾値マスタにあるTypeのみ表示
					//SetCmbbType();

					//<--BTS1482 2011/12/06 Y.Matsushima
					//初回(起動時)のデータベース接続でのみ、工場/カテゴリ/ラインのリストを取得する。
					TmLINEInfoList.AddRange(Line.GetTmLINEInfo(settingInfo.PackageFG, settingInfo.InlineCD));


					//閾値マスタにあるInline_CDのみ表示
					SetCmbbLineNo();
				}

				//工場の項目追加
				SetCmbbPlantNM(TmLINEInfoList);

				//カテゴリの項目追加
				SetCmbbLineCategory(TmLINEInfoList);
				//-->BTS1482 2011/12/06 Y.Matsushima
			}
			//設定ファイルから情報取得出来ない場合（管理者モード）
			else
			{
				//参照するDBを確認
				SetConnectDB();

				//閾値マスタにあるTypeのみ表示
				//SetCmbbType();

				//<--BTS1482 2011/12/06 Y.Matsushima
				//初回(起動時)のデータベース接続でのみ、工場/カテゴリ/ラインのリストを取得する。
				TmLINEInfoList = Line.GetTmLINEInfo(false, 0);

				//工場の項目追加
				SetCmbbPlantNM(TmLINEInfoList);

				//カテゴリの項目追加
				SetCmbbLineCategory(TmLINEInfoList);
				//-->BTS1482 2011/12/06 Y.Matsushima

				//閾値マスタにあるInline_CDのみ表示
				SetCmbbLineNo(TmLINEInfoList);
			}
        }

        private void SetConnectDB()
        {
			int targetLineCD = 0;

            if (cmbbLineNo.Text != "")
            {
                targetLineCD = Com.GetLineNo(cmbbLineNo.Text);
            }

#if TEST
			if (Constant.settingInfoList != null)
			{
				//パッケージされている　且つ　中間サーバーの場合
				if (Constant.settingInfoList[0].PackageFG && Constant.settingInfoList.Count == 1)
				{
                    if (string.IsNullOrEmpty(Constant.settingInfoList[0].EicsConnectionString) == false
                        && Constant.settingInfoList[0].EicsConnectionString.Trim() != "")
                    {
                        Constant.settingInfoList[0].StrQCIL = Constant.settingInfoList[0].EicsConnectionString;
                    }
                    else
                    {
                        Constant.settingInfoList[0].StrQCIL = String.Format(Constant.StrPACKAGE, Constant.settingInfoList[0].PackagePC);//中間サーバーにあるデータベースを利用する。外部DBは見ない。
                    }
                    if (string.IsNullOrEmpty(Constant.settingInfoList[0].ArmsConnectionString) == false 
                        && Constant.settingInfoList[0].ArmsConnectionString.Trim() != "")
                    {
                        Constant.settingInfoList[0].StrARMS = Constant.settingInfoList[0].ArmsConnectionString;
                    }
                    else
                    {
                        Constant.settingInfoList[0].StrARMS = String.Format(Constant.SQLite_ARMS, Constant.settingInfoList[0].PackagePC);
                    }
				}
				else if (!Constant.settingInfoList[0].PackageFG && Constant.settingInfoList.Count == 1)
				{
					Constant.settingInfoList[0].StrARMS = ConnectQCIL.GetARMSConnString(Constant.settingInfoList[0].InlineCD);
				}
				else if (Constant.settingInfoList.Count > 1)
				{
					foreach (SettingInfo settingInfo in Constant.settingInfoList)
					{
						if (settingInfo.PackageFG)
						{
                            if (string.IsNullOrEmpty(settingInfo.EicsConnectionString) == false && settingInfo.EicsConnectionString.Trim() != "")
                            {
                                settingInfo.StrQCIL = settingInfo.EicsConnectionString;
                            }
                            else
                            {
                                settingInfo.StrQCIL = String.Format(Constant.StrPACKAGE, settingInfo.PackagePC);
                            }
                            if (string.IsNullOrEmpty(settingInfo.ArmsConnectionString) == false && settingInfo.ArmsConnectionString.Trim() != "")
                            {
                                Constant.settingInfoList[0].StrARMS = settingInfo.ArmsConnectionString;
                            }
                            else
                            {
                                settingInfo.StrARMS = String.Format(Constant.SQLite_ARMS, settingInfo.PackagePC);
                            }

                            //settingInfo.StrARMS = String.Format(Constant.SQLite_ARMS, settingInfo.PackagePC);
						}
					}
				}
			}
			else
			{
                if (string.IsNullOrEmpty(Constant.StrARMS) == false && Constant.StrARMS.Trim() != "")
                {
                    F04_ErrorRecord.StrARMS = Constant.StrARMS;
                }
                else
                {
                    F04_ErrorRecord.StrARMS = ConnectQCIL.GetARMSConnString(targetLineCD);
                }
            }
#else
            if (Constant.settingInfoList != null)
			{
				//パッケージされている　且つ　中間サーバーの場合
				if (Constant.settingInfoList[0].PackageFG && Constant.settingInfoList.Count == 1)
				{
                    if (string.IsNullOrEmpty(Constant.settingInfoList[0].EicsConnectionString) == false
                        && Constant.settingInfoList[0].EicsConnectionString.Trim() != "")
                    {
                        Constant.settingInfoList[0].StrQCIL = Constant.settingInfoList[0].EicsConnectionString;
                    }
                    else
                    {
                        Constant.settingInfoList[0].StrQCIL = String.Format(Constant.StrPACKAGE, Constant.settingInfoList[0].PackagePC);//中間サーバーにあるデータベースを利用する。外部DBは見ない。
                    }
                    if (string.IsNullOrEmpty(Constant.settingInfoList[0].ArmsConnectionString) == false 
                        && Constant.settingInfoList[0].ArmsConnectionString.Trim() != "")
                    {
                        Constant.settingInfoList[0].StrARMS = Constant.settingInfoList[0].ArmsConnectionString;
                    }
                    else
                    {
                        Constant.settingInfoList[0].StrARMS = String.Format(Constant.SQLite_ARMS, Constant.settingInfoList[0].PackagePC);
                    }
				}
				else if (!Constant.settingInfoList[0].PackageFG && Constant.settingInfoList.Count == 1)
				{
					Constant.settingInfoList[0].StrARMS = ConnectQCIL.GetARMSConnString(Constant.settingInfoList[0].InlineCD);
				}
				else if (Constant.settingInfoList.Count > 1)
				{
					foreach (SettingInfo settingInfo in Constant.settingInfoList)
					{
						if (settingInfo.PackageFG)
						{
                            if (string.IsNullOrEmpty(settingInfo.EicsConnectionString) == false && settingInfo.EicsConnectionString.Trim() != "")
                            {
                                settingInfo.StrQCIL = settingInfo.EicsConnectionString;
                            }
                            else
                            {
                                settingInfo.StrQCIL = String.Format(Constant.StrPACKAGE, settingInfo.PackagePC);
                            }
                            if (string.IsNullOrEmpty(settingInfo.ArmsConnectionString) == false && settingInfo.ArmsConnectionString.Trim() != "")
                            {
                                Constant.settingInfoList[0].StrARMS = settingInfo.ArmsConnectionString;
                            }
                            else
                            {
                                settingInfo.StrARMS = String.Format(Constant.SQLite_ARMS, settingInfo.PackagePC);
                            }

                            //settingInfo.StrARMS = String.Format(Constant.SQLite_ARMS, settingInfo.PackagePC);
                        }
                    }
				}
			}
			else
			{
                if (string.IsNullOrEmpty(Constant.StrARMS) == false && Constant.StrARMS.Trim() != "")
                {
                    F04_ErrorRecord.StrARMS = Constant.StrARMS;
                }
                else
                {
                    F04_ErrorRecord.StrARMS = ConnectQCIL.GetARMSConnString(targetLineCD);
                }
            }
#endif

        }

        //現在時間-24HへSet
        private void SetStartDate()
        {
            dtpStart.Value = System.DateTime.Now.AddDays(-1);
        }

        //現在時間へSet
        private void SetEndDate()
        {
            dtpEnd.Value = System.DateTime.Now;
        }

		public void ResetForm()
		{
			dsTvQCNR.Clear();

			if (Common.IsNotUseQDIW())
			{
				ResetForm_Map();
			}
			else
			{
				ResetFormUseQDIW();
			}

            gvQCErrList.Columns["DBMachine_NM"].Visible = this.chkViewDBWBMachine.Checked;
            gvQCErrList.Columns["WBMachine_NM"].Visible = this.chkViewDBWBMachine.Checked;
        }

        public void ResetForm_Map()
        {
			try
			{
				List<ErrRecord> dataList = ErrRecord.GetData(dtpStart.Value, dtpEnd.Value, rbAllData.Checked, this.chkViewDBWBMachine.Checked);

                // 2015.12.09 永尾修正 TnQCNRに測定日のずれのレコードが重複作成される為、重複レコードを表対象から省く。
                for (int i = dataList.Count - 1; i >= 0; i--)
                {
                    bool RemoveFG = false;

                    ErrRecord er0 = dataList[i];

                    if (er0.Inspection_NO == 2086 || er0.Inspection_NO == 2087)
                    {

                        List<ErrRecord> otherList = dataList.Where(o => o != er0).ToList();
                        List<ErrRecord> oldList = otherList.Where(o => o.Inline_CD == er0.Inline_CD && o.Equipment_NO == er0.Equipment_NO && o.NascaLot_NO == er0.NascaLot_NO
                                                                    && o.Defect_NO == er0.Defect_NO && o.Inspection_NO == er0.Inspection_NO && o.Process_NO == er0.Process_NO
                                                                    && o.Timing_NO == er0.Timing_NO && o.Multi_NO == er0.Multi_NO && o.Message == er0.Message).ToList();
                        if (oldList.Count > 0)
                        {
                            foreach (ErrRecord er1 in oldList)
                            {
                                DateTime Measure_DT0 = er0.Measure_DT;
                                DateTime Measure_DT1 = er1.Measure_DT;

                                if ((Measure_DT0 - Measure_DT1) <= new TimeSpan(1, 0, 0))
                                {
                                    dataList.Remove(er0);
                                    RemoveFG = true;
                                    break;
                                }
                            }
                        }
                        
                        //  2016.01.25 永尾修正  2015.12.09の修正では、「エラー未対応のみ」の出力時に不要データを削除できなかった為、追加対応。
                        //  抽出した全レコードのエラー未対応のレコードに対して、同じデータで1日以内に対応済みデータがある時、出力対象外とする。
                        if (RemoveFG)
                        {
                            continue;
                        }

                        try
                        {
                            if (er0.Check_NO == 0)
                            {
                                string sEqui_NO = er0.Equipment_NO;
                                if (!string.IsNullOrEmpty(sEqui_NO))
                                {
                                    string[] lEqui_NO = sEqui_NO.Split('(');
                                    if (lEqui_NO.Length >= 2)
                                    {
                                        sEqui_NO = lEqui_NO[1].Replace(")", "").Trim();
                                    }
                                    else
                                    {
                                        sEqui_NO = null;
                                    }
                                }

                                oldList = ErrRecord.GetData(er0.Inline_CD, sEqui_NO, er0.NascaLot_NO, er0.Defect_NO, er0.Inspection_NO,
                                                                            null, null, er0.Multi_NO, er0.Message,
                                                                            er0.Measure_DT.AddDays(-1.0), er0.Measure_DT.AddDays(1.0), 1,
                                                                            this.chkViewDBWBMachine.Checked).ToList();
                                if (oldList.Count > 0)
                                {
                                    dataList.Remove(er0);
                                }
                            }
                        }
                        catch (Exception err)
                        {

                        }
                    }
                }

				//dsTvQCNR.TvQCNR. = dataList.ToSortableBindingList().ToArray();

				gvQCErrList.DataSource = dataList.ToSortableBindingList();
			}
			catch (Exception err)
			{
				MessageBox.Show(Constant.MessageInfo.Message_47 + err.ToString());
			}
			finally
			{
				fDrawComplete = true;
			}

			//処理時間が半端無いので、上記に改善 2015/8/15 n.yoshimoto ResetFormUseQDIWの方もいずれ
			#region 改善の為、下記廃止

//            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
//            {
//                try
//                {
//                    string sql = @" SELECT {0} TvQCNR.QCNR_NO, TvQCNR.Inline_CD, TmLINE.Inline_NM, TvQCNR.Equipment_NO, TvQCNR.NascaLot_NO, TvQCNR.Defect_NO, TvQCNR.Inspection_NO, TvQCNR.Inspection_NM,
//                                TvQCNR.Process_NO, TvQCNR.Timing_NO, TvQCNR.Timing_NM, TvQCNR.Multi_NO, TvQCNR.Measure_DT, TvQCNR.Message, TvQCNR.Type_CD, TvQCNR.BackNum_NO, TvQCNR.Check_NO, TnQCNRCnfm.Confirm_NM, TvQCNR.UpdUser_CD, TvQCNR.LastUpd_DT, TmEQUI.Assets_NM 
//                                FROM dbo.TvQCNR_Map AS TvQCNR WITH(NOLOCK) 
//                                INNER JOIN TmLINE WITH(NOLOCK) ON TvQCNR.Inline_CD = TmLINE.Inline_CD
//                                LEFT OUTER JOIN TmEQUI WITH(NOLOCK) ON TvQCNR.Equipment_NO = TmEQUI.Equipment_NO
//                                LEFT OUTER JOIN 
//                                    (SELECT Inline_CD, QCNR_NO, MAX(CONVERT(int, Confirm_NO)) AS Confirm_NO, Confirm_NM, Product_FG, Operator_NM 
//                                        FROM dbo.TnQCNRCnfm AS TnQCNRCnfm WITH(NOLOCK) 
//                                        WHERE (Del_FG = 0) 
//                                        GROUP BY Inline_CD, QCNR_NO, Confirm_NM, Product_FG, Operator_NM) AS TnQCNRCnfm 
//                                ON TvQCNR.QCNR_NO = TnQCNRCnfm.QCNR_NO 
//                                AND TvQCNR.Inline_CD = TnQCNRCnfm.Inline_CD 
//                                WHERE (TvQCNR.Inline_CD = @LINECD)";

//                    if (rbAllData.Checked)
//                    {
//                        sql = string.Format(sql, "");
//                        sql += " AND (Measure_DT >= @FROMDT AND Measure_DT <= @TODT AND Check_NO = 1) OR (TvQCNR.Inline_CD = @LINECD AND Check_NO = 0) ";
//                        connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
//                    }
//                    else 
//                    {
//                        sql = string.Format(sql, "TOP 1000");
//                        sql += " AND (Check_NO = 0) ";
//                        connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
//                    }

//                    connect.Command.Parameters.Add("@FROMDT", SqlDbType.DateTime).Value = dtpStart.Value;
//                    connect.Command.Parameters.Add("@TODT", SqlDbType.DateTime).Value = dtpEnd.Value;
//                    connect.Command.Parameters.Add("@LINECD", SqlDbType.Int).Value = Constant.nLineCD;

//                    connect.Command.CommandText = sql;
//                    using (SqlDataReader rd = connect.Command.ExecuteReader())
//                    {
//                        int ordQcnrNo = rd.GetOrdinal("QCNR_NO");
//                        int ordLineCd = rd.GetOrdinal("Inline_CD");
//                        int ordLineNm = rd.GetOrdinal("Inline_NM");
//                        int ordEquipNo = rd.GetOrdinal("Equipment_NO");
//                        int ordLotNo = rd.GetOrdinal("NascaLot_NO");
//                        int ordDefNo = rd.GetOrdinal("Defect_NO");
//                        int ordInspectNo = rd.GetOrdinal("Inspection_NO");
//                        int ordInspectNm = rd.GetOrdinal("Inspection_NM");
//                        int ordProcNo = rd.GetOrdinal("Process_NO");
//                        int ordTimNo = rd.GetOrdinal("Timing_NO");
//                        int ordTimNm = rd.GetOrdinal("Timing_NM");
//                        int ordMultiNo = rd.GetOrdinal("Multi_NO");
//                        int ordMeasureDt = rd.GetOrdinal("Measure_DT");
//                        int ordMsg = rd.GetOrdinal("Message");
//                        int ordTypeCd = rd.GetOrdinal("Type_CD");
//                        int ordBackNumNo = rd.GetOrdinal("BackNum_NO");
//                        int ordChkNo = rd.GetOrdinal("Check_NO");
//                        int ordUpdUserCd = rd.GetOrdinal("UpdUser_CD");
//                        int ordLastUpdDt = rd.GetOrdinal("LastUpd_DT");
//                        int ordConfirmNm = rd.GetOrdinal("Confirm_NM");
//                        int ordAssetsNm = rd.GetOrdinal("Assets_NM");
//                        //int ordMagNo = rd.GetOrdinal("Magazine_NO");
//                        //int ordDBMacNm = rd.GetOrdinal("DBMachine_NM");
//                        //int ordWBMacNm = rd.GetOrdinal("WBMachine_NM");

//                        while (rd.Read())
//                        {
//                            string sInspectionNM = rd.GetString(ordInspectNm).Trim();

//                            if (sInspectionNM.Substring(0, 1) == "F")
//                            {
//                                sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
//                            }

//                            string sEquipmentNO = rd.GetString(ordEquipNo).Trim();
//                            sEquipmentNO = Com.AddCommentEquipmentNO(sEquipmentNO);

//                            DataRow wDataRow = dsTvQCNR.TvQCNR.NewRow();

//                            wDataRow["QCNR_NO"] = rd.GetInt32(ordQcnrNo);
//                            int lineNO = rd.GetInt32(ordLineCd);
//                            wDataRow["Inline_CD"] = lineNO;
//                            wDataRow["Inline_NM"] = rd.GetString(ordLineNm).Trim();
//                            wDataRow["Equipment_NO"] = sEquipmentNO;
//                            string lotno = rd.GetString(ordLotNo).Trim();
//                            wDataRow["NascaLot_NO"] = rd.GetString(ordLotNo).Trim();
//                            wDataRow["Defect_NO"] = rd.GetInt32(ordDefNo); ;
//                            wDataRow["Defect_NM"] = "";
//                            wDataRow["Inspection_NO"] = rd.GetInt32(ordInspectNo);
//                            wDataRow["Inspection_NM"] = sInspectionNM;
//                            wDataRow["Process_NO"] = rd.GetString(ordProcNo).Trim();
//                            wDataRow["Timing_NO"] = rd.GetString(ordTimNo).Trim();
//                            wDataRow["Timing_NM"] = rd.GetString(ordTimNm).Trim();
//                            wDataRow["Multi_NO"] = rd.GetInt32(ordMultiNo);
//                            wDataRow["Measure_DT"] = rd.GetDateTime(ordMeasureDt);
//                            wDataRow["Message"] = rd.GetString(ordMsg).Replace("\r\n", "。").Trim();
//                            wDataRow["Type_CD"] = rd.GetString(ordTypeCd).Trim();
//                            wDataRow["BackNum_NO"] = rd.GetInt32(ordBackNumNo);
//                            wDataRow["Check_NO"] = rd.GetInt32(ordChkNo);
//                            wDataRow["UpdUser_CD"] = rd.GetString(ordUpdUserCd).Trim();
//                            wDataRow["LastUpd_DT"] = rd.GetDateTime(ordLastUpdDt);

//                            if (!rd.IsDBNull(ordConfirmNm))
//                            {
//                                wDataRow["Confirm_NM"] = rd.GetString(ordConfirmNm).Replace("\r\n", "。").Trim();
//                            }

//                            if (!rd.IsDBNull(ordAssetsNm))
//                            {
//                                wDataRow["Assets_NM"] = rd.GetString(ordAssetsNm).Trim();
//                            }
//                            wDataRow["Magazine_NO"] = Database.Log.GetMagazineNO(lotno, sEquipmentNO, lineNO, dtpStart.Value, dtpEnd.Value);
//                            wDataRow["DBMachine_NM"] = Database.Lott.GetDBMachineNames(lineNO, lotno);
//                            wDataRow["WBMachine_NM"] = Database.Lott.GetWBMachineNames(lineNO, lotno);

//                            //wDataRow["QCNR_NO"] = rd["QCNR_NO"];
//                            //int lineNO = Convert.ToInt32(rd["Inline_CD"]);
//                            //wDataRow["Inline_CD"] = lineNO;
//                            //wDataRow["Inline_NM"] = rd["Inline_NM"];
//                            //wDataRow["Equipment_NO"] = sEquipmentNO;
//                            //string lotno = Convert.ToString(rd["NascaLot_NO"]).Trim();
//                            //wDataRow["NascaLot_NO"] = lotno;
//                            //wDataRow["Defect_NO"] = rd["Defect_NO"];
//                            //wDataRow["Defect_NM"] = "";
//                            //wDataRow["Inspection_NO"] = rd["Inspection_NO"];
//                            //wDataRow["Inspection_NM"] = sInspectionNM;
//                            //wDataRow["Process_NO"] = rd["Process_NO"];
//                            //wDataRow["Timing_NO"] = rd["Timing_NO"];
//                            //wDataRow["Timing_NM"] = rd["Timing_NM"];
//                            //wDataRow["Multi_NO"] = rd["Multi_NO"];
//                            //wDataRow["Measure_DT"] = rd["Measure_DT"];
//                            //wDataRow["Message"] = rd["Message"].ToString().Replace("\r\n", "。");
//                            //wDataRow["Type_CD"] = rd["Type_CD"];
//                            //wDataRow["BackNum_NO"] = rd["BackNum_NO"];
//                            //wDataRow["Check_NO"] = rd["Check_NO"];
//                            //wDataRow["UpdUser_CD"] = rd["UpdUser_CD"];
//                            //wDataRow["LastUpd_DT"] = rd["LastUpd_DT"];
//                            //wDataRow["Confirm_NM"] = rd["Confirm_NM"].ToString().Replace("\r\n", "。");
//                            //if (!rd.IsDBNull(rd.GetOrdinal("Assets_NM")))
//                            //{
//                            //    wDataRow["Assets_NM"] = rd.GetString(rd.GetOrdinal("Assets_NM")).Trim();
//                            //}
//                            //wDataRow["Magazine_NO"] = Database.Log.GetMagazineNO(lotno, sEquipmentNO, lineNO, dtpStart.Value, dtpEnd.Value);
//                            //wDataRow["DBMachine_NM"] = Database.Lott.GetDBMachineNames(lineNO, lotno);
//                            //wDataRow["WBMachine_NM"] = Database.Lott.GetWBMachineNames(lineNO, lotno);
//                            dsTvQCNR.TvQCNR.Rows.Add(wDataRow);

//                            //<--Start 2010.03.09 応答なし回避
//                            // メッセージ・キューにあるWindowsメッセージをすべて処理する
//                            //Application.DoEvents();
//                            //-->End 2010.03.09 応答なし回避
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.ToString());
//                    MessageBox.Show(Constant.MessageInfo.Message_47 + ex.ToString());
//                }
//                finally
//                {
//                    connect.Close();

//                    //<--Start 2010.03.09 応答なし回避
//                    fDrawComplete = true;
//                    //-->End 2010.03.09 応答なし回避
//                }
			//            }
			#endregion
		}

        public void ResetFormUseQDIW()
        {
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                try
                {
                    string sql = @" SELECT TvQCNR.QCNR_NO, TvQCNR.Inline_CD, TmLINE.Inline_NM, TvQCNR.Equipment_NO, TvQCNR.NascaLot_NO, TvQCNR.Defect_NO, TvQCNR.Defect_NM, TvQCNR.Inspection_NO, TvQCNR.Inspection_NM,
                                TvQCNR.Process_NO, TvQCNR.Timing_NO, TvQCNR.Timing_NM, TvQCNR.Multi_NO, TvQCNR.Measure_DT, TvQCNR.Message, TvQCNR.Type_CD, TvQCNR.BackNum_NO, TvQCNR.Check_NO, TnQCNRCnfm.Confirm_NM, TvQCNR.UpdUser_CD, TvQCNR.LastUpd_DT, TmEQUI.Assets_NM
                                FROM dbo.TvQCNR AS TvQCNR WITH(NOLOCK) 
                                INNER JOIN TmLINE ON TvQCNR.Inline_CD = TmLINE.Inline_CD
                                LEFT OUTER JOIN TmEQUI ON TvQCNR.Equipment_NO = TmEQUI.Equipment_NO
                                LEFT OUTER JOIN 
                                    (SELECT Inline_CD, QCNR_NO, MAX(CONVERT(int, Confirm_NO)) AS Confirm_NO, Confirm_NM, Product_FG, Operator_NM 
                                        FROM dbo.TnQCNRCnfm AS TnQCNRCnfm
                                        WHERE (Del_FG = 0) 
                                        GROUP BY Inline_CD, QCNR_NO, Confirm_NM, Product_FG, Operator_NM) AS TnQCNRCnfm 
                                ON TvQCNR.QCNR_NO = TnQCNRCnfm.QCNR_NO 
                                AND TvQCNR.Inline_CD = TnQCNRCnfm.Inline_CD 
                                WHERE (TvQCNR.Inline_CD = @LINECD)";

                    if (rbAllData.Checked)
                    {
						sql += " AND (Measure_DT >= @FROMDT AND Measure_DT <= @TODT AND Check_NO = 1) OR (TvQCNR.Inline_CD = @LINECD AND Check_NO = 0) ";
                        connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
                    }
                    else
                    {
                        sql += " AND (Check_NO = 0) ";
                        connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
                    }

                    connect.Command.Parameters.Add("@FROMDT", SqlDbType.DateTime).Value = dtpStart.Value;
                    connect.Command.Parameters.Add("@TODT", SqlDbType.DateTime).Value = dtpEnd.Value;
                    connect.Command.Parameters.Add("@LINECD", SqlDbType.Int).Value = Common.nLineCD;

                    connect.Command.CommandText = sql;
                    using (SqlDataReader reader = connect.Command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
                            if (sInspectionNM.Substring(0, 1) == "F")
                            {
                                sInspectionNM = Com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
                            }

                            DataRow wDataRow = dsTvQCNR.TvQCNR.NewRow();

                            wDataRow["QCNR_NO"] = reader["QCNR_NO"];

                            int lineNO = Convert.ToInt32(reader["Inline_CD"]);
                            wDataRow["Inline_CD"] = lineNO;
                            wDataRow["Inline_NM"] = reader["Inline_NM"];
                            string sEquipmentNO = Convert.ToString(reader["Equipment_NO"]).Trim();
                            wDataRow["Equipment_NO"] = Com.AddCommentEquipmentNO(sEquipmentNO);
                            string lotno = Convert.ToString(reader["NascaLot_NO"]).Trim();
                            wDataRow["NascaLot_NO"] = lotno;
                            wDataRow["Defect_NO"] = reader["Defect_NO"];
							if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
                            {
                                wDataRow["Defect_NM"] = "";
                            }
                            else
                            {
                                wDataRow["Defect_NM"] = reader["Defect_NM"];
                            }
                            wDataRow["Inspection_NO"] = reader["Inspection_NO"];
                            wDataRow["Inspection_NM"] = sInspectionNM;
                            wDataRow["Process_NO"] = reader["Process_NO"];
                            wDataRow["Timing_NO"] = reader["Timing_NO"];
                            wDataRow["Timing_NM"] = reader["Timing_NM"];
                            wDataRow["Multi_NO"] = reader["Multi_NO"];
                            wDataRow["Measure_DT"] = reader["Measure_DT"];
                            wDataRow["Message"] = reader["Message"].ToString().Replace("\r\n", "。");
                            wDataRow["Type_CD"] = reader["Type_CD"];
                            wDataRow["BackNum_NO"] = reader["BackNum_NO"];
                            wDataRow["Check_NO"] = reader["Check_NO"];
                            wDataRow["UpdUser_CD"] = reader["UpdUser_CD"];
                            wDataRow["LastUpd_DT"] = reader["LastUpd_DT"];
                            wDataRow["Confirm_NM"] = reader["Confirm_NM"].ToString().Replace("\r\n", "。");
                            if (!reader.IsDBNull(reader.GetOrdinal("Assets_NM")))
                            {
                                wDataRow["Assets_NM"] = reader.GetString(reader.GetOrdinal("Assets_NM")).Trim();
                            }
                            wDataRow["Magazine_NO"] = Database.Log.GetMagazineNO(lotno, sEquipmentNO, lineNO, dtpStart.Value, dtpEnd.Value);
                            if (this.chkViewDBWBMachine.Checked)
                            {
                                wDataRow["DBMachine_NM"] = Database.Lott.GetDBMachineNames(lineNO, lotno);
                                wDataRow["WBMachine_NM"] = Database.Lott.GetWBMachineNames(lineNO, lotno);
                            }
                            dsTvQCNR.TvQCNR.Rows.Add(wDataRow);
                            //<--Start 2010.03.09 応答なし回避
                            // メッセージ・キューにあるWindowsメッセージをすべて処理する
                            Application.DoEvents();
                            //-->End 2010.03.09 応答なし回避
                        }
                    }
                    if (dsTvQCNR.TvQCNR.Count > 0)
                    {
                        tvQCNRBindingSource.DataSource = dsTvQCNR;
                        gvQCErrList.DataSource = tvQCNRBindingSource;
                    }
                    else
                    {
                        gvQCErrList.DataSource = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Constant.MessageInfo.Message_47 + ex.ToString());
                }
                finally
                {
                    connect.Close();

                    //<--Start 2010.03.09 応答なし回避
                    fDrawComplete = true;
                    //-->End 2010.03.09 応答なし回避
                }
            }
        }

        private void gvQCErrList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            int nLineCD=0;
            int QcnrNO = 0;
            int DefectNO = 0;
            string DefectNM="";
            string LotNO = "";
            int BackNum = 0;
            string Type="";
            string InspectionNM = "";
            string Result = "";
            int MultiNO = 0;
            string sEquiNO = "";
            string sAssetsNM = "";
            DateTime dtMeasure = Convert.ToDateTime("9999/01/01");
            int InspectionNO=0;

            DataGridView dgv = (DataGridView)sender;

            //nLineCD = Convert.ToInt32(cmbbLineNo.Text.Trim());
            nLineCD = Common.nLineCD;

            DefectNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["Defect_NO"].Value);              //DefectNO = Convert.ToInt32(gvQCErrList[3, e.RowIndex].Value);               //Defect_NO
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
            {
                DefectNM = "";
                gvQCErrList.Columns["Defect_NM"].Visible = false;
            }else{
                DefectNM = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Defect_NM"].Value);             //DefectNM = Convert.ToString(gvQCErrList[4, e.RowIndex].Value);
            }

            InspectionNM = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Inspection_NM"].Value);     //InspectionNM = Convert.ToString(gvQCErrList[5, e.RowIndex].Value).Trim();   //Inspection_NM
            if (InspectionNM.Substring(0, 1) == "F")
            {
                InspectionNM=InspectionNM.Substring(0, 10);
            }
            Result = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Message"].Value);                 //Result = Convert.ToString(gvQCErrList[6, e.RowIndex].Value).Trim();         //異常メッセージ
            LotNO = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["NascaLot_NO"].Value);              //LotNO = Convert.ToString(gvQCErrList[7, e.RowIndex].Value).Trim();          //Lot
            dtMeasure = Convert.ToDateTime(gvQCErrList.Rows[e.RowIndex].Cells["Measure_DT"].Value);         //dtMeasure = Convert.ToDateTime(gvQCErrList[8, e.RowIndex].Value);           //dtMeasure
            //dtMeasure = Convert.ToDateTime(gvQCErrList[15, e.RowIndex].Value);           //dtMeasure
            sEquiNO = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Equipment_NO"].Value);           //sEquiNO = Convert.ToString(gvQCErrList[9, e.RowIndex].Value).Trim();        //Equipment_NO
            //sAssetsNM = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Timing_NM"].Value).Trim();     //sAssetsNM = Convert.ToString(gvQCErrList[10, e.RowIndex].Value).Trim();     //Assets_NM
            sAssetsNM = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Assets_NM"].Value).Trim();     //sAssetsNM = Convert.ToString(gvQCErrList[10, e.RowIndex].Value).Trim();     //Assets_NM
            Type = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Type_CD"].Value).Trim();            //Type = Convert.ToString(gvQCErrList[11, e.RowIndex].Value).Trim();          //Type
            //BackNum = Convert.ToInt32(gvQCErrList[12, e.RowIndex].Value);               //BackNum(=遡るデータ数)
            BackNum = Convert.ToInt32(txtbGraphPlotNum.Text);                           //BackNum(=遡るデータ数)
            MultiNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["Multi_NO"].Value);                //MultiNO = Convert.ToInt32(gvQCErrList[13, e.RowIndex].Value);               //単/複数号機
            InspectionNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["Inspection_NO"].Value);      //InspectionNO = Convert.ToInt32(gvQCErrList[16, e.RowIndex].Value);

            QcnrNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["QCNR_NO"].Value);                  //QcnrNO = Convert.ToInt32(gvQCErrList[2, e.RowIndex].Value);     //QCNR_NO
            int nCnfmNO = Com.GetTnQCNRCnfm_CnfmNO(nLineCD, QcnrNO);        //Confirm_NO
            int nCheckNO = Convert.ToInt32(gvQCErrList.Rows[e.RowIndex].Cells["Check_NO"].Value);           //int nCheckNO = Convert.ToInt32(gvQCErrList[17, e.RowIndex].Value);//Check_NO
            string timingNM = Convert.ToString(gvQCErrList.Rows[e.RowIndex].Cells["Timing_NM"].Value);

            //確認ボタンがクリックされた
            if (dgv.Columns[e.ColumnIndex].DisplayIndex == 0)
            {
#if MEASURE_TIME
				Console.WriteLine("確認ボタン1:" + DateTime.Now.ToLongTimeString());
#endif
                F03_TrendChart frmDrawGraphAndList = new F03_TrendChart(nLineCD, DefectNO, DefectNM, LotNO, Type, Result, InspectionNM, dtMeasure.AddMinutes(10), sEquiNO, sAssetsNM, BackNum, MultiNO, InspectionNO, timingNM);
#if MEASURE_TIME
				Console.WriteLine("確認ボタン2:" + DateTime.Now.ToLongTimeString());
#endif
                frmDrawGraphAndList.ShowDialog();
                GC.Collect();

                //dsTvQCNR.Clear();
                //InitForm();
                //ResetForm();
            }

            try
            {
                //対応ボタンがクリックされた
                if (dgv.Columns[e.ColumnIndex].DisplayIndex == 2)
                {
                    //if (sAssetsNM.Contains("ダイボンダー"))
                    //{
                    //    sAssetsNM = "ダイボンダー";
                    //}
                    if (sAssetsNM.Contains("プラズマ"))
                    {
                        sAssetsNM = "プラズマ";
                    }
                    frmQCBackground frmbkgd = new frmQCBackground(nLineCD, QcnrNO, nCnfmNO, sAssetsNM, nCheckNO, sEquiNO, InspectionNO, dtMeasure);
                    frmbkgd.ShowDialog();
                    GC.Collect();

					if (frmbkgd.InputFg)
					{
						ResetForm();
					}

					//dsTvQCNR.Clear();
					////更新ボタンクリック
					//btnUpdate.PerformClick();

                    //InitForm();
                    //if (Form1.fMap == true)
                    //{
                    //    ResetForm_Map();
                    //}
                    //else
                    //{
                    //    ResetForm();
                    //}
                }
            }
            catch { }

        }

        private void gvQCErrList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            //セルの列を確認
            if (Convert.ToBoolean(dgv.Rows[e.RowIndex].Cells["Check_NO"].Value) == false)
            {
                e.CellStyle.BackColor = Color.Salmon;//未確認行は着色
            }
        }

        private void timerSD_Tick(object sender, EventArgs e)
        {
            //中間サーバーであれば
            if (!Constant.fClient)
            {
                //開始のみ更新 1分おき
                SetStartDate();

                //自身のライン情報にコンボボックス初期化
                cmbbPlantNM.Text = myPlant;
                cmbbLineCategory.Text = myCategory;
                cmbbLineNo.Text = myLineName;
            }
        }

        private void timerED_Tick(object sender, EventArgs e)
        {
            //中間サーバーであれば
            if (!Constant.fClient)
            {
                //終了のみ更新 60分おき
                SetEndDate();
            }
        }

        private void btnInlineDisp_Click(object sender, EventArgs e)
        {
			foreach (TmLINEInfo tmLineInfo in TmLINEInfoList)
			{
				if (tmLineInfo.Inline_CD.ToString() == cmbbLineNo.Text)
				{
					//工場とカテゴリを渡す
					Form2 = new F05_MachineErrorRecord(tmLineInfo, cmbbPlantNM.Text, cmbbLineCategory.Text);
					Form2.Show();

					break;
				}
			}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //クライアント起動の場合は、パスワード入力なしで終了可能とする。
            if (this.Text.Contains(Constant.MessageInfo.Message_78))
            {
                return;
            }

            Password frmPassword = new Password();
            frmPassword.ShowDialog();

            switch (frmPassword.DialogResult)
            {
                case DialogResult.OK:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;   //EICS停止なし。
                    break;
            }
        }

        //レポート出力
        private void btnReport_Click(object sender, EventArgs e)
        {
            frmReport = new F07_Report();
            frmReport.Show();
        }

        //マスタ設定ボタン→マスタ設定画面へ
        private void btnMasterSetting_Click(object sender, EventArgs e)
        {
			//frmMasterSetting formMasterSetting = new frmMasterSetting(false,cmbbLineNo.Text);
			//formMasterSetting.ShowDialog();
        }

        //工場変更
        private void cmbbPlantNM_SelectedIndexChanged(object sender, EventArgs e)
        {
			foreach (TmLINEInfo tmLineInfo in TmLINEInfoList)
			{
				if (tmLineInfo.Inline_CD.ToString() == cmbbLineNo.Text)
				{
					//カテゴリ項目再取得
					AddCmbbLineCategory(tmLineInfo);
					break;
				}
			}
        }

        //カテゴリ変更
        private void cmbbLineCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbbLineCategory.SelectedText == "")
            {
                return;
            }
			if (Constant.settingInfoList == null)
			{
				//ライン設定
				SetCmbbLineNo(Line.GetTmLINEInfo(false, 0));
			}
			else
			{
				//ライン設定
				SetCmbbLineNo();
			}

            //参照するDBを変更
            SetConnectDB();
        }

        //ライン変更
        private void cmbbLineNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbbLineNo.SelectedText == "")
            {
                return;
            }

            //参照するDBを確認
            //SetConnectDB();
            Common.nLineCD = Com.GetLineNo(cmbbLineNo.SelectedText.Trim());

			List<TmLINEInfo> lineInfoList = Line.GetTmLINEInfo(false, Common.nLineCD);

            if (lineInfoList.Count > 0)
            {
                Common.notUseTmQdiwFG = lineInfoList.Find(l => l.Inline_CD == Common.nLineCD).NotUseTmQDIW;
            }

            SetConnectDB();

			//設定ファイルからラインの情報を取得している場合
			if (Constant.settingInfoList != null)
			{
				foreach (SettingInfo settingInfo in Constant.settingInfoList)
				{
					if (settingInfo.InlineCD == Common.nLineCD)
					{
						Constant.StrQCIL = settingInfo.StrQCIL;
						StrARMS = settingInfo.StrARMS;
					}
				}
			}

            //更新ボタンクリック
            //btnUpdate.PerformClick();
        }

        private void toolcmbLanguage_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void toolbtnMasterSetting_Click(object sender, EventArgs e)
        {
            M01_Limit formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.Read);
            formMasterLimit.ShowDialog();
        }

        private void toolbtnErrorLog_Click(object sender, EventArgs e)
        {
			foreach (TmLINEInfo tmLineInfo in TmLINEInfoList)
			{
				if (tmLineInfo.Inline_NM == cmbbLineNo.Text)
				{
					//工場とカテゴリを渡す
					Form2 = new F05_MachineErrorRecord(tmLineInfo, cmbbPlantNM.Text, cmbbLineCategory.Text);
					Form2.Show();

					break;
				}
			}


        }

        private void toolbtnReport_Click(object sender, EventArgs e)
        {
            frmReport = new F07_Report();
            frmReport.Show();
        }

        private void toolbtnDefect_Click(object sender, EventArgs e)
        {
            F06_DefectRecord formDefectData = new F06_DefectRecord();
            formDefectData.ShowDialog();
        }

		private void cmbbLineNo_MouseClick(object sender, MouseEventArgs e)
		{
			SetCmbbLineNo(Line.GetTmLINEInfo(false, 0));
		}

		private void cmbbLineCategory_MouseClick(object sender, MouseEventArgs e)
		{
			SetCmbbLineCategory(Line.GetTmLINEInfo(false, 0));
		}

        private void toolbtnPhosphorSheetMasterSetting_Click(object sender, EventArgs e)
        {
            M01_Limit formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.PhosphorSheetRead);
            formMasterLimit.ShowDialog();
        }

        //private void toolcmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //視覚言語の設定
        //    string language = "";
        //    if (toolcmbLanguage.Text == "Japanese")
        //    {
        //        language = "JA";
        //    }
        //    else
        //    {
        //        language = "EN";
        //    }
        //    Thread.CurrentThread.CurrentUICulture =
        //        new System.Globalization.CultureInfo(language);
        //    Application.EnableVisualStyles();
        //}
    }
}


