using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEICS
{
	public partial class F100_MsgBox : Form
	{
		public F100_MsgBox(string msg)
		{
			InitializeComponent();

			tbMsgBox.Text = msg;
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			tbMsgBox.Text = string.Empty;
			this.Close();
			this.Dispose();
		}

		public static void Show(string msg)
		{
			F100_MsgBox msgBox = new F100_MsgBox(msg);

			msgBox.ShowDialog();
		}
	}
}
