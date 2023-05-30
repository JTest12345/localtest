using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

using Newtonsoft.Json;

using KodaWeb.Models;
using KodaWeb.Models.Highcharts;
using KodaClassLibrary;

namespace KodaWeb.Controllers {
    public class EnvController : Controller {

        /// <summary>
        /// データベース接続文字列取得
        /// <para>web.config内の文字列変えた時に自動で取得しなおされる</para>
        /// </summary>
        private static string env_connect_string = WebConfigurationManager.ConnectionStrings["EnvDatabase"].ConnectionString;

        OndotoriSQL ondotori_sql = new OndotoriSQL(env_connect_string);

        ParticleSQL particle_sql = new ParticleSQL(env_connect_string);

        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// おんどとりデータ表示ページ
        /// </summary>
        public ActionResult ViewOndotoriGraph() {

            var list = ondotori_sql.Get_RemoteList<OndotoriGraph>();
            var tree_json = OndotoriGraph.Get_TreeJson(list);

            var og = new OndotoriGraph {
                TreeViewJson = tree_json,
                Start = DateTime.Now,
                End = DateTime.Now
            };

            return View(og);
        }

        /// <summary>
        /// おんどとりデータ表示ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewOndotoriGraph(OndotoriGraph og) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {

                //子機の温度データ取得
                var list = ondotori_sql.Get_DataList(og, og.Start, og.End);

                if (list.Count == 0) {
                    ViewBag.Msg = "データがありませんでした。";
                    return View(og);
                }

                var highchart_data = new List<List<object>>();

                for (int i = 0; i < list.Count; i++) {

                    var hd = new List<object>();

                    var d = list[i].Temperature;
                    if (d != null) {

                        //日本時間のUnixTimeミリ秒に変換
                        long ms = (list[i].UnixTime + 9 * 3600) * 1000;

                        hd.Add(ms);
                        hd.Add(d);
                        highchart_data.Add(hd);
                    }
                }

                var high = new Highcharts();
                high.Chart = new Chart { ZoomType = "x" };
                high.Title = new Title { Text = $"{og.BaseName}_{og.GroupName}_{og.RemoteName}", Margin = 5 };
                high.Xaxis = new XAxis { Type = "datetime", TickInterval = 3600 * 1000 };
                high.Xaxis.Labels.Format = "{value:%Y-%m-%d<br/>%H:%M:%S}";
                high.Xaxis.Labels.Rotation = 60;
                high.Yaxis = new List<YAxis> {
                    new YAxis { Title = new Title { Text = "Temperature [℃]" } , Labels=new YAxis.Label{X=-10,Y=4 } }
                };

                high.Tooltip = new Tooltip {
                    XDateFormat = "%Y-%m-%d %H:%M:%S",
                    PointFormat = "{point.y}°C",
                    BackgroundColor = "Snow",
                    Distance = 64
                };

                high.Legend = new Legend { Layout = "vertical", Align = "right", VerticalAlign = "middle", BorderWidth = 0 };
                high.SeriresList = new List<Series>();
                high.SeriresList.Add(new Series { Name = $"{og.RemoteName}", Data = highchart_data });

                high.PlotOptions = new PlotOptions { Series = new Series { Marker = new Marker { Enabled = false, Radius = 2 } } };

                high.RangeSelector.Selected = 2;

