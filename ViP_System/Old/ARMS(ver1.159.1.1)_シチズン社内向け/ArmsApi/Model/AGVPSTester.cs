using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class AGVPSTester
    {
        /// <summary>
        /// 既に試験対象に選出されているか
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        public static bool ExistTestLot(string lotno, string plantcd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var datas = armsDB.TnAGVPSTester.Where(r => r.lotno == lotno);
                if (string.IsNullOrWhiteSpace(plantcd) == false)
                {
                    datas = datas.Where(r => r.plantcd == plantcd);
                }

                if (datas != null && datas.Count() != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 指定期間に対象設備で強度試験対象になったロットを取得
        /// </summary>
        /// <param name="plantcd"></param>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public static List<string> GetTestLot(string plantcd, DateTime fromdt, DateTime todt)
        {
            List<string> retv = new List<string>();

            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var datas = armsDB.TnAGVPSTester.Where(r => r.plantcd == plantcd && r.testdt >= fromdt && r.testdt < todt);

                foreach (var d in datas)
                {
                    retv.Add(d.lotno);
                }
            }

            return retv;
        }

        /// <summary>
        /// 対象設備で強度試験対象になった最新のロットを取得
        /// </summary>
        /// <param name="nascaPlantCd"></param>
        /// <returns></returns>
        public static string GetTestLot(string plantcd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var datas = armsDB.TnAGVPSTester.Where(r => r.plantcd == plantcd);

                if (datas != null && datas.Count() > 0)
                {
                    return datas.OrderByDescending(r => r.testdt).First().lotno;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void Insert(string plantcd, DateTime testdt, string lotno)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                DataContext.TnAGVPSTester d = new DataContext.TnAGVPSTester();
                armsDB.TnAGVPSTester.InsertOnSubmit(d);

                d.plantcd = plantcd;
                d.testdt = testdt;
                d.lotno = lotno;
                armsDB.SubmitChanges();
            }
        }
    }
}
