using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ResinClassLibrary {

    /// <summary>
    /// Recipeクラスで使う部材クラス
    /// </summary>
    public class RecipeBuzai : MixBuzai {

        /// <summary>
        /// 配合量上限値
        /// </summary>
        [JsonIgnore]
        public decimal UpperLimit {
            get {
                return Amount + UpperAllowableErrorGram;
            }
        }

        /// <summary>
        /// 配合量下限値
        /// </summary>
        [JsonIgnore]
        public decimal LowerLimit {
            get {
                return Amount - LowerAllowableErrorGram;
            }
        }

        /// <summary>
        /// 実際の計量結果
        /// </summary>
        [JsonIgnore]
        public decimal ResultAmount { get; set; }

        /// <summary>
        /// 計量開始時間
        /// </summary>
        [JsonIgnore]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 計量完了時間
        /// </summary>
        [JsonIgnore]
        public DateTime EndTime { get; set; }

    }


    /// <summary>
    /// MixInfoクラスで使う部材クラス
    /// </summary>
    public class MixBuzai : Buzai {

        /// <summary>
        /// Format名称
        /// </summary>
        [JsonIgnore]
        public string FormatName { get; set; }

        /// <summary>
        /// 100gベース配合量[g]
        /// </summary>
        [JsonProperty("base-amount")]
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// 総重量に対する割合
        /// <para>ろ過後重量に対する配合量[g]を計算する時に使用する</para>
        /// </summary>
        [JsonProperty("percent-of-totalWeight")]
        public decimal? PercentOfTotalWeight { get; set; }

        /// <summary>
        /// 配合量[g]
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 自動配合機を使うかどうか
        /// <para>true = 使う (初期値はfalse)</para>
        /// <para>使いたい部材の品番とロット番号が同じものが、自動配合機にセットされている場合にtrueにする</para>
        /// </summary>
        [JsonProperty("use-autoMachine")]
        public bool UseAutoMachine { get; set; } = false;

        /// <summary>
        /// 配合順番(1,2,3,…)
        /// </summary>
        [JsonProperty("mixOrder")]
        public int MixOrder { get; set; }

    }


    /// <summary>
    /// ベースとなる部材クラス
    /// </summary>
    public class Buzai {
 
        public const string RESIN_A = "RESIN_A";
        public const string RESIN_B = "RESIN_B";
        public const string FILLER = "FILLER";
        public const string TIO2 = "TiO2";
        public const string TORO = "TORO";
        public const string RED = "RED";
        public const string YELLOW = "YELLOW";


        /// <summary>
        /// Fコード
        /// </summary>
        [JsonProperty("f-code")]
        public virtual string Fcode { get; set; }

        /// <summary>
        /// 部材名
        /// </summary>
        [JsonProperty("name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// 部材種類
        /// <para>RESIN_A、RESIN_B、FILLER、TORO、RED、YELLOW</para>
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        /// <summary>
        /// ロット番号
        /// </summary>
        [JsonProperty("lotno")]
        public virtual string LotNo { get; set; }

        /// <summary>
        /// 配合時にグローブボックスを使うか部材か？
        /// <para>true = 使う</para>
        /// </summary>
        [JsonProperty("need-grovebox")]
        public virtual bool NeedGroveBox { get; set; }

        /// <summary>
        /// 許容誤差[g](絶対値)
        /// <para>±0.001g ⇒ 0.001 で設定する</para>
        /// </summary>
        [JsonProperty("allowableError-gram")]
        public decimal? AllowableErrorGram { get; set; }

        /// <summary>
        /// 許容誤差[%](絶対値)
        /// <para>±0.025% ⇒ 0.025 で設定する</para>
        /// </summary>
        [JsonProperty("allowableError-percent")]
        public decimal? AllowableErrorPercent { get; set; }

        /// <summary>
        /// 上許容誤差[g](絶対値)
        /// <para>±0.001g ⇒ 0.001 で設定する</para>
        /// </summary>
        [JsonProperty("upperAllowableError-gram")]
        public decimal UpperAllowableErrorGram { get; set; }

        /// <summary>
        /// 下許容誤差[g](絶対値)
        /// <para>±0.001g ⇒ 0.001 で設定する</para>
        /// </summary>
        [JsonProperty("lowerAllowableError-gram")]
        public decimal LowerAllowableErrorGram { get; set; }


        /// <summary>
        /// 部材情報ファイルから辞書を作成する
        /// <para>dic_keyで辞書のキーを指定する</para>
        /// <para>0：Fコード</para>
        /// <para>1：Format名称</para>
        /// <para>2：部材名</para>
        /// </summary>
        public static Dictionary<string, Buzai> Get_BuzaiInfo(string path, int dic_key) {

            var dic = new Dictionary<string, Buzai>();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                //1行目を読み込む（使わない）
                sr.ReadLine();

                //2行目以降
                string str, format_name, name, fcode;

                while (true) {
                    str = sr.ReadLine();

                    //空白ならループ終了
                    if (str == "" | str == null) {
                        break;
                    }

                    string[] sp = str.Split(',');

                    fcode = sp[0];
                    format_name = sp[1];
                    name = sp[2];

                    try {
                        var buzai = new Buzai {
                            Fcode = fcode,
                            Name = name,
                            Type = sp[3],
                            NeedGroveBox = bool.Parse(sp[4])
                        };

                        if (double.Parse(sp[5]) < 0) {
                            buzai.AllowableErrorGram = 0.001m;
                        }
                        else {
                            buzai.AllowableErrorPercent = decimal.Parse(sp[5]);
                        }

                        if (dic_key == 0) {
                            dic.Add(fcode, buzai);
                        }
                        else if (dic_key == 1) {
                            dic.Add(format_name, buzai);
                        }
                        else {
                            dic.Add(name, buzai);
                        }
                    }
                    catch (Exception ex) {
                        throw;
                    }
                }
            }
            return dic;
        }

    }
}