                og.GraphDataJson = high.GetJson(); ;
            }
            return View(og);
        }


        /// <summary>
        /// パーティクルデータ表示ページ
        /// </summary>
        public ActionResult ViewParticleGraph() {

            var list = particle_sql.Get_SensorList<ParticleGraph>();
            var tree_json = ParticleGraph.Get_TreeJson(list);

            var pg = new ParticleGraph {
                TreeViewJson = tree_json,
                Start = DateTime.Now,
                End = DateTime.Now
            };

            return View(pg);
        }

        /// <summary>
        /// パーティクルデータ表示ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewParticleGraph(ParticleGraph pg) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {

                //対象センサーのデータ取得
                var list = particle_sql.Get_DataList(pg, pg.Start, pg.End);

                if (list.Count == 0) {
                    ViewBag.Msg = "データがありませんでした。";
                    return View(pg);
                }

                var par03_datalist = new List<List<object>>();
                var par05_datalist = new List<List<object>>();
                var par1_datalist = new List<List<object>>();
                var temp_datalist = new List<List<object>>();
                var hum_datalist = new List<List<object>>();
                var dew_datalist = new List<List<object>>();

                //データをセットする
                for (int i = 0; i < list.Count; i++) {

                    //日本時間のUnixTimeミリ秒に変換
                    long ms = (list[i].Time.ToUnixTimeSeconds() + 9 * 3600) * 1000;

                    par03_datalist.Add(new List<object> { ms, list[i].Particle03 });
                    par05_datalist.Add(new List<object> { ms, list[i].Particle05 });
                    par1_datalist.Add(new List<object> { ms, list[i].Particle1 });

                    if (list[i].Temperature != null) {
                        temp_datalist.Add(new List<object> { ms, list[i].Temperature });
                        hum_datalist.Add(new List<object> { ms, list[i].Humidity });
                        dew_datalist.Add(new List<object> { ms, list[i].DewPoint });
                    }
                }

                //グラフ用オブジェクト作成
                var high = new Highcharts();
                high.Title = new Title { Text = $"{pg.ManuBase}_{pg.Place}_{pg.SensorName}", Margin = 5 };

                high.Xaxis = new XAxis { Type = "datetime", TickInterval = 3600 * 1000 };
                high.Xaxis.Labels.Format = "{value:%Y-%m-%d<br/>%H:%M:%S}";
                high.Xaxis.Labels.Rotation = 60;

                high.Yaxis = new List<YAxis> {
                    new YAxis {
                        Title= new Title { Text = "Particle [count]" },
                        Labels = new YAxis.Label{
                            X = -10,
                            Y = 4,
                            Format = "{value}"
                        },
                        Height = "60%",
                        PlotLines = new List<PlotLine> {
                            new PlotLine {
                                Value=10000,
                                Color="Red",
                                DashStyle="shortdash",
                                Width=1,
                                Labels=new PlotLine.Label{Text="0.5μm粒子上限値"},
                                ZIndex=5
                            }
                        }
                    },
                    new YAxis {
                        Title = new Title { Text = "Temperature" },
                        Labels=new YAxis.Label{
                            X = 50,
                            Y = 4,
                            Format = "{value}℃"
                        },
                        Min = 0,
                        Max = 40,
                        Height = "30%",
                        Top = "70%"
                    },
                    new YAxis {
                        Title = new Title { Text = "Humidity" },
                        Labels = new YAxis.Label{
                            X = 10,
                            Y = 4,
                            Format = "{value}%",
                            Align = "left"
                        },
                        Min = 0,
                        Max = 100,
                        Opposite = true,
                        Height = "30%",
                        Top = "70%"
                    }
                };

                high.Tooltip = new Tooltip {
                    XDateFormat = "%Y-%m-%d %H:%M:%S",
                    PointFormat = "<span style=\"color: {series.color}\">{series.name}:{point.y}</span><br/>",
                    Shared = true,
                    ValueSuffix = "count",
                    BackgroundColor = "Snow",
                    Distance = 64
                };

                high.Legend = new Legend { Layout = "vertical", Align = "right", VerticalAlign = "middle", BorderWidth = 0 };
                high.SeriresList = new List<Series>();
                high.SeriresList.Add(new Series { Name = $"0.3μm粒子", Data = par03_datalist, Type = "area", FillOpacity = 0.3, YAxis = 0 });
                high.SeriresList.Add(new Series { Name = $"0.5μm粒子", Data = par05_datalist, Type = "area", FillOpacity = 0.3, YAxis = 0 });
                high.SeriresList.Add(new Series { Name = $"1.0μm粒子", Data = par1_datalist, Type = "area", FillOpacity = 0.3, YAxis = 0 });
                high.SeriresList.Add(new Series { Name = $"温度", Data = temp_datalist, Type = "line", YAxis = 1, Tooltip = new Tooltip { ValueSuffix = "℃" } });
                high.SeriresList.Add(new Series { Name = $"湿度", Data = hum_datalist, Type = "line", YAxis = 2, Tooltip = new Tooltip { ValueSuffix = "%" } });

                high.PlotOptions = new PlotOptions { Series = new Series { Marker = new Marker { Enabled = false, Radius = 2 } } };

                high.RangeSelector.Selected = 2;

                //シリアライズ
                pg.GraphDataJson = high.GetJson();
            }
            return View(pg);
        }
    }
}