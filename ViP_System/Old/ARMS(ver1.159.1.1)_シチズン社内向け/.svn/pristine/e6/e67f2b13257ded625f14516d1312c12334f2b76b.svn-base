using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ArmsApi.Model;
using ArmsWorkTransparency.Common;
using System.Windows.Controls;

namespace ArmsWorkTransparency.ViewModels
{
	public class LotMonitorSettingViewModel : INotifyPropertyChanged
	{
        #region プロパティ

        public ItemCollection TypeSelectedList { get; set; }
        public System.Collections.IList CurrentProcSelectedList { get; set; }
        public System.Collections.IList NextProcSelectedList { get; set; }
        public int UpdateInterval { get; set; }
        public bool ResinGroupVisible { get; set; }
        public bool SupplyIdVisible { get; set; }
        public bool BlendCdVisible { get; set; }
        public bool MagazineNoVisibleVisible { get; set; }
        public bool DBThrowDtVisible { get; set; }
        public bool AimRankVisible { get; set; }
        public bool PhosphorSheetMatCdVisible { get; set; }
        public bool NextLineVisible { get; set; }
        public bool ReachingDateVisible { get; set; }
        public bool ReachingMinutesVisible { get; set; }


        #endregion

        public class Proc
		{
			public int No { get; set; }
			public string Name { get; set; }
			public bool IsSelected { get; set; }

			public Proc(int no, string name, bool isSelected)
			{
				this.No = no;
				this.Name = name;
				this.IsSelected = isSelected;
			}
		}
		private List<Proc> currentProcList;
		public List<Proc> CurrentProcList
		{
			get { return currentProcList = currentProcList ?? new List<Proc>(); }
			set
			{
				currentProcList = value;
				NotifyPropertyChanged(nameof(CurrentProcList));
			}
		}
		private List<Proc> nextProcList;
		public List<Proc> NextProcList
		{
			get { return nextProcList = nextProcList ?? new List<Proc>(); }
			set
			{
				nextProcList = value;
				NotifyPropertyChanged(nameof(NextProcList));
			}
		}

        private List<string> searchTypeList;
        public List<string> SearchTypeList
        {
            get { return searchTypeList = searchTypeList ?? new List<string>(); }
            set
            {
                searchTypeList = value;
                NotifyPropertyChanged(nameof(SearchTypeList));
            }
        }

        public LotMonitorSettingViewModel()
		{
		}

		private void LoadProcNoList()
		{
            currentProcList = new List<Proc>();
            nextProcList = new List<Proc>();

            int[] procNoList = Process.GetWorkFlowProcNo();
            procNoList = procNoList.OrderBy(p => p).ToArray();
            foreach (int procNo in procNoList)
            {
                Process proc = Process.GetProcess(procNo);

            	currentProcList.Add(new Proc(proc.ProcNo, proc.InlineProNM, false));
            	nextProcList.Add(new Proc(proc.ProcNo, proc.InlineProNM, false));
            }

            applySettingsToProcList();
		}

        private void LoadTypeCdList()
        {
            string[] typeArray = Process.GetWorkFlowTypeList();

            searchTypeList = new List<string>();

            foreach (string type in typeArray)
            {
                searchTypeList.Add(type);
            }
        }

        private void applySettingsToProcList()
		{
			StringCollection sc = Properties.Settings.Default.TargetCurrentProcessList;
            if (sc == null) return;

            foreach (string currentProcStr in sc)
			{
				int currentProcNo = int.Parse(currentProcStr);
				Proc proc = currentProcList.Find(p => p.No == currentProcNo);

				if (proc == null) continue;

				proc.IsSelected = true;
			}

			sc = Properties.Settings.Default.TargetNextProcessList;

			foreach (string nextProcStr in sc)
			{
				int nextProcNo = int.Parse(nextProcStr);
				Proc proc = nextProcList.Find(p => p.No == nextProcNo);

				if (proc == null) continue;

				proc.IsSelected = true;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}

		public void LoadSetting()
		{
            LoadProcNoList();

            LoadTypeCdList();
        }

        //public void SaveSetting(ItemCollection typeSelectedList, System.Collections.IList currentProcSelectedList, System.Collections.IList nextProcSelectedList, string updateInterval, bool supplyIdVisible, bool blendCdVisible)
        public void SaveSetting()
        {
			StringCollection sc = new StringCollection();

            //foreach(string type in typeSelectedList)
            foreach (string type in this.TypeSelectedList)
            {
                sc.Add(type);
            }
            Properties.Settings.Default.TargetTypeList = sc;

            sc = new StringCollection();
            //foreach (Proc currentProc in currentProcSelectedList)
            foreach (Proc currentProc in this.CurrentProcSelectedList)
            {
				sc.Add(currentProc.No.ToString());
			}
			Properties.Settings.Default.TargetCurrentProcessList = sc;
			
			sc = new StringCollection();
            //foreach (Proc nextProc in nextProcSelectedList)
            foreach (Proc nextProc in this.NextProcSelectedList)
            {
				sc.Add(nextProc.No.ToString());
			}
			Properties.Settings.Default.TargetNextProcessList = sc;

            //Properties.Settings.Default.LotMonitorUpdateMinutes = int.Parse(updateInterval);
            Properties.Settings.Default.LotMonitorUpdateMinutes = this.UpdateInterval;

            //Properties.Settings.Default.SupplyIdVisible = supplyIdVisible;
            //Properties.Settings.Default.BlendCdVisible = blendCdVisible;
            Properties.Settings.Default.ResinGroupVisible = this.ResinGroupVisible;
            Properties.Settings.Default.SupplyIdVisible = this.SupplyIdVisible;
            Properties.Settings.Default.BlendCdVisible = this.BlendCdVisible;
            Properties.Settings.Default.MagazineNoVisibleVisible = this.MagazineNoVisibleVisible;
            Properties.Settings.Default.DBThrowDtVisible = this.DBThrowDtVisible;
            Properties.Settings.Default.AimRankVisible = this.AimRankVisible;
            Properties.Settings.Default.PhosphorSheetMatCdVisible = this.PhosphorSheetMatCdVisible;
            Properties.Settings.Default.NextLineVisible = this.NextLineVisible;
            Properties.Settings.Default.ReachingDateVisible = this.ReachingDateVisible;
            Properties.Settings.Default.ReachingMinutesVisible = this.ReachingMinutesVisible;

            Properties.Settings.Default.Save();
			MessageBox.Show("設定の保存を完了しました。");
		}
	}
}
