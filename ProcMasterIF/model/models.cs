using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMasterIF
{

    ////////////////////////////////////////
    /// procjsonmodel
    ////////////////////////////////////////

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Formno
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class released
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Updateat
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Revision
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Hankan
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Kansei
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Kyakusaki
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Typeinfo
    {
        public Hankan hankan { get; set; }
        public Kansei kansei { get; set; }
        public Kyakusaki kyakusaki { get; set; }
    }

    public class Torikosu
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class MaisuLot
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class PkgLot
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Lotinfo
    {
        public Torikosu torikosu { get; set; }
        public MaisuLot maisu_lot { get; set; }
        public PkgLot pkg_lot { get; set; }
    }

    public class Jp
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Ch
    {
        public string col { get; set; }
        public string value { get; set; }
    }

    public class Zuban
    {
        public Jp jp { get; set; }
        public Ch ch { get; set; }
    }

    public class Buhinhyou
    {
        public Zuban zuban { get; set; }
        public Typecd typecd { get; set; }
        public Formno formno { get; set; }
        public released released { get; set; }
    }

    public class Etc
    {
        public Buhinhyou buhinhyou { get; set; }
    }

    public class Shinkishutenkai
    {
        public Formno formno { get; set; }
        public released released { get; set; }
        public Updateat updateat { get; set; }
        public Revision revision { get; set; }
        public Typeinfo typeinfo { get; set; }
        public Lotinfo lotinfo { get; set; }
        public Etc etc { get; set; }
    }

    public class Typecd
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Tmworkflow
    {
        public int workorder { get; set; }
        public int procno { get; set; }
        public string formno { get; set; }
        public string formrev { get; set; }
    }

    public class Tmprocess
    {
        public int procno { get; set; }
        public string procnm { get; set; }
        public string workcd { get; set; }
    }

    public class Arms
    {
        public Tmworkflow tmworkflow { get; set; }
        public Tmprocess tmprocess { get; set; }
        public Tmdefect tmdefect { get; set; }
    }

    public class M4
    {
        public string proccd { get; set; }
        public Code code { get; set; }
        public Name name { get; set; }
    }

    public class Pop
    {
        public string popproccd { get; set; }
        public string popprocname { get; set; }
    }

    public class Code
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Name
    {
        public string address { get; set; }
        public string value { get; set; }
    }

    public class Material
    {
        public List<M4> m4 { get; set; }
        public Arms arms { get; set; }
    }

    public class Tmdefect
    {
        public string itemcd { get; set; }
        public string causecd { get; set; }
        public string classcd { get; set; }
    }

    public class Defect
    {
        public string code { get; set; }
        public string name { get; set; }
        public Arms arms { get; set; }
        public M4 m4 { get; set; }
    }

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

    public class Procmastermodel
    {
        public string typecd { get; set; }
        public Shinkishutenkai shinkishutenkai { get; set; }
        public Buhinhyou buhinhyou { get; set; }
        public List<Process> process { get; set; }
    }

    public class MastermodelRoot
    {
        public Procmastermodel procmastermodel { get; set; }
    }


    ////////////////////////////////////////
    /// config.json
    ////////////////////////////////////////
    ///

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ProcjsonFolder
    {
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class Path
    {
        public string shinkishutenkai_file { get; set; }
        public string buhinhyou_folder { get; set; }
        public ProcjsonFolder procjson_folder { get; set; }
    }

    public class Defaultmodel
    {
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class Model
    {
        public string model { get; set; }
        public string typekey { get; set; }
        public string hankan { get; set; }
        public string kansei { get; set; }
    }

    public class Config
    {
        public string type { get; set; }
        public Path path { get; set; }
        public Defaultmodel defaultmodel { get; set; }
        public List<Model> models { get; set; }
    }

    public class Makeprocjson
    {
        public string typecat { get; set; }
        public List<Config> config { get; set; }
    }

    public class ConfigRoot
    {
        public List<Makeprocjson> makeprocjson { get; set; }
    }


}



