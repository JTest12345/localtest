using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class WorkGroup
    {
        public string WorkGroupNm { get; set; }
        public decimal StartHour { get; set; }
        public decimal EndHour { get; set; }

        public static WorkGroup[] SearchWorkGroup()
        {
            List<WorkGroup> retv = new List<WorkGroup>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @"
                       SELECT
                        workgroupnm
                        , starthour
                        , endhour
                        FROM TmWorkGroup
                        WHERE 1=1 ";


                    cmd.CommandText = sql.Replace("\r\n", "");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkGroup w = new WorkGroup();

                            w.WorkGroupNm = SQLite.ParseString(reader["workgroupnm"]);
                            w.StartHour = SQLite.ParseDecimal(reader["starthour"]);
                            w.EndHour = SQLite.ParseDecimal(reader["endhour"]);
                            
                            retv.Add(w);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }

            return retv.ToArray();
        }
    }
}
