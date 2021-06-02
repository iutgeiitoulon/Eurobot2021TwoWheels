using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Drawing.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Utilities;

namespace WpfWorldMapDisplay
{
    public class SegmentRenderableSeries : CustomRenderableSeries
    {
        List<SegmentExtended> segmentList = new List<SegmentExtended>();
        XyDataSeries<double, double> lineData = new XyDataSeries<double, double> { }; //Nécessaire pour l'update d'affichage

        public SegmentRenderableSeries()
        {
        }

        public void AddSegmentExtended(int id, SegmentExtended s)
        {
            segmentList.Add(s);
        }

        public void Clear()
        {
            segmentList.Clear();
        }

        public int Count()
        {
            return segmentList.Count();
        }

        public void RedrawAll()
        {
            //TODO Attention : Permet de déclencher l'update : workaround pas classe du tout
            lineData.Clear();
            lineData.Append(1, 1);
            DataSeries = lineData;
        }

        protected override void Draw(IRenderContext2D renderContext, IRenderPassData renderPassData)
        {
            base.Draw(renderContext, renderPassData);

            // Create a line drawing context. Make sure you dispose it!
            // NOTE: You can create mutliple line drawing contexts to draw segments if you want
            //       You can also call renderContext.DrawLine() and renderContext.DrawLines(), but the lineDrawingContext is higher performance
            CustomDraw(renderContext);
        }

        private void CustomDraw(IRenderContext2D renderContext)
        {
            foreach (var s in segmentList)
            {
                Point initialPoint = GetRenderingPoint(new Point(s.Segment.X1, s.Segment.Y1));
                Point endPoint = GetRenderingPoint(new Point(s.Segment.X2, s.Segment.Y2));
                System.Windows.Media.Color segmentColor = System.Windows.Media.Color.FromArgb(s.Color.A, s.Color.R, s.Color.G, s.Color.B);

                /// Create a pen to draw. Make sure you dispose it! 
                using (var linePen = renderContext.CreatePen(segmentColor, this.AntiAliasing, (float)s.Width, s.Opacity, s.DashPattern))
                {
                    using (var lineDrawingContext = renderContext.BeginLine(linePen, initialPoint.X, initialPoint.Y))
                    {
                        lineDrawingContext.MoveTo(endPoint.X, endPoint.Y);
                        lineDrawingContext.End();
                    }
                }
            }
        }


        private Point GetRenderingPoint(Point pt)
        {
            // Get the coordinateCalculators. See 'Converting Pixel Coordinates to Data Coordinates' documentation for coordinate transforms
            var xCoord = CurrentRenderPassData.XCoordinateCalculator.GetCoordinate(pt.X);
            var yCoord = CurrentRenderPassData.YCoordinateCalculator.GetCoordinate(pt.Y);

            //if (CurrentRenderPassData.IsVerticalChart)
            //{
            //    Swap(ref xCoord, ref yCoord);
            //}

            return new Point(xCoord, yCoord);
        }
    }
}