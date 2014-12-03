using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Mausr.Web.Models;

namespace Mausr.Web.Infrastructure {
	public class TrainStorageManager {

		public const string TRAIN_SETTINGS_FILE_NAME = "TrainSettings.xml";

		protected readonly AppSettingsProvider appSettings;

		private XmlSerializer trainSettingsSerializer = new XmlSerializer(typeof(TrainSettings));


		public TrainStorageManager(AppSettingsProvider appSettings) {
			this.appSettings = appSettings;
		}


		public string CreateSafeNetName(string netName) {
			return MyUrl.UrlizeString(netName + DateTime.UtcNow.ToString("-yyyy-MM-dd-hh-mm-ss"));
		}
		

		public bool SaveTrainSettings(string netName, TrainSettings settings) {

			string baseDir = appSettings.NetTrainDataDirAbsolute;
			string settingsFilePath = Path.Combine(baseDir, netName, TRAIN_SETTINGS_FILE_NAME);

			settingsFilePath = Path.GetFullPath(settingsFilePath);  // Canonicalize the path.

			if (!settingsFilePath.StartsWith(baseDir)) {
				return false;  // Invalid path - escaped from the root dir.
			}

			try {
				Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));
				using (var stream = File.OpenWrite(settingsFilePath)) {
					trainSettingsSerializer.Serialize(stream, settings);
					return true;
				}
			}
			catch (Exception ex) {
				return false;
			}
		}

		public TrainSettings LoadTrainSettings(string netName) {
			string baseDir = appSettings.NetTrainDataDirAbsolute;
			string settingsFilePath = Path.Combine(baseDir, netName, TRAIN_SETTINGS_FILE_NAME);

			settingsFilePath = Path.GetFullPath(settingsFilePath);  // Canonicalize the path.

			if (!settingsFilePath.StartsWith(baseDir)) {
				return null;  // Invalid path - escaped from the root dir.
			}

			try {
				using (var stream = File.OpenRead(settingsFilePath)) {
					return trainSettingsSerializer.Deserialize(stream) as TrainSettings;
				}
			}
			catch (Exception ex) {
				return null;
			}
		}

	}
}