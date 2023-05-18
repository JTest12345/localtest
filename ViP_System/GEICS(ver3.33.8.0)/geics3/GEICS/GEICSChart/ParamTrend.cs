using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS.GEICSChart
{
	class ParamTrend
	{
		public string LotNo { get; set; }
		public double Ave { get; set; }
		public double UpperLimit { get; set; }
		public double LowerLimit { get; set; }
		public double QcUpperLimit { get; set; }
		public double QcLowerLimit { get; set; }
		public double reserve1 { get; set; }
		public double InnerUpperLimit { get; set; }
		public double InnerLowerLimit { get; set; }



		//    //データの容れ物作成
		//    DataTable table = new DataTable();
		//    DataColumn col0 = new DataColumn(Constant.CHART_X, typeof(string));
		//    table.Columns.Add(col0);
		//    DataColumn col1 = new DataColumn(Constant.CHART_CL_NM, typeof(double));//平均値
		//    table.Columns.Add(col1);
		//    //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
		//    //DataColumn col2 = new DataColumn(Constant.CHART_UCL_NM, typeof(double));//上限値
		//    DataColumn col2 = new DataColumn(sMAX, typeof(double));//上限値
		//    table.Columns.Add(col2);
		//    //DataColumn col3 = new DataColumn(Constant.CHART_LCL_NM, typeof(double));//下限値
		//    DataColumn col3 = new DataColumn(sMIN, typeof(double));//下限値
		//    //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
		//    table.Columns.Add(col3);
		//    DataColumn col4 = new DataColumn(Constant.CHART_EQUINO_ALL, typeof(double));
		//    table.Columns.Add(col4);
		//    DataColumn col5 = new DataColumn(Constant.CHART_FLAG_LOT, typeof(double));
		//    table.Columns.Add(col5);
		//    //★☆
		//    DataColumn col6 = new DataColumn(Constant.ENUM_CHART_LineKind.QUCL.ToString(), typeof(double));
		//    table.Columns.Add(col6);
		//    DataColumn col7 = new DataColumn(Constant.ENUM_CHART_LineKind.QLCL.ToString(), typeof(double));
		//    table.Columns.Add(col7);


		//    //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
		//    DataColumn col8 = new DataColumn(wPrmAddInfo.Info2, typeof(double));
		//    table.Columns.Add(col8);
		//    //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima


		//    DataColumn col9 = new DataColumn(Constant.CHART_INNER_UPPER_LIMIT_NM, typeof(double));
		//    table.Columns.Add(col9);
		//    DataColumn col10 = new DataColumn(Constant.CHART_INNER_LOWER_LIMIT_NM, typeof(double));
		//    table.Columns.Add(col10);
	}
}
