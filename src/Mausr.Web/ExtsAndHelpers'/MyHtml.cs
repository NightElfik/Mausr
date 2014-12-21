using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Mausr.Web.Entities;

namespace Mausr.Web {
	public static class MyHtml {

		private const string reqScriptKey = "RequiredScripts";

		/// <summary>
		/// Registers script path and automatically includes the script at the bottom of the loaded page.
		/// Redundant scripts with the same path are filtered and included only once.
		/// Order of scripts with the same order parameter is preserved.
		/// </summary>
		/// <param name="path">Local or global path to the script that will be put to "src" attribute of a script tag.</param>
		/// <param name="order">Order of all included scripts (the lower, the sooner included).</param>
		public static void RequireScript(string path, LoadingOrder order = LoadingOrder.Default) {
			var requiredScripts = HttpContext.Current.Items[reqScriptKey] as Dictionary<string, int>;
			if (requiredScripts == null) {
				HttpContext.Current.Items[reqScriptKey] = requiredScripts = new Dictionary<string, int>();
			}

			requiredScripts[path] = (int)order + requiredScripts.Count;
		}

		public static HtmlString EmitRequiredScripts() {
			var requiredScripts = HttpContext.Current.Items[reqScriptKey] as Dictionary<string, int>;
			if (requiredScripts == null) {
				return new HtmlString("");
			}

			var sb = new StringBuilder();
			foreach (var scriptPath in requiredScripts.OrderBy(x => x.Value).Select(x => x.Key)) {
				sb.Append("<script src='");
				sb.Append(scriptPath);
				sb.AppendLine("'></script>");
			}
			return new HtmlString(sb.ToString());
		}


		public static void SuccessMessage(string message, params object[] args) {
			addMessage(Message.Styles.Success, message, args);
		}

		public static void InformationMessage(string message, params object[] args) {
			addMessage(Message.Styles.Information, message, args);
		}

		public static void WarningMessage(string message, params object[] args) {
			addMessage(Message.Styles.Warning, message, args);
		}

		public static void DangerMessage(string message, params object[] args) {
			addMessage(Message.Styles.Danger, message, args);
		}

		public static HtmlString ShowMessages() {			
			var msgs = HttpContext.Current.Items[Message.DataKey] as List<Message>;
			if (msgs == null) {
				return null;
			}

			var sb = new StringBuilder();
			foreach (var msg in msgs) {
				sb.AppendFormat("<div class='alert alert-{0}'>{1}</div>", msg.Style, msg.Text);
			}

			return new HtmlString(sb.ToString());
		}

		public static HtmlString UserNameAsAbbr(ApplicationUser user) {
			if (user == null) {
				return null;
			}

			string fullName = user.UserName;
			int atPos = fullName.IndexOf('@');
			if (atPos < 0) {
				return new HtmlString(fullName);
			}

			return new HtmlString(string.Format("<abbr title='{0}'>{1}</abbr>", fullName, fullName.Substring(0, atPos)));
		}

		private static void addMessage(string style, string message, params object[] args) {
			var msgs = HttpContext.Current.Items[Message.DataKey] as List<Message>;
			if (msgs == null) {
				msgs = new List<Message>();
				HttpContext.Current.Items[Message.DataKey] = msgs;
			}

			msgs.Add(new Message {
				Style = style,
				Text = string.Format(message, args),
			});
		}

	}
	

	public class Message {
		public const string DataKey = "StatusMessageKey";

		public string Style { get; set; }
		public string Text { get; set; }


		public static class Styles {
			public const string Success = "success";
			public const string Information = "info";
			public const string Warning = "warning";
			public const string Danger = "danger";
		}
	}
	
	public enum LoadingOrder {

		VeryFirst = 0,
		Sooner = 100,
		Default = 200,
		Later = 300,

	}
}