using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class DieShearSampling
    {
        public string TypeCd { get; set; }
        
        public string JudgeWorkCd { get; set; }

        public DieShearSampling(string typeCd, string judgeWorkCd)
        {
            this.TypeCd = typeCd;
            this.JudgeWorkCd = judgeWorkCd;
        }

        public static DieShearSampling getDieShearSampling(string typeCd)
        {
            List<DieShearSampling> dssList = getDieShearSamplingList(typeCd);
            if(dssList.Count == 1)
            {
                return dssList[0];
            }
            else
            {
                return null;
            }
        }

        public static List<DieShearSampling> getDieShearSamplingList(string typeCd)
        {
            List<DieShearSampling> retV = new List<DieShearSampling>();

            using (var armsDB = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                IEnumerable<DieShearSampling> ieDS = from ds in armsDB.TmDieShearSampling
                                                     where ds.delfg == 0
                                                     select new DieShearSampling(
                                                         ds.typecd,
                                                         ds.judgeworkcd
                                                         );

                if(string.IsNullOrWhiteSpace(typeCd) == false)
                {
                    ieDS = ieDS.Where(i => i.TypeCd == typeCd);
                }

                retV.AddRange(ieDS);
            }

            return retV;
        }
    }
}
