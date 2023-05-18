using System;
using System.Data.SqlClient;

namespace GEICS
{
    /// <summary>
    /// IConnection の概要の説明です。
    /// </summary>
    public interface IConnection : IDisposable
    {
        void Commit();
        void Rollback();
        void Close();
        SqlCommand Command { get; }
        SqlConnection Connection {get;}
        SqlTransaction Transaction { get; }
    }
}
