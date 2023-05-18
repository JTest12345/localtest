using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class WaferEditModel
    {
          /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plantcd"></param>
        public WaferEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(this.PlantCd);


            this.WaferList = new Dictionary<int, Material>();
            var wafArray = this.Mac.GetMaterials(DateTime.Now, DateTime.Now).OrderBy(m => m.StockerNo).ToArray();
            //表示条件を「ウェハのみ」ではなく「搭載機以外でTnMacMatのstockernoが0以外」に変更
            //蛍光体ブロック、YAGガラス対応　2016.11.5 湯浅
            if (this.Mac.HasDoubleStocker == false)
            {
                for(int i = 0; i < wafArray.Length; i++)
                {
                    //if (waf.IsWafer != true) continue;
                    if (wafArray[i].StockerNo == 0) continue;
                    if (wafArray[i].RemoveDt.HasValue) continue;
                    this.WaferList.Add(i, wafArray[i]);
                }
            }
            this.RemoveList = new List<Material>();
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        public List<Material> EditTarget { get; set; }

        public MachineInfo Mac { get; set; }

        public Dictionary<int, Material> WaferList;
        public List<string> SelectedList { get; set; }
        public string NewPlantCd { get; set; }
        public MachineInfo NewMac { get; set; }
        public List<Material> RemoveList { get; set; }


        public void RemoveAllWafer(DateTime removedt)
        {

            foreach (Material w in this.WaferList.Values)
            {
                w.RemoveDt = removedt.AddSeconds(-1);
                Mac.DeleteInsertMacMat(w);
            }
        }

        public void InsertNew()
        {
            foreach (Material w in this.EditTarget)
            {
                w.InputDt = DateTime.Now;
                this.Mac.DeleteInsertMacMat(w);
            }
        }

        public List<Material> ParseWafers(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) throw new ApplicationException("バーコード内容が不正です");

            string[] inputval = barcode.Split(',');

            List<Material> waflist = new List<Material>();

            //QRコード内容分解
            for (int i = 0; i < inputval.Length; i++)
            {
                try
                {
                    string[] subval = inputval[i].Split(':');
                    if (subval.Length != 3) throw new ApplicationException("バーコード内容が不正です");

                    string stocker = subval[0];
                    string matcd = subval[1];
                    string lotno = subval[2];

                    Material waf = Material.GetMaterial(matcd, lotno);
                    if (waf == null) throw new ApplicationException("ロット情報が存在しません：" + lotno);

                    //蛍光体ブロック・YAGガラス対応。ウェハ縛りを削除。 2016.11.5 湯浅
                    //if (!waf.IsWafer) throw new ApplicationException("ウェハーではありません：" + lotno);

                    int stockerNo;
                    if (int.TryParse(stocker, out stockerNo))
                    {
                        waf.StockerNo = stockerNo;
                    }
                    else
                    {
                        throw new ApplicationException("ストッカー段数が不正です：" + stocker);
                    }

                    waf.InputDt = DateTime.Now;

                    string errMsg;
                    if (WorkChecker.IsErrorBeforeInputMat(waf, this.Mac, out errMsg))
                    {
                        throw new ApplicationException(errMsg);
                    }

                    waflist.Add(waf);
                }
                catch (ApplicationException)
                {
                    throw;
                }
                catch
                {
                    throw new ApplicationException("QR処理中に不明なエラーが発生しました");
                }
            }

            return waflist;
        }
                
        public void ChangeMachine()
        {
            // 振替元装置のウェハーを取り外す
            this.RemoveList.Clear();
            foreach (Material w in this.EditTarget)
            {
                w.RemoveDt = DateTime.Now;
                this.Mac.DeleteInsertMacMat(w);

                Material remove = Material.GetMaterial(w.MaterialCd, w.LotNo);
                if (remove == null) continue;
                remove.StockerNo = w.StockerNo;
                this.RemoveList.Add(remove);
            }

            List<Material> newMacWafList = new List<Material>(); 
            var wafArray = this.NewMac.GetMaterials(DateTime.Now, DateTime.Now);
            if (this.NewMac.HasDoubleStocker == false)
            {
                foreach (var waf in wafArray)
                {
                    if (waf.StockerNo == 0) continue;
                    if (waf.RemoveDt.HasValue) continue;
                    newMacWafList.Add(waf);
                }
            }

            // 振替対象のウェハーカセットのストッカーNoを振り直して登録する
            //    振替先装置に割付済みカセット無し：1番から
            //    振替先装置に割付済みカセット有り：ストッカーNoの後ろから連番
            int stockerNo = 1;
            if (newMacWafList.Any() == true)
            {
                stockerNo = newMacWafList.Select(w => w.StockerNo).Max() + 1;
            }
            foreach (Material w in this.EditTarget)
            {
                w.StockerNo = stockerNo;
                w.InputDt = DateTime.Now;
                w.RemoveDt = null;
                this.NewMac.DeleteInsertMacMat(w);

                stockerNo++;
            }
        }
    }
}