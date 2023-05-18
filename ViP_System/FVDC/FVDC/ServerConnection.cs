using System;
using System.Data;
using System.Data.SqlClient;

namespace FVDC
{
	/// <summary>
	/// ServerConnection の概要の説明です。
	/// </summary>
	public class ServerConnection : IConnection
	{

        private SqlConnection conn      = null;
        private SqlConnection checkconn = null;
		private SqlTransaction trans    = null;
		private SqlCommand command      = null;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ServerConnection(string connectionString, bool createTrans)
		{
            /// ↓↓↓↓↓↓　2018/01/12 SLA2.Uchida　N工場MAPラインのみARMS以外のサーバーが別になるので切り替える　↓↓↓↓↓↓
            if ((Transfer.ServerName.Contains("172.16.34.201"))
                && (!connectionString.Contains("Database=ARMS")) && (!connectionString.Contains("Database=LENS")))
            {
                connectionString = string.Format(connectionString, "172.16.34.219\\SQLExpress");
            }
            /// ↑↑↑↑↑↑  2018/01/12 SLA2.Uchida　N工場MAPラインのみARMS以外のサーバーが別になるので切り替える  ↑↑↑↑↑↑
            /// ↓↓↓↓↓↓　2018/06/13 SLA2.Uchida　三方工場SVラインのARMS以外のサーバーが別になるので切り替える　↓↓↓↓↓↓
            else if ((Transfer.ServerName.Contains("192.168.209.23"))
                && (!connectionString.Contains("Database=ARMS")) && (!connectionString.Contains("Database=LENS")))
            {
                connectionString = string.Format(connectionString, "192.168.209.7\\SQLExpress");
            }
            /// ↑↑↑↑↑↑  2018/06/13 SLA2.Uchida　三方工場SVラインのARMS以外のサーバーが別になるので切り替える  ↑↑↑↑↑↑
            /// ↓↓↓↓↓↓　2019/02/22 SLA2.Uchida　アオイMAP自動搬送ラインのARMS以外のサーバーが別になるので切り替える　↓↓↓↓↓↓
            else if ((Transfer.ServerName.Contains("172.19.19.235"))
                && (!connectionString.Contains("Database=ARMS")) && (!connectionString.Contains("Database=LENS")))
            {
                connectionString = string.Format(connectionString, "172.19.20.57\\SQLExpress");
            }
            /// ↑↑↑↑↑↑  2019/02/22 SLA2.Uchida　アオイMAP自動搬送ラインのARMS以外のサーバーが別になるので切り替える  ↑↑↑↑↑↑
            /// ↓↓↓↓↓↓　2018/12/06 SLA2.Uchida　日亜NTSV/車載ラインのARMS以外のサーバーが別になるので切り替える　↓↓↓↓↓↓
            else if ((Transfer.ServerName.Contains("172.21.208.225"))
                && (!connectionString.Contains("Database=ARMS")) && (!connectionString.Contains("Database=LENS")))
            {
                connectionString = string.Format(connectionString, "172.21.208.221\\SQLExpress");
            }
            else if ((Transfer.ServerName.Contains("172.21.193.95"))
                && (!connectionString.Contains("Database=ARMS")) && (!connectionString.Contains("Database=LENS")))
            {
                connectionString = string.Format(connectionString, "172.21.193.116\\SQLExpress");
            }
            /// ↑↑↑↑↑↑  2018/12/06 SLA2.Uchida　日亜NTSV/車載ラインのARMS以外のサーバーが別になるので切り替える  ↑↑↑↑↑↑

            if (CheckConnection(connectionString))
			    OpenConnection(connectionString, createTrans);
		}
		#endregion

        /// <summary>
        /// コネクションなどをオープンする
        /// </summary>
        /// <param name="createTrans"></param>
        private bool CheckConnection(string connectionString)
        {
            // サーバー名設定（サーバー名が無いときTransfer.ServerNameの内容に設定）
            string connectionServer     = connectionString;
            try
            {
                connectionServer        = string.Format(connectionString, Transfer.ServerName);
            }
            catch
            { }
            // DB接続
            checkconn                   = new SqlConnection(connectionServer);
            // DB OPEN
            try
            {
                checkconn.Open();
                /// 問題無ければすぐにクローズする。
                if (checkconn != null)
                {
                    checkconn.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

		/// <summary>
		/// コネクションなどをオープンする
		/// </summary>
		/// <param name="createTrans"></param>
		private void OpenConnection(string connectionString, bool createTrans) 
		{
			// サーバー名設定（サーバー名が無いときTransfer.ServerNameの内容に設定）
			string connectionServer		= connectionString;
			try
			{
				connectionServer		= string.Format(connectionString, Transfer.ServerName);
			}
			catch
			{}
			// DB接続
			conn                        = new SqlConnection(connectionServer);
			// DB OPEN
            conn.Open();

			if (createTrans) 
			{
				// Start Transaction
				trans = conn.BeginTransaction();
			}
		}

		/// <summary>
		/// コネクションを生成する
		/// </summary>
		/// <returns></returns>
		public static IConnection CreateInstance(string connectionString, bool createTrans)
		{
			return new ServerConnection(connectionString, createTrans);
		}

		/// <summary>
		/// コネクションを生成する（トランザクションも開始する）
		/// </summary>
		/// <returns></returns>
		public static IConnection CreateInstance()
		{
			return CreateInstance("", true);
		}

		/// <summary>
		/// SqlCommand
		/// </summary>
		public SqlCommand Command
		{
			get
			{
				if (command == null) 
				{
					// Create Command
					command = conn.CreateCommand();
					command.Connection = conn;
					if (trans != null) 
					{
						command.Transaction		= trans;
						command.CommandTimeout  = 0;
					}
					command.CommandText = "SET XACT_ABORT ON";
					command.ExecuteScalar();
				}
				return command;
			}
		}

		/// <summary>
		/// SqlTransaction
		/// </summary>
		/// <returns></returns>
		public SqlTransaction Transaction
		{
			get
			{
				return trans;
			}
		}

		/// <summary>
		/// コミット
		/// </summary>
		public void Commit()
		{
			if (trans != null) 
			{
				trans.Commit();
			}
		}

		/// <summary>
		/// ロールバック
		/// </summary>
		public void Rollback()
		{
			if (trans != null) 
			{
				trans.Rollback();
			}
		}

		/// <summary>
		/// コネクションのクローズ
		/// </summary>
		public void Close()
		{
			if (trans != null)
			{
				trans.Dispose();
			}

			if (command != null)
			{
				command.Dispose();
			}

			if (conn != null) 
			{
				conn.Dispose();
			}
		}

		#region IDisposable実装

		/// <summary>
		/// IDisposable.Dispose
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		private void Dispose(bool disposing)
		{
			Close();

			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}
		#endregion
	}
}


