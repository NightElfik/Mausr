using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.NeuralNet;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimizer {

		bool Optimize(Vector<double> result, IFunctionWithDerivative function, Vector<double> initialPosition,
			Action<Vector<double>> iterationCallback);

	}
}
