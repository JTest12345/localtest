using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
	public class Software
	{
		public enum Soft 
		{
			Arms,
			Maintenance,
			NascaBridge,
            ResinMixDirection,
        }

        public static bool CanActivateSoftware(Soft soft)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = " SELECT PcNM FROM TmServer WITH(nolock) WHERE delfg = 0 ";

				if (soft == Soft.Arms)
				{
					sql += " AND IsInstalledArms = 1 ";
				}

				if (soft == Soft.Maintenance)
				{
					sql += " AND IsInstalledMaintenance = 1 ";
				}

				if (soft == Soft.NascaBridge)
				{
					sql += " AND IsInstalledNascaBridge = 1 ";
				}

				cmd.CommandText = sql;

				List<string> sevList = new List<string>();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						sevList.Add(rd["PcNm"].ToString().Trim());
					}
				}

				/*if (sevList.Count == 0) 
				{
					throw new ApplicationException(
						string.Format("{0}をインストールしたPCが存在しません。システム管理者に連絡して、マスタ(TmServer)を確認して下さい。", soft.ToString()));
				}*/

				string hostPcNm = System.Net.Dns.GetHostName();

				if (sevList.Exists(s => s == hostPcNm))
				{
					return true;
				}
				else 
				{
					return false;
				}
			}

			return true;
		}
        
        public static void UpdateArmsSystemVersion(string versionNm, bool configUpdatefg, bool lineConfigUpdatefg)
        {
            string hostPcNm = System.Net.Dns.GetHostName();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"
                    UPDATE TmServer 
                    SET ArmsSystemVersion = @SYSTEMVERSION
                    {0}
                    WHERE PcNm = @PCNM AND delfg = 0";

                string sqladd = "";
                if (configUpdatefg == true)
                    sqladd += ", ConfigUpdateDt = @CONFIGUPDATEDT";
                if (lineConfigUpdatefg == true)
                    sqladd += ", LineConfigUpdateDt = @LINECONFIGUPDATEDT";
                
                sql = string.Format(sql, sqladd);

                cmd.CommandText = sql;

                cmd.Parameters.Add("@PCNM", SqlDbType.NVarChar).Value = hostPcNm;
                cmd.Parameters.Add("@SYSTEMVERSION", SqlDbType.NVarChar).Value = versionNm;

                if (configUpdatefg == true)
                    cmd.Parameters.Add("@CONFIGUPDATEDT", SqlDbType.DateTime).Value = DateTime.Now;
                if (lineConfigUpdatefg == true)
                    cmd.Parameters.Add("@LINECONFIGUPDATEDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("Armsシステム情報更新エラー", ex);
                }
            }
        }

        public static void UpdateArmsMaintenanceSystemVersion(string versionNm, bool updatefg)
        {
            string hostPcNm = System.Net.Dns.GetHostName();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"
                    UPDATE TmServer 
                    SET ArmsMaintenanceSystemVersion = @SYSTEMVERSION
                    {0}
                    WHERE PcNm = @PCNM AND delfg = 0";

                if (updatefg == true)
                    sql = string.Format(sql, ", ConfigUpdateDt = @CONFIGUPDATEDT");
                else
                    sql = string.Format(sql, "");

                cmd.CommandText = sql;

                cmd.Parameters.Add("@PCNM", SqlDbType.NVarChar).Value = hostPcNm;
                cmd.Parameters.Add("@SYSTEMVERSION", SqlDbType.NVarChar).Value = versionNm;

                if (updatefg == true)
                    cmd.Parameters.Add("@CONFIGUPDATEDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("ArmsMaintenanceシステム情報更新エラー", ex);
                }
            }
        }

        public static void UpdateArmsMonitorSystemVersion(string versionNm, bool updatefg)
        {
            string hostPcNm = System.Net.Dns.GetHostName();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"
                    UPDATE TmServer 
                    SET ArmsMonitorSystemVersion = @SYSTEMVERSION
                    {0}
                    WHERE PcNm = @PCNM AND delfg = 0";

                if (updatefg == true)
                    sql = string.Format(sql, ", ConfigUpdateDt = @CONFIGUPDATEDT");
                else
                    sql = string.Format(sql, "");

                cmd.CommandText = sql;

                cmd.Parameters.Add("@PCNM", SqlDbType.NVarChar).Value = hostPcNm;
                cmd.Parameters.Add("@SYSTEMVERSION", SqlDbType.NVarChar).Value = versionNm;

                if (updatefg == true)
                    cmd.Parameters.Add("@CONFIGUPDATEDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("ArmsMonitorシステム情報更新エラー", ex);
                }
            }
        }
    }
}
