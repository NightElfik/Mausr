using System;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.NeuralNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.NeuralNet {
	[TestClass]
	public class NetEvaluatorTests {
		[TestMethod]
		public void EvaluateAndTest() {
			var net = NetBuilder.CreateAndNet();
			performEvaluatorTest(net, 0, 0, 0);
			performEvaluatorTest(net, 0, 0, 1);
			performEvaluatorTest(net, 0, 1, 0);
			performEvaluatorTest(net, 1, 1, 1);
		}

		[TestMethod]
		public void EvaluateNxorTest() {
			var net = NetBuilder.CreateNxorNet();
			performEvaluatorTest(net, 1, 0, 0);
			performEvaluatorTest(net, 0, 0, 1);
			performEvaluatorTest(net, 0, 1, 0);
			performEvaluatorTest(net, 1, 1, 1);
		}

		[TestMethod]
		public void EvaluateAndNotandTest() {
			var net = NetBuilder.CreateAndNandNet();
			performEvaluatorTest(net, new int[] { 0, 1 }, 0, 0);
			performEvaluatorTest(net, new int[] { 0, 1 }, 0, 1);
			performEvaluatorTest(net, new int[] { 0, 1 }, 1, 0);
			performEvaluatorTest(net, new int[] { 1, 0 }, 1, 1);
		}


		public static void performEvaluatorTest(Net network, int expectedOutput, params double[] input) {
			performEvaluatorTest(network, new int[] { expectedOutput }, input);
		}

		public static void performEvaluatorTest(Net network, int[] expectedOutput, params double[] input) {
			var evaluator = new NetEvaluator(network);
			var actual = evaluator.Evaluate(DenseMatrix.Build.DenseOfRowArrays(input));

			Assert.AreEqual(1, actual.RowCount);
			Assert.AreEqual(expectedOutput.Length, actual.ColumnCount);

			for (int i = 0; i < expectedOutput.Length; ++i) {
				int actualInt = (int)(Math.Round(actual[0, i]));
				Assert.AreEqual(expectedOutput[i], actualInt);
			}

		}
	}
}
