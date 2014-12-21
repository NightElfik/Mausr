using System;
using System.Linq;
using System.Web.Mvc;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Mausr.Web.NeuralNet;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	[Authorize(Roles = RolesHelper.Trainer)]
	public partial class TrainController : Controller {

		protected readonly MausrDb db;
		protected readonly TrainStorageManager trainStorageManager;
		protected readonly CurrentEvaluator evaluator;


		public TrainController(MausrDb db, TrainStorageManager trainStorageManager, CurrentEvaluator evaluator) {
			this.db = db;
			this.trainStorageManager = trainStorageManager;
			this.evaluator = evaluator;
		}

		public virtual ActionResult Index() {
			return View(new TrainIndexViewModel() {
				AllNets = trainStorageManager.LoadAllSavedNets(),
				DefaultNet = trainStorageManager.LoadDefaultNetName(),
			});
		}

		[HttpGet]
		public virtual ActionResult NetSettings(string id = null) {
			Logger.LogInfo<TrainController>("Net settings opened (id=[{0}])", id ?? "null");

			TrainViewModel model = null;

			if (id != null) {
				var settings = trainStorageManager.LoadTrainSettings(id);
				if (settings != null) {
					model = settings.ShallowCloneAs<TrainViewModel>();
					model.HiddenLayersSizesStr = string.Join("; ", settings.HiddenLayersSizes);
				}
			}

			if (model == null) {
				model = new TrainViewModel() {
					InputImgSizePx = 20,
					PenThicknessPerc = 12,
					GenerateExtraInputsByRotation = 10,
					NormalizeInput = true,
					LearnRounds = 1,
					BatchSize = 0,
					MaxIteratinosPerBatch = 100,
					RegularizationLambda = 0.5,
					MinDerivCompMaxMagn = 1e-3,
					OptimizationAlgorithm = OptimizationAlgorithm.ImprovedRpropMinus,
					InitSeed = new Random().Next(),

					LearningRate = 0.1,
					MomentumStartPerc = 60,
					MomentumEndPerc = 90,

					RpropInitStep = 0.01,
					RpropMaxStep = 10,
					RpropStepUpMult = 1.2,
					RpropStepDownMult = 0.5,

					TestDataSetSizePerc = 10,
					TrainEvaluationIters = 3,
					SkipFrstIters = 6,
				};
			}

			initModel(model);

			return View(model);
		}

		[HttpPost]
		public virtual ActionResult NetSettings(string id = null, TrainViewModel model = null) {
			if (ModelState.IsValid) {
				if (string.IsNullOrWhiteSpace(id)) {
					model.NetId = trainStorageManager.CreateSafeNetId(DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss-") + model.NetName);
				}
				else {
					model.NetId = trainStorageManager.CreateSafeNetId(id);
				}

				model.HiddenLayersSizes = parseInts(model.HiddenLayersSizesStr);
				bool hlValid = model.HiddenLayersSizes != null
					&& model.HiddenLayersSizes.All(s => s > 0 && s < 65536);
				if (hlValid) {
					if (trainStorageManager.SaveTrainSettings(model.NetId, model.ShallowCloneAs<TrainSettings>())) {
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
				TrainData = isJobRunning(id) ? null : trainStorageManager.LoadTrainData(id),
			};

			return View(model);
		}

		[HttpPost]
		public virtual ActionResult StartTraining(string id) {
			if (id == null) {
				return HttpNotFound();
			}

			bool success = startTrainig(id);
			return Json(new {
				success = success,
				message = success ? "Training started successfully." : "Training is already running."
			});
		}

		[HttpPost]
		public virtual ActionResult StopTraining(string id, bool? cancel = null) {
			if (id == null) {
				return HttpNotFound();
			}

			bool cancelValue = cancel ?? true;
			bool success = stopTrainig(id, cancelValue);
			return Json(new {
				success = success,
				message = success
					? string.Format("Training was {0} successfully.", cancelValue ? "canceled" : "stopped")
					: "Training is not running."
			});
		}

		[HttpPost]
		public virtual ActionResult SetDefault(string id) {
			if (id == null) {
				return HttpNotFound();
			}

			bool success = evaluator.SetDefaultNetwork(id);
			return Json(new {
				success = success,
				message = success ? "Default network was set successfully." : "Failed to set default network."
			});
		}

		[HttpGet]
		public virtual ActionResult DeleteNet(string id) {
			return View((object)id);
		}

		[HttpPost]
		[ActionName("DeleteNet")]
		public virtual ActionResult DeleteNetPost(string id) {
			if (id == null) {
				return HttpNotFound();
			}

			string defaultNetName = trainStorageManager.LoadDefaultNetName();
			if (id == defaultNetName) {				
				return HttpNotFound();
			}

			trainStorageManager.DeleteNet(id);

			return RedirectToAction(Actions.Index());
		}

		
		private bool isJobRunning(string id) {
			return JobManager.Instance.GetJob(id) != null;
		}

		private bool startTrainig(string id) {
			var trainSettings = trainStorageManager.LoadTrainSettings(id);
			if (trainSettings == null) {
				return false;
			}

			Logger.LogInfo<TrainController>("Training of [{0}] started:\n{1}\n",
				id, JsonConvert.SerializeObject(trainSettings));
			return JobManager.Instance.TryStartJobAsync(id, j => {
				new NetDbTrainer(id, trainStorageManager, j).TrainNetwork();
			});
		}

		private bool stopTrainig(string id, bool cancel) {
			Logger.LogInfo<TrainController>("Training of [{0}] was {1}",
				id, cancel ? "canceled" : "stopped");
			return JobManager.Instance.StopJob(id, cancel);
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