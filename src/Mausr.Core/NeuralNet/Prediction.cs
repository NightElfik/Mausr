
namespace Mausr.Core.NeuralNet {
	public struct Prediction {

		public int OutputId;
		public float NeuronOutputValue;


		public Prediction(int outId, float neuronOutput) {
			OutputId = outId;
			NeuronOutputValue = neuronOutput;
		}

	}
}
