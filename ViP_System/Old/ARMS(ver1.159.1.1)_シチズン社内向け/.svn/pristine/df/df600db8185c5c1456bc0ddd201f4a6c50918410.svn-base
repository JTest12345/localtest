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
    /// フレーム搭載機
    /// </summary>
    class FrameLoader : MachineBase
    {
        /// <summary>
        /// ロットマーキングデータ読み取り用のPLC通信
        /// </summary>
        public Mitsubishi LocalPLC { get; set; }

        /// <summary>
        /// 空マガジン供給アドレス
        /// </summary>
        public SortedList<int, string> EmptyMagLoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 空マガジンFromTo
        /// </summary>
        public SortedList<int, string> EmptyMagLoaderPointList { get; set; }

        /// <summary>
        /// 排出FromTo
        /// </summary>
        public SortedList<int, string> UnloaderPointList { get; set; }

        /// <summary>
        /// 排出アドレス
        /// </summary>
        public SortedList<int, string> UnloaderReqBitAddressList { get; set; }

        /// <summary>
        /// Stocker1段数取得アドレス
        /// </summary>
        public SortedList<int, string> Stocker1WordAddressList { get; set; }

        /// <summary>
        /// ストッカー2段数取得アドレス
        /// </summary>
        public SortedList<int, string> Stocker2WordAddressList { get; set; }

        /// <summary>
        /// 作業完了時間PLCアドレス
        /// </summary>
        public SortedList<int, string> WorkCompleteTimeAddressList { get; set; }

        /// <summary>
        /// 作業開始時間PLCアドレス
        /// </summary>
        public SortedList<int, string> WorkStartTimeAddressList { get; set; }

        /// <summary>
        /// ロットマーキング必要時のプログラム番号
        /// </summary>
        public int LotMarkingNeedProgramNo { get; set; }

        /// <summary>
        /// ロットマーキング禁止時のプログラム番号
        /// </summary>
        public int LotMarkingForbiddenProgramNo { get; set; }

        /// <summary>
        /// 空マガジン供給中アドレス
        /// </summary>
        public SortedList<int, string> EmptyMagLoaderDoBitAddressList { get; set; }

        /// <summary>
        /// 実マガジン排出中アドレス
        /// </summary>
        public SortedList<int, string> UnloaderDoBitAddressList { get; set; }


        public FrameLoader()
            : base()
        {
            this.EmptyMagLoaderPointList = new SortedList<int, string>();
            this.EmptyMagLoaderReqBitAddressList = new SortedList<int, string>();
            this.UnloaderPointList = new SortedList<int, string>();
            this.UnloaderReqBitAddressList = new SortedList<int, string>();
            this.Stocker1WordAddressList = new SortedList<int, string>();
            this.Stocker2WordAddressList = new SortedList<int, string>();
            this.WorkCompleteTimeAddressList = new SortedList<int, string>();
            this.WorkStartTimeAddressList = new SortedList<int, string>();
            this.EmptyMagLoaderDoBitAddressList = new SortedList<int, string>();
            this.UnloaderDoBitAddressList = new SortedList<int, string>();
        }

        protected override void concreteThreadWork() 
        {
            try
            {
                //排出要求2は必ず要求1がOFFの場合しか見ないようにする
                var outReq2 = this.Plc.GetBit(this.UnloaderReqBitAddressList[2]);
                var outReq1 = this.Plc.GetBit(this.UnloaderReqBitAddressList[1]);

                if (outReq1 == Mitsubishi.BIT_ON)
                {
                    workComplete(this.Plc, 1);
                }
                else if (outReq2 == Mitsubishi.BIT_ON)
                {
                    workComplete(this.Plc, 2);
                }

                if (LocalPLC != null)
                {
                    //プロファイルのタイプ名からロットマーキング有無を調査して搭載機へ直接指示
                    checkLotMarkingFg();
                }

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("", ex);
            }
        }



        /// <summary>
        /// 作業完了
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="listKey"></param>
        protected void workComplete(IPLC plc, int listKey)
        {
            try
            {
                VirtualMag ulMagazine = this.Peek(Station.Unloader);
                if (ulMagazine != null)
                {
                    return;
                }

                LotMarkData[] markdata1;

                VirtualMag newMagazine = this.Peek(Station.EmptyMagazineLoader);
                if (newMagazine != null)
                {
                    if (this.LocalPLC != null)
                    {

                        //シフト停止の完了確認
                        if ((this.LocalPLC.GetBit(MAG_SHIFT_STOP_READY_ADDRESS) != Mitsubishi.BIT_ON))
                        {
                            //マガジンシフトの停止要求
                            this.LocalPLC.SetBit(MAG_SHIFT_STOP_REQ_ADDRESS, 1, "1");
                            Log.SysLog.Info("マガジンシフト停止要求ON");

                            //シフト停止の装置側準備が整うまではデータ読み取りを行わない
                            return;
                        }
                        Log.SysLog.Info("マガジンシフト停止完了の装置応答確認");

                    }

                    try
                    {
                        int decadr = DEC_ADDRESS_START_MAG1;
                        if (listKey == 2)
                        {
                            decadr = DEC_ADDRESS_START_MAG2;
                        }

                        //先にマーキングデータを取得して後で変更ないか比較
                        markdata1 = GetMarkingData(decadr);

                        //作業開始完了時間取得
                        newMagazine.LastMagazineNo = newMagazine.MagazineNo;
                        newMagazine.MagazineNo = newMagazine.MagazineNo;
                        newMagazine.WorkComplete = plc.GetWordsAsDateTime(this.WorkCompleteTimeAddressList[listKey]);
                        newMagazine.WorkStart = plc.GetWordsAsDateTime(this.WorkStartTimeAddressList[listKey]);
                        newMagazine.Stocker1 = plc.GetWordAsDecimalData(this.Stocker1WordAddressList[listKey]);
                        newMagazine.Stocker2 = plc.GetWordAsDecimalData(this.Stocker2WordAddressList[listKey]);
                    }
                    catch (Exception ex)
                    {
                        Log.ApiLog.Error("搭載機排出位置に作業履歴の無い実マガジンが存在します\n" + ex.ToString());
                        return;
                    }
                    finally
                    {
                        if (this.LocalPLC != null)
                        {
                            //マガジンシフトの停止要求
                            this.LocalPLC.SetBit(MAG_SHIFT_STOP_REQ_ADDRESS, 1, "0");
                            Log.SysLog.Info("マガジンシフト停止要求OFF");
                        }
                    }

                    this.Enqueue(newMagazine, Station.Unloader);

                    this.WorkComplete(newMagazine, this, true);

                    //ロットマーキング情報の保存
                    if (LocalPLC != null)
                    {
                        Magazine svrmag = Magazine.GetCurrent(newMagazine.MagazineNo);
                        foreach (LotMarkData m in markdata1)
                        {
                            //読み込み値が0の場合は読み取り無効で運転されているため登録しない
                            if (m.MarkData != 0)
                            {
                                m.LotNo = svrmag.NascaLotNO;
                                m.Update();
                            }
                        }
                    }
                }
                else
                {
                    //空マガジンを無い状態で作業完了が発生した場合
                    //検証の結果、マガジン排出後に暫く排出要求が経ち続ける事が分かったので正常動作として何も行わない
                }

                this.Dequeue(Station.EmptyMagazineLoader);

            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("", ex);
            }
        }

        #region マーキングデータ読み取り関連

        //この辺の定数を分岐する必要がある場合は設定ファイルに切り出すこと
        //またその際は他のZRアドレスとの整合をとるため全体的にconvertZRAddress関数を切り出す仕様にする事

        private const int DEC_ADDRESS_START_MAG1 = 1000;
        private const int DEC_ADDRESS_START_MAG2 = 500;
        private const int DEC_ADDRESS_PROGRAM_NO = 4000;

        private const int MAG_ROWS = 45;
        private const int DATA_ROW_OFFSET = 10;
        private const int DM_BYTE_LENGTH = 18;
        private const string ADDRESS_HEADER = "ZR";

        private const string MAG_SHIFT_STOP_REQ_ADDRESS = "B000015";
        private const string MAG_SHIFT_STOP_READY_ADDRESS = "B000005";


        /// <summary>
        /// 段アドレス基点から数えてストッカー番号の出現位置
        /// </summary>
        private const int STOCKER_NUM_OFFSET = 9;

        public LotMarkData[] GetMarkingData(int decAddrStart)
        {
            List<LotMarkData> retv = new List<LotMarkData>();

            if (this.LocalPLC == null) return retv.ToArray();


            string address = convertZRAddress(decAddrStart);
            byte[] bs = LocalPLC.GetWordRaw(address, MAG_ROWS * DATA_ROW_OFFSET);

            Console.WriteLine(bs.Length);
            for (int i = 0; i < MAG_ROWS; i++)
            {
                LotMarkData m = new LotMarkData();
                m.Row = i + 1;

                string str = Encoding.ASCII.GetString(bs, i * DATA_ROW_OFFSET * 2, DM_BYTE_LENGTH);
                try
                {
                    m.MarkData = decimal.Parse(str);
                }
                catch
                {
                    m.MarkData = 0;
                }


                m.StockerNo = BitConverter.ToInt16(bs, DM_BYTE_LENGTH + i * DATA_ROW_OFFSET * 2);
                m.WorkDt = DateTime.Now;
                retv.Add(m);
            }

            return retv.ToArray();
        }

        protected void checkLotMarkingFg()
        {
           
            Profile p = Profile.GetCurrentDBProfile(this.MacNo);
            var fg = LotMarkingType.GetMarkingFg(p.TypeCd);
            if (fg == LotMarkingType.MarkingFg.Need)
            {
                LocalPLC.SetWordAsDecimalData(convertZRAddress(DEC_ADDRESS_PROGRAM_NO), LotMarkingNeedProgramNo);
            }
            else
            {
                LocalPLC.SetWordAsDecimalData(convertZRAddress(DEC_ADDRESS_PROGRAM_NO), LotMarkingForbiddenProgramNo);
            }
        }

        protected string convertZRAddress(int decAddr)
        {
            string address = ADDRESS_HEADER + decAddr.ToString("X").PadLeft(6, '0');
            return address;
        }


        #endregion


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
                    foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.UnloaderPointList[kv.Key];
                        }
                    }
                    return this.UnloaderPointList[1];
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        /// <summary>
        /// 空マガジン配置
        /// </summary>
        /// <returns></returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            bool moved = false;

            if (this.IsInputForbidden() == true)
            {
                return moved;
            }

            foreach (KeyValuePair<int, string> kv in this.EmptyMagLoaderReqBitAddressList)
            {
                string retv = Plc.GetBit(kv.Value);

                if (retv == Mitsubishi.BIT_ON)
                {
                    // 空マガジン投入コンベアの状態確認
                    IMachine conveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                    if (conveyor.IsRequireOutputEmptyMagazine() == true)
                    {
                        IMachine empMagLoadComveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                        Location from = new Location(empMagLoadComveyor.MacNo, Station.EmptyMagazineUnloader);

                        //空マガジン供給コンベヤからマガジンの移動
                        LineKeeper.MoveFromTo(from,
                                new Location(this.MacNo, Station.EmptyMagazineLoader), false, true, true);

                        moved = true;
                    }
                    else
                    {
                        //空マガジンがどこにも無い場合
                        return moved;
                    }
                }
            }

            return moved;
        }

        /// <summary>
        /// 空マガジン供給　可否確認(搭載機の場合は2つの要求を確認)
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireEmptyMagazine()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            foreach (KeyValuePair<int, string> kv in this.EmptyMagLoaderReqBitAddressList)
            {
                //string retv = this.Plc.GetBit(kv.Value);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(kv.Value);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、空供給要求OFF扱い。アドレス：『{kv.Value}』, エラー内容：{ex.Message}");
                    return false;
                }
                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// マガジン排出　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (this.UnloaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.UnloaderReqBitAddressList.Count == 0)
            {
                return false;
            }

            foreach (string address in this.UnloaderReqBitAddressList.Values)
            {
                //string retv = Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }
                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }

        //【1.124.3】関数Override版追加
        #region GetFromBufferCode        
        /// <summary>
        /// 排出側バッファ位置取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            switch (station)
            {
                case Station.EmptyMagazineLoader:
                    foreach (KeyValuePair<int, string> kv in this.EmptyMagLoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return kv.Key.ToString();
                        }
                    }
                    return null;

                case Station.Unloader:
                    foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return kv.Key.ToString();
                        }
                    }
                    return null;

                default:
                    throw new ApplicationException($"{this.Name}：GetFromToBufferCodeで定義されていないStation:{station.ToString()}の要求を検知しました。lineconfigを確認して下さい。");
            }

        }
        #endregion

        //【1.124.3】関数Override版追加＋引数buffercode追加
        #region GetRequireBitData
        public override string GetRequireBitData(Station st, string buffercode)
        {
            int bc;
            if (int.TryParse(buffercode, out bc) == false)
            {
                bc = 1;
            }

            switch (st)
            {
                case Station.EmptyMagazineLoader:
                    return this.Plc.GetBit(this.EmptyMagLoaderReqBitAddressList[bc]);

                case Station.Unloader:
                    return this.Plc.GetBit(this.UnloaderReqBitAddressList[bc]);

                default:
                    throw new ApplicationException($"{this.Name}：GetRequireBitDataで定義されていないStation:{st.ToString()}の要求を検知しました。lineconfigを確認して下さい。");
            }

        }
        #endregion

        //【1.124.3】関数Override版追加＋引数buffercode追加
        #region SetAddressDoingLoadMagazine
        public override void SetAddressDoingLoadMagazine(Station st, string buffercode, bool loader, string data)
        {
            int bc;
            if (int.TryParse(buffercode, out bc) == false)
            {
                bc = 1;
            }

            string tgtAddress = null;

            switch (st)
            {
                case Station.EmptyMagazineLoader:
                    tgtAddress = this.EmptyMagLoaderDoBitAddressList[bc];
                    break;

                case Station.Unloader:
                    tgtAddress = this.UnloaderDoBitAddressList[bc];
                    break;

                default:
                    throw new ApplicationException($"{this.Name}：SetAddressDoingLoadMagazineで定義されていないStation:{st.ToString()}の要求を検知しました。lineconfigを確認して下さい。");
            }

            Plc.SetBit(tgtAddress, 1, data);
        }
        #endregion
        
    }
}
