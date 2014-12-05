using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Mausr.Core.NeuralNet;
using Mausr.Web.Models;

namespace Mausr.Web.Infrastructure {
	public class TrainStorageManager {

		public const string TRAIN_SETTINGS_FILE_NAME = "TrainSettings.xml";
		public const string NET_FILE_NAME = "NeuralNetwork.bin";
		public const string DEFAULT_NET_FILE_NAME = "DefaultNetId.txt";

		protected readonly AppSettingsProvider appSettings;

		private XmlSerializer trainSettingsSerializer = new XmlSerializer(typeof(TrainSettings));
		private BinaryFormatter netSerializer = new BinaryFormatter();


		public TrainStorageManager(AppSettingsProvider appSettings) {
			this.appSettings = appSettings;
		}


		public string CreateSafeNetId(string netName) {
			return MyUrl.UrlizeString(DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss-") + netName);
		}

		public IEnumerable<string> LoadAllSavedNets() {
			var dirSeps = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
			return Directory.EnumerateDirectories(appSettings.NetTrainDataDirAbsolute)
				.Select(x => x.Split(dirSeps, StringSplitOptions.RemoveEmptyEntries).Last());
		}


		public bool SaveTrainSettings(string netId, TrainSettings settings) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return false;
			}

			try {
				using (var stream = File.OpenWrite(Path.Combine(baseDir, TRAIN_SETTINGS_FILE_NAME))) {
					trainSettingsSerializer.Serialize(stream, settings);
					return true;
				}
			}
			catch (Exception ex) {
				return false;
			}
		}

		public TrainSettings LoadTrainSettings(string netId) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return null;
			}

			try {
				using (var stream = File.OpenRead(Path.Combine(baseDir, TRAIN_SETTINGS_FILE_NAME))) {
					return trainSettingsSerializer.Deserialize(stream) as TrainSettings;
				}
			}
			catch (Exception ex) {
				return null;
			}
		}


		public bool SaveNet(string netId, Net netwrok) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return false;
			}

			try {
				using (var stream = File.OpenWrite(Path.Combine(baseDir, NET_FILE_NAME))) {
					netSerializer.Serialize(stream, netwrok);
					return true;
				}
			}
			catch (Exception ex) {
				return false;
			}
		}

		public Net LoadNet(string netId) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return null;
			}

			try {
				using (var stream = File.OpenRead(Path.Combine(baseDir, NET_FILE_NAME))) {
					return netSerializer.Deserialize(stream) as Net;
				}
			}
			catch (Exception ex) {
				return null;
			}
		}


		public bool SaveDefaultNetName(string netId) {
			try {
				File.WriteAllText(Path.Combine(appSettings.NetTrainDataDirAbsolute, DEFAULT_NET_FILE_NAME), netId);
				return true;
			}
			catch (Exception ex) {
				return false;
			}
		}

		public string LoadDefaultNetName() {
			try {
				return File.ReadAllText(Path.Combine(appSettings.NetTrainDataDirAbsolute, DEFAULT_NET_FILE_NAME));
			}
			catch (Exception ex) {
				return null;
			}
		}

		private string getSafeBaseDirPath(string netId) {
			string baseDir = appSettings.NetTrainDataDirAbsolute;
			string dirPath = Path.Combine(baseDir, netId);

			dirPath = Path.GetFullPath(dirPath);  // Canonicalize the path.

			if (dirPath.StartsWith(baseDir)) {
				if (!Directory.Exists(dirPath)) {
					Directory.CreateDirectory(dirPath);
				}
				return dirPath;
			}
			else {
				return null;  // Invalid path - escaped from the root dir.
			}

		}

	}
}