using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Threading;
using ARMS3.Model.PLC;


namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 大隆精機製の汎用ロットマーキング装置（マガジンToマガジン）
    /// </summary>
    class GeneralLotMarking : MachineBase
    {
        private const string LOT_MARKING_WORK_CD = "MD0030";
        private const string LOT_MARKING_WORK_CD_2 = "TC0059";
        private const string LOT_MARKING_WORK_CD_3 = "DB0055";

        //public string IpAddress { get; set; }
        //public int Port { get; set; }

        protected override void concreteThreadWork()
        {
            workComplete();

            //開始完了登録されたデータに対して、開始時間をマーキング時間で上書き
			//updateWorkStart();

            //開始時のプログラム名教示
            sendProgram();
        }

        #region workComplete
             
        /// <summary>
        /// 作業完了(高効率)
        /// </summary>
        public void workComplete()
        {
            if (IsMarkingEnd() == false) return;

            List<MarkingData> md = GetMarkingData();

            if (md.Count == 0)
            {
                throw new ApplicationException("[完了処理異常] マガジン全段マーキングデータ無し異常");
            }

            Magazine svrMag = Magazine.GetCurrent(md[0].MagNo);
            if (svrMag == null)
            {
                Log.RBLog.Info("汎用LM装置完了処理でマガジン情報の取得に失敗:" + md[0].MagNo);
                return;
            }
            AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
            Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

            //TnLotmark更新
            MarkingData.UpdateTnLotmark(md, lot.NascaLotNo, p.ProcNo);

            SetMarkingDataReadEnd();

			VirtualMag mag = new VirtualMag();
			mag.MagazineNo = svrMag.MagazineNo;
			mag.LastMagazineNo = svrMag.MagazineNo;
			mag.ProcNo = p.ProcNo;
			mag.WorkStart = md.Select(d => d.WorkStartDt).Min();
			mag.WorkComplete = md.Select(d => d.WorkEndDt).Max();

			this.Enqueue(mag, Station.Unloader);
		}

        #endregion

        private void updateWorkStart()
        {
            Order[] orders = Order.SearchOrder(null, null, null, this.MacNo, false, false, null, null, DateTime.Now.AddDays(-2), null);

            foreach (Order o in orders)
            {
                if (o.WorkStartDt == o.WorkEndDt)
                {
                    var lms = LotMarkData.Search(o.NascaLotNo, null, null, null, null, null);
                    if (lms.Count() >= 1)
                    {
                        DateTime dt = lms.OrderBy(l => l.WorkDt).First().WorkDt;
                        Log.SysLog.Info("汎用LM 開始時間上書き処理：Lotno:" + o.LotNo + " proc:" + o.ProcNo + " " + o.WorkStartDt.ToString() + " → " + dt.ToString()); 
                        o.WorkStartDt = dt;
                        o.DeleteInsert(o.LotNo);
                    }
                }
            }
        }

        #region PLCアドレス定義
        /// <summary>
        /// 1基板毎のアドレスオフセット量
        /// </summary>
        private const int FRAME_ADDR_OFFSET = 200;

        private const string ADDR_HEADER = "ZF";
        private const string ADDR_HEADER2 = "MR";

        private const int MAG_NO_ADDR_START = 71400;
        private const int MAG_NO_ADDR_LENGTH = 15;

        private const int FRAME_NO_ADDR_START1 = 71430;
        private const int FRAME_NO_ADDR_START2 = 71431;

        private const int WORK_START_YEAR_ADDR = 71434;
        private const int WORK_END_YEAR_ADDR = 71452;

        private const int ERROR_START_ADDR = 71458;
        private const int ERROR_ADDR_LENGTH = 20;

        private const int ROW_NO_ADDR = 71429;

        private const int IMAGE_NG_CT_ADDR = 71480;

        private const int IMAGE_NG_BLOCK1_ADDR = 71481;
        private const int IMAGE_NG_XY1_ADDR = 71482;
        private const int IMAGE_NG_ADDR_OFFSET = 2;
        private const int IMAGE_MAX_NG_BLOCK_CT = 50;

        private const int MARKING_END_FG_ADDR = 1000;

        private const int MAX_FRAME_CT = 50;

        private const string PROGRAM_REQ_BIT_ADDR = "MR1003";
        private const string START_MAGNO_WORD_ADDR = "W1000";
        private const int START_MAGNO_WORD_LENGTH = 10;
        private const string START_TEACH_PROGRAM_WORD_ADDR = "W2000";

        #endregion

        public bool IsMarkingEnd()
        {
			//MachinePLC plc = MachinePLC.GetInstance();
			
            if (this.Plc.GetBit(ADDR_HEADER2 + MARKING_END_FG_ADDR.ToString("0000")) == MachinePLC.BIT_ON)
            {
                return true;
            }

            else return false;
        }

        private void sendProgram()
        {
            //MachinePLC plc = MachinePLC.GetInstance();
            string trg = this.Plc.GetBit(PROGRAM_REQ_BIT_ADDR);

            //品種切り替え要求
            if (int.Parse(trg) == 1)
            {
				OutputSysLog(string.Format("[開始処理] 開始 品種要求ON"));

                string magno = this.Plc.GetString(START_MAGNO_WORD_ADDR, START_MAGNO_WORD_LENGTH);
                string[] magStr = magno.Split(new[] { ' ', '\r' });
                if (magno.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    //自動化用の30_を取り除き
                    magno = magStr[1].Trim();
                }
                else if (magno.StartsWith(AsmLot.PREFIX_INLINE_LOT))
                {
                    //高効率用の11_を取り除き
                    magno = magStr[1].Trim();
                }
                else if (magno.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    //高効率用の13_を取り除き
                    magno = magStr[1].Trim();
                }
                else
                {
                    magno = magno.Trim();
                }

				Log.SysLog.Info(string.Format("[開始処理] MagazineNo:{0}", magno));

                var svrMag = Magazine.GetCurrent(magno);
                if (svrMag == null)
                {
                    throw new ApplicationException(string.Format("[開始登録異常] 稼働中マガジンが存在しません。MagazineNo:{0}", magno));
                }
                var lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                Process proc = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (proc == null || proc.IsLotMarking == false)
                {
                        throw new ApplicationException("[開始登録異常] 次作業工程の作業CDが不正です。MagazineNo:" + magno);
                }

                // 下のCommonApi.WorkStart()関数内の次作業開始チェックはロットマーキング作業の次の作業を対象に見ている為、
                // ロットマーキング作業自体を参照する次作業開始チェックを追加。
                if (WorkChecker.IsNextWorkStarted(svrMag.NascaLotNO, svrMag.NowCompProcess))
                {
                    throw new ApplicationException(string.Format("[開始登録異常] 次の作業[{0}]の作業実績が既に登録されています。MagazineNo:{1}", proc.InlineProNM ,magno));
                }

				VirtualMag mag = new VirtualMag();
				mag.MagazineNo = magno;
				mag.LastMagazineNo = magno;
				mag.ProcNo = proc.ProcNo;
				mag.WorkStart = DateTime.Now;

				Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

				ArmsApiResponse workResponse = CommonApi.WorkStart(order);
				if (workResponse.IsError)
				{
					throw new ApplicationException(
						string.Format("[開始登録異常] 理由:{0}", workResponse.Message));
				}
				else
				{
					//品種書き込み
					this.Plc.SetString(START_TEACH_PROGRAM_WORD_ADDR, lot.TypeCd);
					//this.Plc.SetString(START_TEACH_PROGRAM_WORD_ADDR, "NAAA150-AA");
					this.Plc.SetBit(PROGRAM_REQ_BIT_ADDR, 1, PLC.Common.BIT_OFF);

					OutputSysLog(string.Format("[開始処理] 完了 品種書込:{0}", lot.TypeCd));
				}
            }
        }


        public void SetMarkingDataReadEnd()
        {
            //MachinePLC plc = MachinePLC.GetInstance();

            this.Plc.SetBit(ADDR_HEADER2 + MARKING_END_FG_ADDR.ToString("0000"), 1, PLC.Common.BIT_OFF);
        }


        #region GetMarkingData
        
        private List<MarkingData> GetMarkingData()
        {
            MachinePLC plc = MachinePLC.GetInstance();

            List<MarkingData> retv = new List<MarkingData>();

            for(int i = 0; i < MAX_FRAME_CT; i++)
            {

                MarkingData m = new MarkingData();

                string magno = this.Plc.GetString(
                    MarkingData.ToAdr(MAG_NO_ADDR_START, i), MAG_NO_ADDR_LENGTH);

                string[] magStr = magno.Split(new[]{' ', '\r'});
                if (magno.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    //自動化用の30_を取り除き
                    m.MagNo = magStr[1].Trim();
                }
                else if (magno.StartsWith(AsmLot.PREFIX_INLINE_LOT))
                {
                    //高効率用の11_を取り除き
                    m.MagNo = magStr[1].Trim();
                }
                else if (magno.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    //高効率用の13_を取り除き
                    m.MagNo = magStr[1].Trim();
                }
                else
                {
                    m.MagNo = magno.Trim();
                }

                if (string.IsNullOrWhiteSpace(m.MagNo) == true)
                {
                    continue;
                }

                m.FrameNo = Plc.GetWord(
                    MarkingData.ToAdr(FRAME_NO_ADDR_START1, i), 0);

                if (string.IsNullOrWhiteSpace(m.FrameNo) == true)
                {
                    continue;
                }

				//if (m.WorkStartDt.HasValue == false)
				//{
					m.WorkStartDt = MarkingData.GetDateTime(WORK_START_YEAR_ADDR, i, this.Plc.IPAddress, this.Plc.Port);
				//}

				m.WorkEndDt = MarkingData.GetDateTime(WORK_END_YEAR_ADDR, i, this.Plc.IPAddress, this.Plc.Port);

                m.Row = int.Parse(this.Plc.GetBit(
                    MarkingData.ToAdr(ROW_NO_ADDR, i)));

                //m.Errors.AddRange(
                //    plc.GetBit(
                //        MarkingData.ToAdr(ERROR_START_ADDR, i), ERROR_ADDR_LENGTH, IpAddress, Port));


                //for (int j = 0; j < IMAGE_MAX_NG_BLOCK_CT; j++)
                //{
                //    string blk = plc.GetBit(
                //        MarkingData.ToImageAdr(IMAGE_NG_BLOCK1_ADDR, j, i), IpAddress, Port);

                //    string xy = plc.GetBit(
                //        MarkingData.ToImageAdr(IMAGE_NG_XY1_ADDR, j, i), IpAddress, Port);

                //    m.InspectionResult.Add(new ImageInspectionResult() { BlockNo = blk, XY = xy });
                //}

                retv.Add(m);
            }

            return retv;
        }
        #endregion

        public class MarkingData
        {
            public string MagNo { get; set; }
            public string FrameNo { get; set; }
            public int Row { get; set; }
            public DateTime? WorkStartDt { get; set; }
            public DateTime WorkEndDt { get; set; }
            public List<String> Errors { get; set; }
            public int NgCt { get; set; }
            public List<ImageInspectionResult> InspectionResult { get; set; }

            #region 
            public MarkingData()
            {
                Errors = new List<string>();
                InspectionResult = new List<ImageInspectionResult>();
            }


            public static string ToAdr(int baseAddress, int frameNo)
            {
                return ADDR_HEADER
                    + (baseAddress + (frameNo * FRAME_ADDR_OFFSET)).ToString("0000");
            }

            public static string ToImageAdr(int baseAddress, int ngBlockNo, int frameNo)
            {
                return ADDR_HEADER
                    + (baseAddress + (IMAGE_NG_ADDR_OFFSET * ngBlockNo) + (frameNo * FRAME_ADDR_OFFSET)).ToString("0000");
            }

            public static DateTime GetDateTime(int yearAddress, int frameNo, string host, int port)
            {
                string adr = ToAdr(yearAddress, frameNo);

				MachinePLC plc = MachinePLC.GetInstance();
                var elm = plc.GetBit(adr, 6, host, port).Select(d => int.Parse(d)).ToArray();

                try
                {
                    DateTime dt = new DateTime(
                        elm[0] + 2000, elm[1], elm[2], elm[3], elm[4], elm[5]);
                    return dt;
                }
                catch (Exception)
                {
                    Log.SysLog.Info("汎用LM装置作業時間変換エラー:" + frameNo.ToString());
                    Log.SysLog.Info("応急処置として現在の時間をセット");
                    return DateTime.Now;
                }

            }

            public static void UpdateTnLotmark(List<MarkingData> md, string lotno, int procno)
            {
                foreach (var m in md)
                {
                    LotMarkData l = new LotMarkData();
                    l.LotNo = lotno;
                    l.ProcNo = procno;
                    l.StockerNo = 0;
                    l.WorkDt = m.WorkEndDt;
                    l.MarkData = decimal.Parse(m.FrameNo);
                    l.Row = m.Row;
                    l.Update();
                }

            }

            #endregion

            public static DateTime GetWorkStartDt(List<MarkingData> mag)
            {
                return mag.OrderBy(d => d.WorkStartDt.Value).First().WorkStartDt.Value;
            }

            public static DateTime GetWorkEndDt(List<MarkingData> mag)
            {
                return mag.OrderByDescending(d => d.WorkStartDt).First().WorkEndDt;
            }

        }

        public class ImageInspectionResult
        {
            #region
            
            public string BlockNo { get; set; }
            public string XY { get; set; }
            #endregion
        }


    }
}
