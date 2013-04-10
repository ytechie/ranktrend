using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Rt.TrayApplication.Configuration
{
	public class Settings
	{
		private const string SETTINGS_FILE_NAME = "Settings.xml";

		private string _userName;
		private Guid _userGuid;
		private int _pollHours = 1;

		/// <summary>
		///		Gets or sets the username of the user that has
		///		entered their credentials into this tray application.
		/// </summary>
		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		/// <summary>
		///		The <see cref="Guid" /> of the user that has
		///		successfully authenticated when they set up
		///		the client.
		/// </summary>
		/// <remarks>
		///		This is cached instead of the username and password
		///		for security reasons.  They don't have to have their
		///		credentials saved on their computer.
		/// </remarks>
		public Guid UserGuid
		{
			get { return _userGuid; }
			set { _userGuid = value; }
		}

		/// <summary>
		///		The number of hours between each poll attempt of the server.
		/// </summary>
		public int PollHours
		{
			get { return _pollHours; }
			set { _pollHours = value; }
		}

		public static Settings LoadSavedSettings()
		{
			XmlSerializer deserializer;
			FileStream fs;
			Settings settings;

			if (File.Exists(getSettingsPath()))
			{
				//Load existing files
				deserializer = new XmlSerializer(typeof(Settings));
				using (fs = new FileStream(getSettingsPath(), FileMode.Open))
					settings = (Settings)deserializer.Deserialize(fs);
			}
			else
			{
				settings = null;
			}

			return settings;
		}

		public static void SaveSettings(Settings settings)
		{
			FileStream fs;
			XmlSerializer serializer;

			//Save the settings back to a file
			using (fs = new FileStream(getSettingsPath(), FileMode.Create))
			{
				serializer = new XmlSerializer(typeof(Settings));
				serializer.Serialize(fs, settings);
			}
		}

		private static string getSettingsPath()
		{
			string appDataPath;
			appDataPath = getPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RankTrend", "Tray Application");

			if (!Directory.Exists(appDataPath))
				Directory.CreateDirectory(appDataPath);

			return getPath(appDataPath, SETTINGS_FILE_NAME);
		}

		private static string getPath(params string[] folders)
		{
			string fullPath = null;
			foreach (string folder in folders)
			{
				if (fullPath == null)
					fullPath = folder;
				else
					fullPath = Path.Combine(fullPath, folder);
			}
			return fullPath;
		}
	}
}
