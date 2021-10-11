using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArmsApi.Model
{    
    public class CarrireWorkData
    {

        #region プロパティ
        public string CarrierNo { get; set; }
        public string LotNo { get; set; }
        public long ProcNo { get; set; }
        public string Infoid { get; set; }
        public string InfoName { get; set; }
        public string Value { get; set; }
        public int Delfg { get; set; }
        public DateTime LastUpdDt { get; set; }
        #endregion

        #region 情報種類

        public const string MAGAZINE_STEP_INFOCD = "1";
        public const string IN_SURFACE_ADDR_INFOCD = "2";
        public const string IN_STAGE_ADDR_INFOCD = "3";
        public const string PRESS_ADDR_INFOCD = "4";
        public const string STAGENO_INFOCD = "5";
        public const string IN_PRESSADDR_INFOCD = "6";

        #endregion

        /// <summary>
        /// キー重複の旧データが有ったら削除して登録する。
        /// </summary>
        /// <param name="carrierNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="procNo"></param>
        /// <param name="infoId"></param>
        /// <param name="value"></param>
        /// <param name="delFg"></param>
        public void InsertUpdate() 
        {
            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                var svrData = db.TnCarrierWorkData.Where(c => c.carrierno == this.CarrierNo && c.lotno == this.LotNo &&
                                                            c.procno == this.ProcNo && c.infoid == this.Infoid).FirstOrDefault();

                if(svrData != null)
                {
                    svrData.value = this.Value;
                    svrData.lastupddt = DateTime.Now;
                }
                else
                {
                    DataContext.TnCarrierWorkData regDate = new DataContext.TnCarrierWorkData
                    {
                        carrierno = this.CarrierNo,
                        lotno = this.LotNo,
                        procno = this.ProcNo,
                        infoid = this.Infoid,
                        value = this.Value,
                        delfg = this.Delfg,
                        lastupddt = DateTime.Now
                    };

                    db.TnCarrierWorkData.InsertOnSubmit(regDate);
                }
                db.SubmitChanges();
            }
        }

        public static bool Exists(string carrierNo, long procNo, string infoId)
        {           
            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                var d = db.TnCarrierWorkData.Join(db.TmGeneral, a=> a.infoid, b=>b.code,(a,b) => new
                {
                    a.carrierno,
                    a.lotno,
                    a.procno,
                    a.infoid,
                    a.value,
                    a.delfg,
                    a.lastupddt,
                    b.tid,
                    b.val
                }).Where(c => c.carrierno == carrierNo && c.procno == procNo && c.infoid == infoId && c.tid == GnlMaster.TID_CARRIERWORKINFOCD).SingleOrDefault();

                if (d == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static List<CarrireWorkData> GetDataFromLot(string lotNo)
        {
            List<CarrireWorkData> retv = new List<CarrireWorkData>();

            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                var dataList = db.TnCarrierWorkData.Join(db.TmGeneral, a => a.infoid, b => b.code, (a, b) => new
                {
                    a.carrierno,
                    a.lotno,
                    a.procno,
                    a.infoid,
                    a.value,
                    a.delfg,
                    a.lastupddt,
                    b.tid,
                    b.val
                }).Where(c => c.lotno == lotNo && c.tid == GnlMaster.TID_CARRIERWORKINFOCD).ToList();

                if (dataList.Count() > 0)
                {
                    foreach (var d in dataList)
                    {
                        CarrireWorkData temp = new CarrireWorkData();
                        temp.CarrierNo = d.carrierno;
                        temp.LotNo = d.lotno;
                        temp.ProcNo = d.procno;
                        temp.Infoid = d.infoid;
                        temp.Value = d.value;
                        temp.InfoName = d.val;
                        temp.Delfg = d.delfg;
                        temp.LastUpdDt = d.lastupddt;

                        retv.Add(temp);
                    }
                }
            }
            return retv;
        }

        /// <summary>
        /// マガジン段数を自動で算出・登録するための関数。過去データから段数を自動的に判定して最大値＋１を現段数として登録する。
        /// 既に同キャリアで過去データがある場合、過去データを削除したうえで全データを書き込みなおす。
        /// ※呼出前に既に段数が分かっている場合はこの関数は使わず普通にDeleteInsertを使う※
        /// </summary>
        public void RegisterMagazineStepWithAutotCalc()
        {
            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                List<CarrireWorkData> svrDataList = GetDataFromLot(this.LotNo);

                //マガジン段数情報抽出(段数情報で降順に並び替え)
                List<CarrireWorkData> carrierDataList = svrDataList.Where(s => s.ProcNo == this.ProcNo && s.LotNo == this.LotNo
                                                                            && s.Infoid == MAGAZINE_STEP_INFOCD).OrderByDescending(s => Convert.ToInt32(s.Value)).ToList();

                //一切情報が無い場合は1段目として登録
                if (carrierDataList.Count() == 0)
                {
                    this.Value = "1";
                    this.InsertUpdate();
                    return;
                }

                //現キャリアの情報が既に登録済みかチェック
                CarrireWorkData correntCarrierData = carrierDataList.Where(c => c.CarrierNo == this.CarrierNo).FirstOrDefault();

                //まだ未登録の場合は最新の値に+1したものを段数として判定
                if (correntCarrierData == null)
                {
                    this.Value = Convert.ToString(Convert.ToInt32(carrierDataList[0].Value) + 1);
                    this.InsertUpdate();
                    return;
                }
                else
                {
                    //自身のデータがある場合、自分を除いて順に並べ直して再登録する。
                    List<CarrireWorkData> withOutCurrentList = carrierDataList.Where(c => c.CarrierNo != this.CarrierNo).OrderBy(c => Convert.ToInt32(c.Value)).ToList();
                    int step = 1;
                    foreach(CarrireWorkData data in withOutCurrentList)
                    {
                        data.Value = Convert.ToString(step);
                        data.InsertUpdate();
                        step += 1;
                    }
                    this.Value = Convert.ToString(step);
                    this.InsertUpdate();
                    return;
                }
            }
        }        
    }
}
