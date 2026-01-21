using System;
using System.Diagnostics;
using ScottPlot.Statistics;
using static System.Random;

namespace Plotting {

	public class HistogramRunner {
		private Histogram _histogram;
		private Random _random;
		private bool _isRunning = false;

		private double _delay = 0;
		private double _successChance = .5;
		private int _experimentSize = 1000;

		public Histogram @Histogram { get { return _histogram; } }
		public double Delay { get { return _delay; } set { _delay = value; } }
		public double SuccessChance {
			get { return _successChance; } 
			set {
				// ClearHistogram();
				_successChance = value;
			}
		}
		public int ExperimentSize {
			get { return _experimentSize; }
			set {
				// ClearHistogram();
				_experimentSize = value;
			}
		}

		public HistogramRunner() {
			_histogram = Histogram.WithBinSize(1, 0, 100);
			_random = new Random();
		}

		private void AddTrial(int experimentSize, double successChance) {
			int successes = 0;
			for (int i = 0; i < experimentSize; i++) {
				if (_random.NextDouble() < successChance) {
					successes++;
				}
			}
			double value = (double)successes / experimentSize;
			_histogram.Add(value);
		}

		public void ClearHistogram() {
			_histogram.Clear();
		}

		public void Run(int experimentSize = 1000, double delay = 0, double successChance = 0.5) {
			_delay = delay;
			_experimentSize = experimentSize;
			_successChance = successChance;
			if (_isRunning) return;
			_isRunning = true;
			RunSimulation();
		}

		public void Stop() {
			if (!_isRunning) return;
			_isRunning = false;
		}

		private async void RunSimulation() {
			Stopwatch sw = new Stopwatch();
			while (_isRunning) {
				sw.Start();
				AddTrial(_experimentSize, _successChance);
				sw.Stop();
				int waitTime = (int)(_delay*1000) - (int)sw.ElapsedMilliseconds;
				await Task.Delay(Math.Max(0, waitTime));
				sw.Reset();
			}
		}
	}
}