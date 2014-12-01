using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	public partial class TrainController : Controller {
		
		protected readonly SymbolsDb symbolsDb;


		public TrainController(SymbolsDb symbolsDb) {
			this.symbolsDb = symbolsDb;
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
			int outputSize = symbolsDb.Symbols.Count();

			var layout = NetLayout.Parse(inputSize + " " + model.HiddenLayersSizes + " " + outputSize);
			var net = new Net(layout, new SigomidActivationFunc());

			var optimizer = new SteepestDescentAdvancedOptmizer(model.LearningRate,
				model.MomentumStartPerc, model.MomentumEndPerc, model.MinDerivativeMagnitude, model.MaxIteratinosPerBatch);
			var trainer = new NetTrainer(net, optimizer, model.RegularizationLambda);

			//trainer.Train();

			return View();
		}


		private void initModel(TrainViewModel model) {
			model.OutputSize = symbolsDb.Symbols.Count();
			model.TrainingSamples = symbolsDb.SymbolDrawings.Count();
			model.ExampleDrawings = symbolsDb.SymbolDrawings.Take(16).ToList();
		}

	}
}