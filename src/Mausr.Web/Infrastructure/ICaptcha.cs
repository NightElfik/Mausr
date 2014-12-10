using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Mausr.Web.Infrastructure {
	public interface ICaptcha {

		bool Validate(HttpContextBase context);

		HtmlString Html();

	}

	public class ReCaptcha : ICaptcha {

		private string publicKey;
		private string privateKey;

		public ReCaptcha(string publicKey, string privateKey) {
			this.publicKey = publicKey;
			this.privateKey = privateKey;
		}

		public bool Validate(HttpContextBase context) {
			string value = context.Request.Form["g-recaptcha-response"];
			string response = null;
			try {
				using (var client = new WebClient()) {
					string url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}&remoteip={2}",
						privateKey, value, context.Request.UserHostAddress);
					response = client.DownloadString(url);
				}
			}
			catch {
				return false;
			}

			var responseObject = JsonConvert.DeserializeObject<ReCaptchaApiResponse>(response);
			if (responseObject == null) {
				return false;
			}

			return responseObject.Success;
		}

		public HtmlString Html() {
			MyHtml.RequireScript("https://www.google.com/recaptcha/api.js");
			return new HtmlString(string.Format("<div class='g-recaptcha' data-sitekey='{0}'></div>", publicKey));

		}

		private class ReCaptchaApiResponse {

			[JsonProperty(PropertyName="success")]
			public bool Success { get; set; }

			[JsonProperty(PropertyName="error-codes")]
			public List<string> ErrorCodes { get; set; }

		}
	}
}