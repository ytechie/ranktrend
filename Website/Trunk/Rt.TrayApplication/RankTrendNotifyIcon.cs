using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using log4net;
using System.Reflection;
using System.Drawing;

namespace Rt.TrayApplication
{
	class RankTrendNotifyIcon : UserControl, IStatus, IDisposable
	{
		/// <summary>
		///		Declare and create our logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private IContainer components = null;
		private NotifyIcon _notifyIcon;

		private string _status;

		public RankTrendNotifyIcon(IContainer container)
		{
			container.Add(this, Name);
			components = new Container();

			InitializeComponent();
		}

		public override ContextMenuStrip ContextMenuStrip
		{
			get { return _notifyIcon.ContextMenuStrip; }
			set { _notifyIcon.ContextMenuStrip = value; }
		}

		public Icon Icon
		{
			get { return _notifyIcon.Icon; }
			set { _notifyIcon.Icon = value; }
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				string message = string.Format("{0} - {1}", MainApplicationContext.APP_NAME, _status);
				_notifyIcon.Text = message;
				_log.DebugFormat("Status message set to '{0}'", message);
			}
		}

		public new bool Visible
		{
			get { return _notifyIcon.Visible; }
			set { _notifyIcon.Visible = value; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			_notifyIcon = new NotifyIcon(components);
			//
			// notifyIcon
			//
			_notifyIcon.DoubleClick += new EventHandler(_notifyIcon_DoubleClick);
		}

		void _notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			OnDoubleClick(e);
		}
	}
}
