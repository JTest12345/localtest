using System;
using System.Data;
using System.Data.SqlClient;
using log4net;
using System.Windows.Forms;
namespace GEICS
{
    /// <summary>
    /// NascaConnection の概要の説明です
    /// </summary>
    public class NascaConnection : IConnection
    {
        private SqlConnection conn = null;
        private SqlTransaction trans = null;
        private SqlCommand command = null;

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private NascaConnection(string connectionString, bool createTrans)
        {
            OpenConnection(connectionString, createTrans);
        }
        #endregion

        /// <summary>
        /// コネクションなどをオープンする
        /// </summary>
        /// <param name="createTrans"></param>
        private void OpenConnection(string connectionString, bool createTrans)
        {
            // UNDONE 接続文字列をXMLから読み込み

            // DB接続文字列取得
            //string connectionString = "Server=NASCA-TS-01;Database=NADB01;User=sa;password=systemadmin";
            // DB接続
            conn = new SqlConnection(connectionString);

            // DB OPEN
            try
            {
                conn.Open();
            }
            catch(Exception err)
            {
                //MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
            return new NascaConnection(connectionString, createTrans);
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
                        command.Transaction = trans;
                        command.CommandTimeout = 0;
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

        public SqlConnection Connection
        {
            get{return conn;}
        }

        /// <summary>
        /// コミット
        /// </summary>
        public void Commit()
        {
#if TEST
#else
            if (trans != null)
            {
                trans.Commit();
            }
#endif
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