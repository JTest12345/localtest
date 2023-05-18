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
    /// UV選別機(内製) 後工程号機化対応
    /// 　→装置(PLC)から得た情報をTnSortingTraylotに蓄積するのみ。
    /// </summary>
    class STMachineInfo : PLCDDGBasedMachine
    {
        private const string PLC_READY_ADDR = "B0000";         //開始要求信号(OFF:0 or ON:1)  PLC(1)→上位(0)

        private const string PLC_LotNo_ADDR = "W0000";          //ロットNo。64文字
        private const string PLC_PotNo_ADDR = "W0040";         //ポットNo。2ワード10進32ビット
        private const string PLC_TrayNo_ADDR = "W0050";        //トレイNo。24文字


        private const int PLC_ON = 1;
        private const int PLC_OFF = 0;

        public STMachineInfo(LSETInfo lsetInfo)
        {

        }

        public override void CheckFile(LSETInfo lsetInfo)
        {
            base.machineStatus = Constant.MachineStatus.Runtime;
            InitPropAtLoop(lsetInfo);
            InitPLC(lsetInfo);

            //装置スタート時の要求信号あれば
            if (IsGetableData(plc, PLC_READY_ADDR) == true)
            {
                try
                {
                    var tnsortingtraylot = new DataContext.TnSortingTrayLot();
                    tnsortingtraylot.Tray_ID = plc.GetDataAsString(PLC_TrayNo_ADDR, 24, PLC.DT_STR).Trim('\r', '\0');
                    tnsortingtraylot.NascaLot_NO = plc.GetDataAsString(PLC_LotNo_ADDR, 20, PLC.DT_STR).Trim('\r', '\0');
                    tnsortingtraylot.Pot_NO = plc.GetWordAsDecimalData(PLC_PotNo_ADDR, 1);
                    tnsortingtraylot.Effect_FG = true;
                    tnsortingtraylot.Register_DT = DateTime.Now;
                    tnsortingtraylot.LastUpd_DT = DateTime.Now;


                    //装置取得データ(lotno/trayno)のエラー処理
                    if (String.IsNullOrEmpty(tnsortingtraylot.NascaLot_NO) || String.IsNullOrEmpty(tnsortingtraylot.Tray_ID))
                    {
                        string errMsg = string.Format("選別装置から取得したlotno又はtrayidが空です。設備:{0}({1})/lotno:{2}/trayid:{3}",
                                                        lsetInfo.ModelNM, lsetInfo.EquipmentNO, tnsortingtraylot.NascaLot_NO, tnsortingtraylot.Tray_ID);
                        throw new ApplicationException(errMsg);
                    }

                    using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, LineCD)))
                    {
                        //同一トレイで有効フラグONが既にあるか?
                        DataContext.TnSortingTrayLot temptnsortingtraylot = eicsDB.TnSortingTrayLot.Where
                            (t => t.Tray_ID == tnsortingtraylot.Tray_ID && 
                                  t.Effect_FG == tnsortingtraylot.Effect_FG).FirstOrDefault();

                        //無ければ、装置から得た情報をTnSortingTrayLotに挿入
                        if (temptnsortingtraylot == null)
                        {
                            eicsDB.TnSortingTrayLot.InsertOnSubmit(tnsortingtraylot);
                        }
                        else//あれば、削除後挿入(登録日時と更新日時は現在時刻に書き換え)。
                        {
                            //有効フラグTrueのレコード削除
                            eicsDB.TnSortingTrayLot.DeleteOnSubmit(temptnsortingtraylot);
                            //有効フラグFalseでレコード追加
                            temptnsortingtraylot.Effect_FG = false;
                            eicsDB.TnSortingTrayLot.InsertOnSubmit(temptnsortingtraylot);

                            //新規レコード追加
                            eicsDB.TnSortingTrayLot.InsertOnSubmit(tnsortingtraylot);
                        }
                        eicsDB.SubmitChanges();
                    }
                    //トリガOFF
                    plc.SetWordAsDecimalData(PLC_READY_ADDR, PLC_OFF);
                }
                catch (Exception)
                {
                    //問題あればトリガそのままでEICSエラー。装置はタイムアウトで止まる。
                    throw;
                }
            }
        }
        private bool isRequestStartCheck()
        {
            if (plc.GetDataAsString(PLC_READY_ADDR, 1, PLC.DT_BOOL) == PLC.BIT_ON)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// UVLEDはパラメータ照合無視
        /// </summary>
        /// <param name="lsetInfo"></param>
        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
        }
    }
}
