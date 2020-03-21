using ImageProcessing.Core.Helpers;
using ImageProcessing.View.ViewModel.Base;
using LiveCharts;
using LiveCharts.Wpf;

namespace ImageProcessing.View.ViewModel
{
    public class HistogramVM : BaseViewModel
    {
        public bool IsHistogramMulti { get; set; } = false;
        public SeriesCollection Histogram1 { get; set; }
        public SeriesCollection Histogram2 { get; set; }
        public SeriesCollection Histogram3 { get; set; }
        public HistogramVM(Histogram histogramToRender)
        {
            if (histogramToRender == null)
            {
                return;
            }
            IsHistogramMulti = histogramToRender.IsGreyscale;

            IsHistogramMulti = false;
            if (histogramToRender.Values.Length == 0)
            {
                return;
            }
            Histogram1 = new SeriesCollection(new ColumnSeries
            {
                Values = new ChartValues<long>(histogramToRender.Values[0])
            });
            if (histogramToRender.Values.Length == 1)
            {
                return;
            }
            Histogram2 = new SeriesCollection(new ColumnSeries
            {
                Values = new ChartValues<long>(histogramToRender.Values[1])
            });
            Histogram3 = new SeriesCollection(new ColumnSeries
            {
                Values = new ChartValues<long>(histogramToRender.Values[2])
            });

        }
    }
}
