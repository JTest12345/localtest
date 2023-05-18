using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 基板搭載機(SIGMA用)
    /// </summary>
    class SubstrateLoader : MachineBase
    {
        /// <summary>
        /// データ確認要求
        /// </summary>
        private const string STR_ADDRESS_DATA_READ_REQUEST = "B1012";
        /// <summary>
        /// マガジンNOアドレス
        /// </summary>
        private const string STR_ADDRESS_MAGAZINENO = "W1010";
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
            //マガジンNo取得
            string mag = this.Plc.GetMagazineNo(STR_ADDRESS_MAGAZINENO, MAGAZINENO_WORD_LENGTH);
            if (string.IsNullOrWhiteSpace(mag))
            {
                throw new ApplicationException("装置からマガジン内容が取得できませんでした。");
            }
            
            //基板DM取得
            List<string> datamatrixList = getDataMatrix();
            if (datamatrixList.Count == 0)
            {
                throw new ApplicationException("装置から基板DM内容が取得できませんでした。");
            }

            //作業開始完了時間取得
            VirtualMag newMagazine = new VirtualMag();
            DateTime? workcomplete;
            DateTime? workstart;
            getDate(out workcomplete, out workstart);

            newMagazine.LastMagazineNo = mag;
            newMagazine.MagazineNo = mag;
            newMagazine.WorkComplete = workcomplete;
            newMagazine.WorkStart = workstart;

            if (!this.WorkComplete(newMagazine, this, true))
            {
                throw new ApplicationException("完了処理失敗");
            }

            //基板DMの保存
            Magazine svrmag = Magazine.GetCurrent(newMagazine.MagazineNo);
            if (svrmag == null)
            {
                throw new ApplicationException("マガジン情報が見つかりません:" + newMagazine.MagazineNo);
            }
            string lotno = svrmag.NascaLotNO;
            string magazineno = svrmag.MagazineNo;
            foreach (string datamatrix in datamatrixList)
            {
                LotCarrier lotcarrier = new LotCarrier(lotno, datamatrix);
                lotcarrier.Insert();
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

        private List<string> getDataMatrix()
        {
            List<string> dmList = new List<string>();

            if (this.Plc == null) return dmList;

            for (int i = 0; i < DATAMATRIX_ROWS; i++)
            {
                int sNewAddrVal = STR_ADDRESS_DATAMATRIX_START + DATAMATRIX_ROW_OFFSET * i;
                string address = DATAMATRIX_ADDRESS_HEADER + sNewAddrVal.ToString("x").ToUpper();

                string retv = Plc.GetMagazineNo(address, DATAMATRIX_WORD_LENGTH);
                if (!string.IsNullOrEmpty(retv))
                {
                    dmList.Add(retv);
                }
            }

            return dmList;
        }
    }
}
