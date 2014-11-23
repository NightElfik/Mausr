using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.Optimization {
	public interface IGradientBasedOptimization {

		void Optimize(IFunctionWithDerivative function);

	}
}
