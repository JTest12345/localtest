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
    class NJsonSchema
    {

        static void _Main(string[] args)
        {
            /////////////////////////////////////////////////////////////////
            // Json SchemaからC#クラスを生成するサンプル
            //
            // コンテキストからJson Schemaを取得する
            var assm = Assembly.GetExecutingAssembly();
            string schemaStr;
            // "sctschema.json"のStreamを取得する
            using (var stream = assm.GetManifestResourceStream("Samples.fileContext.cojsample_gaikan.json"))
            {
                var schemaContext = new StreamReader(stream);
                schemaStr = schemaContext.ReadToEnd();
            }

            //Console.WriteLine(schemaStr);

            // COJをデシリアライズする(Schema/UiSchema/FormDataを除く)
            var coj = JsonConvert.DeserializeObject<COJ>(schemaStr);


            foreach (var item in coj.cejObject.objList)
            {
                // NJsonSchema(NuGet)を使ってC#クラス生成
                var schema = JsonSchema.FromJsonAsync(item.Schema.ToString()).GetAwaiter().GetResult();
                var generator = new CSharpGenerator(schema);
                var file = generator.GenerateFile();
                Console.WriteLine(file);
            }


            Console.ReadKey();
        }
    }


}
