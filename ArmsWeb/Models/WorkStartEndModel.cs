﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;


namespace ArmsWeb.Models
{
    public class WorkStartEndModel
    {
        public WorkStartEndModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
            this.MagList = new List<Magazine>();
			this.NeedInspectionWhenCompleteLotList = new List<string>();
        }

        public void AddMagazine(ArmsApi.Model.Magazine mag)
        {
            this.MagList.Add(mag);
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        public MachineInfo Mac { get; set; }

        public Magazine LastReadMag { get; set; }

        public List<ArmsApi.Model.Magazine> MagList { get; set; }

        /// <summary>
        /// 仮想マガジンリスト　キー=NascaLotNo
        /// </summary>
        //public Dictionary<string, VirtualMag> VirtualMags { get; set; }

        /// <summary>
        /// 1:1完成時のみ
        /// 案ローダー側マガジンプレート情報
        /// </summary>
        public string UnloaderMagNo { get; set; }

		public List<string> NeedInspectionWhenCompleteLotList { get; set; }

        public static bool IsDoubleToSingleProcess(Magazine mag)
        {
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    throw new ApplicationException("ロット情報が存在しません");
                }

                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    throw new ApplicationException("工程情報が存在しません");
                }

                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                if (dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Magazine FindOtherMag(Magazine mag)
        {
            string lotno = Order.MagLotToNascaLot(mag.NascaLotNO);
            Magazine[] mags = Magazine.GetMagazine(null, lotno, true, null);
            return mags.Where(m => m.MagazineNo != mag.MagazineNo).FirstOrDefault();
        }


        /// <summary>
        /// 全マガジンを完了
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool WorkEnd(out List<string> errMsg)
        {
            bool isSuccess = true;
            errMsg = new List<string>();

            foreach (Magazine mag in this.MagList)
            {
                //念のため最新情報取得
                ArmsApi.Model.Magazine mag2 = Magazine.GetCurrent(mag.MagazineNo);

                if (mag2 == null)
                {
                    errMsg.Add("マガジン情報が見つかりません:" + mag.MagazineNo);
                    isSuccess = false;
                    continue;
                }

                string lotlog;
                bool lotSuccecc = endMag(mag2, out lotlog, false);
                if (!lotSuccecc)
                {
                    isSuccess = false;
                    errMsg.Add(lotlog);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 開始前チェックを実施。
        /// 実際の登録は行わない
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckBeforeStart(Magazine mag, out string msg)
        {
            return endMag(mag, out msg, true);
        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool endMag(Magazine mag, out string msg, bool isBeforeStartCheckOnly)
        {
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    msg = "ロット情報が存在しません";
                    return false;
                }

                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    msg = "工程情報が存在しません";
                    return false;
                }

                // ダミー作業実績登録の可・不可の判定
                bool insertDummyTranFg = false;
                int nowprocno = mag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                if (WorkChecker.IsNotRelatedMachineAndProcess(this.Mac.MacNo, p.ProcNo) == true)
                {
                    Process oldProc = p;
                    p = Process.GetNextProcess(p.ProcNo, lot);
                    if (p != null && WorkChecker.IsNotRelatedMachineAndProcess(this.Mac.MacNo, p.ProcNo) == false)
                    {
                        bool needDummyInsertBeforeWork = WorkChecker.CanDummyAutoTran(lot, p.ProcNo);
                        if (needDummyInsertBeforeWork == true)
                        {
                            if (!isBeforeStartCheckOnly)
                            {
                                // 登録可の場合、このタイミングでダミー実績登録を実施
                                Process dummyProc = Process.GetPrevProcess(p.ProcNo, lot.TypeCd);
                                Order.RecordDummyWork(lot, dummyProc.ProcNo, dummyProc.AutoUpdMachineNo.Value, mag);
                                insertDummyTranFg = true;
                            }
                        }
                    }
                    else
                    {
                        p = oldProc;
                    }
                }

                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                
                Order order = Order.GetMagazineOrder(mag.NascaLotNO, p.ProcNo);
                if (order == null || dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order = new Order();
                    order.LotNo = mag.NascaLotNO;
                    order.ProcNo = p.ProcNo;
                    order.InMagazineNo = mag.MagazineNo;
                }


                if (!string.IsNullOrEmpty(this.UnloaderMagNo))
                {
                    order.OutMagazineNo = this.UnloaderMagNo;
                }
                else
                {
                    order.OutMagazineNo = mag.MagazineNo;
                }

                order.MacNo = this.Mac.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = DateTime.Now;

                order.TranStartEmpCd = this.EmpCd;
                order.TranCompEmpCd = this.EmpCd;
                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }

                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }
                string errMsg;

                //IsErrorBeforeStartは分割ロット番号で確認する
                bool isStartError = WorkChecker.IsErrorBeforeStartWork(lot, Mac, order, p, out errMsg);

                if (dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order.LotNo = Order.MagLotToNascaLot(order.LotNo);
                }

                if (isStartError)
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }

                    msg = errMsg;
                    return false;
                }

