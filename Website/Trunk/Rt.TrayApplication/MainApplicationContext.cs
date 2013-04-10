using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Timer=System.Threading.Timer;
using log4net;
using System.Reflection;
using System.Diagnostics;

namespace Rt.TrayApplication
{
	public class MainApplicationContext : ApplicationContext
	{
		public const string APP_NAME = "RankTrend Tray";

		private static MainForm settingsForm;

		private static Configuration.Settings _settings;
		Runner _runner;

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///		Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">
		///		true if managed resources should be disposed; otherwise, false.
		///	</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			_log.Debug("Initializing controls");

			ComponentResourceManager mainFormResources = new ComponentResourceManager(typeof(MainForm));
			components = new Container();
			separator1 = new ToolStripSeparator();
			exitItem = new ToolStripMenuItem();
			settingsItem = new ToolStripMenuItem();
			rankTrendItem = new ToolStripMenuItem();
			menu = new ContextMenuStrip(components);
			trayIcon = new RankTrendNotifyIcon(components);
			//
			// exitItem
			//
			exitItem.Text = "Exit";
			exitItem.Size = new Size(136, 22);
			exitItem.Click += new EventHandler(exitItem_Click);
			//
			// separator1
			//
			separator1.Size = new Size(133, 6);
			//
			// settingsItem
			//			
			settingsItem.Text = "Settings";
			settingsItem.Size = new Size(136, 22);
			settingsItem.Click += new EventHandler(settingsItem_Click);
			//
			// rankTrendItem
			//			
			rankTrendItem.Text = "RankTrend.com";
			rankTrendItem.Font = new Font(rankTrendItem.Font, FontStyle.Bold);
			rankTrendItem.Size = new Size(136, 22);
			rankTrendItem.Click += new EventHandler(rankTrendItem_Click);
			//
			// menu
			//
			menu.Items.AddRange(new ToolStripItem[]{
				rankTrendItem,
				settingsItem,
				separator1,
				exitItem});
			// 
			// trayIcon
			//
			trayIcon.Icon = ((System.Drawing.Icon)(mainFormResources.GetObject("$this.Icon")));
			trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);
			trayIcon.ContextMenuStrip = menu;
			trayIcon.Visible = true;
		}

		ContextMenuStrip menu;
		ToolStripItem exitItem;
		ToolStripItem settingsItem;
		ToolStripItem rankTrendItem;
		ToolStripSeparator separator1;
		private RankTrendNotifyIcon trayIcon;
		#endregion

		public MainApplicationContext()
		{
			_log.Info("Starting main application context");
			InitializeComponent();

			if (settingsForm == null)
			{
				_log.Debug("Creating a new instance of the settings form");
				settingsForm = new MainForm();
				settingsForm.UserCredentialsSave += new EventHandler(settingsForm_UserCredentialsSave);
				settingsForm.RunCycle += new EventHandler(settingsForm_RunCycle);
			}

			trayIcon.Status = "Idle";
			checkVersion();
			loadSettings();
			_runner = new Runner(_settings, trayIcon);
		}

		void settingsForm_RunCycle(object sender, EventArgs e)
		{
			_runner.RunCycle();
		}

		public static Configuration.Settings Settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public static void ShowMessageBox(string message)
		{
			MessageBox.Show(message, APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		#region Event Handlers

		void settingsItem_Click(object sender, EventArgs e)
		{
			settingsForm.Show();
		}

		void rankTrendItem_Click(object sender, EventArgs e)
		{
			launchRankTrend();
		}

		void exitItem_Click(object sender, EventArgs e)
		{
			_log.Debug("User clicked 'Exit' on the tray icon");
			Application.ExitThread();
		}

		void trayIcon_DoubleClick(object sender, EventArgs e)
		{
			_log.Debug("User double clicked on the tray icon, opening settings form");
			launchRankTrend();
		}

		void settingsForm_UserCredentialsSave(object sender, EventArgs e)
		{
			_runner.Dispose();
			_runner = new Runner(Settings, trayIcon);
		}

		#endregion
		#region Private Functions

		private void checkVersion()
		{
			Webservices.TrayApplication ws;
			string minVersionString;
			Version minVersion;

			ws = new Webservices.TrayApplication();
			minVersionString = ws.GetMinimumClientVersion();
			minVersion = new Version(minVersionString);

			if (minVersion.CompareTo(new Version(Application.ProductVersion)) > 0)
			{
				MainApplicationContext.ShowMessageBox("The version of the RankTrend.com tray application that you are using is out of date, and must be upgraded, you will now be taken to the upgrade page.");
				System.Diagnostics.Process.Start("http://www.RankTrend.com/Members/Tray-Application/");
				Application.ExitThread();
			}
		}

		private void loadSettings()
		{
			_settings = Configuration.Settings.LoadSavedSettings();

			if (Settings == null)
			{
				_log.Info("The user has not yet entered their credentials, so they will be prompted");
				MainApplicationContext.ShowMessageBox("You need to enter your credentials.");

				//We need the main form to create the initial options
				settingsForm.Show();
			}
		}

		private void launchRankTrend()
		{
			Process proc = new Process();
			proc.StartInfo.FileName = "http://www.RankTrend.com/Members/";
			proc.Start();
		}

		#endregion
	}
}
