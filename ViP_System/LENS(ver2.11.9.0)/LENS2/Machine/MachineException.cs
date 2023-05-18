using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2.Machine
{
	public class MachineException : ApplicationException
	{
		public MachineException(string classnm, string machinenm, ApplicationException err) : base(err.Message, err)
		{
			Log.Error(classnm, machinenm, err.Message, true);
		}

		public MachineException(string classnm, string machinenm, Exception err)
			: base(err.Message, err)
		{
			Log.Error(classnm, machinenm, err.Message, true);
		}
	}
}
