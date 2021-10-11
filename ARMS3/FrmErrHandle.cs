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
using System.Threading;
using ARMS3.Model.Carriers;
using ArmsApi.Model;
using ArmsApi;

namespace ARMS3
{
    public enum ErrorHandleMethod
    {
        None,
        Retry,
        Discharge,
    }

    public partial class FrmErrHandle : Form
    {
        private System.Media.SoundPlayer sp;

        /// <summary>
        /// ロボット操作用
        /// </summary>
        //private Robot robo;
        private ICarrier robo;

        /// <summary>
        /// QR照合不一致情報
        /// </summary>
        private Robot.QRMissMatchException qrException;

        /// <summary>
        /// 排出処理用のサブスレッド
        /// </summary>
        private Thread robotSubThread;


        private ArmsApiResponse workResponse;

        private int macno;

        /// <summary>
        /// 空マガジン排出の1マガジン不一致フラグ
        /// </summary>
        private bool isEmptyUnloaderMissMatchAndNotFix = false;

        public ErrorHandleMethod Method { get; set; }

        #region コンストラクタ　ロボット異常

        public FrmErrHandle(ICarrier robo, string msg)
        {
            InitializeComponent();
            this.sp = new System.Media.SoundPlayer("alert.wav");
            this.Method = ErrorHandleMethod.None;

            this.robo = robo;
            this.txtMessage.Text = msg;

            btnDischarge.Enabled = true;
            btnRetry.Enabled = false;
        }
        #endregion

        #region コンストラクタ　汎用　排出のみ

        public FrmErrHandle(string msg)
        {
            InitializeComponent();
            this.sp = new System.Media.SoundPlayer("alert.wav");
            this.Method = ErrorHandleMethod.None;
            this.txtMessage.Text = msg;
            this.btnDischarge.Enabled = false;

        }
        #endregion

        #region コンストラクタ　QR照合不一致の場合

        /// <summary>
        /// QR不整合エラー時
        /// </summary>
        /// <param name="robo"></param>
        /// <param name="ex"></param>
        public FrmErrHandle(ICarrier robo, CarrierBase.QRMissMatchException ex)
        {
            InitializeComponent();
            this.sp = new System.Media.SoundPlayer("alert.wav");
            this.Method = ErrorHandleMethod.None;
            this.qrException = ex;

            this.robo = robo;

            if (ex.From.Station == Station.EmptyMagazineUnloader)
            {
                VirtualMag nextVirtualMag = LineKeeper.GetMachine(ex.From.MacNo).Peek(Station.EmptyMagazineUnloader);
                if (nextVirtualMag != null)
                {
                    if (nextVirtualMag.MagazineNo == ex.RealMag)
                    {
                        this.txtMessage.AppendText("空マガジン排出で仮想マガジンが1つずれています。\n");
                        this.txtMessage.AppendText("[排出]を押した場合、自動で仮想マガジンが修正されます");

                        this.txtMagazine.Text = ex.VirtualMag;
                        this.txtMachine.Text = LineKeeper.GetMachine(ex.From.MacNo).Name;
                        this.isEmptyUnloaderMissMatchAndNotFix = true;
                        return;
                    }
                }
            }

            btnDischarge.Enabled = true;
            btnRetry.Enabled = false;

            this.txtMessage.Text = ex.Message;
            this.txtMagazine.Text = ex.VirtualMag;
            this.txtMachine.Text = LineKeeper.GetMachine(ex.From.MacNo).Name;
        }

        #endregion

        #region コンストラクタ　完了登録NG

        public FrmErrHandle(ArmsApiResponse res)
        {
            InitializeComponent();
            this.sp = new System.Media.SoundPlayer("alert.wav");
            this.Method = ErrorHandleMethod.None;

            this.txtMessage.Text = res.Message;

            btnDischarge.Enabled = true;
            btnRetry.Enabled = false;
            btnExit.Enabled = false;

            this.txtMagazine.Text = res.MagazineNo;
            if (res.MacNo.HasValue)
            {
                MachineInfo m = MachineInfo.GetMachine(res.MacNo.Value);
                if (m != null)
                {
                    this.txtMachine.Text = m.MachineName;
                }
            }

            this.workResponse = res;
        }
        #endregion

        #region コンストラクタ　装置処理異常

