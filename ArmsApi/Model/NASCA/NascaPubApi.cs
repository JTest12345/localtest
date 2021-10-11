//#if DEBUG
//using ArmsApi.local.nichia.naweb_dev;
//#else
//using ArmsApi.local.nichia.naweb;
//#endif
using ArmsApi.local.nichia.naweb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ArmsApi.Model.NASCA
{
    public class NascaPubApi
    {
        //private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// HV試験結果
        /// </summary>
        public const string LOTCHAR_HV_RESULT = "T0000020";

        /// <summary>
        /// HV試験破壊数
        /// </summary>
        public const string LOTCHAR_HV_NGCT = "T0000023";

        /// <summary>
        /// ⊿L測定値
        /// </summary>
        public const string LOTCHAR_DL = "M0000008";

        /// <summary>
        /// HV試験規格（HBMorMM)
        /// </summary>
        public const string LOTCHAR_HV_SPEC = "P0000127";

        /// <summary>
        /// ⊿L試験結果待ち
        /// </summary>
        public const string LOTCHAR_DL_WAIT = "T0000025";

        public const string CHAR_VAL_CD_TEST_WAIT = "4";

        private const string CHAR_VAL_ALL_TESTED = "全数";


        private const string AP_NAME = "VFJUDGMENTSOFT";
        private const string API_SEARCH_PASSWORD = "9H0bBSS5";
        private const string API_EDIT_PASSWORD = "3Nf5yBDy";

        private const string AP_NAME_QCIL = "QCIL";
        private const string API_QCIL_EDIT_PASSWORD = "62j1lY4h";

        private const string AP_NAME_ARMS = "ARMS";
        private const string API_MOVE_ORDER_PASSWORD = "29f5DZdW";
        private const string API_EDIT_CUTORDER_PASSWORD = "CR806Hoq";
        private const string API_EDIT_MNFCT_PASSWORD = "nC1ee61l";

//#if DEBUG
//		private const string USER_CD = "5175";
//		private const string PASSWORD = "5175";
//#else
//		private const string USER_CD = "660";
//		private const string PASSWORD = "Rs6a60";
//#endif

        //ロット特性更新だけなので53でも全部署可能
        private static string SECTION_CD = Config.Settings.SectionCd;

        private AuthHeader searchAuth;
        private AuthHeader editAuth;
        private AuthHeader qcilEditAuth;

        private AuthHeader orderMoveAuth;
        private AuthHeader cutOrderEditAuth;
        private AuthHeader mnfctEditAuth;

        /// <summary>
        /// WebServiceClient
        /// </summary>
        private NppPublicWebServiceSoap client;

        /// <summary>
        /// セッション保存用クッキー置き場
        /// </summary>
        private CookieContainer cookieJar = new CookieContainer();

        #region Singleton
        /// <summary>
        /// Singleton
        /// </summary>
        private static NascaPubApi instance;

        public static NascaPubApi GetInstance()
        {
            if (instance == null)
            {
                instance = new NascaPubApi();
            }

            return instance;
        }
        #endregion

        private NascaPubApi()
        {
            if (this.client == null)
            {
                this.client = new NppPublicWebServiceSoap();
                client.CookieContainer = this.cookieJar;
                string url = Config.Settings.NASCAWebService;
                if (!string.IsNullOrEmpty(url))
                {
                    client.Url = url;
                }
            }

            this.searchAuth = new AuthHeader();
            this.searchAuth.ApplicationName = AP_NAME;
            this.searchAuth.PassWord = API_SEARCH_PASSWORD;

            this.editAuth = new AuthHeader();
            this.editAuth.ApplicationName = AP_NAME;
            this.editAuth.PassWord = API_EDIT_PASSWORD;

            this.qcilEditAuth = new AuthHeader();
            this.qcilEditAuth.ApplicationName = AP_NAME_QCIL;
            this.qcilEditAuth.PassWord = API_QCIL_EDIT_PASSWORD;

            this.orderMoveAuth = new AuthHeader();
            this.orderMoveAuth.ApplicationName = AP_NAME_ARMS;
            this.orderMoveAuth.PassWord = API_MOVE_ORDER_PASSWORD;

            this.cutOrderEditAuth = new AuthHeader();
            this.cutOrderEditAuth.ApplicationName = AP_NAME_ARMS;
            this.cutOrderEditAuth.PassWord = API_EDIT_CUTORDER_PASSWORD;

            this.mnfctEditAuth = new AuthHeader();
            this.mnfctEditAuth.ApplicationName = AP_NAME_ARMS;
            this.mnfctEditAuth.PassWord = API_EDIT_MNFCT_PASSWORD;

            loginAPI();
        }

        ~NascaPubApi()
        {
            logoutAPI();
        }

        #region login logout;

        private void loginAPI()
        {
            //
            // Debug code by juni
            //
            return;
            //
            // Debug code end
            //


            NppLoginInfo login = new NppLoginInfo();

			//login.EmployeeCD = USER_CD;
			//login.Password = PASSWORD;

			login.EmployeeCD = Config.Settings.NascaApiLoginUser;
			login.Password = Config.Settings.NascaApiLoginPassword;

            login.LangKB = Language.ja;
            login.ServerKeyCD = Config.Settings.NascaApiServerCd;

            client.NppPublicLogin(ref login);
        }

        private void logoutAPI()
        {
            //
            // Debug code by juni
            //
            return;
            //
            // Debug code end
            //

            if (this.client == null)
            {
                return;
            }

            AuthHeader auth = new AuthHeader();
            auth.ApplicationName = AP_NAME;
            auth.PassWord = API_SEARCH_PASSWORD;

            client.AuthHeaderValue = auth;

            NppReturnInfo info = client.NppPublicLogout();

            System.Diagnostics.Debug.WriteLine("done");

        }

        #endregion

        /// <summary>
        /// 特性削除
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="lotchar"></param>
        /// <param name="canRetry"></param>
        public void DeleteLotChar(string lotno, string typecd, NppLotCharInfo lotchar, bool canRetry)
        {
            lotchar.IsDelete = true;
            client.AuthHeaderValue = this.editAuth;
            NppReturnInfo info = client.NppEditLotChar(lotchar);

            // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
            if (info.MsgCd == MessageCd.RW0100)
            {
                if (canRetry == true)
                {
                    loginAPI();

                    DeleteLotChar(lotno, typecd, lotchar, false);
                    return;
                }
                else
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }
            }
        }

        /// <summary>
        /// 特性更新
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="lotcharCD"></param>
        /// <param name="charValCD"></param>
        /// <param name="overwrite"></param>
        /// <param name="canRetry"></param>
        public void WriteLotChar(string lotno, string typecd, string lotcharcd, string charval, string listcd, bool overwrite, bool canRetry)
        {
            //書き込み用
            NppLotCharInfo charinfo = new NppLotCharInfo();


            //⊿L結果を登録する際は結果待ち特性を削除
            if (lotcharcd == LOTCHAR_DL)
            {
                NppLotCharInfo[] testWaitLotChar = SearchLotChar(lotno, typecd, LOTCHAR_DL_WAIT, true);
                if (testWaitLotChar != null)
                {
                    foreach (NppLotCharInfo delinfo in testWaitLotChar)
                    {
                        DeleteLotChar(lotno, typecd, delinfo, true);
                    }
                }
            }


            if (overwrite)
            {
                //既存特性を取得
                NppLotCharInfo[] retv = SearchLotChar(lotno, typecd, lotcharcd, true);

                if (retv != null)
                {
                    foreach (NppLotCharInfo exists in retv)
                    {
                        //既に持っている特性は削除
                        DeleteLotChar(lotno, typecd, retv[0], true);
                    }
                }
            }

            charinfo.TypeCd = typecd;
            charinfo.LotNo = lotno;
            charinfo.CD = lotcharcd;
            charinfo.CharValCD = listcd;
            charinfo.LotCharValue = charval;
            charinfo.MaxValue = 0;
            charinfo.MinValue = 0;
            charinfo.ReferenceCode = 10001;
            charinfo.LineNO = 0;

            client.AuthHeaderValue = this.editAuth;
            NppReturnInfo info = client.NppEditLotChar(charinfo);

            // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
            if (info.MsgCd == MessageCd.RW0100)
            {
                if (canRetry == true)
                {
                    loginAPI();
                    WriteLotChar(lotno, typecd, lotcharcd, charval, listcd, overwrite, false);
                    return;
                }
                else
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }
            }
        }

        /// <summary>
        /// 特性検索
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="lotcharCD"></param>
        /// <param name="canRetry"></param>
        /// <returns></returns>
        public NppLotCharInfo[] SearchLotChar(string lotno, string typecd, string lotcharCD, bool canRetry)
        {
            NppLotCharInfo charinfo = new NppLotCharInfo();

            charinfo.TypeCd = typecd;
            charinfo.LotNo = lotno;
            charinfo.CD = lotcharCD;

            NppLotCharInfo[] outprm;
            client.AuthHeaderValue = this.searchAuth;
            NppReturnInfo info = client.NppSearchLotChar(charinfo, out outprm);

            // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
            if (info.MsgCd == MessageCd.RW0100)
            {
                if (canRetry == true)
                {
                    loginAPI();
                    return SearchLotChar(lotno, typecd, lotcharCD, false);
                }
                else
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }
            }

            return outprm;
        }

		///// <summary>
		///// 特性検索
		///// </summary>
		///// <param name="lot"></param>
		///// <param name="lotcharCD"></param>
		///// <param name="canRetry"></param>
		///// <returns></returns>
		//public NppLotCharInfo[] SearchLotChar2(string lotno, string typecd, string[] lotcharCD, bool canRetry)
		//{
		//	NppLotCharInfo charinfo = new NppLotCharInfo();

		//	charinfo.TypeCd = typecd;
		//	charinfo.LotNo = lotno;

		//	NppLotCharInfo[] outprm;
		//	client.AuthHeaderValue = this.searchAuth;
		//	NppReturnInfo info = client.NppSearchLotChar2(charinfo, lotcharCD, out outprm);

		//	// 公開APIセッション切れ時の再ログイン（1回のみリトライ）
		//	if (info.MsgCd == MessageCd.RW0100)
		//	{
		//		if (canRetry == true)
		//		{
		//			loginAPI();
		//			return SearchLotChar2(lotno, typecd, lotcharCD, false);
		//		}
		//		else
		//		{
		//			throw new ApplicationException("公開APIログイン処理失敗");
		//		}
		//	}

		//	return outprm;
		//}

        /// <summary>
        /// 指図移動登録情報取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="movekb"></param>
        /// <param name="profileno"></param>
        /// <param name="orderdt"></param>
        /// <param name="orderct"></param>
        /// <param name="orderMoveString"></param>
        /// <returns></returns>
        public static NppNelOrderMoveParamInfo GetOrderMoveParamInfo(string lotno, int movekb, int profileno, DateTime orderdt, decimal orderct, out string orderMoveString) 
        {
            NppNelOrderMoveParamInfo orderMoveInfo = new NppNelOrderMoveParamInfo();

			//orderMoveInfo.InlineNO = int.Parse(Config.Settings.SpiderInlineNo);
			//orderMoveInfo.InlineID = "ORDERMOVE";
            orderMoveInfo.OrderLotNO = lotno;
            orderMoveInfo.OrderMoveKb = movekb;
            orderMoveInfo.ProfileNO = profileno;
            orderMoveInfo.OrderDt = orderdt.ToString();
            orderMoveInfo.OrderCt = orderct;

            //ログ出力用
            StringBuilder log = new StringBuilder();
			//log.Append(orderMoveInfo.InlineNO + ",");
			//log.Append(orderMoveInfo.InlineID + ",");
            log.Append(orderMoveInfo.OrderLotNO + ",");
            log.Append(orderMoveInfo.OrderMoveKb + ",");
            log.Append(orderMoveInfo.ProfileNO + ",");
            log.Append(orderMoveInfo.OrderDt + ",");
            log.Append(orderMoveInfo.OrderCt);
            orderMoveString = log.ToString(); 

            return orderMoveInfo;
        }
        public static NppNelOrderMoveParamInfo GetOrderMoveParamInfo(string lotno, int movekb, int profileno, DateTime orderdt, out string orderMoveString)
        {
            return GetOrderMoveParamInfo(lotno, movekb, profileno, orderdt, -1, out orderMoveString);
        }

        /// <summary>
        /// 指図移動登録
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="movekb"></param>
        /// <param name="profileno"></param>
        /// <param name="orderdt"></param>
        /// <param name="orderct"></param>
        /// <param name="canRetry"></param>
        public NASCAResponse WriteOrderMove(NppNelOrderMoveParamInfo orderMoveInfo, bool canRetry) 
        {
            client.AuthHeaderValue = this.orderMoveAuth;

            try
            {
                NppReturnInfo res = client.NppNelInsertOrderMove(orderMoveInfo);
                if (res.MsgCd == MessageCd.RW0100)
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }

                if (res.IsErrorFlag)
                {
                    return NASCAResponse.GetNGResponse(string.Format("受信{0}:{1}", res.MsgCd, res.Message));
                }
                else
                {
                    return NASCAResponse.GetOKResponse();
                }
            }
            catch (Exception err)
            {
                if (err is SocketException || err is ApplicationException)
                {
                    //通信障害、公開APIセッション切れ発生時、一回のみリトライ
                    if (canRetry == true)
                    {
                        loginAPI();
                        return WriteOrderMove(orderMoveInfo, false);
                    }
                    else
                    {
                        return NASCAResponse.GetNGResponse(err.Message);
                    }
                }
                else
                {
                    return NASCAResponse.GetNGResponse(err.Message);
                }
            }
        }

        /// <summary>
        /// 指図登録(カット工程専用)情報取得
        /// </summary>
        /// <param name="startdt"></param>
        /// <param name="macno"></param>
        /// <param name="lotno"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        public static NppNelCutBlendOrdParamInfo GetCutBlendOrderParamInfo(DateTime startdt, int macno, string lotno, string cutlotno, out string orderString)
        {
            NppNelCutBlendOrdParamInfo orderInfo = new NppNelCutBlendOrdParamInfo();

			//orderInfo.InlineNO = int.Parse(Config.Settings.SpiderInlineNo);
			//orderInfo.InlineID = "CUTBLEND";
            orderInfo.BinCD = Config.Settings.CutBinCd;
            orderInfo.StartDT = startdt.ToString();
            orderInfo.PlantCD = MachineInfo.GetMachine(macno).NascaPlantCd;
            orderInfo.CutLotNO = cutlotno;
            orderInfo.BlendLotContents = lotno;

            //ログ出力用
            StringBuilder log = new StringBuilder();
			//log.Append(orderInfo.InlineNO + ",");
			//log.Append(orderInfo.InlineID + ",");
            log.Append(orderInfo.BinCD + ",");
            log.Append(orderInfo.StartDT + ",");
            log.Append(orderInfo.PlantCD + ",");
            log.Append(orderInfo.CutLotNO + ",");
            log.Append(orderInfo.BlendLotContents);
            orderString = log.ToString();

            return orderInfo;
        }
        public static NppNelCutBlendOrdParamInfo GetCutBlendOrderParamInfo(CutBlend[] cutblendList, out string orderString)
        {
            string lotno = string.Empty;

            bool isfirst = true;
            foreach (CutBlend cb in cutblendList)
            {
                if (!isfirst) lotno += ";";
                lotno += cb.LotNo;
                isfirst = false;
            }
            return GetCutBlendOrderParamInfo(cutblendList[0].StartDt, cutblendList[0].MacNo, lotno, cutblendList[0].BlendLotNo, out orderString);
        }

        /// <summary>
        /// 指図登録(カット工程専用)
        /// </summary>
        /// <param name="bincd"></param>
        /// <param name="startdt"></param>
        /// <param name="plantcd"></param>
        /// <param name="lotno"></param>
        /// <param name="canRetry"></param>
        public NASCAResponse WriteCutBlendOrder(NppNelCutBlendOrdParamInfo orderInfo, bool canRetry) 
        {         
            client.AuthHeaderValue = this.cutOrderEditAuth;

            try
            {
                NppReturnInfo res = client.NppNelInsertCutBlendOrder(orderInfo);

                // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
                if (res.MsgCd == MessageCd.RW0100)
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }

                if (res.IsErrorFlag)
                {
                    return NASCAResponse.GetNGResponse(string.Format("受信{0}:{1}", res.MsgCd, res.Message));
                }
                else
                {
                    return NASCAResponse.GetOKResponse();
                }
            }
            catch (Exception err)
            {
                if (err is SocketException || err is ApplicationException)
                {
                    //通信障害、公開APIセッション切れ発生時、一回のみリトライ
                    if (canRetry == true)
                    {
                        loginAPI();
                        return WriteCutBlendOrder(orderInfo, false);
                    }
                    else
                    {
                        return NASCAResponse.GetNGResponse(err.Message);
                    }
                }
                else
                {
                    return NASCAResponse.GetNGResponse(err.Message);
                }
            }
        }

        /// <summary>
        /// 実績登録情報取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="magno"></param>
        /// <param name="plantcd"></param>
        /// <param name="startdt"></param>
        /// <param name="enddt"></param>
        /// <param name="partsList"></param>
        /// <param name="defList"></param>
        /// <param name="matList"></param>
        /// <param name="resinList"></param>
        /// <param name="startempcd"></param>
        /// <param name="endempcd"></param>
        /// <param name="comment"></param>
        /// <param name="inspecempcd"></param>
        /// <param name="inspecct"></param>
        /// <returns></returns>
        public static NppNelMnfctParamInfo GetMnfctParamInfo(string lotno, string magno, string plantcd, DateTime startdt, DateTime? enddt,
            string[] partsList, DefItem[] defList, Material[] matList, Resin[] resinList,
            string startempcd, string endempcd, string comment, string inspecempcd, int inspecct, out string mnfctString)
        {
            if (string.IsNullOrEmpty(startempcd)) startempcd = "660";
            if (string.IsNullOrEmpty(endempcd)) endempcd = "660";
            if (string.IsNullOrEmpty(inspecempcd)) inspecempcd = "660";

            if (comment != null)
            {
                comment = comment.Replace(';', '；').Replace('@', '＠')
                    .Replace(',', '，').Replace('\r', ' ').Replace('\n', ' ');
            }
            magno = magno ?? "";

            StringBuilder partsContents = new StringBuilder();
            bool isfirst = true;
            if (partsList != null)
            {
                List<string> existsCondList = new List<string>();
                foreach (string parts in partsList)
                {
                    if (existsCondList.Contains(parts) == false)
                    {
                        if (!isfirst) partsContents.Append(";");
                        partsContents.Append(parts);
                        isfirst = false;
                        existsCondList.Add(parts);
                    }
                }
            }

            StringBuilder failContents = new StringBuilder();
            isfirst = true;
            if (defList != null)
            {
                foreach (DefItem def in defList)
                {
                    if (isfirst == false) failContents.Append(";");
                    failContents.Append(def.CauseCd + "@" + def.ClassCd + "@" + def.DefectCd + "@" + def.DefectCt.ToString());
                    isfirst = false;
                }
            }


            StringBuilder matContents = new StringBuilder();
            bool isInputMatCt = false; //資材数量をNASCAに飛ばすかどうかのフラグ
            if (matList != null)
            {
				// 資材を投入順に並び変える
				matList = matList.OrderBy(m => m.InputDt).ToArray();

                List<Material> existsMatList = new List<Material>();
                isfirst = true;

                foreach (Material mat in matList)
                {
                    //materialのinputCtに入力が有る場合のみ数量を飛ばす。（※0だとNASCAが自動処理する)
                    decimal inputCt = 0;
                    if(mat.InputCt != null)
                    {
                        inputCt = mat.InputCt.Value;
                        isInputMatCt = true;
                    }

                    //同一ロットの重複は排除
                    if (existsMatList.Contains(mat) == false)
                    {
                        if (isfirst == false) matContents.Append(";");
                        matContents.Append(mat.MaterialCd + "@" + mat.LotNo + "@" + inputCt);
                        isfirst = false;
                        existsMatList.Add(mat);
                    }
                }
            }

            StringBuilder resinContents = new StringBuilder();
            if (resinList != null)
            {
                List<Resin> existsResinList = new List<Resin>();
                isfirst = true;
                foreach (Resin res in resinList)
                {
                    if (existsResinList.Contains(res) == false)
                    {
                        if (isfirst == false) resinContents.Append(";");
                        resinContents.Append(res.MixResultId + "@@" + res.ResinGroupCd);
                        isfirst = false;
                        existsResinList.Add(res);
                    }
                }
            }

            StringBuilder inspecContents = new StringBuilder();
            //2=抜き取り　3=全数
            if (inspecct < 0)
            {
                inspecContents.Append("3@");
            }
            else
            {
                inspecContents.Append("2@");
            }

            inspecContents.Append(inspecempcd);
            inspecContents.Append("@");
            if (inspecct < 0)
            {
                inspecContents.Append("0");
            }
            else
            {
                inspecContents.Append(inspecct);
            }

            NppNelMnfctParamInfo mnfctInfo = new NppNelMnfctParamInfo();
			//mnfctInfo.InlineNO = int.Parse(Config.Settings.SpiderInlineNo);
			//mnfctInfo.InlineID = "MNFCT";
            mnfctInfo.PlantCD = plantcd;
            mnfctInfo.LotNO = lotno;
            mnfctInfo.MagazineNO = magno;
            mnfctInfo.WorkStartDT = startdt.ToString();
            mnfctInfo.WorkEndDT = enddt.ToString();
            mnfctInfo.PartsContents = partsContents.ToString();
            mnfctInfo.FailContents = failContents.ToString();
            mnfctInfo.MatContents = matContents.ToString();
            mnfctInfo.ResinContents = resinContents.ToString();
            mnfctInfo.StartUserCD = startempcd;
            mnfctInfo.EndUserCD = endempcd;
            mnfctInfo.Comment = comment;
            mnfctInfo.InspectContents = inspecContents.ToString();

            if(isInputMatCt)
            {
                mnfctInfo.IsCalculateMaterialInputCT = false;
                mnfctInfo.IsTrustMaterialInputCT = true;
            }
            else
            {
                mnfctInfo.IsCalculateMaterialInputCT = true;
                mnfctInfo.IsTrustMaterialInputCT = false;
            }


            //ログ出力用
            StringBuilder log = new StringBuilder();
			//log.Append(mnfctInfo.InlineNO + ",");
			//log.Append(mnfctInfo.InlineID + ",");
            log.Append(mnfctInfo.PlantCD + ",");
            log.Append(mnfctInfo.LotNO + ",");
            log.Append(mnfctInfo.MagazineNO + ",");
            log.Append(mnfctInfo.WorkStartDT + ",");
            log.Append(mnfctInfo.WorkEndDT + ",");
            log.Append(mnfctInfo.PartsContents + ",");
            log.Append(mnfctInfo.FailContents + ",");
            log.Append(mnfctInfo.MatContents + ",");
            log.Append(mnfctInfo.ResinContents + ",");
            log.Append(mnfctInfo.StartUserCD + ",");
            log.Append(mnfctInfo.EndUserCD + ",");
            log.Append(mnfctInfo.Comment + ",");
            log.Append(mnfctInfo.InspectContents);
            mnfctString = log.ToString();

            return mnfctInfo;
        }

        /// <summary>
        /// 実績登録
        /// </summary>
        /// <param name="mnfctInfo"></param>
        /// <param name="canRetry"></param>
        public NASCAResponse WriteMnfctResult(NppNelMnfctParamInfo mnfctInfo, bool canRetry)
        {
            client.AuthHeaderValue = this.mnfctEditAuth;

            try
            {
                NppReturnInfo res = client.NppNelInsertMnfctResult(mnfctInfo);

                // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
                if (res.MsgCd == MessageCd.RW0100)
                {
                    throw new ApplicationException("公開APIログイン処理失敗");
                }

                if (res.IsErrorFlag)
                {
                    return NASCAResponse.GetNGResponse(string.Format("受信{0}:{1}", res.MsgCd, res.Message));
                }
                else
                {
                    return NASCAResponse.GetOKResponse();
                }
            }
            catch (Exception err)
            {
                if (err is SocketException || err is ApplicationException)
                {
                    //通信障害、公開APIセッション切れ発生時、一回のみリトライ
                    if (canRetry == true)
                    {
                        loginAPI();
                        return WriteMnfctResult(mnfctInfo, false);
                    }
                    else
                    {
                        return NASCAResponse.GetNGResponse(err.Message);
                    }
                }
                else
                {
                    return NASCAResponse.GetNGResponse(err.Message);
                }
            }
        }

        /// <summary>
        /// 遠心沈降機　アウトライン共用のための処置
        /// 製造実績登録（アウトラインロット）
        /// 将来的には高効率は専用設備で運用できるようにすること。
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="plantcd"></param>
        /// <param name="strDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="generalVal"></param>
        /// <param name="isComplete"></param>
        /// <param name="canRetry"></param>
        /// <returns></returns>
        public bool NppEditMeasureRstInfo(string lotno, string plantcd, string strDateTime, string endDateTime, string generalVal, bool isComplete, bool canRetry, out string errMsg)
        {
            try
            {
                client.AuthHeaderValue = this.qcilEditAuth;
                // 編集項目の設定
                NppEditManufactOrderInfo orderInfo = new NppEditManufactOrderInfo();

                //アウトラインは0固定
                orderInfo.InlineNo = 0;
                orderInfo.NascaLotNo = lotno.Trim();
                orderInfo.MagazineNo = "";
                orderInfo.WorkCd = "";
                orderInfo.PlantCd = plantcd;

				//orderInfo.StrUsrCd = USER_CD;
				//orderInfo.EndUsrCd = USER_CD;
				orderInfo.StrUsrCd = Config.Settings.NascaApiLoginUser;
				orderInfo.EndUsrCd = Config.Settings.NascaApiLoginUser;

                orderInfo.StrDate = strDateTime;
                orderInfo.EndDate = endDateTime;
                orderInfo.GeneralVal = generalVal;
                orderInfo.IsComplete = isComplete;

                // API呼び出し
                NppReturnInfo retInfo = client.NppEditManufactOrderResult(orderInfo);

                // 公開APIセッション切れ時の再ログイン（1回のみリトライ）
                if (retInfo.MsgCd == MessageCd.RW0100)
                {
                    if (canRetry == true)
                    {
                        loginAPI();
                        return NppEditMeasureRstInfo(lotno, plantcd, strDateTime, endDateTime, generalVal, isComplete, false, out errMsg);
                    }
                    else
                    {
                        throw new ApplicationException("公開APIログイン処理失敗");
                    }
                }

                if (retInfo.IsErrorFlag)
                {
                    errMsg = "NASCA NG:" + lotno + ":" + retInfo.Message;
                    return false;
                }

                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = "NASCA API 呼び出しエラー" + ex.ToString();
                return false;
            }
        }

		public NppCarrierLotInfo[] GetCarrierData(string lotNo, bool canRetry)
		{
			try
			{
				NppCarrierLotInfo[] carriers;

				// API呼び出し
				client.AuthHeaderValue = this.searchAuth;
				// API呼び出し
				NppReturnInfo retInfo = client.NppSearchCarrierLot(lotNo, null, string.Empty, out carriers);

				// 公開APIセッション切れ時の再ログイン（1回のみリトライ）
				if (retInfo.MsgCd == MessageCd.RW0100)
				{
					if (canRetry == true)
					{
						loginAPI();
						//Log.Write("NASCA API 再ログイン");
						return GetCarrierData(lotNo, false);
					}
				}

				if (retInfo.IsErrorFlag)
				{
					throw new ApplicationException("キャリアロット情報取得に失敗しました:" + retInfo.Message + " /lotno:" + lotNo);
				}

				return carriers;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("NASCA API GetCarrierData 呼び出しエラー /lotno:" + lotNo + " /例外:" + ex.ToString());
			}
		}

		public void ReleaseCarrierNo(string lotno, string oldCarrierNo, bool canRetry)
		{
			try
			{
				// API呼び出し
				client.AuthHeaderValue = this.editAuth;
				// API呼び出し
				NppReturnInfo retInfo = client.NppEditCarrierChange(lotno, oldCarrierNo, null, string.Empty, string.Empty);

				// 公開APIセッション切れ時の再ログイン（1回のみリトライ）
				if (retInfo.MsgCd == MessageCd.RW0100)
				{
					if (canRetry == true)
					{
						loginAPI();
						ReleaseCarrierNo(lotno, oldCarrierNo, false);
						return;
					}
				}

				if (retInfo.IsErrorFlag)
				{
					throw new ApplicationException("キャリア解放登録に失敗しました:" + retInfo.Message);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("NASCA API ReleaseCarrierNo 呼び出しエラー /lotno:" + lotno + " /carrierno:" + oldCarrierNo + " /例外内容:" + ex.ToString());
			}
		}
    }
}
