using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS
{
    public class SQLite
    {
        public static string ConStr_ARMS = @"data source=C:\NEL\DB\ARMS.db3";

        static SQLite()
        {
            ConStr_ARMS = Constant.SQLite_ARMS;//固定
        }

        #region 日付とintシリアル値の変換

        /// <summary>
        /// Sqliteの日付データ変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string SerializeDate(DateTime? dt)
        {
            if (dt.HasValue)
            {
                string str = dt.Value.ToString("yyyyMMddHHmmss");
                return long.Parse(str).ToString();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Sqlite日付データからDateTimeへ変換
        /// </summary>
        /// <param name="serial"></param>
        /// <returns>非long値はnull</returns>
        public static DateTime? ParseDate(object serial)
        {
            long? longSerial = serial as long?;
            if (longSerial.HasValue == true)
            {
                return ParseDate(longSerial.Value);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Sqlite日付データからDateTimeへ変換
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public static DateTime ParseDate(long serial)
        {
            string str = serial.ToString();

            if (str.Length == 14)
            {
                return DateTime.ParseExact(str, "yyyyMMddHHmmss", null);
            }

            else throw new ApplicationException("不正なDateTime値です:" + str);

        }
        #endregion


        #region boolとintシリアル値の変換

        /// <summary>
        /// Sqliteの日付データ変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int SerializeBool(bool flag)
        {
            if (flag == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public bool ParseBool(object o)
        {
            if (o == null)
            {
                return false;
            }

            int i;
            if (!int.TryParse(o.ToString(), out i))
            {
                return false;
            }

            return ParseBool(i);
        }

        public bool ParseBool(int i)
        {
            if (i == 0)
            {
                return false;
            }
            else if (i == 1)
            {
                return true;
            }
            else
            {
                throw new InvalidCastException("不正なBOOLシリアル値です:" + i.ToString());
            }
        }
        #endregion

        public int ParseInt(object o)
        {
            if (o == DBNull.Value)
            {
                return 0;
            }
            else
            {
                int i;
                if (int.TryParse(o.ToString(), out i) == true)
                {
                    return i;
                }

                return 0;
            }
        }

        public string ParseString(object o)
        {
            if (o == DBNull.Value)
            {
                return null;
            }
            else
            {
                return o.ToString();
            }
        }

    }
}
