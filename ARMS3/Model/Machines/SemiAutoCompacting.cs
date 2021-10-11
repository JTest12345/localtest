using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 圧縮成型機(セミオート)
	/// </summary>
	public class SemiAutoCompacting : CifsMachineBase
	{
		protected override void concreteThreadWork()
		{
			try
			{
				workStart();

				workComplete();
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

		private void workStart() 
		{
			VirtualMag mag = this.Peek(Station.Loader);
			if (mag == null)
			{
				return;
			}

			mag.LastMagazineNo = mag.MagazineNo;

			SendInFile(this.LogInputDirectoryPath, mag.WorkStart.Value);

			if (this.Enqueue(mag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);
			}
		}

		private void workComplete() 
		{
			VirtualMag mag = this.Peek(Station.EmptyMagazineUnloader);
			if (mag == null)
			{
				return;
			}

			// finファイルの出力が無い場合、要求する
			if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath) == false)
			{
				SendOutFile(this.LogOutputDirectoryPath, mag.WorkStart.Value);
			}
			
			// finファイル出力まで待機する
			int retryCt = 0;
			while (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath) == false)
			{
				if (retryCt == 10)
					throw new ApplicationException("finファイル要求後、一定時間経過しましたがファイルが出力されません。");
				
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
			}

			MachineLog.FinishedFile fin = MachineLog.FinishedFile.GetLastFile(this.LogOutputDirectoryPath);

			int procno = Order.GetLastProcNo(this.MacNo, mag.MagazineNo);

			Order order = Order.GetLastCompOrderInMachine(this.MacNo, procno, DateTime.Now);
			if (order == null)
			{
				throw new ApplicationException(string.Format("装置の実績が存在しません。 MacNo:{0} ProcNo:{1}", this.MacNo, procno));
			}
			AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, order.WorkStartDt, fin.LastUpdDt.Value);
			foreach (string lotFile in lotFiles)
			{
				MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, mag.MagazineNo);
				OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
			}

			this.Dequeue(Station.EmptyMagazineUnloader);
		}
	}
}
