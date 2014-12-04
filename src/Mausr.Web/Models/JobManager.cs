using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mausr.Web.Hubs;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Models {
	public class JobManager {

		public static readonly JobManager Instance = new JobManager();


		private ConcurrentDictionary<string, Job> runningJobs = new ConcurrentDictionary<string, Job>();
		private IHubContext progressHubContext;


		private JobManager() {
			progressHubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
		}

		public bool TryStartJobAsync(string jobId, Action<Job> action) {
			var job = new Job(jobId);
			var dbJob = runningJobs.GetOrAdd(job.Id, job);
			if (dbJob != job) {
				return false;  // Some job is already running.
			}

			Task.Factory.StartNew(() => {
					action(job);
					job.IsComplete = true;
					runningJobs.TryRemove(job.Id, out job);
				},
				TaskCreationOptions.LongRunning);
			
			return true;
		}

		public bool StopJob(string id) {
			Job job;
			if (runningJobs.TryGetValue(id, out job)) {
				job.Cancel();
				return true;
			}

			return false;
		}
		
		public Job GetJob(string id) {
			Job result;
			return runningJobs.TryGetValue(id, out result) ? result : null;
		}


		public dynamic GetClientoObject(Job job) {
			return progressHubContext.Clients.Group(job.Id);
		}
	}
}