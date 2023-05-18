using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class FrameEditModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plantcd"></param>
        public FrameEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(this.PlantCd);

            this.FrameList = new List<Material>();
            var matArray = this.Mac.GetMaterials(DateTime.Now, DateTime.Now);
            foreach (var mat in matArray)
            {
                if (mat.IsFrame != true) continue;
                this.FrameList.Add(mat);
            }
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        public Material EditTarget { get; set; }

        public MachineInfo Mac { get; set; }

        public List<Material> FrameList;

        public int StockerNo { get; set; }

        public Material ParseFrame(string barcode, int stockerNo)
        {
            //LUPR-104TD, ,TEST-110318-1,1643261,0,GG0143060,2,1, ,CG
            string[] inputval = barcode.Split(',');

            //金型メンテナンス履歴ありの場合は要素数11
            if (inputval.Length != 10 && inputval.Length != 11)
            {
                throw new ApplicationException("バーコード内容が不正です");
            }

            string ggcode = inputval[5];
            string lotno = inputval[2];

            Material mat = Material.GetMaterial(ggcode, lotno);
            if (mat == null)
            {
                throw new ApplicationException("原材料ロットが存在しません");
            }

            mat.InputDt = DateTime.Now;
            mat.StockerNo = stockerNo;

            return mat;
        }

        public void InsertNew()
        {
            EditTarget.InputDt = DateTime.Now;
            this.Mac.DeleteInsertMacMat(EditTarget);
        }

        public void Remove()
        {
            EditTarget.RemoveDt = DateTime.Now;
            this.Mac.DeleteInsertMacMat(EditTarget);
        }
    }
}