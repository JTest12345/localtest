using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;
using static ArmsApi.Model.Material;

namespace ArmsNascaBridge
{
    public class Wafer
    {
        public string MaterialCd { get; set; }
        public string MaterialNm { get; set; }
        public string LotNo { get; set; }
        public string BlendCd { get; set; }
        public string BinCd { get; set; }
        public string WorkCd { get; set; }
        public DateTime UseLimit { get; set; }
        public DateTime LastUpdDt { get; set; }

		public string DiceWaferKb { get; set; }
		public string SupplyId { get; set; }
        public float? ThicknessAve { get; set; }

        public decimal StockCt { get; set; }

        public DateTime? StockLastUpdDt { get; set; }


        public static void Import()
        {
			try
			{
				foreach (string dirPath in Config.Settings.WaferFilePath)
				{
					string donePath = Path.Combine(dirPath, "done");
					if (!Directory.Exists(donePath))
					{
						Directory.CreateDirectory(donePath);
					}

					List<Wafer> retv = new List<Wafer>();

					string[] files = Directory.GetFiles(dirPath);

					foreach (string file in files)
					{
						#region ファイル長比較　300ms待機
						long length = 0;
						while (length == 0)
						{
							FileInfo fi = new FileInfo(file);
							length = fi.Length;
							System.Threading.Thread.Sleep(300);
							fi = new FileInfo(file);
							long length2 = fi.Length;
							if (length != length2)
							{
								length = 0;
							}
						}
						#endregion

						Wafer[] wflist = getWafers(file);
						foreach (Wafer w in wflist)
						{
							updateWafer(w);

							//2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
							List<MatCond> conds = getWaferCond(w);
							foreach (MatCond c in conds)
							{
								MatCond.InsertUpdate(c);
							}

                        }

						File.Move(file, Path.Combine(donePath, Path.GetFileName(file)));
					}
				}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge] Wafer Error:" + err.ToString());
			}
        }

