using System;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	public class Net {

		private Matrix<double>[] coefficients;

		public NetLayout Layout { get; private set; }

		public INeuronActivationFunc NeuronActivationFunc { get; private set; }



		public Net(NetLayout layout, INeuronActivationFunc neuronActivationFunc) {
			Layout = layout;
			NeuronActivationFunc = neuronActivationFunc;
			coefficients = new DenseMatrix[layout.CoefsCount];
		}


		public Matrix<double> GetCoefsMatrix(int index) {
			Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Layout.CoefsCount);
			Contract.Ensures(Contract.Result<Matrix<double>>().RowCount == Layout.GetCoefMatrixRows(index));
			Contract.Ensures(Contract.Result<Matrix<double>>().ColumnCount == Layout.GetCoefMatrixCols(index));
			return coefficients[index];
		}

		public void SetCoefsMatrix(int index, Matrix<double> value) {
			Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Layout.CoefsCount);
			Contract.Requires<ArgumentException>(value.RowCount == Layout.GetCoefMatrixRows(index));
			Contract.Requires<ArgumentException>(value.ColumnCount == Layout.GetCoefMatrixCols(index));
			coefficients[index] = value;
		}

	}
}
