using System.Web.Optimization;

namespace Mausr.Web {
	public class BundleConfig {

		public static void RegisterBundles(BundleCollection bundles) {

			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));


			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Css/bootstrap.css",
					  "~/Css/font-awesome.css",
					  "~/Css/site.css"));

#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}

	}
}
