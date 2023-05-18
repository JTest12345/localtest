using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmsApi.Model;
using ArmsApi;
using System.Data;
using System.Data.SqlClient;
using ArmsApi.Model.NASCA;

namespace ArmsNascaBridge
{
    /// <summary>
    /// 高効率用　搭載済みロット取り込み
    /// </summary>
    class LotImport
    {
        public static void Import()
        {
            try
            {
                //高効率限定処理 ⇒　限定をやめる 2018.06.06
                //やめた理由：MAPとNTSVの分岐があるし、MELFrameLoadersに設定がなければ処理されないので。
                //　　　　　　プロファイルIDの取得に失敗したデータのリカバリーは処理されても問題ない。
                //if (Config.GetLineType == Config.LineTypes.MEL_SV || Config.GetLineType == Config.LineTypes.MEL_MAP
                //    || Config.GetLineType == Config.LineTypes.MEL_GAM
                //    || Config.GetLineType == Config.LineTypes.MEL_19
                //    || Config.GetLineType == Config.LineTypes.MEL_MPL || Config.GetLineType == Config.LineTypes.MEL_83385
                //    || Config.GetLineType == Config.LineTypes.MEL_COB
                //    || Config.GetLineType == Config.LineTypes.MEL_SIGMA
                //    || Config.GetLineType == Config.LineTypes.MEL_NTSV || Config.GetLineType == Config.LineTypes.MEL_VOYAGER
                //    || Config.GetLineType == Config.LineTypes.MEL_KIRAMEKI || Config.GetLineType == Config.LineTypes.MEL_CSP
                //    || Config.GetLineType == Config.LineTypes.MEL_093)
                //{
                    if (Config.GetLineType == Config.LineTypes.MEL_MAP || Config.GetLineType == Config.LineTypes.MEL_NTSV)
					{
						string[] lotlist = GetLotListMAP(); //搭載実績のないデータも取り込む
						foreach (string lot in lotlist)
						{
                            if (ImportCancelJudge(lot))
                            {
                                // 取り消し対象の場合、取り込まない。
                                continue;
                            }
							ArmsApi.Model.NASCA.Importer.ImportAsmLot(lot, Config.Settings.SectionCd, new Action<string>(Log.SysLog.Info));
                        }
					}
					else
					{
                        if (Config.Settings.MELFrameLoaders != null)
                        {
                            foreach (string plantcd in Config.Settings.MELFrameLoaders)
                            {
                                string[] lotlist = GetLotList(plantcd);

                                foreach (string lot in lotlist)
                                {
                                    if (ImportCancelJudge(lot))
                                    {
                                        // 取り消し対象の場合、取り込まない。
                                        continue;
                                    }

                                    //if (lot != "B1784B03500")
                                    //    continue;

                                    ArmsApi.Model.NASCA.Importer.ImportAsmLot(lot, Config.Settings.SectionCd, new Action<string>(Log.SysLog.Info));
                                }
                            }
                        }
					}

					//プロファイルIDの取得に失敗したデータのリカバリー
					string[] lots = GetLotListProfileNG();
					foreach (string lot in lots)
					{
                        //if (lot != "B1784B03500")
                        //    continue;

                        AsmLot svrlot = AsmLot.GetAsmLot(lot);
						svrlot.ProfileId = Importer.GetProfileNo(svrlot.TypeCd, svrlot.DBThrowDT);
						if (svrlot.ProfileId == 0)
						{
							continue;
						}
						svrlot.Update();
					}
                //}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge] LotImport Error:" + err.ToString());
			}
		}

        /// <summary>
        /// MAP高効率用
        /// </summary>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        public static string[] GetLotListMAP()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@FROM", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-7);

                //2011.09.20 湯浅君の強い要望で品目DBC縛り。
                //将来的にこの部分で取り込み漏れが出た場合は、運用手順含めて再検証が必要
                //cmd.CommandText = string.Format(@"
                //    select
                //     o.material_cd, s.lot_no
                //    from rvtordh o (nolock)
                //    inner join nttsshj s (nolock)
                //    on s.mnfctinst_no = o.mnfctinst_no
                //    where startsch_dt >= @FROM
                //    and material_cd LIKE '%.{0}'
                //    and o.del_fg = '0'", Config.Settings.ImportLotLineState);

