using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using ArmsApi;

namespace FIFDebugTools
{
    public partial class ArmsTranController : Form
    {
        //DBクラス実体
        //Arms
        TnLot tnlot = new TnLot();
        TnTran tntran = new TnTran();
        TnMag tnmag = new TnMag();
        TnLotLog tnlotlog = new TnLotLog();
        TnRestrict tnrestrict = new TnRestrict();
        TnVirtualMag tnvtmag = new TnVirtualMag();
        //Lens
        TnLot_Lens tnlotlens = new TnLot_Lens();
        TnTran_Lens tntranlens = new TnTran_Lens();
        //Qcil
        TnLog_Qcil tnlogqcil = new TnLog_Qcil();
        TnLott_Qcil tnlottqcil = new TnLott_Qcil();
        TnQcnr_Qcil tnqcnrqcil = new TnQcnr_Qcil();
        //etc.
        public Dictionary<string, string> ProcessName;
        string workheader = string.Empty;
        string CR = "\r\n";
        string msg = string.Empty;

        public ArmsTranController()
        {
            InitializeComponent();

            // プロセス定義
            ProcessName = new Dictionary<string, string>
            {
                { "2", "バリ取り・クリーナー" },
                { "3", "ダイボンド(LED)" },
                { "4", "ダイボンド(ZD)" },
                { "5", "ダイボンドキュアー" },
                { "6", "WB前プラズマ" },
                { "7", "ワイヤーボンド" },
                { "8", "SDR" },
                { "9", "SDRキュアー" },
                { "10", "SCT" },
                { "11", "SCTキュアー" },
                { "15", "樹脂成形" },
                { "16", "樹脂成形キュアー" },
                { "17", "レーザーダイシング" }
            };

        }


        //
        // ProcNoコントロール
        //
        private void btn_reset_proc_Click(object sender, EventArgs e)
        {
            cb_procno.SelectedIndex = 0;
        }

