using MathNet.Numerics.LinearAlgebra;

namespace Mausr.Core.Optimization {
	public interface IFunctionWithDerivative {

		int DimensionsCount { get; }

		double Evaluate(Vector<double> point);

		void Derivate(Vector<double> resultDerivative, Vector<double> point);

	}
}
