using System;
using System.Collections.Specialized;
using System.Reflection;
using log4net;
using Rt.Framework.Components.PayPal;

namespace Rt.Framework.Payments.PayPal
{
	public class IpnPostHandler
	{
		private NameValueCollection _postedValues;
		private Subscription _existingSubscription;
		private Subscription _saveSubscription;
		private DateTime _dbNow;

		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public const string PP_TRANSACTION_TYPE = "txn_type";
		public const string PP_TRANSACTION_TYPE_MODIFY = "subscr_modify";
		public const string PP_TRANSACTION_TYPE_PAYMENT = "subscr_payment";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="existingSubscription">
		///		The subscription specified in the IPN post data. This should be
		///		NULL if no subscription ID is specified in the post data.
		/// </param>
		/// <param name="postedValues">
		/// </param>
		public IpnPostHandler(NameValueCollection postedValues, Subscription existingSubscription)
		{
			_postedValues = postedValues;
			_existingSubscription = existingSubscription;
		}

		public Subscription SaveSubscription
		{
			get { return _saveSubscription; }
		}

		public void Process(DateTime dbNow)
		{
			string transactionString;

			_dbNow = dbNow;
			transactionString = _postedValues[PP_TRANSACTION_TYPE];

			_log.DebugFormat("Transaction string is '{0}'", transactionString);

			if (transactionString == PP_TRANSACTION_TYPE_MODIFY)
				processModification();
			else if (transactionString == PP_TRANSACTION_TYPE_PAYMENT)
				processPayment();
		}

		private void processModification()
		{
			_log.Debug("Modifying an existing subscription record");
			_saveSubscription = _existingSubscription;

			//Change the subscription duration and amount
			_saveSubscription.Cost = double.Parse(_postedValues["amount3"]);
			_saveSubscription.PayPalInterval = _postedValues["period3"];

			_log.DebugFormat("Changing subscription amount to {0} and duration to {1}", _saveSubscription.Cost, _saveSubscription.PayPalInterval);
		}

		private void processPayment()
		{
			renewAccount();
		}

		/// <summary>
		///		Modifies the subscription and adds the appropriate interval of
		///		time t the subscription.
		/// </summary>
		private void renewAccount()
		{
			PayPalInterval ppi;

			_saveSubscription = _existingSubscription;

			_log.DebugFormat("Renewing account for subscription '{0}'", _saveSubscription.Id);

			if (_saveSubscription.StartTime == null)
				_saveSubscription.StartTime = _dbNow;

			//Parse the interval, and add it to the subscription
			ppi = new PayPalInterval(_saveSubscription.PayPalInterval);

			_saveSubscription.EndTime = ppi.AddTo(_dbNow);
		}
	}
}
