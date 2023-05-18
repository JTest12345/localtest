using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// プラズマ
    /// </summary>
    public class Plasma : MachineBase
    {
        /// <summary>
        /// 供給禁止信号（工程別）
        /// </summary>
        public SortedList<int, string> ProcForbiddenAddressList { get; set; }

        /// <summary>
        /// 排出モード信号（工程別）
        /// </summary>
        public SortedList<int, string> ProcDischargeAddressList { get; set; }

        /// <summary>
        /// 終了時排出マガジンNo取得アドレス
        /// </summary>
        public string ULMagazineAddress { get; set; }

        public Plasma()
            : base()
        {
            this.ProcForbiddenAddressList = new SortedList<int, string>();
            this.ProcDischargeAddressList = new SortedList<int, string>();
        }

        protected override void concreteThreadWork()
        {
            if (this.IsRequireOutput() == true)
            {
                workComplete();
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

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
            Log.RBLog.Info("プラズマ");

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("プラズマ機排出マガジンNOの取得に失敗");
                return;
            }

            newMagazine.LastMagazineNo = newmagno;

            //作業開始完了時間取得
            try
            {
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("プラズマ機 作業完了時間取得失敗:{0}", this.MacNo));
            }

            try
            {
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            }
            catch
            {
                throw new ApplicationException(string.Format("プラズマ機 作業開始時間取得失敗:{0}", this.MacNo));
            }

            //作業IDを取得
            newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newMagazine.MagazineNo);

            this.Enqueue(newMagazine, Station.Unloader);

            this.WorkComplete(newMagazine, this, true);
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

        /// <summary>
        /// 工程指定で判定
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public override bool CanInput(VirtualMag mag)
        {
            if (ProcForbiddenAddressList.Keys.Contains(mag.ProcNo.Value) == false) return true;
            string address = ProcForbiddenAddressList[mag.ProcNo.Value];

            if (string.IsNullOrEmpty(address)) return true;

            string retv = Plc.GetBit(address);
            if (retv == Mitsubishi.BIT_ON)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDischargeMode(VirtualMag mag)
        {
            if (mag == null) return false;

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null) return false;

            int proc = svrmag.NowCompProcess;

            if (ProcDischargeAddressList.Keys.Contains(proc) == false) return false;
            string address = ProcDischargeAddressList[proc];

            if (string.IsNullOrEmpty(address)) return false;

            string retv = string.Empty;
            if (this.CarrierPlc != null)
            {
                // 指定PLC(搬送パネルを想定)を使用
                retv = this.CarrierPlc.GetBit(address);
            }
            else
            {
                // 装置PLCを使用
                retv = Plc.GetBit(address);
            }
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
