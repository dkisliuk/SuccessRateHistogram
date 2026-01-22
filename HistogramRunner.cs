using System;
using System.Diagnostics;
using ScottPlot.Statistics;
using static System.Random;

namespace Plotting {

	public class HistogramRunner {
		private Histogram _histogram;
		private Random _random;

		public Histogram @Histogram { get { return _histogram; } }

		public HistogramRunner() {
			_histogram = Histogram.WithBinSize(1, 0, 100);
			_random = new Random();
		}

		public double AddTrial(int experimentSize, double successChance) {
			int successes = 0;
			for (int i = 0; i < experimentSize; i++) {
				if (_random.NextDouble() < successChance) {
					successes++;
				}
			}
			double value = (double)successes / experimentSize;
			_histogram.Add(value);
			return value;
		}

		public void ClearHistogram() {
			_histogram.Clear();
		}
	}
}