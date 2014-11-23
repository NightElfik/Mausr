using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mausr.Core.NeuralNet {
	public class NetLayout {

		private List<int> hiddenLayersSizes = new List<int>();


		public int InputSize { get; private set; }

		public int OutputSize { get; private set; }

		public int HiddenLayersCount { get { return hiddenLayersSizes.Count; } }

		public int TotalLayersCount { get { return 2 + hiddenLayersSizes.Count; } }


		public int GetHiddenLayerSize(int index) {
			Contract.Requires(index >= 0 && index < HiddenLayersCount);
			return hiddenLayersSizes[index];
		}

	}
}
