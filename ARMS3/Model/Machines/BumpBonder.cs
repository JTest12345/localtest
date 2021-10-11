using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// SLS1 バンプボンダー
	/// 作業完了処理のトリガはMPファイル出力済
	/// </summary>
	public class BumpBonder : MachineBase
	{
		public const string FINISHEDFILE_IDENTITYNAME = "MP";
        private const string MAPPING_FIN_EXT = "fin";

		protected override void concreteThreadWork()
		{
			try
			{
				if (this.IsRequireOutput() == true)
				{
					workCompletehigh();
				}

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);

				RenameRelatedMappingFile(this.LogOutputDirectoryPath);
			}
			catch (Exception ex)
			{
				FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
				frmErr.ShowDialog();

				if (frmErr.Method == ErrorHandleMethod.None)
				{
					throw;
				}
			}
		}

		private void workCompletehigh()
		{
			VirtualMag ulMagazine = this.Peek(Station.Unloader);
			if (ulMagazine != null)
			{
				return;
			}

			VirtualMag lMag = this.Peek(Station.Loader);
			if (lMag == null)
			{
				return;
			}

			if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME) == false)
			{
				throw new ApplicationException(string.Format("排出信号ONで{0}ファイルが存在しません。 DirectoryName:{1}", FINISHEDFILE_IDENTITYNAME, this.LogOutputDirectoryPath));
			}

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

			if (lMag.WorkComplete.HasValue == false)
			{
				lMag.WorkComplete = System.DateTime.Now;
				lMag.Updatequeue();
			}

			Order order = Order.GetMachineStartOrder(this.MacNo);
			if (order == null)
			{
				throw new ApplicationException(string.Format("装置の開始実績が存在しません。 LoaderMagazineNo:{0}", lMag.MagazineNo));
			}
			AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			List<string> lotFiles = GetLotFiles(this.LogOutputDirectoryPath, lMag.WorkStart.Value, lMag.WorkComplete.Value);

			foreach (string lotFile in lotFiles)
			{
				string fileExtension = System.IO.Path.GetExtension(lotFile).ToLower();
				
				if (fileExtension == ".mpd")
				{
					continue;
				}

				MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, lMag.MagazineNo);
				OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
			}

			lMag.LastMagazineNo = lMag.MagazineNo;

			if (this.Enqueue(lMag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);
				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
			}
		}

		/// <summary>
		/// 1ロットの全ログファイルを取得
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		public static List<string> GetLotFiles(string directoryPath, DateTime startDt, DateTime endDt)
		{
			List<string> retv = new List<string>();

			List<string> files = MachineLog.GetFiles(directoryPath);
			foreach (string file in files)
			{
				if (File.Exists(file) == false) 
				{
					if (File.Exists(string.Format("_{0}", file)))
					{
						throw new ApplicationException(
							string.Format("装置パラメータのチェックが終わっていないか、存在しないファイルを参照しようとしてます。 ファイルパス:{0}", file));
					}
				}

				string fileExt = Path.GetExtension(file);
				string fileName = Path.GetFileNameWithoutExtension(file);

				if (fileExt != ".mpd" && fileExt != ".fin")
				{
					DateTime outputDate;
					string date = fileName.Substring(6, 2) + "/" + fileName.Substring(8, 2) + "/" + fileName.Substring(10, 2) + " " +
						fileName.Substring(12, 2) + ":" + fileName.Substring(14, 2) + ":" + fileName.Substring(16, 2);
					if (DateTime.TryParse(date, out outputDate) == false)
					{
						throw new ApplicationException(string.Format("日付型に変換できないファイル名が付いています ファイルパス:{0}", fileName));
					}

					if (outputDate >= startDt && outputDate <= endDt)
					{
						retv.Add(file);
					}
				}
			}

			return retv;
		}

		public void RenameRelatedMappingFile(string targetDirPath)
		{
			List<string> relatedMappingFiles = MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]" + MAPPING_FIN_EXT);
			relatedMappingFiles.AddRange(MachineLog.GetFilesByBackwardMatching(targetDirPath, "[.]mpd"));

			//mpdファイル名からアンダーバー以降削除するようなリネームが必要なければList中のmpdファイルの順番は気にする必要ないが
			//mpdもリネームする為、finファイルよりも先に、見つけた全mpdファイルのリネームを行う SLP2対応:n.yoshi
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

            List<MachineLog.FinishedFile5> finList = MachineLog.FinishedFile5.GetAllFiles(targetDirPath);

			if (finList.Count == 0)
			{
				return;
			}

			foreach (MachineLog.FinishedFile5 fin in finList)
			{
				OutputSysLog(string.Format("[基板完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

				AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
				OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				//工程取得
				int procno = Process.GetNowProcess(lot).ProcNo;
				OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

				//finファイル名をリネーム
				//基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
				MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.DataMatrix);
				string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.DataMatrix, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
				OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

				OutputSysLog(string.Format("[基板完了処理] 完了"));
			}
		}
	}
}
