using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EICS.Arms
{
	class AsmLot
	{
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			AsmLot lot = (AsmLot)obj;
			if (this.NascaLotNo == lot.NascaLotNo)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		//static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 自動化用　マガジンバーコード
		/// </summary>
		public const string PREFIX_INLINE_MAGAZINE = "30 ";

		/// <summary>
		/// 高効率用　ロット番号バーコード
		/// </summary>
		public const string PREFIX_INLINE_LOT = "11 ";

		/// <summary>
		/// 高効率用　マガジン分割対応ロット番号バーコード
		/// </summary>
		public const string PREFIX_DEVIDED_INLINE_LOT = "13 ";

		/// <summary>
		/// エラーマガジン番号
		/// </summary>
		public const string MAGAZINE_ERROR = "ERROR";

		#region プロパティ

		/// <summary>
		/// NASCAロットNO
		/// マガジン連番ロットを入れた場合は自動変換
		/// </summary>
		public string NascaLotNo { get { return _nascaLotNo; } set { _nascaLotNo = Order.MagLotToNascaLot(value); } }
		private string _nascaLotNo;

		/// <summary>
		/// 指図タイプ名
		/// </summary>
		public string TypeCd { get; set; }

		/// <summary>
		/// プロファイルID
		/// </summary>
		public int ProfileId { get; set; }

		/// <summary>
		/// 樹脂グループコード
		/// </summary>
		public string ResinGpCd { get; set; }

		/// <summary>
		/// ブレンドコード
		/// </summary>
		public string BlendCd { get; set; }

		/// <summary>
		/// カットブレンド判定コード
		/// </summary>
		public string CutBlendCd { get; set; }

		/// <summary>
		/// ロットステータス警告
		/// </summary>
		public bool IsWarning { get; set; }

		/// <summary>
		/// 出荷規制(TnRestrictに切り出したため使用禁止)
		/// </summary>
		public bool IsRestricted { get; set; }

		/// <summary>
		/// ライフ試験対象フラグ
		/// </summary>
		public bool IsLifeTest { get; set; }

		/// <summary>
		/// 先行色調
		/// </summary>
		public bool IsColorTest { get; set; }

		/// <summary>
		/// 吸湿保管点灯試験対象フラグ
		/// </summary>
		public bool IsKHLTest { get; set; }

		/// <summary>
		/// 仮採番ロット番号
		/// </summary>
		public string TempLotNo { get; set; }

		/// <summary>
		/// 仮採番状態
		/// </summary>
		public bool IsTemp { get; set; }

		/// <summary>
		/// NASCAロット特性情報連携完了フラグ
		/// </summary>
		public bool IsNascaLotCharEnd { get; set; }

		/// <summary>
		/// 指図数量
		/// </summary>
		public int LotSize { get; set; }

		/// <summary>
		/// マッピング検査実施フラグ
		/// </summary>
		public bool IsMappingInspection { get; set; }

		/// <summary>
		/// 全数検査を行うかのフラグ
		/// </summary>
		public bool IsFullSizeInspection { get; set; }

		/// <summary>
		/// 抜取り検査対象ロットフラグ
		/// </summary>
		public bool IsChangePointLot { get; set; }

		#endregion

		#region HasErrorStatus

		/// <summary>
		/// マガジンステータスの内容を確認
		/// </summary>
		/// <param name="paramInfo">作業開始コマンド</param>
		/// <param name="msg">マガジン内容に該当するメッセージ</param>
		/// <param name="dataupdflg">データ更新フラグ</param>
		/// <returns>True=Inline Error,False=Inline Normal</returns>
		//public static bool HasErrorStatus(Order paramInfo)
		//{
		//    try
		//    {
		//        // マガジンSTが設定されていない場合は制御なし
		//        if (string.IsNullOrEmpty(paramInfo.MagazineSt))
		//        {
		//            return false;
		//        }
		//        else
		//        {
		//            return true;
		//        }

		//    }
		//    catch
		//    {
		//        throw;
		//    }
		//}

		#endregion

		//public static AsmLot CreateNewAsmLot(DateTime dt, int? macno)
		//{
		//    AsmLot retv = CreateNewAsmLotWithoutLotNumbering(dt, "", false, macno);
		//    retv.TempLotNo = Numbering.GetNewAsmLotNo(dt);
		//    retv.NascaLotNo = retv.TempLotNo;
		//    retv.Update();
		//    log.Info("新規ロット発行");

		//    try
		//    {
		//        Profile prof = null;
		//        if (Config.UseDBProfile == false)
		//        {
		//            prof = Profile.GetCurrentProfile();
		//        }
		//        else
		//        {
		//            if (macno.HasValue)
		//            {
		//                prof = Profile.GetCurrentDBProfile(macno.Value);
		//            }
		//        }

		//        if (prof == null) throw new ArmsException("投入可能なプロファイルが存在しません");

		//        //プロファイル時点の抜き取り検査設定
		//        foreach (int proc in prof.InspectionProcs)
		//        {
		//            Inspection isp = new Inspection();
		//            isp.LotNo = retv.NascaLotNo;
		//            isp.ProcNo = proc;
		//            isp.DeleteInsert();
		//            if (retv.IsChangePointLot == false)
		//            {
		//                //抜取り検査フラグON
		//                retv.IsChangePointLot = true;
		//                retv.Update();
		//            }
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        log.Error("ロット発行時抜き取り検査設定異常" + ex.ToString());
		//    }

		//    return retv;
		//}

		//public static AsmLot CreateNewAsmLotWithoutLotNumbering(DateTime dt, string tempLot, bool recordDb, int? macno)
		//{
		//    AsmLot retv = new AsmLot();

		//    Profile prof = null;
		//    if (Config.UseDBProfile == false)
		//    {
		//        prof = Profile.GetCurrentProfile();
		//    }
		//    else
		//    {
		//        if (macno.HasValue)
		//        {
		//            prof = Profile.GetCurrentDBProfile(macno.Value);
		//        }
		//    }
		//    if (prof == null) throw new ArmsException("投入可能なプロファイルが存在しません");

		//    retv.TempLotNo = tempLot;
		//    retv.NascaLotNo = tempLot;

		//    retv.TypeCd = prof.TypeCd;
		//    retv.ProfileId = prof.ProfileId;
		//    retv.LotSize = prof.LotSize;
		//    retv.ResinGpCd = prof.ResinGpCd;
		//    retv.BlendCd = prof.BlendCd;
		//    retv.CutBlendCd = prof.CutBlendCd;

		//    if (recordDb)
		//    {
		//        retv.Update();
		//    }

		//    return retv;
		//}

		#region Update ロット情報更新

		//public void Update(int lineCD)
		//{
		//    Update();
		//}

		public void Update(int lineCD)
		{
			//ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要

			//log.Info("ロット情報更新" + this.NascaLotNo);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ロット情報更新" + this.NascaLotNo);
			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.NascaLotNo;
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = this.TypeCd ?? "";
				cmd.Parameters.Add("@WARNINGFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsWarning);
				cmd.Parameters.Add("@RESTRICTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsRestricted);
				cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = this.ProfileId;
				cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = this.ResinGpCd ?? "";
				cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = this.BlendCd ?? "";
				cmd.Parameters.Add("@CUTBLENDCD", SqlDbType.NVarChar).Value = this.CutBlendCd ?? "";
				cmd.Parameters.Add("@LIFEFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsLifeTest);
				cmd.Parameters.Add("@KHLFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsKHLTest);
				cmd.Parameters.Add("@COLORTESTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsColorTest);
				cmd.Parameters.Add("@TEMPLOTNO", SqlDbType.NVarChar).Value = this.TempLotNo ?? "";
				cmd.Parameters.Add("@ISTEMP", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsTemp);
				cmd.Parameters.Add("@ISNASCALOTCHAREND", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaLotCharEnd);
				cmd.Parameters.Add("@ISFULLSIZEINSPECTION", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsFullSizeInspection);
				cmd.Parameters.Add("@ISMAPPINGINSPECTION", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsMappingInspection);
				cmd.Parameters.Add("@ISCHANGEPOINTLOT", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsChangePointLot);
				cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

				try
				{
					con.Open();

					//新規Insert
					cmd.CommandText = @"SELECT lotno FROM TnLot WHERE lotno=@LOTNO";
					object lot = cmd.ExecuteScalar();

					if (lot == null)
					{
						#region Insertコマンド
						cmd.CommandText = @"
                            INSERT INTO tnlot 
                              ( 
                                lotno , 
                                typecd , 
                                blendcd , 
                                warningfg , 
                                restrictfg ,
                                profileid, 
                                resingpcd,
                                cutblendcd,
                                lifetestfg,
                                khltestfg,
                                colortestfg,
                                templotno,
                                istemp,
                                isnascalotcharend,
                                isfullsizeinspection,
                                ismappinginspection,
                                ischangepointlot,
                                lastupddt 
                              ) 
                            VALUES 
                              ( 
                                @LOTNO , 
                                @TYPECD , 
                                @BLENDCD , 
                                @WARNINGFG , 
                                @RESTRICTFG ,
                                @PROFILEID ,
                                @RESINGPCD ,
                                @CUTBLENDCD,
                                @LIFEFG,
                                @KHLFG,
                                @COLORTESTFG,
                                @TEMPLOTNO,
                                @ISTEMP,
                                @ISNASCALOTCHAREND,
                                @ISFULLSIZEINSPECTION,
                                @ISMAPPINGINSPECTION,
                                @ISCHANGEPOINTLOT,
                                @LASTUPDDT 
                              )";
						#endregion
					}
					else
					{
						#region Updateコマンド

						cmd.CommandText = @"
                            UPDATE tnlot 
                            SET 
                              typecd = @TYPECD , 
                              blendcd = @BLENDCD , 
                              warningfg = @WARNINGFG , 
                              restrictfg = @RESTRICTFG , 
                              profileid = @PROFILEID,
                              resingpcd = @RESINGPCD,
                              cutblendcd = @CUTBLENDCD, 
                              lifetestfg = @LIFEFG,
                              khltestfg = @KHLFG,
                              colortestfg = @COLORTESTFG,
                              templotno = @TEMPLOTNO,
                              istemp = @ISTEMP,
                              isnascalotcharend = @ISNASCALOTCHAREND,
                              isfullsizeinspection = @ISFULLSIZEINSPECTION,
                              ismappinginspection = @ISMAPPINGINSPECTION,
                              ischangepointlot = @ISCHANGEPOINTLOT,
                              lastupddt = @LASTUPDDT
                            WHERE 
                              lotno = @LOTNO";
						#endregion
					}

					cmd.ExecuteNonQuery();

					//ロット特性情報の連動更新
					updateLotChars(lineCD);
				}
				catch (Exception ex)
				{
					throw new Exception("ロット情報更新エラー:" + this.NascaLotNo, ex);
				}
			}

		}

		/// <summary>
		/// ロット特性リスト
		/// </summary>
		private void updateLotChars(int lineCD)
		{
			//ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
			try
			{
				string constr = ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD);
				if (this.IsColorTest)
				{
					LotChar lc = new LotChar();
					lc.LotCharCd = Constant.COLOR_TEST_LOTCHAR_CD;
					lc.ListVal = "1"; //ON
					lc.DeleteInsert(this.NascaLotNo, constr);
				}
				else
				{
					LotChar.Delete(lineCD, this.NascaLotNo, Constant.COLOR_TEST_LOTCHAR_CD);
				}


				if (this.IsKHLTest)
				{
					LotChar lc = new LotChar();
					lc.LotCharCd = Constant.KHL_TEST_CT_LOTCHAR_CD;
					lc.LotCharVal = Constant.KHL_TEST_CT.ToString(); ;
					lc.DeleteInsert(this.NascaLotNo, constr);

					lc.LotCharCd = Constant.KHL_TEST_RESULT_LOTCHAR_CD;
					lc.ListVal = "3";
					lc.DeleteInsert(this.NascaLotNo, constr);
				}
				else
				{
					LotChar.Delete(this.NascaLotNo, Constant.KHL_TEST_RESULT_LOTCHAR_CD, constr);
					LotChar.Delete(this.NascaLotNo, Constant.KHL_TEST_CT_LOTCHAR_CD, constr);
				}


				if (this.IsLifeTest)
				{
				}
				else
				{
					LotChar.Delete(this.NascaLotNo, Constant.LIFE_TEST_CT_LOTCHAR_CD, constr);
				}
			}
			catch (Exception ex)
			{
				//log.Error("ロット更新時の特性連動更新異常:" + this.NascaLotNo + ":" + ex.ToString());
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "ロット更新時の特性連動更新異常:" + this.NascaLotNo + ":" + ex.ToString());
			}
		}

		#endregion

		#region SearchAsmLot

		public static AsmLot[] SearchAsmLot(int lineCD, string lotno, bool onlyTempLot, bool onlyNascaNotEnd)
		{
			return SearchAsmLot(lineCD, lotno, onlyTempLot, onlyNascaNotEnd, null);
		}

		public static AsmLot[] SearchAsmLot(int lineCD, string lotno, bool onlyTempLot, bool onlyNascaNotEnd, string serverNm)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			List<AsmLot> retv = new List<AsmLot>();

			string armsConnStr = string.Empty;

			if (string.IsNullOrEmpty(serverNm) == false)
			{
				armsConnStr = ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, serverNm);
			}
			else
			{
				armsConnStr = ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD);
			}

			using (SqlConnection con = new SqlConnection(armsConnStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				try
				{
					con.Open();
					//新規Insert
					cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.typecd ,
                          t.profileid, 
                          t.blendcd , 
                          t.resingpcd,
                          t.cutblendcd,
                          t.warningfg, 
                          t.lifetestfg,
                          t.khltestfg,
                          t.colortestfg,
                          t.templotno,
                          t.istemp,
                          t.isnascalotcharend,
                          t.restrictfg,
                          t.isfullsizeinspection,
                          t.ismappinginspection,
                          t.ischangepointlot,
                          t.lotsize
                        FROM
                          tnlot t with(nolock)
                        WHERE
                          1=1 ";

					if (string.IsNullOrEmpty(lotno) == false)
					{
						cmd.CommandText += " AND t.lotno = @LOTNO";
						cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
					}

					if (onlyTempLot)
					{
						cmd.CommandText += " AND istemp = 1";
					}

					if (onlyNascaNotEnd)
					{
						cmd.CommandText += " AND isnascalotcharend <> 1";
					}

					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							AsmLot lot = new AsmLot();
							lot.NascaLotNo = SQLite.ParseString(rd["lotno"]);
							lot.TypeCd = SQLite.ParseString(rd["typecd"]);
							lot.IsWarning = SQLite.ParseBool(rd["warningfg"]);
							lot.IsRestricted = SQLite.ParseBool(rd["restrictfg"]);
							lot.ProfileId = SQLite.ParseInt(rd["profileid"]);
							lot.ResinGpCd = SQLite.ParseString(rd["resingpcd"]);
							lot.BlendCd = SQLite.ParseString(rd["blendcd"]);
							lot.IsLifeTest = SQLite.ParseBool(rd["lifetestfg"]);
							lot.IsKHLTest = SQLite.ParseBool(rd["khltestfg"]);
							lot.IsColorTest = SQLite.ParseBool(rd["colortestfg"]);
							lot.CutBlendCd = SQLite.ParseString(rd["cutblendcd"]);
							lot.TempLotNo = SQLite.ParseString(rd["templotno"]);
							lot.IsNascaLotCharEnd = SQLite.ParseBool(rd["isnascalotcharend"]);
							lot.IsTemp = SQLite.ParseBool(rd["istemp"]);
							lot.LotSize = SQLite.ParseInt(rd["lotsize"]);
							lot.IsFullSizeInspection = SQLite.ParseBool(rd["isfullsizeinspection"]);
							lot.IsMappingInspection = SQLite.ParseBool(rd["ismappinginspection"]);
							lot.IsChangePointLot = SQLite.ParseBool(rd["ischangepointlot"]);
							retv.Add(lot);
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("ロット情報取得時エラー:" + lotno, ex);
				}
			}

			return retv.ToArray();
		}

		#endregion

		#region GetAsmLot
		/// <summary>
		/// インラインマガジンロット情報取得
		/// </summary>
		/// <param name="schParam">検索条件</param>
		/// <returns>インラインマガジンロット情報構造体</returns>
		public static AsmLot GetAsmLot(int lineCD, string lotno)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			AsmLot[] list = SearchAsmLot(lineCD, lotno, false, false);
			if (list.Length == 0)
			{
				return null;
			}
			else
			{
				return list[0];
			}
		}

        public static AsmLot GetAsmLotFromMultipleServer(int lineCD, string lotno, bool referMultiServerFG)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			AsmLot[] list = SearchAsmLot(lineCD, lotno, false, false);

			if (list.Length == 0)
			{
                if (referMultiServerFG)
                {
                    List<AsmLot> lotList = new List<AsmLot>();

                    SettingInfo commonSetting = SettingInfo.GetSingleton();

                    foreach (string serverNm in commonSetting.ArmsServerList)
                    {
                        lotList.AddRange(SearchAsmLot(lineCD, lotno, false, false, serverNm).ToList());
                    }

                    list = lotList.ToArray();
                }
			}

			if (list.Length == 0)
			{		
				return null;
			}
			else
			{
				return list[0];
			}
		}

		#endregion

		/// <summary>
		/// インラインマガジンロット情報取得
		/// </summary>
		/// <param name="schParam">検索条件</param>
		/// <returns>インラインマガジンロット情報構造体</returns>
		public static string GetType(int lineCD, string lotno, List<string> serverNmList)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			List<AsmLot> list = new List<AsmLot>();

			list.AddRange(SearchAsmLot(lineCD, lotno, false, false, null));

			foreach (string serverNm in serverNmList)
			{
				list.AddRange(SearchAsmLot(lineCD, lotno, false, false, serverNm));
			}

			List<string> typeList = new List<string>();

			foreach (AsmLot lot in list)
			{
				if (typeList.Contains(lot.TypeCd) == false)
				{
					typeList.Add(lot.TypeCd);
				}
			}

			if (typeList.Count > 1)
			{
				string serverNmListStr = string.Join(", ", serverNmList);

				throw new ApplicationException(string.Format(
					"1つのﾛｯﾄNoから複数のﾀｲﾌﾟが取得された為、ｴﾗｰ停止します。lineCD:{0}/ ﾛｯﾄNo:{1}/ lineCD以外で参照したｻｰﾊﾞ名:{2}",
					lineCD, lotno, serverNmListStr));
			}

			if (list.Count == 0)
			{
				return null;
			}
			else
			{
				return list[0].TypeCd;
			}
		}


		#region GetLotLog

		//public static SortedList<DateTime, string> GetLotLog(string lotno)
		//{
		//    SortedList<DateTime, string> retv = new SortedList<DateTime, string>();

		//    using (SqlConnection con = new SqlConnection(SQLite.ConStr))
		//    using (SqlCommand cmd = con.CreateCommand())
		//    {
		//        con.Open();
		//        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

		//        try
		//        {
		//            cmd.CommandText = @"SELECT indt, msg FROM TnLotLog WHERE lotno=@LOTNO";

		//            using (SqlDataReader rd = cmd.ExecuteReader())
		//            {
		//                while (rd.Read())
		//                {
		//                    DateTime key = SQLite.ParseDate(rd["indt"]) ?? DateTime.MinValue;
		//                    string msg = SQLite.ParseString(rd["msg"]);

		//                    retv.Add(key, msg);
		//                }
		//            }
		//        }
		//        catch (Exception ex)
		//        {
		//            throw new ArmsException(ex.Message);
		//        }
		//    }

		//    return retv;
		//}
		#endregion

		#region InsertLotLog
		//ロット単位の応答ログ保存
