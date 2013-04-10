using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using Rt.TrayApplication.Configuration;

namespace Rt.TrayApplication
{
	public partial class MainForm : Form
	{
		private bool _forceEnteringOptions = false;
		
		private const string BLANK_PASSWORD = "asd;lkefaasdf";

		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public event EventHandler UserCredentialsSave;
		public event EventHandler RunCycle;

		/// <summary>
		///		Creates a new instance of the <see cref="MainForm" /> class.
		/// </summary>
		/// <param name="trayContext"></param>
		public MainForm()
		{
			InitializeComponent();
		}

		private Settings Settings
		{
			get { return MainApplicationContext.Settings; }
			set { MainApplicationContext.Settings = value; }
		}

		public bool ForceEnteringOptions
		{
			get { return _forceEnteringOptions; }
			set { _forceEnteringOptions = value; }
		}

		#region Load/Save Settings

		/// <summary>
		///		Populates the form with the settings stored in the
		///		specified <see cref="Settings"/> class.
		/// </summary>
		private void populateForm()
		{
			txtPollHours.Text = Settings.PollHours.ToString();
			txtUsername.Text = Settings.UserName;

			//Load the default password so we can tell when it changes
			txtPassword.Text = BLANK_PASSWORD;
		}

		private bool saveSettings()
		{
			int newPollHours;
			
			if (!int.TryParse(txtPollHours.Text, out newPollHours))
			{
				MessageBox.Show("Invalid value for the number of hours between each poll of the server",
					MainApplicationContext.APP_NAME + " - Invalid Poll Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				return false;
			}

			Settings.PollHours = newPollHours;

			if (txtUsername.Text.Length == 0)
			{
				MessageBox.Show("You have entered invalid credentials",
					MainApplicationContext.APP_NAME + " - Invalid Credentials", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				return false;
			}

			if (Settings.UserName != txtUsername.Text || txtPassword.Text != BLANK_PASSWORD)
				if (!changeCredentials())
					return false;

			Configuration.Settings.SaveSettings(Settings);

			return true;
		}

		private bool changeCredentials()
		{
			Webservices.TrayApplication ws;
			Guid guid;
			string guidString;

			if (txtPassword.Text == BLANK_PASSWORD)
			{
				MainApplicationContext.ShowMessageBox("If you change your user name, you need to re-enter your password. Your user name has not been changed.");
				return false;
			}

			//The username or password has changed, so we need to get a new GUID
			ws = new Webservices.TrayApplication();

			Cursor = Cursors.WaitCursor;
			try
			{
				guidString = ws.Authenticate(txtUsername.Text, txtPassword.Text);
			}
			finally
			{
				Cursor = Cursors.Default;
			}

			if (guidString == null)
			{
				MainApplicationContext.ShowMessageBox("The user name and password you entered are not correct.");
				return false;
			}

			guid = new Guid(guidString);
			
			Settings.UserName = txtUsername.Text;
			Settings.UserGuid = guid;

			return true;
		}

		#endregion

		#region Form Events

		private void cmdSave_Click(object sender, EventArgs e)
		{
			cmdSave.Enabled = false;

			if (!saveSettings())
			{
				cmdSave.Enabled = true;
				return;
			}
			else
				MainApplicationContext.ShowMessageBox("Your credentials have been successfully saved.");
			
			Hide();

			//Re-enabled the "Save" button
			cmdSave.Enabled = true;

			OnUserCredentialsSave(new EventArgs());
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			DialogResult dr = DialogResult.OK;

			if(_forceEnteringOptions)
				dr = MessageBox.Show("You have not yet configured your options. This application will not work until you do.", MainApplicationContext.APP_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

			if (_forceEnteringOptions && dr == DialogResult.OK)
				Application.ExitThread();
			else if(_forceEnteringOptions)
				Hide();
		}

		private void mainForm_Closing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
#if(DEBUG)
			cmdRunCycle.Visible = true;
#endif

			if (Settings == null)
			{
				_log.Info("Saved options could not be found, so a new set with default options is being created");
				Settings = new Configuration.Settings();
				ForceEnteringOptions = true;
			}
			populateForm();
		}

		#endregion

		protected void OnUserCredentialsSave(EventArgs e)
		{
			EventHandler hnd = UserCredentialsSave;
			if (hnd != null) hnd(this, e);
		}

		private void cmdRunCycle_Click(object sender, EventArgs e)
		{
			EventHandler eh;

			eh = RunCycle;
			if (eh != null)
				eh(sender, e);
		}
	}
}