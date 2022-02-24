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

namespace ProcMasterIF
{
    public partial class FmProcMasterIF : Form
    {
        string crlf = "\r\n";
        string msg;
        string configpath = @"C:\Oskas\procmaster\config.json";
        ConfigRoot conf;
        SLDocument sl;
        public bool interLock = false;

        public FmProcMasterIF()
        {
            InitializeComponent();

            var configstr = string.Empty;
            if (!readJson(configpath, ref configstr))
            {
                ConsoleShow("config.jsonが正常に読み込めませんでした", 3);
                return;
            }

            conf = JsonConvert.DeserializeObject<ConfigRoot>(configstr);

            // (とりあえず)表示処理
            txt_shinkishutenkai.Text = conf.makeprocjson[0].config[0].path.shinkishutenkai_file;
            txt_buhinhyou.Text = conf.makeprocjson[0].config[0].path.buhinhyou_folder;
            txt_procjson_hankan.Text = conf.makeprocjson[0].config[0].path.procjson_folder.hankan;
            txt_procjson_kansei.Text = conf.makeprocjson[0].config[0].path.procjson_folder.kansei;
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

                Task readHankan = Task.Run(() =>
                {
                    for (var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++)
                    {
                        var procobj_hankan = new MastermodelRoot();
                        var procobj_kansei = new MastermodelRoot();

                        var hankan = sl.GetCellValueAsString("A" + i);
                        var kansei = sl.GetCellValueAsString("B" + i);

                        ConsoleShow("hankan-" + i + "=" + hankan, 1);
                        ConsoleShow("kansei-" + i + "=" + kansei, 1);

                        var jsonmodelpath_hankan = conf.makeprocjson[0].config[0].defaultmodel.hankan;
                        var jsonmodelpath_kansei = conf.makeprocjson[0].config[0].defaultmodel.kansei;

                        foreach (var model in conf.makeprocjson[0].config[0].models)
                        {
                            if (kansei.Contains(model.typekey))
                            {
                                jsonmodelpath_hankan = model.hankan;
                                jsonmodelpath_kansei = model.kansei;
                                break;
                            }
                        }
                        
                        // ProcJson作成
                        if(!makeProcJson(jsonmodelpath_hankan, i, ref procobj_hankan))
                        {
                            interLock = false;
                            return;
                        }
                        if (!makeProcJson(jsonmodelpath_kansei, i, ref procobj_kansei))
                        {
                            interLock = false;
                            return;
                        }

                        procobj_hankan.procmastermodel.typecd = procobj_hankan.procmastermodel.shinkishutenkai.typeinfo.hankan.value;
                        procobj_kansei.procmastermodel.typecd = procobj_kansei.procmastermodel.shinkishutenkai.typeinfo.kansei.value;

                        // Jsonファイルに書き出し
                        var procjsonfolder_hankan = conf.makeprocjson[0].config[0].path.procjson_folder.hankan;
                        var procjsonfolder_kansei = conf.makeprocjson[0].config[0].path.procjson_folder.kansei;

                        if (!string.IsNullOrEmpty(procjsonfolder_hankan))
                        {
                            var procjsonfolder = procjsonfolder_hankan + "\\" + procobj_hankan.procmastermodel.typecd + ".json";
                            //if (!System.IO.File.Exists(jsonpath) )
                            //{
                            //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj, ref msg);
                            //}
                            Oskas.CommonFuncs.JsonFileWriter(procjsonfolder, procobj_hankan, ref msg);
                        }

                        if (!string.IsNullOrEmpty(procjsonfolder_kansei))
                        {
                            var procjsonfolder = procjsonfolder_kansei + "\\" + procobj_kansei.procmastermodel.typecd + ".json";
                            //if (!System.IO.File.Exists(jsonpath) )
                            //{
                            //    Oskas.CommonFuncs.JsonFileWriter(jsonpath, procobj, ref msg);
                            //}
                            Oskas.CommonFuncs.JsonFileWriter(procjsonfolder, procobj_kansei, ref msg);
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

                numericUpDown1Set(10);
                numericUpDown2Set(10);
            });
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            txt_typecd_kansei_start.Text = sl.GetCellValueAsString("B" + numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            txt_typecd_kansei_end.Text = sl.GetCellValueAsString("B" + numericUpDown2.Value);
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


        private bool makeProcJson(string jsonmodelpath, decimal row, ref MastermodelRoot procobj)
        {
            var jsonstr = string.Empty;

            if (!readJson(jsonmodelpath, ref jsonstr))
            {
                procobj = null;
                return false;
            }

            procobj = JsonConvert.DeserializeObject<MastermodelRoot>(jsonstr);

            //////////////////////
            //新機種展開表を読込
            //////////////////////
            ///
            var skt = procobj.procmastermodel.shinkishutenkai;
            
            //formo
            if (skt.formno.value == "")
            {
                skt.formno.value = sl.GetCellValueAsString(skt.formno.address);
            }

            //released
            if (skt.released.value == "")
            {
                var dt = Double.Parse(sl.GetCellValueAsString(skt.released.address));
                skt.released.value = DateTime.FromOADate(dt).ToString("d");
            }

            //updateat
            if (skt.updateat.value == "")
            {
                var dt = Double.Parse(sl.GetCellValueAsString(skt.updateat.address));
                skt.updateat.value = DateTime.FromOADate(dt).ToString("d");
            }

            //revision
            if (skt.revision.value == "")
            {
                skt.revision.value = sl.GetCellValueAsString(skt.revision.address);
            }

            //typeinfo.hankan
            if (skt.typeinfo.hankan.value == "")
            {
                skt.typeinfo.hankan.value = sl.GetCellValueAsString(skt.typeinfo.hankan.col + row);
            }

            //typeinfo.kansei
            if (skt.typeinfo.kansei.value == "")
            {
                skt.typeinfo.kansei.value = sl.GetCellValueAsString(skt.typeinfo.kansei.col + row);
            }

            //typeinfo.kyakusaki
            if (skt.typeinfo.kyakusaki.value == "")
            {
                skt.typeinfo.kyakusaki.value = sl.GetCellValueAsString(skt.typeinfo.kyakusaki.col + row);
            }

            //lotinfo.torikosu
            if (skt.lotinfo.torikosu.value == "")
            {
                skt.lotinfo.torikosu.value = sl.GetCellValueAsString(skt.lotinfo.torikosu.col + row);
            }

            //lotinfo.maisu_lot
            if (skt.lotinfo.maisu_lot.value == "")
            {
                skt.lotinfo.maisu_lot.value = sl.GetCellValueAsString(skt.lotinfo.maisu_lot.col + row);
            }

            //lotinfo.pkg_lot
            if (skt.lotinfo.pkg_lot.value == "")
            {
                skt.lotinfo.pkg_lot.value = sl.GetCellValueAsString(skt.lotinfo.pkg_lot.col + row);
            }

            //etc.buhinhyou.zuban.jp
            if (skt.etc.buhinhyou.zuban.jp.value == "")
            {
                skt.etc.buhinhyou.zuban.jp.value = sl.GetCellValueAsString(skt.etc.buhinhyou.zuban.jp.col + row);
            }

            //etc.buhinhyou.zuban.ch
            if (skt.etc.buhinhyou.zuban.ch.value == "")
            {
                skt.etc.buhinhyou.zuban.ch.value = sl.GetCellValueAsString(skt.etc.buhinhyou.zuban.ch.col + row);
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
                        material.name.value = slbom.GetCellValueAsString(material.name.address);
                    }
                    else
                    {
                        if (material.name.value != slbom.GetCellValueAsString(material.name.address))
                        {
                            ConsoleShow("部品表から取得した部品コードが基底モデルと違っています", 2);
                            ConsoleShow("部品表：" + slbom.GetCellValueAsString(material.name.address), 2);
                            ConsoleShow("基底モデル：" + material.name.value, 2);
                            return false;
                        }
                    }

                    if (material.code.value == "")
                    {
                        material.code.value = slbom.GetCellValueAsString(material.code.address);
                    }
                    else
                    {
                        if (material.code.value != slbom.GetCellValueAsString(material.code.address))
                        {
                            ConsoleShow("部品表から取得した部品コードが基底モデルと違っています", 2);
                            ConsoleShow("部品表：" + slbom.GetCellValueAsString(material.code.address), 2);
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
    }

}
