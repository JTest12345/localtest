using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System.IO;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 
    /// </summary>
    public class MAP1stDieBonder : DieBonder
    {
        public string DefectFileBasePath { get; set; }
        public string BadMarkDefetCd { get; set; }
        public string BadMarkCauseCd { get; set; }
        public string BadMarkClassCd { get; set; }

        private const string DEFECT_FILE_HEADER = "B";

        public MAP1stDieBonder()
            : base()
        {
        }

        protected override void concreteThreadWork()
        {
            try
            {
                //チェンジャーの交換監視
                if (Plc.GetBit(WaferChangerChangeSensorBitAddress) == Mitsubishi.BIT_ON)
                {
                    waferChangerChange();
                }

                //完了処理
                if (this.IsRequireOutput() == true)
                {
                    FrameTransfer();
					
                    if (this.IsAutoLine)
                    {
                        this.workComplete();
                    }
                    else
                    {
                        if (IsWaitingMagazineTakeout == false)
                        {
                            this.workCompletehigh();
                        }                  

                        if (IsWaitingMagazineTakeout)
                        {
                            //完了位置からマガジンが取り除かれたかのチェック
                            if (Plc.GetBit(MagazineTakeoutBitAddress, 1) == Mitsubishi.BIT_ON)
                            {
                                //アンローダーが動作したかのチェック
                                if (Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Mitsubishi.BIT_ON)
                                {
                                    IsWaitingMagazineTakeout = false;
                                    Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);
                                    Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Mitsubishi.BIT_OFF);
                                }
                            }
                        }
                    }
                }

                //開始処理
                checkWorkStart();
                checkWorkStartLoader();

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);

				////空のMAP基板マガジン排出処理(自動化はResponseEmptyMagazine内で処理)
				//if (this.IsHighLine && this.IsRequireOutputEmptyMagazine() == true)
				//{
				//	FrameTransfer();

				//	VirtualMag lMagazine = Peek(Station.Loader);
				//	if (lMagazine != null && lMagazine.CurrentFrameCt == 0)
				//	{
				//		this.Dequeue(Station.Loader);
				//	}
				//}

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("macno:{0} {1}", this.MacNo, ex.Message));
            }
        }

        #region waferChangerChange アンローダー側マガジンに記録

        /// <summary>
        /// 通常のダイボンダーとは逆に
        /// アンローダー側のマガジンに記録する
        /// </summary>
        /// <param name="plc"></param>
        private void waferChangerChange()
        {
            VirtualMag mag = this.Peek(Station.EmptyMagazineLoader);
            if (mag == null)
            {
                //PLCのBITを0に
                Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);
                //マガジンが無い状態のチェンジャー交換は無視する
                return;
            }

            if (mag.WaferChangerChangeCount.HasValue == false)
            {
                mag.WaferChangerChangeCount = 0;
            }


            //最終のチェンジャー交換から60秒以内の交換信号は無視する
            //センサーの揺らぎ対策
            if (mag.WaferChangerChangeTime.HasValue == true)
            {
                if ((DateTime.Now - mag.WaferChangerChangeTime.Value).TotalSeconds <= 60)
                {
                    //PLCのBITを0に
                    Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);
                    return;
                }
            }

            //インクリメント
            mag.WaferChangerChangeCount += 1;
            mag.WaferChangerChangeTime = DateTime.Now;

            //マガジンファイル更新
            mag.Updatequeue();

            //PLCのBITを0に
            this.Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);

            Log.RBLog.Info(string.Format("ウェハーカセットの交換処理:{0}", this.MacNo));
        }
        #endregion

        #region FrameTransfer

        /// <summary>
        /// 実マガジンに投入フレーム情報を保存する
        /// </summary>
        protected void FrameTransfer()
        {
            VirtualMag emptyMag = this.Peek(Station.EmptyMagazineUnloader);
            if (emptyMag != null)
            {
                //既に空の基板マガジンがある場合は何もしない
                return;
            }

            VirtualMag unloaderMag = this.Peek(Station.Unloader);
            if (unloaderMag != null)
            {
                //既に排出待ちの仮想マガジンがある場合は何もしない
                return;
            }

            VirtualMag frameMag = this.Peek(Station.Loader);
            if (frameMag == null) return;
            if (frameMag.CurrentFrameCt.HasValue == false) throw new ApplicationException("基板マガジン内に現在数量の値がありません");
            if (frameMag.CurrentFrameCt == 0) return;

            VirtualMag workingMag = this.Peek(Station.EmptyMagazineLoader);
            if (workingMag == null) return;
            if (workingMag.CurrentFrameCt.HasValue == false) workingMag.CurrentFrameCt = 0;
            if (workingMag.MaxFrameCt.HasValue == false) throw new ApplicationException("1マガジン内基板数量の設定がありません");

            int charge = workingMag.MaxFrameCt.Value - workingMag.CurrentFrameCt.Value;
            if (charge == 0) return;

            if (frameMag.CurrentFrameCt > charge)
            {
                frameMag.CurrentFrameCt -= charge;
                frameMag.Updatequeue();

                workingMag.RelatedMaterials.Add(Material.GetMaterial(frameMag.FrameMatCd,frameMag.FrameLotNo));
                workingMag.CurrentFrameCt += charge;
                workingMag.Updatequeue();
            }
            else
            {
                workingMag.RelatedMaterials.Add(Material.GetMaterial(frameMag.FrameMatCd,frameMag.FrameLotNo));
                workingMag.CurrentFrameCt += frameMag.CurrentFrameCt;
                workingMag.Updatequeue();

                frameMag.CurrentFrameCt = 0;
                if (this.IsAutoLine)
                {
                    this.Enqueue(frameMag, Station.EmptyMagazineUnloader);
                }
                this.Dequeue(Station.Loader);
            }
        }

        #endregion

        #region workComplete

        /// <summary>
        /// 作業完了処理
        /// </summary>
        public override void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                //完了処理が終了しているので何もしない
                return;
            }

            VirtualMag newMagazine = this.Peek(Station.EmptyMagazineLoader);
            if (newMagazine == null)
            {
                return;
            }

            //フレーム関連付け枚数が搭載数に満たない場合は待機
            if (newMagazine.CurrentFrameCt.Value < newMagazine.MaxFrameCt.Value)
            {
                return;
            }

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", newMagazine.MagazineNo));

            newMagazine.LastMagazineNo = newMagazine.MagazineNo;
            newMagazine.WorkComplete = DateTime.Now;

            //終了時ウェハー段数を取得
            newMagazine.EndWafer = this.GetWaferPlateNo();
            if (newMagazine.StartWafer == null)
            {
                throw new Exception("開始ウェハー段数が取得できていません。");
            }
            Log.ApiLog.Info(string.Format("終了ウェハー段数を設定:{0}", newMagazine.EndWafer.ToString()));

			if (this.Enqueue(newMagazine, Station.Unloader))
			{
				this.Dequeue(Station.EmptyMagazineLoader);

				if (this.LogOutputDirectoryPath != null)
				{
					string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
					if (string.IsNullOrEmpty(lFile))
						throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。MagazineNo:{0}", newMagazine.MagazineNo));

					newMagazine.WorkComplete = File.GetLastWriteTime(lFile);

					base.WorkComplete(newMagazine, this, true);

					//※下記のロットファイル名称変更処理は完了登録後が必須
					//完了登録時にロット番号をマガジン番号からロット番号へ変更しているので、後でないとマガジン番号をロット番号として付与してしまうため
					Magazine svrMag = Magazine.GetCurrent(newMagazine.MagazineNo);
					AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
					OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

					List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, newMagazine.WorkStart.Value, File.GetLastWriteTime(lFile));
					if (lotFiles.Count == 0)
						throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", newMagazine.WorkStart.Value, newMagazine.WorkComplete.Value));

					int procno = Process.GetPrevProcess(newMagazine.ProcNo.Value, lot.TypeCd).ProcNo;
					foreach (string lotFile in lotFiles)
					{
						if (IsStartLogFile(lotFile))
							continue;

                        //EICSでリネーム前のファイルを移動させようとしてエラーになる現象がMAPでちょこちょこ発生しているので
                        //その対策としてLファイルを一番最後にリネームするよう改修する 
                        //EICSではLファイルをリネームしていないと全ファイル移動しないような仕組みになっているので
                        //Lファイルを最後にリネームすることで問題が解決する見込み 2019.01.21
                        if (lotFile.Trim() == lFile.Trim())
                            continue;

                        MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, procno, newMagazine.MagazineNo);
						OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
					}

                    //Lファイルを一番最後にリネーム 2019.01.21
                    MachineLog.ChangeFileName(lFile, lot.NascaLotNo, lot.TypeCd, procno, newMagazine.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lFile));
                }
                else 
				{
					base.WorkComplete(newMagazine, this, true);
				}

				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", newMagazine.MagazineNo));
			}
        }

        private void workCompletehigh()
        {
            VirtualMag unloadermag = this.Peek(Station.Unloader);
            if (unloadermag != null)
            {
                //完了処理が終了しているので何もしない
                return;
            }

			VirtualMag newMagazine = this.Peek(Station.EmptyMagazineLoader);

			if (newMagazine == null)
            {
                return;
            }

            //フレーム関連付け枚数が搭載数に満たない場合は待機
			if (newMagazine.CurrentFrameCt.Value < newMagazine.MaxFrameCt.Value)
            {
                return;
            }

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", newMagazine.MagazineNo));

            //前マガジンの取り除きフラグが残っている場合は削除
            Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);

			newMagazine.LastMagazineNo = newMagazine.MagazineNo;
			newMagazine.WorkComplete = DateTime.Now;

            //終了時ウェハー段数を取得
			newMagazine.EndWafer = this.GetWaferPlateNo();
			if (newMagazine.StartWafer == null)
            {
                throw new Exception("開始ウェハー段数が取得できていません。");
            }
			Log.ApiLog.Info(string.Format("終了ウェハー段数を設定:{0}", newMagazine.EndWafer.ToString()));

			Magazine svrMag = Magazine.GetCurrent(newMagazine.MagazineNo);
			AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));


            bool firstProcessFg = false;
            Process p = Process.GetProcess(newMagazine.ProcNo.Value);
            if (p.ProcNo == Process.GetFirstProcess(lot.TypeCd).ProcNo)
            {
                firstProcessFg = true;
            }

            if (this.LogOutputDirectoryPath != null)
			{
				string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
				if (string.IsNullOrEmpty(lFile))
					throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

				newMagazine.WorkComplete = File.GetLastWriteTime(lFile);

                // 設定ファイルにサーバー統合の識別が必要か？
                if (firstProcessFg == true)
                {
                    // ここで新規ロットNoの取得と新規ロットの作業実績作成
                    ChangeNewAsmLot(newMagazine);

                    //※下記のロットファイル名称変更処理は完了登録後が必須
                    //完了登録時にロット番号をマガジン番号からロット番号へ変更しているので、後でないとマガジン番号をロット番号として付与してしまうため
                    svrMag = Magazine.GetCurrent(newMagazine.MagazineNo);
                    lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));
                }

                List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, newMagazine.WorkStart.Value, File.GetLastWriteTime(lFile));
				if (lotFiles.Count == 0)
					throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", newMagazine.WorkStart.Value, newMagazine.WorkComplete.Value));

				foreach (string lotFile in lotFiles)
				{
					if (IsStartLogFile(lotFile))
						continue;

                    //EICSでリネーム前のファイルを移動させようとしてエラーになる現象がMAPでちょこちょこ発生しているので
                    //その対策としてLファイルを一番最後にリネームするよう改修する 
                    //EICSではLファイルをリネームしていないと全ファイル移動しないような仕組みになっているので
                    //Lファイルを最後にリネームすることで問題が解決する見込み 2019.01.21
                    if (lotFile.Trim() == lFile.Trim())
                        continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, newMagazine.ProcNo.Value, newMagazine.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}

                //Lファイルを一番最後にリネーム 2019.01.21
                MachineLog.ChangeFileName(lFile, lot.NascaLotNo, lot.TypeCd, newMagazine.ProcNo.Value, newMagazine.MagazineNo);
                OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lFile));
            }
            else
            {
                if (firstProcessFg == true)
                {
                    // ここで新規ロットNoの取得と新規ロットの作業実績作成
                    ChangeNewAsmLot(newMagazine);
                }                        
            }

			if (this.Enqueue(newMagazine, Station.Unloader))
			{
				this.Dequeue(Station.EmptyMagazineLoader);
				IsWaitingMagazineTakeout = true;
				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", newMagazine.MagazineNo));
			}

        }

        #endregion

        #region readBadMarkData

        /// <summary>
        /// バッドマーク不良データを読み込んで不良登録実施
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        private bool readBadMarkData(VirtualMag mag)
        {
            string[] files = System.IO.Directory.GetFiles(DefectFileBasePath, DEFECT_FILE_HEADER + "*");

            string path = null;
            //最新の更新日付のデータのみ対象
            DateTime lastWrite = DateTime.MinValue;
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (lastWrite >= fi.LastWriteTime)
                {
                    continue;
                }
                path = file;
                lastWrite = fi.LastWriteTime;
            }

            if (string.IsNullOrEmpty(path) == true)
            {
                //バッドマーク不良データ待ち
                return false;
            }

            int ct = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line)) ct++;
                }
            }

            List<DefItem> defItems = new List<DefItem>();
            defItems.Add(new DefItem(mag.ProcNo.Value, BadMarkClassCd, BadMarkCauseCd, BadMarkDefetCd, ct));

            Magazine svrMagazine = Magazine.GetCurrent(mag.MagazineNo);

            Defect defect = new Defect(svrMagazine.NascaLotNO, mag.ProcNo.Value, defItems);
            defect.DeleteInsert();

            try
            {
                //完了ファイルの移動
				if (Directory.Exists(Config.Settings.NascaDefectFileDonePath) == false)
                {
					Directory.CreateDirectory(Config.Settings.NascaDefectFileDonePath);
                }

                foreach (string file in files)
                {
					string destPath = Path.Combine(Config.Settings.NascaDefectFileDonePath, Path.GetFileName(file));
                    if (File.Exists(destPath) == true)
                    {
                        File.Delete(destPath);
                    }
                    File.Move(file, destPath);
                }
            }
            catch
            {
                Log.RBLog.Error("外観検査データの完了フォルダへの移動時にエラー発生。\nNASCA登録は正常完了しているため処理続行。\n" + path);
                //ファイル移動に関するエラーは握りつぶす
            }
            return true;
        }

        #endregion

        #region CheckWorkStart アンローダー側マガジンに記録

        /// <summary>
        /// アンローダーの先頭マガジンに作業開始時間とウェハー段数を記録
        /// </summary>
        private void checkWorkStart()
        {
            VirtualMag mag = this.Peek(Station.EmptyMagazineLoader);
            if (mag == null)
            {
                return;
            }

            if (mag.StartWafer.HasValue == true)
            {
                //すでに開始段数が有る場合は何もしない
                return;
            }

            //開始時間を記録
            mag.WorkStart = DateTime.Now;

            //開始時のウェハー段数を記録
            mag.StartWafer = this.GetWaferPlateNo();

            //ウェハーチェンジャーの交換回数を0回に設定
            mag.WaferChangerChangeCount = 0;

            //最大基板積載数と現在数の設定
            mag.MaxFrameCt = Magazine.GetFrameCt(mag.MagazineNo);// mag.MaxFrameCtとなっているが、湯浅氏の仕様により取得するGetFrameCt()はLoadStepCDにより計算した実搭載枚数(2014/1/9 n.yoshimoto)
            mag.CurrentFrameCt = 0;

            //実ファイル更新
            mag.Updatequeue();

            //TnTranの開始時間も更新する(ローダー側にあるマガジン全て更新)
            Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
            Order startOrder = Order.GetMachineOrder(this.MacNo, svrMag.NascaLotNO);
            if (startOrder == null)
            {
                throw new ApplicationException(string.Format("作業の開始実績が存在しませんでしたので、アンローダーの先頭マガジンの作業開始時間を更新できませんでした。LotNo:{0}", svrMag.NascaLotNO));
            }
            DateTime workStart = mag.WorkStart.Value;
            List<Order> macOrders = Order.GetMachineOrderStart(this.MacNo, startOrder.WorkStartDt, workStart).Where(r => r.WorkEndDt.HasValue == false).OrderBy(r => r.WorkStartDt).ToList();
            foreach (Order macOrder in macOrders)
            {
                OutputSysLog($"[開始時間記録] Loader側マガジンの開始時刻を更新(TnTran) 更新対象ロット：{macOrder.LotNo} 更新対象工程：{macOrder.ProcNo} 更新前の開始時間：{macOrder.WorkStartDt} 更新後の開始時間：{workStart}");

                macOrder.WorkStartDt = workStart;
                macOrder.DeleteInsert(macOrder.LotNo);

                workStart = workStart.AddSeconds(1);
            }

            this.Plc.SetBit(StartWorkTableSensorAddress, 1, Mitsubishi.BIT_OFF);
            return;
        }

        private void checkWorkStartLoader() 
        {
            VirtualMag mag = this.Peek(Station.Loader);
            if (mag == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(mag.MapAoiMagazineLotNo)) 
            {
                return;
            }

            //品名,,ロット,,数量,GGコード,,,
            string rawstring = mag.MagazineNo;

            string[] values = rawstring.Split(VirtualMag.MAP_FRAME_SEPERATOR);
            if (values.Length != VirtualMag.MAP_FRAME_ELEMENT_CT)
            {
                throw new ApplicationException(string.Format("MAP基板マガジンのマガジン番号が不正です。 マガジン位置:{0}", Station.Loader.ToString()));
            }

            mag.MapAoiMagazineLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
            mag.MaxFrameCt = int.Parse(values[VirtualMag.MAP_FRAME_QR_CT_IDX]);
            mag.CurrentFrameCt = mag.MaxFrameCt;
            mag.FrameMatCd = values[VirtualMag.MAP_FRAME_QR_MATCD_IDX];
            mag.FrameLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
            mag.MagazineNo = mag.MapAoiMagazineLotNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        #endregion

        #region Enqueue
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //MAP基板マガジンの場合はQR読込内容を分解する
            if (station == Station.Loader)
            {
                //品名,,ロット,,数量,GGコード,,,
                string rawstring = mag.MagazineNo;

                string[] values = rawstring.Split(VirtualMag.MAP_FRAME_SEPERATOR);
                if (values.Length != VirtualMag.MAP_FRAME_ELEMENT_CT)
                {
                    throw new ApplicationException(string.Format("MAP基板QRコード内容が不正です。 QRコード内容:{0}", mag.MagazineNo));
                }

                mag.MapAoiMagazineLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
                mag.MaxFrameCt = int.Parse(values[VirtualMag.MAP_FRAME_QR_CT_IDX]);
                mag.CurrentFrameCt = mag.MaxFrameCt;
                mag.FrameMatCd = values[VirtualMag.MAP_FRAME_QR_MATCD_IDX];
                mag.FrameLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
                mag.MagazineNo = mag.MapAoiMagazineLotNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            if (station == Station.EmptyMagazineLoader)
            {
                //開始時間を記録
                mag.WorkStart = DateTime.Now;

                //ウェハーチェンジャーの交換回数を0回に設定
                mag.WaferChangerChangeCount = 0;

                //最大基板積載数と現在数の設定
                mag.MaxFrameCt = Magazine.GetFrameCt(mag.MagazineNo);// mag.MaxFrameCtとなっているが、湯浅氏の仕様により取得するGetFrameCt()はLoadStepCDにより計算した実搭載枚数(2014/1/9 n.yoshimoto)
                mag.CurrentFrameCt = 0;
            }
            
            return base.Enqueue(mag, station);
        }
        #endregion

        #region ResponseEmptyMagazineRequest

        /// <summary>
        /// ローダー側　From：MAP基板投入CV　To：MAP基板排出CV
        /// アンローダー側　From:空マガジン投入CV　To:別関数
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="keeper"></param>
        /// <returns></returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            //新規基板投入処理
            //投入禁止の場合は基板も運ばない
            if (this.IsInputForbidden() == false)
            {
                //アンローダー側空マガジン（日亜マガジン）
                if (this.IsRequireEmptyMagazine() == true)
                {
                    //2011.10.06追加仕様
                    //既にアンローダー側に基板がある場合、投入から4hr以上経過していなければ次を運ばない
                    bool skip = false;
                    VirtualMag ulempMag = Peek(Station.EmptyMagazineLoader);
                    if (ulempMag != null)
                    {
                        double totalminutes = 0;
                        if (ulempMag.WorkStart.HasValue)
                        {
                            totalminutes = (DateTime.Now - ulempMag.WorkStart.Value).TotalMinutes;
                        }

                        GnlMaster[] master = GnlMaster.Search("DB_EMPMAG_INTERVAL", null, null, null);
                        double mininterval = 0;
                        if (master.Length >= 1)
                        {
                            double.TryParse(master[0].Val, out mininterval);
                        }

                        if (totalminutes <= mininterval)
                        {
                            skip = true;
                        }
                    }

                    // インターロック機能 (紐付けプロファイルのロット数の上限チェック)
                    if (skip == false)
                    {
                        // ここでは、関数の引数はout後、使用しないため、変数名は適当な名前にしてます。
                        int i, j;
                        if (this.IsLimitOverProfileLotCount(out i, out j))
                        {
                            skip = true;
                        }
                    }

                    //ライン連結橋の空マガジンを使用
                    Location from = null;
                    LineBridge bridge = LineKeeper.GetReachBridge(this.MacNo);
                    if (skip == false && bridge != null && bridge.IsRequireOutputEmptyMagazine())
                    {
                        //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                        if (VirtualMag.IsECKMag(mag.MagazineNo)) return false;
                        if (mag.IsAOIMag()) return false;

                        from = bridge.GetUnLoaderLocation();
                        Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);

                        //移動元からDequeueが必要
                        LineKeeper.MoveFromTo(from, to, true, true, true);
                        return true;
                    }
                    else
                    {
                        //空マガジン投入CVの状態確認
                        IMachine empMagLoadConveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                        if (skip == false && empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                        {
                            CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                            CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
                            if (fromCar.CarNo != toCar.CarNo)
                            {
                                //空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする                              
                                List<VirtualMag> bridgeMags = new List<VirtualMag>();

                                List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                                foreach (IMachine b in bridges)
                                {
                                    //橋内のすべての仮想マガジンを取得
                                    bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                                }
                                if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null &&
                                                        VirtualMag.IsECKMag(m.MagazineNo) == false &&
                                                        m.IsAOIMag() == false))
                                {
                                    return false;
                                }
                            }

                            from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
                            Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
                            LineKeeper.MoveFromTo(from, to, false, true, true);
                            return true;
                        }
                        else
                        {
                            //空マガジン投入CVにマガジンが無い場合
                            return false;
                        }
                    }
                }

                //ローダー側　アオイマガジン供給
                if (this.IsRequireInput() == true)
                {
                    // 紐付け中のDBプロファイルを確認
                    //DBプロファイルと一致判定追加　2012.01.15
                    Profile prof = Profile.GetCurrentDBProfile(this.MacNo);
                    if (prof != null)
                    {
                        BOM[] boms = Profile.GetBOM(prof.ProfileId);

                        // MAP基板マガジン投入コンベアの状態確認
                        IMachine mapLoadConveyor = LineKeeper.GetMachine(Route.GetAoiLoadConveyor(this.MacNo));

                        CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                        CarrierInfo toCar = Route.GetReachable(new Location(mapLoadConveyor.MacNo, Station.Loader));
                        if (fromCar.CarNo != toCar.CarNo)
                        {
                            //空マガジン投入CVが自ラインでは無い場合、橋に稼働フラグ無しマガジン（アオイor空）が無いか確認し、有れば搬送しないようにする
                            List<VirtualMag> bridgeMags = new List<VirtualMag>();

                            List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                            foreach (IMachine b in bridges)
                            {
                                //橋内のすべての仮想マガジンを取得
                                bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                            }
                            if (bridgeMags.Count != 0)
                            {
                                if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
                                {
                                    return false;
                                }
                            }
                        }

                        if (mapLoadConveyor.IsRequireOutput() == true)
                        {
                            //アオイマガジン拾い上げ（実際の移動先はRobotが再度変更する可能性がある）
                            Location from = new Location(mapLoadConveyor.MacNo, Station.Unloader);
                            VirtualMag ulMag = mapLoadConveyor.Peek(from.Station);
                            //if (mapLoadConveyor.Peek(from.Station) != null)
                            if (ulMag != null && string.IsNullOrWhiteSpace(ulMag.FrameMatCd) == false)
                            {
                                // BOMに含まれているフレーム型番の場合は、搬送指示を出す
                                if (!Array.Exists(boms, b => b.MaterialCd == ulMag.FrameMatCd))
                                {
                                    Location to = this.GetLoaderLocation();
                                    LineKeeper.MoveFromTo(from, to, false, true, false);
                                    return true;
                                }
                            }
                        }

                        if (mapLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                        {
                            //アオイマガジン拾い上げ（実際の移動先はRobotが再度変更する可能性がある）
                            Location from = new Location(mapLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
                            VirtualMag ulMag = mapLoadConveyor.Peek(from.Station);
                            //if (mapLoadConveyor.Peek(from.Station) != null)
                            if (ulMag != null && string.IsNullOrWhiteSpace(ulMag.FrameMatCd) == false)
                            {
                                // BOMに含まれているフレーム型番の場合は、搬送指示を出す
                                if (!Array.Exists(boms, b => b.MaterialCd == ulMag.FrameMatCd))
                                {
                                    Location to = this.GetLoaderLocation();
                                    LineKeeper.MoveFromTo(from, to, false, true, false);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            //空のMAP基板マガジン排出処理
            if (this.IsRequireOutputEmptyMagazine() == true)
            {
				FrameTransfer();

                VirtualMag ulMagazine = Peek(Station.EmptyMagazineUnloader);
                if (ulMagazine == null)
				{
					VirtualMag lMagazine = Peek(Station.Loader);
					if (lMagazine != null && lMagazine.CurrentFrameCt == 0)
					{
						this.Enqueue(lMagazine, Station.EmptyMagazineUnloader);
						this.Dequeue(Station.Loader);
					}
				}

                VirtualMag emptyMag = Peek(Station.EmptyMagazineUnloader);
                if (emptyMag == null)
                {
                    //仮想マガジンが無い場合は処理しない
                    return false;
                }

                // アオイマガジン排出CVの状態確認
                IMachine mapDischargeConveyor = LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(this.MacNo));
                if (mapDischargeConveyor.IsRequireInput() == true)
                {
                    CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                    CarrierInfo toCar = Route.GetReachable(new Location(mapDischargeConveyor.MacNo, Station.Loader));
                    if (fromCar.CarNo != toCar.CarNo)
                    {
                        //アオイマガジン排出CVが自ラインでは無い場合、橋に稼働フラグ無しマガジンが無いか確認し、有れば搬送しないようにする
                        List<VirtualMag> bridgeMags = new List<VirtualMag>();

                        List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                        foreach (IMachine b in bridges)
                        {
                            //橋内のすべての仮想マガジンを取得
                            bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                        }
                        if (bridgeMags.Count != 0)
                        {
                            if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
                            {
                                return false;
                            }
                        }
                    }

                    Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    Location to = new Location(mapDischargeConveyor.MacNo, Station.Loader);

                    LineKeeper.MoveFromTo(from, to, true, true, false, false);
                    return true;
                }
                else
                {
                    //空マガジンをどこにも置けない場合
                    return false;
                }
            }

            return false;
        }
        #endregion

        private void ChangeNewAsmLot(VirtualMag mag)
        {
            Order order = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
            ArmsApiResponse workResponse = CommonApi.ChangeNewAsmLot(order);
            if (workResponse.IsError)
            {
                OutputSysLog(string.Format("新規ロット振り替え処理失敗：マガジンNo= {0},  理由= {1}", mag.MagazineNo, workResponse.Message));
            }
        }

        /// <summary>
        /// 装置に紐付いているプロファイルのロット数を超えるかどうか
        /// </summary>
        public bool IsLimitOverProfileLotCount(out int inputlotct, out int orderlotct)
        {
            inputlotct = 0;
            orderlotct = 0;

            Profile prof = Profile.GetCurrentDBProfile(this.MacNo);
            if (prof == null) return false;

            try
            {
                orderlotct = Profile.GetOrderCountFromProfileName(prof.ProfileNm);
            }
            catch (Exception)
            {
                // プロファイル名にロット数が記載されていない場合、
                // Profile.GetOrderCountFromProfileName関数でExceptionを飛ばす仕様になっている。
                // 先行色調品用のプロファイルなどは、プロファイル名にロット数が記載されていないことがあるので、その場合はチェックをしない
                return false;
            }

            // 紐付けプロファイルの投入済みロット数を取得する

            Process firstProc = Process.GetWorkFlow(prof.TypeCd).FirstOrDefault();
            if (firstProc == null) return false;

            AsmLot[] lots = AsmLot.SearchAsmLot(prof.ProfileId);
            Order[] orderArray = Order.GetOrder(lots.Select(x => x.NascaLotNo).Distinct(), firstProc.ProcNo);

            var orders = orderArray.Where(l => l.NascaLotNo != l.InMagazineNo || l.WorkEndDt.HasValue == false);
            inputlotct = orders.Count();

            // 投入済みロット数 = 上限未満のチェック
            if (inputlotct < orderlotct) return false;

            return true;
        }
    }
}
