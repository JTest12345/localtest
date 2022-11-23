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
    class MakeProcMst
    {
        string workingdir = @"C:\Oskas\procmaster\shomei\ver9";
        string armsworkingdir = @"C:\Oskas\procmaster\shomei\ver9\model\root\arms";
        string armssqldir = @"C:\Oskas\procmaster\shomei\ver9\sql\arms";
        SeriesTypeMaster sr;
        //新機種展開表オブジェクト
        //SLDocument sl; //SpreadSheetLight
        System.Data.DataTable sl;

        //素子波長一覧オブジェクト
        //SLDocument sl; //SpreadSheetLight
        System.Data.DataTable ss_hist; 
        System.Data.DataTable ss_data;

        //Arms機種Keyテーブルオブジェクト
        Dictionary<string, System.Data.DataTable> amstbls_hankan;
        Dictionary<string, System.Data.DataTable> amstbls_kansei; 

        MakeprocjsonRoot conf;
        public List<SeriesTypeMaster> srList;
        string msg;
        string crlf = "\r\n";

        ArmsSqlIF asif = new ArmsSqlIF();

        public MakeProcMst()
        {
            //*************************************************************************************************************
            // ※ルート：ここでは対象ヴァージョンの共通情報をルートという
            //
            // ◆ルート処理① 各種yamlファイルの読込
            //*************************************************************************************************************

            /////////////////////////////////////////////////////
            ///conf
            /// 機種フォルダの基本情報
            /// ①各種資料のtype名/path
            /// ②シリーズ毎の新機種展開行指定
            /// ③その他
            /// が書かれたcojmakeprocfile.yamlをオブジェクト化
            /////////////////////////////////////////////////////
            ///
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


            /////////////////////////////////////////////////////
            ///ROOT
            /// ①各種EXCEL資料を読込するための各種オブジェクト
            /// ②工程定義
            /// を丸っと収めた形状シリーズ毎のリスト(srList)を作る
            /////////////////////////////////////////////////////
            ///
            var rootmodelfldpath = workingdir + @"\model\root";

            // ◇新機種展開表
            // 機種名・ロット構成・部品表図番・改定情報などのExcelアドレスと値枠
            yamlPath = rootmodelfldpath + @"\shinkishutenkai.yaml";
            var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            var shinkishutenai_obj = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);

            // ◇部品表
            // 機種名・図番・改定情報などのExcelアドレスと値枠
            yamlPath = rootmodelfldpath + @"\buhinhyou.yaml";
            var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var buhinhyou_obj = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);

            // ◇素子制限
            // 機種名・素子情報・制限・ソーティング情報・改定情報などのExcelアドレスと値枠
            yamlPath = rootmodelfldpath + @"\soshiseigen.yaml";
            var soshiseigen_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var soshiseigen_obj = deserializer.Deserialize<SoshiSeigen>(soshiseigen_yaml);

            // ◇工程定義
            // 工程記号の工程順列記リスト
            yamlPath = rootmodelfldpath + @"\processlist.yaml";
            var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
            deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
            var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);

            // ◇工程詳細辞書
            // 実績, 4M, Armsの工程情報、部材情報の部品表Excelアドレスと値枠
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

            // ◇ARMSコンフィグ
            // テーブル情報／DB接続情報
            var armsyamlPath = armsworkingdir + @"\config\armsconfig.yaml";
            var armsconfig_yaml = new StreamReader(armsyamlPath, Encoding.UTF8);
            var armsdeserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            var armsconf_obj = armsdeserializer.Deserialize<ArmsDbConfig>(armsconfig_yaml);

            /// ◇ Arms機種Keyテーブル
            /// テーブルオブジェクトのList:
            /// amstbls_hankan
            /// amstbls_kansei
            amstbls_hankan = new Dictionary<string, System.Data.DataTable>();
            amstbls_kansei = new Dictionary<string, System.Data.DataTable>();
            foreach (var tblnm in armsconf_obj.table)
            {
                var shettbl = new System.Data.DataTable();
                if (!string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.armstypecdtbls.hankan, tblnm, ref shettbl)))
                {
                    OskNLog.Log("半完用Arms機種KeyテーブルExcelを読込失敗", Cnslcnf.msg_error);
                    return;
                }
                amstbls_hankan.Add(tblnm, shettbl);

                shettbl = new System.Data.DataTable();
                if (!string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.armstypecdtbls.kansei, tblnm, ref shettbl)))
                {
                    OskNLog.Log("完成用Arms機種KeyテーブルExcelを読込失敗", Cnslcnf.msg_error);
                    return;
                }
                amstbls_kansei.Add(tblnm, shettbl);
            }
            var amstbls_dict = new Dictionary<string, Dictionary<string, System.Data.DataTable>>()
            {
                {"hankan",  amstbls_hankan},
                {"kansei",  amstbls_kansei},
            };


            //*************************************************************************************************************
            // ◆ルート処理②　各種yamlファイル情報のオブジェクト化
            // srListに形状シリーズ毎、上記のオブジェクトを丸っと纏めてリスト化
            //*************************************************************************************************************
            srList = new List<SeriesTypeMaster>();
            foreach (var model in conf.makeprocjson.config.model)
            {
                sr = new SeriesTypeMaster();
                sr.shinkishutenkai = shinkishutenai_obj;
                sr.buhinhyou = buhinhyou_obj;
                sr.processdict = process_dict;
                sr.model = model;
                sr.soshiseigen = soshiseigen_obj;
                sr.armsdbconfig = armsconf_obj;
                sr.amstblsdict = amstbls_dict;

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
            //*************************************************************************************************************
            // ■新機種展開表の完成品行を基準として各JSONファイルを生成する
            // 半完成品については共通する完成品に合わせ各ファイルが上書き生成される為、最終的には共通する完成品の
            // 一番最後で生成されたファイルが残ることになります
            //
            // ◆Excel読込
            // 新機種展開表／素子波長指定一覧をExcelDataReaderで読込
            //*************************************************************************************************************

            //////////////////////////////////////
            /// 新機種展開表読込
            /// オブジェクト：sl
            //////////////////////////////////////
            OskNLog.Log("新機種展開表を読込開始", Cnslcnf.msg_info);
            // SpreadSheetlightで読込
            // sl = new SLDocument(conf.makeprocjson.config.path.shinkishutenkaifile);
            // ExcelDataReaderで読込
            if (!string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.shinkishutenkaifile, "一覧", ref sl)))
            {
                OskNLog.Log("新機種展開表を読込失敗", Cnslcnf.msg_error);
                return false;
            }

            OskNLog.Log("新機種展開表を読込完了", Cnslcnf.msg_info);


            //////////////////////////////////////
            /// 素子波長指定一覧読込
            /// 変更履歴オブジェクト：ss_hist
            /// 詳細オブジェクト　　：ss_data
            //////////////////////////////////////
            OskNLog.Log("素子波長指定一覧を読込開始", Cnslcnf.msg_info);
            // ExcelDataReaderで読込
            if (!string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.soshiseigenfile, "変更履歴", ref ss_hist)))
            {
                OskNLog.Log("素子波長指定一覧（履歴）を読込失敗", Cnslcnf.msg_error);
                return false;
            }
            if (!string.IsNullOrEmpty(ExcelDataReader(conf.makeprocjson.config.path.soshiseigenfile, "詳細", ref ss_data)))
            {
                OskNLog.Log("素子波長指定一覧（詳細）を読込失敗", Cnslcnf.msg_error);
                return false;
            }

            OskNLog.Log("素子波長指定一覧を読込完了", Cnslcnf.msg_info);


            return true;
        }


        public bool makeProcObjectsAndJson(string folder, SeriesTypeMaster sr, bool isThisKansei, ref Dictionary<string, PROC_OBJECTS> PrObjDict)
        {
            foreach (var col in sr.collist)
            {
                //*************************************************************************************************************
                //
                // ■ ProcMasterObjectを作成開始
                //    全ての情報はPrObjDictに機種名keyで参照渡し
                //
                //*************************************************************************************************************
                var procobj = new MastermodelRoot();
                var ort = new Procmastermodel();
                var PrObj = new PROC_OBJECTS();

                // ◇新機種展開objをルートからコピー
                ort.shinkishutenkai = sr.shinkishutenkai.DeepClone(); ;

                // ◇部品表objをルートからコピー
                ort.buhinhyou = sr.buhinhyou.DeepClone(); ;

                // ◇工程辞書objをルートからコピー
                var processdict = sr.processdict.DeepClone();

                // ◇品目マスタ―
                // 形状シリーズ毎の実績・4Mの工程定義読込
                var yamlPath = folder + @"\hinmokumaster.yaml";
                var hinmokumaster_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();
                ort.hinmokumaster = deserializer.Deserialize<List<HINMOKUMASTER>>(hinmokumaster_yaml);

                // ◇Armsテーブル
                if (!isThisKansei)
                {
                    ort.amstbls = sr.amstblsdict["hankan"];
                }
                else
                {
                    ort.amstbls = sr.amstblsdict["kansei"];
                }

                //*************************************************************************************************************
                // ◇オーバーライド処理
                // ルートの情報に対して対象シリーズが相違がある場合、シリーズフォルダにオーバーライド用のファイルを用意
                // これらを読込、ルートの情報に上書きする
                //*************************************************************************************************************
                //【オーバーライド処理】新機種展開obj
                yamlPath = folder + @"\shinkishutenkai.yaml";
                if (CommonFuncs.FileExists(yamlPath))
                {
                    OskNLog.Log("新機種展開objオーバーライド実施", Cnslcnf.msg_info);
                    var shinkishutenai_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    deserializer = new DeserializerBuilder()
                                       .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                       .Build();
                    ort.shinkishutenkai = deserializer.Deserialize<Shinkishutenkai>(shinkishutenai_yaml);
                }

                //【オーバーライド処理】部品表obj
                yamlPath = folder + @"\buhinhyou.yaml";
                if (CommonFuncs.FileExists(yamlPath))
                {
                    OskNLog.Log("部品表objオーバーライド実施", Cnslcnf.msg_info);
                    var buhinhyou_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    deserializer = new DeserializerBuilder()
                                       .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                       .Build();
                    ort.buhinhyou = deserializer.Deserialize<Buhinhyou>(buhinhyou_yaml);
                }

                //【オーバーライド処理】工程定義obj
                yamlPath = folder + @"\processlist.yaml";
                if (CommonFuncs.FileExists(yamlPath))
                {
                    OskNLog.Log("工程定義objオーバーライド実施", Cnslcnf.msg_info);
                    var processlist_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                    deserializer = new DeserializerBuilder()
                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                    .Build();
                    var processlist_obj = deserializer.Deserialize<List<string>>(processlist_yaml);

                    // ◇工程詳細辞書オーバーライド
                    var process_dict = new Dictionary<string, Process>();
                    foreach (var proc in processlist_obj)
                    {
                        if (processdict.ContainsKey(proc))
                        {
                            if (processdict.Remove(key: proc) == true)
                            {
                                OskNLog.Log("工程オーバーライド：" + proc, Cnslcnf.msg_info);
                            }
                            else
                            {
                                OskNLog.Log("工程オーバーライドが失敗：" + proc, Cnslcnf.msg_info);
                                return false;
                            }
                        }
                        yamlPath = folder + @"\process_override\" + proc + ".yaml";
                        var proc_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                        deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .Build();
                        var proc_obj = deserializer.Deserialize<Process>(proc_yaml);
                        processdict[proc] = proc_obj;
                    }
                }

                //【オーバーライド処理】Armsテーブル
                var xlsPath = folder + @"\arms_override\arms_typecd_tbls.xls";
                var amstbls = new Dictionary<string, System.Data.DataTable>();
                if (CommonFuncs.FileExists(xlsPath))
                {
                    OskNLog.Log("Armsテーブルオーバーライド実施", Cnslcnf.msg_info);
                    foreach (var tblnm in sr.armsdbconfig.table)
                    {
                        var shettbl = new System.Data.DataTable();
                        if (!string.IsNullOrEmpty(ExcelDataReader(xlsPath, tblnm, ref shettbl)))
                        {
                            OskNLog.Log("オーバーライド用Arms機種KeyテーブルExcelを読込失敗", Cnslcnf.msg_error);
                            return false;
                        }
                        amstbls.Add(tblnm, shettbl);
                    }
                        
                    ort.amstbls = amstbls;
                }

                //*************************************************************************************************************
                // ◇ シリーズ工程詳細情報割付
                // シリーズ用にオーバーライド処理された工程詳細辞書を用いて品目マスタで定義された工程詳細を引き出す処理
                //*************************************************************************************************************
                ort.process = new List<Process>();
                foreach (var obj in ort.hinmokumaster)
                {
                    if (obj.kouteisagyoubango != null)
                    {
                        foreach (var proc in obj.kouteisagyoubango)
                        {
                            if (proc != null)
                            {
                                ort.process.Add(processdict[proc.sagyobango]);
                            }
                        }
                    }
                }

                //*************************************************************************************************************
                // ◆ ProcJson作成
                // ほとんどの情報を詰込んだオブジェクト:procobjを生成
                //*************************************************************************************************************
                procobj.procmastermodel = ort;
                if (!makeProcJson(col, ref procobj))
                {
                    return false;
                }

                var typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
                if (isThisKansei)
                {
                    typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value;
                }

                PrObj.ProcObj = new MastermodelRoot();
                PrObj.ProcObj.procmastermodel = procobj.procmastermodel;
                PrObj.TypeCdList = typecd;

                //*************************************************************************************************************
                // ◆ Coj_Jisseki作成
                // procobj_hankanをもとに実績用のCOJを作成
                //*************************************************************************************************************
                var coj_jisseki = new Coj.mst000001.Root();
                var update_jisseki = false;
                if (!makeJissekiCoj(procobj, false, update_jisseki, ref coj_jisseki))
                {
                    return false;
                }

                PrObj.CojJsk = coj_jisseki;

                //*************************************************************************************************************
                // ◆ Coj_4M作成
                // procobj_hankanをもとに4M用のCOJを作成
                //*************************************************************************************************************
                var coj_4m = new Coj.mst000002.Root();
                var update_4m = false;
                if (!make4mCoj(procobj, false, update_4m, ref coj_4m))
                {
                    return false;
                }

                PrObj.Coj4m = coj_4m;

                //*************************************************************************************************************
                // ◆ Coj_SoshiSeigen作成
                // procobj_hankanをもとに素子波長用のCOJを作成
                //*************************************************************************************************************
                var coj_ss = new Coj.mst000003.Root();
                //var update_ss_kansei = false;
                if (!makeSoshiCoj(procobj, false, sr, ref coj_ss))
                {
                    return false;
                }

                PrObj.CojSs = coj_ss;

                if (!PrObjDict.ContainsKey(typecd))
                {
                    PrObjDict.Add(typecd, PrObj);
                }
                
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
            //新機種展開表
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
                        if (material.name.bomaddress == "")
                        {
                            OskNLog.Log("部品名の取得先が指定されていません", 2);
                            return false;
                        }

                        // P-REQ(Production Request)でなければBOMから情報入手
                        if (material.name.bomaddress != "P-REQ")
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
                        if (material.code.bomaddress == "")
                        {
                            OskNLog.Log("部品コードの取得先が指定されていません", 2);
                            return false;
                        }

                        // P-REQ(Production Request)でなければBOMから情報入手
                        if (material.code.bomaddress != "P-REQ")
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
                        if (material.qty.bomaddress != "NA")
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
            }

            return true;
        }

        private bool makeJissekiCoj(MastermodelRoot procobj, bool kansei, bool update, ref Coj.mst000001.Root coj_jisseki)
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
            coj_jisseki.header = new Coj.mst000001.Header();
            var datestr = DateTime.Now.ToString("yyyyMMddhhmmss");
            // thisdocNo
            coj_jisseki.header.thisdocNo = typecd + "_J.JSON";
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
            coj_jisseki.cejObject = new Coj.mst000001.CejObject();
            coj_jisseki.cejObject.coHeader = new Coj.mst000001.CoHeader();
            coj_jisseki.cejObject.coHeader.docs = new List<Coj.mst000001.Doc>();
            // 新機種展開表
            var doc_sinkishu = new Coj.mst000001.Doc();
            doc_sinkishu.name = "新機種展開表";
            doc_sinkishu.formno = procobj.procmastermodel.shinkishutenkai.formno.value;
            doc_sinkishu.released = procobj.procmastermodel.shinkishutenkai.released.value;
            doc_sinkishu.update = procobj.procmastermodel.shinkishutenkai.updateat.value;
            doc_sinkishu.revision = procobj.procmastermodel.shinkishutenkai.revision.value;
            coj_jisseki.cejObject.coHeader.docs.Add(doc_sinkishu);
            // 部品表
            var doc_buhinhyo = new Coj.mst000001.Doc();
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
            coj_jisseki.cejObject.coHeader.kishuprofile = new Coj.mst000001.Kishuprofile();
            coj_jisseki.cejObject.coHeader.kishuprofile.name = "照明HWLED_Ver.9";
            coj_jisseki.cejObject.coHeader.kishuprofile.foldername = "ver9";
            coj_jisseki.cejObject.coHeader.kishuprofile.revision = 1;
            //++++++++++++++++++++++++++
            // coList
            // "verify_hinmokucode"
            //++++++++++++++++++++++++++
            coj_jisseki.cejObject.coList = new List<Coj.mst000001.CoList>();
            var colistobj = new Coj.mst000001.CoList();
            colistobj.function = "verify_hinmokucode";
            colistobj.props = new Coj.mst000001.Props();
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
            // "overwrite_seisan_lt_jyouhou"
            //++++++++++++++++++++++++++
            colistobj = new Coj.mst000001.CoList();
            colistobj.function = "overwrite_seisan_lt_jyouhou";
            colistobj.props = new Coj.mst000001.Props();
            colistobj.props.propname = "生産・LT情報";
            var dctdata = new Dictionary<string, string>(){
                   { "hyoujunlotsize", procobj.procmastermodel.shinkishutenkai.lotinfo.lotpics.value }
            };
            colistobj.props.propdata = new List<object>()
            {
                dctdata
            };
            coj_jisseki.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "overwrite_seihinkousei"
            //++++++++++++++++++++++++++
            colistobj = new Coj.mst000001.CoList();
            colistobj.function = "overwrite_seihinkousei";
            colistobj.props = new Coj.mst000001.Props();
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
                    if (ks != null)
                    {
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
                                    buzai.siyouryou = new List<double> { Double.Parse(siyousu[0]), int.Parse(siyousu[1]) };
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


        private bool make4mCoj(MastermodelRoot procobj, bool kansei, bool update, ref Coj.mst000002.Root coj_4m)
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
            coj_4m.header = new Coj.mst000002.Header();
            var datestr = DateTime.Now.ToString("yyyyMMddhhmmss");
            // thisdocNo
            coj_4m.header.thisdocNo = typecd + "_M.JSON";
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
            coj_4m.cejObject = new Coj.mst000002.CejObject();
            coj_4m.cejObject.coHeader = new Coj.mst000002.CoHeader();
            coj_4m.cejObject.coHeader.docs = new List<Coj.mst000002.Doc>();
            // 新機種展開表
            var doc_sinkishu = new Coj.mst000002.Doc();
            doc_sinkishu.name = "新機種展開表";
            doc_sinkishu.formno = procobj.procmastermodel.shinkishutenkai.formno.value;
            doc_sinkishu.released = procobj.procmastermodel.shinkishutenkai.released.value;
            doc_sinkishu.update = procobj.procmastermodel.shinkishutenkai.updateat.value;
            doc_sinkishu.revision = procobj.procmastermodel.shinkishutenkai.revision.value;
            coj_4m.cejObject.coHeader.docs.Add(doc_sinkishu);
            // 部品表
            var doc_buhinhyo = new Coj.mst000002.Doc();
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
            coj_4m.cejObject.coHeader.kishuprofile = new Coj.mst000002.Kishuprofile();
            coj_4m.cejObject.coHeader.kishuprofile.name = "照明HWLED_Ver.9";
            coj_4m.cejObject.coHeader.kishuprofile.foldername = "ver9";
            coj_4m.cejObject.coHeader.kishuprofile.revision = 1;
            //++++++++++++++++++++++++++
            // coList
            // "kishu_info"
            //++++++++++++++++++++++++++
            coj_4m.cejObject.coList = new List<Coj.mst000002.CoList>();
            var colistobj = new Coj.mst000002.CoList();
            colistobj.function = "kishu_info";
            colistobj.props = new Coj.mst000002.Props();
            colistobj.props.propname = "機種情報";
            colistobj.props.propdata = new List<object>();
            //機種名・倉庫グループ
            var propdata = new Coj.mst000002.KishuInfoPropData();
            propdata.kishucode = typecd;
            propdata.soukogr = conf.makeprocjson.etc.soukogr;
            colistobj.props.propdata.Add(propdata);
            coj_4m.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "append_kouteisagyou"
            //++++++++++++++++++++++++++
            colistobj = new Coj.mst000002.CoList();
            colistobj.function = "append_kouteisagyou";
            colistobj.props = new Coj.mst000002.Props();
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
                            var kpd = new Coj.mst000002.KouteisagyouPropData()
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
            colistobj = new Coj.mst000002.CoList();
            colistobj.function = "modify_kouteisagyou";
            colistobj.props = new Coj.mst000002.Props();
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
                            var kpd = new Coj.mst000002.KouteisagyouPropData()
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
            colistobj = new Coj.mst000002.CoList();
            colistobj.function = "verify_curent_kouteisagyou";
            colistobj.props = new Coj.mst000002.Props();
            colistobj.props.propname = "工程作業照合";
            colistobj.props.propdata = new List<object>();
            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {
                if (item.kouteisagyoubango[0] != null)
                {
                    foreach (var ksb in item.kouteisagyoubango)
                    {
                        var kpd = new Coj.mst000002.KouteisagyouPropData()
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
            colistobj = new Coj.mst000002.CoList();
            colistobj.function = "overwrite_kouteisagyoubuzai";
            colistobj.props = new Coj.mst000002.Props();
            colistobj.props.propname = "工程作業・部材編集";
            colistobj.props.propdata = new List<object>();

            foreach (var item in procobj.procmastermodel.hinmokumaster)
            {

                foreach (var ksb in item.kouteisagyoubango)
                {
                    var bzilst = new List<Coj.mst000002.Buzai>();
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

                                    var orzai = mat.orzai;
                                    if (orzai == "") orzai = "-";

                                    var bzi = new Coj.mst000002.Buzai()
                                    {
                                        buzaikodo = mat.code.value,
                                        siyouryou = (double.Parse(siyousu[0]) / double.Parse(siyousu[1])).ToString("F9"),
                                        orzairyou = orzai
                                    };
                                    bzilst.Add(bzi);
                                }
                                sagyoukijunsho = proc.m4.kjunsho;
                            }
                        }

                        var kpd = new Coj.mst000002.HenshuKouteiSagyouPropData()
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


        private bool makeSoshiCoj(MastermodelRoot procobj, bool kansei, SeriesTypeMaster srl, ref Coj.mst000003.Root coj_ss)
        {
            string typecd;
            if (kansei)
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value;
            }
            else
            {
                typecd = procobj.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
            }

            int i;
            for (i = srl.soshiseigen.indexinfo.kishu.start; i < srl.soshiseigen.indexinfo.kishu.end + 1; i++)
            {
                if (procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value == GetCellValueAsString(ss_data, srl.soshiseigen.indexinfo.kishu.xlscol, i)) break;
            }

            if (procobj.procmastermodel.shinkishutenkai.typeinfo.kansei.value != GetCellValueAsString(ss_data, srl.soshiseigen.indexinfo.kishu.xlscol, i))
            {
                OskNLog.Log("素子波長一覧から対象機種が見つかりませんでした", Cnslcnf.msg_error);
                return false;
            }

            //++++++++++++++++++++++++++
            // header
            //++++++++++++++++++++++++++
            coj_ss.header = new Coj.mst000003.Header();
            var datestr = DateTime.Now.ToString("yyyyMMddhhmmss");
            // thisdocNo
            coj_ss.header.thisdocNo = typecd + "_S.JSON";
            // update?
            var update = false;
            if (update)
            {
                // updateBy, updateAt
                coj_ss.header.updateBy = "jn-wtnb";
                coj_ss.header.updateBy = datestr;
            }
            else
            {
                // createBy, createAt
                coj_ss.header.createdBy = "jn-wtnb";
                coj_ss.header.createdAt = datestr;
            }
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - docs
            //++++++++++++++++++++++++++
            coj_ss.cejObject = new Coj.mst000003.CejObject();
            coj_ss.cejObject.coHeader = new Coj.mst000003.CoHeader();
            coj_ss.cejObject.coHeader.docs = new List<Coj.mst000003.Doc>();
            // 新機種展開表
            var doc_sinkishu = new Coj.mst000003.Doc();
            doc_sinkishu.name = "新機種展開表";
            doc_sinkishu.formno = procobj.procmastermodel.shinkishutenkai.formno.value;
            doc_sinkishu.released = procobj.procmastermodel.shinkishutenkai.released.value;
            doc_sinkishu.update = procobj.procmastermodel.shinkishutenkai.updateat.value;
            doc_sinkishu.revision = procobj.procmastermodel.shinkishutenkai.revision.value;
            coj_ss.cejObject.coHeader.docs.Add(doc_sinkishu);
            // 部品表
            var doc_buhinhyo = new Coj.mst000003.Doc();
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
            coj_ss.cejObject.coHeader.docs.Add(doc_buhinhyo);
            // 素子波長制限
            var doc_soshiseigen = new Coj.mst000003.Doc();
            doc_soshiseigen.name = "素子波長投入制限";
            doc_soshiseigen.formno = System.IO.Path.GetFileName(conf.makeprocjson.config.path.soshiseigenfile);
            doc_soshiseigen.released = GetCellValueAsString(ss_data, srl.soshiseigen.released.xlsaddress);
            doc_soshiseigen.update = GetCellValueAsString(ss_hist, srl.soshiseigen.updateat.xlscol, ss_hist.Rows.Count);
            doc_soshiseigen.revision = GetCellValueAsString(ss_hist, srl.soshiseigen.revision.xlscol, ss_hist.Rows.Count);
            coj_ss.cejObject.coHeader.docs.Add(doc_soshiseigen);
            //++++++++++++++++++++++++++
            // cejObject
            // - cojHeader
            //   - kishuprofile
            //++++++++++++++++++++++++++
            coj_ss.cejObject.coHeader.kishuprofile = new Coj.mst000003.Kishuprofile();
            coj_ss.cejObject.coHeader.kishuprofile.name = "照明HWLED_Ver.9";
            coj_ss.cejObject.coHeader.kishuprofile.foldername = "ver9";
            coj_ss.cejObject.coHeader.kishuprofile.revision = 1;
            //++++++++++++++++++++++++++
            // coList
            // "kishu_soshi_codes"
            //++++++++++++++++++++++++++
            coj_ss.cejObject.coList = new List<Coj.mst000003.CoList>();
            var colistobj = new Coj.mst000003.CoList();
            colistobj.function = "kishu_soshi_codes";
            colistobj.props = new Coj.mst000003.Props();
            colistobj.props.propname = "機種・素子コード";
            colistobj.props.propdata = new List<Coj.mst000003.Propdatum>();
            //機種名
            var propdata = new Coj.mst000003.Propdatum();
            propdata.meisho = "機種名";
            propdata.code = typecd;
            colistobj.props.propdata.Add(propdata);
            //素子名
            propdata = new Coj.mst000003.Propdatum();
            propdata.meisho = "素子名";
            propdata.code = GetCellValueAsString(ss_data, srl.soshiseigen.indexinfo.shiyousoshi.xlscol, i);
            colistobj.props.propdata.Add(propdata);
            //部品コード
            propdata = new Coj.mst000003.Propdatum();
            propdata.meisho = "部品コード";
            propdata.code = GetCellValueAsString(ss_data, srl.soshiseigen.indexinfo.buhinkodo.xlsaddress);
            colistobj.props.propdata.Add(propdata);
            //ソーティング部品コード
            propdata = new Coj.mst000003.Propdatum();
            propdata.meisho = "部品コード";
            propdata.code = GetCellValueAsString(ss_data, srl.soshiseigen.indexinfo.sbuhinkodo.xlscol, i);
            colistobj.props.propdata.Add(propdata);
            coj_ss.cejObject.coList.Add(colistobj);
            //++++++++++++++++++++++++++
            // coList
            // "hachou_rank"
            //++++++++++++++++++++++++++
            colistobj = new Coj.mst000003.CoList();
            colistobj.function = "hachou_rank";
            colistobj.props = new Coj.mst000003.Props();
            colistobj.props.propname = "波長ランクリスト";
            colistobj.props.propdata = new List<Coj.mst000003.Propdatum>();
            //素子ランク数ループする
            var soshiranksu = srl.soshiseigen.hachouinfo.hachourank.datasu;
            for (int s = 0; s < soshiranksu; s++)
            {
                propdata = new Coj.mst000003.Propdatum();
                var soshirankcol = srl.soshiseigen.hachouinfo.hachourank.datasta.xlscol + s;
                var soshirankrow = srl.soshiseigen.hachouinfo.hachourank.xlsrow;
                propdata.code = GetCellValueAsString(ss_data, soshirankcol, soshirankrow);
                var cejrankcol = srl.soshiseigen.hachouinfo.cejrank.datasta.xlscol + s;
                var cejrankrow = srl.soshiseigen.hachouinfo.cejrank.xlsrow;
                propdata.cejrank = GetCellValueAsString(ss_data, cejrankcol, cejrankrow);
                propdata.tounyukyoka = GetCellValueAsString(ss_data, soshirankcol, i);
                colistobj.props.propdata.Add(propdata);
            }
            coj_ss.cejObject.coList.Add(colistobj);

            //++++++++++++++++++++++++++
            // coList
            // "soting_rank"
            //++++++++++++++++++++++++++
            colistobj = new Coj.mst000003.CoList();
            colistobj.function = "soting_rank";
            colistobj.props = new Coj.mst000003.Props();
            colistobj.props.propname = "機種・素子コード";
            colistobj.props.propdata = new List<Coj.mst000003.Propdatum>();
            //素子ランク数ループする
            var sortingranksu = srl.soshiseigen.sortinginfo.sortingrank.datasu;
            for (int s = 0; s < sortingranksu; s++)
            {
                propdata = new Coj.mst000003.Propdatum();
                var sortingrankcol = srl.soshiseigen.sortinginfo.sortingrank.datasta.xlscol + s;
                var sortingrankrow = srl.soshiseigen.sortinginfo.sortingrank.xlsrow;
                propdata.sortingrank = GetCellValueAsString(ss_data, sortingrankcol, sortingrankrow);
                propdata.tounyukyoka = GetCellValueAsString(ss_data, sortingrankcol, i);
                colistobj.props.propdata.Add(propdata);
            }
            coj_ss.cejObject.coList.Add(colistobj);

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
            return exobj.GetCellValueAsString(col + row);
        }


        private string GetCellValueAsString(System.Data.DataTable exobj, string address)
        {
            var addarr = Regex.Split(address, @"(?<=[a-zA-Z])(?=\d)");
            var rownm = ToAlphabetIndex(addarr[0]) - 1;
            var colnm = int.Parse(addarr[1]) - 1;
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

        private string GetCellValueAsString(System.Data.DataTable exobj, decimal col, decimal row)
        {
            var rownm = (int)col - 1;
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
