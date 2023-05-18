using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class LotChar
    {
        public static void Import()
        {
            try
            {
                AsmLot[] lotlist = getActiveLotList();

                foreach (AsmLot lot in lotlist)
                {
                    string lotcharcd;

                    //研削厚み取込処理対象のサーバなら厚み対象品種データを取得
                    //自動搬送ロット(ARMS側で指図発行するロット)の場合は、この取り込み特性が無い為、ここで取り込む
                    if (Config.Settings.ImportMatThickness)
                    {
                        lotcharcd = ArmsApi.Model.NASCA.Importer.GR_THICKNESS_DECISION_MATKIND_LOTCHARCD;
                        string thicknessDesitionMat_Kb = ArmsApi.Model.NASCA.Importer.GetLotCharListCd(lot.TypeCd, lot.NascaLotNo, lotcharcd);

                        if (!string.IsNullOrWhiteSpace(thicknessDesitionMat_Kb))
                        {
                            ArmsApi.Model.LotChar lotChar = new ArmsApi.Model.LotChar();
                            lotChar.ListVal = thicknessDesitionMat_Kb;
                            lotChar.LotCharCd = lotcharcd;
                            lotChar.DeleteInsert(lot.NascaLotNo, Config.Settings.LocalConnString, false);

                        }
                    }
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] LotChar Error:" + err.ToString());
            }
        }

        private static AsmLot[] getActiveLotList()
        {
            List<AsmLot> retv = new List<AsmLot>();

            Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
            foreach (Magazine mag in maglist)
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot != null) retv.Add(lot);
            }

            return retv.ToArray();
        }
    }
}
