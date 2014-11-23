using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Mausr.Web.Models {
	public class PlotViewModel {
		
		public string Name { get; set; }

		public ActionResult ViewAction { get; set; }

		public PlotImageModel ImageModel { get; set; }

	}
}