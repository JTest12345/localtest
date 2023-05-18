using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class PSTeserMaster
    {
        /// <summary>
        /// 強度試験マスタ（装置ごとに強度試験対象時間が設定されている）
        /// </summary>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        public static decimal? GetSettingTime(string plantcd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var datas = armsDB.TmPSTeser.Where(r => r.delfg == 0 && r.plantcd == plantcd);

                if (datas != null && datas.Count() > 0)
                    return datas.First().settingtime;
                else
                    return null;
            }
        }
    }
}
