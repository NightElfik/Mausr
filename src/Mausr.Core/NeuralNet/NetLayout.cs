using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Mausr.Core.NeuralNet {
	public class NetLayout {

		private int[] layersSizes;


		public int InputSize { get { return layersSizes[0]; } }

		public int OutputSize { get { return layersSizes[layersSizes.Length - 1]; } }
		
		public int LayersCount { get { return layersSizes.Length; } }

		public int CoefsCount { get { return layersSizes.Length - 1; } }



		/// <summary>
		/// Initializes net layout with layers sizes.
		/// Directly uses the given array without a copy.
		/// </summary>
		public NetLayout(params int[] sizes) {
			Contract.Requires(sizes.Length >= 2);
			layersSizes = sizes;
		}

		public NetLayout(int inputSize, IEnumerable<int> hiddenSizes, int outputSize) {
			var sizes = new List<int>();
			sizes.Add(inputSize);
			sizes.AddRange(hiddenSizes);
			sizes.Add(outputSize);
			layersSizes = sizes.ToArray();
		}


		public int GetLayerSize(int layerIndex) {
			Contract.Requires(layerIndex >= 0 && layerIndex < LayersCount);
			return layersSizes[layerIndex];
		}

		[Pure]
		public int GetCoefMatrixRows(int coefIndex) {
			Contract.Requires(coefIndex >= 0 && coefIndex < CoefsCount);
			return layersSizes[coefIndex] + 1;
		}
		
		[Pure]
		public int GetCoefMatrixCols(int coefIndex) {
			Contract.Requires(coefIndex >= 0 && coefIndex < CoefsCount);
			return layersSizes[coefIndex + 1];
		}

	}
}
