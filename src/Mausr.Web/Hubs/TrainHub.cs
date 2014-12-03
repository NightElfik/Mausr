using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Mausr.Core.NeuralNet;
using Mausr.Core.Optimization;
using Mausr.Web.DataContexts;
using Mausr.Web.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace Mausr.Web.Hubs {
	public class TrainHub : Hub {
		
		//private static ConcurrentDictionary<string, TrainingContext> trainings;

		//public void StartTraining(string netName) {


		//	caller = Clients.Caller;
		//	if (trainings.ContainsKey(netName)) {
		//		Clients.
		//	}

		//	stopwatch = new Stopwatch();
		//	stopwatch.Start();

		//	var storageManager = new TrainStorageManager(new AppSettingsProvider());
		//	message("Loading training settings.");
		//	var trainSettings = storageManager.LoadTrainSettings(netName);
		//	if (trainSettings == null) {
		//		fail("Grain settings file was not found.");
		//		return;
		//	}
			
		//	using (var db = MausrDb.Create()) {
		//		message("Initializing learning environment.");
		//		int inputSize = trainSettings.InputImgSizePx * trainSettings.InputImgSizePx;
		//		int outputSize = db.Symbols.Count();

		//		var layout = new NetLayout(inputSize, trainSettings.HiddenLayersSizes, outputSize);
		//		var net = new Net(layout, new SigomidActivationFunc());

		//		var optimizer = new SteepestDescentAdvancedOptmizer(trainSettings.LearningRate,
		//			trainSettings.MomentumStartPerc / 100.0, trainSettings.MomentumEndPerc / 100.0,
		//			trainSettings.MinDerivativeMagnitude, trainSettings.MaxIteratinosPerBatch);
		//		var trainer = new NetTrainer(net, optimizer, trainSettings.RegularizationLambda);

		//		message("Preparing inputs and outputs.");
		//		var drawings = db.SymbolDrawings;

				
		//		message(string.Format("Starting learning of {0} samples.", drawings.Count()));
		//		//trainer.Train();
		//	}

		//	done("Learning successfully finished.");
		//}
		
		//private void done(string msg, params object[] args) {
		//	string timeStamp = string.Format("[{0:0000.00}] ", stopwatch.Elapsed.TotalSeconds);
		//	caller.done(timeStamp + string.Format(msg, args));
		//}

		//private void fail(string msg, params object[] args) {
		//	string timeStamp = string.Format("[{0:0000.00}] ", stopwatch.Elapsed.TotalSeconds);
		//	caller.fail(timeStamp + string.Format(msg, args));
		//}

		//private void message(string msg, params object[] args) {
		//	string timeStamp = string.Format("[{0:0000.00}] ", stopwatch.Elapsed.TotalSeconds);
		//	caller.message(timeStamp + string.Format(msg, args));
		//}


		//public override Task OnDisconnected(bool stopCalled) {
		//	clientConnected = false;
		//	return base.OnDisconnected(stopCalled);
		//}

	}
}