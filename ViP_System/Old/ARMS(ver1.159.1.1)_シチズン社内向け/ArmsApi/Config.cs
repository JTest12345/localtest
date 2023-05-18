using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using ArmsApi.Model.SQDB;

namespace ArmsApi
{
    public class Config
    {
        /// <summary>
        /// StaticInstance
        /// </summary>
        public static Config Settings;

        private const string SETTING_FILE_NM = "ArmsConfig.xml";
        public const string SETTING_FILE_FULLPATH = @"C:\ARMS\Config\ArmsConfig.xml";

        /// <summary>
        /// 作業者バーコード先頭文字
        /// </summary>
        public const string EMP_BARCODE_HEADER = "01 ";

        /// <summary>
        /// 装置バーコード先頭文字
        /// </summary>
        public const string MAC_BARCODE_HEADER = "07 ";

		/// <summary>
		/// 装置完了登録用バーコード先頭文字
		/// </summary>
		public const string MAC_COMP_BARCODE_HEADER = "36 ";

        /// <summary>
        /// カセットチェンジャー登録用バーコード先頭文字
        /// </summary>
        public const string MAC_WAFER_BARCODE_HEADER = "40 ";

        /// <summary>
        /// ライフ試験数
        /// </summary>
        public const string LIFE_TEST_CT_LOTCHAR_CD = "T0000001";

        /// <summary>
        /// 吸湿保管点灯試験数
        /// </summary>
        public const string KHL_TEST_CT_LOTCHAR_CD = "T0000036";

        /// <summary>
        /// 吸湿保管点灯試験結果
        /// </summary>
        public const string KHL_TEST_RESULT_LOTCHAR_CD = "T0000037";

        /// <summary>
        /// 吸湿保管点灯試験数
        /// </summary>
        public const int KHL_TEST_CT = 50;

        /// <summary>
        /// 吸湿保管点灯試験数
        /// </summary>
        public const int KHL_COB_TEST_CT = 4;

        /// <summary>
        /// 静電耐圧試験結果ロット特性
        /// </summary>
        public const string HV_TEST_RESULT_LOTCHAR_CD = "T0000020";

        /// <summary>
        /// 静電耐圧試験数ロット特性
        /// </summary>
        public const string HV_TEST_CT_LOTCHAR_CD = "T0000006";

        /// <summary>
        /// 静電耐圧試験数（固定値）
        /// </summary>
        public const int HV_TEST_CT = 20;

        /// <summary>
        /// 静電耐圧試験「結果待ち」特性値
        /// </summary>
        public const string HV_TEST_RESULT_LIST_VAL = "4";

        /// <summary>
        /// L値試験数ロット特性
        /// </summary>
        public const string DELTA_L_TEST_CT_LOTCHAR_CD = "T0000018";

        /// <summary>
        /// L値試験結果待ちロット特性
        /// </summary>
        public const string DELTA_L_TEST_RESULT_WAIT_LOTCHAR_CD = "T0000025";

        /// <summary>
        /// 先行色調確認
        /// </summary>
        public const string COLOR_TEST_LOTCHAR_CD = "P0000031";

        /// <summary>
        /// リフローパス試験数
        /// </summary>
        public const string REFLOW_TEST_CT_LOTCHAR_CD = "T0000042";

        /// <summary>
        /// リフローパス試験(WB)数
        /// </summary>
        public const string REFLOW_TEST_CT_WB_LOTCHAR_CD = "T0000095";

        /// <summary>
        /// 弾性率試験数
        /// </summary>
        public const string ELASTICITY_TEST_CT_LOTCHAR_CD = "T0000028";

        /// <summary>
        /// 初品識別
        /// </summary>
        public const string FIRST_ARTICLE_LOTCHAR_CD = "P0000192";

        /// <summary>
        /// ダイシェア試験抜取グループ
        /// </summary>
        public const string DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD = "P0000212";

        /// <summary>
        /// 蛍光体ブロック品目
        /// </summary>
        public const string PHOSPHORSHEET_LOTCHARCD = "M0000118";

        /// <summary>
        /// 高生産性ライン名称
        /// </summary>
        public const string LINENAME_HIGHLINE = "高生産性";

        //public string SpiderInlineNo { get; set; }

        /// <summary>
        /// 常温待機CV開始作業CD(追加)
        /// </summary>
        public string MoldConveyorWaitStartWorkCd { get; set; }

