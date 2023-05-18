using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 検査機(高効率用)
    /// </summary>
    public class Inspector2 : Inspector
    {
        /// <summary>
        /// コンベア排出予約アドレス
        /// </summary>
        public string OutputReserveBitAddress { get; set; }

		/// <summary>
		/// ローダー側マガジンNo読取完了アドレス
		/// </summary>
		public string LoaderQRReadCompleteBitAddress { get; set; }

		/// <summary>
		/// 開始登録OKBit
		/// </summary>
		public string WorkStartOKBitAddress { get; set; }

		/// <summary>
		/// 開始登録NGBit
		/// </summary>
		public string WorkStartNGBitAddress { get; set; }

		/// <summary>
		/// 完了登録NGBit
		/// </summary>
		public string WorkCompleteNGBitAddress { get; set; }

		/// <summary>
		/// 作業開始登録を本体側でするフラグ
		/// </summary>
		public bool IsWorkStartAutoComplete { get; set; }

		/// <summary>
		/// 作業完了登録を本体側でするフラグ
		/// </summary>
		public bool IsWorkEndAutoComplete { get; set; }

        protected override void concreteThreadWork()
        {
            if (this.IsRequireOutput() == true)
            {
                workComplete();
            }

			if (this.IsWorkStartAutoComplete)
			{
                if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
                {
					workStart();
				}
			}

	    	// MMファイル処理
    		CheckMachineLogFile();

			//Nasca不良ファイル取り込み
			Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

		public virtual void workStart()
		{
			VirtualMag mag = new VirtualMag();

			string magno = Plc.GetMagazineNo(LMagazineAddress);
			if (string.IsNullOrEmpty(magno) == false)
			{
				mag.MagazineNo = magno;
				mag.LastMagazineNo = magno;
			}
			else
			{
				throw new ApplicationException("[開始登録異常] 検査機搬入マガジンNOの取得に失敗。\n検査機搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
			}
			Magazine svrmag = Magazine.GetCurrent(magno);
			if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

			OutputSysLog(string.Format("[開始処理] 開始 LoaderMagazineNo:{0}", magno));

			AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

			mag.ProcNo = nextproc.ProcNo;
			mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
			
			Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

			ArmsApiResponse workResponse = CommonApi.WorkStart(order);
			if (workResponse.IsError)
			{
				Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
				Log.ApiLog.Info(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
			}
			else
			{
				Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
				OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", mag.MagazineNo)); 
			}
		}

        public virtual void workComplete()
        {
            VirtualMag mag = new VirtualMag();

            VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader));

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress, true);

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                mag.MagazineNo = newmagno;
                mag.LastMagazineNo = newmagno;
            }
            else
            {
				throw new ApplicationException("[完了登録異常] 検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
			}
            Magazine svrmag = Magazine.GetCurrent(newmagno);
			if (svrmag == null) throw new ApplicationException("[完了登録異常] マガジン情報が見つかりません" + newmagno);

			OutputSysLog(string.Format("[完了処理] 開始 UnLoaderMagazineNo:{0}", newmagno)); 

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
            mag.ProcNo = nextproc.ProcNo;

            //既にキュー内に存在するかを確認
            bool found = false;
            foreach (VirtualMag exist in mags)
            {
                if (exist.MagazineNo == mag.MagazineNo)
                {
                    found = true;
                }
            }

            //既存キュー内に存在しない場合のみ検査結果をまってEnqueue
            if (found == false)
            {
                try
                {
                    //作業開始完了時間取得
                    mag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
					if (mag.WorkComplete.HasValue == false)
					{
						Log.ApiLog.Info("検査機排出位置のマガジンに完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
						return;
					}

					if (IsWorkStartAutoComplete)
					{
						Order startOrder = Order.GetMachineOrder(this.MacNo, svrmag.NascaLotNO);
						if (startOrder == null) 
						{
							throw new ApplicationException(string.Format("作業の開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。LotNo:{0}", svrmag.NascaLotNO));
						}
						mag.WorkStart = startOrder.WorkStartDt;
					}
					else
					{
						mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
						if (mag.WorkStart.HasValue == false)
						{
							Log.ApiLog.Info("検査機排出位置のマガジンに開始時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
							return;
						}
					}
                }
                catch
                {
                    Log.ApiLog.Info("検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
                    return;
                }

				////EICSが作成したNasca不良ファイルを登録
				//bool result = RunDefect(mag.MagazineNo, this.PlantCd, mag.ProcNo.Value);
				//if (result == false) 
				//{
				//	return;
				//}

                //現在完了工程で削除フラグONかつ規制理由「」が存在する場合、毎回規制を復活させる
                if (svrmag == null) throw new ApplicationException("検査機マガジンデータ異常　現在稼働中マガジンがありません:" + svrmag);
                Restrict[] reslist = Restrict.SearchRestrict(svrmag.NascaLotNO, nextproc.ProcNo, false);
                foreach (Restrict res in reslist)
                {
                    //周辺強度による規制理由と同じか確認
                    if (res.Reason == WireBonder.MMFile.RESTRICT_REASON_2)
                    {
                        //規制を有効化
                        res.DelFg = false;
                        res.Save();
                    }
                }

                //完了登録にUnloaderマガジンが必要なので先に作成
                this.Enqueue(mag, Station.Unloader);

                //高効率でArmsWebが作成してしまうので削除
                this.Dequeue(Station.Loader);

				if (this.IsWorkEndAutoComplete)
				{
					Order order = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
					ArmsApiResponse workResponse = CommonApi.WorkEnd(order);

					this.Dequeue(Station.Unloader);

					if (workResponse.IsError)
					{
						Plc.SetBit(this.WorkCompleteNGBitAddress, 1, Mitsubishi.BIT_ON);
						Log.ApiLog.Info(string.Format("[完了登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
						return;
					}
				}
            }

            //立ち上げ要求送信
            if (Plc.GetBit(this.OutputReserveBitAddress) == Mitsubishi.BIT_OFF)
            {
                //この時点で要求が0かつ、マガジンが最初と一致している場合は排出許可
                string currentMag = Plc.GetMagazineNo(ULMagazineAddress, true);
                if (currentMag == newmagno)
                {
                    Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);
					OutputSysLog(string.Format("[完了処理] 完了 UnLoaderMagazineNo:{0}", newmagno)); 
                }
            }
        }

		///// <summary>
		///// 1ロットの全ログファイルを取得
		///// </summary>
		///// <param name="directoryPath"></param>
		///// <param name="startDt"></param>
		///// <param name="endDt"></param>
		///// <returns></returns>
		//public static List<string> GetLotFiles(string directoryPath, DateTime startDt, DateTime endDt)
		//{
		//	List<string> retv = new List<string>();

		//	List<string> files = MachineLog.GetFiles(directoryPath);
		//	foreach (string file in files)
		//	{
		//		if (File.Exists(file) == false)
		//		{
		//			if (File.Exists(string.Format("_{0}", file)))
		//			{
		//				throw new ApplicationException(
		//					string.Format("装置パラメータのチェックが終わっていないか、存在しないファイルを参照しようとしてます。 ファイルパス:{0}", file));
		//			}
		//		}

		//		string fileName = Path.GetFileNameWithoutExtension(file);

				

		//		DateTime outputDate;
		//		string date = fileName.Substring(9, 2) + "/" + fileName.Substring(11, 2) + "/" + fileName.Substring(13, 2) + " " +
		//			fileName.Substring(15, 2) + ":" + fileName.Substring(17, 2) + ":" + fileName.Substring(19, 2);
		//		if (DateTime.TryParse(date, out outputDate) == false)
		//		{
		//			throw new ApplicationException(string.Format("日付型に変換できないファイル名が付いています ファイルパス:{0}", fileName));
		//		}

		//		if (outputDate >= startDt && outputDate <= endDt)
		//		{
		//			retv.Add(file);
		//		}
		//	}

		//	return retv;
		//}
    }
}
