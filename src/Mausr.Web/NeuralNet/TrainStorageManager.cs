using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Mausr.Core.NeuralNet;
using Mausr.Web.Infrastructure;

namespace Mausr.Web.NeuralNet {
	public class TrainStorageManager {

		public const string TRAIN_SETTINGS_FILE_NAME = "TrainSettings.xml";
		public const string NET_FILE_NAME = "NeuralNetwork.bin";
		public const string TRAIN_DATA_FILE_NAME = "TrainData.bin";
		public const string DEFAULT_NET_FILE_NAME = "DefaultNetId.txt";
		public const string TRAIN_RESULT_IMAGE_NAME = "TrainResults.png";
		public const string TEST_RESULT_IMAGE_NAME = "TestResults.png";

		protected readonly AppSettingsProvider appSettings;

		private XmlSerializer trainSettingsSerializer = new XmlSerializer(typeof(TrainSettings));
		private BinaryFormatter binarySerializer = new BinaryFormatter();


		public TrainStorageManager(AppSettingsProvider appSettings) {
			this.appSettings = appSettings;
		}


		public string CreateSafeNetId(string netName) {
			return MyUrl.UrlizeString(netName);
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
				using (var stream = File.Create(Path.Combine(baseDir, TRAIN_SETTINGS_FILE_NAME))) {
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
			return save(netId, netwrok, NET_FILE_NAME);
		}

		public Net LoadNet(string netId) {
			return load<Net>(netId, NET_FILE_NAME);
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


		public bool SaveTrainData(string netId, TrainData trainData) {
			return save(netId, trainData, TRAIN_DATA_FILE_NAME);
		}

		public TrainData LoadTrainData(string netId) {
			return load<TrainData>(netId, TRAIN_DATA_FILE_NAME);
		}

		public bool DeleteNet(string netId) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return false;
			}

			try {
				Directory.Delete(baseDir, true);
				return true;
			}
			catch (Exception ex) {
				return false;
			}
		}

		
		public bool SaveTrainResultsImg(string netId, Bitmap img) {
			return saveImg(netId, img, TRAIN_RESULT_IMAGE_NAME);
		}

		public Bitmap LoadTrainResultsImg(string netId) {
			return loadImg(netId, TRAIN_RESULT_IMAGE_NAME);
		}


		public bool SaveTestResultsImg(string netId, Bitmap img) {
			return saveImg(netId, img, TEST_RESULT_IMAGE_NAME);
		}

		public Bitmap LoadTestResultsImg(string netId) {
			return loadImg(netId, TEST_RESULT_IMAGE_NAME);
		}
		


		private bool save<T>(string netId, T data, string fileName) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return false;
			}

			try {
				using (var stream = File.Create(Path.Combine(baseDir, fileName))) {
					binarySerializer.Serialize(stream, data);
					return true;
				}
			}
			catch (Exception ex) {
				return false;
			}
		}

		private T load<T>(string netId, string fileName) where T : class {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return null;
			}

			try {
				using (var stream = File.OpenRead(Path.Combine(baseDir, fileName))) {
					return binarySerializer.Deserialize(stream) as T;
				}
			}
			catch (Exception ex) {
				return null;
			}
		}

		private bool saveImg(string netId, Bitmap img, string fileName) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return false;
			}

			try {
				img.Save(Path.Combine(baseDir, fileName), ImageFormat.Png);
				return true;
			}
			catch (Exception ex) {
				return false;
			}
		}

		private Bitmap loadImg(string netId, string fileName) {
			string baseDir = getSafeBaseDirPath(netId);
			if (baseDir == null) {
				return null;
			}

			try {
				return Image.FromFile(Path.Combine(baseDir, fileName)) as Bitmap;
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