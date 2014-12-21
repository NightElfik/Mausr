using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mausr.Web.Entities;

namespace Mausr.Web.Areas.Administration.Models {
	public class SymbolDrawingPartialViewModel {

		public SymbolDrawing SymbolDrawing { get; set; }

		public int ImageSise { get; set; }

		public int PenSizePerc { get; set; }

	}
}