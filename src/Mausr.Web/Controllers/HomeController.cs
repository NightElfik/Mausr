using System;
using System.Linq;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;
using Mausr.Web.NeuralNet;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	public partial class HomeController : Controller {
		
		protected readonly MausrDb db;
		protected readonly CurrentEvaluator evaluator;

		public HomeController(MausrDb db, CurrentEvaluator evaluator) {
			this.db = db;
			this.evaluator = evaluator;
		}

		public virtual ActionResult Index() {
			return View();
		}

		//public virtual ActionResult About() {
		//	ViewBag.Message = "Your application description page.";

		//	return View();
		//}

		//public virtual ActionResult Contact() {
		//	ViewBag.Message = "Your contact page.";

		//	return View();
		//}

		[HttpPost]
		public virtual ActionResult Predict(PredictModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}
			
			RawDrawing drawing;
			try {
				var lines = JsonConvert.DeserializeObject<RawPoint[][]>(model.JsonData);
				drawing = new RawDrawing() { Lines = lines };
			}
			catch (Exception ex) {
				return HttpNotFound();
			}

			var predictions = evaluator.PredictTopN(drawing, 10, 0.05);
			var results = predictions.Join(db.Symbols, p => p.OutputId, s => s.SymbolId, (p, s) => new {
				SymbolId = p.OutputId,
				Symbol = s.SymbolStr,
				SymbolName = s.Name,
				Rating = (float)p.NeuronOutputValue,
			});
			return Json(results);
		}
	}
}