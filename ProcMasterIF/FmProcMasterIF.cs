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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Oskas;

namespace ProcMasterIF
{
    public partial class FmProcMasterIF : Form
    {
        string crlf = "\r\n";
        string msg;
        string workingdir = @"C:\Oskas\procmaster\shomei\ver9";
        MakeprocjsonRoot conf;
        SLDocument sl;
        public bool interLock = false;
        SeriesTypeMaster sr;


        public FmProcMasterIF()
        {
            InitializeComponent();

            // cojmakeprocfile
            var yamlPath = workingdir + @"\cojmakeprocfile.yaml";
            var cojmakeprocfile_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            conf = deserializer.Deserialize<MakeprocjsonRoot>(cojmakeprocfile_yaml);

            //// (とりあえず)表示処理
            txt_shinkishutenkai.Text = conf.makeprocjson.config.path.shinkishutenkaifile;
            txt_buhinhyou.Text = conf.makeprocjson.config.path.buhinhyoufolder;
            txt_procjson_hankan.Text = conf.makeprocjson.config.path.procjsonfolder.hankan;
            txt_procjson_kansei.Text = conf.makeprocjson.config.path.procjsonfolder.kansei;


            /////////////////////////
            ///ROOT
            /////////////////////////
            ///
            var rootmodelfldpath = workingdir + @"\model\root";

            // 新機種展開表
            yamlPath = rootmodelfldpath + @"\shinkishutenkai.yaml";
            var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            var shinkishutenai_obj = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);


            // 部品表
            yamlPath = rootmodelfldpath + @"\buhinhyou.yaml";
            var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var buhinhyou_obj = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);


