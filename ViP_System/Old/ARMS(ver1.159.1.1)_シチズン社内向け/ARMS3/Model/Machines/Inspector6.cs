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
    /// 検査機(NTSV TS洗浄後検査機)
    /// </summary>
    public class Inspector6 : Inspector2
    {

        /// <summary>
        /// 開始登録NG時の種類書込み ※今のところ1固定で書込み
        /// </summary>
        public string WorkStartNGKindAddress { get; set; }

        /// <summary>
        /// 完了登録NGBit
        /// </summary>
        public string WorkCompleteOKBitAddress { get; set; }

        /// <summary>
        /// 完了登録NG時の種類書込み ※今のところ1固定で書込み
        /// </summary>
        public string WorkCompleteNGKindAddress { get; set; }

        //PEとの調整でInpector2の処理と同一処理に収束したのでコメントアウト。一応一度コメントアウト状態でアップロードしておく。
        //次の改修時に削除すること。
  //      public override void workStart()
		//{
		//	VirtualMag mag = new VirtualMag();

		//	string magno = Plc.GetMagazineNo(LMagazineAddress);
		//	if (string.IsNullOrEmpty(magno) == false)
		//	{
		//		mag.MagazineNo = magno;
		//		mag.LastMagazineNo = magno;
		//	}
		//	else
		//	{
		//		throw new ApplicationException("[開始登録異常] 検査機搬入マガジンNOの取得に失敗。\n検査機搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
		//	}
		//	Magazine svrmag = Magazine.GetCurrent(magno);
		//	if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

		//	OutputSysLog(string.Format("[開始処理] 開始 LoaderMagazineNo:{0}", magno));
            
		//	AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
		//	Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

		//	mag.ProcNo = nextproc.ProcNo;
		//	mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);

  //          //同ロットで他装置での開始実績が有ると開始処理エラーにする。
  //          List<Order> orderList = ArmsApi.Model.Order.SearchOrder(svrlot.NascaLotNo, nextproc.ProcNo, null, true, false).ToList();
  //          if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
  //          {
  //              throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
  //                  string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
  //          }

  //          //登録済みレコードが無い場合のみ開始登録する。
  //          if(orderList.Any() == false)
  //          {
  //              Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);
  //              ArmsApiResponse workResponse = CommonApi.WorkStart(order);

  //              if (workResponse.IsError)
  //              {
  //                  Plc.SetWordAsDecimalData(this.WorkStartNGKindAddress, 1); //NG種類は今のところ1固定(装置側でも特に使用していないが、一応送る必要がある)
  //                  Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
  //                  Log.ApiLog.Info(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
  //              }
  //              else
  //              {
  //                  Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
  //                  OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", mag.MagazineNo));
  //              }
  //          }
  //          else
  //          {
  //              Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
  //              OutputSysLog(string.Format("[開始処理] 開始実績有りのため開始登録スキップ LoaderMagazineNo:{0}", mag.MagazineNo));
  //          }          
		//}

  //      public override void workComplete()
  //      {
  //          try
  //          {
  //              VirtualMag mag = new VirtualMag();

  //              VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader));

  //              //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
  //              string newmagno = Plc.GetMagazineNo(ULMagazineAddress, true);

  //              if (string.IsNullOrEmpty(newmagno) == false)
  //              {
  //                  mag.MagazineNo = newmagno;
  //                  mag.LastMagazineNo = newmagno;
  //              }
  //              else
  //              {
  //                  throw new ApplicationException("[完了登録異常] 検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
  //              }
  //              Magazine svrmag = Magazine.GetCurrent(newmagno);
  //              if (svrmag == null) throw new ApplicationException("[完了登録異常] マガジン情報が見つかりません" + newmagno);

  //              OutputSysLog(string.Format("[完了処理] 開始 UnLoaderMagazineNo:{0}", newmagno));

  //              AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
  //              Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
  //              mag.ProcNo = nextproc.ProcNo;

  //              //既にキュー内に存在するかを確認
  //              bool found = false;
  //              foreach (VirtualMag exist in mags)
  //              {
  //                  if (exist.MagazineNo == mag.MagazineNo)
  //                  {
  //                      found = true;
  //                  }
  //              }

  //              //既存キュー内に存在しない場合のみ検査結果をまってEnqueue
  //              if (found == false)
  //              {
  //                  try
  //                  {
  //                      //作業開始完了時間取得
  //                      mag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
  //                      if (mag.WorkComplete.HasValue == false)
  //                      {
  //                          Log.ApiLog.Info("検査機排出位置のマガジンに完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
  //                          return;
  //                      }

  //                      if (IsWorkStartAutoComplete)
  //                      {
  //                          Order startOrder = Order.GetMachineOrder(this.MacNo, svrmag.NascaLotNO);
  //                          if (startOrder == null)
  //                          {
  //                              throw new ApplicationException(string.Format("作業の開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。LotNo:{0}", svrmag.NascaLotNO));
  //                          }
  //                          mag.WorkStart = startOrder.WorkStartDt;
  //                      }
  //                      else
  //                      {
  //                          mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
  //                          if (mag.WorkStart.HasValue == false)
  //                          {
  //                              Log.ApiLog.Info("検査機排出位置のマガジンに開始時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
  //                              return;
  //                          }
  //                      }
  //                  }
  //                  catch
  //                  {
  //                      Log.ApiLog.Info("検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください" + mag.MagazineNo);
  //                      return;
  //                  }

  //                  //現在完了工程で削除フラグONかつ規制理由「」が存在する場合、毎回規制を復活させる
  //                  if (svrmag == null) throw new ApplicationException("検査機マガジンデータ異常　現在稼働中マガジンがありません:" + svrmag);
  //                  Restrict[] reslist = Restrict.SearchRestrict(svrmag.NascaLotNO, nextproc.ProcNo, false);
  //                  foreach (Restrict res in reslist)
  //                  {
  //                      //周辺強度による規制理由と同じか確認
  //                      if (res.Reason == WireBonder.MMFile.RESTRICT_REASON_2)
  //                      {
  //                          //規制を有効化
  //                          res.DelFg = false;
  //                          res.Save();
  //                      }
  //                  }

  //                  //完了登録にUnloaderマガジンが必要なので先に作成
  //                  this.Enqueue(mag, Station.Unloader);

  //                  //高効率でArmsWebが作成してしまうので削除
  //                  this.Dequeue(Station.Loader);

  //                  if (this.IsWorkEndAutoComplete)
  //                  {
  //                      Order order = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
  //                      ArmsApiResponse workResponse = CommonApi.WorkEnd(order);

  //                      this.Dequeue(Station.Unloader);

  //                      if (workResponse.IsError)
  //                      {
  //                          //完了時処理NGについては装置にはNGは送らない。（例外エラー時のみ）
  //                          OutputSysLog(string.Format("完了処理失敗：マガジンNo= {0},  理由= {1}", mag.MagazineNo, workResponse.Message));
  //                      }
  //                  }
  //              }

  //              Plc.SetBit(this.WorkCompleteOKBitAddress, 1, Mitsubishi.BIT_ON);
  //              OutputSysLog(string.Format("[完了処理] 完了 UnLoaderMagazineNo:{0}", newmagno));
  //          }
  //          catch (Exception ex)
  //          {
  //              Plc.SetWordAsDecimalData(this.WorkCompleteNGKindAddress, 1);//NG種類は今のところ1固定(装置側でも特に使用していないが、一応送る必要がある)
  //              Plc.SetBit(this.WorkCompleteNGBitAddress, 1, Mitsubishi.BIT_ON);
  //              Log.ApiLog.Info($"[完了登録異常] 装置:{this.MacNo} 理由:{ex.Message} StackTrace:{ex.StackTrace}");
  //          }
  //      }

        //従来の同名関数は完了時ファイルのみリネームするが、こちらはOAファイルだけリネームする。
        //EICSにOAファイルを退避してもらうためだけの処理。
        public override void CheckMachineLogFile()
        {
            List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);

            //OAファイルのみ対象にリネームする。※この装置はOAファイルしか出ないが、念のため。
            logFiles = logFiles.Where(l => l.Contains("_OA") == true).ToList();

            foreach (string logFile in logFiles)
            {
                if (MachineLog.IsLotFromFileName(logFile)) continue;

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

                //OAファイルは完了時に出力されるらしいがこの関数の呼び出しは完了処理とは別処理なので
                //タイミングによってprocNoが異なってしまう可能性があるが、この後のファイル処理(バックアップだけ)に影響はないので気にしない。
                int procNo = svrMag.NowCompProcess;

                MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
            }
        }
    }
}
