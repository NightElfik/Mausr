using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mausr.Web {
	public static class MyHtml {
		
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

}