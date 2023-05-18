using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;
using ArmsApi.Model.NASCA;

namespace ArmsMaintenance
{
    public partial class FrmDebug : Form
    {
        public FrmDebug()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(this.textBox1.Text) == true)
            {
                return;
            }


            InlineAPI api = new InlineAPI();
            ArmsApiResponse ret = api.NelInput(this.textBox1.Text);

            this.textBox2.Text = ret.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Exporter exp = Exporter.GetInstance(true);

            AsmLot lot = AsmLot.GetAsmLot("N122OCS0348");
            exp.SendAsmLotAllProc(lot, false);

            //this.textBox2.Text = Config.GetARMSConStr("#3");
        }
    }
}
