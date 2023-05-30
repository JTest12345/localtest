using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う
using System.ComponentModel.DataAnnotations;//プロパティの属性値付けるのに使う
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace KodaClassLibrary {

    /// <summary>
    /// KODAデータベースに接続して帳票システムデータを扱うクラス
    /// </summary>
    public class FormsSQL : KodaSQL {

        #region 帳票システムで使用するものを定義

        /// <summary>
        /// TmFormMasterテーブルのデータクラス
        /// </summary>
        public class TmFormMasterData {

            /// <summary>
            /// 帳票番号
            /// </summary>
            [Display(Name = "帳票番号")]
            public virtual string FormNo { get; set; }

            /// <summary>
            /// 版番号（新規は1）
            /// </summary>
            [Display(Name = "帳票版番号")]
            public virtual int FormRev { get; set; }

            /// <summary>
            /// 帳票名
            /// </summary>
            [Display(Name = "帳票名")]
            public virtual string FormName { get; set; }

            /// <summary>
            /// 帳票検索するためのキーワード
            /// </summary>
            public virtual string Keywords { get; set; }

            /// <summary>
            /// 何用の帳票かを表わす
            /// <para>PRO：製品/工程帳票</para>
            /// <para>PI：定期点検帳票</para>
            /// </summary>
            public virtual string Usefor { get; set; }

            /// <summary>
            /// 帳票データを表わすJson
            /// </summary>
            public virtual string COJ { get; set; }

            /// <summary>
            /// リリースされたかどうか
            /// <para>1:リリースされた　0:リリースされていない</para>
            /// </summary>
            public virtual int IsReleased { get; set; }

            /// <summary>
            /// 最新帳票かどうか
            /// <para>1:最新　0:現在は使用されていない（過去データ取得に必要）</para>
            /// </summary>
            public virtual int IsCurrversion { get; set; }

            /// <summary>
            /// 作成された日時
            /// </summary>
            public virtual DateTime? CreateAt { get; set; }

            /// <summary>
            /// 作成した人
            /// </summary>
            public virtual string CreateBy { get; set; }

            /// <summary>
            /// 更新された日時
            /// </summary>
            public virtual DateTime? UpdateAt { get; set; }

            /// <summary>
            /// 更新した人
            /// </summary>
            [Display(Name = "作業者")]
            public virtual string UpdateBy { get; set; }
        }

        /// <summary>
        /// TnFormTranテーブルのデータクラス
        /// </summary>
        public class TnFormTranData {

            /// <summary>
            /// 機種名
            /// </summary>
            [Display(Name = "機種名")]
            public virtual string Typecd { get; set; }

            /// <summary>
            /// ロット番号(18桁)
            /// </summary>
            [Display(Name = "18桁ロット番号")]
            public virtual string LotNo18 { get; set; }

            /// <summary>
            /// 工程番号
            /// </summary>
            [Display(Name = "工程番号")]
            public virtual int ProcNo { get; set; }

            /// <summary>
            /// 製造拠点
            /// </summary>
            [Display(Name = "製造拠点")]
            public virtual string ManuBase { get; set; }

            /// <summary>
            /// 作業単位
            /// </summary>
            [Display(Name = "作業単位ロット")]
            public virtual string WorkUnitID { get; set; }

            /// <summary>
            /// 工程コード
            /// </summary>
            public virtual string Workcd { get; set; }

            /// <summary>
            /// 設備番号
            /// </summary>
            [Display(Name = "設備番号")]
            public virtual string Plantcd { get; set; }

            /// <summary>
            /// 工程設備番号（普通の設備番号のことではない）
            /// 同じ設備に複数のmacnoがある
            /// </summary>
            public virtual long? MacNo { get; set; }

            /// <summary>
            /// 帳票番号
            /// </summary>
            [Display(Name = "帳票番号")]
            public virtual string FormNo { get; set; }

            /// <summary>
            /// 版番号（新規は1）
            /// </summary>
            [Display(Name = "帳票版番号")]
            public virtual int FormRev { get; set; }

            /// <summary>
            /// 帳票データを表わすJson
            /// </summary>
            public virtual string COJ { get; set; }

            /// <summary>
            /// 帳票入力完了したかどうか
            /// <para>1:完了した　0:完了していない 9:一時保存</para>
            /// </summary>
            public virtual int CloseState { get; set; }

            /// <summary>
            /// レコード挿入された日時
            /// </summary>
            public virtual DateTime InsertAt { get; set; }

            /// <summary>
            /// レコード挿入した人
            /// </summary>
            public virtual string InsertBy { get; set; }

            /// <summary>
            /// 更新された日時
            /// </summary>
            [Display(Name = "更新日時")]
            public virtual DateTime? UpdateAt { get; set; }

            /// <summary>
            /// 更新した人
            /// </summary>
            [Display(Name = "作業者")]
            public virtual string UpdateBy { get; set; }
        }

        /// <summary>
        /// TnPIFormテーブルのデータクラス
        /// </summary>
        public class TnPIFormData {

            /// <summary>
            /// 帳票ID
            /// </summary>
            [Display(Name = "ID")]
            public virtual int ID { get; set; }

            /// <summary>
            /// 設備型式名
            /// </summary>
            [Display(Name = "設備型式")]
            public virtual string MacName { get; set; }

            /// <summary>
            /// 設備番号
            /// </summary>
            [Display(Name = "設備番号")]
            public virtual string Plantcd { get; set; }

            /// <summary>
            /// 設備シリアル番号
            /// </summary>
            [Display(Name = "シリアル番号")]
            public virtual string SerialNo { get; set; }

            /// <summary>
            /// 製造拠点
            /// </summary>
            [Display(Name = "製造拠点")]
            public virtual string ManuBase { get; set; }

            /// <summary>
            /// 設備設置場所
            /// </summary>
            [Display(Name = "場所")]
            public virtual string Place { get; set; }

            /// <summary>
            /// 帳票名
            /// </summary>
            [Display(Name = "帳票名")]
            public virtual string FormName { get; set; }

            /// <summary>
            /// 帳票番号
            /// </summary>
            [Display(Name = "帳票番号")]
            public virtual string FormNo { get; set; }

            /// <summary>
            /// 版番号（新規は1）
            /// </summary>
            [Display(Name = "帳票版番号")]
            public virtual int? FormRev { get; set; }

            /// <summary>
            /// 帳票データを表わすJson
            /// </summary>
            public virtual string COJ { get; set; }

            /// <summary>
            /// 点検周期
            /// </summary>
            [Display(Name = "点検周期")]
            public virtual string Period { get; set; }

            /// <summary>
            /// 帳票入力完了したかどうか
            /// <para>1:完了した　0:完了していない　2:削除された</para>
            /// </summary>
            public virtual int IsClosed { get; set; }

            /// <summary>
            /// この帳票完了時にレコード挿入した次の帳票ID
            /// </summary>
            public virtual int? NextID { get; set; }

            /// <summary>
            /// レコード挿入された日時
            /// </summary>
            [Display(Name = "登録日")]
            public virtual DateTime InsertAt { get; set; }

            /// <summary>
            /// 点検開始日時
            /// </summary>
            [Display(Name = "点検開始日")]
            public virtual DateTime Next { get; set; }

            /// <summary>
            /// 点検実施期限
            /// </summary>
            [Display(Name = "実施期限")]
            public virtual DateTime Limit { get; set; }

            /// <summary>
            /// 更新された日時
            /// </summary>
            [Display(Name = "更新日時")]
            public virtual DateTime? UpdateAt { get; set; }

            /// <summary>
            /// 更新した人
            /// </summary>
            [Display(Name = "作業者")]
            public virtual string UpdateBy { get; set; }

            /// <summary>
            /// 点検周期辞書
            /// </summary>
            public static readonly Dictionary<string, string> period_dic = new Dictionary<string, string>() {
                { "D1", "1日" },
                { "D7", "1週間" },
                { "M1", "1か月" },
                { "M3", "3か月" },
                { "M6", "6か月" },
                { "Y1", "1年" },
            };
        }

        /// <summary>
        /// RepairRecordテーブルのデータクラス
        /// </summary>
        public class RepairRecordData {

            /// <summary>
            /// 記録ID
            /// </summary>
            [Display(Name = "ID")]
            public virtual int ID { get; set; }

            /// <summary>
            /// 設備型式名
            /// </summary>
            [Display(Name = "設備型式")]
            public virtual string MacName { get; set; }

            /// <summary>
            /// 設備番号
            /// </summary>
            [Display(Name = "設備番号")]
            public virtual string Plantcd { get; set; }

            /// <summary>
            /// 修理記録の件名
            /// </summary>
            [Display(Name = "件名")]
            public virtual string Title { get; set; }

            /// <summary>
            /// 不具合現象
            /// </summary>
            [Display(Name = "不具合現象")]
            public virtual string Failure { get; set; }

            /// <summary>
            /// 登録日時
            /// </summary>
            [Display(Name = "登録日時")]
            public virtual DateTimeOffset? InsertAt { get; set; }

            /// <summary>
            /// 登録した人
            /// </summary>
            [Display(Name = "登録者")]
            public virtual string InsertBy { get; set; }

            /// <summary>
            /// 登録時備考
            /// </summary>
            [Display(Name = "登録時備考")]
            public virtual string Remarks1 { get; set; }

            /// <summary>
            /// 原因
            /// </summary>
            [Display(Name = "原因")]
            public virtual string Cause { get; set; }

            /// <summary>
            /// 処置
            /// </summary>
            [Display(Name = "処置")]
            public virtual string Treatment { get; set; }

            /// <summary>
            /// 修理完了日時
            /// </summary>
            [Display(Name = "修理完了日時")]
            public virtual DateTimeOffset? UpdateAt { get; set; }

            /// <summary>
            /// 修理完了した人
            /// </summary>
            [Display(Name = "修理完了者")]
            public virtual string UpdateBy { get; set; }

            /// <summary>
            /// 完了時備考
            /// </summary>
            [Display(Name = "完了時備考")]
            public virtual string Remarks2 { get; set; }
        }

        const string TmFormMaster_TABLE = "[dbo].[TmFormMaster]";
        const string TnFormTran_TABLE = "[dbo].[TnFormTran]";
        const string TnPIForm_TABLE = "[dbo].[TnPIForm]";
        const string RepairRecord_TABLE = "[dbo].[RepairRecord]";

        /* 列名は複数のテーブルで使用しているので
         * 変更する時は慎重に確認する必要があります
         */
        const string cause_COL = "cause";
        const string closestate_COL = "closestate";
        const string coj_COL = "coj";
        const string createat_COL = "createat";
        const string createby_COL = "createby";
        const string failure_COL = "failure";
        const string formname_COL = "formname";
        const string formno_COL = "formno";
        const string formrev_COL = "formrev";
        const string id_COL = "id";
        const string isclosed_COL = "isclosed";
        const string iscurrversion_COL = "iscurrversion";
        const string isreleased_COL = "isreleased";
        const string insertat_COL = "insertat";
        const string insertby_COL = "insertby";
        const string keywords_COL = "keywords";
        const string limit_COL = "limit";
        const string lotno_COL = "lotno";
        const string macname_COL = "macname";
        const string macno_COL = "macno";
        const string manubase_COL = "manubase";
        const string next_COL = "next";
        const string nextid_COL = "nextid";
        const string period_COL = "period";
        const string place_COL = "place";
        const string plantcd_COL = "plantcd";
        const string procno_COL = "procno";
        const string remarks1_COL = "remarks1";
        const string remarks2_COL = "remarks2";
        const string serialno_COL = "serialno";
        const string title_COL = "title";
        const string treatment_COL = "treatment";
        const string typecd_COL = "typecd";
        const string updateat_COL = "updateat";
        const string updateby_COL = "updateby";
        const string usefor_COL = "usefor";
        const string workcd_COL = "workcd";
        const string workunitid_COL = "workunitid";

        #endregion

        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public FormsSQL(string data_source, string db_name, string user_id, string pass, int timeout)
            : base(data_source, db_name, user_id, pass, timeout) {

        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public FormsSQL(string con_str)
            : base(con_str) {

        }

        /// <summary>
        /// 帳票マスタテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_TmFormMasterData<T>(string add_where = null, string new_query = null)
            where T : TmFormMasterData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {formno_COL},{formrev_COL},{formname_COL},{keywords_COL},
                                     {usefor_COL},{coj_COL},{isreleased_COL},{iscurrversion_COL},
                                     {createat_COL},{createby_COL},{updateat_COL},{updateby_COL}
                              FROM {TmFormMaster_TABLE} as mfm
                              WHERE 1=1 ";
            }
            else {
                query = new_query;
            }

            //条件があれば付け加える
            if (add_where != null) {
                query += add_where;
            }

            //データベースからデータ取得
            var table = read_data(query);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    FormNo = col.Contains(formno_COL) ? row[formno_COL].ToString() : "",
                    FormRev = col.Contains(formrev_COL) ? (int)row[formrev_COL] : -1,
                    FormName = col.Contains(formname_COL) ? row[formname_COL].ToString() : "",
                    Keywords = col.Contains(keywords_COL) ? row[keywords_COL].ToString() : "",
                    Usefor = col.Contains(usefor_COL) ? row[usefor_COL].ToString() : "",
                    COJ = col.Contains(coj_COL) ? row[coj_COL].ToString() : "",
                    IsReleased = col.Contains(isreleased_COL) ? (int)row[isreleased_COL] : -1,
                    IsCurrversion = col.Contains(iscurrversion_COL) ? (int)row[iscurrversion_COL] : -1,
                    CreateAt = col.Contains(createat_COL) ? row[createat_COL] as DateTime? : null,
                    CreateBy = col.Contains(createby_COL) ? row[createby_COL].ToString() : "",
                    UpdateAt = col.Contains(updateat_COL) ? row[updateat_COL] as DateTime? : null,
                    UpdateBy = col.Contains(updateby_COL) ? row[updateby_COL].ToString() : ""
                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 製品/工程帳票データテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_TnFormTranData<T>(string add_where = null, string new_query = null)
            where T : TnFormTranData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {typecd_COL},{lotno_COL},{procno_COL},{manubase_COL},
                                     {workunitid_COL},{workcd_COL}, {plantcd_COL},{macno_COL},
                                     {formno_COL},{formrev_COL},{coj_COL},{closestate_COL},
                                     {insertat_COL},{insertby_COL},{updateat_COL},{updateby_COL}
                              FROM {TnFormTran_TABLE} as nft
                              WHERE 1=1 ";
            }
            else {
                query = new_query;
            }

            //条件があれば付け加える
            if (add_where != null) {
                query += add_where;
            }

            //データベースからデータ取得
            var table = read_data(query);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    Typecd = col.Contains(typecd_COL) ? row[typecd_COL].ToString() : "",
                    LotNo18 = col.Contains(lotno_COL) ? row[lotno_COL].ToString() : "",
                    ProcNo = col.Contains(procno_COL) ? (int)row[procno_COL] : -1,
                    ManuBase = col.Contains(manubase_COL) ? row[manubase_COL].ToString() : "",
                    WorkUnitID = col.Contains(workunitid_COL) ? row[workunitid_COL].ToString() : "",
                    Workcd = col.Contains(workcd_COL) ? row[workcd_COL].ToString() : "",
                    Plantcd = col.Contains(plantcd_COL) ? row[plantcd_COL].ToString() : "",
                    MacNo = col.Contains(macno_COL) ? row[macno_COL] as long? : null,
                    FormNo = col.Contains(formno_COL) ? row[formno_COL].ToString() : "",
                    FormRev = col.Contains(formrev_COL) ? (int)row[formrev_COL] : -1,
                    COJ = col.Contains(coj_COL) ? row[coj_COL].ToString() : "",
                    CloseState = col.Contains(closestate_COL) ? (int)row[closestate_COL] : -1,
                    InsertAt = col.Contains(insertat_COL) ? (DateTime)row[insertat_COL] : new DateTime(),
                    InsertBy = col.Contains(insertby_COL) ? row[insertby_COL].ToString() : "",
                    UpdateAt = col.Contains(updateat_COL) ? row[updateat_COL] as DateTime? : null,
                    UpdateBy = col.Contains(updateby_COL) ? row[updateby_COL].ToString() : ""
                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 定期点検帳票データテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_TnPIFormData<T>(string add_where = null, string new_query = null)
            where T : TnPIFormData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {id_COL},{macname_COL},{plantcd_COL},{serialno_COL},
                                     {manubase_COL},{place_COL}, {formname_COL},{formno_COL},
                                     {formrev_COL},{coj_COL},{period_COL},{isclosed_COL},
                                     {nextid_COL},{insertat_COL},{next_COL},{limit_COL},
                                     {updateat_COL},{updateby_COL}
                              FROM {TnPIForm_TABLE} as npif
                              WHERE 1=1 ";
            }
            else {
                query = new_query;
            }

            //条件があれば付け加える
            if (add_where != null) {
                query += add_where;
            }

            //データベースからデータ取得
            var table = read_data(query);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    ID = col.Contains(id_COL) ? (int)row[id_COL] : -1,
                    MacName = col.Contains(macname_COL) ? row[macname_COL].ToString() : "",
                    Plantcd = col.Contains(plantcd_COL) ? row[plantcd_COL].ToString() : "",
                    SerialNo = col.Contains(serialno_COL) ? row[serialno_COL].ToString() : "",
                    ManuBase = col.Contains(manubase_COL) ? row[manubase_COL].ToString() : "",
                    Place = col.Contains(place_COL) ? row[place_COL].ToString() : "",
                    FormName = col.Contains(formname_COL) ? row[formname_COL].ToString() : "",
                    FormNo = col.Contains(formno_COL) ? row[formno_COL].ToString() : "",
                    FormRev = col.Contains(formrev_COL) ? row[formrev_COL] as int? : null,
                    COJ = col.Contains(coj_COL) ? row[coj_COL].ToString() : "",
                    Period = col.Contains(period_COL) ? row[period_COL].ToString() : "",
                    IsClosed = col.Contains(isclosed_COL) ? (int)row[isclosed_COL] : -1,
                    NextID = col.Contains(nextid_COL) ? row[nextid_COL] as int? : null,
                    InsertAt = col.Contains(insertat_COL) ? (DateTime)row[insertat_COL] : new DateTime(),
                    Next = col.Contains(next_COL) ? (DateTime)row[next_COL] : new DateTime(),
                    Limit = col.Contains(limit_COL) ? (DateTime)row[limit_COL] : new DateTime(),
                    UpdateAt = col.Contains(updateat_COL) ? row[updateat_COL] as DateTime? : null,
                    UpdateBy = col.Contains(updateby_COL) ? row[updateby_COL].ToString() : ""
                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 修理記録テーブルからデータを読み取る共通メソッド
        /// </summary>
        private List<T> Get_RepairRecordData<T>(string add_where = null, string new_query = null)
            where T : RepairRecordData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {id_COL},{macname_COL},{plantcd_COL},{title_COL},{failure_COL},{insertat_COL},{insertby_COL},{remarks1_COL},
                                  {cause_COL},{treatment_COL},{updateat_COL},{updateby_COL},{remarks2_COL}
                             FROM {RepairRecord_TABLE} as rr
                             WHERE 1=1 ";
            }
            else {
                query = new_query;
            }

            //条件があれば付け加える
            if (add_where != null) {
                query += add_where;
            }

            //データベースからデータ取得
            var table = read_data(query);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    ID = col.Contains(id_COL) ? (int)row[id_COL] : -1,
                    MacName = col.Contains(macname_COL) ? row[macname_COL].ToString() : "",
                    Plantcd = col.Contains(plantcd_COL) ? row[plantcd_COL].ToString() : "",
                    Title = col.Contains(title_COL) ? row[title_COL].ToString() : "",
                    Failure = col.Contains(failure_COL) ? row[failure_COL].ToString() : "",
                    InsertAt = col.Contains(insertat_COL) ? row[insertat_COL] as DateTimeOffset? : null,
                    InsertBy = col.Contains(insertby_COL) ? row[insertby_COL].ToString() : "",
                    Remarks1 = col.Contains(remarks1_COL) ? row[remarks1_COL].ToString() : "",
                    Cause = col.Contains(cause_COL) ? row[cause_COL].ToString() : "",
                    Treatment = col.Contains(treatment_COL) ? row[treatment_COL].ToString() : "",
                    UpdateAt = col.Contains(updateat_COL) ? row[updateat_COL] as DateTimeOffset? : null,
                    UpdateBy = col.Contains(updateby_COL) ? row[updateby_COL].ToString() : "",
                    Remarks2 = col.Contains(remarks2_COL) ? row[remarks2_COL].ToString() : "",
                };

                list.Add(data);
            }

            return list;
        }


        /// <summary>
        /// 帳票マスタテーブルからリリース済みの帳票情報を取得する
        /// <para>latest_only=trueの時は最新版のみ</para>
        /// </summary>
        /// <param name="latest_only"></param>
        /// <returns>リリース済みの帳票リスト</returns>
        public List<T> Get_FormMasterList<T>(bool latest_only = false)
            where T : TmFormMasterData, new() {

            string add_where = $@"AND {isreleased_COL}=1 ";

            if (latest_only) {
                add_where += $@"AND {iscurrversion_COL}=1 ";
            }

            //データベースからデータ取得
            var list = Get_TmFormMasterData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 帳票マスタテーブルから帳票情報を取得する
        /// </summary>
        public T Get_FormMaster<T>(string formno, int formrev)
            where T : TmFormMasterData, new() {

            string add_where = $@"AND {formno_COL}='{formno}' AND {formrev_COL}={formrev}";

            //データベースからデータ取得
            var list = Get_TmFormMasterData<T>(add_where);

            //データが1個ではない時はおかしい
            if (list.Count == 0) {
                throw new Exception("帳票マスタに使用された帳票のデータがありません。おかしいです。");
            }
            else if (list.Count > 1) {
                var msg = "取得データが1個ではありませんでした。この帳票のマスタデータが重複している可能性があります。";
                throw new Exception(msg);
            }

            return list[0];
        }

        /// <summary>
        /// LotNo(18桁)から製品/工程帳票を取得する。
        /// <para>現在工程で入力が必要な(closestate!=1)機種名、工程番号、LotNo18、設備番号を取得する</para>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Get_FormTran_from_Lotno18<T>(string LotNo18)
            where T : TnFormTranData, new() {

            string add_where = $@"AND {lotno_COL}='{LotNo18}' AND {closestate_COL}!=1 AND {plantcd_COL}!=''";

            var list = Get_TnFormTranData<T>(add_where);

            if (list.Count == 0) { throw new Exception("現在このLotが入力する帳票はありませんでした。"); }

            if (list.Count > 1) { throw new Exception("現在このLotが入力する帳票が2個以上見つかりました。おかしいです。"); }

            return list[0];
        }

        /// <summary>
        /// 機種名、ロット番号から登録済み製品/工程帳票リストを取得する。
        /// <para>複数工程の帳票が取得される</para>
        /// </summary>
        public List<T> Get_ClosedFormTranList<T>(string typecd, string lotno)
            where T : TnFormTranData, new() {

            string add_where = $@"AND {typecd_COL}='{typecd}' AND {lotno_COL}='{lotno}' AND {closestate_COL}=1";

            var list = Get_TnFormTranData<T>(add_where);

            if (list.Count == 0) { throw new Exception("登録済み帳票はありませんでした。"); }

            return list;
        }

        /// <summary>
        /// 作業単位Lot(workunitid)から製品/工程帳票リストを取得する。VLotを想定。
        /// <para>基本はVLot単位でバリ取り工程帳票を記載するが、作業が終わらなくて分割作業することあり</para>
        /// <para>分割した時はisclosed=1 or 9になる（1：完了、9：分割されて一時保存または未開始）</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plantcd"></param>
        /// <returns></returns>
        public List<T> Get_FormTranList_from_WorkUnitID<T>(string work_unit_id)
            where T : TnFormTranData, new() {

            string add_where = $@"AND {workunitid_COL} LIKE '{work_unit_id}%' AND {closestate_COL}!=1";

            var list = Get_TnFormTranData<T>(add_where);

            if (list.Count == 0) { throw new Exception("現在このLotが入力する帳票はありませんでした。"); }

            return list;
        }

        /// <summary>
        /// 製品/工程帳票データテーブルから
        /// 指定の帳票番号+帳票版番号の指定期間内のデータを取得する
        /// </summary>
        /// <param name="form_info">帳票情報クラス</param>
        /// <param name="start">開始日付</param>
        /// <param name="end">終了日付</param>
        /// <returns>登録完了順のデータリスト</returns>
        public List<T> Get_ProductFormData<T>(string formno, int formrev, string start, string end)
            where T : TnFormTranData, new() {

            string add_where = $@"AND {formno_COL}='{formno}' AND {formrev_COL}={formrev}
                                  AND {closestate_COL}=1
                                  AND {updateat_COL}>='{start} 00:00:00.000' AND {updateat_COL}<='{end} 23:59:59.999'
                                  ORDER BY {updateat_COL}";

            //データベースからデータ取得
            var list = Get_TnFormTranData<T>(add_where);

            if (list.Count == 0) {
                throw new Exception("対象のデータはありませんでした。");
            }

            return list;
        }

        /// <summary>
        /// 新システム対象ロットで帳票クローズしていないリストを取得する
        /// </summary>
        /// <returns></returns>
        public List<T> Get_NotClosePFList<T>() where T : TnFormTranData, new() {

            string add_where = $@"AND {procno_COL}>0 AND {closestate_COL}=0";

            var list = Get_TnFormTranData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 旧システム使用機種の製品/工程帳票リストを取得する。
        /// <para>新システムに移行していない機種で帳票システムを使用したいため</para>
        /// <para>対象はprocnoがマイナスの値</para>
        /// </summary>
        public List<T> Get_OldSystePFList<T>(string typecd, string lotno)
            where T : TnFormTranData, new() {

            string add_where = $@"AND {typecd_COL}='{typecd}' AND {lotno_COL}='{lotno}' AND {procno_COL}<0";

            var list = Get_TnFormTranData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 旧システム使用機種の製品/工程帳票リストを取得する。
        /// <para>新システムに移行していない機種で帳票システムを使用したいため</para>
        /// </summary>
        public List<T> Get_OldSystemNotClosePFList<T>()
            where T : TnFormTranData, new() {

            string add_where = $@"AND {procno_COL}<0 AND {closestate_COL}=0";

            var list = Get_TnFormTranData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 定期点検帳票データテーブルから
        /// 指定の帳票番号+帳票版番号+拠点+点検周期のデータを取得する
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public List<T> Get_PIFormData<T>(string formno, int formrev, string manubase, string period)
            where T : TnPIFormData, new() {

            string add_where = $@"AND {formno_COL}='{formno}' AND {formrev_COL}={formrev}
                                  AND {manubase_COL}='{manubase}' AND {period_COL}='{period}'
                                  AND {isclosed_COL}=1
                                  ORDER BY {updateat_COL}";

            //データベースからデータ取得
            var list = Get_TnPIFormData<T>(add_where);

            //データが1個もない時
            if (list.Count == 0) {
                throw new Exception("対象のデータはありませんでした。");
            }

            return list;
        }

        /// <summary>
        /// 拠点+点検周期を条件にして定期点検一覧を取得する
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        /// <exception cref="Exception">データが0個の場合</exception>
        public List<T> Get_PIFormList<T>(string manubase, string period)
            where T : TnPIFormData, new() {

            string add_where = $@"AND {formrev_COL} IS NOT NULL AND {manubase_COL}='{manubase}' AND {period_COL}='{period}'";

            //データベースからデータ取得
            var list = Get_TnPIFormData<T>(add_where);

            //データが1個もない時
            if (list.Count == 0) {
                throw new Exception("対象のデータはありませんでした。");
            }

            return list;
        }

        /// <summary>
        /// 指定した日時が実施期限を過ぎている未実施定期点検を取得する
        /// <para>指定した日時 > 実施期限　の未実施定期点検</para>
        /// </summary>
        /// <returns></returns>
        public List<T> Get_ExpiredPI<T>(DateTime dt)
            where T : TnPIFormData, new() {

            string add_where = $@"AND {isclosed_COL}=0 AND '{dt}' > {limit_COL}";

            var list = Get_TnPIFormData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 指定した日時が点検開始日である未実施定期点検を取得する
        /// <para>前倒しで終わっているものは実施済みなので取得されない</para>
        /// </summary>
        /// <returns></returns>
        public List<T> Get_NewStartPI<T>(DateTime dt)
            where T : TnPIFormData, new() {

            DateTime start_dt = new DateTime(dt.Year, dt.Month, dt.Day);
            DateTime end_dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);

            string add_where = $@"AND {isclosed_COL}=0 AND {next_COL} BETWEEN'{start_dt}' AND '{end_dt}'";

            var list = Get_TnPIFormData<T>(add_where);

            return list;
        }

        /// <summary>
        /// ID指定の未実施定期点検取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get_NotClosePI<T>(int id)
            where T : TnPIFormData, new() {

            string add_where = $@"AND {isclosed_COL}=0 AND {id_COL}={id}";

            var list = Get_TnPIFormData<T>(add_where);

            //id指定の場合は取得データは0個か1個のみ
            if (list.Count == 0) {
                throw new Exception("対象のデータはありませんでした。");
            }

            return list[0];
        }

        /// <summary>
        /// 未実施定期点検一覧取得
        /// </summary>
        public List<T> Get_NotClosePI<T>()
            where T : TnPIFormData, new() {

            string add_where = $@"AND {isclosed_COL}=0";

            var list = Get_TnPIFormData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 修理記録一覧取得
        /// </summary>
        public List<T> Get_RRData<T>()
            where T : RepairRecordData, new() {

            //string add_where = $@"AND {id_COL}={id}";

            var list = Get_RepairRecordData<T>();

            return list;
        }

        /// <summary>
        /// 指定したIDの修理記録取得
        /// </summary>
        public T Get_RRData<T>(int id)
            where T : RepairRecordData, new() {

            string add_where = $@"AND {id_COL}={id}";

            var list = Get_RepairRecordData<T>(add_where);

            if (list.Count > 1) {
                throw new Exception("指定IDの記録が2個以上見つかりました。");
            }

            return list[0];
        }







        /// <summary>
        /// 製品/工程帳票テーブルに登録する
        /// <para>旧システム使用の為、帳票が自動で挿入されない機種に使う</para>
        /// <para>全て新システム使用になればいらない(情シスがレコード追加するため)</para>
        /// </summary>
        public void Insert_PF<T>(T pf)
            where T : TnFormTranData {

            string query = $@"INSERT INTO {TnFormTran_TABLE}
                              ({typecd_COL},{lotno_COL},{procno_COL},{manubase_COL},{workunitid_COL},{workcd_COL},{plantcd_COL},{macno_COL},
                               {formno_COL},{formrev_COL},{coj_COL},{closestate_COL},{insertat_COL},{insertby_COL}) 
                              VALUES(@{typecd_COL},@{lotno_COL},@{procno_COL},@{manubase_COL},@{workunitid_COL},@{workcd_COL},@{plantcd_COL},@{macno_COL},
                               @{formno_COL},@{formrev_COL},@{coj_COL},@{closestate_COL},@{insertat_COL},@{insertby_COL})";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{typecd_COL}", SqlDbType.VarChar, pf.Typecd));
            param_list.Add(create_sql_param($"@{lotno_COL}", SqlDbType.VarChar, pf.LotNo18));
            param_list.Add(create_sql_param($"@{procno_COL}", SqlDbType.Int, pf.ProcNo));
            param_list.Add(create_sql_param($"@{manubase_COL}", SqlDbType.VarChar, pf.ManuBase));
            param_list.Add(create_sql_param($"@{workunitid_COL}", SqlDbType.VarChar, pf.WorkUnitID));
            param_list.Add(create_sql_param($"@{workcd_COL}", SqlDbType.NVarChar, pf.Workcd));
            param_list.Add(create_sql_param($"@{plantcd_COL}", SqlDbType.NVarChar, pf.Plantcd));
            param_list.Add(create_sql_param($"@{macno_COL}", SqlDbType.BigInt, pf.MacNo));
            param_list.Add(create_sql_param($"@{formno_COL}", SqlDbType.VarChar, pf.FormNo));
            param_list.Add(create_sql_param($"@{formrev_COL}", SqlDbType.Int, pf.FormRev));
            param_list.Add(create_sql_param($"@{coj_COL}", SqlDbType.NVarChar, pf.COJ));
            param_list.Add(create_sql_param($"@{closestate_COL}", SqlDbType.Int, pf.CloseState));
            param_list.Add(create_sql_param($"@{insertat_COL}", SqlDbType.DateTime, pf.InsertAt));
            param_list.Add(create_sql_param($"@{insertby_COL}", SqlDbType.NVarChar, pf.InsertBy));

            execute(query, param_list);
        }

        /// <summary>
        /// 製品/工程帳票テーブルの対象帳票を消す
        /// <para>旧システム対象機種のみに使う</para>
        /// <para>間違って2回以上登録してしまうことがあるため</para>
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        public void Delete_NotClosePF(string typecd, string lotno, int procno) {

            string query = $@"DELETE FROM {TnFormTran_TABLE}
                              WHERE {typecd_COL}='{typecd}' AND {lotno_COL}='{lotno}' AND {procno_COL}={procno} AND {closestate_COL}=0";

            execute(query);
        }

        /// <summary>
        /// 定期点検テーブルに登録する
        /// </summary>
        public void Insert_PI<T>(T pi)
            where T : TnPIFormData, new() {

            //定期点検帳票テーブルの最大idを取得
            string query = $@"SELECT MAX({id_COL}) 
                              FROM {TnPIForm_TABLE}";

            DataTable t = read_data(query);

            int id;
            if (String.IsNullOrEmpty(t.Rows[0].ItemArray[0].ToString())) {
                //最初だけは空のobjectが返ってきてしまう為(nullではない)
                id = 1;
            }
            else {
                id = (int)t.Rows[0].ItemArray[0];
                id += 1;
            }

            //帳票マスタテーブルから帳票名を取得
            query = $@"SELECT {formname_COL} 
                       FROM {TmFormMaster_TABLE}
                       WHERE {formno_COL}='{pi.FormNo}' AND {iscurrversion_COL}=1";

            t = read_data(query);
            string formname = t.Rows[0].ItemArray[0].ToString();


            query = $@"INSERT INTO {TnPIForm_TABLE}
                              ({id_COL},{macname_COL},{plantcd_COL},{serialno_COL},{manubase_COL},{place_COL},{formname_COL},{formno_COL},
                               {coj_COL},{period_COL},{isclosed_COL},{insertat_COL},{next_COL},{limit_COL}) 
                              VALUES(@{id_COL},@{macname_COL},@{plantcd_COL},@{serialno_COL},@{manubase_COL},@{place_COL},@{formname_COL},@{formno_COL},
                               @{coj_COL},@{period_COL},@{isclosed_COL},@{insertat_COL},@{next_COL},@{limit_COL})";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{id_COL}", SqlDbType.Int, id));
            param_list.Add(create_sql_param($"@{macname_COL}", SqlDbType.NVarChar, pi.MacName));
            param_list.Add(create_sql_param($"@{plantcd_COL}", SqlDbType.VarChar, pi.Plantcd));
            param_list.Add(create_sql_param($"@{serialno_COL}", SqlDbType.NVarChar, pi.SerialNo));
            param_list.Add(create_sql_param($"@{manubase_COL}", SqlDbType.VarChar, pi.ManuBase));
            param_list.Add(create_sql_param($"@{place_COL}", SqlDbType.NVarChar, pi.Place));
            param_list.Add(create_sql_param($"@{formname_COL}", SqlDbType.NVarChar, formname));
            param_list.Add(create_sql_param($"@{formno_COL}", SqlDbType.VarChar, pi.FormNo));
            param_list.Add(create_sql_param($"@{coj_COL}", SqlDbType.NVarChar, "{}"));
            param_list.Add(create_sql_param($"@{period_COL}", SqlDbType.VarChar, pi.Period));
            param_list.Add(create_sql_param($"@{isclosed_COL}", SqlDbType.Int, 0));
            param_list.Add(create_sql_param($"@{insertat_COL}", SqlDbType.DateTime, pi.InsertAt));
            param_list.Add(create_sql_param($"@{next_COL}", SqlDbType.DateTime, pi.Next));
            param_list.Add(create_sql_param($"@{limit_COL}", SqlDbType.DateTime, pi.Limit));

            execute(query, param_list);
        }

        /// <summary>
        /// 定期点検帳票を消す（レコードは消さないでisclose=2にする）
        /// </summary>
        /// <param name="id"></param>
        public void Delete_PI(int id, string empcd) {

            var dt = DateTime.Now;

            string query = $@"UPDATE {TnPIForm_TABLE}
                              SET {isclosed_COL}=2,{updateat_COL}='{dt}',{updateby_COL}='{empcd}' 
                              WHERE {id_COL}={id}";

            execute(query);
        }

        /// <summary>
        /// 修理記録を登録する
        /// </summary>
        /// <returns>登録ID</returns>
        public int Insert_RR<T>(T rr)
            where T : RepairRecordData, new() {

            //修理記録テーブルの最大idを取得
            string query = $@"SELECT MAX({id_COL}) 
                              FROM {RepairRecord_TABLE}";

            DataTable t = read_data(query);

            int insert_id;
            if (String.IsNullOrEmpty(t.Rows[0].ItemArray[0].ToString())) {
                //最初だけは空のobjectが返ってきてしまう為(nullではない)
                insert_id = 1;
            }
            else {
                insert_id = (int)t.Rows[0].ItemArray[0];
                insert_id += 1;
            }


            string insert_query = $@"INSERT INTO {RepairRecord_TABLE}
                                     ({id_COL},{macname_COL},{plantcd_COL},{title_COL},{failure_COL},{insertat_COL},{insertby_COL},{remarks1_COL})
                              VALUES (@{id_COL},@{macname_COL},@{plantcd_COL},@{title_COL},@{failure_COL},@{insertat_COL},@{insertby_COL},@{remarks1_COL})";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{id_COL}", SqlDbType.Int, insert_id));
            param_list.Add(create_sql_param($"@{macname_COL}", SqlDbType.NVarChar, rr.MacName));
            param_list.Add(create_sql_param($"@{plantcd_COL}", SqlDbType.NVarChar, rr.Plantcd));
            param_list.Add(create_sql_param($"@{title_COL}", SqlDbType.NVarChar, rr.Title));
            param_list.Add(create_sql_param($"@{failure_COL}", SqlDbType.NVarChar, rr.Failure));
            param_list.Add(create_sql_param($"@{insertat_COL}", SqlDbType.DateTimeOffset, DateTimeOffset.Now));
            param_list.Add(create_sql_param($"@{insertby_COL}", SqlDbType.NVarChar, rr.InsertBy));
            param_list.Add(create_sql_param($"@{remarks1_COL}", SqlDbType.NVarChar, rr.Remarks1));

            execute(insert_query, param_list);

            return insert_id;
        }

        /// <summary>
        /// 修理記録を更新する
        /// </summary>
        public void Update_RR<T>(T rr)
            where T : RepairRecordData, new() {

            string update_query = $@"UPDATE {RepairRecord_TABLE}
                                     SET {cause_COL}=@{cause_COL},
                                         {treatment_COL}=@{treatment_COL},
                                         {updateat_COL}=@{updateat_COL},
                                         {updateby_COL}=@{updateby_COL},
                                         {remarks2_COL}=@{remarks2_COL}
                                     WHERE {id_COL}={rr.ID}";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{cause_COL}", SqlDbType.NVarChar, rr.Cause));
            param_list.Add(create_sql_param($"@{treatment_COL}", SqlDbType.NVarChar, rr.Treatment));
            param_list.Add(create_sql_param($"@{updateat_COL}", SqlDbType.DateTimeOffset, DateTimeOffset.Now));
            param_list.Add(create_sql_param($"@{updateby_COL}", SqlDbType.NVarChar, rr.UpdateBy));
            param_list.Add(create_sql_param($"@{remarks2_COL}", SqlDbType.NVarChar, rr.Remarks2));

            execute(update_query, param_list);
        }


    }



}
