using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimizer {

		bool Optimize(Vector<double> result, IFunctionWithDerivative function, Vector<double> initialPosition);

		bool Optimize(List<Vector<double>> steps, IFunctionWithDerivative function, Vector<double> initialPosition);

	}
}
