
namespace EICS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using log4net;
    using NascaAPI;

    public class ConnectAPI : IDisposable
    {
        //private string appName = "TESTAPP";
        //private string appPassword_Edit = "TESTAPP";

        private NppPublicWebService nppWS;
        private LoginInfo loginInfo = null;

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="login"></param>
        public ConnectAPI(LoginInfo login) 
        {
            this.loginInfo = login;
        }

        #endregion

        #region IDisposable メンバ

        /// <summary>
        /// Disposable
        /// </summary>
        public void Dispose()
        {
            Logoff();

            if (this.nppWS != null)
            {
                this.nppWS.Dispose();
                this.nppWS = null;
            }
        }

        #endregion

        /// <summary>ログイン</summary>
        public bool Login() 
        {
            string sMessage = "";
            NppLoginInfo nppLogin = new NppLoginInfo();

            nppLogin.EmployeeCD = this.loginInfo.EmployeeID;
            nppLogin.Password = this.loginInfo.Password;

            //nppLogin.ServerKeyCD = SLCommonLib.Commons.Configuration.GetAppConfigString(this.loginInfo.SectionNM + "_SERVER");
            nppLogin.ServerKeyCD = System.Configuration.ConfigurationSettings.AppSettings[this.loginInfo.SectionNM + "_SERVER"];
            nppLogin.ServerKeyCD = "SLC-SERVER";
            nppLogin.LangKB = Language.ja;

            //WebServiceクラス生成
            //nppWS = NppPublicWebService.CreateInstance(this.loginInfo);
            //nppWS = NppPublicWebService.CreateInstance(nppLogin.EmployeeCD, nppLogin.Password);
            nppWS = NppPublicWebService.CreateInstance();
            
            //ログイン
            try
            {
                NppReturnInfo nppReturnInfo = nppWS.NppPublicLogin(ref nppLogin);
                if (nppReturnInfo.IsErrorFlag)
                {
                    sMessage = "APIのログインに失敗しました。";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,sMessage);

                    return false;
                }
            }
            catch (Exception ex)
            {
                sMessage = ex.ToString() + "APIのログインに失敗しました。";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage); 
                return false;
            }
            return true;
        }

        /// <summary>ログオフ</summary>
        public void Logoff()
        {
             nppWS.NppPublicLogout();
        }

        //NASCA API①
        public NppInlineMagazineLotInfo GetInlineMagazineLot(int inlineCD,string magazineNO)
        {
            // インラインロット情報
            NppInlineMagazineLotInfo rtnInlineMagLotInfo = null;

            // 検索条件セット
            //int inlineCD = 1001;	    //☆検索したいインラインCDを設定
            //string magazineNO = "0033"; //☆検索したいマガジンNOを設定

            //////////////////////////////////////////////
            // アプリケーションパスワードをここで設定！ //
            //////////////////////////////////////////////
            string appNM = Constant.APP1_CD;
            string appPassword = Constant.APP1_PWD;
            //////////////////////////////////////////////

            /**********************************************
                ここから変更不要
            ***********************************************/
            NppPublicWebService nppWS = NppPublicWebService.CreateInstance(appNM, appPassword);
            NppReturnInfo retInfo = nppWS.NppSearchInlineMagLot(inlineCD, magazineNO, out rtnInlineMagLotInfo);

            /**********************************************
                ここまで
            ***********************************************/

            // エラーフラグの確認。
            if (retInfo.IsErrorFlag)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, retInfo.Message);

                //return ;
            }

            // nullの場合は存在していないロットで検索していた
            /*
            if (rtnInlineMagLotInfo == null)
            {
                MessageBox.Show("流動中でないかロット情報がない");
            }
            else
            {
                // マガジンロット情報取得
                // rtnInlineMagLotInfo.TypeCD // 型番
                // rtnInlineMagLotInfo.NascaLotNO // ロットNO
            }
            */
            return rtnInlineMagLotInfo;
        }

        //NASCA API②
        //public NppResultsFacilityInfo GetInlineMagazineLot(string plantCD, string start2EndDt)
        public NppResultsFacilityInfo[] GetInlineMagazineLot(string plantCD, string start2EndDt)
        {
            // インラインロット情報
            NppResultsFacilityInfo[] rtnResultsFacilityInfo = null;

            // 検索条件セット
            //string plantCD = "検索したい設備CD(S*****)を設定";
            //string start2EndDt = "検索したい開始時刻≦X≦終了時刻となるXを設定 YYYY/MM/DD hh:mm:ss";

            //////////////////////////////////////////////
            // アプリケーションパスワードをここで設定！ //
            //////////////////////////////////////////////
            string appNM = Constant.APP2_CD;
            string appPassword = Constant.APP2_PWD;
            //////////////////////////////////////////////

            /**********************************************
                ここから変更不要
            ***********************************************/
            NppPublicWebService nppWS = NppPublicWebService.CreateInstance(appNM, appPassword);
            NppReturnInfo retInfo = nppWS.NppSearchResultsFacility(plantCD, start2EndDt, out rtnResultsFacilityInfo);

            /**********************************************
                ここまで
            ***********************************************/

            // エラーフラグの確認。
            
            if (retInfo.IsErrorFlag)
            {
                //MessageBox.Show(retInfo.Message);
                //return;
            }
            /*
            // 検索条件に合う実績がなかった
            if (rtnResultsFacilityInfo == null)
            {
                MessageBox.Show("投入されていないか時刻がずれているか・・・");
            }
            else
            {
                // マガジンロット情報取得
                // rtnResultsFacilityInfo.TypeCd // 型番
                // rtnResultsFacilityInfo.LotNo // ロットNO
            }
            */

            return rtnResultsFacilityInfo;
        }

        //NASCA API③
        public NppLotCharInfo[] GetLotCharInfo(string sType,string sLotNo,string sCD)
        {
            // 返り値
            NppLotCharInfo[] retValue = null;

            try
            {
                // 検索条件
                NppLotCharInfo srcPara = new NppLotCharInfo();

                // 検索条件の設定
                srcPara.TypeCd = sType; //[型番]
                srcPara.LotNo = sLotNo; //[NASCAロット番号]
                srcPara.CD = sCD;       //[ロット特性CD]

                //////////////////////////////////////////////
                // アプリケーションパスワードをここで設定！ //
                //////////////////////////////////////////////
                string appNM = Constant.APP3_CD;
                string appPassword = Constant.APP3_PWD;
                //////////////////////////////////////////////

                /**********************************************
                    ここから変更不要
                ***********************************************/
                // WebServiceクラス生成
                //NppPublicWebService nppWS = NppPublicWebService.CreateInstance(Application.ProductName, appPassword);
                NppPublicWebService nppWS = NppPublicWebService.CreateInstance(appNM, appPassword);

                // API呼び出し
                NppReturnInfo retInfo = nppWS.NppSearchLotChar(srcPara, out retValue);
                /**********************************************
                    ここまで
                ***********************************************/

                if (retInfo.IsErrorFlag)
                {
                    throw new ApplicationException(retInfo.Message);
                }

                //MessageBox.Show("検索完了");

                // 検索結果
                // [retValue]　の値に結果が返ってくる。　
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return retValue;
        }

    }
}

