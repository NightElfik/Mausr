using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Mausr.Web {
	public static class MyUrl {

		private static readonly Regex nonAlphanumRegex = new Regex("[^a-zA-Z0-9_]", RegexOptions.Compiled);
		private static readonly Regex multiDash = new Regex("[-]+", RegexOptions.Compiled);

		/// <summary>
		/// Converts given string to safe representation usable in URLs or IDs.
		/// </summary>
		/// <remarks>
		/// Converts all non-alphanumeric characters to dashes and all multi-dashes to one.
		/// Also, dashes are deleted from beginning and end.
		/// </remarks>
		public static string UrlizeString(string str) {
			Contract.Requires(string.IsNullOrWhiteSpace(str) == false);
			Contract.Ensures(Contract.Result<string>() != null && Contract.Result<string>().Length > 1);

			str = nonAlphanumRegex.Replace(str, "-");
			str = multiDash.Replace(str, "-");
			str = str.Trim('-');
			if (str.Length == 0) {
				return "-";
			}
			return str;
		}
	}
}