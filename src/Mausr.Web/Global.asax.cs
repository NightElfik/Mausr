using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Mausr.Web.Entities;
using Mausr.Web.Infrastructure;
using Mausr.Web.NeuralNet;

namespace Mausr.Web {
	public class MvcApplication : HttpApplication {

		protected void Application_Start() {
			AppSettingsProvider appSettings = new AppSettingsProvider();						
			loadPrivateData(appSettings.PrivateDir);

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<MausrDb, Mausr.Web.Migrations.Configuration>());

			var resolver = buildDependencyResolver(appSettings);
			DependencyResolver.SetResolver(resolver);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			checkFileSystem(appSettings);

			Logger.LogInfo<MvcApplication>("App started.");
		}
		
		protected void Application_End() {
			Logger.LogInfo<MvcApplication>("App ended.");
		}
		

		private void loadPrivateData(string virtPath) {
			string absPath = Server.MapPath(virtPath);
			PrivateData.LoadFormDir(absPath);
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.GoogleAnalyticsKey));
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.ReCaptchaPrivate));
			Debug.Assert(!string.IsNullOrWhiteSpace(PrivateData.ReCaptchaPublic));
		}

		private IDependencyResolver buildDependencyResolver(AppSettingsProvider appSettings) {
			
			var builder = new ContainerBuilder();

			// Register all MVC controllers.
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			// OPTIONAL: Register model binders that require DI.
			//builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
			//builder.RegisterModelBinderProvider();

			// OPTIONAL: Register web abstractions like HttpContextBase.
			builder.RegisterModule<AutofacWebTypesModule>();

			// OPTIONAL: Enable property injection in view pages.
			//builder.RegisterSource(new ViewRegistrationSource());

			// OPTIONAL: Enable property injection into action filters.
			//builder.RegisterFilterProvider();


			builder.RegisterType<MausrDb>()
				//.As<Idb>()
				.InstancePerRequest();

			builder.Register(x => appSettings)
				.As<AppSettingsProvider>()
				.SingleInstance();

			var tsm = new TrainStorageManager(appSettings);
			builder.Register(x => tsm)
				.As<TrainStorageManager>()
				.SingleInstance();

			var evaluator = new CurrentEvaluator(tsm);
			builder.Register(x => evaluator)
				.As<CurrentEvaluator>()
				.SingleInstance();
			
			var captcha = new ReCaptcha(PrivateData.ReCaptchaPublic, PrivateData.ReCaptchaPrivate);
			builder.Register(x => captcha)
				.As<ICaptcha>()
				.SingleInstance();

			var container = builder.Build();
			return new AutofacDependencyResolver(container);
		}


		/// <summary>
		/// Checks if all necessary directories exists and are writable.
		/// </summary>
		private void checkFileSystem(AppSettingsProvider appSettingsProvider) {

			ensureDirExistsAndIsWritable(appSettingsProvider.SymbolDrawingsCacheDirVirtual);
			ensureDirExistsAndIsWritable(appSettingsProvider.DrawingsCacheDirVirtual);
			ensureDirExistsAndIsWritable(appSettingsProvider.NetTrainDataDirVirtual);

			{
				// Checks whether ELMAH's error logs directory exists and is writable.
				// This is not elegant solution but better than nothing (I can not find better solution).
				var section = WebConfigurationManager.OpenWebConfiguration("/").GetSection("elmah/errorLog") as DefaultSection;
				string rawXml = section.SectionInformation.GetRawXml();

				const string start = "logPath=\"";
				const string end = "\"";
				int startI = rawXml.IndexOf(start) + start.Length;
				int endI = rawXml.IndexOf(end, startI);
				string errReportDirPath = rawXml.Substring(startI, endI - startI);
				if (errReportDirPath.Length == 0) {
					throw new Exception(string.Format("Invalid logging dir '{0}', fix the config or the parsing of the path.",
						errReportDirPath));
				}

				ensureDirExistsAndIsWritable(errReportDirPath);
			}

			{
				// Checks whether Log4Net's error logs directory exists and is writable.
				// This is not elegant solution but better than nothing (I can not find better solution).
				var section = WebConfigurationManager.OpenWebConfiguration("/").GetSection("log4net") as DefaultSection;
				string rawXml = section.SectionInformation.GetRawXml();

				const string start = "<file value=\"";
				const string end = "/";
				int startI = rawXml.IndexOf(start) + start.Length;
				int endI = rawXml.IndexOf(end, startI);
				string errReportDirPath = rawXml.Substring(startI, endI - startI);
				if (errReportDirPath.Length == 0) {
					throw new Exception(string.Format("Invalid logging dir '{0}', fix the config or the parsing of the path.",
						errReportDirPath));
				}

				ensureDirExistsAndIsWritable(errReportDirPath);
			}

		}

		/// <summary>
		/// Ensures that directory at given path exists and is writable.
		/// Directory is created if don't exist and exception is thrown if is not writable.
		/// </summary>
		private void ensureDirExistsAndIsWritable(string path, bool pathIsVirtual = true) {
			if (pathIsVirtual) {
				path = Server.MapPath(path);
			}

			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			string filePath = Path.Combine(path, "IsThisDirectoryWritableTest." + DateTime.UtcNow.Ticks.ToString());
			try {
				File.Create(filePath).Dispose();
				File.Delete(filePath);
			}
			catch (Exception ex) {
				throw new Exception(string.Format("Directory '{0}' is not writable!", path), ex);
			}
		}


	}
}
