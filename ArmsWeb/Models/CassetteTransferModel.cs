using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;


namespace ArmsWeb.Models
{
    public class CassetteTransferModel
    {
        public string LotNo { get; set; }
        public class CassetteCng 
        {
            public string cassetteno { get; set; }
            public string nextcassetteno { get; set; }
        }

        public CassetteCng cassetteCng { get; set; }
        public List<CassetteCng> listCassetteCng { get; set; }

        public string precassetteno { get; set; }
        public string prenextcassetteno { get; set; }
        /*
        public class procinfo
        {
            public int procno { get; set; }
            public string procnm { get; set; }
        }*/
        public int selectedprocno { get; set; }
        Cassette.procinfo procinfo { get; set; }
        public List<Cassette.procinfo> listProcinfo { get; set; }

        public CassetteTransferModel(string magno)
        {
            string[] elms = magno.Split(' ');

            #region バーコードヘッダー部判定

            if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
            {
                magno = elms[1];
            }
            else if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
            {
                magno = elms[1];
            }
            else if (elms.Length == 4 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
            {
                magno = elms[1];
            }
            else
            {
                throw new ApplicationException("マガジンバーコードを読み込んでください");
            }
            #endregion

            this.listCassetteCng = new List<CassetteCng>();
            this.LotNo = magno;

            //ロットから品種取得
            string typecd = Cassette.GetTypeCD(this.LotNo);

            //品種から作業一覧取得
            listProcinfo = Cassette.GetListProcess(typecd);
        }

        public void UpdateDB()
        {
            foreach (var CassetteCng in this.listCassetteCng)
            {
                //転写前の事前確認 → 枝番有無含めて現ロットでリング情報が有効になっているか？
                var exists = Cassette.GetCassetteList(this.LotNo, CassetteCng.cassetteno, 1);
                if (exists.Any() == false)
                {
                    throw new ApplicationException("読み込まれたロットにリングの稼働フラグが立っていません。[LOTNO]" + this.LotNo + "[RINGNO]" + CassetteCng.cassetteno);
                }

                //転写先の事前確認　⇒枝番含めてリング情報が別ロットで有効になっているか？
                var attached = Cassette.GetCassetteList(this.LotNo, CassetteCng.nextcassetteno, 1);
                if (attached.Any() == true)
                {
                    throw new ApplicationException("既に別のロットで割りついています。[LOTNO]" + attached[0].LotNo + "[RINGNO]" + attached[0].CassetteNo);
                }

                foreach(Cassette exist in exists)
                {
                    string[] tempCassette = exist.CassetteNo.Split('-');
                    string blanchno = "";
                    if(tempCassette.Length > 1)
                    {
                        blanchno = "-" + tempCassette[1].Trim();
                    }

                    //新規レコード作成
                    ArmsApi.Model.Cassette cs = new Cassette();
                    cs.LotNo = this.LotNo;
                    cs.CassetteNo = CassetteCng.nextcassetteno + blanchno;
                    cs.ProcNo = this.selectedprocno;
                    cs.SeqNo = exist.SeqNo;
                    cs.Attachdt = DateTime.Now;
                    cs.NextCassetteNo = null;
                    cs.Lastupddt = DateTime.Now;
                    cs.Detachdt = null;
                    cs.Newfg = 1;
                    cs.CreateNew();

                    //登録済みのレコードを更新
                    exist.Newfg = 0;
                    exist.NextCassetteNo = CassetteCng.nextcassetteno + blanchno;
                    exist.Detachdt = DateTime.Now;
                    exist.Lastupddt = DateTime.Now;
                    exist.Update();
                }
            }
        }
    }
}