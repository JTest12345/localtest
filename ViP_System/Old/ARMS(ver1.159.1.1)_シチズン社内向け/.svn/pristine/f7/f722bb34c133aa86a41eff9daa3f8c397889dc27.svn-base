using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ARMS3.Model.Machines
{
	public abstract class CifsMachineBase : MachineBase
	{
		private const string MAPPING_FIN_EXT = "fin2";

		//public string LogOutputDirectoryPath { get; set; }

		//public string LogInputDirectoryPath { get; set; }

		public void SendStopFile(string directoryPath, string reason)
		{
			sendFile(directoryPath, string.Empty, "stop", reason);
		}

		public void SendOkFile(string directoryPath) 
		{
            sendFile(directoryPath, string.Empty, "ok", "");
		}

        public void SendOkFile(string directoryPath, string fileName)
        {
            sendFile(directoryPath, fileName, "ok", "");
        }

		public void SendNgFile(string directoryPath) 
		{
            sendFile(directoryPath, string.Empty, "ng", "");
		}

        public void SendNgFile(string directoryPath, string fileName)
        {
            sendFile(directoryPath, fileName, "ng", "");
        }

        public void SendNgFile(string directoryPath, string fileName, string content)
        {
            sendFile(directoryPath, fileName, "ng", content);
        }

        private void sendFile(string directoryPath, string fileName, string instructionChar, string content)
		{
			if (Directory.Exists(directoryPath) == false) 
			{
				Directory.CreateDirectory(directoryPath);
			}

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            fileName = string.Format("{0}.{1}", fileName, instructionChar);

			StreamWriter sw = new StreamWriter(Path.Combine(directoryPath, fileName), true, Encoding.UTF8);
			sw.WriteLine(content);
			sw.Close();
		}
        
        /// <summary>
        /// 作業開始(trgファイル名をリネーム)  ※TmProMacにMacNoで問い合わせた時、複数ProcNoがヒットするような装置は使用不可
        /// </summary>
        public void WorkStart()
        {
            List<MachineLog.TriggerFile> trgList = MachineLog.TriggerFile.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.TriggerFile trg in trgList)
            {
                OutputSysLog(string.Format("[開始処理] 開始 trgファイル取得成功 FileName:{0}", trg.FullName));

                AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
                OutputSysLog(string.Format("[開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[開始処理] 工程取得成功 ProcNo:{0}", procno));

                //trgファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(trg.FullName, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo, trg.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", trg.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", trg.FullName, fileName));

                OutputSysLog(string.Format("[開始処理] 完了"));
            }
        }

        /// <summary>
        /// 実績登録
        /// </summary>
        public Order EntryOrder(AsmLot lot, int procno, string magno, DateTime? workStartDt_Fin, DateTime? workEndDt_Fin, Material[] frameList)
        {
            Process p = Process.GetProcess(procno);
            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

            Order order = new Order();
            Order[] magOrdList = Order.GetOrder(lot.NascaLotNo, procno);
            if (magOrdList.Length != 0)
            {
                order = magOrdList[0];
            }
            else
            {
                order.LotNo = lot.NascaLotNo;
                order.ProcNo = procno;
                order.InMagazineNo = magno;
                order.OutMagazineNo = magno;
                order.MacNo = this.MacNo;
                order.TranStartEmpCd = "660";
                order.TranCompEmpCd = "660";
                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }
                order.IsAutoImport = false;
                order.WorkStartDt = DateTime.MaxValue;
                order.WorkEndDt = null;
            }

            DateTime? workStartDt;
            DateTime? workEndDt;
            if (isReflect(order, workStartDt_Fin, workEndDt_Fin, out workStartDt, out workEndDt))
            {
                order.WorkStartDt = workStartDt.Value;
                order.WorkEndDt = workEndDt;
                order.DeleteInsert(order.LotNo);
            }

            //原材料関連付け
            if (frameList != null && frameList.Count() != 0)
            {
                order.UpdateMaterialRelation(frameList);
            }

            return order;
        }

        /// <summary>
        /// 反映するかチェック
        /// </summary>
        /// <param name="order"></param>
        /// <param name="workStartDt_Fin"></param>
        /// <param name="workEndDt_Fin"></param>
        /// <param name="workStartDt"></param>
        /// <param name="workEndDt"></param>
        /// <returns></returns>
        private bool isReflect(Order order, DateTime? workStartDt_Fin, DateTime? workEndDt_Fin, out DateTime? workStartDt, out DateTime? workEndDt)
        {
            workStartDt = null;
            workEndDt = null;

            if (workStartDt_Fin.HasValue)
            {
                workStartDt = workStartDt_Fin.Value;
            }
            if (workEndDt_Fin.HasValue)
            {
                workEndDt = workEndDt_Fin.Value;
            }

            if (!workStartDt_Fin.HasValue ||
                (workStartDt_Fin.HasValue && order.WorkStartDt < workStartDt_Fin.Value))
            {
                workStartDt = order.WorkStartDt;
            }
            if ((order.WorkEndDt.HasValue && !workEndDt_Fin.HasValue) ||
                (order.WorkEndDt.HasValue && workEndDt_Fin.HasValue && order.WorkEndDt.Value > workEndDt_Fin.Value))
            {
                workEndDt = order.WorkEndDt.Value;
            }

            if (workStartDt != workStartDt_Fin &&
                workEndDt != workEndDt_Fin)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 作業完了(fin2ファイル名をリネーム)
        /// </summary>
        public void WorkComplete()
        {
            List<MachineLog.FinishedFile6> finList = MachineLog.FinishedFile6.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile6 fin in finList)
            {
                OutputSysLog(string.Format("[完了処理] 開始 fin2ファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //fin2ファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                OutputSysLog(string.Format("[完了処理] 完了"));
            }
        }

		public void RenameRelatedMappingFile(string targetDirPath)
		{
			List<string> relatedMappingFiles = MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]" + MAPPING_FIN_EXT);
			relatedMappingFiles.AddRange(MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]mpd"));

			//mpdファイル名からアンダーバー以降削除するようなリネームが必要なければList中のmpdファイルの順番は気にする必要ないが
			//mpdもリネームする為、finファイルよりも先に、見つけた全mpdファイルのリネームを行う SLP2対応
			List<string> mpdFirstSortFiles = new List<string>();
			mpdFirstSortFiles.AddRange(relatedMappingFiles.FindAll(l => l.EndsWith(".mpd")));

			foreach (string filePath in mpdFirstSortFiles)
			{
				string renamedFileNm = MachineLog.RemoveFileNmAfterUnderBar(filePath);

				if (string.IsNullOrEmpty(renamedFileNm))
				{
					continue;
				}
			}

			List<MachineLog.FinishedFile6> finList = MachineLog.FinishedFile6.GetAllFiles(targetDirPath);

			if (finList.Count == 0)
			{
				return;
			}

			foreach (MachineLog.FinishedFile6 fin in finList)
			{
				OutputSysLog(string.Format("[基板完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

				AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
				OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				//工程取得
				int procno = Process.GetNowProcess(lot).ProcNo;
				OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

				//finファイル名をリネーム
				//基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
				MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
				string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
				OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

				OutputSysLog(string.Format("[基板完了処理] 完了"));
			}
		}
	}
}
