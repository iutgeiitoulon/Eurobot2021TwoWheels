using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Data.Model;
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
    public class PointListRenderableSeries : CustomRenderableSeries
    {
        List<PointDExtended> ptList = new List<PointDExtended>();

        XyDataSeries<double, double> lineData = new XyDataSeries<double, double> { }; //Nécessaire pour l'update d'affichage

        public PointListRenderableSeries()
        {
        }

        public void AddPtExtended(PointDExtended pt)
        {
            ptList.Add(pt);
        }

        public void Clear()
        {
            ptList.Clear();
        }

        public int Count()
        {
            return ptList.Count();
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
            CustomDraw(renderContext, renderPassData);
        }

        private void CustomDraw(IRenderContext2D renderContext, IRenderPassData renderPassData)
        {
            var dataPointSeries = renderPassData.PointSeries as Point2DSeries;

            foreach (var pt in ptList)
            {
                System.Windows.Media.Color ptColor = System.Windows.Media.Color.FromArgb(pt.Color.A, pt.Color.R, pt.Color.G, pt.Color.B);

                var ptRefLocal = GetRenderingPoint(new Point(pt.Pt.X, pt.Pt.Y));

                /// Create a pen to draw. Make sure you dispose it! 
                using (var ptPen = renderContext.CreatePen(ptColor, this.AntiAliasing, (float)1))
                {
                    using (var ptBrush = renderContext.CreateBrush(new SolidColorBrush(ptColor)))
                    {
                        renderContext.DrawEllipse(ptPen, ptBrush, ptRefLocal, pt.Width, pt.Width);

                        //using (var lineDrawingContext = renderContext.BeginLine(linePen, initialPoint.X, initialPoint.Y))
                        //    {
                        //        lineDrawingContext.MoveTo(endPoint.X, endPoint.Y);
                        //        lineDrawingContext.End();
                        //    }
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