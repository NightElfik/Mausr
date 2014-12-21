using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class PlotViewModel {

		[Required]
		[Range(32, 1024)]
		public int Width { get; set; }

		[Required]
		[Range(32, 1024)]
		public int Height { get; set; }

		[Required]
		[Range(1, 128)]
		public int ContoursCount { get; set; }

		[Required]
		public double ScalePower { get; set; }



		[Required]
		public double InitialX { get; set; }

		[Required]
		public double InitialY { get; set; }

		[Required]
		public double MinDerivCompMaxMagn { get; set; }

		[Range(1, 2048)]
		public int MaxIters { get; set; }


		[Required]
		public double BasicStep { get; set; }


		[Required]
		public double MomentumStep { get; set; }

		[Required]
		public double MomentumStart { get; set; }

		[Required]
		public double MomentumEnd { get; set; }

	}
}