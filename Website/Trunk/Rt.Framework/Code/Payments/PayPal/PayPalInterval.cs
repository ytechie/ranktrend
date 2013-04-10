using System;
using System.Text;

namespace Rt.Framework.Payments.PayPal
{
	public class PayPalInterval
	{
		private int _quantity;
		private IntervalUnits _units;

		/// <summary>
		///		Creates a new instance of the <see cref="PayPalInterval"/> class.
		/// </summary>
		/// <param name="quantity"></param>
		/// <param name="units"></param>
		public PayPalInterval(int quantity, IntervalUnits units)
		{
			_quantity = quantity;
			_units = units;
		}

		/// <summary>
		///		Creates a new instance of the <see cref="PayPalInterval"/> class, parsing
		///		the specified PayPal interval string.
		/// </summary>
		/// <param name="intervalString"></param>
		public PayPalInterval(string intervalString)
		{
			string[] parts;

			parts = intervalString.Split(" ".ToCharArray());
			_quantity = int.Parse(parts[0]);

			switch (parts[1].ToUpper())
			{
				case "D":
					_units = IntervalUnits.Days;
					break;
				case "W":
					_units = IntervalUnits.Weeks;
					break;
				case "M":
					_units = IntervalUnits.Months;
					break;
				case "Y":
					_units = IntervalUnits.Years;
					break;
			}
		}

		/// <summary>
		///		The numeric portion of the interval.  For an interval
		///		of 5 days, this would be 5.
		/// </summary>
		public int Quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}

		/// <summary>
		///		The unit portion of the interval.  For an interval
		///		of 5 days, this would be the "days" part.
		/// </summary>
		public IntervalUnits Units
		{
			get { return _units; }
			set { _units = value; }
		}

		/// <summary>
		///		Gets the PayPal style interval string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder result;

			result = new StringBuilder();
			result.Append(_quantity);
			result.Append(" ");

			switch (_units)
			{
				case IntervalUnits.Days:
					result.Append("D");
					break;
				case IntervalUnits.Weeks:
					result.Append("W");
					break;
				case IntervalUnits.Months:
					result.Append("M");
					break;
				case IntervalUnits.Years:
					result.Append("Y");
					break;
			}

			return result.ToString();
		}

		/// <summary>
		///		Adds this interval to a DateTime.
		/// </summary>
		/// <param name="startTime"></param>
		/// <returns></returns>
		public DateTime AddTo(DateTime startTime)
		{
			switch (_units)
			{
				
				case IntervalUnits.Weeks:
					return startTime.AddDays(_quantity * 7);
				case IntervalUnits.Months:
					return startTime.AddMonths(_quantity);
				case IntervalUnits.Years:
					return startTime.AddYears(_quantity);
				default:
					return startTime.AddDays(_quantity);
			}
		}
	}
}