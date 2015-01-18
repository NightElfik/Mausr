using System;
using System.ComponentModel.DataAnnotations;

namespace Mausr.Web.Models {
	public class PredictModel {

		[Required]
		public Guid Guid { get; set; }

		[Required]
		public string JsonData { get; set; }

		[Required]
		public bool DrawnUsingTouch { get; set; }
		
		[Required]
		public bool IsFollowupDraw { get; set; }

	}
}