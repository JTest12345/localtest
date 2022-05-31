using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMasterIF
{

    public static class ObjectExtension
    {
        // ディープコピーの複製を作る拡張メソッド
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter
                  = new System.Runtime.Serialization
                        .Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src); // シリアライズ
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
            }
        }
    }
}
