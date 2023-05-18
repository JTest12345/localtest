using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;


namespace MacMnt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        //BarcodeWriter writer = new BarcodeWriter();
        //BarcodeFormat format = BarcodeFormat.QR_CODE;

    
        private void MacMnt_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();

        }

        private void MFI_Click(object sender, EventArgs e)
        {
            MacFailInput MFI = new MacFailInput();
            MFI.Show();
        }

        private void MacFailRead_Click(object sender, EventArgs e)
        {
            MacFailRead MFR = new MacFailRead();
            MFR.Show();
        }
    }

}

