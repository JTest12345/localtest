using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 富士情報　新規作成　2022/5
/// 実績収集ロット
/// </summary>
namespace ArmsApi.Model
{
    public class POPLotNo
    {
        public class LotNoHnknArms
        {
            public string FromLotNo { get; set; }
            public string KshCd { get; set; }
            public string ToLotNo { get; set; }
        }
        public static string GetPOPLotNo(string seilotno, string typecd)
        {
            string retv = "";
            //試作用ロットの場合、試作用のロットNoヘッダーを残して採番する
            if (seilotno.StartsWith(Config.TrialProc_CD))
            {
                return Config.TrialProc_CD.PadRight(10,'0');
            }

            //実績収集ロットNo取得(拠点SCMDB)
            retv = GetPOPLotNoSCM(seilotno);

            //取得できなければ実績収集ロットNo取得(実績収集DB)
            if (retv == "") retv = GetPOPLotNoPOP(seilotno);

            if (retv != "") return retv;

            //取得できなければ実績収集システムのストアドプロシージャで採番する
            using (SqlConnection con = new SqlConnection(Config.Settings.POPConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"
                    EXECUTE @RC = dbo.up_pop_OPT_create_SizuLotNo
                    @BHNCD
                   ,@SGYU_BSH_CD
                   ,@KUTI_RYKGU_LOT
                   ,@Ret_SIZU_LOT_NO OUTPUT
                   ,@UPDUSER
                   ,@bDebugMode";

                cmd.CommandText = sql;
                int rc = 0;
                cmd.Parameters.Add("@RC", System.Data.SqlDbType.Int).Value = rc;
                cmd.Parameters.Add("@BHNCD", System.Data.SqlDbType.VarChar).Value = typecd;
                cmd.Parameters.Add("@SGYU_BSH_CD", System.Data.SqlDbType.VarChar).Value = Config.Settings.MANU_SGYUK_CD;
                cmd.Parameters.Add("@KUTI_RYKGU_LOT", System.Data.SqlDbType.VarChar).Value = Config.Settings.POPDCKutiRykgu;
                cmd.Parameters.Add("@Ret_SIZU_LOT_NO", System.Data.SqlDbType.VarChar, 20).Value = retv;
                cmd.Parameters["@Ret_SIZU_LOT_NO"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@UPDUSER", System.Data.SqlDbType.VarChar).Value = Environment.UserName.PadRight(8).Substring(0, 8).Trim();
                cmd.Parameters.Add("@bDebugMode", System.Data.SqlDbType.Int).Value = 0;

                con.Open();
                cmd.Transaction = con.BeginTransaction();

                try
                {
                    cmd.ExecuteNonQuery();

                    if (rc != 0)
                    {
                        throw new Exception("実績収集ロットの採番でエラーが発生しました。");
                    }

                    //ロットNo変更情報登録(実績収集DB)
                    retv = cmd.Parameters["@Ret_SIZU_LOT_NO"].Value.ToString();
                    InsLotNoHnknArms(cmd, seilotno, typecd, retv);

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new Exception("実績収集ロットの取得でエラーが発生しました。" + ex.ToString());
                }
            }
            return retv;
        }
        public static string GetPOPLotNoSCM(string seilotno)
        {
            string retv = "";

            using (SqlConnection con = new SqlConnection(Config.Settings.SCMLocalConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"SELECT JITULOTNO FROM T_SW_QRLOT(nolock) WHERE SISN_LOT_NO = @SISNLOTNO";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@SISNLOTNO", System.Data.SqlDbType.VarChar).Value = seilotno;

                con.Open();

                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            retv = rd["JITULOTNO"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("4Mシステムから実績収集ロットの取得でエラーが発生しました。" + ex.ToString());
                }
            }
            return retv;
        }
        public static string GetPOPLotNoPOP(string seilotno)
        {
            string retv = "";

            using (SqlConnection con = new SqlConnection(Config.Settings.POPConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"SELECT TO_LOT_NO FROM JA_T_LOTNO_HNKN_ARMS(nolock) WHERE FROM_LOT_NO = @FROMLOTNO";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@FROMLOTNO", System.Data.SqlDbType.NVarChar).Value = seilotno;

                con.Open();

                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            retv = rd["TO_LOT_NO"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("実績収集システムから実績収集ロットの取得でエラーが発生しました。" + ex.ToString());
                }
            }
            return retv;
        }
        static void InsLotNoHnknArms(SqlCommand cmd, string seilotno, string typecd, string jitlotno)
        {
            string sql = @"
                    INSERT INTO dbo.JA_T_LOTNO_HNKN_ARMS
                    (FROM_LOT_NO, KSH_CD, TO_LOT_NO, INS_YMD, INS_HMS, UPD_YMD, UPD_HMS, UPD_USR_ID, LGIN_ID, UPD_PG_ID)
                    VALUES(@FROMLOTNO, @KSHCD, @TOLOTNO, @INSYMD, @INSHMS, @UPDYMD, @UPDHMS, @UPDUSRID, @LGINID, @UPDPGID)";

            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@FROMLOTNO", System.Data.SqlDbType.VarChar).Value = seilotno;
            cmd.Parameters.Add("@KSHCD", System.Data.SqlDbType.VarChar).Value = typecd;
            cmd.Parameters.Add("@TOLOTNO", System.Data.SqlDbType.VarChar).Value = jitlotno;
            cmd.Parameters.Add("@INSYMD", System.Data.SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyyMMdd");
            cmd.Parameters.Add("@INSHMS", System.Data.SqlDbType.VarChar).Value = DateTime.Now.ToString("HHmmss");
            cmd.Parameters.Add("@UPDYMD", System.Data.SqlDbType.VarChar).Value = DateTime.Now.ToString("yyyyMMdd");
            cmd.Parameters.Add("@UPDHMS", System.Data.SqlDbType.VarChar).Value = DateTime.Now.ToString("HHmmss");
            cmd.Parameters.Add("@UPDUSRID", System.Data.SqlDbType.VarChar).Value = Environment.UserName.PadRight(8).Substring(0, 8).Trim();
            cmd.Parameters.Add("@LGINID", System.Data.SqlDbType.VarChar).Value = Environment.UserName.PadRight(8).Substring(0, 8).Trim();
            cmd.Parameters.Add("@UPDPGID", System.Data.SqlDbType.VarChar).Value = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.PadRight(30).Substring(0, 30).Trim();
            cmd.Parameters.Add("@bDebugMode", System.Data.SqlDbType.Int).Value = 0;

            cmd.ExecuteNonQuery();
        }
        public static string GetSeiLotNoSCM(string typecd, string jitlotno, bool IsNotFoundOK)
        {
            string retv = "";

            using (SqlConnection con = new SqlConnection(Config.Settings.SCMLocalConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"SELECT SISN_LOT_NO FROM T_SW_QRLOT(nolock) WHERE BHNCD = @BHNCD AND JITULOTNO = @POPLOTNO";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@BHNCD", System.Data.SqlDbType.VarChar).Value = typecd;
                cmd.Parameters.Add("@POPLOTNO", System.Data.SqlDbType.VarChar).Value = jitlotno;

                con.Open();

                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            retv = rd["SISN_LOT_NO"].ToString();
                        }
                        else
                        {
                            if (IsNotFoundOK)
                                retv = "";
                            else
                                throw new Exception("4Mシステムから実績収集ロットの取得ができませんでした。");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("4Mシステムから実績収集ロットの取得でエラーが発生しました。" + ex.ToString());
                }
            }
            return retv;
        }
    }
}
