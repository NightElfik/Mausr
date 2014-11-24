using System;

namespace Mausr.Core.NeuralNet {
	public interface INeuronActivationFunc {
		
		Func<double, double> Get();

	}
}
