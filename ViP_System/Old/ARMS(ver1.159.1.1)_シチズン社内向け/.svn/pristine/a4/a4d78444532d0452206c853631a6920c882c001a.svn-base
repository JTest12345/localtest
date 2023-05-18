using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.NJDB
{
	public class MixMaterial
	{
		/// <summary>調合結果ID</summary>
		public long MixResultID { get; set; }
		/// <summary調合順番></summary>
		public int MateLineNO { get; set; }
		/// <summary>資材ロットNO</summary>
		public string MateLotNO { get; set; }
		public string MaterialCD { get; set; }
		/// <summary>前仕込樹脂調合結果ID</summary>
		public long? ParentsResultID { get; set; }
		/// <summary>前仕込樹脂識別FG</summary>
		public bool? BefResinFG { get; set; }
		/// <summary>資材登録除外FG</summary>
		public bool? ExcludeFG { get; set; }

		public MixMaterial(long mixResultID, int lotLineNO, string mateLotNO, string materialCD, long? parentsResultID, bool? befResinFG, bool? excludeFG)
		{
			this.MixResultID = mixResultID;
			this.MateLineNO = lotLineNO;
			this.MateLotNO = mateLotNO;
			this.MaterialCD = materialCD;
			this.ParentsResultID = parentsResultID;
			this.BefResinFG = befResinFG;
			this.ExcludeFG = excludeFG;
		}

		public static IEnumerable<MixMaterial> GetAllMaterial(DataContext.NJDBDataContext resinDB, long mixResultID)
		{
			List<MixMaterial> resinMatList = MixMaterial.Get(resinDB, mixResultID).ToList();

			foreach (MixMaterial resinMat in resinMatList.FindAll(r => r.BefResinFG == true))
			{
				resinMatList.AddRange(MixMaterial.GetBefResinMaterial(resinDB, resinMat, mixResultID));
			}

			return resinMatList;
		}

		public static IEnumerable<MixMaterial> Get(DataContext.NJDBDataContext resinDB, long mixResultID)
		{
			IEnumerable<MixMaterial> retV = from mr in resinDB.JttMIXRESULT
											from mrl in resinDB.JttMIXRESULTLOT
											from mrm in resinDB.JtmMIXRATEMAT
											from mat in resinDB.JtmMAT
											from matconv in resinDB.JtmMATCONV
											where mr.MixResult_ID == mrl.MixResult_ID && mr.MixRate_ID == mrm.MixRate_ID && mrl.LotLin_NO == mrm.MateLin_NO
												 && mrm.Material_CD == mat.Material_CD && mat.Material_CD == matconv.ResinMat_CD && mr.MixResult_ID == mixResultID
											select new MixMaterial(
														mr.MixResult_ID,
														mrl.LotLin_NO,
														mrl.Lot_NO,
														matconv.RootsMat_CD,
														mrl.ParentMixResult_ID,
														mat.BefResin_FG,
														mat.Exclude_FG
												);

			return retV;
		}

		public static IEnumerable<MixMaterial> Get(DataContext.NJDBDataContext resinDB, long mixResultID, bool excludeFG)
		{
			IEnumerable<MixMaterial> temp = Get(resinDB, mixResultID);

			temp.Where(t => t.ExcludeFG == excludeFG);

			return temp;
		}

		/// <summary>
		/// mixMaterialで指定した資材以下に紐つく全前仕込樹脂の全資材を取得
		/// 資材の樹脂調合結果IDはrootMixResultIDで指定したIDに置換する。
		/// </summary>
		/// <param name="resinDB"></param>
		/// <param name="mixMaterial"></param>
		/// <param name="rootMixResultID"></param>
		/// <returns></returns>
		public static IEnumerable<MixMaterial> GetBefResinMaterial(DataContext.NJDBDataContext resinDB, MixMaterial mixMaterial, long rootMixResultID)
		{
			List<MixMaterial> retv = new List<MixMaterial>();

			if (mixMaterial.BefResinFG == false && mixMaterial.ExcludeFG == true)
			{
				return retv;
			}

			if (mixMaterial.BefResinFG == false || mixMaterial.ParentsResultID.HasValue == false || mixMaterial.ParentsResultID.Value == 0)
			{
				mixMaterial.MixResultID = rootMixResultID;
				retv.Add(mixMaterial);
				return retv;
			}

			IEnumerable<MixMaterial> temp = Get(resinDB, mixMaterial.ParentsResultID.Value);

			foreach (MixMaterial befResin in temp)
			{
				retv.AddRange(GetBefResinMaterial(resinDB, befResin, rootMixResultID));
			}

			return retv;
		}

		public static void UpdateResinMix(DataContext.ARMSDataContext armsDB, IEnumerable<MixMaterial> targetList)
		{
			List<DataContext.TnResinMixMat> insertTargetList = new List<DataContext.TnResinMixMat>();

			foreach (MixMaterial target in targetList)
			{
                if(insertTargetList.Where(l => l.mixresultid == target.MixResultID && l.materialcd == target.MaterialCD.Trim() && l.lotno.ToUpper() == target.MateLotNO.Trim().ToUpper())
                    .Any() == true)
                {
                    continue;
                }

                if (target.ExcludeFG.HasValue && target.ExcludeFG.Value || target.BefResinFG.HasValue && target.BefResinFG.Value)
                    continue;

                if (insertTargetList.Exists(i => i.mixresultid == target.MixResultID
                        && i.materialcd == target.MaterialCD.Trim() && i.lotno == target.MateLotNO.Trim()))
                    continue;

                var e = armsDB.TnResinMixMat
                    .SingleOrDefault(r => r.mixresultid == target.MixResultID && r.materialcd == target.MaterialCD.Trim() && r.lotno == target.MateLotNO.Trim());

                if (e == null)
                {
                    DataContext.TnResinMixMat insertTarget = new DataContext.TnResinMixMat();

                    insertTarget.mixresultid = target.MixResultID;
                    insertTarget.materialcd = target.MaterialCD.Trim();
                    insertTarget.lotno = target.MateLotNO.Trim();

                    insertTargetList.Add(insertTarget);
                }
			}

			armsDB.TnResinMixMat.InsertAllOnSubmit(insertTargetList);
			armsDB.SubmitChanges();
		}
	}
}
