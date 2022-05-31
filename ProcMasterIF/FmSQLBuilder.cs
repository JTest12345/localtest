using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using SpreadsheetLight;


namespace ProcMasterIF
{
    public partial class FmSQLBuilder : Form
    {
        public FmSQLBuilder()
        {
            string crlf = "\r\n";
            string msg;
            string workingdir = @"C:\Users\jn-wtnb\Desktop\マスタ管理アイデア\共通台帳";
            KouteiTigiRoot conf;

            InitializeComponent();

            var yamlPath = workingdir + @"\工程定義台帳.yaml";
            var xlsPath = workingdir + @"\工程定義台帳.xlsm";
            var kouteiteigi_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            conf = deserializer.Deserialize<KouteiTigiRoot>(kouteiteigi_yaml);

            var sheetsnum = conf.header.sheets.Count();
            SLDocument[] sl = new SLDocument[sheetsnum];

            for (int i=0; i< sheetsnum; i++)
            {
                sl[i] = new SLDocument(xlsPath, conf.header.sheets[i]);
            }
        }
    }
}
