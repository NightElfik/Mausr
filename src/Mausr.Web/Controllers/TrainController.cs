﻿using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.DataContexts;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class TrainController : Controller {

		protected readonly MausrDb db;
		protected readonly TrainStorageManager trainStorageManager;


		public TrainController(MausrDb db, TrainStorageManager trainStorageManager) {
			this.db = db;
			this.trainStorageManager = trainStorageManager;
		}

		public virtual ActionResult Index() {
			var model = new TrainViewModel() {
				InputImgSizePx = 20,
				PenThicknessPerc = 8,
				GenerateExtraInputsByRotation = 10,
				NormalizeInput = true,
				LearnRounds = 2,
				BatchSize = 128,
				MaxIteratinosPerBatch = 256,
				RegularizationLambda = 0.1,
				LearningRate = 0.1,
				MomentumStartPerc = 60,
				MomentumEndPerc = 99,
				MinDerivativeMagnitude = 1e-4,
				TestDataSetSizePerc = 10,
			};

			initModel(model);

			return View(model);
		}

		[HttpPost]
		public virtual ActionResult Index(TrainViewModel model) {
			if (ModelState.IsValid) {
				model.NetId = trainStorageManager.CreateSafeNetName(model.NetName);

				model.HiddenLayersSizes = parseInts(model.HiddenLayersSizesStr);
				bool hlValid = model.HiddenLayersSizes != null
					&& model.HiddenLayersSizes.All(s => s > 0 && s < 65536);
				if (hlValid) {
					if (trainStorageManager.SaveTrainSettings(model.NetId, model.ShallowCloneAs<TrainSettings>())) {
						startTrainig(model.NetId);
						return RedirectToAction(Actions.Details(model.NetId));
					}
					else {
						ModelState.AddModelError("", "Failed to save train settings.");
					}
				}
				else {
					ModelState.AddModelError("HiddenLayersSizesStr", "Invalid hidden layer sizes.");
				}
			}

			initModel(model);
			return View(model);
		}

		public virtual ActionResult Details(string id) {
			if (id == null) {
				return HttpNotFound();
			}

			var trainSettings = trainStorageManager.LoadTrainSettings(id);

			if (trainSettings == null) {
				return HttpNotFound();
			}
			
			var model = new TrainDetailsViewModel() {
				TrainSettings = trainSettings,
			};

			return View(model);
		}

		private Job startTrainig(string id) {			
			var job = JobManager.Instance.TryStartJobAsync(id, j => {
				for (var progress = 0; progress <= 100; progress++) {
					if (j.CancellationToken.IsCancellationRequested) {
						return;
					}

					Thread.Sleep(200);
					j.ReportProgress(progress);
				}
			});

			return job;
		}

		private void initModel(TrainViewModel model) {
			model.OutputSize = db.Symbols.Count();
			model.TrainingSamples = db.SymbolDrawings.Count();
			model.ExampleDrawings = db.SymbolDrawings.Take(16).ToList();
		}

		public static int[] parseInts(string str) {
			var sizesStr = str.Split(new char[] { ' ', '\t', ',', ';', '-' }, StringSplitOptions.RemoveEmptyEntries);
			if (sizesStr.Length == 0) {
				return new int[0];
			}

			int[] sizes = new int[sizesStr.Length];
			for (int i = 0; i < sizes.Length; i++) {
				if (!int.TryParse(sizesStr[i], out sizes[i])) {
					return null;
				}
			}

			return sizes;
		}

	}
}