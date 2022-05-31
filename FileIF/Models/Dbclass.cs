using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{

    ////////////////////////////////////
    /// 
    /// Arms使用により未使用化
    /// 
    ////////////////////////////////////
    ///
    /// DB_TABLEクラス 
    /// class名 = DB Table名
    ///
    ////////////////////////////////////

    public class Current_mag
    {
        public string Magno { get; set; }  //dbname: magno
        public string Product { get; set; }  //dbname: product
        public string Lotno { get; set; }  //dbname: lotno
        public string Lpcode { get; set; }  //dbname: last_pcode
        //public int Lpno { get; set; }
        public string Cstmproduct { get; set; }  //dbname: custom_product
        public string Cstmlotno { get; set; }  //dbname: custom_product
        public string Macno { get; set; }  //dbname: macno
        public string Io { get; set; }  //dbname: in_out
        public int Valbs { get; set; }  //dbname: val_bs
        private string henkaten = "NA";
        public string Henkaten { get { return henkaten; } set { if (value != "") henkaten = value; } }
        Tasks_Common tcommons = new Tasks_Common();

        public string[] SelectFromCurrent_magTable(int taskid, Mcfilesys fs, string Magno, ref Dictionary<string, string> Dict, ref string Dbgmsg)
        {
            string msg = "";
            try
            {
                var retDict = new Dictionary<string, string>(); //SQLのリターン格納用辞書
                MySQL sql = new MySQL();
                string query = $"SELECT * FROM Current_mag WHERE magno='{Magno}'";
                const int rcdlen = 1;

                if (!sql._SqlTask_Read(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, query, rcdlen, ref retDict, ref Dbgmsg))
                {
                    string mes = "マガジンテーブル(Current_mag)から情報が取得できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                Magno = retDict["magno"];
                Product = retDict["product"];
                Lotno = retDict["lotno"];
                Cstmproduct = retDict["custom_product"];
                Cstmlotno = retDict["custom_lotno"];
                Lpcode = retDict["last_pcode"];
                Macno = retDict["macno"];
                Io = retDict["in_out"];
                Valbs = int.Parse(retDict["val_bs"]);

                return new string[] { retkey.ok };
            }
            catch(Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
            }
        }
    }

    class Process_master
    {
        public string Product { get; set; }  //dbname: product
        public string Pcode { get; set; }  //dbname: pcode
        //public string Mno { get; set; }  // v1_2で削除
        public string Pcat { get; set; }  //dbname: pcat
        public int Pno { get; set; }  //dbname: pno
        //public string Mparam { get; set; }  // v1_2で削除

        public string[] SelectFromProcess_masterTable(int taskid, Mcfilesys fs, string[] Lproc, string Product, ref Dictionary<string, string> Dict, ref string Dbgmsg)
        {
            string msg = "";
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                var retDict = new Dictionary<string, string>(); //SQLのリターン格納用辞書
                MySQL sql = new MySQL();
                string query = "";
                if (Lproc[0] == "Pno")
                    query = $"SELECT * FROM Process_master WHERE product='{Product}' AND pno={Lproc[1]}"; 
                else if (Lproc[0] == "Pcode")
                    query = $"SELECT * FROM Process_master WHERE product='{Product}' AND pcode='{Lproc[1]}'";
                else
                {
                    string mes = "Queryの条件（Pno or Pcode）が設定されていません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                int rcdlen = 1;

                if (!sql._SqlTask_Read(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, query, rcdlen, ref retDict, ref Dbgmsg))
                {
                    string mes = "テーブル(Process_master)から情報が取得できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                Product = retDict["product"];
                Pcode = retDict["pcode"];
                Pcat = retDict["pcat"];
                Pno = int.Parse(retDict["pno"]);
                //Mparam = retDict["Mparam"];

                return new string[] { retkey.ok };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
            }
        }
    }


    class Mac_prams
    {
        public string Product { get; set; }  //dbname: product
        public string Cstmproduct { get; set; }  //dbname: custom_product
        public string Pcode { get; set; }  //dbname: pcode
        public string Macno { get; set; }  //dbname: macno
        public string Mparam { get; set; }  //dbname: mac_param
        Tasks_Common tcommons = new Tasks_Common();

        public string[] GetParamFromMacprocparamTable(int taskid, Mcfilesys fs, string Product, string Pcode, ref Dictionary<string, string> Dict, ref string Dbgmsg)
        {
            string msg = "";
            try
            {
                var retDict = new Dictionary<string, string>(); //SQLのリターン格納用辞書
                MySQL sql = new MySQL();
                string query = $"SELECT * FROM mac_params WHERE macno='{fs.Macno}' AND product='{Product}' AND pcode='{Pcode}'";

                int rcdlen = 1;

                if (!sql._SqlTask_Read(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, query, rcdlen, ref retDict, ref Dbgmsg))
                {
                    string mes = $"取得した条件/'{fs.Macno}'/'{Product}'/'{Pcode}'/でテーブル(mac_params)から情報が取得できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                Product = retDict["product"];
                Cstmproduct = retDict["custom_product"];
                Pcode = retDict["pcode"];
                Macno = retDict["macno"];
                Mparam = retDict["mac_param"];

                return new string[] { retkey.ok };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
            }
        }
    }


    public class Process_results
    {
        public string Product { get; set; }  //dbname: product
        public string Lotno { get; set; }  //dbname: lotno
        public string Pcode { get; set; }  //dbname: pcode
        public string Cstmproduct { get; set; }  //dbname: custom_product
        public string Cstmlotno { get; set; }  //dbname: custom_lotno
        public string Valin { get; set; }  //dbname: val_in
        public string Valout { get; set; } //dbname: val_out
        public string Valpass { get; set; }  //dbname: val_pass
        public string Valfail { get; set; }  //dbname: val_fail
        public string Magin { get; set; }  //dbname: magno_in
        public string Maginio { get; set; }  //dbname: is_mag_in_io
        public string Datein { get; set; }  //dbname: date_mag_in
        public string Magout { get; set; }  //dbname: magno_out
        public string Dateout { get; set; }  //dbname: date_mag_out
        public string Macno { get; set; }  //dbname: macno

        public string[] SelectFromProcess_resultsTable(int taskid, Mcfilesys fs, string Product, string Lotno, string Pcode, ref Dictionary<string, string> Dict, ref string Dbgmsg)
        {
            string msg = "";
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                var retDict = new Dictionary<string, string>(); //SQLのリターン格納用辞書
                MySQL sql = new MySQL();
                string query = $"SELECT * FROM Process_results WHERE product='{Product}' AND lotno='{Lotno}' AND pcode='{Pcode}' ";
                int rcdlen = 1;

                if (!sql._SqlTask_Read(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, query, rcdlen, ref retDict, ref Dbgmsg))
                {
                    string mes = "テーブル(Process_results)から基板数量が取得できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                Product = retDict["product"];
                Lotno = retDict["lotno"];
                Pcode = retDict["pcode"];
                Cstmproduct = retDict["custom_product"];
                Cstmlotno = retDict["custom_lotno"];
                Valin = retDict["val_in"];
                Valout = retDict["val_out"];
                Valpass = retDict["val_pass"];
                Valfail = retDict["val_fail"];
                Magin = retDict["magno_in"];
                Maginio = retDict["is_maginio"];
                Datein = retDict["date_mag_in"];
                Magout = retDict["magno_out"];
                Product = retDict["date_mag_out"];
                Macno = retDict["macno"];

                return new string[] { retkey.ok };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
            }
        }
    }

    class resincup_lots
    {
        public string Cstmproduct { get; set; }  //dbname: custom_product
        public string Cstmlotno { get; set; }  //dbname: custom_lotno
        public string Cupno { get; set; }  //dbname: cupno
    }

    class resincup_info
    {
        public string Cupno { get; set; }  //dbname: cupno
        public string Cstmproduct { get; set; }  //dbname: custom_product
        public string Cstmlotno { get; set; }  //dbname: custom_lotno
        public string Dierank { get; set; }  //dbname: led_rank
        public string Bdvol { get; set; }  //dbname: board_vol
        public int Result { get; set; }  //dbname: result
        public string Dt_haigo { get; set; }  //dbname: datetime_haigo
        public string Macno_haigo { get; set; }  //dbname:  macno_haigo
        public string Dt_kakuhan { get; set; }  //dbname: datetime_kakuhan
        public string Macno_kakuhan { get; set; }  //dbname: macno_kakuhan
        public string Kubun { get; set; }  //dbname: kubun
        public string Seikeiki { get; set; }  //dbname: seikeiki
        private string henkaten = "NA";
        public string Henkaten { get { return henkaten; } set { if (value != "") henkaten = value; } }  //dbname: henkaten
        private string recipe = "NA";
        public string Recipe { get { return recipe; } set { if (value != "") recipe = value; } }  //dbname: recipefile_name

        public Dictionary<string, string> resincupInfo { get; set; }

        public void updateResinCCupInfo()
        {
            this.resincupInfo = new Dictionary<string, string>()
            {
                {"cupno", Cupno},
                {"productnm", Cstmproduct},
                {"ledrank", Dierank},
                {"mixtypecd", "1"},
                {"result", Result.ToString()},
                {"kubun", Kubun},
                {"seikeiki", Seikeiki},
                {"henkaten", Henkaten},
                {"recipefilenm", Recipe},
                {"bdqty", Bdvol}
            };
        }

    }

    class recipe_info
    { 
        public string Recipe { get; set; }  //dbname: recipefile_name
        public string Mname { get; set; }  //dbname: mac_name
        public string Dtin { get; set; }  //dbname: date_in
        public int Getsta { get; set; }  //dbname: get_sta
        public string Dtsta { get; set; }  //dbname: date_sta
    }

}
