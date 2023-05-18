using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class WorkEngagementPeriod
    {
        public string EmpCd { get; set; }
        public int Process { get; set; }
        public DateTime StartDT { get; set; }
        public DateTime EndDT { get; set; }

        public void Update()
        {
            Update(SQLite.ConStr);
        }

        public void Update(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = this.EmpCd;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.Process;
                cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = this.StartDT;
                cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = this.EndDT;
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();

                    //新規Insert
                    cmd.CommandText = @"SELECT empcd FROM TnWorkEngagementPeriod WHERE empcd=@EMPCD AND procno=@PROCNO AND startdt=@STARTDT AND enddt=@ENDDT";

                    object workPeriod = cmd.ExecuteScalar();

                    if (workPeriod == null)
                    {
                        #region Insertコマンド
                        cmd.CommandText = @"
                            INSERT INTO TnWorkEngagementPeriod
                              ( 
                                empcd , 
                                procno , 
                                startdt , 
                                enddt , 
                                lastupddt 
                              ) 
                            VALUES 
                              ( 
                                @EMPCD , 
                                @PROCNO , 
                                @STARTDT , 
                                @ENDDT , 
                                @LASTUPDDT 
                              )";
                        #endregion
                    }
                    else
                    {
                        #region Updateコマンド

                        cmd.CommandText = @"
                            UPDATE TnWorkEngagementPeriod
                            SET 
                              lastupddt = @LASTUPDDT 
                            WHERE 
                              empcd=@EMPCD AND procno=@PROCNO AND startdt=@STARTDT AND enddt=@ENDDT";
                        #endregion
                    }

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("作業者登録更新エラー:" + ex.ToString());
                }
            }
        }
    }
}
