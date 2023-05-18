//////////////////////////////////////////////////////////////////////////
//
// namespace は設定してください
// app.configに、app.config.txtの内容を追加してください。
//
//////////////////////////////////////////////////////////////////////////
namespace EICS
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.ComponentModel;
    using System.Web.Services;          //参照の追加
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;

    using NascaAPI;                     //Web参照の追加

	//public class NppPublicWebService:PublicWebService.NppPublicWebServiceSoap
    public class NppPublicWebService : NppPublicWebServiceSoap
	{
		/// <summary>クッキー</summary>
		private static CookieContainer cookieJar=new System.Net.CookieContainer();

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="url"></param>
		/// <param name="timeout"></param>
		/// <param name="cookie"></param>
		private NppPublicWebService(
			string url,
			int timeout,
			CookieContainer cookie,
			string appNM,
			string appPassword):base()
		{
            
			this.Url = url;
			this.Timeout=timeout;
			this.CookieContainer=cookie;

			//PublicWebService.AuthHeader aHeader = new PublicWebService.AuthHeader();
            AuthHeader aHeader = new AuthHeader();
			aHeader.ApplicationName	= appNM;
			aHeader.PassWord		= appPassword;

			this.AuthHeaderValue = aHeader;
		}
		#endregion

		#region インスタンス作成
		/// <summary>
		/// インスタンス作成
		/// </summary>
		/// <returns></returns>
		public static NppPublicWebService CreateInstance()
		{
			string urlSetting=null;
			int timeout=0;
			CookieContainer cookie=null;

			try
			{
				//アプリケーションサーバ（WebService）のURLを設定します。

				urlSetting = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.NppPublicWebService"];
                //User/Password=社員番号/Password
				if ((urlSetting != null)) 
				{
					urlSetting = string.Concat(urlSetting, "");
				}
				else 
				{
					//TODO デフォルト値 開発時のみ適用
					//運用時にはこのコードは消去します。

					urlSetting = "http://HQAPDEV1:8090/RSL3/NPPPUBLIC/NppPublicWebService.asmx";
                    //User/Password=社員番号/社員番号
				}
		
				string strTemp = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.TimeOut"];

				timeout = Convert.ToInt32(strTemp);

				//Cookieコンテナ
				cookie = cookieJar;
				
			}
			catch
			{
				throw;
			}
			return(new NppPublicWebService(urlSetting,timeout,cookie, "", ""));
		}
		#endregion

		#region インスタンス作成
		/// <summary>
		/// インスタンス作成
		/// </summary>
		/// <returns></returns>
		public static NppPublicWebService CreateInstance( string appNM, string appPassword )
		{
			string urlSetting=null;
			int timeout=0;
			CookieContainer cookie=null;

			try
			{
				//アプリケーションサーバ（WebService）のURLを設定します。
				urlSetting = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.NppPublicWebService"];
				if ((urlSetting != null)) 
				{
					urlSetting = string.Concat(urlSetting, "");
				}
				else
				{
					//TODO デフォルト値 開発時のみ適用
					//運用時にはこのコードは消去します。

					urlSetting = "http://HQAPDEV1:8090/RSL3/NPPPUBLIC/NppPublicWebService.asmx";
				}
				
				string strTemp = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.TimeOut"];

				timeout = Convert.ToInt32(strTemp);

				//Cookieコンテナ
				cookie = cookieJar;
		    }
			catch
			{
				throw;
			}
			return(new NppPublicWebService(urlSetting,timeout,cookie,appNM, appPassword));
		}
		#endregion

		#region アプリケーションパスワードの設定
		/// <summary>
		/// アプリケーションパスワードの設定
		/// </summary>
		/// <param name="appNM"></param>
		/// <param name="appPassword"></param>
		/// <returns></returns>
		//public static PublicWebService.AuthHeader SetAppPassword( string appNM, string appPassword)
        public static AuthHeader SetAppPassword(string appNM, string appPassword)
		{
			//PublicWebService.AuthHeader aHeader = new PublicWebService.AuthHeader();
            AuthHeader aHeader = new AuthHeader();
			aHeader.ApplicationName	= appNM;
			aHeader.PassWord		= appPassword;
		
			return aHeader;
		}
		#endregion
	}
}