        public FrmErrHandle(string msg, int macno)
        {
            InitializeComponent();
            this.sp = new System.Media.SoundPlayer("alert.wav");
            this.Method = ErrorHandleMethod.None;
            this.txtMessage.Text = msg;

            btnDischarge.Enabled = false;
            btnRetry.Enabled = true;

            MachineInfo m = MachineInfo.GetMachine(macno);
            if (m != null)
            {
                this.txtMachine.Text = m.MachineName;
            }
        }
        #endregion


        public FrmErrHandle()
        {
            InitializeComponent();
        }

        private void FrmErrHandle_Load(object sender, EventArgs e)
        {
            sp.PlayLooping();
        }

        private void FrmErrHandle_FormClosing(object sender, FormClosingEventArgs e)
        {
            sp.Stop();
        }

        private void btnDischarge_Click(object sender, EventArgs e)
        {
            sp.Stop();

            //装置スレッドのエラー処理の場合
            if (this.workResponse != null)
            {
                this.Method = ErrorHandleMethod.Discharge;

                workResponse.NextMachines = new List<int>();
                workResponse.NextMachines.Add(Route.GetDischargeConveyor(workResponse.MacNo.Value));

                VirtualMag vMag 
                    = VirtualMag.GetVirtualMag(workResponse.MacNo.ToString(), string.Empty, workResponse.MagazineNo)[0];
                Station st = (Station)Enum.Parse(typeof(Station), vMag.LocationId.ToString());

                VirtualMag mag = new VirtualMag();
                mag = mag.Peek(new Location(workResponse.MacNo.Value, st));

                mag.NextMachines = workResponse.NextMachines;
				mag.PurgeReason = workResponse.Message;
                mag.Updatequeue();

                Log.RBLog.Info("排出スレッド-装置");

                this.Method = ErrorHandleMethod.Discharge;
                this.Close();
            }
            //ロボットスレッドのエラー処理の場合
            else if (this.robo != null)
            {
                if (isEmptyUnloaderMissMatchAndNotFix == true)
                {
                    LineKeeper.GetMachine(qrException.From.MacNo).Dequeue(Station.EmptyMagazineUnloader);
                    isEmptyUnloaderMissMatchAndNotFix = false;
                }

                MethodInvoker invoker = new MethodInvoker(callbackPurgeMagazine);
                ParameterizedThreadStart pts = new ParameterizedThreadStart(purgeMagazine);
                this.robotSubThread = new Thread(pts);
                this.robotSubThread.Start(invoker);
                this.btnDischarge.Enabled = false;
                this.btnRetry.Enabled = false;

                Log.RBLog.Info("排出スレッド-ロボット");
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            sp.Stop();
            this.Method = ErrorHandleMethod.Retry;
            //NASCA.JobEnqueue(job);
            //Log.WriteNASCALog("NASCAJOBを再投入しました");
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            sp.Stop();
            this.Method = ErrorHandleMethod.None;

            MessageBox.Show("装置の場合は装置監視を停止するので、問題解決後は再度開始ボタンを押して装置監視を再開して下さい。",
                "注意", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Log.RBLog.Info("終了スレッド");


            this.Close();

            //throw new Exception(txtMessage.Text);

            //DialogResult res = MessageBox.Show(
            //    this,
            //    "マガジンを排出せずに終了しますか？\n\nマガジンを手でつかんでいる場合は手動で取り出しが必要です。\n排出動作中であれば処理が途中停止します。",
            //    "警告",
            //    MessageBoxButtons.OKCancel,
            //    MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

            //if (res == DialogResult.OK)
            //{


            //    if (this.robotSubThread != null)
            //    {
            //        if (this.robotSubThread.IsAlive == true)
            //        {
            //            this.robotSubThread.Abort();
            //        }
            //    }

                //this.Close();

            //}
        }

        private void btnStopWav_Click(object sender, EventArgs e)
        {
            sp.Stop();
        }

        private void callbackPurgeMagazine()
        {
            if (this.InvokeRequired == true)
            {
                this.Invoke(new MethodInvoker(callbackPurgeMagazine));
            }
            else
            {
                this.Method = ErrorHandleMethod.Discharge;
                this.Close();
            }
        }

        private void purgeMagazine(object callbackDelegate)
        {
            string msg = this.txtMessage.Text;

            //if (job != null)
            //{
            //    robo.PurgeHandlingMagazine(job.Mag.MagazineNo, msg);
            //}
            //else
            //{
                VirtualMag mag = new VirtualMag();
                mag.MagazineNo = "unknown";
                robo.PurgeHandlingMagazine(mag, msg);
            //}

            ((MethodInvoker)callbackDelegate).Invoke();
        }

    }
}
