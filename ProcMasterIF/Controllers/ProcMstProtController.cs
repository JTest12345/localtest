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

namespace ProcMasterIF
{
    class ProcMstProtController
    {
        string workingdir = @"C:\Oskas\procmaster\shomei\ver9_old";
        SeriesTypeMaster sr;
        SLDocument sl;
        MakeprocjsonRoot conf;
        List<int> colList;
        string msg;
        string crlf = "\r\n";

        public ProcMstProtController()
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

            this.colList = new List<int>();
            foreach (var colstr in conf.makeprocjson.config.model[0].shinkishutenkaicol)
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
                        colList.Add(i);
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
                    colList.Add(colNum);
                }
            }

            OskNLog.Log("makeを開始しました。", Cnslcnf.msg_info);
        }

        public bool Excute()
        {
            /////////////////////////////////
            /// 新機種展開表読込
            /////////////////////////////////
            OskNLog.Log("新機種展開表を読込開始", Cnslcnf.msg_info);
            sl = new SLDocument(conf.makeprocjson.config.path.shinkishutenkaifile);
            OskNLog.Log("新機種展開表を読込完了", Cnslcnf.msg_info);
            var orthankanfld = workingdir + @"\model\sources\" + conf.makeprocjson.config.model[0].folder + @"\" + conf.makeprocjson.config.model[0].hankan;
            var ortkanseifld = workingdir + @"\model\sources\" + conf.makeprocjson.config.model[0].folder + @"\" + conf.makeprocjson.config.model[0].kansei;


            /////////////////////////////////
            ///make procjson
            /////////////////////////////////
            foreach (var col in colList)
            {
                var procobj_hankan = new MastermodelRoot();
                var procobj_kansei = new MastermodelRoot();

                var hankan = sl.GetCellValueAsString("A" + col);
                var kansei = sl.GetCellValueAsString("B" + col);

                OskNLog.Log("hankan-" + col + "=" + hankan, 1);
                OskNLog.Log("kansei-" + col + "=" + kansei, 1);

                ////////////////////////////////
                // 半完マスタ
                ////////////////////////////////
                ///
                var orthankan = new Procmastermodel();

                // 新機種展開表をルートからコピー
                orthankan.shinkishutenkai = sr.shinkishutenkai.DeepClone(); ;

                // 部品表をルートからコピー
                orthankan.buhinhyou = sr.buhinhyou.DeepClone(); ;

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

                procobj_hankan.procmastermodel = orthankan;

                // ProcJson作成
                if (!makeProcJson(col, ref procobj_hankan))
                {
                    return false;
                }


                ////////////////////////////////
                // 完成品マスタ
                ////////////////////////////////
                ///
                var ortkansei = new Procmastermodel();

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

                // ProcJson作成
                procobj_kansei.procmastermodel = ortkansei;
                if (!makeProcJson(col, ref procobj_kansei))
                {
                    return false;
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
                    //if (!System.IO.File.Exists(jsonpath))
                    //{
                    //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);
                    //}
                    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj_hankan, ref msg);
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

            msg = "makeを完了しました。";
            OskNLog.Log(msg, Cnslcnf.msg_info);

            return false;
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
                OskNLog.Log(skt.etc.buhinhyou.zuban.jp.value + "@" + skt.typeinfo.kansei.value + ".xlsx は対検索対象フォルダから検出できませんでした", 2);
                OskNLog.Log("中止します", 2);
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
                            OskNLog.Log("部品表から取得した部品コードが基底モデルと違っています", 2);
                            OskNLog.Log("部品表：" + slbom.GetCellValueAsString(material.name.bomaddress), 2);
                            OskNLog.Log("基底モデル：" + material.name.value, 2);
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
                            OskNLog.Log("部品表から取得した部品コードが基底モデルと違っています", 2);
                            OskNLog.Log("部品表：" + slbom.GetCellValueAsString(material.code.bomaddress), 2);
                            OskNLog.Log("基底モデル：" + material.code.value, 2);
                            return false;
                        }
                    }
                }
            }

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
    }

}
