using System;
using System.Diagnostics.Contracts;

namespace Mausr.Core.NeuralNet {
	public class SigomidActivationFunc : INeuronActivationFunc {

		[Pure]
		public double Evaluate(double value) {
			return 1.0 / (1.0 + Math.Exp(-value));
		}

		public Func<double, double> Get() {
			return Evaluate;
		}

	}
}
