using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmsApi;
using ArmsApi.Model;
using System.Data;
using System.Data.SqlClient;


namespace ArmsNascaBridge2
{
    class ResinSamplePick
    {
        private class SLESamplingRecord
        {
            public ArmsApi.Model.Resin Resin { get; set; }
            public MachineInfo WorkMachine { get; set; }
            public DateTime InputDt { get; set; }
            public string ResinGpCd { get; set; }
        }

        public static void PickSamples()
        {
            try
            {
                switch (Config.GetLineType)
                {
                    case Config.LineTypes.NEL_SV:
                        PickSLCSamples();
                        break;

                    case Config.LineTypes.NEL_MAP:
                        PickSLESamples();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("先行色調抜き取り処理エラー：" + ex.ToString());
            }
        }

        #region SLE関連処理


        /// <summary>
        /// SLE抜き取り方式
        /// 樹脂Gr・投入日単位
        /// 抜き取り最低ロット数に満たない組み合わせしかない場合も先頭を抜き取り
        /// </summary>
        public static void PickSLESamples()
        {
            try
            {
                List<MachineInfo> moldlist = new List<MachineInfo>();

                #region モールド機のリストを作成

                MachineInfo[] maclist = MachineInfo.SearchMachine(null, null, true, false);
                foreach (MachineInfo mac in maclist)
                {
                    if (mac.ResinCheckFg == true)
                    {
                        moldlist.Add(mac);
                    }
                }
                #endregion

                List<SLESamplingRecord> samplelist = new List<SLESamplingRecord>();
                foreach (MachineInfo mac in moldlist)
                {
                    ArmsApi.Model.Resin[] resins = mac.GetResins(DateTime.Now.AddDays(-2), null);

                    foreach (ArmsApi.Model.Resin resin in resins)
                    {
                        SLESamplingRecord rec = new SLESamplingRecord();
                        rec.Resin = resin;
                        rec.InputDt = resin.InputDt.Value;
                        rec.ResinGpCd = resin.ResinGroupCd;
                        rec.WorkMachine = mac;
                        samplelist.Add(rec);
                    }
                }

                //2ロット以上に関連付いた樹脂Grを先行色調ON
                sampleMapResinsNormal(samplelist);

                //昨日分で未抜き取り樹脂Grの先行色調On
                sampleMapResinsLastDate(samplelist);
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("先行色調確認ロット設定エラー：" + ex.ToString());
            }
        }

        private static void sampleMapResinsNormal(List<SLESamplingRecord> samplelist)
        {
            //昨日分に絞って投入順に並べ見抜き取り樹脂Grは先頭を抜き取り
            IEnumerable<SLESamplingRecord> samples = samplelist.Where(s=> s.InputDt < DateTime.Today).OrderBy(s => s.InputDt);
            foreach (SLESamplingRecord smp in samples)
            {
                bool sampled = isSampled(smp.ResinGpCd, 0, smp.InputDt);
                if (sampled) continue;

                IEnumerable<Order> lotlist = Order.SearchOrder(null, null, null, smp.WorkMachine.MacNo, false, false, null, smp.Resin.RemoveDt, smp.Resin.InputDt, null).OrderBy(o => o.WorkStartDt);

                //マガジン分割対応
                var distinctlist = lotlist.Select(l => l.LotNo).Distinct();

                if (distinctlist.Count() >= 2)
                {
                    //2ロット目を先行色調ONに
                    AsmLot lot = AsmLot.GetAsmLot(distinctlist.ElementAt(1));
                    if (lot == null) continue;
                    lot.IsColorTest = true;
                    lot.Update();
                    Log.SysLog.Info("先行色調確認ロット設定(前日分)：" + lot.NascaLotNo + ":" + smp.ResinGpCd);
                    updateSampled(smp.ResinGpCd, 0, smp.InputDt);
                    break;
                }
            }
        }


        /// <summary>
        /// 前日投入分で抜き取り未の樹脂Grは先頭ロットを抜き取り対象に
        /// </summary>
        /// <param name="samplelist"></param>
        private static void sampleMapResinsLastDate(List<SLESamplingRecord> samplelist)
        {
            //投入順に並べ、指定ロット数以上のアッセンロットに関連付いた最初のロットを色調先行に
            IEnumerable<SLESamplingRecord> samples = samplelist.OrderBy(s => s.InputDt);
            foreach (SLESamplingRecord smp in samples)
            {
                bool sampled = isSampled(smp.ResinGpCd, 0, smp.InputDt);
                if (sampled) continue;

                IEnumerable<Order> lotlist = Order.SearchOrder(null, null, null, smp.WorkMachine.MacNo, false, false, null, smp.Resin.RemoveDt, smp.Resin.InputDt, null).OrderBy(o => o.WorkStartDt);

                //マガジン分割対応
                var distinctlist = lotlist.Select(l => l.LotNo).Distinct();

                if (distinctlist.Count() >= 1)
                {
                    //1ロット目を先行色調ONに
                    AsmLot lot = AsmLot.GetAsmLot(distinctlist.ElementAt(0));
                    if (lot == null) continue;
                    lot.IsColorTest = true;
                    lot.Update();
                    Log.SysLog.Info("先行色調確認ロット設定：" + lot.NascaLotNo + ":" + smp.ResinGpCd);
                    updateSampled(smp.ResinGpCd, 0, smp.InputDt);
                    break;
                }
            }
        }

        
        #endregion

        #region SLC関連処理
        
        /// <summary>
        /// SLC抜き取り方式
        /// （対象）設備・樹脂Gr・投入日単位で抜き取り
        /// </summary>
        public static void PickSLCSamples()
        {
            try
            {
                List<MachineInfo> moldlist = new List<MachineInfo>();
                MachineInfo[] maclist = MachineInfo.SearchMachine(null, null, true, false);

                foreach (MachineInfo mac in maclist)
                {
                    if (mac.ResinCheckFg == true)
                    {
                        if (isTargetPlant(mac.NascaPlantCd))
                        {
                            moldlist.Add(mac);
                        }
                    }
                }

                foreach (MachineInfo mac in moldlist)
                {
                    sampleResins(DateTime.Now.AddDays(-2), mac);
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("先行色調確認ロット設定エラー：" + ex.ToString());
            }
        }


        #region isTargetPlant SV系　対象設備フラグON/OFF
        
        /// <summary>
        /// SV系　対象設備フラグON/OFF
        /// </summary>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        private static bool isTargetPlant(string plantcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = plantcd;

                cmd.CommandText = @"select Plant_CD from ColorPrecessionDB.dbo.TmTARGETPLANT
                    where plant_cd = @PLANTCD and Del_fg = 0";

                object ret = cmd.ExecuteScalar();
                if (ret == null) return false;

                return true;
            }
        }
        #endregion

