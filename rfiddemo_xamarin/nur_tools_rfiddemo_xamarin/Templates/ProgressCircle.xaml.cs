using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace nur_tools_rfiddemo_xamarin.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProgressCircle : Frame
    {
        SKPaintSurfaceEventArgs args;
        ProgressUtils progressUtils = new ProgressUtils();                
        int gauge_value = 0;
        SKColor gauge_color;
        SKColor gauge_value_color;
        string gaugeUnitText;
        SKColor gaugeUnitTextColor;
        
        int gauge_radial_width;
        private int rangeMin;
        private int rangeMax;
        
        public ProgressCircle()
        {
            InitializeComponent();

            gauge_color = Color.FromHex("#05c782").ToSKColor();
            gauge_value_color =  Color.FromHex("#676a69").ToSKColor();
            gauge_radial_width = 40;
            RangeMin = 0;
            RangeMax = 100;
            gaugeUnitText = "%";
            gaugeUnitTextColor = Color.FromHex("#e2797a").ToSKColor();

            Invalidate();                        
        }

        // Initializing the canvas & drawing over it
        async void OnCanvasViewPaintSurfaceAsync(object sender, SKPaintSurfaceEventArgs args1)
        {
            args = args1;
            drawGauge();
        }
              
        /// <summary>
        /// Thickness of gauge
        /// </summary>
        public int GaugeRadialWidth
        {
            set { gauge_radial_width = value; }
        }

        /// <summary>
        /// Set cauge color. Use SetProgress() to take new color in use.
        /// </summary>
        /// <param name="gColor">Color of gauge</param>
        public Color GaugeColor
        {
            set { gauge_color = value.ToSKColor(); }
        }

        public Color GaugeValueTextColor
        {
            set { gauge_value_color = value.ToSKColor(); }            
        }

        public int RangeMin { get => rangeMin; set => rangeMin = value; }
        public int RangeMax { get => rangeMax; set => rangeMax = value; }
        public string GaugeUnitText { get => gaugeUnitText; set => gaugeUnitText = value; }
        public Color GaugeUnitTextColor { set => gaugeUnitTextColor = value.ToSKColor(); }

        /// <summary>
        /// Show gauge value between RangeMin - RangeMax)        
        /// </summary>
        /// <param name="gaugeValue">set value between RangeMin - RangeMax</param>
        public void SetProgress(int gaugeValue)
        {
            if (gaugeValue >= RangeMin && gaugeValue <= RangeMax)
            {
                gauge_value = gaugeValue;
                Invalidate();
            }
        }

        /// <summary>
        /// Update gauge
        /// </summary>
        public void Invalidate()
        {
            if (canvas != null)
            {
                // Invalidating surface due to data change
                canvas.InvalidateSurface();
            }
        }

        private void drawGauge()
        {
            // Radial Gauge Constants
            int uPadding = 50;
            int side = 500;
            int radialGaugeWidth = gauge_radial_width;

            // Line TextSize inside Radial Gauge
            int lineSize1 = 220;
            int lineSize2 = 150;
            //int lineSize3 = 80;

            // Line Y Coordinate inside Radial Gauge
            int lineHeight1 = 100;
            int lineHeight2 = 300;
            //int lineHeight3 = 300;

            // Start & End Angle for Radial Gauge
            float startAngle = -220;
            float sweepAngle = 260;

            try
            {
                // Getting Canvas Info 
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;
                progressUtils.setDevice(info.Height, info.Width);
                canvas.Clear();


                // Getting Device Specific Screen Values
                // -------------------------------------------------

                // Top Padding for Radial Gauge
                float upperPading = progressUtils.getFactoredHeight(uPadding);

                /* Coordinate Plotting for Radial Gauge
                *
                *    (X1,Y1) ------------
                *           |   (XC,YC)  |
                *           |      .     |
                *         Y |            |
                *           |            |
                *            ------------ (X2,Y2))
                *                  X
                *   
                *To fit a perfect Circle inside --> X==Y
                *       i.e It should be a Square
                */

                // Xc & Yc are center of the Circle
                int Xc = info.Width / 2;
                float Yc = progressUtils.getFactoredHeight(side);

                // X1 Y1 are lefttop cordiates of rectange
                int X1 = (int)(Xc - Yc);
                int Y1 = (int)(Yc - Yc + upperPading);

                // X2 Y2 are rightbottom cordiates of rectange
                int X2 = (int)(Xc + Yc);
                int Y2 = (int)(Yc + Yc + upperPading);

                //  Empty Gauge Styling
                SKPaint paint1 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.FromHex("#e0dfdf").ToSKColor(),                   // Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Filled Gauge Styling
                SKPaint paint2 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = gauge_color,                                             // Overlay Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Overlay Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Defining boundaries for Gauge
                SKRect rect = new SKRect(X1, Y1, X2, Y2);

                // Rendering Empty Gauge
                SKPath path1 = new SKPath();
                path1.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path1, paint1);

                // Rendering Filled Gauge
                SKPath path2 = new SKPath();
                path2.AddArc(rect, startAngle, (float)(sweepAngle / (RangeMax - RangeMin) * gauge_value));
                canvas.DrawPath(path2, paint2);

                //---------------- Drawing Text Over Gauge ---------------------------

                // Achieved Minutes
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = gauge_value_color;
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize1);
                    skPaint.Typeface = SKTypeface.FromFamilyName(
                                        "Arial",
                                        SKFontStyleWeight.Bold,
                                        SKFontStyleWidth.Normal,
                                        SKFontStyleSlant.Upright);

                    canvas.DrawText(gauge_value.ToString(), Xc, Yc + progressUtils.getFactoredHeight(lineHeight1), skPaint);
                }


                // Achieved Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = gaugeUnitTextColor;
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize2);
                    canvas.DrawText(GaugeUnitText, Xc, Yc + progressUtils.getFactoredHeight(lineHeight2), skPaint);
                }

                /*
                // Goal Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#e2797a");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize3);

                    canvas.DrawText("Near: " + pros, Xc, Yc + progressUtils.getFactoredHeight(lineHeight3), skPaint);
                    
                }
                */

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

    }

    public class ProgressUtils
    {
        // Reference Values(Standard Pixel 1 Device)
        private const float refHeight = 1080;//1677;
        private const float refWidth = 632;//940;

        // Derived Proportinate Values
        private float deviceHeight = 1; // Initializing to 1
        private float deviceWidth = 1;  // Initializing to 1

        // Empty Constructor
        public ProgressUtils() { }

        // Setting Device Specific Values
        public void setDevice(int deviceHeight, int deviceWidth)
        {
            this.deviceHeight = deviceHeight;
            this.deviceWidth = deviceWidth;
        }

        // Getting Device Specific Values
        public float getFactoredValue(int value)
        {

            float refRatio = refHeight / refWidth;
            float devRatio = deviceHeight / deviceWidth;

            float factoredValue = value * (refRatio / devRatio);

            Debug.WriteLine("RR:" + refRatio + "  DR: " + devRatio + " DIV:" + (refRatio / devRatio));
            Debug.WriteLine("Calculated Value for " + value + "  : " + factoredValue);

            return factoredValue;
        }

        // Deriving Proportinate Height
        public float getFactoredHeight(int value)
        {
            return (float)((value / refHeight) * deviceHeight);
        }

        // Deriving Proportinate Width
        public float getFactoredWidth(int value)
        {
            return (float)((value / refWidth) * deviceWidth);
        }

        // Deriving Sweep Angle
        public int getSweepAngle(int goal, int achieved)
        {
            int SweepAngle = 260;
            float factor = (float)achieved / goal;
            Debug.WriteLine("SWEEP ANGLE : " + (int)(SweepAngle * factor));

            return (int)(SweepAngle * factor);

        }

    }
}