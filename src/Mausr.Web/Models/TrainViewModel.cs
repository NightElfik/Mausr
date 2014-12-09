using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mausr.Web.Entities;
using Mausr.Web.NeuralNet;

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