                if (isBeforeStartCheckOnly)
                {
                    msg = "";
                    return true;
                }
                else
                {
                    bool isError = WorkChecker.IsErrorWorkComplete(order, Mac, lot, out errMsg);
                    //if (isCheckOnly)
                    //{
                    //    if (isError)
                    //    {
                    //        msg = errMsg;
                    //        return false;
                    //    }
                    //    else 
                    //    {
                    //        msg = "";
                    //        return true;
                    //    }
                    //}
                    //else
                    //{
                    if (isError)
                    {
                        msg = errMsg + " ロットは完了しましたが警告状態になっています。";
                        lot.IsWarning = true;
                        lot.Update();
                        order.Comment += errMsg;
                        order.DeleteInsert(order.LotNo);

                        // インラインマガジンロット更新
                        mag = Magazine.ApplyMagazineInOut(order, order.LotNo);

                        //状態検査必要フラグ
                        Inspection isp = Inspection.GetInspection(order.NascaLotNo, order.ProcNo);
						if (isp != null && isp.IsInspected == false) this.NeedInspectionWhenCompleteLotList.Add(order.NascaLotNo);

                        //TODO ワイヤー号機の判定がワイヤーの文字で行われているLineConfigをDB化必要
                        if (this.Mac.ClassName.StartsWith("ワイヤー"))
                        {
                            VirtualMag evmag = new VirtualMag();
                            evmag.MagazineNo = order.OutMagazineNo;
                            evmag.LastMagazineNo = order.InMagazineNo;
                            evmag.ProcNo = order.ProcNo;

                            //TODO 空マガジンローダーを使って擬似的にマッピングデータの集計を行っている。別の仕組みが必要
                            evmag.Enqueue(evmag, new Location(this.Mac.MacNo, Station.EmptyMagazineLoader));
                        }

                        return false;
                    }
                    else
                    {
                        order.DeleteInsert(order.LotNo);

                        // インラインマガジンロット更新
                        mag = Magazine.ApplyMagazineInOut(order, order.LotNo);
                        msg = "";

                        Process nextproc = Process.GetNextProcess(order.ProcNo, lot);
                        //次工程が抜き取り検査工程の場合
                        if (nextproc != null && nextproc.IsSamplingInspection == true)
                        {
                            //検査工程の抜き取り指示有無再取得
                            Inspection isp = Inspection.GetInspection(lot.NascaLotNo, nextproc.ProcNo);
                            if (isp == null)
                            {
                                //抜き取りフラグが無い場合は、ダミー作業を登録
                                Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                            }
                        }

                        // 次工程がロットマーキング作業、先行色調品、作業マスタにダミー設備マスタが登録されている場合、ダミー登録
                        if (nextproc != null && nextproc.IsLotMarking)
                        {
                            if (nextproc.AutoUpdMachineNo.HasValue)
                            {
                                Profile prof = Profile.GetProfile(lot.ProfileId);
                                if (prof != null && prof.ProfileNm.Contains("先行色調"))
                                {
                                    Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                                    nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                                }
                            }
                        }

                        //TODO ワイヤー号機の判定がワイヤーの文字で行われているLineConfigをDB化必要
                        if (this.Mac.ClassName.StartsWith("ワイヤー"))
                        {
                            VirtualMag evmag = new VirtualMag();
                            evmag.MagazineNo = order.OutMagazineNo;
                            evmag.LastMagazineNo = order.InMagazineNo;
                            evmag.ProcNo = order.ProcNo;

                            //TODO 空マガジンローダーを使って擬似的にマッピングデータの集計を行っている。別の仕組みが必要
                            evmag.Enqueue(evmag, new Location(this.Mac.MacNo, Station.EmptyMagazineLoader));
                        }
                        //状態検査必要フラグ
                        Inspection isp2 = Inspection.GetInspection(order.NascaLotNo, order.ProcNo);
						if (isp2 != null && isp2.IsInspected == false) this.NeedInspectionWhenCompleteLotList.Add(order.NascaLotNo);

                        return true;
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }

    }
}