using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    public class Sputter : MachineBase
    {
        /// <summary>
        /// ローダー側マガジンNo読取完了信号アドレス
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }

        /// <summary>
        /// ローダー側マガジンNo取得アドレス
        /// </summary>
        public string LoaderMagazineAddress { get; set; }

        /// <summary>
        /// ローダー側マガジン装置選択完了信号アドレス
        /// </summary>
        public string LoaderMachineSelectCompleteBitAddress { get; set; }

        /// <summary>
        /// ローダー側装置番号アドレス
        /// </summary>
        public string LoaderMachineNoAddress { get; set; }

        /// <summary>
        /// ローダー側作業完了アドレス
        /// </summary>
        public string LoaderWorkCompleteBitAddress { get; set; }

        /// <summary>
        /// ローダー側作業エラーアドレス
        /// </summary>
        public string LoaderWorkErrorBitAddress { get; set; }

        /// <summary>
        /// アンローダー側マガジンNo読取完了信号アドレス
        /// </summary>
        public string UnLoaderQRReadCompleteBitAddress { get; set; }

        /// <summary>
        /// アンローダー側マガジンNo取得アドレス
        /// </summary>
        public string UnLoaderMagazineAddress { get; set; }

        /// <summary>
        /// アンローダー側作業完了アドレス
        /// </summary>
        public string UnLoaderWorkCompleteBitAddress { get; set; }

        /// <summary>
        /// アンローダー側作業エラーアドレス
        /// </summary>
        public string UnLoaderWorkErrorBitAddress { get; set; }

        /// <summary>
        /// アンローダー側装置番号アドレス
        /// </summary>
        public string UnLoaderMachineNoAddress { get; set; }

        /// <summary>
        /// ハートビート
        /// </summary>
        public string HeartBeatAddress { get; set; }

        public int QRMagazineNoLength { get; set; }

        /// <summary>
        /// ローダー側トレイNo取得アドレス
        /// </summary>
        public string LoaderTrayAddress { get; set; }

        /// <summary>
        /// ローダー側トレイ順序取得アドレス
        /// </summary>
        public string LoaderTrayOrderAddress { get; set; }

        /// <summary>
        /// アンローダー側トレイNo取得アドレス
        /// </summary>
        public string UnLoaderTrayAddress { get; set; }

        /// <summary>
        /// アンローダー側トレイ順序取得アドレス
        /// </summary>
        public string UnLoaderTrayOrderAddress { get; set; }

        /// <summary>
        /// ロットとトレイ関連付け機能のON/OFFアドレス
        /// </summary>
        public string RelateLotTrayFunctionBitAddress { get; set; }

        /// <summary>
        /// マガジン番号WORDアドレスの長さ(自動化)
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH_AUTO = 6;

        /// <summary>
        /// マガジン番号WORDアドレスの長さ(高効率)
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH_HIGH = 10;

        /// <summary>
        /// トレイ番号WORDアドレスの長さ
        /// </summary>
        public const int TRAY_NO_WORD_LENGTH_HIGH = 10;

        protected override void concreteThreadWork()
        {
            Plc.SetBit(HeartBeatAddress, 1, Mitsubishi.BIT_ON);

            if (IsUnLoaderQRReadComplete())
            {
                workComplete();
            }

            if (IsLoaderQRReadComplete())
            {
                workStart();
            }
        }

        protected bool IsLoaderQRReadComplete()
        {
            // 供給ﾊﾞｰｺｰﾄﾞ読込指令信号確認
            if (string.IsNullOrEmpty(this.LoaderQRReadCompleteBitAddress) == true)
            {
                return false;
            }

            if (Plc.GetBit(this.LoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsUnLoaderQRReadComplete()
        {
            // 排出ﾊﾞｰｺｰﾄﾞ読込指令信号確認
            if (string.IsNullOrEmpty(this.UnLoaderQRReadCompleteBitAddress) == true)
            {
                return false;
            }

            if (Plc.GetBit(this.UnLoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            if (this.IsAutoLine) { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_AUTO; }
            else { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_HIGH; }

            string magno = Plc.GetMagazineNo(this.UnLoaderMagazineAddress, this.QRMagazineNoLength);
            Magazine svrmag = Magazine.GetCurrent(magno);
            if (svrmag == null)
            {
                return;
            }
            newMagazine.MagazineNo = magno;

            string machineNo;
            try
            {
                // 装置番号取得
                machineNo = Plc.GetMachineNo(this.UnLoaderMachineNoAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("{0} 供給装置番号取得失敗", this.Name));
            }
            MachineInfo machine = MachineInfo.GetMachine(machineNo);
            if (machine == null)
            {
                throw new ApplicationException(string.Format("{0} 装置マスタに対象の装置が見つかりません。設備番号：{1}", this.Name, machineNo));
            }
            //スパッタ装置のインスタンスを装置から受信した設備で作成する
            Sputter sputter = new Sputter();
            sputter.MacNo = machine.MacNo;
            sputter.LineNo = this.LineNo;
            sputter.IsAutoLine = machine.IsAutoLine;
            sputter.IsHighLine = machine.IsHighLine;
            sputter.MacGroup = machine.MacGroup;

            try
            {
                // 作業完了時間取得
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("{0} 作業完了時間取得失敗", this.Name));
            }

            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process p = Process.GetNextProcess(svrmag.NowCompProcess, lot);

            Order startOrder = Order.GetMagazineOrder(svrmag.NascaLotNO, p.ProcNo);
            if (startOrder == null)
            {
                return;
            }
            newMagazine.WorkStart = startOrder.WorkStartDt;
            newMagazine.ProcNo = p.ProcNo;
            newMagazine.LastMagazineNo = magno;

            if (isLotTrayRelation())
            {
                try
                {
                    // トレイに紐づいたロットとマガジンに紐づいたロットが一致するかの確認
                    string trayNo = getTrayNo(Station.Unloader);
                    if (string.IsNullOrWhiteSpace(trayNo))
                    {
                        throw new ApplicationException("取得したトレイNoが空白です。");
                    }
                    int orderNo = getTrayOrderNo(Station.Unloader);
                    if (orderNo == 0)
                    {
                        throw new ApplicationException("取得した連番が0です。");
                    }

                    List<AsmLot.LotTray> LotTrayList = AsmLot.GetRelateTray(trayNo, orderNo, null, true, false);
                    List<AsmLot.LotTray> tempLotTrayList = AsmLot.GetRelateTray(trayNo, orderNo, null, false, true);

                    if (LotTrayList.Count == 0 && tempLotTrayList.Count == 0)
                    {
                        throw new ApplicationException(string.Format(
                            "取得したトレイ・連番に割当て中のロットが見つかりませんでした。トレイ={0}, 連番={1}",
                            trayNo, orderNo));
                    }

                    if (LotTrayList.Count > 1)
                    {
                        throw new ApplicationException(string.Format(
                            "取得したトレイ・連番に割当て中のロットが複数あります。トレイ={0}, 連番={1}, 割当て中ロット={2}",
                            trayNo, orderNo, string.Join(",", LotTrayList.Select(l => l.LotNo))));
                    }

                    List<string> lotList = new List<string>();
                    lotList.AddRange(LotTrayList.Select(l => l.LotNo));
                    lotList.AddRange(tempLotTrayList.Select(l => l.LotNo));

                    // 本割当てと仮割当てのロットで比較
                    if (lotList.Contains(lot.NascaLotNo) == false)
                    {
                        throw new ApplicationException(string.Format("トレイとマガジンに紐づいたロットが一致しません。\r\nトレイ側({2}-{3})：[{0}], マガジン側({4})：{1}"
                            , string.Join(",",lotList), lot.NascaLotNo, trayNo, orderNo.ToString(), magno));
                    }

                    AsmLot.DissolveTray(lot.NascaLotNo, false);
                    this.OutputSysLog(string.Format("ロット-トレイ割当て解除 ： {0}", lot.NascaLotNo));
                }
                catch (ApplicationException ex)
                {
                    Plc.SetBit(UnLoaderWorkErrorBitAddress, 1, Mitsubishi.BIT_ON);
                    this.OutputSysLog("マガジン-トレイのロット照合処理で異常発生：" + ex.Message);

                    Plc.SetBit(UnLoaderWorkCompleteBitAddress, 1, Mitsubishi.BIT_ON);
                    this.OutputSysLog("作業完了処理中止");

                    // 作業完了処理を行わない
                    return;
                }
            }

            this.Enqueue(newMagazine, Station.Unloader);
            if (this.WorkComplete(newMagazine, sputter, true) == false)
            {
                Plc.SetBit(UnLoaderWorkErrorBitAddress, 1, Mitsubishi.BIT_ON);
                this.OutputSysLog("作業完了処理で異常発生");
            }
            this.Dequeue(Station.Unloader);

            Plc.SetBit(UnLoaderWorkCompleteBitAddress, 1, Mitsubishi.BIT_ON);
            this.OutputSysLog("作業完了処理完了");
        }

        /// <summary>
        /// 作業開始
        /// </summary>
        private void workStart()
        {
            if (this.IsAutoLine) { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_AUTO; }
            else { QRMagazineNoLength = MAGAZINE_NO_WORD_LENGTH_HIGH; }

            string magno = Plc.GetMagazineNo(LoaderMagazineAddress, this.QRMagazineNoLength);
            Magazine svrmag = Magazine.GetCurrent(magno);
            if (svrmag == null)
            {
                return;
            }

            // 供給ﾊﾞｰｺｰﾄﾞ確認信号確認
            if (string.IsNullOrEmpty(this.LoaderMachineSelectCompleteBitAddress) == true)
            {
                return;
            }
            if (Plc.GetBit(this.LoaderMachineSelectCompleteBitAddress) == Mitsubishi.BIT_OFF)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();
            newMagazine.MagazineNo = magno;

            string machineNo;
            try
            {
                // 装置番号取得
                machineNo = Plc.GetMachineNo(this.LoaderMachineNoAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("{0} 排出装置番号取得失敗", this.Name));
            }
            MachineInfo machine = MachineInfo.GetMachine(machineNo);
            if (machine == null)
            {
                throw new ApplicationException(string.Format("{0} 装置マスタに対象の装置が見つかりません。設備番号：{1}", this.Name, machineNo));
            }
            //自身のインスタンスを装置から受信した設備で作成する
            Sputter sputter = new Sputter();
            sputter.MacNo = machine.MacNo;
            sputter.LineNo = this.LineNo;
            sputter.IsAutoLine = machine.IsAutoLine;
            sputter.IsHighLine = machine.IsHighLine;
            sputter.MacGroup = machine.MacGroup;

            try
            {
                // 作業開始時間取得
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("{0} 作業開始時間取得失敗", this.Name));
            }

            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process p = Process.GetNextProcess(svrmag.NowCompProcess, lot);
            newMagazine.ProcNo = p.ProcNo;

            if (base.WorkStart(newMagazine, sputter) == false)
            {
                Plc.SetBit(LoaderWorkErrorBitAddress, 1, Mitsubishi.BIT_ON);
                this.OutputSysLog("作業開始処理で異常発生");
            }

            if (isLotTrayRelation())
            {
                try
                {
                    string trayNo = getTrayNo(Station.Loader);
                    if (string.IsNullOrWhiteSpace(trayNo))
                    {
                        throw new ApplicationException("取得したトレイNoが空白です。");
                    }
                    int orderNo = getTrayOrderNo(Station.Loader);
                    if (orderNo == 0)
                    {
                        throw new ApplicationException("取得した連番が0です。");
                    }
                    //// 割当て前に同じトレイ・連番で残っている割当て情報を解除(仮割当てフラグを付与)
                    AsmLot.DissolveTray(trayNo, orderNo, true);

                    // トレイにロットを割当て
                    AsmLot.RelateTray(lot.NascaLotNo, trayNo, orderNo);
                }
                catch (ApplicationException ex)
                {
                    Plc.SetBit(LoaderWorkErrorBitAddress, 1, Mitsubishi.BIT_ON);
                    this.OutputSysLog(ex.Message);
                }
            }

            Plc.SetBit(LoaderWorkCompleteBitAddress, 1, Mitsubishi.BIT_ON);
            this.OutputSysLog("作業開始処理完了");
        }

        /// <summary>
        /// トレイNo取得
        /// </summary>
        /// <returns></returns>
        private string getTrayNo(Station st)
        {
            string trayAddress;

            switch (st)
            {
                case Station.Loader:
                    trayAddress = this.LoaderTrayAddress;
                    break;

                case Station.Unloader:
                    trayAddress = this.UnLoaderTrayAddress;
                    break;

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }

            try
            {
                return Plc.GetMagazineNo(trayAddress, TRAY_NO_WORD_LENGTH_HIGH);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("トレイNo取得失敗 アドレス:{0}に異常値が入っている可能性がある為、装置担当者に確認して下さい。{1}", trayAddress, ex.ToString()));
            }
        }

        /// <summary>
        /// トレイ順序取得
        /// </summary>
        /// <returns></returns>
        private int getTrayOrderNo(Station st)
        {
            string trayAddress;

            switch (st)
            {
                case Station.Loader:
                    trayAddress = this.LoaderTrayOrderAddress;
                    break;

                case Station.Unloader:
                    trayAddress = this.UnLoaderTrayOrderAddress;
                    break;

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }

            try
            {
                return Plc.GetWordAsDecimalData(trayAddress);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("トレイ順序取得失敗 アドレス:{0}に異常値が入っている可能性がある為、装置担当者に確認して下さい。{1}", trayAddress, ex.ToString()));
            }
        }

        /// <summary>
        /// ロットとトレイの関連づけ機能ON/OFF
        /// </summary>
        /// <returns></returns>
        private bool isLotTrayRelation()
        {
            if (string.IsNullOrEmpty(this.RelateLotTrayFunctionBitAddress))
            {
                return false;
            }

            if (this.Plc.GetBit(this.RelateLotTrayFunctionBitAddress) == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            if (station != Station.Unloader)
            {
                return true;
            }
            return base.Enqueue(mag, station);
        }
    }
}
