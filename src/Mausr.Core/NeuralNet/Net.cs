using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	[Serializable]
	public class Net {

		public Matrix<double>[] Coefficients { get; private set; }

		public NetLayout Layout { get; private set; }

		public NeuronActivationFunc NeuronActivationFunc { get; private set; }

		public INetCostFunction CostFunction { get; private set; }

		/// <summary>
		/// Maps output index to zero-based neuron index.
		/// </summary>
		private Dictionary<int, int> outputToIndexMap;

		/// <summary>
		/// Maps output neuron index to output index.
		/// </summary>
		private int[] indexToOutputMap;



		public Net(NetLayout layout, NeuronActivationFunc activationFunc, NetCostFunction costFunction) {
			Layout = layout;
			NeuronActivationFunc = activationFunc;
			CostFunction = costFunction;
			Coefficients = layout.AllocateCoefMatrices();

			costFunction.Initialize(this);
		}

		public Matrix<double> GetCoefsMatrix(int index) {
			Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Layout.CoefsCount);
			Contract.Ensures(Contract.Result<Matrix<double>>().RowCount == Layout.GetCoefMatrixRows(index));
			Contract.Ensures(Contract.Result<Matrix<double>>().ColumnCount == Layout.GetCoefMatrixCols(index));
			return Coefficients[index];
		}

		public void SetCoefsMatrix(int index, Matrix<double> value) {
			Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index < Layout.CoefsCount);
			Contract.Requires<ArgumentException>(value.RowCount == Layout.GetCoefMatrixRows(index));
			Contract.Requires<ArgumentException>(value.ColumnCount == Layout.GetCoefMatrixCols(index));
			Coefficients[index] = value;
		}

		public void SetOutputMap(int[] outNeuronIndexToOutputIndexMap) {
			Contract.Requires<ArgumentException>(outNeuronIndexToOutputIndexMap.Length == Layout.OutputSize);

			int outSize = Layout.OutputSize;
			indexToOutputMap = new int[outSize];

			Array.Copy(outNeuronIndexToOutputIndexMap, indexToOutputMap, outSize);

			outputToIndexMap = new Dictionary<int, int>();
			for (int i = 0; i < outSize; ++i) {
				outputToIndexMap.Add(indexToOutputMap[i], i);
			}
		}

		public int MapOutputToOutNeuron(int outputIndex) {
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() < Layout.OutputSize);

			if (outputToIndexMap != null) {
				return outputToIndexMap[outputIndex];
			}
			else {
				return outputIndex;  // Identity by default.
			}
		}

		public int MapOutNeuronToOutput(int outNeuronIndex) {
			Contract.Requires<ArgumentOutOfRangeException>(outNeuronIndex >= 0 && outNeuronIndex < Layout.OutputSize);

			if (indexToOutputMap != null) {
				return indexToOutputMap[outNeuronIndex];
			}
			else {
				return outNeuronIndex;  // Identity by default.
			}
		}

	}
}
