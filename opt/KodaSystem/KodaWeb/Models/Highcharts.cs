using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace KodaWeb.Models {

    namespace Highcharts {

        public class Highcharts {


            [JsonProperty("chart")]
            public Chart Chart { get; set; } = new Chart();

            [JsonProperty("title")]
            public Title Title { get; set; }

            [JsonProperty("xAxis")]
            public XAxis Xaxis { get; set; }

            [JsonProperty("yAxis")]
            public List<YAxis> Yaxis { get; set; }

            [JsonProperty("tooltip")]
            public Tooltip Tooltip { get; set; }

            [JsonProperty("legend")]
            public Legend Legend { get; set; }

            [JsonProperty("series")]
            public List<Series> SeriresList { get; set; }

            [JsonProperty("plotOptions")]
            public PlotOptions PlotOptions { get; set; }

            [JsonProperty("scrollbar")]
            public Scrollbar Scrollbar { get; set; } = new Scrollbar();

            [JsonProperty("navigator")]
            public Navigator Navigator { get; set; } = new Navigator();

            [JsonProperty("rangeSelector")]
            public RangeSelector RangeSelector { get; set; } = new RangeSelector {
                Buttons = new List<RangeSelector.Button> {
                    new RangeSelector.Button{Type="hour",Count=1,Text="1H"},
                    new RangeSelector.Button{Type="hour",Count=4,Text="4H"},
                    new RangeSelector.Button{Type="day",Count=1,Text="Day"},
                    new RangeSelector.Button{Type="day",Count=7,Text="Week"},
                    new RangeSelector.Button{Type="all",Count=1,Text="ALL"},
                }
            };

            [JsonProperty("credits")]
            public Credit Credits { get; set; } = new Credit();

            /// <summary>
            /// このクラスのJSON取得
            /// </summary>
            /// <returns></returns>
            public string GetJson() {
                var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                return JsonConvert.SerializeObject(this, Formatting.None, setting);
            }


        }

        public class Chart {

            [JsonProperty("zoomType")]
            public string ZoomType { get; set; } = "x";

        }

        public class Title {

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("x")]
            public int? X { get; set; }

            public string Align { get; set; } = "center";

            [JsonProperty("margin")]
            public int? Margin { get; set; }
        }

        public class XAxis {

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("categories")]
            public List<string> Categories { get; set; }

            [JsonProperty("tickInterval")]
            public long? TickInterval { get; set; }

            [JsonProperty("gridLineWidth")]
            public int GridLineWidth { get; set; } = 1;

            [JsonProperty("labels")]
            public Label Labels { get; set; } = new Label();

            public class Label {

                [JsonProperty("align")]
                public string Align { get; set; } = "left";

                [JsonProperty("rotation")]
                public int Rotation { get; set; }

                [JsonProperty("x")]
                public int X { get; set; } = 0;

                [JsonProperty("y")]
                public int Y { get; set; } = 0;

                [JsonProperty("format")]
                public string Format { get; set; }
            }

            [JsonProperty("crosshair")]
            public Crosshair Crosshair { get; set; } = new Crosshair();

        }

        public class YAxis {

            [JsonProperty("title")]
            public Title Title { get; set; }

            [JsonProperty("height")]
            public string Height { get; set; }

            [JsonProperty("top")]
            public string Top { get; set; }

            [JsonProperty("opposite")]
            public bool Opposite { get; set; }

            [JsonProperty("min")]
            public double? Min { get; set; }

            [JsonProperty("max")]
            public double? Max { get; set; }

            [JsonProperty("labels")]
            public Label Labels { get; set; } = new Label();

            public class Label {

                [JsonProperty("align")]
                public string Align { get; set; } = "right";

                [JsonProperty("rotation")]
                public int Rotation { get; set; }

                [JsonProperty("x")]
                public int X { get; set; } = 0;

                [JsonProperty("y")]
                public int Y { get; set; } = 0;

                [JsonProperty("format")]
                public string Format { get; set; }

            }

            [JsonProperty("crosshair")]
            public Crosshair Crosshair { get; set; } = new Crosshair();

            [JsonProperty("plotLines")]
            public List<PlotLine> PlotLines { get; set; }
        }

        public class Crosshair {

            [JsonProperty("snap")]
            public bool Snap { get; set; } = true;

            [JsonProperty("width")]
            public int Width { get; set; } = 1;

            [JsonProperty("color")]
            public string Color { get; set; } = "gray";

            [JsonProperty("dashStyle")]
            public string DashStyle { get; set; } = "shortdot";

        }

        public class PlotLine {

            [JsonProperty("value")]
            public double Value { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("dashStyle")]
            public string DashStyle { get; set; }

            [JsonProperty("width")]
            public int? Width { get; set; }

            [JsonProperty("zIndex")]
            public int? ZIndex { get; set; }

            [JsonProperty("label")]
            public Label Labels { get; set; }

            public class Label {

                [JsonProperty("text")]
                public string Text { get; set; }
            }

        }

        public class Legend {

            [JsonProperty("layout")]
            public string Layout { get; set; }

            [JsonProperty("align")]
            public string Align { get; set; }

            [JsonProperty("verticalAlign")]
            public string VerticalAlign { get; set; }

            [JsonProperty("borderWidth")]
            public int? BorderWidth { get; set; }
        }

        public class Tooltip {

            [JsonProperty("headerFormat")]
            public string HeaderFormat { get; set; }

            [JsonProperty("pointFormat")]
            public string PointFormat { get; set; }

            [JsonProperty("xDateFormat")]
            public string XDateFormat { get; set; }

            [JsonProperty("valueSuffix")]
            public string ValueSuffix { get; set; }

            [JsonProperty("valueDecimals")]
            public int? ValueDecimals { get; set; }

            [JsonProperty("split")]
            public bool? split { get; set; }

            [JsonProperty("shared")]
            public bool? Shared { get; set; }

            [JsonProperty("backgroundColor")]
            public string BackgroundColor { get; set; }

            [JsonProperty("distance")]
            public int? Distance { get; set; }

        }



        public class Series {

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("data")]
            public object Data { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("fillOpacity")]
            public double? FillOpacity { get; set; }

            [JsonProperty("yAxis")]
            public int? YAxis { get; set; }

            [JsonProperty("tooltip")]
            public Tooltip Tooltip { get; set; }

            [JsonProperty("marker")]
            public Marker Marker { get; set; }
        }

        public class Marker {

            [JsonProperty("enabled")]
            public bool? Enabled { get; set; }

            [JsonProperty("radius")]
            public int? Radius { get; set; }
        }



        public class PlotOptions {

            [JsonProperty("series")]
            public Series Series { get; set; }
        }



        public class Scrollbar {

            [JsonProperty("enabled")]
            public bool Enabled { get; set; } = true;
        }

        public class Navigator {

            [JsonProperty("enabled")]
            public bool Enabled { get; set; } = true;
        }

        /// <summary>
        /// RangeSelectorを使う時はデータ時間をUnixTimeのミリ秒表記にしないと機能しない
        /// </summary>
        public class RangeSelector {

            [JsonProperty("enabled")]
            public bool Enabled { get; set; } = true;

            [JsonProperty("selected")]
            public int? Selected { get; set; } = 0;

            [JsonProperty("buttons")]
            public List<Button> Buttons { get; set; }

            public class Button {

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("count")]
                public int? Count { get; set; }

                [JsonProperty("text")]
                public string Text { get; set; }
            }


        }

        public class Credit {

            [JsonProperty("enabled")]
            public bool Enabled { get; set; } = false;

        }
    }
}
