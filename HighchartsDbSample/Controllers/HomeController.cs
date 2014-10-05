using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersianDate.Globalization;

namespace HighchartsDbSample.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            DbDataContext dt = new DbDataContext();

            

             //Fetching data from db:
            var voltageValues = dt.Values.Where(v => v.FieldName == "Voltage").OrderBy(v => v.Datetime).ToList<Value>();
            var currentValues = dt.Values.Where(v => v.FieldName == "Current").OrderBy(v => v.Datetime).ToList<Value>(); 

            Highcharts Chart = new Highcharts("Chart");
            // Initiizing chart
            // Making month and days persian, however it is not accurate at all!
            Chart.SetOptions(new GlobalOptions
            {
                Lang = new DotNet.Highcharts.Helpers.Lang
                {
                    Loading = "در حال بارگذاری",
                    Months = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" },
                    Weekdays = new string[] { "شنبه", "یک شنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه" },
                    ShortMonths = new string[] { "فرور", "اردی", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" }
                }
            });
            Chart.InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line,
                    MarginRight = 130,
                    MarginBottom = 55,
                    ClassName = "chart",
                    ZoomType = ZoomTypes.X
                })
            .SetTitle(new Title
                {
                    Text = "نمودار تغییرات داده ها "
                })
            .SetSubtitle(new Subtitle
                {
                    Text = "نمونه استفاده نمودار",
                    X = -20
                })
            .SetXAxis(new XAxis
                {
                    Type = AxisTypes.Datetime,
                    Title = new XAxisTitle
                    {
                        Text = "بازه زمانی از ... تا..."
                    },
                    MinorTickInterval = 3600 * 1000,
                    TickLength = 1,
                    MinRange = 3600 * 1000,
                    MinTickInterval = 3600 * 1000,
                    GridLineWidth = 1,
                    Labels = new XAxisLabels
                    {
                        Align = HorizontalAligns.Right,
                        Rotation = -30,
                    },
                    DateTimeLabelFormats = new DateTimeLabel
                    {
                        Second = "%H:%M:%S",
                        Minute = "%H:%M",
                        Hour = "%H:%M",
                        Day = "%e %b",
                        Week = "%e %b",
                        Month = "%b",
                        Year = "%Y",
                    },
                    ShowEmpty = false,

                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    X = 20,
                    VerticalAlign = VerticalAligns.Top,
                    Y = 80,
                    BackgroundColor = new BackColorOrGradient(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"))
                });

            YAxis[] yAxis = new YAxis[2];
            yAxis[0] = (new YAxis
            {
                Title = new YAxisTitle
                {
                    Text = string.Format("{0} ({1})", "Voltage", "V"),

                },
                Labels = new YAxisLabels
                {
                    //Align = HorizontalAligns.Right,
                    Formatter = "function() { return this.value; }",
                },
                Opposite = true,
                GridLineWidth = 0
            });
            yAxis[1] = (new YAxis
            {
                Title = new YAxisTitle
                {
                    Text = string.Format("{0} ({1})", "Current", "A"),

                },
                Labels = new YAxisLabels
                {
                    //Align = HorizontalAligns.Left,
                    Formatter = "function() { return this.value; }",
                },
                Opposite = false,
                GridLineWidth = 1
            });

            Chart.SetYAxis(yAxis);

            Series[] seriesOfData = new Series[2];


     
            object[,] x1 = new object[voltageValues.Count(), 2];
            for (int i = 0; i < voltageValues.Count(); i++)
            {

                x1[i, 0] = PersianDateTime.ParseFromDateTime(voltageValues[i].Datetime).ToString("Date.parse('MM/dd/yyyy HH:mm:ss')");
                x1[i, 1] = voltageValues[i].Value1;
            }

            DotNet.Highcharts.Helpers.Data data1 = new DotNet.Highcharts.Helpers.Data(x1);
            Series series1 = new Series
            {
                Name = "Voltage",
                Data = data1,
                Type = ChartTypes.Line,
            };
            series1.YAxis = "0";
            seriesOfData[0] = series1;

            object[,] x2 = new object[currentValues.Count(), 2];
            for (int i = 0; i < voltageValues.Count(); i++)
            {
                x2[i, 0] = PersianDateTime.ParseFromDateTime(voltageValues[i].Datetime).ToString("Date.parse('MM/dd/yyyy HH:mm:ss')");
                x2[i, 1] = currentValues[i].Value1;
            }
            DotNet.Highcharts.Helpers.Data data2 = new DotNet.Highcharts.Helpers.Data(x2);
            Series series2 = new Series
            {
                Name = "Current",
                Data = data2,
                Type = ChartTypes.Spline,
            };
            series1.YAxis = "1";
            seriesOfData[1] = series2;


            Chart.SetSeries(seriesOfData);
            ViewBag.Chart = Chart;


          

            return View();
        }

    }
}
