using System;
using NHibernate.Collection.Generic;

namespace Rt.Framework.Components
{
	public class CustomReport
	{
		private DateTime _created;
		private string _description;
		private int _emailIntervalDays;
		private int? _id;
		private DateTime? _lastEmailed;
		private DateTime _lastSaved;
		private string _name;
		private PersistentGenericSet<CustomReportComponent> _reportComponents;
		private Guid _userId;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Guid UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public int EmailIntervalDays
		{
			get { return _emailIntervalDays; }
			set { _emailIntervalDays = value; }
		}

		public DateTime? LastEmailed
		{
			get { return _lastEmailed; }
			set { _lastEmailed = value; }
		}

		public DateTime Created
		{
			get { return _created; }
			set { _created = value; }
		}

		public DateTime LastSaved
		{
			get { return _lastSaved; }
			set { _lastSaved = value; }
		}

		public PersistentGenericSet<CustomReportComponent> ReportComponents
		{
			get { return _reportComponents; }
			set { _reportComponents = value; }
		}
	}
}