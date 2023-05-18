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
    /// ウェットブラスト装置(高効率用)
    /// </summary>
    public class WetBlaster : MachineBase
	{
		private const int CARRIER_WORD_LEN = 10;
		private const int MAX_MAG_STEP_CT = 40;
		private const int DATETIME_DATA_NOTHING = 0;

        //マガジン段数記録情報開始アドレス
        private const string MAG_STEP_DATA_START_ADDR = "ZR000028";

        /// <summary>
        /// ローダー側マガジンNo読取完了アドレス
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }

		/// <summary>
		/// ローダー側読み取り済みDM格納アドレス
		/// </summary>
		public string LoaderQRAddress { get; set; }

		/// <summary>
		/// アンローダー側読み取り済みDM格納アドレス
		/// </summary>
		public string UnloaderQRAddress { get; set; }

		/// <summary>
		/// 開始登録OKBit
		/// </summary>
		public string WorkStartOKBitAddress { get; set; }

		/// <summary>
		/// 開始登録NGBit
		/// </summary>
		public string WorkStartNGBitAddress { get; set; }

		/// <summary>
		/// 完了登録OKBit
		/// </summary>
		public string RecvCompleteBitAddress { get; set; }

		/// <summary>
		/// マガジンの段毎の完了日時が格納されるアドレス群の開始アドレス
		/// </summary>
		public string WorkCompleteDtStartAddress { get; set; }

		/// <summary>
		/// マガジンの段毎のDMが格納されるアドレス群の開始アドレス
		/// </summary>
		public string UnloaderDmStartAddress { get; set; }

		/// <summary>
		/// マガジンの段毎の完了日時が格納されるアドレス群のアドレスの間隔
		/// </summary>
		public int WorkCompleteDtAddressInterval { get; set; }

		/// <summary>
		/// マガジンの段毎のDMが格納されるアドレス群のアドレスの間隔
		/// </summary>
		public int DmAddressInterval { get; set; }

		#region 装置ログ関連

		private const int DATA_NO_COL = 0;
		private const int MAG_NO_COL = 3;

        #endregion


        #region データ記録用プロパティ

        private string DataMatrix { get; set; }
        private DateTime? WorkCompleteDt { get; set; }
        private int Step { get; set; }

        #endregion

        protected override void concreteThreadWork()
        {
			// 作業完了登録
			if (this.IsRequireOutput() == true)
            {
                workComplete();
            }

			// 作業開始登録
            if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == Mitsubishi.BIT_ON)
            {
				workStart();
			}

			// Nasca不良ファイル取り込み
			//Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);
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

		public void workStart()
		{
			VirtualMag mag = new VirtualMag();
			string lotno;
			string dm = Plc.GetWord(LoaderQRAddress, CARRIER_WORD_LEN);
			if (string.IsNullOrWhiteSpace(dm) == false)
			{
				string[] lotNoArray = ArmsApi.Model.LotCarrier.GetLotNo(dm, null, true, true);

				if (lotNoArray.Length > 1)
				{
					throw new ApplicationException("[開始登録異常] ウェットブラスト装置搬入ロットNOの取得に失敗。(読み込んだDMに紐つくロットが複数存在します。)\nウェットブラスト装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。 DM:" + dm);
				}

				lotno = lotNoArray[0];
			}
			else
			{
				throw new ApplicationException("[開始登録異常] ウェットブラスト装置搬入マガジンNOの取得に失敗。\nウェットブラスト装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
			}

			Magazine[] magArray = Magazine.GetMagazine(lotno, true);

			if(magArray.Length != 1)
			{
				throw new ApplicationException("[開始登録異常] マガジン情報が1件に絞れません。ロット:" + lotno);
			}

			string magno = magArray[0].MagazineNo;
			Magazine svrmag = magArray[0];

			AsmLot svrlot = AsmLot.GetAsmLot(lotno);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

			mag.MagazineNo = magno;
			mag.ProcNo = nextproc.ProcNo;

			List<Order> orderList = ArmsApi.Model.Order.SearchOrder(lotno, mag.ProcNo, null, true, false).ToList();

			if(orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
			{
				Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
				throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
					string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
			}
	
			mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);

			Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);
			order.LotNo = svrlot.NascaLotNo;
            order.TranStartEmpCd = "660";

            if (orderList.Exists(o => o.MacNo == this.MacNo))
			{
				string errMsg;
				MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
				bool isError = WorkChecker.IsErrorBeforeStartWork(svrlot, machine, order, nextproc, out errMsg);
				if (isError)
				{
					Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
					Log.ApiLog.Info(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, errMsg));
				}
				else
				{
					Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
				}
				return;
			}
			OutputSysLog(string.Format("[開始処理] 開始 MagazineNo:{0}", magno));

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

        public void workComplete()
        {
			string lotno;
            VirtualMag mag = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string dm = getDataMatrix(MAX_MAG_STEP_CT);
            List<WetBlaster> dmList = getDataMatrixList(MAX_MAG_STEP_CT, DmAddressInterval);

            if(dmList.Count()> 0)
            {
                string[] lotNoArray = ArmsApi.Model.LotCarrier.GetLotNo(dmList[0].DataMatrix, null, true, true);

                if (lotNoArray.Length > 1)
                {
                    throw new ApplicationException("[完了登録異常] ウェットブラスト装置搬入ロットNOの取得に失敗。(読み込んだDMに紐つくロットが複数存在します。)\nウェットブラスト装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。 DM:" + dm);
                }
                lotno = lotNoArray[0];
            }
            else
            {
                throw new ApplicationException("[完了登録異常] ウェットブラスト装置搬入マガジンNOの取得に失敗。\nウェットブラスト装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
            }

			Magazine[] magArray = Magazine.GetMagazine(lotno, true);

			if (magArray.Length != 1)
			{
				throw new ApplicationException("[完了登録異常] マガジン情報が1件に絞れません。ロット:" + lotno + "/取得マガジン(カンマ区切り):" + string.Join(",", magArray.Select(m => m.MagazineNo)));
			}

			string magno = magArray[0].MagazineNo;
			Magazine svrmag = magArray[0];

			OutputSysLog(string.Format("[完了処理] 開始 MagazineNo:{0}/DM:{1}", magno, dm));

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
            mag.ProcNo = nextproc.ProcNo;
			mag.MagazineNo = magno;
			mag.LastMagazineNo = magno;

			Order startOrder = Order.GetMachineOrder(this.MacNo, svrmag.NascaLotNO);
			if (startOrder == null)
			{
				throw new ApplicationException(string.Format("作業の開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。LotNo:{0}", svrmag.NascaLotNO));
			}
			mag.WorkStart = startOrder.WorkStartDt;

            
            //既にキュー内に存在するかを確認
            bool found = false;

			VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader));

			foreach (VirtualMag exist in mags)
            {
                if (exist.MagazineNo == magno)
                {
                    found = true;
                    mag.WorkComplete = exist.WorkComplete;
                }
            }

            if (found == false)
            {
                List<WetBlaster> compDtList = dmList.Where(d => d.WorkCompleteDt != null).OrderByDescending(d => d.WorkCompleteDt).ToList();
                if(compDtList.Count() == 0)
                {
                    throw new ApplicationException("装置から完了日時が取得できませんでした。1件以上の完了日時を取得出来る必要があります。");
                }
                mag.WorkComplete = compDtList[0].WorkCompleteDt;
            }

            if(renameLotFiles(svrmag, mag) == false)
            {
                Log.SysLog.Info($"WetBlaster装置 傾向管理ファイルが見つからないため完了処理スキップ:{this.MacNo}");
                return;
            }

            Log.SysLog.Info("WetBlaster装置 ファイルリネーム完了:" + this.MacNo);

            //既存キュー内に存在しない場合のみ、複数存在する完了時間から最も新しい時間でEnqueue
            if (found == false)
            {
                try
                {
					//作業完了時間取得
					if (mag.WorkComplete.HasValue == false)
					{
						Log.ApiLog.Info("ウェットブラスト装置排出位置のマガジンに完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
						return;
					}
                }
                catch(Exception err)
                {
                    Log.ApiLog.Info("ウェットブラスト装置排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo + "\n例外:" + err.ToString());
                    return;
                }
				
                if (svrmag == null) throw new ApplicationException("ウェットブラスト装置マガジンデータ異常　現在稼働中マガジンがありません:" + svrmag);                

                //完了登録にUnloaderマガジンが必要なので先に作成
                this.Enqueue(mag, Station.Unloader);

				//下記はInspector2からコピーした為、記述があるがウェットブラストでは不要なはずなのでコメントアウト
				//NTSV検証完了後に削除
                //高効率でArmsWebが作成してしまうので削除
                //this.Dequeue(Station.Loader);
            }

            //段数情報を記録
            RegisterCarrierData(dmList, mag, svrlot);


            Plc.SetBit(this.RecvCompleteBitAddress, 1, Mitsubishi.BIT_ON);
			OutputSysLog(string.Format("[完了処理] 完了 MagazineNo:{0}/DM:{1}", magno, dm)); 

        }

		private DateTime getLatestWorkCompleteDt(int maxMgStepCt)
		{
			List<DateTime> workCompDtList = new List<DateTime>();

			string workCompleteDtAddress = WorkCompleteDtStartAddress;

			for (int stepCt = 0; stepCt < maxMgStepCt; stepCt++)
			{
				if(Plc.GetWordAsDecimalData(workCompleteDtAddress) == DATETIME_DATA_NOTHING)
				{
                    workCompleteDtAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(workCompleteDtAddress, WorkCompleteDtAddressInterval);
					continue;
				}

				workCompDtList.Add(Plc.GetWordsAsDateTime(workCompleteDtAddress));
                workCompleteDtAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(workCompleteDtAddress, WorkCompleteDtAddressInterval);


            }

            if (workCompDtList.Count == 0)
			{
				throw new ApplicationException("装置から完了日時が取得できませんでした。1件以上の完了日時を取得出来る必要があります。");
			}

			return workCompDtList.OrderByDescending(w => w).First();
		}

		private string getDataMatrix(int maxMgStepCt)
		{
			string retv = string.Empty;

			string dmAddress = UnloaderDmStartAddress;

			for (int stepCt = 0; stepCt < maxMgStepCt; stepCt++)
			{
				retv = Plc.GetWord(dmAddress, CARRIER_WORD_LEN).Replace("\0","");

				dmAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(dmAddress, DmAddressInterval);

				if (string.IsNullOrWhiteSpace(retv) == false)
				{
					return retv;
				}
			}

			return retv;
		}

        private List<WetBlaster> getDataMatrixList(int maxMgStepCt, int dataDistance)
        {
            List<WetBlaster> retv = new List<WetBlaster>();

            for (int stepCt = 0; stepCt < maxMgStepCt; stepCt++)
            {
                // 基板DMを取得して0ならcontinue
                string dmAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(UnloaderDmStartAddress, dataDistance * stepCt);
                string dataMatrix = this.Plc.GetWord(dmAddress, CARRIER_WORD_LEN).Replace("\0","");

                if (string.IsNullOrWhiteSpace(dataMatrix) == true || dataMatrix == "0")
                {
                    continue;
                }

                WetBlaster temp = new WetBlaster();
                temp.DataMatrix = dataMatrix;

                //完了・段数情報取得

                string completeDtAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(WorkCompleteDtStartAddress, dataDistance * stepCt);
                if (Plc.GetWordAsDecimalData(completeDtAddress) == DATETIME_DATA_NOTHING)
                {
                    temp.WorkCompleteDt = null;
                }
                else
                {
                    temp.WorkCompleteDt = this.Plc.GetWordsAsDateTime(completeDtAddress);
                }

                string stepAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(MAG_STEP_DATA_START_ADDR, dataDistance * stepCt);
                temp.Step = stepCt + 1;
                    
                retv.Add(temp);
            }
            return retv;
        }

        public void CheckMachineLogFile()
		{
			List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
			foreach (string logFile in logFiles)
			{
				if (MachineLog.IsLotFromFileName(logFile)) continue;

				if (IsStartLogFile(logFile) == false)
				{
					MachineLog mLog = parseMachineLog(logFile);
					if (mLog.IsUnknownData)
					{
						MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
						return;
					}

					Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
					if (svrMag == null)
						throw new ApplicationException(
							string.Format("装置ログ内のマガジンNo:{0}の稼働中マガジンが存在しません。", mLog.MagazineNo));

					AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
					int procNo = Process.GetNowProcess(lot).ProcNo;

					MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
				}
			}
		}

		public bool IsStartLogFile(string fullName)
		{
			string fileName = Path.GetFileNameWithoutExtension(fullName);
			if (Regex.IsMatch(fileName, "^log..._OA.*$"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 装置ログファイルの内容を解析
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="magno"></param>
		/// <param name="isContains1"></param>
		/// <param name="isContains3"></param>
		private MachineLog parseMachineLog(string filepath, int retryCt)
		{
			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
			{
				throw new ApplicationException("ファイル解析中にエラーが発生しました：" + filepath);
			}

			MachineLog mLog = new MachineLog();
			string magNo = null;

			try
			{
				using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
				{
					int lineno = 0;
					while (sr.Peek() > 0)
					{
						string line = sr.ReadLine();

						line = line.Replace("\"", "");

						if (string.IsNullOrEmpty(line)) continue;

						//CSV各要素分解
						string[] data = line.Split(',');

						//DATA_NOが数字変換できない行は飛ばす
						int datano;
						if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

						string[] magStr = data[MAG_NO_COL].Split(' ');
						if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
						{
							//自動化用の30_を取り除き
							magNo = magStr[1];
						}
						else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
						{
							//高効率用の11_を取り除き
							magNo = magStr[1];
						}
						else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
						{
							//高効率用の13_を取り除き
							magNo = magStr[1];
						}
						else
						{
							magNo = data[MAG_NO_COL];
						}

						lineno = lineno + 1;
					}

					if (lineno == 0)
					{
						// データがヘッダのみを想定
						mLog.IsUnknownData = true;
						return mLog;
					}

					if (magNo == null) throw new ApplicationException("装置ログにマガジン番号が存在しません:" + filepath);
				}

				mLog.MagazineNo = magNo;
				return mLog;
			}
			catch (IOException)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return parseMachineLog(filepath, retryCt);
			}
		}

		private MachineLog parseMachineLog(string filepath)
		{
			return parseMachineLog(filepath, 0);
		}

        /// <summary>
        /// EICSが作成したファイル名のリネーム
        /// </summary>
        private bool renameLotFiles(Magazine svmag, VirtualMag mag)
        {
            AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                return false;
            }

            Order order = orders.FirstOrDefault();

            List<string> lotFilestemp = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, mag.WorkComplete.Value);
            
            if (lotFilestemp.Count == 0)
            {
                return false;
            }

            List<string> lotFiles = new List<string>();

            foreach (string file in lotFilestemp)
            {
                if(lotFilestemp.Contains(".OK") == true || lotFilestemp.Contains(".NG") == true)
                {
                    continue;
                }
                lotFiles.Add(file);
            }

            if (lotFiles.Count == 0)
            {
                return false;
            }

            foreach (string lotFile in lotFiles)
            {

                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                //リネイム済みは除外
                if (dataMatrix.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                {
                    continue;
                }

                MachineLog.ChangeFileName(lotFile, svmag.NascaLotNO, lot.TypeCd, order.ProcNo, svmag.MagazineNo);
                OutputSysLog($"[完了処理] ファイル名称変更 FileName:{lotFile}");
            }

            return true;
        }

        private void RegisterCarrierData(List<WetBlaster> dmList, VirtualMag mag, AsmLot lot)
        {
            //int loadStepCd = ArmsApi.Model.LENS.Mag.GetMagStepCd(lot.TypeCd).Value;

            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = lot.NascaLotNo;
            regData.Delfg = 0;
            regData.ProcNo = Convert.ToInt32(mag.ProcNo);
            regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach (WetBlaster data in dmList)
            {
                regData.CarrierNo = data.DataMatrix;

                int step = data.Step;

                //装置によって積み方が違ってマスタで対応できないので無しに変更。（レポーター側で対応する）
                ////段数情報はPLCから抜く場合元々搭載しない段もカウントしてしまうので、それを除外して集計する。
                ////loadStepCdは1が偶数段のみ、2が奇数段のみ
                //if (loadStepCd == 1)
                //{
                //    step = step / 2;
                //}
                //else if (loadStepCd == 2)
                //{
                //    step = (step + 1) / 2;
                //}
                //else if (loadStepCd != 3)
                //{
                //    continue;
                //}

                //段数登録
                regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
                regData.Value = Convert.ToString(step);
                regData.InsertUpdate();                
            }
        }
    }
}
