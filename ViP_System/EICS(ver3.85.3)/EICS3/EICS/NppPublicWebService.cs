//////////////////////////////////////////////////////////////////////////
//
// namespace �͐ݒ肵�Ă�������
// app.config�ɁAapp.config.txt�̓��e��ǉ����Ă��������B
//
//////////////////////////////////////////////////////////////////////////
namespace EICS
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.ComponentModel;
    using System.Web.Services;          //�Q�Ƃ̒ǉ�
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;

    using NascaAPI;                     //Web�Q�Ƃ̒ǉ�

	//public class NppPublicWebService:PublicWebService.NppPublicWebServiceSoap
    public class NppPublicWebService : NppPublicWebServiceSoap
	{
		/// <summary>�N�b�L�[</summary>
		private static CookieContainer cookieJar=new System.Net.CookieContainer();

		#region �R���X�g���N�^
		/// <summary>
		/// �R���X�g���N�^
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

		#region �C���X�^���X�쐬
		/// <summary>
		/// �C���X�^���X�쐬
		/// </summary>
		/// <returns></returns>
		public static NppPublicWebService CreateInstance()
		{
			string urlSetting=null;
			int timeout=0;
			CookieContainer cookie=null;

			try
			{
				//�A�v���P�[�V�����T�[�o�iWebService�j��URL��ݒ肵�܂��B

				urlSetting = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.NppPublicWebService"];
                //User/Password=�Ј��ԍ�/Password
				if ((urlSetting != null)) 
				{
					urlSetting = string.Concat(urlSetting, "");
				}
				else 
				{
					//TODO �f�t�H���g�l �J�����̂ݓK�p
					//�^�p���ɂ͂��̃R�[�h�͏������܂��B

					urlSetting = "http://HQAPDEV1:8090/RSL3/NPPPUBLIC/NppPublicWebService.asmx";
                    //User/Password=�Ј��ԍ�/�Ј��ԍ�
				}
		
				string strTemp = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.TimeOut"];

				timeout = Convert.ToInt32(strTemp);

				//Cookie�R���e�i
				cookie = cookieJar;
				
			}
			catch
			{
				throw;
			}
			return(new NppPublicWebService(urlSetting,timeout,cookie, "", ""));
		}
		#endregion

		#region �C���X�^���X�쐬
		/// <summary>
		/// �C���X�^���X�쐬
		/// </summary>
		/// <returns></returns>
		public static NppPublicWebService CreateInstance( string appNM, string appPassword )
		{
			string urlSetting=null;
			int timeout=0;
			CookieContainer cookie=null;

			try
			{
				//�A�v���P�[�V�����T�[�o�iWebService�j��URL��ݒ肵�܂��B
				urlSetting = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.NppPublicWebService"];
				if ((urlSetting != null)) 
				{
					urlSetting = string.Concat(urlSetting, "");
				}
				else
				{
					//TODO �f�t�H���g�l �J�����̂ݓK�p
					//�^�p���ɂ͂��̃R�[�h�͏������܂��B

					urlSetting = "http://HQAPDEV1:8090/RSL3/NPPPUBLIC/NppPublicWebService.asmx";
				}
				
				string strTemp = System.Configuration.ConfigurationSettings.AppSettings["PublicWebService.TimeOut"];

				timeout = Convert.ToInt32(strTemp);

				//Cookie�R���e�i
				cookie = cookieJar;
		    }
			catch
			{
				throw;
			}
			return(new NppPublicWebService(urlSetting,timeout,cookie,appNM, appPassword));
		}
		#endregion

		#region �A�v���P�[�V�����p�X���[�h�̐ݒ�
		/// <summary>
		/// �A�v���P�[�V�����p�X���[�h�̐ݒ�
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