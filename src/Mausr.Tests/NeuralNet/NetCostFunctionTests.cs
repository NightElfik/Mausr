using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Mausr.Tests.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.NeuralNet {
	[TestClass]
	public class NetCostFunctionTests {

		[TestMethod]
		public void EvaluateAndNandNoRegularizationTest() {
			var net = NetBuilder.CreateAndNandNet();
			var func = net.CostFunction;
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
			{
				var outputIndices = new int[] { 1, 1, 1, 0 };
				func.SetInputsOutputs(inputs, outputIndices, 0);

				double value = func.Evaluate(net.Coefficients.Pack());
				Assert.IsTrue(value < 0.001);
			}
			{
				var outputIndices = new int[] { 0, 0, 0, 1 };
				func.SetInputsOutputs(inputs, outputIndices, 0);

				double value = func.Evaluate(net.Coefficients.Pack());
				Assert.IsTrue(value > 10);
			}
		}

		[TestMethod]
		public void DerivativeAndNandNoRegularizationTest() {
			var net = NetBuilder.CreateAndNandNet();
			var func = net.CostFunction;
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
			{
				var outputIndices = new int[] { 1, 1, 1, 0 };
				func.SetInputsOutputs(inputs, outputIndices, 0);
				int dims = func.DimensionsCount;

				var rand = new Random(0);
				FunctionDerivativeTester.PerformDerivativeTest(func,
					Enumerable.Range(0, 100).Select(x =>
						Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
					).ToArray()
				);
			}
			{
				var outputIndices = new int[] { 0, 0, 0, 1 };
				func.SetInputsOutputs(inputs, outputIndices, 0);
				int dims = func.DimensionsCount;

				var rand = new Random(0);
				FunctionDerivativeTester.PerformDerivativeTest(func,
					Enumerable.Range(0, 100).Select(x =>
						Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
					).ToArray()
				);
			}
		}

		[TestMethod]
		public void DerivativeNxorNoRegularizationTest() {
			var net = NetBuilder.CreateNxorNet();
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 1, 1 } });
			var outputIndices = new int[] { 0, 0 };
			net.CostFunction.SetInputsOutputs(inputs, outputIndices, 0);
			int dims = net.CostFunction.DimensionsCount;

			var rand = new Random(0);
			FunctionDerivativeTester.PerformDerivativeTest(net.CostFunction,
				Enumerable.Range(0, 100).Select(x =>
					Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
				).ToArray()
			);

		}

		[TestMethod]
		public void DerivativeDeepNetTest() {
			var net = new Net(new NetLayout(5, 10, 5, 7, 2), new SigomidActivationFunc(), new NetCostFunction());
			var inputs = DenseMatrix.OfArray(new double[,] {
				{ 0, 0, 1, 0, 0 }, 
				{ 1, 1, 0, 1, 1 }, 
				{ 1, 0, 0, 1, 0 }, 
				{ 1, 0, 1, 0, 1 }, 
				{ 0, 1, 0, 1, 0 },
			});
			var outputIndices = new int[] { 0, 1, 0, 0, 1 };
			net.CostFunction.SetInputsOutputs(inputs, outputIndices, 0);
			int dims = net.CostFunction.DimensionsCount;

			var rand = new Random(0);
			FunctionDerivativeTester.PerformDerivativeTest(net.CostFunction,
				Enumerable.Range(0, 20).Select(x =>
					Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
				).ToArray()
			);

		}

	}
}
