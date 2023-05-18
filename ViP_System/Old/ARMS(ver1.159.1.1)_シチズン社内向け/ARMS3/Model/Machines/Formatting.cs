using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// NTSV 成型機(圧縮成型, TM成型)
    /// </summary>
    public class Formatting : CifsMachineBase
    {
        //<--SGA-IM0000007795
        /// <summary>
        /// 基板Datamatrixの頭8文字が「ZZ000000」であれば、ダミー基板
        /// </summary>
        private const string DUMMYSUBSTRATE_SERCHSTRING = "ZZ000000";
        private const string PRESS_NO_HEADER = "プレスNo";
        private const string IN_PRESS_ADDR_HEADER = "プレス内アドレス";
        //--SGA-IM0000007795

        public Formatting()
        {
        }

        /// <summary>
        /// メインルーチン
        /// </summary>
        protected override void concreteThreadWork()
        {
            try
            {
                //OutputSysLog(string.Format("メインスレッド開始"));

                if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true, false) == true)
                {
                    workComplete();
                }

                workStart();
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw;
                }
            }
        }

        public void workStart()
        {
            //<--SGA-IM0000007795
            //trgファイルに於いて、DM内容がダミー基板であれば、通常の開始判定・実績登録をスキップして判定OKの応答を行う。
            string[] searchfiles = System.IO.Directory.GetFiles(this.LogInputDirectoryPath, DUMMYSUBSTRATE_SERCHSTRING + "*.trg", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in searchfiles)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                string carrierno = Path.GetFileNameWithoutExtension(file);//ファイル名(拡張子(.trg)抜き)

                if (carrierno.StartsWith(DUMMYSUBSTRATE_SERCHSTRING) == true)
                {
                    SendOkFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(file));
                    OutputSysLog(string.Format("[開始処理] ダミー基板:{0} 検知 okファイル送信", carrierno));

                    File.Delete(file);//トリガファイル削除
                    OutputSysLog(string.Format("[開始処理] ダミー基板:{0} 検知 trgファイル削除", carrierno));
                }
            }
            //<--削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆
            string[] tempsearchfiles = System.IO.Directory.GetFiles(this.LogInputDirectoryPath, "*.trg", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string file in tempsearchfiles)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                string carrierno = Path.GetFileNameWithoutExtension(file);//ファイル名(拡張子(.trg)抜き)

                List<string> lotlist = LotCarrier.GetLotNotemp(carrierno);
                if (lotlist.Count == 0)
                {
                    SendOkFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(file));
                    OutputSysLog(string.Format("[開始処理] ダミー基板:{0} 検知 okファイル送信", carrierno));

                    File.Delete(file);//トリガファイル削除
                    OutputSysLog(string.Format("[開始処理] ダミー基板:{0} 検知 trgファイル削除", carrierno));
                }
            }
            //-->削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆
            //-->SGA-IM0000007795

            //OutputSysLog(string.Format("WorkStart開始"));

            List<MachineLog.TriggerFile6> trgList = MachineLog.TriggerFile6.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                //OutputSysLog(string.Format("トリガファイル無しのためWorkStartスルー"));
                return;
            }

            MachineLog.TriggerFile6 newTrg = trgList.OrderByDescending(t => t.LastUpdDt).First();
            OutputSysLog(string.Format("[開始処理] 開始 trgファイル取得成功 FileName:{0}", newTrg.FullName));

            try
            {
                AsmLot lot = AsmLot.GetAsmLot(newTrg.NascaLotNo);

                Magazine svrMag = Magazine.GetMagazine(lot.NascaLotNo);
                if (svrMag == null)
                {
                    throw new Exception(string.Format("稼働中マガジンが存在しない為、開始登録ができません。LotNo:{0}",
                        lot.NascaLotNo));
                }

　              Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (p == null)
                {
                    throw new Exception(string.Format("稼動中マガジンの次作業Noが存在しない為、開始登録ができません。LotNo:{0} 稼動中マガジン完了工程No:{1}",
                        lot.NascaLotNo, svrMag.NowCompProcess));
                }

                //同ロットで他装置での開始実績が有ると開始処理エラーにする。
                List<Order> orderList = ArmsApi.Model.Order.SearchOrder(lot.NascaLotNo, p.ProcNo, null, true, false).ToList();

                if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
                {
                    SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), "作業開始登録失敗 ");
                    throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
                        string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
                }

                VirtualMag vMag = new VirtualMag();
                vMag.MagazineNo = svrMag.MagazineNo;
                vMag.LastMagazineNo = svrMag.MagazineNo;
                vMag.ProcNo = p.ProcNo;

                Order order = CommonApi.GetWorkStartOrder(vMag, this.MacNo);

                // 1基板目の開始時間をマガジンの開始時間とする為、既に開始実績が存在する場合(2基板目以降)は登録しない
                Order[] startedOrder = Order.GetOrder(lot.NascaLotNo, p.ProcNo);

                if (startedOrder.Count() == 0)
                {
                    ArmsApiResponse workResponse = CommonApi.WorkStart(order);
                    if (workResponse.IsError)
                    {
                        SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), workResponse.Message);
                        Log.ApiLog.Info(string.Format("[{0}] 成形機 START ERROR {1}", this.MacNo, workResponse.Message));
                        return;
                    }
                }
                else
                {
                    //既に履歴がある場合でも開始登録チェックは毎基板実施する
                    MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
                    order.LotNo = lot.NascaLotNo;
                    order.ProcNo = p.ProcNo;
                    order.InMagazineNo = svrMag.MagazineNo;
                    order.MacNo = this.MacNo;
                    order.WorkStartDt = DateTime.Now;
                    order.WorkEndDt = null;

                    string errMsg;
                    bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                    if (isError)
                    {
                        SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), errMsg);
                        Log.ApiLog.Info(string.Format("[{0}] 成形機 START ERROR {1}", this.MacNo, errMsg));
                        return;
                    }
                }

                SendOkFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName));
            }
            catch (Exception err)
            {
                SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), "作業開始登録失敗 ");
                throw new Exception(err.ToString());
            }
        }

        public void workComplete()
        {
            //<--SGA-IM0000007795
            //ダミー基板のfinファイルが出力された場合、finとwedを合わせて削除する。（動作ログに削除ログは残す）
            string[] searchfiles = System.IO.Directory.GetFiles(this.LogOutputDirectoryPath, DUMMYSUBSTRATE_SERCHSTRING + "*.fin", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string finfile in searchfiles)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(finfile);
                string carrierno = Path.GetFileNameWithoutExtension(finfile);//ファイル名(拡張子(.fin)抜き)
                if (carrierno.StartsWith(DUMMYSUBSTRATE_SERCHSTRING) == true)
                {
                    string wedfile = Path.Combine(Path.GetDirectoryName(finfile), carrierno + ".wed");
                    //wedファイル削除
                    if (System.IO.File.Exists(wedfile))
                    {
                        File.Delete(wedfile);
                        OutputSysLog(string.Format("[完了処理] ダミー基板:{0} 検知 wedファイル削除", carrierno));
                    }

                    //finファイル削除
                    File.Delete(finfile);
                    OutputSysLog(string.Format("[完了処理] ダミー基板:{0} 検知 finファイル削除", carrierno));
                }
            }

            //<--削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆
            string[] searchfilestmp = System.IO.Directory.GetFiles(this.LogOutputDirectoryPath, "*.fin", System.IO.SearchOption.TopDirectoryOnly);
            foreach (string finfile in searchfilestmp)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(finfile);
                string carrierno = Path.GetFileNameWithoutExtension(finfile);//ファイル名(拡張子(.fin)抜き)

                //<--削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。
                List<string> lotlist = LotCarrier.GetLotNotemp(carrierno);
                if (lotlist.Count == 0)
                {
                    string wedfile = Path.Combine(Path.GetDirectoryName(finfile), carrierno + ".wed");
                    //wedファイル削除
                    if (System.IO.File.Exists(wedfile))
                    {
                        File.Delete(wedfile);
                        OutputSysLog(string.Format("[完了処理] ダミー基板:{0} 検知 wedファイル削除", carrierno));
                    }

                    //finファイル削除
                    File.Delete(finfile);
                    OutputSysLog(string.Format("[完了処理] ダミー基板:{0} 検知 finファイル削除", carrierno));
                }
            }
            //-->削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆★削除☆
            //-->SGA-IM0000007795

            List<MachineLog.FinishedFile9> finList = MachineLog.FinishedFile9.GetAllFiles(this.LogOutputDirectoryPath);

            //更新時間順に並べ替え
            finList = finList.OrderBy(f => File.GetLastWriteTime(f.FullName_Fin)).ToList();

            foreach (MachineLog.FinishedFile9 fin in finList)
            {
                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);

                OutputSysLog(string.Format("[完了処理] 開始 LotNo:{0}", lot.NascaLotNo));

                VirtualMag mag = new VirtualMag();

                Magazine svrMag = Magazine.GetMagazine(fin.NascaLotNo);
                if (svrMag == null)
                {
                    throw new Exception(string.Format("稼働中マガジンが存在しない為、完了処理ができませんでした。LotNo:{0} ",
                        lot.NascaLotNo));
                }

                //工程取得
                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (p == null)
                {
                    throw new Exception();
                }
                mag.ProcNo = p.ProcNo;

                mag.MagazineNo = svrMag.MagazineNo;
                mag.LastMagazineNo = svrMag.MagazineNo;

                Order[] order = Order.GetOrder(lot.NascaLotNo, p.ProcNo);
                if (order.Count() == 0)
                {
                    throw new Exception(string.Format(@"作業開始登録が存在しない為、完了処理ができませんでした。
                        データメンテナンスより開始登録を行い、再度この装置を開始(稼働中)にして下さい。 LotNo:{0}", fin.NascaLotNo));
                }

                mag.WorkStart = order.Single().WorkStartDt;
                mag.WorkComplete = System.DateTime.Now;



                VirtualMag vMag = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Unloader, lot.NascaLotNo);
                List<string> carrierFileList = MachineLog.GetFiles(this.LogOutputDirectoryPath, "^" + fin.CarrierNo + ".*$");

                //段数情報登録
                RegisterCarrierData(carrierFileList, fin.CarrierNo, mag, lot);

                if (vMag == null)
                {
                    this.Enqueue(mag, Station.Unloader);
                }
                else
                {
                    vMag.WorkComplete = System.DateTime.Now;
                    vMag.Updatequeue();
                }

                foreach (string file in carrierFileList)
                {
                    MachineLog.ChangeFileNameCarrier(file, lot.NascaLotNo, lot.TypeCd, p.ProcNo, mag.MagazineNo, fin.CarrierNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", file));
                }
                                
                OutputSysLog(string.Format("[完了処理] 完了 LotNo:{0}", fin.NascaLotNo));
            }
        }

        private void RegisterCarrierData(List<string> fileList, string carrierNo, VirtualMag mag, AsmLot lot)
        {
            //段数情報登録
            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = lot.NascaLotNo;
            regData.ProcNo = Convert.ToInt32(mag.ProcNo);
            regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
            regData.Delfg = 0;
            regData.CarrierNo = carrierNo;
            regData.RegisterMagazineStepWithAutotCalc();


            string wedFile = fileList.Where(f => Path.GetExtension(f).ToUpper() == "." + MachineLog.FinishedFile9.WORKENDFILE_IDENTITYNAME.ToUpper()).FirstOrDefault();

            if (wedFile == null) return;

            //ファイル内容からプレスアドレスと金型アドレスを取得
            string[] content = File.ReadAllLines(wedFile);

            string pressNo = string.Empty;
            string inPressAddr = string.Empty;

            foreach (string s in content)
            {
                string[] fileData = s.Split(',');
                if (fileData.Length <= 1)
                {
                    continue;
                }
                
                if (fileData[0].Trim() == PRESS_NO_HEADER)
                {
                    pressNo = fileData[1].Trim();
                    regData.Infoid = CarrireWorkData.PRESS_ADDR_INFOCD;
                    regData.Value = pressNo;
                    regData.InsertUpdate();
                }

                if (fileData[0].Trim() == IN_PRESS_ADDR_HEADER)
                {
                    inPressAddr = fileData[1].Trim();
                    regData.Infoid = CarrireWorkData.IN_PRESSADDR_INFOCD;
                    regData.Value = inPressAddr;
                    regData.InsertUpdate();
                }

                if (string.IsNullOrWhiteSpace(pressNo) == false && string.IsNullOrWhiteSpace(inPressAddr) == false)
                {
                    return;
                }
            }

        }
    }
}
