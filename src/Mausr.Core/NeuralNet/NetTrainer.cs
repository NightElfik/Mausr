using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.Optimization;

namespace Mausr.Core.NeuralNet {
	/// <summary>
	/// Neural network trainer using back-propagation algorithm.
	/// </summary>
	public class NetTrainer : NetEvaluator {

		public IGradientBasedOptimizer Optimizer { get; private set; }

		public NetTrainer(Net network, IGradientBasedOptimizer optimizer)
			: base(network) {

			Optimizer = optimizer;
		}


		/// <param name="inputs">Matrix with data in rows.</param>
		/// <param name="expectedOutputIndices">Vector of indices of output neurons that should be activated for each input.</param>
		public void Train(Matrix<double> inputs, Vector<int> expectedOutputIndices) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);





		}

	}
}
