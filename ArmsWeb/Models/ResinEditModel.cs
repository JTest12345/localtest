using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class ResinEditModel
    {
          /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plantcd"></param>
        public ResinEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(this.PlantCd);
            
            Resin[] resins = this.Mac.GetResins(DateTime.Now, DateTime.Now);
            this.ResinList = new Dictionary<int, Resin>();
            for (int i = 0; i < resins.Length; i++)
            {
                this.ResinList.Add(i, resins[i]);
            }
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        public Resin EditTarget { get; set; }

        public MachineInfo Mac { get; set; }

        //20220413 ADD START
        public String message { get; set; }
        //20220413 ADD END

        /// <summary>
        /// 装置に割り付け済みの条件一覧
        /// </summary>
        public Dictionary<int, Resin> ResinList;

        public void RemoveResin(Resin rsn, DateTime removedt)
        {
            rsn.RemoveDt = removedt.AddSeconds(-1);
            Mac.DeleteInsertMacResin(rsn);
        }

        public void InsertNew()
        {
            this.EditTarget.InputDt = DateTime.Now;
            this.Mac.DeleteInsertMacResin(this.EditTarget);
        }

        public Resin ParseResin(string barcode)
        {

            if (string.IsNullOrEmpty(barcode)) throw new ApplicationException($"バーコード内容が不正です。バーコード内容:『{barcode}』");

            //富士情報　変更　start
            Resin res = Resin.GetResin(barcode);
            if (res == null) throw new ApplicationException($"調合結果ID=『{barcode}』がデータベース内(TnResinMix)存在しません。樹脂調合ツール登録後からARMSに取り込まれていない可能性があります。");

            //int resid;
            //if (int.TryParse(barcode, out resid) == false) throw new ApplicationException($"調合結果IDは数字を入力してください。バーコード内容:『{barcode}』");
            //Resin res = Resin.GetResin(resid);
            //if (res == null) throw new ApplicationException($"調合結果ID=『{barcode}』がデータベース内(TnResinMix)存在しません。樹脂調合ツール登録後からARMSに取り込まれていない可能性があります。");
            //富士情報　追加　end

            string errMsg;
            if (WorkChecker.IsErrorBeforeInputResin(res, this.Mac, out errMsg))
            {
                throw new ApplicationException(errMsg);
            }
            //20220413 ADD START
            else
            {
                message = errMsg;
            }
            //20220413 ADD END

            res.InputDt = DateTime.Now;
            return res;
        }
    }
}