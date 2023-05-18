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
	/// SLS1 フリップチップ装置
	/// ウェハーカセット無しの為、製品出来栄えファイル(**ファイル)から交換回数を取得?
	/// </summary>
	public class FlipChip : CifsMachineBase
	{
        /// <summary>
        /// 排出信号アドレスリスト
        /// </summary>
        public List<string> UnLoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出信号の記憶
        /// </summary>
        public Dictionary<string, bool> UnLoaderReqBitMemory { get; set; }

        private const string MAPPING_FIN_EXT = "fin2";

        public FlipChip()
        {
            UnLoaderReqBitAddressList = new List<string>();
            UnLoaderReqBitMemory = new Dictionary<string, bool>();
        }

        //~FlipChip() 
        //{
        //    UnLoaderReqBitMemory.Clear();
        //}

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

                //trgファイル名をリネーム
                base.WorkStart();

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);

                RenameRelatedMappingFile(this.LogOutputDirectoryPath);
			}
			catch (Exception ex)
			{
				SendStopFile(this.LogInputDirectoryPath, ex.Message);

				FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
				frmErr.ShowDialog();

				UnLoaderReqBitMemory = new Dictionary<string, bool>();

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

                DateTime finLastWriteDt = File.GetLastWriteTime(fin.FullName);

                List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, fin.WorkStartDt.Value, finLastWriteDt);
                foreach (string lotFile in lotFiles)
                {
                    if (MachineLog.IsLotFromFileName(lotFile)) continue;
                    if (Path.GetExtension(lotFile) == ".mpd") continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
                }

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

        public void RenameRelatedMappingFile(string targetDirPath)
        {
            List<string> relatedMappingFiles = MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]" + MAPPING_FIN_EXT);
            relatedMappingFiles.AddRange(MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]mpd"));

            //mpdファイル名からアンダーバー以降削除するようなリネームが必要なければList中のmpdファイルの順番は気にする必要ないが
            //mpdもリネームする為、finファイルよりも先に、見つけた全mpdファイルのリネームを行う SLP2対応:n.yoshi
            List<string> mpdFirstSortFiles = new List<string>();
            mpdFirstSortFiles.AddRange(relatedMappingFiles.FindAll(l => l.EndsWith(".mpd")));

            foreach (string filePath in mpdFirstSortFiles)
            {
                string renamedFileNm = MachineLog.RemoveFileNmAfterUnderBar(filePath);

                if (string.IsNullOrEmpty(renamedFileNm))
                {
                    continue;
                }
            }

            List<MachineLog.FinishedFile6> finList = MachineLog.FinishedFile6.GetAllFiles(targetDirPath);

            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile6 fin in finList)
            {
                OutputSysLog(string.Format("[基板完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                OutputSysLog(string.Format("[基板完了処理] 完了"));
            }
        }
    }
}
