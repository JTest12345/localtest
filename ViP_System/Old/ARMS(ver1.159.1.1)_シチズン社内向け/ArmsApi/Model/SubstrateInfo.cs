using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    /// <summary>
    /// 基板DM-厚み-マッピング-マーキングNoの統合テーブル
    /// </summary>
    public class SubstrateInfo
    {

        #region プロパティ
        public string DataMatrix { get; set; }
        public string thicknessRank { get; set; }
        public string MarkingNo { get; set; }
        public string MappingData { get; set; }

        //最終更新日は呼出用。登録時は無条件で現在時刻で登録。
        public DateTime LastUpdDt { get; set; }
        #endregion

        /// <summary>
        /// 厚みランクを取得（TnSubstrateThicknessRankから移植
        /// </summary>
        /// <param name="datamatrix"></param>
        /// <returns></returns>
        public static string GetThicknessRank(string datamatrix)
        {
            List<string> ranklist = new List<string>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@DataMatrix", System.Data.SqlDbType.NVarChar).Value = datamatrix;

                try
                {
                    con.Open();

                    string sql = @" SELECT thicknessrank
								FROM TnSubstrateInfo WITH(nolock) 
								WHERE datamatrix = @DataMatrix";

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            ranklist.Add(rd["thicknessrank"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("基板DMから厚みランク取得時にエラー発生:基板DM:{0}", datamatrix), ex);
                }
            }

            if (ranklist.Count == 1)
            {
                return ranklist[0];
            }
            else if (ranklist.Count == 0)
            {
                throw new ApplicationException(string.Format("基板DMに厚みランクが紐づいていません。基板DM：{0}", datamatrix));
            }
            else
            {
                throw new ApplicationException(string.Format("基板DMに複数の厚みランクが紐づいています。基板DM：{0}", datamatrix));
            }
        }

        public static SubstrateInfo GetData(string datamatrix)
        {
            SubstrateInfo retv = new SubstrateInfo();

            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                var svrData = db.TnSubstrateInfo.Where(s => s.datamatrix == datamatrix).FirstOrDefault();

                if (svrData != null)
                {
                    retv.DataMatrix = svrData.datamatrix;
                    retv.thicknessRank = svrData.thicknessrank;
                    retv.MarkingNo = svrData.markingno;
                    retv.MappingData = svrData.mappingdata;
                    retv.LastUpdDt = svrData.lastupddt;
                }
            }
            return retv;
        }

        public void InsertUpdate() 
        {
            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                var svrData = db.TnSubstrateInfo.Where(s => s.datamatrix == this.DataMatrix).FirstOrDefault();

                if (svrData != null)
                {
                    svrData.markingno = this.MarkingNo;
                    svrData.lastupddt = DateTime.Now;
                }
                else
                {
                    DataContext.TnSubstrateInfo regDate = new DataContext.TnSubstrateInfo
                    {
                        datamatrix = this.DataMatrix,
                        thicknessrank = this.thicknessRank,
                        markingno = this.MarkingNo,
                        mappingdata = this.MappingData,
                        lastupddt = DateTime.Now
                    };

                    db.TnSubstrateInfo.InsertOnSubmit(regDate);
                }

                db.SubmitChanges();
            }
        }
        
    }
}