        private void btn_inc_proc_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex < (cb_procno.Items.Count - 1))
            {
                cb_procno.SelectedIndex += 1;
            }
        }

        private void btn_dec_proc_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex > 0 )
            {
                cb_procno.SelectedIndex -= 1;
            }
        }


        //
        // 共通ファンクション
        //

        // FormTextBox入力チェック
        public bool CheckTxtBox()
        {
            if (string.IsNullOrEmpty(txt_dbcnt_lotno.Text))
            {
                ConsoleShow("LotNoを入力してください");
                return false;
            }

            if (string.IsNullOrEmpty(txt_debugmagheader.Text))
            {
                ConsoleShow("Mag接頭文字を入力してください");
                return false;
            }

            int d;
            if (!int.TryParse(txt_bdqty.Text, out d))
            {
                ConsoleShow("基板数量が不正です");
                return false;
            }

            if (string.IsNullOrEmpty(txt_magno.Text))
            {
                ConsoleShow("MagNoを入力してください");
                return false;
            }

            

            return true;
        }

        // Lotの存在確認
        public bool IsActiveLot(string lotno)
        {
            if (ArmsApi.Model.AsmLot.SearchAsmLot(lotno, false, false, int.Parse(txt_profileno.Text)).Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // デバックモニタ・デリゲート
        public delegate void ConsoleDelegate(string text, bool cr = true);
        public void ConsoleShow(string text, bool cr = true)
        {
            if (debugMonitor.InvokeRequired)
            {
                ConsoleDelegate d = new ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] { text, cr });
            }
            else
            {
                if (cr)
                    debugMonitor.AppendText(text + CR);
                else
                    debugMonitor.AppendText(text);
            }
        }

        // 作業開始確認ダイアログ
        public DialogResult SelectYesNoMessage(string msg)
        {
            string message = msg;
            string caption = "CAUTION!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);
            return result;
        }

        // ファイル作成
        public bool CreateFile(string FilePath, string Contents, ref string msg, string enccode = "utf-8")
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                if (enccode == "utf-8")
                {
                    using (FileStream fs = File.Create(FilePath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(Contents.TrimEnd('\r', '\n'));
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                    StreamWriter stw = new StreamWriter(FilePath, false, sjisEnc);
                    stw.WriteLine(Contents.TrimEnd('\r', '\n'));
                    stw.Close();
                }

                return true;
            }

            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        //
        /* テキストファイル全読み込み関数
         * OK: true
         * NG: false
         * 読込内容：contents 
         */
        //
        public bool ReadTextFile(string filepath, ref string contents, string enccode)
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                contents = sr.ReadToEnd();
                //contents = new string(contents.Where(c => !char.IsControl(c)).ToArray());
                sr.Close();

                return true;
            }
            catch (Exception ex)
            {
                contents = ex.Message;
                return false;
            }
        }

        //ファイルコピー
        public bool CopyFile(string FilePath, string MoveToPath)
        {
            try
            {
                File.Copy(FilePath, MoveToPath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //
        public bool CreateMLog(string OriginPath, string filepath, Dictionary<string, string> logdct, ref string msg, string enccode = "UTF-8")
        {
            try
            {
                var contents = "";
                if (ReadTextFile(OriginPath, ref contents, enccode))
                {
                    foreach (var item in logdct)
                    {
                        contents = contents.Replace(item.Key, item.Value);
                    }
                }
                else return false;
                
                CreateFile(filepath, contents, ref msg, enccode);
                return true;
            }
            catch(Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }


        public void cleanFolder(string folderpath)
        {
            foreach (string f in Directory.GetFiles(folderpath, "*.*"))
            {
                File.Delete(f);
            }
        }

        //
        // DB処理
        //

        // DBダミーデータ処理
        private void _btn_dbcnt_exec_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex > 0)
            {
                var dblogfpath = "C:\\ARMS\\DB\\";
                var fname_tntran = "tntran.csv";
                var fname_tnmag = "tnmag.csv";
                var fname_tntranlens = "tntran_lens.csv";
                var tntranloglist = new List<string>();
                var tnmagloglist = new List<string>();
                var tntranlensloglist = new List<string>();
                var dbcnt = new DbControls();
                var typecd = cmb_typecd.Text;
                var lotno = txt_dbcnt_lotno.Text;
                var dt = DateTime.Now;
                var dtformed = dt.ToString("yyyyMMddHHmmss");
                var msg = string.Empty;
                var curProcno = string.Empty;

                if (string.IsNullOrEmpty(lotno))
                {
                    lotno = "test" + dtformed;
                    txt_dbcnt_lotno.Text = lotno;
                }

                var swapStrings = new Dictionary<string, string>
                {
                    { "@lotno", lotno }
                };

                // TnMag, TnVirtualMag Clean
                if (SelectYesNoMessage("各種トランザクションデータを初期化・DBの途中投入準備します") == DialogResult.Yes)
                {

                    // DBhistoryフォルダクリーン
                    foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory", "*.*"))
                    {
                        File.Delete(f);
                    }
                    // DBhistoryフォーマットコピー
                    foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory\format", "*.*"))
                    {
                        CopyFile(f, @"C:\ARMS\DBhistory\" + Path.GetFileName(f));
                    }

                    // TnLot Insert
                    dt = DateTime.Now;
                    dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    tnlot.lotno = lotno;
                    tnlot.typecd = typecd;
                    tnlot.templotno = lotno;
                    tnlot.lastupddt = dtformed;
                    tnlot.dbthrowdt = dtformed;
                    if (tnlot.insertTnlot(ref msg))
                    {
                        ConsoleShow($"TnLot: Insert OK");
                    }
                    else
                    {
                        ConsoleShow(msg);
                        ConsoleShow($"TnLot: Insert NG");
                    }

                    // TnLot_Lens Insert
                    dt = DateTime.Now;
                    dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    tnlotlens.LotNO = lotno;
                    tnlotlens.TypeCD = typecd;
                    tnlotlens.LastupdDT = dtformed;
                    if (tnlotlens.InsertTnlot(ref msg))
                    {
                        ConsoleShow($"TnLot_Lens: Insert OK");
                    }
                    else
                    {
                        ConsoleShow(msg);
                        ConsoleShow($"TnLot_Lens: Insert NG");
                    }

                    // トランザクションダミーデータ読込
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tntran, ref tntranloglist);
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tnmag, ref tnmagloglist);
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tntranlens, ref tntranlensloglist);

                    // Procno毎に処理
                    for (int i = 0; i < cb_procno.SelectedIndex; i++)
                    {
                        // TnTran
                        foreach (var log in tntranloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var tran = _log.Split(',');
                            var procno = int.Parse(tran[1]);
                            curProcno = cb_procno.Items[i].ToString();
                            dt = DateTime.Now;
                            dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");

                            if ((procno == int.Parse(curProcno))
                               || (procno == 25 && int.Parse(curProcno) == 24)
                               || (procno == 26 && int.Parse(curProcno) == 24)
                               || (procno == 16 && int.Parse(curProcno) == 18))
                            {
                                tntran.lotno = tran[0];
                                tntran.procno = tran[1];
                                tntran.macno = tran[2];
                                tntran.inmag = tran[3];
                                tntran.outmag = tran[4];
                                tntran.startdt = dtformed;
                                tntran.enddt = dtformed;
                                tntran.iscomplt = tran[7];
                                tntran.stocker1 = tran[8];
                                tntran.stocker2 = tran[9];
                                tntran.comment = tran[10];
                                tntran.transtartempcd = tran[11];
                                tntran.trancompempcd = tran[12];
                                tntran.inspectempcd = tran[13];
                                tntran.inspectct = tran[14];
                                tntran.isnascastart = tran[15];
                                tntran.isnascaend = tran[16];
                                tntran.lastupddt = tran[17];
                                tntran.isnascadefectend = tran[18];
                                tntran.isnascacommentend = tran[19];
                                tntran.delfg = tran[20];
                                tntran.isdefectend = tran[21];
                                tntran.isdefectautoimportend = tran[22];
                                tntran.isnascarunning = tran[23];
                                tntran.isautoimport = tran[24];
                                tntran.isresinmixordered = tran[25];

                                if (tntran.InsertTnTran(ref msg))
                                {
                                    ConsoleShow($"TnTran: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    //ConsoleShow(msg);
                                    ConsoleShow($"TnTran: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                        //TnMag
                        foreach (var log in tnmagloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var mag = _log.Split(',');
                            var procno = int.Parse(mag[2]);
                            if (procno == int.Parse(cb_procno.Items[i].ToString()))
                            {
                                dt = DateTime.Now;
                                dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                                tnmag.lotno = mag[0];
                                tnmag.magno = mag[1];
                                tnmag.inlineprocno = mag[2];
                                tnmag.newfg = mag[3];
                                tnmag.lastupddt = dtformed;

                                if (tnmag.InsertTnMag(ref msg))
                                {
                                    ConsoleShow($"TnMag: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    ConsoleShow(msg);
                                    ConsoleShow($"TnMag: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                        //TnTran_Lens
                        foreach (var log in tntranlensloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var tranlens = _log.Split(',');
                            var procno = int.Parse(tranlens[1]);
                            if (procno == int.Parse(cb_procno.Items[i].ToString()))
                            {
                                dt = DateTime.Now;
                                dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                                tntranlens.LotNO = tranlens[0];
                                tntranlens.ProcNO = tranlens[1];
                                tntranlens.PlantCD = tranlens[2];
                                tntranlens.StartDT = dtformed;
                                tntranlens.EndDT = dtformed;
                                tntranlens.IsCompleted = tranlens[5];
                                tntranlens.LastUpdDT = dtformed;
                                tntranlens.LastUpdDT = dtformed;
                                tntranlens.CarrierNO = tranlens[8];

                                if (tntranlens.InsertTnTran(ref msg))
                                {
                                    ConsoleShow($"TnTran_lens: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    //ConsoleShow(msg);
                                    ConsoleShow($"TnTran_lens: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                    }
                    // Update TnMag
                    if (curProcno == "12")
                    {
                        if (tnmag.UpdateTnMag(lotno + "_#1", "ME0001", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updateできませんでした");
                        }
                        if (tnmag.UpdateTnMag(lotno + "_#2", "ME0002", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updateできませんでした");
                        }
                    }
                    if (curProcno == "13")
                    {
                        if (tnmag.UpdateTnMag(lotno + "_#1", "ME0001", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updateできませんでした");
                        }
                        if (tnmag.UpdateTnMag(lotno + "_#2", "ME0002", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updateできませんでした");
                        }
                    }
                    else if (curProcno == "24")
                    {
                        if (tnmag.UpdateTnMag(lotno, tntran.outmag, "26", ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updateできませんでした");
                        }
                    }
                    else if (curProcno == "18")
                    {
                        if (tnmag.UpdateTnMag(lotno, "M00005", "16", ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.M00005⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.M00005⇒Updateできませんでした");
                        }
                    }
                    else
                    {
                        if (tnmag.UpdateTnMag(lotno, tntran.outmag, curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updateできませんでした");
                        }
                    }
                    //Insert TnInspection
                    if (curProcno == "9")
                    {
                        var TnInsp = new TnInspection() { lotno = lotno };
                        if (TnInsp.InsertTnMag(ref msg))
                        {
                            ConsoleShow($"TnInspection: Inserted");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnInspection: Insertできませんでした");
                        }
                    }
                }
            }
            else
            {
                ConsoleShow("初工程のためDBへのダミー書き込みはできません");
            }
        }

        // RecordTn
        public void RecordTn(string header)
        {
            var msg = string.Empty;
            tnlot.RecordTrData(header, ref msg);
            tntran.RecordTrData(header, ref msg);
            tnlotlog.RecordTrData(header, ref msg);
            tnrestrict.RecordTrData(header, ref msg);
            tnlotlens.RecordTrData(header, ref msg);
            tntranlens.RecordTrData(header, ref msg);
            tnlogqcil.RecordTrData(header, ref msg);
            tnlogqcil.RecordTrData(header, ref msg);
            tnqcnrqcil.RecordTrData(header, ref msg);

        }

        // 最新のロットナンバーをTnTranから取得
        public bool getLastLotNofromTnTran(ref string lotno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"SELECT[lotno] ";
                    cmd.CommandText += $" FROM[dbo].[TnTran] ";
                    cmd.CommandText += $"WHERE startdt = (select MAX(startdt) from[dbo].[TnTran])";

                    lotno = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }
        public string getLastLotNo()
        {
            string lotno = "";
            if (getLastLotNofromTnTran(ref lotno))
            {
                return lotno;
            }
            else
            {
                ConsoleShow("ロットNOの取得が失敗しました");
                return string.Empty;
            }
        }

        // TnMagのアップデート
        private bool EditTnMag(string magno, string lotno, int procno, int bdqty, ref string msg)
        {
            try
            {
                ArmsApi.Model.Process fp;
                ArmsApi.Model.Magazine mag;

                mag = ArmsApi.Model.Magazine.GetCurrent(magno);
                if (mag != null)
                {
                    mag.NewFg = false;
                    mag.Update();
                }

                mag = new ArmsApi.Model.Magazine()
                {
                    MagazineNo = magno,
                    NascaLotNO = lotno,
                    NowCompProcess = procno,
                    NewFg = true,
                    FrameQty = bdqty,
                    LastUpdDt = DateTime.Now
                };

                mag.Update();

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }


        ///////////////////////////////////
        // トランザクション操作実行関数
        ///////////////////////////////////
        ///
        private void cmb_typecd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_typecd.Text == "CLUHANKAN-SIM")
            {
                cb_procno.Items.Clear();
                cb_procno.Items.AddRange(new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" });
                cb_procno.SelectedIndex = 0;
                txt_profileno.Text = "99991";
            }
            else if (cmb_typecd.Text == "CLUKANSEI-SIM")
            {
                cb_procno.Items.Clear();
                cb_procno.Items.AddRange(new string[] { "15", "16", "17"});
                cb_procno.SelectedIndex = 0;
                txt_profileno.Text = "99992";
            }
        }

        private void cb_procno_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_processname.Text = ProcessName[cb_procno.Text];
            
            if (cb_procno.SelectedIndex > 0)
            {
                btn_instarans.Enabled = true;
            }
            else
            {
                btn_instarans.Enabled = false;
            }
        }

        private void btn_inslot_Click(object sender, EventArgs e)
        {
            if (!CheckTxtBox())
            {
                return;
            }

            var lotno = txt_dbcnt_lotno.Text;

            if (IsActiveLot(lotno))
            {
                ConsoleShow("既に指定のロットが登録されています");
                return;
            }

            int bdqty;
            int d;

            bdqty = int.Parse(txt_bdqty.Text);

            try
            {
                //
                // LOTチェック～インサート
                //
                if (string.IsNullOrEmpty(lotno))
                {
                    ConsoleShow("ロットNOを指定してください");
                    return;
                }

                if (ArmsApi.Model.AsmLot.GetAsmLot(lotno) != null)
                {
                    ConsoleShow("指定のロットNOは登録済みです");
                    return;
                }

                var lot = new ArmsApi.Model.AsmLot()
                {
                    TypeCd = cmb_typecd.Text,
                    NascaLotNo = lotno,
                    TempLotNo = lotno,
                    ProfileId = int.Parse(txt_profileno.Text),
                    BlendCd = txt_blendcd.Text,
                    ResinGpCd = new List<string> { txt_jushicd.Text },
                    CutBlendCd = "NA",
                    IsWarning = false,
                    IsRestricted = false,
                    IsLifeTest = false,
                    IsKHLTest = false,
                    IsColorTest = false,
                    IsTemp = false,
                    IsNascaLotCharEnd = false,
                    MacGroup = new List<string> { "1" },
                    IsBadMarkFrame = false,
                    IsReflowTest = false,
                    IsReflowTestWirebond = false,
                    IsElasticityTest = false,
                    IsDieShearLot = false,
                    MoveStockCt = 0,
                    BeforeLifeTestCondCd = "",
                    TempCutBlendId = 0,
                    TempCutBlendNo = ""
                };

                lot.Update();

            }
            catch(Exception ex)
            {
                ConsoleShow(ex.Message + CR);
                ConsoleShow("ロットの登録が失敗しています");
                return;
            }


            //
            // MAGをインサート
            //
            var magno = txt_dbcnt_lotno.Text;

            if (!EditTnMag(magno, lotno, int.Parse(cb_procno.Items[0].ToString()), bdqty, ref msg))
            {
                ConsoleShow("マガジン情報登録が失敗しました");
                return;
            }

            ConsoleShow(lotno + "をインサートしました");
        }

        private void btn_instarans_Click(object sender, EventArgs e)
        {
            if (!CheckTxtBox())
            {
                return;
            }

            var lotno = txt_dbcnt_lotno.Text;

            if (!IsActiveLot(lotno))
            {
                ConsoleShow("指定のロットが登録されていません");
                return;
            }

            CleanTrans();

            DbControls dbcnt = new DbControls();
            var dblogfpath = "C:\\ARMS\\DB_cob\\";
            var fname_tntran = "tntran_" + cmb_typecd.Text + ".csv";
            var tntranloglist = new List<string>();
            var magno = txt_debugmagheader.Text + txt_magno.Text;
            var swapStrings = new Dictionary<string, string>
                {
                    { "@lotno", lotno },
                    { "@magno", magno }
                };
            var curProcno = string.Empty;
            var bdqty = int.Parse(txt_bdqty.Text);

            try
            {
                //////////////////////////
                // TnTranインサート
                //////////////////////////

                // トランザクションダミーデータ読込
                dbcnt.ReadTextFileLine(dblogfpath + fname_tntran, ref tntranloglist);

                // Procno毎に処理
                for (int i = 0; i < cb_procno.SelectedIndex; i++)
                {
                    // TnTran
                    foreach (var log in tntranloglist)
                    {
                        var _log = log;
                        foreach (var item in swapStrings)
                        {
                            _log = _log.Replace(item.Key, item.Value);
                        }
                        var tran = _log.Split(',');
                        var procno = int.Parse(tran[1]);
                        curProcno = cb_procno.Items[i].ToString();
                        var dt = DateTime.Now;
                        var dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");

                        if (procno == int.Parse(curProcno))
                        {
                            tntran.lotno = tran[0];
                            tntran.procno = tran[1];
                            tntran.macno = tran[2];
                            tntran.inmag = tran[3];
                            tntran.outmag = tran[4];
                            tntran.startdt = dtformed;
                            tntran.enddt = dtformed;
                            tntran.iscomplt = tran[7];
                            tntran.stocker1 = null;
                            tntran.stocker2 = null;
                            tntran.comment = tran[10];
                            tntran.transtartempcd = tran[11];
                            tntran.trancompempcd = tran[12];
                            tntran.inspectempcd = tran[13];
                            tntran.inspectct = tran[14];
                            tntran.isnascastart = tran[15];
                            tntran.isnascaend = tran[16];
                            tntran.lastupddt = dtformed;
                            tntran.isnascadefectend = null;
                            tntran.isnascacommentend = null;
                            tntran.delfg = tran[20];
                            tntran.isdefectend = tran[21];
                            tntran.isdefectautoimportend = tran[22];
                            tntran.isnascarunning = tran[23];
                            tntran.isautoimport = tran[24];
                            tntran.isresinmixordered = tran[25];

                            if (tntran.InsertTnTran(ref msg))
                            {
                                ConsoleShow($"TnTran: ProcNo.{procno}⇒OK");
                            }
                            else
                            {
                                ConsoleShow(msg);
                                ConsoleShow($"TnTran: ProcNo.{procno}⇒NG");
                                ConsoleShow("ダミー実績登録が失敗しています");
                                return;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ConsoleShow("ダミー実績登録が失敗しています");
                return;
            }


            //////////////////////////
            // TnMagインサート
            //////////////////////////
            ///
            if (!EditTnMag(magno, lotno, int.Parse(cb_procno.Items[cb_procno.SelectedIndex-1].ToString()), bdqty, ref msg))
            {
                ConsoleShow("マガジン情報登録が失敗しました");
                return;
            }
        }

        private void btn_deltrans_Click(object sender, EventArgs e)
        {
            if (!CheckTxtBox())
            {
                return;
            }

            CleanTrans();
        }

        private void btn_dellot_Click(object sender, EventArgs e)
        {
            if (!CheckTxtBox())
            {
                return;
            }

            string key;
            string value;

            //////////////////////////
            // TnLot削除
            //////////////////////////
            key = "lotno";
            value = txt_dbcnt_lotno.Text;
            if (tnlot.DeleteTnlot(key, value, ref msg))
            {
                ConsoleShow("TnLotからLotNo: " + value + "を削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }

            //////////////////////////
            // TnLot_Lens削除
            //////////////////////////
            key = "lotno";
            value = txt_dbcnt_lotno.Text;
            if (tnlotlens.DeletTnLot_Lens(key, value, ref msg))
            {
                ConsoleShow("TnLot(Lens)からLotNo: " + value + "を削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }

        }

        private void CleanTrans()
        {
            string key;
            string value;

            //////////////////////////
            // TnTran削除
            //////////////////////////
            key = "lotno";
            value = txt_dbcnt_lotno.Text;
            if (tntran.DeleteTnTran(key, value, ref msg))
            {
                ConsoleShow("TnTranからLotNo: " + value + "を削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }

            //////////////////////////
            // TnMag削除
            //////////////////////////
            key = "lotno";
            value = txt_dbcnt_lotno.Text;
            if (tnmag.DeleteTnMag(key, value, ref msg))
            {
                ConsoleShow("TnMagからLotNo: " + value + "を削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }

            //////////////////////////
            // TnVirtualMag削除
            //////////////////////////
            key = "magno";
            value = txt_dbcnt_lotno.Text;
            if (tnvtmag.DeleteTnVirtualMag(key, value, ref msg))
            {
                ConsoleShow("TnVirtualMagからmagno: " + value + " のVTマガジンを削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }

            value = txt_debugmagheader.Text + txt_magno.Text;
            if (tnvtmag.DeleteTnVirtualMag(key, value, ref msg))
            {
                ConsoleShow("TnVirtualMagからmagno: " + value + " のVTマガジンを削除しました");
            }
            else
            {
                ConsoleShow(msg);
            }
        }
    }
}
