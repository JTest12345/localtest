using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
    public class MachineInfo
    {
        public string NascaPlantCd { get; set; }
        public string WatchingDirectoryPath { get; set; }
        public string ClassCd { get; set; }
        public string ClassNm { get; set; }
        public string MachineNm { get; set; }
        public string PlcIpAddress { get; set; }
        public int PlcPort { get; set; }
        public string MachineGroupCd { get; set; }

        public enum MachineClass
        {
            Wirebonder,
            Inspector,
            Mold,
        }
        public static MachineInfo GetData(string plantcd)
        {
            List<MachineInfo> machines = GetDatas(plantcd, string.Empty);
            if (machines.Count == 0)
            {
                return null;
            }
            else
            {
                return machines.Single();
            }
        }
        public static List<MachineInfo> GetDatas()
        {
            return GetDatas(string.Empty, string.Empty);
        }
        public static List<MachineInfo> GetDatas(string plantcd, string machinegroupcd)
        {
            List<MachineInfo> retv = new List<MachineInfo>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT PlantCD, ClassCD, MachineNM, PlcAddress, PlcPort, WatchingDirectoryPath, GroupCd
								FROM TmMachine WITH (nolock) WHERE DelFG = 0 ";

                if (!string.IsNullOrWhiteSpace(plantcd))
                {
                    sql += " AND (PlantCD = @PlantCD) ";
                    cmd.Parameters.Add("@PlantCD", SqlDbType.NVarChar).Value = plantcd;
                }
                if (!string.IsNullOrWhiteSpace(machinegroupcd))
                {
                    sql += " AND (GroupCd = @GroupCd) ";
                    cmd.Parameters.Add("@GroupCd", SqlDbType.NVarChar).Value = machinegroupcd;
                }

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int OrdPlcIp = rd.GetOrdinal("PlcAddress");
                    int OrdPlcPort = rd.GetOrdinal("PlcPort");

                    while (rd.Read())
                    {
                        MachineInfo m = new MachineInfo();
                        m.ClassCd = rd["ClassCD"].ToString().Trim();
                        m.ClassNm = General.GetMachineClassName(m.ClassCd);
                        m.NascaPlantCd = rd["PlantCD"].ToString().Trim();
                        m.MachineNm = rd["MachineNM"].ToString().Trim();

                        if (rd.IsDBNull(OrdPlcIp))
                        {
                            m.PlcIpAddress = string.Empty;
                        }
                        else
                        {
                            m.PlcIpAddress = rd["PlcAddress"].ToString().Trim();
                        }

                        if (rd.IsDBNull(OrdPlcPort))
                        {
                            m.PlcPort = 0;
                        }
                        else
                        {
                            m.PlcPort = Convert.ToInt32(rd["PlcPort"]);
                        }
                        m.WatchingDirectoryPath = rd["WatchingDirectoryPath"].ToString().Trim();
                        m.MachineGroupCd = rd["GroupCD"].ToString().Trim();
                        retv.Add(m);
                    }
                }
            }

            return retv;
        }

        public static string GetEicsModel(string plantCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.EicsConnectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT Model_NM FROM TmEQUI WITH(nolock) WHERE Equipment_NO = @PlantCd ";

                cmd.Parameters.Add("@PlantCd", SqlDbType.NVarChar).Value = plantCd;

                cmd.CommandText = sql;

                object modelNm = cmd.ExecuteScalar();
                if (modelNm == null)
                {
                    return "";
                }
                else
                {
                    return modelNm.ToString().Trim();
                }
            }

            //		private static MachineInfo getMasterData(int macno)
            //		{
            //			MachineInfo retv = null;

            //			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
            //			using (SqlCommand cmd = con.CreateCommand())
            //			{
            //				con.Open();

            //				string sql = @" SELECT ClassCD, ClassNM, PlantCD, MachineNM, PlcAddress, PlcPort
            //								FROM TmMachine WHERE macno = @MacNo ";

            //				cmd.Parameters.Add("@MacNo", SqlDbType.BigInt).Value = macno;

            //				cmd.CommandText = sql;
            //				using (SqlDataReader rd = cmd.ExecuteReader())
            //				{
            //					while (rd.Read())
            //					{
            //						retv = new MachineInfo();
            //						retv.MacNo = macno;
            //						retv.ClassCd = rd["ClassCD"].ToString().Trim();
            //						retv.ClassNm = rd["ClassNM"].ToString().Trim();
            //						retv.NascaPlantCd = rd["PlantCD"].ToString().Trim();
            //						retv.MachineNm = rd["MachineNM"].ToString().Trim();
            //						retv.PlcIpAddress = rd["PlcAddress"].ToString().Trim();
            //						retv.PlcPort = Convert.ToInt32(rd["PlcPort"]);
            //					}
            //				}
            //			}

            //			return retv;
            //}
        }
    }
}
