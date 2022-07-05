using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.Drawing.Text;

namespace JSLabelPrint
{
    class qrcoder
    {

        static private Image _img;
        //static string title = "先行対応品(計画外)";
        //static string dt = "2022/06/06 9:00"
        //static string vlot = "V22-00000020";
        //static string vlot_BTC = "V22-00000020,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002,CU0309-1206E1W11-00E-6212,720220603140000002";

        static int Main(string[] args)
        {
            string title, vlot, vlot_BTC, printer;
            string[] dt = { "", "" };
            if (args[0] == "")
            { //タイトル
                throw new Exception();
            }
            else
            {
                title = args[0];
            }
            if (args[1] == "")
            { //発行日時
                throw new Exception();
            }
            else
            {
                dt = args[1].Split (' ');
            }
            if (args[2] == "")
            { //VLOT
                throw new Exception();
            }
            else
            {
                vlot = args[2];
            }
            if (args[3] == "")
            { //BTC
                throw new Exception();
            }
            else
            {
                vlot_BTC = args[3];
            }
            if (args[4] == "")
            { //BTC
                throw new Exception();
            }
            else
            {
                printer= args[4];
            }

            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();

                //VLOT QRCD
                QRCodeData qrCodeData_VLOT = qrGenerator.CreateQrCode(vlot, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode_VLOT = new QRCode(qrCodeData_VLOT);
                Bitmap qrCodeImage_VLOT = qrCode_VLOT.GetGraphic(2);

                //背景イメージ作成(バーコードイメージの余白になるように大きめにする。)
                Image backImg = new Bitmap(300, 160, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(backImg);
                Brush b = new SolidBrush(Color.Black);
                Font f = new Font("メイリオ", 8);
                StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                //レイアウト
                g.Clear(Color.White);
                //タイトル
                g.DrawString($"【{title}】", f, b, 10, 40, sf);
                //発行日時
                g.DrawString($"発行：{dt[0]}", f, b, 15, 55, sf);
                g.DrawString($"{dt[1]}", f, b, 50, 68, sf);
                //VLOT
                g.DrawString("VLOT NO.", f, b, 15, 85, sf);
                g.DrawString($"{vlot}", f, b, 50, 98, sf);
                g.DrawImage(qrCodeImage_VLOT, 20, 95);   //バーコードを貼り付け
                //BTC バリ取り設備読込用QR
                if (vlot_BTC != "null")
                {
                    QRCodeData qrCodeData_BTC = qrGenerator.CreateQrCode(vlot_BTC, QRCodeGenerator.ECCLevel.H);
                    QRCode qrCode_BTC = new QRCode(qrCodeData_BTC);
                    Bitmap qrCodeImage_BTC = qrCode_BTC.GetGraphic(1);
                    g.DrawImage(qrCodeImage_BTC, 170, 25);   //バーコードを貼り付け
                    //g.DrawString("設備読込用", f, b, 180, 160, sf);  //バーコード文字を下部に配置
                    qrCodeImage_BTC.Dispose();
                }

                g.Dispose();
                qrCodeImage_VLOT.Dispose();
                
                _img = backImg;

                //PrintDocumentを初期化し、プリントイベントを紐付ける。
                var pd = new System.Drawing.Printing.PrintDocument();
                pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
                pd.PrinterSettings.PrinterName = printer;
                //pd.PrinterSettings.PrinterName = "P0364_オプト技術_7-1F_1";
                //pd.PrinterSettings.PrinterName = "CubePDF";
                //pd.PrinterSettings.PrinterName = "TEC B-EX4T1-T"; 
                pd.Print();
                pd.PrintPage -= pd_PrintPage;

                backImg.Dispose();

                return 0;
            }
            catch(Exception e)
            {
                return 1;
            }

        }

        //PrintDocument.Print();により、以下が実行される。
        private static void pd_PrintPage(Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //ImageをeのGraphicsに当てたら、印刷された。
            e.Graphics.DrawImage(_img, 0, 0);
        }
    }
}
