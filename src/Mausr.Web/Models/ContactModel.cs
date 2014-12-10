using System.ComponentModel.DataAnnotations;
using Mausr.Web.Infrastructure;

namespace Mausr.Web.Models {
	public class ContactModel {

		public ICaptcha Captcha { get; set; }

		[Required]
		[StringLength(64)]
		public string Name { get; set; }

		[StringLength(64)]
		[Display(Name = "E-mail (optional)")]
		public string Email { get; set; }

		[Required]
		[StringLength(64)]
		public string Subject { get; set; }

		[Required]
		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string Message { get; set; }
	}
}