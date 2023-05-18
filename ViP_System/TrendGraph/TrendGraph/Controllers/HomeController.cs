using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrendGraph.Models;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json.Linq;
using ArmsWebApi;
//using CsvHelper;

namespace TrendGraph.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            //Configファイルから接続文字列等取得
            TrendGraph.Config.LoadSetting();
            //string test = Config.Settings.LocalConnString;
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult WorkStart()
        {
            //ViewBag.Message = "Your application description page.";
            return View();
        }

        [HttpPost]
        public ActionResult Input(String employee, String machine, String matlabel, String planlotno, String Title)
        {                     
            string magno = matlabel;
            string msg;

            if (magno == "" || machine == "" || employee == "" || planlotno == "")
            {
                ViewData["MatLabel"] = "フォームへの入力不足があります";
                return View("WorkStart");
            }         

            string constr;
            
            //読み込んだ設備の作業番号
            int macproc;
            string nextproc_name = "";           

            if (employee.Substring(0, 2) == "01")
            {
                employee = employee.Substring(employee.IndexOf(" ")).Trim();
            }
            else
            {
                ViewData["MatLabel"] = "社員バーコード：ヘッダー文字列が異常です";
                return View("WorkStart");
            }            

            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";
            //ローカルデバッグ
            //constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=MDWork;Integrated Security=true";

            //Arms接続
            //constr_arms = "server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=ARMS_VSP;UID=inline;PWD=R28uHta;database=ARMS_VSP;Max Pool Size=100";

            //設備が作業できるprocnoがあるかチェック
            List<Promac> retv = new List<Promac>();

            //Tranリスト
            List<Tran> tran_list = new List<Tran>();
            
            //WorkFlow
            List<WorkFlow> workflow_list = new List<WorkFlow>();
            List<WorkFlow> workflow_last = new List<WorkFlow>();

            List<string> magnoarr = new List<string>();

            //最新の完了している作業番号
            int now_procno;
            //ワークフローに従って決まっている次の作業番号
            int next_procno;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //magnoからどこまでの作業が完了しているか確かめる
                cmd.CommandText = "SELECT procno, mixtypecd FROM TnPMMSTran_BKU where lotno like @lotno order by procno desc";
                //cmd.Parameters.Add(new SqlParameter("@magno", magno));
                cmd.Parameters.Add(new SqlParameter("@lotno", planlotno));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tran t = new Tran();
                        //降順に並べているので、最初の1回でbreak ※どうもローカル変数だと比較ができない?
                        //t.lotno = SQLite.ParseString(reader["lotno"]);
                        t.procno = SQLite.ParseInt(reader["procno"]);

                        t.mixtypecd = SQLite.ParseString(reader["mixtypecd"]);

                        tran_list.Add(t);
                        break;
                    }
                }
                cmd.Parameters.Clear();

                //計画ロットNoの実績がない場合
                if (tran_list.Count == 0)
                {
                    //実績がないので、初工程の登録の可能性を探索                   

                    //設備とmixtypecdからどの作業ができるのかを抽出
                    cmd.CommandText = "SELECT procno, mixtypecd FROM TmPMMSProMac where macno like @MN";
                    cmd.Parameters.Add(new SqlParameter("@MN", machine));
                    //この時点だとmixtypecd(作業区分)が分からないので、定義不可　初工程から別のmixtypecdを定義することはできない
                    //cmd.Parameters.Add(new SqlParameter("@MTC", tran_list[0].mixtypecd));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Promac m = new Promac();
                            m.procno = SQLite.ParseInt(reader["procno"]);
                            m.mixtypecd = SQLite.ParseString(reader["mixtypecd"]);
                            retv.Add(m);
                        }
                    }
                    cmd.Parameters.Clear();

                    //作業できる設備なければリターンしてメッセージを出す
                    if (retv.Count == 0)
                    {
                        ViewData["MatLabel"] = "入力された設備[" + machine + "]は作業できる工程がありません";
                        return View("WorkStart");
                    }                 

                    //WorkFlowから初工程であることを確認する
                    cmd.CommandText = "SELECT workorder FROM TmPMMSWorkFlow where procno like @procno and typecd like @MXType order by procno desc";
                    cmd.Parameters.Add(new SqlParameter("@procno", retv[0].procno));
                    cmd.Parameters.Add(new SqlParameter("@MXType", retv[0].mixtypecd));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkFlow wf_now = new WorkFlow();
                            wf_now.workorder = SQLite.ParseInt(reader["workorder"]);
                            workflow_list.Add(wf_now);
                            break;
                        }
                    }
                    cmd.Parameters.Clear();

                    //作業できる設備なければリターンしてメッセージを出す
                    if (workflow_list[0].workorder != 1)
                    {
                        ViewData["MatLabel"] = "入力された設備[" + machine + "]は初工程投入用ではありません.";
                        return View("WorkStart");
                    }

                    //ここまで来たら、設備○

                    

                    //ここでArmsWebdll 開始処理挿入

                    ////////////////////////
                    //②開始
                    ////////////////////////
                    
                    ArmsWebApi.WorkStart ws;
                    //ws = new ArmsWebApi.WorkStart("TCLDC01", "APP", magno);
                    ws = new ArmsWebApi.WorkStart("TCLDC01", "APP", "J00001");

                    bool success_cbs = ws.CheckBeforeStart(out msg);

                    if (success_cbs)
                    {
                        //OK 登録前確認完了                        
                    }
                    else
                    {                      
                        ViewData["MatLabel"] = "Armsの開始前チェックでNG" + "<br/>" + "メッセージ:" + msg;
                        return View("WorkStart");
                    }

                    bool success_in = ws.Start(out msg);

                    if (success_in)
                    {
                        //OK 開始実績登録完了
                    }
                    else
                    {
                        ViewData["MatLabel"] = "Armsの開始登録時にNG" + "<br/>" + "メッセージ:" + msg;
                        return View("WorkStart");
                    }

                    //新規でレコード登録 TnPMMSTran_BKU
                    cmd.CommandText = cmd.CommandText = @"
                            INSERT
                             INTO TnPMMSTran_BKU(magno
	                            , lotno
                                , procno
                                , mixtypecd
	                            , macno
                                , dtend
                                , Employee)
                            values(@magno
	                            , @lotno
                                , @procno
                                , @mixtypecd
	                            , @macno
                                , @dtend
                                , @Employee)";

                    cmd.Parameters.Add(new SqlParameter("@magno", magno));
                    cmd.Parameters.Add(new SqlParameter("@lotno", planlotno));
                    cmd.Parameters.Add(new SqlParameter("@procno", retv[0].procno));
                    cmd.Parameters.Add(new SqlParameter("@mixtypecd", retv[0].mixtypecd));//初登録時のため、Procから取得したmixtypecdを使う                  
                    cmd.Parameters.Add(new SqlParameter("@macno", machine));
                    cmd.Parameters.Add(new SqlParameter("@dtend", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@Employee", employee));

                    cmd.ExecuteNonQuery();



                    ViewData["MatLabel"] = "マガジン番号：" + magno + ",計画ロットNo：" + planlotno + "<br/>の初工程実績を登録しました.";
                    return View("WorkStart");
                }

                //設備とmixtypecdからどの作業ができるのかを抽出
                cmd.CommandText = "SELECT procno, macno FROM TmPMMSProMac where macno like @MN and mixtypecd like @MTC order by macno asc";
                cmd.Parameters.Add(new SqlParameter("@MN", machine));
                cmd.Parameters.Add(new SqlParameter("@MTC", tran_list[0].mixtypecd));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Promac m = new Promac();
                        m.procno = SQLite.ParseInt(reader["procno"]);
                        m.macno = SQLite.ParseString(reader["macno"]);

                        retv.Add(m);
                    }
                }

                //作業できる設備なければリターンしてメッセージを出す
                if (retv.Count == 0)
                {
                    ViewData["MatLabel"] = "入力された設備[" + machine + "]は作業できる工程がありません";
                    return View("WorkStart");
                }

                //ここまで来たら、設備○

                cmd.Parameters.Clear();

                //20220215変更

                //読み込まれた設備のProcnoと実績のProcno比較　※1つの設備に複数のProcnoがあることは想定していない
                macproc = retv[0].procno;
               
                //設備の作業番号と最新の完了済み作業番号が同じ場合
                if (macproc == tran_list[0].procno)
                {
                    ViewData["MatLabel"] = "計画ロットNo：" + planlotno + "は" + "<br/>" + "対象設備：" + machine + "での作業が完了しています";                   
                    return View("WorkStart");
                }

                //※ここまで　初工程ではない、入力された設備は何かしらの作業はできることまで確認

                //以下をTmPMMSWorkFlowを読み込んで、作業順があっているかを確認して作業登録
                cmd.CommandText = "SELECT workorder FROM TmPMMSWorkFlow where procno like @procno and typecd like @MXType order by procno desc";
                cmd.Parameters.Add(new SqlParameter("@procno", tran_list[0].procno));
                cmd.Parameters.Add(new SqlParameter("@MXType", tran_list[0].mixtypecd));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WorkFlow wf_now = new WorkFlow();
                        wf_now.workorder = SQLite.ParseInt(reader["workorder"]);
                        workflow_list.Add(wf_now);
                        break;
                    }
                }
                cmd.Parameters.Clear();
               
                //次作業Noを抽出
                cmd.CommandText = "SELECT procno, workname FROM TmPMMSWorkFlow where workorder like @workorder and typecd like @MXType order by procno desc";
                cmd.Parameters.Add(new SqlParameter("@workorder", workflow_list[0].workorder + 1));
                cmd.Parameters.Add(new SqlParameter("@MXType", tran_list[0].mixtypecd));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WorkFlow wf_next = new WorkFlow();
                        wf_next.procno = SQLite.ParseInt(reader["procno"]);
                        wf_next.workname = SQLite.ParseString(reader["workname"]);
                        workflow_list.Add(wf_next);
                        break;
                    }
                }
                cmd.Parameters.Clear();

                //次の作業がない場合のエラー回避
                if (workflow_list.Count >= 2)
                {
                    next_procno = workflow_list[1].procno;
                    nextproc_name = workflow_list[1].workname;
                    now_procno = tran_list[0].procno;
                }
                else
                {
                    ViewData["MatLabel"] = "計画ロットNo：" + planlotno + "は最終作業まで完了しています";                    
                    return View("WorkStart");
                }

                //設備の作業番号と実績の作業番号が異なっている場合　※まだ対象設備で作業できていないメッセージ
                //ここに来たら、読み込んだ調合結果IDは最終工程まで終わっていない状態 その状態で、読み込んだ設備に対する作業番号と次の作業番号を比較
                /*else*/
                if (macproc != next_procno)
                {                                  
                    ViewData["MatLabel"] = "この設備では次工程:" + nextproc_name + "は実施できません.";
                    return View("WorkStart");
                }

                //正常判定　実績に対して次工程の設備を読み込んでいる場合
                else if (macproc == next_procno)
                {                   
                    //PMMSTranにレコード作成
                    cmd.CommandText = cmd.CommandText = @"
                            INSERT
                             INTO TnPMMSTran_BKU(magno
	                            , lotno
                                , procno
                                , mixtypecd
	                            , macno
                                , dtend
                                , Employee)
                            values(@magno
	                            , @lotno
                                , @procno
                                , @mixtypecd
	                            , @macno
                                , @dtend
                                , @Employee)";

                    cmd.Parameters.Add(new SqlParameter("@magno", magno));
                    cmd.Parameters.Add(new SqlParameter("@lotno", planlotno));
                    cmd.Parameters.Add(new SqlParameter("@procno", next_procno));
                    cmd.Parameters.Add(new SqlParameter("@mixtypecd", tran_list[0].mixtypecd));
                    cmd.Parameters.Add(new SqlParameter("@macno", machine));
                    cmd.Parameters.Add(new SqlParameter("@dtend", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@Employee", employee));

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT workname FROM TmPMMSWorkFlow where typecd like @tc and procno like @pn";
                    cmd.Parameters.Add(new SqlParameter("@tc", tran_list[0].mixtypecd));
                    cmd.Parameters.Add(new SqlParameter("@pn", next_procno));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nextproc_name = SQLite.ParseString(reader["workname"]); ;
                        }
                    }
                    cmd.Parameters.Clear();
                  
                    //登録した作業が最終工程なら、ArmsDBへ書き込み処理を行う                    
                    cmd.CommandText = "SELECT procno FROM TmPMMSWorkFlow where workorder like @workorder and typecd like @MXType order by procno desc";
                    cmd.Parameters.Add(new SqlParameter("@workorder", workflow_list[0].workorder + 2));
                    cmd.Parameters.Add(new SqlParameter("@MXType", tran_list[0].mixtypecd));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkFlow wf_last = new WorkFlow();
                            wf_last.procno = SQLite.ParseInt(reader["procno"]);
                            workflow_last.Add(wf_last);
                            break;
                        }
                    }
                    cmd.Parameters.Clear();
                    con.Close();

                    //20220304　書き込むときに1:調合完了時間+240と3:遠心撹拌完了時間+300のうち、短い期限の時間をTnResinMix->stirringlimitに設定
                    //20220304　新規でTnResinMixに待機時間の列を設定して、Now+60minをデータ挿入する

                    //次の作業がない場合(最終作業工程)のみArmsDBへデータを書き込む
                    if (workflow_last.Count == 0)
                    {
                        //ここでArmsWebdll 完了処理挿入


                        ////////////////////////
                        //③完了
                        ////////////////////////
                        msg = "";

                        //mixtypecd=21ならダイシング関連作業
                        if (tran_list[0].mixtypecd == "21")
                        {
                            magnoarr.Add(magno);
                            if (ArmsWebApi.LdcBlend.mkblendlot("TCLDC01", magnoarr, ref msg))
                            {

                            }
                            else
                            {
                                ViewData["MatLabel"] = "Arms完了処理でNG" + "<br/>" + "メッセージ:" + msg;
                                return View("WorkStart");
                            }
                        }
                        else //その他工程作業の完了
                        {
                            string outmagcode = magno;
                            ArmsWebApi.WorkEnd we;
                            we = new ArmsWebApi.WorkEnd("TCLDC01", employee, magno, outmagcode);
                            //開始と完了が同じmagnoにしてあるが、もしかしたら初工程で開始したmagnoをDBから取得してmagnoにしないといけない?

                            //不良登録は処理なし
                            /*if (we.RegisterDefects(out msg, Defectdict))
                            {
                                //Console.WriteLine("不良登録完了。\r\n");
                            }
                            else
                            {
                                ViewData["MatLabel"] = "不良登録でNG" + "<br/>" + "メッセージ:" + msg;
                                return View("WorkStart");
                            }*/

                            if (we.End(out msg))
                            {
                                //完了登録処理完了
                            }
                            else
                            {
                                ViewData["MatLabel"] = "Arms完了処理でNG" + "<br/>" + "メッセージ:" + msg;
                                return View("WorkStart");
                            }
                        }
                    }
                    //メッセージ表示
                    ViewData["MatLabel"] = "計画ロットNo：" + planlotno + "に対して、" + nextproc_name + "作業が正常に登録されました.";
                    return View("WorkStart");
                }
            }
            return View("WorkStart");
        }

        public ActionResult WorkHistory()
        {
            string constr;
            constr = "server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            List<Tran> tran_list = new List<Tran>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT magno, lotno, procno, mixtypecd, macno, dtend, Employee FROM TnPMMSTran_BKU order by dtend desc";
                //cmd.Parameters.Add(new SqlParameter("@RMId", magno));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tran t = new Tran();

                        t.magno = SQLite.ParseString(reader["magno"]);
                        t.lotno = SQLite.ParseString(reader["lotno"]);
                        t.procno = SQLite.ParseInt(reader["procno"]);                                              
                        t.mixtypecd = SQLite.ParseString(reader["mixtypecd"]);                       
                        t.macno = SQLite.ParseString(reader["macno"]);                        
                        t.dtend = SQLite.ParseDate(reader["dtend"]) ?? DateTime.MinValue;
                        t.Employee = SQLite.ParseString(reader["Employee"]);

                        tran_list.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }
            return View(tran_list);
        }

        [HttpPost]
        public ActionResult ResinIdFilter(String magno)
        {
            string constr;
            constr = "server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            List<Tran> tran_list = new List<Tran>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT magno, lotno, procno, mixtypecd, macno, dtend, Employee FROM TnPMMSTran_BKU where magno like @magno order by dtend desc";
                //部分一致検索用
                cmd.Parameters.Add(new SqlParameter("@magno", magno + "%"));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tran t = new Tran();

                        t.magno = SQLite.ParseString(reader["magno"]);
                        t.lotno = SQLite.ParseString(reader["lotno"]);
                        t.procno = SQLite.ParseInt(reader["procno"]);                                               
                        t.mixtypecd = SQLite.ParseString(reader["mixtypecd"]);
                        t.macno = SQLite.ParseString(reader["macno"]);                        
                        t.dtend = SQLite.ParseDate(reader["dtend"]) ?? DateTime.MinValue;
                        t.Employee = SQLite.ParseString(reader["Employee"]);
                        tran_list.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }
            /*using (var writer = new CsvHelper.CsvWriter(@"hoge.csv"))
            {
                writer.Write(tran_list);
            }*/


            return View("WorkHistory",tran_list);
        }

        public static bool WorkTimeCheck(int procfrom, int procto, string constr, string magno, ref string msg) 
        {
            //procfrom:一つ前の作業番号　procto:現在の作業番号 constr:データベース接続文字列
            int fwtime=0;
            int ftetime=0;
            int data_count=0;
            DateTime dtend = DateTime.Now;
            DateTime Test;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //時間管理テーブルからprocに応じた内容を調べる
                cmd.CommandText = "SELECT fromwaittime,fromtoendtime FROM TmWorkRegulation where procfrom like @pf and procto like @pt and magno like @magno";
                cmd.Parameters.Add(new SqlParameter("@pf", procfrom));
                cmd.Parameters.Add(new SqlParameter("@pt", procto));
                cmd.Parameters.Add(new SqlParameter("@magno", magno));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fwtime = SQLite.ParseInt(reader["fromwaittime"]);
                        ftetime = SQLite.ParseInt(reader["fromtoendtime"]);
                        data_count = reader.FieldCount;
                    }
                }
                cmd.Parameters.Clear();

                //データ数が0個の場合は時間監視なしなので、Trueを返して終了
                if (data_count == 0) 
                {
                    return true;
                }

                //データ数あった場合は作業間での時間管理があるので、それを満たしているかを判定してTrue or Falseを返す
                //dtend + fromwaittime < Now -> True
                //dtend + fromtoendtime > Now -> True
                cmd.CommandText = "SELECT dtend FROM TnPMMSTran_BKU where procno like @procno and magno like @magno";
                cmd.Parameters.Add(new SqlParameter("@procno", procfrom));
                cmd.Parameters.Add(new SqlParameter("@magno", magno));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dtend = SQLite.ParseDate(reader["dtend"]) ?? DateTime.MinValue;
                    }
                }
                cmd.Parameters.Clear();

                Test = dtend.AddMinutes(ftetime);
                if (dtend.AddMinutes(ftetime) > DateTime.Now && DateTime.Now > dtend.AddMinutes(fwtime))
                {
                    return true;
                }
                else
                {
                    if (ftetime == 0 && procfrom == 3 && procto == 2)
                    {
                        msg = "再撹拌は禁止されています";
                    }
                    else
                        msg = "作業許可時間外です." + "<br/>" + "開始可能時間:" + dtend.AddMinutes(fwtime).ToString() + "<br/>" +"期限時間:" + dtend.AddMinutes(ftetime).ToString();

                    return false;
                }
                    
                    
            }

                //return true;
        }


            /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public ActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }*/
    }
}
