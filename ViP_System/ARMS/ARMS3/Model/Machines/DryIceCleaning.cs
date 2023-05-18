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
    public class DryIceCleaning : MachineBase
    {
        private string REQUIRE_WORKSTART_ADDRESS = "B000510";

        private string RESPONSE_WORKSTART_RESULT_ADDRESS = "W000410";

        private string RESPONSE_WORKSTART_RESULT_COMPLETE_ADDRESS = "B000410";

        private string LOADER_MAGAZINENO_ADDRESS = "W000500";

        private string UNLOADER_MAGAZINENO_ADDRESS = "W000530";

        /// <summary>
        /// 基板枚数上限
        /// </summary>
        private const int MAX_FRAME_CT = 40;

        /// <summary>
        /// 基板情報(DMコード)アドレス 基板1枚目
        /// </summary>
        private const string DATAMATRIX_ADDR_START = "D007594";
        private const int DATAMATRIX_ADDR_LENGTH = 10;

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
                throw new ApplicationException("装置完了時のログ出力先(LogOutputDirectoryPath)が未設定です。システム担当者に連絡して下さい。");
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
                    // ローダー仮想マガジンに読み込んだマガジンが有る場合、登録済みと判断
                    return;
                }

                Order o = Order.GetMagazineOrder(svrMag.NascaLotNO, p.ProcNo);
                if (o != null)
                {
                    throw new ApplicationException($"このマガジンは現在完了工程の次工程で既に開始実績が存在します。データメンテナンスで確認して下さい。MagazineNo:{magazineNo}");
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

            VirtualMag lMag = this.Peek(Station.Loader);
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

            Order order = Order.GetMachineStartOrder(this.MacNo);
            if (order == null)
            {
                throw new ApplicationException($"作業開始実績が存在しない為、完了登録できません。データメンテナンスで完了登録を行って下さい。MagazineNo:{lMag.MagazineNo}");
            }
            AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
            OutputApiLog($"{ this.Name } [完了登録] ロット存在確認 LotNo:{lot.NascaLotNo}");

            string finFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, MachineLog.FinishedFile.FINISHEDFILE_IDENTITYNAME, true);

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, lMag.WorkStart.Value, File.GetLastWriteTime(finFile));
            foreach (string lotFile in lotFiles)
            {
                MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
                OutputApiLog($"{ this.Name } [完了登録] ロットファイル名称変更 FileName:{lotFile}");
            }

            //段数情報を記録
            RegisterCarrierData(lMag, lot);

            this.WorkComplete(lMag, this, true);
            this.Dequeue(Station.Loader);

            OutputApiLog($"{ this.Name } [完了登録] 実績登録完了 LotNo:{lot.NascaLotNo}");
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


        ///****未検証****
        /// <summary>
        /// 基板段数及びポジションデータをデータベースに登録
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="distance"></param>
        /// <param name="MaxCt"></param>
        /// <returns></returns>
        private void RegisterCarrierData(VirtualMag mag, AsmLot lot)
        {
            Dictionary<string, int> stockData = new Dictionary<string, int>();

            //int loadStepCd = ArmsApi.Model.LENS.Mag.GetMagStepCd(lot.TypeCd).Value;

            //段数情報をPLCから取得
            for (int i = 0; i < MAX_FRAME_CT; i++)
            {
                int step = i + 1;

                // 基板DMを取得して0ならcontinue
                string dmAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(DATAMATRIX_ADDR_START, DATAMATRIX_ADDR_LENGTH * i);
                string dataMatrix = this.Plc.GetMagazineNo(dmAddress, DATAMATRIX_ADDR_LENGTH);

                if (string.IsNullOrWhiteSpace(dataMatrix) == true || dataMatrix == "0")
                {
                    continue;
                }

                stockData.Add(dataMatrix, step);
            }

            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = lot.NascaLotNo;
            regData.ProcNo = Convert.ToInt32(mag.ProcNo);
            regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            int loadCt = 0;

            foreach (KeyValuePair<string, int> data in stockData)
            {
                regData.CarrierNo = data.Key;

                int step = data.Value;
                loadCt++;

                //装置によって積み方が違ってマスタで対応できないので無しに変更。（レポーター側で対応する）
                ////段数情報はPLCから抜く場合元々搭載しない段もカウントしてしまうので、それを除外して集計する。
                ////loadStepCdは1が偶数段のみ、2が奇数段のみ
                //if (loadStepCd == 1)
                //{
                //    step = step / 2;
                //}
                //else if (loadStepCd == 2)
                //{
                //    step = (step + 1) / 2;
                //}
                //else if (loadStepCd != 3)
                //{
                //    continue;
                //}

                //段数登録
                regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
                regData.Value = Convert.ToString(step);
                regData.InsertUpdate();

                //キャリア投入位置を設定⇒4枚で1キャリアなので積んだ順で割り振る。（抜けが有ったら次段で埋めるので段抜けは考慮しない）
                regData.Infoid = CarrireWorkData.IN_SURFACE_ADDR_INFOCD;
                regData.Value = Convert.ToString(loadCt % 4);
                if (regData.Value == "0") regData.Value = "4";
                regData.InsertUpdate();
            }
        }
    }
}

