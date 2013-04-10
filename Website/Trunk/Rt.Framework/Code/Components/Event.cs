using System;
using System.Drawing;

namespace Rt.Framework.Components
{
	public class Event
	{
		private EventCategory _category;
		private Color? _color;
		private string _description;
		private DateTime? _endTime;
		private string _eventLink;
		private int _id;
		private string _name;
		private DateTime _startTime;
		private UrlClass _url;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public DateTime StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		public DateTime? EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public UrlClass Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public Color? Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public int? ColorValue
		{
			get
			{
				if (_color == null)
					return null;

				return ((Color) _color).ToArgb();
			}
			set
			{
				if (value == null)
				{
					_color = null;
					return;
				}

				_color = System.Drawing.Color.FromArgb((int) value);
			}
		}

		public EventCategory Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public string EventLink
		{
			get { return _eventLink; }
			set { _eventLink = value; }
		}
	}
}