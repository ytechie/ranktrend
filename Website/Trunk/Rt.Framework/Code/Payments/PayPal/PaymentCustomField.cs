using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace Rt.Framework.Payments.PayPal
{
	/// <summary>
	///		This class is used to parse and write the custom field
	///		that is associated with PayPal subscriptions. This allows
	///		us to store the subscription ID so that it can be looked up.
	///		It also stores a digital signature that is used to verify the
	///		authenticity of the transaction.
	/// </summary>
	/// <remarks>
	///		Format: {signature}|{subscriptionId}
	/// </remarks>
	public class PaymentCustomField
	{
		private Guid _subscriptionId;
		private string _digitalSignature;

		public Guid SubscriptionId
		{
			get { return _subscriptionId; }
			set{ _subscriptionId = value; }
		}

		public string DigitalSignature
		{
			get { return _digitalSignature; }
			set{ _digitalSignature = value; }
		}

		public PaymentCustomField()
		{
		}

		public PaymentCustomField(string customField)
		{
			string[] parts;

			parts = customField.Split('|');
			_digitalSignature = parts[0];
			_subscriptionId = new Guid(parts[1]);
		}

		public override string ToString()
		{
			return string.Format("{0}|{1}", _digitalSignature, _subscriptionId);
		}

		#region Digial Signature

		public static string GenerateDigitalSignature(NameValueCollection formData)
		{
			StringBuilder sig;

			sig = new StringBuilder();

			//Append the begin salt
			sig.Append("aofiejao3aF$Qawefao;ije--");

			sig.Append(formData["amount1"]);
			sig.Append(formData["amount2"]);
			sig.Append(formData["amount3"]);
			sig.Append(formData["period1"]);
			sig.Append(formData["period2"]);
			sig.Append(formData["period3"]);
			sig.Append(formData["item_number"]);

			//Append the end salt
			sig.Append("--aeiirQGqfaoijflkn$Tq4wmn;lkas");

			return md5Hash(sig.ToString());
		}

		private static string md5Hash(string stringToHash)
		{
			ASCIIEncoding encoding;
			byte[] hashValue;
			byte[] messageBytes;
			StringBuilder hexString;

			encoding = new ASCIIEncoding();
			messageBytes = encoding.GetBytes(stringToHash);
			MD5 md5 = new MD5CryptoServiceProvider();

			hashValue = md5.ComputeHash(messageBytes);

			hexString = new StringBuilder();
			foreach (byte b in hashValue)
				hexString.AppendFormat("{0:x2}", b);

			return hexString.ToString();
		}

		#endregion
	}
}
