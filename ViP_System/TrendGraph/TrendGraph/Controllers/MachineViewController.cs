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
using System.Web.Helpers;

namespace TrendGraph.Controllers
{
    public class MachineViewController : Controller
    {
        // GET: MachineView
        public ActionResult Index()
        {
            return View();
        }

        private List<MachineList> GetMachineList()
        {
            List<MachineList> machines = new List<MachineList>();

            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT Distinct macno, macname FROM TnDBTrend";
                //部分一致検索用
                

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineList t = new MachineList();
                        t.macno = SQLite.ParseString(reader["macno"]);
                        t.macname = SQLite.ParseString(reader["macname"]);

                        machines.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }


            //machines.Add(new MachineList { macno = "1" });
            return machines;
        }

        private List<GraphData> GetGraphData()
        {
            //初回のみ
            List<GraphData> GD = new List<GraphData>();

            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "%"));
                //部分一致検索用


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GraphData t = new GraphData();
                        t.x_data = SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue;
                        t.y_data = SQLite.ParseSingle(reader["shigma"]);

                        GD.Add(t);
                        break;
                    }
                }
                cmd.Parameters.Clear();
            }

            //GD.Add(new GraphData { x_data = DateTime.Now, y_data = "3" });
            return GD;
        }

        private List<GraphData> GetGraphData(string macno)
        {
            List<GraphData> GD = new List<GraphData>();

            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", macno));
                //部分一致検索用


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GraphData t = new GraphData();
                        t.x_data = SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue;
                        t.y_data = SQLite.ParseSingle(reader["shigma"]);

                        GD.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }

            //GD.Add(new GraphData { x_data = DateTime.Now, y_data = "3" });
            return GD;
        }

        private List<CurrentMacno> GetCurrentMacno(string macno)
        {
            List<CurrentMacno> CM = new List<CurrentMacno>();

            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT macname FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", macno));
                //部分一致検索用


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CurrentMacno t = new CurrentMacno();
                        
                        t.current_macno = SQLite.ParseString(reader["macname"]);

                        CM.Add(t);
                        break;
                    }
                }
                cmd.Parameters.Clear();
            }

            return CM;
        }

        public ActionResult List()
        {
            MultiList ML = new MultiList();

            //ドロップダウンメニュー選択用の設備リスト取得
            ML.MachineList = GetMachineList();

            //グラフデータ取得
            ML.GraphData = GetGraphData();

            //現在選択中の設備名称取得
            ML.CurrentMacno = GetCurrentMacno("%");

            return View("List", ML);

        }

        //ボタン押下時
        [HttpPost]
        public ActionResult machinenum(String macno)
        {
            MultiList ML = new MultiList();

            //ドロップダウンメニュー選択用の設備リスト取得
            ML.MachineList = GetMachineList();

            //グラフデータ取得
            ML.GraphData = GetGraphData(macno);

            //現在選択中の設備名称取得
            ML.CurrentMacno = GetCurrentMacno(macno);
            
            return View("List", ML);
        }
    }
}