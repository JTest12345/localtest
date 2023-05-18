using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class GnlMaster
    {
		/// <summary>
		/// ダイシェア抜き取り対象の型番リスト
		/// </summary>
		public const string TID_DIESHEARSAMPLETYPE = "DIESHEARSAMPLETYPE";
		
		/// <summary>
		/// NASCAから試験数を取り込む特性CDリスト
		/// </summary>
		public const string TID_TESTSAMPLELOTCHAR = "TESTSAMPLELOTCHAR";

		/// <summary>
		/// ライフ試験結果で"良"扱いにする特性リスト値のリスト
		/// </summary>
		public const string TID_LIFETESTGOODRESULT = "LIFETESTGOODRESULT";

        /// <summary>
        /// WBスキップ数の集計対象となるMMファイル内のエラーコード
        /// </summary>
        public const string TID_WBSKIPERROR = "WIREMMFILESKIPERRORCODELIST";

        /// <summary>
        /// ダイシェア抜き取り判定工程-優先度
        /// </summary>
        public const string TID_DIESHEARSAMPLEJUDGEWORKCD = "DIESHEARSAMPLEJUDGEWORKCD";

        public const string TID_DEFECTNOTINCLUDE = "DEFECTNOTINCLUDE";

        /// <summary>
        /// 蛍光体ブロックの品目グループコード
        /// </summary>
        public const string TID_PHOSPHORSHEETMATGROUPCD = "PHOSPHORSHEETMATEGROUPCD";

        /// <summary>
        /// 蛍光体ブロックミキシング時の作業と特性コードの紐付
        /// </summary>
        public const string TID_PBMIXINGWORKLIST = "PBMIXINGWORKLIST";

        /// <summary>
        /// 装置グループ(ライン)
        /// </summary>
        public const string TID_MACGROUP = "macgroup";

        /// <summary>
        /// キャリア用作業情報の情報種類
        /// </summary>
        public const string TID_CARRIERWORKINFOCD = "CARRIERWORKINFO";

        public string Tid { get; set; }
        public string Code { get; set; }
        public string Val { get; set; }
        public string Val2 { get; set; }

        public static GnlMaster[] Search(string tid, string code, string val, string val2)
        {
            List<GnlMaster> retv = new List<GnlMaster>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"SELECT tid, code, val, val2 FROM TmGeneral
                    WHERE delfg=0";

                if (tid != null)
                {
                    cmd.CommandText += " AND tid=@TID";
                    cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = tid;
                }

                if (code != null)
                {
                    cmd.CommandText += " AND code=@CODE";
                    cmd.Parameters.Add("@CODE", SqlDbType.NVarChar).Value = code;
                }

                if (val != null)
                {
                    cmd.CommandText += " AND val=@VAL";
                    cmd.Parameters.Add("@VAL", SqlDbType.NVarChar).Value = val;
                }

                if (val2 != null)
                {
                    cmd.CommandText += " AND val2=@VAL2";
                    cmd.Parameters.Add("@VAL2", SqlDbType.NVarChar).Value = val2;
                }

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        GnlMaster gnl = new GnlMaster();
                        gnl.Tid = rd["tid"].ToString();
                        gnl.Code = rd["code"].ToString();
                        gnl.Val = rd["val"].ToString();
                        gnl.Val2 = rd["val2"].ToString();
                        retv.Add(gnl);
                    }
                }
            }

            return retv.ToArray();
        }
		public static GnlMaster[] Search(string tid) 
		{
			return Search(tid, null, null, null);
		}
		public static GnlMaster[] Search(string tid, string code)
		{
			return Search(tid, code, null, null);
		}

		public static GnlMaster[] GetDieshearSampleType()
		{
			return Search(TID_DIESHEARSAMPLETYPE);
		}

		public static GnlMaster[] GetTestSampleLotChar()
		{
			return Search(TID_TESTSAMPLELOTCHAR);
		}

		public static GnlMaster[] GetLifeTestGoodResult(string beforelifetestcondcd) 
		{
			return Search(TID_LIFETESTGOODRESULT, beforelifetestcondcd);
		}

        public static GnlMaster[] GetWbSkipError()
        {
            return Search(TID_WBSKIPERROR);
        }

        public static GnlMaster[] GetDieshearSampleJudgeWorkCd()
        {
            return Search(TID_DIESHEARSAMPLEJUDGEWORKCD);
        }

        public static GnlMaster[] GetDefectNotInclude()
        {
            return Search(TID_DEFECTNOTINCLUDE);
        }

        public static GnlMaster[] GetMacGroup()
        {
            return Search(TID_MACGROUP);
        }

        public static GnlMaster[] GetPhosphorSheetMateGroupCd()
        {
            return Search(TID_PHOSPHORSHEETMATGROUPCD);
        }

        public static GnlMaster[] GetPBMixingWorkList()
        {
            return Search(TID_PBMIXINGWORKLIST);
        }
    }
}
