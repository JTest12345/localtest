using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 遠心沈降機(MAPバッファ付き)
	/// </summary>
	public class ECK3 : MachineBase
	{
		#region 供給側バッファ

		/// <summary>
		/// 供給側バッファ供給アドレスリスト
		/// </summary>
		public SortedList<int, string> InputBufferLoaderReqBitAddressList { get; set; }

		/// <summary>
		/// 供給側バッファ排出アドレスリスト
		/// </summary>
		public SortedList<int, string> InputBufferUnloaderReqBitAddressList { get; set; }

		/// <summary>
		/// 供給側バッファ供給搬送位置リスト
		/// </summary>
		public SortedList<int, string> InputBufferLoaderPointList { get; set; }

		/// <summary>
		/// 供給側バッファ排出搬送位置リスト
		/// </summary>
		public SortedList<int, string> InputBufferUnloaderPointList { get; set; }

		/// <summary>
		/// 供給側バッファ搬送位置リスト
		/// </summary>
		public SortedList<int, Station> InputBufferLoaderLocationList { get; set; }

		/// <summary>
		/// 供給側バッファ搬送位置リスト
		/// </summary>
		public SortedList<int, Station> InputBufferUnloaderLocationList { get; set; }

		#endregion

		#region 排出側バッファ

		/// <summary>
		/// 排出側バッファ供給アドレスリスト
		/// </summary>
		public SortedList<int, string> OutputBufferLoaderReqBitAddressList { get; set; }

		/// <summary>
		/// 排出側バッファ排出アドレスリスト
		/// </summary>
		public SortedList<int, string> OutputBufferUnloaderReqBitAddressList { get; set; }

		/// <summary>
		/// 排出側バッファ供給搬送位置リスト
		/// </summary>
		public SortedList<int, string> OutputBufferLoaderPointList { get; set; }

		/// <summary>
		/// 排出側バッファ排出搬送位置リスト
		/// </summary>
		public SortedList<int, string> OutputBufferUnloaderPointList { get; set; }

		/// <summary>
		/// 排出側バッファ搬送位置リスト
		/// </summary>
		public SortedList<int, Station> OutputBufferLoaderLocationList { get; set; }

		/// <summary>
		/// 排出側バッファ搬送位置リスト
		/// </summary>
		public SortedList<int, Station> OutputBufferUnloaderLocationList { get; set; }

		#endregion

		#region 本体

		/// <summary>
		/// 供給アドレスリスト
		/// </summary>
		public SortedList<int, string> LoaderReqBitAddressList { get; set; }

		/// <summary>
		/// 排出アドレスリスト
		/// </summary>
		public SortedList<int, string> UnloaderReqBitAddressList { get; set; }

		/// <summary>
		/// 搬送位置リスト
		/// </summary>
		public SortedList<int, string> LoaderPointList { get; set; }

		/// <summary>
		/// 搬送位置リスト
		/// </summary>
		public SortedList<int, string> UnloaderPointList { get; set; }

		/// <summary>
		/// 排出マガジンNo取得アドレス
		/// </summary>
		public string ULMagazineAddress { get; set; }

		/// <summary>
		/// 沈降前マガジン供給予定信号
		/// </summary>
		public string LoaderReqPlanBitAddress { get; set; }

		/// <summary>
		/// 沈降済みマガジン排出予定信号
		/// </summary>
		public string UnloaderReqPlanBitAddress { get; set; }

		#endregion

		#region ダミーマガジン

		/// <summary>
		/// ダミーマガジン投入タイマの一時停止
		/// </summary>
		private const string TIMER_OFF_BIT_ADDRESS = "B000D20";

		/// <summary>
		/// ダミーマガジンのマガジン番号(実際にはDUMMY1)
		/// </summary>
		private const string DUMMY_MAG_NO = "DUMMY";

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

		#endregion

		public ECK3()
			: base()
		{
			this.InputBufferLoaderReqBitAddressList = new SortedList<int, string>();
			this.InputBufferUnloaderReqBitAddressList = new SortedList<int, string>();
			this.InputBufferLoaderPointList = new SortedList<int, string>();
			this.InputBufferUnloaderPointList = new SortedList<int, string>();
			this.InputBufferLoaderLocationList = new SortedList<int, Station>();
			this.InputBufferUnloaderLocationList = new SortedList<int, Station>();

			this.OutputBufferLoaderReqBitAddressList = new SortedList<int, string>();
			this.OutputBufferUnloaderReqBitAddressList = new SortedList<int, string>();
			this.OutputBufferLoaderPointList = new SortedList<int, string>();
			this.OutputBufferUnloaderPointList = new SortedList<int, string>();
			this.OutputBufferLoaderLocationList = new SortedList<int, Station>();
			this.OutputBufferUnloaderLocationList = new SortedList<int, Station>();

			this.LoaderReqBitAddressList = new SortedList<int, string>();
			this.UnloaderReqBitAddressList = new SortedList<int, string>();
			this.LoaderPointList = new SortedList<int, string>();
			this.UnloaderPointList = new SortedList<int, string>();

			//this.InputBufferLocationList.Add(1, Station.Loader1);
			//this.InputBufferLocationList.Add(2, Station.Loader2);

			//this.OutputBufferLocationList.Add(1, Station.Unloader1);
			//this.OutputBufferLocationList.Add(2, Station.Unloader2);
		}

		protected override void concreteThreadWork()
		{
			if (IsRequireOutput() == true)
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
		}

		/// <summary>
		/// 装置⇒排出バッファ、供給側バッファ⇒装置への移動
		/// </summary>
		/// <returns></returns>
		public override bool ResponseEmptyMagazineRequest()
		{
			bool isMove = false;

			//装置⇒排出側バッファへの移動　
			//※排出予定信号ON 又は 排出信号ONの間、処理を繰り返す
			while (Plc.GetBit(this.UnloaderReqPlanBitAddress) == Mitsubishi.BIT_ON || isRequireOutput())
			{
				foreach (KeyValuePair<int, string> address in this.UnloaderReqBitAddressList)
				{
					if (Plc.GetBit(address.Value) == Mitsubishi.BIT_ON)
					{
						if (isRequireOutputBufferInput())
						{
							#region この時点でバッファへEnqueue

							VirtualMag newmag = new VirtualMag();

							//キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
							string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
							if (string.IsNullOrEmpty(newmagno) == false)
							{
								newmag.MagazineNo = newmagno;

								//lastmagnoを空白
								newmag.LastMagazineNo = newmagno;
							}
							else
							{
								throw new ApplicationException(string.Format("遠心沈降機 排出マガジンNOの取得に失敗:{0}", this.MacNo));
							}

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
							base.Enqueue(newmag, this.OutputBufferUnloaderLocationList[address.Key]);
							base.WorkComplete(newmag, this, true);
							#endregion

							LineKeeper.MoveFromTo(
								new Location(this.MacNo, Station.Unloader), new Location(this.MacNo, OutputBufferLoaderLocationList[address.Key]),
								false, false, false, false);
							isMove = true;
						}
					}
				}
			}
			
			//供給側バッファ⇒装置への移動　
			//※供給予定信号ON＆供給側バッファ排出信号ON 又は 供給信号ON＆供給側バッファ排出信号ONの間、処理を繰り返す
			while ((Plc.GetBit(this.LoaderReqPlanBitAddress) == Mitsubishi.BIT_ON && isRequireInputBufferOutput())
				|| (isRequireInput() && isRequireInputBufferOutput()))
			{
				foreach (KeyValuePair<int, string> address in this.InputBufferUnloaderReqBitAddressList)
				{
					if (Plc.GetBit(address.Value) == Mitsubishi.BIT_ON)
					{
						if (isRequireInput())
						{
							LineKeeper.MoveFromTo(
								new Location(this.MacNo, InputBufferUnloaderLocationList[address.Key]), new Location(this.MacNo, Station.Loader),
								false, false, false, false);
							isMove = true;
						}
					}
				}
			}

			//ダミーマガジン以外はここで処理しない
			//VirtualMag ulMagazine = this.Peek(GetUnLoaderLocation().Station);
			//if (ulMagazine != null && ulMagazine.MagazineNo.ToUpper().StartsWith(DUMMY_MAG_NO))
			//{
			//    if (this.IsRequireOutput() == true)
			//    {
			//        //排出側バッファ排出要求ON　&　ダミー置き場の供給要求ONの場合は移動
			//        if (Plc.GetBit(this.DummyMagStationReqBitAddress) == PLC.BIT_ON)
			//        {
			//            Location from = GetUnLoaderLocation();
			//            Location to = new Location(this.MacNo, Station.DummyMagStationLoader);
			//            LineKeeper.MoveFromTo(from, to, true, false, false, true);
			//            isMove = true;
			//        }
			//        else
			//        {
			//            //ダミーマガジン置き場に置けない場合はライン外へ排出
			//            Location from = GetUnLoaderLocation();
			//            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
			//            Location to = dischargeConveyor.GetLoaderLocation();
			//            LineKeeper.MoveFromTo(from, to, true, false, false, true);
			//            isMove = true;
			//        }
			//    }
			//}

			////沈降機時間切れのダミーマガジン供給要求
			//if (Plc.GetBit(this.DummyMagReqBitAddress) == PLC.BIT_ON)
			//{
			//    if (Plc.GetBit(this.DummyMagStationOutputReqBitAddress) == PLC.BIT_ON)
			//    {
			//        Location from = new Location(this.MacNo, Station.DummyMagStationUnloader);
			//        Location to = new Location(this.MacNo, Station.Loader);
			//        LineKeeper.MoveFromTo(from, to, false, true, false);
			//        isMove = true;
			//    }
			//}

			return isMove;
		}

		public override bool Enqueue(VirtualMag mag, Station station)
		{
			//何もしない
            return true;
			////排出側バッファのアンローダー以外は何もしない
			//if (OutputBufferUnloaderLocationList.ContainsValue(station) == false)
			//{
			//	return;
			//}

			//base.Enqueue(mag, station);
		}

		/// <summary>
		/// 実マガジン供給 ※供給側バッファ供給信号で判定
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireInput()
		{
			if (base.IsInputForbidden() == true)
			{
				return false;
			}

			if (InputBufferLoaderReqBitAddressList == null || InputBufferLoaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "InputBufferLoaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.InputBufferLoaderReqBitAddressList)
			{
                //string retv = this.Plc.GetBit(kv.Value);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(kv.Value);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{kv.Value}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }
                if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 供給側バッファ排出
		/// </summary>
		/// <returns></returns>
		private bool isRequireInputBufferOutput() 
		{
			if (InputBufferUnloaderReqBitAddressList == null || InputBufferUnloaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "InputBufferUnloaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.InputBufferUnloaderReqBitAddressList)
			{
				string retv = this.Plc.GetBit(kv.Value);
				if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}

		#region 装置要求確認

		/// <summary>
		/// 実マガジン供給
		/// </summary>
		/// <returns></returns>
		private bool isRequireInput()
		{
			if (LoaderReqBitAddressList == null || LoaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "LoaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
			{
				string retv = this.Plc.GetBit(kv.Value);
				if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 実マガジン排出
		/// </summary>
		/// <returns></returns>
		private bool isRequireOutput()
		{
			if (UnloaderReqBitAddressList == null || UnloaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "UnloaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
			{
				string retv = this.Plc.GetBit(kv.Value);
				if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		/// <summary>
		/// 排出側バッファ供給要求
		/// </summary>
		/// <returns></returns>
		private bool isRequireOutputBufferInput()
		{
			if (OutputBufferLoaderReqBitAddressList == null || OutputBufferLoaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "OutputBufferLoaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.OutputBufferLoaderReqBitAddressList)
			{
				string retv = this.Plc.GetBit(kv.Value);
				if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// 実マガジン排出 ※排出側バッファ排出信号で判定
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireOutput()
		{
			if (OutputBufferUnloaderReqBitAddressList == null || OutputBufferUnloaderReqBitAddressList.Count == 0)
			{
				Log.SysLog.Info(
					string.Format("lineconfigに設定されていない項目が参照されています。 MacNo:{0} AddressNm:{1}",
					this.MacNo, "OutputBufferUnloaderReqBitAddressList"));

				return false;
			}

			foreach (KeyValuePair<int, string> kv in this.OutputBufferUnloaderReqBitAddressList)
			{
                //string retv = this.Plc.GetBit(kv.Value);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(kv.Value);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{kv.Value}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }
                if (retv == Mitsubishi.BIT_ON)
				{
					return true;
				}
			}

			return false;
		}

		public override string GetFromToCode(Station station)
		{
			switch (station)
			{
				case Station.Loader1:
					return this.InputBufferLoaderPointList[1];
				case Station.Loader2:
					return this.InputBufferLoaderPointList[2];
				case Station.Loader3:
					return this.OutputBufferLoaderPointList[1];
				case Station.Loader4:
					return this.OutputBufferLoaderPointList[2];

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

				case Station.Unloader1:
					return this.InputBufferUnloaderPointList[1];
				case Station.Unloader2:
					return this.InputBufferUnloaderPointList[2];
				case Station.Unloader3:
					return this.OutputBufferUnloaderPointList[1];
				case Station.Unloader4:
					return this.OutputBufferUnloaderPointList[2];

				case Station.DummyMagStationLoader:
					return this.DummyMagStationLoaderPoint;

				case Station.DummyMagStationUnloader:
					return this.DummyMagStationUnloaderPoint;
			}

			throw new ApplicationException(
				string.Format("定義外のStationのGetFromToCode MacNo:{0} Station:{1}", this.MacNo, station));
		}

		#region GetLoaderLocation

		public override Location GetLoaderLocation()
		{
			foreach (KeyValuePair<int, string> kv in this.InputBufferLoaderReqBitAddressList)
			{
				string retv = Plc.GetBit(kv.Value);
				if (retv == Mitsubishi.BIT_ON)
				{
					return new Location(this.MacNo, this.InputBufferLoaderLocationList[kv.Key]);
				}
			}

			//投入先が無ければ排出コンベヤを返す
			Log.RBLog.Info("遠心沈降機で投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");
			IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
			return dischargeConveyor.GetLoaderLocation();
		}

		#endregion

		#region GetUnloaderLocation

		public override Location GetUnLoaderLocation()
		{
			foreach (KeyValuePair<int, string> kv in this.OutputBufferUnloaderReqBitAddressList)
			{
				string retv = Plc.GetBit(kv.Value);

				if (retv == Mitsubishi.BIT_ON)
				{
					return new Location(this.MacNo, this.OutputBufferUnloaderLocationList[kv.Key]);
				}
			}

			//投入先が無ければnull
			//Log.RBLog.Info("遠心沈降機で排出元が無い状態で排出元の問い合わせ発生。");
			return null;
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
