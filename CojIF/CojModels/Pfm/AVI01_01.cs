using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CojIF
{
    /// <summary>
    /// {COJ}のCSVファイル適用実験クラス
    /// </summary>
    /// 
    public class AVI01_01 : COJ
    {
        //
        // ◆基底クラスをCOJクラスとしています
        // 　COJファイル共通のクラスをCOJとしています。
        // 　ヘッダーなど各オブジェクトはインターフェースを使っています
        // 　⇒ クラスを超えてForeachが使えるんじゃないかと思いつつ(未検証)
        //

        //
        // ◆入力データの取出し
        // 　COJ毎にプロパティは変わってしまうので個別対応
        // 　ここでもCSV読込のインターフェースを用意して基本的な要素はなるべくあわせていく方向
        //

        public class Fd01
        {
            public HeaderItem formdata { get; set; }
        }

        public class HeaderItem
        {
            public string kishulot { get; set; }
            public string tanto { get; set; }
            public string jyoken { get; set; }
            public string setubi { get; set; }
        }

        public class Fd02
        {
            public JissekiJouho formdata { get; set; }
        }


        public class Fd03
        {
            public List<FuryoJouho> formdata { get; set; }
        }


        ///////////////////////////////////////////
        ///
        /// FORM DATA 
        /// 
        ///////////////////////////////////////////

        // 基本情報
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
        public partial class KihonJyouho
        {
            [Newtonsoft.Json.JsonProperty("kishulot", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Kishulot { get; set; }

            [Newtonsoft.Json.JsonProperty("tanto", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Tanto { get; set; }

            [Newtonsoft.Json.JsonProperty("jyoukenmei", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Jyoukenmei { get; set; }

            [Newtonsoft.Json.JsonProperty("gouki", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double Gouki { get; set; }

            private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

            [Newtonsoft.Json.JsonExtensionData]
            public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
            {
                get { return _additionalProperties; }
                set { _additionalProperties = value; }
            }
        }

        // 実績情報
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
        public partial class JissekiJouho
        {
            [Newtonsoft.Json.JsonProperty("kaishijikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Kaishijikan { get; set; }

            [Newtonsoft.Json.JsonProperty("shuryojikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Shuryojikan { get; set; }

            [Newtonsoft.Json.JsonProperty("kensajikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Kensajikan { get; set; }

            [Newtonsoft.Json.JsonProperty("hpk", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Hpk { get; set; }

            [Newtonsoft.Json.JsonProperty("budomari", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Budomari { get; set; }

            [Newtonsoft.Json.JsonProperty("tounyusu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long Tounyusu { get; set; }

            [Newtonsoft.Json.JsonProperty("ryouhinkei", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long Ryouhinkei { get; set; }

            [Newtonsoft.Json.JsonProperty("furyoukei", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long Furyoukei { get; set; }

            private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

            [Newtonsoft.Json.JsonExtensionData]
            public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
            {
                get { return _additionalProperties; }
                set { _additionalProperties = value; }
            }
        }

        // 不良情報
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
        public partial class FuryoJouho
        {
            [Newtonsoft.Json.JsonProperty("furyocode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Furyocode { get; set; }

            [Newtonsoft.Json.JsonProperty("furyomei", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Furyomei { get; set; }

            [Newtonsoft.Json.JsonProperty("haishutusu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long Haishutusu { get; set; }

            [Newtonsoft.Json.JsonProperty("kenshutusu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public long Kenshutusu { get; set; }

            private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

            [Newtonsoft.Json.JsonExtensionData]
            public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
            {
                get { return _additionalProperties; }
                set { _additionalProperties = value; }
            }
        }

        // 投入情報
        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
        public partial class TounyuJouho
        {
            [Newtonsoft.Json.JsonProperty("kaisijikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Kaisijikan { get; set; }

            [Newtonsoft.Json.JsonProperty("shuryoujikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Shuryoujikan { get; set; }

            [Newtonsoft.Json.JsonProperty("jikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Jikan { get; set; }

            [Newtonsoft.Json.JsonProperty("kaisu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Kaisu { get; set; }

            [Newtonsoft.Json.JsonProperty("budomari", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double Budomari { get; set; }

            [Newtonsoft.Json.JsonProperty("tounyusu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Tounyusu { get; set; }

            [Newtonsoft.Json.JsonProperty("ryouhinsu", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Ryouhinsu { get; set; }

            [Newtonsoft.Json.JsonProperty("furyo1", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Furyo1 { get; set; }

            [Newtonsoft.Json.JsonProperty("furyo2", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Furyo2 { get; set; }

            [Newtonsoft.Json.JsonProperty("mikensa", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Mikensa { get; set; }

            [Newtonsoft.Json.JsonProperty("hpk", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double Hpk { get; set; }

            [Newtonsoft.Json.JsonProperty("costpp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public double Costpp { get; set; }

            private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

            [Newtonsoft.Json.JsonExtensionData]
            public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
            {
                get { return _additionalProperties; }
                set { _additionalProperties = value; }
            }
        }
    }

}
