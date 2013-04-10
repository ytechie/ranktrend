if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetOverviewForEditor]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetOverviewForEditor]
Go

CREATE PROCEDURE dbo.[Ds_GetOverviewForEditor]
	@UrlId Int
AS

/*

Summary: Looks up the information required for the datasource editor page.  Be careful
	when making modifications since the datasource editor page requires a specific
	format of the data returned by this SP.

*/

--Get all the datasources with no parameters
Select dt.Id, Case When cd.Id Is Null Then 0 Else cd.Enabled End Enabled, dt.Description
From DatasourceTypes dt
Left Outer Join ConfiguredDatasources cd On (dt.Id = cd.DatasourceTypeId And cd.UrlId = @UrlId)
--Only get the datasources without parameters
Where (Select Count(*) From DatasourceParameterTypes dpt Where dpt.DatasourceTypeId = dt.Id) = 0
--Don't include generic datasources
And dt.Id <> 8

--Get the search terms
Select Distinct dsp.TextValue SearchPhrase
From ConfiguredDatasources cd
Join DatasourceParameters dsp On dsp.ConfiguredDatasourceId = cd.Id
Where cd.UrlId = @UrlId
--Only get the search datasource types
And DatasourceTypeId In (1, 2, 7)
--Parameter #1 is the search phrase
And dsp.ParameterNumber = 1

--Get the generic datasources
Select cd.[Name] GenericDsName
From ConfiguredDatasources cd
Where cd.UrlId = @UrlId
--Only get the generic datasource types
And DatasourceTypeId = 8

--Get the digg data sources
Select dsp.TextValue DiggUrl
From ConfiguredDatasources cd
Join DatasourceParameters dsp On dsp.ConfiguredDatasourceId = cd.Id
Where cd.UrlId = @UrlId
--Only get the search datasource types
And DatasourceTypeId = 9
--Parameter #1 is the digg url
And dsp.ParameterNumber = 1

Go

GRANT EXECUTE ON dbo.[Ds_GetOverviewForEditor] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters
select * from datasourcetypes
select * from datasourceparametertypes

[Ds_GetOverviewForEditor] 1
[Ds_GetOverviewForEditor] 2
[Ds_GetOverviewForEditor] 3

*/

