using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOscilloscopeControl
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class WpfOscilloscope : UserControl
    {
        Dictionary<int, XyDataSeries<double, double> > lineDictionary = new Dictionary<int, XyDataSeries<double, double>>();

        public WpfOscilloscope()
        {
            InitializeComponent();
        }

        public void AddOrUpdateLine(int lineId, int maxNumberOfPoints, string lineName, bool useYAxisRight = true)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId] = new XyDataSeries<double, double>(maxNumberOfPoints) { SeriesName = lineName };
                //sciChart.RenderableSeries.RemoveAt(id);
            }
            else
            {
                lineDictionary.Add(lineId, new XyDataSeries<double, double>(maxNumberOfPoints) { SeriesName = lineName });
           
                var lineRenderableSerie = new FastLineRenderableSeries();
                lineRenderableSerie.Name = "lineRenderableSerie"+lineId.ToString();
                lineRenderableSerie.DataSeries = lineDictionary[lineId];
                lineRenderableSerie.DataSeries.AcceptsUnsortedData = true;
                if(useYAxisRight)
                    lineRenderableSerie.YAxisId = "RightYAxis";
                else
                    lineRenderableSerie.YAxisId = "LeftYAxis";

                //Ajout de la ligne dans le scichart
                sciChart.RenderableSeries.Add(lineRenderableSerie);
            }             
        }

        public void RemoveLine(int lineId)
        {
            if (LineExist(lineId))
            {
                sciChart.RenderableSeries.Remove(sciChart.RenderableSeries.Single(x => x.DataSeries.SeriesName == lineDictionary[lineId].SeriesName));
                lineDictionary.Remove(lineId);
            }
        }

        public void ResetGraph()
        {
            foreach(var serie in sciChart.RenderableSeries)
            {
                serie.DataSeries.Clear();
            }
        }
        public void ResetLine(int lineId)
        {
            if (LineExist(lineId))
            {
                sciChart.RenderableSeries.Single(x => x.DataSeries.SeriesName == lineDictionary[lineId].SeriesName).DataSeries.Clear();
            }
        }
        public bool LineExist(int lineId)
        {
            return lineDictionary.ContainsKey(lineId);
        }


        public void SetTitle(string title)
        {
            titleText.Text = title;
        }
        public void SetSerieName(int lineId, string name)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId].SeriesName = name;
            }
        }

        public void ChangeLineColor(string lineName, Color color)
        {
            sciChart.RenderableSeries.Single(x => x.DataSeries.SeriesName == lineName).Stroke = color;
        }

        public void ChangeLineColor(int lineId, Color color)
        {
            if (LineExist(lineId))
            {
                sciChart.RenderableSeries.Single(x => x.DataSeries.SeriesName == lineDictionary[lineId].SeriesName).Stroke = color;
            }
        }

        public void DrawOnlyPoints(int lineId)
        {
            if (LineExist(lineId))
            {
                sciChart.RenderableSeries.Single(x => x.DataSeries.SeriesName == lineDictionary[lineId].SeriesName).Stroke = Color.FromArgb(0, 255, 255, 255);
            }
        }

        public void AddPointToLine(int lineId, double x, double y)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId].Append(x, y);
                if (lineDictionary[lineId].Count > lineDictionary[lineId].Capacity)
                    lineDictionary[lineId].RemoveAt(0);
            }
        }

        public void AddPointToLine(int lineId, Point point)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId].Append(point.X, point.Y);
                if (lineDictionary[lineId].Count > lineDictionary[lineId].Capacity)
                    lineDictionary[lineId].RemoveAt(0);
            }
        }

        public void AddPointListToLine(int lineId, List<Point> pointList)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId].Append(pointList.Select(e => e.X).ToList(), pointList.Select(e2 => e2.Y).ToList());
                if (lineDictionary[lineId].Count > lineDictionary[lineId].Capacity)
                    lineDictionary[lineId].RemoveAt(0);
            }
        }
        public void UpdatePointListOfLine(int lineId, List<Point> pointList)
        {
            if (LineExist(lineId))
            {
                lineDictionary[lineId].Clear();
                lineDictionary[lineId].Append(pointList.Select(e => e.X).ToList(), pointList.Select(e2 => e2.Y).ToList());
            }
        }
    }
}
