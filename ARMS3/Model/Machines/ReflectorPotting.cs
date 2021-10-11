using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 反射材ポッティング(内製)
	/// </summary>
	public class ReflectorPotting : MachineBase
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
					workCompletehigh();
				}
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

		private void workCompletehigh()
		{
			try
			{
				VirtualMag lMag = this.Peek(Station.Loader);
				if (lMag == null)
				{
					return;
				}
				OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

				lMag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				lMag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);

				Order order = Order.GetMachineStartOrder(this.MacNo);
				if (order == null)
				{
					throw new ApplicationException(string.Format("装置の開始実績が存在しません。 LoaderMagazineNo:{0}", lMag.MagazineNo));
				}
				AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
				OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, lMag.WorkStart.Value, System.DateTime.Now);
				foreach (string lotFile in lotFiles) 
				{
					MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}

				lMag.LastMagazineNo = lMag.MagazineNo;

				if (this.Enqueue(lMag, Station.Unloader))
				{
					this.Dequeue(Station.Loader);
					Plc.SetBit(this.WorkCompleteOKBitAddress, 1, PLC.Common.BIT_ON);
					OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
				}
			}
			catch (Exception)
			{
				Plc.SetBit(this.WorkCompleteNGBitAddress, 1, PLC.Common.BIT_ON);
				throw;
			}
		}
	}
}
