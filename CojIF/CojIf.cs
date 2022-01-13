using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CojIF
{
    public class CojIf
    {
        public object cojdat;
        public Type cojIdType;

        public CojIf(string cojstr)
        {
            //COJを取得する
            // ◆COJのCSV適用クラスに上記データを展開・格納
            //　var cojdat = new Coj_csv001_01(dataStr);
            //　↑のようにする代わりにCOJのヘッダーIDからクラスにCOJを渡す。

            // ◆空のJson.netのJObjectにデータを入れてIDを抜く
            JObject jsonconv = JObject.Parse(cojstr);
            var ObjId = jsonconv["header"]["objectId"];

            // ◆対象Classのコンストラクターを取得
            cojIdType = Type.GetType("CojIF." + ObjId);
            Type[] types = new Type[1];
            types[0] = typeof(string);
            ConstructorInfo magicConstructor = cojIdType.GetConstructor(types);

            // ◆対象クラスを実体化
            cojdat = magicConstructor.Invoke(new object[] { cojstr });

        }

        public string TestCojMethod()
        {
            try
            {
                string retstr = string.Empty;

                // ◆格納状態確認
                //　【cojHeder】
                MethodInfo magicMethod = cojIdType.GetMethod("GetCojHeader");
                object GetCojHeader = magicMethod.Invoke(cojdat, new object[] { });
                retstr += GetCojHeader.ToString();

                //　【objheader】cejObjectのヘッダー
                magicMethod = cojIdType.GetMethod("GetObjHeader");
                object GetObjHeader = magicMethod.Invoke(cojdat, new object[] { });
                retstr += GetObjHeader.ToString();

                //　【基本データ】cejObjectのCsv基本データ
                //　【項目情報】cejObjectの項目情報
                // classの全てのメソッドから"GetFd"が含まれているものだけを対象に実行
                foreach (var method in cojIdType.GetMethods())
                {
                    //Console.WriteLine(method.Name);
                    if (method.Name.Contains("GetFd"))
                    {
                        var methedStr = method.Name;
                        magicMethod = cojIdType.GetMethod(methedStr);
                        object GetFd = magicMethod.Invoke(cojdat, new object[] { });
                        retstr += GetFd.ToString();
                    }
                }

                return retstr;

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

    }
}
