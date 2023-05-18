using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS
{
    public class Notifier
    {
        KLinkInfo Com = new KLinkInfo();

        SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
        int nMultiNO = 0;
		public int LineCD { get; set; }
        //0     SGA閾値外   SGA指定閾値を超えている
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
        public Notifier(SortedList<int, QCLogData> cndDataItem, int nMultiNO, int lineCD)
        {
            this.cndDataItem = cndDataItem;
            this.nMultiNO = nMultiNO;
			this.LineCD = lineCD;
        }

        /// <summary>
        /// 異常通知を行う
        /// </summary>
        /// <param name="equipmentNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="cndData"></param>
        public void Notify()
        {

            if (cndDataItem.Count < 1)
            {
                return;
            }

            double avg = calcAvg(cndDataItem);
            if (avg == 0.0)
            {
                return;
            }

            double sigma = calcSigma(cndDataItem, avg);
            if (sigma == 0.0)
            {
                return;
            }

            //設定に応じて、チェックする/しない
            List<bool> ListSwitch = new List<bool>();
            for (int i = 1; i < 11; i++)
            {
                ListSwitch.Add(ConnectDB.GetQCSettingSwitch(this.LineCD, cndDataItem[0].TypeCD, cndDataItem[0].InspectionNO, i));//USE_FGを取得。Trueの場合↓のcheckが走る
            }

            List<string> ListQcNum = new List<string>();
            for (int i = 2; i < 11; i++)
            {
                //ListQcNum.Add(GetQcNum(i));
				ListQcNum.Add(ConnectDB.GetQcNum(this.LineCD, cndDataItem[0].TypeCD, cndDataItem[0].InspectionNO, i));
            }

            for (int i = cndDataItem.Count - 1; i >= 0; i--)//現在→過去
            {
                //SGA判定は、無条件
                check0(i);
                if (ListSwitch[0])
                {
                    check1(i, avg, sigma);                  //1  	管理限界外  領域(※1)Aを超えている
                }

                if (ListSwitch[1])
                {
                    check2(i, avg, ListQcNum[0]);           //2 	連   	    連続する9点が中心線に対して同じ側にある
                }

                if (ListSwitch[2])
                {
                    check3(i, ListQcNum[1]);                //3 	上昇・下降 	連続する6点が増加，又は減少している
                }

                if (ListSwitch[3])
                {
                    check4(i, ListQcNum[2]);                //4 	交互増減 	14の点が交互に増減している
                }

                if (ListSwitch[4])
                {
                    check5(i, avg, sigma, ListQcNum[3]);    //5 	2σ外    	連続する3点中，2点が領域A又はそれを超えた領域にある（＞2σ）
                }

                if (ListSwitch[5])
                {
                    check6(i, avg, sigma, ListQcNum[4]);     //6 	1σ外 	    連続する5点中，4点が領域B又はそれを超えた領域にある（＞1σ）
                }

                if (ListSwitch[6])
                {
                    check7(i, avg, sigma, ListQcNum[5]);      //7 	中心化傾向 	連続する15点が領域Cに存在する（≦1σ）
                }

                if (ListSwitch[7])
                {
                    check8(i, avg, sigma, ListQcNum[6]);     //8 	連続1σ外 	連続する8点が領域Cを超えた領域にある（＞1σ）
                }

                if (ListSwitch[8])
                {
                    check9(i, ListQcNum[7]);                //9 	*点連続で、「傾向管理ライン」(両側)を超えている
                }

                if (ListSwitch[9])
                {
                    check10(i, ListQcNum[8]);               //10 	*点連続で、「傾向管理ライン」(狙い値)を超えている
                }
            }
        }

        /// <summary>
        /// 0 SGA管理限界外 1点が限界値を超えている
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool check0(int i)
        {
            //TnLogのMessageに文字が入っている→SGA管理限界外
            if (cndDataItem[i].MessageNM != "")
            {
				ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i, "制約0：SGA管理限界外", this.LineCD);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 1 管理限界外  領域(※1)Aを超えている
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool check1(int i, double avg, double sigma)
        {

            double ucl = avg + (3 * sigma);
            double lcl = avg - (3 * sigma);

            if (cndDataItem[i].Data > ucl)
            {
                ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i, "制約1：管理限界外", this.LineCD);
                return true;
            }

            if (cndDataItem[i].Data < lcl)
            {
				ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i, "制約1：管理限界外", this.LineCD);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 2 連続する9点が中心線に対して同じ側にある
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        private bool check2(int i, double avg, string sQcNum)
        {
            int points = 9;//ここ!!データベース取得
            //string sQcNum = GetQcNum(2);

            //check2の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            double lastdata = cndDataItem[i].Data;
            double side = 1.0;

            if (lastdata < avg)
            {
                side = -1.0;
            }

            for (int j = 0; j < points - 1; j++)
            {
                double judge = side * (cndDataItem[i - j].Data - avg);

                if (judge < 0)
                {
                    return false;
                }
            }

			ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i, "制約2：連続9点同側", this.LineCD);
            return true;
        }

        /// <summary>
        /// 3 上昇・下降 	連続する6点が増加，又は減少している
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        /// <returns></returns>
        private bool check3(int i, string sQcNum)
        {
            int points = 6;//ここ!!データベース取得

            //string sQcNum = GetQcNum(3);

            //check3の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            bool isPositive;
            double lastdata = cndDataItem[i].Data;
            double nextdata = cndDataItem[i - 1].Data;
            if (lastdata < nextdata)
            {
                isPositive = false;
            }
            else if (lastdata > nextdata)
            {
                isPositive = true;
            }
            else//同じ値
            {
                return false;
            }

            //6点の増加/減少だが、実際は5連続増加/下降であれば、NG
            for (int j = 0; j < points - 1; j++) //6点チェックなので比較は5回
            {
                lastdata = cndDataItem[i - j].Data;
                nextdata = cndDataItem[i - j - 1].Data;

                if (isPositive)
                {
                    if (lastdata <= nextdata)//同じ値は弾く
                    {
                        return false;
                    }
                }
                else
                {
                    if (lastdata >= nextdata)//同じ値は弾く
                    {
                        return false;
                    }
                }
            }

			ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i, "制約3：連続6点上昇/下降", this.LineCD);
            return true;
        }

        /// <summary>
        /// 4 	交互増減 	14の点が交互に増減している
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sQcNum"></param>
        /// <returns></returns>
        private bool check4(int i, string sQcNum)
        {
            int points = 14;//ここ!!データベース取得

            //string sQcNum = GetQcNum(4);

            //check4の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            bool isPositive;
            double lastdata = cndDataItem[i].Data;
            double nextdata = cndDataItem[i - 1].Data;
            if (lastdata < nextdata)
            {
                isPositive = false;//／
            }
            else if (lastdata > nextdata)
            {
                isPositive = true;
            }
            else//同じ値
            {
                return false;
            }

            for (int j = 0; j < points - 1; j++)
            {
                lastdata = cndDataItem[i - j].Data;
                nextdata = cndDataItem[i - j - 1].Data;

                if (isPositive)//＼
                {
                    if (lastdata <= nextdata)//同じ値は弾く
                    {
                        isPositive = false;//／
                    }
                    else
                    {
                        return false;
                    }
                }
                else//／
                {
                    if (lastdata >= nextdata)//同じ値は弾く
                    {
                        isPositive = true;//＼
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 5 	2σ外    	連続する3点中，2点が領域A又はそれを超えた領域にある（＞2σ） 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="avg"></param>
        /// <param name="sigma"></param>
        /// <param name="sQcNum"></param>
        /// <returns></returns>
        private bool check5(int i, double avg, double sigma, string sQcNum)
        {
            double ucl = avg + (2 * sigma);
            double lcl = avg - (2 * sigma);

            int points1 = 3;//ここ!!データベース取得
            int points2 = 2;//ここ!!データベース取得

            int nCnt = 0;
            string[] recordArray = new string[] { };

            //string sQcNum = GetQcNum(5);
            recordArray = sQcNum.Split(',');

            //check5の数字はふたつ
            if (sQcNum != "")
            {
                points1 = Convert.ToInt32(recordArray[0]);
                points2 = Convert.ToInt32(recordArray[1]);
                //points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points1) < 0)
            if ((i - (points1 - 1)) < 0)
            {
                return false;
            }

            double lastdata;
            for (int j = 0; j < points1; j++)
            {
                lastdata = cndDataItem[i - j].Data;

                if (lastdata > ucl || lcl < lastdata)
                {
                    nCnt = nCnt + 1;
                }
                if (nCnt > points2 - 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 6 	1σ外 	    連続する5点中，4点が領域B又はそれを超えた領域にある（＞1σ）
        /// </summary>
        /// <param name="i"></param>
        /// <param name="avg"></param>
        /// <param name="sigma"></param>
        /// <param name="sQcNum"></param>
        /// <returns></returns>
        private bool check6(int i, double avg, double sigma, string sQcNum)
        {
            double ucl = avg + (1 * sigma);
            double lcl = avg - (1 * sigma);

            int points1 = 5;//ここ!!データベース取得
            int points2 = 4;//ここ!!データベース取得

            string[] recordArray = new string[] { };
            //string sQcNum = GetQcNum(6);
            recordArray = sQcNum.Split(',');

            //check5の数字はふたつ
            if (sQcNum != "")
            {
                points1 = Convert.ToInt32(recordArray[0]);
                points2 = Convert.ToInt32(recordArray[1]);
                //points = Convert.ToInt32(sQcNum);
            }

            int nCnt = 0;
            //if ((i - points1) < 0)
            if ((i - (points1 - 1)) < 0)
            {
                return false;
            }

            double lastdata;
            for (int j = 0; j < points1; j++)
            {
                lastdata = cndDataItem[i - j].Data;

                if (lastdata > ucl || lcl < lastdata)
                {
                    nCnt = nCnt + 1;
                }
                if (nCnt > points2 - 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 7 	中心化傾向 	連続する15点が領域Cに存在する（≦1σ） 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="avg"></param>
        /// <param name="sigma"></param>
        /// <param name="sQcNum"></param>
        /// <returns></returns>
        private bool check7(int i, double avg, double sigma, string sQcNum)
        {
            double ucl = avg + (1 * sigma);
            double lcl = avg - (1 * sigma);

            int points = 15;//ここ!!データベース取得

            //string sQcNum = GetQcNum(7);

            //check4の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            double lastdata;
            for (int j = 0; j < points; j++)
            {
                lastdata = cndDataItem[i - j].Data;
                if (lastdata < ucl && lastdata > lcl)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 8 	連続1σ外 	連続する8点が領域Cを超えた領域にある（＞1σ） 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="avg"></param>
        /// <param name="sigma"></param>
        /// <param name="sQcNum"></param>
        /// <returns></returns>
        private bool check8(int i, double avg, double sigma, string sQcNum)
        {
            double ucl = avg + (1 * sigma);
            double lcl = avg - (1 * sigma);

            int points = 8;//ここ!!データベース取得

            //string sQcNum = GetQcNum(8);

            //check4の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            double lastdata;
            for (int j = 0; j < points; j++)
            {
                lastdata = cndDataItem[i - j].Data;
                if (lastdata <= ucl && lastdata >= lcl)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 9 	*点連続で、「傾向管理ライン」(両側)を超えている
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        private bool check9(int i, string sQcNum)
        {
            double ucl = 0, lcl = 0;
            int points = 2;//ここ!!データベース取得
            //string sQcNum = GetQcNum(2);

            //check2の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            string[] recordArray = new string[] { };
			string swork = ConnectDB.GetQcLine(this.LineCD, cndDataItem[i].TypeCD, cndDataItem[i].QcprmNO);
            recordArray = swork.Split(',');

            //1つでも値が入っていなければ、checkを行わない
            try
            {
                ucl = Convert.ToDouble(recordArray[0].Trim());
                lcl = Convert.ToDouble(recordArray[1].Trim());
            }
            catch
            {
                string sMessage = "★[check9]:QcParamNO=" + cndDataItem[i].QcprmNO + "の[QcLine_MAX],[QcLine_MIN]が正しく設定出来ていません。";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                return false;
            }

            double lastdata;
            for (int j = 0; j < points; j++)
            {
                lastdata = cndDataItem[i - j].Data;
                if (lastdata <= ucl && lastdata >= lcl)
                {
                    return false;
                }
            }

            string sMsg = "制約9：" + points + "点連続で、「傾向管理ライン」を超えている";

            //<--Y.Matsushima BTS.0001114 2011/02/02
            //recordNotify(i, sMsg);
			ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i - (points - 1), sMsg, this.LineCD);
            //-->Y.Matsushima BTS.0001114 2011/02/02

            return true;
        }

        /// <summary>
        /// 10 	*点連続で、「傾向管理ライン」(片側)を超えている
        /// </summary>
        /// <param name="i"></param>
        /// <param name="sigma"></param>
        /// <param name="avg"></param>
        private bool check10(int i, string sQcNum)
        {
            double dAim = 0;
            double dRate = 0;
            double dMax = 0;
            double dMin = 0;
            double dLimMax = 0;
            double dLimMin = 0;

            int points = 2;//ここ!!データベース取得
            //string sQcNum = GetQcNum(2);

            //check2の数字はひとつ
            if (sQcNum != "")
            {
                points = Convert.ToInt32(sQcNum);
            }

            //if ((i - points) < 0)
            if ((i - (points - 1)) < 0)
            {
                return false;
            }

            string[] recordArray = new string[] { };
			string swork = ConnectDB.GetQcLine(this.LineCD, cndDataItem[i].TypeCD, cndDataItem[i].QcprmNO);
            recordArray = swork.Split(',');

            //1つでも値が入っていなければ、checkを行わない
            try
            {
                dAim = Convert.ToDouble(recordArray[2].Trim());
                dRate = Convert.ToDouble(recordArray[3].Trim());
                dMax = Convert.ToDouble(recordArray[4].Trim());
                dMin = Convert.ToDouble(recordArray[5].Trim());
                dLimMax = ((dMax - dAim) * dRate / 100) + dAim;
                dLimMin = ((dMin - dAim) * dRate / 100) + dAim;
            }
            catch
            {
                string sMessage = "★[check10]:QcParamNO=" + cndDataItem[i].QcprmNO + "の[AimLine_VAL],[AimRate_VAL],[Parameter_MAX]が正しく設定出来ていません。";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                return false;
            }

            double lastdata;
            for (int j = 0; j < points; j++)
            {
                lastdata = cndDataItem[i - j].Data;
                if (lastdata < dLimMax && lastdata > dLimMin)
                {
                    return false;
                }
            }

            string sMsg = "制約10：" + points + "点連続で、「傾向管理ライン(狙い値)」を超えている";

            //<--Y.Matsushima BTS.0001114 2011/02/02
            //recordNotify(i, sMsg);
			ConnectDB.RecordNotify(this.cndDataItem, this.nMultiNO, i - (points - 1), sMsg, this.LineCD);
            //-->Y.Matsushima BTS.0001114 2011/02/02

            return true;
        }

        /// <summary>
        /// σ計算。
        /// ExcelのStDevAと計算方法は同じ。
        /// </summary>
        /// <param name="cndData"></param>
        /// <param name="avg"></param>
        /// <returns></returns>
        public double calcSigma(SortedList<int, QCLogData> cndDataItem, double avg)
        {
            double xsum = 0;
            double x2sum = 0;
            double n = cndDataItem.Count;
            //  念のためのエラーチェック
            if (n == 0) return 0.0;

            for (int i = 0; i < cndDataItem.Count; i++)
            {
                xsum += cndDataItem[i].Data;
                x2sum += cndDataItem[i].Data * cndDataItem[i].Data;
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
        public double calcAvg(SortedList<int, QCLogData> cndDataItem)
        {
            double retv = 0.0;
            //  念のためのエラーチェック
            if (cndDataItem.Count == 0) return 0.0;

            for (int i = 0; i < cndDataItem.Count; i++)
            {
                retv += cndDataItem[i].Data;
            }

            retv = retv / cndDataItem.Count;

            return retv;
        }
    }
}
