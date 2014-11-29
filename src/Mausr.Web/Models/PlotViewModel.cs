using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class PlotViewModel {

		[Required]
		public double OriginX { get; set; }

		[Required]
		public double OriginY { get; set; }

		[Required]
		[Range(32, 4096)]
		public int Width { get; set; }

		[Required]
		[Range(32, 4096)]
		public int Height { get; set; }

		[Required]
		public double StepPerPixel { get; set; }

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
		public double MinDerivMagn { get; set; }

		[Range(1, 8192)]
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