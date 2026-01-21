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

        public MainWindow() {
            InitializeComponent();
            _histogramRunner = new HistogramRunner();
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {

        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine($"Start button pressed");
            if (_isRunning) {
                Stop();
                startButton.Content = "Start";
            }
            else {
                Run();
                startButton.Content = "Stop";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }


        public void Run() {
            if (_isRunning) return;
            _isRunning = true;
            // RunSimulation();
        }

        public void Stop() {
            if (!_isRunning) return;
            _isRunning = false;
        }

        private async Task RunSimulation() {
            Stopwatch sw = new Stopwatch();
            while (_isRunning) {
                sw.Start();
                _histogramRunner.AddTrial(Int32.Parse(experimentSize.Text), successSlider.Value);
                sw.Stop();
                int waitTime = (int)(delaySlider.Value * 1000) - (int)sw.ElapsedMilliseconds;
                await Task.Delay(Math.Max(0, waitTime));
                sw.Reset();
            }
        }
    }
}