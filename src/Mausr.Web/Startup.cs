using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mausr.Web.Startup))]
namespace Mausr.Web {
	public partial class Startup {
		public void Configuration(IAppBuilder app) {
			ConfigureAuth(app);
			app.MapSignalR();
		}
	}
}
