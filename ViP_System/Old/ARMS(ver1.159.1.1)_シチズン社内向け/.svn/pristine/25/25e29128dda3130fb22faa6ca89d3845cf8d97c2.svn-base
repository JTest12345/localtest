using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LENS
{
	public class MacDefect
	{
		public string PlantCd { get; set; }
		public string LotNo { get; set; }
		public int ProcNo { get; set; }
		public string ProcNm { get; set; }
		public string DefItemCd { get; set; }
		public string DefItemNm { get; set; }

		public static void InsertUpdate(string magno, AsmLot lot, DefItem def, string address, string unit, string empcd)
		{
			if (def.DefectCt == 0)
			{
				throw new ApplicationException("不良数0でアドレス登録は出来ません");
			}

			try
			{
				Magazine mag = Magazine.GetCurrent(magno);
				if (mag == null) throw new ApplicationException("マガジン情報が見つかりません:" + magno);

				//次工程が開始されていたら次工程。無ければ現在完了工程
				Order ord = Order.GetNextMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
				if (ord == null)
				{
					ord = Order.GetMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
					if (ord == null) throw new ApplicationException("作業中の指図がありません:" + magno);
				}

				Process proc = Process.GetProcess(ord.ProcNo);

				MachineInfo mac = MachineInfo.GetMachine(ord.MacNo);
				if (mac == null) throw new ApplicationException("装置マスタ情報がありません:" + ord.MacNo);

				using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.Settings.LENSConSTR))
				using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					#region パラメータ設定

					cmd.Parameters.Add("@EQNO", SqlDbType.Char).Value = mac.NascaPlantCd;
					cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo;
					cmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = address ?? (object)DBNull.Value;
					cmd.Parameters.Add("@UNIT", SqlDbType.VarChar).Value = unit ?? (object)DBNull.Value;
					cmd.Parameters.Add("@TGTDT", SqlDbType.DateTime).Value = DateTime.Now;
					cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = proc.ProcNo;
					cmd.Parameters.Add("@PROCNM", SqlDbType.NVarChar).Value = proc.InlineProNM;
					cmd.Parameters.Add("@DEFITEMCD", SqlDbType.Char).Value = def.DefectCd;
					cmd.Parameters.Add("@DEFITEMNM", SqlDbType.NVarChar).Value = def.DefectName;
					cmd.Parameters.Add("@DEFCAUSECD", SqlDbType.Char).Value = def.CauseCd;
					cmd.Parameters.Add("@DEFCAUSENM", SqlDbType.NVarChar).Value = def.CauseName;
					cmd.Parameters.Add("@DEFCLASSCD", SqlDbType.Char).Value = def.ClassCd;
					cmd.Parameters.Add("@DEFCLASSNM", SqlDbType.NVarChar).Value = def.ClassName;
					cmd.Parameters.Add("@EMPCD", SqlDbType.Char).Value = empcd ?? "9999";
					cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
					cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

					#endregion

					cmd.CommandText = @"
                            SELECT LotNO
                            FROM TnMacDefect
                            WHERE PlantCD=@EQNO AND LotNO=@LOTNO 
                            AND DefAddressNO=@ADDRESS AND DefUnitNO=@UNIT AND ProcNO=@ProcNO";

					object exists = cmd.ExecuteScalar();
					if (exists == null)
					{
						cmd.CommandText = @"
                            INSERT INTO TnMacDefect(
                            PlantCD, LotNO, DefAddressNO, DefUnitNO, TargetDT, ProcNO, ProcNM,
                            DefItemCD, DefItemNM, DefCauseCD, DefCauseNM, DefClassCD, DefClassNM, UpdUserCD, DelFG, LastUpdDT)
                            VALUES (@EQNO, @LOTNO, @ADDRESS, @UNIT, @TGTDT, @PROCNO, @PROCNM, 
                            @DEFITEMCD, @DEFITEMNM, @DEFCAUSECD, @DEFCAUSENM, @DEFCLASSCD, @DEFCLASSNM, @EMPCD, @DELFG, @LASTUPDDT)";

						cmd.ExecuteNonQuery();
					}
					else
					{
						cmd.CommandText = @"
                            UPDATE TnMacDefect SET
                                DefItemCD=@DEFITEMCD, DefItemNM=@DEFITEMNM, DefCauseCD=@DEFCAUSECD, DefCauseNM=@DEFCAUSENM, DefClassCD=@DEFCLASSCD,
                                DefClassNM=@DEFCLASSNM, DelFG=@DELFG, LastUpdDT=@LASTUPDDT
                            WHERE PlantCD=@EQNO AND LotNO=@LOTNO 
                            AND DefAddressNO=@ADDRESS AND DefUnitNO=@UNIT AND ProcNO=@PROCNO";

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				Log.SysLog.Error("UPDATE EICS ERROR:" + ex.ToString());
				throw ex;
			}
		}

		public static List<MacDefect> GetData(string plantCd, string lotNo, int procNo, string defItemCd) 
		{
			List<MacDefect> retv = new List<MacDefect>();

			using (SqlConnection conn = new SqlConnection(Config.Settings.LENSConSTR))
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					string sql = @" SELECT PlantCD, LotNO, ProcNO, ProcNM, DefItemCD, DefItemNM
							FROM TnMacDefect WITH (nolock)
							WHERE (DelFG = 0) ";

					if (string.IsNullOrEmpty(plantCd) == false) 
					{
						cmd.Parameters.Add("@PlantCD", SqlDbType.Char).Value = plantCd;
						sql += " AND PlantCD = @PlantCD ";
					}

					if (string.IsNullOrEmpty(defItemCd) == false)
					{
						cmd.Parameters.Add("@DefItemCD", SqlDbType.Char).Value = defItemCd;
						sql += " AND DefItemCD = @DefItemCD ";
					}

					cmd.CommandText = sql;
					using (SqlDataReader rd = cmd.ExecuteReader()) 
					{
						while (rd.Read()) 
						{
							MacDefect d = new MacDefect();
							d.PlantCd = rd["PlantCD"].ToString().Trim();
							d.LotNo = rd["LotNO"].ToString().Trim();
							d.ProcNo = Convert.ToInt32(rd["ProcNO"]);
							d.ProcNm = rd["ProcNM"].ToString().Trim(); 
							d.DefItemCd = rd["DefItemCD"].ToString().Trim();
							d.DefItemNm = rd["DefItemNM"].ToString().Trim();
							retv.Add(d);
						}
					}
				}
			}
			
			return retv;
		}

		public static int GetMaterialChangeCt(string lotNo, int procNo) 
		{
			List<MacDefect> defCtList = new List<MacDefect>();
			
			foreach(string defCd in Config.Settings.MaterialChangeDefectCode)
			{
				defCtList.AddRange(GetData(string.Empty, lotNo, procNo, defCd));
			}

			return defCtList.Count();
		}
	}
}
