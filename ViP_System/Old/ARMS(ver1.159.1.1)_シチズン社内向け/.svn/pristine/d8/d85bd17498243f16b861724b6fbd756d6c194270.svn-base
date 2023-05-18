using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    public class OpeningCheckConveyor : MachineBase
    {
        /// <summary>
        /// 始業点検マガジン存在確認アドレス
        /// </summary>
        public string MagazineExistBitAddress { get; set; }

        /// <summary>
        /// 始業点検時期確認アドレス
        /// </summary>
        public string OpeningCheckBitAddress { get; set; }

        /// <summary>
        /// 親装置番号
        /// </summary>
        public int ParentMacNo { get; set; }

        protected override void concreteThreadWork()
        {
            if (magazineExist())
            {
                workComplete();
            }
            else
            {
                if (!isOpeningCheck()) 
                {
                    //仮想マガジン削除
                    ClearVirtualMagazines();
                }
            }
        }

        /// <summary>
        /// 始業点検マガジン存在確認
        /// </summary>
        /// <returns></returns>
        private bool magazineExist() 
        {
            if (string.IsNullOrEmpty(MagazineExistBitAddress) == true)
            {
                return false;
            }

            string retv = this.Plc.GetBit(MagazineExistBitAddress);
            if (retv == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
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

        /// <summary>
        /// 始業点検時期確認
        /// </summary>
        /// <returns></returns>
        private bool isOpeningCheck() 
        {
            if (string.IsNullOrEmpty(OpeningCheckBitAddress) == true)
            {
                return false;
            }

            string retv = this.Plc.GetBit(OpeningCheckBitAddress);
            if (retv == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
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
            newMagazine.MagazineNo = VirtualMag.UNKNOWN_MAGNO;
            newMagazine.ProcNo = null;
            
            this.Enqueue(newMagazine, Station.Unloader);

            base.WorkComplete(newMagazine, this, false);
        }

        /// <summary>
        /// マガジン排出　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput() 
        {
            if (string.IsNullOrEmpty(MagazineExistBitAddress) == true) 
            {
                return false;
            }
            if (string.IsNullOrEmpty(OpeningCheckBitAddress) == true) 
            {
                return false;
            }

            //string isOutput1 = Plc.GetBit(MagazineExistBitAddress);
            string isOutput1;
            try
            {
                isOutput1 = Plc.GetBit(MagazineExistBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{this.MagazineExistBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (isOutput1 == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }
            //string isOutput2 = Plc.GetBit(OpeningCheckBitAddress);
            string isOutput2;
            try
            {
                isOutput2 = Plc.GetBit(OpeningCheckBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{this.OpeningCheckBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (isOutput2 == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }

            if (isOutput1 == Mitsubishi.BIT_ON && isOutput2 == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// マガジン供給　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireInput()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.MagazineExistBitAddress) == true)
            {
                return false;
            }

            //string retv = this.Plc.GetBit(this.MagazineExistBitAddress);
            string retv;
            try
            {
                retv = this.Plc.GetBit(this.MagazineExistBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.MagazineExistBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (retv == Mitsubishi.BIT_OFF)
            {
                //マガジンが始業点検CVに無ければ可
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
