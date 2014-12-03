using System;
using System.Threading;

namespace Mausr.Web.Models {
	public class Job {
		
		private volatile int _progress;
		private volatile bool _completed;
		private CancellationTokenSource _cancellationTokenSource;

		public event EventHandler<EventArgs> ProgressChanged;
		public event EventHandler<EventArgs> Completed;
		

		public string Id { get; private set; }

		public int Progress {
			get { return _progress; }
		}

		public bool IsComplete {
			get { return _completed; }
		}

		public CancellationToken CancellationToken {
			get { return _cancellationTokenSource.Token; }
		}


		public Job(string id) {
			Id = id;
			_cancellationTokenSource = new CancellationTokenSource();
		}


		public void ReportProgress(int progress) {
			if (_progress != progress) {
				_progress = progress;
				OnProgressChanged();
			}
		}

		public void ReportComplete() {
			if (!IsComplete) {
				_completed = true;
				OnCompleted();
			}
		}

		protected virtual void OnCompleted() {
			var handler = Completed;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		protected virtual void OnProgressChanged() {
			var handler = ProgressChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		public void Cancel() {
			_cancellationTokenSource.Cancel();
		}
	}
}