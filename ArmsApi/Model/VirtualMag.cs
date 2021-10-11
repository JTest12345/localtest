using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    //purgeReasonは廃止
    //各WorkComplete内でDischargeConveyorを行き先に設定する等の処理に変更

    /// <summary>
    /// 仮想マガジンレコード
    /// </summary>
    public class VirtualMag
    {
        /// <summary>
        /// 不明マガジンの規定値
        /// </summary>
        public const string UNKNOWN_MAGNO = "000000";

        /// <summary>
        /// MAP基板アオイマガジンの規定値
        /// </summary>
        public const string MAP_AOI_MAGAZINE = "999999";

        public const char MAP_FRAME_SEPERATOR = ',';
        public const int MAP_FRAME_QR_LOTNO_IDX = 2;
        public const int MAP_FRAME_QR_MATCD_IDX = 5;
        public const int MAP_FRAME_QR_CT_IDX = 4;
        public const int MAP_FRAME_QR_MAGINDEX_IDX = 8;
        public const int MAP_FRAME_ELEMENT_CT = 9;

        /// <summary>
        /// 遠心沈降用の改良マガジン用マガジン番号ヘッダー
        /// </summary>
        public const string ECK_REVICED_MAG_HEADER = "ME";

        /// <summary>
        /// 画面のチェック
        /// </summary>
        public bool Check { get; set; }

        /// <summary>
        /// 装置NO
        /// </summary>
        public int MacNo { get; set; }

        /// <summary>
        /// 装置
        /// </summary>
        public string ClassNm { get; set; }

        /// <summary>
        /// 号機
        /// </summary>
        public string PlantNm { get; set; }

        /// <summary>
        /// 位置CD
        /// </summary>
        public int LocationId { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string LocationNm { get; set; }

        /// <summary>
        /// Magazineの現在位置
        /// </summary>
        public Location CurrentLocation { get; set; }

        public string MagazineNo { get; set; }

        /// <summary>
        /// 元マガジン
        /// </summary>
        public string LastMagazineNo { get; set; }

        /// <summary>
        /// 投入順番
        /// </summary>
        public int orderid { get; set; }

        /// <summary>
        /// 作業開始時間
        /// </summary>
        public DateTime? WorkStart { get; set; }

        /// <summary>
        /// 作業完了時間
        /// </summary>
        public DateTime? WorkComplete { get; set; }

        /// <summary>
        /// ストッカー開始段数
        /// </summary>
        public int? StockerStartCol { get; set; }

        /// <summary>
        /// ストッカー終了段数
        /// </summary>
        public int? StockerEndCol { get; set; }

        /// <summary>
        /// ストッカー交換回数
        /// </summary>
        public int? StockerChangeCt { get; set; }

        /// <summary>
        /// 最終のストッカー交換時間
        /// </summary>
        public DateTime? LastStockerChange { get; set; }

        /// <summary>
        /// 現在作業中の工程No
        /// 完了後登録後は次工程にシフト
        /// </summary>
        public int? ProcNo { get; set; }

        /// <summary>
        /// 次投入可能装置一覧
        /// </summary>
        public List<int> NextMachines { get; set; }
        public string NextMachinesString { get; set; }

        /// <summary>
        /// 関連原材料一覧
        /// </summary>
        public List<Material> RelatedMaterials { get; set; }
        public string RelatedMaterialsString { get; set; }

        /// <summary>
        /// MAP搭載ダイボンダー用
        /// 最大基板格納数
        /// </summary>
        public int? MaxFrameCt { get; set; }

        /// <summary>
        /// MAP搭載ダイボンダー用
        /// 現在基板格納数
        /// </summary>
        public int? CurrentFrameCt { get; set; }

        /// <summary>
        /// MAP基板原材料コード
        /// </summary>
        public string FrameMatCd { get; set; }

        /// <summary>
        /// MAP基板ロット番号
        /// </summary>
        public string FrameLotNo { get; set; }

        /// <summary>
        /// アオイの基板マガジンのロット番号（連番部除く）
        /// </summary>
        public string MapAoiMagazineLotNo { get; set; }

        ///// <summary>
        ///// アオイの基板マガジンについてくる番号
        ///// </summary>
        //public string MapAoiMagazineIndex { get; set; }

        ///// <summary>
        ///// アオイマガジンのロット番号
        ///// </summary>
        //public string AoiMagLotNO { get; set; }

        ///// <summary>
        ///// MAP基板アオイマガジンのインデックス
        ///// </summary>
        //public string AoiMagIndex { get; set; }

        /// <summary>
        /// 開始時ウェハー段数
        /// </summary>
        public int? StartWafer { get; set; }

        /// <summary>
        /// 終了時ウェハー段数
        /// </summary>
        public int? EndWafer { get; set; }

        /// <summary>
        /// ウェハーチェンジャーの交換回数
        /// </summary>
        public int? WaferChangerChangeCount { get; set; }

        /// <summary>
        /// ウェハーチェンジャーの交換時間
        /// この時間から設定値（60sec）以内の交換信号は無視する
        /// センサーの揺らぎ対策
        /// </summary>
        public DateTime? WaferChangerChangeTime { get; set; }

        /// <summary>
        /// ストッカー１使用段数
        /// </summary>
        public int? Stocker1 { get; set; }

        /// <summary>
        /// ストッカー２使用段数
        /// </summary>
        public int? Stocker2 { get; set; }

        /// <summary>
        /// どのラインで製造されたか
        /// nullは自ライン製品。
        /// それ以外の数字なら他ラインからの搬送品
        /// </summary>
        public int? Origin { get; set; }

        /// <summary>
        /// 搬送元位置CD (Originとセットで使用
        /// </summary>
        public int? OriginLocationId { get; set; }

        /// <summary>
        /// 排出理由
        /// ここが空白、Null以外ならNASCA完了後に排出される
        /// </summary>
        public string PurgeReason { get; set; }

        /// <summary>
        /// プログラム運転時間（分)
        /// </summary>
        public int? ProgramTotalMinutes { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime LastUpdDt { get; set; }

        /// <summary>
        /// 優先搬送フラグ
        /// </summary>
        public bool PriorityFg { get; set; }

        /// <summary>
        /// LastMagazine,NextProcess以外の作業データを初期化
        /// </summary>
        public void InitializeWorkData()
        {
            this.WorkStart = null;
            this.WorkComplete = null;
            this.StockerStartCol = null;
            this.StockerEndCol = null;
            this.NextMachines = new List<int>();
            this.RelatedMaterials = new List<Material>();
            //this.AoiMagIndex = null;

            this.StartWafer = null;
            this.EndWafer = null;
            this.Stocker1 = null;
            this.Stocker2 = null;
            this.WaferChangerChangeCount = null;
            this.WaferChangerChangeTime = null;
            this.PurgeReason = null;
            this.Origin = null;
            this.OriginLocationId = null;
            //this.MapAoiMagazineIndex = null;
        }

        public VirtualMag() 
        {
            NextMachines = new List<int>();
            RelatedMaterials = new List<Material>();
        }

        /// <summary>
        /// 先頭を削除せずに返す
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public VirtualMag Peek(Location location) 
        {           
            try
            {
                List<VirtualMag> mags = getMagazines(location);
                if (mags.Count == 0) { return null; }

                int orderMinID =  mags.Min(m => m.orderid);

                VirtualMag mag = mags.Find(m => m.orderid == orderMinID);
                mag.CurrentLocation = location;

                return mag;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 終端に追加
        /// magのCurrentLocationを更新
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="location"></param>
        public void Enqueue(VirtualMag mag, Location location)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    int orderNextID = 1;
                    List<VirtualMag> mags = getMagazines(location);
                    if (mags.Count != 0)
                    {
                        orderNextID = mags.Max(m => m.orderid) + 1;
                    }

                    string sql = @" INSERT INTO TnVirtualMag(macno, locationid, orderid, magno, 
                                        lastmagno, procno, workstartdt, workenddt,
                                        nextmachines, relatedmaterials,
                                        framematerialcd, framelotno, framecurrentct, framemaxct,
                                        stockerstartno, stockerendno, stockerchangect, stockerlastupddt,
                                        stocker1no, stocker2no, 
                                        waferstartno, waferendno, waferchangect, waferlastupddt,
                                        mapaoimaglotno, purgereason, originno, programtotalminutes, originlocationid) 
                                    VALUES(@MACNO, @LOCATIONID,
                                        @ORDERID, @MAGNO,
                                        @LASTMAGNO,
                                        @PROCNO, 
                                        @WORKSTARTDT,  @WORKENDT, 
                                        @NEXTMACHINES, @RELATEDMATERIALS, 
                                        @FRAMEMATERIALCD,
                                        @FRAMELOTNO,
                                        @FRAMECURRENTCT, 
                                        @FRAMEMAXCT, 
                                        @STOCKERSTARTNO, @STOCKERENDNO,
                                        @STOCKERCHANGECT, @STOCKERLASTUPDDT, 
                                        @STOCKER1NO, @STOCKER2NO, 
                                        @WAFERSTARTNO, @WAFERENDNO, 
                                        @WAFERCHANGECT, @WAFERLASTUPDDT, 
                                        @MAPAOIMAGLOTNO, 
                                        @PURGEREASON, @ORIGINNO, @PROGRAMTOTALMINUTES, @ORIGINLOCATIONID) ";

                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = location.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)location.Station;
                    cmd.Parameters.Add("@ORDERID", SqlDbType.Int).Value = orderNextID;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = mag.MagazineNo;
                    cmd.Parameters.Add("@LASTMAGNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(mag.LastMagazineNo);
                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.ProcNo);
                    cmd.Parameters.Add("@WORKSTARTDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(mag.WorkStart);
                    cmd.Parameters.Add("@WORKENDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(mag.WorkComplete);
                    cmd.Parameters.Add("@FRAMEMATERIALCD", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(mag.FrameMatCd);
                    cmd.Parameters.Add("@FRAMELOTNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(mag.FrameLotNo);
                    cmd.Parameters.Add("@FRAMECURRENTCT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(mag.CurrentFrameCt);
                    cmd.Parameters.Add("@FRAMEMAXCT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(mag.MaxFrameCt);
                    cmd.Parameters.Add("@STOCKERSTARTNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.StockerStartCol);
                    cmd.Parameters.Add("@STOCKERENDNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.StockerEndCol);
                    cmd.Parameters.Add("@STOCKERCHANGECT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(mag.StockerChangeCt);
                    cmd.Parameters.Add("@STOCKERLASTUPDDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(mag.LastStockerChange);
                    cmd.Parameters.Add("@STOCKER1NO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.Stocker1);
                    cmd.Parameters.Add("@STOCKER2NO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.Stocker2);
                    cmd.Parameters.Add("@WAFERSTARTNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.StartWafer);
                    cmd.Parameters.Add("@WAFERENDNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.EndWafer);
                    cmd.Parameters.Add("@WAFERCHANGECT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(mag.WaferChangerChangeCount);
                    cmd.Parameters.Add("@WAFERLASTUPDDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(mag.WaferChangerChangeTime);
                    cmd.Parameters.Add("@MAPAOIMAGLOTNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(mag.MapAoiMagazineLotNo);
                    cmd.Parameters.Add("@PURGEREASON", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(mag.PurgeReason);
                    cmd.Parameters.Add("@ORIGINNO", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.Origin);
                    cmd.Parameters.Add("@PROGRAMTOTALMINUTES", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.ProgramTotalMinutes);
                    cmd.Parameters.Add("@ORIGINLOCATIONID", SqlDbType.Int).Value = SQLite.GetParameterValue(mag.OriginLocationId);

                    cmd.Parameters.Add("@NEXTMACHINES", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(string.Join(",", mag.NextMachines));

                    List<string> relatedmaterials = new List<string>();
                    foreach (Material material in mag.RelatedMaterials)
                    {
                        string relatedmaterial = string.Format("{0}@{1}", material.MaterialCd, material.LotNo);
                        relatedmaterials.Add(relatedmaterial);
                    }
                    cmd.Parameters.Add("@RELATEDMATERIALS", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(string.Join(",", relatedmaterials));
                    
                    cmd.ExecuteNonQuery();

                    Log.RBLog.Info(string.Format(@"[Enqueue]{0}\{1}:{2}", location.MacNo, location.Station, mag.MagazineNo));
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 指定マガジンを更新
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="location"></param>
        public void Updatequeue()
        {
            try
            {
				if (this.CurrentLocation == null) 
				{
					this.CurrentLocation = new Location(this.MacNo, (Station)this.LocationId);
				}

                string sql = @" UPDATE TnVirtualMag SET
                                    lastmagno = @LASTMAGNO,
                                    procno = @PROCNO, 
                                    workstartdt = @WORKSTARTDT, workenddt = @WORKENDT, 
                                    nextmachines = @NEXTMACHINES, relatedmaterials = @RELATEDMATERIALS,
                                    framematerialcd = @FRAMEMATERIALCD,
                                    framelotno = @FRAMELOTNO,
                                    framecurrentct = @FRAMECURRENTCT, 
                                    framemaxct = @FRAMEMAXCT, 
                                    stockerstartno = @STOCKERSTARTNO, stockerendno = @STOCKERENDNO,
                                    stockerchangect = @STOCKERCHANGECT, stockerlastupddt = @STOCKERLASTUPDDT, 
                                    stocker1no = @STOCKER1NO, stocker2no = @STOCKER2NO, 
                                    waferstartno = @WAFERSTARTNO, waferendno = @WAFERENDNO, 
                                    waferchangect = @WAFERCHANGECT, waferlastupddt = @WAFERLASTUPDDT, 
                                    mapaoimaglotno = @MAPAOIMAGLOTNO, 
                                    purgereason = @PURGEREASON, originno = @ORIGINNO, programtotalminutes = @PROGRAMTOTALMINUTES,
                                    originlocationid = @ORIGINLOCATIONID
                                WHERE (macno = @MACNO) AND (magno = @MAGNO) ";
                //AND (locationid = @LOCATIONID)
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.CurrentLocation.MacNo;
                    //cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)this.CurrentLocation.Station;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = this.MagazineNo;

                    cmd.Parameters.Add("@LASTMAGNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.LastMagazineNo);
                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.ProcNo);
                    cmd.Parameters.Add("@WORKSTARTDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(this.WorkStart);
                    cmd.Parameters.Add("@WORKENDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(this.WorkComplete);
                    cmd.Parameters.Add("@FRAMEMATERIALCD", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.FrameMatCd);
                    cmd.Parameters.Add("@FRAMELOTNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.FrameLotNo);
                    cmd.Parameters.Add("@FRAMECURRENTCT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(this.CurrentFrameCt);
                    cmd.Parameters.Add("@FRAMEMAXCT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(this.MaxFrameCt);
                    cmd.Parameters.Add("@STOCKERSTARTNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.StockerStartCol);
                    cmd.Parameters.Add("@STOCKERENDNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.StockerEndCol);
                    cmd.Parameters.Add("@STOCKERCHANGECT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(this.StockerChangeCt);
                    cmd.Parameters.Add("@STOCKERLASTUPDDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(this.LastStockerChange);
                    cmd.Parameters.Add("@STOCKER1NO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.Stocker1);
                    cmd.Parameters.Add("@STOCKER2NO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.Stocker2);
                    cmd.Parameters.Add("@WAFERSTARTNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.StartWafer);
                    cmd.Parameters.Add("@WAFERENDNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.EndWafer);
                    cmd.Parameters.Add("@WAFERCHANGECT", SqlDbType.BigInt).Value = SQLite.GetParameterValue(this.WaferChangerChangeCount);
                    cmd.Parameters.Add("@WAFERLASTUPDDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(this.WaferChangerChangeTime);
                    cmd.Parameters.Add("@MAPAOIMAGLOTNO", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.MapAoiMagazineLotNo);
                    cmd.Parameters.Add("@PURGEREASON", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.PurgeReason);
                    cmd.Parameters.Add("@ORIGINNO", SqlDbType.Int).Value = SQLite.GetParameterValue(this.Origin);
                    cmd.Parameters.Add("@PROGRAMTOTALMINUTES", SqlDbType.Int).Value = SQLite.GetParameterValue(this.ProgramTotalMinutes);
                    cmd.Parameters.Add("@ORIGINLOCATIONID", SqlDbType.Int).Value = SQLite.GetParameterValue(this.OriginLocationId);

                    cmd.Parameters.Add("@NEXTMACHINES", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(string.Join(",", this.NextMachines));

                    List<string> relatedmaterials = new List<string>();
                    foreach (Material material in this.RelatedMaterials)
                    {
                        string relatedmaterial = string.Format("{0}@{1}", material.MaterialCd, material.LotNo);
                        relatedmaterials.Add(relatedmaterial);
                    }
                    cmd.Parameters.Add("@RELATEDMATERIALS", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(string.Join(",", relatedmaterials));

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 先頭を返して削除
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public VirtualMag Dequeue(Location location)
        {
            try
            {
                List<VirtualMag> mags = getMagazines(location);
                if (mags.Count == 0) { return null; }

                int orderMinID = mags.Min(m => m.orderid);

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" DELETE FROM TnVirtualMag 
                                        WHERE macno = @MACNO AND locationid = @LOCATIONID AND orderid = @ORDERID ";
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = location.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)location.Station;
                    cmd.Parameters.Add("@ORDERID", SqlDbType.Int).Value = orderMinID;

                    cmd.ExecuteNonQuery();

                    Log.RBLog.Info(string.Format(@"[Dequeue]{0}\{1}:{2}", location.MacNo, location.Station, mags.Find(m => m.orderid == orderMinID).MagazineNo));
                }

                //投入順番を再割り当て
                int updateOrderID = 1;
                foreach (VirtualMag mag in mags) 
                {
                    if (mag.orderid == orderMinID) { continue; }

                    updateOrder(location, mag.MagazineNo, mag.orderid, updateOrderID);
                    updateOrderID++;
                }

                return mags.Find(m => m.orderid == orderMinID);
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        public void Delete(string magno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" DELETE FROM TnVirtualMag WHERE magno = @MAGNO ";
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magno;
                    cmd.ExecuteNonQuery();

                    Log.RBLog.Info(string.Format(@"[Delete]{0}", magno));
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 指定ロケーションのマガジンを返す
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<VirtualMag> getMagazines(Location location) 
        {
            List<VirtualMag> mags = new List<VirtualMag>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT macno, locationid, orderid, magno, lastmagno, procno, 
                                        workstartdt, workenddt, nextmachines, relatedmaterials,
                                        framematerialcd, framelotno, framecurrentct, framemaxct, 
                                        stockerstartno, stockerendno, stockerchangect, stockerlastupddt, stocker1no, stocker2no, 
                                        waferstartno, waferendno, waferchangect, waferlastupddt, 
                                        mapaoimaglotno, purgereason, originno, programtotalminutes,
                                        lastupddt, priorityfg, originlocationid
                                    FROM TnVirtualMag 
                                        WHERE macno = @MACNO AND locationid = @LOCATIONID ";

                cmd.CommandText = sql.Replace("\r\n", "");
                cmd.Parameters.Add("@MACNO", SqlDbType.NVarChar).Value = location.MacNo;
                cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)location.Station;


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int ordMacno = reader.GetOrdinal("macno");
                    int ordProcno = reader.GetOrdinal("procno");
                    int ordNextMachines = reader.GetOrdinal("nextmachines");
                    int ordRelatedMaterials = reader.GetOrdinal("relatedmaterials");
                    int ordStockerstartno = reader.GetOrdinal("stockerstartno");
                    int ordStockerendno = reader.GetOrdinal("stockerendno");
                    int ordStocker1no = reader.GetOrdinal("stocker1no");
                    int ordStocker2no = reader.GetOrdinal("stocker2no");
                    int ordWaferstartno = reader.GetOrdinal("waferstartno");
                    int ordWaferendno = reader.GetOrdinal("waferendno");
                    int ordOrigin = reader.GetOrdinal("originno");
                    int ordOriginlocation = reader.GetOrdinal("originlocationid"); 

                    while (reader.Read())
                    {
                        VirtualMag mag = new VirtualMag();
                        mag.MacNo = SQLite.ParseInt(reader[ordMacno]);
                        mag.MagazineNo = SQLite.ParseString(reader["magno"]);
                        mag.LastMagazineNo = SQLite.ParseString(reader["lastmagno"]);
                        mag.orderid = SQLite.ParseInt(reader["orderid"]);

                        if (!reader.IsDBNull(ordProcno))
                        {
                            mag.ProcNo = SQLite.ParseInt(reader[ordProcno]);
                        }

                        mag.WorkStart = SQLite.ParseDate(reader["workstartdt"]);
                        mag.WorkComplete = SQLite.ParseDate(reader["workenddt"]);
                        mag.FrameMatCd = SQLite.ParseString(reader["framematerialcd"]);
                        mag.FrameLotNo = SQLite.ParseString(reader["framelotno"]);
                        mag.CurrentFrameCt = SQLite.ParseInt(reader["framecurrentct"]);
                        mag.MaxFrameCt = SQLite.ParseInt(reader["framemaxct"]);

                        if (!reader.IsDBNull(ordStockerstartno))
                        {
                            mag.StockerStartCol = SQLite.ParseInt(reader[ordStockerstartno]);
                        }
                        if (!reader.IsDBNull(ordStockerendno))
                        {
                            mag.StockerEndCol = SQLite.ParseInt(reader[ordStockerendno]);
                        }

                        mag.StockerChangeCt = SQLite.ParseInt(reader["stockerchangect"]);
                        mag.LastStockerChange = SQLite.ParseDate(reader["stockerlastupddt"]);

                        if (!reader.IsDBNull(ordStocker1no))
                        {
                            mag.Stocker1 = SQLite.ParseInt(reader[ordStocker1no]);
                        }
                        if (!reader.IsDBNull(ordStocker2no))
                        {
                            mag.Stocker2 = SQLite.ParseInt(reader[ordStocker2no]);
                        }
                        if (!reader.IsDBNull(ordWaferstartno))
                        {
                            mag.StartWafer = SQLite.ParseInt(reader[ordWaferstartno]);
                        }
                        if (!reader.IsDBNull(ordWaferendno))
                        {
                            mag.EndWafer = SQLite.ParseInt(reader[ordWaferendno]);
                        }

                        mag.WaferChangerChangeCount = SQLite.ParseInt(reader["waferchangect"]);
                        mag.WaferChangerChangeTime = SQLite.ParseDate(reader["waferlastupddt"]);
                        mag.MapAoiMagazineLotNo = SQLite.ParseString(reader["mapaoimaglotno"]);
                        mag.PurgeReason = SQLite.ParseString(reader["purgereason"]);

                        if (!reader.IsDBNull(ordOrigin))
                        {
                            mag.Origin = SQLite.ParseInt(reader[ordOrigin]);
                        }
                        mag.ProgramTotalMinutes = SQLite.ParseInt(reader["programtotalminutes"]);
                        mag.LastUpdDt = SQLite.ParseDate(reader["lastupddt"]).Value;
                        mag.PriorityFg = SQLite.ParseBool(reader["priorityfg"]);
                        if (!reader.IsDBNull(ordOriginlocation))
                        {
                            mag.OriginLocationId = SQLite.ParseInt(reader[ordOriginlocation]);
                        }
                        mag.NextMachines = new List<int>();
                        if (!reader.IsDBNull(ordNextMachines))
                        {
                            string[] nextMachines = SQLite.ParseString(reader[ordNextMachines]).Split(',');
                            foreach (string nextmachine in nextMachines)
                            {
                                mag.NextMachines.Add(int.Parse(nextmachine));
                            }
                        }

                        mag.RelatedMaterials = new List<Material>();
                        if (!reader.IsDBNull(ordRelatedMaterials))
                        {
                            string[] relatedMaterials = SQLite.ParseString(reader[ordRelatedMaterials]).Split(',');
                            foreach (string relatedMaterial in relatedMaterials)
                            {
                                string[] m = relatedMaterial.Split('@');
                                Material material = Material.GetMaterial(m[0], m[1]);
                                mag.RelatedMaterials.Add(material);
                            }
                        }

                        mags.Add(mag);
                    }
                }
            }
            return mags;
        }

        /// <summary>
        /// 投入順番を再割り当て
        /// </summary>
        /// <param name="mag"></param>
        private void updateOrder(Location location, string magazineNO, int oldOrderID, int updateOrderID) 
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" UPDATE TnVirtualMag SET orderid = @ORDERID
                                    WHERE macno = @MACNO AND locationid = @LOCATIONID AND orderid = @OLDORDERID AND magno = @MAGNO ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = location.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)location.Station;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magazineNO;
                    cmd.Parameters.Add("@OLDORDERID", SqlDbType.Int).Value = oldOrderID;
                    cmd.Parameters.Add("@ORDERID", SqlDbType.Int).Value = updateOrderID;

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// TODO 構造が悪いので修正する必要有り
        /// 投入順番を再割り当て
        /// </summary>
        public void updateOrder()
        {
            Log.RBLog.Info("投入順番を再割り当て macno:" + this.MacNo + " locationid:" + this.LocationId);

            VirtualMag[] virtualMagList = VirtualMag.GetVirtualMag(this.MacNo, this.LocationId);
            if (virtualMagList != null)
            {
                virtualMagList = virtualMagList.OrderBy(o => o.orderid).ToArray();
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = this.LocationId;
                    SqlParameter prmNewOrderID = cmd.Parameters.Add("@NEWORDERID", SqlDbType.Int);
                    SqlParameter prmOldOrderID = cmd.Parameters.Add("@OLDORDERID", SqlDbType.Int);
                    SqlParameter prmMagNO = cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar);

                    int newOrderID = 1;
                    foreach (VirtualMag v in virtualMagList)
                    {
                        if (v.orderid != newOrderID)
                        {
                            prmMagNO.Value = v.MagazineNo;
                            prmNewOrderID.Value = newOrderID;
                            prmOldOrderID.Value = v.orderid;

                            cmd.CommandText = @"
                            UPDATE
                                TnVirtualMag 
                            SET 
                                orderid=@NEWORDERID,
                                lastupddt=GETDATE()
                            WHERE 
                                macno=@MACNO AND 
                                locationid=@LOCATIONID AND
                                orderid=@OLDORDERID AND
                                magno=@MAGNO";
                            cmd.ExecuteNonQuery();
                        }

                        newOrderID++;
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("投入順番を再割り当てエラー", ex);
                }
            }
        }

        /// <summary>
        /// 遠心沈降専用MGか否かを判定する
        /// </summary>
        /// <param name="magno"></param>
        /// <returns></returns>
        public static bool IsECKMag(string magno)
        {
			if (string.IsNullOrEmpty(magno)) 
			{
				return false;
			}	

			if (magno.StartsWith(ECK_REVICED_MAG_HEADER) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// アオイ基板マガジンか否かを判定する
        /// </summary>
        /// <param name="includeEmptyMagazine">trueなら空マガジンもアオイ基板マガジンと判定する</param>
        /// <returns></returns>
        public bool IsAOIMag(bool includeEmptyMagazine = true)
        {
            if (this.MagazineNo == MAP_AOI_MAGAZINE)
            {
                return true;
            }
            if (includeEmptyMagazine == true)
            {
                // 実マガジン + 空マガジン
                if (string.IsNullOrWhiteSpace(this.MapAoiMagazineLotNo) == false)
                {
                    return true;
                }
            }
            else
            {
                // 実マガジンのみ
                if (this.MapAoiMagazineLotNo == MAP_AOI_MAGAZINE)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="macNo"></param>
        /// <param name="locationid"></param>
        /// <param name="magNo"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static VirtualMag[] GetVirtualMag(string macNo, string locationid, string magNo, int? procno)
        {
            List<VirtualMag> retv = new List<VirtualMag>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                        TnVirtualMag.macno
                        , TmMachine.clasnm
                        , TmMachine.plantnm
                        , TnVirtualMag.locationid
                        , TmGeneral.val AS locationnm
                        , TnVirtualMag.orderid
                        , TnVirtualMag.magno
                        , TnVirtualMag.lastmagno
                        , TnVirtualMag.procno
                        , TnVirtualMag.workstartdt
                        , TnVirtualMag.workenddt
                        , TnVirtualMag.nextmachines
                        , TnVirtualMag.relatedmaterials
                        , TnVirtualMag.framematerialcd
                        , TnVirtualMag.framelotno
                        , TnVirtualMag.framecurrentct
                        , TnVirtualMag.framemaxct
                        , TnVirtualMag.stockerstartno
                        , TnVirtualMag.stockerendno
                        , TnVirtualMag.stockerchangect
                        , TnVirtualMag.stockerlastupddt
                        , TnVirtualMag.stocker1no
                        , TnVirtualMag.stocker2no
                        , TnVirtualMag.waferstartno
                        , TnVirtualMag.waferendno
                        , TnVirtualMag.waferchangect
                        , TnVirtualMag.waferlastupddt
                        , TnVirtualMag.mapaoimaglotno
                        , TnVirtualMag.purgereason
                        , TnVirtualMag.originno
                        , TnVirtualMag.programtotalminutes
                        , TnVirtualMag.lastupddt
                        , TnVirtualMag.priorityfg
                        , TnVirtualMag.originlocationid
                        FROM TnVirtualMag
                        INNER JOIN TmMachine ON TnVirtualMag.macno = TmMachine.macno 
                        INNER JOIN TmGeneral ON TnVirtualMag.locationid = TmGeneral.code AND TmGeneral.tid = N'LOCATION'
                        WHERE  1=1";

                    if (string.IsNullOrEmpty(macNo) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.macno=@MACNO";
                        cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;
                    }

                    if (string.IsNullOrEmpty(magNo) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.magno=@MAGNO";
                        cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magNo;
                    }

                    if (string.IsNullOrEmpty(locationid) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.locationid=@LOCATIONID";
                        cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = locationid;
                    }

                    if (procno.HasValue)
                    {
                        cmd.CommandText += " AND TnVirtualMag.procno=@PROCNO";
                        cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;
                    }

                    cmd.CommandText += " ORDER BY TnVirtualMag.macno,TnVirtualMag.locationid,TnVirtualMag.orderid";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int ordNextMachines = reader.GetOrdinal("nextmachines");
                        int ordRelatedMaterials = reader.GetOrdinal("relatedmaterials");

                        while (reader.Read())
                        {
                            VirtualMag v = new VirtualMag();
                            v.Check = false;
                            v.MacNo = SQLite.ParseInt(reader["macno"]);
                            v.ClassNm = SQLite.ParseString(reader["clasnm"]);
                            v.PlantNm = SQLite.ParseString(reader["plantnm"]);
                            v.LocationId = SQLite.ParseInt(reader["locationid"]);
                            v.LocationNm = SQLite.ParseString(reader["locationnm"]);
                            v.orderid = SQLite.ParseInt(reader["orderid"]);
                            v.MagazineNo = SQLite.ParseString(reader["magno"]);
                            v.LastMagazineNo = SQLite.ParseString(reader["lastmagno"]);
                            v.ProcNo = reader["procno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["procno"]);
                            v.WorkStart = SQLite.ParseDate(reader["workstartdt"]);
                            v.WorkComplete = SQLite.ParseDate(reader["workenddt"]);

                            v.NextMachinesString = SQLite.ParseString(reader[ordNextMachines]);
                            v.RelatedMaterialsString = SQLite.ParseString(reader[ordRelatedMaterials]);
                            v.NextMachines = new List<int>();
                            if (!reader.IsDBNull(ordNextMachines))
                            {
                                string[] nextMachines = SQLite.ParseString(reader[ordNextMachines]).Split(',');
                                foreach (string nextmachine in nextMachines)
                                {
                                    v.NextMachines.Add(int.Parse(nextmachine));
                                }
                            }

                            v.RelatedMaterials = new List<Material>();
                            if (!reader.IsDBNull(ordRelatedMaterials))
                            {
                                string[] relatedMaterials = SQLite.ParseString(reader[ordRelatedMaterials]).Split(',');
                                foreach (string relatedMaterial in relatedMaterials)
                                {
                                    string[] m = relatedMaterial.Split('@');
                                    Material material = Material.GetMaterial(m[0], m[1]);
                                    v.RelatedMaterials.Add(material);
                                }
                            }

                            v.FrameMatCd = SQLite.ParseString(reader["framematerialcd"]);
                            v.FrameLotNo = SQLite.ParseString(reader["framelotno"]);
                            v.CurrentFrameCt = reader["framecurrentct"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["framecurrentct"]);
                            v.MaxFrameCt = reader["framemaxct"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["framemaxct"]);
                            v.StockerStartCol = reader["stockerstartno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["stockerstartno"]);
                            v.StockerEndCol = reader["stockerendno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["stockerendno"]);
                            v.StockerChangeCt = reader["stockerchangect"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["stockerchangect"]);
                            v.LastStockerChange = SQLite.ParseDate(reader["stockerlastupddt"]);
                            v.Stocker1 = reader["stocker1no"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["stocker1no"]);
                            v.Stocker2 = reader["stocker2no"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["stocker2no"]);
                            v.StartWafer = reader["waferstartno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["waferstartno"]);
                            v.EndWafer = reader["waferendno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["waferendno"]);
                            v.WaferChangerChangeCount = reader["waferchangect"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["waferchangect"]);
                            v.WaferChangerChangeTime = SQLite.ParseDate(reader["waferlastupddt"]);
                            v.MapAoiMagazineLotNo = SQLite.ParseString(reader["mapaoimaglotno"]);
                            v.PurgeReason = SQLite.ParseString(reader["purgereason"]);
                            v.Origin = reader["originno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["originno"]);
                            v.ProgramTotalMinutes = reader["programtotalminutes"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["programtotalminutes"]);
                            v.LastUpdDt = SQLite.ParseDate(reader["lastupddt"]).Value;
                            v.PriorityFg = SQLite.ParseBool(reader["priorityfg"]);
                            v.OriginLocationId = reader["originlocationid"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["originlocationid"]);
                            retv.Add(v);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ArmsException("仮想マガジンデータ取得エラー", ex);
            }

            return retv.ToArray();
        }

        public static VirtualMag[] GetVirtualMag(string macNo, string locationid, string magNo) 
        {
            return GetVirtualMag(macNo, locationid, magNo, null);
        }

        public static VirtualMag[] GetVirtualMag(int macNo, int locationid)
        {
            return GetVirtualMag(macNo.ToString(), locationid.ToString(), string.Empty);
        }

        public static VirtualMag[] GetVirtualMag(int macNo)
        {
            return GetVirtualMag(macNo.ToString(), string.Empty, string.Empty);
        }

        public static VirtualMag GetVirtualMag(int locationId, string magNo, int procNo)
        {
            VirtualMag[] magList = GetVirtualMag(null, locationId.ToString(), magNo, procNo);
            if (magList.Count() == 0)
            {
                return null;
            }
            else if (magList.Count() == 1)
            {
                return magList.Single();
            }
            else
            {
                throw new ApplicationException(string.Format("同マガジンNoの仮想マガジンが複数存在します。マガジンNo:{1}", magNo));
            }
        }

        public static VirtualMag GetVirtualMag(int macNo, int locationId, string magNo)
        {
            VirtualMag[] magList = GetVirtualMag(macNo.ToString(), locationId.ToString(), magNo, null);
            if (magList.Count() == 0)
            {
                return null;
            }
            else if (magList.Count() == 1)
            {
                return magList.Single();
            }
            else
            {
                throw new ApplicationException(string.Format("同マガジンNoの仮想マガジンが複数存在します。装置No:{0} マガジンNo:{1}", macNo, magNo));
            }
        }
        
        /// <summary>
        /// 仮想マガジンデータ削除
        /// </summary>
        public void Delete()
        {
            Log.RBLog.Info("仮想マガジンデータ削除 macno:" + this.MacNo + " locationid:" + this.LocationId + " orderid:" + this.orderid + " magno:" + this.MagazineNo);

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = this.LocationId;
                    SqlParameter prmOrderID = cmd.Parameters.Add("@ORDERID", SqlDbType.Int);
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = this.MagazineNo ?? "";

                    prmOrderID.Value = this.orderid;
                    cmd.CommandText = @"
                            DELETE
                                FROM TnVirtualMag 
                            WHERE 
                                macno=@MACNO AND 
                                locationid=@LOCATIONID AND
                                orderid=@ORDERID AND
                                magno=@MAGNO";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("仮想マガジン削除エラー マガジン番号:{0}", this.MagazineNo), ex);
                }
            }
        }

        public static void Delete(Location location, string magno)
        {
            Log.SysLog.Info($"仮想マガジンデータ削除 macno:{ location.MacNo } Station:{ location.Station.ToString() } magno:{ magno }");

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" DELETE FROM TnVirtualMag 
                                        WHERE macno = @MACNO AND locationid = @LOCATIONID AND magno = @MAGAZINENO ";
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = location.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = (int)location.Station;
                    cmd.Parameters.Add("@MAGAZINENO", SqlDbType.NVarChar).Value = magno;

                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 優先搬送フラグを更新
        /// </summary>
        public static void UpdatePriority(VirtualMag mag)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.CommandText = @" UPDATE TnVirtualMag SET priorityfg = @PRIORITYFG 
                                        WHERE macno = @MACNO AND locationid = @LOCATIONID AND magno = @MAGNO ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = mag.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = mag.LocationId;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = mag.MagazineNo;

                    cmd.Parameters.Add("@PRIORITYFG", SqlDbType.Bit).Value = mag.PriorityFg;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("仮想マガジン更新エラー マガジン番号:{0}", mag.MagazineNo), ex);
                }
            }
        }

		public static void UpdatePurgeReason(int macNo, string magazineNo, string purgereason)
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				try
				{
					con.Open();

					cmd.CommandText = @" UPDATE TnVirtualMag SET purgereason = @purgereason 
                                        WHERE macno = @MACNO AND magno = @MAGNO ";

					cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;
					cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magazineNo;

					cmd.Parameters.Add("@purgereason", SqlDbType.NVarChar).Value = purgereason;

					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException(string.Format("仮想マガジン更新エラー マガジン番号:{0}", magazineNo), ex);
				}
			}
		}

        /// <summary>
        /// 次装置を変更
        /// </summary>
        /// <param name="macno"></param>
        public static void UpdateNextMachines(VirtualMag mag) 
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.CommandText = @" UPDATE TnVirtualMag SET nextmachines = @NEXTMACHINES 
                                        WHERE macno = @MACNO AND locationid = @LOCATIONID AND magno = @MAGNO ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = mag.MacNo;
                    cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = mag.LocationId;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = mag.MagazineNo;

                    cmd.Parameters.Add("@NEXTMACHINES", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(string.Join(",", mag.NextMachines));

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("仮想マガジン更新エラー マガジン番号:{0}", mag.MagazineNo), ex);
                }
            }   
        }

        /// <summary>
        /// 空マガジンのインスタンス作成
        /// </summary>
        /// <returns></returns>
        public static VirtualMag CreateEmptyMagazine() 
        {
            VirtualMag mag = new VirtualMag();

            mag.MagazineNo = UNKNOWN_MAGNO;
            mag.ProcNo = null;

            return mag;
        }

		/// <summary>
		/// 装置の指定Stationの最後尾マガジンを取得
		/// </summary>
		/// <param name="macNo"></param>
		/// <param name="station"></param>
		/// <returns></returns>
		public static VirtualMag GetLastTailMagazine(int macNo, Station station) 
		{
			VirtualMag[] mags = VirtualMag.GetVirtualMag(macNo, ((int)station));
			if (mags.Count() == 0) return null;

			return mags.OrderByDescending(m => m.orderid).First();
		}

        /// <summary>
        /// 搬送ロボット上、AGV上のマガジンで指定装置行きの仮想マガジン数取得
        /// </summary>
        /// <param name="macNo"></param>
        /// <returns></returns>
        public static int GetInTransitMagazineCt(List<int> inTransitmacNo, int toMacNo)
        {
            int retv = 0;

            if (inTransitmacNo.Count == 0)
                return 0;

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT macno FROM TnVirtualMag WITH(nolock) WHERE nextmachines like @MACNO AND macno in (" + string.Join(",", inTransitmacNo) + ")";
                cmd.Parameters.Add("@MACNO", SqlDbType.Int).Value = toMacNo;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv++;
                    }
                }
            }

            return retv;
        }

        public static VirtualMag[] GetVirtualMagVCM(int macNo)
        {
            return GetVirtualMagVCM(macNo.ToString(), string.Empty, string.Empty);
        }

        public static VirtualMag GetVirtualMagVCM(int macNo, string magNo)
        {
            return GetVirtualMagVCM(macNo.ToString(), string.Empty, magNo).ToList().FirstOrDefault();
        }

        public static VirtualMag[] GetVirtualMagVCM(string macNo, string locationid, string magNo)
        {
            List<VirtualMag> retv = new List<VirtualMag>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                        TnVirtualMag.macno
                        , TmAGV.name
                        , TnVirtualMag.locationid
                        , TmGeneral.val AS locationnm
                        , TnVirtualMag.orderid
                        , TnVirtualMag.magno
                        , TnVirtualMag.procno
                        , TnVirtualMag.lastupddt
                        , TnVirtualMag.nextmachines
                        FROM TnVirtualMag
                        INNER JOIN TmAGV ON TnVirtualMag.macno = TmAGV.macno 
                        INNER JOIN TmGeneral ON TnVirtualMag.locationid = TmGeneral.code AND TmGeneral.tid = N'LOCATION'
                        WHERE  1=1";

                    if (string.IsNullOrEmpty(macNo) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.macno=@MACNO";
                        cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;
                    }

                    if (string.IsNullOrEmpty(magNo) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.magno=@MAGNO";
                        cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magNo;
                    }

                    if (string.IsNullOrEmpty(locationid) == false)
                    {
                        cmd.CommandText += " AND TnVirtualMag.locationid=@LOCATIONID";
                        cmd.Parameters.Add("@LOCATIONID", SqlDbType.Int).Value = locationid;
                    }

                    cmd.CommandText += " ORDER BY TnVirtualMag.lastupddt";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VirtualMag v = new VirtualMag();
                            v.Check = false;
                            v.MacNo = SQLite.ParseInt(reader["macno"]);
                            v.PlantNm = SQLite.ParseString(reader["name"]);
                            v.LocationId = SQLite.ParseInt(reader["locationid"]);
                            v.LocationNm = SQLite.ParseString(reader["locationnm"]);
                            v.orderid = SQLite.ParseInt(reader["orderid"]);
                            v.MagazineNo = SQLite.ParseString(reader["magno"]);
                            v.ProcNo = reader["procno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(reader["procno"]);
                            v.LastUpdDt = SQLite.ParseDate(reader["lastupddt"]).Value;

                            v.NextMachines = new List<int>();
                            string[] nextMachines = SQLite.ParseString(reader["nextmachines"]).Split(',');
                            foreach (string nextmachine in nextMachines)
                            {
                                v.NextMachines.Add(int.Parse(nextmachine));
                            }

                            retv.Add(v);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ArmsException("仮想マガジンデータ取得エラー", ex);
            }

            return retv.ToArray();
        }
    }
}
