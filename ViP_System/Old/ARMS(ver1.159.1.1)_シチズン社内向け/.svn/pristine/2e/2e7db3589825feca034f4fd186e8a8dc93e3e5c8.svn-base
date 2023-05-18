using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// ロットマーキング機　樹脂基板タイプ(MPL/38/385モデル)
	/// </summary>
	public class LotMarking2 : LotMarking
	{
		protected override void concreteThreadWork()
		{
			try 
			{
				if (base.IsRequireOutput() == true)
				{
					this.WorkCompleteHigh();
				}

				if (this.IsWorkStartAutoComplete)
				{
					if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
					{
						workStart();
					}
				}
			}
			catch (Exception ex)
			{
				FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
				frmErr.ShowDialog();

				if (frmErr.Method == ErrorHandleMethod.None)
				{
					throw new Exception(ex.Message, ex);
				}
			}
		}

		public override void WorkCompleteHigh()
		{
			try
			{
				VirtualMag ulMag = new VirtualMag();

				string magno = Plc.GetMagazineNo(ULMagazineAddress);
				if (string.IsNullOrEmpty(magno) == false)
				{
					ulMag.MagazineNo = magno;
					ulMag.LastMagazineNo = magno;
				}
				else
				{
					throw new ApplicationException("[完了登録異常] 排出マガジンNOの取得に失敗。\n排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
				}

				OutputSysLog(string.Format("[完了処理] 開始 UnLoaderMagazineNo:{0}", magno));

				Order startOrder = Order.GetMachineOrder(this.MacNo, magno);
				if (startOrder == null)
				{
					throw new ApplicationException(string.Format("作業の開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。LotNo:{0}", magno));
				}
				ulMag.WorkStart = startOrder.WorkStartDt;

				//try
				//{
				//	ulMag.WorkStart = this.Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				//}
				//catch (Exception)
				//{
				//	throw new ApplicationException(string.Format("ロットマーキング機から開始時間の取得に失敗 MagazinNo:{0}", ulMag.MagazineNo));
				//}

				try
				{
					ulMag.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				}
				catch (Exception)
				{
					throw new ApplicationException(string.Format("ロットマーキング機から完了時間の取得に失敗 MagazinNo:{0}", ulMag.MagazineNo));
				}

				//作業IDを取得
				int procno = Order.GetLastProcNo(this.MacNo, ulMag.MagazineNo);
				ulMag.ProcNo = procno;          

				if (this.Enqueue(ulMag, Station.Unloader))
				{
					Plc.SetBit(this.WorkCompleteOKBitAddress, 1, Common.BIT_ON);
					OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", ulMag.MagazineNo));
				}
			}
			catch (Exception)
			{
				Plc.SetBit(this.WorkCompleteNGBitAddress, 1, Common.BIT_ON);
				throw;
			}
		}

        private void workStart()
        {
            VirtualMag mag = new VirtualMag();

            string magno = Plc.GetMagazineNo(LMagazineAddress);
            if (string.IsNullOrEmpty(magno) == false)
            {
                mag.MagazineNo = magno;
                mag.LastMagazineNo = magno;
            }
            else
            {
                throw new ApplicationException("[開始登録異常] 搬入マガジンNOの取得に失敗。\n搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
            }

            OutputSysLog(string.Format("[開始処理] 開始 LoaderMagazineNo:{0}", magno));

            Magazine svrmag = Magazine.GetCurrent(magno);
            if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

            mag.ProcNo = nextproc.ProcNo;
            //mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            mag.WorkStart = DateTime.Now;

            Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

            ArmsApiResponse workResponse = CommonApi.WorkStart(order);
            if (workResponse.IsError)
            {
                Plc.SetBit(this.WorkStartNGBitAddress, 1, PLC.Common.BIT_ON);
                throw new ApplicationException(
                    string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
            }
            else
            {
                // ﾁｯﾌﾟﾀｲﾌﾟを想定
                Plc.SetBit(this.WorkStartOKBitAddress, 1, PLC.Common.BIT_ON);
                OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", magno));
            }
        }
    }
}
