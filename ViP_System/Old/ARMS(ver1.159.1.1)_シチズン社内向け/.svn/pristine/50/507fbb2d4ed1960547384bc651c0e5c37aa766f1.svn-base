using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;



namespace ArmsApi.Model
{
    public class LineconfigSignalDisplay
    {
        public LineconfigSignalDisplay()
        {

        }

        public static List<DataContext.TmLineconfigSignalDisplay> GetLineconfigSignalDisplay(IEnumerable<string> flownms)
        {
            List<DataContext.TmLineconfigSignalDisplay> retv;
            using (DataContext.ARMSDataContext armsDB = new DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                // Where句は 変数 query のデータ型を Table型 → IQueryable型 に変換するために記載
                var query = armsDB.TmLineconfigSignalDisplay.Where(l => true);

                if (flownms?.Any() == true)
                {
                    query = query.Where(q => flownms.Contains(q.flownm));
                }
                retv = query.ToList();                
            }

            return retv;
        }
    }
}
