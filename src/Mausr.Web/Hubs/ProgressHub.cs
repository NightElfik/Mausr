using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mausr.Web.Models;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Hubs {
	public class ProgressHub : Hub {

		public void TrackJob(string jobId) {
			Groups.Add(Context.ConnectionId, jobId);
		}

		public void CancelJob(string jobId) {
			var job = JobManager.Instance.GetJob(jobId);
			if (job != null) {
				job.Cancel();
			}
		}

		public void ProgressChanged(string jobId, int progress) {
			Clients.Group(jobId).progressChanged(jobId, progress);
		}

		public void JobCompleted(string jobId) {
			Clients.Group(jobId).jobCompleted(jobId);
		}

	}
}