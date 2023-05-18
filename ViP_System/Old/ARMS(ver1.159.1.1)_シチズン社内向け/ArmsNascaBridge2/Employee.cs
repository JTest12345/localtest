using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class Employee
    {
        public int EmpCode { get; set; }

        public string EmpName { get; set; }

        public DateTime LastUpdDt { get; set; }
        
        public static bool Import()
        {
            try
            {
                List<Employee> empList = getEmployee(7);
                update(empList);

                return true;
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] Employee Error:" + err.ToString());
                return false;
            }
        }

        private static List<Employee> getEmployee(int agoDay)
        {
            List<Employee> retv = new List<Employee>();

            using (SqlConnection con = new SqlConnection(Config.Settings.ROOTSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                        SELECT empcode, empname_ja, upd_date
                        FROM dbo.RtmEMPLOYEE WITH (nolock)
                        WHERE (void_p = 0) and upd_date >= @LASTUPDDT";

                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = System.DateTime.Now.AddDays(-agoDay);

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Employee emp = new Employee();
                        emp.EmpCode = Convert.ToInt32(rd["empcode"]);
                        emp.EmpName = rd["empname_ja"].ToString().Trim();
                        emp.LastUpdDt = Convert.ToDateTime(rd["upd_date"]);
                        retv.Add(emp);
                    }
                }
            }

            return retv;
        }

        private static void update(List<Employee> updateList)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pCode = cmd.Parameters.Add("@CODE", SqlDbType.Int);
                SqlParameter pName = cmd.Parameters.Add("@NAME", SqlDbType.NVarChar);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

                foreach (Employee emp in updateList)
                {
                    pCode.Value = emp.EmpCode;
                    pName.Value = emp.EmpName;
                    pLastUpdDt.Value = emp.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmEmployee
                            WHERE empcode=@CODE ";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
                        cmd.CommandText = @"
                                INSERT INTO TmEmployee(empcode, empname, lastupddt)
                                VALUES (@CODE, @NAME, @LASTUPDDT) ";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (emp.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                    UPDATE TmEmployee SET empname=@NAME, lastupddt=@LASTUPDDT
                                    WHERE empcode=@CODE ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
