using System.Threading.Tasks;
using Mausr.Web.Models;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Hubs {
	public class ProgressHub : Hub {

		public async Task<bool> TrackJob(string jobId) {
			await Groups.Add(Context.ConnectionId, jobId);
			return JobManager.Instance.GetJob(jobId) != null;
		}

		public void CancelJob(string jobId) {
			var job = JobManager.Instance.GetJob(jobId);
			if (job != null) {
				job.Cancel();
			}
		}

		//public void ProgressChanged(string jobId, int progress) {
		//	Clients.Group(jobId).progressChanged(jobId, progress);
		//}

		//public void JobCompleted(string jobId) {
		//	Clients.Group(jobId).jobCompleted(jobId);
		//}

	}
}