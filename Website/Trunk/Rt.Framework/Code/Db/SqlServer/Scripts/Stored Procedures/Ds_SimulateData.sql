if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_SimulateData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_SimulateData]
Go

CREATE PROCEDURE dbo.[Ds_SimulateData]
	@ConfiguredDatasourceId Int,
	@Start DateTime,
	@End DateTime = Null,
	@MaxVariation Int = 3,
	@Min Int = 0,
	@Max Int = 50,
	@StartVal Float = 10.0,
	@OverwriteExisting Bit = 0
AS

/*

*/

Declare @CurrVal Float
Declare @TimeCursor DateTime

If @End Is Null
	Set @End = GetUtcDate()

If @OverwriteExisting = 1
	Delete From RawData
	Where ConfiguredDatasourceId = @ConfiguredDatasourceId
	And [Timestamp] Between @Start And @End

Set @CurrVal = @StartVal
Set @TimeCursor = @Start

While @TimeCursor < @End
	Begin
		Select @CurrVal = @CurrVal + Round((Rand()*@MaxVariation*2) - @MaxVariation, 0)
		Set @TimeCursor = DateAdd(d, 1, @TimeCursor)

		Insert Into RawData
		(ConfiguredDatasourceId, [Timestamp], FloatValue, Success)
		Values(@ConfiguredDatasourceId, @TimeCursor, @CurrVal, 1)
	End

Go

GRANT EXECUTE ON dbo.[Ds_SimulateData] TO [Public]
Go

/*

select * from configuredDatasources
select * from rawData

[Ds_SimulateData] 32, '12/1/06', Null, 5, 0, 50, 10, 1

*/

