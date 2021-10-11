﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Threading;
using ARMS3.Model.PLC;
using ArmsCejApi.Model;


namespace ARMS3.Model.Machines
{
    /// <summary>
    /// CEJレーザー捺印装置プロトタイプ
    /// </summary>
    class CejlLotMarkingPrototype : MachineBase
    {
        static object lockobj = new object();

        // for debug log
        public static string logfilepath = "C:\\ARMS\\Debug\\cejlm_log.csv";

        protected override void concreteThreadWork()
        {
            workComplete();
        }

        #region workComplete
             
        /// <summary>
        /// 作業完了
        /// </summary>
        public void workComplete()
        {
            if (IsMarkingEnd() == false) return;

            lock (lockobj)
            {
                Log.RBLog.Info("【VIP捺印機】上位通信要求受付・通信開始");

                var msg = string.Empty;

                if (!GetMarkingData(out msg))
                {
                    
                    SetMarkingDataReadEnd(false);
                    Log.RBLog.Error(msg);
                    throw new ApplicationException("上位通信(DB登録)が失敗しています");
                }

                SetMarkingDataReadEnd(true);
                Log.RBLog.Info("【VIP捺印機】上位通信完了");
            }
        }

        #endregion


        #region PLCアドレス定義
        /// <summary>
        /// 1基板毎のアドレスオフセット量
        /// </summary>
        private const int FRAME_ADDR_OFFSET = 100;

        private const string ADDR_HEADER = "DM";
        private const string ADDR_HEADER2 = "B";

        private const int MAG_NO_ADDR_START = 500;
        private const int MAG_NO_ADDR_LENGTH = 9;

        private const int PCBNO_NO_ADDR = 509;

        private const int TYPECD_NO_ADDR = 510;
        private const int TYPECD_NO_ADDR_LENGTH = 10;

        private const int MATERIALCD_NO_ADDR = 520;
        private const int MATERIALCD_NO_ADDR_LENGTH = 10;

        private const int PCBLOT_NO_ADDR = 530;
        private const int PCBLOT_NO_ADDR_LENGTH = 10;

        private const int MARKSTR_NO_ADDR = 540;
        private const int MARKSTR_NO_ADDR_LENGTH = 10;

        private const int WORK_START_YEAR_ADDR = 550;
        private const int WORK_END_YEAR_ADDR = 560;

        private const int MARKING_END_FG_ADDR = 0;
        private const int WORKEND_RETCODE_ADDE = 499;

        private const int MAX_PCB_CT = 50;

        #endregion

        public bool IsMarkingEnd()
        {
            if (this.Plc.GetBit(ADDR_HEADER2 + MARKING_END_FG_ADDR.ToString("0000")) == MachinePLC.BIT_ON)
            {
                return true;
            }

            else return false;
        }


        public void SetMarkingDataReadEnd(bool success)
        {
            if (success)
            {
                Plc.SetWordAsDecimalData(ADDR_HEADER + WORKEND_RETCODE_ADDE.ToString("0000"), 1);
            }

            this.Plc.SetBit(ADDR_HEADER2 + MARKING_END_FG_ADDR.ToString("0000"), 1, PLC.Common.BIT_OFF);

        }


        #region GetMarkingData
        
