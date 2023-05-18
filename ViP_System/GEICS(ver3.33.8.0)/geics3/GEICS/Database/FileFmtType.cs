using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace GEICS.Database
{
	class FileFmtType
	{
		public string TypeCD { get; set; }
		public int FileFmtNo { get; set; }
		public int FrameNo { get; set; }

		public static List<FileFmtType> GetFromQCIL(string typeCD, int? fileFmtNo, int? frameNo)
		{
			List<FileFmtType> fileFmtTypeList = new List<FileFmtType>();

			using (DBConnect conn = DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT Type_CD, FileFmt_NO, Frame_NO FROM TmFILEFMTTYPE WHERE Del_FG = 0 ";

				if (string.IsNullOrEmpty(typeCD) == false)
				{
					sql += " AND Type_CD = @type ";
					conn.SetParameter("@type", System.Data.DbType.String, typeCD);
				}

				if (fileFmtNo.HasValue)
				{
					sql += " AND FileFmt_NO = @fmtNo ";
					conn.SetParameter("@fmtNo", System.Data.DbType.Int32, fileFmtNo.Value);
				}

				if (frameNo.HasValue)
				{
					sql += " AND Frame_NO = @frameNo ";
					conn.SetParameter("@frameNo", System.Data.DbType.Int32, frameNo.Value);
				}

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int ordType = rd.GetOrdinal("Type_CD");
					int ordFmt = rd.GetOrdinal("FileFmt_NO");
					int ordFrame = rd.GetOrdinal("Frame_NO");

					while (rd.Read())
					{
						FileFmtType fileFmtType = new FileFmtType();

						fileFmtType.TypeCD = rd.GetString(ordType).Trim();
						fileFmtType.FileFmtNo = rd.GetInt32(ordFmt);
						//fileFmtType.FrameNo = rd.GetInt32(ordFrame);

						fileFmtTypeList.Add(fileFmtType);
					}
				}
			}
			return fileFmtTypeList;
		}

		public static List<FileFmtType> GetFromLENS(string typeCD, int? fileFmtNo, int? frameNo)
		{
			List<FileFmtType> fileFmtTypeList = new List<FileFmtType>();

			string connStr = string.Format(GEICS.Properties.Settings.Default.ConStrServ, Constant.EmployeeInfo.ServerInstance, Constant.LENS_DB_NM,
				ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);

			using (DBConnect conn = DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TypeCD, FileFmtNO, MagazineID FROM TmType WHERE DelFG = 0 ";

				if (string.IsNullOrEmpty(typeCD) == false)
				{
					sql += " AND TypeCD = @type ";
					conn.SetParameter("@type", System.Data.DbType.String, typeCD);
				}

				if (fileFmtNo.HasValue)
				{
					sql += " AND FileFmtNO = @fmtNo ";
					conn.SetParameter("@fmtNo", System.Data.DbType.Int32, fileFmtNo.Value);
				}

				if (frameNo.HasValue)
				{
					sql += " AND MagazineID = @frameNo ";
					conn.SetParameter("@frameNo", System.Data.DbType.Int32, frameNo.Value);
				}

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int ordType = rd.GetOrdinal("TypeCD");
					int ordFmt = rd.GetOrdinal("FileFmtNO");
					int ordFrame = rd.GetOrdinal("MagazineID");

					while (rd.Read())
					{
						FileFmtType fileFmtType = new FileFmtType();

						fileFmtType.TypeCD = rd.GetString(ordType).Trim();
						fileFmtType.FileFmtNo = rd.GetInt32(ordFmt);
						//fileFmtType.FrameNo = rd.GetInt32(ordFrame);

						fileFmtTypeList.Add(fileFmtType);
					}
				}
			}
			return fileFmtTypeList;
		}
	}
}
