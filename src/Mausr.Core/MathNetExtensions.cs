using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core {
	public static class MathNetExtensions {

		public static void Abs(this DenseVector vector, DenseVector result) {
			Contract.Requires(vector.Count == result.Count);
			int len = vector.Count;
			for (int i = 0; i < len; ++i) {
				result[i] = Math.Abs(vector[i]);
			}
		}

	}
}
