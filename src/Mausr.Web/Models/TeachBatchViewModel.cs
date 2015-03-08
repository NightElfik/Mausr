using System.ComponentModel.DataAnnotations;
using Mausr.Web.Entities;

namespace Mausr.Web.Models {
	public class TeachBatchViewModel : BatchInitViewModel {
						
		[Required]
		public string JsonData { get; set; }
			
		[Required]
		public bool DrawnUsingTouch { get; set; }

		
		public Symbol Symbol { get; set; }


		public SymbolDrawing SavedDrawing { get; set; }


	}
}