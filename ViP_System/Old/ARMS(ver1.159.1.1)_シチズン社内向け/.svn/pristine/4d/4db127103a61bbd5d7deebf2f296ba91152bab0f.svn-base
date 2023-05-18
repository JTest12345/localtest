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
    public class BomD
    {
        /// <summary>
        /// 製品型番
        /// </summary>
        public string TypeCd { get; set; }

        /// <summary>
        /// 原材料品目CD
        /// </summary>
        public string MaterialCd { get; set; }

        /// <summary>
        /// BOM構成Ver
        /// </summary>
        public string StructureVr { get; set; }

        /// <summary>
        /// 構成数
        /// </summary>
        public int StructureCt { get; set; }

        public bool DelFg { get; set; }

        public DateTime LastUpdDt { get; set; }

        public static bool Import()
        {
            try
            {
                string[] typeList = Process.GetWorkFlowTypeList();
                foreach (string type in typeList)
                {
                    List<BomD> nascaList = getNascaData(type);

                    updateInsert(nascaList);
                }

                return true;
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] BomD Error:" + err.ToString());
                return false;
            }
        }

        private static List<BomD> getNascaData(string typeCd)
        {
            List<BomD> retv = new List<BomD>(); 

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT RTMBOMD.material_cd, structure_vr, cmaterial_cd, pmtralstr_pc, RTMBOMD.del_fg, RTMBOMD.lastupd_dt
                     FROM ROOTSDB.dbo.RTMBOMD AS RTMBOMD WITH (nolock) 
                     INNER JOIN ROOTSDB.dbo.RTMMAT AS RTMMAT WITH (nolock) ON 
                     RTMBOMD.cmaterial_cd = RTMMAT.material_cd                     
                     INNER JOIN ROOTSDB.dbo.RTMMCONV AS RTMMCONV WITH (nolock) ON RTMBOMD.material_cd = RTMMCONV.material_cd 
                     INNER JOIN ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RTMMCONV.workcond_cd = RTMNFORMGROUP.fcode
                     WHERE RTMMCONV.mtralbase_cd = @TypeCd AND (RTMMAT.dicewafer_kb IN ('2', '8', '16')) 
                     AND RTMNFORMGROUP.fgroupclass_cd = @FGROUPCLSCD ";

                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;
                cmd.Parameters.Add("@TypeCd", SqlDbType.NVarChar).Value = typeCd;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        BomD b = new BomD();

                        b.TypeCd = rd["material_cd"].ToString().Trim().Split('.')?[0];
                        b.StructureVr = rd["structure_vr"].ToString().Trim();
                        b.MaterialCd = rd["cmaterial_cd"].ToString().Trim();
                        b.StructureCt = Convert.ToInt32(rd["pmtralstr_pc"]);
                        b.DelFg = Convert.ToBoolean(Convert.ToInt16(rd["del_fg"]));
                        b.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);

                        retv.Add(b);
                    }
                }
            }

            return retv;
        }

        private static void updateInsert(List<BomD> nascaList)
        {
            if (nascaList.Count == 0)
                return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TypeCd", SqlDbType.NVarChar);
                SqlParameter pMaterialCd = cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar);
                SqlParameter pStructureVr = cmd.Parameters.Add("@StructureVr", SqlDbType.NVarChar);
                SqlParameter pStructureCt = cmd.Parameters.Add("@StructureCt", SqlDbType.Int);
                SqlParameter pDelFg = cmd.Parameters.Add("@DelFg", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime);

                con.Open();

                foreach (BomD nasca in nascaList)
                {
                    pType.Value = nasca.TypeCd;
                    pMaterialCd.Value = nasca.MaterialCd;
                    pStructureVr.Value = nasca.StructureVr;
                    pStructureCt.Value = nasca.StructureCt;
                    pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
                    pLastUpdDt.Value = nasca.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmBomD
                            WHERE typecd=@TypeCd AND materialcd=@MaterialCd AND structurevr=@StructureVr ";

                    object objlastupd = cmd.ExecuteScalar();
                    if (objlastupd == null)
                    {
                        if (nasca.DelFg) continue;

                        cmd.CommandText = @"
                                INSERT INTO TmBomD(typecd, materialcd, structurevr, pmtralstrpc, delfg, lastupddt)
                                VALUES (@TypeCd, @MaterialCd, @StructureVr, @StructureCt, @DelFg, @LastUpdDt);";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (nasca.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                      UPDATE TmBomD SET pmtralstrpc = @StructureCt, @delfg=@DelFg, lastupddt=@LastUpdDt
                                      WHERE typecd = @TypeCd AND materialcd = @MaterialCd AND structurevr = @StructureVr ";

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
