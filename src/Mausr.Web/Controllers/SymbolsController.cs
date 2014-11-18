using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class SymbolsController : Controller {
			
		protected readonly SymbolsDb symbolsDb;

		public SymbolsController(SymbolsDb symbolsDb) {			
			this.symbolsDb = symbolsDb;
		}


		public virtual ActionResult Index() {
			return View(symbolsDb.Symbols.ToList());
		}

		public virtual ActionResult Details(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = symbolsDb.Symbols.Find(id);
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
				symbolsDb.Symbols.Add(symbol);
				symbolsDb.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(symbol);
		}

		public virtual ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = symbolsDb.Symbols.Find(id);
			if (symbol == null) {
				return HttpNotFound();
			}
			return View(symbol);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Edit([Bind(Include = "SymbolId,SymbolStr,Name")] Symbol symbol) {
			if (ModelState.IsValid) {
				symbolsDb.Entry(symbol).State = EntityState.Modified;
				symbolsDb.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(symbol);
		}

		public virtual ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Symbol symbol = symbolsDb.Symbols.Find(id);
			if (symbol == null) {
				return HttpNotFound();
			}
			return View(symbol);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public virtual ActionResult DeleteConfirmed(int id) {
			Symbol symbol = symbolsDb.Symbols.Find(id);
			symbolsDb.Symbols.Remove(symbol);
			symbolsDb.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				symbolsDb.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
