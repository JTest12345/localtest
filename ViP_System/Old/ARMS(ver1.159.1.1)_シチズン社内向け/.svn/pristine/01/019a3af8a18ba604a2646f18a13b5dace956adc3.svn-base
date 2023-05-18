using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARMS3.Model;
using ARMS3.Model.Machines;
using ARMS3.Model.Carriers;
using ArmsApi;
using ArmsApi.Model;
using System.Threading;
using ARMS3.Model.PLC;

namespace ARMS3
{
    public partial class FrmTestMode : Form
    {
        /// <summary>搬送機リスト</summary>
        private List<ICarrier> carriers = new List<ICarrier>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cars"></param>
        public FrmTestMode(List<ICarrier> cars)
        {
            InitializeComponent();

            this.carriers = cars;
        }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cars"></param>
		public FrmTestMode()
		{
			InitializeComponent();
		}
		
        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmTestMode_Load(object sender, EventArgs e)
        {
            Log.SysLog.Info("テストフォーム開始");

            if (carriers.Count == 0)
            {
                rbTargetAddress.Checked = true;
            }
            else 
            {
                cmbCarrier.DataSource = this.carriers;
                cmbCarrier.ValueMember = "CarNo";
                cmbCarrier.DisplayMember = "Name";
                cmbCarrier.SelectedIndex = 0;

                rbTargetCarrier.Checked = true;
            }

			changeProtocolMode();
        }

        /// <summary>
        /// Bit読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBitRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBitAddress.Text) == true)
            {
                return;
            }

            IPLC plc = getPlc();
            this.txtBitResult.Text = plc.GetBit(txtBitAddress.Text);
        }

        /// <summary>
        /// Bit書き込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBitWrite_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBitAddress.Text) == true)
            {
                return;
            }
            if (string.IsNullOrEmpty(txtBitWrite.Text) == true)
            {
                return;
            }

            IPLC plc = getPlc();
            plc.SetBit(txtBitAddress.Text, 1, txtBitWrite.Text);
        }

        /// <summary>
        /// WORD読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWordRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtWordAddress.Text) == true)
            {
                return;
            }
            if (nudWordLength.Text == "0")
            {
                return;
            }

            IPLC plc = getPlc();
			this.txtWordResult.Text = plc.GetString(txtWordAddress.Text, int.Parse(nudWordLength.Text),false);
        }

        /// <summary>
        /// WORD書き込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWordWrite_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtWordAddress.Text) == true)
            {
                return;
            }
            if (string.IsNullOrEmpty(txtWordWrite.Text) == true)
            {
                return;
            }

            IPLC plc = getPlc();
            plc.SetString(txtWordAddress.Text, txtWordWrite.Text);
        }

        /// <summary>
        /// マガジンプレート読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMagRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMagPlateAddress.Text) == true)
            {
                return;
            }

            IPLC plc = getPlc();

			if (rbTargetCarrier.Checked)
			{
				this.txtMagReadResult.Text = plc.GetMagazineNo(txtMagPlateAddress.Text, ((Robot)cmbCarrier.SelectedItem).QRWordAddressLength);
			}
			else 
			{
				this.txtMagReadResult.Text = plc.GetMagazineNo(txtMagPlateAddress.Text);
			}
        }

        /// <summary>
        /// ウェハー段数読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWaferPlate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtWaferPlateAddress.Text) == true)
            {
                return;
            }

            IPLC plc = getPlc();
            string s = plc.GetBit(txtWaferPlateAddress.Text, 5);
            int retv = 0;
            int counter = 0;
            foreach (char c in s)
            {
                int i;

                //各段数を2のn乗として足し合わせ
                if (int.TryParse(c.ToString(), out i) == true)
                {
                    retv += (i * (int)Math.Pow(2, counter));
                    counter++;
                }
                else
                {
                    throw new ApplicationException("ウェハー段数取得時にINTパースエラー発生");
                }
            }

            this.txtWaferPlateResult.Text = retv.ToString();
        }

        /// <summary>
        /// 警報テスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestAlert_Click(object sender, EventArgs e)
        {  
            FrmErrHandle frmErr = new FrmErrHandle("警報テスト");
            frmErr.ShowDialog();
        }

        private void rbTargetCarrier_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                txtIpAddress.Enabled = false;
                txtPort.Enabled = false;
				txtHost.Enabled = false;

				grpPlcMaker.Enabled = false;

                cmbCarrier.Enabled = true;
            }
        }

        private void rbTargetAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                txtIpAddress.Enabled = true;
                txtPort.Enabled = true;
				txtHost.Enabled = true;

				grpPlcMaker.Enabled = true;

                cmbCarrier.Enabled = false;
            }
        }

        private IPLC getPlc()
        {
            if (rbTargetCarrier.Checked)
            {
                return ((ICarrier)cmbCarrier.SelectedItem).Plc;
            }
            else 
            {
                int port = 0;
                if (!int.TryParse(txtPort.Text,out port))
                {
                    throw new ApplicationException("Portに不正な文字が入力されています。");
                }

				if (rbOmronPlc.Checked)
				{
					return new Omron(txtIpAddress.Text.Trim(), port, txtHost.Text.Trim());
				}
				else if (rbMitsubishiPlc.Checked)
				{
					if (rbUdp.Checked)
					{
						return new Mitsubishi(txtIpAddress.Text.Trim(), port);
					}
					else 
					{
						return new MitsubishiTcp(txtIpAddress.Text.Trim(), port);
					}
				}
				else
				{
					return new RelayMachinePLC(txtIpAddress.Text, port);
				}
            }
        }

		private void btnDateRead_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtDateAddress.Text) == true)
			{
				return;
			}

			IPLC plc = getPlc();
			this.txtDateResult.Text = plc.GetWordsAsDateTime(txtDateAddress.Text).ToString();
		}

		private void btnBinaryWrite_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtBinaryAddress.Text) == true)
			{
				return;
			}

			IPLC plc = getPlc();
			plc.SetWordAsDecimalData(txtBinaryAddress.Text, int.Parse(txtBinaryWrite.Text));
		}

		private void btnBinaryRead_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtBinaryAddress.Text) == true)
			{
				return;
			}

			IPLC plc = getPlc();
			txtBinaryResult.Text = plc.GetWordAsDecimalData(txtBinaryAddress.Text, Convert.ToInt32(nudBinaryLength.Value)).ToString();
		}

		private void rbMitsubishiPlc_CheckedChanged(object sender, EventArgs e)
		{
			changeProtocolMode();
		}

		private void rbOmronPlc_CheckedChanged(object sender, EventArgs e)
		{
			changeProtocolMode();
		}

		private void rbKeyencePlc_CheckedChanged(object sender, EventArgs e)
		{
			changeProtocolMode();
		}

		private void changeProtocolMode()
		{
			if (rbMitsubishiPlc.Checked)
			{
				gbProtocol.Enabled = true;
			}
			else if (rbOmronPlc.Checked)
			{
				gbProtocol.Enabled = false;
				rbUdp.Checked = true;
			}
			else 
			{
				gbProtocol.Enabled = false;
				rbUdp.Checked = true;
			}
		}
    }
}
