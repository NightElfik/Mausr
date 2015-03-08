using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class SymbolDrawingsController : Controller {

		protected readonly MausrDb db;
		protected readonly AppSettingsProvider appSettingsProvider;


		public SymbolDrawingsController(MausrDb db, AppSettingsProvider appSettingsProvider) {
			this.db = db;
			this.appSettingsProvider = appSettingsProvider;
		}


		public virtual ActionResult Index(int? id = null) {
			return View(new SymbolDrawingsViewModel() {
				CurrentSymbol = db.Symbols.FirstOrDefault(s => s.SymbolId == id),
				Symbols = db.Symbols.OrderBy(d => d.SymbolStr).ToList(),
				Drawings = id == null
					? new List<SymbolDrawing>()
					: db.SymbolDrawings
						.Where(d => d.Approved == true && d.Symbol.SymbolId == id.Value)
						.OrderByDescending(sd => sd.CreatedDateTime)
						.Take(8 * 3)
						.ToList()
			});
		}


		[HttpGet]
		[Route("SymbolDrawings/Img/{normalized:bool}/{decorated:bool}/{rotation:int:min(-360):max(360)}/{imageSize:int:min(8):max(1024)}/{penSizePerc:int:min(1):max(30)}/{id:int:min(0)}.png")]
		[OutputCache(CacheProfile = CacheProfileKeys.LongClientCache)]
		public virtual ActionResult Img(int id, int imageSize, int penSizePerc, int rotation, bool normalized, bool decorated) {
			var sd = db.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == id);
			if (sd == null) {
				return HttpNotFound();
			}

			string fileName = string.Format("{0}-s{1}-p{2}-r{3}-d{4}-n{5}.png",
				id, imageSize, penSizePerc, rotation, decorated ? 1 : 0, normalized ? 1 : 0);
			string filePath = Path.Combine(appSettingsProvider.SymbolDrawingsCacheDirAbsolute, fileName);

			if (!System.IO.File.Exists(filePath)) {
				var drawing = sd.GetRawDrawing();

				if (rotation != 0) {
					new RawDataProcessor().RotateInPlace(drawing, rotation);
				}
				if (normalized) {
					new RawDataProcessor().NormalizeInPlace(drawing);
				}

				var img = new Rasterizer().Rasterize(drawing, imageSize, penSizePerc / 100f, normalized, decorated);
				img.Save(filePath, ImageFormat.Png);
			}

			return File(filePath, "image/png");
		}
	}
}