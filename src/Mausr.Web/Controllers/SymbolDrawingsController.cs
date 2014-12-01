using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
			var groups = symbolsDb.SymbolDrawings.GroupBy(x => x.Symbol).ToList();
			return View(groups);
		}


		public virtual ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			SymbolDrawing symbolDrawing = symbolsDb.SymbolDrawings.Find(id);
			if (symbolDrawing == null) {
				return HttpNotFound();
			}
			return View(symbolDrawing);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public virtual ActionResult DeleteConfirmed(int id) {
			SymbolDrawing symbolDrawing = symbolsDb.SymbolDrawings.Find(id);
			symbolsDb.SymbolDrawings.Remove(symbolDrawing);
			symbolsDb.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("SymbolDrawings/Img/{normalized:bool}/{decorated:bool}/{rotation:int:min(-360):max(360)}/{imageSize:int:min(8):max(1024)}/{penSizePerc:int:min(1):max(30)}/{id:int:min(0)}.png")]
		public virtual ActionResult Img(int id, int imageSize, int penSizePerc, int rotation, bool normalized, bool decorated) {
			var sd = symbolsDb.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == id);
			if (sd == null) {
				return HttpNotFound();
			}

			string fileName = string.Format("{0}-s{1}-p{2}-r{3}-d{4}-n{5}.png",
				id, imageSize, penSizePerc, rotation, decorated ? 1 : 0, normalized ? 1 : 0);
			string filePath = Path.Combine(appSettingsProvider.SymbolDrawingsCacheDirAbsolute, fileName);

			if (!System.IO.File.Exists(filePath)) {
				var drawing = sd.RawDrawing;
				
				if (rotation != 0) {
					drawing = new RawDataProcessor().Rotate(drawing, rotation, normalized);
				}
				else if (normalized) {
					new RawDataProcessor().Normalize(drawing);
				}

				var img = new Rasterizer().Rasterize(drawing, imageSize, penSizePerc / 100f, normalized, decorated);
				img.Save(filePath, ImageFormat.Png);
			}

			return File(filePath, "image/png");
		}
	}
}
