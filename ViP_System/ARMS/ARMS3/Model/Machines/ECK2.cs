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
	/// 遠心沈降機(SIDEVIEW自動機用)
	/// </summary>
	public class ECK2 : MachineBase
	{
		/// <summary>
		/// 終了時排出マガジンNo取得アドレス
		/// </summary>
		public string ULMagazineAddress { get; set; }

		/// <summary>
		/// 開始時マガジンNo取得アドレス
		/// </summary>
		public string LMagazineAddress { get; set; }

		/// <summary>
		/// 直近100ロットの完成履歴
		/// </summary>
		public Queue<string> preCompleteLotQueue = new Queue<string>();

		/// <summary>
		/// アンローダーマガジン有無確認の最初のアドレス
		/// </summary>
		public string UnloaderReqBitAddressStart { get; set; }

		/// <summary>
		/// マガジン情報全体の開始アドレス
		/// </summary>
		public string MagazineDataAddressStart { get; set; }

		/// <summary>
		/// 遠心沈降中架空マガジンNOの識別文字
		/// </summary>
		public string WORKMAGAZINE_PREFIX = "_E";

		/// <summary>
		/// アンローダーマガジン数
		/// </summary>
		private const int UNLOADER_MAGAZINE_COUNT = 6;

		/// <summary>
		/// 1マガジンデータ長
		/// </summary>
		private const int MAGAZINE_DATA_LENGTH = 50;

		private enum MagazineType
		{
			Magazine = 1,
			EmptyMagazine = 2,
		}

		#region アンローダーマガジン情報読み込みアドレス定数

		private const int LOADER_MAGAZINE_NO_OFFSET = 0;

		private const int UNLOADER_MAGAZINE_NO_OFFSET = 10;

		private const int MAGAZINE_NO_LENGTH = 10;

		private const int WORK_START_ADDRESS_OFFSET = 20;

		private const int WORK_COMPLETE_ADDRESS_OFFSET = 26;

		private const int UNLOADER_DATA_LENGTH = 300;

		private const int DATE_TIME_ADDRESS_LENGTH = 6;

		private const int MAGAZINE_TYPE_LENGTH = 1;

		#endregion

		public ECK2() { }

		protected override void concreteThreadWork()
		{
			if (base.IsRequireOutput() == true)
			{
				if (this.IsAutoLine)
				{
					workComplete();
				}
				else
				{
					workCompletehigh();
				}
			}

			//仮想マガジン消去要求応答
			ResponseClearMagazineRequest();
		}

		/// <summary>
		/// 作業完了(自動化)
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
			string newmagno = Plc.GetMagazineNo(ULMagazineAddress, Mitsubishi.MAGAZINE_NO_WORD_LENGTH);
			if (string.IsNullOrEmpty(newmagno) == false)
			{
				newMagazine.MagazineNo = newmagno;
			}
			else
			{
				Log.RBLog.Info(string.Format("遠心沈降機 排出マガジンNOの取得に失敗 アドレス:{0} 取得値:{1}", ULMagazineAddress, newmagno));
				return;
			}

			string oldmagno = Plc.GetMagazineNo(LMagazineAddress, Mitsubishi.MAGAZINE_NO_WORD_LENGTH);
			if (string.IsNullOrEmpty(oldmagno) == false)
			{
				//Enqueueの処理でTnMag.magnoは架空マガジンNOに入れ替わっている為、(架空マガジンNO：マガジンNO+識別文字(M******_E))
				//PLCから受信したマガジンNOを架空マガジンNOに振替
				newMagazine.LastMagazineNo = string.Format("{0}{1}", oldmagno, WORKMAGAZINE_PREFIX);
			}
			else
			{
				Log.RBLog.Info("遠心沈降機 搬入側マガジンNOの取得に失敗");
				return;
			}

			//作業開始完了時間取得
			try
			{
				newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
			}
			catch
			{
				throw new ApplicationException(string.Format("遠心沈降機 作業完了時間取得失敗:{0}" + this.MacNo));
			}

			try
			{
				newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
			}
			catch
			{
				throw new ApplicationException(string.Format("遠心沈降機 作業開始時間取得失敗:{0}", this.MacNo));
			}

			VirtualMag oldmag = new VirtualMag();
			oldmag.MagazineNo = oldmagno;
			oldmag.LastMagazineNo = oldmagno;

			//作業IDを取得
			newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newMagazine.LastMagazineNo);

			this.Enqueue(newMagazine, Station.Unloader);

			this.WorkComplete(newMagazine, this, true);
		}

		/// <summary>
		/// 作業完了(高効率)
		/// </summary>
		private void workCompletehigh()
		{
			//実マガジン又は空マガジンか判断するデータを全て取得
			string[] magazineExists = Plc.GetBitArray(this.UnloaderReqBitAddressStart, UNLOADER_DATA_LENGTH);

			//アンローダーの全マガジンのデータを全て取得
			string[] raw = Plc.GetBitArray(this.MagazineDataAddressStart, UNLOADER_DATA_LENGTH);

			if (magazineExists == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY || raw == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY)
			{
				return;
			}

			for (int i = 0; i < UNLOADER_MAGAZINE_COUNT; i++)
			{
				//実体がある位置のデータを小分けにしてマガジン情報に展開
				//if (magazineExists[i] == PLC.BIT_ON)
				//{
				string[] magTypeChar = new string[MAGAZINE_TYPE_LENGTH];
				Array.Copy(magazineExists, i * MAGAZINE_DATA_LENGTH, magTypeChar, 0, MAGAZINE_TYPE_LENGTH);
				int magType = int.Parse(magTypeChar[0]);
				if (magType == ((int)MagazineType.Magazine))
				{
					VirtualMag mag = parseMagazine(raw, i * MAGAZINE_DATA_LENGTH);
					if (mag == null)
					{
						continue;
					}
					Magazine svrmag = Magazine.GetCurrent(mag.LastMagazineNo);

					//直近100ロットの完成リストに存在する場合は何もしない
					if (preCompleteLotQueue.Contains(svrmag.NascaLotNO) == true)
					{
						continue;
					}

					//Enqueueが先にないとLoc情報が無い
					this.Enqueue(mag, Station.Unloader);
					base.WorkComplete(mag, this, true);
					this.Dequeue(Station.Unloader);

					//高効率でArmsWebが作成してしまうので削除
					this.Dequeue(Station.Loader);

					//直近100マガジンの完成記憶
					preCompleteLotQueue.Enqueue(svrmag.NascaLotNO);
					if (preCompleteLotQueue.Count >= 100)
					{
						preCompleteLotQueue.Dequeue();
					}
				}
			}
		}

		/// <summary>
		/// アンローダー指定位置のマガジン情報を取得
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="startOffset"></param>
		/// <param name="plc"></param>
		/// <returns></returns>
		private VirtualMag parseMagazine(string[] raw, int startOffset)
		{
			string[] workstart = new string[DATE_TIME_ADDRESS_LENGTH];
			string[] workcomplete = new string[DATE_TIME_ADDRESS_LENGTH];
			string[] lmagazineno = new string[MAGAZINE_NO_LENGTH];
			string[] ulmagazineno = new string[MAGAZINE_NO_LENGTH];

			try
			{
				Array.Copy(raw, startOffset + LOADER_MAGAZINE_NO_OFFSET, lmagazineno, 0, MAGAZINE_NO_LENGTH);
				Array.Copy(raw, startOffset + UNLOADER_MAGAZINE_NO_OFFSET, ulmagazineno, 0, MAGAZINE_NO_LENGTH);
				Array.Copy(raw, startOffset + WORK_START_ADDRESS_OFFSET, workstart, 0, DATE_TIME_ADDRESS_LENGTH);
				Array.Copy(raw, startOffset + WORK_COMPLETE_ADDRESS_OFFSET, workcomplete, 0, DATE_TIME_ADDRESS_LENGTH);
			}
			catch (Exception ex)
			{
				Log.ApiLog.Error("遠心沈降機の排出マガジン情報パースエラー offset:" + startOffset, ex);
				return null;
			}

			VirtualMag mag = new VirtualMag();

			//分割なしマガジン番号を返す
			mag.MagazineNo = Plc.GetMagazineNo(ulmagazineno, true);
			mag.LastMagazineNo = Plc.GetMagazineNo(lmagazineno, true);
			mag.WorkStart = Plc.GetWordsAsDateTime(workstart);
			mag.WorkComplete = Plc.GetWordsAsDateTime(workcomplete);

			if (string.IsNullOrEmpty(mag.MagazineNo) == true)
			{
				Log.ApiLog.Info("遠心沈降機でアンローダーマガジン番号取得失敗");
				return null;
			}

			if (string.IsNullOrEmpty(mag.LastMagazineNo) == true)
			{
				Log.ApiLog.Info("遠心沈降機でローダーマガジン番号取得失敗");
				return null;
			}

			if (mag.WorkStart.HasValue == false || mag.WorkComplete.HasValue == false)
			{
				Log.ApiLog.Info("遠心沈降機で作業時間取得失敗");
				return null;
			}

			mag.LastMagazineNo = string.Format("{0}{1}", mag.LastMagazineNo, WORKMAGAZINE_PREFIX);

			Magazine svrmag
				= Magazine.GetCurrent(mag.LastMagazineNo);
			if (svrmag == null)
			{
				return null;
			}

			AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
			mag.ProcNo = nextproc.ProcNo;

			return mag;
		}

		public override bool ResponseEmptyMagazineRequest()
		{
			//供給禁止状態なら処理しない
			if (this.IsInputForbidden() == true)
			{
				return false;
			}

			bool retv = false;

			if (IsRequireOutputEmptyMagazine() == true)
			{
				VirtualMag mag = this.Peek(Station.EmptyMagazineUnloader);
				if (mag == null)
				{
					VirtualMag ulempmag = new VirtualMag();

					string magno = Plc.GetMagazineNo(ULMagazineAddress, Mitsubishi.MAGAZINE_NO_WORD_LENGTH);
					if (string.IsNullOrEmpty(magno) == false)
					{
						ulempmag.MagazineNo = magno;
						ulempmag.LastMagazineNo = magno;
					}
					else
					{
						Log.RBLog.Info(string.Format("遠心沈降機 排出マガジンNOの取得に失敗 アドレス:{0} 取得値:{1}", ULMagazineAddress, magno));
						return false;
					}

					this.Enqueue(ulempmag, Station.EmptyMagazineUnloader);

					Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
					IMachine conveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
					Location to = conveyor.GetLoaderLocation();
					LineKeeper.MoveFromTo(from, to, true, true, false);

					retv = true;
				}
			}

			bool IsDeleteFromMag = false;
			if (this.IsRequireEmptyMagazine() == true)
			{
				Location from = null;
				LineBridge bridge = LineKeeper.GetReachBridge(this.MacNo);

				//ライン連結橋の空マガジンを使用
				if (bridge != null && bridge.IsRequireOutputEmptyMagazine())
				{
					from = bridge.GetUnLoaderLocation();
					IsDeleteFromMag = true;
				}
				//空マガジン投入CVの空マガジンを使用
				else
				{
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
							if (bridgeMags.Count != 0)
							{
								if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
								{
									return false;
								}
							}
						}

						from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
						IsDeleteFromMag = false;
					}
					else
					{
						//空マガジン投入CVにマガジンが無い場合
						Log.RBLog.Info(string.Format("{0} 空マガジン投入CVにマガジンが無い為、空マガジンを搬入できません。", this.Name));
						return false;
					}
				}

				Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
				LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);

				retv = true;
			}

			return retv;
		}

		/// <summary>
		/// 仮想マガジン作成
		/// </summary>
		/// <param name="mag"></param>
		/// <param name="station"></param>
		public override bool Enqueue(VirtualMag mag, Station station)
		{
			if (station == Station.Loader)
			{
				// 供給後、フレームのみ装置内に入り、排出で別マガジンへ格納される為、TnMag.magnoを架空マガジンに振替える。
				// (架空マガジンNO：マガジンNO+識別文字(******_E))
				Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
				Order order = Order.GetMagazineOrder(svrmag.NascaLotNO, mag.ProcNo.Value);
				AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);

				order.InMagazineNo = mag.MagazineNo;
				order.OutMagazineNo = string.Format("{0}{1}", mag.MagazineNo, WORKMAGAZINE_PREFIX);

				//ApplyMagazineInOutで現在完了作業が現作業に書き変わってしまう為、一つ前の作業にする。
				order.ProcNo = Process.GetPrevProcess(order.ProcNo, lot.TypeCd).ProcNo;

				Magazine.ApplyMagazineInOut(order, order.LotNo);

                return true;
			}
			else if (station == Station.Unloader || station == Station.EmptyMagazineUnloader)
			{
				//アンローダー側のみ仮想マガジンを作成する
				return base.Enqueue(mag, station);
			}

            return true;
		}
	}
}
