using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 内製スクリューMD(初導入:SLP2)
	/// </summary>
	public class Mold7 : Mold
	{
		private const string HEART_BEAT_ADDRESS = "EM40007";
		///// <summary>
		///// 排出信号アドレスリスト
		///// </summary>
		//public List<string> UnLoaderReqBitAddressList { get; set; }

		///// <summary>
		///// 排出信号の記憶
		///// </summary>
		//public Dictionary<string, bool> UnLoaderReqBitMemory { get; set; }

		/// <summary>装置側で設定される自動開始・完了登録の有効/無効の信号アドレス</summary>
		public string EnableWorkRecordAutoRegisterBitAddress { get; set; }

		/// <summary>開始登録時のOK/NG返答用アドレス</summary>
		public string WorkStartOKBitAddress { get; set; }
		public string WorkStartNGBitAddress { get; set; }

		/// <summary>完了登録時のOK/NG返答用アドレス</summary>
		public string WorkEndOKBitAddress { get; set; }
		public string WorkEndNGBitAddress { get; set; }

		public string WorkStartTrigAddress { get; set; }

		public string WorkEndTrigAddress { get; set; }

		public string StartAutoRegisterNGKindAddress { get; set; }

		public string EndAutoRegisterNGKindAddress { get; set; }

		public Mold7()
        {
        }

		protected override void concreteThreadWork()
		{
			try
			{
				if (Plc.GetBit(EnableWorkRecordAutoRegisterBitAddress) == PLC.Common.BIT_ON)
				{
					if (Plc.GetBit(this.WorkStartTrigAddress) == PLC.Common.BIT_ON)
					{
						workStart();
					}

					if (this.IsRequireOutput() == true)
					{
						workCompletehigh();
					}
				}

				CheckMachineLogFile();

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);
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
			VirtualMag mag = new VirtualMag();
			try
			{
				string magno = Plc.GetMagazineNo(LMagazineAddress);
				if (string.IsNullOrEmpty(magno) == false)
				{
					mag.MagazineNo = magno;
					mag.LastMagazineNo = magno;
				}
				else
				{
					throw new ApplicationException("モールド装置　供給マガジンNOの取得に失敗。");
				}

				Magazine svrmag = Magazine.GetCurrent(magno);
				if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

				OutputSysLog($"[開始処理] 開始 LoaderMagazineNo:{magno}");

				AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
				Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

				mag.ProcNo = nextproc.ProcNo;

				//同ロットで他装置での開始実績が有ると開始処理エラーにする。
				List<Order> orderList = ArmsApi.Model.Order.SearchOrder(svrmag.NascaLotNO, mag.ProcNo, null, true, false).ToList();

				if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
				{
					MachineInfo macInfo = MachineInfo.GetMachine(this.MacNo);

					throw new ApplicationException($"[開始登録異常] 装置:『{macInfo.LongName}』 理由:他の装置での開始実績が既に存在します。"
						+ $"macno(複数の場合カンマ区切り):『{string.Join(",", orderList.Select(o => o.MacNo))}』");
				}

				Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

				ArmsApiResponse workResponse = CommonApi.WorkStart(order);
				if (workResponse.IsError)
				{
					throw new ApplicationException(workResponse.Message);
				}
			}
			catch(Exception ex)
			{
				//自動登録NGのNG種類 装置側から値：1を固定で入力するように指示があった
				Plc.SetWordAsDecimalData(this.StartAutoRegisterNGKindAddress, 1);

				Plc.SetBit(this.WorkStartNGBitAddress, 1, PLC.Common.BIT_ON);
				Log.ApiLog.Info($"[開始登録異常] 装置:{this.MacNo} 理由:{ex.Message} StackTrace:{ex.StackTrace}");
			}

			Plc.SetBit(this.WorkStartOKBitAddress, 1, PLC.Common.BIT_ON);
			OutputSysLog($"[開始処理] 完了 LoaderMagazineNo:{mag.MagazineNo}");
		}

		private void workCompletehigh()
		{
			VirtualMag lMag = new VirtualMag();

			try
			{
				string newmagno = Plc.GetMagazineNo(this.ULMagazineAddress);

				if (string.IsNullOrEmpty(newmagno) == false)
				{
					lMag.MagazineNo = newmagno;
					lMag.LastMagazineNo = newmagno;
				}
				else
				{
					Log.RBLog.Info($"モールド装置 排出マガジンNOの取得に失敗:{this.MacNo}");
					return;
				}

				//作業開始時間取得
				try
				{
					lMag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				}
				catch (Exception ex)
				{
					Log.SysLog.Error(ex.ToString());
					throw new ApplicationException($"モールド装置 作業開始時間取得失敗:{this.MacNo}");
				}

				//作業完了時間取得
				try
				{
					lMag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				}
				catch
				{
					throw new ApplicationException($"モールド装置 作業完了時間取得失敗:{this.MacNo}");
				}

				//作業IDを取得
				lMag.ProcNo = Order.GetLastProcNo(this.MacNo, lMag.MagazineNo);
				base.WorkComplete(lMag, this, true);

				lMag.LastMagazineNo = lMag.MagazineNo;

				Plc.SetBit(this.WorkEndOKBitAddress, 1, PLC.Common.BIT_ON);
				//Plc.SetBit(this.WorkEndTrigAddress, 1, PLC.Common.BIT_ON);

				OutputSysLog($"[完了処理] 完了 UnloaderMagazineNo:{lMag.MagazineNo}");
			}
			catch(Exception ex)
			{
				//自動登録NGのNG種類 装置側から値：1を固定で入力するように指示があった
				Plc.SetWordAsDecimalData(this.EndAutoRegisterNGKindAddress, 1);
				Plc.SetBit(this.WorkEndNGBitAddress, 1, PLC.Common.BIT_ON);
				Log.ApiLog.Info($"[完了登録異常] 装置:{this.MacNo} 理由:{ex.Message} StackTrace:{ex.StackTrace}");
			}
		}

		/// <summary>
		/// 排出信号をチェックして排出要求があればTrueを返す
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireOutput()
		{
			if(this.Plc.GetBit(this.WorkEndTrigAddress) == PLC.Common.BIT_ON)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
