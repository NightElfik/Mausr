using System;
using System.Diagnostics.Contracts;
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


		[HttpGet]
		public virtual ActionResult Index() {
			return View(new LearnInitViewModel());
		}

		[HttpPost]
		public virtual ActionResult Index(LearnInitViewModel model) {
			if (ModelState.IsValid) {
				var rand = new Random();
				var batchInitModel = new BatchInitViewModel() {
					DrawingDevice = model.DrawingDevice,
					DrawingTool = model.DrawingTool,
					BatchNumber = rand.Next(),
					SymbolNumber = 0,
				};
				return RedirectToAction(Actions.Batch().AddRouteValues(batchInitModel));
			}

			return View(model);
		}

		[HttpGet]
		public virtual ActionResult Batch(BatchInitViewModel model) {
			if (!ModelState.IsValid) {
				return RedirectToAction(Index());
			}

			var batchModel = new LearnBatchViewModel() {
				DrawingDevice = model.DrawingDevice,
				DrawingTool = model.DrawingTool,
				BatchNumber = model.BatchNumber,
				SymbolNumber = model.SymbolNumber,
				SymbolsCount = symbolsDb.Symbols.Count(),
			};

			if (batchModel.SymbolNumber >= batchModel.SymbolsCount) {
				return RedirectToAction(Actions.Done());
			}

			batchModel.Symbol = getSymbol(model.BatchNumber, model.SymbolNumber);
			if (batchModel.Symbol == null) {
				return HttpNotFound();
			}

			if (model.SavedDrawingId != null) {
				batchModel.SavedDrawingId = model.SavedDrawingId;
				batchModel.SavedDrawing = symbolsDb.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == model.SavedDrawingId);
			}

			return View(batchModel);
		}

		[HttpPost]
		[ActionName("Batch")]
		public virtual ActionResult BatchPost(LearnBatchViewModel model) {
			if (ModelState.IsValid) {
				var symbol = getSymbol(model.BatchNumber, model.SymbolNumber);
				if (symbol != null) {
					var sd = symbolsDb.InsertSymbolDrawingFromJson(model.JsonData, symbol,
						model.DrawingDevice, model.DrawingTool);

					if (sd != null) {
						var initModel = new BatchInitViewModel() {
							DrawingDevice = model.DrawingDevice,
							DrawingTool = model.DrawingTool,
							BatchNumber = model.BatchNumber,
							SymbolNumber = model.SymbolNumber + 1,
							SavedDrawingId = sd.SymbolDrawingId,
						};
						return RedirectToAction(Batch().AddRouteValues(initModel));
					}
					else {
						ModelState.AddModelError("", "Failed to parse or insert symbol drawing to the DB.");
					}
				}
				else {
					ModelState.AddModelError("", "Invalid symbol.");
				}
			}
			else {
				ModelState.AddModelError("", "Model is not valid.");
			}

			model.Symbol = getSymbol(model.BatchNumber, model.SymbolNumber);
			if (model.Symbol == null) {
				return HttpNotFound();
			}

			model.SymbolsCount = symbolsDb.Symbols.Count();
			return View(model);
		}

		public virtual ActionResult Done() {
			return View();
		}

		private Symbol getSymbol(int batchNumber, int symbolNumber) {
			Contract.Requires(symbolNumber >= 0);

			return symbolsDb.Symbols.OrderBy(s => (s.SymbolId * 113) ^ batchNumber).Skip(symbolNumber).FirstOrDefault();
		}


	}
}