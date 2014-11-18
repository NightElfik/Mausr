using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.DataContexts;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	public partial class SymbolDrawingsController : Controller {


		protected readonly SymbolsDb symbolsDb;
		protected readonly AppSettingsProvider appSettingsProvider;


		public SymbolDrawingsController(SymbolsDb symbolsDb, AppSettingsProvider appSettingsProvider) {
			this.symbolsDb = symbolsDb;
			this.appSettingsProvider = appSettingsProvider;
		}

		public virtual ActionResult Index() {
			return View();
		}

		[Route("SymbolDrawings/{imageSize:int:min(64):max(1024)}/{brushSize:int:min(1):max(32)}/{id:int:min(0)}.png")]
		public virtual ActionResult Img(int id, int imageSize, int brushSize) {
			var sd = symbolsDb.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == id);
			if (sd == null) {
				return HttpNotFound();
			}

			string fileName = string.Format("{0}-s{1}-b{2}.png", id, imageSize, brushSize);
			string filePath = Path.Combine(appSettingsProvider.SymbolDrawingsCacheDirAbsolute, fileName);

			if (!System.IO.File.Exists(filePath)) {
				var rasterizer = new Rasterizer();
				var img = rasterizer.Rasterize(sd.RawDrawing, imageSize, brushSize);
				img.Save(filePath, ImageFormat.Png);
			}

			return File(filePath, "image/png");
		}

		[HttpPost]
		public virtual ActionResult Save(int symbolId, string jsonData) {
			if (string.IsNullOrWhiteSpace(jsonData)) {
				return HttpNotFound();
			}

			var sym = symbolsDb.Symbols.FirstOrDefault(s => s.SymbolId == symbolId);
			if (sym == null) {
				return HttpNotFound();
			}

			var lines = JsonConvert.DeserializeObject<RawPoint[][]>(jsonData);
			var drawing = new RawDrawing() { Lines = lines };

			var sd = new SymbolDrawing() {
				Symbol = sym,
				CreatedDateTime = DateTime.UtcNow,
				RawDrawing = drawing,
			};

			symbolsDb.SymbolDrawings.Add(sd);
			symbolsDb.SaveChanges();

			return Content(sd.SymbolDrawingId.ToString());
		}
	}
}