using YTech.General.DataMapping;

namespace Rt.Framework.Components
{
	public class EmailTemplate
	{
		private bool _html;
		private int? _id;
		private bool _locked;
		private string _message;
		private string _subject;

		#region Public Properties

		/// <summary>
		///		The unique identifier for this object in
		///		the database.  If this object was not loaded
		///		from the database, an exception will be thrown.
		/// </summary>
		[FieldMapping("Id")]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		The subject of the email.
		/// </summary>
		[FieldMapping("Subject")]
		public string Subject
		{
			get { return _subject; }
			set { _subject = value; }
		}

		/// <summary>
		///		The text or HTML that goes in the body of the email
		/// </summary>
		[FieldMapping("Message")]
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		/// <summary>
		///		If true, the email will be treated as an HTML email.  Only
		///		set this to true if you are using HTML.  Text is generally
		///		accepted better.
		/// </summary>
		[FieldMapping("Html")]
		public bool Html
		{
			get { return _html; }
			set { _html = value; }
		}

		/// <summary>
		///     Gets or sets whether or not the email template is locked.
		/// </summary>
		[FieldMapping("Locked")]
		public bool Locked
		{
			get { return _locked; }
			set { _locked = value; }
		}

		#endregion
	}
}