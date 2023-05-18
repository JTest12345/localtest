using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GEICS.Database;
using System.Collections.Specialized;
using SLCommonLib.Commons;

namespace GEICS
{
	public partial class F80_MultiSelectPlant : Form
	{
		private List<MultiSelectPlant> SelectItemList { get; set; }
		private List<MultiSelectPlant> CurrentSelectItemList { get; set; }
		private List<MultiSelectPlant> GridItemList { get; set; }

		private List<Equi> EquipList { get; set; }



		public class MultiSelectPlant
		{
			public bool SelectFG { get; set; }
			public string ServerNM { get; set; }
			public string ModelNM { get; set; }
			public string AssetsNM { get; set; }
			public string MachineSeqNO { get; set; }
			public string EquipmentNO { get; set; }
			public bool DelFG { get; set; }

			public MultiSelectPlant(bool selectFG, string serverNM, string modelNM, string assetsNM, string machineSeqNO, string equipmentNO, bool delFG)
			{
				this.SelectFG = selectFG;
				this.ServerNM = serverNM;
				this.ModelNM = modelNM;
				this.AssetsNM = assetsNM;
				this.MachineSeqNO = machineSeqNO;
				this.EquipmentNO = equipmentNO;
				this.DelFG = DelFG;
			}
		}

		public F80_MultiSelectPlant(List<string> serverNMList, List<MultiSelectPlant> selectItemList, int? assetsCD)
		{
			InitializeComponent();

			SelectItemList = selectItemList;
			CurrentSelectItemList = selectItemList;

			GetPlant(serverNMList, assetsCD);
			SetPlantToGrid(EquipList);

			cmbAssetsNMCond.Items.AddRange(EquipList.Select(e => e.AssetsNM).Distinct().ToArray());
			cmbPlantCDCond.Items.AddRange(EquipList.Select(e => e.EquipmentNO).Distinct().ToArray());
		}

		public List<MultiSelectPlant> GetMultiSelectPlant()
		{
			return CurrentSelectItemList;
		}

		private void GetPlant(List<string> serverNMList, int? assetsCD)
		{
			EquipList = new List<Equi>();

			foreach (string serverNM in serverNMList)
			{
				string connStr = ConnectQCIL.GetQCILConnStrFromServerNM(serverNM);

				//選択されたサーバの設備を全取得
				EquipList.AddRange(Equi.GetEquipmentInfo(serverNM, connStr, assetsCD));
			}
		}

		/// <summary>
		/// グリッドへ設備を表示する
		/// </summary>
		/// <param name="setTargetEquiList"></param>
		private void SetPlantToGrid(List<Equi> setTargetEquiList)
		{
			List<MultiSelectPlant> multiSelectPlantList = new List<MultiSelectPlant>();
			
			//グリッド表示用のデータへ変換
			foreach (Equi equi in setTargetEquiList)
			{
				multiSelectPlantList.Add(new MultiSelectPlant(false, equi.ServerNM, equi.ModelNM, equi.AssetsNM, equi.MachineSeqNO, equi.EquipmentNO, equi.DelFG));				
			}
			if (SelectItemList != null)
			{
				SetSelectStateFromSelectedItem(multiSelectPlantList);
			}

			GridItemList = multiSelectPlantList;

			//装置種別、サーバ、号機順にソートしてグリッドへ表示
			bsItems.DataSource = GridItemList.OrderBy(m => m.AssetsNM).ThenBy(m => m.ServerNM).ThenBy(m => m.MachineSeqNO).ToSortableBindingList();

		}

		/// <summary>
		/// グリッドに表示されるmultiSelectPlantListに対して裏で保持している設備毎の選択状況を反映
		/// </summary>
		/// <param name="multiSelectPlantList"></param>
		private void SetSelectStateFromSelectedItem(List<MultiSelectPlant> multiSelectPlantList)
		{
			foreach (MultiSelectPlant selectItem in SelectItemList)
			{
				if (multiSelectPlantList.Exists(m => m.EquipmentNO == selectItem.EquipmentNO))
				{
					multiSelectPlantList.Single(m => m.EquipmentNO == selectItem.EquipmentNO).SelectFG = true;
				}
			}
		}

		/// <summary>
		/// 全選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolAllSelect_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

			for (int i = 0; i < GridItemList.Count; i++)
			{
				GridItemList[i].SelectFG = true;
			}
			bsItems.DataSource = GridItemList.ToSortableBindingList();
			grdItems.Refresh();
			//UpdateSelectItemList();
		}

		/// <summary>
		/// 全選択のキャンセル
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
		}

		/// <summary>
		/// データを戻す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolReturnData_Click(object sender, EventArgs e)
		{
			UpdateSelectItemList();

			CurrentSelectItemList = SelectItemList;

			this.Close();
			this.Dispose();
		}

		private void UpdateSelectItemList()
		{
			grdItems.EndEdit();

			//既にSelectItemListに有れば、グリッド上の選択状態で更新する。
			foreach (MultiSelectPlant bsItem in ((SortableBindingList<MultiSelectPlant>)bsItems.DataSource))
			{
				MultiSelectPlant checkItem = bsItem;

				int index = SelectItemList.FindIndex(s => s.AssetsNM == bsItem.AssetsNM
					&& s.EquipmentNO == bsItem.EquipmentNO
					&& s.MachineSeqNO == bsItem.MachineSeqNO
					&& s.ModelNM == bsItem.ModelNM
					&& s.ServerNM == bsItem.ServerNM
					//&& s.DelFG == bsItem.DelFG
					);

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

		/// <summary>
		/// 検索条件でグリッドに表示する設備を絞り込んで表示する
		/// </summary>
		private void GetDataByCond()
		{
			string sPlantCD = cmbPlantCDCond.Text;
			string sAssetsNM = cmbAssetsNMCond.Text;

			UpdateSelectItemList();

			List<Equi> setTargetEquiList = EquipList.FindAll(equi => equi.EquipmentNO.Contains(sPlantCD)).FindAll(equi => equi.AssetsNM.Contains(sAssetsNM));
			
			SetPlantToGrid(setTargetEquiList);
		}

		private void tbPlantCDCond_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void tbAssetsNMCond_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void cmbAssetsNMCond_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GetDataByCond();
			}
		}

		private void cmbPlantCDCond_KeyDown(object sender, KeyEventArgs e)
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
