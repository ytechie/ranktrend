using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Rt.Framework.Components;

namespace Rt.Framework.Services.DataEngine
{
	/// <summary>
	///		This interface defines a class that can retreive
	///		a particular form of data from a remote service.
	/// </summary>
	/// <remarks>
	///		This class is designed so that a remote proxy application
	///		can call a webservice and maked queued requests. Since
	///		webservices connect and disconnect for each request, multiple
	///		requests need to be made, thus the reason for the queueing
	///		mechanism.
	/// </remarks>
	public interface IDataSource
	{
		/// <summary>
		///		Gets the next queued request that should be executed
		/// </summary>
		/// <returns></returns>
		SerializableWebRequest GetNextRequest();

		/// <summary>
		///		The code that executed the request should use this
		///		method to report back the results.
		/// </summary>
		/// <param name="response"></param>
		void SetResponse(SerializableWebResponse response);

		RawDataValue[] Values { get;}

		/// <summary>
		///		Gets and sets the list of parameters that this
		///		DataSource needs.
		/// </summary>
		Dictionary<int, object> Parameters{get;set;}

		string Url { get; set;}
	}
}
