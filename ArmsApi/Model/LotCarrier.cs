using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class LotCarrier
    {
		private const string LEFT_SIDE_IDENTITY = "L";
		private const string RIGHT_SIDE_IDENTITY = "R";

        public string LotNo { get; set; }
        public string CarrierNo { get; set; }
        public List<string> CarrierList { get; set; }
		public bool DivideHalfFlowfg { get; set; }
		public string EmpCd { get; set; }
		public string RingNo { get; set; }

		public LotCarrier(string lotNO, string carrierNo)
		{
			this.LotNo = lotNO;
			this.CarrierNo = carrierNo;
			this.EmpCd = "660";
		}

        public LotCarrier(string lotNO, string carrierNo, string empCD)
		{
            this.LotNo = lotNO;
            this.CarrierNo = carrierNo;
			this.EmpCd = empCD;
        }

        public LotCarrier(string lotNO, List<string> carrierList, string empCD)
        {
            this.LotNo = lotNO;
            this.CarrierList = carrierList;
			this.EmpCd = empCD;
        }

                /// <summary>
        /// キャリア番号からキャリア-ロット紐づけデータ取得
        /// </summary>
        /// <param name="carrierNo"></param>
        /// <param name="onlyActiveCarrier"></param>
        /// <returns></returns>
        public static LotCarrier GetData(string carrierNo, bool onlyActiveCarrier)
        {
            return GetData(carrierNo, onlyActiveCarrier, true);
        }

        /// <summary>
        /// キャリア番号からキャリア-ロット紐づけデータ取得
        /// </summary>
        /// <param name="carrierNo"></param>
        /// <param name="onlyActiveCarrier"></param>
        /// <param name="needToExist">true：結果が1件以上なければ例外エラーを発生させる/ false：結果がない場合、nullを返す</param>
        /// <returns></returns>
        public static LotCarrier GetData(string carrierNo, bool onlyActiveCarrier, bool needToExist)
        {
            List<LotCarrier> datalist = new List<LotCarrier>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
                cmd.Parameters.Add("@CarrierNO", System.Data.SqlDbType.NVarChar).Value = carrierNo;

                try
				{
					con.Open();

					string sql = @" SELECT lotno, dividehalfflowfg
								FROM TnLotCarrier WITH(nolock) 
								WHERE carrierno = @CarrierNO ";

                    if (onlyActiveCarrier)
                    {
                        sql += " AND operatefg = @OperateFG ";
                        cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = 1;
                    }

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
						int ordDivideHalfFlowFg = rd.GetOrdinal("dividehalfflowfg");
                        while (rd.Read())
                        {
                            string lotno = rd["lotno"].ToString().Trim();
                            LotCarrier l = new LotCarrier(lotno, carrierNo);
							l.DivideHalfFlowfg = Convert.ToBoolean(rd.GetInt32(ordDivideHalfFlowFg));
                            datalist.Add(l);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("キャリア番号からキャリア-ロット紐づけデータ取得時にエラー発生:キャリア番号:{0}", carrierNo), ex);
                }
            }

            if (datalist.Count == 1)
            {
                return datalist[0];
            }
            else if (needToExist == true && datalist.Count == 0)
            {
                throw new ApplicationException(string.Format("キャリア番号にロットが紐づいていません。キャリア番号：{0}", carrierNo));
            }
            else if (needToExist == false && datalist.Count == 0)
            {
                return null;
            }
            else
            {
                throw new ApplicationException(string.Format("キャリア番号に複数のロットが紐づいています。キャリア番号：{0}", carrierNo));
            }
        }

        public static LotCarrier GetData(string carrierNo, string lotNo, bool onlyActiveCarrier)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@CarrierNO", System.Data.SqlDbType.NVarChar).Value = carrierNo;
                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotNo;

                try
                {
                    con.Open();

                    string sql = @" SELECT lotno, dividehalfflowfg
								FROM TnLotCarrier WITH(nolock) 
								WHERE carrierno = @CarrierNO AND lotno = @LOTNO";

                    if (onlyActiveCarrier)
                    {
                        sql += " AND operatefg = @OperateFG ";
                        cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = 1;
                    }

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordDivideHalfFlowFg = rd.GetOrdinal("dividehalfflowfg");
                        while (rd.Read())
                        {
                            string lotno = rd["lotno"].ToString().Trim();
                            LotCarrier l = new LotCarrier(lotno, carrierNo);
                            return l;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("キャリア/ロット番号からキャリア-ロット紐づけデータ取得時にエラー発生:キャリア番号:{0}", carrierNo), ex);
                }

                return null;
            }
 
        }


        public static string[] GetCarrierNo(string lotNo, bool onlyActiveCarrier)
        {
            return GetCarrierNo(lotNo, onlyActiveCarrier, null);
        }


        public static string[] GetCarrierNo(string lotNo, bool onlyActiveCarrier, string ringNo)
        {
            return GetCarrierNo(lotNo, onlyActiveCarrier, ringNo, true);
        }

        /// <summary>
        /// ロット番号からキャリア番号取得
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="onlyActiveCarrier"></param>
        /// <returns></returns>
        public static string[] GetCarrierNo(string lotNo, bool onlyActiveCarrier, string ringNo, bool needToExist)
        {
            List<string> retv = new List<string>();

            if (String.IsNullOrWhiteSpace(lotNo) == true && String.IsNullOrWhiteSpace(ringNo) == true)
            {
                throw new ArmsException("ロット番号及びリング番号の何れも指定されていません");
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();

                    string sql = @" SELECT carrierno
								FROM TnLotCarrier WITH(nolock) 
                                WHERE 1 = 1 ";

                    if (onlyActiveCarrier)
                    {
                        sql += " AND operatefg = @OperateFG ";
                        cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = 1;
                    }

                    if (string.IsNullOrWhiteSpace(lotNo) == false)
                    {
                        cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = lotNo;
                        sql += "AND lotno = @LotNO ";
                    }

                    if (string.IsNullOrWhiteSpace(ringNo) == false)
                    {
                        cmd.Parameters.Add("@RingNo", System.Data.SqlDbType.NVarChar).Value = ringNo;
                        sql += "AND ringno = @RingNo ";
                    }

                    cmd.CommandText = sql;
                    
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            retv.Add(rd["carrierno"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("ロット番号/リングNoからキャリア番号取得時にエラー発生:ロット番号:{0}、リングNo:{1}", lotNo, ringNo), ex);
                }
            }

            if (retv.Count == 0 && needToExist == true)
            {
                throw new ApplicationException(string.Format("ロット番号/リングNoからキャリア番号が取得できません。ロット番号：{0}、リングNo:{1}", lotNo, ringNo));
            }
            else
            {
                return retv.ToArray();
            }
        }

        //<--削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。
        public static List<string> GetLotNotemp(string carrierNo)
        {
            List<string> retv = new List<string>();

            if (String.IsNullOrWhiteSpace(carrierNo) == true)
            {
                throw new ArmsException("キャリア番号が指定されていません。");
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    string sql = @" SELECT lotno
								FROM TnLotCarrier WITH(nolock) 
								WHERE operatefg=1 ";

                    if (string.IsNullOrEmpty(carrierNo) == false)
                    {
                        sql += " AND carrierno = @CarrierNO ";
                        cmd.Parameters.Add("@CarrierNO", System.Data.SqlDbType.NVarChar).Value = carrierNo;
                    }

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            retv.Add(rd["lotno"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("キャリア番号からロット番号取得時にエラー発生:キャリア番号:{0}", carrierNo), ex);
                }
            }

            return retv;
        }
        //-->削除機能：TnLotCarrierに無ければ、Dummy基板と同じ振る舞いをする。

        public static string[] GetLotNo(string carrierNo, bool onlyActiveCarrier)
		{
			return GetLotNo(carrierNo, null, onlyActiveCarrier, true);
		}

		/// <summary>
		/// キャリア番号からロット番号取得 2016/9/20 fukatani
		/// </summary>
		/// <param name=carrierNo"></param>
		/// <param name="onlyActiveCarrier"></param>
		/// <returns></returns>
		public static string[] GetLotNo(string carrierNo, string ringno, bool onlyActiveCarrier, bool isRequiredLotNo)
		{
			List<string> retv = new List<string>();

            if(String.IsNullOrWhiteSpace(carrierNo) == true && String.IsNullOrWhiteSpace(ringno) == true)
            {
                throw new ArmsException("ロット番号取得時にエラー発生:キャリア番号及びリング番号の何れも指定されていません");
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{

				try
				{
					con.Open();

					string sql = @" SELECT lotno
								FROM TnLotCarrier WITH(nolock) 
								WHERE 1 = 1 ";

                    if (onlyActiveCarrier)
                    {
                        sql += " AND operatefg = @OperateFG ";
                        cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = 1;
                    }
                    
                    if (string.IsNullOrEmpty(carrierNo) == false)
					{
						sql += " AND carrierno = @CarrierNO ";
						cmd.Parameters.Add("@CarrierNO", System.Data.SqlDbType.NVarChar).Value = carrierNo;
					}

					if(string.IsNullOrEmpty(ringno) == false)
					{
						sql += " AND ringno = @RingNO ";
						cmd.Parameters.Add("@RingNO", System.Data.SqlDbType.NVarChar).Value = ringno;
					}

					cmd.CommandText = sql;
					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							retv.Add(rd["lotno"].ToString().Trim());
						}
					}
				}
				catch (Exception ex)
				{
					throw new ArmsException(string.Format("キャリア番号からロット番号取得時にエラー発生:キャリア番号:{0}", carrierNo), ex);
				}
			}

			if (retv.Count == 0)
			{
				if (isRequiredLotNo)
				{
					throw new ApplicationException(string.Format("キャリア番号からロット番号が取得できません。キャリア番号：{0}", carrierNo));
				}
				else
				{
					return retv.Distinct().ToArray(); ;
				}
			}
			else
			{
				return retv.Distinct().ToArray();
			}
		}

        public static string GetLotNoFromRingNo(string ringNo)
        {
            return GetLotNoFromRingNo(ringNo, true);
        }

        public static string GetLotNoFromRingNo(string ringNo, bool needToExist)
		{
			string[] lotNoArrayFromLotCarrier = GetLotNo(null, ringNo, true, false);

			string lotNoArrayFromCassette = Cassette.GetCassette(ringNo);

			if (lotNoArrayFromLotCarrier.Length == 0 && string.IsNullOrEmpty(lotNoArrayFromCassette) && needToExist == true)
			{
				string errMsg = string.Format("ロットが見つかりませんでした。ringno:{0}", ringNo);

				throw new ApplicationException(errMsg);
			}
            else if(lotNoArrayFromLotCarrier.Length == 0 && string.IsNullOrEmpty(lotNoArrayFromCassette) && needToExist == false)
            {
                return null;
            }
			else if (lotNoArrayFromLotCarrier.Length > 0 && string.IsNullOrEmpty(lotNoArrayFromCassette) == false)
			{
				string errMsg = string.Format("ロットが複数テーブルにわたって見つかりました。ringno:{0}/(TnLotCarrier)lotno:{1}/(TnCassette)lotno:{2}",
					ringNo, string.Join(",", lotNoArrayFromLotCarrier), lotNoArrayFromCassette);

				throw new ApplicationException(errMsg);
			}
			else if(string.IsNullOrEmpty(lotNoArrayFromCassette) == false)
			{
				return lotNoArrayFromCassette;
			}
			else if(lotNoArrayFromLotCarrier.Length > 1)
			{
				string errMsg = string.Format("ロットが複数見つかりました。ringno:{0}/(TnLotCarrier)lotno:{1}", ringNo, string.Join(",", lotNoArrayFromLotCarrier));

				throw new ApplicationException(errMsg);
			}
			else
			{
				return lotNoArrayFromLotCarrier[0];
			}	
		}

		public static string GetCurrentCarrierNo(string lotNo)
        {
            if (string.IsNullOrEmpty(lotNo)) return null;

            string[] list = GetCarrierNo(lotNo, true);

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


//        /// <summary>
//        /// ロット番号とマガジン番号からキャリア番号取得
//        /// </summary>
//        /// <param name="lotNo"></param>
//        /// <param name="magazineNo"></param>
//        /// <param name="onlyActiveCarrier"></param>
//        /// <returns></returns>
//        public static string[] GetCarrierNo(string lotNo, string magazineNo, bool onlyActiveCarrier)
//        {
//            List<string> retv = new List<string>();

//            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {
//                cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = lotNo;
//                cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = Convert.ToInt32(onlyActiveCarrier);
//                cmd.Parameters.Add("@MagNO", System.Data.SqlDbType.NVarChar).Value = magazineNo;

//                try
//                {
//                    con.Open();

//                    string sql = @" SELECT carrierno
//								FROM TnLotCarrier WITH(nolock) 
//								WHERE operatefg = @OperateFG 
//                                AND lotno = @LotNO 
//                                AND magno = @MagNO";

//                    cmd.CommandText = sql;
//                    using (SqlDataReader rd = cmd.ExecuteReader())
//                    {
//                        while (rd.Read())
//                        {
//                            retv.Add(rd["carrierno"].ToString().Trim());
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    throw new ArmsException(string.Format("ロット番号とマガジン番号からキャリア番号取得時にエラー発生:ロット番号:{0} マガジン番号:{1}", lotNo, magazineNo), ex);
//                }
//            }

//            if (retv.Count == 0)
//            {
//                throw new ApplicationException(string.Format("ロット番号とマガジン番号からキャリア番号が取得できません。ロット番号：{0} マガジン番号:{1}", lotNo, magazineNo));
//            }
//            else
//            {
//                return retv.ToArray();
//            }
//        }

		/// <summary>
		/// 呼び出しもとでTransaction.Commit()必要
		/// </summary>
		/// <param name="cmd"></param>
		public void Insert(SqlCommand cmd)
        {
            try
            {
				cmd.Parameters.Clear();
                cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@Carrierno", System.Data.SqlDbType.NVarChar).Value = this.CarrierNo;
                cmd.Parameters.Add("@Operatefg", System.Data.SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = this.EmpCd;
                cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;

                //既に登録されているときはエラーにする
                cmd.CommandText = @"SELECT lotno FROM TnLotCarrier WHERE lotno=@LotNO AND carrierno=@Carrierno";
                object lot = cmd.ExecuteScalar();

                if (lot == null)
                {
                    //新規Insert
                    cmd.CommandText = @" INSERT INTO dbo.TnLotCarrier

                                   (lotno
                                   ,carrierno 
                                   ,operatefg
                                   ,updusercd
                                   ,lastupddt)
                             VALUES
                                   (@LotNO
                                   ,@Carrierno
                                   ,@Operatefg
                                   ,@UpdUserCD
                                   ,@LastupdDT) ";

					cmd.ExecuteNonQuery();

                }
                else
                {
                    throw new ApplicationException(string.Format("既に同一データが登録されている為、登録できません。 ロット番号:{0} 基板DM:{1}", this.LotNo, this.CarrierNo));
                }

				//cmd.Transaction.Commit();
            }
			catch (Exception ex)
			{
				throw new ArmsException("ロット-キャリア紐付け登録エラー:" + this.LotNo + " " + ex.Message, ex);
			}
	    }

		public void Insert()
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
                try
                {
                    con.Open();

                    //con.BeginTransaction();

                    Insert(cmd);

                    //cmd.Transaction.Commit();
                }
                catch
                {
                    throw;
                    //cmd.Transaction.Rollback();
                }
			}
		}

        public static void CheckRegistrableData(string lotno, string carrierno)
        {
            LotCarrier lotCarrier = LotCarrier.GetData(carrierno, true, false);

            if (lotCarrier != null)
            {
                throw new ApplicationException(string.Format(
                    "稼働中のキャリアとして存在します。LotNo:{0} CarrierNo:{1}", lotno, carrierno));
            }
        }


		public void DeleteInsert(/*bool onlycarrier*/)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    if (this.CarrierList == null || this.CarrierList.Count == 0)
                    {
                        throw new ApplicationException(string.Format("登録対象の基板DMデータがありません。 ロット番号:{0}", this.LotNo));
                    }

                    cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@Operatefg", System.Data.SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = "660";
                    cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;

                    if (this.RingNo != null)
                    {
                        cmd.Parameters.Add("@RingNo", System.Data.SqlDbType.NVarChar).Value = this.RingNo;
                    }
                    else
                    {
                        cmd.Parameters.Add("@RingNo", System.Data.SqlDbType.NVarChar).Value = System.DBNull.Value;
                    }

                    //前履歴は削除

                    cmd.CommandText = "DELETE FROM TnLotCarrier WHERE lotno=@LotNO";
				
					cmd.ExecuteNonQuery();

                    SqlParameter prmCarrierNo = cmd.Parameters.Add("@Carrierno", System.Data.SqlDbType.NVarChar);

                    //新規Insert
                    cmd.CommandText = @" INSERT INTO dbo.TnLotCarrier
                                   (lotno
                                   ,carrierno 
                                   ,operatefg
                                   ,updusercd
                                   ,lastupddt
                                   ,ringno)
                             VALUES
                                   (@LotNO
                                   ,@Carrierno
                                   ,@Operatefg
                                   ,@UpdUserCD
                                   ,@LastupdDT
                                   ,@RingNo) ";

                    foreach (string carrierno in this.CarrierList)
                    {
                        prmCarrierNo.Value = carrierno;
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("ロット-キャリア紐付け登録エラー:" + this.LotNo + " " + ex.Message, ex);
                }
            }
        }

		public static void Transfer(SqlCommand cmd, string lotNo, string oldCarrierNo, string newCarrierNo, int procno, string empcd)
		{
			try
			{
				cmd.Parameters.Clear();
				cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = lotNo;
				cmd.Parameters.Add("@OldCarrierNO", System.Data.SqlDbType.NVarChar).Value = oldCarrierNo;

				//前履歴は削除
				cmd.CommandText = "DELETE FROM TnLotCarrier WHERE lotno=@LotNO and carrierno=@OldCarrierNO";
				cmd.ExecuteNonQuery();

				cmd.Parameters.Add("@Operatefg", System.Data.SqlDbType.Bit).Value = true;
				cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = empcd;
				cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;
				cmd.Parameters.Add("@Carrierno", System.Data.SqlDbType.NVarChar).Value = newCarrierNo;
				cmd.Parameters.Add("@ProcNO", System.Data.SqlDbType.BigInt).Value = procno;

				//新規Insert
				cmd.CommandText = @" INSERT INTO dbo.TnLotCarrier
				                                (lotno
				                                ,carrierno 
				                                ,operatefg
				                                ,updusercd
				                                ,lastupddt)
				                            VALUES
				                                (@LotNO
				                                ,@Carrierno
				                                ,@Operatefg
				                                ,@UpdUserCD
				                                ,@LastupdDT) ";

				cmd.ExecuteNonQuery();

				cmd.CommandText = @" INSERT INTO dbo.TnCarrierHistory
                                (lotno
                                ,carriernoOrg
                                ,carriernoNew
                                ,procno
                                ,updusercd
								,lastupddt)
						VALUES
								(@LotNO
								,@OldCarrierNO
								,@Carrierno
								,@ProcNO
								,@UpdUserCD
								,@LastupdDT) ";

				cmd.ExecuteNonQuery();

			}
			catch (Exception ex)
			{
				throw new ArmsException("ロット-キャリア紐付け登録エラー:" + lotNo + " " + ex.Message, ex);
			}
		}

        /// <summary>
        /// リングデータを更新する関数。ロット・キャリア指定必須。
        /// </summary>
        public void UpdateRingNo(bool needToExist)
        {
            if (string.IsNullOrWhiteSpace(this.CarrierNo) == true || string.IsNullOrEmpty(this.LotNo) == true)
            {
                if (needToExist == false) return;
                throw new ApplicationException(string.Format("修正対象の基板DMデータがありません。 ロット番号:{0}、キャリア番号：{1}", this.LotNo, this.CarrierNo));
            }

            //キャリアとロットの組み合わせが存在するかの事前チェック。
            List<string> exsistlot = LotCarrier.GetLotNo(this.CarrierNo, false).ToList();
            if (exsistlot.Contains(this.LotNo) == false)
            {
                if (needToExist == false) return;
                throw new ApplicationException(string.Format("修正対象のデータがありません。 ロット番号:{0}、キャリア番号：{1}", LotNo, CarrierNo));
            }
            

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;
                    cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@CarrierNo", System.Data.SqlDbType.NVarChar).Value = this.CarrierNo;

                    if (string.IsNullOrWhiteSpace(this.EmpCd) == false)
                    {
                        cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = this.EmpCd;
                    }
                    else
                    {
                        cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = "660";
                    }

                    if (string.IsNullOrWhiteSpace(this.RingNo) == false)
                    {
                        cmd.Parameters.Add("@RingNo", System.Data.SqlDbType.NVarChar).Value = this.RingNo;
                    }
                    else
                    {
                        cmd.Parameters.Add("@RingNo", System.Data.SqlDbType.NVarChar).Value = System.DBNull.Value;
                    }

                    cmd.CommandText = @" UPDATE TnLotCarrier SET ringno = @RingNo, updusercd = @UpdUserCd, lastupddt = @LastUpdDt
                                        WHERE LotNo = @lotno AND carrierno = @Carrierno";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("リング情報更新エラー:" + this.LotNo + " " + ex.Message, ex);
                }
            }
        }

        public void Delete(string oldCarrierNo, string newCarrierNo, int procno, string empcd)
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
				con.Open();
				cmd.Transaction = con.BeginTransaction();

				try
				{
					Delete(cmd, oldCarrierNo, newCarrierNo, procno, empcd);
			
					cmd.Transaction.Commit();
				}
				catch(Exception ex)
                {
                    cmd.Transaction.Rollback();
					throw;
				}
			}
		}

        public void Delete(SqlCommand cmd, string oldCarrierNo, string newCarrierNo, int procno, string empcd)
        {
            try
            {
				cmd.Parameters.Clear();
                cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@OldCarrierNO", System.Data.SqlDbType.NVarChar).Value = oldCarrierNo;

                //前履歴は削除
                cmd.CommandText = "DELETE FROM TnLotCarrier WHERE lotno=@LotNO and carrierno=@OldCarrierNO";
                cmd.ExecuteNonQuery();

				//cmd.Parameters.Add("@Operatefg", System.Data.SqlDbType.Bit).Value = true;
				cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = empcd;
				cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;
				cmd.Parameters.Add("@Carrierno", System.Data.SqlDbType.NVarChar).Value = newCarrierNo;
				cmd.Parameters.Add("@ProcNO", System.Data.SqlDbType.BigInt).Value = procno;

//				//新規Insert
//				cmd.CommandText = @" INSERT INTO dbo.TnLotCarrier
//                                (lotno
//                                ,carrierno 
//                                ,operatefg
//                                ,updusercd
//                                ,lastupddt)
//                            VALUES
//                                (@LotNO
//                                ,@Carrierno
//                                ,@Operatefg
//                                ,@UpdUserCD
//                                ,@LastupdDT) ";

//				cmd.ExecuteNonQuery();

				cmd.CommandText = @" INSERT INTO dbo.TnCarrierHistory
                                (lotno
                                ,carriernoOrg
                                ,carriernoNew
                                ,procno
                                ,updusercd
								,lastupddt)
						VALUES
								(@LotNO
								,@OldCarrierNO
								,@Carrierno
								,@ProcNO
								,@UpdUserCD
								,@LastupdDT) ";

				cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new ArmsException("ロット-キャリア紐付け登録エラー:" + this.LotNo + " " + ex.Message, ex);
            }
        }

		/// <summary>
		/// 分割途中のキャリアか(1つ目のキャリア分割登録完了　2つ目未実施の状態)
		/// </summary>
		/// <returns></returns>
		public static bool HasDivideHalfFlow(string carrierNo)
		{
			LotCarrier lotCarrier = GetData(carrierNo, true);
			if (lotCarrier == null)
			{
				throw new ApplicationException();
			}

			return lotCarrier.DivideHalfFlowfg;
		}


        public static void Delete(SqlCommand cmd, string lotNo)
        {
            if (string.IsNullOrEmpty(lotNo))
            {
                throw new ApplicationException(string.Format("削除に必要な情報が足りません。LotNo:{0}", lotNo));
            }

            try
            {
                string sql = " DELETE FROM TnLotCarrier WHERE lotno = @LotNo";
                cmd.CommandText = sql;

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Delete(SqlCommand cmd, string lotNo, string carrierNo)
		{
			if (string.IsNullOrEmpty(lotNo) || string.IsNullOrEmpty(carrierNo))
			{
				throw new ApplicationException(string.Format("削除に必要な情報が足りません。LotNo:{0} CarrierNo:{1}"
					, lotNo, carrierNo));
			}

			try
			{
				string sql = " DELETE FROM TnLotCarrier WHERE lotno = @LotNo AND carrierNo = @CarrierNo ";
				cmd.CommandText = sql;

				cmd.Parameters.Clear();
				cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
				cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = carrierNo;

				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public static void Delete(string lotNo, string carrierNo)
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				Delete(cmd, lotNo, carrierNo);
			}
		}

        public static void Delete(string lotNo)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                Delete(cmd, lotNo);
            }
        }
        /// <summary>
        /// キャリア分割
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="orgCarrierNo"></param>
        /// <param name="newCarrierNo"></param>
        public static void DivideCarrier(string lotNo, string orgCarrierNo, string newCarrierNo, string empcd)
		{
			SqlCommand cmdLens = null;

			try
			{
				using (SqlConnection con = new SqlConnection(SQLite.ConStr))
				using (SqlConnection conLens = new SqlConnection(Config.Settings.LENSConSTR))
				using (SqlCommand cmd = con.CreateCommand())
				{
					try
					{
						con.Open();
						cmd.Transaction = con.BeginTransaction();						

						AsmLot lot = AsmLot.GetAsmLot(lotNo);
						Process[] workFlow = Process.GetWorkFlow(lot.TypeCd);

						string side = orgCarrierNo.Split(' ')[2];
						if (orgCarrierNo.EndsWith(LEFT_SIDE_IDENTITY))
						{
							orgCarrierNo = orgCarrierNo.TrimEnd(Convert.ToChar(LEFT_SIDE_IDENTITY)) + RIGHT_SIDE_IDENTITY;
						}

						// マップデータ分割の為、情報収集
						int currentProcNo = 0;
						List<LENS.MapResult> mapList = LENS.MapResult.GetCurrentOutputData(lotNo, orgCarrierNo);
						foreach (Process w in workFlow)
						{
							if (mapList.Exists(m => m.ProcNo == w.ProcNo) == false)
							{
								continue;
							}

							currentProcNo = w.ProcNo;
						}
						LENS.MapResult mapData = LENS.MapResult.GetCurrentProcOutputData(lotNo, currentProcNo, orgCarrierNo);
						if (mapData == null)
						{
							throw new ApplicationException(string.Format("マッピングデータが存在しません。ロットNo:{0} 工程No:{1}", lotNo, currentProcNo));
						}
						string[] mapDataChar = mapData.ResultValue.Split(',');
						int divideCt = mapDataChar.Count() / 2;

						// 移載先キャリアを追加
						LotCarrier lotCarrier = new LotCarrier(lotNo, newCarrierNo, empcd);
						lotCarrier.Insert(cmd);

						cmdLens = conLens.CreateCommand();
						conLens.Open();
						cmdLens.Transaction = conLens.BeginTransaction();

						IEnumerable<string> divideMapData;

						if (side == LEFT_SIDE_IDENTITY)
						{
							divideMapData = mapDataChar.Skip(divideCt);
						}
						else
						{
							divideMapData = mapDataChar.Take(divideCt);
						}

						bool isDivideHalfFlow = LotCarrier.HasDivideHalfFlow(orgCarrierNo);
						if (isDivideHalfFlow)
						{
							// 2基板目の分割登録の為、前キャリアの紐付けは削除 
							LotCarrier orgLotCarrier = new LotCarrier(lotNo, orgCarrierNo, empcd);
							orgLotCarrier.Delete(cmd, orgCarrierNo, newCarrierNo, currentProcNo, empcd);

							LENS.WorkResult.ChangeCarrier(ref cmdLens, lotNo, orgCarrierNo, newCarrierNo, currentProcNo);

							// マップデータの分割登録
							LENS.MapResult.Insert(cmdLens, lotNo, newCarrierNo, currentProcNo, string.Join(",", divideMapData), true, true, true, empcd);
							LENS.MapResult.Update(cmdLens, lotNo, orgCarrierNo, currentProcNo, true, false, empcd);
						}
						else
						{
							// 前キャリアを分割中に変更
							DivideHalfFlowCarrier(cmd, lotNo, currentProcNo, empcd, orgCarrierNo, newCarrierNo);

							LENS.WorkResult workResult = LENS.WorkResult.GetData(lotNo, currentProcNo, orgCarrierNo).First();
							workResult.CarrierNo = newCarrierNo;
							workResult.Insert(ref cmdLens);

							
							// マップデータの分割登録
							LENS.MapResult.Insert(cmdLens, lotNo, newCarrierNo, currentProcNo, string.Join(",", divideMapData), true, true, true, empcd);
						}

						cmdLens.Transaction.Commit();
						cmd.Transaction.Commit();
					}
					catch (Exception)
					{
						cmd.Transaction.Rollback();

						if (cmdLens != null)
						{
							cmdLens.Transaction.Rollback();
						}
						throw;
					}
				}
			}
			finally
			{
				if (cmdLens != null)
				{
					cmdLens.Dispose();
				}
			}
		}

		public static void UpdateOperateFg(string lotNo, string carrierNo, bool operateFg)
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				//string sql = @" UPDATE TnLotCarrier SET operatefg = @OperateFg, updusercd = @UpdUserCd, lastupddt = @LastUpdDt
				//				WHERE LotNo = @LotNo AND operatefg = 1 ";

                string sql = @" UPDATE TnLotCarrier SET operatefg = @OperateFg, updusercd = @UpdUserCd, lastupddt = @LastUpdDt
								WHERE LotNo = @LotNo ";

                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
				cmd.Parameters.Add("@OperateFg", SqlDbType.Int).Value = SQLite.SerializeBool(operateFg);
				cmd.Parameters.Add("@UpdUserCd", SqlDbType.NVarChar).Value = "660";
				cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime).Value = DateTime.Now;

				if (string.IsNullOrEmpty(carrierNo) == false)
				{
					sql += @" AND carrierno = @CarrierNo ";
					cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = carrierNo;
				}

                con.Open();
                cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// 分割途中の状態に変更
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="lotNo"></param>
		/// <param name="carrierNo"></param>
		public static void DivideHalfFlowCarrier(SqlCommand cmd, string lotNo, int procno, string empcd, string carrierNo, string newCarrierNo)
		{
			cmd.Parameters.Clear();

			cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
			cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = carrierNo;

			string sql = " UPDATE TnLotCarrier SET dividehalfflowfg = 1 WHERE LotNo = @LotNo AND CarrierNo = @CarrierNo ";

			cmd.CommandText = sql;
			cmd.ExecuteNonQuery();

			cmd.Parameters.Add("@Operatefg", System.Data.SqlDbType.Bit).Value = true;
			cmd.Parameters.Add("@UpdUserCD", System.Data.SqlDbType.NVarChar).Value = empcd;
			cmd.Parameters.Add("@LastupdDT", System.Data.SqlDbType.DateTime).Value = System.DateTime.Now;

			cmd.Parameters.Add("@CarrierNoNew", System.Data.SqlDbType.NVarChar).Value = newCarrierNo;
			cmd.Parameters.Add("@CarrierNoOrg", System.Data.SqlDbType.NVarChar).Value = carrierNo;
			cmd.Parameters.Add("@ProcNO", System.Data.SqlDbType.BigInt).Value = procno;

			cmd.CommandText = @" INSERT INTO dbo.TnCarrierHistory
                                    (lotno
                                    ,carriernoOrg
                                    ,carriernoNew
                                    ,procno
                                    ,updusercd
									,lastupddt)
							VALUES
									(@LotNO
									,@CarrierNoOrg
									,@CarrierNoNew
									,@ProcNO
									,@UpdUserCD
									,@LastupdDT) ";

			cmd.ExecuteNonQuery();

		}

		public static void DivideHalfFlowCarrier(string lotNo, int procno, string empcd, string carrierNo, string newCarrierNo)
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				DivideHalfFlowCarrier(cmd, lotNo, procno, empcd, carrierNo, newCarrierNo);
			}
		}

		public static int GetProc(string lotNo)
		{
			List<int> proclist = new List<int>();

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT inlineprocno
								FROM TnMag WITH(nolock) 
								WHERE lotNo = @LotNo 
                                AND newfg = 1";

				cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = lotNo;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						proclist.Add(Convert.ToInt32(rd["inlineprocno"]));
					}
				}
			}

			if (proclist.Count == 1)
			{
				return proclist[0];
			}
			else if (proclist.Count == 0)
			{
				return 0;
			}
			else
			{
				throw new ApplicationException(string.Format("現在工程が複数存在しています。ロット番号：{0}", lotNo));
			}

		}

        /// <summary>
        /// RingNoとLotNoで検索してヒットしたデータからRingNoのみ削除(nullで上書き)する関数
        /// </summary>
        /// <param name="ringNo"></param>
        public static void DeleteRingNo(string ringNo, string lotno)
        {
            string[] carrierList = GetCarrierNo(lotno, false, ringNo, false);
            if (carrierList == null || carrierList.Length == 0) return;
            
            LotCarrier lotCarrier = new LotCarrier(lotno, null);
            lotCarrier.LotNo = lotno;
            lotCarrier.RingNo = null;

            foreach (string Carrier in carrierList)
            {
                lotCarrier.CarrierNo = Carrier;
                lotCarrier.UpdateRingNo(false);
            }
        }


        /// <summary>
        /// 紐づくキャリアを解除する
        /// </summary>
        public static void CancelCarrier(string carrierno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = " UPDATE TnLotCarrier SET operatefg = 0 WHERE carrierno = @carrierno AND operatefg = 1 ";
                cmd.Parameters.Add("@carrierno", SqlDbType.NVarChar).Value = carrierno;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
