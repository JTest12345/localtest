﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class General
	{
		public string GeneralCd { get; set; }
		public string GeneralNM { get; set; }

		public General(string generalCd, string generalNm) 
		{
			this.GeneralCd = generalCd;
			this.GeneralNM = generalNm;
		}

		public enum GeneralGrp
		{
			NonInspectionDefectItem,
			WirebonderResultCode,
			WirebonderResultCodePriority,
			WirebonderErrorCodeBadMark,
			WirebonderErrorCodeSkip,
			MachineClassName,
        }

		private static Dictionary<string, string> GetData(GeneralGrp generalgrp, string generalcd) 
		{
			Dictionary<string, string> retv = new Dictionary<string, string>();
			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT GeneralCD, GeneralNM 
					 FROM TmGeneral WITH (nolock) 
					 WHERE (DelFG = 0) AND (GeneralGrpCD = @GeneralGrpCd) ";

				if (!string.IsNullOrWhiteSpace(generalcd))
				{
					sql += " AND (GeneralCD = @GeneralCD) ";
					cmd.Parameters.Add("@GeneralCD", System.Data.SqlDbType.NVarChar).Value = generalcd;
				}

				cmd.Parameters.Add("@GeneralGrpCd", System.Data.SqlDbType.Char).Value = generalgrp.ToString();

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv.Add(rd["GeneralCD"].ToString().Trim(), rd["GeneralNM"].ToString().Trim());
					}
				}
			}
			return retv;
		}

		/// <summary>
		/// 外観検査機で検査不要の不良項目
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetNonInspectionDefectItem() 
		{
			return GetData(GeneralGrp.NonInspectionDefectItem, string.Empty);
		}

		/// <summary>
		/// ワイヤーボンダー処理CD
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetWirebonderResultCode() 
		{
			return GetData(GeneralGrp.WirebonderResultCode, string.Empty);
		}
		public static List<General> GetWirebonderResultCodeToList() 
		{
			List<General> retv = new List<General>();

			Dictionary<string, string> genes = GetWirebonderResultCode();
			foreach(KeyValuePair<string, string> gene in genes)
			{
				General g = new General(gene.Key, gene.Value);
				retv.Add(g);
			}

			return retv;
		}

		/// <summary>
		/// ワイヤーボンダー処理CDをマッピングデータへ変換する時の優先順位
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetWirebonderResultCodePriority()
		{
			return GetData(GeneralGrp.WirebonderResultCodePriority, string.Empty);
		}

		/// <summary>
		/// 周辺検査箇所ならスキップするワイヤーボンダーバッドマークエラーCD
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetWirebonderErrorCodeBadMark()
		{
			return GetData(GeneralGrp.WirebonderErrorCodeBadMark, string.Empty);
		}

		/// <summary>
		/// エラー数をカウントして不良登録するワイヤーボンダースキップエラーCD
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetWirebonderErrorCodeSkip()
		{
			return GetData(GeneralGrp.WirebonderErrorCodeSkip, string.Empty);
		}

		public static string GetMachineClassName(string classcd) 
		{
			Dictionary<string, string> list = GetData(GeneralGrp.MachineClassName, classcd);
			if (list.Count() == 0)
			{
				throw new ApplicationException(string.Format("汎用テーブルに存在しない分類CDが装置マスタに設定されています。分類CD:{0}", classcd));
			}
			else 
			{
				return list.Single().Value;
			}
		}
	}
}