            // 工程定義
            yamlPath = rootmodelfldpath + @"\processlist.yaml";
            var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);
            var process_dict = new Dictionary<string, Process>();


            foreach (var proc in processlist_obj)
            {
                yamlPath = rootmodelfldpath + @"\process\" + proc + ".yaml";
                var proc_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                deserializer = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();
                var proc_obj = deserializer.Deserialize<Process>(proc_yaml);
                process_dict[proc] = proc_obj;
            }

            // シリーズルート
            sr = new SeriesTypeMaster();
            sr.shinkishutenkai = shinkishutenai_obj;
            sr.buhinhyou = buhinhyou_obj;
            sr.processdict = process_dict;

            numericUpDown1Set(1);
            numericUpDown2Set(1);
        }


        delegate void ConsoleDelegate(string text, int level);
        private void ConsoleShow(string text, int level)
        {
            if (consoleBox.InvokeRequired)
            {
                ConsoleDelegate d = new ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] { text, level });
            }
            else
            {
                string message = "";
                switch (level)
                {
                    case 1:
                        message = "[info] ";
                        break;
                    case 2:
                        message = "[Warn] ";
                        break;
                    case 3:
                        message = "[ERROR] ";
                        break;
                }
                consoleBox.AppendText(message + text + crlf);
            }
        }


        delegate void ToolStripStatusDelegate(string text);
        private void ToolStripStatusShow(string text)
        {
            if (consoleBox.InvokeRequired)
            {
                ToolStripStatusDelegate d = new ToolStripStatusDelegate(ToolStripStatusShow);
                BeginInvoke(d, new object[] { text });
            }
            else
            {
                toolStripStatusLabel1.Text = text;
            }
        }


        delegate void numericUpDown1Delegate(int value);
        private void numericUpDown1Set(int value)
        {
            if (numericUpDown1.InvokeRequired)
            {
                numericUpDown1Delegate d = new numericUpDown1Delegate(numericUpDown1Set);
                BeginInvoke(d, new object[] { value });
            }
            else
            {
                numericUpDown1.Value = value;
            }
        }


        delegate void numericUpDown2Delegate(int value);
        private void numericUpDown2Set(int value)
        {
            if (numericUpDown2.InvokeRequired)
            {
                numericUpDown2Delegate d = new numericUpDown2Delegate(numericUpDown2Set);
                BeginInvoke(d, new object[] { value });
            }
            else
            {
                numericUpDown2.Value = value;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (!interLock)
            {
                interLock = true;

                var cols = conf.makeprocjson.config.model[0].shinkishutenkaicol[0].ToString().Split('/');
                numericUpDown1.Value = int.Parse(cols[0].Replace(" ", ""));
                numericUpDown2.Value = int.Parse(cols[1].Replace(" ", ""));

                Task readHankan = Task.Run(() =>
                {
                    /////////////////////////////////
                    /// 半完ルートからマスタ作成
                    /////////////////////////////////
                    var orthankanfld = workingdir + @"\model\sources\" + conf.makeprocjson.config.model[0].folder + @"\" + conf.makeprocjson.config.model[0].hankan;
                    var orthankan = new Procmastermodel();

                    // 新機種展開表をルートからコピー
                    orthankan.shinkishutenkai = sr.shinkishutenkai;

                    // 部品表をルートからコピー
                    orthankan.buhinhyou = sr.buhinhyou;

                    // 工程順
                    var yamlPath = orthankanfld + @"\processorder.yaml";
                    var processorder_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    var deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                    orthankan.processorder = deserializer.Deserialize<List<string>>(processorder_yaml);

                    // 工程定義をルートの工程辞書から工程順リストを元に取得
                    orthankan.process = new List<Process>();
                    foreach (var proc in orthankan.processorder)
                    {
                        orthankan.process.Add(sr.processdict[proc]);
                    }

                    // オーバーライド処理
                    // 新機種展開表
                    yamlPath = orthankanfld + @"\shinkishutenkai.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        orthankan.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                    }

                    yamlPath = orthankanfld + @"\buhinhyou.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        orthankan.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                    }

                    /////////////////////////////////
                    /// 完成ルートからマスタ作成
                    /////////////////////////////////
                    var ortkanseifld = workingdir + @"\model\sources\" + conf.makeprocjson.config.model[0].folder + @"\" + conf.makeprocjson.config.model[0].kansei;
                    var ortkansei = new Procmastermodel();

                    // 新機種展開表をルートからコピー
                    ortkansei.shinkishutenkai = sr.shinkishutenkai;

                    // 部品表をルートからコピー
                    ortkansei.buhinhyou = sr.buhinhyou;

                    // 工程順
                    yamlPath = ortkanseifld + @"\processorder.yaml";
                    processorder_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                    ortkansei.processorder = deserializer.Deserialize<List<string>>(processorder_yaml);

                    // 工程定義をルートの工程辞書から工程順リストを元に取得
                    ortkansei.process = new List<Process>();
                    foreach (var proc in ortkansei.processorder)
                    {
                        ortkansei.process.Add(sr.processdict[proc]);
                    }

                    // オーバーライド処理
                    // 新機種展開表
                    yamlPath = ortkanseifld + @"\shinkishutenkai.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        ortkansei.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                    }

                    yamlPath = ortkanseifld + @"\buhinhyou.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        ortkansei.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                    }

                    ///////////////////////////
                    ///make procjson
                    ///////////////////////////
                    for (var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++)
                    {
                        var procobj_hankan = new MastermodelRoot();
                        var procobj_kansei = new MastermodelRoot();

                        var hankan = sl.GetCellValueAsString("A" + i);
                        var kansei = sl.GetCellValueAsString("B" + i);

                        ConsoleShow("hankan-" + i + "=" + hankan, 1);
                        ConsoleShow("kansei-" + i + "=" + kansei, 1);

                        ////////////////////////////////
                        // 半完マスタ
                        ////////////////////////////////
                        ///
                        
                        ortkansei = new Procmastermodel();

                        // 新機種展開表をルートからコピー
                        ortkansei.shinkishutenkai = sr.shinkishutenkai.DeepClone(); ;

                        // 部品表をルートからコピー
                        ortkansei.buhinhyou = sr.buhinhyou.DeepClone(); ;

                        // 工程順
                        yamlPath = orthankanfld + @"\processorder.yaml";
                        processorder_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                        ortkansei.processorder = deserializer.Deserialize<List<string>>(processorder_yaml);

                        // 工程定義をルートの工程辞書から工程順リストを元に取得
                        ortkansei.process = new List<Process>();
                        foreach (var proc in ortkansei.processorder)
                        {
                            ortkansei.process.Add(sr.processdict[proc]);
                        }

                        // オーバーライド処理
                        // 新機種展開表
                        yamlPath = orthankanfld + @"\shinkishutenkai.yaml";
                        if (CommonFuncs.FileExists(yamlPath))
                        {
                            var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                               .Build();
                            ortkansei.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                        }

                        yamlPath = orthankanfld + @"\buhinhyou.yaml";
                        if (CommonFuncs.FileExists(yamlPath))
                        {
                            var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                               .Build();
                            ortkansei.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                        }

                        procobj_hankan.procmastermodel = ortkansei;

                        // ProcJson作成
                        if (!makeProcJson(i, ref procobj_hankan))
                        {
                            interLock = false;
                            return;
                        }


                        ////////////////////////////////
                        // 完成品マスタ
                        ////////////////////////////////
                        ///

                        ortkansei = new Procmastermodel();

                        // 新機種展開表をルートからコピー
                        ortkansei.shinkishutenkai = sr.shinkishutenkai.DeepClone(); ;

                        // 部品表をルートからコピー
                        ortkansei.buhinhyou = sr.buhinhyou.DeepClone(); ;

                        // 工程順
                        yamlPath = ortkanseifld + @"\processorder.yaml";
                        processorder_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                        ortkansei.processorder = deserializer.Deserialize<List<string>>(processorder_yaml);

                        // 工程定義をルートの工程辞書から工程順リストを元に取得
                        ortkansei.process = new List<Process>();
                        foreach (var proc in ortkansei.processorder)
                        {
                            ortkansei.process.Add(sr.processdict[proc]);
                        }

                        // オーバーライド処理
                        // 新機種展開表
                        yamlPath = ortkanseifld + @"\shinkishutenkai.yaml";
                        if (CommonFuncs.FileExists(yamlPath))
                        {
                            var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                               .Build();
                            ortkansei.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                        }

                        yamlPath = ortkanseifld + @"\buhinhyou.yaml";
                        if (CommonFuncs.FileExists(yamlPath))
                        {
                            var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                               .Build();
                            ortkansei.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                        }

                        procobj_kansei.procmastermodel = ortkansei;

                        if (!makeProcJson(i, ref procobj_kansei))
                        {
                            interLock = false;
                            return;
                        }


                        /////////////////////////////////
                        // Jsonファイルに書き出し
                        /////////////////////////////////
                        ///
                        procobj_hankan.procmastermodel.typecd = procobj_hankan.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
                        procobj_kansei.procmastermodel.typecd = procobj_kansei.procmastermodel.shinkishutenkai.typeinfo.kansei.value;
                                               
                        var procjsonfolder_hankan = conf.makeprocjson.config.path.procjsonfolder.hankan;
                        var procjsonfolder_kansei = conf.makeprocjson.config.path.procjsonfolder.kansei;

                        if (!string.IsNullOrEmpty(procjsonfolder_hankan))
                        {
                            var jsonpath = procjsonfolder_hankan + "\\" + procobj_hankan.procmastermodel.typecd + ".json";
                            if (!System.IO.File.Exists(jsonpath))
                            {
                                Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);
                            }
                            //Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);
                        }

                        if (!string.IsNullOrEmpty(procjsonfolder_kansei))
                        {
                            var jsonpath = procjsonfolder_kansei + "\\" + procobj_kansei.procmastermodel.typecd + ".json";
                            //if (!System.IO.File.Exists(jsonpath) )
                            //{
                            //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj, ref msg);
                            //}
                            Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_kansei, ref msg);
                        }
                    }
                    ToolStripStatusShow("");
                    interLock = false;
                });
            }
        }


        private void FmProcMasterIF_Shown(object sender, EventArgs e)
        {

            Task readHankan = Task.Run(() =>
            {
                ToolStripStatusShow("◆新機種展開表を読み込んでいます");
                sl = new SLDocument(txt_shinkishutenkai.Text);
                ToolStripStatusShow("読込完了");
            });
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (sl != null)
            {
                txt_typecd_kansei_start.Text = sl.GetCellValueAsString("B" + numericUpDown1.Value);
            }
        }


        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (sl != null)
            {
                txt_typecd_kansei_end.Text = sl.GetCellValueAsString("B" + numericUpDown2.Value);
            }
        }


        private bool readJson(string jsonpath, ref string json)
        {
            var msg = string.Empty;
            if (!Oskas.CommonFuncs.JsonFileReader(jsonpath, ref json, ref msg))
            {
                ConsoleShow(jsonpath + "を読み込み失敗", 1);
                return false;
            }
            ConsoleShow(jsonpath + "を読み込み完了", 1);
            return true;
        }


        private bool makeProcJson(decimal row, ref MastermodelRoot procobj)
        {
            //var jsonstr = string.Empty;

            //if (!readJson(jsonmodelpath, ref jsonstr))
            //{
            //    procobj = null;
            //    return false;
            //}

            //procobj = JsonConvert.DeserializeObject<MastermodelRoot>(jsonstr);

            //////////////////////
            //新機種展開表を読込
            //////////////////////
            ///
            var skt = procobj.procmastermodel.shinkishutenkai;
            
            //formo
            if (skt.formno.value == "")
            {
                skt.formno.value = sl.GetCellValueAsString(skt.formno.xlsaddress);
            }

            //released
            if (skt.released.value == "")
            {
                var dt = Double.Parse(sl.GetCellValueAsString(skt.released.xlsaddress));
                skt.released.value = DateTime.FromOADate(dt).ToString("d");
            }

            //updateat
            if (skt.updateat.value == "")
            {
                var dt = Double.Parse(sl.GetCellValueAsString(skt.updateat.xlsaddress));
                skt.updateat.value = DateTime.FromOADate(dt).ToString("d");
            }

            //revision
            if (skt.revision.value == "")
            {
                skt.revision.value = sl.GetCellValueAsString(skt.revision.xlsaddress);
            }

            //typeinfo.hankan
            if (skt.typeinfo.hankan.value == "")
            {
                skt.typeinfo.hankan.value = sl.GetCellValueAsString(skt.typeinfo.hankan.xlscol + row);
            }

            //typeinfo.kansei
            if (skt.typeinfo.kansei.value == "")
            {
                skt.typeinfo.kansei.value = sl.GetCellValueAsString(skt.typeinfo.kansei.xlscol + row);
            }

            //typeinfo.kyakusaki
            if (skt.typeinfo.kyakusaki.value == "")
            {
                skt.typeinfo.kyakusaki.value = sl.GetCellValueAsString(skt.typeinfo.kyakusaki.xlscol + row);
            }

            //lotinfo.torikosu
            if (skt.lotinfo.torikosu.value == "")
            {
                skt.lotinfo.torikosu.value = sl.GetCellValueAsString(skt.lotinfo.torikosu.xlscol + row);
            }

            //lotinfo.maisu_lot
            if (skt.lotinfo.lotmaisu.value == "")
            {
                skt.lotinfo.lotmaisu.value = sl.GetCellValueAsString(skt.lotinfo.lotmaisu.xlscol + row);
            }

            //lotinfo.pkg_lot
            if (skt.lotinfo.lotpics.value == "")
            {
                skt.lotinfo.lotpics.value = sl.GetCellValueAsString(skt.lotinfo.lotpics.xlscol + row);
            }

            //etc.buhinhyou.zuban.jp
            if (skt.etc.buhinhyou.zuban.jp.value == "")
            {
                skt.etc.buhinhyou.zuban.jp.value = sl.GetCellValueAsString(skt.etc.buhinhyou.zuban.jp.xlscol + row);
            }

            //etc.buhinhyou.zuban.ch
            if (skt.etc.buhinhyou.zuban.ch.value == "")
            {
                skt.etc.buhinhyou.zuban.ch.value = sl.GetCellValueAsString(skt.etc.buhinhyou.zuban.ch.xlscol + row);
            }
            
            // 部品表を検出する
            var bompath = GetBomPath(skt.etc.buhinhyou.zuban.jp.value + "@" + skt.typeinfo.kansei.value + ".xlsx");

            if (bompath == "")
            {
                ConsoleShow(skt.etc.buhinhyou.zuban.jp.value + "@" + skt.typeinfo.kansei.value + ".xlsx は対検索対象フォルダから検出できませんでした", 2);
                ConsoleShow("中止します", 2);
                return false;
            }

            // 部品表を読み込む
            var slbom = new SLDocument(bompath);


            // process(工程)にmaterial(部品)を割り付ける
            var procs = procobj.procmastermodel.process;

            foreach (var proc in procs)
            {
                foreach (var material in proc.material.m4)
                {
                    if (material.name.value == "")
                    {
                        material.name.value = slbom.GetCellValueAsString(material.name.bomaddress);
                    }
                    else
                    {
                        if (material.name.value != slbom.GetCellValueAsString(material.name.bomaddress))
                        {
                            ConsoleShow("部品表から取得した部品コードが基底モデルと違っています", 2);
                            ConsoleShow("部品表：" + slbom.GetCellValueAsString(material.name.bomaddress), 2);
                            ConsoleShow("基底モデル：" + material.name.value, 2);
                            return false;
                        }
                    }

                    if (material.code.value == "")
                    {
                        material.code.value = slbom.GetCellValueAsString(material.code.bomaddress);
                    }
                    else
                    {
                        if (material.code.value != slbom.GetCellValueAsString(material.code.bomaddress))
                        {
                            ConsoleShow("部品表から取得した部品コードが基底モデルと違っています", 2);
                            ConsoleShow("部品表：" + slbom.GetCellValueAsString(material.code.bomaddress), 2);
                            ConsoleShow("基底モデル：" + material.code.value, 2);
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        private string GetBomPath(string bomno)
        {
            var serchDir = txt_buhinhyou.Text;
            foreach (string d in Directory.GetDirectories(serchDir))
            {
                var fileName = d + "\\" + bomno;
                if (System.IO.File.Exists(fileName))
                {
                    ConsoleShow("'" + fileName + "'は存在します。", 1);
                    return fileName;
                }
            }

            return "";
        }


        private void btn_makeRootModel_Click(object sender, EventArgs e)
        {
            var rootmodelfldpath = @"C:\Oskas\procmaster\model\shoumei\ver9\root";

            // 新機種展開表
            var yamlPath = rootmodelfldpath + @"\shinkishutenkai.yaml";
            var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            var shinkishutenai_obj = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);


            // 部品表
            yamlPath = rootmodelfldpath + @"\buhinhyou.yaml";
            var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var buhinhyou_obj = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);


            // 工程定義
            yamlPath = rootmodelfldpath + @"\processlist.yaml";
            var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);
            var process_dict = new Dictionary<string, Process>();


            foreach (var proc in processlist_obj)
            {
                yamlPath = rootmodelfldpath + @"\process\" + proc + ".yaml";
                var proc_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                deserializer = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();
                var proc_obj = deserializer.Deserialize<Process>(proc_yaml);
                process_dict[proc] = proc_obj;
            }

            // シリーズルート
            var sr = new SeriesTypeMaster();
            sr.shinkishutenkai = shinkishutenai_obj;
            sr.buhinhyou = buhinhyou_obj;
            sr.processdict = process_dict;

            ////////////////////////////////
            //　オーバーライドテスト
            ////////////////////////////////
            ///
            var ortfld = @"C:\Oskas\procmaster\model\shoumei\ver9\modelsources\ver9_hankan_as0309";
            var ort = new Procmastermodel();
            
            // 新機種展開表をルートからコピー
            ort.shinkishutenkai = sr.shinkishutenkai;

            // 部品表をルートからコピー
            ort.buhinhyou = sr.buhinhyou;

            // 工程順
            yamlPath = ortfld + @"\processorder.yaml";
            var processorder_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            ort.processorder = deserializer.Deserialize<List<string>>(processorder_yaml);

            // 工程定義をルートの工程辞書から工程順リストを元に取得
            ort.process = new List<Process>();
            foreach (var proc in ort.processorder)
            {
                ort.process.Add(sr.processdict[proc]);
            }

            // オーバーライド処理
            // 新機種展開表
            yamlPath = ortfld + @"\shinkishutenkai.yaml";
            if (CommonFuncs.FileExists(yamlPath))
            {
                shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                ort.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
            }

            yamlPath = ortfld + @"\buhinhyou.yaml";
            if (CommonFuncs.FileExists(yamlPath))
            {
                buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                ort.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
            }
        }
    }

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
