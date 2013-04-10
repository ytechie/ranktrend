using System;
using Rt.Framework.Components;

namespace Rt.Framework.Db.Exceptions
{
	/// <summary>
	/// Used to signal that a <see cref="GlobalSetting"/> could not be found.
	/// </summary>
	public class GlobalSettingNotFound : ApplicationException
	{
		private int _globalSettingId;

		/// <summary>
		///		Creates a new instance of the <see cref="GlobalSettingNotFound"/> exception.
		/// </summary>
		/// <param name="globalSettingId">
		///		The unique identifier of the global setting that could not be found.
		/// </param>
		public GlobalSettingNotFound(int globalSettingId)
		{
			_globalSettingId = globalSettingId;
		}
	}
}