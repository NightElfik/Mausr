using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Mausr.Web.DataContexts;

namespace Mausr.Web {
	public class MvcApplication : HttpApplication {
		protected void Application_Start() {

			DependencyResolver.SetResolver(buildDependencyResolver());

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
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


			var container = builder.Build();
			return new AutofacDependencyResolver(container);
		}
	}
}
