using System.Web;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Mausr.Web.Infrastructure {
	public static class Logger {

		public static void LogInfo<T>(string message, params object[] args) {
			var context = HttpContext.Current;
			if (context != null && HttpContext.Current.Handler != null) {
				ThreadContext.Properties["Url"] = context.Request.Url.AbsoluteUri;
				ThreadContext.Properties["HttpReferer"] = context.Request.ServerVariables["HTTP_REFERER"];
			}

			try {
				LogManager.GetLogger(typeof(T)).InfoFormat(message, args);
			}
			catch {				
				// TODO: Log me maybe.
			}
		}

	}
}