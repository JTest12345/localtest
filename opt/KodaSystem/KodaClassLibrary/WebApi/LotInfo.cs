using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;

using Newtonsoft.Json;


namespace KodaClassLibrary {

    /// <summary>
    /// ロット情報クラス
    /// </summary>
    public class LotInfo {

        /// <summary>
        /// 機種名
        /// </summary>
        [JsonProperty("productName")]
        public virtual string ProductName { get; set; }

        /// <summary>
        /// 10桁ロット番号(実績ロット)
        /// </summary>
        [JsonProperty("lotno10")]
        public virtual string LotnNo10 { get; set; }

        /// <summary>
        /// 18桁ロット番号(ARMSシステム)
        /// </summary>
        [JsonProperty("lotno18")]
        public virtual string LotnNo18 { get; set; }

        /// <summary>
        /// V溝ロット番号
        /// </summary>
        [JsonProperty("vlotno")]
        public virtual string VLotNo { get; set; }

        /// <summary>
        /// 基板が入っているマガジン番号
        /// </summary>
        [JsonProperty("magazineNo")]
        public virtual string MagazineNo { get; set; }

        /// <summary>
        /// 完了工程(前工程)
        /// </summary>
        [JsonProperty("preProcess")]
        public virtual Process PreProcess { get; set; }

        /// <summary>
        /// 現在実施中工程(ARMSで作業開始していなければ無い)
        /// </summary>
        [JsonProperty("nowProcess")]
        public virtual Process NowProcess { get; set; }

        /// <summary>
        /// 次工程
        /// </summary>
        [JsonProperty("nextProcess")]
        public virtual Process NextProcess { get; set; }

        /// <summary>
        /// 工程クラス
        /// </summary>
        public class Process {

            /// <summary>
            /// 工程名(SDR塗布とか)
            /// </summary>
            [JsonProperty("processName")]
            public virtual string ProcessName { get; set; }

            /// <summary>
            /// 工程番号(8とか)
            /// </summary>
            [JsonProperty("processNo")]
            public virtual int ProcessNo { get; set; }

            /// <summary>
            /// 工程コード(SDRとか)
            /// </summary>
            [JsonProperty("processCode")]
            public virtual string ProcessCode { get; set; }

            /// <summary>
            /// 製品/工程帳票完了
            /// <para>true = 帳票完了</para>
            /// </summary>
            [JsonProperty("closePF")]
            public virtual bool ClosePF { get; set; }
        }

        /// <summary>
        /// ロット情報を取得する共通メソッド
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static LotInfo GetLotInfo(string url) {

            string json = "";
            using (var client = new HttpClient()) {

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("ContentType", "application/json");

                //リクエスト送信
                var resp = client.SendAsync(request).Result;

                if (resp.IsSuccessStatusCode) {
                    json = resp.Content.ReadAsStringAsync().Result;
                }
                else {
                    string msg = $"status code {(int)resp.StatusCode} : {resp.ReasonPhrase}\n";

                    string resp_body = resp.Content.ReadAsStringAsync().Result;
                    var body = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp_body);
                    msg += body["Message"];

                    throw new Exception(msg);
                }
            }

            var lotinfo_res = JsonConvert.DeserializeObject<LotInfoResponse>(json);

            if (lotinfo_res.OnlyJissekiSystem) {
                throw new NotNewSystemException(lotinfo_res.ErrorMessage);
            }

            if (lotinfo_res.LotInfo == null) {
                throw new Exception(lotinfo_res.ErrorMessage);
            }

            return lotinfo_res.LotInfo;
        }

        /// <summary>
        /// 18桁のロット番号からロット情報を取得する
        /// </summary>
        /// <param name="lotno18"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LotInfo GetLotInfo_from_LotNo18(string lotno18) {

            string url = $@"{KodaWebApi.KODA_API_URL}lotinfo/18/{lotno18}";

            return GetLotInfo(url);
        }

        /// <summary>
        /// 機種名と10桁のロット番号からロット情報を取得する
        /// </summary>
        /// <param name="productname"></param>
        /// <param name="lotno10"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LotInfo GetLotInfo_from_LotNo10(string productname, string lotno10) {

            string url = $@"{KodaWebApi.KODA_API_URL}lotinfo/10/{productname}/{lotno10}";

            return GetLotInfo(url);
        }
    }
}