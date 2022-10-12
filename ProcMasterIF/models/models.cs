using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMasterIF
{
    ////////////////////////////////////////
    /// ProcObjects
    ////////////////////////////////////////
    public class PROC_OBJECTS
    {
        public MastermodelRoot ProcObj { get; set; }
        public Coj.mst000001.Root CojJsk { get; set; }
        public Coj.mst000002.Root Coj4m { get; set; }
        public Coj.mst000003.Root CojSs { get; set; }
        public string TypeCdList { get; set; }
    }

    ////////////////////////////////////////
    /// ArmsConfigObj
    ////////////////////////////////////////
    public class ArmsDbConfig
    {
        public string datasource { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string initialcatalog { get; set; }
        public List<string> table { get; set; }
    }

    ////////////////////////////////////////
    /// makeprocjson
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Jsonfolder
    {
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class ArmsTypecdTbls
    {
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class CojFolder
    {
        public Jsonfolder jissekifolder { get; set; }
        public Jsonfolder m4folder { get; set; }
        public Jsonfolder soshifolder { get; set; }
    }

    public class Path
    {
        public string shinkishutenkaifile { get; set; }
        public string buhinhyoufolder { get; set; }
        public Jsonfolder procjsonfolder { get; set; }
        public CojFolder cojfolder { get; set; }
        public string soshiseigenfile { get; set; }
        public ArmsTypecdTbls armstypecdtbls { get; set; }
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
        public Etcinfo etc { get; set; }
    }

    public class MakeprocjsonRoot
    {
        public Makeprocjson makeprocjson { get; set; }
    }

    public class Etcinfo
    {
        public string soukogr { get; set; }
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
    // 構造が被っているところがあるので整理が必要
    // とりあえず動いていそうなので後回し。。。
    public class M4
    {
        public string proccd { get; set; }
        public string kjunsho { get; set; }
        public string kanritani { get; set; }
        private string _platform { get; set; }
        private string _setteisagyoujikan { get; set; }
        private string _setteikousu { get; set; }
        private string _kousujyougen { get; set; }
        private string _kousukagen { get; set; }
        private string _setteibudomari { get; set; }
        private string _budomarijyougen { get; set; }
        private string _budomarikagen { get; set; }
        private string _houchijikan { get; set; }
        private string _tairyukanoujikan { get; set; }
        private string _seisanhituyoujikan { get; set; }
        private string _seisankanoujikan { get; set; }
        public Code code { get; set; }
        public Name name { get; set; }
        public Qty qty { get; set; }
        public string orzai { get; set; }

        public string platform 
        {
            set { _platform = CheckValueAndFormat(value, 0); }
            get { return _platform;  }
        }
        public string setteisagyoujikan
        {
            set { _setteisagyoujikan = CheckValueAndFormat(value, 2); }
            get { return _setteisagyoujikan; }
        }
        public string setteikousu
        {
            set { _setteikousu = CheckValueAndFormat(value, 4); }
            get { return _setteikousu; }
        }
        public string kousujyougen
        {
            set { _kousujyougen = CheckValueAndFormat(value, 2); }
            get { return _kousujyougen; }
        }
        public string kousukagen
        {
            set { _kousukagen = CheckValueAndFormat(value, 2); }
            get { return _kousukagen; }
        }
        public string setteibudomari
        {
            set { _setteibudomari = CheckValueAndFormat(value, 2); }
            get { return _setteibudomari; }
        }
        public string budomarijyougen
        {
            set { _budomarijyougen = CheckValueAndFormat(value, 2); }
            get { return _budomarijyougen; }
        }
        public string budomarikagen
        {
            set { _budomarikagen = CheckValueAndFormat(value, 2); }
            get { return _budomarikagen; }
        }
        public string houchijikan
        {
            set { _houchijikan = CheckValueAndFormat(value, 2); }
            get { return _houchijikan; }
        }
        public string tairyukanoujikan
        {
            set { _tairyukanoujikan = CheckValueAndFormat(value, 2); }
            get { return _tairyukanoujikan; }
        }
        public string seisanhituyoujikan
        {
            set { _seisanhituyoujikan = CheckValueAndFormat(value, 2); }
            get { return _seisanhituyoujikan; }
        }
        public string seisankanoujikan
        {
            set { _seisankanoujikan = CheckValueAndFormat(value, 2); }
            get { return _seisankanoujikan; }
        }
        
        private string CheckValueAndFormat(string stringNumber, int dp)
        {
            if (stringNumber == "")
            {
                return "";
            }

            int numericValue;
            bool isNumber = int.TryParse(stringNumber, out numericValue);
            if (!isNumber)
            {
                return "error";
            }

            var ret = string.Empty;
            if (dp == 0)
            {
                ret = $"{numericValue:F0}";
            }
            else if (dp == 1)
            {
                ret = $"{numericValue:F1}";
            }
            else if (dp == 2)
            {
                ret = $"{numericValue:F2}";
            }
            else if (dp == 3)
            {
                ret = $"{numericValue:F3}";
            }
            else if (dp == 4)
            {
                ret = $"{numericValue:F4}";
            }
            else
            {
                ret = "error";
            }

            return ret;
        }
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
        //public Arms arms { get; set; }
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
        // public List<Defect> defect { get; set; }
    }

    [Serializable]
    public class Procmastermodel
    {
        public string typecd { get; set; }
        public Shinkishutenkai shinkishutenkai { get; set; }
        public Buhinhyou buhinhyou { get; set; }
        public List<HINMOKUMASTER> hinmokumaster { get; set; }
        public List<string> processorder { get; set; }
        public List<Process> process { get; set; }
        public Dictionary<string, System.Data.DataTable> amstbls { get; set; }
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
        public Model model { get; set; }
        public List<int> collist { get; set; }
        public SoshiSeigen soshiseigen { get; set; }
        public ArmsDbConfig armsdbconfig { get; set; }
        public Dictionary<string, Dictionary<string, System.Data.DataTable>> amstblsdict { get; set; }
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

    ////////////////////////////////////////
    /// 共通台帳： 工程定義台帳.yaml
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Armsproc
    {
        public string table { get; set; }
        public List<string> key { get; set; }
        public List<Col> cols { get; set; }
    }

    public class Col
    {
        public string name { get; set; }
        public string type { get; set; }
        public string allownull { get; set; }
        public string address { get; set; }
    }

    public class Datum
    {
        public Armsproc arms { get; set; }
    }

    public class Header
    {
        public string aunther { get; set; }
        public string firstdate { get; set; }
        public object update { get; set; }
        public string datatype { get; set; }
        public int rowstart { get; set; }
        public List<string> sheets { get; set; }
    }

    public class KouteiTigiRoot
    {
        public Header header { get; set; }
        public List<Datum> data { get; set; }
    }



    ////////////////////////////////////////
    /// 素子制限.yaml
    ////////////////////////////////////////
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Buhinkodo
    {
        public string xlscol { get; set; }
        public string xlsaddress { get; set; }
    }

    public class Cejrank
    {
        public int xlsrow { get; set; }
        public int datasu { get; set; }
        public Datasta datasta { get; set; }
    }

    public class Datasta
    {
        public int xlscol { get; set; }
    }

    public class Hachouinfo
    {
        public Hinfo Buhinkodo { get; set; }
        public Hinfo hachourank { get; set; }
        public Hinfo cejrank { get; set; }
    }

    public class Hinfo
    {
        public int xlsrow { get; set; }
        public int datasu { get; set; }
        public Datasta datasta { get; set; }
    }

    public class Indexinfo
    {
        public Kishu kishu { get; set; }
        public Shiyousoshi shiyousoshi { get; set; }
        public Buhinkodo buhinkodo { get; set; }
        public Buhinkodo sbuhinkodo { get; set; }
    }

    public class Kishu
    {
        public string xlscol { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Released_ss
    {
        public string xlsaddress { get; set; }
    }

    public class Revision_ss
    {
        public string xlscol { get; set; }
    }


    public class Shiyousoshi
    {
        public string xlscol { get; set; }
    }

    public class Sortinginfo
    {
        public Sortingrank sortingrank { get; set; }
    }

    public class Sortingrank
    {
        public int xlsrow { get; set; }
        public int datasu { get; set; }
        public Datasta datasta { get; set; }
    }

    public class Updateat_ss
    {
        public string xlscol { get; set; }
    }

    public class SoshiSeigen
    {
        public Released_ss released { get; set; }
        public Updateat_ss updateat { get; set; }
        public Revision_ss revision { get; set; }
        public Indexinfo indexinfo { get; set; }
        public Hachouinfo hachouinfo { get; set; }
        public Sortinginfo sortinginfo { get; set; }
    }


}



