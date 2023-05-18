using LENS2.Machine;
using LENS2_Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LENS2
{
	public partial class F91_MachineLogOutputCompletionMainte : Form
	{
		public IMachine Mac { get; set; }
		public List<LENS2.Machine.MachineBase.MainteTargetMachineAddress> mainteTargetMachineAddress { get; set; }


		public F91_MachineLogOutputCompletionMainte()
		{
			InitializeComponent();
		}

		private void F91_MachineLogOutputCompletionMainte_Load(object sender, EventArgs e)
		{
			try
			{
				List<MachineInfo> machines = MachineInfo.GetDatas(string.Empty, Config.Settings.MachineGroupCd);
				foreach (MachineInfo machine in machines)
				{
					if (machine.ClassCd == "Inspector" || machine.ClassCd == "Mold")
					{
						addMainteMachineTree(machine.NascaPlantCd, machine.NascaPlantCd, machine.ClassNm, machine.MachineNm);
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void addMainteMachineTree(string plantcd, string machinecd, string classnm, string machinenm)
		{
			try
			{
				tvMainteMachine.Nodes.Add(plantcd, string.Format("{0} {1}({2})", classnm, machinenm, machinecd), "wave_stop.ico");
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void tvMainteMachine_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			try
			{
				cmbMainteAddr.Items.Clear();

				string plantcd = e.Node.Name;
				Mac = MachineBase.GetMachine(plantcd);

				MachineInfo machine = MachineInfo.GetData(plantcd);

				switch (machine.ClassCd)
				{
					case "Wirebonder":
						return;

					case "Inspector":
						mainteTargetMachineAddress = ((Inspector)Mac).mainteTargetMachineAddress;
						break;

					case "Mold":
						mainteTargetMachineAddress = ((Mold)Mac).mainteTargetMachineAddress;
						break;
				}

				if (mainteTargetMachineAddress != null)
				{
					SetMainteAddressComboBox(mainteTargetMachineAddress);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void SetMainteAddressComboBox(List<LENS2.Machine.MachineBase.MainteTargetMachineAddress> machineAddressList)
		{
			try
			{
				foreach (LENS2.Machine.MachineBase.MainteTargetMachineAddress addrInfo in machineAddressList)
				{
					cmbMainteAddr.Items.Add(addrInfo.AddressNm);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void cbWriteUnlock_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				tmWriteLock.Start();
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void tmWriteLock_Tick(object sender, EventArgs e)
		{
			try
			{
				tmWriteLock.Stop();

				cbWriteUnlock.Checked = false;
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void btnGetVal_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(tbMainteAddr.Text) == false)
				{
					string getVal = Mac.Plc.GetBit(tbMainteAddr.Text).ToString().ToLower();

					if (getVal == "true")
					{
						tbCurrentVal.Text = "1";
					}
					else if (getVal == "false")
					{
						tbCurrentVal.Text = "0";
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void btnSendVal_Click(object sender, EventArgs e)
		{
			try
			{
				if (cbWriteUnlock.Checked)
				{
					string writeValStr = tbWriteVal.Text.ToLower();
					if (string.IsNullOrEmpty(writeValStr) == false)
					{
						bool setVal;

						if (writeValStr == "0")
						{
							writeValStr = "false";
						}
						else if (writeValStr == "1")
						{
							writeValStr = "true";
						}
						else if (writeValStr == "true" || writeValStr == "false"){ } // 処理の必要無し
						else
						{
							MessageBox.Show("書き込み値の値は、次の何れかの必要があります。(可能な値：true, false, 0, 1)");
							return;
						}

						if (bool.TryParse(writeValStr, out setVal) == false)
						{
							MessageBox.Show("書き込み値がbool型に変換できません。");
							return;
						}
						Mac.Plc.SetBit(tbMainteAddr.Text, setVal);
						cbWriteUnlock.Checked = false;
					}
				}
				else
				{
					MessageBox.Show("装置への値書き込みを行う場合、値の書き込みロック解除をチェックして下さい。");
				}
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

		private void cmbMainteAddr_TextChanged(object sender, EventArgs e)
		{
			try
			{
				tbMainteAddr.Text = mainteTargetMachineAddress.Find(m => m.AddressNm == (string)cmbMainteAddr.SelectedItem).Address;
			}
			catch (Exception err)
			{
				MessageBox.Show("ErrMsg：" + err.Message + "\r\n\r\n" + "StackTrace：" + err.StackTrace);
			}
		}

        private void tbCurrentVal_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
