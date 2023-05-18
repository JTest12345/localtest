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
    /// 高生産性ライン用ロットマーキング情報のみ読み取るフレーム搭載機
    /// </summary>
    class FrameLoader2 : FrameLoader
    {
        private const int DEC_ADDRESS_START_MAG = 0;
        private const int DEC_ADDRESS_LOTNO = 3000;
        private const int LOTNO_WORD_LENGTH = 25;
        private const string STR_ADDRESS_OUTPUT_REQ = "B000000";
        private const string STR_ADDRESS_READ_COMPLETE = "B000010";

        protected override void concreteThreadWork()
        {
            try
            {
                if (LocalPLC == null) return;

                workComplete(this.Plc, 0);

                //プロファイルのタイプ名からロットマーキング有無を調査して搭載機へ直接指示
                checkLotMarkingFg();

                if (isMagazineEnd() == false)
                {
                    //排出要求OFFかつ読取完了ONの場合は読取完了を上位から落とす
                    if (LocalPLC.GetBit(STR_ADDRESS_READ_COMPLETE) == Mitsubishi.BIT_ON)
                    {
                        LocalPLC.SetBit(STR_ADDRESS_READ_COMPLETE, 1, Mitsubishi.BIT_OFF);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("", ex);
            }
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="listKey"></param>
        private void workComplete(IPLC plc, int listKey)
        {
            try
            {
                LotMarkData[] markdata1;

                if (isMagazineEnd() == true)
                {
                    markdata1 = GetMarkingData(DEC_ADDRESS_START_MAG);
                    string lotno = getLotNo();
                    Log.ApiLog.Info("FR2:Lotno:" + lotno);


                    foreach (LotMarkData m in markdata1)
                    {
                        //読み込み値が0の場合は読み取り無効で運転されているため登録しない
                        if (m.MarkData != 0)
                        {
                            m.LotNo = lotno;
                            m.Update();
                            Log.ApiLog.Info("FR2:" + m.Row.ToString() + "," + m.MarkData + "," + m.StockerNo.ToString());
                        }
                    }

                    //読み取り完了フラグON
                    LocalPLC.SetBit(STR_ADDRESS_READ_COMPLETE, 1, Mitsubishi.BIT_ON);

                    //安全のため1スキャン分待機
                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("", ex);
            }
        }

        private string getLotNo()
        {
            string adr = convertZRAddress(DEC_ADDRESS_LOTNO);
            string rawMgNo = LocalPLC.GetWord(adr, LOTNO_WORD_LENGTH);
            string lotno = "";
            if (rawMgNo.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
            {
                lotno = rawMgNo.Split(' ')[1];
            }
            else if (rawMgNo.StartsWith(AsmLot.PREFIX_INLINE_LOT))
            {
                lotno = rawMgNo.Split(' ')[1];
            }
            else if (rawMgNo.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
            {
                lotno = Magazine.GetCurrent(rawMgNo.Split(' ')[1]).NascaLotNO;
            }
            else
            {
                lotno = rawMgNo;
            }

            return lotno;
        }

        private bool isMagazineEnd()
        {
            string retv = LocalPLC.GetBit(STR_ADDRESS_OUTPUT_REQ);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }

            return false;
        }

        #region 通常動作は何も行わない
        public override bool ResponseEmptyMagazineRequest()
        {
            return false;
        }

        public override bool IsRequireEmptyMagazine()
        {
            return false;
        }

        public override bool IsRequireOutput()
        {
            return false;
        }
        #endregion


    }
}
