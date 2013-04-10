using System;

namespace Rt.Framework.Components.PayPal
{
	/// <summary>
	///		Represents a subscription to a plan
	/// </summary>
	public class Subscription
	{
		private double _cost;
		private DateTime? _endTime;
		private Guid _id;
		private string _payPalInterval;
		private Plan _plan;
		private DateTime? _startTime;
		private Guid _userId;

		#region Public Properties

		/// <summary>
		///		The unique identifier for the subscription. This
		///		will be used as the invoice identifier in PayPal.
		/// </summary>
		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		The ID of the user that this
		///		subscription is for.
		/// </summary>
		public Guid UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		/// <summary>
		///		The ID of the plan that
		///		this subscription is for.
		/// </summary>
		public Plan Plan
		{
			get { return _plan; }
			set { _plan = value; }
		}

		/// <summary>
		///		The <see cref="DateTime"/> that this subscription
		///		starts on.
		/// </summary>
		public DateTime? StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		/// <summary>
		///		The <see cref="DateTime"/> that this subscription ends on.
		/// </summary>
		public DateTime? EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public string PayPalInterval
		{
			get { return _payPalInterval; }
			set { _payPalInterval = value; }
		}

		public double Cost
		{
			get { return _cost; }
			set { _cost = value; }
		}

		#endregion
	}
}