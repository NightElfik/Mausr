using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

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

		public virtual ActionResult DoublePosts() {
			return View(getDoublePosts());
		}

		public virtual ActionResult ClearDoublePosts() {
			var doublePosts = getDoublePosts();
			int deleted = 0;
			foreach (var dpGroup in doublePosts) {
				foreach (var drawing in dpGroup.Skip(1)) {
					db.SymbolDrawings.Remove(drawing);
					deleted += 1;
				}
			}

			int affectedRows = db.SaveChanges();

			return RedirectToAction(Actions.DoublePosts());
		}

		private List<List<SymbolDrawing>> getDoublePosts() {
			return db.SymbolDrawings
				.GroupBy(d => d.RawData)
				.Where(g => g.Count() > 1)
				.Select(g => g.ToList())
				.ToList();
		}


		public virtual ActionResult ApprovalProblems() {
			return View(getApprovalProblems());
		}

		public virtual ActionResult FixApprovalProblems() {
			List<SymbolDrawing> problems = getApprovalProblems();

			foreach (var sd in problems) {
				sd.Approved = true;
			}

			db.SaveChangesNotValidated();

			return RedirectToAction(Actions.ApprovalProblems());
		}

		private List<SymbolDrawing> getApprovalProblems() {
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

			var userInRoleCache = new Dictionary<string, bool>();

			return db.SymbolDrawings
				.Where(d => d.Approved == null && d.Creator != null)
				.ToList()
				.Where(d => {
					bool isInRole;
					if (!userInRoleCache.TryGetValue(d.Creator.Id, out isInRole)) {
						isInRole = userManager.IsInRole(d.Creator.Id, RolesHelper.Teacher);
						userInRoleCache[d.Creator.Id] = isInRole;
					}
					return isInRole;
				})
				.ToList();
		}

	}
}
