using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EICS
{
	public partial class F99_MustConfirmMsg : Form
	{
		public DialogResult dialogResult = new DialogResult();
		public string Message { get; set; }

		public F99_MustConfirmMsg(string msg)
		{
			InitializeComponent();

			Message = msg;
		}

		public DialogResult GetButtonResult()
		{
			return dialogResult;
		}

		private void F99_MustConfirmMsg_Load(object sender, EventArgs e)
		{
			textBox.Text = Message;
		}

		private void btnConfirmed_Click(object sender, EventArgs e)
		{
			dialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			dialogResult = DialogResult.Cancel;
			this.Close();
		}


	}
}
