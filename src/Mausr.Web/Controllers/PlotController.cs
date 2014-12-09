using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;
using Mausr.Core.Plot;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
	[Authorize(Roles = RolesHelper.Trainer)]
	public partial class PlotController : Controller {

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult RosenbrockFunction(PlotViewModel model) {
			if (!ModelState.IsValid) {
				if (model.Width == 0 && model.Height == 0) {
					ModelState.Clear();
				}
				model = new PlotViewModel() {
					Width = 400,
					Height = 400,
					ContoursCount = 10,
					ScalePower = 0.2,

					InitialX = -0.1,
					InitialY = 4.5,
					MinDerivMagn = 0.1,
					MaxIters = 1024,

					BasicStep = 0.0005,

					MomentumStep = 0.0005,
					MomentumStart = 0.6,
					MomentumEnd = 0.9,
				};
			}

			ViewBag.Name = "Rosenbrock function";
			ViewBag.ViewAction = MVC.Plot.RosenbrockFunctionPlot();

			return View(Views.Plot, model);
		}

		public virtual ActionResult RosenbrockFunctionPlot(PlotViewModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var f = new RosenbrockFunction();
			return plot(f, model);
		}

		public virtual ActionResult CrazySinCosFunction(PlotViewModel model) {
			if (!ModelState.IsValid) {
				if (model.Width == 0 && model.Height == 0) {
					ModelState.Clear();
				}
				model = new PlotViewModel() {
					Width = 400,
					Height = 400,
					ContoursCount = 10,
					ScalePower = 1,

					InitialX = -0.35,
					InitialY = -2,
					MinDerivMagn = 0.001,
					MaxIters = 256,

					BasicStep = 0.05,

					MomentumStep = 0.05,
					MomentumStart = 0.6,
					MomentumEnd = 0.9,
				};
			}

			ViewBag.Name = "Crazy sin cos function";
			ViewBag.ViewAction = MVC.Plot.CrazySinCosFunctionPlot();

			return View(Views.Plot, model);
		}

		public virtual ActionResult CrazySinCosFunctionPlot(PlotViewModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var f = new SinCosCrazyFunction();
			return plot(f, model);
		}

		private ActionResult plot(IFunctionWithDerivative f, PlotViewModel model) {

			List<Tuple<Color, List<Vector<double>>>> points = new List<Tuple<Color, List<Vector<double>>>>();
			var result = new DenseVector(f.DimensionsCount);
			{
				result[0] = model.InitialX;
				result[1] = model.InitialY;
				var ps = new List<Vector<double>>();
				points.Add(new Tuple<Color, List<Vector<double>>>(Color.DarkGreen, ps));
				var sdImpl = new SteepestDescentBasicOptmizer(model.BasicStep, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(result, f, p => ps.Add(p), CancellationToken.None);
			}
			{
				result[0] = model.InitialX;
				result[1] = model.InitialY;
				var ps = new List<Vector<double>>();
				points.Add(new Tuple<Color, List<Vector<double>>>(Color.DarkRed, ps));
				var sdImpl = new SteepestDescentBasicOptmizer(model.MomentumStep,
					model.MomentumStart, model.MomentumEnd, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(result, f, p => ps.Add(p), CancellationToken.None);
			}
			{
				result[0] = model.InitialX;
				result[1] = model.InitialY;
				var ps = new List<Vector<double>>();
				points.Add(new Tuple<Color, List<Vector<double>>>(Color.Blue, ps));
				var sdImpl = new SteepestDescentAdvancedOptmizer(model.MomentumStep,
					model.MomentumStart, model.MomentumEnd, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(result, f, p => ps.Add(p), CancellationToken.None);
			}

			var fp = new FunctionPlotter();

			var img = fp.AutoContourPlot(f, points, 0.1f, 0, 1, model.Width, model.Height,
				model.ContoursCount, model.ScalePower);
			var ms = new MemoryStream();
			img.Save(ms, ImageFormat.Png);
			ms.Seek(0, SeekOrigin.Begin);

			return File(ms, "image/png");
		}

	}
}