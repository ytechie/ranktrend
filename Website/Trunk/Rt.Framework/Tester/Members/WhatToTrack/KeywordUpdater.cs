using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rt.Framework.Api;
using Rt.Framework.Api.Model;

namespace Rt.Framework.Members.WhatToTrack
{
	[TestFixture]
	public class KeywordUpdater_Tester
	{
		private KeywordManager _ku;
		private MockRepository _mocks;
		private IDatasourceRepository _datasourceRepo;

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository();
			_datasourceRepo = _mocks.StrictMock<IDatasourceRepository>();
			_ku = new KeywordManager(_datasourceRepo);
		}

		[Test]
		public void Get_Keyword_List_Multiple_Datasources()
		{
			var apiResults = new List<Datasource>();
			var newDatasource = new Datasource();
			newDatasource.Parameters = new DatasourceParameter[1];
			newDatasource.Parameters[0] = new DatasourceParameter() { Value = "phrase1" };
			apiResults.Add(newDatasource);

			newDatasource = new Datasource();
			newDatasource.Parameters = new DatasourceParameter[1];
			newDatasource.Parameters[0] = new DatasourceParameter() { Value = "phrase2" };
			apiResults.Add(newDatasource);

			Expect.Call(_datasourceRepo.GetDatasources("http://test.com", true, new[] { 1, 2, 7 })).Return(apiResults.ToArray());

			_mocks.ReplayAll();

			var keywords = _ku.GetKeywordList("http://test.com");
			_mocks.VerifyAll();
			Assert.AreEqual(2, keywords.Count);
			Assert.AreEqual("phrase1", keywords[0]);
			Assert.AreEqual("phrase2", keywords[1]);
		}

		[Test]
		public void Repeated_Keywords_Verify_No_Duplicates()
		{
			var apiResults = new List<Datasource>();
			var newDatasource = new Datasource();
			newDatasource.Parameters = new DatasourceParameter[1];
			newDatasource.Parameters[0] = new DatasourceParameter() { Value = "phrase1" };
			apiResults.Add(newDatasource);

			newDatasource = new Datasource();
			newDatasource.Parameters = new DatasourceParameter[1];
			newDatasource.Parameters[0] = new DatasourceParameter() { Value = "phrase1" };
			apiResults.Add(newDatasource);

			Expect.Call(_datasourceRepo.GetDatasources("http://test.com", true, new[] { 1, 2, 7 })).Return(apiResults.ToArray());

			_mocks.ReplayAll();

			var keywords = _ku.GetKeywordList("http://test.com");
			_mocks.VerifyAll();
			Assert.AreEqual(1, keywords.Count);
			Assert.AreEqual("phrase1", keywords[0]);
		}
	}
}
