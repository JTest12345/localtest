using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMasterIF
{
    ////////////////////////////////////////
    /// makeprocjson
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Procjsonfolder
    {
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class Path
    {
        public string shinkishutenkaifile { get; set; }
        public string buhinhyoufolder { get; set; }
        public Procjsonfolder procjsonfolder { get; set; }
    }

    public class Model
    {
        public string folder { get; set; }
        public string hankan { get; set; }
        public string kansei { get; set; }
        public List<object> shinkishutenkaicol { get; set; }
    }

    public class Config
    {
        public string type { get; set; }
        public Path path { get; set; }
        public List<Model> model { get; set; }
    }

    public class Makeprocjson
    {
        public string productcat { get; set; }
        public Config config { get; set; }
    }

    public class MakeprocjsonRoot
    {
        public Makeprocjson makeprocjson { get; set; }
    }



    ////////////////////////////////////////
    /// procjsonmodel
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class Formno
    {
        public string xlsaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Released
    {
        public string xlsaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Updateat
    {
        public string xlsaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Revision
    {
        public string xlsaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Hankan
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Kansei
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Kyakusaki
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Typeinfo
    {
        public Hankan hankan { get; set; }
        public Kansei kansei { get; set; }
        public Kyakusaki kyakusaki { get; set; }
    }

    [Serializable]
    public class Torikosu
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Lotmaisu
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Lotpcs
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Lotinfo
    {
        public Torikosu torikosu { get; set; }
        public Lotmaisu lotmaisu { get; set; }
        public Lotpcs lotpics { get; set; }
    }

    [Serializable]
    public class Jp
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Ch
    {
        public string xlscol { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Zuban
    {
        public Jp jp { get; set; }
        public Ch ch { get; set; }
    }

    [Serializable]
    public class Buhinhyou
    {
        public Zuban zuban { get; set; }
        public Typecd typecd { get; set; }
        public Formno formno { get; set; }
        public Released released { get; set; }
    }

    [Serializable]
    public class Etc
    {
        public Buhinhyou buhinhyou { get; set; }
    }

    [Serializable]
    public class Shinkishutenkai
    {
        public Formno formno { get; set; }
        public Released released { get; set; }
        public Updateat updateat { get; set; }
        public Revision revision { get; set; }
        public Typeinfo typeinfo { get; set; }
        public Lotinfo lotinfo { get; set; }
        public Etc etc { get; set; }
    }

    [Serializable]
    public class Typecd
    {
        public string xlsaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Tmworkflow
    {
        public int workorder { get; set; }
        public int procno { get; set; }
        public string formno { get; set; }
        public string formrev { get; set; }
    }

    [Serializable]
    public class Tmprocess
    {
        public int procno { get; set; }
        public string procnm { get; set; }
        public string workcd { get; set; }
    }

    [Serializable]
    public class Arms
    {
        public Tmworkflow tmworkflow { get; set; }
        public Tmprocess tmprocess { get; set; }
        public Tmdefect tmdefect { get; set; }
    }

    [Serializable]
    public class M4
    {
        public string proccd { get; set; }
        public string kjunsho { get; set; }
        public Code code { get; set; }
        public Name name { get; set; }
        public Qty qty { get; set; }
    }

    [Serializable]
    public class Pop
    {
        public string popproccd { get; set; }
        public string popprocname { get; set; }
    }

    [Serializable]
    public class Code
    {
        public string bomaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Name
    {
        public string bomaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Qty
    {
        public string bomaddress { get; set; }
        public string value { get; set; }
    }

    [Serializable]
    public class Material
    {
        public List<M4> m4 { get; set; }
        public Arms arms { get; set; }
    }

    [Serializable]
    public class Tmdefect
    {
        public string itemcd { get; set; }
        public string causecd { get; set; }
        public string classcd { get; set; }
    }

    [Serializable]
    public class Defect
    {
        public string code { get; set; }
        public string name { get; set; }
        public Arms arms { get; set; }
        public M4 m4 { get; set; }
    }

    [Serializable]
    public class Process
    {
        public string code { get; set; }
        public string name { get; set; }
        public Arms arms { get; set; }
        public M4 m4 { get; set; }
        public Pop pop { get; set; }
        public Material material { get; set; }
        public List<Defect> defect { get; set; }
    }

    [Serializable]
    public class Procmastermodel
    {
        public string typecd { get; set; }
        public Shinkishutenkai shinkishutenkai { get; set; }
        public Buhinhyou buhinhyou { get; set; }
        public List<string> processorder { get; set; }
        public List<Process> process { get; set; }
    }

    [Serializable]
    public class MastermodelRoot
    {
        public Procmastermodel procmastermodel { get; set; }
    }


    ////////////////////////////////////////
    /// SeriesTypeMaster
    ////////////////////////////////////////
    ///

    public class SeriesTypeMaster
    {   
        public string seriestypecd { get; set; }
        public Shinkishutenkai shinkishutenkai { get; set; }
        public Buhinhyou buhinhyou { get; set; }
        public Dictionary<string, Process> processdict { get; set; }
    }


    ////////////////////////////////////////
    /// config.json
    ////////////////////////////////////////
    ///

    //// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //public class ProcjsonFolder
    //{
    //    public string hankan { get; set; }
    //    public string kansei { get; set; }
    //}

    //public class Path
    //{
    //    public string shinkishutenkai_file { get; set; }
    //    public string buhinhyou_folder { get; set; }
    //    public ProcjsonFolder procjson_folder { get; set; }
    //}

    //public class Defaultmodel
    //{
    //    public string hankan { get; set; }
    //    public string kansei { get; set; }
    //}

    //public class Model
    //{
    //    public string model { get; set; }
    //    public string typekey { get; set; }
    //    public string hankan { get; set; }
    //    public string kansei { get; set; }
    //}

    //public class Config
    //{
    //    public string type { get; set; }
    //    public Path path { get; set; }
    //    public Defaultmodel defaultmodel { get; set; }
    //    public List<Model> models { get; set; }
    //}

    //public class Makeprocjson
    //{
    //    public string typecat { get; set; }
    //    public List<Config> config { get; set; }
    //}

    //public class ConfigRoot
    //{
    //    public List<Makeprocjson> makeprocjson { get; set; }
    //}


}



