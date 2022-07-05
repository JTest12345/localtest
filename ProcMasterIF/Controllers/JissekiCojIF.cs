using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Oskas;
using System.Text.RegularExpressions;
using ExcelDataReader;

namespace ProcMasterIF
{
    class JissekiCojIF
    {
        string workingdir = @"C:\Oskas\procmaster\shomei\ver9";
        SeriesTypeMaster sr;
        //新機種展開表オブジェクト
        //SLDocument sl; //SpreadSheetLight
        System.Data.DataTable sl; //ExcelDataReader

        MakeprocjsonRoot conf;
        //List<int> colList;
        List<string> cmdlist_jisseki_obj;
        List<SeriesTypeMaster> srList;
        string msg;
        string crlf = "\r\n";


        public JissekiCojIF()
        {
            // cojmakeprocfile
            var yamlPath = workingdir + @"\cojmakeprocfile.yaml";
            var cojmakeprocfile_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            conf = deserializer.Deserialize<MakeprocjsonRoot>(cojmakeprocfile_yaml);

            //// 表示処理
            msg = crlf;
            msg += conf.makeprocjson.config.path.shinkishutenkaifile + crlf;
            msg += conf.makeprocjson.config.path.buhinhyoufolder + crlf;
            msg += conf.makeprocjson.config.path.procjsonfolder.hankan + crlf;
            msg += conf.makeprocjson.config.path.procjsonfolder.kansei + crlf;
            OskNLog.Log(msg, Cnslcnf.msg_info);


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

            // コマンドリスト
            yamlPath = rootmodelfldpath + @"\cmdlist_jisseki.yaml";
            var cmdlist_jisseki_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            cmdlist_jisseki_obj = deserializer.Deserialize<List<string>>(cmdlist_jisseki_yaml);

            // 工程定義
            yamlPath = rootmodelfldpath + @"\processlist.yaml";
            var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);

            // 工程詳細辞書
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
            srList = new List<SeriesTypeMaster>();
            foreach (var model in conf.makeprocjson.config.model)
            {
                sr = new SeriesTypeMaster();
                sr.shinkishutenkai = shinkishutenai_obj;
                sr.buhinhyou = buhinhyou_obj;
                sr.processdict = process_dict;
                sr.model = model;

                sr.collist = new List<int>();
                foreach (var colstr in model.shinkishutenkaicol)
                {
                    var cols = colstr.ToString().Split('/');
                    if (cols.Length > 1)
                    {
                        int startColNum, stopColNum;
                        if (!int.TryParse(cols[0].Replace(" ", ""), out startColNum))
                        {
                            OskNLog.Log("新機種展開のCOL指定が不正です", Cnslcnf.msg_error);
                            return;
                        }
                        if (!int.TryParse(cols[1].Replace(" ", ""), out stopColNum))
                        {
                            OskNLog.Log("新機種展開のCOL指定が不正です", Cnslcnf.msg_error);
                            return;
                        }
                        for (int i = startColNum; i < stopColNum + 1; i++)
                        {
                            sr.collist.Add(i);
                        }
                    }
                    else
                    {
                        int colNum;
                        if (!int.TryParse(colstr.ToString().Replace(" ", ""), out colNum))
                        {
                            OskNLog.Log("新機種展開のCOL指定が不正です", Cnslcnf.msg_error);
                            return;
                        }
                        sr.collist.Add(colNum);
                    }
                }
                srList.Add(sr);
            }
            
            OskNLog.Log("makeを開始しました。", Cnslcnf.msg_info);
        }


