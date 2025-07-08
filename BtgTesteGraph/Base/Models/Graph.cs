using SkiaSharp;

namespace BtgTesteGraph.Base.Models
{
    public class Graph 
    {
        public double InitialPrice { get; set; }
        public double Volatility  {get; set; }

        public double Media { get; set; }
        public int Time { get; set; }

        public double[] Curve { get; set; }
        public SKColor CurveColor { get; set; }
    }
}
