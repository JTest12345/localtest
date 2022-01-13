using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.CojCsModel
{
    public class gaikansample
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Header
        {
            public string objectId { get; set; }
            public string objectType { get; set; }
            public string docNo { get; set; }
            public string createdBy { get; set; }
            public string createdAt { get; set; }
            public string updatedBy { get; set; }
            public string updatedAt { get; set; }
            public int revision { get; set; }
            public bool isRelesed { get; set; }
            public bool isDeleted { get; set; }
            public string division { get; set; }
            public string site { get; set; }
            public string procno { get; set; }
            public string plantcd { get; set; }
            public string macno { get; set; }
            public string typecd { get; set; }
            public string lotno { get; set; }
            public string magno { get; set; }
            public string description { get; set; }
        }

        public class CoHeader
        {
            public bool hasValues { get; set; }
            public bool isClosed { get; set; }
        }

        public class MmStatus
        {
            public string inpStatus { get; set; }
            public bool hasValues { get; set; }
            public bool isClosed { get; set; }
            public bool? useDefaultFmData { get; set; }
        }

        public class InStatus
        {
            public string inpStatus { get; set; }
            public bool hasValues { get; set; }
            public bool isClosed { get; set; }
        }

        public class Kishulot
        {
            public string type { get; set; }
            public string title { get; set; }
        }

        public class Tanto
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Jyoukenmei
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Gouki
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Kensajikan
        {
            public string type { get; set; }
            public string title { get; set; }
        }

        public class Hour1k
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Budomari
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Tounyusu
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Ryouhinkei
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Furyoukei
        {
            public string type { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        public class Properties
        {
            public Kishulot kishulot { get; set; }
            public Tanto tanto { get; set; }
            public Jyoukenmei jyoukenmei { get; set; }
            public Gouki gouki { get; set; }
            public Kensajikan kensajikan { get; set; }
            public Hour1k hour1k { get; set; }
            public Budomari budomari { get; set; }
            public Tounyusu tounyusu { get; set; }
            public Ryouhinkei ryouhinkei { get; set; }
            public Furyoukei furyoukei { get; set; }
            public Name name { get; set; }
            public Colno colno { get; set; }
            public Unit unit { get; set; }
            public Kaisijikan kaisijikan { get; set; }
            public Shuryoujikan shuryoujikan { get; set; }
            public Jikan jikan { get; set; }
            public Kaisu kaisu { get; set; }
            public Ryouhinsu ryouhinsu { get; set; }
            public Furyo1 furyo1 { get; set; }
            public Furyo2 furyo2 { get; set; }
            public Mikensa mikensa { get; set; }
            public Hp1000 hp1000 { get; set; }
            public Costpp costpp { get; set; }
        }

        public class Name
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Colno
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Unit
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Kaisijikan
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Shuryoujikan
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Jikan
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Kaisu
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Ryouhinsu
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Furyo1
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Furyo2
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Mikensa
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Hp1000
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Costpp
        {
            public string title { get; set; }
            public string type { get; set; }
        }

        public class Items
        {
            public string type { get; set; }
            public Properties properties { get; set; }
        }

        public class Schema
        {
            public string title { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public List<object> required { get; set; }
            public Properties properties { get; set; }
            public int? minItems { get; set; }
            public Items items { get; set; }
        }

        public class UiSchema
        {
        }

        public class CoList
        {
        }

        public class DataNames
        {
            public string test { get; set; }
        }

        public class ProdFormRjsfList
        {
            public MmStatus mmStatus { get; set; }
            public InStatus inStatus { get; set; }
            public Schema schema { get; set; }
            public UiSchema uiSchema { get; set; }
            public CoList coList { get; set; }
            public DataNames dataNames { get; set; }
        }

        public class CejObject
        {
            public CoHeader formHeader { get; set; }
            public List<ProdFormRjsfList> prodFormRjsfList { get; set; }
        }

        public class Root
        {
            public Header header { get; set; }
            public CejObject cejObject { get; set; }
        }


    }
}
