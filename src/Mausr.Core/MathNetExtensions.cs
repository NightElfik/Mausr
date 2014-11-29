using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core {
	public static class MathNetExtensions {

		/// <summary>
		/// Packs given matrices to the array.
		/// </summary>
		public static Vector<double> Pack(this Matrix<double>[] matrices) {
			Contract.Requires(matrices != null && matrices.Length > 0);
			Contract.Requires(Contract.ForAll(matrices, m => m != null));

			int size = 0;
			for (int i = 0; i < matrices.Length; ++i) {
				size += matrices[i].RowCount * matrices[i].ColumnCount;
			}

			var result = new DenseVector(size);
			PackTo(matrices, result);
			return result;
		}
	
		/// <summary>
		/// Packs given matrices to given array.
		/// </summary>
		public static void PackTo(this Matrix<double>[] matrices, Vector<double> result) {
			Contract.Requires(matrices != null && matrices.Length > 0);
			Contract.Requires(Contract.ForAll(matrices, m => m != null));
			Contract.Requires(result != null);
			Contract.Requires(result.Count == matrices.Sum(m => m.RowCount * m.ColumnCount));

			int globalIndex = 0;
			for (int i = 0; i < matrices.Length; ++i) {
				var m = matrices[i];
				int rows = m.RowCount;
				int cols = m.ColumnCount;

				for (int c = 0; c < cols; ++c) {
					for (int r = 0; r < rows; ++r) {
						result[globalIndex++] = m[r, c];
					}
				}
			}
		}

		/// <summary>
		/// Packs given matrices to the array.
		/// </summary>
		public static void UnpackTo(this Vector<double> packedMatrices, Matrix<double>[] resultMatrices) {
			Contract.Requires(packedMatrices != null);
			Contract.Requires(resultMatrices != null && resultMatrices.Length > 0);
			Contract.Requires(Contract.ForAll(resultMatrices, m => m != null));
			Contract.Requires(packedMatrices.Count == resultMatrices.Sum(m => m.RowCount * m.ColumnCount));

			int globalIndex = 0;
			for (int i = 0; i < resultMatrices.Length; ++i) {
				var m = resultMatrices[i];
				int rows = m.RowCount;
				int cols = m.ColumnCount;

				for (int c = 0; c < cols; ++c) {
					for (int r = 0; r < rows; ++r) {
						m[r, c] = packedMatrices[globalIndex++];
					}
				}
			}
		}

	}
}
