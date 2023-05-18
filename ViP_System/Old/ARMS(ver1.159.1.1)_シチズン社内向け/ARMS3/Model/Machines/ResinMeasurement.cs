using ARMS3.Model.PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 反射材樹脂量3D測定
    /// </summary>
    public class ResinMeasurement : MachineBase
    {
        /// <summary>
        /// 取得要求
        /// </summary>
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_REQ_ADDRESS = "B3FB2";

        /// <summary>
        /// 吐出圧力
        /// </summary>
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_1 = "W3FE0";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_2 = "W3FE1";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_3 = "W3FE2";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_4 = "W3FE3";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_5 = "W3FE4";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_6 = "W3FE5";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_7 = "W3FE6";
        private const string OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_8 = "W3FE7";

        /// <summary>
        /// 測定結果
        /// </summary>
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_1 = "W3FC0";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_2 = "W3FC1";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_3 = "W3FC2";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_4 = "W3FC3";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_5 = "W3FC4";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_6 = "W3FC5";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_7 = "W3FC6";
        private const string INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_8 = "W3FC7";

        /// <summary>
        /// 設備番号
        /// </summary>
        private const string PLANT_CD_ADDRESS = "W3FB0";
        private const int PLANT_CD_WORD_LENGTH = 4;

        /// <summary>
        /// MD機設備番
        /// </summary>
        private const string MD_PLANT_CD_ADDRESS = "W3FB4";

        /// <summary>
        /// 照合OK
        /// </summary>
        private const string READ_DISCHARGE_PRESSURE_DATA_OK_ADDRESS = "B3FD2";
        /// <summary>
        /// 照合NG
        /// </summary>
        private const string READ_DISCHARGE_PRESSURE_DATA_NG_ADDRESS = "B3FD3";
        /// <summary>
        /// 吐出圧力転送完了
        /// </summary>
        private const string RESULT_SEND_COMPLETE = "B3FD4";
        /// <summary>
        /// ハートビート
        /// </summary>
        private const string HEART_BEAT_ADDRESS0 = "EM40000";
        private const string HEART_BEAT_ADDRESS1 = "EM40001";
        private const string HEART_BEAT_ADDRESS2 = "EM40002";
        private const string HEART_BEAT_ADDRESS3 = "EM40003";
        private const string HEART_BEAT_ADDRESS4 = "EM40004";
        private const string HEART_BEAT_ADDRESS5 = "EM40005";
        private const string HEART_BEAT_ADDRESS6 = "EM40006";
        private const string HEART_BEAT_ADDRESS7 = "EM40007";
        private const string HEART_BEAT_ADDRESS8 = "EM40008";
        private const string HEART_BEAT_ADDRESS9 = "EM40009";

        protected override void concreteThreadWork()
        {
            try
            {
                //ハートビート
                Plc.SetBit(HEART_BEAT_ADDRESS0, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS1, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS2, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS3, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS4, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS5, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS6, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS7, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS8, 1, Keyence.BIT_ON);
                Plc.SetBit(HEART_BEAT_ADDRESS9, 1, Keyence.BIT_ON);


                //接続要求時
                if (base.IsRequireConnection() == true)
                {
                    base.ConnectionProcess();
                }

                //取得要求時
                if (isRequireOutputDischargePressureData() == true)
                {
                    outputDischargePressureData();
                }

                //出力要求時
                if (base.IsRequireReadDischargePressureData() == true)
                {
                    readDischargePressureData();
                }

                //吐出圧力記録テーブルに記録されている設備レコードの送信済みフラグがONであれば吐出圧力転送完了信号をON
                if (ArmsApi.Model.MoldDischargePressure.IsSend(this.PlantCd) == true)
                {
                    Plc.SetBit(RESULT_SEND_COMPLETE, 1, Keyence.BIT_ON);

                    //吐出圧力記録テーブルの完了フラグをON
                    ArmsApi.Model.MoldDischargePressure.SetComplete(this.PlantCd);
                }

                //切断要求時
                if (base.IsRequireDisconnect() == true)
                {
                    base.DisconnectionProcess();
                }

                //ハートビート
                Plc.SetBit(HEART_BEAT_ADDRESS0, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS1, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS2, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS3, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS4, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS5, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS6, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS7, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS8, 1, Keyence.BIT_OFF);
                Plc.SetBit(HEART_BEAT_ADDRESS9, 1, Keyence.BIT_OFF);
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 取得要求
        /// </summary>
        /// <returns></returns>
        private bool isRequireOutputDischargePressureData()
        {
            string retv;
            try
            {
                retv = Plc.GetBit(OUTPUT_DISCHARGE_PRESSURE_DATA_REQ_ADDRESS);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、取得要求OFF扱い。アドレス：『{OUTPUT_DISCHARGE_PRESSURE_DATA_REQ_ADDRESS}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Keyence.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 吐出圧力を設定
        /// </summary>
        private void outputDischargePressureData()
        {
            this.OutputApiLog($"吐出圧力設定処理開始");

            //設備番号
            string plantCd = Plc.GetString(PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH, true);
            //Plc.GetMagazineNo(PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH);
            this.OutputApiLog($"[{this.PlantCd}]装置から取得した設備番号:{plantCd}");
            if (string.IsNullOrWhiteSpace(plantCd) == true)
            {
                throw new ApplicationException($"[{this.PlantCd}]設備番号を取得できませんでした。");
            }
            //MD機設備番号
            string mdPlantCd = Plc.GetString(MD_PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH, true);
            //Plc.GetMagazineNo(MD_PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH);
            this.OutputApiLog($"[{this.PlantCd}]装置から取得したMD機設備番号:{mdPlantCd}");
            if (string.IsNullOrWhiteSpace(plantCd) == true)
            {
                throw new ApplicationException($"[{this.PlantCd}]MD機設備番号を取得できませんでした。");
            }

            //吐出圧力
            List<decimal> dischargePressureData = ArmsApi.Model.MoldDischargePressure.GetDischargePressureData(mdPlantCd);
            if (dischargePressureData == null ||
                dischargePressureData.Count != 8)
            {
                throw new ApplicationException($"[{this.PlantCd}]吐出圧力値を取得できませんでした。設備番号：{plantCd}、 MD機設備番号：{mdPlantCd}");
            }

            this.OutputApiLog($"吐出圧力値書込:{this.PlantCd}、吐出圧力「{Convert.ToInt32(dischargePressureData[0])},{Convert.ToInt32(dischargePressureData[1])},{Convert.ToInt32(dischargePressureData[2])},{Convert.ToInt32(dischargePressureData[3])},{Convert.ToInt32(dischargePressureData[4])},{Convert.ToInt32(dischargePressureData[5])},{Convert.ToInt32(dischargePressureData[6])},{Convert.ToInt32(dischargePressureData[7])}」");
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_1, Convert.ToInt32(dischargePressureData[0]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_2, Convert.ToInt32(dischargePressureData[1]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_3, Convert.ToInt32(dischargePressureData[2]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_4, Convert.ToInt32(dischargePressureData[3]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_5, Convert.ToInt32(dischargePressureData[4]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_6, Convert.ToInt32(dischargePressureData[5]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_7, Convert.ToInt32(dischargePressureData[6]));
            Plc.SetWordAsDecimalData(OUTPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_8, Convert.ToInt32(dischargePressureData[7]));

            //取得要求信号をOFF
            this.OutputApiLog($"取得要求信号をOFF");
            Plc.SetBit(OUTPUT_DISCHARGE_PRESSURE_DATA_REQ_ADDRESS, 1, Keyence.BIT_OFF);

            this.OutputApiLog($"吐出圧力設定処理完了");
        }

        /// <summary>
        /// 変更吐出圧力取得
        /// </summary>
        private void readDischargePressureData()
        {
            try
            {
                this.OutputApiLog($"変更吐出圧力取得処理開始");

                //設備番号
                string plantCd = Plc.GetString(PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH, true);
                //Plc.GetMagazineNo(PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH);
                this.OutputApiLog($"[{this.PlantCd}]装置から取得した設備番号:{plantCd}");
                if (string.IsNullOrWhiteSpace(plantCd) == true)
                {
                    throw new ApplicationException($"[{this.PlantCd}]設備番号を取得できませんでした。");
                }
                //MD機設備番号
                string mdPlantCd = Plc.GetString(MD_PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH, true);
                //Plc.GetMagazineNo(MD_PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH);
                this.OutputApiLog($"[{this.PlantCd}]装置から取得したMD機設備番号:{mdPlantCd}");
                if (string.IsNullOrWhiteSpace(plantCd) == true)
                {
                    throw new ApplicationException($"[{this.PlantCd}]MD機設備番号を取得できませんでした。");
                }
                //変更吐出圧力
                string d1 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_1);
                string d2 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_2);
                string d3 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_3);
                string d4 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_4);
                string d5 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_5);
                string d6 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_6);
                string d7 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_7);
                string d8 = Plc.GetBit(INPUT_DISCHARGE_PRESSURE_DATA_ADDRESS_8);
                if (string.IsNullOrWhiteSpace(d1) == true ||
                    string.IsNullOrWhiteSpace(d2) == true ||
                    string.IsNullOrWhiteSpace(d3) == true ||
                    string.IsNullOrWhiteSpace(d4) == true ||
                    string.IsNullOrWhiteSpace(d5) == true ||
                    string.IsNullOrWhiteSpace(d6) == true ||
                    string.IsNullOrWhiteSpace(d7) == true ||
                    string.IsNullOrWhiteSpace(d8) == true)
                {
                    throw new ApplicationException($"[{this.PlantCd}]吐出圧力値を取得できませんでした。設備番号：{plantCd}、 MD機設備番号：{mdPlantCd}");
                }

                decimal dischargePressureData1 = Convert.ToDecimal(d1);
                decimal dischargePressureData2 = Convert.ToDecimal(d2);
                decimal dischargePressureData3 = Convert.ToDecimal(d3);
                decimal dischargePressureData4 = Convert.ToDecimal(d4);
                decimal dischargePressureData5 = Convert.ToDecimal(d5);
                decimal dischargePressureData6 = Convert.ToDecimal(d6);
                decimal dischargePressureData7 = Convert.ToDecimal(d7);
                decimal dischargePressureData8 = Convert.ToDecimal(d8);

                this.OutputApiLog($"[{this.PlantCd}]装置から取得した吐出圧力:「{Convert.ToInt32(dischargePressureData1)},{Convert.ToInt32(dischargePressureData2)},{Convert.ToInt32(dischargePressureData3)},{Convert.ToInt32(dischargePressureData4)},{Convert.ToInt32(dischargePressureData5)},{Convert.ToInt32(dischargePressureData6)},{Convert.ToInt32(dischargePressureData7)},{Convert.ToInt32(dischargePressureData8)}」");

                //吐出圧力記録テーブルデータへ記録
                ArmsApi.Model.MoldDischargePressure.Update(mdPlantCd, plantCd, dischargePressureData1, dischargePressureData2, dischargePressureData3
                , dischargePressureData4, dischargePressureData5, dischargePressureData6, dischargePressureData7, dischargePressureData8);

                //OK判定信号をON
                this.OutputApiLog($"OK判定信号をON");
                Plc.SetBit(READ_DISCHARGE_PRESSURE_DATA_NG_ADDRESS, 1, Keyence.BIT_OFF);
                Plc.SetBit(READ_DISCHARGE_PRESSURE_DATA_OK_ADDRESS, 1, Keyence.BIT_ON);

                //出力要求信号をOFF
                this.OutputApiLog($"出力要求信号をOFF");
                Plc.SetBit(this.ReadDischargePressureDataReqBitAddress, 1, Keyence.BIT_OFF);
            }
            catch (Exception ex)
            {
                //NG判定信号をON
                this.OutputApiLog($"NG判定信号をON");
                Plc.SetBit(READ_DISCHARGE_PRESSURE_DATA_OK_ADDRESS, 1, Keyence.BIT_OFF);
                Plc.SetBit(READ_DISCHARGE_PRESSURE_DATA_NG_ADDRESS, 1, Keyence.BIT_ON);
                throw ex;
            }

            this.OutputApiLog($"変更吐出圧力取得処理完了");
        }
    }
}
