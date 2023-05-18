using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    public class LineBridgeBuffer : LineBridge
    {
        #region プロパティ

        /// <summary>
        /// バッファ上の排出マガジンが空Magかどうかのフラグ  (True：空Mag, False：実Mag)
        /// 排出信号ON + 仮想マガジン有りでTrue/Falseを入れる。
        /// ソフト立ち上げ時 or 供給要求ONでNullに戻す
        /// </summary>
        public bool? EmpMagOutputFg { get; set; }
                
        /// <summary>
        /// 装置内バッファ位置No  全てのバッファで一つの同じ装置No(MacPoint)が振られておりバッファ位置Noで区分けしているので装置毎に設定が必要
        /// </summary>
        public int BufferCode { get; set; }

        /// <summary>
        /// 最後にフラグを更新した時刻
        /// </summary>
        public DateTime FlagLastUpdateTime { get; set; }

        /// <summary>
        /// 定期的にフラグをOFFにする周期 単位[秒]
        /// </summary>
        private const int FLAG_OFF_TIME = 300;


        #endregion

        #region コンストラクタ
        public LineBridgeBuffer()
        {
            // フラグをリセット
            this.ResetEmpMagOutputFg();
            this.FlagLastUpdateTime = DateTime.MinValue;
        }
        #endregion

        protected override void concreteThreadWork()
        {
            if (base.IsRequireOutput() == true)
            {
                // 仮想マガジンの移動
                // Bridgeクラスのロボットに処理させるので不要
                //this.workComplete();

                // フラグの設定
                this.SettingEmpMagOutputFg();
            }

            // 供給要求ON ⇒ フラグをリセット
            if (base.IsRequireInput() == true)
            {
                this.ResetEmpMagOutputFg();
            }

            // 定期的にフラグを付け替える
            if ((DateTime.Now - this.FlagLastUpdateTime).Seconds >= FLAG_OFF_TIME)
            {
                this.ResetEmpMagOutputFg();
                this.SettingEmpMagOutputFg();
            }

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 空Magフラグの値をリセット
        /// </summary>
        /// <returns></returns>
        public void ResetEmpMagOutputFg()
        {
            if (this.EmpMagOutputFg.HasValue == false)
            {
                return;
            }

            this.EmpMagOutputFg = null;
            this.OutputSysLog($"排出フラグ初期化");
        }

        /// <summary>
        /// 空Magフラグの値を設定
        /// </summary>
        /// <returns></returns>
        public void SettingEmpMagOutputFg()
        {
            #region

            // 設定済みなら処理しない
            if (this.EmpMagOutputFg.HasValue == true)
            {
                return;
            }

            VirtualMag ulMag = this.Peek(Station.Unloader);
            if (ulMag == null)
            {
                return;
            }

            Magazine svrMag = Magazine.GetCurrent(ulMag.MagazineNo);
            if (svrMag != null)
            {
                // 稼働中ロットがある ⇒ 実Mag (フラグ = False)
                this.EmpMagOutputFg = false;
                this.OutputSysLog($"実Mag排出要求ON。[マガジンNO：{ulMag.MagazineNo}, ロットNO：{svrMag.NascaLotNO}]");
            }
            else if (VirtualMag.IsECKMag(ulMag.MagazineNo) == true)
            {
                // 遠沈マガジンの空Mag ⇒ 実Mag (フラグ = False)
                this.EmpMagOutputFg = false;
                this.OutputSysLog($"実Mag排出要求ON。[マガジンNO：{ulMag.MagazineNo}](遠沈マガジン)");
            }
            else
            {
                // 稼働中ロットがない ⇒ 空Mag (フラグ = True)
                this.EmpMagOutputFg = true;
                this.OutputSysLog($"空Mag排出要求ON。[マガジンNO：{ulMag.MagazineNo}]");
            }

            // フラグを設定した時刻を記録
            this.FlagLastUpdateTime = DateTime.Now; 

            #endregion
        }

        /// <summary>
        /// 排出要求確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            #region

            if (this.EmpMagOutputFg.HasValue == false)
            {
                return false;
            }

            if (this.EmpMagOutputFg.Value == true)
            {
                // フラグ = 空Mag扱いなら実Mag排出要求OFF扱いとする
                return false;
            }

            if (base.IsRequireOutput() == false)
            {
                return false;
            }
            return true;

            #endregion
        }

        /// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            #region

            if (this.EmpMagOutputFg.HasValue == false)
            {
                return false;
            }

            if (this.EmpMagOutputFg.Value == false)
            {
                // フラグ = 実Mag扱いなら実Mag排出要求OFF扱いとする
                return false;
            }

            if (base.IsRequireOutput() == false)
            {
                return false;
            }
            return true;

            #endregion
        }

        /// <summary>
        /// 作業完了 (仮想マガジンを『Loader』 ⇒ 『UnLoader』に移動
        /// </summary>
        public void workComplete()
        {
            #region

            VirtualMag lMag = this.Peek(Station.Loader);
            if (lMag == null)
            {
                return;
            }

            lMag.LastMagazineNo = lMag.MagazineNo;

            //仮想マガジンのEnqueueが失敗(Unloaderに既に同マガジンが有る)場合でもLoader側の
            //仮想マガジンは削除する。
            this.Enqueue(lMag, Station.Unloader);
            this.Dequeue(Station.Loader);

            #endregion
        }

        /// <summary>
        /// バッファ位置取得
        /// </summary>
        /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            // 搬送先ステーション関係なく、LineConfigの設定値を返す
            return this.BufferCode.ToString();
        }
        
        /// <summary>
        /// キャリアクラス「Bridge」の処理用
        /// </summary>
        /// <returns></returns>
        public bool IsRequireOutput_UsingBaseClass()
        {
            return base.IsRequireOutput();
        }

        #region ResponseEmptyMagazineRequest
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

                // 【N工場MAP J9・10不具合 修正】
                if (mag.NextMachines.Count == 1)
                {
                    int nextMacNo = mag.NextMachines.First();
                    if (LineKeeper.GetMachine(nextMacNo) is DischargeConveyor ||
                        LineKeeper.GetMachine(nextMacNo) is MAPBoardDischargeConveyor)
                    {
                        // 次装置 = 排出CVのみの時は、この関数内で排出CVへの搬送命令を出す。
                        IMachine dischargecv = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                        Location locDischargecv = dischargecv.GetLoaderLocation();
                        this.OutputSysLog($"空Mag排出ONの状態で排出CV行きの為、排出します。-> マガジンNo『{mag.MagazineNo}』 排出先： {locDischargecv.DirectoryPath}");
                        LineKeeper.MoveFromTo(from, locDischargecv, true, true, false);
                        return true;
                    }
                }

                CarrierInfo fromCarrier = Route.GetReachable(from);
                List<IMachine> reqEmpMagMachines = new List<IMachine>();
                foreach (int macno in Route.GetMachines(fromCarrier.CarNo))
                {
                    IMachine mac = LineKeeper.Machines.Find(m => m.MacNo == macno);
                    if (mac == null) continue;
                    if (mac is Mold && !(mac is Mold3))
                    {
                        continue;
                    }
                    if (mac is DischargeConveyor) continue;
                    if (mac is LineBridge) continue;
                    if (mac is AutoLineOutConveyor) continue;
                    if (mac is MAPBoardDischargeConveyor) continue;

                    if (mac.IsRequireInput() == true)
                    {
                        reqEmpMagMachines.Add(mac);
                    }
                }
                if (reqEmpMagMachines.Count() > 0)
                {
                    return false;
                }

                if (mag.NextMachines.Count() > 1)
                {
                    return false;
                }
                else if (mag.NextMachines.Count() == 1)
                {
                    int nextMacNo = mag.NextMachines.First();
                    if (!(LineKeeper.GetMachine(nextMacNo) is DischargeConveyor))
                    {
                        // 次装置が排出CV以外の場合は、他装置行として扱う。
                        return false;
                    }
                }                

                // 次装置が無い or 次装置 =排出CVの時は、排出CVへの搬送指令を出す。
                IMachine conveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                Location to = conveyor.GetLoaderLocation();
                this.OutputSysLog($"空Mag排出ONの状態で手の届く範囲に空Mag供給要求の装置が無い為、排出します。-> {to.DirectoryPath}");
                LineKeeper.MoveFromTo(from, to, true, true, false);

                #endregion

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
