using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Core.Plot;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests.NeuralNet {
	[TestClass]
	public class NetTrainerTests {
		[TestMethod]
		public void TrainAndNandTest() {
			var net = NetBuilder.CreateAndNandNet();
			var inputs = DenseMatrix.OfArray(new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } });
			var outputIndices = new int[] { 1, 1, 1, 0 };
			performTrainTest(net, inputs, outputIndices, 1, 0.05, 1024);
			//performTrainTestWithPlots(net, inputs, outputIndices, 1, 0.05, 1024, 0.05);
		}
		
		private void performTrainTest(Net net, Matrix<double> inputs, int[] outputIndices,
				double learningRate, double regularizationLambda, int maxIters) {
			var optimizer = new SteepestDescentAdvancedOptmizer(learningRate, 0.6, 0.99, 1e-4, maxIters);
			var trainer = new NetTrainer(net, optimizer, regularizationLambda);

			bool result = trainer.Train(inputs, outputIndices, null);

			var actualOutputIndices = trainer.Predict(inputs);
			CollectionAssert.AreEqual(outputIndices, actualOutputIndices);

			Assert.IsTrue(result);
		}

		private void performTrainTestWithPlots(Net net, Matrix<double> inputs, int[] outputIndices,
				double learningRate, double regularizationLambda, int maxIters, double stepPerPixel) {
			var optimizer = new SteepestDescentAdvancedOptmizer(learningRate, 0.6, 0.99, 1e-4, maxIters);
			var trainer = new NetTrainer(net, optimizer, regularizationLambda);

			var cf = new NetCostFunction(net, inputs, outputIndices, regularizationLambda);
			trainer.InitializeCoefs(net);
			double costBefore = cf.Evaluate(net.Coefficients.Pack());

			var optimizationSteps = new List<Vector<double>>();
			bool result = trainer.Train(inputs, outputIndices, p => optimizationSteps.Add(p));			

			new FunctionPlotter()
				.FunctionPlot(optimizationSteps.Select(pt => cf.Evaluate(pt)).ToArray(), 1024, 512, 0, 2)
				.Save("err.png", ImageFormat.Png);

			int dims = cf.DimensionsCount;
			var plotOrigin = net.Coefficients.Pack();
			for (int dx = 0; dx < dims; ++dx) {
				for (int dy = dx + 1; dy < dims; ++dy) {
					plotOptimization(cf, plotOrigin, dx, dy, stepPerPixel, optimizationSteps);
				}
			}

			double costAfter = cf.Evaluate(net.Coefficients.Pack());
			Assert.IsTrue(costAfter < costBefore);

			var actualOutputIndices = trainer.Predict(inputs);
			CollectionAssert.AreEqual(outputIndices, actualOutputIndices);

			Assert.IsTrue(result);
		}

		private void plotOptimization(IFunctionWithDerivative fun, Vector<double> origin, int xDimIndex, int yDimIndex,
				double stepPerPixel, List<Vector<double>> points) {

			var plotter = new FunctionPlotter();
			var img = plotter.ContourPlot(fun, origin, xDimIndex, yDimIndex, 512, 512, stepPerPixel, 10, 0.5,
				new List<Tuple<Color, List<Vector<double>>>>() { 
					new Tuple<Color, List<Vector<double>>>(Color.DarkGreen, points) 
				});
			img.Save(string.Format("x={0};y={1}.png", xDimIndex, yDimIndex), ImageFormat.Png);
		}
	}
}
