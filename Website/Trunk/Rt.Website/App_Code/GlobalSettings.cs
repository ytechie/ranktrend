using Rt.Framework.Components;

namespace Rt.Website
{
	/// <summary>
	/// Summary description for GlobalSettings
	/// </summary>
	public class GlobalSettings
	{
		public static int VerifyUserEmail
		{
			get { return (int) getGlobalSetting(1).IntValue; }
		}

		public static string AdministrativeEmail
		{
			get { return getGlobalSetting(2).TextValue; }
		}

		public static int SignUpTermsOfService
		{
			get { return (int) getGlobalSetting(3).IntValue; }
		}

		public static int? TrayApplicationLicenseAgreement
		{
			get { return getGlobalSetting(7).IntValue; }
		}

		public static string FogBugzSupportEmail
		{
			get { return getGlobalSetting(11).TextValue; }
		}

		public static int? CancelAccountEmail
		{
			get { return getGlobalSetting(12).IntValue; }
		}

		public static int? WelcomeEmail
		{
			get { return getGlobalSetting(13).IntValue; }
		}

		private static GlobalSetting getGlobalSetting(int id)
		{
			return Global.GetDbConnection().GetGlobalSetting(id);
		}
	}
}