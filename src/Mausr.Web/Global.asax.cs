using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Mausr.Web.DataContexts;
using Mausr.Web.Infrastructure;

namespace Mausr.Web {
	public class MvcApplication : HttpApplication {
		protected void Application_Start() {

			var resolver = buildDependencyResolver();
			DependencyResolver.SetResolver(resolver);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			
			var asp = resolver.GetService<AppSettingsProvider>();
			checkFileSystem(asp);
		}


		private IDependencyResolver buildDependencyResolver() {

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


			builder.RegisterType<SymbolsDb>()
				//.As<ISymbolsDb>()
				.InstancePerRequest();

			builder.Register(x => new AppSettingsProvider())
				.As<AppSettingsProvider>()
				.SingleInstance();
			
			var container = builder.Build();
			return new AutofacDependencyResolver(container);
		}


		/// <summary>
		/// Checks if all necessary directories exists and are writable.
		/// </summary>
		private void checkFileSystem(AppSettingsProvider appSettingsProvider) {

			ensureDirExistsAndIsWritable(appSettingsProvider.SymbolDrawingsCacheDirVirtual);

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
