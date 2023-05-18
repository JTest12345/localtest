using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class UseHistory
    {
        public UseHistory(Category categorykb, string twodimensionscd, DateTime workdt, string workusercd, bool cleaningfg)
        {
            CategoryKB = categorykb;
            TwoDimensionsCD = twodimensionscd;
            WorkDT = workdt;
            WorkUserCD = workusercd;
            CleaningFG = cleaningfg;
        }

        public enum Category : int
        {
            Magazine = 1,
            Carrier = 2,
            LensTray = 3,
            DicingTray = 4,
        }

        public Category CategoryKB { get; set; }
        public string TwoDimensionsCD { get; set; }
        public DateTime WorkDT { get; set; }
        public string WorkUserCD { get; set; }
        public bool CleaningFG { get; set; }

        public void Insert()
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@CategoryKB", System.Data.SqlDbType.Int).Value = this.CategoryKB;
                    cmd.Parameters.Add("@TwoDimensionsCD", System.Data.SqlDbType.NVarChar).Value = this.TwoDimensionsCD;
                    cmd.Parameters.Add("@WorkDT", System.Data.SqlDbType.DateTime).Value = this.WorkDT;
                    cmd.Parameters.Add("@WorkUserCD", System.Data.SqlDbType.NVarChar).Value = this.WorkUserCD;
                    cmd.Parameters.Add("@CleaningFG", System.Data.SqlDbType.Int).Value = CleaningFG;

                    cmd.CommandText = @" INSERT INTO dbo.TnUseHistory
                                   (categorykb
                                   ,twodimensionscd
                                   ,workdt
                                   ,workusercd
                                   ,cleaningfg)
                             VALUES
                                   (@CategoryKB
                                   ,@TwoDimensionsCD
                                   ,@WorkDT
                                   ,@WorkUserCD
                                   ,@CleaningFG) ";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("仕様履歴登録エラー:" + this.TwoDimensionsCD + " " + ex.Message, ex);
                }
            }
        }
    }
}
