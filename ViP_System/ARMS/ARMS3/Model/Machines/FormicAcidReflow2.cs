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
    /// NTSV オリジン製ギ酸リフロー装置
    /// </summary>
    public class FormicAcidReflow2 : CifsMachineBase
    {


        public FormicAcidReflow2()
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

            List<MachineLog.TriggerFile7> trgList = MachineLog.TriggerFile7.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }

            MachineLog.TriggerFile7 newTrg = trgList.OrderByDescending(t => t.LastUpdDt).First();
            OutputSysLog(string.Format("[開始処理] 開始 trgファイル取得成功 FileName:{0}", newTrg.FullName));

            try
            {

　              Process p = Process.GetNextProcess(newTrg.Mag.NowCompProcess, newTrg.Lot);
                if (p == null)
                {
                    throw new Exception(string.Format("稼動中マガジンの次作業Noが存在しない為、開始登録ができません。LotNo:{0} 稼動中マガジン完了工程No:{1}",
                        newTrg.Lot.NascaLotNo, newTrg.Mag.NowCompProcess));
                }

                //同ロットで他装置での開始実績が有ると開始処理エラーにする。
                List<Order> orderList = ArmsApi.Model.Order.SearchOrder(newTrg.Lot.NascaLotNo, p.ProcNo, null, true, false).ToList();

                if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
                {
                    SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), "作業開始登録失敗 ");
                    throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
                        string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
                }

                VirtualMag vMag = new VirtualMag();
                vMag.MagazineNo = newTrg.Mag.MagazineNo;
                vMag.LastMagazineNo = newTrg.Mag.MagazineNo;
                vMag.ProcNo = p.ProcNo;

                Order order = CommonApi.GetWorkStartOrder(vMag, this.MacNo);

                // 登録時に既に実績があっても実績登録（上書き）する。
                ArmsApiResponse workResponse = CommonApi.WorkStart(order);
                if (workResponse.IsError)
                {
                    SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), workResponse.Message);
                    File.Delete(newTrg.FullName);
                    Log.ApiLog.Info(string.Format("[{0}] 蟻酸リフロー炉 START ERROR {1}", this.MacNo, workResponse.Message));
                    return;
                } 

                SendOkFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName));
                File.Delete(newTrg.FullName);
            }
            catch (Exception err)
            {
                SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(newTrg.FullName), "作業開始登録失敗 ");
                File.Delete(newTrg.FullName);
                throw new Exception(err.ToString());
            }
        }

        public void workComplete()
        {

            List<MachineLog.FinishedFile10> finList = MachineLog.FinishedFile10.GetAllFiles(this.LogOutputDirectoryPath);

            //更新時間順に並べ替え
            finList = finList.OrderBy(f => File.GetLastWriteTime(f.FullName_Fin)).ToList();

            foreach (MachineLog.FinishedFile10 fin in finList)
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

                mag.MagazineNo = fin.UnloaderMagNo;
                mag.LastMagazineNo = svrMag.MagazineNo;

                Order[] order = Order.GetOrder(lot.NascaLotNo, p.ProcNo);
                if (order.Count() == 0)
                {
                    throw new Exception(string.Format(@"作業開始登録が存在しない為、完了処理ができませんでした。
                        データメンテナンスより開始登録を行い、再度この装置を開始(稼働中)にして下さい。 LotNo:{0}", fin.NascaLotNo));
                }

                mag.WorkStart = fin.WorkStartDt;
                mag.WorkComplete = fin.WorkEndDt;

                //VirtualMag vMag = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Unloader, lot.NascaLotNo);
                //if (vMag == null)
                //{
                //    this.Enqueue(mag, Station.Unloader);
                //}
                //else
                //{
                //    vMag.WorkComplete = fin.WorkEndDt;
                //    vMag.Updatequeue();
                //}

                if (this.Enqueue(mag, Station.Unloader))
                {
                    this.Dequeue(Station.Loader);
                    base.WorkComplete(mag, this, true);
                    this.Dequeue(Station.Unloader);

                    OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", mag.MagazineNo));
                }

                //finファイルと同名称の全ファイルをリネーム(1行で済むのでMachineLogの関数を増やしたりはしない)
                List<string> lotFiles = Directory.GetFiles(this.LogOutputDirectoryPath).Where(f => Path.GetFileNameWithoutExtension(f) == Path.GetFileNameWithoutExtension(fin.FullName_Fin)).ToList();
                if (lotFiles.Count == 0)
                {
                    return;
                }

                foreach (string lotFile in lotFiles)
                {
                    string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                    //リネイム済みは除外
                    if (dataMatrix.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, p.ProcNo, mag.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ファイル名称変更 FileName:{0}", lotFile));
                }
            }
        }        
    }
}
