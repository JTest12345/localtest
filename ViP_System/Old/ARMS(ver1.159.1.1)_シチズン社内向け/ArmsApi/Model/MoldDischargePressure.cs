using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class MoldDischargePressure
    {
        /// <summary>
        /// 吐出圧力記録テーブルへのモールドデータ登録
        /// </summary>
        /// <param name="mdPlantCd"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="d4"></param>
        /// <param name="d5"></param>
        /// <param name="d6"></param>
        /// <param name="d7"></param>
        /// <param name="d8"></param>
        public static void Insert(string mdPlantCd, decimal d1, decimal d2, decimal d3, decimal d4, decimal d5, decimal d6
            , decimal d7, decimal d8)
        {
            try
            {
                Log.ApiLog.Info($"吐出圧力記録テーブルへのモールドデータ登録。対象設備番号：「{mdPlantCd}」、吐出圧力「{d1},{d2},{d3},{d4},{d5},{d6},{d7},{d8}」");
                using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
                {
                    var svrData = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();

                    if (svrData != null)
                    {
                        svrData.measurementplantcd = null;
                        svrData.dischargepressureval1 = d1;
                        svrData.dischargepressureval2 = d2;
                        svrData.dischargepressureval3 = d3;
                        svrData.dischargepressureval4 = d4;
                        svrData.dischargepressureval5 = d5;
                        svrData.dischargepressureval6 = d6;
                        svrData.dischargepressureval7 = d7;
                        svrData.dischargepressureval8 = d8;
                        svrData.aftercalculationdischargepressureval1 = null;
                        svrData.aftercalculationdischargepressureval2 = null;
                        svrData.aftercalculationdischargepressureval3 = null;
                        svrData.aftercalculationdischargepressureval4 = null;
                        svrData.aftercalculationdischargepressureval5 = null;
                        svrData.aftercalculationdischargepressureval6 = null;
                        svrData.aftercalculationdischargepressureval7 = null;
                        svrData.aftercalculationdischargepressureval8 = null;
                        svrData.receivingdonefg = false;
                        svrData.moldsendingdonefg = false;
                        svrData.lastupddt = DateTime.Now;
                        svrData.voidfg = false;
                    }
                    else
                    {
                        DataContext.TnMoldDischargePressure regDate = new DataContext.TnMoldDischargePressure
                        {
                            moldplantcd = mdPlantCd,
                            measurementplantcd = null,
                            dischargepressureval1 = d1,
                            dischargepressureval2 = d2,
                            dischargepressureval3 = d3,
                            dischargepressureval4 = d4,
                            dischargepressureval5 = d5,
                            dischargepressureval6 = d6,
                            dischargepressureval7 = d7,
                            dischargepressureval8 = d8,
                            aftercalculationdischargepressureval1 = null,
                            aftercalculationdischargepressureval2 = null,
                            aftercalculationdischargepressureval3 = null,
                            aftercalculationdischargepressureval4 = null,
                            aftercalculationdischargepressureval5 = null,
                            aftercalculationdischargepressureval6 = null,
                            aftercalculationdischargepressureval7 = null,
                            aftercalculationdischargepressureval8 = null,
                            receivingdonefg = false,
                            moldsendingdonefg = false,
                            lastupddt = DateTime.Now,
                            voidfg = false
                        };

                        armsDB.TnMoldDischargePressure.InsertOnSubmit(regDate);
                    }
                    armsDB.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"吐出圧力記録テーブルへのデータ登録に失敗しました。対象設備番号：「{mdPlantCd}」、吐出圧力「{d1},{d2},{d3},{d4},{d5},{d6},{d7},{d8}」、エラー内容：{ex.ToString()}");
            }
        }

        /// <summary>
        /// 吐出圧力記録テーブルへの測定機データ登録
        /// </summary>
        /// <param name="mdPlantCd"></param>
        /// <param name="measurementPlantCd"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="d4"></param>
        /// <param name="d5"></param>
        /// <param name="d6"></param>
        /// <param name="d7"></param>
        /// <param name="d8"></param>
        public static void Update(string mdPlantCd, string measurementPlantCd, decimal d1, decimal d2, decimal d3, decimal d4, decimal d5, decimal d6
            , decimal d7, decimal d8)
        {
            try
            {
                Log.ApiLog.Info($"吐出圧力記録テーブルへの測定機データ登録。モールド設備番号：「{mdPlantCd}」、測定機設備番号：「{measurementPlantCd}」、吐出圧力「{d1},{d2},{d3},{d4},{d5},{d6},{d7},{d8}」");
                using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
                {
                    var svrData = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();

                    if (svrData != null)
                    {
                        svrData.measurementplantcd = measurementPlantCd;
                        svrData.aftercalculationdischargepressureval1 = d1;
                        svrData.aftercalculationdischargepressureval2 = d2;
                        svrData.aftercalculationdischargepressureval3 = d3;
                        svrData.aftercalculationdischargepressureval4 = d4;
                        svrData.aftercalculationdischargepressureval5 = d5;
                        svrData.aftercalculationdischargepressureval6 = d6;
                        svrData.aftercalculationdischargepressureval7 = d7;
                        svrData.aftercalculationdischargepressureval8 = d8;
                        svrData.receivingdonefg = true;
                        svrData.moldsendingdonefg = false;
                        svrData.lastupddt = DateTime.Now;
                        armsDB.SubmitChanges();
                    }
                    else
                    {
                        throw new ApplicationException("モールド装置の登録がありませんでした。");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"吐出圧力記録テーブルへのデータ登録に失敗しました。モールド設備番号：「{mdPlantCd}」、測定機設備番号：「{measurementPlantCd}」、吐出圧力「{d1},{d2},{d3},{d4},{d5},{d6},{d7},{d8}」、エラー内容：{ex.ToString()}");
            }
        }

        /// <summary>
        /// モールド装置への変更吐出圧力送信完了フラグON
        /// </summary>
        /// <param name="mdPlantCd"></param>
        public static void MoldSendingDone(string mdPlantCd)
        {
            try
            {
                Log.ApiLog.Info($"吐出圧力記録テーブルへデータ登録(モールド装置への変更吐出圧力送信完了フラグON)。モールド設備番号：「{mdPlantCd}」");
                using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
                {
                    var svrData = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();

                    if (svrData != null)
                    {
                        svrData.receivingdonefg = false;
                        svrData.moldsendingdonefg = true;
                        svrData.lastupddt = DateTime.Now;
                        armsDB.SubmitChanges();
                    }
                    else
                    {
                        throw new ApplicationException("モールド装置の登録がありませんでした。");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"吐出圧力記録テーブルへのデータ登録に失敗しました。モールド設備番号：「{mdPlantCd}」、エラー内容：{ex.ToString()}");
            }
        }

        /// <summary>
        /// 完了フラグをON
        /// </summary>
        /// <param name="measurementPlantCd"></param>
        public static void SetComplete(string measurementPlantCd)
        {
            try
            {
                Log.ApiLog.Info($"吐出圧力記録テーブルへデータ登録(完了フラグをON)。測定機設備番号：「{measurementPlantCd}」");
                using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
                {
                    var svrData = armsDB.TnMoldDischargePressure.Where(r => r.measurementplantcd == measurementPlantCd && r.voidfg == false).OrderByDescending(r => r.lastupddt).FirstOrDefault();

                    if (svrData != null)
                    {
                        svrData.receivingdonefg = false;
                        svrData.moldsendingdonefg = false;
                        svrData.lastupddt = DateTime.Now;
                        svrData.voidfg = true;
                        armsDB.SubmitChanges();
                    }
                    else
                    {
                        throw new ApplicationException("測定機の登録がありませんでした。");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"吐出圧力記録テーブルへのデータ登録に失敗しました。測定機設備番号：「{measurementPlantCd}」、エラー内容：{ex.ToString()}");
            }
        }


        /// <summary>
        /// 吐出圧力送信済みフラグがONであるかどうか
        /// </summary>
        /// <param name="measurementPlantCd"></param>
        /// <returns></returns>
        public static bool IsSend(string measurementPlantCd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var data = armsDB.TnMoldDischargePressure.Where(r => r.measurementplantcd == measurementPlantCd && r.voidfg == false).OrderByDescending(r => r.lastupddt).FirstOrDefault();
                if (data != null)
                {
                    return data.moldsendingdonefg;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 測定器からの結果書込が完了したかどうか
        /// </summary>
        /// <param name="mdPlantCd"></param>
        /// <returns></returns>
        public static bool IsDataReceived(string mdPlantCd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var data = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();
                if (data != null)
                {
                    return data.receivingdonefg;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// MD吐出圧力取得
        /// </summary>
        /// <param name="mdPlantCd"></param>
        /// <returns></returns>
        public static List<decimal> GetDischargePressureData(string mdPlantCd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var data = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();
                if (data != null)
                {
                    List<decimal> retv = new List<decimal>();
                    if (data.dischargepressureval1.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval1.Value);
                    }
                    if (data.dischargepressureval2.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval2.Value);
                    }
                    if (data.dischargepressureval3.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval3.Value);
                    }
                    if (data.dischargepressureval4.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval4.Value);
                    }
                    if (data.dischargepressureval5.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval5.Value);
                    }
                    if (data.dischargepressureval6.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval6.Value);
                    }
                    if (data.dischargepressureval7.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval7.Value);
                    }
                    if (data.dischargepressureval8.HasValue == true)
                    {
                        retv.Add(data.dischargepressureval8.Value);
                    }

                    return retv;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// MD反射材3D測定機からの変更吐出圧力取得
        /// </summary>
        /// <param name="mdPlantCd"></param>
        /// <returns></returns>
        public static List<decimal> GetMeasurementData(string mdPlantCd)
        {
            using (var armsDB = new DataContext.ARMSDataContext(SQLite.ConStr))
            {
                var data = armsDB.TnMoldDischargePressure.Where(r => r.moldplantcd == mdPlantCd).FirstOrDefault();
                if (data != null)
                {
                    List<decimal> retv = new List<decimal>();
                    if (data.aftercalculationdischargepressureval1.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval1.Value);
                    }
                    if (data.aftercalculationdischargepressureval2.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval2.Value);
                    }
                    if (data.aftercalculationdischargepressureval3.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval3.Value);
                    }
                    if (data.aftercalculationdischargepressureval4.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval4.Value);
                    }
                    if (data.aftercalculationdischargepressureval5.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval5.Value);
                    }
                    if (data.aftercalculationdischargepressureval6.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval6.Value);
                    }
                    if (data.aftercalculationdischargepressureval7.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval7.Value);
                    }
                    if (data.aftercalculationdischargepressureval8.HasValue == true)
                    {
                        retv.Add(data.aftercalculationdischargepressureval8.Value);
                    }

                    return retv;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
