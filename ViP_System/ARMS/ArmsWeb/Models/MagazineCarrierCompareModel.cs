using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class MagazineCarrierCompareModel
    {
        public MagazineCarrierCompareModel(string magazineno)
        {
            DataMatirxList = new List<string>();

            this.MagazineNo = magazineno;
        }

        /// <summary>
        /// マガジン
        /// </summary>
        public string MagazineNo { get; set; }

        /// <summary>
        /// 基板DM
        /// </summary>
        public List<string> DataMatirxList { get; set; }

        /// <summary>
        /// 判定結果
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 照合
        /// </summary>
        /// <returns></returns>
        public void Compare()
        {
            string errMsg;
            IsOK = false;
            
            ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(this.MagazineNo);
            if (mag == null)
            {
                IsOK = false;
                errMsg = "照合NG：ロット情報が存在しません";
            }
            else
            {
                string[] list = ArmsApi.Model.LotCarrier.GetCarrierNo(mag.NascaLotNO, true);

                if (list == null || list.Count() == 0)
                {
                    IsOK = false;
                    errMsg = "照合NG：ロットと基板DM/キャリアQRの紐付け情報が存在しません";
                }
                else
                {
                    List<string> carrierList = list.ToList();

                    List<string> notExistList = new List<string>();
                    foreach (string dataMatirx in this.DataMatirxList)
                    {
                        if (!carrierList.Exists(r => r == dataMatirx))
                        {
                            notExistList.Add(dataMatirx);
                        }
                    }

                    List<string> notExistList_DB = new List<string>();
                    foreach (string carrier in carrierList)
                    {
                        if (!this.DataMatirxList.Exists(r => r == carrier))
                        {
                            notExistList_DB.Add(carrier);
                        }
                    }

                    if (notExistList.Count == 0 && notExistList_DB.Count == 0)
                    {
                        IsOK = true;
                        errMsg = "照合OK。";
                    }
                    else
                    {
                        IsOK = false;
                        errMsg = "照合NG";
                        if (notExistList.Count != 0)
                        {
                            errMsg += string.Format("\r\n{0}は紐付けされていません。", string.Join(",", notExistList));
                        }
                        if (notExistList_DB.Count != 0)
                        {
                            errMsg += string.Format("\r\n{0}が照合対象として読み込まれていません。", string.Join(",", notExistList_DB));
                        }
                    }
                }
            }

            
            Msg = errMsg;
        }
    }
}