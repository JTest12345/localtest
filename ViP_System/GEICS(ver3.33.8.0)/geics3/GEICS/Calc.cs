using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS
{
	class Calc
	{
		public static double Average(SortedList<int, QCLogData> cndDataItem)
		{
			List<double> ValueList = new List<double>();

			for (int i = 0; i < cndDataItem.Count; i++)
			{
				ValueList.Add(cndDataItem[i].Data);
			}

			return ValueList.Average();
		}
	}
}
