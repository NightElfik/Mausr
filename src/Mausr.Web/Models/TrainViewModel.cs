using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class TrainViewModel : TrainSettings {
		
		[Required]
		[MaxLength(32)]
		[Display(Name = "Hidden layers sizes")]
		public string HiddenLayersSizesStr { get; set; }

		
		public int OutputSize { get; set; }

		public int TrainingSamples { get; set; }

		public IEnumerable<SymbolDrawing> ExampleDrawings { get; set; }


	}
}