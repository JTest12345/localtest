using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ライン連結橋
    /// </summary>
    public class LineBridge : MachineBase
    {
        ///// <summary>
        ///// 送り側　仮想ファイルパイプ
        ///// </summary>
        //public string SendBridgePipeBasePath { get; set; }

        ///// <summary>
        ///// 受け側　仮想ファイルパイプ（別PC)
        ///// </summary>
        //public string ReceiveBridgePipiBasePath { get; set; }

        ///// <summary>
        ///// アンローダー位置でOriginをリセットする値
        ///// </summary>
        //public int OriginReset { get; set; }

        ///// <summary>
        ///// Origin!=0の場合アンローダー位置で上書きするNextMachine
        ///// </summary>
        //public string AltMoveToMacNo { get; set; }

        ///// <summary>
        ///// 空マガジン排出信号(System上)
        ///// </summary>
        //public bool IsEmptyMagazineOutput { get; set; } 

        /// <summary>
        /// 搬送するマガジンが空マガジンの場合はONにするAddress
        /// </summary>
        public string EmptyMagazineInputBitAddress { get; set; }

        public LineBridge() 
        {
        }

        protected override void concreteThreadWork()
        {
            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            if (string.IsNullOrWhiteSpace(this.EmpMagUnLoaderReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = this.Plc.GetBit(EmpMagUnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空排出要求OFF扱い。アドレス：『{EmpMagUnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (retv == Mitsubishi.BIT_OFF)
            //if (this.Plc.GetBit(EmpMagUnLoaderReqBitAddress) == Mitsubishi.BIT_OFF)
            {
                return false;
            }
            if (this.Peek(Station.Unloader) == null)
            {
                return false;
            }

            return true;
        }
    }
}
