using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Web.Entities;
using Mausr.Web.Models;

namespace Mausr.Web.Areas.Administration.Controllers {
	[Authorize(Roles = RolesHelper.Admin)]
	public partial class SymbolsController : Controller {
			
		protected readonly MausrDb db;

		public SymbolsController(MausrDb db) {			
			this.db = db;
		}


		public virtual ActionResult Index() {
			return View(db.Symbols.ToList());
		}

		public virtual ActionResult Collisions() {
			return View(db.Symbols.GroupBy(s => s.SymbolStr).Where(g => g.Count() > 1).ToList());
		}

		public virtual ActionResult Details(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = db.Symbols.Find(id);
			if (symbol == null) {
				return HttpNotFound();
			}
			return View(symbol);
		}

		public virtual ActionResult Create() {
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Create([Bind(Include = "SymbolStr,Name")] Symbol symbol) {
			if (ModelState.IsValid) {
				db.Symbols.Add(symbol);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(symbol);
		}

		public virtual ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = db.Symbols.Find(id);
			if (symbol == null) {
				return HttpNotFound();
			}
			return View(symbol);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Edit([Bind(Include = "SymbolId,SymbolStr,Name")] Symbol symbol) {
			if (ModelState.IsValid) {
				db.Entry(symbol).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(symbol);
		}

		public virtual ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = db.Symbols.Find(id);
			if (symbol == null) {
				return HttpNotFound();
			}
			return View(symbol);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public virtual ActionResult DeleteConfirmed(int id) {
			Symbol symbol = db.Symbols.Find(id);
			db.Symbols.Remove(symbol);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

	}
}
