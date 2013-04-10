if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Api_GetDatasources]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Api_GetDatasources]
Go

CREATE PROCEDURE dbo.[Api_GetDatasources]
	@Url Varchar(Max),
	@GetParameters Bit,
	@TypeString Varchar(Max)
AS

If(@TypeString Is Not Null)
Begin
	Declare @TypeTable Table (p Int)

	--Note: This way of parsing the type string list is not scalable, but
	--		the type table should never be large, so it's a non-issue.
	Set @TypeString = ',' + @TypeString + ','

	Insert Into @TypeTable
	Select Id--, Patindex('%,' + Cast(Id As Varchar(20)) + ',%', ',1,2,3,11,10,')
	From DatasourceTypes
	Where Patindex('%,' + Cast(Id As Varchar(20)) + ',%', @TypeString) > 0

--	--Convert the type string into XML
--	Declare @TypeXml Xml
--	Set @TypeXml = '<ps><p>' + Replace(@TypeString, ',', '</p><p>') + '</p></ps>'
--
--	--Convert the type XML into a table to join against
--	Declare @TypeTable Table (p Int)
--
--	Insert Into @TypeTable
--	Select
--	Params.p.value('.','Varchar(Max)')
--	From @TypeXml.nodes('/ps/p') as Params(p)
End

Declare @Datasources Table
(
	DatasourceId Int,
	[Name] Varchar(Max),
	Description Varchar(Max)
)

--Retrieve the datasource list
Insert Into @Datasources
Select cd.Id, [Name], Description
From ConfiguredDatasources cd
Join Urls u On u.Id = cd.UrlId
Where (@TypeString Is Null Or (cd.DatasourceTypeId In (Select p From @TypeTable)))
And u.Url = @Url

Select DatasourceId Id, [Name], Description
From @Datasources

Select cd.Id, dpt.Description, dp.TextValue [Value]--, dpt.Masked, dpt.ShowInName
From ConfiguredDatasources cd
Join DatasourceParameterTypes dpt On dpt.DatasourceTypeId = cd.DatasourceTypeId
Join DatasourceParameters dp On dp.ConfiguredDatasourceId = cd.Id And dp.ParameterNumber = dpt.ParameterNumber
Where cd.Id In (Select DatasourceId From @Datasources)
Order By cd.Id, dpt.ParameterNumber

Go

GRANT EXECUTE ON dbo.[Api_GetDatasources] TO [Public]
Go

/*

select * from configuredDatasources
select * from DatasourceParameters
select * from DatasourceParametertypes
select * from datasourcetypes

[Api_GetDatasources] 'http://www.simpletracking.com', 1, Null
[Api_GetDatasources] 'http://www.simpletracking.com', 1, '1,2,3'
[Api_GetDatasources] 'http://www.ranktrend.com', 1, '1,2,3'

*/

