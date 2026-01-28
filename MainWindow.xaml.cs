using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScottPlot;

namespace Plotting {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private HistogramRunner _histogramRunner;
        private bool _isRunning = false;
        private Thread _histogramThread;

        private int _experimentSize = 1000;
        private int _delayMilli = 500;
        private double _successChance = .5;
        private int _numBins = 100;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MainWindow() {
            InitializeComponent();
            Loaded += OnLoaded; // Initializing fields in the OnLoaded method this way is recommended
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        private void OnLoaded(object sender, RoutedEventArgs e) {
            _histogramRunner = new HistogramRunner();
            _histogramThread = new Thread(RunSimulation);
            ScottPlot.Plottables.HistogramBars histBars = WpfPlot.Plot.Add.Histogram(_histogramRunner.Histogram);
            histBars.BarWidthFraction = 0.9;
            WpfPlot.Plot.Axes.SetLimitsX(-.1, 1.1);
            this.Loaded -= OnLoaded;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e) {
            Trace.WriteLine($"Start button pressed");
            if (_isRunning) {
                Stop();
                startButton.Content = "Start";
            }
            else {
                Run();
                startButton.Content = "Stop";
            }
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _delayMilli = (int)(e.NewValue * 1000);
            Trace.WriteLine($"New delay (ms): {_delayMilli}");
        }
        private void SuccessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _successChance = (double)e.NewValue;
            Trace.WriteLine($"New success chance: {_successChance}");
        }

        private void ExperimentSize_Changed(object sender, TextChangedEventArgs e) {
            if (experimentSize.Text.Equals(String.Empty) || experimentSize.Text.Equals("0")) {
                experimentSize.Text = "1";
            }
            else if (!experimentSize.Text.All(char.IsDigit)) { // User has input some invalid text
                experimentSize.Text = _experimentSize.ToString();
            }
            else { // All good
                e.Handled = true;
                try {
                    _experimentSize = Int32.Parse(experimentSize.Text);
                    if (_experimentSize > 9999) {
                        experimentSize.Text = "9999";
                    }
                }
                catch {
                    throw new Exception($"ExperimentSize_Changed call has invalid text '{experimentSize.Text}' which is not handled.");
                }

                Trace.WriteLine($"New experiment size: {_experimentSize}");
            }
        }

        private void NumBinsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _numBins = (int)numBinsSlider.Value;
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            _histogramRunner.ClearHistogram();
            _histogramRunner.SetNumberOfBins(_numBins);
            ScottPlot.Plottables.HistogramBars histBars = WpfPlot.Plot.Add.Histogram(_histogramRunner.Histogram);
            histBars.BarWidthFraction = 0.9;
            if (_meanLine != null) {
                WpfPlot.Plot.Remove(_meanLine);
            }
            WpfPlot.Refresh();
            Trace.WriteLine($"Histogram cleared");
        }

        public void Run() {
            if (_isRunning) return;
            _isRunning = true;
            _histogramThread.Start();
        }

        public void Stop() {
            if (!_isRunning) return;
            _isRunning = false;
            _histogramThread = new Thread(RunSimulation);
        }

        private ScottPlot.Plottables.VerticalLine _meanLine;
        private void RunSimulation() {
            Stopwatch sw = new Stopwatch();
            if (_meanLine != null) {
                WpfPlot.Plot.Remove(_meanLine);
            }
            var palette = new ScottPlot.Palettes.Amber();
            ScottPlot.Color meanColor = palette.GetColor(0);
            _meanLine = WpfPlot.Plot.Add.VerticalLine(0, 2, meanColor);
            while (_isRunning) {
                sw.Start();

                double value = _histogramRunner.AddTrial(_experimentSize, _successChance);
                Trace.WriteLine($"Adding {value} to histogram.");

                WpfPlot.Plot.Remove(_meanLine);
                double mean = _histogramRunner.ComputeMean();
                _meanLine = WpfPlot.Plot.Add.VerticalLine(mean, 2, meanColor);
                Trace.WriteLine($"Mean: {mean}");
                int maxFreq = _histogramRunner.Histogram.Counts.Max();
                WpfPlot.Plot.Axes.SetLimitsY(0, maxFreq * 1.2);

                /*
                if (_histogramRunner.Histogram.GetCumulativeCounts().Last() % 10 == 0) {
                    double stdDev = ScottPlot.Statistics.Descriptive.StandardDeviation(_histogramRunner.Histogram.Counts);
                    var stdDevLine = WpfPlot.Plot.Add.Line(mean, maxFreq * .1, mean+stdDev, maxFreq * .1);
                }
                */

                WpfPlot.Refresh();

                sw.Stop();
                int waitTime = _delayMilli - (int)sw.ElapsedMilliseconds;
                Thread.Sleep(Math.Max(0, waitTime));
                sw.Reset();
            }
        }

    }
}