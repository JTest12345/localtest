using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class Magazine
	{
		public string MagazineNo { get; set; }

		public const string MAGAZINE_LABEL_IDENTIFIER = "30";
		public const string LOT_LABEL_IDENTIFIER = "13";
		/// <summary>マガジンラベルに最低限必要な要素数</summary>
		public const int LABEL_NEED_ELEMENT_NUM = 2;

		/// <summary>
		/// パッケージ進行方向
		/// </summary>
		public enum FrameDirection
		{
			/// <summary>前</summary>
			Before,
			/// <summary>後</summary>
			After,
		}

		public class Config : Magazine
		{
			public enum LoadStep
			{
				Even,
				Odd,
				All,
				Odd_NaturalRev,
				Even_NaturalRev
			}

			//public enum FrameWirebondStartPosition 
			//{
			//	FrontSide,
			//	BackSide,
			//}

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

			//public FrameWirebondStartPosition FrameWirebondStartPositionCD { get; set; }

			public bool IsWbFrameInAscendingOrder { get; set; }

			/// <summary>
			/// 周辺検査方法が列の合計パッケージ数
			/// </summary>
			public int TotalFrameAroundInspectPackage 
			{
				get 
				{
					if (this.AroundInspectType == AroundInspectionType.Column)
					{
						return Convert.ToInt32(PackageQtyY * FrameAroundInspectColumn);
					}
					else 
					{
						return FrameAroundInspectPackage;
					}
				}
			}

			/// <summary>
			/// 全フレームのY(列)数
			/// </summary>
			public int TotalFrameColumnCount
			{
				get
				{
					return TotalMagazinePackage / PackageQtyY;
				}
			}

			public Config() { }

			/// <summary>
			/// マガジン構成を取得
			/// </summary>
			/// <param name="typeCD"></param>
			public static Config GetData(string typecd)
			{
				List<Config> mags = getDatas(typecd);
				if (mags.Count == 0) { return null; }
				else
				{
					return mags.Single();
				}
			}
			public static List<Config> GetDatas()
			{
				return getDatas(null);
			}
			private static List<Config> getDatas(string typecd)
			{
				List<Config> retv = new List<Config>();

				using (SqlConnection con = new SqlConnection(LENS2_Api.Config.Settings.LensConnectionString))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					string sql = @" SELECT TmMag.MagazineID, Step, FrameXPackage, FrameYPackage, FrameAroundInspecColumn, FrameAroundInspecPackage, LoadStepCD, IsWbFrameInAscendingOrder
									FROM TmMag WITH (nolock) 
									INNER JOIN TmType WITH (nolock) ON TmMag.MagazineID = TmType.MagazineID 
									WHERE (TmType.DelFG = 0) AND (TmMag.DelFG = 0) ";

					if (!string.IsNullOrEmpty(typecd))
					{
						sql += " AND (TmType.TypeCD = @TypeCD) ";
						cmd.Parameters.Add("@TypeCD", System.Data.SqlDbType.Char).Value = typecd;
					}

					cmd.CommandText = sql;
					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							Config c = new Config();
							
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
							//	c.FrameWirebondStartPositionCD = FrameWirebondStartPosition.FrontSide;
							//}
							//else if (FrameWirebondStartPositionNum == 2)
							//{
							//	c.FrameWirebondStartPositionCD = FrameWirebondStartPosition.BackSide;
							//}
							//else
							//{
							//	throw new ApplicationException(string.Format("ワイヤーボンダーボンディング開始位置のマスタ設定に誤りがあります。FrameWirebondStartPositionCD:{0}", FrameWirebondStartPositionNum));						
							//}
							
							int loadStepNum = Convert.ToInt32(rd["LoadStepCD"]);
							if (loadStepNum == 1)
							{
								c.LoadStepCD = LoadStep.Even;
							}
							else if (loadStepNum == 2)
							{
								c.LoadStepCD = LoadStep.Odd;
							}
							else if (loadStepNum == 3)
							{
								c.LoadStepCD = LoadStep.All;
							}
							else if (loadStepNum == 4)
							{
								c.LoadStepCD = LoadStep.Even_NaturalRev;
							}
							else if (loadStepNum == 5)
							{
								c.LoadStepCD = LoadStep.Odd_NaturalRev;
							}
							else
							{
								throw new ApplicationException(string.Format("マガジンへのフレーム搭載段のマスタ設定に誤りがあります。LoadStepCD:{0}", loadStepNum));
							}

							c.IsWbFrameInAscendingOrder = Convert.ToBoolean(rd["IsWbFrameInAscendingOrder"]);

							retv.Add(c);
						}
					}
				}
				return retv;
			}

			/// <summary>
			/// ロギングアドレスから列番号(全フレームを連ねて何列目になるか)を取得する
			/// </summary>
			/// <param name="addressNo"></param>
			/// <returns></returns>
			public int GetColumnNO(int addressNo)
			{
				double remainder = Math.IEEERemainder(addressNo, PackageQtyY);
				if (remainder == 0)
				{
					return Convert.ToInt32(addressNo / PackageQtyY);
				}
				else
				{
					return Convert.ToInt32(Math.Ceiling((double)addressNo / (double)PackageQtyY));
				}
			}

		}
	}
}