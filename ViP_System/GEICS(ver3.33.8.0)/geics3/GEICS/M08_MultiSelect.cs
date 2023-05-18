using System;
using SLCommonLib.Commons;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace GEICS
{
    public partial class M08_MultiSelect : Form
    {
        public List<string> ItemList { get; set; }
        public List<string> SelectItemList { get; set; }
        private NameValueCollection NVC { get; set; }
        private bool UseNVC;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="selectItemList"></param>
        public M08_MultiSelect(List<string> itemList, List<string> selectItemList)
        {
            InitializeComponent();

            ItemList = itemList;
            SelectItemList = selectItemList;

            UseNVC = false;
            NVC = new NameValueCollection();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="selectItemList"></param>
        public M08_MultiSelect(NameValueCollection nVC, List<string> selectItemList)
        {
            InitializeComponent();

            ItemList = new List<string>();
            SelectItemList = selectItemList;

            UseNVC = true;
            NVC = nVC;
        }

        /// <summary>
        /// データを戻す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDataReturn_Click(object sender, EventArgs e)
        {
			grdItems.EndEdit();
            SelectItemList 
                = ((SortableBindingList<MultiSelect>)bsItems.DataSource)
                .Where(m => m.SelectFG)
                .Select(m => m.ItemNM).ToList();

            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M08_MultiSelect_Load(object sender, EventArgs e)
        {
            List<MultiSelect> multiList = new List<MultiSelect>();
            if (!UseNVC)
            {
                foreach (string item in ItemList)
                {
                    MultiSelect multi = new MultiSelect(false, item);
                    multiList.Add(multi);
                }
            }
            else 
            {
                grdItems.Columns["ItemNM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                grdItems.Columns["ItemNM"].Width = 100;
                grdItems.Columns["ItemNM2"].Visible = true;

                foreach (string item in NVC.AllKeys)
                {
                    MultiSelect multi = new MultiSelect(false, item, NVC[item]);
                    multiList.Add(multi);
                }
            }
            foreach (string select in SelectItemList)
            {
                int targetIndex = multiList.FindIndex(m => m.ItemNM == select);
                multiList[targetIndex].SelectFG = true;
            }
            bsItems.DataSource = multiList.OrderByDescending(m => m.SelectFG).ToSortableBindingList();
        }

        /// <summary>
        /// 選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) 
            {
                return;
            }

            DataGridViewCheckBoxCell selectCell = (DataGridViewCheckBoxCell)grdItems.Rows[e.RowIndex].Cells["SelectFG"];
            if (Convert.ToBoolean(selectCell.Value))
            {
                selectCell.Value = false;
            }
            else 
            {
                selectCell.Value = true;
            }
        }

        /// <summary>
        /// 全選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllSelect_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow dr in grdItems.Rows)
            {
                dr.Cells["SelectFG"].Value = true;
            }
        }

        /// <summary>
        /// 全解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllCancel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                dr.Cells["SelectFG"].Value = false;
            }
        }

        /// <summary>
        /// 貼り付け
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPaste_Click(object sender, EventArgs e)
        {
            grdItems.EndEdit();

            string[] clipDatas = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (clipDatas.Count() == 0)
            {
                return;
            }

			List<MultiSelect> multiSelectList = new List<MultiSelect>();

			foreach (MultiSelect item in bsItems.List)
			{
				multiSelectList.Add(item);
			}

            foreach(string clipData in clipDatas)
            {
                if (string.IsNullOrEmpty(clipData)) { continue; }

				int recordRow = multiSelectList.FindIndex(m => m.ItemNM == clipData);

				if (recordRow >= 0)
				{
					multiSelectList[recordRow].SelectFG = true;
				}
            }

			bsItems.DataSource = multiSelectList.OrderByDescending(m => m.SelectFG).ToSortableBindingList();
			
        }
    }

    public class MultiSelect 
    {
        public MultiSelect(bool selectFG, string itemNM) 
        {
            this.SelectFG = selectFG;
            this.ItemNM = itemNM;
            this.ItemNM2 = itemNM;
        }
        public MultiSelect(bool selectFG, string itemNM, string itemNM2)
        {
            this.SelectFG = selectFG;
            this.ItemNM = itemNM;
            this.ItemNM2 = itemNM2;
        }


        public bool SelectFG { get; set; }

        public string ItemNM { get; set; }

        public string ItemNM2 { get; set; }
    }
}
