using System.Linq;
using System.Web.Mvc;
using Mausr.Web.Entities;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class SymbolsController : Controller {

		protected readonly MausrDb db;

		public SymbolsController(MausrDb db) {			
			this.db = db;
		}


		public virtual ActionResult Index(int? page) {
			return View(db.Symbols
				.OrderBy(x => x.SymbolStr)
				.ToPagination(page, 20, p => Url.Action(Actions.Index(p)))
			);
		}
	}
}