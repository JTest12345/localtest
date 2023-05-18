using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmsApi.Model.NASCA
{
    public class Spider
    {
        /// <summary>
        /// NASCAのリトライ応答に何度対応するか
        /// </summary>
        private const int MAX_RETRY_TIMES = 1;

        public enum NASCAJobCategory : int
        {
            WorkStart = 1,
            StartProgress = 2,
            FrameComplete = 11,
            DBComplete = 12,
            OtherComplete = 14,
            WorkStartAndComplete = 21,
            RequestNextWork = 51,
        }

        /// <summary>
        /// Spider区切り文字
        /// </summary>
        public const char SPL = ',';

        public const char AT = '@';


        #region sendNASCACommandToSpider
		//public static NASCAResponse SendNASCACommandToSpider(string command, int currentRetryTimes, bool isSecondary, string mutexname)
		//{
		//	string senddir, receivedir, donedir;

		//	if (!isSecondary)
		//	{
		//		senddir = Config.Settings.NASCACommandSendDir;
		//		receivedir = Config.Settings.NASCACommandReceiveDir;
		//		donedir = Config.Settings.NASCACommandDoneDir;
		//	}
		//	else
		//	{
		//		senddir = Config.Settings.NASCACommandSendDir2nd;
		//		receivedir = Config.Settings.NASCACommandReceiveDir2nd;
		//		donedir = Config.Settings.NASCACommandDoneDir2nd;
		//	}

		//	Mutex mut = new Mutex(false, mutexname);
		//	if (mut.WaitOne(Config.Settings.NASCATimeoutMilliSecond + 5000))
		//	{
		//		try
		//		{
		//			Log.SysLog.Info("[FILE-OUT]:" + command);
		//			string fileheader = Config.Settings.SpiderInlineNo + "_" + DateTime.Now.Ticks.ToString();
		//			string path = Path.Combine(senddir, fileheader + ".csv");
		//			Log.SysLog.Info("path:" + path);

		//			DateTime sendtime = DateTime.Now;

		//			using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS")))
		//			{
		//				sw.WriteLine(command);
		//			}

		//			//Spider応答ファイル待ち受け
		//			string responseFile = getLastFile(receivedir, fileheader, sendtime, Config.Settings.NASCATimeoutMilliSecond);

		//			//Spider応答をパース
		//			NASCAResponse response = NASCAResponse.ParseResponseFile(responseFile, 0);

		//			//処理済みフォルダへ退避
		//			try
		//			{
		//				File.Move(responseFile, Path.Combine(donedir, Path.GetFileName(responseFile)));
		//			}
		//			catch (Exception fex)
		//			{
		//				//処理済みファイルの移動は失敗してもログだけ保存して継続
		//				Log.SysLog.Error("ファイル移動失敗:" + fex.ToString());
		//			}

		//			Log.SysLog.Info("[FILE-IN]:" + response.OriginalMessage);

		//			//リトライ応答の場合は再帰
		//			if (response.Status == NASCAStatus.Retry)
		//			{
		//				Log.SysLog.Error("NASCA応答リトライ発生:" + command);
		//				if (currentRetryTimes > MAX_RETRY_TIMES)
		//				{
		//					Log.SysLog.Error("リトライ回数が制限を超えたため処理中断");
		//					return NASCAResponse.GetNGResponse("リトライ回数が制限を超えたため処理中断");
		//				}

		//				return SendNASCACommandToSpider(command, ++currentRetryTimes, isSecondary, mutexname);
		//			}
		//			return response;
		//		}
		//		catch (TimeoutException ex)
		//		{
		//			Log.SysLog.Error("NASCA送受信タイムアウトエラー" + ex.ToString());
		//			return NASCAResponse.GetNGResponse("NASCA送受信でタイムアウトエラーが発生");
		//		}
		//		catch (Exception ex)
		//		{
		//			Log.SysLog.Error("NASCA送受信エラー" + ex.ToString());
		//			return NASCAResponse.GetNGResponse("NASCA送受信で不明なエラーが発生");
		//		}
		//		finally
		//		{
		//			mut.ReleaseMutex();
		//			mut.Close();
		//		}
		//	}
		//	else
		//	{
		//		mut.Close();
		//		Log.SysLog.Error("NASCA送受信タイムアウトエラー Mutex取得タイムアウト");
		//		return NASCAResponse.GetNGResponse("NASCA送受信でタイムアウトエラーが発生");
		//	}
		//}


        #endregion


        #region getLastFile 送信日時移行に作られた最後のファイルを返す リトライあり

        /// <summary>
        /// sendtime以降に作られた先頭のファイルを返す
        /// </summary>
        /// <param name="basepath"></param>
        /// <param name="sendtime"></param>
        /// <param name="timeoutMilliSeconds"></param>
        /// <returns></returns>
        private static string getLastFile(string basepath, string filenameHeader, DateTime sendtime, int timeoutMilliSeconds)
        {
            Log.SysLog.Info(string.Format("GetLastFile start send:{0} timeout:{1}", sendtime, timeoutMilliSeconds));
            DirectoryInfo dir = new DirectoryInfo(basepath);
            FileInfo[] files = dir.GetFiles(filenameHeader + "*");

            FileInfo top = files.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if (top != null)
            {
                return top.FullName;
            }

            if ((DateTime.Now - sendtime).TotalMilliseconds >= timeoutMilliSeconds)
            {
                //タイムアウト
                throw new TimeoutException("DataSpider応答タイムアウト");
            }
            else
            {
                //5秒待機して再帰
                Thread.Sleep(5000);
                return getLastFile(basepath, filenameHeader, sendtime, timeoutMilliSeconds);
            }
        }
        #endregion

        #region GetCutBlendStartCommand
        /// <summary>
        /// カット指図発行コマンド
        /// </summary>
        /// <param name="cutblendList"></param>
        /// <returns></returns>
		//public static string GetCutBlendStartCommand(CutBlend[] cutblendList)
		//{
		//	string macno = cutblendList[0].MacNo.ToString();
		//	string bincd = Config.Settings.CutBinCd;
		//	MachineInfo m = MachineInfo.GetMachine(cutblendList[0].MacNo);
		//	DateTime startdt = cutblendList[0].StartDt;
		//	string cutlotno = cutblendList[0].BlendLotNo;


		//	string cmd =
		//		  Config.Settings.SpiderInlineNo + SPL
		//			  + "CUTBLEND" + SPL
		//			  + bincd + SPL
		//			  + startdt.ToString() + SPL
		//			  + m.NascaPlantCd + SPL
		//			  + cutlotno + SPL;

		//	bool isfirst = true;
		//	foreach (CutBlend cb in cutblendList)
		//	{
		//		if (!isfirst) cmd += ";";
		//		cmd += cb.LotNo;
		//		isfirst = false;
		//	}

		//	return cmd;
		//}
        #endregion

        #region GetMnfctCommand
        /// <summary>
        /// </summary>
        /// <param name="cutblendList"></param>
        /// <returns></returns>
		//public static string GetMnfctCommand(string lotno, string magno, string plantcd, DateTime startdt, DateTime? enddt,
		//	Material[] materials, Resin[] resins, string[] partsList, DefItem[] defs, string startempcd,
		//	string compempcd, string comment, string inspectempcd, int inspectct)
		//{
		//	if (string.IsNullOrEmpty(startempcd)) startempcd = "660";
		//	if (string.IsNullOrEmpty(compempcd)) compempcd = "660";
		//	if (string.IsNullOrEmpty(inspectempcd)) inspectempcd = "660";

		//	if (comment != null)
		//	{
		//		comment = comment.Replace(';', '；').Replace('@', '＠')
		//			.Replace(',', '，').Replace('\r', ' ').Replace('\n', ' ');
		//	}
		//	magno = magno ?? "";

		//	StringBuilder cmd = new StringBuilder();

		//	cmd.Append(
		//		  Config.Settings.SpiderInlineNo + SPL
		//			  + "MNFCT" + SPL
		//			  + plantcd + SPL
		//			  + lotno + SPL
		//			  + magno + SPL
		//			  + startdt.ToString() + SPL
		//			  + enddt.ToString() + SPL);


		//	bool isfirst = true;
		//	if (partsList != null)
		//	{
		//		List<string> existsCondList = new List<string>();
		//		foreach (string parts in partsList)
		//		{
		//			if (existsCondList.Contains(parts) == false)
		//			{
		//				if (!isfirst) cmd.Append(";");
		//				cmd.Append(parts);
		//				isfirst = false;
		//				existsCondList.Add(parts);
		//			}
		//		}
		//	}

		//	cmd.Append(SPL);

		//	isfirst = true;
		//	if (defs != null)
		//	{
		//		foreach (DefItem def in defs)
		//		{
		//			if (isfirst == false) cmd.Append(";");
		//			cmd.Append(def.CauseCd + "@" + def.ClassCd + "@" + def.DefectCd + "@" + def.DefectCt.ToString());
		//			isfirst = false;
		//		}
		//	}

		//	cmd.Append(SPL);

		//	if (materials != null)
		//	{
		//		List<Material> existsMatList = new List<Material>();
		//		isfirst = true;
		//		foreach (Material mat in materials)
		//		{
		//			//同一ロットの重複は排除
		//			if (existsMatList.Contains(mat) == false)
		//			{
		//				if (isfirst == false) cmd.Append(";");
		//				cmd.Append(mat.MaterialCd + "@" + mat.LotNo + "@0");
		//				isfirst = false;
		//				existsMatList.Add(mat);
		//			}
		//		}
		//	}

		//	cmd.Append(SPL);

		//	if (resins != null)
		//	{
		//		List<Resin> existsResinList = new List<Resin>();
		//		isfirst = true;
		//		foreach (Resin res in resins)
		//		{
		//			if (existsResinList.Contains(res) == false)
		//			{
		//				if (isfirst == false) cmd.Append(";");
		//				cmd.Append(res.MixResultId + "@@" + res.ResinGroupCd);
		//				isfirst = false;
		//				existsResinList.Add(res);
		//			}
		//		}
		//	}

		//	cmd.Append(SPL);
		//	cmd.Append(startempcd);
		//	cmd.Append(SPL);
		//	cmd.Append(compempcd);
		//	cmd.Append(SPL);
		//	cmd.Append(comment);
		//	cmd.Append(SPL);

		//	//2=抜き取り　3=全数
		//	if (inspectct < 0)
		//	{
		//		cmd.Append("3@");
		//	}
		//	else
		//	{
		//		cmd.Append("2@");
		//	}

		//	cmd.Append(inspectempcd);
		//	cmd.Append("@");
		//	if (inspectct < 0)
		//	{
		//		cmd.Append("0");
		//	}
		//	else
		//	{
		//		cmd.Append(inspectct.ToString());
		//	}

		//	return cmd.ToString();
		//}
        #endregion

		//public static string GetOrderCommand(string lotno, string profileno, int ordermove, DateTime movedt)
		//{
		//	//インラインNO, ORDERMOVE, 対象ロットNO, 指図/移動区分, プロファイルNO, 指図発行日
		//	return GetOrderCommand(lotno, profileno, ordermove, movedt, -1);
		//}

		//public static string GetOrderCommand(string lotno, string profileno, int ordermove, DateTime movedt, int orderct)
		//{
		//	//インラインNO, ORDERMOVE, 対象ロットNO, 指図/移動区分, プロファイルNO, 指図発行日, 指図数量

		//	StringBuilder sb = new StringBuilder();
		//	sb.Append(Config.Settings.SpiderInlineNo);
		//	sb.Append(SPL);

		//	sb.Append("ORDERMOVE");
		//	sb.Append(SPL);

		//	sb.Append(lotno);
		//	sb.Append(SPL);

		//	sb.Append(ordermove.ToString());
		//	sb.Append(SPL);

		//	sb.Append(profileno);
		//	sb.Append(SPL);

		//	sb.Append(movedt);
		//	sb.Append(SPL);

		//	sb.Append(orderct.ToString());

		//	return sb.ToString();
		//}
    }
}
