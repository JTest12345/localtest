using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using HighchartSample.Model;

namespace HighchartSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<MachineName> machines = new List<MachineName>();
            

            //ドロップダウンメニュー選択用の設備リスト取得
            machines = GetMachineList();
            string macno = "";

            CreateLineChart(macno);
            CreateLineChart2(macno);

            return View("index",machines);
        }

        private string GetCurrentMacno(string macno)
        {
            string CM = "";

            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"]; ;

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
                        CM = SQLite.ParseString(reader["macname"]);
                        break;
                    }
                }
                cmd.Parameters.Clear();
            }

            return CM;
        }

        public void CreateLineChart(string macno)
        {
            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];
            List<string> x = new List<string>();
            List<object> y101301 = new List<object>();
            List<object> y101302 = new List<object>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma, macno FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "101301"));
                //部分一致検索用

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {                      
                        x.Add((SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue).ToString());
                        y101301.Add(SQLite.ParseSingle(reader["shigma"]).ToString());
                        //y.Add(SQLite.ParseString(reader["macno"]));
                    }
                }
                cmd.Parameters.Clear();


                cmd.CommandText = "SELECT dbdate, shigma, macno FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "101302"));
                //部分一致検索用

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        y101302.Add(SQLite.ParseSingle(reader["shigma"]).ToString());
                        //y.Add(SQLite.ParseString(reader["macno"]));
                    }
                }
                cmd.Parameters.Clear();
            }           

            Highcharts chart = new Highcharts("linechart")
                .InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line,
                    BackgroundColor = new BackColorOrGradient(
                                                    new Gradient
                                                    {
                                                        LinearGradient = new[] { 0, 0, 0, 400 }
                                                       ,
                                                        Stops = new object[,]
                                                               {
                                                                    {   0, System.Drawing.Color.White }
                                                                    , {1, "#C7DFF5" }
                                                               }
                                                    }),
                    Height = 400
                })
                .SetTitle(new Title
                {
                    Text = "DB傾向監視",
                    Align = HorizontalAligns.Center,
                    VerticalAlign = VerticalAligns.Top
                })
                .SetXAxis(new XAxis
                {                   
                    Categories = x.ToArray()
                    //new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
                })
                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "Shigma(σ)" },
                    PlotLines = new[] { new YAxisPlotLines { Value = 0, Width = 1, Color = System.Drawing.Color.AliceBlue } }
                })
                .SetTooltip(new Tooltip { ValueSuffix = "σ" })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical, Align = HorizontalAligns.Right, VerticalAlign = VerticalAligns.Middle, BorderWidth = 0
                })
                .SetSeries(new [] 
                {

                    new Series {
                        Name = GetCurrentMacno("101301"),
                        Data = new Data(y101301.ToArray())//new object[] { 7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6 })
                    },
                    new Series {
                        Name = GetCurrentMacno("101302"),
                        Data = new Data(y101302.ToArray())//new object[] { 7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6 })
                    }
                });
            ViewData.Add("lineChart", chart.ToHtmlString());
        }

        public void CreateLineChart2(string macno)
        {
            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];
            List<string> x = new List<string>();
            List<object> y101303 = new List<object>();
            List<object> y101304 = new List<object>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma, macno FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "101303"));
                //部分一致検索用

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        x.Add((SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue).ToString());
                        y101303.Add(SQLite.ParseSingle(reader["shigma"]).ToString());
                        //y.Add(SQLite.ParseString(reader["macno"]));
                    }
                }
                cmd.Parameters.Clear();


                cmd.CommandText = "SELECT dbdate, shigma, macno FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "101304"));
                //部分一致検索用

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        y101304.Add(SQLite.ParseSingle(reader["shigma"]).ToString());
                        //y.Add(SQLite.ParseString(reader["macno"]));
                    }
                }
                cmd.Parameters.Clear();
            }

            Highcharts chart = new Highcharts("linechart2")
                .InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line,
                    BackgroundColor = new BackColorOrGradient(
                                                    new Gradient
                                                    {
                                                        LinearGradient = new[] { 0, 0, 0, 400 }
                                                       ,
                                                        Stops = new object[,]
                                                               {
                                                                    {   0, System.Drawing.Color.White }
                                                                    , {1, "#C7DFF5" }
                                                               }
                                                    }),
                    Height = 400
                })
                .SetTitle(new Title
                {
                    Text = "DB傾向監視",
                    Align = HorizontalAligns.Center,
                    VerticalAlign = VerticalAligns.Top
                })
                .SetXAxis(new XAxis
                {
                    Categories = x.ToArray()
                    //new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
                })
                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "Shigma(σ)" },
                    PlotLines = new[] { new YAxisPlotLines { Value = 0, Width = 1, Color = System.Drawing.Color.AliceBlue } }
                })
                .SetTooltip(new Tooltip { ValueSuffix = "σ" })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Right,
                    VerticalAlign = VerticalAligns.Middle,
                    BorderWidth = 0
                })
                .SetSeries(new[]
                {

                    new Series {
                        Name = GetCurrentMacno("101303"),
                        Data = new Data(y101303.ToArray())//new object[] { 7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6 })
                    },
                    new Series {
                        Name = GetCurrentMacno("101304"),
                        Data = new Data(y101304.ToArray())//new object[] { 7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6 })
                    }
                });
            ViewData.Add("lineChart2", chart.ToHtmlString());
        }

        private List<MachineName> GetMachineList()
        {
            List<MachineName> machines = new List<MachineName>();

            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT Distinct macno, macname FROM TnDBTrend";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineName t = new MachineName();
                        t.macno = SQLite.ParseString(reader["macno"]);
                        t.macname = SQLite.ParseString(reader["macname"]);
                        machines.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }

            return machines;
        }      
    }
}