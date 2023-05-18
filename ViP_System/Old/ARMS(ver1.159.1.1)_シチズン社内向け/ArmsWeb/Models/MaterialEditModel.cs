using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class MaterialEditModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plantcd"></param>
        public MaterialEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(this.PlantCd);

            Material[] matArray = this.Mac.GetMaterials(DateTime.Now, DateTime.Now);
            this.MatList = new Dictionary<int,Material>();

            //YAGガラス、蛍光体ブロック対応。
            //元のウェハ画面はチェンジャー用、こちらを非チェンジャー用で切り分け。2016.11.5 湯浅
            for (int i = 0; i < matArray.Length; i++)
            {
                if (matArray[i].StockerNo == 0 && matArray[i].IsWafer == false)
                {
                    this.MatList.Add(i, matArray[i]);
                }
            }

            isCheckedRingID = false;
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        public List<Material> EditTarget { get; set; }

        public MachineInfo Mac { get; set; }

        public Dictionary<int, Material> MatList { get; set; }

        public string MapDicingBladePlaceCd { get; set; }

        public bool RemoveAllFg { get; set; }

        public bool isCheckedRingID { get; set; }

        public Order[] GetCurrentOrders()
        {
            Order[] orders = Order.GetCurrentWorkingOrderInMachine(Mac.MacNo).OrderBy(o => o.WorkStartDt).ToArray();
            return orders;
        }


        /// <summary>
        /// 原材料取り外し
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="removedt"></param>
        public void RemoveMaterial(Material mat, DateTime removedt)
        {
            mat.RemoveDt = removedt;
            Mac.DeleteInsertMacMat(mat);
        }


        /// <summary>
        /// バーコード内容からMaterialを返す。
        /// 同時に投入前チェックも行う。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Material ParseMatBarCode(string code)
        {
            return ParseMatBarCode(code, false);
        }

        public Material ParseMatBarCode(string code, bool isRingId)
        {
            if (string.IsNullOrEmpty(code)) throw new ApplicationException("バーコード内容が不正です");

            // リングID対応
            if(isRingId == true)
            {
                Material mat = Material.GetMaterial(code);
                if (mat == null) throw new ApplicationException("原材料ロットが存在しません");

                mat.InputDt = DateTime.Now;

                return mat;
            }

            string[] inputval = code.Split(' ');
			string[] psinputval = code.Split(',');

			//蛍光体シート対応　2016.09.30
			if (psinputval.Length == 12 || psinputval.Length == 11)
			{
				//string hinmokucd = psinputval[0] + ".PS"; //品目CD(例：PSL27CHLC-0A1.PS)
				string lotno = psinputval[2];     //lotno (例：TFBG010-PS-0002)
                Material mat = new Material();
                //try
                //{
                //    mat = Material.GetMaterial(hinmokucd, lotno);
                //}
                //catch
                //{
                //    hinmokucd = psinputval[0] + ".DC"; //品目CD(例：PSL27CHLC-0A1.DC)
                //    mat = Material.GetMaterial(hinmokucd, lotno);
                //}

                // PS, DC工程の合理化対応
                string[] exts = { ".PS", ".DC", ".PSA", ".DCA" };

                foreach (string ext in exts)
                {
                    string materialcd = psinputval[0] + ext;
                    try
                    {
                        mat = Material.GetMaterial(materialcd, lotno);
                        break;
                    }
                    catch
                    {

                    }
                }
                if (string.IsNullOrWhiteSpace(mat.MaterialCd) == true)
                {
                    throw new ArmsException(
                        string.Format("原材料ロットが存在しません。 型番CD:{0} ロットNO:{1}", psinputval[0], lotno));
                }

                mat.InputDt = DateTime.Now;

				return mat;
			}


			if (inputval.Length == 1)
            {
                //プレフィックス無しのロット番号のみラベルはウェハーラベルとして扱う
                //ウェハーチェンジャー無しダイボンダーの対策　2014.01.08
                Material[] matlst = Material.GetMaterials(null, null, inputval[0], false, false);
                var waferlst = matlst.Where(m => m.IsWafer == true);

                if (waferlst.Count() == 0)
                {
                    throw new ApplicationException("存在しないウェハーです：" + inputval[0]);
                }

                if (waferlst.Count() >= 2)
                {
                    throw new ApplicationException("同ロット番号の複数ウェハーが存在します：" + inputval[0]);
                }
                var wafer = waferlst.First();
                wafer.InputDt = DateTime.Now;

                return wafer;
            }
            else if (inputval.Length == 3)
            {
                string labelkb = inputval[0];
                string labelno = inputval[1];

                string ggcode = Material.GetMaterialCdFromLabel(labelkb, labelno);
                string lotno = inputval[2];

                if (ggcode == null) throw new ApplicationException("原材料ラベルのマスタに存在しません");

                Material mat = Material.GetMaterial(ggcode, lotno);
                if (mat == null) throw new ApplicationException("原材料ロットが存在しません");


                if (!string.IsNullOrEmpty(this.MapDicingBladePlaceCd))
                {
                    if (Material.CanInputMapDicingBlade(this.MapDicingBladePlaceCd, mat) == false)
                    {
                        throw new ApplicationException("このブレードはこの場所に取付できません");
                    }
                }

                mat.InputDt = DateTime.Now;

                string errMsg;
                if (WorkChecker.IsErrorBeforeInputMat(mat, this.Mac, out errMsg)) throw new ApplicationException(errMsg);

                return mat;
            }
            else if (inputval.Length == 4)
            {
                // 19ラインのリングデータマトリックス読込を想定
                string ringId = string.Join(" ", inputval.Skip(1));
                Material wafer = Material.GetMaterial(ringId);
                wafer.InputDt = DateTime.Now;

                return wafer;
            }
            else
            {
                throw new ApplicationException("バーコード内容が不正です");
            }
        }

        public void InsertNew(List<string> exceptMags)
        {
            foreach (Material w in this.EditTarget)
            {
                Order[] orders = GetCurrentOrders();
                foreach (string magno in exceptMags)
                {
                    Order end = orders.Where(o => o.InMagazineNo == magno).FirstOrDefault();
                    if (end != null)
                    {
                        //対象マガジンの指図がこの時点でも未完成なら、
                        //原材料投入日の1秒前を完了日として登録する
                        //実際のWorkEnd処理はSpiderから来たときに走るが、この時間が優先。
                        end.WorkEndDt = w.InputDt.Value.AddSeconds(-1);
                        end.DeleteInsert(end.LotNo);
                    }
                }

                this.Mac.DeleteInsertMacMat(w);
            }
        }

    }
}