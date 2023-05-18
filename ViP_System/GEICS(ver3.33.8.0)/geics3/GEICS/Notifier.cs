using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace GEICS
{
    class Notifier
    {
        private int nQcParamNo;
        private SortedList<DateTime, QCLogData> cndData;
        //private SortedList<string, double> cndData;
        //private string typeCd;


        //1  	管理限界外  領域(※1)Aを超えている
        //2 	連   	    連続する9点が中心線に対して同じ側にある
        //3 	上昇・下降 	連続する6点が増加，又は減少している

        //今回採用するのは上記3つの判定のみ。JIS規格では以下も定義されている。
        //傾向を見ながら採用の有無を決定する。
        //4 	交互増減 	14の点が交互に増減している
        //5 	2σ外    	連続する3点中，2点が領域A又はそれを超えた領域にある（＞2σ）
        //6 	1σ外 	    連続する5点中，4点が領域B又はそれを超えた領域にある（＞1σ）
        //7 	中心化傾向 	連続する15点が領域Cに存在する（≦1σ）
        //8 	連続1σ外 	連続する8点が領域Cを超えた領域にある（＞1σ）


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="equipmentNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="cndData"></param>
        //public Notifier(string sQcParamNo, string typeCd, SortedList<string, double> cndData)
        //public Notifier(int nQcParamNo, SortedList<string, double> cndData)
        public Notifier(int nQcParamNo, SortedList<DateTime, QCLogData> cndData)
        {
            this.nQcParamNo = nQcParamNo;
            this.cndData = cndData;
            //this.typeCd = typeCd;
        }

        /*
        public Notifier(string sQcParamNo, SortedList<string, string> cndData)
        {
            this.sQcParamNo = sQcParamNo;
            this.cndData = cndData;
            //this.typeCd = typeCd;
        }
        */
        /// <summary>
        /// 異常通知を行う
        /// </summary>
        /// <param name="equipmentNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="cndData"></param>
        public void Notify()
        {
            if (cndData.Count <= 30)
            {
                return;
            }

            double avg = calcAvg(cndData);
            if (avg == 0.0)
            {
                return;
            }
            double sigma = calcSigma(cndData, avg);

            for (int i = cndData.Count-1; i > 0; i--)
            {
                //check1(i, avg, sigma);
                //check2(i, avg);
                //check3(i);
            }
        }

        /// <summary>
        /// 管理限界外  領域(※1)Aを超えている
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool check1(int i, double avg, double sigma)
        {

            double ucl = avg +  (3 * sigma);
            double lcl = avg -  (3 * sigma);

            if (cndData.Values[i].Data > ucl)
            {
                recordNotify(i, "制約1：管理限界外");
                return true;
            }
            
            if (cndData.Values[i].Data < lcl)
            {
                recordNotify(i, "制約1：管理限界外");
                return true;
            }

            return false;
        }


        /// <summary>
        /// 連続する9点が中心線に対して同じ側にある
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        private bool check2(int i, double avg)
        {
            int points = 9;

            if ((i - points) < 0)
            {
                return false;
            }

            double lastdata = cndData.Values[i].Data;
            double side = 1.0;

            if (lastdata < avg)
            {
                side = -1.0;
            }

            for (int j = 0; j < points; j++)
            {
                double judge = side * (cndData.Values[i - j].Data - avg);

                if (judge < 0)
                {
                    return false;
                }
            }

            recordNotify(i, "制約2：連続9点同側");
            return true;
        }

        /// <summary>
        /// 上昇・下降 	連続する6点が増加，又は減少している
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        /// <returns></returns>
        private bool check3(int i)
        {
            int points = 6;

            if ((i - points) < 0)
            {
                return false;
            }

            bool isPositive;
            double lastdata = cndData.Values[i].Data;
            double nextdata = cndData.Values[i -1].Data;
            if (lastdata < nextdata)
            {
                isPositive = false;
            }
            else
            {
                isPositive = true;
            }
            //6点の増加/減少だが、実際は5連続増加/下降であれば、NG
            for (int j = 0; j < points - 1; j++) //6点チェックなので比較は5回
            {
                lastdata = cndData.Values[i - j].Data;
                nextdata = cndData.Values[i - j - 1].Data;

                if (isPositive)
                {
                    if (lastdata < nextdata)
                    {
                        return false;
                    }
                }
                else
                {
                    if (lastdata > nextdata)
                    {
                        return false;
                    }
                }
            }

            recordNotify(i, "制約3：連続6点上昇/下降");
            return true;
        }
        private void recordNotify(int i, string text)
        {
            /*
            using (SqlConnection con = DBAdapter.GetDTMDBConnection())
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@EQUIPMENTNO", SqlDbType.Char).Value = this.equipmentNo;
                cmd.Parameters.Add("@UPDUSERCD", SqlDbType.Char).Value = "9999";
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                cmd.Parameters.Add("@MEASUREDT", SqlDbType.DateTime).Value = DateTime.Parse(cndData.Keys[i]);
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = this.typeCd;
                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar).Value = text;


                cmd.CommandText = @"
                        SELECT
                         Equipment_NO
                        FROM TnCNR
                        WHERE Equipment_NO = @EQUIPMENTNO
                        AND Measure_DT = @MEASUREDT
                        AND Type_CD = @TYPECD";

                if (cmd.ExecuteScalar() == null)
                {
                    cmd.CommandText = @"
                    INSERT
                     TnCNR(Equipment_NO
	                    , Measure_DT
	                    , Type_CD
	                    , Message
	                    , UpdUser_CD
	                    , LastUpd_DT)
                    VALUES(@EQUIPMENTNO
	                    , @MEASUREDT
	                    , @TYPECD
	                    , @MESSAGE
	                    , @UPDUSERCD
	                    , @LASTUPDDT)";

                    cmd.ExecuteNonQuery();
                }
            }*/
        }

        /// <summary>
        /// σ計算。
        /// ExcelのStDevAと計算方法は同じ。
        /// </summary>
        /// <param name="cndData"></param>
        /// <param name="avg"></param>
        /// <returns></returns>
        public double calcSigma(SortedList<DateTime, QCLogData> cndData, double avg)
        {
            double xsum = 0;
            double x2sum = 0;
            double n = cndData.Count;
            //  念のためのエラーチェック
            if (n == 0) return 0.0;

            for (int i = 0; i < cndData.Count; i++)
            {
                xsum += cndData.Values[i].Data;
                x2sum += cndData.Values[i].Data * cndData.Values[i].Data;
            }

            //  Excel STDEVA()の定義
            double sigma = Math.Sqrt((n * x2sum - (xsum * xsum)) / (n * (n - 1)));

            return sigma;
        }

        /// <summary>
        /// 平均値計算
        /// </summary>
        /// <param name="cndData"></param>
        /// <returns></returns>
        public double calcAvg(SortedList<DateTime, QCLogData> cndData)
        {
            double retv = 0.0;
            //  念のためのエラーチェック
            if (cndData.Count == 0) return 0.0;

            for (int i = 0; i < cndData.Count; i++)
            {
                retv += cndData.Values[i].Data;
            }
            retv = retv / cndData.Count;

            return retv;
        }
    }
}

