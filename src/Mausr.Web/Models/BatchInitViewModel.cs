using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class BatchInitViewModel {
		
		[Required]
		public int BatchNumber { get; set; }
		
		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "Symbol number is not valid.")]
		public int SymbolNumber { get; set; }


		public int? SavedDrawingId { get; set; }

	}
}