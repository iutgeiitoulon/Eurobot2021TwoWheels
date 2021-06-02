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
    public class PolygonRenderableSeries : CustomRenderableSeries
    {
        ConcurrentDictionary<int, PolygonExtended> polygonList = new ConcurrentDictionary<int, PolygonExtended>();
        XyDataSeries<double, double> lineData = new XyDataSeries<double, double> { }; //Nécessaire pour l'update d'affichage

        public PolygonRenderableSeries()
        {
        }

        public void AddOrUpdatePolygonExtended(int id, PolygonExtended p)
        {
            polygonList.AddOrUpdate(id, p, (key, value) => p);
        }

        public void Clear()
        {
            polygonList.Clear();
        }

        public int Count()
        {
            return polygonList.Count();
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
            foreach (var p in polygonList)
            {
                Polygon polygon = p.Value.polygon;

                if (polygon.Points.Count > 0)
                {
                    Point initialPoint = GetRenderingPoint(polygon.Points[0]);

                    System.Windows.Media.Color backgroundColor = System.Windows.Media.Color.FromArgb(p.Value.backgroundColor.A, p.Value.backgroundColor.R, p.Value.backgroundColor.G, p.Value.backgroundColor.B);

                    using (var brush = renderContext.CreateBrush(backgroundColor))
                    {
                        //IEnumerable<Point> points; // define your points
                        renderContext.FillPolygon(brush, GetRenderingPoints(polygon.Points));
                    }

                    //// Create a pen to draw. Make sure you dispose it! 
                    System.Windows.Media.Color borderColor = System.Windows.Media.Color.FromArgb(p.Value.borderColor.A, p.Value.borderColor.R, p.Value.borderColor.G, p.Value.borderColor.B);

                    using (var linePen = renderContext.CreatePen(borderColor, this.AntiAliasing, p.Value.borderWidth, p.Value.borderOpacity, p.Value.borderDashPattern))
                    {
                        using (var lineDrawingContext = renderContext.BeginLine(linePen, initialPoint.X, initialPoint.Y))
                        {
                            for (int i = 1; i < polygon.Points.Count; i++)
                            {
                                lineDrawingContext.MoveTo(GetRenderingPoint(polygon.Points[i]).X, GetRenderingPoint(polygon.Points[i]).Y);
                            }
                            lineDrawingContext.End();
                        }
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
        private PointCollection GetRenderingPoints(PointCollection ptColl)
        {
            PointCollection ptCollRender = new PointCollection();
            foreach (var pt in ptColl)
            {
                // Get the coordinateCalculators. See 'Converting Pixel Coordinates to Data Coordinates' documentation for coordinate transforms
                var xCoord = CurrentRenderPassData.XCoordinateCalculator.GetCoordinate(pt.X);
                var yCoord = CurrentRenderPassData.YCoordinateCalculator.GetCoordinate(pt.Y);
                ptCollRender.Add(new Point(xCoord, yCoord));
            }

            return ptCollRender;
        }
    }
}
