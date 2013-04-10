if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Keywords_GetKeywordList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Keywords_GetKeywordList]
Go

CREATE PROCEDURE dbo.[Keywords_GetKeywordList]
	@SiteId Int,
	@DatasourceTypeId Int
AS

Select Distinct(dsp.TextValue)
From ConfiguredDatasources cd
Join DatasourceParameters dsp On dsp.ConfiguredDatasourceId = cd.Id
--This list should include all search position datasources
--Eventually this should be looked up somewhere.
Where cd.DatasourceTypeId In (1, 2, 7)
And (@SiteId Is Null Or UrlId = @SiteId)
And (@DatasourceTypeId Is Null Or cd.DatasourceTypeId = @DatasourceTypeId)
And dsp.TextValue Is Not Null

Go

GRANT EXECUTE ON dbo.[Keywords_GetKeywordList] TO [Public]
Go

/*

select * from configureddatasources
select * from datasourceparameters
select * from datasourcetypes
select * from urls

[Keywords_GetKeywordList] 152, 1

*/