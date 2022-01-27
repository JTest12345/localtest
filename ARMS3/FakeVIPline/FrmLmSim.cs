using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi;
using ARMS3.Model;
using ARMS3.Model.PLC;
using System.Data.SqlClient;
using System.Threading;


namespace ARMS3.FakeVIPline
{
    public partial class FrmLmSim : Form
    {

        //RelayMachinePLC Plc = new RelayMachinePLC("192.168.1.100", 8501);
        //Keyence Plc = new Keyence("192.168.1.100", 8500);
        Keyence Plc = new Keyence("10.129.98.107", 8500);

        public FrmLmSim()
        {
            InitializeComponent();
            Plc.SetBit("B0", 1, "0");
            Plc.SetBit("DM0499", 1, "0");
        }

        private void btn_setdata_Click(object sender, EventArgs e)
        {
            var memaddtop = 500;
            var memstep = 100;

            try
            {
                for(int i=0; i < int.Parse(txt_bdvol.Text); i++ )
                {
                    var bdmemaddress = memaddtop + memstep * i;
                    //マガジンNo
                    Plc.SetString("DM" + (bdmemaddress).ToString(), txt_magno.Text);
                    //基板枚数
                    Plc.SetWordAsDecimalData("DM" + (bdmemaddress + 9).ToString(), (i+1));
                    //製品名
                    Plc.SetString("DM" + (bdmemaddress + 10).ToString(), txt_typecd.Text);
                    //基板部品コード
                    Plc.SetString("DM" + (bdmemaddress + 20).ToString(), txt_matcode.Text);
                    //基板ロットNo
                    Plc.SetString("DM" + (bdmemaddress + 30).ToString(), txt_matlot.Text);
                    //捺印コード
                    Plc.SetString("DM" + (bdmemaddress + 40).ToString(), "M***" + (i + 1).ToString());
                    //作業開始時間
                    var dtst = DateTime.Now;
                    Plc.SetString("DM" + (bdmemaddress + 50).ToString(), dtst.ToString("yyyyMMddhhmmss"));
                    Thread.Sleep(1000);
                    //作業終了時間
                    var dted = DateTime.Now;
                    Plc.SetString("DM" + (bdmemaddress + 60).ToString(), dted.ToString("yyyyMMddhhmmss"));
                }

                MessageBox.Show("完了しました");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            if (Plc.GetBit("B0") == "0")
            {
                Plc.SetBit("B0", 1, "1");
                //btn_upload.Text = "OFF";
            }
            else
            {
                Plc.SetBit("B0", 1, "0");
                //btn_upload.Text = "ON";
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Plc.GetBit("B0") == "0")
            {
                btn_upload.Text = "出力";
                txt_retcode.Text = Plc.GetBit("DM0499");
            }
            else
            {
                btn_upload.Text = "上位通信中";
            }
        }
    }
}
