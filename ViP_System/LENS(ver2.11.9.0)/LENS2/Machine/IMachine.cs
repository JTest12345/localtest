using PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2.Machine
{
	public interface IMachine
	{
		/// <summary>
		/// 設備番号
		/// </summary>
		string NascaPlantCD { get; set; }

		/// <summary>
		/// 装置種類
		/// </summary>
		string ClassNM { get; set; }

		/// <summary>
		/// 号機名
		/// </summary>
		string MachineNM { get; set; }

		IPlc Plc { get; set; }

		string WatchingDirectoryPath { get; set; }

		string WorkStatus { get; set; }

		void MainWork();

		void InfoLog(string message);
		void InfoLog(string message, string lotNo);
		void InfoLog(string message, string lotNo, string magazineNo);
		void ExclamationLog(string message);
		void ExclamationLog(string message, string lotNo);
		void ExclamationLog(string message, string lotNo, string magazineNo);
		void ErrorLog(string message);
		void ErrorLog(string message, string lotNo);
		void ErrorLog(string message, string lotNo, string magazineNo);
	}
}
