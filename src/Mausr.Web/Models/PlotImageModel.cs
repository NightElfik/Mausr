using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class PlotImageModel {

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
	}
}