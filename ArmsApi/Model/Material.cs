using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    /// <summary>
    /// 設計変更
    /// 装置割り付け日時等はMacMatHistoryへ移動
    /// 純粋に原材料ロットの情報のみに変更
    /// </summary>
    public class Material
    {
        public Material()
        {
            this.WorkCd = new List<string>();
        }

        #region Equals実装

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Material mat = (Material)obj;
            if (this.MaterialCd == mat.MaterialCd && this.LotNo == mat.LotNo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        public string MaterialCd { get; set; }

        public string LotNo { get; set; }

        public string MaterialNm { get; private set; }

        public string BinCd { get; set; }

        public string HMGroup { get; set; }

        public bool IsFrame { get; private set; }

        public bool IsWafer { get; private set; }

        public bool IsBadMarkFrame { get; set; }

        /// <summary>
        /// ロット単位で検査抜き取り済みフラグ
        /// </summary>
        public bool IsLotSampled { get; set; }

        /// <summary>
        /// 投入時・装置単位で抜き取り検査済みフラグ
        /// </summary>
        public bool IsTimeSampled { get; set; }

        public string RingId { get; set; }

        public string LongName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RingId))
                {
                    return MaterialNm + ":" + LotNo;
                }
                else
                {
                    return MaterialNm + ":" + LotNo + ":" + RingId;
                }
            }
        }

        /// <summary>
        /// 装置投入日
        /// </summary>
        public DateTime? InputDt { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        public DateTime LimitDt { get; set; }

        /// <summary>
        /// ウェハーのみブレンドコード
        /// </summary>
        public string BlendCd { get; set; }

        /// <summary>
        /// Update時のみ参照Nullの場合はNow
        /// </summary>
        public DateTime? LastUpdDt { get; set; }

        /// <summary>
        /// 投入ストッカーNo
        /// </summary>
        public int StockerNo { get; set; }

        /// <summary>
        /// 装置取り外し日時
        /// </summary>
        public DateTime? RemoveDt { get; set; }

        /// <summary>
        /// 装置取り外し済みフラグ
        /// </summary>
        public bool isRemoved
        {
            get
            {
                return RemoveDt.HasValue == true ? true : false;
            }
        }
        /// <summary>
        /// ウェハーのみ作業コード
        /// </summary>
        public List<string> WorkCd { get; set; }

        public string FrameMoldedNm { get; set; }

        public string FrameMoldedClass { get; set; }

        public List<DateTime> FrameMoldedDt { get; set; }

        public string TypeCd { get; set; }

        public string Supplyid { get; set; }

        public string DiceWaferKb { get; set; }

        //厚み平均値
        public float ThicknessAve { get; set; }

        /// <summary>
        /// 在庫数更新日
        /// </summary>
        public DateTime? StockLastUpdDt { get; set; }

        public Decimal StockCt { get; set; }

        public string UpdUserCd { get; set; }

        public Decimal? InputCt { get; set; }

        // 富士情報追加 開始 2022/9
        /// <summary>
        /// ソーティングランク　素子のみ設定
        /// </summary>
        public string SortingRnk
        {
            get
            {
                if (BlendCd.Length > 2)
                    return BlendCd.Substring(0, 2);
                else
                    return "";
            }
        }

        /// <summary>
        /// 色コード　　素子のみ設定
        /// </summary>
        public string IrCd
        {
            get
            {
                if (BlendCd.Length > 2)
                    return BlendCd.Substring(2);
                else
                    return "";
            }
        }
        // 富士情報追加 終了

        /// <summary>
        /// 原材料情報を取得
        /// </summary>
        /// <param name="matcd"></param>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static Material GetMaterial(string matcd, string lotno)
        {
            Material[] matlist = GetMaterials(matcd, null, lotno, false, false);

            if (matlist.Length >= 1)
            {
                return matlist[0];
            }
            else
            {
                throw new ArmsException(
                    string.Format("原材料ロットが存在しません。 品目CD:{0} ロットNO:{1}", matcd, lotno));
            }
        }

        public static Material GetMaterial(string ringId)
        {
            using (var db = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                // 更新日時をキーに降順に並び替え
                var mats = db.TnMaterials.Where(r => r.ringid == ringId && r.delfg == 0).OrderByDescending(r => r.lastupddt);
                if (mats.Count() == 0)
                {
                    throw new ApplicationException($"リングID「{ringId}」に紐付くウェハーは存在しません。NASCAのNPPG228リングID管理で確認して下さい。");
                }

                //Material mat = GetMaterial(mats.Single().materialcd, mats.Single().lotno);
                // 候補が複数ある場合は、更新日時が新しい原材料を使用
                Material mat = GetMaterial(mats.First().materialcd, mats.First().lotno);
                if (mat == null)
                {
                    throw new ApplicationException($"ロット「{mats.Single().lotno}」はARMS資材情報に存在しません。ArmsNascaBridgeの取り込みが正常に行われているか確認して下さい。");
                }

                return mat;
            }
        }

        public class MatLabel
        {
            public string MaterialCd { get; set; }

            public string LotNo { get; set; }

            /// <summary>
            /// バーコードの不正
            /// </summary>
            public bool IsBarcodeError { get; set; }
        }

        /// <summary>
        /// 富士情報　部材バーコード4M対応
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MatLabel GetMatLabelFromBarcode(string code)
        {
            MatLabel ML = new MatLabel();
            ML.IsBarcodeError = false;

            string[] inputval = code.Split(',');
            if (inputval.Length >= 10)
            //4M部材受け入れ時に発行するラベルは10項目以上
            {
                ML.MaterialCd = inputval[0];
                ML.LotNo = inputval[3] + "-" + inputval[4] + "-" + inputval[6];
            }
            else if (inputval.Length == 1)
            {
                string[] inputval2 = inputval[0].Split(' ');
                if (inputval2.Length == 1)
                //メーカーラベルの場合はロット番号(枝番なし)のみ
                {
                    ML.MaterialCd = null;
                    ML.LotNo = inputval2[0] + "-1-1";
                }
                else
                //ソーティングラベルの場合は空白で区切ってロット番号(枝番なし)+ランク+個数+品種+色コード+作成日
                {
                    ML.MaterialCd = null;
                    ML.LotNo = inputval2[0] + "-1-1";
                }
            }
            else
            {
                ML.IsBarcodeError = true;
            }

            return ML;
        }


        /// <summary>
        /// ロット検索実体
        /// 名称のみLIKE検索
        /// </summary>
        /// <param name="matcd"></param>
        /// <param name="matname"></param>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static Material[] GetMaterials(string matcd, string matname, string lotno, bool isLotBackwardMatch, bool isStock)
        {
            List<Material> retv = new List<Material>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT materialcd, lotno, materialnm, limitdt, bincd, hmgroup, blendcd, isframe, iswafer, issampled, workcd, isbadmarkframe, framemoldednm, framemoldedclass, framemoldeddt, supplyid, dicewaferkb, thicknessave, stocklastupddt, stockct, ringid
                        FROM TnMaterials
                        WHERE
                          delfg=0";

                    if (matcd != null)
                    {
                        cmd.CommandText += " AND materialcd = @MATCD";
                        cmd.Parameters.Add("@MATCD", System.Data.SqlDbType.NVarChar).Value = matcd;
                    }

                    if (matname != null)
                    {
                        cmd.CommandText += " AND materialnm LIKE @MATNM";
                        cmd.Parameters.Add("@MATNM", System.Data.SqlDbType.NVarChar).Value = "%" + matname + "%";
                    }

                    if (string.IsNullOrEmpty(lotno) == false)
                    {
                        if (isLotBackwardMatch == true)
                        {
                            cmd.CommandText += " AND lotno LIKE @LOTNO";
                            cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotno + "%";
                        }
                        else
                        {
                            cmd.CommandText += " AND lotno = @LOTNO";
                            cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotno;
                        }
                    }

                    if (isStock)
                    {
                        cmd.CommandText += " AND (stockct is not null) AND (stockct <> 0) ";
                    }
                    
                    cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordWorkCd = rd.GetOrdinal("workcd");

                        while (rd.Read())
                        {
                            Material m = new Material();
                            m.MaterialCd = SQLite.ParseString(rd["materialcd"]);

                            m.TypeCd = m.MaterialCd.Split('.').First();

                            m.LotNo = SQLite.ParseString(rd["lotno"]);
                            m.MaterialNm = SQLite.ParseString(rd["materialnm"]);
                            m.BinCd = SQLite.ParseString(rd["bincd"]);
                            m.LimitDt = SQLite.ParseDate(rd["limitdt"]) ?? DateTime.MaxValue.AddYears(-2);
                            m.BlendCd = SQLite.ParseString(rd["blendcd"]);
                            m.HMGroup = SQLite.ParseString(rd["hmgroup"]);
                            m.IsLotSampled = SQLite.ParseBool(rd["issampled"]);
                            m.IsWafer = SQLite.ParseBool(rd["iswafer"]);
                            m.IsFrame = SQLite.ParseBool(rd["isframe"]);
                            m.IsBadMarkFrame = SQLite.ParseBool(rd["isbadmarkframe"]);
                            if (!rd.IsDBNull(ordWorkCd) && string.IsNullOrEmpty(rd.GetString(ordWorkCd)) == false)
                            {
                                // 2015.5.13 [変更]ブレンド詳細マスタ複数作業CD取り込み
                                //m.WorkCd = SQLite.ParseString(rd.GetString(ordWorkCd));
                                m.WorkCd.AddRange(rd.GetString(ordWorkCd).Split(','));
                            }

                            // 2015.4.18 リードフレーム成型日管理機能追加
                            m.FrameMoldedNm = SQLite.ParseString(rd["framemoldednm"]);
                            m.FrameMoldedClass = SQLite.ParseString(rd["framemoldedclass"]);
                            m.FrameMoldedDt = new List<DateTime>();
                            if (!rd.IsDBNull(rd.GetOrdinal("framemoldeddt")))
                            {
                                string[] moldDtList = rd["framemoldeddt"].ToString().Trim().Split(',');
                                foreach (string molddt in moldDtList)
                                {
                                    m.FrameMoldedDt.Add(Convert.ToDateTime(molddt));
                                }
                            }
                            m.Supplyid = SQLite.ParseString(rd["supplyid"]);
                            m.DiceWaferKb = SQLite.ParseString(rd["dicewaferkb"]);

                            if (!rd.IsDBNull(rd.GetOrdinal("thicknessave")))
                            {
                                m.ThicknessAve = SQLite.ParseSingle(rd["thicknessave"]);
                            }
                            if (!rd.IsDBNull(rd.GetOrdinal("stocklastupddt")))
                            {
                                m.StockLastUpdDt = Convert.ToDateTime(rd["stocklastupddt"]);
                            }
                            m.StockCt = Convert.ToDecimal(rd["StockCt"]);

                            m.RingId = SQLite.ParseString(rd["ringid"]);

                            retv.Add(m);
                        }
                    }
                }
                return retv.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArmsException("原材料情報取得エラー", ex);
            }
        }

        public static Material[] GetMaterials(string lotNo, bool isStock)
        {
            return GetMaterials(null, null, lotNo, false, isStock);
        }

        /// <summary>
        /// DB樹脂有効期限情報
        /// </summary>
        public class ResinLife
        {
            public string TypeCd { get; set; }
            public string ResinMaterialCd { get; set; }
            public int LifeFromInput { get; set; }
            public int LifeFromUnpackToWorkStart { get; set; }
            public int LifeFromUnpackToWorkEnd { get; set; }
            public int ForbiddenFromUnpacktoInput { get; set; }
        }

        /// <summary>
        /// 解凍(開封)→投入の有効期限切れチェック(投入時)
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="resinmatcd"></param>
        /// <param name="resinlotno"></param>
        /// <param name="inputdt"></param>
        /// <param name="checkNasca"></param>
        /// <returns></returns>
        public static bool IsExpiredDBResinWhenInput(string typecd, string resinmatcd, string resinlotno, DateTime inputdt, bool checkNasca)
        {
            return IsExpiredDBResinWhenInput(typecd, resinmatcd, resinlotno, inputdt, checkNasca, 0);
        }

        /// <summary>
        /// 解凍→投入の有効期限オーバーチェック（投入時）
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="resinmatcd"></param>
        /// <param name="resinlotno"></param>
        /// <param name="inputdt"></param>
        /// <param name="checkNasca"></param>
        /// <param name="addMinutes"></param>
        /// <returns></returns>
        public static bool IsExpiredDBResinWhenInput(string typecd, string resinmatcd, string resinlotno, DateTime inputdt, bool checkNasca, int addMinutes)
        {
            if (!checkNasca) return false;

            ResinLife life = getResinLife(typecd, resinmatcd);
            if (life == null || (life.LifeFromUnpackToWorkStart == 0 && life.LifeFromUnpackToWorkEnd == 0))
            {
                // 開封期限マスタに設定値無の場合は確認しない
                return false;
            }

            // 解凍(開封)日時を取得
            DateTime? unpackDt = GetDbResinUnpackDate(resinmatcd, resinlotno);
            if (unpackDt.HasValue == false)
            {
                throw new ApplicationException(
                    string.Format("ロットNO：{0}の開封日時が存在しません。NASCAで開封登録が行われていることを確認して下さい。", resinlotno));
            }

            // 投入日時が期限を超えている場合はtrue
            DateTime unpackLife = unpackDt.Value.AddMinutes(life.LifeFromUnpackToWorkStart);
            if (inputdt >= unpackLife)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 解凍→投入の使用可能チェック（投入時）
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="resinmatcd"></param>
        /// <param name="resinlotno"></param>
        /// <param name="inputdt"></param>
        /// <param name="checkNasca"></param>
        /// <param name="addMinutes"></param>
        /// <returns></returns>
        public static bool IsFobiddenDBResinWhenInput(string typecd, string resinmatcd, string resinlotno, DateTime inputdt)
        {
            List<ResinLife> lifeList = getResinLifes(typecd, resinmatcd, true).ToList();
            List<int> forbiddenTimeList = lifeList
                                        .Where(l => l.ForbiddenFromUnpacktoInput > 0)
                                        .Select(l => l.ForbiddenFromUnpacktoInput)
                                        .Distinct().ToList();

            if (forbiddenTimeList.Count == 0)
            {
                // 開封期限マスタに設定値無の場合は確認しない
                return false;
            }
            
            // 同じ樹脂品目で複数種類の設定値がある場合は、最大値を使用する。
            int unUseTime = forbiddenTimeList.Max();

            // 解凍(開封)日時を取得
            DateTime? unpackDt = GetDbResinUnpackDate(resinmatcd, resinlotno);
            if (unpackDt.HasValue == false)
            {
                throw new ApplicationException(
                    string.Format("ロットNO：{0}の開封日時が存在しません。NASCAで開封登録が行われていることを確認して下さい。", resinlotno));
            }

            // 投入日時が使用禁止期間を経過していない場合はtrue
            DateTime canusedt = unpackDt.Value.AddMinutes(unUseTime);
            if (inputdt >= canusedt)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static ResinLife getResinLife(string typeCd, string resinMatCd)
        {
            ResinLife[] resinLifes = getResinLifes(typeCd, resinMatCd, false);
            if (resinLifes.Count() == 0)
            {
                return null;
            }
            else
            {
                return resinLifes.Single();
            }
        }
        public static ResinLife[] getResinLifes(string typeCd, bool includeDelete)
        {
            ResinLife[] resinLifes = getResinLifes(typeCd, null, includeDelete);
            return resinLifes;
        }
        /// <summary>
        /// DB樹脂の有効期限マスタ取得
        /// </summary>
        /// <param name="typeCd"></param>
        /// <param name="resinMatCd"></param>
        /// <param name="includeDelete"></param>
        /// <returns></returns>
        //20221005 MOD private -> public
        //private static ResinLife[] getResinLifes(string typeCd, string resinMatCd, bool includeDelete)
        public static ResinLife[] getResinLifes(string typeCd, string resinMatCd, bool includeDelete)
        {
            List<ResinLife> retv = new List<ResinLife>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    string sql = @" SELECT typecd, resinmaterialcd, lifefromunpacktoworkstart, lifefromunpacktoworkend, lifefrominput, forbiddenfromunpacktoinput
			                    FROM TmDBResinLife WHERE 1=1 ";

                    if (!includeDelete)
                    {
                        sql += " AND delfg = 0 ";
                    }

                    if (!string.IsNullOrEmpty(typeCd))
                    {
                        sql += " AND typecd = @TYPECD ";
                        cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;
                    }

                    if (!string.IsNullOrEmpty(resinMatCd))
                    {
                        sql += " AND resinmaterialcd=@matcd ";
                        cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = resinMatCd;
                    }

                    cmd.CommandText = sql;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            ResinLife rl = new ResinLife();
                            rl.TypeCd = SQLite.ParseString(rd["typecd"]);
                            rl.ResinMaterialCd = SQLite.ParseString(rd["resinmaterialcd"]);
                            rl.LifeFromInput = SQLite.ParseInt(rd["lifefrominput"]);
                            rl.LifeFromUnpackToWorkStart = SQLite.ParseInt(rd["lifefromunpacktoworkstart"]);
                            rl.LifeFromUnpackToWorkEnd = SQLite.ParseInt(rd["lifefromunpacktoworkend"]);
                            rl.ForbiddenFromUnpacktoInput = SQLite.ParseInt(rd["forbiddenfromunpacktoinput"]);
                            retv.Add(rl);
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("DB樹脂有効期限取得エラー", ex);
                }
            }
        }

        public class WaferWashedLife
        {
            public string TypeCd { get; set; }
            public string WaferTypeCd { get; set; }
            public int LifeFromWashEnd { get; set; }
        }

        public static WaferWashedLife GetWaferWashedLife(string typeCd, string matTypeCd)
        {
            WaferWashedLife retv = null;

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT lifefromwashend
								FROM TmDBWaferWashedLife WITH(nolock) 
								WHERE delfg = 0 AND (typecd = @TypeCd) AND (wafertypecd = @MatTypeCd) ";

                cmd.Parameters.Add("@TypeCd", SqlDbType.NVarChar).Value = typeCd;
                cmd.Parameters.Add("@MatTypeCd", SqlDbType.NVarChar).Value = matTypeCd;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv = new WaferWashedLife();
                        retv.TypeCd = typeCd;
                        retv.WaferTypeCd = matTypeCd;
                        retv.LifeFromWashEnd = Convert.ToInt32(rd["lifefromwashend"]);
                    }
                }
            }

            return retv;
        }

        #region 樹脂開封日時を取得する

        public static DateTime? GetDbResinUnpackDate(string matCd, string lotNo)
        {
            MatChar[] unPackDateList = MatChar.GetMatChar(matCd, lotNo, MatChar.LOTCHARCD_MATERIALOPEN);
            if (unPackDateList.Count() == 1)
            {
                DateTime dt;
                if (DateTime.TryParse(unPackDateList.Single().LotCharVal, out dt))
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// DB樹脂の装置投入後有効期限取得
        /// 投入後と解凍後のより短い方を取得
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="resinmatcd"></param>
        /// <param name="resinlotno"></param>
        /// <param name="inputdt"></param>
        /// <param name="checkNasca"></param>
        /// <returns></returns>
        public static DateTime GetDBResinLimitDt(string typecd, string resinmatcd, string resinlotno, DateTime inputdt, bool checkNasca)
        {
            ResinLife life = getResinLife(typecd, resinmatcd);

            //設定なしの場合は9999年
            if (life == null)
            {
                return DateTime.MaxValue.AddYears(-2);
            }

            //投入後有効期限
            DateTime lifeFromInput = DateTime.MaxValue.AddYears(-2);

            //開封（解凍）後有効期限
            DateTime lifeFromUnpack = DateTime.MaxValue.AddYears(-2);

            //設定が0分の場合は判定なし
            if (life.LifeFromInput > 1)
            {
                lifeFromInput = inputdt.AddMinutes(life.LifeFromInput);
            }

            if (checkNasca && life.LifeFromUnpackToWorkEnd > 1)
            {
                try
                {
                    DateTime? unpackdt = GetDbResinUnpackDate(resinmatcd, resinlotno);
                    if (unpackdt.HasValue)
                    {
                        lifeFromUnpack = unpackdt.Value.AddMinutes(life.LifeFromUnpackToWorkEnd);
                    }
                }
                catch (Exception ex)
                {
                    Log.SysLog.Error("NASCA 樹脂解凍時間取得失敗 最大値を設定して処理継続:" + resinlotno + ":" + resinmatcd, ex);
                }
            }

            if (lifeFromInput <= lifeFromUnpack)
            {
                return lifeFromInput;
            }
            else
            {
                return lifeFromUnpack;
            }
        }

        /// <summary>
        /// DBウェハー水洗浄後有効期限取得
        /// 2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetDBWaferWashedLimitDt(string typecd, string wafertypecd, DateTime inputdt)
        {
            WaferWashedLife life = GetWaferWashedLife(typecd, wafertypecd);
            if (life == null)
            {
                return null;
            }

            return inputdt.AddMinutes(life.LifeFromWashEnd);
        }

        public static DateTime GetDBWaferWashedLimitDt(WaferWashedLife life, DateTime inputdt)
        {
            return inputdt.AddMinutes(life.LifeFromWashEnd);
        }

        /// <summary>
        /// MAPのダイシングブレード取付位置照合
        /// </summary>
        /// <param name="stockerCd"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static bool CanInputMapDicingBlade(string stockerCd, Material mat)
        {
            GnlMaster[] master = GnlMaster.Search("MAPDICINGBLADE", stockerCd, mat.MaterialCd, null);
            if (master.Length >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Material[] GetNascaFrameMaterials(string lotNo)
        {
            List<Material> retv = new List<Material>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    string sql = @" SELECT RvtTRANMAT.lot_no, RvtTRANMAT.mtralitem_cd
							FROM dbo.RvtTRANMAT AS RvtTRANMAT WITH (nolock) 
							INNER JOIN dbo.RvtTRANH AS RvtTRANH WITH (nolock) ON RvtTRANMAT.mnfctrsl_no = RvtTRANH.mnfctrsl_no 
							INNER JOIN dbo.RvtORDH AS RvtORDH WITH (nolock) ON RvtTRANH.mnfctinst_no = RvtORDH.mnfctinst_no 
							INNER JOIN dbo.NttSSHJ AS NttSSHJ ON RvtORDH.mnfctinst_no = NttSSHJ.MnfctInst_NO 
							INNER JOIN dbo.NtmHMGK AS NtmHMGK ON RvtTRANMAT.mtralitem_cd = NtmHMGK.Material_CD
							WHERE (RvtTRANMAT.del_fg = 0) AND (RvtTRANH.del_fg = 0) AND (RvtORDH.del_fg = 0) AND (NttSSHJ.Del_FG = 0) AND (NtmHMGK.Del_FG = 0) 
							AND nttsshj.lot_no = @LotNo
							AND (NtmHMGK.MateGroup_CD = 'MATE001') OPTION (maxdop 1) ";

                    cmd.Parameters.Add("@LotNo", SqlDbType.VarChar).Value = lotNo;

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Material matlot = new Material();

                            matlot.MaterialCd = rd["mtralitem_cd"].ToString().Trim();
                            matlot.LotNo = rd["lot_no"].ToString().Trim();

                            retv.Add(matlot);
                        }
                    }
                }
            }

            return retv.ToArray();
        }

        /// <summary>
        /// 社内チップか
        /// </summary>
        /// <param name="diceWaferKb"></param>
        /// <returns></returns>
        public static bool IsInHouseDice(string diceWaferKb)
        {
            if (diceWaferKb == "2")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 取り込み済みの資材か確認
        /// </summary>
        /// <param name="materialCd"></param>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        public static bool IsImported(string materialCd, string lotNo)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT lotno
                    FROM TnMaterials WITH(nolock)
                    WHERE delfg = 0 AND (lotno = @LotNo) ";

                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;

                if (string.IsNullOrEmpty(materialCd) == false)
                {
                    cmd.CommandText += " AND (materialcd = @MaterialCd)";
                    cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialCd;
                }

                object exists = cmd.ExecuteScalar();
                if (exists == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 取り込み済みの資材か確認
        /// </summary>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        public static bool IsImported(string lotNo)
        {
            return IsImported(string.Empty, lotNo);
        }

        /// <summary>
        /// 樹脂グループ判定品かどうかのチェック  2016.11.5 湯浅
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static bool IsCompareResinMatGr(string matHmgp)
        {
            if (Config.Settings.CompareResinMaterialGroup != null)
            {
                foreach (string hmgpCd in Config.Settings.CompareResinMaterialGroup)
                {
                    if (matHmgp == hmgpCd) return true;
                }
            }
            return false;
        }

        public static bool IsCompareGoldWireMatGr(string matHmgp)
        {
            if (Config.Settings.CompareGoldWireMaterialGroup == null)
                throw new ApplicationException("資材が金線か判断する品目ｸﾞﾙｰﾌﾟCDがArmsConfigに設定されていない状態で関数の呼び出しがされています。");

            if (Config.Settings.CompareGoldWireMaterialGroup == matHmgp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ロット番号から各資材の厚みを取得する。
        /// 対象となる資材が一つも取れなかったら例外を返す。
        /// </summary>
        /// <param name="ringNo"></param>
        /// <param name="substrateDM"></param>
        /// <returns></returns>
        public static List<float> GetMatThicknessList(string lotno)
        {
            //if (String.IsNullOrWhiteSpace(ringNo) == false && String.IsNullOrWhiteSpace(substrateDM) == false)
            //{
            //    throw new ApplicationException(string.Format("リングIDと基板DMの両方が指定されています。"));
            //}

            List<float> thicknessList = new List<float>();
            string thicknessMatkb;

            //string lotno = string.Empty;

            //if(String.IsNullOrWhiteSpace(ringNo) == false)
            //{
            //   lotno = LotCarrier.GetLotNoFromRingNo(ringNo);
            //}
            //else if (String.IsNullOrWhiteSpace(substrateDM) == false)
            //{
            //    lotno = LotCarrier.GetLotNo(substrateDM, true)[0];
            //}
            //else
            //{
            //    throw new ApplicationException(string.Format("リングID及び基板DMがどちらも指定されていません。"));
            //}

            List<LotChar> thicknessMatkbList = LotChar.GetLotChar(lotno, NASCA.Importer.GR_THICKNESS_DECISION_MATKIND_LOTCHARCD).ToList();
            if(thicknessMatkbList == null || thicknessMatkbList.Count != 1)
            {
                throw new ApplicationException(string.Format("ロットから研削資材区分が取得できないか、複数存在します。ロット：{0}", lotno));
            }
            else
            {
                thicknessMatkb = thicknessMatkbList.First().LotCharVal.ToString().Trim();
            }


            if (thicknessMatkb.Trim() == "0")
            {
                List<string> carrierList = LotCarrier.GetCarrierNo(lotno, true).ToList();
                foreach (string carrier in carrierList)
                {
                    string thicknessRank = SubstrateThicknessRank.GetThicknessRank(carrier).Trim();
                    thicknessList.Add(float.Parse(thicknessRank)); //他資材（チップ）の厚みがreal型なので合わせる
                }

                if (thicknessList.Count() == 0)
                {
                    throw new ApplicationException(string.Format("基板がロットに割り付いていません。ロット：{0}", lotno));
                }
            }
            else if (thicknessMatkb.Trim() == "1")
            {
                List<Order> orders = Order.GetOrder(lotno).Where(c => c.IsComplete == true).ToList();

                foreach (Order order in orders)
                {
                    List<Material> matList = new List<Material>();
                    matList = order.GetMaterials().ToList();

                    foreach (Material mat in matList)
                    {
                        Material matDetail = Material.GetMaterial(mat.MaterialCd, mat.LotNo);
                        if (matDetail.IsWafer)
                        {
                            if (matDetail.ThicknessAve == 0)
                            {
                                throw new ApplicationException(string.Format("ウェハに厚み情報が紐づいていません。ロット：{0}", mat.LotNo));
                            }
                            else
                            {
                                thicknessList.Add(matDetail.ThicknessAve);
                            }
                        }
                    }
                }
                if (thicknessList.Count() == 0)
                {
                    throw new ApplicationException(string.Format("ウェハがロットに割り付いていません。ロット：{0}", lotno));
                }

            }
            else
            {
                throw new ApplicationException(string.Format("ロット特性「研削厚み判定資材種類」の値が誤っています。ロット：{0}", lotno));
            }
            return thicknessList;

        }

        /// <summary>
        /// 在庫数のみ更新する
        /// </summary>
        public static void UpdateStockCount(string materialCd, string lotNo, decimal stockCt, string updUserCd)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                        UPDATE TnMaterials SET stockct = @StockCt, stocklastupddt = @Stocklastupddt, updusercd = @UpdUserCd 
                        WHERE (materialcd = @MaterialCd) AND (lotno = @LotNo) ";

                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialCd;
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
                cmd.Parameters.Add("@StockCt", SqlDbType.Decimal).Value = stockCt;
                cmd.Parameters.Add("@Stocklastupddt", SqlDbType.DateTime).Value = System.DateTime.Now;
                cmd.Parameters.Add("@UpdUserCd", SqlDbType.Char).Value = updUserCd ?? (object)DBNull.Value;
                
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 富士情報追加　素子チェック対応　
        /// 在庫数のみ更新した情報を4MIF用テーブルに登録する　
        /// </summary>
        public static void InsertStockCount(string materialCd, string lotNo, string TOKUCD, decimal stockCt)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.IF4MConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                       Insert Into TnMatStock4m(materialcd, lotno,TOKUCD , stockct, stocklastupddt, ifflg)
                        VALUES (@MaterialCd, @LotNo, @TOKUCD, @StockCt, @Stocklastupddt, 0)";

                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialCd;
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
                cmd.Parameters.Add("@StockCt", SqlDbType.Decimal).Value = stockCt;
                cmd.Parameters.Add("@Stocklastupddt", SqlDbType.DateTime).Value = System.DateTime.Now;
                cmd.Parameters.Add("@TOKUCD", SqlDbType.VarChar).Value = TOKUCD;

                cmd.ExecuteNonQuery();
            }
        }
        public class BinLot
        {
            public decimal StockCt { get; set; }

            public DateTime StockLastUpdDt { get; set; }
        }

        public static BinLot GetStockCount(string materialCd, string lotNo)
        {
            if (Config.Settings.StockImportBinCodeList == null)
                return null;

            BinLot retv = null;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = $@" SELECT stock_ct, lastupd_dt 
                                FROM RvtBINLOT WITH(nolock) 
                                WHERE del_fg = 0 AND zero_fg = 0 
                                AND bin_cd in ('{ string.Join("','", Config.Settings.StockImportBinCodeList) }') 
                                AND material_cd = @MaterialCd AND lot_no = @LotNo ";

                cmd.Parameters.Add("@MaterialCd", SqlDbType.Char).Value = materialCd;
                cmd.Parameters.Add("@LotNo", SqlDbType.VarChar).Value = lotNo;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv = new BinLot();
                        retv.StockCt = Convert.ToDecimal(rd["stock_ct"]);
                        retv.StockLastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
                    }
                }
            }

            return retv;
        }

        public static int GetMaterialStructureCount(string materialCd, string typeCd, string structureVer)
        {
            int retv = 0;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT typecd, materialcd, structurevr, pmtralstrpc, delfg, lastupddt
                            FROM TmBomD WITH(nolock)
                            WHERE(delfg = 0) and typecd = '1' and materialcd = '1' and structurevr = '1' ";

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while(rd.Read())
                    {
                        retv = Convert.ToInt32(rd["pmtralstrpc"]);
                    }
                }
            }

            return retv;
        }
    }
}
