using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;
using System.Xml.Linq;

namespace ArmsMonitor
{
    public partial class FrmMonitor : Form
    {
        private const string MONITOR_XML = @"C:\ARMS\Config\monitor.xml";

        /// <summary>
        /// 監視対象設備リスト
        /// </summary>
        private int[] maclist = null;
        private List<string> plantcdList;

        private class MaterialLimitRecord
        {
            public string MacNm { get; set; }
            public string MatNm { get; set; }
            public string LotNo { get; set; }
            public DateTime UseLimit { get; set; }
            public int Remains { get; set; }
            public LimitCheckResult.ResultKb Result { get; set; }
        }

        private class ResinLimitRecord
        {
            public string MacNm { get; set; }

            public string ResinGroupCd { get; set; }

            public int MixResultId { get; set; }

            public DateTime UseLimit { get; set; }

            public int Remains { get; set; }

            public LimitCheckResult.ResultKb Result { get; set; }

        }

        private System.Media.SoundPlayer sp;

        private static FrmMonitor instance;
        public static FrmMonitor GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmMonitor();
            }

            return instance;
        }

		public static bool isRunning = false;

        private FrmMonitor()
        {
            InitializeComponent();
        }


        LimitCheckResult[] result;
        List<MaterialLimitRecord> materialResult;
        List<ResinLimitRecord> resinResult;

        /// <summary>
        /// DB樹脂警告無効フラグ設定リスト
        /// </summary>
        List<string> ignoreResinList = new List<string>();


        private void FrmMonitor_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.sp = new System.Media.SoundPlayer("ringin.wav");

            loadXmlMacList();
        }

        #region setLimitGrid
        
        private void setGrid()
        {
            this.grdLimit.DataSource = result;

            #region 列設定

            foreach (DataGridViewColumn col in this.grdLimit.Columns)
            {
                switch (col.Name)
                {
                    case "colChk":
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット";
                        col.ReadOnly = true;
                        break;

                    case "MagNo":
                        col.HeaderText = "マガジン";
                        col.ReadOnly = true;
                        break;

                    case "FromProcNm":
                        col.HeaderText = "開始工程";
                        col.ReadOnly = true;
                        break;

                    case "FromKb":
                        col.HeaderText = "開始区分";
                        col.ReadOnly = true;
                        break;

                    case "ChkProcNm":
                        col.HeaderText = "完了工程";
                        col.ReadOnly = true;
                        break;

                    case "ChkKb":
                        col.HeaderText = "完了区分";
                        col.ReadOnly = true;
                        break;

                    case "LimitDt":
                        col.HeaderText = "有効期限";
                        col.ReadOnly = true;
                        break;

                    case "Remains":
                        col.HeaderText = "残時間(分)";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;


                }
            }
            #endregion

            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                switch ((LimitCheckResult.ResultKb)row.Cells["Result"].Value)
                {
                    case LimitCheckResult.ResultKb.Expired:
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;

                    case LimitCheckResult.ResultKb.Warning:
                        row.DefaultCellStyle.BackColor = Color.Pink;
                        break;

                    case LimitCheckResult.ResultKb.Attension:
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion
        
        #region setMaterialGrid

        private void setMaterialGrid()
        {
            this.grdMaterialLimit.DataSource = materialResult;

            #region 列設定

            foreach (DataGridViewColumn col in this.grdMaterialLimit.Columns)
            {
                switch (col.Name)
                {
                    case "MacNm":
                        col.HeaderText = "設備";
                        col.ReadOnly = true;
                        break;

                    case "MatNm":
                        col.HeaderText = "原材料名";
                        col.ReadOnly = true;
                        break;

                    case "LotNo":
                        col.HeaderText = "原材料ロット";
                        col.ReadOnly = true;
                        break;

                    case "ChkProcNm":
                        col.HeaderText = "完了工程";
                        col.ReadOnly = true;
                        break;

                    case "UseLimit":
                        col.HeaderText = "有効期限";
                        col.ReadOnly = true;
                        break;

                    case "Remains":
                        col.HeaderText = "残時間(分)";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

            foreach (DataGridViewRow row in this.grdMaterialLimit.Rows)
            {
                switch ((LimitCheckResult.ResultKb)row.Cells["Result"].Value)
                {
                    case LimitCheckResult.ResultKb.Expired:
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;

                    case LimitCheckResult.ResultKb.Warning:
                        row.DefaultCellStyle.BackColor = Color.Pink;
                        break;

                    case LimitCheckResult.ResultKb.Attension:
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        break;
                }
            }
        }
        #endregion

        #region setResinGrid

        private void setResinGrid()
        {
            this.grdResinLimit.DataSource = resinResult;

            #region 列設定

            foreach (DataGridViewColumn col in this.grdResinLimit.Columns)
            {
                switch (col.Name)
                {
                    case "MacNm":
                        col.HeaderText = "設備";
                        col.ReadOnly = true;
                        break;

                    case "ResinGroupCd":
                        col.HeaderText = "樹脂グループ";
                        col.ReadOnly = true;
                        break;

                    case "MixResultId":
                        col.HeaderText = "調合結果ID";
                        col.ReadOnly = true;
                        break;

                    case "UseLimit":
                        col.HeaderText = "有効期限";
                        col.ReadOnly = true;
                        break;

                    case "Remains":
                        col.HeaderText = "残時間(分)";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

            foreach (DataGridViewRow row in this.grdResinLimit.Rows)
            {
                switch ((LimitCheckResult.ResultKb)row.Cells["Result"].Value)
                {
                    case LimitCheckResult.ResultKb.Expired:
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;

                    case LimitCheckResult.ResultKb.Warning:
                        row.DefaultCellStyle.BackColor = Color.Pink;
                        break;

                    case LimitCheckResult.ResultKb.Attension:
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        break;

                    case LimitCheckResult.ResultKb.None:
                        row.DefaultCellStyle.BackColor = Color.Silver;
                        break;
                }
            }
        }

        #endregion

        private void UpdateData()
        {
			if (isRunning) return;

            try
            {
                isRunning = true;

                // 監視設備リストを更新 (監視設備変更中の場合は、クラス変数のみ更新。画面上のリストは更新しない)
                this.Invoke(new loadMacListFromPlantListDelegate(loadMacListFromPlantList), this.plantcdList, !this.chkMacList.Enabled);
                
                Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
				List<LimitCheckResult> lcr = new List<LimitCheckResult>();
                string limitCheckErrStr = string.Empty;

                // ロット毎のループ前に対象ロットリストの全型番の時間監視マスタを取得して変数に格納
                List<AsmLot> lotList = new List<AsmLot>();
                foreach (string lotno in maglist.Select(m => Order.MagLotToNascaLot(m.NascaLotNO)).Distinct())
                {
                    AsmLot lot = AsmLot.GetAsmLot(lotno);
                    lotList.Add(lot);
                }
                List<string> typeList = lotList.Select(l => l.TypeCd).Distinct().ToList();
                Dictionary<string, TimeLimit[]> dicLimit = new Dictionary<string, TimeLimit[]>();
                foreach (string typecd in typeList)
                {
                    dicLimit.Add(typecd, TimeLimit.GetLimits(typecd, null));
                }

                foreach (string lotno in maglist.Select(m => Order.MagLotToNascaLot(m.NascaLotNO)).Distinct())
                {
                    try
                    {
                        AsmLot lot = AsmLot.GetAsmLot(lotno);
                        //TimeLimit[] limitList = TimeLimit.GetLimits(lot.TypeCd, null);
                        TimeLimit[] limitList = dicLimit[lot.TypeCd];
                        LimitCheckResult[] crlist = TimeLimit.CheckTimeLimit(lot, limitList, null, true, maclist);

                        foreach (LimitCheckResult cr in crlist)
                        {
                            if (TimeLimit.IsChecked(cr.LotNo, cr.Limit.ChkWorkCd, cr.Limit.ChkKb, cr.Limit.TgtWorkCd, cr.Limit.TgtKb) == true)
                            {
                                continue;
                            }
                            else
                            {
                                lcr.Add(cr);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        //判定の例外エラー時も他ロットの判定処理は止めないようにするため、ログだけ残しておいて後でエラー表示する。
                        if(string.IsNullOrWhiteSpace(limitCheckErrStr) == true)
                        {
                            limitCheckErrStr = ex.Message;
                        }
                    }
				}

                result = lcr.ToArray().OrderBy(l => l.Remains).ToArray();
                result = lcr.OrderBy(l => l.Remains).ToArray();

                this.Invoke(new Action(setGrid));

				if (lcr.Count >= 1 || string.IsNullOrWhiteSpace(limitCheckErrStr) == false)
				{
					int ct = lcr.Where(l => (l.Result != LimitCheckResult.ResultKb.Normal) && (l.Result != LimitCheckResult.ResultKb.Attension)).Count();

					if (ct >= 1 || string.IsNullOrWhiteSpace(limitCheckErrStr) == false)
					{
                        string displayMessage = string.Empty;

                        if(ct >= 1)
                        {
                            displayMessage = "警告作業が存在します\n\n";
                        }

                        if(string.IsNullOrWhiteSpace(limitCheckErrStr) == false)
                        {
                            displayMessage = displayMessage + "正常に監視できていないデータが存在します\n" + limitCheckErrStr;
                        }

						this.sp.PlayLooping();
						MessageBox.Show(displayMessage);
						this.sp.Stop();
					}
				}

                // 資材有効期限チェック
				bool materialWarning = materialCheck(out this.materialResult);

                this.materialResult = this.materialResult.OrderBy(r => r.Remains).ToList();
                
				this.Invoke(new Action(setMaterialGrid));
				if (materialWarning)
				{
					this.sp.PlayLooping();
					MessageBox.Show("資材有効期限の警告が存在します");
					this.sp.Stop();
				}
                
                // 樹脂有効期限チェック
                bool resinWarning = resinCheck(out this.resinResult);

                this.resinResult = this.resinResult.OrderBy(r => r.Remains).ToList();

                this.Invoke(new Action(setResinGrid));
                if (resinWarning)
                {
                    this.sp.PlayLooping();
                    MessageBox.Show("樹脂有効期限の警告が存在します");
                    this.sp.Stop();
                }
            }
            finally
			{
				isRunning = false;
			}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = Config.Settings.MonitorTimerMilliSecond;

				Action act = new Action(UpdateData);
				act.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmPasswordDialog pwd = new FrmPasswordDialog("ARMS", "時間監視を本当に終了しますか?\r\n終了する場合パスワードを入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        #region btnCheck_CLick
        
        private void btnCheck_Click(object sender, EventArgs e)
        {
            List<LimitCheckResult> checklist = new List<LimitCheckResult>();
            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colChk"].Value) == true)
                {
                    checklist.Add((LimitCheckResult)row.DataBoundItem);
                }
            }

            if (checklist.Count == 0) return;

            FrmPasswordDialog pwd = new FrmPasswordDialog("OK", "チェックを付けた行を時間監視無効に設定します\r\n続行する場合OKと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK) return;

            foreach (LimitCheckResult cr in checklist)
            {
                TimeLimit.Check(cr.LotNo, cr.Limit.ChkWorkCd, cr.Limit.ChkKb, cr.Limit.TgtWorkCd, cr.Limit.TgtKb);
            }

            Action act = new Action(UpdateData);
            act.BeginInvoke(null, null);

            MessageBox.Show("無効フラグ設定完了");

            timer1.Interval = Config.Settings.MonitorTimerMilliSecond;

        }
        #endregion

        #region MaterialCheck

        private bool materialCheck(out List<MaterialLimitRecord> records)
        {
            bool retv = false;
            records = new List<MaterialLimitRecord>();
            try
            {
                MachineInfo[] machines = MachineInfo.SearchMachine(null, null, true, false);
                foreach (MachineInfo m in machines)
                {
                    //2015.11.6 車載対応。資材グリッドも設備リストを参照するよう変更
                    if(maclist != null)
                    {
                        if(maclist.Contains(m.MacNo) == false)
                        {
                            continue;
                        }
                    }

                    string typecd = MachineInfo.GetCurrentEICSTypeCd(m);
                    Material[] mats = m.GetMaterials(DateTime.Now, DateTime.Now);

                    foreach (Material mat in mats)
                    {
                        //ウェハー、フレーム以外を対象
                        if (mat.IsWafer == true || mat.IsFrame == true) continue;
                        
                        //原材料使用期限
                        DateTime limitdt = mat.LimitDt;

                        //タイプ名が取得できた場合は、解凍・開封有効期限を使用
                        if (!string.IsNullOrEmpty(typecd))
                        {
                            DateTime dbResinLimit = Material.GetDBResinLimitDt(typecd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true);
                            if (limitdt >= dbResinLimit) limitdt = dbResinLimit;
                        }

                        //2015.2.5 車載3次 期限監視不要資材を除外
                        if (limitdt >= DateTime.Parse("9000/1/1")) continue;

                        MaterialLimitRecord rec = new MaterialLimitRecord();
                        rec.MacNm = m.LongName;
                        rec.MatNm = mat.MaterialNm;
                        rec.LotNo = mat.LotNo;
                        rec.UseLimit = limitdt;
                        rec.Remains = (int)(limitdt - DateTime.Now).TotalMinutes;


                        //暫定で60分固定
                        //NASCAから取得する形は高コスト過ぎるため
                        //今後もし変更の必要が出た場合は、ライン個別で汎用マスタ内に格納。
                        int warningMinutes = 10;
                        int attensionMinutes = 60;

                        if (limitdt <= DateTime.Now)
                        {
                            rec.Result = LimitCheckResult.ResultKb.Expired;
                            retv = true;
                        }
                        else if (limitdt <= DateTime.Now.AddMinutes(warningMinutes))
                        {
                            rec.Result = LimitCheckResult.ResultKb.Warning;
                            retv = true;
                        }
                        else if (limitdt < DateTime.Now.AddMinutes(attensionMinutes))
                        {
                            rec.Result = LimitCheckResult.ResultKb.Attension;

                            //警報済みリストに存在しない場合は警報ON
                            if (!this.ignoreResinList.Contains(mat.LotNo))
                            {
                                this.ignoreResinList.Add(mat.LotNo);
                                retv = true;
                            }
                        }
                        else
                        {
                            rec.Result = LimitCheckResult.ResultKb.Normal;
                        }

                        records.Add(rec);
                    }
                }
                return retv;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("資材有効期限監視エラー:" + ex.ToString());
                return false;
            }
        }

        #endregion

        #region ResinCheck

        private bool resinCheck(out List<ResinLimitRecord> records)
        {
            bool retv = false;
            records = new List<ResinLimitRecord>();

            try
            {
                MachineInfo[] machines = MachineInfo.SearchMachine(null, null, true, false);
                foreach (MachineInfo m in machines)
                {
                    if (maclist != null)
                    {
                        if (maclist.Contains(m.MacNo) == false)
                        {
                            continue;
                        }
                    }

                    Resin[] resinList = m.GetResins(DateTime.Now, DateTime.Now);
                    foreach (Resin resin in resinList)
                    {
                        ResinLimitRecord rec = new ResinLimitRecord();
                        rec.MacNm = m.LongName;
                        rec.MixResultId = int.Parse(resin.MixResultId);
                        rec.ResinGroupCd = resin.ResinGroupCd;
                        rec.UseLimit = resin.LimitDt;
                        rec.Remains = (int)(resin.LimitDt - DateTime.Now).TotalMinutes;

                        TimeLimit limit = TimeLimit.GetResinLimit(resin.ResinGroupCd);
                        if (limit == null)
                        {
                            // マスタが無い場合は、注意時間 = 4hr前, 警告時間 = 2hr前をデフォルトとする
                            limit = new TimeLimit();
                            limit.ResinGroupCd = resin.ResinGroupCd;
                            limit.WarningBefore = 60 * 2;
                            limit.AttensionBefore = 60 * 4;
                        }

                        if (resin.LimitDt <= DateTime.Now)
                        {
                            rec.Result = LimitCheckResult.ResultKb.Expired;
                            retv = true;
                        }
                        else if (resin.LimitDt <= DateTime.Now.AddMinutes(limit.WarningBefore))
                        {
                            rec.Result = LimitCheckResult.ResultKb.Warning;
                            retv = true;
                        }
                        else if (resin.LimitDt < DateTime.Now.AddMinutes(limit.AttensionBefore))
                        {
                            rec.Result = LimitCheckResult.ResultKb.Attension;
                            retv = true;
                        }
                        else
                        {
                            rec.Result = LimitCheckResult.ResultKb.Normal;
                        }

                        //if (limit == null)
                        //{
                        //    rec.Result = LimitCheckResult.ResultKb.None;
                        //}
                        //else
                        //{
                        //    if (resin.LimitDt <= DateTime.Now)
                        //    {
                        //        rec.Result = LimitCheckResult.ResultKb.Expired;
                        //        retv = true;
                        //    }
                        //    else if (resin.LimitDt <= DateTime.Now.AddMinutes(limit.WarningBefore))
                        //    {
                        //        rec.Result = LimitCheckResult.ResultKb.Warning;
                        //        retv = true;
                        //    }
                        //    else if (resin.LimitDt < DateTime.Now.AddMinutes(limit.AttensionBefore))
                        //    {
                        //        rec.Result = LimitCheckResult.ResultKb.Attension;
                        //        retv = true;
                        //    }
                        //    else
                        //    {
                        //        rec.Result = LimitCheckResult.ResultKb.Normal;
                        //    }
                        //}

                        records.Add(rec);
                    }
                }

                return retv;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("樹脂有効期限監視エラー:" + ex.ToString());
                return false;
            }
        }

        #endregion

        #region XML関係

        private void saveXmlMacList(List<int> maclist)
        {
            XDocument doc;
            if (System.IO.File.Exists(MONITOR_XML))
            {
                doc = XDocument.Load(MONITOR_XML);
            }
            else
            {
                doc = new XDocument();
            }

            doc.RemoveNodes();

            XElement root = new XElement("settings");
            doc.Add(root);

            foreach (int macno in maclist)
            {
                XElement elm = new XElement("mac");
                elm.Value = macno.ToString();
                root.Add(elm);
            }
            doc.Save(MONITOR_XML);
        }

        private void saveXmlMacList(List<string> plantlist)
        {
            XDocument doc;
            if (System.IO.File.Exists(MONITOR_XML))
            {
                doc = XDocument.Load(MONITOR_XML);
            }
            else
            {
                doc = new XDocument();
            }

            doc.RemoveNodes();

            XElement root = new XElement("settings");
            doc.Add(root);

            foreach (string plantcd in plantlist)
            {
                // 設備CDが空欄の奴は除去
                if (!string.IsNullOrEmpty(plantcd))
                {
                    XElement elm = new XElement("plant");
                    elm.Value = plantcd;
                    root.Add(elm);
                }
            }
            doc.Save(MONITOR_XML);
        }

        private void loadXmlMacList()
        {
            this.plantcdList = new List<string>();
            XDocument doc;
            if (System.IO.File.Exists(MONITOR_XML))
            {
                doc = XDocument.Load(MONITOR_XML);
                var settings = doc.Root.Elements();
                foreach (var setting in settings)
                {
                    this.plantcdList.Add(setting.Value);
                }
            }
            
            // 取得したplantcdListでmacListを更新
            this.Invoke(new loadMacListFromPlantListDelegate(loadMacListFromPlantList), this.plantcdList, true);
        }

        delegate void loadMacListFromPlantListDelegate(List<string> plantcdList, bool chkMacListUpdate);

        private void loadMacListFromPlantList(List<string> plantcdList, bool chkMacListUpdate)
        {
            // 処理①：クラス変数を更新 (実際の更新処理で使用する変数)
            MachineInfo[] maclist = MachineInfo.SearchMachine(null, null, false, false);
            List<int> macnoList = new List<int>();

            foreach (var mac in maclist.Where(m => plantcdList.Contains(m.NascaPlantCd)).OrderBy(m => m.MacNo))
            {
                macnoList.Add(mac.MacNo);
            }

            if (macnoList == null || macnoList.Count == 0)
            {
                this.maclist = null;
            }
            else
            {
                this.maclist = macnoList.ToArray();
            }

            // 処理②：画面上の監視設備リストを更新 (引数 = true 時のみ)
            if (chkMacListUpdate)
            {
                //チェックリストを初期化 (空にする)
                this.chkMacList.Items.Clear();

                //最初にチェックされたものを追加
                foreach (var mac in maclist.Where(m => plantcdList.Contains(m.NascaPlantCd)).OrderBy(m => m.MacNo))
                {
                    this.chkMacList.Items.Add(mac, true);
                }

                //チェックされていないものを追加
                foreach (var mac in maclist.Where(m => !plantcdList.Contains(m.NascaPlantCd)).OrderBy(m => m.MacNo))
                {
                    this.chkMacList.Items.Add(mac, false);
                }
            }
        }
        #endregion

        private void btnChangeMac_Click(object sender, EventArgs e)
        {
            if (this.chkMacList.Enabled)
            {
				this.btnChangeMac.Text = "変更";
                this.chkMacList.Enabled = false;

                List<string> plantlist = new List<string>();
                foreach (int i in this.chkMacList.CheckedIndices)
                {

                    MachineInfo m = chkMacList.Items[i] as MachineInfo;
                    if (m != null)
                    {
                        // 同じ設備CDが追加済みの場合は、新たに追加しない
                        if (!plantlist.Contains(m.NascaPlantCd))
                        {
                            plantlist.Add(m.NascaPlantCd);
                        }
                    }
                }
                saveXmlMacList(plantlist);
                loadXmlMacList();
            }
            else
            {
				//変更ボタン押下時に確認ダイアログ表示
				FrmPasswordDialog pwd = new FrmPasswordDialog("CHANGE", "監視設備を変更しますか?\r\n変更する場合パスワードを入力してください");
				DialogResult res = pwd.ShowDialog();

				if (res != DialogResult.OK)
				{
					return;
				}

				this.chkMacList.Enabled = true;
                this.btnChangeMac.Text = "更新確定";
            }
        }

        private void btnDBPreOvnPlasma_Click(object sender, EventArgs e)
        {
            FrmDBPreOvnPlasma frm = FrmDBPreOvnPlasma.GetInstance();
            frm.Show();
        }

    }
}
