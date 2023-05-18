using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace EICS
{
	class MustConfirmMsgBox
	{
		public static DialogResult Show(string msg)
		{
			F99_MustConfirmMsg msgBox = new F99_MustConfirmMsg(msg);

			msgBox.StartPosition = FormStartPosition.CenterScreen;

			msgBox.ShowDialog();

			return msgBox.GetButtonResult();
		}
	}
}
