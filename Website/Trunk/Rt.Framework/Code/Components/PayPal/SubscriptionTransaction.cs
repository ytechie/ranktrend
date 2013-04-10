//using System;

//namespace Rt.Framework.Components.PayPal
//{
//  /// <summary>
//  ///		A transaction that represents a pending action to renew/create,
//  ///		modify, or cancel a subscription.
//  /// </summary>
//  /// <remarks>
//  ///		Transactions can be repeated if a payment is made for the same transaction.
//  ///		In that case, the transaction should be copied.
//  /// </remarks>
//  public class SubscriptionTransaction
//  {
//    private Guid _guidId;
//    private int _siteId;
//    private int _planId;
//    private bool _processed;
//    private double _paymentAmount;
//    private string _paymentInterval;

//    #region Public Properties

//    /// <summary>
//    ///		The unique GUID that represents this transaction.
//    /// </summary>
//    /// <remarks>
//    ///		A <see cref="Guid"/> was used instead of a standard
//    ///		ID to reduce tampering when it's passed to PayPal.
//    /// </remarks>
//    public Guid GuidId
//    {
//      get { return _guidId; }
//      set { _guidId = value; }
//    }

//    /// <summary>
//    ///		The ID of the site that this transaction should
//    ///		be processed for.
//    /// </summary>
//    public int SiteId
//    {
//      get { return _siteId; }
//      set { _siteId = value; }
//    }

//    /// <summary>
//    ///		The plan that this transaction is for.
//    /// </summary>
//    public int PlanId
//    {
//      get { return _planId; }
//      set { _planId = value; }
//    }

//    /// <summary>
//    ///		If true, the transaction has been processed and
//    ///		is basically only kept as an audit trail in the
//    ///		system.
//    /// </summary>
//    public bool Processed
//    {
//      get { return _processed; }
//      set { _processed = value; }
//    }

//    /// <summary>
//    ///		The dollar amount of the payment.
//    /// </summary>
//    public double PaymentAmount
//    {
//      get { return _paymentAmount; }
//      set { _paymentAmount = value; }
//    }

//    /// <summary>
//    ///		The PayPal style interval string.
//    /// </summary>
//    /// <remarks>
//    ///		Examples: 1 D, 4 W, 2 M, 5 Y
//    /// </remarks>
//    public string PaymentInterval
//    {
//      get { return _paymentInterval; }
//      set { _paymentInterval = value; }
//    }

//    #endregion

//    /// <summary>
//    ///		The different types of subscription transactions that are possible.
//    /// </summary>
//    public enum SubscriptionTransactionTypes
//    {
//      Modify = 1,

//      /// <summary>Create a new subscription or renew one that exists</summary>
//      CreateRenew = 2,

//      /// <summary>Cancel an existing subscription</summary>
//      Cancel = 3
//    }
//  }
//}