                cmd.CommandText = string.Format(@"
                    select
                     o.material_cd, s.lot_no
                    from rvtordh o (nolock)
                    inner join nttsshj s (nolock)
                    on s.mnfctinst_no = o.mnfctinst_no
                    where startsch_dt >= @FROM
                    and process_cd in ({0})
                    and o.del_fg = '0'
                    OPTION(maxdop 1) ", string.Join(",", Config.Settings.LotImportOfTargetNascaProcess));
                
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string matcd = rd["material_cd"].ToString().Trim();
                        string lotno = rd["lot_no"].ToString().Trim();
                        lotno = lotno.Replace('I', 'i');

                        //ワークフローに無い場合は無視
                        if (hasWorkFlow(matcd) == false) continue;

                        //2016.03.01 品目統合一時対応
                        string typecd = matcd.Split('.').First();
                        if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typecd))
                        {
                            continue;
                        }

                        AsmLot exists = AsmLot.GetAsmLot(lotno);
                        if (exists == null)
                        {
                            retv.Add(lotno);
                        }
                        else
                        {
                            //Tranレコードが一件もない場合も採り直し
                            Order[] orders = Order.GetOrder(exists.NascaLotNo);
                            if (orders.Length == 0)
                            {
                                retv.Add(lotno);
                            }
                        }
                    }
                }

                //2016.03.01 品目統合一時対応
                cmd.CommandText = @"
                select
                    o.material_cd, s.lot_no
                    from rvtordh o (nolock)
                    inner join nttsshj s (nolock)
                    on s.mnfctinst_no = o.mnfctinst_no
                    where startsch_dt >= @FROM
                    and material_cd = @MATERIALCD
                    and o.del_fg = '0'";
                SqlParameter prm = cmd.Parameters.Add("@MATERIALCD", SqlDbType.Char);
                foreach (string typecd in Config.Settings.UnificationTargetTypeList)
                {
                    prm.Value = typecd + ".DBA";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string matcd = rd["material_cd"].ToString().Trim();
                            string lotno = rd["lot_no"].ToString().Trim();
                            lotno = lotno.Replace('I', 'i');

                            //ワークフローに無い場合は無視
                            if (hasWorkFlow(matcd) == false) continue;

                            AsmLot exists = AsmLot.GetAsmLot(lotno);
                            if (exists == null)
                            {
                                retv.Add(lotno);
                            }
                            else
                            {
                                //Tranレコードが一件もない場合も採り直し
                                Order[] orders = Order.GetOrder(exists.NascaLotNo);
                                if (orders.Length == 0)
                                {
                                    retv.Add(lotno);
                                }
                            }
                        }
                    }
                }
            }

            return retv.ToArray();
        }


		/// <summary>
		/// 19高効率用
		/// </summary>
		/// <param name="plantcd"></param>
		/// <returns></returns>
		public static string[] GetLotList19()
		{
			List<string> retv = new List<string>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				cmd.Parameters.Add("@FROM", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-7);

				//cmd.CommandText = string.Format(@"
    //                select
    //                 o.material_cd, s.lot_no
    //                from rvtordh o (nolock)
    //                inner join nttsshj s (nolock)
    //                on s.mnfctinst_no = o.mnfctinst_no
    //                where startsch_dt >= @FROM
    //                and material_cd LIKE '%.{0}'
    //                and o.del_fg = '0'", Config.Settings.ImportLotLineState);

                cmd.CommandText = string.Format(@"
                    select
                     o.material_cd, s.lot_no
                    from rvtordh o (nolock)
                    inner join nttsshj s (nolock)
                    on s.mnfctinst_no = o.mnfctinst_no
                    where startsch_dt >= @FROM
                    and process_cd in ({0})
                    and o.del_fg = '0'
                    OPTION(maxdop 1) ", string.Join(",", Config.Settings.LotImportOfTargetNascaProcess));

                using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						string matcd = rd["material_cd"].ToString().Trim();
						string lotno = rd["lot_no"].ToString().Trim();
						lotno = lotno.Replace('I', 'i');

						//ワークフローに無い場合は無視
						if (hasWorkFlow(matcd) == false) continue;

						AsmLot exists = AsmLot.GetAsmLot(lotno);
						if (exists == null)
						{
							retv.Add(lotno);
						}
						else
						{
							//Tranレコードが一件もない場合も採り直し
							Order[] orders = Order.GetOrder(exists.NascaLotNo);
							if (orders.Length == 0)
							{
								retv.Add(lotno);
							}
						}
					}
				}
			}

			return retv.ToArray();
		}


        /// <summary>
        /// ワークフローマスタに存在する場合はTrue
        /// </summary>
        /// <param name="matcd"></param>
        /// <returns></returns>
        private static bool hasWorkFlow(string matcd)
        {
            if (string.IsNullOrEmpty(matcd)) return false;
            string typecd = matcd.Split('.')[0];

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
                cmd.CommandText = "SELECT COUNT(1) FROM TmWorkFlow (NOLOCK) WHERE typecd=@TYPECD AND delfg=0";

                if (Convert.ToInt32(cmd.ExecuteScalar()) >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        /// <summary>
        /// SV高効率用
        /// </summary>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        public static string[] GetLotList(string plantcd)
        {
            List<string> retv = new List<string>();

			//if (Config.Settings.LotImportOfTargetNascaProcess == null)
			//{
			//	throw new ApplicationException("取り込み対象の工程NOが設定ファイルで設定されていません。設定名:LotImportOfTargetNascaProcess");
			//}

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = plantcd;
                cmd.Parameters.Add("@FROM", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-7);

                //2011.09.20 湯浅君の強い要望で品目DBC縛り。
                //将来的にこの部分で取り込み漏れが出た場合は、運用手順含めて再検証が必要
                //2013.08.23 負荷対策で湯浅君推奨の工程CD縛りに変更

//				cmd.CommandText = @"
//                    SELECT b.Lot_NO FROM dbo.NttSJSB AS b WITH(nolock)
//                    INNER JOIN dbo.RvtTRANH AS t WITH(nolock) ON b.MnfctRsl_NO = t.mnfctrsl_no 
//                    INNER JOIN dbo.RvtORDH AS o WITH(nolock) ON t.mnfctinst_no = o.mnfctinst_no
//                    WHERE (b.Complt_DT >= @FROM) AND (b.Plant_CD = @PLANTCD) 
//                    AND (b.Del_FG = 0) AND (t.del_fg = 0) AND (o.del_fg = 0) 
//					AND (o.process_cd in (5371, 28372, 28575, 23652))
//					OPTION (maxdop 1) ";

				cmd.CommandText = string.Format(@"
                    SELECT b.Lot_NO FROM dbo.NttSJSB AS b WITH(nolock)
                    INNER JOIN dbo.RvtTRANH AS t WITH(nolock) ON b.MnfctRsl_NO = t.mnfctrsl_no 
                    INNER JOIN dbo.RvtORDH AS o WITH(nolock) ON t.mnfctinst_no = o.mnfctinst_no
                    WHERE (b.Complt_DT >= @FROM) AND (b.Plant_CD = @PLANTCD) 
                    AND (b.Del_FG = 0) AND (t.del_fg = 0) AND (o.del_fg = 0) 
					AND (o.process_cd in ({0}))
					OPTION (maxdop 1) ", string.Join(",", Config.Settings.LotImportOfTargetNascaProcess));

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string lotno = rd["lot_no"].ToString().Trim();
                        lotno = lotno.Replace('I', 'i');

                        AsmLot exists = AsmLot.GetAsmLot(lotno);
                        if (exists == null)
                        {
                            retv.Add(lotno);
                        }
                        else
                        {
                            //Tranレコードが一件もない場合も採り直し
                            Order[] orders = Order.GetOrder(exists.NascaLotNo);
                            if (orders.Length == 0)
                            {
                                retv.Add(lotno);
                            }
                        }
                    }
                }
            }

            return retv.ToArray();
        }


        /// <summary>
        /// プロファイルIDの取得に失敗したロット
        /// </summary>
        /// <returns></returns>
        public static string[] GetLotListProfileNG()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @" SELECT TnLot.lotno FROM TnLot WITH(nolock) 
                                        INNER JOIN TnMag WITH(nolock) ON TnLot.lotno = TnMag.lotno 
                                        WHERE TnMag.newfg = 1 AND profileid = 0 ";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string lotno = rd["lotno"].ToString().Trim();
                        retv.Add(lotno);
                    }
                }
            }
            return retv.ToArray();
        }

        /// <summary>
        /// 取り込み対象外の判定 (取り込み対象外 = true)
        /// </summary>
        /// <returns></returns>
        private static bool ImportCancelJudge(string lotno)
        {
            string[] armsConSTRList = Config.Settings.ArmsConSTRList;

            if (armsConSTRList == null || armsConSTRList.Length == 0)
            {
                return false;
            }

            // 他ライン(自動搬送ライン)に任意の作業実績があったら、取り込み対象外にする。
            foreach (string constr in armsConSTRList)
            {
                if (string.IsNullOrWhiteSpace(constr))
                {
                    continue;
                }

                Order[] orderList = Order.SearchOrder(lotno, null, null, null, null, false, false, null, null,null, null, constr);

                if (orderList != null && orderList.Length > 0)
                {
                    return true;
                }
            }
            
            return false;
        }

    }
}
