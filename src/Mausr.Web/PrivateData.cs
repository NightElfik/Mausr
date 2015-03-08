using System.Collections.Generic;
using System.IO;

namespace Mausr.Web {
	public static class PrivateData {

		private static Dictionary<string, string> data = new Dictionary<string, string>();
				
		
		public static string GoogleAnalyticsKey { get { return Get("GoogleAnalyticsKey"); } }

		public static string ReCaptchaPrivate { get { return Get("ReCaptchaPrivate"); } }

		public static string ReCaptchaPublic { get { return Get("ReCaptchaPublic"); } }


		public static string Get(string key) {
			string value;
			if (data.TryGetValue(key, out value)) {
				return value;
			}
			else {
				return "";
			}
		}

		internal static void LoadFormDir(string absDirPath) {
			foreach (var filePath in Directory.EnumerateFiles(absDirPath)) {
				string key = Path.GetFileNameWithoutExtension(filePath);
				string value = File.ReadAllText(filePath).Trim();
				data[key] = value;
			}
		}

	}
}