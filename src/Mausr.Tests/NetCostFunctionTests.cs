using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	[TestClass]
	public class NetCostFunctionTests {

		[TestMethod]
		public void EvaluateAndNotandNoRegularizationTest() {
			var net = NetBuilder.CreateAndNotandNet();
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
			{
				var outputIndices = new int[] { 1, 1, 1, 0 };
				var func = new NetCostFunction(net, inputs, outputIndices, 0);

				double value = func.Evaluate(net.Coefficients.Pack());
				Assert.IsTrue(value < 0.001);
			}
			{
				var outputIndices = new int[] { 0, 0, 0, 1 };
				var func = new NetCostFunction(net, inputs, outputIndices, 0);

				double value = func.Evaluate(net.Coefficients.Pack());
				Assert.IsTrue(value > 10);
			}
		}

		[TestMethod]
		public void DerivativeAndNotandNoRegularizationTest() {
			var net = NetBuilder.CreateAndNotandNet();
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
			{
				var outputIndices = new int[] { 1, 1, 1, 0 };
				var func = new NetCostFunction(net, inputs, outputIndices, 0);
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
				var func = new NetCostFunction(net, inputs, outputIndices, 0);
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
			var func = new NetCostFunction(net, inputs, outputIndices, 0);
			int dims = func.DimensionsCount;

			var rand = new Random(0);
			FunctionDerivativeTester.PerformDerivativeTest(func,
				Enumerable.Range(0, 100).Select(x =>
					Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
				).ToArray()
			);

		}

		[TestMethod]
		public void DerivativeDeepNetTest() {
			var net = new Net(new NetLayout(5, 10, 5, 7, 2), new SigomidActivationFunc());
			var inputs = DenseMatrix.OfArray(new double[,] {
				{ 0, 0, 1, 0, 0 }, 
				{ 1, 1, 0, 1, 1 }, 
				{ 1, 0, 0, 1, 0 }, 
				{ 1, 0, 1, 0, 1 }, 
				{ 0, 1, 0, 1, 0 },
			});
			var outputIndices = new int[] { 0, 1, 0, 0, 1 };
			var func = new NetCostFunction(net, inputs, outputIndices, 0);
			int dims = func.DimensionsCount;

			var rand = new Random(0);
			FunctionDerivativeTester.PerformDerivativeTest(func,
				Enumerable.Range(0, 20).Select(x =>
					Enumerable.Range(0, dims).Select(y => rand.NextDouble() * 10 - 5).ToArray()
				).ToArray()
			);

		}

	}
}
