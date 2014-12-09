using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class TeachBatchViewModel : BatchInitViewModel {
						
		[Required]
		public string JsonData { get; set; }
			
		[Required]
		public bool DrawnUsingTouch { get; set; }


		public int SymbolsCount { get; set; }

		public Symbol Symbol { get; set; }


		public SymbolDrawing SavedDrawing { get; set; }


	}
}