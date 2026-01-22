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

        public MainWindow() {
            InitializeComponent();
            Loaded += OnLoaded; // Initializing fields in the OnLoaded method this way is recommended
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            _histogramRunner = new HistogramRunner();
            _histogramThread = new Thread(RunSimulation);
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
            _delayMilli = (int)e.NewValue * 1000;
            Trace.WriteLine($"New delay (ms): {_delayMilli}");
        }
        private void SuccessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _successChance = (double)e.NewValue;
            Trace.WriteLine($"New success chance: {_successChance}");
        }

        private void ExperimentSize_Changed(object sender, TextChangedEventArgs e) {
            _experimentSize = Int32.Parse(experimentSize.Text);
            Trace.WriteLine($"New experiment size: {_experimentSize}");
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }


        public void Run() {
            if (_isRunning) return;
            _isRunning = true;
            // RunSimulation();
            _histogramThread.Start();
        }

        public void Stop() {
            if (!_isRunning) return;
            _isRunning = false;
        }

        private void RunSimulation() {
            Stopwatch sw = new Stopwatch();
            while (_isRunning) {
                sw.Start();
                double value = _histogramRunner.AddTrial(_experimentSize, _successChance);
                Trace.WriteLine($"Adding {value} to histogram.");
                sw.Stop();
                int waitTime = _delayMilli - (int)sw.ElapsedMilliseconds;
                Thread.Sleep(Math.Max(0, waitTime));
                sw.Reset();
            }
        }
    }
}