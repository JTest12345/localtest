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
    /// ライン連結橋 (実Mag排出要求判定に空Mag判定フラグを採用)
    /// </summary>
    public class LineBridge2 : LineBridge
    {
        public LineBridge2()
        {
        }

        protected override void concreteThreadWork()
        {
            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 排出要求確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            // ブリッジを経由して空Magを搬送する時に
            // ブリッジ > 空Mag要求装置 の優先度設定になっている場合、
            // ブリッジ出口の空Magがブリッジ上の実Mag排出扱いになってしまう為、
            // 空Magフラグが立っている時は、実Mag排出扱いにしないようにする。

            string bitdata;
            try
            {
                bitdata = this.Plc.GetBit(EmpMagUnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{EmpMagUnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (bitdata == Mitsubishi.BIT_ON)
            //if (this.Plc.GetBit(EmpMagUnLoaderReqBitAddress) == Mitsubishi.BIT_ON)
            {
                return false;
            }

            // 空Magフラグが立っていなければ、元と同じ処理で実Mag排出要求判定を行う
            bool retv = base.IsRequireOutput();

            return retv;
        }

        /// <summary>
        /// キャリアクラス「Bridge」の処理用
        /// </summary>
        /// <returns></returns>
        public bool IsRequireOutput_UsingBaseClass()
        {
            return base.IsRequireOutput();
        }

        /// <summary>
        /// 空マガジンの配置処理
        /// </summary>
        /// <param name="m"></param>
        public override bool ResponseEmptyMagazineRequest()
        {
            if (IsRequireOutputEmptyMagazine() == true)
            {
                #region 空排出ONの処理

                VirtualMag mag = this.Peek(Station.Unloader);
                if (mag == null) return false;

                // 排出行の空マガジン かつ 手の届く範囲に空Mag供給の装置が無い場合、
                // この関数内で排出CVへの搬送指令を出す
                Location from = this.GetUnLoaderLocation();

                if (mag.NextMachines.Count == 1)
                {
                    int nextMacNo = mag.NextMachines.First();
                    if (LineKeeper.GetMachine(nextMacNo) is DischargeConveyor ||
                        LineKeeper.GetMachine(nextMacNo) is MAPBoardDischargeConveyor)
                    {
                        // 次装置 = 排出CVのみの時は、この関数内で排出CVへの搬送命令を出す。
                        Location to = new Location(nextMacNo, Station.Loader);
                        this.OutputSysLog($"空Mag排出ONの状態で排出CV行きの為、排出します。-> マガジンNo『{mag.MagazineNo}』 排出先：{to.DirectoryPath}");
                        LineKeeper.MoveFromTo(from, to, true, true, false);
                        return true;
                    }
                }
                #endregion

                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
