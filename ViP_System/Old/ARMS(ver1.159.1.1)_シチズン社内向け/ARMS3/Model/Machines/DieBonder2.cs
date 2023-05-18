using ARMS3.Model.PLC;
using ArmsApi;
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
	/// ダイボンダー AD8930などウェハーチェンジャー無しの装置を想定
	/// ※現状高効率のみ機能実装
	/// </summary>
	public class DieBonder2 : DieBonder
	{
		/// <summary>
        /// メインルーチン
        /// </summary>
		protected override void concreteThreadWork()
		{
			try
			{
				// 作業完了
				if (base.IsRequireOutput() == true)
				{
					//前マガジンの排出を待っている場合は次の完了処理を行わない
					if (IsWaitingMagazineTakeout == false)
					{
						workCompletehigh();
					}

					if (IsWaitingMagazineTakeout)
					{
						//完了位置からマガジンが取り除かれたかのチェック
						if (base.Plc.GetBit(MagazineTakeoutBitAddress, 1) == Omron.BIT_ON)
						{
							//アンローダーが動作したかのチェック
							if (base.Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Omron.BIT_ON)
							{
								IsWaitingMagazineTakeout = false;
								base.Plc.SetBit(MagazineTakeoutBitAddress, 1, Omron.BIT_OFF);
								base.Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Omron.BIT_OFF);
							}
						}
					}
				}

				//作業開始
				checkWorkStart();

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);

				//仮想マガジン消去要求応答
				ResponseClearMagazineRequest();
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

		/// <summary>
		/// 高生産性 作業完了
		/// </summary>
		public void workCompletehigh()
		{
			VirtualMag oldmag = this.Peek(Station.Loader);
			if (oldmag == null)
			{
				return;
			}
			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldmag.MagazineNo));

			//前マガジンの取り除きフラグが残っている場合は削除
			this.Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);

			oldmag.LastMagazineNo = oldmag.MagazineNo;
			oldmag.WorkComplete = DateTime.Now;
			oldmag.StartWafer = 0;
			oldmag.EndWafer = 0;

			Magazine svrMag = Magazine.GetCurrent(oldmag.MagazineNo);
			AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			if (this.LogOutputDirectoryPath != null)
			{
				string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
				if (string.IsNullOrEmpty(lFile))
					throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

				List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, oldmag.WorkStart.Value, File.GetLastWriteTime(lFile));
				if (lotFiles.Count == 0)
					throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", oldmag.WorkStart.Value, oldmag.WorkComplete.Value));

				foreach (string lotFile in lotFiles)
				{
					if (IsStartLogFile(lotFile))
						continue;

					MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, oldmag.ProcNo.Value, oldmag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}
			}

			if (this.Enqueue(oldmag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);

				IsWaitingMagazineTakeout = true;

				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", oldmag.MagazineNo));
			}
		}

		/// <summary>
		/// 作業開始 ローダーの先頭マガジンに作業開始時間とウェハー段数を記録
		/// </summary>
		public void checkWorkStart()
		{
			VirtualMag lMagazine = this.Peek(Station.Loader);
			if (lMagazine == null)
			{
				return;
			}
			
			if (lMagazine.StartWafer.HasValue == true)
			{
				//すでに開始段数が有る場合は何もしない
				return;
			}
			lMagazine.StartWafer = 0;
			lMagazine.WaferChangerChangeCount = 0;

			//開始時間を記録
			VirtualMag ulMag = VirtualMag.GetLastTailMagazine(this.MacNo, Station.Unloader);
			if (ulMag == null)
			{
				lMagazine.WorkStart = DateTime.Now;
			}
			else
			{
				//1つ前のマガジンで出力されたLファイルの更新日時を完了日時として使用した場合、
				//それを次マガジンの開始時間として使う事で正確な開始時間が得られる
				lMagazine.WorkStart = ulMag.WorkComplete;
			}

			//実仮想マガジン更新
			lMagazine.Updatequeue();
		}
	}
}
