using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArmsApi.Model
{
    public class Cassette
    {
        public string LotNo { get; set; }
        public string CassetteNo { get; set; }
        public int ProcNo { get; set; }

		//暫定追加　2016/9/29　fukatani
		public int SeqNo { get; set; }
		public DateTime Attachdt { get; set; }
		public DateTime? Detachdt { get; set; }
		public string NextCassetteNo { get; set; }
		public int Newfg { get; set; }
		public DateTime Lastupddt { get; set; }
        public string RingNo { get; set; }

        public List<string> listProcnm { get; set; }
		public class procinfo
		{
			public int procno { get; set; }
			public string procnm { get; set; }
		}



		public static Cassette CreateNewCassette()
        {
            throw new NotImplementedException();
        }


		//以下暫定コメントアウト　2016/9/29　fukatani
		/*public void Update()
        {
            throw new NotImplementedException();
        }*/

		public static Cassette Transfer(Cassette oldcas, string newCassetteNo, int proc)
        {
            throw new NotImplementedException();
        }

		//以下暫定追加　2016/9/29　fukatani

		public void CreateNew()
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
				cmd.Parameters.Add("@CASSETTENO", SqlDbType.NVarChar).Value = this.CassetteNo;
				cmd.Parameters.Add("@SEQNO", SqlDbType.Int).Value = this.SeqNo;
				cmd.Parameters.Add("@ATTACHDT", SqlDbType.DateTime).Value = this.Attachdt;
				if (this.Detachdt == null)
				{
					cmd.Parameters.Add("@DETACHDT", SqlDbType.DateTime).Value = System.DBNull.Value;
				}
				else
				{
					cmd.Parameters.Add("@DETACHDT", SqlDbType.DateTime).Value = this.Detachdt;
				}
				cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = this.ProcNo;
				if (this.NextCassetteNo == null)
				{
					cmd.Parameters.Add("@NEXTCASSETENO", SqlDbType.NVarChar).Value = System.DBNull.Value;
				}
				else
				{
					cmd.Parameters.Add("@NEXTCASSETENO", SqlDbType.NVarChar).Value = this.NextCassetteNo;
				}
				cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = this.Newfg;
				cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = this.Lastupddt;

				try
				{
					con.Open();

					cmd.CommandText = @"
                        INSERT INTO [TnCassette]
                            (lotno,cassetteno,seqno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt)
                        VALUES
                            (@LOTNO,@CASSETTENO,@SEQNO,@ATTACHDT,@DETACHDT,@PROCNO,@NEXTCASSETENO,@NEWFG,@LASTUPDDT)";
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException("ロット<->リング情報新規挿入エラー:" + ex.ToString());
				}
			}

		}
		public int GetSeqNo()
		{
			int seqno = 1;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT lotno
								FROM TnCassette WITH(nolock) 
								WHERE LotNO = @LotNO ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						seqno = seqno + 1;
					}
				}
			}
			return seqno;
		}

		public void Update()
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL
				string sql = @"
                    UPDATE TnCassette 
                    SET attachdt=@ATTACHDT,detachdt=@DETACHDT,procno=@PROCNO, nextcassetteno=@NEXTCASSETTENO,newfg=@NEWFG,ringno=@RINGNO
                    where lotno =@LOTNO and seqno=@SEQNO and cassetteno=@CASSETTENO and attachdt=@ATTACHDT";
				#endregion

				cmd.CommandText = sql;

				cmd.Parameters.Add("@ATTACHDT", SqlDbType.DateTime).Value = this.Attachdt;

                if(this.Detachdt != null)
                {
                    cmd.Parameters.Add("@DETACHDT", SqlDbType.DateTime).Value = this.Detachdt;
                }
                else
                {
                    cmd.Parameters.Add("@DETACHDT", SqlDbType.DateTime).Value = System.DBNull.Value;
                }

				//AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
				//if (lot == null)
				//{
				//throw new ApplicationException("ロット情報が見つかりません:" + this.LotNo);
				//}
				//Process first = Process.GetFirstProcess(lot.TypeCd);
				//cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = first.ProcNo;
				cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = this.ProcNo;

                if(string.IsNullOrWhiteSpace(this.NextCassetteNo) == false)
                {
                    cmd.Parameters.Add("@NEXTCASSETTENO", SqlDbType.NVarChar).Value = this.NextCassetteNo;
                }
                else
                {
                    cmd.Parameters.Add("@NEXTCASSETTENO", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }

				cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = this.Newfg;

				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
				cmd.Parameters.Add("@SEQNO", SqlDbType.Int).Value = this.SeqNo;
				cmd.Parameters.Add("@CASSETTENO", SqlDbType.NVarChar).Value = this.CassetteNo;
                
                if(string.IsNullOrWhiteSpace(this.RingNo) == false)
                {
                    cmd.Parameters.Add("@RINGNO", SqlDbType.NVarChar).Value = this.RingNo;
                }
                else
                {
                    cmd.Parameters.Add("@RINGNO", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }


				try
				{
					con.Open();
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException("リング情報更新エラー:" + this.LotNo, ex);
				}
			}
		}

		public static void DeleteNewFgOff(string lotno)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL
				string sql = @"
                    UPDATE TnCassette 
                    SET newfg = 0
                    where lotno =@LOTNO AND newfg = 1";
				#endregion

				cmd.CommandText = sql;
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

				try
				{
					con.Open();
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException("最終工程Cassette newfg0 更新エラー" + lotno, ex);
				}
			}
		}


		public static string GetTypeCD(string lotno)
		{
			string typecd = "";

			//using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))

			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT TypeCD
								FROM TnLot WITH(nolock) 
								WHERE LotNO = @LotNO ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{

						typecd = rd["TypeCD"].ToString().Trim();
					}
				}
			}

			return typecd;
		}
		public static List<procinfo> GetListProcess(string typecd)
		{
			List<procinfo> ListProcess = new List<procinfo>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL
				string sql = @"
                        SELECT dbo.TmProcess.procno,dbo.TmProcess.procnm
                        FROM dbo.TmWorkFlow WITH (NOLOCK)  INNER JOIN dbo.TmProcess WITH (NOLOCK)
                               ON dbo.TmWorkFlow.procno = dbo.TmProcess.procno
                        WHERE (dbo.TmWorkFlow.typecd = @TYPECD)
                        ORDER BY dbo.TmWorkFlow.workorder";
				#endregion

				cmd.CommandText = sql;
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						procinfo procinfo = new procinfo();

						procinfo.procno = Convert.ToInt32(rd["procno"]);
						procinfo.procnm = rd["procnm"].ToString().Trim();
						ListProcess.Add(procinfo);
					}
				}
				return ListProcess;
			}
		}


		public static List<string> GetCassette(string lotno, int newfg)
		{
			List<string> listitem = new List<string>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL

				string sql = @"
                     select cassetteno
                     from TnCassette(nolock)
                     where lotno = @LOTNO and newfg=@NEWFG";
				#endregion

				cmd.CommandText = sql;

				cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
				cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = newfg;

				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						string cassette = rd["cassetteno"].ToString().Trim();
						listitem.Add(cassette);
					}
				}
				return listitem;
			}
		}

		public static string GetCassette(string cassetteNo)
		{
			string lotno = "";

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL

				string sql = @"
                     select lotno
                     from TnCassette(nolock)
                     where cassetteno = @CASSETTENO and newfg=1";
				#endregion

				cmd.CommandText = sql;

				cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;

				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						lotno = rd["lotno"].ToString().Trim();
					}
				}
				return lotno;
			}
		}

		public static Cassette GetCassette(string lotno, string cassetteNo, int newfg)
		{
			Cassette item = new Cassette();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
                #region SQL

                cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = newfg;

                string sql = @"
                     select lotno,cassetteno,seqno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt,ringno 
                     from TnCassette(nolock)
                     where newfg = @NEWFG ";
                #endregion

                if (string.IsNullOrWhiteSpace(lotno) == false)
                {
                    sql = sql + " and lotno = @LOTNO ";
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                }

                if (string.IsNullOrWhiteSpace(cassetteNo) == false)
                {
                    sql = sql + " and cassetteno = @CASSETTENO ";
                    cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;
                }

                cmd.CommandText = sql;
                con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{

                    int ordRingNo = rd.GetOrdinal("ringno");

                    while (rd.Read())
					{
						item.LotNo = rd["lotno"].ToString().Trim();
						item.SeqNo = Convert.ToInt32(rd["seqno"]);
						item.CassetteNo = rd["cassetteno"].ToString().Trim();
						item.Attachdt = Convert.ToDateTime(rd["attachdt"]);
						item.Detachdt = rd["detachdt"] == DBNull.Value ? (DateTime?)null : SQLite.ParseDate(rd["detachdt"]);
						item.ProcNo = Convert.ToInt32(rd["procno"]);
						item.NextCassetteNo = rd["nextcassetteno"] == DBNull.Value ? (String)null : SQLite.ParseString(rd["nextcassetteno"]);
						item.Newfg = Convert.ToInt32(rd["newfg"]);
						item.Lastupddt = Convert.ToDateTime(rd["lastupddt"]);

                        if (rd.IsDBNull(ordRingNo) == false)
                        {
                            item.RingNo = rd[ordRingNo].ToString().Trim();
                        }
                    }
				}
				return item;
			}
		}

        //リングID指定で、枝番付のデータも抜く関数。cassetteNo必須。 2018/04追加
        public static List<Cassette> GetCassetteList(string lotno, string cassetteNo, int newfg)
        {
            List<Cassette> retv = new List<Cassette>();

            if(string.IsNullOrWhiteSpace(cassetteNo) == true)
            {
                throw new ApplicationException("cassetteNoが指定されていません");
            }

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = newfg;
                cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;
                cmd.Parameters.Add("@CASSETTELIKE", SqlDbType.VarChar).Value = cassetteNo + "-%";

                string sql = @"
                     select lotno,cassetteno,seqno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt,ringno 
                     from TnCassette(nolock)
                     where newfg = @NEWFG and ((cassetteno = @CASSETTENO) OR (cassetteno LIKE @CASSETTELIKE)) ";
                #endregion

                if (string.IsNullOrWhiteSpace(lotno) == false)
                {
                    sql = sql + " and lotno = @LOTNO ";
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                }

                cmd.CommandText = sql;
                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {

                    int ordRingNo = rd.GetOrdinal("ringno");

                    while (rd.Read())
                    {
                        Cassette item = new Cassette();
                        item.LotNo = rd["lotno"].ToString().Trim();
                        item.SeqNo = Convert.ToInt32(rd["seqno"]);
                        item.CassetteNo = rd["cassetteno"].ToString().Trim();
                        item.Attachdt = Convert.ToDateTime(rd["attachdt"]);
                        item.Detachdt = rd["detachdt"] == DBNull.Value ? (DateTime?)null : SQLite.ParseDate(rd["detachdt"]);
                        item.ProcNo = Convert.ToInt32(rd["procno"]);
                        item.NextCassetteNo = rd["nextcassetteno"] == DBNull.Value ? (String)null : SQLite.ParseString(rd["nextcassetteno"]);
                        item.Newfg = Convert.ToInt32(rd["newfg"]);
                        item.Lastupddt = Convert.ToDateTime(rd["lastupddt"]);

                        if (rd.IsDBNull(ordRingNo) == false)
                        {
                            item.RingNo = rd[ordRingNo].ToString().Trim();
                        }

                        retv.Add(item);
                    }
                }

                return retv;
            }
        }


        public static Cassette GetCassetteOther(string lotno, string cassetteNo, int newfg)
		{
			Cassette item = new Cassette();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL

				string sql = @"
                     select lotno,cassetteno,seqno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt 
                     from TnCassette(nolock)
                     where cassetteno = @CASSETTENO and lotno <> @LOTNO and newfg=@NEWFG";
				#endregion

				cmd.CommandText = sql;

				cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
				cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;
				cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = newfg;

				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						item.LotNo = rd["lotno"].ToString().Trim();
						item.SeqNo = Convert.ToInt32(rd["seqno"]);
						item.CassetteNo = rd["cassetteno"].ToString().Trim();
						item.Attachdt = Convert.ToDateTime(rd["attachdt"]);
						item.Detachdt = rd["detachdt"] == DBNull.Value ? (DateTime?)null : SQLite.ParseDate(rd["detachdt"]);
						item.ProcNo = Convert.ToInt32(rd["procno"]);
						item.NextCassetteNo = rd["nextcassetteno"] == DBNull.Value ? (String)null : SQLite.ParseString(rd["nextcassetteno"]);
						item.Newfg = Convert.ToInt32(rd["newfg"]);
						item.Lastupddt = Convert.ToDateTime(rd["lastupddt"]);
					}
				}
				return item;
			}
		}

		public static Cassette GetCassette(string lotno, string cassetteNo)
		{
			Cassette item = new Cassette();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				#region SQL

				string sql = @"
                     select lotno,seqno,cassetteno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt 
                     from TnCassette(nolock)
                     where cassetteno = @CASSETTENO and lotno = @LOTNO";
				#endregion

				cmd.CommandText = sql;

				cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
				cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;

				con.Open();

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						item.LotNo = rd["lotno"].ToString().Trim();
						item.SeqNo = Convert.ToInt32(rd["seqno"]);
						item.CassetteNo = rd["cassetteno"].ToString().Trim();
						item.Attachdt = Convert.ToDateTime(rd["attachdt"]);
						item.Detachdt = rd["detachdt"] == DBNull.Value ? (DateTime?)null : SQLite.ParseDate(rd["detachdt"]);
						item.ProcNo = Convert.ToInt32(rd["procno"]);
						item.NextCassetteNo = rd["nextcassetteno"] == DBNull.Value ? (String)null : SQLite.ParseString(rd["nextcassetteno"]);
						item.Newfg = Convert.ToInt32(rd["newfg"]);
						item.Lastupddt = Convert.ToDateTime(rd["lastupddt"]);
					}
				}
				return item;
			}
		}
        public static string[] Getlotno(string ringno)
        {
            List<string> listitem = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
                             SELECT lotno
                                FROM TnCassette With(nolock)
                                Where delfg = 0 AND newfg = 1 AND
                                ringno = @ringno";
                #endregion

                cmd.CommandText = sql;

                cmd.Parameters.Add("@ringno", SqlDbType.VarChar).Value = ringno;

                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string lotno = rd["lotno"].ToString().Trim();
                        listitem.Add(lotno);
                    }
                }
                return listitem.Distinct().ToArray();
            }
        }

        public static List<Cassette> GetCassetteList(string lotno, string cassetteNo, string ringNo, int newfg)
        {
            List<Cassette> retv = new List<Cassette>();
            
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = newfg;

                string sql = @"
                     select lotno,cassetteno,seqno,attachdt,detachdt,procno,nextcassetteno,newfg,lastupddt,ringno 
                     from TnCassette(nolock)
                     where newfg = @NEWFG ";
                #endregion
                
                if (string.IsNullOrWhiteSpace(lotno) == false)
                {
                    sql = sql + " and lotno = @LOTNO ";
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                }

                if (string.IsNullOrWhiteSpace(cassetteNo) == false)
                {
                    sql = sql + " and cassetteno = @CASSETTENO ";
                    cmd.Parameters.Add("@CASSETTENO", SqlDbType.VarChar).Value = cassetteNo;
                }

                if (string.IsNullOrWhiteSpace(ringNo) == false)
                {
                    sql = sql + " and ringno = @RINGNO ";
                    cmd.Parameters.Add("@RINGNO", SqlDbType.VarChar).Value = ringNo;
                }

                cmd.CommandText = sql;
                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {

                    int ordRingNo = rd.GetOrdinal("ringno");

                    while (rd.Read())
                    {
                        Cassette item = new Cassette();

                        item.LotNo = rd["lotno"].ToString().Trim();
                        item.SeqNo = Convert.ToInt32(rd["seqno"]);
                        item.CassetteNo = rd["cassetteno"].ToString().Trim();
                        item.Attachdt = Convert.ToDateTime(rd["attachdt"]);
                        item.Detachdt = rd["detachdt"] == DBNull.Value ? (DateTime?)null : SQLite.ParseDate(rd["detachdt"]);
                        item.ProcNo = Convert.ToInt32(rd["procno"]);
                        item.NextCassetteNo = rd["nextcassetteno"] == DBNull.Value ? (String)null : SQLite.ParseString(rd["nextcassetteno"]);
                        item.Newfg = Convert.ToInt32(rd["newfg"]);
                        item.Lastupddt = Convert.ToDateTime(rd["lastupddt"]);

                        if (rd.IsDBNull(ordRingNo) == false)
                        {
                            item.RingNo = rd[ordRingNo].ToString().Trim();
                        }
                        retv.Add(item);
                    }
                }
                return retv;
            }
        }

    }
}