        private static void sampleResins(DateTime from, MachineInfo mac)
        {
            ArmsApi.Model.Resin[] resins = mac.GetResins(from, null);
            if (resins.Length == 0) return;

            resins = resins.OrderBy(r => r.InputDt).ToArray();

            foreach (ArmsApi.Model.Resin r in resins)
            {
                if (r.InputDt.HasValue == false) continue;
                if (isSampled(r.ResinGroupCd, mac.MacNo, r.InputDt.Value))
                {
                    continue;
                }

                Order[] lotlist = Order.SearchOrder(null, null, null, mac.MacNo, false, false, null, r.RemoveDt, r.InputDt, null);

                if (lotlist.Length >= 1)
                {
                    pickSample(lotlist, r, mac);
                }
            }
        }


        private static void pickSample(Order[] lotlist, ArmsApi.Model.Resin resin, MachineInfo mac)
        {
            //同一樹脂投入中はタイプの変更なしの想定で先頭ロットのタイプ名を取得
            AsmLot firstlot = AsmLot.GetAsmLot(lotlist[0].LotNo);

            int minlotct;
            int[] seqList;
            getSamplingSettings(firstlot.TypeCd, out minlotct, out seqList);

            //投入順に並び替え
            lotlist = lotlist.OrderBy(o => o.WorkStartDt).ToArray();

            //マガジン分割対応
            string[] distinctList = lotlist.Select(l => l.LotNo).Distinct().ToArray();

            //製造ロットが最低必要数より少ない調合結果IDは抜き取り対象外
            if (distinctList.Length < minlotct)
            {
                return;
            }

            foreach (int seq in seqList)
            {
                AsmLot lot = AsmLot.GetAsmLot(distinctList[seq - 1]);
                lot.IsColorTest = true;
                lot.Update();
                Log.SysLog.Info("先行色調確認ロット設定：" + lot.NascaLotNo + ":" + resin.ResinGroupCd);
            }
            updateSampled(resin.ResinGroupCd, mac.MacNo, resin.InputDt.Value);
        }

        #endregion

        private static void getSamplingSettings(string typecd, out int minLotCt, out int[] sampleLotSeq)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@TYPECD", SqlDbType.VarChar).Value = typecd;

                cmd.CommandText = @"
                    select
                     top 1 lot_kb
                    , targetlot_kb
                    from ColorPrecessionDB.dbo.tmtypelotkb
                    where @TYPECD LIKE type_nm
                    and del_fg = 0
                    order by order_no";

                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        minLotCt = Convert.ToInt32(rd["lot_kb"]);
                        string seqStr = rd["targetlot_kb"].ToString().Trim();

                        List<int> seqList = new List<int>();
                        foreach (string num in seqStr.Split(','))
                        {
                            seqList.Add(int.Parse(num));
                        }
                        sampleLotSeq = seqList.ToArray();
                        return;
                    }
                }
            }

            throw new ApplicationException("先行色調抜き取りマスタ取得エラー:" + typecd);
        }

        private static void updateSampled(string resingpcd, int macno, DateTime dt)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = resingpcd;
                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
                cmd.Parameters.Add("@SAMPLEDT", SqlDbType.BigInt).Value = int.Parse(dt.ToString("yyyyMMdd"));
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                cmd.CommandText = @"
                    INSERT INTO TnResinGrSample(resingpcd, macno, sampledt,  lastupddt)
                    VALUES (@RESINGPCD, @MACNO, @SAMPLEDT, @LASTUPDDT)";

                con.Open();
                object retv = cmd.ExecuteNonQuery();
            }
        }

        private static bool isSampled(string resingpcd, int macno, DateTime dt)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = resingpcd;
                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
                cmd.Parameters.Add("@SAMPLEDT", SqlDbType.BigInt).Value = int.Parse(dt.ToString("yyyyMMdd"));

                cmd.CommandText = @"
                    SELECT lastupddt FROM TnResinGrSample
                    WHERE resingpcd = @RESINGPCD AND macno = @MACNO AND sampledt = @SAMPLEDT";

                con.Open();
                object retv = cmd.ExecuteScalar();

                if (retv != null)
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
}
