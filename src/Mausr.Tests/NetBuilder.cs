using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.NeuralNet;

namespace Mausr.Tests {
	public static class NetBuilder {
		
		public static Net CreateAndNet() {
			var net = new Net(new NetLayout(2, 1), new SigomidActivationFunc());
			net.SetCoefsMatrix(0, DenseMatrix.Build.DenseOfArray(new double[,] { { -30 }, { 20 }, { 20 } }));
			return net;
		}

		public static Net CreateNxorNet() {
			var net = new Net(new NetLayout(2, 2, 1), new SigomidActivationFunc());
			net.SetCoefsMatrix(0, DenseMatrix.Build.DenseOfArray(new double[,] { { -30, 10 }, { 20, -20 }, { 20, -20 } }));
			net.SetCoefsMatrix(1, DenseMatrix.Build.DenseOfArray(new double[,] { { -10 }, { 20 }, { 20 } }));
			return net;
		}

	}
}
