using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mausr.Web.Hubs;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Models {
	public class JobManager {

		public static readonly JobManager Instance = new JobManager();


		private ConcurrentDictionary<string, Job> _runningJobs = new ConcurrentDictionary<string, Job>();
		private IHubContext _hubContext;


		public JobManager() {
			_hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
		}

		public Job TryStartJobAsync(string jobId, Action<Job> action) {
			var job = new Job(jobId);
			var dbJob = _runningJobs.GetOrAdd(job.Id, job);
			if (dbJob != job) {
				return dbJob;  // Some job is already running.
			}

			Task.Factory.StartNew(() => {
					action(job);
					job.ReportComplete();
					_runningJobs.TryRemove(job.Id, out job);
				},
				TaskCreationOptions.LongRunning);

			BroadcastJobStatus(job);

			return job;
		}

		private void BroadcastJobStatus(Job job) {
			job.ProgressChanged += HandleJobProgressChanged;
			job.Completed += HandleJobCompleted;
		}

		private void HandleJobCompleted(object sender, EventArgs e) {
			var job = (Job)sender;

			_hubContext.Clients.Group(job.Id).jobCompleted(job.Id);

			job.ProgressChanged -= HandleJobProgressChanged;
			job.Completed -= HandleJobCompleted;
		}

		private void HandleJobProgressChanged(object sender, EventArgs e) {
			var job = (Job)sender;
			_hubContext.Clients.Group(job.Id).progressChanged(job.Id, job.Progress);
		}

		public Job GetJob(string id) {
			Job result;
			return _runningJobs.TryGetValue(id, out result) ? result : null;
		}

	}
}