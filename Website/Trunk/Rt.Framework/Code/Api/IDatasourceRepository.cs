using Rt.Framework.Api.Model;

namespace Rt.Framework.Api
{
	//TODO: Figure out how the configuration dialog will get a list of
	//	datasource types, and the parameter information.

	/// <summary>
	///		Interface methods for working with the datasource repository.
	/// </summary>
	public interface IDatasourceRepository
	{
		/// <summary>
		///		Gets a list of configured datasources for the
		///		specified URL.
		/// </summary>
		/// <param name="url">
		///		The web address (URL) of the page to get a
		///		list of datasource for.
		/// </param>
		/// <param name="getParameters">
		///		A boolean indicating if the parameters should also
		///		be returned with each datasource.
		/// </param>
		/// <param name="types">
		///		If not-null, only datasources with these types will be included
		///		in the results.
		/// </param>
		/// <returns>
		///		An array of <see cref="Datasource"/> objects.
		/// </returns>
		Datasource[] GetDatasources(string url, bool getParameters, int[] types);

		/// <summary>
		///		Saves the datasource.
		/// </summary>
		/// <param name="datasources">
		///		A list of populated datasources to save.
		/// </param>
		void SaveDatasources(Datasource[] datasources);

		/// <summary>
		///		Deletes the datasource with the specified datasource Id.
		/// </summary>
		/// <param name="datasourceId">
		///		The ID of the datasource to delete.
		/// </param>
		void DeleteDatasource(int datasourceId);

		/// <summary>
		///		Gets an array of datasources that can be used as templates
		///		for creating datasource instances.
		/// </summary>
		/// <remarks>
		///		<para>
		///			To create a new datasource, you can use this method to get a list
		///			of unconfigured datasources. Then, populate the paramter values
		///			for one or more of the templates, and then save them with
		///			<see cref="SaveDatasources"/>. Only save the datasources that you
		///			have populated parameter values for.
		///		</para>
		/// </remarks>
		/// <returns></returns>
		Datasource[] GetDatasourceTemplates();

		/// <summary>
		///		A high performance way of replacing all of the keywords being
		///		tracked by all of the search datasources.
		/// </summary>
		/// <remarks>
		///		<para>
		///			A list of keywords can be built by using the <see cref="GetDatasources"/>
		///			method, requesting datasources that are search position datasources.
		///		</para>
		/// </remarks>
		/// <param name="url">
		///		The URL to replace the keywords for. Any keywords not in this list will
		///		have their datasources deleted, so use it with care.
		/// </param>
		/// <param name="keywords">
		///		An array of keywords (or keyphrases) that should now
		///		be used for this URL.
		/// </param>
		void ReplaceKeywords(string url, string[] keywords);
	}
}
