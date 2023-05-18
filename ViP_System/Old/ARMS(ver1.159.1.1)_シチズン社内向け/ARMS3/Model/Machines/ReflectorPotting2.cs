using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// 反射材ポッティング(進和製)
	/// </summary>
	public class ReflectorPotting2 : CifsMachineBase
	{
		/// <summary>
        /// 排出信号アドレスリスト
        /// </summary>
        public List<string> UnLoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出信号の記憶
        /// </summary>
        public Dictionary<string, bool> UnLoaderReqBitMemory { get; set; }

		public ReflectorPotting2()
        {
            UnLoaderReqBitAddressList = new List<string>();
            UnLoaderReqBitMemory = new Dictionary<string, bool>();
        }

		protected override void concreteThreadWork()
		{
			try
			{
				if (this.IsRequireOutput() == true)
				{
					if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath) == false)
					{
						throw new ApplicationException(string.Format("排出信号ONでfinファイルが存在しません。 DirectoryName:{0}", this.LogOutputDirectoryPath));
					}

					workCompletehigh();
				}
			}
			catch (Exception ex)
			{
				SendStopFile(this.LogInputDirectoryPath, ex.Message);

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
			List<MachineLog.FinishedFile> finList = MachineLog.FinishedFile.GetAllFiles(this.LogOutputDirectoryPath);
			foreach (MachineLog.FinishedFile fin in finList)
			{
				OutputSysLog(string.Format("[完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

				VirtualMag lMag = this.Peek(Station.Loader);
				if (lMag == null)
				{
					throw new ApplicationException("ローダー側に仮想マガジンが存在しないため、完了処理できません。");
				}
				OutputSysLog(string.Format("[完了処理] LoaderMagazineNo:{0}", lMag.MagazineNo));

				Order order = Order.GetMachineStartOrder(this.MacNo);
				if (order == null)
				{
					throw new ApplicationException(string.Format("装置の開始実績が存在しません。 LoaderMagazineNo:{0}", lMag.MagazineNo));
				}
				AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
				OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, fin.WorkStartDt.Value, fin.LastUpdDt.Value);
				foreach (string lotFile in lotFiles)
				{
					if (MachineLog.IsLotFromFileName(lotFile)) continue;

					//finファイルは最後に名称変更しないとEICSが完了フォルダに移動してしまう為
					if (lotFile == fin.FullName) continue;

					MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}
				MachineLog.ChangeFileName(fin.FullName, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
				OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", fin.FullName));

				lMag.LastMagazineNo = lMag.MagazineNo;

				lMag.WorkStart = fin.WorkStartDt;
				lMag.WorkComplete = fin.WorkEndDt;

				if (this.Enqueue(lMag, Station.Unloader))
				{
					this.Dequeue(Station.Loader);
				}
				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
			}
		}

		/// <summary>
		/// 排出信号2つをチェックして変化点があればTrueを返す
		/// </summary>
		/// <returns></returns>
		public override bool IsRequireOutput()
		{
			Dictionary<string, bool> unloaderReqBitList = new Dictionary<string, bool>();
			foreach (string bitAddress in this.UnLoaderReqBitAddressList)
			{
                string bitdata;
                try
                {
                    bitdata = this.Plc.GetBit(bitAddress);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{bitAddress}』, エラー内容：{ex.Message}");
                    return false;
                }
                if (bitdata == PLC.Common.BIT_ON)
                //if (this.Plc.GetBit(bitAddress) == PLC.Common.BIT_ON)
                {
                    unloaderReqBitList.Add(bitAddress, true);
				}
				else
				{
					unloaderReqBitList.Add(bitAddress, false);
				}
			}

			if (UnLoaderReqBitMemory.Count == 0)
			{
				if (unloaderReqBitList.Where(b => b.Value == true).Count() >= 1)
				{
					// 記憶が無ければ変化点としてTrueを返す
					UnLoaderReqBitMemory = unloaderReqBitList;
					OutputSysLog(string.Format("[排出信号変化時記憶] 起動時:{0}", string.Join(",", UnLoaderReqBitMemory.Values)));
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				// 記憶と取得値に変化点がある、排出ONの数が増加した場合はTrueを返す
				// ※変化点は必ずメモリ記憶する
				foreach (KeyValuePair<string, bool> bit in UnLoaderReqBitMemory)
				{
					if (unloaderReqBitList[bit.Key] != bit.Value)
					{
						if (UnLoaderReqBitMemory.Where(b => b.Value == true).Count() <= unloaderReqBitList.Where(b => b.Value == true).Count())
						{
							UnLoaderReqBitMemory = unloaderReqBitList;
							OutputSysLog(string.Format("[排出信号変化時記憶] 排出ON増加時:{0}", string.Join(",", UnLoaderReqBitMemory.Values)));
							return true;
						}
						else
						{
							UnLoaderReqBitMemory = unloaderReqBitList;
							OutputSysLog(string.Format("[排出信号変化時記憶] 排出ON減少時:{0}", string.Join(",", UnLoaderReqBitMemory.Values)));
							return false;
						}
					}
				}
				return false;
			}
		}
	}
}
