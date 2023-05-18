using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
    /// <summary>
    /// 蛍光体シート色調測定結果
    /// </summary>
    public class PsMeasureResult
    {
        public class TraySummary
        {
            public string TrayNo { get; set; }
            public decimal MeasureAveVAL { get; set; }            
            public bool IsBetaSialonSheet { get; set; }
            public bool IsKsfSheet { get; set; }

            /// <summary>
            /// 段数
            /// </summary>
            public int RowNo { get; set; }

            public static TraySummary GetCurrentAverage(string trayNo, int qcParamNo, int hostLineCd)
            {
                TraySummary retv = new TraySummary();
                retv.TrayNo = trayNo;

                List<PsMeasureResult> list = PsMeasureResult.GetData(trayNo, qcParamNo, hostLineCd);
                if (list.Any() == false)
                {
                    return null;
                }
                else
                {
                    retv.MeasureAveVAL = list.Average(d => d.MeasureAveVAL);

                    // 下記判断はせずにクリア+KFSのときでも組み合わせを送るようにする
                    
                    // TODO NASCAの作業順でシート貼付作業2だったらtrueにするように変更?
                    if (list.First().TypeCd.Split('-')[1].Substring(0, 1) == "G")
                        retv.IsBetaSialonSheet = true;

                    if (list.First().TypeCd.Split('-')[1].Substring(0, 1) == "R")
                        retv.IsKsfSheet = true;
                }

                return retv;
            }
        }

        public class SheetSummary
        {
            private const int MaxSheetCount = 12;

            public int SheetNo { get; set; }
            public string TrayNo { get; set; }
            public decimal MeasureAveVAL { get; set; }
            public string LotNo { get; set; }

            public static List<SheetSummary> GetCurrentAverage(string trayNo, int qcParamNo, int hostLineCd)
            {
                List<SheetSummary> retv = new List<SheetSummary>();

                List<PsMeasureResult> list = PsMeasureResult.GetData(trayNo, qcParamNo, hostLineCd);
                foreach(PsMeasureResult l in list)
                {
                    SheetSummary s = new SheetSummary();
                    s.TrayNo = l.TrayNo;
                    s.SheetNo = l.SheetNo;
                    s.MeasureAveVAL = l.MeasureAveVAL;
                    s.LotNo = l.LotNo;

                    retv.Add(s);
                }

                // 測定値が存在しないシートがある場合、シート順を指示できなくなるため、０埋めデータを作成する
                for (int i = 1; i <= MaxSheetCount; i++)
                {
                    if (retv.Where(r => r.SheetNo == i).Any() == true)
                    {
                        continue;
                    }

                    SheetSummary s = new SheetSummary();
                    s.TrayNo = trayNo;
                    s.SheetNo = i;
                    s.MeasureAveVAL = 0;
                    s.LotNo = "";

                    retv.Add(s);
                }

                return retv;
            }

        }

        public string TrayNo { get; set; }
        public int SheetNo { get; set; }
        public string TypeCd { get; set; }
        public decimal MeasureAveVAL { get; set; }
        public string LotNo { get; set; }

        private static List<PsMeasureResult> GetData(string trayNo, int qcParamNo, int hostLineCd)
        {
            List<PsMeasureResult> retv = new List<PsMeasureResult>();

            using (DataContext.EICSDataContext db = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, hostLineCd)))
            {
                var list = db.TnPsMeasureResult.Where(p => p.New_FG == true && p.Tray_NO == trayNo && p.QcParam_NO == qcParamNo);
                foreach (var l in list)
                {
                    PsMeasureResult p = new PsMeasureResult();
                    p.TrayNo = l.Tray_NO;
                    p.SheetNo = l.Sheet_NO;
                    p.TypeCd = l.Type_CD;
                    p.MeasureAveVAL = l.MeasureAve_VAL;
                    p.LotNo = l.Lot_NO;
                    retv.Add(p);
                }
            }

            return retv;
        }
    }
}
