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


namespace Make_QR
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

        BarcodeWriter writer = new BarcodeWriter();
        BarcodeFormat format = BarcodeFormat.QR_CODE;

        private void updateQrCode(string contents)
        {
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                //CharacterSet = "UTF-8",
                //CharacterSet = "ASCII",
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M,
                Height = height,
                Width = width,
                Margin = 0,
            };

            writer.Options = options;
            writer.Format = format;

            // string contents = "あいう漢字汉语\n123ABC";
            if (contents.Length > 0)
            {
                pictureBox1.Image = writer.Write(contents);
                //SendKeys.SendWait("^{PRTSC}");
                //画面のイメージデータは大きいため、
                //用がなくなればクリップボードから削除した方がいいかもしれない
                //Clipboard.SetDataObject(new DataObject());

                //削除してからQRをクリップボードへ貼付け
                //Clipboard.SetDataObject(pictureBox1.Image, true);
            }
            else
            {
                pictureBox1.Image = null;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string contents = ((TextBox)sender).Text;
            updateQrCode(contents);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string contents = textBox1.Text;
            if (contents != ""){

                //用がなくなればクリップボードから削除した方がいいかもしれない
                Clipboard.SetDataObject(new DataObject());

                //削除してからQRをクリップボードへ貼付け
                Clipboard.SetDataObject(pictureBox1.Image, true);
            }

            Form2 form2 = new Form2();
            form2.Show();

        }
    }

}

