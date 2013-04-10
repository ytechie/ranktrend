using System;
using NUnit.Framework;

namespace Rt.Framework.Payments.PayPal
{
	[TestFixture]
	public class PaymentCustomField_Tester
	{
		private PaymentCustomField pcf;

		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void TestToString()
		{
			Guid newGuid;

			newGuid = Guid.NewGuid();

			pcf = new PaymentCustomField();
			pcf.SubscriptionId = newGuid;
			pcf.DigitalSignature = "3afea";

			Assert.AreEqual("3afea|" + newGuid, pcf.ToString());
		}

		[Test]
		public void TestParse()
		{
			Guid newGuid;

			newGuid = Guid.NewGuid();

			pcf = new PaymentCustomField("f3as|" + newGuid);

			Assert.AreEqual("f3as", pcf.DigitalSignature);
			Assert.AreEqual(newGuid, pcf.SubscriptionId);
		}
	}
}
