using System;

namespace Rt.Framework.Components.Leads
{
	public class LeadSource
	{
		private double? _cost;
		private string _description;
		private int? _estimatedDistribution;
		private int _hitCount;
		private int? _id;
		private LeadType _leadType;
		private string _redirectPageName;
		private string _redirectUrl;
		private DateTime? _runTime;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public int HitCount
		{
			get { return _hitCount; }
			set { _hitCount = value; }
		}

		public int? EstimatedDistribution
		{
			get { return _estimatedDistribution; }
			set { _estimatedDistribution = value; }
		}

		public double? Cost
		{
			get { return _cost; }
			set { _cost = value; }
		}

		public DateTime? RunTime
		{
			get { return _runTime; }
			set { _runTime = value; }
		}

		public LeadType LeadType
		{
			get { return _leadType; }
			set { _leadType = value; }
		}

		public string RedirectPageName
		{
			get { return _redirectPageName; }
			set { _redirectPageName = value; }
		}

		public string RedirectUrl
		{
			get { return _redirectUrl; }
			set { _redirectUrl = value; }
		}
	}
}