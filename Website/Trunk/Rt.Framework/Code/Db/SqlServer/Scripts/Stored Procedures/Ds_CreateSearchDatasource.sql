if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_CreateSearchDatasource]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_CreateSearchDatasource]
Go

CREATE PROCEDURE dbo.[Ds_CreateSearchDatasource]
	@UrlId Int,
	@DatasourceTypeId Int,
	@Keyword Varchar(Max)
AS

Insert Into ConfiguredDatasources
(UrlId, DatasourceTypeId, [Name])
Select @UrlId, @DatasourceTypeId, dt.Description + ' (' + @Keyword + ')'
From DatasourceTypes dt
Where dt.Id = @DatasourceTypeId

Insert Into DatasourceParameters
(ParameterNumber, TextValue, ConfiguredDatasourceId)
Select 1, @Keyword, Scope_Identity()

Go

GRANT EXECUTE ON dbo.[Ds_CreateSearchDatasource] TO [Public]
Go

/*

Exec [Ds_CreateSearchDatasource] 1, 1, 'test'

*/