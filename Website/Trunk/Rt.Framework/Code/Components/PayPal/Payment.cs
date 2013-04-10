//using System;

//namespace Rt.Framework.Components.PayPal
//{
//  public class Payment
//  {
//    private double _amount;
//    private bool _applied;
//    private string _postData;
//    private Guid _subscriptionTransactionId;
//    private string _payPal_SubscriptionId;
//    private string _payPal_VerifySign;
//    private string _payPal_TransactionId;
//    private string _payPal_PayerId;
//    private double _payPal_Fee;
//    private string _payPal_PayerEmail;

//    /// <summary>
//    ///		The amount of money they payment is for.
//    /// </summary>
//    public double Amount
//    {
//      get { return _amount; }
//      set { _amount = value; }
//    }

//    /// <summary>
//    ///		If true, the payment has been processed and applied
//    ///		to the action that it was supposed to be for.
//    /// </summary>
//    public bool Applied
//    {
//      get { return _applied; }
//      set { _applied = value; }
//    }

//    /// <summary>
//    ///		The name value pairs that were recieved from
//    ///		the payment system post.  Right now, this would
//    ///		just be the PayPal IPN post.
//    /// </summary>
//    public string PostData
//    {
//      get { return _postData; }
//      set { _postData = value; }
//    }

//    /// <summary>
//    ///		Gets or sets the <see cref="Guid"/> of the 
//    ///		<see cref="SubscriptionTransaction"/> that this payment is associated with, if any.
//    /// </summary>
//    /// <remarks>
//    ///		Right now, this will always be set because payments are only made for
//    ///		subscriptions using subscription transactions.
//    /// </remarks>
//    public Guid SubscriptionTransactionId
//    {
//      get { return _subscriptionTransactionId; }
//      set { _subscriptionTransactionId = value; }
//    }

//    /// <summary>
//    ///		The subscription Id from the paypal transaction.
//    /// </summary>
//    public string PayPal_SubscriptionId
//    {
//      get { return _payPal_SubscriptionId; }
//      set { _payPal_SubscriptionId = value; }
//    }

//    /// <summary>
//    ///		The PayPal verification signature of the payment.
//    /// </summary>
//    public string PayPal_VerifySign
//    {
//      get { return _payPal_VerifySign; }
//      set { _payPal_VerifySign = value; }
//    }

//    /// <summary>
//    ///		The PayPal transaction Id of the payment.
//    /// </summary>
//    public string PayPal_TransactionId
//    {
//      get { return _payPal_TransactionId; }
//      set { _payPal_TransactionId = value; }
//    }

//    /// <summary>
//    ///		The PayPal payer Id of the person sending the payment.
//    /// </summary>
//    public string PayPal_PayerId
//    {
//      get { return _payPal_PayerId; }
//      set { _payPal_PayerId = value; }
//    }

//    /// <summary>
//    ///		The fee that PayPal is charging for this transaction.
//    /// </summary>
//    public double PayPal_Fee
//    {
//      get { return _payPal_Fee; }
//      set { _payPal_Fee = value; }
//    }

//    /// <summary>
//    ///		The email of the person making the payment.
//    /// </summary>
//    public string PayPal_PayerEmail
//    {
//      get { return _payPal_PayerEmail; }
//      set { _payPal_PayerEmail = value; }
//    }
//  }
//}
