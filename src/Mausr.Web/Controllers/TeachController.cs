using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Mausr.Web.NeuralNet;
using Mausr.Core.NeuralNet;
using System.Collections.Generic;

namespace Mausr.Web.Controllers {
	public partial class TeachController : Controller {

		protected readonly MausrDb db;
		protected readonly CurrentEvaluator evaluator;


		public TeachController(MausrDb db, CurrentEvaluator evaluator) {
			this.db = db;
			this.evaluator = evaluator;
		}


		public virtual ActionResult Index() {
			return View(new TeachInitViewModel() {
				Symbols = db.Symbols.ToList(),
			});
		}

		/// <remarks>
		/// This action is post to avoid indexing of infinite amounts of random batches.
		/// </remarks>
		[HttpPost]
		public virtual ActionResult StartTeaching(int count) {
			Logger.LogInfo<TeachController>("Started teaching all.");
			var rand = new Random();
			var batchInitModel = new BatchInitViewModel() {
				BatchNumber = rand.Next(),
				SymbolNumber = 0,
				SymbolsCount = Math.Min(Math.Max(1, count), db.Symbols.Count()),
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
				SymbolsCount = model.SymbolsCount,
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
							SymbolsCount = model.SymbolsCount,
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
				ModelState.AddModelError("", "Submitted data are not valid.");
			}

			model.Symbol = getSymbol(model.BatchNumber, model.SymbolNumber);
			if (model.Symbol == null) {
				return HttpNotFound();
			}

			model.SymbolsCount = db.Symbols.Count();
			return View(model);
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
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			ApplicationUser author = User.Identity.IsAuthenticated ? userManager.FindById(User.Identity.GetUserId()) : null;
			var sd = new SymbolDrawing() {
				Symbol = symbol,
				CreatedDateTime = DateTime.UtcNow,
				DrawnUsingTouch = drawnUsingTouch,
				Creator = author,
				Approved = (author == null || userManager.IsInRole(author.Id, RolesHelper.Teacher) == false) ? null : (bool?)true
			};

			sd.SetRawDrawing(drawing);
			sd = db.SymbolDrawings.Add(sd);
			db.SaveChanges();

			Logger.LogInfo<TeachController>("New symbol drawing [{0}].", sd.SymbolDrawingId);

			return sd;
		}


		public virtual ActionResult Done() {
			int drawingsCount = -1;
			if (User.Identity.IsAuthenticated) {
				var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
				var user = userManager.FindByName(User.Identity.Name);
				drawingsCount = user.SymbolDrawings.Count();
			}

			return View(new DoneViewModel() {
				DrawingsCount = drawingsCount
			});
		}

		[HttpGet]
		[Authorize(Roles = RolesHelper.Teacher)]
		public virtual ActionResult ApproveSymbolDrawings(int? savedRows = null, int? skippedRows = null) {
			var unapproved = db.SymbolDrawings
				.Include(x => x.Symbol)
				.Where(x => x.Approved == null)
				.OrderBy(x => x.CreatedDateTime)
				.Take(20)
				.ToList();

			Prediction[] predictions = evaluator.Predict(unapproved.Select(x => x.GetRawDrawing()));
			var symbolPredictions = predictions.Join(db.Symbols, p => p.OutputId, s => s.SymbolId, (p, s) => new SymbolPredicion(s, p.NeuronOutputValue))
				.ToList();

			return View(new ApproveSymbolDrawingsModel() {
				UnapprovedSymbolDrawings = unapproved,
				Predictions = symbolPredictions,
				SavedRows = savedRows ?? 0,
				SkippedRows = skippedRows ?? 0,
			});
		}

		[HttpPost]
		[Authorize(Roles = RolesHelper.Teacher)]
		[ActionName("ApproveSymbolDrawings")]
		public virtual ActionResult ApproveSymbolDrawingsPost(IDictionary<string, string> approvals) {
			int savedRows = 0;
			int skippedRows = 0;

			foreach (var kvp in approvals) {
				int symbolDrawingId;
				if (int.TryParse(kvp.Key, out symbolDrawingId) == false) {
					skippedRows += 1;
					continue;
				}

				bool approved;
				if (bool.TryParse(kvp.Value, out approved) == false) {
					skippedRows += 1;
					continue;
				}

				var symbol = db.SymbolDrawings
					//.Include(x => x.Symbol)
					.Where(x => x.Approved == null && x.SymbolDrawingId == symbolDrawingId)
					.FirstOrDefault();

				if (symbol == null) {
					skippedRows += 1;
					continue;
				}

				symbol.Approved = approved;
				savedRows += 1;
			}
			
			db.SaveChangesNotValidated();
			return RedirectToAction(Actions.ApproveSymbolDrawings(savedRows, skippedRows));
		}

	}
}