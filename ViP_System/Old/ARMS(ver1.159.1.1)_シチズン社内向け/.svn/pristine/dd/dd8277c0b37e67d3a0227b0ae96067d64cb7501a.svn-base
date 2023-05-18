using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// ダイボンダー AD838Lのバッファを利用するタイプ
	/// 排出信号が使えない為、出来栄えファイルの出力をトリガに完了処理をする
	/// </summary>
	public class DieBonder4 : DieBonder
	{
        /// <summary>
        /// AD838L固有の不良取得方法ON/OFF(H,IファイルからBM免責、樹脂少量取得)
        /// </summary>
        public bool IsUniqueDefectImport { get; set; }

        protected override void concreteThreadWork()
		{
			try
			{
				//ウェハーチェンジャーの交換監視
				if (Plc.GetBit(WaferChangerChangeSensorBitAddress) == PLC.Common.BIT_ON)
				{
					WaferChangerChange();
				}

				//作業完了
				if (this.IsRequireOutput() == true)
				{
					WorkCompleteHigh();
				}

				//作業開始
				CheckWorkStart();

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
					throw;
				}
			}
		}

		/// <summary>
		/// 高生産性完了処理　基底ダイボンダークラスと違って、Lファイルの更新日時を作業完了日時とする仕様
		/// </summary>
		public override void WorkCompleteHigh()
		{
			VirtualMag oldmag = this.Peek(Station.Loader);
			if (oldmag == null)
			{
				return;
			}

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldmag.MagazineNo));

			//前マガジンの取り除きフラグが残っている場合は削除
			this.Plc.SetBit(MagazineTakeoutBitAddress, 1, PLC.Common.BIT_OFF);

			oldmag.LastMagazineNo = oldmag.MagazineNo;
			oldmag.WorkComplete = DateTime.Now;

			//終了時ウェハー段数を取得
			oldmag.EndWafer = this.GetWaferPlateNo();
			if (oldmag.StartWafer == null)
			{
				throw new Exception("開始ウェハー段数が取得できていません。");
			}
			Log.ApiLog.Info("終了ウェハー段数を設定:" + oldmag.EndWafer.ToString());

			Magazine svrMag = Magazine.GetCurrent(oldmag.MagazineNo);
			AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			if (this.LogOutputDirectoryPath != null)
			{
				string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
				if (string.IsNullOrEmpty(lFile))
					throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

				oldmag.WorkComplete = File.GetLastWriteTime(lFile);

                List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, oldmag.WorkStart.Value, File.GetLastWriteTime(lFile));
                if (lotFiles.Count == 0)
					throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", oldmag.WorkStart.Value, oldmag.WorkComplete.Value));

                if (this.IsUniqueDefectImport)
                {
                    string hFile = GetHLogFile(lotFiles);
                    if (string.IsNullOrEmpty(hFile))
                        throw new ApplicationException("Hファイルが存在しません。ファイルを除去後、手動で実績入力を行って下さい。");
                 
                    int badMarkCt = getBadMarkDefect(hFile, lot.NascaLotNo);

                    string iFile = GetILogFile(lotFiles);
                    if (string.IsNullOrEmpty(iFile))
                        throw new ApplicationException("Iファイルが存在しません。ファイルを除去後、手動で実績入力を行って下さい。");

                    int resinDefCt = getResinDefect(iFile, badMarkCt, lot.NascaLotNo);

                    setDiebondDefect(lot, badMarkCt, resinDefCt);
                }

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

		public override bool IsRequireOutput()
		{
			if (IsReady() && IsFishishedFileOutput())
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// HファイルからBM免責取得
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public int getBadMarkDefect(string filePath, string lotNo)
        {
            return getBadMarkDefect(filePath, lotNo, 0);
        }
        private int getBadMarkDefect(string filePath, string lotNo, int retryCt)
        {
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException("Hファイルがロック中の為、BM免責の集計ができません。ファイルロック解除後、再開して下さい。");
            }

            List<string> lineDataList = new List<string>();
            try
            {
                lineDataList = File.ReadAllLines(filePath).ToList();
            }
            catch (IOException)
            {
                retryCt = retryCt + 1;
                System.Threading.Thread.Sleep(1000);
                return getBadMarkDefect(filePath, lotNo, retryCt);
            }

            try
            {
                if (lineDataList.Count == 0)
                {
                    throw new ApplicationException(
                        "Hファイル内が空だった為、BM免責を0で登録しました。");
                }                
                lineDataList = lineDataList.Where(l => Regex.IsMatch(l, "^.*,Skipped.*$")).ToList();
            
                return lineDataList.Count;
            }
            catch (ApplicationException err)
            {
                AsmLot lot = AsmLot.GetAsmLot(lotNo);
                lot.IsWarning = true;
                lot.Update();

                AsmLot.InsertLotLog(lotNo, System.DateTime.Now,
                    string.Format(err.Message + " ファイルパス:{0}", filePath), 0, string.Empty, true, this.LineNo);

                return 0;
            }
        }

        /// <summary>
        /// IファイルからBM免責+樹脂少量を抜き出し、引数のBM免責を差し引いて樹脂少量を算出
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="badMarkCt"></param>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        public int getResinDefect(string filePath, int badMarkCt, string lotNo)
        {
            return getResinDefect(filePath, badMarkCt, lotNo, 0);
        }
        private int getResinDefect(string filePath, int badMarkCt, string lotNo, int retryCt)
        {
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException("Iファイルがロック中の為、樹脂少量の集計ができません。ファイルロック解除後、再開して下さい。");
            }

            List<string> lineDataList = new List<string>();
            try
            {
                lineDataList = File.ReadAllLines(filePath).ToList();
            }
            catch (IOException)
            {
                retryCt = retryCt + 1;
                System.Threading.Thread.Sleep(1000);
                return getResinDefect(filePath, badMarkCt, lotNo, retryCt);
            }

            try
            {
                if (lineDataList.Count == 0)
                {
                    throw new ApplicationException("Iファイル内が空だった為、樹脂少量を0で登録しました。");
                }

                AsmLot lot = AsmLot.GetAsmLot(lotNo);
                int diceCt = AsmLot.GetLedDiceCount(lot.TypeCd);

                double remainderCt = Math.IEEERemainder(lineDataList.Count, diceCt);
                if (remainderCt != 0)
                {
                    // Iファイル内データ数÷チップ数で除算できなければ、ファイル内のデータ数が異常
                    throw new ApplicationException(string.Format("Iファイルのデータ数:{0}がチップ1個分の数量に換算できない為、樹脂少量を0で登録しました。データメンテナンスで正しい樹脂少量数を入力してください。製品型番:{1}のチップ搭載数:{2}", lineDataList.Count, lot.TypeCd, diceCt));
                }
                int dataCt = lineDataList.Count / diceCt;

				Profile prof = Profile.GetProfile(lot.ProfileId);
				if (prof.LotSize != dataCt)
				{
					throw new ApplicationException(
						string.Format("指図数量:{0}よりIファイル内のデータ数:{1}が少ないため、樹脂少量を集計する事ができませんでした。", lot.LotSize, lineDataList.Count));
				}

				lineDataList = lineDataList.Where(l => Regex.IsMatch(l, "^.*,Skipped.*$")).ToList();

                remainderCt = Math.IEEERemainder(lineDataList.Count, diceCt);
                if (remainderCt != 0)
                {
                    // (BM免責+樹脂少量)÷チップ数で除算できなければ、ファイル内の不良数が異常
                    throw new ApplicationException(string.Format("BM免責+樹脂少量:{0}がチップ1個分の数量に換算できない為、樹脂少量を集計する事ができませんでした。製品型番:{1}のチップ搭載数:{2}", lineDataList.Count, lot.TypeCd, diceCt));
                }
                int dbDefectCt = lineDataList.Count / diceCt;

				if (dbDefectCt < badMarkCt)
				{
					// BM免責+樹脂少量より樹脂不良が多いのは異常
					throw new ApplicationException("Iファイル不良(BM免責+樹脂少量)よりHファイル不良(樹脂少量)が多かった為、樹脂少量を集計する事ができませんでした。");
				}

                return dbDefectCt - badMarkCt;
            }
            catch (ApplicationException err)
            {
                AsmLot lot = AsmLot.GetAsmLot(lotNo);
                lot.IsWarning = true;
                lot.Update();

                AsmLot.InsertLotLog(lotNo, System.DateTime.Now,
                    string.Format(err.Message + " ファイルパス:{0}", filePath), 0, string.Empty, true, this.LineNo);

                return 0;
            }
}

        /// <summary>
        /// ダイボンド不良登録(BM免責、樹脂少量)
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="resinDefectCt"></param>
        /// <param name="badmartCt"></param>
        public void setDiebondDefect(AsmLot lot, int badMarkCt, int resinDefCt)
        {
            if (badMarkCt == 0 && resinDefCt == 0)
                return;

            int procNo = Order.GetLastProcNoFromLot(this.MacNo, lot.NascaLotNo);
            
            Defect defect = Defect.GetDefect(lot.NascaLotNo, procNo);

            defect.DefItems = Defect.GetAllDefect(lot, procNo).ToList();
            if (defect.DefItems.Count == 0)
            {
                throw new ApplicationException("不良マスタが1件も登録されていません。"); 
            }

            int index = defect.DefItems.FindIndex(d => d.DefectCd == Defect.DB_RESIN_DEFECT_CD);
            if (index == -1)
            {
                throw new ApplicationException("樹脂少量が不良マスタに登録されていません。");
            }
            defect.DefItems[index].DefectCt = resinDefCt;

            index = defect.DefItems.FindIndex(d => d.DefectCd == Defect.DB_BADMARK_DEFECT_CD);
            if (index == -1)
            {
                throw new ApplicationException("BM免責が不良マスタに登録されていません。");
            }
            defect.DefItems[index].DefectCt = badMarkCt;

            defect.DeleteInsert();
            OutputSysLog(string.Format("[完了処理] 不良登録完了 LotNo:{0} BM免責:{1} 樹脂少量:{2}", lot.NascaLotNo, badMarkCt, resinDefCt));
        }
    }
}
