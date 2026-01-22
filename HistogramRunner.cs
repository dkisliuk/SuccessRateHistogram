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
			_histogram = Histogram.WithBinSize(.01, 0, 1);
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

		public double ComputeMean() {
			double mean = 0;

			int nBins = Histogram.Counts.Length;
			double halfBinWidth = (Histogram.Bins[1] - Histogram.Bins[0]) / 2;
			for (int i = 0; i < nBins; i++) {
				mean += Histogram.Counts[i] * (Histogram.Bins[i] + halfBinWidth);
			}
			mean /= Histogram.GetCumulativeCounts().Last();

            return mean;
		}

		public double ComputeXSquared() {
			double result = 0;

            int nBins = Histogram.Counts.Length;
            double halfBinWidth = (Histogram.Bins[1] - Histogram.Bins[0]) / 2;
            for (int i = 0; i < nBins; i++) {
                result += Histogram.Counts[i] * (Histogram.Bins[i] + halfBinWidth) * (Histogram.Bins[i] + halfBinWidth);
            }
            result /= Histogram.GetCumulativeCounts().Last();

            return result;
        }
	}
}