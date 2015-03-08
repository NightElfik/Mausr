using System.Collections.Generic;
using Mausr.Core.NeuralNet;
using Mausr.Web.Entities;

namespace Mausr.Web.Models {
	public class ApproveSymbolDrawingsModel {

		public IList<SymbolDrawing> UnapprovedSymbolDrawings { get; set; }
		public IList<SymbolPredicion> Predictions { get; set; }
		
		public int SavedRows { get; set; }
		public int SkippedRows { get; set; }

	}
}