        private bool GetMarkingData(out string msg)
        {
            MachinePLC plc = MachinePLC.GetInstance();

            List<MarkingData> retv = new List<MarkingData>();

            try
            {
                Plc.SetWordAsDecimalData(ADDR_HEADER + WORKEND_RETCODE_ADDE.ToString("0000"), 0);

                // CejApi
                List<PcbMark.PcbMarkF> pmds = new List<PcbMark.PcbMarkF>();
                PcbMark.PcbMarkM pmm = new PcbMark.PcbMarkM();

                // for debug
                File.AppendAllText(logfilepath, DateTime.Now.ToString("G") + "：捺印データ上位通信テスト\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));

                for (int i = 0; i < MAX_PCB_CT; i++)
                {

                    MarkingData m = new MarkingData();

                    if(m.GetAllData(Plc, i))
                    {
                        if (i==0)
                        {
                            pmm.MagNo = m.MagNo;
                            pmm.TypeCd = m.TypeCd;
                            pmm.MaterialCd = m.MaterialCd;
                            pmm.WorkStDt = m.WorkStartDt;
                            pmm.WorkEndDt = m.WorkEndDt;
                        }
                        else
                        {
                            if (pmm.MagNo != m.MagNo)
                            {
                                msg = $"[PCBNO.{m.MagNo}]PLC内のMagNoに不正があります";
                                return false;
                            }

                            if (pmm.TypeCd != m.TypeCd)
                            {
                                msg = $"[PCBNO.{m.MagNo}]PLC内のTypeCdに不正があります";
                                return false;
                            }

                            if (pmm.MaterialCd != m.MaterialCd)
                            {
                                msg = $"[PCBNO.{m.MagNo}]PLC内のMaterialCdに不正があります";
                                return false;
                            }

                            pmm.WorkEndDt = m.WorkEndDt;
                        }

                        PcbMark.PcbMarkF pmd = new PcbMark.PcbMarkF
                        {
                            PcbNo = m.PcbNo,
                            PcbLotNo = m.PcbLotno,
                            Markstr = m.Markstr
                        };
                        
                        pmds.Add(pmd);


                        // for debug
                        var ret = m.MagNo + ","
                            + m.MagNo + ","
                            + m.PcbNo.ToString() + ","
                            + m.TypeCd + ","
                            + m.MaterialCd + ","
                            + m.PcbLotno + ","
                            + m.Markstr + ","
                            + m.WorkStartDt.ToString("G") + ","
                            + m.WorkEndDt.ToString("G") + ","
                            + DateTime.Now.ToString("G") + "\r\n";
                        File.AppendAllText(logfilepath, ret, System.Text.Encoding.GetEncoding("Shift-Jis"));

                        Log.RBLog.Info($"【基板No.{m.PcbNo}】{ret}");

                        retv.Add(m);
                    }
                }

                pmm.MacNo = this.MacNo;
                pmm.PcbMarks = pmds;

                // DB登録実行
                msg = PcbMark.InsertPcbMark(pmm);
                if ( msg != "")
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }
        #endregion

        public class MarkingData
        {
            public string MagNo { get; set; }
            public int PcbNo { get; set; }
            public string TypeCd { get; set; }
            public string MaterialCd { get; set; }
            public string PcbLotno { get; set; }
            public string Markstr { get; set; }
            public DateTime WorkStartDt { get; set; }
            public DateTime WorkEndDt { get; set; }

            #region 
            public bool GetAllData(IPLC Plc, int i)
            {
                if (!GetMagNo(Plc, i)) return false;
                if (!GetPcbNo(Plc, i)) return false;
                if (!GetTypeCd(Plc, i)) return false;
                if (!GetMaterialCd(Plc, i)) return false;
                if (!GetPcbLotno(Plc, i)) return false;
                if (!GetMarkdata(Plc, i)) return false;
                if (!GetWorkStartDt(Plc, i)) return false;
                if (!GetWorkEndDt(Plc, i)) return false;

                return true;
            }

            public bool GetMagNo(IPLC Plc, int pcbno)
            {
                try
                {
                    MagNo = Plc.GetString(ToAdr(MAG_NO_ADDR_START, pcbno), MAG_NO_ADDR_LENGTH, true);
                    if (string.IsNullOrWhiteSpace(MagNo) == true)
                    {
                        return false;
                    }

                    string[] magStr = MagNo.Split(new[] { ' ', '\r' });
                    //マガジンコードC30_を取り除き
                    MagNo = magStr[1].Trim();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetPcbNo(IPLC Plc, int pcbno)
            {
                try
                {
                    PcbNo = int.Parse(Plc.GetBit(ToAdr(PCBNO_NO_ADDR, pcbno)));
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetTypeCd(IPLC Plc, int pcbno)
            {
                try
                {
                    TypeCd = Plc.GetString(ToAdr(TYPECD_NO_ADDR, pcbno), TYPECD_NO_ADDR_LENGTH, true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetMaterialCd(IPLC Plc, int pcbno)
            {
                try
                {
                    MaterialCd = Plc.GetString(ToAdr(MATERIALCD_NO_ADDR, pcbno), MATERIALCD_NO_ADDR_LENGTH, true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetPcbLotno(IPLC Plc, int pcbno)
            {
                try
                {
                    PcbLotno = Plc.GetString(ToAdr(PCBLOT_NO_ADDR, pcbno), PCBLOT_NO_ADDR_LENGTH, true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetMarkdata(IPLC Plc, int pcbno)
            {
                try
                {
                    Markstr = Plc.GetString(ToAdr(MARKSTR_NO_ADDR, pcbno), MARKSTR_NO_ADDR_LENGTH, true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetWorkStartDt(IPLC Plc, int pcbno)
            {
                try
                {
                    WorkStartDt = GetDateTime(Plc, WORK_START_YEAR_ADDR, pcbno, Plc.IPAddress, Plc.Port);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public bool GetWorkEndDt(IPLC Plc, int pcbno)
            {
                try
                {
                    WorkEndDt = GetDateTime(Plc, WORK_END_YEAR_ADDR, pcbno, Plc.IPAddress, Plc.Port);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public static string ToAdr(int baseAddress, int pcbno)
            {
                return ADDR_HEADER + (baseAddress + (pcbno * FRAME_ADDR_OFFSET)).ToString("0000");
            }

            public static DateTime GetDateTime(IPLC Plc, int yearAddress, int pcbno, string host, int port)
            {
                try
                {
                    var elm = new int[7];
                    for (int num=0; num<7; num++)
                    {
                        string adr = ToAdr(yearAddress + num, pcbno);
                        elm[num] = int.Parse(Plc.GetString(adr, 1, true));
                    }

                    DateTime dt = new DateTime(
                        elm[0] * 100 + elm[1], elm[2], elm[3], elm[4], elm[5], elm[6]);

                    return dt;
                }
                catch (Exception)
                {
                    Log.SysLog.Info("捺印機作業時間変換エラー:" + pcbno.ToString());
                    Log.SysLog.Info("応急処置として現在の時間をセット");
                    return DateTime.Now;
                }

            }

            #endregion

        }

    }
}
