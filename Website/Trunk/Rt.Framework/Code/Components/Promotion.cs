using System;

namespace Rt.Framework.Components
{
	public class Promotion
	{
		private DateTime? _endDate;
		private int? _id;
		private string _promoCode;
		private int? _quantity;
		private DateTime _startDate;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string PromoCode
		{
			get { return _promoCode; }
			set { _promoCode = value; }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}

		public DateTime? EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}

		public int? Quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}
	}
}