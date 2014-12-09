using System.Threading.Tasks;
using Mausr.Web.Models;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Hubs {
	[Authorize(Roles = RolesHelper.Trainer)]
	public class ProgressHub : Hub {

		public async Task<bool> TrackJob(string id) {
			await Groups.Add(Context.ConnectionId, id);
			return JobManager.Instance.GetJob(id) != null;
		}

	}
}