using System.Linq;
using System.Web.Mvc;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class LearnController : Controller {


		protected readonly SymbolsDb symbolsDb;


		public LearnController(SymbolsDb symbolsDb) {
			this.symbolsDb = symbolsDb;
		}


		public virtual ActionResult Index() {
			return View(new LearnViewModel() {
				Symbols = symbolsDb.Symbols.ToList()
			});
		}

	}
}