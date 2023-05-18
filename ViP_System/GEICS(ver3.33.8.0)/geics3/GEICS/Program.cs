using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace GEICS
{
    static class Program
    {
        /// <summary>アプリケーションバージョン</summary>
        public static string SystemVer = "";
        public static string sBatch = "";

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //バッチ用GEICSはコマンドライン引数になんでも良いので、文字列を入れる。 →バッチGEICS
            //ClickOnce発行時はコマンドライン引数に文字列を入れない。               →通常GEICS
           if (args.Length > 0)
            {
                try
                {
                    BatchReport(args[0], args[1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                return;
            }

            //アプリケーションバージョンの取得
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                SystemVer = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                SystemVer = Application.ProductVersion + " -NonUpdate";
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
#if NMC
            //視覚言語の設定
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("EN");
            //メッセージ言語の設定
            Constant.MessageInfo = (IMessage)System.Activator.CreateInstance(Type.GetType("GEICS.MessageENInfo"));
#else
            //視覚言語の設定
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("JA");
            //メッセージ言語の設定
            Constant.MessageInfo = (IMessage)System.Activator.CreateInstance(Type.GetType("GEICS.MessageJAInfo"));
#endif
            //設定ファイルの値取得
            Constant.settingInfoList = SettingInfo.GetSetting();

			if((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				Constant.fClient = true;
				Application.Run(new F01_Login());
			}
			else if (Constant.fClient) 
            {
                Application.Run(new F01_Login());
            }
            else
            {
                Application.Run(new F04_ErrorRecord());
            }
            
        }

        static public void BatchReport(string arg, string batchNm)
        {
            try
            {
				BLCS_Library.LogHelper.EntryLog(batchNm, System.DateTime.Now.ToString(), null, null);

                switch (arg)
                {
                    case "/HIGH":
                    case "/HIGH_MONTH":
                        Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_SV_HIGH, ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
                        break;
                    case "/AUTO":
                    case "/AUTO_MONTH":
                        Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_SV_AUTO, ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
                        break;
					case "/MAP_HIGH":
					case "/MAP_HIGH_MONTH":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_MAP_HIGH, ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
						break;
					case "/MAP_AUTO":
					case "/MAP_AUTO_MONTH":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_MAP_AUTO, ConnectQCIL.CONNECT_USER_ID, ConnectQCIL.CONNECT_USER_PASS);
						break;
#if AOI
					case "/AOI_SV_AUTO":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_SV_AUTO, ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
						break;
					case "/AOI_SV_HIGH":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_SV_HIGH, ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
						break;
					case "/AOI_MAP_AUTO":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_MAP_AUTO, ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
						break;
					case "/AOI_MAP_HIGH":
						Constant.StrQCIL = string.Format(Properties.Settings.Default.ConnectionString_BATCH_MAP_HIGH, ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS);
						break;
#endif
                    default:
                        return;
                }

                List<int> ListLineNo = GetLineNoList();

                //自動搬送・高生産性ライン分ループ
                foreach (int nLine in ListLineNo)
                {
                    Common.nLineCD = nLine;

                    //データベース選択
#if !AOI && !KYO && !MIK && !CEJ && !KMC
                    if (Common.nLineCD >= 1000 && Common.nLineCD < 2000)
                    {
                        //SIDEVIEW(自動搬送)
                        Constant.StrQCIL = Constant.StrQCIL_SV_AUTO;
                    }
                    else
                    {
                        continue;
                    }
#endif

                    F07_Report frmReport = new F07_Report();
                    sBatch = arg;
                    frmReport.BatchReport(arg);
                    frmReport.Dispose();
                }

				BLCS_Library.LogHelper.EntryLog(batchNm, null, System.DateTime.Now.ToString(), null);

            }
            catch (Exception err) 
            {
                log4net.Log.Logger.Info(err.Message + "\r\n" + err.StackTrace);
            }
        }

        static public List<int> GetLineNoList()
        {
            List<int> ListLineNo = new List<int>();

            string sqlCmdTxt = "SELECT DISTINCT [Inline_CD] FROM [TmLINE] WITH(NOLOCK) WHERE Del_FG <> '1' ORDER BY [Inline_CD]  ASC";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        ListLineNo.Add(Convert.ToInt32(reader["Inline_CD"]));
                        i = i + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return ListLineNo;
        }
    }
}
