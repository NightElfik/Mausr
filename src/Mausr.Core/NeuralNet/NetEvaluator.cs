using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	public class NetEvaluator {

		public Net Net { get; private set; }


		public NetEvaluator(Net network) {
			Net = network;
		}

		/// <param name="inputs">Matrix with data in rows.</param>
		public Matrix<double> Evaluate(Matrix<double> inputs) {
			return Evaluate(inputs, Net.Coefficients);
		}

		public Matrix<double> Evaluate(Matrix<double> inputs, Matrix<double>[] customCoefs) {
			Contract.Requires(inputs.ColumnCount == Net.Layout.InputSize);
			Contract.Ensures(Contract.Result<Matrix<double>>().ColumnCount == Net.Layout.OutputSize);

			Matrix<double> result = inputs;

			for (int i = 0; i < customCoefs.Length; ++i) {
				result = evalStep(result, customCoefs[i]);
			}

			return result;
		}

		public int[] PredictFromEvaluated(Matrix<double> outpus) {
			return outpus.EnumerateRows().Select(row => Net.MapOutNeuronToOutput(row.MaximumIndex())).ToArray();
		}

		public int[] Predict(Matrix<double> inputs) {
			return PredictFromEvaluated(Evaluate(inputs));
		}

		public int[] Predict(Matrix<double> inputs, Matrix<double>[] customCoefs) {
			return PredictFromEvaluated(Evaluate(inputs, customCoefs));
		}

		public IEnumerable<Prediction> PredictTopN(Vector<double> input, int maxPredictionsCount, double minActivation) {
			var outValues = Evaluate(DenseMatrix.OfRowVectors(input));
			Contract.Assert(outValues.RowCount == 1);
			return PredictTopNFromEvaluated(outValues.Row(0), maxPredictionsCount, minActivation);
		}

		public IEnumerable<Prediction> PredictTopNFromEvaluated(Vector<double> output, int maxPredictionsCount, double minActivation) {
			var outValsAndIndices = new List<Prediction>();
			for (int i = 0; i < output.Count; ++i) {
				double activation = output[i];
				if (activation < minActivation) {
					continue;
				}

				outValsAndIndices.Add(new Prediction() {
					OutputId = Net.MapOutNeuronToOutput(i),
					NeuronOutputValue = activation
				});
			}

			return outValsAndIndices.OrderByDescending(x => x.NeuronOutputValue).Take(maxPredictionsCount);
		}


		public IEnumerable<Prediction>[] PredictTopN(Matrix<double> inputs, int maxPredictionsCount, double minActivation) {
			Contract.Ensures(Contract.Result<IEnumerable<Prediction>[]>().Length == inputs.RowCount);

			var outValues = Evaluate(inputs);
			return outValues.EnumerateRows()
				.Select(row => PredictTopNFromEvaluated(row, maxPredictionsCount, minActivation))
				.ToArray();
		}


		protected Matrix<double> evalStep(Matrix<double> input, Matrix<double> coefs) {
			var result = input.InsertColumn(0, DenseVector.Create(input.RowCount, 1));
			result = result * coefs;
			result.MapInplace(Net.NeuronActivationFunc.Evaluate);
			return result;
		}

	}
}
