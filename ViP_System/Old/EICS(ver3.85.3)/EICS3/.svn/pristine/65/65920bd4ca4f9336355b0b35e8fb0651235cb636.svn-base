using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace EICS.Arms
{
	class Defect
	{
		#region 不良項目 DefItem

		/// <summary>
		/// 不良項目
		/// </summary>
		public class DefItem : IComparable
		{

			/// <summary>
			/// 表示順
			/// </summary>
			public int OrderNo { get; set; }

			/// <summary>
			/// 起因CD
			/// </summary>
			public string CauseCd { get; set; }

			/// <summary>
			/// 起因名称
			/// </summary>
			public string CauseName { get; set; }

			/// <summary>
			/// 分類CD
			/// </summary>
			public string ClassCd { get; set; }

			/// <summary>
			/// 分類名称
			/// </summary>
			public string ClassName { get; set; }

			/// <summary>
			/// 不良CD
			/// </summary>
			public string DefectCd { get; set; }

			/// <summary>
			/// 不良名称
			/// </summary>
			public string DefectName { get; set; }

			/// <summary>
			/// 不良数
			/// </summary>
			public int DefectCt { get; set; }



			#region IComparable メンバ

			public int CompareTo(object obj)
			{
				//nullより大きい
				if (obj == null)
				{
					return 1;
				}

				//違う型とは比較できない
				if (this.GetType() != obj.GetType())
				{
					throw new ArgumentException("別の型とは比較できません。", "obj");
				}
				DefItem def = (DefItem)obj;
				return this.GetHashCode().CompareTo(def.GetHashCode());
			}

			#endregion
		}
		#endregion

		/// <summary>
		/// 不良情報クラス
		/// </summary>

		/// <summary>
		/// 吸湿保管点灯試験で除外する工程
		/// </summary>
		private const int KHLTEST_EXCLUDE_PROCNO = 1;

		/// <summary>
		/// 吸湿保管点灯試験の判定に使う不良コード
		/// </summary>
		private const string KHLTEST_CAUSE_DEFECT_CD = "F0021";

		/// <summary>
		/// 吸湿保管点灯試験必要閾値
		/// </summary>
		private const int KHLTEST_THRESHOLD = 1;

		#region プロパティ

		public string LotNo { get; set; }

		public long ProcNo { get; set; }

		public List<DefItem> DefItems { get; set; }

		#endregion


		/// <summary>
		/// 指定タイプ、工程の不良項目一覧を取得
		/// </summary>
		/// <param name="typecd"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static DefItem[] GetDefectMaster(int lineCD, string typecd, long procno)
		{
			List<DefItem> retv = new List<DefItem>();

			//工程マスタ取得
			Process proc = Process.GetProcess(lineCD, procno);

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd ?? "";
				cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = proc.WorkCd ?? "";

				try
				{
					con.Open();

					//新規Insert
					cmd.CommandText = @"
                        SELECT
                            itemno,
                            causecd,
                            classcd,
                            itemcd,
                            itemnm,
                            causenm,
                            classnm
                        FROM
                            TmDefect
                        WHERE
                                workcd = @WORKCD
                        AND
                                typecd = @TYPECD
                        AND
                                delfg = 0";

					cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							DefItem item = new DefItem();
							item.CauseCd = SQLite.ParseString(reader["causecd"]);
							item.CauseName = SQLite.ParseString(reader["causenm"]);
							item.ClassCd = SQLite.ParseString(reader["classcd"]);
							item.ClassName = SQLite.ParseString(reader["classnm"]);
							item.DefectCd = SQLite.ParseString(reader["itemcd"]);
							item.DefectName = SQLite.ParseString(reader["itemnm"]);
							item.OrderNo = SQLite.ParseInt(reader["itemno"]);

							retv.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					//log.Info(ex.ToString());
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
				}
			}


			return retv.ToArray();
		}


		/// <summary>
		/// 特定不良発生の抜き取り検査判定
		/// </summary>
		private void JudgeDefIsp(int lineCD)
		{
			foreach (DefItem item in this.DefItems)
			{
				GnlMaster[] procs = GnlMaster.Search("DEFISP_PROC", item.DefectCd, this.ProcNo.ToString(), null, lineCD);

				if (procs.Length >= 1)
				{
					GnlMaster[] thresholds = GnlMaster.Search("DEFISP_THRESHOLD", item.DefectCd, null, this.ProcNo.ToString(), lineCD);
					if (thresholds.Length == 0)
					{
						//log.Error("特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
						continue;
					}

					int th;
					if (int.TryParse(thresholds[0].Val, out th) == false)
					{
						//log.Error("特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
						continue;
					}

					if (item.DefectCt < th) continue;

					foreach (GnlMaster proc in procs)
					{
						int procno;
						if (int.TryParse(proc.Val2, out procno) == false)
						{
							//log.Error("特定不良判定の工程NOに異常があります:" + item.DefectCd + ":DEFISP_PROC");
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "特定不良判定の工程NOに異常があります:" + item.DefectCd + ":DEFISP_PROC");
							continue;
						}

						Inspection isp = new Inspection();
						isp.LotNo = this.LotNo;
						isp.ProcNo = procno;
						isp.DeleteInsert(lineCD);
					}
				}
			}
		}


		#region GetDefect

		/// <summary>
		/// マスタ上存在する全ての不良項目を返す。
		/// 記録されている不良以外は数量0
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="typecd"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static DefItem[] GetAllDefect(int lineCD, string lotno, string typecd, long procno)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			Defect record = GetDefect(lineCD, lotno, procno);

			DefItem[] master = GetDefectMaster(lineCD, typecd, procno);

			foreach (DefItem d in record.DefItems)
			{

				bool found = false;
				foreach (DefItem m in master)
				{

					if (d.CauseCd == m.CauseCd && d.ClassCd == m.ClassCd && d.DefectCd == m.DefectCd)
					{
						m.DefectCt = d.DefectCt;
						found = true;
						break;
					}
				}

				if (!found)
				{
					throw new Exception("不良マスタに存在しません:" + d.DefectCd);
				}
			}

			return master;
		}

		/// <summary>
		/// マスタ上存在する全ての不良項目を返す。
		/// 記録されている不良以外は数量0
		/// </summary>
		/// <param name="lot"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static DefItem[] GetAllDefect(int lineCD, AsmLot lot, long procno)
		{
			return GetAllDefect(lineCD, lot.NascaLotNo, lot.TypeCd, procno);

		}


		/// <summary>
		/// 指定ロット、工程の不良全取得
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static Defect GetDefect(int lineCD, string lotno, long? procno)
		{
			//マガジン分割対応
			lotno = Order.MagLotToNascaLot(lotno);

			List<DefItem> items = new List<DefItem>();

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";

				try
				{
					con.Open();

					//新規Insert
					cmd.CommandText = @"
                    SELECT 
                      causecd , 
                      classcd , 
                      defectcd , 
                      defectct 
                    FROM 
                      tndefect with(nolock)
                    WHERE 
                      lotno = @LOTNO";

					if (procno.HasValue)
					{
						cmd.CommandText += " AND procno = @PROCNO";
						cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno.Value;
					}

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							DefItem item = new DefItem();
							item.CauseCd = SQLite.ParseString(reader["causecd"]);
							item.ClassCd = SQLite.ParseString(reader["classcd"]);
							item.DefectCd = SQLite.ParseString(reader["defectcd"]);
							item.DefectCt = SQLite.ParseInt(reader["defectct"]);

							items.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("不良情報取得エラー:" + lotno, ex);
				}
			}

			Defect retv = new Defect();
			retv.LotNo = lotno;
			if (procno.HasValue)
			{
				retv.ProcNo = procno.Value;
			}
			retv.DefItems = items;
			return retv;
		}

		#endregion
		public void DeleteInsert(int lineCD)
		{
			DeleteInsert(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), lineCD);
		}

		/// <summary>
		/// 該当proc,lotの不良を全削除した上で登録
		/// </summary>
		/// <param name="procno"></param>
		/// <param name="defects"></param>
		public void DeleteInsert(string constr, int lineCD)
		{
			//ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
			//log.Info("不良情報更新" + this.LotNo);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "不良情報更新" + this.LotNo);
			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				try
				{
					con.Open();
					cmd.Transaction = con.BeginTransaction();

					cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
					cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
					cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

					//前履歴は削除
					cmd.CommandText = " DELETE FROM TnDefect WHERE lotno=@LOTNO AND procno=@PROC ";
					cmd.ExecuteNonQuery();

					SqlParameter prmCauseCd = cmd.Parameters.Add("@CAUSECD", SqlDbType.NVarChar);
					SqlParameter prmClassCd = cmd.Parameters.Add("@CLASSCD", SqlDbType.NVarChar);
					SqlParameter prmDefectCd = cmd.Parameters.Add("@DEFECTCD", SqlDbType.NVarChar);
					SqlParameter prmDefectCt = cmd.Parameters.Add("@DEFECTCT", SqlDbType.BigInt);

					//新規Insert
					cmd.CommandText = @"
                        INSERT
                         INTO TnDefect(lotno
                            , procno
                            , causecd
                            , classcd
                            , defectcd
                            , defectct
                            , lastupddt
                            , delfg)
                        values(@LOTNO
                            , @PROC
                            , @CAUSECD
                            , @CLASSCD
                            , @DEFECTCD
                            , @DEFECTCT
                            , @UPDDT
                            , 0) ";

					foreach (DefItem d in this.DefItems)
					{
						//不良数0は対象外
						if (d.DefectCt == 0) continue;

						prmCauseCd.Value = d.CauseCd ?? "";
						prmClassCd.Value = d.ClassCd ?? "";
						prmDefectCd.Value = d.DefectCd ?? "";
						prmDefectCt.Value = d.DefectCt;

						cmd.ExecuteNonQuery();
					}

					cmd.Transaction.Commit();
				}
				catch (Exception ex)
				{
					cmd.Transaction.Rollback();
					throw new Exception("不良情報更新エラー", ex);
				}
			}

			//ライン間受渡しの場合は処理を行わない
			if (constr == ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD))
			{
				//特定工程・不良発生時の抜き取り検査フラグ追加判定
				JudgeDefIsp(lineCD);

				//QCIL更新
				AsmLot lot = AsmLot.GetAsmLot(lineCD, this.LotNo);
				if (lot != null)
				{
					DefItem[] updateList = GetAllDefect(lineCD, lot, this.ProcNo);
					UpdateEICS(lot, this.ProcNo, updateList, lineCD);
				}
			}
		}

		/// <summary>
		/// デフォルト検査数マスタ取得
		/// </summary>
		/// <param name="typecd"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		//public static int? GetDefaultInspectCtMaster(string typecd, int procno)
		//{
		//    GnlMaster[] mst = GnlMaster.Search("DEFCT", typecd, procno.ToString(), null);

		//    if (mst.Length == 0) return null;

		//    else
		//    {
		//        int ct;
		//        if (int.TryParse(mst[0].Val2, out ct) == true)
		//        {
		//            return ct;
		//        }
		//        else
		//        {
		//            return null;
		//        }
		//    }
		//}

		#region UpdateEICS


		/// <summary>
		/// 
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <param name="defList"></param>
		public static void UpdateEICS(AsmLot lot, long procno, DefItem[] defList, int lineCD)
		{
			try
			{
				Order[] magOrdList = Order.GetOrder(lineCD, lot.NascaLotNo, procno);
				if (magOrdList.Length == 0)
				{
					return;
				}
				Order ord = magOrdList[0];

				Machine mac = Machine.GetMachine(lineCD, ord.MacNo);
				if (mac == null)
				{
					return;
				}

				using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD)))
				using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
				{
					con.Open();


					DateTime inspectdt = DateTime.Now;
					if (ord.WorkEndDt.HasValue)
					{
						inspectdt = ord.WorkEndDt.Value;
					}

					#region パラメータ設定

					cmd.Parameters.Add("@INLINECD", SqlDbType.Int).Value = lineCD;
					cmd.Parameters.Add("@Equipment_NO", SqlDbType.Char).Value = mac.NascaPlantCd;
					cmd.Parameters.Add("@Measure_DT", SqlDbType.DateTime).Value = inspectdt;
					cmd.Parameters.Add("@NascaLot_NO", SqlDbType.VarChar).Value = lot.NascaLotNo;
					cmd.Parameters.Add("@Material_CD", SqlDbType.Char).Value = lot.TypeCd;
					cmd.Parameters.Add("@Magazine_NO", SqlDbType.Char).Value = ord.InMagazineNo ?? "";
					cmd.Parameters.Add("@Seq_NO", SqlDbType.Int).Value = 1;
					cmd.Parameters.Add("@SParameter_VAL", SqlDbType.VarChar).Value = "";
					cmd.Parameters.Add("@Message_NM", SqlDbType.VarChar).Value = "";
					cmd.Parameters.Add("@Check_FG", SqlDbType.Bit).Value = 0;
					cmd.Parameters.Add("@Del_FG", SqlDbType.Bit).Value = 0;
					cmd.Parameters.Add("@UpdUser_CD", SqlDbType.Char).Value = "660";
					cmd.Parameters.Add("@LastUpd_DT", SqlDbType.DateTime).Value = DateTime.Now;
					#endregion


					System.Data.SqlClient.SqlParameter prmDefCd = cmd.Parameters.Add("@DEFECTCD", SqlDbType.VarChar);
					System.Data.SqlClient.SqlParameter prmPrmNo = cmd.Parameters.Add("@QcParam_NO", SqlDbType.Int);
					System.Data.SqlClient.SqlParameter prmDVal = cmd.Parameters.Add("@DParameter_VAL", SqlDbType.Decimal);

					foreach (DefItem def in defList)
					{
						prmPrmNo.Value = 0;
						prmDVal.Value = def.DefectCt;
						prmDefCd.Value = def.DefectCd + "_" + def.ClassCd;
						cmd.CommandText = "SELECT QcParam_NO FROM TmPRM Where Parameter_NM = @DEFECTCD";

						object prmNo = cmd.ExecuteScalar();
						if (prmNo == null)
						{
							continue;
						}
						prmPrmNo.Value = Convert.ToInt32(prmNo);



						cmd.CommandText = @"
                        SELECT DParameter_VAL FROM TnLog WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_No
                        AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

						object objLastVal = cmd.ExecuteScalar();
						if (objLastVal != null)
						{
							if (Convert.ToInt32(objLastVal) == def.DefectCt)
							{
								continue;
							}
							else
							{
								cmd.CommandText = @"
                              UPDATE TnLog SET  Measure_DT=@Measure_DT, DParameter_VAL=@DParameter_VAL, LastUpd_DT=@LastUpd_DT
                                 WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_No
                                   AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

								cmd.ExecuteNonQuery();
								continue;
							}
						}
						else
						{
							cmd.CommandText = @"
                              INSERT INTO TnLog(Inline_CD,Equipment_NO,Measure_DT,Seq_NO,QcParam_NO,Material_CD,Magazine_NO,NascaLot_NO,DParameter_VAL,SParameter_VAL,Message_NM,Check_FG,Del_FG,UpdUser_CD,LastUpd_DT)
                                VALUES(@INLINECD,@Equipment_NO,@Measure_DT,@Seq_NO,@QcParam_NO,@Material_CD,@Magazine_NO,@NascaLot_NO,@DParameter_VAL,@SParameter_VAL,@Message_NM,@Check_FG,@Del_FG,@UpdUser_CD,@LastUpd_DT)";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}
			catch (Exception ex)
			{
				//log.Error("UPDATE EICS ERROR:" + ex.ToString());
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "UPDATE EICS ERROR:" + ex.ToString());
			}
		}
		#endregion


		#region UpdateEICS


		/// <summary>
		/// 
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <param name="defList"></param>
//            public static void UpdateEICSWireBondAddress(string magno, AsmLot lot, DefItem def, string address, string unit, string empcd)
//            {
//                if (def.DefectCt == 0)
//                {
//                    throw new ApplicationException("不良数0でアドレス登録は出来ません");
//                }

//                try
//                {
//                    Magazine mag = Magazine.GetCurrent(magno);
//                    if (mag == null) throw new ApplicationException("マガジン情報が見つかりません:" + magno);

//                    //次工程が開始されていたら次工程。無ければ現在完了工程
//                    Order ord = Order.GetNextMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
//                    if (ord == null)
//                    {
//                        ord = Order.GetMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
//                        if (ord == null) throw new ApplicationException("作業中の指図がありません:" + magno);
//                    }

//                    Process proc = Process.GetProcess(ord.ProcNo);

//                    Machine mac = Machine.GetMachine(ord.MacNo);
//                    if (mac == null) throw new ApplicationException("装置マスタ情報がありません:" + ord.MacNo);

//                    using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.QCILConSTR))
//                    using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
//                    {
//                        con.Open();

//                        #region パラメータ設定

//                        cmd.Parameters.Add("@LINENO", SqlDbType.Int).Value = int.Parse(Config.InlineNo);
//                        cmd.Parameters.Add("@EQNO", SqlDbType.Char).Value = mac.NascaPlantCd;
//                        cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo;
//                        cmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = address ?? (object)DBNull.Value;
//                        cmd.Parameters.Add("@UNIT", SqlDbType.VarChar).Value = unit ?? (object)DBNull.Value;
//                        cmd.Parameters.Add("@TGTDT", SqlDbType.DateTime).Value = DateTime.Now;
//                        cmd.Parameters.Add("@WORKCD", SqlDbType.Char).Value = proc.WorkCd;
//                        cmd.Parameters.Add("@DEFITEMCD", SqlDbType.Char).Value = def.DefectCd;
//                        cmd.Parameters.Add("@DEFITEMNM", SqlDbType.NVarChar).Value = def.DefectName;
//                        cmd.Parameters.Add("@DEFCAUSECD", SqlDbType.Char).Value = def.CauseCd;
//                        cmd.Parameters.Add("@DEFCAUSENM", SqlDbType.NVarChar).Value = def.CauseName;
//                        cmd.Parameters.Add("@DEFCLASSCD", SqlDbType.Char).Value = def.ClassCd;
//                        cmd.Parameters.Add("@DEFCLASSNM", SqlDbType.NVarChar).Value = def.ClassName;
//                        cmd.Parameters.Add("@EMPCD", SqlDbType.Char).Value = empcd ?? "9999";
//                        cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
//                        cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;
//                        #endregion

//                        cmd.CommandText = @"
//                            SELECT LOT_NO
//                            FROM TnDEFECT
//                            WHERE Line_CD=@LINENO AND Plant_CD=@EQNO AND Lot_NO=@LOTNO 
//                            AND DefAddress_NO=@ADDRESS AND DefUnit_NO=@UNIT AND WORK_CD=@WORKCD";

//                        object exists = cmd.ExecuteScalar();
//                        if (exists == null)
//                        {
//                            cmd.CommandText = @"
//                            INSERT INTO TnDEFECT(
//                            Line_Cd, Plant_CD, Lot_NO, DefAddress_NO, DefUnit_NO, Target_DT, Work_CD,
//                            DefItem_CD, DefItem_NM, DefCause_CD, DefCause_NM, DefClass_CD, DefClass_NM, UpdUser_CD, Del_FG, LastUpd_DT)
//                            VALUES (@LINENO, @EQNO, @LOTNO, @ADDRESS, @UNIT, @TGTDT, @WORKCD, 
//                            @DEFITEMCD, @DEFITEMNM, @DEFCAUSECD, @DEFCAUSENM, @DEFCLASSCD, @DEFCLASSNM, @EMPCD, @DELFG, @LASTUPDDT)";

//                            cmd.ExecuteNonQuery();
//                        }
//                        else
//                        {
//                            cmd.CommandText = @"
//                            UPDATE TnDEFECT SET
//                                DefItem_CD=@DEFITEMCD, DefItem_NM=@DEFITEMNM, DefCause_CD=@DEFCAUSECD, DefCause_NM=@DEFCAUSENM, DefClass_CD=@DEFCLASSCD,
//                                DefClass_NM=@DEFCLASSNM, Del_FG=@DELFG, LastUpd_DT=@LASTUPDDT
//                            WHERE Line_CD=@LINENO AND Plant_CD=@EQNO AND Lot_NO=@LOTNO 
//                            AND DefAddress_NO=@ADDRESS AND DefUnit_NO=@UNIT AND WORK_CD=@WORKCD";

//                            cmd.ExecuteNonQuery();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    log.Error("UPDATE EICS ERROR:" + ex.ToString());
//                    throw ex;
//                }
//            }



		/// <summary>
		/// ワイヤーボンダーで区分0以外に設定されたエラーアドレス
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="procno"></param>
		/// <param name="defList"></param>
//            public static void UpdateEICSWireBondMappingAddress(string magno, string address, string empcd)
//            {
//                try
//                {
//                    Magazine mag = Magazine.GetCurrent(magno);
//                    if (mag == null) throw new ApplicationException("マガジン情報が見つかりません:" + magno);

//                    using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.QCILConSTR))
//                    using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
//                    {
//                        con.Open();

//                        #region パラメータ設定
//                        cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = mag.NascaLotNO;
//                        cmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = address ?? (object)DBNull.Value;
//                        cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
//                        cmd.Parameters.Add("@EMPCD", SqlDbType.Char).Value = empcd ?? "9999";
//                        cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;
//                        #endregion

//                        cmd.CommandText = @"
//                            SELECT LOT_NO
//                            FROM TnDEFECTRESIN
//                            WHERE Lot_NO=@LOTNO 
//                            AND Address_NO=@ADDRESS";

//                        object exists = cmd.ExecuteScalar();
//                        if (exists == null)
//                        {
//                            cmd.CommandText = @"
//                            INSERT INTO TnDEFECTRESIN(
//                            Lot_NO, Address_NO, UpdUser_CD, Del_FG, LastUpd_DT)
//                            VALUES (@LOTNO, @ADDRESS, 
//                            @EMPCD, @DELFG, @LASTUPDDT)";

//                            cmd.ExecuteNonQuery();
//                        }
//                        else
//                        {
//                            cmd.CommandText = @"
//                            UPDATE TnDEFECTRESIN SET
//                                Del_FG=@DELFG, LastUpd_DT=@LASTUPDDT
//                            WHERE Lot_NO=@LOTNO AND Address_NO=@ADDRESS";

//                            cmd.ExecuteNonQuery();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    log.Error("UPDATE EICS ERROR:" + ex.ToString());
//                    throw ex;
//                }
//            }
		#endregion


		//#region KHLTestCheckAndUpdateLot



		///// <summary>
		///// 吸湿保管点灯試験のフラグ更新処理
		///// </summary>
		//public static void KHLTestCheckAndUpdateLot(string lotno, int procno, List<DefItem> defList)
		//{
		//    //ダイボンドで発見した場合は何もしない
		//    if (procno == KHLTEST_EXCLUDE_PROCNO) return;

		//    AsmLot lot = AsmLot.GetAsmLot(lotno);
		//    if (lot == null)
		//    {
		//        return;
		//    }


		//    foreach (DefItem def in defList)
		//    {
		//        if (def.DefectCd == KHLTEST_CAUSE_DEFECT_CD && def.DefectCt >= KHLTEST_THRESHOLD)
		//        {
		//            log.Info("[吸湿保管点灯試験]ON:" + lotno);
		//            lot.IsKHLTest = true;
		//            lot.Update();
		//        }
		//    }
		//}
		//#endregion



		/// <summary>
		/// WB工程NGマッピング用MMファイルのスキップエラーに該当するか
		/// </summary>
		/// <param name="errCode"></param>
		/// <returns></returns>
		//public static bool IsWireMMFileSkipErrorCode(string errCode)
		//{
		//    string[] codeList = Config.WireMMFileSkipErrorCodeList;
		//    errCode = errCode.ToLower();

		//    if (codeList.Contains(errCode))
		//    {
		//        return true;
		//    }
		//    else
		//    {
		//        return false;
		//    }
		//}


		//public static void InsertWireMMFileSkipError(string magno, int errct, int procno)
		//{
		//    Magazine mag = Magazine.GetCurrent(magno);
		//    if (mag == null) throw new ApplicationException("svrマガジン情報が見つかりません:" + magno);
		//    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
		//    if (lot == null) throw new ApplicationException("svrロット情報が見つかりません:" + mag.NascaLotNO);

		//    DefItem di = Config.WireMMFileSkipErrorDefItem;
		//    if (di == null) throw new ApplicationException("WBスキップ不良の設定がSvrConfig.xmlにありません");

		//    DefItem[] defs = Defect.GetAllDefect(lot, procno);

		//    bool found = false;
		//    foreach (DefItem item in defs)
		//    {
		//        if (di.CauseCd == item.CauseCd && di.ClassCd == item.ClassCd && di.DefectCd == item.DefectCd)
		//        {
		//            item.DefectCt = errct;
		//            found = true;
		//        }
		//    }

		//    if (found == false)
		//    {
		//        if (lot == null) throw new ApplicationException("svr不良明細が見つかりません:" + mag.NascaLotNO);
		//    }

		//    Defect d = new Defect();
		//    d.LotNo = lot.NascaLotNo;
		//    d.DefItems = new List<DefItem>(defs);
		//    d.ProcNo = procno;
		//    d.DeleteInsert();
		//}
	}
}
