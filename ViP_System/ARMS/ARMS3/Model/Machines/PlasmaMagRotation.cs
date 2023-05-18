using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using System.Threading;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// プラズマ(PE製)
    /// </summary>
    public class PlasmaMagRotation : Plasma
    {
        /// <summary>
        /// 開始時マガジンNo取得アドレス
        /// </summary>
        public string LMagazineAddress { get; set; }

        /// <summary>
        /// ローダー側マガジンNo読取完了アドレス(※この装置は装置がONしてくるので、開始登録が正常に終わった後はOFFにする)
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }
        
        /// <summary>
        /// 投入ﾏｶﾞｼﾞﾝ判定結果（OK=1, NG=2）= 開始登録処理の合否結果
        /// </summary>
        public string WorkStartOKWordAddress { get; set; }

        /// <summary>
        /// 投入ﾏｶﾞｼﾞﾝ空実判定（実=1, 空=2） = 開始処理した読み込んだマガジンが実マガジン or 空マガジンかを貸せる。
        /// </summary>
        public string lMagazineMnfctWordAddress { get; set; }

        public int QRMagazineNoLength { get; set; }

        /// <summary>
        /// マガジン番号WORDアドレスの長さ(自動化)
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH_AUTO = 6;

        /// <summary>
        /// マガジン番号WORDアドレスの長さ(高効率)
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH_HIGH = 10;

		/// <summary>
        /// 完了登録OKBit   （OK=1, NG=2）
		/// </summary>
		public string WorkCompleteOKBitAddress { get; set; }

		/// <summary>
		/// 完了登録NGBit
		/// </summary>
        //public string WorkCompleteNGBitAddress { get; set; }

        public PlasmaMagRotation()
            : base()
        {
            this.ProcForbiddenAddressList = new SortedList<int, string>();
            this.ProcDischargeAddressList = new SortedList<int, string>();
        }

		//#region 高効率用

		///// <summary>
		///// 直近100ロットの完成履歴
		///// </summary>
		//public Queue<string> preCompleteLotQueue = new Queue<string>();

		//public List<UnloaderMagazine> UnloaderMagazineList { get; set; }

		//public class UnloaderMagazine 
		//{
		//	public string ReqBitAddress { get; set; }
		//	public string MagazineNoAddress { get; set; }
		//	public string WorkStartAddress { get; set; }
		//	public string WorkEndAddress { get; set; }
		//}

		///// <summary>
		///// 通信する中間PCのIPアドレス
		///// </summary>
		//public string HostIpAddress { get; set; }

		//#endregion

        protected override void concreteThreadWork()
        {
            try
            {
                if (this.IsRequireOutput() == true)
                {
                    if (this.IsAutoLine)
                    {
                        workComplete();
                    }
                    else
                    {
                        workCompleteHigh();
                    }
                }

                if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
                {
                    workStart();
                }

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();
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
        /// 作業完了
        /// </summary>
        private void workComplete() 
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
                Log.RBLog.Info("プラズマ機排出マガジンNOの取得に失敗");
                return;
            }

            string oldmagno = Plc.GetMagazineNo(LMagazineAddress);
            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.LastMagazineNo = oldmagno;
            }
            else
            {
                Log.RBLog.Info("プラズマ機供給マガジンNOの取得に失敗");
                return;
            }

            //作業開始完了時間取得
            try
            {
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("プラズマ機 作業開始時間取得失敗:{0}", this.MacNo));
            }

            try
            {
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("プラズマ機 作業完了時間取得失敗:{0}", this.MacNo));
            }

            //作業IDを取得
            newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, oldmagno);

            this.Enqueue(newMagazine, Station.Unloader);

            this.WorkComplete(newMagazine, this, true);
        }

        /// <summary>
        /// 作業完了(高生産性)
        /// </summary>
        private void workCompleteHigh()
		{
			try
			{
				if (this.IsAutoLine) { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_AUTO; }
				else { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_HIGH; }

				VirtualMag ulMagazine = this.Peek(Station.Unloader);
				if (ulMagazine != null)
				{
                    throw new ApplicationException(this.Name + "の" + Station.Unloader.ToString() + "に仮想マガジンが残っています。Armsメンテナンスの画面で全て削除して下さい。");
                    //return;
				}

				VirtualMag newMagazine = new VirtualMag();

				//キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
				string newmagno = Plc.GetMagazineNo(ULMagazineAddress, QRMagazineNoLength);
				if (string.IsNullOrEmpty(newmagno))
				{
					throw new ApplicationException("排出マガジンNOの取得に失敗 または 取得マガジンNOが空です。");
				}

				newMagazine.MagazineNo = newmagno;
				newMagazine.LastMagazineNo = newmagno;
				
				//else
				//{
					//Log.RBLog.Info("プラズマ機排出マガジンNOの取得に失敗 または 取得マガジンNOが空です。取得先アドレス= " + ULMagazineAddress);
				//	return;
				//}

				// 稼働中マガジンを取得 (分割ロットは考慮していない)
				//Magazine svrmag = Magazine.GetCurrent(newmagno);
				//if (svrmag == null)
				//{
				//	return;
				//}

				//// 開始実績から作業IDと作業開始時間を取得
				//Order[] orders = Order.SearchOrder(svrmag.NascaLotNO, null, null, this.MacNo, true, false, null, null, null, null);
				//if (orders.Count() == 0)
				//{
				//	return;
				//}
				//else if (orders.Count() >= 2)
				//{
				//	throw new ApplicationException(
				//		string.Format("開始実績が複数存在する為、取得に失敗しました。MacNo:{0} LotNo:{1}", this.MacNo, svrmag.NascaLotNO));
				//}
				//Order startOrder = orders.Single();
				//newMagazine.ProcNo = startOrder.ProcNo;
				//newMagazine.WorkStart = startOrder.WorkStartDt;

				////作業開始完了時間取得
				//try
				//{
				//	newMagazine.WorkComplete = DateTime.Now;
				//	//newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				//}
				//catch
				//{
				//	throw new ApplicationException(string.Format("プラズマ機 作業完了時間取得失敗:{0}", this.MacNo));
				//}

				try
				{
					newMagazine.WorkStart = this.Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				}
				catch (Exception)
				{
					throw new ApplicationException(string.Format("開始時間の取得に失敗 MagazinNo:{0}", newMagazine.MagazineNo));
				}

				try
				{
					newMagazine.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				}
				catch (Exception)
				{
					throw new ApplicationException(string.Format("完了時間の取得に失敗 MagazinNo:{0}", newMagazine.MagazineNo));
				}

				//作業IDを取得
				newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newmagno);

				if (this.Enqueue(newMagazine, Station.Unloader))
				{
                    if (base.WorkComplete(newMagazine, this, true))
                    {
                        this.Dequeue(Station.Unloader);
                        Plc.SetWordAsDecimalData(this.WorkCompleteOKBitAddress, 1);
                        Thread.Sleep(1000);
                        Plc.SetBit(this.UnLoaderReqBitAddress, 1, PLC.Common.BIT_OFF);
                        //Plc.SetBit(this.WorkCompleteOKBitAddress, 1, PLC.Common.BIT_ON);

                        OutputSysLog(string.Format("[完了処理] 完了 OK信号送信 UnloaderMagazineNo:{0}", newMagazine.MagazineNo));
                    }
                    else
                    {
                        this.Dequeue(Station.Unloader);
                        throw new Exception(string.Format("作業実績の完了登録に失敗 MagazineNo:{0}", newMagazine.MagazineNo));
                    }
				}
			}
			catch (Exception ex)
            {
                OutputSysLog(string.Format("[完了処理] 失敗 NG信号送信 :{0}", ex.Message));

                Plc.SetWordAsDecimalData(this.WorkCompleteOKBitAddress, 2);
                Thread.Sleep(1000);
                Plc.SetBit(this.UnLoaderReqBitAddress, 1, PLC.Common.BIT_OFF);
                //Plc.SetBit(this.WorkCompleteNGBitAddress, 1, PLC.Common.BIT_ON);
                //throw;
			}
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

        /// <summary>
        /// マガジン供給(マガジン指定)　可否確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public override bool CanInput(VirtualMag mag)
        {
            //工程指定で判定
            if (ProcForbiddenAddressList.Keys.Contains(mag.ProcNo.Value) == false) return true;
            string address = ProcForbiddenAddressList[mag.ProcNo.Value];

            if (string.IsNullOrEmpty(address)) return true;

            string retv = Plc.GetBit(address);
            if (retv == Mitsubishi.BIT_ON)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 排出確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public override bool IsDischargeMode(VirtualMag mag)
        {
            if (mag == null) return false;

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null) return false;

            int proc = svrmag.NowCompProcess;

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

        public void workStart()
        {
            if (this.IsAutoLine) { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_AUTO; }
            else { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_HIGH; }

            VirtualMag mag = new VirtualMag();

            string magno = Plc.GetMagazineNo(LMagazineAddress, QRMagazineNoLength);
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
            //if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

            if (svrmag == null)
            {
                // 空マガジン
                SetCompletePLC(1, 2);
                OutputSysLog(string.Format("[開始処理] 完了 稼働中マガジンでは無い為、空マガジンとして扱います。 EmptyLoaderMagazineNo:{0}", magno));
            }
            else
            {
                // 実マガジン
                AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

                mag.ProcNo = nextproc.ProcNo;
                //mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                mag.WorkStart = DateTime.Now;

                Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

                ArmsApiResponse workResponse = CommonApi.WorkStart(order);
                if (workResponse.IsError)
                {
                    SetCompletePLC(2, 1);
                    throw new ApplicationException(
                        string.Format("[開始登録異常] 理由:{1}", this.MacNo, workResponse.Message));
                }
                else
                {
                    SetCompletePLC(1, 1);
                    OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", magno));
                }
            }
        }

        private void SetCompletePLC(int StartOK, int lMagazineMnfct)
        {
            Plc.SetWordAsDecimalData(this.WorkStartOKWordAddress, StartOK);
            Plc.SetWordAsDecimalData(this.lMagazineMnfctWordAddress, lMagazineMnfct);
            Thread.Sleep(200);
            Plc.SetBit(this.LoaderQRReadCompleteBitAddress, 1, PLC.Common.BIT_OFF);
        }
    }
}
