using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMasterIF
{
    ////////////////////////////////////////
    /// 実績部材割付: 製品工程構成
    /// Props.propdata Object
    ////////////////////////////////////////
    [Serializable]
    public class SeihinKouteiKosei
    {
        public string hinmokukodo { get; set; }
        public List<Seihinkousei> seihinkousei { get; set; }
    }

    [Serializable]
    public class Seihinkousei
    {
        public string meisho { get; set; }
        public List<int> siyouryou { get; set; }
        public List<string> yukokigen { get; set; }
    }


    ////////////////////////////////////////
    /// 実績部材割付: 品目マスタ―
    /// hinmokumaster.yaml読込用
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    [Serializable]
    public class HINMOKUMASTER
    {
        public string code { get; set; }
        public string kouteimei { get; set; }
        public List<Kouteisagyoubango> kouteisagyoubango { get; set; }
    }

    public class Kouteisagyoubango
    {
        public string sagyobango { get; set; }
        public Prop prop { get; set; }
    }

    public class Prop
    {
        public int sagyoujun { get; set; }
        public bool yukoujyoutai { get; set; }
        public string command { get; set; }
    }

}

namespace ProcMasterIF.Coj.mst000001
{
    ////////////////////////////////////////
    /// 実績部材割付: COJ本体
    /// ID: coj-mst-000001
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class CejObject
    {
        public CoHeader coHeader { get; set; }
        public List<CoList> coList { get; set; }
    }

    [Serializable]
    public class CoHeader
    {
        public List<Doc> docs { get; set; }
        public Kishuprofile kishuprofile { get; set; }
    }

    [Serializable]
    public class CoList
    {
        public string function { get; set; }
        public Props props { get; set; }
    }

    [Serializable]
    public class Doc
    {
        public string name { get; set; }
        public string formno { get; set; }
        public string released { get; set; }
        public string update { get; set; }
        public string revision { get; set; }
        public string formno_jp { get; set; }
        public string formno_ch { get; set; }
    }

    [Serializable]
    public class Header
    {
        public string objecttypeId = "CojMst000001";
        public int objectTypeRev = 1;
        public string objectType = "modify-master-propaties";
        public string thisdocNo { get; set; }
        public string createdBy { get; set; }
        public string createdAt { get; set; }
        public string updateBy { get; set; }
        public string updateAt { get; set; }
        public string discription = "実績収集部材割付用フォーマット";
    }

    [Serializable]
    public class Kishuprofile
    {
        public string name { get; set; }
        public string foldername { get; set; }
        public int revision { get; set; }
    }

    [Serializable]
    public class Props
    {
        public string propname { get; set; }
        public List<object> propdata { get; set; }
    }

    [Serializable]
    public class Root
    {
        public Header header { get; set; }
        public CejObject cejObject { get; set; }
    }
}

    namespace ProcMasterIF.Coj.mst000002
{
    ////////////////////////////////////////
    /// 4MBOP編集用: COJ本体
    /// ID: coj-mst-000002
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CejObject
    {
        public CoHeader coHeader { get; set; }
        public List<CoList> coList { get; set; }
    }

    public class CoHeader
    {
        public List<Doc> docs { get; set; }
        public Kishuprofile kishuprofile { get; set; }
    }

    public class CoList
    {
        public string function { get; set; }
        public Props props { get; set; }
    }

    public class Doc
    {
        public string name { get; set; }
        public string formno { get; set; }
        public string released { get; set; }
        public string update { get; set; }
        public string revision { get; set; }
        public string formno_jp { get; set; }
        public string formno_ch { get; set; }
    }

    public class Kishuprofile
    {
        public string name { get; set; }
        public string foldername { get; set; }
        public int revision { get; set; }
    }

    public class Props
    {
        public string propname { get; set; }
        public List<object> propdata { get; set; }
    }
    public class KishuInfoPropData
    {
        public string kishucode { get; set; }
        public string soukogr { get; set; }
    }

    public class KouteisagyouPropData
    {
        public string kouteisagyou { get; set; }
        public string sagyoukijunkodo { get; set; }
        public string koutei { get; set; }
        public int sagyoujun { get; set; }
        public bool yukoujyoutai { get; set; } = true;
        public KouteisagyouOption option { get; set; }
        public List<Buzai> buzai { get; set; }
    }

    public class KouteisagyouOption
    {
        public string kanritani { get; set; }
        public string platform { get; set; }
        public string setteisagyoujikan { get; set; }
        public string setteikousu { get; set; }
        public string kousujyougen { get; set; }
        public string kousukagen { get; set; }
        public string setteibudomari { get; set; }
        public string budomarijyougen { get; set; }
        public string budomarikagen { get; set; }
        public string houchijikan { get; set; }
        public string tairyukanoujikan { get; set; }
        public string seisanhituyoujikan { get; set; }
        public string seisankanoujikan { get; set; }
    }

    public class HenshuKouteiSagyouPropData
    {
        public string kouteisagyou { get; set; }
        public string sagyoukijunkodo { get; set; }
        public List<Buzai> buzai { get; set; }
    }

    public class Buzai
    {
        public string buzaikodo { get; set; }
        public string siyouryou { get; set; }
        public string orzairyou { get; set; }
    }

    public class Header
    {
        public string objecttypeId = "CojMst000002";
        public int objectTypeRev = 1;
        public string objectType = "modify-master-propaties";
        public string thisdocNo { get; set; }
        public string createdBy { get; set; }
        public string createdAt { get; set; }
        public string updateBy { get; set; }
        public string updateAt { get; set; }
        public string discription = "4MBOP編集用フォーマット";
    }

    public class Root
    {
        public Header header { get; set; }
        public CejObject cejObject { get; set; }
    }

}

namespace ProcMasterIF.Coj.mst000003
{
    ////////////////////////////////////////
    /// 素子波長制限編集用: COJ本体
    /// ID: coj-mst-000003
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CejObject
    {
        public CoHeader coHeader { get; set; }
        public List<CoList> coList { get; set; }
    }

    public class CoHeader
    {
        public List<Doc> docs { get; set; }
        public Kishuprofile kishuprofile { get; set; }
    }

    public class CoList
    {
        public string function { get; set; }
        public Props props { get; set; }
    }

    public class Doc
    {
        public string name { get; set; }
        public string formno { get; set; }
        public string released { get; set; }
        public string update { get; set; }
        public string revision { get; set; }
        public string formno_jp { get; set; }
        public string formno_ch { get; set; }
    }

    public class Header
    {
        public string objecttypeId { get; set; }
        public int objectTypeRev { get; set; }
        public string objectType { get; set; }
        public string discription { get; set; }
        public string thisdocNo { get; set; }
        public string createdBy { get; set; }
        public string createdAt { get; set; }
        public object updateBy { get; set; }
        public object updateAt { get; set; }
    }

    public class Kishuprofile
    {
        public string name { get; set; }
        public string foldername { get; set; }
        public int revision { get; set; }
    }

    public class Propdatum
    {
        public string meisho { get; set; }
        public string code { get; set; }
        public string cejrank { get; set; }
        public string tounyukyoka { get; set; }
        public string sortingrank { get; set; }
    }

    public class Props
    {
        public string propname { get; set; }
        public List<Propdatum> propdata { get; set; }
    }

    public class Root
    {
        public Header header { get; set; }
        public CejObject cejObject { get; set; }
    }
}



