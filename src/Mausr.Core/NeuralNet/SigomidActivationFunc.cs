using System;
using System.Diagnostics.Contracts;

namespace Mausr.Core.NeuralNet {
	[Serializable]
	public class SigomidActivationFunc : NeuronActivationFunc {
		
		public override double Evaluate(double value) {
			return 1.0 / (1.0 + Math.Exp(-value));
		}
				
		public override double Derivate(double value) {
			double x = 1.0 / (1.0 + Math.Exp(-value));
			return x * (1.0 - x);
		}
		
	}
}
