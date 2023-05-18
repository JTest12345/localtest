using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using GEICS.Database;
using SLCommonLib.Commons;

namespace GEICS
{
	public partial class F81_MultiSelectParam : Form
	{
		private List<MultiSelectParam> SelectItemList { get; set; }
		private List<MultiSelectParam> CurrentSelectItemList { get; set; }
		private List<MultiSelectParam> GridItemList { get; set; }
		private List<Prm> ParamList { get; set; }

		public class MultiSelectParam
		{
			public bool SelectFG { get; set; }
			public string ServerNM { get; set; }
			public string TypeGroup { get; set; }
			public int QcParamNO { get; set; }
			public string ModelNM { get; set; }
			public string ClassNM { get; set; }
			public string ParameterNM { get; set; }

			public MultiSelectParam(bool selectFG, string serverNM, string typeGroup, int qcParamNO, string modelNM, string classNM, string parameterNM)
			{
				this.SelectFG = selectFG;
				this.ServerNM = serverNM;
				this.TypeGroup = typeGroup;
				this.QcParamNO = qcParamNO;
				this.ModelNM = modelNM;
				this.ClassNM = classNM;
				this.ParameterNM = parameterNM;
			}
		}

		public F81_MultiSelectParam(List<string> serverNMList, List<MultiSelectParam> selectItemList, List<string> modelNMList)
		{
			InitializeComponent();

			SelectItemList = selectItemList;
			CurrentSelectItemList = selectItemList;

			List<Prm> prmList = GetParamList(serverNMList, modelNMList);
			SetParamToGrid(prmList);

			cmbClassCond.Items.AddRange(prmList.Select(p => p.ClassNM).Distinct().ToArray());
			cmbParamCond.Items.AddRange(prmList.Select(p => p.QcParamNO.ToString()).Distinct().ToArray());
			cmbParamNMCond.Items.AddRange(prmList.Select(p => p.ParameterNM).Distinct().ToArray());
		}

		public List<MultiSelectParam> GetMultiSelectParam()
		{
			return CurrentSelectItemList;
		}

		private List<Prm> GetParamList(List<string> serverNMList, List<string> modelNMList)
		{
            List<Prm> retv = new List<Prm>();

            List<Plm> plmList = new List<Plm>();
			foreach (string serverNM in serverNMList)
			{
				string connStr = ConnectQCIL.GetQCILConnStrFromServerNM(serverNM);

				if (modelNMList == null || modelNMList.Count == 0)
				{
                    //選択されたサーバのパラメータを全取得
                    plmList = Plm.GetDatas(connStr, null, serverNM);
				}
				else
				{
					foreach (string modelNM in modelNMList)
					{
                        List<Plm> list = Plm.GetDatas(connStr, modelNM, serverNM);
                        plmList.AddRange(list);
                    }
				}
			}

            var prmList = plmList.Select(p => new { p.QcParamNO, p.ModelNM, p.ClassNM, p.ParameterNM, p.ServerNM, p.TypeGroup }).Distinct();
            foreach(var prm in prmList)
            {
                Prm p = new Prm();
                p.QcParamNO = prm.QcParamNO;
                p.ModelNM = prm.ModelNM;
                p.ClassNM = prm.ClassNM;
                p.ParameterNM = prm.ParameterNM;
                p.ServerNM = prm.ServerNM;
                p.TypeGroup = prm.TypeGroup;
                retv.Add(p);
            }
            this.ParamList = retv;

            return retv;
		}

		/// <summary>
		/// パラメタをグリッドへ表示
		/// </summary>
		/// <param name="setTargetPrmList"></param>
		private void SetParamToGrid(List<Prm> setTargetPrmList)
		{
			List<MultiSelectParam> multiSelectParamList = new List<MultiSelectParam>();

			//品種グループとパラメータが同一のレコードは重複除去
			for (int index = 0; index < multiSelectParamList.Count - 1; index++)
			{
				for (int targetIndex = index; targetIndex < multiSelectParamList.Count; targetIndex++)
				{
					if ((multiSelectParamList[index].TypeGroup == multiSelectParamList[targetIndex].TypeGroup) &&
						(multiSelectParamList[index].QcParamNO == multiSelectParamList[targetIndex].QcParamNO))
					{
						multiSelectParamList.RemoveAt(targetIndex);
					}
				}
			}

			//グリッド表示用のデータへ変換
			foreach (Prm prm in setTargetPrmList)
			{
				multiSelectParamList.Add(new MultiSelectParam(false, prm.ServerNM, prm.TypeGroup, prm.QcParamNO, prm.ModelNM, prm.ClassNM, prm.ParameterNM));
			}
			if (SelectItemList != null)
			{
				SetSelectStateFromSelectedItem(multiSelectParamList);
			}

			GridItemList = multiSelectParamList;

			//品種、装置種別、パラメータNO順にソートしてグリッドへ表示
			bsItems.DataSource = GridItemList
				.OrderBy(g => g.TypeGroup)
				.ThenBy(g => g.ModelNM)
				.ThenBy(g => g.ClassNM)
				.ThenBy(g => g.QcParamNO).ToSortableBindingList();

		}

