using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;


namespace ArmsWeb.Models
{
    public class CassetteTransfer2to1Model
    {
        public string LotNo { get; set; }
        public class CassetteCng 
        {
            public string cassetteno { get; set; }
            public string branchno { get; set; }
            public string nextcassetteno { get; set; }
        }

        public const string BRANCH_NO_RIGHT = "-1";
        public const string BRANCH_NO_LEFT = "-2";
        public const string POSITION_CD_RIGHT = "MIGI";
        public const string POSITION_CD_LEFT = "HIDARI";

        public CassetteCng cassetteCng { get; set; }
        public List<CassetteCng> listCassetteCng { get; set; }

        public string precassetteno { get; set; }
        public string prenextcassetteno { get; set; }
        public string attachposition { get; set; }
        /*
        public class procinfo
        {
            public int procno { get; set; }
            public string procnm { get; set; }
        }*/
        public int selectedprocno { get; set; }
        Cassette.procinfo procinfo { get; set; }
        public List<Cassette.procinfo> listProcinfo { get; set; }

        public CassetteTransfer2to1Model(string magno)
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
           
                //転写前の事前確認 → 読み込まれたロットのNewfgが1であるか?
                var exists = Cassette.GetCassette(this.LotNo, CassetteCng.cassetteno, 1);
                if (exists.Newfg == 0)
                {
                    throw new ApplicationException("読み込まれたロットにリングの稼働フラグが立っていません。[LOTNO]" + this.LotNo + " [RINGNO]" + CassetteCng.cassetteno);
                }

                //リングが別のロット(newfg1)で使われていないか確認。
                var noblanch = Cassette.GetCassette(this.LotNo, CassetteCng.nextcassetteno, 1);
                if (string.IsNullOrWhiteSpace(noblanch.LotNo) == false) 
                {
                    throw new ApplicationException("既に別のロットで割りついています。[LOTNO]" + noblanch.LotNo + " [RINGNO]" + CassetteCng.nextcassetteno);
                }

                //枝番付は自分の分だけチェック
                var blanch = Cassette.GetCassette(this.LotNo, CassetteCng.nextcassetteno + CassetteCng.branchno, 1);
                if (string.IsNullOrWhiteSpace(blanch.LotNo) == false)
                {
                    throw new ApplicationException("既に別のロットで割りついています。[LOTNO]" + blanch.LotNo + " [RINGNO]" + CassetteCng.nextcassetteno + CassetteCng.branchno);
                }

                //新規レコード作成
                ArmsApi.Model.Cassette cs = new Cassette();
                cs.LotNo = this.LotNo;
                cs.CassetteNo = CassetteCng.nextcassetteno + CassetteCng.branchno;
                cs.ProcNo = this.selectedprocno;
                cs.SeqNo = exists.SeqNo;
                cs.Attachdt = DateTime.Now;
                cs.NextCassetteNo = null;
                cs.Lastupddt = DateTime.Now;
                cs.Detachdt = null;
                cs.Newfg = 1;
                cs.CreateNew();

                //登録済みのレコードを更新
                exists.Newfg = 0;
                exists.NextCassetteNo = CassetteCng.nextcassetteno + CassetteCng.branchno;
                exists.Detachdt = DateTime.Now;
                exists.Lastupddt = DateTime.Now;
                exists.Update();
            }
        }
    }
}