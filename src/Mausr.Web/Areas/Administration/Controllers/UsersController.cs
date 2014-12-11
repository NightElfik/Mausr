using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mausr.Web.Areas.Administration.Models;
using Mausr.Web.Entities;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.Areas.Administration.Controllers {
	[Authorize(Roles = RolesHelper.Admin)]
	public partial class UsersController : Controller {
	
		protected readonly MausrDb db;

		public UsersController(MausrDb db) {
			this.db = db;
		}


		public virtual ActionResult Index(int? page) {
			return View(new UsersViewModel() {
				Users = db.Users.OrderBy(u => u.Id).ToPagination(page, 20, p => Url.Action(Actions.Index(p))),
				RoleNamesLookup = db.Roles.ToDictionary(r => r.Id, r => r.Name),
			});
		}

		public virtual ActionResult Details(string id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ApplicationUser applicationUser = db.Users.Find(id);
			if (applicationUser == null) {
				return HttpNotFound();
			}

			return View(new UserViewModel() {
				User = applicationUser,
				RoleNamesLookup = db.Roles.ToDictionary(r => r.Id, r => r.Name),
			});
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

		public virtual ActionResult AddRole(string id, string role) {
			if (id == null || role == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			userManager.AddToRole(id, role);

			return RedirectToAction(Actions.Details(id));
		}

		public virtual ActionResult RemoveRole(string id, string role) {
			if (id == null || role == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			userManager.RemoveFromRole(id, role);

			return RedirectToAction(Actions.Details(id));
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
