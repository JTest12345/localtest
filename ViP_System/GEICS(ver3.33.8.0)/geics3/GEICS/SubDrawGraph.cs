using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace GEICS
{
    public partial class SubDrawGraph : Form
    {
        Chart _chart = null;//関連項目
        DataTable _table = new DataTable();
        DataGridView gvDataList = new DataGridView();
        F03_TrendChart _parent = null;

        public SubDrawGraph(Chart chart, F03_TrendChart parent)
        {

            InitializeComponent();
            _chart = chart;
            _table = (DataTable)_chart.DataSource;
            _parent = parent;
        }

        private void SubDrawGraph_Load(object sender, EventArgs e)
        {
            //2011/07/28 HIshiguchi 凡例に表示する系列が多い場合の為、グラフ範囲いっぱいに表示できるように設定しておく。
            this._chart.Legends[0].MaximumAutoSize = 100;
            //--------------------------------------------
            this.gvDataList.Sorted += new EventHandler(gvDataList_Sorted);

            this.splitContainer1.Panel1.Controls.Add(this._chart);
            this._chart.Dock = System.Windows.Forms.DockStyle.Fill;

            this.gvDataList.DataSource = _table;
            this.gvDataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvDataList.Sorted += new EventHandler(gvDataList_Sorted);
            this.splitContainer1.Panel2.Controls.Add(this.gvDataList);

            //2012/02/13 必要無しとの事で列を非表示
            if (this.gvDataList.Columns.Count != 0)
            {
                this.gvDataList.Columns[8].Visible = false;
            }

            //if (_parent == null) 
            //{
            //    toolLotInfo.Visible = false;
            //}

            //2011/07/28 HIshiguchi 上限値、下限値の判定
            dataJudge();
            //------------------------------------------
        }

        private void gvDataList_Sorted(object sender, EventArgs e)
        {
            dataJudge();
        }

        /// <summary>
        /// 上限、下限値の判定
        /// </summary>
        private void dataJudge()
        {
            foreach (DataGridViewRow dr in gvDataList.Rows)
            {
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    //号機列以外は次へ
                    if (!Regex.IsMatch(dc.OwningColumn.Name, "^*号機*"))
                    {
                        continue;
                    }

                    //リミット値を格納
                    double upperVAL, lowerVAL;
                    if (Convert.ToDouble(dr.Cells["QUCL"].Value) == 9999
                        && Convert.ToDouble(dr.Cells["QLCL"].Value) == 9999)
                    {
                        //<--[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                        //upperVAL = Convert.ToDouble(dr.Cells["上限値"].Value);
                        //lowerVAL = Convert.ToDouble(dr.Cells["下限値"].Value);
                        upperVAL = Convert.ToDouble(dr.Cells[2].Value);//規格上限値
                        lowerVAL = Convert.ToDouble(dr.Cells[3].Value);//規格下限値
                        //-->[BTS1457]上限値/下限値→規格上限値/規格下限値 2011/11/16 Y.Matsushima
                    }
                    else
                    {
                        upperVAL = Convert.ToDouble(dr.Cells["QUCL"].Value);
                        lowerVAL = Convert.ToDouble(dr.Cells["QLCL"].Value);
                    }

                    //リミット超え行の背景色を変更
                    double pointVAL = Convert.ToDouble(dc.Value);
                    if (pointVAL == double.NaN)
                    {
                        continue;
                    }

                    if (pointVAL > upperVAL)
                    {
                        dr.DefaultCellStyle.BackColor = Color.Salmon;
                    }
                    if (lowerVAL != double.NaN && pointVAL < lowerVAL)
                    {
                        dr.DefaultCellStyle.BackColor = Color.LightBlue;
                    }
                }
            }
        }

        /// <summary>
        /// ロット情報表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolLotInfo_Click(object sender, EventArgs e)
        {
            frmLotInfo formLotInfo = null;
            if (_parent != null)
            {
                formLotInfo = new frmLotInfo(_parent);
            }
            else
            {
                List<string> lotList = new List<string>();
                foreach (DataGridViewRow dr in gvDataList.Rows)
                {
                    lotList.Add(Convert.ToString(dr.Cells[0].Value));
                }

                formLotInfo = new frmLotInfo(lotList);
            }
            formLotInfo.Show();
        }
    }
}
