using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class MaterialThawingModel
    {
        public MaterialThawingModel(string empcd)
        {
            this.EmpCd = empcd;
            this.EditTarget = new List<Material>();
        }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        /// <summary>
        /// 原材料リスト（単入力なのですが・・・）
        /// </summary>
        public List<ArmsApi.Model.Material> EditTarget { get; set; }

        /// <summary>
        /// QRコード分割 & TnMaterials存在チェック
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Material ParseMatBarCode(string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ApplicationException("バーコード内容が不正です");

            //QRコード分割
            Material.MatLabel ml = Material.GetMatLabelFromBarcode(code);
            if (ml.IsBarcodeError)
            {
                throw new ApplicationException("バーコード内容が不正です");
            }

            //TnMaterials検索
            Material mat = Material.GetMaterial(ml.MaterialCd, ml.LotNo);

            //limitdtチェック
            if (mat.LimitDt < DateTime.Now)
            {
                throw new ApplicationException("すでに期限切れの原材料です");
            }
            mat.InputDt = DateTime.Now;

            return mat;
        }

        /// <summary>
        /// QRコードチェック
        /// 1. TmDBResinLifeに存在しているか
        /// 2. TnMatCondに存在していないか
        /// </summary>
        /// <param name="matcd"></param>
        /// <param name="lotno"></param>
        public void CheckQRCode(string matcd, string lotno)
        {
            //TmDBResinLife 存在チェック（存在していること）
            Material.ResinLife[] life = Material.getResinLifes("", matcd, false);
            if (life.Length == 0)
            {
                throw new ApplicationException("解凍処理できない原材料です");
            }

            //TnMatCond 存在チェック（存在していないこと）
            MatChar mc = MatChar.GetMaterialOpen(matcd, lotno);
            if (mc != null)
            {
                throw new ApplicationException("すでに解凍処理されている原材料です");
            }
        }

        /// <summary>
        /// TnMatCond登録
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool InsertUpdateMatCond(out string errMsg)
        {
            try
            {
                foreach (ArmsApi.Model.Material m in EditTarget)
                {
                    //TnMatCond登録
                    MatChar.InsertUpdateMatCond(m.TypeCd, m.LotNo, MatChar.LOTCHARCD_MATERIALOPEN,
                                                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), false, DateTime.Now);
                }
                errMsg = string.Empty;
                return true;
            }
            catch (ApplicationException ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }
    }
}