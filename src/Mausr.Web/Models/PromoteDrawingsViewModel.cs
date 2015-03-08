using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mausr.Web.Entities;

namespace Mausr.Web.Models {
	public class PromoteDrawingsViewModel {

		public IList<Drawing> Drawings { get; set; }
		public IList<SymbolPredicion> Predictions { get; set; }
		
		public int SavedRows { get; set; }
		public int SkippedRows { get; set; }

	}
}