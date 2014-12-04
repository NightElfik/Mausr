using System;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.Optimization;

namespace Mausr.Core.NeuralNet {
	public abstract class NeuronActivationFunc : IFunctionWithDerivative {
		
		public int DimensionsCount {
			get { return 1; }
		}

		public abstract double Evaluate(double value);

		public abstract double Derivate(double value);
		

		public double Evaluate(Vector<double> point) {
			return Evaluate(point[0]);
		}

		public void Derivate(Vector<double> resultDerivative, Vector<double> point) {
			resultDerivative[0] = Derivate(point[0]);
		}

	}
}
