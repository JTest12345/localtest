using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    public partial class FrmLotProgress : Form
    {
        public FrmLotProgress()
        {
            InitializeComponent();
        }

        private void FrmLotProgress_Load(object sender, EventArgs e)
        {
            List<AsmLot> lots = new List<AsmLot>();

            Magazine[] mags = Magazine.GetMagazine(true);
            foreach(Magazine mag in mags)
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                lots.Add(lot);
            }

            var groupLots = lots
                .GroupBy(l => l.DBThrowDT)
                .Select(l => new { DBThrowDT = l.Key, LotCT = l.Count() })
                .OrderBy(l => l.DBThrowDT);
            
            List<Item> items = new List<Item>();
            foreach(var groupLot in groupLots)
            {
                Item i = new Item();
                i.DBThrowDT = groupLot.DBThrowDT;
                i.LotCT = groupLot.LotCT;

                items.Add(i);
            }

            bsItems.DataSource = items;
        }
    }

    public class Item
    {
        public string DBThrowDT { get; set; }
        public int LotCT { get; set; }
    }
}
