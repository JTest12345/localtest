using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Samples
{
    //
    // Json.net版
    //

    class Json2CsClass
    {
        static void _Main(string[] args)
        {
            string jsonString = "{\"Header\":{\"ObjectId\":\"TCSDB0001\",\"CreatedBy\":\"\",},\"TCSDB0001\":{\"ProductionFormRjsf\":[{\"status\":{\"inpStatus\":\"incomplete\"}}]}}";

            JObject jsonconv = JObject.Parse(jsonString);

            Console.WriteLine(jsonconv["Header"]["ObjectId"]);

            Type magicType = Type.GetType("Samples." + jsonconv["Header"]["ObjectId"].ToString());
            ConstructorInfo magicConstructor = magicType.GetConstructor(Type.EmptyTypes);
            object magicClassObject = magicConstructor.Invoke(new object[] { });
        }
    }

    public class TCSDB0001
    {
        public TCSDB0001()
        {
            Console.WriteLine("Costructed!");
            Console.ReadKey();
        }
    }

}
