using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
	public class GrinderRingModel
	{
		//public string LotNo { get; set; }
		public int ProcNo { get; set; }
		public string GrinderRingNo { get; set; }
		public List<string> RingNoList { get; set; }
		public string EmpCd { get; set; }


		public GrinderRingModel(string empcd)
		{
			RingNoList = new List<string>();
			this.EmpCd = empcd;		
		}
	}
}