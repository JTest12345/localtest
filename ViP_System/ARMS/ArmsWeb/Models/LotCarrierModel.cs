using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace ArmsWeb.Models
{
    public class LotCarrierModel 
    {
        public LotCarrierModel(string empcd)
        {
            DataMatirxList = new List<string>();
			this.EmpCd = empcd;
            this.ConfirmedMagazineFg = false;
		}
		

		/// <summary>
		/// 登録作業者
		/// </summary>
		public string EmpCd { get; set; }
        public string LotNo { get; set; }
        public string MagazineNo { get; set; }
        public List<string> DataMatirxList { get; set; }
        public Magazine OperationMagazine { get; set; }
        public Magazine CurrentMagazine { get; set; }
        public bool ConfirmedMagazineFg { get; set; }


		static string MAPPING_IMPORT_FRAME = "MAPPINGIMPORTFRAME";

		public void Insert()
        {
            //TnLotCarrier登録
            LotCarrier lotcarrier = new LotCarrier(this.LotNo, this.DataMatirxList, this.EmpCd);
            lotcarrier.DeleteInsert();
        }

        public Magazine GetCurrentMagazine()
        {
            return Magazine.GetCurrent(this.MagazineNo);
        }

        public Magazine GetMagazine()
        {
			Magazine.GetMagazine(this.LotNo, true);
			Magazine[] lotlist = Magazine.GetMagazine(this.LotNo, true);
            if (lotlist.Count() == 1)
            {
                return lotlist[0];
            }
            else
            {
                throw new ApplicationException(string.Format("稼働中ではないロットです。MagazineNo:{0} LotNo:{1}", this.MagazineNo, this.LotNo));
            }
        }

		public bool MappingConfilm()
		{
			AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
			if (lot == null)
			{
				return false;
			}
			BOM[] boms = Profile.GetBOM(lot.ProfileId);
			if(boms.Count() == 0)
			{
				return false;
			}
			GnlMaster[] gnls = GnlMaster.Search(MAPPING_IMPORT_FRAME);
			if(gnls.Count() == 0)
			{
				return false;
			}
			for(int i = 0; i < boms.Count(); i++)
            {
				for (int j = 0; j < gnls.Count(); j++)
				{
					if (boms[i].MaterialCd == gnls[j].Code)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int? getMappingStartProc()
		{
			AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
			if (lot == null)
			{
				return null;
			}

			return Process.GetFirstMappingProc(lot.TypeCd);

		}

        /// <summary>
        /// メーカーの基板データを取り込む
        /// </summary>
        /// <param name="procNo"></param>
		public void ImportMapping(int procNo)
        {
            ArmsApi.Config.LoadSetting();
            //読み込み関数（carrier）
            using (SqlConnection conLens = new SqlConnection(LENS2_Api.Config.Settings.LensConnectionString))
            {
                conLens.Open();
                SqlCommand cmdLens = conLens.CreateCommand();
                cmdLens.Transaction = conLens.BeginTransaction();
                foreach (string carrier in this.DataMatirxList)
                {
                    if (Directory.Exists(ArmsApi.Config.Settings.MappingDirectoryPath) == false)
                    {
                        cmdLens.Transaction.Rollback();
                        throw new ApplicationException(string.Format("マッピングフォルダが見つかりません。 ディレクトリパス:{0}", ArmsApi.Config.Settings.MappingDirectoryPath));
                    }

                    string filePath = Path.Combine(ArmsApi.Config.Settings.MappingDirectoryPath, carrier + ".mpd");
                    if (File.Exists(filePath) == false)
                    {
                        cmdLens.Transaction.Rollback();
                        throw new ApplicationException(string.Format("マッピングデータが存在していません。データ名：{0} ファイルパス:{1}", carrier, filePath));
                    }
                    string mappingData = File.ReadAllText(filePath, Encoding.Default);

                    if (!LENS2_Api.MapResult.SaveData(ref cmdLens, this.LotNo, carrier, carrier, procNo, mappingData, this.EmpCd, false, 0, 0, 0, 0)
                        || !LENS2_Api.MapResult.SaveData(ref cmdLens, this.LotNo, carrier, carrier, procNo, mappingData, this.EmpCd, true, 0, 0, 0, 0))
                    {
                        cmdLens.Transaction.Rollback();
                        throw new ApplicationException(string.Format("マッピングデータの登録に失敗しました。データ名：{0}", carrier));
                    }
                    //初期取込の実績はダミーなので設備CDは"0"固定
                    LENS2_Api.WorkResult.CompleteEndProcess(this.LotNo, procNo, "0", carrier, true);
                }
                cmdLens.Transaction.Commit();
            }
        }


        /// <summary>
        /// 厚みランクが混ざっていないかチェック
        /// </summary>
        /// <returns></returns>
        public static bool CheckSubstrateThicknessRank(out string msg, List<string> dataMatrixList)
        {
            List<string> thicknessRankList = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool first = true;

			foreach (string datamatrix in dataMatrixList)
            {
                string thicknessRank = getSubstrateThicknessRank(datamatrix);
                if (!thicknessRankList.Exists(r => r == thicknessRank))
                {
                    thicknessRankList.Add(thicknessRank);
                }

                if (!first) sb.Append(",");
                sb.Append(datamatrix + ":" + thicknessRank);
                first = false;
            }

            msg = sb.ToString();
            if (thicknessRankList.Count >= 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string getSubstrateThicknessRank(string datamatrix)
        {
            return ArmsApi.Model.SubstrateThicknessRank.GetThicknessRank(datamatrix);
        }

        /// <summary>
        /// 全部良のマッピングデータを生成する。既に何かしらのデータがある場合は無視。
        /// </summary>
        /// <param name="procNo"></param>
        public void CreateAllOKMapping(int procNo)
        {
            //ArmsApi.Config.LoadSetting();

            AsmLot asmLot = new AsmLot();
            ArmsApi.Model.LENS.Mag magData = new ArmsApi.Model.LENS.Mag();

            asmLot = AsmLot.GetAsmLot(this.LotNo);
            if (asmLot == null)
            {
                throw new ApplicationException($"ARMSにロット情報が存在しません。ロット『{this.LotNo}』");
            }

            magData = ArmsApi.Model.LENS.Mag.GetData(asmLot.TypeCd);
            if (magData == null)
            {
                throw new ApplicationException($"LENSに基板構成情報が存在ません。タイプ『{asmLot.TypeCd}』");
            }

            using (SqlConnection conLens = new SqlConnection(LENS2_Api.Config.Settings.LensConnectionString))
            {
                conLens.Open();
                SqlCommand cmdLens = conLens.CreateCommand();
                cmdLens.Transaction = conLens.BeginTransaction();
                foreach (string carrier in this.DataMatirxList)
                {
                    List<ArmsApi.Model.LENS.MapResult> mapData = ArmsApi.Model.LENS.MapResult.GetCurrentData(LotNo, null, carrier, true);

                    if (mapData.Count() != 0)
                    {
                        continue;
                    }

                    string defaultMap = string.Empty;

                    int packageCt = magData.PackageQtyX * magData.PackageQtyY
                                      - LENS2_Api.MapDisableDeploy.GetData(asmLot.TypeCd, magData.PackageQtyX, magData.PackageQtyY).Count();

                    for (int i = 0; i < packageCt; i++)
                    {
                        if (i != 0)
                        {
                            defaultMap = defaultMap + ",";
                        }
                        defaultMap = defaultMap + "0";
                    }

                    if (!LENS2_Api.MapResult.SaveData(ref cmdLens, this.LotNo, carrier, carrier, procNo, defaultMap, this.EmpCd, false, 0, 0, 0, 0)
                        || !LENS2_Api.MapResult.SaveData(ref cmdLens, this.LotNo, carrier, carrier, procNo, defaultMap, this.EmpCd, true, 0, 0, 0, 0))
                    {
                        cmdLens.Transaction.Rollback();
                        throw new ApplicationException(string.Format("マッピングデータの登録に失敗しました。データ名：{0}", carrier));
                    }
                    //初期取込の実績はダミーなので設備CDは"0"固定
                    LENS2_Api.WorkResult.CompleteEndProcess(this.LotNo, procNo, "0", carrier, true);
                }
                cmdLens.Transaction.Commit();
            }
        }
        
        //TnCarrierWorkDataにデータ登録する関数を作ったが不要になったのでコメントアウト。実装前に不要になったので未検証
        //public void RegisterStepData()
        //{
        //    AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
        //    CarrireWorkData stepData = new CarrireWorkData();
        //    stepData.LotNo = this.LotNo;
        //    stepData.ProcNo = Process.GetFirstProcess(lot.TypeCd).ProcNo;
        //    stepData.Delfg = 0;
        //    stepData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
        //    foreach (string dm in this.DataMatirxList)
        //    {
        //        stepData.CarrierNo = dm;
        //        stepData.RegisterMagazineStepWithAutotCalc();
        //    }
        //}
    }
}