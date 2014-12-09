using System.Collections.Generic;
using Mausr.Web.Entities;

namespace Mausr.Web.Models {
	public class SymbolDrawingsViewModel {

		public IEnumerable<Symbol> Symbols { get; set; }

		public IEnumerable<SymbolDrawing> Drawings { get; set; }

	}
}