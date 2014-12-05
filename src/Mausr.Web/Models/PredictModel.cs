using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class PredictModel {

		[Required]
		public string JsonData { get; set; }

		[Required]
		public bool DrawnUsingTouch { get; set; }

	}
}