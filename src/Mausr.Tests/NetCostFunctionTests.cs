using System;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	[TestClass]
	public class NetCostFunctionTests {
		[TestMethod]
		public void EvaluateNxorAndNandnNoRegularizationTest() {
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
	}
}
