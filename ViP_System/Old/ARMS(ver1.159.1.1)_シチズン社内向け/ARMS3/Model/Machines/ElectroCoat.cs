using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 電着(白)
	/// </summary>
	public class ElectroCoat : MachineBase
	{
		/// <summary>
		/// 槽の有効/無効アドレス
		/// </summary>
		public string TankAvailableBitAddress { get; set; }

		protected override void concreteThreadWork()
		{
			try
			{
				if (base.IsRequireOutput() == true)
				{
					//if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath) == false)
					//{
					//	throw new ApplicationException(string.Format("排出信号ONでfinファイルが存在しません。 DirectoryName:{0}", this.LogOutputDirectoryPath));
					//}

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
			VirtualMag ulMagazine = this.Peek(Station.Unloader);
			if (ulMagazine != null)
			{
				return;
			}

			VirtualMag lMag = this.Peek(Station.Loader);
			if (lMag == null)
			{
				throw new ApplicationException("作業開始登録がされていない為(Loader側の仮想マガジンが存在しない)、Unloader側に仮想マガジンを移動できません。");
			}
			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

			if (lMag.WorkComplete.HasValue == false)
			{
				lMag.WorkComplete = System.DateTime.Now;
				lMag.Updatequeue();
			}

			Order order = Order.GetMachineStartOrder(this.MacNo);
			if (order == null)
			{
				throw new ApplicationException(string.Format("装置の開始実績が存在しません。 LoaderMagazineNo:{0}", lMag.MagazineNo));
			}
			AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			//電着は出来栄えファイル出力無しの為（2015/11/11時点）ファイル取得、名称変更処理はコメントアウト
			//List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, lMag.WorkStart.Value, lMag.WorkComplete.Value);
			//foreach (string lotFile in lotFiles)
			//{
			//	MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
			//	OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
			//}

			lMag.LastMagazineNo = lMag.MagazineNo;

			if (this.Enqueue(lMag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);
			}
			OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
		}

		/// <summary>
		/// 排出信号と有効信号2つを見て両信号がONならTrueを返す
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireOutput()
		{
			bool retv = false;

            string bitdata1;
            string bitdata2;
            try
            {
                bitdata1 = this.Plc.GetBit(this.TankAvailableBitAddress);
                bitdata2 = this.Plc.GetBit(this.UnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{this.TankAvailableBitAddress}』『{this.UnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (bitdata1 == PLC.Common.BIT_ON)
			{
				if (bitdata2 == PLC.Common.BIT_ON)
				{
					retv = true;
				}
			}

			return retv;
		}
	}
}
