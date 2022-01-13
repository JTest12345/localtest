using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ArmsCejApi.Model;

namespace Ovens
{

    public class TnOvenData
    {
        public string macno { get; set; }
        public DateTime insert_at { get; set; }
        public string insert_at_str { get; set; }
        public string insert_by { get; set; } = "OvenDataHub";
        public string oven_data { get; set; }
        public bool error_flg { get; set; }
        public string error_flg_str { get; set; }

        public TnOvenData(
            DateTime dt,
            string macno,
            string oven_data,
            string error_flg_str
            )
        {
            this.insert_at = dt;
            this.insert_at_str = dt.ToString("G");
            this.macno = macno;
            this.oven_data = oven_data;
            this.error_flg_str = error_flg_str;
            if (error_flg_str == "true")
            {
                error_flg = true;
            }
            else
            {
                error_flg = false;
            }
        }

        public bool InsertTnOvenData_old(ref string msg)
        {
            var SqlStrings = "INSERT INTO[dbo].[TnOvenData] " +
                    "([macno]" +
                    ",[insert_at]" +
                    ",[insert_by]" +
                    ",[oven_data]" +
                    ",[error_flg])" +

                    " VALUES " +
                    $"('{macno}'" +
                    $",'{insert_at_str}'" +
                    $",'{insert_by}'" +
                    $",'{oven_data}'" +
                    $",'{error_flg_str}')";

            return DbControls.execSqlCommand(SqlStrings, ref msg);
        }

        public bool InsertTnOvenData(ref string msg)
        {
            OvenP OP = new OvenP
            {
                MacNo = this.macno,
                Insert_At = this.insert_at,
                Insert_By = this.insert_by,
                Json = this.oven_data,
                Error_Flg = this.error_flg
            };

            try
            {
                msg = "";
                return OP.CreateOvenData();
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
            
        }
    }

    public class DbControls
    {
        public static string SqlConnectionString = "Data Source=CE05394;Initial Catalog=ARMS;Integrated Security=True";

        public static bool execSqlCommand(string SqlStrings, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SqlConnectionString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SqlStrings;
                    con.Open();
                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }
    }

}
