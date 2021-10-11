using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi.Model;
using ArmsApi;

namespace ARMS3.Model
{
    public class OvenProfiler
    {
        public const int PROFILE_NONE = 0;

        /// <summary>
        /// モールド常温待機完了の何分前から予約ONするかの時間
        /// </summary>
        public const int MoldCVTopMagazineReserveOffsetMinutes = 60;

        #region 各アドレス

        public string ReadyBitAddress { get; set; }
        public int DBOvenNumbers { get; set; }
        public int MoldOvenNumbers { get; set; }

        public string DBChangeInterlockBit { get; set; }
        public string MoldChangeInterlockBit { get; set; }

        public string DBCurrentProfileWord { get; set; }
        public string MoldCurrentProfileWord { get; set; }

        public string DBReserveProfileWord { get; set; }
        public string MoldReserveProfileWord { get; set; }

        public string DBAutoModeBit { get; set; }
        public string MoldAutoModeBit { get; set; }

        public string DBProfileChangeCompleteBit { get; set; }
        public string MoldProfileChangeCompleteBit { get; set; }

        public string DBProfileChangeRequestBit { get; set; }
        public string MoldProfileChangeRequestBit { get; set; }

        public static int? DBProfileReserveProcNo { get; set; }
        public static int? MoldProfileReserveProcNo { get; set; }

        #endregion


        #region class OvenData

        private class OvenData
        {
            public int Number { get; set; }
            public bool Interlock { get; set; }
            public int CurrentProfile { get; set; }
            public int ReservedProfile { get; set; }
            public bool IsAutoMode { get; set; }
            public bool IsChangeComplete { get; set; }
            public bool IsChangeRequest { get; set; }

            public OvenData(int number)
            {
                this.Number = number;
            }
        }
        #endregion

        private OvenData[] Diebond;

        private OvenData[] Mold;


        #region コンストラクタ
        public OvenProfiler()
        {

        }
        #endregion


        public void Initialize(IPLC plc)
        {
            //PLCにReady通知
            plc.SetBit(ReadyBitAddress, 1, Mitsubishi.BIT_ON);

            List<OvenData> db = new List<OvenData>();
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                db.Add(new OvenData(i));
            }

            Diebond = db.ToArray();

            List<OvenData> md = new List<OvenData>();
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                md.Add(new OvenData(i));
            }

