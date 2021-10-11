using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ロットマーキング機　フレームタイプ(車載自/高モデル) 
    /// </summary>
    public class LotMarking : MachineBase
    {
		/// <summary>
		/// 開始時マガジンNo取得アドレス
		/// </summary>
		public string LMagazineAddress { get; set; }

        /// <summary>
        /// 終了時排出マガジンNo取得アドレス
        /// </summary>
        public string ULMagazineAddress { get; set; }

		/// <summary>
		/// ローダー側マガジンNo読取完了アドレス
		/// </summary>
		public string LoaderQRReadCompleteBitAddress { get; set; }

		/// <summary>
		/// 作業開始登録を本体側でするフラグ
		/// </summary>
		public bool IsWorkStartAutoComplete { get; set; }

		/// <summary>
		/// EICSとのハンドシェイクに使用するフォルダパス
		/// </summary>
		public string HandshakeDirPath { get; set; }

		#region 高効率用

		/// <summary>
		/// 直近100ロットの完成履歴
		/// </summary>
		public Queue<string> preCompleteLotQueue = new Queue<string>();

		public List<UnloaderMagazine> UnloaderMagazineList { get; set; }

		public class UnloaderMagazine 
		{
			public string ReqBitAddress { get; set; }
			public string MagazineNoAddress { get; set; }
			public string WorkStartAddress { get; set; }
			public string WorkEndAddress { get; set; }
		}

		/// <summary>
		/// 通信する中間PCのIPアドレス
		/// </summary>
		public string HostIpAddress { get; set; }

		/// <summary>
		/// 開始登録OKBit
		/// </summary>
		public string WorkStartOKBitAddress { get; set; }

		/// <summary>
		/// 開始登録NGBit
		/// </summary>
		public string WorkStartNGBitAddress { get; set; }

		/// <summary>
		/// 完了登録OKBit
		/// </summary>
		public string WorkCompleteOKBitAddress { get; set; }

		/// <summary>
		/// 完了登録NGBit
		/// </summary>
		public string WorkCompleteNGBitAddress { get; set; }

		#endregion

		public LotMarking() 
		{
			UnloaderMagazineList = new List<UnloaderMagazine>();
		}

		protected override void concreteThreadWork()
        {
			if (this.IsAutoLine)
			{
				if (this.IsRequireOutput() == true)
				{
					WorkComplete();
				}
			}
			else 
			{
				if (this.IsRequireOutput() == true)
				{
					//※OmlonPLCで排出信号後、日付データ等のデータ欠けが発生した為、待機する
					Thread.Sleep(1000);

					WorkCompleteHigh();
				}
				else 
				{
					Plc.SetBit(WorkCompleteOKBitAddress, 1, PLC.Common.BIT_OFF);
				}
			}

			//2015.11.27 作業自動開始登録機能　SLS2-83ラインで検証中
			if (this.IsWorkStartAutoComplete)
			{
				if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
				{
					WorkStart();
				}
			}

            //EICS側でハンドシェイクを無効にしたため不要。削除。2016.6.10
            //else
            //{
            //    //2016.5.24 作業自動登録対象外の装置でもEICSファイルのリネームだけはするように改修。
            //    //※EICSが自動登録しない装置を考慮できていない(ハンドシェイク対象にしてしまう)ため、
            //    //その不具合対応。
            //    if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
            //    {
            //        RenameEicsHandShakeFile();
            //    }
            //}

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        public virtual void WorkComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("ロットマーキング排出マガジンNOの取得に失敗");
                return;
            }

            string oldmagno = Plc.GetMagazineNo(LMagazineAddress);
            if (string.IsNullOrEmpty(oldmagno) == false)
            {
                newMagazine.LastMagazineNo = oldmagno;
            }
            else
            {
                Log.RBLog.Info("ロットマーキング搬入側マガジンNOの取得に失敗");
                return;
            }

            //作業開始完了時間取得
            try
            {
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("ロットマーキング機 作業開始時間取得失敗:{0}", this.MacNo));
            }
            try
            {
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("ロットマーキング機 作業完了時間取得失敗:{0}" + this.MacNo));
            }

            //作業IDを取得
            newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, oldmagno);

			VirtualMag oldmag = new VirtualMag();
			oldmag.MagazineNo = oldmagno;
			oldmag.LastMagazineNo = oldmagno;
			this.Enqueue(oldmag, Station.EmptyMagazineUnloader);

            this.Enqueue(newMagazine, Station.Unloader);

            this.WorkComplete(newMagazine, this, true);
        }

		/// <summary>
		/// 作業完了(高効率)
		/// </summary>
		public virtual void WorkCompleteHigh() 
		{
			foreach(UnloaderMagazine ulMagazine in UnloaderMagazineList)
			{
				if (Plc.GetBit(ulMagazine.ReqBitAddress) == PLC.Omron.BIT_ON) 
				{
					string magno = Plc.GetMagazineNo(ulMagazine.MagazineNoAddress);
					if (string.IsNullOrEmpty(magno) == true)
					{
						Log.ApiLog.Info(string.Format("マガジン番号取得失敗 読込アドレス:{0}", ulMagazine.MagazineNoAddress));
						return;
					}

					DateTime startDt = Plc.GetWordsAsDateTime(ulMagazine.WorkStartAddress);
					DateTime endDt = Plc.GetWordsAsDateTime(ulMagazine.WorkEndAddress);

					VirtualMag mag = getMagazine(magno, startDt, endDt);
					if (mag != null)
					{
						completeEnqueue(mag);
					}
				}
			}

			Plc.SetBit(WorkCompleteOKBitAddress, 1, PLC.Common.BIT_ON);
		}

		private VirtualMag getMagazine(string magazineNo, DateTime startDt, DateTime endDt) 
		{
			VirtualMag mag = new VirtualMag();

			//分割なしマガジン番号を返す
			mag.MagazineNo = magazineNo;
			mag.LastMagazineNo = mag.MagazineNo;
			mag.WorkStart = startDt;
			mag.WorkComplete = endDt;

			Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
			if (svrmag == null)
			{
				//分割マガジンも検索
				Order ord = Order.SearchOrder(mag.MagazineNo, null, this.MacNo, true, false).OrderBy(m => m.DevidedMagazineSeqNo).FirstOrDefault();

				if (ord != null)
				{
					svrmag = Magazine.GetCurrent(ord.LotNo);
				}

				if (svrmag == null)
				{
					//分割状態か判定して、分割の完了履歴も存在しないなら異常
					ord = Order.SearchOrder(mag.MagazineNo, null, this.MacNo, false, false).OrderBy(m => m.DevidedMagazineSeqNo).FirstOrDefault();
					if (ord == null)
					{
						throw new ApplicationException("マガジン情報が見つかりません" + mag.MagazineNo);
					}
					else
					{
						return null;
					}
				}
				else
				{
					mag.MagazineNo = svrmag.MagazineNo;
					mag.LastMagazineNo = svrmag.MagazineNo;
				}
			}

			AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

			return mag;
		}

		private void completeEnqueue(VirtualMag mag) 
		{
			//作業IDを取得
			int procno = Order.GetLastProcNo(this.MacNo, mag.MagazineNo);
            mag.ProcNo = procno;          
			
			//直近100ロットの完成リストに存在する場合は何もしない
			if (preCompleteLotQueue.Contains(string.Format("{0},{1}", mag.MagazineNo, procno)) == false)
			{
				//Enqueueが先にないとLoc情報が無い
				this.Enqueue(mag, Station.Unloader);

				//高効率でArmsWebが作成してしまうので削除
				this.Dequeue(Station.Loader);

				//直近100マガジンの完成記憶
				preCompleteLotQueue.Enqueue(string.Format("{0},{1}", mag.MagazineNo, procno));
				if (preCompleteLotQueue.Count >= 100)
				{
					preCompleteLotQueue.Dequeue();
				}
			}
		}

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //アンローダー以外は何もしない
            if (station != Station.Unloader && station != Station.EmptyMagazineUnloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

		#region ResponseEmptyMagazineRequest
		/// <summary>
		/// 空マガジンの配置処理
		/// </summary>
		/// <param name="m"></param>
		public override bool ResponseEmptyMagazineRequest()
		{
			//供給禁止状態なら処理しない
			if (this.IsInputForbidden() == true)
			{
				return false;
			}

			if (this.IsRequireEmptyMagazine() == true)
			{
				bool IsDeleteFromMag = false;
				Location from = null;

				LineBridge bridge = LineKeeper.GetReachBridge(this.MacNo);

				//自装置の空マガジンを使用
				if (this.IsRequireOutputEmptyMagazine() == true)
				{
					from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
					IsDeleteFromMag = true;
				}
				//ライン連結橋の空マガジンを使用
				else if (bridge != null && bridge.IsRequireOutputEmptyMagazine())
				{
                    //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                    VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
					if (VirtualMag.IsECKMag(mag.MagazineNo)) return false;
                    if (mag.IsAOIMag()) return false;

                    from = bridge.GetUnLoaderLocation();
					IsDeleteFromMag = true;
				}
				//空マガジン投入CVの空マガジンを使用
				else
				{
					if (base.IsRequireOutputEmptyMagazine())
					{
						//自装置の空マガジン待ち(完了処理待ち)
						return false;
					}

					//空マガジン投入CVの状態確認
					IMachine empMagLoadConveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
					if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
					{
						CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
						CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
						if (fromCar.CarNo != toCar.CarNo)
						{
							//空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
							List<VirtualMag> bridgeMags = new List<VirtualMag>();

							List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge).ToList();
							foreach (LineBridge b in bridges)
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
						IsDeleteFromMag = false;
					}
					else
					{
						//空マガジン投入CVにマガジンが無い場合
						return false;
					}
				}

				Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
				LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);
	
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		/// <summary>
		/// 空マガジン排出要求
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireOutputEmptyMagazine()
		{
			//空マガジン排出信号ON、仮想マガジン有り(EmptyMagazineUnloader)
			if (!base.IsRequireOutputEmptyMagazine())
			{
				return false;
			}

			if (this.IsAutoLine)
			{
				VirtualMag mag = Peek(Station.EmptyMagazineUnloader);
				if (mag == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}

		public void WorkStart()
		{
            //ARMSファイルがフォルダに既に存在する場合は処理中断。
            //EICS処理待ちの際に再度開始処理してしまうことがあるのでその対策。2016.5.25
            string armsEndTrigFilePath = MachineLog.GetNewestFile(HandshakeDirPath, "ARMSEND");
            if (string.IsNullOrEmpty(armsEndTrigFilePath) == false)
            {
                OutputSysLog(string.Format("ARMSENDファイルが既に存在するため開始処理スキップ:{0}", armsEndTrigFilePath));
                return;
            }

			VirtualMag mag = new VirtualMag();

			string magno = Plc.GetMagazineNo(LMagazineAddress);
			if (string.IsNullOrEmpty(magno) == false)
			{
				mag.MagazineNo = magno;
				mag.LastMagazineNo = magno;
			}
			else
			{
				throw new ApplicationException("[開始登録異常] 搬入マガジンNOの取得に失敗。\n搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
			}

			OutputSysLog(string.Format("[開始処理] 開始 LoaderMagazineNo:{0}", magno));

			Magazine svrmag = Magazine.GetCurrent(magno);
			if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

			AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

			mag.ProcNo = nextproc.ProcNo;
			//mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
			mag.WorkStart = DateTime.Now;

			Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

			ArmsApiResponse workResponse = CommonApi.WorkStart(order);
			if (workResponse.IsError)
			{
				Plc.SetBit(this.WorkStartNGBitAddress, 1, PLC.Common.BIT_ON);
				throw new ApplicationException(
					string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
			}
			else
			{
				if (string.IsNullOrWhiteSpace(WorkStartOKBitAddress))
				{
					// ﾌﾚｰﾑﾀｲﾌﾟを想定
					// EICSが品種No, 印字文字の送信が完了した時にONする信号をQR読取信号の代わりにする為、
					// 作業開始登録が終わればOFFにする
					//Plc.SetBit(this.LoaderQRReadCompleteBitAddress, 1, PLC.Common.BIT_OFF);

                    //タイムアウト対策にリトライ回数を10回に増加。2016.5.25
					string eicsEndTrigFilePath = GetEicsEndTrigFilePath(10, 1000);

					if (string.IsNullOrEmpty(eicsEndTrigFilePath) == false)
					{
                        //誤信号を受け取っている可能性があるのでファイルリトライ中に要求信号が落ちたらログだけ残して
                        //処理を中断する。
                        if (eicsEndTrigFilePath == "BIT_OFF")
                        {
                            OutputSysLog(string.Format("EICSファイル確認中に要求ビットが落ちたため処理中断：{0}", magno));
                            return;
                        }

                        string fileNm = Path.GetFileNameWithoutExtension(eicsEndTrigFilePath);

                        OutputArmsEndTrigFile(fileNm.Replace("EICSEND", string.Empty));
					}

				}
				else
				{
					// ﾁｯﾌﾟﾀｲﾌﾟを想定
					Plc.SetBit(this.WorkStartOKBitAddress, 1, PLC.Common.BIT_ON);
				}
				OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", magno));
			}
		}

		public void OutputArmsEndTrigFile(string dateStr)
		{
			string fileName = string.Format("ARMSEND{0}.CSV", dateStr);
			StreamWriter sw = new StreamWriter(Path.Combine(HandshakeDirPath, fileName), true, Encoding.UTF8);

			sw.Close();
		}

		public string GetEicsEndTrigFilePath(int retryCt, int retryIntervalMilliSec)
		{
			string eicsEndTrigFilePath = MachineLog.GetNewestFile(HandshakeDirPath, "EICSEND");

            if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_OFF)
            {
                return "BIT_OFF";
            }

            int tryCt = 0;
			while (eicsEndTrigFilePath == null)
			{
				tryCt++;
				eicsEndTrigFilePath = MachineLog.GetNewestFile(HandshakeDirPath, "EICSEND");

				if (tryCt >= retryCt)
				{
					throw new ApplicationException(
					string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, "EICSﾄﾘｶﾞﾌｧｲﾙが見つかりませんでした。EICSが停止していないか確認してください。"));
				}
				else
				{
                    //誤信号が入っている可能性があるのでファイルチェックのRetry毎にビットを確認する。
                    if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_OFF)
                    {
                        return "BIT_OFF";
                    }
                    Thread.Sleep(retryIntervalMilliSec);
                }
			}

			return eicsEndTrigFilePath;
		}

        //2016.5.24 EICSのファイルを見つけてリネームするだけの処理
        //public void RenameEicsHandShakeFile()
        //{

        //    string eicsEndTrigFilePath = GetEicsEndTrigFilePath(5, 1000);

        //    if (string.IsNullOrEmpty(eicsEndTrigFilePath) == false)
        //    {
        //        string fileNm = Path.GetFileNameWithoutExtension(eicsEndTrigFilePath);

        //        OutputArmsEndTrigFile(fileNm.Replace("EICSEND", string.Empty));
        //    }
        //}

    }
}
