using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmsApi.Model;
using ArmsApi;
using System.Data;
using System.Data.SqlClient;
using ArmsApi.Model.NASCA;

namespace ArmsNascaBridge2
{
    class AsmLotExport
    {
        public static void Export()
        {
			try
			{
				ArmsApi.Model.NASCA.Exporter exp = ArmsApi.Model.NASCA.Exporter.GetInstance(true);
				AsmLot[] lotlist = getActiveLotList();

				foreach (AsmLot lot in lotlist)
				{
					Log.SysLog.Info("[NascaBridge2] Nasca連携Start:" + lot.NascaLotNo);
					exp.SendAsmLotAllProc(lot, true);
					Log.SysLog.Info("[NascaBridge2] Nasca連携End:" + lot.NascaLotNo);
				}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] AsmLotExport Error:" + err.ToString());
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
