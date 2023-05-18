using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.Carriers;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// SLP1の高生産性用搭載機。一次改修ではマガジン情報を読んで装置に割付けている資材と
    /// BOMチェックをするだけで実績登録等一切なし。
    /// </summary>
    class FrameLoader4 : MachineBase
    {
        public string LMagazineAddress { get; set; }
        public string WorkStartOKBitAddress { get; set; }
        public string WorkStartNGBitAddress { get; set; }


        public const int MAGAZINENO_WORD_LENGTH = 9;

        protected override void concreteThreadWork()
        {
            if (this.IsRequireInput() == true)
            {
                this.workStart();
            }
        }

        private void workStart()
        {
            string magno = this.Plc.GetMagazineNo(this.LMagazineAddress, MAGAZINENO_WORD_LENGTH);

            if(string.IsNullOrWhiteSpace(magno) == true)
            {
                throw new ApplicationException("フレーム搭載機投入マガジンNOの取得に失敗");
            }

            Magazine svrMag = Magazine.GetCurrent(magno);

            if(svrMag == null)
            {
                throw new ApplicationException($"稼働中マガジン情報が見つかりません。mag:『{magno}』");
            }

            AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);

            if(lot == null)
            {
                throw new ApplicationException($"アッセンロット情報が見つかりません。lotNo:『{svrMag.NascaLotNO}』");
            }

            ArmsApi.Model.MachineInfo machine = ArmsApi.Model.MachineInfo.GetMachine(this.MacNo);
            Material[] matlist = machine.GetMaterials();

            Process process = Process.GetProcess(svrMag.NowCompProcess);

            //原材料のBOMチェック
            BOM[] bom = Profile.GetBOM(lot.ProfileId);
            string errMsg = string.Empty;
            bool isError = WorkChecker.IsBomError(matlist, bom, out errMsg, lot, process.WorkCd, machine.MacNo);

            if (isError == true)
            {
                Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
                Log.ApiLog.Info(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, errMsg));
            }
            else
            {
                Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
                OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", svrMag.MagazineNo));
            }            
        }
    }
}
