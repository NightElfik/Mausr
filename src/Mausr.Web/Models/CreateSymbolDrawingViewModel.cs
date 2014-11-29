using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Mausr.Web.Models {
	[Bind(Exclude="Symbols")]
	public class CreateSymbolDrawingViewModel {

		public IEnumerable<Symbol> Symbols { get; set; }

		[Required]
		[Display(Name="Symbol")]
		public int SymbolId { get; set; }
		
		[Required]
		[Display(Name="Data")]
		public string JsonData { get; set; }

	}
}