            Mold = md.ToArray();
        }

        public void SetReady(Mitsubishi plc)
        {
            //PLCにReady通知
            plc.SetBit(ReadyBitAddress, 1, Mitsubishi.BIT_ON);
        }

        #region initializeDB

        private void initializeDB(Mitsubishi plc)
        {
            //インターロック
            string interlocks = plc.GetBit(DBChangeInterlockBit, DBOvenNumbers);
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].Interlock = (interlocks[i] == '1') ? true : false;
            }

            //現プロファイル
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].CurrentProfile = plc.GetWordAsDecimalData(offsetAddr(DBCurrentProfileWord, i));
            }

            //予約プロファイル
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].ReservedProfile = plc.GetWordAsDecimalData(offsetAddr(DBReserveProfileWord, i));
            }

            //自動モード
            string automodes = plc.GetBit(DBAutoModeBit, DBOvenNumbers);
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].IsAutoMode = (automodes[i] == '1') ? true : false;
            }

            //変更完了
            string changeCompletes = plc.GetBit(DBProfileChangeCompleteBit, DBOvenNumbers);
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].IsChangeComplete = (changeCompletes[i] == '1') ? true : false;
            }


            //変更完了
            string changeRequests = plc.GetBit(DBProfileChangeRequestBit, DBOvenNumbers);
            for (int i = 0; i < DBOvenNumbers; i++)
            {
                Diebond[i].IsChangeRequest = (changeRequests[i] == '1') ? true : false;
            }
        }
        #endregion

        #region initializeMD

        private void initializeMD(Mitsubishi plc)
        {
            //インターロック
            string interlocks = plc.GetBit(MoldChangeInterlockBit, MoldOvenNumbers);
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].Interlock = (interlocks[i] == '1') ? true : false;
            }


            //現プロファイル
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].CurrentProfile = plc.GetWordAsDecimalData(offsetAddr(MoldCurrentProfileWord, i));
            }

            //予約プロファイル
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].ReservedProfile = plc.GetWordAsDecimalData(offsetAddr(MoldReserveProfileWord, i));
            }

            //自動モード
            string automodes = plc.GetBit(MoldAutoModeBit, MoldOvenNumbers);
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].IsAutoMode = (automodes[i] == '1') ? true : false;
            }

            //変更完了
            string changeCompletes = plc.GetBit(MoldProfileChangeCompleteBit, MoldOvenNumbers);
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].IsChangeComplete = (changeCompletes[i] == '1') ? true : false;
            }


            //変更完了
            string changeRequests = plc.GetBit(MoldProfileChangeRequestBit, MoldOvenNumbers);
            for (int i = 0; i < MoldOvenNumbers; i++)
            {
                Mold[i].IsChangeRequest = (changeRequests[i] == '1') ? true : false;
            }
        }
        #endregion

        /// <summary>
        /// Mag.NextProcessIdは必須
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public static int GetProfile(VirtualMag mag)
        {
            Log.ApiLog.Info("プロファイル調査:" + mag.MagazineNo);

            try
            {
                //アオイ基板の場合はベーキング用のプロファイル番号取得
                string[] values = mag.MagazineNo.Split(VirtualMag.MAP_FRAME_SEPERATOR);
                if (values.Length ==VirtualMag.MAP_FRAME_ELEMENT_CT)
                {
                    return MapFrameBaking.GetBakingProfileNo();
                }

                //日亜マガジンの場合はSvrマスタから取得
                if (mag.ProcNo == null)
                {
                    Log.RBLog.Error("仮想マガジン内に次作業工程IDが無いためオーブンプロファイル取得失敗:" + mag.MagazineNo);
                }

                int retv = OvenProfile.GetOvenProfileId(mag.MagazineNo, mag.ProcNo.Value);
                Log.ApiLog.Info("プロファイル取得:" + retv.ToString());
                return retv;
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("オーブンプロファイル取得失敗:" + mag.MagazineNo + ":" + ex.ToString());
            }

            return 0;
        }


        /// <summary>
        /// ダイボンドプロファイル変更
        /// profileNo = PROFILE_NONEで、完了監視のみ
        /// </summary>
        /// <param name="profileNo"></param>
        /// <param name="plc"></param>
        public void ChangeDBProfile(int profileNo, Mitsubishi plc)
        {
            initializeDB(plc);

            changeProfile(Diebond, plc, profileNo, DBProfileChangeRequestBit, DBReserveProfileWord);
        }


        /// <summary>
        /// モールドプロファイル変更
        /// profileNo = PROFILE_NONEで、完了監視のみ
        /// </summary>
        /// <param name="profileNo"></param>
        /// <param name="plc"></param>
        public void ChangeMDProfile(int profileNo, Mitsubishi plc)
        {
            initializeMD(plc);

            changeProfile(Mold, plc, profileNo, MoldProfileChangeRequestBit, MoldReserveProfileWord);
        }


        public static int GetDBReserveProfileNo(VirtualMag mag)
        {
            Log.ApiLog.Info("DBプロファイル調査:" + mag.MagazineNo);
            if (DBProfileReserveProcNo.HasValue)
            {
                try
                {
                    int retv = OvenProfile.GetOvenProfileId(mag.MagazineNo, DBProfileReserveProcNo.Value);
                    Log.ApiLog.Info("プロファイル取得:" + retv.ToString());
                    return retv;
                }
                catch (Exception ex)
                {
                    Log.RBLog.Error("オーブンプロファイル取得失敗:" + mag.MagazineNo);
                }
            }
            return 0;
        }

        public static int GetMDReserveProfileNo(VirtualMag mag)
        {
            Log.ApiLog.Info("DBプロファイル調査:" + mag.MagazineNo);
            if (MoldProfileReserveProcNo.HasValue)
            {
                try
                {
                    int retv = OvenProfile.GetOvenProfileId(mag.MagazineNo, MoldProfileReserveProcNo.Value);
                    Log.ApiLog.Info("プロファイル取得:" + retv.ToString());
                    return retv;
                }
                catch (Exception ex)
                {
                    Log.ApiLog.Error("オーブンプロファイル取得失敗:" + mag.MagazineNo);
                }
            }
            return 0;


        }


        private void changeProfile(OvenData[] ovens, Mitsubishi plc, int newProfile,
            string changeRequestStartAddr, string newprofileStartAddr)
        {
            foreach (OvenData oven in ovens)
            {
                //  完了フラグが立っている場合は変更要求をPC側から立ち下げ
                if (oven.IsChangeComplete == true)
                {
                    //  予約プロファイル番号を0に戻す
                    plc.SetWordAsDecimalData(offsetAddr(newprofileStartAddr, oven.Number), PROFILE_NONE);
                    //  要求立ち下げ
                    plc.SetBit(offsetAddr(changeRequestStartAddr, oven.Number), 1, Mitsubishi.BIT_OFF);
                }

                //  インタロック中は切り替え操作禁止
                if (oven.Interlock == true)
                {
                    continue;
                }

                //  オーブンが手動モードなら操作不可
                if (oven.IsAutoMode == false)
                {
                    continue;
                }

                //  番号0のプロファイルは変更要求として処理しない
                if (newProfile == PROFILE_NONE)
                {
                    continue;
                }


                //  既に予約済みなら何もしない
                if (oven.ReservedProfile != 0 && oven.ReservedProfile != newProfile)
                {
                    continue;
                }

                //  プロファイルが一致している場合は何もしない
                if (oven.CurrentProfile == newProfile)
                {
                    continue;

                }

                //  予約プロファイルを設定
                plc.SetWordAsDecimalData(offsetAddr(newprofileStartAddr, oven.Number), newProfile);
                //  変更要求ON
                plc.SetBit(offsetAddr(changeRequestStartAddr, oven.Number), 1, Mitsubishi.BIT_ON);
            }
        }


        private string offsetAddr(string orgAddress, int offset)
        {
            if (orgAddress.Length != 7)
            {
                throw new ApplicationException("アドレスの形式が不正です");
            }

            string prefix = orgAddress.Substring(0, 1);
            string hex = orgAddress.Substring(1, 6);

            int address = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            address = address + offset;

            return prefix + address.ToString("X").PadLeft(6, '0');
        }
    }
}
