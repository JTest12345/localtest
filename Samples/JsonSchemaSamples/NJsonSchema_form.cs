using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using Newtonsoft.Json;

namespace Samples
{
    class NJsonSchema_form
    {

        static void _Main(string[] args)
        {
            ///////////////////////////////////////////////////////////////////
            //// Json SchemaからC#クラスを生成するサンプル
            ////
            //// コンテキストからJson Schemaを取得する
            //var assm = Assembly.GetExecutingAssembly();
            //string schemaStr;
            //// "sctschema.json"のStreamを取得する
            //using (var stream = assm.GetManifestResourceStream("Samples.fileContext.sctschema.json"))
            //{
            //    var schemaContext = new StreamReader(stream);
            //    schemaStr = schemaContext.ReadToEnd();
            //}

            //// Console.WriteLine(schemaStr);

            //// NJsonSchema(NuGet)を使ってC#クラス生成
            //var schema = JsonSchema.FromJsonAsync(schemaStr).GetAwaiter().GetResult();
            //var generator = new CSharpGenerator(schema);
            //var file = generator.GenerateFile();
            //Console.WriteLine(file);


            /////////////////////////////////////////////////////////////////
            // NJsonSchemaで生成されたクラスを使ってデータを読取るサンプル
            //
            // コンテキストからJson Schemaを取得する
            var assm = Assembly.GetExecutingAssembly();
            string dataStr;
            // "sctdatat.json"のStreamを取得する
            using (var stream = assm.GetManifestResourceStream("Samples.fileContext.sctdata.json"))
            {
                var dataContext = new StreamReader(stream);
                dataStr = dataContext.ReadToEnd();
            }

            var data = JsonConvert.DeserializeObject<sct_01>(dataStr);
            Console.WriteLine("macno:" + data.Macno);
            Console.WriteLine("nano_prgno:" + data.Nano_prgno);
            Console.WriteLine("ml:" + data.Ml);
            Console.WriteLine("shot_prgno:" + data.Shot_prgno);
            Console.WriteLine("check1:" + data.Check1);
            Console.WriteLine("check2:" + data.Check2);
            Console.WriteLine("check3:" + data.Check3);
            Console.WriteLine("strat_datetime:" + data.Strat_datetime.AltDatetime);
            Console.WriteLine("First_gaikan:" + data.First_gaikan.GetType());
            foreach (var item in data.First_gaikan)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("end_datetime:" + data.End_datetime.AltDatetime);
            Console.WriteLine("Last_gaikan:" + data.Last_gaikan.GetType());
            foreach (var item in data.Last_gaikan)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("check4:" + data.Check4);


            Console.ReadKey();
        }
    }


    /////////////////////////////////////////////////////////////////
    // NJsonSchemaで生成されたクラス
    /// <summary>記録番号：JQQ-E0036</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class sct_01
    {
        [Newtonsoft.Json.JsonProperty("macno", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int Macno { get; set; }

        [Newtonsoft.Json.JsonProperty("nano_prgno", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int Nano_prgno { get; set; }

        [Newtonsoft.Json.JsonProperty("ml", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.Range(0D, double.MaxValue)]
        public double Ml { get; set; }

        [Newtonsoft.Json.JsonProperty("shot_prgno", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int Shot_prgno { get; set; }

        [Newtonsoft.Json.JsonProperty("check1", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Check1 Check1 { get; set; }

        [Newtonsoft.Json.JsonProperty("check2", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Check2 Check2 { get; set; }

        [Newtonsoft.Json.JsonProperty("check3", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Check3 { get; set; }

        [Newtonsoft.Json.JsonProperty("strat_datetime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Strat_datetime Strat_datetime { get; set; }

        [Newtonsoft.Json.JsonProperty("first_gaikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public System.Collections.Generic.ICollection<First_gaikan> First_gaikan { get; set; }

        [Newtonsoft.Json.JsonProperty("end_datetime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public End_datetime End_datetime { get; set; }

        [Newtonsoft.Json.JsonProperty("last_gaikan", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public System.Collections.Generic.ICollection<Last_gaikan> Last_gaikan { get; set; }

        [Newtonsoft.Json.JsonProperty("check4", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Check4 { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum Check1
    {
        [System.Runtime.Serialization.EnumMember(Value = @"OK")]
        OK = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"交換")]
        交換 = 1,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum Check2
    {
        [System.Runtime.Serialization.EnumMember(Value = @"OK")]
        OK = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"交換")]
        交換 = 1,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Strat_datetime
    {
        [Newtonsoft.Json.JsonProperty("alt-datetime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset AltDatetime { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum First_gaikan
    {
        [System.Runtime.Serialization.EnumMember(Value = @"不良無し")]
        不良無し = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"液だれ")]
        液だれ = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"気充填")]
        気充填 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"溢れ")]
        溢れ = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"気泡")]
        気泡 = 4,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class End_datetime
    {
        [Newtonsoft.Json.JsonProperty("alt-datetime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset AltDatetime { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public enum Last_gaikan
    {
        [System.Runtime.Serialization.EnumMember(Value = @"不良無し")]
        不良無し = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"液だれ")]
        液だれ = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"気充填")]
        気充填 = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"溢れ")]
        溢れ = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"気泡")]
        気泡 = 4,

    }

}
