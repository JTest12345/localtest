/*************************************************************************************
 * システム名     : 無塵ウェアシステム
 * 
 * 処理名		  : Registry レジストリ管理
 * 
 * 概略           : システム構成情報格納レジストリの読み書きを行います。
 * 
 * 作成           : 2006/10/16 SLA.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using Microsoft.Win32;

namespace FVDC
{
	/// <summary>
	/// 
	/// fvdcシステムで扱うレジストリは
	/// HKEY_CURRENT_USER\software\nichia\fvdc\configurations\配下に保存されます。
	/// </summary>
	public abstract class fvdcRegistry
	{

		private const string REGISTORY_SUB_KEY = "software\\nichia\\fvdc\\configurations";
		/// <summary>
		/// Newでインスタンス化はさせない。
		/// </summary>
		private fvdcRegistry()
		{
		}
		
		private static RegistryKey createResistrySubKey()
		{
			try
			{
				/// ＰＣのプロファイルユーザー名を取得
				string		ProfileUser		= Environment.GetEnvironmentVariable("USERNAME").Trim();
				/// レジストリのキーを作成
				RegistryKey regKey			= Registry.CurrentUser;
				regKey                      = regKey.CreateSubKey(REGISTORY_SUB_KEY + "\\" + ProfileUser);
				return(regKey);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// レジストリから引数で指定したキーの値を取得します。
		/// </summary>
		/// <param name="keyName">レジストリキー</param>
		/// <returns></returns>
		private static object getfvdcRegistry(string keyName)
		{
			try
			{
				//サブキーを生成します。
				RegistryKey regKey          = createResistrySubKey();
				//レジストリから値を読み出します。
				return(regKey.GetValue(keyName));

			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// レジストリから引数で指定したキーの値で書き込みます。
		/// </summary>
		/// <param name="RegkeyName"></param>
		/// <param name="RegValue"></param>
		private static void setfvdcRegistry(string RegkeyName,object RegValue)
		{
			
			try
			{
				//サブキーを生成します。
				RegistryKey regKey          = createResistrySubKey();
				//レジストリに書き込みます。
				regKey.SetValue(RegkeyName,RegValue);
			}
			catch
			{
				throw;
			}
			return;

		}
        /// <summary>
        /// レジストリから引数で指定したキーの値を削除します。
        /// </summary>
        /// <param name="RegkeyName"></param>
        /// <param name="RegValue"></param>
        private static void delfvdcRegistry(string RegkeyName)
        {

            try
            {
                //サブキーを生成します。
                RegistryKey regKey          = createResistrySubKey();
                //レジストリに書き込みます。
                regKey.DeleteValue(RegkeyName);
            }
            catch
            {
                throw;
            }
            return;

        }

		public enum fvdcRefistryKey
		{
			defaultServer,
            defaultType,
            defaultLine,
            defaultLot,
            defaultTestLot,
            defaultAddLot

		}
		/// <summary>
		/// レジストリの書き込みを行います。
		/// </summary>
		/// <param name="fvdcRegKey"></param>
		/// <param name="RegValue"></param>
		public static void SetRegistory(fvdcRefistryKey fvdcRegKey,object RegValue)
		{
			try
			{
				setfvdcRegistry(fvdcRegKey.ToString(),RegValue);
			}
			catch
			{
				throw;
			}
			return;
		}
        /// <summary>
        /// レジストリの削除を行います。
        /// </summary>
        /// <param name="csvtRegKey"></param>
        /// <param name="RegValue"></param>
        public static void DelRegistory(fvdcRefistryKey fvdcRegKey)
        {
            try
            {
                delfvdcRegistry(fvdcRegKey.ToString());
            }
            catch
            {
                throw;
            }
            return;
        }
		/// <summary>
		/// レジストリの読み出しを行います。
		/// </summary>
		/// <param name="fvdcRegKey"></param>
		/// <returns></returns>
		public static object GetRegistory(fvdcRefistryKey fvdcRegKey)
		{
			try
			{
				object regValue                 = getfvdcRegistry(fvdcRegKey.ToString());
				return(regValue);
			}
			catch
			{
				throw;
			}
		}

	}
}