        /// <summary>
        /// 樹脂グループ特性CD
        /// </summary>
        public const string RESINGROUP_LOTCHARCD = "P0000007";

        /// <summary>
        /// 常温待機CV開始作業区分(追加)
        /// </summary>
        public Model.TimeLimit.JudgeKb MoldConveyorWaitStartWorkKb { get; set; }

        /// <summary>
        /// 常温待機CV完了作業CD(追加)
        /// </summary>
        public string MoldConveyorWaitEndWorkCd { get; set; }

        /// <summary>
        /// 常温待機CV完了作業区分(追加)
        /// </summary>
        public Model.TimeLimit.JudgeKb MoldConveyorWaitEndWorkKb { get; set; }
        
        /// <summary>
        /// ループ確認用サンプル数特性CD
        /// </summary>
        public const string LOOP_CHECK_SAMPLE_CT_LOTCHAR_CD = "T0000115";

        /// <summary>
        /// 自ラインの手が届く装置が供給OFFの時、他ラインの供給可能装置へ搬送するかのフラグ
        /// </summary>
        public bool RelayOtherLineIfReachMachineIsBusy { get; set; }

        /// <summary>
        /// DB前プラズマ監視の対象作業CD　空白の場合はDB0027が代用される
        /// </summary>
        public string DBPreOvnPLWorkCD { get; set; }

        static Config()
		{
			try
			{
                if (Settings == null)
                {
                    string raw;

                    string settingfolderpath = GetSettingFolderPath();

                    raw = File.ReadAllText(Path.Combine(settingfolderpath, SETTING_FILE_NM), Encoding.UTF8);

                    // GetSettingFolderPath関数に切り出し

                    //string localConfigFilePath = System.Configuration.ConfigurationManager.AppSettings["LocalConfigFilePath"];
                    //if (string.IsNullOrWhiteSpace(localConfigFilePath) == false && File.Exists(Path.Combine(localConfigFilePath, SETTING_FILE_NM)))
                    //{
                    //    // ARMSWebの「Web.config」ファイル内の「LocalConfigFilePath」の設定を最優先でチェック 
                    //    //    ※同一PC内で複数のARMSWebを立ち上げる為に必要 (使用場所：N工場SVライン - メイン中間PC)
                    //    raw = File.ReadAllText(Path.Combine(localConfigFilePath, SETTING_FILE_NM), Encoding.UTF8);
                    //}
                    //else if (File.Exists(SETTING_FILE_FULLPATH) == true)
                    //{
                    //    raw = File.ReadAllText(SETTING_FILE_FULLPATH, Encoding.UTF8);
                    //}
                    //else if (File.Exists(Path.Combine(Environment.CurrentDirectory, SETTING_FILE_NM)))
                    //{
                    //    raw = File.ReadAllText(SETTING_FILE_NM, Encoding.UTF8);
                    //}
                    //else
                    //{
                    //    raw = File.ReadAllText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), SETTING_FILE_NM), Encoding.UTF8);
                    //}

                    Settings = JsonConvert.DeserializeObject<Config>(raw);
                }
			}
			catch (Exception err)
			{
				Settings = new Config();
			}
		}

        public static void LoadSetting()
        {
            if (Settings == null)
            {
                string raw;

                string settingfolderpath = GetSettingFolderPath();

                raw = File.ReadAllText(Path.Combine(settingfolderpath, SETTING_FILE_NM), Encoding.UTF8);

                // GetSettingFolderPath関数に切り出し

                //string localConfigFilePath = System.Configuration.ConfigurationManager.AppSettings["LocalConfigFilePath"];
                //if (string.IsNullOrWhiteSpace(localConfigFilePath) == false && File.Exists(Path.Combine(localConfigFilePath, SETTING_FILE_NM)))
                //{
                //    // ARMSWebの「Web.config」ファイル内の「LocalConfigFilePath」の設定を最優先でチェック 
                //    //    ※同一PC内で複数のARMSWebを立ち上げる為に必要 (使用場所：N工場SVライン - メイン中間PC)
                //    raw = File.ReadAllText(Path.Combine(localConfigFilePath, SETTING_FILE_NM), Encoding.UTF8);
                //}
                //else if (File.Exists(SETTING_FILE_FULLPATH) == true)
                //{
                //    raw = File.ReadAllText(SETTING_FILE_FULLPATH, Encoding.UTF8);
                //}
                //else if (File.Exists(Path.Combine(Environment.CurrentDirectory, SETTING_FILE_NM)))
                //{
                //    raw = File.ReadAllText(SETTING_FILE_NM, Encoding.UTF8);
                //}
                //else
                //{
                //    raw = File.ReadAllText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), SETTING_FILE_NM), Encoding.UTF8);
                //}

                Settings = JsonConvert.DeserializeObject<Config>(raw);
            }
        }