//        public static void InsertLotLog(string lotno, DateTime indt, string msg)
//        {
//            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {
//                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
//                cmd.Parameters.Add("@INDT", SqlDbType.DateTime).Value = indt;
//                cmd.Parameters.Add("@MSG", SqlDbType.NVarChar).Value = msg;

//                try
//                {
//                    con.Open();

//                    //DELETE
//                    cmd.CommandText = @"DELETE FROM TnLotLog WHERE lotno=@LOTNO AND indt=@INDT";
//                    cmd.ExecuteNonQuery();

//                    //INSERT
//                    cmd.CommandText = @"
//                            INSERT INTO TnLotLog (lotno, indt, msg) 
//                            VALUES (@LOTNO, @INDT, @MSG)";
//                    cmd.ExecuteNonQuery();
//                }
//                catch (Exception ex)
//                {
//                    throw new ArmsException(ex.Message);
//                }
//            }
//        }
		#endregion



		/// <summary>
		/// NASCAから供給IDを取得
		/// </summary>
		/// <param name="blendcd"></param>
		/// <returns></returns>
//        public static string GetNascaWaferShipId(string blendcd)
//        {
//            List<string> idlist = new List<string>();

//            using (SqlConnection con = new SqlConnection(Config.NASCAConSTR))
//            using (SqlCommand cmd = con.CreateCommand())
//            {
//                #region SQL

//                string sql = @"
//                     select Supply_ID from ntmbdss(nolock)
//                     where blend_cd = @BLENDCD";
//                #endregion

//                cmd.CommandText = sql;
//                cmd.Parameters.Add("@BLENDCD", System.Data.SqlDbType.VarChar).Value = blendcd;

//                con.Open();

//                using (SqlDataReader rd = cmd.ExecuteReader())
//                {
//                    while (rd.Read())
//                    {
//                        string id = rd["Supply_id"].ToString().Trim();
//                        if (idlist.Contains(id) == false)
//                        {
//                            idlist.Add(id);
//                        }
//                    }
//                }

//                string retv = "";
//                bool first = true;
//                foreach (string s in idlist)
//                {
//                    if (!first) retv += ":";
//                    retv += s;
//                    first = false;
//                }

//                return retv;
//            }
//        }
	}
}
