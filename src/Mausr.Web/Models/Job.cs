using System;
using System.Threading;

namespace Mausr.Web.Models {
	public class Job {

		private CancellationTokenSource cancellationTokenSource;

		public string Id { get; private set; }

		private float p_progress = 0;
		public float Progress {
			get { return p_progress; }
			set {
				if (value == p_progress) {
					return;
				}

				p_progress = value;
				Clients.progressChanged(p_progress);
			}
		}

		private bool p_isComplete = false;
		public bool IsComplete {
			get {
				return p_isComplete;
			}
			set {
				if (value == p_isComplete) {
					return;
				}

				p_isComplete = value;
				Clients.jobCompleted();
			}
		}


		public dynamic Clients { get { return JobManager.Instance.GetClientoObject(this); } }


		public CancellationToken CancellationToken {
			get { return cancellationTokenSource.Token; }
		}


		public Job(string id) {
			Id = id;
			cancellationTokenSource = new CancellationTokenSource();
		}

		public void Cancel() {
			cancellationTokenSource.Cancel();
		}
	}
}