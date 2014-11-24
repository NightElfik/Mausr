using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;
using Mausr.Core.Plot;
using Mausr.Web.Models;

namespace Mausr.Web.Controllers {
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
					OriginX = 0,
					OriginY = 2,
					Width = 512,
					Height = 512,
					StepPerPixel = 0.01,
					ContoursCount = 10,
					ScalePower = 0.2,

					InitialX = -0.1,
					InitialY = 4.5,
					MinDerivMagn = 0.1,
					MaxIters = 4096,

					BasicStep = 0.0005,

					MomentumStep = 0.0005,
					MomentumStart = 0.6,
					MomentumEnd = 0.99,
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
					OriginX = 0.5,
					OriginY = -1.8,
					Width = 512,
					Height = 512,
					StepPerPixel = 0.006,
					ContoursCount = 10,
					ScalePower = 1,

					InitialX = -0.35,
					InitialY = -2,
					MinDerivMagn = 0.001,
					MaxIters = 512,

					BasicStep = 0.05,

					MomentumStep = 0.05,
					MomentumStart = 0.6,
					MomentumEnd = 0.99,
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

			List<Tuple<Color, List<DenseVector>>> points = new List<Tuple<Color, List<DenseVector>>>();
			var initPos = new DenseVector(2);
			initPos[0] = model.InitialX;
			initPos[1] = model.InitialY;
			{
				var ps = new List<DenseVector>();
				points.Add(new Tuple<Color, List<DenseVector>>(Color.DarkGreen, ps));
				var sdImpl = new SteepestDescentBasicOptmizer(model.BasicStep, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(ps, f, initPos);
			}
			{
				var ps = new List<DenseVector>();
				points.Add(new Tuple<Color, List<DenseVector>>(Color.DarkRed, ps));
				var sdImpl = new SteepestDescentBasicOptmizer(model.MomentumStep,
					model.MomentumStart, model.MomentumEnd, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(ps, f, initPos);
			}
			{
				var ps = new List<DenseVector>();
				points.Add(new Tuple<Color, List<DenseVector>>(Color.Blue, ps));
				var sdImpl = new SteepestDescentAdvancedOptmizer(model.MomentumStep,
					model.MomentumStart, model.MomentumEnd, model.MinDerivMagn, model.MaxIters);
				sdImpl.Optimize(ps, f, initPos);
			}

			var fp = new FunctionPlotter();
			var o = new DenseVector(2);
			o[0] = model.OriginX;
			o[1] = model.OriginY;

			var img = fp.ContourPlot(f, o, 0, 1, model.Width, model.Height, model.StepPerPixel,
				model.ContoursCount, model.ScalePower, points);
			var ms = new MemoryStream();
			img.Save(ms, ImageFormat.Png);
			ms.Seek(0, SeekOrigin.Begin);

			return File(ms, "image/png");
		}

	}
}