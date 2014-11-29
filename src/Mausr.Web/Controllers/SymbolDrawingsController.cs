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
		[Route("SymbolDrawings/{imageSize:int:min(64):max(1024)}/{penSize:int:min(1):max(32)}/{id:int:min(0)}.png")]
		public virtual ActionResult Img(int id, int imageSize, int penSize) {
			var sd = symbolsDb.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == id);
			if (sd == null) {
				return HttpNotFound();
			}

			string fileName = string.Format("{0}-s{1}-b{2}.png", id, imageSize, penSize);
			string filePath = Path.Combine(appSettingsProvider.SymbolDrawingsCacheDirAbsolute, fileName);

			if (!System.IO.File.Exists(filePath)) {
				var rasterizer = new Rasterizer();
				var img = rasterizer.Rasterize(sd.RawDrawing, imageSize, penSize);
				img.Save(filePath, ImageFormat.Png);
			}

			return File(filePath, "image/png");
		}
	}
}
