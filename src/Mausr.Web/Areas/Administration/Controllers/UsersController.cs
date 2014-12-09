using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;

namespace Mausr.Web.Areas.Administration.Controllers {
	[Authorize(Roles = RolesHelper.Admin)]
	public partial class UsersController : Controller {
	
		protected readonly MausrDb db;

		public UsersController(MausrDb db) {
			this.db = db;
		}


		public virtual ActionResult Index() {
			return View(db.Users.Include(u => u.Roles).ToList());
		}

		public virtual ActionResult Details(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser applicationUser = db.Users.Find(id);
			if (applicationUser == null) {
				return HttpNotFound();
			}
			return View(applicationUser);
		}


		public virtual ActionResult Edit(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser applicationUser = db.Users.Find(id);
			if (applicationUser == null) {
				return HttpNotFound();
			}
			return View(applicationUser);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser) {
			if (ModelState.IsValid) {
				db.Entry(applicationUser).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(applicationUser);
		}

		public virtual ActionResult Delete(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser applicationUser = db.Users.Find(id);
			if (applicationUser == null) {
				return HttpNotFound();
			}
			return View(applicationUser);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public virtual ActionResult DeleteConfirmed(string id) {
			ApplicationUser applicationUser = db.Users.Find(id);
			db.Users.Remove(applicationUser);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

	}
}
