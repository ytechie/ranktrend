if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Report_DatasourceCorrelations]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Report_DatasourceCorrelations]
Go

CREATE PROCEDURE dbo.[Report_DatasourceCorrelations]
	@UrlId Int
AS

Set Nocount On

Declare @Results Table
(
	ConfiguredDatasourceId Int,
	OtherDatasourceId Int,
	Correlation Float,
	StrengthLabel Varchar(Max)
)

Insert Into @Results (ConfiguredDatasourceId, OtherDatasourceId)
Select cd1.Id, cd2.Id
From ConfiguredDatasources cd1
Join ConfiguredDatasources cd2 On (cd1.UrlId = cd2.UrlId  And cd1.Id <> cd2.Id And cd2.Id > cd1.Id)
Where cd1.UrlId = @UrlId

Print Cast(@@Rowcount As Varchar(Max)) + ' total unique combinations'

Declare cur Cursor For
Select ConfiguredDatasourceId, OtherDatasourceId
From @Results

Open Cur

Declare @cdid1 Int, @cdid2 Int
Declare @CorrelationResultTable Table (Correlation Float)

Fetch Next From Cur Into @cdid1, @cdid2

While @@Fetch_Status = 0
Begin
	Delete From @CorrelationResultTable

	Insert Into @CorrelationResultTable (Correlation)
		Exec Ds_GetCorrelation @cdid1, @cdid2

	Print 'Processing ' + Cast(@cdid1 As Varchar(Max)) + ' and ' + Cast(@cdid2 As Varchar(Max))

	Update @Results
	Set Correlation = IsNull((Select Correlation From @CorrelationResultTable), 0)
	Where ConfiguredDatasourceId = @cdid1
	And OtherDatasourceId = @cdid2

	Fetch Next From Cur Into @cdid1, @cdid2
End

Close Cur
Deallocate Cur

Update @Results
Set StrengthLabel =
	Case When Correlation >= .5 Then 'Large'
		When Correlation >= .3 Then 'Medium'
		When Correlation >= .1 Then 'Small'
		Else 'None' End

Select r.*, dbo.Ds_DatasourceDisplayName(cd.Id, Null) [Name], dbo.Ds_DatasourceDisplayName(cd2.Id, Null) OtherName
From @Results r
Join ConfiguredDatasources cd On r.ConfiguredDatasourceId = cd.Id
Join ConfiguredDatasources cd2 On r.OtherDatasourceId = cd2.Id
Where Correlation >= .1
Order By Correlation Desc

Go

GRANT EXECUTE ON dbo.[Report_DatasourceCorrelations] TO [Public]
Go

/*

[Report_DatasourceCorrelations] 8
[Report_DatasourceCorrelations] 3

select * from urls

*/

