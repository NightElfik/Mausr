using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.NeuralNet;

namespace Mausr.Tests.NeuralNet {
	public static class NetBuilder {
		
		/// <summary>
		/// Creates neural net that computes AND function.
		/// </summary>
		public static Net CreateAndNet() {
			var net = new Net(new NetLayout(2, 1), new SigomidActivationFunc(), new NetCostFunction());
			net.SetCoefsMatrix(0, DenseMatrix.Build.DenseOfArray(new double[,] { { -30 }, { 20 }, { 20 } }));
			return net;
		}
		
		/// <summary>
		/// Creates neural net that computes XNOR function.
		/// </summary>
		public static Net CreateNxorNet() {
			var net = new Net(new NetLayout(2, 2, 1), new SigomidActivationFunc(), new NetCostFunction());
			net.SetCoefsMatrix(0, DenseMatrix.Build.DenseOfArray(new double[,] { { -30, 10 }, { 20, -20 }, { 20, -20 } }));
			net.SetCoefsMatrix(1, DenseMatrix.Build.DenseOfArray(new double[,] { { -10 }, { 20 }, { 20 } }));
			return net;
		}

		/// <summary>
		/// Creates neural net that computes AND, and NOT AND functions.
		/// </summary>
		public static Net CreateAndNandNet() {
			var net = new Net(new NetLayout(2, 2), new SigomidActivationFunc(), new NetCostFunction());
			net.SetCoefsMatrix(0, DenseMatrix.Build.DenseOfArray(new double[,] { { -30, 30 }, { 20, -20 }, { 20, -20 } }));
			return net;
		}
		

	}
}
