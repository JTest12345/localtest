using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    public class Deburring : MachineBase
    {
        private string REQUIRE_WORKSTART_ADDRESS = "B000B10";

        private string RESPONSE_WORKSTART_RESULT_ADDRESS = "W000A10";

        private string RESPONSE_WORKSTART_RESULT_COMPLETE_ADDRESS = "B000A10";

        private string LOADER_MAGAZINENO_ADDRESS = "W000B00";

        private string UNLOADER_MAGAZINENO_ADDRESS = "W000B30";

        protected override void concreteThreadWork()
        {
            try
            {
                if (isRequireWorkEnd())
                {
                    workEnd();
                }

                if (isRequireWorkStart())
                {
                    workStart();
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
        /// 実績開始登録の要求確認
        /// </summary>
        /// <returns></returns>
        private bool isRequireWorkStart()
        {
            if (Plc.GetBit(REQUIRE_WORKSTART_ADDRESS) == PLC.Common.BIT_ON)
            {
                this.OutputApiLog($"{this.Name} >> システム 開始登録要求");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 実績完了登録の要求確認
        /// </summary>
        /// <returns></returns>
        private bool isRequireWorkEnd()
        {
            if (string.IsNullOrEmpty(this.LogOutputDirectoryPath) == true)
            {
                throw new ApplicationException("装置完了時のログ出力先(LogOutputDirectoryPath)が未設定です。");
            }

            if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true))
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
        /// 開始登録
        /// </summary>
        /// <returns></returns>
        private void workStart()
        {
            try
            {
                string magazineNo = Plc.GetMagazineNo(LOADER_MAGAZINENO_ADDRESS, PLC.Common.MAGAZINE_NO_WORD_LENGTH_HIGH);
                if (string.IsNullOrEmpty(magazineNo))
                {
                    throw new ApplicationException("投入マガジンのラベルを読み取れませんでした。再度読み直して下さい。");
                }
                this.OutputApiLog($"読込マガジンNo:{magazineNo}");

                Magazine svrMag = Magazine.GetCurrent(magazineNo);
                if (svrMag == null)
                {
                    throw new ApplicationException($"マガジンNo:{magazineNo}は稼動中マガジンではありません。投入マガジンのラベルと装置で読み取った情報が正しいか確認して下さい。");
                }
                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

                List<VirtualMag> lMags = this.GetMagazines(Station.Loader);
                if (lMags.Exists(l => l.MagazineNo == magazineNo) == true)
                {
                    // ローダー仮想マガジンに読み込んだマガジンが有る場合、同ロットの2缶目の読み込みと判断し、開始登録しない
                    return;
                }

                Order o = Order.GetMagazineOrder(svrMag.NascaLotNO, p.ProcNo);
                if (o != null)
                {
                    if (o.MacNo != this.MacNo)
                    {
                        throw new ApplicationException($"ロット：{svrMag.NascaLotNO}は別装置：{this.Name}で開始実績が有るか、完了工程が異なる為、データメンテナンスで削除後、再投入して下さい。");
                    }

                    // 実績登録が有る場合、同ロットの2缶目の読み込みと判断し、開始登録しない
                    responseResultOfWorkStartOk();

                    return;
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
                responseResultOfWorkStartOk();
            }
            catch (Exception)
            {
                responseResultOfWorkStartNg();
                throw;
            }
        }

        /// <summary>
        /// 完了登録
        /// </summary>
        private void workEnd()
        {
            string magazineNo = Plc.GetMagazineNo(UNLOADER_MAGAZINENO_ADDRESS, PLC.Common.MAGAZINE_NO_WORD_LENGTH_HIGH);
            if (string.IsNullOrEmpty(magazineNo))
            {
                throw new ApplicationException("完了マガジンのラベルを読み取れませんでした。再度読み直して下さい。");
            }

            Magazine svrMag = Magazine.GetCurrent(magazineNo);
            if (svrMag == null)
            {
                throw new ApplicationException($"マガジンNo:{magazineNo}は稼動中マガジンではありません。投入マガジンのラベルと装置で読み取った情報が正しいか確認して下さい。");
            }

            VirtualMag lMag = null;

            VirtualMag ulMag = this.GetMagazines(Station.Unloader).Where(m => m.MagazineNo == magazineNo).SingleOrDefault();
            if (ulMag == null)
            {
                lMag = this.Peek(Station.Loader);
                if (lMag == null || magazineNo != lMag.MagazineNo)
                {
                    throw new ApplicationException(
                        $"投入と完了でマガジンの入れ替わりが発生しています。データメンテナンスで前投入マガジンの実績修正、現投入マガジンの実績登録、仮想マガジンの削除を行って下さい。投入マガジン:{magazineNo} 仮想マガジン:{lMag?.MagazineNo}");
                }
                OutputApiLog($"{ this.Name } [完了登録] ローダー仮想マガジン存在確認 MagazinNo:{lMag.MagazineNo}");

                try
                {
                    lMag.WorkStart = this.Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"開始日時の取得に失敗 MagazinNo:{lMag.MagazineNo}");
                }

                try
                {
                    lMag.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"完了日時の取得に失敗 MagazinNo:{lMag.MagazineNo}");
                }

                lMag.LastMagazineNo = lMag.MagazineNo;
            }
                                        
            //Order order = Order.GetMachineStartOrder(this.MacNo);
            //if (order == null)
            //{
            //    throw new ApplicationException($"作業開始実績が存在しない為、完了登録できません。データメンテナンスで完了登録を行って下さい。MagazineNo:{lMag.MagazineNo}");
            //}

            AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
            OutputApiLog($"{ this.Name } [完了登録] ロット存在確認 LotNo:{lot.NascaLotNo}");

            Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

            Order order = Order.GetMagazineOrder(lot.NascaLotNo, p.ProcNo);
            if (order == null)
            {
                throw new ApplicationException($"作業開始実績が存在しない為、完了登録できません。データメンテナンスで完了登録を行って下さい。MagazineNo:{lMag.MagazineNo}");
            }
            
            string finFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, MachineLog.FinishedFile.FINISHEDFILE_IDENTITYNAME, true);
            OutputApiLog($"{ this.Name } [完了登録] finFile FileName:{finFile}");

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, File.GetLastWriteTime(finFile));
            foreach (string lotFile in lotFiles)
            {
                MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, magazineNo);
                OutputApiLog($"{ this.Name } [完了登録] ロットファイル名称変更 FileName:{lotFile}");
            }

            if (ulMag == null)
            {
                if (this.Enqueue(lMag, Station.Unloader))
                {
                    this.Dequeue(Station.Loader);
                    OutputApiLog($"{ this.Name } [完了登録] アンローダーマガジン作成完了 MagazineNo:{lMag.MagazineNo}");
                }
            }
            else
            {
                // 同ロットの2缶目の読み込みと判断し、アンローダーマガジンの完了日時のみ更新
                ulMag.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                ulMag.Updatequeue();
            }
        }

        /// <summary>
        /// 開始登録結果OKを装置へ送信
        /// </summary>
        private void responseResultOfWorkStartOk()
        {
            this.Plc.SetString(RESPONSE_WORKSTART_RESULT_ADDRESS, "1");
            this.Plc.SetBit(RESPONSE_WORKSTART_RESULT_COMPLETE_ADDRESS, 1, "1");
            this.OutputApiLog($"{this.Name} << システム 開始登録OK");
        }

        /// <summary>
        /// 開始登録結果NGを装置へ送信
        /// </summary>
        private void responseResultOfWorkStartNg()
        {
            this.Plc.SetString(RESPONSE_WORKSTART_RESULT_ADDRESS, "2");
            this.Plc.SetBit(RESPONSE_WORKSTART_RESULT_COMPLETE_ADDRESS, 1, "1");
            this.OutputApiLog($"{this.Name} << システム 開始登録NG");
        }
    }
}
