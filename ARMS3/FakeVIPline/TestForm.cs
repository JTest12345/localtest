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
//using ArmsApi.Model.FORMS;


namespace ARMS3.FakeVIPline
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            // LOTチェック～インサート
            //
            if (ArmsApi.Model.AsmLot.GetAsmLot(textBox2.Text) != null)
            {
                MessageBox.Show("指定のロットNOは登録済みです");
                return;
            }

            var lot = new ArmsApi.Model.AsmLot()
            {
                TypeCd = comboBox1.Text,
                NascaLotNo = textBox2.Text,
                TempLotNo = textBox2.Text,
                ProfileId = int.Parse(textBox3.Text),
                BlendCd = textBox4.Text,
                ResinGpCd = new List<string>{ textBox5.Text },
                CutBlendCd = "NA",
                IsWarning = false,
                IsRestricted = false,
                IsLifeTest = false,
                IsKHLTest = false,
                IsColorTest = false,
                IsTemp = false,
                IsNascaLotCharEnd = false,
                MacGroup = new List<string>{ "1" },
                IsBadMarkFrame = false,
                IsReflowTest = false,
                IsReflowTestWirebond = false,
                IsElasticityTest = false,
                IsDieShearLot = false,
                MoveStockCt = 0,
                BeforeLifeTestCondCd = "",
                TempCutBlendId = 0,
                TempCutBlendNo = ""
            };

            lot.Update();


            //
            // MAGをインサート
            //
            ArmsApi.Model.Process fp;
            ArmsApi.Model.Magazine mag;

            mag = ArmsApi.Model.Magazine.GetCurrent(textBox6.Text);
            if (mag != null)
            {
                mag.NewFg = false;
                mag.Update();
            }

            fp = ArmsApi.Model.Process.GetFirstProcess(comboBox1.Text);
            mag = new ArmsApi.Model.Magazine()
            {
                MagazineNo = textBox6.Text,
                NascaLotNO = textBox2.Text,
                NowCompProcess = fp.ProcNo,
                NewFg = true,
                //FrameQty = int.Parse(textBox7.Text),
                LastUpdDt = DateTime.Now
            };

            mag.Update();


            //
            // 帳票をインサート
            //
            //var msg = ProccessForms.InsertProcForms(comboBox1.Text, textBox2.Text);
            //MessageBox.Show(msg); 

            MessageBox.Show("Done");
        }
    }
}
