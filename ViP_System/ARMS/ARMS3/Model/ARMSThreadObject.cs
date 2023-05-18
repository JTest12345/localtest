using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ArmsApi;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 単独でスレッド動作が可能なオブジェクトの基底クラス
    /// </summary>
    public abstract class ARMSThreadObject
    {
        /// <summary>
        /// ルーチン動作停止イベント
        /// </summary>
        public event EventHandler<MachineThreadEventArgs> OnStopWork;

        /// <summary>
        /// ARMS設備No
        /// </summary>
        public int MacNo { get; set; }

        /// <summary>
        /// NASCA設備コード
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 装置日本語名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ARMSThreadObjectのThreadRoutineWorkのループ時間
        /// </summary>
        public int ThreadRoutineWorkMilliSecond { get; set; }

        #region マルチスレッド制御関連

        /// <summary>
        /// 通常停止
        /// </summary>
        public bool StopRequested { get; set; }


        /// <summary>
        /// 初期化処理実行済みフラグ
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// 実行中のスレッド
        /// </summary>
        private Thread runningThread;

        /// <summary>
        /// ルーチン動作中フラグ
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// スレッド二重起動防止用
        /// </summary>
        private object locker = new object();
        #endregion

        #region GetWorkStatus
        /// <summary>
        /// 動作要求状態取得
        /// </summary>
        /// <returns></returns>
        public WorkStatus GetWorkStatus()
        {
            if (IsRunning && StopRequested == false)
            {
                return WorkStatus.Active;
            }
            else if (IsRunning && StopRequested == true)
            {
                return WorkStatus.WaitForStop;
            }
            else
            {
                return WorkStatus.Idle;
            }
        }
        #endregion

        #region RunWork
        /// <summary>
        /// スレッド処理開始
        /// </summary>
        /// <returns>開始成功時True</returns>
        public async void RunWork()
        {
            try
            {
                //Log.SysLog.Info("[Thread start] " + this.Name);
                MachineThreadEventArgs arg = await Task<MachineThreadEventArgs>.Run(() => ThreadRoutineWork());
                //Log.SysLog.Info("[Thread end]" + this.Name);
                

                if (OnStopWork != null)
                {
                    OnStopWork(this, arg);
                }
            }
            catch (Exception ex)
            {
                OnStopWork(this, new MachineThreadEventArgs(this, MachineThreadEventArgs.ExitStatus.Exception, ex));
            }
        }
        #endregion

        #region AbortThread()

        /// <summary>
        /// スレッド強制終了処理
        /// 予期しない状態に陥る可能性があるので、警告したうえで実行させること
        /// </summary>
        public void AbortThread()
        {
            if (runningThread != null)
            {
                IsRunning = false;

                runningThread.Abort();
                Log.RBLog.Debug("thread aborted:" + this.Name);
            }
        }
        #endregion

        #region ThreadRoutineWork
        protected MachineThreadEventArgs ThreadRoutineWork()
        {
            try
            {
                //スレッド多重起動チェック
                bool entered = false;
                Monitor.TryEnter(locker, 0, ref entered);
                if (entered == false) return new MachineThreadEventArgs(this, MachineThreadEventArgs.ExitStatus.ThreadAlreadyRun, null);

                IsRunning = true;
                StopRequested = false;
                IsInitialized = false;

                //実行スレッドの記録（Abort用）
                runningThread = Thread.CurrentThread;


                #region ------- スレッドの実行間隔 (LineConfigの設定値 -> ArmsConfigの設定値 -> デフォルト)

                // デフォルト値
                int milliSecond;
                if (this is Model.Carriers.Robot || this is Model.Carriers.Robot3 || this is Model.Carriers.Robot4 || this is Model.Carriers.Robot5)
                {
                    milliSecond = 50;
                }
                else if (this is Model.Machines.Oven)
                {
                    milliSecond = 60000;
                }
                else if (this is Model.Machines.SQMachine)
                {
                    milliSecond = 10000;
                }
                else if (this is Model.Carriers.Bridge)
                {
                    milliSecond = 2000;
                }
                else
                {
                    if (Config.Settings.ThreadRoutineWorkMilliSecond > 0)
                    {
                        // ArmsConfigに設定項目がある場合は、上書き
                        milliSecond = Config.Settings.ThreadRoutineWorkMilliSecond;
                    }
                    else
                    {
                        milliSecond = 10000;
                    }
                }

                // LineConfigに設定項目がある場合は、上書き
                if (ThreadRoutineWorkMilliSecond > 0)
                {
                    milliSecond = ThreadRoutineWorkMilliSecond;
                }
                #endregion

                Log.RBLog.Info($"{this.Name} 監視処理間隔時間:{milliSecond}[ミリ秒]");

                while (true)
                {
                    //スレッドの実処理 abstract
                    concreteThreadWork();

                    Thread.Sleep(milliSecond);
                    
                    //通常停止要求応答
                    if (StopRequested) 
                        return new MachineThreadEventArgs(this, MachineThreadEventArgs.ExitStatus.NormalExit, null);
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex.Message, ex);
                return new MachineThreadEventArgs(this, MachineThreadEventArgs.ExitStatus.Exception, ex);
            }
            finally
            {
                if (Monitor.IsEntered(locker))
                {
                    Monitor.Exit(locker);
                    IsRunning = false;
                    runningThread = null;
                }
            }
        }
        #endregion

        /// <summary>
        /// 実際にスレッド内でループ実行される処理
        /// </summary>
        protected abstract void concreteThreadWork();
    }
}
