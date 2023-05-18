using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace EICS.Arms
{
	class Magazine
	{
		//private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		#region プロパティ

		/// <summary>
		/// マガジン番号
		/// </summary>
		public string MagazineNo { get; set; }

		/// <summary>
		/// NASCAロットNO(分割時は分割ロット番号）
		/// </summary>
		public string NascaLotNO { get; set; }


		//private string resinGpCd = null;
		/// <summary>
		/// 樹脂グループ
		/// </summary>
		//public string ResinGr
		//{
		//    get
		//    {
		//        if (string.IsNullOrEmpty(resinGpCd))
		//        {
		//            AsmLot lot = AsmLot.GetAsmLot(this.NascaLotNO);
		//            if (lot == null) return null;
		//            this.resinGpCd = lot.ResinGpCd;
		//        }
		//        return resinGpCd;
		//    }
		//}

		/// <summary>
		/// 現在完了工程
		/// </summary>
		public int NowCompProcess { get; set; }


		public string NowCompProcessNm(int lineCD)
		{
			return Process.GetProcess(lineCD, NowCompProcess).InlineProNM;
		}

		/// <summary>
		/// 現在稼働中フラグ
		/// </summary>
		public bool NewFg { get; set; }
		#endregion

		#region UpdateNewFgOff

		/// <summary>
		/// 現在稼働中フラグを外す
		/// </summary>
		/// <param name="magazineNo"></param>
		//public static void UpdateNewFgOff(string magazineNo)
		//{
		//    Magazine mag = Magazine.GetCurrent(magazineNo);

		//    if (mag == null)
		//    {
		//        return;
		//    }

		//    mag.NewFg = false;
		//    mag.Update();
		//}
		#endregion

        public static Magazine[] GetCurrentFromMultipleServer(int lineCD, string magazineNo, bool referMultiServerFG)
		{
			if (string.IsNullOrEmpty(magazineNo)) return null;

			Magazine[] list = GetMagazine(lineCD, magazineNo, null, true, null);

			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			if (list == null || list.Count() == 0)
			{
                if (referMultiServerFG)
                {
                    List<Magazine> magList = new List<Magazine>();

                    foreach (string serverNm in commonSettingInfo.ArmsServerList)
                    {
                        magList.AddRange(GetMagazine(serverNm, magazineNo, null, true, null).ToList());
                    }

                    list = magList.ToArray();
                }
			}

            //if (list == null)
            //{
            //    return null;
            //}
            //else if (list.Count() == 0)
            //{
            //    return null;
            //}
            //else if (list.Count() >= 2)
            //{
            //    return null;
            //}
            //else
            //{
                //return list[0];
            //}
            return list;
		}

		public static Magazine GetCurrent(int lineCD, string magazineNo)
		{
			if (string.IsNullOrEmpty(magazineNo)) return null;

			Magazine[] list = GetMagazine(lineCD, magazineNo, null, true, null);


			if (list == null)
			{
				return null;
			}
			else if (list.Count() == 0)
			{
				return null;
			}
			else
			{
				return list[0];
			}
		}

		#region GetMagazine

		public static Magazine[] GetMagazine(int lineCD, string magazineNo, string lotno, bool newFg, string resingpcd)
		{
			return GetMagazine(magazineNo, lotno, newFg, resingpcd, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
		}

		public static Magazine[] GetMagazine(string serverNm, string magazineNo, string lotno, bool newFg, string resingpcd)
		{
			return GetMagazine(magazineNo, lotno, newFg, resingpcd, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, serverNm));
		}

		/// <summary>
		/// マガジン検索(汎用 パラメータnull許容)
		/// </summary>
		/// <param name="magazineNo"></param>
		/// <param name="lotno"></param>
		/// <param name="newFg"></param>
		/// <returns></returns>
		public static Magazine[] GetMagazine(string magazineNo, string lotno, bool newFg, string resingpcd, string constr)
		{
			//ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
			List<Magazine> retv = new List<Magazine>();

			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{

				try
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.magno , 
                          t.inlineprocno , 
                          t.newfg
                        FROM
                          tnmag t with(nolock)
                        WHERE
                          1 = 1";


					if (newFg == true)
					{
						cmd.CommandText += " AND t.newfg = 1";
					}

					if (string.IsNullOrEmpty(magazineNo) == false)
					{
						cmd.CommandText += " AND t.magno = @MAGNO";
						cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magazineNo;
					}

					if (string.IsNullOrEmpty(lotno) == false)
					{
						cmd.CommandText += " AND t.lotno LIKE @LOTNO";
						cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";
					}


					if (string.IsNullOrEmpty(resingpcd) == false)
					{
						cmd.CommandText += " AND EXISTS (SELECT * FROM TnLot with(nolock) WHERE resingpcd = @RESINGPCD AND TnLot.lotno = t.lotno)";
						cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = resingpcd;
					}

					cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{

						while (reader.Read())
						{
							Magazine mag = new Magazine();
							mag.MagazineNo = SQLite.ParseString(reader["magno"]);
							mag.NascaLotNO = SQLite.ParseString(reader["lotno"]);
							mag.NowCompProcess = SQLite.ParseInt(reader["inlineprocno"]);
							mag.NewFg = SQLite.ParseBool(reader["newfg"]);

							retv.Add(mag);
						}
					}

					return retv.ToArray();
				}
				catch (Exception ex)
				{
					//log.Info(ex.ToString());
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
					throw ex;
				}
			}
		}
		#endregion

		#region (作業終了登録)インラインマガジンロット更新

		//public static Magazine ApplyMagazineInOut(Order workinfo)
		//{
		//    return ApplyMagazineInOut(workinfo, null);
		//}



		/// <summary>
		/// インラインマガジンロット更新
		/// </summary>
		/// <param name="workinfo"></param>
		/// <param name="duplicateMag">Trueの場合、IN側マガジンの稼働フラグを残したままにする</param>
		/// <param name="newMagLotNo">nullの場合はIn側マガジンのロット番号を継承</param>
		/// <returns></returns>
		//public static Magazine ApplyMagazineInOut(Order workinfo, string newMagLotNo)
		//{
		//    // インラインマガジンロット情報取得
		//    Magazine mag = GetCurrent(workinfo.InMagazineNo);

		//    if (workinfo.InMagazineNo == workinfo.OutMagazineNo)
		//    {
		//        mag.NowCompProcess = Convert.ToInt32(workinfo.ProcNo);
		//        mag.Update();
		//        return mag;
		//    }
		//    else
		//    {
		//        // 搬入マガジンNOと搬出マガジンNOが異なる場合

		//        Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(workinfo.InMagazineNo, workinfo.ProcNo);

		//        // ----- 搬入マガジンのデータを更新 -----
		//        // 流動フラグ
		//        mag.NewFg = false;
		//        mag.NowCompProcess = Convert.ToInt32(workinfo.ProcNo);
		//        mag.Update();


		//        // ----- 搬出マガジンのデータを編集 -----
		//        // 異なったロットで、同じマガジンを使用しているデータが存在するか
		//        Magazine newmag = GetCurrent(workinfo.OutMagazineNo);
		//        if (newmag != null && dst != Process.MagazineDevideStatus.DoubleToSingle)
		//        {
		//            throw new Exception("NW0650：搬出マガジンに、現在稼動中のロットがあります");

		//        }

		//        //新しいロット番号と関連付け
		//        if (!string.IsNullOrEmpty(newMagLotNo))
		//        {
		//            mag.NascaLotNO = newMagLotNo;
		//        }

		//        // マガジンNO
		//        mag.MagazineNo = workinfo.OutMagazineNo;
		//        mag.NowCompProcess = workinfo.ProcNo;
		//        mag.NewFg = true;

		//        mag.Update();
		//        return mag;
		//    }
		//}
		#endregion

		#region Update

		//public void Update()
		//{
		//    Update(SQLite.ConStr);
		//}


//        public void Update(string constr)
//        {
//            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
//            if (this.MagazineNo == AsmLot.MAGAZINE_ERROR)
//            {
//                throw new Exception("ERROR MAGAZINE");
//            }

//            using (SqlConnection con = new SqlConnection(constr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {

//                cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = this.MagazineNo;
//                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.NascaLotNO ?? "";
//                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.NowCompProcess;
//                cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.NewFg);
//                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

//                try
//                {
//                    con.Open();

//                    //新規Insert
//                    cmd.CommandText = @"SELECT magno FROM TnMag WHERE lotno=@LOTNO AND magno=@MAGNO";

//                    object mag = cmd.ExecuteScalar();

//                    if (mag == null)
//                    {
//                        #region Insertコマンド
//                        cmd.CommandText = @"
//                            INSERT INTO tnmag
//                              ( 
//                                lotno , 
//                                magno , 
//                                inlineprocno , 
//                                newfg , 
//                                lastupddt 
//                              ) 
//                            VALUES 
//                              ( 
//                                @LOTNO , 
//                                @MAGNO , 
//                                @PROCNO , 
//                                @NEWFG , 
//                                @LASTUPDDT 
//                              )";
//                        #endregion
//                    }
//                    else
//                    {
//                        #region Updateコマンド

//                        cmd.CommandText = @"
//                            UPDATE tnmag
//                            SET 
//                              inlineprocno = @PROCNO, 
//                              newfg = @NEWFG, 
//                              lastupddt = @LASTUPDDT 
//                            WHERE 
//                              lotno = @LOTNO AND magno = @MAGNO";
//                        #endregion
//                    }

//                    cmd.ExecuteNonQuery();
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception("マガジン情報更新エラー:" + ex.ToString());
//                }
//            }
//        }
		#endregion

		/// <summary>
		/// 強度試験用ファイル出力
		/// SvrConfigへの設定なしの場合は何もせずに終了
		/// </summary>
		//public void WritePSTesterFile()
		//{
		//    KeyValuePair<int, string>? cfg = Config.PSTesterLinkInfo;
		//    if (cfg.HasValue == false) return;

		//    //出力先ディレクトリ
		//    string basepath = cfg.Value.Value;

		//    //WB工程ID
		//    int wbproc = cfg.Value.Key;

		//    //設定が無い場合は何もしない
		//    if (basepath == null) return;

		//    //出力形態："ラインCD"+"_"+"型番"+"："+"ロットNo"+【"設備番号"_"WB号機"】
		//    //1013_NESW455B31：N1236CS1302【S00455_455号機】
		//    string format = "{0}_{1}：{2}【{3}_{4}】";

		//    AsmLot lot = AsmLot.GetAsmLot(this.NascaLotNO);
		//    if (lot == null) throw new Exception("AsmLotが見つかりません mag:" + this.MagazineNo);

		//    Order wborder = Order.GetMagazineOrder(this.NascaLotNO, wbproc);
		//    if (wborder == null) throw new Exception("WB工程の作業実績が存在しません mag:" + this.MagazineNo);

		//    Machine wbmac = Machine.GetMachine(wborder.MacNo);
		//    if (wbmac == null) throw new Exception("WB号機マスタが存在しません mac:" + wborder.MacNo.ToString());

		//    string filenm = string.Format(format, Config.InlineNo, lot.TypeCd, lot.NascaLotNo, wbmac.NascaPlantCd, wbmac.MachineName);

		//    string fullpath = Path.Combine(basepath, filenm);

		//    if (File.Exists(fullpath))
		//    {
		//        //既にまったく同名のファイルがあれば何もしない
		//        return;
		//    }

		//    using (StreamWriter sw = new StreamWriter(fullpath))
		//    {
		//        sw.WriteLine(DateTime.Now.ToString());
		//    }
		//}
	}
}
