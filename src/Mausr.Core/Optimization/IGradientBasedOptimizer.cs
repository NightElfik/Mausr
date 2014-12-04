using System;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimizer {
		
		bool Optimize(Vector<double> resultAndInitPosition, IFunctionWithDerivative function,
				Action<Vector<double>> iterationCallback, CancellationToken ct);

	}
}
