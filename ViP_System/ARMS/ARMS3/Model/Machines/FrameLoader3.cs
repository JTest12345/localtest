using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;


namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 左右ストッカー切り替え付き
    /// 3箇所からの取出しに対応した搭載装置
    /// 
    /// UnloaderReqBitAddressの内、
    ///     Key=1は搭載位置
    ///     Key=2はバッファと共用している位置
    ///     
    /// 一番外側のバッファが空の場合は、搭載後に即内側バッファに移動する挙動のため
    /// この装置で一つ外側のバッファの状態までは監視する。
    /// 
    /// </summary>
    class FrameLoader3 : FrameLoader
    {
        /// <summary>
        /// ロットマーキングデータ読み取り用のPLC通信
        /// </summary>
        public Mitsubishi LocalPLC { get; set; }

        /// <summary>
        /// 最初の排出バッファ
        /// key=2の排出要求と共有
        /// </summary>
        public int NextBufferMacNo { get; set; }

        /// <summary>
        /// 最初の排出バッファ
        /// key=3の排出要求と共有
        /// </summary>
        public int NextBuffer2MacNo { get; set; }

        /// <summary>
        /// 搭載位置ストッカーNo取得アドレス
        /// </summary>
        public string PT1StockerNoAddress { get; set; }

        /// <summary>
        /// バッファ1ストッカーNo取得アドレス
        /// </summary>
        public string PT2StockerNoAddress { get; set; }

        /// <summary>
        /// バッファ２ストッカーNo取得アドレス
        /// </summary>
        public string PT3StockerNoAddress { get; set; }

        /// <summary>
        /// ストッカーNo指示要求
        /// </summary>
        public string StockerNoRequestBitAddress { get; set; }

        /// <summary>
        /// ストッカーNo指示
        /// </summary>
        public string StockerNoCommandBitAddress { get; set; }

        /// <summary>
        /// ストッカーNo
        /// </summary>
        public string StockerNoWordAddress { get; set; }

        /// <summary>
        /// 選択中品種設定アドレス　本装置の場合は2固定
        /// </summary>
        public string ProgramNoAddress { get; set; }



        protected override void concreteThreadWork()
        {
            try
            {

                for (int i = 3; i >= 1; i--)
                {
                    string val = this.UnloaderReqBitAddressList[i];
                    string retv = this.Plc.GetBit(val);
                    if (retv == Mitsubishi.BIT_ON)
                    {
                        //2つ目の排出要求は次バッファと共有しているので既に仮想マガジンがある場合は何もしない
                        if (i == 2)
                        {
                            IMachine nextBuffer = LineKeeper.GetMachine(this.NextBufferMacNo);

                            VirtualMag nextVmag = nextBuffer.Peek(Station.Unloader);
                            if (nextVmag != null) continue;
                        }
                        //2つ目の排出要求は次バッファと共有しているので既に仮想マガジンがある場合は何もしない
                        if (i == 3)
                        {
                            IMachine nextBuffer = LineKeeper.GetMachine(this.NextBufferMacNo);
                            VirtualMag nextVmag = nextBuffer.Peek(Station.Unloader);
                            if (nextVmag != null) continue;

                            IMachine nextBuffer2 = LineKeeper.GetMachine(this.NextBuffer2MacNo);
                            nextVmag = nextBuffer2.Peek(Station.Unloader);
                            if (nextVmag != null) continue;
                        }

                        base.workComplete(this.Plc, i);
                    }

                }

                //選択中品種のアドレスに2を固定で書き込み。2=混合ありモード
                LocalPLC.SetWordAsDecimalData(ProgramNoAddress, 2);

                //フレーム搭載指示
                setFrameInsertOrder();

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("", ex);
            }
        }

        public override bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (string.IsNullOrEmpty(this.UnLoaderReqBitAddress) == true)
            {
                return false;
            }

            string retv = Plc.GetBit(this.UnLoaderReqBitAddress);
            if (retv != Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //通信エラー
                return false;
            }
        }


        public override string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.EmptyMagazineLoader:
                    foreach (KeyValuePair<int, string> kv in this.EmptyMagLoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.EmptyMagLoaderPointList[kv.Key];
                        }
                    }
                    return this.EmptyMagLoaderPointList[1];

                case Station.Unloader:
                    return this.UnloaderPoint;
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        /// <summary>
        /// 排出位置の実績を確認して無い方のストッカーで搭載指示
        /// </summary>
        private void setFrameInsertOrder()
        {
            string retv = this.LocalPLC.GetBit(StockerNoRequestBitAddress);
            if (retv != Mitsubishi.BIT_ON)
            {
                return;
            }

            int stocker = ChooseStocker();
            if (stocker != 0)
            {
                LocalPLC.SetWordAsDecimalData(StockerNoWordAddress, stocker);
                LocalPLC.SetBit(StockerNoCommandBitAddress, 1, Mitsubishi.BIT_ON);
            }
        }

        /// <summary>
        /// 排出バッファ位置に存在しないストッカーがあり、
        /// 資材が割りついている場合は搭載指示を出す
        /// </summary>
        /// <returns></returns>
        private int ChooseStocker()
        {
            List<int> stockers = new List<int>() { 1, 2 };

            int i = LocalPLC.GetWordAsDecimalData(PT1StockerNoAddress);
            int j = LocalPLC.GetWordAsDecimalData(PT2StockerNoAddress);
            int k = LocalPLC.GetWordAsDecimalData(PT3StockerNoAddress);

            stockers.Remove(i);
            stockers.Remove(j);
            stockers.Remove(k);

            if (stockers.Count == 0)
            {
                return 0;
            }
            else
            {
                MachineInfo m = MachineInfo.GetMachine(this.MacNo);
                foreach (int s in stockers)
                {
                    Material[] mat;
                    if (s == 1)
                    {
                        mat = m.GetMaterialsFrameLoader(DateTime.Now, null, "1", "0");
                    }
                    else
                    {
                        mat = m.GetMaterialsFrameLoader(DateTime.Now, null, "0", "1");
                    }
                    //資材割り付けされている場合はそのストッカーを返す
                    if (mat.Length >= 1) return s;
                }

                return 0;
            }
        }
    }
}
