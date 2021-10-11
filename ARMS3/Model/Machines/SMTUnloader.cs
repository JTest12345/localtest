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
    /// アンローダー(SMTライン内) SIGMA用
    /// </summary>
    public class SMTUnloader : MachineBase
    {
        /// <summary>
        /// 基板読取完了ビットアドレス
        /// </summary>
        private const string STR_ADDRESS_READ_COMPLETE = "DM5110.0";
        /// <summary>
        /// 基板DMアドレス(今回)
        /// </summary>
        private const string STR_ADDRESS_DATAMATRIX = "DM6650.0";
        /// <summary>
        /// 基板DMアドレス(１枚前)
        /// </summary>
        private const string STR_ADDRESS_DATAMATRIX_BEFORE = "DM6660.0";
        /// <summary>
        /// 収納OKビットアドレス
        /// </summary>
        private const string STR_ADDRESS_PERMISSION = "DM5010.0";
        /// <summary>
        /// マガジンアドレス
        /// </summary>
        private const string STR_ADDRESS_MAGAZINE = "DM7000.0";
        /// <summary>
        /// マガジン切替ビットアドレス
        /// </summary>
        private const string STR_ADDRESS_MAGAZINE_CHANGE = "DM5011.0";
        /// <summary>
        /// ﾗｯｸ単位読取完了ビットアドレス
        /// </summary>
        private const string STR_ADDRESS_RACK_READ_COMPLETE = "DM5012.0";
        /// <summary>
        /// ﾗｯｸ単位読取開始ビットアドレス
        /// </summary>
        private const string STR_ADDRESS_REQUIRE_OUTPUT = "DM5111.0";

        private const int DATAMATRIX_WORD_LENGTH = 10;
        private const int MAGAZINE_WORD_LENGTH = 10;

        protected override void concreteThreadWork()
        {
            try
            {
                //２Ｄ読み取り完了ビットＯＮ
                if (isDatamatrixEnd())
                {
                    workComplete();
                }

                if (isRequireOutput())
                {
                    //パネルから排出ボタンを押した時は、マガジン内の基板をすべて読み取り
                    //下記ビットをONする必要がある

                    //ﾗｯｸ単位読取完了ビットON
                    this.Plc.SetBit(STR_ADDRESS_RACK_READ_COMPLETE, 1, Omron.BIT_ON);
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
                //前回の基板２Ｄと今回の基板２Ｄの内容読み取り　比較判断
                string dm = getDataMatrix(STR_ADDRESS_DATAMATRIX);
                OutputSysLog("SMTUnloader:DataMatrix(今回):" + dm);
                string dmBefore = getDataMatrix(STR_ADDRESS_DATAMATRIX_BEFORE);
                OutputSysLog("SMTUnloader:DataMatrix(１枚前):" + dmBefore);
                                
                string lotno = string.Empty;
                string lotnoBefore = string.Empty;
                if (!string.IsNullOrWhiteSpace(dm))
                {
                    lotno = LotCarrier.GetData(dm, true).LotNo;
                }
                if (!string.IsNullOrWhiteSpace(dmBefore))
                {
                    lotnoBefore = LotCarrier.GetData(dmBefore, true).LotNo;
                }

                bool permissionfg = false;
                string errmsg = string.Empty;
                if (!string.IsNullOrWhiteSpace(lotno) &&
                    !string.IsNullOrWhiteSpace(lotnoBefore))
                {
                    if (lotno == lotnoBefore)
                    {
                        permissionfg = true;
                    }
                    else
                    {
                        permissionfg = false;
                        //errmsg = string.Format("SMTUnloader:紐付ロットが一致しない 基板DM(今回):{0} ロットNO(今回):{1} 基板DM(前回):{2} ロットNO(前回):{3}", dm, lotno, dmBefore, lotnoBefore);
                    }
                }
                else if (string.IsNullOrWhiteSpace(dmBefore))
                {
                    permissionfg = true;
                    OutputSysLog(string.Format("SMTUnloader:基板DM(前回)データがなかったので投入を許可しました。基板DM(今回):{0} ロットNO(今回):{1}", dm, lotno));
                }
                else
                {
                    permissionfg = false;
                    if (string.IsNullOrWhiteSpace(lotno))
                    {
                        errmsg = string.Format("SMTUnloader:基板DMとロットの紐付が存在しない 基板DM(今回):{0}", dm);
                    }
                    if (string.IsNullOrWhiteSpace(lotnoBefore))
                    {
                        errmsg = string.Format("SMTUnloader:基板DMとロットの紐付が存在しない 基板DM(前回):{0}", dmBefore);
                    }
                }

                if (!permissionfg &&
                    !string.IsNullOrWhiteSpace(lotno) &&
                    !string.IsNullOrWhiteSpace(lotnoBefore))
                {
                    //マガジン内容を取得し、TnMagに新規紐付データを登録
                    string magazineno = getMagazine();
                    if (string.IsNullOrWhiteSpace(magazineno))
                    {
                        throw new ApplicationException("マガジン内容が取得できませんでした。");
                    }

                    Magazine[] lotlist = Magazine.GetMagazine(magazineno, null, false, null).Where(m => !m.NewFg).ToArray();
                    if (lotlist.Count() == 1)
                    {
                        Magazine svmag = lotlist.FirstOrDefault();

                        Magazine mag = new Magazine();
                        mag.MagazineNo = magazineno;
                        mag.NascaLotNO = lotnoBefore;
                        mag.NewFg = true;
                        mag.NowCompProcess = svmag.NowCompProcess;
                        mag.Update();
                    }
                    else
                    {
                        permissionfg = false;
                        if (lotlist.Count() == 0)
                        {
                            errmsg = string.Format("SMTUnloader:マガジン情報が取得できない Magazine:{0}", magazineno);
                        }
                        else
                        {
                            errmsg = string.Format("SMTUnloader:マガジン情報が複数取得された Magazine:{0}", magazineno);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(errmsg))
                {
                    throw new ApplicationException(errmsg);
                }

                //ビットON
                setBit(permissionfg);
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

        private string getDataMatrix(string address)
        {
            if (this.Plc == null) return string.Empty;

            return this.Plc.GetWord(address, DATAMATRIX_WORD_LENGTH).Replace("\0", "");
        }

        private void setBit(bool permissionfg)
        {
            if (permissionfg)
            {
                //収納OKビットON
                this.Plc.SetBit(STR_ADDRESS_PERMISSION, 1, Omron.BIT_ON);
            }
            else
            {
                //収納OKビットOFF
                this.Plc.SetBit(STR_ADDRESS_PERMISSION, 1, Omron.BIT_OFF);

                //マガジン切替ビットON
                this.Plc.SetBit(STR_ADDRESS_MAGAZINE_CHANGE, 1, Omron.BIT_ON);

                //ﾗｯｸ単位読取完了ビットON
                this.Plc.SetBit(STR_ADDRESS_RACK_READ_COMPLETE, 1, Omron.BIT_ON);
            }
        }

        private bool isDatamatrixEnd()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_READ_COMPLETE);
            if (retv == Omron.BIT_ON)
            {
                return true;
            }

            return false;
        }

        private bool isRequireOutput()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_REQUIRE_OUTPUT);
            if (retv == Omron.BIT_ON)
            {
                return true;
            }

            return false;
        }
    }
}