        public static void updateWafer(Wafer wf)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = wf.MaterialCd;
                    cmd.Parameters.Add("@MATNM", SqlDbType.NVarChar).Value = wf.MaterialNm;
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = wf.LotNo;
                    cmd.Parameters.Add("@BINCD", SqlDbType.NVarChar).Value = wf.BinCd;
                    cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = wf.BlendCd;
                    cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = wf.WorkCd;
                    cmd.Parameters.Add("@LIMITDT", SqlDbType.DateTime).Value = wf.UseLimit;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = wf.LastUpdDt;
					cmd.Parameters.Add("@DICEWAFERKB", SqlDbType.NVarChar).Value = wf.DiceWaferKb;
					cmd.Parameters.Add("@SUPPLYID", SqlDbType.NVarChar).Value = wf.SupplyId ?? string.Empty;
                    if (wf.ThicknessAve.HasValue)
                    {
                        cmd.Parameters.Add("@THICKNESSAVE", SqlDbType.Real).Value = wf.ThicknessAve;
                    }
                    else
                    {
                        cmd.Parameters.Add("@THICKNESSAVE", SqlDbType.NVarChar).Value = System.DBNull.Value;
                    }
                    cmd.Parameters.Add("@STOCKCT", SqlDbType.Decimal).Value = wf.StockCt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TnMaterials
                            WHERE materialcd=@MATCD AND lotno=@LOTNO";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd != null)
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (wf.LastUpdDt > current)
                        {
                            cmd.CommandText = "DELETE FROM TnMaterials WHERE materialcd=@MATCD AND lotno=@LOTNO";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            return;
                        }
                    }

                    cmd.CommandText = @"
                        INSERT INTO TnMaterials(materialcd, lotno, materialnm, limitdt, bincd, blendcd, iswafer, delfg, lastupddt, workcd, dicewaferkb, supplyid, thicknessave, stockct)
                        VALUES (@MATCD, @LOTNO, @MATNM, @LIMITDT, @BINCD, @BLENDCD, 1, 0, @UPDDT, @WORKCD, @DICEWAFERKB, @SUPPLYID, @THICKNESSAVE, @STOCKCT);";
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                    return;
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }
        }

        public static Wafer[] getWafers(string file)
        {
            List<Wafer> retv = new List<Wafer>();
            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        //2011/03/17 9:00:46,2TB-4G39L.11,L1NEW25S,NNSW208A-Sa6275/V-8
                        string[] data = line.Split('\t');
                        
                        if (data.Length != 5)
                        {
                            throw new Exception(string.Format("ウェハーファイルフォーマットが不正です。ファイル:{0}", file));
                        }

                        Wafer w = new Wafer();
                        w.LastUpdDt = DateTime.Parse(data[0]);
                        w.MaterialCd = data[1].Trim();
                        w.LotNo = data[2].Trim();
                        w.BlendCd = data[3].Trim();
                        w.WorkCd = data[4].Trim();

                        //使用期限、表示名
                        setMatInfo(w);

                        //厚み平均値※必要なラインのみ　2016/12/2 湯浅
                        if (Config.Settings.ImportMatThickness)
                        {
                            w.ThicknessAve = ArmsApi.Model.NASCA.Importer.GetThicknessAve(w.LotNo);
                        }

                        BinLot binLot = ArmsApi.Model.Material.GetStockCount(w.MaterialCd, w.LotNo);
                        if (binLot != null)
                        {
                            w.StockCt = binLot.StockCt;
                            w.StockLastUpdDt = binLot.StockLastUpdDt;
                        }

                        retv.Add(w);
                    }
                }
            }

            return retv.ToArray();
        }

        private static void setMatInfo(Wafer w)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = w.MaterialCd;
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = w.LotNo;

                cmd.CommandText = @"
                        select
                         t.material_cd
                        , t.lot_no
                        , t.bin_cd
                        , m.material_ja
                        , l.qultymnte_dt
                        , t.lastupd_dt
                        , dicewafer_kb
						, mtralbase_cd
                        from rvtbinlot(nolock) t
                        INNER JOIN rvmMat(nolock) m
                        ON m.material_cd = t.material_cd
                        INNER JOIN rvtlot(nolock) l
                        ON l.material_cd = t.material_cd
                        and l.lot_no = t.lot_no
						INNER JOIN dbo.RvmMCONV ON m.material_cd = dbo.RvmMCONV.material_cd
                        where t.material_cd = @MATCD
                        and l.material_cd = @MATCD
                        and t.lot_no = @LOTNO
                        and t.del_fg = 0
                        and m.del_fg = 0
                        and dbo.RvmMCONV.del_fg = 0
                        and l.del_Fg =0 option (maxdop 1)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        w.MaterialNm = rd["material_ja"].ToString().Trim();
                        w.UseLimit = Convert.ToDateTime(rd["qultymnte_dt"]);
                        w.BinCd = rd["bin_cd"].ToString().Trim();
						w.DiceWaferKb = rd["dicewafer_kb"].ToString().Trim();
						w.SupplyId = Material.GetWaferSupplyId(w.LotNo, w.MaterialCd);
                    }
                }
            }
        }
		
		private static List<MatCond> getWaferCond(Wafer w)
		{
			List<MatCond> retv = new List<MatCond>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.CommandText = @" SELECT dbo.NttLTTS.LotChar_VAL, dbo.NttLTTS.LastUpd_DT                        
									FROM dbo.NttLTTS WITH (nolock) 
									INNER JOIN dbo.NttLTNA WITH (nolock) ON dbo.NttLTTS.NascaLot_ID = dbo.NttLTNA.NascaLot_ID
									WHERE (dbo.NttLTTS.LotChar_CD = @LotCharCd) AND (NttLTNA.NascaLot_NO = @LotNo) ";

				cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = w.LotNo;
				cmd.Parameters.Add("@LotCharCd", SqlDbType.NVarChar).Value = MatChar.LOTCHARCD_WASHED;

				using (SqlDataReader rd = cmd.ExecuteReader()) 
				{
					while (rd.Read()) 
					{
						MatCond c = new MatCond();

						c.MaterialCd = w.MaterialCd;
						c.LotNo = w.LotNo;
						c.LotCharCd = MatChar.LOTCHARCD_WASHED;
						c.LotCharVal = rd["LotChar_VAL"].ToString().Trim();
						c.LastUpdDt = Convert.ToDateTime(rd["LastUpd_DT"]);

						retv.Add(c);
					}
				}
			}

			return retv;
		}

        public class MatCond
		{
			public string MaterialCd { get; set; }
			public string LotNo { get; set; }
			public string LotCharCd { get; set; }
			public string LotCharVal { get; set; }
			public DateTime LastUpdDt { get; set; }

			public static void InsertUpdate(MatCond c) 
			{
				using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @" UPDATE TnMatCond SET delfg = @DelFG, lastupddt = @LastUpdDT 
										 WHERE materialcd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd AND lotcharval = @LotCharVal
										 INSERT INTO TnMatCond (materialcd, lotno, lotcharcd, lotcharval)
										 SELECT @MaterialCd, @LotNo, @LotCharCd, @LotCharVal
										 WHERE NOT EXISTS (SELECT * FROM TnMatCond WHERE materialcd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd AND lotcharval = @LotCharVal) ";

					cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = c.MaterialCd;
					cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = c.LotNo;
					cmd.Parameters.Add("@LotCharCd", SqlDbType.NVarChar).Value = c.LotCharCd;
					cmd.Parameters.Add("@LotCharVal", SqlDbType.NVarChar).Value = c.LotCharVal;
					cmd.Parameters.Add("@DelFG", SqlDbType.Int).Value = 0;
					cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = c.LastUpdDt;

					cmd.ExecuteNonQuery();
				}
			}
		}
    }
}
