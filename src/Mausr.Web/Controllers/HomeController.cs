using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Mausr.Web.NeuralNet;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	public partial class HomeController : Controller {

		const int GUID_CHECK_WINDOW_SECONDS = 60;

		protected readonly MausrDb db;
		protected readonly CurrentEvaluator evaluator;

		public HomeController(MausrDb db, CurrentEvaluator evaluator) {
			this.db = db;
			this.evaluator = evaluator;
		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult About() {
			return View();
		}

		public virtual ActionResult Contact() {
			return View();
		}

		[HttpPost]
		public virtual ActionResult Predict(PredictModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var sw = new Stopwatch();
			sw.Start();
			RawDrawing rawDrawing;
			try {
				var lines = JsonConvert.DeserializeObject<RawPoint[][]>(model.JsonData);
				rawDrawing = new RawDrawing() { Lines = lines };
			}
			catch (Exception ex) {
				return HttpNotFound();
			}

			var predictions = evaluator.PredictTopN(rawDrawing, 8, 0.05);
			var rawResults = predictions.Join(db.Symbols, p => p.OutputId, s => s.SymbolId, (p, s) => new {
				Symbol = s,
				Rating = (float)p.NeuronOutputValue,
			}).ToList();

			var minTime = DateTime.UtcNow.AddSeconds(-GUID_CHECK_WINDOW_SECONDS);

			var drawing = db.Drawings
				.Where(d => d.ClientGuid == model.Guid && DateTime.Compare(d.DrawnDateTime, minTime) > 0)
				.FirstOrDefault();

			if (drawing == null) {
				drawing = new Drawing();
				drawing.DrawnDateTime = DateTime.UtcNow;
				db.Drawings.Add(drawing);
			}
			else {
				// Delete potentially cached image - this does not happen ofthen but it is annoying when it does happen.
				new DrawingsController(db, DependencyResolver.Current.GetService<AppSettingsProvider>())
					.ClearCachedImage(drawing.DrawingId);
			}

			var firstResult = rawResults.FirstOrDefault();

			// TODO: fix problems with image cache.
			drawing.ClientGuid = model.Guid;
			drawing.TopSymbol = firstResult == null ? null : firstResult.Symbol;
			drawing.TopSymbolScore = firstResult == null ? null : (double?)firstResult.Rating;
			drawing.DrawnUsingTouch = model.DrawnUsingTouch;
			drawing.SetRawDrawing(rawDrawing);

			db.SaveChanges();
			sw.Stop();

			return Json(new {
				Results = rawResults.Select(x => new {
					SymbolId = x.Symbol.SymbolId,
					Symbol = x.Symbol.SymbolStr,
					SymbolName = x.Symbol.Name,
					Rating = x.Rating,
					HtmlEntity = x.Symbol.HtmlEntity ?? "",
					UtfCode = char.ConvertToUtf32(x.Symbol.SymbolStr, 0),
				}),
				Duration = (float)sw.Elapsed.TotalMilliseconds,
			});
		}
	}
}