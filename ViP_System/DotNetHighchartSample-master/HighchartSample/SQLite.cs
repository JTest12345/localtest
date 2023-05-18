using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HighchartSample
{
    public class SQLite
    {
        static SQLite()
        {
            //ConStr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";//Config.Settings.LocalConnString;
        }

        public static string ConStr = "";

        public static object GetParameterValue(object targetValue)
        {
            if (targetValue == null || targetValue.ToString() == "")
            {
                return DBNull.Value;
            }
            else
            {
                return targetValue;
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public static DateTime? ParseDate(object serial)
        {
            if (serial is DateTime)
            {
                return (DateTime)serial;
            }
            else
            {
                return null;
            }
        }


        #region boolとintシリアル値の変換

        /// <summary>
        /// Sqliteの日付データ変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int SerializeBool(bool flag)
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


        public static bool ParseBool(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o is bool)
            {
                return (bool)o;
            }

            int i;
            if (!int.TryParse(o.ToString(), out i))
            {
                return false;
            }

            return ParseBool(i);
        }

        public static bool ParseBool(int i)
        {
            if (i == 0)
            {
                return false;
            }
            else if (i >= 1)
            {
                return true;
            }
            else
            {
                throw new InvalidCastException("不正なBOOLシリアル値です:" + i.ToString());
            }
        }





        #endregion


        public static int? ParseNullableInt(object o)
        {
            if (o == null || o == DBNull.Value)
            {
                return null;
            }
            else
            {
                int i = 0;
                try
                {
                    i = Convert.ToInt32(o);
                }
                catch { }

                return i;
            }
        }

        public static int ParseInt(object o)
        {
            if (o == DBNull.Value)
            {
                return 0;
            }
            else
            {
                int i = 0;
                try
                {
                    i = Convert.ToInt32(o);
                }
                catch { }

                return i;
            }
        }

        public static string ParseString(object o)
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

        public static decimal ParseDecimal(object o)
        {
            if (o == DBNull.Value)
            {
                return 0;
            }
            else
            {
                decimal i = 0;
                try
                {
                    i = Convert.ToDecimal(o);
                }
                catch { }

                return i;
            }
        }
        public static float ParseSingle(object o)
        {
            if (o == DBNull.Value)
            {
                return 0;
            }
            else
            {
                float i = 0;
                try
                {
                    i = Convert.ToSingle(o);
                }
                catch { }

                return i;
            }
        }
    }
}
