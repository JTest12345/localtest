using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
	public class FileScan
	{
		public string ModelNM { get; set; }
		public string PrefixNM { get; set; }

		/// <summary>
		/// HeaderScanInRowFG = true
		/// item1, item2, item3, item4,…
		/// data1, data2, data3, data4,…
		/// 
		/// HeaderScanInRowFG = false
		/// item1, data1,…
		/// item2, data2,…
		/// item3, data3,…
		/// </summary>
		public bool HeaderScanInRowFG { get; set; }
		public bool DelFG { get; set; }
		public bool StartUpFG { get; set; }

		public static List<FileScan> GetDataList(int lineCd, string modelNm, string prefixNm, bool startUpFg)
		{
			try
			{
				List<FileScan> retv = new List<FileScan>();

				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
				{
					string sql = @" SELECT TmFILESCAN.Model_NM, TmFILESCAN.Prefix_NM, TmFILESCAN.StartUp_FG, TmFILESCAN.HeaderScanInRow_FG, TmFILESCAN.Del_FG
                                FROM TmFILESCAN WITH(nolock) 
                                WHERE (TmFILESCAN.Model_NM = @ModelNM)  
                                AND (TmFILESCAN.Del_FG = 0) AND (TmFILESCAN.StartUp_FG = @StartUpFG) ";

					if (string.IsNullOrEmpty(prefixNm) == false)
					{
						sql += " AND (TmFILESCAN.Prefix_NM = @PrefixNM) ";
						conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);
					}

					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
					conn.SetParameter("@StartUpFG", SqlDbType.Bit, startUpFg);

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordModelNM = rd.GetOrdinal("Model_NM");
						int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
						int ordHeaderScanInRowFG = rd.GetOrdinal("HeaderScanInRow_FG");
						int ordDelFG = rd.GetOrdinal("Del_FG");
						int ordStartUpFG = rd.GetOrdinal("StartUp_FG");

						while (rd.Read())
						{
							FileScan fs = new FileScan();
							fs.ModelNM = rd.GetString(ordModelNM).Trim();
							fs.PrefixNM = rd.GetString(ordPrefixNM).Trim();
							fs.HeaderScanInRowFG = rd.GetBoolean(ordHeaderScanInRowFG);
							fs.DelFG = rd.GetBoolean(ordDelFG);
							fs.StartUpFG = rd.GetBoolean(ordStartUpFG);

							retv.Add(fs);
						}
					}
				}

				return retv;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// TmFileScanの引数で指定した対象レコードを取得する
		/// </summary>
		/// <param name="inlineCD"></param>
		/// <param name="modelNM"></param>
		/// <param name="fileKind"></param>
		/// <returns>取得できるレコードが無い場合はNullを返す</returns>
		public static FileScan GetSingle(int inlineCD, string modelNM, string fileKind)
		{
			List<FileFmt> fileFmtList = FileFmt.GetData(inlineCD, modelNM, true, fileKind, null);

			List<FileScan> fileScanList = FileScan.GetDataList(inlineCD, modelNM, fileKind, true);
			FileScan fileScan;

			if (fileScanList.Count == 0 && fileFmtList.Count == 0)
			{
				return null;
			}
			else if (fileScanList.Count > 0 && fileFmtList.Count == 0)
			{
				throw new ApplicationException(
					string.Format("TmFileScanﾏｽﾀ設定済みですがTmFILEFMT未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
					modelNM, fileKind));
			}
			else if (fileScanList.Count == 0 && fileFmtList.Count > 0)
			{
				throw new ApplicationException(
					string.Format("TmFILEFMTﾏｽﾀ設定済みですがTmFileScan未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
					modelNM, fileKind));
			}
			else if (fileScanList.Count == 1)
			{
				fileScan = fileScanList.Single();
			}
			else
			{
				throw new ApplicationException(
					string.Format("TmFileScanから複数のﾏｽﾀが取得されました。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}", modelNM, fileKind));
			}

			return fileScan;
		}

	}
}
