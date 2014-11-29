using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mausr.Web.Models {
	public class TrainViewModel {
		
		[Required]
		[MaxLength(64)]
		public string NetName { get; set; }
		
		[Required]
		[MaxLength(32)]
		public string HiddenLayersSizes { get; set; }

		[Required]
		[Range(16, 64)]
		public int InputImgSizePx { get; set; }
		
		[Required]
		[Range(0, 10)]
		public double RegularizationLambda { get; set; }
		
		[Required]
		[Range(0.001, 10)]
		public double LearningRate { get; set; }

		[Required]
		[Range(0, 1)]
		public double MomentumStart { get; set; }

		[Required]
		[Range(0, 1)]
		public double MomentumEnd { get; set; }

		[Required]
		[Range(0, 1)]
		public double MinDerivativeMagnitude { get; set; }

		[Required]
		[Range(10, 65536)]
		public int MaxIteratinos { get; set; }

	}
}