using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VSPMasterMaintenance
{
    public partial class Top : Form
    {
        public Top()
        {
            InitializeComponent();
        }

        private void MakeNewType_Click(object sender, EventArgs e)
        {
            MakeNewType MNT = new MakeNewType();
            MNT.Show();
        }

        private void MakeNewWork_Click(object sender, EventArgs e)
        {
            MakeNewWork MNW = new MakeNewWork();
            MNW.Show();
        }

        private void MakeNewMachine_Click(object sender, EventArgs e)
        {
            MakeNewMachine MNM = new MakeNewMachine();
            MNM.Show();
        }
    }
}
