using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;

namespace Mausr.Core.Optimization {
	public class NumericDerivator {


		private Func<Vector<double>, double> function;
		private double epsilon;


		public NumericDerivator(Func<Vector<double>, double> func, double epsilon) {
			function = func;
			this.epsilon = epsilon;
		}


		public void ComputeDerivative(Vector<double> resultDerivative, Vector<double> point) {
			Contract.Requires(resultDerivative.Count == point.Count);

			Vector<double> pt = point.Clone();
			int dimensions = point.Count;

			for (int i = 0; i < dimensions; ++i) {
				pt[i] = point[i] - epsilon;
				double v1 = function(pt);

				pt[i] = point[i] + epsilon;
				double v2 = function(pt);

				resultDerivative[i] = (v2 - v1) / (2 * epsilon);

				pt[i] = point[i];
			}
		}

	}
}
