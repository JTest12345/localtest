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
    /// <summary>
    /// {COJ}のCSVファイル適用実験クラス
    /// </summary>
    /// 
    public class COJ_CSV001_01 : COJ
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
            public FdProps01 formdata { get; set; }
        }

        public class FdProps01 : IFd01_csv
        {
            public string Filepath { get; set; }
            public int Header { get; set; }
            public string Encoding { get; set; }
        }

        public class Fd02
        {
            public List<FdProps02> formdata { get; set; }
        }

        public class FdProps02 : IFd02_csv
        {
            public string Name { get; set; }
            public int Colno { get; set; }
            public string Unit { get; set; }
            public string Type { get; set; }
        }

        public Fd01 _Fd01;
        public Fd02 _Fd02;

        //
        // ◆Constractor
        // 　Jsonの文字列を読み込んで各プロパティに値を代入しています。
        //
        public COJ_CSV001_01(string cojstr)
        {
            // COJをデシリアライズする(Schema/UiSchema/FormDataを除く)
            var coj = JsonConvert.DeserializeObject<COJ>(cojstr);

            // 基底クラスへの適用
            this.cojHeader = coj.cojHeader;
            this.cejObject = coj.cejObject;

            // 入力データへの適用
            _Fd01 = JsonConvert.DeserializeObject<Fd01>("{\"formdata\":" + coj.cejObject.objList[0].formData.ToString() + "}");
            _Fd02 = JsonConvert.DeserializeObject<Fd02>("{\"formdata\":" + coj.cejObject.objList[1].formData.ToString() + "}");

        }

        //
        // ◆CSV基本情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd01Csv()
        {
            var ret = string.Empty;
            foreach (var prop in this._Fd01.formdata.GetType().GetProperties())
            {
                ret += $"_Fd01.{prop.Name}: " + prop.GetValue(this._Fd01.formdata) + "\r\n";
            }

            return ret;
        }

        //
        // ◆CSV項目情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd02Csv()
        {
            var ret = string.Empty;
            foreach (var props in this._Fd02.formdata)
            {
                ret += "-------------------------------" + "\r\n";
                foreach (var prop in props.GetType().GetProperties())
                {
                    ret += $"_Fd02.{prop.Name}: " + prop.GetValue(props) + "\r\n";
                }
            }

            return ret;
        }

    }

    /// <summary>
    /// TEST
    /// </summary>
    /// 
    public class COJClassTest
    {
        static void _Main(string[] args)
        {
            // コンテキストからCOJを取得する
            var assm = Assembly.GetExecutingAssembly();
            string dataStr;
            // "fileContext/csvsample.json"のStreamを取得する
            using (var stream = assm.GetManifestResourceStream("Samples.fileContext.cojsample.json"))
            {
                var dataContext = new StreamReader(stream);
                dataStr = dataContext.ReadToEnd();
            }

            // ◆COJのCSV適用クラスに上記データを展開・格納
            //　var cojdat = new Coj_csv001_01(dataStr);
            //　↑のようにする代わりにCOJのヘッダーIDからクラスにCOJを渡してみます。

            // ◆空のJson.netのJObjectにとりあえずデータを入れてIDを抜く
            JObject jsonconv = JObject.Parse(dataStr);
            var ObjId = jsonconv["cojHeader"]["objectId"];

            // ◆対象Classのコンストラクターを取得
            Type magicType = Type.GetType("Samples." + ObjId);
            Type[] types = new Type[1];
            types[0] = typeof(string);
            ConstructorInfo magicConstructor = magicType.GetConstructor(types);

            // ◆対象クラスを実体化
            object cojdat = magicConstructor.Invoke(new object[] { dataStr });

            // ◆格納状態確認
            //　【cojHeder】
            MethodInfo magicMethod = magicType.GetMethod("GetCojHeader");
            object GetCojHeader = magicMethod.Invoke(cojdat, new object[] {  });
            Console.WriteLine(GetCojHeader.ToString());
            //　【objheader】cejObjectのヘッダー
            magicMethod = magicType.GetMethod("GetObjHeader");
            object GetObjHeader = magicMethod.Invoke(cojdat, new object[] { });
            Console.WriteLine(GetObjHeader.ToString());
            //　【基本データ】cejObjectのCsv基本データ
            //　【項目情報】cejObjectの項目情報
            // classの全てのメソッドから"GetFd"が含まれているものだけを対象に実行
            foreach (var method in magicType.GetMethods())
            {
                //Console.WriteLine(method.Name);
                if (method.Name.Contains("GetFd"))
                {
                    var methedStr = method.Name;
                    magicMethod = magicType.GetMethod(methedStr);
                    object GetFd = magicMethod.Invoke(cojdat, new object[] { });
                    Console.WriteLine(GetFd.ToString());
                }
            }

            Console.ReadKey();
        }
    }
}
