/*************************************************************************************
 * �V�X�e����     : �V�X�e��
 *  
 * ������         : CommonIO
 * 
 * �T��           : �e�[�u���̓��o�͂ŋ��ʓI�ȏ������W�߂��N���X
 * 
 * �쐬           : 2006/10/25 ���c(SLA)
 * 
 * �C������       : 
 ************************************************************************************/
 
using System;
using System. Collections. Generic;
using System. Text;
using System. Data;
using System. Data. SqlClient;
using System. Windows.Forms;

namespace FVDC
{	
	/// <summary>
	/// �e�[�u���̓��o�͂ŋ��ʓI�ȏ������W�߂��N���X
	/// </summary>
	public class CommonIO
	{
		/// <summary>
		/// �Ǎ�
		/// </summary>
		/// <param name="ConnectServer">�ڑ���</param>
		/// <param name="TableName">�e�[�u����</param>
		/// <param name="WhereSql">��������</param>
		/// <param name="dsTable">DataTable	dsTable	= �f�[�^�Z�b�g.�f�[�^�e�[�u��.Copy();</param>
		/// <returns></returns>
		public bool Read(string ConnectServer, string TableName, string WhereSql, ref DataTable dsTable)
		{
//			/// �f�[�^���N���A����
			if (dsTable != null) dsTable.Clear();

			string SelectSql		= "SELECT ";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

			/// �Ǎ��ΏۃJ�����ݒ�
			for(int j = 0; j < dsTable.Columns.Count; j++)
			{
				SelectSql			= SelectSql + "," + dsTable.Columns[j].ColumnName;
			}
			SelectSql				= SelectSql.Replace(" ,", " ");

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// �ǂ�
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					reader = connect.Command.ExecuteReader();

					for ( int i = 0; reader.Read(); i++)
					{
						object[] DetailData	= new object[dsTable.Columns.Count];
						for(int j = 0; j < dsTable.Columns.Count; j++)
						{
							DetailData[j]	= reader[dsTable.Columns[j].ColumnName];
						}
						/// �f�[�^�Z�b�g�e�[�u���ɒǉ�����
						dsTable.Rows.Add(DetailData);
					}
				}
				catch (SqlException ex)
				{
					///TODO: �G���[����
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Read");
					return false;
				}
				finally
				{
					if (reader != null)	reader.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// �Ǎ�
		/// </summary>
		/// <param name="ConnectServer">�ڑ���</param>
		/// <param name="TableName">�e�[�u����</param>
		/// <param name="WhereSql">��������</param>
		/// <param name="dsTable">DataTable	dsTable	= �f�[�^�Z�b�g.�f�[�^�e�[�u��.Copy();</param>
		/// <param name="RetryFG">�f�b�h���b�N�E�^�C���I�[�o�[�̂Ƃ�ON</param>
		/// <returns></returns>
		public bool Read(string ConnectServer, string TableName, string WhereSql, ref DataTable dsTable, ref bool RetryFG)
		{
			/// �f�[�^���N���A����
			if (dsTable != null) dsTable.Clear();
			RetryFG					= false;

			string SelectSql		= "SELECT ";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

			/// �Ǎ��ΏۃJ�����ݒ�
			for(int j = 0; j < dsTable.Columns.Count; j++)
			{
				SelectSql			= SelectSql + "," + dsTable.Columns[j].ColumnName;
			}
			SelectSql				= SelectSql.Replace(" ,", " ");

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// �ǂ�
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					reader = connect.Command.ExecuteReader();

					for ( int i = 0; reader.Read(); i++)
					{
						object[] DetailData	= new object[dsTable.Columns.Count];
						for(int j = 0; j < dsTable.Columns.Count; j++)
						{
							DetailData[j]	= reader[dsTable.Columns[j].ColumnName];
						}
						/// �f�[�^�Z�b�g�e�[�u���ɒǉ�����
						dsTable.Rows.Add(DetailData);
					}
				}
				catch (SqlException ex)
				{
					///�f�b�h���b�N�E�^�C���I�[�o�[�̂Ƃ��͏������J��Ԃ�
					if (ex.Number == 1205 || ex.Number == -20 || ex.Number == -2)	
					{
						RetryFG				= true;
						return true;
					}
					///TODO: �G���[����
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Read");
					return false;
				}
				finally
				{
					if (reader != null)	reader.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// �ő�l�Ǎ�
		/// </summary>
		/// <param name="ConnectServer">�ڑ���</param>
		/// <param name="TableName">�e�[�u����</param>
		/// <param name="WhereSql">��������</param>
		/// <param name="dsTable">DataTable	dsTable	= �f�[�^�Z�b�g.�f�[�^�e�[�u��.Copy();</param>
		/// <returns></returns>
		public bool MaxRead(string ConnectServer, string TableName, string ItemName, string WhereSql, ref object MaxData)
		{
			string SelectSql		= "SELECT MAX(" + ItemName + ")";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// �ǂ�
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					MaxData = connect.Command.ExecuteScalar();
				}
				catch (SqlException ex)
				{
					///TODO: �G���[����
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO MaxRead");
					return false;
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// �X�V
		/// </summary>
		/// <param name="ConnectServer">�ڑ���</param>
		/// <param name="TableName">�e�[�u����</param>�@
		/// <param name="dsTable">�ǉ��f�[�^</param>
		/// <returns></returns>
		public bool Update(string ConnectServer, string TableName, string UpdateChar, string WhereChar)
		{
			string sql = "UPDATE " + TableName  + " SET " + UpdateChar + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
			{
				SqlDataReader reader = null;
				try
				{
					/// �폜
					connect.Command.CommandText =  sql;
					int ExecCount = connect.Command.ExecuteNonQuery();

					if (ExecCount > 0)
					{
						connect.Commit();
					}
				}
				catch (SqlException ex)
				{
					///TODO: �G���[����
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Update");
					return false;
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}

        /// <summary>
        /// �X�V(�V��)
        /// </summary>
        /// <param name="ConnectServer"></param>
        /// <param name="TableName"></param>
        /// <param name="IX"></param>
        /// <param name="dsTable_OLD"></param>
        /// <param name="dsTable_NEW"></param>
        /// <returns></returns>
        public bool Update(string ConnectServer, string TableName, int IX, DataTable dsTable_OLD, DataTable dsTable_NEW)
        {
            string UpdateChar           = "@";
            string WhereChar            = "@";

            /// �f�[�^���X�V���Ղ������^�C�v�ɐ��^����
            DataCast objDataCast        = new DataCast();
            string[] stgNewItem         = new string[dsTable_NEW.Columns.Count];
            DataRow dsRow               = dsTable_NEW.Rows[IX];
            objDataCast.dsRowCast(dsRow, ref stgNewItem);

            /// �X�V�Ώېݒ�
            for (int i = 1; i < dsTable_NEW.Columns.Count; i++)
            {
                UpdateChar              += "," + dsTable_NEW.Columns[i].ColumnName + " = " + stgNewItem[i];
            }
            UpdateChar                  = UpdateChar.Replace("@,", "");

            /// �f�[�^���������Ղ������^�C�v�ɐ��^����
            string[] stgOldItem         = new string[dsTable_OLD.Columns.Count];
            DataRow dsRow_OLD           = dsTable_OLD.Rows[IX];
            objDataCast.dsRowCast(dsRow_OLD, ref stgOldItem);

            /// �����Ώېݒ�i�O�S���ڂ������ΏۂƂ���j
            for (int i = 1; (i < 5 && i < dsTable_OLD.Columns.Count); i++)
            {
                WhereChar               += " AND " + dsTable_OLD.Columns[i].ColumnName + " = " + stgOldItem[i];
            }
            WhereChar                   = WhereChar.Replace("@ AND ", " WHERE ");


            string sql                  = "UPDATE " + TableName + " SET " + UpdateChar + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                SqlDataReader reader    = null;
                try
                {
                    /// �X�V
                    connect.Command.CommandText = sql;
                    int ExecCount       = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: �G���[����
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Update");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }

		/// <summary>
		/// �ǉ�
		/// </summary>
		/// <param name="ConnectServer">�ڑ���</param>
		/// <param name="TableName">�e�[�u����</param>�@
		/// <param name="dsTable">�ǉ��f�[�^</param>
		/// <returns></returns>
        public bool Insert(string ConnectServer, string TableName, int IX, DataTable dsTable)
		{
			DataCast	objDataCast		= new DataCast();
			string IntoSql				= "INSERT INTO " + TableName + "("; 
			string ValuesSql			= " VALUES";

			/// �ǉ��ΏۃJ�����ݒ�
			for(int i = 1; i < dsTable.Columns.Count; i++)
			{
				IntoSql					= IntoSql + "," + dsTable.Columns[i].ColumnName;
			}
			IntoSql						= IntoSql.Replace("(,", "(") + ")";
						
            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
			{
				SqlDataReader reader = null;
				try
				{
					/// �f�[�^���X�V���Ղ������^�C�v�ɐ��^����
					string[] stgItem			= new string[dsTable.Columns.Count];
                    DataRow dsRow               = dsTable.Rows[IX];
					objDataCast.dsRowCast(dsRow, ref stgItem);
					/// �ǉ��ΏۃJ�����ݒ�
					string InsData				= "(";
					for(int j = 1; j < dsTable.Columns.Count; j++)
					{
						InsData					= InsData + "," + stgItem[j];
					}
					InsData						= InsData.Replace("(,", "(") + ")";

					/// �ǉ�
					connect.Command.CommandText =  IntoSql + ValuesSql + InsData;
					int ExecCount = connect.Command.ExecuteNonQuery();

					if (ExecCount > 0)
					{
						connect.Commit();
					}
				}
				catch (SqlException ex)
				{
					switch(ex.Number)
					{
						case 11:		/// �l�b�g���[�N�̈�ʃG���[
							break;
						case 2627:		/// �d���G���[
						case 8152:		/// �����ӂ�G���[
							break;		/// ���̂܂܂n�j�Ƃ���
						default:
							///TODO: �G���[����
							MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Insert");
							return false;
					}
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}
                
        /// <summary>
        /// �폜
        /// </summary>
        /// <param name="ConnectServer">�ڑ���</param>
        /// <param name="TableName">�e�[�u����</param>
        /// <param name="WhereSql">��������</param>
        /// <returns></returns>
        public bool Delete(string ConnectServer, string TableName, string WhereSql)
        {
            string sql = "DELETE FROM " + TableName + " " + WhereSql;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                SqlDataReader reader = null;
                try
                {
                    /// �폜
                    connect.Command.CommandText = sql;
                    int ExecCount = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: �G���[����
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Delete");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }

        /// <summary>
        /// �폜
        /// </summary>
        /// <param name="ConnectServer">�ڑ���</param>
        /// <param name="TableName">�e�[�u����</param>
        /// <param name="WhereSql">��������</param>
        /// <returns></returns>
        public bool Delete(string ConnectServer, string TableName, int IX, DataTable dsTable)
        {            
            /// �f�[�^���������Ղ������^�C�v�ɐ��^����
            DataCast objDataCast        = new DataCast();
            string[] stgOldItem         = new string[dsTable.Columns.Count];
            DataRow dsRow_OLD           = dsTable.Rows[IX];
            objDataCast.dsRowCast(dsRow_OLD, ref stgOldItem);

            /// �����Ώېݒ�i�S���ڂ������ΏۂƂ���j
            string WhereChar            = "@";
            for (int i = 1; (i < dsTable.Columns.Count); i++)
            {
                WhereChar               += " AND " + dsTable.Columns[i].ColumnName + " = " + stgOldItem[i];
            }
            WhereChar                   = WhereChar.Replace("@ AND ", " WHERE ");
            
            string sql = "DELETE FROM " + TableName + " " + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                try
                {
                    /// �폜
                    connect.Command.CommandText = sql;
                    int ExecCount               = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: �G���[����
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Delete");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }
	}
}
