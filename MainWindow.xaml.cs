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

        public MainWindow() {
            InitializeComponent();
            HistogramSetup();
        }

        private void HistogramSetup() {
            _histogramRunner = new HistogramRunner {
                ExperimentSize = 1000,
                SuccessChance = successSlider.Value,
                Delay = delaySlider.Value
            };
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {

        }

        private void StartButton_Click(object sender, RoutedEventArgs e) {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}