        public static string GetSettingFolderPath()
        {
            string localConfigFilePath = System.Configuration.ConfigurationManager.AppSettings["LocalConfigFilePath"];
            if (string.IsNullOrWhiteSpace(localConfigFilePath) == false && File.Exists(Path.Combine(localConfigFilePath, SETTING_FILE_NM)))
            {
                // ARMSWebの「Web.config」ファイル内の「LocalConfigFilePath」の設定を最優先でチェック 
                //    ※同一PC内で複数のARMSWebを立ち上げる為に必要 (使用場所：N工場SVライン - メイン中間PC)
                return localConfigFilePath;
            }
            else if (File.Exists(SETTING_FILE_FULLPATH) == true)
            {
                return Path.GetDirectoryName(SETTING_FILE_FULLPATH);
            }
            else if (File.Exists(Path.Combine(Environment.CurrentDirectory, SETTING_FILE_NM)))
            {
                return Environment.CurrentDirectory;
            }
            else
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        /// <summary>
        /// ネットワークサーバの設定ファイルの更新日時が新しい場合、ローカルにコピーする
        /// </summary>
        public static bool CopySettingFileFromServer(string serverPath, int retryCt)
        {
            try
            {
                string serverFullPath = Path.Combine(serverPath, SETTING_FILE_NM);
                if (File.Exists(serverFullPath) == false)
                {
                    throw new ApplicationException(
                        string.Format("ネットワークサーバの設定ファイルが存在しませんでした。設定に間違いがないか確認して下さい。コピー先:{0}、コピー元:{1}", SETTING_FILE_FULLPATH, serverFullPath));
                }


                DateTime localTimeStamp = File.GetLastWriteTime(SETTING_FILE_FULLPATH);
                DateTime serverTimeStamp = File.GetLastWriteTime(serverFullPath);

                if (retryCt >= 5)
                {
                    throw new ApplicationException(
                        string.Format("ネットワークサーバの設定ファイルコピーに失敗しました。ファイルが編集可能か確認して下さい。コピー先:{0}、コピー元:{1}", SETTING_FILE_FULLPATH, serverFullPath));
                }

                if (localTimeStamp < serverTimeStamp)
                {
                    File.Copy(serverFullPath, SETTING_FILE_FULLPATH, true);

                    return true;
                }

                return false;
            }
            catch (IOException ex)
            {
                System.Threading.Thread.Sleep(1000);
                retryCt = retryCt + 1;
                CopySettingFileFromServer(serverPath, retryCt);
                return false;
            }
        }


        public void Save()
        {
#if DEBUG
            //DBOneOfTwoInspectionProcNo = new List<int>();
            //DBOneOfTwoInspectionProcNo.Add(1);
            //PSTesterLinkInfo = new List<KeyValuePair<int,string>?>();
            //PSTesterLinkInfo.Add(new KeyValuePair<int, string>(6, "test"));
            //PSTesterLinkInfo.Add(new KeyValuePair<int, string>(7, string.Empty));
            //List<string> test = new List<string>();
            //test.Add("0x0001");
            //test.Add("0x0002");
            //WireMMFileSkipErrorCodeList = test.ToArray();
            //CutLabelDir = @"\\hqfs3\Share\SAC\LabelData\LT_Receive\SLC\NARUTO_4\";
            //Model.DefItem def = new Model.DefItem();
            //def.CauseCd = "causecd1";
            //def.ClassCd = "classcd1";
            //def.DefectCd = "defectcd1";
            //WireMMFileSkipErrorDefItem = def;
            //ErroMailTitle = "test";
            //ErrorMailFrom = "haruhisa.ishiguchi@nichia.co.jp";
            //List<string> list = new List<string>();
            //list.Add("haruhisa.ishiguchi@nichia.co.jp");
            //list.Add("haruhisa.ishiguchi@nichia.co.jp");
            //ErrorMailTo = list.ToArray();
            //MoldConveyorWaitStartWorkKb = Model.TimeLimit.JudgeKb.Start;

#endif
            //NascaOrderMoveInfo = new List<KeyValuePair<int, int>>();
            //NascaOrderMoveInfo.Add(new KeyValuePair<int,int>(70001, 1));
            //NascaOrderMoveInfo.Add(new KeyValuePair<int,int>(70002, 2));

            string raw = JsonConvert.SerializeObject(this, Formatting.Indented);
            try
            {
                File.WriteAllText(SETTING_FILE_FULLPATH, raw);
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("Config保存失敗 " + ex.ToString());
                throw new ApplicationException("Config保存失敗");
            }
        }


        /// <summary>
        /// エラーメール送信先
        /// </summary>
        public string[] ErrorMailTo { get; set; }

        /// <summary>
        /// エラーメールタイトル
        /// </summary>
        public string ErroMailTitle { get; set; }

        /// <summary>
        /// エラーメール送信元
        /// </summary>
        public string ErrorMailFrom { get; set; }

        /// <summary>
        /// エラーメール本文(本文頭)
        /// </summary>
        public string ErrorMailBodyHeader { get; set; }

		//public string MelWorkCompltBasePath { get; set; }

		//public string MelWorkStartBasePath { get; set; }

        /// <summary>
        /// ローカルDB接続文字列
        /// </summary>
        public string LocalConnString { get; set; }

		/// <summary>
		/// WBマッピング用MMファイル内のバッドマーク扱いコード
		/// </summary>
		public string[] WireMMFileBadmarkErrorCodeList { get; set; }

        public Model.DefItem WireMMFileSkipErrorDefItem { get; set; }

		public Model.DefItem WireMMFileBadmarkErrorDefItem { get; set; }

		public Model.DefItem AutoInspectionSkipErrorDefItem { get; set; }

        public enum LineTypes
        {
            NEL_SV,
            NEL_MAP,
            NEL_GAM,
            MEL_SV,
            MEL_MAP,
			MEL_GAM,
			MEL_19,
			MEL_MPL,
			MEL_83385,
            MEL_COB,
            MEL_SIGMA,
            MEL_NTSV,
			MEL_VOYAGER,
			MEL_KIRAMEKI,
			MEL_CSP,
			MEL_093
        }

        /// <summary>
        /// 自動化ライン形態
        /// </summary>
        public static LineTypes GetLineType
        {
            get
            {
                string val = Config.Settings.LineType;
                switch (val)
                {
                    case "1":
                        return LineTypes.NEL_SV;

                    case "2":
                        return LineTypes.NEL_MAP;

                    case "3":
                        return LineTypes.MEL_SV;

                    case "4":
                        return LineTypes.MEL_MAP;

                    case "5":
                        return LineTypes.NEL_GAM;

					case "6":
						return LineTypes.MEL_GAM;

					case "7":
						return LineTypes.MEL_19;

					case "8":
						return LineTypes.MEL_MPL;

					case "9":
						return LineTypes.MEL_83385;

					case "10":
						return LineTypes.MEL_COB;

                    case "11":
                        return LineTypes.MEL_SIGMA;

                    case "12":
                        return LineTypes.MEL_NTSV;

					case "13":
                        return LineTypes.MEL_VOYAGER;

					case "14":
                        return LineTypes.MEL_KIRAMEKI;

					case "15":
                        return LineTypes.MEL_CSP;

					case "16":
                        return LineTypes.MEL_093;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 自動化ライン形態
        /// </summary>
        public string LineType { get; set; }

        #region 作業ID
        /// <summary>作業開始</summary>
        public const string WORKID_WORK_START = "001";
        /// <summary>途中品</summary>
        public const string WORKID_NEXT_PROCESS = "002";
        /// <summary>フレーム搭載作業</summary>
        public const string WORKID_FRAME = "011";
        /// <summary>ダイボンド作業</summary>
        public const string WORKID_DIE_BONDING = "012";
        /// <summary>時間管理無の実績登録</summary>
        public const string WORKID_RESULT_NO_TIME = "014";
        /// <summary>開始完了</summary>
        public const string WORKID_START_COMPLT = "021";
        /// <summary>作業問合せ</summary>
        public const string WORKID_QUERY = "051";
        #endregion

        public string AsmLot1 { get; set; }

        public string AsmLot6789 { get; set; }

        public string CutBinCd { get; set; }

        public string InlineNo { get; set; }

        public string CutPreLabelDir { get; set; }

        public string CutLabelDir { get; set; }

        public string CutBlend12 { get; set; }

        public string CutBlend789A { get; set; }

        public string CutBlendOut789 { get; set; }

        /// <summary>
        /// マスタ連携時　取り込み対象の品目コードの末尾
        /// </summary>
        public string AsmMatCdSurfix {get;set;}

        /// <summary>
        /// NASCA原材料コード変換時の末尾文字
        /// </summary>
        public string MaterialCodeSurfix { get; set; }

        /// <summary>
        /// 原材料取り込み時に参照するBinJaのLike文字列
        /// </summary>
        public string MaterialsImportBinJaLike { get; set; }

        /// <summary>
        /// 高効率ライン　自動取り込み対象の搭載機
        /// </summary>
        public string[] MELFrameLoaders { get; set; }

        /// <summary>
        /// NASCA参照サーバー接続文字列
        /// </summary>
        public string NASCAConSTR { get; set; }
        
        /// <summary>
        /// DB号機単位のプロファイル設定を使うか
        /// </summary>
        public bool UseDBProfile { get; set; }

        /// <summary>
        /// 移載機(反転)作業CD
        /// </summary>
        public string MagExchangerReverseWorkCd { get; set; }

        /// <summary>
        /// 完成品排出CVに搬送する作業CD
        /// </summary>
        public string MastDoCompltDischargeWorkCd { get; set; }

        /// <summary>
        /// 強度試験DB接続文字列
        /// </summary>
        public string PSTesterConSTR { get; set; }
        
        /// <summary>
        /// EICS接続文字列
        /// </summary>
        public string QCILConSTR { get; set; }

        /// <summary>
        /// EICSメインサーバー接続文字列
        /// </summary>
        public string MainQCILConSTR { get; set; }

        /// <summary>
        /// 樹脂調合DB接続文字列
        /// </summary>
        public string ResinDBConSTR { get; set; }

        /// <summary>
        /// 他ラインのARMSメインサーバー接続文字列
        /// </summary>
        public string[] ArmsConSTRList { get; set; }

        /// <summary>
        /// PSTester用のファイル出力先情報
        /// 工程ID、出力先パス
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>?> PSTesterLinkInfo { get; set; }

        /// <summary>
        /// BDTester用のファイル出力先情報
        /// 工程ID、出力先パス
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>?> BDTesterLinkInfo { get; set; }

        /// <summary>
        /// EICSの設定ファイルパス
        /// 装置の現在設定型番を取得するために使用
        /// </summary>
        public string[] QCILXmlFilePath { get; set; }

        /// <summary>
        /// カットブレンド最大マガジン数
        /// </summary>
        public int CutBlendMagazineCt{ get; set; }

        /// <summary>
        /// ウェハー取り込みファイル保存場所
        /// </summary>
        public string[] WaferFilePath { get; set; }

        /// <summary>
        /// 部署コード
        /// </summary>
        public string SectionCd { get; set; }

        /// <summary>
        /// ダイボンド状態検査工程No
        /// </summary>
        public int DBInspectionProcNo { get; set; }

        /// <summary>
        /// ワイヤーボンド状態検査工程No
        /// </summary>
        public int WBInspectionProcNo { get; set; }

        /// <summary>
        /// ダイボンド状態検査工程No(2ロット中、1ロット抜き取り)
        /// </summary>
        public List<int?> DBOneOfTwoInspectionProcNo { get; set; }

        /// <summary>
        /// NASCA連携時　樹脂原材料引き落とし保管場所
        /// </summary>
        public string ResinBinCd { get; set; }

		///// <summary>
		///// Spider応答
		///// </summary>
		//public string NASCACommandReceiveDir { get; set; }

		///// <summary>
		///// Spider送信
		///// </summary>
		//public string NASCACommandSendDir { get; set; }

		///// <summary>
		///// Spider応答受信後保存先
		///// </summary>
		//public string NASCACommandDoneDir { get; set; }

		///// <summary>
		///// PDA用Spider応答
		///// </summary>
		//public string NASCACommandReceiveDir2nd { get; set; }

		///// <summary>
		///// PDA用Spider送信
		///// </summary>
		//public string NASCACommandSendDir2nd { get; set; }

		///// <summary>
		///// PDA用Spider応答受信後保存先
		///// </summary>
		//public string NASCACommandDoneDir2nd { get; set; }

		///// <summary>
		///// DataSpider応答タイムアウト
		///// </summary>
		//public int NASCATimeoutMilliSecond { get; set; }

        public string NascaPRFDItemLineNo { get; set; }

        public string[] NascaPRFDItemValLike { get; set; }

        /// <summary>
        /// Nasca公開API サーバ名(追加)
        /// </summary>
        public string NascaApiServerCd { get; set; }

        /// <summary>
        /// 作業監視画面更新間隔
        /// </summary>
        public int MonitorTimerMilliSecond { get; set; }
 
        /// <summary>
        /// NASCA 公開API
        /// </summary>
        public string NASCAWebService { get; set; }
 
        public int?[] FrmDefInputMacNoList { get; set; }

        /// <summary>
        /// Nascaライン識別CD
        /// </summary>
        public int NascaLineGroupCd { get; set; }

        /// <summary>
        /// Nasca指図発行/移動ステータス情報
        /// </summary>
        public List<KeyValuePair<int, int>> NascaOrderMoveInfo { get; set; }

        /// <summary>
        /// ProcNoをダイス分類CDで特定する必要のある作業CD
        /// </summary>
        public List<string> DiceClassCheckWorkCd { get; set; }

        /// <summary>
        /// システムログ保管期間
        /// </summary>
        public int SystemLogKeepDay { get; set; }

        /// <summary>
        /// 作業順取り込み有効/無効
        /// </summary>
        public bool ImportWorkFlow { get; set; }

		/// <summary>
		/// Nasca不良ファイルパス
		/// </summary>
		public string NascaDefectFilePath { get; set; }

		/// <summary>
		/// Nasca不良ファイル完了後保管パス
		/// </summary>
		public string NascaDefectFileDonePath { get; set; }

		/// <summary>
		/// Nasca不良ファイル取り込み有効/無効
		/// </summary>
		public bool ImportNascaDefect { get; set; }

		public bool UseOvenProfiler { get; set; }

		/// <summary>
		/// カットラベル照合有効/無効
		/// </summary>
		public bool HasCutLabelCompare { get; set; }

		/// <summary>
		/// ダイシェア抜き取り試験が必要か確認処理をする工程No
		/// </summary>
		public int? DieShearSamplingCheckProcNo { get; set; }

		/// <summary>
		/// ダイシェア抜き取り試験が必要か判定材料にする作業CD
		/// </summary>
		public string DieShearSamplingJudgeWorkCd { get; set; }

		/// <summary>
		/// ロット番号の連番箇所初期値
		/// </summary>
		public List<KeyValuePair<string, int>> LotDefaultSerialNumber { get; set; }

        /// <summary>
        /// カットロット番号の連番箇所初期値
        /// </summary>
        public List<KeyValuePair<string, int>> CutLotDefaultSerialNumber { get; set; }

        /// <summary>
        /// LENSサーバー接続文字列
        /// </summary>
        public string LENSConSTR { get; set; }

		/// <summary>
		/// マッピング機能有効/無効
		/// </summary>
		public bool IsMappingMode { get; set; }

		/// <summary>
		/// リードフレーム成型金型情報取り込み有効/無効
		/// </summary>
		public bool IsFrameMoldedImport { get; set; }

		/// <summary>
		/// リードフレーム成型金型区分取り込み対象
		/// </summary>
		public List<string> FrameMoldedImportClass { get; set; }

		/// <summary>
		/// リードフレーム成型金型区分検査対象
		/// </summary>
		public List<string> FrameMoldedInspectionClass { get; set; }

		/// <summary>
		/// 2015.4.27 車載自動化(2015.GW限定)のDBProfileのロット数に応じてマガジンを供給するモード
		/// </summary>
		public bool IsDBProfileRequireMode { get; set; }

		/// <summary>
		/// 2015.4.27 車載自動化(2015.GW限定)のロボットが運転前に作業開始前チェックをするモード
		/// </summary>
		public bool IsBeforeDriveWorkStartCheckerMode { get; set; }

		public string NascaApiLoginUser { get; set; }

		public string NascaApiLoginPassword { get; set; }

		///// <summary>
		///// カット工程でカットブレンドチェックをするかどうか
		///// </summary>
		//public bool IsCutBlendErrorCheck { get; set; }

		/// <summary>
		/// 部材交換免責不良CD
		/// </summary>
		public List<string> MaterialChangeDefectCode { get; set; }

		/// <summary>
		/// PsTesterの型番マスタを対象にダイシェア抜き取り試験をするかどうか
		/// </summary>
		public bool IsDieShearSamplingFromPsTesterType { get; set; }

		/// <summary>
		/// NASCAロットを取り込む対象の工程NO
		/// </summary>
		public List<string> LotImportOfTargetNascaProcess { get; set; }

		/// <summary>
		/// 工程を識別するコード
		/// </summary>
		public List<string> NascaFGroupCd { get; set; }

		/// <summary>
		/// 出力完了トリガファイルのアクセスリトライ回数(1回1秒待機)
		/// </summary>
		public int FinishedFileAccessRetryCt { get; set; }

        /// <summary>
        /// ダイシェア抜き取り試験を設備・日毎にするかどうか (DieShearSamplingCheckProcNoの設定がなければ何もしない)
        /// </summary>
        public bool IsDieShearSamplingEachMachineDay { get; set; }

        /// <summary>
        /// 品目統合対象タイプ
        /// </summary>
        public List<string> UnificationTargetTypeList { get; set; }

        /// <summary>
        /// マッピングデータ取り込み有効/無効
        /// </summary>
        public bool ImportMappingData { get; set; }

        /// <summary>
        /// マッピングファイルが保存されているパス
        /// </summary>
        public string MappingDirectoryPath { get; set; }

		/// <summary>
		/// 高生産性で取り込む対象のロット加工状態(DBC or DBA)
		/// </summary>
		public string ImportLotLineState { get; set; }

        /// <summary>
        /// 資材開封ファイル取り込み先
        /// </summary>
        public string MaterialOpenFilePath { get; set; }

        /// <summary>
        /// 検査機でNGマーキングをする
        /// </summary>
        public bool IsInspectorNgMarking { get; set; }

        /// <summary>
        /// 使用不可とする有効期限の残分
        /// </summary>
        public int? RemainingTimeOfLimit { get; set; }

        /// <summary>
        /// 警告勧告を出す有効期限の残分
        /// </summary>
        public int WarningMinutesOfLimit { get; set; }

        /// <summary>
        /// 注意勧告を出す有効期限の残分
        /// </summary>
        public int AttentionMinutesOfLimit { get; set; }

        /// <summary>
        /// stopファイルを配置する有効期限の残分
        /// </summary>
        public int StopMinutesOfLimit { get; set; }

        /// <summary>
        /// 基板の厚みランク取り込み有効/無効
        /// </summary>
        public bool ImportSubstrateThicknessRankData { get; set; }

        /// <summary>
        /// 基板の厚みランクファイルが保存されているパス
        /// </summary>
        public string SubstrateThicknessRankDirectoryPath { get; set; }

        /// <summary>
        /// 廃棄ラベル印刷用ファイルの出力先パス
        /// </summary>
        public string ScrapLabelDirectoryPath { get; set; }

        /// <summary>
        /// 基板厚みランクが同条件かのチェックをする
        /// </summary>
        public bool IsFrameThicknessRankCheck{ get; set; }
        
        /// <summary>
        /// アッセンの樹脂グループと照合を行う資材の品目グループコード
        /// </summary>
        public string[] CompareResinMaterialGroup { get; set; }

        /// <summary>
        /// 金線か照合を行う資材の品目グループコード
        /// </summary>
        public string CompareGoldWireMaterialGroup {get;set;}

        /// <summary>
        /// ダイスの厚み平均値を取得するかどうかのフラグ
        /// </summary>
        public bool ImportMatThickness { get; set; }

        /// <summary>
        /// ダイシェア試験結果がNG以外のロットの流動規制を解除する対象のPstesterファンクション
        /// </summary>
        public int? CancelRestrictDieShearFunctionId { get; set; }

        /// <summary>
        /// SQデータベースへの接続文字列
        /// </summary>
        public string SQDBConSTR { get; set; }

        /// <summary>
        /// SQデータベースへの接続文字列
        /// </summary>
        public List<ExamsMngInfoSrch_Condition> ExamsMngInfoSrchCondList { get; set; }

        /// <summary>
        /// 遠心沈降作業
        /// </summary>
        public List<string> EckWorkCdList { get; set; }

        /// <summary>
        /// 調合場所選択リスト
        /// </summary>
        public List<string> ExpectPlaceCdList { get; set; }

        /// <summary>
        /// 樹脂調合DB サーバー名
        /// </summary>
        public string ResinDBServerCd { get; set; }

        /// <summary>
        /// LAMSサーバー接続文字列
        /// </summary>
        public string LAMSConSTR { get; set; }

        /// <summary>
        /// 在庫数を取り込む保管場所CDリスト
        /// </summary>
        public List<int> StockImportBinCodeList { get; set; }

        /// <summary>
        /// カットブレンド(SV自)開始時の履歴チェック対象外の作業番号リスト
        /// </summary>
        public List<int> HistoryNoCheckProcList { get; set; }

        /// <summary>
        /// ウェハーに紐付くリングIDの取り込みを行うかのフラグ
        /// </summary>
        public bool ImportMatRingId { get; set; }


        /// <summary>
        /// DMC(ARMS4)のマッピングデータフォルダ
        /// </summary>
        public string ARMS4MappingDirectoryPath { get; set; }

        /// <summary>
        /// ロット-キャリア紐付時に初期の空マッピングデータを作るかどうかのフラグ
        /// </summary>
        public bool CreateAllOKMappingForMainte { get; set; }

        /// <summary>
        /// 膜厚Tester用のファイル出力先情報
        /// 工程ID、出力先パス
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>?> FilmTesterLinkInfo { get; set; }

        /// <summary>
        /// 装置作業者設定画面(FrmMachineOperator)に表示するキー文字 TnMachineOperator.showkeyと一致させる
        /// </summary>
        public string OperationMachineGroupShowKey { get; set; }

        /// <summary>
        /// 自動登録の作業者記録機能の有効/無効  有効の場合、データメンテナンスで監視ON
        /// </summary>
        public bool ActiveMachineOperator { get; set; }

        /// <summary>
        /// ROOTS参照サーバー接続文字列
        /// </summary>
        public string ROOTSConSTR { get; set; }


        /// <summary>
        /// マガジンプレート印刷ソフト用CSVファイル作成先フォルダ
        /// </summary>
        public string OutLabelOutputDirectoryPath { get; set; }


        /// <summary>
        /// ARMS3の装置監視間隔 ARMSThreadObjectクラス -> ThreadRoutineWork関数の装置のSleep時間(ミリ秒)
        /// </summary>
        public int ThreadRoutineWorkMilliSecond { get; set; }

        /// <summary>
        /// 色調自動測定機の割付作業CDリスト
        /// </summary>
        public List<string> ColorAutoMeasurerWorkCdList  { get; set; }

        /// <summary>
        /// 自動貼付機の割付作業CDリスト
        /// </summary>
        public List<string> AutoPasterWorkCdList { get; set; }


        /// <summary>
        /// メーカーの基板マーキングNoファイル保存先フォルダ
        /// </summary>
        public string MarkingNoDirectoryPath { get; set; }


        /// <summary>
        /// ネットワークサーバのArmsConfigファイルの保管場所
        /// </summary>
        public string OriginalConfigFileStorageFolderPath { get; set; }

        /// <summary>
        /// ネットワークサーバのlineconfigファイルの保管場所
        /// </summary>
        public string OriginalLineFileStorageFolderPath { get; set; }

        /// <summary>
        /// ARMSのシステムバージョン
        /// </summary>
        public string ArmsSystemVersion { get; set; }

        /// <summary>
        /// ArmsMaintenanceのシステムバージョン
        /// </summary>
        public string ArmsMaintenanceSystemVersion { get; set; }

        /// <summary>
        /// ArmsMonitorのシステムバージョン
        /// </summary>
        public string ArmsMonitorSystemVersion { get; set; }

        /// <summary>
        /// ARMSMonitorのクリックワンスパス
        /// </summary>
        public string ArmsMonitorClickOnceFullPath { get; set; }
        /// <summary>
        /// 装置選択画面に表示する装置の検索条件リスト
        /// </summary>
        public List<KeyValuePair<string, string>> FrmDefInputLineAndMachineClassList { get; set; }

        /// <summary>
        /// 蛍光体シートリング搭載自動機の作業CD
        /// </summary>
        public List<string> SheetRingLoaderWorkCdList { get; set; }

        /// <summary>
        /// 強度試験対象ロットを決める作業No
        /// </summary>
        public int? SetPSTesterProcNo { get; set; }
        
        /// <summary>
        /// ロボットが装置へアクセス中に作成するファイルのディレクトリパス
        /// </summary>
        public string MachineAccessDirectoryPath { get; set; }

        /// <summary>
        /// 強度試験結果がNG以外のロットの流動規制を解除する対象のPstesterファンクション
        /// </summary>
        public List<int> CancelRestrictPSTesterFunctionIdList { get; set; }

        /// <summary>
        /// ARMS3の「信号確認」画面に表示する装置のライン
        /// </summary>
        public List<string> SignalDisplayLineNoList { get; set; }
    }
}
