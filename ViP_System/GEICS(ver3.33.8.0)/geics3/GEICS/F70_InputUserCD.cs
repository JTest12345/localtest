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
	public partial class F70_InputUserCD : Form
	{
		private string UserCD;

		public F70_InputUserCD()
		{
			InitializeComponent();
		}

		public string GetInputUserCD()
		{
			return UserCD;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				int testNum;
				if (int.TryParse(txtUserCD.Text, out testNum))
				{
					UserCD = txtUserCD.Text;
				}
				else
				{
					throw new ApplicationException("入力に誤りがあります。(数値のみ入力可)");
				}
			}
			catch (Exception err)
			{
				throw err;
			}
			finally
			{
				this.Close();
			}
		}
	}
}
