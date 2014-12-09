using System;
using System.Collections.Generic;

namespace Mausr.Web.NeuralNet {
	[Serializable]
	public class TrainData {	

		public List<int> IteraionNumbers = new List<int>();

		public List<float> TrainCosts = new List<float>();
		public List<float> TestCosts = new List<float>();

		public List<float> TrainPredicts = new List<float>();
		public List<float> TestPredicts = new List<float>();

	}
}