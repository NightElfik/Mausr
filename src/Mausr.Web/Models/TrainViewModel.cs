using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class TrainViewModel {
		
		[Required]
		[MaxLength(64)]
		[MinLength(4)]
		[Display(Name = "Network name")]
		public string NetName { get; set; }
		
		[Required]
		[MaxLength(32)]
		[Display(Name = "Hidden layers sizes")]
		public string HiddenLayersSizes { get; set; }
		
		
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
		[Range(1, 65536)]
		[Display(Name = "Batch size")]
		public int BatchSize { get; set; }

		[Required]
		[Range(10, 65536)]
		[Display(Name = "Max iterations per batch")]
		public int MaxIteratinosPerBatch { get; set; }

		[Required]
		[Range(0, 10)]
		[Display(Name = "Regularization")]
		public double RegularizationLambda { get; set; }
		
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
		[Range(0, 1)]
		[Display(Name = "Min derivative magnitude")]
		public double MinDerivativeMagnitude { get; set; }


		[Required]
		[Range(5, 30)]
		[Display(Name = "Test data set size")]
		public int TestDataSetSizePerc { get; set; }

		
		public int OutputSize { get; set; }

		public int TrainingSamples { get; set; }

		public IEnumerable<SymbolDrawing> ExampleDrawings { get; set; }

	}
}