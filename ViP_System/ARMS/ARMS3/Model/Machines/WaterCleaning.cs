using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model.DataContext;
using ArmsApi;
using System.IO;

namespace ARMS3.Model.Machines
{
    public class WaterCleaning : MachineBase
    {
        #region PLCアドレス

        /// <summary>
        /// 開始登録要求Bit 1層～5層
        /// </summary>
        private string[] StartReqBitPlcAddressList
        {
            get { return new string[] { "B000AC0", "B000BC0", "B000CC0", "B000DC0", "B000EC0" }; }
        }

        /// <summary>
        /// 開始登録結果Word 1層～5層 (応答内容：OK = 1, NG = 2)
        /// </summary>
        private string[] StartResResultPlcAddressList
        {
            get { return new string[] { "W000AC0", "W000BC0", "W000CC0", "W000DC0", "W000EC0" }; }
        }

        /// <summary>
        /// 開始登録完了応答Bit 1層～5層
        /// </summary>
        private string[] StartResBitPlcAddressList
        {
            get { return new string[] { "B000A30", "B000B30", "B000C30", "B000D30", "B000E30" }; }
        }

        /// <summary>
        /// 完了登録要求Bit 1層～5層
        /// </summary>
        private string[] CompleteReqBitPlcAddressList
        {
            get { return new string[] { "B000AD0", "B000BD0", "B000CD0", "B000DD0", "B000ED0" }; }
        }

        /// <summary>
        /// 完了登録完了応答Bit 1層～5層
        /// </summary>
        private string[] CompleteResBitPlcAddressList
        {
            get { return new string[] { "B000A40", "B000B40", "B000C40", "B000D40", "B000E40" }; }
        }

        /// <summary>
        /// 投入マガジンNo Word 1層～5層
        /// </summary>
        private string[] InputMagazineNoPlcAddressList
        {
            get { return new string[] { "W000A20", "W000B20", "W000C20", "W000D20", "W000E20" }; }
        }

        /// <summary>
        /// 完了マガジンNo Word 1層～5層
        /// </summary>
        private string[] OutputMagazineNoPlcAddressList
        {
            get { return new string[] { "W000A70", "W000B70", "W000C70", "W000D70", "W000E70" }; }
        }

        /// <summary>
        /// 開始日時 1層～5層
        /// </summary>
        private string[] WorkStartDatePlcAddressList
        {
            get { return new string[] { "W000A10", "W000B00", "W000C10", "W000D10", "W000E10" }; }
        }

        /// <summary>
        /// 完了日時 1層～5層
        /// </summary>
        private string[] WorkCompleteDatePlcAddressList
        {
            get { return new string[] { "W000A16", "W000B16", "W000C16", "W000D16", "W000E16" }; }
        }
        #endregion

        /// <summary>
        /// 槽番号
        /// </summary>
        private int? tankNo { get; set; }

