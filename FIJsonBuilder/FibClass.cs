using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace FIFJsonBuilder
{
    ////////////////////////////////////
    ///
    /// Mac_Config_Plusクラス
    ///
    ////////////////////////////////////
    ///

    public class macconfjson
    {
        public MacHeader Mchd;
        public PLCs Plcs;
        public PCs Pcs;
        public MCFs Mcfs;
        public MSTSFs Mstsfs;


        public macconfjson()
        {
            //macconfsample();
        }

        private void macconfsample()
        {
            string FilePath = "C:\\magcupdir\\DB\\M00001\\conf\\macconf.json";

            //header
            Mchd = new MacHeader();
            Mchd.mcat = "DB";
            Mchd.macno = "M00001";

            //plc vars
            Plcs = new PLCs();
            for (int num = 0; num < 3; num++)
            {
                Plcs.plcconfs.Add(new PLCconf());
                Plcs[num].bender = "Keyence";
                Plcs[num].model = "KV-8500";
                Plcs[num].name = "PLC"+num.ToString();
                Plcs[num].ipa = "192.168.4."+num.ToString();
                Plcs[num].devport = "8501";
                if (num==0)
                {
                    //Plcs[num].usemc = true;
                    //Plcs[num].usemm = true;
                }
                else
                {
                    //Plcs[num].usemc = false;
                    //Plcs[num].usemm = false;
                }

                string[] devid = new string[] { "IO-1", "IO-2", "IO-3" };
                string[] devtype = new string[] { "MR", "MR", "MR" };
                string[] devno = new string[] { "500" + (num), "500" + (num + 1), "500" + (num + 2) };
                for (int i = 0; i < devtype.Length; i++)
                {
                    Plcs[num].devs.devconfs.Add(new devconf());
                    Plcs[num].devs[i].devid = devid[i];
                    Plcs[num].devs[i].devtype = devtype[i];
                    Plcs[num].devs[i].devno = devno[i];
                }

                bool[] useftpsv = new bool[] { true };
                string[] id = new string[] { "plc" + num };
                string[] password = new string[] { "pass" + num };
                string[] port = new string[] { "22" };
                for (int i = 0; i < id.Length; i++)
                {
                    Plcs[num].ftps.ftpconfs.Add(new ftpconf());
                    Plcs[num].ftps[i].useftpsv = useftpsv[i];
                    Plcs[num].ftps[i].id = id[i];
                    Plcs[num].ftps[i].password = password[i];
                    Plcs[num].ftps[i].port = port[i];
                }
            }

            //pc vars
            Pcs = new PCs();
            for (int num = 0; num < 3; num++)
            {
                Pcs.pcconfs.Add(new PCconf());
                Pcs[num].name = "PC" + num.ToString();
                Pcs[num].ipa = "192.168.4."+num.ToString();
                if (num == 0)
                {
                    //Plcs[num].usemc = true;
                    //Plcs[num].usemm = true;
                }
                else
                {
                    //Plcs[num].usemc = false;
                    //Plcs[num].usemm = false;
                }
                string[] port = new string[] { "22" };
                string[] id = new string[] { "pc" + num };
                string[] password = new string[] { "pass" + num };
                for (int i = 0; i < id.Length; i++)
                {
                    Pcs[num].ftps.ftpconfs.Add(new ftpconf());
                    Pcs[num].ftps[i].port = port[i];
                    Pcs[num].ftps[i].id = id[i];
                    Pcs[num].ftps[i].password = password[i];
                }
                string[] name = new string[] { "share1", "share2", "share3" };
                string[] path = new string[] { "sharedir1", "sharedir2", "sharedir3" };
                for (int i = 0; i < id.Length; i++)
                {
                    Pcs[num].shfld.sfconf.Add(new shfldconf());
                    Pcs[num].shfld[i].name = name[i];
                    Pcs[num].shfld[i].path = path[i];
                }
            }

            Mcfs = new MCFs();
            List<string> mcflist = new List<string>{ "min1", "min2", "mot" };
            List<string> encs = new List<string> { "utf-8", "utf-8", "utf-8" };
            List<string> rets = new List<string> { "magno:5,product:25,lotno:10,valout", "ok,0", "ok,0" };
            List<bool> sp1 = new List<bool> { false, false, false};
            for (int i = 0; i < mcflist.Count; i++)
            {
                Mcfs.mcfconfs.Add(new MCFconf());
                Mcfs[i].mcfilekey = mcflist[i];
                Mcfs[i].encoding = encs[i];
                Mcfs[i].returns = rets[i];
                Mcfs[i].spfnc1 = sp1[i];
            }

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Encoding enc = Encoding.GetEncoding("UTF-8");
            StreamWriter writer = new StreamWriter(FilePath, false, enc);
            writer.WriteLine(json);
            writer.Close();
        }
    }


    /*******************************************/
    // MacConfig : Header情報
    /*******************************************/

    public class MacHeader
    {
        public string mcat { get; set; }
        public string macno { get; set; }
        //public bool mcuseplc { get; set; }
        //public bool mcusepc { get; set; }
    }


    /*******************************************/
    // MacConfig : PCデバイスリスト
    /*******************************************/

    public class PCs
    {
        public List<PCconf> pcconfs = new List<PCconf>();

        public PCconf this[int index]
        {
            get
            {
                return pcconfs[index];
            }
            set
            {
                pcconfs.Add(value);
            }
        }
    }

    public class PCconf
    {
        public string name { get; set; }
        public string ipa { get; set; }
        //public bool usemc { get; set; }
        //public bool usemm { get; set; }
        public ftps ftps = new ftps();
        public Shareflds shfld = new Shareflds();
    }


    /*******************************************/
    // MacConfig : MCfile Configリスト
    /*******************************************/

    public class MCFs
    {
        public List<MCFconf> mcfconfs = new List<MCFconf>();

        public MCFconf this[int index]
        {
            get
            {
                return mcfconfs[index];
            }
            set
            {
                mcfconfs.Add(value);
            }
        }
    }

    public class MCFconf
    {
        public string mcfilekey { get; set; }
        public string encoding { get; set; }
        public string returns { get; set; }
        public bool disableEndfile { get; set; }
        public bool verifiparam { get; set; }
        public bool spfnc1 { get; set; }
        public fileOwnerinfo foi = new fileOwnerinfo();
    }


    /*******************************************/
    // MacConfig : 設備情報 Configリスト
    /*******************************************/

    public class MSTSFs
    {
        public List<MSTSFconf> mstsfconfs = new List<MSTSFconf>();

        public MSTSFconf this[int index]
        {
            get
            {
                return mstsfconfs[index];
            }
            set
            {
                mstsfconfs.Add(value);
            }
        }
    }

    public class MSTSFconf
    {
        public string mstsfilekey { get; set; }
        public string mstsfileid { get; set; }
        public fileOwnerinfo foi = new fileOwnerinfo();

    }

    /*******************************************/
    // MacConfig : PLCリスト
    /*******************************************/

    public class PLCs
    {
        public List<PLCconf> plcconfs = new List<PLCconf>();

        public PLCconf this[int index]
        {
            get
            {
                return plcconfs[index];
            }
            set
            {
                plcconfs.Add(value);
            }
        }
    }

    public class PLCconf
    {
        public string bender { get; set; }
        public string model { get; set; }
        public string name { get; set; }
        public string ipa { get; set; }
        public string devport { get; set; }
        //public bool usemc { get; set; }
        //public bool usemm { get; set; }
        public devs devs = new devs();
        public ftps ftps = new ftps();
        public Shareflds shfld = new Shareflds();
    }


    /*******************************************/
    // MacConfig : PLCデバイスリスト
    /*******************************************/

    public class devs
    {
        public List<devconf> devconfs = new List<devconf>();

        public devconf this[int index]
        {
            get
            {
                return devconfs[index];
            }
            set
            {
                devconfs.Add(value);
            }
        }
    }

    public class devconf
    {
        public string devid { get; set; }
        public string devtype { get; set; }
        public string devno { get; set; }
    }


    /*******************************************/
    // MacConfig : FTPリスト
    /*******************************************/

    public class ftps
    {
        public List<ftpconf> ftpconfs = new List<ftpconf>();

        public ftpconf this[int index]
        {
            get
            {
                return ftpconfs[index];
            }
            set
            {
                ftpconfs.Add(value);
            }
        }
    }

    public class ftpconf
    {
        public bool useftpsv { get; set; }
        public string port { get; set; }
        public string id { get; set; }
        public string password { get; set; }
        public string homedir { get; set; }
    }


    /*******************************************/
    // MacConfig : 共有フォルダリスト
    /*******************************************/

    public class Shareflds
    {
        public List<shfldconf> sfconf = new List<shfldconf>();

        public shfldconf this[int index]
        {
            get
            {
                return sfconf[index];
            }
            set
            {
                sfconf.Add(value);
            }
        }
    }

    public class shfldconf
    {
        public string name { get; set; }
        public string path { get; set; }
    }



    /*******************************************/
    // fileOwnerinfo : 共有フォルダリスト
    /*******************************************/

    public class fileOwnerinfo
    {
        public bool serverpull { get; set; }
        public string cnttype { get; set; }
        public string cntid { get; set; }
        public bool useplcdev { get; set; }
        public string devid { get; set; }
        public string pulltype { get; set; }
        public string minttimefetch { get; set; }
        public string shfld { get; set; }
        public string path { get; set; }
        public string inttimefetch { get; set; }
    }

}
