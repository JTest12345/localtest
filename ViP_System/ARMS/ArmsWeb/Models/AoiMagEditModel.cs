using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;


namespace ArmsWeb.Models
{
    public class AoiMagEditModel
    {
        public AoiMagEditModel(string plantcd)
        {
            MagList = new List<Magazine>();
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);

            ExistMags = VirtualMag.GetVirtualMag(this.Mac.MacNo, ((int)Station.Loader));
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        public MachineInfo Mac { get; set; }

        public List<ArmsApi.Model.Magazine> MagList { get; set; }

        public List<ArmsApi.Model.Material> MatList { get; set; }

        /// <summary>
        /// 割り当て済みロット情報リスト
        /// </summary>
        public VirtualMag[] ExistMags { get; set; }

        /// <summary>
        /// すでに作業中のマガジン
        /// </summary>
        public List<string> WorkingMags { get; set; }

        public bool WorkStart(out string errMsg)
        {
            foreach (ArmsApi.Model.Magazine mag in MagList)
            {
                bool isSuccecc = startMag(mag, false, out errMsg);
                if (!isSuccecc)
                {
                    return false;
                }
            }

            errMsg = "";
            return true;
        }

        public bool IsErrorBeforeStart(Magazine mag, out string msg)
        {
            if (startMag(mag, true, out msg) == false)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool startMag(Magazine mag, bool isCheckOnly, out string msg)
        {
            try
            {
                VirtualMag vmag = new VirtualMag();

                string[] values = mag.MagazineNo.Split(VirtualMag.MAP_FRAME_SEPERATOR);
                vmag.MapAoiMagazineLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
                vmag.MaxFrameCt = int.Parse(values[VirtualMag.MAP_FRAME_QR_CT_IDX]);
                vmag.CurrentFrameCt = vmag.MaxFrameCt;
                vmag.FrameMatCd = values[VirtualMag.MAP_FRAME_QR_MATCD_IDX];
                vmag.FrameLotNo = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
                vmag.MagazineNo = vmag.MapAoiMagazineLotNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");


                Material mat = Material.GetMaterial(vmag.FrameMatCd, vmag.FrameLotNo);
                mat.InputDt = DateTime.Now;
                if (mat == null)
                {
                    msg = "原材料レコードが存在しません";
                    return false;
                }
                if (WorkChecker.IsErrorBeforeInputMat(mat, this.Mac, out msg))
                {
                    return false;
                }

                if (!isCheckOnly)
                {
                    vmag.Enqueue(vmag, new Location(this.Mac.MacNo, Station.Loader));
                    System.Threading.Thread.Sleep(10);
                }

                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }
    }
}