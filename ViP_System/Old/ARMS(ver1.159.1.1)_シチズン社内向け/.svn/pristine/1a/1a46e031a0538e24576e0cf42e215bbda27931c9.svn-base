using ARMS3.Model.PLC;
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
    /// SQ装置 NTSV用
    /// </summary>
    public class SQMachine : CifsMachineBase
    {
        /// <summary>
        /// データ受付準備OK(DM_LDマガジンID)
        /// </summary>
        private const string STR_ADDRESS_DATA_ACCEPT_OK_MAGAZINE = "EM50001";

        /// <summary>
        /// LDマガジンID装置停止要求(0：継続 1：停止)
        /// </summary>
        private const string STR_ADDRESS_REQUEST_STOP_MAGAZINE = "EM50021";

        /// <summary>
        /// データ確認要求(DM_LDマガジンID)
        /// </summary>
        private const string STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE = "EM50051";

        /// <summary>
        /// LDマガジンDM
        /// </summary>
        private const string STR_ADDRESS_LD_MAGAZINE = "EM50900";

        /// <summary>
        /// ULDマガジンDM
        /// </summary>
        private const string STR_ADDRESS_ULD_MAGAZINE = "EM51590";

        /// <summary>
        /// データ確認要求(SM_製品出来栄え)
        /// </summary>
        private const string STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY = "EM50052";

        /// <summary>
        /// マガジンDMの文字数
        /// </summary>
        private const int MAGAZINE_LENGTH = 10;

        protected override void concreteThreadWork()
        {
            try
            {
                //データ受付準備OK(DM_LDマガジンID)ON (ハートビート)
                this.Plc.SetBit(STR_ADDRESS_DATA_ACCEPT_OK_MAGAZINE, 1, Keyence.BIT_ON);

                magazineComplete();

                workStart();

                base.WorkStart();

                workComplete();
                
                Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);

                workComlpeteForEICS();

                //データ受付準備OK(DM_LDマガジンID)OFF (ハートビート)
                this.Plc.SetBit(STR_ADDRESS_DATA_ACCEPT_OK_MAGAZINE, 1, Keyence.BIT_OFF);
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

        /// <summary>
        /// 作業開始登録
        /// </summary>
        private void workStart()
        {
            //開始処理（NASCA自動登録）
            try
            {
                if (!isRequestMagazine())
                {
                    return;
                }

                Log.ApiLog.Info("SQ装置 START BEGIN:" + this.MacNo);

                //string mgno = this.Plc.GetMagazineNo(STR_ADDRESS_MAGAZINE, true);
                string mgno = this.Plc.GetMagazineNo(STR_ADDRESS_LD_MAGAZINE, MAGAZINE_LENGTH);

                Magazine mag = Magazine.GetCurrent(mgno);
                if (mag == null)
                {
                    throw new Exception(string.Format("[{0}] 稼働中ではないマガジンです。magazine:{1}", this.MacNo, mgno));
                }

                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

                Order order = new Order();
                order.LotNo = mag.NascaLotNO;
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = mag.MagazineNo;
                order.MacNo = this.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = null;
                order.TranStartEmpCd = "660";
                order.TranCompEmpCd = "660";

                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }

                string errMsg;

                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                if (!isError)
                {
                    order.DeleteInsert(order.LotNo);

                    //装置停止要求をOFF
                    this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_OFF);

                    //マガジンOFF
                    this.Plc.SetBit(STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE, 1, Keyence.BIT_OFF);
                }
                else
                {
                    //装置停止要求をON
                    this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_ON);

                    Log.ApiLog.Info(string.Format("[{0}] SQ装置 START ERROR {1}", this.MacNo, errMsg));
                }

                Log.ApiLog.Info("SQ装置 START COMPLETE:" + this.MacNo);
            }
            catch (Exception ex)
            {
                //装置停止要求をON
                this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_ON);

                Log.ApiLog.Error("SQ装置 WS" + this.MacNo + "ERROR:", ex);
                throw ex;
            }
        }

        private bool isRequestMagazine()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE);

            if (Convert.ToBoolean(Convert.ToInt32(retv)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// マガジン完了
        /// </summary>
        private void magazineComplete()
        {
            if (!isCheckProductQuality())
            {
                return;
            }


            //VirtualMag ulMagazine = this.Peek(Station.Unloader);
            //if (ulMagazine != null)
            //{
            //    return;
            //}
            //VirtualMag oldmag = this.Peek(Station.Loader);
            //if (oldmag == null)
            //{
            //    return;
            //}

            Log.RBLog.Info("SQ装置");

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = this.Plc.GetMagazineNo(STR_ADDRESS_ULD_MAGAZINE, MAGAZINE_LENGTH);

            Magazine svmag = Magazine.GetCurrent(newmagno);

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                throw new ApplicationException("作業開始レコードが見つかりません mag:" + newmagno + " 装置:" + this.MacNo.ToString());
            }
            Order order = orders.FirstOrDefault();

            VirtualMag newMagazine = new VirtualMag();

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("SQ装置マガジンNOの取得に失敗");
                return;
            }
            newMagazine.LastMagazineNo = order.InMagazineNo;

            //作業IDを取得
            newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newmagno);
            newMagazine.WorkComplete = DateTime.Now;
            newMagazine.WorkStart = order.WorkStartDt;

            this.Enqueue(newMagazine, Station.Unloader);

            //this.WorkComplete(newMagazine, this, true);
        }

        private bool isCheckProductQuality()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY);

            if (Convert.ToBoolean(Convert.ToInt32(retv)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基板完了
        /// </summary>
        private void workComplete()
        {
            List<MachineLog.FinishedFile5> finList = MachineLog.FinishedFile5.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            //更新時間順に並べ替え
            finList = finList.OrderBy(f => File.GetLastWriteTime(f.FullName)).ToList();

            CarrireWorkData stepData = new CarrireWorkData();
            stepData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach (MachineLog.FinishedFile5 fin in finList)
            {
                OutputSysLog(string.Format("[基板完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.DataMatrix);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.DataMatrix, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);

                //段数情報を記録
                stepData.LotNo = lot.NascaLotNo;
                stepData.ProcNo = procno;
                stepData.CarrierNo = fin.DataMatrix;
                stepData.Delfg = 0;
                stepData.RegisterMagazineStepWithAutotCalc();                

                OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                OutputSysLog(string.Format("[基板完了処理] 完了"));
            }
        }

        /// <summary>
        /// EICSが作成したファイル名のリネーム
        /// </summary>
        private void workComlpeteForEICS()
        {
            string newmagno = this.Plc.GetMagazineNo(STR_ADDRESS_ULD_MAGAZINE, MAGAZINE_LENGTH);

			//NTSV対応　20161004 h.fukatani
			if (string.IsNullOrWhiteSpace(newmagno))
			{
				return;
			}

			Magazine svmag = Magazine.GetCurrent(newmagno);

            AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                return;
            }
            Order order = orders.FirstOrDefault();

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, DateTime.Now);
            lotFiles = lotFiles.Where(w => Path.GetExtension(w).ToLower() == "." + MachineLog.FinishedFile6.FINISHEDFILE_IDENTITYNAME).ToList();

            foreach (string lotFile in lotFiles)
            {
                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                //リネイム済みは除外
                if (dataMatrix.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                {
                    continue;
                }

                MachineLog.ChangeFileName(lotFile, svmag.NascaLotNO, lot.TypeCd, order.ProcNo, newmagno);
                OutputSysLog(string.Format("[完了処理] ファイル名称変更 FileName:{0}", lotFile));
            }
        }
    }
}
