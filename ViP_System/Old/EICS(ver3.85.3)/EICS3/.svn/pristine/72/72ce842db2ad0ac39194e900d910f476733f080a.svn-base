using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace EICS.Database.LENS
{
    public class Magazine
    {
        public enum LoadStep
        {
            Even,
            Odd,
            All
        }

        public enum FrameWirebondStartPosition
        {
            FrontSide,
            BackSide,
        }

        public enum AroundInspectionType
        {
            Package,
            Column,
        }

        public int StepNum { get; set; }
        public int PackageQtyX { get; set; }
        public int PackageQtyY { get; set; }
        public LoadStep LoadStepCD { get; set; }
        public AroundInspectionType AroundInspectType { get; set; }
        public const string MAGAZINE_LABEL_IDENTIFIER = "30";

        public int TotalFramePackage
        {
            get
            {
                return PackageQtyX * PackageQtyY;
            }
        }
        public int TotalMagazinePackage
        {
            get
            {
                return TotalFramePackage * StepNum;
            }
        }

        public decimal FrameAroundInspectColumn { get; set; }
        public int FrameAroundInspectPackage { get; set; }

        public FrameWirebondStartPosition FrameWirebondStartPositionCD { get; set; }
        public int SamplingModeID { get; set; }
        public int SamplingCT { get; set; }

        /// <summary>
        /// マガジン構成を取得
        /// </summary>
        /// <param name="typeCD"></param>
        public static Magazine GetData(string typeCd, int hostLineCd)
        {
            List<Magazine> mags = getDatas(typeCd, hostLineCd);
            if (mags.Count == 0) { return null; }
            else
            {
                return mags.Single();
            }
        }
        private static List<Magazine> getDatas(string typeCd, int hostLineCd)
        {
            List<Magazine> retv = new List<Magazine>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.LENS, hostLineCd), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmMag.MagazineID, Step, FrameXPackage, FrameYPackage, FrameAroundInspecColumn, FrameAroundInspecPackage, LoadStepCD, SamplingModeID, SamplingCT
									FROM TmMag WITH (nolock) 
									INNER JOIN TmType WITH (nolock) ON TmMag.MagazineID = TmType.MagazineID 
									WHERE (TmType.DelFG = 0) AND (TmMag.DelFG = 0) ";

                if (!string.IsNullOrEmpty(typeCd))
                {
                    sql += " AND (TmType.TypeCD = @TypeCD) ";
                    conn.SetParameter("@TypeCD", System.Data.SqlDbType.Char, typeCd);
                }

                conn.Command.CommandText = sql;
                using (DbDataReader rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        Magazine c = new Magazine();

                        c.StepNum = Convert.ToInt32(rd["Step"]);
                        c.PackageQtyX = Convert.ToInt32(rd["FrameXPackage"]);
                        c.PackageQtyY = Convert.ToInt32(rd["FrameYPackage"]);
                        c.FrameAroundInspectColumn = Convert.ToDecimal(rd["FrameAroundInspecColumn"]);
                        c.FrameAroundInspectPackage = Convert.ToInt32(rd["FrameAroundInspecPackage"]);

                        if (c.FrameAroundInspectColumn != 0 && c.FrameAroundInspectPackage == 0)
                        {
                            c.AroundInspectType = AroundInspectionType.Column;
                        }
                        else if (c.FrameAroundInspectColumn == 0 && c.FrameAroundInspectPackage != 0)
                        {
                            c.AroundInspectType = AroundInspectionType.Package;
                        }
                        else
                        {
                            throw new ApplicationException(
                                string.Format("周辺検査数を設定しているマスタに不備があります。 MagazineID:{0}", rd["MagazineID"]));
                        }

						//int FrameWirebondStartPositionNum = Convert.ToInt32(rd["FrameWirebondStartPositionCD"]);
						//if (FrameWirebondStartPositionNum == 1)
						//{
						//    c.FrameWirebondStartPositionCD = FrameWirebondStartPosition.FrontSide;
						//}
						//else if (FrameWirebondStartPositionNum == 2)
						//{
						//    c.FrameWirebondStartPositionCD = FrameWirebondStartPosition.BackSide;
						//}
						//else
						//{
						//    throw new ApplicationException(string.Format("ワイヤーボンダーボンディング開始位置のマスタ設定に誤りがあります。FrameWirebondStartPositionCD:{0}", FrameWirebondStartPositionNum));
						//}

                        int loadStepNum = Convert.ToInt32(rd["LoadStepCD"]);

						////////////////////////////////////////////////////////////////
						// 2016/2/10 吉本 3in1のTmMag.LoadStepCDの2⇒5の変更を加えた際に下記のコメントアウト部のチェックでエラーが発生した為
						// チェックを除去
						////////////////////////////////////////////////////////////////
						//if (loadStepNum == 1)
						//{
						//	c.LoadStepCD = LoadStep.Even;
						//}
						//else if (loadStepNum == 2)
						//{
						//	c.LoadStepCD = LoadStep.Odd;
						//}
						//else if (loadStepNum == 3)
						//{
						//	c.LoadStepCD = LoadStep.All;
						//}
						//else
						//{
						//	throw new ApplicationException(string.Format("マガジンへのフレーム搭載段のマスタ設定に誤りがあります。LoadStepCD:{0}", loadStepNum));
						//}
						////////////////////////////////////////////////////////////////
						// 2016/2/10 吉本 ここまで
						////////////////////////////////////////////////////////////////

                        c.SamplingModeID = Convert.ToInt32(rd["SamplingModeID"]);
                        c.SamplingCT = Convert.ToInt32(rd["SamplingCT"]);

                        retv.Add(c);
                    }
                }
            }
            return retv;
        }
    }
}
