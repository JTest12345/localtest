using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ローダー(SMTライン内) SIGMA用
    /// </summary>
    public class SMTLoader : CifsMachineBase
    {
        /// <summary>
        /// マガジン読み取り完了ビットアドレス
        /// </summary>
        private const string STR_ADDRESS_READ_COMPLETE = "DM5100.0";
        /// <summary>
        /// マガジンアドレス
        /// </summary>
        private const string STR_ADDRESS_MAGAZINE = "DM6500.0";
        /// <summary>
        /// ラック搬入許可アドレス
        /// </summary>
        private const string STR_ADDRESS_PERMISSION = "DM5000.0";
        /// <summary>
        /// ラック搬入禁止アドレス
        /// </summary>
        private const string STR_ADDRESS_PROHIBITION = "DM5001.0";

        private const int MAGAZINE_WORD_LENGTH = 10;

        protected override void concreteThreadWork()
        {
            try
            {
                //２Ｄ読み取り完了ビットＯＮ
                if (isMagazineEnd())
                {
                    workComplete();
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
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            try
            {
                //２Ｄ内容読み取り
                string magazineno = getMagazine();
                if (string.IsNullOrWhiteSpace(magazineno))
                {
                    throw new ApplicationException("マガジン内容が取得できませんでした。");
                }

                OutputSysLog("SMTLoader:Magazine:" + magazineno);

                //ロット取得
                Magazine[] lotlist = Magazine.GetMagazine(magazineno, null, true, null);

                bool permissionfg = false;
                string errmsg = string.Empty;
                if (lotlist.Count() == 1)
                {
                    Magazine svmag = lotlist.FirstOrDefault();
                    //基板DM取得
                    string datamatrix = LotCarrier.GetCurrentCarrierNo(svmag.NascaLotNO);
                    if (!string.IsNullOrWhiteSpace(datamatrix))
                    {
                        //TnMagのnewfgを0に更新
                        svmag.NewFg = false;
                        svmag.Update();

                        //OKビットを返す
                        permissionfg = true;
                    }
                    else
                    {
                        //NGビットを返す
                        permissionfg = false;
                        errmsg = string.Format("SMTLoader:基板DMとロットの紐付が存在しません。 Magazine:{0} ロットNO:{1}", magazineno, svmag.NascaLotNO);
                    }
                }
                else
                {
                    //NGビットを返す
                    permissionfg = false;
                    if (lotlist.Count() == 0)
                    {
                        errmsg = string.Format("SMTLoader:マガジン情報が取得できません。 Magazine:{0}", magazineno);
                    }
                    else
                    {
                        errmsg = string.Format("SMTLoader:マガジン情報が複数取得されました。 Magazine:{0}", magazineno);
                    }
                }

                //ビットON
                setBit(permissionfg);

                if (!string.IsNullOrWhiteSpace(errmsg))
                {
                    throw new ApplicationException(errmsg);
                }

                //はんだ印刷の実績自動登録
                //ローダーの最終搭載時刻＝開始時刻
                Magazine mag = lotlist.FirstOrDefault();
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                Process nextproc = ArmsApi.Model.Process.GetNextProcess(mag.NowCompProcess, lot);
                EntryOrder(lot, nextproc.ProcNo, magazineno, DateTime.Now, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getMagazine()
        {
            if (this.Plc == null) return string.Empty;

            return this.Plc.GetWord(STR_ADDRESS_MAGAZINE, MAGAZINE_WORD_LENGTH).Replace("\0", "");
        }

        private void setBit(bool permissionfg)
        {
            if (permissionfg)
            {
                //ラック搬入禁止ビットOFF
                this.Plc.SetBit(STR_ADDRESS_PROHIBITION, 1, Omron.BIT_OFF);
                //ラック搬入許可ビットON
                this.Plc.SetBit(STR_ADDRESS_PERMISSION, 1, Omron.BIT_ON);
            }
            else
            {
                //ラック搬入許可ビットOFF
                this.Plc.SetBit(STR_ADDRESS_PERMISSION, 1, Omron.BIT_OFF);
                //ラック搬入禁止ビットON
                this.Plc.SetBit(STR_ADDRESS_PROHIBITION, 1, Omron.BIT_ON);
            }
        }

        private bool isMagazineEnd()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_READ_COMPLETE);
            if (retv == Omron.BIT_ON)
            {
                return true;
            }

            return false;
        }
    }
}
