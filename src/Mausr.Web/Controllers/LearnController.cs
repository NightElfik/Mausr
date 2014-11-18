using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;
using Newtonsoft.Json;

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