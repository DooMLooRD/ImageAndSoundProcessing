using ImageProcessing.Core.Helpers;
using ImageProcessing.View.ViewModel.Base;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;

namespace ImageProcessing.View.ViewModel
{
    public class HistogramVM : BaseViewModel
    {
        public bool IsHistogramMulti { get; set; } = false;
        public SeriesCollection Histogram1 { get; set; }
        public SeriesCollection Histogram2 { get; set; }
        public SeriesCollection Histogram3 { get; set; }
        public string[] Labels { get; set; }
        public HistogramVM(Histogram histogramToRender)
        {
            if (histogramToRender == null)
            {
                return;
            }
            IsHistogramMulti = !histogramToRender.IsGreyscale;

            Histogram1 = new SeriesCollection();

            if (histogramToRender.Values.Length == 0)
            {
                return;
            }
            Histogram1 = new SeriesCollection{new ColumnSeries
            {
                Values = getHistogramValues(histogramToRender.Values, 0),
                Title = "h1",
                ColumnPadding = 0.0
            }};
            if (!IsHistogramMulti)
            {
                return;
            }
            Histogram2 = new SeriesCollection();
            Histogram3 = new SeriesCollection();

            Histogram2 = new SeriesCollection {new ColumnSeries
            {
                Values = getHistogramValues(histogramToRender.Values, 1),
                Title = "h1",
                ColumnPadding = 0.0
            }};
            Histogram3 = new SeriesCollection{new ColumnSeries
            {
                Values = getHistogramValues(histogramToRender.Values, 2),
                Title = "h1",
                ColumnPadding = 0.0
            }};

        }
        public ChartValues<long> getHistogramValues(long[][] values, int valueIndex)
        {
            var labels = new List<string>();

            var ret = new ChartValues<long>();

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                ret.Add(value[valueIndex]);
                labels.Add($"{i}");
            }

            Labels = labels.ToArray();
            return ret;
        }
    }


}
