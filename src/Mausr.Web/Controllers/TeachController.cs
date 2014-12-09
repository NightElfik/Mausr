using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Models;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	[Authorize(Roles = RolesHelper.Teacher)]
	public partial class TeachController : Controller {

		protected readonly MausrDb db;


		public TeachController(MausrDb db) {
			this.db = db;
		}


		public virtual ActionResult Index() {
			return View(new TeachInitViewModel() {
				Symbols = db.Symbols.ToList(),
			});
		}

		public virtual ActionResult StartTeachingAll() {
			var rand = new Random();
			var batchInitModel = new BatchInitViewModel() {
				BatchNumber = rand.Next(),
				SymbolNumber = 0,
			};
			return RedirectToAction(Actions.Batch().AddRouteValues(batchInitModel));
		}

		[HttpGet]
		public virtual ActionResult Batch(BatchInitViewModel model) {
			if (!ModelState.IsValid) {
				return RedirectToAction(Actions.Index());
			}

			var batchModel = new TeachBatchViewModel() {
				BatchNumber = model.BatchNumber,
				SymbolNumber = model.SymbolNumber,
				SymbolsCount = db.Symbols.Count(),
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
				batchModel.SavedDrawing = db.SymbolDrawings.FirstOrDefault(x => x.SymbolDrawingId == model.SavedDrawingId);
			}

			return View(batchModel);
		}

		[HttpPost]
		[ActionName("Batch")]
		public virtual ActionResult BatchPost(TeachBatchViewModel model) {
			if (ModelState.IsValid) {
				var symbol = getSymbol(model.BatchNumber, model.SymbolNumber);
				if (symbol != null) {
					var sd = insertSymbolDrawingFromJson(model.JsonData, symbol, model.DrawnUsingTouch);

					if (sd != null) {
						var initModel = new BatchInitViewModel() {
							BatchNumber = model.BatchNumber,
							SymbolNumber = model.SymbolNumber + 1,
							SavedDrawingId = sd.SymbolDrawingId,
						};
						return RedirectToAction(Batch().AddRouteValues(initModel));
					}
					else {
						ModelState.AddModelError("", "Invalid symbol drawing.");
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

			model.SymbolsCount = db.Symbols.Count();
			return View(model);
		}

		public virtual ActionResult Done() {
			return View();
		}

		private Symbol getSymbol(int batchNumber, int symbolNumber) {
			Contract.Requires(symbolNumber >= 0);

			return db.Symbols.OrderBy(s => (s.SymbolId * 113) ^ batchNumber).Skip(symbolNumber).FirstOrDefault();
		}
		
		private SymbolDrawing insertSymbolDrawingFromJson(string jsonData, Symbol symbol, bool drawnUsingTouch) {

			RawDrawing drawing;
			try {
				var lines = JsonConvert.DeserializeObject<RawPoint[][]>(jsonData);
				drawing = new RawDrawing() { Lines = lines };
			}
			catch (Exception ex) {
				return null;
			}

			new RawDataProcessor().CleanData(drawing);

			if (drawing.LinesCount == 0) {
				return null;
			}

			var sd = new SymbolDrawing() {
				Symbol = symbol,
				CreatedDateTime = DateTime.UtcNow,
				DrawnUsingTouch = drawnUsingTouch,
			};

			sd.SetRawDrawing(drawing);
			sd = db.SymbolDrawings.Add(sd);
			db.SaveChanges();

			return sd;
		}

	}
}