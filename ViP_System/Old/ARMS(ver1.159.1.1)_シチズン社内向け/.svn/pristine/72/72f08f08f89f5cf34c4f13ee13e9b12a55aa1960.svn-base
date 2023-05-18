using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// SLS1 ブレイク機
	/// </summary>
	public class Break : CifsMachineBase
	{
		protected override void concreteThreadWork()
		{
			try
			{
                //if (this.IsRequireOutput() == true)
                //{
                if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath))
                {
                    workCompletehigh();
                }
				//}
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
            OutputSysLog(string.Format("[完了処理] 開始"));

            //List<MachineLog.FinishedFile> finList = MachineLog.FinishedFile.GetAllFiles(this.LogOutputDirectoryPath);
            //foreach (MachineLog.FinishedFile fin in finList)
            //{
            //	OutputSysLog(string.Format("[完了処理] Finファイル取得成功 FileName:{0}", fin.FullName));

            //	CutBlend[] cutBlendList = CutBlend.GetData(this.MacNo, fin.WorkStartDt.Value, fin.WorkEndDt.Value);
            //	if (cutBlendList.Count() == 0) 
            //	{
            //		throw new ApplicationException(string.Format("装置のブレンド実績が存在しません。 開始日時が{0}以前 完了日時が{1}以降の実績", fin.WorkStartDt, fin.WorkEndDt));
            //	}
            //	CutBlend cutBlendLot = cutBlendList.First();
            //	OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", cutBlendLot.BlendLotNo));

            //	AsmLot asmLot = AsmLot.GetAsmLot(cutBlendLot.LotNo);
            //	Order order = Order.GetMachineOrder(this.MacNo, asmLot.NascaLotNo);
            //	if (order == null) 
            //	{
            //		throw new ApplicationException(string.Format("装置の実績が存在しません。 MacNo:{0} LotNo:{1}", this.MacNo, asmLot.NascaLotNo));
            //	}

            //	List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, fin.WorkStartDt.Value, fin.LastUpdDt.Value);
            //	foreach (string lotFile in lotFiles)
            //	{
            //		if (MachineLog.IsLotFromFileName(lotFile)) continue;

            //		MachineLog.ChangeFileName(lotFile, cutBlendLot.BlendLotNo, asmLot.TypeCd, order.ProcNo, cutBlendLot.BlendLotNo);
            //		OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
            //	}
            //}

            //OutputSysLog(string.Format("[完了処理] 完了"));

            VirtualMag lMag = this.Peek(Station.Loader);
            if (lMag == null)
            {
                throw new ApplicationException("ローダー側に仮想マガジンが存在しないため、完了処理できません。");
            }
            OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

            List<MachineLog.FinishedFile> finList = MachineLog.FinishedFile.GetAllFiles(this.LogOutputDirectoryPath);
            foreach (MachineLog.FinishedFile fin in finList)
            {
                OutputSysLog(string.Format("[完了処理] Finファイル取得成功 FileName:{0}", fin.FullName));

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

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
                }

                if (fin.WorkStartDt.HasValue == false)
                {
                    fin.WorkStartDt = lMag.WorkStart.Value;
                    OutputSysLog(string.Format("[完了処理] finファイル内の開始日時が取得できなかった為、仮想マガジンの開始日時を代入しました。"));
                }

                if (fin.WorkEndDt.HasValue == false)
                {
                    fin.WorkEndDt = System.DateTime.Now;
                    OutputSysLog(string.Format("[完了処理] finファイル内の完了日時が取得できなかった為、現在時刻を代入しました。"));
                }

                lMag.LastMagazineNo = lMag.MagazineNo;

                lMag.WorkStart = fin.WorkStartDt;
                lMag.WorkComplete = fin.WorkEndDt;

                if (this.Enqueue(lMag, Station.Unloader))
                {
                    this.Dequeue(Station.Loader);
                }
            }

            OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));

        }
    }
}
