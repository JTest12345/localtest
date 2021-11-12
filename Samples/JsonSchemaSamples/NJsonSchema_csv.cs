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
using Newtonsoft.Json.Linq;

namespace Samples
{
    class NJsonSchema_csv
    {

        static void _Main(string[] args)
        {
            ///////////////////////////////////////////////////////////////////
            //// Json SchemaからC#クラスを生成するサンプル
            ////
            //// コンテキストからJson Schemaを取得する
            //var assm = Assembly.GetExecutingAssembly();
            //string schemaStr;
            //// "csvschema.json"のStreamを取得する
            //using (var stream = assm.GetManifestResourceStream("Samples.fileContext.csvschema.json"))
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
            // "csvdata.json"のStreamを取得する
            using (var stream = assm.GetManifestResourceStream("Samples.fileContext.csvdata.json"))
            {
                var dataContext = new StreamReader(stream);
                dataStr = dataContext.ReadToEnd();
            }

            JObject jsonconv = JObject.Parse(dataStr);
            foreach (var item in jsonconv["formData"])
            {
                var data = JsonConvert.DeserializeObject<csv_01>(item.ToString());
                Console.WriteLine("-------------------");
                Console.WriteLine("name: " + data.Name);
                Console.WriteLine("colno: " + data.Colno);
                Console.WriteLine("unit: " + data.Unit);
                Console.WriteLine("type: " + data.Type);
            }



            Console.ReadKey();
        }


    }
    // NJsonSchemaで生成されたクラス
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class csv_01
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("colno", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Colno { get; set; }

        [Newtonsoft.Json.JsonProperty("unit", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Unit { get; set; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    }
}