using LENS2_Api;
using PLC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LENS2.Machine
{
	/// <summary>
	/// ワイヤーボンダー
	/// </summary>
	public class Wirebonder : MachineBase, IMachine
	{
		public static string GetCompareMachineLogDirectoryPath()
		{
			return LENS2_Api.Config.Settings.ForWBCompareMachineLogDirPath;
		}

		public bool IsMappingMode
		{
			get
			{
				if (!string.IsNullOrEmpty(WatchingDirectoryPath))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		//共通化出来る処理は共通のルーチンへ!!!
		//外部システム・装置との連携部の処理にはきめ細かなログ出力を!!!

		public Wirebonder()	{}

		public void MainWork()
		{
			try
			{
				FileInfo mm = MachineLog.GetLatest(this.WatchingDirectoryPath, MMFile.FILE_KIND, MMFile.DATESTARTINDEX_FROM_MM_ON_FILENAME);
				if (mm == null)
				{
					return;
				}

				if (!MMFile.IsFileNameAddLot(mm))
				{
					// ARMS処理待ち(ファイル名にロット情報付与)
					return;
				}

				MMFile mmFile = new MMFile(mm, this);
				if (WorkResult.IsCompleteEndProcess(mmFile.LotNo, Config.Settings.WirebondProcNo))
				{
					// NAMI処理済み
					return;
				}

				InfoLog("[完了時] マッピング処理開始", mmFile.LotNo);
				mmFile.Run();

				// NAMI処理済フラグON(ARMS, EICS処理開始)
				WorkResult.CompleteEndProcess(mmFile.LotNo, Config.Settings.WirebondProcNo, this.NascaPlantCD);
				InfoLog("[完了時] マッピング処理完了", mmFile.LotNo);
			}
			catch(ApplicationException err)
			{
				throw new MachineException(this.ClassNM, this.MachineNM, err);
			}
		}

		public class MMFile : MachineLog
		{
			public const string FILE_KIND = "MM";
			public const int CONTENTS_HEADEREND_ROW = 2;

			public const int CONTENTS_DATANO_COL = 0;

			/// <summary>内容(マガジン番号)</summary>
			public const int CONTENTS_MAGAZINE_COL = 3;

			/// <summary>内容(ロギングアドレス)</summary>
			public const int CONTENTS_ADDRESS_COL = 4;

			/// <summary>内容(エラー番号) </summary>
			public const int CONTENTS_ERRORNO_COL = 5;
			
			/// <summary>内容(ユニット番号) </summary>
			public const int CONTENTS_UNITNO_COL = 6;

			/// <summary>内容(結果CD)</summary>
			public const int CONTENTS_RESULT_COL = 7;
			
			public const int DATESTARTINDEX_FROM_MM_ON_FILENAME = 6;

			/// <summary>
			/// 結果CD列のOK時出力文字列
			/// </summary>
			private const string CONTENTS_ERRORNO_OK = "0x0000";

			/// <summary>
			/// 通常の排出
			/// </summary>
			public const string RESTRICT_REASON = "WBエラーに対する検査の為";

			/// <summary>
			/// 周辺強度試験のための排出
			/// </summary>
			public const string RESTRICT_REASON_2 = "WBエラーに対する周辺強度実施の為";

			public FileInfo Info { get; set; }

			public string MagazineNo { get; set; }
			public string LotNo { get; set; }
			public string TypeCd { get; set; }
			public int ProcNo { get; set; }
			public IMachine WorkMachine { get; set; }
			public bool IsMapping { get; set; }

			public enum ResultCode : int
			{
				Inspection = 1,
				StrengthInspection = 2,
				SeeingInspection = 3,
			}

            /// <summary>
            /// スキップエラー数
            /// </summary>
            public int SkipCt { get; set; }

			public Dictionary<string, string> SkipErrorCodeList { get; set; }

			public Dictionary<string, string> BadMarkErrorCodeList { get; set; }

			/// <summary>
			/// 内容(このリストはボンディング打順に並んでいる事が必須)
			/// </summary>
            public List<Content> Contents { get; set; }
			public class Content
            {
                public int Address { get; set; }
                public string Unit { get; set; }
                public string ErrorCode { get; set; }

				/// <summary>
				///	内容そのまま
				/// </summary>
				public int ResultCode { get; set; }

				/// <summary>
				/// 内容に周辺検査(S)、検査不要(M)を設定
				/// </summary>
				public string MappingResultCode { get; set; }
            }

			/// <summary>
			/// 列別のパッケージ情報が入ったリスト
			/// </summary>
			public class ColumnData
			{
				// 列有無のフラグ
				// 仕様変更でフレーム飛ばして周辺検査付与しなくなってる為、このフラグを追加
				public bool ExistColumnFg { get; set; }

				// 列順
				public int ColumnNo { get; set; }

				// 列毎のパッケージ情報
				public List<Content> PackageList { get; set; }

				public bool IsFrameStart { get; set; }

				public bool IsFrameEnd { get; set; }
			}

			public MMFile(FileInfo mmFile, IMachine workMachine)
			{
				this.Info = mmFile;
				this.WorkMachine = workMachine;

				getLotFromFileName();

				this.SkipErrorCodeList = General.GetWirebonderErrorCodeSkip();
				this.BadMarkErrorCodeList = General.GetWirebonderErrorCodeBadMark();
				this.Contents = new List<Content>();
			}

			private void getLotFromFileName()
			{
				if (MMFile.IsFileNameAddLot(this.Info) == false)  
				{
					throw new ApplicationException(
						string.Format("MMファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
				}

				string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
				this.LotNo = nameChar[1].Trim();
				this.TypeCd = nameChar[2].Trim();
				this.ProcNo = int.Parse(nameChar[3].Trim());
			}

			public List<Content> GetData()
			{
				string[] fileValue = FileWrapper.ReadAllLines(this.Info.FullName);

				// ヘッダ部除去
				fileValue = fileValue.Skip(CONTENTS_HEADEREND_ROW).ToArray();
				if (fileValue.Count() == 0)
				{
					// ヘッダ部のみの空ファイルの場合、処理停止
					throw new ApplicationException(
						string.Format("空のMMファイルが読み込まれました。出力ファイルを確認して下さい。出力ファイル:{0}", Info.FullName));
				}
				List<Content> retv = new List<Content>();

				Magazine.Config magConfig = Magazine.Config.GetData(this.TypeCd);		
	 
				// ボンディング結果(1, 2, 3, 4)を内容(ResultCode, MappingResultCode)に設定
				retv = InitializeBondingList(retv, fileValue, magConfig.TotalMagazinePackage);

				//2014/10/2 同一アドレスでのエラー重複時の仕様確認  ユニットNo違いなどで複数回、同じアドレスが出現する場合

				// 周辺検査(S)を内容(MappingResultCode)に設定
				retv = getAroundInspectionData(retv, magConfig);

				return retv;
			}

			/// <summary>
			/// ボンディング結果(1, 2, 3, 4, 不足分)を内容(ResultCode, MappingResultCode)に設定
			/// Contentを扱う最初に呼び出す事。（MMファイル内容からマッピングアドレスの初期化を行う為）
			/// </summary>
			/// <param name="fileValue"></param>
			/// <param name="totalMagPackage"></param>
			private List<Content> InitializeBondingList(List<Content> retv, string[] fileValue, int totalMagPackage)
			{
			
				//2014/10/2 this.Contentsの初期化がimportBondingResultにあるのでその後の関数の呼び出し順が暗黙のうちに固定化されている点を修正
				//this.Contents = new List<Content>();

				Dictionary<string, string> resultCodeList = General.GetWirebonderResultCode();

				for (int i = 0; i < fileValue.Count(); i++)
				{
					if (string.IsNullOrWhiteSpace(fileValue[i]))
					{
						//空白行の場合、次へ
						continue;
					}

					// CSV各要素分解
					string[] data = fileValue[i].Split(',');

					// DATA_NOが数字変換できない行は飛ばす
					int datano;
					if (int.TryParse(data[CONTENTS_DATANO_COL], out datano) == false) continue;

					string unitno = data[CONTENTS_UNITNO_COL].Trim();

					int address;
					if (int.TryParse(data[CONTENTS_ADDRESS_COL].Trim(), out address) == false)
					{
						throw new ApplicationException(
							string.Format("想定外のアドレスがファイル内に含まれています。アドレス：{0} ファイルパス:{1}", data[CONTENTS_ADDRESS_COL].Trim(), this.Info.FullName));
					}

					if (address == 0) { continue; }

					string errcd = data[CONTENTS_ERRORNO_COL].Trim();
					int resultcd;
					if (int.TryParse(data[CONTENTS_RESULT_COL].Trim(), out resultcd) == false)
					{
						throw new ApplicationException(
							string.Format("数値変換出来ない処理CDがファイル内に含まれています。処理CD:{0} ファイルパス:{1}", resultcd, this.Info.FullName));
					}

					if (!resultCodeList.ContainsKey(resultcd.ToString()))
					{
						throw new ApplicationException(
							string.Format("想定外の処理CDがファイル内に含まれています。処理CD:{0} ファイルパス:{1}", resultcd, this.Info.FullName));
					}
				
					if (resultcd != 0) IsMapping = true;

					// 処理済みリストに追加
					retv.Add(new Content() { Address = address, Unit = unitno, ErrorCode = errcd, ResultCode = resultcd, MappingResultCode = resultcd.ToString() });
				}

				return retv;
			}

			public static Dictionary<int, string> ConvertToMold(Dictionary<int, string> original)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();

				Dictionary<string, string> conv = MapConv.GetData("WB_MM", "MD_IN");

				foreach (KeyValuePair<int, string> data in original)
				{
					retv.Add(data.Key, conv[data.Value]);
				}

				return retv;
			}

			/// <summary>
			/// 周辺検査(S)を内容(MappingResultCode)に設定
			/// </summary>
			/// <param name="magConfig"></param>
			/// <returns></returns>
			private List<Content> getAroundInspectionData(List<Content> contents, Magazine.Config magConfig)
			{
				if (magConfig.AroundInspectType == Magazine.Config.AroundInspectionType.Package)
				{
					for (int i = 0; i < contents.Count; i++)
					{
						if (contents[i].MappingResultCode != MappingFile.Data.Wirebonder.INSPECTION_POINT)
						{
							continue;
						}

						int replaceStartAddr = contents[i].Address;

						// 検査(1)の前何個(先で取得した周辺検査数)に"S"を代入  NotInspection,Inspection,Strength,Seeing優先
						int sInspCount = 0;
						for (int beforeIndex = i - 1; sInspCount < magConfig.FrameAroundInspectPackage; beforeIndex--)
						{
							if (beforeIndex < 0) { break; }

							//2014/10/2 強度試験、目視検査、検査なし、周辺検査でスキップされるコードになっているので仕様再確認
							string errorCd = contents[beforeIndex].ErrorCode;
							if (containtsBadMarkErrorCode(errorCd))
							{
								// 不良CDがバッドマークの場合、1つ飛ばす
								continue;
							}

							contents[beforeIndex].MappingResultCode
								= getHighPriorityResultCode(MappingFile.Data.Wirebonder.AROUNDINSPECTION_POINT, contents[beforeIndex].MappingResultCode);
							sInspCount++;
						}

						// 検査(1)の後何個(先で取得した周辺検査数)に"S"を代入  NotInspection,Inspection,Strength,Seeing優先
						sInspCount = 0;
						for (int afterIndex = i + 1; sInspCount < magConfig.FrameAroundInspectPackage; afterIndex++)
						{
							if (afterIndex + 1 > contents.Count) { break; }

							string errorCd = contents[afterIndex].ErrorCode;
							if (containtsBadMarkErrorCode(errorCd))
							{
								// 不良CDがバッドマークの場合、1つ飛ばす
								continue;
							}

							contents[afterIndex].MappingResultCode
								= getHighPriorityResultCode(MappingFile.Data.Wirebonder.AROUNDINSPECTION_POINT, contents[afterIndex].MappingResultCode);
							sInspCount++;
						}
					}
				}
				else 
				{
					//列アドレスとその列のマッピングデータリストを持ったデータ取得
					//不良アドレスの列を特定するだけで、不良発生列と前後指定列を上記の列単位で周辺検査付与の処理が出来る。
					//このデータ形式にする事でWBのボンディング順序に左右される事の無い仕組みに出来る 2015/1/24 h.ishiguchi n.yoshimoto
					List<MMFile.ColumnData> colDataList = GetColumnDataList(contents, magConfig);

					List<Content> tempContentsPackageList = new List<Content>();
					//周辺検査付与対象の不良パッケージ走査
					foreach (MMFile.ColumnData colData in colDataList)
					{
						foreach (Content c in colData.PackageList)
						{
							if (c.ResultCode != int.Parse(MappingFile.Data.Wirebonder.INSPECTION_POINT))
								continue;

							int currentErrCol = magConfig.GetColumnNO(c.Address);

							int targetIndex = currentErrCol - 1;
							colDataList[targetIndex].PackageList = GetUpdateByErrCd(colDataList[targetIndex].PackageList, MappingFile.Data.Wirebonder.AROUNDINSPECTION_POINT);

							colDataList = GetAroundInspectAddedData(magConfig, colDataList, currentErrCol, true);
							colDataList = GetAroundInspectAddedData(magConfig, colDataList, currentErrCol, false);
						}
						tempContentsPackageList.AddRange(colData.PackageList);
					}
					contents = tempContentsPackageList;
				}

				return contents;
			}

			private List<ColumnData> GetColumnDataList(List<Content> contentsPackageList, Magazine.Config magConfig)
			{
				List<ColumnData> colDataList = new List<ColumnData>();
				for (int i = 0; i < magConfig.TotalFrameColumnCount; i++)
				{
					MMFile.ColumnData data = new ColumnData();
					data.ColumnNo = i + 1;
					data.ExistColumnFg = false;

					if (Math.IEEERemainder(data.ColumnNo, magConfig.PackageQtyX) == 1)
					{
						data.IsFrameStart = true;
					}
					if (Math.IEEERemainder(data.ColumnNo, magConfig.PackageQtyX) == 0)
					{
						data.IsFrameEnd = true;
					}

					data.PackageList = new List<Content>();
					colDataList.Add(data);
				}
				foreach (Content contentsPackage in contentsPackageList)
				{
					int columnNo = magConfig.GetColumnNO(contentsPackage.Address);
					int columnIndex = colDataList.FindIndex(d => d.ColumnNo == columnNo);
					colDataList[columnIndex].ExistColumnFg = true;
					colDataList[columnIndex].PackageList.Add(contentsPackage);
				}

				return colDataList;
			}

			private List<Content> GetUpdateByErrCd(List<Content> packageList, string errCd)
			{
				for (int rowIndex = 0; rowIndex < packageList.Count; rowIndex++)
				{
					packageList[rowIndex].MappingResultCode = getHighPriorityResultCode(errCd, packageList[rowIndex].ResultCode.ToString());
				}

				return packageList;
			}

			private List<MMFile.ColumnData> GetAroundInspectAddedData(Magazine.Config magConfig, List<MMFile.ColumnData> colDataList, int errCol, bool isBeforeCol)
			{
				int addedNum = 0;

				int targetIndex = errCol - 1;

				while (addedNum < magConfig.FrameAroundInspectColumn)
				{
					if (isBeforeCol == true)
					{
						// フレームの先頭列で、ULマガジンの下段からフレームが詰まるパターン（手前フレームがULマガジンにて1つ下のフレームとなる）
						if (colDataList[targetIndex].IsFrameStart && !magConfig.IsWbFrameInAscendingOrder)
						{
							// 一つ下のフレームの最後尾列（手前フレームの最後の列）のインデクス取得）
							targetIndex = targetIndex + (2 * magConfig.PackageQtyX) - 1;
						}
						// フレーム先頭列ではない、又はULマガジンの上段からフレームが詰まるパターン
						else
						{
							targetIndex--;
						}
					}
					else
					{
						// フレームの尻尾列で、ULマガジンの上段からフレームが詰まるパターン（手前フレームがULマガジンにて1つ上のフレームとなる）
						if (colDataList[targetIndex].IsFrameEnd	&& !magConfig.IsWbFrameInAscendingOrder)
						{
							//
							targetIndex = targetIndex - (2 * magConfig.PackageQtyX) + 1;
						}
						// フレーム尻尾列ではない、又はULマガジンの下段からフレームが詰まるパターン
						else
						{
							targetIndex++;
						}
					}

					// インデックスがマガジン外を指すインデックスになったら付与処理終了
					if (targetIndex < 0 || targetIndex >= colDataList.Count)
					{
						break;
					}

					if (colDataList[targetIndex].ExistColumnFg)
					{
						colDataList[targetIndex].PackageList =
							GetUpdateByErrCd(colDataList[targetIndex].PackageList, MappingFile.Data.Wirebonder.AROUNDINSPECTION_POINT);

						addedNum++;
					}
				}

				return colDataList;
			}

			///// <summary>
			///// 周辺検査数を取得
			///// </summary>
			///// <param name="frameInfo">フレーム情報</param>
			///// <param name="targetIndex">対象インデックス</param>
			///// <param name="mappingBaseList">MMファイル内容(打順)</param>
			///// <param name="direct">パッケージ進行方法</param>
			///// <returns></returns>
			//private int getAroundInspection(int framePackageCt, int frameYPackageCt, int aroundInspectPackageCt, Magazine.Config.FrameWirebondStartPosition frameStartPositionCd, int replaceStartAddr, Magazine.FrameDirection direct, Magazine.Config.AroundInspectionType type)
			//{
			//	if (type == Magazine.Config.AroundInspectionType.Package)
			//	{
			//		// MAPの場合、前後5pcsが対象
			//		return aroundInspectPackageCt;
			//	}

			//	//
			//	//前後2列(41mm) 前後1列(60mm縦) 前後半列(60mm横)の周辺検査数算出
			//	//

			//	// 異常アドレスの発生フレームを1フレーム目として計算する為に差し引くべきオフセット段数の計算
			//	double step = Math.Truncate(Convert.ToDouble((replaceStartAddr - 1) / framePackageCt));

			//	// フレーム内でのアドレスを算出
			//	int addrOnFrame = replaceStartAddr - framePackageCt * (int)step;

			//	// remainderVAL:異常パッケージのその列におけるアドレスを算出(1～YPackageCTまでのアドレス)
			//	decimal remainderVAL = decimal.Remainder(addrOnFrame, frameYPackageCt);
			//	if (remainderVAL == 0) //フレームの川幅方向pcs数でちょうど割り切れるアドレスの時は、川幅方向で常に一番手前のパッケージなので、アドレスはその列の最大値となる。
			//	{
			//		remainderVAL = frameYPackageCt;
			//	}

			//	int aroundCT = 0;

			//	//double resultVAL = Math.Truncate(Convert.ToDouble((mappingBaseList[targetIndex].AddressNO -1) / frameInfo.YPackageCT)); //フレーム当たりの列数が奇数の場合に対応して無い為、この行は使用不可
			//	// フレーム内における異常パッケージの列アドレス
			//	double resultVAL = Math.Ceiling(((double)addrOnFrame) / ((double)frameYPackageCt));

			//	switch (frameStartPositionCd)
			//	{
			//		case Magazine.Config.FrameWirebondStartPosition.FrontSide:
			//			// 偶数列
			//			if (resultVAL % 2 == 0)
			//			{
			//				if (direct == Magazine.FrameDirection.After) //異常アドレスから奥(After)方向の周辺検査数取得
			//				{// ノーマル打順・偶数列におけるAfterは、その列内の奥方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(remainderVAL - 1);
			//				}
			//				else //異常アドレスから手前(Before)方向の周辺検査数取得
			//				{// ノーマル打順・偶数列におけるBeforeは、その列無いの手前方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(Math.Abs(frameYPackageCt - remainderVAL));
			//				}
			//			}
			//			//奇数列
			//			else
			//			{
			//				if (direct == Magazine.FrameDirection.After) //異常アドレスから奥(After)方向の周辺検査数取得
			//				{// ノーマル打順・奇数列におけるAfterは、その列内の手前方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(Math.Abs(frameYPackageCt - remainderVAL));
			//				}
			//				else // 異常アドレスから手前(Before)方向の周辺検査数取得
			//				{// ノーマル打順・奇数列におけるBeforeは、その列内の奥方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(remainderVAL - 1);

			//				}
			//			}
			//			break;
			//		case Magazine.Config.FrameWirebondStartPosition.BackSide:
			//			//TODO 60mmフレームの打順が反対の為、緊急対応　後に書き換える事

			//			// 偶数列
			//			if (resultVAL % 2 == 0)
			//			{
			//				if (direct == Magazine.FrameDirection.After) //異常アドレスから手前(After)方向の周辺検査数取得
			//				{// 逆打順・偶数列におけるAfterは、その列内の手前方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(Math.Abs(frameYPackageCt - remainderVAL));
			//				}
			//				else// 異常アドレスから奥(Before)方向の周辺検査数取得
			//				{// 逆打順・偶数列におけるBeforeは、その列内の奥方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(remainderVAL - 1);
			//				}
			//			}
			//			// 奇数列
			//			else
			//			{
			//				if (direct == Magazine.FrameDirection.After) //異常アドレスから奥(After)方向の周辺検査数取得
			//				{// 逆打順・奇数列におけるAfterは、その列内の奥方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(remainderVAL - 1);
			//				}
			//				else // 異常アドレスから手前(Before)方向の周辺検査数取得
			//				{// 逆打順・奇数列におけるBeforeは、その列内の手前方向のパッケージ数を取得
			//					aroundCT = Convert.ToInt32(Math.Abs(frameYPackageCt - remainderVAL));
			//				}
			//			}
			//			break;
			//	}

			//	if (aroundInspectPackageCt == (frameYPackageCt / 2))
			//	{
			//		if (aroundCT < (frameYPackageCt / 2))
			//		{
			//			//任意列が半列の場合、寄っている進行方向に半列数を足す
			//			aroundCT += aroundInspectPackageCt;
			//		}
			//	}
			//	else
			//	{
			//		//折り返し地点までの数と任意の列検査数を足す
			//		aroundCT += aroundInspectPackageCt;
			//	}

			//	return aroundCT;
			//}

			/// <summary>
			/// 検査不要(M)を内容(MappingResultCode)に設定
			/// </summary>
			private Dictionary<int, string> getNonInspectionData(Dictionary<int, string> mappingData)
			{
				List<MacDefect> defectList = MacDefect.GetNonInspection(this.LotNo);
				foreach (MacDefect d in defectList)
				{
					if (!mappingData.ContainsKey(d.DefAddressNo))
					{
						throw new ApplicationException(
							string.Format("検査不要(M)を設定中, 存在しないアドレスへの参照が発生。 MMファイル:{0} アドレス:{1}", this.Info.FullName, d.DefAddressNo));
					}

					mappingData[d.DefAddressNo] = MappingFile.Data.Wirebonder.NONINSPECTION_POINT;

					this.IsMapping = true;
				}

				return mappingData;
			}

			public static bool IsFileNameAddLot(FileInfo file)
			{
				string[] nameChar = Path.GetFileNameWithoutExtension(file.Name).Split('_');
				if (nameChar.Count() < 4)
				{
					return false;
				}
				else { return true; }
			}

			public void Run()
			{
				//内部で使用している引数を関数の引数に出す
				this.Contents = GetData();

				// WBマッピングフラグを立てる。
				if (this.IsMapping)
				{
					setMappingInspection();
				}

				// AIマッピング用データ取得
				Magazine.Config magConfig = Magazine.Config.GetData(this.TypeCd);

				Dictionary<int, string> mappingData = GetMappingDataForAI(this.Contents, magConfig);

				// 2015.3.5 「M」の箇所が有った場合、マッピングフラグを立てる処理追加
				// 手前のWBマッピングフラグを立てている処理とここの場所で一緒にできそうだが、
				// 流動中の更新の為、処理を移動させるとDataBase側のマッピングフラグを見て処理している箇所が有った場合に不具合に繋がる恐れがある。
				// なのでこの処理に再度立てる処理を追加する。
				if (this.IsMapping)
				{
					setMappingInspection();
				}

				// AIマッピング用ファイル作成
				createMappingFile(this.LotNo, mappingData);

				// MMファイルの複製作成(AI後照合, MD前照合用)
				keepMachineLog(this.Info.FullName, this.LotNo);
			}

			/// <summary>
			/// マッピング検査フラグをセットする
			/// </summary>
			public void setMappingInspection()
			{
				Lot lot = Lot.GetData(this.LotNo);
				if (lot == null)
				{
					lot = new Lot();
					lot.LotNo = this.LotNo;
					lot.TypeCd = this.TypeCd;
				}

				lot.IsMappingInspection = true;
				lot.InsertUpdate();

				this.WorkMachine.InfoLog("MMファイル内容からマッピング検査設定", lot.LotNo);
			}

			public Dictionary<int, string> GetMappingDataForAI(List<Content> wireContents, Magazine.Config magConfig) 
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();		

				// ユニットで別れたデータをアドレスで纏めたマッピングデータを生成する
				foreach (Content c in wireContents)
				{
					if (retv.ContainsKey(c.Address))
					{
						// ユニットとユニットを比較して優先順位が高い方の処理CDをアドレスへ代入する
						retv[c.Address] = getHighPriorityResultCode(c.MappingResultCode, retv[c.Address]);
					}
					else
					{
						retv.Add(c.Address, c.MappingResultCode);
					}
				}

				// 不足アドレスが存在する場合、警告表示
				if (retv.Max(c => c.Key) < magConfig.TotalMagazinePackage)
				{
					this.WorkMachine.ExclamationLog(
						string.Format("MMファイル内容のアドレスが不足しています。不足アドレスには検査を代入しました。ファイルパス:{0}", this.Info.FullName));
				}

				// 不足アドレスが存在する場合、該当アドレスに周辺検査(S)を設定
				for (int addressNO = 1; addressNO <= magConfig.TotalMagazinePackage; addressNO++)
				{
					if (!retv.ContainsKey(addressNO))
					{
						retv.Add(addressNO, MappingFile.Data.Wirebonder.AROUNDINSPECTION_POINT);
					}
				}

				// 検査不要(M)を内容(MappingResultCode)に設定
				retv = getNonInspectionData(retv);

				// フレームが空の段（タイプにより偶数段あるいは奇数段に設定されている）をAI装置に空突きさせない為に
				//frameInfoのLoadStepCDによって、偶数段、奇数段のマッピングCDを0にしたマッピングデータリストを作成(2014/1/17 n.yoshimoto)
				if (magConfig.LoadStepCD == Magazine.Config.LoadStep.Even || magConfig.LoadStepCD == Magazine.Config.LoadStep.Even_NaturalRev)
				{// 偶数段にフレームがある場合
					retv = getInspectUnneededStepZeroMappingData(0, magConfig.StepNum, magConfig.TotalFramePackage, retv);
				}
				else if (magConfig.LoadStepCD == Magazine.Config.LoadStep.Odd || magConfig.LoadStepCD == Magazine.Config.LoadStep.Odd_NaturalRev)
				{// 奇数段にフレームがある場合
					retv = getInspectUnneededStepZeroMappingData(1, magConfig.StepNum, magConfig.TotalFramePackage, retv);
				}

				return retv;
			}

			public static Dictionary<int, string> getInspectUnneededStepZeroMappingData(int startStepIndex, int magazineStepMaxCT, int framePkgGT, Dictionary<int, string> mappingData)
			{
				// 偶数設定されてる場合：1,3,5…
				// 奇数設定されてる場合：2,4,6…最大段数までのフレームで繰り返し
				for (int stepIndex = startStepIndex; stepIndex < magazineStepMaxCT; stepIndex += 2)
				{
					for (int pkgIndexInFrame = 1; pkgIndexInFrame <= framePkgGT; pkgIndexInFrame++)
					{
						int mappingAddr = framePkgGT * stepIndex + pkgIndexInFrame;

						mappingData[mappingAddr] = "0";
					}
				}

				return mappingData;
			}

			private void createMappingFile(string lotNo, Dictionary<int, string> mappingData)
			{
				string filePath = Path.Combine(Config.Settings.ForAIMappingFileDirPath, lotNo + ".wbm");

				MappingFile.Create(filePath, mappingData);
				this.WorkMachine.InfoLog("マッピングファイル作成完了", lotNo);
			}

			private void keepMachineLog(string filePath, string lotNo) 
			{
				MachineLog.CopyKeepRecord(filePath, Config.Settings.ForWBCompareMachineLogDirPath, lotNo);
				this.WorkMachine.InfoLog("照合用MMファイル複製完了", lotNo);
			}

			public static FileInfo GetCompletedFile(string lotNo)
			{
				string lotNoDirPath = Path.Combine(Config.Settings.ForWBCompareMachineLogDirPath, lotNo);

				return MachineLog.GetLatest(lotNoDirPath, FILE_KIND, MMFile.DATESTARTINDEX_FROM_MM_ON_FILENAME);
			}

			private string getHighPriorityResultCode(string compare1, string compare2) 
			{
				Dictionary<string, string> pList = General.GetWirebonderResultCodePriority();

				int compare1Priority = int.Parse(pList[compare1]);
				int compare2Priority = int.Parse(pList[compare2]);

				if (compare1Priority == compare2Priority)
				{
					return compare1;
				}
				else if (compare1Priority < compare2Priority)
				{
					return compare1;
				}
				else
				{
					return compare2;
				}
			}

			private bool containtsBadMarkErrorCode(string errorCd) 
			{
				if (this.BadMarkErrorCodeList.ContainsKey(errorCd))
				{
					return true;
				}
				else 
				{
					return false;
				}
			}

			//private bool containtsSkipErrorCode(string errorCd)
			//{
			//	if (this.SkipErrorCodeList.ContainsKey(errorCd))
			//	{
			//		return true;
			//	}
			//	else
			//	{
			//		return false;
			//	}
			//}

			//private string getMagazineQR(string[] fileValue)
			//{
			//	string retv = string.Empty;
			//	for (int targetMagRow = fileValue.Count() - 1; targetMagRow >= 0; targetMagRow--)
			//	{
			//		if (string.IsNullOrEmpty(fileValue[targetMagRow]))
			//		{// 空白行の場合、手前の行をサーチする
			//			continue;
			//		}

			//		string[] fileValueLine = fileValue[targetMagRow].Split(',');

			//		retv = fileValueLine[CONTENTS_MAGAZINE_COL].Trim().Replace("\"", "");
			//		if (string.IsNullOrWhiteSpace(retv))
			//		{// マガジン・ロットNo列がnullか空白なら手前の行をサーチする
			//			continue;
			//		}
			//		else
			//		{// 何らかの文字列が入っていた場合、サーチ終了
			//			break;
			//		}
			//	}
			//	return retv;
			//}
		}
	}
}
