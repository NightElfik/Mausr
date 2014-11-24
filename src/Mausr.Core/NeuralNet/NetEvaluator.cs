using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	public class NetEvaluator {

		public Net Net { get; private set; }


		public NetEvaluator(Net network) {
			Net = network;
		}

		/// <summary>
		/// Evaluates given neural network with given data.
		/// </summary>
		/// <param name="inputs">Matrix with data in rows.</param>
		public Matrix<double> Evaluate(Matrix<double> inputs) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);
			Contract.Ensures(Contract.Result<Matrix<double>>().ColumnCount == Net.Layout.OutputSize);

			Matrix<double> result = inputs;

			for (int i = 0; i < Net.Layout.CoefsCount; ++i) {
				result = result.InsertColumn(0, DenseVector.Create(result.RowCount, 1));
				result = result.Multiply(Net.GetCoefsMatrix(i));
				result.Map(Net.NeuronActivationFunc.Get(), result);
			}

			return result;
		}

	}
}
