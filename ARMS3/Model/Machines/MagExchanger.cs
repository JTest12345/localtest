using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 移載機(反転含む)
    /// </summary>
    public class MagExchanger : MachineBase
    {
        /// <summary>
        /// 排出モード信号（工程別）
        /// </summary>
        public SortedList<string, string> ProcDischargeAddressList { get; set; }

        /// <summary>
        /// マガジンシフト設定用アドレス
        /// BIT_ON=段替え動作あり、BIT_OFF=1:1で詰め替え
        /// </summary>
        public string MagShiftEnableBitAddress { get; set; }

        /// <summary>
        /// アンローダーマガジン反転ユニット設定用アドレス
        /// </summary>
        public string UnloaderMagReverseBitAddress { get; set; }

        List<IMachine> moldlist = new List<IMachine>();

        public MagExchanger() 
            : base()
        {
            this.ProcDischargeAddressList = new SortedList<string, string>();
        }

        protected override void concreteThreadWork()
        {
            if (base.IsRequireOutputEmptyMagazine() == true)
            {
                workComplete();
            }

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            //
            // debug code by juni
            //
            // empMagUnloaderReqBitAddressを落とします
            Plc.SetBit("B0002DD",1, "0");
            // code end

            string retv = Plc.GetBit(this.UnloaderMagReverseBitAddress);
            if (retv == Mitsubishi.BIT_ON) return;

            //この装置だけは2マガジン詰め替えのため空マガジン排出ONでWorkCompleteに入る
            VirtualMag ldmag = this.Peek(Station.Loader);
            if (ldmag == null) return;

            //1マガジン目は完了させる
            //2マガジン目は再度完了させるためにULのマガジンを更新
            VirtualMag unloadermag = this.Peek(Station.Unloader);
            if (unloadermag != null)
            {
                if (unloadermag.LastMagazineNo == ldmag.MagazineNo)
                {
                    return;
                }
                else
                {
                    //UL仮想マガジンを未完了状態に更新してLDを削除
                    unloadermag.NextMachines.Clear();
                    unloadermag.WorkComplete = DateTime.Now;
                    unloadermag.LastMagazineNo = ldmag.MagazineNo;
                    unloadermag.ProcNo = ldmag.ProcNo;

                    unloadermag.Updatequeue();
                    
                    Dequeue(Station.Loader);

                    base.WorkComplete(unloadermag, this, true);

                    return;
                }
            }

            VirtualMag newmag = this.Peek(Station.EmptyMagazineLoader);
            VirtualMag oldmag = this.Peek(Station.Loader);

            if (newmag == null)
            {
                return;
            }

            if (oldmag == null)
            {
                return;
            }

            newmag.LastMagazineNo = oldmag.MagazineNo;
            newmag.ProcNo = oldmag.ProcNo;
            newmag.WorkStart = oldmag.WorkStart;
            newmag.WorkComplete = DateTime.Now;

            oldmag.InitializeWorkData();
            oldmag.ProcNo = null;

            this.Enqueue(newmag, Station.Unloader);
            this.Dequeue(Station.EmptyMagazineLoader);
            this.Dequeue(Station.Loader);

            base.WorkComplete(newmag, this, true);
        }

        /// <summary>
        /// 投入要求　可否確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns>結果</returns>
        public override bool CanInput(VirtualMag mag)
        {
            bool retv = base.CanInput(mag);
            if (retv == false) return retv;

            Process.MagazineDevideStatus mst = Process.GetMagazineDevideStatus(mag.MagazineNo, mag.ProcNo.Value);

            if (mst == Process.MagazineDevideStatus.DoubleToSingle)
            {
                VirtualMag ulmag = Peek(Station.Unloader);
                if (ulmag == null)
                {
                    //UL側にマガジンが無い場合
                    //即座に供給可能な位置にセットのマガジンがあるかを判定
                    #region

                    //判定中マガジンのロット確定
                    Magazine svrmagorg = Magazine.GetCurrent(mag.MagazineNo);
                    AsmLot svrlotorg = AsmLot.GetAsmLot(svrmagorg.NascaLotNO);

                    #region 投入CV、モールド(遠心沈降機)、バッファーから即座に供給可能な仮想マガジンのリストを作成

                    //途中投入
                    List<VirtualMag> maglist = new List<VirtualMag>();

                    IMachine conveyor = LineKeeper.GetMachine(Route.GetLoadConveyor(this.MacNo));
                    if (conveyor != null && conveyor.IsRequireOutput())
                    {
                        maglist.Add(conveyor.Peek(Station.Unloader));
                    }

                    //バッファー
                    var buffers = LineKeeper.Machines.Where(m => m is GeneralBuffer);
                    foreach (GeneralBuffer bf in buffers)
                    {
                        if (bf.IsRequireOutput() == true)
                        {
                            maglist.Add(bf.Peek(Station.Unloader));
                        }
                    }

                    //WorkFlowに遠心沈降が有る場合=遠心沈降機のmaglist
                    //                    無い場合=モールド機のmaglist
                    List<Process> workflow = Process.GetWorkFlow(svrlotorg.TypeCd).ToList();
                    if (Config.Settings.EckWorkCdList != null && workflow.Exists(w => Config.Settings.EckWorkCdList.Contains(w.WorkCd)))
                    {
                        //遠心沈降機
                        var ecks = LineKeeper.Machines.Where(m => m is ECK);
                        foreach (ECK eck in ecks)
                        {
                            if (eck.IsRequireOutput())
                            {
                                maglist.Add(eck.Peek(Station.Unloader));
                            }
                        }
						ecks = LineKeeper.Machines.Where(m => m is ECK3);
						foreach (ECK3 eck in ecks)
						{
							if (eck.IsRequireOutput())
							{
								maglist.Add(eck.Peek(eck.GetUnLoaderLocation().Station));
							}
						}
                    }
                    else
                    {
                        //モールド機
                        var molds = LineKeeper.Machines.Where(m => m is Mold);
                        foreach (Mold mold in molds)
                        {
                            if (mold.IsRequireOutput())
                            {
                                maglist.Add(mold.Peek(Station.Unloader));
                            }
                        }
                    }

                    #endregion

                    bool hasDouble = false;
                    foreach (VirtualMag m in maglist)
                    {
                        if (m == null || m.MagazineNo == mag.MagazineNo) continue;

                        Magazine svrmag = Magazine.GetCurrent(m.MagazineNo);
                        if (svrmag == null) continue;
						
                        AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

                        if (svrlot != null &&svrlot.NascaLotNo == svrlotorg.NascaLotNo)
                        {
                            hasDouble = true;
                            break;
                        }
                    }

                    if (hasDouble)
                    {
                        //マガジンのシフト動作あり
                        Plc.SetBit(MagShiftEnableBitAddress, 1, Mitsubishi.BIT_ON);
                        return true;
                    }
                    return false;
                    #endregion
                }
                else
                {
                    //UL側にマガジンがある場合は
                    //供給しようとしているマガジンとのロット一致判定
                    #region
                    Magazine svrulmag = Magazine.GetCurrent(ulmag.MagazineNo);
                    if (svrulmag == null)
                    {
                        //UL側マガジンに稼働フラグ無ければ投入NG
                        //排出状態で稼働フラグ無=完了登録待ち状態
                        return false;
                    }
					AsmLot svrullot = AsmLot.GetAsmLot(svrulmag.NascaLotNO);

                    Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                    if (svrmag == null)
                    {
                        //稼働フラグ無の場合は投入不可
                        return false;
                    }
					AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

                    if (svrullot.NascaLotNo != svrlot.NascaLotNo)
                    {
                        //UL側マガジンと供給予定マガジンのロット番号が違う場合は供給不可
                        return false;
                    }
                    //マガジンのシフト動作あり
                    Plc.SetBit(MagShiftEnableBitAddress, 1, Mitsubishi.BIT_ON);
                    return true;
                    #endregion
                }
            }
            else
            {
                VirtualMag ulmag = Peek(Station.Unloader);

                //UL側にマガジンが有り、ULマガジンNOとInputマガジンNOが違う場合はNG
                if (ulmag != null && mag.MagazineNo != ulmag.MagazineNo) return false;

                //マガジンのシフト動作なしフラグセット
                Plc.SetBit(MagShiftEnableBitAddress, 1, Mitsubishi.BIT_OFF);

				//仮想マガジン上の次工程が移載機(反転)の場合、アンローダマガジン反転フラグをtrue
				Process currentProc = Process.GetProcess(mag.ProcNo.Value);
				if (currentProc.WorkCd == Config.Settings.MagExchangerReverseWorkCd)
				{
					Plc.SetBit(UnloaderMagReverseBitAddress, 1, Mitsubishi.BIT_ON);
				}
				else
				{
					Plc.SetBit(UnloaderMagReverseBitAddress, 1, Mitsubishi.BIT_OFF);
				}

                return true;
            }
        }

        public override bool IsDischargeMode(VirtualMag mag)
        {
            if (mag == null) return false;

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null) return false;

            string proc = svrmag.NowCompProcess.ToString();
            if (string.IsNullOrEmpty(proc)) return false;

            if (ProcDischargeAddressList.Keys.Contains(proc) == false) return false;
            string address = ProcDischargeAddressList[proc];

            if (string.IsNullOrEmpty(address)) return false;

            string retv = Plc.GetBit(address);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 空マガジン配置
        /// 基板移載機の空マガジンはモールドの空マガジン排出からのみ取得する
        /// 空マガジン投入CVからは拾わない
        /// </summary>
        /// <returns>結果</returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            //供給禁止状態なら処理しない
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (moldlist.Count == 0)
            {
                foreach (IMachine mac in LineKeeper.Machines)
                {
                    if (mac is Machines.Mold)
                    {
                        moldlist.Add(mac);
                    }
                }
            }

            #region 通常の空マガジンをモールド機の空マガジン排出から取得

            if (this.IsRequireEmptyMagazine() == true)
            {
                Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
                Location from = null;

				int eulMagCt = 0;

                foreach (IMachine mac in moldlist)
                {
                    if (mac.IsRequireOutputEmptyMagazine())
                    {
                        VirtualMag mag = mac.Peek(Station.EmptyMagazineUnloader);
                        if (mag == null) continue;

                        Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                        if (svrmag != null)
                        {
                            //現在稼働中フラグのあるマガジンなら無視する
                            continue;
                        }

						int ct = ((Mold)mac).GetMagazineCount(Station.EmptyMagazineUnloader);
						if (ct >= eulMagCt)
						{
							from = new Location(mac.MacNo, Station.EmptyMagazineUnloader);
							eulMagCt = ct;
						}
					}
                }

                if (from != null)
                {
                    LineKeeper.MoveFromTo(from, to, true, true, false);
                    return true;
                }
                else
                {
                    // 空マガジン投入コンベアの状態確認
                    IMachine conveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                    if (conveyor.IsRequireOutputEmptyMagazine() == true)
                    {
                        from = new Location(conveyor.MacNo, Station.EmptyMagazineUnloader);
                        LineKeeper.MoveFromTo(from, to, false, true, false);
                    }
                }
            }
            #endregion

            #region 遠心沈降用の空マガジンをモールド機の空マガジン供給に移載、要求無しなら排出CV行き
            if (this.IsRequireOutputEmptyMagazine() == true)
            {
                //LDに仮想マガジンが残った状態なら排出要求ONでも空マガジン排出しない
                //センサースレッドが作業完了を検知してマガジン移し替え処理を行った後でLD側はDequeueされる
                VirtualMag ldmag = Peek(Station.Loader);
                if (ldmag == null)
                {
                    //常に排出CV行きで移動開始。
                    //MoveFromTo内でモールド要求に応じてモールド機へ振り替える処理あり（途中投入CV兼用）
                    IMachine conveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                    Location to = conveyor.GetLoaderLocation();
                    Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    LineKeeper.MoveFromTo(from, to, false, true, false);
                    return true;
                }
            }
            #endregion

            #region 反転移載用のUL側に空マガジンが置かれている場合の処理

            string retv = Plc.GetBit(this.UnloaderMagReverseBitAddress);
            if (retv == Mitsubishi.BIT_ON && this.IsRequireOutput() == true)
            {
                VirtualMag ldmag = Peek(Station.Loader);

                Process process = Process.GetProcess(ldmag.ProcNo.Value);
                if (process.WorkCd == Config.Settings.MagExchangerReverseWorkCd)
                {
                    //LD側に仮想マガジンが存在する状態からの移動になる為、先にLD側の仮想マガジンをDequeue
                    Dequeue(Station.EmptyMagazineLoader);

                    //UL側からLD側への移動命令で反転ユニットへ
                    Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
                    Location from = new Location(this.MacNo, Station.Unloader);
                    LineKeeper.MoveFromTo(from, to, false, true, false);
                    return true;
                }
            }
            #endregion

            return false;
        }
    }
}
