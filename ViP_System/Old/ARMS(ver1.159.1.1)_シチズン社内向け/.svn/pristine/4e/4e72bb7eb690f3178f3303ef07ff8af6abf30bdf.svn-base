using ARMS3.Model.PLC;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// テープカット収納機(SIGMA用)
    /// </summary>
    class TapePeeler : MachineBase
    {
        /// <summary>
        /// データ確認要求
        /// </summary>
        private const string STR_ADDRESS_DATA_READ_REQUEST = "B1012";
        /// <summary>
        /// マガジンNOアドレス(ULD)
        /// </summary>
        private const string STR_ADDRESS_MAGAZINENO_ULD = "W1010";
        /// <summary>
        /// マガジンNOアドレス(LD)
        /// </summary>
        private const string STR_ADDRESS_MAGAZINENO_LD = "W1020";
        ///// <summary>
        ///// 作業完了時間アドレス
        ///// </summary>
        //private const string STR_ADDRESS_WORK_COMPLETE_TIME = "W1006";
        /// <summary>
        /// 作業開始時間アドレス
        /// </summary>
        private const string STR_ADDRESS_WORK_START_TIME = "W1000";
        /// <summary>
        /// DataMatrix開始アドレス
        /// </summary>
        private const int STR_ADDRESS_DATAMATRIX_START = 4144;
        private const string DATAMATRIX_ADDRESS_HEADER = "W";
        private const int DATAMATRIX_ROWS = 25;
        private const int DATAMATRIX_ROW_OFFSET = 16;

        private const int MAGAZINENO_WORD_LENGTH = 14;
        private const int DATAMATRIX_WORD_LENGTH = 14;

        private const int DATE_DATA_LENGTH = 12;
        private const int DATE_TIME_ADDRESS_LENGTH = 6;
        private const int WORK_START_ADDRESS_OFFSET = 0;
        private const int WORK_COMPLETE_ADDRESS_OFFSET = 6;

        protected override void concreteThreadWork() 
        {
            try
            {
                if (isDataReadRequest())
                {
                    workComplete();
                }
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw;
                }
            }
        }

        private bool isDataReadRequest()
        {
            if (this.Plc == null) return false;

            if (this.Plc.GetBit(STR_ADDRESS_DATA_READ_REQUEST) == Keyence.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            //マガジンNo取得（LD）
            string magLD = this.Plc.GetMagazineNo(STR_ADDRESS_MAGAZINENO_LD, MAGAZINENO_WORD_LENGTH);
            if (string.IsNullOrWhiteSpace(magLD))
            {
                throw new ApplicationException("LDマガジン内容が取得できませんでした。");
            }
            //マガジンNo取得（ULD）
            string magULD = this.Plc.GetMagazineNo(STR_ADDRESS_MAGAZINENO_ULD, MAGAZINENO_WORD_LENGTH);
            if (string.IsNullOrWhiteSpace(magULD))
            {
                throw new ApplicationException("ULDマガジン内容が取得できませんでした。");
            }

            //キャリアNO取得
            List<string> carrierList = getCarrier();
            if (carrierList.Count == 0)
            {
                throw new ApplicationException("キャリアNO内容が取得できませんでした。");
            }

            //作業開始完了時間取得
            DateTime? workcomplete;
            DateTime? workstart;
            getDate(out workcomplete, out workstart);

            //TnMagのnewfgを0に更新
            Magazine svrmag = Magazine.GetCurrent(magLD);
            if (svrmag == null)
            {
                throw new ApplicationException("マガジン情報が見つかりません:" + magLD);
            }
            svrmag.NewFg = false;
            svrmag.Update();

            //TnMagに新規紐付データを登録
            Magazine mag = new Magazine();
            mag.MagazineNo = magULD;
            mag.NascaLotNO = svrmag.NascaLotNO;
            mag.NewFg = true;
            mag.NowCompProcess = svrmag.NowCompProcess;
            mag.Update();

            ////TnLotCarrier紐付け直し
            //string[] list = LotCarrier.GetCarrierNo(svrmag.NascaLotNO, magLD, true);
            //LotCarrier.ReplaceMagazine(svrmag.NascaLotNO, magLD, magULD, list.ToList());

            //使用履歴記録
            foreach (string carrierno in carrierList)
            {
                UseHistory his = new UseHistory(UseHistory.Category.Carrier, carrierno, workcomplete.Value, "660", false);
                his.Insert();
            }

            //データ確認要求をOFF
            this.Plc.SetBit(STR_ADDRESS_DATA_READ_REQUEST, 1, Keyence.BIT_OFF);
        }

        private void getDate(out DateTime? workComplete, out DateTime? workStart)
        {
            string[] raw = this.Plc.GetBitArray(STR_ADDRESS_WORK_START_TIME, DATE_DATA_LENGTH);
            string[] tmpworkstart = new string[DATE_TIME_ADDRESS_LENGTH];
            string[] tmpworkcomplete = new string[DATE_TIME_ADDRESS_LENGTH];
            Array.Copy(raw, WORK_START_ADDRESS_OFFSET, tmpworkstart, 0, DATE_TIME_ADDRESS_LENGTH);
            Array.Copy(raw, WORK_COMPLETE_ADDRESS_OFFSET, tmpworkcomplete, 0, DATE_TIME_ADDRESS_LENGTH);
            workStart = Plc.GetWordsAsDateTime(tmpworkstart);
            workComplete = Plc.GetWordsAsDateTime(tmpworkcomplete);
        }

        private List<string> getCarrier()
        {
            List<string> carrierList = new List<string>();

            if (this.Plc == null) return carrierList;

            for (int i = 0; i < DATAMATRIX_ROWS; i++)
            {
                int sNewAddrVal = STR_ADDRESS_DATAMATRIX_START + DATAMATRIX_ROW_OFFSET * i;
                string address = DATAMATRIX_ADDRESS_HEADER + sNewAddrVal.ToString("x").ToUpper();

                string retv = Plc.GetMagazineNo(address, DATAMATRIX_WORD_LENGTH);
                if (!string.IsNullOrEmpty(retv))
                {
                    carrierList.Add(retv);
                }
            }

            return carrierList;
        }
    }
}
