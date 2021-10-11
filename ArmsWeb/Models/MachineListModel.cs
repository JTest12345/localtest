using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
	public class MachineListModel
	{
		public MachineListModel()
		{
			try
			{
				this.mList = new List<MachineInfo>();
				// 装置リスト取得
				MachineInfo[] macineList = MachineInfo.GetMachineList(false);

				this.mList.AddRange(macineList);
			}
			catch (Exception ex)
			{
				this.ErrMsg = ex.ToString();
			}
		}

		/// <summary>
		/// 装置一覧
		/// </summary>
		public List<MachineInfo> mList { get; set; }

		public string ErrMsg { get; set; }

	}
}