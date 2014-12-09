using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	[Authorize(Roles = RolesHelper.Teacher)]
	public partial class DrawingsController : Controller {

		const int pageSize = 20;
		
		protected readonly MausrDb db;
		protected readonly AppSettingsProvider appSettingsProvider;


		public DrawingsController(MausrDb db, AppSettingsProvider appSettingsProvider) {
			this.db = db;
			this.appSettingsProvider = appSettingsProvider;
		}


		public virtual ActionResult Index(int? page) {
			var drawings = db.Drawings.OrderByDescending(d => d.DrawnDateTime)
				.ToPagination(page, pageSize, p => Url.Action(Actions.Index(p)));
			return View(drawings);
		}

		[HttpGet]
		[Route("SymbolDrawings/Img/{normalized:bool}/{decorated:bool}/{imageSize:int:min(8):max(1024)}/{penSizePerc:int:min(1):max(30)}/{id:int:min(0)}.png")]
		public virtual ActionResult Img(int id, int imageSize, int penSizePerc, bool normalized, bool decorated) {
			var sd = db.Drawings.FirstOrDefault(x => x.DrawingId == id);
			if (sd == null) {
				return HttpNotFound();
			}

			string fileName = string.Format("{0}-s{1}-p{2}-d{3}-n{4}.png",
				id, imageSize, penSizePerc, decorated ? 1 : 0, normalized ? 1 : 0);
			string filePath = Path.Combine(appSettingsProvider.DrawingsCacheDirAbsolute, fileName);

			if (!System.IO.File.Exists(filePath)) {
				var drawing = sd.GetRawDrawing();

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