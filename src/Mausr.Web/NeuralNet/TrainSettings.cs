using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.NeuralNet {
	public class TrainSettings {
		
		[Display(Name = "Network ID")]
		public string NetId { get; set; }

		[Required]
		[MaxLength(64)]
		[MinLength(4)]
		[Display(Name = "Network name")]
		public string NetName { get; set; }
		
		
		[Display(Name = "Hidden layers sizes")]
		public int[] HiddenLayersSizes { get; set; }
		
		[Required]
		[Range(16, 64)]
		[Display(Name = "Input image edge size")]
		public int InputImgSizePx { get; set; }

		[Required]
		[Range(1, 20)]
		[Display(Name = "Pen thickness")]
		public int PenThicknessPerc { get; set; }

		[Required]
		[Display(Name = "Normalize input data")]
		public bool NormalizeInput { get; set; }

		[Required]
		[Range(0, 30)]
		[Display(Name = "Generate extra inputs by rotation")]
		public int GenerateExtraInputsByRotation { get; set; }

		
		[Required]
		[Range(1, 64)]
		[Display(Name = "Learning rounds")]
		public int LearnRounds { get; set; }

		[Required]
		[Range(0, 65536)]
		[Display(Name = "Batch size")]
		public int BatchSize { get; set; }

		[Required]
		[Range(10, 1024)]
		[Display(Name = "Max iterations per batch")]
		public int MaxIteratinosPerBatch { get; set; }

		[Required]
		[Range(0, 10)]
		[Display(Name = "Regularization")]
		public double RegularizationLambda { get; set; }

		[Required]
		[Range(0, 1)]
		[Display(Name = "Min deriv. comp. max magnitude")]
		public double MinDerivCompMaxMagn { get; set; }
				
		[Required]
		[Display(Name = "Optimization algorithm")]
		public OptimizationAlgorithm OptimizationAlgorithm { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		[Display(Name = "Initialization seed")]
		public int InitSeed { get; set; }
		
		[Required]
		[Range(0.001, 10)]
		[Display(Name = "Learning rate")]
		public double LearningRate { get; set; }

		[Required]
		[Range(0, 100)]
		[Display(Name = "Start momentum")]
		public int MomentumStartPerc { get; set; }

		[Required]
		[Range(0, 100)]
		[Display(Name = "End momentum")]
		public int MomentumEndPerc { get; set; }

		[Required]
		[Range(0.001, 10)]
		[Display(Name = "Rprop initial step")]
		public double RpropInitStep { get; set; }

		[Required]
		[Range(0.1, 100)]
		[Display(Name = "Rprop max step")]
		public double RpropMaxStep { get; set; }

		[Required]
		[Range(1.1, 2)]
		[Display(Name = "Rprop step up mult")]
		public double RpropStepUpMult { get; set; }

		[Required]
		[Range(0.1, 0.9)]
		[Display(Name = "Rprop step down mult")]
		public double RpropStepDownMult { get; set; }


		[Required]
		[Range(5, 30)]
		[Display(Name = "Test data set size")]
		public int TestDataSetSizePerc { get; set; }

		[Required]
		[Range(1, 32)]
		[Display(Name = "Training evaluation interval")]
		public int TrainEvaluationIters { get; set; }

		[Required]
		[Range(0, 32)]
		[Display(Name = "Skip train evaluation")]
		public int SkipFrstIters { get; set; }


	}

	public enum OptimizationAlgorithm {

		BasicGradientDescent,
		NesterovSutskeverGradientDescent,

		RpropPlus,
		ImprovedRpropMinus,

	}
}