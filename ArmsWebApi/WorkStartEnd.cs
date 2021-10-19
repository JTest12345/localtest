﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsWeb.Models;

namespace ArmsWebApi
{
    public class WorkStartEnd
    {
        public string plantcd;

        public string magno;

        public string lotno;

        public int NewMagFrameQty;

        public string UnloaderMagNo;

        public string empcd;

        public Magazine mag;

        public AsmLot lot;

        public WorkStartEndAltModel wsem;

        public WorkStartEnd(string plantcd, string empcd, string magno, string UnloaderMagNo, int NewMagFrameQty=0)
        {
            this.plantcd = plantcd;
            this.magno = magno;
            this.UnloaderMagNo = UnloaderMagNo;
            this.empcd = empcd;
            this.NewMagFrameQty = NewMagFrameQty; 
        }

        public bool StartEnd(out string msg)
        {
            this.mag = Magazine.GetCurrent(magno);

            if (mag == null)
            {
                msg = "対象の実マガジンは存在しません";
                return false;
            }

            this.lotno = mag.NascaLotNO;
            this.lot = AsmLot.GetAsmLot(lotno);

            if (lot == null)
            {
                msg = "対象のロットは存在しません";
                return false;
            }

            if (plantcd == "")
            {
                msg = "設備コード(plantcd)が必要です";
                return false;
            }

            this.lotno = mag.NascaLotNO;

            WorkStartEndAltModel wsem = new WorkStartEndAltModel(plantcd);


            //開始完了の場合、マガジン入れ替えに伴う1マガジン毎の読み込み判定はスキップする
            if (!string.IsNullOrEmpty(magno))
            {
                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);

                if (mag == null)
                {
                    msg = "ロット情報が存在しません(Inマガジン)";
                    return false;
                }

                bool isOk = wsem.CheckBeforeStart(mag, out msg);

                if (!isOk)
                {
                    return false;
                }

                ///////////////////////////////////
                // 2021.10.12 Junichi Watanabe
                // 開始完了一括の基板数はアウト分の基板数が操作しにくいので見合わせ
                //
                //wsem.LastReadMag = mag;
                //if (string.IsNullOrEmpty(mio.val_in))
                //{
                //    mag.FrameQty = int.Parse(mio.val_in);
                //}
                ///////////////////////////////////

                //開始完了の場合は仮想マガジンも処理しない
                wsem.AddMagazine(mag);

            }
            else
            {
                msg = "IOファイルのINマガジンNoが不正です";
                return false;
            }

            // In/Outのマガジンが違う場合
            if (magno != UnloaderMagNo)
            {

                if (string.IsNullOrEmpty(UnloaderMagNo))
                {
                    msg = "IOファイルのOUTマガジンNoが不正です";
                    return false;
                }

                wsem.UnloaderMagNo = UnloaderMagNo;

            }

            wsem.EmpCd = empcd;

            List<string> msgs;
            bool success = wsem.WorkEnd(out msgs);

            if (!success)
            {
                msg = string.Join(" ", msgs.ToArray());
                return false;
            }

            msg = "";
            return true;
        }
    }
}