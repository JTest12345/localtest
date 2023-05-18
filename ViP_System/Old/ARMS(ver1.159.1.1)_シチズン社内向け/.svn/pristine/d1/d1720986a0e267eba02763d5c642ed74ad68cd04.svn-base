using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsApi;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 遠心沈降機
    /// </summary>
    public class ECK : MachineBase
    {
        /// <summary>
        /// 終了時排出マガジンNo取得アドレス
        /// </summary>
        public string ULMagazineAddress { get; set; }

        /// <summary>
        /// 供給アドレス
        /// </summary>
        public SortedList<int, string> LoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出アドレス
        /// </summary>
        public SortedList<int, string> UnloaderReqBitAddressList { get; set; }

        /// <summary>
        /// 供給FromTo
        /// </summary>
        public SortedList<int, string> LoaderPointList { get; set; }

        /// <summary>
        /// 排出FromTo
        /// </summary>
        public SortedList<int, string> UnloaderPointList { get; set; }

        /// <summary>
        /// ダミーマガジン置き場の供給要求信号
        /// </summary>
        public string DummyMagStationReqBitAddress { get; set; }

        /// <summary>
        /// ダミーマガジン置き場の排出要求信号
        /// </summary>
        public string DummyMagStationOutputReqBitAddress { get; set; }

        /// <summary>
        /// 沈降機へのダミーマガジン供給要求
        /// </summary>
        public string DummyMagReqBitAddress { get; set; }

        /// <summary>
        /// ダミーマガジン置き場供給移動ポイント
        /// </summary>
        public string DummyMagStationLoaderPoint { get; set; }

        /// <summary>
        /// ダミーマガジン置き場排出移動ポイント
        /// </summary>
        public string DummyMagStationUnloaderPoint { get; set; }

        /// <summary>
        /// ダミーマガジン投入タイマの一時停止
        /// </summary>
        private const string TIMER_OFF_BIT_ADDRESS = "B000D20";

        /// <summary>
        /// ダミーマガジンのマガジン番号(実際にはDUMMY1)
        /// </summary>
        private const string DUMMY_MAG_NO = "DUMMY";

        /// <summary>
        /// プログラム番号のBIT長
        /// </summary>
        private const int PROGRAM_NO_BIT_LENGTH = 8;

        public ECK()
            : base()
        {
            this.LoaderPointList = new SortedList<int, string>();
            this.LoaderReqBitAddressList = new SortedList<int, string>();
            this.UnloaderPointList = new SortedList<int, string>();
            this.UnloaderReqBitAddressList = new SortedList<int, string>();
        }

        protected override void concreteThreadWork() 
        {
            if (this.IsRequireOutput() == true)
            {
                workComplete();
            }
            
            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        #region workComplete
        private void workComplete()
        {
            //遠心沈降はマガジン供給位置が2カ所があるが、要求は常に片側しか立たない仕様

            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newmag = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newmag.MagazineNo = newmagno;
                newmag.LastMagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("遠心沈降機 排出マガジンNOの取得に失敗");
                return;
            }

			if (newmag.MagazineNo.ToUpper().StartsWith(DUMMY_MAG_NO))
			{
				//ダミーマガジンは完了登録無し
				this.Enqueue(newmag, Station.Unloader);
			}
			else
			{
				//作業開始完了時間取得
				try
				{
					newmag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				}
				catch
				{
					throw new ApplicationException(string.Format("遠心沈降機 作業完了時間取得失敗:{0}", this.MacNo));
				}

				try
				{
					newmag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				}
				catch
				{
					throw new ApplicationException(string.Format("遠心沈降機 作業開始時間取得失敗:{0}", this.MacNo));
				}

				//作業IDを取得
				newmag.ProcNo = Order.GetLastProcNo(this.MacNo, newmag.MagazineNo);
				this.Enqueue(newmag, Station.Unloader);
				base.WorkComplete(newmag, this, true);
			}
        }

        #endregion

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

        #region IsRequireOutput

        public override bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (this.UnloaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.UnloaderReqBitAddressList.Count == 0)
            {
                return false;
            }


            foreach (string address in this.UnloaderReqBitAddressList.Values)
            {
                //string retv = this.Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region IsRequireInput

        public override bool IsRequireInput()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //排出要求信号の確認
            if (this.LoaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.LoaderReqBitAddressList.Count == 0)
            {
                return false;
            }


            foreach (string address in this.LoaderReqBitAddressList.Values)
            {
                //string retv = this.Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region ResponseEmptyMagazineRequest

        public override bool ResponseEmptyMagazineRequest()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);

            //ダミーマガジン以外はここで処理しない
            if (ulMagazine != null && ulMagazine.MagazineNo.ToUpper().StartsWith(DUMMY_MAG_NO))
            {
                if (this.IsRequireOutput() == true)
                {
                    //沈降機の排出要求ON　&　ダミー置き場の供給要求ONの場合は移動
                    if (Plc.GetBit(this.DummyMagStationReqBitAddress) == Mitsubishi.BIT_ON)
                    {
                        Location from = new Location(this.MacNo, Station.Unloader);
                        Location to = new Location(this.MacNo, Station.DummyMagStationLoader);
                        LineKeeper.MoveFromTo(from, to, true, false, false, true);
                        return true;
                    }
                    else
                    {
                        //ダミーマガジン置き場に置けない場合はライン外へ排出
                        Location from = new Location(this.MacNo, Station.Unloader);
                        IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                        Location to = dischargeConveyor.GetLoaderLocation();
                        LineKeeper.MoveFromTo(from, to, true, false, false, true);
                        return true;
                    }
                }
            }

            //沈降機時間切れのダミーマガジン供給要求
            if (Plc.GetBit(this.DummyMagReqBitAddress) == Mitsubishi.BIT_ON)
            {
                if (Plc.GetBit(this.DummyMagStationOutputReqBitAddress) == Mitsubishi.BIT_ON)
                {
                    Location from = new Location(this.MacNo, Station.DummyMagStationUnloader);
                    Location to = new Location(this.MacNo, Station.Loader);
                    LineKeeper.MoveFromTo(from, to, false, true, false);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region GetFromToCode

        public override string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.Loader:
                    foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.LoaderPointList[kv.Key];
                        }
                    }
                    return this.LoaderPointList[1];

                case Station.Unloader:
                    foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.UnloaderPointList[kv.Key];
                        }
                    }
                    return this.UnloaderPointList[1];

                case Station.DummyMagStationLoader:
                    return this.DummyMagStationLoaderPoint;

                case Station.DummyMagStationUnloader:
                    return this.DummyMagStationUnloaderPoint;
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }
        #endregion

        /// <summary>
        /// 搬送可能マガジンがある場合に、ダミーマガジン投入タイマを一時停止する
        /// </summary>
        /// <param name="plc"></param>
        public void SetDummyMagTimerOff()
        {
            Plc.SetBit(TIMER_OFF_BIT_ADDRESS, 1, Mitsubishi.BIT_ON);
        }
    }
}
