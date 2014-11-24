using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimizer {

		bool Optimize(DenseVector result, IFunctionWithDerivative function, DenseVector initialPosition);

		bool Optimize(List<DenseVector> steps, IFunctionWithDerivative function, DenseVector initialPosition);

	}
}