        protected override void concreteThreadWork()
        {
            try
            {
                if (tankNo.HasValue == false)
                {
                    this.tankNo = getTankNo(this.PlantCd);
                }
                
                if (this.Plc.GetBit(StartReqBitPlcAddressList[this.tankNo.Value-1]) == PLC.Common.BIT_ON)
                {    
                    workStart(this.tankNo.Value);   
                }
                
                if (isRequireWorkEnd(this.tankNo.Value) == true)
                {
                    workComplete(this.tankNo.Value);
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

        /// <summary>
        /// 開始処理
        /// </summary>
        /// <param name="tankNo"></param>
        private void workStart(int tankNo)
        {
            this.OutputSysLog("[開始処理] 開始");

            try
            {
                string magazineNo = Plc.GetMagazineNo(InputMagazineNoPlcAddressList[tankNo-1], PLC.Common.MAGAZINE_NO_WORD_LENGTH_HIGH);
                if (string.IsNullOrEmpty(magazineNo))
                {
                    throw new ApplicationException("投入マガジンのラベルを読み取れませんでした。再度読み直して下さい。");
                }
                this.OutputApiLog($"投入マガジンNo:{magazineNo}");

                Magazine svrMag = Magazine.GetCurrent(magazineNo);
                if (svrMag == null)
                {
                    throw new ApplicationException($"マガジンNo:{magazineNo}は稼動中マガジンではありません。投入マガジンのラベルと装置で読み取った情報が正しいか確認して下さい。");
                }
                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

                Order o = Order.GetMagazineOrder(svrMag.NascaLotNO, p.ProcNo);
                if (o != null)
                {
                    throw new ApplicationException($"マガジンNo「{magazineNo}」は作業「{p.InlineProNM}」で既に開始実績が存在します。データメンテナンスで確認して下さい。");
                }

                VirtualMag lMag = new VirtualMag();
                lMag = new VirtualMag();
                lMag.MacNo = this.MacNo;
                lMag.WorkStart = System.DateTime.Now;
                lMag.MagazineNo = magazineNo;
                lMag.LastMagazineNo = magazineNo;
                lMag.ProcNo = p.ProcNo;

                if (base.WorkStart(lMag, this) == false)
                {
                    throw new ApplicationException("作業開始登録失敗");
                }

                this.Enqueue(lMag, Station.Loader);
                responseResultOfWorkStartOk(tankNo);
            }
            catch (Exception)
            {
                responseResultOfWorkStartNg(tankNo);
                throw;
            }

            this.OutputSysLog("[開始処理] 完了");
        }

        /// <summary>
        /// 開始登録結果OKを装置へ送信
        /// </summary>
        private void responseResultOfWorkStartOk(int tankNo)
        {
            this.Plc.SetString(StartResResultPlcAddressList[tankNo-1], "1");
            this.Plc.SetBit(StartResBitPlcAddressList[tankNo-1], 1, "1");
            this.OutputApiLog($"{this.Name} << システム 開始登録OK 層:{tankNo}");
        }

        /// <summary>
        /// 開始登録結果NGを装置へ送信
        /// </summary>
        private void responseResultOfWorkStartNg(int tankNo)
        {
            this.Plc.SetString(StartResResultPlcAddressList[tankNo-1], "2");
            this.Plc.SetBit(StartResBitPlcAddressList[tankNo-1], 1, "1");
            this.OutputApiLog($"{this.Name} << システム 開始登録NG 層:{tankNo}");
        }
        
        /// <summary>
        /// 完了処理
        /// </summary>
        /// <param name="tankNo"></param>
        private void workComplete(int tankNo)
        {
            this.OutputSysLog("[完了処理] 開始");

            string magazineNo = Plc.GetMagazineNo(OutputMagazineNoPlcAddressList[tankNo-1], PLC.Common.MAGAZINE_NO_WORD_LENGTH_HIGH);
            if (string.IsNullOrEmpty(magazineNo))
            {
                throw new ApplicationException("完了マガジンのラベルを読み取れませんでした。再度読み直して下さい。");
            }

            Magazine svrMag = Magazine.GetCurrent(magazineNo);
            if (svrMag == null)
            {
                throw new ApplicationException($"マガジンNo:{magazineNo}は稼動中マガジンではありません。完了マガジンのラベルと装置で読み取った情報が正しいか確認して下さい。");
            }

            VirtualMag lMag = this.Peek(Station.Loader);
            if (lMag == null || magazineNo != lMag.MagazineNo)
            {
                throw new ApplicationException(
                    $"投入と完了でマガジンの入れ替わりが発生しています。データメンテナンスで前投入マガジンの実績修正、現投入マガジンの実績登録、仮想マガジンの削除を行って下さい。投入マガジン:{magazineNo} 仮想マガジン:{lMag?.MagazineNo}");
            }
            this.OutputApiLog($"完了マガジンNo:{magazineNo}");

            try
            {
                lMag.WorkStart = this.Plc.GetWordsAsDateTime(WorkStartDatePlcAddressList[tankNo-1]);
            }
            catch (Exception)
            {
                throw new ApplicationException($"開始日時の取得に失敗 MagazinNo:{lMag.MagazineNo}");
            }

            try
            {
                lMag.WorkComplete = this.Plc.GetWordsAsDateTime(WorkCompleteDatePlcAddressList[tankNo-1]);
            }
            catch (Exception)
            {
                throw new ApplicationException($"完了日時の取得に失敗 MagazinNo:{lMag.MagazineNo}");
            }

            lMag.LastMagazineNo = lMag.MagazineNo;

            Order order = Order.GetMachineStartOrder(this.MacNo);
            if (order == null)
            {
                throw new ApplicationException($"作業開始実績が存在しない為、完了登録できません。データメンテナンスで完了登録を行って下さい。MagazineNo:{lMag.MagazineNo}");
            }
            AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
            OutputApiLog($"{ this.Name } [完了処理] ロット存在確認 LotNo:{lot.NascaLotNo}");

            string finFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, MachineLog.FinishedFile.FINISHEDFILE_IDENTITYNAME, true);

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, lMag.WorkStart.Value, File.GetLastWriteTime(finFile));
            foreach (string lotFile in lotFiles)
            {
                MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
                OutputApiLog($"{ this.Name } [完了処理] ロットファイル名称変更 FileName:{lotFile}");
            }

            this.WorkComplete(lMag, this, true);
            this.Dequeue(Station.Loader);

            responseResultOfWorkCompleteOk(tankNo);

            this.OutputSysLog($"[完了処理] 完了");
        }

        /// <summary>
        /// 実績完了登録の要求確認
        /// </summary>
        /// <returns></returns>
        private bool isRequireWorkEnd(int tankNo)
        {
            if (string.IsNullOrEmpty(this.LogOutputDirectoryPath) == true)
            {
                throw new ApplicationException("装置完了時のログ出力先(LogOutputDirectoryPath)が未設定です。");
            }

            if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true) && this.Plc.GetBit(CompleteReqBitPlcAddressList[tankNo - 1]) == PLC.Common.BIT_ON)
            {
                this.OutputApiLog($"{this.Name} >> システム 完了登録要求");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 完了登録完了を装置へ送信
        /// </summary>
        private void responseResultOfWorkCompleteOk(int tankNo)
        {
            this.Plc.SetBit(CompleteResBitPlcAddressList[tankNo-1], 1, "1");
            this.OutputApiLog($"{this.Name} << システム 完了登録完了 層:{tankNo}");
        }
        
        /// <summary>
        /// EICSデータベースから槽番号を取得
        /// </summary>
        /// <param name="plantCd"></param>
        /// <returns></returns>
        private int getTankNo(string plantCd)
        {
            using (EICSDataContext db = new EICSDataContext(Config.Settings.QCILConSTR))
            {
                TmLSET lset = db.TmLSET.SingleOrDefault(l => l.Equipment_NO == plantCd);
                if (lset == null)
                {
                    throw new ApplicationException("槽番号の設定がデータベースマスタで設定されていない為、監視できません。設定場所:QCIL.TmLSET.EquipPart_ID");
                        
                }
                
                // TODO 6(アドレスの配列数)槽以上が取得できた時のエラー処理を追加する

                // TODO 数値かどうかのチェック、0の場合のエラー処理追加

                return int.Parse(lset.EquipPart_ID);
            }
        }
    }
}
