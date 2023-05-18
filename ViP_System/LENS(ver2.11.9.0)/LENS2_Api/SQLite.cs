using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
    public class SQLite
    {
        static SQLite()
        {
            LensConStr = Config.Settings.LensConnectionString;
            ArmsConStr = Config.Settings.ArmsConnectionString;
        }

        public static string LensConStr = "";
        public static string ArmsConStr = "";

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
