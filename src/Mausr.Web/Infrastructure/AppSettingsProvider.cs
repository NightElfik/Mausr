using System.Configuration;
using System.Web;

namespace Mausr.Web.Infrastructure {
	public class AppSettingsProvider {

		public const string SymbolDrawingsCacheDirKey = "SymbolDrawingsCacheDir";


		private string p_symbolDrawingsCacheDirVirtual = null;
		public string SymbolDrawingsCacheDirVirtual {
			get {
				if (p_symbolDrawingsCacheDirVirtual == null) {
					p_symbolDrawingsCacheDirVirtual = ConfigurationManager.AppSettings[SymbolDrawingsCacheDirKey];
				}
				return p_symbolDrawingsCacheDirVirtual;
			}
		}

		private string p_symbolDrawingsCacheDirAbsolute = null;
		public string SymbolDrawingsCacheDirAbsolute {
			get {
				if (p_symbolDrawingsCacheDirAbsolute == null) {
					p_symbolDrawingsCacheDirAbsolute = HttpContext.Current.Server.MapPath(SymbolDrawingsCacheDirVirtual);
				}
				return p_symbolDrawingsCacheDirAbsolute;
			}
		}

	}
}