        public bool Excute()
        {
            /////////////////////////////////
            /// 新機種展開表読込
            /////////////////////////////////
            OskNLog.Log("新機種展開表を読込開始", Cnslcnf.msg_info);
            // SpreadSheetlightで読込
            // sl = new SLDocument(conf.makeprocjson.config.path.shinkishutenkaifile);
            // ExcelDataReaderで読込
            if ( !string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.shinkishutenkaifile, "一覧", ref sl)))
            {
                OskNLog.Log("新機種展開表を読込失敗", Cnslcnf.msg_error);
                return false;
            }

            OskNLog.Log("新機種展開表を読込完了", Cnslcnf.msg_info);

            foreach (var srl in srList)
            {
                /////////////////////////////////
                /// シリーズフォルダ
                /////////////////////////////////
                var orthankanfld = workingdir + @"\model\sources\" + srl.model.folder + @"\" + srl.model.hankan;
                var ortkanseifld = workingdir + @"\model\sources\" + srl.model.folder + @"\" + srl.model.kansei;

                /////////////////////////////////
                /// make procjson
                /////////////////////////////////
                foreach (var col in srl.collist)
                {
                    var procobj_hankan = new MastermodelRoot();
                    var procobj_kansei = new MastermodelRoot();

                    //var hankan = GetCellValueAsString(sl, "A" + col);
                    //var kansei = GetCellValueAsString(sl, "B" + col);

                    //OskNLog.Log("hankan-" + col + "=" + hankan, 1);
                    //OskNLog.Log("kansei-" + col + "=" + kansei, 1);

                    ////////////////////////////////
                    // 半完マスタ
                    ////////////////////////////////
                    var orthankan = new Procmastermodel();

                    // 新機種展開表をルートからコピー
                    orthankan.shinkishutenkai = srl.shinkishutenkai.DeepClone(); ;

                    // 部品表をルートからコピー
                    orthankan.buhinhyou = srl.buhinhyou.DeepClone(); ;

                    // 工程辞書をルートからコピー
                    var processdict_hankan = srl.processdict.DeepClone();

                    // 品目マスタ―
                    var yamlPath = orthankanfld + @"\hinmokumaster.yaml";
                    var hinmokumaster_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    var deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                    orthankan.hinmokumaster = deserializer.Deserialize<List<HINMOKUMASTER>>(hinmokumaster_yaml);


                    //【オーバーライド処理】新機種展開表
                    yamlPath = orthankanfld + @"\shinkishutenkai.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        orthankan.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                    }

                    //【オーバーライド処理】部品表
                    yamlPath = orthankanfld + @"\buhinhyou.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        orthankan.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                    }

                    //【オーバーライド処理】コマンド
                    yamlPath = orthankanfld + @"\cmdlist_jisseki.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var cmdlist_jisseki_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        cmdlist_jisseki_obj = deserializer.Deserialize<List<string>>(cmdlist_jisseki_yaml);
                    }

                    //【オーバーライド処理】工程定義
                    yamlPath = orthankanfld + @"\processlist.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                        var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);

                        // 工程詳細辞書
                        var process_dict = new Dictionary<string, Process>();
                        foreach (var proc in processlist_obj)
                        {
                            if (processdict_hankan.ContainsKey(proc))
                            {
                                if (processdict_hankan.Remove(key: proc) == true)
                                {
                                    OskNLog.Log("工程オーバーライド：" + proc, Cnslcnf.msg_info);
                                }
                                else
                                {
                                    OskNLog.Log("工程オーバーライドが失敗：" + proc, Cnslcnf.msg_info);
                                    return false;
                                }
                            }
                            yamlPath = orthankanfld + @"\process_override\" + proc + ".yaml";
                            var proc_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                            .Build();
                            var proc_obj = deserializer.Deserialize<Process>(proc_yaml);
                            processdict_hankan[proc] = proc_obj;
                        }
                    }

                    // 工程詳細辞書
                    orthankan.process = new List<Process>();
                    foreach (var obj in orthankan.hinmokumaster)
                    {
                        foreach (var proc in obj.kouteisagyoubango)
                        {
                            orthankan.process.Add(processdict_hankan[proc.sagyobango]);
                        }
                    }

                    //**************************
                    // ProcJson作成
                    //**************************
                    procobj_hankan.procmastermodel = orthankan;
                    if (!makeProcJson(col, ref procobj_hankan))
                    {
                        return false;
                    }

                    //**************************
                    // Coj_Jisseki作成
                    //**************************
                    var coj_jisseki_hankan = new CojMst000001();
                    var update_hankan = false;
                    if (!makeJissekiCoj(procobj_hankan, false, update_hankan, ref coj_jisseki_hankan))
                    {
                        return false;
                    }

                    //**************************
                    // Coj_4M作成
                    //**************************
                    var coj_4m_hankan = new CojMst000002();
                    var update_4m_hankan = false;
                    if (!make4mCoj(procobj_hankan, false, update_4m_hankan, ref coj_4m_hankan))
                    {
                        return false;
                    }

                    ////////////////////////////////
                    // 完成品マスタ
                    ////////////////////////////////
                    var ortkansei = new Procmastermodel();

                    // 新機種展開表をルートからコピー
                    ortkansei.shinkishutenkai = srl.shinkishutenkai.DeepClone();

                    // 部品表をルートからコピー
                    ortkansei.buhinhyou = srl.buhinhyou.DeepClone();

                    // 工程辞書をルートからコピー
                    var processdict_kansei = srl.processdict.DeepClone();

                    // 品目マスタ―
                    yamlPath = ortkanseifld + @"\hinmokumaster.yaml";
                    hinmokumaster_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                    ortkansei.hinmokumaster = deserializer.Deserialize<List<HINMOKUMASTER>>(hinmokumaster_yaml);

                    //【オーバーライド処理】新機種展開表
                    yamlPath = ortkanseifld + @"\shinkishutenkai.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        ortkansei.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                    }

                    //【オーバーライド処理】部品表
                    yamlPath = ortkanseifld + @"\buhinhyou.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        ortkansei.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                    }

                    //【オーバーライド処理】コマンド
                    yamlPath = ortkanseifld + @"\cmdlist_jisseki.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var cmdlist_jisseki_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                           .Build();
                        cmdlist_jisseki_obj = deserializer.Deserialize<List<string>>(cmdlist_jisseki_yaml);
                    }

                    //【オーバーライド処理】工程定義
                    yamlPath = ortkanseifld + @"\processlist.yaml";
                    if (CommonFuncs.FileExists(yamlPath))
                    {
                        var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                        var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);

                        // 工程詳細辞書
                        var process_dict = new Dictionary<string, Process>();
                        foreach (var proc in processlist_obj)
                        {
                            if (processdict_kansei.ContainsKey(proc))
                            {
                                if (processdict_kansei.Remove(key: proc) == true)
                                {
                                    OskNLog.Log("工程オーバーライド：" + proc , Cnslcnf.msg_info);
                                }
                                else
                                {
                                    OskNLog.Log("工程オーバーライドが失敗：" + proc, Cnslcnf.msg_info);
                                    return false;
                                }
                            }
                            yamlPath = ortkanseifld + @"\process_override\" + proc + ".yaml";
                            var proc_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                            deserializer = new DeserializerBuilder()
                                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                            .Build();
                            var proc_obj = deserializer.Deserialize<Process>(proc_yaml);
                            processdict_kansei[proc] = proc_obj;
                        }
                    }

                    // 工程詳細辞書
                    ortkansei.process = new List<Process>();
                    foreach (var obj in ortkansei.hinmokumaster)
                    {
                        if (obj.kouteisagyoubango != null)
                        {
                            foreach (var proc in obj.kouteisagyoubango)
                            {
                                if (proc != null)
                                {
                                    ortkansei.process.Add(processdict_kansei[proc.sagyobango]);
                                }
                            }
                        }
                    }

                    //**************************
                    // ProcJson作成
                    //**************************
                    procobj_kansei.procmastermodel = ortkansei;
                    if (!makeProcJson(col, ref procobj_kansei))
                    {
                        return false;
                    }

                    //**************************
                    // Coj_Jisseki作成
                    //**************************
                    var coj_jisseki_kansei = new CojMst000001();
                    var update_kansei = false;
                    if (!makeJissekiCoj(procobj_kansei, true, update_kansei, ref coj_jisseki_kansei))
                    {
                        return false;
                    }

                    //**************************
                    // Coj_4M作成
                    //**************************
                    var coj_4m_kansei = new CojMst000002();
                    var update_4m_kansei = false;
                    if (!make4mCoj(procobj_kansei, false, update_4m_kansei, ref coj_4m_kansei))
                    {
                        return false;
                    }

                    /////////////////////////////////
                    // Jsonファイルに書き出し
                    /////////////////////////////////
                    procobj_hankan.procmastermodel.typecd = procobj_hankan.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
                    procobj_kansei.procmastermodel.typecd = procobj_kansei.procmastermodel.shinkishutenkai.typeinfo.kansei.value;

                    var procjsonfolder_hankan = conf.makeprocjson.config.path.procjsonfolder.hankan;
                    var procjsonfolder_kansei = conf.makeprocjson.config.path.procjsonfolder.kansei;

                    var cojfolder_jisseki_hankan = conf.makeprocjson.config.path.cojfolder.jissekifolder.hankan;
                    var cojfolder_jisseki_kansei = conf.makeprocjson.config.path.cojfolder.jissekifolder.kansei;

                    var cojfolder_4m_hankan = conf.makeprocjson.config.path.cojfolder.m4folder.hankan;
                    var cojfolder_4m_kansei = conf.makeprocjson.config.path.cojfolder.m4folder.kansei;

                    if (!string.IsNullOrEmpty(procjsonfolder_hankan))
                    {
                        // Procmaster
                        var jsonpath = procjsonfolder_hankan + "\\" + procobj_hankan.procmastermodel.typecd + ".json";
                        //if (!System.IO.File.Exists(jsonpath))
                        //{
                        //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);
                        //}
                        CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);

                        // JissekiCoj
                        jsonpath = cojfolder_jisseki_hankan + "\\" + procobj_hankan.procmastermodel.typecd + ".jcoj";
                        CommonFuncs.JsonFileWriter(jsonpath, coj_jisseki_hankan, ref msg);

                        // 4mCoj
                        jsonpath = cojfolder_4m_hankan + "\\" + procobj_hankan.procmastermodel.typecd + ".mcoj";
                        CommonFuncs.JsonFileWriter(jsonpath, coj_4m_hankan, ref msg);
                    }

                    if (!string.IsNullOrEmpty(procjsonfolder_kansei))
                    {
                        var jsonpath = procjsonfolder_kansei + "\\" + procobj_kansei.procmastermodel.typecd + ".json";
                        //if (!System.IO.File.Exists(jsonpath) )
                        //{
                        //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj, ref msg);
                        //}
                        CommonFuncs.JsonFileWriter(jsonpath, procobj_kansei, ref msg);

                        // JissekiCoj
                        jsonpath = cojfolder_jisseki_kansei + "\\" + procobj_kansei.procmastermodel.typecd + ".jcoj";
                        CommonFuncs.JsonFileWriter(jsonpath, coj_jisseki_kansei, ref msg);

                        // 4mCoj
                        jsonpath = cojfolder_4m_kansei + "\\" + procobj_kansei.procmastermodel.typecd + ".mcoj";
                        CommonFuncs.JsonFileWriter(jsonpath, coj_4m_kansei, ref msg);
                    }
                }

                msg = "makeを完了しました。";
                OskNLog.Log(msg, Cnslcnf.msg_info);
            }

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
            var skt = procobj.procmastermodel.shinkishutenkai;

            //formo
            if (skt.formno.value == "")
            {
                skt.formno.value = GetCellValueAsString(sl, skt.formno.xlsaddress);
            }

            //released
            if (skt.released.value == "")
            {
                //spreadsheetlight
                //var dt = Double.Parse(GetCellValueAsString(sl, skt.released.xlsaddress)); 
                //skt.released.value = DateTime.FromOADate(dt).ToString("d");

                var dtstring = GetCellValueAsString(sl, skt.released.xlsaddress);
                skt.released.value = dtstring;
                
            }

            //updateat
            if (skt.updateat.value == "")
            {
                //var dt = Double.Parse(GetCellValueAsString(sl, skt.updateat.xlsaddress));
                //skt.updateat.value = DateTime.FromOADate(dt).ToString("d");

                var dtstring = GetCellValueAsString(sl, skt.updateat.xlsaddress);
                skt.updateat.value = dtstring;
            }

            //revision
            if (skt.revision.value == "")
            {
                skt.revision.value = GetCellValueAsString(sl, skt.revision.xlsaddress);
            }

            //typeinfo.hankan
            if (skt.typeinfo.hankan.value == "")
            {
                skt.typeinfo.hankan.value = GetCellValueAsString(sl, skt.typeinfo.hankan.xlscol, row);
            }

            //typeinfo.kansei
            if (skt.typeinfo.kansei.value == "")
            {
                skt.typeinfo.kansei.value = GetCellValueAsString(sl, skt.typeinfo.kansei.xlscol, row);
            }

            //typeinfo.kyakusaki
            if (skt.typeinfo.kyakusaki.value == "")
            {
                skt.typeinfo.kyakusaki.value = GetCellValueAsString(sl, skt.typeinfo.kyakusaki.xlscol, row);
            }

            //lotinfo.torikosu
            if (skt.lotinfo.torikosu.value == "")
            {
                skt.lotinfo.torikosu.value = GetCellValueAsString(sl, skt.lotinfo.torikosu.xlscol, row);
                int i;
                if (!int.TryParse(skt.lotinfo.torikosu.value, out i))
                {
                    OskNLog.Log("新機種展開表の取り個数が不正です：" + skt.lotinfo.torikosu.value, 2);
                    return false;
                }
            }

            //lotinfo.maisu_lot
            if (skt.lotinfo.lotmaisu.value == "")
            {
                skt.lotinfo.lotmaisu.value = GetCellValueAsString(sl, skt.lotinfo.lotmaisu.xlscol, row);
                int i;
                if (!int.TryParse(skt.lotinfo.lotmaisu.value, out i))
                {
                    OskNLog.Log("新機種展開表の基板枚数が不正です：" + skt.lotinfo.lotmaisu.value, 2);
                    return false;
                }
            }

            //lotinfo.pkg_lot
            if (skt.lotinfo.lotpics.value == "")
            {
                skt.lotinfo.lotpics.value = GetCellValueAsString(sl, skt.lotinfo.lotpics.xlscol, row);
                int i;
                if (!int.TryParse(skt.lotinfo.lotpics.value, out i))
                {
                    OskNLog.Log("新機種展開表のPKG数が不正です：" + skt.lotinfo.lotpics.value, 2);
                    return false;
                }
            }

            //etc.buhinhyou.zuban.jp
            if (skt.etc.buhinhyou.zuban.jp.value == "")
            {
                skt.etc.buhinhyou.zuban.jp.value = GetCellValueAsString(sl, skt.etc.buhinhyou.zuban.jp.xlscol, row);
            }

            //etc.buhinhyou.zuban.ch
            if (skt.etc.buhinhyou.zuban.ch.value == "")
            {
                skt.etc.buhinhyou.zuban.ch.value = GetCellValueAsString(sl, skt.etc.buhinhyou.zuban.ch.xlscol, row);
            }

            // 部品表を検出する
            var bompath = GetBomPath(skt.etc.buhinhyou.zuban.jp.value + "@" + skt.typeinfo.kansei.value + ".xlsx");

            if (bompath == "")
            {
                OskNLog.Log(skt.etc.buhinhyou.zuban.jp.value + "@" + skt.typeinfo.kansei.value + ".xlsx は対検索対象フォルダから検出できませんでした", 2);
                OskNLog.Log("中止します", 2);
                return false;
            }

            // 部品表
            // zuban
            var bhh = procobj.procmastermodel.buhinhyou;
            bhh.zuban = new Zuban();
            bhh.zuban.jp = new Jp();
            bhh.zuban.jp.value = skt.etc.buhinhyou.zuban.jp.value;
            bhh.zuban.ch = new Ch();
            bhh.zuban.ch.value = skt.etc.buhinhyou.zuban.ch.value;

            // 部品表を読み込む
            // var slbom = new SLDocument(bompath); //SpreadSheetLight
            // ExcelDataReaderで読込
            System.Data.DataTable slbom = new System.Data.DataTable(); //ExcelDataReader
            if (!string.IsNullOrEmpty(ExcelDataReader(bompath, "BomOptBziRpt", ref slbom)))
            {
                OskNLog.Log("部品表を読込失敗", Cnslcnf.msg_error);
                return false;
            }

            // bhh.typecd.value
            if (bhh.typecd.value == "")
            {
                bhh.typecd.value = GetCellValueAsString(slbom, bhh.typecd.xlsaddress);
            }

            // formno
            if (bhh.formno.value == "")
            {
                bhh.formno.value = GetCellValueAsString(slbom, bhh.formno.xlsaddress);
            }

            // released
            if (bhh.released.value == "")
            {
                bhh.released.value = GetCellValueAsString(slbom, bhh.released.xlsaddress);
            }

            // process(工程)にmaterial(部品)を割り付ける
            var procs = procobj.procmastermodel.process;

            foreach (var proc in procs)
            {
                foreach (var material in proc.material.m4)
                {
                    if (material.name.value == "" || material.name.value == null)
                    {
                        if (material.name.bomaddress != "HANKAN")
                        {
                            material.name.value = GetCellValueAsString(slbom, material.name.bomaddress);
                            if (string.IsNullOrEmpty(material.name.value))
                            {
                                OskNLog.Log("部品表から取得した部品名が空です", 2);
                                OskNLog.Log("工程：" + proc.code, 2);
                                OskNLog.Log("基底モデルアドレス：" + material.name.bomaddress, 2);
                                return false;
                            }
                        }
                        else
                        {
                            material.name.value = "半完成品";
                        }
                    }
                    else
                    {
                        if (material.name.value != GetCellValueAsString(slbom, material.name.bomaddress))
                        {
                            OskNLog.Log("部品表から取得した部品コードが基底モデルと違っています", 2);
                            OskNLog.Log("工程：" + proc.code, 2);
                            OskNLog.Log("部品表：" + GetCellValueAsString(slbom, material.name.bomaddress), 2);
                            OskNLog.Log("基底モデル：" + material.name.value, 2);
                            return false;
                        }
                    }

                    if (material.code.value == "" || material.code.value == null)
                    {
                        if (material.code.bomaddress != "HANKAN")
                        {
                            material.code.value = GetCellValueAsString(slbom, material.code.bomaddress);
                            if (string.IsNullOrEmpty(material.code.value))
                            {
                                OskNLog.Log("部品表から取得した部品コードが空です", 2);
                                OskNLog.Log("工程：" + proc.code, 2);
                                OskNLog.Log("基底モデルアドレス：" + material.code.bomaddress, 2);
                                return false;
                            }
                        }
                        else
                        {
                            material.code.value = skt.typeinfo.hankan.value;
                            OskNLog.Log("半完品を割り付けました", 2);
                            OskNLog.Log("工程：" + proc.code, 2);
                            OskNLog.Log("半完名：" + material.code.value, 2);
                        }
                    }
                    else
                    {
                        if (material.code.value != GetCellValueAsString(slbom, material.code.bomaddress))
                        {
                            OskNLog.Log("部品表から取得した部品コードが基底モデルと違っています", 2);
                            OskNLog.Log("工程：" + proc.code, 2);
                            OskNLog.Log("部品表：" + GetCellValueAsString(slbom, material.code.bomaddress), 2);
                            OskNLog.Log("基底モデル：" + material.code.value, 2);
                            return false;
                        }
                    }

                    if (material.qty.value == "" || material.qty.value == null)
                    {
                        if (!string.IsNullOrEmpty(material.qty.bomaddress))
                        {
                            int i;
                            if (!int.TryParse(GetCellValueAsString(slbom, material.qty.bomaddress).Replace("/", "").Replace("／", "").Replace(" ", ""), out i))
                            {
                                OskNLog.Log("部品表の数値が不正です" + material.code.value, 2);
                                OskNLog.Log("工程：" + proc.code, 2);
                                return false;
                            }

                            var bomvalue = GetCellValueAsString(slbom, material.qty.bomaddress);
                            var bomvalueArr = new string[2];
                            if (bomvalue.Contains('/'))
                            {
                                bomvalueArr = bomvalue.Split('/');
                            }
                            else if (bomvalue.Contains('／'))
                            {
                                bomvalueArr = bomvalue.Split('／');
                            }
                            else
                            {
                                bomvalueArr = new string[]
                                {
                                    bomvalue,
                                    "1"
                                };
                            }

                            // 分母が1ロットの場合
                            // var qtyValue = float.Parse(bomvalueArr[0]) / float.Parse(bomvalueArr[1]) * float.Parse(skt.lotinfo.torikosu.value) * float.Parse(skt.lotinfo.lotmaisu.value);
                            // 分母が1PKGの場合
                            // var qtyValue = float.Parse(bomvalueArr[0]) / float.Parse(bomvalueArr[1])

                            material.qty.value = bomvalueArr[0] + "/" + bomvalueArr[1];
                        }
                        else
                        {
                            material.qty.value = "1/1";
                        }
                    }
                    else
                    {
                        if (material.qty.value != GetCellValueAsString(slbom, material.qty.bomaddress))
                        {
                            OskNLog.Log("部品表から取得した部品数量が基底モデルと違っています", 2);
                            OskNLog.Log("工程：" + proc.code, 2);
                            OskNLog.Log("部品表：" + GetCellValueAsString(slbom, material.qty.bomaddress), 2);
                            OskNLog.Log("基底モデル：" + material.qty.value, 2);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool makeJissekiCoj(MastermodelRoot procobj, bool kansei, bool update, ref CojMst000001 coj_jisseki)
        {
            //coj_jisseki = new CojMst000001();
            //var update = false;
            string typecd;
            if (kansei)
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value;
            }
            else
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
            }

            //++++++++++++++++++++++++++
            // header
            //++++++++++++++++++++++++++
            coj_jisseki.header = new Header_CojMst000001();
            var datestr = DateTime.Now.ToString("yyyyMMddhhmmss");
            // thisdocNo
            coj_jisseki.header.thisdocNo = typecd + ".JCOJ";
            // update?
            if (update)
            {
                // updateBy, updateAt
                coj_jisseki.header.updateBy = "jn-wtnb";
                coj_jisseki.header.updateBy = datestr;
            }
            else
            {
                // createBy, createAt
                coj_jisseki.header.createdBy = "jn-wtnb";
                coj_jisseki.header.createdAt = datestr;
            }
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - docs
            //++++++++++++++++++++++++++
            coj_jisseki.cejObject = new CejObject();
            coj_jisseki.cejObject.coHeader = new CoHeader();
            coj_jisseki.cejObject.coHeader.docs = new List<Doc>();
            // 新機種展開表
            var doc_sinkishu = new Doc();
            doc_sinkishu.name = "新機種展開表";
            doc_sinkishu.formno = procobj.procmastermodel.shinkishutenkai.formno.value;
            doc_sinkishu.released = procobj.procmastermodel.shinkishutenkai.released.value;
            doc_sinkishu.update = procobj.procmastermodel.shinkishutenkai.updateat.value;
            doc_sinkishu.revision = procobj.procmastermodel.shinkishutenkai.revision.value;
            coj_jisseki.cejObject.coHeader.docs.Add(doc_sinkishu);
            // 部品表
            var doc_buhinhyo = new Doc();
            doc_buhinhyo.name = "部品表";
            doc_buhinhyo.formno = procobj.procmastermodel.buhinhyou.formno.value;
            doc_buhinhyo.released = procobj.procmastermodel.buhinhyou.released.value;
            if (procobj.procmastermodel.buhinhyou.zuban != null)
            {
                doc_buhinhyo.formno_jp = procobj.procmastermodel.buhinhyou.zuban.jp.value;
                doc_buhinhyo.formno_ch = procobj.procmastermodel.buhinhyou.zuban.ch.value;
            }
            else
            {
                doc_buhinhyo.formno_jp = "";
                doc_buhinhyo.formno_ch = "";
            }
            coj_jisseki.cejObject.coHeader.docs.Add(doc_buhinhyo);
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - kishuprofile
            //++++++++++++++++++++++++++
            coj_jisseki.cejObject.coHeader.kishuprofile = new Kishuprofile();
            coj_jisseki.cejObject.coHeader.kishuprofile.name = "照明HWLED_Ver.9";
            coj_jisseki.cejObject.coHeader.kishuprofile.foldername = "ver9";
            coj_jisseki.cejObject.coHeader.kishuprofile.revision = 1;
            //++++++++++++++++++++++++++
            // coList
            // "verify_hinmokucode"
            //++++++++++++++++++++++++++
            coj_jisseki.cejObject.coList = new List<CoList>();
            var colistobj = new CoList();
            colistobj.function = "verify_hinmokucode";
            colistobj.props = new Props();
            colistobj.props.propname = "品目コード";
            colistobj.props.propdata = new List<object>();
            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                colistobj.props.propdata.Add(typecd + "_" + item.code);
            }
            colistobj.props.propdata.Add(typecd);
            colistobj.props.propdata.Reverse();
            coj_jisseki.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "overwrite_seihinkousei"
            //++++++++++++++++++++++++++
            colistobj = new CoList();
            colistobj.function = "overwrite_seihinkousei";
            colistobj.props = new Props();
            colistobj.props.propname = "製品工程構成";
            colistobj.props.propdata = new List<object>();

            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                var seihinkoutei = new SeihinKouteiKosei();
                seihinkoutei.hinmokukodo = typecd + "_" + item.code;
                seihinkoutei.seihinkousei = new List<Seihinkousei>();
                var addedCodeList = new List<string>();
                var kouteisagyoubangoList = new List<string>();
                foreach (var ks in item.kouteisagyoubango)
                {
                    if (ks != null){
                        kouteisagyoubangoList.Add(ks.sagyobango);
                    }
                }

                foreach (var proc in procobj.procmastermodel.process)
                {
                    if (kouteisagyoubangoList.Contains(proc.code))
                    {
                        if (proc.material.m4.Count > 0)
                        {
                            foreach (var mat in proc.material.m4)
                            {
                                if (!addedCodeList.Contains(mat.code.value))
                                {
                                    var siyousu = mat.qty.value.Split('/');
                                    if (siyousu.Length != 2)
                                    {
                                        msg = "使用数の分子分母が不正です";
                                        OskNLog.Log(msg, Cnslcnf.msg_error);
                                        return false;
                                    }

                                    var buzai = new Seihinkousei();
                                    buzai.meisho = mat.code.value;
                                    buzai.siyouryou = new List<int> { int.Parse(siyousu[0]), int.Parse(siyousu[1]) };
                                    buzai.yukokigen = new List<string>
                                        {
                                            DateTime.Now.AddYears(-1).ToString("yyyy") + "0101",
                                            "99999999"
                                        };
                                    addedCodeList.Add(buzai.meisho);
                                    seihinkoutei.seihinkousei.Add(buzai);
                                }
                                else
                                {
                                    foreach (var sk in seihinkoutei.seihinkousei)
                                    {
                                        if (sk.meisho == mat.code.value)
                                        {
                                            sk.siyouryou[1] += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                colistobj.props.propdata.Add(seihinkoutei);
            }
            coj_jisseki.cejObject.coList.Add(colistobj);

            return true;
        }


        private bool make4mCoj(MastermodelRoot procobj, bool kansei, bool update, ref CojMst000002 coj_4m)
        {
            //coj_jisseki = new CojMst000001();
            //var update = false;
            string typecd;
            if (kansei)
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value;
            }
            else
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
            }

            //++++++++++++++++++++++++++
            // header
            //++++++++++++++++++++++++++
            coj_4m.header = new Header_CojMst000002();
            var datestr = DateTime.Now.ToString("yyyyMMddhhmmss");
            // thisdocNo
            coj_4m.header.thisdocNo = typecd + ".MCOJ";
            // update?
            if (update)
            {
                // updateBy, updateAt
                coj_4m.header.updateBy = "jn-wtnb";
                coj_4m.header.updateBy = datestr;
            }
            else
            {
                // createBy, createAt
                coj_4m.header.createdBy = "jn-wtnb";
                coj_4m.header.createdAt = datestr;
            }
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - docs
            //++++++++++++++++++++++++++
            coj_4m.cejObject = new CejObject();
            coj_4m.cejObject.coHeader = new CoHeader();
            coj_4m.cejObject.coHeader.docs = new List<Doc>();
            // 新機種展開表
            var doc_sinkishu = new Doc();
            doc_sinkishu.name = "新機種展開表";
            doc_sinkishu.formno = procobj.procmastermodel.shinkishutenkai.formno.value;
            doc_sinkishu.released = procobj.procmastermodel.shinkishutenkai.released.value;
            doc_sinkishu.update = procobj.procmastermodel.shinkishutenkai.updateat.value;
            doc_sinkishu.revision = procobj.procmastermodel.shinkishutenkai.revision.value;
            coj_4m.cejObject.coHeader.docs.Add(doc_sinkishu);
            // 部品表
            var doc_buhinhyo = new Doc();
            doc_buhinhyo.name = "部品表";
            doc_buhinhyo.formno = procobj.procmastermodel.buhinhyou.formno.value;
            doc_buhinhyo.released = procobj.procmastermodel.buhinhyou.released.value;
            if (procobj.procmastermodel.buhinhyou.zuban != null)
            {
                doc_buhinhyo.formno_jp = procobj.procmastermodel.buhinhyou.zuban.jp.value;
                doc_buhinhyo.formno_ch = procobj.procmastermodel.buhinhyou.zuban.ch.value;
            }
            else
            {
                doc_buhinhyo.formno_jp = "";
                doc_buhinhyo.formno_ch = "";
            }
            coj_4m.cejObject.coHeader.docs.Add(doc_buhinhyo);
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - kishuprofile
            //++++++++++++++++++++++++++
            coj_4m.cejObject.coHeader.kishuprofile = new Kishuprofile();
            coj_4m.cejObject.coHeader.kishuprofile.name = "照明HWLED_Ver.9";
            coj_4m.cejObject.coHeader.kishuprofile.foldername = "ver9";
            coj_4m.cejObject.coHeader.kishuprofile.revision = 1;
            //++++++++++++++++++++++++++
            // coList
            // "append_kouteisagyou"
            //++++++++++++++++++++++++++
            coj_4m.cejObject.coList = new List<CoList>();

            var colistobj = new CoList();
            colistobj.function = "append_kouteisagyou";
            colistobj.props = new Props();
            colistobj.props.propname = "工程作業追加";
            colistobj.props.propdata = new List<object>();
            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                if (item.kouteisagyoubango[0] != null)
                {
                    foreach (var ksb in item.kouteisagyoubango)
                    {
                        if (ksb.prop.command == "append")
                        {
                            var kpd = new KouteisagyouPropData()
                            {
                                kouteisagyou = ksb.sagyobango,
                                koutei = item.code,
                                sagyoujun = ksb.prop.sagyoujun,
                                yukoujyoutai = ksb.prop.yukoujyoutai
                            };
                            colistobj.props.propdata.Add(kpd);
                        }
                    }
                }
            }
            coj_4m.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "modify_kouteisagyou"
            //++++++++++++++++++++++++++
            colistobj = new CoList();
            colistobj.function = "modify_kouteisagyou";
            colistobj.props = new Props();
            colistobj.props.propname = "工程作業編集";
            colistobj.props.propdata = new List<object>();
            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                if (item.kouteisagyoubango[0] != null)
                {
                    foreach (var ksb in item.kouteisagyoubango)
                    {
                        if (ksb.prop.command == "modify")
                        {
                            var kpd = new KouteisagyouPropData()
                            {
                                kouteisagyou = ksb.sagyobango,
                                koutei = item.code,
                                sagyoujun = ksb.prop.sagyoujun,
                                yukoujyoutai = ksb.prop.yukoujyoutai
                            };
                            colistobj.props.propdata.Add(kpd);
                        }
                    }
                }
            }
            coj_4m.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "verify_curent_kouteisagyou"
            //++++++++++++++++++++++++++
            colistobj = new CoList();
            colistobj.function = "verify_curent_kouteisagyou";
            colistobj.props = new Props();
            colistobj.props.propname = "工程作業照合";
            colistobj.props.propdata = new List<object>();
            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                if (item.kouteisagyoubango[0] != null)
                {
                    foreach (var ksb in item.kouteisagyoubango)
                    {
                        var kpd = new KouteisagyouPropData()
                        {
                            kouteisagyou = ksb.sagyobango,
                            koutei = item.code,
                            sagyoujun = ksb.prop.sagyoujun,
                            yukoujyoutai = ksb.prop.yukoujyoutai
                        };
                        colistobj.props.propdata.Add(kpd);
                    }
                }  
            }
            coj_4m.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "overwrite_seihinkousei"
            //++++++++++++++++++++++++++
            colistobj = new CoList();
            colistobj.function = "overwrite_kouteisagyoubuzai";
            colistobj.props = new Props();
            colistobj.props.propname = "工程作業・部材編集";
            colistobj.props.propdata = new List<object>();

            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                
                foreach (var ksb in item.kouteisagyoubango)
                {
                    var bzilst = new List<Buzai>();
                    var sagyoukijunsho = string.Empty;
                    if (item.kouteisagyoubango[0] != null)
                    {
                        foreach (var proc in procobj.procmastermodel.process)
                        {
                            if (proc.code == ksb.sagyobango)
                            {
                                foreach (var mat in proc.material.m4)
                                {
                                    var siyousu = mat.qty.value.Split('/');
                                    if (siyousu.Length != 2)
                                    {
                                        msg = "使用数の分子分母が不正です";
                                        OskNLog.Log(msg, Cnslcnf.msg_error);
                                        return false;
                                    }

                                    var bzi = new Buzai()
                                    {
                                        buzaikodo = mat.code.value,
                                        siyouryou = double.Parse(siyousu[0]) / double.Parse(siyousu[1])
                                    };
                                    bzilst.Add(bzi);
                                }
                                sagyoukijunsho = proc.m4.kjunsho;
                            }
                        }

                        var kpd = new HenshuKouteiSagyouPropData()
                        {
                            kouteisagyou = ksb.sagyobango,
                            sagyoukijunkodo = sagyoukijunsho,
                            buzai = bzilst
                        };
                        colistobj.props.propdata.Add(kpd);
                    }
                }
            }
            coj_4m.cejObject.coList.Add(colistobj);

            return true;
        }


        private string GetBomPath(string bomno)
        {
            var serchDir = conf.makeprocjson.config.path.buhinhyoufolder;
            foreach (string d in Directory.GetDirectories(serchDir))
            {
                var fileName = d + "\\" + bomno;
                if (System.IO.File.Exists(fileName))
                {
                    OskNLog.Log("'" + fileName + "'の存在を確認", 1);
                    return fileName;
                }
            }

            return "";
        }


        private string GetCellValueAsString(SLDocument exobj, string address)
        {
            return exobj.GetCellValueAsString(address);
        }


        private string GetCellValueAsString(SLDocument exobj, string col, decimal row)
        {
            return exobj.GetCellValueAsString(col+row);
        }


        private string GetCellValueAsString(System.Data.DataTable exobj, string address)
        {
            var addarr = Regex.Split(address, @"(?<=[a-zA-Z])(?=\d)");
            var rownm = ToAlphabetIndex(addarr[0]) -1;
            var colnm = int.Parse(addarr[1]) -1;
            var ret = exobj.Rows[colnm][rownm].ToString();
            return ret;
        }


        private string GetCellValueAsString(System.Data.DataTable exobj, string col, decimal row)
        {
            var rownm = ToAlphabetIndex(col) - 1;
            var colnm = (int)row - 1; // int.Parse(row);
            var ret = exobj.Rows[colnm][rownm].ToString();
            return ret;
        }


        public static int ToAlphabetIndex(string alphabet)
        {
            if (string.IsNullOrEmpty(alphabet))
            {
                return -1;
            }

            if (new System.Text.RegularExpressions.Regex("^[A-Z]+$").IsMatch(alphabet))
            {
                int index = 0;
                for (int i = 0; i < alphabet.Length; i++)
                {
                    // ASCIIではAは10進数で65
                    int num = Convert.ToChar(alphabet[alphabet.Length - i - 1]) - 65;
                    // A-Zの変換が0-25になっているため1を足して、A-Zが1-26になるようにする
                    num++;

                    index += (int)(num * Math.Pow(26, i));
                }
                return index;
            }

            return -1;
        }

        private string ExcelDataReader(string filepath, string sheetname, ref System.Data.DataTable worksheet)
        {
            //ファイルの読み取り開始
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else if (filepath.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else
                {
                    return "サポート対象外の拡張子です。";
                }

                //全シート全セルを読み取り
                var dataset = reader.AsDataSet();

                //シート名を指定
                worksheet = dataset.Tables[sheetname];

                if (worksheet is null)
                {
                    return "指定されたワークシート名が存在しません。";
                }

                reader.Close();
            }

            return "";
        }
    }

}
