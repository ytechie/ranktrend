if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Api_ReplaceKeywords]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Api_ReplaceKeywords]
Go

CREATE PROCEDURE dbo.[Api_ReplaceKeywords]
	@Url Varchar(Max),
	@Keywords Varchar(Max)
AS

Declare @UrlId Int
Select @UrlId = Id
From Urls
Where Url = @Url

Declare @KeywordTable Table (Keyword Varchar(Max))

Insert Into @KeywordTable
Select Cast(Item As Varchar(Max))
From [GenericStringSplit](@Keywords, ',')

Delete From ConfiguredDatasources Where Id In
(
	Select cd.Id
	From ConfiguredDatasources cd
	Join DatasourceParameters dp On dp.ConfiguredDatasourceId = cd.Id
	Where cd.DatasourceTypeId In (1,2,7)
	And dp.ParameterNumber = 1
	And dp.TextValue Not In (Select Keyword From @KeywordTable)
	And cd.UrlId = @UrlId
)

--New rows to insert

Declare @NewKeywordRecords Table
(
	Id Int Identity,
	DatasourceTypeId Int,
	Keyword Varchar(Max)
)

Insert Into @NewKeywordRecords
(DatasourceTypeId, Keyword)
Select dt.Id [DatasourceTypeId], Keyword
From @KeywordTable
Join DatasourceTypes dt On dt.Id In (1,2,7)

--Don't need to keep the keywords that are already saved
Delete
From @NewKeywordRecords
Where Cast(DatasourceTypeId As Varchar(20)) + Keyword In
(
	Select Cast(cd.DatasourceTypeId As Varchar(20)) + dp.TextValue
	From @NewKeywordRecords nk
	Join ConfiguredDatasources cd On 1=1
	Join DatasourceParameters dp On dp.ConfiguredDatasourceId = cd.Id
	Where cd.DatasourceTypeId In (1,2,7)
	And dp.ParameterNumber = 1
	And cd.UrlId = @UrlId
)

While (Select Count(*) From @NewKeywordRecords) > 0
Begin
	Declare @CurrKeywordId Int
	Declare @DatasourceTypeId Int
	Declare @Keyword Varchar(Max)
	Select Top 1 @CurrKeywordId = Id From @NewKeywordRecords
	Select Top 1 @DatasourceTypeId = DatasourceTypeId From @NewKeywordRecords
	Select Top 1 @Keyword = Keyword From @NewKeywordRecords

	Exec Ds_CreateSearchDatasource @UrlId, @DatasourceTypeId, @Keyword

	Delete From @NewKeywordRecords
	Where Id = @CurrKeywordId
End

Go

GRANT EXECUTE ON dbo.[Api_ReplaceKeywords] TO [Public]
Go

/*

[Api_ReplaceKeywords] 'http://www.superjason.com', 'adsf,there,jason'

select * from configureddatasources where urlid = 1
select * from datasourcetypes
select * from datasourceparameters
select * from urls

*/

