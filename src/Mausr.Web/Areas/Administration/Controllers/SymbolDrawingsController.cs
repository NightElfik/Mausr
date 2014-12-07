using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.DataContexts;
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


		public virtual ActionResult Index(int? page = null) {
			var query = db.SymbolDrawings
				.GroupBy(x => x.Symbol)
				.OrderBy(x => x.Key.SymbolId);
			var keys = query.Select(x => x.Key.SymbolStr).ToList();
			var items = query.ToPagination(page, 1, i => Url.Action(Actions.Index(i)), 4, i => keys[i - 1]);
			return View(items);
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
