using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.Optimization;

namespace Mausr.Core.NeuralNet {
	public interface INetCostFunction : IFunctionWithDerivative {

		void Initialize(Net network);

		void SetInputsOutputs(Matrix<double> inputs, int[] outputIndices, double regularizationLambda);
		
		double Evaluate(Matrix<double>[] coefs, Matrix<double> inputs, int[] outputIndices, double regularizationLambda);

		void Derivate(Vector<double> resultDerivative, Matrix<double>[] coefs, Matrix<double> inputs,
			int[] outputIndices, double regularizationLambda);
	}
}
