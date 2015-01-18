using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;
using Mausr.Core;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.Models;
using Mausr.Web.NeuralNet;
using Newtonsoft.Json;

namespace Mausr.Web.Controllers {
	public partial class HomeController : Controller {

		private const int GUID_CHECK_WINDOW_SECONDS = 5;

		private static readonly Regex CleanNameRegex = new Regex(@"[^a-zA-Z0-9!#$%&'*+-/=?^_{|}~(),:;\[\] ]+",
			RegexOptions.Compiled);
		private static readonly Regex CleanSubjectRegex = new Regex(@"[^a-zA-Z0-9!#$%&'*+-/=?^_{|}~(),:;@\[\]]+",
			RegexOptions.Compiled);

		protected readonly MausrDb db;
		protected readonly CurrentEvaluator evaluator;
		protected readonly ICaptcha captcha;

		public HomeController(MausrDb db, CurrentEvaluator evaluator, ICaptcha captcha) {
			this.db = db;
			this.evaluator = evaluator;
			this.captcha = captcha;
		}

		public virtual ActionResult Index() {
			return View();
		}

		public virtual ActionResult About() {
			return View();
		}

		[HttpPost]
		public virtual ActionResult Predict(PredictModel model) {
			if (!ModelState.IsValid) {
				return HttpNotFound();
			}

			var sw = Stopwatch.StartNew();

			RawDrawing rawDrawing;
			try {
				var lines = JsonConvert.DeserializeObject<RawPoint[][]>(model.JsonData);
				rawDrawing = new RawDrawing() { Lines = lines };
			}
			catch (Exception ex) {
				return HttpNotFound();
			}

			var predictions = evaluator.PredictTopN(rawDrawing, 8, 0.05);

			// TODO: Optimize this query/step.
			var rawResults = predictions.Join(db.Symbols, p => p.OutputId, s => s.SymbolId, (p, s) => new {
				Symbol = s,
				Rating = (float)p.NeuronOutputValue,
			}).ToList();

			// Warm-up queries with no lines should interact with the DB above but not proceed any further.
			if (rawDrawing.LinesCount > 0) {
				var minTime = DateTime.UtcNow.AddSeconds(-GUID_CHECK_WINDOW_SECONDS);

				var drawing = db.Drawings
					.Where(d => d.ClientGuid == model.Guid && DateTime.Compare(d.DrawnDateTime, minTime) > 0)
					.FirstOrDefault();

				if (drawing == null) {
					drawing = new Drawing();
					drawing.DrawnDateTime = DateTime.UtcNow;
					drawing.ClientGuid = model.Guid;
					db.Drawings.Add(drawing);
				}
#if DEBUG
				else {
					// Delete potentially cached image - this does not happen ofthen but it is annoying when it does happen.
					new DrawingsController(db, DependencyResolver.Current.GetService<AppSettingsProvider>())
						.ClearCachedImage(drawing.DrawingId);
				}
#endif

				var firstResult = rawResults.FirstOrDefault();

				drawing.TopSymbol = firstResult == null ? null : firstResult.Symbol;
				drawing.TopSymbolScore = firstResult == null ? null : (double?)firstResult.Rating;
				drawing.DrawnUsingTouch = model.DrawnUsingTouch;
				drawing.SetRawDrawing(rawDrawing);

				db.SaveChanges();
			}

			sw.Stop();

			return Json(new {
				Results = rawResults.Select(x => new {
					SymbolId = x.Symbol.SymbolId,
					Symbol = x.Symbol.SymbolStr,
					SymbolName = x.Symbol.Name,
					Rating = x.Rating,
					HtmlEntity = x.Symbol.HtmlEntity ?? "",
					UtfCode = char.ConvertToUtf32(x.Symbol.SymbolStr, 0),
				}),
				Duration = (float)sw.Elapsed.TotalMilliseconds,
			});
		}

		[HttpGet]
		public virtual ActionResult Contact() {
			return View(new ContactModel() {
				Captcha = captcha
			});
		}

		[HttpPost]
		public virtual ActionResult Contact(ContactModel model) {
			model.Captcha = captcha;
			if (!ModelState.IsValid) {
				return View(model);
			}

			if (!captcha.Validate(ControllerContext.HttpContext)) {
				ModelState.AddModelError("", "Capthca invalid.");
				return View(model);
			}

			string email = model.Email == null ? "unknown@mausr.com" : model.Email.Trim();
			string rawName = model.Name.Trim();
			string subejct = model.Subject.Trim();
			string body = model.Message.Trim();

			string cleanName = CleanNameRegex.Replace(rawName, "_");
			string from = string.Format("\"{0}\" <{1}>", cleanName, email);
			string cleanSubject = CleanSubjectRegex.Replace(subejct, "_");

			Logger.LogInfo<HomeController>("Contact message sent\n\tfrom: {0} <{1}>\n\tsubj: {2}\n\tmsg: {3}\n",
				rawName, model.Email == null ? "null" : email, subejct, body);

			try {
				WebMail.Send("mausr@marekfiser.cz", cleanSubject + " [Mausr.com]", body, from);

				ViewBag.Message = "E-mail sent successfully.";
				return View();
			}
			catch (Exception ex) {
				Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Failed to send e-mail", ex));
				ModelState.AddModelError("", "Failed to send e-mail.");
			}

			return View(model);
		}

		public virtual ActionResult TestMail() {
			WebMail.Send("mausr@marekfiser.cz", "Mausr.com - test", "test");
			return HttpNotFound();
		}

		public virtual ActionResult TrainDetails() {
			var model = new TrainDetailsViewModel() {
				TrainSettings = evaluator.TrainSettings,
				TrainData = evaluator.TrainData,
			};

			return View(model);
		}
				
		[Route("Home/TestSetImage.png")]
		[OutputCache(CacheProfile = CacheProfileKeys.LongClientCache)]
		public virtual ActionResult TestSetImage() {
			var stream = evaluator.TrainStorageManager.OpenTestResultsImg(evaluator.NetId);
			return new FileStreamResult(stream, MimeType.Image.Png);
		}
	}
}