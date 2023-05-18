using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LENS2
{
	public partial class F02_Confirm : Form
	{
		/// <summary>認証完了フラグ</summary>
		public bool IsCompletedConfirm { get; set; }

		public F02_Confirm()
		{
			InitializeComponent();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (txtPassword.Text.ToUpper() == "CLOSE")
			{
				IsCompletedConfirm = true;

				this.Close();
				this.Dispose();
			}
			else
			{
				txtPassword.Text = string.Empty;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

	}
}
