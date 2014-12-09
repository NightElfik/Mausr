using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;

namespace Mausr.Web.Areas.Administration.Controllers {
	[Authorize(Roles = RolesHelper.Admin)]
	public partial class SymbolDrawingsController : Controller {

		protected readonly MausrDb db;
		protected readonly AppSettingsProvider appSettingsProvider;


		public SymbolDrawingsController(MausrDb db, AppSettingsProvider appSettingsProvider) {
			this.db = db;
			this.appSettingsProvider = appSettingsProvider;
		}


		public virtual ActionResult Index(int? id = null) {
			return View(new SymbolDrawingsViewModel() {
				Symbols = db.Symbols.ToList(),
				Drawings = id == null
					? new List<SymbolDrawing>()
					: db.SymbolDrawings.Where(d => d.Symbol.SymbolId == id.Value).ToList()
			});
		}


		public virtual ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			SymbolDrawing symbolDrawing = db.SymbolDrawings.Find(id);
			if (symbolDrawing == null) {
				return HttpNotFound();
			}
			return View(symbolDrawing);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public virtual ActionResult DeleteConfirmed(int id) {
			SymbolDrawing symbolDrawing = db.SymbolDrawings.Find(id);
			db.SymbolDrawings.Remove(symbolDrawing);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
