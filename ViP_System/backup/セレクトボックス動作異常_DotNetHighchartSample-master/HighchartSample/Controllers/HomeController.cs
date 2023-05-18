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
            //CreateBarChart();
            //CreatePieChart();
            return View("index",machines);
        }

        private void CreateLineChart(string macno)
        {
            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];
            List<string> x = new List<string>();
            List<object> y = new List<object>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma, macno FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", "%"));
                //部分一致検索用


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {                      
                        x.Add((SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue).ToString());
                        y.Add(SQLite.ParseSingle(reader["shigma"]).ToString());
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
                    Text = "Line Chart Sample",
                    Align = HorizontalAligns.Center,
                    VerticalAlign = VerticalAligns.Top
                })
                .SetXAxis(new XAxis
                {                   
                    Categories = x.ToArray()//new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
                })
                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "気温（℃）" },
                    PlotLines = new[] { new YAxisPlotLines { Value = 0, Width = 1, Color = System.Drawing.Color.AliceBlue } }
                })
                .SetTooltip(new Tooltip { ValueSuffix = "℃" })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical, Align = HorizontalAligns.Right, VerticalAlign = VerticalAligns.Middle, BorderWidth = 0
                })
                .SetSeries(new [] 
                {
                    new Series {
                        Name = "東京",
                        Data = new Data(y.ToArray())//new object[] { 7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6 })
                    }/*,
                    new Series {
                        Name = "ニューヨーク",
                        Data = new Data(new object[] { -0.2, 0.8, 5.7, 11.3, 17.0, 22.0, 24.8, 24.1, 20.1, 14.1, 8.6, 2.5 })
                    },
                    new Series {
                        Name = "ベルリン",
                        Data = new Data( new object[] { -0.9, 0.6, 3.5, 8.4, 13.5, 17.0, 18.6, 17.9, 14.3, 9.0, 3.9, 1.0 })
                    },
                    new Series {
                        Name = "ロンドン",
                        Data = new Data(new object[] { 3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8 })
                    }*/
                });
            ViewData.Add("lineChart", chart.ToHtmlString());
        }

        private void CreateBarChart()
        {
            Highcharts chart = new Highcharts("barchart")
               .InitChart(new Chart { DefaultSeriesType = ChartTypes.Bar })
               .SetTitle(new Title { Text = "月間降雨量" })
               .SetSubtitle(new Subtitle { Text = "Source: <a href=\"http://WorldClimate.com\">WorldClimate.com</a>", UseHTML = true })
               .SetXAxis(new XAxis { Categories = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }})
               .SetYAxis(new YAxis
               {
                   Min = 0,
                   Title = new YAxisTitle { Text = "雨量 (mm)" }
               })
               .SetTooltip(new Tooltip {
                   /*
                   HeaderFormat = "<span style=\"font-size:10px\">{point.key}</span><table>",
                   PointFormat = "'<tr><td style=\"color:{ series.color}; padding: 0\">{series.name}: </td>' + '<td style=\"padding:0\"><b>{point.y:.1f} mm</b></td></tr>'",
                   Formatter = "'</table>'",
                   */
                   Shared = true,
                   UseHTML = true
               })
               .SetPlotOptions(new PlotOptions
               {
                   Column = new PlotOptionsColumn { PointPadding = 0.2, BorderWidth = 0 }
               })
               .SetSeries(new[] {
                    new Series {
                        Name = "東京",
                        Data = new Data(new object[] { 49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 })
                    },
                    new Series {
                        Name = "ニューヨーク",
                        Data = new Data(new object[] { 83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3 })
                    },
                    new Series {
                        Name = "ベルリン",
                        Data = new Data( new object[] { 42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1 })
                    },
                    new Series {
                        Name = "ロンドン",
                        Data = new Data(new object[] { 48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2 })
                    }
               });
            ViewData.Add("barChart", chart.ToHtmlString());
        }

        private void CreatePieChart() {
            Highcharts chart = new Highcharts("piechart")
                .InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Pie,
                    PlotBackgroundColor = null,
                    PlotBorderColor = null,
                    PlotShadow = null
                })
                .SetTitle(new Title { Text = "ブラウザサマリ" })
                .SetTooltip(new Tooltip
                {
                    PointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>"
                })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels { Enabled = false },
                        ShowInLegend = true
                    }
                })
                .SetSeries(new []
                {
                    new Series
                    {
                        Name = "製品名",
                        Data = new Data(new Point[] {
                            new Point { Name = "Microsoft Internet Explorer", Y = 56.33 },
                            new Point { Name = "Chrome", Y = 24.03, Selected = true, Sliced = true },
                            new Point { Name = "Firefox", Y = 10.38 },
                            new Point { Name = "Safari", Y = 4.77 },
                            new Point { Name = "Opera", Y = 0.91 },
                            new Point { Name = "Others", Y = 0.2 }
                        })
                    }
                });
            ViewData.Add("pieChart", chart.ToHtmlString());
        }

        //ボタン押下時
        [HttpPost]
        public ActionResult SelectMachine(String macno)
        {
            List<MachineName> machines = new List<MachineName>();
            //List<String> CM = new List<String>();

            //ドロップダウンメニュー選択用の設備リスト取得
            //machines = GetMachineList(macno);
            CreateLineChart(macno);


            //現在選択中の設備名称取得
            //CM = GetCurrentMacno(macno);

            //return View("index");
            return View("index");
        }

        [HttpPost]
        public ActionResult ResinIdFilter(String magno)
        {
            string constr = "test";
           
            return View("index");
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

        private List<string> GetCurrentMacno(string macno)
        {
            List<string> CM = new List<string>();

            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];
            
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
                        CM.Add(SQLite.ParseString(reader["macname"]));
                        break;
                    }
                }
                cmd.Parameters.Clear();
            }

            return CM;
        }
    }
}