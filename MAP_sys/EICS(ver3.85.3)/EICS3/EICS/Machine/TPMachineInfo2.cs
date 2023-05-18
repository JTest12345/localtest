//<--後工程合理化/エラー集計
using EICS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace EICS.Machine
{
	class TPMachineInfo2 : STMachineInfo2
    //class TPMachineInfo2 : MachineBase
    {
        /// <summary>ﾃｰﾋﾟﾝｸﾞ機(ｴﾗｰ集計)</summary>
        //public const string ASSETS_NM = "ﾃｰﾋﾟﾝｸﾞ機ｴﾗｰ集計";

        protected override string POSTPROCESSEXTENSION() { return ".log$"; }


        protected override string SQL()/*
                                        { return @"SELECT DISTINCT dbo.RvtTRANCOMPLT.lot_no,dbo.RvtTRANH.start_dt,dbo.RvtTRANH.complt_dt
                                                    FROM dbo.RvtTRANH with(nolock) INNER JOIN
                                                            dbo.RvtTRANCOMPLT with(nolock) ON dbo.RvtTRANH.mnfctrsl_no = dbo.RvtTRANCOMPLT.mnfctrsl_no
                                                    WHERE (dbo.RvtTRANCOMPLT.lot_no = @lotno) AND(dbo.RvtTRANH.del_fg = '0') AND(dbo.RvtTRANCOMPLT.del_fg = '0')"; }*/
    
        { return @"SELECT DISTINCT dbo.RvtTRANCOMPLT.lot_no,dbo.RvtTRANH.start_dt,dbo.RvtTRANH.complt_dt
                                                    FROM dbo.RvtTRANH with(nolock) INNER JOIN
                                                            dbo.RvtTRANCOMPLT with(nolock) ON dbo.RvtTRANH.mnfctrsl_no = dbo.RvtTRANCOMPLT.mnfctrsl_no
                                                    WHERE (dbo.RvtTRANCOMPLT.lot_no like @lotno) AND (dbo.RvtTRANH.del_fg = '0') AND (dbo.RvtTRANCOMPLT.del_fg = '0')"; }
        public TPMachineInfo2(LSETInfo lsetInfo) : base(lsetInfo)
        {
		}

		~TPMachineInfo2()
		{
		}

        protected override NascaTranInfo GetNascaTranInfo(int linecd, string sql, string lotno)
        {
            var nascatraninfo = new NascaTranInfo();
            nascatraninfo = ConnectDB.GetNascaTran(this.LineCD, SQL(), lotno + "%");
            return nascatraninfo;
        }
    }
}
//-->後工程合理化/エラー集計