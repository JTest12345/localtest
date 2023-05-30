using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace KodaClassLibrary {

    /// <summary>
    /// KODA APIに接続して情報を取得するクラス
    /// </summary>
    public class KodaWebApi {

        /// <summary>
        /// KODA WEB APIにつなげるURLのベース部分
        /// </summary>
        [JsonIgnore]
        public const string KODA_API_URL = @"https://koda.cej.citizen.co.jp/kodaweb/api/";

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 新システムに存在するかどうか(18桁のロット番号がシステムに存在するか)
        /// <para>true = 実績収集システムのみ(ARMS,4Mシステムは使用していない)</para>
        /// </summary>
        [JsonProperty("onlyJissekiSystem")]
        public bool OnlyJissekiSystem { get; set; } = false;

    }

    /// <summary>
    /// LotInfoについての応答クラス
    /// </summary>
    public class LotInfoResponse : KodaWebApi {

        [JsonProperty("lotinfo")]
        public LotInfo LotInfo { get; set; }
    }


    /// <summary>
    /// 新システムに対応していない場合の例外
    /// </summary>
    public class NotNewSystemException : Exception {

        /// <summary>
        /// 引数無しのコンストラクタ
        /// </summary>
        public NotNewSystemException() {
            //
        }

        /// <summary>
        /// メッセージ文字列を渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        public NotNewSystemException(string message) : base(message) {
            // メッセージ文字列を渡すコンストラクタ
        }

        /// <summary>
        /// メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="innerException">発生済み例外オブジェクト</param>
        public NotNewSystemException(string message, Exception innerException) : base(message, innerException) {
            //メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        }
    }

    /// <summary>
    /// ArmsApiDLLからの例外
    /// </summary>
    public class ArmsSystemException : Exception {

        /// <summary>
        /// 引数無しのコンストラクタ
        /// </summary>
        public ArmsSystemException() {
            //
        }

        /// <summary>
        /// メッセージ文字列を渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        public ArmsSystemException(string message) : base(message) {
            // メッセージ文字列を渡すコンストラクタ
        }

        /// <summary>
        /// メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="innerException">発生済み例外オブジェクト</param>
        public ArmsSystemException(string message, Exception innerException) : base(message, innerException) {
            //メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        }

    }

    /// <summary>
    /// 富士情報DLLからの例外
    /// </summary>
    public class FjhSystemException : Exception {

        /// <summary>
        /// 引数無しのコンストラクタ
        /// </summary>
        public FjhSystemException() {
            //
        }

        /// <summary>
        /// メッセージ文字列を渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        public FjhSystemException(string message) : base(message) {
            // メッセージ文字列を渡すコンストラクタ
        }

        /// <summary>
        /// メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="innerException">発生済み例外オブジェクト</param>
        public FjhSystemException(string message, Exception innerException) : base(message, innerException) {
            //メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        }

    }


}
