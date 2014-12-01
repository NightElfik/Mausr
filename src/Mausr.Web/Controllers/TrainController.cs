using System.Linq;
using System.Web.Mvc;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class TrainController : Controller {
		
		protected readonly MausrDb db;


		public TrainController(MausrDb db) {
			this.db = db;
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
			if (!ModelState.IsValid) {
				initModel(model);
				return View(model);
			}

			int inputSize = model.InputImgSizePx * model.InputImgSizePx;
			int outputSize = db.Symbols.Count();

			var layout = NetLayout.Parse(inputSize + " " + model.HiddenLayersSizes + " " + outputSize);
			var net = new Net(layout, new SigomidActivationFunc());

			var optimizer = new SteepestDescentAdvancedOptmizer(model.LearningRate,
				model.MomentumStartPerc, model.MomentumEndPerc, model.MinDerivativeMagnitude, model.MaxIteratinosPerBatch);
			var trainer = new NetTrainer(net, optimizer, model.RegularizationLambda);

			//trainer.Train();

			return View();
		}


		private void initModel(TrainViewModel model) {
			model.OutputSize = db.Symbols.Count();
			model.TrainingSamples = db.SymbolDrawings.Count();
			model.ExampleDrawings = db.SymbolDrawings.Take(16).ToList();
		}

	}
}