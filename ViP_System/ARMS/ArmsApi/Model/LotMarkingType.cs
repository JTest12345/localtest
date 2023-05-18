using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ArmsApi.Model
{
    /// <summary>
    /// ロットマーキング必要性判断テーブル
    /// </summary>
    public class LotMarkingType
    {
        public enum MarkingFg
        {
            Need = 0,
            Forbidden = 1,
        }

        public static MarkingFg GetMarkingFg(string typecd)
        {
            if (string.IsNullOrWhiteSpace(typecd)) return MarkingFg.Need;

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT lotmarkfg FROM TmLotMarkingType WHERE typecd=@TYPECD AND delfg=0";
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

                int? fg = SQLite.ParseNullableInt(cmd.ExecuteScalar());

                if (fg == null)
                {
                    //必要なものはフラグ設定無し
                    return MarkingFg.Need;
                }
                else if (fg == 1)
                {
                    return MarkingFg.Forbidden;
                }
                else
                {
                    throw new ApplicationException("不正なロットマーキング必要性有無フラグ:" + typecd);
                }
            }
        }
    }
}
