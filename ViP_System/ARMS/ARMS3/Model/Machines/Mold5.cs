using ARMS3.Model.PLC;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// モールド 山本製(SLS2)
	/// </summary>
	public class Mold5 : MachineBase
	{
		/// <summary>
		/// 完了登録OKBit
		/// </summary>
		public string WorkCompleteOKBitAddress { get; set; }

		/// <summary>
		/// 完了登録NGBit
		/// </summary>
		public string WorkCompleteNGBitAddress { get; set; }

		protected override void concreteThreadWork()
		{
			try 
			{
				if (this.IsRequireOutput() == true)
				{
					workComplete();
				}

				checkWorkStart();
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

		private void workComplete() 
		{
			try
			{
				VirtualMag lMag = this.Peek(Station.Loader);
				if (lMag == null)
				{
					return;
				}

				OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

				try
				{
					lMag.WorkStart = this.Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				}
				catch (Exception)
				{
					throw new ApplicationException(string.Format("モールド機から開始時間の取得に失敗 MagazinNo:{0}", lMag.MagazineNo));
				}

				try
				{
					lMag.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				}
				catch (Exception)
				{
					throw new ApplicationException(string.Format("モールド機から完了時間の取得に失敗 MagazinNo:{0}", lMag.MagazineNo));
				}

				lMag.LastMagazineNo = lMag.MagazineNo;

				Order order = Order.GetMachineStartOrder(this.MacNo);
				if (order == null)
				{
					throw new ApplicationException(string.Format("装置の開始実績が存在しません。 LoaderMagazineNo:{0}", lMag.MagazineNo));
				}
				AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
				OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				if (this.LogOutputDirectoryPath != null)
				{
					List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, lMag.WorkStart.Value, System.DateTime.Now);
					foreach (string lotFile in lotFiles)
					{
						MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
						OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
					}
				}

				if (this.Enqueue(lMag, Station.Unloader))
				{
					this.Dequeue(Station.Loader);
					Plc.SetBit(this.WorkCompleteOKBitAddress, 1, Common.BIT_ON);
					OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
				}
			}
			catch (Exception)
			{
				Plc.SetBit(this.WorkCompleteNGBitAddress, 1, Common.BIT_ON);
				throw;
			}
		}

		public override bool IsRequireOutput()
		{
			if (this.LogOutputDirectoryPath == null)
			{
				// 装置に傾向監視の仕組みが全装置展開されるまでの旧仕様
				if (base.IsRequireOutput() == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else 
			{
				bool isRequire = base.IsRequireOutput();
				if (isRequire)
				{
					if (isFishishedFileOutput(isRequire))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else 
				{
					return false;
				}
			}
		}

		/// <summary>
		/// 先頭仮想マガジンの開始時間後の名称変更前finファイルの出力があればTrue
		/// </summary>
		/// <returns></returns>
		private bool isFishishedFileOutput(bool isRequireOutput)
		{
			if (isRequireOutput == false) 
			{
				throw new ApplicationException("この関数は排出信号がOFFの場合、呼び出しできません。");
			}

			if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true))
			{
				VirtualMag lMag = this.Peek(Station.Loader);
				if (lMag == null)
				{
					throw new ApplicationException(string.Format(
						"ローダー側の仮想マガジンが無い状態で傾向管理ファイルが存在します。 除去作業後、監視を再開して下さい。 対象フォルダ:{1}", this.LogOutputDirectoryPath));
				}

				if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true, lMag.WorkStart.Value))
				{
					return true;
				}
				else
				{
					throw new ApplicationException(string.Format(
						"ローダー側の先頭マガジンより古い傾向管理ファイルが存在します。 除去作業後、監視を再開して下さい。先頭マガジン開始時間:{0} 対象フォルダ:{1}", lMag.WorkStart.Value, this.LogOutputDirectoryPath));
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 作業開始 ローダーの先頭マガジンに作業開始時間を記録
		/// </summary>
		private void checkWorkStart()
		{
			VirtualMag lMagazine = this.Peek(Station.Loader);
			if (lMagazine == null)
			{
				return;
			}

			if (lMagazine.StartWafer.HasValue == true && lMagazine.StartWafer != 0)
			{
				//すでに開始段数が有る場合は何もしない
				return;
			}

			//開始時間を記録
			VirtualMag ulMag = VirtualMag.GetLastTailMagazine(this.MacNo, Station.Unloader);
			if (ulMag == null)
			{
				lMagazine.WorkStart = DateTime.Now;
				OutputSysLog(string.Format("[開始時間記録] Unloader側に仮想マガジン無しの為、現在時刻:{0}をLoader側マガジンに記録", lMagazine.WorkStart));
			}
			else
			{
				//1つ前のマガジンで出力されたLファイルの更新日時を完了日時として使用した場合、
				//それを次マガジンの開始時間として使う事で正確な開始時間が得られる
				lMagazine.WorkStart = ulMag.WorkComplete;
				OutputSysLog(string.Format("[開始時間記録] Unloader側の最後尾仮想マガジン完了時刻:{0}をLoader側マガジンに記録", lMagazine.WorkStart));
			}

			//開始時のウェハー段数を仮に記録
			//この記録は開始時間を二度更新しない為のフラグにだけ用いて、ULの仮想マガジンには転記しない
			lMagazine.StartWafer = 1;

			//実仮想マガジン更新
			lMagazine.Updatequeue();
		}
	}
}
