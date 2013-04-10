if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetPageDatasourceList]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetPageDatasourceList]
Go

CREATE PROCEDURE dbo.[Ds_GetPageDatasourceList]
	@UrlId Int,
	@IncludeSubtypes Bit = 0
AS

/*

Summary: Gets a list of datasources by the specified page Id.  This is
	used to populate the configured datasource lists.

*/

Select cd.Id, st.Id SubTypeId, dbo.Ds_DatasourceDisplayName(cd.Id, Null) DisplayName
From ConfiguredDatasources cd
Left Outer Join DatasourceSubtypes st On @IncludeSubtypes = 1 And cd.DatasourceTypeId = st.DatasourceTypeId
Where cd.UrlId = @UrlId

Go

GRANT EXECUTE ON dbo.[Ds_GetPageDatasourceList] TO [Public]
Go

/*

select * from urls

Ds_GetPageDatasourceList 1
Ds_GetPageDatasourceList 3
Ds_GetPageDatasourceList 3, 1
select * from datasourcesubtypes

*/

