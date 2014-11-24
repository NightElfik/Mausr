using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mausr.Tests {
	[TestClass]
	public class MathNetExtensionsTests {

		[TestMethod]
		public void SingleMatrixPackTest() {
			var matrix = new Matrix<double>[] { DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }) };
			var expected = DenseVector.OfArray(new double[] { 1, 4, 2, 5, 3, 6 });
			var actual = matrix.Pack();

			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MultiMatrixPackTest() {
			var matrix = new Matrix<double>[] {
				DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }),
				DenseMatrix.OfArray(new double[,] { { 7, 8 }, { 9, 0 } })
			};
			var expected = DenseVector.OfArray(new double[] { 1, 4, 2, 5, 3, 6, 7, 9, 8, 0 });
			var actual = matrix.Pack();

			CollectionAssert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleMatrixUnpackTest() {
			var actual = new Matrix<double>[] { new DenseMatrix(2, 3) };
			var expected = new Matrix<double>[] { DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }) };
			var data = DenseVector.OfArray(new double[] { 1, 4, 2, 5, 3, 6 });
			data.Unpack(actual);

			for (int i = 0; i < expected.Length; ++i) {
				CollectionAssert.AreEqual(expected[i].Enumerate().ToList(), actual[i].Enumerate().ToList());
			}
		}

		[TestMethod]
		public void MultiMatrixUnpackTest() {
			var actual = new Matrix<double>[] { new DenseMatrix(2, 3), new DenseMatrix(2, 2) };
			var expected = new Matrix<double>[] {
				DenseMatrix.OfArray(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }),
				DenseMatrix.OfArray(new double[,] { { 7, 8 }, { 9, 0 } })
			};
			var data = DenseVector.OfArray(new double[] { 1, 4, 2, 5, 3, 6, 7, 9, 8, 0 });
			data.Unpack(actual);

			for (int i = 0; i < expected.Length; ++i) {
				CollectionAssert.AreEqual(expected[i].Enumerate().ToList(), actual[i].Enumerate().ToList());
			}
		}
	}
}
