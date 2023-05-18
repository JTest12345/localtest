using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
    /// <summary>
    /// ﾃｰﾋﾟﾝｸﾞ機(内製) 後工程号機化対応
    /// 　  1.PLCからトレイID及び梱包指図書Noを読みとる。
    ///     2.トレイID及び有効フラグTrueでTnSortingTrayLotを検索し、現在有効となっているロット、ポットNoを取得する。
    ///     3.梱包指図書NoからNASCAを検索※し、投入予定ロットのロット及びポットNo一覧を取得する。
    ///     4.3のリストに2の組み合わせがあるかを確認し、ある場合OK、無い場合NGをPLCに書き込む。
    /// </summary>
    class TPMachineInfo : PLCDDGBasedMachine
    {
        private const string PLC_READY_ADDR = "B00";         //開始要求信号(OFF:0 or ON:1)  PLC(1)→上位(0)
        private const string PLC_JudgeOK_ADDR = "B10";       //ONでOK。OK/NGのどちらか一方をON
        private const string PLC_JudgeNG_ADDR = "B11";       //ONでNG。OK/NGのどちらか一方をON

        private const string PLC_SASHIZU_ADDR = "W0";        //ロットNo。20文字
        private const string PLC_TrayNo_ADDR = "W40";        //トレイNo。24文字

        private const int PLC_ON = 1;
        private const int PLC_OFF = 0
;
        protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "EM50020"; }

        public TPMachineInfo(LSETInfo lsetInfo)
        {
        }

        /// <summary>
        /// UVLEDはパラメータ照合無視
        /// </summary>
        /// <param name="lsetInfo"></param>
        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {

            base.machineStatus = Constant.MachineStatus.Runtime;
            InitPropAtLoop(lsetInfo);
            InitPLC(lsetInfo);

            //装置スタート時の要求信号あれば
            if (isRequestStartCheck())
            {
                try
                {
                    //1.PLCからトレイID及び梱包指図書Noを読みとる。
                    //指図取得
                    string sashizuno = string.Empty;
                    string sashizunoData = plc.GetDataAsString(PLC_SASHIZU_ADDR, 20, PLC.DT_STR).Trim('\r', '\0');

                    //トレイNo取得
                    string trayDm = plc.GetDataAsString(PLC_TrayNo_ADDR, 24, PLC.DT_STR).Trim('\r', '\0');

                    //装置取得データ(lotno/trayno)のエラー処理
                    if (String.IsNullOrEmpty(sashizunoData) || String.IsNullOrEmpty(trayDm))
                    {
                        string errMsg = string.Format("TP装置から取得した指図No又はトレイDMが空です。指図No:{0}/トレイDM:{1}", sashizuno, trayDm);
                        throw new ApplicationException(errMsg);
                    }

                    //指図Noは頭に"12 "が付いている場合があるので除外する。
                    string[] sashizuNoElm = sashizunoData.Split(' ');
                    if(sashizuNoElm.Count() > 1)
                    {
                        sashizuno = sashizuNoElm[1];
                    }
                    else
                    {
                        sashizuno = sashizuNoElm[0];
                    }

                    //2.トレイID及び有効フラグTrueでTnSortingTrayLotを検索し、現在有効となっているロット、ポットNoを取得する。
                    var temptnsortingtraylot = new DataContext.TnSortingTrayLot();
                    using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, LineCD)))
                    {
                        //現在有効になっているロット、ポットNoを取得する。
                        //※ここにデータ登録するST用機能で有効フラグの重複は無いように制御しているので
                        //　有効フラグレコードの重複チェックは無し。
                        temptnsortingtraylot = eicsDB.TnSortingTrayLot.Where
                            (t => t.Tray_ID == trayDm && t.Effect_FG == true).FirstOrDefault();

                        //無ければ、装置から得た情報をTnSortingTrayLotに挿入
                        if (temptnsortingtraylot == null)
                        {
                            string errMsg = string.Format("TP装置から取得したトレイは有効なデータが存在しません。トレイDM:{0}", trayDm);
                            throw new ApplicationException(errMsg);
                        }
                    }

                    //3.梱包指図書NoからNASCAを検索し、投入予定ロットのロット及びポットNo一覧を取得する。
                    List<InputScheduleInfo> listInputScheduleInfo = ConnectDB.GetInputSchedule(LineCD, sashizuno);

                    if(listInputScheduleInfo.Count() == 0)
                    {
                        string errMsg = string.Format("装置から取得した指図NoはNASCAに存在しません。指図No:{0}", sashizuno);
                        throw new ApplicationException(errMsg);
                    }

                    //4.3のリストに2の組み合わせがあるかを確認し、ある場合OK、無い場合NGをPLCに書き込む。
                    bool judge = false;//判定NG
                    foreach (InputScheduleInfo inputschduleinfo in listInputScheduleInfo)
                    {
                        if (inputschduleinfo.lotno == temptnsortingtraylot.NascaLot_NO && inputschduleinfo.potno == temptnsortingtraylot.Pot_NO) {
                            judge = true;//判定OK
                            break;
                        }
                    }

                    if (judge == false)
                    {
                        string errMsg = string.Format("TP装置から取得したトレイはNASCA梱包指図書資材と一致していません。トレイDM:{0}/指図No:{1}/ロット:{2}/ポットNo:{3}",
                                                        trayDm, sashizuno, temptnsortingtraylot.NascaLot_NO, temptnsortingtraylot.Pot_NO);
                        throw new ApplicationException(errMsg);
                    }

                    //判定OK送信トリガOFFは不要。
                    plc.SetWordAsDecimalData(PLC_JudgeOK_ADDR, PLC_ON);

                }
                catch (Exception)
                {
                    //判定NG送信。トリガOFFは不要。
                    plc.SetWordAsDecimalData(PLC_JudgeNG_ADDR, PLC_ON);
                    throw;
                }
            }
        }

    private bool isRequestStartCheck()
    {
        if (plc.GetDataAsString(PLC_READY_ADDR, 1, PLC.DT_DEC_16BIT) == PLC.BIT_ON)
        {
            return true;
        }
        return false;
    }
}
}
