If exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_DatasourceDisplayName]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[Ds_DatasourceDisplayName]
GO

create function dbo.[Ds_DatasourceDisplayName] (@ConfiguredDatasourceId Int, @SubTypeId Int)
  Returns Varchar(Max)
As

/*

Summary: Determines the display name that should be used for
	a particular configured datasource.

*/

Begin

Declare @DisplayName Varchar(Max)
Declare @Name Varchar(MAX)
Declare @TypeName Varchar(MAX)
Declare @SubTypeName Varchar(MAX)
Declare @DisplayParameters Varchar(MAX)

Select @Name = cd.[Name], @TypeName = t.[Description], 
	@SubTypeName = Case When @SubTypeId Is Null Then Null Else dst.[Name] End
From ConfiguredDatasources cd
Join DatasourceTypes t On cd.DatasourceTypeId = t.Id
Left Outer Join DatasourceSubTypes dst On dst.DatasourceTypeId = cd.DatasourceTypeId
Where cd.Id = @ConfiguredDatasourceId
And ((@SubTypeId Is Null And dst.Id Is Null) Or dst.Id = @SubTypeId Or @SubTypeId Is Null)

If Len(@Name) > 0
	Begin
		Set @DisplayName = @Name + IsNull(' - ' + @SubTypeName, '')
	End
Else
	Begin
		--Build the parameter list
		Select @DisplayParameters = dp.TextValue
		From DatasourceParameters dp
		Join DatasourceParameterTypes dpt On dpt.ParameterNumber = dp.ParameterNumber
		Where dp.ConfiguredDatasourceId = @ConfiguredDatasourceId
		And ShowInName = 1

		Set @DisplayName = @TypeName + IsNull(' - ' + @SubTypeName, '')
			+ IsNull(' (' + @DisplayParameters + ')', '')
	End

Return @DisplayName

End

Go

/*

select * from configureddatasources
Select * From DatasourceTypes
select * from datasourcesubtypes
select * from datasourceparametertypes
select * from datasourceparameters

select dbo.Ds_DatasourceDisplayName(1, null)
select dbo.Ds_DatasourceDisplayName(41, null)
select dbo.Ds_DatasourceDisplayName(33, 2)
select dbo.Ds_DatasourceDisplayName(33, Null)

sp_columns configureddatasources
*/