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

		public virtual ActionResult RosenbrockFunction(PlotImageModel model) {
			if (!ModelState.IsValid) {
				model = new PlotImageModel() {
					OriginX = 1,
					OriginY = 1,
					Width = 512,
					Height = 512,
					StepPerPixel = 0.01,
					ContoursCount = 10,
					ScalePower = 0.2,
				};
			}

			return View(Views.Plot, new PlotViewModel() {
				Name = "Rosenbrock function",
				ViewAction = MVC.Plot.RosenbrockFunctionPlot(),
				ImageModel = model,
			});
		}

		public virtual ActionResult RosenbrockFunctionPlot(PlotImageModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var f = new RosenbrockFunction();
			return plot(f, model);
		}

		public virtual ActionResult CrazySinCosFunction(PlotImageModel model) {
			if (!ModelState.IsValid) {
				model = new PlotImageModel() {
					OriginX = 0,
					OriginY = -1,
					Width = 512,
					Height = 512,
					StepPerPixel = 0.01,
					ContoursCount = 10,
					ScalePower = 1,
				};
			}
			
			return View(Views.Plot, new PlotViewModel() {
				Name = "Crazy sin cos function",
				ViewAction = MVC.Plot.CrazySinCosFunctionPlot(),
				ImageModel = model,
			});
		}

		public virtual ActionResult CrazySinCosFunctionPlot(PlotImageModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var f = new SinCosCrazyFunction();
			return plot(f, model);
		}

		private ActionResult plot(IFunctionWithDerivative f, PlotImageModel model) {
			
			var fp = new FunctionPlotter();
			var o = new DenseVector(2);
			o[0] = model.OriginX;
			o[1] = model.OriginY;

			var img = fp.ContourPlot(f, o, 0, 1, model.Width, model.Height, model.StepPerPixel,
				model.ContoursCount, model.ScalePower);
			var ms = new MemoryStream();
			img.Save(ms, ImageFormat.Png);
			ms.Seek(0, SeekOrigin.Begin);

			return File(ms, "image/png");
		}

	}
}