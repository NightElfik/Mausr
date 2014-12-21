using System;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimizer {
		
		bool Optimize(Vector<double> resultAndInitPos, IFunctionWithDerivative function, double minDerivCompMaxMagn,
				Action<int, Func<Vector<double>>> iterationCallback, CancellationToken ct);

	}
}
