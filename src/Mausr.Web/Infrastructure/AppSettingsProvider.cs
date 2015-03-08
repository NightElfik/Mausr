using System.Configuration;
using System.Web;

namespace Mausr.Web.Infrastructure {
	public class AppSettingsProvider {

		public const string RecaptchaPublicKey = "RecaptchaPublicKey";
		public const string RecaptchaPrivateKey = "RecaptchaPrivateKey";
		public const string SymbolDrawingsCacheDirKey = "SymbolDrawingsCacheDir";
		public const string DrawingsCacheDirKey = "DrawingsCacheDir";
		public const string NetTrainDataDirKey = "NetTrainDataDir";
		public const string PrivateDirKey = "PrivateDir";

		
		private string p_recaptchaPublic = null;
		public string RecaptchaPublic {
			get {
				if (p_recaptchaPublic == null) {
					p_recaptchaPublic = ConfigurationManager.AppSettings[RecaptchaPublicKey];
				}
				return p_recaptchaPublic;
			}
		}

		private string p_recaptchaPrivate = null;
		public string RecaptchaPrivate {
			get {
				if (p_recaptchaPrivate == null) {
					p_recaptchaPrivate = ConfigurationManager.AppSettings[RecaptchaPrivateKey];
				}
				return p_recaptchaPrivate;
			}
		}
		

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

		private string p_drawingsCacheDirVirtual = null;
		public string DrawingsCacheDirVirtual {
			get {
				if (p_drawingsCacheDirVirtual == null) {
					p_drawingsCacheDirVirtual = ConfigurationManager.AppSettings[DrawingsCacheDirKey];
				}
				return p_drawingsCacheDirVirtual;
			}
		}

		private string p_drawingsCacheDirAbsolute = null;
		public string DrawingsCacheDirAbsolute {
			get {
				if (p_drawingsCacheDirAbsolute == null) {
					p_drawingsCacheDirAbsolute = HttpContext.Current.Server.MapPath(DrawingsCacheDirVirtual);
				}
				return p_drawingsCacheDirAbsolute;
			}
		}

		private string p_netTrainDataDirVirtual = null;
		public string NetTrainDataDirVirtual {
			get {
				if (p_netTrainDataDirVirtual == null) {
					p_netTrainDataDirVirtual = ConfigurationManager.AppSettings[NetTrainDataDirKey];
				}
				return p_netTrainDataDirVirtual;
			}
		}

		private string p_netTrainDataDirAbsolute = null;
		public string NetTrainDataDirAbsolute {
			get {
				if (p_netTrainDataDirAbsolute == null) {
					p_netTrainDataDirAbsolute = HttpContext.Current.Server.MapPath(NetTrainDataDirVirtual);
				}
				return p_netTrainDataDirAbsolute;
			}
		}
		
		private string p_privateDir = null;
		public string PrivateDir {
			get {
				if (p_privateDir == null) {
					p_privateDir = ConfigurationManager.AppSettings[PrivateDirKey];
				}
				return p_privateDir;
			}
		}

	}
}