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
			return View();
		}

		[HttpPost]
		public virtual ActionResult Train(TrainViewModel model) {
			if (!ModelState.IsValid) {
				return View(model);
			}

			int inputSize = model.InputImgSizePx * model.InputImgSizePx;
			int outputSize = symbolsDb.Symbols.Count();

			var layout = NetLayout.Parse(inputSize + " " + model.HiddenLayersSizes + " " + outputSize);
			var net = new Net(layout, new SigomidActivationFunc());

			var optimizer = new SteepestDescentAdvancedOptmizer(model.LearningRate,
				model.MomentumStart, model.MomentumEnd, model.MinDerivativeMagnitude, model.MaxIteratinos);
			var trainer = new NetTrainer(net, optimizer, model.RegularizationLambda);

			//trainer.Train();

			return View();
		}

	}
}