		/// <summary>
		/// フォームが再度開かれた時などの選択状態の再現
		/// </summary>
		/// <param name="multiSelectParamList"></param>
		private void SetSelectStateFromSelectedItem(List<MultiSelectParam> multiSelectParamList)
		{
			foreach (MultiSelectParam selectItem in SelectItemList)
			{
				if (multiSelectParamList.Exists(m => m.TypeGroup == selectItem.TypeGroup) && (multiSelectParamList.Exists(m => m.QcParamNO == selectItem.QcParamNO)))
				{
					multiSelectParamList.Single(m => m.TypeGroup == selectItem.TypeGroup && m.QcParamNO == selectItem.QcParamNO).SelectFG = true;
				}
			}

			//if (!multiSelectParamList.Exists(m => m.SelectFG == true))
			//{
			//    SelectItemList = new List<MultiSelectParam>();
			//}
		}

		/// <summary>
		/// パラメータ全選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolAllSelect_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

			for(int i = 0; i < GridItemList.Count; i++)
			{
				GridItemList[i].SelectFG = true;
			}
			bsItems.DataSource = GridItemList.ToSortableBindingList();
			grdItems.Refresh();

			//UpdateSelectItemList();
		}

		/// <summary>
		/// パラメータ選択全解除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolAllCansel_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

			for (int i = 0; i < GridItemList.Count; i++)
			{
				GridItemList[i].SelectFG = false;
			}
			bsItems.DataSource = GridItemList.ToSortableBindingList();
			grdItems.Refresh();

			//UpdateSelectItemList();
		}

		/// <summary>
		/// データを戻す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolReturnData_Click(object sender, EventArgs e)
		{
			//List<MultiSelectParam> selectParamList =((SortableBindingList<MultiSelectParam>)bsItems.DataSource).Where(m => m.SelectFG).ToList();

			//if (selectParamList.Count > 256 - ExcelControl.TPL02_HEADER_COL + 1)
			//{
			//    MessageBox.Show("Excelの出力列上限に達する為、パラメータ数を減らして下さい。");
			//    return;
			//}

			UpdateSelectItemList();

			//SelectItemList = ((SortableBindingList<MultiSelectParam>)bsItems.DataSource).Where(m => m.SelectFG).ToList();

			CurrentSelectItemList = SelectItemList;

			this.Close();
			this.Dispose();
		}

		/// <summary>
		/// 裏で保持している選択アイテムリスト(SelectItemList)の状態を更新
		/// </summary>
		private void UpdateSelectItemList()
		{
			grdItems.EndEdit();

			////グリッドに表示されたアイテムがSelectItemListに無ければ追加。
			//foreach (MultiSelectParam bsItem in ((SortableBindingList<MultiSelectParam>)bsItems.DataSource).Where(b => b.SelectFG == true))
			//{
			//    int index = SelectItemList.FindIndex(s => s.ClassNM == bsItem.ClassNM
			//        && s.ModelNM == bsItem.ModelNM
			//        && s.ParameterNM == bsItem.ParameterNM
			//        && s.QcParamNO == bsItem.QcParamNO
			//        && s.ServerNM == bsItem.ServerNM
			//        && s.TypeGroup == bsItem.TypeGroup);

			//    if (index < 0)
			//    {
			//        SelectItemList.Add(bsItem);
			//    }
			//}

			//既にSelectItemListに有れば、グリッド上の選択状態で更新する。
			foreach (MultiSelectParam bsItem in ((SortableBindingList<MultiSelectParam>)bsItems.DataSource))
			{
				MultiSelectParam checkItem = bsItem;

				int index = SelectItemList.FindIndex(s => s.ClassNM == bsItem.ClassNM
					&& s.ModelNM == bsItem.ModelNM
					&& s.ParameterNM == bsItem.ParameterNM
					&& s.QcParamNO == bsItem.QcParamNO
					&& s.ServerNM == bsItem.ServerNM
					&& s.TypeGroup == bsItem.TypeGroup);

				if (index >= 0 && bsItem.SelectFG == true)
				{
					SelectItemList[index].SelectFG = true;
				}
				else if (index >= 0 && bsItem.SelectFG == false)
				{
					SelectItemList.RemoveAt(index);
				}
				else if (index < 0 && bsItem.SelectFG == true)
				{
					SelectItemList.Add(bsItem);
				}
			}
		}

		private void GetDataByCond()
		{
			string sParamCD = cmbParamCond.Text;
			string sParamNM = cmbParamNMCond.Text;
			string sClass = cmbClassCond.Text;

			UpdateSelectItemList();

			List<Prm> setTargetParamList = ParamList
				.FindAll(p => p.ClassNM.Contains(sClass))
				.FindAll(p => p.ParameterNM.Contains(sParamNM))
				.FindAll(p => p.QcParamNO.ToString().Contains(sParamCD));

			SetParamToGrid(setTargetParamList);
		}

		private void cmbClassCond_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GetDataByCond();
			}
		}

		private void cmbParamCond_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GetDataByCond();
			}
		}

		private void cmbParamNMCond_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GetDataByCond();
			}
		}

		private void grdItems_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				foreach (DataGridViewRow row in grdItems.SelectedRows)
				{
					row.Cells["SelectFG"].Value = !(bool)row.Cells["SelectFG"].Value;
				}
			}
		}

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
	}
}
