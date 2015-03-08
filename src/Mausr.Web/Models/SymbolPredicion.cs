using Mausr.Web.Entities;

namespace Mausr.Web.Models {
	public class SymbolPredicion {

		public Symbol Symbol { get; set; }

		public float OutValue { get; set; }


		public SymbolPredicion(Symbol symbol, float outVal) {
			Symbol = symbol;
			OutValue = outVal;
		}

	}
}