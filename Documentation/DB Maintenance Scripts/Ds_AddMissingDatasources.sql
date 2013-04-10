/*
Right now this is a prototype for adding datasources that are not yet configured
*/

Declare @UrlId Int

Set @UrlId = 4

Insert Into ConfiguredDatasources
(UrlId, DatasourceTypeId, CheckFrequencyDays, Enabled, [Name], Description)
Select @UrlId, dt.Id, 1, 1, dt.Description, ''
From DatasourceTypes dt
--Find missing data source types
Where dt.Id Not In (Select DatasourceTypeId
										From ConfiguredDatasources
										Where UrlId = @UrlId)
--Only find those that take no parameters
And dt.Id Not In (Select DatasourceTypeId
									From DatasourceParameterTypes)
--Don't get disabled datasource types
And Enabled = 1

/*

select * from configureddatasources
select * from url
select * from datasourceparametertypes
select * from datasourcetypes

*/