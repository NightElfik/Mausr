using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class LearnInitViewModel {
		
		[Required]
		[Display(Name = "Drawing tool")]
		[Range(1, int.MaxValue, ErrorMessage = "Please select a your drawing tool.")]
		public virtual DrawingTool DrawingTool { get; set; }

		[Required]
		[Display(Name = "Drawing device")]
		[Range(1, int.MaxValue, ErrorMessage = "Please select a your drawing device.")]
		public virtual DrawingDevice DrawingDevice { get; set; }

	}
}